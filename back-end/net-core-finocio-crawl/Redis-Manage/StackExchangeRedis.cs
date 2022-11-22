using net_core_sample_crawl.Model;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net_core_sample_crawl
{
    public class StackExchangeRedis
    {
        private readonly ConnectionMultiplexer connection;
        private readonly IDatabase redis_db;
        private static int REDIS_RANGE = 100;

        private static readonly StackExchangeRedis m_redis = new StackExchangeRedis();

        public StackExchangeRedis()
        {
            connection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=True,connectTimeout=60000");
            redis_db = connection.GetDatabase();
        }

        public static StackExchangeRedis Instance
        {
            get
            {
                return m_redis;
            }
        }

        public async Task<List<OHLC>> ListOHLC(FilterOHLCRequest request)
        {
            var retVal = new List<OHLC>();
            try
            {
                // dua vao interval de lay thoi gian bat dau vaf ket thuc de request vao redis
                long startCandleUnixTime = GetStartRequestTime(request.Interval);
                long endCandleUnixTime = GetEndRequestTime(request.Interval);

                var data = await redis_db.ListRangeAsync($"{request.Prefix}:{request.Symbol}:{request.Interval}", startCandleUnixTime, endCandleUnixTime);

                // sample data:
                // BID;1M;1622480400000;1625072399999;47800;49500;47400;48800;671540;32632640500;5693;1500;73200000;false

                if (data.Any())
                {
                    foreach (var redisValue in data)
                    {
                        var valueArr = redisValue.ToString().Split(';');

                        retVal.Add(new OHLC {
                            Symbol = valueArr[0],
                            Interval = valueArr[1],
                            StartTime = long.Parse(valueArr[2]),
                            EndTime = long.Parse(valueArr[3]),
                            Open = double.Parse(valueArr[4]),
                            High = double.Parse(valueArr[5]),
                            Low = double.Parse(valueArr[6]),
                            Close = double.Parse(valueArr[7]),
                            BaseVolume = double.Parse(valueArr[8]),
                            QuoteVolume = double.Parse(valueArr[9]),
                            Trades = int.Parse(valueArr[10]),
                            TakerBuyAssetVolume = double.Parse(valueArr[11]),
                            TakerBuyAssetQuoteVolume = double.Parse(valueArr[12]),
                            IsKlineClose = Convert.ToBoolean(valueArr[13])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ListOHLC error {ex.ToString()}");
            }

            return retVal;
        }

        private long GetStartRequestTime(string interval)
        {
            var dateNow = DateTime.Now;

            // tinh toan so ticks trong 1 don vi interval
            var intervalInMinute = Interval.GetIntervalInMinute(interval);

            // So tick trong 1 khoang interval * REDIS_RANGE
            // vi du: interval = 1H = 60 phut, REDIS_RANGE=100,
            // rangeMinuteSize la so tick cua 100 candle co interval = 1H
            var rangeMinuteSize = REDIS_RANGE * intervalInMinute * TimeSpan.TicksPerMinute;
            var currentIndexByTick = dateNow.TimeOfDay.Ticks / rangeMinuteSize;

            // thoi gian bat dau cua 1 nen OHLC
            DateTime startTime = dateNow.Date.AddTicks(currentIndexByTick * rangeMinuteSize);

            if (interval == Interval.WEEK_1)
            {
                // thu high la ngay dau tuan
                var monday = dateNow.AddDays(1).AddDays(-(int)dateNow.DayOfWeek);

                // Voi nen tuan thi start_time la tgian bat dau ngay thu high
                startTime = monday.Date.AddTicks(currentIndexByTick * rangeMinuteSize);
            }
            else if (interval == Interval.MONTH_1)
            {
                // ngay dau tien cua thang
                var firstDayInMonth = new DateTime(dateNow.Year, dateNow.Month, 1);

                // Voi nen thang thi start_time la tgian bat dau cua ngay dau tien trong thang
                startTime = firstDayInMonth.Date.AddTicks(currentIndexByTick * rangeMinuteSize);
            }

            long startTimeUnix = ((DateTimeOffset)startTime).ToUnixTimeMilliseconds();

            return startTimeUnix;
        }

        private long GetEndRequestTime(string interval)
        {
            var dateNow = DateTime.Now;

            // tinh toan so ticks trong 1 don vi interval
            var intervalInMinute = Interval.GetIntervalInMinute(interval);

            // So tick trong 1 khoang interval
            // vi du: interval = 1H = 60 phut, minuteSize la so tick trong 60 phut
            var minuteSize = (intervalInMinute * TimeSpan.TicksPerMinute);
            var currentIndexByTick = dateNow.TimeOfDay.Ticks / minuteSize;

            // thoi gian bat dau cua 1 nen OHLC
            DateTime startTime = dateNow.Date.AddTicks(currentIndexByTick * minuteSize);

            if (interval == Interval.WEEK_1)
            {
                // thu high la ngay dau tuan
                var monday = dateNow.AddDays(1).AddDays(-(int)dateNow.DayOfWeek);

                // Voi nen tuan thi start_time la tgian bat dau ngay thu high
                startTime = monday.Date.AddTicks(currentIndexByTick * minuteSize);
            }
            else if (interval == Interval.MONTH_1)
            {
                // ngay dau tien cua thang
                var firstDayInMonth = new DateTime(dateNow.Year, dateNow.Month, 1);

                // Voi nen thang thi start_time la tgian bat dau cua ngay dau tien trong thang
                startTime = firstDayInMonth.Date.AddTicks(currentIndexByTick * minuteSize);
            }

            DateTime endTime = startTime.AddMinutes(intervalInMinute).AddTicks(-10);

            long endTimeUnix = ((DateTimeOffset)endTime).ToUnixTimeMilliseconds();

            return endTimeUnix;
        }
    }
}
