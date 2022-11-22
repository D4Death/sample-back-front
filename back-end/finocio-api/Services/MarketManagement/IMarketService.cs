using System;
using System.Collections.Generic;
using sample_api.Model;
using sample_ultilities;

namespace sample_api.Services
{
    public interface IMarketService
    {
        Result<List<MarketSearchResponse>> FilterSymbol(string filterSymbol);
    }
}
