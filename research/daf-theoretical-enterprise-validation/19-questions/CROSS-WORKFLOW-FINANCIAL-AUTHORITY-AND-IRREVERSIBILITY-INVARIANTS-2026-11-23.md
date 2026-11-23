# Cross-workflow transaction, authority and irreversibility invariants
Date: 2026-11-23. Proposed acceptance requirements only. Not tested.

This document tightens the 18-workflow completeness audit into enforceable single-system invariants. The **single authoritative financial writer**, sponsor-scoped policy and rights, and no-shadow-state rules apply to all operating models.

| ID | Invariant | Condition | Boundary | Expected safe behavior | Workflows |
|---|---|---|---|---|---|
| INV-01 | No grantable balance from donor intent | Contribution intent, unsettled cash or unaccepted asset cannot increase grantable capacity | Contribution/Gift Acceptance -> Ledger | Reject or stage; no capacity write | WF-03/04/07 |
| INV-02 | No double reservation | Two grants competing for same fund liquidity cannot both reserve beyond available funds | Grant -> Ledger | Atomic reservation/optimistic conflict; one fails safely | WF-07/08 |
| INV-03 | No premature payment finality | Provider accepted or check mailed is not proof of recipient receipt/settlement | Payment -> Ledger/Reporting | Separate states and evidence; Unknown remains open | WF-09/10 |
| INV-04 | No automatic retry on Unknown | Timeout or lost callback does not authorize a second disbursement | Provider Adapter -> Payment | Query/reconcile by idempotency key; escalate | WF-09/10 |
| INV-05 | No stale policy bypass | Sponsor policy or charity adverse hold change must revalidate pending grant/payment | Policy -> Grant/Payment | Recheck at defined irreversible boundary; hold | WF-06/07/08/15 |
| INV-06 | No shadow balances | Advisor/charity/external checkout cannot post or calculate authoritative funds separately | Ledger -> Views | Read scoped projections; reject direct writes | WF-05/09/10 |
| INV-07 | No conflicting writers on migration | Legacy and Unbound cannot both post authoritative transactions at cutover | Migration -> Ledger | Fencing and reconciliation; compensation after irreversible commit | WF-17 |
| INV-08 | No retroactive report overwrite | Closed period and issued statement remain as-issued despite later corrections | Ledger/Reporting -> Artifacts | New adjustment/reissue version with links | WF-05/13/14 |
| INV-09 | No role conflation | Advisor, donor, sponsor reviewer and charity contact rights do not merge through shared identity | Identity -> All Commands | Sponsor-scoped delegated permission check | WF-01/02/11/18 |
| INV-10 | No undisclosed related-party fee | Fee posting with conflict/recusal requirement cannot skip independent approval | Governance -> Fee/Ledger | Hold posting or governed exception | WF-12/14 |
| INV-11 | No bank-change reroute | Pending payment cannot silently adopt recipient's newly entered bank details | Recipient -> Payment | Payment instruction pins verified destination version | WF-09/11 |
| INV-12 | No filing finality from generation | Prepared report/document is not signed/filed/accepted regulatory obligation | Reporting -> Compliance | Separate signer, submission and acceptance evidence | WF-13/14 |
| INV-13 | No schedule perpetual approval | Recurring grant must revalidate each occurrence and use durable occurrence key | Scheduler -> Grant | New validation/reservation each instance | WF-06/08 |
| INV-14 | No donor anonymity leak | Charity remittance or CRM export cannot expose anonymous donor identity | Privacy -> Recipient/API | Redaction/permission gate and audit | WF-09/11 |
| INV-15 | No AI authority escalation | Assistant cannot approve grants, clear hard holds, sign filings or move funds | Assistance -> Domain Commands | Governed proposal, human/automated authority checks | WF-01..18 |
| INV-16 | No irreversible cancellation claim | After external payout finality, cancellation must be refund/return process not state deletion | Payment -> Ledger | Compensating transaction, status transparency | WF-07/09 |
| INV-17 | No unverified complex gift receipt | Tax acknowledgement must reflect accepted asset and applicable substantiation evidence | Gift Acceptance -> Documents | Require acceptance evidence and professional gates | WF-04/14 |
| INV-18 | No contradictory legal sponsor identity | White-label UI cannot conceal legally responsible sponsoring charity | Brand -> Sponsor Contract | Mandatory legal disclosure injection | WF-01/10 |

## Transaction lifecycle decisions requiring explicit state-machine design
- **Contribution:** intent vs accepted gift vs settled cash vs custodied asset vs recognized accounting vs grantable capacity; capture irrevocability effective time and donor receipt prerequisites.
- **Grant:** donor affirmation vs submitted recommendation vs sponsor approval vs atomic fund reservation vs payment instruction vs settlement vs charity acknowledgement. Decide cancellation/reapproval semantics for each transition.
- **Payment:** provider request id, idempotency, unknown, accepted, in-transit, settled, returned, voided/reissued and compensating entries; no overwrite of immutable prior status.
- **Fees:** assessed vs accrued vs posted vs paid vs reversed, and sponsor/operator/advisor payee attribution.
- **Close:** reconciliation vs signed close vs artifact issuance vs delivery vs later amendment; report as-of and known-at.
- **Migration:** staging and parallel reconciliation vs authorized cutover, writer fencing and after-first-live-posting recovery boundary.

## Acceptance suite to design, not execute yet
For each INV-01..18 create synthetic fixtures with before/after balances, actor/delegation, policy versions, evidence timestamps, exact commands, expected ledger/event effects, denied path, compensation and displayed UX state. Test concurrency, replay, partial failure, stale data, model outage, sponsor separation and document immutability. Record independent accounting/legal signoff where needed.

## Challenge
One system must not become one undifferentiated transaction. A cross-domain orchestration can coordinate a workflow but each authoritative command needs one owner. External custody, banking and government filing cannot be made atomic with Unbound's database; use idempotency, durable state and reconciliation rather than pretending distributed transactions exist.
