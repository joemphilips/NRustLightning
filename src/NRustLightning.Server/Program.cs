using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NRustLightning.Server.Configuration;

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
                builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    Directory.CreateDirectory(Constants.HomeDirectoryPath);
                    var iniFile = Path.Join(Constants.HomeDirectoryPath, "nrustlightning.conf");
                    if (!File.Exists(iniFile))
                    {
                        File.Create(iniFile);
                    }

                    builder.AddIniFile(iniFile);
                    builder.AddEnvironmentVariables(prefix: "NRUSTLIGHTNING_");
                    return builder.AddCommandLine(args);
                };

            Action<ILoggingBuilder> configureLogging = builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Debug);
            };
            var s = new ServiceCollection();
            s.AddLogging(configureLogging);
            var logger = s.BuildServiceProvider().GetService<ILoggerFactory>().CreateLogger<Program>();
            
            var config = configureConfig(new ConfigurationBuilder()).Build();
            var v = config.GetValue<string>("P2PAllowIp");
            var isListenAllIp = v == "*";
            var maybeP2PPort = config.GetValue<int>("P2Port");
            var p2pPort = maybeP2PPort == 0 ? Constants.DefaultP2PPort : maybeP2PPort;

            var maybeHttpPort = config.GetValue<int>("HTTPPort");
            var httpPort = maybeHttpPort == 0 ? Constants.DefaultHttpPort : maybeHttpPort;

            return
                Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(config))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options => {
                        options.ListenAnyIP(httpPort, listenOptions =>
                        {
                            listenOptions.UseConnectionLogging();
                        });
                        var httpsConf = config.GetSection("https");
                        if (httpsConf.Exists())
                        {
                            var https = new HttpsConfig();
                            httpsConf.Bind(https);
                            logger.LogDebug($"https config is port: {https.Port}. cert: {https.CertName}. pass: {https.CertPass}");
                            options.ListenAnyIP(https.Port, listenOptions =>
                            {
                                listenOptions.UseConnectionLogging();
                                listenOptions.UseHttps(https.CertName, https.CertPass);
                            });
                        }

                        if (isListenAllIp)
                        {
                            logger.LogInformation($"Listening for P2P message from any IP on port {p2pPort}");
                            options.ListenAnyIP(p2pPort, listenOptions =>
                            {
                                listenOptions.UseConnectionLogging();
                                listenOptions.UseConnectionHandler<P2PConnectionHandler>();
                            });
                        }
                        else
                        {
                            var allowedIpEndPoint = IPEndPoint.Parse(v ?? Constants.DefaultP2PAllowIp);
                            logger.LogInformation($"Listening for P2P message from {allowedIpEndPoint}");
                            options.Listen(allowedIpEndPoint, listenOptions =>
                            {
                                listenOptions.UseConnectionLogging();
                                listenOptions.UseConnectionHandler<P2PConnectionHandler>();
                            });
                        }
                    });
                });
        }
    }
}
