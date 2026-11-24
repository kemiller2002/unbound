"""Clause amendments from evidence-backlog research completed after the mission report (report 13)."""

AMENDMENTS = {
    "CNV-LEG-002": {"finding": "EVB-STATE (report 13): Hawaii HRS 467B-2.3 verified from statute text; fundraising-counsel exposure",
        "replace": [("Unbound SHALL register with the California Attorney General's Registry of Charities and Fundraisers as a charitable fundraising platform before activating",
                     "Unbound SHALL register as a charitable fundraising platform with the California Attorney General's Registry of Charities and Fundraisers (Gov. Code §12599.9) and with the Hawaii Department of the Attorney General (HRS §467B-2.3, in force 1 July 2026, annual renewal by 1 July) before activating")],
        "append": "The jurisdiction table (data/evidence-backlog/evb-state-registration.jsonl) SHALL be re-verified annually and on each new deployment model. Unbound SHALL NOT author solicitation appeals or provide fundraising advice to sponsors unless the fundraising-counsel registration requirements of each affected jurisdiction (about 25 have them) have first been determined from statute text and any registrations made; sponsor-authored content in sponsor-branded deployments is not affected.",
        "acceptance_add": ["Given no recorded Hawaii registration, when a white-label sponsor enables Hawaii donors, then activation is refused with PlatformRegistrationRequired.",
                           "Given a feature that generates fundraising appeal text authored by Unbound, when activation is requested without a recorded fundraising-counsel determination, then it is refused."]},
    "CNV-PAY-001": {"finding": "EVB-RAILS (report 13): partner rails reach an estimated 15-25% of charities but 55-75% of grant dollars",
        "append": "Every program SHALL keep a check fallback and a stored verified-destination record so that grants to charities outside a partner network are paid without a partner enrolment."},
}
