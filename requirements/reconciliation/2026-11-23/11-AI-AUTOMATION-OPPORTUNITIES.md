# 11 — AI automation opportunity analysis

Date: 2026-11-23 | Scope: where AI genuinely improves Unbound, where deterministic automation must be preferred, the single authority scale, and the standard AI-assistance contract. Market context: only Daffy documents a live donor-facing agent interface (MCP, with no documented limits), and no sponsor uses AI to approve grants (report 05, CAP-24 and CAP-41).

## 1. Governing rule

**Deterministic rules decide; AI prepares, explains and flags; authorized humans resolve what rules cannot.** This is REQ-X-017's rules-first, AI-second, human-third routing, made precise by three refinements:

- `RR-UNB-AI-STD-003-r1`: money movement, access grants, eligibility gates, hold lifting and policy publication are decided only by versioned rules or authorized humans. AI output is typed `Proposal | Finding | Explanation` and is excluded from gate input schemas. No agent closes an alert it raised or one that concerns its own action.
- `RR-AIN-003-r1`: A4 decisions need a human who did not originate, prepare or modify the item. A3 decisions may be made by board-delegated deterministic policy only if RCD-005 / OD-01 approves it. An AI agent never approves A3, A4 or A5.
- **One-way ratchet:** AI may *add* a hold or a finding, never *remove* one (CAP-41 exclusion).

## 2. One authority scale (RCD-025)

The corpus uses four incompatible scales: A0–A5 (`SPONSOR-AUTHORITY-AND-AUTONOMOUS-OPERATIONS.md`), L0–L3, Tier 0–4 and Tier 1–3 (RCF-PLT-019). Adopt A0–A5 as the only scale (`UNB-RCN-PLT-005`) and map the others onto it:

