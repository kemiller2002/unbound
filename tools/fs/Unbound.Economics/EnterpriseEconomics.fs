/// Phase 4: activity-based economics of five target DAF enterprises built on Unbound: port of
/// requirements/convergence/2026-11-24-master/model/enterprise_economics.py.
///
/// Uses the executed simplification model (Model) and the decision-team restructuring (DecisionTeams) UNCHANGED and adds:
///   * five target enterprises (E1-E5) with their own account mixes, scales, routes and revenue models;
///   * operating approaches per enterprise:
///         A traditional human-centred, self-built systems
///         AL A's workflows on licensed software (isolates workflow from technology sourcing)
///         B existing workflow enhanced with AI, self-built systems
///         C AI-native redesign on Unbound (per-sponsor decision teams, licence fee)
///         D C plus elimination of convention-only activities
///   * an activity map that assigns every cost line to a business activity and a bearer, so that transfers between
///     bearers are reported separately and never counted as savings;
///   * base / favourable / adverse ranges (Monte Carlo P50 / P10 / P90 of total economic cost);
///   * one-way sensitivity, stress scenarios and revenue / margin by enterprise.
///
/// THEORETICAL MODEL. Every number follows from labelled assumptions: parameters.json (labels S = sourced,
/// A = assumption, D = derived) and the [A] values in ENTERPRISES, APPROACHES and LICENCE below. Nothing is an
/// observation of a real organization. All functions are pure.
module Unbound.Economics.EnterpriseEconomics

open System
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.Economics.PyCompat
open Unbound.Economics.Model

module DT = Unbound.Economics.DecisionTeams

let SEED = 20261124
let MC_RUNS = 300

// ---------------------------------------------------------------------------- assumptions [A]
// (Literal tables transcribed mechanically from enterprise_economics.py; key order is the Python dict order.)

let MIXES : Json =
    jo
        [
          "community_foundation", jo
              [
                "inactive", jf 0.2
                "small_recurring", jf 0.08
                "typical", jf 0.32
                "highly_active", jf 0.05
                "hnw", jf 0.14
                "institutional", jf 0.03
                "advisor_managed", jf 0.18 ]
          "advisor_distributed", jo [ "advisor_managed", jf 0.62; "hnw", jf 0.18; "typical", jf 0.15; "inactive", jf 0.05 ] ]

let ENTERPRISES : Json =
    jo
        [
          "E1", jo
              [
                "name", js "Independent national DAF sponsor (direct to donor)"
                "mix", js "market"
                "route", js "sponsor_direct"
                "accounts", ji 50000
                "scales", jl [ ji 10000; ji 50000; ji 250000 ]
                "accounts_per_sponsor", JNull
                "revenue", js "asset_tiered_min100"
                "cac_bearer", js "sponsor"
                "complex_share_mult", jf 1.0
                "basis", js "daf-landscape business model D2C; market mix from parameters.json" ]
          "E2", jo
              [
                "name", js "Community foundation (DAF program inside a broader foundation)"
                "mix", js "community_foundation"
                "route", js "sponsor_direct"
                "accounts", ji 128
                "scales", jl [ ji 46; ji 128; ji 313; ji 1500 ]
                "accounts_per_sponsor", JNull
                "revenue", js "asset_tiered_min250"
                "cac_bearer", js "sponsor"
                "complex_share_mult", jf 1.5
                "basis", js "[S] DAFRC FY2024 panel, 802 community foundations: median 46, p75 128, p90 313 DAFs; only 7 hold >=1,500 (daf-landscape data/mission-2026-11-14/dafrc-sponsor-schedule-d-fy2019-2024.csv); non-DAF programs excluded" ]
          "E3", jo
              [
                "name", js "Wealth-management and advisor-distributed DAF provider"
                "mix", js "advisor_distributed"
                "route", js "sponsor_direct"
                "accounts", ji 40000
                "scales", jl [ ji 10000; ji 40000; ji 200000 ]
                "accounts_per_sponsor", JNull
                "revenue", js "asset_tiered_min250"
                "cac_bearer", js "sponsor"
                "complex_share_mult", jf 1.3
                "revenue_later", js "advisor_sponsored"
                "basis", js "daf-landscape advisor-distribution model; release 1 has no advisor-managed program and no fund-paid advisor compensation (CNV-GRT-009), so release-1 revenue is the sponsor admin fee only; advisor_sponsored is a later option" ]
          "E4", jo
              [
                "name", js "High-volume workplace giving provider"
                "mix", js "digital_minimal"
                "route", js "sponsor_direct"
                "accounts", ji 500000
                "scales", jl [ ji 100000; ji 500000; ji 2000000 ]
                "accounts_per_sponsor", JNull
                "revenue", js "employer_sponsored"
                "cac_bearer", js "employer"
                "complex_share_mult", jf 1.0
                "basis", js "daf-landscape high-volume-low-balance and embedded models; employer pays acquisition" ]
          "E5", jo
              [
                "name", js "Enterprise outsourced DAF administrator serving many sponsors"
                "mix", js "market"
                "route", js "via_administrator"
                "accounts", ji 250000
                "scales", jl [ ji 50000; ji 250000; ji 1000000 ]
                "accounts_per_sponsor", ji 5000
                "revenue", js "institutional_licensing"
                "cac_bearer", js "sponsor"
                "complex_share_mult", jf 1.0
                "basis", js "daf-landscape outsourced-admin/white-label; decisions stay with each sponsor (SPD-ADV-001)" ] ]

