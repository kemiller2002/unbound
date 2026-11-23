# Authoritative data model and transaction boundaries

Date: 2026-11-23. Theoretical design only; no implementation, executed test, legal or accounting approval. Component monolith, not microservices. Companion REQ-X-017..021.

## Core findings and challenged assumptions

1. The ledger is the **single authoritative source** for fund balances, reservations and accounting journal. Other components may present derived views, never independently mutate money.
2. Donor recommendation, sponsor authorization, reservation, payment release, provider submission, settlement and legal finality are **different states**. Do not collapse them into Approved or Paid.
3. One DB transaction can atomically coordinate internal grant, ledger, hold and payment-intent state, but **cannot atomically commit an external bank transfer**. Use durable outbox, stable provider keys, independent evidence and Unknown reconciliation.
4. Charity eligibility, sanctions disposition, sponsor policy, donor authority and destination verification are distinct facts owned by distinct components. A pass in one never clears a hold in another.
5. Every critical record requires versioned provenance, tenant scope, effective-at and recorded-at where applicable, and append-only correction history. Historical reconstruction must support both what was known then and what was learned later.
6. A shared physical DB does not mean shared write ownership. One component owns each aggregate; application coordinator invokes owned commands inside one transaction.
7. Global hold publication and payment release require a provable serialization point. An adverse event learned later cannot be guaranteed to prevent an already irreversible external payment; preserve the exposure window honestly.

## Logical authoritative records

| Aggregate | Owning component | Minimum contract |
|---|---|---|
| SponsorPolicyVersion | Governance | Signed approval, effective interval, delegation, immutable version |
| PrincipalAuthority | Identity | Scope, controlling principal, revocation epoch, validity |
| CharityIdentityAndEvidence | Common Charity | Legal IDs, aliases, issuer, provenance, evidence timestamps |
| CharityEligibility | Common Charity | Version, verified-at, valid-until, global status and evidence links |
| ProtectiveHold | Compliance | Target scope, reason, effective commit, independent release |
| GrantRecommendation | Grant | Donor intent, affirmation, sponsor, charity, amount, external ID |
| GrantAuthorization | Grant | Decision ID, policy versions, independent sponsor authority |
| GrantReservation | Ledger | Fund, amount, currency, grant, status and ledger version |
| FundJournal | Ledger | Balanced postings, source, effective/recorded time, reversal links |
| FundCapacity | Ledger | Transactionally derived capacity, encumbrances, fees, buffers |
| PaymentIntent | Payment | Stable identity, grant, destination version, provider state |
| DestinationAuthorization | Payment | Independent charity-control and execution-binding evidence |
| ExternalPaymentObservation | Payment | Provider evidence, observation time, finality class |
| OutboxDispatch | Command Core | Durable external intent, idempotency key, fencing epoch |
| DecisionTrace | Evidence/Audit | Evidence/policy snapshots, gate outcomes, authority and commit position |
| EmergencyProtectiveInbox | Recovery | Independent durable receipt, reconciliation epoch |
| OutageIntake | Recovery | Authenticated queued-only receipt, stable intent |
| FeeCharge | Fees/Ledger | Contract version, service event, posting ID |
| AssetPositionAndValuation | AUM | Custodian, lots, valuation evidence, allocation |
| ReconciliationCase | Finance/Payment | Unknown status, conflicting evidence, authorized disposition |

## Twelve atomic transaction boundaries

| ID | Command | Atomic records | Required invariant |
|---|---|---|---|
| TX01 | Grant approval + capacity reservation | Grant authorization, reservation, audit | One serializable/equivalent transaction; failure commits neither |
| TX02 | Publish charity adverse hold | Charity/compliance hold, release gate version, audit | No later-ordered payment release passes |
| TX03 | Payment release authorization | Payment intent, gate snapshot, idempotency, outbox, audit | Recheck current holds, destination, authority, provider and quorum |
| TX04 | Contribution accepted | Contribution state and balanced journal | No unverified funding added to available capacity |
| TX05 | Fee accrued/charged | Fee event, ledger postings, trace | No duplicate charge or capacity race |
| TX06 | Payment accounting effect | Payment evidence, balanced journal, trace | Use approved accounting recognition policy, not provider claim alone |
| TX07 | Cancel payment | Payment state, reservation disposition, audit | Cannot claim cancelled if submitted or external Unknown |
| TX08 | Resolve external Unknown | Observation, case, authorized disposition, journal if warranted | No blind retry; independent evidence required |
| TX09 | Publish sponsor policy | Signed version, activation, audit | Independent maker-checker and immutable history |
| TX10 | Reconcile emergency holds | Protective inbox, core hold, replay barrier | Protective state applied before queued replay |
| TX11 | Investment allocation | Position/unit changes and financial journal | Fund-level fairness and reconciliation |
| TX12 | Reversal/return | New linked reversing journal and state | Never erase original financial event |

