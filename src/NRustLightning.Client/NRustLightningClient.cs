using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using DotNetLightning.Payment;
using Microsoft.AspNetCore.Http;
using NBitcoin;
using NBXplorer;
using NRustLightning.Server.JsonConverters;
using NRustLightning.Server.Models;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Repository;

namespace NRustLightning.Client
{
    public class NRustLightningClient : IDisposable, INRustLightningClient
    {
        public HttpClient HttpClient;
        private Uri _baseUri;
        private string cryptoCode;

        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };

        public NRustLightningClient(string baseUrl, NRustLightningNetwork network, X509Certificate2? certificate = null)
        {
            var handler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Automatic,
            };
            if (certificate != null)
            {
                handler.ClientCertificates.Add(certificate);
            }

            cryptoCode = network.CryptoCode;
            var ser = new RepositorySerializer(network);
            ser.ConfigureSerializer(jsonSerializerOptions);
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

        public Task<InvoiceResponse> GetInvoiceAsync(InvoiceCreationOption option)
        {
            return RequestAsync<InvoiceResponse>($"/v1/payment/{cryptoCode}/invoice", HttpMethod.Post, option);
        }

        public Task<ChannelInfoResponse> GetChannelDetailsAsync()
        {
            return RequestAsync<ChannelInfoResponse>($"/v1/channel/{cryptoCode}/", HttpMethod.Get);
        }

        public Task<WalletInfo> GetWalletInfoAsync()
        {
            return RequestAsync<WalletInfo>($"/v1/wallet/{cryptoCode}", HttpMethod.Get);
        }

        public Task<GetNewAddressResponse> GetNewDepositAddressAsync()
        {
            return RequestAsync<GetNewAddressResponse>($"/v1/wallet/{cryptoCode}/address", HttpMethod.Get);
        }
        
        /// <summary>
        /// Returns Id for the opened channel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ulong> OpenChannelAsync(OpenChannelRequest request)
        {
            return RequestAsync<ulong>($"/v1/channel/{cryptoCode}", HttpMethod.Post, request);
        }

        public Task CloseChannelAsync(PubKey theirNodeId)
        {
            return RequestAsync($"/v1/channel/{cryptoCode}", HttpMethod.Delete, new CloseChannelRequest(){TheirNetworkKey = theirNodeId});
        }

        private Task RequestAsync(string relativePath, HttpMethod method, object parameters = null)
        {
            return RequestAsync<object>(relativePath, method, parameters);
        }
        

        private async Task<T> RequestAsync<T>(string relativePath, HttpMethod method, object parameters = null)
        {
            using var msg = new HttpRequestMessage();
            msg.RequestUri = new Uri(_baseUri, relativePath);
            msg.Method = method;
            if (parameters != null)
            {
                var stringContent = JsonSerializer.Serialize(parameters, jsonSerializerOptions);
                msg.Content = new StringContent(stringContent, Encoding.UTF8, "application/json");
            }
            using var resp = await HttpClient.SendAsync(msg).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                var errMsg= await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException(errMsg);
            }
            var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(content))
                return default;
            return JsonSerializer.Deserialize<T>(content, jsonSerializerOptions);
        }
    }
}