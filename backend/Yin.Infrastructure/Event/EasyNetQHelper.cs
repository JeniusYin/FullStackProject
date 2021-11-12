using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yin.Infrastructure.Event
{
    public class EasyNetQHelper
    {
        private readonly IBus _bus;
        private readonly ILogger _logger;
        public EasyNetQHelper(
            IBus bus, ILogger<EasyNetQHelper> logger)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(IBus), "EasyNetQ获取实例失败");
            _logger = logger;
        }

        public async Task PublishImmediately(List<IIntegrationEvent<object>> @events)
        {
            foreach (var @event in @events)
            {
                await PublishAsync(
                    @event.Exchange,
                    @event.Topic,
                    @event,
                    @event.Queue);
                _logger.LogInformation("EventBusId: 发送 事件 {des} {exchange} {queue} {topic} -- {@event}",
                    @event.Description, @event.Exchange, @event.Queue ?? string.Empty, @event.Topic, @event);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routeKey">topic</param>
        /// <param name="message">消息体</param>
        /// <param name="queueName">队列名称(若队列不存在，消息会丢失)</param>
        /// <returns></returns>
        public async Task PublishAsync<T>(string exchangeName, string routeKey, T message, string queueName = null)
        {
            var advancedBus = _bus.Advanced;
            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentNullException(nameof(exchangeName), $"{nameof(exchangeName)} cannot be null");
            var exchange = await advancedBus.ExchangeDeclareAsync(name: exchangeName, ExchangeType.Topic);
            var properties = new MessageProperties();
            // header 属性 CAP在收到消息时能够提取到关键特征进行操作
            // https://cap.dotnetcore.xyz/user-guide/zh/cap/messaging/
            properties.Headers.Add("cap-msg-id", Guid.NewGuid().ToString());
            properties.Headers.Add("cap-msg-name", routeKey);
            properties.Headers.Add("cap-msg-type", typeof(T).FullName);
            properties.Headers.Add("cap-senttime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            if (!string.IsNullOrEmpty(queueName))
            {
                var queue = await advancedBus.QueueDeclareAsync(name: queueName, configure => { });
                await advancedBus.BindAsync(exchange, queue, routeKey, properties.Headers);
            }
            if (string.IsNullOrEmpty(routeKey))
                throw new ArgumentNullException(nameof(routeKey), "routeKey不能为空");

            await advancedBus.PublishAsync(
                exchange, routeKey, false, properties,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }
    }
}
