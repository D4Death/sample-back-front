using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace worker_netcore_crawl.Model
{
    /// <summary>
    /// Class chứa thông tin đơn giản được lấy theo OHLC 1D
    /// dùng để ủn vào channel socket cập nhật thông tin watchlist
    /// </summary>
    public class MinifiedTradingData
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("current_price")]
        public double CurrentPrice { get; set; }

        [JsonProperty("change")]
        public double Change { get; set; }

        [JsonProperty("change_percent")]
        public double ChangePercent { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        public  MinifiedTradingData (OHLC model) {
            Symbol = model.Symbol;
            CurrentPrice = model.Close;
            Change = model.Change;
            ChangePercent = model.ChangePercent;
            Amount = model.BaseVolume;
        }
    }
}
