#!/usr/bin/env node
// Verifies the Forma screen prototypes in ../screens.
//
// For every page, at 320 and 1280 CSS px (deviceScaleFactor 1):
//   - full-page screenshot to ../rendered/<page>-<width>.png
//   - page-level horizontal scroll check: documentElement.scrollWidth <= viewport width
//   - axe-core (wcag2a, wcag2aa, wcag21a, wcag21aa, wcag22aa)
//   - every <dialog> on the page is opened as a modal by this harness (the pages themselves carry
//     no script), screenshotted to ../rendered/<page>-<dialog-id>-<width>.png and axe-checked while open
// plus static rule checks: no <style>, no style= attributes, no <script>.
// Results are merged with ../screens/page-manifest.json into ../screens/verification.json.
//
// Usage: node tools/verify-screens.mjs [--axe <path to axe.min.js>] [page.html ...]
// Requires the preinstalled Playwright (require('playwright')) and axe-core@4.10.2.
import { createRequire } from "node:module";
import { readFile, writeFile, mkdir, readdir } from "node:fs/promises";
import { fileURLToPath, pathToFileURL } from "node:url";
import path from "node:path";

const require = createRequire(import.meta.url);
const { chromium } = require("playwright");

const here = path.dirname(fileURLToPath(import.meta.url));
const designs = path.resolve(here, "..");
const screensDir = path.join(designs, "screens");
const renderedDir = path.join(designs, "rendered");
const catalogCsv = path.resolve(designs, "../../reconciliation/2026-11-23/data/forma-catalog.csv");
const DEFAULT_AXE = "/tmp/claude-0/-home-user/179dd429-c49d-5559-a2b4-4866d79585b5/scratchpad/axe/node_modules/axe-core/axe.min.js";

const WIDTHS = [320, 1280];
const AXE_TAGS = ["wcag2a", "wcag2aa", "wcag21a", "wcag21aa", "wcag22aa"];

// Class roots that do not share their catalog id's spelling.
const CLASS_ALIASES = {
  "forma-field": "text-field",
  "forma-choice": "choice-group",
  "forma-turn": "conversation",
  "forma-rule-group": "rule-builder",
  "forma-rule-clause": "rule-builder",
  "forma-select-field": "select",
  "forma-critical-value": "identifier",
  "forma-site": "marketing-shell",
  "forma-site__main": "marketing-shell",
  "forma-overflow-commands": "overflow-command-disclosure",
  "forma-obligation": "obligation-panel",
  "forma-obligation-list": "obligation-panel",
  "forma-fault-reference": "fault-reference",
  "forma-diff-row": "diff-viewer",
  "forma-binary-option": "binary-choice",
  "forma-status-lozenge-set": "status-lozenge",
  "forma-eyebrow": "text-roles",
  "forma-lead": "text-roles",
  "forma-component-kicker": "text-roles",
  "forma-actions": "button",
  "forma-button": "button",
};

// ---------- pure helpers ----------

