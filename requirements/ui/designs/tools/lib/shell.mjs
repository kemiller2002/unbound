// Page frame for the screen prototypes, composed only from Forma 1.0.2 patterns:
// marketing-shell frame (.forma-site, .forma-site__main, .forma-site-footer) because Forma's
// application shell (.forma-app-shell) is still planned (catalog/planned.json), plus
// skip-link, app-identity and navigation-shell for application navigation.
import { tag } from "./markup.mjs";

export const SPONSOR = {
  legalName: "Example Charitable Foundation",
  ein: "00-0000000",
  brand: "Harbor Giving",
  tz: "America/Chicago",
};

const NAVS = {
  donor: [
    ["S-001-giving-home.html", "Giving home"],
    ["S-003-grant-recommendation.html", "Recommend a grant"],
    ["S-004-grant-detail.html", "Grants"],
    ["S-005-add-money.html", "Add money"],
    ["S-006-participants-successors.html", "People"],
    ["S-007-document-center.html", "Documents"],
    ["S-013-notification-preferences.html", "Notifications"],
  ],
  staff: [
    ["S-008-exception-inbox.html", "Exception inbox"],
    ["S-009-case-workspace.html", "Cases"],
    ["S-010-policy-configuration.html", "Sponsor policy"],
    ["S-011-sponsor-overview.html", "Overview"],
  ],
  charity: [["S-012-charity-view.html", "Grants received"]],
};

const navLinks = (audience, current) =>
  NAVS[audience]
    .map(([href, label]) => `<a href="${href}"${href === current ? ' aria-current="page"' : ""}>${label}</a>`)
    .join("");

const identity = (audience) =>
  tag(
    "app-identity",
    `<a class="forma-app-identity" href="S-001-giving-home.html">
  <span class="forma-app-identity__mark">
    <svg viewBox="0 0 32 32" aria-hidden="true" focusable="false"><path fill="currentColor" fill-rule="evenodd" d="M6 0h20a6 6 0 0 1 6 6v20a6 6 0 0 1-6 6H6a6 6 0 0 1-6-6V6a6 6 0 0 1 6-6Zm10 8-8 14h16Z"/></svg>
  </span>
  <span class="forma-app-identity__name">${SPONSOR.brand}${audience === "staff" ? " · Sponsor operations" : audience === "charity" ? " · Charity portal" : ""}</span>
</a>`
  );

/** Legal-sponsor disclosure (G-014, protected text per G-007). */
export const legalFooter = () =>
  tag(
    "site-footer",
    `<footer class="forma-site-footer" data-forma-tone="surface">
  <div class="forma-site-footer__inner">
    <div class="forma-site-footer__main">
      <div>
        <p class="forma-eyebrow">Legal sponsor disclosure</p>
        <p>${SPONSOR.brand} is a donor-advised fund program of ${SPONSOR.legalName}, EIN ${SPONSOR.ein}, a public charity. ${SPONSOR.legalName} holds exclusive legal control over all contributed assets. Grant recommendations are advisory and are approved or declined by ${SPONSOR.legalName}.</p>
      </div>
    </div>
    <nav class="forma-site-footer__nav" aria-label="Footer"><ul><li><a href="#program-terms">Program terms</a></li><li><a href="#privacy">Privacy</a></li><li><a href="#accessibility">Accessibility</a></li></ul></nav>
    <p class="forma-site-footer__note"><span>${SPONSOR.legalName} · EIN ${SPONSOR.ein}</span><span>Design prototype · synthetic data</span></p>
  </div>
</footer>`
  );

/**
 * Full prototype page.
 * cfg: {file, id, title, lead, audience, brand, states:[{id,title,...,html}], reqIds}
 */
export const page = (cfg) => {
  const brandAttr = cfg.brand ? ' data-forma-brand="example-harbor"' : "";
  const stateIndex = cfg.states
    .map((s, i) => `<li><a href="#${s.id}">State ${i + 1}: ${s.title}</a></li>`)
    .join("");
  return `<!doctype html>
<html lang="en" data-forma-theme="light">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>${cfg.id} ${cfg.title} | ${SPONSOR.brand} design prototype</title>
<meta name="description" content="Static Forma 1.0.2 design prototype for ${cfg.reqIds.join(", ")}. Synthetic data; no application behaviour.">
<link rel="stylesheet" href="../vendor/forma-1.0.2/all.css">
${cfg.brand ? '<link rel="stylesheet" href="../vendor/forma-1.0.2/brands/example-harbor.css">\n' : ""}</head>
<body class="forma-site"${brandAttr}>
${tag("skip-link", '<a class="forma-skip-link" href="#main-content">Skip to main content</a>')}
<header class="forma-site-header">
  <div class="forma-site-header__inner">
    <div class="forma-site-header__identity">
      ${identity(cfg.audience)}
    </div>
  </div>
</header>
<main class="forma-site__main" id="main-content" tabindex="-1">
${tag(
  "navigation-shell",
  `<div class="forma-navigation-shell">
  <details class="forma-navigation-shell__compact">
    <summary>Navigation</summary>
    <nav aria-label="Primary">${navLinks(cfg.audience, cfg.file)}</nav>
  </details>
  <nav class="forma-navigation-shell__persistent" aria-label="Primary">${navLinks(cfg.audience, cfg.file)}</nav>
  <div class="forma-navigation-shell__content">
    <header class="forma-section-heading">
      <div class="forma-section-heading__text">
        <p class="forma-eyebrow">Design prototype · ${cfg.reqIds.join(" · ")}</p>
        <h1>${cfg.title}</h1>
        <p class="forma-lead">${cfg.lead}</p>
      </div>
    </header>
    <nav aria-label="Design states on this page"><ul>${stateIndex}</ul></nav>
    ${cfg.states.map((s) => s.html).join("\n")}
  </div>
</div>`
)}
</main>
${cfg.footer === false ? "" : legalFooter()}
</body>
</html>
`;
};
