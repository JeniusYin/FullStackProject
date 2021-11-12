using Yin.HttpCommon.Middleware;
using Yin.HttpCommon.Swagger;
using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// swagger 配置
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// 自定义swagger配置
        /// 版本控制 token认证
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {

            // 运行时 加载 assembly
            // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/580

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options =>
            {
                // https://github.com/Microsoft/aspnet-api-versioning/wiki/Swashbuckle-Integration
                options.OperationFilter<SwaggerDefaultValues>();
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/README.md#add-security-definitions-and-requirements
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "添加token认证头 Bearer:token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.CustomOperationIds(apiDesc =>
                {
                    var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                    return $"ID-{controllerAction.GetHashCode()}";
                });
                // 全局 token 认证
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }, new[] { "Bearer" } }
                });
                // 使用 fluentValidation 规则生成swagger文档
                // https://github.com/micro-elements/MicroElements.Swashbuckle.FluentValidation
                try
                {
                    var baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                    var files = Directory.EnumerateFileSystemEntries(baseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
                    files?.ToList().ForEach(t => options.IncludeXmlComments(t, true));
                }
                catch (Exception exception)
                {
                    // 原文件 可能没有
                    Console.WriteLine(exception);
                }
                options.EnableAnnotations();
                //options.DocumentFilter<TagDescriptionsDocumentFilter>();
                // 按照分组排序
                options.OrderActionsBy(t => t.GroupName);
            });
            services.AddFluentValidationRulesToSwagger();
            return services;
        }
        public static void UseCustomerSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                // 目前 只对 swagger 进行过滤请求
                app.UseMiddleware<CustomerBasicAuthMiddleware>();
            }
            app.UseSwagger(t =>
            {
                t.SerializeAsV2 = false;
                t.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    var url = $"https://{httpReq.Host.Value}";
                    if (httpReq.Host.Value.IndexOf("localhost", StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        url = $"{httpReq.Scheme}://{httpReq.Host.Value}";
                    }
                    swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = url } };
                });
            });

            app.UseSwaggerUI(option =>
            {
                var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var item in provider.ApiVersionDescriptions)
                {
                    option.SwaggerEndpoint($"/swagger/{item.GroupName}/swagger.json",
                        "api-" + item.ApiVersion);
                }

                option.RoutePrefix = "";
            });

            app.UseKnife4UI(c =>
            {
                c.RoutePrefix = "api-docs"; // serve the UI at root
                var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var item in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{item.GroupName}/swagger.json",
                        "api-" + item.ApiVersion);
                }
            });
        }
    }
}
