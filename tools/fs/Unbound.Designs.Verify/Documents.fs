/// verify-documents: renders the Unbound print-document prototypes with headless Chromium and verifies them
/// (port of tools/verify-documents.mjs).
/// - PDF: Letter, printBackground, preferCSSPageSize -> rendered/<doc>.pdf
/// - PNG: page-1 viewport screenshot at 1280 px width, deviceScaleFactor 1 -> rendered/<doc>.png
/// - Checks via pdfinfo / pdftotext, determinism (two independent browser launches), axe-core on the HTML,
///   tagged-PDF status, plus two capability probes that do NOT change the documents:
///     * tagged probe: the same render with Chromium's tagged-PDF option;
///     * P1 margin-box probe: an injected @page margin-box stylesheet (what Folio's own Signal fixture
///       hand-authors) to show whether the engine can meet "Page n of N" / running reference when Folio cannot.
/// Writes documents/verification.json. The checks are pure functions of the HTML source and the PDF text.
module Unbound.Designs.Verify.Documents

open System
open System.Diagnostics
open System.IO
open System.Security.Cryptography
open System.Text
open System.Text.RegularExpressions
open Unbound.Designs.Verify.JsCompat

let axeTags = [ "wcag2a"; "wcag2aa"; "wcag21a"; "wcag21aa"; "wcag22aa" ]

let pdfOptionsJson = JObj [ "format", str "Letter"; "printBackground", JBool true; "preferCSSPageSize", JBool true ]

// ---------- document manifest ----------

let sponsorDisclosure =
    "Harbor Giving is a donor advised fund program of Example Charitable Foundation (EIN 00-0000000), the legal sponsor. Example Charitable Foundation holds exclusive legal control over all contributed assets."
let exclusiveControl =
    "Example Charitable Foundation, as sponsoring organization of this donor advised fund, has exclusive legal control over the contributed assets."
let noGoods = "No goods or services were provided in exchange for this contribution."

type Doc = { File: string; Ids: string list; Ref: string; DonorFacing: bool; Gaps: string list }

let docs =
    [ { File = "D-001-acknowledgment-cash"; Ids = [ "UNB-UI-D-001"; "UNB-UI-G-007"; "UNB-UI-G-014"; "UNB-UI-G-016"; "UNB-UI-G-017"; "UNB-UI-G-018" ]; Ref = "2026-880114"; DonorFacing = true
        Gaps = [ "FOLIO-GAP-01"; "FOLIO-GAP-02"; "FOLIO-GAP-04"; "FOLIO-GAP-05"; "FOLIO-GAP-06"; "FOLIO-GAP-07"; "FOLIO-GAP-11" ] }
      { File = "D-001-acknowledgment-securities"; Ids = [ "UNB-UI-D-001"; "UNB-UI-G-007"; "UNB-UI-G-014"; "UNB-UI-G-016"; "UNB-UI-G-018" ]; Ref = "2026-880121"; DonorFacing = true
        Gaps = [ "FOLIO-GAP-01"; "FOLIO-GAP-02"; "FOLIO-GAP-04"; "FOLIO-GAP-05"; "FOLIO-GAP-06"; "FOLIO-GAP-07"; "FOLIO-GAP-11" ] }
      { File = "D-002-fund-statement"; Ids = [ "UNB-UI-D-002"; "UNB-UI-G-007"; "UNB-UI-G-014"; "UNB-UI-G-016"; "UNB-UI-G-017"; "UNB-UI-G-018" ]; Ref = "2026-310482"; DonorFacing = true
        Gaps = [ "FOLIO-GAP-02"; "FOLIO-GAP-03"; "FOLIO-GAP-04"; "FOLIO-GAP-05"; "FOLIO-GAP-06"; "FOLIO-GAP-08"; "FOLIO-GAP-09"; "FOLIO-GAP-10"; "FOLIO-GAP-11" ] }
      { File = "D-003-grant-remittance"; Ids = [ "UNB-UI-D-003"; "UNB-UI-G-014"; "UNB-UI-G-016"; "UNB-UI-G-017"; "UNB-UI-G-018" ]; Ref = "2026-550207"; DonorFacing = true
        Gaps = [ "FOLIO-GAP-01"; "FOLIO-GAP-02"; "FOLIO-GAP-04"; "FOLIO-GAP-05"; "FOLIO-GAP-06"; "FOLIO-GAP-07"; "FOLIO-GAP-11" ] }
      { File = "D-004-daf-reporting-package"; Ids = [ "UNB-UI-D-004"; "UNB-UI-G-016"; "UNB-UI-G-017"; "UNB-UI-G-018" ]; Ref = "2026-770025"; DonorFacing = true
        Gaps = [ "FOLIO-GAP-02"; "FOLIO-GAP-03"; "FOLIO-GAP-04"; "FOLIO-GAP-05"; "FOLIO-GAP-06"; "FOLIO-GAP-09"; "FOLIO-GAP-11"; "FOLIO-GAP-12" ] }
      { File = "D-005-case-decision-record"; Ids = [ "UNB-UI-D-005"; "UNB-UI-G-016"; "UNB-UI-G-018" ]; Ref = "2026-000731"; DonorFacing = false
        Gaps = [ "FOLIO-GAP-04"; "FOLIO-GAP-05"; "FOLIO-GAP-06"; "FOLIO-GAP-11" ] }
      { File = "D-006-audit-evidence-package"; Ids = [ "UNB-UI-D-006"; "UNB-UI-G-016"; "UNB-UI-G-018" ]; Ref = "2026-990004"; DonorFacing = false
        Gaps = [ "FOLIO-GAP-03"; "FOLIO-GAP-04"; "FOLIO-GAP-05"; "FOLIO-GAP-06"; "FOLIO-GAP-09"; "FOLIO-GAP-10"; "FOLIO-GAP-11" ] } ]