let LEVERS_C : Json =
    jo
        [
          "determination_reuse", jo [ "set", jf 0.5 ]
          "minutes_per_exception", jo [ "mult", jf 0.667 ]
          "contacts_mult", jo [ "mult", jf 0.6 ]
          "recon_break_rate", jo [ "mult", jf 0.3 ]
          "complex_hours", jo [ "set", jf 2.0 ]
          "seasonal_frac", jo [ "set", jf 0.5 ]
          "recurring_obligations_per_account", jo [ "mult", jf 0.5 ]
          "check_share_mult", jo [ "set", jf 0.2 ] ]

let D_EXTRA : Json =
    jo
        [
          "recurring_obligations_per_account", jo [ "mult", jf 0.25 ]
          "check_share_mult", jo [ "set", jf 0.05 ]
          "contacts_mult", jo [ "mult", jf 0.45 ]
          "determination_reuse", jo [ "set", jf 0.6 ] ]

let APPROACHES : Json =
    jo
        [
          "A", jo
              [
                "name", js "Traditional human-centred operation"
                "platform", JBool false
                "outsource_complex", JBool false
                "overrides", jo
                    [
                      "exception_rate", jo [ "set", jf 0.3 ]
                      "minutes_per_exception", jo [ "mult", jf 1.0 ]
                      "contacts_mult", jo [ "mult", jf 1.3 ]
                      "ai_cost_per_dossier", jo [ "set", jf 0.0 ]
                      "ai_interactions_per_account", jo [ "set", jf 0.0 ]
                      "ai_cost_per_grant_screen", jo [ "set", jf 0.0 ]
                      "recurring_obligations_per_account", jo [ "mult", jf 1.5 ]
                      "check_share_mult", jo [ "set", jf 1.5 ] ]
                "basis", js "[A] per-item human review: 30% of grants touched by a person; no AI; more paper and contacts" ]
          "AL", jo
              [
                "name", js "Traditional human-centred operation on licensed software"
                "platform", JBool true
                "outsource_complex", JBool false
                "overrides", jo
                    [
                      "exception_rate", jo [ "set", jf 0.3 ]
                      "contacts_mult", jo [ "mult", jf 1.3 ]
                      "ai_cost_per_dossier", jo [ "set", jf 0.0 ]
                      "ai_interactions_per_account", jo [ "set", jf 0.0 ]
                      "ai_cost_per_grant_screen", jo [ "set", jf 0.0 ]
                      "recurring_obligations_per_account", jo [ "mult", jf 1.5 ]
                      "check_share_mult", jo [ "set", jf 1.5 ] ]
                "basis", js "[A] A's human workflows, but technology bought at the same licence as C; isolates the workflow effect from the build-versus-buy effect" ]
          "A8", jo
              [
                "name", js "Traditional operation, same exception definition as B-D (8%)"
                "platform", JBool false
                "outsource_complex", JBool false
                "overrides", jo
                    [
                      "contacts_mult", jo [ "mult", jf 1.3 ]
                      "ai_cost_per_dossier", jo [ "set", jf 0.0 ]
                      "ai_interactions_per_account", jo [ "set", jf 0.0 ]
                      "ai_cost_per_grant_screen", jo [ "set", jf 0.0 ]
                      "recurring_obligations_per_account", jo [ "mult", jf 1.5 ]
                      "check_share_mult", jo [ "set", jf 1.5 ] ]
                "basis", js "Adversarial correction ADV-B-005: A with the same exception incidence as the other approaches; the 30% per-item-review assumption in A is a separate, unmeasured lever" ]
          "C0", jo
              [
                "name", js "Unbound without unmeasured redesign levers"
                "platform", JBool true
                "outsource_complex", JBool false
                "overrides", jo []
                "basis", js "Adversarial correction ADV-B-006: C's platform, licence and per-sponsor teams with B's workflow parameters (no determination reuse, no minute/contact/reconciliation/complex-hour reductions)" ]
          "B", jo
              [
                "name", js "Existing workflow enhanced with AI"
                "platform", JBool false
                "outsource_complex", JBool false
                "overrides", jo []
                "basis", js "OM1 levers (parameters.json): exception-based review, AI assistance, same processes" ]
          "C", jo
              [
                "name", js "AI-native redesign on Unbound"
                "platform", JBool true
                "outsource_complex", JBool true
                "overrides", LEVERS_C
                "basis", js "OM2/OM3 levers; Unbound licence; per-sponsor decision teams (SPD-ADV-001, SPD-ADV-004)" ]
          "D", jo
              [
                "name", js "AI-native redesign plus elimination of convention-only activities"
                "platform", JBool true
                "outsource_complex", JBool true
                "overrides", (merge (items D_EXTRA) LEVERS_C)
                "basis", js "[A] removes convention-only recurring obligations, default-ACH, proactive notices; legal obligations retained" ] ]

