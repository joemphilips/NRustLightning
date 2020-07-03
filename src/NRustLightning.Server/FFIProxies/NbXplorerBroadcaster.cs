using System;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBXplorer;
using NRustLightning.Interfaces;

namespace NRustLightning.Server.FFIProxies
{
    public class NbXplorerBroadcaster : IBroadcaster
    {
        private readonly ExplorerClient _client;
        private readonly ILogger<NbXplorerBroadcaster> _logger;
        public NbXplorerBroadcaster(ExplorerClient client, ILogger<NbXplorerBroadcaster> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger;
        }
        void IBroadcaster.BroadcastTransaction(Transaction tx)
        {
            _logger.LogDebug($"Broadcasting transaction {tx.ToHex()}");
            _client.Broadcast(tx);
        }
    }
}