using System;
namespace worker_netcore_crawl.Utilities
{
    public class DatetimeHelper
    {
        private static readonly TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");

        public static DateTime GetVietNamDateNow()
        {
            DateTime utcNow = DateTime.UtcNow;

            DateTime vnTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tzi);

            return vnTime;
        }

        public static DateTime ConvertToVNTime(DateTime utcDate)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, tzi);
        }

        public static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
