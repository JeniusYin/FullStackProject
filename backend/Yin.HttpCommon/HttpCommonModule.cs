using Yin.HttpCommon.Filters;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Yin.HttpCommon
{
    public static class HttpCommonModule
    {
        public static Action<MvcOptions> ExceptionMvcOption => t => t.Filters.Add(typeof(HttpGlobalExceptionFilter));

    }
}
