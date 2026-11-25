/// Python-compatibility primitives needed by the convergence pipeline that Unbound.Py does not provide:
/// str whitespace handling and splitlines, code-point length and slicing, str.lower, repr/str of values,
/// truthiness, dict-style access on ordered JSON objects, pathlib/os.path string forms, csv writing with a chosen
/// line terminator, and sha256 of UTF-8 text. All functions are pure.
module Unbound.Convergence.PyCompat

open System
open System.Globalization
open System.Security.Cryptography
open System.Text
open Unbound.Py
open Unbound.Py.PyJson

// ---------------------------------------------------------------- str

/// str.isspace() for one UTF-16 unit (every Python whitespace character is in the BMP).
let isSpace (c: char) : bool = Char.IsWhiteSpace c || (c >= '\u001c' && c <= '\u001f')

/// str.split() with no arguments: runs of whitespace separate; no empty strings.
let splitWs (s: string) : string list =
    let rec go i start acc =
        if i >= s.Length then
            List.rev (if start < i then s.Substring(start, i - start) :: acc else acc)
        elif isSpace s.[i] then
            go (i + 1) (i + 1) (if start < i then s.Substring(start, i - start) :: acc else acc)
        else go (i + 1) start acc
    go 0 0 []

/// " ".join(s.split())
let joinWs (s: string) : string = splitWs s |> String.concat " "

/// str.strip() / lstrip() / rstrip() with no arguments.
let strip (s: string) : string =
    let a = s |> Seq.tryFindIndex (isSpace >> not)
    match a with
    | None -> ""
    | Some a ->
        let b = s |> Seq.tryFindIndexBack (isSpace >> not) |> Option.get
        s.Substring(a, b - a + 1)

let rstrip (s: string) : string =
    match s |> Seq.tryFindIndexBack (isSpace >> not) with
    | None -> ""
    | Some b -> s.Substring(0, b + 1)

/// str.strip(chars) / rstrip(chars).
let stripChars (chars: string) (s: string) : string = s.Trim(chars.ToCharArray())
let rstripChars (chars: string) (s: string) : string = s.TrimEnd(chars.ToCharArray())

/// str.splitlines() (no keepends): every Python line boundary, "\r\n" counted once, no trailing empty line.
let splitLines (s: string) : string list =
    let isBreak c =
        match c with
        | '\n' | '\r' | '\u000b' | '\u000c' | '\u001c' | '\u001d' | '\u001e' | '\u0085' | ' ' | ' ' -> true
        | _ -> false
    let rec go i start acc =
        if i >= s.Length then List.rev (if start < s.Length then s.Substring start :: acc else acc)
        elif isBreak s.[i] then
            let next = if s.[i] = '\r' && i + 1 < s.Length && s.[i + 1] = '\n' then i + 2 else i + 1
            go next next (s.Substring(start, i - start) :: acc)
        else go (i + 1) start acc
    go 0 0 []

/// Number of code points (len(s)).
let cpLen (s: string) : int =
    let rec go i n =
        if i >= s.Length then n
        elif Char.IsHighSurrogate s.[i] && i + 1 < s.Length && Char.IsLowSurrogate s.[i + 1] then go (i + 2) (n + 1)
        else go (i + 1) (n + 1)
    go 0 0

/// s[:n] for n >= 0, counted in code points.
let cpTake (n: int) (s: string) : string =
    let rec go i k =
        if i >= s.Length || k >= n then i
        elif Char.IsHighSurrogate s.[i] && i + 1 < s.Length && Char.IsLowSurrogate s.[i + 1] then go (i + 2) (k + 1)
        else go (i + 1) (k + 1)
    s.Substring(0, go 0 0)

/// str.lower(): full lowercase mapping for the one BMP character whose lowercase is longer (U+0130), simple mapping
/// elsewhere (the invariant culture's mapping).
let lower (s: string) : string = s.Replace("İ", "i̇").ToLowerInvariant()

