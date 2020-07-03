using System;
using System.IO;
using System.Net;
using NBitcoin;

namespace NRustLightning.Server.Configuration
{
    public static class Constants
    {
        public static string HomePath =
            Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.GetEnvironmentVariable("%HOMEDRIVE%%HOMEPATH%");

        public static string HomeDirectoryName = ".nrustlightning";
        public static string HomeDirectoryPath = Path.Join(HomePath, HomeDirectoryName);
        public static string DataDirectoryPath = Path.Join(HomeDirectoryPath, "data");
        public static string DefaultCertFile = Path.Join(HomePath, ".aspnet", "https", "ssl.cert");
        public static string DefaultP2PHost = "127.0.0.1";
        public static int DefaultP2PPort = 9735;
        public static int DefaultHttpPort = 80;
        public static int DefaultHttpsPort = 443;
        public static string DefaultP2PExternalIpStr = $"{DefaultP2PHost}:{DefaultP2PPort}";
        public static IPEndPoint DefaultP2PExternalIp = IPEndPoint.Parse($"{DefaultP2PHost}:{DefaultP2PPort}");
        public static string DefaultNBXplorerCookieFile (NetworkType network) => $"{HomePath}/.nbxplorer/{network}/.cookie";

        public static string DefaultNBXplorerUri = "http://127.0.0.1:4774";
    }
}