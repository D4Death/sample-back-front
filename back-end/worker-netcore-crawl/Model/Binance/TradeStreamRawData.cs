using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace worker_netcore_crawl.Model
{
    public class TradeStreamRawData
    {
        /// <summary>
        /// Event type: trade, kline, ticker,...
        /// </summary>
        [JsonProperty("e")]
        public string EventType { get; set; }

        /// <summary>
        /// Event time
        /// </summary>
        [JsonProperty("E")]
        public long EventUnixTime { get; set; }

        [JsonProperty("s")]
        public string Symbol { get; set; }

        /// <summary>
        /// Id of trade
        /// </summary>
        [JsonProperty("t")]
        public long TradeId { get; set; }

        [JsonProperty("p")]
        public double Price { get; set; }

        [JsonProperty("q")]
        public double Quantity { get; set; }

        [JsonProperty("b")]
        public long BuyerOrderId { get; set; }

        [JsonProperty("a")]
        public long SellerOrderId { get; set; }

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
        public bool IsBuyerMarketMaker { get; set; }

        [JsonProperty("M")]
        public bool Ignore { get; set; }
    }
}
