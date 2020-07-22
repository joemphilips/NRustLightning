using NBitcoin.RPC;
using NRustLightning.Infrastructure.Networks;

namespace NRustLightning.Server.Interfaces
{
    public interface IRPCClientProvider
    {
        RPCClient? GetRpcClient(NRustLightningNetwork n);
    }
}