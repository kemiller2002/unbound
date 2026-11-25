/// audit-v1: UI audit harness v1 for the Unbound screen prototypes (port of tools/audit-v1.mjs).
/// Read-only against the repository: writes <out>/checks-v1.json and <out>/shots/*.png.
/// The probes run inside the page (JavaScript, pure functions of the DOM); the analysis of what they return is the
/// pure F# below; only `run` and the page steps perform I/O.
module Unbound.Designs.Verify.AuditV1

open System
open System.IO
open System.Text.RegularExpressions
open System.Threading
open System.Threading.Tasks
open Unbound.Designs.Verify.JsCompat

type Viewport = { Key: string; Width: int; Height: int; Label: string }

let viewports =
    [ { Key = "320"; Width = 320; Height = 800; Label = "320x800" }
      { Key = "768"; Width = 768; Height = 1024; Label = "768x1024" }
      { Key = "1280"; Width = 1280; Height = 900; Label = "1280x900" }
      { Key = "1280z200"; Width = 640; Height = 450; Label = "1280x900 @200% (640 CSS px)" }
      { Key = "1280z400"; Width = 320; Height = 225; Label = "1280x900 @400% reflow (320 CSS px)" } ]

let viewportJson (v: Viewport) = JObj [ "key", str v.Key; "width", jint v.Width; "height", jint v.Height; "label", str v.Label ]

let axeTags = [ "wcag2a"; "wcag2aa"; "wcag21a"; "wcag21aa"; "wcag22aa" ]

let isPageFile (f: string) = Regex.IsMatch(f, @"^(S|W)-[0-9]{3}.*\.html$")

// ---------- in-page probes (pure functions of the DOM; JavaScript evaluated in the page) ----------

let probeCommon = """() => {
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
}"""

let probeNumbers = """() => {
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
}"""

let probeWordBreaks = """() => {
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
}"""

let probeSticky = """() => {
  const { stateOf, sel, isVisible } = window.__audit;
  return [...document.querySelectorAll("body *")].filter((e) => isVisible(e) && /(sticky|fixed)/.test(getComputedStyle(e).position)).map((e) => { const r = e.getBoundingClientRect(); return { state: stateOf(e), element: sel(e), position: getComputedStyle(e).position, top: getComputedStyle(e).top, bottom: getComputedStyle(e).bottom, h: Math.round(r.height), w: Math.round(r.width), text: (e.innerText ?? "").replace(/\s+/g, " ").slice(0, 60) }; });
}"""

let probeTypography = """() => {
  const { stateOf } = window.__audit;
  const fs = (q) => [...new Set([...document.querySelectorAll(q)].map((e) => getComputedStyle(e).fontSize + " " + getComputedStyle(e).fontFamily.split(",")[0] + " " + getComputedStyle(e).textTransform))];
  const cur = [...document.querySelectorAll("[aria-current=page]")].filter((e) => /^\d+$/.test(e.textContent.trim())).map((e) => { const cs = getComputedStyle(e); let bg = "rgba(0, 0, 0, 0)"; for (let x = e; x && /rgba\(0, 0, 0, 0\)|transparent/.test(bg); x = x.parentElement) bg = getComputedStyle(x).backgroundColor; return { state: stateOf(e), color: cs.color, bg }; });
  return { lozenge: fs(".forma-status-lozenge"), eyebrow: fs(".forma-eyebrow"), h1: fs("h1"), h2: fs("h2"), h3: fs("h3"), metric: fs(".forma-metric-card__value"), identifier: fs(".forma-identifier"), body: fs("p"), th: fs("th"), small: fs("small"), dialogTitle: fs("dialog h2"), paginationCurrent: cur, numericCellAlign: [...document.querySelectorAll("td")].filter((t) => /^[−$\d.,%\s]+$/.test(t.textContent.trim()) && /\d/.test(t.textContent)).map((t) => getComputedStyle(t).textAlign + "/" + getComputedStyle(t).fontVariantNumeric).reduce((a, k) => ({ ...a, [k]: (a[k] ?? 0) + 1 }), {}) };
}"""

