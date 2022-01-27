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

        /// <summary>
        /// 默认 Prefetch count = 50
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="action"></param>
        /// <param name="remark"></param>
        public void Subscribe<TEvent>(string exchangeName, string routingKey, Func<TEvent, bool> action,
            string remark = "") where TEvent : class
        {
            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentNullException(nameof(exchangeName), $"{nameof(exchangeName)} cannot be null");
            string queueName = routingKey + ".queue";
            var advancedBus = _bus.Advanced;
            var exchange = advancedBus.ExchangeDeclare(name: exchangeName, ExchangeType.Topic);
            var queue = advancedBus.QueueDeclare(name: queueName);
            advancedBus.Bind(exchange, queue, routingKey);
            var advance = advancedBus.Consume(queue, (body, processInfo, info) =>
            {
                processInfo.Headers.TryGetValue("cap-msg-id", out object capIdObj);
                var capId = "null";
                if (capIdObj is byte[])
                {
                    capId = Encoding.UTF8.GetString(capIdObj as byte[]);
                }

                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"mq:Subscribe[cap]: 开始处理消息:{remark} {info.Exchange}:{info.Queue}");
                var model = JsonConvert.DeserializeObject<TEvent>(message);

                if (model != default(TEvent))
                {
                    var result = false;
                    try
                    {
                        result = action(model);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"mq: 执行方法出错 {remark}");
                    }
                }
                else
                {
                    // 消息异常
                    _logger.LogError(
                        "mq:Subscribe[cap][失败] 消息内容异常 {remark} event:{@event}, info:{@info},prop:{@prop}", remark, model, info, processInfo);
                }
            });
        }
    }
}
