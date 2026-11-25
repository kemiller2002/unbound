/// ui_trace.py: report-18 data: the v1 defect register with resolutions, the UI state matrix, scenario coverage, UI
/// refinement traceability records (UIR-nnn) and the DKS-UI scenarios. Pure builders; the file I/O is in `main`.
module Unbound.Authoring.UiTrace

open Unbound.Py
open Unbound.Py.PyJson
open Unbound.Authoring.PyCompat
open Unbound.Authoring.UiTraceData

/// The loaded inputs the builders read.
type Inputs =
    { /// ui_index(): UI requirement records by id (last record wins).
      Ui: Map<string, Json>
      /// baseline_ids(): inventory identifiers.
      Base: Set<string>
      /// links(): refinement id -> [original id].
      Refinements: Map<string, string list>
      /// links(): gap requirement id -> parent ids.
      GapParents: Map<string, string list> }

let private strOf (j: Json) : string =
    match j with
    | JStr s -> s
    | v -> raise (PyError $"TypeError: expected str, got {v}")

let uiIndex (records: Json list) : Map<string, Json> =
    records |> List.fold (fun (m: Map<string, Json>) r -> m.Add(itemStr "id" r, r)) Map.empty

/// The parent ids of a gap requirement: str(g.get("parent", "")) split on "," and ";", stripped, empty parts dropped.
let parents (g: Json) : string list =
    let text =
        match field "parent" g with
        | None -> ""
        | Some(JStr s) -> s
        | Some v -> raise (PyError $"unsupported parent value {v}")
    text.Replace(",", ";").Split(';') |> List.ofArray |> List.map strip |> List.filter ((<>) "")

/// links().
let links (refinements: Json list) (gaps: Json list) : Map<string, string list> * Map<string, string list> =
    let rr = refinements |> List.fold (fun (m: Map<string, string list>) r -> m.Add(itemStr "refinement_id" r, [ itemStr "original_id" r ])) Map.empty
    let gp = gaps |> List.fold (fun (m: Map<string, string list>) g -> m.Add(itemStr "id" g, parents g)) Map.empty
    rr, gp

/// originals_of(req_ids, ui, base, depth=3): baseline identifiers reachable through UI traces, refinement originals
/// and gap parents, in breadth-first discovery order.
let originalsOf (inp: Inputs) (reqIds: string list) : string list =
    let step (found: string list, seen: Set<string>, frontier: string list) _ =
        let found', seen', next =
            frontier
            |> List.fold
                (fun (found: string list, seen: Set<string>, next: string list) i ->
                    if seen.Contains i then found, seen, next
                    else
                        let seen = seen.Add i
                        if inp.Base.Contains i then i :: found, seen, next
                        else
                            match inp.Ui.TryFind i with
                            | Some r -> found, seen, List.rev (itemList "traces" r |> List.map strOf) @ next
                            | None ->
                                match inp.Refinements.TryFind i, inp.GapParents.TryFind i with
                                | Some o, _ -> found, seen, List.rev o @ next
                                | None, Some p -> found, seen, List.rev p @ next
                                | None, None -> found, seen, next)
                (found, seen, [])
        found', seen', List.rev next
    let found, _, _ = [ 0..3 ] |> List.fold step ([], Set.empty, reqIds)
    List.rev found

/// defects(ui): each v1 defect with its resolution status, refining requirements and v2 evidence.
let defects (ui: Map<string, Json>) (v1: CsvRow list) : CsvRow list =
    v1
    |> List.map (fun r ->
        let id = cellStr "defect_id" r
        let st, reqs, evid =
            match resolution |> List.tryFind (fst >> (=) id) with
            | Some(_, v) -> v
            | None -> raise (PyError $"KeyError: {reprStr id}")
        pyAssert (reqs.Split(';') |> Array.forall ui.ContainsKey) id
        r |> setKey "resolution_status" (Some st) |> setKey "refined_by" (Some reqs) |> setKey "v2_evidence" (Some evid))

let private alias (t: string) = aliases |> List.tryFind (fst >> (=) t) |> Option.map snd |> Option.defaultValue t

