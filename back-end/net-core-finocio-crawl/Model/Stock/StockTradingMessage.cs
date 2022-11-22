using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace net_core_sample_crawl.Model
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
            TimeStamp = TimeSpan.ParseExact(Time, "hh\\:mm\\:ss", CultureInfo.InvariantCulture);

            DateTime dateNow = DateTime.Now.Date + TimeStamp;
            TradeUnixTime = ((DateTimeOffset)dateNow).ToUnixTimeMilliseconds();
            Price *= 1000;
            Change *= 1000;
            HighPrice *= 1000;
            LowPrice *= 1000;
            AvgPrice *= 1000;
        }
    }
}