// ---------- pure helpers ----------

let private W = "[" + jsWs + "]"

let sha256Hex (bytes: byte[]) = Convert.ToHexString(SHA256.HashData bytes).ToLowerInvariant()
let sha256 (s: string) = sha256Hex (Encoding.UTF8.GetBytes s)
let squash (s: string) = jsTrim (Regex.Replace(s, W + "+", " "))
let noSpace (s: string) = Regex.Replace(s, W + "+", "")
let unique (xs: string list) = List.distinct xs
let all (xs: bool list) = not xs.IsEmpty && List.forall id xs
let includes (needle: string) (s: string) = s.Contains(needle, StringComparison.Ordinal)

let decodeEntities (s: string) =
    s.Replace("&amp;", "&").Replace("&middot;", "·").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">")

let between (s: string) (a: string) (b: string) =
    let i = s.IndexOf(a, StringComparison.Ordinal)
    if i < 0 then ""
    else
        let j = if i + 1 <= s.Length then s.IndexOf(b, i + 1, StringComparison.Ordinal) else -1
        if j < 0 then s.Substring i else s.Substring(i, j - i)

let private matches (pattern: string) (s: string) = [ for m in Regex.Matches(s, pattern) -> m ]
let private group1 pattern s = matches pattern s |> List.map (fun m -> m.Groups.[1].Value)

let rowRefs (html: string) = group1 "<th scope=\"row\">([0-9]{4}-[0-9]{6})</th>" html |> unique

let pageNumberOk (pages: string list) =
    pages |> List.mapi (fun i p -> Regex.IsMatch(p, $"Page{W}+{i + 1}{W}+of{W}+{pages.Length}(?![A-Za-z0-9_])")) |> List.forall id

let moneyTokens (text: string) = matches @"\$[0-9,.]*[0-9]" text |> List.map (fun m -> m.Value)
let moneyOk (text: string) = moneyTokens text |> List.forall (fun t -> Regex.IsMatch(t, @"^\$[0-9]{1,3}(,[0-9]{3})*\.[0-9]{2}\z"))

let pagesContaining (pages: string list) (needles: string list) =
    pages |> List.mapi (fun i p -> i, p) |> List.filter (fun (_, p) -> needles |> List.exists (fun n -> includes n p))

/// Strips the volatile Info-dictionary dates and trailer /ID so we can tell whether that is the ONLY byte difference.
let normalizePdf (bytes: byte[]) =
    let s = Encoding.Latin1.GetString bytes
    let s = Regex.Replace(s, @"/(CreationDate|ModDate)" + W + @"*\(D:[^)]*\)", "/$1 ()")
    Regex.Replace(s, @"/ID" + W + @"*\[" + W + @"*<[0-9A-Fa-f]*>" + W + @"*<[0-9A-Fa-f]*>" + W + @"*\]", "/ID []")