let probeOverflow = """() => {
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
}"""

let probeSemantics = """() => {
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
}"""

let probeTargets = """() => {
  const { stateOf, sel, isVisible, accName } = window.__audit;
  const els = [...document.querySelectorAll("a[href],button,input:not([type=hidden]),select,textarea,summary,[role=button],[tabindex]:not([tabindex='-1'])")].filter((e) => isVisible(e));
  return els.map((e) => {
    const r = e.getBoundingClientRect();
    const inline = e.tagName === "A" && getComputedStyle(e).display === "inline" && !!e.closest("p,li,dd,td") && (e.parentElement.textContent.trim().length > (e.textContent.trim().length + 10));
    return { state: stateOf(e), element: sel(e), name: accName(e).slice(0, 50), w: Math.round(r.width * 10) / 10, h: Math.round(r.height * 10) / 10, inline, type: e.type ?? null };
  }).filter((t) => t.w < 44 || t.h < 44);
}"""

let tagFocusables = """(props) => {
  const els = [...document.querySelectorAll("a[href],button,input,select,textarea,summary,[tabindex]")];
  return els.map((e, i) => {
    e.dataset.auditIdx = i;
    const cs = getComputedStyle(e);
    return Object.fromEntries(props.map((p) => [p, cs[p]]));
  });
}"""

let readActive = """(props) => {
  const e = document.activeElement;
  if (!e || e === document.body) return { body: true };
  const { stateOf, accName, isVisible } = window.__audit;
  const r = e.getBoundingClientRect();
  const cs = getComputedStyle(e);
  return { idx: e.dataset.auditIdx ?? null, tag: e.tagName.toLowerCase(), name: accName(e).slice(0, 60), state: stateOf(e), box: { x: Math.round(r.left + scrollX), y: Math.round(r.top + scrollY), w: Math.round(r.width), h: Math.round(r.height) }, visible: isVisible(e), obscured: (() => { const pts = [[r.left + r.width / 2, r.top + r.height / 2], [r.left + 2, r.top + 2], [r.right - 2, r.bottom - 2], [r.left + 2, r.bottom - 2], [r.right - 2, r.top + 2]]; const hits = pts.map(([x, y]) => { const t = document.elementFromPoint(x, y); return !!t && (t === e || e.contains(t) || t.contains(e)); }); const by = (() => { const t = document.elementFromPoint(r.left + r.width / 2, r.top + r.height / 2); return t && !(t === e || e.contains(t) || t.contains(e)) ? (t.closest("[class]")?.className ?? t.tagName) : null; })(); return { hitCount: hits.filter(Boolean).length, by: by && String(by).slice(0, 60) }; })(), offscreen: r.right < 0 || r.bottom + scrollY < 0 || r.left > document.documentElement.clientWidth, style: Object.fromEntries(props.map((p) => [p, cs[p]])) };
}"""

let probeClipped = """() => {
  const { stateOf, sel, isVisible } = window.__audit;
  return [...document.querySelectorAll("body *")].filter((e) => {
    if (!isVisible(e)) return false;
    const cs = getComputedStyle(e);
    const hid = (v) => /(hidden|clip)/.test(v);
    return ((hid(cs.overflowY) || hid(cs.overflow)) && e.scrollHeight > e.clientHeight + 1) || ((hid(cs.overflowX) || hid(cs.overflow)) && e.scrollWidth > e.clientWidth + 1) || (cs.textOverflow === "ellipsis" && e.scrollWidth > e.clientWidth + 1) || (cs.webkitLineClamp && cs.webkitLineClamp !== "none" && e.scrollHeight > e.clientHeight + 1);
  }).map((e) => ({ state: stateOf(e), element: sel(e), text: (e.innerText ?? "").slice(0, 60), sh: e.scrollHeight, ch: e.clientHeight, sw: e.scrollWidth, cw: e.clientWidth }));
}"""

