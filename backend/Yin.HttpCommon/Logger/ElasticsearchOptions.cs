namespace Yin.HttpCommon.Logger
{
    public class ElasticsearchOptions
    {
        /// <summary>
        /// 客户端地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 默认索引
        /// </summary>
        public string DefaultIndex { get; set; }
    }
}
