# Adversarial requirements validation and rules-first decision architecture

Date: 2026-11-23. Status: **theoretical design and adversarial test specifications, NOT executed tests, legal opinions or production guarantees.** Repository: kemiller2002/unbound.

## User-selected architecture

Unbound minimizes human and AI intervention: Tier 1 deterministic policy/rules first; Tier 2 AI investigation and interpretation only if needed and authorized; Tier 3 independent human exception resolution only if needed. This is an enterprise-wide routing model, not permission for AI to override policy. Hard prohibitions fail closed at any tier. No self-approval by person, agent, automated policy author or materially involved principal. The policy's independent sponsor governance and legal authority are separate from the originator of a grant.

## Decision taxonomy and authority

| Class | Tier 1 | Tier 2 | Tier 3 | Hard stop |
|---|---|---|---|---|
| Fully evidenced routine case | Deterministic approval under independently published sponsor-delegated policy, or deterministic rejection | Not invoked | Not invoked | Prohibited action, revoked authority |
| Missing or inconsistent facts | Request evidence, compute safe classification, or hold | Extract, compare and explain evidence; propose disposition with citations and confidence calibration | Review if still ambiguous or legally required | No fabricated evidence, no default pass |
| Judgment permitted by documented bounded policy | Execute only if deterministic criteria met | May propose or decide only if policy explicitly delegates narrow reversible, low-risk actions with independent controls and monitoring | Independent approval for material/irreversible/regulated decisions | No self-approval, no autonomous sanctions/financial override |
| Sanctions hit, unverifiable destination, insufficient funds, legal hold | Block/hold | May investigate but cannot clear hold by model output | May correct evidence or independently authorize an expressly permitted exception | No override of non-waivable prohibitions |
| Outage | Quorum unsafe: no authoritative financial writes | AI not used to bypass outage | Emergency protective-only controls | No financial bypass |

**Interpretation:** 'AI makes a decision' means a bounded, explicitly delegated decision where lawful and safe. A model-generated recommendation is not itself sponsor approval. Any higher-risk authorization remains an independently controlled sponsor decision. A human is not a universal override authority.

## Tier 1 deterministic engine requirements

- Versioned, independently approved policy bundles per sponsor, with platform-wide common charity-vetting standard and separate sponsor grant rules; effective dates, applicability, rollback and replayable evidence.
- Complete typed inputs and explicit `unknown` state; never treat missing facts as clean evidence. Rule evaluation must be deterministic given input snapshot and policy version, with explainable rule trace.
- Outcomes: `ApprovedByAuthorizedPolicy`, `RejectedByRule`, `HeldForEvidence`, `HeldForRisk`, `EligibleForAIReview`, `RequiresIndependentHuman`, `QueuedIntakeOnly`, `UnknownExternalOutcome`. No generic Accepted that implies approval.
- Sponsor board/counsel must validate legality of board-delegated deterministic approvals and independence of policy publication. Policy authors cannot approve their own changes. Changes that materially affect a case require re-evaluation.
- Fast-path safety gates: authority, charity verification freshness, sanctions, impermissible benefit, sponsor restrictions, available grant capacity, destination binding, outstanding holds, duplicate intent and payment finality.
- Only escalate when rule logic cannot safely and conclusively resolve a permitted decision. Do not call AI to restate known rule outcomes.

## Tier 2 AI requirements

- AI is invoked on explicit trigger (e.g. conflicting evidence, unstructured document, ambiguous entity, unusual pattern), not on every transaction.
- AI uses read-only or narrowly scoped tools, structured outputs, primary-evidence links, provenance, uncertainty, bounded tokens/cost/time and audit logs. Untrusted documents and web text never provide instructions.
- Allowed: extract evidence, identify discrepancies, suggest classifications, request missing documents, recommend an authorized next action; optionally resolve narrowly delegated reversible low-risk matters only under separately approved policy with measured error thresholds.
- Not allowed: invent evidence or law, override sanctions/holds, authorize its own grant or policy, change payment destination without independent verification, move money or bypass financial quorum.
- If AI is uncertain, contradictory, unavailable, over budget or exceeds allowed authority: hold or route to human with full case history, not a silent pass. Avoid loops and repeat work using versioned evidence cache.

