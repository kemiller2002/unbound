# 13 — Architecture dependency analysis

Date: 2026-11-23 | Method: read-only inspection of shallow default-branch clones of each Echelon repository. **No component was built and no component test was executed by this mission.** "Verified" below means the capability is implemented in code with tests or a release artifact in that repository, not that this mission ran those tests.

## 1. Release authority

Versions resolve from the Echelon Registry channel `echelon-current` **1.39.0** (status `approved`; `kemiller2002/echelon-registry@fef1f94`), not from sibling repositories' lock files. Those lag: Strata's own resolved set pins Forma 0.4.1, Folio 0.3.0 and Dokimos 0.2.0.

| System | Registry version | Repo HEAD inspected | Maturity stated by the repository |
|---|---|---|---|
| Strata | 0.1.1 | `c6a384e` (0.1.1) | **Pilot**; PostgreSQL 16 only; CLI only, no application library |
| Forma | 1.0.1 | `d7c499e` (1.0.2) | 1.x semver on HTML/class/token contracts; 170 catalog components, 29 required but not implemented |
| Folio | 0.4.1 | `56a18e1` (0.4.2) | Print components established; `src/engine` and `src/kernel` empty |
| Limen | 0.9.1 (npm), limen-fsharp 0.9.0 | `743cdb9` | 0.x: protocol may change in a minor release |
| Dokimos | 0.11.1 | `6ea8624` | Released CLI |
| Praxis | 3.25.0 | `12bf995` | Released CLI / dotnet tool |
| Conditor | releases to 0.10.19 recorded | not inspected | Installation authority per AGENT-MISSION; adoption workflow for an existing requirements-only repository **unknown** |
| Knowledge Store | not in Registry | `35d9cee` | "Research phase. No stable schema, no released packages." |
| EDF | not in Registry | `710d54c` | v0.3 method spec; states its own incremental value is not established |
| Clarity | not in Registry | `200a9eb` | Prose framework, no tooling |

**Circular-evidence warning.** On 2026-11-23, Unbound-driven *proposals* were committed into Forma (`docs/requirements/email-designer-2026-11-23.md`, `unbound-dashboard-consumer-capability-review-2026-11-23.md`), Folio (`unbound-financial-report-consumer-capability-review-2026-11-23.md`) and Strata (`docs/requirements/data-interaction-virtualization.md`). Each states "proposed; no implementation or tests executed". They are evidence of **gaps**, never of capability.

## 2. Dependency verification matrix

