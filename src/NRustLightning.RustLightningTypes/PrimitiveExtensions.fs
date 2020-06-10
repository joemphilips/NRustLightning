namespace NRustLightning.RustLightningTypes

open System
open System.Runtime.CompilerServices
open NBitcoin
open DotNetLightning.Utils.Primitives
open DotNetLightning.Utils
open DotNetLightning.Serialize
open DotNetLightning.Core.Utils.Extensions

[<AutoOpen>]
module PrimitiveStaticExtensions =
    type System.UInt32 with
        static member FromBytes(value: byte[], littleEndian) =
            if littleEndian then
                (uint32 value.[0]) <<< 24
                 ||| (uint32 value.[1] <<< 16)
                 ||| (uint32 value.[2] <<< 8)
                 ||| (uint32 value.[3])
            else
                (uint32 value.[3])
                ||| (uint32 value.[2] <<< 8)
                ||| (uint32 value.[1] <<< 16)
                ||| (uint32 value.[0] <<< 24)

    type System.Int32 with
        member this.ToVarInt() = (uint64 this).ToVarInt()

    type System.Int64 with
        member this.ToVarInt() = (uint64 this).ToVarInt()

    type System.UInt64 with
        static member ToVarInt(x: uint64) =
            if x < 0xfdUL then
                [|uint8 x|]
            else if x < 0x10000UL then
                let buf = Array.zeroCreate(3)
                buf.[0] <- (0xfduy)
                buf.[1] <- (byte (x >>> 8))
                buf.[2] <- byte x
                buf
            else if x < 0x100000000UL then
                let buf = Array.zeroCreate(5)
                buf.[0] <- (0xfeuy)
                buf.[1] <- (byte (x >>> 24))
                buf.[2] <- (byte (x >>> 16))
                buf.[3] <- (byte (x >>> 8))
                buf.[4] <- (byte x)
                buf
            else
                let buf = Array.zeroCreate(9)
                buf.[0] <- (0xffuy)
                buf.[1] <- (byte (x >>> 56))
                buf.[2] <- (byte (x >>> 48))
                buf.[3] <- (byte (x >>> 40))
                buf.[4] <- (byte (x >>> 32))
                buf.[5] <- (byte (x >>> 24))
                buf.[6] <- (byte (x >>> 16))
                buf.[7] <- (byte (x >>> 8))
                buf.[8] <- (byte x)
                buf
        static member FromSpan(value: Span<byte>, littleEndian: bool) =
            if (littleEndian) then
                (uint64 value.[0])
                ||| (uint64 value.[1] <<< 8)
                ||| (uint64 value.[2] <<< 16)
                ||| (uint64 value.[3] <<< 24)
                ||| (uint64 value.[4] <<< 32)
                ||| (uint64 value.[5] <<< 40)
                ||| (uint64 value.[6] <<< 48)
                ||| (uint64 value.[7] <<< 56)
            else
                (uint64 value.[7])
                ||| (uint64 value.[6] <<< 8)
                ||| (uint64 value.[5] <<< 16)
                ||| (uint64 value.[4] <<< 24)
                ||| (uint64 value.[3] <<< 32)
                ||| (uint64 value.[2] <<< 40)
                ||| (uint64 value.[1] <<< 48)
                ||| (uint64 value.[0] <<< 56)
    type LNOutPoint with
        /// rust-lightning compatible serialization
        static member FromBytes(b: byte[]) =
            let txid = uint256(b.[0..31], false)
            let vout = UInt32.FromBytes(b.[32..35], false)
            OutPoint(txid, vout) |> LNOutPoint
            
    type LNMoney with
        static member FromSpan(bytes: Span<byte>) =
            UInt64.FromSpan(bytes, false) |> int64 |> LNMoney


[<Extension;AbstractClass;Sealed>]
type PrimitiveExtensions() =
    [<Extension>]
    static member ToByteArrayWithLength(this: FeatureBit) =
        let b = this.ByteArray
        let len = ((uint16)b.Length).GetBytesBigEndian()
        Array.concat[len; b]
    [<Extension>]
    static member GetBytesBigEndian(this: uint64) =
        let d = BitConverter.GetBytes(this)
        if BitConverter.IsLittleEndian then (d |> Array.rev) else d
    [<Extension>]
    static member GetBytesBigEndian(this: int64) =
        (uint64 this).GetBytesBigEndian()
        
    [<Extension>]
    static member ToVarInt(x: uint64) =
        if x < 0xfdUL then
            [|uint8 x|]
        else if x < 0x10000UL then
            let buf = Array.zeroCreate(3)
            buf.[0] <- (0xfduy)
            buf.[1] <- (byte (x >>> 8))
            buf.[2] <- byte x
            buf
        else if x < 0x100000000UL then
            let buf = Array.zeroCreate(5)
            buf.[0] <- (0xfeuy)
            buf.[1] <- (byte (x >>> 24))
            buf.[2] <- (byte (x >>> 16))
            buf.[3] <- (byte (x >>> 8))
            buf.[4] <- (byte x)
            buf
        else
            let buf = Array.zeroCreate(9)
            buf.[0] <- (0xffuy)
            buf.[1] <- (byte (x >>> 56))
            buf.[2] <- (byte (x >>> 48))
            buf.[3] <- (byte (x >>> 40))
            buf.[4] <- (byte (x >>> 32))
            buf.[5] <- (byte (x >>> 24))
            buf.[6] <- (byte (x >>> 16))
            buf.[7] <- (byte (x >>> 8))
            buf.[8] <- (byte x)
            buf
        
    [<Extension>]
    static member GetBytesBigEndian(this: uint32) =
        let d = BitConverter.GetBytes(this)
        if BitConverter.IsLittleEndian then (d |> Array.rev) else d
        
    [<Extension>]
    static member GetBytesBigEndian(this: uint16) =
        let d = BitConverter.GetBytes(this)
        if BitConverter.IsLittleEndian then (d |> Array.rev) else d
        
