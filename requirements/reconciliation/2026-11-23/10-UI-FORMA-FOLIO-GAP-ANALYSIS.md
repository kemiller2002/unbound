# 10 — UI and Forma/Folio gap analysis

Date: 2026-11-23 | Inputs: `requirements/ui/*`, `UNB-UX-*`, `INT-*`, `UX-S*`, `ADM-*`, `AIN-*`, REQ-X-011/024..035/043..055/063; the Echelon capability verification in [13](13-ARCHITECTURE-DEPENDENCY-ANALYSIS.md). Forma component names are candidates from the Forma catalog. **No Forma or Folio capability is asserted beyond what report 13 verified.**

## 1. Verdict

The UI requirements are strong on *integrity semantics*: decision cards, evidence-versus-inference labelling, authoritative status, and "never show Completed until confirmed". They are weak in four ways:

1. **They are organised around departments and queues** (Sponsor Command Center, Grants, Compliance, Contributions, Control Operations workspaces) rather than user goals and unresolved exceptions (RCF-PLT-023). The same "work" appears in five case models, three dashboard concepts and several command centers (RCF-PLT-024/026).
2. **Screen state sets omit `Unknown` and `Failed`** (RCF-PLT-027). An unconfirmed payment can only render as Processing or Completed, which is exactly the false-finality risk the payment requirements try to prevent.
3. **They assume Forma and Folio capabilities that do not exist**: charts, configurable dashboards, data-table behaviour, a widget runtime, an email designer, accessible (tagged) PDF, cryptographic document verification, and terminology. See §4.
4. **Some mandates are not outcomes.** "Every journey phone-complete" (RCF-PLT-028) is impractical for reconciliation and close. "AI help on every field" (RCF-PLT-022) is unbounded. "Monthly statement for every fund" (RCF-FIN-034) is a convention.

## 2. Recommended experience model: goal and exception first

| Surface | For | Organised around | Replaces |
|---|---|---|---|
| **Giving home** (donor/advisor) | Donors, advisors, successors | Balance and capacity (authoritative), what is pending and why, next action, charity search (UNB-RCN-PLT-014) | Account overview plus separate grant, contribution and document pages as primary navigation |
| **Exception inbox** (staff, single) | Sponsor operations, compliance, finance | Rule-created WorkItems, each with a creating rule, owner, SLA and exit condition (RR-REQ-X-048-r1). Routine items never appear. | Department queues, round-robin of routine work, multiple command centers |
| **Case workspace** | The person resolving one exception | Facts, evidence, conflicting sources, permitted actions, consequences preview | Five case models |
| **Configuration and approval** | Sponsor governance | Policy diff, impact preview generated from contracts, approval | Free-form settings screens plus separate help |
| **Sponsor overview** (role-composed) | Executives | Outcomes at risk: holds by age, STP rate, reconciliation status, evidence freshness, with no unsupported health score | Sponsor Command Center plus executive dashboards plus assurance dashboards |
| **Charity view** | Charities | Grants to them, remittance, destination management, verification status | Separate charity portal products per rail |
| **Embedded widgets** | Partner hosts | Grant initiation and status only, through Unbound-owned widget runtime (DG-03) | Five restatements of widget security (RCF-PLT-047) |

Desktop-first is acceptable for financial operations, close, reconciliation and configuration. Donor and advisor journeys and approve-or-review actions must be phone-complete (UNB-RCN-PLT-011).

## 3. Actor and capability coverage

