# 06 — Experience, dashboard, report, widget and AI-assistance specifications (Phase 5)

**Provenance.** Drafted by a delegated design pass that read the existing UI corpus (screens S-001..S-061, decision
panels P-001..P-020, v2 designs, metrics, surface inventory) and reused existing IDs wherever they exist; integrated
here unchanged except for the decisions below. Machine-readable catalogs (each line a complete specification with
purpose, roles, permissions, hierarchy, data sources, actions, next best action, empty/loading/error/exception states,
accessibility, mobile behaviour, audit consequences, AI assistance, components and Given/When/Then acceptance criteria):

| Catalog | File | Count |
|---|---|---|
| Screens (19 user groups incl. AI agents as API consumers) | [`data/experience/screens.jsonl`](data/experience/screens.jsonl) | 140 (81 existing IDs, 59 new) |
| Dashboards and metrics (business question, calculation, source, freshness, owner, threshold method, counter-metric, drill-down, action, privacy, manipulation risk) | [`data/experience/dashboards.jsonl`](data/experience/dashboards.jsonl) | 12 dashboards, 112 metrics (70 existing, 42 new); no composite scores |
| Reports (snapshot rule, versioning, access, distribution, accessible HTML and PDF, retention, generator) | [`data/experience/reports.jsonl`](data/experience/reports.jsonl) | 60 (6 existing, 54 new) |
| Embeddable widgets (auth, authorization, branding, host integration, data boundaries, accessibility, errors, versioning, events, AI docs) | [`data/experience/widgets.jsonl`](data/experience/widgets.jsonl) | 12 (grant recommendation and checkout embeds deferred) |

These specifications are requirements, not designs validated with people: no manual screen-reader or usability
testing has been done, and the 59 new screens and 42 new metrics need prototypes, state-matrix rows and metric-registry
entries before they count as acceptance evidence.

## Decisions applied from the converged baseline

The draft listed owner decisions it did not hard-code. The converged baseline (report 03) settles them as follows.

| Open item in draft | Resolution | Clause |
|---|---|---|
| Whether AI may execute in release 1 | Recommend and prepare only; any execute contract needs a separate owner decision and never for reserved (A3+) authority | CNV-AIO-001 |
| Reconsideration window (30, 60 or 90 days) | 60-day filing, 30-day response default, configurable with mandatory exceptions | CNV-GRT-012 |
| Statement cadence | Generate every configured period; quarterly digest notification default; monthly opt-in; program terms override | CNV-PLT-005 |
| Widget deprecation window | Widgets are public APIs: 18 months' notice for breaking changes with emergency exceptions | CNV-EXP-001 |
| Minimum cell size for partner and cross-sponsor aggregates | Experimental minimum five sponsors and at most 60% dominance plus differencing tests; not an anonymity guarantee | CNV-ECO-001 |
| Accessible PDF unproven | Accessible HTML is the primary artifact; PDF is Unbound-owned and gated on veraPDF/PAC verification | CNV-EXP-001, report 07 |
| Charts (FORMA-GAP-05) | Release 1 dashboards render as accessible tables; charts follow when an accessible chart component exists | report 07 |
| IVR reference length and check digit | Remains a product decision; recorded in the evidence backlog with a usability test (report 10) | — |
| Form 990 mapping beyond Schedule D Part I | Research task from the IRS instructions (no paid review); Schedule D Part I governed by CNV-FIN-009 | report 10 backlog |

## A. Deliverables

| File | Contents | Count |
|---|---|---|
| `screens.jsonl` | 81 existing screens and panels (UNB-UI-S-001..S-061, UNB-UI-P-001..P-020), enriched with every Phase 5 field, plus 59 new screens (NEW-S-001..059) | 140 |
| `dashboards.jsonl` | 12 dashboard header records, plus 70 kept registry metrics (SPM-*) and 42 new metrics (NEW-M-001..042) | 112 metrics |
| `reports.jsonl` | 6 existing documents (UNB-UI-D-001..006) and 54 new reports (NEW-R-001..054) | 60 |
| `widgets.jsonl` | UNB-UI-W-001 and NEW-W-002..012 | 12 |
| `ai-assistance.md` | Assistance contract JSON Schema (draft 2020-12), template-language requirements, 16 controls, 13 acceptance criteria | — |

Every JSONL file was parsed line by line. The JSON Schema was checked with `jsonschema` (Draft 2020-12). Its worked example validates. The same example with `execute` set at A4 fails validation.

ID conventions:
- Existing IDs are kept.
- New items are prefixed `NEW-`.
- In the table below, `S-` means `UNB-UI-S-`, `P-` means `UNB-UI-P-` and `N-` means `NEW-S-`.

## A. Coverage: user group x workflow

