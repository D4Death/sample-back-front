using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sample_api6.Entity
{
    [Table("user", Schema = "fino")]
    public class FinoUser
    {
        [Key]
        [Column("uid")]
        public string UId { get; set; } = String.Empty;
        [Column("user_name")]
        public string UserName { get; set; } = String.Empty;
        [Column("password")]
        public string Password { get; set; } = String.Empty;
        [Column("auth_token")]
        public string AuthToken { get; set; } = String.Empty;
    }
}
