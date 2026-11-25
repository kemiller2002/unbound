# 03 — Deadline-sensitive operations and financial failure scenarios

Status: **proposed analysis** · Evidence tags: [F] repository fact with path; [R] daf-landscape research (cited, not re-verified); [I] inference.

## 1. Deadline-sensitive operations

| Deadline | Operations | Evidence | Architectural consequence |
|---|---|---|---|
| Calendar year-end tax deduction (contributions) | OPC-001, OPC-002, OPC-003 | Fidelity 2025 year-end cut-offs ran from Nov 10 (outside mutual funds) to Dec 31 (web); "the tax receipt is issued at contribution, so year-end pressure falls on contribution intake" [R: daf-landscape `research/mission-2026-11-14/industry-incentives-structural-analysis.md:218-225`] | Intake availability matters most in late December; but gift date is set by instrument rules (CNV-FIN-003, UNB-RCN-FIN-012), never by receipt time (UNB-AVL-002, UNB-AVL-012) |
| Securities completed-gift date | OPC-002 | Date received in sponsor custody [R: `research/ai-native-daf-enterprise/03-workflow-redesign/wf-contributions.md:133`] | Receipt for a Dec 29 transfer must not imply a December gift; custody cut-off published in year-end calendar |
| Grant timing in December | OPC-004/005/006/008 | Vanguard Charitable: more than a quarter of CY2025 grants Nov 1–Dec 31 [R: `direction-policy-lobbying-obbba.md:201`]; grants "may take 10 or more business days" at peak [R] | Grants are not tax events for the donor, but charity and sponsor expectations peak; processing-delay objective by priority (UNB-AVL-014) |
| Settlement and holidays | OPC-008, OPC-001 | Dec 25 and Jan 1 holidays; a grant approved Dec 30–31 cannot be paid in the calendar year; December ACH contributions may not be grantable until ~February [R: `theoretical-enterprise-validation/13-adversarial-reviews/raw/review-fin-ops.md:262-272`] | Business calendars in deadlines (EAR-CMD-016); year-end calendar per rail (UNB-AVL-012) |
| December surge | All durable classes | ~3× surge modelled day 330–365 [R: `ai-native-daf-enterprise/11-scenarios-and-validation/digital-twin.md:31`]; December interval arrivals unmeasured [F: CNV-EVB-001] | Load test the durable path at the declared peak before each December (UNB-AVL-013); quotas and priority (UNB-AVL-006) |
| Investment cut-offs | OPC-013 | Custodian cut-offs per sponsor; Unbound relays only (CNV-LEG-001) | Acceptance after cut-off executes next window; status says so |
| Statement and acknowledgment deadlines | OPC-021, acknowledgment issuance | CNV-PLT-005; CNV-FIN-003 acknowledgment date | Long-running per-fund items; breaks block only affected funds (CNV-FIN-004) |

## 2. Realistic financial failure scenarios (challenge cases)

| # | Scenario | Risk without controls | Required behaviour | Requirements |
|---|---|---|---|---|
| S-A1 | Donor double-taps "Contribute" at 23:59 Dec 31 on a slow connection | Two ACH debits; two acknowledgments | One intent per idempotency key; second tap returns the same receipt | EAR-API-005, EAR-PRS-006, UNB-AVL-003 |
| S-A2 | Intake commits at 23:59:58 but response is lost; client retries at 00:00:03 Jan 1 | Client believes submission missed the year; duplicate | Retry returns original receipt with original acceptance time; gift date still by instrument | UNB-AVL-002, UNB-AVL-012 |
| S-A3 | Deploy at 23:50 Dec 31 | Dropped requests at peak | Rolling deploy with drain; or change freeze declared in December readiness gate | EAR-AVL-003, CNV-SVC-002 |
| S-A4 | Database failover at 23:55 Dec 31 (~1–2 minutes) | Silent loss if intake buffers in memory | Honest refusal; client retains draft and retries; status surface explains | UNB-AVL-011 |
| S-A5 | Two advisors on a joint fund recommend grants totalling more than available | Overdraft / negative fund | Per-fund ordering + atomic reservation: one declined | UNB-AVL-005, REQ-X-002 |
| S-A6 | Charity auto-revoked between recommendation and release | Grant to ineligible organization | Execution-time eligibility check within freshness window, fail safe when stale | UNB-AVL-004, CNV-GRT-003 |
| S-A7 | Rail times out after accepting a grant payment file | Duplicate payment on retry | OutcomeUnknown, trace protocol, no automatic retry | CNV-PAY-004, EAR-CMD-014 |
| S-A8 | Restore from backup to 10 minutes ago | Re-dispatch of already-sent instructions | Dispatch blocked until holds re-applied and in-flight reconciled | RR-UNB-RES-003-r2, CNV-ARC-001 |
| S-A9 | Fee run crashes after charging half the funds | Double-charging on rerun | Per-fund idempotent items (RR-ADM-028-r1) | EAR-CMD-018 |
| S-A10 | Sponsor publishes a new mandatory policy at 17:00 while 200 recommendations are queued | Grants executed under unacknowledged policy, or mass rejection | Per-operation policy (hold until acknowledged within deadline, or reject) — owner decision U-10 | UNB-AVL-004, UNB-AVL-023 |
| S-A11 | Sponsor-specific migration fails during a rolling release in December | All sponsors down | Failed tenant stops; others serve (CNV-ARC-005) | EAR-AVL-012 |
| S-A12 | AI assistant retries a prepared grant after a timeout | Duplicate recommendation | Agent must reuse key; no blind replay | UNB-AVL-027, AIN-020 |

## 3. Scenarios that cannot be resolved automatically

S-A7 when the rail offers no conclusive trace (two-approver resolution, UNB-RCN-WF-022); S-A8 reconciliation
release (operator); S-A10 policy choice (owner decision); S-A11 diagnosis of the failed tenant migration.
