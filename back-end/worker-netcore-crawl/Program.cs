using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using worker_netcore_crawl.Context;
//using Microsoft.AspNetCore.Hosting;
using worker_netcore_crawl.Model;

namespace worker_netcore_crawl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();

            CreateDbIfNotExists(host);

            //StackExchangeRedis.Instance.ReBuildStockOHLCData();

            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var context = services.GetRequiredService<StockDbContext>();
                    context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<StockDbContext>(options => options.UseNpgsql(AppSettings.Instance.HostConnection));
                    //            services.Configure<IFinoMongoSettings>(
                    //Configuration.GetSection(nameof(FinoMongoSettings)));
                    //            services.AddSingleton<IFinoMongoSettings>(x => x.GetRequiredService<IOptions<FinoMongoSettings>>().Value);
                    services.AddHostedService<SocketIOCrawlService>();
                    services.AddHostedService<StockRealtimeDataBuilder>();
                    //services.AddHostedService<BinanceWebsocketCrawlService>();
                    //services.AddHostedService<BinanceRealtimeDataBuilder>();
                    //services.AddHostedService<DailyEODService>();
                })
            .UseConsoleLifetime();
    }
}
