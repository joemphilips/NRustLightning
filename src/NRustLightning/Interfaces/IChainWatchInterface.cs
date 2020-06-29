using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DotNetLightning.Utils;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Utils;
using Network = NBitcoin.Network;

namespace NRustLightning.Interfaces
{
    internal interface IChainWatchInterfaceDelegatesHolder
    {
        /// <summary>
        /// Provides a txid/random-scriptPubKey-in-the-tx which must be watched for.
        /// </summary>
        InstallWatchTx InstallWatchTx { get; }
        /// <summary>
        /// Provides an outpoint which must be watched for, providing any transactions which spend the given outpoint.
        /// </summary>
        InstallWatchOutPoint InstallWatchOutPoint { get; }
        /// <summary>
        /// Indicates that a listener needs to see all transactions.
        /// </summary>
        WatchAllTxn WatchAllTxn { get; }
        /// <summary>
        /// Gets the script and value in satoshis for a given unspent transaction output given a short_channel_id
        /// (a.k.a. unspent_tx_output_identifier). For BTC/tBTC channels the top three
        /// bytes are the block height, the next 3 the transaction index within the block and the
        /// final two the output within the tx.
        /// </summary>
        GetChainUtxo GetChainUtxo { get; }
        
        FilterBlock FilterBlock { get; }
        
        ReEntered ReEntered { get; }
    }

    /// <summary>
    /// User defined interface for watching blockchain.
    ///
    /// keep in mind that the methods defined in this class may be called from
    /// multiple threads at the same time, so it must be thread safe.
    /// </summary>
    public interface IChainWatchInterface
    {
        Network Network { get; }
        void InstallWatchTxImpl(uint256 txid, Script spk);
        void InstallWatchOutPointImpl(OutPoint outpoint, Script spk);

        bool TryGetChainUtxoImpl(uint256 genesisBlockHash, ulong utxoId, ref ChainError error, out Script scriptPubKey, out Money amount);
        
        void WatchAllTxnImpl();
        
        List<uint> FilterBlockImpl(Block b);
        int ReEntered();
    }

    /// <summary>
    /// Holds user-defined chain watch interface, and translate it to ffi-compatible style.
    /// Why not use abstract class which inherits from ChainWatchInterface? because converting an instance method into
    /// delegate and passing it to rust will cause a crash with following error message.
    /// "Process terminated. A callback was made on a garbage collected delegate of type"
    /// </summary>
    internal class ChainWatchInterfaceConverter : IChainWatchInterfaceDelegatesHolder
    {

        private FilterBlock _filterBlock;
        private InstallWatchTx _installWatchTx;
        private InstallWatchOutPoint _installWatchOutPoint;
        private GetChainUtxo _getChainUtxo;
        private WatchAllTxn _watchAllTxn;
        private ReEntered _reEntered;
        
        public ChainWatchInterfaceConverter(IChainWatchInterface chainWatchInterface)
        {
            _filterBlock = (ref byte blockPtr, UIntPtr blockLen, ref UIntPtr indexPtr, ref UIntPtr indexLen) =>
            {
                unsafe
                {
                    var blockS = new Span<byte>(Unsafe.AsPointer(ref blockPtr), (int) blockLen);
                    var block = Block.Load(blockS.ToArray(), chainWatchInterface.Network);
                    
                    var indexes = chainWatchInterface.FilterBlockImpl(block).Select(uintIndex => (UIntPtr)uintIndex).ToArray();
                    Console.WriteLine($"Found {indexes.Length} matching tx");
                    if (indexes.Length == 0)
                    {
                        indexLen = UIntPtr.Zero;
                        indexPtr = UIntPtr.Zero;
                        return;
                    }
                    fixed (UIntPtr* _ = indexes)
                    {
                        Unsafe.Write(Unsafe.AsPointer(ref indexLen), (UIntPtr)indexes.Length);
                        Unsafe.CopyBlockUnaligned(
                            ref Unsafe.As<UIntPtr, byte>(ref indexPtr),
                            ref Unsafe.As<UIntPtr, byte>(ref indexes[0]),
                            (uint)(indexes.Length * Unsafe.SizeOf<UIntPtr>()));
                    }
                }
            };

            _installWatchTx = (ref FFISha256dHash txid, ref FFIScript spk) =>
            {
                chainWatchInterface.InstallWatchTxImpl(txid.ToUInt256(), spk.ToScript());
            };

            _installWatchOutPoint = (ref FFIOutPoint ffiOutPoint, ref FFIScript script) =>
            {
                var t = ffiOutPoint.ToTuple();
                var outpoint = new OutPoint(t.Item1, t.Item2);
                chainWatchInterface.InstallWatchOutPointImpl(outpoint, script.ToScript());
            };

            _getChainUtxo = (ref FFISha256dHash hash, ulong id, ref ChainError error, ref byte scriptPtr, ref UIntPtr scriptLen,
                ref ulong satoshis) =>
            {
                if (chainWatchInterface.TryGetChainUtxoImpl(hash.ToUInt256(), id, ref error, out var script, out var a))
                {
                    var scriptPubKeyBytes = script.ToBytes();
                    scriptLen = (UIntPtr) scriptPubKeyBytes.Length;
                    satoshis = (ulong) a.Satoshi;
                    Unsafe.CopyBlock(ref scriptPtr, ref scriptPubKeyBytes[0], (uint) scriptPubKeyBytes.Length);
                }
            };

            _watchAllTxn = () =>
            {
                chainWatchInterface.WatchAllTxnImpl();
            };

            _reEntered = () => (UIntPtr)chainWatchInterface.ReEntered();
        }

        
        public InstallWatchTx InstallWatchTx => _installWatchTx;
        public InstallWatchOutPoint InstallWatchOutPoint => _installWatchOutPoint;
        public WatchAllTxn WatchAllTxn => _watchAllTxn;
        public GetChainUtxo GetChainUtxo => _getChainUtxo;
        public FilterBlock FilterBlock => _filterBlock;
        public ReEntered ReEntered => _reEntered;
    }
}