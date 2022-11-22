using System;
using System.Collections.Generic;
using sample_model;
using sample_ultilities;

namespace sample_api.Services
{
    public interface IChartManagement
    {
        public Result<List<OHLC>> ListCandleBySymbol(string symbol);
    }
}
