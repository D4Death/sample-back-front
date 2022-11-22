using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace worker_netcore_crawl.MongoCollection
{
    /// <summary>
    /// mongo collection collect daily data, clear end of day
    /// </summary>
    public class DailyTradingMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("group_id")]
        public int GroupId { get; set; }

        [BsonElement("on_date")]
        public long OnDate { get; set; }

        [BsonElement("message_data")]
        public string MessageData { get; set; }
        //DYTnnXdG2uzcmiEQV30JceyBFzXpCYazBCi0JbDdstTvijkpHJGYlez8JG9TsHIK
    }
}
