using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace net_core_sample_crawl.Entity
{
    /// <summary>
    /// Bảng chứa thông tin các chỉ số tài chình của một mã CK
    /// được tính hàng ngày
    /// </summary>
    [Table("symbol_financial_index")]
    public class SymbolFinancialIndex
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [JsonProperty("Symbol")]
        [Column("code")]
        public string Code { get; set; }
        [Column("name")]
        public string Name { get; set; }
        /// <summary>
        /// Sàn niêm yết
        /// </summary>
        [Column("exchange")]
        public string Exchange { get; set; }
        /// <summary>
        /// Giới thiếu ngắn về công ty
        /// </summary>
        [Column("descriptions")]
        public string Descriptions { get; set; }

        [Column("on_date")]
        public DateTime OnDate { get; set; }

        /// <summary>
        /// Năm tài chính gần nhất
        /// </summary>
        public int LFY { get; set; }

        /// <summary>
        /// Năm
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Quý
        /// </summary>
        public int Quarter { get; set; }

        #region Các nhóm chỉ số
        #region 1. Nhóm chỉ số phản ánh khả năng thanh toán: Nhằm kiểm tra khả năng thanh toán các khoản nợ ngắn hạn của doanh nghiệp. Một doanh nghiệp chỉ có thể tồn tại nếu đáp ứng được các nghĩa vụ thanh toán khi đến hạn.
        /// <summary>
        /// Chỉ số khả năng thanh toán hiện hành
        /// Hệ số này phản ánh khả năng chuyển đổi tài sản thành tiền để thanh toán cho các khoản nợ ngắn hạn, 
        /// hay thể hiện mức độ đảm bảo thanh toán các khoản nợ ngắn hạn của doanh nghiệp.
        /// Hệ số khả năng thanh toán hiện hành = Tài sản ngắn hạn / Nợ ngắn hạn
        /// </summary>
        [Column("current_ratio")]
        public decimal CurrentRatio { get; set; }

        /// <summary>
        /// Hệ số khả năng thanh toán nhanh
        /// Khả năng thanh toán nợ ngắn hạn của doanh nghiệp mà không cần phải thanh lý khẩn cấp hàng tồn kho 
        /// (do: hàng tồn kho là tài sản có tính thanh khoản thấp hơn).
        /// Hệ số khả năng thanh toán nhanh = (Tài sản ngắn hạn - Hàng tồn kho) / Nợ ngắn hạn
        /// </summary>
        [Column("quick_ratio")]
        public decimal QuickRatio { get; set; }

        /// <summary>
        /// Hệ số khả năng thanh toán tức thời
        /// Hệ số này thể hiện khả năng thanh toán của doanh nghiệp khi có khủng hoảng tài chính
        ///  khi mà hàng tồn kho không tiệu thụ được, cũng như các khoản nợ phải thu khó thu hồi.
        ///  Hệ số khả năng thanh toán tức thời = Tiền và các khoản tương đương tiền / Nợ ngắn hạn
        /// </summary>
        [Column("fast_payment_ratio")]
        public decimal FastPaymentRatio { get; set; }

        /// <summary>
        /// Hệ số khả năng thanh toán lãi vay
        /// Một doanh nghiệp vay nợ nhiều, nhưng kinh doanh không hiệu quả, 
        /// mức sinh lời của đồng vốn thấp (hoặc thua lỗ) thì khó có thể đảm bảo thanh toán tiền lãi vay đúng hạn.
        /// </summary>
        [Column("interest_coverage_ratio")]
        public decimal InterestCoverageRatio { get; set; }
        #endregion
        #region 2. Nhóm chỉ số phản ánh cơ cấu nguồn vốn và cơ cấu tài sản: Nhằm kiểm tra tính cân đối trong cơ cấu nguồn vốn của doanh nghiệp, hay mức độ tự chủ tài chính; kiểm tra tính cân đối trong việc đầu tư tài sản doanh nghiệp…
        #endregion
        #region 3. Nhóm chỉ số hiệu suất hoạt động: Đo lường mức độ sử dụng tài sản của doanh nghiệp.
        #endregion
        #region 4. Nhóm chỉ số hiệu quả hoạt động: Đo lường khả năng sinh lời của vốn.
        /// <summary>
        /// Tỷ suất lợi nhuận sau thuế trên doanh thu
        /// Chỉ số này thể hiện: tạo ra 1 đồng doanh thu thuần thì doanh nghiệp thu về bao nhiêu đồng lợi nhuận sau thuế?
        /// Tỷ suất lợi nhuận này phụ thuộc vào đặc điểm kinh tế kỹ thuật của ngành kinh doanh, chiến lược cạnh tranh của doanh nghiệp.
        /// ROS = Lợi nhuận sau thuế / Doanh thu thuần
        /// </summary>
        [Column("ros")]
        public decimal ROS { get; set; }

        /// <summary>
        /// Tỷ suất sinh lời kinh tế của tài sản
        /// Chỉ tiêu phản ánh khả năng sinh lời của tài sản, không tính đến nguồn gốc hình thành lên tài sản và thuế thu nhập doanh nghiệp.
        /// Chỉ tiêu này có tác dụng rất lớn trong việc đánh giá mối quan hệ giữa lãi suất vay vốn, 
        /// việc sử dụng vốn vay tác động như thế nào đến tỷ suất sinh lời của doanh nghiệp.
        /// BEP = lợi nhuận trước lãi vay và thuế (EBIT) / Tổng tài sản bình quân
        /// </summary>
        [Column("bep")]
        public decimal BEP { get; set; }

        /// <summary>
        /// Tỷ suất sinh lợi trên tài sản (ROA)
        /// Chỉ số này đo lường hiệu quả hoạt động của công ty mà không quan tâm đến cấu trúc tài chính
        /// ROA = Thu nhập trước thuế và lãi vay/ Tổng tài sản trung bình
        /// Trong đó: Tổng tài sản trung bình = (Tổng tài sản trong báo báo năm trước + tổng tài sản hiện hành)/2
        /// </summary>
        [Column("roa")]
        public decimal ROA { get; set; }

        /// <summary>
        /// Tỷ suất sinh lợi trên tổng vốn cổ phần(ROE)
        /// Đo lường khả năng sinh lơị đối với cổ phần nói chung, bao gồm cả cổ phần ưu đãi.
        /// ROE = Thu nhập ròng/ Tổng vốn cổ phần bình quân
        /// Trong đó: Vốn cổ phần bình quân = (Tổng vốn cổ phần năm trước+ tổng vốn cổ phần hiện tại) / 2
        /// </summary>
        [Column("roe")]
        public decimal ROE { get; set; }

        /// <summary>
        /// Chỉ số EPS
        /// Chỉ tiêu phản ánh: 1 cổ phần thường trong năm thu được bao nhiêu đồng lợi nhuận sau thuế?
        /// EPS = (LNST - Cổ tức cho cổ đông ưu đãi) / Số lượng cổ phần thường lưu hành
        /// </summary>
        [Column("eps")]
        public decimal EPS { get; set; }

        public decimal BasicEPS { get; set; }

        public decimal DilutedEPS { get; set; }
        #endregion
        #region 5. Nhóm chỉ số phân phối lợi nhuận: Đo lường mức độ phân phối lợi nhuận so với thu nhập mà công ty tạo ra cho cổ đông.
        #endregion
        #region 6. Nhóm chỉ số giá thị trường: Phản ánh giá trị thị trường của doanh nghiệp.
        /// <summary>
        /// Chỉ số PE
        /// Nhà đầu tư hay thị trường sẵn sàng trả bao nhiêu để đổi lấy 1 đồng thu nhập hiện tại của doanh nghiệp?
        /// P/E = Giá thị trường 1 cổ phần thường / Thu nhập 1 cổ phần thường
        /// </summary>
        [Column("pe_ratio")]
        public decimal PERatio { get; set; }

        public decimal BasicPE { get; set; }

        public decimal DilutedPE { get; set; }
        #endregion
        #endregion

        /// <summary>
        /// Tỷ suất sinh lợi trên vốn cổ phần thường (ROCE)
        /// Đo lường khả năng sinh lợi đối với các cổ đông thường không bao gồm cổ đông ưu đãi.
        /// ROCE = (Thu nhập ròng – Cổ tức ưu đãi)/ Vốn cổ phần thường bình quân
        /// Trong đó: Vốn cổ phần thường bình quân = (Vốn cổ phần thường trong báo cáo năm trước + vốn cổ phần thường hiện tại)/2
        /// </summary>
        [Column("roce")]
        public decimal ROCE { get; set; }
        /// <summary>
        /// Tỷ suất sinh lợi trên tổng vốn (ROTC –Return on Total Capital)
        /// Tổng vốn được định nghĩa là tổng nợ phải trả và vốn cổ phần cổ đông. 
        /// Chi phí lãi vay được định nghĩa là tổng chi phí lãi vay phải trả trừ đi tất cả thu nhập lãi vay (nếu có). 
        /// Chỉ số này đo lường tổng khả năng sinh lợi trong hoạt động của doanh nghiệp từ tất cả các nguồn tài trợ
        /// ROTC = (Thu nhập ròng + Chi phí lãi vay)/ Tổng vốn trung bình
        /// </summary>
        [Column("rotc")]
        public decimal ROTC { get; set; }
        /// <summary>
        /// Giá đóng cửa ngày hôm trước, cập nhật cuối ngày hôm trước
        /// </summary>
        [Column("last_close")]
        public decimal LastClose { get; set; }
        /// <summary>
        /// Giá mở cửa, cập nhật đầu ngày
        /// </summary>
        [Column("daily_open")]
        public decimal DailyOpen { get; set; }
        /// <summary>
        /// Volume giao dịch, cập nhật vào cuối ngày
        /// </summary>
        [Column("volume")]
        public decimal Volume { get; set; }
        /// <summary>
        /// Vol giao dịch trung bình
        /// </summary>
        [Column("average_volume")]
        public decimal AverageVolume { get; set; }
        /// <summary>
        /// Cổ phiếu đang lưu hành
        /// </summary>
        [Column("shares_outstanding")]
        public decimal SharesOutstanding { get; set; }
    }
}
