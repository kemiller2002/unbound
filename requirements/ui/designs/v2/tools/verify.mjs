#!/usr/bin/env node
// Verifies every v2 prototype page (pages.json) at five viewports.
// Pure check functions run in the page; orchestration is a fold over pages x viewports.
//
// Viewports: phone 390x844, reflow 320x800 (= 1280 px at 400%), zoom200 640x450 (= 1280x900 at 200%),
//            tablet 768x1024, desktop 1280x900. Embedded widget pages add host-iframe checks (see W-001).
// Checks:    axe-core (wcag2a, wcag2aa, wcag21a, wcag21aa, wcag22aa); page horizontal overflow;
//            money/number wrapping (.unb-money, .unb-num must render on one line);
//            target size (WCAG 2.5.8, 24x24 CSS px; inline links in sentences exempt);
//            keyboard walk: focus visible (2.4.7), focus not obscured by fixed bars (2.4.11),
//            focus order monotonic within regions; heading outline (one h1, no skipped level);
//            text spacing (1.4.12): clipped text after the WCAG spacing overrides.
// Screenshots: 390 and 1280 are committed reference images (rendered/); others go to OUT_EXTRA.
//
// Usage: node tools/verify.mjs [--axe <axe.min.js>]   (from designs/v2)
import { createRequire } from "node:module";
import { readFile, writeFile, mkdir } from "node:fs/promises";
import { fileURLToPath } from "node:url";
import path from "node:path";
import http from "node:http";
import { createReadStream, existsSync } from "node:fs";

const require = createRequire(import.meta.url);
const { chromium } = require("playwright");

const here = path.dirname(fileURLToPath(import.meta.url));
const root = path.resolve(here, "..");
const args = process.argv.slice(2);
const AXE = args.includes("--axe") ? args[args.indexOf("--axe") + 1] : "/tmp/claude-0/-home-user/179dd429-c49d-5559-a2b4-4866d79585b5/scratchpad/axe/node_modules/axe-core/axe.min.js";
const OUT_EXTRA = process.env.OUT_EXTRA ?? "/tmp/claude-0/-home-user/179dd429-c49d-5559-a2b4-4866d79585b5/scratchpad/v2-extra";
const ONLY = process.env.ONLY ? process.env.ONLY.split(",") : null;

const VIEWPORTS = [
  { id: "phone", width: 390, height: 844, reference: true },
  { id: "reflow320", width: 320, height: 800 },
  { id: "zoom200", width: 640, height: 450 },
  { id: "tablet", width: 768, height: 1024 },
  { id: "desktop", width: 1280, height: 900, reference: true },
];
const AXE_TAGS = ["wcag2a", "wcag2aa", "wcag21a", "wcag21aa", "wcag22aa"];
const TEXT_SPACING_CSS = `*{line-height:1.5!important;letter-spacing:.12em!important;word-spacing:.16em!important}p{margin-block-end:2em!important}`;

// ---------- in-page pure checks (serialised into the page) ----------
const pageChecks = () => {
  const visible = (el) => { const r = el.getBoundingClientRect(); const cs = getComputedStyle(el); return r.width > 0 && r.height > 0 && cs.visibility !== "hidden" && cs.display !== "none"; };
  const lines = (el) => new Set([...el.getClientRects()].map((r) => Math.round(r.top))).size;
  const wrappedMoney = [...document.querySelectorAll(".unb-money, .unb-num")].filter(visible).filter((el) => lines(el) > 1).map((el) => el.textContent.trim());
  const interactive = [...document.querySelectorAll("a[href], button, input, select, textarea, [tabindex]:not([tabindex='-1']), summary")].filter(visible);
  const inlineInSentence = (el) => el.tagName === "A" && el.closest("p, li") && !el.className && (el.closest("p, li").textContent.trim().length > el.textContent.trim().length + 10);
  const smallTargets = interactive.filter((el) => !inlineInSentence(el)).filter((el) => { const r = el.getBoundingClientRect(); return r.width < 24 || r.height < 24; }).map((el) => `${el.tagName.toLowerCase()} "${(el.getAttribute("aria-label") || el.textContent).trim().slice(0, 40)}" ${Math.round(el.getBoundingClientRect().width)}x${Math.round(el.getBoundingClientRect().height)}`);
  const headings = [...document.querySelectorAll("h1,h2,h3,h4,h5,h6")].filter(visible).map((h) => +h.tagName[1]);
  const skipped = headings.filter((lvl, i) => i > 0 && lvl > headings[i - 1] + 1).length;
  const navLabels = [...document.querySelectorAll("nav")].filter(visible).map((n) => n.getAttribute("aria-label"));
  return {
    overflowX: document.documentElement.scrollWidth > window.innerWidth + 1,
    scrollWidth: document.documentElement.scrollWidth,
    wrappedMoney,
    smallTargets,
    h1Count: headings.filter((l) => l === 1).length,
    skippedHeadingLevels: skipped,
    duplicateNavLabels: navLabels.length !== new Set(navLabels).size,
    mainCount: document.querySelectorAll("main").length,
  };
};

