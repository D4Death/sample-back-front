using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using sample_api.Context;
using sample_api.Entity;
using sample_api.Model;
using sample_ultilities;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace sample_api.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly ApiDbContext _dbContext;
        private readonly ITokenBaseService _tokenBaseService;
        public UserService(ApiDbContext context, ITokenBaseService tokenBaseService, ILogger<UserService> logger)
        {
            _dbContext = context;
            _logger = logger;
            _tokenBaseService = tokenBaseService;
        }

        public Result<string> AnonymousLogin(string deviceKey)
        {
            if (string.IsNullOrEmpty(deviceKey))
            {
                return new Result<string>
                {
                    Code = ResponseCode.INVALID_PARAMETER,
                    Message = "Tham số đầu vào không chính xác!"
                };
            }

            try
            {
                var anonymousUser = _dbContext.FinoUsers.FirstOrDefault(x => x.UId == deviceKey);
                string token = generateJwtToken(deviceKey, true);
                if (anonymousUser == null)
                {
                    anonymousUser = new FinoUser
                    {
                        UId = deviceKey,
                        UserName = deviceKey,
                        AuthToken = token
                    };

                    var newAnonynousPortfolio = new FinoPortfolio
                    {
                        IsAnonymous = true,
                        IsDefault = true,
                        Name = "VN-INDEX",
                        UId = deviceKey,
                        WatchList = "VNM,VRE,VHM"
                    };

                    _dbContext.Add(anonymousUser);
                    _dbContext.Add(newAnonynousPortfolio);
                    _dbContext.SaveChanges();
                }
                else
                {
                    var currentPortfolio = _dbContext.Portfolios.FirstOrDefault(x => x.UId == anonymousUser.UId);

                    if (currentPortfolio == null)
                    {
                        currentPortfolio = new FinoPortfolio
                        {
                            IsAnonymous = true,
                            IsDefault = true,
                            Name = "VN-INDEX",
                            UId = deviceKey,
                            WatchList = "VNM,VRE,VHM"
                        };

                        _dbContext.Add(currentPortfolio);
                        _dbContext.SaveChanges();
                    }
                }

                return new Result<string>
                {
                    Data = token
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Result<string>
                {
                    Code = ResponseCode.SYS_GENERIC_ERROR
                };
            }
        }

        public Result AuthenUser()
        {
            try
            {
                var authToken = _tokenBaseService.GetToken();

                if (!string.IsNullOrEmpty(authToken))
                {
                    var userAuthen = _dbContext.FinoUsers.FirstOrDefault(x => x.AuthToken == authToken);

                    if (userAuthen != null)
                    {
                        return new Result ();
                    }

                    return new Result {
                        Code = ResponseCode.UNAUTHORIZED
                    };
                }
                else
                {
                    return new Result {
                        Code = ResponseCode.UNAUTHORIZED,
                        Message = ""
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Result {
                    Code = ResponseCode.SYS_GENERIC_ERROR,
                    Message = "Có lỗi xảy ra."
                };
            }
        }

        public Result<FinoUser> GetById(string uid)
        {
            try
            {
                var user = _dbContext.FinoUsers.FirstOrDefault(x => x.UId == uid);

                if (user != null)
                {
                    return new Result<FinoUser>
                    {
                        Data = user
                    };
                }

                return new Result<FinoUser>
                {
                    Code = ResponseCode.RECORD_NOT_FOUND
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return new Result<FinoUser>
                {
                    Code = ResponseCode.SYS_GENERIC_ERROR
                };
            }
        }

        public Result Login()
        {
            throw new NotImplementedException();
        }

        public Result RegisterFinoUSer()
        {
            throw new NotImplementedException();
        }

        public Result<string> SocialLogin(SocialLoginRequest request)
        {
            try
            {
                var socialUser = _dbContext.FinoUsers.FirstOrDefault(x => x.UId == request.UId);
                if (socialUser == null)
                {
                    // Nếu user trước đó đã có thông tin device key thì cần map thông tin cũ vào user social
                    var existsUser = _dbContext.FinoUsers.FirstOrDefault(x => x.UId == request.DeviceKey);
                    if (existsUser == null)
                    {
                        socialUser = new FinoUser
                        {
                            UId = request.UId,
                            UserName = request.Email,
                            AuthToken = request.SocialRefreshToken,
                        };
                        var newSosialPortfolio = new FinoPortfolio
                        {
                            IsAnonymous = true,
                            IsDefault = true,
                            Name = "VN-INDEX",
                            UId = request.UId,
                            WatchList = "VNM,VRE,VHM"
                        };
                        
                        _dbContext.Add(socialUser);
                        _dbContext.Add(newSosialPortfolio);
                    }
                    else // Nếu đã tồn tại thông tin anonymous trước đó thì map lại thông tin portfolio vào tài khoản
                    {
                        existsUser.UId = request.UId;
                        existsUser.UserName = request.Email;
                        existsUser.AuthToken = request.SocialRefreshToken;

                        var existsPortfolio = _dbContext.Portfolios.FirstOrDefault(x => x.UId == request.DeviceKey);
                        if (existsPortfolio == null)
                        {
                            var newSosialPortfolio = new FinoPortfolio
                            {
                                IsAnonymous = true,
                                IsDefault = true,
                                Name = "VN-INDEX",
                                UId = request.UId,
                                WatchList = "VNM,VRE,VHM"
                            };
                            _dbContext.Add(newSosialPortfolio);
                        }
                        else
                        {
                            existsPortfolio.UId = request.UId;
                            existsPortfolio.IsAnonymous = false;
                        }

                        _dbContext.Update(existsUser);
                        _dbContext.Update(existsPortfolio);
                    }

                    var socialUserInfo = new FinoUserInfo
                    {
                        Email = request.Email,
                        Address = "",
                        Age = 0,
                        Gender = "",
                        MobileNumber = request.PhoneNumber,
                        Name = request.Name,
                        Type = Definitions.USER_TYPE_FREE,
                        UserName = request.Email,
                        UId = request.UId
                    };
                    _dbContext.Add(socialUserInfo);
                }
                else
                {
                    if (socialUser.AuthToken != request.SocialRefreshToken)
                    {
                        socialUser.AuthToken = request.SocialRefreshToken;
                        _dbContext.Update(socialUser);
                    }

                    var currentPortfolio = _dbContext.Portfolios.FirstOrDefault(x => x.UId == request.UId);

                    if (currentPortfolio == null)
                    {
                        currentPortfolio = new FinoPortfolio
                        {
                            IsAnonymous = false,
                            IsDefault = true,
                            Name = "Danh mục theo dõi",
                            UId = request.UId,
                            WatchList = "VNM,VRE,VHM"
                        };

                        _dbContext.Add(currentPortfolio);
                    }
                }

                _dbContext.SaveChanges();

                return new Result<string>
                {
                    Data = request.SocialRefreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Result<string>
                {
                    Code = ResponseCode.SYS_GENERIC_ERROR
                };
            }
        }

        private string generateJwtToken(string uid, bool isAnonymous)
        {
            // generate token that is valid for 1 year
            var key = Encoding.ASCII.GetBytes(AppSettings.JwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //var key = Encoding.UTF8.GetBytes(AppSettings.JwtKey);
            var claims = new[] { new Claim("id", uid), new Claim("social_login", "true") };
            var tokenDescriptor = new JwtSecurityToken(AppSettings.Issuer, AppSettings.Issuer, claims,
                                        expires: DateTime.UtcNow.AddYears(1), signingCredentials: credentials);

            //var tokenDescriptor = new JwtSecurityToken(AppSettings.Issuer, AppSettings.Issuer, claims, DateTime.UtcNow.AddDays(100));
            //{
            //    Issuer = AppSettings.Issuer,
            //    //Subject = new ClaimsIdentity(new[] { new Claim("id", uid), new Claim("social_login", "true") }),
            //    //Payload = isAnonymous ? DateTime.UtcNow.AddDays(100) : DateTime.UtcNow.AddDays(7),
            //    //SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(tokenDescriptor);
            return token;
        }
    }
}
