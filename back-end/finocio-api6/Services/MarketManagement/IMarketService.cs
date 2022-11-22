using System;
using System.Collections.Generic;
using sample_api6.Model;
using sample_ultilities;

namespace sample_api6.Services
{
    public interface IMarketService
    {
        Result<List<MarketSearchResponse>> FilterSymbol(string filterSymbol);
    }
}
