using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Yin.API.Extension.Authentication;
using Yin.Application.Contracts.AdminUser;
using Yin.Infrastructure.Model;

namespace Yin.HttpApi.Controllers.v1
{
    [ApiController, ApiVersion("1.0"), Route("api/v{version:apiVersion}/api/[controller]")]
    public class TestController : MyControllerBase
    {
        private readonly JwtConfigOption _jwtConfigOption;
        private readonly IAdminUserService _adminUserService;

        public TestController(
            IOptionsMonitor<JwtConfigOption> jwtConfigOptionsMonitor,
            IAdminUserService adminUserService)
        {
            _jwtConfigOption = jwtConfigOptionsMonitor.CurrentValue;
            _adminUserService = adminUserService;
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            return MyOk("Hello Test！");
        }

        [HttpGet]
        [Route("auth/admin")]
        [Authorize(Roles = nameof(UserRoleType.Admin))]
        public IActionResult AuthenAdminTest()
        {
            return MyOk("Hello Admin AuthenTest！");
        }

        [HttpGet]
        [Route("auth/normal")]
        [Authorize(Policy = "LoginType")]
        //[Authorize(Roles = nameof(UserRoleType.Normal))]
        public IActionResult AuthenNormalTest()
        {
            return MyOk("Hello Normal AuthenTest！");
        }

        [Route("token")]
        [HttpPost]
        public IActionResult Authenticate(string role)
        {
            //秘钥，就是标头，这里用Hmacsha256算法，需要256bit的密钥
            //Claim，JwtRegisteredClaimNames中预定义了好多种默认的参数名，也可以像下面的UserId一样自定义键名.
            //ClaimTypes也预定义了好多类型如role、email、name。Role用于赋予权限，不同的角色可以访问不同的接口
            var jwtSecurity = new JwtSecurityToken(
                header: new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfigOption.Secret)), SecurityAlgorithms.HmacSha256)),
                payload: new JwtPayload(
                   issuer: _jwtConfigOption.Issuer,
                   audience: _jwtConfigOption.Audience,
                   claims: new List<Claim>()
                   {
                        new Claim(ClaimTypes.Name, "test"),
                        new Claim(ClaimTypes.Role, role ?? string.Empty),
                        new Claim("UserId", Guid.NewGuid().ToString()),
                        new Claim("LoginType", "1")
                   }, notBefore: DateTime.Now, expires: DateTime.Now.AddMinutes(1)));

            //生成jwt令牌
            return Content(new JwtSecurityTokenHandler().WriteToken(jwtSecurity));
        }
    }
}
