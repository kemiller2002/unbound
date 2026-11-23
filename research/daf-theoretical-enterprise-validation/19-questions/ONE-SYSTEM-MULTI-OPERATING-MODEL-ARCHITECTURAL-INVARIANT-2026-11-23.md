# One Unbound system: multi-operating-model architectural invariant
Date: 2026-11-23. Owner direction: **one system**. Requirements only, no implementation.

## Decision
Unbound is **one component monolith** with a single coordinated deployment and unified operational model. It supports multiple sponsors, service contracts, distribution channels and user personas by configuration, authorization, and adapters, not separate sponsor/wealth/adviser/charity/consumer applications or duplicative microservices.

## Non-negotiable boundaries
- One authoritative financial ledger and fund accounting subsystem, partitioned and reconciled by legal sponsor and fund. No independent balance engines in widgets, advisor portals, checkout or reports.
- One policy engine and versioned sponsor rule lifecycle. No AI policy engine, template policy engine, external widget approval logic or special grant engine for each channel.
- One identity, delegation and decision-rights framework. Role/relationship varies by sponsor and contract, but no parallel permission system.
- One work orchestration and case queue with routing policies; charity vetting, grants, operations and exceptions reuse it.
- One provider integration and evidence framework with typed adapters, idempotency, outbox/inbox, Unknown handling and deterministic mocks.
- One reporting semantic layer and artifact registry, one assistance contract catalog, one document and communication lifecycle.
- One brand and terminology system for white label, across portal, API, widget, email, print and communications.
- One shared component design system (Forma), authoring Studio, print Folio and F#/Limen browser app, subject to compatibility and commercial-license checks.

## Variation without fragmentation
**Sponsor-operated SaaS:** sponsor retains board/legal authority and configured staff roles.
**Outsourced administration:** provider staff perform authorized tasks while sponsor retains nondelegable responsibilities.
**Turnkey sponsor partnership:** wealth manager/distributor uses a legal sponsor partner; legal sponsor retains charitable ownership and authority.
**Embedded DAF checkout:** external systems initiate recommendations via constrained APIs/widgets; Unbound validates and processes through the same grant lifecycle.
**Digital-first donor channel:** mobile-first self-service shares the same fund, grant and payment commands.
**Workplace and adjacent channels:** partner integrations and optional capabilities remain inside the same model; separate legal charitable vehicles are not silently treated as DAFs.

## Unified core entities
LegalSponsor, SponsorContract, OperatingRole, Donor/Party, Account/Fund, Contribution, Asset, CustodyPosition, Valuation, Fee, GrantRecommendation, Charity, VettingEvidence, GrantDecision, Reservation, PaymentInstruction, PaymentOutcome, LedgerEntry, WorkCase, DocumentArtifact, PolicyVersion, Delegation, ExternalIdentifier, Communication, AuditEvent. Each has a canonical owner, sponsor scope, source/provenance, time semantics and permitted commands.

## Explicit prohibitions
No separate DAFpay ledger; no wealth-manager copy of authoritative balances; no sponsor-specific fork; no nonprofit-facing shadow grant state; no AI-initiated bypass of grant approval; no template-defined financial logic; no duplicated policy decisions across portals; no separate queue for each function. Protective-only DR plane remains the narrowly scoped exception already documented, never an alternate financial writer.

## Design challenge
"One system" should not force identical UX or universal data visibility. A nonprofit, donor, advisor, sponsor reviewer and operator need different task-oriented views. Likewise, legal segregation, sponsor-specific policies, custody and privacy are not optional. Avoid giant all-knowing modules: bounded components with in-process typed interfaces, tests and ownership preserve coherence.

## Acceptance
1. Every operating-model scenario SCN-01..SCN-10 uses the same authoritative ledger, policy, case, document and evidence contracts.
2. Sponsor separation holds under cross-tenant role switching and delegated administrators.
3. Embedded widgets and API clients cannot create a second approval/payment state machine.
4. A donor portal, nonprofit portal and wealth-manager dashboard display source-derived status, not local invented states.
5. Model-specific differences are declarative policies, authorized relationships, UI composition or adapter configurations, with reviewed exception ADRs.
6. A system diagram identifies each domain command's single authoritative owner and all legal/contractual responsibility boundaries.
7. Any proposal for a second deployable, ledger, policy engine, queue or shadow source of truth must be rejected unless an explicit, independently justified architecture ADR establishes a protective-only exception.

Status: requirements decision, not an implemented architectural guarantee.
