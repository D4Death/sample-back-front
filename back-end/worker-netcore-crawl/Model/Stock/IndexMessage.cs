using System;
using System.Globalization;
using Newtonsoft.Json;
using worker_netcore_crawl.Utilities;

namespace worker_netcore_crawl.Model
{
    public class IndexMessage
    {
        [JsonProperty("mc")]
        public string MarketCode { get; set; }

        [JsonProperty("cIndex")]
        public double Index { get; set; }

        [JsonProperty("oIndex")]
        public double Open { get; set; }

        [JsonProperty("vol")]
        public double Volume { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("accVol")]
        public double AccVol { get; set; }

        [JsonProperty("ot")]
        public string Others { get; set; }

        /// <summary>
        /// Trade time
        /// </summary>
        [JsonProperty("T")]
        public long TradeUnixTime { get; set; }

        public TimeSpan TimeStamp { get; set; }
        public string Symbol { get; set; }

        public void ProcessData()
        {
            string[] allowedFormats = { "hh\\:mm\\:ss\\:ffffff", "hh\\:mm\\:ss" };
            //TimeStamp = TimeSpan.ParseExact(Time, allowedFormats, CultureInfo.InvariantCulture);

            if (!String.IsNullOrEmpty(Time) && Time != "99:99:99")
            {
                TimeStamp = TimeSpan.ParseExact(Time, allowedFormats, CultureInfo.InvariantCulture);

                DateTime dateNow = DatetimeHelper.GetVietNamDateNow().Date + TimeStamp;

                DateTime utcNow = dateNow.ToUniversalTime();

                TradeUnixTime = ((DateTimeOffset)utcNow).ToUnixTimeMilliseconds();
            }

            if (Definitions.IndexDictionary.ContainsKey(MarketCode))
            {
                Symbol = Definitions.IndexDictionary[MarketCode];
            }
            else
            {
                Symbol = MarketCode;
            }
        }
    }
}

//"{
//"id": 1101,
//"mc": "03",
//"cIndex": 110.23,
//"oIndex": 110.2,
//"vol": 62460362,
//"value": 1460227,
//"time": "13:10:34:501609",
//"status": "O",
//"accVol": 23901,
//"ot": "0.03|0.03%|1460227|136|153|67"}"
