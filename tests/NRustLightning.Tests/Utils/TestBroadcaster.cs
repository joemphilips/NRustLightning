using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Handles;
using NRustLightning.Interfaces;
using NRustLightning.Utils;

namespace NRustLightning.Tests.Utils
{
        internal class TestBroadcaster : IBroadcaster
        {
            public ConcurrentBag<string> BroadcastedTxHex { get; } = new ConcurrentBag<string>();

            public void BroadcastTransaction(Transaction tx)
            {
                BroadcastedTxHex.Add(tx.ToHex());
            }
        }

}