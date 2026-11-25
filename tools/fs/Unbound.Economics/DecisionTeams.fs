/// Owner decisions SPD-ADV-001 and SPD-ADV-004 applied to the executed model: port of
/// requirements/discovery/2026-11-24-simplification-economics/model/decision_teams.py.
///
/// SPD-ADV-001 (2026-11-24): no team may exercise legally reserved (A3+) authority for more than one sponsor; every
/// sponsor has its own decision team.
/// SPD-ADV-004 (2026-11-24): Unbound is a software platform only. Unbound builds and runs the software; the
/// organizations that use it (a sponsor directly, or a third-party administrator acting for several sponsors) employ
/// every person who does human work. Operator-supplied staff are a future option kept as a hook.
///
/// Re-runs the executed model (Model, unchanged) and re-assigns cost lines by who bears them. Pure.
/// THEORETICAL MODEL: the new assumptions are in DT_PARAMS and VENDOR_LINES below and are all [A].
module Unbound.Economics.DecisionTeams

open System
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.Economics.PyCompat
open Unbound.Economics.Model

// Queues whose decisions are reserved to the sponsor (A3+): exception grants and complex-gift acceptance.
// Conservative [A]: the whole queue is treated as reserved.
let RESERVED = [ "exceptions"; "complex" ]
let STAFFED = [ "exceptions"; "complex"; "service"; "ops" ]

let DT_PARAMS : Json =
    jo
        [ "team_floor_fte",
          jo
              [ "value", jf 2.5
                "low", jf 2.0
                "high", jf 3.5
                "label", js "A"
                "source",
                js "five named authority holders x 0.5 minimum time share (report 10, S1 floor); covers two-person approval and absence" ]
          "team_granularity_fte",
          jo
              [ "value", jf 0.5
                "label", js "A"
                "source", js "staff hired in half-FTE steps; a part-time person serves one sponsor only" ]
          "dedicated_mgmt_overhead",
          jo
              [ "value", jf 0.10
                "low", jf 0.05
                "high", jf 0.20
                "label", js "A"
                "source", js "future option only: operator supervision and appointment paperwork per dedicated team" ]
          "customer_loaded_cost_mult",
          jo
              [ "value", jf 1.0
                "low", jf 0.8
                "high", jf 1.3
                "label", js "A"
                "source", js "customer's loaded cost per FTE relative to model.py loaded_cost_per_fte" ]
          "tenant_authority_setup_cost",
          jo
              [ "value", ji 15000
                "low", ji 5000
                "high", ji 30000
                "label", js "A"
                "source",
                js "vendor-borne per sponsor tenant: authority-register set-up, training material, attestation evidence, audit support (USD/yr)" ] ]

// Lines Unbound bears as the software vendor [A]. Everything else that is operating cost falls on the customer.
let VENDOR_LINES =
    set
        [ "staff:leadership"; "staff:engineering"; "staff:agent_ownership"; "staff:security"
          "staff:philanthropic_intelligence"; "ai_inference"; "infrastructure_base"; "infrastructure_per_account"
          "data_providers"; "eligibility_screening"; "identity_new_accounts"; "insurance"
          "sponsor_level:platform_service"; "tenant:authority_setup" ]

// Active modes for release 1, plus the reference and the future hook.
let MODES = [ "pooled_v1_baseline"; "sponsor_direct"; "via_administrator"; "future_operator_dedicated" ]
let ACTIVE = [ "sponsor_direct"; "via_administrator" ]
let OMS = [ "OM3"; "OM4"; "OM5" ]
let SPONSOR_SIZES = [ 500; 1000; 2500; 5000; 10000; 25000; 100000 ]

type Dt = Map<string, Num>

let dtExpected () : Dt = items DT_PARAMS |> List.map (fun (k, s) -> k, num s?value) |> Map.ofList

let dtSample (rng: Rng) : Dt * Rng =
    let kvs, rng =
        items DT_PARAMS
        |> List.mapFold
            (fun rng (k, s) ->
                if (PyJson.field "low" s).IsSome then
                    let x, rng' = triangular (num s?low) (num s?high) (num s?value) rng
                    (k, x), rng'
                else (k, num s?value), rng)
            rng
    Map.ofList kvs, rng

