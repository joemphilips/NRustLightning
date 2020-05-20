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
using NRustLightning.Utils;

namespace NRustLightning
{
    
    public sealed class ChannelManager : IDisposable
    {
        internal readonly ChannelManagerHandle Handle;
        private readonly object[] _deps;
        internal ChannelManager(ChannelManagerHandle handle, object[] deps = null)
        {
            _deps = deps ?? new object[] {};
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
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
                        ref broadcaster.BroadcastTransaction,
                        ref logger.Log,
                        ref feeEstimator.getEstSatPer1000Weight,
                        currentBlockHeight,
                        out var handle);
                    return new ChannelManager(handle, new object[] {chainWatchInterface, logger, broadcaster, feeEstimator});
                }
            }
        }

        public void SendPayment(RoutesWithFeature routesWithFeature, Span<byte> paymentHash)
            => SendPayment(routesWithFeature, paymentHash, new byte[0]);
        public void SendPayment(RoutesWithFeature routesWithFeature, Span<byte> paymentHash, Span<byte> paymentSecret)
        {
            if (paymentSecret.Length != 0 && paymentSecret.Length != 32) throw new ArgumentException($"paymentSecret must be length of 32 or empty");
            unsafe
            {

                var routesInBytes = routesWithFeature.AsArray();
                fixed (byte* r = routesInBytes)
                fixed (byte* p = paymentHash)
                fixed (byte* s = paymentSecret)
                {
                    var route = new FFIRoute((IntPtr)r, (UIntPtr)routesInBytes.Length);
                    var ffiPaymentHash = new FFISha256dHash((IntPtr)p, (UIntPtr)paymentHash.Length);
                    var ffiPaymentSecret = new FFISecret((IntPtr)s, (UIntPtr)paymentSecret.Length);
                    Interop.send_payment(Handle, ref route, ref ffiPaymentHash, ref ffiPaymentSecret);
                }
            }
        }

        public Event[] GetAndClearPendingEvents()
        {
            Interop.get_and_clear_pending_events(Handle, out var eventsBytes);
            return Event.ParseManyUnsafe(eventsBytes.AsArray());
        }
        
        
        public void Dispose()
        {
            Handle.Dispose();
        }
    }
}