using System;

namespace PatchKit.Api.Utilities
{
    /// <summary>
    /// Utility for converting Unix time stamp.
    /// </summary>
    public static class UnixTimeConvert
    {
        /// <summary>
        /// Converts Unix time stamp to DateTime value.
        /// </summary>
        public static DateTime FromUnixTimeStamp(double unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        /// <summary>
        /// Converts DateTime value to Unix time stamp.
        /// </summary>
        public static long ToUnixTimeStamp(DateTime dateTime)
        {
            return Convert.ToInt64((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
        }
    }
}