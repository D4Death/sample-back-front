using Microsoft.Extensions.Configuration;
using System;
namespace worker_netcore_crawl.Model
{
    public class AppSettings
    {
        public string LocalConnection { get; set; }
        public string HostConnection { get; set; }
        public string CrawlUrl { get; set; }
        public string MongoConnection { get; set; }
        public string MongoDbName { get; set; }
        public string RedisConnection { get; set; }
        public int CleanEOD { get; set; }
        public static string JwtKey { get; set; }

        private static readonly AppSettings m_AppSettings = new AppSettings();

        public static AppSettings Instance
        {
            get
            {
                return m_AppSettings;
            }
        }

        private AppSettings()
        {
            InitAppConfig();
        }

        private void InitAppConfig()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true);

            //var builder = new ConfigurationBuilder()
            //    .AddJsonFile($"appsettings.Development.json", true, true);

            var config = builder.Build();

            LocalConnection = config["ConnectionStrings:HostConnection"];
            HostConnection = config["ConnectionStrings:HostConnection"];
            CrawlUrl = config["ApiCrawl:FireAnt"];
            MongoConnection = config["MongoSettings:ConnectionString"];
            MongoDbName = config["MongoSettings:DatabaseName"];
            CleanEOD = int.Parse(config["MongoSettings:CleanEOD"]);
            RedisConnection = config["RedisSettings:ConnectionString"];
        }
    }
}
