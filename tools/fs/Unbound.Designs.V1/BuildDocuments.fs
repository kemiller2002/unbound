/// Builds the Unbound print-document prototypes (UNB-UI-D-001..006) from synthetic data (port of
/// tools/build-documents.mjs). Data first (seeded mulberry32, identical to the JavaScript), then pure renderers;
/// `run` is the only part that touches the file system.
/// Usage (from requirements/ui/designs): Unbound.Designs.V1 build-documents
module Unbound.Designs.V1.BuildDocuments

open System
open System.IO
open Unbound.Designs.V1.JsCompat
open Unbound.Designs.V1.DocumentKit

// ---------- D-001 contribution acknowledgment ----------

let acknowledgment (variant: string) =
    let cash = variant = "cash"
    let reference = if cash then "2026-101942" else "2026-101957"
    let docRef = if cash then "2026-880114" else "2026-880121"
    let code = if cash then "7K4P-9QX2-M3RT" else "4HWD-2LZN-8C6V"
    let giftDate = if cash then "2026-11-16" else "2026-11-18"
    let giftTable =
        if cash then
            table
                "Contribution received"
                [ col "Contribution reference"; col "Date of gift"; col "Method"; num "Amount" ]
                [ [ reference; absDate giftDate; "ACH transfer"; usd (cents 25000.0) ] ]
                None
        else
            table
                "Contribution received (noncash property)"
                [ col "Contribution reference"; col "Date of gift"; col "Description of property" ]
                [ [ reference; absDate giftDate; "600 shares of Northwind Analytics, Inc. common stock (fictional issuer)" ] ]
                None
    let valueNote =
        if cash then ""
        else " This acknowledgment describes the property received and states no value; valuing noncash property is the donor's responsibility."
    let title = $"Contribution acknowledgment {reference}"
    let body =
        $$"""<ef-print-document data-document-kind="contribution-acknowledgment" data-requirement="UNB-UI-D-001">
{{runningHeader sponsor.Brand "Contribution acknowledgment" docRef}}
  <main class="ef-dense">
    <ef-print-section>
{{letterhead ()}}
      <p>{{absDate "2026-11-20"}}</p>
      <address>
        <span>Ms. Elena Rivera</span><br>
        <span>48 Lakeview Terrace</span><br>
        <span>Madison, WI 53703</span>
      </address>
      <h1>Contribution acknowledgment</h1>
      <p>Dear Ms. Rivera,</p>
      <p>Thank you for your contribution to {{esc sponsor.Brand}}, received by {{esc sponsor.LegalName}} (EIN {{esc sponsor.Ein}}) for The Rivera Family Fund (fund 100482).{{valueNote}}</p>
      {{giftTable}}
      <ef-print-callout variant="plain">
        <p data-protected-text="goods-or-services">{{esc protectedNoGoods}}</p>
        <p data-protected-text="exclusive-legal-control">{{esc protectedExclusiveControl}}</p>
      </ef-print-callout>
      <p>Sincerely,</p>
      <p>Jordan Okafor<br>Director of Donor Services, {{esc sponsor.LegalName}}</p>
    </ef-print-section>
  </main>
{{runningFooter docRef code true}}
</ef-print-document>"""
    page title $"UNB-UI-D-001 prototype ({variant}) composed only from Folio 0.4.2 primitives." body

// ---------- synthetic fund data for D-002 / D-003 ----------

let charities =
    [ "Lakeshore Food Pantry", "36-1000001"
      "Prairie Literacy Alliance", "36-1000002"
      "Riverside Youth Orchestra", "39-1000003"
      "Northwoods Land Trust", "39-1000004"
      "Madison Free Clinic", "39-1000005"
      "Harbor Animal Rescue", "36-1000006"
      "Coulee Region Habitat Builders", "39-1000007"
      "Great Lakes Science Center", "34-1000008"
      "Second Chance Reentry Network", "36-1000009"
      "Fox Valley Hospice", "39-1000010"
      "Midwest Refugee Partnership", "36-1000011"
      "Dane County Arts League", "39-1000012" ]

let purposes =
    [ "General operating support"; "Youth programs"; "Capital campaign"; "Scholarship fund"; "Emergency relief"; "Conservation easement program"; "Annual fund" ]

let recognition = [ "The Rivera Family"; "Elena and Marco Rivera"; "Anonymous"; "In honor of Rosa Rivera" ]

