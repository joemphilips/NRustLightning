using System;
using Microsoft.Extensions.Logging;
using NBXplorer;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Server.Services;
using Org.BouncyCastle.Crypto.Tls;

namespace NRustLightning.Server.FFIProxies
{
    public class NBXplorerBroadcaster : IBroadcaster
    {
        private readonly ExplorerClient _client;
        private BroadcastTransaction _broadcastTransaction;
        public NBXplorerBroadcaster(ExplorerClient client, ILogger<NBXplorerBroadcaster> logger) {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _broadcastTransaction = (ref FFITransaction tx) =>
            {
                logger.LogDebug($"Broadcasting transaction {tx.AsTransaction(_client.Network.NBitcoinNetwork).ToHex()}");
                _client.Broadcast(tx.AsTransaction(client.Network.NBitcoinNetwork));
            };
        }
        public BroadcastTransaction BroadcastTransaction => _broadcastTransaction;
    }
}