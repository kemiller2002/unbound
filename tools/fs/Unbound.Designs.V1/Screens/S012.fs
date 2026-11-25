/// S-012 Charity view (port of tools/screens/S-012.mjs).
module Unbound.Designs.V1.Screens.S012

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let private scope = scopeTrail [ "Example Charitable Foundation"; "Charity portal"; "Riverside Food Bank (EIN 00-0000103)" ]

let private grants (_p: string) =
    Ui.dataGrid(
        label = "Grants addressed to Riverside Food Bank",
        cols = [ Ui.col "Grant"; Ui.col "From"; Ui.col "Amount"; Ui.col "Payment status"; Ui.col "Paid"; Ui.col("Actions", plain = true) ],
        rows =
            [ [ ident "2026-004790"; "Example Charitable Foundation, recommended by Avery Lindqvist"; money "$2,500.00"; loz "ok" "Paid"; "30 Nov 2026"; """<button type="button" aria-label="View remittance for grant 2026-004790">View remittance</button>""" ]
              [ ident "2026-004702"; "Anonymous donor via Example Charitable Foundation"; money "$10,000.00"; loz "ok" "Paid"; "20 Nov 2026"; """<button type="button" aria-label="View remittance for grant 2026-004702">View remittance</button>""" ]
              [ ident "2026-004817"; "Example Charitable Foundation, recommended by Avery Lindqvist"; money "$12,345.67"; loz "attention" "Payment scheduled"; "Not yet paid"; """<button type="button" aria-label="View grant 2026-004817">View</button>""" ] ]
    )

let private received (p: string) =
    let remittance =
        kv [ "Legal donor", "Example Charitable Foundation, EIN 00-0000000"
             "Recognition", "Anonymous donor via Example Charitable Foundation"
             "Grant reference", refWithCopy "grant reference" "2026-004702"
             "Amount", critical "$10,000.00"
             "Payment", "ACH to account ••••7710, sent 20 Nov 2026 (CT)"
             "Purpose", "General operating support" ]
    $$"""{{scope}}{{grants p}}
<section class="forma-stack" aria-labelledby="{{p}}-rem"><h3 id="{{p}}-rem">Remittance for grant 2026-004702</h3>{{remittance}}</section>"""

let private pendingChange (p: string) =
    let destinations =
        Ui.dataGrid(
            label = "Payment destinations",
            cols = [ Ui.col "Destination"; Ui.col "Status"; Ui.col "Since" ],
            rows =
                [ [ "ACH to Prairie Community Bank ••••7710"; loz "ok" "Verified: receiving payments"; "3 Mar 2024" ]
                  [ "ACH to Lakeshore Credit Union ••••2298"; loz "attention" "Pending independent verification"; "Requested 30 Nov 2026" ] ]
        )
    let note =
        callout
            $"{p}-note"
            "note"
            "Payments are not redirected yet"
            "<p>Until Example Charitable Foundation verifies the new account independently (by calling the number on your IRS record), all payments continue to Prairie Community Bank ••••7710. You cannot activate a destination yourself.</p>"
    let tl =
        timeline
            "Destination change DST-2026-00412"
            [ Ui.event("2026-11-30T14:20:00-06:00", "30 Nov 2026, 2:20 PM CT", "attention", "Pending independent verification", "Change requested by Jamie Cole", body = "Bank letter uploaded. Callback scheduled.") ]
    $$"""{{scope}}
<section class="forma-stack" aria-labelledby="{{p}}-dest"><h3 id="{{p}}-dest">Payment destinations</h3>{{destinations}}
{{note}}
{{tl}}</section>"""

let private requestChange (p: string) =
    let bank = Ui.field(id = $"{p}-bank", label = "Bank name (required)", attrs = "required")
    let routing = Ui.field(id = $"{p}-routing", label = "Routing number (required)", attrs = "inputmode=\"numeric\" required")
    let acct = Ui.field(id = $"{p}-acct", label = "Account number (required)", attrs = "inputmode=\"numeric\" required autocomplete=\"off\"")
    let upload =
        tag
            "file-upload"
            $$"""<div class="forma-field"><label class="forma-field__label" for="{{p}}-letter">Bank letter or voided check (required)</label><span class="forma-field__description" id="{{p}}-letter-description">PDF or image, up to 10 MB.</span><input id="{{p}}-letter" name="{{p}}-letter" type="file" accept="application/pdf,image/*" required aria-describedby="{{p}}-letter-description"></div>"""
    let form =
        tag
            "form"
            $$"""<form class="forma-form" id="{{p}}-f" aria-labelledby="{{p}}-ft"><h3 id="{{p}}-ft">Request a new payment destination</h3>
<p>We verify every change independently before any payment uses it.</p>
<div class="forma-form-row" data-forma-form-layout="grid">
{{bank}}
{{routing}}
{{acct}}
</div>
{{upload}}
<div class="forma-actions"><button type="submit">Submit for verification</button></div>
</form>"""
    scope + form

let screen: Screen =
    { Id = "S-012"
      File = "S-012-charity-view.html"
      Title = "Charity view"
      ReqIds = [ "UNB-UI-S-012" ]
      Audience = "charity"
      Brand = true
      Lead = "Grants addressed to your organization, with Example Charitable Foundation as the legal donor, remittance details, payment status and verified payment destinations."
      States =
        [ state
              "Grants received, including an anonymous grant"
              "Charity finance user"
              "UNB-UI-S-012 AC-1 (donor field reads 'Anonymous donor via Example Charitable Foundation'); remittance detail"
              received
          state
              "Payment destination change pending verification"
              "Charity finance user"
              "UNB-UI-S-012 AC-2 ('Pending independent verification'; payments continue to the previous verified destination); no self-service activation"
              pendingChange
          state "Request a destination change" "Charity finance user" "DestinationEvidence request form (submission starts verification, never activation)" requestChange ]
      Coverage =
        [ cov "UNB-UI-S-012" "AC-1" Yes "Grant 2026-004702 'From' cell and remittance Recognition read 'Anonymous donor via Example Charitable Foundation'."
          cov "UNB-UI-S-012" "AC-2" NeedsApplicationBehaviour "New destination 'Pending independent verification' and note that payments continue to ••••7710. Payment routing is server behaviour." ]
      Gaps = [ "FORMA-GAP-06"; "FORMA-GAP-07"; "FORMA-GAP-08" ]
      Render = None }
