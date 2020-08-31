using System;
using System.Runtime.InteropServices;
using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{
    /// <summary>
    /// User defined interface for estimating fee.
    /// </summary>
    public interface IFeeEstimator
    {
        uint GetEstSatPer1000Weight(FFIConfirmationTarget confirmationTarget);
    }

    internal struct FeeEstimatorDelegatesHolder : IDisposable
    {
        private readonly IFeeEstimator _feeEstimator;
        private readonly GetEstSatPer1000Weight _getEstSatPer1000Weight;

        public FeeEstimatorDelegatesHolder(IFeeEstimator feeEstimator)
        {
            _feeEstimator = feeEstimator ?? throw new ArgumentNullException(nameof(feeEstimator));
            _getEstSatPer1000Weight = feeEstimator.GetEstSatPer1000Weight;
            _handle = GCHandle.Alloc(_getEstSatPer1000Weight);
            _disposed = false;
        }
        private GCHandle _handle;
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _handle.Free();
            _disposed = true;
        }

        public GetEstSatPer1000Weight getEstSatPer1000Weight => _getEstSatPer1000Weight;
    }
}