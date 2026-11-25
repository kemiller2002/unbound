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
