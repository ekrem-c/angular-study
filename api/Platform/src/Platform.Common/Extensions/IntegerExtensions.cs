using System;
using System.Xml;
using System.Xml.Linq;

namespace Platform.Common
{
    /// <summary>
    ///     Provides extension methods for Integer types.
    /// </summary>
    public static class IntegerExtensions
    {
        /// <summary>
        ///     Converts the specified Value into the enumerated type <c>T</c> returning DefaultValue if the conversion fails.
        /// </summary>
        /// <typeparam name="T">The type that the integer will be converted to.</typeparam>
        /// <param name="value">
        ///     The value to be converted.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value to return in the event Value cannot be converted.
        /// </param>
        /// <returns>
        ///     The specified value in the Enumerated type <c>T</c> or the DefaultValue if the conversion fails.
        /// </returns>
        public static T ToEnum<T>(this int value, T defaultValue)
        {
            try
            {
                var name = Enum.GetName(typeof(T), value);
                return (T) Enum.Parse(typeof(T), name, true);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        ///     Retrieves the name of the constant having the specified value in the specified enumerated type <c>T</c>
        ///     returning Empty string if the constant is NOT found.
        /// </summary>
        /// <typeparam name="T">The type that this integer will be converted to.</typeparam>
        /// <param name="value">The value to be searched in the enumeration.</param>
        /// <returns>
        ///     The name of the constant specified as a value in the Enumerated type <c>T</c> or Empty string
        ///     if the corresponding constant is NOT found.
        /// </returns>
        public static string ToEnumName<T>(this int value)
        {
            return ToEnumName<T>(value, string.Empty);
        }

        /// <summary>
        ///     Retrieves the name of the constant having the specified value in the specified enumerated type <c>T</c>
        ///     returning DefaultValue if the constant is NOT found.
        /// </summary>
        /// <typeparam name="T">The type that this integer will be converted to.</typeparam>
        /// <param name="value">The value to be searched in the enumeration.</param>
        /// <param name="defaultValue">The default value to return in the event the Value cannot be found.</param>
        /// <returns>
        ///     The name of the constant specified as a value in the Enumerated type <c>T</c> or the DefaultValue
        ///     if the corresponding constant is NOT found.
        /// </returns>
        public static string ToEnumName<T>(this int value, string defaultValue)
        {
            var statusText = Enum.GetName(typeof(T), value);
            return statusText.ToString(defaultValue);
        }

        /// <summary>
        ///     Identifies if the value of the integer is odd.
        /// </summary>
        /// <param name="value">The integer.</param>
        /// <returns>True if odd, false if even.</returns>
        public static bool IsOdd(this int value)
        {
            return Math.Abs(value % 2) == 1;
        }

        /// <summary>
        ///     Identifies if the value of the integer is even.
        /// </summary>
        /// <param name="value">The integer.</param>
        /// <returns>True if even, false if odd.</returns>
        public static bool IsEven(this int value)
        {
            return value.IsOdd() == false;
        }

        /// <summary>
        ///     Returns the value if it is above the minimum value, or the minimum value.
        /// </summary>
        /// <param name="value">The value to be parsed.</param>
        /// <param name="maximumValue">The minimum value.</param>
        /// <returns>The value, or the minimum value, whichever is higher.</returns>
        public static int LimitToMaxOf(this int value, int maximumValue)
        {
            return maximumValue >= value
                ? value
                : maximumValue;
        }

        /// <summary>
        ///     Returns the value if it is above the minimum value, or the minimum value.
        /// </summary>
        /// <param name="value">The value to be parsed.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <returns>The value, or the minimum value, whichever is higher.</returns>
        public static int LimitToMinOf(this int value, int minimumValue)
        {
            return value >= minimumValue
                ? value
                : minimumValue;
        }

        /// <summary>
        ///     Returns an XElement instance named NodeName containing Value or <c>null</c> if Value is 0.
        /// </summary>
        /// <param name="value">The value placed within the node.</param>
        /// <param name="nodeName">The name of the XElement.</param>
        /// <param name="exceptionMessage">The exception message to return.</param>
        /// <returns>
        ///     An XElement instance named NodeName containing Value or <c>null</c> if Value is 0.
        /// </returns>
        public static XElement ToXElement(this int value, string nodeName, out string exceptionMessage)
        {
            XElement result = null;
            exceptionMessage = string.Empty;

            if (nodeName.HasNoValue())
                exceptionMessage =
                    string.Format("The nodeName contained invalid characters: '{0}'", nodeName ?? "null");
            else
                try
                {
                    result = new XElement(nodeName, value);
                }
                catch (XmlException ex)
                {
                    // Some character sequences are not premitted in the node name (the int value will not cause an exception)
                    exceptionMessage =
                        string.Format("The XElement constructor thew an exception: nodeName = '{0}', message ='{1}',",
                            nodeName, ex.Message);
                    result = null;
                }

            return result;
        }

        /// <summary>
        ///     Returns a boolean representation of the integer.
        /// </summary>
        /// <param name="value">The integer value to be converted.</param>
        /// <returns>A boolean representation of the integer.</returns>
        public static bool ToBool(this int value)
        {
            return Convert.ToBoolean(value);
        }
    }
}