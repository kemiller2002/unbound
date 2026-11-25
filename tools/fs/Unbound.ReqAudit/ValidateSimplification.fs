/// validate_simplification.py: quality gates for the simplification and economic-engineering mission. Reuses the gates of
/// Validate and ValidateDiscovery and adds S1-S7.
module Unbound.ReqAudit.ValidateSimplification

open System
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat
open Unbound.ReqAudit.Validate
module VD = Unbound.ReqAudit.ValidateDiscovery

let MISSION = "requirements/discovery/2026-11-24-simplification-economics"
let DISCOVERY = "requirements/discovery/2026-11-23-cross-industry"

let NEW_ID = re @"^(RR-.+-r\d+|UNB-SMP-[A-Z]{3}-\d{3}|SP[FDSR]-[A-Z]{3}-\d{3}|SP[WMGCTX]-\d{3}|ORG-R\d{2})$"

let CITED =
    re (@"\b(UNB-SMP-[A-Z]{3}-\d{3}|SP[FDSR]-[A-Z]{3}-\d{3}|SP[WMGCTX]-\d{3}|"
        + @"UNB-XIN-[A-Z]{3}-\d{3}|XI[FCD]-[A-Z]{3}-\d{3}|XDS-[A-Z]{3}-\d{3}|XRJ-[A-Z]{3}-\d{3}|"
        + @"UNB-RCN-[A-Z]+-\d{3}|RCF-[A-Z]+-\d{3}|RCC-[A-Z]+-\d{3}|RCD-\d{3}|DKS-[A-Z]+-\d{3}|RR-[A-Z0-9-]+?-r\d+)\b")

let DISPOSITIONS =
    set [ "retain"; "simplify"; "consolidate"; "replace"; "automate-deterministic"; "automate-ai-assisted"
          "exception-driven"; "conditional-on-operating-model"; "defer"; "recommend-removal"; "need-evidence" ]

let LEVELS = set [ "none"; "low"; "medium"; "high" ]

let ORIGINS =
    set [ "legacy-systems"; "department-silos"; "digitized-paper"; "integration-gaps"; "information-unavailable"
          "human-compensating-for-technology"; "vendor-limitation"; "fee-model-dependency"
          "conservative-interpretation"; "policy-accretion"; "none" ]

let private keys = [ "id"; "refinement_id"; "finding_id"; "conflict_id"; "scenario_id"; "decision_id" ]

/// Every record identifier defined in a mission data directory (JSONL 'id'-like keys and CSV first columns).
let idsOf (data: string) : Set<string> =
    let collect (recs: Json list) = recs |> List.collect (fun r -> keys |> List.map (fun k -> get k r) |> List.filter truthy |> List.map str)
    let jl = glob data "*.jsonl" |> List.filter IO.File.Exists |> List.collect (Io.loadJsonl >> collect)
    let cv = glob data "*.csv" |> List.filter (fun p -> IO.File.Exists p && name p <> "trace-edges.csv") |> List.collect (Io.loadCsv >> collect)
    Set.ofList (jl @ cv)

let universe (recon: string) (disc: string) (dataDir: string) : Set<string> =
    let inv = Io.loadCsv (combine recon "inventory.csv")
    let baseIds =
        inv
        |> List.filter (fun r -> s "kind" r = "requirement" && not ((s "identifier" r).EndsWith("-Q", StringComparison.Ordinal)))
        |> List.map (s "identifier") |> Set.ofList
    let recs =
        Set.unionMany
            [ column "refinement_id" (Io.loadJsonl (combine recon "refinements.jsonl"))
              column "id" (Io.loadJsonl (combine recon "gap-requirements.jsonl"))
              column "refinement_id" (Io.loadJsonl (combine disc "refinements.jsonl"))
              column "id" (Io.loadJsonl (combine disc "gap-requirements.jsonl")) ]
    let second =
        Io.loadCsv (combine dataDir "second-site-accounting.csv")
        |> List.filter (fun r -> s "action" r = "reviewed-separately") |> List.map (s "register_id") |> Set.ofList
    Set.unionMany [ baseIds; recs; second ]

let gScheme (newIds: Set<string>) (baseline: Set<string>) (prior: Set<string>) : Issue list =
    (sortStrings (Set.intersect newIds baseline) |> List.map (fun i -> err "G2" i "new identifier collides with a baseline identifier"))
    @ (sortStrings (Set.intersect newIds prior) |> List.map (fun i -> err "G2" i "new identifier collides with a prior mission record"))
    @ (sortStrings newIds |> List.filter (fun i -> not (isMatch NEW_ID i))
       |> List.map (fun i -> err "G2" i "identifier does not follow a simplification-mission ID scheme"))

let private inSet (allowed: Set<string>) (j: Json) = match j with JStr v -> allowed.Contains v | _ -> false

