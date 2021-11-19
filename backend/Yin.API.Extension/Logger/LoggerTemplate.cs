using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yin.API.Extension.Logger
{
    public class LoggerTemplate
    {
        public Guid Id { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorSource { get; set; }
        public string ErrorStackTrace { get; set; }
        public DateTime LogTime { get; }

        public LoggerTemplate(string level, string message, string errorMessage, string errorSource, string errorStackTrace)
        {
            Id = Guid.NewGuid();
            Level = level;
            Message = message;
            ErrorMessage = errorMessage;
            ErrorSource = errorSource;
            ErrorStackTrace = errorSource;
            LogTime = DateTime.Now;
        }
    }
}