let focusProps = [ "outlineStyle"; "outlineWidth"; "outlineColor"; "boxShadow"; "backgroundColor"; "borderColor"; "textDecorationLine"; "color" ]

let textSpacingCss =
    "*{line-height:1.5!important;letter-spacing:.12em!important;word-spacing:.16em!important}p{margin-bottom:2em!important}"

let axeRunScript = """(t) => window.axe.run(document, { runOnly: { type: "tag", values: t }, resultTypes: ["violations"] }).then((r) => ({ violations: r.violations.map((v) => ({ id: v.id, impact: v.impact, tags: v.tags, help: v.help, nodes: v.nodes.map((n) => ({ target: n.target, failureSummary: n.failureSummary })) })) }))"""

// ---------- pure analysis ----------

let private changedProps = [ "outlineStyle"; "outlineWidth"; "boxShadow"; "backgroundColor"; "borderColor"; "textDecorationLine"; "color" ]

/// Whether the focused element's computed style differs visibly from its unfocused baseline.
let focusChanged (before: Json) (after: Json) =
    let eq p = before ? (p: string) = after ? (p: string)
    truthy before && truthy after
    && changedProps |> List.exists (eq >> not)
    && not (after ? outlineStyle = JStr "none" && eq "boxShadow" && eq "backgroundColor" && eq "borderColor" && eq "textDecorationLine" && eq "color")

let private baselineAt (baseline: Json list) (idx: Json) =
    match idx with
    | JStr s ->
        match Int32.TryParse s with
        | true, i when i >= 0 && i < baseline.Length && string i = s -> baseline.[i]
        | _ -> JNull
    | _ -> JNull

/// The keyboard-walk analysis (focus visible, hidden, backwards, obscured, traps) over the sequence of focus stops.
let analyseFocus (items: Json list) (wrapped: bool) (baseline: Json list) : Json =
    let noStyle = without "style"
    let num (j: Json) = asNum j
    let notVisible = items |> List.filter (fun s -> s ? idx <> JNull && not (focusChanged (baselineAt baseline (s ? idx)) (s ? style))) |> List.map noStyle
    let hidden =
        items
        |> List.filter (fun s -> not (truthy (s ? visible)) || truthy (s ? offscreen) || num (s ? box ? w) = 0.0 || num (s ? box ? h) = 0.0)
        |> List.map noStyle
    let pairs = items |> List.pairwise
    let backwards =
        pairs
        |> List.filter (fun (prev, s) -> num (s ? box ? y) < num (prev ? box ? y) - 40.0 && s ? state = prev ? state)
        |> List.map (fun (prev, s) -> JObj [ "from", prev ? name; "to", s ? name; "state", s ? state; "dy", JNum(num (s ? box ? y) - num (prev ? box ? y)) ])
    let hits (s: Json) = num (s ? obscured ? hitCount)
    let obscured = items |> List.filter (fun s -> truthy (s ? obscured) && hits s = 0.0) |> List.map noStyle
    let partlyObscured = items |> List.filter (fun s -> truthy (s ? obscured) && hits s > 0.0 && hits s < 5.0) |> List.map noStyle
    let repeats = pairs |> List.filter (fun (prev, s) -> s ? idx = prev ? idx) |> List.length
    JObj [
        "obscured", JArr obscured
        "partlyObscured", JArr partlyObscured
        "stops", jint items.Length
        "wrapped", JBool wrapped
        "trap", JBool(repeats > 2)
        "notVisible", JArr notVisible
        "hidden", JArr hidden
        "backwards", JArr backwards
        "sequence", JArr(items |> List.map noStyle)
    ]

