using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net_core_sample_crawl.Model
{
    /// <summary>
    /// Class tổng hợp dữ liệu nến
    /// </summary>
    public class OHLC
    {
        public string Symbol { get; set; }
        /// <summary>
        /// Tgian bắt đầu 1 nến dạng Unix time
        /// </summary>
        public long StartTime { get; set; }
        /// <summary>
        /// Tgian kết thúc 1 nến dạng Unix time
        /// </summary>
        public long EndTime { get; set; }
        /// <summary>
        /// Time range: 1m, 5m, 1h, 2h, ....
        /// </summary>
        public string Interval { get; set; }

        /// <summary>
        /// Giá mở cửa
        /// </summary>
        public double Open { get; set; }

        /// <summary>
        /// Giá cao nhất
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Giá thấp nhất
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// Giá đóng cửa
        /// </summary>
        public double Close { get; set; }

        /// <summary>
        /// Volume dựa trên tài sản giao dịch
        /// Ví dụ: cặp BTC/USDT => Base volume = volume BTC
        /// </summary>
        public double BaseVolume { get; set; }

        /// <summary>
        /// Volume dựa trên tài sản trao đổi
        /// Ví dụ: cặp BTC/USDT => Quote volume = volume USDT
        /// </summary>
        public double QuoteVolume { get; set; }

        /// <summary>
        /// Xác định nến này đã đóng hay chưa
        /// Để xác định so sánh End_Time vs thời gian hiện tại
        /// </summary>
        public bool IsKlineClose { get; set; }

        /// <summary>
        /// Số lượng giao dịch khớp lệnh trong 1 interval
        /// </summary>
        public int Trades { get; set; }

        /// <summary>
        /// Taker buy base asset volume
        /// Volume giao dịch lệnh Market Order dựa trên Asset Volume
        /// </summary>
        public double TakerBuyAssetVolume { get; set; }

        /// <summary>
        /// Taker buy quote asset volume
        /// Volume giao dịch lệnh Market Order dựa trên Quote Volume
        /// </summary>
        public double TakerBuyAssetQuoteVolume { get; set; }
    }
}
