module Unbound.Replay.Tests

open System
open System.Xml.Linq
open Xunit
open Unbound.Replay.Domain

let private recipient ein name desc =
    { Name = name
      Ein = ein
      State = Some "OH"
      Foreign = false
      SectionDesc = desc
      Cash = 10000m
      NonCash = 0m }

let private registry =
    { Pub78 =
        Map.ofList
            [ "111111111", { Ein = "111111111"; Codes = [ "PC" ] }
              "222222222", { Ein = "222222222"; Codes = [ "PF" ] }
              "333333333", { Ein = "333333333"; Codes = [ "SONFI" ] }
              "444444444", { Ein = "444444444"; Codes = [ "FORGN"; "PF" ] }
              "555555555", { Ein = "555555555"; Codes = [ "POF" ] } ]
      Bmf =
        Map.ofList
            [ "666666666", { Ein = "666666666"; Subsection = "04"; Foundation = "00"; Deductibility = "2" }
              "777777777", { Ein = "777777777"; Subsection = "03"; Foundation = "10"; Deductibility = "1" } ]
      Revocations =
        Map.ofList
            [ "888888888",
              [ { Ein = "888888888"; Revoked = DateOnly(2020, 5, 15); Posted = Some(DateOnly(2021, 2, 1)); Reinstated = None } ]
              "999999999",
              [ { Ein = "999999999"; Revoked = DateOnly(2019, 5, 15); Posted = None; Reinstated = Some(DateOnly(2020, 1, 1)) } ] ] }

let private noSanctions (_: string) = 0.0
let private asOf = DateOnly(2024, 12, 31)
let private route policy r = (Screening.screen policy 0.92 registry asOf noSanctions r).Route

[<Fact>]
let ``quoted CSV keeps commas inside quotes and unescapes doubled quotes`` () =
    Assert.Equal<string list>([ "1"; "A, B \"C\""; "" ], Csv.split ',' "1,\"A, B \"\"C\"\"\",")

[<Fact>]
let ``EINs normalise to nine digits and zero is absent`` () =
    Assert.Equal(Some "012345678", Sources.normalizeEin "12345678")
    Assert.Equal(None, Sources.normalizeEin "0")
    Assert.Equal(None, Sources.normalizeEin "")

[<Fact>]
let ``revocation line parses IRS dates`` () =
    let e = Sources.parseRevocation "000003154|OAKLEAF|| ST|NORFOLK|VA|23523|US|03|15-NOV-2017|12-MAR-2018|"
    Assert.Equal(Some(DateOnly(2017, 11, 15)), e |> Option.map (fun x -> x.Revoked))
    Assert.Equal(Some(Some(DateOnly(2018, 3, 12))), e |> Option.map (fun x -> x.Posted))

[<Fact>]
let ``private foundation code takes precedence over foreign-address code`` () =
    Assert.Equal(PrivateFoundation, Screening.classify registry (recipient (Some "444444444") "X" ""))

[<Fact>]
let ``public charity passes, foundation and Type III NFI go to a person`` () =
    Assert.Equal(StraightThrough, route Standard (recipient (Some "111111111") "X" ""))
    Assert.Equal(Exception "private_foundation_expenditure_responsibility", route Standard (recipient (Some "222222222") "X" ""))
    Assert.Equal(Exception "type3_non_functionally_integrated", route Standard (recipient (Some "333333333") "X" ""))

[<Fact>]
let ``BMF fallback: non-501c3 is an exception, church passes`` () =
    Assert.Equal(Exception "not_501c3", route Standard (recipient (Some "666666666") "X" ""))
    Assert.Equal(StraightThrough, route Standard (recipient (Some "777777777") "X" ""))

[<Fact>]
let ``conservative policy routes private operating foundations to a person`` () =
    Assert.Equal(StraightThrough, route Standard (recipient (Some "555555555") "X" ""))
    Assert.Equal(Exception "private_operating_foundation", route Conservative (recipient (Some "555555555") "X" ""))

