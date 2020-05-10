using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin.RPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NRustLightning.Server
{
    public class Startup
    {
        private readonly ILogger<Startup> logger;

        internal static ILoggerFactory GetStartupLoggerFactory()
            => LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Debug);
            });
        public Startup(IConfiguration configuration)
        {
            this.logger = GetStartupLoggerFactory().CreateLogger<Startup>();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();
            services.AddNRustLightning();
            services.ConfigureNRustLightning(Configuration, logger);
            services.AddMvc();
            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
