/// validate_discovery.py: quality gates for the cross-industry discovery records. Reuses the pure gates of Validate
/// (G1, G3-G9, G11) and adds the discovery gates D1-D10.
module Unbound.ReqAudit.ValidateDiscovery

open System
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat
open Unbound.ReqAudit.Validate

let MISSION = "requirements/discovery/2026-11-23-cross-industry"

let NEW_ID =
    re (@"^(RR-.+-r\d+|UNB-XIN-[A-Z]{3}-\d{3}|XIF-[A-Z]{3}-\d{3}|XIC-[A-Z]{3}-\d{3}|XID-[A-Z]{3}-\d{3}|"
        + @"XDS-[A-Z]{3}-\d{3}|XCAP-[A-Z]{3}-\d{3}|XRJ-[A-Z]{3}-\d{3}|XFC-[A-Z]{3}-\d{3}|XEV-\d{3}|XRA-\d{3}|"
        + @"XWF-\d{3}|XADR-\d{3})$")

let CITED =
    re (@"\b(UNB-XIN-[A-Z]{3}-\d{3}|XIF-[A-Z]{3}-\d{3}|XIC-[A-Z]{3}-\d{3}|XID-[A-Z]{3}-\d{3}|XDS-[A-Z]{3}-\d{3}|"
        + @"XCAP-[A-Z]{3}-\d{3}|XRJ-[A-Z]{3}-\d{3}|XFC-[A-Z]{3}-\d{3}|XEV-\d{3}|XRA-\d{3}|XWF-\d{3}|XADR-\d{3}|"
        + @"RR-[A-Z0-9-]+?-r\d+|UNB-RCN-[A-Z]+-\d{3}|RCF-[A-Z]+-\d{3}|RCC-[A-Z]+-\d{3}|RCD-\d{3}|DKS-[A-Z]+-\d{3})\b")

let COVERAGE = set [ "covered"; "partial"; "contradictory"; "missing" ]
let NECESSITY = set [ "legal"; "fiduciary"; "contractual"; "operational"; "commercial"; "conventional" ]
let EQUIV = set [ "yes"; "partial"; "no" ]
let AUTOMATION = set [ "deterministic"; "ai-assisted"; "configuration"; "human-required"; "mixed" ]
let AUTHORITY = set [ "authoritative-financial"; "authoritative-nonfinancial"; "derived"; "informational"; "observation-ai" ]
let PRIORITY = set [ "P0"; "P1"; "P2"; "P3"; "P4" ]

let SECTION11 =
    [ "discovery_source"; "industry_analogy"; "parent_capability"; "business_problem"; "applicability"
      "failure_conditions"; "priority_justification"; "security_compliance"; "ui_implications"
      "api_implications"; "reporting_implications"; "testing_requirements"; "argue_for"; "argue_against"
      "simplest_valid_alternative"; "decision_rationale"; "confidence" ]

/// as_list(x): a list as is, anything else through ids_in(str(x or "")).
let asList (x: Json) : string list =
    match x with
    | JArr xs -> xs |> List.map asStr
    | x -> Graph.idsIn (str (orElse (JStr "") x))

let private inSet (allowed: Set<string>) (j: Json) = match j with JStr v -> allowed.Contains v | _ -> false

let gScheme (newIds: Set<string>) (baseline: Set<string>) (prior: Set<string>) : Issue list =
    (sortStrings (Set.intersect newIds baseline) |> List.map (fun i -> err "G2" i "new identifier collides with a baseline identifier"))
    @ (sortStrings (Set.intersect newIds prior) |> List.map (fun i -> err "G2" i "new identifier collides with a reconciliation record"))
    @ (sortStrings newIds |> List.filter (fun i -> not (isMatch NEW_ID i))
       |> List.map (fun i -> err "G2" i "identifier does not follow a discovery ID scheme"))

let gEnum (gate: string) (recs: Json list) (key: string) (field: string) (allowed: Set<string>) : Issue list =
    recs
    |> List.filter (fun r -> not (inSet allowed (get field r)))
    |> List.map (fun r -> err gate (str (getOr key (JStr "?") r)) $"{field}={repr (get field r)} not in {reprList (sortStrings allowed)}")

