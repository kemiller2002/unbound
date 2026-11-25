/// inventory.py: requirements inventory extractor. files -> lines -> definition/reference records -> CSV/JSON.
/// Scans the audited baseline revision through git by default (REQAUDIT_REV=WORKTREE scans the working tree).
module Unbound.ReqAudit.Inventory

open System
open System.Security.Cryptography
open System.Text
open System.Text.RegularExpressions
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

let ID = @"[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-[A-Z]?\d{1,4}[A-Za-z]?(?:-Q)?"
let ID_RE = re $@"\b({ID})\b"
let BULLET_RE = re $@"^\s*[-*]\s+(?:\*\*({ID})\b[^*]*\*\*|({ID})\s*:)\s*:?\s*(.*)$"
let LINE_RE = re $@"^(?:\*\*)?({ID})(?:\s*\([^)]*\))?:(?:\*\*)?\s+(\S.*)$"
let HEADING_RE = re (@"^(#{1,6})\s+(" + ID + @")\s*[:.—-]?\s*(.*)$")
let TABLE_RE = re $@"^\|\s*`?({ID})`?\s*\|(.*)$"
let CATALOG_RE = re $@"^({ID})\s+([A-Z][A-Za-z0-9]+.*)$"
let ID_FULL = re ID

let BASELINE = "777a70d27a70e7e6851560ed513a872b5b4d134b"

let SCOPE_GLOBS =
    [ "requirements/**/*.md"; "requirements/**/*.csv"; "architecture/**/*.md"
      "research/daf-theoretical-enterprise-validation/**/*.md"; "research/daf-theoretical-enterprise-validation/**/*.csv" ]

let REFERENCE_ONLY_CSV =
    [ "REQUIREMENT-TO-CONTROL-CROSSWALK-WAVE-01.csv"; "DETAILED-REQUIREMENT-CONTROL-LINKS-WAVE-02.csv"
      "ENTERPRISE-CAPABILITY-TO-CONTROL-CROSSWALK.csv"; "WORKFLOW-SIMULATION-RECORD-TEMPLATE.csv" ]

let EXCLUDE_PREFIXES = [ "requirements/reconciliation/" ]

let KIND_BY_FAMILY : Map<string, string> =
    let kinds (kind: string) (fams: string list) = fams |> List.map (fun f -> f, kind)
    Map.ofList (
        kinds "requirement"
            [ "UNB-ENT"; "UNB-LEG"; "UNB-DON"; "UNB-GRT"; "UNB-AST"; "UNB-FIN"; "UNB-ADV"; "UNB-AIO"
              "UNB-OPS"; "UNB-TEC"; "UNB-EXP"; "UNB-VAL"; "UNB-ACC"; "UNB-NAV"; "UNB-INV"; "UNB-GRANT"
              "UNB-GEX"; "UNB-GFUND"; "UNB-CHAR"; "UNB-EXT"; "UNB-RULE"; "UNB-CUST"; "UNB-TEN"; "UNB-SUCC"
              "UNB-PLAT"; "UNB-KNOW"; "UNB-WF"; "UNB-DATA"; "UNB-LS"; "UNB-REM"; "UNB-IMM"; "UNB-XSC"
              "UNB-PAY"; "UNB-PART"; "UNB-API"; "UNB-UX"; "UNB-GOV"; "UNB-ECO"; "UNB-EVT"; "UNB-TRACE"
              "UNB-DOC"; "UNB-COM"; "UNB-AGT"; "UNB-CAP"; "UNB-RES"; "UNB-REGMAP"; "UNB-API-STD"
              "UNB-AI-STD"; "UNB-TEC-STRATA"; "UNB-GAP"; "UNB-CTL"; "UNB-HYP"
              "REQ-X"; "ADM"; "AIN"; "MON"; "OPS"; "INT"; "TVR-GR"; "TVR-ORG"; "TVR-FIN"; "TVR-PAY"
              "TVR-SEC"; "TVR-UX"; "TVR-WID"; "TVR-DATA"; "TVR-CMP"; "TVR-OPS"; "TVR-AI"; "TVR-CHR"; "UX"; "ACC-MDL"; "EXC"; "AUTH"
              "TVR-CON"; "TVR-GOV"; "TVR-INV"; "TVR-ACC"; "GOV-AUTH"; "SOC2"; "UX-S"; "WG" ]
        @ kinds "control" [ "UNB-CTRL"; "UNB-SCTRL"; "CTL"; "TVCTL"; "VAL" ]
        @ kinds "test" [ "ADM-T"; "CCM-T"; "LIFE-T"; "UI-T"; "GOV-T" ]
        @ kinds "scenario" [ "SIM"; "TVS"; "SC"; "SCN"; "XSIM"; "ADV"; "UNB-XSC" ]
        @ kinds "event" [ "GR-EVT" ]
        @ kinds "finding" [ "TVF"; "GAP" ]
        @ kinds "decision" [ "OD"; "ACC-ADR"; "NAV-ADR"; "GRANT-ADR"; "UNB-CHAR-DEC"; "UNB-DATA-DEC" ]
        @ kinds "contradiction" [ "TVC" ]
        @ kinds "assumption" [ "TVA" ]
        @ kinds "regulatory-assertion" [ "REG" ]
        @ kinds "failure-mode" [ "FM"; "FMEA" ]
        @ kinds "invariant" [ "FI"; "INV"; "CONC" ]
        @ kinds "workflow" [ "WF"; "CW" ])

