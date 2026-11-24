# Unbound v1 screen prototypes: design critique

Scope: the product UI inside each design state of S-001..S-013 and W-001. Measurements come from `checks-v1.json` (harness `audit-v1.mjs`). Screenshots are in `shots/` (full page) and `shots/states/` (one per state, 320 and 1280). Defect ids (UXD-nnn) refer to `ui-defects-v1.csv`. Anything not measured is marked *(judgement)*.

Overall: the prototypes are semantically careful (axe-core: 0 WCAG violations on all 14 pages at all 5 viewports; every focus stop has a visible indicator; no page-level horizontal scroll at 320 or 400%). The problems are mostly about **priority and density**. Every fact is shown at the same weight, often twice. Status steps take whole screens. No action is visually primary. Money and identifiers break mid-value. The prototype harness (104 px titles, "Demonstrates" lines) makes all of this look worse than it is, but the product UI has the same problems once the harness is removed.

## S-001 Giving home
- **Keep:** the three-figure model (Available to grant / Market value / Pending) and the "How this is calculated" disclosure. They do the G-004 job well.
- **Reprioritise:** at 320 the balance starts about 981 px down, after a 710 px attention queue (UXD-008). Put the fund summary first, with Available to grant as the only hero number. Show Pending as one line with a disclosure, and attention items as one-line rows ("2 things need you").
- **Demote** Market value. It is currently the same 56 px figure as Available to grant (UXD-009). Fix the mid-amount breaks ("$100,000.|00"), which happen at 768 and 1280 (UXD-001).
- **Remove** the "Created by sponsor rules" kicker, the "Ledger confirmed" lozenge (state it once in the disclosure) and the duplicate "Not current" lozenge in s001-4.
- **Fix the action bar:** at 320 it is 138 px tall, sticky, and hides focused controls (UXD-003). On desktop it ends up at the bottom of the page as the only place the two main actions appear. Put actions in the page header on desktop. On phones, show one compact row with "Recommend a grant" as primary.
- **Loading:** the skeleton should match the final layout: card row, list rows (UXD-013). **Empty:** keep one call to action per empty block, hide the bar, and replace the "□" glyph (UXD-041).
- s001-5 (suspended) has the right structure. Soften the "verified death" wording with sponsor-approved copy *(judgement, UXD-060)*.

## S-002 Charity search
- **Keep:** the EIN and city in every result, the non-selectable "On hold" row, the fault with a support reference, and the "Can't find it?" path.
- **Merge:** the search term should not also appear as an "Active filter" chip with its own Remove control and a "1 active" count. That gives three clear controls for one query (UXD-026). Drop pagination for a single page (UXD-045).
- **Reprioritise the result row:** name, then "City, ST · EIN" on one line, then status and Select. Show "IRS data as of" once in the results header, not on every row (UXD-027). At 320 the labelled-card recomposition fits about 2.5 results per screen.
- **Fix:** "Springfiel|d", the "00-|0000101" EIN breaks (UXD-005, UXD-006) and the near-invisible current page "1" at 1.63:1 (UXD-002).
- s002-5 form: inputs in one row sit at different heights (UXD-022), and "Clear form" next to "Send request" is an unnecessary destructive peer.

## S-003 Grant recommendation
- **Keep:** the error summary linked to the field with the value kept, the verbatim affirmations, the clear pledge-blocking explanation, and the unknown-outcome panel that warns against resubmitting.
- **Remove:** the triple progress indicator (kicker, blue progress bar, stepper; UXD-043). Remove step 1 as a page, since the charity is already chosen. Remove the "Available to grant" block repeated on every step; a single line in the step header is enough.
- **Progressive disclosure on step 2:** show percentage fields only for a percentage grant, and schedule fields only when "recurring" is on. Today the step shows 5 inactive fields and is 1,600 px tall (UXD-023).
- **Merge the confirmations:** the review step and the G-005 modal repeat the same data. Put "what changes / reversible / who acts next / when" on the review step and make its button "Submit grant recommendation" (UXD-024, *judgement* on how G-005 is met).
- **One primary per step:** Back, Save draft and Continue currently look identical (UXD-010). Do not disable Continue silently (UXD-025).
- s003-7: the five-dimension tracker is about 870 px of mostly "Not started" (UXD-019). Show a compact tracker and lead with the new Available to grant. s003-8: replace "Request reference IDK-…" with "Support reference" (UXD-014).

