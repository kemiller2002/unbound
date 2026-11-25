/// Command-line edge of the economics models: the only place that reads files, writes files and prints.
module Unbound.Economics.Program

open System
open System.IO
open System.Text
open Unbound.Py

let private utf8 = UTF8Encoding(false)

let private writeStdout (text: string) =
    use out = Console.OpenStandardOutput()
    let bytes = utf8.GetBytes text
    out.Write(bytes, 0, bytes.Length)
    out.Flush()

/// The Python scripts locate their files relative to the script (HERE). The F# commands find the same directories by
/// walking up from the working directory to the repository root, so they behave alike from anywhere in the repository.
let private modelRel = Path.Combine("requirements", "discovery", "2026-11-24-simplification-economics", "model")
let private masterRel = Path.Combine("requirements", "convergence", "2026-11-24-master", "model")

let private repoRoot () : string =
    let rec up (dir: DirectoryInfo) =
        if isNull dir then failwith $"cannot find {modelRel} above {Directory.GetCurrentDirectory()}"
        elif Directory.Exists(Path.Combine(dir.FullName, modelRel)) then dir.FullName
        else up dir.Parent
    up (DirectoryInfo(Directory.GetCurrentDirectory()))

/// model.py [parameters.json] [output_dir]
let private runModel (args: string list) =
    let here = Path.Combine(repoRoot (), modelRel)
    let pfile = match args with p :: _ -> p | [] -> Path.Combine(here, "parameters.json")
    let outdir = match args with _ :: o :: _ -> o | _ -> here
    let out = Model.results (Model.load pfile)
    PyText.writeText (Path.Combine(outdir, "results.json")) out.ResultsJson
    PyText.writeText (Path.Combine(outdir, "results.csv")) out.ResultsCsv
    writeStdout out.Stdout
    0

/// decision_teams.py [parameters.json] [output_dir]
let private runDecisionTeams (args: string list) =
    let here = Path.Combine(repoRoot (), modelRel)
    let pfile = match args with p :: _ -> p | [] -> Path.Combine(here, "parameters.json")
    let outdir = match args with _ :: o :: _ -> o | _ -> here
    let out = DecisionTeams.results (Model.load pfile)
    PyText.writeText (Path.Combine(outdir, "decision-teams-results.json")) out.ResultsJson
    PyText.writeText (Path.Combine(outdir, "decision-teams-results.csv")) out.ResultsCsv
    writeStdout out.Stdout
    0

/// enterprise_economics.py [output_dir]
let private runEnterpriseEconomics (args: string list) =
    let root = repoRoot ()
    let here = Path.Combine(root, masterRel)
    let out = match args with o :: _ -> o | [] -> here
    let res = EnterpriseEconomics.outputs (Model.load (Path.Combine(root, modelRel, "parameters.json")))
    Directory.CreateDirectory out |> ignore
    PyText.writeText (Path.Combine(out, "enterprise-results.json")) res.ResultsJson
    PyText.writeText (Path.Combine(out, "activity-costs.csv")) res.ActivityCsv
    PyText.writeText (Path.Combine(out, "sensitivity.csv")) res.SensitivityCsv
    PyText.writeText (Path.Combine(out, "run.log")) (res.Summary + "\n")
    writeStdout (res.Summary + "\n")
    0

/// render_tables.py
let private runRenderTables () =
    let here = Path.Combine(repoRoot (), masterRel)
    let res = PyJson.loads (PyText.readText (Path.Combine(here, "enterprise-results.json")))
    PyText.writeText (Path.Combine(here, "..", "05a-ECONOMIC-TABLES.md")) (RenderTables.render res)
    0

let private usage =
    "usage: Unbound.Economics (model [parameters.json] [output_dir] | decision-teams [parameters.json] [output_dir] | enterprise-economics [output_dir] | render-tables)"

[<EntryPoint>]
let main argv =
    match List.ofArray argv with
    | "model" :: rest -> runModel rest
    | "decision-teams" :: rest -> runDecisionTeams rest
    | "enterprise-economics" :: rest -> runEnterpriseEconomics rest
    | [ "render-tables" ] -> runRenderTables ()
    | _ ->
        eprintfn "%s" usage
        2
