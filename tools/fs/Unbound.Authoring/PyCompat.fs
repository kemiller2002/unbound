/// Python-compatibility primitives the authoring port needs beyond Unbound.Py: exceptions that end the program, str
/// methods with Python's rules (strip, code-point slices), repr() of str lists, truthiness, ordered dict updates,
/// hashlib.sha1 hex digests and csv.DictReader/DictWriter rows. Copied in part from Unbound.ReqAudit/PyCompat.fs.
module Unbound.Authoring.PyCompat

open System
open System.Globalization
open System.Security.Cryptography
open System.Text
open Unbound.Py
open Unbound.Py.PyJson

/// A Python exception that would end the program with a traceback (exit 1).
exception PyError of string

/// `assert cond, msg`.
let pyAssert (cond: bool) (msg: string) : unit = if not cond then raise (PyError("AssertionError: " + msg))

/// str.isspace() for one UTF-16 unit (Python adds U+001C..U+001F to .NET's set).
let isSpace (c: char) = Char.IsWhiteSpace c || (c >= '\u001c' && c <= '\u001f')

/// `bool(s.strip())`: the string has a non-whitespace character.
let hasText (s: string) : bool = s |> Seq.exists (isSpace >> not)

let rec private skipFwd (p: char -> bool) (s: string) (i: int) = if i < s.Length && p s.[i] then skipFwd p s (i + 1) else i
let rec private skipBack (p: char -> bool) (s: string) (lo: int) (b: int) = if b > lo && p s.[b - 1] then skipBack p s lo (b - 1) else b

/// str.strip() with no argument.
let strip (s: string) = let a = skipFwd isSpace s 0 in s.Substring(a, skipBack isSpace s a s.Length - a)

/// str.startswith(prefix).
let startsWith (prefix: string) (s: string) = s.StartsWith(prefix, StringComparison.Ordinal)

let private hasSurrogate (s: string) = s |> Seq.exists Char.IsSurrogate

/// s[:n] in code points (n >= 0).
let take (n: int) (s: string) : string =
    if not (hasSurrogate s) then (if n >= s.Length then s else s.Substring(0, n))
    else
        let rec go i k = if i >= s.Length || k = n then i else go (if Char.IsHighSurrogate s.[i] && i + 1 < s.Length then i + 2 else i + 1) (k + 1)
        s.Substring(0, go 0 0)

/// str.split(sep) for a one-character separator.
let splitOn (sep: char) (s: string) : string list = s.Split(sep) |> List.ofArray

let private printable (cp: int) =
    if cp = 0x20 then true
    else
        match CharUnicodeInfo.GetUnicodeCategory cp with
        | UnicodeCategory.Control | UnicodeCategory.Format | UnicodeCategory.Surrogate | UnicodeCategory.PrivateUse
        | UnicodeCategory.OtherNotAssigned | UnicodeCategory.LineSeparator | UnicodeCategory.ParagraphSeparator
        | UnicodeCategory.SpaceSeparator -> false
        | _ -> true

/// repr(str).
let reprStr (s: string) : string =
    let q = if s.Contains '\'' && not (s.Contains '"') then '"' else '\''
    let sb = StringBuilder()
    sb.Append q |> ignore
    let rec go i =
        if i < s.Length then
            let c = s.[i]
            let cp, w =
                if Char.IsHighSurrogate c && i + 1 < s.Length && Char.IsLowSurrogate s.[i + 1] then Char.ConvertToUtf32(c, s.[i + 1]), 2
                else int c, 1
            if cp = int q || cp = int '\\' then sb.Append('\\').Append(c) |> ignore
            elif cp = int '\t' then sb.Append "\\t" |> ignore
            elif cp = int '\n' then sb.Append "\\n" |> ignore
            elif cp = int '\r' then sb.Append "\\r" |> ignore
            elif cp < 0x20 || cp = 0x7f then sb.Append("\\x").Append(cp.ToString "x2") |> ignore
            elif cp < 0x7f then sb.Append c |> ignore
            elif printable cp then sb.Append(s, i, w) |> ignore
            elif cp <= 0xff then sb.Append("\\x").Append(cp.ToString "x2") |> ignore
            elif cp <= 0xffff then sb.Append("\\u").Append(cp.ToString "x4") |> ignore
            else sb.Append("\\U").Append(cp.ToString "x8") |> ignore
            go (i + w)
    go 0
    sb.Append(q).ToString()

/// repr() of a list of str.
let reprList (xs: string seq) = "[" + (xs |> Seq.map reprStr |> String.concat ", ") + "]"

