# Financial document retention: seven-year minimum, legal holds and lifecycle
Date: 2026-11-23. Proposed owner-directed policy and requirements. No implementation or legal certification.

## Owner policy decision
**All financial documents must be retained for at least seven years.** This is a platform minimum and cannot be shortened by sponsor settings, administrators, account closure, advisor changes, storage migrations or ordinary deletion requests. A longer applicable law, regulatory obligation, contract, sponsor-approved policy or legal hold controls. Seven years is not a representation that deletion after seven years is legally permitted in every jurisdiction.

## Definition of financial document
Include issued monthly/annual fund statements and their revisions; contribution acknowledgments and tax-related receipts; grant approvals, payment/disbursement records and letters; check issuance and remittance evidence; financial fee notices/invoices; transfers, valuation reports and reconciliations; investment/custodian statements; financial adjustments, reversals and correction evidence; and records/attachments that substantiate financial transactions. Classify financial source records and evidentiary attachments explicitly, even if not PDF or originally created in Unbound. A communication may be financially material and therefore subject to this policy even when stored in Communications.

## Retention clock
Default minimum: seven years from the end of the calendar year in which the document was **last officially issued**, or for underlying transaction evidence from the end of the accounting period/year to which it relates, unless a later final correction, settlement, closure event or legal trigger requires a longer period. The precise clock for each document class must be approved by compliance/counsel before implementation; do not silently use creation timestamp for all classes. RetentionUntil is computed as the maximum of platform floor, sponsor policy, applicable law/contract and hold/restriction conditions. Preserve original and corrected versions independently and link lineage. Never reset clocks arbitrarily to permit earlier deletion.

## Lifecycle and closure
Fund closure, donor death, succession, advisor revocation, sponsor offboarding, client migration, provider termination and user account deletion do not automatically erase financial records. Maintain controlled archival access for authorized former clients, successors, regulators and litigation needs under policy. Revocation changes who can see documents, not whether documents exist. After seven years, move to eligible-for-review, not automatic purge. Require approved disposition process, classification checks, hold checks and documented authority. Support retention of minimal tombstone/audit metadata after lawful deletion where appropriate.

## Legal holds
Authorized compliance/legal personnel can place holds at document, fund, sponsor, subject, case and time-range scope. Holds propagate to existing matching artifacts and future matching records, including attachments, originals, corrections, delivery evidence and relevant logs. Hold records include reason, legal matter reference, custodian, effective time, scope, reviewer, audit and release authority. No ordinary sponsor administrator can bypass or remove a legal hold. Release is a separately approved, audited event, not an immediate delete instruction. Holds supersede all scheduled deletion/expiration.

## Immutable storage and cryptographic evidence
Store exact issued artifact bytes and final SHA-256 in versioned, access-controlled immutable or retention-locked storage with a policy supporting at least seven years. Preserve canonical evidence manifests, source references, signing metadata, audit and correction lineage for equivalent or longer policy where necessary. Hash alone does not ensure seven-year availability: require backup/restore, redundancy, key management, periodic integrity verification, storage-provider lifecycle safeguards and documented disaster recovery. Prevent premature lifecycle expiration and deletion by privileged cloud operators through technical and procedural controls.

## Security, privacy and cost
Retention is not blanket visibility. Encrypt and restrict access to sensitive financial records; enforce sponsor/fund/recipient authorization and time-bounded administrative access. Apply data minimization to nonfinancial PII and separate deletion eligibility from retained financial evidence. Archive tiers can reduce cost only if recovery, latency, accessibility and verification remain adequate. Retention rules cover exported copies, accessible variants, print fulfillment evidence and third-party providers by contract where feasible.

## Operations and UI
Document Center: show issue date, correction lineage, available historical artifacts and controlled archive status, without revealing confidential legal hold details to ordinary users. Admin Retention Console: policy registry by document class/jurisdiction/sponsor, retention start/end, pending holds, archive health, integrity check results, review queue, disposition approvals and audit. Provide sponsor offboarding export/escrow plan and proof of preserved historical access.

## Acceptance criteria
1. No financial artifact or supporting evidence can be deleted before the seven-year floor.
2. Sponsor-specific retention may extend, never shorten, platform floor.
3. Legal holds prevent deletion across all matching present and future documents.
4. Fund closure, donor death, succession and sponsor termination do not trigger premature purge.
5. Original and corrected versions retained with lineage and independently verified hashes.
6. Storage restore preserves exact bytes, access policy, metadata and verification.
7. Expired records enter governed disposition review; no automatic deletion at seven years.
8. Unauthorized users cannot read retained records after access revocation.
9. Records transferred to archive remain searchable and retrievable by authorized parties.
10. Legal/regulatory retention exceptions override platform minimum.

## Adversarial cases (unexecuted)
ADV-335 sponsor sets retention to 3 years; ADV-336 document deleted after fund closure; ADV-337 legal hold added one day before expiry; ADV-338 hold applies to future corrected statement; ADV-339 original destroyed when correction issued; ADV-340 cloud object lifecycle expires at year 6; ADV-341 sponsor offboarding removes archive; ADV-342 revoked advisor accesses retained document; ADV-343 backup restoration changes bytes; ADV-344 source financial attachment omitted from retention; ADV-345 retention clock starts at draft creation rather than issue; ADV-346 privacy deletion request erases legally retained financial evidence; ADV-347 hold released by unauthorized staff; ADV-348 archive encryption key lost; ADV-349 document crosses jurisdiction with longer requirement; ADV-350 automated purge runs immediately after seven years; ADV-351 provider destroys mailed-check evidence early; ADV-352 historical artifact preserved but metadata/manifest lost; ADV-353 retention rule migration shortens existing expiry; ADV-354 immutable storage blocks lawful post-retention disposal indefinitely without reviewed plan.

## Open legal and technical decisions
Counsel/compliance must validate per-record trigger dates, applicable jurisdictions, legal/regulatory minima, litigation hold requirements and deletion exceptions. Define record classes that are financial, including mixed-content communications and third-party evidence. Select object retention/lock mechanism, key lifecycle and disaster recovery proof. Establish disposition approval and sponsor offboarding access. Do not claim seven years is universally sufficient under law.

Status: owner-directed seven-year platform floor; proposed design, not implemented or legally validated. ADV-335..ADV-354 not executed.
