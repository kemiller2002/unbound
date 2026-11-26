/// The program edge: standard output, repository paths and file loaders/writers. Everything else in this project is pure.
module Unbound.Authoring.Io

open System
open System.IO
open System.Text
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.Authoring.PyCompat

let private utf8 = UTF8Encoding(false)

let private stdout' = lazy (new StreamWriter(Console.OpenStandardOutput(), utf8, NewLine = "\n", AutoFlush = false))
let private stderr' = lazy (new StreamWriter(Console.OpenStandardError(), utf8, NewLine = "\n", AutoFlush = true))

/// print(s).
let print (s: string) : unit = stdout'.Value.Write(s + "\n")

/// print(s, file=sys.stderr).
let eprint (s: string) : unit =
    if stdout'.IsValueCreated then stdout'.Value.Flush()
    stderr'.Value.Write(s + "\n")

let flush () = if stdout'.IsValueCreated then stdout'.Value.Flush()

/// The directories the scripts hard-coded, resolved against the repository root.
type Paths = { Data: string; V2: string; Specs: string }

let paths (repo: string) : Paths =
    { Data = Path.Combine(repo, "requirements/reconciliation/2026-11-23/data")
      V2 = Path.Combine(repo, "requirements/ui/designs/v2")
      Specs = Path.Combine(repo, "tools/fs/Unbound.Authoring/specs") }

/// The lines of `open(p)` (universal newlines), each without its "\n".
let private lines (path: string) : string list =
    let text = PyText.readText path
    let body = if text.EndsWith "\n" then text.Substring(0, text.Length - 1) else text
    if text = "" then [] else body.Split('\n') |> List.ofArray

/// `[json.loads(l) for l in open(p) if l.strip()]`.
let loadJsonl (path: string) : Json list = lines path |> List.filter hasText |> List.map loads

/// `[json.loads(l) for l in open(p)]` (a blank line raises).
let loadJsonlStrict (path: string) : Json list =
    lines path
    |> List.map (fun l -> if hasText l then loads l else raise (PyError "json.decoder.JSONDecodeError: Expecting value"))

/// json.load(open(p)).
let loadJson (path: string) : Json = loads (PyText.readText path)

/// list(csv.DictReader(open(p))): universal newlines.
let loadCsv (path: string) : CsvRow list = dictRows (PyText.readText path)

/// list(csv.DictReader(open(p, newline=""))).
let loadCsvRaw (path: string) : CsvRow list = dictRows (PyText.readRaw path)

/// open(p, "w").write(text) on POSIX (text mode; "\n" written as is).
let write (path: string) (text: string) : unit = PyText.writeText path text
