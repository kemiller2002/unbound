# Folio fit report: Unbound print documents (UNB-UI-D-001..006)

Date: 2026-11-23. Folio kemiller2002/folio@56a18e1 (v0.4.2), vendored at `../vendor/folio-0.4.2/` (design review only).
Renderer: Chromium 157.0.8103.78 (Playwright 1.63.0, preinstalled), `page.pdf({format:'Letter', printBackground:true, preferCSSPageSize:true})`.
Evidence: `verification.json` (every check, hash and axe result); PDFs and page-1 PNGs (1280 px) in `../rendered/`.
Tools: `../tools/build-documents.mjs` generates the HTML from seeded synthetic data. `../tools/verify-documents.mjs` renders and checks it.

**Authoring rule followed:** Folio custom elements, Folio recipe classes (`ef-identity`, `ef-meta-list`, `ef-category-grid`, `ef-dense`) and semantic HTML only. There is no `<style>` element, no `style=""` attribute and no consumer stylesheet. Every document passes `html_has_no_authored_css` and `html_uses_only_folio_custom_elements`.

## 1. Fit by document

| Document | Pages | Verdict | Reason |
|---|---|---|---|
| D-001 acknowledgment, cash | 1 | **Fits with recorded gaps** | All legal content passes, including both protected sentences verbatim and $25,000.00 ACH. The letter is built with the FOLIO-GAP-01 and GAP-02 workarounds. Problems: the verification URL breaks across lines (GAP-06), and the address block has no fixed window-envelope position (GAP-07). The letter only fit on one page after trimming. The in-flow footer had spilled the legal-sponsor disclosure onto a page 2 with no reference (GAP-05). |
| D-001 acknowledgment, securities | 1 | **Fits with recorded gaps** | Same as the cash variant. "600 shares of Northwind Analytics, Inc. common stock" appears with no value, and the document contains no currency amount anywhere. |
| D-002 fund statement | 8 | **Does not fit (Folio 0.4.2 alone)** | The grants table spans pages 3–7 and repeats its header on each one. The period summary stays on one page. But the "Page n of N" MUST fails on 0 of 8 pages. The reference, verification code and disclosure are missing from the continuation pages. The grand-total row repeats at the foot of pages 3–7. It becomes "fits with gap" if GAP-04, GAP-05 and GAP-08 are closed. The P1 probe shows the engine can already do this. |
| D-003 grant remittance + advisor confirmation | 2 | **Fits with recorded gaps** | No "pledge" text appears in the PDF text, PDF bytes/metadata or HTML. Four grants are listed, the sponsor is stated as the legal donor, and the total is $14,250.75. Problems: no window-envelope position (GAP-07). The advisor confirmation page carries the remittance's footer reference because there is no per-section running content (GAP-05). |
| D-004 DAF reporting package | 3 | **Fits with recorded gaps** | All Schedule D Part I figures, the ledger reconciliation, the register version, the snapshot SHA-256 and the preparer/signer fields are present. `ef-print-integrity` fits well. Problems: signer fields come from `ef-meta-list` because there is no signature/approval recipe (GAP-12). The reference passed "every page" only by coincidence: the title page, then the header on page 2, then the footer on page 3. |
| D-005 case decision record | 2 | **Fits fully** for its own statement | Every content check passes: preparer and decider are different people, and the OFAC report due date is shown. The general gaps GAP-04, GAP-06 and GAP-11 still apply: no page numbers, and evidence IDs and hashes wrap mid-token. |
| D-006 audit evidence package | 6 | **Fits with recorded gaps** | Both acceptance criteria pass: all 43 evidence rows show their SHA-256, and the table of contents lists all 5 control sections. Problems: pages 3–6 carry no reference (GAP-05), there is no page numbering (GAP-04), and the table of contents cannot show target pages (GAP-10). |

## 2. Check results

