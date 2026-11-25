/// The constants of merge_specs.py: placeholder gap names mapped to registered gaps, the renumbered screen and the
/// component gaps the panel and view specs raise. Transcribed in the same order and wording.
module Unbound.Authoring.MergeSpecsData

/// merge_specs.py: GAPMAP
let gapMap : (string * string) list =
    [ ("PROPOSED:Approval quorum tracker", "FORMA-GAP-24")
      ("PROPOSED:Two-person approval capture", "FORMA-GAP-24")
      ("PROPOSED:Accounting period context bar", "FORMA-GAP-25")
      ("PROPOSED:Per-item batch outcome summary", "FORMA-GAP-26")
      ("PROPOSED:Typed panel slot registry", "UNB-GAP-W-02") ]

/// merge_specs.py: RENUM
let renum : (string * string) list =
    [ ("UNB-UI-S-099", "UNB-UI-S-061") ]

/// merge_specs.py: NEWGAPS
let newGaps : (string * string * string * string * string * string * string) list =
    [ ("FORMA-GAP-24", "Approval quorum tracker (two or more distinct approvers)", "Forma", "P1", "Restatement, tolerance override, disposition batches, cutover and two-person approvals need a tracker of required approvers, each with status and independence rule (not the preparer). readiness-checklist only approximates it.", "readiness-checklist with text per approver", "A component listing required approvals with approver, status, time and an explicit 'cannot be the preparer' rule, announced on change")
      ("FORMA-GAP-25", "Accounting period context bar", "Forma", "P2", "Finance workspaces need a persistent header with period, period state (open, closing, certified), cutoff and snapshot as-of/known-at times; scope-trail and date-range do not carry period state.", "scope-trail plus freshness", "A context bar component with period state and two timestamps, copyable, with forced-colours support")
      ("FORMA-GAP-26", "Per-item batch outcome summary", "Forma", "P2", "Batch approval commits each item in its own transaction, so results are mixed (approved, in review, waiting for funds, unknown); FORMA-GAP-04 assumed a single outcome.", "Table of items with status chips", "A summary that groups items by outcome with counts and links, and treats Unknown distinctly (G-006)")
      ("UNB-GAP-W-02", "Typed panel slot registry for the case workspace", "Unbound", "", "The case frame (S-014) hosts 18 typed decision panels; a registry maps case type to panel, required facts and permitted actions.", "Static mapping in the prototype", "Registry data validated against WFM transitions; unknown case types render a safe read-only panel") ]
