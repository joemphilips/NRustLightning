using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using NRustLightning.Adaptors;
using NRustLightning.Facades;
using NRustLightning.Handles;
using NRustLightning.Interfaces;

namespace NRustLightning
{
    
    public sealed class ChannelManager : IDisposable
    {
        private readonly ChannelManagerHandle _handle;
        internal ChannelManager(ChannelManagerHandle handle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }
        
        public static ChannelManager Create(
            Span<byte> seed,
            in Network network,
            in UserConfig config,
            IChainWatchInterface chainWatchInterface,
            ILogger logger,
            IBroadcaster broadcaster,
            IFeeEstimator feeEstimator,
            ulong currentBlockHeight
            )
        {
            unsafe
            {
                fixed (byte* b = seed)
                fixed (Network* n = &network)
                fixed (UserConfig* configPtr = &config)
                {
                    Interop.create_ffi_channel_manager(
                        b,
                        (UIntPtr)seed.Length,
                        n,
                        configPtr,
                        ref chainWatchInterface.InstallWatchTx,
                        ref chainWatchInterface.InstallWatchOutPoint,
                        ref chainWatchInterface.WatchAllTxn,
                        ref chainWatchInterface.GetChainUtxo,
                        ref chainWatchInterface.FilterBlock,
                        ref broadcaster.BroadcastTransaction,
                        ref logger.Log,
                        ref feeEstimator.getEstSatPer1000Weight,
                        currentBlockHeight,
                        out var handle);
                    return new ChannelManager(handle);
                }
            }
        }
        
        public void SendPayment(Route routes, ref FFISha256dHash paymentHash)
        {
            unsafe
            {

                var span = routes.AsSpan();
                fixed (byte* r = span)
                fixed (FFISha256dHash* _ = &paymentHash)
                {
                    var route = new FFIRoute((IntPtr)r, (UIntPtr)span.Length);
                    Interop.send_payment(_handle, ref route, ref paymentHash);
                }
            }
        }
        
        
        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}