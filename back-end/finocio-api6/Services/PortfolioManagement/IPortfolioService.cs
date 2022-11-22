using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sample_api6.Entity;
using sample_api6.Model;
using sample_ultilities;

namespace sample_api6.Services
{
    public interface IPortfolioService
    {
        Result<List<FinoPortfolio>> ListPortfolios();

        Task<Result<FinoPortfolio>> GetPortfolio(long id);

        Result<FinoPortfolio> Createportfolio(string identification, string name, string watchList);
        /// <summary>
        /// dong bo user dang nhap vs tk anonymous truoc khi dang ky
        /// oldIdentification: laf device_key
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="oldIdentification"></param>
        /// <returns></returns>
        Result<FinoPortfolio> SyncPortfolioWithRegistedUser(string uid, string oldIdentification);

        Task<Result> UpdatePortfolio(UpdateWatchlistRequest request);
    }
}
