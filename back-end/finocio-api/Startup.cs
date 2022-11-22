using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sample_api.Context;
using sample_api.Helpers;
using sample_api.Services;
using sample_ultilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace sample_api
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

            services.AddDbContext<ApiDbContext>(options =>
                        options.UseNpgsql(Configuration.GetConnectionString("HostConnection")), ServiceLifetime.Singleton);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JwtKey:Issuer"],
                    ValidAudience = Configuration["JwtKey:Issuer"],
                    IssuerSigningKey = new
                    SymmetricSecurityKey
                    (Encoding.ASCII.GetBytes(Configuration["JwtKey:Key"]))
                };
            });

            //Configure other services up here
            var multiplexer = ConnectionMultiplexer.Connect("localhost:2309,password=WdGY6ZJ0yFWyKrf,allowAdmin=True,connectTimeout=60000");
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ITokenBaseService, TokenBaseService>();
            services.AddSingleton<IPortfolioService, PortfolioService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IMarketService, MarketService>();
            services.AddSingleton<IChartManagement, ChartManagement>();
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

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

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
            AppSettings.JwtKey = Configuration["JwtKey:Key"];
            AppSettings.Issuer = Configuration["JwtKey:Issuer"];
            AppSettings.AppRequestKey = Configuration["AppRequestKey"];
        }
    }
}
