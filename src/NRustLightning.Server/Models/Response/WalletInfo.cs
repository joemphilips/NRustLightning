using System.Text.Json.Serialization;
using NBXplorer.DerivationStrategy;
using NBXplorer.JsonConverters;

namespace NRustLightning.Server.Models.Response
{
    public class WalletInfo
    {
        public DerivationStrategyBase DerivationStrategy { get; set; }
    }
}