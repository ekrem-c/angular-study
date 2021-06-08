using System;

namespace Platform.Common
{
    /// <summary>
    ///     Extension methods for System.DateTime objects.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Gets the first day for the year provided.
        /// </summary>
        /// <param name="date">The date from which to get the year-month string.</param>
        /// <returns>A string value containing the year and 2-digit month, separated by a hyphen.</returns>
        public static DateTime BeginningOfTheYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        /// <summary>
        ///     Returns a string formatted with the year and 2-digit month (ex: "2012-02" for February, 2012).
        /// </summary>
        /// <param name="date">The date from which to get the year-month string.</param>
        /// <returns>A string value containing the year and 2-digit month, separated by a hyphen.</returns>
        public static string ToYearMonthString(this DateTime date)
        {
            return date.ToString("yyyy-MM");
        }

        /// <summary>
        ///     Returns the first day of the last week.
        /// </summary>
        /// <param name="date">The instance of the day for which the last week is calculated.</param>
        /// <returns>The date time of the first day of last week.</returns>
        public static DateTime LastWeekFirstDay(this DateTime date)
        {
            var day = (int) date.DayOfWeek;
            return date.AddDays(-(7 + (day - 1)));
        }

        /// <summary>
        ///     Returns the last day of the last week.
        /// </summary>
        /// <param name="date">The instance of the day for which the last week is calculated.</param>
        /// <returns>The date time of the last day of last week.</returns>
        public static DateTime LastWeekLastDay(this DateTime date)
        {
            var day = (int) date.DayOfWeek;
            return date.AddDays(-day);
        }

        /// <summary>
        ///     Gets the end of day of the submitted date, which is one second ahead of the midnight of the next day.
        /// </summary>
        /// <param name="date">The date for which to get its end of day.</param>
        /// <returns>The date time of the end of day.</returns>
        public static DateTime GetEndOfDay(this DateTime date)
        {
            if (date.Date.Equals(DateTime.MaxValue.Date)) return DateTime.MaxValue;

            return date.Date.AddDays(1).AddMilliseconds(-1);
        }

        /// <summary>
        ///     Gets the end of day of the submitted date, which is one second ahead of the midnight of the next day.
        /// </summary>
        /// <param name="date">The date for which to get its end of day.</param>
        /// <returns>The date time of the end of day.</returns>
        public static DateTime GetStartOfDay(this DateTime date)
        {
            return date.Date;
        }

        /// <summary>
        ///     Determines if the date under test is equal to DateTime.MaxValue or DateTime.MinValue.
        /// </summary>
        /// <param name="date">The date under test.</param>
        /// <param name="ignoreTime">An optional flag that will ignore the time portion of the date.</param>
        /// <returns>True if the date is equivalent to the DateTime.MaxValue or DateTime.MinValue, otherwise false.</returns>
        public static bool IsMaxOrMinDate(this DateTime date, bool ignoreTime = false)
        {
            switch (ignoreTime)
            {
                case true:
                {
                    return date.GetStartOfDay().Equals(DateTime.MinValue) ||
                           date.GetEndOfDay().Equals(DateTime.MaxValue);
                }

                default:
                {
                    return date.Equals(DateTime.MinValue) || date.Equals(DateTime.MaxValue);
                }
            }
        }

        /// <summary>
        ///     Converts a time from one time zone to another.
        /// </summary>
        /// <param name="timeToAdjust">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of <see cref="TimeZoneInfo" />.</param>
        /// <param name="destinationTimeZone">The time zone to convert <see cref="TimeZoneInfo" />.</param>
        /// <param name="exceptionMessage">Exception message</param>
        /// <returns>
        ///     The date and time in the destination time zone that corresponds to the
        ///     <paramref name="timeToAdjust" /> parameter in the source time zone.
        /// </returns>
        public static DateTime AdjustTimeZone(this DateTime timeToAdjust, TimeZoneInfo sourceTimeZone,
            TimeZoneInfo destinationTimeZone, out string exceptionMessage)
        {
            exceptionMessage = string.Empty;
            var adjustedTime = timeToAdjust;
            try
            {
                adjustedTime = TimeZoneInfo.ConvertTime(timeToAdjust, sourceTimeZone, destinationTimeZone);
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }

            return adjustedTime;
        }

        /// <summary>
        ///     Determines whether the timestamp occurs before the date to compare.
        /// </summary>
        /// <param name="timeStamp">The date that we're interested in.</param>
        /// <param name="dateToCompare">The date that we want to compare.</param>
        /// <returns>True if the date stamp is before the date to compare.</returns>
        public static bool IsEarlierThan(this DateTime timeStamp, DateTime dateToCompare)
        {
            return timeStamp.CompareTo(dateToCompare) == -1;
        }

        /// <summary>
        ///     Determines whether the timestamp occurs before or equal to the date to compare.
        /// </summary>
        /// <param name="timeStamp">The date that we're interested in.</param>
        /// <param name="dateToCompare">The date that we want to compare.</param>
        /// <returns>True if the date stamp is before the date to compare.</returns>
        public static bool IsEarlierThanOrEqualTo(this DateTime timeStamp, DateTime dateToCompare)
        {
            return timeStamp.CompareTo(dateToCompare) < 1;
        }

        /// <summary>
        ///     Determines whether the timestamp occurs after the date to compare.
        /// </summary>
        /// <param name="timeStamp">The date that we're interested in.</param>
        /// <param name="dateToCompare">The date that we want to compare.</param>
        /// <returns>True if the date stamp is after the date to compare.</returns>
        public static bool IsLaterThan(this DateTime timeStamp, DateTime dateToCompare)
        {
            return timeStamp.CompareTo(dateToCompare) == 1;
        }

        /// <summary>
        ///     Determines whether the timestamp occurs after or equals the date to compare.
        /// </summary>
        /// <param name="timeStamp">The date that we're interested in.</param>
        /// <param name="dateToCompare">The date that we want to compare.</param>
        /// <returns>True if the date stamp is after the date to compare.</returns>
        public static bool IsLaterThanOrEqualTo(this DateTime timeStamp, DateTime dateToCompare)
        {
            return timeStamp.CompareTo(dateToCompare) > -1;
        }
    }
}