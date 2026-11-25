/// Pure replay analyses over parsed public data.
module Unbound.Replay.Analysis

open System
open Unbound.Replay.Domain
open Unbound.Replay.Screening
open Unbound.Replay.Stats

/// Identity used for reuse: the EIN, or normalised name plus state when the sponsor reported no EIN.
let recipientKey (r: Recipient) : string =
    match r.Ein with
    | Some e -> e
    | None -> "N:" + NameMatch.key r.Name + "|" + (r.State |> Option.defaultValue (if r.Foreign then "FOREIGN" else ""))

let dollars (r: Recipient) = r.Cash + r.NonCash

/// Latest return per sponsor paired with the immediately preceding tax year when available.
let pairs (returns: SponsorReturn list) : (SponsorReturn * SponsorReturn option) list =
    returns
    |> List.groupBy (fun r -> r.Ein)
    |> List.map (fun (_, rs) ->
        let current = rs |> List.maxBy (fun r -> r.TaxYear)
        current, rs |> List.tryFind (fun r -> r.TaxYear = current.TaxYear - 1))
    |> List.sortBy (fun (c, _) -> c.Ein)

/// Sponsors whose Schedule I lists recipients individually (at least ten rows with an EIN).
let itemises (r: SponsorReturn) =
    r.Recipients |> List.filter (fun x -> x.Ein.IsSome) |> List.length >= 10

type Labelled = { Screened: Screened; Policy: Policy }

type SponsorResult =
    { Ein: string
      Name: string
      TaxYear: int
      PriorYear: int option
      Rows: int
      RowsWithEin: int
      Dollars: decimal
      ExceptionRows: Map<string, int> // policy name -> rows routed to a person (exceptions plus blocks)
      ExceptionDollars: Map<string, decimal>
      Blocked: int
      SanctionsAlerts: int
      LabelMismatch: int
      ReuseWithinSponsor: float option // share of exception rows whose recipient the sponsor also granted to in the prior year
      ReuseWithinSponsorDollars: float option
      ReuseAnySponsor: float // share of exception rows seen at this or any other sponsor in either year
      RecipientOverlapPrior: float option // share of all rows whose recipient also appears in the prior year
      RevokedDuringYear: int
      GrantedWhileRevoked: int
      GrantedWhileRevokedVisible: int }

let policyName =
    function
    | Standard -> "standard"
    | Conservative -> "conservative"

let private publicLabel (desc: string) =
    let u = desc.ToUpperInvariant()
    u.Contains "PUBLIC" || u.Contains "509(A)(1)" || u.Contains "509(A)(2)"

let private notPublicClass =
    function
    | PrivateFoundation
    | SupportingOrgType3Nfi
    | Non501c3 -> true
    | _ -> false

let private routedToPerson (s: Screened) = not (isStraightThrough s)