let gRegister (reg: Json list) (scope: Set<string>) : Issue list =
    let counts = counter (reg |> List.map (get "id"))
    let counted = counts |> List.choose (fun (k, _) -> match k with JStr v -> Some v | _ -> None) |> Set.ofList
    let missing = sortStrings (Set.difference scope counted) |> List.map (fun i -> err "S1" i "requirement not reviewed in the register")
    let extra =
        sortJson (counts |> List.map fst |> List.filter (fun k -> match k with JStr v -> not (scope.Contains v) | _ -> true))
        |> List.map (fun i -> err "S1" (str i) "register row for an identifier outside the review universe")
    let dup = counts |> List.filter (fun (_, n) -> n > 1) |> List.map (fun (i, n) -> err "S1" (str i) $"reviewed {n} times")
    let row (r: Json) : Issue list =
        let i = str (getOr "id" (JStr "?") r)
        [ if not (inSet DISPOSITIONS (get "disposition" r)) then yield err "S2" i $"""disposition {repr (get "disposition" r)}"""
          for f in [ "human_work"; "customer_friction"; "configuration_burden" ] do
              let level = get "level" (orElse (JObj []) (get f r))
              if not (inSet LEVELS level) then yield err "S2" i $"{f}.level {repr level}"
          let bad = iter (orElse (JArr []) (get "convention_origin" r)) |> List.filter (fun o -> not (inSet ORIGINS o))
          if not bad.IsEmpty then yield err "S2" i $"convention_origin {repr (JArr bad)}"
          for f in [ "original_purpose"; "reasoning"; "confidence" ] do
              if empty (get f r) then yield err "S2" i $"missing {f}"
          if pyEq (get "disposition" r) (JStr "recommend-removal") then
              for f in [ "elimination_consequence"; "evidence_to_change" ] do
                  if empty (get f r) then yield err "S3" i $"removal recommended without {f}" ]
    missing @ extra @ dup @ (reg |> List.collect row)

let gChains (reg: Json list) : Issue list =
    let by = reg |> List.fold (fun (m: Map<string, Json>) r -> m.Add(s "id" r, r)) Map.empty
    let rec endOf (i: string) (seen: string list) : bool * string =
        match by.TryFind i with
        | None -> true, i
        | Some r ->
            let disp = at "disposition" r
            if pyEq disp (JStr "recommend-removal") && not seen.IsEmpty then false, $"{i} (recommend-removal)"
            elif pyEq disp (JStr "defer") && not seen.IsEmpty then true, $"DEFERRED:{i}"
            elif not (pyEq disp (JStr "consolidate")) then true, i
            else
                let targets = iter (orElse (JArr []) (get "consolidate_with" r)) |> List.map asStr |> List.filter (fun t -> t <> i)
                if targets.IsEmpty then false, $"{i} (no target)"
                elif List.contains i seen then false, "cycle " + String.Join(" -> ", seen @ [ i ])
                else
                    let results = targets |> List.map (fun t -> endOf t (seen @ [ i ]))
                    match results |> List.tryFind fst with
                    | Some(_, where) -> true, where
                    | None -> results.Head
    reg
    |> List.collect (fun r ->
        if pyEq (at "disposition" r) (JStr "consolidate") then
            let ok, where = endOf (s "id" r) []
            if not ok then [ err "S7" (s "id" r) $"consolidation chain does not reach a retained carrier: {where}" ]
            elif where.StartsWith("DEFERRED:", StringComparison.Ordinal) then
                [ issue "WARN" "S7" (s "id" r) $"obligation is carried by a deferred requirement ({where.Substring 9}); it is deferred with that capability" ]
            else []
        else [])

let gMetrics (metrics: Json list) : Issue list =
    metrics
    |> List.collect (fun m ->
        let i = str (getOr "id" (JStr "?") m)
        let keep = (upper (str (getOr "keep" (JStr "") m))).StartsWith("Y", StringComparison.Ordinal)
        let need = if keep then [ "definition"; "decision_supported"; "expected_action"; "authoritative_source" ] else [ "keep" ]
        need |> List.filter (fun f -> empty (get f m)) |> List.map (fun f -> err "S4" i $"missing {f}"))

