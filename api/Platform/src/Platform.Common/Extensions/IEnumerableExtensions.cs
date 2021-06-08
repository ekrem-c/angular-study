using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Platform.Common
{
    /// <summary>
    ///     Extension methods for IEnumerables.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        ///     Indicates whether an IEnumerable is null or has no items including a string.
        /// </summary>
        /// <param name="o">The extension method source.</param>
        /// <returns>True if it is either null or empty, otherwise false.</returns>
        public static bool IsNullOrEmpty(this IEnumerable o)
        {
            return (o != null && o.GetEnumerator().MoveNext()) == false;
        }

        /// <summary>
        ///     Adds the for each methodology to any generic enumerable to allow use in Linq queries.
        /// </summary>
        /// <remarks>Sourced from Linq in Action book by Fabrice Marguerie, Steve Eichert, and Jim Wooley (Manning Publishing).</remarks>
        /// <typeparam name="T">The type of the sequence items.</typeparam>
        /// <param name="source">The sequence to loop through.</param>
        /// <param name="func">The action to perform on the sequence items.</param>
        public static void ForEachItem<T>(this IEnumerable<T> source, Action<T> func)
        {
            if (source == null) return;

            foreach (var item in source.Where(s => s != null)) func(item);
        }

        /// <summary>
        ///     Adds the for each methodology to any generic enumerable to allow use in Linq queries, only taking those items that
        ///     are not null.
        /// </summary>
        /// <remarks>Sourced from Linq in Action book by Fabrice Marguerie, Steve Eichert, and Jim Wooley (Manning Publishing).</remarks>
        /// <typeparam name="T">The type of the sequence items.</typeparam>
        /// <param name="source">The sequence to loop through.</param>
        /// <param name="predicate">The where clause used to further filter.</param>
        /// <param name="func">The action to perform on the sequence items.</param>
        public static void ForEachItem<T>(this IEnumerable<T> source, Func<T, bool> predicate, Action<T> func)
        {
            if (source == null) return;

            source
                .Where(i => i != null && predicate(i))
                .ForEachItem(func);
        }

        /// <summary>
        ///     Indicates whether a generic IEnumerable is not null and contains at least one non-null item.
        /// </summary>
        /// <typeparam name="T">The generic type of IEnumerable.</typeparam>
        /// <param name="source">The extension method source.</param>
        /// <returns>True if is not null and contains at least one non-null item, otherwise false.</returns>
        public static bool AnyItems<T>(this IEnumerable<T> source)
        {
            return source.IsNullOrEmpty() == false && source.Any(i => i != null);
        }

        /// <summary>
        ///     Indicates whether a generic IEnumerable is not null and contains at least one non-null item.
        /// </summary>
        /// <typeparam name="T">The generic type of IEnumerable.</typeparam>
        /// <param name="source">The extension method source.</param>
        /// <param name="predicate">The where clause used to further filter.</param>
        /// <returns>True if is not null and contains at least one non-null item, otherwise false.</returns>
        public static bool AnyItems<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.IsNullOrEmpty() == false && source.Any(i => i != null && predicate(i));
        }

        /// <summary>
        ///     Indicates whether a generic IEnumerable is either null or contains only non-null item.
        /// </summary>
        /// <typeparam name="T">The generic type of IEnumerable.</typeparam>
        /// <param name="source">The extension method source.</param>
        /// <returns>True if is not null and contains at least one non-null item, otherwise false.</returns>
        public static bool NoItems<T>(this IEnumerable<T> source)
        {
            return source.AnyItems() == false;
        }

        /// <summary>
        ///     Returns those items in the IEnumerable that are not null.
        /// </summary>
        /// <typeparam name="T">The generic type of IEnumerable.</typeparam>
        /// <param name="source">The extension method source.</param>
        /// <returns>An IEnumerable of non-null items.</returns>
        public static IEnumerable<T> Items<T>(this IEnumerable<T> source)
        {
            if (source.IsNullOrEmpty()) return Enumerable.Empty<T>();

            return source.Where(i => i != null);
        }

        /// <summary>
        ///     Returns those items in the IEnumerable that are not null.
        /// </summary>
        /// <typeparam name="T">The generic type of IEnumerable.</typeparam>
        /// <param name="source">The extension method source.</param>
        /// <param name="predicate">The where clause used to further filter.</param>
        /// <returns>An IEnumerable of non-null items.</returns>
        public static IEnumerable<T> Items<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source.IsNullOrEmpty()) return Enumerable.Empty<T>();

            return source.Where(i => i != null && predicate(i));
        }

        /// <summary>
        ///     Indicates whether a generic IEnumerable is not null and contains at least one non-null item.
        /// </summary>
        /// <typeparam name="T">The generic type of IEnumerable.</typeparam>
        /// <param name="source">The extension method source.</param>
        /// <param name="predicate">The where clause used to further filter.</param>
        /// <returns>True if is not null and contains at least one non-null item, otherwise false.</returns>
        public static bool NoItems<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.AnyItems(predicate) == false;
        }

        /// <summary>
        ///     Adds the items in the specified dictionary to the current dictionary.
        /// </summary>
        /// <typeparam name="T1">Key type for the dictionaries.</typeparam>
        /// <typeparam name="T2">Value type for the dictionaries.</typeparam>
        /// <param name="target">The dictionary to which the items are to be added.</param>
        /// <param name="source">The dictionary containing items to add.</param>
        public static void AddRange<T1, T2>(this Dictionary<T1, T2> target, Dictionary<T1, T2> source)
        {
            source.ForEachItem(i => target.ContainsKey(i.Key) == false, i => target.Add(i.Key, i.Value));
        }

        /// <summary>
        ///     Adds the items in the specified collection to the current list.
        /// </summary>
        /// <typeparam name="TKey">The Type to represent the key.</typeparam>
        /// <typeparam name="TValue">The Type to represent the value.</typeparam>
        /// <param name="target">
        ///     The list to which the items are to be added.
        /// </param>
        /// <param name="source">
        ///     The collection containing items to add.
        /// </param>
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> target,
            IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            source.ForEachItem(i => target.ContainsKey(i.Key) == false, i => target.Add(i.Key, i.Value));
        }

        /// <summary>
        ///     Convert all string in the list to lower case
        /// </summary>
        /// <param name="list">The list containing string to be converted</param>
        /// <returns>The list that has been converted</returns>
        public static IEnumerable<string> ToLower(this IEnumerable<string> list)
        {
            if (list.AnyItems()) return list.Select(l => l.ToLower());

            return Enumerable.Empty<string>();
        }

        /// <summary>
        ///     Convert all string in the list to upper case
        /// </summary>
        /// <param name="list">The list containing string to be converted</param>
        /// <returns>The list that has been converted</returns>
        public static IEnumerable<string> ToUpper(this IEnumerable<string> list)
        {
            if (list.AnyItems()) return list.Select(l => l.ToUpper());

            return Enumerable.Empty<string>();
        }

        /// <summary>
        ///     Performs a string join operation returning a correctly punctuated list.
        /// </summary>
        /// <param name="list">The sequence of strings to join together.</param>
        /// <returns>A string that is correctly punctuated.</returns>
        /// <remarks>English language specific.</remarks>
        public static string JoinAsPunctuatedList(this IEnumerable<string> list)
        {
            var response = string.Empty;

            if (list.IsNullOrEmpty()) return response;

            response = JoinAsPunctuatedListWithoutPeriod(list);

            return response + ".";
        }

        /// <summary>
        ///     Performs a string join operation returning a correctly punctuated list without period.
        /// </summary>
        /// <param name="list">The sequence of strings to join together.</param>
        /// <returns>A string that is correctly punctuated.</returns>
        /// <remarks>English language specific.</remarks>
        public static string JoinAsPunctuatedListWithoutPeriod(this IEnumerable<string> list)
        {
            var response = string.Empty;

            if (list.IsNullOrEmpty()) return response;

            const string Separator = ", ";

            var data = list.JoinViaCommaSpace();
            var lastSeparatorPosn = data.LastIndexOf(Separator);
            response = lastSeparatorPosn != -1
                ? data.Remove(lastSeparatorPosn, Separator.Length).Insert(lastSeparatorPosn, " and ")
                : data;

            return response.Trim();
        }

        /// <summary>
        ///     Performs a string join operations with a comma as the separator.
        /// </summary>
        /// <param name="source">The sequence to join together.</param>
        /// <returns>Returns a string of the source joined by a comma.</returns>
        public static string JoinViaComma(this IEnumerable<string> source)
        {
            return source.JoinViaSeparator(",");
        }

        /// <summary>
        ///     Performs a string join operations with a comma as the separator.
        /// </summary>
        /// <param name="source">The sequence to join together.</param>
        /// <returns>Returns a string of the source joined by a comma.</returns>
        public static string JoinViaSpace(this IEnumerable<string> source)
        {
            return source.JoinViaSeparator(" ");
        }

        /// <summary>
        ///     Performs a string join operations with a comma and a space as the separator.
        /// </summary>
        /// <param name="source">The sequence to join together.</param>
        /// <returns>Returns a string of the source joined by a comma and a space.</returns>
        public static string JoinViaCommaSpace(this IEnumerable<string> source)
        {
            return JoinViaSeparator(source, ", ");
        }

        /// <summary>
        ///     Performs a string join operations with a semi-colon and a space as the separator.
        /// </summary>
        /// <param name="source">The sequence to join together.</param>
        /// <returns>Returns a string of the source joined by a comma and a space.</returns>
        public static string JoinViaSemiColonSpace(this IEnumerable<string> source)
        {
            return source.JoinViaSeparator("; ");
        }

        /// <summary>
        ///     Performs a string join operations with an empty string as the separator.
        /// </summary>
        /// <param name="source">The sequence to join together.</param>
        /// <returns>Returns a string of the source joined by an empty string.</returns>
        public static string JoinViaEmptyString(this IEnumerable<string> source)
        {
            return source.JoinViaSeparator(string.Empty);
        }

        /// <summary>
        ///     Performs a string join operations with a pipe as the separator.
        /// </summary>
        /// <param name="source">The sequence to join together.</param>
        /// <returns>Returns a string of the source joined by a pipe.</returns>
        public static string JoinViaPipe(this IEnumerable<string> source)
        {
            return source.JoinViaSeparator("|");
        }

        /// <summary>
        ///     Performs a string join operations with a tilde as the separator.
        /// </summary>
        /// <param name="source">The sequence to join together.</param>
        /// <returns>Returns a string of the source joined by a tilde.</returns>
        public static string JoinViaTilde(this IEnumerable<string> source)
        {
            return source.JoinViaSeparator("~");
        }

        /// <summary>
        ///     Performs a string join operations with a caret (^) as the separator.
        /// </summary>
        /// <param name="source">The sequence to join together.</param>
        /// <returns>Returns a string of the source joined by a caret.</returns>
        public static string JoinViaCaret(this IEnumerable<string> source)
        {
            return source.JoinViaSeparator("^");
        }

        /// <summary>
        ///     Performs a string join operations with a space and space as the separator.
        /// </summary>
        /// <param name="source">The sequence to join together.</param>
        /// <returns>Returns a string of the source joined by a space and space.</returns>
        public static string JoinViaAnd(this IEnumerable<string> source)
        {
            return source.JoinViaSeparator(" and ");
        }

        /// <summary>
        ///     Joins via the specified separator.
        /// </summary>
        /// <param name="source">The sequence to join together.</param>
        /// <param name="separator">The specified separator.</param>
        /// <returns>A string of the source joined by the specified separator.</returns>
        public static string JoinViaSeparator(this IEnumerable<string> source, string separator)
        {
            if (source.NoItems()) return string.Empty;

            return string.Join(separator, source.ToArray());
        }

        /// <summary>
        ///     Returns the value at the index or default value if the index is not found.
        ///     In case, type of the sequence items is <c>String</c>, value returned will be <c>String.Empty</c> if the index is
        ///     not found or value is null.
        /// </summary>
        /// <typeparam name="T">The type of the sequence items.</typeparam>
        /// <param name="list">The collection to search.</param>
        /// <param name="index">The index of the value to return.</param>
        /// <returns>Item present at the specified index or default value.</returns>
        public static T ElementAtOrSafeDefault<T>(this IEnumerable<T> list, int index)
        {
            var value = list.ElementAtOrDefault(index);

            if (typeof(T) == typeof(string)) value = (T) (object) value.ToSafeString();

            return value;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
        }
    }
}