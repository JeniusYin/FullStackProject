using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Yin.HttpCommon.Authentication
{
    public class ConfigureJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly IOptionsMonitor<JwtConfigOption> _jwtConfigOptionsMonitor;
        public ConfigureJwtBearerOptions(
            IOptionsMonitor<JwtConfigOption> jwtConfigOptionsMonitor)
        {
            _jwtConfigOptionsMonitor = jwtConfigOptionsMonitor;
        }

        public void PostConfigure(string name, JwtBearerOptions options)
        {
            options.RequireHttpsMetadata = false;
            var jwtSettings = _jwtConfigOptionsMonitor.CurrentValue;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.Key)),
                ValidateLifetime = true,
                //https://stackoverflow.com/questions/55341414/how-to-validate-jwt-using-utc-time-using-net-core
                LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken,
                    TokenValidationParameters validationParameters) =>
                {
                    return notBefore <= DateTime.UtcNow &&
                           expires >= DateTime.UtcNow;
                    /* 暂时  不做 token 检验
                    var token = securityToken as JwtSecurityToken;
                    var validate = _loginInfrastructure.TokenValidate(token, expires.Value);
                    return notBefore <= DateTime.UtcNow &&
                           expires >= DateTime.UtcNow && validate;
                           */
                }
            };
        }
    }
}