| User group | Access and onboarding | Contributions | Grants | Documents and statements | Exceptions and decisions | Finance and close | Oversight and reporting | Configuration | AI help |
|---|---|---|---|---|---|---|---|---|---|
| Donors | N-001, N-002, S-030, S-015 | S-005, N-007 | S-001..S-004, S-026, N-006, N-014 | S-007, N-005 | S-016, S-033, S-029 | — | N-008 | S-013, N-009, N-045 | N-003 |
| Co-advisors | S-027, N-001 | S-005 | S-003, N-010 | S-007 | S-029, S-033 | — | N-008 | S-006 | N-003 |
| Successors | N-011, S-028 | — | S-026 (resume), S-001 | S-007, N-007 | S-033 | — | — | S-006 | N-003 |
| Financial advisors | S-027 | S-005 | N-013, S-003 (prepare), N-014 | S-007, N-015 | — | — | N-013 | — | N-003 |
| Charities | N-016 | — | S-012, N-017, S-061 | NEW-R-009, D-003 | S-016, S-033 | — | — | S-013 | N-003 |
| Sponsor administrators | N-018, S-023, S-052 | — | — | N-025, N-026 | S-008 | — | S-011 | S-010, N-019..N-024, N-027 | N-003 |
| Grant operations | — | — | N-029, N-030, S-017, S-034 | — | S-008, S-014, P-001, N-059 | — | Dashboard: grant-operations | — | N-003 |
| Charity vetting | N-016 (verification) | — | S-002 (registry) | — | P-002..P-004, P-013, P-017, P-020, N-031, N-032 | — | Dashboard: charity-vetting | — | N-003 |
| Customer service | N-033 | — | N-028, N-030 | N-022 | S-025, S-060, N-034 | — | Dashboard: customer-service | — | N-003 |
| Treasury | — | N-038 | S-018, S-037, S-035, P-018 | — | P-005, P-009, P-011 | S-043, N-036, N-037 | Dashboard: treasury | — | N-003 |
| Accountants | — | N-041 | — | — | P-012 | S-019, S-042, S-044, N-039..N-042 | Dashboard: financial | — | N-003 |
| Controllers | S-051 | — | — | N-025 (statement batches) | S-038 | S-041, S-046, S-020 | N-046, NEW-R-014 | — | N-003 |
| Compliance officers | — | P-008, S-039, S-040 | P-014, P-015, P-019 | S-021, S-047, S-048 | S-036, N-043, N-044 | — | Dashboard: compliance | N-045 | N-003 |
| Executives | — | — | — | — | — | — | S-011, N-046, N-029 | — | N-003 |
| Board members | — | — | — | NEW-R-047 | — | — | N-047, N-048, S-050 | — | N-003 (summaries) |
| Auditors | — | — | — | N-005 | D-005 | N-039 (read-only) | N-049, D-006 | — | — |
| Distribution partners | N-051 | — | W-004/W-009 (deferred) | W-006 | — | — | N-050 | N-027 | — |
| Platform operators | S-053..S-055, S-023 | — | — | — | P-010 | — | N-052, N-055 | N-053, N-054, N-056 | — |
| AI agents (API) | N-057 | — | N-058 (proposals only) | — | N-059 (human review) | — | — | — | contracts |

## A. Design principles

1. **Answer the three questions first.** Each screen states what the user can do now, what is in flight and who acts next. Customer homes keep the authoritative figure and "Needs your attention" in the first viewport (UNB-UI-N-006).
2. **Authoritative state only.** Statuses come from domain state machines. "Unknown" is a first-class state and no success label appears before evidence exists (RR-UNB-UX-003-r1, UNB-UI-G-001/G-006). Totals become "Not determinable" when any component is stale (UNB-UI-S-011).
3. **Exception-driven work.** Staff work from one rule-created queue (UNB-UI-S-008) and one typed case frame (UNB-UI-S-014). Dashboards link to the queue rather than duplicating it (RR-REQ-X-048-r1).
4. **Authority is visible, not inferred.** Unavailable actions stay visible with the reason and the role that can act (FORMA-GAP-11). The preparer is never the decider. A3+ decisions belong only to the owning sponsor's decision team (UNB-SMP-ORG-003). Unbound remains software only.
5. **AI explains and prepares; people and deterministic rules decide.** AI output is labelled and cited and has no consequential controls (UNB-UI-G-011). Execute is limited to reversible A0..A2 commands (ai-assistance.md UX-AI-002).
6. **One definition per figure.** Dashboard, export and print values come from the Metric Registry and carry metric id@version and snapshot or watermark. Parity failures block publication (RR-REQ-X-043-r1).
7. **No composite scores.** Every metric names its decision, owner, counter-metric and manipulation risk. The 14 removed metrics stay removed, including SPM-005 (composite score), SPM-029 (staff ranking), SPM-054/SPM-080 (asset rankings), SPM-076 (impact score) and SPM-077 (badges).
8. **Thresholds are not invented.** Where there is evidence (RCD-017 buffer floor, RCD-028 stale checks, RCD-006 180-day refresh, RCD-046 sanctions SLA, REQ-X-036 7-year retention), the threshold is cited. Otherwise the metric uses a baseline-then-policy method.
9. **Accessible by construction and testable.** Every screen has an axe-core WCAG 2.2 AA criterion at 320 and 1280 px, plus keyboard, focus, target-size and reflow rules. Every issued document has an accessible HTML twin from the same snapshot (UNB-RCN-PLT-010).
10. **Plain language.** No user-facing copy uses internal terms. A scan of the quoted copy in `screens.jsonl` for terms such as WorkItem, read model, idempotency, snapshot and tenant found none.

## A. Notable decisions

- **Panels are screens.** The 20 typed decision panels (P-001..P-020) are listed as screens because compliance, treasury and vetting staff spend their decision time there. They share the S-014 frame.
- **Release-1 embeds stay read-only.** The grant recommendation and checkout widgets (NEW-W-004, NEW-W-009) are fully specified but marked deferred (DG-03, CAP-03). Hosts receive stage events only, never balances.
- **Dashboards are fixed, role-composed views (RCD-026)** with at most one accessible trend per view until FORMA-GAP-05 closes. There is no user-configurable dashboard authoring in release 1.
- **Advisors see no asset aggregates.** The advisor client book and widget show per-client figures with no cross-client totals (UNB-XIN-CRM-006).
- **The IVR is read-only status with secure hand-off** (REQ-X-025). Numeric references are strings so leading zeros survive, and failed lookups get neutral responses.
- **Release 1 ships recommend/prepare only** (recommended). Contracts can declare execute for A0..A2 reversible commands, but whether to enable that is an owner decision.
- **Schedule D Part I** is supported by D-004. Support data for other parts of Form 990 (NEW-R-019) is a candidate whose schedule mapping must be confirmed by the sponsor's CPA and counsel.

## A. Required reusable components and vendored fit

