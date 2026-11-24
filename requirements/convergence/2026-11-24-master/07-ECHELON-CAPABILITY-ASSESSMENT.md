# 07 — Echelon capability assessment (Phase 5.7)

Provenance: read-only inspection of shallow clones of the public Echelon repositories and the local Enterprise Design Framework checkout on 2026-11-24 by a delegated research pass; machine-readable records in [`data/echelon-inventory.jsonl`](data/echelon-inventory.jsonl). No builds or tests were run; capabilities are backed by documents and source paths in the named repositories. The traceability of each gap to Unbound requirements and the resulting requirement changes are in section "Unbound requirement consequences" at the end of this report.


Date: 2026-11-24. This was a read-only inspection of shallow clones of public `kemiller2002/*` repositories, plus the local Enterprise Design Framework checkout. Nothing was modified. Machine-readable records are in `echelon-inventory.jsonl`, with one object per system.

**Method and limits.** For each repository I read the README, AGENTS/CHANGELOG where present, the git tags (`git ls-remote`) and the key docs and source. I cross-checked versions against `echelon-registry` channel `echelon-current` profile 1.39.0 (`sets/echelon-current/1.39.0.json`). The GitHub Releases API could not be reached from this session, so "released" means a git tag, a registry release manifest, or both. clarity-framework is private and was **not inspected**. No tests or builds were run, so every capability below is backed by documents and source only.

## Version reality: tags, the registry channel and Unbound's vendor copies disagree

| System | Latest tag | HEAD | Registry `echelon-current` | Unbound vendored |
|---|---|---|---|---|
| Limen | v0.9.1 (= HEAD 743cdb9) | 743cdb9 | 0.9.1; limen-fsharp 0.9.0 | not vendored |
| Forma | v1.1.0 (= HEAD c8f40b4) | c8f40b4 | **1.0.1** | "1.0.2" at **d7c499e**, an untagged commit on main |
| Folio | v0.4.1 (a94cea7) | 56a18e1, package 0.4.2, **untagged** | 0.4.1 | "0.4.2" at 56a18e1, untagged |
| Strata | v0.1.1 (f650d91) | c6a384e (ahead) | 0.1.1 | n/a |
| Echelon theme | v1.1.0 | 2c0e82e | 1.1.0 | 1.1.0 fonts |
| Conditor | v0.10.25 | f29d01c | manifests only up to 0.10.19 | n/a |

Consequences:
- A Conditor-installed Unbound would get Forma 1.0.1, which has none of the 1.1.0 gap closures.
- Unbound's Folio "0.4.2" is not a release, so it cannot be verified by digest.
- Every inspected system except the Registry (MIT), the Echelon Diagnostic Framework (MIT) and the echelon theme (UNLICENSED in `package.json`) is **PolyForm Noncommercial 1.0.0**. The local Enterprise Design Framework has no LICENSE file. Commercial licensing remains a launch blocker, as Unbound already recorded.

## 1. Limen (F# WebAssembly-first application hosting)

**What it is.** A dependency-free *TypeScript* browser kernel and JSON protocol. An "engine" in any language sits behind `EngineTransport` (`src/protocol.ts`). The npm package ships **no WASM engine** (`docs/17-wasm-migration.md`). F# appears in four places: the lifecycle CLI, the product-site engine compiled to .NET WASM (an existence proof), a federation proof, and four published NuGet packages (Contract, Guest, Routing, Store; `releases/limen-fsharp/0.9.0.release.json`). The other F# libraries (Forms, Http, Resources, Outbox, Localization) exist **only as source**: their `.fsproj` files have no PackageId.

**Verified against Unbound needs:**
- **Hosting.** .NET WASM transport (`src/hosts/dotnet-wasm-transport.ts`), worker hosting (`docs/50`), SSR/SSG renderer (`docs/54`). Measured in a sandbox: a trimmed F# guest is 5.5 MB, 29 requests, 521 ms cold handshake; untrimmed is 26 MB (`docs/27-performance-baseline.md:117-118`). This partly answers Unbound DG-07, but nothing was measured on mobile devices.
- **Routing.** Typed routes, deep links, hash routing for static hosts, sign-in `returnTo`. TypeScript and F# libraries pass 165 shared vectors (`docs/31-routing.md`, CHANGELOG 0.9.0). Guards are explicitly "not an authorization boundary".
- **Auth.** A WebAuthn passkey *transport* pack (`src/capabilities/credentials`, which does not verify) and an Http credentials/XSRF option (`docs/43`). There is **no IdP, session or tenant-token capability**.
- **Offline.** Service-worker registration and update, a cached shell, and an IndexedDB store pack (released in 0.7.0). The outbox for unknown outcomes is source-only (`docs/49`, `docs/41`).
- **Embeddable widgets.** **Not supported as a host.** `BrowserKernel` takes a `Document` and binds `document.head` and `document.body` (`src/kernel/browser-kernel.ts:322,352`), with no element or shadow-root mount. Limen can *host* third-party web components behind governed adapters (`docs/42`), which is the reverse of what Unbound needs for partner embeds.
- **Accessibility.** An accessible-patterns example covering tabs, menu, listbox, combobox, tree, grid and dialog (`docs/37`), a focus/selection/scroll pack (`docs/30`), and native `<dialog>`/popover overlays (`docs/38`). The kernel projects ARIA state; it owns no widget semantics.

