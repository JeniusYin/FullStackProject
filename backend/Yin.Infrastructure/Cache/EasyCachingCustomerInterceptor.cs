using AspectCore.DynamicProxy;
using EasyCaching.Core.Interceptor;
using EasyCaching.Interceptor.AspectCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Yin.Infrastructure.Cache
{
    public class EasyCachingCustomerInterceptor : EasyCachingInterceptor
    {
        /// <summary>
        /// The typeof task result method.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, MethodInfo>
            TypeofTaskResultMethod = new ConcurrentDictionary<Type, MethodInfo>();

        /// <summary>
        /// The typeof task result method.
        /// </summary>
        private static readonly ConcurrentDictionary<MethodInfo, object[]>
            MethodAttributes = new ConcurrentDictionary<MethodInfo, object[]>();
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            //Process any early evictions 
            await ProcessEvictAsync(context, true);

            //Process any cache interceptor 
            await ProceedAbleAsync(context, next);

            // Process any put requests
            await ProcessPutAsync(context);

            // Process any late evictions
            await ProcessEvictAsync(context, false);
        }
        private object[] GetMethodAttributes(MethodInfo mi)
        {
            return MethodAttributes.GetOrAdd(mi, mi.GetCustomAttributes(true));
        }

        /// <summary>
        /// Proceeds the able async.
        /// </summary>
        /// <returns>The able async.</returns>
        /// <param name="context">Context.</param>
        /// <param name="next">Next.</param>
        private async Task ProceedAbleAsync(AspectContext context, AspectDelegate next)
        {
            if (GetMethodAttributes(context.ServiceMethod).FirstOrDefault(x => typeof(EasyCachingAbleCustomerAttribute).IsAssignableFrom(x.GetType())) is EasyCachingAbleCustomerAttribute attribute)
            {
                var returnType = context.IsAsync()
                        ? context.ServiceMethod.ReturnType.GetGenericArguments().First()
                        : context.ServiceMethod.ReturnType;

                var cacheKey = string.Empty;
                try
                {
                    cacheKey = KeyGenerator.GetCacheKey(context.ServiceMethod, context.Parameters, attribute.CacheKeyPrefix);
                }
                catch (Exception e)
                {
                    Logger.LogError("创建cache key 出错", e);
                    await next(context);
                    return;
                }
                if (string.IsNullOrEmpty(cacheKey))
                {
                    await next(context);
                    return;
                }
                object cacheValue = null;
                var isAvailable = true;
                try
                {
                    if (attribute.IsHybridProvider)
                    {
                        cacheValue = await HybridCachingProvider.GetAsync(cacheKey, returnType);
                    }
                    else
                    {
                        var _cacheProvider = CacheProviderFactory.GetCachingProvider(attribute.CacheProviderName ?? Options.Value.CacheProviderName);
                        cacheValue = await _cacheProvider.GetAsync(cacheKey, returnType);
                    }
                }
                catch (CacheException ex)
                {
                    if (!attribute.IsHighAvailability) throw;
                    else Logger?.LogError(new EventId(), ex, $"Cache provider remove error.");
                }
                catch (Exception ex)
                {
                    if (!attribute.IsHighAvailability)
                    {
                        throw;
                    }
                    else
                    {
                        isAvailable = false;
                        Logger?.LogError(new EventId(), ex, $"Cache provider get error.");
                    }
                }

                if (cacheValue != null)
                {
                    if (context.IsAsync())
                    {
                        //#1
                        //dynamic member = context.ServiceMethod.ReturnType.GetMember("Result")[0];
                        //dynamic temp = System.Convert.ChangeType(cacheValue.Value, member.PropertyType);
                        //context.ReturnValue = System.Convert.ChangeType(Task.FromResult(temp), context.ServiceMethod.ReturnType);

                        //#2                                               
                        context.ReturnValue =
                            TypeofTaskResultMethod.GetOrAdd(returnType, t => typeof(Task).GetMethods().First(p => p.Name == "FromResult" && p.ContainsGenericParameters).MakeGenericMethod(returnType)).Invoke(null, new object[] { cacheValue });
                    }
                    else
                    {
                        //context.ReturnValue = System.Convert.ChangeType(cacheValue.Value, context.ServiceMethod.ReturnType);
                        context.ReturnValue = cacheValue;
                    }
                }
                else
                {
                    // Invoke the method if we don't have a cache hit
                    await next(context);

                    if (isAvailable)
                    {
                        // get the result
                        var returnValue = context.IsAsync()
                            ? await context.UnwrapAsyncReturnValue()
                            : context.ReturnValue;

                        // should we do something when method return null?
                        // 1. cached a null value for a short time
                        // 2. do nothing
                        if (returnValue != null)
                        {
                            if (attribute.IsHybridProvider)
                            {
                                await HybridCachingProvider.SetAsync(cacheKey, returnValue, TimeSpan.FromSeconds(attribute.Expiration));
                            }
                            else
                            {
                                var _cacheProvider = CacheProviderFactory.GetCachingProvider(attribute.CacheProviderName ?? Options.Value.CacheProviderName);
                                await _cacheProvider.SetAsync(cacheKey, returnValue, TimeSpan.FromSeconds(attribute.Expiration));
                            }
                        }
                    }
                }
            }
            else
            {
                // Invoke the method if we don't have EasyCachingAbleAttribute
                await next(context);
            }
        }

        /// <summary>
        /// Processes the put async.
        /// </summary>
        /// <returns>The put async.</returns>
        /// <param name="context">Context.</param>
        private async Task ProcessPutAsync(AspectContext context)
        {
            if (GetMethodAttributes(context.ServiceMethod).FirstOrDefault(x => typeof(EasyCachingPutAttribute).IsAssignableFrom(x.GetType())) is EasyCachingPutAttribute attribute && context.ReturnValue != null)
            {
                var cacheKey = KeyGenerator.GetCacheKey(context.ServiceMethod, context.Parameters, attribute.CacheKeyPrefix);

                try
                {
                    // get the result
                    var returnValue = context.IsAsync()
                        ? await context.UnwrapAsyncReturnValue()
                        : context.ReturnValue;

                    if (attribute.IsHybridProvider)
                    {
                        await HybridCachingProvider.SetAsync(cacheKey, returnValue, TimeSpan.FromSeconds(attribute.Expiration));
                    }
                    else
                    {
                        var _cacheProvider = CacheProviderFactory.GetCachingProvider(attribute.CacheProviderName ?? Options.Value.CacheProviderName);
                        await _cacheProvider.SetAsync(cacheKey, returnValue, TimeSpan.FromSeconds(attribute.Expiration));
                    }
                }
                catch (CacheException ex)
                {
                    if (!attribute.IsHighAvailability) throw;
                    else Logger?.LogError(new EventId(), ex, $"Cache provider remove error.");
                }
                catch (Exception ex)
                {
                    if (!attribute.IsHighAvailability) throw;
                    else Logger?.LogError(new EventId(), ex, $"Cache provider set error.");
                }
            }
        }

        /// <summary>
        /// Processes the evict async.
        /// </summary>
        /// <returns>The evict async.</returns>
        /// <param name="context">Context.</param>
        /// <param name="isBefore">If set to <c>true</c> is before.</param>
        private async Task ProcessEvictAsync(AspectContext context, bool isBefore)
        {
            if (GetMethodAttributes(context.ServiceMethod).FirstOrDefault(x => typeof(EasyCachingEvictCustomerAttribute).IsAssignableFrom(x.GetType())) is EasyCachingEvictCustomerAttribute attribute && attribute.IsBefore == isBefore)
            {
                try
                {
                    if (attribute.IsAll)
                    {
                        var evictAllPrefixKeyList = new List<string>()
                        {
                            attribute.CacheKeyPrefix
                        };
                        evictAllPrefixKeyList.AddRange(attribute.EvictAllPrefixKeyList);
                        await RemoveKey(attribute, context, evictAllPrefixKeyList, true);
                    }
                    else if (attribute.EvictAllPrefixKeyList?.Any() == true)
                    {
                        await RemoveKey(attribute, context, attribute.EvictAllPrefixKeyList, true);
                        if (!string.IsNullOrEmpty(attribute.CacheKeyPrefix))
                            await RemoveKey(attribute, context, new List<string>() { attribute.CacheKeyPrefix }, false);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(attribute.CacheKeyPrefix))
                            await RemoveKey(attribute, context, new List<string>() { attribute.CacheKeyPrefix }, false);
                    }
                }
                catch (CacheException ex)
                {
                    if (!attribute.IsHighAvailability) throw;
                    else Logger?.LogError(new EventId(), ex, $"Cache provider remove error.");
                }
                catch (Exception ex)
                {
                    if (!attribute.IsHighAvailability) throw;
                    else Logger?.LogError(new EventId(), ex, $"Cache provider remove error.");
                }

                async Task RemoveKey(EasyCachingEvictCustomerAttribute attribute, AspectContext context, List<string> cachePreKeys, bool isRemovePrefix)
                {
                    foreach (var cachePreKey in cachePreKeys)
                    {
                        var cacheKey = KeyGenerator.GetCacheKey(context.ServiceMethod, context.Parameters,
                            cachePreKey);
                        if (cacheKey.Contains(","))
                        {
                            var cacheKeyComb = cacheKey.Split(",", StringSplitOptions.RemoveEmptyEntries);
                            await RemoveKey(attribute, context, cacheKeyComb.ToList(), isRemovePrefix);
                            break;
                        }

                        if (attribute.IsHybridProvider)
                        {
                            if (isRemovePrefix)
                            {
                                await HybridCachingProvider.RemoveByPrefixAsync(cacheKey);
                            }
                            else
                            {
                                await HybridCachingProvider.RemoveAsync(cacheKey);
                            }
                        }
                        else
                        {
                            var _cacheProvider =
                                CacheProviderFactory.GetCachingProvider(
                                    attribute.CacheProviderName ?? Options.Value.CacheProviderName);
                            if (isRemovePrefix)
                            {
                                await _cacheProvider.RemoveByPrefixAsync(cacheKey);
                            }
                            else
                            {
                                await _cacheProvider.RemoveAsync(cacheKey);
                            }
                        }
                        Logger.LogInformation($"remove：{cacheKey}  isPrefix:{isRemovePrefix}");
                    }
                }
            }
        }
    }
}
