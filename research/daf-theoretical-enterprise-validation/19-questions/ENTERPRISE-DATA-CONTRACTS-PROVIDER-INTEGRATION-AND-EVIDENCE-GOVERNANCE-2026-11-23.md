# Enterprise data contracts, provider integration and evidence governance
Status: proposed requirements, not implemented. 2026-11-23.

## Principle
External providers supply claims, evidence or execute instructions. They do not independently decide Unbound's financial truth or override authoritative domain commands. All integrations live behind typed adapters in the component monolith, with durable outbox/inbox, schema versions, provenance and idempotency. No default microservices.

## Provider families
Custody/brokerage and valuation; ACH/wire/check/payment processors; charity registry and sanctions sources; banking/lockbox; CRM/wealth-advisor systems; email/SMS/mail vendors; identity/KYC; sponsor websites and embedded widgets; export/audit and regulatory recipients.

## Typed exchange contract
ProviderId, SponsorScope, OperationId, IdempotencyKey, ExternalReferenceIds, ProviderEventId, EventTime, ReceivedAt, BusinessEffectiveAt, PayloadSchemaVersion, SourceDigest, SignatureValidation, Freshness, EvidenceStatus(Known/Unknown/Disputed/Stale), DomainMappingVersion, RetryPolicy, ReconciliationRef. Do not mistake external IDs for global unique authoritative IDs. Support multiple IDs and manual check handoff evidence.

## Lifecycle and failure semantics
Inbound: authenticate -> validate -> deduplicate -> normalize -> preserve original evidence -> map to domain facts -> reconcile -> authorize domain command -> record result. Outbound: authorize -> reserve/commit domain intent -> durable dispatch -> observe provider status -> reconcile settlement -> close. A timeout or lost callback is Unknown, never Failed; investigate before retrying irreversible actions. Model provider accepted, submitted, processing, settled, returned and reversed distinctly. Use polling/webhook reconciliation with rate budgets and provider-specific capability matrix.

## Safety and privacy
Least-privilege credentials per sponsor where supported, rotation, secrets vault, signed webhooks, replay protection, PII minimization, field-level authorization, audit trails, external data retention, DPA and provider offboarding. External charity evidence does not replace sponsor grant rules; charity control of destination must be independently checked. Verify custody records against GL, not vice versa by blind overwrite.

## Contract testing and mock mode
Every adapter must support deterministic mock interactions, seeded success/failure/timeout/out-of-order/duplicate/stale cases and replay. Contract suites verify schemas, signatures, money amounts, currencies, settlement states, idempotency, provider reconciliation and tenant isolation. Mock mode must never call production money-moving endpoints; test data visibly labeled and isolated.

## UI and validation
Integration Operations console shows coverage, freshness, errors, Unknown operations, provider incidents, last reconciled watermark, credentials health (without secrets), replay queue and safe actions. Test duplicate callbacks, late settlement, lost acknowledgment, signature failure, stale custody prices, changed provider schema, tenant mismatch, provider partial outage, malicious payload and reconciliation divergence.