/// pdfinfo output as an ordered key/value list (Object.fromEntries of `key: value` lines).
let parsePdfinfo (output: string) : (string * string) list =
    output.Split '\n'
    |> Array.filter (fun l -> l <> "")
    |> Array.map (fun l ->
        let i = l.IndexOf ':'
        let key = if i < 0 then (if l.Length > 0 then l.Substring(0, l.Length - 1) else l) else l.Substring(0, i)
        jsTrim key, jsTrim (l.Substring(i + 1)))
    |> Array.fold (fun acc (k, v) -> if List.exists (fun (k2, _) -> k2 = k) acc then acc |> List.map (fun (k2, v2) -> if k2 = k then (k2, v) else (k2, v2)) else acc @ [ (k, v) ]) []

let infoGet (info: (string * string) list) key = info |> List.tryFind (fun (k, _) -> k = key) |> Option.map snd

/// Pages of `pdftotext` output: split at form feeds, keeping the first `count`.
let splitPages (count: int) (text: string) = text.Split '\f' |> Array.truncate count |> List.ofArray

/// The -raw page text as the checks read it: a line-end hyphen keeps its hyphen, other breaks become spaces.
let checkPage (p: string) = squash (p.Replace("-\n", "-"))

let marginBoxProbeCss (reference: string) =
    $"@page {{ @top-right {{ content: \"Reference {reference}\"; font-size: 8pt; }}\n  @bottom-right {{ content: \"Page \" counter(page) \" of \" counter(pages); font-size: 8pt; }} }}"

// ---------- checks ----------

type Inputs =
    { Doc: Doc
      Html: string
      /// -raw page text, squashed
      Pages: string list
      Info: (string * string) list
      /// The PDF bytes as a latin1 string
      Raw: string
      FolioElements: Set<string> }

let commonChecks (x: Inputs) : (string * Json) list =
    let pages = x.Pages
    let text = String.Join("\n", pages)
    let customTags = group1 "<([a-z][a-z0-9]*-[a-z0-9-]+)" x.Html |> unique
    let headings =
        matches ("<h[23][^>]*>([^<]+)</h[23]>" + W + "*<ef-print-table>" + W + "*<table>" + W + "*<caption>([^<]+)</caption>") x.Html
    [ "html_has_no_authored_css", JBool(not (Regex.IsMatch(x.Html, "<style[" + jsWs + ">]", RegexOptions.IgnoreCase)) && not (Regex.IsMatch(x.Html, W + "style" + W + "*=", RegexOptions.IgnoreCase)))
      "html_uses_only_folio_custom_elements", JBool(customTags |> List.forall x.FolioElements.Contains)
      "pdf_page_size_letter", JBool(includes "612 x 792" (infoGet x.Info "Page size" |> Option.defaultValue ""))
      "reference_on_page_1", JBool(includes x.Doc.Ref pages.Head)
      "reference_on_every_page", JBool(pages |> List.forall (includes x.Doc.Ref))
      "page_numbering_n_of_N_on_every_page", JBool(pageNumberOk pages)
      "verification_code_present", JBool(Regex.IsMatch(text, "code [0-9A-Z]{4}-[0-9A-Z]{4}-[0-9A-Z]{4}"))
      "verification_url_extractable_intact", JBool(includes $"https://verify.harborgiving.example/d/{x.Doc.Ref}" text)
      "verification_code_on_every_page", JBool(pages |> List.forall (includes "Verify at"))
      "money_format_all_amounts_two_decimals", JBool(moneyOk text)
      "headings_kept_with_following_table",
      JBool(headings |> List.forall (fun m -> pages |> List.exists (includes $"{decodeEntities m.Groups.[1].Value} {decodeEntities m.Groups.[2].Value}")))
      "no_relative_dates", JBool(not (Regex.IsMatch(text, @"\b(ago|yesterday|tomorrow)\b", RegexOptions.IgnoreCase ||| RegexOptions.ECMAScript)))
      if x.Doc.DonorFacing then
          "legal_sponsor_disclosure_present", JBool(includes sponsorDisclosure (squash text))
          "legal_sponsor_disclosure_on_every_page", JBool(pages |> List.forall (fun p -> includes sponsorDisclosure (squash p))) ]

let private b v = JBool v
let private hashes64 html = group1 "<code>([0-9a-f]{64})</code>" html