let roundUp (x: Num) (step: Num) : Num = Num.ceil (x / step - 1e-9) * step

/// Unrounded FTE of human work in the given queues for the whole book.
let workloadFte (v: Values) (vol: Vol) (queues: string list) : Num =
    let handle = handleMinutes v vol
    let arrivals = queueAnnualArrivals vol
    queues |> Num.sumBy (fun q -> arrivals.[q] * handle.[q] / 60.0 / v.["productive_hours"] * v.["staffing_headroom"])

let teamFte (fte: Num) (dt: Dt) : Num = Num.max dt.["team_floor_fte"] (roundUp fte dt.["team_granularity_fte"])

let sponsorsOf (om: Json) (accounts: Num) (v: Values) : Num =
    if truthy om?platform then Num.max (I 1) (Num.ceil (accounts / v.["accounts_per_sponsor"])) else I 1

let bearerOf (lineName: string) = if VENDOR_LINES.Contains lineName then "vendor" else "customer"

let rebear (lines: Line list) : Line list =
    lines
    |> List.map (fun l -> if l.Bearer = "platform_operator" || l.Bearer = "sponsor" then { l with Bearer = bearerOf l.Line } else l)

/// Per-sponsor staffing lines that replace model.py's pooled queue staff: (lines, sponsors, FTE per sponsor, queues replaced).
let staffLines (om: Json) (accounts: Num) (v: Values) (vol: Vol) (dt: Dt) (mode: string) : Line list * Num * Num * string list =
    let n = sponsorsOf om accounts v
    let loaded = v.["loaded_cost_per_fte"] * dt.["customer_loaded_cost_mult"]
    let setup = line "tenant:authority_setup" (n * dt.["tenant_authority_setup_cost"]) "fixed" "vendor" "sponsor"
    if mode = "sponsor_direct" then
        // each sponsor employs everyone: decisions, service and operations, one combined floor
        let per = teamFte (workloadFte v vol STAFFED / n) dt
        [ line "sponsor_staff:all_queues" (n * per * loaded) "step_fixed" "customer" "sponsor"; setup ], n, per, STAFFED
    else
        let per = teamFte (workloadFte v vol RESERVED / n) dt
        if mode = "via_administrator" then
            // an administrator pools service and operations; decisions stay with each sponsor's own team
            [ line "sponsor_staff:decision_team" (n * per * loaded) "step_fixed" "customer" "sponsor"; setup ], n, per, RESERVED
        else
            let staff = n * per * v.["loaded_cost_per_fte"]
            [ line "operator_staff:decision_team" staff "step_fixed" "operator" "sponsor"
              line "operator_staff:management" (staff * dt.["dedicated_mgmt_overhead"]) "fixed" "operator" "sponsor" ],
            n,
            per,
            RESERVED

let restructure (run: Run) (om: Json) (accounts: Num) (dt: Dt) (mode: string) : Line list * Num * Num option =
    if mode = "pooled_v1_baseline" then run.Lines, sponsorsOf om accounts run.Values, None
    else
        let added, n, per, replaced = staffLines om accounts run.Values run.Volumes dt mode
        let dropped = replaced |> List.map (fun q -> "staff:queue:" + q) |> Set.ofList
        let kept = run.Lines |> List.filter (fun l -> not (dropped.Contains l.Line))
        rebear kept @ added, n, Some per

/// Same basis as model.py cost_per_account: operating cost excl. customer acquisition, incl. identity checks.
let opex (l: Line) = l.Type <> "acquisition" || l.Line = "identity_new_accounts"

