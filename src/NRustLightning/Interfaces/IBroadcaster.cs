using System;
using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{
    /// <summary>
    /// User defined broadcaster for broadcasting transaction.
    /// Important: It has to hold the FFIBroadcastTransaction delegate as a static field.
    /// See BroadcasterTest for example.
    ///
    /// keep in mind that the static delegate defined in this class may call the
    /// delegate from multiple threads at the same time, so it must make thread safe,
    /// If you want to hold mutable state in it.
    /// </summary>
    public interface IBroadcaster
    {
        ref BroadcastTransaction BroadcastTransaction { get; }
    }
}