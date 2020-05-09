using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using NRustLightning.Server.Models;
using NRustLightning.Server.Models.Response;

namespace NRustLightning.Client
{
    public class NRustLightningClient : IDisposable
    {
        private HttpClient _client;
        private Uri _baseUri;

        public NRustLightningClient(string baseUrl) : this(baseUrl, null) {}
        public NRustLightningClient(string baseUrl, X509Certificate2? certificate)
        {
            var handler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Automatic,
            };
            if (certificate != null)
            {
                handler.ClientCertificates.Add(certificate);
            }

            _client = new HttpClient(handler);
            _baseUri = new Uri(baseUrl);
        }
        
        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<NodeInfo> GetInfoAsync()
        {
            using var resp = await _client.GetAsync(new Uri(_baseUri, "/v1/info")).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<NodeInfo>(content);
        }

        private async Task<T> SendAsync<T>(HttpMethod method, object body, string relativePath, object[] parameters,
            CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}