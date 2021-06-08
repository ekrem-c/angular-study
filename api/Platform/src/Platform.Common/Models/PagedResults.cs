using System;
using System.Collections.Generic;
using System.Linq;

namespace Platform.Common
{
    public class PagedResults<T>
        where T : class
    {
        public PagedResults(IEnumerable<T> results, Paging paging = null)
        {
            if (paging == null) paging = new Paging();

            Paging = new PageDetail(results.Count(), paging);

            Results = results.Skip(Offset).Take(Paging.PageSize);
        }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        ///     Gets or sets the paging.
        /// </summary>
        /// <value>The paging.</value>
        public PageDetail Paging { get; internal set; }

        internal int Offset => (Paging.Page - 1) * Paging.PageSize;

        /// <summary>
        ///     Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public IEnumerable<T> Results { get; internal set; }
    }
}