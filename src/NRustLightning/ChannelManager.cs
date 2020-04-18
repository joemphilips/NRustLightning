using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using DotNetLightning.Data.NRustLightningTypes;
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
        
        public void SendPayment(RouteWithFeature routesWithFeature, Span<byte> paymentHash)
        {
            unsafe
            {

                var span = routesWithFeature.AsArray();
                fixed (byte* r = span)
                fixed (byte* p = paymentHash)
                {
                    var route = new FFIRoute((IntPtr)r, (UIntPtr)span.Length);
                    var ffiPaymentHash = new FFISha256dHash((IntPtr)p, (UIntPtr)paymentHash.Length);
                    Interop.send_payment(_handle, ref route, ref ffiPaymentHash);
                }
            }
        }

        public Event[] GetAndClearPendingEvents()
        {
            Interop.get_and_clear_pending_events(_handle, out var eventsBytes);
            return Event.ParseManyUnsafe(eventsBytes.AsArray());
        }
        
        
        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}