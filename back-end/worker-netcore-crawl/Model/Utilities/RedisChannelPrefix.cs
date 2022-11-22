using System;
using System.Collections.Generic;
using System.Text;

namespace worker_netcore_crawl.Model
{
    public class RedisChannelPrefix
    {
        public static string BINANCE_CHANNEL = "crypto:binance";

        public static string VNSTOCK_CHANNEL = "stock:vn";

        public static string MINI_CRYPTO_DATA_CHANNEL = "crypto:mini_data";

        public static string MINI_STOCK_DATA_CHANNEL = "stock:mini_data";
    }
}