type Grant = { Ref: string; Iso: string; Name: string; Purpose: string; Amount: int64; Recognition: string }
type Contribution = { Ref: string; Iso: string; Method: string; Amount: int64 }
type Fee = { Iso: string; Amount: int64; Basis: string }
type Pending = { Ref: string; Name: string; Purpose: string; Amount: int64; Status: string }

type FundData =
    { Grants: Grant list
      Contributions: Contribution list
      Fees: Fee list
      Pending: Pending list
      Opening: int64
      Investment: int64
      TotalContrib: int64
      TotalGrants: int64
      TotalFees: int64
      Closing: int64
      Available: int64
      PendingTotal: int64 }

/// Grant i drawn from the generator, in the JavaScript's draw order (charity, two amount draws, purpose, recognition).
let private drawGrant (seed: int32) (i: int) : Grant * int32 =
    let day = 1 + int (floor (float i / 72.0 * 92.0))
    let iso = utcIsoDate 2026 6 day
    let seed, (name, _) = pick seed charities
    let seed, r1 = mulberry32 seed
    let seed, r2 = mulberry32 seed
    let amount = cents (100.0 + floor (r1 * 4900.0) + floor (r2 * 100.0) / 100.0)
    let seed, purpose = pick seed purposes
    let seed, recog = pick seed recognition
    { Ref = "2026-00" + padStart 4 '0' (string (4700 + i * 3)); Iso = iso; Name = name; Purpose = purpose; Amount = amount; Recognition = recog },
    seed

let fundData () =
    let drawn, _ = List.mapFold drawGrant 482 [ 0..71 ]
    // Ensure the G-016 example grant 2026-004817 is present and paid with the D-003 batch.
    let grants =
        drawn
        |> List.mapi (fun i g ->
            if i = 39 then { g with Ref = "2026-004817"; Name = "Lakeshore Food Pantry"; Purpose = "Emergency relief"; Amount = cents 2500.0 } else g)
    let contributions: Contribution list =
        [ "2026-101511", "2026-07-02", "ACH transfer", cents 10000.0
          "2026-101588", "2026-07-21", "Check", cents 2500.0
          "2026-101640", "2026-08-05", "ACH transfer", cents 10000.0
          "2026-101702", "2026-08-30", "Wire transfer", cents 50000.0
          "2026-101733", "2026-09-04", "ACH transfer", cents 10000.0
          "2026-101790", "2026-09-22", "Check", cents 1750.5 ]
        |> List.map (fun (r, iso, m, a) -> { Ref = r; Iso = iso; Method = m; Amount = a })
    let fees: Fee list =
        [ "2026-07-31", cents 1048.17; "2026-08-31", cents 1101.92; "2026-09-30", cents 1123.08 ]
        |> List.map (fun (iso, a) -> { Iso = iso; Amount = a; Basis = "0.85% annual rate on average daily balance, accrued monthly" })
    let pending: Pending list =
        [ "2026-004936", "Northwoods Land Trust", "Conservation easement program", cents 5000.0, "Due diligence"
          "2026-004941", "Madison Free Clinic", "General operating support", cents 1200.0, "Awaiting approval"
          "2026-004947", "Prairie Literacy Alliance", "Youth programs", cents 750.0, "Approved, payment scheduled 2 Oct 2026"
          "2026-004952", "Harbor Animal Rescue", "Annual fund", cents 300.0, "Awaiting approval"
          "2026-004958", "Great Lakes Science Center", "Scholarship fund", cents 2000.0, "Due diligence" ]
        |> List.map (fun (r, n, p, a, s) -> { Ref = r; Name = n; Purpose = p; Amount = a; Status = s })
    let opening = cents 1284310.42
    let investment = cents (-4210.33)
    let totalContrib = contributions |> List.sumBy (fun c -> c.Amount)
    let totalGrants = grants |> List.sumBy (fun g -> g.Amount)
    let totalFees = fees |> List.sumBy (fun f -> f.Amount)
    let closing = opening + totalContrib - totalGrants - totalFees + investment
    let pendingTotal = pending |> List.sumBy (fun p -> p.Amount)
    { Grants = grants
      Contributions = contributions
      Fees = fees
      Pending = pending
      Opening = opening
      Investment = investment
      TotalContrib = totalContrib
      TotalGrants = totalGrants
      TotalFees = totalFees
      Closing = closing
      Available = closing - pendingTotal
      PendingTotal = pendingTotal }

// ---------- D-002 fund statement ----------

