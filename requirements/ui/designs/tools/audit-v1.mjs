// UI audit harness v1 for the Unbound screen prototypes. Functional style: pure helpers,
// data in / data out; only main() performs I/O. Read-only against the repository.
import { createRequire } from "node:module";
import fs from "node:fs";
import path from "node:path";
const require = createRequire(import.meta.url);
const { chromium } = require("/opt/node-tools/node_modules/playwright");

const SCREENS = "/home/user/unbound/requirements/ui/designs/screens";
const OUT = path.dirname(new URL(import.meta.url).pathname);
const AXE = "/tmp/claude-0/-home-user/179dd429-c49d-5559-a2b4-4866d79585b5/scratchpad/axe/node_modules/axe-core/axe.min.js";

const VIEWPORTS = [
  { key: "320", width: 320, height: 800, label: "320x800" },
  { key: "768", width: 768, height: 1024, label: "768x1024" },
  { key: "1280", width: 1280, height: 900, label: "1280x900" },
  { key: "1280z200", width: 640, height: 450, label: "1280x900 @200% (640 CSS px)" },
  { key: "1280z400", width: 320, height: 225, label: "1280x900 @400% reflow (320 CSS px)" },
];

const pagesIn = (dir) => fs.readdirSync(dir).filter((f) => /^(S|W)-\d{3}.*\.html$/.test(f)).sort();

// ---------- in-page probes (pure functions of the DOM; serialised into the page) ----------
const probeCommon = () => {
  const stateOf = (el) => el.closest("section.forma-section[id]")?.id ?? el.closest("dialog[id]")?.id ?? "page-chrome";
  const isVisible = (el) => {
    const cs = getComputedStyle(el);
    const r = el.getBoundingClientRect();
    return cs.visibility !== "hidden" && cs.display !== "none" && r.width > 0 && r.height > 0 && !el.closest("dialog:not([open])") && !el.closest("details:not([open]) > :not(summary)");
  };
  const accName = (el) => {
    const lb = el.getAttribute("aria-labelledby");
    if (lb) return lb.split(/\s+/).map((id) => document.getElementById(id)?.textContent ?? "").join(" ").trim();
    if (el.getAttribute("aria-label")) return el.getAttribute("aria-label").trim();
    if (el.labels && el.labels.length) return [...el.labels].map((l) => l.textContent).join(" ").trim();
    const t = (el.innerText ?? el.textContent ?? "").trim();
    if (t) return t.replace(/\s+/g, " ");
    const img = el.querySelector("img[alt]");
    if (img) return img.alt;
    return (el.getAttribute("title") ?? el.getAttribute("placeholder") ?? "").trim();
  };
  const sel = (el) => {
    const parts = [];
    let e = el;
    for (let i = 0; e && e.nodeType === 1 && i < 4; i++, e = e.parentElement) {
      parts.unshift(e.tagName.toLowerCase() + (e.id ? "#" + e.id : "") + (e.classList.length ? "." + [...e.classList].slice(0, 2).join(".") : ""));
      if (e.id) break;
    }
    return parts.join(" > ");
  };
  window.__audit = { stateOf, isVisible, accName, sel };
};

const probeNumbers = () => {
  const { stateOf, sel } = window.__audit;
  const RE = /[−-]?\$\s?\d[\d,]*(?:\.\d{2})?|\d{1,3}(?:,\d{3})+(?:\.\d+)?|\d+\.\d{2}|\d+(?:\.\d+)?\s?%|\b[A-Z]{1,4}-\d{4}-\d{4,6}\b|\b\d{4}-\d{6}\b|\b\d{2}-\d{7}\b/g;
  const walker = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT);
  const out = [];
  let total = 0;
  for (let n = walker.nextNode(); n; n = walker.nextNode()) {
    const p = n.parentElement;
    if (!p || p.closest("script,style,dialog:not([open])")) continue;
    for (const m of n.data.matchAll(RE)) {
      const r = document.createRange();
      r.setStart(n, m.index);
      r.setEnd(n, m.index + m[0].length);
      const rects = [...r.getClientRects()].filter((x) => x.width > 0);
      if (!rects.length) continue;
      total++;
      const tops = [...new Set(rects.map((x) => Math.round(x.top / 3)))];
      if (tops.length > 1) {
        const cs = getComputedStyle(p);
        out.push({ text: m[0], state: stateOf(p), element: sel(p), lines: tops.length, overflowWrap: cs.overflowWrap, wordBreak: cs.wordBreak, fontVariantNumeric: cs.fontVariantNumeric });
      }
    }
  }
  // tabular numeral audit on money cells
  const moneyEls = [...document.querySelectorAll("td,dd,.forma-metric-card__value,.forma-key-value-list dd")].filter((e) => /\$\s?\d/.test(e.textContent) && e.textContent.trim().length < 30);
  const tabular = moneyEls.map((e) => ({ state: stateOf(e), element: sel(e), text: e.textContent.trim(), fontVariantNumeric: getComputedStyle(e).fontVariantNumeric, textAlign: getComputedStyle(e).textAlign }));
  return { matched: total, wrapped: out, moneyCells: tabular };
};


