using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using worker_netcore_crawl.Model;
using worker_netcore_crawl.MongoManage;
using worker_netcore_crawl.Utilities;

namespace worker_netcore_crawl
{
    /// <summary>
    /// Class implement EOD job
    /// </summary>
    public class DailyEODService : IHostedService, IDisposable
    {
        private readonly ILogger<DailyEODService> _logger;
        private Timer _timer;
        public DailyEODService(ILogger<DailyEODService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Timed Background Service is starting.");
                
                _timer = new Timer(DoWork, null, TimeSpan.Zero,
                    TimeSpan.FromHours(1));

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Task.FromException(ex);
            }
            
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            var vnDateNow = DatetimeHelper.GetVietNamDateNow();

            DateTime cleanDbTime = new DateTime(vnDateNow.Year, vnDateNow.Month, vnDateNow.Day, AppSettings.Instance.CleanEOD, 0, 0);
            ZipData();
            if (vnDateNow >= cleanDbTime)
            {
                //Stopwatch watch = new Stopwatch();
                //watch.Start();
                try
                {

                    ZipData();
                    //watch.Stop();
                    //Console.WriteLine($"Query mongo {tradeMessages.Count} records using: {watch.ElapsedMilliseconds} miliseconds.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }

        private void ZipData()
        {
            _logger.LogInformation("Start Zip data file.");

            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                DateTime utcNow = DateTime.UtcNow;
                int bufferSize = 81920; // 80K
                using (FileStream originalFileStream = File.OpenRead("/Volumes/Macintosh/mongo-daily-data/mini_crypto_trading_message15_8_2021.json"))
                {
                    using (FileStream compressedFileStream = File.Create($"/Volumes/Macintosh/mongo-daily-data/daily_trading_{utcNow.Date.ToString("yyyyMMdd")}.gz"))
                    {
                        //using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionLevel.Optimal, false))
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream, bufferSize);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            watch.Stop();
            Console.WriteLine($"ZipData using: {watch.ElapsedMilliseconds} miliseconds.");
        }

        private static void UnZipData()
        {
            int bufferSize = 81920; // 80K

            using (FileStream compressedFileStream = File.OpenRead("fake.gz"))
            {
                using (FileStream outFileStream = File.OpenWrite("fake_out.txt"))
                {
                    using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Decompress))
                    {
                        compressionStream.CopyTo(outFileStream, bufferSize);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