let main (argv: string list) : int =
    let root = resolve argv.[0]
    let mission = combine root (if argv.Length > 1 then argv.[1] else MISSION)
    let data, recon, disc = combine mission "data", combine root (RECON + "/data"), combine root (DISCOVERY + "/data")
    let rc n = combine recon n
    let inv = Io.loadCsv (rc "inventory.csv")
    let baseline = column "identifier" inv
    let prior = Set.union (Set.difference (idsOf recon) baseline) (idsOf disc)
    let narr =
        Set.unionMany
            [ column "surrogate_id" (Io.loadCsv (rc "narrative-requirements.csv"))
              column "capability_id" (Io.loadCsv (rc "competitive-rollup.csv"))
              column "obligation_id" (Io.loadCsv (rc "regulatory-obligations.csv"))
              column "identifier" (Io.loadCsv (rc "withdrawn-ids.csv")) ]
    let knownUndefined =
        Set.union (column "identifier" (Io.loadCsv (rc "undefined-references.csv"))) (column "identifier" (Io.loadCsv (combine data "external-ids.csv")))
    let L n = Io.loadJsonl (combine data n)
    let C n = Io.loadCsv (combine data n)
    let refs, gaps, reg = L "refinements.jsonl", L "gap-requirements.jsonl", L "register.jsonl"
    let finds, confs, scens, decs = C "findings.csv", C "conflicts.csv", C "scenarios.csv", C "decisions.csv"
    let rejected, work, metrics = L "rejected.jsonl", L "work-inventory.jsonl", L "metrics.jsonl"
    let config, inter, comp, transfer = L "config-items.jsonl", L "interactions.jsonl", L "comparison.jsonl", L "transfer.jsonl"
    let edges = C "trace-edges.csv"
    let sets =
        [ "refinements", ids "refinement_id" refs
          "gaps", ids "id" gaps
          "findings", ids "finding_id" finds
          "conflicts", ids "conflict_id" confs
          "scenarios", ids "scenario_id" scens
          "decisions", ids "decision_id" decs
          "rejected", ids "id" rejected
          "work", ids "id" work
          "metrics", ids "id" metrics
          "config", ids "id" config
          "interactions", ids "id" inter
          "comparison", ids "id" comp
          "transfer", ids "id" transfer
          "roles", L "roles.jsonl" |> List.map (fun r -> str (if truthy (get "id" r) then get "id" r else at "role_id" r)) ]
    let setOf k = sets |> List.find (fst >> (=) k) |> snd
    let newIds = sets |> List.collect snd |> Set.ofList
    let ok = resolver baseline (Set.union newIds prior) narr knownUndefined
    let split = Graph.idsOf
    let bf = baselineFiles root refs
    let issues =
        [ yield! gDupes sets
          yield! gScheme newIds baseline prior
          yield! VD.gRefinementsCsvAware bf refs baseline
          yield! gRequired "refinements" refs "refinement_id"
          yield! gRequired "gaps" gaps "id"
          yield! gRequired "findings" finds "finding_id"
          yield! gRequired "conflicts" confs "conflict_id"
          yield! gRequired "scenarios" scens "scenario_id"
          yield! gRequired "decisions" decs "decision_id"
          yield! gAcceptance refs "refinement_id"
          yield! gAcceptance gaps "id"
          yield! gGaps gaps (Set.ofList (setOf "findings"))
          for f in finds do yield! gRefs "affected_ids" (s "finding_id" f) (split (at "affected_ids" f)) ok
          for c in confs do yield! gRefs "requirements" (s "conflict_id" c) (split (at "requirement_a" c) @ split (at "requirement_b" c)) ok
          for sc in scens do yield! gRefs "requirement_ids" (s "scenario_id" sc) (split (at "requirement_ids" sc)) ok
          for x in decs do yield! gRefs "affected_ids" (s "decision_id" x) (split (at "affected_ids" x)) ok
          for g in gaps do yield! gRefs "links" (s "id" g) (strList (getOr "links" (JArr []) g)) ok
          yield! gEdges edges ok
          yield! VD.gFields "D2" gaps "id" VD.SECTION11
          yield! VD.gEnum "D3" gaps "id" "priority" VD.PRIORITY
          yield! VD.gFields "D5" rejected "id" [ "idea"; "argue_for"; "argue_against"; "reason_rejected" ]
          yield! gRegister reg (universe recon disc data)
          yield! gChains reg
          yield! VD.gRefsList "S2" reg "id" [ "dependencies"; "consolidate_with" ] ok
          yield! gMetrics metrics
          yield! VD.gFields "S5" work "id" [ "function"; "activity"; "trigger"; "why_exists"; "if_not_performed"; "automation" ]
          yield! VD.gRefsList "S5" work "id" [ "existing_requirement_ids"; "consolidate_with"; "depends_on" ] ok
          yield! VD.gFields "S5" config "id" [ "item"; "who_needs_it"; "why"; "disposition" ]
          yield! VD.gRefsList "S5" config "id" [ "source_ids" ] ok
          yield! VD.gFields "S5" inter "id" [ "persona"; "step"; "information_requested"; "recommendation" ]
          yield! VD.gRefsList "S5" inter "id" [ "existing_ids" ] ok
          yield! VD.gFields "S5" comp "id" [ "provider"; "dimension"; "evidence_grade"; "assessment" ]
          yield! VD.gFields "S5" transfer "id" [ "research_question"; "finding_or_hypothesis"; "suggested_daf_landscape_destination" ]
          for t in transfer do
              if take 1 (upper (str (getOr "independent_of_unbound" (JStr "") t))) <> "Y" then
                  yield err "S5" (s "id" t) "transfer candidate not independent of Unbound"
          yield! VD.gTestedStrict (setOf "refinements" @ setOf "gaps") edges
          yield! VD.gLandscape (VD.landscapeState root)
          yield! VD.gReportsD "S6" CITED (VD.missionDocs mission) ok ]
    IO.Directory.CreateDirectory data |> ignore
    report (combine data "validation-report.json") (sets |> List.map (fun (k, v) -> k, v.Length))
        [ "register_rows", jint reg.Length; "trace_edges", jint edges.Length ] issues
