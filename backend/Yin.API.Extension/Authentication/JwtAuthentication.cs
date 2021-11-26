using Yin.API.Extension.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JwtAuthentication
    {
        public static IServiceCollection AddCustomJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("JwtConfig");
            services.Configure<JwtConfigOption>(jwtConfig);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

            // 一、配置授权服务，也就是具体的规则，例：公司不同权限的门禁卡
            services.AddAuthorization(options =>
            {
                //基于策略的授权
                options.AddPolicy("Normal", policy => policy.RequireRole("Normal").Build());//单独角色
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("NormalOrAdmin", policy => policy.RequireRole("Admin", "Normal"));//或的关系
                options.AddPolicy("NormalAndAdmin", policy => policy.RequireRole("Admin").RequireRole("Normal"));//且的关系
                options.AddPolicy("LoginType", policy => policy.RequireClaim("LoginType").RequireRole("Normal")); 
            })
            // 二、配置认证服务，这里是jwt Bearer默认认证，例：光有卡没用，得能识别他们
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            // 三、针对JWT的配置，比如门禁是如何识别的，是IC卡还是磁卡
            .AddJwtBearer(options =>
            {    
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,//是否验证签名,不验证的画可以篡改数据，不安全
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig["Secret"])),//解密的密钥
                    ValidateIssuer = true,//是否验证发行人，就是验证载荷中的Issuer是否对应ValidIssuer参数
                    ValidIssuer = jwtConfig["Issuer"],//发行人
                    ValidateAudience = true,//是否验证订阅人，就是验证载荷中的Audience是否对应ValidAudience参数
                    ValidAudience = jwtConfig["Audience"],//订阅人
                    ValidateLifetime = true,//是否验证过期时间，过期了就拒绝访问
                    ClockSkew = TimeSpan.Zero,//过期时间+缓冲，可以直接设置为0
                    RequireExpirationTime = true,
                    LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken,
                       TokenValidationParameters validationParameters) =>
                    {
                        if (securityToken == null) return false;
                        var token = securityToken as JwtSecurityToken;
                        // 添加 loginType 限制
                        //if (token.Claims.Any(t => t.Type == "LoginType")) return false;
                        return notBefore <= DateTime.UtcNow && expires >= DateTime.UtcNow;
                    }
                };
            });
            return services;
        }
    }
}
