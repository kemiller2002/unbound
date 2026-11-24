// Renders the Unbound print-document prototypes with headless Chromium (Playwright) and verifies them.
// - PDF: page.pdf({format:'Letter', printBackground:true, preferCSSPageSize:true}) -> ../rendered/<doc>.pdf
// - PNG: page-1 viewport screenshot at 1280 px width, deviceScaleFactor 1 -> ../rendered/<doc>.png
// - Checks via pdfinfo / pdftotext, determinism (two independent browser launches), axe-core on the HTML,
//   tagged-PDF status, plus two capability probes that do NOT change the documents:
//     * tagged probe: the same render with Chromium's `tagged: true` option;
//     * P1 margin-box probe: an injected @page margin-box stylesheet (what Folio's own Signal fixture
//       hand-authors) to show whether the engine can meet "Page n of N" / running reference when Folio cannot.
// Writes ../documents/verification.json.
// Usage: node verify-documents.mjs [--axe <path to axe.min.js>]
import { createRequire } from "node:module";
import { createServer } from "node:http";
import { createHash } from "node:crypto";
import { readFileSync, writeFileSync, mkdirSync, existsSync } from "node:fs";
import { execFileSync } from "node:child_process";
import { dirname, join, extname, normalize } from "node:path";
import { fileURLToPath } from "node:url";

const require = createRequire(import.meta.url);
const { chromium } = require("playwright");

const here = dirname(fileURLToPath(import.meta.url));
const root = join(here, "..");
const docsDir = join(root, "documents");
const renderDir = join(root, "rendered");
const argValue = flag => { const i = process.argv.indexOf(flag); return i > 0 ? process.argv[i + 1] : undefined; };
const axePath = argValue("--axe")
  ?? "/tmp/claude-0/-home-user/179dd429-c49d-5559-a2b4-4866d79585b5/scratchpad/axe-folio/node_modules/axe-core/axe.min.js";
const workDir = argValue("--work") ?? "/tmp/claude-0/-home-user/179dd429-c49d-5559-a2b4-4866d79585b5/scratchpad/verify-work";
mkdirSync(renderDir, { recursive: true });
mkdirSync(workDir, { recursive: true });

const PDF_OPTIONS = { format: "Letter", printBackground: true, preferCSSPageSize: true };
const AXE_TAGS = ["wcag2a", "wcag2aa", "wcag21a", "wcag21aa", "wcag22aa"];
const FOLIO_ELEMENTS = new Set(readFileSync(join(root, "vendor/folio-0.4.2/src/components/register.js"), "utf8")
  .match(/"ef-print-[a-z-]+"/g).map(s => s.slice(1, -1)));

// ---------- document manifest ----------
const SPONSOR_DISCLOSURE = "Harbor Giving is a donor advised fund program of Example Charitable Foundation (EIN 00-0000000), the legal sponsor. Example Charitable Foundation holds exclusive legal control over all contributed assets.";
const EXCLUSIVE_CONTROL = "Example Charitable Foundation, as sponsoring organization of this donor advised fund, has exclusive legal control over the contributed assets.";
const NO_GOODS = "No goods or services were provided in exchange for this contribution.";

