using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace net_core_sample_crawl.Entity
{
    /// <summary>
    /// Thông tin của sản chứng khoán
    /// </summary>
    [Table("exchange")]
    public class Exchange
    {
        [Key]
        [Column("code")]
        public string Code { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("location")]
        public string Location { get; set; }
        [Column("timezone")]
        public string TimeZone { get; set; }
        [Column("phone")]
        public string Phone { get; set; }
        [Column("fax")]
        public string Fax { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("website")]
        public string Website { get; set; }
        [Column("address")]
        public string Address { get; set; }
    }
}