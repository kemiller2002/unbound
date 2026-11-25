"""Convergence clauses from the Phase 6 business adversarial review (findings ADV-B-001..033, report 08)."""
from .common import clause, OWN

ECON = lambda: OWN("Economics", "n/a (non-posting model)", "Unbound CFO and product owner", "Unbound", "Unbound finance", "Unbound finance")
PROD = lambda: OWN("Product", "n/a", "Unbound product owner", "Unbound", "Unbound operations", "Unbound product owner")

CLAUSES = (
    clause("CNV-ECO-003", "Economic claims and commercial commitments gated on evidence",
           "No licence price list, customer cost-saving claim or segment commitment SHALL be approved until (a) a shadow replay of at least "
           "12 months of a real sponsor's grant and contribution history and (b) a 90-day time study have measured exception incidence, handling "
           "minutes, determination-reuse share, complex-gift handling hours and contact rates. Economic comparisons SHALL use the same exception "
           "definition and source for every approach, state each lever separately with its evidence and activation gate, headline Monte Carlo "
           "P50 with the P10–P90 range, and use the no-lever Unbound case (C0) as base until levers are measured. The licence used in any "
           "comparison SHALL be computed at the platform scale reachable by named target sponsors within five years, never above the "
           "addressable market. Unbound SHALL maintain a vendor P&L (revenue ramp by segment, sales and implementation per sponsor, build and "
           "maintenance capex, compliance staff, assurance fees) and a configuration SHALL be called viable only if contribution margin is "
           "positive at P90 cost and adverse licence scale. Per-tenant costs SHALL be charged on each sponsor's actual fund count. Customer-facing "
           "claims SHALL attribute savings by lever (policy delegation, determination reuse, outsourcing, software).",
           ["Given a price list submitted before the shadow replay and time study are recorded, when approval is requested, then it is refused.",
            "Given a model run, when rendered, then every per-approach override appears in the run log with its evidence label and C0 is reported beside C.",
            "Given a licence scale above the addressable traditional DAF market, when used in a comparison, then the run is labelled market-exceeding and excluded from headlines."],
           ["SPD-CMP-001", "SPD-ECN-001", "SPD-CLR-001"], ["UNB-SMP-ECN-004", "UNB-XIN-ECO-003"],
           "ECO", "commercial-availability", "Empirical: shadow replay and 90-day time study at a real sponsor.", ECON(), []),
    clause("CNV-ECO-004", "Market entry segment and segment activation",
           "The initial target segment SHALL be mid-size national and single-issue sponsors (about 5,000–50,000 funds) and advisor-distributed "
           "sponsors, subject to CNV-ECO-003. Community foundations SHALL NOT be the entry segment: at observed sizes (median 46, p75 128, p90 "
           "313 DAFs among 802 US community foundations, DAFRC FY2024) redesign adds nothing over licensed traditional software and their "
           "systems must cover non-DAF fund types (designated, field-of-interest, scholarship, agency, endowment spending) that are out of "
           "scope; serving them requires either a partnership with community-foundation software or a separately scoped requirement set. "
           "High-volume workplace giving and outsourced-administrator models SHALL remain architecturally supported but not offered until an "
           "executed employer or administrator funding commitment covers C cost at adverse licence scale. The advisor-distributed segment SHALL "
           "be offered on sponsor admin fees only; any advisor-sponsored structure requires a sponsor-engaged-manager program defined under "
           "CNV-GRT-009. A target sponsor list with incumbents and switching triggers SHALL be maintained; no sponsor SHALL exceed 25% of annual "
           "recurring revenue after year 3 without a board-approved concentration exception.",
           ["Given a community foundation prospect, when a sales commitment is proposed, then it requires a recorded partnership or scoped non-DAF requirement set.",
            "Given an E4 activation request without an executed employer funding commitment, when submitted, then it is refused."],
           ["SPD-CMP-001", "XID-ADV-002", "SPD-ECN-002"], ["UNB-SMP-ECN-003"],
           "ECO", "commercial-availability", "", ECON(), []),
    clause("CNV-ECO-005", "Pre-delegation operation, stress scenarios and integration register",
           "Sponsor economics SHALL be computed for a pre-delegation phase (no deterministic approval, three-distinct-person release, manual NAV "
           "checks) of parameterised duration before steady state, and onboarding plans SHALL schedule every activation gate (board delegations, "
           "auditor planned approach, ODFI and rail agreements, controller policies). The model SHALL include stress scenarios for final §4966 "
           "regulations as proposed, a 15% contribution decline, loss of the largest sponsor, licence renewal price pressure of 20% in downturn "
           "years, revenue concentration loss (top 5% of accounts) and December surge staffing costed to meet each sponsor's approved SLA floor. "
           "Each external dependency SHALL be in an integration register with counterparty, contract status, cost bearer, lead time and "
           "fallback; a sponsor activation plan SHALL NOT be marked ready while a mandatory dependency lacks an executed contract or fallback. "
           "A fixed control-time parameter per sponsor (board, controller, compliance, segregation of duties, roster coverage) SHALL be derived "
           "from a role-by-role inventory and reported separately.",
           ["Given a sponsor with no recorded board delegation, when its year-1 cost is reported, then the pre-delegation figure is shown.",
            "Given an ODFI agreement in negotiation, when activation readiness is assessed, then readiness is false."],
           ["SPD-ECN-004", "SPD-CLR-003"], [],
           "ECO", "architectural-support", "", ECON(), []),
    clause("CNV-GOV-008", "Board oversight of delegated automation and ratification status",
           "Where a board has delegated approval to a deterministic policy, the platform SHALL produce a quarterly board report of auto-approved, "
           "held, declined and human-reviewed grants with sampled human reviews, evidencing that the sponsor exercises variance power and "
           "exclusive legal control, and SHALL refuse to renew a delegation whose last report was not acknowledged by the board or committee. "
           "Clauses whose resolving questions are agent-recommended SHALL be shown as unratified in the baseline index and SHALL NOT be cited in "
           "customer-facing material until the owner ratifies them. A sponsor that does not adopt a mandatory policy-pack change within its "
           "deadline SHALL have the affected feature fail safe.",
           ["Given a delegation whose quarterly report is unacknowledged, when renewal is requested, then it is refused.",
            "Given an unratified clause, when marketing copy cites it, then the content check fails."],
           ["SPD-GRT-001"], ["REQ-X-007"],
           "GOV", "sponsor-activation", "Sponsor board acknowledgment of quarterly delegation reports.", PROD(), []),
    clause("CNV-GOV-009", "Commercial licence and dependency rights",
           "No release-1 production dependency SHALL be under a non-commercial licence without an executed commercial licence plus escrow or a "
           "fork right; a software bill of materials licence check SHALL fail on PolyForm Noncommercial components lacking such a licence. "
           "Dependency versions SHALL be pinned to tagged releases with digests through Conditor.",
           ["Given an SBOM containing a PolyForm Noncommercial component with no licence record, when the release check runs, then it fails."],
           ["RCD-027"], [],
           "GOV", "commercial-availability", "Executed commercial licence for Echelon components.", PROD(), []),
    clause("CNV-EXP-002", "Switching support for donors and charities",
           "Sponsor migration SHALL import each donor's full fund history (contributions, grants, acknowledgments, documents) from the predecessor "
           "platform with a reconciliation report, so switching sponsors' systems does not degrade the donor record. Charities already enrolled "
           "with a supported networked grant-payment partner SHALL be paid through that enrolment without a new Unbound enrolment. Lawful "
           "distribution-partner incentives (fixed-fee platform partnerships; referral payments only where registered and permitted) SHALL be "
           "defined before partner channels launch.",
           ["Given a migrated fund, when the import completes, then contribution and grant totals reconcile to the predecessor's statements with no unexplained difference.",
            "Given a grantee enrolled with a supported rail partner, when a grant is released, then no additional enrolment is requested."],
           ["SPD-CMP-003"], ["UNB-RCN-ENT-007"],
           "EXP", "initial-implementation", "", PROD(), []),
    clause("CNV-EVB-002", "Business evidence tasks",
           "The evidence backlog SHALL include: at least three incumbent software price points (from Form 990 contractor disclosures, RFPs or "
           "sponsor interviews) to re-run the licensed-traditional comparator; sponsor sales-cycle length and implementation cost from at least "
           "three recent platform migrations; partner willingness-to-distribute from at least three wealth platforms; employer willingness to "
           "pay; the share of target-sponsor grantees already enrolled with networked rails; and a 50-state survey of platform, fundraising "
           "counsel and commercial co-venturer registration applicable to a white-label DAF platform built from statute and regulator sources.",
           ["Given the evidence backlog, when the readiness report is generated, then each item shows owner, due date and status."],
           [], [],
           "ECO", "architectural-support", "Empirical: listed measurements.", ECON(), [], classification="evidence-backlog"),
)
