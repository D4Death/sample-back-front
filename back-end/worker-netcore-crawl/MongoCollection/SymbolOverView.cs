using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace worker_netcore_crawl.MongoCollection
{
    /// <summary>
    /// Bảng lưu thông tin overview của mỗi mã chứng khoán hoặc crypto
    /// bảng chỉ lưu data của ngày hiện tại, thời gian chốt để tính là 00:00 AM UTC
    /// </summary>
    public class SymbolOverView
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Mã chứng khoán hoặc crypto
        /// </summary>
        [BsonElement("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Giá đầu ngày
        /// Crypto: giá lúc chuyển ngày 00:00:00
        /// VNStock: giá sau phiên ATO
        /// </summary>
        [BsonElement("sod_price")]
        public decimal SODPrice { get; set; }

        /// <summary>
        /// Tên sàn giao dịch (HNX, HSX, BINANCE, ..)
        /// </summary>
        [BsonElement("exchange")]
        public string Exchange { get; set; }

        /// <summary>
        /// Giá hiện tại
        /// </summary>
        [BsonElement("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Chênh lệch theo tỷ lệ (%) so sánh vs số đầu ngày
        /// </summary>
        [BsonElement("change_percent")]
        public decimal ChangePercent { get; set; }

        /// <summary>
        /// Chênh lệch theo giá so sánh vs số đầu ngày
        /// </summary>
        [BsonElement("change_amount")]
        public decimal ChangeAmount { get; set; }
    }
}
