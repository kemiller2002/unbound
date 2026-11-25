/// Synthetic operating year for the internal time study (OD-M-14). Pure: every random draw comes from a counter-based
/// generator keyed by (seed, entity, purpose), so output depends only on inputs and never on evaluation order.
module Unbound.Replay.Synthetic

open System
open System.Text.Json
open Unbound.Replay.Domain

// ---- Counter-based pseudo-random numbers (SplitMix64 finaliser) ----

let private mix64 (z0: uint64) =
    let z1 = (z0 ^^^ (z0 >>> 30)) * 0xbf58476d1ce4e5b9UL
    let z2 = (z1 ^^^ (z1 >>> 27)) * 0x94d049bb133111ebUL
    z2 ^^^ (z2 >>> 31)

/// Uniform draw in [0, 1) addressed by a seed and a key path.
let uniform (seed: uint64) (keys: int list) : float =
    keys
    |> List.fold (fun acc k -> mix64 (acc + 0x9e3779b97f4a7c15UL + uint64 (uint32 k))) (mix64 seed)
    |> fun x -> float (x >>> 11) / 9007199254740992.0

/// Poisson draw by inversion from one uniform.
let poisson (mean: float) (u: float) : int =
    let rec go k p cdf =
        if u < cdf || k > 10000 then k
        else
            let p' = p * mean / float (k + 1)
            go (k + 1) p' (cdf + p')

    if mean <= 0.0 then 0 else go 0 (exp -mean) (exp -mean)

/// Log-normal draw with the given arithmetic mean and log-scale sigma (Box-Muller from two uniforms).
let lognormal (mean: float) (sigma: float) (u1: float) (u2: float) : float =
    let z = sqrt (-2.0 * log (max u1 1e-300)) * cos (2.0 * Math.PI * u2)
    exp (log mean - sigma * sigma / 2.0 + sigma * z)

/// Index chosen from cumulative weights by one uniform.
let choose (cumulative: float array) (u: float) : int =
    let target = u * cumulative.[cumulative.Length - 1]

    match Array.BinarySearch(cumulative, target) with
    | i when i >= 0 -> min (i + 1) (cumulative.Length - 1)
    | i -> ~~~i |> min (cumulative.Length - 1)

/// Stable 32-bit FNV-1a hash of a string (String.GetHashCode is randomised per process).
let stableHash (s: string) : int =
    s |> Seq.fold (fun (h: uint32) ch -> (h ^^^ uint32 ch) * 16777619u) 2166136261u |> int

let cumulate (weights: float seq) : float array = weights |> Seq.scan (+) 0.0 |> Seq.skip 1 |> Array.ofSeq

// ---- Parameters (read from the economics model's parameters.json; single source of truth) ----

type Segment =
    { Name: string
      Grants: float
      AvgGrant: float
      Contribs: float
      AvgContrib: float
      Contacts: float
      ComplexShare: float
      CheckShare: float
      ContribMix: (string * float) list }

type Params =
    { Segments: Map<string, Segment>
      Mix: (string * float) list
      GrantsMult: float
      AvgGrantScale: float
      ContactsMult: float
      ComplexShareMult: float
      DecemberK: float
      DecemberDays: int
      ReconBreakRate: float
      AchReturnRate: float
      NewAccountShare: float
      MinutesPerException: float
      ContactMinutes: float }

/// OD-M-09 complex-asset policy as applied in model/enterprise_economics.py (COMPLEX_POLICY): sold on receipt through an
/// intermediary, so intake takes 1 hour and incidence is a quarter of the segment share.
let complexHours = 1.0
let complexShareFactor = 0.25

/// Parses parameters.json text (pure). `mix` names an entry of "mixes", e.g. "market".
let parseParams (mix: string) (json: string) : Params =
    use doc = JsonDocument.Parse json
    let root = doc.RootElement
    let p (name: string) = root.GetProperty("params").GetProperty(name).GetProperty("value").GetDouble()
    let num (e: JsonElement) (name: string) = e.GetProperty(name).GetDouble()

    let segments =
        root.GetProperty("segments").EnumerateObject()
        |> Seq.map (fun s ->
            let v = s.Value

            s.Name,
            { Name = s.Name
              Grants = num v "grants"
              AvgGrant = num v "avg_grant"
              Contribs = num v "contribs"
              AvgContrib = num v "avg_contrib"
              Contacts = num v "contacts"
              ComplexShare = num v "complex_share"
              CheckShare = num v "check_share"
              ContribMix = v.GetProperty("mix").EnumerateObject() |> Seq.map (fun m -> m.Name, m.Value.GetDouble()) |> List.ofSeq })
        |> Map.ofSeq

    { Segments = segments
      Mix = root.GetProperty("mixes").GetProperty(mix).EnumerateObject() |> Seq.map (fun m -> m.Name, m.Value.GetDouble()) |> List.ofSeq
      GrantsMult = p "grants_mult"
      AvgGrantScale = p "avg_grant_scale"
      ContactsMult = p "contacts_mult"
      ComplexShareMult = p "complex_share_mult"
      DecemberK = p "december_k"
      DecemberDays = int (p "december_days")
      ReconBreakRate = p "recon_break_rate"
      AchReturnRate = p "ach_return_rate"
      NewAccountShare = p "new_account_share"
      MinutesPerException = p "minutes_per_exception"
      ContactMinutes = p "contact_minutes" }

