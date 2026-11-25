/// validate.py: quality gates for the requirements reconciliation records (mission section 16). Each gate is a pure
/// function returning Issue values; main aggregates them, prints a report and exits non-zero on any ERROR.
module Unbound.ReqAudit.Validate

open System
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

let RECON = "requirements/reconciliation/2026-11-23"
let BASELINE_REV = "777a70d"

let REL_VOCAB =
    set [ "refines"; "supersedes"; "conflicts_with"; "duplicates"; "depends_on"; "derived_from"; "validated_by"
          "implemented_by"; "tested_by"; "affects"; "decided_by" ]

let NEW_ID =
    re @"^(UNB-UI-(?:[GSDWPNFRAV]|DS)-\d{3}|RR-.+-r\d+|UNB-RCN-[A-Z]+-\d{3}|RCF-[A-Z]+-\d{3}|RCC-[A-Z]+-\d{3}|RCD-\d{3}|DKS-[A-Z]+-\d{3})$"

let CITED =
    re @"\b(UNB-UI-(?:[GSDWPNFRAV]|DS)-\d{3}|RR-[A-Z0-9-]+?-r\d+|UNB-RCN-[A-Z]+-\d{3}|RCF-[A-Z]+-\d{3}|RCC-[A-Z]+-\d{3}|RCD-\d{3}|DKS-[A-Z]+-\d{3})\b"

let ANY_ID = re @"^[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-(?:[A-Z]?\d{1,4}[A-Za-z]?(?:-Q)?|\*)$"

let REQUIRED =
    Map.ofList
        [ "refinements",
          [ "refinement_id"; "original_id"; "original_path"; "original_line"; "original_revision"; "original_text"
            "relationship"; "statement"; "business_objective"; "actors"; "processing_rules"; "failure_behavior"
            "security"; "auditability"; "acceptance_criteria"; "reason_for_change"; "evidence"; "confidence"
            "validation_status"; "decision_status" ]
          "gaps",
          [ "id"; "origin"; "parent"; "gap_finding"; "why_no_predecessor"; "statement"; "rationale"; "evidence"
            "acceptance_criteria"; "links"; "priority" ]
          "findings",
          [ "finding_id"; "severity"; "dimension"; "affected_ids"; "evidence"; "title"; "description"; "proposed_resolution" ]
          "conflicts", [ "conflict_id"; "requirement_a"; "requirement_b"; "incompatibility"; "recommended_resolution" ]
          "scenarios", [ "scenario_id"; "title"; "requirement_ids"; "stimulus"; "expected_states" ]
          "decisions", [ "decision_id"; "question"; "affected_ids"; "decision_owner"; "recommendation"; "status" ]
          "ui", [ "id"; "kind"; "title"; "statement"; "traces"; "acceptance_criteria" ] ]

type Issue = { Level: string; Gate: string; Subject: string; Message: string }

let issue level gate subject message = { Level = level; Gate = gate; Subject = subject; Message = message }
let err gate subject message = issue "ERROR" gate subject message

let private normDash = re @"[*_`]|^\s*-\s*"
let private ws = re @"\s+"

let normalize (t: string) : string = lower (strip (subLit ws " " (subLit normDash "" (if isNull t then "" else t))))

/// normalize(x) for a record value: `x or ""` must be a str.
let normalizeJ (j: Json) = normalize (asStr (orElse (JStr "") j))

let private family = re @"-[A-Z]?\d{1,4}[A-Za-z]?(?:-Q)?$"

/// resolver(baseline, new, narr, known_undefined): does a reference resolve?
let resolver (baseline: Set<string>) (newIds: Set<string>) (narr: Set<string>) (knownUndefined: Set<string>) : string -> bool =
    let families = Set.union baseline newIds |> Set.map (subLit family "")
    fun (ref: string) ->
        let ref = (strip ref).TrimEnd('.')
        if baseline.Contains ref || newIds.Contains ref || narr.Contains ref || knownUndefined.Contains ref then true
        elif ref.EndsWith("-*", StringComparison.Ordinal) then families.Contains(ref.Substring(0, ref.Length - 2))
        elif ref.Contains '@' then baseline.Contains(ref.Split('@').[0])
        else false

