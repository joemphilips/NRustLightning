using System.Collections.Concurrent;
using NBitcoin;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Common.Utils
{
        public class TestBroadcaster : IBroadcaster
        {
            public ConcurrentBag<string> BroadcastedTxHex { get; } = new ConcurrentBag<string>();

            public void BroadcastTransaction(Transaction tx)
            {
                BroadcastedTxHex.Add(tx.ToHex());
            }
        }

}