let LICENCE : Json =
    jo
        [
          "platform_accounts", jo
              [
                "favourable", ji 250000
                "base", ji 100000
                "adverse", ji 25000
                "market_exceeding_reference", ji 1000000 ]
          "markup", jo [ "value", jf 0.3; "low", jf 0.1; "high", jf 0.6 ]
          "basis", js "[A] Unbound's vendor cost per account (decision_teams.license_floors) at the stated whole-platform scale, plus markup. Scales corrected after adversarial review ADV-B-001: the US market holds at most ~737k traditional invested DAF accounts and only 20 sponsors hold >=5,000 (DAFRC FY2024); 1M is kept only as a labelled reference." ]

let VENDOR : Json =
    jo
        [
          "build_capex_amortisation_years", ji 5
          "sales_cost_per_new_sponsor", ji 40000
          "new_sponsor_share", jf 0.2
          "basis", js "[A] build_capex from parameters.json amortised over 5 years; sales and implementation per new sponsor; 20% of sponsors new each year. Sponsor onboarding cost is already in the vendor floor (sponsor_level:platform_service)." ]

let ACTIVITY_JSON : Json =
    jo
        [
          "customer_acquisition", js "acquisition"
          "identity_new_accounts", js "onboarding_identity"
          "payments:contributions", js "contributions"
          "staff:queue:complex", js "contributions_complex_assets"
          "complex_asset_vendor", js "contributions_complex_assets"
          "card_fees", js "contributions"
          "custody", js "investment_administration"
          "pool_investment_expense", js "investment_management"
          "eligibility_screening", js "charity_vetting_and_grant_decisions"
          "staff:queue:exceptions", js "charity_vetting_and_grant_decisions"
          "data_providers", js "charity_vetting_and_grant_decisions"
          "staff:philanthropic_intelligence", js "charity_vetting_and_grant_decisions"
          "payments:grants", js "payment_processing"
          "staff:queue:service", js "customer_service"
          "staff:queue:ops", js "reconciliation_accounting_exceptions"
          "recurring_obligations", js "statements_and_recurring_obligations"
          "external_audit", js "compliance_and_assurance"
          "legal_filings", js "compliance_and_assurance"
          "insurance", js "compliance_and_assurance"
          "infrastructure_base", js "technology"
          "infrastructure_per_account", js "technology"
          "staff:engineering", js "technology"
          "staff:security", js "technology"
          "staff:agent_ownership", js "technology"
          "unbound_licence", js "technology"
          "ai_inference", js "ai_usage"
          "staff:leadership", js "management_overhead"
          "staff:partnerships", js "management_overhead"
          "sponsor_level:entity_costs", js "management_overhead"
          "advisor_relationships", js "advisor_service"
          "sponsor_level:platform_service", js "sponsor_onboarding_and_exit"
          "tenant:authority_setup", js "sponsor_onboarding_and_exit" ]

let QUEUE_ACTIVITY_JSON : Json =
    jo
        [
          "exceptions", js "charity_vetting_and_grant_decisions"
          "complex", js "contributions_complex_assets"
          "service", js "customer_service"
          "ops", js "reconciliation_accounting_exceptions" ]

let COMPLEX_POLICY : Json =
    jo
        [
          "complex_hours", jf 1.0
          "share_factor", jf 0.25
          "basis", js "OD-M-09 (owner decision): complex assets are rare and are sold on receipt by a contracted intermediary, so internal handling is intake checks and proceeds reconciliation (1 hour [A]) and the complex share is a quarter of the earlier mix assumption [A]. Applied to every approach, so it is a business policy, not an Unbound-only lever; the intermediary fee stays fund-borne." ]

let STAFFED_JSON : Json =
    jo
        [
          "team_floor_fte", jf 4.0
          "supervision_share", jf 0.15
          "prep_share", jf 0.5
          "ren_price_per_daf", jl [ jf 413.0; jf 745.0 ]
          "ipHi_software_per_daf", jl [ jf 32.0; jf 129.0 ]
          "basis", js "[A] floor of 4 FTE for coverage and segregation of duties; 15% supervision overhead; Unbound staff prepare half of exception and complex-gift work while sponsors' own appointed people decide (SPD-ADV-001). [market-data] Ren administration $413-745 per DAF-year and iPhi software $32-129 per DAF-year from Form 990 contractor payments (report 13; floors, may bundle services not modelled such as gift consulting)." ]

let PLATFORM_SCALES = [ 25000; 50000; 100000; 250000; 500000; 1000000 ]

let private ACTIVITY = items ACTIVITY_JSON |> List.map (fun (k, v) -> k, text v) |> Map.ofList
let private QUEUE_ACTIVITY = items QUEUE_ACTIVITY_JSON |> List.map (fun (k, v) -> k, text v) |> Map.ofList

