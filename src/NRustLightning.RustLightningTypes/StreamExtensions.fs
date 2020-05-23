namespace NRustLightning.RustLightningTypes

open DotNetLightning.Serialize
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
            
    static member Write(this: LightningWriterStream, d: Option<byte[]>) =
        match d with
        | None -> this.WriteByte(0uy)
        | Some (x) ->
            this.Write((x.Length + 1).ToVarInt())
            this.Write(x)