let fundStatement () =
    let f = fundData ()
    let reference = "2026-310482"
    let meta =
        metaList
            [ "Statement period", $"""{absDate "2026-07-01"} to {absDate "2026-09-30"}"""
              "Statement reference", reference
              "Advisors", "Elena Rivera, Marco Rivera"
              "Generated from", "Locked period snapshot 2026-Q3 (locked 3 Oct 2026)"
              "Time zone", sponsor.Tz ]
    let summary =
        metricGrid
            [ { Label = "Opening value"; Value = usd f.Opening; Detail = "at " + absDate "2026-07-01"; Strong = false }
              { Label = "Contributions"; Value = usd f.TotalContrib; Detail = $"{List.length f.Contributions} contributions"; Strong = false }
              { Label = "Grants paid"; Value = usd f.TotalGrants; Detail = $"{List.length f.Grants} grants"; Strong = false }
              { Label = "Fees"; Value = usd f.TotalFees; Detail = "Administrative fee"; Strong = false }
              { Label = "Investment change"; Value = signedUsd f.Investment; Detail = "Net of fund expenses"; Strong = false }
              { Label = "Closing value"; Value = usd f.Closing; Detail = "at " + absDate "2026-09-30"; Strong = true }
              { Label = "Available to grant"; Value = usd f.Available; Detail = $"Closing value less {usd f.PendingTotal} pending"; Strong = true } ]
    let contributions =
        table
            "Contributions received in the period"
            [ col "Contribution reference"; col "Date of gift"; col "Method"; num "Amount" ]
            (f.Contributions |> List.map (fun c -> [ c.Ref; absDate c.Iso; c.Method; usd c.Amount ]))
            (Some [ "Total contributions"; ""; ""; usd f.TotalContrib ])
    let grants =
        table
            "Grants paid in the period"
            [ col "Grant reference"; col "Paid on"; col "Charity"; col "Purpose"; num "Amount" ]
            (f.Grants |> List.map (fun g -> [ g.Ref; absDate g.Iso; esc g.Name; esc g.Purpose; usd g.Amount ]))
            (Some [ "Total grants paid"; ""; ""; ""; usd f.TotalGrants ])
    let pending =
        table
            "Grants recommended but not yet paid at period end"
            [ col "Pending reference"; col "Charity"; col "Purpose"; col "Status"; num "Amount" ]
            (f.Pending |> List.map (fun p -> [ p.Ref; esc p.Name; esc p.Purpose; esc p.Status; usd p.Amount ]))
            (Some [ "Total pending"; ""; ""; ""; usd f.PendingTotal ])
    let fees =
        table
            "Administrative fees charged"
            [ col "Charged on"; col "Basis"; num "Amount" ]
            (f.Fees |> List.map (fun x -> [ absDate x.Iso; esc x.Basis; usd x.Amount ]))
            (Some [ "Total fees"; ""; usd f.TotalFees ])
    let body =
        $$"""<ef-print-document data-document-kind="fund-statement" data-requirement="UNB-UI-D-002">
  <ef-print-title-page>
{{letterhead ()}}
    <h1>Fund statement</h1>
    <p>The Rivera Family Fund &middot; fund 100482</p>
    {{meta}}
    <ef-print-toc>
      <nav aria-label="Statement contents">
        <ol>
          <li><a href="#summary">Period summary</a></li>
          <li><a href="#contributions">Contributions</a></li>
          <li><a href="#grants">Grants paid</a></li>
          <li><a href="#pending">Pending grants</a></li>
          <li><a href="#fees">Fees and fee basis</a></li>
          <li><a href="#disclosures">Disclosures</a></li>
        </ol>
      </nav>
    </ef-print-toc>
  </ef-print-title-page>
{{runningHeader sponsor.Brand "Fund statement, 1 Jul 2026 to 30 Sep 2026" reference}}
  <main class="ef-dense">
    <ef-print-section id="summary">
      <h2>Period summary</h2>
      {{summary}}
    </ef-print-section>
    <ef-print-section id="contributions">
      <h2>Contributions</h2>
      {{contributions}}
    </ef-print-section>
    <ef-print-section id="grants">
      <h2>Grants paid</h2>
      {{grants}}
    </ef-print-section>
    <ef-print-section id="pending">
      <h2>Pending grants</h2>
      {{pending}}
    </ef-print-section>
    <ef-print-section id="fees">
      <h2>Fees and fee basis</h2>
      {{fees}}
      <ef-print-note role="note"><p>Fee basis: 0.85% annual rate on the average daily balance, charged monthly in arrears per the {{esc sponsor.Brand}} fee schedule effective {{absDate "2026-01-01"}}.</p></ef-print-note>
    </ef-print-section>
    <ef-print-section id="disclosures">
      <h2>Disclosures</h2>
      <p>Values are stated in US dollars. Investment change is net of underlying fund expenses and is not a guarantee of future value. Grants are made by {{esc sponsor.LegalName}} on the advice of the fund advisors; advice is not binding.</p>
      <ef-print-callout variant="plain">
        <p data-protected-text="exclusive-legal-control">{{esc protectedExclusiveControl}}</p>
        <p data-protected-text="legal-sponsor-disclosure">{{esc protectedDisclosure}}</p>
      </ef-print-callout>
    </ef-print-section>
  </main>
{{runningFooter reference "9TQM-3BX7-KD2F" true}}
</ef-print-document>"""
    page $"Fund statement {reference}" "UNB-UI-D-002 prototype: quarterly fund statement with 72 grant rows, composed only from Folio 0.4.2 primitives." body

