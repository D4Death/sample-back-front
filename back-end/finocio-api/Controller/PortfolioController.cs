using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sample_api.Entity;
using sample_api.Model;
using sample_api.Services;
using sample_ultilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace sample_api.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/portfolio")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioRepo;

        public PortfolioController(IPortfolioService portfolioRepo)
        {
            this._portfolioRepo = portfolioRepo;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("test")]
        public String TestApi()
        {
            return "OK!";
        }

        [HttpGet]
        [Route("list-portfolio")]
        public Result<List<FinoPortfolio>> ListPortfolios()
        {
            var result = _portfolioRepo.ListPortfolios();
            return result;
        }

        [HttpGet]
        [Route("get-portfolio/{id}")]
        public async Task<Result<FinoPortfolio>> GetPortfolio(long id)
        {
            var result = await _portfolioRepo.GetPortfolio(id);
            return result;
        }

        [HttpPost]
        [Route("update-portfolio")]
        public async Task<Result> UpdatePortfolio([FromBody] UpdateWatchlistRequest request)
        {
            var result = await _portfolioRepo.UpdatePortfolio(request);
            return result;
        }

        [HttpPost]
        [Route("create-portfolio")]
        public Result<FinoPortfolio> Createportfolio([FromBody] CreatePortfolioReqest request)
        {
            var result = _portfolioRepo.Createportfolio(request.Identification, request.Name, request.WatchList);
            return result;
        }

        [HttpPost]
        [Route("sync-portfolio")]
        public Result<FinoPortfolio> SyncPortfolios([FromBody] SyncPortfolioRequest request)
        {
            var result = _portfolioRepo.SyncPortfolioWithRegistedUser(request.UId, request.OldIdentification);
            return result;
        }
    }
}
