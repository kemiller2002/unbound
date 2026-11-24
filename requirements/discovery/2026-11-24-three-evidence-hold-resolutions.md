# Evidence hold resolutions

Product-design directions approved under delegated review; professional confirmations remain outstanding.

## RR-REQ-X-073-r1
Source: requirements/discovery/2026-11-23-cross-industry/data/refinements.jsonl, parent REQ-X-073.
Approve 3-month bridge request and 12-month stale-report alert as provisional monitoring defaults, not universal audit standards. Bridge letters do not extend independent assurance. Track coverage gap, report scope, opinion, exceptions, complementary controls and time-limited risk acceptance independently. Critical vendor suspension requires risk-based continuity decision. Test bridge letter with unmapped CUEC, adverse opinion, expired risk acceptance and vendor outage. Auditor confirms reliance criteria.

## RR-UNB-GOV-002-r1
Source: requirements/discovery/2026-11-24-simplification-economics/data/refinements.jsonl, parent UNB-GOV-002.
Reject blanket one-approver tightening exception. Require independent approval for material changes to financial controls, eligibility, permissions or customer rights regardless of direction. Allow streamlined low-risk changes only within independently approved change policy; ambiguous or mixed changes treated as material. Back-test representative history plus synthetic edge cases. Test lowered grant limit creating deadline breaches, compound changes, author self-approval and missing evidence. Estimated 50 hours/year savings unvalidated. Auditor/CPA confirms reliance.

## UNB-XIN-ECO-005 / XID-ECO-009
Source: requirements/discovery/2026-11-23-cross-industry/data/gap-requirements.jsonl.
Pin FOCUS 1.4 as internal mapping target, not mandatory provider format. Official specification ratified June 4 2026; verified Nov 24 2026: https://focus.finops.org/what-is-focus/ and https://focus.finops.org/docs/specification/v1-4/. Provider listing https://focus.finops.org/docs/implementation/get-started/get-started-with-focus-datasets/ shows AWS 1.2 and several providers 1.3; use version-aware adapters. Preserve native invoice and usage provenance; extensions x_Bearer, x_SourceInvoiceRef, x_CostObject; distinguish fund costs from sponsor and operator costs. Use invoice reconciliation and correction semantics; do not claim full FOCUS conformance for subset. Low-volume vendors may use invoice-only ingestion. Test older schemas, corrections, credits, currencies, residuals and duplicate invoices. Monitor future 1.5 release but no automatic upgrade.

## Status
143/143 requirement questions now have product-direction dispositions including these three. Source records and register still require reconciliation. No professional approval, implementation, or daf-landscape changes.
