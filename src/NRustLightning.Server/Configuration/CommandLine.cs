using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Hosting;
using NBitcoin;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Configuration
{
    public static class CommandLine
    {

        public static Option[] GetOptions()
        {
            var options = new List<Option>();
            var op = new[]
                {
                    new Option(new[] {"-n", "--network"},
                        "Set the network among (mainnet, testnet, regtest) (default:mainnet)")
                    {
                        Argument = new Argument {Arity = ArgumentArity.ZeroOrOne}.FromAmong("mainnet", "testnet", "regtest")
                    },
                    new Option(new[] {"--testnet"}, $"Use testnet"),
                    new Option(new[] {"--regtest"}, "Use regtest"),
                    new Option<DirectoryInfo>(new[] {"--datadir", "-d"}, "Directory to store data")
                    {
                        Argument = new Argument<DirectoryInfo> {Arity = ArgumentArity.ZeroOrOne}
                    },

                    // connection options
                    new Option<string>("--bind", "Bind to given address and always listen on it. (default: any ip)"),
                    new Option<int>(new []{"--port", "-p"}, $"Local p2p port to bind (default: {Constants.DefaultP2PPort})"),
                    new Option<string>("--httpport", $"port for listening http request: (default: {Constants.DefaultHttpPort})")
                };
            options.AddRange(op);
            var provider = new NRustLightningNetworkProvider(NetworkType.Mainnet);
            var allCryptoCodes = provider.GetAll().Select(n => n.CryptoCode.ToLowerInvariant()).ToArray();
            options.Add(
                new Option("--chain",
                $"Chains to support. You can specify this multiple times (default: btc)")
                { Argument = new Argument{ Arity =  ArgumentArity.ZeroOrMore}.FromAmong(allCryptoCodes)});
            foreach (var crypto in allCryptoCodes)
            {
                options.Add(new Option($"--{crypto}rpcuser",
                    "RPC authentication method 1: The RPC user (default: using cookie auth from default network folder)"));
                options.Add(new Option($"--{crypto}rpcpassword",
                    "RPC authentication method 1: The RPC password (default: using cookie auth from default network folder)"));
                options.Add(new Option($"--{crypto}rpccookiefile",
                    $"RPC authentication method 2: The RPC cookiefile (default: using cookie auth from default network folder)"));
                options.Add(new Option($"--{crypto}rpcauth",
                    $"RPC authentication method 3: user:password or cookiefile=path (default: using cookie auth from default network folder)"));
                options.Add(new Option($"--{crypto}rpcurl",
                    $"The RPC server url (default: rpc server depended on the network)"));
            }

            return options.ToArray();
        }
        /// <summary>
        /// Do not specify any default value here since we don't want to override the settings
        /// from other configuration source (e.g. environment variable).
        /// </summary>
        /// <returns></returns>
        public static RootCommand GetRootCommand()
        {
            var rootCommand = new RootCommand();
            rootCommand.Name = "nrustlightning";
            rootCommand.Description =
                "NRustLightning\nBolt-Compatible Lightning Network node which uses rust-lightning internally";
            foreach(var op in GetOptions()) rootCommand.Add(op);
            rootCommand.AddValidator(result =>
            {
                var hasNetwork = result.Children.Contains("network");
                if (result.Children.Contains("mainnet") && hasNetwork)
                    return "You cannot specify both '--network' and '--mainnet'";
                if (result.Children.Contains("testnet") && hasNetwork)
                    return "You cannot specify both '--network' and '--testnet'";
                if (result.Children.Contains("regtest") && hasNetwork)
                    return "You cannot specify both '--network' and '--regtest'";

                return null;
            });
            return rootCommand;
        }
        public static CommandLineBuilder GetBuilder() => new CommandLineBuilder(GetRootCommand());

        public static Parser GetParser() => GetBuilder().Build();
    }
}