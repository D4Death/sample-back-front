using System;
using Microsoft.EntityFrameworkCore;
using net_core_sample_crawl.Entity;

namespace net_core_sample_crawl.Context
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
        {
        }
        public DbSet<CompanyInfo> Companies { get; set; }

        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<ExchangeTradingTime> ExchangeTradingTimes { get; set; }
        //public DbSet<IntraDay> IntraDays { get; set; }
        public DbSet<SymbolFinancialIndex> Symbols { get; set; }
        public DbSet<TradingData> TradingDatas { get; set; }
        //public DbSet<MiniIntradayMsg> MiniIntradayMsgs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("stk");

            //modelBuilder.Entity<MiniIntradayMsg>(entity => {
            //    entity.HasKey(m => m.Id);
            //    entity.Property(m => m.message_data);
            //});

            modelBuilder.Entity<CompanyInfo>(entity =>
            {
                entity.HasKey(m => m.Code);
                entity.Property(m => m.CEO);
                entity.Property(m => m.Descriptions);
                entity.Property(m => m.EngName);
                entity.Property(m => m.Name);
                entity.Property(m => m.Exchange);
                entity.Property(m => m.Branches);
                entity.Property(m => m.BusinessAreas);
                entity.Property(m => m.BusinessLicenseNumber);
                entity.Property(m => m.CharterCapital);
                entity.Property(m => m.Email);
                entity.Property(m => m.Employees);
                entity.Property(m => m.EngName);
                entity.Property(m => m.EstablishmentDate);
                entity.Property(m => m.Fax);
                entity.Property(m => m.ForeignOwnership);
                entity.Property(m => m.HeadQuarters);
                entity.Property(m => m.HistoryDesc);
                entity.Property(m => m.ICBCode);
                entity.Property(m => m.InitialListingPrice);
                entity.Property(m => m.IsListed);
                entity.Property(m => m.IssueDate);
                entity.Property(m => m.ListingVolume);
                entity.Property(m => m.Name);
                entity.Property(m => m.OtherOwnership);
                entity.Property(m => m.Phone);
                entity.Property(m => m.ShortName);
                entity.Property(m => m.StateOwnership);
                entity.Property(m => m.TaxIDNumber);
                entity.Property(m => m.WebAddress);
                entity.Property(m => m.ListingDate);
            });

            //modelBuilder.Entity<IntraDayMessage>(entity =>
            //{
            //    entity.HasKey(m => m.Id);
            //    entity.Property(m => m.HighPrice);
            //    entity.Property(m => m.LowPrice);
            //    entity.Property(m => m.AvgPrice);
            //    entity.Property(m => m.Symbol);
            //    entity.Property(m => m.Quantity);
            //    entity.Property(m => m.Price);
            //    entity.Property(m => m.SID);
            //    entity.Property(m => m.Time);
            //    entity.Property(m => m.TotalVol);
            //    entity.Property(m => m.TradeUnixTime);
            //    entity.Property(m => m.IsBuyerMarketMaker);
            //});

            modelBuilder.Entity<Exchange>(entity =>
            {
                entity.HasKey(m => m.Code);
                entity.Property(m => m.Name);
                entity.Property(m => m.Location);
                entity.Property(m => m.TimeZone);
                entity.Property(m => m.Phone);
                entity.Property(m => m.Fax);
                entity.Property(m => m.Website);
                entity.Property(m => m.Email);
            });

            modelBuilder.Entity<TradingTime>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.MorningStart);
                entity.Property(m => m.MorningEnd);
                entity.Property(m => m.AfternoonStart);
                entity.Property(m => m.AfternoonEnd);
                entity.Property(m => m.ATOStart);
                entity.Property(m => m.ATOEnd);
                entity.Property(m => m.ATCStart);
                entity.Property(m => m.ATCEnd);
            });

            modelBuilder.Entity<ExchangeTradingTime>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.ExchangeCode);
                entity.Property(m => m.TradingTimeId);
            });

            modelBuilder.Entity<SymbolFinancialIndex>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Code);
                entity.Property(m => m.Descriptions);
                entity.Property(m => m.Name);
                entity.Property(m => m.Exchange);
                entity.Property(m => m.OnDate);
                entity.Property(m => m.CurrentRatio);
                entity.Property(m => m.AverageVolume);
                entity.Property(m => m.BEP);
                entity.Property(m => m.DailyOpen);
                entity.Property(m => m.EPS);
                entity.Property(m => m.FastPaymentRatio);
                entity.Property(m => m.InterestCoverageRatio);
                entity.Property(m => m.LastClose);
                entity.Property(m => m.PERatio);
                entity.Property(m => m.QuickRatio);
                entity.Property(m => m.ROA);
                entity.Property(m => m.ROCE);
                entity.Property(m => m.ROE);
                entity.Property(m => m.ROS);
                entity.Property(m => m.ROTC);
                entity.Property(m => m.SharesOutstanding);
                entity.Property(m => m.Volume);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
