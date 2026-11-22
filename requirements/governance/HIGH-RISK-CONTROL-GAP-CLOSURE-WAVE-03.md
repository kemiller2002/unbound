# High-risk control validation and gap closure, wave 3
2026-11-22 | **Requirements specification only; no control execution, verification or SOC 2 attestation.**

## Scope
24 risk-prioritized control validation specifications covering sponsor authority, AI agent boundaries, charity eligibility and legal review, accounting and valuation, independent reconciliation, payments, tenant isolation and evidence integrity. Companion CSV specifies evidence, negative test, failure response, proposed owner and independent reviewer for each. Requirement references are illustrative links, not verified exhaustive mappings.

## Corrections to earlier mapping methodology
The 1,893 wave-2 links are keyword/domain-generated candidates. They are **not validated mappings**, and the fact that every requirement has a link does not mean every requirement has adequate controls. Do not use those candidate counts as assurance coverage. This wave defines more exact assertions but does not retroactively validate all 1,893 links.

## Validation acceptance rule
A mapping may be marked Validated only when (1) requirement semantics and control objective are compared by a reviewer, (2) the proposed control demonstrably prevents/detects the stated risk, (3) evidence source and independent proof are identified, (4) negative and positive test contracts are specified, (5) owner/reviewer are accountable and independent, and (6) the domain-specific legal/accounting obligations are separately assessed. TestExecuted and OperatingVerified are independent statuses and must remain false until observed.

## Design gaps that require new or strengthened controls
- **GAP-V01 Sponsor authority provenance:** board-approved delegation, effective date, scope and quorum must be independently evidenced.
- **GAP-V02 Legal-rule lifecycle:** versioned counsel-approved legal assertions and jurisdiction-specific effective dates; no AI-authored rules activated.
- **GAP-V03 Eligibility change between approval and payment:** reevaluation triggers, freshness and hold semantics.
- **GAP-V04 Reconciliation independence:** ledger and custody cannot both derive from the same imported feed and count as two independent sources.
- **GAP-V05 Payment egress safety:** simulated adapter plus network egress deny and alert; design alone is not an executed safety test.
- **GAP-V06 Missing-log detection:** independently monitored expected-evidence schedule, not a heartbeat produced only by the system being monitored.
- **GAP-V07 Evidence privacy:** prevent cross-tenant exposure and unnecessary secret/PII logging while preserving chain of custody.
- **GAP-V08 Auditor truthfulness:** synthetic fixtures and unverified controls excluded from claims of operating effectiveness.
- **GAP-V09 Exception closure independence:** the same agent cannot be sole actor, verifier and reviewer.
- **GAP-V10 Approval mutation:** changes to approved recipient, amount, funding sources or conditions trigger explicit reapproval policy.
- **GAP-V11 Regulatory research source recovery:** locate actual REG/CTL assertions in daf-landscape before mapping; earlier code search returned no matches.
- **GAP-V12 Full corpus coverage:** inspect remaining requirements, not just the 15 source documents used in wave 2.

## Control evidence and deterministic evaluation
A valid control observation requires controlId, version, actor and authority, event producer identity, tenant/legal entity, command/request hash, source evidence refs, expected outcome, actual outcome, timestamps, integrity digest, reviewer identity, test/production flag and policy version. Evaluator states: Pass, Fail, InsufficientEvidence, StaleEvidence, AwaitingReview, NotApplicable. **Pass requires independent corroboration, not merely a successful log.**

## Priority implementation sequence, when authorized
1. Authorization and tenant isolation, plus agent tool enforcement.
2. Protected evidence capture and independent missing-evidence monitor.
3. Grant approval, legal eligibility and sanctions gates.
4. Accounting/NAV/custody reconciliation and payment egress prohibition.
5. Independent review, exception closure and Folio audit packages.

## Honest status
24 high-risk control specifications defined; 0 reviewed/validated as operational controls; 0 negative tests executed; 0 evidence producers verified; 0 independent SOC 2 examinations. Existing 46 specific controls and 90 generic control patterns remain design artifacts.

## Related files
requirements/governance/DETAILED-REQUIREMENT-CONTROL-LINKS-WAVE-02.csv
requirements/governance/SPECIFIC-EXECUTABLE-CONTROLS.csv
requirements/governance/EXECUTABLE-ASSURANCE-ARCHITECTURE.md
requirements/governance/SOC2-CONTROLS-EVIDENCE-AND-ENTERPRISE-DIRECTIVE.md
