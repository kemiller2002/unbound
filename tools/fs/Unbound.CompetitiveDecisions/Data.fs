/// The package's inputs, loaded once at the program edge into an immutable record.
module Unbound.CompetitiveDecisions.Data

open System.IO
open Unbound.CompetitiveDecisions.Json

let MISSION = "requirements/discovery/2026-11-26-competitive-displacement-decisions"
let BASELINE = "requirements/baseline/baseline.jsonl"
let OWNER_DECISIONS = "requirements/convergence/2026-11-24-master/data/owner-decisions.jsonl"

/// Sibling packages whose proposed requirement ids this package may cite.
let SIBLINGS =
    [ "requirements/discovery/2026-11-26-charity-network-and-onboarding"
      "requirements/discovery/2026-11-26-file-ingestion"
      "requirements/discovery/2026-11-26-transitus-integration" ]

/// The Phase II research package inside a daf-landscape checkout.
let RESEARCH = "research/competitive-displacement"
let RESEARCH_PREFIX = "daf-landscape:"

type Csv = Map<string, string> list

/// What a daf-landscape checkout supplies (only with --research-root).
type Research =
    { Root: string
      RegisterIds: Set<string>
      Implications: Csv
      Decisions: Csv }

type Inputs =
    { Requirements: J list
      Implications: J list
      Decisions: Csv
      Approved: Csv
      Gates: Csv
      Coverage: Csv
      Constraints: Csv
      /// Every id defined outside this package that it may cite.
      ExternalIds: Set<string>
      Research: Research option }

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

/// The value of a CSV column, or "" when absent.
let col (name: string) (row: Map<string, string>) = row.TryFind name |> Option.defaultValue ""

/// Semicolon-separated ids of a CSV cell.
let ids (cell: string) : string list =
    cell.Split(';') |> Array.map (fun s -> s.Trim()) |> Array.filter ((<>) "") |> List.ofArray

let private readIf (path: string) = if File.Exists path then Some(File.ReadAllText path) else None

let private jsonlIds (path: string) : Set<string> =
    readIf path |> Option.map (parseLines >> List.map (fs "id") >> Set.ofList) |> Option.defaultValue Set.empty

let private csvIds (column: string) (path: string) : Set<string> =
    readIf path |> Option.map (csvMaps >> List.map (col column) >> Set.ofList) |> Option.defaultValue Set.empty

let loadResearch (root: string) : Research =
    let registers = Path.Combine(root, RESEARCH, "data/registers")
    let firstColumn (p: string) =
        match parseCsv (File.ReadAllText p) with
        | _ :: body -> body |> List.choose List.tryHead |> List.filter ((<>) "")
        | [] -> []
    { Root = root
      RegisterIds = Directory.GetFiles(registers, "*.csv") |> Seq.collect firstColumn |> Set.ofSeq
      Implications = csvMaps (File.ReadAllText(Path.Combine(registers, "cross-repository-implications.csv")))
      Decisions = csvMaps (File.ReadAllText(Path.Combine(registers, "decisions.csv"))) }

let load (root: string) (researchRoot: string option) : Inputs =
    let m (rel: string) = File.ReadAllText(Path.Combine(root, MISSION, rel))
    let discoveryDecisions =
        Directory.GetDirectories(Path.Combine(root, "requirements/discovery"))
        |> Array.filter (fun d -> not (d.Replace('\\', '/').EndsWith MISSION))
        |> Array.map (fun d -> csvIds "decision_id" (Path.Combine(d, "data/decisions.csv")))
        |> Set.unionMany
    { Requirements = parseLines (m "data/gap-requirements.jsonl")
      Implications = parseLines (m "data/implications.jsonl")
      Decisions = csvMaps (m "data/decisions.csv")
      Approved = csvMaps (m "data/approved-decisions.csv")
      Gates = csvMaps (m "data/validation-gates.csv")
      Coverage = csvMaps (m "data/coverage.csv")
      Constraints = csvMaps (m "data/constraint-checks.csv")
      ExternalIds =
        Set.unionMany
            [ jsonlIds (Path.Combine(root, BASELINE))
              jsonlIds (Path.Combine(root, OWNER_DECISIONS))
              discoveryDecisions
              yield! SIBLINGS |> List.map (fun s -> jsonlIds (Path.Combine(root, s, "data/gap-requirements.jsonl"))) ]
      Research = researchRoot |> Option.map loadResearch }