let specificChecks (x: Inputs) : (string * Json) list =
    let pages = x.Pages
    let t = squash (String.Join("\n", pages))
    let has n = includes n t
    match x.Doc.File with
    | "D-001-acknowledgment-cash" ->
        [ "protected_exclusive_control_sentence_verbatim", b (has exclusiveControl)
          "goods_or_services_statement_verbatim", b (has noGoods)
          "sponsor_legal_name_and_ein", b (has "Example Charitable Foundation" && has "EIN 00-0000000")
          "donor_name", b (has "Elena Rivera")
          "date_of_gift_absolute", b (has "Date of gift" && has "16 Nov 2026")
          "cash_amount_25000_00", b (has "$25,000.00" && has "ACH")
          "contribution_reference", b (has "2026-101942")
          "single_page_letter", b (pages.Length = 1) ]
    | "D-001-acknowledgment-securities" ->
        let desc = between t "600 shares" "No goods or services"
        [ "protected_exclusive_control_sentence_verbatim", b (has exclusiveControl)
          "goods_or_services_statement_verbatim", b (has noGoods)
          "sponsor_legal_name_and_ein", b (has "Example Charitable Foundation" && has "EIN 00-0000000")
          "donor_name", b (has "Elena Rivera")
          "date_of_gift_absolute", b (has "Date of gift" && has "18 Nov 2026")
          "property_description_600_shares", b (has "600 shares of Northwind Analytics, Inc. common stock")
          "property_description_has_no_dollar_value",
          b (desc.Length > 0 && not (Regex.IsMatch(desc, @"\$|USD|\bvalue[d]? at\b", RegexOptions.IgnoreCase ||| RegexOptions.ECMAScript)))
          "document_has_no_currency_amount_anywhere", b ((moneyTokens t).IsEmpty && not (includes "$" (x.Html.Replace("${", ""))))
          "single_page_letter", b (pages.Length = 1) ]
    | "D-002-fund-statement" ->
        let paid = rowRefs (between x.Html "id=\"grants\"" "id=\"pending\"")
        let grantPages = pagesContaining pages paid
        let summaryLabels = [ "OPENING VALUE"; "CONTRIBUTIONS"; "GRANTS PAID"; "FEES"; "INVESTMENT CHANGE"; "CLOSING VALUE"; "AVAILABLE TO GRANT" ]
        let summaryPage = pages |> List.tryFindIndex (includes "OPENING VALUE") |> Option.defaultValue -1
        let totalHits = pages |> List.map (fun p -> Regex.Matches(p, "Total grants paid").Count)
        [ "grant_rows_at_least_70", b (paid.Length >= 70)
          "all_grant_rows_in_pdf", b (paid |> List.forall has)
          "grants_table_spans_pages", b (grantPages.Length >= 2)
          "grants_header_repeated_on_every_grants_page", b (all (grantPages |> List.map (fun (_, p) -> includes "Grant reference" p)))
          "period_summary_kept_on_one_page", b (summaryPage >= 0 && summaryLabels |> List.forall (fun l -> includes l (pages.[summaryPage].ToUpperInvariant())))
          "negative_investment_change_has_text_label", b (has "−$4,210.33 (decrease)")
          "grants_total_row_printed_once", b (List.sum totalHits = 1)
          "pending_grants_present", b (has "Pending grants" && has "2026-004936")
          "fee_basis_present", b (has "Fee basis")
          "protected_exclusive_control_sentence_verbatim", b (has exclusiveControl)
          "g016_grant_2026_004817_present", b (has "2026-004817")
          "toc_present", b (has "Period summary" && has "Disclosures")
          "_diagnostics",
          JObj [
              "grant_rows", jint paid.Length
              "pages_with_grant_rows", JArr(grantPages |> List.map (fun (i, _) -> jint (i + 1)))
              "grants_total_row_hits_per_page", JArr(totalHits |> List.map jint)
          ] ]
    | "D-003-grant-remittance" ->
        let refs = rowRefs x.Html |> List.filter (fun r -> Regex.IsMatch(r, @"^2026-00[0-9]{4}\z"))
        let pledge (s: string) = Regex.IsMatch(s, "pledge", RegexOptions.IgnoreCase ||| RegexOptions.ECMAScript)
        let infoJson = x.Info |> List.map (fun (k, v) -> k, JStr v) |> obj |> stringify 0
        [ "no_pledge_text_in_pdf_text", b (not (pledge t))
          "no_pledge_text_in_pdf_metadata_or_bytes", b (not (pledge infoJson) && not (pledge x.Raw))
          "no_pledge_text_in_html_source", b (not (pledge x.Html))
          "several_grants_listed", b (refs.Length >= 3 && refs |> List.forall has)
          "sponsor_stated_as_legal_donor", b (has "Example Charitable Foundation (EIN 00-0000000) is the donor of each grant")
          "payment_method_and_date", b (has "ACH credit" && has "19 Aug 2026")
          "purpose_and_recognition", b (has "Recognition" && has "Emergency relief")
          "donor_confirmation_present", b (has "Grant confirmation for the advisor")
          "g016_grant_2026_004817_present", b (has "2026-004817")
          "remittance_total_matches", b (has "$14,250.75") ]
    | "D-004-daf-reporting-package" ->
        let snap = group1 "Source snapshot SHA-256</dt><dd><code>([0-9a-f]{64})" x.Html |> List.tryHead
        [ "dafs_count_present", b (has "1,284")
          "aggregate_contributions_present", b (has "$48,210,773.18")
          "aggregate_grants_present", b (has "$39,874,012.55")
          "aggregate_value_present", b (has "$412,907,551.06")
          "reconciliation_to_ledger_table", b (has "Ledger source" && has "Difference")
          "classification_register_version", b (has "v2025.4")
          "preparer_and_signer_fields", b (has "Prepared by" && has "Reviewed and signed by")
          "source_snapshot_hash_present", b (match snap with Some s -> includes s (noSpace t) | None -> false)
          "summary_metrics_and_integrity_kept_together",
          b (pages |> List.exists (fun p -> includes "Year-end figures" p && Regex.IsMatch(p, "SOURCE SNAPSHOT", RegexOptions.IgnoreCase ||| RegexOptions.ECMAScript))) ]
    | "D-005-case-decision-record" ->
        let hashes = hashes64 x.Html
        let ci p = Regex.IsMatch(t, p, RegexOptions.IgnoreCase ||| RegexOptions.ECMAScript)
        [ "case_reference", b (has "2026-000731")
          "rule_that_raised_it", b (has "SAN-OFAC-001")
          "evidence_with_source_and_freshness", b (has "Freshness" && has "Source" && hashes |> List.forall (fun h -> includes h (noSpace t)))
          "decision_and_rationale", b (ci "DECISION" && ci "RATIONALE" && has "Confirmed match")
          "preparer_and_decider_are_different_people", b (has "Prepared by Analyst Dana Whitfield (user 4471)" && has "Decided by Marcus Lee (user 1093)")
          "ofac_report_due_date", b (has "OFAC report due 4 Dec 2026")
          "timestamps_absolute_with_utc", b (has "2026-11-19T13:22:45Z UTC" && has "20 Nov 2026") ]
    | "D-006-audit-evidence-package" ->
        let hashes = hashes64 x.Html
        let evidenceIds = group1 "<th scope=\"row\">([A-Z0-9.-]+-E[0-9][0-9])</th>" x.Html |> unique
        let evPages = pagesContaining pages evidenceIds
        let controls = group1 "<li><a href=\"#control-([^\"]+)\">" x.Html
        let toc = pages |> List.tryFind (includes "Contents") |> Option.defaultValue ""
        [ "every_evidence_row_shows_sha256",
          b (evidenceIds.Length > 0 && hashes.Length = evidenceIds.Length + 1 && hashes |> List.forall (fun h -> includes h (noSpace t)))
          "toc_lists_each_control_section", b (controls.Length = 5 && controls |> List.forall (fun c -> includes $"Control {c}" toc))
          "toc_shows_target_page_numbers", b false // Folio P0 TOC has no target-counter; [data-page] must be authored
          "control_list_present", b (has "Controls in scope")
          "exceptions_with_resolution", b (has "Exceptions identified in the period" && has "Resolution")
          "evidence_header_on_every_evidence_page", b (all (evPages |> List.map (fun (_, p) -> includes "Evidence ID" p)))
          "period_bounded", b (has "1 Oct 2025" && has "31 Dec 2025") ]
    | other -> failwithf "no checks for %s" other

