using Microsoft.Extensions.DependencyInjection;
using NRustLightning.Server.Models.Request;

namespace NRustLightning.Client
{
    public static class Extensions
    {
        public static IHttpClientBuilder AddNRustLightningClient(this IServiceCollection services)
        {
            return services.AddHttpClient<INRustLightningClient, NRustLightningClient>();
        }
    }
}