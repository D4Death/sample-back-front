using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace sample_api6.Services
{
    public interface ITokenBaseService
    {
        String GetToken();

        Dictionary<string, string> GetClaim();
    }
}