/// state_matrix(): v2 states from pages.json and verification.json, then the v1 states.
let stateMatrix (pages: Json list) (verification: Json) (v1: CsvRow list) : CsvRow list =
    let ver = itemList "pages" verification |> List.fold (fun (m: Map<string, Json>) p -> m.Add(itemStr "file" p, p)) Map.empty
    let v2Rows =
        pages
        |> List.map (fun p ->
            let tags = (itemList "scenarios" p |> List.map (strOf >> alias)) @ [ "mobile" ] |> List.distinct |> PyText.sortStrings
            let v = ver.TryFind(itemStr "file" p) |> Option.defaultValue (JObj [])
            let screen, state = itemStr "screen" p, itemStr "state" p
            ofPairs
                [ "version", "v2"
                  "screen", screen
                  "requirement", itemStr "requirement" p
                  "state_id", state
                  "state_title", itemStr "title" p
                  "persona", itemStr "persona" p
                  "scenario_tags", tags |> List.filter (fun t -> List.contains t scenarios) |> String.concat ";"
                  "other_tags", tags |> List.filter (fun t -> not (List.contains t scenarios)) |> String.concat ";"
                  "page", $"""requirements/ui/designs/v2/{itemStr "file" p}"""
                  "reference_images", $"{REL}/{screen}/{state}-390.png;{REL}/{screen}/{state}-1280.png"
                  "viewports_verified", getList "results" v |> List.map (itemStr "viewport") |> String.concat ";"
                  "verification",
                  (if field "pass" v |> Option.map truthy |> Option.defaultValue false then "pass"
                   elif truthy v then "fail"
                   else "not-run") ])
    let pageScreens = pages |> List.map (itemStr "screen") |> Set.ofList
    let v1Rows =
        v1
        |> List.map (fun r ->
            let screen = cellStr "screen" r
            [ "version", Some "v1"
              "screen", Some screen
              "requirement", Some(if not (startsWith "W" screen) then $"UNB-UI-{screen}" else "UNB-UI-W-001")
              "state_id", cell "state_id" r
              "state_title", cell "state_title" r
              "persona", cell "persona" r
              "scenario_tags", cell "scenario_tags" r
              "other_tags", Some ""
              "page", Some $"requirements/ui/designs/screens/{screen}"
              "reference_images", Some "requirements/ui/designs/rendered/"
              "viewports_verified", cell "viewport_checked" r
              "verification", Some(if pageScreens.Contains screen then "superseded-by-v2" else "v1-only") ])
    v2Rows @ v1Rows

/// coverage(matrix): for each required scenario, the passing v2 states that cover it.
let coverage (matrix: CsvRow list) : CsvRow list =
    let v2 = matrix |> List.filter (fun r -> cellStr "version" r = "v2" && cellStr "verification" r = "pass")
    scenarios
    |> List.map (fun s ->
        let covering = v2 |> List.filter (fun r -> (cellStr "scenario_tags" r).Split(';') |> Array.contains s)
        ofPairs
            [ "scenario", s
              "covered_by", covering |> List.map (fun r -> $"""{cellStr "screen" r}:{cellStr "state_id" r}""") |> String.concat ";"
              "count", string covering.Length ])

/// trace_records(defs, ui, base): one UIR record per refining requirement, in requirement-id order.
let traceRecords (inp: Inputs) (defs: CsvRow list) : CsvRow list =
    let byReq =
        defs
        |> List.fold
            (fun (acc: (string * CsvRow list) list) d ->
                (cellStr "refined_by" d).Split(';')
                |> Array.fold (fun acc r -> if acc |> List.exists (fst >> (=) r) then acc |> List.map (fun (k, ds) -> if k = r then k, ds @ [ d ] else k, ds) else acc @ [ r, [ d ] ]) acc)
            []
    byReq
    |> List.sortWith (fun (a, _) (b, _) -> PyJson.compareCodePoints a b)
    |> List.mapi (fun i (req, ds) ->
        let origs = originalsOf inp [ req ]
        let resolved = ds |> List.filter (fun d -> cellStr "resolution_status" d <> OPEN)
        ofPairs
            [ "refinement_id", sprintf "UIR-%03d" (i + 1)
              "refined_requirement", req
              "title", itemStr "title" inp.Ui.[req]
              "defects", ds |> List.map (cellStr "defect_id") |> String.concat ";"
              "original_inbound", String.concat ";" origs
              "origin", (if origs.IsEmpty then "gap-analysis (owner addendum 2026-11-23; see report 18 §3)" else "refines-original")
              "status", (if resolved.Length = ds.Length then "resolved-in-v2" else "partly-open")
              "evidence",
              resolved
              |> List.map (fun d -> (cellStr "v2_evidence" d).Split(';').[0])
              |> List.distinct
              |> PyText.sortStrings
              |> String.concat " | "
              |> take 600
              "tested_by", "" ])

