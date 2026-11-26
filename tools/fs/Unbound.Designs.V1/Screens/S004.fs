/// S-004 Grant detail (port of tools/screens/S-004.mjs).
module Unbound.Designs.V1.Screens.S004

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let private scope =
    scopeTrail [ "Example Charitable Foundation"; "Harbor Giving donor-advised funds"; "Lindqvist Family Fund (F-2026-000418)"; "Grant 2026-004817" ]

let private crumbs =
    """<a href="S-001-giving-home.html">Giving home</a> <span aria-hidden="true">/</span> <a href="#grants">Grants</a>"""

/// The three settled dimensions followed by the given Funding and Payment dimensions.
let private dims (overrides: Dimension list) =
    [ Ui.dim("Recommendation", "ok", "Submitted", "2026-11-20T10:02:00-06:00", "20 Nov 2026, 10:02 AM CT")
      Ui.dim("Eligibility", "ok", "Eligible", "2026-11-20T10:05:00-06:00", "20 Nov 2026, 10:05 AM CT")
      Ui.dim("Approval", "ok", "Approved", "2026-11-24T09:30:00-06:00", "24 Nov 2026, 9:30 AM CT") ]
    @ overrides

let private charity (p: string) =
    let eligibility =
        loz "ok" "Eligible: public charity" + Ui.fresh(label = "IRS data as of", iso = "2026-11-23", shown = "23 Nov 2026", stateText = "Current at approval")
    let rows =
        kv [ "Charity", "Riverside Food Bank"
             "EIN", ident "00-0000103"
             "Address", "410 River Road, Springfield, IL"
             "Eligibility at approval", eligibility ]
    $$"""<section class="forma-stack" aria-labelledby="{{p}}-ch"><h4 id="{{p}}-ch">Charity and eligibility snapshot</h4>{{rows}}</section>"""

let private amountPurpose (p: string) =
    let rows =
        kv [ "Amount", $$"""10% of fund value at approval → {{critical "$12,345.67"}} (fixed <time datetime="2026-11-24">24 Nov 2026</time>)"""
             "Fund value used", money "$123,456.70" + " at 24 Nov 2026, 9:30 AM CT (UTC 15:30)"
             "Purpose", "General operating support"
             "Recognition", "Use my name: Avery Lindqvist" ]
    $$"""<section class="forma-stack" aria-labelledby="{{p}}-am"><h4 id="{{p}}-am">Amount, purpose and recognition</h4>{{rows}}</section>"""

let private docs (p: string) =
    let rows =
        kv [ "Grant confirmation", $$"""<a href="S-007-document-center.html">Open grant confirmation {{ident "DOC-2026-018833"}}</a> · <a href="#pdf">Download PDF</a>"""
             "Remittance advice", "Issued when payment is sent" ]
    $$"""<section class="forma-stack" aria-labelledby="{{p}}-docs"><h4 id="{{p}}-docs">Documents</h4>{{rows}}</section>"""

let private cancelDialog (p: string) =
    let rows =
        kv [ "What changes", "Your sponsor receives a cancellation request; the grant stays Approved until they act"
             "Can it be undone?", "You can withdraw this request until your sponsor acts on it"
             "Who acts next", "Example Charitable Foundation grants team"
             "Expected next step", "Decision within 1 business day. If payment was already sent, the grant cannot be cancelled" ]
    Ui.dialog(
        id = $"{p}-cancel",
        invoker = "Request cancellation…",
        title = "Request cancellation of grant 2026-004817?",
        desc = "Payment may already be in progress. Example Charitable Foundation will try to stop it, but cannot guarantee it.",
        body = rows + Ui.textarea(id = $"{p}-reason", label = "Reason (optional)", desc = "Shared with your sponsor only."),
        actions = """<button type="submit" value="cancel" autofocus>Keep the grant</button><button type="submit" value="request">Request cancellation</button>"""
    )

let private withdrawDialog (p: string) =
    Ui.dialog(
        id = $"{p}-withdraw",
        invoker = "Withdraw recommendation…",
        title = "Withdraw grant recommendation 2026-004821?",
        desc = "The $5,000.00 reserved for this grant returns to Available to grant. This cannot be undone; you can recommend the grant again later.",
        body = kv [ "Who acts next", "No one. The recommendation is closed immediately"; "Charity", "Hope Foundation, EIN 00-0000101" ],
        actions = """<button type="submit" value="cancel" autofocus>Keep the recommendation</button><button type="submit" value="withdraw">Withdraw recommendation</button>"""
    )

let private header (lozenge: string) (title: string option) (sub: string option) (actions: string option) (more: string option) =
    let title = defaultArg title "Grant 2026-004817"
    let sub = defaultArg sub "Riverside Food Bank · 10% of fund value, fixed at $12,345.67"
    let more = defaultArg more """<button type="button">Download confirmation</button>"""
    let reference = refWithCopy "grant reference" (Unbound.Designs.V1.JsCompat.replaceFirst "Grant " "" title)
    Ui.recordHeader(``type`` = "Grant", title = title, lozenge = lozenge, sub = $"{sub}<br>Reference {reference}", crumbs = crumbs, actions = defaultArg actions "", more = more)

