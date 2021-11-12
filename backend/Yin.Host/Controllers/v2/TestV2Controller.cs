using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yin.HttpApi.Controllers.v2
{
    [ApiController, ApiVersion("2.0"), Route("api/v{version:apiVersion}/[controller]")]
    public class TestV2Controller : MyControllerBase
    {
        [Route("Test")]
        [HttpGet]
        public IActionResult Test()
        {
            return MyOk("欢迎使用Yin.Api通用项目模板~");
        }
    }
}
