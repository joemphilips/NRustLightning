using System;

namespace NRustLightning.Infrastructure.Configuration.SubConfiguration
{
    public class HttpsConfig
    {
        public int Port { get; set;  } = Constants.DefaultHttpsPort;
        public string Cert { get; set; } = Constants.DefaultCertFile;
        public string CertPass { get; set;  } = String.Empty;
    }
}