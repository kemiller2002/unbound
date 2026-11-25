/// ECN theoretical DAF enterprise simulation (Phase Five): port of
/// requirements/discovery/2026-11-24-simplification-economics/model/model.py.
///
/// Deterministic seeded Monte Carlo (one random.Random(seed) stream per run; per-cell seeds derived with zlib.crc32).
/// Every function is pure: random state is passed in and the advanced state returned. Arithmetic keeps the Python
/// model's operation order and grouping so that every float is bit-identical.
///
/// THEORETICAL MODEL: every output is a consequence of labelled assumptions (parameters.json).
module Unbound.Economics.Model

open System
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.Economics.PyCompat

let QUEUES = [ "exceptions"; "complex"; "service"; "ops" ]
let EXT_DAYS = 60 // simulate into the following January-March to measure clearance

type Rng = PyRandom.State
type Values = Map<string, Num>

// ----------------------------------------------------------------------------
// Parameters
// ----------------------------------------------------------------------------

let load (path: string) : Json = PyJson.loads (PyText.readText path)

let expectedValues (cfg: Json) : Values = items cfg?``params`` |> List.map (fun (k, s) -> k, num s?value) |> Map.ofList

/// Triangular(low, mode=value, high) for mc params; fixed otherwise (drawn in parameter order).
let sampleValues (cfg: Json) (rng: Rng) : Values * Rng =
    let draw (rng: Rng) (spec: Json) =
        let lo, hi, mode = num spec?low, num spec?high, num spec?value
        if (tryKey "mc" spec |> Option.exists truthy) && hi.V > lo.V then triangular lo hi (Num.min (Num.max mode lo) hi) rng
        else mode, rng
    let kvs, rng' =
        items cfg?``params``
        |> List.mapFold (fun rng (k, s) -> let x, rng' = draw rng s in (k, x), rng') rng
    Map.ofList kvs, rng'

let applyOverrides (values: Values) (overrides: Json) : Values =
    values
    |> Map.map (fun k v ->
        match tryKey k overrides with
        | None -> v
        | Some o ->
            match PyJson.field "set" o with
            | Some s -> num s
            | None -> v * num o?mult)

/// (master * 1000003 + crc32("|".join(str(p) for p in parts))) & 0xFFFFFFFF.
let seedFor (master: Numerics.BigInteger) (parts: string list) : Numerics.BigInteger =
    let key = String.Join("|", parts)
    (master * Numerics.BigInteger 1000003 + Numerics.BigInteger(Crc32.ofString key)) &&& Numerics.BigInteger 0xFFFFFFFFu

// ----------------------------------------------------------------------------
// Volumes (annual, by segment)
// ----------------------------------------------------------------------------

let RAIL_COST_KEYS =
    Map.ofList
        [ "ach", "ach_debit_cost"
          "check", "lockbox_check_cost"
          "wire", "wire_cost"
          "dtc", "dtc_receipt_cost"
          "payroll", "payroll_item_cost" ]

type Vol =
    { Accounts: Num
      Grants: Num
      Exceptions: Num
      Contribs: Num
      Complex: Num
      Contacts: Num
      ContactMinutes: Num
      Returns: Num
      Breaks: Num
      GrantRail: Num
      ContribRail: Num
      CardFeesDonor: Num
      Aum: Num
      GrantDollars: Num
      ContribDollars: Num
      AchDebitDollars: Num
      ComplexVendorFund: Num
      AdvisorAccounts: Num }

/// Applies f field by field (the Python code builds these dicts key by key).
let private volMap (f: Num list -> Num) (vs: Vol list) : Vol =
    let g (sel: Vol -> Num) = f (vs |> List.map sel)
    { Accounts = g _.Accounts
      Grants = g _.Grants
      Exceptions = g _.Exceptions
      Contribs = g _.Contribs
      Complex = g _.Complex
      Contacts = g _.Contacts
      ContactMinutes = g _.ContactMinutes
      Returns = g _.Returns
      Breaks = g _.Breaks
      GrantRail = g _.GrantRail
      ContribRail = g _.ContribRail
      CardFeesDonor = g _.CardFeesDonor
      Aum = g _.Aum
      GrantDollars = g _.GrantDollars
      ContribDollars = g _.ContribDollars
      AchDebitDollars = g _.AchDebitDollars
      ComplexVendorFund = g _.ComplexVendorFund
      AdvisorAccounts = g _.AdvisorAccounts }

let private getOr (key: string) (dflt: Num) (j: Json) = tryKey key j |> Option.map num |> Option.defaultValue dflt

let segmentVolumes (v: Values) (seg: Json) (n: Num) (act: Num) (outsourceComplex: bool) : Vol =
    let grants = n * num seg?grants * v.["grants_mult"] * act
    let exc = grants * Num.min (F 1.0) (v.["exception_rate"] * num seg?exc_mult) * (1.0 - v.["determination_reuse"])
    let contribs = n * num seg?contribs * act
    let complexN = contribs * Num.min (F 1.0) (num seg?complex_share * v.["complex_share_mult"])
    let simple = contribs - complexN
    let contacts = n * num seg?contacts * v.["contacts_mult"]
    let mix = seg?mix
    let achDebits = simple * getOr "ach" (F 0.0) mix
    let returns = achDebits * v.["ach_return_rate"]
    let payrollItems = simple * getOr "payroll" (F 0.0) mix
    let reconcilable = grants + contribs - payrollItems + payrollItems / v.["payroll_batch_size"]
    let breaks = reconcilable * v.["recon_break_rate"]
    let checkShare = Num.min (F 1.0) (num seg?check_share * v.["check_share_mult"])
    let grantRail = grants * ((1 - checkShare) * v.["ach_grant_cost"] + checkShare * v.["check_grant_cost"])
    let contribRail =
        items mix
        |> List.filter (fun (r, _) -> RAIL_COST_KEYS.ContainsKey r)
        |> Num.sumBy (fun (r, share) -> simple * num share * v.[RAIL_COST_KEYS.[r]])
    let avgGrant = num seg?avg_grant * v.["avg_grant_scale"]
    let cardFees = simple * getOr "card" (F 0.0) mix * num seg?avg_contrib * v.["card_fee_pct"]
    { Accounts = n
      Grants = grants
      Exceptions = exc
      Contribs = contribs
      Complex = complexN
      Contacts = contacts
      ContactMinutes = contacts * v.["contact_minutes"] * num seg?contact_min_mult
      Returns = returns
      Breaks = breaks
      GrantRail = grantRail
      ContribRail = contribRail
      CardFeesDonor = cardFees
      Aum = n * num seg?balance
      GrantDollars = grants * avgGrant
      ContribDollars = contribs * num seg?avg_contrib
      AchDebitDollars = achDebits * num seg?avg_contrib
      ComplexVendorFund = (if outsourceComplex then complexN * v.["complex_vendor_fee"] else F 0.0)
      AdvisorAccounts = (if tryKey "advisor" seg |> Option.exists truthy then n else F 0.0) }

let volumes (cfg: Json) (v: Values) (om: Json) (accounts: Num) (actMults: Map<string, Num>) : Vol * (string * Vol) list =
    let mix = cfg?mixes?(text om?mix)
    let perSeg =
        items mix
        |> List.map (fun (s, share) ->
            let act = actMults |> Map.tryFind s |> Option.defaultValue (F 1.0)
            s, segmentVolumes v cfg?segments?(s) (accounts * num share) act (truthy om?outsource_complex))
    volMap Num.sum (perSeg |> List.map snd), perSeg

// ----------------------------------------------------------------------------
// Staffing
// ----------------------------------------------------------------------------

let handleMinutes (v: Values) (vol: Vol) : Map<string, Num> =
    Map.ofList
        [ "exceptions", v.["minutes_per_exception"]
          "complex", v.["complex_hours"] * 60.0
          "service", (if vol.Contacts.V > 0.0 then vol.ContactMinutes / vol.Contacts else v.["contact_minutes"])
          "ops",
          (if (vol.Breaks + vol.Returns).V > 0.0 then
               (vol.Breaks * v.["break_minutes"] + vol.Returns * v.["return_minutes"]) / (vol.Breaks + vol.Returns)
           else v.["break_minutes"]) ]

let queueAnnualArrivals (vol: Vol) : Map<string, Num> =
    Map.ofList [ "exceptions", vol.Exceptions; "complex", vol.Complex; "service", vol.Contacts; "ops", vol.Breaks + vol.Returns ]

let QUEUE_FLOORS = Map.ofList [ "exceptions", 1; "complex", 1; "service", 1; "ops", 2 ]

let baseQueueFte (v: Values) (arrivals: Map<string, Num>) (handle: Map<string, Num>) : (string * Num) list =
    QUEUES
    |> List.map (fun q ->
        q,
        Num.max (I QUEUE_FLOORS.[q])
            (Num.ceil (arrivals.[q] * handle.[q] / 60.0 / v.["productive_hours"] * v.["staffing_headroom"] - 1e-9)))

