using System.Diagnostics;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Utils
{
    internal class TestFeeEstimator : IFeeEstimator
    {
        private static GetEstSatPer1000Weight _estimate = x =>
        {
            return 1000;
        };
        public ref GetEstSatPer1000Weight getEstSatPer1000Weight => ref _estimate;
    }
}