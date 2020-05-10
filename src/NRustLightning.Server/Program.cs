using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.Configuration.SubConfiguration;

namespace NRustLightning.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var command = Configuration.CommandLine.GetRootCommand();
            command.Handler = CommandHandler.Create((ParseResult parseResult) =>
            {
                CreateHostBuilder(args, parseResult).Build().Run();
            });
            await command.InvokeAsync(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args, ParseResult parseResult)
        {
            Func<IConfigurationBuilder, IConfigurationBuilder> configureConfig =
                builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    Directory.CreateDirectory(Constants.HomeDirectoryPath);
                    var iniFile = Path.Join(Constants.HomeDirectoryPath, "nrustlightning.conf");
                    if (File.Exists(iniFile))
                    {
                        builder.AddIniFile(iniFile);
                    }
                    builder.AddEnvironmentVariables(prefix: "NRUSTLIGHTNING_");
                    return builder.AddCommandLineOptions(parseResult);
                };

            var logger = Startup.GetStartupLoggerFactory().CreateLogger<Program>();
            
            var config = configureConfig(new ConfigurationBuilder()).Build();
            
            var v = config.GetValue("bind", "*");
            var isListenAllIp = v == "*";
            
            var p2pPort = config.GetValue("port", Constants.DefaultP2PPort);

            var httpPort = config.GetValue("httpport", Constants.DefaultHttpPort);

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
                            logger.LogDebug($"https config is port: {https.Port}. cert: {https.Cert}. pass: {https.CertPass}");
                            options.ListenAnyIP(https.Port, listenOptions =>
                            {
                                listenOptions.UseConnectionLogging();
                                listenOptions.UseHttps(https.Cert, https.CertPass);
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
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
                            var allowedIpEndPoint = new IPEndPoint(IPAddress.Parse(v), p2pPort);
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