/// Step-fixed roles reproducing the 09 baseline staffing table (FSM section 3).
let fixedRoles (accounts: Num) (platform: bool) : (string * Num) list =
    let lead = if accounts.V < 5000.0 then I 5 elif accounts.V < 50000.0 then I 8 else I 10
    let eng =
        Num.min (I 10) (4 + 2 * Num.max (I 0) (Num.roundInt (F(Math.Log10((Num.max accounts (I 1000) / 1000).V)))))
    let sec = if accounts.V < 100000.0 then I 1 else I 3
    let phil = if accounts.V < 100000.0 then I 1 else I 3
    [ "leadership", lead
      "engineering", eng
      "agent_ownership", I 4
      "security", sec
      "philanthropic_intelligence", phil ]
    @ (if platform then [] else [ "partnerships", Num.max (I 1) (Num.ceil (accounts / 20000)) ])

// ----------------------------------------------------------------------------
// Scenario helpers
// ----------------------------------------------------------------------------

/// A scenario effect (one dict of parameters.json scenarios[*].effects); absent keys raise when read, as in Python.
type Effect =
    { Type: string
      Raw: Json }

    member e.Get(key: string) : Num = num e.Raw?(key)
    member e.Opt(key: string) : Json option = tryKey key e.Raw

let parseEffects (effects: Json) : Effect list =
    PyJson.list effects |> List.map (fun e -> { Type = text e?``type``; Raw = e })

/// Pre-read numeric fields of the effects that the daily loop touches, so that the per-day work does no JSON lookups.
type DayEffect =
    { EType: string
      Queue: string option
      Day: Num
      Frac: Num
      Start: Num
      End: Num
      Value: Num
      Has: Set<string>
      Src: Effect }

let private dayEffect (e: Effect) : DayEffect =
    let get k = e.Opt k |> Option.map num |> Option.defaultValue (F nan)
    { EType = e.Type
      Queue = e.Opt "queue" |> Option.map text
      Day = get "day"
      Frac = get "frac"
      Start = get "start"
      End = get "end"
      Value = get "value"
      Has = items e.Raw |> List.map fst |> Set.ofList
      Src = e }

let private req (e: DayEffect) (key: string) (x: Num) : Num =
    if e.Has.Contains key then x else raise (Collections.Generic.KeyNotFoundException $"KeyError: '{key}'")

let accountsMultiplier (effects: DayEffect list) (day: int) : float =
    let m =
        effects
        |> List.fold
            (fun m e -> if e.EType = "accounts_step" && float day >= (req e "day" e.Day).V then m + (req e "frac" e.Frac).V else m)
            1.0
    if 0.0 > m then 0.0 else m

let accountYears (effects: DayEffect list) (accounts: Num) (days: int) : Num =
    accounts * F(fsum (Seq.init days (accountsMultiplier effects))) / days

let windowFactor (effects: DayEffect list) (etype: string) (queue: string) (day: int) : float =
    effects
    |> List.fold
        (fun f e ->
            if e.EType = etype then
                let q = match e.Queue with Some q -> q | None -> raise (Collections.Generic.KeyNotFoundException "KeyError: 'queue'")
                if (q = queue || q = "all") && (req e "start" e.Start).V <= float day && float day < (req e "end" e.End).V then
                    f * (req e "value" e.Value).V
                else f
            else f)
        1.0

let private fracSum (effects: DayEffect list) (pred: float -> bool) : Num =
    effects |> List.filter (fun x -> x.EType = "accounts_step" && pred (req x "frac" x.Frac).V) |> Num.sumBy _.Frac

let oneOffDaily (effects: DayEffect list) (queue: string) (day: int) (accounts: Num) (annualGrants: Num) (v: Values) : float =
    effects
    |> List.fold
        (fun total e ->
            let start = if e.Has.Contains "start" then e.Start.V else 0.0
            let stop = if e.Has.Contains "end" then e.End.V else -1.0
            if e.Queue <> Some queue || not (start <= float day && float day < stop) then total
            else
                let span = req e "end" e.End - req e "start" e.Start
                match e.EType with
                | "one_off" -> total + (e.Src.Get "per_account" * accounts / span).V
                | "one_off_new" -> total + (e.Src.Get "per_account" * accounts * fracSum effects (fun f -> f > 0.0) / span).V
                | "one_off_departing" ->
                    total + (e.Src.Get "per_account" * accounts * (-(fracSum effects (fun f -> f < 0.0))) / span).V
                | "one_off_from_grants" ->
                    let cases = annualGrants / v.["working_days"] * e.Src.Get "approved_days" * e.Src.Get "f"
                    total + (cases / span).V
                | _ -> total)
        0.0

let scenarioK (v: Values) (effects: Effect list) : Num =
    match effects |> List.filter (fun e -> e.Type = "december_k") with
    | [] -> v.["december_k"]
    | ks -> (List.last ks).Get "value"

let poisson (rng: Rng) (lam: float) : float * Rng =
    if lam <= 0.0 then 0.0, rng
    elif lam > 30.0 then
        let g, rng' = PyRandom.gauss lam (Math.Sqrt lam) rng
        (if g > 0.0 then g else 0.0), rng'
    else
        let limit = Math.Exp(-lam)
        let rec go (k: int) (p: float) (rng: Rng) =
            let u, rng' = PyRandom.random rng
            let p' = p * u
            if p' <= limit then float k, rng' else go (k + 1) p' rng'
        go 0 1.0 rng

// ----------------------------------------------------------------------------
// Daily queue simulation (backlog recursion B(d+1) = max(0, B(d) + a(d) - c(d)))
// ----------------------------------------------------------------------------

type Sim =
    { Arrivals: float
      Served: float
      PeakBacklog: float
      YearEndBacklog: float
      SlaAttainment: float
      LateArrivals: float
      P90WaitDays: float
      MeanWaitDays: float
      ContractorFteDays: float
      Utilisation: float
      BaseFte: Num
      SeasonalFteDays: Num
      RecoveryDays: Num
      Recovered: bool
      ExceededAfterWindow: bool
      EndBacklog: float }

let weightedPct (pairs: (float * float) list) (p: float) : float =
    let tot = fsum (pairs |> List.map snd)
    if tot <= 0.0 then 0.0
    else
        let sorted = pairs |> List.sortWith compare
        let rec go acc rest =
            match rest with
            | [] -> fst (List.last sorted)
            | (w, a) :: tail ->
                let acc' = acc + a
                if acc' >= p * tot then w else go acc' tail
        go 0.0 sorted

/// Loop state of simulate_queue (one value per day; never mutated).
type private QState =
    { Rng: Rng
      Backlog: float
      ServedTotal: float
      ArrivalsTotal: float
      Peak: float
      OkArr: float
      LateArr: float
      Waits: (float * float) list // reversed
      ContractorDays: float
      Contractors: float
      PendingAt: Num option
      BusyMin: float
      AvailMin: float
      Exceeded: bool
      RecoverDay: int option
      YeBacklog: float }

