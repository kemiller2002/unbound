/// adopt.py: adopt reconciliation records into the domain requirement files. Originals are annotated (never changed);
/// each target file gets one regenerated section between BEGIN/END ADOPTED markers. Idempotent.
module Unbound.ReqAudit.Adopt

open System
open System.Text.RegularExpressions
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

let RECON = "requirements/reconciliation/2026-11-23"
let BEGIN, END = "<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->", "<!-- END ADOPTED: reconciliation-2026-11-23 -->"
let NOTE = "Adopted 2026-11-23"

let DOMAIN_FILE =
    Map.ofList
        [ "GRT", "requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md"
          "FIN", "requirements/FULL-ACCOUNTING-SYSTEM.md"
          "PLT", "requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md"
          "ARC", "requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md"
          "ENT", "requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md"
          "REG", "requirements/charities/JURISDICTION-AWARE-REGULATORY-RULES-AND-ELIGIBILITY.md"
          "WF", "requirements/ENTERPRISE-GAP-REGISTER-AND-CROSS-DOMAIN-SCENARIOS.md" ]

let ENT_GOVERNANCE = [ "UNB-RCN-ENT-001"; "UNB-RCN-ENT-002"; "UNB-RCN-ENT-003" ]
let ID = re @"\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-[A-Z]?\d{1,4}(?:-r\d+)?\b"

let private domainFile (k: string) = match DOMAIN_FILE.TryFind k with Some f -> f | None -> raise (PyError $"KeyError: {reprStr k}")

let norm (t: string) : string = Validate.normalize t

let primaryDomain (tag: string) : string = Regex.Split((if String.IsNullOrEmpty tag then "PLT" else tag), "[+/]").[0]

let linkedDecisions (recordIds: string list) (decisions: Json list) : Json list =
    let ids = Set.ofList recordIds
    decisions |> List.filter (fun d -> findall ID (s "affected_ids" d) |> List.exists ids.Contains)

type Status = string * string * string

let statusOf (recordIds: string list) (decisions: Json list) : Status =
    let linked = linkedDecisions recordIds decisions
    let prov = linked |> List.filter (fun d -> s "status" d = "decided-provisional")
    let refs = match linked |> List.map (s "decision_id") with [] -> "—" | xs -> String.Join(", ", xs)
    if not prov.IsEmpty then "adopted-provisional", refs, String.Join("; ", prov |> List.map (fun d -> $"""{s "decision_id" d}: {s "validation_required" d}"""))
    else "adopted", refs, ""

let targetForGap (g: Json) (inventoryPaths: Map<string, string>) : string =
    if List.contains (s "id" g) ENT_GOVERNANCE then "requirements/BASELINE-001.md"
    else
        let candidates = (iter (getOr "links" (JArr []) g) |> List.map asStr) @ findall ID (asStr (getOr "parent" (JStr "") g))
        candidates
        |> List.tryPick (fun l ->
            let p = inventoryPaths.TryFind l |> Option.defaultValue ""
            if p.StartsWith("requirements/", StringComparison.Ordinal) && p.EndsWith(".md", StringComparison.Ordinal) && not (p.Contains "reconciliation") then Some p else None)
        |> Option.defaultWith (fun () -> domainFile ((s "id" g).Split('-').[2]))

let targetForRefinement (r: Json) : string =
    let p = s "original_path" r
    if p.StartsWith("requirements/", StringComparison.Ordinal) then p else domainFile (primaryDomain (asStr (getOr "_domain" (JStr "PLT") r)))

let acLines (acs: Json) : string list =
    iter acs |> List.map (fun a -> $"""  - {s "id" a}: Given {s "given" a}; when {s "when" a}; then {s "then" a}.""")

let renderRefinement (r: Json) ((st, decs, v): Status) : string list =
    [ yield $"""### {s "refinement_id" r} ({s "relationship" r} `{s "original_id" r}`) — {st}"""
      yield $"""- **Original:** `{s "original_path" r}:{s "original_line" r}` @ `777a70d` (kept verbatim) · **Decisions:** {decs}"""
      yield $"""- **Requirement:** {s "statement" r}"""
      if v <> "" then yield $"- **Validation still required:** {v}"
      yield "- **Acceptance criteria:**"
      yield! acLines (at "acceptance_criteria" r)
      yield $"""- Full record: `{RECON}/data/refinements.jsonl` ({s "refinement_id" r}); report `{RECON}/07-REFINEMENT-RECORDS.md`."""
      yield "" ]

let renderGap (g: Json) ((st, decs, v): Status) : string list =
    [ yield $"""### {s "id" g} ({s "priority" g}, origin: gap-analysis) — {st}"""
      yield $"""- **Exposed by:** {s "gap_finding" g} · **Parent:** {s "parent" g} · **Decisions:** {decs}"""
      yield $"""- **Requirement:** {s "statement" g}"""
      if v <> "" then yield $"- **Validation still required:** {v}"
      yield "- **Acceptance criteria:**"
      yield! acLines (at "acceptance_criteria" g)
      yield $"""- Full record: `{RECON}/data/gap-requirements.jsonl` ({s "id" g}); report `{RECON}/04-MISSING-REQUIREMENTS-REGISTER.md`."""
      yield "" ]

