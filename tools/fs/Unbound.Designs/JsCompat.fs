/// JavaScript-exact primitives used by the design generators: Number formatting (toString, toFixed, the en-US
/// two-decimal Intl.NumberFormat), JSON.parse / JSON.stringify(x, null, 2) over an insertion-ordered value, template
/// interpolation of parsed values, and String.prototype.localeCompare as Node's ICU root collation orders ASCII.
module Unbound.Designs.JsCompat

open System
open System.Globalization
open System.Numerics
open System.Text
open System.Text.Json

let private inv = CultureInfo.InvariantCulture

// ---------- numbers ----------

/// The shortest round-trip decimal digits of a finite, non-zero |x| as (digits, n): |x| = 0.digits × 10^n, with no
/// leading or trailing zeros in digits (ECMAScript Number::toString's k digits and n).
let private shortest (x: float) : string * int =
    let r = Math.Abs(x).ToString("R", inv)
    let mant, exp =
        match r.IndexOf 'E' with
        | -1 -> r, 0
        | i -> r.Substring(0, i), int (r.Substring(i + 1))
    let intPart, frac =
        match mant.IndexOf '.' with
        | -1 -> mant, ""
        | i -> mant.Substring(0, i), mant.Substring(i + 1)
    let raw = intPart + frac
    let lead = raw.Length - raw.TrimStart('0').Length
    raw.TrimStart('0').TrimEnd('0'), intPart.Length + exp - lead

/// Number.prototype.toString() (radix 10): shortest round-trip digits, JavaScript's exponent thresholds.
let numberToString (x: float) : string =
    if Double.IsNaN x then "NaN"
    elif Double.IsPositiveInfinity x then "Infinity"
    elif Double.IsNegativeInfinity x then "-Infinity"
    elif x = 0.0 then "0"
    else
        let sign = if x < 0.0 then "-" else ""
        let digits, n = shortest x
        let k = digits.Length
        let body =
            if k <= n && n <= 21 then digits + String('0', n - k)
            elif 0 < n && n <= 21 then digits.Substring(0, n) + "." + digits.Substring(n)
            elif -6 < n && n <= 0 then "0." + String('0', -n) + digits
            else
                let e = n - 1
                let es = if e >= 0 then "+" + string e else string e
                if k = 1 then digits + "e" + es else digits.Substring(0, 1) + "." + digits.Substring(1) + "e" + es
        sign + body

/// A non-negative integer count of 10^-f units as a decimal string with f fraction digits.
let private withPoint (f: int) (units: BigInteger) : string =
    let s = units.ToString(inv).PadLeft(f + 1, '0')
    if f = 0 then s else s.Substring(0, s.Length - f) + "." + s.Substring(s.Length - f)

/// round(num / den) with ties going up, for non-negative num and positive den.
let private roundHalfUp (num: BigInteger) (den: BigInteger) : BigInteger =
    let q = BigInteger.Divide(num, den)
    let r = num - q * den
    if r * BigInteger(2) >= den then q + BigInteger.One else q

/// Number.prototype.toFixed(f): the exact binary value rounded to f decimals, ties away from zero; "-" only for x < 0.
let toFixed (f: int) (x: float) : string =
    if Double.IsNaN x then "NaN"
    elif Math.Abs x >= 1e21 then numberToString x
    else
        let bits = BitConverter.DoubleToInt64Bits(Math.Abs x)
        let expBits = int ((bits >>> 52) &&& 0x7FFL)
        let frac = bits &&& 0xFFFFFFFFFFFFFL
        // |x| = m × 2^e exactly.
        let m, e = if expBits = 0 then BigInteger(frac), -1074 else BigInteger(frac ||| (1L <<< 52)), expBits - 1075
        let scaled = m * BigInteger.Pow(BigInteger(10), f)
        let units = if e >= 0 then scaled * BigInteger.Pow(BigInteger(2), e) else roundHalfUp scaled (BigInteger.Pow(BigInteger(2), -e))
        (if x < 0.0 then "-" else "") + withPoint f units