| Dependency asserted by requirements | Claiming requirements | Verified status | Consequence |
|---|---|---|---|
| **Strata** provides append-only authoritative history, immutable posting, correction links, temporal queries, replay-safe projections and integrity checks | UNB-PLAT-007, UNB-ACC-026, UNB-KNOW-014, UNB-TEC-STRATA-003/004, UNB-CHAR-040, UNB-INV-042, UNB-CUST-013, UNB-NAV-051, KNOW-ADR-001, `EXPANSION-ROADMAP-AND-GATES.md` | **Not present.** Strata compiles `.sql`, diffs against a live database (six explicit comparison states), gates and deploys DDL, signs artifacts and detects drift. It has no data API. | **Category error (RCF-ARC-001, critical).** Re-scope Strata as the *schema governance and drift-control* layer. Assign event storage, posting, temporal queries and projections to an Unbound-owned **Ledger/Event Store component** on managed PostgreSQL, whose append-only rules (grants, triggers, RLS, CHECKs) Strata can declare and drift-check. Refinement: `RR-UNB-PLAT-007-r1`. |
| Strata supports schema-per-tenant, database-per-tenant and dedicated-infrastructure profiles | UNB-TEC-STRATA-002, PARTNER-SEGREGATION-AND-TENANCY, UNB-TEN-*, UNB-ACC-027 | **Partial.** RLS policies are modelled and drift-compared. Directory name = schema name, so there is no schema templating. The same artifact can deploy to multiple databases. | Supporting three isolation profiles at once multiplies migration, test and operations cost. Recommendation (RCD-004): tenant = legal sponsor, database per sponsor, partner schemas inside it (DG-11). See RCC-ARC-001. |
| **Forma** provides UI components | UNB-PLAT-002, ADM-036, UNB-ACC-024 | **Verified as HTML/CSS patterns plus tokens.** No component runtime; behaviour belongs to the application through Limen. | Unbound must build its own F# engine and state for every interactive screen. |
| Forma charts and accessible data visualisation | UNB-GFUND-025, UNB-INV-039, MON-023, assurance dashboards | **Not present** (P1/P2 unimplemented) | Executive, investment and assurance dashboards depend on a missing Forma capability. Dependency gap DG-01. |
| Forma configurable dashboards | REQ-X-049/050, CONTINUOUS-ASSURANCE-FORMA-WIDGETS-AND-FOLIO | **Partial** (fixed `dashboard-grid`, metric cards, work-queue) | Prefer exception-driven work surfaces over configurable dashboards ([10](10-UI-FORMA-FOLIO-GAP-ANALYSIS.md)). |
| Forma data tables | UNB-ACC-024, UNB-NAV-049 | **Partial.** Basic table styling, filter/sort builders, bulk-action bar, virtual list and editable grid are not implemented. | DG-02: required for ledger, grant history and statement screens. |
| Forma white-label branding | UNB-PLAT-009, REQ-X-030 | **Verified** (Brand Manifest compiler and brand-isolation tests) | Brand Kit requirements can rely on this. |
| Forma dynamic terminology | REQ-X-030, sponsor white-label terminology | **Not present by design** (BRAND-012: terminology is application content) | Unbound owns a terminology resolver (gap `UNB-RCN-PLT`). |
| Forma WCAG 2.2 AA | UNB-EXP-001, UNB-PLAT, UNB-GRANT-032 | **Verified (automated subset, on Forma fixtures only)** | Unbound screens still need their own axe checks plus assistive-technology testing. |
| Forma embedded widgets / web components | INT-015, widget requirements, REQ-X-063 | **Not present** generically. Only `<forma-workflow>` is a real custom element. | Embedded DAF checkout and widgets require an Unbound-built widget host, frame protocol and token model (DG-03). |
| Forma email designer | REQ-X-027..031 | **Planned only** (proposal authored for Unbound) | See RCF-PLT findings and decision RCD-008 (defer). |
| **Folio** document output (statements, acknowledgments, audit packages) | UNB-ACC-025, UNB-NAV-050, UNB-CHAR-039, UNB-GEX-027, UNB-PAY-009, REQ-X-032/033 | **Partial.** Light-DOM print elements; PDF via browser print; no PDF service or batch generator. | Monthly statement batch publication (REQ-X-032/040) has no verified renderer (DG-04). |
| Folio accessible PDF (PDF/UA), PDF/A, cryptographic signing, templating/versioning | UNB-ACC-025, UNB-DOC-003, UNB-GAP-023/032, REQ-X-033/034 | **Not present.** Folio's own requirement EPC-NG-008 disclaims PDF/UA from browser PDFs. `ef-print-integrity` is a data-quality block, not a cryptographic check. | Accessible, verifiable statements are unsupported. Artifact hashing and signing must be an Unbound document-registry concern (REQ-X-034), independent of renderer. |
| **Typst** statement rendering | REQ-X-033 | **Not present** in any Echelon repository | Introducing Typst is a new runtime dependency; it needs an ADR and a decision against Folio (RCC-ARC-002). |
| **Limen** WASM-first F# client | UNB-TEC-002, UNB-PLAT-006 | **Partial / seam verified.** Browser kernel and F# packages exist; Unbound writes its own engine. Typed `OutcomeUnknown` effect for timed-out requests is verified. | Fits the payment-unknown semantics (REQ-X-004). Limen has **no authentication, tenant token or server** capability (DG-05). |
| Limen offline / outbox / IndexedDB | INT-012, ADM-036 | **Verified as optional packs** | Offline drafting is feasible. Offline *consequential* commands must stay prohibited. |
| **Dokimos** executes acceptance scenarios | UNB-VAL-001, simulation framework, all `*-T*` cases, this mission's [12](12-DOKIMOS-ACCEPTANCE-SCENARIOS.md) | **Not present.** No Gherkin/BDD runner. Dokimos verifies applications against Praxis verification manifests using Playwright results (UI-field oriented) and records quality evidence. | Financial invariants, state machines and concurrency scenarios need an F# test framework (for example Expecto with FsCheck property tests). Dokimos records the evidence. Scenarios in [12] are written to be runner-neutral and map to `praxis.requirement-catalog/1` types (`state-transition`, `data-contract`, `workflow-outcome`). |
| **Praxis** validates the requirements repository | AGENT-MISSION step 9 | **Verified for Praxis-shaped records only** (front-matter records, `praxis.requirement-catalog/1` JSON). Unbound's Markdown bullets and CSVs are not in that format. | Until conversion, `tools/reqaudit/validate.py` provides the ID, reference and field checks. Converting the refined set into a Praxis requirement catalog is recommended (RCD-010). |
| **Knowledge Store** compiled requirement schema | CHARTER, AGENT-MISSION step 8 | **Not present** (research stage, prototype compiler) | Unbound correctly anticipated this. Machine-readable outputs here are JSONL/CSV with Markdown summaries, behind a compatibility boundary. |
| Knowledge Store jurisdiction rule engine | UNB-RULE-031, ADR-RULE-004 | **Not present** | Charity jurisdiction rules need an Unbound-owned typed rule model in F# (DG-06). |
| EDF, Clarity as validators | AGENT-MISSION | **Method only, no tooling** | Use their vocabulary (verified / assumed / confidence) in findings, not as gates. |
| Conditor bootstrap | AGENT-MISSION step 0 | **Not performed; no report exists** | Prerequisite still unmet (RCD-002). |

