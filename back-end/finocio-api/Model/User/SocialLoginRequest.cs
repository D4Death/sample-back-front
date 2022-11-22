using System;
namespace sample_api.Model
{
    public class SocialLoginRequest
    {
        public string UId { get; set; }
        public string Name { get; set; }
        public string SocialRefreshToken { get; set; }
        public string DeviceKey { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        /// TK đã được gửi mail verified hay chưa
        /// </summary>
        public bool EmailVerified { get; set; }
    }
}
