// Pure markup helpers for the Unbound screen prototypes.
// Every helper returns a string built only from documented Forma 1.0.2 classes
// and canonical markup (catalog/components/<id>.mjs, dist/patterns/<id>.html).
// No helper emits a style attribute or page-level CSS.

/** Public inert authoring tag around a canonical pattern (docs/AGENT-USAGE.md). */
export const tag = (slug, html) => `<forma-${slug} class="forma-component-tag">${html}</forma-${slug}>`;

/** Status lozenge (G-001): the text is the authority, data-state only picks the glyph. */
export const loz = (state, text) =>
  `<span class="forma-status-lozenge"${state ? ` data-state="${state}"` : ""}>${text}</span>`;

/** Freshness line (G-003). `state` is a lozenge state, or null for plain state text. */
export const fresh = ({ label = "As of", iso, shown, stateText, state = null }) =>
  `<p class="forma-freshness"><span class="forma-freshness__label">${label}</span> <time datetime="${iso}">${shown}</time> ${
    state ? loz(state, stateText) : `<span class="forma-freshness__state">${stateText}</span>`
  }</p>`;

/** Key-value list. rows: [label, valueHtml][] */
export const kv = (rows, attrs = "") =>
  `<dl class="forma-key-value-list"${attrs}>${rows.map(([k, v]) => `<div><dt>${k}</dt><dd>${v}</dd></div>`).join("")}</dl>`;

/** Low-context identifier (G-016). */
export const ident = (v) => `<span class="forma-identifier">${v}</span>`;

/** Money in tabular, slashed-zero numerals (G-017). */
export const money = (v) => `<span class="forma-identifier">${v}</span>`;

/** Consequential value marked for deliberate verification. */
export const critical = (v, verify = false) =>
  `<span class="forma-critical-value"${verify ? ' data-forma-verify="true"' : ""}>${v}</span>`;

/** Identifier with an adjacent copy control (G-016). Copy behaviour is application-owned. */
export const refWithCopy = (label, value) =>
  `<span class="forma-cluster">${ident(value)} <button type="button" aria-label="Copy ${label} ${value}">Copy</button></span>`;

/** Callout: kind = note | caution | evidence | decision. */
export const callout = (id, kind, label, body) =>
  tag("callout", `<aside class="forma-callout" data-forma-callout="${kind}" aria-labelledby="${id}"><p class="forma-callout__label" id="${id}">${label}</p>${body}</aside>`);

/** Operation status (G-006). */
export const opStatus = ({ id, state, label, title, body, actions = [], symbol = "↻", level = 3 }) =>
  tag(
    "operation-status",
    `<section class="forma-operation-status" aria-labelledby="${id}">
  <div class="forma-operation-status__symbol" aria-hidden="true">${symbol}</div>
  <div>
    ${loz(state, label)}
    <h${level} id="${id}">${title}</h${level}>
    ${body}
  </div>
  ${actions.length ? `<div class="forma-operation-status__actions">${actions.join("")}</div>` : ""}
</section>`
  );

/** Text field in the .forma-field structure. */
export const field = ({ id, label, type = "text", desc = "", value = "", attrs = "", error = "" }) => {
  const describedBy = [desc ? `${id}-description` : "", error ? `${id}-error` : ""].filter(Boolean).join(" ");
  return `<div class="forma-field">
  <label class="forma-field__label" for="${id}">${label}</label>
  ${desc ? `<span class="forma-field__description" id="${id}-description">${desc}</span>` : ""}
  <input id="${id}" name="${id}" type="${type}"${value ? ` value="${value}"` : ""}${describedBy ? ` aria-describedby="${describedBy}"` : ""}${error ? ' aria-invalid="true"' : ""}${attrs ? ` ${attrs}` : ""}>
  ${error ? validationMessage(`${id}-error`, error) : ""}
</div>`;
};

