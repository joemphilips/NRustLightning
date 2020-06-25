using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Adaptors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void InstallWatchTx(ref FFISha256dHash txid, ref FFIScript spk);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void InstallWatchOutPoint(ref FFIOutPoint spk, ref FFIScript outScript);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WatchAllTxn();
    
    /// <summary>
    /// We want to return the `Result` type here.
    /// But since there is no straight forward way to do it, we must set value to the pointer instead.
    /// In Failure case, we just set chain error and leave other items as is.
    /// Otherwise we must never set error value and set values for other three (script, scriptLen, txOutIndex).
    /// </summary>
    /// <param name="genesisHash">input to the function: genesis hash of the chain we want to query.</param>
    /// <param name="utxoId">input to the function: ulong encoded ShortChannelId, we must get actual txout from this value.</param>
    /// <param name="error">output from the function: an error in case we failed to get the utxo</param>
    /// <param name="scriptPtr">output from the function: pointer to the script buffer.</param>
    /// <param name="scriptLen">output from the function: the length of the actual script.</param>
    /// <param name="amountSatoshis">output from the function: the output amount in satoshis</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetChainUtxo(ref FFISha256dHash genesisHash, ulong utxoId, ref ChainError error, ref byte scriptPtr, ref UIntPtr scriptLen, ref ulong amountSatoshis);
}