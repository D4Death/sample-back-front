using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sample_api6.Entity
{
    /// <summary>
    /// Mỗi user sẽ có nhiều portfolio watch list khác nhau
    /// </summary>
    [Table("portfolio", Schema = "fino")]
    public class FinoPortfolio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        /// <summary>
        /// Tên của portfolio (ví dụ group các mã chứng khoán trong 1 list)
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        [Column("uid")]
        public string UId { get; set; }
        /// <summary>
        /// Danh sách mã CK theo dõi trong portfolio hiện tại
        /// </summary>
        [Column("watch_list")]
        public string WatchList { get; set; }

        /// <summary>
        /// Portfolio mặc định sẽ đc load về đầu tiên
        /// </summary>
        [Column("is_default")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Khi user chua dang ky tai khoan
        /// van cho su dung portfolio nhung chi dung dc 1 watch_list
        /// muon co nhieu hon thi phai dang ky
        /// </summary>
        [Column("is_anonymous")]
        public bool IsAnonymous { get; set; }
    }
}