let bearerSplit (lines: Line list) (acctYears: Num) : (string * Num) list =
    let perAcct (pred: Line -> bool) = (lines |> List.filter (fun l -> opex l && pred l) |> Num.sumBy _.Usd) / acctYears
    [ "cost_per_account",
      perAcct (fun l -> List.contains l.Bearer [ "sponsor"; "platform_operator"; "vendor"; "customer"; "operator" ])
      "vendor_per_account", perAcct (fun l -> l.Bearer = "vendor")
      "customer_per_account", perAcct (fun l -> l.Bearer = "customer")
      "operator_staff_per_account", perAcct (fun l -> l.Bearer = "operator") ]

/// One mode's figures: the bearer split plus sponsors and staff_fte_per_sponsor (None for the pooled baseline).
type ModeResult = { Split: (string * Num) list; Sponsors: Num; StaffFtePerSponsor: Num option }

let modeMetric (r: ModeResult) (metric: string) : Num option =
    match metric with
    | "sponsors" -> Some r.Sponsors
    | "staff_fte_per_sponsor" -> r.StaffFtePerSponsor
    | m -> Some(r.Split |> List.find (fst >> (=) m) |> snd)

let evaluate (cfg: Json) (omKey: string) (accounts: Num) (rng: Rng) (dtRng: Rng) (noise: bool) : (string * ModeResult) list =
    let om = cfg?operating_models?(omKey)
    let run, _ = runOnce cfg omKey accounts "normal" rng noise []
    let dt = if noise then fst (dtSample dtRng) else dtExpected ()
    let ay = run.Metric "accounts_year_avg"
    let one mode =
        let lines, n, per = restructure run om accounts dt mode
        { Split = bearerSplit lines ay; Sponsors = n; StaffFtePerSponsor = per }
    let out = MODES |> List.map (fun mode -> mode, one mode)
    // reproduction check: the pooled view must equal model.py's own figure
    let pooled = (modeMetric (snd out.Head) "cost_per_account").Value
    if not ((Num.abs (pooled - run.Metric "cost_per_account")).V < 1e-6) then
        raise (InvalidOperationException "AssertionError")
    out

let mc (cfg: Json) (omKey: string) (accounts: Num) (n: int) : (string * ModeResult) list list =
    let master =
        match cfg?meta?seed with
        | JInt i -> i
        | other -> failwith $"TypeError: seed must be an int, got {other}"
    parallelInit n (fun i ->
        evaluate cfg omKey accounts
            (seeded (seedFor master [ omKey; Num.str accounts; string i ]))
            (seeded (seedFor master [ "DT"; omKey; Num.str accounts; string i ]))
            true)

let summarise (runs: (string * ModeResult) list list) (mode: string) (metric: string) : (string * Num) list =
    let xs = runs |> List.choose (fun r -> modeMetric (r |> List.find (fst >> (=) mode) |> snd) metric)
    [ "p10", pct xs 0.1; "p50", pct xs 0.5; "p90", pct xs 0.9 ]

/// Deterministic: what ONE sponsor of a given size spends on its own people, by route. Workload per account comes
/// from the expected-value run at 1M accounts (per-account rates are scale-free in model.py).
let sponsorSizeTable (cfg: Json) (omKey: string) (dt: Dt) : Json =
    let run = runDet cfg omKey (I 1000000) "normal" []
    let v, vol, ay = run.Values, run.Volumes, run.Metric "accounts_year_avg"
    let allFte, resFte = workloadFte v vol STAFFED / ay, workloadFte v vol RESERVED / ay
    let loaded = v.["loaded_cost_per_fte"] * dt.["customer_loaded_cost_mult"]
    let row (size: int) =
        let size = I size
        let direct, decide = teamFte (allFte * size) dt, teamFte (resFte * size) dt
        jo
            [ "sponsor_accounts", jn size
              "sponsor_direct_fte", jn direct
              "sponsor_direct_usd", jn (direct * loaded)
              "sponsor_direct_per_account", jn (direct * loaded / size)
              "via_administrator_decision_fte", jn decide
              "via_administrator_decision_usd", jn (decide * loaded)
              "via_administrator_decision_per_account", jn (decide * loaded / size)
              "existing_staff_time_used_usd", jn (resFte * size * loaded)
              // future option (SPR-ADV-009): Unbound supplies a decision team dedicated to this sponsor
              "future_operator_dedicated_fte", jn decide
              "future_operator_dedicated_usd", jn (decide * v.["loaded_cost_per_fte"] * (1 + dt.["dedicated_mgmt_overhead"]))
              "future_operator_dedicated_per_account",
              jn (decide * v.["loaded_cost_per_fte"] * (1 + dt.["dedicated_mgmt_overhead"]) / size) ]
    jo
        [ "om", js omKey
          "all_work_fte_per_1000_accounts", jn (allFte * 1000)
          "reserved_fte_per_1000_accounts", jn (resFte * 1000)
          "floor_binds_below_accounts_direct", (if Num.truthy allFte then jn (dt.["team_floor_fte"] / allFte) else JNull)
          "floor_binds_below_accounts_decisions", (if Num.truthy resFte then jn (dt.["team_floor_fte"] / resFte) else JNull)
          "rows", jl (SPONSOR_SIZES |> List.map row) ]

