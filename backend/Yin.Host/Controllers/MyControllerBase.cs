using Yin.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Yin.HttpApi
{
    [Controller]
    public class MyControllerBase : ControllerBase
    {
        [NonAction]
        internal static MyOKResult MyOk(object obj, string msg = null)
        {
            return new MyOKResult(obj, msg);
        }

        [DefaultStatusCode(200)]
        internal class MyOKResult : ObjectResult
        {
            public MyOKResult(object value, string msg = null) : base(value)
            {
                int code = 200;
                if (value is bool boolean)
                {
                    code = boolean ? 200 : 100;
                }
                Value = new ResponseType()
                {
                    Code = code,
                    Data = value ?? string.Empty,
                    Msg = msg ?? string.Empty
                };
                StatusCode = 200;
            }
        }
    }


}
