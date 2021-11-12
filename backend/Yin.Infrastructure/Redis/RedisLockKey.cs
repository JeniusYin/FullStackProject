using Yin.Redis;
using Microsoft.Extensions.Logging;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yin.Infrastructure.Redis
{
    public class RedisLockKey : IRedisLockKey
    {
        private readonly ILogger<RedisLockKey> _logger;
        private readonly IDistributedLockFactory _lockFactory;
        public RedisLockKey(
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<RedisLockKey> logger)
        {
            _lockFactory = RedLockFactory.Create(new List<RedLockMultiplexer>()
            {
                new RedLockMultiplexer(connectionMultiplexer)
            });
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource">锁定的对象</param>
        /// <param name="expiredTime">锁定过期时间，锁区域内的逻辑执行如果超过过期时间，锁将被释放</param>
        /// <param name="waitTime">等待时间,相同的 resource 如果当前的锁被其他线程占用,最多等待时间</param>
        /// <param name="retryTime">等待时间内，多久尝试获取一次</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<T> LockWorkAsync<T>(string resource, TimeSpan expiredTime, TimeSpan waitTime, TimeSpan retryTime, Func<Task<T>> action)
        {
            using (var redLock = await _lockFactory.CreateLockAsync(resource: resource, expiryTime: expiredTime,
                waitTime: waitTime, retryTime: retryTime))
            {
                if (redLock.IsAcquired)
                {
                    return await action.Invoke();
                }
                else
                {
                    throw new RedLockException($"resource:{resource} 获取分布式锁失败");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="expiredTime"></param>
        /// <param name="waitTime"></param>
        /// <param name="retryTime"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public T LockWork<T>(string resource, TimeSpan expiredTime, TimeSpan waitTime, TimeSpan retryTime, Func<T> action)
        {
            using (var redLock = _lockFactory.CreateLock(resource: resource, expiryTime: expiredTime,
                waitTime: waitTime, retryTime: retryTime))
            {
                if (redLock.IsAcquired)
                {
                    return action.Invoke();
                }
                else
                {
                    throw new RedLockException($"resource:{resource} 获取分布式锁失败");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="expiredTime"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<T> LockWorkAsync<T>(string resource, TimeSpan expiredTime, Func<Task<T>> action)
        {
            using (var redLock = await _lockFactory.CreateLockAsync(resource: resource, expiryTime: expiredTime))
            {
                if (redLock.IsAcquired)
                {
                    return await action.Invoke();
                }
                else
                {
                    throw new RedLockException($"resource:{resource} 获取分布式锁失败");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="expiredTime"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public T LockWork<T>(string resource, TimeSpan expiredTime, Func<T> action)
        {
            using (var redLock = _lockFactory.CreateLock(resource: resource, expiryTime: expiredTime))
            {
                if (redLock.IsAcquired)
                {
                    return action.Invoke();
                }
                else
                {
                    throw new RedLockException($"resource:{resource} 获取分布式锁失败");
                }
            }
        }
    }
}