const DOCS = [
  { file: "D-001-acknowledgment-cash", ids: ["UNB-UI-D-001", "UNB-UI-G-007", "UNB-UI-G-014", "UNB-UI-G-016", "UNB-UI-G-017", "UNB-UI-G-018"], ref: "2026-880114", donorFacing: true,
    gaps: ["FOLIO-GAP-01", "FOLIO-GAP-02", "FOLIO-GAP-04", "FOLIO-GAP-05", "FOLIO-GAP-06", "FOLIO-GAP-07", "FOLIO-GAP-11"] },
  { file: "D-001-acknowledgment-securities", ids: ["UNB-UI-D-001", "UNB-UI-G-007", "UNB-UI-G-014", "UNB-UI-G-016", "UNB-UI-G-018"], ref: "2026-880121", donorFacing: true,
    gaps: ["FOLIO-GAP-01", "FOLIO-GAP-02", "FOLIO-GAP-04", "FOLIO-GAP-05", "FOLIO-GAP-06", "FOLIO-GAP-07", "FOLIO-GAP-11"] },
  { file: "D-002-fund-statement", ids: ["UNB-UI-D-002", "UNB-UI-G-007", "UNB-UI-G-014", "UNB-UI-G-016", "UNB-UI-G-017", "UNB-UI-G-018"], ref: "2026-310482", donorFacing: true,
    gaps: ["FOLIO-GAP-02", "FOLIO-GAP-03", "FOLIO-GAP-04", "FOLIO-GAP-05", "FOLIO-GAP-06", "FOLIO-GAP-08", "FOLIO-GAP-09", "FOLIO-GAP-10", "FOLIO-GAP-11"] },
  { file: "D-003-grant-remittance", ids: ["UNB-UI-D-003", "UNB-UI-G-014", "UNB-UI-G-016", "UNB-UI-G-017", "UNB-UI-G-018"], ref: "2026-550207", donorFacing: true,
    gaps: ["FOLIO-GAP-01", "FOLIO-GAP-02", "FOLIO-GAP-04", "FOLIO-GAP-05", "FOLIO-GAP-06", "FOLIO-GAP-07", "FOLIO-GAP-11"] },
  { file: "D-004-daf-reporting-package", ids: ["UNB-UI-D-004", "UNB-UI-G-016", "UNB-UI-G-017", "UNB-UI-G-018"], ref: "2026-770025", donorFacing: true,
    gaps: ["FOLIO-GAP-02", "FOLIO-GAP-03", "FOLIO-GAP-04", "FOLIO-GAP-05", "FOLIO-GAP-06", "FOLIO-GAP-09", "FOLIO-GAP-11", "FOLIO-GAP-12"] },
  { file: "D-005-case-decision-record", ids: ["UNB-UI-D-005", "UNB-UI-G-016", "UNB-UI-G-018"], ref: "2026-000731", donorFacing: false,
    gaps: ["FOLIO-GAP-04", "FOLIO-GAP-05", "FOLIO-GAP-06", "FOLIO-GAP-11"] },
  { file: "D-006-audit-evidence-package", ids: ["UNB-UI-D-006", "UNB-UI-G-016", "UNB-UI-G-018"], ref: "2026-990004", donorFacing: false,
    gaps: ["FOLIO-GAP-03", "FOLIO-GAP-04", "FOLIO-GAP-05", "FOLIO-GAP-06", "FOLIO-GAP-09", "FOLIO-GAP-10", "FOLIO-GAP-11"] },
];

// ---------- pure helpers ----------
const sha256 = buf => createHash("sha256").update(buf).digest("hex");
const squash = s => s.replace(/\s+/g, " ").trim();
const noSpace = s => s.replace(/\s+/g, "");
const unique = xs => [...new Set(xs)];
const all = xs => xs.length > 0 && xs.every(Boolean);
const decodeEntities = s => s.replaceAll("&amp;", "&").replaceAll("&middot;", "·").replaceAll("&quot;", '"').replaceAll("&lt;", "<").replaceAll("&gt;", ">");
const between = (s, a, b) => { const i = s.indexOf(a); const j = s.indexOf(b, i + 1); return i < 0 ? "" : s.slice(i, j < 0 ? undefined : j); };
const rowRefs = html => unique([...html.matchAll(/<th scope="row">(\d{4}-\d{6})<\/th>/g)].map(m => m[1]));
const pageNumberOk = (pages) => pages.every((p, i) => new RegExp(`Page\\s+${i + 1}\\s+of\\s+${pages.length}\\b`).test(p));
const moneyTokens = text => [...text.matchAll(/\$[\d,.]*\d/g)].map(m => m[0]);
const moneyOk = text => moneyTokens(text).every(t => /^\$\d{1,3}(,\d{3})*\.\d{2}$/.test(t));
const pagesContaining = (pages, needles) => pages.map((p, i) => ({ i, p })).filter(({ p }) => needles.some(n => p.includes(n)));
// Strip the volatile Info-dictionary dates and trailer /ID so we can tell whether that is the ONLY byte difference.
const normalizePdf = buf => buf.toString("latin1")
  .replace(/\/(CreationDate|ModDate)\s*\(D:[^)]*\)/g, "/$1 ()")
  .replace(/\/ID\s*\[\s*<[0-9A-Fa-f]*>\s*<[0-9A-Fa-f]*>\s*\]/g, "/ID []");

