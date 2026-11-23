# Monthly fund statements: generation, reconciliation, distribution and history
Date: 2026-11-23. Proposed requirements only; no implementation, financial reporting certification or executed tests.

## Product decision
Unbound must produce fund statements every month by default, using approved period-end financial data and sponsor policy. Separate **statement financial content**, **document rendering**, **publication**, and **recipient delivery**. Statements are not merely emails. A monthly statement is an immutable as-issued snapshot with a distinct StatementId, statement period, sponsor/legal entity, fund scope, accounting/valuation version, template/layout/brand version and audit provenance.

## Coverage and schedule
- Default monthly calendar period in sponsor's designated accounting timezone; explicit close cutoff, reconciliation gates, generation deadline and distribution deadline. All deadlines configurable by sponsor and supported by SLA monitoring.
- Support per-fund statements, authorized consolidated multi-fund statements, zero-activity statements, closed/suspended funds, newly opened partial periods, corrected/restated statements and ad hoc historical regeneration under governed permissions.
- Sponsor may choose delivery preference: secure portal, notification email with authenticated link, physical mail where required/requested, and accessible alternative formats. Email attachment policy must be sponsor-governed and privacy reviewed; recommend secure portal by default.
- Distinguish creation, reconciled, approved, published, delivered/available and viewed statuses. A statement marked generated is not necessarily approved or delivered.
- Monthly batch scheduler should handle timezones, month-end, leap years, holidays, provider delays, reconciliation exceptions and large sponsor populations. No duplicate statements on retry.

## Statement contents
Required candidate sections: sponsor legal/display identity; fund/account names and numeric reference; statement period and valuation as-of date; opening and closing net assets; opening and closing available-to-grant where supported; contributions (cash/in-kind, reversals); grants (approved vs disbursed clearly separated); investment returns/income, realized/unrealized changes where policy allows; fees and expenses (including manual check/mailing); transfers, restrictions, cash and investment allocation; pending/unsettled activity; transaction detail and summary; sponsor-approved disclosures, support contact and dispute process.

Avoid misleading equation: closing NAV may not equal immediately grantable liquidity. Reconcile opening balance + activity + market changes - expenses to ending balance using approved sponsor accounting policy. Investment performance methodology, returns, benchmarks, AUM, cost basis and tax acknowledgments require independent CPA/CFO/counsel policy approval before inclusion. A statement is not automatically a tax receipt.

## Data and accounting controls
- Statement inputs come from authoritative, period-closed ledger, fund allocation and approved valuation snapshots, not unverified portal caches or AI summaries.
- Enforce sponsor/fund segregation and donor/advisor/successor authority at publication and access. A consolidated statement cannot include a fund merely because the recipient knows its numeric ID.
- Reconcile GL/fund subledger, custody positions, NAV/unit holdings, grant/payment statuses, fees and cash. Material exceptions block publication or require independently approved exception handling and clear disclosure; never silently invent balances.
- Record effective-time and known-at-time provenance, accounting period, source journal versions, valuation price sources and correction links.
- Preserve original statement when correcting; issue an explicitly numbered revised statement, notify authorized recipients according to policy, retain the original and record reason/approver.
- Grant and contribution reversals, late settlement, closed accounts, pooled unitization, FX, fee accrual, restricted funds and unknown provider statuses require explicit handling.

## Rendering and white labeling
Use the sponsor Client Brand Kit for identity, typography, colors and terminology; Folio is the candidate print/PDF document renderer, subject to technical validation. Statements need a distinct versioned StatementTemplate/Layout model, not reuse email HTML as financial document source. Support accessible tagged PDF where feasible, semantic HTML portal view, print-safe monochrome output, multi-page continuation, page numbers, long transaction lists, currency precision, localization, large-print alternatives and consistent tabular alignment. The statement layout must not hide required financial disclosures or critical status notes.

## Distribution
Publish to secure portal with authenticated, expiring access and record-level authorization. Trigger client-overridable email template 'Monthly statement available' through Communications; send no private balance in unsecured notification by default. Support postal dispatch through approved provider with fee assessment rules, mailing address verification, returned mail, opt-in/paper preferences, suppression for deceased/revoked users and auditable delivery attempts. Provider accepted mail/email is not proof of recipient reading. Accessibility accommodations and notification preferences must be supported.

## Administrator UI and workflow
Sponsor Settings > Statements: monthly schedule, cutoff, templates, branding, mandatory disclosures, delivery defaults, retention and approval thresholds. Statement Operations: batch dashboard by period/sponsor, reconciliation readiness, blocked funds, exceptions, approval queue, preview sample, publish and distribution statuses, retries, correction issuance and audit. Fund detail: Statements tab with period, as-issued version, published date, delivery history and secure download. Administrator can add internal notes/cases on exceptions; a note cannot approve a statement or change ledger balances.

