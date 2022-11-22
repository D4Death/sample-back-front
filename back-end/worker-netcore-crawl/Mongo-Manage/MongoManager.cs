using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using worker_netcore_crawl.Model;
using worker_netcore_crawl.MongoCollection;
using worker_netcore_crawl.MongodbContext;

namespace worker_netcore_crawl.MongoManage
{
    public class MongoManager
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _db;
        //protected IMongoCollection<MiniTradingMessage> _miniTradingMessagesCollection;
        protected IMongoCollection<DailyTradingMessage> _dailyTradingMessagesCollection;
        protected IMongoCollection<CryptoMiniTradingMessage> _miniCryptoTradingMessagesCollection;
        protected IMongoCollection<SymbolOverView> _symbolOverViewCollection;
        public MongoManager()
        {
            _mongoClient = new MongoClient(AppSettings.Instance.MongoConnection);
            //_mongoClient = new MongoClient("mongodb://finoMongo:fInocIo2021@127.0.0.1:27017");

            _db = _mongoClient.GetDatabase(AppSettings.Instance.MongoDbName);
            //_miniTradingMessagesCollection = _db.GetCollection<MiniTradingMessage>("mini_intraday_message");
            _dailyTradingMessagesCollection = _db.GetCollection<DailyTradingMessage>("daily_intraday_message");
            _miniCryptoTradingMessagesCollection = _db.GetCollection<CryptoMiniTradingMessage>("mini_crypto_trading_message");
            _symbolOverViewCollection = _db.GetCollection<SymbolOverView>("symbol_overview");
        }

        private static readonly MongoManager m_MongoManager = new MongoManager();

        public static MongoManager Instance
        {
            get
            {
                return m_MongoManager;
            }
        }

        //public async Task InsertManyIntradayMessage(List<string> tradeMessages)
        //{
        //    try
        //    {
        //        var listIntraday = new List<MiniTradingMessage>();

        //        var dateNow = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
        //        if (tradeMessages != null && tradeMessages.Any())
        //        {
        //            foreach (var streamMsg in tradeMessages)
        //            {
        //                JObject objectCheck = JObject.Parse(streamMsg);
        //                if (objectCheck.ContainsKey("id"))
        //                {
        //                    int groupId = (int)objectCheck["id"];
        //                    if (groupId == 3220 || groupId == 1101)
        //                    {
        //                        listIntraday.Add(new MiniTradingMessage { OnDate = dateNow, GroupId = groupId, MessageData = streamMsg });
        //                    }
        //                }
        //            }

        //            if(listIntraday.Count > 0)
        //                await _miniTradingMessagesCollection.InsertManyAsync(listIntraday);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"InsertManyIntradayMessage: {ex.ToString()}");
        //    }
        //}

        public async Task InsertManyDailyMessage(List<string> tradeMessages)
        {
            try
            {
                var listDailyMessage = new List<DailyTradingMessage>();
                List<Task> insertTasks = new List<Task>();

                var dateNow = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
                if (tradeMessages != null && tradeMessages.Any())
                {
                    foreach (var streamMsg in tradeMessages)
                    {
                        JObject objectCheck = JObject.Parse(streamMsg);
                        if (objectCheck.ContainsKey("id"))
                        {
                            int groupId = (int)objectCheck["id"];
                            if (groupId == 3220 || groupId == 1101)
                            {
                                listDailyMessage.Add(new DailyTradingMessage { OnDate = dateNow, GroupId = groupId, MessageData = streamMsg });
                            }
                        }
                    }

                    if (listDailyMessage.Count > 0)
                    {
                        insertTasks.Add(_dailyTradingMessagesCollection.InsertManyAsync(listDailyMessage));
                    }
                }

                await Task.WhenAll(insertTasks.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InsertManyIntradayMessage: {ex.ToString()}");
            }
        }

        public async Task InsertManyCryptoTradingMessage(List<string> tradeMessages)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                var listTradingMessage = new List<CryptoMiniTradingMessage>();
                List<Task> insertTasks = new List<Task>();

                var dateNow = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
                if (tradeMessages != null && tradeMessages.Any())
                {
                    foreach (var streamMsg in tradeMessages)
                    {
                        listTradingMessage.Add(new CryptoMiniTradingMessage { OnDate = dateNow, MessageData = streamMsg });
                    }

                    if(listTradingMessage.Count > 0)
                        insertTasks.Add(_miniCryptoTradingMessagesCollection.InsertManyAsync(listTradingMessage));
                }

                await Task.WhenAll(insertTasks.ToArray());

                watch.Stop();
                Console.WriteLine($"InsertManyCryptoTradingMessage save to mongo: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Processing time of {tradeMessages.Count} in miliseconds: {watch.ElapsedMilliseconds}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InsertManyCryptoTradingMessage: {ex.ToString()}");
            }
        }

        public async Task<List<CryptoMiniTradingMessage>> AllCryptoTradingMessages()
        {
            try
            {
                return await _miniCryptoTradingMessagesCollection.Find(_ => true).Skip(0).Limit(1000000).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public async Task DeleteCryptoMessages(List<CryptoMiniTradingMessage> list)
        {
            try
            {
                var filteredIds = list.Select(x => x.Id).ToArray();
                var mongoFilter = Builders<CryptoMiniTradingMessage>.Filter.In(d => d.Id, filteredIds);
                await _miniCryptoTradingMessagesCollection.DeleteManyAsync(mongoFilter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Task.FromException(ex);
            }
        }

        //public async Task<List<MiniTradingMessage>> AllStockTradingMessages()
        //{
        //    return await _miniTradingMessagesCollection.Find(_ => true).ToListAsync();
        //}

        public List<string> ListDailyTradingMessages(int currentIndex, DateTime? onDate = null) {
            int pageSize = 1000;
            var datas = new List<string>();

            try
            {
                if (onDate == null || onDate.Value.Date == DateTime.MinValue.Date || onDate.Value.Date == DateTime.MaxValue.Date)
                {
                    onDate = DateTime.UtcNow;
                }

                var startUnixTime = ((DateTimeOffset)onDate.Value.Date).ToUnixTimeMilliseconds();
                var endUnixTime = ((DateTimeOffset)onDate.Value.AddDays(1).AddMinutes(-1)).ToUnixTimeMilliseconds();

                var result = _dailyTradingMessagesCollection.Find(x => x.OnDate >= startUnixTime && x.OnDate <= endUnixTime)
                                                       .Sort(Builders<DailyTradingMessage>.Sort.Ascending(x => x.OnDate))
                                                       .Skip(currentIndex * pageSize)
                                                       .Limit(pageSize)
                                                       .ToList();

                datas = result.Select(x => x.MessageData).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return datas;
        }
    }
}