let activityOf (line: string) : string =
    if line.StartsWith "staff:surge:" then QUEUE_ACTIVITY.[line.Split(':').[2]]
    else ACTIVITY |> Map.tryFind line |> Option.defaultValue "other"

let private ACTIVITY_NAMES = PyText.sortStrings (Set.ofSeq (Map.values ACTIVITY) |> Set.add "other")

// ---------------------------------------------------------------------------- configuration

/// parameters.json plus the enterprise x approach operating models (pure).
let config (baseCfg: Json) : Json =
    let oms =
        [ for e, ent in items ENTERPRISES do
              for a, appr in items APPROACHES do
                  let overrides =
                      appr?overrides
                      |> withKey "complex_share_mult" (jo [ "set", jn (num ent?complex_share_mult * num COMPLEX_POLICY?share_factor) ])
                      |> withKey "complex_hours" (jo [ "set", COMPLEX_POLICY?complex_hours ])
                      |> (if truthy ent?accounts_per_sponsor then withKey "accounts_per_sponsor" (jo [ "set", ent?accounts_per_sponsor ]) else id)
                  yield
                      $"{e}_{a}",
                      jo
                          [ "name", js (text ent?name + " / " + text appr?name)
                            "mix", ent?mix
                            "platform", appr?platform
                            "cac_bearer", (if text ent?cac_bearer <> "employer" then ent?cac_bearer else js "employer")
                            "contractors", JBool true
                            "outsource_complex", JBool true
                            "overrides", overrides ] ]
    baseCfg
    |> withKey "mixes" (merge (items MIXES) baseCfg?mixes)
    |> withKey "operating_models" (merge oms baseCfg?operating_models)

let sponsorsFor (e: string) (accounts: Num) : Num =
    let ent = ENTERPRISES?(e)
    if truthy ent?accounts_per_sponsor then Num.max (I 1) (Num.ceil (accounts / num ent?accounts_per_sponsor)) else I 1

/// Vendor cost per account at whole-platform scale (OM3 licence floor) plus markup (the noise=False path, the only one
/// enterprise_economics.py calls).
let licencePerAccount (cfg: Json) (which: string) (markup: Num option) : Num =
    let dt = DT.dtExpected ()
    let scale = num LICENCE?platform_accounts?(which)
    let floors = DT.licenseFloors cfg "OM3" scale dt
    let perTenantAmortised = floors.BTenant / 5000.0
    let m = markup |> Option.defaultValue (num LICENCE?markup?value)
    (floors.ALicence + perTenantAmortised) * (1 + m)

// ---------------------------------------------------------------------------- one evaluation

let private isPlatform (approach: string) = truthy APPROACHES?(approach)?platform

/// Lines as borne by the enterprise. A/B: own systems, own staff (model lines as is).
/// C/D: per-sponsor decision teams; vendor lines replaced by a licence charge.
let restructureFor (run: Run) (om: Json) (accounts: Num) (e: string) (approach: string) (dt: DT.Dt) : Line list =
    if not (isPlatform approach) then
        run.Lines
        |> List.map (fun l -> if l.Bearer = "sponsor" || l.Bearer = "platform_operator" then { l with Bearer = "enterprise" } else l)
    else
        let n = sponsorsFor e accounts
        let v = run.Values.Add("accounts_per_sponsor", accounts / n)
        let added, _, _, replaced = DT.staffLines om accounts v run.Volumes dt (text ENTERPRISES?(e)?route)
        let dropped = replaced |> List.map (fun q -> "staff:queue:" + q) |> Set.ofList
        let kept = DT.rebear run.Lines |> List.filter (fun l -> not (dropped.Contains l.Line) && l.Bearer <> "vendor")
        kept @ added |> List.map (fun l -> if l.Bearer = "customer" then { l with Bearer = "enterprise" } else l)

/// Allocate combined sponsor teams to activities pro rata to modelled queue workload.
let splitDirectTeam (lines: Line list) (run: Run) : Line list =
    let fte = run.BaseFte
    let orOne (x: Num) = if Num.truthy x then x else F 1.0
    let tot = orOne (fte |> Num.sumBy snd)
    let explode (l: Line) =
        match l.Line with
        | "sponsor_staff:all_queues" -> fte |> List.map (fun (q, f) -> { l with Line = $"staff:queue:{q}"; Usd = l.Usd * f / tot })
        | "sponsor_staff:decision_team" ->
            let res = DT.RESERVED |> List.map (fun q -> q, fte |> List.find (fst >> (=) q) |> snd)
            let rt = orOne (res |> Num.sumBy snd)
            res |> List.map (fun (q, f) -> { l with Line = $"staff:queue:{q}"; Usd = l.Usd * f / rt })
        | _ -> [ l ]
    lines |> List.collect explode

type Ev =
    { Lines: Line list
      AccountsYear: Num
      Run: Run }

type Opts =
    { Scen: string
      Licence: string
      Markup: Num option
      Override: (string * Num) list }

let defaults = { Scen = "normal"; Licence = "base"; Markup = None; Override = [] }