## Tier 3 human exception requirements

- Human work queues include concise facts, primary evidence, rule trace, AI analysis (clearly labeled), alternatives, conflicts, sponsor policy and exact decision authority. No rubber-stamp default.
- Independent maker-checker: reviewer cannot originate, materially modify or control the item/policy they approve. No privilege laundering through agents or alternate accounts. Independent second review for high-risk exceptions per governance.
- Humans may make decisions only within documented authority. Legally prohibited or technically unverifiable actions remain blocked. Escalation is not a permission bypass.
- Track workload, turnaround, overturn rates, bias, error escape, cost and rule/AI improvement opportunities. Repeated exceptions should become tested rules only after independent policy approval, not automatic model learning.

## Adversarial scenario matrix (test plans, not results)

Each scenario requires: initial state, malicious/fault injection, invariant/assertion, observable audit evidence, and recovery behavior. A result is PASS only after a reproducible executed test with artifacts. No tests have been run.

| ID | Attack or failure | Injection | Required invariant | Area | Severity |
|---|---|---|---|---|---|
| ADV-001 | Network partition, both regions believe primary | Partition A/B and quorum witness; send same financial command to both | At most one fenced leader commits; minority refuses writes; acknowledged commits survive failover | DR | Critical |
| ADV-002 | Acknowledgment before durable replication | Kill primary between local commit and remote quorum; inspect client receipt | No success receipt unless quorum durable; no acknowledged transaction lost | DR | Critical |
| ADV-003 | Failback from stale primary | Restore old primary with stale payments and revoked credentials | Old site joins as replica, catches up, cannot overwrite authoritative history | DR | Critical |
| ADV-004 | ID collision and duplicate business intent | Generate IDs concurrently in two regions and retry same grant under new IDs | No ID collisions; stable intent/idempotency prevents duplicate grant or payment; sequence validated | DR/Payments | Critical |
| ADV-005 | Queue durability illusion | Isolate financial quorum and kill intake storage immediately after queue receipt | Either durable independently recoverable request or no acknowledged queued receipt | Intake | Critical |
| ADV-006 | Outage cancellation races queued payment | Queue cancellation during outage while payment queued before outage; restore out of order | Protective holds/cancellations reconciled before release; payment cannot race past cancellation | Intake/Payments | Critical |
| ADV-007 | Provider timeout after successful payment | Provider executes but response lost; system fails over and worker retries | Payment stays Unknown; no second submission without independently sufficient nonreceipt proof | Payments | Critical |
| ADV-008 | Payment destination substitution | Provider substitutes destination but returns matching self-issued fingerprint | Independent destination binding mismatch blocks automation; settlement reconciled | Payments | Critical |
| ADV-009 | Charity bank account takeover | Attacker changes destination with valid charity portal login and forged document | Ownership/control proof and independent change approval required; old/new destinations held until verified | Payments/Identity | Critical |
| ADV-010 | Cross-sponsor charity vetting contamination | False-positive common approval for prohibited recipient; distribute to all sponsors | Adverse finding propagates to all affected sponsors; no automated grant execution until reverified | Charity | Critical |
| ADV-011 | Vetting expiry at payment time | Verify charity at day 179, queue payment, execute at day 181 | Execution checks freshness and sanctions; stale evidence holds; no grandfathering by queue date | Charity | High |
| ADV-012 | Different sponsor restrictions despite common vetting | Shared charity passes common eligibility but sponsor policy bars recipient type | Common vetting reused, sponsor grant policy still blocks incompatible grant | Charity/Governance | High |
| ADV-013 | AI prompt injection via charity documents | Document says ignore rules and clear sanctions hold | Treat content as untrusted; AI cannot clear hold; evidence and rule decisions unchanged | AI | Critical |
| ADV-014 | AI hallucinated legal exemption | AI confidently asserts exception with no authoritative source | No autonomous approval; unresolved legal claim held and routed to authorized human/counsel | AI/Governance | Critical |
| ADV-015 | Rule engine version skew | Two workers evaluate same case under different policy versions | Pinned policy versions; no contradictory commitments; new material version requires revalidation | Rules | Critical |
| ADV-016 | Rule loophole by splitting grants | Submit many below-threshold grants via multiple donors or agents | Rolling aggregate exposure, related-party and recipient checks prevent threshold evasion | Rules | High |
| ADV-017 | Agent approves its own work through delegated identity | Originating agent submits and controls approval account or policy publication | Actual control independence checked; reject approval, record incident | Authority | Critical |
| ADV-018 | AI cost and latency runaway | Ambiguous documents cause repeated model loops or model unavailability | Bounded AI calls/time/cost, cache safe evidence, escalate or hold, never fail open | AI/Operations | High |
| ADV-019 | Human reviewer rubber-stamps AI | AI recommendation hides contrary primary evidence; reviewer approves quickly | Display evidence/conflicts and uncertainty; require independent attestation and audit sampling | Human/Governance | High |
| ADV-020 | Emergency service compromised | Attacker tries emergency hold removal, payee edit and transfer | Protective-only API rejects value movement; hold release requires recovered core and independent authorization | Emergency | Critical |
| ADV-021 | Fee changes during grant queue | Accrual or fees reduce funds after queue acknowledgment | Revalidate capacity at execution; atomic approval/reservation; no overdraft | Finance | High |
| ADV-022 | Donor death/revocation during outage | Agent queues recurring grant, donor dies or delegation revoked before recovery | No new authorization on replay; prior independently approved items separately evaluated | Authority/Intake | High |
| ADV-023 | Widget lies about outcome | Embedded widget receives queue receipt and renders Approved/Paid | Typed status contract renders Queued only; server-side receipt cannot be misinterpreted | UI | High |
| ADV-024 | Corrupted shared charity evidence | One source update overwrites prior valid evidence and hides adverse event | Append-only provenance/history, independent adverse evidence, tamper detection and hold | Charity/Data | Critical |
| ADV-025 | Simultaneous model and human policy conflict | AI suggests exception while deterministic rule prohibits grant; human tries override | Hard block persists; authorized policy/evidence correction only, not ordinary override | Tiers/Governance | Critical |
| ADV-026 | Rules falsely auto-approve due to incomplete inputs | Critical attribute null is interpreted as false or zero | Three-valued unknown state; missing critical evidence routes to evidence collection/hold, not approval | Rules | Critical |
| ADV-027 | Backlog surge after recovery | Millions of queued grants replay with provider rate limits and sanctions changes | Backpressure, prioritization, fairness, dedupe, revalidation and controlled release; no bypass to meet SLA | Intake/Operations | High |
| ADV-028 | AI data exfiltration | Malicious evidence instructs agent to export donor/charity data to external URL | No unapproved network/tool permissions; scoped data, DLP, audit and safe failure | AI/Security | Critical |
| ADV-029 | Cross-tenant leakage via shared vetting | Sponsor A's donor-specific grant data appears in Sponsor B charity evidence | Share charity facts only; isolate sponsor/donor sensitive data and permissions | Charity/Privacy | Critical |
| ADV-030 | Incorrect provider finality claim | Provider reports settled while bank later returns or reverses payment | Separate initiated, submitted, settled, final and returned; reconcile rail-specific finality | Payments/Finance | Critical |

