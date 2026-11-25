/// Python-compatible JSON: an ordered value type, json.loads semantics (int vs float, last duplicate key wins in the first
/// key's position) and json.dumps byte-compatible output for indent, ensure_ascii and sort_keys.
module Unbound.Py.PyJson

open System
open System.Globalization
open System.Numerics
open System.Text
open System.Text.Json

type Json =
    | JNull
    | JBool of bool
    | JInt of BigInteger
    | JFloat of float
    | JStr of string
    | JArr of Json list
    | JObj of (string * Json) list

// ---- loads ----

let private number (raw: string) : Json =
    if raw.IndexOfAny [| '.'; 'e'; 'E' |] >= 0 then JFloat(Double.Parse(raw, NumberStyles.Float, CultureInfo.InvariantCulture))
    else JInt(BigInteger.Parse(raw, CultureInfo.InvariantCulture))

/// Python dict semantics: a repeated key keeps its first position and takes the last value.
let private dedupe (pairs: (string * Json) list) : (string * Json) list =
    let last = pairs |> List.fold (fun (m: Map<string, Json>) (k, v) -> m.Add(k, v)) Map.empty
    pairs |> List.map fst |> List.distinct |> List.map (fun k -> k, last.[k])

let rec private ofElement (e: JsonElement) : Json =
    match e.ValueKind with
    | JsonValueKind.Null -> JNull
    | JsonValueKind.True -> JBool true
    | JsonValueKind.False -> JBool false
    | JsonValueKind.Number -> number (e.GetRawText())
    | JsonValueKind.String -> JStr(e.GetString())
    | JsonValueKind.Array -> JArr(e.EnumerateArray() |> Seq.map ofElement |> List.ofSeq)
    | JsonValueKind.Object -> JObj(e.EnumerateObject() |> Seq.map (fun p -> p.Name, ofElement p.Value) |> List.ofSeq |> dedupe)
    | k -> failwith $"unsupported JSON value kind {k}"

/// json.loads(text). NaN/Infinity literals are not supported.
let loads (text: string) : Json =
    use doc = JsonDocument.Parse(text, JsonDocumentOptions(AllowTrailingCommas = false, MaxDepth = 512))
    ofElement doc.RootElement

/// Parses a JSON Lines file's text (blank lines skipped), as `[json.loads(l) for l in text.splitlines() if l.strip()]`.
let loadsLines (text: string) : Json list =
    text.Split('\n') |> Array.map (fun l -> l.TrimEnd('\r')) |> Array.filter (fun l -> l.Trim() <> "") |> Array.map loads |> List.ofArray

// ---- dumps ----

type Options = { Indent: int option; EnsureAscii: bool; SortKeys: bool }

let defaults = { Indent = None; EnsureAscii = true; SortKeys = false }

/// Unicode code points of a string (surrogate pairs combined).
let codePoints (s: string) : int list =
    let rec go i acc =
        if i >= s.Length then List.rev acc
        elif Char.IsHighSurrogate s.[i] && i + 1 < s.Length && Char.IsLowSurrogate s.[i + 1] then go (i + 2) (Char.ConvertToUtf32(s.[i], s.[i + 1]) :: acc)
        else go (i + 1) (int s.[i] :: acc)
    go 0 []

/// Code-point order, as Python compares str (differs from UTF-16 ordinal order only around surrogates).
let compareCodePoints (a: string) (b: string) : int = compare (codePoints a) (codePoints b)

let private escape (ensureAscii: bool) (s: string) : string =
    let sb = StringBuilder(s.Length + 2)
    sb.Append('"') |> ignore
    s
    |> Seq.iter (fun c ->
        match c with
        | '"' -> sb.Append("\\\"") |> ignore
        | '\\' -> sb.Append("\\\\") |> ignore
        | '\n' -> sb.Append("\\n") |> ignore
        | '\r' -> sb.Append("\\r") |> ignore
        | '\t' -> sb.Append("\\t") |> ignore
        | '\b' -> sb.Append("\\b") |> ignore
        | '\012' -> sb.Append("\\f") |> ignore
        | c when c < ' ' -> sb.Append("\\u").Append((int c).ToString("x4")) |> ignore
        | c when ensureAscii && c > '~' -> sb.Append("\\u").Append((int c).ToString("x4")) |> ignore
        | c -> sb.Append(c) |> ignore)
    sb.Append('"').ToString()

let private floatText (x: float) =
    if Double.IsNaN x then "NaN"
    elif Double.IsPositiveInfinity x then "Infinity"
    elif Double.IsNegativeInfinity x then "-Infinity"
    else PyFloat.repr x

/// json.dumps(value, indent=..., ensure_ascii=..., sort_keys=...) with Python's default separators.
let dumpsWith (o: Options) (value: Json) : string =
    let itemSep = if o.Indent.IsSome then "," else ", "
    let sb = StringBuilder()
    let newline (level: int) =
        match o.Indent with
        | Some n -> sb.Append('\n').Append(String(' ', n * level)) |> ignore
        | None -> ()
    let rec go (level: int) (v: Json) =
        match v with
        | JNull -> sb.Append("null") |> ignore
        | JBool b -> sb.Append(if b then "true" else "false") |> ignore
        | JInt i -> sb.Append(i.ToString(CultureInfo.InvariantCulture)) |> ignore
        | JFloat x -> sb.Append(floatText x) |> ignore
        | JStr s -> sb.Append(escape o.EnsureAscii s) |> ignore
        | JArr [] -> sb.Append("[]") |> ignore
        | JObj [] -> sb.Append("{}") |> ignore
        | JArr xs ->
            sb.Append('[') |> ignore
            xs |> List.iteri (fun i x ->
                if i > 0 then sb.Append(itemSep) |> ignore
                newline (level + 1)
                go (level + 1) x)
            newline level
            sb.Append(']') |> ignore
        | JObj kvs ->
            let kvs = if o.SortKeys then kvs |> List.sortWith (fun (a, _) (b, _) -> compareCodePoints a b) else kvs
            sb.Append('{') |> ignore
            kvs |> List.iteri (fun i (k, x) ->
                if i > 0 then sb.Append(itemSep) |> ignore
                newline (level + 1)
                sb.Append(escape o.EnsureAscii k).Append(": ") |> ignore
                go (level + 1) x)
            newline level
            sb.Append('}') |> ignore
    go 0 value
    sb.ToString()

let dumps (value: Json) : string = dumpsWith defaults value

// ---- helpers for reading values ----

let field (name: string) (j: Json) : Json option =
    match j with
    | JObj kvs -> kvs |> List.tryFind (fst >> (=) name) |> Option.map snd
    | _ -> None

let str (j: Json) : string =
    match j with
    | JStr s -> s
    | _ -> failwith $"expected string, got {j}"

let num (j: Json) : float =
    match j with
    | JFloat x -> x
    | JInt i -> float i
    | _ -> failwith $"expected number, got {j}"

let list (j: Json) : Json list =
    match j with
    | JArr xs -> xs
    | _ -> failwith $"expected array, got {j}"
