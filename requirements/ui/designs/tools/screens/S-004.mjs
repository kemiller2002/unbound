import { tag, loz, fresh, kv, money, critical, ident, refWithCopy, recordHeader, grantProgress, timeline, dialog, opStatus, scopeTrail, textarea } from "../lib/markup.mjs";

const scope = scopeTrail(["Example Charitable Foundation", "Harbor Giving donor-advised funds", "Lindqvist Family Fund (F-2026-000418)", "Grant 2026-004817"]);
const crumbs = `<a href="S-001-giving-home.html">Giving home</a> <span aria-hidden="true">/</span> <a href="#grants">Grants</a>`;

const t = (d, time) => ({ iso: `${d}T${time}:00-06:00` });
const dims = (overrides) => [
  { name: "Recommendation", state: "ok", label: "Submitted", iso: "2026-11-20T10:02:00-06:00", shown: "20 Nov 2026, 10:02 AM CT" },
  { name: "Eligibility", state: "ok", label: "Eligible", iso: "2026-11-20T10:05:00-06:00", shown: "20 Nov 2026, 10:05 AM CT" },
  { name: "Approval", state: "ok", label: "Approved", iso: "2026-11-24T09:30:00-06:00", shown: "24 Nov 2026, 9:30 AM CT" },
  ...overrides,
];

const charity = (p) => `<section class="forma-stack" aria-labelledby="${p}-ch"><h4 id="${p}-ch">Charity and eligibility snapshot</h4>${kv([
  ["Charity", "Riverside Food Bank"],
  ["EIN", ident("00-0000103")],
  ["Address", "410 River Road, Springfield, IL"],
  ["Eligibility at approval", `${loz("ok", "Eligible: public charity")}${fresh({ label: "IRS data as of", iso: "2026-11-23", shown: "23 Nov 2026", stateText: "Current at approval" })}`],
])}</section>`;

const amountPurpose = (p) => `<section class="forma-stack" aria-labelledby="${p}-am"><h4 id="${p}-am">Amount, purpose and recognition</h4>${kv([
  ["Amount", `10% of fund value at approval → ${critical("$12,345.67")} (fixed <time datetime="2026-11-24">24 Nov 2026</time>)`],
  ["Fund value used", `${money("$123,456.70")} at 24 Nov 2026, 9:30 AM CT (UTC 15:30)`],
  ["Purpose", "General operating support"],
  ["Recognition", "Use my name: Avery Lindqvist"],
])}</section>`;

const docs = (p) => `<section class="forma-stack" aria-labelledby="${p}-docs"><h4 id="${p}-docs">Documents</h4>${kv([
  ["Grant confirmation", `<a href="S-007-document-center.html">Open grant confirmation ${ident("DOC-2026-018833")}</a> · <a href="#pdf">Download PDF</a>`],
  ["Remittance advice", "Issued when payment is sent"],
])}</section>`;

const cancelDialog = (p) =>
  dialog({
    id: `${p}-cancel`,
    invoker: "Request cancellation…",
    title: "Request cancellation of grant 2026-004817?",
    desc: "Payment may already be in progress. Example Charitable Foundation will try to stop it, but cannot guarantee it.",
    body: `${kv([
      ["What changes", "Your sponsor receives a cancellation request; the grant stays Approved until they act"],
      ["Can it be undone?", "You can withdraw this request until your sponsor acts on it"],
      ["Who acts next", "Example Charitable Foundation grants team"],
      ["Expected next step", "Decision within 1 business day. If payment was already sent, the grant cannot be cancelled"],
    ])}${textarea({ id: `${p}-reason`, label: "Reason (optional)", desc: "Shared with your sponsor only." })}`,
    actions: `<button type="submit" value="cancel" autofocus>Keep the grant</button><button type="submit" value="request">Request cancellation</button>`,
  });

const withdrawDialog = (p) =>
  dialog({
    id: `${p}-withdraw`,
    invoker: "Withdraw recommendation…",
    title: "Withdraw grant recommendation 2026-004821?",
    desc: "The $5,000.00 reserved for this grant returns to Available to grant. This cannot be undone; you can recommend the grant again later.",
    body: kv([["Who acts next", "No one. The recommendation is closed immediately"], ["Charity", "Hope Foundation, EIN 00-0000101"]]),
    actions: `<button type="submit" value="cancel" autofocus>Keep the recommendation</button><button type="submit" value="withdraw">Withdraw recommendation</button>`,
  });

const header = (p, { title = "Grant 2026-004817", lozenge, sub = "Riverside Food Bank · 10% of fund value, fixed at $12,345.67", actions = "", more = '<button type="button">Download confirmation</button>' }) =>
  `${recordHeader({ type: "Grant", title, lozenge, sub: `${sub}<br>Reference ${refWithCopy("grant reference", title.replace("Grant ", ""))}`, crumbs, actions, more })}`;

