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

            return
                Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder => { configureConfig(builder); })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options => {
                        options.ConfigureEndpoints();
                    });
                });
        }

    }
    
    public static class KestrelServerOptionsExtensions
    {
        private static void ConfigureEndPoint(this KestrelServerOptions options, IConfiguration conf, string subsectionKey,
            int defaultPort, string defaultBind, ILogger<Program> logger, Action<ListenOptions> listenConfigure)
        {
            var subSection = conf.GetSection(subsectionKey);
            var port = subSection.GetValue("port", defaultPort);
            var endPoints = new List<IPEndPoint>();
            var b = subSection["bind"];
            if (b != null)
            {
                foreach (var section in b.Split(","))
                {
                    if (NBitcoin.Utils.TryParseEndpoint(section, port, out var endpoint) &&
                        endpoint is IPEndPoint ipEndPoint)
                    {
                        endPoints.Add(ipEndPoint);
                    }

                    if (section == "localhost")
                    {
                        endPoints.Add(new IPEndPoint(IPAddress.IPv6Loopback, port));
                        endPoints.Add(new IPEndPoint(IPAddress.Loopback, port));
                    }
                }
            }

            if (endPoints.Count == 0)
            {
                endPoints.Add(new IPEndPoint(IPAddress.Parse(defaultBind), port));
            }

            foreach (var endpoint in endPoints)
            {
                logger.LogInformation($"Binding with {subsectionKey} to {endpoint}");
                options.Listen(endpoint, listenConfigure);
            }
        }
        
        public static void ConfigureEndpoints(this KestrelServerOptions options)
        {
            var logger = options.ApplicationServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger<Program>();
            var config = options.ApplicationServices.GetRequiredService<IConfiguration>();

            # region http
            options.ConfigureEndPoint(config, "http", Constants.DefaultHttpPort, Constants.DefaultHttpBind, logger,
                listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                }
            );
            # endregion
            
            # region https
            var httpsConf = config.GetSection("https");
            var noHttps = config.GetValue<bool>("nohttps");
            if (!noHttps && httpsConf.Exists())
            {
                var https = new HttpsConfig();
                httpsConf.Bind(httpsConf);
                logger.LogDebug($"https config is port: {https.Port}. cert: {https.Cert}. pass: {https.CertPass}");
                options.ConfigureEndPoint(config, "https", Constants.DefaultHttpsPort, Constants.DefaultHttpsBind, logger,
                    listenOptions =>
                    {
                        listenOptions.UseConnectionLogging();
                        listenOptions.UseHttps(https.Cert, https.CertPass);
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    });
            }
            # endregion

            # region p2p
            
            options.ConfigureEndPoint(config, "p2p", Constants.DefaultP2PPort, Constants.DefaultP2PBind, logger,
                listenOptions =>
                {
                    listenOptions.UseConnectionLogging();
                    listenOptions.UseConnectionHandler<P2PConnectionHandler>();
                });
            # endregion
        }
    }
}
