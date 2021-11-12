using System;

namespace Yin.Infrastructure.Model
{
    public class QueryParameters
    {
        private const int DefaultPageSize = 1;
        private const int DefaultMaxPageSize = 50;

        private int _pageIndex;
        /// <summary>
        /// 当前页面
        /// </summary>
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value >= 1 ? value : 1;
        }
        private int _pageSize = DefaultPageSize;
        /// <summary>
        /// 每页数据
        /// </summary>
        public virtual int PageSize
        {
            get => _pageSize;
            set => SetPageSize(value);
        }
        private void SetPageSize(int value)
        {
            if (value <= 0)
            {
                value = DefaultPageSize;
            }
            if (value > DefaultMaxPageSize)
            {
                value = DefaultMaxPageSize;
            }
            _pageSize = value;
        }

        private int _totalCount;

        public int GetTotalCount() => _totalCount;
        /// <summary>
        /// 检查request 
        /// </summary>
        /// <param name="totalCount"></param>
        public void CheckRequest(int totalCount)
        {
            PageSize = PageSize < 1 ? 1 : PageSize > DefaultMaxPageSize ? DefaultMaxPageSize : PageSize;
            var totalPage = (int)Math.Ceiling(totalCount * 1.0 / PageSize);
            PageIndex = PageIndex < 1 ? 1 : PageIndex > totalPage ? totalPage : PageIndex;
            _totalCount = totalCount;
        }
    }
}
