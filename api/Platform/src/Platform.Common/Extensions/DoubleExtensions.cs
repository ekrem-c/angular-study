namespace Platform.Common
{
    /// <summary>
    ///     Extensions for doubles.
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        ///     Converts a double to a decimal.
        /// </summary>
        /// <param name="d">The extension method double.</param>
        /// <remarks>There can be serious rounding errors, so use sparingly.</remarks>
        /// <returns>The double as decimal.</returns>
        public static decimal ToDecimal(this double d)
        {
            return new decimal(d);
        }

        /// <summary>
        ///     Returns the value if it is above the minimum value, or the minimum value.
        /// </summary>
        /// <param name="value">The value to be parsed.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <returns>The value, or the minimum value, whichever is higher.</returns>
        public static double LimitToMinOf(this double value, double minimumValue)
        {
            return value >= minimumValue
                ? value
                : minimumValue;
        }
    }
}