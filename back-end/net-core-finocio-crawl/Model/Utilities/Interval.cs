using System;
using System.Collections.Generic;
using System.Text;

namespace net_core_sample_crawl.Model
{
    public class Interval
    {
        public const string MINUTE_1 = "1m";
        public const string MINUTE_5 = "5m";
        public const string MINUTE_15 = "15m";
        public const string MINUTE_30 = "30m";
        public const string HOUR_1 = "1H";
        public const string HOUR_2 = "2H";
        public const string HOUR_4 = "4H";
        public const string HOUR_6 = "6H";
        public const string HOUR_12 = "12H";
        public const string DAY_1 = "1D";
        public const string DAY_3 = "3D";
        public const string WEEK_1 = "1W";
        public const string MONTH_1 = "1M";

        //public static List<string> ListInterval = new List<string> { 
        //    MINUTE_1
        //};

        public static List<string> ListInterval = new List<string> {
            MINUTE_1,
            MINUTE_5,
            MINUTE_15,
            MINUTE_30,
            HOUR_1,
            HOUR_2,
            HOUR_4,
            HOUR_6,
            HOUR_12,
            DAY_1,
            DAY_3,
            WEEK_1,
            MONTH_1
        };

        public static int GetIntervalInMinute(string interval)
        {
            switch (interval)
            {
                case MINUTE_1:
                    return (int)IntervalMinute._1m;
                    
                case MINUTE_5:
                    return (int)IntervalMinute._5m;

                case MINUTE_15:
                    return (int)IntervalMinute._15m;

                case MINUTE_30:
                    return (int)IntervalMinute._30m;

                case HOUR_1:
                    return (int)IntervalMinute._1h;

                case HOUR_2:
                    return (int)IntervalMinute._2h;

                case HOUR_4:
                    return (int)IntervalMinute._4h;

                case HOUR_6:
                    return (int)IntervalMinute._6h;

                case HOUR_12:
                    return (int)IntervalMinute._12h;

                case DAY_1:
                    return (int)IntervalMinute._1d;

                case DAY_3:
                    return (int)IntervalMinute._3d;

                case WEEK_1:
                    return (int)IntervalMinute._1w;

                case MONTH_1:
                    return DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month) * 24 * 60;

                default:
                    return 1;
            }
        }
    }

    public enum IntervalMinute : int
    {
        _1m = 1,
        _5m= 5,
        _15m = 15,
        _30m = 30,
        _1h = 60,
        _2h = 2 * 60,
        _4h = 4 * 60,
        _6h = 6*60,
        _12h = 12 * 60,
        _1d = 24 * 60,
        _3d = 3 * 24 * 60,
        _1w = 7 * 24 * 60,
        //_1M = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month) * 24 * 60,
    }
}