type Record =
    { Identifier: string; Family: string; Kind: string; Site: string; Path: string; Line: int; Blob: string; Heading: string; Text: string; TextSha: string }

let FIELDS = [ "identifier"; "family"; "kind"; "site"; "path"; "line"; "blob"; "heading"; "text"; "text_sha" ]

let private qSuffix = re "-Q$"
let private famSuffix = re @"-[A-Z]?\d{1,4}[A-Za-z]?$"
let private testSuffix = re @"-T\d+$"

let familyOf (identifier: string) = subLit famSuffix "" (subLit qSuffix "" identifier)

let kindOf (identifier: string) =
    let fam = familyOf identifier
    if isSearch testSuffix identifier then "test"
    elif fam.EndsWith("ADR", StringComparison.Ordinal) || fam.StartsWith("ADR", StringComparison.Ordinal) || fam.EndsWith("-DEC", StringComparison.Ordinal) then "decision"
    elif fam.StartsWith("CTRL", StringComparison.Ordinal) then "control"
    else KIND_BY_FAMILY.TryFind fam |> Option.defaultValue "other"

let private nonAlnum = re "[^a-z0-9]+"

let normalize (text: string) = strip (subLit nonAlnum " " (lower text))

let sha (text: string) =
    (SHA256.HashData(UTF8Encoding(false).GetBytes(normalize text)) |> Array.map (fun b -> b.ToString "x2") |> String.concat "").Substring(0, 16)

/// Nearest preceding heading for each line (the heading line itself gets the previous heading).
let headingsByLine (lines: string list) : string list =
    lines
    |> List.fold
        (fun (out, current) (line: string) -> current :: out, (if line.StartsWith '#' then strip (line.TrimStart('#')) else current))
        ([], "")
    |> fst
    |> List.rev

/// Heading definitions take the heading plus the following block up to the next heading.
let paragraphAfter (lines: string[]) (index: int) : string =
    let tail = lines |> Array.skip (min lines.Length (index + 1)) |> Array.truncate 12
    tail |> Array.takeWhile (fun l -> not (l.StartsWith '#')) |> Array.filter (fun l -> strip l <> "") |> Array.map strip |> String.concat " "

let private words3 = re "[A-Za-z]{3,}"

/// A bullet whose body is essentially a list of identifiers is a cross-reference, not a definition.
let isReferenceList (text: string) =
    let body = subLit ID_RE "" (match text.IndexOf ':' with -1 -> text | i -> text.Substring(i + 1))
    (findall words3 body).Length < 3

let tokens (text: string) : Set<string> = split (normalize (subLit ID_RE " " text)) |> List.filter (fun w -> len w > 3) |> Set.ofList

let similarity (a: string) (b: string) : float =
    let ta, tb = tokens a, tokens b
    if ta.IsEmpty || tb.IsEmpty then 1.0 else float (Set.intersect ta tb).Count / float (min ta.Count tb.Count)

let collisionClass (recs: Record list) : string =
    if (recs |> List.map (fun r -> r.TextSha) |> List.distinct).Length < 2 then "identical"
    else
        let arr = Array.ofList recs
        let pairs = [ for i in 0 .. arr.Length - 1 do for j in i + 1 .. arr.Length - 1 do yield arr.[i], arr.[j] ]
        if pairs |> List.exists (fun (a, b) -> a.Path = b.Path && a.Site = "heading" && b.Site = "heading" && a.TextSha <> b.TextSha) then "same-file-redefinition"
        else
            let divergent = pairs |> List.filter (fun (a, b) -> similarity a.Text b.Text < 0.34)
            if divergent.IsEmpty then "mirror-or-summary"
            elif divergent |> List.exists (fun (a, b) -> a.Path = b.Path) then "same-file-redefinition"
            else "cross-file-divergent-needs-review"