const pdfinfo = file => Object.fromEntries(execFileSync("pdfinfo", [file], { encoding: "utf8" })
  .split("\n").filter(Boolean).map(l => [l.slice(0, l.indexOf(":")).trim(), l.slice(l.indexOf(":") + 1).trim()]));
const pdfPages = file => execFileSync("pdftotext", ["-enc", "UTF-8", file, "-"], { encoding: "utf8" })
  .split("\f").slice(0, Number(pdfinfo(file).Pages));
// Checks read content-stream order (-raw), joined so a line-end hyphen keeps its hyphen ("00-\n0000000" -> "00-0000000")
// and other line breaks become spaces. Hashing uses the default pdftotext output (pdfPages).
const pdfPagesForChecks = file => execFileSync("pdftotext", ["-raw", "-enc", "UTF-8", file, "-"], { encoding: "utf8" })
  .split("\f").slice(0, Number(pdfinfo(file).Pages)).map(p => squash(p.replace(/-\n/g, "-")));
const pdfRawStrings = file => readFileSync(file).toString("latin1");

// ---------- static server (register.js is an ES module; file:// module loading is blocked) ----------
const MIME = { ".html": "text/html; charset=utf-8", ".js": "text/javascript", ".css": "text/css", ".json": "application/json" };
const startServer = () => new Promise(resolve => {
  const server = createServer((req, res) => {
    const path = normalize(join(root, decodeURIComponent(new URL(req.url, "http://x").pathname)));
    if (!path.startsWith(root) || !existsSync(path)) { res.writeHead(404).end(); return; }
    res.writeHead(200, { "content-type": MIME[extname(path)] ?? "application/octet-stream" }).end(readFileSync(path));
  });
  server.listen(0, "127.0.0.1", () => resolve(server));
});

const openDocument = async (browser, url, viewport = { width: 1280, height: 1656 }) => {
  const context = await browser.newContext({ viewport, deviceScaleFactor: 1 });
  const page = await context.newPage();
  const consoleErrors = [];
  page.on("console", m => m.type() === "error" && consoleErrors.push(m.text()));
  page.on("pageerror", e => consoleErrors.push(String(e)));
  await page.goto(url, { waitUntil: "load" });
  const defined = await page.evaluate(async () => {
    const names = unique => [...new Set([...document.querySelectorAll("*")].map(e => e.localName).filter(n => n.startsWith("ef-print-")))];
    const used = names();
    await Promise.all(used.map(n => customElements.whenDefined(n)));
    await document.fonts.ready;
    return used.map(n => ({ name: n, defined: !!customElements.get(n) }));
  });
  return { context, page, defined, consoleErrors };
};

const renderOnce = async ({ url, pdfPath, pngPath, extra = {}, injectCss }) => {
  const browser = await chromium.launch();
  try {
    const { context, page, defined, consoleErrors } = await openDocument(browser, url);
    if (pngPath) await page.screenshot({ path: pngPath });
    if (injectCss) await page.addStyleTag({ content: injectCss });
    await page.pdf({ ...PDF_OPTIONS, ...extra, path: pdfPath });
    await context.close();
    return { defined, consoleErrors, browserVersion: browser.version() };
  } finally { await browser.close(); }
};

const runAxe = async url => {
  const browser = await chromium.launch();
  try {
    const { context, page } = await openDocument(browser, url, { width: 1280, height: 900 });
    await page.addScriptTag({ path: axePath });
    const result = await page.evaluate(tags => window.axe.run(document, { runOnly: { type: "tag", values: tags } }), AXE_TAGS);
    await context.close();
    return {
      violations: result.violations.map(v => ({ id: v.id, impact: v.impact, help: v.help, nodes: v.nodes.length, targets: v.nodes.slice(0, 5).map(n => n.target.join(" ")) })),
      passes: result.passes.length,
      incomplete: result.incomplete.map(v => ({ id: v.id, nodes: v.nodes.length })),
    };
  } finally { await browser.close(); }
};