export const textarea = ({ id, label, desc = "", value = "", attrs = "", error = "" }) => {
  const describedBy = [desc ? `${id}-description` : "", error ? `${id}-error` : ""].filter(Boolean).join(" ");
  return `<div class="forma-field">
  <label class="forma-field__label" for="${id}">${label}</label>
  ${desc ? `<span class="forma-field__description" id="${id}-description">${desc}</span>` : ""}
  <textarea id="${id}" name="${id}" rows="3"${describedBy ? ` aria-describedby="${describedBy}"` : ""}${error ? ' aria-invalid="true"' : ""}${attrs ? ` ${attrs}` : ""}>${value}</textarea>
  ${error ? validationMessage(`${id}-error`, error) : ""}
</div>`;
};

export const validationMessage = (id, text) =>
  `<p class="forma-validation-message" id="${id}"><span class="forma-validation-message__mark" aria-hidden="true">!</span>${text}</p>`;

/** Native select in the Forma select-field structure. options: [value, label, selected?][] */
export const select = ({ id, label, options, desc = "", attrs = "" }) =>
  tag(
    "select",
    `<label class="forma-select-field">
  <span class="forma-field__label">${label}</span>
  <span class="forma-select" data-forma-motion-weight="standard">
    <select class="forma-select__input" id="${id}" name="${id}"${desc ? ` aria-describedby="${id}-description"` : ""}${attrs ? ` ${attrs}` : ""}>
      ${options.map(([v, l, s]) => `<option value="${v}"${s ? " selected" : ""}>${l}</option>`).join("")}
    </select>
    <span class="forma-select__indicator" aria-hidden="true"></span>
  </span>
  ${desc ? `<span class="forma-field__description" id="${id}-description">${desc}</span>` : ""}
</label>`
  );

export const checkbox = ({ id, label, desc = "", checked = false, attrs = "" }) =>
  tag(
    "checkbox",
    `<label class="forma-checkbox" data-forma-motion-weight="standard">
  <input class="forma-checkbox__input" type="checkbox" id="${id}" name="${id}" value="yes"${checked ? " checked" : ""}${desc ? ` aria-describedby="${id}-description"` : ""}${attrs ? ` ${attrs}` : ""}>
  <span class="forma-checkbox__box" aria-hidden="true"></span>
  <span class="forma-checkbox__text">
    <span class="forma-checkbox__label">${label}</span>
    ${desc ? `<span class="forma-checkbox__description" id="${id}-description">${desc}</span>` : ""}
  </span>
</label>`
  );

/** Choice group. choices: [value, title, text, checked?][] */
export const choiceGroup = ({ name, legend, choices, attrs = "" }) =>
  tag(
    "choice-group",
    `<fieldset class="forma-choice-group"${attrs ? ` ${attrs}` : ""}>
  <legend class="forma-choice-group__legend">${legend}</legend>
  ${choices
    .map(
      ([v, t, d, c]) => `<label class="forma-choice">
    <input type="radio" name="${name}" value="${v}"${c ? " checked" : ""}>
    <span class="forma-choice__content"><strong>${t}</strong><span>${d}</span></span>
  </label>`
    )
    .join("")}
</fieldset>`
  );

/** Validation summary (G-012). items: [href, text][] */
export const validationSummary = ({ id, title, items }) =>
  tag(
    "validation-summary",
    `<div class="forma-validation-summary" role="alert" tabindex="-1" aria-labelledby="${id}">
  <div class="forma-validation-summary__icon" aria-hidden="true">!</div>
  <div>
    <h3 class="forma-validation-summary__title" id="${id}">${title}</h3>
    <ul class="forma-validation-summary__list">${items.map(([h, t]) => `<li><a href="${h}">${t}</a></li>`).join("")}</ul>
  </div>
</div>`
  );

/** Modal dialog with invoker (G-005). The dialog is closed in the static page; verify-screens opens it. */
export const dialog = ({ id, invoker, title, desc, body = "", actions, invokerAttrs = "" }) =>
  tag(
    "dialog",
    `<button type="button" commandfor="${id}" command="show-modal"${invokerAttrs ? ` ${invokerAttrs}` : ""}>${invoker}</button>
<dialog class="forma-dialog" id="${id}" data-forma-motion-weight="heavy" aria-labelledby="${id}-title" aria-describedby="${id}-description">
  <form method="dialog">
    <div class="forma-dialog__body">
      <h2 id="${id}-title">${title}</h2>
      <p id="${id}-description">${desc}</p>
      ${body}
    </div>
    <div class="forma-dialog__actions">${actions}</div>
  </form>
</dialog>`
  );