// ---------- D-003 grant remittance + donor confirmation ----------

type BatchGrant = { Ref: string; Fund: string; Recognition: string; Purpose: string; Amount: int64 }

let remittance () =
    let batch: BatchGrant list =
        [ { Ref = "2026-004817"; Fund = "The Rivera Family Fund"; Recognition = "The Rivera Family"; Purpose = "Emergency relief"; Amount = cents 2500.0 }
          { Ref = "2026-004822"; Fund = "Okonkwo Charitable Fund"; Recognition = "Anonymous"; Purpose = "General operating support"; Amount = cents 10000.0 }
          { Ref = "2026-004829"; Fund = "Lindqvist Family Fund"; Recognition = "In memory of Karin Lindqvist"; Purpose = "Youth programs"; Amount = cents 1250.75 }
          { Ref = "2026-004831"; Fund = "Chen-Alvarez Giving Fund"; Recognition = "Mei Chen and Luis Alvarez"; Purpose = "Food distribution, winter 2026"; Amount = cents 500.0 } ]
    let total = batch |> List.sumBy (fun b -> b.Amount)
    let remitRef = "2026-550207"
    let confirmRef = "2026-880167"
    let paymentDate = $"""{absDate "2026-08-19"} ({sponsor.Tz}; 2026-08-19T14:00:00Z UTC)"""
    let remitMeta =
        metaList
            [ "Payee", "Lakeshore Food Pantry (EIN 36-1000001)"
              "Remittance reference", remitRef
              "Payment method", "ACH credit to account ending 4471"
              "Payment date", paymentDate
              "Total paid", usd total ]
    let grants =
        table
            "Grants included in this payment"
            [ col "Grant reference"; col "Recognition"; col "Purpose"; num "Amount" ]
            (batch |> List.map (fun b -> [ b.Ref; esc b.Recognition; esc b.Purpose; usd b.Amount ]))
            (Some [ "Total paid"; ""; ""; usd total ])
    let confirmMeta =
        metaList
            [ "Advisor", "Elena Rivera, The Rivera Family Fund (fund 100482)"
              "Grant reference", "2026-004817"
              "Document reference", confirmRef
              "Legal donor", $"{esc sponsor.LegalName} (EIN {esc sponsor.Ein})"
              "Recipient", "Lakeshore Food Pantry (EIN 36-1000001)"
              "Amount", usd (cents 2500.0)
              "Payment method", "ACH credit"
              "Payment date", paymentDate
              "Purpose", "Emergency relief"
              "Recognition", "The Rivera Family (as you chose)" ]
    let body =
        $$"""<ef-print-document data-document-kind="grant-remittance" data-requirement="UNB-UI-D-003">
{{runningHeader sponsor.Brand "Grant remittance advice" remitRef}}
  <main class="ef-dense">
    <ef-print-section>
{{letterhead ()}}
      <p>{{absDate "2026-08-19"}}</p>
      <address>
        <span>Lakeshore Food Pantry</span><br>
        <span>Attn: Finance Office</span><br>
        <span>310 Harbor Street</span><br>
        <span>Milwaukee, WI 53202</span>
      </address>
      <h1>Grant remittance advice</h1>
      {{remitMeta}}
      <p>{{esc sponsor.LegalName}} (EIN {{esc sponsor.Ein}}) is the donor of each grant below. The grants were recommended by advisors of donor advised funds sponsored by {{esc sponsor.LegalName}}. Recognition is shown as each advisor chose.</p>
      {{grants}}
      <ef-print-note role="note"><p>Please acknowledge these grants to {{esc sponsor.LegalName}} as the donor. No goods or services may be provided to any advisor in exchange for these grants.</p></ef-print-note>
    </ef-print-section>
    <ef-print-section break-before="page">
{{letterhead ()}}
      <h2>Grant confirmation for the advisor</h2>
      {{confirmMeta}}
      <p>{{esc sponsor.LegalName}} made this grant on your recommendation. The grant is not a contribution by you and is not tax-deductible to you.</p>
    </ef-print-section>
  </main>
{{runningFooter remitRef "2RNX-6VJH-Q8LS" true}}
</ef-print-document>"""
    page
        $"Grant remittance advice {remitRef}"
        "UNB-UI-D-003 prototype: charity remittance advice for several grants plus donor grant confirmation, composed only from Folio 0.4.2 primitives."
        body

