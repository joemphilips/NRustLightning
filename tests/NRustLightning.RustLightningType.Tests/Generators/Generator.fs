module internal Generators.Generator

open DotNetLightning.Utils
open FsCheck
open NBitcoin

let byteGen = byte <!> Gen.choose(0, 127)
let bytesGen = Gen.listOf(byteGen) |> Gen.map(List.toArray)
let bytesOfNGen(n) = Gen.listOfLength n byteGen |> Gen.map(List.toArray)
let uint48Gen = bytesOfNGen(6) |> Gen.map(fun bs -> UInt48.FromBytesBigEndian bs)
let uint256Gen = bytesOfNGen(32) |> Gen.map(fun bs -> uint256(bs))
let temporaryChannelGen = uint256Gen |> Gen.map ChannelId
let moneyGen = Arb.generate<uint64> |> Gen.map(Money.Satoshis)
let lnMoneyGen = Arb.generate<uint64> |> Gen.map(LNMoney.MilliSatoshis)

let shortChannelIdsGen = Arb.generate<uint64> |> Gen.map(ShortChannelId.FromUInt64)
// crypto stuffs

let keyGen = Gen.fresh (fun () -> new Key())

let inline optionGen (g: Gen<_>) =
    Gen.oneof(seq { yield g |> Gen.map(Some); yield Gen.constant(None) })

let pubKeyGen = gen {
    let! key = keyGen
    return key.PubKey
}

let revocationKeyGen = gen {
    let! key = keyGen
    return RevocationKey key
}

let commitmentPubKeyGen = gen {
    let! pubKey = pubKeyGen
    return CommitmentPubKey pubKey
}

let commitmentNumberGen = gen {
    let! n = uint48Gen
    return CommitmentNumber n
}

let signatureGen: Gen<LNECDSASignature> = gen {
    let! h = uint256Gen
    let! k = keyGen
    return k.Sign(h, false) |> LNECDSASignature
}
// scripts

let pushOnlyOpcodeGen = bytesOfNGen(4) |> Gen.map(Op.GetPushOp)
let pushOnlyOpcodesGen = Gen.listOf pushOnlyOpcodeGen

let pushScriptGen = Gen.nonEmptyListOf pushOnlyOpcodeGen |> Gen.map(fun ops -> Script(ops))

type PrimitiveGenerators =
    static member ECDSASignature() : Arbitrary<LNECDSASignature> =
        Arb.fromGen(signatureGen)

    static member UInt256(): Arbitrary<uint256> =
        Arb.fromGen(uint256Gen)
        
    static member PubKey() = Arb.fromGen(pubKeyGen)
    
    static member NodeId() = Arb.fromGen(NodeId <!> pubKeyGen)
    
