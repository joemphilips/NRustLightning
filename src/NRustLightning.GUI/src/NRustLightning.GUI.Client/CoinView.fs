[<RequireQualifiedAccess>]
module NRustLightning.GUI.Client.CoinViewModule

open Elmish
open Bolero
open Bolero.Html
open MatBlazor
open NBitcoin

type Model = {
    Utxos: Coin list
}

type Msg =
    | NoOp


let init = {
    Utxos = []
}

let update msg model =
    match msg with
    | NoOp -> model
let utxoView _utxo =
    comp<MatDataTableContent<Coin>> []
        [
            comp<MatBody1> [] [text "TODO: show utxo"]
        ]

let view model _dispatch =
    comp<MatDataTable> [
        "Items" => model.Utxos
        attr.fragment "MatTableHeader" (concat [ th [] [ text "TxId" ]
                                                 th [] [ text "Amount" ]
                                                 th [] [ text "Type" ] ] )
        attr.fragmentWith "MatTableRow" (fun (coin: Coin) -> utxoView coin)
    ] []
    
type App () =
    inherit ProgramComponent<Model, Msg>()
    override this.Program =
        Program.mkSimple (fun _ -> init) update view