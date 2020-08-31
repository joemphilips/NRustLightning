using System;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBXplorer;
using NRustLightning.Interfaces;
using NRustLightning.Server.Events;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.FFIProxies
{
    public class NbXplorerBroadcaster : IBroadcaster
    {
        private readonly ExplorerClient _client;
        private readonly ILogger<NbXplorerBroadcaster> _logger;
        private readonly EventAggregator _eventAggregator;

        public NbXplorerBroadcaster(ExplorerClient client, ILogger<NbXplorerBroadcaster> logger, EventAggregator eventAggregator)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger;
            _eventAggregator = eventAggregator;
        }
        void IBroadcaster.BroadcastTransaction(Transaction tx)
        {
            _eventAggregator.Publish(new TxBroadcastEvent { Tx = tx });
            _logger.LogDebug($"Broadcasting transaction {tx.ToHex()}");
            _client.Broadcast(tx);
        }
    }
}