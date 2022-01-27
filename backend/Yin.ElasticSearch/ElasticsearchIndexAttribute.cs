using System;

namespace Yin.ElasticSearch
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ElasticsearchIndexAttribute : Attribute
    {
        public string IndexName { get; }

        /// <summary>
        /// 
        /// </summary>
        public ElasticsearchIndexAttribute(string indexName)
        {
            IndexName = indexName ?? throw new ArgumentNullException(nameof(indexName));
        }
    }
}
