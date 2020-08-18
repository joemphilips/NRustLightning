using System;
using System.IO;
using System.Net;
using NBitcoin;

namespace NRustLightning.Infrastructure.Configuration
{
    public static class Constants
    {
        public static readonly string HomePath =
            Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.GetEnvironmentVariable("%HOMEDRIVE%%HOMEPATH%");

        public static readonly string HomeDirectoryName = ".nrustlightning";
        public static readonly string HomeDirectoryPath = Path.Join(HomePath, HomeDirectoryName);
        public static readonly string DataDirectoryPath = Path.Join(HomeDirectoryPath, "data");
        public static readonly string DefaultCertFile = Path.Combine(HomePath, ".aspnet", "https", "ssl.cert");
        public static readonly string DefaultP2PHost = "127.0.0.1";
        public static readonly int DefaultP2PPort = 9735;
        public static readonly int DefaultHttpPort = 80;
        public static readonly int DefaultHttpsPort = 443;
        public static readonly int DefaultPaymentTimeoutSec = 10;
        public static readonly string DefaultP2PExternalIpStr = $"{DefaultP2PHost}:{DefaultP2PPort}";
        public static readonly IPEndPoint DefaultP2PExternalIp = (IPEndPoint)NBitcoin.Utils.ParseEndpoint(DefaultP2PHost, DefaultP2PPort);
        public static string DefaultNbXplorerCookieFile (NetworkType network) => $"{HomePath}/.nbxplorer/{network}/.cookie";

        public static readonly string DefaultNBXplorerUri = "http://127.0.0.1:4774";

        public static readonly int DefaultDBCacheMB = 600;
    }
}