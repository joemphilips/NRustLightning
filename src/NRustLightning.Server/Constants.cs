using System;
using System.IO;
using System.Net;

namespace NRustLightning.Server
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
        public static string DefaultP2PHost = "127.0.0.1";
        public static int DefaultP2PPort = 9735;
        public static IPEndPoint DefaultP2PEndPoint = IPEndPoint.Parse($"{DefaultP2PHost}:{DefaultP2PPort}");
    }
}