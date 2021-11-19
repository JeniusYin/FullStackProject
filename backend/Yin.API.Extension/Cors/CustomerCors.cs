using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomerCors
    {
        private static string CorsPolicyName = "MyPolicy";
        public static IServiceCollection AddCustomerCors(this IServiceCollection services)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        public static void UseCustomerCors(this IApplicationBuilder app)
        {
            app.UseCors(CorsPolicyName);
        }
    }
}
