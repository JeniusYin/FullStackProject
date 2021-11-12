using System;

namespace Yin.Infrastructure.Event
{
    public interface IIntegrationEvent<out T>
    {
        /// <summary>
        /// 消息唯一id
        /// </summary>
        Guid Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }
        /// <summary>
        /// 消息体
        /// </summary>
        object Message { get; set; }
        /// <summary>
        /// 消息描述
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// exchange
        /// </summary>
        string Exchange { get; set; }
        /// <summary>
        /// queue
        /// </summary>
        string Queue { get; set; }
        /// <summary>
        /// topic
        /// </summary>
        string Topic { get; set; }

        /// <summary>
        /// 转发消息的 topic
        /// </summary>
        string ForwardTopic { get; set; }
    }
}
