using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketIOClient;
using worker_netcore_crawl.Context;

namespace worker_netcore_crawl
{
    public class SocketIOCrawlService : BackgroundService
    {
        private readonly ILogger<SocketIOCrawlService> _logger;
        private readonly StockDbContext _dbContext;

        public SocketIOCrawlService(ILogger<SocketIOCrawlService> logger, IServiceScopeFactory factory)
        {
            _logger = logger;
            _dbContext = factory.CreateScope().ServiceProvider.GetRequiredService<StockDbContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var client = new SocketIO("wss://bgdatafeed.vps.com.vn/");

                var symbolCodes = _dbContext.Companies.Where(x => x.IsListed).Select(x => x.Code).ToArray();
                var subcribeString = String.Join(",", symbolCodes);

                //client.On("board", response =>
                //{
                //    string text = JsonConvert.SerializeObject(response);
                //    //string text = response.GetValue<string>();
                //    _logger.LogInformation($"index ====== {text}");
                //});
                client.On("index", response =>
                {
                    string text = JsonConvert.SerializeObject(response);
                    //string text = response.GetValue<string>();
                    //_logger.LogInformation($"index ====== {text}");
                    JArray messageData = JArray.Parse(response.ToString());

                    if (messageData != null && messageData.Count > 0)
                    {
                        var msgContent = messageData[0];
                        MessageQueue.Instance.VNStockEnqueue(msgContent["data"].ToString());
                        //_logger.LogInformation("index ====== " + response.ToString());
                    }
                });
                client.On("stock", response =>
                {
                    string text = JsonConvert.SerializeObject(response);
                    //string text = response.GetValue<string>();
                    //_logger.LogInformation($"stock ====== {text}");
                    JArray messageData = JArray.Parse(response.ToString());

                    if (messageData != null && messageData.Count > 0)
                    {
                        var msgContent = messageData[0];
                        MessageQueue.Instance.VNStockEnqueue(msgContent["data"].ToString());
                        //_logger.LogInformation($"stock ====== {msgContent["data"]["sym"]} : {msgContent["data"]["lastPrice"]}");
                    }
                });
                //client.On("stockps", response =>
                //{
                //    string text = response.GetValue<string>();
                //    _logger.LogInformation($"stock phai sinh ====== {text}");
                //    JArray messageData = JArray.Parse(response.ToString());

                //    if (messageData != null && messageData.Count > 0)
                //    {
                //        var msgContent = messageData[0];
                //        MessageQueue.Instance.VNStockEnqueue(msgContent["data"].ToString());
                //        _logger.LogInformation($"stock Derivative ====== {msgContent["data"]["sym"]} : {msgContent["data"]["lastPrice"]}");
                //    }
                //});
                //client.On("INDEX_GOLD", response =>
                //{
                //    //string text = JsonConvert.SerializeObject(response);

                //    _logger.LogInformation("INDEX_GOLD ====== " + response.ToString());
                //});
                //client.On("INDEX_SILVER", response =>
                //{
                //    //string text = JsonConvert.SerializeObject(response);

                //    _logger.LogInformation("INDEX_SILVER ====== " + response.ToString());
                //});
                //client.On("INDEX_CRUDEOILWTI", response =>
                //{
                //    //string text = JsonConvert.SerializeObject(response);

                //    _logger.LogInformation("INDEX_CRUDEOILWTI ====== " + response.ToString());
                //});
                //client.On("INDEX_BRENTOIL", response =>
                //{
                //    //string text = JsonConvert.SerializeObject(response);

                //    _logger.LogInformation("INDEX_BRENTOIL ====== " + response.ToString());
                //});
                //client.On("INDEX_COPPER", response =>
                //{
                //    //string text = JsonConvert.SerializeObject(response);

                //    _logger.LogInformation("INDEX_COPPER ====== " + response.ToString());
                //});
                //client.On("INDEX_NATURALGAS", response =>
                //{
                //    //string text = JsonConvert.SerializeObject(response);

                //    _logger.LogInformation("INDEX_NATURALGAS ====== " + response.ToString());
                //});
                client.OnConnected += async (sender, e) =>
                {
                    _logger.LogInformation("Connected socket io");
                    //_logger.LogInformation($"CLIENT = {JsonConvert.SerializeObject(client)}");
                    //var regEvent = JsonConvert.SerializeObject(new { action = "join", list = "ACB,BID,BVH,CTG,FPT,GAS,GVR,HDB,HPG,KDH,MBB,MSN,MWG,NVL,PDR,PLX,PNJ,POW,SAB,SSI,STB,TCB,TPB,VCB,VHM,VIC,VJC,VNM,VPB,VRE" });
                    var regEvent = JsonConvert.SerializeObject(new { action = "join", list = subcribeString });
                    //_logger.LogInformation(regEvent);

                    //await client.EmitAsync("regs", JsonConvert.SerializeObject(new { action = "join", list = subcribeString }));
                    await client.EmitAsync("regs", regEvent);
                };
                client.OnPing += (sender, e) =>
                {
                    _logger.LogInformation(e.ToString());
                };

                //client.OnReconnectFailed += async (sender, e) => { _logger.LogInformation(e.ToString()); };

                //client.OnError += async (sender, e) => {
                //    _logger.LogError($"{DateTime.Now.ToShortDateString()} Connect websocket error: {e.ToString()}");
                //};

                client.OnDisconnected += async (sender, e) =>
                {
                    _logger.LogInformation(e.ToString());
                    await client.DisconnectAsync();
                };

                await client.ConnectAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"SocketIOCrawlService error connected: {e.ToString()}");
            }
        }
    }
}
