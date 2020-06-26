using System;
using System.Runtime.InteropServices;
using NBitcoin;
using NRustLightning.Adaptors;
using Network = NBitcoin.Network;

namespace NRustLightning.Interfaces
{
    /// <summary>
    /// User defined broadcaster for broadcasting transaction.
    /// Important: It has to hold the FFIBroadcastTransaction delegate as a static field.
    /// See BroadcasterTest for example.
    ///
    /// keep in mind that the static delegate defined in this class may call the
    /// delegate from multiple threads at the same time, so it must make thread safe,
    /// If you want to hold mutable state in it.
    ///
    /// You usually want to inherit from abstract class <see cref="Broadcaster"/> instead of this.
    /// Use only when the class wants to inherit from other abstract class and thus you have to use the interface.
    /// </summary>
    public interface IBroadcaster
    {
        BroadcastTransaction BroadcastTransaction { get; }
    }

    public abstract class Broadcaster : IBroadcaster
    {
        private readonly Network _n;
        public Broadcaster(NBitcoin.Network n)
        {
            _n = n;
        }
        public abstract void BroadcastTransactionImpl(Transaction tx);

        public virtual void BroadcastTransactionCore(ref FFITransaction ffiTx)
        {
            var tx = ffiTx.AsTransaction(_n);
            BroadcastTransactionImpl(tx);
        }

        public BroadcastTransaction BroadcastTransaction => this.BroadcastTransactionCore;
    }
}