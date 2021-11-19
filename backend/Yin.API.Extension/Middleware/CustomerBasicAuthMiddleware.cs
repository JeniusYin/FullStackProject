using Yin.API.Extension.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Yin.API.Extension.Middleware
{
    /// <summary>
    /// todo  使用 option 进行 配置
    /// </summary>
    public class CustomerBasicAuthMiddleware
    {
        private readonly RequestDelegate next;

        public CustomerBasicAuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestUrl = context.Request.Path.Value.ToLower();
            if (requestUrl.Contains("swagger") || requestUrl.Contains("api-docs"))
            {
                if (requestUrl.EndsWith(".css") || requestUrl.EndsWith(".js") || requestUrl.EndsWith(".png") || requestUrl.EndsWith(".map"))
                {
                    await next.Invoke(context);
                }
                else
                {
                    var filter = new CustomerBasicAuthFilter();
                    if (filter.Authorization(context))
                    {
                        await next.Invoke(context);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                await next.Invoke(context);
            }

        }
    }
}
