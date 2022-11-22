using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using sample_ultilities;
using net_core_sample_crawl.Context;
using net_core_sample_crawl.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace net_core_sample_crawl.Repository
{
    public class CrawlRepo : ICrawlRepo
    {
        private readonly StockDbContext _context;

        public CrawlRepo(StockDbContext context)
        {
            _context = context;
        }

        public async Task AddCompanyByCode(string compCode)
        {
            try
            {
                var existsComp = _context.Companies.Any(x => x.Code == compCode);
                if (!existsComp)
                {
                    Console.WriteLine($"Start FetchCompanyInfo {compCode}");

                    //await _context.AddAsync(new CompanyInfo { Code = compCode });
                    //await _context.SaveChangesAsync();

                    HttpClient http = new HttpClient();
                    var fetchDataUri = $"{AppSettings.WebApiServer.CrawlUrl}/Companies/CompanyInfo?symbol={compCode}";
                    var fetchDataResponse = await http.GetAsync(fetchDataUri);

                    var listComp = new List<CompanyInfo>();

                    if (fetchDataResponse.StatusCode == HttpStatusCode.OK)
                    {
                        string responseData = await fetchDataResponse.Content.ReadAsStringAsync();
                        var comp = JsonConvert.DeserializeObject<CompanyInfo>(responseData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        listComp.Add(comp);

                        Console.WriteLine($"listComp Add {compCode}");
                    }

                    if (listComp.Any())
                    {
                        await _context.AddAsync(listComp);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"FetchCompanyInfo {compCode} done!!!!!!!!!!!!");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"AddCompanyByCode: {e.ToString()}");
            }
        }

        public async Task<Result> AddStockExchange()
        {
            try
            {
                _context.Database.EnsureCreated();

                if (!_context.Exchanges.Any(x => x.Code == "HSX"))
                {
                    _context.Exchanges.Add(new Exchange
                    {
                        Code = "HSX",
                        Name = "Ho Chi Minh Stock Exchange",
                        Location = "Viet Name",
                        TimeZone = "ICT",
                        Phone = "(84-8)-38217713",
                        Address = "16 Võ Văn Kiệt, Q.1, TP.HCM",
                        Email = "hotline@hsx.vn",
                        Fax = "(84-8)-38217452",
                        Website = "https://www.hsx.vn"
                    });
                }

                if (!_context.Exchanges.Any(x => x.Code == "HNX"))
                {
                    _context.Exchanges.Add(new Exchange
                    {
                        Code = "HNX",
                        Name = "Hanoi Stock Exchange",
                        Location = "Viet Name",
                        TimeZone = "ICT",
                        Phone = "(84-4)-39412626",
                        Address = "Số 02, Phan Chu Trinh, Hoàn Kiếm, Hà Nội",
                        Email = "hnx@hnx.vn",
                        Fax = "(84.4)-39347818",
                        Website = "https://www.hnx.vn"
                    });
                }

                if (!_context.Exchanges.Any(x => x.Code == "UPCOM"))
                {
                    _context.Exchanges.Add(new Exchange
                    {
                        Code = "UPCOM",
                        Name = "Unlisted Public Company Market",
                        Location = "Viet Name",
                        TimeZone = "ICT",
                    });
                }

                if (!_context.Exchanges.Any(x => x.Code == "OTC"))
                {
                    _context.Exchanges.Add(new Exchange
                    {
                        Code = "OTC",
                        Name = "Over the counter",
                        Location = "Viet Name",
                        TimeZone = "ICT",
                    });
                }

                await _context.SaveChangesAsync();

                return new Result();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new Result
                {
                    Code = ResponseCode.SYS_GENERIC_ERROR,
                    Message = e.Message
                };
            }
        }

        public async Task<Result> FetchCompanyInfo()
        {
            try
            {
                var companyDataPath = Directory.GetCurrentDirectory() + "/Data/company-data.json";

                using StreamReader reader = File.OpenText(companyDataPath);
                var listData = JToken.ReadFrom(new JsonTextReader(reader)).ToArray();

                var listSymbolCode = listData.Select(x => x["c"].ToString()).ToList();

                int i = 0;
                var listComp = new List<CompanyInfo>();

                foreach (var compCode in listSymbolCode)
                {
                    var existsComp = _context.Companies.Any(x => x.Code == compCode);
                    if (!existsComp)
                    {
                        Console.WriteLine($"Start FetchCompanyInfo {compCode}");
                        HttpClient http = new HttpClient();
                        var fetchDataUri = $"{AppSettings.WebApiServer.CrawlUrl}/Companies/CompanyInfo?symbol={compCode}";
                        var fetchDataResponse = await http.GetAsync(fetchDataUri);

                        if (fetchDataResponse.StatusCode == HttpStatusCode.OK)
                        {
                            string responseData = await fetchDataResponse.Content.ReadAsStringAsync();
                            var comp = JsonConvert.DeserializeObject<CompanyInfo>(responseData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                            listComp.Add(comp);
                            i++;
                            Console.WriteLine($"listComp add {compCode}");
                        }
                    }
                }

                if (listComp.Any())
                {
                    await _context.AddRangeAsync(listComp);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"FetchCompanyInfo Count: {listComp.Count}");
                    Console.WriteLine($"FetchCompanyInfo done!!!!!!!!!!!!");
                }

                return new Result();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new Result {
                    Code = ResponseCode.SYS_GENERIC_ERROR,
                    Message = e.Message
                };
            }
        }

        public async Task<Result> FetchLastestFinancialSymbolInfo()
        {
            try
            {
                var fetchDataUri = $"{AppSettings.WebApiServer.CrawlUrl}/Finance/AllLastestFinancialInfo";

                HttpClient http = new HttpClient();

                var fetchDataResponse = await http.GetAsync(fetchDataUri);
                fetchDataResponse.EnsureSuccessStatusCode();
                if (fetchDataResponse.StatusCode == HttpStatusCode.OK)
                {
                    string responseData = await fetchDataResponse.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<SymbolFinancialIndex>>(responseData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    await _context.AddRangeAsync(data);
                    await _context.SaveChangesAsync();
                }

                return new Result();
            }
            catch (Exception ex)
            {
                return new Result {
                    Code = ResponseCode.SYS_GENERIC_ERROR,
                    Message = ex.Message
                };
            }
        }
    }
}
