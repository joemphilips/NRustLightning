using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Net;
using NBitcoin;
using NBitcoin.DataEncoders;
using NRustLightning.Client;
using NRustLightning.Utils;

namespace NRustLightning.CLI
{
    public static class CommandLine
    {
        public static Option[] GetOptions()
        {
            return new Option[]
            {
                new Option(new[] {"-n", "--network"},
                    "Set the network among (mainnet, testnet, regtest) (default:mainnet)"),
                new Option(new [] {"--rpcip", "--ip"}, "An ip address for the node we send an rpc request. (default: 127.0.0.1)")
                {
                    Argument = new Argument<string>
                    {
                        Arity = ArgumentArity.ZeroOrOne
                    }
                },
                new Option(new []{"--rpcport", "--port"}, "An port number of the rpc server listening on for http request. (default: 80)"), 
            };
        }

        private static Command Connect()
        {
            var c = new Command("connect", "connect to other lightning node");
            var op1 = new Option(new [] { "--nodeid", "--pubkey" }, "hex-encoded public key of the node we want to connect to")
            {
                Argument = new Argument<string>
                {
                    Arity = ArgumentArity.ExactlyOne
                }
            };
            op1.AddValidator(r =>
            {
                var msg = "Must specify valid pubkey in hex";
                var v = r.GetValueOrDefault<string>();
                
                if (String.IsNullOrEmpty(v))
                {
                    return msg;
                }

                if (!HexEncoder.IsWellFormed(v))
                {
                    return $"Invalid hex for pubkey: {v}";
                }

                var hexEncoder = new HexEncoder();
                var b = hexEncoder.DecodeData(v);

                if (!PubKey.Check(b, true))
                {
                    return $"Invalid pubkey {v}";
                }
                return null;
            });
            c.AddOption(op1);
            
            var op2 = new Option("--host", "ip:port pair of the node")
            {
                Argument = new Argument<string>
                {
                    Arity = ArgumentArity.ExactlyOne
                }
            };
            op2.AddValidator(r =>
            {
                var v = r.GetValueOrDefault<string>();
                
                if (String.IsNullOrEmpty(v))
                {
                    return "Invalid host";
                }

                if (!NBitcoin.Utils.TryParseEndpoint(v, 80, out var _))
                {
                    return "Invalid host";
                }

                return null;
            });
            c.AddOption(op2);
            return c;
        }

        private static IEnumerable<Command> GetSubCommands()
        {
            yield return new Command("getinfo", "get basic information from the node");
            yield return Connect();
        }

        public static RootCommand GetRootCommand()
        {
            var rootCommand = new RootCommand();
            rootCommand.Name = "nrustlightning-cli";
            rootCommand.Description = "CLI interface for requesting to NRustLightning server";
            foreach(var op in GetOptions()) rootCommand.Add(op);
            foreach(var sub in GetSubCommands()) rootCommand.AddCommand(sub);
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
    }
}