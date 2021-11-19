using EasyCaching.Core.Configurations;
using Yin.API.Extension.Redis;
using JsonNet.ContractResolvers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EasyCache
    {
        public static void AddCustomerEasyCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConList = new List<RedisConf>();
            configuration.GetSection("Redis:Endpoints").Bind(redisConList);
            if (redisConList?.Any() == false)
            {
                throw new ArgumentNullException(nameof(redisConList), "redis连接字符串不能为null");
            }

            var redisCon = redisConList.FirstOrDefault() ?? throw new ArgumentNullException(nameof(redisConList), "redis连接字符串不能为null");

            services.AddEasyCaching(option =>
            {
                // 可使用 MessagePack、Binary、Json、Protobuf
                option.WithJson(jsonSerializerSettingsConfigure: t =>
                    {
                        t.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        t.NullValueHandling = NullValueHandling.Ignore;
                        t.ContractResolver = new PrivateSetterCamelCasePropertyNamesContractResolver();
                    }, "cacheJson");

                option.UseRedis(t =>
                {
                    t.EnableLogging = true;
                    redisConList.ForEach(r =>
                    {
                        t.DBConfig.Endpoints.Add(new ServerEndPoint(r.Host, r.Port));
                    });
                    t.DBConfig.Password = redisCon.Password;
                    t.DBConfig.Database = redisCon.DefaultDatabase;
                    t.SerializerName = "cacheJson";
                    // 随机的秒数
                    t.MaxRdSecond = 1;
                }, "UserRedis");
            });
        }
    }
}