// P1 probe: what Folio's own Signal fixture hand-authors. NOT part of the documents.
const marginBoxProbeCss = ref => `@page { @top-right { content: "Reference ${ref}"; font-size: 8pt; }
  @bottom-right { content: "Page " counter(page) " of " counter(pages); font-size: 8pt; } }`;

// ---------- checks ----------
const commonChecks = ({ doc, html, pages, info }) => {
  const text = pages.join("\n");
  const customTags = unique([...html.matchAll(/<([a-z][a-z0-9]*-[a-z0-9-]+)/g)].map(m => m[1]));
  return {
    html_has_no_authored_css: !/<style[\s>]/i.test(html) && !/\sstyle\s*=/i.test(html),
    html_uses_only_folio_custom_elements: customTags.every(t => FOLIO_ELEMENTS.has(t)),
    pdf_page_size_letter: /612 x 792/.test(info["Page size"] ?? ""),
    reference_on_page_1: pages[0].includes(doc.ref),
    reference_on_every_page: pages.every(p => p.includes(doc.ref)),
    page_numbering_n_of_N_on_every_page: pageNumberOk(pages),
    verification_code_present: /code [0-9A-Z]{4}-[0-9A-Z]{4}-[0-9A-Z]{4}/.test(text),
    verification_url_extractable_intact: text.includes(`https://verify.harborgiving.example/d/${doc.ref}`),
    verification_code_on_every_page: pages.every(p => /Verify at/.test(p)),
    money_format_all_amounts_two_decimals: moneyOk(text),
    headings_kept_with_following_table: [...html.matchAll(/<h[23][^>]*>([^<]+)<\/h[23]>\s*<ef-print-table>\s*<table>\s*<caption>([^<]+)<\/caption>/g)]
      .every(([, h, c]) => pages.some(p => p.includes(`${decodeEntities(h)} ${decodeEntities(c)}`))),
    no_relative_dates: !/\b(ago|yesterday|tomorrow)\b/i.test(text),
    ...(doc.donorFacing ? {
      legal_sponsor_disclosure_present: squash(text).includes(SPONSOR_DISCLOSURE),
      legal_sponsor_disclosure_on_every_page: pages.every(p => squash(p).includes(SPONSOR_DISCLOSURE)),
    } : {}),
  };
};

