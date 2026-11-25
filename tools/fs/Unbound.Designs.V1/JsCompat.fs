/// JavaScript-exact primitives used by the v1 design generators: JSON.stringify / JSON.parse for the values the
/// generators handle, Math.round, Number.prototype.toLocaleString("en-US", currency USD), String.prototype.padStart,
/// Date.UTC(...).toISOString().slice(0, 10), the mulberry32 generator and a SHA-256 hex digest. All pure.
module Unbound.Designs.V1.JsCompat

open System
open System.Text

// ---------- JSON ----------

/// A JSON value with object keys in insertion order (as JavaScript keeps string keys).
type JsValue =
    | JsNull
    | JsBool of bool
    | JsNumber of float
    | JsString of string
    | JsArray of JsValue list
    | JsObject of (string * JsValue) list

/// JSON.stringify string quoting: escapes ", \, \b \f \n \r \t, other C0 controls as \u00xx and lone surrogates.
let quote (s: string) =
    let sb = StringBuilder(s.Length + 2)
    sb.Append '"' |> ignore
    for i in 0 .. s.Length - 1 do
        let c = s.[i]
        match c with
        | '"' -> sb.Append "\\\"" |> ignore
        | '\\' -> sb.Append "\\\\" |> ignore
        | '\b' -> sb.Append "\\b" |> ignore
        | '\f' -> sb.Append "\\f" |> ignore
        | '\n' -> sb.Append "\\n" |> ignore
        | '\r' -> sb.Append "\\r" |> ignore
        | '\t' -> sb.Append "\\t" |> ignore
        | c when c < ' ' -> sb.AppendFormat("\\u{0:x4}", int c) |> ignore
        | c when Char.IsHighSurrogate c && not (i + 1 < s.Length && Char.IsLowSurrogate s.[i + 1]) ->
            sb.AppendFormat("\\u{0:x4}", int c) |> ignore
        | c when Char.IsLowSurrogate c && not (i > 0 && Char.IsHighSurrogate s.[i - 1]) ->
            sb.AppendFormat("\\u{0:x4}", int c) |> ignore
        | c -> sb.Append c |> ignore
    sb.Append '"' |> ignore
    sb.ToString()

/// Number to string as JavaScript prints it, for the integers and short decimals the generators use.
let numberToString (n: float) =
    if Double.IsNaN n || Double.IsInfinity n then "null"
    elif n = Math.Floor n && abs n < 1e21 then (if n = 0.0 then "0" else n.ToString("R", Globalization.CultureInfo.InvariantCulture))
    else n.ToString("R", Globalization.CultureInfo.InvariantCulture)

/// JSON.stringify(value) (indent = None) or JSON.stringify(value, null, indent).
let stringify (indent: int option) (value: JsValue) =
    let rec go (pad: string) v =
        match v with
        | JsNull -> "null"
        | JsBool b -> if b then "true" else "false"
        | JsNumber n -> numberToString n
        | JsString s -> quote s
        | JsArray [] -> "[]"
        | JsObject [] -> "{}"
        | JsArray xs ->
            match indent with
            | None -> "[" + (xs |> List.map (go pad) |> String.concat ",") + "]"
            | Some k ->
                let inner = pad + String(' ', k)
                "[\n" + (xs |> List.map (fun x -> inner + go inner x) |> String.concat ",\n") + "\n" + pad + "]"
        | JsObject kvs ->
            match indent with
            | None -> "{" + (kvs |> List.map (fun (k, x) -> quote k + ":" + go pad x) |> String.concat ",") + "}"
            | Some k ->
                let inner = pad + String(' ', k)
                "{\n" + (kvs |> List.map (fun (key, x) -> inner + quote key + ": " + go inner x) |> String.concat ",\n") + "\n" + pad + "}"
    go "" value

