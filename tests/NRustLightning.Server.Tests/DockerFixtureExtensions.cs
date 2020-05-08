using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using BTCPayServer.Lightning;
using BTCPayServer.Lightning.CLightning;
using BTCPayServer.Lightning.LND;
using DockerComposeFixture;
using DockerComposeFixture.Compose;
using DockerComposeFixture.Exceptions;
using NBitcoin.RPC;
using NRustLightning.Adaptors;
using NRustLightning.Client;
using NRustLightning.Server.Tests.Support;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests
{
    public static class DockerFixtureExtensions
    {
        public static Clients StartLNTestFixture(this DockerFixture dockerFixture, ITestOutputHelper output, [CallerMemberName]string caller = null)
        {
            var ports = new int[5];
            Support.Utils.FindEmptyPort(ports);
            var dataPath = Path.GetFullPath(caller);
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
            var clients = new Clients(
                new RPCClient($"{Constants.BitcoindRPCUser}:{Constants.BitcoindRPCPass}", new Uri($"http://localhost:{ports[0]}"), NBitcoin.Network.RegTest),
            (LndClient)LightningClientFactory.CreateClient($"type=lnd-rest;allowinsecure=true;server=http://localhost:{ports[1]}", NBitcoin.Network.RegTest),
                (CLightningClient)LightningClientFactory.CreateClient($"type=clightning;server=tcp://localhost:{ports[2]}", NBitcoin.Network.RegTest), 
                new NRustLightningClient($"https://localhost{ports[4]}")
                );
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
                {"HTTPS_PORT", ports[3]},
                {"HTTP_PORT", ports[4]},
                {"DATA_PATH", dataPath }
            };
            try
            {
                dockerFixture.InitOnce(() => new DockerFixtureOptions
                {
                    DockerComposeFiles = new[] {"docker-compose.yml"},
                    EnvironmentVariables = env,
                    CustomUpTest = o => o.Any(x => x.Contains("Content root path: /app"))
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

            return clients;
        }
    }
}