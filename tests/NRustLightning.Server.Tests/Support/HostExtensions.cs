using System.Linq;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NBitcoin;
using NRustLightning.Client;
using NRustLightning.Infrastructure.Networks;

namespace NRustLightning.Server.Tests.Support
{
    public static class HostExtensions
    {
        public static NRustLightningClient GetTestNRustLightningClient(this IHost host)
        {
            var httpClient = host.GetTestClient();
            var client = new NRustLightningClient(httpClient.BaseAddress.ToString(), new NRustLightningNetworkProvider(NetworkType.Regtest).GetByCryptoCode("BTC"));
            client.HttpClient = httpClient;
            return client;
        }

        public static NRustLightningClient GetTestNRustLightningClient(this CustomWebApplicationFactory factory)
        {
            var httpClient = factory.CreateClient();
            var client = new NRustLightningClient(httpClient.BaseAddress.ToString(), new NRustLightningNetworkProvider(NetworkType.Regtest).GetByCryptoCode("BTC"));
            client.HttpClient = httpClient;
            return client;
        }
        
        /// <summary>
        /// Removes all registered <see cref="ServiceLifetime.Transient"/> registrations of <see cref="TService"/> and adds in <see cref="TImplementation"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service interface which needs to be placed.</typeparam>
        /// <typeparam name="TImplementation">The test or mock implementation of <see cref="TService"/> to add into <see cref="services"/>.</typeparam>
        /// <param name="services"></param>
        public static void SwapTransient<TService, TImplementation>(this IServiceCollection services) 
            where TImplementation : class, TService
        {
            if (services.Any(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient))
            {
                var serviceDescriptors = services.Where(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient).ToList();
                foreach (var serviceDescriptor in serviceDescriptors)
                {
                    services.Remove(serviceDescriptor);
                }
            }

            services.AddTransient(typeof(TService), typeof(TImplementation));
        }
    }
}