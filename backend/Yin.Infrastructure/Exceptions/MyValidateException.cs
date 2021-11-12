using System;

namespace Yin.Infrastructure.Exceptions
{
    /// <summary>
    /// 数据验证异常
    /// </summary>
    public class MyValidateException : Exception
    {
        public MyValidateException()
        { }

        public MyValidateException(string message)
            : base(message)
        { }

        public MyValidateException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
