using Microsoft.Extensions.Logging;

namespace Yin.API.Extension.Logger
{
    public static class CustomerLoggerFactory
    {
        public static ILoggerFactory CustomerLogger = LoggerFactory.Create(t => t.AddConsole());           
    }
}
