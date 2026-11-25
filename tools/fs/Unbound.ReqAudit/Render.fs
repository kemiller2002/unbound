/// render.py: render data-driven tables into the reconciliation Markdown reports between
/// <!-- BEGIN GENERATED: name --> ... <!-- END GENERATED: name --> markers.
module Unbound.ReqAudit.Render

open System
open System.Text.RegularExpressions
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

let RECON = "requirements/reconciliation/2026-11-23"
let SEV = [ "critical"; "high"; "medium"; "low" ]
let DOMAINS = [ "GRT"; "FIN"; "PLT"; "ENT"; "REG"; "ARC" ]

let private ws = re @"\s+"
let private pipe = re @"(?<!\\)\|"

/// cell(text, limit): `str(text or "")`, whitespace collapsed, unescaped pipes escaped, truncated with an ellipsis.
let cellL (limit: int) (text: Json) : string =
    let t = strip (subLit pipe @"\|" (subLit ws " " (str (orElse (JStr "") text))))
    if len t <= limit then t else rstrip (take (limit - 1) t) + "…"

let cell = cellL 220
let cs (limit: int) (s: string) = JStr(cellL limit (JStr s))

let table (header: string list) (rows: Json list list) : string =
    [ yield "| " + String.Join(" | ", header) + " |"
      yield "|" + String.Join("|", header |> List.map (fun _ -> "---")) + "|"
      for r in rows do yield "| " + String.Join(" | ", r |> List.map cell) + " |" ]
    |> String.concat "\n"

let domain (i: string) = if (i |> Seq.filter ((=) '-') |> Seq.length) >= 2 then i.Split('-').[1] else "?"

type Data =
    { F: Json list; C: Json list; S: Json list; D: Json list; E: Json list; Q: Json list; R: Json list; G: Json list
      U: Json list; UG: Json list; W: Json list; IA: Json list; UM: Json list; UC: Json list; UT: Json list; K: Json list
      V: Json }

let J = JStr
let N (n: int) = jint n

let private sevIndex (j: Json) = List.findIndex ((=) (asStr j)) SEV

let gFindingsMatrix (d: Data) =
    let c = counter (d.F |> List.map (fun f -> domain (s "finding_id" f), s "severity" f))
    let rows = DOMAINS |> List.map (fun dm -> J dm :: (SEV |> List.map (fun sv -> N(count c (dm, sv)))) @ [ N(SEV |> List.sumBy (fun sv -> count c (dm, sv))) ])
    let tot = J "**Total**" :: (SEV |> List.map (fun sv -> N(DOMAINS |> List.sumBy (fun dm -> count c (dm, sv))))) @ [ N d.F.Length ]
    table ("Domain" :: SEV @ [ "Total" ]) (rows @ [ tot ])

let gFindingsDimension (d: Data) =
    let c = counter (d.F |> List.map (s "dimension"))
    table [ "Dimension"; "Findings" ] (byCountDesc c |> List.map (fun (k, n) -> [ J k; N n ]))

let private severeSort (fs: Json list) =
    fs |> List.sortWith (fun a b -> match compare (sevIndex (at "severity" a)) (sevIndex (at "severity" b)) with 0 -> cmpStr (s "finding_id" a) (s "finding_id" b) | c -> c)

let gFindingsSevere (d: Data) =
    let rows = d.F |> List.filter (fun f -> List.contains (s "severity" f) [ "critical"; "high" ]) |> severeSort
    table [ "ID"; "Sev."; "Dimension"; "Finding"; "Affected"; "Proposed resolution" ]
        (rows |> List.map (fun f -> [ at "finding_id" f; at "severity" f; at "dimension" f; at "title" f; J(cellL 90 (at "affected_ids" f)); J(cellL 200 (at "proposed_resolution" f)) ]))

let gFindingsOther (d: Data) =
    let rows = d.F |> List.filter (fun f -> List.contains (s "severity" f) [ "medium"; "low" ]) |> severeSort
    table [ "ID"; "Sev."; "Dimension"; "Finding"; "Affected" ]
        (rows |> List.map (fun f -> [ at "finding_id" f; at "severity" f; at "dimension" f; at "title" f; J(cellL 90 (at "affected_ids" f)) ]))

let gConflicts (d: Data) =
    table [ "ID"; "Requirement A"; "Requirement B"; "Incompatibility"; "Recommended resolution"; "Human authority" ]
        (d.C |> List.map (fun c ->
            [ at "conflict_id" c; J(cellL 80 (at "requirement_a" c)); J(cellL 80 (at "requirement_b" c)); J(cellL 260 (at "incompatibility" c))
              J(cellL 220 (at "recommended_resolution" c)); getOr "requires_human_authority" (J "") c ]))