**Risks.** The 0.x protocol "may change in a minor release". The Mono WASM interpreter frame-limit failure mode is only caught by `limen wasm-check`, which is unreleased. Unbound writes every engine.

## 2. Forma (UI components)

**Forma 1.1.0** (the tag equals HEAD) is a zero-runtime design system: HTML patterns, CSS and tokens. It has 174 components in 18 families, and 28 components are required by its own catalog but not implemented (`docs/COMPONENT-INVENTORY.md`). The `<forma-*>` authoring tags are inert; they are not custom elements.

| Need | Status | Evidence |
|---|---|---|
| App frame | **Missing.** `.forma-app-shell` is planned; workspace-shell and navigation-shell cover only the regions | inventory "Required but not implemented" |
| Tables | data-grid and dense-ledger exist; plain table styling, editable grid and virtual list are planned | inventory |
| Forms | form, form-row (subgrid alignment), fields, combobox, date-time-field; invalid/disabled/read-only styles exist | `src/styles/foundations.css:160-177` |
| Money input/display | **Missing.** No money or numeric primitive; tabular numerals appear only inside some components | `src/styles/components.css` |
| Status chips | status-lozenge plus the new sentence variant (1.1.0); removable or selectable chip is planned | `components.css:1259` |
| Task lists | `task-list` (1.1.0) and work-queue | `patterns/task-list.html` |
| Charts/dashboards | dashboard-grid, metric-card, metric-value-change. **No chart primitives or chart accessibility contract** | inventory |
| Email designer | **Requirements only** ("proposed; no implementation"); Unbound is named first consumer | `docs/requirements/email-designer-2026-11-23.md` |
| White-label | Brand Manifest schema compiled to scoped `[data-forma-brand]` CSS with contrast-pair checks; skins; surface/elevation roles per brand | `docs/BRANDING.md`, `tests/brand-compiler.test.mjs` |
| WCAG evidence | axe WCAG 2.2 AA browser tests at 320 and 1280 px, forced colours, keyboard, contrast and non-text contrast tests, a ui-conformance CI job. Chromium only; "pilot, not a passed release gate"; no conformance statement found; about 40 documented a11y/layout known issues | `docs/UNIVERSAL-UI-CONFORMANCE.md`, inventory "Known issues" |
| Embedded components | Only the separate `<forma-workflow>` package is embeddable; the CSS is not shadow-DOM-packaged | `docs/workflow/EMBEDDING.md` |

