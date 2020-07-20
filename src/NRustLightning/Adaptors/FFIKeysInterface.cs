using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Adaptors
{

    [StructLayout(LayoutKind.Sequential)]
    public struct FFIChannelKeys
    {
        private Array32 FundingKey;
    }
    
    /// <summary>
    /// Get node secret key (aka node_id or network_key)
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetNodeSecret(ref byte nodeSecretPtr);
    
    /// <summary>
    /// Get destination redeemScript to encumber static protocol exit points.
    /// </summary>
    /// <param name="scriptPtr"></param>
    /// <param name="scriptLen"></param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetDestinationScript(ref byte scriptPtr, ref UIntPtr scriptLen);

    /// <summary>
    /// Get shutdown_pubkey to use as PublicKey at channel closure
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetShutdownKey(ref byte pkPtr);
    
    /// <summary>
    /// Get a new set of ChannelKeys for per-channel secrets. These MUST be unique even if you restarted with
    /// some stale data.
    /// </summary>
    /// <param name="sec"></param>
    /// <param name="seed"></param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetChannelKeys(byte inbound, ulong channelValueSatoshis, ref byte channelKeysPtr);
    
    /// <summary>
    /// Get a secret and PRNG seed for constructing an onion packet
    /// </summary>
    /// <param name="sec"></param>
    /// <param name="seed"></param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetOnionRand(ref byte secret, ref byte seed);

    /// <summary>
    /// Get a unique temporary channel id. Channels will be referred to by this until the funding
    /// transaction is created, at which point they will use the outpoint in the funding tx.
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetChannelId(ref byte temporaryChannelId);
}