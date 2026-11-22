# Working recommendations, challenges, and justification (2026-11-22)

This is a **preliminary structured design review** of 161 remaining questions, not 161 independently researched determinations. The 22 owner decisions were reviewed separately. No proposed decision is legally approved, implemented, simulated, or externally verified. The typed JSON registry retains each full question, decision, justification, counterargument, validation requirement and review event.

## Review discipline

- Contradictions: retain the proposed canonical resolution unless a material challenge requires clarification; validate cross-references and affected workflows.
- Assumptions: do not affirm unmeasured hypotheses; require evidence, sensitivity analysis and fallback.
- Requirements: preserve the intended outcome without inventing numerical thresholds, vendor capabilities or legal interpretations.
- Severe findings: propose protective controls, then test bypasses, third-party dependencies, outage behavior and accessibility impact.
- External evidence: maintain explicit evidence gates; vendor marketing is not contractual or operational proof.

## Recommendations materially challenged: defer for user review

- **TVC-034:** Which requirement prevails? REG-007 cross-reference "(REG-057, REG-061)"; REG-061 is NYC LL144
  - Original recommendation guesses a cross-reference target ('probably REG-058'); requires source-level verification before correcting.
- **TVC-041:** Which requirement prevails? Org §5.1 (advisor may delegate to an agent) **vs** DD-021 (only the donor may change delegation); INV-A6 lets an advisor confirm their own agent
  - Independent confirmation by a person other than the delegator may conflict with donor-controlled delegations and accessibility; design must distinguish self-authorization, agent self-confirmation and valid principal confirmation.
- **TVA-020:** Is it true that: Shared determinations across sponsors are legally relied upon
  - Cross-sponsor reliance on determinations cannot be assumed; scope, liability and sponsor accountability need legal and commercial evidence.
- **TVR-DR-001-Q:** RPO targets (TVR-DR-001: One consistent restore point across payment, decisioning, authority, evidence; zero-loss or re-derived security state; p)
  - Zero-loss claims across distributed systems and evidence storage require a feasibility and cost decision, not a presumed achievable RPO.
- **TVF-228:** How should the requirements answer: B6 creates break-glass for the period when the command service is down; OD-22 permits break-glass only through a domain command, which cannot exist then
  - Command service outage makes command-only break-glass unavailable; a separate independently controlled emergency command path or freeze-only procedure must be designed and tested without allowing direct data edits.
- **TVF-234:** How should the requirements answer: B18 accepts 'account fingerprint or name-match result', but a fingerprint computed by the provider is controlled by the party whose compromise B18 is meant to detect
  - Provider-produced destination fingerprints cannot independently establish payee integrity; needs a trusted sponsor-side or independent evidence source and a defined mismatch-resolution process.

## All 161 question-by-question dispositions

### TVC-001

**Issue:** Which requirement prevails? DD-006 and data model §1: "event ledger is the system of record; entities are projections" **vs** financial core: "Full event sourcing is not required; … authoritative relational state may be sufficient"

**Proposed:** Restate as a **requirement**, not a mechanism: "the enterprise must reconstruct any past decision's inputs and any balance as known at any recorded time" (TVR-DATA-001). Leave the mechanism to implementation. That removes the conflict without choosing technology.

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-002

**Issue:** Which requirement prevails? Admin master, UI catalog, vertical slices, design system, widget platform: "F#, Limen WASM, Forma, Folio" **vs** DD-014 (no runtime dependency; "Limen ships no WASM engine"; PolyForm Noncommercial licence) **vs** this directive (no premature technology selection)

**Proposed:** (b). Requirements become technology-neutral ("a controlled document renderer producing PDF/UA-conformant output").

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-003

**Issue:** Which requirement prevails? DD-004 "routine grants … approved by deterministic policy with no human touch" **vs** REG-001 row "sponsor-signed approval on every grant" and "approval authority cannot be vested in software or a vendor"

**Proposed:** (a), only with: an explicit board resolution delegating to named policy versions; a publish gate where the author is not the approver; a per-decision record citing the resolution. Counsel must confirm (OD-01). Until then, DD-004 is a **hypothesis**, not a requirement.

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-004

**Issue:** Which requirement prevails? control-framework: exception grants need "specialist plus a second approver above $50k" **vs** governance-architecture: specialist accountable up to $250k, officer above, CEO above $1m

**Proposed:** One delegation-of-authority table, as data, referenced by both. Values are [P].

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-005

**Issue:** Which requirement prevails? Write-off: second human above $500 **vs** Controller to $25k, CFO above, Audit Committee above $250k

**Proposed:** Same as TVC-004, plus SoD rule TVR-ORG-003

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-006

**Issue:** Which requirement prevails? REG-036 map: "AI may never close a **high-score** match without human sign-off"; "automation may clear obvious false positives under a documented rule" **vs** DD-005 / CTL-008 "AI may add flags, never clear them"

**Proposed:** (b), with the rule published under the policy gate and an independent QA sample. A model never clears.

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-007