// ---- Configuration: values not in the economics model are illustrative and labelled so in report 16 ----

type Config =
    { Funds: int
      Seed: uint64
      Year: int
      /// Share of a fund's grants that go to its own recurring recipients [illustrative].
      PortfolioShare: float
      PortfolioSize: int * int
      GrantSigma: float
      ContribSigma: float
      /// Injected known-bad grants per 1,000 grants, per category [illustrative; for reviewer accuracy scoring].
      InjectPer1000: float
      /// Minutes to confirm a reused determination or disposition [illustrative].
      ReuseMinutes: float
      /// Minutes per reconciliation break and per ACH return [illustrative; BNK:121 gives 10-20 per return].
      OpsMinutes: float }

let defaultConfig =
    { Funds = 10000
      Seed = 20261125UL
      Year = 2025
      PortfolioShare = 0.6
      PortfolioSize = (3, 8)
      GrantSigma = 1.0
      ContribSigma = 1.2
      InjectPer1000 = 0.5
      ReuseMinutes = 5.0
      OpsMinutes = 15.0 }

// ---- Generation ----

type Fund = { Id: int; Segment: Segment; IsNew: bool; Portfolio: int list }

type Txn =
    | Contribution of method: string * complex: bool
    | Grant of recipient: Recipient * check: bool * injected: string option

type Event = { Day: int; Fund: int; Amount: decimal; Txn: Txn }

type WorkItem =
    { Day: int
      Kind: string
      Fund: int
      Reference: string
      Detail: string
      Injected: string option
      ModelMinutes: float }

let private daysIn year = if DateTime.IsLeapYear year then 366 else 365

/// Day-of-year weights: 1 per day, December surge days weighted by december_k (EM:60-68).
let seasonal (p: Params) (year: int) : float array =
    let n = daysIn year
    Array.init n (fun d -> if d >= n - p.DecemberDays then p.DecemberK else 1.0) |> cumulate

// Purpose tags keep draws for different decisions independent.
[<Literal>]
let private Seg = 1
[<Literal>]
let private New = 2
[<Literal>]
let private PortN = 3
[<Literal>]
let private PortPick = 4
[<Literal>]
let private NGrants = 5
[<Literal>]
let private NContribs = 6
[<Literal>]
let private NContacts = 7
[<Literal>]
let private GDay = 10
[<Literal>]
let private GAmt = 11
[<Literal>]
let private GAmt2 = 12
[<Literal>]
let private GPort = 13
[<Literal>]
let private GPick = 14
[<Literal>]
let private GCheck = 15
[<Literal>]
let private GInject = 16
[<Literal>]
let private GInjectPick = 17
[<Literal>]
let private CDay = 20
[<Literal>]
let private CAmt = 21
[<Literal>]
let private CAmt2 = 22
[<Literal>]
let private CMethod = 23
[<Literal>]
let private CComplex = 24
[<Literal>]
let private CReturn = 25
[<Literal>]
let private Recon = 26
[<Literal>]
let private KDay = 30
[<Literal>]
let private ODay = 31

let funds (p: Params) (c: Config) (pool: float array) : Fund list =
    let segs = p.Mix |> List.filter (fun (s, w) -> w > 0.0 && p.Segments.ContainsKey s)
    let segCum = segs |> List.map snd |> cumulate
    let lo, hi = c.PortfolioSize

    [ for f in 0 .. c.Funds - 1 ->
          let u k = uniform c.Seed [ f; k ]
          let seg = p.Segments.[fst segs.[choose segCum (u Seg)]]
          let n = lo + int (u PortN * float (hi - lo + 1))

          { Id = f
            Segment = seg
            IsNew = u New < p.NewAccountShare
            Portfolio = [ for i in 0 .. n - 1 -> choose pool (uniform c.Seed [ f; PortPick; i ]) ] } ]

