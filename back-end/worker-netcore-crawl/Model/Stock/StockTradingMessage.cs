using System;
using System.Globalization;
using Newtonsoft.Json;
using worker_netcore_crawl.Utilities;

namespace worker_netcore_crawl.Model
{
    public class StockTradingMessage
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sym")]
        public string Symbol { get; set; }

        [JsonProperty("lastPrice")]
        public double Price { get; set; }

        [JsonProperty("lastVol")]
        public double Quantity { get; set; }

        [JsonProperty("change")]
        public double Change { get; set; }

        [JsonProperty("totalVol")]
        public double TotalVol { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }
        public TimeSpan TimeStamp { get; set; }

        [JsonProperty("cl")]
        public string Color { get; set; }

        [JsonProperty("hp")]
        public double HighPrice { get; set; }

        [JsonProperty("lp")]
        public double LowPrice { get; set; }

        [JsonProperty("ap")]
        public double AvgPrice { get; set; }

        [JsonProperty("sID")]
        public string SID { get; set; }

        /// <summary>
        /// Trade time
        /// </summary>
        [JsonProperty("T")]
        public long TradeUnixTime { get; set; }

        /// <summary>
        /// Is the buyer the market maker? 
        /// true: Buy market order 
        /// false: Sell market order
        /// </summary>
        [JsonProperty("m")]
        public bool IsBuyerMarketMaker => Color == "i";

        public void ProcessData()
        {
            if (!String.IsNullOrEmpty(Time) && Time != "99:99:99")
            {
                TimeStamp = TimeSpan.ParseExact(Time, "hh\\:mm\\:ss", CultureInfo.InvariantCulture);

                DateTime dateNow = DatetimeHelper.GetVietNamDateNow().Date + TimeStamp;

                DateTime utcNow = dateNow.ToUniversalTime();

                TradeUnixTime = ((DateTimeOffset)utcNow).ToUnixTimeMilliseconds();
            }
            
            Price *= 1000;
            Change *= 1000;
            HighPrice *= 1000;
            LowPrice *= 1000;
            AvgPrice *= 1000;
        }
    }
}
// sample stock data
//[
//        {
//          "data": {
//            "id": 3220,
//            "sym": "TCH",
//            "lastPrice": 23.2,
//            "lastVol": 50,
//            "cl": "d",
//            "change": "0.30",
//            "changePc": "1.28",
//            "totalVol": 345390,
//            "time": "11:30:16",
//            "hp": 23.55,
//            "ch": "i",
//            "lp": 23.1,
//            "lc": "d",
//            "ap": 23.25,
//            "ca": "d",
//            "timeServer": "11:30:16",
//            "sID": "25052021080401248220",
//            "lv": 0
//          }
//        }
//      ]