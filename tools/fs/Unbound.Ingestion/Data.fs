/// The mission's inputs, loaded once at the program edge into an immutable record.
module Unbound.Ingestion.Data

open System.IO
open Unbound.Ingestion.Json

let MISSION = "requirements/discovery/2026-11-26-file-ingestion"
let BASELINE = "requirements/baseline/baseline.jsonl"
let AVAILABILITY = "requirements/discovery/2026-11-25-availability-architecture/data/gap-requirements.jsonl"

type Csv = Map<string, string> list

type Inputs =
    { Requirements: J list
      Workflows: J list
      Lifecycle: J
      Operations: J list
      TestCases: J list
      Profiles: (string * J) list
      Decisions: Csv
      Formats: Csv
      Reconciliation: Csv
      Catalogs: J
      Review: Csv
      BaselineIds: Set<string>
      AvailabilityIds: Set<string> }

/// RFC 4180 reader (quoted fields, doubled quotes, embedded newlines).
let parseCsv (text: string) : string list list =
    let folder (rows, row, cell: string, quoted, prevQuote) (c: char) =
        match quoted, c with
        | true, '"' -> (rows, row, cell, false, true)
        | true, _ -> (rows, row, cell + string c, true, false)
        | false, '"' when prevQuote -> (rows, row, cell + "\"", true, false)
        | false, '"' -> (rows, row, cell, true, false)
        | false, ',' -> (rows, cell :: row, "", false, false)
        | false, '\n' -> ((List.rev (cell :: row)) :: rows, [], "", false, false)
        | false, '\r' -> (rows, row, cell, false, false)
        | false, _ -> (rows, row, cell + string c, false, false)
    let rows, row, cell, _, _ = text |> Seq.fold folder ([], [], "", false, false)
    let all = if cell = "" && row.IsEmpty then rows else (List.rev (cell :: row)) :: rows
    List.rev all

let csvMaps (text: string) : Csv =
    match parseCsv text with
    | header :: body -> body |> List.filter (fun r -> r <> [ "" ]) |> List.map (fun r -> List.zip header r |> Map.ofList)
    | [] -> []

let private read (root: string) (rel: string) = File.ReadAllText(Path.Combine(root, rel))
let private exists (root: string) (rel: string) = File.Exists(Path.Combine(root, rel))

let load (root: string) : Inputs =
    let m rel = read root (MISSION + "/" + rel)
    let ids rel = if exists root rel then read root rel |> parseLines |> List.map (fs "id") |> Set.ofList else Set.empty
    let profileDir = Path.Combine(root, MISSION, "data/profiles")
    { Requirements = parseLines (m "data/gap-requirements.jsonl")
      Workflows = parse (m "data/workflow-matrix.json") |> field "workflows" |> items
      Lifecycle = parse (m "data/lifecycle.json")
      Operations = parseLines (m "data/operation-classification.jsonl")
      TestCases = parseLines (m "data/test-corpus.jsonl")
      Profiles =
        Directory.GetFiles(profileDir, "*.json")
        |> Array.sort
        |> Array.map (fun p -> Path.GetFileName p, parse (File.ReadAllText p))
        |> List.ofArray
      Decisions = csvMaps (m "data/decisions.csv")
      Formats = csvMaps (m "data/format-decisions.csv")
      Reconciliation = csvMaps (m "data/reconciliation.csv")
      Catalogs = parse (m "data/catalogs.json")
      Review = csvMaps (m "data/review-findings.csv")
      BaselineIds = ids BASELINE
      AvailabilityIds = ids AVAILABILITY }