/// Cost-recovery floors (not prices) for Unbound's software licence, deterministic.
type LicenceFloors =
    { Om: string
      Accounts: Num
      Sponsors: Num
      FutureC1: Num
      ALicence: Num
      BTenant: Num
      VendorCostTotal: Num }

let licenseFloors (cfg: Json) (omKey: string) (accounts: Num) (dt: Dt) : LicenceFloors =
    let run = runDet cfg omKey accounts "normal" []
    let om = cfg?operating_models?(omKey)
    let ay = run.Metric "accounts_year_avg"
    let lines, n, _ = restructure run om accounts dt "sponsor_direct"
    let vendor = lines |> List.filter (fun l -> l.Bearer = "vendor" && opex l)
    let perTenant = vendor |> List.filter (fun l -> l.Level = "sponsor")
    let perAccount = vendor |> List.filter (fun l -> l.Level <> "sponsor")
    let future, _, _ = restructure run om accounts dt "future_operator_dedicated"
    let dedicated = future |> List.filter (fun l -> l.Bearer = "operator")
    { Om = omKey
      Accounts = accounts
      Sponsors = n
      FutureC1 = (dedicated |> Num.sumBy _.Usd) / n
      ALicence = (perAccount |> Num.sumBy _.Usd) / ay
      BTenant = (perTenant |> Num.sumBy _.Usd) / n
      VendorCostTotal = vendor |> Num.sumBy _.Usd }

let licenceFloorsJson (f: LicenceFloors) : Json =
    jo
        [ "om", js f.Om
          "accounts", jn f.Accounts
          "sponsors", jn f.Sponsors
          "future_C1_dedicated_team_fee_per_sponsor_floor", jn f.FutureC1
          "A_licence_fee_per_account_floor", jn f.ALicence
          "B_tenant_fee_per_sponsor_floor", jn f.BTenant
          "vendor_cost_total_usd", jn f.VendorCostTotal
          "note", js "Floors recover modelled vendor cost only; margin, sales and customer acquisition excluded." ]

/// Outputs of main(): (decision-teams-results.json text, decision-teams-results.csv text, stdout text).
type Outputs = { ResultsJson: string; ResultsCsv: string; Stdout: string }

