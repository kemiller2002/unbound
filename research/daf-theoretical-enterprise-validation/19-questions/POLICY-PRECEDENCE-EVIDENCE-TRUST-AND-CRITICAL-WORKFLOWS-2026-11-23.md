# Policy precedence, evidence trust and critical workflow gates

Date: 2026-11-23. Status: proposed theoretical requirements, not legal advice, approved policy, provider certification or executed testing. Source companions: REQ-X-017, 62-decision inventory, second-pass 24 candidate gaps and ADV-001..ADV-052.

## Design decisions

1. **Authority precedence and factual trust are independent dimensions.** A credible fact does not grant authority; a high-priority rule cannot manufacture a missing fact. Evaluation requires both an applicable authorized policy and sufficient evidence.
2. **Deny/hold precedence:** nonwaivable legal prohibition > protective hold > missing critical evidence/Unknown > sponsor stricter restriction > permitted policy approval. A hard rejection is not a human-override queue. Holds may be resolved by new authoritative evidence or independently authorized, legally permitted actions, never by shopping for a different reviewer.
3. **Scope matters:** shared charity vetting is global, grant decisions and donor records are sponsor/account scoped, while emergency protective holds may apply globally or narrowly based on evidence. Never leak tenant-specific donor information into common charity records.
4. **Time matters:** evidence and policies have observed-at, effective-from, expires-at and revoked-at metadata. Earlier grant approval does not waive execution-time screening. Historical reconstruction preserves what was known then without pretending later discoveries were known earlier.
5. **Unknown is not False:** evaluate typed facts with explicit Present/Absent/Unknown/Disputed/Stale states. Missing proof can trigger bounded trusted retrieval inside Tier 1 before AI. Contradictory high-trust sources hold until reconciled.
6. **Independence is substantive:** separate controlling principals, change histories and governance, not merely separate logins, vendors or APIs.

## Policy precedence layers

| Priority | Policy source | Authorized scope | Constraint |
|---|---|---|---|
| L0 | Applicable law, sanctions, legal orders and nonwaivable fiduciary duties | Mandatory legal constraint; counsel validates scope, effective dates and jurisdiction | No sponsor, model, operator or administrator may waive |
| L1 | Platform safety invariants | No self-approval, verified destinations, quorum safety, idempotency, segregation, tenant isolation | Cannot be weakened by sponsor settings; change through independent platform governance |
| L2 | Common charity-vetting standard | One platform-wide charity identity, eligibility, evidence and 180-day re-verification policy | Sponsor cannot downgrade common vetting; legal reliance remains counsel-gated |
| L3 | Sponsor board-authorized governance and grant policy | Eligible recipients/purposes, delegation, thresholds, fee schedules, disclosures, donor affirmations | May be stricter; cannot override L0-L2; independent policy publication |
| L4 | Account, donor and advisor mandates | Verified authority, recurring instructions, permitted amounts, revocations, preferences | Cannot confer sponsor approval or waive sponsor policy |
| L5 | Operational optimization and AI assistance | Routing, evidence retrieval, model interpretation, service-level and cost controls | Never grants authority, weakens holds or changes financial finality |

**Important qualification:** L0 is a proposed architecture classification, not a claim that any particular legal interpretation has been validated. Counsel and sponsor governance must determine which requirements are nonwaivable and which can lawfully be delegated. Where sponsor-specific governing documents impose a stricter obligation, apply the stricter valid rule.

## Evidence trust model

Evidence strength is **fact-specific**, not one universal numerical score. Two independent sources must be independently controlled, not merely branded differently. Every evidence item records subject/entity ID, asserted fact, issuer, verification method, jurisdiction, collection time, effective interval, expiry, signature/provenance, confidence limitations, permissible disclosure and contradiction links.

| Class | Source type | Examples | Restriction |
|---|---|---|---|
| E0 | Authoritative current direct evidence | Authenticated government/regulator sanctions/eligibility sources, verified sponsor board resolutions, authoritative custody/bank data, independently verified charity destination control | Must be scoped to fact, jurisdiction, timestamp, subject and legal meaning; even E0 may be stale or disputed |
| E1 | Independently corroborated primary evidence | Two genuinely independent records, signed original documents with verified issuer, cryptographic binding to authorized destination | Independence requires different controlling sources, not two feeds of one provider |
| E2 | Verified first-party operational records | Unbound's immutable decision ledger, independently governed policy versions, verified user consent, sponsor-controlled destination registry | Authoritative for actions actually taken, not proof of external payment settlement |
| E3 | Third-party attestation or secondary evidence | Provider self-reported status, aggregator feeds, verified but nonauthoritative documents | Cannot alone establish payment destination or finality when independent proof required |
| E4 | Unverified narrative or model extraction | Emails, uploaded text, public web content, AI output, unverified user assertion | Investigative lead only; cannot satisfy consequential mandatory control |

