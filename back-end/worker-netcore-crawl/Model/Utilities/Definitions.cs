using System;
using System.Collections.Generic;
using System.Text;

namespace worker_netcore_crawl.Model
{
    public class Definitions
    {
        public static string MINI_DATA = "mini_data";
        public static string VNINDEX_CODE = "VNINDEX";
        public static string HNX_CODE = "HNX";
        public static string UPCOM_CODE = "UPCOM";
        public static string VN30_CODE = "VN30";

        public static Dictionary<string, string> IndexDictionary = new Dictionary<string, string> {
            {"10", VNINDEX_CODE },
            {"02", HNX_CODE },
            {"03", UPCOM_CODE },
            {"11", VN30_CODE },
        };
    }

    public enum IndexCode
    {
        vnindex = 10,
        hnxindex = 02,
        upcomindex = 03,
        vn30index = 11
    }

    public class FakeStockSymbol
    {
        public static FakeSymbol GetStockSymbol(string inputCrypto)
        {
            return inputCrypto switch
            {
                "BTCUSDT" => new FakeSymbol { Symbol = "HDB", Rate = 1, VolumeRate = 1000000 },
                "ETHUSDT" => new FakeSymbol { Symbol = "VPB", Rate = 10, VolumeRate = 1000 },
                "ETCUSDT" => new FakeSymbol { Symbol = "TCB", Rate = 1000, VolumeRate = 100 },
                "LINKUSDT" => new FakeSymbol { Symbol = "VHM", Rate = 1000, VolumeRate = 100 },
                "NEOUSDT" => new FakeSymbol { Symbol = "HPG", Rate = 1000, VolumeRate = 100 },
                "HOTUSDT" => new FakeSymbol { Symbol = "VNM", Rate = 1000, VolumeRate = 100 },
                "DOTUSDT" => new FakeSymbol { Symbol = "HSG", Rate = 1000, VolumeRate = 100 },
                "ADAUSDT" => new FakeSymbol { Symbol = "NVL", Rate = 1000, VolumeRate = 100 },
                "BNBUSDT" => new FakeSymbol { Symbol = "VRE", Rate = 100, VolumeRate = 1000 },
                _ => new FakeSymbol { Symbol = inputCrypto.ToUpper(), Rate = 1 },
            };
        }
    }

    public class FakeSymbol {
        public string Symbol { get; set; }
        public int Rate { get; set; }
        public int VolumeRate { get; set; }
    }
}
