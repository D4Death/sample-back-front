using System;
namespace worker_netcore_crawl
{
    public class Startup
    {
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        //public IConfiguration Configuration { get; }

        //// This method gets called by the runtime. Use this method to add services to the container.
        //// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddControllers();

        //    services.AddDbContext<StockDbContext>(options =>
        //                options.UseNpgsql(Configuration.GetConnectionString("HostConnection")), ServiceLifetime.Singleton);

        //    services.AddSingleton<IOHLCRepo, OHLCRepo>();
        //    BuildAppSettingsProvider();
        //}

        //// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }

        //    app.UseRouting();

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllers();
        //    });
        //}

        //private void BuildAppSettingsProvider()
        //{
        //    AppSettings.ConnectionStrings = new ConnectionStrings
        //    {
        //        LocalConnection = Configuration.GetConnectionString("HostConnection")
        //    };
        //    AppSettings.WebApiServer = new WebApiServer
        //    {
        //        CrawlUrl = Configuration["ApiCrawl:FireAnt"]
        //    };
        //    AppSettings.JwtKey = Configuration["JwtKey"];
        //}
    }
}
