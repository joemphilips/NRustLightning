namespace NRustLightning.RustLightningTypes

open DotNetLightning.Serialize
open System.Runtime.CompilerServices
open NBitcoin


/// Extensions for LightningStream to work with rust-lightning style serialization
[<Extension;AbstractClass;Sealed>]
type Extensions() =
    [<Extension>]
    static member ReadOption(this: LightningReaderStream) =
        let b = this.ReadByte()
        match b with
        | 0uy -> None
        | _ ->
            let length = this.ReadBigSize()
            let d = this.ReadBytes((int)length)
            Some(d)

    
    [<Extension>]
    static member ReadWithLen16(this: LightningReaderStream) =
        let length = int(this.ReadUInt16(false))
        this.ReadBytes(length)
            
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
