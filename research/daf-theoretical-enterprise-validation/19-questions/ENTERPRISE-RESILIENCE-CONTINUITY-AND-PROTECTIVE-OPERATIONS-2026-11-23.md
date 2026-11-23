# Enterprise resilience, continuity and protective operations
Status: proposed requirements, not implemented or fault-tested. 2026-11-23.

## Principle
Protect money, authoritative accounting, legal holds and access controls before optimizing availability. One component monolith with authoritative transactional core and managed services. A narrow emergency protective plane may freeze/revoke/hold but cannot approve or move money. No Kubernetes/Terraform assumption.

## Failure domains
Application crash, database failover, stale replica, region outage, object-store unavailability, encryption/key outage, payment-provider ambiguity, custodian feed delay, charity source failure, compromised credentials, cross-sponsor data leak, corrupt archive, failed statement batch and communication outage.

## Continuity rules
Acknowledged authoritative financial commits require zero data loss under explicitly stated replication/fault assumptions, quorum durability and fenced single writer; RPO zero is not guaranteed for arbitrary catastrophic correlated failures. No split brain, no stale-replica financial authorization. Freeze unsafe operations when authoritative truth or holds cannot be verified. Durable outage intake may collect uncommitted requests but must label them Queued, not Approved/Paid. Recovery preserves idempotency, ordering, global holds, reservations and audit.

## Recovery design
Define RTO/RPO by operation and failure scenario, with board-approved tradeoffs. Recovery runbooks for sponsor portal, accounting, payments, document archive, charity evidence, communications and IVR. Separate read-only archive availability from money-moving authority. Restore and verify exact financial PDF hashes, manifest, source evidence and encryption keys for seven-year minimum. Provider status reconciliation is mandatory before resuming irreversible dispatch.

## Security incident and isolation
Incident classification, containment, credential rotation, tenant-scoped blast-radius analysis, forensic preservation, legal/privacy notifications determined by counsel, protective holds, evidence chain and post-incident prevention. Cross-sponsor isolation enforced in authorization and data queries, not merely UI filters. No emergency super-admin financial bypass.

## Validation and exercises
Fault injection, crash-restart at each transactional boundary, concurrent hold and payment execution, stale replica, regional failover fencing, object restore/hash verification, webhook replay, provider Unknown recovery, compromised credential, backup/key loss and sponsor offboarding. Exercises must record assumptions, observed results, deviations and corrective actions; none have yet been run.

## UI
Resilience Command Center: authoritative health, read-only/degraded state, queued requests, protective holds, provider dependency status, RTO/RPO assumptions, incident timeline, reconciliation before resume, tenant exposure and recovery evidence. Never present unverified recovery as success.
