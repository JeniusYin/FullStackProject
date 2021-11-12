using Yin.Infrastructure.Redis;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Yin.Infrastructure.Event
{
    public static class EventExtension
    {
        /// <summary>
        /// 订阅（只进行了消息幂等性校验）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="action"></param>
        /// <param name="actionName"></param>
        /// <param name="redisHelper"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task Subscribe<T>(this IntegrationEvenEto<T> model, Func<Task> action, string actionName,
            IRedisHelper redisHelper, ILogger logger)
        {
            if (model == null)
            {
                logger.LogError("{event}:当前接收消息异常,model 为null 请检查", actionName);
                return;
            }
            logger.LogInformation("接收到事件{@model}", model);
            if (model.Id == Guid.Empty)
            {
                // 当前消息数据异常
                logger.LogError("{event}:当前接收消息异常:{@model}", actionName, model);
                return;
            }

            var key = $"event:userevent:success:{model.Id}";
            if (await redisHelper.GetDatabase().KeyExistsAsync(key))
            {
                logger.LogWarning("当前消息已重复消费:{@model}", model);
                return;
            }

            await action.Invoke();
            // 消息消费记录默认存放1天 （主要保证消息幂等性）
            await redisHelper.GetDatabase().StringSetAsync(key, model, TimeSpan.FromDays(1));
            logger.LogInformation("event消费成功:{@event}", model);
        }
    }
}
