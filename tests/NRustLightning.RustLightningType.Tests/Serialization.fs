module Tests

open DotNetLightning.Serialization
open System.IO
open Expecto
open FsCheck
open Generators.RustLightningTypes
open NRustLightning.RustLightningTypes

let config =
    { FsCheckConfig.defaultConfig with
            arbitrary = [ typeof<RustLightningTypeGenerators> ]
            maxTest = 30
        }

[<Tests>]
let tests =
  testList "serialization property tests" [
      testPropertyWithConfig config "option bytes" <| fun (d: Option<NonNull<byte[]>>) ->
          let d = d |> Option.map(fun b -> b.Get)
          use ms = new MemoryStream()
          use ls = new LightningWriterStream(ms)
          ls.WriteOption(d)
          let b = ms.ToArray()
          use ms2 = new MemoryStream(b)
          use ls2 = new LightningReaderStream(ms2)
          let d2 = ls2.ReadOption()
          Expect.equal (d2) d ""
      testPropertyWithConfig config "node announcement info (de)serialization" <| fun (d: NodeAnnouncementInfo) ->
          Expect.equal (NodeAnnouncementInfo.FromBytes(d.ToBytes())) d "bidirectional conversion must not change the original value"
      testPropertyWithConfig config "node info (de)serialization" <| fun (d: NodeInfo) ->
          Expect.equal (NodeInfo.FromBytes(d.ToBytes())) d "bidirectional conversion must not change the original value"
      testPropertyWithConfig config "directional channel info (de)serialization" <| fun (d: DirectionalChannelInfo) ->
          Expect.equal (DirectionalChannelInfo.FromBytes(d.ToBytes())) d "bidirectional conversion must not change the original value"
      testPropertyWithConfig config "channel info (de)serialization" <| fun (d: ChannelInfo) ->
          Expect.equal (ChannelInfo.FromBytes(d.ToBytes())) d "bidirectional conversion must not change the original value"
      testPropertyWithConfig config "network graph (de)serialization" <| fun (networkGraph: NetworkGraph) ->
          Expect.equal (NetworkGraph.FromBytes(networkGraph.ToBytes())) networkGraph "bidirectional conversion must not change the original value"
  ]
