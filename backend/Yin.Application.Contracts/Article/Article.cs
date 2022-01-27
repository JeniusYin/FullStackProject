using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yin.ElasticSearch;

namespace Yin.Application.Contracts.Article
{
    [ElasticsearchIndex("esb_article_index")]
    [ElasticsearchType(IdProperty = nameof(ArticleGuid))]
    public class Article
    {
        public long ArticleId { get; set; }

        public Guid ArticleGuid { get; set; }

        /// <summary>
        /// 文章类别名
        /// </summary>
        public int ArticleType { get; set; }

        /// <summary>
        /// 文章类别名
        /// </summary>
        [Text(Index = false)]
        public string ArticleCategoryName { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        [Text(Index = true, Analyzer = "ik_smart", SearchAnalyzer = "ik_smart", Boost = 100)]
        public string Title { get; set; }

        /// <summary>
        /// 文章作者
        /// </summary>
        [Text(Index = false)]
        public string Author { get; set; }

        /// <summary>
        /// 文章摘要
        /// </summary>
        [Text(Index = true, Analyzer = "ik_smart", SearchAnalyzer = "ik_smart", Boost = 90)]
        public string Abstract { get; set; }

        /// <summary>
        /// 文章关键字
        /// </summary>
        [Text(Index = true, Analyzer = "ik_smart", SearchAnalyzer = "ik_smart", Boost = 90)]
        public string KeyWords { get; set; }

        /// <summary>
        /// 文章描述
        /// </summary>
        [Text(Index = true, Analyzer = "ik_smart", SearchAnalyzer = "ik_smart", Boost = 80)]
        public string Description { get; set; }

        /// <summary>
        /// 文章正文
        /// </summary>
        [Text(Index = true, Analyzer = "ik_smart", SearchAnalyzer = "ik_smart", Boost = 70)]
        public string Contents { get; set; }
        /// <summary>
        /// 文章首图
        /// </summary>
        public string ConvertImageURL { get; set; }
        /// <summary>
        /// 浏览量
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDelete { get; set; }

        /// <summary>
        /// 所属城市
        /// </summary>
        [Text(Index = false)]
        public string CityName { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public int IsTop { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public int IsShow { get; set; }

        /// <summary>
        /// 是否热门
        /// </summary>
        public int IsHot { get; set; }

    }
}
