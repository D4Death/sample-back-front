using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using worker_netcore_crawl.Model;
using worker_netcore_crawl.MongoManage;
using worker_netcore_crawl.Utilities;

namespace worker_netcore_crawl
{
    public class StackExchangeRedis
    {
        
        private static readonly Dictionary<string, LinkedList<OHLC>> CryptoOHLCDict = new Dictionary<string, LinkedList<OHLC>>();
        private static readonly Dictionary<string, LinkedList<OHLC>> StockOHLCDict = new Dictionary<string, LinkedList<OHLC>>();
        private static readonly int VALUE_RANGE = 5;
        private readonly ConnectionMultiplexer connection;
        private readonly IDatabase redisDb;

        private static readonly StackExchangeRedis m_redis = new StackExchangeRedis();

        public StackExchangeRedis()
        {
            connection = ConnectionMultiplexer.Connect("127.0.0.1:6379,allowAdmin=True,connectTimeout=60000");
            redisDb = connection.GetDatabase();
        }

        public static StackExchangeRedis Instance
        {
            get
            {
                return m_redis;
            }
        }

        public void InitStockOHLCDict()
        {
            StockOHLCDict.Clear();
        }

        /// <summary>
        /// Ham tinh toan message nhan ve roi luu vao redis
        /// </summary>
        /// <param name="tradeMessages"></param>
        /// <returns></returns>
        public async Task BuildCryptoOHLCData(List<string> tradeMessages)
        {
            try
            {
                //m_processTime = DateTime.Now;
                Stopwatch watch = new Stopwatch();
                watch.Start();
                var publisher = connection.GetSubscriber();

                List<Task> pubsubTasks = new List<Task>();
                foreach (var streamMsg in tradeMessages)
                {
                    var tradeDataModel = JsonConvert.DeserializeObject<TradeStreamRawData>(streamMsg);

                    // Tinh toan du lieu voi moi interval de tao ra OHLC tuong ung
                    foreach (var interval in Interval.ListInterval)
                    {
                        string redisKey = $"{RedisPrefix.CRYPTO}:{tradeDataModel.Symbol}:{interval}";

                        var pubsubOHLC = new OHLC();
                        // 18/5/2021: update du lieu tinh toan trong dictionary de tang toc do xu ly
                        // chi luu data xuong redis
                        double quoteVolume = tradeDataModel.Quantity * tradeDataModel.Price;
                        if (!CryptoOHLCDict.ContainsKey(redisKey))
                        {
                            var newOHLC = new OHLC
                            {
                                Symbol = tradeDataModel.Symbol,
                                BaseVolume = tradeDataModel.Quantity,
                                QuoteVolume = quoteVolume,
                                Interval = interval,
                                Trades = 1,
                                Open = tradeDataModel.Price,
                                High = tradeDataModel.Price,
                                Low = tradeDataModel.Price,
                                Close = tradeDataModel.Price,
                                Change = 0,
                                ChangePercent = 0,
                                TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0,
                                TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0,
                            };

                            CalculateTime(ref newOHLC);

                            CryptoOHLCDict[redisKey] = new LinkedList<OHLC>();
                            CryptoOHLCDict[redisKey].AddLast(newOHLC);
                            pubsubOHLC = newOHLC;
                        }
                        else
                        {
                            // update OHLC ở cuối
                            var updateOHLC = CryptoOHLCDict[redisKey].Last();

                            // Kiểm tra message hiện tại với thời gian của ohlc cuối cùng
                            // Nếu message bị cách quãng, thì cần tạo ra các ohlc bị thiếu với giá trị O H L C bằng nhau chèn vào khoảng thời gian bị thiếu
                            // Để tạo sự liền mạch khi client generate candles chart

                            // Trade message nằm trong khoảng thời gian của OHLC hiện tại => update các giá trị
                            if (tradeDataModel.TradeUnixTime >= updateOHLC.StartTime && tradeDataModel.TradeUnixTime <= updateOHLC.EndTime)
                            {
                                updateOHLC.BaseVolume += tradeDataModel.Quantity;
                                updateOHLC.QuoteVolume += quoteVolume;
                                updateOHLC.Trades += 1;
                                updateOHLC.High = Math.Max(tradeDataModel.Price, updateOHLC.High);
                                updateOHLC.Low = Math.Min(tradeDataModel.Price, updateOHLC.Low);
                                updateOHLC.Close = tradeDataModel.Price;
                                updateOHLC.Change = tradeDataModel.Price - updateOHLC.Open;
                                updateOHLC.ChangePercent = Math.Round((updateOHLC.Change / tradeDataModel.Price) * 100, 2, MidpointRounding.AwayFromZero);
                                updateOHLC.TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0;
                                updateOHLC.TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0;

                                pubsubOHLC = updateOHLC;
                            }
                            // Trade message bị cách quãng => tạo ra các OHLC còn thiếu
                            else
                            {
                                // Update OHLC gần nhất về trạng thái đã đóng
                                updateOHLC.IsKlineClose = true;

                                // Tạo OHLC mới
                                var newOHLC = new OHLC
                                {
                                    Symbol = tradeDataModel.Symbol,
                                    BaseVolume = tradeDataModel.Quantity,
                                    QuoteVolume = quoteVolume,
                                    Interval = interval,
                                    Trades = 1,
                                    Open = tradeDataModel.Price,
                                    High = tradeDataModel.Price,
                                    Low = tradeDataModel.Price,
                                    Close = tradeDataModel.Price,
                                    Change = 0,
                                    ChangePercent = 0,
                                    TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0,
                                    TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0,
                                };

                                CalculateTime(ref newOHLC);

                                // neu ohlc moi tao co thoi gian cach quang so vs ohlc truoc do thi tao moi nhung khoang thoi gian thieu de chen vao
                                var checkTimeUnix = newOHLC.StartTime - 1;
                                if (checkTimeUnix > updateOHLC.EndTime)
                                {
                                    // lap nguoc ve thoi gian cua ohlc gan nhat theo interval hien tai
                                    while (checkTimeUnix > updateOHLC.EndTime)
                                    {
                                        var newMissingOHLC = new OHLC
                                        {
                                            Symbol = tradeDataModel.Symbol,
                                            BaseVolume = 0,
                                            QuoteVolume = 0,
                                            Interval = interval,
                                            Trades = 0,
                                            Open = tradeDataModel.Price,
                                            High = tradeDataModel.Price,
                                            Low = tradeDataModel.Price,
                                            Close = tradeDataModel.Price,
                                            Change = 0,
                                            ChangePercent = 0,
                                            TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0,
                                            TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0,
                                        };

                                        CalculateMissingOHLC(checkTimeUnix, ref newMissingOHLC);

                                        checkTimeUnix = newMissingOHLC.StartTime - 1;

                                        CryptoOHLCDict[redisKey].AddLast(newMissingOHLC);
                                        pubsubOHLC = newMissingOHLC;

                                        // Nếu list > VALUE_RANGE thì xóa bản ghi đầu tiên
                                        if (CryptoOHLCDict[redisKey].Count > VALUE_RANGE)
                                        {
                                            CryptoOHLCDict[redisKey].RemoveFirst();
                                        }
                                    }
                                }
                                else
                                {
                                    pubsubOHLC = newOHLC;
                                    CryptoOHLCDict[redisKey].AddLast(newOHLC);

                                    if (CryptoOHLCDict[redisKey].Count > VALUE_RANGE)
                                    {
                                        CryptoOHLCDict[redisKey].RemoveFirst();
                                    }
                                }
                            }
                        }

                        List<string> stringData = new List<string>
                        {
                            pubsubOHLC.Symbol.ToString(),
                            pubsubOHLC.Interval.ToString(),
                            pubsubOHLC.StartTime.ToString(),
                            pubsubOHLC.EndTime.ToString(),
                            pubsubOHLC.Open.ToString(),
                            pubsubOHLC.High.ToString(),
                            pubsubOHLC.Low.ToString(),
                            pubsubOHLC.Close.ToString(),
                            pubsubOHLC.Change.ToString(),
                            pubsubOHLC.ChangePercent.ToString(),
                            pubsubOHLC.BaseVolume.ToString(),
                            pubsubOHLC.QuoteVolume.ToString(),
                            pubsubOHLC.Trades.ToString(),
                            pubsubOHLC.TakerBuyAssetVolume.ToString(),
                            pubsubOHLC.TakerBuyAssetQuoteVolume.ToString(),
                            pubsubOHLC.IsKlineClose.ToString().ToLower()
                        };

                        var publishValue = String.Join(";", stringData);

                        // publish data into channel by interval
                        pubsubTasks.Add(publisher.PublishAsync($"{RedisChannelPrefix.BINANCE_CHANNEL}:{pubsubOHLC.Symbol}:{pubsubOHLC.Interval}", publishValue));

                        // publish data into watchlist channel
                        if (pubsubOHLC.Interval == Interval.DAY_1)
                        {
                            var realtimeData = new MinifiedTradingData(pubsubOHLC);
                            pubsubTasks.Add(publisher.PublishAsync(RedisChannelPrefix.MINI_CRYPTO_DATA_CHANNEL, JsonConvert.SerializeObject(realtimeData)));
                        }
                    }
                }

                await Task.WhenAll(pubsubTasks);
                Console.WriteLine($"Trade Message: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Processing time of {tradeMessages.Count} in miliseconds: {watch.ElapsedMilliseconds}");
                watch.Restart();
                List<Task> addTasks = new List<Task>();

                foreach (var ohlcDict in CryptoOHLCDict)
                {
                    string key = ohlcDict.Key;

                    foreach (var ohlc in ohlcDict.Value)
                    {
                        long score = ohlc.EndTime;
                        Task<long> removedTask = redisDb.SortedSetRemoveRangeByScoreAsync(key, ohlc.StartTime, double.PositiveInfinity);
                        addTasks.Add(removedTask);

                        List<string> stringData = new List<string>
                        {
                            ohlc.Symbol.ToString(),
                            ohlc.Interval.ToString(),
                            ohlc.StartTime.ToString(),
                            ohlc.EndTime.ToString(),
                            ohlc.Open.ToString(),
                            ohlc.High.ToString(),
                            ohlc.Low.ToString(),
                            ohlc.Close.ToString(),
                            ohlc.Change.ToString(),
                            ohlc.ChangePercent.ToString(),
                            ohlc.BaseVolume.ToString(),
                            ohlc.QuoteVolume.ToString(),
                            ohlc.Trades.ToString(),
                            ohlc.TakerBuyAssetVolume.ToString(),
                            ohlc.TakerBuyAssetQuoteVolume.ToString(),
                            ohlc.IsKlineClose.ToString().ToLower()
                        };

                        var redisValue = String.Join(";", stringData);

                        //bool isSuccess = await redisDb.SortedSetAddAsync(key, redisValue, score);
                        Task<bool> isSuccess = redisDb.SortedSetAddAsync(key, redisValue, score);
                        addTasks.Add(isSuccess);
                    }
                }

                await Task.WhenAll(addTasks.ToArray());

                watch.Stop();
                Console.WriteLine($"CryptoOHLCDict.Values save to redis: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Processing time of {CryptoOHLCDict.Values.Sum(x => x.Count)} in miliseconds: {watch.ElapsedMilliseconds}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageQueue.Instance.BinanceRollbackMsg(tradeMessages);
            }
        }

        /// <summary>
        /// Ham tinh toan message nhan ve roi luu vao redis
        /// </summary>
        /// <param name="tradeMessages"></param>
        /// <returns></returns>
        public async Task BuildStockOHLCData(List<string> tradeMessages)
        {
            try
            {
                //Stopwatch watch = new Stopwatch();
                //watch.Start();
                var publisher = connection.GetSubscriber();
                List<Task> pubsubTasks = new List<Task>();

                foreach (var streamMsg in tradeMessages)
                {
                    try
                    {
                        JObject objectCheck = JObject.Parse(streamMsg);

                        if (objectCheck.ContainsKey("id"))
                        {
                            int groupId = (int)objectCheck["id"];

                            switch (groupId)
                            {
                                // handle indexes message
                                case 1101:
                                    var indexMessage = JsonConvert.DeserializeObject<IndexMessage>(streamMsg);
                                    indexMessage.ProcessData();
                                    if (indexMessage == null || String.IsNullOrEmpty(indexMessage.Symbol))
                                    {
                                        continue;
                                    }

                                    // Tinh toan du lieu voi moi interval de tao ra OHLC tuong ung
                                    foreach (var interval in Interval.ListInterval)
                                    {
                                        string redisKey = $"{RedisPrefix.STOCK}:{indexMessage.Symbol}:{interval}";

                                        // Lưu thông tin OHLC mới nhất dùng để bắn vào kênh pubsub
                                        var pubsubOHLC = new OHLC();

                                        if (!StockOHLCDict.ContainsKey(redisKey))
                                        {
                                            var newOHLC = new OHLC
                                            {
                                                Symbol = indexMessage.Symbol,
                                                BaseVolume = indexMessage.Volume,
                                                QuoteVolume = indexMessage.Volume,
                                                Interval = interval,
                                                Trades = 1,
                                                Open = indexMessage.Index,
                                                High = Math.Max(indexMessage.Index, indexMessage.Open),
                                                Low = Math.Min(indexMessage.Index, indexMessage.Open),
                                                Close = indexMessage.Index,
                                                Change = 0,
                                                ChangePercent = 0,
                                            };

                                            CalculateTime(ref newOHLC);

                                            StockOHLCDict[redisKey] = new LinkedList<OHLC>();
                                            StockOHLCDict[redisKey].AddLast(newOHLC);
                                            pubsubOHLC = newOHLC;
                                        }
                                        else
                                        {
                                            // update last OHLC
                                            var updateOHLC = StockOHLCDict[redisKey].Last();

                                            // Kiểm tra message hiện tại với thời gian của ohlc cuối cùng
                                            // Nếu message bị cách quãng, thì cần tạo ra các ohlc bị thiếu với giá trị O H L C bằng nhau chèn vào khoảng thời gian bị thiếu
                                            // Để tạo sự liền mạch khi client generate candles chart

                                            // Trade message nằm trong khoảng thời gian của OHLC hiện tại => update các giá trị
                                            if (indexMessage.TradeUnixTime >= updateOHLC.StartTime && indexMessage.TradeUnixTime <= updateOHLC.EndTime)
                                            {
                                                updateOHLC.BaseVolume += indexMessage.Volume;
                                                updateOHLC.QuoteVolume += indexMessage.Volume;
                                                updateOHLC.Trades += 1;
                                                updateOHLC.High = Math.Max(indexMessage.Index, updateOHLC.High);
                                                updateOHLC.Low = Math.Min(indexMessage.Index, updateOHLC.Low);
                                                updateOHLC.Close = indexMessage.Index;
                                                updateOHLC.Change = indexMessage.Index - updateOHLC.Open;
                                                updateOHLC.ChangePercent = Math.Round((updateOHLC.Change / indexMessage.Index) * 100, 2, MidpointRounding.AwayFromZero);

                                                pubsubOHLC = updateOHLC;
                                            }
                                            // Trade message bị cách quãng => tạo ra các OHLC còn thiếu
                                            else
                                            {
                                                // Update OHLC gần nhất về trạng thái đã đóng
                                                updateOHLC.IsKlineClose = true;

                                                // Tạo OHLC mới có giá trị = updateOHLC
                                                var newOHLC = new OHLC
                                                {
                                                    Symbol = indexMessage.Symbol,
                                                    BaseVolume = indexMessage.Volume,
                                                    QuoteVolume = indexMessage.Volume,
                                                    Interval = interval,
                                                    Trades = 1,
                                                    Open = indexMessage.Index,
                                                    High = indexMessage.Index,
                                                    Low = indexMessage.Index,
                                                    Close = indexMessage.Index,
                                                    Change = 0,
                                                    ChangePercent = 0,
                                                };

                                                CalculateTime(ref newOHLC);

                                                // neu ohlc moi tao co thoi gian cach quang so vs ohlc truoc do thi tao moi nhung khoang thoi gian thieu de chen vao
                                                var checkTimeUnix = newOHLC.StartTime - 1;
                                                if (checkTimeUnix > updateOHLC.EndTime)
                                                {
                                                    // lap nguoc ve thoi gian cua ohlc gan nhat theo interval hien tai
                                                    while (checkTimeUnix > updateOHLC.EndTime)
                                                    {
                                                        var newMissingOHLC = new OHLC
                                                        {
                                                            Symbol = indexMessage.Symbol,
                                                            BaseVolume = 0,
                                                            QuoteVolume = 0,
                                                            Interval = interval,
                                                            Trades = 0,
                                                            Open = indexMessage.Index,
                                                            High = indexMessage.Index,
                                                            Low = indexMessage.Index,
                                                            Close = indexMessage.Index,
                                                            Change = 0,
                                                            ChangePercent = 0,
                                                        };

                                                        CalculateMissingOHLC(checkTimeUnix, ref newMissingOHLC);

                                                        checkTimeUnix = newMissingOHLC.StartTime - 1;

                                                        StockOHLCDict[redisKey].AddLast(newMissingOHLC);
                                                        pubsubOHLC = newMissingOHLC;

                                                        // Nếu list > VALUE_RANGE thì xóa bản ghi đầu tiên
                                                        if (StockOHLCDict[redisKey].Count > VALUE_RANGE)
                                                        {
                                                            StockOHLCDict[redisKey].RemoveFirst();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    pubsubOHLC = newOHLC;
                                                    StockOHLCDict[redisKey].AddLast(newOHLC);

                                                    if (StockOHLCDict[redisKey].Count > VALUE_RANGE)
                                                    {
                                                        StockOHLCDict[redisKey].RemoveFirst();
                                                    }
                                                }
                                            }
                                        }

                                        List<string> stringData = new List<string>
                                        {
                                            pubsubOHLC.Symbol.ToString(),
                                            pubsubOHLC.Interval.ToString(),
                                            pubsubOHLC.StartTime.ToString(),
                                            pubsubOHLC.EndTime.ToString(),
                                            pubsubOHLC.Open.ToString(),
                                            pubsubOHLC.High.ToString(),
                                            pubsubOHLC.Low.ToString(),
                                            pubsubOHLC.Close.ToString(),
                                            pubsubOHLC.Change.ToString(),
                                            pubsubOHLC.ChangePercent.ToString(),
                                            pubsubOHLC.BaseVolume.ToString(),
                                            pubsubOHLC.QuoteVolume.ToString(),
                                            pubsubOHLC.Trades.ToString(),
                                            pubsubOHLC.TakerBuyAssetVolume.ToString(),
                                            pubsubOHLC.TakerBuyAssetQuoteVolume.ToString(),
                                            pubsubOHLC.IsKlineClose.ToString().ToLower()
                                        };

                                        var publishValue = String.Join(";", stringData);

                                        // publish data into channel by interval
                                        pubsubTasks.Add(publisher.PublishAsync($"{RedisChannelPrefix.VNSTOCK_CHANNEL}:{pubsubOHLC.Symbol}:{pubsubOHLC.Interval}", publishValue));

                                        // publish data into watchlist channel
                                        if (pubsubOHLC.Interval == Interval.DAY_1)
                                        {
                                            var realtimeData = new MinifiedTradingData(pubsubOHLC);
                                            pubsubTasks.Add(publisher.PublishAsync(RedisChannelPrefix.MINI_STOCK_DATA_CHANNEL, JsonConvert.SerializeObject(realtimeData)));
                                        }
                                    }

                                    break;
                                // custom transaction (gd thoa thuan)
                                case 2112:
                                    var transactionMesage = JsonConvert.DeserializeObject<TransactionMessage>(streamMsg);
                                    break;
                                // Handle order book message (not implement)
                                case 3210:
                                    //var orderBookMsg = JsonConvert.DeserializeObject<OrderBookMessage>(streamMsg);
                                    break;
                                case 3220:
                                    var tradeDataModel = JsonConvert.DeserializeObject<StockTradingMessage>(streamMsg);
                                    if (tradeDataModel == null || String.IsNullOrEmpty(tradeDataModel.Symbol))
                                    {
                                        continue;
                                    }
                                    tradeDataModel.ProcessData();

                                    // Tinh toan du lieu voi moi interval de tao ra OHLC tuong ung
                                    foreach (var interval in Interval.ListInterval)
                                    {
                                        string redisKey = $"{RedisPrefix.STOCK}:{tradeDataModel.Symbol}:{interval}";

                                        // Lưu thông tin OHLC mới nhất dùng để bắn vào kênh pubsub
                                        var pubsubOHLC = new OHLC();
                                        // 18/5/2021: update du lieu tinh toan trong dictionary de tang toc do xu ly
                                        // chi luu data xuong redis
                                        double quoteVolume = tradeDataModel.Quantity * tradeDataModel.Price;

                                        if (!StockOHLCDict.ContainsKey(redisKey))
                                        {
                                            var newOHLC = new OHLC
                                            {
                                                Symbol = tradeDataModel.Symbol,
                                                BaseVolume = tradeDataModel.Quantity,
                                                QuoteVolume = quoteVolume,
                                                Interval = interval,
                                                Trades = 1,
                                                Open = tradeDataModel.Price,
                                                High = Math.Max(tradeDataModel.HighPrice, tradeDataModel.Price), // > 0 ? tradeDataModel.HighPrice : tradeDataModel.Price,
                                                Low = Math.Min(tradeDataModel.LowPrice, tradeDataModel.Price),
                                                Close = tradeDataModel.Price,
                                                Change = 0,
                                                ChangePercent = 0,
                                                TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0,
                                                TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0,
                                            };

                                            CalculateTime(ref newOHLC);

                                            StockOHLCDict[redisKey] = new LinkedList<OHLC>();
                                            StockOHLCDict[redisKey].AddLast(newOHLC);
                                            pubsubOHLC = newOHLC;
                                        }
                                        else
                                        {
                                            // update last OHLC
                                            var updateOHLC = StockOHLCDict[redisKey].Last();

                                            // Kiểm tra message hiện tại với thời gian của ohlc cuối cùng
                                            // Nếu message bị cách quãng, thì cần tạo ra các ohlc bị thiếu với giá trị O H L C bằng nhau chèn vào khoảng thời gian bị thiếu
                                            // Để tạo sự liền mạch khi client generate candles chart

                                            // Trade message nằm trong khoảng thời gian của OHLC hiện tại => update các giá trị
                                            if (tradeDataModel.TradeUnixTime >= updateOHLC.StartTime && tradeDataModel.TradeUnixTime <= updateOHLC.EndTime)
                                            {
                                                updateOHLC.BaseVolume += tradeDataModel.Quantity;
                                                updateOHLC.QuoteVolume += quoteVolume;
                                                updateOHLC.Trades += 1;
                                                updateOHLC.High = Math.Max(tradeDataModel.Price, updateOHLC.High);
                                                updateOHLC.Low = Math.Min(tradeDataModel.Price, updateOHLC.Low);
                                                updateOHLC.Close = tradeDataModel.Price;
                                                updateOHLC.Change = tradeDataModel.Price - updateOHLC.Open;
                                                updateOHLC.ChangePercent = Math.Round((updateOHLC.Change / tradeDataModel.Price) * 100, 2, MidpointRounding.AwayFromZero);
                                                updateOHLC.TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0;
                                                updateOHLC.TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0;

                                                pubsubOHLC = updateOHLC;
                                            }
                                            // Trade message bị cách quãng => tạo ra các OHLC còn thiếu
                                            else
                                            {
                                                // Update OHLC gần nhất về trạng thái đã đóng
                                                updateOHLC.IsKlineClose = true;

                                                // Tạo OHLC mới có giá trị = updateOHLC
                                                var newOHLC = new OHLC
                                                {
                                                    Symbol = tradeDataModel.Symbol,
                                                    BaseVolume = tradeDataModel.Quantity,
                                                    QuoteVolume = quoteVolume,
                                                    Interval = interval,
                                                    Trades = 1,
                                                    Open = tradeDataModel.Price,
                                                    High = tradeDataModel.Price, // > 0 ? tradeDataModel.HighPrice : tradeDataModel.Price,
                                                    Low = tradeDataModel.Price,
                                                    Close = tradeDataModel.Price,
                                                    Change = 0,
                                                    ChangePercent = 0,
                                                    TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0,
                                                    TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0,
                                                };

                                                CalculateTime(ref newOHLC);

                                                // neu ohlc moi tao co thoi gian cach quang so vs ohlc truoc do thi tao moi nhung khoang thoi gian thieu de chen vao
                                                var checkTimeUnix = newOHLC.StartTime - 1;
                                                if (checkTimeUnix > updateOHLC.EndTime)
                                                {
                                                    // lap nguoc ve thoi gian cua ohlc gan nhat theo interval hien tai
                                                    while (checkTimeUnix > updateOHLC.EndTime)
                                                    {
                                                        var newMissingOHLC = new OHLC
                                                        {
                                                            Symbol = tradeDataModel.Symbol,
                                                            BaseVolume = 0,
                                                            QuoteVolume = 0,
                                                            Interval = interval,
                                                            Trades = 0,
                                                            Open = tradeDataModel.Price,
                                                            High = tradeDataModel.Price,
                                                            Low = tradeDataModel.Price,
                                                            Close = tradeDataModel.Price,
                                                            Change = 0,
                                                            ChangePercent = 0,
                                                            TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0,
                                                            TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0,
                                                        };

                                                        CalculateMissingOHLC(checkTimeUnix, ref newMissingOHLC);

                                                        checkTimeUnix = newMissingOHLC.StartTime - 1;

                                                        StockOHLCDict[redisKey].AddLast(newMissingOHLC);
                                                        pubsubOHLC = newMissingOHLC;

                                                        // Nếu list > VALUE_RANGE thì xóa bản ghi đầu tiên
                                                        if (StockOHLCDict[redisKey].Count > VALUE_RANGE)
                                                        {
                                                            StockOHLCDict[redisKey].RemoveFirst();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    pubsubOHLC = newOHLC;
                                                    StockOHLCDict[redisKey].AddLast(newOHLC);

                                                    if (StockOHLCDict[redisKey].Count > VALUE_RANGE)
                                                    {
                                                        StockOHLCDict[redisKey].RemoveFirst();
                                                    }
                                                }
                                            }
                                        }

                                        List<string> stringData = new List<string>
                                        {
                                            pubsubOHLC.Symbol.ToString(),
                                            pubsubOHLC.Interval.ToString(),
                                            pubsubOHLC.StartTime.ToString(),
                                            pubsubOHLC.EndTime.ToString(),
                                            pubsubOHLC.Open.ToString(),
                                            pubsubOHLC.High.ToString(),
                                            pubsubOHLC.Low.ToString(),
                                            pubsubOHLC.Close.ToString(),
                                            pubsubOHLC.Change.ToString(),
                                            pubsubOHLC.ChangePercent.ToString(),
                                            pubsubOHLC.BaseVolume.ToString(),
                                            pubsubOHLC.QuoteVolume.ToString(),
                                            pubsubOHLC.Trades.ToString(),
                                            pubsubOHLC.TakerBuyAssetVolume.ToString(),
                                            pubsubOHLC.TakerBuyAssetQuoteVolume.ToString(),
                                            pubsubOHLC.IsKlineClose.ToString().ToLower()
                                        };

                                        var publishValue = String.Join(";", stringData);

                                        // publish data into channel by interval
                                        pubsubTasks.Add(publisher.PublishAsync($"{RedisChannelPrefix.VNSTOCK_CHANNEL}:{pubsubOHLC.Symbol}:{pubsubOHLC.Interval}", publishValue));

                                        // publish data into watchlist channel
                                        if (pubsubOHLC.Interval == Interval.DAY_1)
                                        {
                                            var realtimeData = new MinifiedTradingData(pubsubOHLC);
                                            pubsubTasks.Add(publisher.PublishAsync(RedisChannelPrefix.MINI_STOCK_DATA_CHANNEL, JsonConvert.SerializeObject(realtimeData)));
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                await Task.WhenAll(pubsubTasks);
                //Console.WriteLine($"Trade Message: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Processing time of {tradeMessages.Count} in miliseconds: {watch.ElapsedMilliseconds}");
                List<Task> addTasks = new List<Task>();

                foreach (var ohlcDict in StockOHLCDict)
                {
                    string key = ohlcDict.Key;

                    foreach (var ohlc in ohlcDict.Value)
                    {
                        long score = ohlc.EndTime;
                        Task<long> removedTask = redisDb.SortedSetRemoveRangeByScoreAsync(key, ohlc.StartTime, double.PositiveInfinity);
                        addTasks.Add(removedTask);

                        List<string> stringData = new List<string>
                        {
                            ohlc.Symbol.ToString(),
                            ohlc.Interval.ToString(),
                            ohlc.StartTime.ToString(),
                            ohlc.EndTime.ToString(),
                            ohlc.Open.ToString(),
                            ohlc.High.ToString(),
                            ohlc.Low.ToString(),
                            ohlc.Close.ToString(),
                            ohlc.Change.ToString(),
                            ohlc.ChangePercent.ToString(),
                            ohlc.BaseVolume.ToString(),
                            ohlc.QuoteVolume.ToString(),
                            ohlc.Trades.ToString(),
                            ohlc.TakerBuyAssetVolume.ToString(),
                            ohlc.TakerBuyAssetQuoteVolume.ToString(),
                            ohlc.IsKlineClose.ToString().ToLower()
                        };

                        var redisValue = String.Join(";", stringData);

                        Task<bool> isSuccess = redisDb.SortedSetAddAsync(key, redisValue, score);
                        addTasks.Add(isSuccess);
                    }
                }

                await Task.WhenAll(addTasks.ToArray());

                //Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Process {tradeMessages.Count} realtime OHLC message!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageQueue.Instance.VNStockRollbackMsg(tradeMessages);
            }
        }

        public void ReBuildStockOHLCData()
        {
            int index = 1;
            var tradingMessages = MongoManager.Instance.ListDailyTradingMessages(index, DateTime.UtcNow);

            while (tradingMessages.Count > 0)
            {
                //await BuildStockOHLCData(tradingMessages);

                foreach (var streamMsg in tradingMessages)
                {
                    try
                    {
                        JObject objectCheck = JObject.Parse(streamMsg);

                        if (objectCheck.ContainsKey("id"))
                        {
                            int groupId = (int)objectCheck["id"];

                            switch (groupId)
                            {
                                // handle indexes message
                                case 1101:
                                    var indexMessage = JsonConvert.DeserializeObject<IndexMessage>(streamMsg);
                                    indexMessage.ProcessData();
                                    if (indexMessage == null || String.IsNullOrEmpty(indexMessage.Symbol))
                                    {
                                        continue;
                                    }

                                    // Tinh toan du lieu voi moi interval de tao ra OHLC tuong ung
                                    foreach (var interval in Interval.ListInterval)
                                    {
                                        string redisKey = $"{RedisPrefix.STOCK}:{indexMessage.Symbol}:{interval}";

                                        // Lưu thông tin OHLC mới nhất dùng để bắn vào kênh pubsub
                                        var pubsubOHLC = new OHLC();

                                        if (!StockOHLCDict.ContainsKey(redisKey))
                                        {
                                            var newOHLC = new OHLC
                                            {
                                                Symbol = indexMessage.Symbol,
                                                BaseVolume = indexMessage.Volume,
                                                QuoteVolume = indexMessage.Volume,
                                                Interval = interval,
                                                Trades = 1,
                                                Open = indexMessage.Index,
                                                High = Math.Max(indexMessage.Index, indexMessage.Open),
                                                Low = Math.Min(indexMessage.Index, indexMessage.Open),
                                                Close = indexMessage.Index,
                                                Change = 0,
                                                ChangePercent = 0,
                                            };

                                            CalculateTime(ref newOHLC);

                                            StockOHLCDict[redisKey] = new LinkedList<OHLC>();
                                            StockOHLCDict[redisKey].AddLast(newOHLC);
                                            pubsubOHLC = newOHLC;
                                        }
                                        else
                                        {
                                            // update last OHLC
                                            var updateOHLC = StockOHLCDict[redisKey].Last();

                                            // Kiểm tra message hiện tại với thời gian của ohlc cuối cùng
                                            // Nếu message bị cách quãng, thì cần tạo ra các ohlc bị thiếu với giá trị O H L C bằng nhau chèn vào khoảng thời gian bị thiếu
                                            // Để tạo sự liền mạch khi client generate candles chart

                                            // Trade message nằm trong khoảng thời gian của OHLC hiện tại => update các giá trị
                                            if (indexMessage.TradeUnixTime >= updateOHLC.StartTime && indexMessage.TradeUnixTime <= updateOHLC.EndTime)
                                            {
                                                updateOHLC.BaseVolume += indexMessage.Volume;
                                                updateOHLC.QuoteVolume += indexMessage.Volume;
                                                updateOHLC.Trades += 1;
                                                updateOHLC.High = Math.Max(indexMessage.Index, updateOHLC.High);
                                                updateOHLC.Low = Math.Min(indexMessage.Index, updateOHLC.Low);
                                                updateOHLC.Close = indexMessage.Index;
                                                updateOHLC.Change = indexMessage.Index - updateOHLC.Open;
                                                updateOHLC.ChangePercent = Math.Round((updateOHLC.Change / indexMessage.Index) * 100, 2, MidpointRounding.AwayFromZero);

                                                pubsubOHLC = updateOHLC;
                                            }
                                            // Trade message bị cách quãng => tạo ra các OHLC còn thiếu
                                            else
                                            {
                                                // Update OHLC gần nhất về trạng thái đã đóng
                                                updateOHLC.IsKlineClose = true;

                                                // Tạo OHLC mới có giá trị = updateOHLC
                                                var newOHLC = new OHLC
                                                {
                                                    Symbol = indexMessage.Symbol,
                                                    BaseVolume = indexMessage.Volume,
                                                    QuoteVolume = indexMessage.Volume,
                                                    Interval = interval,
                                                    Trades = 1,
                                                    Open = indexMessage.Index,
                                                    High = indexMessage.Index,
                                                    Low = indexMessage.Index,
                                                    Close = indexMessage.Index,
                                                    Change = 0,
                                                    ChangePercent = 0,
                                                };

                                                CalculateTime(ref newOHLC);

                                                // neu ohlc moi tao co thoi gian cach quang so vs ohlc truoc do thi tao moi nhung khoang thoi gian thieu de chen vao
                                                var checkTimeUnix = newOHLC.StartTime - 1;
                                                if (checkTimeUnix > updateOHLC.EndTime)
                                                {
                                                    // lap nguoc ve thoi gian cua ohlc gan nhat theo interval hien tai
                                                    while (checkTimeUnix > updateOHLC.EndTime)
                                                    {
                                                        var newMissingOHLC = new OHLC
                                                        {
                                                            Symbol = indexMessage.Symbol,
                                                            BaseVolume = 0,
                                                            QuoteVolume = 0,
                                                            Interval = interval,
                                                            Trades = 0,
                                                            Open = indexMessage.Index,
                                                            High = indexMessage.Index,
                                                            Low = indexMessage.Index,
                                                            Close = indexMessage.Index,
                                                            Change = 0,
                                                            ChangePercent = 0,
                                                        };

                                                        CalculateMissingOHLC(checkTimeUnix, ref newMissingOHLC);

                                                        checkTimeUnix = newMissingOHLC.StartTime - 1;

                                                        StockOHLCDict[redisKey].AddLast(newMissingOHLC);
                                                        pubsubOHLC = newMissingOHLC;

                                                        // Nếu list > VALUE_RANGE thì xóa bản ghi đầu tiên
                                                        if (StockOHLCDict[redisKey].Count > VALUE_RANGE)
                                                        {
                                                            StockOHLCDict[redisKey].RemoveFirst();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    pubsubOHLC = newOHLC;
                                                    StockOHLCDict[redisKey].AddLast(newOHLC);

                                                    if (StockOHLCDict[redisKey].Count > VALUE_RANGE)
                                                    {
                                                        StockOHLCDict[redisKey].RemoveFirst();
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;
                                // custom transaction (gd thoa thuan)
                                case 2112:
                                    var transactionMesage = JsonConvert.DeserializeObject<TransactionMessage>(streamMsg);
                                    break;
                                // Handle order book message (not implement)
                                case 3210:
                                    //var orderBookMsg = JsonConvert.DeserializeObject<OrderBookMessage>(streamMsg);
                                    break;
                                case 3220:
                                    var tradeDataModel = JsonConvert.DeserializeObject<StockTradingMessage>(streamMsg);
                                    if (tradeDataModel == null || String.IsNullOrEmpty(tradeDataModel.Symbol))
                                    {
                                        continue;
                                    }
                                    tradeDataModel.ProcessData();

                                    // Tinh toan du lieu voi moi interval de tao ra OHLC tuong ung
                                    foreach (var interval in Interval.ListInterval)
                                    {
                                        string redisKey = $"{RedisPrefix.STOCK}:{tradeDataModel.Symbol}:{interval}";

                                        // Lưu thông tin OHLC mới nhất dùng để bắn vào kênh pubsub
                                        var pubsubOHLC = new OHLC();
                                        // 18/5/2021: update du lieu tinh toan trong dictionary de tang toc do xu ly
                                        // chi luu data xuong redis
                                        double quoteVolume = tradeDataModel.Quantity * tradeDataModel.Price;

                                        if (!StockOHLCDict.ContainsKey(redisKey))
                                        {
                                            var newOHLC = new OHLC
                                            {
                                                Symbol = tradeDataModel.Symbol,
                                                BaseVolume = tradeDataModel.Quantity,
                                                QuoteVolume = quoteVolume,
                                                Interval = interval,
                                                Trades = 1,
                                                Open = tradeDataModel.Price,
                                                High = Math.Max(tradeDataModel.HighPrice, tradeDataModel.Price), // > 0 ? tradeDataModel.HighPrice : tradeDataModel.Price,
                                                Low = Math.Min(tradeDataModel.LowPrice, tradeDataModel.Price),
                                                Close = tradeDataModel.Price,
                                                Change = 0,
                                                ChangePercent = 0,
                                                TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0,
                                                TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0,
                                            };

                                            CalculateTime(ref newOHLC);

                                            StockOHLCDict[redisKey] = new LinkedList<OHLC>();
                                            StockOHLCDict[redisKey].AddLast(newOHLC);
                                            pubsubOHLC = newOHLC;
                                        }
                                        else
                                        {
                                            // update last OHLC
                                            var updateOHLC = StockOHLCDict[redisKey].Last();

                                            // Kiểm tra message hiện tại với thời gian của ohlc cuối cùng
                                            // Nếu message bị cách quãng, thì cần tạo ra các ohlc bị thiếu với giá trị O H L C bằng nhau chèn vào khoảng thời gian bị thiếu
                                            // Để tạo sự liền mạch khi client generate candles chart

                                            // Trade message nằm trong khoảng thời gian của OHLC hiện tại => update các giá trị
                                            if (tradeDataModel.TradeUnixTime >= updateOHLC.StartTime && tradeDataModel.TradeUnixTime <= updateOHLC.EndTime)
                                            {
                                                updateOHLC.BaseVolume += tradeDataModel.Quantity;
                                                updateOHLC.QuoteVolume += quoteVolume;
                                                updateOHLC.Trades += 1;
                                                updateOHLC.High = Math.Max(tradeDataModel.Price, updateOHLC.High);
                                                updateOHLC.Low = Math.Min(tradeDataModel.Price, updateOHLC.Low);
                                                updateOHLC.Close = tradeDataModel.Price;
                                                updateOHLC.Change = tradeDataModel.Price - updateOHLC.Open;
                                                updateOHLC.ChangePercent = Math.Round((updateOHLC.Change / tradeDataModel.Price) * 100, 2, MidpointRounding.AwayFromZero);
                                                updateOHLC.TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0;
                                                updateOHLC.TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0;

                                                pubsubOHLC = updateOHLC;
                                            }
                                            // Trade message bị cách quãng => tạo ra các OHLC còn thiếu
                                            else
                                            {
                                                // Update OHLC gần nhất về trạng thái đã đóng
                                                updateOHLC.IsKlineClose = true;

                                                // Tạo OHLC mới có giá trị = updateOHLC
                                                var newOHLC = new OHLC
                                                {
                                                    Symbol = tradeDataModel.Symbol,
                                                    BaseVolume = tradeDataModel.Quantity,
                                                    QuoteVolume = quoteVolume,
                                                    Interval = interval,
                                                    Trades = 1,
                                                    Open = tradeDataModel.Price,
                                                    High = tradeDataModel.Price, // > 0 ? tradeDataModel.HighPrice : tradeDataModel.Price,
                                                    Low = tradeDataModel.Price,
                                                    Close = tradeDataModel.Price,
                                                    Change = 0,
                                                    ChangePercent = 0,
                                                    TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0,
                                                    TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0,
                                                };

                                                CalculateTime(ref newOHLC);

                                                // neu ohlc moi tao co thoi gian cach quang so vs ohlc truoc do thi tao moi nhung khoang thoi gian thieu de chen vao
                                                var checkTimeUnix = newOHLC.StartTime - 1;
                                                if (checkTimeUnix > updateOHLC.EndTime)
                                                {
                                                    // lap nguoc ve thoi gian cua ohlc gan nhat theo interval hien tai
                                                    while (checkTimeUnix > updateOHLC.EndTime)
                                                    {
                                                        var newMissingOHLC = new OHLC
                                                        {
                                                            Symbol = tradeDataModel.Symbol,
                                                            BaseVolume = 0,
                                                            QuoteVolume = 0,
                                                            Interval = interval,
                                                            Trades = 0,
                                                            Open = tradeDataModel.Price,
                                                            High = tradeDataModel.Price,
                                                            Low = tradeDataModel.Price,
                                                            Close = tradeDataModel.Price,
                                                            Change = 0,
                                                            ChangePercent = 0,
                                                            TakerBuyAssetVolume = tradeDataModel.IsBuyerMarketMaker ? tradeDataModel.Quantity : 0,
                                                            TakerBuyAssetQuoteVolume = tradeDataModel.IsBuyerMarketMaker ? quoteVolume : 0,
                                                        };

                                                        CalculateMissingOHLC(checkTimeUnix, ref newMissingOHLC);

                                                        checkTimeUnix = newMissingOHLC.StartTime - 1;

                                                        StockOHLCDict[redisKey].AddLast(newMissingOHLC);
                                                        pubsubOHLC = newMissingOHLC;

                                                        // Nếu list > VALUE_RANGE thì xóa bản ghi đầu tiên
                                                        if (StockOHLCDict[redisKey].Count > VALUE_RANGE)
                                                        {
                                                            StockOHLCDict[redisKey].RemoveFirst();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    pubsubOHLC = newOHLC;
                                                    StockOHLCDict[redisKey].AddLast(newOHLC);

                                                    if (StockOHLCDict[redisKey].Count > VALUE_RANGE)
                                                    {
                                                        StockOHLCDict[redisKey].RemoveFirst();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                foreach (var ohlcDict in StockOHLCDict)
                {
                    string key = ohlcDict.Key;

                    foreach (var ohlc in ohlcDict.Value)
                    {
                        long score = ohlc.EndTime;
                        redisDb.SortedSetRemoveRangeByScore(key, ohlc.StartTime, double.PositiveInfinity);

                        List<string> stringData = new List<string>
                        {
                            ohlc.Symbol.ToString(),
                            ohlc.Interval.ToString(),
                            ohlc.StartTime.ToString(),
                            ohlc.EndTime.ToString(),
                            ohlc.Open.ToString(),
                            ohlc.High.ToString(),
                            ohlc.Low.ToString(),
                            ohlc.Close.ToString(),
                            ohlc.Change.ToString(),
                            ohlc.ChangePercent.ToString(),
                            ohlc.BaseVolume.ToString(),
                            ohlc.QuoteVolume.ToString(),
                            ohlc.Trades.ToString(),
                            ohlc.TakerBuyAssetVolume.ToString(),
                            ohlc.TakerBuyAssetQuoteVolume.ToString(),
                            ohlc.IsKlineClose.ToString().ToLower()
                        };

                        var redisValue = String.Join(";", stringData);

                        redisDb.SortedSetAdd(key, redisValue, score);
                    }
                }

                Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} Process {tradingMessages.Count} realtime OHLC message!");
                index++;
                tradingMessages = MongoManager.Instance.ListDailyTradingMessages(index, DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Tinh start_time, end_time cua 1 candle theo interval
        /// </summary>
        /// <param name="data"></param>
        private void CalculateTime(ref OHLC data)
        {
            var utcNow = DateTime.UtcNow;

            // tinh toan so ticks trong 1 don vi interval
            var intervalInMinute = Interval.GetIntervalInMinute(data.Interval);
            var minuteSize = (intervalInMinute * TimeSpan.TicksPerMinute);
            var currentIndexByTick = utcNow.TimeOfDay.Ticks / minuteSize;

            // thoi gian bat dau cua 1 nen OHLC
            DateTime startTime = utcNow.Date.AddTicks(currentIndexByTick * minuteSize);

            if (data.Interval == Interval.WEEK_1)
            {
                // thu high la ngay dau tuan
                var monday = DatetimeHelper.StartOfWeek(utcNow, DayOfWeek.Monday);

                // Voi nen tuan thi start_time la tgian bat dau ngay thu high
                startTime = monday.Date.AddTicks(currentIndexByTick * minuteSize);
            }
            else if (data.Interval == Interval.MONTH_1)
            {
                // ngay dau tien cua thang
                var firstDayInMonth = new DateTime(utcNow.Year, utcNow.Month, 1);

                // Voi nen thang thi start_time la tgian bat dau cua ngay dau tien trong thang
                startTime = firstDayInMonth.Date.AddTicks(currentIndexByTick * minuteSize);
            }

            DateTime endTime = startTime.AddMinutes(intervalInMinute).AddMilliseconds(-1);

            //var vnEndTime = DatetimeHelper.ConvertToVNTime(endTime);
            //var vnStartTime = DatetimeHelper.ConvertToVNTime(startTime);

            data.StartTime = ((DateTimeOffset)startTime).ToUnixTimeMilliseconds();
            data.EndTime = ((DateTimeOffset)endTime).ToUnixTimeMilliseconds();
        }

        private void CalculateMissingOHLC(long missingEndTime, ref OHLC data)
        {
            DateTime refDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime utcEndTime = refDateTime.AddMilliseconds(missingEndTime).ToUniversalTime();

            //var endTime = DatetimeHelper.ConvertToVNTime(utcDate: utcEndTime);

            DateTime startTime = new DateTime(utcEndTime.Year, utcEndTime.Month, utcEndTime.Day, utcEndTime.Hour, utcEndTime.Minute, 0);
            DateTime endTime = startTime.AddMinutes(1).AddTicks(-1);

            data.StartTime = ((DateTimeOffset)startTime).ToUnixTimeMilliseconds();
            data.EndTime = ((DateTimeOffset)endTime).ToUnixTimeMilliseconds();
            data.IsKlineClose = true;
        }

    }
}