/** Data grid with record projection on phones. cols: [label, sortState?][]; rows: cell html[][] */
export const dataGrid = ({ label, cols, rows }) =>
  tag(
    "data-grid",
    `<div class="forma-data-grid" role="region" tabindex="0" aria-label="${label}">
  <table>
    <caption class="forma-visually-hidden">${label}</caption>
    <thead><tr>${cols
      .map(([c, sort, plain]) =>
        plain
          ? `<th scope="col"><span class="forma-visually-hidden">${c}</span></th>`
          : `<th scope="col"${sort ? ` aria-sort="${sort}"` : ""}><button type="button">${c}</button></th>`
      )
      .join("")}</tr></thead>
    <tbody>
      ${rows.map((r) => `<tr>${r.map((cell, i) => `<td data-label="${cols[i][0]}">${cell}</td>`).join("")}</tr>`).join("\n      ")}
    </tbody>
  </table>
</div>`
  );

/** Timeline. items: {iso, shown, state, label, title, body, source}[] */
export const timeline = (label, items) =>
  tag(
    "timeline",
    `<ol class="forma-timeline" aria-label="${label}">
  ${items
    .map(
      (it) => `<li class="forma-timeline__item">
    <div class="forma-timeline__marker" aria-hidden="true"></div>
    <div class="forma-timeline__content">
      <div class="forma-timeline__meta"><time datetime="${it.iso}">${it.shown}</time>${it.label ? loz(it.state, it.label) : ""}</div>
      <h4>${it.title}</h4>
      ${it.body ? `<p>${it.body}</p>` : ""}
      ${it.source ? `<small>${it.source}</small>` : ""}
    </div>
  </li>`
    )
    .join("\n  ")}
</ol>`
  );

/**
 * Five-dimension grant progress (G-002) composed from Forma steps + status lozenge + freshness.
 * dims: {name, state, label, iso, shown}[]
 */
export const grantProgress = (id, dims) =>
  tag(
    "steps",
    `<ol class="forma-steps" role="list" aria-label="Grant progress by dimension" id="${id}">
  ${dims
    .map(
      (d) => `<li><div class="forma-stack"><p><strong>${d.name}</strong></p>${loz(d.state, d.label)}${
        d.iso ? `<p>As of <time datetime="${d.iso}">${d.shown}</time></p>` : `<p>No activity yet</p>`
      }</div></li>`
    )
    .join("\n  ")}
</ol>`
  );

/** Work queue. items: {title, state, label, text, action, actionLabel}[] */
export const workQueue = ({ id, kicker, title, count, countState = null, items, level = 3 }) =>
  tag(
    "work-queue",
    `<section class="forma-work-queue" aria-labelledby="${id}">
  <header class="forma-work-queue__header">
    <div>${kicker ? `<span class="forma-component-kicker">${kicker}</span>` : ""}<h${level} id="${id}">${title}</h${level}></div>
    ${loz(countState, count)}
  </header>
  <ol class="forma-work-queue__list">
    ${items
      .map(
        (it) => `<li class="forma-work-queue__item">
      <div><strong>${it.title}</strong><p>${it.label ? `${loz(it.state, it.label)} ` : ""}${it.text}</p></div>
      ${it.action ? `<button type="button"${it.actionLabel ? ` aria-label="${it.actionLabel}"` : ""}>${it.action}</button>` : ""}
    </li>`
      )
      .join("\n    ")}
  </ol>
</section>`
  );

/** Metric card. */
export const metric = ({ label, value, context = "", extra = "", id = "" }) =>
  tag(
    "metric-card",
    `<article class="forma-metric-card"${id ? ` aria-labelledby="${id}"` : ""}>
  <div class="forma-metric-card__label"${id ? ` id="${id}"` : ""}>${label}</div>
  <div class="forma-metric-card__value">${value}</div>
  ${context ? `<div class="forma-metric-card__context">${context}</div>` : ""}
  ${extra}
</article>`
  );

