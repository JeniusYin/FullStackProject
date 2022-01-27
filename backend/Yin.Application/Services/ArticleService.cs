using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yin.Application.Contracts.Article;
using Yin.ElasticSearch;

namespace Yin.Application.Services
{
    public class ArticleService<T> : IArticleService<T> where T : Article
    {
        private readonly IElasticSearchService<Article> _elasticSearchService;


        public ArticleService(IElasticSearchService<Article> elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
        }

        public async Task<bool> CreateOrUpdateArticle(Article article)
        {
            if (article == null) return false;
            return await _elasticSearchService.DocumentAddOrUpdateAsync(article);
        }

        public async Task<bool> DeleteArticleIndex()
        {
            return await _elasticSearchService.IndexDeleteAsync();
        }

        public async Task<Article> GetArticleAsync(string articleGuid)
        {
            QueryContainer query(QueryContainerDescriptor<Article> q) => q.Match(mq => mq.Field(f => f.ArticleGuid).Query(articleGuid));
            var res = await _elasticSearchService.SearchAsync(
                query,
                ElasticSearchUtil.SortDesc<Article>(t => t.ViewCount),
                pageSize: 1
              );

            return res.Documents.FirstOrDefault();
        }

        public async Task<(IReadOnlyCollection<IHit<T>> Hits, long TotalCount)> SearchContentsWithHighlight(string keyword)
        {
            //多条件搜索
            var musts = ElasticSearchUtil.Queries<Article>();
            musts.AddTerm(f => f.IsShow, 1);
            musts.AddTerm(f => f.IsDelete, 0);
            musts.AddDateRange(f => f.PublishTime, DateTime.Now.AddYears(-5), DateTime.Now);

            musts.AddMultiMatch(new string[] { "title", "contents^5" }, keyword);
            //var filters = ElasticSearchUtil.Queries<Article>();
            //filters.Add(q => q.MatchPhrase(a => a.Field(f => f.Title).Slop(10).Query(request.Keyword)));
            //filters.Add(q => q.MatchPhrase(a => a.Field(f => f.Contents).Slop(10).Query(request.Keyword)));

            QueryContainer query(QueryContainerDescriptor<Article> q) => q.Bool(b => b
                            .Must(musts));

            IHighlightField highlightTitle(HighlightFieldDescriptor<Article> highlight) => highlight.Field(f => f.Title).Type(HighlighterType.Unified);
            IHighlightField highlightContents(HighlightFieldDescriptor<Article> highlight) => highlight.Field(f => f.Contents).Type(HighlighterType.Unified);

            var res = await _elasticSearchService.SearchWithHighlightAsync(
                      query,
                      ElasticSearchUtil.SortDesc<Article>(t => t.ViewCount),
                      new Func<HighlightFieldDescriptor<Article>, IHighlightField>[] { highlightTitle, highlightContents },
                      1,
                      20
                );
            return ((IReadOnlyCollection<IHit<T>>)res.Hits, res.TotalCount);
        }
    }
}