let private tableRule = re @"^\|\s*-"

let mdRecords (rel: string) (content: string) (blob: string) : Record list =
    let lines = splitLines content |> Array.ofList
    let heads = headingsByLine (List.ofArray lines) |> Array.ofList
    let mk i ident site text = { Identifier = ident; Family = familyOf ident; Kind = kindOf ident; Site = site; Path = rel; Line = i + 1; Blob = blob; Heading = heads.[i]; Text = text; TextSha = sha text }
    lines
    |> Array.mapi (fun i line ->
        let b, h, t, c = pmatch BULLET_RE line, pmatch HEADING_RE line, pmatch TABLE_RE line, pmatch CATALOG_RE line
        match b, h, t with
        | Some bm, _, _ when not (isReferenceList line) && not (rel.Contains "/13-adversarial-reviews/raw/") ->
            Some(mk i (if bm.Groups.[1].Success && bm.Groups.[1].Value <> "" then bm.Groups.[1].Value else bm.Groups.[2].Value) "bullet" (strip line))
        | _, Some hm, _ -> Some(mk i (grp 2 hm) "heading" (strip $"{strip line} {paragraphAfter lines i}"))
        | _, _, Some tm when not (isMatch tableRule line) -> Some(mk i (grp 1 tm) "table" (strip line))
        | _ ->
            match pmatch LINE_RE line, c with
            | Some lm, _ -> Some(mk i (grp 1 lm) "line" (strip line))
            | None, Some cm -> Some(mk i (grp 1 cm) "catalog" (strip line))
            | None, None -> None)
    |> Array.choose id
    |> List.ofArray

let csvRecords (rel: string) (content: string) (blob: string) : Record list =
    if List.contains (IO.Path.GetFileName rel) REFERENCE_ONLY_CSV then []
    else
        let rows = PyCsv.parse content |> List.map (fun r -> if r = [ "" ] then [] else r)
        match rows with
        | [] -> []
        | header :: _ ->
            let headerText = String.Join(",", header)
            rows
            |> List.indexed
            |> List.skip 1
            |> List.choose (fun (i, row) ->
                let first = match row with [] -> "" | x :: _ -> (strip x).Trim('"')
                if not (isFull ID_FULL first) then None
                else
                    let text = String.Join(" | ", row |> List.map strip)
                    Some { Identifier = first; Family = familyOf first; Kind = kindOf first; Site = "csv"; Path = rel; Line = i + 1; Blob = blob
                           Heading = take 120 headerText; Text = text; TextSha = sha (String.Join(" ", row |> List.skip (min 1 row.Length) |> List.truncate 2)) })

/// The revision scanned and how files are read.
type Source =
    { Rev: string
      ListFiles: unit -> string list
      ReadFile: string -> string
      Blob: string -> string }

let private globRegex (g: string) =
    Regex("^" + Regex.Escape(g).Replace(@"\*\*/", "(?:.*/)?").Replace(@"\*", "[^/]*") + "$")

let gitSource (root: string) (rev: string) : Source =
    let pats = SCOPE_GLOBS |> List.map globRegex
    let blobRev = if rev = "WORKTREE" then "HEAD" else rev
    let blob (rel: string) = let r = Io.run "git" [ "-C"; root; "rev-parse"; $"{blobRev}:{rel}" ] in if r.Code = 0 then strip r.Out else "untracked"
    if rev = "WORKTREE" then
        { Rev = rev
          ListFiles =
            fun () ->
                let all = rglobFiles root "*" |> List.map (fun p -> IO.Path.GetRelativePath(root, p))
                all |> List.filter (fun f -> pats |> List.exists (fun p -> p.IsMatch f))
          ReadFile = fun rel -> readTextReplace (combine root rel)
          Blob = blob }
    else
        { Rev = rev
          ListFiles =
            fun () ->
                splitLines (Io.run "git" [ "-C"; root; "ls-tree"; "-r"; "--name-only"; rev ]).Out
                |> List.filter (fun f -> pats |> List.exists (fun p -> p.IsMatch f))
          ReadFile = fun rel -> (Io.gitShow root rev rel).Out
          Blob = blob }

let corpusFiles (src: Source) : string list =
    src.ListFiles () |> List.distinct |> List.filter (fun f -> not (EXCLUDE_PREFIXES |> List.exists (fun p -> f.StartsWith(p, StringComparison.Ordinal)))) |> sortStrings

let private csvText (rows: string list list) = PyCsv.format PyCsv.QuoteMinimal rows

