using Microsoft.Extensions.Logging;

namespace Yin.HttpCommon.Logger
{
    public static class CustomerLoggerFactory
    {
        public static ILoggerFactory CustomerLogger = LoggerFactory.Create(t => t.AddConsole());           
    }
}
