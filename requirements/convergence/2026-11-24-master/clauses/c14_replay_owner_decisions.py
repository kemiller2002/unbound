"""Amendments from owner decisions taken on the public-data replay (report 14): OD-M-10 and OD-M-11."""

AMENDMENTS = {
    "CNV-GRT-006": {"on": "2026-11-25", "finding": "OD-M-10 owner decision (report 14 §3.4)",
        "append": "The name-matching method SHALL meet a recall target approved by the sponsor compliance officer on a "
                  "versioned perturbation suite of listed names (exact, case and punctuation, dropped character, token "
                  "reorder, legal suffix, and listed names attached to otherwise clean identifiers) before release and "
                  "after every matcher or threshold change; no listed name attached to a clean identifier SHALL pass "
                  "straight through. The reference matcher in report 14 does not meet this and SHALL NOT be used in "
                  "production. False-alert rates SHALL be measured and reported by name subgroup, including names with "
                  "Arabic-origin or Islamic tokens, at every matcher or threshold change and at least quarterly. A "
                  "sanctions alert SHALL be a prompt for review, never evidence: a disposition SHALL record the facts a "
                  "person compared and SHALL NOT cite an alert or score as its reason.",
        "acceptance_add": [
            "Given a matcher change whose recall on the perturbation suite is below the approved target, when release is requested, then it is refused.",
            "Given a listed entity name with one character removed, attached to a clean public-charity EIN, when screened, then the grant does not pass straight through.",
            "Given a quarter of screening, when the compliance report is produced, then false-alert rates are shown by name subgroup.",
            "Given a disposition whose only stated reason is a sanctions score, when saved, then it is rejected."]},
    "CNV-ECO-003": {"on": "2026-11-25", "finding": "OD-M-11 owner decision (report 14 §4)",
        "replace": [
            ("No licence price list, customer cost-saving claim or segment commitment SHALL be approved until",
             "No customer cost-saving claim or segment commitment SHALL be approved until"),
            ("Price-list approval SHALL additionally require measured results showing C below both B and C0 at P50 for "
             "the sponsor size band being priced, from a replay covering that band, and SHALL be approved by the Unbound board.",
             "Licence prices SHALL be set by the Unbound board at what the market will bear, informed by the public-data "
             "replay (report 14) and observed incumbent prices (report 13); they SHALL NOT require sponsor or client "
             "validation, and a price list SHALL NOT be refused for lack of a real-sponsor replay or time study. Each "
             "price-list approval SHALL record the vendor P&L result at that price, and a price at which Unbound's fully "
             "loaded result is negative at P90 cost and adverse scale SHALL be labelled loss-making in the approval record."),
            ("Given a price list submitted before the shadow replay and time study are recorded, when approval is requested, then it is refused.",
             "Given a customer cost-saving claim submitted before the shadow replay and time study are recorded, when approval is requested, then it is refused."),
            ("Given recorded studies whose measured C is not below B at P50 for the band, when a price list is submitted, then it is refused.",
             "Given a board-approved price list and no real-sponsor replay, when approval is recorded, then it is accepted and the record shows the vendor P&L result at P90 cost and adverse scale."),
        ]},
}