## Ledger, capacity and payment rules

**Double entry:** Each posted journal must balance in its currency and accounting basis. FX movements use explicit legs and bridge accounts, not arithmetic across unlike currencies. Every posting references its source, and corrections use linked reversing entries rather than overwriting history.

**Grant capacity:** Conceptually eligible liquid or collateralized value less existing reservations, accrued applicable fees, required liquidity buffers and other authorized encumbrances. Exact formula and recognition events remain subject to sponsor/CPA policy. Compute and reserve under the same authoritative concurrency-safe transaction. Pooled custody does not allow one donor fund to spend another fund's capacity.

**Reservation:** Proposed, Active, Consumed, Released or ExceptionHeld are distinct statuses. Partial payments, FX, split disbursements and returns require explicitly linked allocations; no reuse of consumed reservation. A valid reservation is not automatically an accounting payable.

**Payment:** Stable PaymentIntentId and provider idempotency key survive site failover and retries. An external timeout after possible submission is Unknown, never Failed. Independent destination-control and provider-execution binding remain mandatory. A provider success response does not itself establish correct recipient or legal finality.

**Fees:** The fee engine determines authorized charge from sponsor contract and evidenced service event; ledger alone posts financial effect. A replayed fee event cannot post twice. Accrual is considered before new grant capacity without silently invalidating already valid reservations.

## Historical reconstruction and audit

Every consequential command stores actor and controlling principal, sponsor, policy/evidence versions, original fact states including Unknown/Disputed/Stale, gate results, amount and currency, authoritative commit position, relevant destination version, reviewer independence and external observations. Maintain both **as-known-at** and **as-effective-at** histories. Retroactive corrections trigger linked journal/evidence changes and exposure analysis, not rewriting prior decisions. Retention and redaction require separate counsel-approved controls.

## Consistency and performance

Use serializable or proven equivalent isolation for reservation, global holds and release; never rely on eventually consistent reporting or charity projections as a payment gate. Expensive AI investigations, statements, notifications and analytics run outside authoritative DB locks and use versioned snapshots. Benchmark contention and lock duration before selecting DB engine, partitioning or DR topology. One active fenced writer with quorum-durable acknowledged commits is the proposed DR safety model, subject to fault-model validation.

## Adversarial specifications ADV-097 through ADV-116

Unexecuted test plans. Each test must assert no unauthorized money movement, duplicate financial effect, false finality, cross-tenant disclosure or loss of reconstructable evidence, as applicable.

- **ADV-97**: Approval commits while reservation fails.
- **ADV-98**: Two grants race for last available funds.
- **ADV-99**: Pooled custody hides fund-level shortfall.
- **ADV-100**: Cross-sponsor external grant IDs collide.
- **ADV-101**: Multi-currency journal is improperly summed.
- **ADV-102**: Outbox retries after provider success and lost response.
- **ADV-103**: Provider reports success at unauthorized destination.
- **ADV-104**: Late adverse notice has earlier real-world effective time.
- **ADV-105**: Fee and grant reservation race for same capacity.
- **ADV-106**: Return arrives after donor fund closure.
- **ADV-107**: Recurring instance duplicated across failover.
- **ADV-108**: Emergency protective log inaccessible on restoration.
- **ADV-109**: Historical report overwrites original eligibility context.
- **ADV-110**: Destination changed after preparation.
- **ADV-111**: Read projection lags authoritative journal.
- **ADV-112**: Operator attempts direct SQL payment finality change.
- **ADV-113**: Investment price correction rewrites historical AUM.
- **ADV-114**: Cancellation races provider submission.
- **ADV-115**: Payment return and fee reversal arrive out of order.
- **ADV-116**: Outage queue replays under superseded sponsor policy.

## Open decisions and next research

1. Specify typed F# discriminated unions and event contracts for all twenty aggregates, without beginning application implementation.
2. Define journal account taxonomy, fund-level liquidity, unitized pooled AUM, FX and fee allocation with CPA/sponsor validation.
3. Select a candidate managed DB only after verifying transaction isolation, failover semantics, quorum acknowledgments and operational constraints.
4. Define rail-specific destination binding, irreversible submission, nonreceipt and settlement evidence; disqualify opaque rails from automated release.
5. Create model-based property tests for balanced journals, capacity conservation, hold/release serialization, provider Unknown and emergency replay. None have run.
6. Trace all aggregates to original DAF requirements and the 86 candidate decision types; deduplicate and resolve ownership conflicts.

**Status:** 20 candidate aggregates, 12 transaction contracts, 20 additional adversarial scenarios; all proposed, none validated or executed.
