using System;
using System.Collections.Generic;
using sample_api.Context;
using sample_model;
using sample_ultilities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace sample_api.Services
{
    public class ChartManagement : IChartManagement
    {
        private readonly ApiDbContext _dbContext;
        private readonly ILogger _logger;
        private readonly IDatabase _redisDb;
        private readonly IConnectionMultiplexer _redis;
        public ChartManagement(ApiDbContext context, ILogger<ChartManagement> logger, IConnectionMultiplexer redis)
        {
            _dbContext = context;
            _logger = logger;
            _redis = redis;
            _redisDb = _redis.GetDatabase();
        }

        public Result<List<OHLC>> ListCandleBySymbol(string symbol)
        {
            try
            {
                return new Result<List<OHLC>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Result<List<OHLC>> {
                    Code = ResponseCode.SYS_GENERIC_ERROR
                };
            }
        }
    }
}
