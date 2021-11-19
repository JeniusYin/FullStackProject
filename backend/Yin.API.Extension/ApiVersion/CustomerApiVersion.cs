using Yin.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using System;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class CustomerApiVersion
    {
        public static IServiceCollection AddCustomerApiVersion(this IServiceCollection services)
        {
            // https://github.com/Microsoft/aspnet-api-versioning/wiki
            // 版本控制
            services.AddApiVersioning(option =>
            {
                // 可选，为true时API返回支持的版本信息
                option.ReportApiVersions = true;
                // 不提供版本时，默认为1.0
                option.AssumeDefaultVersionWhenUnspecified = true;
                // 请求中未指定版本时默认为1.0
                option.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                option.ApiVersionReader = new UrlSegmentApiVersionReader();
                option.ErrorResponses = new MyErrorResponseProvider();
            }).AddVersionedApiExplorer(option =>
                {
                    // 版本名的格式：v+版本号  "'v'major[.minor]"
                    option.GroupNameFormat = "'v'VVV";
                    option.AssumeDefaultVersionWhenUnspecified = true;
                    option.SubstituteApiVersionInUrl = true;
                });
            return services;
        }
        /// <summary>
        /// https://github.com/Microsoft/aspnet-api-versioning/wiki/Error-Response-Provider
        /// API版本 自定义错误
        /// </summary>
        public class MyErrorResponseProvider : IErrorResponseProvider
        {
            public IActionResult CreateResponse(ErrorResponseContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return new ObjectResult(CreateErrorContent(context)) { StatusCode = 200 };
            }
            protected virtual object CreateErrorContent(ErrorResponseContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                return new ResponseType<string>()
                {
                    Code = 405,
                    Msg = "当前API版本不支持"
                };
            }
        }
    }
}
