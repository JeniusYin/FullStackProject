using Microsoft.AspNetCore.Mvc;

namespace Yin.HttpCommon.ActionResult
{
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object value, int statusCode) : base(value)
        {
            StatusCode = statusCode;
        }
    }
}