/// Python frozenset iteration order is hash-randomised; issues drawn from a set are emitted in sorted order.
let private sortedSet (xs: Set<string>) = sortStrings xs

let gDupes (sets: (string * string list) list) : Issue list =
    counter (sets |> List.collect snd)
    |> List.filter (fun (_, n) -> n > 1)
    |> List.map (fun (i, n) -> err "G1" i $"identifier defined {n} times across reconciliation records")

let gCollide (newIds: Set<string>) (baseline: Set<string>) : Issue list =
    (sortedSet (Set.intersect newIds baseline) |> List.map (fun i -> err "G2" i "new identifier collides with a baseline identifier"))
    @ (sortedSet newIds |> List.filter (fun i -> not (isMatch NEW_ID i))
       |> List.map (fun i -> err "G2" i "new identifier does not follow a reconciliation ID scheme"))

/// The baseline file text for each distinct original_path (git show at BASELINE_REV; "" when absent).
type BaselineFiles = string -> string

let gRefinements (baselineFile: BaselineFiles) (refs: Json list) (baseline: Set<string>) : Issue list =
    refs
    |> List.collect (fun r ->
        let rid = str (getOr "refinement_id" (JStr "?") r)
        let oid = get "original_id" r
        let src = normalize (baselineFile (str (getOr "original_path" (JStr "") r)))
        [ if not (match oid with JStr o -> baseline.Contains o | _ -> false) then
              yield err "G3" rid $"original_id {str oid} not in baseline inventory"
          if not (rid.StartsWith($"RR-{str oid}-r", StringComparison.Ordinal)) then
              yield err "G3" rid "refinement_id must be RR-<original_id>-r<n>"
          if src = "" then
              yield err "G4" rid $"""original_path {str (get "original_path" r)} not found at {BASELINE_REV}"""
          elif not (src.Contains(take 160 (normalizeJ (getOr "original_text" (JStr "") r)), StringComparison.Ordinal)) then
              yield err "G4" rid "original_text not found verbatim (first 160 normalized chars) in original file"
          match get "relationship" r with
          | JStr("refines" | "supersedes") -> ()
          | rel -> yield err "G9" rid $"relationship {str rel} invalid for a refinement" ])

let empty (v: Json) =
    match v with
    | JNull -> true
    | JStr "" | JArr [] | JObj [] -> true
    | _ -> false

let gRequired (name: string) (recs: Json list) (key: string) : Issue list =
    recs
    |> List.collect (fun r ->
        REQUIRED.[name]
        |> List.filter (fun f -> empty (get f r))
        |> List.map (fun f -> err "G5" (str (getOr key (JStr "?") r)) $"{name}: missing or empty field '{f}'"))

let gAcceptance (recs: Json list) (key: string) : Issue list =
    recs
    |> List.collect (fun r ->
        iter (getOr "acceptance_criteria" (JArr []) r)
        |> List.filter (fun a ->
            not (match a with
                 | JObj _ -> [ "given"; "when"; "then" ] |> List.forall (fun k -> truthy (get k a))
                 | _ -> false))
        |> List.map (fun a -> err "G6" (str (getOr key (JStr "?") r)) $"acceptance criterion {take 60 (repr a)} lacks given/when/then"))

let gGaps (gaps: Json list) (findingIds: Set<string>) : Issue list =
    gaps
    |> List.collect (fun g ->
        [ if not (pyEq (get "origin" g) (JStr "gap-analysis")) then yield err "G7" (s "id" g) "origin must be gap-analysis"
          match get "gap_finding" g with
          | JStr f when findingIds.Contains f -> ()
          | f -> yield err "G7" (s "id" g) $"gap_finding {str f} not in findings register" ])

let gRefs (label: string) (subject: string) (refs: string list) (ok: string -> bool) : Issue list =
    refs
    |> List.filter (fun r -> strip r <> "" && isMatch ANY_ID (strip r) && not (ok r))
    |> List.map (fun r -> err "G8" subject $"{label}: unresolved reference '{r}'")

/// Elements of a JSON list used as reference strings.
let strList (j: Json) : string list = iter j |> List.map asStr

