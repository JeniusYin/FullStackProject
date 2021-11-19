using Yin.Infrastructure.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Yin.API.Extension.Middleware
{

    public static class CustomerResponse
    {
        public static void UseCustomerResponse(this IApplicationBuilder app)
        {
            app.UseMiddleware<ResponseHandlingMiddleware>();
        }
    }

    public class ResponseHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ResponseHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                context.Response.OnStarting(async () =>
                {
                    var response = new ResponseType<string>
                    {
                        Code = context.Response.StatusCode
                    };
                    if (response.Code == 401)
                    {
                        response.Msg = "Unauthorized";
                    }
                    else if (response.Code == 403)
                    {
                        response.Msg = "Forbidden";
                    }
                    else if (response.Code == 500)
                    {
                        response.Msg = "服务端错误";
                    }
                    context.Response.StatusCode = 200;
                    if (response.Code != 200 && response.Code != 400)
                    {
                        await HandleExceptionAsync(context, response);
                    }
                });
            }
            await next(context);
        }
        //异常错误信息捕获，将错误信息用Json方式返回
        private static Task HandleExceptionAsync(HttpContext context, ResponseType<string> response)
        {
            // 序列化小驼峰
            var result = JsonConvert.SerializeObject(response, new JsonSerializerSettings()
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            context.Response.ContentType = "application/json;charset=utf-8";
            return context.Response.WriteAsync(result);
        }
    }
}
