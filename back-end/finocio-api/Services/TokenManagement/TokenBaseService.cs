using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace sample_api.Services
{
    public class TokenBaseService : ITokenBaseService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        public TokenBaseService(IHttpContextAccessor httpContextAccessor, ILogger<TokenBaseService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Dictionary<string, string> GetClaim()
        {
            var claimInfo = new Dictionary<string, string>();
            try
            {
                if (_httpContextAccessor.HttpContext.Request != null)
                {
                    // Get the bearer token from the request context (header)
                    var bearerToken = _httpContextAccessor.HttpContext.Request
                                          .Headers[HeaderNames.Authorization]
                                          .FirstOrDefault(h => h.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase));

                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        var token = AuthenticationHeaderValue.Parse(bearerToken).Parameter;

                        var handler = new JwtSecurityTokenHandler();
                        var jwtSecurityToken = handler.ReadJwtToken(token);
                        var claims = jwtSecurityToken.Claims.ToList();

                        foreach (var item in claims)
                        {
                            if (!claimInfo.ContainsKey(item.Type))
                            {
                                claimInfo.Add(item.Type, item.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return claimInfo;
        }

        public string GetToken()
        {
            if (_httpContextAccessor.HttpContext.Request != null)
            {
                // Get the bearer token from the request context (header)
                var bearerToken = _httpContextAccessor.HttpContext.Request
                                      .Headers[HeaderNames.Authorization]
                                      .FirstOrDefault(h => h.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase));

                if (!string.IsNullOrEmpty(bearerToken))
                {
                    return AuthenticationHeaderValue.Parse(bearerToken).Parameter;
                }
            }

            return null;
        }
    }
}
