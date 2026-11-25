/// uiclass.py: classify every canonical requirement by whether it implies a user-facing surface (first matching rule
/// wins: human-decision, document-output, ui-bearing, else backend). A transparent keyword heuristic.
module Unbound.ReqAudit.UiClass

open System
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

let RECON = "requirements/reconciliation/2026-11-23"

let RULES =
    [ "human-decision",
      reI @"\b(human|authori[sz]ed (?:person|reviewer|approver|staff)|reviewer|approv(?:e|al|er)s?|sign[- ]?off|certif(?:y|ication)|escalat\w*|adjudicat\w*|committee|override|disposition|attest\w*|manual)\b"
      "document-output",
      reI @"\b(statement|acknowledg\w*|receipt|(?<!dead-)letter|remittance|report(?:s|ing)? package|PDF|Folio|document(?:s)? (?:issu|generat|render)\w*|audit package|evidence package|Schedule D|Form 8283|Form 8282)\b"
      "ui-bearing",
      reI @"\b(UI|screen|view|display\w*|show\w*|portal|dashboard|workspace|inbox|queue|Forma|mobile|phone|WCAG|accessib\w*|notif\w*|email|widget|preview|visible|selector|user[- ]visible|donors? (?:see|can)|self[- ]service|(?<!indexes )search(?! index)|wizard|form(?!at)|explain\w*|terminology|brand|ask)\b" ]

let classify (text: string) : string * string =
    RULES
    |> List.tryPick (fun (cls, r) ->
        let hits = finditer r text |> List.map (fun m -> lower m.Value) |> Set.ofList |> sortStrings
        if hits.IsEmpty then None else Some(cls, String.Join(";", List.truncate 6 hits)))
    |> Option.defaultValue ("backend", "")

let main (argv: string list) : int =
    let root = argv.[0]
    let data = combine root (RECON + "/data")
    let inv =
        let rows = dictRows (readText (combine data "inventory.csv"))
        rows |> List.fold (fun (m: Map<string, Json>) r -> m.Add(s "identifier" r, r)) Map.empty
    let q = dictRows (readText (combine data "requirements-quality.csv"))
    let rows =
        q
        |> List.map (fun r ->
            let text = match inv.TryFind(s "identifier" r) with Some row -> s "text" row | None -> raise (PyError $"""KeyError: {reprStr (s "identifier" r)}""")
            let cls, hits = classify text
            [ s "identifier" r; s "family" r; cls; hits ])
    writeText (combine data "requirement-ui-classification.csv")
        (PyCsv.format PyCsv.QuoteAll ([ "identifier"; "family"; "class"; "matched_terms" ] :: rows))
    let by = counter (rows |> List.map (fun r -> r.[2]))
    let families = rows |> List.map (fun r -> r.[1]) |> List.distinct
    let fam = families |> List.map (fun f -> f, counter (rows |> List.filter (fun r -> r.[1] = f) |> List.map (fun r -> r.[2])))
    let toObj (c: (string * int) list) = JObj(c |> List.map (fun (k, n) -> k, jint n))
    let out =
        JObj
            [ "denominator", jint rows.Length
              "by_class", toObj by
              "by_family", JObj(fam |> List.sortWith (fun (_, a) (_, b) -> compare (List.sumBy snd b) (List.sumBy snd a)) |> List.map (fun (f, c) -> f, toObj c)) ]
    Io.writeJsonReport (combine data "requirement-ui-classification-summary.json") out
    Io.print (Io.dumps2 (JObj [ "denominator", jint rows.Length; "by_class", toObj by ]))
    0
