using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Server.Tests.Stubs
{
    internal class TestFeeEstimator : IFeeEstimator
    {
        private static GetEstSatPer1000Weight _estimate = target => 1000;
        public ref GetEstSatPer1000Weight getEstSatPer1000Weight => ref _estimate;
    }
}