// ---------- D-004 annual DAF reporting package ----------

type ReconLine = { Line: string; Measure: string; Pkg: string; Ledger: string; Diff: string; Src: string }

let reportingPackage () =
    let reference = "2026-770025"
    let snapshot = sha256Hex "unbound-ledger-snapshot|Example Charitable Foundation|TY2025|locked 2026-02-14"
    let money n = usd (cents n)
    let lines =
        [ { Line = "Line 1(a)"; Measure = "Number of donor advised funds at year end"; Pkg = "1,284"; Ledger = "1,284"; Diff = "0"; Src = "Fund register snapshot, classification register v2025.4" }
          { Line = "Line 2(a)"; Measure = "Aggregate value of contributions to"; Pkg = money 48210773.18; Ledger = money 48210773.18; Diff = usd 0L; Src = "GL 4100 Contributions, TY2025 close" }
          { Line = "Line 3(a)"; Measure = "Aggregate grants from"; Pkg = money 39874012.55; Ledger = money 39874012.55; Diff = usd 0L; Src = "GL 5100 Grants paid, TY2025 close" }
          { Line = "Line 4(a)"; Measure = "Aggregate value at end of year"; Pkg = money 412907551.06; Ledger = money 412907551.06; Diff = usd 0L; Src = "GL 1200 Investments + 1010 Cash, 31 Dec 2025" } ]
    let classification =
        [ [ "Donor advised fund"; "1,284"; "Included in Part I column (a)" ]
          [ "Field-of-interest fund"; "37"; "Excluded (column (b), other funds)" ]
          [ "Designated fund"; "12"; "Excluded (column (b), other funds)" ]
          [ "Closed during year, zero balance at year end"; "41"; "Excluded from year-end count; activity included in lines 2(a) and 3(a)" ] ]
    let meta =
        metaList
            [ "Legal sponsor", $"{esc sponsor.LegalName} (EIN {esc sponsor.Ein})"
              "Tax year", $"""2025 ({absDate "2025-01-01"} to {absDate "2025-12-31"})"""
              "Package reference", reference
              "Tax year locked", $"""{absDate "2026-02-14"} ({sponsor.Tz})""" ]
    let line i = List.item i lines
    let figures =
        metricGrid
            [ { Label = "Donor advised funds at year end"; Value = "1,284"; Detail = "Line 1(a)"; Strong = true }
              { Label = "Aggregate contributions"; Value = (line 1).Pkg; Detail = "Line 2(a)"; Strong = false }
              { Label = "Aggregate grants"; Value = (line 2).Pkg; Detail = "Line 3(a)"; Strong = false }
              { Label = "Aggregate value at year end"; Value = (line 3).Pkg; Detail = "Line 4(a)"; Strong = false } ]
    let recon =
        table
            "Schedule D Part I column (a) reconciliation"
            [ col "Line"; col "Measure"; num "Package"; num "Ledger"; num "Difference"; col "Ledger source" ]
            (lines |> List.map (fun l -> [ l.Line; esc l.Measure; l.Pkg; l.Ledger; l.Diff; esc l.Src ]))
            None
    let classes =
        table
            "Fund classification at year end (classification register v2025.4)"
            [ col "Classification"; num "Funds"; col "Treatment" ]
            (classification |> List.map (List.map esc))
            None
    let signoff =
        metaList
            [ "Prepared by", "Priya Natarajan, Senior Accountant"
              "Prepared on", $"""{absDate "2026-02-16"} (2026-02-16T21:14:03Z UTC)"""
              "Reviewed and signed by", "Samuel Adeyemi, Chief Financial Officer"
              "Signature", "Signed electronically, signature record 2026-SIG-00318"
              "Signed on", $"""{absDate "2026-02-18"} (2026-02-18T16:02:47Z UTC)""" ]
    let body =
        $$"""<ef-print-document data-document-kind="daf-reporting-package" data-requirement="UNB-UI-D-004">
  <ef-print-title-page>
{{letterhead ()}}
    <h1>Annual DAF reporting package</h1>
    <p>Support for Form 990, Schedule D, Part I, column (a)</p>
    {{meta}}
  </ef-print-title-page>
{{runningHeader sponsor.LegalName "Annual DAF reporting package, tax year 2025" reference}}
  <main class="ef-dense">
    <ef-print-section>
      <h2>Year-end figures</h2>
      {{figures}}
      <ef-print-integrity status="complete" role="group" aria-labelledby="pkg-integrity">
        <h3 id="pkg-integrity">Source and reconciliation status</h3>
        <dl>
          <div><dt>Reconciliation</dt><dd>4 of 4 lines agree to ledger</dd></div>
          <div><dt>Classification register</dt><dd>v2025.4 (approved {{absDate "2026-01-20"}})</dd></div>
          <div><dt>Ledger close</dt><dd>TY2025 close 2026-0042</dd></div>
          <div><dt>Source snapshot SHA-256</dt><dd><code>{{snapshot}}</code></dd></div>
        </dl>
        <p data-limitations><strong>Scope.</strong> Figures cover funds sponsored by {{esc sponsor.LegalName}} only. Funds administered for other sponsors are reported in their own packages.</p>
      </ef-print-integrity>
    </ef-print-section>
    <ef-print-section>
      <h2>Reconciliation to the ledger</h2>
      {{recon}}
      {{classes}}
    </ef-print-section>
    <ef-print-section>
      <h2>Preparation and sign-off</h2>
      <ef-print-keep>
        {{signoff}}
      </ef-print-keep>
    </ef-print-section>
  </main>
{{runningFooter reference "6PLC-WM4E-7TZB" true}}
</ef-print-document>"""
    page
        $"DAF reporting package {reference}"
        "UNB-UI-D-004 prototype: annual DAF reporting package supporting Form 990 Schedule D Part I, composed only from Folio 0.4.2 primitives."
        body

