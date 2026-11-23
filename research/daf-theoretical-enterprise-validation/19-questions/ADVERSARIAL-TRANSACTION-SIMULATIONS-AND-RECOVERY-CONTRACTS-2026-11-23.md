# Adversarial transaction simulations and workflow failure recovery
Date: 2026-11-23. **Expected-outcome design fixtures only. None executed.**

## Simulation method
For each case, instantiate a deterministic sponsor, fund, role/authority, policy version, ledger opening balances, external adapter response, timestamps and command IDs. Execute the nominal and failure path against the *same* domain command contract when software exists. Capture before/after ledger entries, reservations, provider state, source evidence, work case, donor/charity/advisor views, immutable audit and recovery decision. A mock must reproduce uncertainty and delayed outcomes, not only success.

| Case | Failure scenario | Synthetic setup | Expected safe outcome | Transitions | Open decision |
|---|---|---|---|---|---|
| SIM-01 | Concurrent grants | Fund available $100; grants $80 and $50 submitted simultaneously | Only one reserves, second fails or remains pending; total reserved <= $100 | TX-11 | Database serializable/optimistic conflict; user sees capacity changed |
| SIM-02 | ACH return after grant reservation | $100 received/accepted, $80 reserved, then $100 returned | Compensating debit, deficit/hold; no silent negative grantable; affected pending payout reviewed | TX-03/05/11/15 | Define recovery priority and sponsor risk appetite |
| SIM-03 | Provider timeout then settlement | $60 payment dispatched; provider timeout; late settlement notice | One payment only, Unknown until reconciliation, then one settlement posting | TX-13/14 | Never retry blindly or infer failure from timeout |
| SIM-04 | Charity bank takeover | Verified account A; attacker changes to B while $60 instruction pending | Pending payment remains pinned to A or held; no silent switch to B | TX-12/19 | Independent bank proof and callback required |
| SIM-05 | Late valuation correction | Fund asset reported $120 then corrected $90 after $100 grant approved | Closed statement remains as-issued; correction logged; capacity deficit handled under governed policy | TX-08/11/21 | Accounting signoff needed for impairment/valuation treatment |
| SIM-06 | Two recurring scheduler deliveries | $10 biweekly occurrence dispatched twice after worker restart | Exactly one occurrence/grant, one reservation, one disbursement | TX-17/18 | Durable occurrence key across DST and pay-period changes |
| SIM-07 | Donor death before grant payout | Grant approved but unpaid; verified donor death event arrives | Reevaluate donor authority and sponsor succession policy; no automatic reversal or payment without rule | TX-11/12/25 | Owner decision and legal review needed |
| SIM-08 | Sponsor policy changed mid-flight | Grant validated under V1; V2 introduces prohibition before dispatch | Recheck at defined execution boundary; hold if new hard restriction applies | TX-11/12/27 | Policy version, effective-at and grandfathering rule required |
| SIM-09 | Migration dual writer | Legacy posts $40 after opening balances extracted; Unbound activation attempted | Cutover blocked until tie-out and fencing; no two authoritative writers | TX-23/24 | Cross-system atomicity impossible; enforce operational fence |
| SIM-10 | Migration issue after live posting | First new Unbound transaction posted; legacy import discrepancy discovered | No blind rollback; protective hold, adjustment and auditable compensation | TX-23/24 | Formal no-return boundary |
| SIM-11 | Complex asset not yet accepted | Donor promises $1m restricted shares; appraisal pending | No tax receipt claiming acceptance, no liquid grantable capacity | TX-06/07/08 | Tax counsel and accounting policy applicability |
| SIM-12 | Partial securities liquidation | 100 shares expected $100; only 60 sold, fees reduce cash | Recognize actual settled proceeds and remaining position; do not assume expected cash | TX-08/09 | Price, fees, valuation and lot reconciliation |
| SIM-13 | Fee contract retroactive change | AUM-based fee posted, then contract amendment backdated | No silent historical overwrite; reviewed adjustment, disclosures, effective-date evidence | TX-20/21 | Board/conflict approval if related party |
| SIM-14 | Regulatory return rejected | Annual form generated and submitted; authority rejects | Status rejected, corrective case, resubmit and preserve earlier artifact | TX-22 | Document creation not filing completion |
| SIM-15 | Cross-sponsor charity evidence leak | Charity global evidence includes sponsor-private investigation | Shared public evidence only; sponsor-specific adverse case invisible outside permitted scope | TX-27 | Row and object authorization at every query |
| SIM-16 | External checkout success mismatch | Widget shows success before sponsor decision; later grant denied | Widget must show submitted recommendation, not payment success; callback denial reconciles | TX-26/11/14 | Marketing language and UI contract |
| SIM-17 | Advisor revocation during session | Advisor loses rights while editing grant, then submits | Command denied using current authority, draft preserved without leakage | TX-25/10 | Revocation applies server-side, not only UI |
| SIM-18 | Charity payment returned after month close | Check reported paid in October; returned in November | November compensation and historical October as-issued preserved, subject to accounting policy | TX-15/21 | Independent accounting classification review |
| SIM-19 | Duplicate settlement callback | Provider repeats same settlement event three times | Exactly one ledger posting and one outward event; duplicates auditable | TX-14 | Unique external event and command IDs |
| SIM-20 | Disaster recovery failover | Primary unavailable after client receives acknowledgement; recovery node takes over | Single fenced writer; no lost acknowledged commit under stated quorum assumptions | TX-03/11/14/23 | Cannot claim unconditional RPO zero |
| SIM-21 | Recipient identity merge | Two charity profiles later discovered to represent same entity | Governed merge with immutable external-ID crosswalk; no retroactive donor identity disclosure | TX-19/27 | No unsafe auto-merge |
| SIM-22 | AI proposes unauthorized payment release | Model recommends release despite sanctions hold | Proposal cannot execute; policy rejects, audit retains suggestion and evidence | TX-12/27 | AI no authority |
| SIM-23 | Unclaimed charity recipient | Recipient has no online account and check is requested | Staff-assisted verified mail payment possible; charity portal not prerequisite | TX-12/13 | Accessibility and low friction |
| SIM-24 | Stale custodian feed | Holdings as-of two days ago; user requests current grantable value | Show dated/Unknown current value; policy restricts capacity as appropriate | TX-08/11 | Do not synthesize live price |
| SIM-25 | Refund after legal acceptance | Donor asks to reverse accepted irrevocable gift | No automatic donor refund; sponsor/legal review and permitted correction only | TX-04/05 | Legal sponsor owns charitable assets |
| SIM-26 | Payment canceled after settlement | Donor requests cancellation of settled grant | Cannot undo; any recovery is new return/recall process with evidence | TX-14/15/16 | No misleading canceled status |
| SIM-27 | Annual close with unreconciled bank | Statement issuance attempted with unresolved material discrepancy | Close blocked or explicit authorized exception under policy; evidence preserved | TX-21/22 | Thresholds need CPA/controller decision |
| SIM-28 | Contract expiration mid-program | Sponsor contract expires with grants in flight | Protective safe-mode, contractual continuity and sponsor authority review; no orphaned funds | TX-28/12/23 | Counsel determines continuity obligations |

