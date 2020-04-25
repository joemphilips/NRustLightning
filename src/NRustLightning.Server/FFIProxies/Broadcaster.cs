using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NBitcoin.RPC;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.FFIProxies
{
    public class BitcoinCoreBroadcaster : IBroadcaster
    {
        private readonly RPCClient Client;
        private BroadcastTransaction broadcastTransaction;
        public BitcoinCoreBroadcaster(RPCClient client)
        {
            Client = client;
            broadcastTransaction = (ref FFITransaction tx) => { Client.SendRawTransaction(tx.AsArray()); };
        }
        public ref BroadcastTransaction BroadcastTransaction => ref broadcastTransaction;
    }
}