using System;
using System.Collections.Generic;
using System.Linq;

namespace Platform.Common
{
    /// <summary>
    ///     Extension methods for List
    ///     <typeparam name=">"></typeparam>
    ///     .
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        ///     Adds the for each methodology to any generic List to allow use in Linq queries.
        /// </summary>
        /// <remarks>Sourced from Linq in Action book by Fabrice Marguerie, Steve Eichert, and Jim Wooley (Manning Publishing).</remarks>
        /// <typeparam name="T">The type of the sequence items.</typeparam>
        /// <param name="source">The sequence to loop through.</param>
        /// <param name="func">The action to perform on the sequence items.</param>
        public static void ForEachItem<T>(this List<T> source, Action<T> func)
        {
            if (source == null) return;

            // Must iterate through a list rather than an IEnumerable in case we are replacing items in that list, hence the ".ToList()".
            foreach (var item in source.Where(s => s != null).ToList()) func(item);
        }

        /// <summary>
        ///     Adds the for each methodology to any generic List to allow use in Linq queries, only taking those items that are
        ///     not null.
        /// </summary>
        /// <remarks>Sourced from Linq in Action book by Fabrice Marguerie, Steve Eichert, and Jim Wooley (Manning Publishing).</remarks>
        /// <typeparam name="T">The type of the sequence items.</typeparam>
        /// <param name="source">The sequence to loop through.</param>
        /// <param name="predicate">The where clause used to further filter.</param>
        /// <param name="func">The action to perform on the sequence items.</param>
        public static void ForEachItem<T>(this List<T> source, Func<T, bool> predicate, Action<T> func)
        {
            if (source == null) return;

            source
                .Where(i => i != null && predicate(i))
                .ForEachItem(func);
        }
    }
}