## S-004 Grant detail
- **Critical:** the scope trail says "Grant 2026-004817" in all six states, even when the record is a different grant (UXD-004).
- **Header:** make the title charity + amount and show the reference once with Copy. "Grant 2026-004817" currently appears in the eyebrow, title, Reference row and scope trail. Style "Request cancellation…" as destructive. Drop the "More" menu, whose only item duplicates Documents (UXD-021).
- **Merge** the five-dimension block and the timeline, which repeat the same four events in opposite orders. Keep a compact tracker at the top and collapse the history (UXD-018).
- **Reprioritise** s004-3 and s004-6: the explanation ("will not be sent twice", "cannot be paid") must sit directly under the header, not after 1,500 px of steps (UXD-020).
- **Remove** donor-facing ledger language: "general ledger entry JE-…", "investment operations", "Balanced Pool", "UTC 15:30" (UXD-014). Remove the second breadcrumb (UXD-015).

## S-005 Add money
- **Keep:** the copyable DTC instructions, the lot-by-lot receipt table, and the plain explanation for the returned ACH.
- **Collapse** the five method cards (about 400 px) to the chosen method with "Change" once a method is selected (UXD-056).
- **Rename** "Declare contribution" to "Send" or "Tell us it's coming" *(judgement)* (UXD-014). Make it the single primary action; "Link a bank account…" is secondary.
- Align the form grids (UXD-022), add a $ adornment (UXD-042), and give Copy a "Copied" confirmation that is announced (UXD-037).
- s005-4: the six-step status column is about 1,030 px (UXD-019). Right-align the shares column (UXD-007). The "Contributions" breadcrumb points to a page that does not exist (UXD-015).

## S-006 Participants and successors
- **Keep:** the role, scope, dates and status columns, the named revoke dialog, and the text equivalent of the successor plan.
- **Remove** the Scope column when every row is the same fund. It wraps "F-2026-000418" over three lines (UXD-029). Move per-row actions into an overflow menu.
- **Successor allocation:** put label and input adjacent with a % suffix (Forma `allocation`). State the error once in the summary and once inline, not three times (UXD-042, UXD-044).
- Design the Viewer participant's own read-only experience, which is missing (UXD-039).

## S-007 Document center
- **Keep:** the "Superseded by" link and the Open / PDF pair from one snapshot.
- **Remove** the "Verified" lozenge from every row of the owner's own documents. Show status only when a document is superseded or failed verification *(judgement)* (UXD-049). Drop single-page pagination.
- **Move** "Verify a document" out of the donor's fund scope to a public sponsor page (UXD-030).
- Stop references and "Download PDF" wrapping mid-token (UXD-005, UXD-006).

## S-008 Exception inbox
- **Keep:** sorting by SLA risk, the rule, subject, age, owner and next-action columns, and the auto-close explanation.
- **Make the summary honest:** "19 open items" does not match the filtered "5 open items". State the scope or follow the filter. Drop the redundant "Breaching SLA" lozenge on the card (UXD-031).
- **Clarify selection:** the "Selected: SAN-02" panel sits under the pagination with no selected row. Use master-detail or remove it. Rewrite the age as "Overdue 1 d 2 h" (UXD-032). Fix "Unassig|ned" (UXD-006) and the 2.45:1 current-page contrast (UXD-002).
- s008-3 heading skips from h2 to h4 (UXD-050).

## S-009 Case workspace
- **Keep:** the rule panel, the side-by-side conflicting sources, segregation of duties with a reason, the AI flyout labelling, and the decision record.
- **One decision entry point:** "Record decision…" in the header and "Decide" in the actions panel duplicate each other. Move the actions panel to the top of the right rail and use buttons, not links. Drop the A1/A4/A2 chips (UXD-033, UXD-014).
- **Restructure the facts** as a raises vs weakens comparison. The 11 px uppercase mono labels and bold-label paragraphs read as a log (UXD-052).
- The AI flyout title is 66 px and the "AI-generated" label appears twice (UXD-011, UXD-053).

## S-010 Policy configuration
- **Keep:** the live validation, the impact preview with counts, and the separate approver.
- **Diff:** use one table (field / active / draft / changed) with a text "Changed" marker. Today the changed fields are shown only by a blue bar, and the two columns are misaligned (UXD-034).
- **Remove** the "AND" chip when there is one condition, the "Meaning" preview of an invalid value, and the "assistance contract" jargon in the error (UXD-054, UXD-014). State the error once (UXD-044).
- Make "Approve and schedule…" primary and "Reject…" destructive (UXD-010).