/// The named checks (diagnostics split out) for one document.
let checksFor (defined: Json list) (consoleErrors: string list) (x: Inputs) : (string * Json) list * Json option =
    let head =
        [ "custom_elements_defined", JBool(all (defined |> List.map (fun d -> truthy (d ? defined))))
          "no_console_errors", JBool consoleErrors.IsEmpty ]
    let merged = obj (head @ commonChecks x @ specificChecks x)
    let kvs = match merged with JObj kvs -> kvs | _ -> []
    kvs |> List.filter (fun (k, _) -> k <> "_diagnostics"), prop "_diagnostics" merged

/// axe results as recorded.
let axeSummary (r: Json) : Json =
    JObj [
        "violations",
        JArr(
            asList (r ? violations)
            |> List.map (fun v ->
                objOpt [
                    "id", prop "id" v
                    "impact", prop "impact" v
                    "help", prop "help" v
                    "nodes", Some(jint (asList (v ? nodes)).Length)
                    "targets", Some(JArr(asList (v ? nodes) |> List.truncate 5 |> List.map (fun n -> JStr(jsJoin " " (asList (n ? target))))))
                ])
        )
        "passes", r ? passes
        "incomplete", r ? incomplete
    ]

type Renders =
    { Defined: Json list
      ConsoleErrors: string list
      BrowserVersion: string
      RenderMs: int64
      Pdf1: byte[]
      Pdf2: byte[]
      Info1: (string * string) list
      Info2: (string * string) list
      InfoTagged: (string * string) list
      Pages1: string list
      Pages2: string list
      CheckPages: string list
      ProbePages: string list
      Axe: Json }