/// dks(ui): the DKS-UI scenario rows, every requirement id resolved.
let dks (ui: Map<string, Json>) : CsvRow list =
    let rows = dksRows |> List.map ofPairs
    pyAssert (rows |> List.forall (fun r -> (cellStr "requirement_ids" r).Split(';') |> Array.forall ui.ContainsKey)) "unresolved requirement"
    rows

/// The trace records with tested_by filled from the scenarios that name each requirement.
let withTests (scen: CsvRow list) (trace: CsvRow list) : CsvRow list =
    let tested (req: string) =
        scen |> List.filter (fun s -> (cellStr "requirement_ids" s).Split(';') |> Array.contains req) |> List.map (cellStr "scenario_id")
    trace |> List.map (fun t -> t |> setKey "tested_by" (Some(String.concat ";" (tested (cellStr "refined_requirement" t)))))

/// scenarios.csv without the old DKS-UI rows, then the new ones.
let scenarioRows (existing: CsvRow list) (scen: CsvRow list) : CsvRow list =
    (existing |> List.filter (fun r -> not (startsWith "DKS-UI-" (cellStr "scenario_id" r)))) @ scen

let private keysOf (rows: CsvRow list) = fieldsOf rows

let main (paths: Io.Paths) : int =
    let data = paths.Data
    let ui = uiIndex (Io.loadJsonlStrict $"{data}/ui-requirements.jsonl")
    let bas = Io.loadCsvRaw $"{data}/inventory.csv" |> List.map (cellStr "identifier") |> Set.ofList
    let defs = defects ui (Io.loadCsvRaw $"{paths.Specs}/ui-defects-v1.csv")
    Io.write $"{data}/ui-defects.csv" (writeDicts (keysOf defs) defs)
    let pages = list (Io.loadJson $"{paths.V2}/pages.json")
    let matrix = stateMatrix pages (Io.loadJson $"{paths.V2}/verification.json") (Io.loadCsvRaw $"{paths.Specs}/state-matrix-v1.csv")
    Io.write $"{data}/ui-state-matrix.csv" (writeDicts (keysOf matrix) matrix)
    let cov = coverage matrix
    Io.write $"{data}/ui-scenario-coverage.csv" (writeDicts [ "scenario"; "covered_by"; "count" ] cov)
    let scen = dks ui
    let rr, gp = links (Io.loadJsonlStrict $"{data}/refinements.jsonl") (Io.loadJsonlStrict $"{data}/gap-requirements.jsonl")
    let inp = { Ui = ui; Base = bas; Refinements = rr; GapParents = gp }
    let trace = traceRecords inp defs |> withTests scen
    Io.write $"{data}/ui-refinement-trace.csv" (writeDicts (keysOf trace) trace)
    let sc = Io.loadCsvRaw $"{data}/scenarios.csv"
    let fields = fieldsOf sc
    Io.write $"{data}/scenarios.csv" (writeDicts fields (scenarioRows sc scen))
    let resolvedCount = defs |> List.filter (fun d -> cellStr "resolution_status" d <> OPEN) |> List.length
    let uncovered = cov |> List.filter (fun c -> cellStr "count" c = "0") |> List.map (cellStr "scenario")
    Io.print (
        $"defects {defs.Length} ({resolvedCount} resolved/prevented); states {matrix.Length}; "
        + $"uncovered scenarios {reprList uncovered}; UIR {trace.Length}; DKS-UI {scen.Length}"
    )
    0
