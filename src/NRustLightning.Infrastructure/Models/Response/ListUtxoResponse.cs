using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using NBitcoin;
using NBXplorer.Models;
using NRustLightning.Infrastructure.JsonConverters;
using NRustLightning.RustLightningTypes;
using DateTimeToUnixTimeConverter = NBitcoin.JsonConverters.DateTimeToUnixTimeConverter;

namespace NRustLightning.Infrastructure.Models.Response
{
    [JsonConverter(typeof(UTXOKindJsonConverter))]
    public enum UTXOKind
    {
        /// <summary>
        /// User-deposited funds
        /// </summary>
        UserDeposit,
        
        /// <summary>
        /// Funds that has been spent to `IKeysInterface.GetDestination()` script.
        /// </summary>
        CooperativelyClosed,
        
        /// <summary>
        /// OP_CSV-locked P2WSH output. Which remote can claim if the state is old (which should never happen from
        /// our point of view)
        /// </summary>
        LocalForceClosed,
        
        /// <summary>
        ///  P2WPKH output which resulted from channels force-closed by remote
        /// </summary>
        RemoteForceClosed
    }

    public class UTXO
    {
        public UTXO(NBXplorer.Models.UTXO utxo)
        {
            Confirmations = utxo.Confirmations;
            KeyPath = utxo.KeyPath;
            Timestamp = utxo.Timestamp;
            
            Index = utxo.Index;
            ScriptPubKey = utxo.ScriptPubKey;
            if (utxo.Value is Money m)
            {
                ValueInSatoshi = m.Satoshi;
            }
            TransactionHash = utxo.TransactionHash;
            Outpoint = utxo.Outpoint;
        }

        [JsonConverter(typeof(JsonConverters.NBitcoinTypes.DateTimeToUnixTimeConverter))]
        public DateTimeOffset Timestamp { get; set; }

        public KeyPath KeyPath { get; set; }

        public int Confirmations { get; set; }

        public Script ScriptPubKey { get; set; }

        public long? ValueInSatoshi { get; set; }
        public uint256 TransactionHash { get; set; }

        public int Index { get; set; }

        public OutPoint Outpoint { get; set; }
    }

    public class UTXOChangeWithMetadata
    {
        public UTXOKind Kind { get; set; }
        public TrackedSource Source { get; set; }
        
        public UTXO Utxo { get; set; }
        public UTXOChangeWithMetadata(UTXO utxo, UTXOKind kind, TrackedSource source)
        {
            Utxo = utxo ?? throw new ArgumentNullException(nameof(utxo));
            Kind = kind;
            Source = source;
        }
    }

    public static class SpendableOutputExtensions
    {
        public static UTXOKind GetKind(this SpendableOutputDescriptor desc) => desc switch
        {
            SpendableOutputDescriptor.StaticOutput _ => UTXOKind.CooperativelyClosed,
            SpendableOutputDescriptor.DynamicOutputP2WSH _ =>  UTXOKind.LocalForceClosed,
            SpendableOutputDescriptor.StaticOutputRemotePayment _ => UTXOKind.RemoteForceClosed,
            _ => throw new Exception("Unreachable"),
        };

        public static string ToFormatString(this UTXOKind kind) => kind switch
        {
            UTXOKind.UserDeposit => "UserDeposit",
            UTXOKind.CooperativelyClosed => "CooperativelyClosed",
            UTXOKind.LocalForceClosed => "LocalForceClosed",
            UTXOKind.RemoteForceClosed => "RemoteForceClosed"
        };
    }
    
    public class UTXOChangeWithSpentOutput
    {
        public List<UTXOChangeWithMetadata> UTXO { get; set; }
        public List<OutPoint> SpentOutPoint { get; set; }
    }

    // [JsonConverter(typeof(UTXOChangesWithMetadataJsonConverter))]
    public class UTXOChangesWithMetadata
    {
        public UTXOChangeWithSpentOutput Confirmed { get; set; }
        public UTXOChangeWithSpentOutput UnConfirmed { get; set; }
        public int CurrentHeight { get; set; }
    }
}