let startsWith (prefix: string) (s: string) : bool = s.StartsWith(prefix, StringComparison.Ordinal)
let endsWith (suffix: string) (s: string) : bool = s.EndsWith(suffix, StringComparison.Ordinal)
let contains (sub: string) (s: string) : bool = s.Contains(sub, StringComparison.Ordinal)
/// str.find(sub): index of the first occurrence (UTF-16 index; order-preserving relative to code-point index).
let find (sub: string) (s: string) : int = s.IndexOf(sub, StringComparison.Ordinal)

/// str.removeprefix.
let removePrefix (prefix: string) (s: string) : string = if startsWith prefix s then s.Substring prefix.Length else s

/// str.replace(old, new) (all occurrences, ordinal).
let replace (oldS: string) (newS: string) (s: string) : string = if oldS = "" then s else s.Replace(oldS, newS, StringComparison.Ordinal)

/// str.split(sep) with an explicit separator.
let splitOn (sep: string) (s: string) : string list = s.Split(sep, StringSplitOptions.None) |> List.ofArray

/// str.split(sep, 1).
let splitOnce (sep: string) (s: string) : string list =
    match s.IndexOf(sep, StringComparison.Ordinal) with
    | -1 -> [ s ]
    | i -> [ s.Substring(0, i); s.Substring(i + sep.Length) ]

/// Python string order (code points).
let cmpStr (a: string) (b: string) : int = PyJson.compareCodePoints a b

/// Lexicographic comparison of string lists (tuples of str).
let rec cmpStrs (a: string list) (b: string list) : int =
    match a, b with
    | [], [] -> 0
    | [], _ -> -1
    | _, [] -> 1
    | x :: xs, y :: ys ->
        match cmpStr x y with
        | 0 -> cmpStrs xs ys
        | c -> c

/// sorted(xs, key=...) where the key is a tuple of str (stable).
let sortByStrs (key: 'a -> string list) (xs: 'a list) : 'a list = List.sortWith (fun a b -> cmpStrs (key a) (key b)) xs
let sortStrs (xs: string seq) : string list = xs |> List.ofSeq |> List.sortWith cmpStr

