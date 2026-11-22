# Unbound: complete question-coverage consistency review
Date: 2026-11-22
Status: Working theoretical requirements review, **not** approved architecture, implemented control, legal opinion or tested simulation.

## Coverage
- Authoritative typed registry: `open-questions.typed.json`.
- 183/183 questions now carry a cross-requirement consistency-review event. First pass: 128 unique questions across nine clusters. Second pass: 55 additional unique questions across seven clusters (56 group memberships because TVF-251 overlaps an earlier cluster).
- The source registry contains 22 owner decisions, 44 contradictions, 30 assumptions, 33 open requirement questions, 16 severity-5 findings, 29 other open findings and 9 external-evidence gates.
- This is **question coverage**, not proof that every one of the 16,653 possible pairs was independently inspected. Specific high-risk dependencies were evaluated. Cross-document edits and formal validation remain open.

## Expanded consistency areas

### 10. Donor death, succession, closure and delayed execution
IDs: OD-03, OD-11, TVC-030, TVC-033, TVC-042, TVC-044, TVR-GR-008-Q.

**Rule:** Distinguish sponsor-approved unpaid grants from future recurring or giving-plan instructions. Approved grants continue subject to sponsor holds; future instructions suspend on death or advisor-authority uncertainty until verified succession. Past staff approvals remain historical facts; payment release checks current authority, risk and holds. Post-closure receipts use governing documents and documented successor or charitable disposition.

**Challenge:** Late notice of death may require effective-time and knowledge-time reconstruction; it must not erase legitimate historic approvals or enable unverified future execution.

**Validation:** Test death before approval, after reservation, before payment, during payment Unknown, and after closure, including contested successor authority.

### 11. Contribution rail states, ACH returns and collections
IDs: TVC-010, TVA-015, TVR-CON-001-Q, TVR-CON-003-Q, TVF-006, EXT-06.

**Rule:** Model contribution acceptance, receipt, provisional availability, valuation, allocation, acknowledgment and rail finality separately. Reversible payments may receive risk-limited provisional capacity under OD-04, but a return creates an explicit receivable and sponsor-exposure case. Do not infer the tax acknowledgment date from mere UI receipt.

**Challenge:** A universal '60-day return window' may be wrong across ACH return codes, account types and legal rules. No fixed return period is adopted without primary Nacha and counsel evidence.

**Validation:** Simulate ACH return before and after grant funding, partial return, fraud return, contested donor collection, and acknowledgment corrections.

### 12. Risk assurance, recipient novelty, authority changes and evasion
IDs: TVA-012, TVR-SEC-004-Q, TVR-UX-003-Q, TVF-016, TVF-239, TVF-240, TVF-241, TVF-243, TVF-251.

**Rule:** Reassess recipient and principal novelty, device/session assurance, policy thresholds, cumulative grant amount and projected exposure at submission and release. New authority and identifier remaps restart relevant risk windows; sliding-window thresholds resist transaction splitting. A UI warning is not an authorization control.

**Challenge:** Waiting out a window, adding an authorized principal, splitting grants and repeatedly triggering fallback can bypass local controls. Avoid arbitrarily excluding legitimate low-tech users.

**Validation:** Multi-account splitting, step-up factor failure, new advisor, credential remap, time-zone boundaries, and exposure before/after return.

### 13. Collusion, incident timing, independent evidence and recovery
IDs: TVF-131, TVF-229, TVF-235, TVF-236, TVF-242, TVF-244, TVF-245, TVF-246, TVF-247, TVF-248, TVF-253.

**Rule:** Threat-model compromised sponsor domains and two-person collusion. Establish incident lookback and triage aging. Treat time-uncertain observations as provisional evidence. Verify actual control independence between provider, bank, custodian and messaging sources. Rate-limit malicious hold creation, and cap cumulative exposure across repeated returns.

**Challenge:** Two apparently distinct evidence sources can be owned by the same provider or compromised together. A security fallback controlled by the requester is not a reliable fallback.

**Validation:** Simulated provider compromise, sponsor DNS/email takeover, clock skew, collusion, stale triage, bulk malicious flags, repeated refund loops.

### 14. Finance, vendor capability, sponsor-vs-infrastructure roles and operating scale
IDs: TVC-013, TVA-001, TVA-005, TVA-006, TVA-017, TVA-024, TVA-029, TVR-GR-001-Q, TVR-FIN-002-Q, TVR-SEC-006-Q, TVR-CMP-003-Q, TVR-PAY-006-Q, TVR-SEC-008-Q, TVR-SEC-009-Q.

**Rule:** Domain P0 workflows require corresponding staff UI. Sponsor and infrastructure-only operating models have different legal and control responsibilities. No unverified provider proof-of-nonreceipt, custodian cash redemption, daily complete feed, staffing floor, haircut or legal board-delegation assumption is treated as fact. Keep independent review and external register matching; minimize retained bank information while maintaining portable verification.

**Challenge:** Cost and operational feasibility vary by sponsor scale and partner capabilities. Avoid selecting unsupported numeric parameters.

