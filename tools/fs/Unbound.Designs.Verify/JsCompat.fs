/// JavaScript-compatible primitives used by the verifier ports: an immutable JSON value with JS property order,
/// JSON.stringify(x, null, n), Number.prototype.toString, String.prototype.localeCompare (ICU root order for
/// ASCII), Date.prototype.toISOString and the JS meaning of the regex class \s. All functions are pure.
module Unbound.Designs.Verify.JsCompat

open System
open System.Globalization
open System.Text
open System.Text.Json

/// A JSON value. Objects keep their keys in insertion order (build them with `obj` to get JS key order).
type Json =
    | JNull
    | JBool of bool
    | JNum of float
    | JStr of string
    | JArr of Json list
    | JObj of (string * Json) list

// ---------- construction and access ----------

let private isArrayIndex (k: string) =
    k.Length > 0 && k.Length <= 10 && Seq.forall Char.IsAsciiDigit k && (k = "0" || k.[0] <> '0')
    && UInt64.Parse(k, CultureInfo.InvariantCulture) < 4294967295UL

/// A JS object literal / Object.fromEntries: later duplicates replace the value in place; integer-like keys first,
/// ascending, then the others in insertion order (ECMAScript OrdinaryOwnPropertyKeys).
let obj (entries: (string * Json) list) : Json =
    let folded =
        entries
        |> List.fold (fun acc (k, v) -> if List.exists (fun (k2, _) -> k2 = k) acc then acc |> List.map (fun (k2, v2) -> if k2 = k then (k2, v) else (k2, v2)) else acc @ [ (k, v) ]) []
    let ints = folded |> List.filter (fst >> isArrayIndex) |> List.sortBy (fun (k, _) -> UInt64.Parse(k, CultureInfo.InvariantCulture))
    JObj(ints @ (folded |> List.filter (fst >> isArrayIndex >> not)))

/// An object whose entries may be `undefined` (None): those keys are omitted, as JSON.stringify does.
let objOpt (entries: (string * Json option) list) : Json =
    entries |> List.choose (fun (k, v) -> v |> Option.map (fun x -> k, x)) |> obj

let str s = JStr s
let num (n: float) = JNum n
let jint (n: int) = JNum(float n)
let bool b = JBool b
let arr xs = JArr xs
let strs xs = xs |> List.map JStr |> JArr

let prop (key: string) (j: Json) : Json option =
    match j with
    | JObj kvs -> kvs |> List.tryFind (fun (k, _) -> k = key) |> Option.map snd
    | _ -> None

/// j[key]; missing keys read as null.
let (?) (j: Json) (key: string) : Json = prop key j |> Option.defaultValue JNull

let asStr j = match j with JStr s -> s | _ -> failwithf "expected a string, got %A" j
let asNum j = match j with JNum n -> n | _ -> failwithf "expected a number, got %A" j
let asBool j = match j with JBool b -> b | _ -> failwithf "expected a boolean, got %A" j
let asList j = match j with JArr xs -> xs | JNull -> [] | _ -> failwithf "expected an array, got %A" j

/// JS truthiness of a JSON value.
let truthy j =
    match j with
    | JNull -> false
    | JBool b -> b
    | JNum n -> not (n = 0.0 || Double.IsNaN n)
    | JStr s -> s <> ""
    | JArr _ | JObj _ -> true

/// Object without the given key ({key, ...rest} => rest).
let without (key: string) (j: Json) =
    match j with
    | JObj kvs -> JObj(kvs |> List.filter (fun (k, _) -> k <> key))
    | other -> other

// ---------- Number.prototype.toString ----------