| Capability | Existing requirements | Assessment | Forma need (verified status from 13) | Folio need | Action |
|---|---|---|---|---|---|
| Donor portal | UX-S02, UX-S06, UNB-UX-001, REQ-X-011 | Partial: no charity search; status must render Unknown/Failed | forms ✔, key-value/facts ✔, freshness ✔; tables partial | Grant confirmation, statements | UNB-RCN-PLT-014; RR-UNB-UX-003-r1 |
| Advisor portal | REQ-X-062, UNB-ADV-001/002, ADM-006 | Partial: feed contract and multi-tenant selection missing (RCF-PLT-044) | same as donor | Advisor statements | Refine REQ-X-062 |
| Sponsor administration | ADM-*, UX-S01/S08, REQ-X-045 | Over-specified by department | diff-viewer ✔, comparison ✔, dialog ✔ | Policy change records | Configuration and approval surface |
| Charity portal | REQ-X-058, REQ-X-077 | Covered (draft); bank-detail integrity in REQ-X-005 | forms ✔ | Remittance advice | Prefer rail-partner onboarding (CAP-27) |
| Universal workbench | REQ-X-048/049, ADM-031, UX-S05 | Duplicated; queue-centric | work-queue ✔, data-grid partial | Case decision package | Single exception inbox |
| Executive reporting | REQ-X-043, UX-S01 | Conflicts with REQ-X-049/050 dashboards (RCF-PLT-026) | **charts ✘**, dashboard-grid partial | Board/operations report | Tabular outcome-at-risk view until DG-01 |
| Operations dashboards | MON-023, REQ-X-049/050, CONTINUOUS-ASSURANCE-FORMA-WIDGETS-AND-FOLIO | Defer configurable dashboards (RCD-026) | **configurable dashboards ✘** | — | Replace with exception inbox metrics (UNB-RCN-PLT-006) |
| Embedded widgets | INT-015, AIN-029, WG-*, REQ-X-063 | Security restated five times; ownership misattributed to Forma (RCF-PLT-010) | **widget runtime ✘** (only `<forma-workflow>`) | — | Unbound-owned runtime, Limen effects, Forma markup only (DG-03) |
| Mobile | UNB-UX-001, UNB-EXP-001 | Over-broad mandate | mobile patterns ✔ | Mobile-readable statements | Phone-complete classification (UNB-RCN-PLT-011) |
| Accessibility | UNB-EXP-001, UNB-PLAT-005 | Sound intent; document accessibility escapes through "where supported" (RCF-PLT-029) | axe WCAG 2.2 AA on Forma fixtures ✔ | **tagged PDF ✘** | Accessible HTML rendition of every document (UNB-RCN-PLT-010); RCD-009 |
| White labelling | REQ-X-030, UNB-PLAT-009 | Conflicts with public "Unbound" branding (RCC-ENT-001) | Brand Manifest compiler ✔ | Brand applied to print | RCD-003 |
| Dynamic terminology | REQ-X-030 | No owner (RCF-ARC-002); no protected terms (RCF-PLT-040) | **not provided by design** | Terminology in documents | UNB-RCN-ARC-002, UNB-RCN-PLT-007 |
| AI assistance | REQ-X-051, 053–055, AIN-* | Duplicated IDs; no schema (RCF-PLT-018) | conversation/composer ✔ | — | RR-REQ-X-051-r1; report 11 |
| Notifications | AIN-023, ADM-030, INT-017 | No event catalog (RCF-PLT-041) | callout, choice-group ✔ | Email/print renditions | UNB-RCN-PLT-002 |
| Forms and validation | UNB-PLAT-002, UNB-GFUND-008 | Covered; field-state styling missing in Forma (P0 unimplemented) | form family ✔; disabled/invalid styling ✘ | — | Forma request DG-02 |
| Document presentation | REQ-X-034/035, UNB-DOC-* | Renderer unverified | data-grid partial, identifier ✔ | **deterministic PDF planned only; signing ✘** | RCD-009; registry hashing independent of renderer |
| Error and recovery states | ADM-036, UX-S* state set | Missing Unknown/Failed (RCF-PLT-027) | fault-*, recovery-actions ✔ | — | RR-UNB-UX-003-r1 |
| IVR / telephone | REQ-X-025 | No demand evidence (RCF-PLT-039) | identifier ✔ | Reference on letters | Defer IVR; keep numeric references (RCD-008) |
| Email designer | REQ-X-029, REQ-X-031 | Not needed for transactional notices (RCF-PLT-011) | **planned only** | — | RR-REQ-X-029-r1 constrained templates (RCD-008) |

✔ verified in report 13 · ✘ not present · "partial" as defined in report 13.

## 4. Forma and Folio requirements to raise (recorded here only, per mission rules)

| ID | Component | Need | Priority | Blocks |
|---|---|---|---|---|
| DG-01 | Forma | Accessible chart primitives with text-equivalent tables | P1 | Executive and investment views (tabular fallback is acceptable at release 1) |
| DG-02 | Forma | Table styling, sort and filter, bulk actions, virtualised lists, field disabled/invalid/read-only states | P0 | Ledger, grant history, exception inbox |
| DG-03 | Unbound (with Forma markup) | Widget host, frame and resize protocol, scoped tokens | P2 (checkout deferred) | REQ-X-063 |
| DG-04 | Folio or alternative | Server-side deterministic PDF, batch generation, tagged PDF or accessible HTML twin | P0 for statements | REQ-X-032, acknowledgments |
| — | Unbound | Terminology resolver with protected legal terms | P1 | White label |
| — | Unbound | Domain status vocabulary including Unknown, Blocked and Failed across all screens | P0 | Payment truthfulness |

## 5. UI capability map from the platform audit

