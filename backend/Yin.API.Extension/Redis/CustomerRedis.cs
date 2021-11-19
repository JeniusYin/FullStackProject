using Yin.API.Extension.Redis;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomerRedis
    {
        public static IServiceCollection AddCustomerRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConList = new List<RedisConf>();
            configuration.GetSection("Redis:Endpoints").Bind(redisConList);
            if (redisConList?.Any() == false)
            {
                throw new ArgumentNullException(nameof(redisConList), "redis连接字符串不能为null");
            }

            var redisCon = redisConList.FirstOrDefault() ?? throw new ArgumentNullException(nameof(redisConList), "redis连接字符串不能为null");
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var conf = new ConfigurationOptions()
                {
                    Password = redisCon.Password,
                    DefaultDatabase = redisCon.DefaultDatabase
                };
                redisConList.ForEach(t =>
                {
                    conf.EndPoints.Add(t.Host, t.Port);
                    conf.ResolveDns = true;
                });
                return ConnectionMultiplexer.Connect(conf);
            });
            return services;
        }
    }
}
