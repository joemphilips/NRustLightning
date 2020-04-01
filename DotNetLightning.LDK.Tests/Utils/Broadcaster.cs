using System;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Utils;

namespace DotNetLightning.LDK.Tests.Utils
{
    public class Broadcaster : IDisposable
    {
        private readonly BroadcasterHandle _handle;

        private Broadcaster(BroadcasterHandle handle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }

        private static FFIBroadcastTransaction broadcast_ptr = (ref FFITransaction tx) =>
        {
            Console.WriteLine($"going to broadcast tx {Hex.Encode(tx.AsSpan())}");
        };
        
        public static Broadcaster Create()
        {
            var broadcast = broadcast_ptr;
            Interop.create_broadcaster(ref broadcast, out var handle);
            return new Broadcaster(handle);
        }

        public void Broadcast()
        {
            Interop.ffi_test_broadcaster(_handle);
        }
        
        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}