/// new Intl.NumberFormat("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(x) for finite x.
/// ICU rounds the shortest round-trip decimal (not the exact binary value) half away from zero, so 1.005 -> "1.01".
let formatEnUs2 (x: float) : string =
    let units =
        if x = 0.0 then BigInteger.Zero
        else
            let digits, n = shortest x
            // value in hundredths = digits × 10^(n - k + 2)
            let shift = n - digits.Length + 2
            let d = BigInteger.Parse(digits, inv)
            if shift >= 0 then d * BigInteger.Pow(BigInteger(10), shift) else roundHalfUp d (BigInteger.Pow(BigInteger(10), -shift))
    let fixedText = withPoint 2 units
    let whole, cents = fixedText.Substring(0, fixedText.Length - 3), fixedText.Substring(fixedText.Length - 2)
    let grouped =
        whole
        |> Seq.rev
        |> Seq.chunkBySize 3
        |> Seq.map (fun c -> String(Array.rev c))
        |> Seq.rev
        |> String.concat ","
    (if x < 0.0 || (x = 0.0 && Double.IsNegative x) then "-" else "") + grouped + "." + cents

// ---------- strings ----------

/// String.prototype.replace(string, string): the first occurrence only.
let replaceFirst (find: string) (repl: string) (s: string) : string =
    match s.IndexOf(find, StringComparison.Ordinal) with
    | -1 -> s
    | i -> s.Substring(0, i) + repl + s.Substring(i + find.Length)

/// String.prototype.replaceAll(string, string).
let replaceAll (find: string) (repl: string) (s: string) : string = s.Replace(find, repl, StringComparison.Ordinal)

// ICU root collation (Node's default localeCompare) for printable ASCII: primary order of punctuation, digits and
// letters (case-blind), then lowercase before uppercase. Control characters are ignorable. Characters outside ASCII
// are ordered by code point after every ASCII character (not ICU; no such character occurs in generated file names).
let private primaryGroups =
    [ " "; "_"; "-"; ","; ";"; ":"; "!"; "?"; "."; "'"; "\""; "("; ")"; "["; "]"; "{"; "}"; "@"; "*"; "/"; "\\"
      "&"; "#"; "%"; "`"; "^"; "+"; "<"; "="; ">"; "|"; "~"; "$"; "0"; "1"; "2"; "3"; "4"; "5"; "6"; "7"; "8"; "9" ]
    @ [ for c in 'a' .. 'z' -> string c + string (Char.ToUpperInvariant c) ]

let private primaryOf =
    primaryGroups |> List.mapi (fun i g -> g |> Seq.map (fun c -> c, i)) |> Seq.concat |> Map.ofSeq

let private collationKeys (s: string) : (int * int) list =
    [ for c in s do
        if c >= ' ' && c < '\u007f' then yield primaryOf.[c], (if Char.IsUpper c then 1 else 0)
        elif c < ' ' || c = '\u007f' then ()
        else yield 1000 + int c, 0 ]

/// a.localeCompare(b) as -1, 0 or 1.
let localeCompare (a: string) (b: string) : int =
    let ka, kb = collationKeys a, collationKeys b
    let p = compare (List.map fst ka) (List.map fst kb)
    if p <> 0 then sign p else sign (compare (List.map snd ka) (List.map snd kb))

// ---------- JSON values ----------

/// A parsed JSON value; objects keep their key order, as JavaScript objects with non-index keys do.
type JsValue =
    | JNull
    | JBool of bool
    | JNum of float
    | JStr of string
    | JArr of JsValue list
    | JObj of (string * JsValue) list

