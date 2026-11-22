# Requirements-to-controls reconciliation, wave 1
Date: 2026-11-21. **Provisional traceability; not implementation or assurance.**

## Actual source inspection
Fetched 15 existing Unbound requirement files through the connected GitHub repository and extracted 494 unique per-document requirement identifiers for a preliminary row-level crosswalk. See companion CSV. Source counts:
- requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md: 48
- requirements/grants/GRANT-EVENT-CATALOG-AND-EXCEPTION-RESOLUTION.md: 40
- requirements/governance/SPONSOR-AUTHORITY-AND-AUTONOMOUS-OPERATIONS.md: 26
- requirements/governance/SOC2-CONTROLS-EVIDENCE-AND-ENTERPRISE-DIRECTIVE.md: 36
- requirements/charities/JURISDICTION-AWARE-REGULATORY-RULES-AND-ELIGIBILITY.md: 32
- requirements/FULL-ACCOUNTING-SYSTEM.md: 36
- requirements/investments/NAV-UNITIZATION-VALUATION-ENGINE.md: 53
- requirements/grants/MULTI-INVESTMENT-FUNDING-AND-PERCENTAGE-GRANTS.md: 30
- requirements/charities/FIRST-PARTY-CHARITY-REGISTRY-AND-ELIGIBILITY.md: 42
- requirements/charities/COMPREHENSIVE-EXTERNAL-DATA-ACQUISITION.md: 38
- requirements/investments/ACCOUNT-STRUCTURES-AND-PORTFOLIO-MANAGEMENT.md: 42
- requirements/integrations/CUSTODIAN-AND-INVESTMENT-MANAGEMENT.md: 28
- requirements/PARTNER-SEGREGATION-AND-TENANCY.md: 20
- requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md: 14
- requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md: 9

## Method and limitations
- Each extracted requirement maps to a proposed domain control from the existing 90-control matrix. The mapping is a **candidate**, not a verified assertion that the control objective fully satisfies the requirement.
- Some requirements span multiple controls; this initial table has one primary candidate only. Add secondary links and separate fiduciary/legal/financial obligation mappings in wave 2.
- Mapping rules are transparent keyword/domain heuristics. Human review is required for objective-level fit and to identify misclassifications.
- A requirement reference mentioned only as a dependency in another document is not counted as a defined requirement for that document.
- The existing enterprise matrix's three cross-cutting patterns per domain are **not sufficiently granular** to prove compliance. The column 'gap_or_review' marks likely need for more precise domain-specific controls.
- No test was executed and no evidence source was verified. All rows explicitly remain unvalidated.
- SOC 2 criteria mappings in the master matrix remain provisional. A qualified independent auditor must validate the actual criteria, scope, system boundaries and reporting period.

## Highest-priority control gaps
1. Legal eligibility and sanctions: distinguish source observation, counsel-approved legal applicability, sponsor-specific eligibility and prohibited grant advancement.
2. Sponsor authority: enforce committee approvals, segregation of duties, donor advisory boundaries and agent self-authorization prohibitions.
3. Accounting and NAV: define control-level reconciliations, independent pricing, posting authorization, valuation corrections and closing procedures.
4. Custody and payments: separate instruction, execution, acknowledgment, settlement and reconciliation; no live payment rail is authorized.
5. Charity source provenance: source rights, lawful acquisition, identity merges, bitemporal corrections and evidence freshness.
6. AI: agent tool permissions, change control, negative tests, escalation and audit evidence.
7. SOC 2 evidence: evidence collection integrity, missing-log detection, review and control-owner attestations.

## Next reconciliation wave
Inspect the remaining requirement documents and daf-landscape REG-001..065 and CTL-001..054 **from actual repository content**, then produce exact source-linked assertion-to-control-to-test rows. Deduplicate by semantics, not matching IDs. Assign each control named owner, actual evidence producer, reviewer, frequency, retention and failure threshold. Track unverified legal claims explicitly.

## Status
494 requirement-to-control candidate rows; 0 mappings validated; 0 operating controls verified; 0 tests executed; 0 auditor-reviewed controls. No SOC 2 compliance or attestation claimed.
