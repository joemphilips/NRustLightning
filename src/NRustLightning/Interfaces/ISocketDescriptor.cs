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
}