## Transaction scope taxonomy
- **Atomic local:** validation, authorization, reservation and ledger writes that can commit together in the single authoritative transactional database. The business operation may still be incomplete.
- **Durable asynchronous:** outbox/inbox and event handling, retries with stable idempotency and state reconciliation.
- **External irreversible:** custodian transfer, bank payment, check presentment, tax filing. Never promise atomic rollback across providers.
- **Compensating:** ledger adjustment, payment return, reissue, correction, amended filing, role revocation and recovery case. Retain original event and provenance.
- **Protective:** freezes, holds, access revocations and bounded DR safety plane; cannot create an alternate financial writer.

## Failure handling contract
1. Confirm actual known state and authority before attempting remediation.
2. Preserve original request, external key, evidence and timestamp.
3. Query external outcome by stable reference when ambiguous; do not blindly replay money movement.
4. Apply only the owning component's typed command with sponsor-scoped policy and segregation of duties.
5. Reconcile accounting, work queue, recipient and donor projections.
6. Verify final authoritative state before marking completed; otherwise remain Unknown/Investigating.
7. Capture root cause and reviewed prevention candidate without model self-modifying policy.

## Critical open design questions
- At exactly which transitions does legal acceptance become irrevocable for each asset class?
- Does a material late valuation correction trigger a fund-level hold, specific pending grants, or sponsor-wide freeze?
- What are the precise accounting entries for returns, write-offs and negative grantable capacity?
- What is the rail-specific proof standard for ACH, wire, mailed check and hand-delivered check?
- Which sponsor rule changes revalidate in-flight approvals and at what irreversible boundary?
- When is migration rollback prohibited and what compensation mechanism is authorized?
- What are sponsor-specific filing obligations and signoff thresholds?
- How do sponsor agreement expiration and contract termination affect pending obligations?
- Which funds, assets and payment rails can be supported without live custody/provider evidence?

## Status
28 designed cases, 0 executed, 0 independently reviewed. Treat numeric examples as synthetic only, not operating forecasts.