All 7 documents passed these checks:
- Custom elements defined; no console errors.
- Letter page size (612×792).
- Reference on page 1.
- Money format: every `$` amount has two decimals with thousands separators. The negative value reads "−$4,210.33 (decrease)".
- No relative dates.
- Headings kept with the following table. This holds only with the `.ef-dense` recipe. In the first render without it, D-002's "Grants paid" heading was orphaned at the foot of page 2.
- **axe-core 4.10.2** (tags wcag2a, wcag2aa, wcag21a, wcag21aa, wcag22aa, 1280 px): **0 violations in all 7**. One earlier *incomplete* result, `aria-prohibited-attr` (`aria-labelledby` on `ef-print-integrity`, which has no role), was fixed by adding `role="group"`.

Required checks:

| Check | Result |
|---|---|
| D-002 header row repeated on continuation pages | **PASS.** "Grant reference" appears on every page with grant rows (pages 3–7, 72 rows). |
| "Page n of N" on every page | **FAIL in all 7** (0 pages). `ef-print-page-number` only prints its fallback text. The P1 probe passes 7/7. |
| D-001 protected sentence verbatim | **PASS** in both variants. |
| D-001 goods-or-services statement verbatim | **PASS** in both variants. |
| D-001 securities: no dollar value in the property description | **PASS.** There is also no currency amount anywhere in the document. |
| D-003 no pledge text | **PASS** in PDF text, PDF bytes/metadata and HTML source. |
| Tagged PDF (`pdfinfo Tagged:`) | **no** for all 7 with the specified options. **yes** for all 7 when rendered with Chromium's `tagged:true` (probe). |
| Verification URL extractable intact | **FAIL in all 7.** The footer column breaks it, e.g. `…/d/202` + `6-880114`. |
| Reference / verification code / disclosure on every page | **FAIL** wherever a document has more pages than its in-flow header and footer can cover (D-002, D-003, D-004, D-005, D-006). |
| D-002 grand total printed once | **FAIL.** The `tfoot` total "Total grants paid $183,677.18" repeats on pages 3, 4, 5, 6 and 7. |
| D-006 table of contents with target page numbers | **FAIL.** P0 table of contents only. |

**Determinism** (two independent browser launches per document):
- **(b) Per-page `pdftotext` SHA-256: stable for 7/7.** This is the determinism claim.
- **(a) Raw PDF SHA-256: not stable in general.** In the final run it matched for 6/7. D-004 differed (`cd86eb69…` vs `557dc0c5…`), and earlier runs differed for other documents. The only difference is the Info dictionary's `/CreationDate` and `/ModDate`, which are stamped to the second: the bytes match whenever both renders fall within the same second.
- After stripping those two dates, the bytes are **identical for 7/7**. Chromium writes no trailer `/ID`.
- Render time is about 0.25 s per document after launch.

**P1 margin-box probe:** two lines of `@page { @top-right / @bottom-right }` CSS, injected at render time and not part of the documents. This is the same technique Folio's own Signal fixture hand-authors. Result: "Page n of N" and the reference on every page pass for **7/7**. The engine is capable; Folio just does not ship the rule.

## 3. New Folio gaps (beyond FOLIO-GAP-01..03)

