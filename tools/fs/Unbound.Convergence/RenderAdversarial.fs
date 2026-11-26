/// Render report 08 (adversarial enterprise review) with a disposition for every finding
/// (port of tools/reqaudit/render_adversarial.py). Amendments are read from the clause JSON data.
module Unbound.Convergence.RenderAdversarial

open System.Text.RegularExpressions
open Unbound.Py.PyJson
open Unbound.Convergence.PyCompat

let MISSION = "requirements/convergence/2026-11-24-master"

let BUSINESS_MAP =
    [ "CNV-ECO-003", [ "001"; "002"; "005"; "006"; "007"; "008"; "009"; "013"; "030"; "033" ]
      "CNV-ECO-004", [ "003"; "004"; "010"; "027"; "029" ]
      "CNV-ECO-005", [ "012"; "016"; "019"; "021"; "022"; "025"; "032" ]
      "CNV-GOV-008", [ "017"; "020"; "028" ]
      "CNV-GOV-009", [ "026" ]
      "CNV-EXP-002", [ "011"; "023"; "024" ]
      "CNV-EVB-002", [ "018"; "031" ]
      "report 05 §5 revised (durability rated with evidence)", [ "014" ]
      "owner decision OD-M-04 (report 10): trigger for reopening staffed services (SPR-ADV-009)", [ "015" ] ]

let ROUND2_OTHER =
    Map.ofList
        [ "ADV-R2-030", "addressed with ADV-R2-075 (CNV-GOV-010 amended: one level set)"
          "ADV-R2-032", "tooling: baseline rows now carry full source text and hash (adopt-convergence)"
          "ADV-R2-054", "tooling: textual clause citations are now derived as declared dependencies and cycle-checked"
          "ADV-R2-021",
          "model: December surge staffing re-run (05a section 7); full exception-SLA attainment at twice base staff for +$1 to +$20 per fund-year"
          "ADV-R2-033",
          "34 clauses tagged internal-control with operator, report relevance, automation and CUEC; crosswalk requirements/governance/CONVERGENCE-CLAUSE-TO-CONTROL-CROSSWALK.csv"
          "ADV-R2-063",
          "numeric basis tags applied (clauses/c13_numeric_parameters.json); generation fails on any untagged number with a unit" ]

/// cell = lambda t, n: (" ".join(str(t).split()).replace("|", "\\|"))[:n]
let cell (t: Json) (n: int) : string = cpTake n ((joinWs (str t)).Replace("|", "\\|"))

/// Fill {name} markers of a template in one pass (inserted values are not rescanned).
let fill (template: string) (values: (string * string) list) : string =
    let m = Map.ofList values
    Regex.Replace(template, @"\{([^{}]+)\}", MatchEvaluator(fun x -> match m.TryFind x.Groups.[1].Value with Some v -> v | None -> x.Value))

/// Dispositions accumulated as finding -> list, in first-seen order.
let private addDisp (acc: (string * string list) list) (f: string, v: string) =
    if acc |> List.exists (fst >> (=) f) then acc |> List.map (fun (k, vs) -> if k = f then k, vs @ [ v ] else k, vs) else acc @ [ f, [ v ] ]

let private amendments (root: string) (m: string) : (string * Json) list =
    loads (Io.readText (Io.path root $"{MISSION}/clauses/{m}.json")) |> item "AMENDMENTS" |> items

let private readData (root: string) (n: string) = Io.readJsonl (Io.path root $"{MISSION}/data/{n}")

let private sortedCount (xs: string list list) = counter xs |> List.sortWith (fun (a, _) (b, _) -> cmpStrs a b)

