using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_core_sample_crawl.Entity
{
    [Table("mini_intraday_msg")]
    public class MiniIntradayMsg
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("message_data")]
        public string message_data { get; set; }
    }
}
