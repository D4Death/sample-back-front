using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using sample_api6.Model;
using sample_api6.Entity;
using sample_ultilities;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using sample_api6.DbContext;
using Dapper;
using System.Data;

namespace sample_api6.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly DapperContext _dbContext;
        private readonly ITokenBaseService _tokenBaseService;
        public UserService(DapperContext context, ITokenBaseService tokenBaseService, ILogger<UserService> logger)
        {
            _dbContext = context;
            _logger = logger;
            _tokenBaseService = tokenBaseService;
        }

        /// <summary>
        /// Login using device key, does not required user_name and password
        /// after login create temp user with user_name = device key
        /// </summary>
        /// <param name="deviceKey"></param>
        /// <returns></returns>
        public Result<string> AnonymousLogin(string deviceKey)
        {
            if (string.IsNullOrEmpty(deviceKey))
            {
                return new Result<string>
                {
                    Code = ResponseCode.INVALID_PARAMETER,
                    Message = "invalid parameter!"
                };
            }

            try
            {
                string query = "SELECT TOP 1 * FROM fino.user WHERE uid = @uid";
                using(var conn = _dbContext.CreateConnection())
                {
                    var anonymousUser = conn.QueryFirst<FinoUser>(query, new {uid = deviceKey});

                    string token = generateJwtToken(deviceKey, true);
                    // If device never use before, create anonymoud user with sample data
                    if (anonymousUser == null)
                    {
                        anonymousUser = new FinoUser
                        {
                            UId = deviceKey,
                            UserName = deviceKey,
                            AuthToken = token
                        };

                        String insertAnonymousUser = "INSER INTO fino.user (uid, user_name, password, auth_token) VALUES (@uid, @userName, @password, @authToken)";
                        var userParam = new DynamicParameters();
                        userParam.Add("uid", anonymousUser.UId, DbType.String);
                        userParam.Add("userName", anonymousUser.UserName, DbType.String);
                        userParam.Add("password", null, DbType.String);
                        userParam.Add("authToken", anonymousUser.AuthToken, DbType.String);

                        conn.Query(insertAnonymousUser, userParam);
                    }

                    var currentPortfolioQuery = "SELECT TOP 1 * FROM fino.portfolio WHERE uid = @uid";
                    var currentPortfolio = conn.QueryFirst<FinoPortfolio>(currentPortfolioQuery, new { uid = deviceKey });

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

                        String inserPorfolio = "INSER INTO fino.portfolio (name, uid, watch_list, is_default, is_anonymous) VALUES (@name, @uid, @watchList, @isDefault, @isAnonymous)";
                        var portfolioParam = new DynamicParameters();
                        portfolioParam.Add("name", currentPortfolio.Name, DbType.String);
                        portfolioParam.Add("uid", currentPortfolio.UId, DbType.String);
                        portfolioParam.Add("watchList", currentPortfolio.WatchList, DbType.String);
                        portfolioParam.Add("isDefault", currentPortfolio.IsDefault, DbType.Boolean);
                        portfolioParam.Add("isAnonymous", currentPortfolio.IsAnonymous, DbType.Boolean);

                        conn.Query(inserPorfolio, portfolioParam);
                    }

                    return new Result<string>
                    {
                        Data = token
                    };
                }
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

                if (!String.IsNullOrEmpty(authToken))
                {
                    String query = "SELECT TOP 1 * FROM fino.user where auth_token = @authToken";
                    using (var conn = _dbContext.CreateConnection()) {
                        var userAuthen = conn.Query<FinoUser>(query, new { authToken = authToken });

                        if (userAuthen != null)
                        {
                            return new Result();
                        }

                        return new Result
                        {
                            Code = ResponseCode.UNAUTHORIZED
                        };
                    }
                    
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
                    Message = "Authen user failure!"
                };
            }
        }

        public Result<FinoUser> GetById(string uid)
        {
            try
            {
                string query = "SELECT TOP 1 * FROM fino.user WHERE uid = @uid";
                using (var conn = _dbContext.CreateConnection()) {
                    var user = conn.QueryFirstOrDefault<FinoUser>(query, new { uid = uid });

                    if (user is not null)
                    {
                        return new Result<FinoUser>
                        {
                            Data = user!
                        };
                    }

                    return new Result<FinoUser>
                    {
                        Code = ResponseCode.RECORD_NOT_FOUND
                    };
                }
                
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
                using (var conn = _dbContext.CreateConnection())
                using (var tranx = conn.BeginTransaction()) {
                    String query = "SELECT TOP 1 * FROM fino.user WHERE uid = @uid";
                    var socialUser = conn.QueryFirstOrDefault<FinoUser>(query, new { uid = request.UId });
                    // if social user login first time, check device key is existing
                    if (socialUser == null)
                    {
                        var existsUser = conn.QueryFirstOrDefault<FinoUser>(query, new { uid = request.DeviceKey });
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

                            String insertUser = "INSER INTO fino.user (uid, user_name, password, auth_token) VALUES (@uid, @userName, @password, @authToken)";
                            var userParam = new DynamicParameters();
                            userParam.Add("uid", socialUser.UId, DbType.String);
                            userParam.Add("userName", socialUser.UserName, DbType.String);
                            userParam.Add("password", null, DbType.String);
                            userParam.Add("authToken", socialUser.AuthToken, DbType.String);

                            conn.Execute(insertUser, userParam);

                            String insertUserPortfolio = "INSER INTO fino.portfolio (name, uid, watch_list, is_default, is_anonymous) VALUES (@name, @uid, @watchList, @isDefault, @isAnonymous)";
                            var portfolioParam = new DynamicParameters();
                            portfolioParam.Add("name", newSosialPortfolio.Name, DbType.String);
                            portfolioParam.Add("uid", newSosialPortfolio.UId, DbType.String);
                            portfolioParam.Add("watchList", newSosialPortfolio.WatchList, DbType.String);
                            portfolioParam.Add("isDefault", newSosialPortfolio.IsDefault, DbType.Boolean);
                            portfolioParam.Add("isAnonymous", newSosialPortfolio.IsAnonymous, DbType.Boolean);

                            conn.Execute(insertUserPortfolio, portfolioParam);
                        }
                        else // If user use app without account before, map all old data into new one
                        {
                            existsUser.UId = request.UId;
                            existsUser.UserName = request.Email;
                            existsUser.AuthToken = request.SocialRefreshToken;

                            String getPortfolioQuery = "SELECT TOP 1 * FROM fino.portfolio WHERE uid = @uid";
                            var existsPortfolio = conn.QueryFirstOrDefault<FinoPortfolio>(getPortfolioQuery, new { uid = request.DeviceKey });
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
                                String insertUserPortfolio = "INSER INTO fino.portfolio (name, uid, watch_list, is_default, is_anonymous) VALUES (@name, @uid, @watchList, @isDefault, @isAnonymous)";
                                var portfolioParam = new DynamicParameters();
                                portfolioParam.Add("name", newSosialPortfolio.Name, DbType.String);
                                portfolioParam.Add("uid", newSosialPortfolio.UId, DbType.String);
                                portfolioParam.Add("watchList", newSosialPortfolio.WatchList, DbType.String);
                                portfolioParam.Add("isDefault", newSosialPortfolio.IsDefault, DbType.Boolean);
                                portfolioParam.Add("isAnonymous", newSosialPortfolio.IsAnonymous, DbType.Boolean);

                                conn.Execute(insertUserPortfolio, portfolioParam);
                            }
                            else
                            {
                                existsPortfolio.UId = request.UId;
                                existsPortfolio.IsAnonymous = false;

                                String updateUserPortfolio = "UPDATE fino.portfolio SET uid = @uid, is_anonymous = @isAnonymous";

                                var updatePortfolioParam = new DynamicParameters();
                                updatePortfolioParam.Add("uid", request.UId, DbType.String);
                                updatePortfolioParam.Add("isAnonymous", false, DbType.Boolean);

                                conn.Execute(updateUserPortfolio, updatePortfolioParam);
                            }
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

                        String insertUserInfo = "INSER INTO fino.portfolio (name, uid, watch_list, is_default, is_anonymous) VALUES (@name, @uid, @watchList, @isDefault, @isAnonymous)";
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

                    tranx.Commit();
                    tranx.Dispose();

                    return new Result<string>
                    {
                        Data = request.SocialRefreshToken
                    };
                }
                    
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
