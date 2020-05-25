using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NRustLightning.Client;

namespace NRustLightning.Server.Tests.Support
{
    public static class HostExtensions
    {
        public static NRustLightningClient GetNRustLightningClient(this IHost host)
        {
            var httpClient = host.GetTestClient();
            var client = new NRustLightningClient(httpClient.BaseAddress.ToString());
            client.HttpClient = httpClient;
            return client;
        }
    }
}