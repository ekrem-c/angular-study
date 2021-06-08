using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Platform.Common
{
    public static class DictionaryExtensions
    {
        /// <summary>
        ///     This extension allows you to look for a dictionary value with a specified key and get the value without the
        ///     possibility of an exception.
        ///     If the key is not found, then empty string is returned.
        /// </summary>
        /// <param name="list">The string dictionary to search in.</param>
        /// <param name="key">The string key to look up.</param>
        /// <returns>Returns an empty string if the key is not found; otherwise it returns the string value based on the found key.</returns>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> list, TKey key)
        {
            TValue result;

            if (list.TryGetValue(key, out result) == false) result = default;

            return result;
        }

        /// <summary>
        ///     Returns a new <c>NameValueCollection</c> containing the values from the input Dictionary.
        /// </summary>
        /// <param name="col">
        ///     The Dictionary to be converted.
        /// </param>
        /// <returns>
        ///     A <c>NameValueCollection</c> containing the values from the input Dictionary.
        /// </returns>
        public static NameValueCollection ToNameValueCollection(this Dictionary<string, string> col)
        {
            var data = new NameValueCollection();
            if (col.AnyItems())
            {
                foreach (var item in col)
                {
                    data.Add(item.Key, item.Value);
                }
                
            }

            return data;
        }

        /// <summary>
        ///     Compiles a list of string key value pairs into a valid JSON string.
        /// </summary>
        /// <param name="list">The list of strings to convert to JSON.</param>
        /// <returns>Returns a valid JSON string based on the dictionary passed in.</returns>
        public static string ToJson(this Dictionary<string, string> list)
        {
            if (list == null) return string.Empty.WrapInCurlyBraces();

            var result = list.Select(kvp => string.Format("{0}:{1}", kvp.Key.WrapInQuotes(), kvp.Value.WrapInQuotes()))
                .JoinViaComma()
                .WrapInCurlyBraces();

            return result;
        }

        /// <summary>
        ///     Returns a formed union instance of two dictionaries.  This ensures that duplicate key violations are avoided.
        /// </summary>
        /// <typeparam name="TKey">The Type to represent the key.</typeparam>
        /// <typeparam name="TValue">The Type to represent the value.</typeparam>
        /// <param name="list">
        ///     The primary list the union is based upon.  Its keys will be what the other list's keys are compared
        ///     to.
        /// </param>
        /// <param name="second">
        ///     The second list to be added to the union.  If keys in this list already exist in the primary list,
        ///     the inserting of that key value pair will be skipped.
        /// </param>
        /// <returns>Returns a formed union of the two dictionaries.</returns>
        public static Dictionary<TKey, TValue> Union<TKey, TValue>(this Dictionary<TKey, TValue> list,
            Dictionary<TKey, TValue> second)
        {
            if (list == null) return new Dictionary<TKey, TValue>();

            if (second.AnyItems())
            {
                var result = list.Concat(second.Where(s => list.Keys.Contains(s.Key) == false))
                    .ToDictionary(d => d.Key, d => d.Value);
                return result;
            }

            return list;
        }
    }
}