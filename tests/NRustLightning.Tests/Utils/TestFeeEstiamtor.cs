using System.Diagnostics;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Utils
{
    internal class TestFeeEstimator : IFeeEstimator
    {
        public uint GetEstSatPer1000Weight(FFIConfirmationTarget confirmationTarget)
        {
            return 1000;
        }
    }
}