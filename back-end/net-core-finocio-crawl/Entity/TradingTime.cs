using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net_core_sample_crawl.Entity
{
    /// <summary>
    /// Thời gian giao dịch
    /// </summary>
    public class TradingTime
    {
        public int Id { get; set; }
        public TimeSpan MorningStart { get; set; }
        public TimeSpan MorningEnd { get; set; }
        public TimeSpan AfternoonStart { get; set; }
        public TimeSpan AfternoonEnd { get; set; }
        public TimeSpan ATOStart { get; set; }
        public TimeSpan ATOEnd { get; set; }
        public TimeSpan ATCStart { get; set; }
        public TimeSpan ATCEnd { get; set; }
    }
}