/// dict.fromkeys(xs): first occurrence order.
let dedupe (xs: 'a list) : 'a list = List.distinct xs

/// Counter(xs) as an ordered list of (key, count) in first-occurrence order.
let counter (xs: 'a list) : ('a * int) list =
    let counts = xs |> List.fold (fun (m: Map<'a, int>) x -> m.Add(x, 1 + (m.TryFind x |> Option.defaultValue 0))) Map.empty
    xs |> List.distinct |> List.map (fun x -> x, counts.[x])

/// repr(str).
let reprStr (s: string) : string =
    let quote = if s.Contains '\'' && not (s.Contains '"') then '"' else '\''
    let sb = StringBuilder()
    sb.Append(quote) |> ignore
    let printable (cp: int) =
        if cp = 0x20 then true
        else
            let cat = CharUnicodeInfo.GetUnicodeCategory cp
            match cat with
            | UnicodeCategory.Control | UnicodeCategory.Format | UnicodeCategory.Surrogate | UnicodeCategory.PrivateUse
            | UnicodeCategory.OtherNotAssigned | UnicodeCategory.LineSeparator | UnicodeCategory.ParagraphSeparator
            | UnicodeCategory.SpaceSeparator -> false
            | _ -> true
    let rec go i =
        if i < s.Length then
            let c = s.[i]
            let cp, w =
                if Char.IsHighSurrogate c && i + 1 < s.Length && Char.IsLowSurrogate s.[i + 1] then Char.ConvertToUtf32(c, s.[i + 1]), 2
                else int c, 1
            (match cp with
             | _ when cp = int quote || cp = int '\\' -> sb.Append('\\').Append(char cp) |> ignore
             | 0x09 -> sb.Append("\\t") |> ignore
             | 0x0a -> sb.Append("\\n") |> ignore
             | 0x0d -> sb.Append("\\r") |> ignore
             | _ when cp < 0x20 || cp = 0x7f -> sb.Append("\\x").Append(cp.ToString("x2")) |> ignore
             | _ when cp < 0x7f -> sb.Append(char cp) |> ignore
             | _ when printable cp -> sb.Append(s, i, w) |> ignore
             | _ when cp <= 0xff -> sb.Append("\\x").Append(cp.ToString("x2")) |> ignore
             | _ when cp <= 0xffff -> sb.Append("\\u").Append(cp.ToString("x4")) |> ignore
             | _ -> sb.Append("\\U").Append(cp.ToString("x8")) |> ignore)
            go (i + w)
    go 0
    sb.Append(quote).ToString()

/// repr(list of str).
let reprStrList (xs: string list) : string = "[" + (xs |> List.map reprStr |> String.concat ", ") + "]"

// ---------------------------------------------------------------- values (JSON as Python objects)

let private floatStr (x: float) = PyFloat.repr x

/// repr() of a JSON-loaded Python value.
let rec repr (j: Json) : string =
    match j with
    | JNull -> "None"
    | JBool b -> if b then "True" else "False"
    | JInt i -> i.ToString(CultureInfo.InvariantCulture)
    | JFloat x -> floatStr x
    | JStr s -> reprStr s
    | JArr xs -> "[" + (xs |> List.map repr |> String.concat ", ") + "]"
    | JObj kvs -> "{" + (kvs |> List.map (fun (k, v) -> reprStr k + ": " + repr v) |> String.concat ", ") + "}"

/// str() of a JSON-loaded Python value.
let str (j: Json) : string =
    match j with
    | JStr s -> s
    | _ -> repr j

/// str() of an optional value (a missing value is None).
let strOpt (j: Json option) : string = j |> Option.map str |> Option.defaultValue "None"
let reprOpt (j: Json option) : string = j |> Option.map repr |> Option.defaultValue "None"

/// Python truthiness.
let truthy (j: Json) : bool =
    match j with
    | JNull -> false
    | JBool b -> b
    | JInt i -> not i.IsZero
    | JFloat x -> x <> 0.0
    | JStr s -> s <> ""
    | JArr xs -> not xs.IsEmpty
    | JObj kvs -> not kvs.IsEmpty

let truthyOpt (j: Json option) : bool = j |> Option.exists truthy

/// `a or b`
let orElse (b: Json) (a: Json) : Json = if truthy a then a else b

/// d.get(k) on a dict value (None when absent or not a dict).
let get (k: string) (j: Json) : Json option =
    match j with
    | JObj kvs -> kvs |> List.tryFind (fst >> (=) k) |> Option.map snd
    | _ -> None

/// d.get(k, default)
let getOr (k: string) (def: Json) (j: Json) : Json = get k j |> Option.defaultValue def

/// d[k] (KeyError when absent).
let item (k: string) (j: Json) : Json =
    match get k j with
    | Some v -> v
    | None -> raise (Collections.Generic.KeyNotFoundException(reprStr k))

/// d[k] as str (the value must be a str).
let itemStr (k: string) (j: Json) : string =
    match item k j with
    | JStr s -> s
    | v -> raise (InvalidCastException($"{reprStr k} is not a str: {repr v}"))

/// d.get(k, "") rendered with str().
let getStr (k: string) (j: Json) : string = str (getOr k (JStr "") j)

let has (k: string) (j: Json) : bool = (get k j).IsSome

/// dict(d, k=v): existing keys keep their position; new keys are appended.
let set (k: string) (v: Json) (j: Json) : Json =
    match j with
    | JObj kvs when kvs |> List.exists (fst >> (=) k) -> JObj(kvs |> List.map (fun (k2, v2) -> if k2 = k then k2, v else k2, v2))
    | JObj kvs -> JObj(kvs @ [ k, v ])
    | _ -> failwith "set on a non-dict"

/// {**a, **b}: b's keys overwrite a's in place; new keys are appended in b's order.
let merge (a: Json) (b: Json) : Json =
    match b with
    | JObj kvs -> kvs |> List.fold (fun acc (k, v) -> set k v acc) a
    | JNull -> raise (InvalidCastException "'NoneType' object is not a mapping")
    | _ -> raise (InvalidCastException "not a mapping")

let keys (j: Json) : string list =
    match j with
    | JObj kvs -> List.map fst kvs
    | _ -> []

let items (j: Json) : (string * Json) list =
    match j with
    | JObj kvs -> kvs
    | _ -> []

/// list(x) for a list value.
let asList (j: Json) : Json list =
    match j with
    | JArr xs -> xs
    | JNull -> raise (InvalidCastException "'NoneType' object is not iterable")
    | JStr s -> s |> Seq.map (string >> JStr) |> List.ofSeq
    | JObj kvs -> kvs |> List.map (fst >> JStr)
    | v -> raise (InvalidCastException($"{repr v} is not iterable"))

let strs (j: Json) : string list = asList j |> List.map str

let jstrs (xs: string list) : Json = JArr(List.map JStr xs)

/// Python dict ordering helpers over (key, value) lists: last assignment wins, first position kept.
let toDict (pairs: ('k * 'v) list) : ('k * 'v) list =
    let last = pairs |> List.fold (fun (m: Map<'k, 'v>) (k, v) -> m.Add(k, v)) Map.empty
    pairs |> List.map fst |> List.distinct |> List.map (fun k -> k, last.[k])

// ---------------------------------------------------------------- paths

/// str(pathlib.PurePosixPath(...)) of a joined path: empty and "." components dropped, duplicate slashes collapsed.
let pathStr (parts: string list) : string =
    let joined = String.concat "/" (parts |> List.filter ((<>) ""))
    let absolute = parts |> List.tryFind ((<>) "") |> Option.exists (startsWith "/")
    let comps = joined.Split('/') |> Array.filter (fun c -> c <> "" && c <> ".") |> List.ofArray
    match absolute, comps with
    | true, _ -> "/" + String.concat "/" comps
    | false, [] -> "."
    | false, _ -> String.concat "/" comps

/// os.path.relpath(path, start) for two relative paths (resolved against the same directory).
let relpath (path: string) (start: string) : string =
    let norm (p: string) =
        p.Split('/') |> Array.filter (fun c -> c <> "" && c <> ".") |> Array.fold (fun acc c -> if c = ".." && not (List.isEmpty acc) && List.head acc <> ".." then List.tail acc else c :: acc) [] |> List.rev
    let p, s = norm path, norm start
    let rec common a b n =
        match a, b with
        | x :: xs, y :: ys when x = y -> common xs ys (n + 1)
        | _ -> n
    let n = common p s 0
    let rel = List.replicate (s.Length - n) ".." @ List.skip n p
    if rel.IsEmpty then "." else String.concat "/" rel

/// PurePosixPath(p).parent
let parent (p: string) : string =
    let s = pathStr [ p ]
    match s.LastIndexOf '/' with
    | -1 -> "."
    | 0 -> "/"
    | i -> s.Substring(0, i)

// ---------------------------------------------------------------- csv writer and hashing

/// csv.writer(buf, quoting=csv.QUOTE_ALL, lineterminator=lt) rows.
let csvQuoteAll (lineTerminator: string) (rows: string list list) : string =
    rows
    |> List.map (fun r -> (r |> List.map (fun s -> "\"" + s.Replace("\"", "\"\"") + "\"") |> String.concat ",") + lineTerminator)
    |> String.concat ""

/// csv.writer(buf, lineterminator=lt) rows with the default QUOTE_MINIMAL.
let csvQuoteMinimal (lineTerminator: string) (rows: string list list) : string =
    let special (s: string) = s.IndexOfAny [| ','; '"'; '\r'; '\n' |] >= 0 || lineTerminator |> Seq.exists (fun c -> s.Contains c)
    let cell (s: string) = if special s then "\"" + s.Replace("\"", "\"\"") + "\"" else s
    rows
    |> List.map (fun r ->
        (match r with
         | [ "" ] -> "\"\""
         | _ -> r |> List.map cell |> String.concat ",")
        + lineTerminator)
    |> String.concat ""

/// hashlib.sha256(s.encode("utf-8")).hexdigest()
let sha256Hex (s: string) : string =
    SHA256.HashData(Encoding.UTF8.GetBytes s) |> Array.map (fun b -> b.ToString("x2")) |> String.concat ""

/// f"{n:06d}" for n >= 0.
let pad6 (n: int) : string = n.ToString("000000", CultureInfo.InvariantCulture)

/// Python exceptions that the scripts raise on purpose (type name and message), reported as "<Type>: <message>".
exception PyError of string * string