Paths are relative to `/home/user/unbound/requirements/ui/designs/vendor/`. Forma presence was checked against class names in `forma-1.0.2/all.css`. Folio presence was checked against the element list in `folio-0.4.2/src/components/register.js`. Gap evidence is in `../screens/FORMA-FIT-REPORT.md`, `../documents/FOLIO-FIT-REPORT.md` and `reconciliation/2026-11-23/data/ui-component-gaps.csv`.

| Component | Provided? | Evidence / gap |
|---|---|---|
| Application shell with legal-note footer | No | Only marketing `.forma-site`/`site-footer` and `workspace-shell` in `forma-1.0.2/all.css`; FORMA-GAP-06 |
| Status chip (sentence case) | Partial | `.forma-status-lozenge` exists (uppercase mono); FORMA-GAP-21; interim `.unb-chip` in `../v2/unbound-app.css` |
| Multi-dimension status track | No | `.forma-steps`, `.forma-wizard` only; FORMA-GAP-09 |
| Money/numeric value presentation | No | Tabular numerals only in `identifier`/`dense-ledger`; FORMA-GAP-08 |
| Unavailable action with reason | No | FORMA-GAP-11 |
| Task list ("Needs your attention") | No | `.forma-work-queue` targets staff; FORMA-GAP-22; interim `.unb-task` |
| Phone confirmation dialog (pinned header/actions) | Partial | `.forma-dialog`; FORMA-GAP-14 |
| AI output panel with provenance, no actions | Partial | `flyout`, `conversation`, `turn`, `composer`, `handoff-summary`, `provenance-trail` exist; FORMA-GAP-16, FORMA-GAP-17 |
| Accessible chart with data table | No | FORMA-GAP-05 (diagram family is not charts) |
| Approval quorum tracker | No | FORMA-GAP-24 |
| Accounting period context bar | No | FORMA-GAP-25 |
| Per-item batch outcome summary | No | FORMA-GAP-26; bulk bar FORMA-GAP-04 |
| Copyable identifier | Partial | `.forma-identifier`; FORMA-GAP-07 |
| Read-only allocation summary | No | `.forma-allocation` is input-only; FORMA-GAP-12 |
| Data grid, toolbar, filters, pagination, dense ledger | Yes | `data-grid`, `collection-toolbar`, `active-filter-summary`, `pagination`, `dense-ledger` |
| Freshness, evidence age, provenance trail | Yes | `freshness`, `evidence-age`, `provenance-trail` |
| Rule builder, diff viewer, readiness checklist, conflict review | Yes | present in `all.css` (no dedicated impact-preview primitive) |
| Wizard, forms, validation summary/message, file upload | Yes | present; FORMA-GAP-02 contrast proof owed |
| Container-query reflow in panes | No | FORMA-GAP-15 |
| Application type scale and density; breakpoint tokens | No | FORMA-GAP-18, FORMA-GAP-19 |
| Domain icons (gift, bank, charity) | No | `forma-1.0.2/icons-registry.json` (80 icons); FORMA-GAP-23 |
| Template token explorer and diagnostics list | No (NEW) | Not in vendored Forma; Forma email designer is upstream scope (REQ-X-031) |
| Colour-contrast checker panel for brand kit | No (NEW) | — |
| Widget host, frame protocol, scoped tokens | No (Unbound-owned) | UNB-GAP-W-01 |
| Typed decision-panel registry | No (Unbound-owned) | UNB-GAP-W-02 |
| Print document, sections, tables, metrics, integrity, TOC, findings, security evidence | Yes | `ef-print-*` in `folio-0.4.2/src/components/register.js` |
| Page n of N; running header/footer | No | FOLIO-GAP-04, FOLIO-GAP-05 (render-time margin-box workaround measured 7/7) |
| Unbroken identifiers/URLs; totals printed once; keep-with-next; TOC page numbers | No | FOLIO-GAP-06, -08, -09, -10 |
| Letter primitives and window-envelope address | No | FOLIO-GAP-01, FOLIO-GAP-07 |
| Verification/document identity block | No | FOLIO-GAP-02 |
| Signature/approval block | No | FOLIO-GAP-12 |
| Tagged PDF/PDF-UA in export contract | No | FOLIO-GAP-11 (`tagged:true` gives Tagged: yes, unvalidated) |
| Deterministic server-side P2 rendering and batches | Partial | FOLIO-GAP-03; per-page text hash stable 7/7, raw bytes vary by date stamp |

## A. Open issues

1. **Accessible output.** PDF/UA is unproven (FOLIO-GAP-11), so the HTML twin is the accessible rendition until veraPDF/PAC validation passes. Running headers and page numbers depend on a render-time workaround (FOLIO-GAP-04/05).
2. **Charts.** With FORMA-GAP-05 open, economics, performance and board trends ship as tables.
3. **Commercial licence.** Forma and Folio are licensed PolyForm Noncommercial 1.0.0. This is a commercial-launch blocker (RCD-027), not a design blocker.
4. **Unvalidated policy values.** Several values in sources are assumptions ([A]) or conflict between sources:
   - reconsideration window: 30, 60 or 90 days (SPF-CXP-004);
   - statement cadence (SPF-CFG-003);
   - widget deprecation window;
   - minimum cell size for partner aggregates;
   - IVR reference length and check digit.

   All are owner decisions, and none is hard-coded here.
5. **Human testing.** No manual screen-reader testing and no donor or staff usability testing has been done. The automated checks are by agents of the same model family (report 18 §1). DKS-UI-018 and INT-020 remain required.
6. **New screens need prototypes.** The 59 new screens and 42 new metrics need v2 static prototypes, state-matrix rows (`ui-state-matrix.csv`) and Metric Registry entries before they can be cited as acceptance evidence (UNB-RCN-PLT-009).
7. **Execute level for AI.** Whether any `execute` contracts are enabled in release 1 is undecided. Recommend/prepare only is recommended.
8. **Form 990 mapping.** Mappings beyond Schedule D Part I need CPA and counsel confirmation (NEW-R-019).
9. **Operational retention.** The retention period for operational reports is not stated in sources. The retention catalog owner must set it (REQ-X-037).

