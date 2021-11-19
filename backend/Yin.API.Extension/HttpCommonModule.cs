using Yin.API.Extension.Filters;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Yin.API.Extension
{
    public static class HttpCommonModule
    {
        public static Action<MvcOptions> ExceptionMvcOption => t => t.Filters.Add(typeof(HttpGlobalExceptionFilter));

    }
}