**Issue:** Which requirement prevails? WF-01: benefit-risk agent still running → grant "waits up to 2 minutes and then proceeds"; flag after release → post-payment review **vs** the safety rationale of flag-only AI

**Proposed:** (b): on timeout, route to a deterministic keyword/rule screen. If that cannot run, hold. "Proceed without the check" is not a default. Record the residual.

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-008

**Issue:** Which requirement prevails? Grant status vocabularies: financial core lifecycle; WF-01; open record (pending/approved/paid/rejected/canceled/returned); design system label list; D-GR-08; widget platform receipt (Rejected/Accepted/Processing/Succeeded/Failed/Unknown); W02 receipt (Accepted/Rejected/Unknown); admin OPS-03

**Proposed:** [Status vocabulary](../09-ux/status-vocabulary.md): separate dimensions (request, decision, funding, payment, finality). "Accepted" is banned in donor-facing copy.

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-009

**Issue:** Which requirement prevails? Widget platform receipt "Succeeded must be defined by domain-specific outcome" **vs** no domain defines it

**Proposed:** Each command type declares its terminal success fact (for a recommendation: `Submitted` recorded)

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-010

**Issue:** Which requirement prevails? Contribution order: D09 "Accepted, received" **vs** W14 "received/accepted" **vs** J-CON "proposed, accepted, transferred, valued, disposed" **vs** OPS-02 (no disposition; adds allocation)

