using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Server.Tests.Stubs
{
    internal class TestFeeEstimator : IFeeEstimator
    {
        public uint GetEstSatPer1000Weight(FFIConfirmationTarget confirmationTarget)
        {
            return 1000;
        }
    }
}