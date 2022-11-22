using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using worker_netcore_crawl.Model;

namespace worker_netcore_crawl.Entity
{
    [Table("intraday_message")]
    public class IntraDayMessage
    {
        [Key]
        [Column("id")]
        //  ID lay theo thoi gian hien tai convert ve unix time
        public long Id { get; set; }

        [Column("symbol")]
        public string Symbol { get; set; }

        [Column("price")]
        public double Price { get; set; }

        [Column("quantity")]
        public double Quantity { get; set; }

        [Column("change")]
        public double Change { get; set; }

        [Column("total_volume")]
        public double TotalVol { get; set; }

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

        public IntraDayMessage ConvertModelToEntity(StockTradingMessage model)
        {
            return new IntraDayMessage
            {
                //Id = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds(),
                Symbol = model.Symbol,
                Price = model.Price,
                Quantity = model.Quantity,
                Change = model.Change,
                TotalVol = model.TotalVol,
                Color = model.Color,
                HighPrice = model.HighPrice,
                LowPrice = model.LowPrice,
                AvgPrice = model.AvgPrice,
                SID = model.SID,
                IsBuyerMarketMaker = model.IsBuyerMarketMaker,
                TradeUnixTime = model.TradeUnixTime
            };
        }
    }
}
