using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using NBitcoin;

namespace NRustLightning.Server.Configuration
{
    public static class ConfigurationExtensions
    {
        public static T GetOrDefault<T>(this IConfiguration configuration, string key, T deafaultValue)
        {
            var str = configuration[key] ?? configuration[key.Replace(".", string.Empty)];
            if (str is null) return deafaultValue;
            if (typeof(T) == typeof(bool))
            {
                var trueValues = new[] { "1", "true" };
                var falseValues = new[] { "0", "false" };
                if (trueValues.Contains(str, StringComparer.OrdinalIgnoreCase))
                    return (T) (object) true;
                if (falseValues.Contains(str, StringComparer.OrdinalIgnoreCase))
                    return (T) (object) false;
                throw new FormatException();
            }
            if (typeof(T) == typeof(Uri))
                return (T) (object) new Uri(str, UriKind.Absolute);
            if (typeof(T) == typeof(string))
                return (T) (object) str;
            if (typeof(T) == typeof(int))
                return (T) (object) int.Parse(str, CultureInfo.InvariantCulture);

            throw new NotSupportedException("Configuration value does not support type " + typeof(T).Name);
        }

        public static IConfigurationBuilder AddCommandLineOptions(this IConfigurationBuilder config,
            ParseResult commandline)
        {
            if (commandline is null)
                throw new ArgumentNullException(nameof(commandline));

            var dict = new Dictionary<string, string>();
            foreach (var op in CommandLine.GetOptions())
            {
                if (op.Name == "nrustlightning")
                    continue;
                var s = op.Name.Replace(".", ":").Replace("_", "");
                var v = commandline.CommandResult.ValueForOption<object>(op.Name);
                if (v != null)
                {
                    dict.Add(s, v.ToString());
                }
            }
            return config.AddInMemoryCollection(dict);
        }
        
        public static string GetDefaultDataDir(this IConfiguration conf)
        {
            return GetDefaultSettings(conf).DefaultDataDir;
        }
        
        public static string GetDefaultConfigurationFile(this IConfiguration conf)
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

        public static IPEndPoint GetDefaultEndpoint(this IConfiguration conf)
        {
            return new IPEndPoint(IPAddress.Parse("127.0.0.1"), GetDefaultSettings(conf).DefaultPort);
        }
        
        private static NRustLightningDefaultSettings GetDefaultSettings(this IConfiguration conf) => NRustLightningDefaultSettings.GetDefaultSettings(GetNetworkType(conf));

        public static NetworkType GetNetworkType(this IConfiguration conf)
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
        public static string EnvironmentVariablePrefix => "_NRUSTLIGHTNING";
    }
}