const parseCatalogIds = (csv) =>
  csv
    .split("\n")
    .slice(1)
    .map((line) => (line.match(/^"([^"]+)"/) || [])[1])
    .filter(Boolean);

const classRoot = (cls) => cls.replace(/__.*$/, "").replace(/--.*$/, "");

const componentsUsed = (html, catalogIds) => {
  const known = new Set(catalogIds);
  const fromTags = [...html.matchAll(/<forma-([a-z0-9-]+) class="forma-component-tag"/g)].map((m) => m[1]);
  const classes = [...html.matchAll(/class="([^"]+)"/g)].flatMap((m) => m[1].split(/\s+/));
  const fromClasses = classes
    .filter((c) => c.startsWith("forma-") && c !== "forma-component-tag")
    .flatMap((c) => [CLASS_ALIASES[c], CLASS_ALIASES[classRoot(c)], classRoot(c).replace(/^forma-/, "")])
    .filter((id) => id && known.has(id));
  const buttonsPresent = /<button\b/.test(html) ? ["button"] : [];
  return [...new Set([...fromTags.filter((id) => known.has(id)), ...fromClasses, ...buttonsPresent])].sort();
};

const staticRuleViolations = (html) =>
  [
    /<style[\s>]/i.test(html) && "page contains a <style> element",
    /\sstyle\s*=/i.test(html) && "page contains a style attribute",
    /<script[\s>]/i.test(html) && "page contains a <script> element",
  ].filter(Boolean);

const summarizeViolations = (violations) =>
  violations.map((v) => ({
    id: v.id,
    impact: v.impact,
    help: v.help,
    nodes: v.nodes.length,
    targets: v.nodes.slice(0, 5).map((n) => n.target.join(" ")),
  }));

const pageName = (file) => file.replace(/\.html$/, "");

// ---------- effectful steps ----------

const runAxe = async (page, axeSource) => {
  await page.addScriptTag({ content: axeSource });
  const result = await page.evaluate(
    (tags) => window.axe.run(document, { runOnly: { type: "tag", values: tags }, resultTypes: ["violations"] }),
    AXE_TAGS
  );
  return summarizeViolations(result.violations);
};

const checkDialogs = async (page, file, width, axeSource) => {
  const ids = await page.$$eval("dialog[id]", (ds) => ds.map((d) => d.id));
  const results = [];
  for (const id of ids) {
    await page.evaluate((dialogId) => document.getElementById(dialogId).showModal(), id);
    await page.waitForTimeout(400);
    const shot = path.join(renderedDir, `${pageName(file)}-${id}-${width}.png`);
    await page.screenshot({ path: shot });
    const violations = await runAxe(page, axeSource);
    await page.evaluate((dialogId) => document.getElementById(dialogId).close(), id);
    results.push({ dialog: id, screenshot: path.relative(designs, shot), axe_violations: violations });
  }
  return results;
};

const checkWidth = async (browser, file, width, axeSource) => {
  const context = await browser.newContext({ viewport: { width, height: 800 }, deviceScaleFactor: 1, reducedMotion: "reduce" });
  const page = await context.newPage();
  const consoleErrors = [];
  page.on("pageerror", (e) => consoleErrors.push(String(e)));
  await page.goto(pathToFileURL(path.join(screensDir, file)).href, { waitUntil: "load" });
  const shot = path.join(renderedDir, `${pageName(file)}-${width}.png`);
  await page.screenshot({ path: shot, fullPage: true });
  const { scrollWidth, clientWidth } = await page.evaluate(() => ({
    scrollWidth: document.documentElement.scrollWidth,
    clientWidth: document.documentElement.clientWidth,
  }));
  const axe_violations = await runAxe(page, axeSource);
  const dialogs = await checkDialogs(page, file, width, axeSource);
  await context.close();
  return {
    h_scroll: scrollWidth > width,
    scroll_width: scrollWidth,
    client_width: clientWidth,
    screenshot: path.relative(designs, shot),
    axe_violations,
    dialogs,
    page_errors: consoleErrors,
  };
};

const verifyPage = async (browser, file, axeSource, catalogIds, manifestEntry) => {
  const html = await readFile(path.join(screensDir, file), "utf8");
  const widths = {};
  for (const w of WIDTHS) widths[w] = await checkWidth(browser, file, w, axeSource);
  return {
    page: file,
    requirement_ids: manifestEntry?.requirement_ids ?? [],
    forma_components_used: componentsUsed(html, catalogIds),
    static_rule_violations: staticRuleViolations(html),
    states: manifestEntry?.states ?? [],
    widths,
    requirement_coverage: manifestEntry?.requirement_coverage ?? [],
    gaps: manifestEntry?.gaps ?? [],
  };
};

const totals = (results) => {
  const flat = results.flatMap((r) => WIDTHS.map((w) => r.widths[w]));
  const dialogViolations = flat.flatMap((w) => w.dialogs.flatMap((d) => d.axe_violations));
  return {
    pages: results.length,
    page_width_checks: flat.length,
    h_scroll_failures: flat.filter((w) => w.h_scroll).length,
    axe_violation_rules_page: flat.reduce((n, w) => n + w.axe_violations.length, 0),
    axe_violation_nodes_page: flat.reduce((n, w) => n + w.axe_violations.reduce((m, v) => m + v.nodes, 0), 0),
    dialog_checks: flat.reduce((n, w) => n + w.dialogs.length, 0),
    axe_violation_rules_dialogs: dialogViolations.length,
    static_rule_violations: results.reduce((n, r) => n + r.static_rule_violations.length, 0),
  };
};

const main = async () => {
  const args = process.argv.slice(2);
  const axeIdx = args.indexOf("--axe");
  const axePath = axeIdx >= 0 ? args[axeIdx + 1] : DEFAULT_AXE;
  const only = args.filter((a, i) => a.endsWith(".html") && (axeIdx < 0 || i !== axeIdx + 1));
  const [axeSource, csv, manifestRaw, files] = await Promise.all([
    readFile(axePath, "utf8"),
    readFile(catalogCsv, "utf8"),
    readFile(path.join(screensDir, "page-manifest.json"), "utf8"),
    readdir(screensDir),
  ]);
  const manifest = JSON.parse(manifestRaw);
  const catalogIds = parseCatalogIds(csv);
  const pages = files.filter((f) => /^(S|W)-\d{3}.*\.html$/.test(f) && (!only.length || only.includes(f))).sort();
  await mkdir(renderedDir, { recursive: true });
  const browser = await chromium.launch();
  const results = [];
  for (const f of pages) {
    results.push(await verifyPage(browser, f, axeSource, catalogIds, manifest.find((m) => m.page === f)));
    console.log(`verified ${f}`);
  }
  await browser.close();
  const report = {
    generated_by: "tools/verify-screens.mjs",
    forma: { version: "1.0.2", commit: "d7c499e", stylesheet: "vendor/forma-1.0.2/all.css" },
    axe: { version: "4.10.2", tags: AXE_TAGS },
    widths: WIDTHS,
    totals: totals(results),
    pages: results,
  };
  const out = only.length ? path.join(screensDir, "verification.partial.json") : path.join(screensDir, "verification.json");
  await writeFile(out, JSON.stringify(report, null, 2) + "\n");
  console.log(JSON.stringify(report.totals));
};

main().catch((e) => {
  console.error(e);
  process.exit(1);
});
