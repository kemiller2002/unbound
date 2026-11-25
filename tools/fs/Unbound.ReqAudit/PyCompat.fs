/// Python-compatibility primitives needed by the reqaudit port that Unbound.Py does not provide: str methods with Python's
/// whitespace and case rules, code-point lengths and slices, repr()/str() of JSON-loaded values, truthiness, ordered
/// Counter, Python `re` pattern translation, BOM-preserving UTF-8 reads and csv.DictReader rows as JSON objects.
module Unbound.ReqAudit.PyCompat

open System
open System.Globalization
open System.Numerics
open System.Text
open System.Text.RegularExpressions
open Unbound.Py
open Unbound.Py.PyJson

/// A Python exception that would end the program with a traceback (exit 1).
exception PyError of string

// ---------------------------------------------------------------- str

/// str.isspace() for one UTF-16 unit (Python adds U+001C..U+001F to .NET's set).
let isSpace (c: char) = Char.IsWhiteSpace c || (c >= '\u001c' && c <= '\u001f')

/// First index at or after i (scanning forward) where p fails, or s.Length.
let rec private skipFwd (p: char -> bool) (s: string) (i: int) = if i < s.Length && p s.[i] then skipFwd p s (i + 1) else i

/// Index one past the last character before b (scanning backward, not below lo) where p fails.
let rec private skipBack (p: char -> bool) (s: string) (lo: int) (b: int) = if b > lo && p s.[b - 1] then skipBack p s lo (b - 1) else b

/// str.strip() / rstrip() / lstrip() with no argument.
let strip (s: string) = let a = skipFwd isSpace s 0 in s.Substring(a, skipBack isSpace s a s.Length - a)
let rstrip (s: string) = s.Substring(0, skipBack isSpace s 0 s.Length)
let lstrip (s: string) = s.Substring(skipFwd isSpace s 0)

/// str.split() with no separator.
let split (s: string) : string list =
    let rec go i acc =
        let i = skipFwd isSpace s i
        if i >= s.Length then List.rev acc
        else
            let j = skipFwd (isSpace >> not) s i
            go j (s.Substring(i, j - i) :: acc)
    go 0 []

let private lineBreak (c: char) =
    match c with
    | '\n' | '\r' | '\u000b' | '\u000c' | '\u001c' | '\u001d' | '\u001e' | '\u0085' | ' ' | ' ' -> true
    | _ -> false

/// str.splitlines() (all of Python's line boundaries; "\r\n" counts as one).
let splitLines (s: string) : string list =
    let rec go start i acc =
        if i >= s.Length then List.rev (if start < s.Length then s.Substring start :: acc else acc)
        elif lineBreak s.[i] then
            let next = if s.[i] = '\r' && i + 1 < s.Length && s.[i + 1] = '\n' then i + 2 else i + 1
            go next next (s.Substring(start, i - start) :: acc)
        else go start (i + 1) acc
    go 0 0 []

let private isCased (c: char) =
    match Char.GetUnicodeCategory c with
    | UnicodeCategory.UppercaseLetter | UnicodeCategory.LowercaseLetter | UnicodeCategory.TitlecaseLetter -> true
    | _ -> false

let private isCaseIgnorable (c: char) =
    match Char.GetUnicodeCategory c with
    | UnicodeCategory.NonSpacingMark | UnicodeCategory.EnclosingMark | UnicodeCategory.Format
    | UnicodeCategory.ModifierLetter | UnicodeCategory.ModifierSymbol -> true
    | _ -> "'.:^`¨­¯´·¸‘’․‧︓﹒﹕＇．：".IndexOf c >= 0

/// str.lower(): .NET invariant lower-casing plus Python's full mappings that differ (U+0130 and final sigma).
let lower (s: string) : string =
    if s.IndexOf 'İ' < 0 && s.IndexOf 'Σ' < 0 then s.ToLowerInvariant()
    else
        let finalSigma (i: int) =
            let rec back j = if j < 0 then false elif isCaseIgnorable s.[j] then back (j - 1) else isCased s.[j]
            let rec fwd j = if j >= s.Length then false elif isCaseIgnorable s.[j] then fwd (j + 1) else isCased s.[j]
            back (i - 1) && not (fwd (i + 1))
        s
        |> Seq.mapi (fun i c ->
            match c with
            | 'İ' -> "i̇"
            | 'Σ' -> if finalSigma i then "ς" else "σ"
            | c -> string (Char.ToLowerInvariant c))
        |> String.concat ""