export const emptyState = ({ id, title, text, action, level = 3 }) =>
  tag(
    "empty-state",
    `<section class="forma-empty-state" aria-labelledby="${id}">
  <div class="forma-empty-state__symbol" aria-hidden="true">□</div>
  <h${level} id="${id}">${title}</h${level}>
  <p>${text}</p>
  ${action}
</section>`
  );

export const skeleton = (label) =>
  tag(
    "skeleton",
    `<section class="forma-skeleton" aria-busy="true" aria-label="${label}">
  <span class="forma-visually-hidden">${label}</span>
  <div class="forma-skeleton__line forma-skeleton__line--title"></div>
  <div class="forma-skeleton__line"></div>
  <div class="forma-skeleton__line"></div>
  <div class="forma-skeleton__line forma-skeleton__line--short"></div>
</section>`
  );

export const scopeTrail = (crumbs) =>
  tag(
    "scope-trail",
    `<nav class="forma-scope-trail" aria-label="Current scope"><ol>${crumbs
      .map((c, i) => (i === crumbs.length - 1 ? `<li aria-current="page">${c}</li>` : `<li><a href="#scope-${i}">${c}</a></li>`))
      .join("")}</ol></nav>`
  );

export const disclosure = (summary, body) =>
  tag(
    "disclosure",
    `<details class="forma-disclosure" data-forma-motion-weight="light">
  <summary>${summary}</summary>
  <div class="forma-disclosure__content">${body}</div>
</details>`
  );

export const faultInline = ({ id, title, message, reference, actions }) =>
  tag(
    "fault-inline",
    `<div class="forma-fault forma-fault--inline" id="${id}" data-forma-intent="inline" data-forma-severity="error" aria-labelledby="${id}-title">
  <div class="forma-fault__marker" aria-hidden="true">!</div>
  <div class="forma-fault__body">
    <p class="forma-fault__severity">Error</p>
    <p class="forma-fault__title" id="${id}-title"><strong>${title}</strong></p>
    <p class="forma-fault__message">${message}</p>
    <p class="forma-fault-reference">Reference <code>${reference}</code></p>
  </div>
  <div class="forma-recovery-actions" role="group" aria-label="Recovery actions">${actions}</div>
</div>`
  );

export const recordHeader = ({ type, title, lozenge, sub, crumbs, actions = "", more = "" }) =>
  tag(
    "record-header",
    `<header class="forma-record-header">
  <div class="forma-record-header__main">
    <nav aria-label="Breadcrumb">${crumbs}</nav>
    <div class="forma-record-header__title">
      <div><span class="forma-record-header__type">${type}</span><h3>${title}</h3></div>
      ${lozenge}
    </div>
    <p>${sub}</p>
  </div>
  <div class="forma-record-header__actions">
    ${actions}
    ${more ? `<details><summary>More</summary><div class="forma-record-header__menu">${more}</div></details>` : ""}
  </div>
</header>`
  );

export const mobileBar = (label, children) =>
  tag("mobile-action-bar", `<nav class="forma-mobile-action-bar" aria-label="${label}">${children}</nav>`);

export const feedItem = ({ id, source, title, state, label, iso, shown, summary, action = "" }) =>
  tag(
    "feed-item",
    `<article class="forma-feed-item" aria-labelledby="${id}">
  <div class="forma-feed-item__primary">
    <p class="forma-feed-item__source">${source}</p>
    <h4 id="${id}">${title}</h4>
  </div>
  <div class="forma-feed-item__meta">${loz(state, label)}<time datetime="${iso}">${shown}</time></div>
  <p class="forma-feed-item__summary">${summary}</p>
  ${action ? `<div class="forma-feed-item__actions">${action}</div>` : ""}
</article>`
  );

/** A labelled design-state section on a prototype page. */
export const stateSection = ({ id, n, total, persona, title, demonstrates, body }) =>
  `<section class="forma-section" id="${id}" aria-labelledby="${id}-title">
  <header class="forma-section-heading">
    <div class="forma-section-heading__text">
      <p class="forma-eyebrow">Design state ${n} of ${total} · ${persona}</p>
      <h2 id="${id}-title">${title}</h2>
      <p>Demonstrates: ${demonstrates}</p>
    </div>
  </header>
  ${body}
</section>`;