let gGaps (d: Data) =
    let rows = d.G |> List.sortWith (fun a b -> cmpTuple [ at "priority" a; at "id" a ] [ at "priority" b; at "id" b ])
    table [ "ID"; "Pri."; "Parent"; "Requirement (normative)"; "Gap finding"; "Why no predecessor" ]
        (rows |> List.map (fun g ->
            [ at "id" g; at "priority" g; J(cellL 60 (at "parent" g)); J(cellL 300 (at "statement" g)); at "gap_finding" g; J(cellL 140 (at "why_no_predecessor" g)) ]))

let gGapCounts (d: Data) =
    let c = counter (d.G |> List.map (fun g -> domain ((s "id" g).Replace("UNB-RCN-", "X-")), s "priority" g))
    let doms = c |> List.map (fst >> fst) |> sortStrings |> List.distinct
    table [ "Domain"; "P0"; "P1"; "P2"; "Total" ]
        (doms |> List.map (fun dm -> [ J dm; N(count c (dm, "P0")); N(count c (dm, "P1")); N(count c (dm, "P2")); N([ "P0"; "P1"; "P2" ] |> List.sumBy (fun p -> count c (dm, p))) ]))

let gRefinementIndex (d: Data) =
    table [ "Refinement"; "Original"; "Relation"; "Domain"; "Decision status"; "Confidence"; "ACs" ]
        (d.R |> List.map (fun r ->
            let file = (s "original_path" r).Split('/') |> Array.last
            [ at "refinement_id" r; J $"""{s "original_id" r} ({file}:{s "original_line" r})"""; at "relationship" r
              getOr "_domain" (J "") r; at "decision_status" r; at "confidence" r; N(jlen (at "acceptance_criteria" r)) ]))

let rec fmtList (v: Json) : string =
    match v with
    | JObj kvs -> kvs |> List.map (fun (k, x) -> $"**{k}:** {fmtList x}") |> String.concat "; "
    | JArr [] -> "—"
    | JArr xs -> xs |> List.map fmtList |> String.concat "; "
    | v -> str v

let private acLine (a: Json) = $"""  - **{s "id" a}** Given {s "given" a}; when {s "when" a}; then {s "then" a}."""

let gRefinementRecords (d: Data) =
    let one (r: Json) =
        let acs = iter (at "acceptance_criteria" r) |> List.map acLine |> String.concat "\n"
        let g k = get k r
        [ $"""### {s "refinement_id" r} — {s "relationship" r} `{s "original_id" r}`"""
          $"""- **Source:** `{s "original_path" r}:{s "original_line" r}` @ `{s "original_revision" r}` · domain {str (getOr "_domain" (J "") r)} · """
          + $"""{s "decision_status" r} · confidence {s "confidence" r} · {s "validation_status" r}"""
          $"""- **Original text:** {cellL 900 (at "original_text" r)}"""
          $"""- **Refined statement:** {s "statement" r}"""
          $"""- **Business objective:** {s "business_objective" r}"""
          $"""- **Reason for change:** {s "reason_for_change" r}"""
          $"""- **Processing rules:** {fmtList (at "processing_rules" r)}"""
          $"""- **Failure behaviour:** {fmtList (at "failure_behavior" r)}"""
          $"""- **Security / audit:** {fmtList (at "security" r)} / {fmtList (at "auditability" r)}"""
          $"""- **Configuration:** {fmtList (g "configuration")}"""
          $"""- **AI assistance:** {fmtList (g "ai_assistance")}"""
          $"""- **UI / API / reporting:** {fmtList (g "ui_implications")} / {fmtList (g "api_implications")} / {fmtList (g "reporting_implications")}"""
          $"""- **Regulatory:** {fmtList (g "regulatory_implications")}"""
          $"""- **Dependencies:** {fmtList (g "dependencies")} · **Findings:** {fmtList (g "related_findings")}"""
          $"""- **Assumptions / alternatives / tradeoffs:** {fmtList (g "assumptions")} / {fmtList (g "alternatives_considered")} / {fmtList (g "tradeoffs")}"""
          $"""- **Evidence:** {fmtList (g "evidence")}"""
          "- **Acceptance criteria:**"
          acs
          "" ]
        |> String.concat "\n"
    d.R |> List.map one |> String.concat "\n"

