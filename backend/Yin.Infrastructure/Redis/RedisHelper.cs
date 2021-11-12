using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Yin.Infrastructure.Redis
{
    public class RedisHelper : IRedisHelper
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly ILogger<RedisHelper> _logger;
        public RedisHelper(
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<RedisHelper> logger)
        {
            _connection = connectionMultiplexer;
            _logger = logger;
            RegisterEvent();
        }
        /// <summary>
        /// 社区默认使用 72
        /// </summary>
        /// <param name="db"></param>
        public IDatabase GetDatabase(int db = 72)
        {
            return _connection.GetDatabase(db);
        }

        private void RegisterEvent()
        {
            _connection.ConnectionFailed += (s, e) =>
            {
                _logger.LogError("redis连接失败:{@e}", e);
            };
            _connection.ErrorMessage += (s, e) =>
            {
                _logger.LogError("redis出现错误:{@e}", e);
            };
        }
    }
}