**Conflict resolution:** Check entity identity and fact scope first, then jurisdiction, effective time, source authenticity, directness and independent corroboration. A current authoritative adverse event supersedes an older clean snapshot for future execution, without deleting history. Two contradictory E0 sources produce Disputed, not a last-write-wins clean status. An AI output is never promoted to E0 merely by confidence or citations; verify its underlying source.

## Sixteen formal gate contracts (design-level, not executable)

All gates take a typed input snapshot, policy version, evidence references, evaluation time and controlling principal. Outcomes: PASS, REJECT, HOLD_MISSING, HOLD_DISPUTED, HOLD_RISK, NOT_APPLICABLE. Any gate may produce explicit Unknown. A PASS for one gate cannot erase a HOLD in another.

| Gate | Required condition | Failure action |
|---|---|---|
| G01 | Principal and action authority | Identity and delegated authority current, scoped, unrevoked | Hold; request verifiable proof |
| G02 | Segregation of duties | Approver controlling principal independent of originator, material editors and relevant policy publication | Reject self-approval; route to independent authority |
| G03 | Common charity status | Correct entity, platform-standard vetted, current evidence (<=180 days), no adverse hold | Hold and refresh; propagate adverse status |
| G04 | Sanctions | Current appropriate screen clears under approved deterministic rule; no unresolved match | Nonwaivable hold; AI cannot clear |
| G05 | Grant purpose and benefit | Valid donor affirmation, no prohibited benefit, pledge, purchase or unresolved material risk | Reject prohibited case or hold unresolved |
| G06 | Sponsor overlays | Applicable sponsor version permits purpose, charity class, amount and authority | Reject/hold; no sponsor override of L0-L2 |
| G07 | Funds and reservation | Net grant capacity sufficient and currency reservation atomic under pinned policy | No approval/reservation; request adjustment |
| G08 | Recipient destination | Charity controls independently verified destination; execution destination verifiably bound to sponsor record | Block automated payment; do not rely on provider-only fingerprint |
| G09 | Payment uniqueness | Stable intent and payment keys, no prior effect, no unresolved external outcome | Hold and independently reconcile; never blind retry |
| G10 | Financial quorum | Fenced single leader and required durable quorum for acknowledged financial/security commits | Pause authoritative financial writes |
| G11 | Independent intake | Authenticated instruction durably recorded outside failed core and marked queued only | No acknowledged queue if durability unproven |
| G12 | Protective state | Emergency holds, revocations and cancellations reconciled before replay/release | Hold replay or release |
| G13 | AI eligibility | No deterministic resolution, no hard stop, allowlisted evidence task, data permission, bounded budget | No AI call; hold or request evidence |
| G14 | Human exception | Independent authorized reviewer, legally permitted judgment and sufficient evidence | Hold, not general override |
| G15 | Fees | Contractually authorized, disclosed, service event evidenced, idempotent | No charge or waiver without authority |
| G16 | Historical audit | Pinned policy/evidence versions, append-only provenance, as-known-at reconstruction | Block certification; preserve prior history |

## Critical workflow specifications

| Workflow | Ordered deterministic evaluation | Typed outcomes | Adversarial injections | Invariants |
|---|---|---|---|---|
| Charity eligibility | Resolve stable charity ID -> check E0/E1 current evidence -> common L2 rules -> material change/sanctions -> sponsor L3 overlay | EligibleCommon, HeldForEvidence, HeldForAdverse, IneligibleCommon; sponsor-specific grant denial separately | Alias collision, expired 180-day evidence, common false-positive propagates globally | Do not replicate vetting per sponsor; propagate signed holds and source provenance; isolate donor-specific data |
| Grant authorization | Authenticate principal -> verify affirmation/purpose -> current common charity status -> sponsor restrictions -> atomic reservation and delegated approval | Submitted, Held, Rejected, ApprovedAndReserved, PendingFundingException | Agent self-approves, policy updated mid-commit, concurrent grants overcommit, donor death | No donor confirmation counted as sponsor approval; re-evaluate on material change |
| Payment release | Verify authorized destination ownership and independent execution binding -> current sanctions/charity/holds -> stable payment identity -> quorum -> provider instruction -> independent reconciliation | Prepared, DestinationVerified, ReleaseAuthorized, Submitted, UnknownExternalOutcome, Confirmed, Returned | Provider swaps account, returns self-fingerprint, times out after payment, contract expires | Never automate with opaque destination; Unknown prevents retry; separate settlement/finality |
| Disaster recovery | Acknowledge only quorum-durable authoritative commit -> fence old leader -> failover -> independent intake queued -> protective state reconcile -> current-state revalidation -> controlled replay/failback | CoreHealthy, CoreReadOnly, IntakeQueued, ProtectiveOnly, Reconciling, CoreRestored | Network partition, stale primary, queue acknowledgment lost, hold races replay | Zero loss only for acknowledged authoritative commits; no financial writes without quorum; failback after catch-up |

## Policy evaluation algorithm (pseudo-logic)

