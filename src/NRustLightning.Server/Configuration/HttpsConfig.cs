using System;

namespace NRustLightning.Server.Configuration
{
    public class HttpsConfig
    {
        public int Port { get; } = 443;
        public string CertName { get; } = "ssl.cert";
        public string CertPass { get; } = String.Empty;
    }
}