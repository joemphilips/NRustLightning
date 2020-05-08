using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using DockerComposeFixture;
using DockerComposeFixture.Compose;
using DockerComposeFixture.Exceptions;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests
{
    public static class DockerFixtureExtensions
    {
        public static void StartLNTestFixture(this DockerFixture dockerFixture, ITestOutputHelper output, [CallerMemberName]string caller = null)
        {
            var ports = new int[5];
            Support.Utils.FindEmptyPort(ports);
            var dataPath = Path.GetFullPath(caller);
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
            Console.WriteLine($"Creating directory {dataPath}");
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
        }
    }
}