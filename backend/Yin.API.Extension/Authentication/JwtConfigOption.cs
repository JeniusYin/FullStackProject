namespace Yin.API.Extension.Authentication
{
    /// <summary>
    /// jwt 配置类
    /// </summary>
    public class JwtConfigOption
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
}