| ID | Component | Need | Evidence | Acceptance criterion |
|---|---|---|---|---|
| **FOLIO-GAP-04** | `ef-print-page-number` / P1–P2 page counters | D-002 MUST "Page n of N"; G-016 document identity | `print.css` only sets `--ef-print-page-number-format`, with no `@page` margin-box or `counter(pages)` rule. 0 of 23 pages show numbering. Injected margin-box CSS passes 7/7. | With no consumer CSS, `ef-print-footer[repeat=page]` containing `ef-print-page-number[format=page-of-pages]` yields "Page n of N" on every body page in P1/P2 Chromium and is suppressed on the title page. At P0, strict mode fails the export (EPC-CAP-011) instead of printing fallback text. |
| **FOLIO-GAP-05** | `ef-print-header` / `ef-print-footer` `repeat="page"` (running content) | Reference (G-016), legal-sponsor disclosure (G-014) and verification code on every page; per-section identity | `repeat` sets only a custom property, so header and footer render once, in flow. D-002 pages 3–8 and D-006 pages 3–6 have no reference. D-002, D-003 and D-004 lack the disclosure on continuation pages. The footer is not `break-inside: avoid`: in the first D-001 draft its disclosure paragraph spilled alone onto page 2. On D-003 the advisor-confirmation page shows the remittance's reference. | Running header and footer slots (left, center, right, plus a full-width row) print on every page of their page profile, never fragment, reserve their own margin space, and can be set per section/document unit. Verified by `pdftotext`: the reference and disclosure appear on every page of a 70-row statement. |
| **FOLIO-GAP-06** | Identifier/reference text treatment | References, EINs, URLs and hashes must print and extract intact | Folio sets `overflow-wrap: break-word` on `html` and `anywhere` on `td`. Results: the verification URL breaks in all 7 documents; "EIN 00-␤0000000" extracts as "000000000" with default `pdftotext` (dehyphenated); D-005's "EV-␤1" and dates wrap ("21 Jul␤2026"). | A recipe or primitive (e.g. `.ef-identifier`, `<data>`) keeps references of 32 characters or fewer and dates unbroken, and breaks hashes and URLs only at declared points. The `pdftotext` and `-raw` extraction of each identifier equals the source string. |
| **FOLIO-GAP-07** | Letter address block at a fixed window-envelope position (split from GAP-01 because it is a measurable print-position contract) | D-001 and D-003 are mailed | `<address>` flows after the letterhead, at a position that depends on content height. There is no absolute or page-anchored primitive (`ef-print-layer` is decorative). | An address primitive places its box at a configurable fixed page position (e.g. US #10 window: 0.875 in left, 2.0 in top, 4.0 × 1.125 in). `pdftotext -bbox` shows the address within the window for any letterhead length. |
| **FOLIO-GAP-08** | `ef-print-table` totals on multi-page tables | Statement totals must not mislead | `tfoot { display: table-footer-group }` repeats the grand-total row at the foot of every fragment (D-002: 5 times, pages 3–7). | An opt-in attribute (e.g. `ef-print-table[footer="final"]`) prints `tfoot` only on the last fragment. An optional carried-forward / page-subtotal pattern is documented and tested. |
| **FOLIO-GAP-09** | Heading keep-with-next default | EPC-FRAG-002/012 | The component layer gives headings no `break-after: avoid`. This is only available through the `.ef-dense` / `.ef-longform` recipes: without them, D-002's "Grants paid" heading was orphaned. | Keep-with-next for h1–h6 before blocks and `ef-print-table` by default in the component layer. A fixture shows no heading in the last 10% of any page. |
| **FOLIO-GAP-10** | `ef-print-toc` target page numbers | D-002 and D-006 tables of contents are more useful with page numbers | `[data-page]` must be authored. Chromium has no `target-counter`, and Folio offers no two-pass P2 fill. | P2 export fills `[data-page]` from a first-pass layout (or P3 adapter), or strict mode reports the capability as missing. Page numbers match the actual PDF. |
| **FOLIO-GAP-11** | Tagged PDF in the P2 export contract | RCD-009 makes HTML the accessible rendition, but the PDF is distributed too | `Tagged: no` for all 7 with the specified options. `tagged:true` gives `Tagged: yes`, but structure quality is unvalidated and the P2 option set is unspecified. | The P2 contract pins `tagged:true` (and `outline:true`) and validates headings, table header cells, reading order and `/Lang` with a PDF/UA checker (veraPDF or PAC), with documented limits (EPC-A11Y-017/018). |
| **FOLIO-GAP-12** | Signature / approval block recipe (EPC-CONT-022) | D-004 preparer and signer; D-005 decider | Uses `ef-meta-list`. Nothing keeps the block together or marks it as a signature or approval. | Recipe with name, role, date (UTC detail) and signature evidence, kept together, with a semantic `dl` and 3 documented examples. |

Keep-together of summary blocks is **not** a gap. `ef-print-keep` around a list of `ef-print-metric` items laid out with `ef-category-grid` kept all 7 D-002 summary metrics on one page, and kept D-004's metrics with their integrity block. Repeating table headers is also not a gap. However, `ef-category-grid` is a resume recipe being reused for metrics. A documented metric-grid recipe would make that intent explicit.