// ---------- D-005 case decision record ----------

let caseRecord () =
    let reference = "2026-000731"
    let ev =
        [ "EV-1", "OFAC SDN list entry (fictional)", "Treasury OFAC SDN feed", "2026-11-19T06:00:00Z", "Same day"
          "EV-2", "Payee registration record", "Unbound charity registry", "2026-11-19T13:22:41Z", "Same day"
          "EV-3", "IRS Pub. 78 / EO BMF lookup", "IRS EO BMF monthly file", "2026-11-12T00:00:00Z", "7 days"
          "EV-4", "Bank account ownership attestation", "Payee upload", "2026-11-19T15:03:10Z", "Same day"
          "EV-5", "Screening engine match explanation", "Sanctions screening service", "2026-11-19T13:22:45Z", "Same day" ]
        |> List.map (fun (id, item, src, at, fresh) ->
            [ id; esc item; esc src; $"{absDate (at.Substring(0, 10))} ({at} UTC)"; esc fresh; $"<code>{sha256Hex (id + item)}</code>" ])
    let meta =
        metaList
            [ "Case reference", reference
              "Raised by rule", "SAN-OFAC-001 version 3: payee name and address match against the OFAC SDN list, score 0.92 or higher"
              "Subject", "Grant 2026-004903 to payee Meridian Relief Collective (fictional)"
              "Case opened", $"""{absDate "2026-11-19"} 07:22 {sponsor.Tz} (2026-11-19T13:22:45Z UTC)"""
              "Decision recorded", $"""{absDate "2026-11-20"} 10:41 {sponsor.Tz} (2026-11-20T16:41:09Z UTC)"""
              "Prepared by", "Analyst Dana Whitfield (user 4471), compliance operations"
              "Decided by", "Marcus Lee (user 1093), Chief Compliance Officer" ]
    let evidence =
        table
            "Evidence considered, with source and freshness"
            [ col "Item"; col "Evidence"; col "Source"; col "Retrieved"; col "Freshness"; col "SHA-256" ]
            ev
            None
    let body =
        $$"""<ef-print-document data-document-kind="case-decision-record" data-requirement="UNB-UI-D-005">
{{runningHeader sponsor.LegalName "Case decision record (internal, audit evidence)" reference}}
  <main class="ef-dense">
    <ef-print-section>
      <h1>Case decision record: sanctions screening match</h1>
      {{meta}}
      <ef-print-callout>
        <p><strong>OFAC report due {{absDate "2026-12-04"}}.</strong> Blocked property report due within 10 business days of blocking (blocked {{absDate "2026-11-20"}}).</p>
      </ef-print-callout>
    </ef-print-section>
    <ef-print-section>
      <h2>Evidence</h2>
      {{evidence}}
    </ef-print-section>
    <ef-print-section>
      <h2>Decision</h2>
      <ef-print-finding>
        <dl>
          <div><dt>Observation</dt><dd>Payee name, street address and bank country match SDN entry (fictional) on three of three identifying attributes (EV-1, EV-2, EV-5).</dd></div>
          <div><dt>Decision</dt><dd>Confirmed match. Grant 2026-004903 payment blocked; funds moved to a blocked interest-bearing account.</dd></div>
          <div><dt>Rationale</dt><dd>Attribute agreement exceeds the confirmation threshold; payee attestation (EV-4) names the same beneficial owner. No contrary evidence.</dd></div>
          <div><dt>Follow-up</dt><dd>File blocked property report with OFAC by {{absDate "2026-12-04"}}; notify the fund advisor that the grant cannot be paid, without disclosing the screening reason.</dd></div>
        </dl>
      </ef-print-finding>
      <ef-print-note role="note"><p>Separation of duties: the preparer and the decider are different people (user 4471 and user 1093).</p></ef-print-note>
    </ef-print-section>
  </main>
{{runningFooter reference "8DKS-5RWA-1MPH" false}}
</ef-print-document>"""
    page $"Case decision record {reference}" "UNB-UI-D-005 prototype: sanctions case decision record, composed only from Folio 0.4.2 primitives." body

