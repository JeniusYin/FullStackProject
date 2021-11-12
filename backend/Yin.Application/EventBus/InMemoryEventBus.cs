using DotNetCore.CAP;
using Yin.Infrastructure.Exceptions;
using Yin.EntityFrameworkCore.Context;
using Yin.Infrastructure.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yin.Application.EventBus
{
    public class InMemoryEventBus : IEventBus, IInMemoryEventBus
    {
        public Guid _id { get; }
        private readonly ILogger<InMemoryEventBus> _logger;
        private readonly ICapPublisher _capPublisher;
        private readonly MyDbContext _dbContext;
        private readonly EasyNetQHelper _easyNetQHelper;
        public InMemoryEventBus(
            ILogger<InMemoryEventBus> logger,
            ICapPublisher capPublisher,
            MyDbContext dbContext,
            EasyNetQHelper easyNetQHelper)
        {
            _logger = logger;
            _capPublisher = capPublisher;
            _dbContext = dbContext;
            _easyNetQHelper = easyNetQHelper;
            _id = Guid.NewGuid();
            _publishEvents = new ConcurrentBag<IIntegrationEvent<IEventModel>>();
        }
        /// <summary>
        ///  消息总线，
        /// </summary>
        private readonly ConcurrentBag<IIntegrationEvent<IEventModel>> _publishEvents;

        public IReadOnlyCollection<IIntegrationEvent<IEventModel>> PublishEvents => _publishEvents;
        public void AddPublish(IIntegrationEvent<IEventModel> @event)
        {
            if (@event?.Topic == null)
                throw new ArgumentException("Topic不能为空");
            if (_publishEvents.IsEmpty || _publishEvents.All(t => t.Id != @event.Id))
            {
                _publishEvents.Add(@event);
                _logger.LogInformation("EventBusId:{id} 添加事件 {des} {topic} -- {@event}",
                    _id, @event.Description, @event.Topic, @event);
            }
            else
            {
                _logger.LogWarning("EventBusId:{id} 重复添加事件 {des} {topic} -- {@event}",
                    _id, @event.Description, @event.Topic, @event);
            }
        }

        public bool IsEmpty()
        {
            return _publishEvents.IsEmpty;
        }

        public async Task PublishEvent(ICapPublisher capPublisher)
        {
            if (!_dbContext.HasActiveTransaction)
                throw new MyException("需开启事务使用当前方法");
            _logger.LogInformation("EventBusId:{id} 开始发送聚合事件", _id);

            foreach (var t in _publishEvents)
            {
                if (t?.Topic == null)
                    throw new ArgumentException("Topic不能为空");
                await capPublisher.PublishAsync(t.Topic, t);
                _logger.LogInformation("EventBusId:{id} 发送 事件 {des} {topic} -- {@event}",
                    _id, t.Description, t.Topic, t);
            }

            _publishEvents.Clear();
            _logger.LogInformation("EventBusId:{id} 结束发送聚合事件", _id);
        }

        public async Task PublishEventUseTransit(IIntegrationEvent<IEventModel> @event)
        {
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var trans = _dbContext.BeginTransaction(_capPublisher))
                {
                    await _capPublisher.PublishAsync(@event.Topic, @event);
                    _logger.LogInformation("EventBusId:{id} 发送 事件 {des} {topic} -- {@event}",
                        _id, @event.Description, @event.Topic, @event);
                    await _dbContext.CommitTransactionAsync(trans);
                }
            });
        }

        public async Task PublishEventNoTransit(List<IIntegrationEvent<IEventModel>> @events)
        {
            foreach (var @event in @events)
            {
                await _capPublisher.PublishAsync(@event.Topic, @event);
                _logger.LogInformation("EventBusId:{id} 发送 事件 {des} {topic} -- {@event}",
                    _id, @event.Description, @event.Topic, @event);
            }
        }

        public async Task PublishImmediately(List<IIntegrationEvent<IEventModel>> @events)
        {
            foreach (var @event in @events)
            {
                await _easyNetQHelper.PublishAsync(
                    @event.Exchange,
                    @event.Topic,
                    @event.Message,
                    @event.Queue);
                _logger.LogInformation("EventBusId:{id} 发送 事件 {des} {exchange} {queue} {topic} -- {@event}",
                    _id, @event.Description, @event.Exchange, @event.Queue, @event.Topic, @event);
            }
        }
    }
}
