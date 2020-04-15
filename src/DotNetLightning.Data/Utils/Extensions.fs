module DotNetLightning.Core.Utils.Extensions

open System.Linq
open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Text

module Dict =
    let tryGetValue key (dict: IDictionary<_,_>)=
       match dict.TryGetValue key with
       | true, v -> Some v
       | false, _ -> None

[<Extension;AbstractClass;Sealed>]
type PrimitiveExtensions() =
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
        
type System.UInt16 with
    static member FromBytesBigEndian(value: byte[]) =
        ((uint16 value.[0]) <<< 8 ||| (uint16 value.[1]))
    static member FromBytesBigEndian(value: Span<byte>) =
        ((uint16 value.[0]) <<< 8 ||| (uint16 value.[1]))
        
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
        
type System.UInt64 with
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
    static member FromBytes(value: byte[], lEndian: bool) =
        UInt64.FromSpan(value.AsSpan(), lEndian)
        
type System.Int64 with
    member this.ToVarInt() = (uint64 this).ToVarInt()
type System.Byte
    with
    member a.FlipBit() =
        ((a &&& 0x1uy)  <<< 7) ||| ((a &&& 0x2uy)  <<< 5) |||
        ((a &&& 0x4uy)  <<< 3) ||| ((a &&& 0x8uy)  <<< 1) |||
        ((a &&& 0x10uy) >>> 1) ||| ((a &&& 0x20uy) >>> 3) |||
        ((a &&& 0x40uy) >>> 5) ||| ((a &&& 0x80uy) >>> 7)
        
[<Extension;AbstractClass;Sealed>]
type BitArrayExtensions() =
    [<Extension>]
    static member ToHex(this: #seq<byte>) =
        let db = StringBuilder()
        db.Append("0x") |> ignore
        this |> Seq.iter(fun b -> sprintf "%X" b |> db.Append |> ignore)
        db.ToString()
        
    [<Extension>]
    static member TryPopVarInt(this: byte array) =
        let e b = sprintf "Decoded VarInt is not canonical %A" b |> Error
        let x = this.[0]
        if x < 0xfduy then
            ((uint64 x), this.[1..]) |> Ok
        else if x = 0xfduy then
            if (this.Length < 2) then e this else
            let v = this.[1..2] |> UInt16.FromBytesBigEndian |> uint64
            if (v < 0xfdUL || 0x10000UL <= v) then e this else
            (v, this.[3..]) |> Ok
        else if x = 0xfeuy then
            if (this.Length < 4) then e this else
            let v = this.[1..4] |> fun x -> NBitcoin.Utils.ToUInt32(x, false) |> uint64
            if (v < 0x10000UL || 0x100000000UL <= v) then e this else
            (v, this.[5..]) |> Ok
        else
            if (this.Length < 8) then e this else
            let v = this.[1..8] |> fun x -> NBitcoin.Utils.ToUInt64(x, false)
            if (v < 0x100000000UL) then e this else
            (v, this.[9..]) |> Ok
        
type System.Collections.BitArray with
    member this.ToByteArray() =
        if this.Length = 0 then [||] else
        let ret: byte[] = Array.zeroCreate (((this.Length - 1) / 8) + 1)
        let boolArray: bool[] = this.Reverse()
        let t = BitArray(boolArray)
        t.CopyTo(ret, 0)
        ret |> Array.rev
        
    static member From5BitEncoding(b: byte[]) =
        let bitArray = System.Collections.BitArray(b.Length * 5)
        for di in 0..(b.Length - 1) do
            bitArray.Set(di * 5 + 0, ((b.[di] >>> 4) &&& 0x01uy) = 1uy)
            bitArray.Set(di * 5 + 1, ((b.[di] >>> 3) &&& 0x01uy) = 1uy)
            bitArray.Set(di * 5 + 2, ((b.[di] >>> 2) &&& 0x01uy) = 1uy)
            bitArray.Set(di * 5 + 3, ((b.[di] >>> 1) &&& 0x01uy) = 1uy)
            bitArray.Set(di * 5 + 4, ((b.[di] >>> 0) &&& 0x01uy) = 1uy)
        bitArray
        
    static member Concat(ba: #seq<BitArray>) =
        let a = ba |> Seq.map(fun b -> b.Cast<bool>()) |> Seq.concat |> Seq.toArray
        BitArray(a)
    static member FromUInt32(d: uint32) =
        let b = d.GetBytesBigEndian()
        BitArray.From5BitEncoding(b)
        
    member this.Reverse() =
        let length = this.Length
        let mutable result = Array.zeroCreate this.Length
        let mid = length / 2
        for i in 0..mid - 1 do
            let bit = this.[i]
            result.[i] <- this.[length - i - 1]
            result.[length - i - 1] <- bit
        result
    member this.PrintBits() =
        let sb = StringBuilder()
        for b in this do
            (if b then "1" else "0") |> sb.Append |> ignore
        sb.ToString()
        
    static member FromInt64(value: int64) =
        let mutable v = value
        let array = Array.zeroCreate 64
        for i in 0..(64 - 1) do
            array.[i] <- (v &&& 1L) = 1L
            v <- v >>> 1
        BitArray(array |> Array.rev)
    static member TryParse(str: string) =
        let mutable str = str.Trim().Clone() :?> string
        if str.StartsWith("0b", StringComparison.OrdinalIgnoreCase) then
            str <- str.Substring("0b".Length)
        let array = Array.zeroCreate(str.Length)
        let mutable hasFunnyChar = -1
        for i in 0..str.Length - 1 do
            if hasFunnyChar <> -1 then () else
            if str.[i] = '0' then array.[i] <- false else
            if str.[i] = '1' then array.[i] <- true else
            hasFunnyChar <- i
        if hasFunnyChar <> -1 then
            sprintf "Failed to parse BitArray! it must have only '0' or '1' but we found %A" str.[hasFunnyChar]
            |> Error
        else
            BitArray(array) |> Ok
        
    /// This flips bits for each byte before passing to the BitArray constructor.
    /// This is necessary for representing bolt 9 feature bits as BitArray
    static member FromBytes(ba: byte[]) =
        ba |> Array.map(fun b -> b.FlipBit()) |> BitArray
        
    member this.ToHex() =
        this.ToByteArray().ToHex()
        
[<Extension;AbstractClass;Sealed>]
type DictionaryExtensions() =

    [<Extension>]
    static member TryGetValueOption(this: IDictionary<_, _>, key) =
        Dict.tryGetValue key this

module Seq =
    let skipSafe num = 
        Seq.zip (Seq.initInfinite id)
        >> Seq.skipWhile (fun (i, _) -> i < num)
        >> Seq.map snd