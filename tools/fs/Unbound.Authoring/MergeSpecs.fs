/// merge_specs.py: merges the panel and view specs (specs/) into data/ui-requirements.jsonl and registers their
/// PROPOSED component gaps in data/ui-component-gaps.csv. Idempotent. Pure merging; the file I/O is in `main`.
module Unbound.Authoring.MergeSpecs

open Unbound.Py.PyJson
open Unbound.Authoring.PyCompat
open Unbound.Authoring.MergeSpecsData

let private lookup (k: string) (table: (string * string) list) = table |> List.tryFind (fst >> (=) k) |> Option.map snd

/// remap(v): renumber UNB-UI-S-099 to UNB-UI-S-061 in every string; list items that are PROPOSED gap names become the
/// registered gap ids.
let rec remap (v: Json) : Json =
    match v with
    | JStr s ->
        match lookup s renum with
        | Some r -> JStr r
        | None -> JStr(s.Replace("UNB-UI-S-099", "UNB-UI-S-061"))
    | JArr xs ->
        xs
        |> List.map (fun x ->
            match x with
            | JStr s -> (match lookup s gapMap with Some g -> JStr g | None -> remap x)
            | _ -> remap x)
        |> JArr
    | JObj kvs -> JObj(kvs |> List.map (fun (k, x) -> k, remap x))
    | _ -> v

/// The spec records as merged: remapped, capability_gaps de-duplicated in first-seen order.
let specRecords (raw: Json list) : Json list =
    raw |> List.map remap |> List.map (fun r -> setField "capability_gaps" (JArr(itemList "capability_gaps" r |> List.distinct)) r)

/// Existing records not replaced by a spec, then the specs.
let merged (existing: Json list) (specs: Json list) : Json list =
    let ids = specs |> List.map (item "id")
    let out = (existing |> List.filter (fun r -> not (List.contains (item "id" r) ids))) @ specs
    pyAssert ((out |> List.map (item "id") |> List.distinct |> List.length) = out.Length) ""
    let proposed (g: Json) =
        match g with
        | JStr s -> startsWith "PROPOSED" s
        | v -> raise (PyError $"AttributeError: {v} has no startswith")
    pyAssert (not (out |> List.exists (fun r -> itemList "capability_gaps" r |> List.exists proposed))) ""
    out

/// Gap rows: existing rows, then each NEWGAPS entry not yet registered, zipped onto the header with status "pending".
let gapRows (rows: CsvRow list) : CsvRow list =
    let fields = fieldsOf rows
    let have = rows |> List.map (cellStr "gap_id")
    let added =
        newGaps
        |> List.filter (fun (id, _, _, _, _, _, _) -> not (List.contains id have))
        |> List.map (fun (a, b, c, d, e, f, g) ->
            let values = [ a; b; c; d; e; f; g; "pending" ]
            let n = min fields.Length values.Length
            List.zip (List.take n fields) (List.take n values) |> ofPairs)
    rows @ added

let main (paths: Io.Paths) : int =
    let specs =
        [ "ui-spec-panels.jsonl"; "ui-spec-views.jsonl" ]
        |> List.collect (fun f -> Io.loadJsonl $"{paths.Specs}/{f}")
        |> specRecords
    let path = $"{paths.Data}/ui-requirements.jsonl"
    let out = merged (Io.loadJsonl path) specs
    Io.write path (UiQuality.jsonLines out)
    let gp = $"{paths.Data}/ui-component-gaps.csv"
    let rows0 = Io.loadCsv gp
    let fields = fieldsOf rows0
    let rows = gapRows rows0
    Io.write gp (writeDicts fields rows)
    Io.print $"{out.Length} ui records; {specs.Length} merged; {rows.Length} gaps"
    0
