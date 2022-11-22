using System;
using Newtonsoft.Json;

namespace sample_api6.Model
{
    public class UpdateWatchlistRequest
    {
        [JsonProperty("portfolioId")]
        public long PortfolioId { get; set; }

        [JsonProperty("watchlist")]
        public string Watchlist { get; set; }

        public string Name { get; set; }
    }
}
