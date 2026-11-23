# Decision inventory second-pass challenge, predicates and state machines

Date: 2026-11-23. Draft theoretical requirements, not executed tests or legal approval. Extends the 62-decision inventory and ADV-001 through ADV-030.

## Critical findings

The initial 62 rows establish coverage, but many use generic placeholders and are **not implementable predicates**. A row is not complete until it has typed inputs, an evidence trust hierarchy, exact rules, explicit Unknown semantics, policy scope and precedence, independent authority, state transitions, audit evidence and falsifiable tests.

The existing three tiers should contain a **zero-AI deterministic evidence-enrichment loop inside Tier 1**: query trusted APIs, check cached evidence freshness, reconcile known records or request specific missing facts before calling AI. Re-evaluate rules after each bounded enrichment. AI only investigates unresolved permitted ambiguity; human review only if lawful independent judgment is available. Hard stops do not escalate into override permissions. Every tier has bounded attempts and latency.

Other cross-cutting gaps: controlling-principal independence rather than different logins; sponsor-specific rules cannot weaken nonwaivable platform safety; shared charity evidence must not expose donor or sponsor-private data; payment authorization is separate from execution and finality; rule/model publication must use independent maker-checker; policy versions must be pinned; and outage intake must never be presented as a financial commitment.

## 24 candidate missing or under-specified decisions

These are investigation candidates, not proven omissions from every source. Deduplicate against original requirements before assigning canonical IDs.

1. **GAP-01**: DAF-to-DAF or sponsor-to-sponsor transfers and restrictions.
2. **GAP-02**: Donor-imposed conditions, naming and side agreements.
3. **GAP-03**: Employer matching and third-party contribution attribution.
4. **GAP-04**: Securities lot tracking, splits, mergers and corporate actions.
5. **GAP-05**: Complex asset disposal, UBIT and tax characterization.
6. **GAP-06**: Mailed-check delivery, stale checks and stop-payment.
7. **GAP-07**: Grant purpose restrictions and recipient reporting.
8. **GAP-08**: Multi-party advisor disputes and conflicting succession claims.
9. **GAP-09**: Fundraising event tickets, pledges and private benefits.
10. **GAP-10**: Privacy deletion and cross-border disclosure requests.
11. **GAP-11**: Unclaimed property and abandoned DAF balances.
12. **GAP-12**: Sponsor/provider insolvency and continuity transitions.
13. **GAP-13**: Forged donor attestations and signature disputes.
14. **GAP-14**: Sponsor offboarding, tenant export and retention.
15. **GAP-15**: Global charity-vetting policy change and blast radius.
16. **GAP-16**: Rules/model shadow-mode publication and rollback.
17. **GAP-17**: Manual-work fee attribution, waivers and disputes.
18. **GAP-18**: Bulk migration, legacy IDs and deduplication.
19. **GAP-19**: Corrected tax and regulatory filings.
20. **GAP-20**: Charity takeover, aliases and fraudulent bank updates.
21. **GAP-21**: Pooled investment allocation and cross-fund fairness.
22. **GAP-22**: AI-human disagreement and reviewer shopping.
23. **GAP-23**: Provider qualification/contract expiration.
24. **GAP-24**: Delegated access revocation and embedded session invalidation.

## Sixteen proposed P0/P1 deterministic predicates

Illustrative pseudo-logic, not executable or legally certified. Unknown critical inputs must never become false/clear by default.

| ID | Gate | Predicate | Failure |
|---|---|---|---|
| P-01 | Identity | verified principal AND active authority for requested action | Hold |
| P-02 | Segregation | approver controlling principal differs from originator AND all material editors | Reject approval |
| P-03 | Charity | common vetting valid at execution AND no global hold AND sponsor grant rules pass | Hold |
| P-04 | Sanctions | current screen clear under independently approved deterministic policy | Hold |
| P-05 | Benefit | donor affirmation valid AND no prohibited benefit or unresolved material ambiguity | Reject/hold |
| P-06 | Funds | net capacity sufficient AND reservation atomic under pinned policy | Hold |
| P-07 | Destination | charity controls destination AND execution destination independently bound to authorization | Hold |
| P-08 | Payment identity | stable intent key AND no prior effect AND no Unknown external outcome | Hold |
| P-09 | DR | safe quorum AND fenced leader AND acknowledged commit durably replicated | Stop financial writes |
| P-10 | Queue | independent durable receipt AND authenticated request AND status Queued only | Reject intake |
| P-11 | Replay | holds/revocations reconciled AND current authority/eligibility/funds rechecked | Hold replay |
| P-12 | AI | unresolved ambiguity AND AI allowed AND no hard stop AND budget/privacy permit | No AI call |
| P-13 | Human | legal exception allowed AND authorized independent reviewer AND sufficient evidence | Hold |
| P-14 | Recurring | active mandate AND current authority AND current charity/policy/financial gates | Skip/hold |
| P-15 | Fee | contract permits AND disclosed AND service verified AND idempotent | Reject charge |
| P-16 | History | evidence snapshot AND policy version AND journal provenance reconstructable | Block certification |

