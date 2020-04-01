using System;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Utils;

namespace DotNetLightning.LDK.Tests.Utils
{
    public class BroadcasterWrapper : IDisposable
    {
        private readonly BroadcasterHandle _handle;

        private BroadcasterWrapper(BroadcasterHandle handle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }

        private static FFIBroadcastTransaction broadcast_ptr = (tx) =>
        {
            Console.WriteLine($"tx is {Hex.Encode(tx.AsSpan())}");
        };
        
        public static BroadcasterWrapper Create()
        {
            var broadcast = broadcast_ptr;
            Interop.create_broadcaster(ref broadcast, out var handle);
            return new BroadcasterWrapper(handle);
        }
        
        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}