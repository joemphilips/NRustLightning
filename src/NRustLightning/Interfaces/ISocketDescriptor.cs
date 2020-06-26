using System;
using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{
    public interface ISocketDescriptor
    {
        UIntPtr Index { get; }

        bool Disconnected { get; set; }
        SendData SendData { get; }
        DisconnectSocket DisconnectSocket { get; }
    }

    public abstract class SocketDescriptorBase : ISocketDescriptor
    {
        public UIntPtr Index { get; }
        public bool Disconnected { get; set; }

        public SocketDescriptorBase(uint index)
        {
            Index = (UIntPtr)index;
        }

        protected abstract UIntPtr SendDataImpl(Span<byte> data, bool resumeRead);
        
        protected virtual UIntPtr SendDataCore(FFIBytes data, byte resumeRead)
        {
                if (data.len == UIntPtr.Zero) return UIntPtr.Zero;
                return SendDataImpl(data.AsSpan(), resumeRead == 1);
        }

        protected abstract void DisconnectSocketImpl();

        public SendData SendData => SendDataCore;

        public DisconnectSocket DisconnectSocket => DisconnectSocketImpl;
    }
}