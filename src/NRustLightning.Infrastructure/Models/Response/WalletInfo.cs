using NBXplorer.DerivationStrategy;

namespace NRustLightning.Infrastructure.Models.Response
{
    public class WalletInfo
    {
        public DerivationStrategyBase DerivationStrategy { get; set; }
        
        public long OnChainBalanceSatoshis { get; set; }
        public ulong OffChainBalanceMSat { get; set; }
    }
}