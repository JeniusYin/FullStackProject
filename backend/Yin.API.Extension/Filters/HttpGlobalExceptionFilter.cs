using Yin.Infrastructure.Exceptions;
using Yin.API.Extension.ActionResult;
using Yin.Infrastructure.Model;
using Yin.Redis;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Yin.API.Extension.Filters
{
    /// <summary>
    /// 全局异常处理
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this._env = env;
            this._logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            bool isShowLog = true;
            if (context.Exception.GetType() == typeof(MyValidateException))
            {
                var errorMessage = ((context.Exception as MyValidateException)?.InnerException as ValidationException)?.Errors.Select(t => t.ErrorMessage) ?? new List<string>();
                var response = new ResponseType<string>()
                {
                    Code = 400,
                    Msg = string.Join(",", errorMessage),
                    Data = string.Empty
                };
                context.Result = new JsonResult(response);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else if (context.Exception.GetType() == typeof(MyException))
            {
                var ex = context.Exception as MyException;
                var response = new ResponseType<string>()
                {
                    Code = ex?.Code ?? 100,
                    Msg = context.Exception.Message,
                    Data = string.Empty
                };
                // pro 环境 不就行记录自定义log
                if (_env.IsProduction())
                    isShowLog = false;
                context.Result = new JsonResult(response);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else if (context.Exception.GetType() == typeof(RedLockException))
            {
                var response = new ResponseType<string>()
                {
                    Code = -2,
                    Msg = "请求过于频繁，请稍后再试",
                    Data = string.Empty
                };
                context.Result = new JsonResult(response);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                var response = new ResponseType<string>()
                {
                    Code = 500,
                    Msg = "服务端错误",
                    Data = $"{context.Exception.Message}---{context.Exception.StackTrace}"
                };
                var stateCode = _env.IsProduction() ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
                context.Result = new InternalServerErrorObjectResult(response, (int)stateCode);
            }
            if (isShowLog)
                _logger.LogError(new EventId(context.Exception.HResult),
                    context.Exception,
                    context.Exception.Message);
            context.ExceptionHandled = true;
        }
    }
}
