using System;

namespace Yin.Redis
{
    public class RedLockException : Exception
    {
        public int Code { get; private set; }
        public override string Message { get; }
        public RedLockException(string message)
        {
            Message = message;
        }

        public RedLockException(string message, int code = 100) : this(message)
        {
            Code = code;
        }
    }
}
