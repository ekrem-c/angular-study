using System;
using System.Collections.Generic;
using System.Linq;

namespace Platform.Common
{
    public static class PagingExtensions
    {
        /// <summary>
        ///     Paging the specified results, paging and id.
        /// </summary>
        /// <returns>The paging.</returns>
        /// <param name="results">Results.</param>
        /// <param name="paging">Paging.</param>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static PagedResults<T> Paging<T>(this IEnumerable<T> results, Paging paging = null, Guid? id = null)
            where T : class
        {
            var pagedResults = new PagedResults<T>(results, paging);

            if (id.HasValue)
            {
                pagedResults.Id = id.Value;
            }

            return pagedResults;
        }

        /// <summary>
        ///     Paging the specified results, converter, paging and id.
        /// </summary>
        /// <returns>The paging.</returns>
        /// <param name="results">Results.</param>
        /// <param name="converter">Converter.</param>
        /// <param name="paging">Paging.</param>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static PagedResults<T2> Paging<T1, T2>(
            this IEnumerable<T1> results,
            Func<T1, T2> converter,
            Paging paging = null, Guid? id = null)
                where T1 : class
                where T2 : class
        {
            return Paging(results.Select(converter), paging, id);
        }
    }
}