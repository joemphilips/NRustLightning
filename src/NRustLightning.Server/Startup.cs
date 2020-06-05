using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using DotNetLightning.Utils;
using NBitcoin.RPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using NBXplorer;
using NRustLightning.Server.Authentication;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.JsonConverters;
using NRustLightning.Server.Middlewares;

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
            services.AddControllers().AddJsonOptions(options =>
            {
                var c = options.JsonSerializerOptions.Converters;
                c.Add(new PaymentRequestJsonConverter());
            });
            services.AddHttpClient();
            services.AddSingleton<ISystemClock, SystemClock>();
            services.AddNRustLightning();
            services.ConfigureNRustLightning(Configuration, logger);
            services.AddMvc();
            
            // configure lsat/macaroon authentication
            string? ourServiceName = null;
            int ourServiceTier = 0;
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = LSATDefaults.Scheme;
                options.DefaultChallengeScheme = LSATDefaults.Scheme;
            }).AddLSATAuthentication(options =>
            {
                options.ServiceName = "nrustlightning";
                options.ServiceTier = ourServiceTier;
                Configuration.GetSection("LSAT").Bind(options);
                ourServiceName = options.ServiceName;
                ourServiceTier = options.ServiceTier;
                // we want to give users only read capability when they have payed for it. not write.
                options.MacaroonCaveats.Add($"{ourServiceName}{LSATDefaults.CapabilitiesConditionPrefix}=read");
                options.InvoiceCreationOption.Amount =
                    LNMoney.MilliSatoshis(Configuration.GetSection("LSAT").GetOrDefault("amount", 0));
                options.InvoiceCreationOption.Description = Configuration.GetSection("LSAT").GetOrDefault("description", "this is invoice for using nrustlightning");
                options.InvoiceCreationOption.Expiry =
                    TimeSpan.FromSeconds(Configuration.GetSection("LSAT").GetOrDefault("expirysecconds", 3600));
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Readonly", policy =>
                {
                    policy.RequireClaim("service", $"{ourServiceName}:{ourServiceTier}");
                    policy.RequireClaim($"{ourServiceName}{LSATDefaults.CapabilitiesConditionPrefix}", "read", "admin");
                });
                
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireClaim("service", $"{ourServiceName}:{ourServiceTier}");
                    policy.RequireClaim($"{ourServiceName}{LSATDefaults.CapabilitiesConditionPrefix}", "admin");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                var useLoggingMiddleware = Configuration.GetSection("debug").GetOrDefault("http", true);
                if (useLoggingMiddleware)
                    app.UseMiddleware<RequestResponseLoggingMiddleware>();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