let gFields (gate: string) (recs: Json list) (key: string) (fields: string list) : Issue list =
    recs
    |> List.collect (fun r ->
        fields |> List.filter (fun f -> empty (get f r)) |> List.map (fun f -> err gate (str (getOr key (JStr "?") r)) $"missing or empty field '{f}'"))

/// inventory.py renders a CSV definition row as its cells joined by ' | '; rebuild that text at baseline.
let csvRowText (baselineFile: BaselineFiles) (path: string) (line: int) : string =
    let rows = PyCsv.parse (baselineFile path)
    if 0 < line && line <= rows.Length then String.Join(" | ", rows.[line - 1]) else ""

/// int(x) for a loaded value.
let private toInt (j: Json) : int =
    match j with
    | JInt i -> int i
    | JBool b -> (if b then 1 else 0)
    | JFloat x -> int (Math.Truncate x)
    | JStr v -> int (pyInt v)
    | j -> raise (PyError $"int() argument must be a string or a number, not {repr j}")

/// validate.g_refinements, except G4 for CSV-defined originals compares against the rebuilt row text.
let gRefinementsCsvAware (baselineFile: BaselineFiles) (refs: Json list) (baseline: Set<string>) : Issue list =
    let csvOk (i: Issue) =
        let r = refs |> List.tryFind (fun x -> pyEq (get "refinement_id" x) (JStr i.Subject)) |> Option.defaultValue (JObj [])
        i.Gate = "G4"
        && (str (getOr "original_path" (JStr "") r)).EndsWith(".csv", StringComparison.Ordinal)
        && (normalize (csvRowText baselineFile (s "original_path" r) (toInt (getOr "original_line" (jint 0) r))))
            .Contains(take 160 (normalizeJ (getOr "original_text" (JStr "") r)), StringComparison.Ordinal)
    gRefinements baselineFile refs baseline |> List.filter (fun i -> not (csvOk i))

let gEvents (events: Json list) : Issue list =
    events
    |> List.collect (fun e ->
        let i = str (getOr "id" (JStr "?") e)
        let auth = get "authority" e
        [ if not (inSet AUTHORITY auth) then yield err "D4" i $"authority {repr auth} not enumerated"
          if empty (get "owning_module" e) then yield err "D4" i "no owning module"
          if pyEq auth (JStr "observation-ai") && (lower (str (getOr "source_of_truth_store" (JStr "") e))).Contains "ledger" then
              yield err "D4" i "AI observation declared as stored in an authoritative ledger"
          if pyEq auth (JStr "observation-ai") && pyEq (get "consistency" e) (JStr "synchronous") then
              yield err "D4" i "AI observation declared synchronous-consistent" ])

let gRefsList (gate: string) (recs: Json list) (key: string) (fields: string list) (ok: string -> bool) : Issue list =
    recs
    |> List.collect (fun r ->
        fields
        |> List.collect (fun f ->
            asList (get f r)
            |> List.filter (fun ref -> strip ref <> "" && isMatch ANY_ID (strip ref) && not (ok ref))
            |> List.map (fun ref -> err gate (str (getOr key (JStr "?") r)) $"{f}: unresolved reference '{ref}'")))

let gTestedStrict (newReqs: string list) (edges: Json list) : Issue list =
    let tested = testedSources edges
    newReqs |> List.filter (fun r -> not (tested.Contains r)) |> List.map (fun r -> err "D7" r "no tested_by scenario")

let gOutcomes (outcomes: Json list) (decs: Json list) : Issue list =
    let status = decs |> List.fold (fun (m: Map<string, Json>) d -> m.Add(s "decision_id" d, get "status" d)) Map.empty
    outcomes
    |> List.collect (fun o ->
        let iv = getOr "decision_id" (JStr "?") o
        let i = str iv
        let known = match iv with JStr k -> status.TryFind k | _ -> None
        [ match known with
          | None -> yield err "D10" i "outcome for an unknown decision"
          | Some st when not (pyEq st (get "status" o)) ->
              yield err "D10" i $"""decisions.csv status {repr st} disagrees with outcome {repr (get "status" o)}"""
          | Some _ -> ()
          for f in [ "decision"; "rationale"; "authority"; "confirmation_required"; "confidence" ] do
              if empty (get f o) then yield err "D10" i $"missing or empty field '{f}'" ])

