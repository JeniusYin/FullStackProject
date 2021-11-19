using Autofac;
using Yin.Application.AutoMapper;
using Yin.API.Extension.AutoFacDI;
using Yin.API.Extension.Logger;
using Yin.API.Extension.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Yin.API.Extension;

namespace Yin.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(HttpCommonModule.ExceptionMvcOption)
                .AddCustomerValidation();

            services.AddElasticSearch(Configuration);
            services.AddCustomerRedis(Configuration);
            services.AddCustomerEasyCache(Configuration);
            services.AddCustomJwtAuthentication(Configuration);
            services.AddCustomerApiVersion();
            services.AddCustomSwagger();
            services.AddCustomerCors();
            services.AddCustomerDbContext(Configuration);
            services.AddCustomerEventBus(Configuration);
            services.AddAutoMapper(t => t.AddProfiles(AutoMapperConfig.GetAllMaProfiles()));
            services.AddCustomerHealthCheck(Configuration);
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<ApplicationModule>();
            builder.RegisterModule<ValidatorModule>();
            builder.RegisterModule<MediatorModule>();
            builder.RegisterModule<AspectCoreModule>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCustomerHealthCheck(env);

            app.UseCustomerSwagger(env);

            loggerFactory.AddProvider(new ESLoggerProvider(app.ApplicationServices.GetRequiredService<ESLogger>()));
            app.UseMiddleware<RequestLogMiddleWare>();

            app.UseRouting();

            app.UseCustomerResponse();

            app.UseCustomerCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
