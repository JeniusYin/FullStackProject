using Microsoft.AspNetCore.Mvc;

namespace Yin.API.Extension.ActionResult
{
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object value, int statusCode) : base(value)
        {
            StatusCode = statusCode;
        }
    }
}
