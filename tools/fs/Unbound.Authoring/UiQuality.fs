/// ui_quality.py: builds the report-18 UI requirement records, applies the report-18 revisions to existing records and
/// writes data/ui-requirements.jsonl and data/decisions.csv. Pure construction; the file I/O is in `main`.
module Unbound.Authoring.UiQuality

open Unbound.Py.PyJson
open Unbound.Authoring.PyCompat
open Unbound.Authoring.UiQualityData

let private strs (xs: string list) = JArr(xs |> List.map JStr)

/// ac(*items): numbered acceptance criteria.
let ac (items: (string * string * string) list) : Json =
    items
    |> List.mapi (fun i (g, w, t) -> JObj [ "id", JStr $"AC-{i + 1}"; "given", JStr g; "when", JStr w; "then", JStr t ])
    |> JArr

/// The record req() returns, keys in the script's order.
let build (s: Spec) : Json =
    JObj [ "id", JStr s.Id
           "kind", JStr s.Kind
           "title", JStr s.Title
           "statement", JStr s.Statement
           "traces", strs s.Traces
           "forma_components", strs s.Forma
           "folio_elements", strs s.Folio
           "capability_gaps", strs s.Gaps
           "actors", JArr []
           "states", JArr []
           "actions", JArr []
           "responsive", JStr ""
           "acceptance_criteria", ac s.Acs
           "detail", JObj(s.Detail |> List.map (fun (k, v) -> k, JStr v))
           "status", JStr "proposed"
           "origin", JStr origin ]

/// NEW, built.
let newRecords : Json list = newSpecs |> List.map build

/// revise_one(rec, (old, new, why), tag): replace the JSON-escaped old text everywhere in the record and append a
/// revision entry, unless a revision with this tag is already recorded.
let reviseOne (record: Json) ((old, new', why): Revision) (tag: string) : Json =
    let history = getList "revisions" record
    let done' = history |> List.exists (fun r -> field "by" r = Some(JStr tag))
    if done' then record
    else
        let text = dumpsUnicode record
        pyAssert (text.Contains(jsonBody old)) (itemStr "id" record)
        let revised = loads (text.Replace(jsonBody old, jsonBody new'))
        let entry = JObj [ "by", JStr tag; "date", JStr "2026-11-23"; "replaced", JStr old; "with", JStr new'; "reason", JStr why ]
        setField "revisions" (JArr(history @ [ entry ])) revised

/// The tag of a MORE_REVISIONS entry: "report-18-" + sha1(old)[:8].
let moreTag (old: string) : string = "report-18-" + (sha1Hex old).Substring(0, 8)

let private lookup (id: string) (table: (string * 'v) list) = table |> List.tryFind (fst >> (=) id) |> Option.map snd

/// revise(rec): REVISIONS, then EXTRA_REVISIONS, then each MORE_REVISIONS entry for the record, in order.
let revise (record: Json) : Json =
    let id = itemStr "id" record
    let r1 = match lookup id revisions with Some e -> reviseOne record e "report-18" | None -> record
    let r2 = match lookup id extraRevisions with Some e -> reviseOne r1 e "report-18b" | None -> r1
    moreRevisions
    |> List.filter (fun (rid, _, _, _) -> rid = id)
    |> List.fold (fun r (_, old, new', why) -> reviseOne r (old, new', why) (moreTag old)) r2

let private inFamilies (id: string) = families |> List.exists (fun f -> startsWith f id)

/// The new ui-requirements.jsonl records: existing records outside the report-18 families (revised), then NEW.
let uiRecords (existing: Json list) : Json list =
    let kept = existing |> List.filter (fun r -> not (inFamilies (itemStr "id" r))) |> List.map revise
    let out = kept @ newRecords
    pyAssert ((out |> List.map (item "id") |> List.distinct |> List.length) = out.Length) ""
    out

/// The new decisions.csv rows: existing rows other than RCD-050..053, then DECISIONS.
let decisionRows (rows: CsvRow list) : CsvRow list =
    let ids = decisions |> List.map (fun d -> d |> List.find (fst >> (=) "decision_id") |> snd)
    (rows |> List.filter (fun r -> not (List.contains (cellStr "decision_id" r) ids))) @ (decisions |> List.map ofPairs)

let jsonLines (records: Json list) : string = records |> List.map (fun r -> dumpsUnicode r + "\n") |> String.concat ""

let main (paths: Io.Paths) : int =
    let path = $"{paths.Data}/ui-requirements.jsonl"
    let out = uiRecords (Io.loadJsonl path)
    Io.write path (jsonLines out)
    let dpath = $"{paths.Data}/decisions.csv"
    let rows0 = Io.loadCsv dpath
    let fields = fieldsOf rows0
    let rows = decisionRows rows0
    Io.write dpath (writeDicts fields rows)
    Io.print $"ui requirements: {out.Length} ({newRecords.Length} new); decisions: {rows.Length}"
    0
