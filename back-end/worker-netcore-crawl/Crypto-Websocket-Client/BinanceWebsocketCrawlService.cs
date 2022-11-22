using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using worker_netcore_crawl.Model;

namespace worker_netcore_crawl
{
    /// <summary>
    /// Handle websocket from https://binance.com
    /// </summary>
    public class BinanceWebsocketCrawlService : BackgroundService
    {
        private readonly ILogger<BinanceWebsocketCrawlService> _logger;
        private static readonly string subcribeList = "/bnbusdt@trade/linkusdt@trade/etcusdt@trade/neousdt@trade/ethusdt@trade/hotusdt@trade/dotusdt@trade/adausdt@trade";
        private readonly string _uri = "wss://stream.binance.com:9443/ws/btcusdt@trade" + subcribeList;

        public BinanceWebsocketCrawlService(ILogger<BinanceWebsocketCrawlService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"BinanceWebsocketCrawlService: {stoppingToken}");
            while (!stoppingToken.IsCancellationRequested)
            {
                using var socket = new ClientWebSocket();
                try
                {
                    await socket.ConnectAsync(new Uri(_uri), stoppingToken);

                    await Receive(socket, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR - {ex.Message}");
                }
            }

        }

        private async Task Send(ClientWebSocket socket, string data, CancellationToken stoppingToken) =>
        await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, stoppingToken);

        private async Task Receive(ClientWebSocket socket, CancellationToken stoppingToken)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            while (!stoppingToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                using var ms = new MemoryStream();
                do
                {
                    result = await socket.ReceiveAsync(buffer, stoppingToken);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                ms.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(ms, Encoding.UTF8);
                try
                {
                    // Nhận message từ ws rồi ủn vào queue
                    var tradeRawData = await reader.ReadToEndAsync();
                    MessageQueue.Instance.BinanceEnqueue(tradeRawData);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Receive socket data from binance BinanceWebsocketCrawlService {e.ToString()}");
                }
            };
        }
    }
}
