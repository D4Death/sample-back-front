using System;
using System.ComponentModel.DataAnnotations.Schema;
using worker_netcore_crawl.Model;

namespace worker_netcore_crawl.Entity
{
    [Table("binance_trading_message")]
    public class BinanceTradingMessage
    {
        /// <summary>
        /// Id of trade
        /// </summary>
        [Column("trade_id")]
        public long TradeId { get; set; }

        /// <summary>
        /// Event type: trade, kline, ticker,...
        /// </summary>
        [Column("event_type")]
        public string EventType { get; set; }

        /// <summary>
        /// Event time
        /// </summary>
        [Column("event_unix_time")]
        public long EventUnixTime { get; set; }

        [Column("symbol")]
        public string Symbol { get; set; }

        [Column("price")]
        public double Price { get; set; }

        [Column("quantity")]
        public double Quantity { get; set; }

        [Column("buyer_order_id")]
        public long BuyerOrderId { get; set; }

        [Column("seller_order_id")]
        public long SellerOrderId { get; set; }

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

        [Column("ignore")]
        public bool Ignore { get; set; }


        public BinanceTradingMessage ConvertModelToEntity(TradeStreamRawData model)
        {
            return new BinanceTradingMessage {
                TradeId = model.TradeId,
                Symbol = model.Symbol,
                Price = model.Price,
                Quantity = model.Quantity,
                BuyerOrderId = model.BuyerOrderId,
                SellerOrderId = model.SellerOrderId,
                EventType = model.EventType,
                EventUnixTime = model.EventUnixTime,
                Ignore = model.Ignore,
                IsBuyerMarketMaker = model.IsBuyerMarketMaker,
                TradeUnixTime = model.TradeUnixTime
            };
        }
    }
}
