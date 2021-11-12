using Yin.EntityFrameworkCore.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomerHealthCheck
    {
        public static IServiceCollection AddCustomerHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<MyDbContext>(name: "MyDbContext")
                .AddMySql(configuration.GetConnectionString("MySql"), name: "MySql")
                .AddRedis(redisConnectionString: configuration["Redis:Connection"])
                //.AddRabbitMQ(rabbitConnectionString: Configuration["RabbitMQCon:Amqp"], name: "RabbitMQ")
                ;
            return services;
        }


        public static void UseCustomerHealthCheck(this IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = (context, result) =>
                {
                    context.Response.ContentType = "application/json";
                    var str = JsonConvert.SerializeObject(new
                    {
                        result,
                        environment = env.EnvironmentName
                    }, Formatting.Indented,
                        new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    return context.Response.WriteAsync(str);
                }
            });

        }
    }
}