let simulateQueue
    (q: string)
    (v: Values)
    (om: Json)
    (annual: Num)
    (handle: Num)
    (baseFte: Num)
    (sla: Num)
    (effects: Effect list)
    (dayEffects: DayEffect list)
    (win: Num * Num)
    (accounts: Num)
    (annualGrants: Num)
    (rng: Rng)
    (noise: bool)
    : Sim * Rng =
    let days = int (Num.toInt v.["working_days"]).V
    let k = scenarioK v effects
    let decStart = days - int (Num.toInt v.["december_days"]).V
    let baseRate = annual / (I decStart + v.["december_days"] * k)
    let perFteHours = v.["productive_hours"] / days
    let seasonal = if q = "exceptions" || q = "complex" || q = "service" then v.["seasonal_frac"] * baseFte else F 0.0
    let contractorsOn = truthy om?contractors
    let win1 = snd win
    let inf = Double.PositiveInfinity
    let step (st: QState) (d: int) : QState =
        let inYear = d < days
        let inDec = decStart <= d && d < days
        let w = if inDec then k else F 1.0
        let lam = (baseRate * w).V * accountsMultiplier dayEffects d * windowFactor dayEffects "arrival_mult" q d
        let lam = lam + oneOffDaily dayEffects q d accounts annualGrants v
        let a, rng = if noise then poisson st.Rng lam else lam, st.Rng
        let h = (handle * windowFactor dayEffects "handle_mult" q d).V
        let avail = windowFactor dayEffects "capacity_mult" q d
        let staff = baseFte + (if inDec then seasonal else F 0.0)
        let perFteCap = (perFteHours * 60.0).V / h
        // surge response: contractors after lead time, capped
        let capNow = (staff + st.Contractors).V * perFteCap * avail
        let waitNow = if capNow > 0.0 then st.Backlog / capNow else inf
        let contractors, pendingAt =
            if not contractorsOn then st.Contractors, st.PendingAt
            else
                let pendingAt =
                    if waitNow > sla.V && st.PendingAt.IsNone && st.Contractors = 0.0 then Some(I d + v.["contractor_lead_days"])
                    else st.PendingAt
                match pendingAt with
                | Some p when float d >= p.V ->
                    let need = if st.Backlog > 0.0 then Num.ceil (F st.Backlog / (sla * perFteCap)) else I 0
                    let c = (Num.min (v.["contractor_max_frac"] * baseFte + 1) need).V
                    c, (if c <= 0.0 then None else pendingAt)
                | _ -> st.Contractors, pendingAt
        let cap = (staff + contractors).V * perFteCap * avail
        let work = st.Backlog + a
        let served = if cap < work then cap else work
        let backlog = work - served
        let wait = if cap > 0.0 then backlog / cap else inf
        let contractors, pendingAt = if contractors > 0.0 && backlog <= 0.5 * cap then 0.0, None else contractors, pendingAt
        let st =
            if inYear then
                { st with
                    ArrivalsTotal = st.ArrivalsTotal + a
                    ServedTotal = st.ServedTotal + served
                    Peak = (if backlog > st.Peak then backlog else st.Peak)
                    Waits = ((if 999.0 < wait then 999.0 else wait), a) :: st.Waits
                    OkArr = (if wait <= sla.V then st.OkArr + a else st.OkArr)
                    LateArr = (if wait <= sla.V then st.LateArr else st.LateArr + a)
                    ContractorDays = st.ContractorDays + contractors
                    BusyMin = st.BusyMin + served * h
                    AvailMin = st.AvailMin + cap * h }
            else st
        let afterWindow = float d >= win1.V
        { st with
            Rng = rng
            Backlog = backlog
            Contractors = contractors
            PendingAt = pendingAt
            YeBacklog = (if d = days - 1 then backlog else st.YeBacklog)
            Exceeded = st.Exceeded || (afterWindow && wait > sla.V)
            RecoverDay = (if afterWindow && st.RecoverDay.IsNone && wait <= 1.0 then Some d else st.RecoverDay) }
    let init =
        { Rng = rng
          Backlog = 0.0
          ServedTotal = 0.0
          ArrivalsTotal = 0.0
          Peak = 0.0
          OkArr = 0.0
          LateArr = 0.0
          Waits = []
          ContractorDays = 0.0
          Contractors = 0.0
          PendingAt = None
          BusyMin = 0.0
          AvailMin = 0.0
          Exceeded = false
          RecoverDay = None
          YeBacklog = 0.0 }
    let st = Seq.fold step init (seq { 0 .. days + EXT_DAYS - 1 })
    let waits = List.rev st.Waits
    let totalW = let t = fsum (waits |> List.map snd) in if t = 0.0 then 1.0 else t
    { Arrivals = st.ArrivalsTotal
      Served = st.ServedTotal
      PeakBacklog = st.Peak
      YearEndBacklog = st.YeBacklog
      SlaAttainment = (if st.ArrivalsTotal > 0.0 then st.OkArr / totalW else 1.0)
      LateArrivals = st.LateArr
      P90WaitDays = weightedPct waits 0.9
      MeanWaitDays = fsum (waits |> List.map (fun (w, a) -> w * a)) / totalW
      ContractorFteDays = st.ContractorDays
      Utilisation = (if st.AvailMin <> 0.0 then st.BusyMin / st.AvailMin else 0.0)
      BaseFte = baseFte
      SeasonalFteDays = seasonal * v.["december_days"]
      RecoveryDays =
        (match st.RecoverDay with
         | Some r -> I r - win1
         | None -> I EXT_DAYS + I days - win1)
      Recovered = st.RecoverDay.IsSome
      ExceededAfterWindow = st.Exceeded
      EndBacklog = st.Backlog },
    st.Rng

// ----------------------------------------------------------------------------
// Exposure model
// ----------------------------------------------------------------------------

type Exposure =
    { Rows: (string * Num * Num) list
      Gross: Num
      Uncontained: Num
      ControlEffectiveness: Num }

let exposures (v: Values) (vol: Vol) (specList: Json) (rng: Rng) (noise: bool) (accounts: Num) : Exposure * Rng =
    let days = v.["working_days"]
    let dailyGrantDollars = vol.GrantDollars / days
    // baseline exposures present in every scenario
    let attempts = accounts * v.["fraud_attempts_per_1000"] / 1000.0
    let returned = vol.AchDebitDollars * v.["ach_return_rate"]
    let baseline =
        [ "fraud_baseline", attempts * v.["fraud_avg_attempt"], attempts * v.["fraud_avg_attempt"] * v.["fraud_detection"]
          "contribution_returns_baseline", returned * v.["provisional_credit_exposure"], F 0.0 ]
    let one (rng: Rng) (s: Json) : (string * Num * Num) list * Rng =
        let kind = text s?kind
        let g (key: string) = num s?(key)
        match kind with
        | "overcommit" ->
            let gross = vol.Aum * g "share" * g "drawdown"
            [ kind, gross, gross * 0.9 ], rng
        | "revocation" ->
            let pipeline = dailyGrantDollars * g "pipeline_days" * g "revoked_share"
            let paidBeforeHold = dailyGrantDollars * g "lag_days" * g "revoked_share"
            let gross = pipeline + paidBeforeHold
            [ kind, gross, pipeline * g "registry_coverage" ], rng
        | "payment_failure" ->
            let atRisk = dailyGrantDollars * g "days" * g "fail_share"
            [ kind, atRisk, atRisk * (1 - g "duplicate_risk") ], rng
        | "contribution_reversal" ->
            let extra = vol.AchDebitDollars * v.["ach_return_rate"] * (g "return_mult" - 1) * g "days" / days
            [ kind, extra, extra * (1 - v.["provisional_credit_exposure"]) ], rng
        | "fraud" ->
            let att = attempts * g "mult" * g "days" / days
            [ kind, att * v.["fraud_avg_attempt"], att * v.["fraud_avg_attempt"] * v.["fraud_detection"] ], rng
        | "ai_error" ->
            let lo, mode, hi =
                match PyJson.list s?detect_days |> List.map num with
                | [ lo; mode; hi ] -> lo, mode, hi
                | other -> failwith $"ValueError: detect_days needs 3 values, got {other.Length}"
            let dd, rng = if noise then triangular lo hi mode rng else mode, rng
            let gross = dailyGrantDollars * g "f" * dd
            // post-payment: nothing caught before payment; recovery reduces loss
            [ kind, gross, gross * v.["post_payment_recovery"]
              "ai_error_excise", gross * v.["taxable_share"] * v.["excise_rate"], F 0.0 ],
            rng
        | "recon" ->
            let unrec = (vol.GrantDollars + vol.ContribDollars) / days * g "days" * v.["recon_break_rate"] * g "mult"
            [ kind, unrec, unrec * 0.98 ], rng
        | "cyber" -> [ kind, accounts * g "affected_share" * v.["cyber_cost_per_record"], F 0.0 ], rng
        | "data_corruption" ->
            let gross = (vol.GrantDollars + vol.ContribDollars) / days * g "days" * g "share"
            [ kind, gross, gross * 0.99 ], rng
        | _ -> [], rng
    let extra, rng' = PyJson.list specList |> List.mapFold one rng
    let rows = baseline @ List.concat extra
    let gross = rows |> Num.sumBy (fun (_, x, _) -> x)
    let caught = rows |> Num.sumBy (fun (_, _, c) -> c)
    { Rows = rows
      Gross = gross
      Uncontained = gross - caught
      ControlEffectiveness = (if gross.V > 0.0 then caught / gross else F 1.0) },
    rng'

let exposureJson (e: Exposure) : Json =
    jo
        [ "rows", jl (e.Rows |> List.map (fun (k, g, c) -> jo [ "kind", js k; "gross", jn g; "contained", jn c ]))
          "gross", jn e.Gross
          "uncontained", jn e.Uncontained
          "control_effectiveness", jn e.ControlEffectiveness ]

// ----------------------------------------------------------------------------
// Cost assembly
// ----------------------------------------------------------------------------

type Line =
    { Line: string
      Usd: Num
      Type: string
      Bearer: string
      Level: string }

let line (name: string) (usd: Num) (ctype: string) (bearer: string) (level: string) : Line =
    { Line = name; Usd = usd; Type = ctype; Bearer = bearer; Level = level }

let lineJson (l: Line) : Json =
    jo [ "line", js l.Line; "usd", jn l.Usd; "type", js l.Type; "bearer", js l.Bearer; "level", js l.Level ]

