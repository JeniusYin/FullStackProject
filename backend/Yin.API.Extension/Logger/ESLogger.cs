using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Yin.API.Extension.Logger
{
    public class ESLogger : ILogger
    {
        /// <summary>
        /// 日志处理队列
        /// </summary>
        private readonly BlockingCollection<LoggerTemplate> msgQueue = new BlockingCollection<LoggerTemplate>(50);

        /// <summary>
        /// ES客户端
        /// </summary>
        private readonly IElasticClient _elasticClient;
        public ESLogger(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
            //初始化消费者队列，若没有消息，任务将阻塞
            Task.Factory.StartNew(async () =>
            {
                while (msgQueue.TryTake(out var msg, Timeout.Infinite))
                {
                    await _elasticClient.IndexDocumentAsync(msg);
                }
            }, TaskCreationOptions.LongRunning);
        }


        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var loggerTemplate = new LoggerTemplate(logLevel.ToString(), formatter(state, exception), exception?.Message, exception?.Source, exception?.StackTrace);
            //添加到日志队列
            msgQueue.Add(loggerTemplate);
        }
    }
}
