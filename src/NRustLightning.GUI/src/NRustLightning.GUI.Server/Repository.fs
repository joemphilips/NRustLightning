namespace NRustLightning.GUI.Server

open FSharp.Control
open FSharp.Control.AsyncSeqExtensions
open System.Text.Json
open System.Threading
open System.Threading.Tasks
open System.IO
open DBTrie
open DBTrie.Storage.Cache
open FSharp.Control.Tasks
open System
open Microsoft.Extensions.Options
open NRustLightning.Infrastructure.Networks
open NRustLightning.Infrastructure.Repository
open NRustLightning.GUI.Client.Configuration
open NRustLightning.GUI.Client.Wallet

type O = OptionalArgumentAttribute
type D = System.Runtime.InteropServices.DefaultParameterValueAttribute
module DBKeys =
    let [<Literal>] WalletIdToWalletInfo = "ww"

type Repository(conf: IOptionsMonitor<WalletBiwaConfiguration>) =
    let _dbPath = conf.CurrentValue.DBPath
    let openEngine() =
        task {
            return! DBTrieEngine.OpenFromFolder(_dbPath)
        }
    let pageSize = 8192
    let serializerOptions = JsonSerializerOptions()
    let repoSerializer = RepositorySerializer()
    
    do
        if (_dbPath |> Directory.Exists |> not) then
            Directory.CreateDirectory(_dbPath) |> ignore
        repoSerializer.ConfigureSerializer(serializerOptions)
        
    let _engine = openEngine().GetAwaiter().GetResult();
    do
        _engine.ConfigurePagePool(PagePool(pageSize, (50 * 1000 * 1000) / pageSize))
        
    member val SerializerOptions = serializerOptions with get
    
    member this.SetWalletInfo(info: WalletInfo, [<O;D(null)>]ct: CancellationToken) =
        if (info |> box |> isNull) then raise <| ArgumentNullException("info") else
        task {
            use! tx = _engine.OpenTransaction(ct)
            let json = JsonSerializer.Serialize(info, serializerOptions)
            let! _ = tx.GetTable(DBKeys.WalletIdToWalletInfo).Insert(info.Id.ToString(), json);
            do! tx.Commit()
        }
        
    member this.GetWalletInfo(wId: WalletId, [<O;D(null)>]ct: CancellationToken): Task<WalletInfo option> =
        task {
            use! tx = _engine.OpenTransaction(ct)
            let! row = tx.GetTable(DBKeys.WalletIdToWalletInfo).Get(wId.ToString())
            if (row |> isNull) then return None else
            let! s = row.ReadValueString()
            return JsonSerializer.Deserialize(s, serializerOptions)
        }
        
    member this.GetAllWalletInfos([<O;D(null)>]ct: CancellationToken) = task {
        use! tx = _engine.OpenTransaction(ct)
        let table = tx.GetTable(DBKeys.WalletIdToWalletInfo)
        let a =  table.Enumerate() |> AsyncSeq.ofAsyncEnum
        return!
            asyncSeq {
                for t in a do
                    let! v =  t.ReadValue().AsTask() |> Async.AwaitTask
                    yield JsonSerializer.Deserialize<WalletInfo>(v.Span, serializerOptions)
            } |> AsyncSeq.toArrayAsync |> Async.StartAsTask
    }
        
        