let evaluate (cfg: Json) (e: string) (approach: string) (accounts: Num) (rng: Rng) (dtRng: Rng) (noise: bool) (o: Opts) : Ev =
    let omKey = $"{e}_{approach}"
    let om = cfg?operating_models?(omKey)
    let run, _ = runOnce cfg omKey accounts o.Scen rng noise o.Override
    let dt = if noise then fst (DT.dtSample dtRng) else DT.dtExpected ()
    let lines = splitDirectTeam (restructureFor run om accounts e approach dt) run
    let ay = run.Metric "accounts_year_avg"
    let lic = if isPlatform approach then [ licencePerAccount cfg o.Licence o.Markup * ay ] else []
    { Lines = lines @ (lic |> List.map (fun u -> line "unbound_licence" u "variable" "enterprise" "account"))
      AccountsYear = ay
      Run = run }

type Summary =
    { EnterpriseAdmin: Num
      FundAdmin: Num
      Total: Num
      Acquisition: Num
      InvestmentManagement: Num
      HumanPer1000: Num
      StaffFteQueueEquivalent: Num
      Activities: (string * Num) list
      Bearers: (string * Num) list
      Fixed: Num
      Variable: Num }

let summarise (ev: Ev) : Summary =
    let ay = ev.AccountsYear
    let by (pred: Line -> bool) = (ev.Lines |> List.filter pred |> Num.sumBy _.Usd) / ay
    let acq (l: Line) = l.Line = "customer_acquisition"
    let enterpriseAdmin = by (fun l -> l.Bearer = "enterprise" && not (acq l))
    let fundAdmin = by (fun l -> (l.Bearer = "fund" || l.Bearer = "donor_or_fund") && l.Line <> "pool_investment_expense")
    { EnterpriseAdmin = enterpriseAdmin
      FundAdmin = fundAdmin
      Total = enterpriseAdmin + fundAdmin
      Acquisition = by acq
      InvestmentManagement = by (fun l -> l.Line = "pool_investment_expense")
      HumanPer1000 = ev.Run.Metric "human_interventions_per_1000_accounts"
      StaffFteQueueEquivalent =
        (ev.Lines |> List.filter (fun l -> l.Line.StartsWith "staff:queue") |> Num.sumBy _.Usd)
        / ev.Run.Values.["loaded_cost_per_fte"]
      Activities =
        ACTIVITY_NAMES
        |> List.map (fun a -> a, by (fun l -> activityOf l.Line = a && l.Line <> "pool_investment_expense"))
      Bearers =
        PyText.sortStrings (ev.Lines |> List.map _.Bearer |> Set.ofList)
        |> List.map (fun b -> b, by (fun l -> l.Bearer = b))
      Fixed = by (fun l -> (l.Type = "fixed" || l.Type = "step_fixed") && l.Bearer = "enterprise")
      Variable = by (fun l -> l.Type.StartsWith "variable" && l.Bearer = "enterprise") }

let summaryJson (s: Summary) : (string * Json) list =
    [ "enterprise_admin_per_account", jn s.EnterpriseAdmin
      "fund_borne_admin_per_account", jn s.FundAdmin
      "total_economic_admin_per_account", jn s.Total
      "acquisition_per_account", jn s.Acquisition
      "investment_management_per_account", jn s.InvestmentManagement
      "human_interventions_per_1000", jn s.HumanPer1000
      "staff_fte_queue_equivalent", jn s.StaffFteQueueEquivalent
      "activities", jo (s.Activities |> List.map (fun (a, x) -> a, jn x))
      "bearers", jo (s.Bearers |> List.map (fun (b, x) -> b, jn x))
      "fixed_per_account", jn s.Fixed
      "variable_per_account", jn s.Variable ]

let seeds (parts: string list) : Numerics.BigInteger =
    Numerics.BigInteger(Crc32.ofString (String.Join("|", string SEED :: parts)))

let mc (cfg: Json) (e: string) (approach: string) (accounts: Num) (n: int) : Json =
    let runs =
        parallelInit n (fun i ->
            let key = [ e; approach; Num.str accounts; string i ]
            summarise (evaluate cfg e approach accounts (seeded (seeds key)) (seeded (seeds ("dt" :: key))) true defaults))
    let xs = runs |> List.map _.Total |> sortByNum id
    jo
        [ "favourable_p10", jn (pct xs 0.1)
          "base_p50", jn (pct xs 0.5)
          "adverse_p90", jn (pct xs 0.9)
          "enterprise_admin_p50", jn (pct (runs |> List.map _.EnterpriseAdmin |> sortByNum id) 0.5) ]

let det (cfg: Json) (e: string) (approach: string) (accounts: Num) (o: Opts) : Summary =
    summarise (evaluate cfg e approach accounts zeroRng zeroRng false o)

// ---------------------------------------------------------------------------- revenue

let blendedRevenue (cfg: Json) (e: string) : Num =
    let ent = ENTERPRISES?(e)
    items cfg?mixes?(text ent?mix) |> Num.sumBy (fun (s, share) -> num share * revenuePerAccount cfg (text ent?revenue) s)

