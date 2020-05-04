using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CommandLine;
using Microsoft.Extensions.Configuration;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Server.Networks;
using Network = NBitcoin.Network;

namespace NRustLightning.Server.Configuration
{
    public class Config
    {
        public IPEndPoint P2PExternalIp { get; set; } = Constants.DefaultP2PExternalIp;
        public Uri NBXplorerUri { get; set; } = new Uri("http://127.0.0.1:4774");
        public string ConfigurationFile { get; set; } = "nrustlightning.conf";
        public string DataDir { get; set; } = Constants.DataDirectoryPath;
        public RPCArgs RpcArgs { get; set; }
        public List<ChainConfiguration> ChainConfiguration { get; } = new List<ChainConfiguration>();
        
        public UserConfig RustLightningConfig { get; }
    }
    
    public class DefaultConfiguration : StandardConfiguration.DefaultConfiguration
    {
        protected override CommandLineApplication CreateCommandLineApplicationCore()
        {
            var provider = new NRustLightningNetworkProvider(NetworkType.Mainnet);
            var chains = string.Join(",", provider.GetAll().Select(n => n.CryptoCode.ToLowerInvariant().ToArray()));
            CommandLineApplication app = new CommandLineApplication(true)
            {
                FullName =  "NRustLightning\r\nBolt-Compatible Lightning Network node which uses rust-lightning internally",
                Name = "NRustLightning"
            };
            app.HelpOption("-? | -h | --help");
            app.Option("-n | --network", "Set the network among (mainnet, testnet, regtest) (default:mainnet)", CommandOptionType.SingleValue);
            app.Option("--testnet | -testnet", $"Use testnet", CommandOptionType.BoolValue);
            app.Option("--regtest | -regtest", "Use regtest", CommandOptionType.BoolValue);
            app.Option("--chains", $"Chains to support comma separated (default: btc, available: {chains})", CommandOptionType.SingleValue);

            foreach (var n in provider.GetAll())
            {
                var crypto = n.CryptoCode.ToLowerInvariant();
                app.Option($"--{crypto}rpcuser", "RPC authentication method 1: The RPC user (default: using cookie auth from default network folder)", CommandOptionType.SingleValue);
                app.Option($"--{crypto}rpcpassword", "RPC authentication method 1: The RPC password (default: using cookie auth from default network folder)", CommandOptionType.SingleValue);
                app.Option($"--{crypto}rpccookiefile", $"RPC authentication method 2: The RPC cookiefile (default: using cookie auth from default network folder)", CommandOptionType.SingleValue);
                app.Option($"--{crypto}rpcauth", $"RPC authentication method 3: user:password or cookiefile=path (default: using cookie auth from default network folder)", CommandOptionType.SingleValue);
                app.Option($"--{crypto}rpcurl", $"The RPC server url (default: rpc server depended on the network)", CommandOptionType.SingleValue);
            }

            return app;
        }

        protected override string GetDefaultDataDir(IConfiguration conf)
        {
            return GetDefaultSettings(conf).DefaultDataDir;
        }

        protected override string GetDefaultConfigurationFile(IConfiguration conf)
        {
            var network = GetDefaultSettings(conf);
            var dataDir = conf["datadir"];
            if (dataDir is null)
                return network.DefaultConfigurationFile;
            var fileName = Path.GetFileName(network.DefaultConfigurationFile);
            var chainDir = Path.GetFileName(Path.GetDirectoryName(network.DefaultConfigurationFile));
            chainDir = Path.Combine(dataDir, chainDir);
            try
            {
                if (!Directory.Exists(chainDir))
                    Directory.CreateDirectory(chainDir);
            }
            catch
            {
                // ignored
            }

            return Path.Combine(chainDir, fileName);
        }
        
        protected override string GetDefaultConfigurationFileTemplate(IConfiguration conf)
        {
            var settings = GetDefaultSettings(conf);
            var networkType = GetNetworkType(conf);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("### Global settings ###");
            sb.AppendLine("#network=mainnet#");

            sb.AppendLine("### Server settings ###");
            throw new NotImplementedException();
        }

        protected override IPEndPoint GetDefaultEndpoint(IConfiguration conf)
        {
            throw new NotImplementedException();
        }
        
        private NRustLightningDefaultSettings GetDefaultSettings(IConfiguration conf) => NRustLightningDefaultSettings.GetDefaultSettings(GetNetworkType(conf));

        public static NetworkType GetNetworkType(IConfiguration conf)
        {
            var network = conf.GetOrDefault<string>("network", null);
            if (!(network is null))
            {
                var n = Network.GetNetwork(network);
                if (n is null)
                    throw new ConfigException($"Invalid network parameter '{network}'");

                return n.NetworkType;
            }

            return
                conf.GetOrDefault("regtest", false) ? NetworkType.Regtest :
                conf.GetOrDefault("testnet", false) ? NetworkType.Testnet :
                NetworkType.Mainnet;
        }
        public override string EnvironmentVariablePrefix => "_NRUSTLIGHTNING";
    }
}