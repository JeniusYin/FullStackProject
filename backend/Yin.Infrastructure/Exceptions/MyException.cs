using System;

namespace Yin.Infrastructure.Exceptions
{
    /// <summary>
    /// 逻辑错误异常
    /// </summary>
    public class MyException : Exception
    {
        public int Code { get; set; }
        public MyException()
        { }

        public MyException(string message, int code = 100)
            : base(message)
        {
            Code = code;
        }

        public MyException(string message, Exception innerException, int code = 100)
            : base(message, innerException)
        {
            Code = code;
        }
    }
}