**Prior Unbound FORMA gaps, re-checked at v1.1.0:**
- **Closed in 1.1.0** (but absent from the registry channel and from Unbound's vendor copy):
  - GAP-01 (primary/destructive buttons)
  - GAP-19 (breakpoint tokens)
  - GAP-20 (surface roles and elevation)
  - GAP-21 (sentence lozenge)
  - GAP-22 (task-list)
  - GAP-23 (domain icons; independent visual review still open)
  - GAP-24 (approval-quorum)
  - GAP-25 (period-context)
  - GAP-26 (batch-outcome)
- **Partly closed:**
  - GAP-18: density shipped; the application type scale did not.
  - GAP-15: container queries now on record-header, data-grid and task-list only.
- **Implemented, documentation wrong:** GAP-02. Unbound's correction was right; the upstream inventory still lists it as unimplemented.
- **Still open:**
  - GAP-03, -04 (planned)
  - GAP-05 (charts)
  - GAP-06 (app shell)
  - GAP-07 (copyable identifier)
  - GAP-08 (money)
  - GAP-09 (status track)
  - GAP-11 (unavailable action)
  - GAP-12 (read-only allocation)
  - GAP-13: `patterns/feed-item.html:7` still uses `data-state="warning"`
  - GAP-14 (pinned dialog)
  - GAP-16 (AI provenance)
  - GAP-17: the flyout body still lacks `role=region`/`tabindex`
- **Superseded figure:** the 2026-11-23 assessment's "170 components / 29 unimplemented" is now 174 / 28.

## 3. Folio (document and print generation)

Folio provides 21 passive light-DOM `ef-print-*` elements (`src/components/register.js`, 48 lines) and `print.css`. Its renderer tiers are P0 portable, P1 Chromium margin boxes, P2 deterministic Chromium and P3 enhanced paged media. Folio is honest about what it cannot do:
- **No renderer.** `src/engine` and `src/kernel` are empty. PDF generation exists only in Playwright experiments (`tests/run-print-experiments.mjs`). There is no batch generator and no throughput evidence. The P2 export adapter is a requirement only (EPC-OUT-002/003).
- **Tagged PDF and PDF/UA are explicitly not claimed** (`PRINT-COMPONENTS-REQUIREMENTS.md:294-295,485,672,1025`). PDF/A and signing are absent.
- **No Typst** support or interoperability.
- **Deterministic re-render** is specified but not implemented. The Unbound render service must own the pinned Chromium, fonts, options and hashing.
- **Repeated header/footer:** `repeat="page"` deliberately does not fake repeated header/footer content (`print.css:213-218`). Target-page cross-references and true footnotes are unsupported at P0-P2.

**Prior FOLIO gaps:** all of FOLIO-GAP-01 to GAP-12 are **still open** at HEAD 56a18e1. In particular:
- no counter or margin-box rule (GAP-04)
- `tfoot` still repeats (GAP-08)
- heading keep-with-next only in `.ef-dense`/`.ef-longform` (GAP-09)
- identifiers still break under `overflow-wrap:anywhere` (GAP-06)

Unbound's statement "no PDF service or batch generator" is correct. One new gap: the vendored "0.4.2" is not a tagged release.

## 4. Strata (schema governance and mock interactions)

Strata v0.1.1 is a **pilot**. It does declarative PostgreSQL 16 schema management: compile, plan, apply, deploy, drift, validate, check and inspect, with six comparison states that are never collapsed. It has signed artifacts, native binaries with Sigstore provenance, and no linux-arm64 build.
- **Fits** DDL governance if Unbound standardises on PostgreSQL 16 (Unbound requirements mention PostgreSQL about 149 times).
- **Deterministic mock interactions are requirements only.** STRATA-DIV-001 (`docs/requirements/data-interaction-virtualization.md`, "Proposed") has DAF as first consumer: virtual time, fault injection, replay, Scenario Studio. Nothing is implemented.
- Unbound was correct that Strata does not provide append-only history, temporal queries or immutable posting.
- The README reports its own null result: agents used 8.7% more tokens with Strata.

## 5. Lifecycle roles for other systems

| Lifecycle role | Best candidate | Verified state | Fit |
|---|---|---|---|
| Requirements governance | **Praxis** v3.25.0 | Work protocol, provenance, validation; `praxis requirements manifest compile` | Good after converting Unbound CSV/Markdown to the Praxis catalog format |
| Acceptance evidence | **Dokimos** 0.11.1 | Manifest-driven requirement verification from `data-requirement-id` plus browser observations; behaviour verification against frozen oracles | Partial: no BDD or financial-invariant runner. Unbound's own F# property tests must feed it |
| UI contract verification | **Percepta** 0.1.0 / semantic 0.4.0 | Semantic UI contract compiler/verifier | Promising, not evaluated in depth |
| Installation | **Conditor** (tag 0.10.25; registry 0.10.19) | Deterministic plan, install, verify, lock; F#/Limen and NuGet scaffolds | Good; installs channel versions (Forma 1.0.1) |
| Registry/discovery | **Echelon Registry** (MIT) | Release manifests with digests; channels; planned-only capabilities (Summa ledger, Chrona time) | Good; pin by manifest digest |
| Fault handling | **Aegis** 1.0.0 (NuGet) | Fault capture, redaction, presentation; Forma fault family | Good. **Correction: Aegis is not a security/assurance system** |
| Security assurance | **Tutela** 0.1.0 | Threat → invariant → control → evidence → posture model | Method fit, immature tooling |
| Authentication | **Fides** 0.2.0 | Exchange plus WASM client; **GitHub is the only IdP** (FID-D-002) | Poor for donors/sponsors |
| Data layer | **Arca** 0.7.0 | Authoritative records **in GitHub repositories**; WASM offline queue | Not fit for regulated ledger |
| Diagnostics | **Echelon Diagnostic Framework** (MIT) | Frozen v0.3 grammar; value not yet established | Research only |
| Knowledge | **Echelon Knowledge Store** | Research; no schema or packages | Not present (prior finding correct) |
| Enterprise design | **Enterprise Design Framework** (local, 763ff8c) | F# model, DAF case study, 87 tests; illustrative inputs | Analysis input |
| Other | Ordo 1.20.0 (state-legality method); Iter, Chrona, Vigila, Forma Studio (untagged or greenfield); Tekmerion 0.2.0; Vitium (proposed, unpublished) | Peripheral | No release-1 dependency |

## Architecture-assumption risks

1. **"F#/WASM via Limen"**: Limen is a TypeScript kernel with a proven F# seam, not an F# application framework. Unbound must build engines, auth, tenancy, a server and an embed runtime. 0.x protocol churn and the noncommercial licence apply.
2. **Embedded partner widgets**: neither Limen (whole-document binding) nor Forma (CSS only, no shadow packaging) supports embedding into third-party pages today.
3. **Forma version drift**: the registry channel (1.0.1), the vendor copy (d7c499e) and the release (1.1.0) differ. Repin to 1.1.0 and ask for a channel update before designing around the closures.
4. **Folio is markup, not a PDF system**: statement batches, determinism, tagged PDF/PDF-UA, PDF/A and signing are all Unbound-owned or need new tooling. Typst remains an open ADR.
5. **Strata mock interactions do not exist**: plan Unbound's own virtualization adapters, or co-develop STRATA-DIV.
6. **Identity**: Fides is GitHub-only, so a managed IdP is required (DG-05 stands).
7. **Licensing**: PolyForm Noncommercial covers almost the whole stack.
