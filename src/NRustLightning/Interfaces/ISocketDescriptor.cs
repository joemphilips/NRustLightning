using System;
using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{
    public interface ISocketDescriptor
    {
        UIntPtr Index { get; }

        bool Disconnected { get; set; }
        ref SendData SendData { get; }
        ref DisconnectSocket DisconnectSocket { get; }
    }
}