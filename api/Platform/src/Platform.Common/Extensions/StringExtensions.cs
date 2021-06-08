using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Platform.Common
{
    /// <summary>
    ///     Enum to define how a string should be trimmed.
    /// </summary>
    public enum StringTrim
    {
        /// <summary>
        ///     The string should not be trimmed.
        /// </summary>
        None = 0,

        /// <summary>
        ///     The whitespace at the start should be trimmed.
        /// </summary>
        TrimStart = 1,

        /// <summary>
        ///     The whitespace at the end should be trimmed.
        /// </summary>
        TrimEnd = 2,

        /// <summary>
        ///     The whitespace at the start and end should be trimmed.
        /// </summary>
        Trim = 3
    }

    /// <summary>
    ///     Provides extension methods for Integer types.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Trace message when the parameter can not be converted.
        ///     This should be traced as "ERROR".
        /// </summary>
        private const string TRACE_CAN_NOT_CONVERT_VALUE =
            "To {0} conversion: Could not convert the value '{1}', so using the default value of '{2}'.";

        /// <summary>
        ///     Trace message when a NaNSymbol is received in the correct locale.
        ///     This should be traced as "INFORMATION".
        /// </summary>
        private const string TRACE_NAN_SYMBOL_CORRECT_CULTURE =
            "To {0} conversion: Converting NaNSymbol '{1}' into the default value of '{2}'.";

        /// <summary>
        ///     Trace message when a NaNSymbol is received in the WRONG locale.
        ///     This should be traced as "WARNING".
        /// </summary>
        private const string TRACE_NAN_SYMBOL_INCORRECT_CULTURE =
            "To {0} conversion: Could not convert '{1}';  the NaNSymbol for the Locale '{2}' passed into this method is '{3}' but this appears to be the NaNSymbol for the locale'{4}' (this *may* indicate that we are using an incorrect Locale).";

        /// <summary>
        ///     This will be created upon the creation of the type.  It seems essentially once per application lifetime.
        ///     It seems that a cache concept is built into the newer version of Regex, and therefore the Compiled option is not
        ///     needed.
        ///     This may need to be reevaluated based on the following article:
        ///     <seealso href="http://stackoverflow.com/questions/513412/how-does-regexoptions-compiled-work" />.
        /// </summary>
        private static readonly Regex _regexXml = new Regex(@"<.+>|<.+/>");

        /// <summary>
        ///     Indicates whether the string is JSON serialized.
        /// </summary>
        /// <remarks>This does not do a full test with the serializer since this is likely to be a hit on performance.</remarks>
        /// <param name="s">The string under test.</param>
        /// <returns>True if the string is JSON serialized, otherwise false.</returns>
        public static bool IsJsonSerialized(this string s)
        {
            var input = s.ToSafeString(StringTrim.Trim);

            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }

        /// <summary>
        ///     Returns a submitted string coalescing the null or empty instance to a default value.
        /// </summary>
        /// <param name="s">A string instance.</param>
        /// <param name="defaultValue">A string the returned value should be set to if submitted instance is null or empty.</param>
        /// <returns>The string value, or the default text.</returns>
        public static string ToString(this string s, string defaultValue)
        {
            return s.IsNullOrEmpty() ? defaultValue : s;
        }

        /// <summary>
        ///     Returns a submitted object coalescing the null instance to a default value.
        /// </summary>
        /// <param name="o">An object instance.</param>
        /// <param name="defaultValue">A string the returned value should be set to if submitted instance is null.</param>
        /// <returns>The string value, or the default text.</returns>
        public static string ToSafeString(this object o, string defaultValue = "")
        {
            return o == null ? defaultValue : o.ToString();
        }

        /// <summary>
        ///     Trims a string without throwing exceptions if null.
        /// </summary>
        /// <param name="s">The string object to be trimmed.</param>
        /// <returns>The string value.</returns>
        public static string SafeTrim(this string s)
        {
            return s.ToSafeString().Trim();
        }

        /// <summary>
        ///     Returns a submitted object coalescing the null instance to a Empty string.
        /// </summary>
        /// <param name="o">An object instance.</param>
        /// <param name="trim">What kind of trim is required in the result.</param>
        /// <returns>The string value.</returns>
        public static string ToSafeString(this object o, StringTrim trim)
        {
            var s = o.ToSafeString();

            switch (trim)
            {
                case StringTrim.Trim:
                {
                    s = s.Trim();
                    break;
                }

                case StringTrim.TrimEnd:
                {
                    s = s.TrimEnd();
                    break;
                }

                case StringTrim.TrimStart:
                {
                    s = s.TrimStart();
                    break;
                }
            }

            return s;
        }

        /// <summary>
        ///     Performs a replace on a string, but will not throw an exception if the string under consideration is null.
        /// </summary>
        /// <param name="s">The string that the replace should be performed on.</param>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The value to replace the old value.</param>
        /// <returns>A string, never null.</returns>
        public static string SafeReplace(this string s, string oldValue, string newValue)
        {
            // If newValue is null, this is treated as string.Empty.
            return s.ToSafeString().Replace(oldValue.ToSafeString(), newValue);
        }

        /// <summary>
        ///     Removes the spaces from the string.
        /// </summary>
        /// <param name="s">The string to remove spaces from.</param>
        /// <returns>The string value with all spaces removed.</returns>
        public static string RemoveSpaces(this string s)
        {
            return s.SafeReplace(" ", string.Empty);
        }

        /// <summary>
        ///     Compares two strings ignoring the case and returns if they evaluate as equal.
        /// </summary>
        /// <param name="stringA">The first string to compare.  Null is allowed.</param>
        /// <param name="stringB">The second string to compare.  Null is allowed</param>
        /// <returns>Returns true if the two strings evaluate as equal; false otherwise.</returns>
        public static bool IsEqualTo(this string stringA, string stringB)
        {
            return string.Equals(stringA, stringB, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Compares two strings ignoring the case and returns if they evaluate as NOT equal.
        /// </summary>
        /// <param name="stringA">The first string to compare.  Null is allowed.</param>
        /// <param name="stringB">The second string to compare.  Null is allowed</param>
        /// <returns>Returns true if the two strings evaluate as NOT equal; false otherwise.</returns>
        public static bool IsNotEqualTo(this string stringA, string stringB)
        {
            return IsEqualTo(stringA, stringB) == false;
        }

        /// <summary>
        ///     Indicates that the string has a value (is neither null, nor empty, nor consists solely of white space).
        /// </summary>
        /// <param name="value">The extension method string.</param>
        /// <returns>True if value is not null, not empty nor consists of white space; othewise false.</returns>
        public static bool HasValue(this string value)
        {
            return value.HasNoValue() == false;
        }

        /// <summary>
        ///     Indicates that the string has no value (is null, or empty or consists solely of white spaces).
        /// </summary>
        /// <param name="value">The extension method string.</param>
        /// <returns>True if value is null, empty or consists of white space; otherwise false.</returns>
        public static bool HasNoValue(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        ///     Returns the end of a string skipping the first n characters.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="count">The number of characters to skip.</param>
        /// <returns>The remainder of the string having skipped the first n characters.</returns>
        /// <remarks>A safe alternative to .Substring(n1, n2).</remarks>
        public static string Skip(this string s, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count",
                    "The number of characters to skip must not be a negative number.");

            if (s.HasNoValue()) return string.Empty;

            if (count > s.Length) return string.Empty;

            return s.Substring(count, s.Length - count);
        }

        /// <summary>
        ///     Returns the first n characters in a string.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="count">The number of characters to take.</param>
        /// <returns>A substring that is n characters long.</returns>
        /// <remarks>A safe alternative to .Substring(0, n1).</remarks>
        public static string Take(this string s, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count",
                    "The number of characters to take must not be a negative number.");

            if (s.HasNoValue()) return string.Empty;

            if (count > s.Length) count = s.Length;

            return s.Substring(0, count);
        }

        /// <summary>
        ///     Replace the line feeds in the string with empty string.
        /// </summary>
        /// <param name="s">The string instance to test.</param>
        /// <returns>Replaced string.</returns>
        public static string ReplaceLineFeedWithEmptyString(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            return s.Replace("\n", string.Empty);
        }

        /// <summary>
        ///     Replace the tab characters in the string with empty string.
        /// </summary>
        /// <param name="s">The string instance to test.</param>
        /// <returns>Replaced string.</returns>
        public static string ReplaceTabWithEmptyString(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            return s.Replace("\t", string.Empty);
        }

        /// <summary>
        ///     Replace the carriage return characters in the string with empty string.
        /// </summary>
        /// <param name="s">The string instance to test.</param>
        /// <returns>Replaced string.</returns>
        public static string ReplaceCarriageReturnWithEmptyString(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            return s.Replace("\r", string.Empty);
        }

        /// <summary>
        ///     Indicates whether a specified string is numeric.
        /// </summary>
        /// <param name="value">The extension method string to test.</param>
        /// <param name="ns">The style of number.</param>
        /// <param name="f">The formatter for the specific culture(defaults to invariant culture).</param>
        /// <returns>True if numeric, otherwise false.</returns>
        public static bool IsNumeric(this string value, NumberStyles ns, IFormatProvider f = null)
        {
            double temp;
            if (f == null) f = CultureInfo.InvariantCulture;
            return double.TryParse(value, ns, f, out temp);
        }

        /// <summary>
        ///     Indicates whether a specified string is numeric.
        /// </summary>
        /// <param name="value">The extension method string to test.</param>
        /// <param name="f">The formatter for the specific culture(defaults to invariant culture).</param>
        /// <returns>True if numeric, otherwise false.</returns>
        public static bool IsNumeric(this string value, IFormatProvider f = null)
        {
            if (f == null) f = CultureInfo.InvariantCulture;
            return value.IsNumeric(NumberStyles.Number, f);
        }

        /// <summary>
        ///     Identifies whether the <paramref name="parentString" /> starts with <paramref name="subString" />.
        /// </summary>
        /// <param name="parentString">The string in which to search.</param>
        /// <param name="subString">The string that we want to match.</param>
        /// <returns>True if  <paramref name="parentString" /> starts with <paramref name="subString" />.</returns>
        /// <remarks>Ordinal Ignore Case search.</remarks>
        public static bool IsStartingWith(this string parentString, string subString)
        {
            return parentString.ToSafeString().StartsWith(subString.ToSafeString(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Identifies whether the <paramref name="parentString" /> does not start with <paramref name="subString" />.
        /// </summary>
        /// <param name="parentString">The string in which to search.</param>
        /// <param name="subString">The string that we want to match.</param>
        /// <returns>True if  <paramref name="parentString" /> starts with <paramref name="subString" />.</returns>
        /// <remarks>Ordinal Ignore Case search.</remarks>
        public static bool IsNotStartingWith(this string parentString, string subString)
        {
            return parentString.IsStartingWith(subString) == false;
        }

        /// <summary>
        ///     Identifies whether the <paramref name="parentString" /> ends with <paramref name="subString" />.
        /// </summary>
        /// <param name="parentString">The string in which to search.</param>
        /// <param name="subString">The string that we want to match.</param>
        /// <returns>True if <paramref name="parentString" /> ends with <paramref name="subString" />.</returns>
        /// <remarks>Ordinal Ignore Case search.</remarks>
        public static bool IsEndingWith(this string parentString, string subString)
        {
            return parentString.ToSafeString().EndsWith(subString.ToSafeString(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Identifies whether the <paramref name="parentString" /> does not end with <paramref name="subString" />.
        /// </summary>
        /// <param name="parentString">The string in which to search.</param>
        /// <param name="subString">The string that we want to match.</param>
        /// <returns>True if <paramref name="parentString" /> does not ends with <paramref name="subString" />.</returns>
        /// <remarks>Ordinal Ignore Case search.</remarks>
        public static bool IsNotEndingWith(this string parentString, string subString)
        {
            return parentString.IsEndingWith(subString) == false;
        }

        /// <summary>
        ///     Safely returns an XDocument if the submitted string parses without error. If an error occurs, then a null value is
        ///     returns with the exception message returned.
        /// </summary>
        /// <param name="value">The string to attempt to parse into an XDocument.</param>
        /// <param name="exceptionMessage">
        ///     The exception message returned if the string was not successfully parsed.  NOTE: this
        ///     will be empty if an empty string was originally submitted.
        /// </param>
        /// <returns>Returns an XDocument of the submitted string if the parsing operation was successful, null otherwise.</returns>
        public static XDocument ToXDocument(this string value, out string exceptionMessage)
        {
            XDocument xd = null;
            exceptionMessage = string.Empty;

            if (value.HasValue() && value.PossiblyXml())
                try
                {
                    xd = XDocument.Parse(value);
                }
                catch (Exception ex)
                {
                    exceptionMessage = ex.Message;
                }

            return xd;
        }

        /// <summary>
        ///     Test a string to see if the value could be valid xml.  It uses regular expressions to test for a xml node.
        /// </summary>
        /// <param name="value">The string to test for xml properties.</param>
        /// <returns>True if the string matches the regex test; false otherwise.</returns>
        public static bool PossiblyXml(this string value)
        {
            if (value.HasNoValue()) return false;

            return _regexXml.IsMatch(value);
        }

        /// <summary>
        ///     Safely returns an XElement if the submitted string parses without error. If an error occurs, then a null value is
        ///     returns with the exception message returned.
        /// </summary>
        /// <param name="value">The string to attempt to parse into an XElement.</param>
        /// <param name="exceptionMessage">
        ///     The exception message returned if the string was not successfully parsed.  NOTE: this
        ///     will be empty if an empty string was originally submitted.
        /// </param>
        /// <returns>Returns an XElement of the submitted string if the parsing operation was successful, null otherwise.</returns>
        public static XElement ToXElement(this string value, out string exceptionMessage)
        {
            XElement xml = null;
            exceptionMessage = string.Empty;

            if (value.HasValue() && value.PossiblyXml())
                try
                {
                    xml = XElement.Parse(value);
                }
                catch (Exception ex)
                {
                    exceptionMessage = ex.Message;
                }

            return xml;
        }

        /// <summary>
        ///     ToXElement assists with returning null instances when no data exists in the string.
        ///     This assists in filtering empty nodes during the construction of a complex XElement.
        /// </summary>
        /// <param name="value">The content to be tested and if present placed within the node.</param>
        /// <param name="nodeName">The name of the XElement to find.</param>
        /// <param name="exceptionMessage">The exception message to return.</param>
        /// <returns>Returns an instance of an XElement if the string is not null or whitespace.</returns>
        public static XElement ToXElement(this string value, string nodeName, out string exceptionMessage)
        {
            XElement result;
            const string NULLSTRING = "null";
            exceptionMessage = string.Empty;

            if (nodeName.HasNoValue())
            {
                exceptionMessage = string.Format("The nodeName contained invalid characters: '{0}'",
                    nodeName ?? NULLSTRING);
                result = null;
            }
            else if (value.HasValue())
            {
                // Business logic defines that the value must contain some characters.
                try
                {
                    result = new XElement(nodeName, value);
                }
                catch (XmlException ex)
                {
                    // Some character sequences are not premitted
                    exceptionMessage =
                        string.Format(
                            "The XElement constructor thew an exception: nodeName = '{0}', value = '{1}', message ='{2}',",
                            nodeName, value, ex.Message);
                    result = null;
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        ///     Checks whether the a string passed in contains another string passed in.
        /// </summary>
        /// <param name="value">The instance of this string to search determining if it contains the <c>toCheck</c> string.</param>
        /// <param name="checkString">The string that may be within the <c>value</c> parameter.</param>
        /// <param name="comp">Define the string comparison.</param>
        /// <returns><c>Bool</c> if <c>value</c> contains <c>checkString</c>, otherwise false.</returns>
        public static bool IsContaining(this string value, string checkString)
        {
            if (value == null || checkString == null)
            {
                return false;
            }
            
            return value.IndexOf(checkString, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        ///     Checks whether the a string passed in contains another string passed in.
        /// </summary>
        /// <param name="value">The instance of this string to search determining if it contains the <c>toCheck</c> string.</param>
        /// <param name="checkString">The string that may be within the <c>value</c> parameter.</param>
        /// <param name="comp">Define the string comparison.</param>
        /// <param name="exceptionMessage">The exception message to return.</param>
        /// <returns><c>Bool</c> if <c>value</c> contains <c>checkString</c>, otherwise false.</returns>
        public static bool IsNotContaining(this string value, string checkString)
        {
            return !value.IsContaining(checkString);
        }

        /// <summary>
        ///     Overloads the Split method accepting a string as the parameter.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="separator">String to split on.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="options">Include System.StringSplitOptions.RemoveEmptyEntries to remove empty strings from the return.</param>
        /// <returns>The string split into an Array.</returns>
        public static string[] Split(this string s, string separator, out string exceptionMessage,
            StringSplitOptions options = StringSplitOptions.None)
        {
            exceptionMessage = string.Empty;
            if (s.HasNoValue()) return new string[] { };

            if (Enum.IsDefined(typeof(StringSplitOptions), options) == false)
            {
                // It's apparently possbile to pass an illegal value for this enum which would throw an argument exception,
                // e.g. values such as 2, or -2147483648.
                exceptionMessage = string.Format("An undefined value of the StringSplitOption was passed: '{0}'",
                    options);
                options = StringSplitOptions.None;
            }

            return s.Split(new[] {separator}, options);
        }

        /// <summary>
        ///     Converts the string to a boolean.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="defaultValue">Default value if conversion fails.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="ci">Culture Info (use Locale class).</param>
        /// <returns>The string as a boolean.</returns>
        public static bool ToBoolean(this string s, bool defaultValue, out string exceptionMessage,
            CultureInfo ci = null)
        {
            /* When comparing strings, it is vitally important to compare them in a way that is
             * culturally aware.  For example, the turkish "i" looks more like the digit "1".
             * For a brief intro, see: http://www.willasrari.com/blog/stringcompare-versus-stringequals/000189.aspx
             */

            exceptionMessage = string.Empty;

            // Have we been passed something sensible to convert?
            if (s.HasNoValue()) return defaultValue;

            if (ci == null) ci = CultureInfo.InvariantCulture;

            // Create s to return
            bool returnValue;

            // Attempt to convert it to a boolean
            if (bool.TryParse(s, out returnValue) == false)
            {
                // Have we been passed an integer?
                int i;
                if (int.TryParse(s, out i)) return Convert.ToBoolean(i);

                // negative string?
                if (string.Compare(s, bool.FalseString, true, ci) == 0 || string.Compare(s, "no", true, ci) == 0)
                    return false;

                // Positive string?
                if (string.Compare(s, bool.TrueString, true, ci) == 0 || string.Compare(s, "yes", true, ci) == 0)
                    return true;

                // Unable to convert this, so trace and return the default s
                exceptionMessage = string.Format(TRACE_CAN_NOT_CONVERT_VALUE, "Boolean", s, defaultValue);
                return defaultValue;
            }

            return returnValue;
        }

        /// <summary>
        ///     Converts the string to a Date.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="defaultValue">Default value if conversion fails.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="fmt">Formatter for locale (use Locale class).</param>
        /// <returns>The string as a Date.</returns>
        public static DateTime ToDate(this string s, DateTime defaultValue, out string exceptionMessage,
            IFormatProvider fmt = null)
        {
            exceptionMessage = string.Empty;

            // Have we been passed something sensible to convert?
            if (s.HasNoValue()) return defaultValue;

            // Create a DateTime variable to return
            DateTime result;

            if (DateTime.TryParse(s, fmt, DateTimeStyles.None, out result) == false)
            {
                // Set the default s [TryParse() will re-initialize result]
                result = defaultValue;

                // We were passed something that failed to convert
                exceptionMessage = string.Format(TRACE_CAN_NOT_CONVERT_VALUE, "DateTime", s, defaultValue);
            }

            return result;
        }

        /// <summary>
        ///     Converts the string to a Decimal.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="defaultValue">Default value if conversion fails.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="ci">Culture Info (use Locale class).</param>
        /// <returns>The string as a decimal.</returns>
        /// <param name="numberStyle">The number style to use when parsing the string.</param>
        public static decimal ToDecimal(this string s, decimal defaultValue, out string exceptionMessage,
            CultureInfo ci = null, NumberStyles numberStyle = NumberStyles.Number)
        {
            exceptionMessage = string.Empty;

            // Have we been passed something sensible to convert?
            if (s.HasNoValue()) return defaultValue;

            // Create a decimal to return
            decimal result;

            if (ci == null) ci = CultureInfo.InvariantCulture;

            if (decimal.TryParse(s, numberStyle, ci, out result) == false)
            {
                // Set the default s [TryParse() will re-initialize result]
                result = defaultValue;

                if (string.CompareOrdinal(s, ci.NumberFormat.NaNSymbol) == 0)
                    exceptionMessage = string.Format(TRACE_NAN_SYMBOL_CORRECT_CULTURE, "Decimal", s, defaultValue);
                else if (string.CompareOrdinal(s, ci.NumberFormat.NaNSymbol) == 0)
                    exceptionMessage = string.Format(TRACE_NAN_SYMBOL_INCORRECT_CULTURE, "Decimal", s, ci.Name,
                        ci.NumberFormat.NaNSymbol, ci.Name);
                else if (string.CompareOrdinal(s, ci.NumberFormat.NaNSymbol) == 0)
                    exceptionMessage = string.Format(TRACE_NAN_SYMBOL_INCORRECT_CULTURE, "Decimal", s, ci.Name,
                        ci.NumberFormat.NaNSymbol, ci.Name);
                else
                    exceptionMessage = string.Format(TRACE_CAN_NOT_CONVERT_VALUE, "Decimal", s, defaultValue);
            }

            return result;
        }

        /// <summary>
        ///     Converts the string to a Double.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="defaultValue">Default value if conversion fails.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="ci">Culture Info (use Locale class).</param>
        /// <param name="numberStyle">Number Style to user in parsing the string.</param>
        /// <returns>The string as a Double.</returns>
        public static double ToDouble(this string s, double defaultValue, out string exceptionMessage,
            CultureInfo ci = null, NumberStyles numberStyle = NumberStyles.Number)
        {
            exceptionMessage = string.Empty;

            // Have we been passed something sensible to convert?
            if (s.HasNoValue()) return defaultValue;

            // Create a Double to return
            double result;

            // Check locale is valid
            if (ci == null) ci = CultureInfo.InvariantCulture;

            if (double.TryParse(s, numberStyle, ci, out result) == false)
            {
                // Set the default s [TryParse() will re-initialize result]
                result = defaultValue;

                if (string.CompareOrdinal(s, ci.NumberFormat.NaNSymbol) == 0)
                    exceptionMessage = string.Format(TRACE_NAN_SYMBOL_CORRECT_CULTURE, "Double", s, defaultValue);
                else
                    exceptionMessage = string.Format(TRACE_CAN_NOT_CONVERT_VALUE, "Double", s, defaultValue);
            }

            return result;
        }

        /// <summary>
        ///     Converts the string to a Guid.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="defaultValue">Default value if conversion fails.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <returns>The string as a guid.</returns>
        public static Guid ToGuid(this string s, Guid defaultValue, out string exceptionMessage)
        {
            exceptionMessage = string.Empty;

            // Have we been passed something sensible to convert?
            if (s.HasNoValue())
            {
                return defaultValue;
            }

            // Assume the worst
            Guid result;

            // Note: in .NET 4.0, there is a Guid.TryParse(s, ref x) method...removes the need to catch an error
            try
            {
                result = new Guid(s);
            }
            catch
            {
                // Set the default value
                result = defaultValue;

                // We were passed something that failed to convert
                exceptionMessage = string.Format(TRACE_CAN_NOT_CONVERT_VALUE, "Guid", s, defaultValue.ToString("B"));
            }

            return result;
        }

        public static Guid ToGuid(this string s)
        {
            Guid guid = new Guid();
            Guid.TryParse(s, out guid);
            return guid;
        }

        /// <summary>
        ///     Converts the string to a short.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="defaultValue">Default value if conversion fails.</param>
        /// <param name="ci">Culture Info (use Locale class).</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <returns>The string as a short.</returns>
        public static short ToShort(this string s, int defaultValue, out string exceptionMessage, CultureInfo ci = null)
        {
            /* This method will trace the fact that "NaN" (Not a number) was passed into this method in an attempt to convert it into a number.
             * The reason being - we may want to investigate WHY we we're attempting to convert a string description...
             *
             * However, the NaNSymbol used depends upon the culture being used.
             *
             * Background reading on string comparisons:
             * http://msdn.microsoft.com/en-us/library/ms973919.aspx
             */
            exceptionMessage = string.Empty;

            // Have we been passed something sensible to convert?
            if (s.HasNoValue()) return (short) defaultValue;

            // Check locale is valid
            if (ci == null) ci = CultureInfo.InvariantCulture;

            // Assume the worst
            short result;

            // Assume that it will convert
            if (short.TryParse(s, NumberStyles.Integer, ci, out result) == false)
            {
                // Set the default s [TryParse() will re-initialize result]
                result = (short) defaultValue;

                if (string.CompareOrdinal(s, ci.NumberFormat.NaNSymbol) == 0)
                    exceptionMessage = string.Format(TRACE_NAN_SYMBOL_CORRECT_CULTURE, "Int16", s, defaultValue);
                else
                    exceptionMessage = string.Format(TRACE_CAN_NOT_CONVERT_VALUE, "Int16", s, defaultValue);
            }

            // Return
            return result;
        }

        /// <summary>
        ///     Converts the string to an Integer.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="defaultValue">Default value if conversion fails.</param>
        /// <param name="ci">Culture Info (use Locale class).</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <returns>The string as an Int.</returns>
        public static int ToInt(this string s, int defaultValue, out string exceptionMessage, CultureInfo ci = null)
        {
            /* This method will trace the fact that "NaN" (Not a number) was passed into this method in an attempt to convert it into a number.
             * The reason being - we may want to investigate WHY we we're attempting to convert a string description...
             *
             * However, the NaNSymbol used depends upon the culture being used.
             *
             * Background reading on string comparisons:
             * http://msdn.microsoft.com/en-us/library/ms973919.aspx
             */
            exceptionMessage = string.Empty;

            // Have we been passed something sensible to convert?
            if (s.HasNoValue()) return defaultValue;

            // Check locale is valid
            if (ci == null) ci = CultureInfo.InvariantCulture;

            // Create an int to return
            int result;

            // Assume that it will convert
            if (int.TryParse(s, NumberStyles.Integer, ci, out result) == false)
            {
                // Set the default value [TryParse() will re-initialize result]
                result = defaultValue;

                if (string.CompareOrdinal(s, ci.NumberFormat.NaNSymbol) == 0)
                    exceptionMessage = string.Format(TRACE_NAN_SYMBOL_CORRECT_CULTURE, "Int32", s, defaultValue);
                else
                    exceptionMessage = string.Format(TRACE_CAN_NOT_CONVERT_VALUE, "Int32", s, defaultValue);
            }

            // Return
            return result;
        }

        /// <summary>
        ///     Converts the string to an char.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="c">Default value if conversion fails.</param>
        /// <returns>The string as a Char, or the supplied default value should the conversion fail.</returns>
        public static char ToChar(this string s, char c)
        {
            char result;

            if (char.TryParse(s, out result) == false) result = c;

            return result;
        }

        /// <summary>
        ///     Converts the string to a Long.
        /// </summary>
        /// <param name="s">The extension method string.</param>
        /// <param name="defaultValue">Default value if conversion fails.</param>
        /// <param name="exceptionMessage">The exception message string.</param>
        /// <param name="ci">Culture Info (use Locale class).</param>
        /// <returns>The string as a long.</returns>
        public static long ToLong(this string s, long defaultValue, out string exceptionMessage, CultureInfo ci = null)
        {
            /* This method will trace the fact that "NaN" (Not a number) was passed into this method in an attempt to convert it into a number.
             * The reason being - we may want to investigate WHY we we're attempting to convert a string description...
             *
             * However, the NaNSymbol used depends upon the culture being used.
             *
             * Background reading on string comparisons:
             * http://msdn.microsoft.com/en-us/library/ms973919.aspx
             */
            exceptionMessage = string.Empty;

            // Have we been passed something sensible to convert?
            if (s.HasNoValue()) return defaultValue;

            // Check locale is valid
            if (ci == null) ci = CultureInfo.InvariantCulture;

            // Assume the worst
            long result;

            // Assume that it will convert
            if (long.TryParse(s, NumberStyles.Integer, ci, out result) == false)
            {
                // Set the default s [TryParse() will re-initialize result]
                result = defaultValue;

                if (string.CompareOrdinal(s, ci.NumberFormat.NaNSymbol) == 0)
                    exceptionMessage = string.Format(TRACE_NAN_SYMBOL_CORRECT_CULTURE, "Int64", s, defaultValue);
                else
                    exceptionMessage = string.Format(TRACE_CAN_NOT_CONVERT_VALUE, "Int64", s, defaultValue);
            }

            // Return
            return result;
        }

        /// <summary>
        ///     If the submitted text is null, then the default string will be returned;
        ///     otherwise a trimmed version of the submitted text is returned.
        /// </summary>
        /// <param name="text">The text to test.</param>
        /// <param name="defaultString">The string to default to.</param>
        /// <returns>Returns a string with either the default string or text submitted.</returns>
        public static string IfNullDefaultTo(this string text, string defaultString)
        {
            return text == null ? defaultString : text.Trim();
        }

        /// <summary>
        ///     If the submitted text is null or empty, then the default string will be returned;
        ///     otherwise a trimmed version of the submitted text is returned.
        /// </summary>
        /// <param name="text">The text to test.</param>
        /// <param name="defaultString">The string to default to.</param>
        /// <returns>Returns a string with either the default string or text submitted.</returns>
        public static string IfNullOrEmptyDefaultTo(this string text, string defaultString)
        {
            return text.IsNullOrEmpty() ? defaultString : text.Trim();
        }

        /// <summary>
        ///     If the submitted text is null or empty or whitespace, then the default string will be returned;
        ///     otherwise a trimmed version of the submitted text is returned.
        /// </summary>
        /// <param name="text">The text to test.</param>
        /// <param name="defaultString">The string to default to.</param>
        /// <returns>Returns a string with either the default string or text submitted.</returns>
        public static string IfNullOrWhiteSpaceDefaultTo(this string text, string defaultString)
        {
            return text.HasNoValue() ? defaultString : text.Trim();
        }

        /// <summary>
        ///     Returns the truncated version of S with an ellipses (...) if its length is greater than the specified
        ///     MaxCharacters.
        /// </summary>
        /// <param name="s">
        ///     The string to be truncated.
        /// </param>
        /// <param name="maxCharacters">
        ///     The maximum number of characters to include in the return string.
        /// </param>
        /// <returns>
        ///     The truncated version of S with an ellipses (...) if its length is greater than the specified MaxCharacters.
        /// </returns>
        public static string TruncateWithEllipses(this string s, int maxCharacters)
        {
            string result;
            if (s.HasNoValue())
            {
                result = string.Empty;
            }
            else
            {
                if (maxCharacters >= 3) return s.Length > maxCharacters ? s.Substring(0, maxCharacters - 3) + "..." : s;

                return s;
            }

            return result;
        }

        /// <summary>
        ///     Returns the string with the specified ending characters if the string is not <c>null</c> or <c>string.Empty</c>.
        /// </summary>
        /// <param name="s">
        ///     The string to be manipulated.
        /// </param>
        /// <param name="endsWithCharacters">
        ///     The characters to ensure are at the end of the string if the string is not <c>null</c> or <c>string.Empty</c>.
        /// </param>
        /// <returns>
        ///     The string with the specified ending characters if the string is not <c>null</c> or <c>string.Empty</c>.
        /// </returns>
        public static string EnsureEndsWith(this string s, string endsWithCharacters)
        {
            var result = s;
            if (s.HasValue() && s.EndsWith(endsWithCharacters) == false) result += endsWithCharacters;

            return result;
        }

        /// <summary>
        ///     Returns the string with the specified beginning characters if the string is not <c>null</c> or <c>string.Empty</c>.
        /// </summary>
        /// <param name="s">
        ///     The string to be manipulated.
        /// </param>
        /// <param name="startsWithCharacters">
        ///     The characters to ensure are at the beginning of the string if the string is not <c>null</c> or <c>string.Empty</c>
        ///     .
        /// </param>
        /// <returns>
        ///     The string with the specified beginning characters if the string is not <c>null</c> or <c>string.Empty</c>.
        /// </returns>
        public static string EnsureStartsWith(this string s, string startsWithCharacters)
        {
            var result = s;
            if (s.HasValue() && s.StartsWith(startsWithCharacters) == false)
                result = string.Format("{0}{1}", startsWithCharacters, s);

            return result;
        }

        /// <summary>
        ///     Returns a string wrapped in curly braces. I.E. {hello}.
        /// </summary>
        /// <param name="s">The string to wrap.</param>
        /// <returns>Returns the specified string in curly braces.</returns>
        public static string WrapInCurlyBraces(this string s)
        {
            return s.Wrap("{{{0}}}");
        }

        /// <summary>
        ///     Returns a string wrapped in curly braces. I.E. {hello}.
        /// </summary>
        /// <param name="s">The string to wrap.</param>
        /// <returns>Returns the specified string in curly braces.</returns>
        public static string WrapInQuotes(this string s)
        {
            return s.Wrap("\"{0}\"");
        }

        /// <summary>
        ///     Returns a string with the specified format specified.  Intended to wrap the string such as {hello}.
        /// </summary>
        /// <param name="s">The string to wrap.</param>
        /// <param name="format">
        ///     Input a format that is valid for string.Format() to utilize.  I.E. "{{{0}}}" would wrap a string
        ///     in curly braces.
        /// </param>
        /// <returns>Returns the incoming string after applying incoming format via string.Format().</returns>
        private static string Wrap(this string s, string format)
        {
            return format == null ? string.Empty : string.Format(format, s);
        }

        /// <summary>
        ///     Returns a safe copy of this <see cref="T:System.String" /> object converted
        ///     to uppercase using the casing rules of the current culture.
        /// </summary>
        /// <param name="o">An object instance.</param>
        /// <returns>The safe uppercase equivalent of the current string.</returns>
        public static string ToSafeUpper(this string o)
        {
            return o.ToSafeString().ToUpper();
        }

        /// <summary>
        ///     Returns a safe copy of this <see cref="T:System.String" /> object converted
        ///     to lower case using the casing rules of the current culture.
        /// </summary>
        /// <param name="o">An object instance.</param>
        /// <returns>The safe uppercase equivalent of the current string.</returns>
        public static string ToSafeLower(this string o)
        {
            return o.ToSafeString().ToLower();
        }
    }
}