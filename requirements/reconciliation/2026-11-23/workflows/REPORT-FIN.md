# Workflow modelling report: FIN (WFM-03, 04, 05, 14, 15, 16, 17, 18)

Generator: /tmp/claude-0/-home-user/179dd429-c49d-5559-a2b4-4866d79585b5/scratchpad/gen_fin.py
Output: /home/user/unbound/requirements/reconciliation/2026-11-23/workflows/

## 1. Workflows and checker status

| File | States | Transitions | Checker |
|---|---|---|---|
| WFM-03-cash-contribution.json | 22 | 29 | 0 ERROR |
| WFM-04-securities-contribution.json | 25 | 34 | 0 ERROR |
| WFM-05-complex-asset-intermediary.json | 19 | 20 | 0 ERROR |
| WFM-14-pool-valuation-unit-pricing.json | 17 | 18 | 0 ERROR |
| WFM-15-fee-accrual-settlement.json | 9 | 13 | 0 ERROR |
| WFM-16-reconciliation-close-statements.json | 13 | 16 | 0 ERROR |
| WFM-17-annual-daf-reporting.json | 16 | 19 | 0 ERROR |
| WFM-18-document-issuance-retention.json | 15 | 18 | 0 ERROR |

`python3 -I tools/reqaudit/workflows.py .` (all 13 files at run time): 0 errors overall. 26 W11 warnings in these files: guarded or failure-bearing transitions with no matching scenario (mostly WFM-05 intermediary steps and WFM-17 filing steps, where no DKS, ADV, SIM or TVS scenario exists).

## 2. Workflow defects found in the requirements

1. **When to acknowledge a reversible rail is contradictory.** RCD-019 says to issue the acknowledgment on CompletedGiftDetermined ∧ SponsorAccepted. state-models.md §5 CON-2 (research/.../06-state-models/state-models.md:203) says to acknowledge a reversible rail only after Final, or only with a tested correction path. Model: a sponsor policy `ack-timing` with two values. OnAcceptance gives WFM-03-T17 and a void on return (T25). AfterFinal is the default and gives T24. **Needs decision.**
2. **Provisional credit is inconsistent.** RR-UNB-ACC-006-r1 permits provisional credit under OD-04. RCD-019 says credit happens only on Final. Model: no provisional credit at R1, noted on the finality transitions. RR-UNB-ACC-006-r1 should drop the OD-04 exception or cite RCD-019.
3. **The Unknown protocol covers payments out only.** UNB-RCN-GRT-008 and RR-REQ-X-004-r1 define Unknown resolution for outbound payments. ACH debit pulls and return-to-sender have no protocol. Model: WFM-03-T04 and T11–T13, and WFM-04-T11/T12, reuse it. This is a gap (see §3).
4. **Who the donee is in the intermediary structure is undecided** (RCD-013, RCD-011). If the intermediary acts as the sponsor's agent, the sponsor is the donee. It must then acknowledge the gift and sign Form 8283, and the intermediary's sale creates a near-certain Form 8282 obligation (UNB-RCN-FIN-003). The §4943 disposal tracking also applies to the sponsor. If the intermediary is the donee, the cash is an inbound charitable transfer (UNB-RCN-ENT-007). Model: two exclusive transitions, WFM-05-T18 and T19, guarded by `structure-is-agent` / `structure-is-donee`. **Needs counsel decision.**
5. **NAV approval authority is unresolved.** UNB-NAV-008 requires a maker-checker for every published NAV. No decision delegates routine in-tolerance NAVs to a deterministic policy, unlike RCD-005 for grants and RCD-020 for reconciliation. Model: a human A4 approves every NAV (WFM-14-T11). This creates daily human work. **Needs decision.**
6. **Fee shortfall priority (OD-10) is undecided.** It is unclear whether a minimum fee or a reserved grant takes precedence (DKS-FIN-022, TVS-050). Model: a human A4 decides the treatment (WFM-15-T11). The fee never draws on reserved capacity. **Needs decision.**
7. **Whether sampled attestation gates close is unstated.** RCD-020 sets "≥5% sampled attestation" but does not say whether it gates certification. Model: it gates certification (WFM-16-T08/T09). **Needs decision.**
8. **Statement approval wording conflicts.** REQ-X-032 asks for a "distinct approval" state, and ADV-270 rejects publication before independent approval. REQ-X-040 allows automatic issuance under sponsor delegation. Model: approval is CertifyClose plus the delegation (WFM-16-T13 note). The requirement text should say this.
9. **Closing a partial multi-lot securities delivery is not defined.** DKS-REG-006 withholds the acknowledgment but gives no exit. Model: a timer closes the partial receipt (WFM-04-T06).
10. **Intent records have no expiry.** No requirement makes an intent lapse. Model: timers WFM-03-T28 and WFM-04-T33, citing UNB-ACC-006.

