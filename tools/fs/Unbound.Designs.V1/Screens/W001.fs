/// W-001 Embedded grant status widget (port of tools/screens/W-001.mjs).
/// The widget is the document a partner iframes; it has no application navigation or site footer.
/// Each design state shows one card as it renders inside the frame.
module Unbound.Designs.V1.Screens.W001

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let private card (p: string) (title: string) (inner: string) =
    tag
        "surface"
        $$"""<section class="forma-surface" aria-labelledby="{{p}}-card">
  <h3 id="{{p}}-card">{{title}}</h3>
  {{inner}}
  <p><small>Grant made by Example Charitable Foundation (EIN 00-0000000), which holds exclusive legal control over donor-advised fund assets. Read only.</small></p>
</section>"""

let private grant (p: string) (lozenge: string) =
    let rows =
        kv [ "Amount", money "$5,000.00"
             "Charity", "Riverside Food Bank, Springfield, IL"
             "Grant reference", ident "2026-004817"
             "Status as of", "1 Dec 2026, 4:58 PM CT" ]
    card p "Grant to Riverside Food Bank" (lozenge + rows)

let private render (states: RenderedState list) =
    let body = states |> List.map (fun s -> s.Html) |> String.concat "\n"
    $$"""<!doctype html>
<html lang="en" data-forma-theme="light">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>W-001 Grant status widget | Harbor Giving design prototype</title>
<meta name="description" content="Static Forma 1.0.2 design prototype for UNB-UI-W-001: read-only grant status card for partner iframes. Synthetic data.">
<link rel="stylesheet" href="../vendor/forma-1.0.2/all.css">
<link rel="stylesheet" href="../vendor/forma-1.0.2/brands/example-harbor.css">
</head>
<body class="forma-site" data-forma-brand="example-harbor">
<main class="forma-site__main" id="main-content">
  <header class="forma-section-heading"><div class="forma-section-heading__text">
    <p class="forma-eyebrow">Design prototype · UNB-UI-W-001</p>
    <h1>Embedded grant status widget</h1>
    <p class="forma-lead">Read-only card served from the sponsor's origin inside a partner iframe. No consequential actions in release 1.</p>
  </div></header>
  {{body}}
</main>
</body>
</html>
"""

let screen: Screen =
    { Id = "W-001"
      File = "W-001-grant-status-widget.html"
      Title = "Embedded grant status widget"
      ReqIds = [ "UNB-UI-W-001" ]
      Audience = ""
      Brand = false
      Lead = ""
      States =
        [ state "Normal: funding in progress" "Partner host, valid token" "Headline status, amount and charity; no actions" (fun p -> grant p (loz "attention" "Funding in progress"))
          state "Payment outcome unknown" "Partner host, valid token" "UNB-UI-G-001 AC-1 in the widget (never 'Paid' while unknown)" (fun p ->
              grant p (loz "unknown" "Payment outcome unknown"))
          state "Expired token" "Partner host, expired token" "UNB-UI-W-001 AC-1 ('Sign in again on your provider's site' and no grant data)" (fun p ->
              card p "Grant status unavailable" (loz "blocked" "Session expired" + "<p>Sign in again on your provider's site.</p>")) ]
      Coverage =
        [ cov "UNB-UI-W-001" "AC-1" NeedsApplicationBehaviour "State 3 shows 'Sign in again on your provider's site' and no grant data. Token validation, CSP frame-ancestors and TTL are server behaviour (UNB-GAP-W-01)."
          cov "UNB-UI-W-001" "statement (no actions)" Yes "No interactive controls in any widget state." ]
      Gaps = [ "UNB-GAP-W-01"; "FORMA-GAP-08" ]
      Render = Some render }
