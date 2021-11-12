using EasyCaching.Core.Interceptor;
using System;

namespace Yin.Infrastructure.Cache
{
    public class EasyCachingAbleCustomerAttribute : EasyCachingAbleAttribute
    {
        public new int Expiration { get; set; } = 60 * 60 * 60 + new Random().Next(10, 20) * 60 * 60;
    }
}
