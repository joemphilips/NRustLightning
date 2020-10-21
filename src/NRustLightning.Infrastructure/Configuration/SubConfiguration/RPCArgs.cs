using System;
using System.IO;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBitcoin.RPC;
using NRustLightning.Infrastructure.Networks;

namespace NRustLightning.Infrastructure.Configuration.SubConfiguration
{
    public class RPCArgs
    {
        public Uri? Url { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
        public string? CookieFile { get; set; }
        public string? AuthenticationString { get; set; }

        public static RPCArgs Parse(IConfiguration config, Network n, string? prefix = null)
        {
            var s = config.GetSection(prefix).GetSection("rpc");
            var rpc = new RPCArgs();
            s.Bind(rpc);
            return rpc;
        }

        internal RPCClient ConfigureRPCClient(NRustLightningNetwork nRustLightningNetwork, ILogger logger)
        {
            var n = nRustLightningNetwork.NBitcoinNetwork;
            RPCClient? rpcClient = null;
            if (Url != null && User != null && Password != null)
                rpcClient = new RPCClient(new NetworkCredential(User, Password), Url, n);

            if (rpcClient is null)
            {
                if (CookieFile != null)
                {
                    try
                    {
                        rpcClient = new RPCClient(new RPCCredentialString {CookieFile = CookieFile}, Url, n);
                    }
                    catch (IOException)
                    {
                        logger.LogWarning($"{nRustLightningNetwork.CryptoCode}: RPC Cookie file not found at " + (CookieFile ?? RPCClient.GetDefaultCookieFilePath(n)));
                    }
                }

                if (AuthenticationString != null)
                {
                    rpcClient = new RPCClient(RPCCredentialString.Parse(AuthenticationString), Url, n);
                }

                if (rpcClient is null)
                {
                    try
                    {
                        rpcClient = new RPCClient(null as NetworkCredential, Url ,n);
                    }
                    catch
                    {
                        // ignored
                    }

                    if (rpcClient == null)
                    {
                        logger.LogError($"{nRustLightningNetwork.CryptoCode}: RPC connection settings not configured");
                        throw new ConfigException();
                    }
                }
            }

            return rpcClient;
        }
    }
}