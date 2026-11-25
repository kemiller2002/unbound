/// Pure helpers and shared furniture for the print-document prototypes (the "pure helpers", "sponsor constants" and
/// "shared document furniture" sections of tools/build-documents.mjs). Folio elements, Folio recipe classes and
/// semantic HTML only: no <style>, no style="" attributes, no consumer stylesheet.
module Unbound.Designs.V1.DocumentKit

open Unbound.Designs.V1.JsCompat

// ---------- pure helpers ----------

/// HTML escape of &, <, > and ".
let esc (s: string) = s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;")

/// Math.round(n * 100) as integer cents.
let cents (n: float) = int64 (mathRound (n * 100.0))

/// (c / 100).toLocaleString("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2 }).
let usd (c: int64) = usdOfCents c

/// G-017: negative values carry a text label as well as a sign.
let signedUsdWith (up: string) (down: string) (c: int64) = if c < 0L then $"−{usd (-c)} ({down})" else $"{usd c} ({up})"

let signedUsd c = signedUsdWith "increase" "decrease" c

let private months = [| "Jan"; "Feb"; "Mar"; "Apr"; "May"; "Jun"; "Jul"; "Aug"; "Sep"; "Oct"; "Nov"; "Dec" |]

/// G-018: absolute dates, sponsor time zone (America/Chicago). Input is an ISO local date "YYYY-MM-DD".
let absDate (iso: string) =
    let parts = iso.Split '-' |> Array.map int
    $"{parts.[2]} {months.[parts.[1] - 1]} {parts.[0]}"

let sum (xs: int64 list) = List.sum xs

/// Pick an element with the next mulberry32 draw: xs[Math.floor(rand() * xs.length)]. Returns (next seed, element).
let pick (seed: int32) (xs: 'a list) =
    let seed, r = mulberry32 seed
    seed, List.item (int (floor (r * float (List.length xs)))) xs

// ---------- sponsor constants (G-014, G-007) ----------

type Sponsor = { Brand: string; LegalName: string; Ein: string; Street: string; City: string; Tz: string; VerifyHost: string }

let sponsor =
    { Brand = "Harbor Giving"
      LegalName = "Example Charitable Foundation"
      Ein = "00-0000000"
      Street = "1200 Example Avenue, Suite 400"
      City = "Springfield, IL 62701"
      Tz = "America/Chicago"
      VerifyHost = "verify.harborgiving.example" }

let protectedExclusiveControl =
    "Example Charitable Foundation, as sponsoring organization of this donor advised fund, has exclusive legal control over the contributed assets."

let protectedNoGoods = "No goods or services were provided in exchange for this contribution."

let protectedDisclosure =
    $"{sponsor.Brand} is a donor advised fund program of {sponsor.LegalName} (EIN {sponsor.Ein}), the legal sponsor. {sponsor.LegalName} holds exclusive legal control over all contributed assets."

// ---------- shared document furniture ----------

let head (title: string) (description: string) =
    $$"""<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <meta name="description" content="{{esc description}}">
  <title>{{esc title}}</title>
  <link rel="stylesheet" href="../vendor/folio-0.4.2/src/styles/print.css">
  <script type="module" src="../vendor/folio-0.4.2/src/components/register.js"></script>
</head>"""

let runningHeader (left: string) (center: string) (reference: string) =
    $$"""  <ef-print-header repeat="page">
    <span slot="left">{{esc left}}</span>
    <span slot="center">{{esc center}}</span>
    <span slot="right">Reference {{esc reference}}</span>
  </ef-print-header>"""

/// FOLIO-GAP-02 workaround: reference + verification code as text in the footer.
/// ef-print-page-number carries authored fallback text; Folio computes no counters (P0).
let runningFooter (reference: string) (code: string) (donorFacing: bool) =
    let disclosure = if donorFacing then $"    <p>{esc protectedDisclosure}</p>\n" else ""
    $$"""  <ef-print-footer repeat="page">
    <span slot="left">Reference {{esc reference}}</span>
    <span slot="center">Verify at https://{{sponsor.VerifyHost}}/d/{{esc reference}} &middot; code {{esc code}}</span>
    <span slot="right"><ef-print-page-number format="page-of-pages">Page numbers: renderer-supplied (P1/P2)</ef-print-page-number></span>
{{disclosure}}  </ef-print-footer>"""

let letterhead () =
    $$"""      <div class="ef-identity">
        <p><strong>{{esc sponsor.Brand}}</strong><br>{{esc sponsor.LegalName}}</p>
        <address>
          <span>{{esc sponsor.Street}}</span>
          <span>{{esc sponsor.City}}</span>
          <span>EIN {{esc sponsor.Ein}}</span>
        </address>
      </div>"""

/// rows: (label, value html).
let metaList (rows: (string * string) list) =
    let items = rows |> List.map (fun (k, v) -> $"        <div><dt>{esc k}</dt><dd>{v}</dd></div>") |> String.concat "\n"
    $$"""<dl class="ef-meta-list">
{{items}}
      </dl>"""

type Metric = { Label: string; Value: string; Detail: string; Strong: bool }

let metric (m: Metric) =
    let emphasis = if m.Strong then " emphasis=\"strong\"" else ""
    let detail = if m.Detail <> "" then $"<span data-detail>{esc m.Detail}</span>" else ""
    $$"""<li><ef-print-metric{{emphasis}}>
            <span data-label>{{esc m.Label}}</span>
            <strong data-value>{{esc m.Value}}</strong>
            {{detail}}
          </ef-print-metric></li>"""

let metricGrid (metrics: Metric list) =
    $$"""<ef-print-keep>
        <ul class="ef-category-grid">
          {{metrics |> List.map metric |> String.concat "\n          "}}
        </ul>
      </ef-print-keep>"""

/// A table column: header text and whether it holds numbers.
type Col = { H: string; Num: bool }

let col h = { H = h; Num = false }
let num h = { H = h; Num = true }

/// rows: lists of already-escaped cell HTML; the first column is a row header. foot: optional footer row.
let table (caption: string) (cols: Col list) (rows: string list list) (foot: string list option) =
    let colArr = List.toArray cols
    let align i = if colArr.[i].Num then " data-align=\"number\"" else ""
    let cells (r: string list) =
        r |> List.mapi (fun i cell -> if i = 0 then $"<th scope=\"row\">{cell}</th>" else $"<td{align i}>{cell}</td>") |> String.concat ""
    let thead = cols |> List.mapi (fun i c -> $"<th scope=\"col\"{align i}>{esc c.H}</th>") |> String.concat ""
    let body = rows |> List.map (fun r -> $"            <tr>{cells r}</tr>") |> String.concat "\n"
    let tfoot =
        match foot with
        | Some f -> $"\n          <tfoot><tr>{cells f}</tr></tfoot>"
        | None -> ""
    $$"""<ef-print-table>
        <table>
          <caption>{{esc caption}}</caption>
          <thead><tr>{{thead}}</tr></thead>
          <tbody>
{{body}}
          </tbody>{{tfoot}}
        </table>
      </ef-print-table>"""

let page (title: string) (description: string) (body: string) =
    $"{head title description}\n<body>\n{body}\n</body>\n</html>\n"
