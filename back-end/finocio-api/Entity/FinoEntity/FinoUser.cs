using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sample_api.Entity
{
    [Table("user", Schema = "fino")]
    public class FinoUser
    {
        [Key]
        [Column("uid")]
        public string UId { get; set; }
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("auth_token")]
        public string AuthToken { get; set; }
    }
}