| Level | Meaning | AI agent may | Deterministic policy may | Human |
|---|---|---|---|---|
| A0 Observe | Read authorized records, explain | Yes (within caller's scope) | — | Yes |
| A1 Prepare | Draft, classify, investigate, simulate, propose | Yes; output is a Proposal | Yes | Yes |
| A2 Bounded internal action | Deterministic, reversible, low-risk (tag, route, fetch permitted public records, open a case) | Only actions listed in the authority matrix | Yes | Yes |
| A3 Controlled consequential action | Sponsor-authorized action within policy limits | **No** | Only with RCD-005 approval for that decision type | Yes, with segregation of duties |
| A4 Reserved human authority | Discretionary grant approval, policy change, high-risk override, sanctions disposition | **No** | **No** | Independent authorized human only |
| A5 Prohibited | Bypass law, falsify evidence, backdate, unapproved live payment, self-escalation | **No** | **No** | **No** |

## 3. Opportunity assessment

Value: H/M/L reduction in human effort or error. Risk: consequence if the AI is wrong *given the controls in §1*.

| Workflow | AI use | Level | Value | Residual risk | Deterministic alternative that must exist first | Verdict |
|---|---|---|---|---|---|---|
| Charity verification | Extract facts from determination letters, audited statements and foreign registries, with provenance | A1 | H | M: a wrong extraction is caught because facts are evidence, not status | Per-fact FreshnessPolicy and evidence matrix (UNB-RCN-GRT-013/014) | **Use** |
| Charity verification | Compare contradictory sources; draft reviewer summary with citations | A1 | H | L | Disputed state is raised deterministically | **Use** |
| Charity verification | Set recipient class, clear an adverse hold, promote web content to evidence | — | — | Unacceptable | Rule-based class from IRS codes plus related-party graph | **Prohibit** |
| Grant intake | Classify free-text grant purpose into candidate benefit categories (may only raise a hold) | A1 | M | L | Benefit catalogue plus host-context declaration (UNB-RCN-GRT-010/012) | **Use** |
| Grant intake | Explain holds and suggest compliant alternatives to donors | A0/A1 | M | L | Typed hold reasons | **Use** |
| Grant approval | Approve, affirm for the donor, or pass a held benefit case | — | — | Unacceptable | — | **Prohibit** |
| Contribution matching | Suggest matches for unidentified receipts with confidence | A1 (A2 only for exact amount, reference and sender) | M | L | Rail evidence matching | **Use** |
| Complex assets | Assemble diligence dossier (liens, debt, prearranged-sale indicators) | A1 | M | M | Asset-class policy table | **Use** |
| Complex assets | Valuation selection, acceptance decision | — | — | Unacceptable | Committee | **Prohibit** |
| Reconciliation | Investigate breaks and propose explanations with cited evidence | A1 | H | L | Continuous deterministic matching | **Use** |
| Reconciliation | Clear a break or mark Unknown as Pass | — | — | Unacceptable | — | **Prohibit** |
| Payments | Detect anomalies in bank letters and destination-change requests (may raise a hold) | A1 | M | L | DestinationEvidence table (UNB-RCN-GRT-017) | **Use** |
| Payments | Any money-state transition | — | — | Unacceptable | — | **Prohibit** |
| Sponsor configuration | Explain contracts; propose configuration diffs; find contradictions between settings | A1 | H | L: a human approves the diff | Validation and impact preview generated from contracts | **Use** (highest-value admin use) |
| Exception handling | Summarise case; draft evidence requests; propose next action with sources | A1 | H | L | Rule-created WorkItems with exit conditions | **Use** |
| Communications | Draft slot text, plain-language rewrites, alt text for human approval | A1 | M | L | Locked legal clauses; typed tokens | **Use** |
| Communications | Publish templates, choose recipients, generate figures or status text | — | — | Unacceptable | Event catalog | **Prohibit** |
| Donor support | Grounded answers from authoritative state under the caller's scope | A0 | H | M: hallucination; mitigated by answering only from retrieved, scoped records with citations | Self-service status | **Use**, with refusal when no source exists |
| Donor tax guidance | Personalized tax advice; implying nonitemizer deductibility of DAF gifts | — | — | Unacceptable (IR-2026-135; conflict of interest) | Counsel-approved disclosures | **Prohibit** (RCD-030) |
| Migration | Propose field mappings; explain breaks | A1 | H | L | Tie-outs and fencing | **Use** |
| Succession | Extract death-certificate and court-document data for comparison | A1 | M | M | Human verification | **Use** (never declares death verified) |
| Regulatory reporting | Variance narratives | A1 | L | L | Package is a pure query | **Use** |
| Control evidence | Draft control narratives; propose requirement-to-control links with rationale | A1 | M | L: replaces the 1,892 keyword links | Human validation of each link | **Use** |
| Telephone | Voice agents authenticating callers or acting | — | — | Unacceptable (deepfake risk) | Callback to verified channel | **Prohibit** |

**Deterministic automation is preferred** wherever the rule can be written down: eligibility gates, sanctions matching thresholds, reservation and capacity, fee accrual, NAV tolerance checks, retention clocks, dormancy stages, recurring-occurrence generation, notification routing, contribution acknowledgment issuance, Schedule D aggregation and closure preconditions. In each of these the requirement set was missing the rule rather than missing intelligence.

## 4. Standard AI-assistance contract

`RR-REQ-X-051-r1` (platform audit, amended by the integrating review) defines a JSON Schema (draft 2020-12) contract, generated from F# types and the OpenAPI source (`UNB-RCN-ARC-001`) and version-pinned to the API. Mapping to the mission's required elements:

| Mission element | Contract field(s) |
|---|---|
| Purpose | `purpose` (≤280 chars), `kind`, `ownerComponent` |
| Available operations | discovery returns contracts filtered by caller role and tenant (`visibility`) |
| Input semantics | `inputSchemaRef`, `outputSchemaRef` |
| Validation rules | `validationRules` (amendment) |
| Permissions | `authorityClass`, `permittedActorTypes` (amendment), `visibility` |
| Business constraints | `businessConstraints` with policy IDs and versions (amendment) |
| Side effects | `sideEffects` (none, createsDraft, changesConfig, changesAuthoritativeState, externalEffect), `reversibility` |
| Error conditions | `errors`, `unknownStates` |
| Example usage | `examples`: at least one request, response and error (amendment) |
| Safe and prohibited actions | `safeActions`, `prohibitedActions` (amendment) |
| Human approval boundaries | `confirmationRequired` (forced true for A3 and above), `humanApprovalBoundary` (amendment) |
| Explanation of outcomes | `outcomeExplanation` template per result and error code (amendment) |

A surface is *assistant-ready* only when its contract validates and matches the deployed schema hash. Contract text never grants permission; authorization is always re-evaluated at execution.

## 5. Required controls for any AI use

| Control | Requirement |
|---|---|
| Provenance and reproducibility | Every AI output records model ID and version, prompt template version, retrieved record IDs and versions, and timestamp. Proposals are reproducible from those inputs or marked non-reproducible. |
| Scoped retrieval | Context is read only through the public query layer under the initiating principal's scope (`UNB-RCN-PLT-004`). Retrieved content is data, never instructions (prompt-injection defence). |
| Confidence | Confidence is reported but never used as an authorization input. |
| Failure | AI unavailable or failing: gates continue on deterministic inputs; ambiguous cases go to humans; nothing fails open (REQ-X-017). |
| Measurement | Straight-through rate, human touches per completed outcome, AI-proposal acceptance and reversal rates, and AI-attributed incidents per sponsor and workflow (`UNB-RCN-PLT-006`). These are the evidence for or against the EF hypothesis in `PRINCIPLES.md`. |
| Adversarial tests | DKS-XD-008 (agent above its level), DKS-PLT-* AI scenarios, plus the existing ADV injection cases. |

## 6. Where AI would introduce unacceptable uncertainty (summary)

Recipient classification, sanctions disposition, hold release, valuation, acceptance of complex assets, money-state transitions, publication of policies or templates, legal or tax conclusions, identity decisions (especially voice or video), and closing its own findings. These are not "AI later" items. They are A4 or A5 by design.