let upper (s: string) : string = s.ToUpperInvariant()

let private hasSurrogate (s: string) = s |> Seq.exists Char.IsSurrogate

/// len(s) in code points.
let len (s: string) : int =
    if hasSurrogate s then s |> Seq.filter (fun c -> not (Char.IsLowSurrogate c)) |> Seq.length else s.Length

/// s[:n] in code points (n >= 0).
let take (n: int) (s: string) : string =
    if not (hasSurrogate s) then (if n >= s.Length then s else s.Substring(0, n))
    else
        let rec go i k = if i >= s.Length || k = n then i else go (if Char.IsHighSurrogate s.[i] && i + 1 < s.Length then i + 2 else i + 1) (k + 1)
        s.Substring(0, go 0 0)

/// Python str comparison (code-point order).
let cmpStr (a: string) (b: string) : int =
    let n = min a.Length b.Length
    let fix (c: char) = if c >= '\uD800' && c <= '\uDFFF' then int c + 0x2000 elif c >= '' then int c - 0x800 else int c
    let rec go i =
        if i = n then compare a.Length b.Length
        elif a.[i] = b.[i] then go (i + 1)
        else compare (fix a.[i]) (fix b.[i])
    go 0

let sortStrings (xs: string seq) : string list = xs |> List.ofSeq |> List.sortWith cmpStr

/// Format spec "{:N}" for a str (left-aligned, padded to N code points).
let ljust (width: int) (s: string) = let l = len s in if l >= width then s else s + String(' ', width - l)

/// int(s) for a string of (possibly Unicode) decimal digits with optional sign and surrounding whitespace.
let pyInt (s: string) : BigInteger =
    let t = strip s
    let neg, body = if t.StartsWith '-' then true, t.Substring 1 elif t.StartsWith '+' then false, t.Substring 1 else false, t
    let body = body.Replace("_", "")
    if body = "" || not (body |> Seq.forall Char.IsDigit) then raise (PyError $"invalid literal for int() with base 10: '{s}'")
    let v = body |> Seq.fold (fun (acc: BigInteger) c -> acc * BigInteger 10 + BigInteger(int (Char.GetNumericValue c))) BigInteger.Zero
    if neg then -v else v

// ---------------------------------------------------------------- repr / str of loaded values

let private printable (cp: int) =
    if cp = 0x20 then true
    else
        let cat = CharUnicodeInfo.GetUnicodeCategory cp
        match cat with
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

/// repr() of a JSON-loaded value (dict, list, str, int, float, bool, None).
let rec repr (j: Json) : string =
    match j with
    | JNull -> "None"
    | JBool b -> if b then "True" else "False"
    | JInt i -> i.ToString(CultureInfo.InvariantCulture)
    | JFloat x -> PyFloat.repr x
    | JStr s -> reprStr s
    | JArr xs -> "[" + (xs |> List.map repr |> String.concat ", ") + "]"
    | JObj kvs -> "{" + (kvs |> List.map (fun (k, v) -> reprStr k + ": " + repr v) |> String.concat ", ") + "}"

/// str() of a JSON-loaded value.
let str (j: Json) : string =
    match j with
    | JStr s -> s
    | j -> repr j

/// repr() of a list of str.
let reprList (xs: string seq) = "[" + (xs |> Seq.map reprStr |> String.concat ", ") + "]"

/// bool(x).
let truthy (j: Json) : bool =
    match j with
    | JNull -> false
    | JBool b -> b
    | JInt i -> not i.IsZero
    | JFloat x -> x <> 0.0
    | JStr s -> s <> ""
    | JArr xs -> not xs.IsEmpty
    | JObj kvs -> not kvs.IsEmpty

/// `a or b`.
let orElse (b: Json) (a: Json) : Json = if truthy a then a else b

/// len(x).
let jlen (j: Json) : int =
    match j with
    | JStr s -> len s
    | JArr xs -> xs.Length
    | JObj kvs -> kvs.Length
    | j -> raise (PyError $"object of type has no len(): {repr j}")

/// Iterating a value as Python does (list items, str characters, dict keys).
let iter (j: Json) : Json list =
    match j with
    | JArr xs -> xs
    | JStr s ->
        let rec go i acc =
            if i >= s.Length then List.rev acc
            elif Char.IsHighSurrogate s.[i] && i + 1 < s.Length then go (i + 2) (JStr(s.Substring(i, 2)) :: acc)
            else go (i + 1) (JStr(string s.[i]) :: acc)
        go 0 []
    | JObj kvs -> kvs |> List.map (fst >> JStr)
    | j -> raise (PyError $"object is not iterable: {repr j}")

