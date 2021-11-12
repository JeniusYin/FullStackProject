using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yin.Infrastructure.Event
{
    /// <summary>
    ///  直接发送消息，清使用 easyNetQHelper
    ///  这个并发搞的情况会造成性能原因
    ///  有dbContext 使用这个
    /// </summary>
    public interface IEventBus
    {
        Guid _id { get; }
        /// <summary>
        /// 添加到内存中，saveChange 后发送
        /// </summary>
        /// <param name="event"></param>
        void AddPublish(IIntegrationEvent<IEventModel> @event);
        /// <summary>
        /// 直接发送 聚合事件(不会包含在事务中)会记录到数据库(cap)
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task PublishEventUseTransit(IIntegrationEvent<IEventModel> @event);
        /// <summary>
        /// 直接发送 聚合事件，会记录到数据库(cap)
        /// (不会开启事务)
        /// ps：直接发送消息，不需要和事务
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        Task PublishEventNoTransit(List<IIntegrationEvent<IEventModel>> @events);
        /// <summary>
        /// 使用EasyNetQ发送消息，不会进行发布者确认，不会记录到数据库
        /// </summary>
        /// <returns></returns>
        Task PublishImmediately(List<IIntegrationEvent<IEventModel>> @events);
    }
}
