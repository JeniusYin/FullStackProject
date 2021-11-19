namespace Yin.API.Extension.Redis
{
    public class RedisConf
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public int DefaultDatabase { get; set; }
    }
}
