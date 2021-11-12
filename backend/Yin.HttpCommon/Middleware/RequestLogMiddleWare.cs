using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Yin.HttpCommon.Middleware
{
    public class RequestLogMiddleWare
    {
        private readonly RequestDelegate _next;
        public RequestLogMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ILogger<RequestLogMiddleWare> logger)
        {
            var request = httpContext.Request;
            var start = Stopwatch.GetTimestamp();
            var url = request.Path;
            var method = request.Method;
            await _next(httpContext);
            var stop = Stopwatch.GetTimestamp();
            var milliseconds = (stop - start) * 1000L / (double)Stopwatch.Frequency;
            var code = httpContext.Response.StatusCode;
            logger.LogInformation("HTTP {RequestMethod} {RequestPath} Header {Header} Responded {StatusCode} in {Elapsed} ms",
                    method, url, JsonConvert.SerializeObject(request.Headers), code, milliseconds);
        }
    }
}
