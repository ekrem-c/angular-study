using System;

namespace Platform.Common
{
    public class PageDetail : Paging
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Platform.Common.Models.PagingDetail" /> class.
        /// </summary>
        /// <param name="totalResults">Total results.</param>
        /// <param name="paging">Paging.</param>
        public PageDetail(int totalResults, Paging paging) : this(totalResults, paging.Page, paging.PageSize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Platform.Common.Models.PagingDetail" /> class.
        /// </summary>
        /// <param name="totalResults">Total results.</param>
        /// <param name="page">Page.</param>
        /// <param name="pageSize">Page size.</param>
        public PageDetail(int totalResults, int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
            TotalResults = totalResults;

            if (Page < 1) Page = 1;

            if (Page > TotalPages) Page = TotalPages;
        }

        /// <summary>
        ///     Gets the total results.
        /// </summary>
        /// <value>The total results.</value>
        public int TotalResults { get; }

        /// <summary>
        ///     Gets the total page.
        /// </summary>
        /// <value>The total page.</value>
        public int TotalPages => (int) Math.Round((double) TotalResults / PageSize, 0, MidpointRounding.AwayFromZero);
    }
}