/// D8: the daf-landscape clone beside the repository (None when absent, else whether `git status --porcelain` is empty).
let gLandscape (clean: bool option) : Issue list =
    match clean with
    | None -> [ issue "WARN" "D8" "daf-landscape" "clone not found beside repository; unchanged-state not checked" ]
    | Some true -> []
    | Some false -> [ err "D8" "daf-landscape" "working tree has changes" ]

let landscapeState (root: string) : bool option =
    let other = combine (IO.Path.GetDirectoryName root) "daf-landscape"
    if not (exists (combine other ".git")) then None
    else Some(strip (Io.run "git" [ "-C"; other; "status"; "--porcelain" ]).Out = "")

let gReportsD (gate: string) (cited: Re) (docs: (string * string) list) (ok: string -> bool) : Issue list =
    docs
    |> List.collect (fun (name, text) ->
        findall cited text |> Set.ofList |> sortStrings |> List.filter (fun m -> not (ok m))
        |> List.map (fun m -> err gate name $"cites unknown identifier {m}"))

let missionDocs (mission: string) = rglobFiles mission "*.md" |> List.map (fun p -> name p, readText p)

/// Identifiers defined in the machine-readable registers of the other discovery packages under
/// requirements/discovery (refinements.jsonl refinement_id, gap-requirements.jsonl id). A later package may
/// refine a requirement this mission introduced (e.g. RR-ADM-023-r1 -> RR-ADM-023-r2 in
/// 2026-11-24-simplification-economics); citing that successor is a reference to a registered identifier, not an
/// unknown one. Only register rows count: prose in another package never makes an identifier resolve. These IDs feed
/// the resolver only; they are not part of the G2 collision set, because sibling packages legitimately re-list their
/// predecessors.
let siblingRegisterIds (discoveryRoot: string) (mission: string) : Set<string> =
    let self = IO.Path.GetFullPath mission |> fun p -> p.TrimEnd(IO.Path.DirectorySeparatorChar)
    if not (IO.Directory.Exists discoveryRoot) then Set.empty
    else
        IO.Directory.GetDirectories discoveryRoot
        |> Array.toList
        |> List.filter (fun dir -> IO.Path.GetFullPath(dir).TrimEnd(IO.Path.DirectorySeparatorChar) <> self)
        |> List.sort
        |> List.collect (fun dir ->
            [ column "refinement_id" (Io.loadJsonl (IO.Path.Combine(dir, "data", "refinements.jsonl")))
              column "id" (Io.loadJsonl (IO.Path.Combine(dir, "data", "gap-requirements.jsonl"))) ])
        |> Set.unionMany