/// JSON.parse for well-formed JSON text. Raises FormatException on malformed input.
let parse (text: string) : JsValue =
    let n = text.Length
    let fail (i: int) = raise (FormatException(sprintf "Unexpected token in JSON at position %d" i))
    let rec ws i = if i < n && (text.[i] = ' ' || text.[i] = '\t' || text.[i] = '\n' || text.[i] = '\r') then ws (i + 1) else i
    let parseString i =
        // text.[i] = '"'
        let sb = StringBuilder()
        let rec loop j =
            if j >= n then fail j
            else
                match text.[j] with
                | '"' -> j + 1
                | '\\' when j + 1 < n ->
                    match text.[j + 1] with
                    | '"' -> sb.Append '"' |> ignore; loop (j + 2)
                    | '\\' -> sb.Append '\\' |> ignore; loop (j + 2)
                    | '/' -> sb.Append '/' |> ignore; loop (j + 2)
                    | 'b' -> sb.Append '\b' |> ignore; loop (j + 2)
                    | 'f' -> sb.Append '\f' |> ignore; loop (j + 2)
                    | 'n' -> sb.Append '\n' |> ignore; loop (j + 2)
                    | 'r' -> sb.Append '\r' |> ignore; loop (j + 2)
                    | 't' -> sb.Append '\t' |> ignore; loop (j + 2)
                    | 'u' when j + 5 < n ->
                        sb.Append(char (Convert.ToInt32(text.Substring(j + 2, 4), 16))) |> ignore
                        loop (j + 6)
                    | _ -> fail j
                | c when c < ' ' -> fail j
                | c -> sb.Append c |> ignore; loop (j + 1)
        let next = loop (i + 1)
        sb.ToString(), next
    let rec value i =
        let i = ws i
        if i >= n then fail i
        else
            match text.[i] with
            | '{' ->
                let rec members j acc =
                    let j = ws j
                    if j < n && text.[j] = '}' && List.isEmpty acc then JsObject [], j + 1
                    elif j < n && text.[j] = '"' then
                        let key, j = parseString j
                        let j = ws j
                        if j >= n || text.[j] <> ':' then fail j
                        let v, j = value (j + 1)
                        let j = ws j
                        if j < n && text.[j] = ',' then members (j + 1) ((key, v) :: acc)
                        elif j < n && text.[j] = '}' then JsObject(List.rev ((key, v) :: acc)), j + 1
                        else fail j
                    else fail j
                members (i + 1) []
            | '[' ->
                let rec items j acc =
                    let j = ws j
                    if j < n && text.[j] = ']' && List.isEmpty acc then JsArray [], j + 1
                    else
                        let v, j = value j
                        let j = ws j
                        if j < n && text.[j] = ',' then items (j + 1) (v :: acc)
                        elif j < n && text.[j] = ']' then JsArray(List.rev (v :: acc)), j + 1
                        else fail j
                items (i + 1) []
            | '"' -> let s, j = parseString i in JsString s, j
            | 't' when String.CompareOrdinal(text, i, "true", 0, 4) = 0 -> JsBool true, i + 4
            | 'f' when String.CompareOrdinal(text, i, "false", 0, 5) = 0 -> JsBool false, i + 5
            | 'n' when String.CompareOrdinal(text, i, "null", 0, 4) = 0 -> JsNull, i + 4
            | c when c = '-' || Char.IsDigit c ->
                let rec stop j = if j < n && ("+-.eE".IndexOf text.[j] >= 0 || Char.IsDigit text.[j]) then stop (j + 1) else j
                let j = stop (i + 1)
                JsNumber(Double.Parse(text.Substring(i, j - i), Globalization.CultureInfo.InvariantCulture)), j
            | _ -> fail i
    let v, i = value 0
    if ws i <> n then fail (ws i) else v

/// Object property lookup (undefined -> None).
let prop (key: string) (v: JsValue) =
    match v with
    | JsObject kvs -> kvs |> List.tryFind (fun (k, _) -> k = key) |> Option.map snd
    | _ -> None

// ---------- numbers and strings ----------

/// Math.round: nearest integer, halves toward +Infinity.
let mathRound (x: float) =
    let f = Math.Floor x
    if x - f >= 0.5 then f + 1.0 else f

/// Integer cents as (cents / 100).toLocaleString("en-US", {style: "currency", currency: "USD", minimumFractionDigits: 2}).
/// Exact for every integer cent amount below 2^53: c / 100 rounds back to the same two decimals.
let usdOfCents (cents: int64) =
    let sign = if cents < 0L then "-" else ""
    let a = abs cents
    let dollars = (a / 100L).ToString("#,0", Globalization.CultureInfo.InvariantCulture)
    sprintf "%s$%s.%02d" sign dollars (a % 100L)

/// String.prototype.replace(search, replacement) with a string pattern: replaces the first occurrence only.
let replaceFirst (search: string) (replacement: string) (s: string) =
    let i = s.IndexOf(search, StringComparison.Ordinal)
    if i < 0 then s else s.Substring(0, i) + replacement + s.Substring(i + search.Length)

/// String.prototype.padStart with a single-character pad.
let padStart (width: int) (ch: char) (s: string) = s.PadLeft(width, ch)

/// new Date(Date.UTC(year, monthIndex, day)).toISOString().slice(0, 10); day may overflow the month as in JavaScript.
let utcIsoDate (year: int) (monthIndex: int) (day: int) =
    DateTime(year, 1, 1).AddMonths(monthIndex).AddDays(float (day - 1)).ToString("yyyy-MM-dd", Globalization.CultureInfo.InvariantCulture)

// ---------- mulberry32 ----------

/// One step of mulberry32: (seed) -> (next seed, value in [0, 1)). Identical to
/// `seed = (seed + 0x6d2b79f5) | 0; t = Math.imul(seed ^ (seed >>> 15), 1 | seed); ...`.
let mulberry32 (seed: int32) : int32 * float =
    let s = seed + 0x6d2b79f5
    let us = uint32 s
    let t = int32 (us ^^^ (us >>> 15)) * (1 ||| s)
    let ut = uint32 t
    let t = (t + int32 (ut ^^^ (ut >>> 7)) * (61 ||| t)) ^^^ t
    let ut = uint32 t
    s, float (ut ^^^ (ut >>> 14)) / 4294967296.0

// ---------- hashing ----------

/// createHash("sha256").update(s).digest("hex") for a JavaScript string (UTF-8 encoded).
let sha256Hex (s: string) =
    Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes s)
    |> Array.map (fun b -> b.ToString("x2"))
    |> String.concat ""