// ---------- D-006 audit evidence package ----------

type Control = { Id: string; Title: string; N: int }

let evidenceItems =
    [ "Access request ticket"; "Approval record"; "System log extract"; "Pull request review"; "Quarterly access review"; "Payment approval pair"; "Alert triage record" ]

/// Evidence rows for one control, continuing the generator: (next seed, rows).
let private evidenceRows (reference: string) (seed: int32) (c: Control) =
    let row seed i =
        let seed, r = mulberry32 seed
        let day = 1 + int (floor (r * 91.0))
        let d = utcIsoDate 2025 9 day
        let seed, item = pick seed evidenceItems
        let id = $"""{c.Id}-E{padStart 2 '0' (string (i + 1))}"""
        let digest = sha256Hex $"{reference}|{id}|{item}|{d}"
        [ id; esc item; absDate d; $"<code>{digest}</code>" ], seed
    let rows, seed = List.mapFold row seed [ 0 .. c.N - 1 ]
    (c, rows), seed

let auditPackage () =
    let reference = "2026-990004"
    let controls =
        [ { Id = "CC6.1"; Title = "Logical access provisioning"; N = 9 }
          { Id = "CC6.3"; Title = "Access removal on termination"; N = 7 }
          { Id = "CC7.2"; Title = "Security event monitoring"; N = 8 }
          { Id = "CC8.1"; Title = "Change management"; N = 10 }
          { Id = "FIN-03"; Title = "Grant disbursement dual approval"; N = 9 } ]
    let evidence, _ = List.mapFold (evidenceRows reference) 2025 controls
    let exceptions =
        [ [ "EX-01"; "CC6.3"; "One leaver account disabled 3 business days after termination (policy: 1)."; "Account had no activity after termination (log extract CC6.3-E04). Offboarding automation fixed 12 Nov 2025."; "Closed" ]
          [ "EX-02"; "FIN-03"; "One grant batch approved by two users in the same role group."; "Batch re-approved by finance controller 9 Dec 2025; role groups separated."; "Closed" ] ]
    let manifest =
        evidence
        |> List.map (fun (_, rows) -> JsArray(rows |> List.map (fun r -> JsArray(r |> List.map JsString))))
        |> JsArray
        |> stringify None
        |> sha256Hex
    let meta =
        metaList
            [ "Package reference", reference
              "Prepared for", "External auditor (SOC 2 Type II examination)"
              "Generated", $"""{absDate "2026-01-09"} (2026-01-09T18:30:00Z UTC)"""
              "Evidence items", string (controls |> List.sumBy (fun c -> c.N)) ]
    let toc =
        controls |> List.map (fun c -> $"            <li><a href=\"#control-{c.Id}\">Control {c.Id}: {esc c.Title}</a></li>") |> String.concat "\n"
    let controlList =
        table
            "Controls in scope"
            [ col "Control"; col "Description"; num "Evidence items" ]
            (controls |> List.map (fun c -> [ c.Id; esc c.Title; string c.N ]))
            None
    let section (c: Control, rows: string list list) =
        let t = table $"Evidence for control {c.Id}" [ col "Evidence ID"; col "Item"; col "Dated"; col "SHA-256" ] rows None
        $$"""    <ef-print-section id="control-{{c.Id}}">
      <h2>Control {{c.Id}}: {{esc c.Title}}</h2>
      {{t}}
    </ef-print-section>"""
    let sections = evidence |> List.map section |> String.concat "\n"
    let exceptionTable =
        table
            "Exceptions identified in the period"
            [ col "Exception"; col "Control"; col "Description"; col "Resolution"; col "Status" ]
            (exceptions |> List.map (List.map esc))
            None
    let body =
        $$"""<ef-print-document data-document-kind="audit-evidence-package" data-requirement="UNB-UI-D-006">
  <ef-print-title-page>
{{letterhead ()}}
    <h1>Audit evidence package</h1>
    <p>Period {{absDate "2025-10-01"}} to {{absDate "2025-12-31"}} (Q4 2025)</p>
    {{meta}}
    <ef-print-integrity status="complete" role="group" aria-labelledby="manifest-h">
      <h2 id="manifest-h">Package manifest</h2>
      <dl>
        <div><dt>Controls</dt><dd>{{List.length controls}}</dd></div>
        <div><dt>Exceptions</dt><dd>{{List.length exceptions}} (all closed)</dd></div>
        <div><dt>Manifest SHA-256</dt><dd><code>{{manifest}}</code></dd></div>
      </dl>
    </ef-print-integrity>
  </ef-print-title-page>
{{runningHeader sponsor.LegalName "Audit evidence package, Q4 2025" reference}}
  <main class="ef-dense">
    <ef-print-section id="contents">
      <h2>Contents</h2>
      <ef-print-toc>
        <nav aria-label="Package contents">
          <ol>
            <li><a href="#controls">Control list</a></li>
{{toc}}
            <li><a href="#exceptions">Exceptions and resolution</a></li>
          </ol>
        </nav>
      </ef-print-toc>
    </ef-print-section>
    <ef-print-section id="controls">
      <h2>Control list</h2>
      {{controlList}}
    </ef-print-section>
{{sections}}
    <ef-print-section id="exceptions">
      <h2>Exceptions and resolution</h2>
      {{exceptionTable}}
    </ef-print-section>
  </main>
{{runningFooter reference "3VFE-HN8Q-5XYA" false}}
</ef-print-document>"""
    page $"Audit evidence package {reference}" "UNB-UI-D-006 prototype: period-bounded audit evidence package, composed only from Folio 0.4.2 primitives." body

// ---------- write ----------

let documents: (string * (unit -> string)) list =
    [ "D-001-acknowledgment-cash.html", (fun () -> acknowledgment "cash")
      "D-001-acknowledgment-securities.html", (fun () -> acknowledgment "securities")
      "D-002-fund-statement.html", fundStatement
      "D-003-grant-remittance.html", remittance
      "D-004-daf-reporting-package.html", reportingPackage
      "D-005-case-decision-record.html", caseRecord
      "D-006-audit-evidence-package.html", auditPackage ]

/// Writes documents/*.html under baseDir (the working directory when run from the command line) and prints their
/// names; returns the exit code.
let run (baseDir: string) =
    let outDir = Path.GetFullPath(Path.Combine(baseDir, "documents"))
    Directory.CreateDirectory outDir |> ignore
    for name, build in documents do
        File.WriteAllText(Path.Combine(outDir, name), build ())
    Console.Out.Write((documents |> List.map fst |> String.concat "\n") + "\n")
    0
