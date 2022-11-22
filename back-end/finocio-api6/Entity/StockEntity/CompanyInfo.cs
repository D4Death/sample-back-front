using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace sample_api6.Entity
{
    [Table("company", Schema = "stk")]
    public class CompanyInfo
    {
        [Key]
        [Column("code")]
        [JsonProperty("symbol")]
        public string Code { get; set; }

        /// <summary>
        /// Phan nganh - company industry (not sure)
        /// </summary>
        [Column("icb_code")]
        [JsonProperty("icb_code")]
        public string ICBCode { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("short_name")]
        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [Column("eng_name")]
        [JsonProperty("eng_name")]
        public string EngName { get; set; }

        /// <summary>
        /// Trụ sở chính
        /// </summary>
        [Column("head_quarters")]
        [JsonProperty("head_quarters")]
        public string HeadQuarters { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("fax")]
        public string Fax { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("web_address")]
        [JsonProperty("web_address")]
        public string WebAddress { get; set; }
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

        /// <summary>
        /// Lịch sử hình thành
        /// </summary>
        [Column("history_desc")]
        [JsonProperty("history_desc")]
        public string HistoryDesc { get; set; }

        /// <summary>
        /// Lĩnh vực kinh doanh
        /// </summary>
        [Column("business_areas")]
        [JsonProperty("business_areas")]
        public string BusinessAreas { get; set; }

        /// <summary>
        /// Số lượng nhân viên
        /// </summary>
        [Column("employees")]
        public int Employees { get; set; }

        /// <summary>
        /// Số lượng thương hiệu sở hữu
        /// </summary>
        [Column("branches")]
        public int Branches { get; set; }

        /// <summary>
        /// Số giấy phép đăng ký kinh doanh
        /// </summary>
        [Column("business_license_number")]
        [JsonProperty("business_license_number")]
        public string BusinessLicenseNumber { get; set; }

        /// <summary>
        /// Mã số thuế
        /// </summary>
        [Column("tax_id_number")]
        [JsonProperty("tax_id_number")]
        public string TaxIDNumber { get; set; }

        /// <summary>
        /// Vốn chủ sở hữu
        /// </summary>
        [Column("charter_capital")]
        [JsonProperty("charter_capital")]
        public decimal CharterCapital { get; set; }

        /// <summary>
        /// Giá niêm yết
        /// </summary>
        [Column("initial_listing_price")]
        [JsonProperty("initial_listing_price")]
        public decimal InitialListingPrice { get; set; }

        /// <summary>
        /// Khối lượng niêm yết
        /// </summary>
        [Column("listing_volume")]
        [JsonProperty("listing_volume")]
        public decimal ListingVolume { get; set; }

        /// <summary>
        /// Tỷ lệ sở hữu nhà nước
        /// </summary>
        [Column("state_ownership")]
        [JsonProperty("state_ownership")]
        public decimal StateOwnership { get; set; }

        /// <summary>
        /// Tỷ lệ sở hữu nước ngoài
        /// </summary>
        [Column("foreign_ownership")]
        [JsonProperty("foreign_ownership")]
        public decimal ForeignOwnership { get; set; }

        /// <summary>
        /// Tỷ lệ sở hữu cá nhân
        /// </summary>
        [Column("other_ownership")]
        [JsonProperty("other_ownership")]
        public decimal OtherOwnership { get; set; }

        [Column("ceo")]
        public string CEO { get; set; }

        /// <summary>
        /// Ngày niêm yết
        /// </summary>
        [Column("listing_date")]
        [JsonProperty("listing_date")]
        public DateTime ListingDate { get; set; }

        /// <summary>
        /// Ngày phát hành
        /// </summary>
        [Column("issue_date")]
        [JsonProperty("issue_date")]
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// Ngày thành lập
        /// </summary>
        [Column("establishment_date")]
        [JsonProperty("establishment_date")]
        public DateTime EstablishmentDate { get; set; }

        /// <summary>
        /// Trạng thái hiện tại có niêm yết hay không
        /// </summary>
        [Column("is_listed")]
        [JsonProperty("is_listed")]
        public bool IsListed { get; set; }
    }
}