| UI capability | Actor goal | Forma component needed | Folio output needed | Existing requirement IDs | Gap |
|---|---|---|---|---|---|
| Exception inbox (single, rule-created items) | Staff: unblock outcomes that rules could not finish | collection-toolbar, data-grid, attention-path, freshness, empty-state, fault-inline | none (export via report) | REQ-X-048; REQ-X-049; ADM-031; UX-S05; AIN-015 | No rule/exit-condition requirement; five case models; no STP metric (RCF-PLT-023/025) |
| Case workspace | Staff: decide or gather evidence for one case | facts, evidence-relationship, conflict-review, handoff-summary, dialog, composer | Case decision package | UX-S05; INT-002; INT-003; AIN-016 | Domain state vocabulary incl. Unknown not required (RCF-PLT-027) |
| Grant recommendation (donor/advisor) | Donor: support a charity with confidence it will arrive | form, form-row, combobox, allocation, freshness, alert | Grant confirmation letter | UX-S06; UNB-UX-001; REQ-X-011 | Phone-complete classification missing (UNB-RCN-PLT-011) |
| DAF account overview | Donor/advisor: know balance, pending items and next action | key-value-list, facts, feed-item, badge, freshness | Monthly statement | UX-S02; ADM-034; INT-016 | Status must render authoritative states incl. Unknown |
| Participant and delegation manager | Donor/admin: give the right people the right rights | hierarchy-tree, diagram, dialog, choice-group | Delegation confirmation | UX-S03; ADM-005; ADM-021 | Tenant selection for multi-partner users (RCF-PLT-044) |
| Succession designer | Donor: record wishes for successors | diagram (with text equivalent), allocation, form | Successor plan summary | UX-S04; UNB-UX-002; INT-010 | Desktop-first acceptable; device policy missing |
| Document Center | Donor/sponsor: find and verify issued documents | data-grid, file-upload, identifier, diagnostic-status | As-issued PDF plus accessible rendition | REQ-X-035; UX-S10; UNB-DOC-004 | Accessible rendition not guaranteed (RCF-PLT-029) |
| Communication template editor (constrained) | Sponsor admin: brand and word notices correctly | form, form-row, diff-viewer, dialog | Email/print preview | REQ-X-027; REQ-X-028; REQ-X-030 | Visual designer to be superseded (RCF-PLT-011) |
| Brand Kit and terminology | Sponsor admin: present sponsor identity without changing meaning | form, comparison-pairs, alert | Brand proof sheet | REQ-X-030; UNB-PLAT-009 | Protected-term registry missing (UNB-RCN-PLT-007) |
| Notification preferences | Donor: control optional messages, keep mandatory ones | choice-group, checkbox, callout | none | AIN-023; ADM-030; INT-017 | No notification catalog (UNB-RCN-PLT-002) |
| Contextual help / explain this | Any user: understand a field or action | flyout, disclosure, conversation | none | REQ-X-051; REQ-X-054; INT-004 | No contract schema; AI scope too broad (RCF-PLT-018/022) |
| Configuration diff and approval | Sponsor governance: approve a policy change knowing its effect | diff-viewer, comparison-grid, dialog, fault-summary | Policy change record | ADM-048; REQ-X-045; UX-S08 | Single authority matrix missing (UNB-RCN-PLT-005) |
| Sponsor home (role-composed) | Sponsor exec/ops: know where outcomes are at risk | dashboard-grid, freshness, badge, card-grid | Board/operations report | UX-S01; REQ-X-043; REQ-X-049; REQ-X-050 | Duplicate command centers/widgets (RCF-PLT-024) |
| Financial operations | Controller: resolve reconciliation breaks | dense-ledger, data-grid, comparison-pairs, fault-details | Reconciliation and close package | UX-S12; MON-019 | Desktop-first; not phone-complete |
| Evidence export | Auditor: obtain period-bounded evidence | data-grid, date-range, diagnostic-status | Folio audit evidence package | SOC2-009; MON-015; MON-016 | GRC screens beyond export not justified (RCF-PLT-035) |
| Embedded widgets | Partner host: show sponsor-authoritative status | (Unbound/Limen-owned; Forma markup only) | none | AIN-029; INT-015; ADM-037; WG-1..12 | Widget runtime ownership misattributed to Forma (RCF-PLT-010) |
| Telephone/numeric reference | Caller/support agent: locate a record | identifier | Reference on statements/letters | REQ-X-025 | IVR channel deferred; reference kept (RCF-PLT-039) |

Component names above are taken from the Forma catalog listing (catalog/components in the forma clone) and are candidates only; no Forma or Folio capability is asserted as verified.

