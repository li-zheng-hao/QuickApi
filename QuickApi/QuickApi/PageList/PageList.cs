using System;
using System.Collections.Generic;

namespace QuickApi
{
    [Serializable]
    public class PageList<T> : List<T>
    {
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
            AddRange(source);
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
        public int TotalCount { get; }

        /// <summary>
        ///     总页数
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        ///     是否有上一页
        /// </summary>
        public bool HasPreviousPage => PageIndex > 0;

        /// <summary>
        ///     是否有下一页
        /// </summary>
        public bool HasNextPage => PageIndex + 1 < TotalPages;

        // public PageList<T1> Adapt<T1>()
        // {
        //     List<T1> res = new List<T1>();
        //     foreach (var item in this)
        //     {
        //         res.Add(item.Adapt<T1>());
        //     }
        //     return new PageList<T1>(res,this.PageIndex,this.PageSize,this.TotalCount);
        // }
    }
}