## Required state machines

**Decision case:** Created -> RuleValidation -> ResolvedByRule / RejectedByRule / HeldForEvidence / HeldForRisk / EligibleForAI / IndependentHumanRequired. HeldForEvidence -> BoundedTrustedEvidenceRefresh -> RuleValidation. EligibleForAI -> BoundedAIInvestigation -> RuleValidation or IndependentHumanRequired or Hold. AI recommendations never create sponsor authorization. IndependentHumanRequired -> independent authorized review -> Approved / Rejected / Hold, only if legal and no self-approval.

**Grant:** Draft -> Submitted -> SponsorPolicyEvaluated -> Rejected / Held / ApprovedAndReserved -> PaymentPrepared -> PaymentReleased -> PaidConfirmed / Returned / Unknown. Approval and currency reservation are atomic when allowed. Pending-funding is separate. New safety signals may impose a hold before payment.

**Payment:** Prepared -> IndependentlyVerifiedDestination -> ReleaseAuthorized -> ProviderSubmitted -> Confirmed / Unknown / Rejected / Returned. Unknown prohibits duplicate retry; settlement and legal finality may be distinct rail-specific states. Destination changes invalidate verification.

**Outage:** Authenticated -> IndependentlyDurableQueued -> AwaitRecovery -> ProtectiveStateReconciled -> Revalidated -> NormalDomainCommand. Emergency holds and revocations use a separate protective-only append-only channel and reconcile before replay. No emergency financial execution.

All transitions require actor/controlling principal, authority, evidence IDs, input snapshot, policy version, timestamps, prior state, idempotency and decision reason. Material changes force revalidation.

## 22 additional adversarial specifications (ADV-031 to ADV-052)

All are unexecuted test plans; pass requires observable fault-injection artifacts.

- **ADV-031**: Stale charity cache conflicts with newer adverse signal. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-032**: Sponsor config attempts to waive global sanctions policy. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-033**: Null critical identity attribute incorrectly treated as clear. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-034**: Policy author approves own grant via alternate account. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-035**: AI extracts fraudulent destination from injected document. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-036**: AI proposes auto-publishing rules from fraudulent exceptions. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-037**: Material evidence changes between approval and payment. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-038**: AI/human escalation loops indefinitely. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-039**: Common charity evidence leaks sponsor-private grant notes. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-040**: Sponsor policy waives global destination proof. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-041**: Fee retries after recovery create duplicate charges. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-042**: Reviewer follows confident AI despite contradictory primary evidence. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-043**: Noncash contribution reverses after grant payment. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-044**: Charity verification expires while backlog replays. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-045**: Employer contribution misattributed to donor tax receipt. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-046**: Assisted accessibility channel bypasses donor consent. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-047**: Policy version changes during approval/reservation. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-048**: Payment provider qualification expires before release. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-049**: Unsigned global charity status update accepted. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-050**: Emergency operator self-releases hold via alternate identity. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-051**: DAF transfer incorrectly processed as grant. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.
- **ADV-052**: Recurring mandate changed after instance queued. Assert no unsafe approval, value movement, tenant disclosure or history rewriting; verify explicit hold, independent authority, idempotency and audit trace as applicable.

## Sequenced next work

1. Map 24 gap candidates against original source requirements, deduplicate and assign canonical owner/authority.
2. Replace generic predicates for P0 money, sanctions, charity, identity, self-approval, payment destination and DR with exact schemas and policy precedence.
3. Model and challenge state transitions with concurrent and adversarial events; no implementation authorization inferred.
4. Design independent policy publication, shadow evaluation and rollback. Proposed AI-derived rules never self-publish.
5. Define measurable exception and cost observability without assuming the illustrative 90/8/2 split.
6. Keep counsel, board, CPA, payment-provider proof and REG-007 cross-reference verification as open evidence gates.

**Working discovery universe:** 62 original inventory entries plus 24 candidate additions equals 86 *candidate* entries before deduplication, not 86 validated distinct decisions.
