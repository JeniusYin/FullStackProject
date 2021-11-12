using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yin.HttpCommon.Logger
{
    public class ESLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ESLogger> _loggers = new ConcurrentDictionary<string, ESLogger>();
        private readonly ESLogger _esLogger;

        public ESLoggerProvider(ESLogger esLogger)
        {
            _esLogger = esLogger;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, _esLogger);
        }

        public void Dispose() => _loggers.Clear();
    }
}
