using System;
namespace net_core_finocio_crawl.Model
{
    public class AppSettings
    {
        public static ConnectionStrings ConnectionStrings { get; set; }
        public static WebApiServer WebApiServer { get; set; }
        public static string JwtKey { get; set; }
    }
}