[<Fact>]
let ``unlisted EIN uses the sponsor label only to name the exception`` () =
    Assert.Equal(Exception "government_unit_unverified", route Standard (recipient (Some "123123123") "X" "170(C)(1) GOVERNMENT"))
    Assert.Equal(Exception "ein_not_in_irs_lists", route Standard (recipient (Some "123123123") "X" "501(C)(3) PUBLIC CHA"))
    Assert.Equal(Exception "no_ein", route Standard (recipient None "X" ""))

[<Fact>]
let ``revocation in force blocks; reinstated revocation does not`` () =
    Assert.Equal(Block "revoked", route Standard (recipient (Some "888888888") "X" ""))
    Assert.Equal(Exception "ein_not_in_irs_lists", route Standard (recipient (Some "999999999") "X" ""))

[<Fact>]
let ``sanctions score at threshold overrides a clean class`` () =
    let s = Screening.screen Standard 0.92 registry asOf (fun _ -> 0.95) (recipient (Some "111111111") "X" "")
    Assert.Equal(Exception "sanctions_alert", s.Route)

[<Fact>]
let ``Jaro-Winkler matches the textbook values`` () =
    Assert.Equal(0.9611, Math.Round(NameMatch.jaroWinkler "MARTHA" "MARHTA", 4))
    Assert.Equal(0.8133, Math.Round(NameMatch.jaroWinkler "DIXON" "DICKSONX", 4))
    Assert.Equal(1.0, NameMatch.jaroWinkler "ABC" "ABC")

[<Fact>]
let ``name key ignores order, case, punctuation and legal suffixes`` () =
    Assert.Equal(NameMatch.key "Acme Trading Co., Ltd.", NameMatch.key "TRADING ACME")

[<Fact>]
let ``index finds perturbed listed names and ignores conveyances`` () =
    let names =
        [ { EntNum = 1; Name = "GOLDEN CRESCENT TRADING LLC"; Kind = Entity; IsAlias = false }
          { EntNum = 2; Name = "SEA BREEZE"; Kind = Conveyance; IsAlias = false } ]

    let idx = NameMatch.buildIndex 400 names
    Assert.True(fst (NameMatch.best NameMatch.weighted idx (NameMatch.mutate 2 "GOLDEN CRESCENT TRADING LLC")) >= 0.92)
    Assert.True(fst (NameMatch.best NameMatch.weighted idx "Trading Golden Crescent") >= 0.99)
    Assert.Equal(0.0, fst (NameMatch.best NameMatch.weighted idx "SEA BREEZE"))

[<Fact>]
let ``percentile uses nearest rank`` () =
    Assert.Equal(2.0, Stats.percentile 0.5 [ 3.0; 1.0; 2.0; 4.0 ])
    Assert.Equal(4.0, Stats.percentile 1.0 [ 3.0; 1.0; 2.0; 4.0 ])

let private xml =
    """<Return xmlns="http://www.irs.gov/efile"><ReturnHeader><TaxPeriodEndDt>2024-12-31</TaxPeriodEndDt>
<TaxPeriodBeginDt>2024-01-01</TaxPeriodBeginDt><Filer><EIN>581493949</EIN><BusinessName><BusinessNameLine1Txt>SPONSOR</BusinessNameLine1Txt>
</BusinessName></Filer><TaxYr>2024</TaxYr></ReturnHeader><ReturnData><IRS990><TotalEmployeeCnt>10</TotalEmployeeCnt>
<CYTotalExpensesAmt>1000</CYTotalExpensesAmt><CYGrantsAndSimilarPaidAmt>800</CYGrantsAndSimilarPaidAmt></IRS990>
<IRS990ScheduleD><DonorAdvisedFundsHeldCnt>50</DonorAdvisedFundsHeldCnt></IRS990ScheduleD><IRS990ScheduleI>
<RecipientTable><RecipientBusinessName><BusinessNameLine1Txt>A CHARITY</BusinessNameLine1Txt></RecipientBusinessName>
<USAddress><StateAbbreviationCd>FL</StateAbbreviationCd></USAddress><RecipientEIN>593229898</RecipientEIN>
<IRCSectionDesc>501(C)(3) PUBLIC CHA</IRCSectionDesc><CashGrantAmt>5003</CashGrantAmt></RecipientTable>
<RecipientTable><RecipientBusinessName><BusinessNameLine1Txt>ABROAD</BusinessNameLine1Txt></RecipientBusinessName>
<ForeignAddress><CountryCd>CA</CountryCd></ForeignAddress><CashGrantAmt>6000</CashGrantAmt></RecipientTable>
</IRS990ScheduleI></ReturnData></Return>"""

