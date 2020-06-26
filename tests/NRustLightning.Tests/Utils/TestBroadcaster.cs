using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NRustLightning.Adaptors;
using NRustLightning.Handles;
using NRustLightning.Interfaces;
using NRustLightning.Utils;

namespace NRustLightning.Tests.Utils
{
        internal class TestBroadcaster : IBroadcaster
        {
            public ConcurrentBag<string> BroadcastedTxHex { get; } = new ConcurrentBag<string>();

            public BroadcastTransaction _broadcast_ptr;

            public TestBroadcaster()
            {
                _broadcast_ptr =
                    (ref FFITransaction tx) =>
                     {
                         var hex = Hex.Encode(tx.AsSpan());
                         BroadcastedTxHex.Add(hex);
                     };
            }
            BroadcastTransaction IBroadcaster.BroadcastTransaction
                => _broadcast_ptr;
        }

}