/// Unique class tokens of the HTML source, sorted by code unit.
let recipeClasses (html: string) =
    [ for m in Regex.Matches(html, "class=\"([^\"]+)\"") do yield! Regex.Split(m.Groups.[1].Value, W + "+") ]
    |> List.distinct
    |> List.sortWith (fun a b -> String.CompareOrdinal(a, b))

/// The verification record for one document.
let documentRecord (doc: Doc) (html: string) (folioElements: Set<string>) (r: Renders) : Json =
    let inputs = { Doc = doc; Html = html; Pages = r.CheckPages; Info = r.Info1; Raw = Encoding.Latin1.GetString r.Pdf1; FolioElements = folioElements }
    let named, diagnostics = checksFor r.Defined r.ConsoleErrors inputs
    let textHashes1 = r.Pages1 |> List.map sha256
    let textHashes2 = r.Pages2 |> List.map sha256
    let info k = infoGet r.Info1 k
    let info2 k = infoGet r.Info2 k
    let differs k = info k <> info2 k
    let dateText (v: string option) = match v with Some s -> s | None -> "undefined"
    let classes = recipeClasses html
    objOpt [
        "document", Some(str $"documents/{doc.File}.html")
        "pdf", Some(str $"rendered/{doc.File}.pdf")
        "screenshot", Some(str $"rendered/{doc.File}.png")
        "requirement_ids", Some(strs doc.Ids)
        "folio_elements_used", Some(strs (r.Defined |> List.map (fun d -> asStr (d ? name)) |> List.sortWith (fun a b -> String.CompareOrdinal(a, b))))
        "folio_recipe_classes_used", Some(strs classes)
        "pages", Some(num (match info "Pages" with Some p -> (match Double.TryParse(p, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture) with | true, v -> v | _ -> nan) | None -> nan))
        "page_size", info "Page size" |> Option.map str
        "render_ms_first_run", Some(num (float r.RenderMs))
        "checks", Some(JObj named)
        "checks_passed", Some(jint (named |> List.filter (snd >> truthy) |> List.length))
        "checks_total", Some(jint named.Length)
        "diagnostics", Some(diagnostics |> Option.defaultValue (JObj []))
        "raw_pdf_sha256", Some(strs [ sha256Hex r.Pdf1; sha256Hex r.Pdf2 ])
        "raw_pdf_hash_stable", Some(JBool(sha256Hex r.Pdf1 = sha256Hex r.Pdf2))
        "raw_pdf_hash_stable_after_stripping_dates_and_id", Some(JBool(sha256 (normalizePdf r.Pdf1) = sha256 (normalizePdf r.Pdf2)))
        "raw_pdf_difference_explained_by",
        Some(
            strs [
                if differs "CreationDate" then "CreationDate (" + dateText (info "CreationDate") + " vs " + dateText (info2 "CreationDate") + ")"
                if differs "ModDate" then "ModDate (" + dateText (info "ModDate") + " vs " + dateText (info2 "ModDate") + ")"
            ]
        )
        "page_text_sha256", Some(strs textHashes1)
        "text_hash_stable", Some(JBool(textHashes1.Length = textHashes2.Length && List.forall2 (=) textHashes1 textHashes2))
        "tagged_pdf", Some(JBool(info "Tagged" = Some "yes"))
        "tagged_pdf_with_chromium_tagged_option", Some(JBool(infoGet r.InfoTagged "Tagged" = Some "yes"))
        "pdf_producer", Some(match info "Producer" with Some p -> str p | None -> JNull)
        "p1_margin_box_probe",
        Some(
            JObj [
                "note", str "Injected @page margin-box CSS (not part of the document). Shows engine capability Folio does not package."
                "page_numbering_n_of_N_on_every_page", JBool(pageNumberOk r.ProbePages)
                "reference_on_every_page", JBool(r.ProbePages |> List.forall (includes doc.Ref))
                "pages", jint r.ProbePages.Length
            ]
        )
        "axe", Some(JObj([ "tags", strs axeTags; "viewport_width", jint 1280 ] @ (match r.Axe with JObj kvs -> kvs | _ -> [])))
        "axe_violations", Some(r.Axe ? violations)
        "gaps", Some(strs doc.Gaps)
        "browser", Some(str r.BrowserVersion)
    ]

