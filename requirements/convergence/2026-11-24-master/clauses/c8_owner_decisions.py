"""Clause amendments applying owner decisions OD-M-01..05 (data/owner-decisions.jsonl). Same format as c7_amendments.

OD-M-06 (ratification of agent-recommended resolutions) is NOT recorded here: the mission recommended a domain-by-domain
owner review, so review packets are prepared in report 11 and approvals are recorded only after that review.
"""

AMENDMENTS = {
    "CNV-GOV-004": {"finding": "OD-M-01 (owner decision)",
        "replace": [("whether to replace the absolute rule with a recorded conflict rule (cross-appointments disclosed in each board resolution, no relationship to the matter, never deciding the same matter for two sponsors) is owner decision OD-M-01 (report 10).",
                     "the owner decided (OD-M-01) to keep the absolute rule for release 1 and to revisit it only with evidence from an administrator deployment.")]},
    "CNV-FIN-012": {"finding": "OD-M-02 (owner decision)",
        "replace": [("this narrowing is owner decision OD-M-02 pending ratification (report 10).", "the owner approved this narrowing (OD-M-02).")]},
    "CNV-PAY-001": {"finding": "OD-M-03 (owner decision)",
        "replace": [("The release-1 default (networked partner rail or sponsor-originated verified ACH) is owner decision OD-M-03; until decided, a program SHALL declare its rail explicitly.",
                     "The release-1 default (OD-M-03) SHALL be a networked payment-partner rail where the sponsor has an executed partner agreement, and otherwise sponsor-originated ACH to a verified destination.")],
        "acceptance_add": ["Given a sponsor without an executed partner agreement, when a program is created, then its rail defaults to sponsor-originated ACH to a verified destination."]},
    "CNV-ECO-004": {"finding": "OD-M-04, OD-M-05, OD-M-08 (owner decisions)",
        "append": "The owner approved this segment choice (OD-M-05). The decision on Unbound- or partner-supplied staffed services (SPR-ADV-009) SHALL be reopened if fewer than five sponsors with under 10,000 funds have signed within 12 months of general availability (OD-M-04; the threshold of five is a business-policy default the owner confirms at the annual review). Superseded by OD-M-08: Unbound SHALL plan two offers on one small core against the same incumbent — software licensed to sponsors and administrators (iPhi-class) and an Unbound-operated administration team (Ren-class outsourced administration); the staffed offer SHALL keep SPD-ADV-001 (Unbound staff prepare and process; each sponsor's own appointed people make reserved decisions); capabilities beyond incumbent parity SHALL be limited to those that create the control, evidence and cost advantages in report 05 section 5.",
        "acceptance_add": ["Given 12 months after general availability with four signed sponsors under 10,000 funds, when the review runs, then a staffed-services decision record is opened."]},
    "CNV-LEG-001": {"finding": "OD-M-07 (owner decision)",
        "replace": [("implementing sponsor-selected models executed by the sponsor's custodian and factual, non-evaluative reporting are permitted.",
                     "relaying donor allocation recommendations among options made available by the sponsor's external investment manager, and factual, non-evaluative reporting, are permitted."),
                    ("Pools SHALL rely on Investment Company Act §3(c)(10)(B) with §7(e) donor disclosure; release 1 SHALL be single-sponsor pools only (multi-sponsor pools are lawful but deferred);",
                     "No sponsor-maintained investment pools SHALL be offered (OD-M-07): each sponsor SHALL delegate investment management of fund assets to an external registered investment manager or custodian platform under a written agreement, and the sponsor board SHALL keep the duties to select the manager, set its scope and investment policy, and monitor performance and cost (prudent delegation under applicable state law, for example UPMIFA §5 where enacted; interpretation). Unbound SHALL ingest the manager's positions, transactions, units and valuations and reconcile them, and SHALL NOT select or recommend investments or managers. Sponsor-maintained pools (lawful under Investment Company Act §3(c)(10)(B) with §7(e) disclosure, including multi-sponsor pools) are not offered and would require a new owner decision;")],
        "acceptance_add": ["Given a sponsor configuration that defines a sponsor-maintained investment pool, when saved, then it is rejected with SponsorPoolsNotOffered.",
                           "Given a donor allocation recommendation naming an option the external manager does not offer, when submitted, then it is rejected and the available options are shown."]},
    "CNV-FIN-005": {"finding": "OD-M-07 (owner decision)",
        "append": "Because investment management is delegated to an external manager (OD-M-07), values for each fund SHALL come from the manager's or custodian's positions and prices (or unit values the manager publishes) and SHALL be reconciled to the custodian; the deterministic-publication delegation in this clause applies only to fund-level valuations Unbound derives from those inputs, and a missing or stale manager price SHALL block the affected statement rather than be estimated.",
        "acceptance_add": ["Given a manager price feed missing for one holding, when statements run, then statements for funds holding it are withheld with reason ManagerPriceMissing."]},
    "CNV-FIN-001": {"finding": "OD-M-07 (owner decision)",
        "replace": [("Each investment pool SHALL carry a sponsor-approved liquidity buffer", "Each fund's investments at the external manager SHALL be backed by a sponsor-approved liquidity buffer (a cash allocation at the custodian)")],
        "append": "The buffer's stress model SHALL include the manager's settlement times for liquidating investments to fund grants."},
    "CNV-LEG-002": {"finding": "OD-P-LEG, OD-M-08 (owner decisions)",
        "append": "Release 1 SHALL ship sponsor-branded deployments only, in which charity listings are visible to advisory-privileged users on the sponsor's own branded portal; white-label consumer marketplaces, embedded checkout, Unbound-branded donor apps and charity-side donation buttons SHALL be disabled until the registrations above are recorded (OD-P-LEG).",
        "acceptance_add": ["Given a release-1 deployment, when embedded checkout is enabled, then activation is refused unless California and Hawaii registrations are recorded."]},
}

