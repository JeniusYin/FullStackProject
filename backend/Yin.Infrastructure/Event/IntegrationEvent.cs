using Newtonsoft.Json;
using System;

namespace Yin.Infrastructure.Event
{
    public class IntegrationEvent<T> : IIntegrationEvent<T>

    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <param name="description"></param>
        public IntegrationEvent(string topic, T message, string description = "")
        {
            Topic = topic;
            Message = message;
            Description = description;
            Init();
        }
        public IntegrationEvent(string exchange, string queue, string topic, T message, string description = "")
        {
            Exchange = exchange;
            Queue = queue;
            Topic = topic;
            Message = message;
            Description = description;
            Init();
        }


        public IntegrationEvent(T message, string description = "")
        {
            Message = message;
            Description = description;
            Init();
        }

        private void Init()
        {
            Id = Guid.NewGuid();
            CreateTime = DateTime.Now;
        }

        [JsonConstructor]
        private IntegrationEvent()
        {

        }
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public object Message { get; set; }
        public string Topic { get; set; }
        public string ForwardTopic { get; set; }
        public string Description { get; set; }
        /// <summary>
        ///  使用 cap 发布消息时，当前属性设置无效，exchange 与 cap配置的保持一致
        ///  使用 easyNetQ 发布消息时，使用当前属性
        /// </summary>
        public string Exchange { get; set; }
        public string Queue { get; set; }
    }
}
