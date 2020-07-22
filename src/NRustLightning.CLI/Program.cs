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

namespace NRustLightning.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var command = CommandLine.GetRootCommand();
            command.Handler = CommandHandler.Create((ParseResult pr) =>
            {
                throw new NotImplementedException();
                return;
            });
            var commandLine = new CommandLineBuilder(command).UseDefaults()
                .Build();
            await commandLine.InvokeAsync(args);
        }
    }
}