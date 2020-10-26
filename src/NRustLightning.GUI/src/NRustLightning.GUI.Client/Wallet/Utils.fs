module NRustLightning.GUI.Client.Wallet.Utils

open System
open NBitcoin
open NBitcoin


[<CustomComparison;CustomEquality>]
type WalletId = private WalletId of RootedKeyPath
    with
    member private this.Value = let (WalletId v) = this in v
    override this.ToString() =
        this.Value.ToString()
        
    static member TryParse(s: string) =
        match RootedKeyPath.TryParse(s) with
        | true, r -> Some (WalletId r)
        | _ -> None
    override this.Equals(o) =
        this.ToString().Equals(o.ToString())
        
    override this.GetHashCode() =
        this.Value.GetHashCode()
    member this.CompareTo(o: obj) =
        match o with
        | :? WalletId as other ->
            (this :> IComparable<WalletId>).CompareTo(other)
        | _ -> raise <| ArgumentException(sprintf "Can not compare WalletId to %s" (o.GetType()).Name)
        
    interface IComparable with
        member this.CompareTo(o) =
            this.CompareTo(o)
    interface IComparable<WalletId> with
        member this.CompareTo(o) =
            o.ToString().CompareTo(this.ToString())
