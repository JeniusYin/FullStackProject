using Yin.API.Extension.Logger;
using Microsoft.Extensions.Configuration;
using Nest;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ElasticSearchMoudle
    {
        public static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ElasticSearch");
            if (!config.Exists())
                throw new ArgumentNullException("Configuration section 'ElasticSearch' not found");

            services.Configure<ElasticsearchOptions>(opt =>
            {
                opt.Url = config["Url"];
                opt.User = config["User"];
                opt.Password = config["Password"];
            });

            var settings = new ConnectionSettings(new Uri(config["Url"]))
                            .BasicAuthentication(config["User"], config["Password"]);
            //缩减日志 1.自定义日志模板，减少日志量 2.每次发布新建一个索引，减少索引量
            settings.DefaultIndex($"{config["DefaultIndex"]}-{DateTime.Now:yyyy.MM.dd}");
            services.AddSingleton<IElasticClient>(new ElasticClient(settings));
            services.AddSingleton<ESLogger>();
            return services;
        }
    }
}
