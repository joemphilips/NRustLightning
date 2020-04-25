using System;

namespace NRustLightning.Server
{
    public class RPCArgs
    {
        public Uri Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CookieFile { get; set; }
        public bool NoTest { get; set; }
        public string AuthenticationString { get; set; }
    }
}