/// axe violations as recorded: WCAG tags only, first six targets, first node's failure summary (200 chars).
let summariseAxe (violations: Json list) : Json list =
    violations
    |> List.map (fun v ->
        let nodes = asList (v ? nodes)
        objOpt [
            "id", prop "id" v
            "impact", prop "impact" v
            "tags", Some(JArr(asList (v ? tags) |> List.filter (fun t -> Regex.IsMatch(jsString t, "^wcag[0-9]"))))
            "help", prop "help" v
            "nodes", Some(jint nodes.Length)
            "targets", Some(JArr(nodes |> List.truncate 6 |> List.map (fun n -> JStr(jsJoin " " (asList (n ? target))))))
            "summary", nodes |> List.tryHead |> Option.bind (prop "failureSummary") |> Option.bind (function JStr s -> Some(JStr(slice0 200 s)) | _ -> None)
        ])

let private small (t: Json) = asNum (t ? w) < 24.0 || asNum (t ? h) < 24.0

let targetsSummary (vp: Viewport) (targets: Json list) : Json =
    objOpt [
        "under24", Some(JArr(targets |> List.filter small))
        "under24NonInline", Some(jint (targets |> List.filter (fun t -> small t && not (truthy (t ? ``inline``))) |> List.length))
        "under44", (if vp.Width <= 320 then Some(JArr targets) else None)
        "under44Count", Some(jint targets.Length)
    ]

let textSpacingSummary (clippedBefore: Json list) (clippedAfter: Json list) (overflowSpacing: Json) (numbersSpacing: Json) : Json =
    let keyOf (c: Json) = jsString (c ? element) + jsString (c ? text)
    let before = clippedBefore |> List.map keyOf |> Set.ofList
    JObj [
        "newlyClipped", JArr(clippedAfter |> List.filter (fun c -> not (before.Contains(keyOf c))))
        "clippedBefore", JArr clippedBefore
        "pageOverflow", overflowSpacing ? page
        "moneyWrapsUnderSpacing", jint (asList (numbersSpacing ? wrapped)).Length
    ]

let dialogResult (id: string) (axe: Json list) (initial: Json) (stops: Json list) (closedByEsc: bool) (targets: Json list) : Json =
    JObj [
        "dialog", str id
        "axe", JArr axe
        "initialFocus", initial
        "focusContained", JBool(stops |> List.forall (fun s -> truthy (s ? inDialog)))
        "tabStops", JArr(stops |> List.map (fun s -> s ? name) |> List.distinct)
        "closedByEsc", JBool closedByEsc
        "smallTargets", JArr(targets |> List.filter (fun t -> t ? state = JStr id && small t))
    ]

let private len (j: Json) = (asList j).Length

/// The one-line summary per viewport printed to stdout.
let briefLine (r: Json) =
    let axe = asList (r ? axe) |> List.map (fun a -> jsString (a ? id) + "(" + jsString (a ? nodes) + ")") |> String.concat "+"
    let f = r ? focus
    $"""wb:{len (r ? wordBreaks)} obs:{len (f ? obscured)}/{len (f ? partlyObscured)} wrap:{len (r ? numbers ? wrapped)} ovf:{(if truthy (r ? overflow ? page ? overflows) then "Y" else "n")}/{len (r ? overflow ? spill)} axe:{(if axe = "" then "0" else axe)} t24:{jsString (r ? targets ? under24NonInline)} fNV:{len (f ? notVisible)} fH:{len (f ? hidden)} ts:{len (r ? textSpacing ? newlyClipped)}"""

let byPage (files: string list) (flat: (string * Json) list) : Json =
    obj (files |> List.map (fun f -> f, obj (flat |> List.filter (fun (file, _) -> file = f) |> List.map (fun (_, r) -> jsString (r ? key), r))))

let brief (files: string list) (pages: Json) : Json =
    JArr(
        files
        |> List.map (fun f ->
            let entries = match pages ? (f: string) with JObj kvs -> kvs | _ -> []
            obj (("f", str f) :: (entries |> List.map (fun (k, r) -> k, str (briefLine r)))))
    )

