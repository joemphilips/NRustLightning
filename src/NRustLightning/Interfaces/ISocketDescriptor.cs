using System;
using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{
    public interface ISocketDescriptor
    {
        UIntPtr Index { get; }
        ref SendData SendData { get; }
        ref DisconnectSocket DisconnectSocket { get; }
    }
}