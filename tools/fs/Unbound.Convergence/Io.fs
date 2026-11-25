/// The program edge: file reads and writes as the Python scripts perform them, and the console sink.
module Unbound.Convergence.Io

open System.IO
open System.Text
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.Convergence.PyCompat

/// Where a command's stdout and stderr text goes (the console, or a buffer in tests).
type Console = { Out: string -> unit; Err: string -> unit }

let private utf8 = UTF8Encoding(false, false)

/// root / rel as Python's pathlib joins it (for I/O; messages use PyCompat.pathStr).
let path (root: string) (rel: string) : string = Path.Combine(root, rel)

let exists (p: string) : bool = File.Exists p

/// Path.read_text(encoding="utf-8"): UTF-8 (a BOM is kept as U+FEFF), universal newlines.
let readText (p: string) : string = utf8.GetString(File.ReadAllBytes p) |> PyCsv.universalNewlines

/// Path.write_text(text, encoding="utf-8").
let writeText (p: string) (text: string) : unit = File.WriteAllBytes(p, utf8.GetBytes text)

/// open(p, "a", encoding="utf-8").write(text): creates the file when absent.
let appendText (p: string) (text: string) : unit =
    use fs = new FileStream(p, FileMode.Append, FileAccess.Write)
    let bytes = utf8.GetBytes text
    fs.Write(bytes, 0, bytes.Length)

/// Path.mkdir(parents=True, exist_ok=True)
let mkdirs (p: string) : unit = Directory.CreateDirectory p |> ignore

/// Path.mkdir(exist_ok=True): the parent must exist.
let mkdir (p: string) : unit =
    if not (Directory.Exists p) then
        let parentDir = Path.GetDirectoryName(Path.GetFullPath p)
        if not (Directory.Exists parentDir) then raise (FileNotFoundException($"[Errno 2] No such file or directory: {reprStr p}"))
        Directory.CreateDirectory p |> ignore

/// [json.loads(l) for l in text.splitlines() if l.strip()]
let parseJsonl (text: string) : Json list = splitLines text |> List.filter (fun l -> strip l <> "") |> List.map loads

/// read_text().splitlines() JSON Lines.
let readJsonl (p: string) : Json list = readText p |> parseJsonl

/// Text of a CSV file read through csv.DictReader(io.StringIO(read_text())): rows as dicts with str values, or None
/// (JNull) for the missing cells of a short row.
let parseCsvDicts (text: string) : Json list =
    PyCsv.parseDicts text
    |> List.map (fun r -> r |> List.map (fun (k, v) -> k, (match v with Some s -> JStr s | None -> JNull)) |> toDict |> JObj)

let readCsvDicts (p: string) : Json list = readText p |> parseCsvDicts