/// Per-sponsor results. `score` maps a recipient name to its best sanctions score.
let sponsorResults
    (registry: Registry)
    (threshold: float)
    (score: string -> float)
    (returns: SponsorReturn list)
    : SponsorResult list =
    let paired = pairs returns |> List.filter (fst >> itemises)

    let keysOf (r: SponsorReturn) = r.Recipients |> List.map recipientKey |> Set.ofList

    let allKeysBySponsor =
        returns |> List.groupBy (fun r -> r.Ein) |> List.map (fun (e, rs) -> e, rs |> List.map keysOf |> Set.unionMany) |> Map.ofList

    paired
    |> List.map (fun (cur, prior) ->
        let screenWith policy = cur.Recipients |> List.map (screen policy threshold registry cur.PeriodEnd score)
        let std = screenWith Standard
        let con = screenWith Conservative
        let priorKeys = prior |> Option.map keysOf

        let othersKeys =
            allKeysBySponsor |> Map.toList |> List.filter (fun (e, _) -> e <> cur.Ein) |> List.map snd |> Set.unionMany

        let exc = std |> List.filter routedToPerson
        let excDollars = exc |> List.sumBy (fun s -> dollars s.Recipient)

        let reuseShare (known: Set<string>) =
            share (exc |> List.filter (fun s -> known.Contains(recipientKey s.Recipient)) |> List.length |> float) (float exc.Length)

        let reuseDollars (known: Set<string>) =
            share
                (exc |> List.filter (fun s -> known.Contains(recipientKey s.Recipient)) |> List.sumBy (fun s -> dollars s.Recipient) |> float)
                (float excDollars)

        let revs (r: Recipient) =
            r.Ein |> Option.bind registry.Revocations.TryFind |> Option.defaultValue []

        let whileRevoked =
            cur.Recipients |> List.choose (fun r -> r.Ein |> Option.bind (revokedAt registry cur.PeriodBegin))

        { Ein = cur.Ein
          Name = cur.Name
          TaxYear = cur.TaxYear
          PriorYear = prior |> Option.map (fun p -> p.TaxYear)
          Rows = cur.Recipients.Length
          RowsWithEin = cur.Recipients |> List.filter (fun r -> r.Ein.IsSome) |> List.length
          Dollars = cur.Recipients |> List.sumBy dollars
          ExceptionRows =
            Map.ofList
                [ "standard", std |> List.filter routedToPerson |> List.length
                  "conservative", con |> List.filter routedToPerson |> List.length ]
          ExceptionDollars =
            Map.ofList
                [ "standard", excDollars
                  "conservative", con |> List.filter routedToPerson |> List.sumBy (fun s -> dollars s.Recipient) ]
          Blocked = std |> List.filter (fun s -> match s.Route with Block _ -> true | _ -> false) |> List.length
          SanctionsAlerts = std |> List.filter (fun s -> s.SanctionsScore >= threshold) |> List.length
          LabelMismatch = std |> List.filter (fun s -> publicLabel s.Recipient.SectionDesc && notPublicClass s.Class) |> List.length
          ReuseWithinSponsor = priorKeys |> Option.map reuseShare
          ReuseWithinSponsorDollars = priorKeys |> Option.map reuseDollars
          ReuseAnySponsor = reuseShare (Set.union othersKeys (priorKeys |> Option.defaultValue Set.empty))
          RecipientOverlapPrior =
            priorKeys
            |> Option.map (fun pk ->
                share (cur.Recipients |> List.filter (recipientKey >> pk.Contains) |> List.length |> float) (float cur.Recipients.Length))
          RevokedDuringYear =
            cur.Recipients
            |> List.filter (fun r -> revs r |> List.exists (fun v -> v.Revoked >= cur.PeriodBegin && v.Revoked <= cur.PeriodEnd))
            |> List.length
          GrantedWhileRevoked = whileRevoked.Length
          GrantedWhileRevokedVisible =
            whileRevoked |> List.filter (fun v -> v.Posted |> Option.exists (fun p -> p <= cur.PeriodEnd)) |> List.length })

/// Pooled rows and dollars per recipient class (standard policy, current year, itemising sponsors).
let classBreakdown (registry: Registry) (returns: SponsorReturn list) : (string * int * decimal) list =
    pairs returns
    |> List.filter (fst >> itemises)
    |> List.collect (fun (cur, _) -> cur.Recipients)
    |> List.groupBy (classify registry)
    |> List.map (fun (c, rs) -> sprintf "%A" c, rs.Length, rs |> List.sumBy dollars)
    |> List.sortByDescending (fun (_, n, _) -> n)

type Pooled =
    { Sponsors: int
      Rows: int
      Dollars: decimal
      ExceptionShare: Map<string, float>
      ExceptionDollarShare: Map<string, float>
      PerSponsorExceptionShare: Summary
      ReuseWithinSponsor: float
      ReuseWithinSponsorDollars: float
      ReuseAnySponsor: float
      RecipientOverlapPrior: float
      RevokedDuringYearPer1000: float
      GrantedWhileRevoked: int
      GrantedWhileRevokedVisible: int
      LabelMismatch: int
      SanctionsAlertsPer1000Rows: float }

