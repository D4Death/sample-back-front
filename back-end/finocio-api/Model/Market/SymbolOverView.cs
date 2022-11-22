using System;
using Newtonsoft.Json;

namespace sample_api.Model.Market
{
    public class SymbolOverView
    {
        public string Id { get; set; }

        /// <summary>
        /// Mã chứng khoán hoặc crypto
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Giá đầu ngày
        /// Crypto: giá lúc chuyển ngày 00:00:00
        /// VNStock: giá sau phiên ATO
        /// </summary>
        [JsonProperty("sod_price")]
        public decimal SODPrice { get; set; }

        /// <summary>
        /// Tên sàn giao dịch (HNX, HSX, BINANCE, ..)
        /// </summary>
        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        /// <summary>
        /// Giá hiện tại
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Chênh lệch theo tỷ lệ (%) so sánh vs số đầu ngày
        /// </summary>
        [JsonProperty("change_percent")]
        public decimal ChangePercent => ((SODPrice - Price) * 100) / SODPrice;

        /// <summary>
        /// Chênh lệch theo giá so sánh vs số đầu ngày
        /// </summary>
        [JsonProperty("change_amount")]
        public decimal ChangeAmount => SODPrice - Price;
    }
}
