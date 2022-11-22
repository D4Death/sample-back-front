using System;
using MongoDB.Driver;
using worker_netcore_crawl.MongoCollection;

namespace worker_netcore_crawl.MongodbContext
{
    public interface IFinocioMongoContext
    {
        IMongoCollection<MiniTradingMessage> GetCollection<MiniTradingMessage>(string name);
    }
}
