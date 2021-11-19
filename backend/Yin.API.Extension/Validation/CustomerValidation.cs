using Yin.API.Extension.ActionResult;
using Yin.Infrastructure.Model;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomerValidation
    {
        public static IMvcBuilder AddCustomerValidation(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder.AddFluentValidation()
                .ConfigureApiBehaviorOptions(options =>
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var res = new ResponseType<List<string>>()
                        {
                            Code = 400,
                            Msg = "模型参数绑定失败",
                            Data = new List<string>()
                        };
                        // https://docs.microsoft.com/zh-cn/aspnet/core/web-api/handle-errors?view=aspnetcore-3.1
                        foreach (var modelState in context.ModelState)
                        {
                            if (modelState.Value.ValidationState != ModelValidationState.Valid)
                                res.Data.Add($"{modelState.Key}:{string.Join(",", modelState.Value.Errors.Select(t => t.ErrorMessage) ?? new List<string>())}");
                        }
                        res.Msg = string.Join(",", res.Data);
                        var result = new InternalServerErrorObjectResult(res, (int)HttpStatusCode.BadRequest);
                        return result;
                    });
        }
    }
}
