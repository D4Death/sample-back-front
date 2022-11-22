using System;
namespace net_core_sample_crawl.Model
{
    public class FilterOHLCRequest
    {
        /// <summary>
        /// Candle Inteval 
        /// </summary>
        public string Interval { get; set; }

        /// <summary>
        /// Request symbol
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Prefix: STOCK || CRYPTO
        /// </summary>
        public string Prefix { get; set; }
    }
}