let private approved (p: string) =
    let gp =
        grantProgress
            $"{p}-gp"
            (dims
                [ Ui.dim("Funding", "attention", "Funding planned", "2026-11-24T09:31:00-06:00", "24 Nov 2026, 9:31 AM CT")
                  Ui.dim("Payment", "", "Not started") ])
    let tl =
        timeline
            "Grant 2026-004817 history"
            [ Ui.event("2026-11-24T09:31:00-06:00", "24 Nov 2026, 9:31 AM CT", "attention", "Funding planned", "Sale of $12,345.67 from the Balanced Pool scheduled", source = "Source: investment operations")
              Ui.event("2026-11-24T09:30:00-06:00", "24 Nov 2026, 9:30 AM CT", "ok", "Approved", "Approved by Example Charitable Foundation", body = "Amount fixed at $12,345.67 (10% of $123,456.70).", source = "Source: grants approval")
              Ui.event("2026-11-20T10:05:00-06:00", "20 Nov 2026, 10:05 AM CT", "ok", "Eligible", "Charity eligibility confirmed", source = "Source: IRS Business Master File, 18 Nov 2026")
              Ui.event("2026-11-20T10:02:00-06:00", "20 Nov 2026, 10:02 AM CT", "ok", "Submitted", "Recommendation submitted by Avery Lindqvist") ]
    $$"""{{scope}}{{header (loz "attention" "Funding in progress") None None (Some(cancelDialog p)) None}}
{{gp}}
{{charity p}}{{amountPurpose p}}
<section class="forma-stack" aria-labelledby="{{p}}-tl"><h4 id="{{p}}-tl">Timeline</h4>{{tl}}</section>{{docs p}}"""

let private awaiting (p: string) =
    let gp =
        grantProgress
            $"{p}-gp"
            [ Ui.dim("Recommendation", "ok", "Submitted", "2026-12-01T17:02:00-06:00", "1 Dec 2026, 5:02 PM CT")
              Ui.dim("Eligibility", "ok", "Eligible", "2026-12-01T17:03:00-06:00", "1 Dec 2026, 5:03 PM CT")
              Ui.dim("Approval", "attention", "Waiting on sponsor", "2026-12-01T17:03:00-06:00", "1 Dec 2026, 5:03 PM CT")
              Ui.dim("Funding", "", "Not started")
              Ui.dim("Payment", "", "Not started") ]
    let h =
        header (loz "attention" "Waiting on sponsor approval") (Some "Grant 2026-004821") (Some "Hope Foundation, Springfield, IL · $5,000.00") (Some(withdrawDialog p)) None
    $"{scope}{h}\n{gp}"

let private outcomeUnknown (p: string) =
    let gp =
        grantProgress
            $"{p}-gp"
            (dims
                [ Ui.dim("Funding", "ok", "Funded", "2026-11-25T15:00:00-06:00", "25 Nov 2026, 3:00 PM CT")
                  Ui.dim("Payment", "unknown", "Payment outcome unknown", "2026-11-27T11:40:00-06:00", "27 Nov 2026, 11:40 AM CT") ])
    let op =
        Ui.opStatus(
            id = $"{p}-pu",
            state = "unknown",
            label = "Payment outcome unknown",
            title = "The bank has not confirmed this payment",
            body = "<p>The ACH payment was sent to the bank on 27 Nov 2026 but no confirmation or return has been received. Example Charitable Foundation is reconciling with the bank. No action is needed from you; it will not be sent twice.</p>"
        )
    let h = header (loz "unknown" "Payment outcome unknown") None None None None
    $"{scope}{h}\n{gp}\n{op}"

let private voided (_p: string) =
    let h = header (loz "blocked" "Check voided; amount re-credited") (Some "Grant 2026-001204") (Some "Northgate Youth Orchestra · $1,000.00 by check") None None
    let tl =
        timeline
            "Grant 2026-001204 payment history"
            [ Ui.event("2026-11-30T08:00:00-06:00", "30 Nov 2026", "ok", "Re-credited", "$1,000.00 re-credited to Available to grant", source = "Source: general ledger entry JE-2026-118204")
              Ui.event("2026-11-29T08:00:00-06:00", "29 Nov 2026", "blocked", "Voided", "Check 004417 for $1,000.00 voided", body = "Not cashed within 180 days of issue.")
              Ui.event("2026-06-02T08:00:00-06:00", "2 Jun 2026", "attention", "Outstanding", "Check 004417 for $1,000.00 outstanding", body = "Mailed to the charity; not yet cashed.")
              Ui.event("2026-06-01T08:00:00-06:00", "1 Jun 2026", "ok", "Issued", "Check 004417 for $1,000.00 issued") ]
    $"{scope}{h}\n{tl}"