let assembleCosts
    (v: Values)
    (om: Json)
    (accounts: Num)
    (acctYears: Num)
    (vol: Vol)
    (sims: Map<string, Sim>)
    (baseFte: (string * Num) list)
    (effects: Effect list)
    (aumMult: Num)
    : Line list =
    let platform = truthy om?platform
    let opBearer = if platform then "platform_operator" else "sponsor"
    let lvl = if platform then "platform" else "sponsor"
    let loaded = v.["loaded_cost_per_fte"]
    let roles = fixedRoles accounts platform
    let fteOf q = baseFte |> List.find (fst >> (=) q) |> snd
    let staff = roles |> List.map (fun (r, n) -> line ("staff:" + r) (n * loaded) "step_fixed" opBearer lvl)
    let queues =
        QUEUES
        |> List.collect (fun q ->
            let s = sims.[q]
            let extra =
                (s.SeasonalFteDays * v.["december_seasonal_premium"] + F s.ContractorFteDays * v.["contractor_premium"])
                * loaded
                / v.["working_days"]
            [ line ("staff:queue:" + q) (fteOf q * loaded) "step_fixed(volume)" opBearer "account"
              line ("staff:surge:" + q) extra "variable(surge)" opBearer "account" ])
    let ai =
        F sims.["exceptions"].Arrivals * v.["ai_cost_per_dossier"]
        + acctYears * v.["ai_interactions_per_account"] * v.["ai_cost_per_interaction"]
        + vol.Grants * v.["ai_cost_per_grant_screen"]
    let scale = 1 + F(Math.Log10((Num.max accounts (I 1000) / 1000.0).V))
    let stepAccounts = effects |> List.filter (fun e -> e.Type = "accounts_step" && (e.Get "frac").V > 0.0)
    let newAccts = accounts * v.["new_account_share"] + accounts * (stepAccounts |> Num.sumBy (fun e -> e.Get "frac"))
    let cacBearer =
        match text om?cac_bearer with
        | "sponsor"
        | "sponsor_distribution" -> "sponsor"
        | "employer" -> "employer"
        | other -> raise (Collections.Generic.KeyNotFoundException $"KeyError: '{other}'")
    // advisor relationships
    let advisors = vol.AdvisorAccounts / v.["accounts_per_advisor"]
    let sponsorLevel =
        if platform then
            let sponsors = Num.max (I 1) (Num.ceil (accounts / v.["accounts_per_sponsor"]))
            [ line "sponsor_level:platform_service" (sponsors * (v.["sponsor_platform_cost"] + v.["sponsor_onboarding_cost"] / 5.0))
                  "step_fixed" "platform_operator" "sponsor"
              line "sponsor_level:entity_costs" (sponsors * v.["sponsor_entity_cost"]) "fixed" "sponsor" "sponsor" ]
        else []
    let exceptional =
        effects
        |> List.filter (fun e -> e.Type = "extra_cost")
        |> List.map (fun e -> line ("exceptional:" + text e.Raw?label) (e.Get "usd") "exceptional" opBearer "platform")
    staff
    @ queues
    @ [ line "ai_inference" ai "variable" opBearer "account"
        line "infrastructure_base" v.["infra_base"] "fixed" opBearer "platform"
        line "infrastructure_per_account" (acctYears * v.["infra_per_account"]) "variable" opBearer "account"
        line "payments:grants" vol.GrantRail "variable" opBearer "account"
        line "payments:contributions" vol.ContribRail "variable" opBearer "account"
        line "eligibility_screening" (vol.Grants * v.["routine_eligibility_per_grant"]) "variable" opBearer "account"
        line "recurring_obligations" (acctYears * v.["recurring_obligations_per_account"]) "variable" opBearer "account"
        line "insurance" (v.["insurance_base"] * scale) "step_fixed" opBearer lvl
        line "external_audit" (v.["audit_base"] + v.["audit_bps"] / 1e4 * vol.Aum * aumMult) "fixed" opBearer lvl
        line "legal_filings" (v.["legal_base"] * scale) "step_fixed" opBearer lvl
        line "data_providers" v.["data_base"] "fixed" opBearer "platform"
        line "identity_new_accounts" (newAccts * v.["identity_per_new"]) "acquisition" opBearer "account"
        line "customer_acquisition" (newAccts * v.["cac"]) "acquisition" cacBearer "account"
        line "advisor_relationships" (advisors * (v.["advisor_relationship_cost"] + v.["advisor_onboarding_cost"] * 0.2))
            "variable" opBearer "advisor"
        // fund-borne
        line "custody" (v.["custody_bps"] / 1e4 * vol.Aum * aumMult) "variable" "fund" "account"
        line "pool_investment_expense" (v.["pool_expense_bps"] / 1e4 * vol.Aum * aumMult) "variable" "fund" "account"
        line "complex_asset_vendor" vol.ComplexVendorFund "variable" "fund" "account"
        line "card_fees" vol.CardFeesDonor "variable" "donor_or_fund" "account" ]
    // sponsor-level (platform model only)
    @ sponsorLevel
    @ exceptional

let summariseCosts (lines: Line list) (acctYears: Num) (txns: Num) (platform: bool) (accounts: Num) (v: Values) : (string * Num) list =
    let total (pred: Line -> bool) = lines |> List.filter pred |> Num.sumBy _.Usd
    let operator (l: Line) = l.Bearer = "sponsor" || l.Bearer = "platform_operator"
    let opexExAcq = total (fun l -> operator l && l.Type <> "acquisition")
    let acq = total (fun l -> l.Type = "acquisition" && l.Line = "customer_acquisition")
    let ident = total (fun l -> l.Line = "identity_new_accounts")
    let fund = total (fun l -> l.Bearer = "fund" || l.Bearer = "donor_or_fund")
    let sponsorLevel = total (fun l -> l.Level = "sponsor" && operator l)
    let accountLevel = total (fun l -> (l.Level = "account" || l.Level = "advisor") && operator l && l.Type <> "acquisition")
    let custody = total (fun l -> l.Line = "custody")
    let sponsors = if platform then Num.max (I 1) (Num.ceil (accounts / v.["accounts_per_sponsor"])) else I 1
    let own = if platform then "platform_operator" else "sponsor"
    [ "opex_ex_acq", opexExAcq + ident
      "operator_only_per_account", (total (fun l -> l.Bearer = own && l.Type <> "acquisition") + ident) / acctYears
      "cost_per_account", (opexExAcq + ident) / acctYears
      "cost_per_account_incl_acq", (opexExAcq + ident + acq) / acctYears
      "baseline09_comparable_per_account", (opexExAcq + ident + acq + custody) / acctYears
      "account_level_per_account", accountLevel / acctYears
      "platform_fixed_per_account", total (fun l -> l.Level = "platform" && operator l) / acctYears
      "sponsor_level_per_sponsor", sponsorLevel / sponsors
      "sponsor_level_per_account", sponsorLevel / acctYears
      "fund_borne_per_account", fund / acctYears
      "acquisition_per_account", acq / acctYears
      "cost_per_transaction", (if Num.truthy txns then (opexExAcq + ident) / txns else F 0.0)
      "sponsors", sponsors
      "exceptional_usd", total (fun l -> l.Type = "exceptional")
      "surge_staff_usd", total (fun l -> l.Line.StartsWith "staff:surge") ]

// ----------------------------------------------------------------------------
// One run
// ----------------------------------------------------------------------------

type Run =
    { Values: Values
      Volumes: Vol
      PerSegment: (string * Vol) list
      Lines: Line list
      Metrics: (string * Num) list
      Sims: Map<string, Sim>
      Exposure: Exposure
      BaseFte: (string * Num) list }

    member r.Metric(name: string) : Num =
        match r.Metrics |> List.tryFind (fst >> (=) name) with
        | Some(_, x) -> x
        | None -> raise (Collections.Generic.KeyNotFoundException $"KeyError: '{name}'")

let metricOf (metrics: (string * Num) list) (name: string) : Num = metrics |> List.find (fst >> (=) name) |> snd

