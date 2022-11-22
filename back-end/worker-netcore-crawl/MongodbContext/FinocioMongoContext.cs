using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using worker_netcore_crawl.Model;

namespace worker_netcore_crawl.MongodbContext
{
    public class FinocioMongoContext : IFinocioMongoContext
    {
        private IMongoDatabase _db { get; set; }
        private IMongoClient _mongoClient { get; set; }
        public IClientSessionHandle Session { get; set; }
        public FinocioMongoContext(IOptions<AppSettings> configuration)
        {
            _mongoClient = new MongoClient(configuration.Value.MongoConnection);

            _db = _mongoClient.GetDatabase(configuration.Value.MongoDbName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return _db.GetCollection<T>(name);
        }
    }
}