/// bool(x) of a loaded JSON value.
let truthy (j: Json) : bool =
    match j with
    | JNull -> false
    | JBool b -> b
    | JInt i -> not i.IsZero
    | JFloat x -> x <> 0.0
    | JStr s -> s <> ""
    | JArr xs -> not xs.IsEmpty
    | JObj kvs -> not kvs.IsEmpty

/// d[key] for a loaded JSON object (KeyError otherwise).
let item (key: string) (j: Json) : Json =
    match field key j with
    | Some v -> v
    | None -> raise (PyError $"KeyError: {reprStr key}")

/// d[key] that must be a str.
let itemStr (key: string) (j: Json) : string =
    match item key j with
    | JStr s -> s
    | v -> raise (PyError $"TypeError: expected str for {reprStr key}, got {v}")

/// d[key] that must be a list.
let itemList (key: string) (j: Json) : Json list =
    match item key j with
    | JArr xs -> xs
    | v -> raise (PyError $"TypeError: expected list for {reprStr key}, got {v}")

/// d.get(key, []) for a list value.
let getList (key: string) (j: Json) : Json list =
    match field key j with
    | Some(JArr xs) -> xs
    | Some v -> raise (PyError $"TypeError: expected list for {reprStr key}, got {v}")
    | None -> []

/// `dict(d, key=value)` / `{**d, key: value}`: an existing key keeps its position, a new key is appended.
let setKey (key: string) (value: 'v) (pairs: (string * 'v) list) : (string * 'v) list =
    if pairs |> List.exists (fst >> (=) key) then pairs |> List.map (fun (k, v) -> if k = key then k, value else k, v)
    else pairs @ [ key, value ]

/// The same for a JSON object.
let setField (key: string) (value: Json) (j: Json) : Json =
    match j with
    | JObj kvs -> JObj(setKey key value kvs)
    | v -> raise (PyError $"TypeError: not a dict: {v}")

/// hashlib.sha1(s.encode()).hexdigest().
let sha1Hex (s: string) : string = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes s)).ToLowerInvariant()

/// json.dumps(s)[1:-1]: a string's escaped JSON body with ensure_ascii=True.
let jsonBody (s: string) : string = let d = dumps (JStr s) in d.Substring(1, d.Length - 2)

/// json.dumps(value, ensure_ascii=False).
let dumpsUnicode (j: Json) : string = dumpsWith { defaults with EnsureAscii = false } j

// ---------------------------------------------------------------- csv.DictReader / csv.DictWriter

/// A csv.DictReader row: header -> value; None when the row was short.
type CsvRow = (string * string option) list

/// csv.DictReader over the file text (already newline-translated if the file was opened without newline="").
let dictRows (text: string) : CsvRow list = PyCsv.parseDicts text

/// The header of a DictReader (`list(rows[0].keys())`; IndexError when there are no rows).
let fieldsOf (rows: CsvRow list) : string list =
    match rows with
    | r :: _ -> r |> List.map fst
    | [] -> raise (PyError "IndexError: list index out of range")

/// row[key] (KeyError when absent; "" written for None as csv does, so callers that need the str use this).
let cell (key: string) (row: CsvRow) : string option =
    match row |> List.tryFind (fst >> (=) key) with
    | Some(_, v) -> v
    | None -> raise (PyError $"KeyError: {reprStr key}")

/// row[key] used as a str (a None value would raise in Python before any output).
let cellStr (key: string) (row: CsvRow) : string =
    match cell key row with
    | Some s -> s
    | None -> raise (PyError $"AttributeError: 'NoneType' value for {reprStr key}")

/// Lifts constructed (key, str) pairs into a row.
let ofPairs (pairs: (string * string) list) : CsvRow = pairs |> List.map (fun (k, v) -> k, Some v)

/// csv.DictWriter(f, fieldnames, quoting=QUOTE_ALL): writeheader() + writerows(rows); missing keys and None write "",
/// keys outside fieldnames raise ValueError.
let writeDicts (fields: string list) (rows: CsvRow list) : string =
    let line (row: CsvRow) =
        match row |> List.tryFind (fun (k, _) -> not (List.contains k fields)) with
        | Some(k, _) -> raise (PyError $"ValueError: dict contains fields not in fieldnames: {reprStr k}")
        | None ->
            fields |> List.map (fun f -> row |> List.tryFind (fst >> (=) f) |> Option.bind snd |> Option.defaultValue "")
    PyCsv.format PyCsv.QuoteAll (fields :: (rows |> List.map line))