**Proposed:** Asset-class-specific machines ([state models §5](../06-state-models/state-models.md#5-contribution))

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-011

**Issue:** Which requirement prevails? J-ADV: "context switching invalidates … queued commands" (implies a client command queue) **vs** D-GR-06 "no offline financial submission claim"

**Proposed:** (b): offline = drafts only; submission is online with fresh revision

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-012

**Issue:** Which requirement prevails? UI catalog: "resumable encrypted drafts" **vs** design system: "avoid storing sensitive information in local storage by default; requires threat-model approval" **vs** J-REM open question

**Proposed:** Server-side drafts by default (W02 already says so); local only after threat model

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-013

**Issue:** Which requirement prevails? Admin P0 includes contribution acceptance and fee engine **vs** UI catalog places D08–09/O08–09 at P1 and E14 fees at P2; traceability P1 includes family access/notifications vs catalog P2

**Proposed:** Admin P0 governs domain; UI priority must not lag a P0 domain that needs staff screens

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-014

**Issue:** Which requirement prevails? Widget platform: iframe and headless API both "first implementation candidate" **vs** widgets/05 places headless in Phase 4

**Proposed:** Recorded only

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-015

**Issue:** Which requirement prevails? control-framework: "45 controls" and "Obligation coverage is 100%" **vs** controls.csv 54 rows; OUTSTANDING A1 open; CTL-037 excludes guidance, conditional and provider-only legal rows

**Proposed:** Coverage is a **measured** property, reported with its denominator

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-016

**Issue:** Which requirement prevails? Retention: map "10-year default driven by OFAC" [S] **vs** continuous-compliance "at least as long as returns can be examined … [A, confirm with counsel]"; no control has a retention field

**Proposed:** Retention schedule per record class with legal basis [C]; deletion obligations reconciled against holds (OD-12)

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-017

**Issue:** Which requirement prevails? Executive report: "the December surge, not the average, sizes the human organization" **vs** the FTE figures in the same report come from an annual-average model with no surge term. (An earlier draft also contrasted tech-arch 10–30× with twin ×3. That was withdrawn in review: they measure per-second throughput vs daily volume.)

**Proposed:** [Economic model](../15-economics/economic-model.md) §3

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-018

**Issue:** Which requirement prevails? Structural floor "~20 at any scale" (Clarity) **vs** "8–10" (org model; Economics.fs uses 8)

**Proposed:** Recorded; floor depends on SoD design ([organization §4](../03-organization/organization-and-authority.md))

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-019

**Issue:** Which requirement prevails? 19 agents (exec, tech-arch) vs 18 agent families (09 model); 29 vs 30 capabilities

**Proposed:** Editorial

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-020

**Issue:** Which requirement prevails? Exception rate 8% (09, Economics.fs) vs ~10% (economic-validation); $103/h stated vs $90/h implied; GAP-040 range $38.87–$86/h

**Proposed:** All economic parameters in one register with label and range

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-021

**Issue:** Which requirement prevails? WF-01 grant machine: `Reconciled (terminal)`; `Released → Returned` only; and `Approved → Canceled: donor cancels before release` (WF-01 diagram, wf-contributions.md l.103) **vs** sponsor control of approved grants (DD-003), WF-09 post-payment review, and rails that allow returns after settlement. **Review correction:** an earlier draft said there was no cancel transition; there is one. The defects are that it is the *donor* cancelling a *sponsor* act, and that nothing covers cancellation or recall after release.

**Proposed:** [State models §2–3](../06-state-models/state-models.md#2-grant-sponsor-side) (SM-0, `Final`, `Paid→Returned`, cancel rules)

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-022

**Issue:** Which requirement prevails? WF-05: "Approved grants **earmark units**" **vs** financial core: reservations are currency encumbrances on cash resources; W02: "preview is not a reservation"

**Proposed:** (b), with fixing per FI-10 and over-commitment per FI-12; (c) is a policy variant

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-023

**Issue:** Which requirement prevails? WF-01: "`ContributionSettled` … issues units"; acknowledgment "generated from the settlement event" **vs** ACH debit reversibility (and the core's "external balance feed is an observation")

**Proposed:** CON-1..3, FI-22/23; acknowledgment timing [C] (OD-04)

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-024

**Issue:** Which requirement prevails? Financial core invariant 7 (stable provider idempotency keys; query before retry) **vs** rail reality. E9 itself states "ACH has no end-to-end idempotency key, so deduplication depends on the originating bank's API" (experiments.md l.194). **Review correction:** an earlier draft wrongly said E9 *assumes* bank deduplication.

**Proposed:** (a) as selection criterion plus (b) per provider, verified by contract (GAP-002); (c) as last resort (PA-2)

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-025

**Issue:** Which requirement prevails? Identity: platform TOTP "allowed for viewing and routine grants"; phishing-resistant only above $25k [A] **vs** routine auto-approval up to $250k [A] (governance-architecture)

**Proposed:** Auto-approval ceiling for a session ≤ that session's assurance ceiling; a new recipient + phishable factor → hold (TVR-SEC-004)

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-026

**Issue:** Which requirement prevails? DD-017 (donor agents are scoped, revocable principals with confirmation rules) + authority-boundaries (confirm only above `confirm_above`) + DD-004 **vs** DD-003's intent that a human sponsor or policy authorizes money movement

**Proposed:** (c) plus caps; (a)'s controls kept. The confirmation threshold is [P].

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-027

**Issue:** Which requirement prevails? UI/admin docs reference only SOC 2 **vs** assurance methodology: "SOC 1 Type II where fund accounting and financial reporting controls are material; SOC 2 alone cannot substitute"

**Proposed:** Both; SOC 1 for ICFR-relevant services [I]

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-028

**Issue:** Which requirement prevails? Accountability matrix: Controller accountable for disbursement (C3.4) **and** reconciliation (C4.2) **and** ledger **and** fees; also attests its own variance pack (CTL-048)

**Proposed:** Reconciliation accountable to a role outside the payment chain (TVR-ORG-003)

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-029

**Issue:** Which requirement prevails? CTL-037 tests only `current_law ∧ sponsor=yes` rows **vs** legal-type guidance rows (REG-016/019/020) and conditional rows

**Proposed:** Test all `obligation_type=legal` rows, with explicit N/A justifications

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-030

**Issue:** Which requirement prevails? Admin scenario 4 "reevaluate advisory rights and pending instructions" on donor death **vs** no rule for already-approved grants; WF-13 not checked against GR-8

**Proposed:** GR-8 (complete sponsor-approved grants; suspend standing recommendations) [P][C]; OD-03

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-031

**Issue:** Which requirement prevails? Widget host event set: platform v0.1 (ready, resize, navigate request, command accepted, close, recoverable error) **vs** W01 trust contract (ready … sessionExpired, fatalError)

**Proposed:** W01 v0.2 supersedes v0.1; mark v0.1 superseded

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-032

**Issue:** Which requirement prevails? identity: "deprovisioning … access ends at once" **vs** onboarding example "deprovision within one hour"

**Proposed:** Both true at different layers. State the end-to-end maximum exposure window as a contractual requirement (TVR-WID-006).

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-033

**Issue:** Which requirement prevails? AUTH-07 / SUCC-04 (interim authority and restricted hold during verification) **vs** WF-05 recurring grants re-evaluated "against current eligibility" only, not current authority

**Proposed:** FI-09 + REC-3: each instance re-checks authority; notice of death applies holds retroactively to the effective date for unfunded instances

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-034 | NEEDS CLARIFICATION

**Issue:** Which requirement prevails? REG-007 cross-reference "(REG-057, REG-061)"; REG-061 is NYC LL144

**Proposed:** DEFERRED FOR USER CLARIFICATION: Original recommendation guesses a cross-reference target ('probably REG-058'); requires source-level verification before correcting.

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** Original recommendation guesses a cross-reference target ('probably REG-058'); requires source-level verification before correcting.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-035

**Issue:** Which requirement prevails? PA-2 (rev 1) let Unknown become NotReceived on provider proof alone **vs** ontology §3 (two-source agreement)

**Proposed:** Two sources required (PA-2 rev 2)

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-036

**Issue:** Which requirement prevails? FI-05 permitted "approved, not reserved" **vs** FI-08 forbids it; FI-12 orders by approval, WF-17 FIFO, CW-03 by reservation time

**Proposed:** One ordering key: reservation sequence (GR-10); atomic approve+reserve for routine

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-037

**Issue:** Which requirement prevails? FI-12 allows partial **vs** FI-13 forbids partial unless policy **vs** FI-14 says exact amount

**Proposed:** Partial only by explicit sponsor policy; FI-12 and FI-14 cite FI-13

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-038

**Issue:** Which requirement prevails? GR-1 (fixed at approval) **vs** FI-15 (residual sweep fixed after last settlement)

**Proposed:** GR-1 rev 2 lists exceptions

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-039

**Issue:** Which requirement prevails? GR-5 (no unqualified "paid" before Final) **vs** status vocabulary "Paid to [Recipient] on D" at Disbursed

**Proposed:** "Payment sent on D" until Final

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-040

**Issue:** Which requirement prevails? SM-5 (a model never releases a hold) **vs** FLAG Superseded on model re-run

**Proposed:** Superseded for rule-raised flags only

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-041 | NEEDS CLARIFICATION

**Issue:** Which requirement prevails? Org §5.1 (advisor may delegate to an agent) **vs** DD-021 (only the donor may change delegation); INV-A6 lets an advisor confirm their own agent

**Proposed:** DEFERRED FOR USER CLARIFICATION: Independent confirmation by a person other than the delegator may conflict with donor-controlled delegations and accessibility; design must distinguish self-authorization, agent self-confirmation and valid principal confirmation.

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** Independent confirmation by a person other than the delegator may conflict with donor-controlled delegations and accessibility; design must distinguish self-authorization, agent self-confirmation and valid principal confirmation.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-042

**Issue:** Which requirement prevails? Organization §5.3 (a terminated staffer's approvals remain valid) **vs** SM-6 (authority re-checked at payment)

**Proposed:** SM-6 re-checks the *releaser's* authority and the grant state, not that of past approvers; recorded

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-043

**Issue:** Which requirement prevails? Invariant facts 5–6 and FI-14 (per-grant liquidation) **vs** WF-18 step 3 (unit redemption for released grants; pools trade net)

**Proposed:** FI-14 rev 2: Funding = internal unit redemption at strike + pool cash availability

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVC-044

**Issue:** Which requirement prevails? Red team: alternative 2D giving plan (pre-clearance) **vs** TVCTL-06 assurance at request time

**Proposed:** REC-7 re-confirmation at execution; AUTH-I7

**Why:** Conflicting specifications would let different components enforce different outcomes; one authoritative invariant and cross-reference avoids silent divergence.

**Challenge:** A single canonical rule may hide different lifecycle states, regulatory regimes, sponsor configurations or exception paths; check against existing owner decisions and legal gates.

**Verify:** Cross-reference both conflicting sources, map state transitions and exception paths, and run conflicting-input and historical-reconstruction scenarios.

### TVA-001

**Issue:** Is it true that: Board-delegated version-pinned policy approval satisfies the sponsor's legal approval of each routine grant

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-002

**Issue:** Is it true that: Exception share of grants needing human decision is about 8%

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-003

**Issue:** Is it true that: Minutes per exception about 45

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-004

**Issue:** Is it true that: December surge multiplier about 3x over ~25 working days

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-005

**Issue:** Is it true that: Payment providers offer idempotent submission and a query that can prove non-receipt

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-006

**Issue:** Is it true that: Custodians support cash-amount redemptions or per-grant liquidation at acceptable cost

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-007

**Issue:** Is it true that: Donors accept a provisional period before contributed ACH value funds grants

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-008

**Issue:** Is it true that: Contribution ACH return and fraud rate is low enough for a nonzero provisional credit limit

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-009

**Issue:** Is it true that: IRS public-charity status plus OFAC screening plus velocity rules catch sham-charity fraud adequately

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-010

**Issue:** Is it true that: AI flag recall exceeds human catch on benefit/related-party issues

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-011

**Issue:** Is it true that: Reviewers do not over-trust AI dossiers (automation bias controllable)

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-012

**Issue:** Is it true that: Authority changes can propagate to every derived session/token/template within ~60s

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-013

**Issue:** Is it true that: Partners will contractually commit to deprovisioning latency and display rules

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-014

**Issue:** Is it true that: Cash sleeve drag (~10 bps of AUM at 5% sleeve) is acceptable to donors and advisors

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-015

**Issue:** Is it true that: Recipient refunds and returns are rare enough to handle as cases

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-016

**Issue:** Is it true that: NAV-error materiality threshold practice from pooled vehicles transfers to DAF pools

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-017

**Issue:** Is it true that: Grant authority delegation tables (DoA) can be maintained as data and linked to decisions

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-018

**Issue:** Is it true that: Year-end peaks are handled by D path with no infrastructure strain

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-019

**Issue:** Is it true that: Contact volume falls with proactive status

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-020 | NEEDS CLARIFICATION

**Issue:** Is it true that: Shared determinations across sponsors are legally relied upon

**Proposed:** DEFERRED FOR USER CLARIFICATION: Cross-sponsor reliance on determinations cannot be assumed; scope, liability and sponsor accountability need legal and commercial evidence.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** Cross-sponsor reliance on determinations cannot be assumed; scope, liability and sponsor accountability need legal and commercial evidence.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-021

**Issue:** Is it true that: Agent ownership needs about 4 FTE flat regardless of volume

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-022

**Issue:** Is it true that: Practical SoD floor is ~6-8 people with absence coverage

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-023

**Issue:** Is it true that: Retention periods can be set per record class once counsel confirms

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-024

**Issue:** Is it true that: Custodian and bank feeds arrive daily and completely enough for continuous reconciliation

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-025

**Issue:** Is it true that: Donor-side AI agents will be a material channel

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-026

**Issue:** Is it true that: The open grant record projection is acceptable to recipients and partners

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-027

**Issue:** Is it true that: Recurring grant donors accept per-instance re-validation holds

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-028

**Issue:** Is it true that: Sanctions fuzzy matching recall is adequate for transliterated names

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-029

**Issue:** Is it true that: Specialists with ED/ER competence can be hired or contracted seasonally

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVA-030

**Issue:** Is it true that: A meaningful share of donors will submit giving plans ahead of December

**Proposed:** Treat as an unproven hypothesis, not an operating fact or guaranteed planning input.

**Why:** The claim determines operating capacity, compliance exposure, unit economics or partner feasibility; relying on an unmeasured assumption would turn a design hypothesis into an undocumented control.

**Challenge:** The hypothesis may fail by sponsor, asset class, season, user cohort or partner. Require measured baseline, confidence interval, adverse-case sensitivity and explicit fallback before use.

**Verify:** Collect attributable primary evidence or controlled experiment; model failure and sensitivity; record evidence owner and acceptance criteria.

### TVR-GR-001-Q

**Issue:** OD-13 sleeve vs reservation (TVR-GR-001: Grant lifecycle includes Reserved; Funding; Funded; FundingShortfall between Approved and payment)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-PAY-001-Q

**Issue:** Which providers can prove non-receipt (TVR-PAY-001: Payment attempt state machine with Unknown; NotReceived requires provider-defined proof)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-PAY-002-Q

**Issue:** ACH lacks end-to-end key (TVR-PAY-002: Provider selection requires idempotent submission and status query per contract)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-PAY-004-Q

**Issue:** Emergency payments during outage (TVR-PAY-004: No payment may be originated outside the platform payment gate; manual fallback runs through the gate with dual control)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-CON-001-Q

**Issue:** Ack timing (TVR-CON-001: Reversible-rail contributions are Provisional until the return window passes)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-CON-003-Q

**Issue:** Collection policy (TVR-CON-003: A reversed contribution that already funded disbursements creates a donor receivable and sponsor exposure case)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-FIN-002-Q

**Issue:** Haircut values (TVR-FIN-002: Reservations are currency encumbrances against capacity net of haircut; over-commitment detected after each NAV strike)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-SAN-001-Q

**Issue:** Blocking vs rejecting [C] (TVR-SAN-001: Donor or advisor sanctions listing freezes the fund and holds all grants)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-SAN-002-Q

**Issue:** Classes (TVR-SAN-002: No model clears any sanctions flag; rule-based clearance limited to enumerated classes with QA sampling)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-SEC-004-Q

**Issue:** Threshold values (TVR-SEC-004: Auto-approval ceiling bounded by session/principal assurance; aggregate thresholds per recipient and period defeat split)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-SEC-006-Q

**Issue:** Scale feasibility (TVR-SEC-006: Collusion-resistant detective controls (cross-role anomaly analytics; mandatory vacation/rotation for payment and reconc)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-ORG-003-Q

**Issue:** Small-scale compensation (TVR-ORG-003: Reconciliation accountable to a role outside the payment chain)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-CMP-003-Q

**Issue:** Model C vs D (TVR-CMP-003: Controls are conditioned on operating model (sponsor vs infrastructure provider vs custody))

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-WID-001-Q

**Issue:** Window value (TVR-WID-001: Widget data cleared within a stated window after authority revision change)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-WID-002-Q

**Issue:** Enforceability (TVR-WID-002: Partners may display sponsor financial figures only via sponsor widgets or signed data with as-of time)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-WID-006-Q

**Issue:** Value (TVR-WID-006: Contractual maximum end-to-end deprovisioning latency per partner tier)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-UX-003-Q

**Issue:** Thresholds (TVR-UX-003: Unusual-amount warning before submission (relative to history and fund))

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-UX-006-Q

**Issue:** Effect size (TVR-UX-006: Reviewer sees evidence before AI summary; seeded known-answer cases)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-DATA-001-Q

**Issue:** Mechanism (TVR-DATA-001: Reconstruct any decision's inputs and any balance as known at any recorded time (mechanism not prescribed))

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-REG-001-Q

**Issue:** Reliance scope [C] (TVR-REG-001: Precedence rule for conflicting IRS sources: most adverse current fact holds pending human)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-REG-003-Q

**Issue:** Signal quality (TVR-REG-003: New-recipient risk signals (registration age; payee age; first grant from sponsor) feed flags)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-GOV-001-Q

**Issue:** Merge strategy (TVR-GOV-001: Cross-reference map between docs/ requirement IDs and research/ai-native IDs)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-PAY-005-Q

**Issue:** Provider capability (TVR-PAY-005: Per-payment destination confirmation for provider-tokenized destinations (fingerprint vs sponsor-held verified data); mi)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-PAY-006-Q

**Issue:** Data minimisation tradeoff (TVR-PAY-006: Sponsor retains portable verified bank data in addition to provider tokens)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-PAY-007-Q

**Issue:** Thresholds (TVR-PAY-007: Bulk-Unknown procedure: graded evidence; dual-control sampling triage; never bulk NotReceived without per-item proof)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-SEC-007-Q

**Issue:** Accessibility balance (TVR-SEC-007: Assisted-channel commands (staff/phone/view-as) are a distinct low-assurance command kind; new recipients require sponso)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-SEC-008-Q

**Issue:** Custodian feed granularity (TVR-SEC-008: Standing-data rule (OD-22) extended to external registers: daily custody movement match; IdP admin action match; bank-po)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-SEC-009-Q

**Issue:** Performance (TVR-SEC-009: Release verifies decision input snapshot against policy and standing-data history (edit-then-revert detection))

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-SEC-010-Q

**Issue:** Accessibility of device-bound factors (TVR-SEC-010: Recipient novelty measured from last confirmation via a pre-existing channel; synced passkey alone insufficient for new )

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-DR-001-Q | NEEDS CLARIFICATION

**Issue:** RPO targets (TVR-DR-001: One consistent restore point across payment, decisioning, authority, evidence; zero-loss or re-derived security state; p)

**Proposed:** DEFERRED FOR USER CLARIFICATION: Zero-loss claims across distributed systems and evidence storage require a feasibility and cost decision, not a presumed achievable RPO.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Zero-loss claims across distributed systems and evidence storage require a feasibility and cost decision, not a presumed achievable RPO.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-CMP-004-Q

**Issue:** Blocking vs rejecting [C] (TVR-CMP-004: OFAC blocking/reject outcomes and reporting with 10-year records (TVCTL-13); third-party Blocked outcome)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-GR-006-Q

**Issue:** Supporting-org classification data quality (TVR-GR-006: Routine path excludes recipients requiring ER or creating taxable distributions and any payee that is a donor/advisor/re)

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVR-GR-008-Q

**Issue:** Look-back values (TVR-GR-008: Delayed execution (giving plan, recurring) re-confirms risk-based at execution; plan confirmation at clearance; visible )

**Proposed:** Preserve the stated control outcome; leave numerical thresholds, provider selection, legal interpretation and operational tuning as explicit gated parameters.

**Why:** The underlying requirement describes an important safety, auditability, usability or operational outcome, but the open question cannot be responsibly converted into an unsupported universal value.

**Challenge:** Overly restrictive defaults may block legitimate grants and accessibility; permissive defaults may permit fraud, loss or regulatory breaches.

**Verify:** Define an accountable owner, legal/technical evidence and adversarial simulation before setting the open parameter.

### TVF-001

**Issue:** How should the requirements answer: Grant state machine has no reservation or funding (liquidation) states; Approved goes straight to Released

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-003

**Issue:** How should the requirements answer: No Unknown state between payment instruction and provider acceptance in the grant machine

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-006

**Issue:** How should the requirements answer: ACH contributions treated as settled and spendable and acknowledged although R10 returns are allowed for ~60 days

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-009

**Issue:** How should the requirements answer: Unit earmark vs currency reservation undefined; NAV drop can underfund approved grants; no over-commitment rule

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-016

**Issue:** How should the requirements answer: Phishable session can originate auto-approved grants to new recipients

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-024

**Issue:** How should the requirements answer: Unknown payment states may never resolve because rails lack proof of non-receipt

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-027

**Issue:** How should the requirements answer: Donor (not recipient) sanctions listing does not freeze fund activity

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-047

**Issue:** How should the requirements answer: DD-004 routine auto-approval lacks a linked board delegation and counsel review

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-048

**Issue:** How should the requirements answer: Sanctions guidance text permits automated closure of low-score matches

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-131

**Issue:** How should the requirements answer: Security §8 lists two-person collusion as residual, mitigated only by 'sampling and internal audit'

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-139

**Issue:** How should the requirements answer: CTL-031 says JIT exists, not what JIT may write

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-144

**Issue:** How should the requirements answer: DR tiers put the payment gate (T0) and grant decisioning (T1) at different RPOs and assign no tier to the evidence store

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-149

**Issue:** How should the requirements answer: Class-level (bulk) decisions are unspecified

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-153

**Issue:** How should the requirements answer: Giving-plan grooming chain: Q3 account takeover pre-clears a real attacker-controlled charity; December auto-execution bypasses step-up; velocity and release re-checks

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-222

**Issue:** How should the requirements answer: All payee-integrity controls inspect sponsor-side data

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-223

**Issue:** How should the requirements answer: OD-22 and TVR-PAY-004 cover platform data and payments, not custodial asset movements or other external authority registers

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-226

**Issue:** How should the requirements answer: B6 allows security state to be re-derived from external registers, but B19 matching needs the sponsor's own record of authorized changes, which B6 does not place in the zero-loss t

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-227

**Issue:** How should the requirements answer: B1 and A12 make protective requests always available, but B6's zero-loss tier omits them

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-228 | NEEDS CLARIFICATION

**Issue:** How should the requirements answer: B6 creates break-glass for the period when the command service is down; OD-22 permits break-glass only through a domain command, which cannot exist then

**Proposed:** DEFERRED FOR USER CLARIFICATION: Command service outage makes command-only break-glass unavailable; a separate independently controlled emergency command path or freeze-only procedure must be designed and tested without allowing direct data edits.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** Command service outage makes command-only break-glass unavailable; a separate independently controlled emergency command path or freeze-only procedure must be designed and tested without allowing direct data edits.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-229

**Issue:** How should the requirements answer: No rule says how the incident window is bounded

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-230

**Issue:** How should the requirements answer: B6's consistent restore point includes the evidence store, so a restore can delete evidence

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-231

**Issue:** How should the requirements answer: PA-3 requires a stable key but not a unique one across restores, and B18 cannot tell two payments to the same destination apart

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-232

**Issue:** How should the requirements answer: B19 covers bank-portal entitlements only through periodic review, while custodian and IdP actions are matched daily

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-233

**Issue:** How should the requirements answer: B19 matches custodian movements to commands but does not require destination comparison, and settlement instructions held at the custodian are not named as a register

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-234 | NEEDS CLARIFICATION

**Issue:** How should the requirements answer: B18 accepts 'account fingerprint or name-match result', but a fingerprint computed by the provider is controlled by the party whose compromise B18 is meant to detect

**Proposed:** DEFERRED FOR USER CLARIFICATION: Provider-produced destination fingerprints cannot independently establish payee integrity; needs a trusted sponsor-side or independent evidence source and a defined mismatch-resolution process.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** Provider-produced destination fingerprints cannot independently establish payee integrity; needs a trusted sponsor-side or independent evidence source and a defined mismatch-resolution process.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-235

**Issue:** How should the requirements answer: Every 'sponsor-direct' and 'verified channel' control assumes the sponsor's own domain and messaging identities are intact

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-236

**Issue:** How should the requirements answer: Provider evidence is checked against the sponsor's bank; custody has no independent second source in any control

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-237

**Issue:** How should the requirements answer: B12 counts banks, not independent infrastructure

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-238

**Issue:** How should the requirements answer: No rule reacts to changes in who controls a third party

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-239

**Issue:** How should the requirements answer: B3 repeats the waitable-window pattern that B2 removed for novelty

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-240

**Issue:** How should the requirements answer: B16 does not say when exposure is measured

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-241

**Issue:** How should the requirements answer: TIME-1 names calendars and zones but not window type

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-242

**Issue:** How should the requirements answer: A12 exempts observation recording from fail-closed without saying which clock stamps it

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-243

**Issue:** How should the requirements answer: B2 and B4 do not say how they interact

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-244

**Issue:** How should the requirements answer: B7 requires re-verification but not how

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-245

**Issue:** How should the requirements answer: B13 makes the fallback safe on average, but the requester can trigger it

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-246

**Issue:** How should the requirements answer: B15 creates a triage queue with no aging rule

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-247

**Issue:** How should the requirements answer: B9 and B16 do not say how they interact

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-248

**Issue:** How should the requirements answer: B5 restricts what a warning can clear, but not who can raise holds at scale

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-249

**Issue:** How should the requirements answer: PA-2 and A10 count sources, not independent sources

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-250

**Issue:** How should the requirements answer: B18 defines the block but not how a mismatch is resolved; the obvious remedy (accept the provider's value) undoes B11 and B18

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-251

**Issue:** How should the requirements answer: B2 fixed waiting-out for recipients but not for authority grants

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-252

**Issue:** How should the requirements answer: SM-6 re-checks authority, eligibility, holds and freshness, not amount thresholds; A6 fixes the amount after approval

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-253

**Issue:** How should the requirements answer: A7 caps the top-up per return, not across cycles

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### TVF-254

**Issue:** How should the requirements answer: B6's security state list (revocations, holds, clocks, flags) does not name keys, secrets and machine-client registrations

**Proposed:** Adopt the identified protective requirement as a proposed design control, subject to architecture and operational validation; fail closed for unauthorized value movement.

**Why:** The finding identifies a concrete exploit path, missing invariant or unsafe reconciliation/authority dependency; an explicit guard is preferable to relying on staff vigilance.

**Challenge:** A local fix may introduce bypasses through third parties, recovery paths, stale data, accessibility channels or correlated evidence sources.

**Verify:** Threat-model the end-to-end path, define independent evidence and recovery, test adversarial bypasses, outages and legitimate-user impact.

### EXT-01

**Issue:** Do our candidate payment providers offer idempotent submission, a status query, a stated deduplication window, per-item observability and a destination fingerprint?

**Proposed:** Create a specific external-evidence gate; do not claim the answer is known.

**Why:** This depends on third-party capabilities, empirical measurements, legal texts or user testing outside the current theoretical model.

**Challenge:** Marketing statements, vendor demonstrations and single-sponsor samples may not generalize or constitute enforceable proof.

**Verify:** Obtain primary contract/API/rule text, representative data or documented user study; independently verify and track expiry.

### EXT-02

**Issue:** Can custodians report every movement (including settlement instructions) daily, and support cash-amount unit redemptions?

**Proposed:** Create a specific external-evidence gate; do not claim the answer is known.

**Why:** This depends on third-party capabilities, empirical measurements, legal texts or user testing outside the current theoretical model.

**Challenge:** Marketing statements, vendor demonstrations and single-sponsor samples may not generalize or constitute enforceable proof.

**Verify:** Obtain primary contract/API/rule text, representative data or documented user study; independently verify and track expiry.

### EXT-03

**Issue:** What do bank portals log (positive-pay decisions, debit filters, wire templates), and can it be exported daily?

**Proposed:** Create a specific external-evidence gate; do not claim the answer is known.

**Why:** This depends on third-party capabilities, empirical measurements, legal texts or user testing outside the current theoretical model.

**Challenge:** Marketing statements, vendor demonstrations and single-sponsor samples may not generalize or constitute enforceable proof.

**Verify:** Obtain primary contract/API/rule text, representative data or documented user study; independently verify and track expiry.

### EXT-04

**Issue:** Measured exception rate, reviewer minutes and December daily volumes from a real sponsor

**Proposed:** Create a specific external-evidence gate; do not claim the answer is known.

**Why:** This depends on third-party capabilities, empirical measurements, legal texts or user testing outside the current theoretical model.

**Challenge:** Marketing statements, vendor demonstrations and single-sponsor samples may not generalize or constitute enforceable proof.

**Verify:** Obtain primary contract/API/rule text, representative data or documented user study; independently verify and track expiry.

### EXT-05

**Issue:** Contribution ACH return and fraud rates in the target donor segment

**Proposed:** Create a specific external-evidence gate; do not claim the answer is known.

**Why:** This depends on third-party capabilities, empirical measurements, legal texts or user testing outside the current theoretical model.

**Challenge:** Marketing statements, vendor demonstrations and single-sponsor samples may not generalize or constitute enforceable proof.

**Verify:** Obtain primary contract/API/rule text, representative data or documented user study; independently verify and track expiry.

### EXT-06

**Issue:** Nacha rule text for return windows (R10/R29 and the 2024 amendment)

**Proposed:** Create a specific external-evidence gate; do not claim the answer is known.

**Why:** This depends on third-party capabilities, empirical measurements, legal texts or user testing outside the current theoretical model.

**Challenge:** Marketing statements, vendor demonstrations and single-sponsor samples may not generalize or constitute enforceable proof.

**Verify:** Obtain primary contract/API/rule text, representative data or documented user study; independently verify and track expiry.

### EXT-07

**Issue:** Sanctions-matching recall on transliterated names

**Proposed:** Create a specific external-evidence gate; do not claim the answer is known.

**Why:** This depends on third-party capabilities, empirical measurements, legal texts or user testing outside the current theoretical model.

**Challenge:** Marketing statements, vendor demonstrations and single-sponsor samples may not generalize or constitute enforceable proof.

**Verify:** Obtain primary contract/API/rule text, representative data or documented user study; independently verify and track expiry.

### EXT-08

**Issue:** Usability of the five-dimension status copy with low-tech, screen-reader, bereaved and charity users

**Proposed:** Create a specific external-evidence gate; do not claim the answer is known.

**Why:** This depends on third-party capabilities, empirical measurements, legal texts or user testing outside the current theoretical model.

**Challenge:** Marketing statements, vendor demonstrations and single-sponsor samples may not generalize or constitute enforceable proof.

**Verify:** Obtain primary contract/API/rule text, representative data or documented user study; independently verify and track expiry.

### EXT-09

**Issue:** Will donors and advisors accept giving plans, provisional periods and cash allocations?

**Proposed:** Create a specific external-evidence gate; do not claim the answer is known.

**Why:** This depends on third-party capabilities, empirical measurements, legal texts or user testing outside the current theoretical model.

**Challenge:** Marketing statements, vendor demonstrations and single-sponsor samples may not generalize or constitute enforceable proof.

**Verify:** Obtain primary contract/API/rule text, representative data or documented user study; independently verify and track expiry.
