/// W-001 Embedded grant-status widget, v2; port of v2/tools/screens/W-001.mjs. The widget is its own document
/// (body.unb-widget) served from the sponsor's Unbound origin; a partner host iframes it.
/// Data first, then pure renderers. Synthetic data only.
module Unbound.Designs.W001

open Unbound.Designs.Ui
open Unbound.Designs.Screen

let private EIN (brand: string) = if brand = "example-harbor" then "00-0000000" else "00-0000001"

type Grant = { Charity: string; Fund: string; Amount: float; Status: string; Next: string; Steps: (string * string) list; Updated: string }

let private GRANT =
    { Charity = "Riverside Food Bank"
      Fund = "Lindqvist Family Fund"
      Amount = 5000.0
      Status = chip "ok" "Approved"
      Next = $"""Payment scheduled for {time "2026-12-03" "Dec 3, 2026"}"""
      Steps = [ "Submitted", "done"; "Approved", "done"; "To be paid", "current" ]
      Updated = time "2026-12-01T16:58:00-06:00" "Dec 1, 2026, 4:58 PM CT" }

// ---------- widget document ----------
let private doc (brand: string) (title: string) (card: string) =
    let b = brands brand
    let css = b.Css |> List.map (fun h -> $"""<link rel="stylesheet" href="{h}">""") |> String.concat "\n"
    $"""<!doctype html>
<html lang="en" data-forma-theme="light">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>{title} · {b.Name}</title>
<link rel="stylesheet" href="../../../vendor/forma-1.0.2/all.css">
{css}
<link rel="stylesheet" href="../../unbound-app.css">
<link rel="stylesheet" href="../../proposed/ops-charity-widget.css">
</head>
<body class="unb-widget" data-forma-brand="{brand}">
<main class="unb-widget__card unb-x-wcard">
<p class="unb-x-wcard__brand"><span class="unb-x-wcard__mark">{marks brand}</span><span class="unb-x-wcard__name">{b.Name}</span><span>Read only</span></p>
<div class="unb-x-wcard__body">{card}</div>
<footer class="unb-x-wcard__foot"><p>Powered by <strong>{b.Name}</strong>, a donor-advised fund program of {b.Sponsor} (EIN <span class="unb-num">{EIN brand}</span>), which has exclusive legal control over contributed assets.</p></footer>
</main>
</body>
</html>
"""

let private grantCard (g: Grant) =
    let facts = dl [ "From", g.Fund; "Last updated", g.Updated ]
    $"""<h1>Grant to {g.Charity}</h1>
<div class="unb-x-wcard__status">{g.Status}<p>{g.Next}</p></div>
<p class="unb-x-wcard__amount">{moneyHero g.Amount}</p>
{steps g.Steps}
{facts}"""

let private expiredCard () =
    $"""<h1>Grant status isn't available right now</h1>
<div class="unb-x-wcard__error">{icon "lock"}<p><strong>Sign in again on your provider's site</strong> to see this grant.</p><p>For your security this view closes after 15 minutes. No grant details are shown until you sign in again.</p></div>"""

