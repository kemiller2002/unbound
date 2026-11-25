/// benchmark/analyse.py: summarise pgbench per-transaction logs into a results table (Markdown on stdout, summary.csv).
module Unbound.ReqAudit.Benchmark

open System
open System.Numerics
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

let SCRIPTS = Map.ofList [ 0, "grant approval"; 1, "payment release"; 2, "hold publication" ]

/// (script, latency_ms, epoch_seconds, schedule_lag_ms)
type Row = int * float * float * float

let pct (sortedVals: float list) (p: float) : float =
    if sortedVals.IsEmpty then nan
    else sortedVals.[min (sortedVals.Length - 1) (int (PyFloat.roundInt (p * float (sortedVals.Length - 1))))]

let private toFloat (i: BigInteger) = float i

let parseLog (lines: string list) : Row list =
    lines
    |> List.choose (fun line ->
        let f = split line |> Array.ofList
        if f.Length >= 6 then
            let n k = pyInt f.[k]
            Some(int (n 3), toFloat (n 2) / 1000.0, toFloat (n 4) + toFloat (n 5) / 1e6, (if f.Length > 6 then toFloat (n 6) / 1000.0 else 0.0))
        else None)

type Summary = { N: int; Tps: float; P50: float; P95: float; P99: float; Max: float; LagP99: float }

let summarise (rows: Row list) : (int * Summary) list =
    if rows.IsEmpty then []
    else
        let epochs = rows |> List.map (fun (_, _, e, _) -> e)
        let span = let d = List.max epochs - List.min epochs in if d = 0.0 then 1.0 else d
        rows
        |> List.map (fun (sc, _, _, _) -> sc)
        |> List.distinct
        |> List.sort
        |> List.map (fun sc ->
            let mine = rows |> List.filter (fun (x, _, _, _) -> x = sc)
            let v = mine |> List.map (fun (_, lat, _, _) -> lat) |> List.sort
            let l = mine |> List.map (fun (_, _, _, lg) -> lg) |> List.sort
            sc, { N = v.Length; Tps = float v.Length / span; P50 = pct v 0.5; P95 = pct v 0.95; P99 = pct v 0.99; Max = List.last v; LagP99 = pct l 0.99 })

let private FAILED = re @"number of failed transactions: (\d+) \(([\d.]+)%\)"

let failures (text: string) : string = match search FAILED text with Some m -> $"{grp 1 m} ({grp 2 m}%%)" | None -> "n/a"

/// Path.stem / Path.suffix.
let private dot (name: string) = let i = name.LastIndexOf '.' in if 0 < i && i < name.Length - 1 then Some i else None
let private suffix (name: string) = match dot name with Some i -> name.Substring i | None -> ""
let private stem (name: string) = match dot name with Some i -> name.Substring(0, i) | None -> name

let r1 (x: float) = JFloat(PyFloat.roundTo 1 x)

let main (argv: string list) : int =
    let out = argv.[0]
    let lag =
        dictRows (readText (combine out "outbox-lag.csv"))
        |> List.fold (fun (m: Map<string, Json>) r -> m.Add(s "scenario" r, r)) Map.empty
    let rowsOut =
        glob out "S*.txt"
        |> List.filter (fun p -> not ((name p).EndsWith(".dispatch.txt", StringComparison.Ordinal)))
        |> List.collect (fun txt ->
            let nm = stem (name txt)
            let logs = glob out $"{nm}.*" |> List.filter (fun p -> suffix (name p) <> ".txt")
            let rows = logs |> List.collect (fun p -> parseLog (splitLines (readText p)))
            let summ = summarise rows
            let totalTps = PySum.sum (summ |> List.map (fun (_, x) -> x.Tps))
            let lagOf k = match lag.TryFind nm with Some r -> getOr k (JStr "") r | None -> JStr ""
            summ
            |> List.map (fun (sc, x) ->
                [ "scenario", JStr nm
                  "script", (match SCRIPTS.TryFind sc with Some n -> JStr n | None -> jint sc)
                  "tx", jint x.N
                  "tps", r1 x.Tps
                  "total_tps", r1 totalTps
                  "p50_ms", r1 x.P50
                  "p95_ms", r1 x.P95
                  "p99_ms", r1 x.P99
                  "max_ms", r1 x.Max
                  "sched_lag_p99_ms", r1 x.LagP99
                  "failed", JStr(failures (readText txt))
                  "outbox_lag_p99_ms", lagOf "lag_p99_ms"
                  "outbox_undelivered", lagOf "undelivered_after_drain" ]))
    match rowsOut with
    | [] -> raise (PyError "IndexError: list index out of range")
    | first :: _ ->
        let hdr = first |> List.map fst
        let cellOf (j: Json) = match j with JNull -> "" | j -> str j
        writeText (combine out "summary.csv") (PyCsv.format PyCsv.QuoteMinimal (hdr :: (rowsOut |> List.map (List.map (snd >> cellOf)))))
        Io.print ("| " + String.Join(" | ", hdr) + " |\n|" + String.replicate hdr.Length "---|")
        Io.print (String.Join("\n", rowsOut |> List.map (fun r -> "| " + String.Join(" | ", r |> List.map (snd >> str)) + " |")))
        0