let main (argv: string list) : int =
    let root = resolve argv.[0]
    let mission = combine root (if argv.Length > 1 then argv.[1] else MISSION)
    let data, recon = combine mission "data", combine root (RECON + "/data")
    let d n = combine data n
    let rc n = combine recon n
    let inv = Io.loadCsv (rc "inventory.csv")
    let baseline = column "identifier" inv
    let prior =
        Set.unionMany
            [ column "refinement_id" (Io.loadJsonl (rc "refinements.jsonl"))
              column "id" (Io.loadJsonl (rc "gap-requirements.jsonl"))
              column "finding_id" (Io.loadCsv (rc "findings.csv"))
              column "conflict_id" (Io.loadCsv (rc "conflicts.csv"))
              column "scenario_id" (Io.loadCsv (rc "scenarios.csv"))
              column "decision_id" (Io.loadCsv (rc "decisions.csv")) ]
    let narr =
        Set.unionMany
            [ column "surrogate_id" (Io.loadCsv (rc "narrative-requirements.csv"))
              column "capability_id" (Io.loadCsv (rc "competitive-rollup.csv"))
              column "obligation_id" (Io.loadCsv (rc "regulatory-obligations.csv"))
              column "identifier" (Io.loadCsv (rc "withdrawn-ids.csv")) ]
    let knownUndefined = column "identifier" (Io.loadCsv (rc "undefined-references.csv"))
    let refs, gaps = Io.loadJsonl (d "refinements.jsonl"), Io.loadJsonl (d "gap-requirements.jsonl")
    let finds, confs = Io.loadCsv (d "findings.csv"), Io.loadCsv (d "conflicts.csv")
    let scens, decs = Io.loadCsv (d "scenarios.csv"), Io.loadCsv (d "decisions.csv")
    let atlas, rejected = Io.loadJsonl (d "capability-atlas.jsonl"), Io.loadJsonl (d "rejected.jsonl")
    let failures, events = Io.loadJsonl (d "failure-catalog.jsonl"), Io.loadJsonl (d "event-matrix.jsonl")
    let regs = Io.loadJsonl (d "regulatory-applicability.jsonl")
    let outcomes = Io.loadJsonl (d "decision-outcomes.jsonl")
    let edges = Io.loadCsv (d "trace-edges.csv")
    let sets =
        [ "refinements", ids "refinement_id" refs
          "gaps", ids "id" gaps
          "findings", ids "finding_id" finds
          "conflicts", ids "conflict_id" confs
          "scenarios", ids "scenario_id" scens
          "decisions", ids "decision_id" decs
          "atlas", ids "id" atlas
          "rejected", ids "id" rejected
          "failures", ids "id" failures
          "events", ids "id" events
          "regulatory", ids "id" regs ]
    let setOf k = sets |> List.find (fst >> (=) k) |> snd
    let newIds = sets |> List.collect snd |> Set.ofList
    let siblings = siblingRegisterIds (combine root "requirements/discovery") mission
    let ok = resolver baseline (Set.unionMany [ newIds; prior; siblings ]) narr knownUndefined
    let split = Graph.idsOf
    let bf = baselineFiles root refs
    let issues =
        [ yield! gDupes sets
          yield! gScheme newIds baseline prior
          yield! gRefinementsCsvAware bf refs baseline
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
          for r in refs do yield! gRefs "related_findings" (s "refinement_id" r) (strList (getOr "related_findings" (JArr []) r)) ok
          for g in gaps do yield! gRefs "links" (s "id" g) (strList (getOr "links" (JArr []) g)) ok
          yield! gEdges edges ok
          yield! gEnum "D1" atlas "id" "coverage" COVERAGE
          yield! gEnum "D1" atlas "id" "necessity" NECESSITY
          yield! gEnum "D1" atlas "id" "daf_equivalent" EQUIV
          yield! gEnum "D1" atlas "id" "automation" AUTOMATION
          yield! gRefsList "D1" atlas "id" [ "existing_ids" ] ok
          yield! gFields "D2" gaps "id" SECTION11
          yield! gEnum "D3" gaps "id" "priority" PRIORITY
          yield! gEvents events
          yield! gRefsList "D4" events "id" [ "existing_ids" ] ok
          yield! gFields "D5" rejected "id" [ "idea"; "argue_for"; "argue_against"; "reason_rejected" ]
          yield! gFields "D6" failures "id" [ "failure_mode"; "detection"; "recovery"; "escalation" ]
          yield! gRefsList "D6" failures "id" [ "covered_by"; "new_ids" ] ok
          yield! gRefsList "D1" regs "id" [ "related_OB_ids"; "existing_requirement_ids" ] ok
          yield! gTestedStrict (setOf "refinements" @ setOf "gaps") edges
          yield! gOutcomes outcomes decs
          yield! gLandscape (landscapeState root)
          yield! gReportsD "D9" CITED (missionDocs mission) ok ]
    report (d "validation-report.json") (sets |> List.map (fun (k, v) -> k, v.Length)) [ "trace_edges", jint edges.Length ] issues
