using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBXplorer;
using NBXplorer.Models;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Server.Entities;

namespace NRustLightning.Server.FFIProxies
{
    public class NbXplorerFeeEstimator : IFeeEstimator
    {
        private readonly ILogger<NbXplorerFeeEstimator> _logger;
        private readonly ChannelReader<FeeRateSet> _feeRateReader;

        private object _feerateSetLock = new object();
        private FeeRateSet _cachedFeeRateSet = new FeeRateSet() { HighPriority = new FeeRate(20m), Normal = new FeeRate(10m), Background = new FeeRate(5m)};

        private FeeRateSet CachedFeeRateSet
        {
            get => _cachedFeeRateSet;
            set
            {
                lock (_feerateSetLock)
                {
                    _cachedFeeRateSet = value;
                }
            }
        }

        private Task _task;
        public NbXplorerFeeEstimator(ILogger<NbXplorerFeeEstimator> logger, ChannelReader<FeeRateSet> feeRateReader)
        {
            _logger = logger;
            _feeRateReader = feeRateReader;
            _task = Task.Run(async () => await GetAndSetFee());
        }

        private async Task GetAndSetFee()
        {
            while (await _feeRateReader.WaitToReadAsync())
            {
                CachedFeeRateSet = await _feeRateReader.ReadAsync();
            }
        }
        
        public uint GetEstSatPer1000Weight(FFIConfirmationTarget target)
        {
            // RL assumes fees for 1000 *weight-units* which is 4 times smaller than that of 1000 *virtual bytes*
            var virtualSize = 250;
            var feeRate = (uint)(target switch
            {
                FFIConfirmationTarget.Background => CachedFeeRateSet.Background.GetFee(virtualSize).Satoshi,
                FFIConfirmationTarget.Normal => CachedFeeRateSet.Normal.GetFee(virtualSize).Satoshi,
                FFIConfirmationTarget.HighPriority => CachedFeeRateSet.HighPriority.GetFee(virtualSize).Satoshi,
                _ => throw new Exception($"Unknown target type {target}")
            });
#if DEBUG
            // dirty hack to avoid nasty feerate mismatch in regtest.
            return
                target == FFIConfirmationTarget.HighPriority ? feeRate * 20u :
                target == FFIConfirmationTarget.Background ? feeRate / 2u :
                feeRate;
#else
            return feeRate;
#endif
        }
    }
}