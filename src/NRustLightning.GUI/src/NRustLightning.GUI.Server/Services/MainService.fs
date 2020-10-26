namespace NRustLightning.GUI.Server.Services

open Bolero.Remoting.Server
open NRustLightning.GUI.Server
open NRustLightning.Infrastructure.Repository

type MainService(ctx: IRemoteContext, repo: Repository) =
    inherit RemoteHandler<NRustLightning.GUI.Client.Main2.MainService>()
    
    override this.Handler = {
        GetWalletList = fun () -> async {
            let! r = repo.GetAllWalletInfos() |> Async.AwaitTask
            return r |> Seq.map(fun o -> (o.Id, o.Name)) |> Map.ofSeq
        }
    }
