/// narrative.py: assign stable content-addressed surrogate identifiers (NARR-<8 hex>) to normative statements that have
/// no identifier.
module Unbound.ReqAudit.Narrative

open System
open System.Security.Cryptography
open System.Text
open Unbound.Py
open Unbound.ReqAudit.PyCompat

let ID = re @"\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-[A-Z]?\d{1,4}\b"
let NORMATIVE = re @"\b(SHALL|MUST|must|shall|never|Never|required|mandatory|prohibited|SHOULD|may not|cannot)\b"
let SCOPE = [ "requirements"; "architecture" ]
let SKIP = "requirements/reconciliation/"
let BASELINE = "777a70d"

let private marks = re "[*_`]"
let private ws = re @"\s+"

let normalize (text: string) : string = strip (subLit ws " " (subLit marks "" text))

let sha256Hex (text: string) : string =
    SHA256.HashData(UTF8Encoding(false).GetBytes text) |> Array.map (fun b -> b.ToString "x2") |> String.concat ""

let surrogate (path: string) (text: string) : string = "NARR-" + (sha256Hex $"{path}\n{lower (normalize text)}").Substring(0, 8)

/// (1-based line, text) of each candidate normative line in a file's text.
let candidates (text: string) : (int * string) list =
    splitLines text
    |> List.mapi (fun i l -> i + 1, l)
    |> List.filter (fun (_, l) ->
        isSearch NORMATIVE l && not (isSearch ID l) && not (l.StartsWith '#' || l.StartsWith '|') && (split l).Length >= 6)

/// The CSV rows (surrogate_id, path, line, revision, original_text, duplicate flag) and the summary line.
let build (files: (string * string) list) : string list list * string =
    let rows = files |> List.collect (fun (rel, text) -> candidates text |> List.map (fun (n, t) -> surrogate rel t, rel, n, BASELINE, normalize t))
    let ids = rows |> List.map (fun (i, _, _, _, _) -> i)
    let counts = counter ids |> Map.ofList
    let collisions = counts |> Map.filter (fun _ n -> n > 1)
    let csvRows =
        rows |> List.map (fun (i, rel, n, rev, t) -> [ i; rel; string n; rev; t; (if collisions.ContainsKey i then "True" else "False") ])
    csvRows, $"{rows.Length} narrative normative statements; {counts.Count} distinct surrogates; {collisions.Count} repeated"

let main (argv: string list) : int =
    let root, out = argv.[0], argv.[1]
    if Io.runInherit "git" [ "-C"; root; "diff"; "--quiet"; BASELINE; "HEAD"; "--"; yield! SCOPE; $":!{SKIP}" ] <> 0 then
        Io.eprint "warning: originals differ from the baseline revision; surrogates describe the working tree"
    let rootFull = IO.Path.GetFullPath root
    let files =
        SCOPE
        |> List.collect (fun d -> rglobFiles (combine rootFull d) "*.md")
        |> List.map (fun p -> IO.Path.GetRelativePath(rootFull, p))
        |> List.filter (fun rel -> not (rel.StartsWith(SKIP, StringComparison.Ordinal)))
        |> sortStrings
    let rows, line = build (files |> List.map (fun rel -> rel, readTextReplace (combine rootFull rel)))
    writeText out (PyCsv.format PyCsv.QuoteMinimal ([ "surrogate_id"; "path"; "line"; "revision"; "original_text"; "duplicate_text_in_same_file" ] :: rows))
    Io.print line
    0
