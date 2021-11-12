using System;
using System.Threading.Tasks;

namespace Yin.Infrastructure.Redis
{
    public interface IRedisLockKey
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource">锁定的对象</param>
        /// <param name="expiredTime">锁定过期时间，锁区域内的逻辑执行如果超过过期时间，锁将被释放</param>
        /// <param name="waitTime">等待时间,相同的 resource 如果当前的锁被其他线程占用,最多等待时间</param>
        /// <param name="retryTime">等待时间内，多久尝试获取一次</param>
        /// <param name="action"></param>
        /// <returns></returns>
        Task<T> LockWorkAsync<T>(string resource, TimeSpan expiredTime, TimeSpan waitTime, TimeSpan retryTime,
            Func<Task<T>> action);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource">锁定的对象</param>
        /// <param name="expiredTime">锁定过期时间，锁区域内的逻辑执行如果超过过期时间，锁将被释放</param>
        /// <param name="waitTime">等待时间,相同的 resource 如果当前的锁被其他线程占用,最多等待时间</param>
        /// <param name="retryTime">等待时间内，多久尝试获取一次</param>
        /// <param name="action"></param>
        /// <returns></returns>
        T LockWork<T>(string resource, TimeSpan expiredTime, TimeSpan waitTime, TimeSpan retryTime,
            Func<T> action);

        Task<T> LockWorkAsync<T>(string resource, TimeSpan expiredTime, Func<Task<T>> action);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource">锁定的对象</param>
        /// <param name="expiredTime">锁定过期时间，锁区域内的逻辑执行如果超过过期时间，锁将被释放</param>
        /// <param name="action"></param>
        /// <returns></returns>
        T LockWork<T>(string resource, TimeSpan expiredTime, Func<T> action);
    }
}