let gEdges (edges: Json list) (ok: string -> bool) : Issue list =
    edges
    |> List.collect (fun e ->
        let subj = $"""{s "source" e}->{s "target" e}"""
        [ if not (REL_VOCAB.Contains(s "relationship" e)) then
              yield err "G9" subj $"""relationship '{s "relationship" e}' not in vocabulary"""
          if not (ok (s "source" e) && ok (s "target" e)) then
              yield err "G9" subj $"""unresolved endpoint ({s "provenance" e})""" ])

/// (document name, text) for each Markdown report.
let gReports (docs: (string * string) list) (ok: string -> bool) : Issue list =
    docs
    |> List.collect (fun (name, text) ->
        findall CITED text |> Set.ofList |> sortStrings |> List.filter (fun m -> not (ok m))
        |> List.map (fun m -> err "G10" name $"cites unknown identifier {m}"))

let gUi (ui: Json list) (forma: Set<string>) (folio: Set<string>) (gaps: Set<string>) : Issue list =
    ui
    |> List.collect (fun u ->
        let uid = s "id" u
        let notIn (set: Set<string>) (c: Json) = match c with JStr x -> not (set.Contains x) | _ -> true
        (iter (getOr "forma_components" (JArr []) u) |> List.filter (notIn forma)
         |> List.map (fun c -> err "G12" uid $"Forma component '{str c}' not in catalog snapshot"))
        @ (iter (getOr "folio_elements" (JArr []) u) |> List.filter (notIn folio)
           |> List.map (fun c -> err "G12" uid $"Folio element '{str c}' not in element snapshot"))
        @ (iter (getOr "capability_gaps" (JArr []) u) |> List.filter (notIn gaps)
           |> List.map (fun g -> err "G12" uid $"capability gap '{str g}' not registered")))

let testedSources (edges: Json list) : Set<string> =
    edges |> List.filter (fun e -> s "relationship" e = "tested_by") |> List.map (s "source") |> Set.ofList

let gTested (newReqs: string list) (edges: Json list) : Issue list =
    let tested = testedSources edges
    newReqs |> List.filter (fun r -> not (tested.Contains r)) |> List.map (fun r -> issue "WARN" "G11" r "no tested_by scenario")

let formatIssue (i: Issue) = $"{ljust 5 i.Level} {ljust 4 i.Gate} {i.Subject}: {i.Message}"

/// The summary written to validation-report.json and printed.
let summaryJson (records: (string * int) list) (extra: (string * Json) list) (issues: Issue list) : Json =
    let errors = issues |> List.filter (fun i -> i.Level = "ERROR")
    JObj(
        [ "records", JObj(records |> List.map (fun (k, n) -> k, jint n)) ]
        @ extra
        @ [ "errors", jint errors.Length
            "warnings", jint (issues.Length - errors.Length)
            "errors_by_gate", JObj(counter (errors |> List.map (fun i -> i.Gate)) |> List.map (fun (k, n) -> k, jint n)) ]
    )

/// Prints the issues and summary, writes the report, returns the exit code.
let report (reportPath: string) (records: (string * int) list) (extra: (string * Json) list) (issues: Issue list) : int =
    issues |> List.iter (formatIssue >> Io.print)
    let summary = summaryJson records extra issues
    Io.writeJsonReport reportPath summary
    Io.print (Io.dumps2 summary)
    if issues |> List.exists (fun i -> i.Level = "ERROR") then 1 else 0

/// `git show BASELINE_REV:path` (text=True: universal newlines) read once for each distinct original_path of the
/// refinements; "" when git fails, as baseline_file returns.
let baselineFiles (root: string) (refs: Json list) : BaselineFiles =
    let texts =
        refs
        |> List.map (fun r -> str (getOr "original_path" (JStr "") r))
        |> List.distinct
        |> List.map (fun p -> p, (let r = Io.gitShow root BASELINE_REV p in if r.Code = 0 then r.Out else ""))
        |> Map.ofList
    fun path -> texts.TryFind path |> Option.defaultWith (fun () -> let r = Io.gitShow root BASELINE_REV path in if r.Code = 0 then r.Out else "")

let ids (key: string) (recs: Json list) = recs |> List.map (s key)

