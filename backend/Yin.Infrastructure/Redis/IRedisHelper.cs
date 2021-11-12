using StackExchange.Redis;

namespace Yin.Infrastructure.Redis
{
    public interface IRedisHelper
    {
        IDatabase GetDatabase(int db = 72);
    }
}
