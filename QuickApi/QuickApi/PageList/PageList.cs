using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuickApi
{
    [Serializable]
    public class PageList<T>
    {
        public List<T> Items { get; set; } = new();
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="totalCount">总记录数</param>
        public PageList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            TotalCount = totalCount;
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            PageSize = pageSize;
            PageIndex = pageIndex;
            Items.AddRange(source);
        }

        /// <summary>
        ///     分页索引
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        ///     分页大小
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        ///     总记录数
        /// </summary>
        [JsonInclude]
        public int TotalCount { get; }

        /// <summary>
        ///     总页数
        /// </summary>
        [JsonInclude]
        public int TotalPages { get; }

        /// <summary>
        ///     是否有上一页
        /// </summary>
        public bool HasPreviousPage => PageIndex > 0;

        /// <summary>
        ///     是否有下一页
        /// </summary>
        public bool HasNextPage => PageIndex + 1 < TotalPages;

    }
}