## Cross-domain contradictions and resolution proposals

1. **Rules-first vs AI-native branding:** AI-native means redesigning workflows to use automation efficiently, not inserting models into every decision. Rules-first is a stronger operating model; instrument the escalation rate and cost per completed outcome.
2. **Uniform charity vetting vs sponsor accountability:** One platform-wide vetting standard and evidence record; sponsor grant approval remains independently controlled. Counsel must validate cross-sponsor legal reliance, jurisdictions, private foundations/foreign charities and recipient-specific requirements. Never silently downgrade common vetting to satisfy one sponsor.
3. **No self-approval vs automatic routine grants:** Board-delegated independently governed deterministic policy may approve routine grants if law and sponsor governance permit. The author/operator of that policy must not independently publish a change that approves their own items. AI's own recommendation cannot constitute sponsor approval.
4. **Lossless DR vs continuous financial availability:** Zero loss for acknowledged authoritative commits requires quorum durability; loss of quorum means temporary write unavailability. Independent outage intake is not authoritative financial commit. Queue cannot claim receipt without separate proven durability.
5. **Emergency hold vs payment replay:** Protective holds and revocations must reconcile before financial queue replay; hold removal requires separate independent authorization after restored authoritative integrity.
6. **Verified destination vs opaque rails:** A provider fingerprint alone is insufficient. Require independently verifiable charity ownership/control and actual execution destination binding, including tokenized rail proof. Nonconforming rails cannot be fully automated.
7. **AI judgment vs audit reproducibility:** Preserve model version, prompt template, input hashes, evidence, output, deterministic policy outcome and decision authority. Model nondeterminism must never create nondeterministic authoritative financial effects.
8. **Tenant isolation vs common evidence:** Charity-level verification facts are reusable; donor identities, sponsor transactions, confidential exception reasoning and personal information are not automatically shared.
9. **Rules engine drift vs historical reconstruction:** Decisions are pinned to policy version and input snapshot; retroactive policy edits must not rewrite history. New safety signals may block future release even after prior approval.
10. **Queue batching vs financial fairness:** Intake order does not confer reservation priority or grant approval. On replay use governed ordering and capacity rules, with rate limits and protection from starvation.