[<Fact>]
let ``Form 990 parser reads header, Schedule D count and Schedule I rows`` () =
    match Sources.parseReturn "oid" (XDocument.Parse xml) with
    | Error e -> failwith e
    | Ok r ->
        Assert.Equal(2024, r.TaxYear)
        Assert.Equal(Some 50, r.DafCount)
        Assert.Equal(Some 10, r.Employees)
        Assert.Equal(2, r.Recipients.Length)
        Assert.Equal(Some "593229898", r.Recipients.[0].Ein)
        Assert.True(r.Recipients.[1].Foreign)
        Assert.Equal(None, r.Recipients.[1].Ein)

[<Fact>]
let ``calibration derives non-grant expense per fund`` () =
    match Sources.parseReturn "oid" (XDocument.Parse xml) with
    | Error e -> failwith e
    | Ok r -> Assert.Equal(Some 4.0, (Analysis.calibration [ r ]).Head.NonGrantExpensePerFund)

[<Fact>]
let ``JSON rendering is deterministic and escapes strings`` () =
    let j = Json.JObj [ "a", Json.JStr "x\"y"; "b", Json.JNum 0.123456; "c", Json.JArr [] ]
    Assert.Equal("{\n  \"a\": \"x\\\"y\",\n  \"b\": 0.1235,\n  \"c\": []\n}\n", Json.render j)

[<Fact>]
let ``weighted matcher does not alert on shared generic words`` () =
    let names =
        [ { EntNum = 1; Name = "AL-RAHMA FOUNDATION"; Kind = Entity; IsAlias = false }
          { EntNum = 2; Name = "FOUNDATION FOR PEACE"; Kind = Entity; IsAlias = false }
          { EntNum = 3; Name = "AL NASR TRADING"; Kind = Entity; IsAlias = false } ]

    let idx = NameMatch.buildIndex 400 names
    Assert.True(fst (NameMatch.best NameMatch.weighted idx "AL NOOR FOUNDATION") < 0.85)
    Assert.True(fst (NameMatch.best NameMatch.weighted idx "AL RAHMA FOUNDATION INC") >= 0.95)

[<Fact>]
let ``church and government names are classified without IRS listing`` () =
    Assert.Equal(ReligiousUnlisted, Screening.classify registry (recipient (Some "123123123") "FIRST UNITED METHODIST CHURCH OF DECORAH" "501(C)(3)"))
    Assert.Equal(GovernmentUnverified, Screening.classify registry (recipient (Some "123123123") "TOWN OF WILTON" "501(C)(3)"))
    Assert.Equal(GovernmentUnverified, Screening.classify registry (recipient (Some "123123123") "REYNOLDSBURG CITY SCHOOLS" "501C3"))

[<Fact>]
let ``current Pub 78 listing supersedes a revocation without reinstatement date`` () =
    let r = { registry with Pub78 = registry.Pub78.Add("888888888", { Ein = "888888888"; Codes = [ "PC" ] }) }
    Assert.Equal(StraightThrough, (Screening.screen Standard 0.92 r asOf noSanctions (recipient (Some "888888888") "X" "")).Route)

// ---- Synthetic operating year ----

[<Fact>]
let ``uniform draws are deterministic, in range and key-sensitive`` () =
    let a = Synthetic.uniform 7UL [ 1; 2; 3 ]
    Assert.Equal(a, Synthetic.uniform 7UL [ 1; 2; 3 ])
    Assert.NotEqual(a, Synthetic.uniform 7UL [ 1; 2; 4 ])
    let xs = [ for i in 0..9999 -> Synthetic.uniform 1UL [ i ] ]
    Assert.True(xs |> List.forall (fun x -> x >= 0.0 && x < 1.0))
    Assert.InRange(List.average xs, 0.49, 0.51)