// ---------- partner host page (fictional, the partner's own CSS) ----------
let private HOST_CSS =
    """
:root{--h-ink:#14202e;--h-muted:#4a5868;--h-navy:#12324a;--h-line:#d5dde5;--h-bg:#f4f7f9;--h-accent:#00695c}
*{box-sizing:border-box}
body{margin:0;font-family:Georgia,"Times New Roman",serif;color:var(--h-ink);background:var(--h-bg);line-height:1.5}
a{color:var(--h-accent)}
:focus-visible{outline:3px solid #b3541e;outline-offset:2px}
.h-bar{display:flex;flex-wrap:wrap;align-items:center;gap:8px 24px;padding:12px clamp(16px,4vw,48px);background:var(--h-navy);color:#fff}
.h-bar strong{font-size:1.2rem;letter-spacing:.02em}
.h-bar nav ul{display:flex;flex-wrap:wrap;gap:4px 16px;margin:0;padding:0;list-style:none}
.h-bar nav a{color:#fff;display:inline-block;padding:6px 0;font-family:system-ui,sans-serif;font-size:.9375rem}
.h-main{max-width:1180px;margin:0 auto;padding:24px clamp(16px,4vw,48px) 48px;display:grid;gap:24px}
h1{margin:0;font-size:clamp(1.6rem,1.3rem + 1vw,2.2rem);font-weight:400}
h2{margin:0 0 8px;font-size:1.15rem;font-family:system-ui,sans-serif}
.h-lead{margin:0;color:var(--h-muted);font-family:system-ui,sans-serif;max-width:60ch}
.h-grid{display:grid;gap:24px;align-items:start;grid-template-columns:minmax(0,1fr)}
@media (min-width:1060px){.h-grid{grid-template-columns:minmax(0,634px) 394px;justify-content:space-between}}
.h-slot{background:#fff;border:1px solid var(--h-line);border-radius:2px;padding:16px;min-width:0}
.h-slot p{margin:0 0 12px;font-family:system-ui,sans-serif;font-size:.875rem;color:var(--h-muted)}
.h-slot iframe{display:block;border:0;width:100%;max-width:100%}
.h-slot--narrow iframe{width:360px}
.h-slot--wide iframe{width:600px;height:416px}
.h-slot--narrow iframe{height:464px}
/* No resize messages in this prototype: the host reserves enough height per slot width. */
@media (max-width:479px){.h-slot iframe{height:560px!important}}
.h-foot{font-family:system-ui,sans-serif;font-size:.8125rem;color:var(--h-muted);border-top:1px solid var(--h-line);padding-top:16px}
"""

let private host () =
    """<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>Your giving · Fairway Wealth Partners (fictional partner host)</title>
<style>"""
    + HOST_CSS
    + """</style>
</head>
<body>
<header class="h-bar"><strong>Fairway Wealth Partners</strong><nav aria-label="Client portal"><ul><li><a href="#accounts">Accounts</a></li><li><a href="#giving" aria-current="page">Giving</a></li><li><a href="#documents">Documents</a></li></ul></nav></header>
<main class="h-main" id="main">
<div><h1>Your charitable giving</h1><p class="h-lead">Your donor-advised fund is held by your giving sponsor. The cards below come straight from the sponsor and update on their own.</p></div>
<div class="h-grid">
<section class="h-slot h-slot--wide" aria-labelledby="wide-t"><h2 id="wide-t">Latest grant</h2><p>Wide slot, 600 px.</p><iframe src="widget.html" title="Grant status from Unbound Giving (wide)" tabindex="-1"></iframe></section>
<section class="h-slot h-slot--narrow" aria-labelledby="narrow-t"><h2 id="narrow-t">Grant status</h2><p>Sidebar slot, 360 px.</p><iframe src="widget.html" title="Grant status from Unbound Giving (sidebar)" tabindex="-1"></iframe></section>
</div>
<p class="h-foot">Fairway Wealth Partners does not hold or control your donor-advised fund. Grant information is provided read-only by your sponsor.</p>
</main>
</body>
</html>
"""

let private html (id: string) (kind: string) (title: string) (persona: string) (scenarios: string list) (content: string) : State =
    { Id = id; Title = title; Persona = persona; Scenarios = scenarios; Kind = Some kind; Content = Html content }

let states : State list =
    [ html "widget" "widget" "Widget document: approved grant, read-only card" "Donor viewing a partner site (valid token)" [ "normal"; "embedded" ] (doc "unbound-default" "Grant to Riverside Food Bank" (grantCard GRANT))
      html "host-page" "host" "Partner host page embedding the widget at 360 px and 600 px" "Donor on the partner's client portal" [ "embedded"; "white-label"; "normal" ] (host ())
      html "expired" "widget" "Expired or invalid token: safe message, no grant data" "Donor whose partner session expired" [ "error"; "embedded" ] (doc "unbound-default" "Grant status unavailable" (expiredCard ()))
      html "widget-harbor" "widget" "Widget under the Harbor Giving sponsor brand" "Donor viewing a partner site (valid token)" [ "white-label"; "embedded" ] (doc "example-harbor" "Grant to Riverside Food Bank" (grantCard GRANT)) ]

let screen : Screen = { Id = "W-001"; Slug = "W-001-grant-status-widget"; Title = "Embedded grant-status widget"; Requirement = "UNB-UI-W-001"; States = states }