let results (cfg: Json) : Outputs =
    let nRuns = int (num cfg?meta?mc_runs_normal).V
    let metrics = [ "cost_per_account"; "vendor_per_account"; "customer_per_account"; "operator_staff_per_account" ]
    let scales = PyJson.list cfg?scales |> List.map num
    let mcOut =
        [ for om in OMS do
              for a in scales do
                  let runs = mc cfg om a nRuns
                  let modes = MODES |> List.map (fun mode -> mode, metrics |> List.map (fun m -> m, summarise runs mode m))
                  let sponsorsP50 = summarise runs "sponsor_direct" "sponsors" |> List.find (fst >> (=) "p50") |> snd
                  yield $"{om}|{Num.str a}", modes, sponsorsP50 ]
    let dt = dtExpected ()
    let grid = [ 1000; 3000; 10000; 30000; 100000; 300000; 1000000; 2000000; 5000000; 10000000 ] |> List.map I
    let det om a mode =
        let r = evaluate cfg om a zeroRng zeroRng false
        (modeMetric (r |> List.find (fst >> (=) mode) |> snd) "cost_per_account").Value
    let targets =
        PyJson.list cfg?targets
        |> List.map num
        |> List.map (fun t ->
            Num.str t,
            jo (
                OMS
                |> List.map (fun om ->
                    om, jo (MODES |> List.map (fun mode -> mode, jopt jn (grid |> List.tryFind (fun a -> (det om a mode).V <= t.V)))))
            ))
    let q3 (q: (string * Num) list) = jo (q |> List.map (fun (p, x) -> p, jn x))
    let resultsJson =
        jo
            [ "meta",
              jo
                  [ "decisions",
                    jo
                        [ "SPD-ADV-001", js "one decision team per sponsor, never shared"
                          "SPD-ADV-004",
                          js "Unbound is a software platform only; customers employ all staff; operator services are a future hook" ]
                    "command", js "python3 -I -B decision_teams.py"
                    "seed", cfg?meta?seed
                    "mc_runs", cfg?meta?mc_runs_normal
                    "base_model", js "model.py (unchanged)"
                    "dt_params", DT_PARAMS
                    "vendor_lines", jl (PyText.sortStrings VENDOR_LINES |> List.map js)
                    "reserved_queues", jl (RESERVED |> List.map js)
                    "active_modes", jl (ACTIVE |> List.map js)
                    "disclaimer", js "THEORETICAL MODEL OUTPUTS from labelled assumptions; not observations." ]
              "normal_mc",
              jo (
                  mcOut
                  |> List.map (fun (k, modes, sp) ->
                      k, jo ((modes |> List.map (fun (mode, ms) -> mode, jo (ms |> List.map (fun (m, q) -> m, q3 q)))) @ [ "sponsors_p50", jn sp ]))
              )
              "targets_min_scale_deterministic", jo targets
              "sponsor_size", jl (OMS |> List.map (fun om -> sponsorSizeTable cfg om dt))
              "licence_floors",
              jl (
                  [ "OM3", 100000; "OM3", 1000000; "OM4", 1000000; "OM5", 5000000 ]
                  |> List.map (fun (om, a) -> licenceFloorsJson (licenseFloors cfg om (I a) dt))
              ) ]
    let r6 (x: Num) = Num.str (Num.round 6 x)
    let csv =
        [ "operating_model"; "accounts"; "mode"; "metric"; "p10"; "p50"; "p90" ]
        :: [ for k, modes, _ in mcOut do
                 let om, a = match k.Split('|') with [| om; a |] -> om, a | _ -> failwith "bad key"
                 for mode, ms in modes do
                     for m, q in ms do
                         let g p = q |> List.find (fst >> (=) p) |> snd
                         yield [ om; a; mode; m; r6 (g "p10"); r6 (g "p50"); r6 (g "p90") ] ]
        |> List.map (csvRow "\r\n")
        |> String.concat ""
    let f2 (x: Num) = PyFloat.fixedPoint 2 x.V
    let p50 modes mode metric =
        modes |> List.find (fst >> (=) mode) |> snd |> List.find (fst >> (=) metric) |> snd |> List.find (fst >> (=) "p50") |> snd
    let lines =
        mcOut
        |> List.map (fun (k, modes, sp) ->
            let perMode =
                MODES
                |> List.map (fun mode -> let c = f2 (p50 modes mode "cost_per_account") in $"{mode} P50 {c}")
                |> String.concat " | "
            let vendor = f2 (p50 modes "sponsor_direct" "vendor_per_account")
            $"{k} {perMode} | vendor P50 {vendor} | sponsors {PyFloat.fixedPoint 0 sp.V}\n")
    let targetsLine = "targets " + PyJson.dumps (jo targets) + "\n"
    { ResultsJson = dumpSorted resultsJson
      ResultsCsv = csv
      Stdout = String.concat "" lines + targetsLine }