## B. AI assistance documentation, template language and controls

Status: proposed. Requirements only, no implementation. This document **extends** the adopted AssistanceContract requirement RR-REQ-X-051-r1 (refines REQ-X-051, decision RCD-025) and RR-REQ-X-051-r2, and the research record `research/daf-theoretical-enterprise-validation/19-questions/AI-DISCOVERABLE-CONTRACTS-AND-SAFE-ASSISTANCE-2026-11-23.md`. It does not replace any of their required fields. New requirement IDs below are marked **NEW**.

## B. 1. Scope

Every one of the following MUST publish a machine-readable assistance contract before it is called "assistant-ready" (RR-REQ-X-051-r1):

| Kind | Examples | Owner |
|---|---|---|
| `apiOperation` | Submit grant recommendation, Query payment status | Owning component (one monolith, REQ-X-057) |
| `configProperty` | Pool liquidity buffer floor, recurring grant cadence limits, reconsideration window | Configuration register item (UNB-SMP-CFG-003) |
| `workflowAction` | Approve selected grants (WFM-08-T02), Dispose as false positive (WFM-12-T06) | Workflow owner |
| `uiField` | Grant amount, recognition election, successor share | Screen owner (screens.jsonl) |
| `templateToken`, `templateFormatter`, `templateCondition`, `templateCollection`, `templateFragment`, `templateBlock` | `grant.amount`, `currency`, `grant.status == Approved`, `statement.items`, `sponsor.requiredDisclosure` | Communications (REQ-X-027/028/052) |
| `widget` | Grant status card (UNB-UI-W-001), NEW-W-002..012 | Widget owner (widgets.jsonl) |
| `metric` | SPM-001..084 kept metrics, NEW-M-001..042 | Metric Registry (RR-REQ-X-043-r1) |
| `reportDefinition` | UNB-UI-D-001..006, NEW-R-001..054 | Report owner (reports.jsonl) |

"Every input" includes derived and accessibility-related fields (e.g. a reduced-motion preference), but documenting a field never means an AI may edit it.

## B. 2. Required documentation fields

The fields the mission requires map onto the contract as follows. Fields marked *051-r1* are already required by RR-REQ-X-051-r1; the others are **NEW** required fields (UX-AI-001).

| Mission field | Contract field | Content rule |
|---|---|---|
| purpose | `purpose` (051-r1) | Plain language, at most 280 characters, no internal architecture terms |
| expected values | `expectedValues` | Type, unit, currency, precision, allowed values or range, default and **where the default comes from** (pack id@version, derived rule, or none). Unknown values are rejected, never coerced |
| constraints | `constraints` + `businessConstraints` (051-r1) | Policy IDs and versions that can block; platform-safety floors and ceilings |
| examples | `examples` (051-r1, at least one request, response and error) | Synthetic data only; marked `synthetic: true`; no live personal data, no secrets |
| dependencies | `dependencies` | Other fields/contracts it requires, conflicts with or changes; cycle-free (validated) |
| validation rules | `validationRules` (051-r1) | Machine-readable (JSON Schema keywords plus policy references) generated from the input schema; the same rules drive UI validation |
| common mistakes | `commonMistakes` | Each with symptom, correct approach and the error code it produces |
| financial implications | `financialImplications` | What money, capacity, fee, valuation or ledger effect follows; `none` must be stated explicitly |
| regulatory implications | `regulatoryImplications` | Which obligations it touches (e.g. acknowledgment content, sanctions, Schedule D, unclaimed property) and the reviewed source; `none` stated explicitly; never a legal determination |
| required authority | `requiredAuthority` = `authorityClass` (051-r1, A0..A5) + `permittedActorTypes` (051-r1) + `humanApprovalBoundary` (051-r1) + `separationOfDuties` | Authority matrix UNB-RCN-PLT-005; decision team rule UNB-SMP-ORG-003 |
| agent permission level | `agentPermissionLevel`: `recommend` \| `prepare` \| `execute` | Ceiling for any AI caller (section 3) |

Also required (051-r1): `contractId`, `version`, `ownerComponent`, `kind`, `inputSchemaRef`, `outputSchemaRef`, `sideEffects`, `confirmationRequired`, `reversibility`, `unknownStates`, `errors`, `visibility`, `safeActions`, `prohibitedActions`, `outcomeExplanation`, plus **NEW**: `sourceReferences`, `freshness`, `sensitivity`, `localizationKeys`, `deprecation`, `schemaHash`, `editorialReview`.

## B. 3. Agent permission levels

| Level | Maps to (research L0..L3) | What an AI may do | Hard limits |
|---|---|---|---|
| `recommend` | L0 Explain, L1 Guide | Explain, compare documented options, point to the field or screen, say "I cannot determine" | No drafts in authoritative stores |
| `prepare` | L2 Prepare | Create a draft, proposal or configuration diff in the caller's draft space, with provenance, risk and a synthetic preview | A human with authority reviews; the draft is a Proposal, Finding or Explanation (RR-UNB-AI-STD-003-r1) and is never input to a gate except as attached evidence |
| `execute` | L3 Execute | Call the typed domain command for the user, with the user's consent at that moment and a fresh state check | Allowed only when **all** hold: `authorityClass` is A0..A2, `reversibility` is `reversible` or `compensatable`, `sideEffects` excludes `externalEffect` money movement, the contract lists the command in `safeActions`, and the calling agent's registered manifest (AIN-002) includes it |

