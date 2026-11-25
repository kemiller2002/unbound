/// Python str and file conventions used when porting: code-point sorting, UTF-8 without BOM, universal newlines.
module Unbound.Py.PyText

open System
open System.IO
open System.Text

/// sorted(strings) in Python order.
let sortStrings (xs: string seq) : string list = xs |> List.ofSeq |> List.sortWith PyJson.compareCodePoints

/// sorted(xs, key=f) for a string key (stable, Python order).
let sortByString (f: 'a -> string) (xs: 'a seq) : 'a list = xs |> List.ofSeq |> List.sortWith (fun a b -> PyJson.compareCodePoints (f a) (f b))

let private utf8 = UTF8Encoding(false)

/// Path.read_text(encoding="utf-8") / open(p, encoding="utf-8").read(): universal newlines applied.
let readText (path: string) : string = File.ReadAllText(path, utf8) |> PyCsv.universalNewlines

/// open(p, newline="").read(): no newline translation.
let readRaw (path: string) : string = File.ReadAllText(path, utf8)

/// Path.write_text(text, encoding="utf-8") on POSIX: written as is, UTF-8 without BOM.
let writeText (path: string) (text: string) : unit = File.WriteAllText(path, text, utf8)

/// str.splitlines() for text that has already had universal newlines applied (only "\n" separators).
let splitLines (text: string) : string list =
    if text = "" then [] else (if text.EndsWith "\n" then text.Substring(0, text.Length - 1) else text).Split('\n') |> List.ofArray
