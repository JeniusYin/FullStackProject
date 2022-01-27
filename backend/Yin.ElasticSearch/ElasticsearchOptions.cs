namespace Yin.ElasticSearch
{
    public class ElasticsearchOptions
    {
        /// <summary>
        /// Base address
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// User
        /// </summary>
        public string User { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