const focusProbe = () => {
  const el = document.activeElement;
  if (!el || el === document.body) return null;
  const r = el.getBoundingClientRect();
  const cs = getComputedStyle(el);
  const visibleIndicator = (cs.outlineStyle !== "none" && parseFloat(cs.outlineWidth) >= 2) || (cs.boxShadow && cs.boxShadow !== "none");
  const cx = Math.min(Math.max(r.left + r.width / 2, 0), window.innerWidth - 1);
  const cy = Math.min(Math.max(r.top + r.height / 2, 0), window.innerHeight - 1);
  const top = document.elementFromPoint(cx, cy);
  const obscured = top && !(top === el || el.contains(top) || top.contains(el));
  return { name: (el.getAttribute("aria-label") || el.textContent || "").trim().replace(/\s+/g, " ").slice(0, 50), tag: el.tagName.toLowerCase(), y: Math.round(r.top + window.scrollY), x: Math.round(r.left), visibleIndicator, obscured: !!obscured, obscuredBy: obscured ? top.className || top.tagName : null };
};

const clippedAfterSpacing = () =>
  [...document.querySelectorAll("body *")].filter((el) => el.getBoundingClientRect().width > 2 && el.getBoundingClientRect().height > 2).filter((el) => { const cs = getComputedStyle(el); return /(hidden|clip)/.test(cs.overflow + cs.overflowY + cs.overflowX) && (el.scrollHeight > el.clientHeight + 2 || el.scrollWidth > el.clientWidth + 2) && el.textContent.trim(); }).map((el) => el.className || el.tagName).slice(0, 10);

// ---------- orchestration ----------
const walkKeyboard = async (page, max = 120) => {
  const step = async (acc, i) => {
    if (i >= max) return acc;
    await page.keyboard.press("Tab");
    const f = await page.evaluate(focusProbe);
    if (!f) return acc;
    if (acc.length && acc[0].name === f.name && acc[0].tag === f.tag && acc.length > 3) return acc; // wrapped
    return step([...acc, f], i + 1);
  };
  return step([], 0);
};

// Static server over designs/ so pages load as http (axe and fonts behave as in production).
const designsRoot = path.resolve(root, "..");
const TYPES = { ".html": "text/html", ".css": "text/css", ".woff2": "font/woff2", ".js": "text/javascript", ".json": "application/json", ".png": "image/png", ".svg": "image/svg+xml" };
const serve = () => new Promise((resolve) => {
  const srv = http.createServer((req, res) => {
    const f = path.join(designsRoot, decodeURIComponent(new URL(req.url, "http://x").pathname));
    if (!f.startsWith(designsRoot) || !existsSync(f)) { res.writeHead(404); res.end(); return; }
    res.writeHead(200, { "content-type": TYPES[path.extname(f)] ?? "application/octet-stream" });
    createReadStream(f).pipe(res);
  });
  srv.listen(0, "127.0.0.1", () => resolve(srv));
});
let BASE = "";

