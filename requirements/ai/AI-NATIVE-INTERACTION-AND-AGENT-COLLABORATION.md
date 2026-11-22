# AI-Native Interaction and Agent Collaboration Requirements
2026-11-22 | Requirements/research only. No implementation authorized.

## Outcome model
Preserve the outcome, not the process. Each donor, advisor, sponsor, partner or administrator goal is represented as a durable outcome case, with versioned scope, authoritative state, evidence, dependencies, next actions and authority limits. The interface should not require the user to know the sponsor's internal departmental structure.

## Agent operating contract
Agents gather, compare, classify, prepare, explain and propose. They cannot grant themselves authority, publish legal conclusions, approve reserved sponsor decisions, trade, post unsupported accounting entries or execute live payments. Delegation to other agents does not expand privileges. Every consequential recommendation must be attributable, source-linked and contestable.

## Requirements
- **AIN-001 Outcome-first agent work** (Orchestration): Agents begin from a sponsor/donor outcome, not an inherited department workflow.
- **AIN-002 Explicit agent capability manifest** (Governance): Every agent advertises tools, data scope, allowed actions, authority ceiling and escalation routes.
- **AIN-003 Human authority checkpoints** (Governance): Reserved legal, grant, financial, trading and payment decisions require authorized humans.
- **AIN-004 Evidence-linked recommendations** (Trust): Every consequential proposal includes sources, freshness, uncertainties, assumptions and alternative interpretations.
- **AIN-005 Separate facts from inferences** (Trust): UI and APIs label observed fact, unverified claim, inference, recommendation and authorized decision.
- **AIN-006 Agent action preview** (UI): Show expected effects, affected DAFs, reversible and irreversible changes, and required approval.
- **AIN-007 Progressive information gathering** (Experience): Ask only for missing material data; reuse verified evidence within scope and consent.
- **AIN-008 Adaptive conversation and forms** (Experience): Support conversational and structured task completion with equivalent authority and validation.
- **AIN-009 Persistent outcome case** (Operations): Every multi-step goal has durable case identity, state, evidence, tasks, blockers and owner.
- **AIN-010 Agent collaboration** (AI): Agents can delegate bounded subtasks with attributable provenance and no authority amplification.
- **AIN-011 Agent conflict handling** (AI): Conflicting agent findings produce a reviewable discrepancy, not silent winner selection.
- **AIN-012 Prompt injection isolation** (Security): Treat documents, messages, retrieved web content and tool results as untrusted instructions.
- **AIN-013 Agent identity and attribution** (Governance): Record agent identity, version, tool action, initiator, delegation and effective permission.
- **AIN-014 No synthetic success claims** (Trust): Drafted, requested, submitted, accepted, posted and settled remain separate statuses.
- **AIN-015 Long-running task visibility** (UI): Show progress, checkpoints, waiting dependencies, stale state, retries and escalation.
- **AIN-016 Agent-to-human handoff** (Operations): Transfer full evidence, context, decisions and pending work without forcing user repetition.
- **AIN-017 Human-to-agent resumption** (Operations): Resume only within renewed authorization and updated evidence after human decision.
- **AIN-018 Interrupt and cancellation** (Operations): Allow authorized pause/cancel with explicit in-flight and irreversible-action semantics.
- **AIN-019 Timeout and retry discipline** (Reliability): Idempotent retry, bounded backoff, dead-letter and explicit uncertain outcome reconciliation.
- **AIN-020 No blind replay** (Reliability): Revalidate authority, policy, source freshness and material facts before resuming delayed work.
- **AIN-021 Cross-workflow coordination** (Operations): Detect shared account, asset, recipient, grant and participant dependencies and collisions.
- **AIN-022 Material change invalidation** (Governance): Revoke affected proposals and approvals when amount, beneficiary, authority or valuation changes.
- **AIN-023 Privacy-safe proactive outreach** (Experience): Trigger only permitted, relevant notifications with consent, timing and minimum necessary data.
- **AIN-024 Accessibility across modalities** (UI): Equivalent keyboard, screen-reader, mobile, structured and conversational paths.
- **AIN-025 Explanations at decision time** (UI): Explain why a decision is blocked, who can resolve it and what evidence is required.
- **AIN-026 User corrections and disputes** (Trust): Allow challenge of agent facts with preserved provenance, correction and independent review.
- **AIN-027 Model and prompt change controls** (Governance): Version models, prompts, tools and policies with impact review and regression scenarios.
- **AIN-028 Agent cost and latency boundaries** (Operations): Bound token, tool, time and external service budgets without silently abandoning work.
- **AIN-029 External widget safety** (Security): Embeds independently authorize data/actions, expire safely and cannot impersonate the sponsor.
- **AIN-030 Event-driven proactive servicing** (Experience): Detect actionable conditions without inventing authority or sending duplicate outreach.
- **AIN-031 Multi-channel continuity** (Experience): Maintain consistent authorized case state across donor portal, staff workspace, partner widget and notices.
- **AIN-032 Financial control preservation** (Finance): Agent suggestions cannot create unauthorized ledger entries, trades, NAV overrides or live payments.
- **AIN-033 Independent assurance monitoring** (Assurance): Missing agent traces, failed checks or unavailable collectors create deficiencies, not green health.
- **AIN-034 Operational learning** (Analytics): Capture repeat failures, correction rates, user effort and remediation proposals without unreviewed policy change.
- **AIN-035 Policy-as-authority boundary** (Compliance): Use versioned sponsor-approved rules; agents cannot publish or reinterpret law as binding policy.
- **AIN-036 Failure-mode traceability** (Simulation): Every workflow and transition links to simulation cases, detection, containment and recovery requirements.

## Interaction patterns
**Proactive:** An eligible business event triggers an agent to prepare a case, confirm relevance and consent, then notify an authorized person only if action is needed.
**Conversational:** A donor describes the outcome; the system clarifies missing essentials, provides a structured confirmation and records an explicit recommendation rather than pretending a sponsor decision occurred.
**Structured:** Staff see decision cards with reason, source evidence, authority, affected accounts, risks and next permitted actions.
**Collaborative:** Agents can gather parallel evidence, but contradictory conclusions are surfaced, not blended into false certainty.
**Handoff:** Humans receive a complete evidence bundle, proposed resolution, unresolved uncertainties and effects of approving or rejecting.
**Resume:** A delayed case revalidates identity, policy, valuations, charity eligibility and approval scope before any consequential continuation.
**Recovery:** Retry is idempotent and bounded. Unknown outcome requires reconciliation before another command.

## Forma UI requirements
Agent Activity and Evidence; Outcome Case; Next Best Action; Decision Review; Human Escalation Inbox; Agent Conflict Resolution; Source Comparison; Automation Permission Review; Long-running Task Timeline; Notification Preferences; Embedded Agent Task Widget. Each supports mobile, WCAG 2.2 AA target, no blank WASM loading state, accessible nonconversational alternative, explicit stale/offline/error states and source traceability.

## Assurance
Agent proposal and human authorization are separate records. Monitoring must detect unauthorized tool attempts, missing trace evidence, stale source decisions, model changes and prompt injection. A synthetic test never proves operating effectiveness. All workflows must be prepared for future failure injection.

## Unresolved research
Actual sponsor delegations, agent model reliability, prompt-injection defenses, source-specific legal rights, exact Limen/Forma/Folio interfaces, operational SLAs, accessibility user testing and authorized communication policies require validation.
