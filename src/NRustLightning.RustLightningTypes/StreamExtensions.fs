namespace NRustLightning.RustLightningTypes

open System.IO
open System.Runtime.CompilerServices
open DotNetLightning.Serialize
open DotNetLightning.Serialize.Msgs
open DotNetLightning.Utils
open NBitcoin


/// Extensions for LightningStream to work with rust-lightning style serialization
[<Extension;AbstractClass;Sealed>]
type Extensions() =
    [<Extension>]
    static member ReadOption(this: LightningReaderStream) =
        let length = this.ReadBigSize()
        match length with
        | 0UL -> None
        | x ->
            let d = this.ReadBytes((int)x - 1)
            Some(d)
            
    [<Extension>]
    static member WriteOption(this: LightningWriterStream, d: Option<byte[]>) =
        match d with
        | None -> this.WriteBigSize(0UL)
        | Some x ->
            let len = x.Length + 1
            this.WriteBigSize(uint64 len)
            this.Write x

    [<Extension>]
    static member ReadWithLen16(this: LightningReaderStream) =
        let length = int(this.ReadUInt16(false))
        this.ReadBytes(length)
        
    [<Extension>]
    static member ReadWithLenVarInt(this: LightningReaderStream) =
        let length = int(this.ReadBigSize())
        this.ReadBytes(length)
        
    [<Extension>]
    static member ReadOutpoint(this: LightningReaderStream) =
        (this.ReadUInt256(false), this.ReadUInt32(false) |> uint32)
        |> OutPoint
        |> LNOutPoint
        
    [<Extension>]
    static member ReadTxOut(this: LightningReaderStream) =
        let m = this.ReadUInt64 true
        ((m |> Money.Satoshis), (this.ReadWithLenVarInt() |> Script.FromBytesUnsafe))
        |> TxOut
        
    [<Extension>]
    static member Write(this: LightningWriterStream, d: Option<byte[]>) =
        match d with
        | None -> this.WriteByte(0uy)
        | Some (x) ->
            this.Write((x.Length + 1).ToVarInt())
            this.Write(x)
            
    [<Extension>]
    static member WriteWithLen16(this: LightningWriterStream, d: byte[]) =
        this.Write((uint16)d.Length, false)
        this.Write d
        
    [<Extension>]
    static member WriteWithLenVarInt(this: LightningWriterStream, d: byte[]) =
        let len = d.Length.ToVarInt()
        this.Write(len)
        this.Write(d)
        
module ILightningSerializable =
    let internal fromBytes<'T when 'T :(new :unit -> 'T) and 'T :> ILightningSerializable<'T>>(data: byte[]) = 
        use ms = new MemoryStream(data)
        use ls = new LightningReaderStream(ms)
        let instance = new 'T()
        instance.Deserialize(ls)
        instance
        