/// Number.prototype.toString(10) for finite and non-finite doubles.
let numberToString (d: float) : string =
    if Double.IsNaN d then "NaN"
    elif Double.IsPositiveInfinity d then "Infinity"
    elif Double.IsNegativeInfinity d then "-Infinity"
    elif d = 0.0 then "0"
    else
        let neg = d < 0.0
        let r = (abs d).ToString("R", CultureInfo.InvariantCulture)
        // r is either plain ("123.45", "0.0001") or exponential ("1.2345E+21", "1E-07"); normalise to digits and exponent n
        // where value = 0.d1d2...dk * 10^n.
        let mant, exp =
            match r.IndexOf 'E' with
            | -1 -> r, 0
            | i -> r.Substring(0, i), Int32.Parse(r.Substring(i + 1), CultureInfo.InvariantCulture)
        let intPart, fracPart =
            match mant.IndexOf '.' with
            | -1 -> mant, ""
            | i -> mant.Substring(0, i), mant.Substring(i + 1)
        let allDigits = intPart + fracPart
        let lead = allDigits.Length - allDigits.TrimStart('0').Length
        let digits = allDigits.TrimStart('0').TrimEnd('0')
        let n = intPart.Length - lead + exp
        let k = digits.Length
        let body =
            if k <= n && n <= 21 then digits + String('0', n - k)
            elif 0 < n && n <= 21 then digits.Substring(0, n) + "." + digits.Substring n
            elif -6 < n && n <= 0 then "0." + String('0', -n) + digits
            else
                let e = n - 1
                let sign = if e >= 0 then "+" else "-"
                let m = if k = 1 then digits else digits.Substring(0, 1) + "." + digits.Substring 1
                m + "e" + sign + string (abs e)
        if neg then "-" + body else body

// ---------- JSON.stringify ----------

let private quote (s: string) =
    let sb = StringBuilder(s.Length + 2)
    sb.Append '"' |> ignore
    let n = s.Length
    for i in 0 .. n - 1 do
        let c = s.[i]
        match c with
        | '"' -> sb.Append "\\\"" |> ignore
        | '\\' -> sb.Append "\\\\" |> ignore
        | '\b' -> sb.Append "\\b" |> ignore
        | '\f' -> sb.Append "\\f" |> ignore
        | '\n' -> sb.Append "\\n" |> ignore
        | '\r' -> sb.Append "\\r" |> ignore
        | '\t' -> sb.Append "\\t" |> ignore
        | c when c < ' ' -> sb.AppendFormat(CultureInfo.InvariantCulture, "\\u{0:x4}", int c) |> ignore
        | c when Char.IsHighSurrogate c && not (i + 1 < n && Char.IsLowSurrogate s.[i + 1]) -> sb.AppendFormat(CultureInfo.InvariantCulture, "\\u{0:x4}", int c) |> ignore
        | c when Char.IsLowSurrogate c && not (i > 0 && Char.IsHighSurrogate s.[i - 1]) -> sb.AppendFormat(CultureInfo.InvariantCulture, "\\u{0:x4}", int c) |> ignore
        | c -> sb.Append c |> ignore
    sb.Append('"').ToString()

let private numJson (d: float) = if Double.IsFinite d then numberToString d else "null"

/// JSON.stringify(value, null, indent) (indent 0 = compact).
let stringify (indent: int) (value: Json) : string =
    let sb = StringBuilder()
    let rec go (level: int) (v: Json) =
        let pad l = if indent > 0 then sb.Append('\n').Append(String(' ', indent * l)) |> ignore
        match v with
        | JNull -> sb.Append "null" |> ignore
        | JBool b -> sb.Append(if b then "true" else "false") |> ignore
        | JNum d -> sb.Append(numJson d) |> ignore
        | JStr s -> sb.Append(quote s) |> ignore
        | JArr [] -> sb.Append "[]" |> ignore
        | JObj [] -> sb.Append "{}" |> ignore
        | JArr xs ->
            sb.Append '[' |> ignore
            xs |> List.iteri (fun i x ->
                if i > 0 then sb.Append ',' |> ignore
                pad (level + 1)
                go (level + 1) x)
            pad level
            sb.Append ']' |> ignore
        | JObj kvs ->
            sb.Append '{' |> ignore
            kvs |> List.iteri (fun i (k, x) ->
                if i > 0 then sb.Append ',' |> ignore
                pad (level + 1)
                sb.Append(quote k).Append(if indent > 0 then ": " else ":") |> ignore
                go (level + 1) x)
            pad level
            sb.Append '}' |> ignore
    go 0 value
    sb.ToString()

