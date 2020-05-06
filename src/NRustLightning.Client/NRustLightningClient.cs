using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using NRustLightning.Server.Models;
using NRustLightning.Server.Models.Response;

namespace NRustLightning.Client
{
    public class NRustLightningClient : IDisposable
    {
        private HttpClient _client;
        private Uri _baseUri;

        public NRustLightningClient(string baseUrl)
        {
            _client = new HttpClient(new HttpClientHandler());
            _baseUri = new Uri(baseUrl);
        }
        
        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<NodeInfo> GetInfo()
        {
            var resp = await _client.GetAsync(new Uri(_baseUri, "/api/v1/info")).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<NodeInfo>(content);
        }
    }
}