let summaryLine (doc: Doc) (r: Json) =
    let bs (j: Json) = if truthy j then "true" else "false"
    $"{doc.File}: pages={jsString (r ? pages)} checks={jsString (r ? checks_passed)}/{jsString (r ? checks_total)} raw={bs (r ? raw_pdf_hash_stable)} text={bs (r ? text_hash_stable)} tagged={bs (r ? tagged_pdf)}/{bs (r ? tagged_pdf_with_chromium_tagged_option)} axe={(asList (r ? axe_violations)).Length}"

let failLines (r: Json) =
    match r ? checks with
    | JObj kvs -> kvs |> List.filter (fun (_, v) -> not (truthy v)) |> List.map (fun (k, _) -> $"   FAIL {k}")
    | _ -> []

let report (results: Json list) : Json =
    JObj [
        "generated_by", str "requirements/ui/designs/tools/verify-documents.mjs"
        "folio", JObj [ "source", str "kemiller2002/folio"; "commit", str "56a18e1"; "version", str "0.4.2"; "vendored_at", str "requirements/ui/designs/vendor/folio-0.4.2" ]
        "renderer",
        objOpt [
            "engine", Some(str "Chromium via Playwright (preinstalled)")
            "browser", results |> List.tryHead |> Option.map (fun r -> r ? browser)
            "pdf_options", Some pdfOptionsJson
        ]
        "axe_core", str "4.10.2"
        "documents", JArr results
    ]

let contentType (ext: string) =
    Map [ ".html", "text/html; charset=utf-8"; ".js", "text/javascript"; ".css", "text/css"; ".json", "application/json" ]
    |> Map.tryFind ext
    |> Option.orElse (Some "application/octet-stream")

let folioElements (registerJs: string) =
    Regex.Matches(registerJs, "\"ef-print-[a-z-]+\"") |> Seq.map (fun m -> m.Value.Substring(1, m.Value.Length - 2)) |> Set.ofSeq

// ---------- I/O ----------

type Settings = { Root: string; Axe: string; Work: string }

let private exec (file: string) (args: string list) : string =
    let psi = ProcessStartInfo(file, args)
    psi.RedirectStandardOutput <- true
    psi.UseShellExecute <- false
    use p = Process.Start psi
    use ms = new MemoryStream()
    p.StandardOutput.BaseStream.CopyTo ms
    p.WaitForExit()
    if p.ExitCode <> 0 then failwithf "Command failed: %s %s" file (String.Join(" ", args))
    Encoding.UTF8.GetString(ms.ToArray())

let pdfinfo (file: string) = parsePdfinfo (exec "pdfinfo" [ file ])
let private pageCount file = match infoGet (pdfinfo file) "Pages" with Some p -> Int32.Parse p | None -> 0
let pdfPages (file: string) = splitPages (pageCount file) (exec "pdftotext" [ "-enc"; "UTF-8"; file; "-" ])
let pdfPagesForChecks (file: string) = splitPages (pageCount file) (exec "pdftotext" [ "-raw"; "-enc"; "UTF-8"; file; "-" ]) |> List.map checkPage

let private definedScript = """async () => {
    const names = unique => [...new Set([...document.querySelectorAll("*")].map(e => e.localName).filter(n => n.startsWith("ef-print-")))];
    const used = names();
    await Promise.all(used.map(n => customElements.whenDefined(n)));
    await document.fonts.ready;
    return used.map(n => ({ name: n, defined: !!customElements.get(n) }));
  }"""

let private axeScript = """(tags) => window.axe.run(document, { runOnly: { type: "tag", values: tags } }).then((result) => ({
    violations: result.violations.map((v) => ({ id: v.id, impact: v.impact, help: v.help, nodes: v.nodes.map((n) => ({ target: n.target })) })),
    passes: result.passes.length,
    incomplete: result.incomplete.map((v) => ({ id: v.id, nodes: v.nodes.length })),
  }))"""