## 3. Architectural direction checks

| Direction | Status in requirements | Finding |
|---|---|---|
| F# primary language | Consistent (UNB-TEC-002, AGENT-MISSION) | Folio is JavaScript and Forma components are HTML/CSS. This is acceptable because they are consumed as assets, not application code. |
| WASM-first client | Stated "subject to verified EF compatibility" | Feasible only through .NET WebAssembly SDK plus Limen; Bolero and Fable are absent, and Fable is explicitly excluded by Forma. Bundle size and cold start on mobile are unmeasured (DG-07). |
| Component monolith, not microservices | REQ-X-020/021 are explicit | Several requirements still imply separate deployables or per-tenant infrastructure (UNB-TEN, UNB-TEC-STRATA-002, PARTNER-SEGREGATION). Conflict RCC-ARC-001. |
| No default Kubernetes or Terraform | No requirement mandates either | No conflict found. |
| Managed infrastructure | "Managed persistence" (UNB-TEC-002) | Database, queue and backup choices are unspecified. The DR requirements (REQ-X-047, protective emergency plane) add a second durable store outside the monolith, which needs explicit justification (RCF-ARC-004). |
| API-first, versioned contracts | UNB-API-STD-*, UNB-ACC-023 | No contract format chosen (OpenAPI, AsyncAPI, F# type-provider). `daf-landscape` holds draft OpenAPI files (`research/ai-native-daf-enterprise/04-enterprise-apis/openapi`). Gap `UNB-RCN-PLT`: contract source of truth. |
| Observable, recoverable workflows | Strong in REQ-X-019..022 | Outbox, idempotency and Unknown states are well specified. A durable workflow/timer mechanism for recurring grants and scheduled tasks is unassigned (DG-08). |

## 4. Dependency gaps to record (not to be fixed in other repositories)

| ID | Gap | Needed by | Owner of resolution |
|---|---|---|---|
| DG-01 | Accessible chart and visualisation primitives | Executive, investment and assurance views | Forma request, or Unbound-owned until delivered |
| DG-02 | Data table styling, sort/filter, bulk actions, virtualisation | Ledger, grant history, statements, work surfaces | Forma request |
| DG-03 | Embeddable widget host, frame/resize protocol, scoped tokens | REQ-X-063 checkout, partner widgets | Unbound (security-owned); Forma for presentation |
| DG-04 | Server-side deterministic PDF generation and batch publication | Monthly statements, acknowledgments, audit packages | Decision RCD-009 (Folio P2 tier vs alternative renderer) |
| DG-05 | Authentication, session, tenant-scoped tokens | Every portal and widget | Unbound (managed identity provider), not Limen |
| DG-06 | Typed jurisdiction/eligibility rule model | Charity eligibility, international grants | Unbound domain component |
| DG-07 | WASM bundle-size and cold-start budget on mobile | WASM-first decision | Unbound spike with measured evidence |
| DG-08 | Durable scheduler/timer for recurring grants, holds, revalidation | Recurring grants, revalidation, close | Unbound component inside the monolith |
| DG-09 | Event store / ledger persistence with append-only enforcement | All financial and authority history | Unbound Ledger component on managed PostgreSQL; Strata declares and drift-checks the schema |
| DG-10 | F# property and state-machine test harness; Dokimos evidence adapter | Acceptance scenarios in [12] | Unbound test project; Dokimos as evidence recorder |
| DG-11 | Per-partner schema generation for Strata projects (Strata maps directory name to schema name; no templating) | RCD-004 partner schemas inside each sponsor database | Unbound build tooling that generates Strata projects; Strata feature request only if justified |

None of these gaps were raised as changes in the Echelon repositories. They are recorded here only, as the mission requires.
