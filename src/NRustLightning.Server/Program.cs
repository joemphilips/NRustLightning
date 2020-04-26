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
            var v = config.GetValue<string>("P2PIpEndPoint");
            var ipEndPoint = (v is null) ? Constants.DefaultP2PEndPoint : IPEndPoint.Parse(v);
            var p2pPort = config.GetValue<int>("P2Port");
            var port = p2pPort == 0 ? Constants.DefaultP2PPort : p2pPort;
            return
                Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(config))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options => {
                        options.ListenAnyIP(port, listenOptions =>
                        {
                            listenOptions.UseConnectionLogging();
                            listenOptions.UseConnectionHandler<P2PConnectionHandler>();
                        });
                    });
                });
        }
    }
}
