using DotNetCore.CAP;
using Yin.Application.EventBus;
using Yin.EntityFrameworkCore.Context;
using Yin.Infrastructure.Event;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Yin.Application.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly MyDbContext _dbContext;
        private readonly ICapPublisher _capPublisher;
        private readonly IInMemoryEventBus _eventBus;
        public TransactionBehaviour(
            MyDbContext dbContext,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger,
            IEventBus eventBus,
            ICapPublisher capPublisher)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(MyDbContext));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
            _eventBus = eventBus as IInMemoryEventBus;
            _capPublisher = capPublisher;
        }
        /// <summary>
        /// https://docs.microsoft.com/zh-cn/ef/core/miscellaneous/connection-resiliency
        /// 执行策略和事务
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();
                // 如果发生暂时性故障，执行策略将再次调用委托
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = _dbContext.BeginTransaction(_capPublisher))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                        // 可以发布 聚合领域事件
                        if (!_eventBus.IsEmpty()) await _eventBus.PublishEvent(_capPublisher);
                        await _dbContext.CommitTransactionAsync(transaction);
                    }
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}