```text
input = typed authenticated intent + evidence snapshot + sponsor scope + time
policy = independently published applicable policy versions
assert no unauthorized actor and no self-approval
for each applicable mandatory safety gate:
    evaluate with explicit Unknown/Disputed/Stale semantics
    if nonwaivable rejection: stop REJECT
    if safety hold: stop HOLD (record reasons)
if missing or stale facts:
    run bounded trusted API/cache/document verification or request evidence
    re-evaluate under versioned policy, with loop and expiry limits
if still ambiguous and explicitly AI-eligible:
    run bounded, read-only AI investigation on minimum necessary data
    verify extracted evidence against source; re-evaluate mandatory rules
if independent human judgment is legally permitted and still required:
    route complete evidence, rule trace, contradictory facts to authorized reviewer
    re-evaluate mandatory execution gates after human disposition
before any value movement:
    recheck current holds, sanctions, charity, destination, capacity, payment ID, quorum
    commit exactly once or stop safely
record immutable evidence and user-facing state accurately
```

**No blanket deny-on-Unknown for all workflows:** An incomplete draft can remain a draft, an information request can proceed, and protective holds can be imposed. Unknown blocks only the actions for which the missing fact is a required safety precondition. This prevents needless friction while maintaining safe financial controls.

## Adversarial precedence and trust tests

| ID | Attack | Expected outcome |
|---|---|---|
| ADV-053 | Sponsor lowers common charity verification interval/standard below global minimum | Reject invalid configuration, preserve L2 |
| ADV-054 | Current regulator adverse record conflicts with stale cached eligibility | Hold release; preserve both records |
| ADV-055 | Two provider feeds share same controlling database but claim independence | Do not count as independent evidence |
| ADV-056 | Sponsor rule allows payment without independent destination binding | Mandatory L1 control wins; no automated payment |
| ADV-057 | Donor authorization valid but sponsor grant restriction rejects purpose | Deny grant; donor authority does not waive sponsor rule |
| ADV-058 | AI cites fabricated official document as evidence | Verify source; reject evidence promotion |
| ADV-059 | Human compliance reviewer tries to clear nonwaivable sanctions prohibition | Block override; retain review audit |
| ADV-060 | Charity record changes after grant approval but before payment | Re-run current charity/sanctions gate; hold if adverse |
| ADV-061 | Operator backdates policy effective date to validate own prior decision | Immutable publication history and independent approval reject retroactive laundering |
| ADV-062 | Cross-sponsor shared charity note contains private donor details | Reject sharing and preserve tenant isolation |
| ADV-063 | Independent queue claims success but separate durable storage fails | Do not acknowledge queued request |
| ADV-064 | Payment provider returns successful status but execution destination is unverifiable | Hold as unresolved, do not certify correct payment |
| ADV-065 | Two high-authority sources contradict on charity identity | Mark Disputed; no automatic eligibility grant |
| ADV-066 | Human reviewer approves grant but new emergency hold arrives before release | Protective hold blocks execution |
| ADV-067 | AI service unavailable on a complete rule-resolvable grant | Rules-only grant proceeds if all safety gates pass |
| ADV-068 | User edits sponsor wording of prohibited-benefit affirmation to omit essential disclosure | Reject white-label change; preserve required semantic assertion |

## Governance, test and observability requirements

- Independent publication of global and sponsor policy, with signed versions, four-eyes controls for material changes, shadow-mode replay against historical cases, adversarial regression and rollback; rollback cannot erase historical effective policy.
- Evidence trust policy itself requires independent governance; source-provider outages must not be interpreted as clean checks. Cache TTL is fact/rail/jurisdiction-specific, never invented as a universal number.
- Every decision stores which layers applied, which gates passed/held/rejected, source evidence and freshness, reason codes, policy versions, AI call reason/cost, reviewer authority and UI state.
- Measure rules-only straight-through completion, deterministic evidence-retrieval success, AI-assisted completion, human-review workload, false approvals, false holds, override attempts, duplicate financial effects, stale-evidence escapes and p95/p99 latency. Do not claim targets until baselined.
- Prioritize property tests for no self-approval, no value movement on hard stop, no stale charity eligibility, no unverified destination, no double payment, no loss of acknowledged authoritative commits and no cross-tenant leakage.

## Outstanding validation gates

1. Counsel and sponsor board must validate delegated deterministic grant approval, cross-sponsor charity reliance, jurisdiction-specific legal obligations and the nonwaivable classification.
2. Payment providers must demonstrate independently verifiable charity destination control, actual execution binding, settlement and nonreceipt proof; provider self-fingerprints alone do not qualify.
3. DR design must prove quorum/fencing, independent intake durability, protective-first replay and measurable failover/failback guarantees.
4. Map this design back to all original source requirements and resolve REG-007 reference TVC-034 from primary source, not guessed REG-058.
5. Convert all 16 gate contracts into typed schemas and falsifiable transition/property tests before claiming implementation readiness.

**Status:** These are design requirements and unexecuted adversarial tests. No code, legal authorization, third-party certification or passing simulation is claimed.
