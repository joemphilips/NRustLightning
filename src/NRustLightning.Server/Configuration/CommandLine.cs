using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace NRustLightning.Server.Configuration
{
    public static class CommandLine
    {

        public static RootCommand GetRootCommand()
        {
            var rootCommand = new RootCommand
            {
                new Option(new[] {"-n", "--network"},
                    "Set the network among (mainnet, testnet, regtest) (default:mainnet)") { },
                new Option<DirectoryInfo>(new[] {"--datadir", "-d"}, "Directory to store data")
            };
            rootCommand.Description =
                "NRustLightning\r\nBolt-Compatible Lightning Network node which uses rust-lightning internally";
            return rootCommand;
        }
        public static CommandLineBuilder GetBuilder() => new CommandLineBuilder(GetRootCommand());

        public static Parser GetParser() => GetBuilder().Build();
    }
}