let pooled (rs: SponsorResult list) : Pooled =
    let rows = rs |> List.sumBy (fun r -> r.Rows) |> float
    let dollarsAll = rs |> List.sumBy (fun r -> r.Dollars)
    let excRows p = rs |> List.sumBy (fun r -> r.ExceptionRows.[p]) |> float
    let excDollars p = rs |> List.sumBy (fun r -> r.ExceptionDollars.[p])
    let withPrior = rs |> List.filter (fun r -> r.ReuseWithinSponsor.IsSome)

    let weighted (f: SponsorResult -> float option) (pool: SponsorResult list) =
        let w r = float r.ExceptionRows.["standard"]
        share (pool |> List.sumBy (fun r -> (f r |> Option.defaultValue 0.0) * w r)) (pool |> List.sumBy w)

    { Sponsors = rs.Length
      Rows = int rows
      Dollars = dollarsAll
      ExceptionShare = Map.ofList [ for p in [ "standard"; "conservative" ] -> p, share (excRows p) rows ]
      ExceptionDollarShare = Map.ofList [ for p in [ "standard"; "conservative" ] -> p, share (float (excDollars p)) (float dollarsAll) ]
      PerSponsorExceptionShare = rs |> List.map (fun r -> share (float r.ExceptionRows.["standard"]) (float r.Rows)) |> summarize
      ReuseWithinSponsor = weighted (fun r -> r.ReuseWithinSponsor) withPrior
      ReuseWithinSponsorDollars = weighted (fun r -> r.ReuseWithinSponsorDollars) withPrior
      ReuseAnySponsor = weighted (fun r -> Some r.ReuseAnySponsor) rs
      RecipientOverlapPrior =
        share
            (withPrior |> List.sumBy (fun r -> (r.RecipientOverlapPrior |> Option.defaultValue 0.0) * float r.Rows))
            (withPrior |> List.sumBy (fun r -> float r.Rows))
      RevokedDuringYearPer1000 = 1000.0 * share (rs |> List.sumBy (fun r -> r.RevokedDuringYear) |> float) rows
      GrantedWhileRevoked = rs |> List.sumBy (fun r -> r.GrantedWhileRevoked)
      GrantedWhileRevokedVisible = rs |> List.sumBy (fun r -> r.GrantedWhileRevokedVisible)
      LabelMismatch = rs |> List.sumBy (fun r -> r.LabelMismatch)
      SanctionsAlertsPer1000Rows = 1000.0 * share (rs |> List.sumBy (fun r -> r.SanctionsAlerts) |> float) rows }

/// Days from revocation effective date to IRS posting, for revocations effective on or after `since`.
let revocationLag (since: DateOnly) (entries: RevocationEntry seq) : Summary =
    entries
    |> Seq.filter (fun e -> e.Revoked >= since)
    |> Seq.choose (fun e -> e.Posted |> Option.map (fun p -> float (p.DayNumber - e.Revoked.DayNumber)))
    |> summarize

type SanctionsResult =
    { Method: string
      DistinctNames: int
      AlertsByThreshold: (float * int) list
      RecallByThreshold: (float * float) list
      RecallByVariant: (string * float) list
      InjectedNames: int
      SubgroupNames: int
      SubgroupAlertRate: float
      OtherAlertRate: float }

let variantName =
    function
    | 0 -> "exact"
    | 1 -> "case_and_punctuation"
    | 2 -> "dropped_character"
    | 3 -> "token_reorder"
    | _ -> "legal_suffix"

/// Names carrying Arabic-origin or Islamic tokens, used to measure whether alerts fall disproportionately on them.
let subgroupTokens = set [ "AL"; "ISLAMIC"; "MUSLIM"; "MUSLIMS"; "MASJID"; "MOSQUE"; "ARAB"; "ARABIC"; "IMAM"; "QURAN" ]

let inSubgroup (name: string) =
    NameMatch.tokens name |> List.exists subgroupTokens.Contains || name.ToUpperInvariant().Contains "AL-"

