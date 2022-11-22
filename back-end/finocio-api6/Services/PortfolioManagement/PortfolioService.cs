using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sample_api6.Context;
using sample_api6.Entity;
using sample_api6.Model;
using sample_ultilities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace sample_api6.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly ApiDbContext _dbContext;
        private readonly ILogger _logger;
        private readonly ITokenBaseService _tokenBaseService;

        public PortfolioService(ApiDbContext context, ITokenBaseService tokenBaseService, ILogger<PortfolioService> logger)
        {
            _dbContext = context;
            _logger = logger;
            _tokenBaseService = tokenBaseService;
        }

        public async Task<Result> UpdatePortfolio(UpdateWatchlistRequest request)
        {
            try
            {
                var existsPortfolio = await _dbContext.Portfolios.FindAsync(request.PortfolioId);
                if (existsPortfolio == null)
                {
                    return new Result
                    {
                        Code = ResponseCode.RECORD_NOT_FOUND,
                        Message = "Not found portfolio to add"
                    };
                }

                if (!String.IsNullOrEmpty(request.Watchlist))
                    existsPortfolio.WatchList += request.Watchlist;

                if (!String.IsNullOrEmpty(request.Name) && request.Name != existsPortfolio.Name)
                    existsPortfolio.WatchList += request.Watchlist;

                await _dbContext.SaveChangesAsync();

                return new Result();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Result {
                    Code = ResponseCode.SYS_GENERIC_ERROR
                };
            }
        }

        /// <summary>
        /// Ham tao moi portfolio
        /// </summary>
        /// <param name="identify">
        /// User chua dang ky: device code
        /// User da dang ky: uid
        /// </param>
        /// <param name="watchList">
        /// List ma code theo doi
        /// </param>
        /// <returns></returns>
        public Result<FinoPortfolio> Createportfolio(string identification, string name, string watchList)
        {
            try
            {
                // Kiem tra xem user nay da ton tai chua
                bool isAnonymous = _dbContext.FinoUsers.Any(x => x.UId == identification);

                // Lay list cua user hien tai
                var hasPortfolio = _dbContext.Portfolios.Any(x => x.UId == identification);

                var newPortfolio = new FinoPortfolio
                {
                    Name = name,
                    UId = identification,
                    WatchList = watchList,
                    IsAnonymous = isAnonymous,
                    IsDefault = hasPortfolio ? false : true
                };

                _dbContext.Add(newPortfolio);
                _dbContext.SaveChanges();

                return new Result<FinoPortfolio>
                {
                    Data = newPortfolio
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Result<FinoPortfolio>
                {
                    Code = ResponseCode.SYS_GENERIC_ERROR
                };
            }
        }

        public Result<List<FinoPortfolio>> ListPortfolios()
        {
            try
            {
                var claims = _tokenBaseService.GetClaim();
                if (claims != null && claims.Count > 0)
                {
                    if (claims.ContainsKey("id"))
                    {
                        string identification = claims["id"];

                        var userPortfolios = _dbContext.Portfolios.Where(x => x.UId == identification);

                        if (userPortfolios != null)
                        {
                            return new Result<List<FinoPortfolio>>
                            {
                                Data = userPortfolios.ToList()
                            };
                        }
                    }

                    return new Result<List<FinoPortfolio>>
                    {
                        Code = ResponseCode.RECORD_NOT_FOUND,
                        Message = "Không tìm thấy thông tin portfolio!"
                    };
                }
                else
                {
                    return new Result<List<FinoPortfolio>> {
                        Code = ResponseCode.INVALID_PARAMETER,
                        Message = "Tham số đầu vào không chính xác!"
                    };
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Result<List<FinoPortfolio>>
                {
                    Code = ResponseCode.SYS_GENERIC_ERROR
                };
            }
        }

        /// <summary>
        /// Khi user chưa có tài khoản vẫn cho sử dụng bình thường (thông tin lưu theo ID máy)
        /// sau khi đăng ký thì đồng bộ các portfolio ở máy user với username mới đăng ký
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="oldIdentification"></param>
        /// <returns></returns>
        public Result<FinoPortfolio> SyncPortfolioWithRegistedUser(string uid, string oldIdentification)
        {
            try
            {
                var portfolios = _dbContext.Portfolios.Where(x => x.UId == oldIdentification).ToList();

                if (portfolios != null && portfolios.Any())
                {
                    foreach (var portfolio in portfolios)
                    {
                        portfolio.UId = uid;
                    }

                    _dbContext.UpdateRange(portfolios);
                    _dbContext.SaveChanges();

                    var retVal = portfolios.FirstOrDefault(x => x.IsDefault);

                    return new Result<FinoPortfolio> {
                        Data = retVal
                    };
                }
                else
                {
                    var newPortfolio = new FinoPortfolio {
                        IsAnonymous = false,
                        IsDefault = true,
                        Name = "DEFAULT PORTFOLIO",
                        WatchList = "HPG, VNM, HDB"
                    };

                    _dbContext.Add(newPortfolio);
                    _dbContext.SaveChanges();

                    return new Result<FinoPortfolio>
                    {
                        Data = newPortfolio
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Result<FinoPortfolio>
                {
                    Code = ResponseCode.SYS_GENERIC_ERROR
                };
            }
        }

        public async Task<Result<FinoPortfolio>> GetPortfolio(long id)
        {
            try
            {
                var portfolio = await _dbContext.Portfolios.FindAsync(id);

                if (portfolio != null)
                {
                    return new Result<FinoPortfolio>
                    {
                        Data = portfolio
                    };
                }

                return new Result<FinoPortfolio> {
                    Code = ResponseCode.RECORD_NOT_FOUND,
                    Message = $"Not found portfolio id: {id}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Result<FinoPortfolio>
                {
                    Code = ResponseCode.SYS_GENERIC_ERROR,
                    Message = ex.ToString()
                };
            }
        }
    }
}
