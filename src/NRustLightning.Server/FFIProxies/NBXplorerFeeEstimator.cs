using System;
using System.Diagnostics;
using NBXplorer;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Server.FFIProxies
{
    public class NBXplorerFeeEstimator : IFeeEstimator
    {
        private readonly ExplorerClient _client;
        private GetEstSatPer1000Weight _getEstSatPer1000Weight;
        public NBXplorerFeeEstimator(ExplorerClient client) {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _getEstSatPer1000Weight = (target) =>
            {
                var blockCountTarget =
                    target switch
                    {
                        FFIConfirmationTarget.Background => 30,
                        FFIConfirmationTarget.Normal => 6,
                        FFIConfirmationTarget.HighPriority => 1,
                        _ => throw new Exception("Unreachable!")
                    };
                var resp = _client.GetFeeRate(blockCountTarget);
                if (resp is null)
                {
                    throw new Exception("resp was null");
                }
                var virtualSize = 1000;
                return (ulong)resp.FeeRate.GetFee(virtualSize).Satoshi;
            };
        }
        public ref GetEstSatPer1000Weight getEstSatPer1000Weight => ref _getEstSatPer1000Weight;
    }
}