let private openDocument (browser: Cdp.Browser) (url: string) (width: int) (height: int) =
    task {
        let! page = Cdp.newPage browser { Width = width; Height = height; DeviceScaleFactor = 1.0; ReducedMotion = None }
        do! Cdp.goto page url
        let! defined = Cdp.call page definedScript []
        return page, asList defined
    }

let private errorsOf (page: Cdp.Page) =
    Cdp.events page |> List.choose (function Cdp.Console("error", t) -> Some t | Cdp.PageError t -> Some t | _ -> None)

type private RenderRun = { Defined: Json list; ConsoleErrors: string list; Version: string }

let private renderOnce (url: string) (pdfPath: string) (pngPath: string option) (tagged: bool) (injectCss: string option) =
    task {
        let! browser = Cdp.launch ()
        try
            let! page, defined = openDocument browser url 1280 1656
            match pngPath with
            | Some p ->
                let! png = Cdp.screenshot page false
                Cdp.save p png
            | None -> ()
            match injectCss with
            | Some css -> do! Cdp.addStyleTag page css
            | None -> ()
            let! bytes = Cdp.pdf page tagged
            Cdp.save pdfPath bytes
            do! Cdp.closePage page
            return { Defined = defined; ConsoleErrors = errorsOf page; Version = browser.Version }
        finally
            (Cdp.close browser).Wait()
    }

let private runAxe (url: string) (axePath: string) =
    task {
        let! browser = Cdp.launch ()
        try
            let! page, _ = openDocument browser url 1280 900
            do! Cdp.addScriptTag page (File.ReadAllText axePath + "\n//# sourceURL=" + axePath.Replace("\n", ""))
            let! result = Cdp.call page axeScript [ strs axeTags ]
            do! Cdp.closePage page
            return axeSummary result
        finally
            (Cdp.close browser).Wait()
    }

let private verifyDocument (s: Settings) (port: int) (folio: Set<string>) (doc: Doc) =
    task {
        let url = $"http://127.0.0.1:{port}/documents/{doc.File}.html"
        let html = File.ReadAllText(Path.Combine(s.Root, "documents", $"{doc.File}.html"))
        let renderDir = Path.Combine(s.Root, "rendered")
        let pdf1 = Path.Combine(renderDir, $"{doc.File}.pdf")
        let pdf2 = Path.Combine(s.Work, $"{doc.File}.run2.pdf")
        let pdfTagged = Path.Combine(s.Work, $"{doc.File}.tagged.pdf")
        let pdfProbe = Path.Combine(s.Work, $"{doc.File}.p1-margin-probe.pdf")
        let sw = Stopwatch.StartNew()
        let! run1 = renderOnce url pdf1 (Some(Path.Combine(renderDir, $"{doc.File}.png"))) false None
        let renderMs = sw.ElapsedMilliseconds
        let! _ = renderOnce url pdf2 None false None
        let! _ = renderOnce url pdfTagged None true None
        let! _ = renderOnce url pdfProbe None false (Some(marginBoxProbeCss doc.Ref))
        let! axe = runAxe url s.Axe
        let renders =
            { Defined = run1.Defined
              ConsoleErrors = run1.ConsoleErrors
              BrowserVersion = run1.Version
              RenderMs = renderMs
              Pdf1 = File.ReadAllBytes pdf1
              Pdf2 = File.ReadAllBytes pdf2
              Info1 = pdfinfo pdf1
              Info2 = pdfinfo pdf2
              InfoTagged = pdfinfo pdfTagged
              Pages1 = pdfPages pdf1
              Pages2 = pdfPages pdf2
              CheckPages = pdfPagesForChecks pdf1
              ProbePages = pdfPagesForChecks pdfProbe
              Axe = axe }
        return documentRecord doc html folio renders
    }

let run (s: Settings) : int =
    try
        Directory.CreateDirectory(Path.Combine(s.Root, "rendered")) |> ignore
        Directory.CreateDirectory s.Work |> ignore
        let folio = folioElements (File.ReadAllText(Path.Combine(s.Root, "vendor/folio-0.4.2/src/components/register.js")))
        let root = Path.GetFullPath s.Root |> fun p -> p.TrimEnd('/')
        use server = StaticServer.start (StaticServer.fileUnder root contentType)
        let results =
            docs
            |> List.map (fun doc ->
                let r = (verifyDocument s server.Port folio doc).Result
                Console.WriteLine(summaryLine doc r)
                failLines r |> List.iter Console.WriteLine
                r)
        File.WriteAllText(Path.Combine(s.Root, "documents", "verification.json"), stringify 2 (report results) + "\n")
        0
    with e ->
        eprintfn "%O" e
        1