## 3. Requirement gaps (proposed statements)

- **G1 (WFM-03-T04, T11–T13; WFM-04-T11, T12):** "Each inbound collection rail (ACH debit) and each outbound refund or redelivery of never-accepted property MUST have an UnknownResolutionProtocol equivalent to UNB-RCN-GRT-008. Re-initiation after a proven NotSubmitted MUST reuse the original idempotency key."
- **G2 (WFM-03-T28, WFM-04-T33):** "A contribution intent with no receipt MUST lapse after a sponsor-set window and notify the declarant. A receipt after lapse MUST be treated as unannounced."
- **G3 (WFM-04-T06):** "A multi-lot securities intent MUST close as partial after a sponsor-set window or donor confirmation. Acknowledgment and Form 8283 then cover only the received lots."
- **G4 (WFM-03-T17, T24, T25):** "The sponsor MUST configure acknowledgment timing for reversible rails (OnAcceptance | AfterFinal; default AfterFinal). OnAcceptance requires a void or correction path on return."
- **G5 (WFM-05-T18/T19):** "Each intermediary agreement MUST record a counsel-approved structure (agent | donee) that determines the donee of record, acknowledgment, 8283/8282 and §4943 duties."
- **G6 (WFM-17-T05):** "The annual reporting obligation MUST track the due date, any extension and escalation tiers."
- **G7 (WFM-17-T18/T19):** "An amended annual package MUST pass the same distinct prepare, review and sign states as the original."
- **G8 (WFM-18-T09/T10):** "A mandatory-legal document whose delivery failed MUST be retried on another verified channel before it is closed as portal-only."

## 4. NEW surfaces

- **NEW:Price exception review:** a pricing reviewer approves a fallback gift-date or NAV price (WFM-04, WFM-14).
- **NEW:NAV review and approval:** an independent checker approves, overrides or suspends a NAV, or approves a restatement.
- **NEW:Pool liquidity buffer:** treasury approves buffer replenishment.
- **NEW:Form 8283 donee signature:** an authorized signatory signs Section B, which acknowledges receipt and not value.
- **NEW:Form 8282 obligation tracker:** tax compliance records the 8282 filing and the donor copy.
- **NEW:Gift acceptance decision:** a gift committee declines a complex gift or approves it with a holdings plan.
- **NEW:Fee waiver decision:** finance requests a waiver, and an independent approver approves or rejects it.
- **NEW:Reconciliation attestation review:** an independent reviewer attests the sampled automated passes.
- **NEW:Month-end close certification:** the controller (not the preparer) certifies, restates or issues corrected statements.
- **NEW:Annual DAF reporting workspace:** the preparer, reviewer, signer and tax staff each record their own distinct step, plus filing and acceptance.
- **NEW:Account-type classification register:** counsel approves the DAF classification register.
- **NEW:Legal hold management:** counsel places a legal hold, and a different counsel releases it.
- **NEW:Retention disposition batch approval:** the records manager and a second approver approve a class-level disposition batch.