let round2 (root: string) : Result<string, string> =
    let disp =
        [ amendments root "c10_round2_amendments"; amendments root "c11_round2_partial_closures" ]
        |> List.concat
        |> List.collect (fun (cid, a) -> Regex.Matches(itemStr "finding" a, @"ADV-R2-\d{3}") |> Seq.map (fun m -> m.Value, $"amended {cid}") |> List.ofSeq)
        |> List.fold addDisp []
    let dispM = Map.ofList disp
    let rows = readData root "adversarial-round2.jsonl"
    let d (r: Json) =
        let fid = itemStr "finding_id" r
        match dispM.TryFind fid with
        | Some vs -> vs |> dedupe |> String.concat "; "
        | None ->
            match ROUND2_OTHER.TryFind fid with
            | Some v -> v
            | None -> if item "verdict" r = JStr "closed" then "closed in round 2 (no action)" else "UNDISPOSITIONED"
    match rows |> List.filter (fun r -> d r = "UNDISPOSITIONED") |> List.map (itemStr "finding_id") with
    | _ :: _ as missing -> Error $"round-2 items without disposition: {reprStrList missing}"
    | [] ->
        let verdicts = sortedCount (rows |> List.map (fun r -> [ str (item "kind" r); str (item "severity" r); str (item "verdict" r) ]))
        let table =
            rows
            |> List.map (fun r ->
                $"""| {str (item "finding_id" r)} | {str (item "kind" r)} | {str (item "refers_to" r)} | {str (item "severity" r)} | {str (item "verdict" r)} | {cell (item "claim" r) 220} | {d r} |""")
            |> String.concat "\n"
        let summary = verdicts |> List.map (fun (k, n) -> $"| {k.[0]} | {k.[1]} | {k.[2]} | {n} |") |> String.concat "\n"
        Ok(fill AdversarialTemplates.round2 [ "summary", summary; "table", table ])

let main (io: Io.Console) (argv: string list) : int =
    let root = argv.[1]
    let c7 = amendments root "c7_amendments"
    let findings = readData root "adversarial-business.jsonl" @ readData root "adversarial-technical.jsonl"
    let clauses = readData root "convergence-clauses.jsonl"
    let fromAmendments =
        c7 |> List.collect (fun (cid, a) -> Regex.Matches(itemStr "finding" a, @"ADV-T-\d{3}") |> Seq.map (fun m -> m.Value, $"amended {cid}") |> List.ofSeq)
    let fromTitles =
        clauses
        |> List.collect (fun c -> Regex.Matches(itemStr "title" c, @"ADV-T-\d{3}") |> Seq.map (fun m -> m.Value, $"""new {itemStr "id" c}""") |> List.ofSeq)
    let fromBusiness =
        BUSINESS_MAP
        |> List.collect (fun (target, nums) ->
            nums |> List.map (fun n -> $"ADV-B-{n}", (if startsWith "report" target || startsWith "owner" target then target else $"new {target}")))
    let disp = fromAmendments @ fromTitles @ fromBusiness |> List.fold addDisp [] |> Map.ofList
    match findings |> List.map (itemStr "finding_id") |> List.filter (disp.ContainsKey >> not) with
    | _ :: _ as missing ->
        io.Err($"findings without disposition: {reprStrList missing}\n")
        2
    | [] ->
        let rows =
            findings
            |> List.map (fun f ->
                let fid = itemStr "finding_id" f
                $"""| {fid} | {str (item "severity" f)} | {cell (item "perspective" f) 30} | {cell (item "claim" f) 260} | {disp.[fid] |> dedupe |> String.concat "; "} |""")
            |> String.concat "\n"
        let sev = findings |> List.map (fun f -> str (item "severity" f)) |> counter |> Map.ofList
        let sevOf k = string (sev.TryFind k |> Option.defaultValue 0)
        let text =
            fill
                AdversarialTemplates.main
                [ "len(findings)", string findings.Length
                  "sev.get('critical', 0)", sevOf "critical"
                  "sev.get('major', 0)", sevOf "major"
                  "sev.get('minor', 0)", sevOf "minor"
                  "rows", rows ]
        match round2 root with
        | Error msg ->
            // raise SystemExit(msg): the message goes to stderr and the exit status is 1.
            io.Err(msg + "\n")
            1
        | Ok r2 ->
            Io.writeText (Io.path root (MISSION + "/08-ADVERSARIAL-ENTERPRISE-REVIEW.md")) (text + r2)
            io.Out($"{findings.Length} findings rendered\n")
            0