let rec private ofElement (e: JsonElement) : JsValue =
    match e.ValueKind with
    | JsonValueKind.Null -> JNull
    | JsonValueKind.True -> JBool true
    | JsonValueKind.False -> JBool false
    | JsonValueKind.Number -> JNum(Double.Parse(e.GetRawText(), NumberStyles.Float, inv))
    | JsonValueKind.String -> JStr(e.GetString())
    | JsonValueKind.Array -> JArr [ for x in e.EnumerateArray() -> ofElement x ]
    | _ ->
        // Duplicate keys: the last value wins, at the first key's position.
        let props = [ for p in e.EnumerateObject() -> p.Name, ofElement p.Value ]
        let keys = props |> List.map fst |> List.distinct
        JObj [ for k in keys -> k, props |> List.findBack (fst >> (=) k) |> snd ]

/// JSON.parse(text); None where JSON.parse throws.
let parseJson (text: string) : JsValue option =
    if text.StartsWith "﻿" then None
    else
        try
            use doc = JsonDocument.Parse(text, JsonDocumentOptions(MaxDepth = 10000))
            Some(ofElement doc.RootElement)
        with _ -> None

/// A property of an object value (None stands for undefined).
let prop (key: string) (v: JsValue) : JsValue option =
    match v with
    | JObj kvs -> kvs |> List.tryFind (fst >> (=) key) |> Option.map snd
    | _ -> None

/// The string a template literal `${v}` produces (None is undefined).
let rec interpolate (v: JsValue option) : string =
    match v with
    | None -> "undefined"
    | Some JNull -> "null"
    | Some(JBool b) -> if b then "true" else "false"
    | Some(JNum n) -> numberToString n
    | Some(JStr s) -> s
    | Some(JArr xs) -> join "," xs
    | Some(JObj _) -> "[object Object]"

/// Array.prototype.join(sep): null and undefined elements become "".
and join (sep: string) (xs: JsValue list) : string =
    xs |> List.map (function JNull -> "" | x -> interpolate (Some x)) |> String.concat sep

let private quote (s: string) : string =
    let sb = StringBuilder("\"")
    for i in 0 .. s.Length - 1 do
        let c = s.[i]
        match c with
        | '"' -> sb.Append("\\\"") |> ignore
        | '\\' -> sb.Append("\\\\") |> ignore
        | '\b' -> sb.Append("\\b") |> ignore
        | '\f' -> sb.Append("\\f") |> ignore
        | '\n' -> sb.Append("\\n") |> ignore
        | '\r' -> sb.Append("\\r") |> ignore
        | '\t' -> sb.Append("\\t") |> ignore
        | c when c < ' ' -> sb.Append("\\u").Append((int c).ToString("x4")) |> ignore
        | c when Char.IsHighSurrogate c && not (i + 1 < s.Length && Char.IsLowSurrogate s.[i + 1]) ->
            sb.Append("\\u").Append((int c).ToString("x4")) |> ignore
        | c when Char.IsLowSurrogate c && not (i > 0 && Char.IsHighSurrogate s.[i - 1]) ->
            sb.Append("\\u").Append((int c).ToString("x4")) |> ignore
        | c -> sb.Append(c) |> ignore
    sb.Append('"').ToString()

/// JSON.stringify(v, null, 2).
let stringify (v: JsValue) : string =
    let rec go (ind: string) (v: JsValue) : string =
        let inner = ind + "  "
        match v with
        | JNull -> "null"
        | JBool b -> if b then "true" else "false"
        | JNum n -> if Double.IsFinite n then numberToString n else "null"
        | JStr s -> quote s
        | JArr [] -> "[]"
        | JArr xs -> "[\n" + (xs |> List.map (fun x -> inner + go inner x) |> String.concat ",\n") + "\n" + ind + "]"
        | JObj [] -> "{}"
        | JObj kvs ->
            "{\n" + (kvs |> List.map (fun (k, x) -> inner + quote k + ": " + go inner x) |> String.concat ",\n") + "\n" + ind + "}"
    go "" v