/// Asset-based revenue scales with balances; minimums and per-account fees do not.
let revenueUnderMarket (cfg: Json) (e: string) (aumMult: Num) : Num =
    let ent = ENTERPRISES?(e)
    let mix = cfg?mixes?(text ent?mix)
    let segs = keys mix |> List.map (fun s -> s, cfg?segments?(s) |> withKey "balance" (jn (num cfg?segments?(s)?balance * aumMult)))
    let c2 = cfg |> withKey "segments" (merge segs cfg?segments)
    items mix |> Num.sumBy (fun (s, share) -> num share * revenuePerAccount c2 (text ent?revenue) s)

// ---------------------------------------------------------------------------- sensitivity

let SENS_PARAMS =
    [ "ai_cost_per_dossier"; "ai_cost_per_interaction"; "exception_rate"; "minutes_per_exception"
      "ach_grant_cost"; "routine_eligibility_per_grant"; "cac"; "loaded_cost_per_fte"
      "contacts_mult"; "recon_break_rate"; "infra_per_account"; "complex_hours" ]

let sensitivity (cfg: Json) (e: string) (accounts: Num) : Json list =
    let p = cfg?``params``
    let baseC = (det cfg e "C" accounts defaults).Total
    let one (k: string) =
        let lo = (det cfg e "C" accounts { defaults with Override = [ k, num p?(k)?low ] }).Total
        let hi = (det cfg e "C" accounts { defaults with Override = [ k, num p?(k)?high ] }).Total
        let swing = Num.abs (hi - lo)
        swing,
        jo
            [ "enterprise", js e
              "parameter", js k
              "low_value", p?(k)?low
              "high_value", p?(k)?high
              "label", p?(k)?label
              "cost_at_low", jn lo
              "cost_at_high", jn hi
              "base", jn baseC
              "swing", jn swing ]
    let rows = SENS_PARAMS |> List.map one |> sortByNum (fun (sw, _) -> -sw) |> List.map snd
    let licence =
        [ "favourable"; "adverse" ]
        |> List.map (fun w ->
            jo
                [ "enterprise", js e
                  "parameter", js $"licence:{w}"
                  "low_value", js ""
                  "high_value", js ""
                  "label", js "A"
                  "cost_at_low", jn (det cfg e "C" accounts { defaults with Licence = w }).Total
                  "cost_at_high", jn (det cfg e "C" accounts { defaults with Licence = w; Markup = Some(num LICENCE?markup?high) }).Total
                  "base", jn baseC
                  "swing", jf 0.0 ])
    rows @ licence

let STRESS =
    [ "december_peak"; "market_downturn"; "payment_failures"; "fraud_attempts"; "staff_absence"; "vendor_outage"
      "ai_outage"; "incorrect_ai_outputs"; "regulatory_change"; "sponsor_termination"; "simultaneous_failures" ]

let stress (cfg: Json) (e: string) (accounts: Num) : Json list =
    let baseC = det cfg e "C" accounts defaults
    STRESS
    |> List.map (fun s ->
        let ev = evaluate cfg e "C" accounts zeroRng zeroRng false { defaults with Scen = s }
        let sm = summarise ev
        jo
            [ "enterprise", js e
              "scenario", js s
              "total_economic_admin_per_account", jn sm.Total
              "delta_vs_normal", jn (sm.Total - baseC.Total)
              "sla_attainment_exceptions", jn (ev.Run.Metric "sla_attainment_exceptions")
              "p90_wait_days_exceptions", jn (ev.Run.Metric "p90_wait_days_exceptions")
              "uncontained_exposure_usd", jn (ev.Run.Metric "financial_exposure_uncontained")
              "recovery_days_max_queue", jn (ev.Run.Metric "recovery_days_max_queue") ])

// ---------------------------------------------------------------------------- December surge (TEV-001, ADV-R2-021)

let SURGE_LEVELS = [ 0.5; 1.0; 2.0; 3.0 ]

/// Cost and exception-SLA attainment of approach C in the december_peak scenario at several seasonal staffing levels
/// (seasonal_frac = extra December staff as a multiple of base queue staff). Note: the queue simulation is the base
/// model's pooled queue, so per-sponsor team rounding is not reflected in waiting times.
let decemberSurge (cfg: Json) (e: string) : Json list =
    let n = num ENTERPRISES?(e)?accounts
    let baseC = (det cfg e "C" n defaults).Total
    SURGE_LEVELS
    |> List.map (fun sf ->
        let omC = cfg?operating_models?($"{e}_C")
        let om = omC |> withKey "overrides" (omC?overrides |> withKey "seasonal_frac" (jo [ "set", jf sf ]))
        let c2 = cfg |> withKey "operating_models" (cfg?operating_models |> withKey $"{e}_C" om)
        let ev = evaluate c2 e "C" n zeroRng zeroRng false { defaults with Scen = "december_peak" }
        jo
            [ "enterprise", js e
              "seasonal_frac", jf sf
              "total_economic_admin_per_account", jn (summarise ev).Total
              "delta_vs_normal_C", jn ((summarise ev).Total - baseC)
              "sla_attainment_exceptions", jn (ev.Run.Metric "sla_attainment_exceptions")
              "p90_wait_days_exceptions", jn (ev.Run.Metric "p90_wait_days_exceptions") ])