let main (argv: string list) : int =
    let root, out = resolve argv.[0], resolve argv.[1]
    IO.Directory.CreateDirectory out |> ignore
    let rev = match Environment.GetEnvironmentVariable "REQAUDIT_REV" with null -> BASELINE | v -> v
    let src = gitSource root rev
    let files = corpusFiles src
    let contents = files |> List.map (fun rel -> rel, src.ReadFile rel)
    let records =
        contents
        |> List.collect (fun (rel, content) ->
            let blob = src.Blob rel
            if rel.EndsWith(".csv", StringComparison.Ordinal) then csvRecords rel content blob else mdRecords rel content blob)
    let refs = counter (contents |> List.collect (fun (_, content) -> finditer ID_RE content |> List.map (grp 1)))
    let refCount = Map.ofList refs
    let grouped = records |> List.groupBy (fun r -> r.Identifier) |> List.sortWith (fun (a, _) (b, _) -> cmpStr a b)
    let recordRow (r: Record) = [ r.Identifier; r.Family; r.Kind; r.Site; r.Path; string r.Line; r.Blob; r.Heading; r.Text; r.TextSha ]
    writeText (combine out "inventory.csv") (csvText (FIELDS :: (records |> List.map recordRow)))
    writeText (combine out "id-index.csv")
        (csvText (
            [ "identifier"; "family"; "kind"; "definition_sites"; "distinct_texts"; "reference_count"; "first_path"; "first_line" ]
            :: (grouped
                |> List.map (fun (k, v) ->
                    let h = v.Head
                    [ k; h.Family; h.Kind; string v.Length; string (v |> List.map (fun r -> r.TextSha) |> List.distinct).Length
                      string (refCount.TryFind k |> Option.defaultValue 0); h.Path; string h.Line ]))))
    let classes = grouped |> List.map (fun (k, v) -> k, collisionClass v)
    let classOf = Map.ofList classes
    writeText (combine out "duplicate-ids.csv")
        (csvText (
            [ "identifier"; "kind"; "collision_class"; "path"; "line"; "site"; "text" ]
            :: (grouped
                |> List.filter (fun (k, v) -> classOf.[k] <> "identical" && v.Length > 1)
                |> List.collect (fun (k, v) -> v |> List.map (fun r -> [ r.Identifier; r.Kind; classOf.[k]; r.Path; string r.Line; r.Site; take 300 r.Text ])))))
    let defined = grouped |> List.map fst |> Set.ofList
    let undefined = refs |> List.filter (fun (i, _) -> not (defined.Contains i)) |> List.sortWith (fun (a, _) (b, _) -> cmpStr a b)
    writeText (combine out "undefined-references.csv")
        (csvText ([ "identifier"; "family"; "kind"; "reference_count" ] :: (undefined |> List.map (fun (i, n) -> [ i; familyOf i; kindOf i; string n ]))))
    let sortedCounts (xs: string list) = JObj(counter xs |> List.sortWith (fun (a, _) (b, _) -> cmpStr a b) |> List.map (fun (k, n) -> k, jint n))
    let multiply = grouped |> List.filter (fun (_, v) -> v.Length > 1) |> List.map (fun (k, _) -> classOf.[k])
    let identical =
        Io.runInherit "git" [ "-C"; root; "diff"; "--quiet"; BASELINE; "HEAD"; "--"; "requirements"; "architecture"; "research"; ":!requirements/reconciliation" ] = 0
    let summary =
        JObj
            [ "scanned_revision", JStr rev
              "scanned_commit", JStr(strip (Io.run "git" [ "-C"; root; "rev-parse"; "HEAD" ]).Out)
              "baseline_revision", JStr BASELINE
              "originals_identical_to_baseline", JBool identical
              "files_scanned", jint (corpusFiles src).Length
              "definition_sites", jint records.Length
              "distinct_identifiers", jint grouped.Length
              "distinct_identifiers_by_kind", sortedCounts (grouped |> List.map (fun (_, v) -> v.Head.Kind))
              "distinct_identifiers_by_family", sortedCounts (grouped |> List.map (fun (_, v) -> v.Head.Family))
              "multiply_defined_by_class", JObj(counter multiply |> List.map (fun (k, n) -> k, jint n))
              "true_collisions",
              JArr(classes |> List.filter (fun (_, c) -> c = "same-file-redefinition" || c = "cross-file-divergent-needs-review") |> List.map fst |> sortStrings |> List.map JStr)
              "undefined_referenced_identifiers", jint undefined.Length ]
    Io.writeJsonReport (combine out "summary.json") summary
    Io.print (Io.dumps2 (match summary with JObj kvs -> JObj(kvs |> List.filter (fun (k, _) -> k <> "distinct_identifiers_by_family")) | j -> j))
    0
