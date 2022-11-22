using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sample_ultilities;

namespace net_core_sample_crawl.Repository
{
    public interface ICrawlRepo
    {
        Task<Result> FetchLastestFinancialSymbolInfo();
        Task<Result> FetchCompanyInfo();
        Task AddCompanyByCode(string compCode);
        Task<Result> AddStockExchange();
    }
}