// ---------------------------------------------------------------------------- staffed administration offer (OD-M-08)

let ADMIN_LINES =
    [ "staff:queue:service"; "staff:queue:ops"; "staff:surge:service"; "staff:surge:ops"; "payments:grants"
      "payments:contributions"; "recurring_obligations"; "advisor_relationships"; "identity_new_accounts" ]

/// Unbound's cost per fund to run Ren-class outsourced administration for sponsors of 5,000 funds each (E5 mix),
/// with Unbound's own platform at cost (no licence markup).
let staffedOffer (cfg: Json) (adminFunds: Num) (licenceWhich: string) : Json =
    let ev =
        evaluate cfg "E5" "C" adminFunds zeroRng zeroRng false { defaults with Licence = licenceWhich; Markup = Some(F 0.0) }
    let ay, v = ev.AccountsYear, ev.Run.Values
    let usd (pred: Line -> bool) = ev.Lines |> List.filter pred |> Num.sumBy _.Usd
    let admin4 = List.take 4 ADMIN_LINES
    let adminOps = usd (fun l -> List.contains l.Line ADMIN_LINES || l.Line = "unbound_licence")
    let reserved =
        usd (fun l ->
            List.contains l.Line [ "staff:queue:exceptions"; "staff:queue:complex"; "staff:surge:exceptions"; "staff:surge:complex" ])
    let prep = reserved * num STAFFED_JSON?prep_share
    let staffUsd = usd (fun l -> List.contains l.Line admin4) + prep
    let floorUsd = num STAFFED_JSON?team_floor_fte * v.["loaded_cost_per_fte"]
    let staffUsdFloored = Num.max staffUsd floorUsd
    let cost =
        adminOps - usd (fun l -> List.contains l.Line admin4) + staffUsdFloored * (1 + num STAFFED_JSON?supervision_share)
        + prep * 0
    let renLow = num (PyJson.list STAFFED_JSON?ren_price_per_daf).Head
    jo
        [ "admin_funds", jn adminFunds
          "licence_scale", js licenceWhich
          "unbound_admin_cost_per_fund", jn (cost / ay)
          "sponsor_retained_decision_cost_per_fund", jn ((reserved - prep) / ay)
          "team_floor_binding", JBool(staffUsd.V < floorUsd.V)
          "margin_vs_ren_low", jn (renLow - cost / ay) ]

let staffedTable (cfg: Json) : Json list =
    [ for n in [ 10000; 25000; 50000; 100000 ] do
          for w in [ "adverse"; "base" ] do
              yield staffedOffer cfg (I n) w ]

// ---------------------------------------------------------------------------- vendor P&L (ADV-B-002)

/// Unbound's own annual P&L at a platform scale and a licence price (deterministic).
let vendorPnl (cfg: Json) (platformFunds: Num) (pricePerFund: Num) : Json =
    let floors = DT.licenseFloors cfg "OM3" platformFunds (DT.dtExpected ())
    let sponsors = floors.Sponsors
    let runCost = floors.VendorCostTotal
    let capex = num cfg?``params``?build_capex?value / num VENDOR?build_capex_amortisation_years
    let sales = sponsors * num VENDOR?new_sponsor_share * num VENDOR?sales_cost_per_new_sponsor
    let cost = runCost + capex + sales
    let revenue = platformFunds * pricePerFund
    jo
        [ "platform_funds", jn platformFunds
          "sponsors", jn sponsors
          "price_per_fund", jn pricePerFund
          "revenue", jn revenue
          "run_cost", jn runCost
          "capex_amortisation", jn capex
          "sales_and_implementation", jn sales
          "total_cost", jn cost
          "operating_result", jn (revenue - cost)
          "break_even_price_per_fund", jn (cost / platformFunds) ]

let vendorTable (cfg: Json) : Json list =
    [ for n in PLATFORM_SCALES do
          for p in [ 28.0; 50.0; 100.0 ] do
              yield vendorPnl cfg (I n) (F p) ]

// ---------------------------------------------------------------------------- orchestration

