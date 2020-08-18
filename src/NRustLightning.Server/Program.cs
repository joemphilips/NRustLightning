using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DotNetLightning.Chain;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Configuration.SubConfiguration;
using NRustLightning.Infrastructure.Repository;
using NRustLightning.Interfaces;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.P2P;
using IKeysRepository = NRustLightning.Infrastructure.Interfaces.IKeysRepository;

namespace NRustLightning.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var command = Configuration.CommandLine.GetRootCommand();
            command.Handler = CommandHandler.Create(async (ParseResult parseResult) =>
            {
                var host = CreateHostBuilder(args, parseResult).Build();
                var k = host.Services.GetRequiredService<IKeysRepository>();
                if (k is FlatFileKeyRepository repo)
                {
                    var conf = host.Services.GetRequiredService<IOptions<Config>>();
                    var logger = host.Services.GetRequiredService<ILogger<Program>>();
                    if (conf.Value.HasSeedInConfig)
                    {
                        logger.LogWarning($"Passing seed as an option will make the node to hold the seed on memory. This is not recommended besides for the testing purpose");
                        repo.InitializeFromSeed(conf.Value.GetSeedInConfig());
                        // we do not store the seed.
                    }
                    else
                    {
                        var maybeEncryptedSeed = await conf.Value.TryGetEncryptedSeed();
                        if (maybeEncryptedSeed != null)
                        {
                            var isValidPin = false;
                            var pin = conf.Value.Pin;
                            var maybeErrMsg = repo.InitializeFromEncryptedSeed(maybeEncryptedSeed, pin);
                            if (maybeErrMsg is null)
                            {
                                isValidPin = true;
                            }
                            while (!isValidPin)
                            {
                                pin = string.Empty;
                                Console.WriteLine("Please enter your pin code:");
                                while (string.IsNullOrEmpty(pin))
                                {
                                    pin = Console.ReadLine();
                                }
                                
                                maybeErrMsg = repo.InitializeFromEncryptedSeed(maybeEncryptedSeed, pin);
                                if (maybeErrMsg is null)
                                {
                                    isValidPin = true;
                                }
                                else
                                {
                                    Console.WriteLine($"{maybeErrMsg}");
                                }
                            }
                        }
                        else
                        {
                            logger.LogWarning($"seed not found in {conf.Value.SeedFilePath}! generating new seed...");
                            var pin = conf.Value.GetNewPin();
                            var seed = RandomUtils.GetUInt256().ToString();
                            repo.InitializeFromSeed(seed);
                            await repo.EncryptSeedAndSaveToFile(seed, pin);
                        }
                    }
                }
                using (var _scope = host.Services.CreateScope())
                {
                    // TODO: check db version and run migration by dedicated service.
                }
                host.Run();
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
                            logger.LogInformation($"Listening on port {httpPort}");
                            listenOptions.UseConnectionLogging();
                        });
                        
                        var httpsConf = config.GetSection("https");
                        var noHttps = config.GetValue<bool>("nohttps");
                        if (!noHttps && httpsConf.Exists())
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
