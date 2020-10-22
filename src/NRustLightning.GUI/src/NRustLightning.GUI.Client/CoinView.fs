module NRustLightning.GUI.Client.CoinView

open Bolero
open Bolero.Html
open MatBlazor
open NBitcoin

type Model = {
    Utxos: Coin list
}

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