using System;
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
        public NBXplorerBroadcaster(ExplorerClient client) {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _broadcastTransaction = (ref FFITransaction tx) =>
            {
                _client.Broadcast(tx.AsTransaction(client.Network.NBitcoinNetwork));
            };
        }
        public ref BroadcastTransaction BroadcastTransaction => ref _broadcastTransaction;
    }
}