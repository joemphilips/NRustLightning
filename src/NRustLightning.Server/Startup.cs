using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using NRustLightning.Server.Configuration;
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
                options.JsonSerializerOptions.Converters
                    .Add(new PaymentRequestJsonConverter());
                options.JsonSerializerOptions.Converters
                    .Add(new HexPubKeyConverter());
                options.JsonSerializerOptions
                    .Converters.Add(new NullableStructConverterFactory());
                options.JsonSerializerOptions.Converters
                    .Add(new JsonFSharpConverter());
            });
            services.AddHttpClient();
            services.AddSingleton<ISystemClock, SystemClock>();
            services.AddNRustLightning();
            services.ConfigureNRustLightning(Configuration, logger);
            services.AddMvc();
            services.ConfigureNRustLightningAuth(Configuration);
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
