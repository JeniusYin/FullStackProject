using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Guids;

namespace Yin.HttpCommon.CustomerGuid
{
    public static class CustomerGuid
    {
        public static IServiceCollection AddCustomerGuid(this IServiceCollection services)
        {
            services.AddOptions<AbpSequentialGuidGeneratorOptions>()
                .Configure(t => t.DefaultSequentialGuidType = SequentialGuidType.SequentialAsString);
            services.AddTransient<IGuidGenerator, SequentialGuidGenerator>();
            return services;
        }
    }
}