**UX-AI-002 (NEW).** The following are `agentPermissionLevel: recommend` or `prepare` and MUST NOT be `execute` for any caller: money movement and payment release; grant approval, decline or reservation; sanctions disposition or release; hold placement or release; eligibility determination; ledger posting, correction or write-off; NAV or price approval; policy, template, brand or routing publication; access grants and delegation; legal holds and disposition; succession activation; closing or resolving any alert, case or finding the agent raised or that concerns its own action (RR-UNB-AI-STD-003-r1). The contract validator rejects a contract that sets `execute` on any of these.

**UX-AI-003 (NEW).** A documentation entry never grants a permission. The effective ceiling is the minimum of: contract `agentPermissionLevel`, the agent's registered manifest, the user's own authority for the command, and sponsor policy. All four are re-evaluated at execution time.

## B. 4. JSON Schema (draft 2020-12)

```json
{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "$id": "urn:unbound:schema:assistance-contract:2.0.0",
  "title": "Unbound AssistanceContract (Phase 5 extension of RR-REQ-X-051-r1)",
  "type": "object",
  "additionalProperties": false,
  "required": ["contractId","version","ownerComponent","kind","purpose","inputSchemaRef","outputSchemaRef","expectedValues","constraints","businessConstraints","validationRules","dependencies","examples","commonMistakes","financialImplications","regulatoryImplications","requiredAuthority","agentPermissionLevel","sideEffects","confirmationRequired","reversibility","unknownStates","errors","visibility","safeActions","prohibitedActions","outcomeExplanation","sourceReferences","sensitivity","schemaHash","editorialReview"],
  "properties": {
    "contractId": {"type": "string", "pattern": "^urn:unbound:contract:[a-z0-9-]+(\\.[a-z0-9-]+)*$"},
    "version": {"type": "string", "pattern": "^(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)$"},
    "ownerComponent": {"type": "string", "minLength": 1},
    "kind": {"enum": ["apiOperation","configProperty","workflowAction","uiField","templateToken","templateFormatter","templateCondition","templateCollection","templateFragment","templateBlock","widget","metric","reportDefinition"]},
    "title": {"type": "string", "maxLength": 80},
    "purpose": {"type": "string", "maxLength": 280},
    "inputSchemaRef": {"type": ["string","null"], "format": "uri-reference"},
    "outputSchemaRef": {"type": ["string","null"], "format": "uri-reference"},
    "expectedValues": {
      "type": "object",
      "required": ["semanticType","defaultSource"],
      "properties": {
        "semanticType": {"enum": ["money","percentage","date","dateTime","duration","count","identifier","numericReference","enum","boolean","text","email","url","ein","reference","composite"]},
        "unit": {"type": "string"},
        "currency": {"type": "string", "pattern": "^[A-Z]{3}$"},
        "precision": {"type": "integer", "minimum": 0},
        "allowedValues": {"type": "array", "items": {"type": "object", "required": ["value","meaning"], "properties": {"value": {}, "meaning": {"type": "string"}, "labelKey": {"type": "string"}}}},
        "range": {"type": "object", "properties": {"min": {}, "max": {}, "exclusiveMin": {}, "exclusiveMax": {}}},
        "preserveLeadingZeros": {"type": "boolean"},
        "default": {},
        "defaultSource": {"type": "object", "required": ["type"], "properties": {"type": {"enum": ["policyPack","derivedRule","sponsorSetting","platformConstant","none"]}, "ref": {"type": "string"}}},
        "unknownValueHandling": {"const": "reject"}
      }
    },
    "constraints": {"type": "array", "items": {"type": "object", "required": ["description"], "properties": {"description": {"type": "string"}, "platformSafetyFloor": {}, "platformSafetyCeiling": {}, "policyRef": {"type": "string"}}}},
    "businessConstraints": {"type": "array", "items": {"type": "object", "required": ["policyId","policyVersion"], "properties": {"policyId": {"type": "string"}, "policyVersion": {"type": "string"}, "effect": {"enum": ["blocks","warns","requiresApproval"]}}}},
    "validationRules": {"type": "array", "minItems": 1, "items": {"type": "object", "required": ["ruleId","expression","errorCode"], "properties": {"ruleId": {"type": "string"}, "expression": {"type": "object", "description": "JSON Schema keywords or a typed predicate reference; no free code"}, "errorCode": {"type": "string", "pattern": "^[A-Z][A-Z0-9_]+$"}, "policyRef": {"type": "string"}}}},
    "dependencies": {"type": "array", "items": {"type": "object", "required": ["contractId","relation"], "properties": {"contractId": {"type": "string"}, "relation": {"enum": ["requires","conflictsWith","changes","derivedFrom","visibleWhen","requiredWhen"]}, "condition": {"type": "string"}}}},
    "examples": {"type": "array", "minItems": 1, "items": {"type": "object", "required": ["name","synthetic"], "properties": {"name": {"type": "string"}, "synthetic": {"const": true}, "request": {}, "response": {}, "error": {}, "explanation": {"type": "string"}}}},
    "commonMistakes": {"type": "array", "items": {"type": "object", "required": ["mistake","correction"], "properties": {"mistake": {"type": "string"}, "symptom": {"type": "string"}, "correction": {"type": "string"}, "errorCode": {"type": "string"}}}},
    "financialImplications": {"type": "object", "required": ["summary","affects"], "properties": {"summary": {"type": "string"}, "affects": {"type": "array", "items": {"enum": ["none","availableToGrant","reservation","ledgerPosting","fee","valuation","cash","payment","statementFigure","costToServe"]}}, "ledgerAccountsTouched": {"type": "array", "items": {"type": "string"}}}},
    "regulatoryImplications": {"type": "object", "required": ["summary","obligations"], "properties": {"summary": {"type": "string"}, "obligations": {"type": "array", "items": {"type": "string"}}, "reviewedSource": {"type": "string"}, "counselReviewed": {"type": "boolean"}}},
    "requiredAuthority": {
      "type": "object",
      "required": ["authorityClass","permittedActorTypes","humanApprovalBoundary","separationOfDuties"],
      "properties": {
        "authorityClass": {"enum": ["A0","A1","A2","A3","A4","A5"]},
        "permittedActorTypes": {"type": "array", "items": {"enum": ["deterministicPolicy","aiAgent","humanRole"]}},
        "humanRoles": {"type": "array", "items": {"type": "string"}},
        "humanApprovalBoundary": {"type": "string"},
        "separationOfDuties": {"type": "array", "items": {"enum": ["preparerNotDecider","requesterNotApprover","authorNotPublisher","notParticipantOrRelatedParty","twoPerson","decisionTeamOnly"]}},
        "delegationSource": {"type": "string", "description": "Board delegation or DoA entry reference"}
      }
    },
    "agentPermissionLevel": {"enum": ["recommend","prepare","execute"]},
    "sideEffects": {"type": "array", "minItems": 1, "items": {"enum": ["none","createsDraft","changesConfig","changesAuthoritativeState","externalEffect"]}},
    "confirmationRequired": {"type": "boolean"},
    "reversibility": {"enum": ["reversible","compensatable","irreversible"]},
    "idempotency": {"type": "object", "properties": {"keyRequired": {"type": "boolean"}, "unknownOutcomeQuery": {"type": "string"}}},
    "unknownStates": {"type": "array", "items": {"type": "string"}},
    "errors": {"type": "array", "items": {"type": "object", "required": ["code","httpStatus"], "properties": {"code": {"type": "string"}, "httpStatus": {"type": "integer"}, "retryable": {"type": "boolean"}}}},
    "outcomeExplanation": {"type": "array", "minItems": 1, "items": {"type": "object", "required": ["result","plainLanguage","evidenceFields"], "properties": {"result": {"type": "string"}, "plainLanguage": {"type": "string"}, "evidenceFields": {"type": "array", "items": {"type": "string"}}}}},
    "visibility": {"enum": ["public","tenantScoped","roleScoped","restricted"]},
    "safeActions": {"type": "array", "items": {"type": "string"}},
    "prohibitedActions": {"type": "array", "minItems": 1, "items": {"type": "string"}},
    "sourceReferences": {"type": "array", "minItems": 1, "items": {"type": "string"}},
    "freshness": {"type": "object", "properties": {"limit": {"type": "string", "format": "duration"}, "staleBehavior": {"type": "string"}}},
    "sensitivity": {"enum": ["public","internal","confidential","restricted"]},
    "localizationKeys": {"type": "array", "items": {"type": "string"}},
    "accessibilityNotes": {"type": "string"},
    "deprecation": {"type": ["object","null"], "properties": {"deprecatedIn": {"type": "string"}, "removalAfter": {"type": "string"}, "replacement": {"type": "string"}}},
    "schemaHash": {"type": "string", "pattern": "^sha256:[0-9a-f]{64}$"},
    "editorialReview": {"type": "object", "required": ["reviewedBy","reviewedOn"], "properties": {"reviewedBy": {"type": "string"}, "reviewedOn": {"type": "string", "format": "date"}, "policyOwnerApproved": {"type": "boolean"}}},
    "changeHistory": {"type": "array", "items": {"type": "object", "properties": {"version": {"type": "string"}, "date": {"type": "string", "format": "date"}, "summary": {"type": "string"}}}}
  },
  "allOf": [
    {"if": {"properties": {"agentPermissionLevel": {"const": "execute"}}},
     "then": {"properties": {
        "requiredAuthority": {"properties": {"authorityClass": {"enum": ["A0","A1","A2"]}}},
        "reversibility": {"enum": ["reversible","compensatable"]},
        "sideEffects": {"not": {"contains": {"const": "externalEffect"}}},
        "confirmationRequired": {"const": true}}}},
    {"if": {"properties": {"requiredAuthority": {"properties": {"authorityClass": {"enum": ["A3","A4","A5"]}}}}},
     "then": {"properties": {"agentPermissionLevel": {"enum": ["recommend","prepare"]},
        "requiredAuthority": {"properties": {"permittedActorTypes": {"not": {"contains": {"const": "aiAgent"}}}}}}}},
    {"if": {"properties": {"expectedValues": {"properties": {"semanticType": {"const": "money"}}}}},
     "then": {"properties": {"expectedValues": {"required": ["currency","precision"]}}}},
    {"if": {"properties": {"expectedValues": {"properties": {"semanticType": {"enum": ["numericReference","ein"]}}}}},
     "then": {"properties": {"expectedValues": {"required": ["preserveLeadingZeros"], "properties": {"preserveLeadingZeros": {"const": true}}}}}}
  ]
}
```

