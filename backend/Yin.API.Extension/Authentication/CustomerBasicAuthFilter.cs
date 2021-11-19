using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Yin.API.Extension.Authentication
{
    public class CustomerBasicAuthFilter
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public CustomerBasicAuthFilter()
        {
            UserName = "admin";
            Password = "123456";
        }

        public CustomerBasicAuthFilter(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public bool Authorization(HttpContext context)
        {
            var authorization = context.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorization))
            {
                context.Response.Headers.Append("WWW-Authenticate", "BASIC realm=\"api\"");
                context.Response.StatusCode = 401;
                return false;
            }
            var authHeader = AuthenticationHeaderValue.Parse(authorization);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
            var userName = credentials[0];
            var password = credentials[1];

            if (userName == UserName && password == Password)
            {
                return true;
            }

            context.Response.StatusCode = 401;
            context.Response.Headers.Append("WWW-Authenticate", "BASIC realm=\"api\"");
            return false;
        }
    }
}
