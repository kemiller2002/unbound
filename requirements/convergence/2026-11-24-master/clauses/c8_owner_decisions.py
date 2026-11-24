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
    "CNV-ECO-004": {"finding": "OD-M-04, OD-M-05 (owner decisions)",
        "append": "The owner approved this segment choice (OD-M-05). The decision on Unbound- or partner-supplied staffed services (SPR-ADV-009) SHALL be reopened if fewer than five sponsors with under 10,000 funds have signed within 12 months of general availability (OD-M-04; the threshold of five is a business-policy default the owner confirms at the annual review).",
        "acceptance_add": ["Given 12 months after general availability with four signed sponsors under 10,000 funds, when the review runs, then a staffed-services decision record is opened."]},
}
