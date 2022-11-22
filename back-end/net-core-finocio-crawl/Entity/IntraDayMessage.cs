using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_core_sample_crawl.Entity
{
    public class IntraDayMessage
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("code")]
        public string Symbol { get; set; }

        [Column("price")]
        public double Price { get; set; }

        [Column("quantity")]
        public double Quantity { get; set; }

        [Column("change")]
        public double Change { get; set; }

        [Column("total_volume")]
        public double TotalVol { get; set; }

        [Column("time")]
        public string Time { get; set; }

        public TimeSpan TimeStamp { get; set; }

        [Column("color")]
        public string Color { get; set; }

        [Column("high_price")]
        public double HighPrice { get; set; }

        [Column("low_price")]
        public double LowPrice { get; set; }

        [Column("average_price")]
        public double AvgPrice { get; set; }

        [Column("sid")]
        public string SID { get; set; }

        /// <summary>
        /// Trade time
        /// </summary>
        [Column("trade_unix_time")]
        public long TradeUnixTime { get; set; }

        /// <summary>
        /// Is the buyer the market maker? 
        /// true: Buy market order 
        /// false: Sell market order
        /// </summary>
        [Column("is_buyer_market_maker")]
        public bool IsBuyerMarketMaker { get; set; }
    }
}
