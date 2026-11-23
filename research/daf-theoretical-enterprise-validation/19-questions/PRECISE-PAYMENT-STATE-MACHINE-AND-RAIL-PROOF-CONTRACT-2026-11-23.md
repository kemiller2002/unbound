# Precise payment state machine and rail-specific evidence contract
Date: 2026-11-23. Proposed requirements, not implemented or legally validated.

## Separate orthogonal state dimensions
Do **not** collapse dispatch, provider knowledge, settlement, accounting and recipient acknowledgement into a single ordered status. Represent:
- **Authorization:** Draft, PolicyValidated, Authorized, Suspended, RevokedBeforeDispatch.
- **Dispatch:** NotSent, Queued, Sending, SentWithRequestId, DispatchOutcomeUnknown, RejectedBeforeExecution.
- **Provider knowledge:** Unknown, Acknowledged, InProcessing, ProviderRejected, Executed, Recalled, Returned. Acknowledged never means settled.
- **Settlement evidence:** Unverified, Provisional, Confirmed, Reversed, PartiallyReturned, Disputed.
- **Accounting reconciliation:** NotPosted, ClearingPosted, SettledPosted, Reconciled, ExceptionOpen, Adjusted.
- **Recipient acknowledgement:** NotRequested, Awaiting, Acknowledged, Disputed, NotApplicable.
- **Work case:** None, Investigating, Hold, RecoveryPending, Resolved.

A UI may derive a concise label, but the persisted authoritative dimensions and evidence must remain distinguishable. Status projection must disclose its as-of time and uncertainty.

## Core command contract
`AuthorizePayment(grantId, reservationVersion, payeeDestinationVersion, policyVersion, actor, idempotencyKey)` validates grant authority, available reservation, current holds, verified destination and segregation of duties, then creates one immutable payment instruction.
`DispatchPayment(paymentId, requestId, adapterVersion)` uses an outbox and external idempotency reference. Timeouts yield **DispatchOutcomeUnknown**, not failure or permission to resend.
`RecordProviderEvidence(paymentId, providerEventId, rawEvidenceHash, observedAt, effectiveAt, providerState)` deduplicates and preserves contradictory evidence; no unverified provider message alone establishes settlement.
`ConfirmSettlement(paymentId, railProof, settledAmount, currency, effectiveAt)` posts through the single ledger writer after required finance controls.
`RecordReturn(paymentId, returnId, amount, reason, effectiveAt)` posts a linked compensating event and handles partial returns.
`RequestVoidOrRecall(paymentId, authority, reason)` is a separate externally uncertain operation. A requested void does not cancel a settled payment.
`ReissuePayment(originalPaymentId, verifiedStopOrReturnEvidence, newDestinationVersion, authority)` creates a new linked payment only after risk controls and resolution of prior uncertainty.

## Rail-specific proof hierarchy
| Rail | Evidence before dispatch | Provisional evidence | Stronger settlement evidence | Irreversibility caveat |
|---|---|---|---|---|
| ACH | Verified account ownership where applicable, sponsor authority, trace reference | Processor acceptance or batch submission | Bank/processor settlement and reconciliation | Returns/reversals may occur later |
| Wire | Beneficiary verification, sanctions/hold check, instruction approval | Bank accepted request | Bank execution trace and reconciled statement, plus recipient confirmation if required | Recall may be unsuccessful |
| Paper check | Verified mailing address, check number, authorized print/release | Printed/mailed or delivered | Cleared check/bank image plus reconciliation | Mailing is not payment finality |
| Hand-delivered check | Verified recipient and chain-of-custody authorization | Signed handoff record | Cleared bank evidence; handoff receipt is not clearing | Recipient can lose or not deposit check |
| Custodial securities transfer | Custodian/recipient account and transfer approval | Transfer instruction accepted | Receiving/issuing custodian confirmation and reconciled position | Reversibility depends on provider |
| International payout/FX | Country/legal route, payee, FX quote, approval | Intermediary acceptance | Beneficiary/settlement evidence and currency reconciliation | Multiple intermediaries and FX slippage |

These are proposed proof standards, not claims of universal banking finality. Provider contracts and counsel/controller must define exact rail thresholds.

## Guard and compensation invariants
1. One grant installment maps to one active payment chain; replacement instructions link the original.
2. Payee destination version is pinned when payment is authorized; later recipient profile edits do not reroute it.
3. Provider Unknown blocks blind retry, duplicate payment creation and premature ledger settlement.
4. Settlement may precede recipient acknowledgement; a charity acknowledgement never substitutes for settlement.
5. Partial settlement, fees, partial returns and currency differences require amount/currency-specific entries.
6. A late return after close creates dated adjustments, never overwrites as-issued statements.
7. External callbacks may arrive out of order; preserve raw evidence and resolve through monotonic knowledge plus explicit reversals, not naive last-message-wins.
8. Every state transition exposes source, actor, sponsor scope, provider key, effective/observed time, authorization and work case.

## Adversarial fixtures
- PAY-01: Timeout followed by late accepted callback; no duplicate dispatch.
- PAY-02: Callback says executed, bank settlement absent; remain unreconciled.
- PAY-03: Return arrives after settled and period closed; linked compensation.
- PAY-04: Stop-check requested, bank has already cleared; reissue prohibited until resolved.
- PAY-05: Bank destination changes after instruction creation; instruction remains pinned or held.
- PAY-06: Partial international payout with FX fee; reconcile by currency and amount.
- PAY-07: Same provider event delivered three times; one accounting effect.
- PAY-08: Hand-delivery receipt without check clearing; display handed over, not settled.
- PAY-09: Contradictory provider callbacks; exception case, no automatic financial rewrite.

## Design challenge
A single linear status is inadequate because external knowledge and accounting are orthogonal. The UI can be simple while the domain model remains precise. Do not build a second payment ledger in the recipient portal, advisor experience or external widget.
