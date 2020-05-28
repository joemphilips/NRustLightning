using NBXplorer.DerivationStrategy;

namespace NRustLightning.Server.Models.Response
{
    public class WalletInfo
    {
        public DerivationStrategyBase DerivationStrategy { get; set; }
    }
}