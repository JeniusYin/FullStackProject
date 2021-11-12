using Yin.EntityFrameworkCore.Context;
using Yin.HttpCommon.Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomerDbContext
    {
        public static IServiceCollection AddCustomerDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var dbOption = new Func<string, Action<DbContextOptionsBuilder>>(connection =>
            {
                return option =>
                    option
#if DEBUG
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging()
#endif
                        .UseLoggerFactory(CustomerLoggerFactory.CustomerLogger)
                        .UseMySql(connection, ServerVersion.AutoDetect(connection),
                            providerOption =>
                            {
                                // 弹性连接
                                // https://docs.microsoft.com/zh-cn/ef/core/miscellaneous/connection-resiliency
                                providerOption.EnableRetryOnFailure();
                                providerOption.MigrationsAssembly("Yin.EntityFrameworkCore.DbMigrations");
                            });
            });

            //读写DbContext
            services.AddDbContext<MyDbContext>(dbOption(configuration.GetConnectionString("MySql")));
            //只读DbContext
            services.AddDbContext<MyReadDbContext>(dbOption(configuration.GetConnectionString("MySql")));
            services.AddScoped<MyReadDbContextFactory>();

            services.AddScoped<IDbManager>(t =>
            {
                var context = t.GetRequiredService<MyDbContext>();
                return context;
            });
            return services;
        }
    }
}
