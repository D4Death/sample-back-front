using System;
using System.Collections.Generic;
using System.Linq;
using sample_api.Context;
using sample_api.Model;
using sample_ultilities;
using Microsoft.Extensions.Logging;

namespace sample_api.Services
{
    public class MarketService : IMarketService
    {
        private readonly ApiDbContext _context;
        private readonly ILogger _logger;

        public MarketService(ILogger<MarketService> logger, ApiDbContext context)
        {
            this._context = context;
            this._logger = logger;
        }

        public Result<List<MarketSearchResponse>> FilterSymbol(string filterSymbol)
        {
            try
            {
                var result = _context.CompanyInfos.Where(x => x.Code.Contains(filterSymbol));

                if (result.Any())
                {
                    var data = result.Select(x => new MarketSearchResponse(x)).ToList();
                    return new Result<List<MarketSearchResponse>>
                    {
                        Data = data
                    };
                }

                return new Result<List<MarketSearchResponse>>
                {
                    Data = new List<MarketSearchResponse>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return new Result<List<MarketSearchResponse>> {
                    Code = ResponseCode.SYS_GENERIC_ERROR,
                    Message = ex.Message
                };
            }
        }
    }
}
