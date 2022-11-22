using System;
namespace sample_api.Entity
{
    public class FinoUserInfo
    {
        public string UId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        /// <summary>
        /// loai user: free, no_ads, paid
        /// </summary>
        public string Type { get; set; }
        public bool EmailVerified { get; set; }
    }
}
