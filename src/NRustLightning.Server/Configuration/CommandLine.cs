using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Hosting;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Server.Configuration.SubConfiguration;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Configuration
{
    public static class CommandLine
    {

        public static Option[] GetOptions()
        {
            var options = new List<Option>();
            var op = new Option[]
                {
                    new Option<string>(new[] {"-n", "--network"},
                        "Set the network among (mainnet, testnet, regtest) (default:mainnet)")
                    {
                        Argument = new Argument<string> {Arity = ArgumentArity.ZeroOrOne}.FromAmong("mainnet", "testnet", "regtest")
                    },
                    new Option<bool>(new[] {"--testnet"}, $"Use testnet"),
                    new Option<bool>(new[] {"--regtest"}, "Use regtest"),
                    new Option<DirectoryInfo>(new[] {"--datadir", "-d"}, "Directory to store data")
                    {
                        Argument = new Argument<DirectoryInfo> {Arity = ArgumentArity.ZeroOrOne}
                    },

                    // connection options
                    new Option<string>("--bind", "Bind to given address and always listen on it. (default: any ip)")
                    {
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrMore
                        }
                    },
                    new Option<int>(new []{"--port", "-p"}, $"Local p2p port to bind (default: {Constants.DefaultP2PPort})")
                    {
                        Argument = new Argument<int>
                        {
                            Arity = ArgumentArity.ZeroOrOne
                        }
                    },
                    
                    new Option<string>("--externalip", $"address:port that we claim to listen on to peers.")
                    {
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrOne
                        }
                    }, 
                    
                    new Option<string>("--httpport", $"port for listening http request: (default: {Constants.DefaultHttpPort})")
                    {
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrOne
                        }
                    },
                    
                    new Option<string>("--nohttps", $"do not run https")
                    {
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrOne
                        }
                    },
                    new Option<string>("--https.port", $"port for listening HTTPs request: (default: {Constants.DefaultHttpsPort})")
                    {
                        Argument = new Argument<string> { Arity = ArgumentArity.ZeroOrOne }
                    },
                    new Option<string>("--https.cert", $"path to the https certification file. (default: {new HttpsConfig().Cert})")
                        {Argument = new Argument<string> {Arity =  ArgumentArity.ZeroOrOne}},
                    new Option<string>("--https.certpass", $"pass to open https certification file. (default: \"\")")
                        {Argument = new Argument<string> {Arity =  ArgumentArity.ZeroOrOne }},
                    
                    new Option<string>("--nbx.cookiefile", $"path to the cookie file for nbxplorer which is required for http authentication")
                    {
                        Argument = new Argument<string> {Arity = ArgumentArity.ZeroOrOne}
                    },
                    new Option<string>("--nbx.rpcurl", $"rpc endpoint for nbxplorer. (default: {Constants.DefaultNBXplorerUri})")
                    {
                        Argument = new Argument<string> {Arity = ArgumentArity.ZeroOrOne}
                    }
                };
            options.AddRange(op);
            
            # region Chain options
            var provider = new NRustLightningNetworkProvider(NetworkType.Mainnet);
            var allCryptoCodes = provider.GetAll().Select(n => n.CryptoCode.ToLowerInvariant()).ToArray();
            options.Add(
                new Option<string>("--chain",
                $"Chains to support. You can specify more than one chain by passing values delimited by ','. (default: btc)")
                { Argument = new Argument<string>{ Arity =  ArgumentArity.ZeroOrOne}.FromAmong(allCryptoCodes)});
            foreach (var crypto in allCryptoCodes)
            {
                options.Add(new Option<string>($"--{crypto}.rpc.user",
                    "RPC authentication method 1: The RPC user (default: using cookie auth from default network folder)")
                    {
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrOne
                        }
                    }
                );
                options.Add(new Option<string>($"--{crypto}.rpc.password",
                    "RPC authentication method 1: The RPC password (default: using cookie auth from default network folder)"
                    )
                    {
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrOne
                        }
                    }
                );
                options.Add(new Option<FileInfo>($"--{crypto}.rpc.cookiefile",
                    $"RPC authentication method 2: The RPC cookiefile (default: using cookie auth from default network folder)")
                    {
                        Argument = new Argument<FileInfo>
                        {
                            Arity = ArgumentArity.ZeroOrOne
                        }
                    }
                );
                options.Add(new Option<string>($"--{crypto}.rpc.auth",
                    $"RPC authentication method 3: user:password or cookiefile=path (default: using cookie auth from default network folder)")
                    {
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrOne
                        }
                    }
                );
                options.Add(new Option<string>($"--{crypto}.rpc.url",
                    $"The RPC server url (default: rpc server depended on the network)")
                    {
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrOne
                        }
                    }
                );
            }
            # endregion
            
            #region rust-lightning specific options
            var uc = UserConfig.GetDefault();
            options.Add(new Option("--ln.own_channel_config.minimum_depth", 
                $"Confirmation we will wait for before considering the channel locked in. (default: {uc.OwnChannelConfig.MinimumDepth})")
            {
                Argument = new Argument<uint> { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option("--ln.own_channel_config.our_to_self_delay", 
                $"Set to the amount of time we require our counterparty to wait to claim their money.  (default: {uc.OwnChannelConfig.OurToSelfDelay})")
            {
                Argument = new Argument<ushort> { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option("--ln.own_channel_config.our_htlc_minimum_msat", 
                $"Set to the smallest value HTLC we will accept to process. (default: {uc.OwnChannelConfig.OurHtlcMinimumMsat})")
            {
                Argument = new Argument<ulong> { Arity = ArgumentArity.ZeroOrOne }
            });

            options.Add(new Option("--ln.channel_options.fee_proportional_millionths", 
                $"Amount (in millionths of satoshi) our channels will charge per transferred satoshi (default: {uc.ChannelOptions.FeeProportionalMillionths})")
            {
                Argument = new Argument<uint> { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option("--ln.channel_options.announced_channel", 
                $"Set this to announce the channel publicly and notify all nodes that they can route via our channels. (default: {uc.ChannelOptions.AnnouncedChannel == 1})")
            {
                Argument = new Argument<bool> { Arity = ArgumentArity.Zero }
            });
            options.Add(new Option("--ln.channel_options.commit_upfront_shutdown_pubkey", 
                $"When set, we commit to an upfront shutdown_pubkey at channel open. (default: {uc.ChannelOptions.CommitUpfrontShutdownPubkey == 1})")
            {
                Argument = new Argument<bool> { Arity = ArgumentArity.ZeroOrOne }
            });
            
            #endregion
            
            # region developer options
            options.Add(new Option("--debug.http", "log every request-response in http")
            {
                Argument = new Argument<bool>
                {
                    Arity = ArgumentArity.ZeroOrOne
                }
            });
            # endregion
            
            # region lsat options
            options.Add(
                new Option("--lsat.servicename", $"service name for lsat. default is 'nrustlightning'")
                    {
                        Argument = new Argument<string>()
                        {
                            Arity = ArgumentArity.ZeroOrOne
                        }
                    }
                );
            
            options.Add(new Option("--lsat.servicetier",
                $"When you update capabilities or constraints for this service, you must increment this number. {Environment.NewLine}"+ 
                "See official LSAT spec for the detail. (default: 0)")
            {
                Argument = new Argument<int>()
                {
                    Arity = ArgumentArity.ZeroOrOne
                }
            });
            
            options.Add(new Option("--lsat.invoice.amount", "You will charge user this amount (by lsat) for giving the 'read' capability, default is '0")
            {
                Argument = new Argument<int>()
                {
                    Arity = ArgumentArity.ZeroOrOne
                }
            });
            
            options.Add(new Option("--lsat.invoice.description", "description filed for lsat invoice")
            {
                Argument = new Argument<string>()
                {
                    Arity = ArgumentArity.ZeroOrOne
                }
            });
            
            options.Add(new Option("--lsat.invoice.expirysecconds", "time before expiring invoice for lsat. default: 3600")
            {
                Argument = new Argument<int>()
                {
                    Arity = ArgumentArity.ZeroOrOne
                }
            });
            
            options.Add(new Option("--lsat.readfee", "If this value is set, user must pay this amount before "));
            # endregion
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