// ---------- JSON.parse ----------

let rec ofElement (e: JsonElement) : Json =
    match e.ValueKind with
    | JsonValueKind.Null | JsonValueKind.Undefined -> JNull
    | JsonValueKind.True -> JBool true
    | JsonValueKind.False -> JBool false
    | JsonValueKind.Number -> JNum(e.GetDouble())
    | JsonValueKind.String -> JStr(e.GetString())
    | JsonValueKind.Array -> JArr [ for x in e.EnumerateArray() -> ofElement x ]
    | _ -> JObj [ for p in e.EnumerateObject() -> p.Name, ofElement p.Value ]

let parse (text: string) : Json =
    use doc = JsonDocument.Parse(text, JsonDocumentOptions(MaxDepth = 512))
    ofElement doc.RootElement

// ---------- String.prototype.localeCompare (ICU root collation, ASCII) ----------

// Primary order of printable ASCII under ICU's root collation (letters compare case-insensitively at the primary level).
let private asciiOrder = " _-,;:!?.'\"()[]{}@*/\\&#%`^+<=>|~$0123456789aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ"

let private primary (c: char) =
    match asciiOrder.IndexOf c with
    | -1 -> 1000 + int c
    | _ when Char.IsAsciiLetter c -> asciiOrder.IndexOf(Char.ToLowerInvariant c)
    | i -> i

let private tertiary (c: char) = if Char.IsAsciiLetterUpper c then 1 else 0

/// a.localeCompare(b) sign for the root locale: primary weights, then case (lower first), then code units.
let localeCompare (a: string) (b: string) : int =
    let cmpSeq (f: char -> int) =
        let xs = a |> Seq.map f |> List.ofSeq
        let ys = b |> Seq.map f |> List.ofSeq
        compare xs ys
    match cmpSeq primary with
    | 0 ->
        match cmpSeq tertiary with
        | 0 -> String.CompareOrdinal(a, b) |> sign
        | t -> sign t
    | p -> sign p

// ---------- dates, regex helpers ----------

/// new Date().toISOString() for the given instant.
let isoString (t: DateTime) = t.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture)

/// The characters JavaScript's \s matches (WhiteSpace and LineTerminator).
let jsWhitespace : char[] =
    Array.append
        [| 0x09; 0x0a; 0x0b; 0x0c; 0x0d; 0x20; 0xa0; 0x1680; 0x2028; 0x2029; 0x202f; 0x205f; 0x3000; 0xfeff |]
        [| 0x2000 .. 0x200a |]
    |> Array.map char

/// A regex character-class body equivalent to JavaScript's \s.
let jsWs = jsWhitespace |> Array.map (fun c -> "\\u" + (int c).ToString("x4", CultureInfo.InvariantCulture)) |> String.concat ""

/// String.prototype.trim().
let jsTrim (s: string) = s.Trim jsWhitespace

/// String(value) for JSON values (arrays join their elements with ",", null elements as "").
let rec jsString (j: Json) : string =
    match j with
    | JNull -> "null"
    | JBool b -> if b then "true" else "false"
    | JNum n -> numberToString n
    | JStr s -> s
    | JArr xs -> xs |> List.map (function JNull -> "" | x -> jsString x) |> String.concat ","
    | JObj _ -> "[object Object]"

/// Array.prototype.join(sep) over JSON values.
let jsJoin (sep: string) (xs: Json list) = xs |> List.map (function JNull -> "" | x -> jsString x) |> String.concat sep

/// String.prototype.slice(0, n) (UTF-16 code units).
let slice0 (n: int) (s: string) = if s.Length <= n then s else s.Substring(0, n)