/// Raised where the Python raises SystemExit(message): message on stderr, exit 1.
exception AdoptExit of string

/// Insert a single annotation line after the original definition (idempotent).
let annotateOriginal (text: string) (r: Json) (target: string) (st: string) : string =
    let marker = $"""{NOTE}: {(if s "relationship" r = "supersedes" then "superseded" else "refined")} by `{s "refinement_id" r}`"""
    if text.Contains marker then text
    else
        let lines = text.Split('\n') |> List.ofArray
        let key = take 80 (norm (asStr (orElse (JStr "") (at "original_text" r))))
        let byText = if key = "" then [] else lines |> List.indexed |> List.filter (fun (_, l) -> (norm l).Contains(take 60 key)) |> List.map fst
        let oid = s "original_id" r
        let hits =
            if not byText.IsEmpty then byText
            else
                lines |> List.indexed
                |> List.filter (fun (_, l) -> l.Contains oid && (l.StartsWith('#') || l.StartsWith('-') || l.StartsWith("**", StringComparison.Ordinal)))
                |> List.map fst
        match hits with
        | [] -> raise (AdoptExit $"""original not found for {s "refinement_id" r} in {s "original_path" r}""")
        | i :: _ ->
            let where = if target = s "original_path" r then "below" else $"in `{target}`"
            let note = $"> {marker} ({st}); the adopted requirement is {where} under 'Adopted reconciliation requirements'."
            String.Join("\n", List.take (i + 1) lines @ (if lines.[i].StartsWith('#') then [ "" ] else []) @ [ note ] @ List.skip (i + 1) lines)

let section (blocks: string list list) : string =
    let body =
        [ BEGIN; ""; "## Adopted reconciliation requirements (2026-11-23)"; ""
          "Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files."
          "" ]
    String.Join("\n", body @ List.concat blocks @ [ END ])

let private sectionRx = rx RegexOptions.Singleline (Regex.Escape BEGIN + ".*?" + Regex.Escape END)

let withSection (text: string) (sec: string) : string =
    if text.Contains BEGIN then subLit sectionRx sec text else text.TrimEnd('\n') + "\n\n" + sec + "\n"

let main (argv: string list) : int =
    let root = argv.[0]
    let data = combine root (RECON + "/data")
    let refs = loadJsonlText (readText (combine data "refinements.jsonl"))
    let gaps = loadJsonlText (readText (combine data "gap-requirements.jsonl"))
    let decs = dictRows (readText (combine data "decisions.csv"))
    let inv =
        dictRows (readText (combine data "inventory.csv"))
        |> List.fold (fun (m: Map<string, string>) r -> if m.ContainsKey(s "identifier" r) then m else m.Add(s "identifier" r, s "path" r)) Map.empty
    let refStatus (r: Json) = statusOf (s "refinement_id" r :: s "original_id" r :: (iter (getOr "related_findings" (JArr []) r) |> List.map asStr)) decs
    let gapStatus (g: Json) = statusOf (s "id" g :: s "gap_finding" g :: (iter (getOr "links" (JArr []) g) |> List.map asStr)) decs
    let entries =
        (refs |> List.map (fun r ->
            let st = refStatus r
            let tgt = targetForRefinement r
            tgt, renderRefinement r st, (let a, b, c = st in [ s "refinement_id" r; "refinement"; s "original_id" r; tgt; a; b; c ])))
        @ (gaps |> List.map (fun g ->
            let st = gapStatus g
            let tgt = targetForGap g inv
            tgt, renderGap g st, (let a, b, c = st in [ s "id" g; "gap-requirement"; ""; tgt; a; b; c ])))
    let register = entries |> List.map (fun (_, _, row) -> row)
    let blocks = entries |> List.map (fun (t, _, _) -> t) |> List.distinct |> List.map (fun t -> t, entries |> List.filter (fun (x, _, _) -> x = t) |> List.map (fun (_, b, _) -> b))
    // files: insertion-ordered path -> text, as the Python dict is built
    let setFile (files: (string * string) list) (p: string) (t: string) =
        if files |> List.exists (fst >> (=) p) then files |> List.map (fun (k, v) -> if k = p then k, t else k, v) else files @ [ p, t ]
    let current (files: (string * string) list) (p: string) =
        match files |> List.tryFind (fst >> (=) p) with
        | Some(_, t) when t <> "" -> t
        | _ -> readText (combine root p)
    try
        let files =
            refs |> List.fold (fun files r ->
                let p = s "original_path" r
                setFile files p (annotateOriginal (current files p) r (targetForRefinement r) (let a, _, _ = refStatus r in a))) []
        let files = blocks |> List.fold (fun files (tgt, bl) -> setFile files tgt (withSection (current files tgt) (section bl))) files
        for p, t in files do writeText (combine root p) t
        writeText (combine data "adoption.csv")
            (PyCsv.format PyCsv.QuoteAll ([ "record_id"; "kind"; "original_id"; "adopted_into"; "status"; "decisions"; "validation_required" ] :: register))
        let c = counter (register |> List.map (fun r -> r.[4]))
        let dict = "{" + String.Join(", ", c |> List.map (fun (k, n) -> $"{reprStr k}: {n}")) + "}"
        Io.print $"{dict} files touched: {files.Length}"
        0
    with AdoptExit msg ->
        Io.eprint msg
        1
