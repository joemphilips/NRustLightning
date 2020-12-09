using NBitcoin.RPC;
using NRustLightning.Infrastructure.Networks;

namespace NRustLightning.Infrastructure.Interfaces
{
    public interface IRPCClientProvider
    {
        RPCClient? GetRpcClient(NRustLightningNetwork n);
    }
}