let column (key: string) (recs: Json list) = recs |> List.map (s key) |> Set.ofList

let main (argv: string list) : int =
    let root = resolve argv.[0]
    let data = combine root (RECON + "/data")
    let d n = combine data n
    let inv = Io.loadCsv (d "inventory.csv")
    let baseline = column "identifier" inv
    let workflowIds =
        glob (combine root (RECON + "/workflows")) "WFM-*.json"
        |> List.collect (fun p ->
            let w = Io.loadJson p
            s "workflow_id" w :: (iter (at "transitions" w) |> List.map (s "id")))
    let narr =
        Set.unionMany
            [ column "surrogate_id" (Io.loadCsv (d "narrative-requirements.csv"))
              column "capability_id" (Io.loadCsv (d "competitive-rollup.csv"))
              column "obligation_id" (Io.loadCsv (d "regulatory-obligations.csv"))
              column "identifier" (Io.loadCsv (d "withdrawn-ids.csv"))
              column "new_id" (Io.loadCsv (d "renamed-ids.csv"))
              column "gap_id" (Io.loadCsv (d "ui-component-gaps.csv"))
              Set.ofList workflowIds ]
    let refs = Io.loadJsonl (d "refinements.jsonl")
    let gaps = Io.loadJsonl (d "gap-requirements.jsonl")
    let finds, confs = Io.loadCsv (d "findings.csv"), Io.loadCsv (d "conflicts.csv")
    let scens, decs = Io.loadCsv (d "scenarios.csv"), Io.loadCsv (d "decisions.csv")
    let edges = Io.loadCsv (d "trace-edges.csv")
    let ui = Io.loadJsonl (d "ui-requirements.jsonl")
    let uigaps = column "gap_id" (Io.loadCsv (d "ui-component-gaps.csv"))
    let sets =
        [ "refinements", ids "refinement_id" refs
          "gaps", ids "id" gaps
          "findings", ids "finding_id" finds
          "conflicts", ids "conflict_id" confs
          "scenarios", ids "scenario_id" scens
          "decisions", ids "decision_id" decs
          "ui", ids "id" ui ]
    let setOf k = sets |> List.find (fst >> (=) k) |> snd
    let newIds = sets |> List.collect snd |> Set.ofList
    let knownUndefined = column "identifier" (Io.loadCsv (d "undefined-references.csv"))
    let ok = resolver baseline newIds narr knownUndefined
    let strict = resolver baseline newIds narr Set.empty
    let split (j: Json) = Graph.idsOf j
    let docs = glob (combine root RECON) "*.md" |> List.filter IO.File.Exists |> List.map (fun p -> name p, readText p)
    let issues =
        [ yield! gDupes sets
          yield! gCollide newIds baseline
          yield! gRefinements (baselineFiles root refs) refs baseline
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
          for r in refs do yield! gRefs "related_findings" (s "refinement_id" r) (strList (getOr "related_findings" (JArr []) r)) ok
          for g in gaps do yield! gRefs "links" (s "id" g) (strList (getOr "links" (JArr []) g)) ok
          yield! gEdges edges ok
          yield! gReports docs ok
          yield! gTested (setOf "refinements" @ setOf "gaps") edges
          yield! gRequired "ui" ui "id"
          yield! gAcceptance ui "id"
          yield! gUi ui (column "component_id" (Io.loadCsv (d "forma-catalog.csv"))) (column "element" (Io.loadCsv (d "folio-elements.csv"))) uigaps
          for u in ui do yield! gRefs "traces" (s "id" u) (strList (getOr "traces" (JArr []) u)) ok
          for w in Io.loadCsv (d "withdrawn-ids.csv") do
              if newIds.Contains(s "identifier" w) then yield err "G1" (s "identifier" w) "withdrawn id is still used by a live record"
          for e in edges do
              if ok (s "target" e) && not (strict (s "target" e)) then
                  yield issue "WARN" "G8u" (s "source" e) $"""references {s "target" e}, which the baseline cites but never defines""" ]
    report (d "validation-report.json") (sets |> List.map (fun (k, v) -> k, v.Length)) [ "trace_edges", jint edges.Length ] issues
