using System;
using sample_api.Entity;
using Microsoft.EntityFrameworkCore;

namespace sample_api.Context
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<FinoPortfolio> Portfolios { get; set; }
        public DbSet<FinoUser> FinoUsers { get; set; }
        public DbSet<CompanyInfo> CompanyInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("fino");

            modelBuilder.Entity<FinoPortfolio>(entity => {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Name);
                entity.Property(m => m.UId);
                entity.Property(m => m.IsAnonymous);
                entity.Property(m => m.IsDefault);
                entity.Property(m => m.WatchList);
            });

            modelBuilder.Entity<FinoUser>(entity => {
                entity.HasKey(m => m.UId);
                entity.Property(m => m.UserName);
                entity.Property(m => m.Password);
                entity.Property(m => m.AuthToken);
            });

            modelBuilder.Entity<CompanyInfo>(entity => {
                entity.HasKey(m => m.Code);
                entity.Property(m => m.Branches);
                entity.Property(m => m.BusinessAreas);
                entity.Property(m => m.BusinessLicenseNumber);
                entity.Property(m => m.CEO);
                entity.Property(m => m.CharterCapital);
                entity.Property(m => m.Code);
                entity.Property(m => m.Descriptions);
                entity.Property(m => m.Email);
                entity.Property(m => m.Employees);
                entity.Property(m => m.EngName);
                entity.Property(m => m.EstablishmentDate);
                entity.Property(m => m.Exchange);
                entity.Property(m => m.Fax);
                entity.Property(m => m.ForeignOwnership);
                entity.Property(m => m.HeadQuarters);
                entity.Property(m => m.HistoryDesc);
                entity.Property(m => m.ICBCode);
                entity.Property(m => m.InitialListingPrice);
                entity.Property(m => m.IsListed);
                entity.Property(m => m.IssueDate);
                entity.Property(m => m.ListingDate);
                entity.Property(m => m.ListingVolume);
                entity.Property(m => m.Name);
                entity.Property(m => m.OtherOwnership);
                entity.Property(m => m.Phone);
                entity.Property(m => m.ShortName);
                entity.Property(m => m.StateOwnership);
                entity.Property(m => m.TaxIDNumber);
                entity.Property(m => m.WebAddress);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
