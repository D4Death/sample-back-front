using System;
using System.Collections.Generic;
using sample_api.Model;
using sample_api.Services;
using sample_ultilities;
using Microsoft.AspNetCore.Mvc;

namespace sample_api.Controller
{
    [ApiController]
    [Route("api/market")]
    public class MarketController
    {
        private readonly IMarketService _marketService;
        public MarketController(IMarketService marketService)
        {
            this._marketService = marketService;
        }

        [HttpGet]
        [Route("filter-symbol/{symbol}")]
        public Result<List<MarketSearchResponse>> ListPortfolios(string symbol)
        {
            var result = _marketService.FilterSymbol(symbol);
            return result;
        }
    }
}