let private returned (p: string) =
    let h = header (loz "blocked" "Payment returned") (Some "Grant 2026-003377") (Some "Lakeview Arts Collective · $750.00 by ACH") None None
    let op =
        Ui.opStatus(
            id = $"{p}-ret",
            state = "blocked",
            label = "Payment returned",
            symbol = "×",
            title = "The charity's bank returned this payment",
            body = "<p>Returned on 18 Nov 2026: account closed. $750.00 was re-credited to Available to grant on 19 Nov 2026. Example Charitable Foundation is asking the charity for a verified new account.</p>"
        )
    $"{scope}{h}\n{op}"

let private blocked (p: string) =
    let h = header (loz "blocked" "Cannot be paid") (Some "Grant 2026-004102") (Some "Recipient organization · $2,000.00") None (Some "")
    let op =
        Ui.opStatus(
            id = $"{p}-blk",
            state = "blocked",
            label = "Cannot be paid",
            symbol = "×",
            title = "This grant cannot be paid",
            body = "<p>Example Charitable Foundation cannot pay this grant under its compliance obligations. The amount remains reserved while your sponsor completes its process; they will contact you about returning it to Available to grant.</p>",
            actions = [ """<button type="button">Contact your sponsor</button>""" ]
        )
    $"{scope}{h}\n{op}"

let screen: Screen =
    { Id = "S-004"
      File = "S-004-grant-detail.html"
      Title = "Grant detail"
      ReqIds = [ "UNB-UI-S-004"; "UNB-UI-G-002"; "UNB-UI-G-016"; "UNB-UI-G-017"; "UNB-UI-G-018" ]
      Audience = "donor"
      Brand = true
      Lead = "One grant: headline status, five-dimension progress, charity snapshot, amount, timeline, documents and only the actions allowed in its current state."
      States =
        [ state
              "Approved, funding in progress"
              "Donor"
              "UNB-UI-G-002 AC-1 (headline 'Funding in progress'), G-017 AC-1 percentage display, G-016 reference with copy, Request cancellation dialog"
              approved
          state "Awaiting approval (withdraw allowed)" "Donor" "Actions allowed in the current state only: Withdraw before approval, with G-005 dialog" awaiting
          state "Payment outcome unknown" "Donor" "UNB-UI-G-002 AC-2 and UNB-UI-G-001 AC-1 (headline 'Payment outcome unknown'; 'Paid' shown nowhere)" outcomeUnknown
          state
              "Check voided after 180 days and re-credited"
              "Donor"
              "UNB-UI-S-004 AC-1 (timeline Issued → Outstanding → Voided → Re-credited with dates and amounts)"
              voided
          state "Returned by the charity's bank" "Donor" "Returned payment state with reason and next step" returned
          state "Blocked by a confirmed sanctions match" "Donor" "UNB-UI-S-004 AC-2 ('This grant cannot be paid', blocked, no screening details)" blocked ]
      Coverage =
        [ cov "UNB-UI-S-004" "AC-1" Yes "State 4 timeline lists Issued, Outstanding, Voided, Re-credited with dates and $1,000.00 amounts (newest first)."
          cov "UNB-UI-S-004" "AC-2" NeedsApplicationBehaviour "State 6 shows 'This grant cannot be paid' (blocked) with no screening details. Ensuring no screening data reaches the client is server behaviour."
          cov "UNB-UI-G-002" "AC-1" NeedsApplicationBehaviour "State 1 headline 'Funding in progress' (attention). The deterministic derivation is application logic; Forma has no multi-dimension status primitive (FORMA-GAP-09)."
          cov "UNB-UI-G-002" "AC-2" NeedsApplicationBehaviour "State 3 headline 'Payment outcome unknown' (unknown) while Funding = Funded."
          cov "UNB-UI-G-001" "AC-1" Yes "State 3: 'Payment outcome unknown' data-state=unknown; the word Paid does not appear in that state."
          cov "UNB-UI-G-016" "AC-1" NeedsApplicationBehaviour "Reference 2026-004817 in forma-identifier with a Copy button; copying is application behaviour (Forma 'copyable field' is planned P1). Documents are Folio scope."
          cov "UNB-UI-G-017" "AC-1" Yes "Text '10% of fund value at approval → $12,345.67 (fixed 24 Nov 2026)'."
          cov "UNB-UI-G-018" "AC-1" NeedsApplicationBehaviour "Legal dates shown as absolute dates in CT with UTC in details; time-zone conversion is application behaviour." ]
      Gaps = [ "FORMA-GAP-01"; "FORMA-GAP-06"; "FORMA-GAP-07"; "FORMA-GAP-09"; "FORMA-GAP-14" ]
      Render = None }
