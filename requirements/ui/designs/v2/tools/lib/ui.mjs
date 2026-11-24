// Pure markup helpers for the v2 Unbound prototypes (report 18).
// Data in, HTML string out. No helper reads state or writes files.
// Classes: Forma 1.0.2 (forma-*) where a verified component fits, otherwise the
// proposed application layer (unb-*, ../unbound-app.css) whose blocks name their gap.
import { readFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import path from "node:path";

const here = path.dirname(fileURLToPath(import.meta.url));
const registry = JSON.parse(
  readFileSync(path.resolve(here, "../../../vendor/forma-1.0.2/icons-registry.json"), "utf8")
);
const ICONS = new Map(registry.icons.map((i) => [i.name, i.shapes]));

const attrs = (o) =>
  Object.entries(o)
    .filter(([, v]) => v !== undefined && v !== null && v !== false)
    .map(([k, v]) => (v === true ? ` ${k}` : ` ${k}="${v}"`))
    .join("");

const shape = ({ element, ...rest }) => `<${element}${attrs(rest)}/>`;

/** Forma icon (registry-backed, decorative: the adjacent text carries meaning). */
export const icon = (name) => {
  const shapes = ICONS.get(name);
  if (!shapes) throw new Error(`icon not in Forma registry: ${name}`);
  return `<svg class="forma-icon__svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false">${shapes
    .map(shape)
    .join("")}</svg>`;
};
export const iconNames = () => [...ICONS.keys()];

// ---------- numbers, money, dates (UNB-UI-F-*) ----------
const fmt = new Intl.NumberFormat("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 });

/** Money: one unbreakable token, tabular figures, true minus sign, text label for negatives. */
export const money = (amount, { hero = false, signed = false } = {}) => {
  const negative = amount < 0;
  const [whole, cents] = fmt.format(Math.abs(amount)).split(".");
  const sign = negative ? "−" : signed && amount > 0 ? "+" : "";
  const centsHtml = hero ? `<span class="unb-money__cents">.${cents}</span>` : `.${cents}`;
  return `<span class="unb-money"${negative ? ' data-sign="negative"' : ""}>${sign}$${whole}${centsHtml}</span>`;
};
export const num = (v) => `<span class="unb-num">${v}</span>`;
export const time = (iso, shown) => `<time datetime="${iso}">${shown}</time>`;

// ---------- status ----------
const STATE_ICON = { ok: "success", attention: "pending", blocked: "error", info: "info", unknown: "help-circle", neutral: "clock" };

/** Status chip: sentence-case words + glyph; state never by colour alone (G-001 r1). */
export const chip = (state, text) => `<span class="unb-chip" data-state="${state}">${icon(STATE_ICON[state])}${text}</span>`;

// ---------- structure ----------
export const panel = ({ id, title, titleTag = "h2", meta = "", body, foot = "", tone, label }) =>
  `<section class="unb-panel"${tone ? ` data-tone="${tone}"` : ""}${title ? ` aria-labelledby="${id}-t"` : label ? ` aria-label="${label}"` : ""}>
${title ? `<header class="unb-panel__head"><${titleTag} id="${id}-t">${title}</${titleTag}>${meta}</header>` : ""}
<div class="unb-panel__body">${body}</div>${foot ? `<footer class="unb-panel__foot">${foot}</footer>` : ""}
</section>`;

export const notice = ({ state, iconName, title, body, actions = [], role }) =>
  `<section class="unb-notice" data-state="${state}"${role ? ` role="${role}"` : ""} aria-label="${title}">${icon(iconName ?? STATE_ICON[state])}
<p class="unb-notice__title">${title}</p>
<div class="unb-notice__body">${body}</div>${actions.length ? `<div class="unb-notice__actions">${actions.join("")}</div>` : ""}
</section>`;

export const button = (label, { primary = false, href, id, describedby } = {}) =>
  href
    ? `<a class="forma-button"${primary ? ' data-forma-variant="primary"' : ""} href="${href}"${id ? ` id="${id}"` : ""}>${label}</a>`
    : `<button class="forma-button" type="button"${primary ? ' data-forma-variant="primary"' : ""}${describedby ? ` aria-describedby="${describedby}"` : ""}>${label}</button>`;

export const quiet = (label, { href = "#", iconName } = {}) =>
  `<a class="unb-btn-quiet" href="${href}">${label}${iconName ? icon(iconName) : ""}</a>`;

export const dl = (rows) =>
  `<dl class="unb-dl">${rows.map(([k, v]) => `<div><dt>${k}</dt><dd>${v}</dd></div>`).join("")}</dl>`;

/** Things that need the user. items: {state, iconName, title, text, actions:[html]} */
export const tasks = (items) =>
  `<ul class="unb-tasks">${items
    .map(
      (t) => `<li class="unb-task" data-state="${t.state}"><span class="unb-task__icon">${icon(t.iconName ?? STATE_ICON[t.state])}</span>
<p class="unb-task__title">${t.title}</p><p class="unb-task__text">${t.text}</p>
<div class="unb-task__action">${t.actions.join("")}</div></li>`
    )
    .join("")}</ul>`;

/** Ledger: rows {what, sub, amount, status, date}; amounts right-aligned tabular. */
export const ledger = ({ caption, rows, hideCaption = true }) =>
  `<table class="unb-ledger"><caption${hideCaption ? ' class="forma-visually-hidden"' : ""}>${caption}</caption>
<thead><tr><th scope="col">Activity</th><th scope="col">Status</th><th scope="col" data-col="date">Date</th><th scope="col" data-col="amount">Amount</th></tr></thead>
<tbody>${rows
    .map(
      (r) => `<tr><td data-col="what"><span class="unb-ledger__primary">${r.what}</span><br><span class="unb-ledger__secondary">${r.sub}</span></td>
<td data-col="status">${r.status}</td><td data-col="date">${r.date}</td><td data-col="amount">${r.amount}</td></tr>`
    )
    .join("")}</tbody></table>`;

export const steps = (list) =>
  `<ol class="unb-steps">${list.map(([label, st]) => `<li data-step="${st}"${st === "current" ? ' aria-current="step"' : ""}>${label}${st === "done" ? '<span class="forma-visually-hidden"> (done)</span>' : st === "failed" ? '<span class="forma-visually-hidden"> (failed)</span>' : ""}</li>`).join("")}</ol>`;

export const skel = (shape) => `<span class="unb-skel" data-shape="${shape}"></span>`;

// ---------- frame ----------
const MARKS = {
  "unbound-default": `<svg viewBox="0 0 32 32" aria-hidden="true" focusable="false"><path fill="currentColor" d="M6 5h5v13a5 5 0 0 0 10 0V5h5v13a10 10 0 0 1-20 0Z"/></svg>`,
  "example-harbor": `<svg viewBox="0 0 32 32" aria-hidden="true" focusable="false"><path fill="currentColor" d="M16 3 4 12v3h24v-3Zm-9 14h4v8H7Zm7 0h4v8h-4Zm7 0h4v8h-4ZM4 27h24v3H4Z"/></svg>`,
};
export const BRANDS = {
  "unbound-default": { name: "Unbound Giving", css: ["../../../vendor/unbound-brands/unbound-default.css", "../../../vendor/echelon-theme-1.1.0/echelon-fonts.css"], sponsor: "Example Community Foundation", ein: "00-0000001", tz: "CT" },
  "example-harbor": { name: "Harbor Giving", css: ["../../../vendor/forma-1.0.2/brands/example-harbor.css"], sponsor: "Example Charitable Foundation", ein: "00-0000000", tz: "CT" },
};

const NAV = {
  donor: [
    ["home", "Home", "home"],
    ["grants", "Grants", "send"],
    ["money", "Contributions", "download"],
    ["documents", "Documents", "file-text"],
    ["people", "People", "users"],
  ],
  staff: [
    ["inbox", "Work queue", "inbox"],
    ["cases", "Cases", "folder"],
    ["recon", "Reconciliation", "workflow"],
    ["funds", "Funds", "database"],
    ["policy", "Policy", "settings"],
  ],
  charity: [
    ["received", "Grants received", "inbox"],
    ["org", "Organization", "globe"],
  ],
};
const TABBAR = { donor: ["home", "grants", "money", "documents"], staff: ["inbox", "cases", "recon", "funds"], charity: ["received", "org"] };

const navItems = (audience, current, counts = {}) =>
  NAV[audience]
    .map(
      ([key, label, ic]) =>
        `<li><a href="#${key}"${key === current ? ' aria-current="page"' : ""}>${icon(ic)}<span>${label}</span>${
          counts[key] ? `<span class="unb-sidenav__count">${counts[key]}<span class="forma-visually-hidden"> items</span></span>` : ""
        }</a></li>`
    )
    .join("");

const tabbar = (audience, current) =>
  `<nav class="unb-tabbar" aria-label="Primary" data-tabs="${TABBAR[audience].length}">${NAV[audience]
    .filter(([k]) => TABBAR[audience].includes(k))
    .map(([key, label, ic]) => `<a href="#${key}"${key === current ? ' aria-current="page"' : ""}>${icon(ic)}<span>${label}</span></a>`)
    .join("")}</nav>`;

/**
 * Full application page for one state.
 * cfg: {title, brand, audience, nav, user:{name, initials, role}, appContext, notifications,
 *       pagehead, body, mobileActions, footer:boolean, counts, docTitle}
 */
export const appPage = (cfg) => {
  const b = BRANDS[cfg.brand];
  const contextLabel = cfg.appContext ?? (cfg.audience === "staff" ? "Sponsor operations" : cfg.audience === "charity" ? "Charity portal" : "Donor-advised funds");
  return `<!doctype html>
<html lang="en" data-forma-theme="${cfg.theme ?? "light"}">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>${cfg.docTitle ?? cfg.title} · ${b.name}</title>
<link rel="stylesheet" href="../../../vendor/forma-1.0.2/all.css">
${b.css.map((h) => `<link rel="stylesheet" href="${h}">`).join("\n")}
<link rel="stylesheet" href="../../unbound-app.css">${(cfg.extraCss ?? []).map((h) => `\n<link rel="stylesheet" href="../../${h}">`).join("")}
</head>
<body class="unb-app"${cfg.extraCss ? "" : ""} data-forma-brand="${cfg.brand}">
<a class="unb-skip" href="#main">Skip to main content</a>
<header class="unb-appbar">
  <a class="unb-appbar__brand" href="#home"><span class="unb-appbar__mark">${MARKS[cfg.brand]}</span>${b.name}</a>
  <span class="unb-appbar__context">${contextLabel}</span>
  <div class="unb-appbar__end">
    <button class="unb-iconbtn unb-hide-narrow" type="button" aria-label="Help">${icon("help-circle")}</button>
    <button class="unb-iconbtn" type="button" aria-label="Notifications${cfg.notifications ? `, ${cfg.notifications} unread` : ""}">${icon("bell")}${cfg.notifications ? `<span class="unb-iconbtn__dot" aria-hidden="true">${cfg.notifications}</span>` : ""}</button>
    <button class="unb-user" type="button" aria-label="Account menu for ${cfg.user.name}"><span class="unb-avatar" aria-hidden="true">${cfg.user.initials}</span><span>${cfg.user.name}${cfg.user.role ? `<span class="unb-user__role"> · ${cfg.user.role}</span>` : ""}</span>${icon("chevron-down")}</button>
    <button class="unb-iconbtn unb-only-compact" type="button" aria-label="Menu" data-unb-menu>${icon("menu")}</button>
  </div>
</header>
<div class="unb-frame">
  <nav class="unb-sidenav" aria-label="Primary"><ul class="unb-sidenav__list">${navItems(cfg.audience, cfg.nav, cfg.counts)}</ul></nav>
  <main class="unb-main" id="main" tabindex="-1">
    <div class="unb-main__inner">
${cfg.pagehead}
${cfg.body}
${cfg.footer === false ? "" : legal(b)}
    </div>
  </main>
</div>
${tabbar(cfg.audience, cfg.nav)}
</body>
</html>
`;
};

/** Legal-sponsor disclosure (G-014), protected text (G-007). Quiet, always present. */
export const legal = (b) => `<footer class="unb-legal unb-small unb-muted" aria-label="Legal sponsor disclosure">
<p>${b.name} is a donor-advised fund program of ${b.sponsor} (EIN <span class="unb-num">${b.ein}</span>), a public charity. ${b.sponsor} has exclusive legal control over contributed assets. Grant recommendations are advisory and are approved or declined by ${b.sponsor}.</p>
<p><a href="#terms">Program terms</a> · <a href="#privacy">Privacy</a> · <a href="#accessibility">Accessibility</a></p>
</footer>`;

/** Page header: context line, title, actions (desktop/tablet; phone uses the sticky bar). */
export const pagehead = ({ context = [], title, sub = "", actions = [], fundSwitch = false }) =>
  `<header class="unb-pagehead">
${context.length ? `<p class="unb-pagehead__context">${context.join('<span aria-hidden="true">/</span>')}</p>` : ""}
<div class="unb-pagehead__row"><div>${
    fundSwitch
      ? `<h1><button class="unb-fundswitch" type="button" aria-haspopup="listbox" aria-label="${title}. Switch fund">${title}${icon("chevron-down")}</button></h1>`
      : `<h1>${title}</h1>`
  }${sub ? `<p class="unb-muted unb-small">${sub}</p>` : ""}</div>${actions.length ? `<div class="unb-pagehead__actions">${actions.join("")}</div>` : ""}</div>
</header>`;