/// Python == between loaded values (numbers compare by value; bool is an int).
let rec pyEq (a: Json) (b: Json) : bool =
    let num j =
        match j with
        | JBool b -> Some(if b then 1.0 else 0.0)
        | JInt i -> Some(float i)
        | JFloat x -> Some x
        | _ -> None
    match a, b with
    | JInt x, JInt y -> x = y
    | (JInt _ | JFloat _ | JBool _), (JInt _ | JFloat _ | JBool _) -> num a = num b
    | JStr x, JStr y -> x = y
    | JNull, JNull -> true
    | JArr xs, JArr ys -> xs.Length = ys.Length && List.forall2 pyEq xs ys
    | JObj xs, JObj ys ->
        xs.Length = ys.Length && xs |> List.forall (fun (k, v) -> ys |> List.exists (fun (k2, v2) -> k = k2 && pyEq v v2))
    | _ -> false

/// Python ordering between loaded values (str with str, numbers with numbers); other pairs raise TypeError.
let rec cmpJson (a: Json) (b: Json) : int =
    match a, b with
    | JStr x, JStr y -> cmpStr x y
    | JInt x, JInt y -> compare x y
    | (JInt _ | JFloat _ | JBool _), (JInt _ | JFloat _ | JBool _) ->
        let f j = match j with JInt i -> float i | JFloat x -> x | JBool b -> (if b then 1.0 else 0.0) | _ -> 0.0
        compare (f a) (f b)
    | JArr xs, JArr ys ->
        let rec go xs ys =
            match xs, ys with
            | [], [] -> 0
            | [], _ -> -1
            | _, [] -> 1
            | x :: xr, y :: yr -> (match cmpJson x y with 0 -> go xr yr | c -> c)
        go xs ys
    | _ -> raise (PyError $"'<' not supported between {repr a} and {repr b}")

/// Lexicographic comparison of tuples of loaded values.
let cmpTuple (xs: Json list) (ys: Json list) : int = cmpJson (JArr xs) (JArr ys)

let sortJson (xs: Json seq) : Json list = xs |> List.ofSeq |> List.sortWith cmpJson

// ---------------------------------------------------------------- dict access

/// d.get(k) (None when absent).
let get (k: string) (j: Json) : Json =
    match j with
    | JObj kvs -> (match kvs |> List.tryFind (fun (k2, _) -> k2 = k) with Some(_, v) -> v | None -> JNull)
    | _ -> raise (PyError $"'{repr j}' has no attribute 'get'")

/// d.get(k, default).
let getOr (k: string) (dflt: Json) (j: Json) : Json =
    match j with
    | JObj kvs -> (match kvs |> List.tryFind (fun (k2, _) -> k2 = k) with Some(_, v) -> v | None -> dflt)
    | _ -> raise (PyError $"'{repr j}' has no attribute 'get'")

let has (k: string) (j: Json) : bool =
    match j with
    | JObj kvs -> kvs |> List.exists (fun (k2, _) -> k2 = k)
    | _ -> false

/// d[k] (KeyError when absent).
let at (k: string) (j: Json) : Json =
    match j with
    | JObj kvs -> (match kvs |> List.tryFind (fun (k2, _) -> k2 = k) with Some(_, v) -> v | None -> raise (PyError $"KeyError: {reprStr k}"))
    | _ -> raise (PyError $"'{repr j}' is not subscriptable")

/// d[k] where the value is used as a str (str() of the value).
let s (k: string) (j: Json) : string = str (at k j)

/// A value used where Python calls str methods on it: must be a str.
let asStr (j: Json) : string =
    match j with
    | JStr s -> s
    | j -> raise (PyError $"'{repr j}' is not a str")

let jstr (s: string) = JStr s
let jint (n: int) = JInt(BigInteger n)

