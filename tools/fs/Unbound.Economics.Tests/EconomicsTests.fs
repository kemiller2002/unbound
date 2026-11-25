/// Tests for the economics port. Expected values were computed with the Python models under CPython 3.13
/// (model.py, enterprise_economics.py) and are hand-checkable where noted.
module Unbound.Economics.Tests.EconomicsTests

open System
open System.IO
open System.Numerics
open Xunit
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.Economics
open Unbound.Economics.PyCompat

let private parametersPath =
    let rel = Path.Combine("requirements", "discovery", "2026-11-24-simplification-economics", "model", "parameters.json")
    let rec up (dir: DirectoryInfo) =
        if isNull dir then failwith "repository root not found"
        elif File.Exists(Path.Combine(dir.FullName, rel)) then Path.Combine(dir.FullName, rel)
        else up dir.Parent
    up (DirectoryInfo AppContext.BaseDirectory)

let private cfg = lazy (Model.load parametersPath)

let private bits (x: float) = BitConverter.DoubleToInt64Bits x

// ---- seed derivation ----

[<Fact>]
let ``model seed_for matches Python`` () =
    Assert.Equal(BigInteger 1259737706, Model.seedFor (BigInteger 20261124) [ "OM1"; "1000"; "0" ])
    Assert.Equal(BigInteger 550133139, Model.seedFor (BigInteger 20261124) [ "DT"; "OM3"; "100000"; "299" ])

[<Fact>]
let ``enterprise seeds match Python`` () =
    Assert.Equal(BigInteger 425213312, EnterpriseEconomics.seeds [ "E1"; "A"; "50000"; "0" ])
    Assert.Equal(BigInteger 1903404713, EnterpriseEconomics.seeds [ "dt"; "E2"; "C"; "128"; "7" ])

// ---- determinism ----

[<Fact>]
let ``same seed gives identical Monte Carlo runs`` () =
    let a = Model.mc cfg.Value "OM3" (I 10000) "normal" 2
    let b = Model.mc cfg.Value "OM3" (I 10000) "normal" 2
    let key (r: Model.Run) = r.Metrics |> List.map (fun (k, x) -> k, bits x.V, x.IsInt)
    Assert.Equal<obj list>(a |> List.map (key >> box), b |> List.map (key >> box))
    // and the values are CPython's, bit for bit
    Assert.Equal(bits 669.3699657511162, bits (a.[0].Metric "cost_per_account").V)
    Assert.Equal(bits 665.8747878715049, bits (a.[1].Metric "cost_per_account").V)

[<Fact>]
let ``different seeds give different runs`` () =
    let rng0 = seeded (Model.seedFor (BigInteger 20261124) [ "OM3"; "10000"; "0" ])
    let rng1 = seeded (Model.seedFor (BigInteger 20261124) [ "OM3"; "10000"; "1" ])
    let r0, _ = Model.runOnce cfg.Value "OM3" (I 10000) "normal" rng0 true []
    let r1, _ = Model.runOnce cfg.Value "OM3" (I 10000) "normal" rng1 true []
    Assert.NotEqual(bits (r0.Metric "cost_per_account").V, bits (r1.Metric "cost_per_account").V)

// ---- hand-checkable cost lines (OM1, 100,000 accounts, expected values, normal scenario) ----

let private om1Lines = lazy ((Model.runDet cfg.Value "OM1" (I 100000) "normal" []).Lines)

let private lineUsd (name: string) = (om1Lines.Value |> List.find (fun l -> l.Line = name)).Usd

[<Fact>]
let ``staff lines are FTE times loaded cost and stay Python ints`` () =
    // leadership: 10 FTE at 100k accounts; engineering: min(10, 4 + 2*round(log10(100))) = 8; loaded cost 165,000
    Assert.Equal("1650000", Num.str (lineUsd "staff:leadership"))
    Assert.Equal("1320000", Num.str (lineUsd "staff:engineering"))
    // exceptions queue: 31 FTE x 165,000
    Assert.Equal("5115000", Num.str (lineUsd "staff:queue:exceptions"))
    Assert.Equal("250000", Num.str (lineUsd "infrastructure_base"))
    Assert.Equal("100000", Num.str (lineUsd "data_providers"))

[<Fact>]
let ``scaled and per-account lines are floats with the hand-computed values`` () =
    // insurance and legal scale by 1 + log10(100000 / 1000) = 3
    Assert.Equal("450000.0", Num.str (lineUsd "insurance"))
    Assert.Equal("600000.0", Num.str (lineUsd "legal_filings"))
    // 100,000 account-years x $3 infrastructure per account
    Assert.Equal("300000.0", Num.str (lineUsd "infrastructure_per_account"))
    // audit: 120,000 + 0.02 bps x $16,596,000,000 AUM = 120,000 + 33,192
    Assert.Equal("153192.0", Num.str (lineUsd "external_audit"))
    // 15% new accounts: 15,000 x $3 identity, 15,000 x $400 CAC
    Assert.Equal("45000.0", Num.str (lineUsd "identity_new_accounts"))
    Assert.Equal("6000000.0", Num.str (lineUsd "customer_acquisition"))

[<Fact>]
let ``activity map assigns lines as enterprise_economics does`` () =
    Assert.Equal("reconciliation_accounting_exceptions", EnterpriseEconomics.activityOf "staff:surge:ops")
    Assert.Equal("charity_vetting_and_grant_decisions", EnterpriseEconomics.activityOf "staff:queue:exceptions")
    Assert.Equal("technology", EnterpriseEconomics.activityOf "unbound_licence")
    Assert.Equal("other", EnterpriseEconomics.activityOf "exceptional:migration programme")

// ---- Python numeric semantics ----

[<Fact>]
let ``int and float typing follows Python`` () =
    Assert.Equal(JInt(BigInteger 6), Num.toJson (I 3 * I 2))
    Assert.Equal(JFloat 1.5, Num.toJson (I 3 / I 2))
    Assert.Equal(JFloat 6.0, Num.toJson (I 3 * F 2.0))
    Assert.Equal("100", Num.str (Num.max (I 100) (F 100.0))) // max keeps the first of equal values
    Assert.Equal("100.0", Num.str (Num.min (F 100.0) (I 100)))

[<Fact>]
let ``sum reproduces CPython 3.13`` () =
    // int items after the first float are added without compensation
    Assert.Equal("1e+16", Num.str (Num.sum [ F 1e16; F 1.0; I 1 ]))
    Assert.Equal("1.0000000000000002e+16", Num.str (Num.sum [ I 1; F 1e16; F 1.0; F 1.0 ]))
    Assert.Equal("0", Num.str (Num.sum []))
    Assert.Equal("3.5", Num.str (Num.sum [ I 1; I 2; F 0.5 ]))
