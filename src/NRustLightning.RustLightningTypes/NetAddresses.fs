namespace NRustLightning.RustLightningTypes

open DotNetLightning.Serialize
open System.Collections.Generic
open System.IO
open DotNetLightning.Serialize.Msgs
open System.Runtime.CompilerServices

[<Extension;AbstractClass;Sealed>]
type NetAddressExtension() =
    [<Extension>]
    static member ToBytes(this: IList<NetAddress>) =
        use ms = new MemoryStream()
        use ls = new LightningWriterStream(ms)
        ls.Write((uint16)this.Count, false)
        for addr in this do
            addr.WriteTo ls
        ms.ToArray()
