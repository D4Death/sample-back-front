using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sample_ultilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net_core_sample_crawl.Context;
using net_core_sample_crawl.Model;
using net_core_sample_crawl.Repository;

namespace net_core_sample_crawl
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<StockDbContext>(options =>
                        options.UseNpgsql(Configuration.GetConnectionString("HostConnection")), ServiceLifetime.Singleton);

            services.AddSingleton<ICrawlRepo, CrawlRepo>();
            BuildAppSettingsProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();

                endpoints.MapControllerRoute("default",
                                     "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void BuildAppSettingsProvider()
        {
            AppSettings.ConnectionStrings = new ConnectionStrings
            {
                LocalConnection = Configuration.GetConnectionString("HostConnection")
            };
            AppSettings.WebApiServer = new WebApiServer
            {
                CrawlUrl = Configuration["ApiCrawl:FireAnt"]
            };
            AppSettings.JwtKey = Configuration["JwtKey"];
        }
    }
}