let results (baseCfg: Json) : Json =
    let cfg = config baseCfg
    let ents = keys ENTERPRISES
    let approaches = keys APPROACHES
    let accountsOf e = num ENTERPRISES?(e)?accounts
    let total (s: Summary) = s.Total
    let comparisonSummaries =
        ents |> List.map (fun e -> e, approaches |> List.map (fun a -> a, det cfg e a (accountsOf e) defaults))
    let comparison =
        comparisonSummaries
        |> List.map (fun (e, byA) ->
            e, jo (byA |> List.map (fun (a, s) -> a, jo (summaryJson s @ [ "range", mc cfg e a (accountsOf e) MC_RUNS ]))))
    let scales =
        ents
        |> List.map (fun e ->
            e,
            jo (
                PyJson.list ENTERPRISES?(e)?scales
                |> List.map num
                |> List.map (fun n -> Num.str n, jo (approaches |> List.map (fun a -> a, jn (total (det cfg e a n defaults)))))
            ))
    let enterpriseAdminC e =
        (comparisonSummaries |> List.find (fst >> (=) e) |> snd |> List.find (fst >> (=) "C") |> snd).EnterpriseAdmin
    let revenue =
        ents
        |> List.map (fun e ->
            e,
            jo
                [ "model", ENTERPRISES?(e)?revenue
                  "blended_revenue_per_account", jn (blendedRevenue cfg e)
                  "revenue_after_30pct_market_decline", jn (revenueUnderMarket cfg e (F 0.7))
                  "margin_C_per_account", jn (blendedRevenue cfg e - enterpriseAdminC e)
                  "margin_C_after_decline", jn (revenueUnderMarket cfg e (F 0.7) - enterpriseAdminC e) ])
    let segment = ents |> List.map (fun e -> e, segmentCostToServe cfg $"{e}_C" (accountsOf e))
    let atLicence e a ws =
        jo (ws |> List.map (fun w -> w, jn (total (det cfg e a (accountsOf e) { defaults with Licence = w }))))
    let corrected =
        ents
        |> List.map (fun e ->
            e,
            jo
                [ "C_at_licence", atLicence e "C" [ "favourable"; "base"; "adverse"; "market_exceeding_reference" ]
                  "C0_at_licence", atLicence e "C0" [ "favourable"; "base"; "adverse" ] ])
    jo
        [ "meta",
          jo
              [ "seed", ji SEED
                "mc_runs", ji MC_RUNS
                "theoretical", JBool true
                "basis", js "simplification Model.fs + DecisionTeams.fs unchanged; assumptions in this file are [A]"
                "enterprises", ENTERPRISES
                "approaches", jo (items APPROACHES |> List.map (fun (k, v) -> k, jo (items v |> List.filter (fun (kk, _) -> kk <> "overrides"))))
                "licence", LICENCE
                "complex_policy", COMPLEX_POLICY
                "licence_per_account",
                jo (
                    [ "favourable"; "base"; "adverse"; "market_exceeding_reference" ]
                    |> List.map (fun w -> w, jn (licencePerAccount cfg w None))
                ) ]
          "comparison", jo comparison
          "scales", jo scales
          "revenue", jo revenue
          "licence_sensitivity", jo corrected
          "vendor_pnl", jl (vendorTable cfg)
          "vendor_assumptions", VENDOR
          "december_surge", jo (ents |> List.map (fun e -> e, jl (decemberSurge cfg e)))
          "staffed_offer", jl (staffedTable cfg)
          "staffed_assumptions", STAFFED_JSON
          "sensitivity", jo (ents |> List.map (fun e -> e, jl (sensitivity cfg e (accountsOf e))))
          "stress", jo (ents |> List.map (fun e -> e, jl (stress cfg e (accountsOf e))))
          "segment_cost_to_serve", jo segment ]

/// rounded(): round(x, 4) for every float, recursively; ints, strings, booleans and None unchanged.
let rec rounded (j: Json) : Json =
    match j with
    | JFloat x -> JFloat(PyFloat.roundTo 4 x)
    | JObj kvs -> JObj(kvs |> List.map (fun (k, v) -> k, rounded v))
    | JArr xs -> JArr(xs |> List.map rounded)
    | other -> other

/// str() of a value as csv.DictWriter writes it.
let private cell (j: Json) : string =
    match j with
    | JStr s -> s
    | JInt _
    | JFloat _ -> Num.str (num j)
    | JNull -> ""
    | JBool b -> if b then "True" else "False"
    | other -> failwith $"unexpected CSV value {other}"

let activityCsv (res: Json) : string =
    let rows =
        [ for e, byA in items res?comparison do
              for a, s in items byA do
                  for act, v in items s?activities do
                      yield [ e; a; act; Num.str (Num.round 4 (num v)) ] ]
    [ "enterprise"; "approach"; "activity"; "usd_per_account" ] :: rows |> List.map (csvRow "\n") |> String.concat ""

let sensCsv (res: Json) : string =
    let rows = [ for _, rs in items res?sensitivity do yield! PyJson.list rs ]
    let fields = keys rows.Head
    fields :: (rows |> List.map (fun r -> fields |> List.map (fun f -> cell r?(f)))) |> List.map (csvRow "\n") |> String.concat ""

let summaryText (res: Json) : string =
    let summary =
        jo (
            items res?comparison
            |> List.map (fun (e, by) ->
                e,
                jo (
                    items by
                    |> List.map (fun (a, s) ->
                        a, jl [ s?total_economic_admin_per_account; s?range?favourable_p10; s?range?adverse_p90 ])
                ))
        )
    PyJson.dumpsWith { PyJson.defaults with Indent = Some 1; SortKeys = true } summary

type Outputs =
    { ResultsJson: string
      ActivityCsv: string
      SensitivityCsv: string
      Summary: string }

let outputs (baseCfg: Json) : Outputs =
    let res = rounded (results baseCfg)
    { ResultsJson = dumpSorted res
      ActivityCsv = activityCsv res
      SensitivityCsv = sensCsv res
      Summary = summaryText res }