The conditional blocks encode UX-AI-002 (no `execute` at A3+, no AI actor at A3+), money units (ADV-731) and leading zeros (ADV-732). The schema is validated in CI together with the drift check against the deployed API schema hash (RR-REQ-X-051-r1 AC-2).

### 4.1 Example (synthetic): configuration property

```json
{
  "contractId": "urn:unbound:contract:treasury.pool-liquidity-buffer-floor",
  "version": "1.0.0",
  "ownerComponent": "Fund Accounting",
  "kind": "configProperty",
  "purpose": "The minimum share of each pool's value kept in settled cash so approved grants can pay without waiting for investment sales.",
  "inputSchemaRef": "#/config/treasury/poolLiquidityBufferFloor",
  "outputSchemaRef": null,
  "expectedValues": {"semanticType": "percentage", "precision": 2, "range": {"min": 0}, "defaultSource": {"type": "policyPack", "ref": "PK-OPS@<pinned version> (RCD-017 default 2%)"}, "unknownValueHandling": "reject"},
  "constraints": [{"description": "Platform-safety floor applies; values below it are refused", "policyRef": "UNB-UI-P-010"}],
  "businessConstraints": [{"policyId": "SPONSOR-TREASURY", "policyVersion": "pinned", "effect": "requiresApproval"}],
  "validationRules": [{"ruleId": "BUF-01", "expression": {"minimum": 0}, "errorCode": "BUFFER_BELOW_ZERO"}],
  "dependencies": [{"contractId": "urn:unbound:contract:treasury.liquidity-advance-limit", "relation": "changes"}],
  "examples": [{"name": "Raise floor", "synthetic": true, "request": {"value": "3.00"}, "error": {"code": "BUFFER_BELOW_PLATFORM_FLOOR"}}],
  "commonMistakes": [{"mistake": "Entering 0.02 meaning 2%", "symptom": "Floor set to 0.02%", "correction": "Enter the percentage value 2.00", "errorCode": "BUFFER_IMPLAUSIBLE"}],
  "financialImplications": {"summary": "Higher floor keeps more cash uninvested and may reduce returns; lower floor makes 'Waiting for funds' more likely.", "affects": ["cash", "valuation"]},
  "regulatoryImplications": {"summary": "none", "obligations": []},
  "requiredAuthority": {"authorityClass": "A4", "permittedActorTypes": ["humanRole"], "humanRoles": ["Sponsor policy approver"], "humanApprovalBoundary": "Published only by an approver other than the author with an effective date (UNB-UI-S-010)", "separationOfDuties": ["authorNotPublisher", "decisionTeamOnly"]},
  "agentPermissionLevel": "prepare",
  "sideEffects": ["changesConfig"], "confirmationRequired": true, "reversibility": "reversible",
  "unknownStates": [], "errors": [{"code": "BUFFER_BELOW_PLATFORM_FLOOR", "httpStatus": 422}],
  "outcomeExplanation": [{"result": "BUFFER_BELOW_PLATFORM_FLOOR", "plainLanguage": "This value is below the platform minimum, so it cannot be used.", "evidenceFields": ["platformSafetyFloor"]}],
  "visibility": "roleScoped", "safeActions": ["explain", "draftChange"], "prohibitedActions": ["publish", "approve"],
  "sourceReferences": ["RCD-017", "UNB-UI-S-043", "SPM-010"], "sensitivity": "internal",
  "schemaHash": "sha256:0000000000000000000000000000000000000000000000000000000000000000",
  "editorialReview": {"reviewedBy": "Treasury policy owner", "reviewedOn": "2026-11-24"}
}
```

