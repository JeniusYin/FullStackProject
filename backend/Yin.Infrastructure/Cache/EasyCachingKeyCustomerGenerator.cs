using EasyCaching.Core.Interceptor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Yin.Infrastructure.Cache
{
    public class EasyCachingKeyCustomerGenerator : IEasyCachingKeyGenerator
    {
        private string LinkChar = ":";
        public string GetCacheKey(MethodInfo methodInfo, object[] args, string prefix)
        {
            var muchKeyTuple = new List<(string, string)>();
            var regs = Regex.Matches(prefix, @"\{(.+?)\}");
            var parameters = methodInfo.GetParameters().ToList();
            foreach (Match reg in regs)
            {
                if (reg.Success)
                {
                    bool isObject = false;
                    bool isRequired = false;
                    var prefixPar = reg.Groups[1].Value;
                    if (prefixPar.Contains(":"))
                    {
                        var prefixTags = prefix.Split(":", StringSplitOptions.RemoveEmptyEntries);
                        if (prefixTags.Length == 2)
                        {
                            if (prefixTags[1].Equals("Required", StringComparison.CurrentCultureIgnoreCase))
                            {
                                isRequired = true;
                            }

                            prefixPar = prefixTags[0];
                        }
                    }
                    var parField = prefixPar.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (prefixPar.Contains("."))
                    {
                        isObject = true;
                        prefixPar = parField[0];
                    }
                    var par = parameters.FindIndex(t =>
                        t.Name.Equals(prefixPar, StringComparison.CurrentCultureIgnoreCase));

                    if (par > -1)
                    {
                        if (args.Length > par)
                        {
                            if (isObject)
                            {
                                var value = parameters[par].ParameterType.GetProperty(parField[1]).GetValue(args[par], null);
                                if (value == null)
                                {
                                    if (isRequired)
                                    {
                                        throw new CacheException("获取缓存key模板值失败");
                                    }
                                    else
                                    {
                                        value = "null";
                                    }
                                }

                                prefix = prefix.Replace(reg.Groups[0].Value, value.ToString());
                            }
                            else
                            {
                                if (args[par] is string)
                                {
                                    prefix = prefix.Replace(reg.Groups[0].Value, args[par].ToString());
                                }
                                else
                                {
                                    if (args[par] is IEnumerable enumerable)
                                    {
                                        foreach (var arg in (IEnumerable)args[par])
                                        {
                                            muchKeyTuple.Add((reg.Groups[0].Value, arg.ToString()));
                                        }
                                    }
                                    else
                                    {
                                        prefix = prefix.Replace(reg.Groups[0].Value, args[par].ToString());
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        throw new CacheException("获取缓存参数失败");
                    }
                }
                else
                {
                    throw new CacheException("获取缓存key失效");
                }
            }

            if (muchKeyTuple.Any())
            {
                return string.Join(",", muchKeyTuple.Select(t => prefix.Replace(t.Item1, t.Item2)));
            }
            else
            {
                return prefix;
            }
        }
        public string GetCacheKeyPrefix(MethodInfo methodInfo, string prefix)
        {
            if (!string.IsNullOrWhiteSpace(prefix)) return $"{prefix}{LinkChar}";

            var typeName = methodInfo.DeclaringType?.Name;
            var methodName = methodInfo.Name;

            return $"{typeName}{LinkChar}{methodName}{LinkChar}";
        }
    }
}
