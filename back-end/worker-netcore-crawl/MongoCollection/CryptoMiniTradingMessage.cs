using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace worker_netcore_crawl.MongoCollection
{
    public class CryptoMiniTradingMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("on_date")]
        public long OnDate { get; set; }

        [BsonElement("message_data")]
        public string MessageData { get; set; }
    }
}
