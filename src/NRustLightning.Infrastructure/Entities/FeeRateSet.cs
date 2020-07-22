using NBitcoin;
using NRustLightning.Infrastructure.Networks;

namespace NRustLightning.Infrastructure.Entities
{
    public class FeeRateSet
    {
        public static int HighPriorityBlockCount = 1;
        public static int NormalBlockCount = 6;
        public static int BackgroundBlockCount = 30;
        public FeeRate HighPriority { get; set; }
        public FeeRate Normal { get; set; }
        public FeeRate Background { get; set; }
        public NRustLightningNetwork Network { get; set; }
    }
}