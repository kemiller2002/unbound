"""Convergence clauses: charitable law, eligibility, grants and sanctions (batches A, E, F)."""
from .common import clause, OWN

G = lambda: OWN("Grants and eligibility", "Unbound program ledger", "Sponsor board via DoA", "Sponsor", "Sponsor grant operations", "Sponsor compliance officer")

CLAUSES = (
    clause("CNV-GRT-001", "Board-delegated deterministic approval",
           "Deterministic grant, renewal and fund-acceptance policies SHALL produce recommendations (ApproveRecommended, "
           "AcceptRecommended) until a sponsor board or bylaw-authorized committee resolution is recorded that adopts the "
           "version-pinned policy, delegates final approval of conforming items to a named officer or committee and names who reviews "
           "exceptions. Only then MAY conforming items be approved by the policy under that delegate's authority. Each policy version "
           "SHALL record its §4966(d)(3) fund managers (officers, directors, trustees, the delegate and any employee with final authority). "
           "Without a delegation the operating model SHALL be individual accountable batch decisions on AI-prepared dossiers (SPD-CLR-004).",
           ["Given no recorded resolution, when a conforming grant is evaluated, then its state is ApproveRecommended and it waits for a named approver.",
            "Given a recorded resolution delegating to the CFO, when a conforming grant is evaluated, then it is approved with the CFO as accountable authority and the policy version recorded.",
            "Given a resolution that expires, when the next grant is evaluated, then approval reverts to recommendation."],
           ["RCD-005", "SPD-ENT-006", "RR-ADM-003-r1#Q1", "RR-ADM-002-r1#Q1", "SPD-GRT-001", "RR-REQ-X-007-r2#Q1", "SPD-CLR-004", "RR-UNB-GRT-003-r1#Q2", "RR-UNB-GRANT-030-r1#Q1"],
           ["REQ-X-007", "RR-ADM-003-r1", "RR-REQ-X-007-r2", "RR-UNB-GRT-003-r1", "RR-UNB-GRANT-030-r1"],
           "GRT", "sponsor-activation", "Sponsor board (or bylaw-authorized committee) resolution adopting the policy version and naming the delegate.", G(), ["EV-A", "EV-G"]),
    clause("CNV-GRT-002", "Eligible recipient classes",
           "Release 1 SHALL support grants to domestic §170(b)(1)(A) public charities (including private operating foundations and "
           "§170(b)(1)(F) conduit foundations labelled 'legally excepted, unsupported in release 1' until enabled) and SHALL reject grants "
           "to natural persons. Governmental units for exclusively public purposes MAY be enabled by sponsor policy. Expenditure-responsibility "
           "grants, disqualified supporting organizations and direct foreign grants SHALL be architecturally supported but disabled. Rejection "
           "text SHALL say 'not supported by this sponsor or release', never 'prohibited by law', except for natural persons.",
           ["Given a grant to an individual, when evaluated, then it is rejected with NaturalPersonProhibited.",
            "Given a grant to a private operating foundation in release 1, when evaluated, then the donor sees 'not supported by this sponsor' and the class is recorded as legally excepted."],
           ["RCD-014", "XID-ADV-001"], ["RCF-GRT-001", "UNB-XIN-NPG-003"],
           "GRT", "initial-implementation", "", G(), ["EV-A"]),
    clause("CNV-GRT-003", "Charity status freshness and deep review",
           "Status SHALL be checked at approval and at release against the latest published Tax Exempt Organization Search / Pub 78, EO BMF "
           "and auto-revocation releases, recording each source's posting date; retrieval within 24 hours of publication is an operational "
           "target, not a legal threshold. Grants SHALL be made to the listed legal name; group-exemption subordinates SHALL be flagged for "
           "additional verification. Deep review SHALL default to 180 days (sponsor may tighten) with refresh 30 days before expiry and "
           "event-triggered invalidation. Cross-sponsor evidence SHALL be reused only as input; any known adverse public fact SHALL be "
           "considered. Release-time checks MAY use cached verified evidence within the approved freshness window per source and SHALL fail "
           "safe when stale. Informational changes without eligibility, authority, payment or policy impact SHALL NOT block payment.",
           ["Given a recipient removed from Pub 78 after approval, when release runs, then payment is held with EligibilityChanged.",
            "Given cached status older than the source's approved window, when release runs, then a fresh check is required before payment.",
            "Given only a phone number change, when detected, then no payment hold is created."],
           ["RCD-006", "RR-REQ-X-007-r3#Q1", "SPD-GRT-002", "RR-UNB-EXT-028-r1#Q1", "RR-UNB-EXT-028-r1#Q2", "SPD-ENT-005"],
           ["RCF-REG-001", "RCF-GRT-001", "RCF-GRT-002", "RCF-GRT-003", "RCF-GRT-024", "RCF-GRT-038", "RCC-GRT-003", "RCC-GRT-005", "REQ-X-007", "REQ-X-018", "RR-UNB-EXT-028-r1"],
           "GRT", "initial-implementation", "", G(), ["EV-E"]),
    clause("CNV-GRT-004", "Shared public facts versus private investigations",
           "The common charity registry SHALL propagate only holds derived from verified public facts (IRS revocation, Pub 78 removal, OFAC "
           "listing, published judgments) re-verified by the platform from the primary source with source and timestamp. Sponsor-private "
           "investigation content, suspicions and decisions SHALL NOT propagate. A neutral 'enhanced review recommended' flag MAY be shared "
           "only under a written inter-sponsor information-sharing agreement limited to sponsors with pending or recent grants to the charity "
           "and containing no factual allegations. Sponsor-originated corrections SHALL be re-verified before updating the registry.",
           ["Given sponsor A opens a confidential investigation, when sponsor B views the charity, then no indication of A's investigation is visible.",
            "Given an IRS revocation, when published, then every sponsor's pending grants to that EIN are held with the public source cited."],
           ["RCD-029", "SPD-GRT-002"], ["RCF-GRT-025", "RCC-GRT-004"],
           "GRT", "initial-implementation", "Inter-sponsor information-sharing agreement only if the neutral flag is enabled.", G(), ["EV-E"]),
    clause("CNV-GRT-005", "Discretionary holds",
           "Discretionary holds SHALL be permitted only under a board-approved DiscretionaryGrantPolicy listing criteria (adjudicated findings, "
           "regulator actions, official pending matters such as indictment, attorney-general or IRS action or a government investigation the "
           "sponsor is informed of; media allegations only with elevated approval and a shorter maximum). Each hold SHALL record reason, "
           "evidence, owner, review deadline (maximum 90 days, renewable once on new evidence), escalation and automatic release; legally "
           "mandatory holds SHALL be distinguished and SHALL NOT auto-release.",
           ["Given a hold based on a media report approved by a single grant officer, when saved, then it is rejected for insufficient authority.",
            "Given a discretionary hold reaching 90 days without new evidence, when the deadline passes, then it is released and the donor notified."],
           ["XID-NPG-004", "UNB-XIN-NPG-006#Q1", "UNB-XIN-CLM-006#Q1"], ["UNB-XIN-NPG-006"],
           "GRT", "sponsor-activation", "Sponsor board resolution approving DiscretionaryGrantPolicy.", G(), ["EV-A"]),
    clause("CNV-GRT-006", "Sanctions screening, dispositions and OFAC reporting",
           "Donors, advisors, successors, grantees and payors SHALL be screened at onboarding, delta-screened automatically on list updates and "
           "screened before each grant release against the current list version. A confirmed match SHALL resolve to Block (a blocked person "
           "has an interest in property in the sponsor's possession or control), Reject (prohibited transaction, nothing blockable) or Proceed "
           "(licence or exemption), never automatically to Blocked. Block or reject actions SHALL produce 31 CFR 501.603/501.604 report data due "
           "within 10 business days and blocked property SHALL enter the annual report of property held on 30 June. A true recipient match "
           "before approval SHALL refuse the recommendation, open a compliance task due in 10 business days with report data prepared "
           "(default: file a precautionary report) and release the reservation only if donor, advisors and successors screen clear. Dispatch "
           "SHALL be fenced atomically at the irreversible step so a match found during release prevents transmission. Human false-positive "
           "dispositions SHALL be reusable within the deciding sponsor until a subject attribute changes, the list entry changes, a new entry "
           "matches, 12 months elapse or the officer's authority is revoked for cause; cross-sponsor reuse SHALL NOT apply to donors or "
           "advisors and SHALL apply to recipients only where the relying sponsor has adopted a written reliance policy. Donor-facing status "
           "SHALL use the neutral catalog and never show list names or scores.",
           ["Given a potential match found after the payment instruction was staged, when dispatch runs, then nothing is transmitted and the reservation remains.",
            "Given a disposition reused after the SDN entry's hash changed, when screening runs, then the reuse is refused and a new review opens.",
            "Given a held grant, when the donor views it, then the page shows 'under review' and no list or score text (Playwright assertion)."],
           ["RCD-015", "RCD-044", "SPD-GRT-003", "SPD-ADV-002", "RR-UNB-EXT-004-r2#Q1", "RCD-023"],
           ["RCF-GRT-013", "RCF-GRT-014", "RCF-REG-006", "UNB-RCN-WF-024", "RR-UNB-EXT-004-r2", "UNB-EXT-004", "UNB-RCN-GRT-007"],
           "GRT", "initial-implementation", "Sponsor compliance officer designation and approval of the ScreeningScope matrix and reuse policy.",
           OWN("Sanctions", "Unbound program ledger", "Sponsor compliance officer", "Sponsor", "Sponsor compliance", "Sponsor compliance officer"), ["EV-F"]),
    clause("CNV-GRT-007", "OFAC general licences and FCPA diligence for foreign-related grants",
           "Reliance on OFAC general licences SHALL be versioned as {program, CFR section or GL number, Federal Register citation, eCFR as-of date, "
           "conditions} and re-verified unchanged at release; only targeted-program NGO licences of the December 2022 type SHALL be accepted by "
           "default, with comprehensive-embargo programs held for program-specific approval or a specific licence. Foreign-related grants "
           "(when enabled) SHALL apply risk-based FCPA diligence: grantee declaration on government ownership and official connections, "
           "sanctions and PEP screening, staged payments and use reporting for larger or higher-risk grants, and escalation on any affirmative answer.",
           ["Given a GL reliance whose eCFR text changed since approval, when release runs, then payment is held for re-approval."],
           ["UNB-XIN-NPG-009#Q1", "UNB-XIN-NPG-010#Q1"], ["UNB-XIN-NPG-009", "UNB-XIN-NPG-010"],
           "GRT", "deferred", "OFAC specific licence for any activity outside a general licence; sponsor approval of the GL catalog.", G(), ["EV-F"]),
    clause("CNV-GRT-008", "Intermediaries, equivalency determinations and fiscal sponsors",
           "Outbound wording to an intermediary MAY name its program, country or cause and SHALL NOT name a specific foreign organization, even "
           "as a non-binding suggestion; the donor's original recommendation SHALL be retained only as internal evidence. The automatic "
           "(non-expenditure-responsibility) foreign path SHALL require current written advice received from a qualified tax practitioner "
           "(directly, or through a repository whose practitioner issues the advice or a reliance letter to the sponsor), no contrary "
           "knowledge and currency per §53.4945-5(a)(5); other advice SHALL go to sponsor review under the general good-faith standard. No "
           "agreement copy SHALL be required for a §170(b)(1)(A) fiscal sponsor that retains discretion and control, evidenced by a published "
           "project list or signed attestation; projects that are individuals, for-profits or foreign entities SHALL require agreement review. "
           "The proposed §4966 regulations SHALL be tracked as proposed (not effective) on the rule watch list.",
           ["Given an intermediary grant whose purpose text names a foreign charity, when submitted, then it is rejected with EarmarkWordingNotPermitted.",
            "Given ED advice forwarded by another grantor, when evaluated, then the grant routes to sponsor review."],
           ["XID-NPG-001", "XID-NPG-002", "UNB-XIN-NPG-001#Q1", "UNB-XIN-NPG-001#Q2", "UNB-XIN-NPG-002#Q1", "UNB-XIN-NPG-003#Q1"],
           ["UNB-XIN-NPG-001", "UNB-XIN-NPG-002", "UNB-XIN-NPG-003"],
           "GRT", "deferred", "Repository contract under which a qualified tax practitioner issues advice or a reliance letter to the sponsor.", G(), ["EV-A"]),
    clause("CNV-GRT-009", "Prohibited benefits, related parties and compensation from funds",
           "No compensation, referral payment or other payment SHALL be made from a fund to any donor, donor-advisor (including an advisor the "
           "donor designated with investment privileges), family member or 35% controlled entity of that fund. Grants SHALL be blocked to an "
           "advising charity from a fund that is a DAF; a fund whose sole beneficiary is the contributing charity SHALL be classified as a "
           "designated or agency fund, and reliance on the proposed definition for funds contributed and advised only by public charities SHALL "
           "be a recorded sponsor policy choice. Broad charitable classes defined by family SHALL be rejected and class grants SHALL remain "
           "disabled in release 1. Sponsor-engaged investment managers MAY be paid only under sponsor contracts with uniform program-wide fees "
           "and not designated by the donor for that fund.",
           ["Given a payment from a fund to its donor-advisor's investment firm designated by the donor, when proposed, then it is blocked with AutomaticExcessBenefit.",
            "Given an organization-advised fund that is a DAF, when it recommends a grant to the advising charity, then the grant is blocked."],
           ["RCD-021", "XID-CRM-004", "UNB-XIN-CRM-002#Q1", "UNB-XIN-NPG-007#Q1", "XID-CRM-002", "UNB-XIN-CRM-006#Q1", "UNB-XIN-ERP-003#Q1"],
           ["UNB-XIN-CRM-002", "UNB-XIN-CRM-006", "UNB-XIN-NPG-007", "REQ-X-076"],
           "GRT", "initial-implementation", "Sponsor board-approved account-type classification policy; executed investment-management agreements.", G(), ["EV-A", "EV-D"]),
    clause("CNV-GRT-010", "Pledges, payee substitution and taxable-distribution remediation",
           "Pledge handling SHALL be an explicit sponsor policy choice from {Prohibited, NonBindingOnly, NoticeConditions (IRS Notice 2017-73 §7)} "
           "with NonBindingOnly as the onboarding suggestion. Payee substitution to a merger successor SHALL require a verified legal successor "
           "passing eligibility on its own EIN in the same recipient class, sponsor approval and donor notice; recurring series SHALL carry a "
           "notice-and-opt-out window before the next disbursement and SHALL be reconfirmed when purpose, class or eligibility changes; one "
           "succession record MAY drive bulk notices. A taxable distribution or prohibited benefit SHALL open a remediation record (recovery to "
           "the sponsor, reasonable-cause evidence, Form 4720 inputs per schedule, optional Form 843 abatement request under §4962, which is "
           "discretionary).",
           ["Given a merged recipient with 1,200 recurring series, when the succession is verified, then each donor receives notice and series continue only after the opt-out window passes without objection.",
            "Given a remediation record, when the tax year closes, then Form 4720 inputs list the sponsor and fund-manager amounts by schedule."],
           ["RCD-016", "XID-NPG-005", "UNB-XIN-NPG-005#Q1", "XID-NPG-007", "UNB-XIN-NPG-008#Q1"],
           ["UNB-XIN-NPG-005", "UNB-XIN-NPG-008", "RCF-GRT-028"],
           "GRT", "initial-implementation", "", G(), ["EV-A"]),
    clause("CNV-GRT-011", "Tier-2 review triggers and decision quality",
           "Non-expenditure-responsibility grants SHALL route to enhanced (Tier 2) review when any of these is present: donor, advisor or family "
           "affiliation or benefit signals; Type I or II supporting organization; fiscally sponsored separate-entity project; intermediary with a "
           "stated program or country preference; equivalency-determination-based foreign recipient; governmental unit; grant large relative to "
           "recipient revenue; unusual restrictions; ambiguous or conflicting eligibility evidence. Thresholds SHALL be by risk and decision type, "
           "not amount alone; sponsor-specific amount thresholds MAY be configured. Decision quality SHALL be checked by a common risk-stratified, "
           "error-bound sampling policy with qualified independent reviewers, increasing after material errors or policy changes.",
           ["Given a grant to a Type I supporting organization, when evaluated, then it routes to Tier 2 regardless of amount."],
           ["RR-UNB-GRANT-024-r1#Q1", "RR-UNB-GRANT-030-r1#Q1", "RR-UNB-GRANT-030-r1#Q2", "XID-CLM-004", "RR-UNB-GRT-003-r1#Q2", "RR-UNB-CHAR-013-r1#Q1"],
           ["RR-UNB-GRANT-024-r1", "RR-UNB-GRANT-030-r1", "RR-UNB-CHAR-013-r1"],
           "GRT", "initial-implementation", "", G(), ["EV-A"]),
    clause("CNV-GRT-012", "Decline reasons and reconsideration",
           "Declines SHALL use the sponsor-configurable neutral catalog: donor-facing text describes the policy outcome, never facts about a "
           "charity; sanctions, investigation and law-enforcement reasons map to COMPLIANCE_REVIEW; charity-facing communication is limited to "
           "status codes; free text is optional and drawn from approved phrases. Reconsideration SHALL default to a 60-day filing window and a "
           "30-day response target, configurable with mandatory exceptions, decided by a reviewer independent of the original decision (contracted "
           "or board-appointed for single-reviewer sponsors); the original and reconsideration records SHALL both survive with outcomes {Upheld, "
           "Reversed, Modified, Withdrawn, Referred}. Charity corrections of shared facts and appeals of sponsor decisions SHALL be distinct paths.",
           ["Given a decline for an OFAC match, when shown to the donor, then the text is the COMPLIANCE_REVIEW phrase.",
            "Given a single-reviewer sponsor, when reconsideration is filed, then it is assigned to an appointed independent reviewer, not the original decider."],
           ["XID-CLM-003", "XID-CLM-001", "SPD-ENT-002", "UNB-XIN-CLM-001#Q1", "UNB-XIN-CLM-004#Q1", "XID-CLM-005"],
           ["UNB-XIN-CLM-001", "UNB-XIN-CLM-002", "UNB-XIN-CLM-004"],
           "GRT", "initial-implementation", "", G(), ["EV-E"]),
    clause("CNV-GRT-013", "Complex-asset intermediaries and fund-borne charges",
           "Complex-asset gifts expected to credit a DAF SHALL use the agent model: the sponsor is donee of record (acknowledgment, Form 8283 "
           "Part V, Form 8282, §4943 holding) and the intermediary is the sponsor's contracted agent. The donee model SHALL be activated only "
           "where the intermediary agreement gives it discretion over proceeds and the donor is told the deduction is for a gift to the "
           "intermediary. Service charges to funds (including outsourced vendor fees netted from proceeds) SHALL be listed in the disclosed "
           "schedule, reasonable with cost-based evidence and never paid to the donor, advisor, family or 35% entities; a recovery ratio above "
           "1.10 SHALL trigger review, never service denial; fund-borne charges SHALL be reported as bearer shifts, not savings.",
           ["Given an intermediary agreement without a title clause, when a complex gift is configured, then activation is refused.",
            "Given a charge schedule with recovery ratio 1.25, when the annual review runs, then a CFO review task is opened and service continues."],
           ["RCD-034", "XID-ECO-002", "SPD-ECN-005", "UNB-XIN-ECO-018#Q1", "UNB-SMP-ECN-001#Q1"],
           ["UNB-XIN-ECO-018", "UNB-SMP-ECN-001"],
           "GRT", "sponsor-activation", "Executed intermediary agreement; sponsor-approved fee schedule.", G(), ["EV-A"]),
    clause("CNV-GRT-014", "Tax-fit guidance and statutory disclosures",
           "The platform SHALL NOT provide individualized tax advice. Donor-facing tax content SHALL be generated from verified authority with "
           "citations and effective dates: DAF contributions do not qualify for the §170(p) non-itemizer deduction; the 0.5% itemized floor "
           "from 2026; acknowledgments state the sponsor's exclusive legal control and whether goods or services were provided; grants from the "
           "fund are not deductible by the donor. Schedule F manner-of-disbursement values SHALL use the IRS wording versioned by form revision.",
           ["Given the donor tax summary, when rendered, then every statement cites its authority and effective year."],
           ["RCD-030", "UNB-XIN-NPG-004#Q1"], ["UNB-XIN-NPG-004", "RCF-REG-009"],
           "GRT", "initial-implementation", "", G(), ["EV-A"]),
    clause("CNV-GRT-015", "State charity standing (California first)",
           "The 50-state registration build SHALL be deferred, except that California Attorney General Registry status (and FTB status) SHALL "
           "be ingested as a recipient attribute and shown, with sponsor policy choosing hold or warn.",
           ["Given a recipient delinquent in the California Registry, when a grant is evaluated, then the sponsor's hold-or-warn policy applies."],
           ["SPD-GRT-005"], [],
           "LEG", "initial-implementation", "Empirical: confirm data access to the California Registry.", G(), ["EV-A"]),
)