## Component ownership
Fund Accounting owns source financial snapshots; Valuation owns approved NAV/prices; Statement Management owns StatementRun, StatementSnapshot, StatementRevision, StatementApproval, Publication and RecipientStatementAccess; Folio adapter renders printable documents; Brand Kit supplies theme; Communications owns notification templates/delivery; Distribution adapter handles physical mail. Statement publication is a governed authoritative command; notifications use outbox. Do not create independent financial truth in the statement component.

## Security, retention and performance
Statement documents are sensitive financial data. Encrypt and control storage, access and export; avoid public CDN URLs and permanent bearer links. Log recipient access, generation, correction and delivery. Honor legal holds, sponsor retention and privacy policies. Support scalable monthly batch processing with checkpointing, resumability, deduplication, operational monitoring and per-fund error isolation. Do not assert RPO-zero for external providers.

## Acceptance criteria
1. Every eligible fund has one approved monthly statement version or a documented blocking exception by deadline.
2. Opening-to-closing rollforward ties to approved source books under sponsor accounting policy.
3. Reconciled NAV, fees, grants and contributions match period snapshots.
4. No cross-sponsor/fund/recipient leakage in consolidated reports or exports.
5. Immutable as-issued snapshot survives later ledger, brand or template changes.
6. Corrected statements link originals and preserve historical visibility.
7. Scheduled generation is idempotent, resumable and timezone-correct.
8. Notification and document publication are separate, accurately tracked states.
9. Portal, PDF and print representations are accessible and consistent in meaning.
10. Failed delivery, returned mail and stale recipient authority are handled without false success.

## Adversarial test specifications (not executed)
| ID | Scenario | Expected control |
|---|---|---|
| ADV-253 | Month-end NAV still pending | Hold publication or approved qualified exception |
| ADV-254 | Late grant reversal after statement issued | Linked revised statement, original preserved |
| ADV-255 | Sponsor A statement delivered to Sponsor B recipient | Deny via scope and recipient authorization |
| ADV-256 | Recipient loses fund authority before email dispatch | Suppress/revalidate access |
| ADV-257 | Monthly job retried after partial completion | No duplicate statement or notification |
| ADV-258 | Closed fund with midmonth activity | Correct partial/closing statement policy |
| ADV-259 | Two funds consolidated but recipient authorized for only one | Exclude unauthorized fund or reject |
| ADV-260 | Brand logo changed after publication | Original as-issued rendering preserved |
| ADV-261 | Print provider reports accepted but mail returned | Distinct dispatch/return status |
| ADV-262 | Account has negative grant capacity but positive NAV | Both clearly distinguished |
| ADV-263 | FX conversion rate revised | Reconcile and issue correction if material |
| ADV-264 | Fee posted twice before close | Reconciliation exception blocks false statement |
| ADV-265 | Zero-activity fund omitted from monthly batch | Eligible fund included or policy exclusion evidenced |
| ADV-266 | Provider payment Unknown appears as Paid | Accurate pending/Unknown reporting |
| ADV-267 | Statement download link forwarded | Authentication and authorization required |
| ADV-268 | Large-print or screen-reader user cannot interpret PDF | Accessible alternate format available |
| ADV-269 | Sponsor timezone changes around period close | Explicit accounting period boundary and version |
| ADV-270 | Statement published before independent approval | Publication command rejected |
| ADV-271 | Batch failure leaves half the sponsor's funds unprocessed | Checkpoint/retry and exception dashboard |
| ADV-272 | AI-generated explanation contradicts ledger | AI not authoritative; reviewed narrative only |

## Open decisions
- Exact mandatory statement content, presentation and performance methodology require sponsor CFO/CPA and counsel validation.
- Monthly cadence is default; determine sponsor-configurable alternative, fund-specific opt-outs and statement minimum thresholds.
- Determine whether consolidated statements are one legal statement or a cover report enclosing fund statements.
- Choose generation/close SLAs, materiality and approval delegation.
- Decide whether paper statements are default, opt-in or accessibility-driven by sponsor policy, including postage/handling fee disclosures.
- Define archive format, retention and accessible PDF conformance requirements.
- Decide whether sponsor clients can edit financial statement layout, and which blocks are protected from removal.
- Model scale and provider cost for large monthly runs.

Status: requirements only. ADV-253..ADV-272 not executed.