## B. 5. Email and communication template language

Builds on REQ-X-027, REQ-X-028, REQ-X-029 (superseded by RR-REQ-X-029-r1), REQ-X-031, REQ-X-052 and `TYPED-EMAIL-TEMPLATE-LANGUAGE-AND-AI-AUTHORING-ASSISTANCE-2026-11-23.md`.

**UX-ETL-001 (NEW).** The canonical stored form of every communication template is a versioned, typed AST. Optional friendly syntax compiles to it; it is not Turing-complete: no arbitrary functions, unbounded loops, network or file access, raw HTML, scripts or untrusted partials. Syntax choice remains ADR-gated.

**UX-ETL-002 (NEW).** Every token, formatter, condition, collection, fragment and layout block publishes an AssistanceContract (section 4) with `kind` `templateToken`/`templateFormatter`/`templateCondition`/`templateCollection`/`templateFragment`/`templateBlock`, and additionally:
- `allowedEvents`: the notification catalog events (UNB-RCN-PLT-002) where it may be used (event-scoped allowlist; `TOKEN_NOT_ALLOWED` otherwise, RR-REQ-X-029-r1 AC-1);
- `missingBehavior`: `failValidation` or an approved fallback text; **no silent empty financial value**;
- `unknownBehavior`: how Unknown/stale states render (Unknown is a first-class state and never defaults to Paid, Approved or Delivered);
- `formatting`: formatter, locale, currency, time zone (sponsor time zone, absolute dates per UNB-UI-G-018);
- `outputContexts`: subject, HTML, plain text, SMS where permitted, with the escaping rule for each;
- `sensitivity` and `channelRestrictions`: e.g. fund balances and amounts are not allowed in unauthenticated notification channels by default (REQ-X-032);
- `maxCardinality` for collections; `protected: true` for fragments that authors cannot remove or alter.

**UX-ETL-003 (NEW).** Protected legal fragments (legal-sponsor disclosure, acknowledgment statements, mandatory notices) are injected by the publication pipeline, not by author includes; publication fails if a rendered branch lacks a required fragment.

**UX-ETL-004 (NEW).** Conditions are limited to declared boolean and enum states of the event's token snapshot. Eligibility, approval and payment decisions are never computed in templates.

**UX-ETL-005 (NEW).** The editor (NEW-S-021 Communication template editor) shows a searchable token explorer sourced from the contracts, diagnostics with source spans and error codes, previews for every permitted enum state including Unknown, desktop/mobile/plain-text previews with synthetic data, and a semantic diff between revisions. AI help can answer "What can I insert here?", "Why does this fail validation?", "Rewrite this for clarity without changing legal meaning" and "Preview for pending and Unknown", and returns a **structured proposed diff** that is validated exactly like a human edit. AI never publishes; publication requires an approver other than the author.

**UX-ETL-006 (NEW).** As-sent reconstruction: every sent message records template revision, token snapshot hash, brand kit version, terminology version, policy version and renderer version, so the exact subject, HTML and plain text can be regenerated (REQ-X-028); delivery status distinguishes queued, accepted by provider, delivered when evidenced, bounced and (only where reliable) opened.

**UX-ETL-007 (NEW).** Accessibility of output: semantic HTML email with a plain-text equivalent, reading order equal to visual order, link text that names the destination, no meaning in images or colour alone, and a check that sponsor theme colours meet 4.5:1 for text before publication.

