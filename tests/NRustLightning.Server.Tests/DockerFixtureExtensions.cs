using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BTCPayServer.Lightning;
using BTCPayServer.Lightning.CLightning;
using BTCPayServer.Lightning.LND;
using DockerComposeFixture;
using DockerComposeFixture.Exceptions;
using NBitcoin.RPC;
using NRustLightning.Client;
using NRustLightning.Server.Tests.Support;
using NRustLightning.Utils;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests
{
    public static class DockerFixtureExtensions
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
            var ports = new int[4];
            Support.Utils.FindEmptyPort(ports);
            var dataPath = Path.GetFullPath(caller);
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
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
                {"DATA_PATH", dataPath }
            };
            try
            {
                await dockerFixture.InitOnceAsync(() => new DockerFixtureOptions
                {
                    DockerComposeFiles = new[] {"docker-compose.yml"},
                    EnvironmentVariables = env,
                    DockerComposeDownArgs = "--remove-orphans --volumes",
                    StartupTimeoutSecs = 600,
                    CustomUpTest = o =>
                    {
                        return
                            o.Any(x => x.Contains("Content root path: /app")) // nrustlightning is up
                            && o.Any(x => x.Contains("Server started with public key")) // lightningd is up
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
            
            await Task.Delay(4000);
            var lndMacaroonPath = Path.Join(dataPath, ".lnd", "chain", "bitcoin", "regtest", "admin.macaroon");
            var lndTlsCertThumbPrint = GetCertificateFingerPrintHex(Path.Join(dataPath, ".lnd", "tls.cert"));
            var clients = new Clients(
                new RPCClient($"{Constants.BitcoindRPCUser}:{Constants.BitcoindRPCPass}", new Uri($"http://localhost:{ports[0]}"), NBitcoin.Network.RegTest),
                (LndClient)LightningClientFactory.CreateClient($"type=lnd-rest;macaroonfilepath={lndMacaroonPath};certthumbprint={lndTlsCertThumbPrint};server=https://localhost:{ports[1]}", NBitcoin.Network.RegTest),
                (CLightningClient)LightningClientFactory.CreateClient($"type=clightning;server=tcp://127.0.0.1:{ports[2]}", NBitcoin.Network.RegTest), 
                new NRustLightningClient($"http://localhost:{ports[3]}")
                );
            return clients;
        }
    }
}