[<Fact>]
let ``Poisson draws have the requested mean`` () =
    let mean = [ for i in 0..19999 -> float (Synthetic.poisson 8.0 (Synthetic.uniform 3UL [ i ])) ] |> List.average
    Assert.InRange(mean, 7.9, 8.1)

[<Fact>]
let ``log-normal draws have the requested arithmetic mean`` () =
    let mean = [ for i in 0..49999 -> Synthetic.lognormal 3000.0 1.0 (Synthetic.uniform 5UL [ i; 0 ]) (Synthetic.uniform 5UL [ i; 1 ]) ] |> List.average
    Assert.InRange(mean, 2850.0, 3150.0)

[<Fact>]
let ``weighted choice follows the weights`` () =
    let cum = Synthetic.cumulate [ 1.0; 3.0 ]
    let share = [ for i in 0..9999 -> Synthetic.choose cum (Synthetic.uniform 9UL [ i ]) ] |> List.filter ((=) 1) |> List.length |> float
    Assert.InRange(share / 10000.0, 0.73, 0.77)

[<Fact>]
let ``stable hash is FNV-1a (independently computed value)`` () =
    Assert.Equal(823376872, Synthetic.stableHash "F000001-D010-3")

let private paramsJson =
    """{"params": {"grants_mult": {"value": 1.0}, "avg_grant_scale": {"value": 1.0}, "contacts_mult": {"value": 1.0},
    "complex_share_mult": {"value": 1.0}, "december_k": {"value": 3}, "december_days": {"value": 25},
    "recon_break_rate": {"value": 0.0}, "ach_return_rate": {"value": 0.0}, "new_account_share": {"value": 0.0},
    "minutes_per_exception": {"value": 45}, "contact_minutes": {"value": 20}},
    "segments": {"typical": {"grants": 8, "avg_grant": 3000, "contribs": 2, "avg_contrib": 12000, "contacts": 0.0,
    "complex_share": 0.0, "check_share": 0.0, "mix": {"ach": 1.0}}},
    "mixes": {"market": {"typical": 1.0}}}"""

[<Fact>]
let ``December surge weights the last 25 days threefold`` () =
    let p = Synthetic.parseParams "market" paramsJson
    let cfg = { Synthetic.defaultConfig with Funds = 400; InjectPer1000 = 0.0 }
    let pool = [| recipient (Some "111111111") "A" ""; recipient (Some "222222222") "B" "" |]
    let fs = Synthetic.funds p cfg (Synthetic.cumulate [ 1.0; 1.0 ])
    let evs = Synthetic.events p cfg pool (Synthetic.cumulate [ 1.0; 1.0 ]) [] fs
    let lateDec = evs |> List.filter (fun e -> e.Day >= 340) |> List.length |> float
    // 25 days x 3 out of 340 + 75 = 18.1% of arrivals
    Assert.InRange(lateDec / float evs.Length, 0.165, 0.197)

[<Fact>]
let ``repeat exception recipients become reuse items`` () =
    let p = Synthetic.parseParams "market" paramsJson
    let cfg = { Synthetic.defaultConfig with Funds = 50; InjectPer1000 = 0.0 }
    let pool = [| recipient (Some "222222222") "PF" "" |]
    let fs = Synthetic.funds p cfg (Synthetic.cumulate [ 1.0 ])
    let evs = Synthetic.events p cfg pool (Synthetic.cumulate [ 1.0 ]) [] fs
    let screenGrant = Screening.screen Standard 0.92 registry asOf noSanctions
    let items = Synthetic.workItems p cfg screenGrant fs evs
    Assert.Equal(1, items |> List.filter (fun w -> w.Kind = "exception_first") |> List.length)
    Assert.Equal((evs |> List.length) - 1 - (evs |> List.filter (fun e -> match e.Txn with Synthetic.Contribution _ -> true | _ -> false) |> List.length),
                 items |> List.filter (fun w -> w.Kind = "exception_reuse") |> List.length)
