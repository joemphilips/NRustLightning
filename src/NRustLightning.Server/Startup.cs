using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.JsonConverters;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Middlewares;
#if DEBUG
using Microsoft.OpenApi.Models;
#endif

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
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.Converters
                    .Add(new HexPubKeyConverter());
                options.JsonSerializerOptions.Converters
                    .Add(new PaymentRequestJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new uint256JsonConverter());
                options.JsonSerializerOptions.Converters.Add(new FeatureBitJsonConverter());
                
                options.JsonSerializerOptions
                    .Converters.Add(new NullableStructConverterFactory());
                options.JsonSerializerOptions.Converters
                    .Add(new JsonFSharpConverter());
            });
            services.AddHttpClient();
            services.AddSingleton<ISystemClock, SystemClock>();
            services.ConfigureNRustLightning(Configuration, logger);
            services.AddNRustLightning();
            services.AddMvc();
            services.ConfigureNRustLightningAuth(Configuration);

#if DEBUG
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "NRustLightning API",
                    Description = "API endpoint for NRustLightning.Server",
                    Contact = new OpenApiContact()
                    {
                        Name = "Joe Miyamoto",
                        Email = "joemphilips@gmail.com"
                    },
                    TermsOfService = new Uri("https://example.com/terms"),
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT license",
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    }
                });
            });
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                var useLoggingMiddleware = Configuration.GetSection("debug").GetOrDefault("http", true);
                if (useLoggingMiddleware)
                    app.UseMiddleware<RequestResponseLoggingMiddleware>();

#if DEBUG
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NRustLightning API v1");
                });
#endif
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
