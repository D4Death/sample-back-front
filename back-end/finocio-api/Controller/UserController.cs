using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sample_api.Entity;
using sample_api.Services;
using sample_ultilities;
using Microsoft.AspNetCore.Mvc;

namespace sample_api.Controller
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpGet]
        [Route("get-by-id/{uid}")]
        public Result<FinoUser> GetUserById([FromQuery] String uid)
        {
            var result = _userService.GetById(uid);

            return result;
        }
    }
}
