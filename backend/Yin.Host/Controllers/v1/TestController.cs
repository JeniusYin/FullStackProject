using Microsoft.AspNetCore.Mvc;

namespace Yin.HttpApi.Controllers.v1
{
    [ApiController, ApiVersion("1.0"), Route("api/v{version:apiVersion}/[controller]")]
    public class TestController : MyControllerBase
    {
        [Route("Test")]
        [HttpGet]
        public IActionResult Test()
        {
            return MyOk("欢迎使用Yin.Api通用项目模板~");
        }
    }
}