let report (generated: string) (pages: Json) : Json =
    JObj [
        "generated", str generated
        "viewports", JArr(viewports |> List.map viewportJson)
        "axeVersion", str "axe-core (scratchpad)"
        "pages", pages
    ]

// ---------- I/O ----------

type Settings = { Screens: string; Out: string; Axe: string }

let private runAxe (page: Cdp.Page) (axeSrc: string) (tags: string list) =
    task {
        do! Cdp.evaluateScript page axeSrc
        let! r = Cdp.call page axeRunScript [ strs tags ]
        return asList (r ? violations)
    }

let private tabWalk (page: Cdp.Page) =
    task {
        let! baseline = Cdp.call page tagFocusables [ strs focusProps ]
        do! Cdp.evaluate page "(() => { document.activeElement?.blur(); window.scrollTo(0, 0); })()" :> Task
        let seq = ResizeArray<Json>()
        let mutable first = JNull
        let mutable wrapped = false
        let mutable i = 0
        while i < 200 do
            do! Cdp.press page "Tab"
            let! a = Cdp.call page readActive [ strs focusProps ]
            if truthy (a ? body) then
                wrapped <- true
                i <- 200
            elif truthy first && a ? idx = first then
                wrapped <- true
                i <- 200
            else
                if first = JNull then first <- a ? idx
                seq.Add a
                i <- i + 1
        return analyseFocus (List.ofSeq seq) wrapped (asList baseline)
    }

let private dialogChecks (page: Cdp.Page) (axeSrc: string) =
    task {
        let! ids = Cdp.evaluate page """[...document.querySelectorAll("dialog[id]")].map((d) => d.id)"""
        let out = ResizeArray<Json>()
        for id in asList ids |> List.map asStr do
            do! Cdp.call page "(d) => { document.getElementById(d).showModal(); }" [ str id ] :> Task
            do! Cdp.call page probeCommon [] :> Task
            let! res = runAxe page axeSrc axeTags
            let! initial =
                Cdp.evaluate page """(() => ({ tag: document.activeElement?.tagName, name: (document.activeElement?.innerText || document.activeElement?.getAttribute("aria-label") || "").trim().slice(0, 50) }))()"""
            let stops = ResizeArray<Json>()
            for _ in 1..12 do
                do! Cdp.press page "Tab"
                let! s =
                    Cdp.call page """(d) => ({ inDialog: !!document.activeElement?.closest("#" + d), name: (document.activeElement?.innerText || document.activeElement?.getAttribute("aria-label") || document.activeElement?.tagName || "").trim().slice(0, 40) })""" [ str id ]
                stops.Add s
            let! targets = Cdp.call page probeTargets []
            do! Cdp.press page "Escape"
            let! closed = Cdp.call page "(d) => !document.getElementById(d).open" [ str id ]
            do! Cdp.call page "(d) => { document.getElementById(d).open && document.getElementById(d).close(); }" [ str id ] :> Task
            out.Add(dialogResult id (summariseAxe res) initial (List.ofSeq stops) (truthy closed) (asList targets))
        return List.ofSeq out
    }