const probeWordBreaks = () => {
  const { stateOf, sel } = window.__audit;
  const walker = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT);
  const out = [];
  for (let n = walker.nextNode(); n; n = walker.nextNode()) {
    const p = n.parentElement;
    if (!p || p.closest("script,style,dialog:not([open])")) continue;
    for (const m of n.data.matchAll(/[A-Za-z][A-Za-z'’]{3,}/g)) {
      const r = document.createRange();
      r.setStart(n, m.index); r.setEnd(n, m.index + m[0].length);
      const rects = [...r.getClientRects()].filter((x) => x.width > 0);
      if (new Set(rects.map((x) => Math.round(x.top / 3))).size > 1) out.push({ word: m[0], state: stateOf(p), element: sel(p), overflowWrap: getComputedStyle(p).overflowWrap, wordBreak: getComputedStyle(p).wordBreak });
    }
  }
  return out;
};
const probeSticky = () => {
  const { stateOf, sel, isVisible } = window.__audit;
  return [...document.querySelectorAll("body *")].filter((e) => isVisible(e) && /(sticky|fixed)/.test(getComputedStyle(e).position)).map((e) => { const r = e.getBoundingClientRect(); return { state: stateOf(e), element: sel(e), position: getComputedStyle(e).position, top: getComputedStyle(e).top, bottom: getComputedStyle(e).bottom, h: Math.round(r.height), w: Math.round(r.width), text: (e.innerText ?? "").replace(/\s+/g, " ").slice(0, 60) }; });
};
const probeTypography = () => {
  const { stateOf } = window.__audit;
  const fs = (q) => [...new Set([...document.querySelectorAll(q)].map((e) => getComputedStyle(e).fontSize + " " + getComputedStyle(e).fontFamily.split(",")[0] + " " + getComputedStyle(e).textTransform))];
  const cur = [...document.querySelectorAll("[aria-current=page]")].filter((e) => /^\d+$/.test(e.textContent.trim())).map((e) => { const cs = getComputedStyle(e); let bg = "rgba(0, 0, 0, 0)"; for (let x = e; x && /rgba\(0, 0, 0, 0\)|transparent/.test(bg); x = x.parentElement) bg = getComputedStyle(x).backgroundColor; return { state: stateOf(e), color: cs.color, bg }; });
  return { lozenge: fs(".forma-status-lozenge"), eyebrow: fs(".forma-eyebrow"), h1: fs("h1"), h2: fs("h2"), h3: fs("h3"), metric: fs(".forma-metric-card__value"), identifier: fs(".forma-identifier"), body: fs("p"), th: fs("th"), small: fs("small"), dialogTitle: fs("dialog h2"), paginationCurrent: cur, numericCellAlign: [...document.querySelectorAll("td")].filter((t) => /^[−$\d.,%\s]+$/.test(t.textContent.trim()) && /\d/.test(t.textContent)).map((t) => getComputedStyle(t).textAlign + "/" + getComputedStyle(t).fontVariantNumeric).reduce((a, k) => ({ ...a, [k]: (a[k] ?? 0) + 1 }), {}) };
};

const probeOverflow = () => {
  const { stateOf, sel, isVisible } = window.__audit;
  const vw = document.documentElement.clientWidth;
  const page = { scrollWidth: document.documentElement.scrollWidth, clientWidth: vw, overflows: document.documentElement.scrollWidth > vw + 1 };
  const spill = [];
  const scrollers = [];
  const clipped = [];
  for (const el of document.querySelectorAll("body *")) {
    if (!isVisible(el)) continue;
    const cs = getComputedStyle(el);
    const r = el.getBoundingClientRect();
    if (r.right > vw + 1 && cs.position !== "fixed") spill.push({ state: stateOf(el), element: sel(el), right: Math.round(r.right), text: (el.innerText ?? "").slice(0, 60) });
    if (el.scrollWidth > el.clientWidth + 1 && el.clientWidth > 0) {
      if (/(auto|scroll)/.test(cs.overflowX)) scrollers.push({ state: stateOf(el), element: sel(el), scrollWidth: el.scrollWidth, clientWidth: el.clientWidth, focusable: el.tabIndex >= 0, role: el.getAttribute("role"), label: el.getAttribute("aria-label") });
      else if (/(hidden|clip)/.test(cs.overflowX)) clipped.push({ state: stateOf(el), element: sel(el), scrollWidth: el.scrollWidth, clientWidth: el.clientWidth, text: (el.innerText ?? "").slice(0, 60) });
      else if (el.children.length === 0 || el.scrollWidth - el.clientWidth > 4) spill.push({ state: stateOf(el), element: sel(el), contentOverflowsBy: el.scrollWidth - el.clientWidth, text: (el.innerText ?? "").slice(0, 60) });
    }
  }
  // keep only outermost spill per state
  const dedup = spill.filter((s, i) => !spill.slice(0, i).some((t) => s.element.startsWith(t.element) && t.state === s.state)).slice(0, 40);
  return { page, spill: dedup, spillCount: spill.length, scrollRegions: scrollers, clippedX: clipped };
};

const probeSemantics = () => {
  const { stateOf, sel, isVisible, accName } = window.__audit;
  const hs = [...document.querySelectorAll("h1,h2,h3,h4,h5,h6,[role=heading]")].filter((h) => !h.closest("dialog:not([open])"));
  const outline = hs.map((h) => ({ level: h.getAttribute("aria-level") ? +h.getAttribute("aria-level") : +h.tagName[1], text: h.textContent.trim().replace(/\s+/g, " ").slice(0, 80), state: stateOf(h) }));
  const skips = outline.filter((h, i) => i > 0 && h.level > outline[i - 1].level + 1).map((h, i) => ({ ...h, prevLevel: outline[outline.indexOf(h) - 1].level }));
  const landmarks = [...document.querySelectorAll("main,nav,header,footer,aside,[role=main],[role=navigation],[role=banner],[role=contentinfo],[role=complementary],[role=region],section[aria-label],section[aria-labelledby],form[aria-label]")].map((e) => ({ tag: e.tagName.toLowerCase(), role: e.getAttribute("role"), name: e.getAttribute("aria-label") ?? (e.getAttribute("aria-labelledby") ? accName(e) : ""), state: stateOf(e) }));
  const navNames = landmarks.filter((l) => l.tag === "nav").map((l) => l.name);
  const interactive = [...document.querySelectorAll("a[href],button,input,select,textarea,summary,[role=button],[role=link],[tabindex]:not([tabindex='-1'])")].filter((e) => !e.closest("dialog:not([open])"));
  const nameless = interactive.filter((e) => e.type !== "hidden" && !accName(e)).map((e) => ({ state: stateOf(e), element: sel(e) }));
  const GENERIC = /^(view|details|more|click here|here|open|learn more|read more|show|edit|select|download|copy|link)$/i;
  const generic = interactive.filter((e) => GENERIC.test((e.innerText ?? "").trim()) || GENERIC.test(accName(e))).map((e) => ({ state: stateOf(e), element: sel(e), visibleText: (e.innerText ?? "").trim(), accName: accName(e) }));
  const fields = [...document.querySelectorAll("input:not([type=hidden]),select,textarea")].filter((e) => !e.closest("dialog:not([open])"));
  const unlabelled = fields.filter((f) => !accName(f) || (f.labels?.length === 0 && !f.getAttribute("aria-label") && !f.getAttribute("aria-labelledby"))).map((f) => ({ state: stateOf(f), element: sel(f), type: f.type, accName: accName(f), placeholder: f.placeholder }));
  const fieldSummary = fields.map((f) => ({ state: stateOf(f), type: f.type, name: accName(f), describedby: !!f.getAttribute("aria-describedby"), invalid: f.getAttribute("aria-invalid"), required: f.required || f.getAttribute("aria-required") === "true", autocomplete: f.getAttribute("autocomplete"), inputmode: f.getAttribute("inputmode") }));
  const tables = [...document.querySelectorAll("table")].map((t) => ({ state: stateOf(t), th: t.querySelectorAll("th").length, scopedTh: t.querySelectorAll("th[scope]").length, caption: t.querySelector("caption")?.textContent.trim() ?? null, name: t.getAttribute("aria-label") ?? t.getAttribute("aria-labelledby") ?? null }));
  const live = [...document.querySelectorAll("[aria-live],[role=status],[role=alert],[role=log],output")].map((e) => ({ state: stateOf(e), element: sel(e), live: e.getAttribute("aria-live"), role: e.getAttribute("role"), text: e.textContent.trim().slice(0, 60) }));
  const colourOnly = [...document.querySelectorAll("[data-state]")].filter((e) => isVisible(e) && !e.textContent.trim() && !e.getAttribute("aria-label")).map((e) => ({ state: stateOf(e), element: sel(e), dataState: e.dataset.state }));
  const lozenges = [...document.querySelectorAll(".forma-status-lozenge")].map((e) => ({ state: stateOf(e), text: e.textContent.trim(), dataState: e.dataset.state }));
  const busy = [...document.querySelectorAll("[aria-busy=true]")].map((e) => ({ state: stateOf(e), element: sel(e), label: e.getAttribute("aria-label") }));
  const disabled = [...document.querySelectorAll("button:disabled,[aria-disabled=true],input:disabled")].map((e) => ({ state: stateOf(e), element: sel(e), text: accName(e) }));
  const mono = [...document.querySelectorAll("body *")].filter((e) => isVisible(e) && e.children.length === 0 && e.textContent.trim() && /mono/i.test(getComputedStyle(e).fontFamily) ).map((e) => ({ state: stateOf(e), text: e.textContent.trim().slice(0, 40), transform: getComputedStyle(e).textTransform, cls: e.className }));
  const h1s = outline.filter((h) => h.level === 1).length;
  const kickers = [...document.querySelectorAll(".forma-component-kicker")].map((e) => ({ state: stateOf(e), text: e.textContent.trim() }));
  const buttonsPerState = {};
  for (const b of [...document.querySelectorAll("button, a[href]")].filter((e) => !e.closest("nav,dialog:not([open]),footer,header.forma-site-header"))) {
    const s = stateOf(b);
    (buttonsPerState[s] ??= []).push(accName(b).slice(0, 50));
  }
  return { h1s, outline, skips, landmarks, navNames, nameless, generic, unlabelled, fieldSummary, tables, live, colourOnly, lozenges, busy, disabled, mono, kickers, buttonsPerState };
};

const probeTargets = () => {
  const { stateOf, sel, isVisible, accName } = window.__audit;
  const els = [...document.querySelectorAll("a[href],button,input:not([type=hidden]),select,textarea,summary,[role=button],[tabindex]:not([tabindex='-1'])")].filter((e) => isVisible(e));
  return els.map((e) => {
    const r = e.getBoundingClientRect();
    const inline = e.tagName === "A" && getComputedStyle(e).display === "inline" && !!e.closest("p,li,dd,td") && (e.parentElement.textContent.trim().length > (e.textContent.trim().length + 10));
    return { state: stateOf(e), element: sel(e), name: accName(e).slice(0, 50), w: Math.round(r.width * 10) / 10, h: Math.round(r.height * 10) / 10, inline, type: e.type ?? null };
  }).filter((t) => t.w < 44 || t.h < 44);
};

const FOCUS_PROPS = ["outlineStyle", "outlineWidth", "outlineColor", "boxShadow", "backgroundColor", "borderColor", "textDecorationLine", "color"];
const tagFocusables = (props) => {
  const els = [...document.querySelectorAll("a[href],button,input,select,textarea,summary,[tabindex]")];
  return els.map((e, i) => {
    e.dataset.auditIdx = i;
    const cs = getComputedStyle(e);
    return Object.fromEntries(props.map((p) => [p, cs[p]]));
  });
};
const readActive = (props) => {
  const e = document.activeElement;
  if (!e || e === document.body) return { body: true };
  const { stateOf, accName, isVisible } = window.__audit;
  const r = e.getBoundingClientRect();
  const cs = getComputedStyle(e);
  return { idx: e.dataset.auditIdx ?? null, tag: e.tagName.toLowerCase(), name: accName(e).slice(0, 60), state: stateOf(e), box: { x: Math.round(r.left + scrollX), y: Math.round(r.top + scrollY), w: Math.round(r.width), h: Math.round(r.height) }, visible: isVisible(e), obscured: (() => { const pts = [[r.left + r.width / 2, r.top + r.height / 2], [r.left + 2, r.top + 2], [r.right - 2, r.bottom - 2], [r.left + 2, r.bottom - 2], [r.right - 2, r.top + 2]]; const hits = pts.map(([x, y]) => { const t = document.elementFromPoint(x, y); return !!t && (t === e || e.contains(t) || t.contains(e)); }); const by = (() => { const t = document.elementFromPoint(r.left + r.width / 2, r.top + r.height / 2); return t && !(t === e || e.contains(t) || t.contains(e)) ? (t.closest("[class]")?.className ?? t.tagName) : null; })(); return { hitCount: hits.filter(Boolean).length, by: by && String(by).slice(0, 60) }; })(), offscreen: r.right < 0 || r.bottom + scrollY < 0 || r.left > document.documentElement.clientWidth, style: Object.fromEntries(props.map((p) => [p, cs[p]])) };
};
const textSpacingCss = `*{line-height:1.5!important;letter-spacing:.12em!important;word-spacing:.16em!important}p{margin-bottom:2em!important}`;
const probeClipped = () => {
  const { stateOf, sel, isVisible } = window.__audit;
  return [...document.querySelectorAll("body *")].filter((e) => {
    if (!isVisible(e)) return false;
    const cs = getComputedStyle(e);
    const hid = (v) => /(hidden|clip)/.test(v);
    return ((hid(cs.overflowY) || hid(cs.overflow)) && e.scrollHeight > e.clientHeight + 1) || ((hid(cs.overflowX) || hid(cs.overflow)) && e.scrollWidth > e.clientWidth + 1) || (cs.textOverflow === "ellipsis" && e.scrollWidth > e.clientWidth + 1) || (cs.webkitLineClamp && cs.webkitLineClamp !== "none" && e.scrollHeight > e.clientHeight + 1);
  }).map((e) => ({ state: stateOf(e), element: sel(e), text: (e.innerText ?? "").slice(0, 60), sh: e.scrollHeight, ch: e.clientHeight, sw: e.scrollWidth, cw: e.clientWidth }));
};

// ---------- pure analysis helpers ----------
const focusChanged = (before, after) => !!before && !!after && ["outlineStyle", "outlineWidth", "boxShadow", "backgroundColor", "borderColor", "textDecorationLine", "color"].some((p) => before[p] !== after[p]) && !(after.outlineStyle === "none" && before.boxShadow === after.boxShadow && before.backgroundColor === after.backgroundColor && before.borderColor === after.borderColor && before.textDecorationLine === after.textDecorationLine && before.color === after.color);
const analyseFocus = (seq, baseline) => {
  const items = seq.filter((s) => !s.body);
  const notVisible = items.filter((s) => s.idx != null && !focusChanged(baseline[s.idx], s.style)).map(({ style, ...r }) => r);
  const hidden = items.filter((s) => !s.visible || s.offscreen || s.box.w === 0 || s.box.h === 0).map(({ style, ...r }) => r);
  const backwards = items.flatMap((s, i) => (i > 0 && s.box.y < items[i - 1].box.y - 40 && s.state === items[i - 1].state ? [{ from: items[i - 1].name, to: s.name, state: s.state, dy: s.box.y - items[i - 1].box.y }] : []));
  const obscured = items.filter((s) => s.obscured && s.obscured.hitCount === 0).map(({ style, ...r }) => r);
  const partlyObscured = items.filter((s) => s.obscured && s.obscured.hitCount > 0 && s.obscured.hitCount < 5).map(({ style, ...r }) => r);
  const repeats = items.filter((s, i) => i > 0 && s.idx === items[i - 1].idx);
  return { obscured, partlyObscured, stops: items.length, wrapped: seq.wrapped ?? false, trap: repeats.length > 2, notVisible, hidden, backwards, sequence: items.map(({ style, ...r }) => r) };
};
const summariseAxe = (res) => res.violations.map((v) => ({ id: v.id, impact: v.impact, tags: v.tags.filter((t) => /^wcag\d/.test(t)), help: v.help, nodes: v.nodes.length, targets: v.nodes.slice(0, 6).map((n) => n.target.join(" ")), summary: v.nodes[0]?.failureSummary?.slice(0, 200) }));

// ---------- I/O ----------
const runAxe = async (page, axeSrc, tags) => {
  await page.evaluate(axeSrc);
  return page.evaluate((t) => window.axe.run(document, { runOnly: { type: "tag", values: t }, resultTypes: ["violations"] }), tags);
};

const tabWalk = async (page, max = 200) => {
  const baseline = await page.evaluate(tagFocusables, FOCUS_PROPS);
  await page.evaluate(() => { document.activeElement?.blur(); window.scrollTo(0, 0); });
  const seq = [];
  let first = null;
  for (let i = 0; i < max; i++) {
    await page.keyboard.press("Tab");
    const a = await page.evaluate(readActive, FOCUS_PROPS);
    if (a.body) { seq.wrapped = true; break; }
    if (first && a.idx === first) { seq.wrapped = true; break; }
    first ??= a.idx;
    seq.push(a);
  }
  return analyseFocus(seq, baseline);
};

const dialogChecks = async (page, axeSrc) => {
  const ids = await page.$$eval("dialog[id]", (ds) => ds.map((d) => d.id));
  const out = [];
  for (const id of ids) {
    await page.evaluate((d) => document.getElementById(d).showModal(), id);
    await page.evaluate(probeCommon);
    const res = await runAxe(page, axeSrc, ["wcag2a", "wcag2aa", "wcag21a", "wcag21aa", "wcag22aa"]);
    const initial = await page.evaluate(() => ({ tag: document.activeElement?.tagName, name: (document.activeElement?.innerText || document.activeElement?.getAttribute("aria-label") || "").trim().slice(0, 50) }));
    const stops = [];
    for (let i = 0; i < 12; i++) { await page.keyboard.press("Tab"); stops.push(await page.evaluate((d) => ({ inDialog: !!document.activeElement?.closest("#" + d), name: (document.activeElement?.innerText || document.activeElement?.getAttribute("aria-label") || document.activeElement?.tagName || "").trim().slice(0, 40) }), id)); }
    const targets = await page.evaluate(probeTargets);
    await page.keyboard.press("Escape");
    const closedByEsc = await page.evaluate((d) => !document.getElementById(d).open, id);
    await page.evaluate((d) => document.getElementById(d).open && document.getElementById(d).close(), id);
    out.push({ dialog: id, axe: summariseAxe(res), initialFocus: initial, focusContained: stops.every((s) => s.inDialog), tabStops: [...new Set(stops.map((s) => s.name))], closedByEsc, smallTargets: targets.filter((t) => t.state === id && (t.w < 24 || t.h < 24)) });
  }
  return out;
};

const auditOne = async (browser, file, vp, axeSrc) => {
  const ctx = await browser.newContext({ viewport: { width: vp.width, height: vp.height }, deviceScaleFactor: 1, reducedMotion: "reduce" });
  const page = await ctx.newPage();
  const url = "file://" + path.join(SCREENS, file);
  await page.goto(url);
  await page.evaluate(probeCommon);
  const base = file.replace(/\.html$/, "");
  const shot = path.join(OUT, "shots", `${base}-${vp.key}.png`);
  await page.screenshot({ path: shot, fullPage: true });
  const numbers = await page.evaluate(probeNumbers);
  const overflow = await page.evaluate(probeOverflow);
  const wordBreaks = await page.evaluate(probeWordBreaks);
  const sticky = await page.evaluate(probeSticky);
  const typography = vp.key === "1280" || vp.key === "320" ? await page.evaluate(probeTypography) : undefined;
  const semantics = vp.key === "1280" || vp.key === "320" ? await page.evaluate(probeSemantics) : undefined;
  const targets = await page.evaluate(probeTargets);
  const axeCore = summariseAxe(await runAxe(page, axeSrc, ["wcag2a", "wcag2aa", "wcag21a", "wcag21aa", "wcag22aa"]));
  const axeBP = vp.key === "1280" || vp.key === "320" ? summariseAxe(await runAxe(page, axeSrc, ["best-practice"])) : undefined;
  const clippedBefore = await page.evaluate(probeClipped);
  const focus = await tabWalk(page);
  const dialogs = vp.key === "320" || vp.key === "1280" ? await dialogChecks(page, axeSrc) : undefined;
  await page.addStyleTag({ content: textSpacingCss });
  await page.evaluate(probeCommon);
  const clippedAfter = await page.evaluate(probeClipped);
  const overflowSpacing = await page.evaluate(probeOverflow);
  const numbersSpacing = await page.evaluate(probeNumbers);
  const keyOf = (c) => c.element + c.text;
  const textSpacing = { newlyClipped: clippedAfter.filter((c) => !clippedBefore.some((b) => keyOf(b) === keyOf(c))), clippedBefore, pageOverflow: overflowSpacing.page, moneyWrapsUnderSpacing: numbersSpacing.wrapped.length };
  await ctx.close();
  return {
    viewport: vp.label, key: vp.key, screenshot: shot,
    numbers, wordBreaks, sticky, typography, overflow, semantics,
    targets: { under24: targets.filter((t) => (t.w < 24 || t.h < 24)), under24NonInline: targets.filter((t) => (t.w < 24 || t.h < 24) && !t.inline).length, under44: vp.width <= 320 ? targets : undefined, under44Count: targets.length },
    axe: axeCore, axeBestPractice: axeBP, focus, dialogs, textSpacing,
  };
};

const pool = async (tasks, n) => {
  const results = [];
  let i = 0;
  const worker = async () => { while (i < tasks.length) { const k = i++; results[k] = await tasks[k](); } };
  await Promise.all(Array.from({ length: n }, worker));
  return results;
};

const main = async () => {
  const axeSrc = fs.readFileSync(AXE, "utf8");
  const browser = await chromium.launch();
  const files = pagesIn(SCREENS);
  const jobs = files.flatMap((f) => VIEWPORTS.map((vp) => async () => { process.stderr.write(`${f} ${vp.key}\n`); return { file: f, ...(await auditOne(browser, f, vp, axeSrc)) }; }));
  const flat = await pool(jobs, 4);
  await browser.close();
  const byPage = Object.fromEntries(files.map((f) => [f, Object.fromEntries(flat.filter((r) => r.file === f).map(({ file, ...r }) => [r.key, r]))]));
  fs.writeFileSync(path.join(OUT, "checks-v1.json"), JSON.stringify({ generated: new Date().toISOString(), viewports: VIEWPORTS, axeVersion: "axe-core (scratchpad)", pages: byPage }, null, 1));
  const brief = files.map((f) => ({ f, ...Object.fromEntries(Object.entries(byPage[f]).map(([k, r]) => [k, `wb:${r.wordBreaks.length} obs:${r.focus.obscured.length}/${r.focus.partlyObscured.length} wrap:${r.numbers.wrapped.length} ovf:${r.overflow.page.overflows ? "Y" : "n"}/${r.overflow.spill.length} axe:${r.axe.map((a) => a.id + "(" + a.nodes + ")").join("+") || 0} t24:${r.targets.under24NonInline} fNV:${r.focus.notVisible.length} fH:${r.focus.hidden.length} ts:${r.textSpacing.newlyClipped.length}`])) }));
  console.log(JSON.stringify(brief, null, 1));
};

main().catch((e) => { console.error(e); process.exit(1); });
