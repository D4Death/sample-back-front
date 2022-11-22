using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using worker_netcore_crawl.Context;
using worker_netcore_crawl.Model;
using worker_netcore_crawl.MongoManage;

namespace worker_netcore_crawl
{
    /// <summary>
    /// Doc du lieu trong queue, build du lieu reltime ohlc
    /// </summary>
    public class BinanceRealtimeDataBuilder : BackgroundService
    {
        private readonly ILogger<BinanceRealtimeDataBuilder> _logger;

        public BinanceRealtimeDataBuilder(ILogger<BinanceRealtimeDataBuilder> logger, IServiceScopeFactory factory)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //List<Task> mongoTasks = new List<Task>();
                // lay het du lieu ra khoi queue
                List<string> tradeMessages = MessageQueue.Instance.BinanceDequeueAll();
                if (tradeMessages.Count > 0)
                {
                    await StackExchangeRedis.Instance.BuildCryptoOHLCData(tradeMessages);
                    //await MongoManager.Instance.InsertManyCryptoTradingMessage(tradeMessages);
                    
                    //mongoTasks.Add(MongoManager.Instance.InsertManyCryptoTradingMessage(tradeMessages));
                    //mongoTasks.Add(StackExchangeRedis.Instance.BuildCryptoOHLCData(tradeMessages));

                    //await Task.WhenAll(mongoTasks.ToArray());
                    //await Task.(buildOHLCTask);
                }
                else
                {
                    _logger.LogInformation("BinanceRealtimeDataBuilder Thread sleeping ...... ");
                    await Task.Delay(1000);
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            MessageQueue.Instance.BinanceClearQueue();

            _logger.LogInformation("BinanceRealtimeDataBuilder Hosted Service is stopping.");

            return base.StopAsync(cancellationToken);
        }

    }
}