/// dict.fromkeys(xs): first occurrence order.
let distinct (xs: 'a list) : 'a list = List.distinct xs

// ---------------------------------------------------------------- Counter

/// Counter(xs).items() in insertion order.
let counter (xs: 'k seq) : ('k * int) list when 'k: comparison =
    let counts, order =
        xs
        |> Seq.fold
            (fun (m: Map<'k, int>, order: 'k list) x ->
                match m.TryFind x with
                | Some n -> m.Add(x, n + 1), order
                | None -> m.Add(x, 1), x :: order)
            (Map.empty, [])
    order |> List.rev |> List.map (fun k -> k, counts.[k])

/// c[k] for a Counter given as items.
let count (items: ('k * int) list) (k: 'k) : int =
    match items |> List.tryFind (fun (x, _) -> x = k) with
    | Some(_, n) -> n
    | None -> 0

/// sorted(c.items(), key=lambda kv: -kv[1]) / most_common(): stable by descending count.
let byCountDesc (items: ('k * int) list) : ('k * int) list = items |> List.sortWith (fun (_, a) (_, b) -> compare b a)

// ---------------------------------------------------------------- re

let private word = @"\p{L}\p{N}_"
let private space = @"\s\x1c-\x1f"

/// Translates a Python `re` pattern to .NET syntax with Python's Unicode \w, \s and \b semantics.
let translate (p: string) : string =
    let sb = StringBuilder()
    let rec go i inClass =
        if i < p.Length then
            let c = p.[i]
            if c = '\\' && i + 1 < p.Length then
                let e = p.[i + 1]
                match e, inClass with
                | 'w', false -> sb.Append("[" + word + "]") |> ignore
                | 'W', false -> sb.Append("[^" + word + "]") |> ignore
                | 's', false -> sb.Append("[" + space + "]") |> ignore
                | 'S', false -> sb.Append("[^" + space + "]") |> ignore
                | 'b', false -> sb.Append($"(?:(?<=[{word}])(?![{word}])|(?<![{word}])(?=[{word}]))") |> ignore
                | 'B', false -> sb.Append($"(?:(?<=[{word}])(?=[{word}])|(?<![{word}])(?![{word}]))") |> ignore
                | 'Z', false -> sb.Append(@"\z") |> ignore
                | 'w', true -> sb.Append(word) |> ignore
                | 's', true -> sb.Append(space) |> ignore
                | ('W' | 'S'), true -> failwith "negated class escape inside a character class is not supported"
                | _ -> sb.Append(c).Append(e) |> ignore
                go (i + 2) inClass
            elif not inClass && c = '[' then
                sb.Append c |> ignore
                // a ']' right after '[' or '[^' is literal
                let j = if i + 1 < p.Length && p.[i + 1] = '^' then (sb.Append '^' |> ignore; i + 2) else i + 1
                if j < p.Length && p.[j] = ']' then (sb.Append @"\]" |> ignore; go (j + 1) true) else go j true
            elif inClass && c = '[' then
                sb.Append @"\[" |> ignore
                go (i + 1) true
            elif inClass && c = ']' then
                sb.Append c |> ignore
                go (i + 1) false
            elif not inClass && c = '(' && p.Substring(i).StartsWith "(?P<" then
                sb.Append "(?<" |> ignore
                go (i + 4) false
            elif not inClass && c = '(' && p.Substring(i).StartsWith "(?P=" then
                let close = p.IndexOf(')', i)
                sb.Append(@"\k<" + p.Substring(i + 4, close - i - 4) + ">") |> ignore
                go (close + 1) false
            else
                sb.Append c |> ignore
                go (i + 1) inClass
    go 0 false
    sb.ToString()

type Re = { Search: Regex; Match: Regex; Full: Regex }

/// re.compile(pattern, flags): Search is re.search/finditer/sub, Match is re.match, Full is re.fullmatch.
let rx (opts: RegexOptions) (pattern: string) : Re =
    let t = translate pattern
    let o = opts ||| RegexOptions.CultureInvariant
    { Search = Regex(t, o); Match = Regex(@"\A(?:" + t + ")", o); Full = Regex(@"\A(?:" + t + @")\z", o) }

let re (pattern: string) = rx RegexOptions.None pattern
let reI (pattern: string) = rx RegexOptions.IgnoreCase pattern

let search (r: Re) (s: string) : Match option = let m = r.Search.Match s in if m.Success then Some m else None
let isSearch (r: Re) (s: string) = r.Search.IsMatch s
let isMatch (r: Re) (s: string) = r.Match.IsMatch s
let pmatch (r: Re) (s: string) : Match option = let m = r.Match.Match s in if m.Success then Some m else None
let isFull (r: Re) (s: string) = r.Full.IsMatch s
let finditer (r: Re) (s: string) : Match list = r.Search.Matches s |> Seq.toList

/// re.findall: whole matches without groups, group 1 with one group.
let findall (r: Re) (s: string) : string list =
    let groups = r.Search.GetGroupNumbers().Length - 1
    finditer r s |> List.map (fun m -> if groups = 0 then m.Value else m.Groups.[1].Value)

/// re.sub with a literal replacement.
let subLit (r: Re) (replacement: string) (s: string) : string = r.Search.Replace(s, MatchEvaluator(fun _ -> replacement))

/// re.sub with a function.
let subFn (r: Re) (f: Match -> string) (s: string) : string = r.Search.Replace(s, MatchEvaluator f)

/// m.group(n), "" when the group did not participate (callers that need None test .Success).
let grp (n: int) (m: Match) : string = m.Groups.[n].Value

// ---------------------------------------------------------------- files

let private utf8 = UTF8Encoding(false)

/// open(p, encoding="utf-8").read() / Path.read_text(): BOM kept as U+FEFF, universal newlines applied.
let readText (path: string) : string = utf8.GetString(IO.File.ReadAllBytes path) |> PyCsv.universalNewlines

/// read_text(encoding="utf-8", errors="replace"): invalid bytes become U+FFFD.
let readTextReplace (path: string) : string = readText path

/// Path.write_text(text, encoding="utf-8").
let writeText (path: string) (text: string) : unit = IO.File.WriteAllBytes(path, utf8.GetBytes text)

let exists (path: string) = IO.File.Exists path || IO.Directory.Exists path

/// csv.DictReader over a file opened in text mode: each row as an ordered dict of str (None for missing fields).
let dictRows (text: string) : Json list =
    let rows = PyCsv.parse text |> List.filter (fun r -> r <> [] && r <> [ "" ])
    match rows with
    | [] -> []
    | header :: rest ->
        let dedupe (pairs: (string * Json) list) =
            let last = pairs |> List.fold (fun (m: Map<string, Json>) (k, v) -> m.Add(k, v)) Map.empty
            pairs |> List.map fst |> List.distinct |> List.map (fun k -> k, last.[k])
        rest |> List.map (fun r -> header |> List.mapi (fun i h -> h, (if i < r.Length then JStr r.[i] else JNull)) |> dedupe |> JObj)

/// tuple(json.loads(l) for l in p.read_text().splitlines() if l.strip()).
let loadJsonlText (text: string) : Json list = splitLines text |> List.filter (fun l -> strip l <> "") |> List.map loads

/// Path parts order used by sorted() on pathlib paths.
let cmpPath (a: string) (b: string) : int =
    let pa = a.Split '/' |> List.ofArray
    let pb = b.Split '/' |> List.ofArray
    let rec go xs ys =
        match xs, ys with
        | [], [] -> 0
        | [], _ -> -1
        | _, [] -> 1
        | x :: xr, y :: yr -> (match cmpStr x y with 0 -> go xr yr | c -> c)
    go pa pb

/// sorted(dir.glob(pattern)) for a single-segment pattern: full paths of matching entries (files and directories).
let glob (dir: string) (pattern: string) : string list =
    if not (IO.Directory.Exists dir) then []
    else
        let rxp = Regex("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$")
        IO.Directory.EnumerateFileSystemEntries dir
        |> Seq.filter (fun p -> rxp.IsMatch(IO.Path.GetFileName p))
        |> Seq.map (fun p -> IO.Path.Combine(dir, IO.Path.GetFileName p))
        |> List.ofSeq
        |> List.sortWith cmpPath

/// sorted(dir.rglob(pattern)) for files: recursive, not following directory symlinks.
let rglobFiles (dir: string) (pattern: string) : string list =
    let rxp = Regex("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$")
    let rec walk (d: string) : string seq =
        seq {
            for e in IO.Directory.EnumerateFileSystemEntries d do
                let name = IO.Path.GetFileName e
                let info = IO.FileInfo e
                let isDir = IO.Directory.Exists e
                let isLink = info.LinkTarget <> null
                if rxp.IsMatch name && IO.File.Exists e then yield IO.Path.Combine(d, name)
                if isDir && not isLink then yield! walk (IO.Path.Combine(d, name))
        }
    if IO.Directory.Exists dir then walk dir |> List.ofSeq |> List.sortWith cmpPath else []

/// Path(p).resolve() for an existing or new path (no symlink resolution needed in this repository).
let resolve (p: string) : string = IO.Path.GetFullPath p |> fun s -> if s.Length > 1 then s.TrimEnd('/') else s

let combine (a: string) (b: string) = IO.Path.Combine(a, b)

/// Path.name.
let name (p: string) = IO.Path.GetFileName(p.TrimEnd('/'))
