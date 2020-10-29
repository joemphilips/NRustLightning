namespace NRustLightning.GUI.Client.Wallet

open System
open DotNetLightning.Utils
open NBitcoin


[<CustomComparison;CustomEquality>]
type WalletId = private WalletId of RootedKeyPath
    with
    member private this.Value = let (WalletId v) = this in v
    override this.ToString() =
        this.Value.ToString()
        
    member this.ToEncodableValue() =
        let master = this.Value.MasterFingerprint.ToString()
        let paths = this.Value.KeyPath.Indexes
        (master, paths)
        
    static member FromElements(masterKeyFingerPrint: string, keyPathIndexes: uint32[]) =
        let m = HDFingerprint.Parse(masterKeyFingerPrint)
        let paths = KeyPath(keyPathIndexes)
        RootedKeyPath(m, paths) |> WalletId
        
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
            
type WalletInfo = {
    Id: WalletId
    OnChainBalance: Money
    OffChainBalance: LNMoney
    CryptoCode: string
    Name: string
}