let private auditOne (browser: Cdp.Browser) (s: Settings) (file: string) (vp: Viewport) (axeSrc: string) =
    task {
        let! page = Cdp.newPage browser { Width = vp.Width; Height = vp.Height; DeviceScaleFactor = 1.0; ReducedMotion = Some "reduce" }
        do! Cdp.goto page ("file://" + Path.GetFullPath(Path.Combine(s.Screens, file)))
        do! Cdp.call page probeCommon [] :> Task
        let stem = Regex.Replace(file, @"\.html$", "")
        let shot = Path.Combine(Path.GetFullPath s.Out, "shots", $"{stem}-{vp.Key}.png")
        let! png = Cdp.screenshot page true
        Cdp.save shot png
        let wide = vp.Key = "1280" || vp.Key = "320"
        let! numbers = Cdp.call page probeNumbers []
        let! overflow = Cdp.call page probeOverflow []
        let! wordBreaks = Cdp.call page probeWordBreaks []
        let! sticky = Cdp.call page probeSticky []
        let! typography = if wide then task { let! t = Cdp.call page probeTypography [] in return Some t } else Task.FromResult None
        let! semantics = if wide then task { let! t = Cdp.call page probeSemantics [] in return Some t } else Task.FromResult None
        let! targets = Cdp.call page probeTargets []
        let! axeCore = runAxe page axeSrc axeTags
        let! axeBP = if wide then task { let! t = runAxe page axeSrc [ "best-practice" ] in return Some t } else Task.FromResult None
        let! clippedBefore = Cdp.call page probeClipped []
        let! focus = tabWalk page
        let! dialogs = if wide then task { let! d = dialogChecks page axeSrc in return Some d } else Task.FromResult None
        do! Cdp.addStyleTag page textSpacingCss
        do! Cdp.call page probeCommon [] :> Task
        let! clippedAfter = Cdp.call page probeClipped []
        let! overflowSpacing = Cdp.call page probeOverflow []
        let! numbersSpacing = Cdp.call page probeNumbers []
        do! Cdp.closePage page
        return
            objOpt [
                "viewport", Some(str vp.Label)
                "key", Some(str vp.Key)
                "screenshot", Some(str shot)
                "numbers", Some numbers
                "wordBreaks", Some wordBreaks
                "sticky", Some sticky
                "typography", typography
                "overflow", Some overflow
                "semantics", semantics
                "targets", Some(targetsSummary vp (asList targets))
                "axe", Some(JArr(summariseAxe axeCore))
                "axeBestPractice", axeBP |> Option.map (summariseAxe >> JArr)
                "focus", Some focus
                "dialogs", dialogs |> Option.map JArr
                "textSpacing", Some(textSpacingSummary (asList clippedBefore) (asList clippedAfter) overflowSpacing numbersSpacing)
            ]
    }

/// Runs the jobs on `n` workers in job order; `onStart` runs (serialised) as each job is taken.
let private pool (n: int) (onStart: int -> unit) (jobs: (unit -> Task<'T>) array) : 'T array =
    let results = Array.zeroCreate jobs.Length
    let next = ref 0
    let gate = Object()
    let take () = lock gate (fun () -> let k = next.Value in (if k < jobs.Length then next.Value <- k + 1; onStart k); k)
    let worker () =
        task {
            let mutable k = take ()
            while k < jobs.Length do
                let! r = jobs.[k] ()
                results.[k] <- r
                k <- take ()
        }
    Task.WhenAll(Array.init n (fun _ -> worker () :> Task)).Wait()
    results

let run (s: Settings) : int =
    try
        let axeSrc = File.ReadAllText s.Axe
        let files =
            Directory.GetFiles s.Screens |> Array.map Path.GetFileName |> Array.filter isPageFile
            |> Array.sortWith (fun a b -> String.CompareOrdinal(a, b)) |> List.ofArray
        let browser = (Cdp.launch ()).Result
        let specs = files |> List.collect (fun f -> viewports |> List.map (fun vp -> f, vp)) |> Array.ofList
        let jobs = specs |> Array.map (fun (f, vp) -> fun () -> auditOne browser s f vp axeSrc)
        let flat = pool 4 (fun k -> let f, vp = specs.[k] in eprintf "%s %s\n" f vp.Key) jobs
        (Cdp.close browser).Wait()
        let pages = byPage files (Array.zip (Array.map fst specs) flat |> List.ofArray)
        Directory.CreateDirectory s.Out |> ignore
        File.WriteAllText(Path.Combine(s.Out, "checks-v1.json"), stringify 1 (report (isoString DateTime.UtcNow) pages))
        Console.WriteLine(stringify 1 (brief files pages))
        0
    with e ->
        eprintfn "%O" e
        1
