using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace worker_netcore_crawl.Model
{
    /// <summary>
    /// Class offer three price off order book
    /// </summary>
    public class OrderBookMessage
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sym")]
        public string Symbol { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("timeServer")]
        public string ServerTime { get; set; }

        [JsonProperty("g1")]
        public string PriceOne { get; set; }

        [JsonProperty("g2")]
        public string PriceTwo { get; set; }

        [JsonProperty("g3")]
        public string PriceThree { get; set; }

        /// <summary>
        /// Trade time
        /// </summary>
        public long MessageUnixTime { get; set; }

        public List<OrderBookItem> OrderBookItems { get; set; }

        public void ProcessData()
        {
            if (!String.IsNullOrEmpty(ServerTime))
            {
                var timeStamp = TimeSpan.ParseExact(ServerTime, "hh\\:mm\\:ss", CultureInfo.InvariantCulture);

                DateTime dateNow = DateTime.Now.Date + timeStamp;
                MessageUnixTime = ((DateTimeOffset)dateNow).ToUnixTimeMilliseconds();
            }

            OrderBookItems = new List<OrderBookItem>();

            var priceOneSplit = PriceOne.Split('|');
            var p1Price = double.Parse(priceOneSplit[0]);
            var p1Vol = double.Parse(priceOneSplit[1]);
            var p1Status = priceOneSplit[2];

            OrderBookItems.Add(new OrderBookItem
            {
                Price = p1Price,
                Volume = p1Vol,
                Status = p1Status
            });

            var priceTwoSplit = PriceTwo.Split('|');
            var p2Price = double.Parse(priceTwoSplit[0]);
            var p2Vol = double.Parse(priceTwoSplit[1]);
            var p2Status = priceTwoSplit[2];

            OrderBookItems.Add(new OrderBookItem
            {
                Price = p2Price,
                Volume = p2Vol,
                Status = p2Status
            });

            var priceThreeSplit = PriceThree.Split('|');
            var p3Price = double.Parse(priceThreeSplit[0]);
            var p3Vol = double.Parse(priceThreeSplit[1]);
            var p3Status = priceThreeSplit[2];

            OrderBookItems.Add(new OrderBookItem
            {
                Price = p3Price,
                Volume = p3Vol,
                Status = p3Status
            });
        }
    }

    public class OrderBookItem
    {
        public double Price { get; set; }

        public double Volume { get; set; }

        /// <summary>
        /// i: increase
        /// d: decrease
        /// other
        /// </summary>
        public string Status { get; set; }
    }
}

// sample data
//{
//    "id": 3210,
//        "sym": "ACB",
//        "side": "S",
//        "g1": "36.15|21350|i",
//        "g2": "36.20|37640|i",
//        "g3": "36.25|21390|i",
//        "vol4": 0,
//        "timeServer": "09:23:47"
//      }