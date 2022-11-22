using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sample_ultilities;
using Microsoft.AspNetCore.Mvc;
using net_core_sample_crawl.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace net_core_sample_crawl.Controller
{
    [ApiController]
    [Route("api/crawl-data")]
    public class FetchDataController : ControllerBase
    {
        private readonly ICrawlRepo _crawlRepo;

        public FetchDataController(ICrawlRepo crawlRepo)
        {
            this._crawlRepo = crawlRepo;
        }

        [HttpGet]
        [Route("lastest-finacial-info")]
        public async Task<Result> FetchLastestFinancialSymbolInfo()
        {
            return await _crawlRepo.FetchLastestFinancialSymbolInfo();
        }

        [HttpGet]
        [Route("add-company-infos")]
        public async Task<Result> FetchCompanyInfo()
        {
            return await _crawlRepo.FetchCompanyInfo();
        }

        [HttpGet]
        [Route("add-exchange")]
        public async Task AddStockExchange()
        {
            await _crawlRepo.AddStockExchange();
        }

        [HttpGet]
        [Route("company-info-by-code/{code}")]
        public string FetchCompanyInfoByCode(string code)
        {
            _crawlRepo.AddCompanyByCode(code);

            return "OK";
        }
    }
}
