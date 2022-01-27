using Nest;

namespace Yin.ElasticSearch
{
    public interface IElasticSearchService<TDoc> where TDoc : class
    {
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="numberOfReplicas"></param>
        /// <param name="numberOfShards"></param>
        /// <returns></returns>
        Task<CreateIndexResponse> IndexCreateAsync(int numberOfReplicas = 1, int numberOfShards = 5);
        /// <summary>
        /// 删除索引
        /// </summary>
        /// <returns></returns>
        Task<bool> IndexDeleteAsync();
        /// <summary>
        /// 判断索引是否存在
        /// </summary>
        /// <returns></returns>
        Task<ExistsResponse> IndexExistsAsync();

        /// <summary>
        /// 新增或更新文档
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task<bool> DocumentAddOrUpdateAsync(TDoc document);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<UpdateByQueryResponse> DocumentUpdateByQueryAsync(Func<QueryContainerDescriptor<TDoc>, QueryContainer> query);

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task<DeleteResponse> DocumentDeleteAsync(TDoc document);

        /// <summary>
        /// 对多个文档建立索引的更精细控制
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        Task<BulkResponse> DocumentsBulkAsync(IEnumerable<TDoc> documents);
        /// <summary>
        /// 对批量文档建立索引，适合大数据量使用
        /// </summary>
        /// <param name="documents"></param>
        void DocumentsBulkAll(IEnumerable<TDoc> documents);
        /// <summary>
        /// 文档批量删除
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<DeleteByQueryResponse> DocumentDeleteByQueryAsync(
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query);

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="tokenizer">分析器名称</param>
        /// <returns></returns>
        Task<AnalyzeResponse> Analyze(string text, string tokenizer = "ik_smart");

        /// <summary>
        /// 智能匹配
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sort"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<ISearchResponse<TDoc>> SearchAsync(
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query,
            Func<SortDescriptor<TDoc>, IPromise<IList<ISort>>> sort,
            int pageIndex = 1, int pageSize = 20);

        /// <summary>
        /// 高亮显示
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sort"></param>
        /// <param name="fieldHighlighters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<(IReadOnlyCollection<IHit<TDoc>> Hits, long TotalCount)> SearchWithHighlightAsync(
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query,
            Func<SortDescriptor<TDoc>, IPromise<IList<ISort>>> sort,
            Func<HighlightFieldDescriptor<TDoc>, IHighlightField>[] fieldHighlighters,
            int pageIndex = 1, int pageSize = 20);

        /// <summary>
        /// 智能匹配易社保服务
        /// </summary>
        /// <param name="takeCount"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<ISearchResponse<TDoc>> SearchProdAsync(int takeCount, Func<QueryContainerDescriptor<TDoc>, QueryContainer> query);

        /// <summary>
        /// 智能匹配易社保服务（高亮）
        /// </summary>
        /// <param name="takeCount"></param>
        /// <param name="query"></param>
        /// <param name="fieldHighlighters"></param>
        /// <returns></returns>
        Task<ISearchResponse<TDoc>> SearchProdWithHeighlightAsync(int takeCount, Func<QueryContainerDescriptor<TDoc>, QueryContainer> query, Func<HighlightFieldDescriptor<TDoc>, IHighlightField>[] fieldHighlighters);

        /// <summary>
        /// 智能提醒
        /// </summary>
        /// <param name="suggest"></param>
        /// <returns></returns>
        Task<ISuggestDictionary<TDoc>> Suggest(ISuggestContainer suggest);
    }
}
