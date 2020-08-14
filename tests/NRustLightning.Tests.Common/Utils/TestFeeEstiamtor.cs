using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Common.Utils
{
    public class TestFeeEstimator : IFeeEstimator
    {
        public uint GetEstSatPer1000Weight(FFIConfirmationTarget confirmationTarget)
        {
            return 1000;
        }
    }
}