export default {
  id: "S-004",
  file: "S-004-grant-detail.html",
  title: "Grant detail",
  reqIds: ["UNB-UI-S-004", "UNB-UI-G-002", "UNB-UI-G-016", "UNB-UI-G-017", "UNB-UI-G-018"],
  audience: "donor",
  brand: true,
  lead: "One grant: headline status, five-dimension progress, charity snapshot, amount, timeline, documents and only the actions allowed in its current state.",
  states: [
    {
      title: "Approved, funding in progress",
      persona: "Donor",
      demonstrates: "UNB-UI-G-002 AC-1 (headline 'Funding in progress'), G-017 AC-1 percentage display, G-016 reference with copy, Request cancellation dialog",
      body: (p) => `${scope}${header(p, { lozenge: loz("attention", "Funding in progress"), actions: cancelDialog(p) })}
${grantProgress(`${p}-gp`, dims([
  { name: "Funding", state: "attention", label: "Funding planned", iso: "2026-11-24T09:31:00-06:00", shown: "24 Nov 2026, 9:31 AM CT" },
  { name: "Payment", state: null, label: "Not started" },
]))}
${charity(p)}${amountPurpose(p)}
<section class="forma-stack" aria-labelledby="${p}-tl"><h4 id="${p}-tl">Timeline</h4>${timeline("Grant 2026-004817 history", [
  { iso: "2026-11-24T09:31:00-06:00", shown: "24 Nov 2026, 9:31 AM CT", state: "attention", label: "Funding planned", title: "Sale of $12,345.67 from the Balanced Pool scheduled", source: "Source: investment operations" },
  { iso: "2026-11-24T09:30:00-06:00", shown: "24 Nov 2026, 9:30 AM CT", state: "ok", label: "Approved", title: "Approved by Example Charitable Foundation", body: "Amount fixed at $12,345.67 (10% of $123,456.70).", source: "Source: grants approval" },
  { iso: "2026-11-20T10:05:00-06:00", shown: "20 Nov 2026, 10:05 AM CT", state: "ok", label: "Eligible", title: "Charity eligibility confirmed", source: "Source: IRS Business Master File, 18 Nov 2026" },
  { iso: "2026-11-20T10:02:00-06:00", shown: "20 Nov 2026, 10:02 AM CT", state: "ok", label: "Submitted", title: "Recommendation submitted by Avery Lindqvist" },
])}</section>${docs(p)}`,
    },
    {
      title: "Awaiting approval (withdraw allowed)",
      persona: "Donor",
      demonstrates: "Actions allowed in the current state only: Withdraw before approval, with G-005 dialog",
      body: (p) => `${scope}${header(p, { title: "Grant 2026-004821", lozenge: loz("attention", "Waiting on sponsor approval"), sub: "Hope Foundation, Springfield, IL · $5,000.00", actions: withdrawDialog(p) })}
${grantProgress(`${p}-gp`, [
  { name: "Recommendation", state: "ok", label: "Submitted", iso: "2026-12-01T17:02:00-06:00", shown: "1 Dec 2026, 5:02 PM CT" },
  { name: "Eligibility", state: "ok", label: "Eligible", iso: "2026-12-01T17:03:00-06:00", shown: "1 Dec 2026, 5:03 PM CT" },
  { name: "Approval", state: "attention", label: "Waiting on sponsor", iso: "2026-12-01T17:03:00-06:00", shown: "1 Dec 2026, 5:03 PM CT" },
  { name: "Funding", state: null, label: "Not started" },
  { name: "Payment", state: null, label: "Not started" },
])}`,
    },
    {
      title: "Payment outcome unknown",
      persona: "Donor",
      demonstrates: "UNB-UI-G-002 AC-2 and UNB-UI-G-001 AC-1 (headline 'Payment outcome unknown'; 'Paid' shown nowhere)",
      body: (p) => `${scope}${header(p, { lozenge: loz("unknown", "Payment outcome unknown") })}
${grantProgress(`${p}-gp`, dims([
  { name: "Funding", state: "ok", label: "Funded", iso: "2026-11-25T15:00:00-06:00", shown: "25 Nov 2026, 3:00 PM CT" },
  { name: "Payment", state: "unknown", label: "Payment outcome unknown", iso: "2026-11-27T11:40:00-06:00", shown: "27 Nov 2026, 11:40 AM CT" },
]))}
${opStatus({ id: `${p}-pu`, state: "unknown", label: "Payment outcome unknown", title: "The bank has not confirmed this payment", body: "<p>The ACH payment was sent to the bank on 27 Nov 2026 but no confirmation or return has been received. Example Charitable Foundation is reconciling with the bank. No action is needed from you; it will not be sent twice.</p>" })}`,
    },
    {
      title: "Check voided after 180 days and re-credited",
      persona: "Donor",
      demonstrates: "UNB-UI-S-004 AC-1 (timeline Issued → Outstanding → Voided → Re-credited with dates and amounts)",
      body: (p) => `${scope}${header(p, { title: "Grant 2026-001204", lozenge: loz("blocked", "Check voided; amount re-credited"), sub: "Northgate Youth Orchestra · $1,000.00 by check" })}
${timeline("Grant 2026-001204 payment history", [
  { iso: "2026-11-30T08:00:00-06:00", shown: "30 Nov 2026", state: "ok", label: "Re-credited", title: "$1,000.00 re-credited to Available to grant", source: "Source: general ledger entry JE-2026-118204" },
  { iso: "2026-11-29T08:00:00-06:00", shown: "29 Nov 2026", state: "blocked", label: "Voided", title: "Check 004417 for $1,000.00 voided", body: "Not cashed within 180 days of issue." },
  { iso: "2026-06-02T08:00:00-06:00", shown: "2 Jun 2026", state: "attention", label: "Outstanding", title: "Check 004417 for $1,000.00 outstanding", body: "Mailed to the charity; not yet cashed." },
  { iso: "2026-06-01T08:00:00-06:00", shown: "1 Jun 2026", state: "ok", label: "Issued", title: "Check 004417 for $1,000.00 issued" },
])}`,
    },
    {
      title: "Returned by the charity's bank",
      persona: "Donor",
      demonstrates: "Returned payment state with reason and next step",
      body: (p) => `${scope}${header(p, { title: "Grant 2026-003377", lozenge: loz("blocked", "Payment returned"), sub: "Lakeview Arts Collective · $750.00 by ACH" })}
${opStatus({ id: `${p}-ret`, state: "blocked", label: "Payment returned", symbol: "×", title: "The charity's bank returned this payment", body: "<p>Returned on 18 Nov 2026: account closed. $750.00 was re-credited to Available to grant on 19 Nov 2026. Example Charitable Foundation is asking the charity for a verified new account.</p>" })}`,
    },
    {
      title: "Blocked by a confirmed sanctions match",
      persona: "Donor",
      demonstrates: "UNB-UI-S-004 AC-2 ('This grant cannot be paid', blocked, no screening details)",
      body: (p) => `${scope}${header(p, { title: "Grant 2026-004102", lozenge: loz("blocked", "Cannot be paid"), sub: "Recipient organization · $2,000.00", more: "" })}
${opStatus({ id: `${p}-blk`, state: "blocked", label: "Cannot be paid", symbol: "×", title: "This grant cannot be paid", body: "<p>Example Charitable Foundation cannot pay this grant under its compliance obligations. The amount remains reserved while your sponsor completes its process; they will contact you about returning it to Available to grant.</p>", actions: ['<button type="button">Contact your sponsor</button>'] })}`,
    },
  ],
  coverage: [
    { requirement_id: "UNB-UI-S-004", acceptance_id: "AC-1", satisfied_by_markup: true, note: "State 4 timeline lists Issued, Outstanding, Voided, Re-credited with dates and $1,000.00 amounts (newest first)." },
    { requirement_id: "UNB-UI-S-004", acceptance_id: "AC-2", satisfied_by_markup: "needs-application-behaviour", note: "State 6 shows 'This grant cannot be paid' (blocked) with no screening details. Ensuring no screening data reaches the client is server behaviour." },
    { requirement_id: "UNB-UI-G-002", acceptance_id: "AC-1", satisfied_by_markup: "needs-application-behaviour", note: "State 1 headline 'Funding in progress' (attention). The deterministic derivation is application logic; Forma has no multi-dimension status primitive (FORMA-GAP-09)." },
    { requirement_id: "UNB-UI-G-002", acceptance_id: "AC-2", satisfied_by_markup: "needs-application-behaviour", note: "State 3 headline 'Payment outcome unknown' (unknown) while Funding = Funded." },
    { requirement_id: "UNB-UI-G-001", acceptance_id: "AC-1", satisfied_by_markup: true, note: "State 3: 'Payment outcome unknown' data-state=unknown; the word Paid does not appear in that state." },
    { requirement_id: "UNB-UI-G-016", acceptance_id: "AC-1", satisfied_by_markup: "needs-application-behaviour", note: "Reference 2026-004817 in forma-identifier with a Copy button; copying is application behaviour (Forma 'copyable field' is planned P1). Documents are Folio scope." },
    { requirement_id: "UNB-UI-G-017", acceptance_id: "AC-1", satisfied_by_markup: true, note: "Text '10% of fund value at approval → $12,345.67 (fixed 24 Nov 2026)'." },
    { requirement_id: "UNB-UI-G-018", acceptance_id: "AC-1", satisfied_by_markup: "needs-application-behaviour", note: "Legal dates shown as absolute dates in CT with UTC in details; time-zone conversion is application behaviour." },
  ],
  gaps: ["FORMA-GAP-01", "FORMA-GAP-06", "FORMA-GAP-07", "FORMA-GAP-09"],
};
