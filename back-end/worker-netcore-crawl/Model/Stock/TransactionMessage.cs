using System;
using System.Globalization;
using Newtonsoft.Json;

namespace worker_netcore_crawl.Model
{
    public class TransactionMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("sym")]
        public string Symbol { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }

        public TimeSpan TimeStamp { get; set; }

        public void ProcessData()
        {
            string[] allowedFormats = { "hh\\:mm\\:ss\\:ffffff", "hh\\:mm\\:ss" };
            TimeStamp = TimeSpan.ParseExact(Time, allowedFormats, CultureInfo.InvariantCulture);
        }
    }
}
