using EasyCaching.Core.Interceptor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Yin.Infrastructure.Cache
{
    public class EasyCachingEvictCustomerAttribute : EasyCachingEvictAttribute
    {
        /// <summary>
        /// 移除所有 prefixKey
        /// </summary>
        public string EvictAllPrefixKeys { get; set; } = string.Empty;
        public List<string> EvictAllPrefixKeyList => string.IsNullOrEmpty(EvictAllPrefixKeys) ? new List<string>() : EvictAllPrefixKeys?.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
