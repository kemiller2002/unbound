# Shared Dashboard Framework and Forma/Folio Capability Assessment
Date: 2026-11-23. Requirements and repository-documentation assessment only. No code changes, browser tests, PDF renderer benchmarks or implementation.

## Repository evidence reviewed
- kemiller2002/forma README.md: zero-component-runtime, semantic HTML/CSS/tokens, browser-native patterns; app behavior belongs to Limen; version-pinned consumption; portable workflow capability.
- kemiller2002/forma docs/COMPONENT-INVENTORY.md: generated inventory reports 170 public components, 18 families, 5 compositions including Operations dashboard, and 29 requirement-defined components not implemented. The inventory lists implemented freshness, empty-state, skeleton, navigation shell, filters and operation-status patterns, among others.
- kemiller2002/forma-studio README.md and REQUIREMENTS.md: versioned design documents, public Forma parity, responsive/mobile and accessibility validation, deterministic HTML export, explicit Folio integration when print/PDF authoring is introduced.
- kemiller2002/folio README.md and docs/requirements/PRINT-COMPONENTS-REQUIREMENTS.md: print component system, implemented ef-print-* family, tiered P0 browser/P1 Chromium margin-box/P2 deterministic Chromium/P3 enhanced paged-media renderer support, report fixture and table/metric/integrity/finding components; requirements distinguish intended capability from proven renderer behavior.
This was a selective documentation review, not a complete source-code audit. Treat readiness and integration claims as candidates pending executable consumer tests.

## Architectural recommendation
Unbound owns business semantics, metric definitions, widget permissions, dashboard state, drilldown targets, authoritative actions, source freshness and report issuance. Forma owns reusable visual patterns and accessibility contracts, not business logic or widget data access. Forma Studio owns authoring of supported Forma compositions, not runtime authority. Folio owns print layout and renderer capability contracts, not accounting/report truth. Limen/F# owns UI application behavior and WASM interaction. Typst remains a candidate behind a Folio-compatible document rendering adapter, not an assumed mandatory or proven equivalent renderer. Do not fork CSS or build a competing generic UI/print library inside Unbound.

## Shared dashboard requirements
- Role-based shells: platform executive, sponsor executive, controller/CFO, grant operations, charity vetting/compliance, work queue supervisor, donor/advisor, board/auditor.
- Versioned DashboardDefinition with DashboardId, owner/scope, role, widget instances, layout by breakpoint, metric definition versions, filter schema, access policy, freshness rules and migration history.
- WidgetDefinition with type/version, public Forma pattern dependencies, data contract, dimensions, accessible name, loading/empty/error/Unknown/stale states, safe drilldown and optional authorized action contract.
- Widget types: KPI metric, trend, reconciliation state, fund/AUM breakdown, grant funnel, charity 180-day coverage, exception aging, work queue, capacity heatmap, case timeline, statement batch, financial provenance, source freshness and audit evidence.
- Layout supports responsive grid, resize/reorder under guardrails, saved views, scoped shared views, safe defaults, versioned migrations, stable IDs and explicit empty/loading/degraded states. Mobile cannot simply hide mandatory safety information.
- Filter propagation must be typed and permission-aware; cross-widget sponsor/fund/time selection cannot broaden access. Cross-filtering cannot mutate financial truth.
- Actions: all domain changes require rechecked authoritative commands, sponsor scope, valid delegation, hold checks, optimistic concurrency and auditable outcomes. Read models and widgets cannot post ledger entries or release payments.
- Embedding: versioned externally consumable widget contract, origin restrictions, signed/scoped short-lived authorization, tenant isolation, accessibility, branding, safe resizing, CSP, event schema and rate limits. Distinguish read-only embed from privileged action embed; no silent third-party tracking.
- Performance: data-budgeted widgets, pagination/virtualization for large queues, cancellation, bounded refresh, source watermarks and deterministic error handling; define quantitative budgets by test rather than inventing them.
- Studio integration: use published Forma components and tokens; if a design cannot be represented, record a gap rather than invent Studio-private markup. Pin releases and validate migrations.

## Folio and report requirements
- Canonical ReportDefinition references approved semantic metrics, accounting snapshots, sponsor/fund scope, disclosure/template version, authorization, renderer capability, intended media, correction policy and evidence.
- Report families: financial statements, AUM and reconciliation, grant processing, charity vetting, work queue staffing/SLA, sponsor executive/board, compliance/audit, operational economics and prevention.
- Folio layout primitives for headings, running page headers, tables, notes, metrics, integrity/provenance, finding and document sections; long tables and page fragmentation must be tested at realistic extremes.
- Renderer matrix: P0 portable browser print, P1 margin-box Chromium, P2 controlled Chromium PDF, P3 enhanced paged media. Explicitly reject or downgrade unsupported features; no silent omission of financial rows, footnotes or disclaimers.
- Test determinism on exact renderer, fonts, assets, locale, timezone, pagination and print settings. Distinguish byte-level deterministic PDF from visual/semantic equivalence. PDF/A, tagged PDF accessibility, digital signatures and archival compliance are NOT assumed present and require proof or separate tooling.
- Use immutable as-issued PDF artifacts, final-byte SHA-256 and external signed manifest where approved, bitemporal as-issued/corrected snapshots, and seven-year financial retention plus legal holds. Do not place PDF's own hash inside itself.
- Report designer may use Forma Studio and Folio authoring only after proving faithful roundtrip, semantic data binding, accessibility, sponsor branding, safe preview and authorized publish. Design previews must never imply approved financial figures.
- Typst vs Folio/Chromium decision is ADR-gated by representative fixtures, print fidelity, accessibility, archival metadata, repeatability, F# integration and operational complexity. Folio's public print primitives are not proof Typst can consume them directly.

