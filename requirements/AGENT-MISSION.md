# Agent mission: requirements discovery, not application construction
Target: kemiller2002/unbound
Read requirements/CHARTER.md, PRINCIPLES.md and BASELINE-001.md first.

1. Discover the current Echelon system inventory from echelon-registry, its catalog, profiles, release manifests and source repositories. Check actual latest compatible releases, tests, docs and AGENTS instructions; do not infer from stale lists. Create requirements/echelon-capability-matrix.md with evidence URLs, status, compatibility and decisions. All relevant EF systems must participate in their appropriate lifecycle, not necessarily runtime.
2. Audit daf-landscape research, identify evidence, disagreements, gaps and freshness. Cite original sources; mark unknowns. Do not copy unverified conclusions into law or policy.
3. Expand all twelve domains from CHARTER into detailed individually numbered outcome-oriented requirements. Each must include rationale, authority type, source, jurisdiction, owner, dependencies, normal/negative/exception scenarios, measurable criteria and validation.
4. Develop at least three nontrivially different organizational and operating models; compare with common scenarios, costs, controls and failure modes. Challenge the need for departments, queues, forms and legacy handoffs.
5. Map complete end-to-end donor, sponsor, advisor, grantee, asset, finance, governance and exception journeys. Include uncommon and adversarial cases.
6. Build a regulatory source register and qualified-review queue. Do not present unsettled interpretations as settled law.
7. Create requirements traceability, contradiction log, assumption register, risk register, decision log and EF autonomy/cost evidence ledger.
8. Use the Knowledge Store's *current supported* format and compiler where actually available; do not invent released schemas.
9. Validate the requirements against current Praxis and Dokimos governance, record failures and remediation. No implementation code.
10. Produce a readiness report with coverage counts, outstanding blockers, review decisions and an explicit PASS/FAIL for the requirements gate. FAIL is acceptable; unsupported PASS is not.

Execution discipline: work in bounded, checkpointed batches; commit coherent increments; respect repository governance and branch rules; do not auto-merge failed checks. Use only F# for eventual product code unless a verified EF tool contract requires otherwise. Record each EF component's version and rationale at the time of use.
