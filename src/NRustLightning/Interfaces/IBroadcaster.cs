using System;
using System.Runtime.InteropServices;
using NBitcoin;
using NRustLightning.Adaptors;
using Network = NBitcoin.Network;

namespace NRustLightning.Interfaces
{
    /// <summary>
    /// User defined broadcaster for broadcasting transaction.
    /// </summary>
    public interface IBroadcaster
    {
        void BroadcastTransaction(Transaction tx);
    }
    
    internal struct BroadcasterDelegatesHolder
    {
        private readonly IBroadcaster _broadcaster;
        private readonly Network _n;
        private readonly BroadcastTransaction _broadcastTransaction;
        public BroadcasterDelegatesHolder(IBroadcaster broadcaster, NBitcoin.Network n)
        {
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            _n = n ?? throw new ArgumentNullException(nameof(n));
            _broadcastTransaction = (ref FFITransaction ffiTx) =>
            {
                var tx = ffiTx.AsTransaction(n);
                broadcaster.BroadcastTransaction(tx);
            };
        }
        public BroadcastTransaction BroadcastTransaction => _broadcastTransaction;
    }
}