using AspectCore.Configuration;
using AspectCore.Extensions.Autofac;
using Autofac;
using EasyCaching.Core.Configurations;
using EasyCaching.Core.Interceptor;
using Yin.Infrastructure.Cache;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;

namespace Yin.HttpCommon.AutoFacDI
{
    public class AspectCoreModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EasyCachingKeyCustomerGenerator>().As<IEasyCachingKeyGenerator>();

            builder.RegisterType<EasyCachingCustomerInterceptor>();

            var config = new EasyCachingInterceptorOptions()
            {
                CacheProviderName = "UserRedis"
            };

            var options = Options.Create(config);

            builder.Register(x => options);

            builder.RegisterDynamicProxy(configure =>
            {
                bool all(MethodInfo x) => x.CustomAttributes.Any(data => typeof(EasyCachingInterceptorAttribute).GetTypeInfo().IsAssignableFrom(data.AttributeType));

                configure.Interceptors.AddTyped<EasyCachingCustomerInterceptor>(all);
            });
        }
    }
}