## S-011 Sponsor overview
- **Keep:** no composite score, "Not determinable" instead of a false total, and freshness per source.
- **Fix:** "Not determina|ble" breaks at 56 px; use a smaller value style for non-numeric states (UXD-006). Right-align all 19 numeric cells with tabular numerals (UXD-007).
- **Remove** "Charts deferred (FORMA-GAP-05)" from product copy (UXD-014).
- **Reprioritise** *(judgement)*: put the stale reconciliation account, the only at-risk item, in a top alert that links to its row.

## S-012 Charity view
- **Keep:** the anonymous recognition wording and the remittance key-value list.
- **Fix** amounts breaking ("$2,500.|00") and left-aligned amount columns (UXD-001, UXD-007). Use consistent row verbs: "View remittance" / "View grant", not "View" (UXD-055).
- **Destination change form:** stack the fields, add format hints and account-number re-entry, and use Forma file upload (UXD-035).

## S-013 Notification preferences
- **Keep:** the per-channel switches and the reason why some notices cannot be turned off.
- **Replace** the disabled "locked on" switches. They look broken and cannot be reached by keyboard. Use text such as "Always on: Email, In-app — required for tax receipts" (UXD-036).
- **Reduce repetition:** "Required: tax acknowledgment" appears under every channel; say it once per class.

## W-001 Grant status widget
- **Keep:** the minimal headline status, amount, charity and as-of time, the legal sentence, and the expired-token message.
- **Show** the widget at real iframe widths (320 and 400 px) with the sponsor mark and an optional "View in Harbor Giving" link. Add loading and unavailable states (UXD-061, UXD-039).

## Cross-screen patterns that should become reusable Forma components

| Pattern (seen on) | Closest Forma catalog component | Gap |
|---|---|---|
| **Money value**: non-breaking, tabular, signed with a text label, adornment in inputs (S-001, S-003, S-004, S-005, S-012) | `identifier` (Identifier & critical value), `metric-card` | None exists for money. Needs nowrap, tabular-nums, end alignment and negative label (FORMA-GAP-08) |
| **Copyable identifier** with announced "Copied" (S-004, S-005, S-012, S-008) | `identifier` | Copy affordance is planned P1 (FORMA-GAP-07) |
| **Compact multi-dimension status tracker** (five grant dimensions, six contribution stages) (S-003, S-004, S-005) | `steps` (site), `progress-bar`, `timeline` | No dense application tracker. `steps` is a site component and renders about 170 px per step |
| **Record header** with one primary action, a destructive action and a reference shown once (S-004, S-005, S-009, S-010) | `record-header` | Needs button variants (FORMA-GAP-01) and guidance against repeating the reference |
| **Fund summary**: hero authoritative figure plus secondary informational and pending figures (S-001, S-003 step header) | `metric-card`, `dashboard-grid`, `priority-stack` | No hero/secondary metric hierarchy |
| **App shell** with account menu, fund switcher and compact scope trail on phones (all) | `navigation-shell`, `scope-trail`, `app-identity` | `.forma-app-shell` is planned P0 (FORMA-GAP-06). `scope-trail` needs a phone-collapsed form |
| **Numeric data-grid column** (end-aligned, tabular) (S-005, S-011, S-012) | `data-grid`, `dense-ledger` | No numeric column type (FORMA-GAP-08) |
| **Always-on preference row** (S-013) | `switch` | None. Needs a non-disabled "required" presentation |
| **Version diff with changed markers** (S-010) | `diff-viewer` | The prototype does not use it. Adopt `diff-viewer` with text change markers |
| **Allocation to 100%** (S-006) | `allocation` | The prototype does not use it. Adopt it |
| **Search results row for entity disambiguation** (S-002) | `data-grid` with `collection-toolbar` and `active-filter-summary` | No compact record-row recomposition; the current recomposition uses labelled cards |
| **Operation-status / alert with state-coloured accent and non-button icon** (S-001, S-003, S-004, S-005, S-006, S-007) | `operation-status`, `alert` | The icon box looks clickable and the accent does not follow data-state |
| **Sticky phone action bar that respects focus** (S-001) | `mobile-action-bar` | Needs scroll-padding, a single-row layout and a short-viewport opt-out (fails 2.4.11 today) |
| **Wizard review-and-submit** with G-005 consequences built in (S-003) | `wizard`, `key-value-list`, `dialog` | No review-step pattern that meets G-005 without a second modal |
