using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Yin.ElasticSearch
{
    public class ElasticSearchService<TDoc> : IElasticSearchService<TDoc> where TDoc : class
    {
        /// <summary>
        /// ElasticClient
        /// </summary>
        private readonly IElasticClient _elasticClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="elasticClient"></param>
        public ElasticSearchService(IElasticClient elasticClient)
        {
            this._elasticClient = elasticClient;
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="numberOfReplicas"></param>
        /// <param name="numberOfShards"></param>
        /// <returns></returns>
        public async Task<CreateIndexResponse> IndexCreateAsync(int numberOfReplicas = 1
            , int numberOfShards = 2)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));

            if (_elasticClient.Indices.Exists(indexName.ToLower()).Exists) return new CreateIndexResponse { Index = indexName.ToLower() };

            var analyzers = new Analyzers
            {
                { "pinyin_analyzer", new CustomAnalyzer() { Tokenizer = "pinyin" } }
            };
            //var tokenizers = new Tokenizers
            //{
            //    { "my_pinyin", }
            //};
            IIndexState indexState = new IndexState
            {

                Settings = new IndexSettings
                {
                    NumberOfReplicas = numberOfReplicas, // 副本数量
                    NumberOfShards = numberOfShards, //分片数量
                    Analysis = new Analysis()
                    {
                        Analyzers = analyzers,
                        //Tokenizers = tokenizers
                    }
                },
                Mappings = new TypeMapping
                {

                }
            };
            ICreateIndexRequest func(CreateIndexDescriptor x) => x.InitializeUsing(indexState).Map(m => m.AutoMap());
            return await _elasticClient.Indices.CreateAsync(indexName, func);
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        public async Task<bool> IndexDeleteAsync()
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));

            var response = _elasticClient.Indices.Exists(indexName.ToLower());
            if (!response.Exists)
            {
                return true;
            };
            var res = await _elasticClient.Indices.DeleteAsync(indexName);
            return res.Acknowledged;
        }

        /// <summary>
        /// 判断索引是否存在
        /// </summary>
        /// <returns></returns>
        public async Task<ExistsResponse> IndexExistsAsync()
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));

            return await _elasticClient.Indices.ExistsAsync(indexName);
        }

        public async Task<bool> DocumentAddOrUpdateAsync(TDoc document)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
            if (document == null) throw new ArgumentNullException(nameof(document));

            var exits = _elasticClient.DocumentExists(DocumentPath<TDoc>.Id(new Id(document)), dd => dd.Index(indexName));

            if (exits.Exists)
            {
                var result = await _elasticClient.UpdateAsync(DocumentPath<TDoc>.Id(new Id(document)),
                    ss => ss.Index(indexName).Doc(document).RetryOnConflict(3));

                if (result.ServerError != null) throw new ResponseException($"Update Document failed at index{indexName} :" +
                                                 result.ServerError.Error.Reason, result);
            }
            else
            {
                var result = await _elasticClient.IndexAsync<TDoc>(document, iDesc => iDesc.Index(indexName));
                if (result.ServerError != null) throw new ResponseException($"Insert Docuemnt failed at index {indexName} :" +
                                                 result.ServerError.Error.Reason, result);
            }

            return true;
        }

        public async Task<UpdateByQueryResponse> DocumentUpdateByQueryAsync(Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
            if (query == null) throw new ArgumentNullException(nameof(query));

            var updateByQueryResponse = await _elasticClient.UpdateByQueryAsync<TDoc>(selector => selector.Index(Indices.Index(indexName)).Query(query));
            if (!updateByQueryResponse.IsValid)
                throw new ResponseException("更新文档异常！", updateByQueryResponse);

            return updateByQueryResponse;
        }

        public async Task<DeleteResponse> DocumentDeleteAsync(TDoc document)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
            if (document == null) throw new ArgumentNullException(nameof(document));

            var deleteResponse = await _elasticClient.DeleteAsync(new DeleteRequest(indexName, new Id(document)));
            if (!deleteResponse.IsValid)
                throw new ResponseException("要删除的文档不存在！", deleteResponse);

            return deleteResponse;
        }

        public async Task<BulkResponse> DocumentsBulkAsync(IEnumerable<TDoc> documents)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
            if (documents == null) throw new ArgumentNullException(nameof(documents));

            var bulk = new BulkRequest(indexName)
            {
                Operations = new List<IBulkOperation>()
            };
            foreach (var item in documents)
            {
                bulk.Operations.Add(new BulkIndexOperation<TDoc>(item));
            }

            return await _elasticClient.BulkAsync(bulk);
        }

        public void DocumentsBulkAll(IEnumerable<TDoc> documents)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
            if (documents == null) throw new ArgumentNullException(nameof(documents));

            _elasticClient.BulkAll(documents, b => b
                               .Index(indexName)
                //.BackOffTime(new Time(TimeSpan.FromSeconds(30)))//重试之前等待的时间
                //.BackOffRetries(3) //重试3次
                .RefreshOnCompleted()
                //.MaxDegreeOfParallelism(Environment.ProcessorCount)
                .Size(200))
                .Wait(TimeSpan.FromMinutes(2), next =>
                {
                    // doing something
                });
        }

        public async Task<DeleteByQueryResponse> DocumentDeleteByQueryAsync(
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
            if (query == null) throw new ArgumentNullException(nameof(query));

            var deleteByQueryResponse = await _elasticClient.DeleteByQueryAsync<TDoc>(selector => selector.Index(Indices.Index(indexName)).Query(query));
            if (!deleteByQueryResponse.IsValid)
                throw new ResponseException("要删除的文档不存在！", deleteByQueryResponse);

            return deleteByQueryResponse;
        }

        public async Task<ISearchResponse<TDoc>> SearchAsync(
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query,
            Func<SortDescriptor<TDoc>, IPromise<IList<ISort>>> sort,
            int pageIndex = 1, int pageSize = 20)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
            if (query == null) throw new ArgumentNullException(nameof(query));

            var sr = await _elasticClient.SearchAsync<TDoc>(sd => sd
                .Index(indexName)
                .From((pageIndex - 1) * pageSize)
                .Size(pageSize)
                .Query(query)
                .Sort(sort));

            if (!sr.IsValid)
                throw new EsSearchException<TDoc>(sr);

            return sr;
        }

        public async Task<(IReadOnlyCollection<IHit<TDoc>> Hits, long TotalCount)> SearchWithHighlightAsync(
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query,
            Func<SortDescriptor<TDoc>, IPromise<IList<ISort>>> sort,
            Func<HighlightFieldDescriptor<TDoc>, IHighlightField>[] fieldHighlighters,
            int pageIndex = 1, int pageSize = 20)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
            if (query == null) throw new ArgumentNullException(nameof(query));

            IHighlight highlight(HighlightDescriptor<TDoc> h) => h.PreTags("<strong style='color: red;'>").PostTags("</strong>")
                    .Encoder(HighlighterEncoder.Html)
                    .Fields(fieldHighlighters);

            var sr = await _elasticClient.SearchAsync<TDoc>(sd => sd
                .Index(indexName)
                .From((pageIndex - 1) * pageSize)
                .Size(pageSize)
                .Query(query)
                .Highlight(highlight)
                .Sort(sort));

            if (!sr.IsValid)
                throw new EsSearchException<TDoc>(sr);
            return (sr.Hits, sr.Total);
        }

        public async Task<ISuggestDictionary<TDoc>> Suggest(ISuggestContainer suggest)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            SearchRequest searchRequest = new SearchRequest<TDoc>(indexName)
            {
                Suggest = suggest
            };

            var result = await _elasticClient.SearchAsync<TDoc>(searchRequest);
            return result.Suggest;
        }

        //ik_smart
        //ik_max_word
        public async Task<AnalyzeResponse> Analyze(string text, string tokenizer = "ik_smart")
        {
            return await _elasticClient.Indices.AnalyzeAsync(a => a.Analyzer(tokenizer).Text(text));
        }

        public async Task<ISearchResponse<TDoc>> SearchProdAsync(
            int takeCount,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
            if (query == null) throw new ArgumentNullException(nameof(query));

            var sr = await _elasticClient.SearchAsync<TDoc>(sd => sd
                .Index(indexName)
                .Size(takeCount)
                .Query(query));

            if (!sr.IsValid)
                throw new EsSearchException<TDoc>(sr);

            return sr;
        }

        public async Task<ISearchResponse<TDoc>> SearchProdWithHeighlightAsync(
            int takeCount,
            Func<QueryContainerDescriptor<TDoc>, QueryContainer> query,
            Func<HighlightFieldDescriptor<TDoc>, IHighlightField>[] fieldHighlighters)
        {
            string? indexName = typeof(TDoc).GetCustomAttribute<ElasticsearchIndexAttribute>()?.IndexName;
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
            if (query == null) throw new ArgumentNullException(nameof(query));

            IHighlight highlight(HighlightDescriptor<TDoc> h) => h.PreTags("<strong style='color: red;'>").PostTags("</strong>")
            .Encoder(HighlighterEncoder.Html)
            .Fields(fieldHighlighters);

            var sr = await _elasticClient.SearchAsync<TDoc>(sd => sd
                .Index(indexName)
                .Size(takeCount)
                .Query(query)
                .Highlight(highlight));

            if (!sr.IsValid)
                throw new EsSearchException<TDoc>(sr);

            return sr;
        }
    }
}
