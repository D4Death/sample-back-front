using System;
using sample_api.Entity;
using sample_api.Model;
using sample_ultilities;

namespace sample_api.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Ham sync thong tin khi client su dung firebase authen
        /// Sau khi login bằng tk social, tạo user vs email ủn lên
        /// </summary>
        /// <returns></returns>
        Result<string> SocialLogin(SocialLoginRequest request);

        /// <summary>
        /// Lan dau user vao app, lay thong tin device de tao access token
        /// tạo luôn user anonymous vs user_name = device key
        /// </summary>
        /// <param name="deviceKey"></param>
        /// <returns></returns>
        Result<string> AnonymousLogin(string deviceKey);

        /// <summary>
        /// Login su dung user dang ky tren he thong
        /// </summary>
        /// <returns></returns>
        Result Login();

        Result<FinoUser> GetById(string uid);

        /// <summary>
        /// Ham check authen user
        /// </summary>
        /// <returns></returns>
        Result AuthenUser();

        Result RegisterFinoUSer();
    }
}
