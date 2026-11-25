/// Gap register used by the prototypes (port of tools/lib/gaps.mjs). FORMA-GAP-01..05 and UNB-GAP-W-01 come from
/// reconciliation/2026-11-23/data/ui-component-gaps.csv; FORMA-GAP-06..17 are new, found by these prototypes
/// (see screens/FORMA-FIT-REPORT.md for need, evidence and proposed acceptance).
module Unbound.Designs.V1.Gaps

let gaps : (string * string) list =
    [
      "FORMA-GAP-01", """Existing. Button variants (primary, destructive) and async button. 1.0.2 now styles :disabled (dashed, 0.62 opacity) and [aria-busy=true] (spinner glyph) for application buttons; data-forma-variant=primary exists only inside .forma-site and no destructive variant exists. Prototypes use native buttons; the action label carries the consequence."""
      "FORMA-GAP-02", """Existing. Field invalid/disabled/read-only styling. The 1.0.2 build ships [aria-invalid=true]/:user-invalid, :disabled and :read-only rules in foundations (catalog/known-issues.json still lists it); prototypes use aria-invalid + validation-message. Appears closed pending the 3:1 contrast browser proof in the gap's acceptance."""
      "FORMA-GAP-05", """Existing. Charts deferred; S-011 uses tables and metric cards only."""
      "UNB-GAP-W-01", """Existing. Widget host, frame protocol and scoped tokens (Unbound-owned)."""
      "FORMA-GAP-06", """NEW. No application page frame: .forma-app-shell is planned (P0). Page gutters exist only in .forma-site__main and the legal-sponsor footer only as the marketing site-footer, so every prototype composes the marketing shell around navigation-shell."""
      "FORMA-GAP-07", """NEW. Identifier has no copy affordance (G-016 requires one); prototypes place a plain Copy button beside .forma-identifier with no defined layout, feedback or announcement. Forma lists 'copyable field' as planned P1."""
      "FORMA-GAP-08", """NEW. Money presentation (G-017): metric-card, data-grid and key-value-list do not use tabular numerals; data-grid has no numeric/end-aligned column; metric-card values break mid-amount in a 3-column dashboard-grid inside navigation-shell at 1280 px: plain '$100,000.0|0' (overflow-wrap:break-word) and '$100,00|0.00' with .forma-identifier (overflow-wrap:anywhere), see rendered/gap-evidence/FORMA-GAP-08-*.png; no convention for labelled negative amounts."""
      "FORMA-GAP-09", """NEW. No multi-dimension status track (G-002, S-005 lifecycle): steps is static site content with no per-item state; wizard__steps only knows complete/current. Prototypes compose steps + status-lozenge + time per item, with no headline slot."""
      "FORMA-GAP-11", """NEW. Unavailable action with reason (segregation of duties S-009/S-010, required notices S-013): no pattern ties a disabled command to a visible reason; the reason is ad-hoc text referenced by aria-describedby. Disabled switch dims its own description (the reason) to 0.62 opacity; obligation-panel has no 'not available to you' state."""
      "FORMA-GAP-12", """NEW (low). No read-only allocation summary or text-equivalent pattern for a percentage split (successor plan); allocation is input-only. Prototype uses a plain ordered list."""
      "FORMA-GAP-13", """NEW (catalog defect). Canonical feed-item pattern uses data-state="warning", which is not a status-lozenge value (renders the neutral bullet). Prototypes use the documented ok|attention|unknown|blocked values."""
      "FORMA-GAP-14", """NEW. Dialog consequence preview on phones: no pinned header or action row, so a G-005 dialog with 7 consequence rows at 320x800 scrolls the title and amount out of view when focus lands on the actions; key-value values indent under labels, lengthening it."""
      "FORMA-GAP-15", """NEW. Container adaptation: collection-toolbar, obligation-panel, workspace-shell and navigation-shell reflow only by viewport media queries. Placed in a pane (workspace-shell primary, split-pane secondary) at 1280 px the toolbar sets labels vertically and obligation text breaks one word per line."""
      "FORMA-GAP-16", """NEW. AI output labelling (G-011): conversation and handoff-summary have no slot for 'AI-generated, not a decision', sources or freshness; the canonical handoff-summary includes Approve/Reject buttons, which G-011 forbids in AI surfaces. Prototype adds a caution callout and omits the buttons."""
      "FORMA-GAP-17", """NEW. Flyout body is a scroll container with no focusable element when content is text-only, failing axe scrollable-region-focusable (serious) at 320 and 1280 px. Fixed in markup with role=region tabindex=0 aria-label on .forma-flyout__body; the canonical pattern should carry it."""
    ]

/// GAPS[id] ?? "undocumented".
let describe (id: string) = gaps |> List.tryFind (fun (k, _) -> k = id) |> Option.map snd |> Option.defaultValue "undocumented"
