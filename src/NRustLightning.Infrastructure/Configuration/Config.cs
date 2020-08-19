using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public string DataDir { get; set; }
        public List<ChainConfiguration> ChainConfiguration { get; } = new List<ChainConfiguration>();

        public UserConfigObject RustLightningConfig { get; set; } = new UserConfigObject();

        public string DBFilePath { get; set; }

        public int PaymentTimeoutSec { get; set; } = Constants.DefaultPaymentTimeoutSec;

        public int DBCacheMB { get; set; } = Constants.DefaultDBCacheMB;
        private ILogger? _logger;

        private string? _seedFromConfig;

        private string? _pin;
        
        public NRustLightningNetworkProvider NetworkProvider { get; set; }
        
        public string SeedFilePath { get; set; }

        public Config LoadArgs(IConfiguration config, ILogger? logger)
        {
            _logger = logger;
            var networkType = config.GetNetworkType();
            logger?.LogInformation($"Network type: {networkType}");
            NetworkProvider = new NRustLightningNetworkProvider(networkType);
            var defaultSettings = NRustLightningDefaultSettings.GetDefaultSettings(NetworkProvider.NetworkType);
            var d = config.GetOrDefault<string>("datadir", null);
            DataDir = d is null ? Path.GetDirectoryName(defaultSettings.DefaultDataDir) : Path.Join(d, NRustLightningDefaultSettings.GetFolderName(networkType));
            
            if (!Directory.Exists(DataDir))
                Directory.CreateDirectory(DataDir ?? throw new Exception("Unreachable"));

            var nbxConfig = config.GetSection("nbx");
            var nbxCookieFile =
                nbxConfig.GetOrDefault("cookiefile",
                    Constants.DefaultNbXplorerCookieFile(NetworkProvider.NetworkType));
            NBXplorerUri = new Uri(nbxConfig.GetOrDefault("rpcurl", Constants.DefaultNBXplorerUri));
            
            if (!File.Exists(nbxCookieFile))
            {
                logger?.LogWarning($"cookie file for nbxplorer does not exist in {nbxCookieFile}" +
                                  " Make sure you are running nbx with --noauth.");
            }

            logger?.LogInformation($"nbxplorer url {NBXplorerUri}");
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
            
            logger?.LogInformation($"Advertising external ip: {P2PExternalIp.ToEndpointString()}");
            logger?.LogDebug($"Network: {NetworkProvider.NetworkType.ToString()}");
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
                        logger?.LogWarning($"{n.CryptoCode}: It seems that the１ RPC port ({chainConfiguration.Rpc.Address.Port}) is equal to the default P2P port ({n.NBitcoinNetwork.DefaultPort}, this is probably a misconfiguration)");
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


            DBFilePath = Path.Combine(DataDir, "Db.dat");
            if (!Directory.Exists(DBFilePath))
                Directory.CreateDirectory(DBFilePath);
            
            PaymentTimeoutSec = config.GetOrDefault("paymenttimeout", Constants.DefaultPaymentTimeoutSec);

            DBCacheMB = config.GetOrDefault("dbcache", Constants.DefaultDBCacheMB);
            _seedFromConfig = config.GetOrDefault("seed", string.Empty);
            _pin = config.GetOrDefault("pin", string.Empty);
            SeedFilePath = Path.Join(DataDir, "node_secret");
            return this;
        }

        public async Task<string?> TryGetEncryptedSeed()
        {
            if (!File.Exists(SeedFilePath)) return null;
            _logger?.LogDebug($"reading seed from {SeedFilePath}");
            var encryptedSeed = await File.ReadAllTextAsync(SeedFilePath);
            return string.IsNullOrEmpty(encryptedSeed) ? null : encryptedSeed;
        }

        public bool HasSeedInConfig => !string.IsNullOrEmpty(_seedFromConfig);

        public string GetSeedInConfig()
        {
            Debug.Assert(HasSeedInConfig);
            return _seedFromConfig;
        }

        public string Pin => _pin;

        public string GetNewPin()
        {
            string pin = _pin;
            if (!string.IsNullOrEmpty(pin)) return pin;
            
            Console.WriteLine("========================================================================");
            Console.WriteLine("Please enter the pin code to secure your seed on disk ");
            Console.WriteLine("It is recommended to be longer than 10 characters to be cryptographically secure");
            Console.WriteLine("Please do not forget this pin code. You will need it when restarting the server");
            Console.WriteLine("========================================================================");
            while (true)
            {
                Console.WriteLine("Please enter your pin code: ");
                var pin1 = Console.ReadLine();
                Console.WriteLine("Please enter again:");
                var pin2 = Console.ReadLine();
                if (pin1 == pin2 && !string.IsNullOrEmpty(pin1))
                {
                    pin = pin1;
                    break;
                }
                else if (pin1 == pin2)
                {
                    Console.WriteLine("You cannot specify empty pin code");
                }
                else
                {
                    Console.WriteLine("pin code mismatch! try again");
                }
            }

            return pin;
        }
    }
}