let runOnce
    (cfg: Json)
    (omKey: string)
    (accounts: Num)
    (scenKey: string)
    (rng: Rng)
    (noise: bool)
    (valueOverride: (string * Num) list)
    : Run * Rng =
    let om = cfg?operating_models?(omKey)
    let scen = cfg?scenarios?(scenKey)
    let baseV, rng = if noise then sampleValues cfg rng else expectedValues cfg, rng
    let baseV = valueOverride |> List.fold (fun (m: Values) (k, x) -> m.Add(k, x)) baseV
    let v = applyOverrides baseV om?overrides
    let effects = parseEffects scen?effects
    let dayEffects = effects |> List.map dayEffect
    let mix = cfg?mixes?(text om?mix)
    let actList, rng =
        keys mix
        |> List.mapFold
            (fun rng s ->
                if noise then
                    let g, rng' = PyRandom.gauss 0.0 0.15 rng
                    (s, F(Math.Exp g)), rng'
                else (s, F 1.0), rng)
            rng
    let vol, perSeg = volumes cfg v om accounts (Map.ofList actList)
    let aumMult = effects |> List.fold (fun m e -> if e.Type = "aum_mult" then m * e.Get "value" else m) (F 1.0)
    let handle = handleMinutes v vol
    let arrivals = queueAnnualArrivals vol
    let baseFte = baseQueueFte v arrivals handle
    let sla =
        Map.ofList
            [ "exceptions", v.["sla_exceptions_days"]
              "complex", v.["sla_complex_days"]
              "service", v.["sla_service_days"]
              "ops", v.["sla_ops_days"] ]
    let win =
        match PyJson.list scen?window |> List.map num with
        | a :: b :: _ -> a, b
        | _ -> failwith "IndexError: scenario window needs two values"
    let simList, rng =
        QUEUES
        |> List.mapFold
            (fun rng q ->
                let s, rng' =
                    simulateQueue q v om arrivals.[q] handle.[q] (snd (List.find (fst >> (=) q) baseFte)) sla.[q] effects dayEffects win
                        accounts vol.Grants rng noise
                (q, s), rng')
            rng
    let sims = Map.ofList simList
    let simsInOrder = simList |> List.map snd
    let days = int (Num.toInt v.["working_days"]).V
    let ay = accountYears dayEffects accounts days
    let scaleVol = ay / accounts
    let volS = volMap (fun xs -> let x = List.head xs in if x.IsInt then x else x * scaleVol) [ vol ]
    let lines = assembleCosts v om accounts ay volS sims baseFte effects aumMult
    let txns = volS.Grants + volS.Contribs
    let cs = summariseCosts lines ay txns (truthy om?platform) accounts v
    let exp, rng = exposures v volS scen?exposure rng noise accounts
    let human = simsInOrder |> Num.sumBy (fun s -> F s.Arrivals)
    let staffFte = (fixedRoles accounts (truthy om?platform) |> Num.sumBy snd) + (baseFte |> Num.sumBy snd)
    let surgeFte = (simsInOrder |> Num.sumBy (fun s -> F s.ContractorFteDays + s.SeasonalFteDays)) / days
    let lateExc = sims.["exceptions"].LateArrivals + sims.["complex"].LateArrivals
    let friction = (sims.["service"].Arrivals + lateExc + sims.["ops"].Arrivals * 0.5) / ay.V * 1000.0
    let recovery = simsInOrder |> List.map _.RecoveryDays |> Num.maxOf
    let unresolvedItems = simsInOrder |> Num.sumBy (fun s -> F s.EndBacklog)
    let slaOverall =
        if Num.truthy human then (simsInOrder |> Num.sumBy (fun s -> F(s.SlaAttainment * s.Arrivals))) / human else F 1.0
    let excRate = if Num.truthy volS.Grants then F sims.["exceptions"].Arrivals / volS.Grants else F 0.0
    let metrics =
        cs
        @ [ "human_interventions", human
            "human_interventions_per_1000_accounts", human / ay * 1000.0
            "exception_rate_per_grant", excRate
            "peak_backlog_exceptions", F sims.["exceptions"].PeakBacklog
            "peak_backlog_total", simsInOrder |> Num.sumBy (fun s -> F s.PeakBacklog)
            "p90_wait_days_exceptions", F sims.["exceptions"].P90WaitDays
            "p90_wait_days_service", F sims.["service"].P90WaitDays
            "sla_attainment_overall", slaOverall
            "sla_attainment_exceptions", F sims.["exceptions"].SlaAttainment
            "recovery_days_max_queue", recovery
            "financial_exposure_gross", exp.Gross
            "financial_exposure_uncontained", exp.Uncontained
            "control_effectiveness_proxy", exp.ControlEffectiveness
            "staff_fte_base", staffFte
            "surge_fte_equivalent", surgeFte
            "utilisation_exceptions", F sims.["exceptions"].Utilisation
            "customer_friction_per_1000", F friction
            "unresolved_backlog_items_after_ext", unresolvedItems
            "unresolved_risk_usd", exp.Uncontained
            "accounts_year_avg", ay
            "grants", volS.Grants
            "contributions", volS.Contribs
            "aum", volS.Aum * aumMult ]
    { Values = v
      Volumes = volS
      PerSegment = perSeg
      Lines = lines
      Metrics = metrics
      Sims = sims
      Exposure = exp
      BaseFte = baseFte },
    rng

/// random.Random(0): deterministic runs never draw from it.
let zeroRng : Rng = PyRandom.seedInt 0L

/// run_once(cfg, om, accounts, scen, random.Random(0), noise=False, value_override=override).
let runDet (cfg: Json) (omKey: string) (accounts: Num) (scenKey: string) (valueOverride: (string * Num) list) : Run =
    fst (runOnce cfg omKey accounts scenKey zeroRng false valueOverride)

// ----------------------------------------------------------------------------
// Aggregation helpers
// ----------------------------------------------------------------------------

let pct (xs: Num list) (p: float) : Num =
    let s = xs |> sortByNum id |> Array.ofList
    if s.Length = 0 then F 0.0
    else
        let i = p * float (s.Length - 1)
        let lo, hi = int (Math.Floor i), int (Math.Ceiling i)
        s.[lo] + (s.[hi] - s.[lo]) * (i - float lo)

let summariseRuns (runs: Run list) : (string * (string * Num) list) list =
    let ks = (List.head runs).Metrics |> List.map fst
    ks
    |> List.map (fun k ->
        let xs = runs |> List.map (fun r -> r.Metric k)
        k, [ "p10", pct xs 0.1; "p50", pct xs 0.5; "p90", pct xs 0.9 ])

let mc (cfg: Json) (omKey: string) (accounts: Num) (scenKey: string) (n: int) : Run list =
    let master =
        match cfg?meta?seed with
        | JInt i -> i
        | other -> failwith $"TypeError: seed must be an int, got {other}"
    // common random numbers: run i uses the same seed in every scenario, so scenario deltas are paired
    parallelInit n (fun i ->
        let rng = seeded (seedFor master [ omKey; Num.str accounts; string i ])
        fst (runOnce cfg omKey accounts scenKey rng true []))

let ranks (xs: Num list) : float list =
    let arr = Array.ofList xs
    let order = List.init arr.Length id |> sortByNum (fun i -> arr.[i])
    let r = order |> List.mapi (fun pos i -> i, float pos) |> Map.ofList
    List.init arr.Length (fun i -> r.[i])

let spearman (xs: Num list) (ys: Num list) : float =
    let rx, ry = ranks xs, ranks ys
    let n = float xs.Length
    let mx, my = fsum rx / n, fsum ry / n
    let cov = fsum (List.map2 (fun a b -> (a - mx) * (b - my)) rx ry)
    let vx = Math.Sqrt(fsum (rx |> List.map (fun a -> Math.Pow(a - mx, 2.0))))
    let vy = Math.Sqrt(fsum (ry |> List.map (fun b -> Math.Pow(b - my, 2.0))))
    if vx <> 0.0 && vy <> 0.0 then cov / (vx * vy) else 0.0

let tornado (cfg: Json) (omKey: string) (accounts: Num) (metric: string) : Json =
    let baseM = (runDet cfg omKey accounts "normal" []).Metric metric
    let rows =
        items cfg?``params``
        |> List.filter (fun (_, s) -> num s?low <> num s?high)
        |> List.map (fun (k, s) ->
            let lo = (runDet cfg omKey accounts "normal" [ k, num s?low ]).Metric metric
            let hi = (runDet cfg omKey accounts "normal" [ k, num s?high ]).Metric metric
            let swing = Num.abs (hi - lo)
            swing,
            jo
                [ "param", js k
                  "label", s?label
                  "low_input", s?low
                  "high_input", s?high
                  "metric_at_low", jn lo
                  "metric_at_high", jn hi
                  "swing", jn swing
                  "swing_pct_of_base", jn (if Num.truthy baseM then Num.abs (hi - lo) / baseM * 100 else F 0.0) ])
        |> sortByNum (fun (sw, _) -> -sw)
    jo [ "om", js omKey; "accounts", jn accounts; "metric", js metric; "base", jn baseM; "rows", jl (rows |> List.map snd) ]

// ----------------------------------------------------------------------------
// Revenue models
// ----------------------------------------------------------------------------

let tieredFee (balance: Num) (tiers: Json) : Num =
    let rec go (fee: Num) (rem: Num) (tiers: Json list) =
        match tiers with
        | [] -> fee
        | t :: rest ->
            let width, bps =
                match PyJson.list t |> List.map num with
                | [ w; b ] -> w, b
                | _ -> failwith "ValueError: a tier needs (width, bps)"
            let take = Num.min rem width
            let fee = fee + take * bps / 1e4
            let rem = rem - take
            if rem.V <= 0.0 then fee else go fee rem rest
    go (F 0.0) balance (PyJson.list tiers)

let rec revenuePerAccount (cfg: Json) (modelKey: string) (segKey: string) : Num =
    let m = cfg?revenue_models?(modelKey)
    let seg = cfg?segments?(segKey)
    let b = num seg?balance
    if (PyJson.field "tiers_bps" m).IsSome then Num.max (num m?minimum) (tieredFee b m?tiers_bps)
    else
        match modelKey with
        | "asset_flat_no_min" -> b * num m?flat_bps / 1e4
        | "subscription" -> getOr segKey (I 60) m?per_segment_annual
        | "transaction" ->
            num seg?grants * num m?per_grant
            + num seg?contribs * Num.min (num m?contrib_cap) (num seg?avg_contrib * num m?contrib_pct)
        | "employer_sponsored" ->
            if segKey = "workplace" then num m?employer_per_workplace_account else revenuePerAccount cfg (text m?``else``) segKey
        | "advisor_sponsored" ->
            if tryKey "advisor" seg |> Option.exists truthy then
                Num.max (I 250) (tieredFee b m?advisor_tiers_bps) * num m?platform_share
            else revenuePerAccount cfg (text m?``else``) segKey
        | "institutional_licensing" -> num m?per_account
        | "hybrid" -> num m?``base`` + b * num m?flat_bps / 1e4
        | "transparent_cross_subsidy" -> if b.V >= (num m?threshold).V then b * num m?flat_bps / 1e4 else num m?below_threshold_fee
        | _ -> raise (Collections.Generic.KeyNotFoundException $"KeyError: '{modelKey}'")

/// Deterministic marginal (variable) cost per account by segment, plus fixed share.
let segmentCostToServe (cfg: Json) (omKey: string) (accounts: Num) : Json =
    let r = runDet cfg omKey accounts "normal" []
    let v = r.Values
    let loadedMin = v.["loaded_cost_per_fte"] / (v.["productive_hours"] * 60.0)
    let fixedTotal =
        r.Lines
        |> List.filter (fun l ->
            (l.Bearer = "sponsor" || l.Bearer = "platform_operator")
            && (l.Level = "platform" || l.Level = "sponsor")
            && l.Type <> "acquisition")
        |> Num.sumBy _.Usd
    let fixedPa = fixedTotal / accounts
    let segs =
        r.PerSegment
        |> List.filter (fun (_, d) -> not (d.Accounts.V <= 0.0))
        |> List.map (fun (s, d) ->
            let n = d.Accounts
            let labourMin =
                d.Exceptions * v.["minutes_per_exception"] + d.Complex * v.["complex_hours"] * 60 + d.ContactMinutes
                + d.Breaks * v.["break_minutes"]
                + d.Returns * v.["return_minutes"]
            let labour = labourMin * loadedMin * v.["staffing_headroom"]
            let nonlab =
                d.GrantRail + d.ContribRail + d.Grants * (v.["routine_eligibility_per_grant"] + v.["ai_cost_per_grant_screen"])
                + d.Exceptions * v.["ai_cost_per_dossier"]
                + n * (v.["infra_per_account"] + v.["recurring_obligations_per_account"] + v.["ai_interactions_per_account"] * v.["ai_cost_per_interaction"])
            let marginal = (labour + nonlab) / n
            s,
            jo
                [ "marginal_variable_per_account", jn marginal
                  "labour_per_account", jn (labour / n)
                  "nonlabour_per_account", jn (nonlab / n)
                  "fully_allocated_per_account", jn (marginal + fixedPa)
                  "fund_borne_per_account", jn ((d.Aum * (v.["custody_bps"] + v.["pool_expense_bps"]) / 1e4 + d.ComplexVendorFund) / n)
                  "balance", cfg?segments?(s)?balance
                  "revenue", jo (keys cfg?revenue_models |> List.map (fun m -> m, jn (revenuePerAccount cfg m s))) ])
    jo [ "om", js omKey; "accounts", jn accounts; "fixed_per_account", jn fixedPa; "segments", jo segs ]

// ----------------------------------------------------------------------------
// Marginal cost floor
// ----------------------------------------------------------------------------

let marginalFloor (cfg: Json) (omKey: string) : Json =
    let v = applyOverrides (expectedValues cfg) cfg?operating_models?(omKey)?overrides
    let lm = v.["loaded_cost_per_fte"] / (v.["productive_hours"] * 60.0)
    let eEff = v.["exception_rate"] * (1 - v.["determination_reuse"])
    let row (kvs: (string * Num) list) (basis: string) = jo ((kvs |> List.map (fun (k, x) -> k, jn x)) @ [ "basis", js basis ])
    let rows =
        jo
            [ "additional_account_per_year",
              row
                  [ "theoretical", v.["infra_per_account"] * 0.1 + 0.0
                    "practical",
                    v.["infra_per_account"] + v.["recurring_obligations_per_account"]
                    + v.["ai_interactions_per_account"] * v.["ai_cost_per_interaction"]
                    + 0.3 * v.["contacts_mult"] * v.["contact_minutes"] * lm
                    "one_off_onboarding", v.["identity_per_new"] ]
                  "theoretical = 10% of infra_per_account (storage/compute only; BENCH shows compute not binding) [A]; practical adds recurring obligations, AI service and 0.3 x contacts_mult human contacts"
              "contribution_ach",
              row
                  [ "theoretical", v.["ach_debit_cost"]
                    "practical",
                    v.["ach_debit_cost"] + v.["ach_return_rate"] * (3.5 + v.["return_minutes"] * lm)
                    + v.["recon_break_rate"] * v.["break_minutes"] * lm ]
                  "ACH entry + expected return handling ($3.5 midpoint of BNK $2-5) + expected break"
              "contribution_payroll",
              row
                  [ "theoretical", v.["payroll_item_cost"]
                    "practical", v.["payroll_item_cost"] + v.["recon_break_rate"] * 0.1 * v.["break_minutes"] * lm ]
                  "batched employer file; breaks reconciled per batch [A]"
              "contribution_check_lockbox",
              row
                  [ "theoretical", v.["lockbox_check_cost"]
                    "practical", v.["lockbox_check_cost"] + 3 * lm + v.["recon_break_rate"] * v.["break_minutes"] * lm ]
                  "Sunwest $0.65 [S] + 3 min manual exception [A]"
              "grant_ach",
              row
                  [ "theoretical", v.["ach_grant_cost"] + v.["routine_eligibility_per_grant"] + v.["ai_cost_per_grant_screen"]
                    "practical",
                    v.["ach_grant_cost"] + v.["routine_eligibility_per_grant"] + v.["ai_cost_per_grant_screen"]
                    + eEff * (v.["minutes_per_exception"] * lm * v.["staffing_headroom"] + v.["ai_cost_per_dossier"])
                    + v.["recon_break_rate"] * v.["break_minutes"] * lm ]
                  "rail + routine eligibility ($0.14 [S] GRT:206) + AI screen + expected exception labour"
              "grant_check",
              row [ "theoretical", v.["check_grant_cost"]; "practical", v.["check_grant_cost"] + F 0.02 * I 25 ]
                  "all-in check + 2% reissue at $25 [A] (BNK:123)"
              "charity_verification_routine",
              row [ "theoretical", v.["routine_eligibility_per_grant"]; "practical", v.["routine_eligibility_per_grant"] ]
                  "$0.14 per listed domestic public charity check [S]; reusable across all grants to that EIN within validity"
              "charity_verification_nonstandard",
              row [ "theoretical", I 4000 / 2.0 / 12.5; "practical", I 4000 / 2.0 ]
                  "RCF typical international ~$4k [S] GRT:208; ED validity ~2 yrs [S]; theoretical spreads over ~12.5 grants per recipient (FSM:91) [S]"
              "sponsor_per_year",
              row
                  [ "theoretical", v.["sponsor_platform_cost"] * 0.2
                    "practical", v.["sponsor_platform_cost"] + v.["sponsor_onboarding_cost"] / 5.0
                    "sponsor_entity_cost_borne_by_sponsor", v.["sponsor_entity_cost"] ]
                  "theoretical = default policy pack with no sponsor-specific configuration [A]"
              "advisor_relationship_per_year",
              row
                  [ "theoretical", v.["advisor_relationship_cost"] * 0.2
                    "practical", v.["advisor_relationship_cost"] + v.["advisor_onboarding_cost"] / 5.0 ]
                  "[A] firm-level federation and fee feed vs per-advisor manual entitlement"
              "payment_failure_case",
              row [ "theoretical", v.["return_minutes"] * lm * 0.2; "practical", v.["return_minutes"] * lm + 3.5 ]
                  "[A] auto-retry with typed return codes vs manual"
              "service_case_ai_resolved",
              row [ "theoretical", v.["ai_cost_per_interaction"]; "practical", F 0.50 ]
                  "SVC:172 <$0.50 plus corpus governance (illustrative)"
              "service_case_human_async",
              row [ "theoretical", F 7.0; "practical", v.["contact_minutes"] * lm ]
                  "SVC:170-171 async ~$7 at $1/min (illustrative); practical at modelled minutes x loaded rate"
              "exception_human",
              row
                  [ "theoretical", v.["minutes_per_exception"] * lm
                    "practical", v.["minutes_per_exception"] * lm * v.["staffing_headroom"] + v.["ai_cost_per_dossier"] ]
                  "minutes x loaded $/min; SVC:173 ~$46 illustrative" ]
    jo [ "om", js omKey; "loaded_cost_per_minute", jn lm; "rows", rows ]

// ----------------------------------------------------------------------------
// Target engineering and capital
// ----------------------------------------------------------------------------

let detCost (cfg: Json) (omKey: string) (accounts: Num) (overrides: (string * Num) list) (metric: string) : Num =
    (runDet cfg omKey accounts "normal" overrides).Metric metric

let minScaleFor (cfg: Json) (omKey: string) (target: Num) (grid: Num list) : Num option =
    grid |> List.tryFind (fun a -> (detCost cfg omKey a [] "cost_per_account").V <= target.V)

/// Bisection on minutes_per_exception (others expected) to hit target cost/account.
let maxExceptionMinutesPerGrant (cfg: Json) (omKey: string) (accounts: Num) (target: Num) : Json =
    let om = cfg?operating_models?(omKey)
    let baseV = expectedValues cfg
    let mult =
        match tryKey "minutes_per_exception" om?overrides with
        | Some o -> getOr "mult" (F 1.0) o
        | None -> F 1.0
    let cost (m: Num) = detCost cfg omKey accounts [ "minutes_per_exception", m ] "cost_per_account"
    if (cost (F 1e-6)).V > target.V then JNull
    else
        let lo, _ =
            List.fold
                (fun (lo: Num, hi: Num) _ ->
                    let mid = (lo + hi) / 2
                    if (cost mid).V <= target.V then mid, hi else lo, mid)
                (F 0.0, F 2000.0)
                [ 1..40 ]
        let v = applyOverrides (baseV.Add("minutes_per_exception", lo)) om?overrides
        jo
            [ "minutes_per_exception_input", jn lo
              "effective_minutes", jn (lo * mult)
              "effective_exception_rate", jn (v.["exception_rate"] * (1 - v.["determination_reuse"]))
              "exception_minutes_per_grant", jn (v.["exception_rate"] * (1 - v.["determination_reuse"]) * lo * mult) ]

/// Deterministic 5-year ramp, accounts_y = target*(y/years)^1.5 [A].
let rampCapital (cfg: Json) (omKey: string) (targetAccounts: Num) (pricePerAccount: Num) (years: int) : (string * Json) list =
    let v = expectedValues cfg
    let om = cfg?operating_models?(omKey)
    let platform = truthy om?platform
    let step (cum: Num, peak: Num, rows: Json list, prev: Num) (y: int) =
        let a = Num.max (I 1000) (Num.toInt (targetAccounts * Num.pow (I y / years) (F 1.5)))
        let r = runDet cfg omKey a "normal" []
        let cost = r.Metric "operator_only_per_account" * a
        let cost = if not platform then cost + v.["cac"] * Num.max (a - prev) (v.["new_account_share"] * a) else cost
        let rev = pricePerAccount * a
        let cum = cum + (rev - cost)
        let peak = Num.max peak (-cum)
        cum, peak, rows @ [ jo [ "year", ji y; "accounts", jn a; "revenue", jn rev; "cost", jn cost; "cumulative", jn cum ] ], a
    let _, peak, rows, _ = List.fold step (-v.["build_capex"], v.["build_capex"], [], I 0) [ 1..years ]
    [ "om", js omKey; "target_accounts", jn targetAccounts; "peak_capital", jn peak; "years", jl rows ]

let blendedAssetRevenue (cfg: Json) (mixKey: string) (model: string) : Num =
    items cfg?mixes?(mixKey) |> Num.sumBy (fun (s, share) -> num share * revenuePerAccount cfg model s)

// ----------------------------------------------------------------------------
// Main
// ----------------------------------------------------------------------------

let HEAD_METRICS =
    [ "cost_per_account"; "operator_only_per_account"; "cost_per_account_incl_acq"; "baseline09_comparable_per_account"
      "account_level_per_account"; "platform_fixed_per_account"; "sponsor_level_per_sponsor"
      "fund_borne_per_account"; "cost_per_transaction"; "human_interventions_per_1000_accounts"
      "exception_rate_per_grant"; "peak_backlog_exceptions"; "p90_wait_days_exceptions"
      "sla_attainment_overall"; "sla_attainment_exceptions"; "recovery_days_max_queue"
      "financial_exposure_gross"; "financial_exposure_uncontained"; "control_effectiveness_proxy"
      "staff_fte_base"; "surge_fte_equivalent"; "utilisation_exceptions"; "customer_friction_per_1000"
      "unresolved_backlog_items_after_ext"; "unresolved_risk_usd" ]

let private pctJson (q: (string * Num) list) : Json = jo (q |> List.map (fun (p, x) -> p, jn x))
let private lookup (kvs: (string * 'a) list) (k: string) : 'a = kvs |> List.find (fst >> (=) k) |> snd

/// Outputs of main(): (results.json text, results.csv text, stdout text).
type Outputs = { ResultsJson: string; ResultsCsv: string; Stdout: string }

let results (cfg: Json) : Outputs =
    let scales = PyJson.list cfg?scales |> List.map num
    let nNorm, nScen = int (num cfg?meta?mc_runs_normal).V, int (num cfg?meta?mc_runs_scenarios).V
    let meta =
        jo
            [ "seed", cfg?meta?seed
              "command", js "python3 -I -B model.py"
              "mc_runs_normal", cfg?meta?mc_runs_normal
              "mc_runs_scenarios", cfg?meta?mc_runs_scenarios
              "disclaimer", js "THEORETICAL MODEL OUTPUTS from labelled assumptions; not observations." ]
    // 0. replication of the 09 baseline
    let replication =
        [ 1000; 10000; 100000; 1000000 ]
        |> List.map (fun a -> string a, detCost cfg "OM0_REPLICA" (I a) [] "baseline09_comparable_per_account")
    // 1. normal-scenario MC per operating model and scale
    let oms = [ "OM1"; "OM2"; "OM3"; "OM4"; "OM5" ]
    let sampledCells = [ "OM1", 100000.0; "OM3", 1000000.0; "OM4", 1000000.0 ]
    let normalCells =
        [ for om in oms do
              for a in scales do
                  let runs = mc cfg om a "normal" nNorm
                  let sampled =
                      if sampledCells |> List.contains (om, a.V) then
                          Some(runs |> List.map (fun r -> r.Values, r.Metric "cost_per_account"))
                      else None
                  yield $"{om}|{Num.str a}", summariseRuns runs, sampled ]
    let normal = normalCells |> List.map (fun (k, s, _) -> k, s)
    let sampled = normalCells |> List.choose (fun (k, _, s) -> s |> Option.map (fun s -> k, s))
    let normalMc = normal |> List.map (fun (k, s) -> k, HEAD_METRICS |> List.map (fun m -> m, lookup s m))
    // 2. scenarios: OM3 at every scale, OM1 at 100k
    let dkeys =
        [ "cost_per_account"; "sla_attainment_overall"; "financial_exposure_uncontained"
          "human_interventions_per_1000_accounts"; "customer_friction_per_1000" ]
    let cells = (scales |> List.map (fun s -> "OM3", s)) @ [ "OM1", I 100000 ]
    let scenOut, _ =
        keys cfg?scenarios
        |> List.mapFold
            (fun (baseRuns: Map<string * float, (string * Num) list list>) sk ->
                let outs, baseRuns =
                    cells
                    |> List.mapFold
                        (fun (baseRuns: Map<string * float, (string * Num) list list>) (om, a) ->
                            let runs = mc cfg om a sk nScen
                            let summ = summariseRuns runs
                            let key = $"{om}|{Num.str a}|{sk}"
                            let baseRuns = if sk = "normal" then baseRuns.Add((om, a.V), runs |> List.map _.Metrics) else baseRuns
                            let b = baseRuns.[(om, a.V)]
                            let delta =
                                dkeys
                                |> List.map (fun m ->
                                    let diffs = Seq.zip runs b |> Seq.map (fun (r, b) -> r.Metric m - metricOf b m) |> List.ofSeq
                                    m, [ "p10", pct diffs 0.1; "p50", pct diffs 0.5; "p90", pct diffs 0.9 ])
                            ((key, HEAD_METRICS |> List.map (fun m -> m, lookup summ m)), (key, delta)), baseRuns)
                        baseRuns
                outs, baseRuns)
            Map.empty
    let scenMc = scenOut |> List.concat |> List.map fst
    let deltas = scenOut |> List.concat |> List.map snd
    // 3. sensitivity
    let tornados =
        [ tornado cfg "OM1" (I 100000) "cost_per_account"
          tornado cfg "OM3" (I 1000000) "cost_per_account"
          tornado cfg "OM4" (I 1000000) "cost_per_account" ]
    let rankCorr =
        sampled
        |> List.map (fun (key, pairs) ->
            let names =
                items cfg?``params``
                |> List.filter (fun (_, s) -> (tryKey "mc" s |> Option.exists truthy) && (num s?high).V > (num s?low).V)
                |> List.map fst
            let ys = pairs |> List.map snd
            key,
            names
            |> List.map (fun n -> n, spearman (pairs |> List.map (fun (vals, _) -> vals.[n])) ys)
            |> sortByNum (fun (_, r) -> F(-(Math.Abs r)))
            |> List.map (fun (n, r) -> jo [ "param", js n; "spearman", jf r ]))
    // 4. targets
    let grid = [ 1000; 3000; 10000; 30000; 100000; 300000; 1000000; 2000000; 5000000; 10000000 ] |> List.map I
    let tgt =
        PyJson.list cfg?targets
        |> List.map num
        |> List.map (fun t ->
            let perOm =
                oms
                |> List.map (fun om ->
                    om,
                    jo
                        [ "min_scale_deterministic", jopt jn (minScaleFor cfg om t grid)
                          "p50_by_scale",
                          jo (scales |> List.map (fun a -> Num.str a, jn (lookup (lookup (lookup normal $"{om}|{Num.str a}") "cost_per_account") "p50")))
                          "p90_by_scale",
                          jo (scales |> List.map (fun a -> Num.str a, jn (lookup (lookup (lookup normal $"{om}|{Num.str a}") "cost_per_account") "p90"))) ])
            Num.str t,
            jo (
                perOm
                @ [ "OM3_1M_max_exception_minutes_per_grant", maxExceptionMinutesPerGrant cfg "OM3" (I 1000000) t
                    "OM4_1M_max_exception_minutes_per_grant", maxExceptionMinutesPerGrant cfg "OM4" (I 1000000) t
                    "break_even_balance_at_60bps", jn (t / 0.006) ]
            ))
    // 5. cost floor, segment economics, revenue, capital
    let marginal = oms |> List.map (fun om -> om, marginalFloor cfg om)
    let segEcon =
        [ segmentCostToServe cfg "OM1" (I 100000); segmentCostToServe cfg "OM3" (I 1000000); segmentCostToServe cfg "OM4" (I 1000000) ]
    let blended =
        [ "market"; "digital_minimal" ]
        |> List.map (fun mk -> mk, jo (keys cfg?revenue_models |> List.map (fun m -> m, jn (blendedAssetRevenue cfg mk m))))
    let mktRev = blendedAssetRevenue cfg "market" "asset_tiered_min100"
    let smallRev = Num.max (F 100.0) (tieredFee (I 100000) cfg?revenue_models?asset_tiered_min100?tiers_bps)
    let dmBal = items cfg?mixes?digital_minimal |> Num.sumBy (fun (s, sh) -> num sh * num cfg?segments?(s)?balance)
    let cap om a price basis = jo (rampCapital cfg om (I a) price 5 @ [ "price_basis", js basis ])
    let capital =
        [ cap "OM1" 100000 mktRev "market-mix blended Fidelity tiers ($100 min) [S tiers, A mix]"
          cap "OM1" 100000 smallRev "calibrated: blended tiers on a $100k-average book [A] (FSM:150 calibrated path)"
          cap "OM2" 100000 mktRev "market-mix blended Fidelity tiers [S tiers, A mix]"
          cap "OM3" 1000000 (F 100.0) "platform fee $100/account [A] (below FSM:140 $200-300)"
          cap "OM3" 1000000 (F 150.0) "platform fee $150/account [S FSM:140 low]"
          cap "OM4" 1000000 (F 12.0) "employer fee $12/account [A] only"
          cap "OM4" 1000000 (12.0 + 0.003 * dmBal) "employer $12 + 30 bps of balance [A]" ]
    // 6. worked examples (deterministic expected-value runs)
    let workedCells =
        [ "OM1", 100000, "normal"
          "OM3", 1000000, "normal"
          "OM4", 1000000, "normal"
          "OM3", 100000, "simultaneous_failures"
          "OM3", 100000, "incorrect_ai_outputs"
          "OM3", 10000, "december_peak" ]
    let worked =
        workedCells
        |> List.map (fun (om, a, sk) ->
            let r = runDet cfg om (I a) sk []
            $"{om}|{a}|{sk}",
            jo
                [ "metrics", jo (r.Metrics |> List.map (fun (k, x) -> k, jn x))
                  "base_fte", jo (r.BaseFte |> List.map (fun (q, x) -> q, jn x))
                  "lines", jl (r.Lines |> List.map (fun l -> lineJson { l with Usd = Num.round 2 l.Usd }))
                  "queues",
                  jo (
                      QUEUES
                      |> List.map (fun q ->
                          let s = r.Sims.[q]
                          q,
                          jo
                              [ "arrivals", jf s.Arrivals
                                "peak_backlog", jf s.PeakBacklog
                                "sla_attainment", jf s.SlaAttainment
                                "p90_wait_days", jf s.P90WaitDays
                                "contractor_fte_days", jf s.ContractorFteDays
                                "recovery_days", jn s.RecoveryDays
                                "recovered", JBool s.Recovered ])
                  )
                  "exposure", exposureJson r.Exposure ])
    let pctsJson (rows: (string * (string * (string * Num) list) list) list) =
        jo (rows |> List.map (fun (k, ms) -> k, jo (ms |> List.map (fun (m, q) -> m, pctJson q))))
    let resultsJson =
        jo
            [ "meta", meta
              "replication_09", jo (replication |> List.map (fun (k, x) -> k, jn x))
              "normal_mc", pctsJson normalMc
              "scenario_mc", pctsJson scenMc
              "scenario_paired_delta_vs_normal", pctsJson deltas
              "tornado", jl tornados
              "rank_correlation", jo (rankCorr |> List.map (fun (k, rows) -> k, jl rows))
              "targets", jo tgt
              "marginal_floor", jo marginal
              "segment_economics", jl segEcon
              "blended_revenue_per_account", jo blended
              "capital", jl capital
              "worked_examples", jo worked ]
    let r6 (x: Num) = Num.str (Num.round 6 x)
    let pctRows table (rows: (string * (string * (string * Num) list) list) list) =
        rows
        |> List.collect (fun (k, ms) ->
            let parts = k.Split('|') |> List.ofArray
            let om, a, sk =
                match parts with
                | [ om; a ] -> om, a, "normal"
                | [ om; a; sk ] -> om, a, sk
                | _ -> failwith "bad key"
            ms |> List.map (fun (m, q) -> [ table; om; a; sk; m; r6 (lookup q "p10"); r6 (lookup q "p50"); r6 (lookup q "p90") ]))
    let tornadoRows =
        tornados
        |> List.collect (fun t ->
            PyJson.list t?rows
            |> List.map (fun r ->
                [ "tornado"; text t?om; Num.str (num t?accounts); "normal"; text r?param
                  r6 (num r?metric_at_low); r6 (num t?``base``); r6 (num r?metric_at_high) ]))
    let csv =
        [ [ "table"; "operating_model"; "accounts"; "scenario"; "metric"; "p10"; "p50"; "p90" ] ]
        @ pctRows "normal_mc" normalMc
        @ pctRows "scenario_mc" scenMc
        @ pctRows "scenario_delta_vs_normal" deltas
        @ tornadoRows
        |> List.map (csvRow "\r\n")
        |> String.concat ""
    let stdout =
        let rep = replication |> List.map (fun (k, x) -> $"'{k}': {Num.str (Num.round 2 x)}") |> String.concat ", "
        let lines =
            normalMc
            |> List.map (fun (k, s) ->
                let q = lookup s "cost_per_account"
                let r2 p = Num.str (Num.round 2 (lookup q p))
                String.Join(" ", [ k; "cost/acct P10/P50/P90"; r2 "p10"; r2 "p50"; r2 "p90" ]))
        ($"replication_09 {{{rep}}}" :: lines |> List.map (fun l -> l + "\n") |> String.concat "")
    { ResultsJson = dumpSorted resultsJson; ResultsCsv = csv; Stdout = stdout }
