using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NRustLightning.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Func<IConfigurationBuilder, IConfigurationBuilder> configureConfig =
                (IConfigurationBuilder builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    Directory.CreateDirectory(Constants.HomeDirectoryPath);
                    var iniFile = Path.Join(Constants.HomeDirectoryPath, "nrustlightning.conf");
                    if (!File.Exists(iniFile))
                    {
                        File.Create(iniFile);
                    }

                    builder.AddIniFile(iniFile);
                    builder.AddEnvironmentVariables("NRUSTLIGHTNING");
                    return builder.AddCommandLine(args);
                };
            var config = configureConfig(new ConfigurationBuilder()).Build();
            var ipEndPoint = IPEndPoint.Parse(config.GetValue<string>("P2PIpEndPoint") ?? Constants.DefaultP2PHost);
            return
                Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(config))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options => { options.Listen(ipEndPoint, listenOptions =>
                        {
                            listenOptions.UseConnectionLogging();
                            listenOptions.UseConnectionHandler<P2PConnectionHandler>();
                        });
                    });
                });
        }
    }
}
