using System.Collections.Generic;
using System.Linq;

namespace Yin.Infrastructure.Model
{
    /// <summary>
    /// 分页model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedList<T>
    {
        public PaginatedList()
        {

        }
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> Page { get; set; }
        /// <summary>
        /// 每页数据
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }
        private int _totalItemsCount;
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalItemsCount
        {
            get => _totalItemsCount;
            set => _totalItemsCount = value >= 0 ? value : 0;
        }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount => TotalItemsCount == 0 ? 0 : TotalItemsCount / PageSize + (TotalItemsCount % PageSize > 0 ? 1 : 0);
        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPrevious => PageIndex > 1;
        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNext => PageIndex <= PageCount - 1;

        public PaginatedList(int pageIndex, int pageSize, int totalItemsCount, IEnumerable<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalItemsCount = totalItemsCount;
            Page = data?.ToList() ?? new List<T>(data);
        }

        public PaginatedList(QueryParameters parameters, IEnumerable<T> data)
        {
            PageIndex = parameters.PageIndex;
            PageSize = parameters.PageSize;
            TotalItemsCount = parameters.GetTotalCount();
            Page = data?.ToList() ?? new List<T>(data);
        }
    }
}
