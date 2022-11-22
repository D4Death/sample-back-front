using System;
using sample_api.Entity;
using Newtonsoft.Json;

namespace sample_api.Model
{
    public class MarketSearchResponse
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        [JsonProperty("logo_data")]
        public string LogoData { get; set; }

        public MarketSearchResponse(CompanyInfo comp) {
            Symbol = comp.Code;
            Name = comp.ShortName;
            LogoData = "";
        }
    }
}
