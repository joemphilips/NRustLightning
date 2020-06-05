using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using DotNetLightning.Payment;
using Microsoft.AspNetCore.Http;
using NBitcoin;
using NBXplorer;
using NRustLightning.Server.JsonConverters;
using NRustLightning.Server.Models;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;

namespace NRustLightning.Client
{
    public class NRustLightningClient : IDisposable, INRustLightningClient
    {
        public HttpClient HttpClient;
        private Uri _baseUri;

        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new PaymentRequestJsonConverter()}
        };

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

            HttpClient = new HttpClient(handler);
            _baseUri = new Uri(baseUrl);
        }
        
        public void Dispose()
        {
            HttpClient.Dispose();
        }

        public Task<NodeInfo> GetInfoAsync()
        {
            return RequestAsync<NodeInfo>("/v1/info", HttpMethod.Get);
        }

        public Task<bool> ConnectAsync(PeerConnectionString connectionString)
        {
            return RequestAsync<bool>("/v1/peer/connect", HttpMethod.Post, connectionString.ToString());
        }

        public Task DisconnectAsync(PeerConnectionString connectionString)
        {
            return RequestAsync<object>("/v1/peer/disconnect", HttpMethod.Delete, connectionString.ToString());
        }

        public Task<InvoiceResponse> GetInvoiceAsync(InvoiceCreationOption option, string cryptoCode = "BTC")
        {
            return RequestAsync<InvoiceResponse>($"/v1/payment/{cryptoCode}/invoice", HttpMethod.Get, option);
        }

        public Task<ChannelInfoResponse> GetChannelDetails(string cryptoCode = "BTC")
        {
            return RequestAsync<ChannelInfoResponse>($"/v1/channel/{cryptoCode}/", HttpMethod.Get);
        }

        public Task<WalletInfo> GetWalletInfoAsync(string cryptoCode = "BTC")
        {
            return RequestAsync<WalletInfo>($"/v1/info/{cryptoCode}/wallet", HttpMethod.Get);
        }

        public Task<GetNewAddressResponse> GetNewDepositAddressAsync(string cryptoCode = "BTC")
        {
            return RequestAsync<GetNewAddressResponse>($"/v1/wallet/{cryptoCode}/address", HttpMethod.Get);
        }
        
        /// <summary>
        /// Returns Id for that channel
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cryptoCode"></param>
        /// <returns></returns>
        public Task<ulong> CreateChannel(OpenChannelRequest request, string cryptoCode = "BTC")
        {
            return RequestAsync<ulong>($"/v1/channel/{cryptoCode}", HttpMethod.Post);
        }

        private async Task<T> RequestAsync<T>(string relativePath, HttpMethod method, object parameters = null)
        {
            using var msg = new HttpRequestMessage();
            msg.RequestUri = new Uri(_baseUri, relativePath);
            msg.Method = method;
            if (parameters != null)
            {
                var stringContent = JsonSerializer.Serialize(parameters);
                msg.Content = new StringContent(stringContent, Encoding.UTF8, "application/json");
            }
            using var resp = await HttpClient.SendAsync(msg).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(content, jsonSerializerOptions);
        }
    }
}