## B. 6. Controls that stop AI assistance becoming an unauthorized decision channel

| ID | Control | Enforced where | Evidence |
|---|---|---|---|
| UX-AIC-01 | AI output is typed Proposal, Finding or Explanation; gates accept it only as attached evidence for a human (RR-UNB-AI-STD-003-r1) | Domain command layer | Gate input schema has no AI-output type |
| UX-AIC-02 | Contract validator rejects `execute` at A3+ and any `aiAgent` actor at A3+ (UX-AI-002) | CI | Schema conditional; failing fixture |
| UX-AIC-03 | Effective ceiling = min(contract, agent manifest, user authority, sponsor policy), re-evaluated at execution (UX-AI-003) | Safe Tool Gateway | Denial audit events |
| UX-AIC-04 | Assistance panels contain no consequential controls; output labelled "AI-generated, not a decision" with sources and as-of (UNB-UI-G-011, FORMA-GAP-16) | UI and Playwright tests | Screens AC: no Approve/Release/Submit in panel |
| UX-AIC-05 | Context only through the public query layer under the initiating principal's verified tenant context; retrieved resource IDs logged (UNB-RCN-PLT-004) | Context broker | Session retrieval log |
| UX-AIC-06 | Retrieved documents, messages, template content and tool results are untrusted data, never instructions (AIN-012); injection detections counted (SPM-066) | Gateway | Adversarial suite ADV-727, ADV-747 |
| UX-AIC-07 | An agent cannot close, resolve or suppress a case, alert or finding it raised or that concerns its own action | Case kernel | AC: CloseCase by raising agent denied |
| UX-AIC-08 | AI cannot change Next Best Action rank or queue priority; it may only explain (UNB-SMP-DSH-001) | Ranking service | Rank computed without AI inputs |
| UX-AIC-09 | Material change invalidates AI proposals and any approvals built on them (AIN-022) | Proposal service | Stale-proposal acceptance blocked |
| UX-AIC-10 | Drafted, submitted, accepted, posted and settled remain distinct statuses; AI never reports success it did not observe (AIN-014) | Response templates | Status vocabulary tests |
| UX-AIC-11 | Unknown and stale are stated as such; "I cannot determine" with the missing evidence named (REQ-X-053) | Explanation service | SPM-064, NEW-M-039 samples |
| UX-AIC-12 | AI agents never hold A3+ delegation; reserved work is routed only to the owning sponsor's decision team (UNB-SMP-ORG-003, AIN-003) | Delegation and routing | Refused registration (NEW-S-027) |
| UX-AIC-13 | Model, prompt and tool changes require evaluation and independent model-risk sign-off (AIN-027) | AI configuration promotion (NEW-S-054) | Promotion record |
| UX-AIC-14 | AI configuration proposals are drafts that follow the normal policy lifecycle; preview-to-save version check (ADV-746) | Policy lifecycle | Version mismatch refusal |
| UX-AIC-15 | Every AI suggestion, proposed diff, user confirmation and executed command is audited with agent identity and version (AIN-013) | Audit | Replayable trace |
| UX-AIC-16 | When the model is unavailable, deterministic gates still decide from deterministic inputs; assistance degrades to help articles | Gates; UI | RR-UNB-AI-STD-003-r1 AC-2 |

## B. 7. Acceptance criteria

- **AC-1** Given an API operation, configuration property, workflow action or UI field in scope without a valid contract, when an assistant is asked about or to use it, then the assistant says it is undocumented, makes no call, and the surface appears as "Not assistant-ready" on NEW-S-055.
- **AC-2** Given a contract with `agentPermissionLevel: execute` and `authorityClass: A4`, when CI validates contracts, then validation fails.
- **AC-3** Given a contract with `semanticType: money` and no `currency`, when validated, then it fails (ADV-731).
- **AC-4** Given a field with `semanticType: numericReference`, when an example value `000417295` is round-tripped through the contract, UI validation and API, then all three preserve the leading zeros (ADV-732).
- **AC-5** Given a donor-scoped agent, when it lists contracts, then restricted and staff contracts are not returned (RR-REQ-X-051-r1 AC-3).
- **AC-6** Given UI validation and the contract `validationRules` for the same field, when the generated UI rules are compared, then they are identical (no silent divergence; ADV-729).
- **AC-7** Given a template slot using `fund.balance` for event GrantPaid, when the author saves, then `TOKEN_NOT_ALLOWED` (RR-REQ-X-029-r1 AC-1).
- **AC-8** Given an AI rewrite proposal that removes the legal-sponsor disclosure, when applied as a draft, then validation fails and publication is impossible.
- **AC-9** Given a template condition on `grant.status`, when previews are generated, then a preview exists for every enum value including Unknown, and none of them renders "Paid" for Unknown.
- **AC-10** Given an agent that raised finding F, when the same agent calls close on F, then it is denied and logged.
- **AC-11** Given an AI configuration proposal created at config version 40, when the user saves after version 41 is published, then the save is refused with a version-changed message (ADV-746).
- **AC-12** Given every contract example, when scanned, then all are marked `synthetic: true` and contain no secrets or live personal data (ADV-726).
- **AC-13** Given the assistance panel on any screen, when Playwright queries for buttons named Approve, Release, Submit, Publish, Pay or Post inside the panel, then the count is 0.

## B. 8. Open issues

- Template syntax (Liquid-like vs bespoke) remains ADR-gated; the AST is canonical either way.
- Whether `execute` is offered at all in release 1 is an owner decision; this document assumes it is limited to A0..A2 reversible self-service commands (for example, saving a draft or changing an optional notification preference) and recommends shipping release 1 with `recommend`/`prepare` only.
- Editorial review ownership for business meaning (who signs `editorialReview` for regulatory text) needs a named role per component.
- Deprecation window length for contracts and widgets is not stated in sources and must be set by platform policy.