/// All contributions and grants for the year. `pool` is the cumulative popularity of `recipients`; `injections` maps a
/// category to known-bad recipients.
let events
    (p: Params)
    (c: Config)
    (recipients: Recipient array)
    (pool: float array)
    (injections: (string * Recipient array) list)
    (fs: Fund list)
    : Event list =
    let days = seasonal p c.Year

    fs
    |> List.collect (fun f ->
        let s = f.Segment
        let u keys = uniform c.Seed (f.Id :: keys)

        let grants =
            [ for g in 0 .. poisson (s.Grants * p.GrantsMult) (u [ NGrants ]) - 1 ->
                  let ug k = u [ k; g ]

                  let injected =
                      injections
                      |> List.indexed
                      |> List.tryPick (fun (i, (cat, xs)) ->
                          if xs.Length > 0 && u [ GInject; g; i ] < c.InjectPer1000 / 1000.0 then
                              Some(cat, xs.[int (u [ GInjectPick; g; i ] * float xs.Length)])
                          else
                              None)

                  let recipient =
                      match injected with
                      | Some(_, r) -> r
                      | None when ug GPort < c.PortfolioShare && not f.Portfolio.IsEmpty ->
                          recipients.[f.Portfolio.[int (ug GPick * float f.Portfolio.Length)]]
                      | None -> recipients.[choose pool (ug GPick)]

                  ({ Day = choose days (ug GDay)
                     Fund = f.Id
                     Amount = decimal (Math.Round(lognormal (s.AvgGrant * p.AvgGrantScale) c.GrantSigma (ug GAmt) (ug GAmt2), 0))
                     Txn = Grant(recipient, ug GCheck < s.CheckShare, injected |> Option.map fst) }: Event) ]

        let methods = s.ContribMix |> List.map snd |> cumulate

        let contributions =
            [ for k in 0 .. poisson s.Contribs (u [ NContribs ]) - 1 ->
                  let uc t = u [ t; k ]

                  ({ Day = choose days (uc CDay)
                     Fund = f.Id
                     Amount = decimal (Math.Round(lognormal s.AvgContrib c.ContribSigma (uc CAmt) (uc CAmt2), 0))
                     Txn =
                       Contribution(
                           fst s.ContribMix.[choose methods (uc CMethod)],
                           uc CComplex < s.ComplexShare * p.ComplexShareMult * complexShareFactor
                       ) }: Event) ]

        grants @ contributions)
    |> List.sortBy (fun (e: Event) -> e.Day, e.Fund)

let private eventRef (e: Event) (i: int) = $"F{e.Fund:D6}-D{e.Day:D3}-{i}"

/// Routes grants through screening in date order, folding the set of recipients already decided at this sponsor
/// (CNV-GRT-006 disposition reuse within the deciding sponsor), and derives the other work items from model rates.
let workItems
    (p: Params)
    (c: Config)
    (screenGrant: Recipient -> Screened)
    (fs: Fund list)
    (evs: Event list)
    : WorkItem list =
    let grantItems, _ =
        evs
        |> List.indexed
        |> List.fold
            (fun (acc, decided: Set<string>) (i, e) ->
                match e.Txn with
                | Grant(r, _, injected) ->
                    let s = screenGrant r
                    let key = Analysis.recipientKey r

                    let item kind minutes detail =
                        { Day = e.Day
                          Kind = kind
                          Fund = e.Fund
                          Reference = eventRef e i
                          Detail = detail
                          Injected = injected
                          ModelMinutes = minutes }

                    match s.Route with
                    | StraightThrough -> (acc, decided)
                    | Block reason -> (item "block_review" p.MinutesPerException reason :: acc, decided)
                    | Exception reason when decided.Contains key -> (item "exception_reuse" c.ReuseMinutes reason :: acc, decided)
                    | Exception reason ->
                        let kind = if reason = "sanctions_alert" then "sanctions_alert" else "exception_first"
                        (item kind p.MinutesPerException reason :: acc, decided.Add key)
                | Contribution _ -> (acc, decided))
            ([], Set.empty)

    let txnItems =
        evs
        |> List.indexed
        |> List.collect (fun (i, e) ->
            let u k = uniform c.Seed [ e.Fund; k; i ]

            let item kind day minutes detail =
                { Day = day
                  Kind = kind
                  Fund = e.Fund
                  Reference = eventRef e i
                  Detail = detail
                  Injected = None
                  ModelMinutes = minutes }

            let recon = if u Recon < p.ReconBreakRate then [ item "recon_break" e.Day c.OpsMinutes "unmatched bank line" ] else []

            match e.Txn with
            | Contribution(m, complex) ->
                recon
                @ (if complex then [ item "complex_intake" e.Day (complexHours * 60.0) "complex asset, sold on receipt (OD-M-09)" ] else [])
                @ (if m = "ach" && u CReturn < p.AchReturnRate then [ item "ach_return" (min (e.Day + 3) (daysIn c.Year - 1)) c.OpsMinutes "ACH debit returned" ] else [])
            | Grant _ -> recon)

    let days = seasonal p c.Year

    let fundItems =
        fs
        |> List.collect (fun f ->
            let contacts =
                [ for k in 0 .. poisson (f.Segment.Contacts * p.ContactsMult) (uniform c.Seed [ f.Id; NContacts ]) - 1 ->
                      { Day = choose days (uniform c.Seed [ f.Id; KDay; k ])
                        Kind = "contact"
                        Fund = f.Id
                        Reference = $"F{f.Id:D6}-K{k}"
                        Detail = "donor or advisor contact needing a person"
                        Injected = None
                        ModelMinutes = p.ContactMinutes } ]

            let onboarding =
                if f.IsNew then
                    [ { Day = int (uniform c.Seed [ f.Id; ODay ] * float (daysIn c.Year))
                        Kind = "onboarding_review"
                        Fund = f.Id
                        Reference = $"F{f.Id:D6}-O"
                        Detail = "new fund: identity and sanctions screening result to confirm"
                        Injected = None
                        ModelMinutes = c.ReuseMinutes } ]
                else
                    []

            contacts @ onboarding)

    (List.rev grantItems @ txnItems @ fundItems) |> List.sortBy (fun w -> w.Day, w.Kind, w.Reference)

