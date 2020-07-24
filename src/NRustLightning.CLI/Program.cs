using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NBitcoin;
using NRustLightning.Client;
using NRustLightning.Infrastructure.Networks;

namespace NRustLightning.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var command = CommandLine.GetRootCommand();
            command.Handler = CommandHandler.Create((ParseResult pr) =>
            {
                Console.WriteLine($"Calling handler");
                Network? network; 
                var n = pr.RootCommandResult.ValueForOption<string>("network");
                network = n == "mainnet" ? Network.Main :
                    n == "testnet" ? Network.TestNet :
                    n == "regtest" ? Network.RegTest : null;
                if (network is null)
                {
                    if (pr.RootCommandResult.ValueForOption<bool>("testnet"))
                    {
                        network = Network.TestNet;
                    }
                    else if (pr.RootCommandResult.ValueForOption<bool>("regtest"))
                    {
                        network = Network.RegTest;
                    }
                }
                if (network is null)
                    network = Network.Main;
                
                var ip = pr.RootCommandResult.ValueForOption<string>("rpcip");
                if (String.IsNullOrEmpty(ip))
                    ip = "127.0.0.1";

                var networkProvider = new NRustLightningNetworkProvider(network.NetworkType);
                var nrustLightningNetwork = networkProvider.GetByCryptoCode(pr.RootCommandResult.ValueForOption<string>("cryptocode") ?? "btc");

                var port = pr.RootCommandResult.ValueForOption<int>("rpcport");
                if (port == 0)
                    port = 80;
                var baseUrl = $"http://{ip}:{port}";
                var client = new NRustLightningClient(baseUrl, nrustLightningNetwork);
                
                var subCommand = pr.CommandResult.Symbol.Name;
                Console.WriteLine($"Subcomamnd is {subCommand}");
                if (subCommand == SubCommands.GetInfo)
                {
                    var nodeInfo = client.GetInfoAsync().Result;
                    Console.WriteLine(nodeInfo.ConnectionString);
                }
                else
                {
                    throw new ArgumentException($"Unknown sub command {subCommand}");
                }
                return;
            });
            var commandLine = new CommandLineBuilder(command).UseDefaults()
                .Build();
            await commandLine.InvokeAsync(args);
        }
    }
}