using System;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Primitives;

namespace DotNetLightning.LDK
{
    public sealed class ChannelManager : IDisposable
    {
        private readonly ChannelManagerHandle _handle;
        internal ChannelManager(ChannelManagerHandle handle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }
        public static void Create(UserConfig config) {}
        
        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}