## Capability assessment
| Capability | Evidence | Decision |
|---|---|---|
| Common visual controls, navigation, work queue, empty/freshness states | Forma inventory lists implemented patterns | Reuse, verify pinned version and consumer fixture |
| Operations dashboard composition | Forma inventory lists composition | Reuse as starting pattern; dynamic user-authored layouts unverified |
| Dashboard widgets with typed data and permissions | Domain-specific requirement, not established by reviewed Forma docs | Build Unbound domain contracts on Forma presentation |
| Visual composition and responsive preview | Forma Studio requirements/README | Validate current implementation and version parity before relying on it |
| Portable workflow diagrams | Forma README documents format/renderer | Reuse where relevant; not substitute for finance workflow engine |
| Print layout, report metric and table components | Folio README | Reuse with real financial-report fixtures |
| Deterministic PDF output | Folio P2 capability tier | Validate renderer, exact output, fonts and hashes; not assume production-ready |
| Accessible archival PDF/PDF-A and digital signatures | Not verified in reviewed docs | Gap investigation and possible adapter/tooling |
| Typst direct interoperability with Folio HTML | Not established | Do not assume; comparative ADR |
| License suitability for commercial Unbound | Forma, Forma Studio and Folio READMEs specify PolyForm Noncommercial 1.0.0 with separate commercial license | Resolve licensing with rights holder before commercial deployment; do not assume MIT rights |

## Proposed shared capability follow-ups
**Forma:** validate dashboard composition/layout contract, responsive widget grid, data-grid virtualization, cross-filter visualization, accessible financial chart patterns, work queue/approval surfaces, embedding/iframe sizing and source freshness/Unknown states; add only missing generalizable primitives.
**Forma Studio:** dashboard page/template authoring, breakpoint preview, widget slot contract, version-pinned public component sync, permissions simulation, publish validation, migration diff and report-authoring integration as a separate gated scope.
**Folio:** long multipage financial tables, running headers and disclosures, footnotes and dense legal text, accessible reading order, output reproducibility, print QA and archival metadata; explicitly map renderer tiers and adapter behavior.
**Unbound:** authoritative semantic metric registry, dashboard definition and widget contracts, secure embed broker, report definition/approval, immutable artifact issuance, data scope, tenant authorization and audit.

## Proposed adversarial scenarios (not executed)
- ADV-687: widget shows stale AUM without watermark; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-688: dragged layout hides material compliance hold; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-689: saved filter crosses sponsor scope; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-690: embed token reused on unauthorized origin; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-691: iframe exposes donor data through postMessage; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-692: chart loses meaning in grayscale; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-693: screen reader cannot navigate dense dashboard; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-694: mobile layout hides approval state; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-695: dashboard action relies on stale permission; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-696: user-custom widget executes arbitrary code; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-697: widget failure blanks entire dashboard; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-698: cross-filter mixes known-at and effective-at; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-699: metric definition changed without migration; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-700: published Studio design relies on private CSS; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-701: Studio component version differs from deployed Forma; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-702: dashboard export leaks hidden restricted column; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-703: long grant table clips rows in PDF; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-704: running footer absent in non-Chromium browser; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-705: print disclaimer orphaned from related figures; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-706: PDF changes because of unpinned fonts; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-707: PDF hash computed before final render; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-708: Typst assumed to render Folio custom elements; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-709: report preview mistaken for approved statement; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-710: report regeneration overwrites as-issued original; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-711: financial footnote silently omitted by renderer; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-712: PDF/A claimed without conformance evidence; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-713: accessible PDF claimed without tagged-PDF test; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-714: report signature represented by decorative icon; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-715: commercial use assumed allowed by noncommercial license; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-716: widget embed bypasses sponsor white-label disclosures; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-717: report timezone causes wrong period cutoff; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-718: browser print header overlays legal text; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-719: source evidence mismatch hidden by successful render; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-720: folio component version floats across reports; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-721: chart PDF uses color as only risk indicator; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.
- ADV-722: custom layout enables misleading juxtaposition of financial figures; expected: governed semantics, accessibility, source provenance, safe permissions and verified rendering.

## Acceptance and next steps
1. Create a canonical dashboard/widget schema and role-based information architecture in Unbound.
2. Create representative consumer fixtures for Forma and Folio, with actual tests in implementation phase.
3. Log gaps against owning repositories; do not create duplicate Unbound presentation libraries.
4. Record a Typst/Folio renderer ADR with explicit unsupported features and evidence.
5. Confirm commercial license terms before deployment.
6. Proceed to detailed dashboards, embedded widgets, reports and report designer UX after requirements validation.
Status: proposed. ADV-687..ADV-722 not executed.
