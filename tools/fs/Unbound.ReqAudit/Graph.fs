/// graph.py: derive the bidirectional traceability graph from reconciliation records. Edges are computed, never
/// hand-maintained.
module Unbound.ReqAudit.Graph

open System
open System.Numerics
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

/// source, relationship, target, provenance
type Edge = string * string * string * string

let ID = re @"\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-[A-Z]?\d{1,4}(?:-r\d+)?\*?\b|\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-\*"

let RANGE = re @"\b([A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-)([A-Z]?)(\d{1,4})\.\.(?:\1)?\2?(\d{1,4})\b"

/// UNB-SUCC-001..014 -> UNB-SUCC-001 UNB-SUCC-002 ... (zero padding preserved).
let expandRanges (text: string) : string =
    let rep (m: Text.RegularExpressions.Match) =
        let width = len (grp 3 m)
        let lo, hi = pyInt (grp 3 m), pyInt (grp 4 m)
        if hi < lo then ""
        else
            Seq.unfold (fun (n: BigInteger) -> if n > hi then None else Some(n, n + BigInteger.One)) lo
            |> Seq.map (fun n -> grp 1 m + grp 2 m + n.ToString().PadLeft(width, '0'))
            |> String.concat " "
    subFn RANGE rep (if isNull text then "" else text)

let idsIn (text: string) : string list = finditer ID (expandRanges text) |> List.map (fun m -> m.Value) |> List.distinct

/// ids_in(x) for a record field: `x or ""` must be a str.
let idsOf (j: Json) : string list = idsIn (asStr (orElse (JStr "") j))

let private strs (j: Json) : string list = iter j |> List.map asStr

let refinementEdges (recs: Json list) : Edge list =
    recs
    |> List.collect (fun r ->
        let rid = s "refinement_id" r
        [ yield rid, str (getOr "relationship" (JStr "refines") r), s "original_id" r, "refinements.jsonl"
          yield! strs (getOr "related_findings" (JArr []) r) |> List.map (fun f -> rid, "derived_from", f, "refinements.jsonl")
          yield!
              strs (getOr "dependencies" (JArr []) r)
              |> List.filter (isFull ID)
              |> List.map (fun d -> rid, "depends_on", d, "refinements.jsonl") ])

let gapEdges (recs: Json list) : Edge list =
    recs
    |> List.collect (fun g ->
        let gid = s "id" g
        [ yield gid, "derived_from", s "gap_finding" g, "gap-requirements.jsonl"
          yield! idsOf (getOr "parent" (JStr "") g) |> List.map (fun p -> gid, "derived_from", p, "gap-requirements.jsonl")
          yield!
              strs (getOr "links" (JArr []) g)
              |> List.filter (isFull ID)
              |> List.map (fun l -> gid, "depends_on", l, "gap-requirements.jsonl") ])

let findingEdges (recs: Json list) : Edge list =
    recs
    |> List.collect (fun f ->
        let fid = s "finding_id" f
        (idsOf (getOr "affected_ids" (JStr "") f) |> List.map (fun a -> fid, "affects", a, "findings.csv"))
        @ (idsOf (getOr "prior_ids" (JStr "") f) |> List.map (fun p -> fid, "derived_from", p, "findings.csv")))

let conflictEdges (recs: Json list) : Edge list =
    recs
    |> List.collect (fun c ->
        let a, b = idsOf (at "requirement_a" c), idsOf (at "requirement_b" c)
        [ for x in a do
              for y in b do
                  if x <> y then yield x, "conflicts_with", y, s "conflict_id" c ])

let duplicateEdges (recs: Json list) : Edge list =
    recs
    |> List.collect (fun f ->
        if (lower (asStr (getOr "title" (JStr "") f))).StartsWith("duplicate", StringComparison.Ordinal) then
            let ids = idsOf (at "affected_ids" f)
            ids |> List.skip (min 1 ids.Length) |> List.map (fun i -> i, "duplicates", ids.Head, s "finding_id" f)
        else [])

let scenarioEdges (recs: Json list) : Edge list =
    recs
    |> List.collect (fun sc -> idsOf (getOr "requirement_ids" (JStr "") sc) |> List.map (fun r -> r, "tested_by", s "scenario_id" sc, "scenarios.csv"))

/// A scenario that cites an original requirement also validates its refinement(s).
let inheritedTestEdges (refs: Json list) (scen: Json list) : Edge list =
    let byOrig =
        refs
        |> List.fold
            (fun (m: Map<string, string list>) r ->
                let o = s "original_id" r
                m.Add(o, (m.TryFind o |> Option.defaultValue []) @ [ s "refinement_id" r ]))
            Map.empty
    scen
    |> List.collect (fun sc ->
        idsOf (getOr "requirement_ids" (JStr "") sc)
        |> List.collect (fun rid ->
            byOrig.TryFind rid |> Option.defaultValue [] |> List.map (fun rr -> rr, "tested_by", s "scenario_id" sc, "scenarios.csv (via original)")))

let uiEdges (recs: Json list) : Edge list =
    recs
    |> List.collect (fun u ->
        let uid = s "id" u
        (strs (getOr "traces" (JArr []) u) |> List.filter (isFull ID) |> List.map (fun t -> uid, "derived_from", t, "ui-requirements.jsonl"))
        @ (strs (getOr "capability_gaps" (JArr []) u) |> List.map (fun g -> uid, "depends_on", g, "ui-requirements.jsonl")))

let decisionEdges (recs: Json list) : Edge list =
    recs
    |> List.collect (fun d -> idsOf (getOr "affected_ids" (JStr "") d) |> List.map (fun x -> x, "decided_by", s "decision_id" d, "decisions.csv"))

type Inputs =
    { Refinements: Json list
      Gaps: Json list
      Findings: Json list
      Conflicts: Json list
      Scenarios: Json list
      Decisions: Json list
      Ui: Json list }

let build (d: Inputs) : Edge list =
    [ yield! refinementEdges d.Refinements
      yield! gapEdges d.Gaps
      yield! findingEdges d.Findings
      yield! duplicateEdges d.Findings
      yield! conflictEdges d.Conflicts
      yield! scenarioEdges d.Scenarios
      yield! inheritedTestEdges d.Refinements d.Scenarios
      yield! decisionEdges d.Decisions
      yield! uiEdges d.Ui ]
    |> List.distinct

let edgesCsv (edges: Edge list) : string =
    PyCsv.format PyCsv.QuoteMinimal ([ "source"; "relationship"; "target"; "provenance" ] :: (edges |> List.map (fun (a, b, c, d) -> [ a; b; c; d ])))

let summary (edges: Edge list) : Json =
    JObj(counter (edges |> List.map (fun (_, r, _, _) -> r)) |> List.map (fun (k, n) -> k, jint n))

let main (argv: string list) : int =
    let data = argv.[0]
    let p n = combine data n
    let edges =
        build
            { Refinements = Io.loadJsonl (p "refinements.jsonl")
              Gaps = Io.loadJsonl (p "gap-requirements.jsonl")
              Findings = Io.loadCsv (p "findings.csv")
              Conflicts = Io.loadCsv (p "conflicts.csv")
              Scenarios = Io.loadCsv (p "scenarios.csv")
              Decisions = Io.loadCsv (p "decisions.csv")
              Ui = Io.loadJsonl (p "ui-requirements.jsonl") }
    writeText (p "trace-edges.csv") (edgesCsv edges)
    Io.print (Io.dumps2 (summary edges))
    0