const specificChecks = {
  "D-001-acknowledgment-cash": ({ pages }) => {
    const t = squash(pages.join("\n"));
    return {
      protected_exclusive_control_sentence_verbatim: t.includes(EXCLUSIVE_CONTROL),
      goods_or_services_statement_verbatim: t.includes(NO_GOODS),
      sponsor_legal_name_and_ein: t.includes("Example Charitable Foundation") && t.includes("EIN 00-0000000"),
      donor_name: t.includes("Elena Rivera"),
      date_of_gift_absolute: t.includes("Date of gift") && t.includes("16 Nov 2026"),
      cash_amount_25000_00: t.includes("$25,000.00") && t.includes("ACH"),
      contribution_reference: t.includes("2026-101942"),
      single_page_letter: pages.length === 1,
    };
  },
  "D-001-acknowledgment-securities": ({ pages, html }) => {
    const t = squash(pages.join("\n"));
    const desc = between(t, "600 shares", "No goods or services");
    return {
      protected_exclusive_control_sentence_verbatim: t.includes(EXCLUSIVE_CONTROL),
      goods_or_services_statement_verbatim: t.includes(NO_GOODS),
      sponsor_legal_name_and_ein: t.includes("Example Charitable Foundation") && t.includes("EIN 00-0000000"),
      donor_name: t.includes("Elena Rivera"),
      date_of_gift_absolute: t.includes("Date of gift") && t.includes("18 Nov 2026"),
      property_description_600_shares: t.includes("600 shares of Northwind Analytics, Inc. common stock"),
      property_description_has_no_dollar_value: desc.length > 0 && !/\$|USD|\bvalue[d]? at\b/i.test(desc),
      document_has_no_currency_amount_anywhere: moneyTokens(t).length === 0 && !/\$/.test(html.replace(/\$\{/g, "")),
      single_page_letter: pages.length === 1,
    };
  },
  "D-002-fund-statement": ({ pages, html }) => {
    const paid = rowRefs(between(html, 'id="grants"', 'id="pending"'));
    const grantPages = pagesContaining(pages, paid);
    const summaryLabels = ["OPENING VALUE", "CONTRIBUTIONS", "GRANTS PAID", "FEES", "INVESTMENT CHANGE", "CLOSING VALUE", "AVAILABLE TO GRANT"];
    const summaryPage = pages.findIndex(p => p.includes("OPENING VALUE"));
    const totalHits = pages.map(p => (p.match(/Total grants paid/g) ?? []).length);
    const t = squash(pages.join("\n"));
    return {
      grant_rows_at_least_70: paid.length >= 70,
      all_grant_rows_in_pdf: paid.every(r => t.includes(r)),
      grants_table_spans_pages: grantPages.length >= 2,
      grants_header_repeated_on_every_grants_page: all(grantPages.map(({ p }) => p.includes("Grant reference"))),
      period_summary_kept_on_one_page: summaryPage >= 0 && summaryLabels.every(l => pages[summaryPage].toUpperCase().includes(l)),
      negative_investment_change_has_text_label: t.includes("−$4,210.33 (decrease)"),
      grants_total_row_printed_once: totalHits.reduce((a, b) => a + b, 0) === 1,
      pending_grants_present: t.includes("Pending grants") && t.includes("2026-004936"),
      fee_basis_present: t.includes("Fee basis"),
      protected_exclusive_control_sentence_verbatim: t.includes(EXCLUSIVE_CONTROL),
      g016_grant_2026_004817_present: t.includes("2026-004817"),
      toc_present: t.includes("Period summary") && t.includes("Disclosures"),
      _diagnostics: { grant_rows: paid.length, pages_with_grant_rows: grantPages.map(g => g.i + 1), grants_total_row_hits_per_page: totalHits },
    };
  },
  "D-003-grant-remittance": ({ pages, html, info, raw }) => {
    const t = squash(pages.join("\n"));
    const refs = rowRefs(html).filter(r => /^2026-00\d{4}$/.test(r));
    return {
      no_pledge_text_in_pdf_text: !/pledge/i.test(t),
      no_pledge_text_in_pdf_metadata_or_bytes: !/pledge/i.test(JSON.stringify(info)) && !/pledge/i.test(raw),
      no_pledge_text_in_html_source: !/pledge/i.test(html),
      several_grants_listed: refs.length >= 3 && refs.every(r => t.includes(r)),
      sponsor_stated_as_legal_donor: t.includes("Example Charitable Foundation (EIN 00-0000000) is the donor of each grant"),
      payment_method_and_date: t.includes("ACH credit") && t.includes("19 Aug 2026"),
      purpose_and_recognition: t.includes("Recognition") && t.includes("Emergency relief"),
      donor_confirmation_present: t.includes("Grant confirmation for the advisor"),
      g016_grant_2026_004817_present: t.includes("2026-004817"),
      remittance_total_matches: t.includes("$14,250.75"),
    };
  },
  "D-004-daf-reporting-package": ({ pages, html }) => {
    const t = squash(pages.join("\n"));
    const snap = (html.match(/Source snapshot SHA-256<\/dt><dd><code>([0-9a-f]{64})/) ?? [])[1];
    return {
      dafs_count_present: t.includes("1,284"),
      aggregate_contributions_present: t.includes("$48,210,773.18"),
      aggregate_grants_present: t.includes("$39,874,012.55"),
      aggregate_value_present: t.includes("$412,907,551.06"),
      reconciliation_to_ledger_table: t.includes("Ledger source") && t.includes("Difference"),
      classification_register_version: t.includes("v2025.4"),
      preparer_and_signer_fields: t.includes("Prepared by") && t.includes("Reviewed and signed by"),
      source_snapshot_hash_present: !!snap && noSpace(t).includes(snap),
      summary_metrics_and_integrity_kept_together: pages.some(p => p.includes("Year-end figures") && /SOURCE SNAPSHOT/i.test(p)),
    };
  },
  "D-005-case-decision-record": ({ pages, html }) => {
    const t = squash(pages.join("\n"));
    const hashes = [...html.matchAll(/<code>([0-9a-f]{64})<\/code>/g)].map(m => m[1]);
    return {
      case_reference: t.includes("2026-000731"),
      rule_that_raised_it: t.includes("SAN-OFAC-001"),
      evidence_with_source_and_freshness: t.includes("Freshness") && t.includes("Source") && hashes.every(h => noSpace(t).includes(h)),
      decision_and_rationale: /DECISION/i.test(t) && /RATIONALE/i.test(t) && t.includes("Confirmed match"),
      preparer_and_decider_are_different_people: t.includes("Prepared by Analyst Dana Whitfield (user 4471)") && t.includes("Decided by Marcus Lee (user 1093)"),
      ofac_report_due_date: t.includes("OFAC report due 4 Dec 2026"),
      timestamps_absolute_with_utc: t.includes("2026-11-19T13:22:45Z UTC") && t.includes("20 Nov 2026"),
    };
  },
  "D-006-audit-evidence-package": ({ pages, html }) => {
    const t = squash(pages.join("\n"));
    const hashes = [...html.matchAll(/<code>([0-9a-f]{64})<\/code>/g)].map(m => m[1]);
    const evidenceIds = unique([...html.matchAll(/<th scope="row">([A-Z0-9.-]+-E\d\d)<\/th>/g)].map(m => m[1]));
    const evPages = pagesContaining(pages, evidenceIds);
    const controls = [...html.matchAll(/<li><a href="#control-([^"]+)">/g)].map(m => m[1]);
    const toc = pages.find(p => p.includes("Contents")) ?? "";
    return {
      every_evidence_row_shows_sha256: evidenceIds.length > 0 && hashes.length === evidenceIds.length + 1 && hashes.every(h => noSpace(t).includes(h)),
      toc_lists_each_control_section: controls.length === 5 && controls.every(c => toc.includes(`Control ${c}`)),
      toc_shows_target_page_numbers: false, // Folio P0 TOC has no target-counter; [data-page] must be authored
      control_list_present: t.includes("Controls in scope"),
      exceptions_with_resolution: t.includes("Exceptions identified in the period") && t.includes("Resolution"),
      evidence_header_on_every_evidence_page: all(evPages.map(({ p }) => p.includes("Evidence ID"))),
      period_bounded: t.includes("1 Oct 2025") && t.includes("31 Dec 2025"),
    };
  },
};

// ---------- pipeline ----------
const verifyDocument = async (server, doc) => {
  const url = `http://127.0.0.1:${server.address().port}/documents/${doc.file}.html`;
  const html = readFileSync(join(docsDir, `${doc.file}.html`), "utf8");
  const pdf1 = join(renderDir, `${doc.file}.pdf`);
  const pdf2 = join(workDir, `${doc.file}.run2.pdf`);
  const pdfTagged = join(workDir, `${doc.file}.tagged.pdf`);
  const pdfProbe = join(workDir, `${doc.file}.p1-margin-probe.pdf`);

  const t0 = Date.now();
  const run1 = await renderOnce({ url, pdfPath: pdf1, pngPath: join(renderDir, `${doc.file}.png`) });
  const renderMs = Date.now() - t0;
  await renderOnce({ url, pdfPath: pdf2 });
  await renderOnce({ url, pdfPath: pdfTagged, extra: { tagged: true } });
  await renderOnce({ url, pdfPath: pdfProbe, injectCss: marginBoxProbeCss(doc.ref) });
  const axe = await runAxe(url);

  const info = pdfinfo(pdf1);
  const pages1 = pdfPages(pdf1);
  const checkPages = pdfPagesForChecks(pdf1);
  const pages2 = pdfPages(pdf2);
  const raw1 = readFileSync(pdf1);
  const raw2 = readFileSync(pdf2);
  const textHashes1 = pages1.map(p => sha256(p));
  const textHashes2 = pages2.map(p => sha256(p));
  const info2 = pdfinfo(pdf2);
  const probePages = pdfPagesForChecks(pdfProbe);

  const checks = {
    custom_elements_defined: all(run1.defined.map(d => d.defined)),
    no_console_errors: run1.consoleErrors.length === 0,
    ...commonChecks({ doc, html, pages: checkPages, info }),
    ...specificChecks[doc.file]({ doc, html, pages: checkPages, info, raw: pdfRawStrings(pdf1) }),
  };
  const { _diagnostics, ...namedChecks } = checks;

  return {
    document: `documents/${doc.file}.html`,
    pdf: `rendered/${doc.file}.pdf`,
    screenshot: `rendered/${doc.file}.png`,
    requirement_ids: doc.ids,
    folio_elements_used: run1.defined.map(d => d.name).sort(),
    folio_recipe_classes_used: unique([...html.matchAll(/class="([^"]+)"/g)].flatMap(m => m[1].split(/\s+/))).sort(),
    pages: Number(info.Pages),
    page_size: info["Page size"],
    render_ms_first_run: renderMs,
    checks: namedChecks,
    checks_passed: Object.values(namedChecks).filter(Boolean).length,
    checks_total: Object.keys(namedChecks).length,
    diagnostics: _diagnostics ?? {},
    raw_pdf_sha256: [sha256(raw1), sha256(raw2)],
    raw_pdf_hash_stable: sha256(raw1) === sha256(raw2),
    raw_pdf_hash_stable_after_stripping_dates_and_id: sha256(normalizePdf(raw1)) === sha256(normalizePdf(raw2)),
    raw_pdf_difference_explained_by: [
      info.CreationDate !== info2.CreationDate ? `CreationDate (${info.CreationDate} vs ${info2.CreationDate})` : null,
      info.ModDate !== info2.ModDate ? `ModDate (${info.ModDate} vs ${info2.ModDate})` : null,
    ].filter(Boolean),
    page_text_sha256: textHashes1,
    text_hash_stable: textHashes1.length === textHashes2.length && textHashes1.every((h, i) => h === textHashes2[i]),
    tagged_pdf: info.Tagged === "yes",
    tagged_pdf_with_chromium_tagged_option: pdfinfo(pdfTagged).Tagged === "yes",
    pdf_producer: info.Producer ?? null,
    p1_margin_box_probe: {
      note: "Injected @page margin-box CSS (not part of the document). Shows engine capability Folio does not package.",
      page_numbering_n_of_N_on_every_page: pageNumberOk(probePages),
      reference_on_every_page: probePages.every(p => p.includes(doc.ref)),
      pages: probePages.length,
    },
    axe: { tags: AXE_TAGS, viewport_width: 1280, ...axe },
    axe_violations: axe.violations,
    gaps: doc.gaps,
    browser: run1.browserVersion,
  };
};

const main = async () => {
  const server = await startServer();
  try {
    const results = [];
    for (const doc of DOCS) {
      results.push(await verifyDocument(server, doc));
      const r = results.at(-1);
      console.log(`${doc.file}: pages=${r.pages} checks=${r.checks_passed}/${r.checks_total} raw=${r.raw_pdf_hash_stable} text=${r.text_hash_stable} tagged=${r.tagged_pdf}/${r.tagged_pdf_with_chromium_tagged_option} axe=${r.axe_violations.length}`);
      Object.entries(r.checks).filter(([, v]) => !v).forEach(([k]) => console.log(`   FAIL ${k}`));
    }
    const out = {
      generated_by: "requirements/ui/designs/tools/verify-documents.mjs",
      folio: { source: "kemiller2002/folio", commit: "56a18e1", version: "0.4.2", vendored_at: "requirements/ui/designs/vendor/folio-0.4.2" },
      renderer: { engine: "Chromium via Playwright (preinstalled)", browser: results[0]?.browser, pdf_options: PDF_OPTIONS },
      axe_core: "4.10.2",
      documents: results,
    };
    writeFileSync(join(docsDir, "verification.json"), JSON.stringify(out, null, 2) + "\n");
  } finally { server.close(); }
};

main().catch(e => { console.error(e); process.exit(1); });
