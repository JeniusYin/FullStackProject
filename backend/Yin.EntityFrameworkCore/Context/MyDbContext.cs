using DotNetCore.CAP;
using Yin.Domain.Interfaces;
using Yin.EntityFrameworkCore.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Yin.EntityFrameworkCore.Context
{
    public class MyDbContext : DbContext, IUnitOfWork, IDbManager
    {
        private IDbContextTransaction _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction != null;

        private readonly IMediator _mediator;
        private readonly ILogger<MyDbContext> _logger;

        /// <summary>
        /// 设计时 DbContext 创建
        /// https://docs.microsoft.com/zh-cn/ef/core/miscellaneous/cli/dbcontext-creation
        /// </summary>
        /// <param name="options"></param>
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }


        public MyDbContext(DbContextOptions<MyDbContext> options,
            IMediator mediator,
            ILogger<MyDbContext> logger) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // 发送所有领域事件
            await _mediator.DispatchDomainEventsAsync(this);
            // commit到 数据库
            return await base.SaveChangesAsync(cancellationToken) > 0;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            return _currentTransaction;
        }
        public IDbContextTransaction BeginTransaction(ICapPublisher capPublisher)
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = Database.BeginTransaction(capPublisher);
            return _currentTransaction;
        }
        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                await SaveChangesAsync();
                transaction.Commit();
                sw.Stop();
                _logger.LogInformation($"CommitTransactionAsync耗时 {sw.ElapsedMilliseconds} ms");
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task ExecuteSql(FormattableString sql)
        {
            using (var transaction = await BeginTransactionAsync())
            {
                await Database.ExecuteSqlInterpolatedAsync(sql);
                await CommitTransactionAsync(transaction);
            }
        }

        public async Task ExecuteSqlRaw(string sql)
        {
            if (HasActiveTransaction)
            {
                await Database.ExecuteSqlRawAsync(sql);
                return;
            }
            var strategy = Database.CreateExecutionStrategy();
            // 如果发生暂时性故障，执行策略将再次调用委托
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await BeginTransactionAsync())
                {
                    await Database.ExecuteSqlRawAsync(sql);
                    await CommitTransactionAsync(transaction);
                }
            });
        }
    }
}