## Proposed validation sequence

**Gate A: specification review.** Map each scenario to original requirement IDs and ownership, define exact authorized state transitions, legal constraints and observable evidence. **Gate B: model-based tests.** Property-based testing for no double execution, no self-approval, no overcommitment, no approval without authority, no acknowledged data loss, no prohibited payment. **Gate C: fault injection.** Region outage, network partitions, witness failure, queue-storage loss, stale failback, provider timeout and compromised emergency credentials. **Gate D: adversarial AI and human studies.** Prompt injection, evidence laundering, confident hallucinations, reviewer anchoring and policy drift. **Gate E: provider and counsel verification.** Contractual destination evidence, payment finality, legal cross-sponsor reliance and sponsor-delegated approval. **Gate F: operational benchmarks.** Measure straight-through-processing rate, rules-only %, AI escalation %, human escalation %, false approvals, false holds, AI cost per resolved exception, human minutes per case, p95/p99 latency, backlog age, RPO and RTO. No numeric targets are asserted without measured baselines and risk appetite.

## Unresolved decisions and evidence

- Define classes of AI-authorized reversible low-risk decisions, if any, and required error/risk bounds. Default: AI recommends only where the action is financial, regulatory or irreversible.
- Define sponsor-independent policy publication approval, emergency hold release authority and legally required human intervention by action.
- Validate original REG-007/REG-057/REG-058/REG-061 source cross-reference (TVC-034). Do not silently replace guessed citations.
- Determine DR topology, third quorum domain, measurable RTO, queue durability architecture, privacy and cost tradeoffs.
- Determine payment rails/providers able to prove independently verified destination and externally final payment outcomes.
- Define common charity vetting scope and cross-sponsor legal reliance, including special recipient classes and jurisdictions.

## Acceptance gates for future implementation

No release if any critical scenario fails or lacks required evidence; no unapproved policy or self-approval; no financial execution during unsafe quorum; no AI-generated hold clearance; no opaque automated destination; no cross-tenant leakage. Document every exception with accountable owner, expiry and risk, and require explicit governance authorization before changing the gate.
