/// Page frame for the screen prototypes (port of tools/lib/shell.mjs), composed only from Forma 1.0.2 patterns:
/// marketing-shell frame (.forma-site, .forma-site__main, .forma-site-footer) because Forma's application shell
/// (.forma-app-shell) is still planned (catalog/planned.json), plus skip-link, app-identity and navigation-shell for
/// application navigation.
module Unbound.Designs.V1.Shell

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

type Sponsor = { LegalName: string; Ein: string; Brand: string; Tz: string }

let sponsor = { LegalName = "Example Charitable Foundation"; Ein = "00-0000000"; Brand = "Harbor Giving"; Tz = "America/Chicago" }

let navs =
    Map [
        "donor",
        [ "S-001-giving-home.html", "Giving home"
          "S-003-grant-recommendation.html", "Recommend a grant"
          "S-004-grant-detail.html", "Grants"
          "S-005-add-money.html", "Add money"
          "S-006-participants-successors.html", "People"
          "S-007-document-center.html", "Documents"
          "S-013-notification-preferences.html", "Notifications" ]
        "staff",
        [ "S-008-exception-inbox.html", "Exception inbox"
          "S-009-case-workspace.html", "Cases"
          "S-010-policy-configuration.html", "Sponsor policy"
          "S-011-sponsor-overview.html", "Overview" ]
        "charity", [ ("S-012-charity-view.html", "Grants received") ]
    ]

let navLinks (audience: string) (current: string) =
    navs.[audience]
    |> List.map (fun (href, label) ->
        let cur = if href = current then " aria-current=\"page\"" else ""
        $"<a href=\"{href}\"{cur}>{label}</a>")
    |> String.concat ""

let identity (audience: string) =
    let suffix = if audience = "staff" then " · Sponsor operations" elif audience = "charity" then " · Charity portal" else ""
    tag
        "app-identity"
        $$"""<a class="forma-app-identity" href="S-001-giving-home.html">
  <span class="forma-app-identity__mark">
    <svg viewBox="0 0 32 32" aria-hidden="true" focusable="false"><path fill="currentColor" fill-rule="evenodd" d="M6 0h20a6 6 0 0 1 6 6v20a6 6 0 0 1-6 6H6a6 6 0 0 1-6-6V6a6 6 0 0 1 6-6Zm10 8-8 14h16Z"/></svg>
  </span>
  <span class="forma-app-identity__name">{{sponsor.Brand}}{{suffix}}</span>
</a>"""

/// Legal-sponsor disclosure (G-014, protected text per G-007).
let legalFooter () =
    tag
        "site-footer"
        $$"""<footer class="forma-site-footer" data-forma-tone="surface">
  <div class="forma-site-footer__inner">
    <div class="forma-site-footer__main">
      <div>
        <p class="forma-eyebrow">Legal sponsor disclosure</p>
        <p>{{sponsor.Brand}} is a donor-advised fund program of {{sponsor.LegalName}}, EIN {{sponsor.Ein}}, a public charity. {{sponsor.LegalName}} holds exclusive legal control over all contributed assets. Grant recommendations are advisory and are approved or declined by {{sponsor.LegalName}}.</p>
      </div>
    </div>
    <nav class="forma-site-footer__nav" aria-label="Footer"><ul><li><a href="#program-terms">Program terms</a></li><li><a href="#privacy">Privacy</a></li><li><a href="#accessibility">Accessibility</a></li></ul></nav>
    <p class="forma-site-footer__note"><span>{{sponsor.LegalName}} · EIN {{sponsor.Ein}}</span><span>Design prototype · synthetic data</span></p>
  </div>
</footer>"""

/// Full prototype page for a screen and its rendered states.
let page (cfg: Screen) (states: RenderedState list) =
    let brandAttr = if cfg.Brand then " data-forma-brand=\"example-harbor\"" else ""
    let brandCss = if cfg.Brand then "<link rel=\"stylesheet\" href=\"../vendor/forma-1.0.2/brands/example-harbor.css\">\n" else ""
    let stateIndex =
        states |> List.mapi (fun i s -> $"<li><a href=\"#{s.Id}\">State {i + 1}: {s.Title}</a></li>") |> String.concat ""
    let nav = navLinks cfg.Audience cfg.File
    let shell =
        tag
            "navigation-shell"
            $$"""<div class="forma-navigation-shell">
  <details class="forma-navigation-shell__compact">
    <summary>Navigation</summary>
    <nav aria-label="Primary">{{nav}}</nav>
  </details>
  <nav class="forma-navigation-shell__persistent" aria-label="Primary">{{nav}}</nav>
  <div class="forma-navigation-shell__content">
    <header class="forma-section-heading">
      <div class="forma-section-heading__text">
        <p class="forma-eyebrow">Design prototype · {{String.concat " · " cfg.ReqIds}}</p>
        <h1>{{cfg.Title}}</h1>
        <p class="forma-lead">{{cfg.Lead}}</p>
      </div>
    </header>
    <nav aria-label="Design states on this page"><ul>{{stateIndex}}</ul></nav>
    {{states |> List.map (fun s -> s.Html) |> String.concat "\n"}}
  </div>
</div>"""
    $$"""<!doctype html>
<html lang="en" data-forma-theme="light">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>{{cfg.Id}} {{cfg.Title}} | {{sponsor.Brand}} design prototype</title>
<meta name="description" content="Static Forma 1.0.2 design prototype for {{String.concat ", " cfg.ReqIds}}. Synthetic data; no application behaviour.">
<link rel="stylesheet" href="../vendor/forma-1.0.2/all.css">
{{brandCss}}</head>
<body class="forma-site"{{brandAttr}}>
{{tag "skip-link" "<a class=\"forma-skip-link\" href=\"#main-content\">Skip to main content</a>"}}
<header class="forma-site-header">
  <div class="forma-site-header__inner">
    <div class="forma-site-header__identity">
      {{identity cfg.Audience}}
    </div>
  </div>
</header>
<main class="forma-site__main" id="main-content" tabindex="-1">
{{shell}}
</main>
{{legalFooter ()}}
</body>
</html>
"""