**Validation:** Primary contract/API tests, legal advice, cost stress tests, feed completeness sampling and SoD absence coverage.

### 15. Recipient legal classification, sanctions and reviewer bias
IDs: TVA-010, TVA-011, TVR-UX-006-Q, TVR-REG-001-Q, TVR-REG-003-Q, TVR-CMP-004-Q, TVR-GR-006-Q, EXT-07.

**Rule:** Separate sanctions blocking versus rejection and the applicable reporting/recordkeeping obligations. Adverse conflicting eligibility evidence holds payment pending adjudication. Supporting-organization and expenditure-responsibility cases leave the routine path until validated. Test model recall, transliterated-name screening and reviewer automation bias with blinded cases.

**Challenge:** The most adverse data source can be wrong; provide sponsor-controlled appeals and evidence resolution without letting AI clear legally required holds.

**Validation:** Sanctions counsel review, verified test sets, error rate/recall evaluation and human-factors experiments.

### 16. Partner deprovisioning
ID: TVA-013.

**Rule:** Partner revocation latency and display obligations remain unverified until enforceable contract terms and actual end-to-end revocation tests exist.

**Challenge:** Data already exported or displayed by a third party may be impossible to retract. The architecture should limit disclosure, not promise retroactive deletion.

**Validation:** Partner contract, API access cut-off test, stale-widget test, revocation and export handling.

## Integrated invariants: required across all domains
1. **No release without contemporaneous authority:** Sponsor policy approval, delegator scope, recipient eligibility, risk state and funding all checked at execution.
2. **No money movement without independently verifiable provenance:** Command identity, amount, destination and external execution must reconcile.
3. **No inferred finality:** An acknowledgment, provider acceptance or timeout cannot substitute for final settlement evidence.
4. **No double counting:** Capacity, reservations, fees, liquidity and receivables are reconciled as distinct accounting facts.
5. **No lost security state on recovery:** Holds, revocations, protective cancellations, payment IDs and evidence survive or are independently reconstructed before release.
6. **No hidden delegation:** AI agents, partner widgets, staff assisted channels and successor advisors never gain more authority than the underlying verified principal.
7. **No unchecked legal assumptions:** Board delegation, charity eligibility, sanctions disposition, grant recognition and retention remain counsel/CFO/board-gated as applicable.
8. **No conflated UI status:** Submitted, approved, reserved, funded, payment sent and final are distinct and must be described accurately.
9. **No single-source independence fiction:** Independence is evaluated by ownership, control, infrastructure and failure mode.
10. **No unsupported economics:** Volume, exception rate, staffing, cash drag and vendor capability remain scenario inputs until measured.

## Cross-domain failure simulations to specify
| ID | Scenario | Required result |
|---|---|---|
| XSIM-01 | Approved grant, NAV collapse, pending payment | Reserve/funding shortfall handled without unauthorized payment |
| XSIM-02 | ACH return after grant paid | Receivable, loss allocation and recovery; no ledger disappearance |
| XSIM-03 | Donor dies before recurring grant execution | Suspend and re-check succession; preserve prior valid grants |
| XSIM-04 | Payment provider times out and retry requested | Unknown blocks retry until sufficient independent non-receipt proof |
| XSIM-05 | Provider substitutes destination after sponsor confirmation | Independent mismatch detection, payment hold and incident record |
| XSIM-06 | Restore from backup after security hold and payment instruction | Holds and unique payment IDs survive; release blocked until verified |
| XSIM-07 | Custodian console changes settlement destination | External register reconciliation, incident escalation and release hold |
| XSIM-08 | Agent creates new advisor then confirms its own grant | Privilege escalation blocked; human principal authority required |
| XSIM-09 | Charity passes 180-day review then sanctioned before payment | Execution-time screening holds release |
| XSIM-10 | Screen-reader donor uses assisted approval | Equivalent authorization controls without inaccessible step-up |
| XSIM-11 | Colluding employees and compromised sponsor email | Independent evidence, anomalies and dual-control limits prevent undetected movement |
| XSIM-12 | Year-end grant approved, funded and paid in different periods | Separate accounting and donor-facing events, immutable correction |

These are **scenario specifications**, not tests that have been executed.

## Issues requiring a decision or independent evidence
**Held for final user review (unchanged):** TVC-034, TVC-041, TVA-020, TVR-DR-001-Q, TVF-228, TVF-234.

**External evidence:** EXT-01 through EXT-09. Contractual capabilities, legal texts, real sponsor performance, user testing and sanctions recall are not yet verified.

**Potential architectural bottlenecks:** independent emergency command service, durable globally unique payment IDs across disaster recovery, truly independent provider/custody evidence, automated 180-day charity re-verification, partner deprovisioning, historical reconstruction.

## What this review does not claim
No source documents were rewritten to resolve all contradictions; no ADR, control, simulation or contract test was completed; no legal interpretation is approved. All registry decisions remain proposals. The next meaningful step is to reconcile canonical requirement texts and attach verification criteria, then test the cross-domain scenarios and bring the six deferred questions together at the end.
