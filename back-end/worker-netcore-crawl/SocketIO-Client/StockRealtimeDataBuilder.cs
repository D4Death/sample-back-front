using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using worker_netcore_crawl.MongoManage;

namespace worker_netcore_crawl
{
    public class StockRealtimeDataBuilder : BackgroundService
    {
        //private readonly StockDbContext m_context;
        private readonly ILogger<StockRealtimeDataBuilder> _logger;

        public StockRealtimeDataBuilder(ILogger<StockRealtimeDataBuilder> logger, IServiceScopeFactory factory)
        {
            _logger = logger;
            //m_context = factory.CreateScope().ServiceProvider.GetRequiredService<StockDbContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // lay het du lieu ra khoi queue
                    List<string> tradeMessages = MessageQueue.Instance.VNStockDequeueAll();
                    if (tradeMessages.Count > 0)
                    {
                        await StackExchangeRedis.Instance.BuildStockOHLCData(tradeMessages);
                        await MongoManager.Instance.InsertManyDailyMessage(tradeMessages);
                    }
                    else
                    {
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }
    }
}
