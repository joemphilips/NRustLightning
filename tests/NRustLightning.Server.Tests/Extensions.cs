using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BTCPayServer.Lightning;
using BTCPayServer.Lightning.CLightning;
using BTCPayServer.Lightning.LND;
using DockerComposeFixture;
using DockerComposeFixture.Exceptions;
using LSATAuthenticationHandler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NBitcoin;
using NBitcoin.RPC;
using NBXplorer;
using NRustLightning.Client;
using NRustLightning.Interfaces;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Repository;
using NRustLightning.Server.Services;
using NRustLightning.Server.Tests.Support;
using NRustLightning.Server.Tests.Stubs;
using NRustLightning.Utils;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests
{
    public static class Extensions
    {
        private static byte[] GetCertificateFingerPrint(string filePath)
        {
            X509Certificate2 cert = new X509Certificate2(filePath);
            using var hashAlg = SHA256.Create();
            return hashAlg.ComputeHash(cert.RawData);
        }

        private static string GetCertificateFingerPrintHex(string filepath)
        {
            return Hex.Encode(GetCertificateFingerPrint(filepath));
        }
        
        public static async Task<Clients> StartLNTestFixtureAsync(this DockerFixture dockerFixture, ITestOutputHelper output, string caller)
        {
            var ports = new int[5];
            Support.Utils.FindEmptyPort(ports);
            var dataPath = Path.GetFullPath(caller);
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
            else
            {
                Directory.Delete(dataPath, true);
                Directory.CreateDirectory(dataPath);
            }
            var env = new Dictionary<string, object>()
            {
                {
                    "BITCOIND_RPC_AUTH",
                    Constants.BitcoindRPCAuth
                },
                {"BITCOIND_RPC_USER", Constants.BitcoindRPCUser},
                {"BITCOIND_RPC_PASS", Constants.BitcoindRPCPass},
                {"BITCOIND_RPC_PORT", ports[0]},
                {"LND_REST_PORT", ports[1]},
                {"LIGHTNINGD_RPC_PORT", ports[2]},
                {"HTTP_PORT", ports[3]},
                {"NBXPLORER_PORT", ports[4]},
                {"DATA_PATH", dataPath }
            };
            try
            {
                await dockerFixture.InitAsync(() => new DockerFixtureOptions
                {
                    DockerComposeFiles = new[] {"docker-compose.yml"},
                    EnvironmentVariables = env,
                    DockerComposeDownArgs = "--remove-orphans --volumes",
                    StartupTimeoutSecs = 400,
                    CustomUpTest = o =>
                    {
                        return
                            o.Any(x => x.Contains("Content root path: /app")) // nrustlightning is up
                            && o.Any(x => x.Contains("Server started with public key")) // lightningd is up
                            && o.Any(x => x.Contains("BTC: Node state changed: NBXplorerSynching => Ready")) // nbx is up
                            && o.Any(x => x.Contains("BTCN: Server listening on")); // lnd is up
                    }
                });
            }
            catch (DockerComposeException ex)
            {
                foreach (var m in ex.DockerComposeOutput)
                {
                    output.WriteLine(m);
                    throw;
                }
            }
            
            var networkProvider = new NRustLightningNetworkProvider(NetworkType.Regtest);
            var btcNetwork = networkProvider.GetByCryptoCode("BTC");
            var lndMacaroonPath = Path.Join(dataPath, ".lnd", "chain", "bitcoin", "regtest", "admin.macaroon");
            var lndTlsCertThumbPrint = GetCertificateFingerPrintHex(Path.Join(dataPath, ".lnd", "tls.cert"));
            var clients = new Clients(
                new RPCClient($"{Constants.BitcoindRPCUser}:{Constants.BitcoindRPCPass}", new Uri($"http://localhost:{ports[0]}"), NBitcoin.Network.RegTest),
                (LndClient)LightningClientFactory.CreateClient($"type=lnd-rest;macaroonfilepath={lndMacaroonPath};certthumbprint={lndTlsCertThumbPrint};server=https://localhost:{ports[1]}", NBitcoin.Network.RegTest),
                (CLightningClient)LightningClientFactory.CreateClient($"type=clightning;server=tcp://127.0.0.1:{ports[2]}", NBitcoin.Network.RegTest), 
                new NRustLightningClient($"http://localhost:{ports[3]}",btcNetwork),
                new ExplorerClient(btcNetwork.NbXplorerNetwork, new Uri($"http://localhost:{ports[4]}"))
                );
            return clients;
        }

        public static IHostBuilder ConfigureTestHost(this IHostBuilder builder)
        {
            return builder.ConfigureWebHost(webHost =>
            {
                webHost.UseEnvironment("Development");
                var curr = Directory.GetCurrentDirectory();
                webHost.UseContentRoot(curr);
                webHost.ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddJsonFile("appsettings.test.json");
                });
                webHost.UseStartup<Startup>();
                webHost.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IKeysRepository, InMemoryKeysRepository>();
                    services.AddSingleton<IInvoiceRepository, InMemoryInvoiceRepository>();
                    services.AddSingleton<IMacaroonSecretRepository, InMemoryMacaroonSecretRepository>();
                    services.AddSingleton<ILSATInvoiceProvider, InMemoryInvoiceRepository>();
                    
                    services.AddSingleton<IFeeEstimator, TestFeeEstimator>();
                    services.AddSingleton<IBroadcaster, TestBroadcaster>();
                    services.AddSingleton<IChainWatchInterface, TestChainWatchInterface>();
                    services.AddSingleton<IPeerManagerProvider, TestPeerManagerProvider>();
                    services.AddSingleton<IWalletService, StubWalletService>();
                    services.AddSingleton<INBXplorerClientProvider, StubNBXplorerClientProvider>();
                });
                webHost.UseTestServer();
            });
        }
    }
}