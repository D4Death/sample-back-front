using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sample_api.Model;
using sample_api.Services;
using sample_ultilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace sample_api.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("social-login")]
        public Result<string> SocialLogin([FromBody] SocialLoginRequest request) {
            var result = _userService.SocialLogin(request);

            return result;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("anonymous-login/{deviceKey}")]
        public Result<string> AnonymousLogin(string deviceKey)
        {
            var result = _userService.AnonymousLogin(deviceKey);

            return result;
        }

        
        [HttpGet]
        [Route("authen-user")]
        public Result AuthenUser()
        {
            var result = _userService.AuthenUser();

            return result;
        }
    }
}
