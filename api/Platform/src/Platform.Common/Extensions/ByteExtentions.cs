using System;

namespace Platform.Common
{
    /// <summary>
    ///     Provides extension methods for byte types.
    /// </summary>
    public static class ByteExtentions
    {
        /// <summary>
        ///     Converts the specified Value into the enumerated type <c>T</c> returning DefaultValue if the conversion fails.
        /// </summary>
        /// <typeparam name="T">The type that the byte will be converted to.</typeparam>
        /// <param name="value">
        ///     The value to be converted.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value to return in the event Value cannot be converted.
        /// </param>
        /// <returns>
        ///     The specified value in the Enumerated type <c>T</c> or the DefaultValue if the conversion fails.
        /// </returns>
        public static T ToEnum<T>(this byte value, T defaultValue)
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
    }
}