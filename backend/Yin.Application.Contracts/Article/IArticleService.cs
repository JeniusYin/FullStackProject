using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yin.Application.Contracts.Article
{
    public interface IArticleService<T> where T : class
    {
        /// <summary>
        /// 创建或更新一篇Article
        /// </summary>
        /// <param name="article">文章</param>
        /// <returns></returns>
        Task<bool> CreateOrUpdateArticle(Article article);

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteArticleIndex();

        /// <summary>
        /// 精准查询单篇文章
        /// </summary>
        /// <param name="articleGuid"></param>
        /// <returns></returns>
        Task<Article> GetArticleAsync(string articleGuid);

        /// <summary>
        /// 按关键字搜索 (标题 内容)高亮关键字
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<(IReadOnlyCollection<IHit<T>> Hits, long TotalCount)> SearchContentsWithHighlight(string keyword);
    }
}
