using System;
using NBitcoin;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NBXplorer;
using NRustLightning.Adaptors;
using Network = NBitcoin.Network;

namespace NRustLightning.Server.Configuration
{
    public class NRustLightningDefaultSettings
    {
        private static Dictionary<NetworkType, NRustLightningDefaultSettings> _Settings;

        static NRustLightningDefaultSettings()
        {
            _Settings = new Dictionary<NetworkType, NRustLightningDefaultSettings>();
            foreach (var nType in new [] { NetworkType.Mainnet, NetworkType.Testnet, NetworkType.Regtest })
            {
                var settings = new NRustLightningDefaultSettings();
                _Settings.Add(nType, settings);
                settings.DefaultDataDir = StandardConfiguration.DefaultDataDirectory.GetDirectory("NRustLightning", GetFolderName(nType), false);
                settings.DefaultConfigurationFile = Path.Combine(settings.DefaultDataDir, "nrustlightning.conf");
                settings.DefaultCookieFile = Path.Combine(settings.DefaultDataDir, ".cookie");
                settings.NBXplorerSettings = NBXplorerDefaultSettings.GetDefaultSettings(nType);
                settings.DefaultPort = Constants.DefaultP2PPort;
                settings.RustLightningConfig = new UserConfig();
            }
        }

        public static string GetFolderName(NetworkType nType)
            => nType switch
            {
                NetworkType.Mainnet => "Main",
                NetworkType.Testnet => "Testnet",
                NetworkType.Regtest => "RegTest",
                _ => throw new NotSupportedException()
            };
        
        public IPEndPoint P2PExternalIp { get;  private set; } = Constants.DefaultP2PExternalIp;
        public NBXplorerDefaultSettings NBXplorerSettings { get;  private set; }
        public string DefaultConfigurationFile { get; private set; } = "nrustlightning.conf";
        public string DefaultDataDir { get;  private set; } = Constants.DataDirectoryPath;
        public string DefaultCookieFile { get;  private set; }
        public int DefaultPort { get; set; }
        
        public UserConfig RustLightningConfig { get; private set; }
        public static NRustLightningDefaultSettings GetDefaultSettings(NetworkType nType) => _Settings[nType];
    }
}