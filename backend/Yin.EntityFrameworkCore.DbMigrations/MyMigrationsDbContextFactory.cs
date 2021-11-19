using Yin.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Yin.EntityFrameworkCore.DbMigrations
{
    public class MyMigrationsDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            var config = GetConfiguration();
            var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
            optionsBuilder.UseMySql(ServerVersion.AutoDetect(config.GetConnectionString("MySql")), option =>
                 option.MigrationsAssembly("Yin.EntityFrameworkCore.DbMigrations"));
            return new MyDbContext(optionsBuilder.Options);
        }
        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Yin.API/"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    optional: true)
                .Build();
        }
    }
}
