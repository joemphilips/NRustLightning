using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBitcoin.DataEncoders;
using NRustLightning.Infrastructure.Configuration.SubConfiguration;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Utils;
using NRustLightning.Interfaces;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NRustLightning.Infrastructure.Configuration
{
    public class Config
    {
        public EndPoint P2PExternalIp { get; set; } = Constants.DefaultP2PExternalIp;
        public Uri NBXplorerUri { get; set; } = new Uri(Constants.DefaultNBXplorerUri);
        public string? NBXCookieFile = null;
        public string ConfigurationFile { get; set; } = "nrustlightning.conf";
        public string DataDir { get; set; } = Constants.DataDirectoryPath;
        public List<ChainConfiguration> ChainConfiguration { get; } = new List<ChainConfiguration>();

        public UserConfigObject RustLightningConfig { get; set; } = new UserConfigObject();

        public Func<Task<byte[]>>? GetSeed;
        
        public string InvoiceDBFilePath { get; set; }

        public int PaymentTimeoutSec { get; set; } = Constants.DefaultPaymentTimeoutSec;

        public int DBCacheMB { get; set; } = Constants.DefaultDBCacheMB;
        
        public NRustLightningNetworkProvider NetworkProvider { get; set; }

        public Config LoadArgs(IConfiguration config, ILogger? logger)
        {
            var networkType = config.GetNetworkType();
            logger.LogInformation($"Network type: {networkType}");
            NetworkProvider = new NRustLightningNetworkProvider(networkType);
            var defaultSettings = NRustLightningDefaultSettings.GetDefaultSettings(NetworkProvider.NetworkType);
            DataDir = config.GetOrDefault<string>("datadir", null);
            if (DataDir is null)
            {
                DataDir = Path.GetDirectoryName(defaultSettings.DefaultDataDir);
                if (!Directory.Exists(DataDir))
                    Directory.CreateDirectory(DataDir);
                if (!Directory.Exists(defaultSettings.DefaultDataDir))
                    Directory.CreateDirectory(defaultSettings.DefaultDataDir);
            }

            var nbxConfig = config.GetSection("nbx");
            var nbxCookieFile =
                nbxConfig.GetOrDefault("cookiefile",
                    Constants.DefaultNBXplorerCookieFile(NetworkProvider.NetworkType));
            NBXplorerUri = new Uri(nbxConfig.GetOrDefault("rpcurl", Constants.DefaultNBXplorerUri));
            
            if (!File.Exists(nbxCookieFile))
            {
                logger.LogWarning($"cookie file for nbxplorer does not exist in {nbxCookieFile}" +
                                  " Make sure you are running nbx with --noauth.");
            }

            logger.LogInformation($"nbxplorer url {NBXplorerUri}");
            NBXCookieFile = nbxCookieFile;

            var p2pExternalIp = config.GetOrDefault("externalip", Constants.DefaultP2PExternalIpStr);
            if (NBitcoin.Utils.TryParseEndpoint(p2pExternalIp, Constants.DefaultP2PPort, out var ip))
            {
                P2PExternalIp = ip;
            }
            else if (p2pExternalIp.Contains(":"))
            {
                var s = p2pExternalIp.Split(":", StringSplitOptions.RemoveEmptyEntries);
                if (s.Length != 2)
                {
                    throw new ConfigException($"Invalid external ip {p2pExternalIp}");
                }

                if (Int32.TryParse(s[1], out var port))
                {
                    P2PExternalIp = new DnsEndPoint(s[0], port);
                }
                else
                {
                    throw new ConfigException($"Invalid external ip {p2pExternalIp}");
                }
            }
            else
                throw new ConfigException($"Invalid external ip {p2pExternalIp}");
            
            logger.LogInformation($"Advertising external ip: {P2PExternalIp.ToEndpointString()}");
            logger.LogDebug($"Network: {NetworkProvider.NetworkType.ToString()}");
            var supportedChains = config.GetOrDefault<string>("chains", "BTC")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLowerInvariant());
            var validChains = new List<string>();
            foreach (var n in NetworkProvider.GetAll())
            {
                if (supportedChains.Contains(n.CryptoCode))
                {
                    validChains.Add(n.CryptoCode);
                    var chainConfiguration = new ChainConfiguration();
                    chainConfiguration.CryptoCode = n.CryptoCode;
                    var args = RPCArgs.Parse(config, n.NBitcoinNetwork, n.CryptoCode);
                    chainConfiguration.Rpc = args.ConfigureRPCClient(n, logger);
                    if (chainConfiguration.Rpc.Address.Port == n.NBitcoinNetwork.DefaultPort)
                    {
                        logger.LogWarning($"{n.CryptoCode}: It seems that the１ RPC port ({chainConfiguration.Rpc.Address.Port}) is equal to the default P2P port ({n.NBitcoinNetwork.DefaultPort}, this is probably a misconfiguration)");
                    }
                    if((chainConfiguration.Rpc.CredentialString.CookieFile != null || chainConfiguration.Rpc.CredentialString.UseDefault) && !n.SupportCookieAuthentication)
                    {
                        throw new ConfigException($"Chain {n.CryptoCode} does not support cookie file authentication,\n" +
                                                  $"Please use {n.CryptoCode.ToLowerInvariant()}rpcuser and {n.CryptoCode.ToLowerInvariant()}rpcpassword settings in NRustLightning" +
                                                  $"And configure rpcuser and rpcpassword in the configuration file or in commandline or your node");
                    }
                    ChainConfiguration.Add(chainConfiguration);
                }
            }
            var invalidChains = String.Join(',', supportedChains.Where(s => !validChains.Contains(s)));
            if(!string.IsNullOrEmpty(invalidChains))
                throw new ConfigException($"Invalid chains {invalidChains}");

            config.GetSection("ln").Bind(RustLightningConfig);

            string? seed = null;
            var filePath = Path.Join(DataDir, "node_secret");
            if (File.Exists(filePath))
            {
                logger.LogDebug($"reading seed from {filePath}");
                seed = File.ReadAllText(filePath);
            }
            if (seed is null)
                seed = config.GetOrDefault("seed", String.Empty);
            if (String.IsNullOrEmpty(seed))
            {
                logger.LogWarning($"seed not found in {filePath}! You can specify it with --seed option.");
                logger.LogInformation("generating new seed...");
                seed = RandomUtils.GetUInt256().ToString();
            }

            InvoiceDBFilePath = Path.Combine(DataDir, "InvoiceDb");
            if (!Directory.Exists(InvoiceDBFilePath))
                Directory.CreateDirectory(InvoiceDBFilePath);
            
            var h = new HexEncoder();
            if (!(h.IsValid(seed) && seed.Length == 64))
            {
                throw new NRustLightningException($"Seed corrupted {seed}");
            }
            File.WriteAllText(filePath, seed);
            GetSeed = async () => {
                var s = await File.ReadAllTextAsync(filePath);
                return h.DecodeData(s);
            };

            PaymentTimeoutSec = config.GetOrDefault("paymenttimeout", Constants.DefaultPaymentTimeoutSec);

            DBCacheMB = config.GetOrDefault("dbcache", Constants.DefaultDBCacheMB);
            return this;
        }
    }
}