/// False-positive alert counts over real recipient names (presumed true negatives: IRS-recognised US recipients) and
/// recall over deterministically mutated listed entity names.
let sanctions
    (methodName: string)
    (thresholds: float list)
    (operating: float)
    (score: string -> float)
    (recipientNames: string list)
    (injected: (int * string) list)
    : SanctionsResult =
    let names = recipientNames |> List.distinct
    let scored = names |> List.map (fun n -> n, score n)
    let injectedScores = injected |> List.map (fun (v, n) -> v, score (NameMatch.mutate v n))
    let sub, other = scored |> List.partition (fst >> inSubgroup)
    let alertRate xs = share (xs |> List.filter (fun (_, s) -> s >= operating) |> List.length |> float) (float (List.length xs))

    { Method = methodName
      DistinctNames = names.Length
      AlertsByThreshold = thresholds |> List.map (fun t -> t, scored |> List.filter (fun (_, s) -> s >= t) |> List.length)
      RecallByThreshold =
        thresholds
        |> List.map (fun t -> t, share (injectedScores |> List.filter (fun (_, s) -> s >= t) |> List.length |> float) (float injected.Length))
      RecallByVariant =
        injectedScores
        |> List.groupBy (fun (v, _) -> v % 5)
        |> List.sortBy fst
        |> List.map (fun (v, xs) -> variantName v, share (xs |> List.filter (fun (_, s) -> s >= operating) |> List.length |> float) (float xs.Length))
      InjectedNames = injected.Length
      SubgroupNames = sub.Length
      SubgroupAlertRate = alertRate sub
      OtherAlertRate = alertRate other }

type Expect =
    | ExpectBlock
    | ExpectPerson
    | ExpectStraightThrough

type SafetyCase = { Category: string; Recipient: Recipient; Expect: Expect }

type SafetyResult = { Category: string; Expected: string; Cases: int; Caught: int; CorrectRoute: int }

let expectName =
    function
    | ExpectBlock -> "block"
    | ExpectPerson -> "person"
    | ExpectStraightThrough -> "straight_through"

/// Injected cases: bad cases must leave the straight-through path (revoked ones must block); control cases must not.
let safety (registry: Registry) (threshold: float) (asOf: DateOnly) (score: string -> float) (cases: SafetyCase list) : SafetyResult list =
    cases
    |> List.groupBy (fun c -> c.Category, c.Expect)
    |> List.sortBy (fun ((cat, _), _) -> cat)
    |> List.map (fun ((cat, expect), cs) ->
        let screened = cs |> List.map (fun c -> screen Standard threshold registry asOf score c.Recipient)

        { Category = cat
          Expected = expectName expect
          Cases = cs.Length
          Caught = screened |> List.filter (isStraightThrough >> not) |> List.length
          CorrectRoute =
            screened
            |> List.filter (fun s ->
                match expect, s.Route with
                | ExpectBlock, Block _
                | ExpectPerson, (Exception _ | Block _)
                | ExpectStraightThrough, StraightThrough -> true
                | _ -> false)
            |> List.length })

type Calibration =
    { Ein: string
      Name: string
      TaxYear: int
      DafCount: int option
      Employees: int option
      TotalExpenses: decimal option
      GrantsPaid: decimal option
      Salaries: decimal option
      DafGrants: decimal option
      DafAssets: decimal option
      ScheduleIRows: int
      FundsPerEmployee: float option
      NonGrantExpensePerFund: float option
      SalaryPerFund: float option }

let private ratio (a: decimal option) (b: int option) =
    match a, b with
    | Some x, Some y when y > 0 -> Some(float x / float y)
    | _ -> None

/// Incumbent operating ratios from each sponsor's latest return. Whole-organisation figures: community foundations
/// carry non-DAF programmes, and sponsors that outsource administration show contractor cost, not staff.
let calibration (returns: SponsorReturn list) : Calibration list =
    pairs returns
    |> List.map (fun (cur, _) ->
        { Ein = cur.Ein
          Name = cur.Name
          TaxYear = cur.TaxYear
          DafCount = cur.DafCount
          Employees = cur.Employees
          TotalExpenses = cur.TotalExpenses
          GrantsPaid = cur.GrantsPaid
          Salaries = cur.Salaries
          DafGrants = cur.DafGrants
          DafAssets = cur.DafAssets
          ScheduleIRows = cur.Recipients.Length
          FundsPerEmployee =
            match cur.DafCount, cur.Employees with
            | Some f, Some e when e > 0 -> Some(float f / float e)
            | _ -> None
          NonGrantExpensePerFund =
            ratio (Option.map2 (fun t g -> t - g) cur.TotalExpenses cur.GrantsPaid) cur.DafCount
          SalaryPerFund = ratio cur.Salaries cur.DafCount })
