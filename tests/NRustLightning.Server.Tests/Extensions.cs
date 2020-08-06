using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Interfaces;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Services;
using NRustLightning.Server.Tests.Support;
using NRustLightning.Server.Tests.Stubs;
using NRustLightning.Utils;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests
{
    public static class Extensions
    {

        public static async Task<GetBlockRPCResponse> GetBestBlockAsync(this RPCClient c, GetBlockVerbosity v)
        {
            return await c.GetBlockAsync(await c.GetBestBlockHashAsync(), v);
        }

        public static async Task<uint256[]> GenerateToOwnAddressAsync(this RPCClient c, int num)
        {
            var addr = await c.GetNewAddressAsync();
            return await c.GenerateToAddressAsync(num, addr);
        }
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

        /// <summary>
        ///  Start bitcoind and nbxplorer in the backgroud.
        /// </summary>
        /// <param name="dockerFixture"></param>
        /// <param name="output"></param>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static async Task<ExplorerClient> StartExplorerFixtureAsync(this DockerFixture dockerFixture, ITestOutputHelper output, string caller)
        {
            var ports = new int[2];
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
                {"NBXPLORER_PORT", ports[1]},
                {"DATA_PATH", dataPath }
            };
            var envFile = Path.Join(dataPath, "env.sh");
            using (TextWriter w = File.AppendText(envFile))
            {
                foreach (var kv in env)
                {
                    w.WriteLine($"export {kv.Key}='{kv.Value}'");
                }
            }
            try
            {
                await dockerFixture.InitAsync(() => new DockerFixtureOptions
                {
                    DockerComposeFiles = new[] {"docker-compose.base.yml"},
                    EnvironmentVariables = env,
                    DockerComposeDownArgs = "--remove-orphans --volumes",
                    // we need this because c-lightning is not working well with bind mount.
                    // If we use volume mount instead, this is the only way to recreate the volume at runtime.
                    DockerComposeUpArgs = "--renew-anon-volumes",
                    StartupTimeoutSecs = 400,
                    LogFilePath = Path.Join(dataPath, "docker-compose.log"),
                    CustomUpTest = o =>
                    {
                        return
                            o.Any(x => x.Contains("BTC: Node state changed: NBXplorerSynching => Ready")); // nbx is up
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
            return new ExplorerClient(btcNetwork.NbXplorerNetwork, new Uri($"http://localhost:{ports[1]}"));
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
            var envFile = Path.Join(dataPath, "env.sh");
            using (TextWriter w = File.AppendText(envFile))
            {
                foreach (var kv in env)
                {
                    w.WriteLine($"export {kv.Key}='{kv.Value}'");
                }
            }
            try
            {
                await dockerFixture.InitAsync(() => new DockerFixtureOptions
                {
                    DockerComposeFiles = new[] {"docker-compose.yml"},
                    EnvironmentVariables = env,
                    DockerComposeDownArgs = "--remove-orphans --volumes",
                    // we need this because c-lightning is not working well with bind mount.
                    // If we use volume mount instead, this is the only way to recreate the volume at runtime.
                    DockerComposeUpArgs = "--renew-anon-volumes",
                    StartupTimeoutSecs = 400,
                    LogFilePath = Path.Join(dataPath, "docker-compose.log"),
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
                    services.AddSingleton(Network.RegTest);
                    services.AddSingleton<IMacaroonSecretRepository, InMemoryMacaroonSecretRepository>();
                    // services.AddSingleton<ILSATInvoiceProvider, IRepository>();
                    
                    services.AddSingleton<IFeeEstimator, TestFeeEstimator>();
                    services.AddSingleton<IBroadcaster, TestBroadcaster>();
                    services.AddSingleton<IChainWatchInterface, TestChainWatchInterface>();
                    services.AddSingleton<IWalletService, StubWalletService>();
                    services.AddSingleton<IRepository, InMemoryRepository>();
                    services.AddSingleton<INBXplorerClientProvider, StubNBXplorerClientProvider>();
                });
                webHost.UseTestServer();
            });
        }
    }
}