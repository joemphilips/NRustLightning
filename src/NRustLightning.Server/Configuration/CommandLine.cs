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
            options.Add(new Option<uint>("--ln.own_channel_config.minimum_depth", 
                $"Confirmation we will wait for before considering the channel locked in. (default: {uc.OwnChannelConfig.MinimumDepth})")
            {
                Argument = new Argument<uint> { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<ushort>("--ln.own_channel_config.our_to_self_delay", 
                $"Set to the amount of time we require our counterparty to wait to claim their money.  (default: {uc.OwnChannelConfig.OurToSelfDelay})")
            {
                Argument = new Argument<ushort> { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<ulong>("--ln.own_channel_config.our_htlc_minimum_msat", 
                $"Set to the smallest value HTLC we will accept to process. (default: {uc.OwnChannelConfig.OurHtlcMinimumMsat})")
            {
                Argument = new Argument<ulong> { Arity = ArgumentArity.ZeroOrOne }
            });
            
            
            options.Add(new Option<ulong>("--ln.peer_channel_config_limits.min_funding_satoshis", $"Minimum allowed satoshis when a channel is funded, this is supplied by the sender and only applies to inbound channels. (default: {uc.PeerChannelConfigLimits.MinFundingSatoshis})")
            {
                Argument = new Argument<ulong>() { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<ulong>("--ln.peer_channel_config_limits.max_htlc_minimum_msat", $"The remote node sets a limit on the minimum size of HTLCs we can send to them. This allows you to limit the maximum minimum-size they can require. (default: {uc.PeerChannelConfigLimits.MaxHtlcMinimumMsat})")
            {
                Argument = new Argument<ulong>() { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<ulong>("--ln.peer_channel_config_limits.min_max_htlc_value_inflight_msat", $"The remote sets a limit on the maximum value of pending HTLCs to the mat any given time to limit their funds exposure to HTLCs. This allows you to set a minimum such value. (default: {uc.PeerChannelConfigLimits.MinMaxHtlcValueInFlightMsat})")
            {
                Argument = new Argument<ulong>() { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<ulong>("--ln.peer_channel_config_limits.max_channel_reserve_satoshis", $"The remote node will require we keep a certain amount in direct payment to ourselves at all time,  ensuring that we are able to be punished if we broadcast an old state. This allows to you limit the amount which will have to keep to ourselves. This allows to you limit the amount which we will have to keep to ourselves (and cannot use for HTLCs.) (default: {uc.PeerChannelConfigLimits.MinMaxAcceptedHtlcs})" )
            {
                Argument = new Argument<ulong>() { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<ushort>("--ln.peer_channel_config_limits.min_max_accepted_htlcs", $"The remote node sets a limit on the maximum number of pending HTLCs to them at any given time. This allows you to set a minimum such value. (default: {uc.PeerChannelConfigLimits.MinMaxAcceptedHtlcs})" )
            {
                Argument = new Argument<ushort>() { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<ulong>("--ln.peer_channel_config_limits.min_dust_limit_satoshis", $@"Outputs below a certain value will not be added to on-chain txs. The dust value is required to always be higher than this value so this only applies to HTLC outputs (and potentially to-self outputs before any payments have been made
thus, htlcs below this amount plus htlc transaction fees are not enforceable on-chain.
this setting allows you to set a minimum dust limit for their commitment transactions,
reflecting the reality that tiny outputs are not considered standard transactions and will
not propagate through the Bitcoin network.
(default: {uc.PeerChannelConfigLimits.MinDustLimitSatoshis})")
            {
                Argument = new Argument<ulong>() { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<ulong>("--ln.peer_channel_config_limits.max_dust_limit_satoshis", $@"Maximum allowed threshold above which outputs will not be generated in their commitment
transactions.
HTLCs below this amount plus HTLC transaction fees are not enforceable on-chain.
(default: {uc.PeerChannelConfigLimits.MaxDustLimitSatoshis})")
            {
                Argument = new Argument<ulong>() { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<uint>("--ln.peer_channel_config_limits.max_minimum_depth", $@"Before a channel is usable the funding transaction will need to be confirmed by at least a
certain number of blocks, specified by the node which is not the funder (as the funder can
assume they aren't going to double-spend themselves).
This config allows you to set a limit on the maximum amount of time to wait.
(default: {uc.PeerChannelConfigLimits.MaxMinimumDepth})")
            {
                Argument = new Argument<uint>() { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<bool>("--ln.peer_channel_config_limits.force_announced_channel_preference", $"Set to force the incoming channel to match our announced channel preference in ChannelConfig (default: {uc.PeerChannelConfigLimits.ForceAnnouncedChannelPreference})")
            {
                Argument = new Argument<bool> { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<ushort>("--ln.peer_channel_config_limits.their_to_self_delay", $@"Set to the amount of time we're willing to wait to claim money back to us.
Not checking this value would be a security issue, as our peer would be able to set it to
max relative lock-time (a year) and we would 'lose' money as it would be locked for a long time.
(default: {uc.PeerChannelConfigLimits.TheirToSelfDelay})")
            {
                Argument = new Argument<ushort> { Arity = ArgumentArity.ZeroOrOne }
            });
            

            options.Add(new Option<uint>("--ln.channel_options.fee_proportional_millionths", 
                $"Amount (in millionths of satoshi) our channels will charge per transferred satoshi (default: {uc.ChannelOptions.FeeProportionalMillionths})")
            {
                Argument = new Argument<uint> { Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option<bool>("--ln.channel_options.announced_channel", 
                $"Set this to announce the channel publicly and notify all nodes that they can route via our channels. (default: {uc.ChannelOptions.AnnouncedChannel})")
            {
                Argument = new Argument<bool>{ Arity = ArgumentArity.ZeroOrOne }
            });
            options.Add(new Option("--ln.channel_options.commit_upfront_shutdown_pubkey", 
                $"When set, we commit to an upfront shutdown_pubkey at channel open. (default: {uc.ChannelOptions.CommitUpfrontShutdownPubkey})")
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