// ---- Summaries and the time-study sample ----

type Summary =
    { Funds: int
      Grants: int
      Contributions: int
      GrantDollars: decimal
      ContributionDollars: decimal
      DecemberGrantShare: float
      ItemsByKind: (string * int) list
      ItemsByMonth: (int * int) list
      PeakWeek: int * int
      MeanWeek: float
      ExceptionFirstPer1000Grants: float
      ExceptionReusePer1000Grants: float
      Injected: int
      /// Per injected category: grants injected, and how many surfaced as a work item (the rest passed straight through).
      InjectedOutcome: (string * int * int) list
      ModelHours: float }

let summarize (c: Config) (evs: Event list) (items: WorkItem list) : Summary =
    let grants = evs |> List.filter (fun e -> match e.Txn with Grant _ -> true | _ -> false)
    let contribs = evs |> List.filter (fun e -> match e.Txn with Contribution _ -> true | _ -> false)
    let start = DateOnly(c.Year, 1, 1)
    let month d = start.AddDays(d).Month
    let weekly = items |> List.countBy (fun w -> w.Day / 7) |> List.sortBy fst
    let count k = items |> List.filter (fun w -> w.Kind = k) |> List.length |> float

    { Funds = c.Funds
      Grants = grants.Length
      Contributions = contribs.Length
      GrantDollars = grants |> List.sumBy (fun e -> e.Amount)
      ContributionDollars = contribs |> List.sumBy (fun e -> e.Amount)
      DecemberGrantShare = Stats.share (grants |> List.filter (fun e -> month e.Day = 12) |> List.length |> float) (float grants.Length)
      ItemsByKind = items |> List.countBy (fun w -> w.Kind) |> List.sortBy fst
      ItemsByMonth = items |> List.countBy (fun w -> month w.Day) |> List.sortBy fst
      PeakWeek = weekly |> List.maxBy snd
      MeanWeek = weekly |> List.averageBy (snd >> float)
      ExceptionFirstPer1000Grants = 1000.0 * Stats.share (count "exception_first" + count "sanctions_alert") (float grants.Length)
      ExceptionReusePer1000Grants = 1000.0 * Stats.share (count "exception_reuse") (float grants.Length)
      Injected = items |> List.filter (fun w -> w.Injected.IsSome) |> List.length
      InjectedOutcome =
        grants
        |> List.choose (fun e -> match e.Txn with Grant(_, _, Some cat) -> Some cat | _ -> None)
        |> List.countBy id
        |> List.sortBy fst
        |> List.map (fun (cat, n) -> cat, n, items |> List.filter (fun w -> w.Injected = Some cat) |> List.length)
      ModelHours = (items |> List.sumBy (fun w -> w.ModelMinutes)) / 60.0 }

/// Stratified sample for timing: up to `perKind` items of each kind chosen by a seeded ordering, plus up to `injected`
/// injected cases (reviewers are blind to which; the answer key is produced separately).
let timeStudySample (c: Config) (perKind: int) (injected: int) (items: WorkItem list) : WorkItem list =
    let order (w: WorkItem) = uniform c.Seed [ 99; stableHash w.Reference; stableHash w.Kind ]
    let byOrder xs = xs |> List.sortBy (fun (w: WorkItem) -> order w, w.Reference)

    let strat =
        items
        |> List.filter (fun w -> w.Injected.IsNone)
        |> List.groupBy (fun w -> w.Kind)
        |> List.collect (fun (_, ws) -> ws |> byOrder |> List.truncate perKind)

    let bad = items |> List.filter (fun w -> w.Injected.IsSome) |> byOrder |> List.truncate injected
    (strat @ bad) |> List.sortBy (fun w -> w.Day, w.Reference)
