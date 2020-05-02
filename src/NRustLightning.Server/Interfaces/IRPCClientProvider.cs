using NBitcoin.RPC;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Interfaces
{
    public interface IRPCClientProvider
    {
        RPCClient? GetRpcClient(NRustLightningNetwork n);
    }
}