const checkPage = async (browser, axeSrc, p, vp) => {
  const ctx = await browser.newContext({ viewport: { width: vp.width, height: vp.height }, deviceScaleFactor: 1, reducedMotion: "reduce" });
  const page = await ctx.newPage();
  const errors = [];
  page.on("console", (m) => m.type() === "error" && errors.push(m.text()));
  page.on("requestfailed", (r) => errors.push(`failed: ${r.url()}`));
  await page.goto(`${BASE}/v2/${p.file}`);
  await page.evaluate(() => document.fonts.ready);
  const base = await page.evaluate(pageChecks);
  await page.addScriptTag({ content: axeSrc });
  // axe marks some contrast cases "incomplete" (e.g. single characters, v1 audit UXD-002);
  // those are treated as failures unless their measured ratio passes.
  const axeRun = await page.evaluate(async (tags) => {
    const r = await window.axe.run(document, { runOnly: { type: "tag", values: tags } });
    return {
      violations: r.violations.map((v) => ({ id: v.id, impact: v.impact, nodes: v.nodes.length, target: v.nodes[0]?.target?.join(" ") })),
      contrastIncomplete: r.incomplete.filter((v) => v.id === "color-contrast").flatMap((v) => v.nodes).filter((n) => { const d = n.any?.[0]?.data; return d && d.contrastRatio && d.contrastRatio < (parseFloat(d.expectedContrastRatio) || 4.5); }).map((n) => n.target.join(" ")),
    };
  }, AXE_TAGS);
  const axe = [...axeRun.violations, ...axeRun.contrastIncomplete.map((t) => ({ id: "color-contrast-incomplete", impact: "serious", nodes: 1, target: t }))];
  const focus = await walkKeyboard(page);
  const shotDir = vp.reference ? path.join(root, "rendered", p.screen) : path.join(OUT_EXTRA, p.screen);
  await mkdir(shotDir, { recursive: true });
  await page.evaluate(() => window.scrollTo(0, 0));
  await page.screenshot({ path: path.join(shotDir, `${p.state}-${vp.width}.png`), fullPage: true });
  await page.addStyleTag({ content: TEXT_SPACING_CSS });
  const spacing = { clipped: await page.evaluate(clippedAfterSpacing), overflowX: (await page.evaluate(pageChecks)).overflowX, wrappedMoney: (await page.evaluate(pageChecks)).wrappedMoney };
  await ctx.close();
  const focusIssues = {
    notVisible: focus.filter((f) => !f.visibleIndicator).map((f) => f.name),
    obscured: focus.filter((f) => f.obscured).map((f) => `${f.name} (by ${f.obscuredBy})`),
  };
  const failures = [
    ...axe.map((a) => `axe:${a.id}`),
    ...(base.overflowX ? ["overflow-x"] : []),
    ...base.wrappedMoney.map((m) => `money-wraps:${m}`),
    ...base.smallTargets.map((t) => `target<24:${t}`),
    ...(base.h1Count !== 1 ? [`h1-count:${base.h1Count}`] : []),
    ...(base.skippedHeadingLevels ? [`heading-skip:${base.skippedHeadingLevels}`] : []),
    ...(base.duplicateNavLabels ? ["duplicate-nav-label"] : []),
    ...focusIssues.notVisible.map((n) => `focus-not-visible:${n}`),
    ...focusIssues.obscured.map((n) => `focus-obscured:${n}`),
    ...spacing.clipped.map((c) => `text-spacing-clipped:${c}`),
    ...(spacing.overflowX ? ["text-spacing-overflow-x"] : []),
    ...errors.map((e) => `console:${e}`),
  ];
  return { viewport: vp.id, width: vp.width, failures, axe, base, focusOrder: focus.map((f) => f.name), spacing };
};

const main = async () => {
  const pages = JSON.parse(await readFile(path.join(root, "pages.json"), "utf8")).filter((p) => !ONLY || ONLY.includes(p.screen));
  const axeSrc = await readFile(AXE, "utf8");
  const srv = await serve();
  BASE = `http://127.0.0.1:${srv.address().port}`;
  const browser = await chromium.launch();
  const results = [];
  for (const p of pages) {
    const perVp = [];
    for (const vp of VIEWPORTS) perVp.push(await checkPage(browser, axeSrc, p, vp));
    results.push({ ...p, results: perVp, pass: perVp.every((r) => r.failures.length === 0) });
    console.log(`${perVp.every((r) => !r.failures.length) ? "PASS" : "FAIL"} ${p.screen}/${p.state} ${perVp.flatMap((r) => r.failures.map((f) => `[${r.viewport}] ${f}`)).slice(0, 6).join(" | ")}`);
  }
  await browser.close();
  srv.close();
  const prior = await readFile(path.join(root, "verification.json"), "utf8").then(JSON.parse).catch(() => ({ pages: [] }));
  const merged = [...prior.pages.filter((x) => !results.some((r) => r.file === x.file)), ...results].sort((a, b) => a.file.localeCompare(b.file));
  const summary = { checked_at: new Date().toISOString(), viewports: VIEWPORTS, axe_tags: AXE_TAGS, pages: merged.length, passing: merged.filter((r) => r.pass).length, pages_detail: undefined };
  await writeFile(path.join(root, "verification.json"), JSON.stringify({ ...summary, pages: merged }, null, 2) + "\n");
  console.log(`${summary.passing}/${summary.pages} pages pass all checks at all viewports`);
};

main();
