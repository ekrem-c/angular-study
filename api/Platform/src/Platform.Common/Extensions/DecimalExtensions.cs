using System;

namespace Platform.Common
{
    /// <summary>
    ///     Extensions for decimals.
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        ///     Converts a decimal to a negative version.
        /// </summary>
        /// <param name="d">The extension method decimal.</param>
        /// <returns>The negative version of decimal.</returns>
        public static decimal ToNegative(this decimal d)
        {
            return -Math.Abs(d);
        }

        /// <summary>
        ///     Converts a decimal to a double.
        /// </summary>
        /// <param name="d">The extension method decimal.</param>
        /// <returns>The decimal as double.</returns>
        public static double ToDouble(this decimal d)
        {
            return Convert.ToDouble(d);
        }

        /// <summary>
        ///     If the value of the decimal is negative, then it will return zero, otherwise the original value.
        /// </summary>
        /// <param name="value">The decimal to be parsed.</param>
        /// <returns>The original value if not negative, otherwise zero.</returns>
        public static decimal ZeroIfNegative(this decimal value)
        {
            return Math.Sign(value) == -1
                ? 0m
                : value;
        }

        /// <summary>
        ///     Returns the value if it is above the minimum value, or the minimum value.
        /// </summary>
        /// <param name="value">The value to be parsed.</param>
        /// <param name="maximumValue">The minimum value.</param>
        /// <returns>The value, or the minimum value, whichever is higher.</returns>
        public static decimal LimitToMaxOf(this decimal value, decimal maximumValue)
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
        public static decimal LimitToMinOf(this decimal value, decimal minimumValue)
        {
            return value >= minimumValue
                ? value
                : minimumValue;
        }
    }
}