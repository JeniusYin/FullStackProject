using EasyNetQ;
using Yin.Application.EventBus;
using Yin.EntityFrameworkCore.Context;
using Yin.API.Extension.Event;
using Yin.API.Extension.Logger;
using Yin.Infrastructure.Event;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomerEventBus
    {
        /// <summary>
        /// 注册 订阅eventHandler
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomerEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqCon = new RabbitMQConfigOption();
            configuration.GetSection("RabbitMQCon").Bind(rabbitMqCon);
            if (rabbitMqCon.Port == default(ushort))
                throw new ArgumentException("RabbitMQCon bind error");
            services.RegisterEasyNetQ(s =>
                new ConnectionConfiguration()
                {
                    Hosts = new List<HostConfiguration>()
                    {
                        new HostConfiguration()
                        {
                            Host = rabbitMqCon.Host,
                            Port = rabbitMqCon.Port
                        }
                    },
                    UserName = rabbitMqCon.UserName,
                    Password = rabbitMqCon.Password,
                    Port = rabbitMqCon.Port
                });

            services.AddSingleton<EasyNetQHelper>();
            // 注册Cap
            services.AddCustomerCap(rabbitMqCon);
            // 注册bus
            services.AddScoped(typeof(Yin.Infrastructure.Event.IEventBus), typeof(InMemoryEventBus));
            return services;
        }


        private static void AddCustomerCap(this IServiceCollection services, RabbitMQConfigOption rabbitMqConfig)
        {
            var logger = CustomerLoggerFactory.CustomerLogger.CreateLogger(typeof(CustomerEventBus));
            services.AddCap(config =>
            {
                config.UseEntityFramework<MyDbContext>();
                config.UseRabbitMQ(mq =>
                {
                    mq.HostName = rabbitMqConfig.Host;
                    mq.UserName = rabbitMqConfig.UserName;
                    mq.Password = rabbitMqConfig.Password;
                    mq.ExchangeName = MqConst.DefaultExchangeName;
                });
                // 在CAP中，我们采用的交付保证为 At Least Once(至少一次)。
                //  cap.queue.{程序集名称}  在 RabbitMQ 中映射到 Queue Names
                config.FailedRetryInterval = 60;
                // 在消息发送的时候 默认 60s，如果发送失败，CAP将会对消息进行重试，此配置项用来配置每次重试的间隔时间。
                config.ConsumerThreadCount = 1;
                // 消费者线程并行处理消息的线程数 默认 1 ，当这个值大于1时，将不能保证消息执行的顺序。
                config.FailedRetryCount = 50;
                // 重试的最大次数  50。当达到此设置值时，将不会再继续重试，通过改变此参数来设置重试的最大次数
                config.FailedThresholdCallback = info =>
                {
                    logger.LogError("cap 发送 {type} {content} 重试达到最高次数失败 {@info}", info.MessageType, info.Message,
                        info);
                };
                config.UseDashboard();
            });
        }
    }
}
