/// The program edge: standard streams, git subprocesses and file loaders. Everything else in this project is pure.
module Unbound.ReqAudit.Io

open System
open System.Diagnostics
open System.IO
open System.Text
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

let private utf8 = UTF8Encoding(false)

let private stdout' = lazy (new StreamWriter(Console.OpenStandardOutput(), utf8, NewLine = "\n", AutoFlush = false))
let private stderr' = lazy (new StreamWriter(Console.OpenStandardError(), utf8, NewLine = "\n", AutoFlush = true))

/// print(s).
let print (s: string) : unit = stdout'.Value.Write(s + "\n")

/// print(s, file=sys.stderr).
let eprint (s: string) : unit =
    stdout'.Value.Flush()
    stderr'.Value.Write(s + "\n")

let flush () = if stdout'.IsValueCreated then stdout'.Value.Flush()

type ProcResult = { Code: int; Out: string }

/// subprocess.run(args, capture_output=True, text=True[, errors="replace"]): stdout decoded as UTF-8 with universal
/// newlines; stderr captured and discarded.
let run (exe: string) (args: string list) : ProcResult =
    let psi = ProcessStartInfo(exe, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false,
                               StandardOutputEncoding = utf8, StandardErrorEncoding = utf8)
    args |> List.iter psi.ArgumentList.Add
    use p = Process.Start psi
    let errTask = p.StandardError.ReadToEndAsync()
    let out = p.StandardOutput.ReadToEnd()
    errTask.Wait()
    p.WaitForExit()
    { Code = p.ExitCode; Out = Unbound.Py.PyCsv.universalNewlines out }

/// subprocess.run(args).returncode with inherited standard streams.
let runInherit (exe: string) (args: string list) : int =
    flush ()
    let psi = ProcessStartInfo(exe, UseShellExecute = false)
    args |> List.iter psi.ArgumentList.Add
    use p = Process.Start psi
    p.WaitForExit()
    p.ExitCode

/// `git -C root show rev:path` stdout ("" when the command fails, as the callers treat it).
let gitShow (root: string) (rev: string) (path: string) : ProcResult = run "git" [ "-C"; root; "show"; $"{rev}:{path}" ]

/// load_jsonl: () when the file is absent.
let loadJsonl (path: string) : Json list = if File.Exists path then loadJsonlText (readText path) else []

/// load_csv (csv.DictReader): () when the file is absent.
let loadCsv (path: string) : Json list = if File.Exists path then dictRows (readText path) else []

/// json.loads(Path.read_text()).
let loadJson (path: string) : Json = loads (readText path)

/// Path.write_text(json.dumps(x, indent=2) + "\n").
let writeJsonReport (path: string) (j: Json) : unit = writeText path (dumpsWith { defaults with Indent = Some 2 } j + "\n")

let dumps2 (j: Json) = dumpsWith { defaults with Indent = Some 2 } j
