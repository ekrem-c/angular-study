using System;
using System.ComponentModel;

namespace Platform.Common
{
    /// <summary>
    ///     Extensions for Enum.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        ///     Converts the ENUM to an Integer.
        /// </summary>
        /// <param name="t">The extension method string.</param>
        /// <returns>The string as an Int.</returns>
        public static int ToInt(this Enum t)
        {
            return Convert.ToInt32(t);
        }

        /// <summary>
        ///     Converts the specified value of <c>s</c> into the enumerated type <c>T</c> using a case-insensitive operation.
        /// </summary>
        /// <param name="s">
        ///     The value to be parsed.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value to return if <c>s</c> does not translate to a valid enumeration of <c>T</c>.
        /// </param>
        /// <typeparam name="T">The type to convert to an <c>enum</c>.</typeparam>
        /// <returns>
        ///     The specified value in the Enumerated type or the DefaultValue if <c>s</c> is not a valid value of the enumerated
        ///     type.
        /// </returns>
        /// <remarks>
        ///     The Generic constraing can't be:
        ///     System.Array, System.Delegate, System.Enum, System.ValueType or object.
        ///     Enum inherits from:
        ///     ValueType, IComparable, IFormattable, IConvertible
        ///     So the best course fo action is to constrain by interface and throw if not an ENUM.
        ///     Enum.TryParse vs Enum.Parse in a Try/Catch block
        ///     Testing in LinqPad, when resorting to default value, Enum.TryParse was ~ 50x quicker than using a try/catch block.
        ///     Need the Enum.IsDefined line to test the result; see:
        ///     http://stackoverflow.com/questions/3575176/why-does-enum-parse-create-undefined-entries
        /// </remarks>
        public static T ToEnum<T>(this string s, T defaultValue)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            if (typeof(T).IsEnum == false) throw new ArgumentException("T must be an enumerated type");

            T enumResult;

            if (Enum.TryParse(s, true, out enumResult))
                if (Enum.IsDefined(typeof(T), enumResult))
                    return enumResult;

            return defaultValue;
        }

        public static string GetDescription<T>(this T value)
            where T : struct
        {
            if (Enum.GetName(value.GetType(), value) != null)
                if (value.GetType().GetField(Enum.GetName(value.GetType(), value)) != null)
                    if (Attribute.GetCustomAttribute(value.GetType().GetField(Enum.GetName(value.GetType(), value)),
                        typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                        return attr.Description;
            return null;
        }
    }
}