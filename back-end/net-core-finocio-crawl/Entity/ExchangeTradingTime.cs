using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net_core_sample_crawl.Entity
{
    public class ExchangeTradingTime
    {
        public int Id { get; set; }
        public string ExchangeCode { get; set; }
        public int TradingTimeId { get; set; }
    }
}
