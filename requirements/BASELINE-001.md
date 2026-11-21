# Unbound Enterprise Requirements Baseline 001
Status: candidate requirements; NOT approved. Each item needs source validation, scenario coverage and acceptance evidence.

## Enterprise and economic outcomes
- UNB-ENT-001: The enterprise SHALL express donor charitable intent as explicit goals, restrictions, and measurable outcomes independent of legacy forms.
- UNB-ENT-002: Every capability SHALL have an accountable owner, outcome metric, operating cost hypothesis, and documented alternative.
- UNB-ENT-003: The organization SHALL support a documented sponsor business model, revenue/fee policy, and sensitivity-tested unit economics.
- UNB-ENT-004: Enterprise operating decisions SHALL be explainable and reviewable without relying on an agent's private reasoning.

## Legal, authority and governance
- UNB-LEG-001: Every material legal assertion SHALL record jurisdiction, authoritative source, currency, interpretation, and review status.
- UNB-LEG-002: The sponsor SHALL retain required legal ownership/control and final authority over distributions; donor recommendations SHALL not be treated as binding instructions.
- UNB-LEG-003: The enterprise SHALL distinguish donor advice, sponsor decisions, operational execution, and audit evidence.
- UNB-LEG-004: Conflicts, prohibited benefits, sanctions and other applicable restrictions SHALL have documented review and escalation paths, subject to jurisdiction-specific verification.
- UNB-LEG-005: Changes to material policies SHALL be versioned, approved by authorized decision makers, and linked to impacted tests.

## Donor and grant outcomes
- UNB-DON-001: Donors SHALL be able to express charitable goals and recommendations in accessible channels without being forced into a single legacy workflow.
- UNB-DON-002: Contribution status SHALL be transparent while explicitly distinguishing pledged, received, valued, liquidated, and available funds.
- UNB-GRT-001: Every grant recommendation SHALL have a traceable eligibility determination, sponsor decision, execution state and reconciliation evidence.
- UNB-GRT-002: The enterprise SHALL handle incomplete, ambiguous, ineligible, disputed and potentially fraudulent recommendations safely.
- UNB-GRT-003: Charity due diligence SHALL be proportionate to documented risk and applicable requirements, with human escalation for exceptions.

## Assets, finance and advisors
- UNB-AST-001: The enterprise SHALL model cash, public securities and complex noncash contributions without pretending all assets settle identically.
- UNB-AST-002: Asset valuation, restrictions, custody state and liquidation SHALL have distinct, auditable records.
- UNB-FIN-001: All simulated financial events SHALL be recorded in balanced, immutable or correction-traceable accounting entries.
- UNB-FIN-002: Grants, fees, contributions and investment events SHALL reconcile to independent simulated statements; discrepancies SHALL block unsupported finality claims.
- UNB-ADV-001: Advisors SHALL operate only within explicit, revocable, scoped delegated permissions.
- UNB-ADV-002: Donor, advisor, sponsor and grantee roles SHALL have separately enforced access and information boundaries.

## AI, enterprise operations and technology
- UNB-AIO-001: Every agent action SHALL identify its permissions, inputs, policy version, result, evidence, and escalation route.
- UNB-AIO-002: High-impact, ambiguous, irreversible or legally reserved decisions SHALL require appropriate authorized human review.
- UNB-AIO-003: Agent unavailability, model changes and prompt injection SHALL not bypass deterministic financial or authorization controls.
- UNB-OPS-001: The enterprise SHALL support incident response, vendor failure, disputes, business continuity and recoverable audit trails.
- UNB-TEC-001: Integration SHALL use versioned contracts and the current Echelon Registry capability-discovery model; optional EF providers SHALL not break core operations.
- UNB-TEC-002: The intended implementation SHALL prioritize F#, WASM-first delivery, Limen and Forma where validated, managed persistence, and minimal operational complexity.
- UNB-TEC-003: Secrets, personal information and financial records SHALL have explicit classification, retention, access, and deletion policies.
- UNB-EXP-001: Every user journey SHALL be usable with keyboard, assistive technology, mobile layouts, and documented accessibility tests.
- UNB-EXP-002: Public identity SHALL be independently branded as Unbound, with transparent attribution to EF as its construction platform.
- UNB-VAL-001: Each requirement SHALL link to normal, exception and adversarial acceptance scenarios.
- UNB-VAL-002: Every claimed efficiency gain SHALL distinguish observed test measurements, modeled assumptions and unverified projections.
- UNB-VAL-003: The project SHALL log EF autonomous contributions, manual intervention, failures, cost, and reproducibility.

## Mandatory scenario families
Donor onboarding; restricted gifts; public securities; complex assets; grant to eligible charity; ambiguous charity identity; donor-benefit conflict; rejected recommendation; duplicate disbursement attempt; sanctions alert; market movement; reconciliation discrepancy; advisor revocation; accessibility failure; model outage; policy change; sponsor decision appeal; data correction; privacy incident.

## Open research blockers
Determine legal entity and sponsor model; applicable jurisdictions; public vs private reference scope; exact authoritative legal sources; charity verification methods; custody and banking partner assumptions; investment policy; fees; sponsor oversight model; regulatory obligations by operating geography; third-party assurance requirements. No blocker is silently deemed resolved.