let gTraceSummary (d: Data) =
    let c = counter (d.E |> List.map (s "relationship"))
    table [ "Relationship"; "Edges" ] ((byCountDesc c |> List.map (fun (k, n) -> [ J k; N n ])) @ [ [ J "**Total**"; N d.E.Length ] ])

let private pct (a: int) (b: int) = PyFloat.fixedPoint 1 (float (100 * a) / float b) + "%"

let gTraceCoverage (d: Data) =
    let rel r = d.E |> List.filter (fun e -> s "relationship" e = r)
    let tested = rel "tested_by" |> List.map (s "source") |> Set.ofList
    let affected = rel "affects" |> List.map (s "target") |> Set.ofList
    let refined = d.R |> List.map (s "original_id") |> Set.ofList
    let base' = d.Q |> List.map (s "identifier") |> Set.ofList
    let newIds = (d.R |> List.map (s "refinement_id")) @ (d.G |> List.map (s "id"))
    let n = base'.Count
    let inter (x: Set<string>) = (Set.intersect base' x).Count
    let newTested = newIds |> List.filter tested.Contains |> List.length
    table [ "Measure"; "Count"; "Share" ]
        [ [ J "Baseline canonical requirements (denominator)"; N n; J "" ]
          [ J "… affected by at least one finding"; N(inter affected); J(pct (inter affected) n) ]
          [ J "… with a refinement record"; N(inter refined); J(pct (inter refined) n) ]
          [ J "… tested_by at least one scenario (baseline + reconciliation scenarios)"; N(inter tested); J(pct (inter tested) n) ]
          [ J "Reconciliation refinements + gap requirements (denominator)"; N newIds.Length; J "" ]
          [ J "… tested_by at least one scenario"; N newTested; J(pct newTested newIds.Length) ] ]

let gScenarios (d: Data) =
    table [ "ID"; "Scenario"; "Requirements"; "Stimulus"; "Expected states"; "Financial invariants"; "Recovery" ]
        (d.S |> List.map (fun sc ->
            [ at "scenario_id" sc; at "title" sc; J(cellL 70 (at "requirement_ids" sc)); J(cellL 140 (at "stimulus" sc)); J(cellL 160 (at "expected_states" sc))
              J(cellL 120 (getOr "financial_invariants" (J "") sc)); J(cellL 110 (getOr "recovery_behavior" (J "") sc)) ]))

let gScenarioCounts (d: Data) =
    let c = counter (d.S |> List.map (fun sc -> (s "scenario_id" sc).Split('-').[1]))
    table [ "Domain"; "Scenarios" ] ((c |> List.sortWith (fun (a, _) (b, _) -> cmpStr a b) |> List.map (fun (k, n) -> [ J k; N n ])) @ [ [ J "**Total**"; N d.S.Length ] ])

let gDecisions (d: Data) =
    table [ "ID"; "Type"; "Decision"; "Status"; "Decided option"; "Validation still required"; "Owner to confirm"; "Linked records" ]
        (d.D |> List.map (fun x ->
            let rec' = at "recommendation" x
            [ at "decision_id" x; at "type" x; at "question" x; getOr "status" (J "") x; J(cellL 420 (getOr "decided_option" rec' x))
              getOr "validation_required" (J "") x; at "decision_owner" x; J(cellL 120 (at "affected_ids" x)) ]))

let gCounsel (d: Data) =
    table [ "Question"; "Topic"; "Provisional working position"; "Status" ]
        (d.K |> List.map (fun q -> [ at "question_id" q; at "topic" q; at "provisional_position" q; at "status" q ]))

let private items (j: Json) = match j with JObj kvs -> kvs | j -> raise (PyError $"not a dict: {repr j}")

let gValidation (d: Data) =
    let v = d.V
    table [ "Record set / result"; "Count" ]
        ((items (at "records" v) |> List.map (fun (k, n) -> [ J k; n ]))
         @ [ [ J "trace edges"; at "trace_edges" v ]; [ J "**errors**"; at "errors" v ]; [ J "warnings"; at "warnings" v ] ])

let private joinStrs (sep: string) (j: Json) = iter j |> List.map asStr |> String.concat sep

let gUiIndex (d: Data) =
    table [ "ID"; "Kind"; "Title"; "Forma components"; "Folio elements"; "Capability gaps"; "ACs" ]
        (d.U |> List.map (fun u ->
            let gaps = joinStrs ", " (at "capability_gaps" u)
            [ at "id" u; at "kind" u; at "title" u; J(cellL 160 (J(joinStrs ", " (at "forma_components" u)))); J(cellL 120 (J(joinStrs ", " (at "folio_elements" u))))
              J(if gaps = "" then "—" else gaps); N(jlen (at "acceptance_criteria" u)) ]))

let gUiRecords (d: Data) =
    let one (u: Json) =
        let acs = iter (at "acceptance_criteria" u) |> List.map acLine |> String.concat "\n"
        let opt k (line: unit -> string) = if truthy (at k u) then [ line () ] else []
        let ticks k = iter (at k u) |> List.map (fun c -> $"`{asStr c}`") |> String.concat ", "
        [ yield $"""### {s "id" u} — {s "title" u} ({s "kind" u})"""
          yield $"""- **Requirement:** {s "statement" u}"""
          yield $"""- **Traces to:** {joinStrs ", " (at "traces" u)}"""
          yield! opt "actors" (fun () -> $"""- **Actors:** {joinStrs ", " (at "actors" u)}""")
          yield! opt "forma_components" (fun () -> $"""- **Forma components:** {ticks "forma_components"}""")
          yield! opt "folio_elements" (fun () -> $"""- **Folio elements:** {ticks "folio_elements"}""")
          yield! opt "states" (fun () -> $"""- **States:** {joinStrs "; " (at "states" u)}""")
          yield! opt "actions" (fun () -> $"""- **Actions:** {joinStrs "; " (at "actions" u)}""")
          yield! opt "responsive" (fun () -> $"""- **Responsive:** {s "responsive" u}""")
          yield! opt "capability_gaps" (fun () -> $"""- **Capability gaps:** {joinStrs ", " (at "capability_gaps" u)}""")
          yield "- **Acceptance criteria:**"
          yield acs
          yield "" ]
        |> String.concat "\n"
    d.U |> List.map one |> String.concat "\n"

let gUiGaps (d: Data) =
    table [ "Gap"; "Component"; "Owner"; "Filed as"; "Need"; "Release-1 workaround"; "Acceptance" ]
        (d.UG |> List.map (fun g ->
            [ at "gap_id" g; at "component" g; at "owner" g; getOr "filed_as" (J "") g; J(cellL 200 (at "need" g)); J(cellL 220 (at "workaround_r1" g)); J(cellL 200 (at "acceptance" g)) ]))

let gWorkflows (d: Data) =
    let rows =
        d.W
        |> List.map (fun w ->
            let tr = iter (at "transitions" w)
            let cnt f = tr |> List.filter f |> List.length |> N
            let actor t = get "actor_type" t
            [ at "workflow_id" w; at "name" w; N(jlen (at "states" w)); N tr.Length
              cnt (fun t -> pyEq (actor t) (J "human"))
              cnt (fun t -> pyEq (actor t) (J "deterministic") || pyEq (actor t) (J "timer"))
              cnt (fun t -> pyEq (actor t) (J "external"))
              cnt (fun t -> pyEq (actor t) (J "ai"))
              cnt (fun t -> truthy (get "scenario_ids" t)) ])
    table [ "Workflow"; "Name"; "States"; "Transitions"; "Human"; "Deterministic/timer"; "External"; "AI"; "With scenario" ] rows

let gIa (d: Data) =
    let key (r: Json) = [ at "workspace_id" r; at "workspace" r; at "audience" r; at "responsive" r ]
    let keys = d.IA |> List.map key |> List.distinctBy (List.map str)
    let groups = keys |> List.map (fun k -> k, d.IA |> List.filter (fun r -> List.map str (key r) = List.map str k))
    table [ "Workspace"; "Name"; "Audience"; "Responsive"; "Views and panels (surface → transitions, max authority)" ]
        (groups
         |> List.sortWith (fun (a, _) (b, _) -> cmpTuple a b)
         |> List.map (fun (k, v) ->
             k @ [ J(v |> List.map (fun r -> $"""{s "panel_or_view" r} ({s "transitions" r}, {s "max_authority" r})""") |> String.concat "; ") ]))

let UIQ_FAMILIES = [ "UNB-UI-DS-"; "UNB-UI-N-"; "UNB-UI-F-"; "UNB-UI-R-"; "UNB-UI-A-"; "UNB-UI-V-" ]

let gUiqRequirements (d: Data) =
    table [ "ID"; "Kind"; "Title"; "Traces (inbound and governing)"; "Gaps"; "ACs" ]
        (d.U
         |> List.filter (fun u -> UIQ_FAMILIES |> List.exists (fun p -> (s "id" u).StartsWith(p, StringComparison.Ordinal)))
         |> List.map (fun u ->
             let gaps = joinStrs ", " (at "capability_gaps" u)
             [ at "id" u; at "kind" u; at "title" u; J(cellL 120 (J(joinStrs ", " (at "traces" u)))); J(if gaps = "" then "—" else gaps); N(jlen (at "acceptance_criteria" u)) ]))

let private semi (j: Json) = J((asStr j).Replace(";", ", "))

let gUiqCoverage (d: Data) =
    table [ "Required scenario"; "v2 states (passing all checks)"; "Count" ] (d.UC |> List.map (fun c -> [ at "scenario" c; semi (at "covered_by" c); at "count" c ]))

let gUiqStates (d: Data) =
    table [ "Screen"; "State"; "Title"; "Persona"; "Scenarios"; "Verified" ]
        (d.UM |> List.filter (fun r -> s "version" r = "v2")
         |> List.map (fun r -> [ at "screen" r; at "state_id" r; at "state_title" r; at "persona" r; semi (at "scenario_tags" r); at "verification" r ]))

let gUiqTrace (d: Data) =
    table [ "UIR"; "Refined requirement"; "Defects"; "Original inbound"; "Status"; "Tested by" ]
        (d.UT |> List.map (fun t ->
            let tb = (s "tested_by" t).Replace(";", ", ")
            [ at "refinement_id" t; at "refined_requirement" t; semi (at "defects" t); J(cellL 120 (semi (at "original_inbound" t))); at "status" t
              J(if tb = "" then "—" else tb) ]))

let GENERATORS : Map<string, Data -> string> =
    Map.ofList
        [ "uiq-requirements", gUiqRequirements; "uiq-coverage", gUiqCoverage; "uiq-states", gUiqStates; "uiq-trace", gUiqTrace
          "workflows", gWorkflows; "ia", gIa
          "ui-index", gUiIndex; "ui-records", gUiRecords; "ui-gaps", gUiGaps
          "findings-matrix", gFindingsMatrix; "findings-dimension", gFindingsDimension
          "findings-severe", gFindingsSevere; "findings-other", gFindingsOther
          "conflicts", gConflicts; "gaps", gGaps; "gap-counts", gGapCounts
          "refinement-index", gRefinementIndex; "refinement-records", gRefinementRecords
          "trace-summary", gTraceSummary; "trace-coverage", gTraceCoverage
          "scenarios", gScenarios; "scenario-counts", gScenarioCounts
          "decisions", gDecisions; "counsel", gCounsel; "validation", gValidation ]

let BLOCK = rx RegexOptions.Singleline @"(<!-- BEGIN GENERATED: ([a-z-]+) -->)(.*?)(<!-- END GENERATED: \2 -->)"

let render (text: string) (d: Data) : string =
    subFn BLOCK (fun m ->
        match GENERATORS.TryFind(grp 2 m) with
        | Some g -> grp 1 m + "\n" + g d + "\n" + grp 4 m
        | None -> raise (PyError $"KeyError: {reprStr (grp 2 m)}")) text

let load (data: string) : Data =
    let p n = combine data n
    let jl n = loadJsonlText (readText (p n))
    let cv n = dictRows (readText (p n))
    let opt n f = if IO.File.Exists(p n) then f n else []
    { F = cv "findings.csv"; C = cv "conflicts.csv"; S = cv "scenarios.csv"; D = cv "decisions.csv"; E = cv "trace-edges.csv"
      Q = cv "requirements-quality.csv"; R = jl "refinements.jsonl"; G = jl "gap-requirements.jsonl"
      U = opt "ui-requirements.jsonl" jl; UG = opt "ui-component-gaps.csv" cv
      W = glob (combine (IO.Path.GetDirectoryName data) "workflows") "WFM-*.json" |> List.map Io.loadJson
      IA = opt "surface-ia.csv" cv; UM = opt "ui-state-matrix.csv" cv; UC = opt "ui-scenario-coverage.csv" cv
      UT = opt "ui-refinement-trace.csv" cv; K = opt "counsel-positions.csv" cv; V = Io.loadJson (p "validation-report.json") }

let main (argv: string list) : int =
    let root = argv.[0]
    let d = load (combine root (RECON + "/data"))
    for p in glob (combine root RECON) "*.md" |> List.filter IO.File.Exists do
        let before = readText p
        let after = render before d
        if after <> before then
            writeText p after
            Io.print $"rendered {name p}"
    0
