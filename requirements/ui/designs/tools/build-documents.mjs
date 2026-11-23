// Builds the Unbound print-document prototypes (UNB-UI-D-001..006) from synthetic data.
// Output: ../documents/*.html. Folio elements, Folio recipe classes and semantic HTML only:
// no <style>, no style="" attributes, no consumer stylesheet.
// Usage: node build-documents.mjs
import { createHash } from "node:crypto";
import { writeFileSync, mkdirSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const here = dirname(fileURLToPath(import.meta.url));
const outDir = join(here, "..", "documents");

// ---------- pure helpers ----------
const esc = s => String(s)
  .replaceAll("&", "&amp;").replaceAll("<", "&lt;").replaceAll(">", "&gt;").replaceAll('"', "&quot;");
const sha256 = s => createHash("sha256").update(s).digest("hex");
const cents = n => Math.round(n * 100);
const usd = c => (c / 100).toLocaleString("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2 });
// G-017: negative values carry a text label as well as a sign.
const signedUsd = (c, up = "increase", down = "decrease") => c < 0 ? `−${usd(-c)} (${down})` : `${usd(c)} (${up})`;
const MONTHS = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
// G-018: absolute dates, sponsor time zone (America/Chicago). Input is an ISO local date.
const absDate = iso => { const [y, m, d] = iso.split("-").map(Number); return `${d} ${MONTHS[m - 1]} ${y}`; };
const sum = xs => xs.reduce((a, b) => a + b, 0);

// Deterministic PRNG (mulberry32) so every build yields identical synthetic data.
const prng = seed => () => {
  seed |= 0; seed = (seed + 0x6d2b79f5) | 0;
  let t = Math.imul(seed ^ (seed >>> 15), 1 | seed);
  t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
  return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
};
const pick = (rand, xs) => xs[Math.floor(rand() * xs.length)];

// ---------- sponsor constants (G-014, G-007) ----------
const SPONSOR = {
  brand: "Harbor Giving",
  legalName: "Example Charitable Foundation",
  ein: "00-0000000",
  street: "1200 Example Avenue, Suite 400",
  city: "Springfield, IL 62701",
  tz: "America/Chicago",
  verifyHost: "verify.harborgiving.example",
};
const PROTECTED = {
  exclusiveControl: "Example Charitable Foundation, as sponsoring organization of this donor advised fund, has exclusive legal control over the contributed assets.",
  noGoods: "No goods or services were provided in exchange for this contribution.",
  disclosure: `${SPONSOR.brand} is a donor advised fund program of ${SPONSOR.legalName} (EIN ${SPONSOR.ein}), the legal sponsor. ${SPONSOR.legalName} holds exclusive legal control over all contributed assets.`,
};

// ---------- shared document furniture ----------
const head = (title, description) => `<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <meta name="description" content="${esc(description)}">
  <title>${esc(title)}</title>
  <link rel="stylesheet" href="../vendor/folio-0.4.2/src/styles/print.css">
  <script type="module" src="../vendor/folio-0.4.2/src/components/register.js"></script>
</head>`;

const runningHeader = (left, center, ref) => `  <ef-print-header repeat="page">
    <span slot="left">${esc(left)}</span>
    <span slot="center">${esc(center)}</span>
    <span slot="right">Reference ${esc(ref)}</span>
  </ef-print-header>`;

// FOLIO-GAP-02 workaround: reference + verification code as text in the footer.
// ef-print-page-number carries authored fallback text; Folio computes no counters (P0).
const runningFooter = ({ ref, code, donorFacing = true }) => `  <ef-print-footer repeat="page">
    <span slot="left">Reference ${esc(ref)}</span>
    <span slot="center">Verify at https://${SPONSOR.verifyHost}/d/${esc(ref)} &middot; code ${esc(code)}</span>
    <span slot="right"><ef-print-page-number format="page-of-pages">Page numbers: renderer-supplied (P1/P2)</ef-print-page-number></span>
${donorFacing ? `    <p>${esc(PROTECTED.disclosure)}</p>\n` : ""}  </ef-print-footer>`;

const letterhead = () => `      <div class="ef-identity">
        <p><strong>${esc(SPONSOR.brand)}</strong><br>${esc(SPONSOR.legalName)}</p>
        <address>
          <span>${esc(SPONSOR.street)}</span>
          <span>${esc(SPONSOR.city)}</span>
          <span>EIN ${esc(SPONSOR.ein)}</span>
        </address>
      </div>`;

const metaList = rows => `<dl class="ef-meta-list">
${rows.map(([k, v]) => `        <div><dt>${esc(k)}</dt><dd>${v}</dd></div>`).join("\n")}
      </dl>`;

const metric = ({ label, value, detail, strong = false }) => `<li><ef-print-metric${strong ? ' emphasis="strong"' : ""}>
            <span data-label>${esc(label)}</span>
            <strong data-value>${esc(value)}</strong>
            ${detail ? `<span data-detail>${esc(detail)}</span>` : ""}
          </ef-print-metric></li>`;

const metricGrid = metrics => `<ef-print-keep>
        <ul class="ef-category-grid">
          ${metrics.map(metric).join("\n          ")}
        </ul>
      </ef-print-keep>`;

// cols: [{h, num?}], rows: arrays of already-escaped cell HTML; first column is a row header.
const table = ({ caption, cols, rows, foot }) => `<ef-print-table>
        <table>
          <caption>${esc(caption)}</caption>
          <thead><tr>${cols.map(c => `<th scope="col"${c.num ? ' data-align="number"' : ""}>${esc(c.h)}</th>`).join("")}</tr></thead>
          <tbody>
${rows.map(r => `            <tr>${r.map((cell, i) => i === 0
  ? `<th scope="row">${cell}</th>`
  : `<td${cols[i].num ? ' data-align="number"' : ""}>${cell}</td>`).join("")}</tr>`).join("\n")}
          </tbody>${foot ? `
          <tfoot><tr>${foot.map((cell, i) => i === 0 ? `<th scope="row">${cell}</th>` : `<td${cols[i].num ? ' data-align="number"' : ""}>${cell}</td>`).join("")}</tr></tfoot>` : ""}
        </table>
      </ef-print-table>`;

const page = ({ title, description, body }) => `${head(title, description)}
<body>
${body}
</body>
</html>
`;

// ---------- D-001 contribution acknowledgment ----------
const acknowledgment = ({ variant }) => {
  const ref = variant === "cash" ? "2026-101942" : "2026-101957";
  const docRef = variant === "cash" ? "2026-880114" : "2026-880121";
  const code = variant === "cash" ? "7K4P-9QX2-M3RT" : "4HWD-2LZN-8C6V";
  const giftDate = variant === "cash" ? "2026-11-16" : "2026-11-18";
  const giftTable = variant === "cash"
    ? table({
      caption: "Contribution received",
      cols: [{ h: "Contribution reference" }, { h: "Date of gift" }, { h: "Method" }, { h: "Amount", num: true }],
      rows: [[ref, absDate(giftDate), "ACH transfer", usd(cents(25000))]],
    })
    : table({
      caption: "Contribution received (noncash property)",
      cols: [{ h: "Contribution reference" }, { h: "Date of gift" }, { h: "Description of property" }],
      rows: [[ref, absDate(giftDate), "600 shares of Northwind Analytics, Inc. common stock (fictional issuer)"]],
    });
  const valueNote = variant === "cash" ? "" : " This acknowledgment describes the property received and states no value; valuing noncash property is the donor's responsibility.";
  const title = `Contribution acknowledgment ${ref}`;
  return page({
    title,
    description: `UNB-UI-D-001 prototype (${variant}) composed only from Folio 0.4.2 primitives.`,
    body: `<ef-print-document data-document-kind="contribution-acknowledgment" data-requirement="UNB-UI-D-001">
${runningHeader(SPONSOR.brand, "Contribution acknowledgment", docRef)}
  <main class="ef-dense">
    <ef-print-section>
${letterhead()}
      <p>${absDate("2026-11-20")}</p>
      <address>
        <span>Ms. Elena Rivera</span><br>
        <span>48 Lakeview Terrace</span><br>
        <span>Madison, WI 53703</span>
      </address>
      <h1>Contribution acknowledgment</h1>
      <p>Dear Ms. Rivera,</p>
      <p>Thank you for your contribution to ${esc(SPONSOR.brand)}, received by ${esc(SPONSOR.legalName)} (EIN ${esc(SPONSOR.ein)}) for The Rivera Family Fund (fund 100482).${valueNote}</p>
      ${giftTable}
      <ef-print-callout variant="plain">
        <p data-protected-text="goods-or-services">${esc(PROTECTED.noGoods)}</p>
        <p data-protected-text="exclusive-legal-control">${esc(PROTECTED.exclusiveControl)}</p>
      </ef-print-callout>
      <p>Sincerely,</p>
      <p>Jordan Okafor<br>Director of Donor Services, ${esc(SPONSOR.legalName)}</p>
    </ef-print-section>
  </main>
${runningFooter({ ref: docRef, code })}
</ef-print-document>`,
  });
};

// ---------- synthetic fund data for D-002 / D-003 ----------
const CHARITIES = [
  ["Lakeshore Food Pantry", "36-1000001"], ["Prairie Literacy Alliance", "36-1000002"], ["Riverside Youth Orchestra", "39-1000003"],
  ["Northwoods Land Trust", "39-1000004"], ["Madison Free Clinic", "39-1000005"], ["Harbor Animal Rescue", "36-1000006"],
  ["Coulee Region Habitat Builders", "39-1000007"], ["Great Lakes Science Center", "34-1000008"], ["Second Chance Reentry Network", "36-1000009"],
  ["Fox Valley Hospice", "39-1000010"], ["Midwest Refugee Partnership", "36-1000011"], ["Dane County Arts League", "39-1000012"],
];
const PURPOSES = ["General operating support", "Youth programs", "Capital campaign", "Scholarship fund", "Emergency relief", "Conservation easement program", "Annual fund"];
const RECOGNITION = ["The Rivera Family", "Elena and Marco Rivera", "Anonymous", "In honor of Rosa Rivera"];

const fundData = () => {
  const rand = prng(482);
  const grants = Array.from({ length: 72 }, (_, i) => {
    const day = 1 + Math.floor((i / 72) * 92);
    const date = new Date(Date.UTC(2026, 6, 1 + day - 1));
    const iso = date.toISOString().slice(0, 10);
    const [name] = pick(rand, CHARITIES);
    const amount = cents(100 + Math.floor(rand() * 4900) + Math.floor(rand() * 100) / 100);
    return { ref: `2026-00${String(4700 + i * 3).padStart(4, "0")}`, iso, name, purpose: pick(rand, PURPOSES), amount, recognition: pick(rand, RECOGNITION) };
  });
  // Ensure the G-016 example grant 2026-004817 is present and paid with the D-003 batch.
  grants[39] = { ...grants[39], ref: "2026-004817", name: "Lakeshore Food Pantry", purpose: "Emergency relief", amount: cents(2500) };
  const contributions = [
    ["2026-101511", "2026-07-02", "ACH transfer", cents(10000)],
    ["2026-101588", "2026-07-21", "Check", cents(2500)],
    ["2026-101640", "2026-08-05", "ACH transfer", cents(10000)],
    ["2026-101702", "2026-08-30", "Wire transfer", cents(50000)],
    ["2026-101733", "2026-09-04", "ACH transfer", cents(10000)],
    ["2026-101790", "2026-09-22", "Check", cents(1750.5)],
  ].map(([ref, iso, method, amount]) => ({ ref, iso, method, amount }));
  const fees = [
    ["2026-07-31", cents(1048.17)], ["2026-08-31", cents(1101.92)], ["2026-09-30", cents(1123.08)],
  ].map(([iso, amount]) => ({ iso, amount, basis: "0.85% annual rate on average daily balance, accrued monthly" }));
  const pending = [
    ["2026-004936", "Northwoods Land Trust", "Conservation easement program", cents(5000), "Due diligence"],
    ["2026-004941", "Madison Free Clinic", "General operating support", cents(1200), "Awaiting approval"],
    ["2026-004947", "Prairie Literacy Alliance", "Youth programs", cents(750), "Approved, payment scheduled 2 Oct 2026"],
    ["2026-004952", "Harbor Animal Rescue", "Annual fund", cents(300), "Awaiting approval"],
    ["2026-004958", "Great Lakes Science Center", "Scholarship fund", cents(2000), "Due diligence"],
  ].map(([ref, name, purpose, amount, status]) => ({ ref, name, purpose, amount, status }));
  const opening = cents(1284310.42);
  const investment = cents(-4210.33);
  const totalContrib = sum(contributions.map(c => c.amount));
  const totalGrants = sum(grants.map(g => g.amount));
  const totalFees = sum(fees.map(f => f.amount));
  const closing = opening + totalContrib - totalGrants - totalFees + investment;
  const pendingTotal = sum(pending.map(p => p.amount));
  return { grants, contributions, fees, pending, opening, investment, totalContrib, totalGrants, totalFees, closing, available: closing - pendingTotal, pendingTotal };
};

// ---------- D-002 fund statement ----------
const fundStatement = () => {
  const f = fundData();
  const ref = "2026-310482";
  return page({
    title: `Fund statement ${ref}`,
    description: "UNB-UI-D-002 prototype: quarterly fund statement with 72 grant rows, composed only from Folio 0.4.2 primitives.",
    body: `<ef-print-document data-document-kind="fund-statement" data-requirement="UNB-UI-D-002">
  <ef-print-title-page>
${letterhead()}
    <h1>Fund statement</h1>
    <p>The Rivera Family Fund &middot; fund 100482</p>
    ${metaList([
      ["Statement period", `${absDate("2026-07-01")} to ${absDate("2026-09-30")}`],
      ["Statement reference", ref],
      ["Advisors", "Elena Rivera, Marco Rivera"],
      ["Generated from", "Locked period snapshot 2026-Q3 (locked 3 Oct 2026)"],
      ["Time zone", `${SPONSOR.tz}`],
    ])}
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
${runningHeader(SPONSOR.brand, "Fund statement, 1 Jul 2026 to 30 Sep 2026", ref)}
  <main class="ef-dense">
    <ef-print-section id="summary">
      <h2>Period summary</h2>
      ${metricGrid([
        { label: "Opening value", value: usd(f.opening), detail: `at ${absDate("2026-07-01")}` },
        { label: "Contributions", value: usd(f.totalContrib), detail: `${f.contributions.length} contributions` },
        { label: "Grants paid", value: usd(f.totalGrants), detail: `${f.grants.length} grants` },
        { label: "Fees", value: usd(f.totalFees), detail: "Administrative fee" },
        { label: "Investment change", value: signedUsd(f.investment), detail: "Net of fund expenses" },
        { label: "Closing value", value: usd(f.closing), detail: `at ${absDate("2026-09-30")}`, strong: true },
        { label: "Available to grant", value: usd(f.available), detail: `Closing value less ${usd(f.pendingTotal)} pending` , strong: true },
      ])}
    </ef-print-section>
    <ef-print-section id="contributions">
      <h2>Contributions</h2>
      ${table({
        caption: "Contributions received in the period",
        cols: [{ h: "Contribution reference" }, { h: "Date of gift" }, { h: "Method" }, { h: "Amount", num: true }],
        rows: f.contributions.map(c => [c.ref, absDate(c.iso), c.method, usd(c.amount)]),
        foot: ["Total contributions", "", "", usd(f.totalContrib)],
      })}
    </ef-print-section>
    <ef-print-section id="grants">
      <h2>Grants paid</h2>
      ${table({
        caption: "Grants paid in the period",
        cols: [{ h: "Grant reference" }, { h: "Paid on" }, { h: "Charity" }, { h: "Purpose" }, { h: "Amount", num: true }],
        rows: f.grants.map(g => [g.ref, absDate(g.iso), esc(g.name), esc(g.purpose), usd(g.amount)]),
        foot: ["Total grants paid", "", "", "", usd(f.totalGrants)],
      })}
    </ef-print-section>
    <ef-print-section id="pending">
      <h2>Pending grants</h2>
      ${table({
        caption: "Grants recommended but not yet paid at period end",
        cols: [{ h: "Pending reference" }, { h: "Charity" }, { h: "Purpose" }, { h: "Status" }, { h: "Amount", num: true }],
        rows: f.pending.map(p => [p.ref, esc(p.name), esc(p.purpose), esc(p.status), usd(p.amount)]),
        foot: ["Total pending", "", "", "", usd(f.pendingTotal)],
      })}
    </ef-print-section>
    <ef-print-section id="fees">
      <h2>Fees and fee basis</h2>
      ${table({
        caption: "Administrative fees charged",
        cols: [{ h: "Charged on" }, { h: "Basis" }, { h: "Amount", num: true }],
        rows: f.fees.map(x => [absDate(x.iso), esc(x.basis), usd(x.amount)]),
        foot: ["Total fees", "", usd(f.totalFees)],
      })}
      <ef-print-note role="note"><p>Fee basis: 0.85% annual rate on the average daily balance, charged monthly in arrears per the ${esc(SPONSOR.brand)} fee schedule effective ${absDate("2026-01-01")}.</p></ef-print-note>
    </ef-print-section>
    <ef-print-section id="disclosures">
      <h2>Disclosures</h2>
      <p>Values are stated in US dollars. Investment change is net of underlying fund expenses and is not a guarantee of future value. Grants are made by ${esc(SPONSOR.legalName)} on the advice of the fund advisors; advice is not binding.</p>
      <ef-print-callout variant="plain">
        <p data-protected-text="exclusive-legal-control">${esc(PROTECTED.exclusiveControl)}</p>
        <p data-protected-text="legal-sponsor-disclosure">${esc(PROTECTED.disclosure)}</p>
      </ef-print-callout>
    </ef-print-section>
  </main>
${runningFooter({ ref, code: "9TQM-3BX7-KD2F" })}
</ef-print-document>`,
  });
};

// ---------- D-003 grant remittance + donor confirmation ----------
const remittance = () => {
  const batch = [
    { ref: "2026-004817", fund: "The Rivera Family Fund", recognition: "The Rivera Family", purpose: "Emergency relief", amount: cents(2500) },
    { ref: "2026-004822", fund: "Okonkwo Charitable Fund", recognition: "Anonymous", purpose: "General operating support", amount: cents(10000) },
    { ref: "2026-004829", fund: "Lindqvist Family Fund", recognition: "In memory of Karin Lindqvist", purpose: "Youth programs", amount: cents(1250.75) },
    { ref: "2026-004831", fund: "Chen-Alvarez Giving Fund", recognition: "Mei Chen and Luis Alvarez", purpose: "Food distribution, winter 2026", amount: cents(500) },
  ];
  const total = sum(batch.map(b => b.amount));
  const remitRef = "2026-550207";
  const confirmRef = "2026-880167";
  return page({
    title: `Grant remittance advice ${remitRef}`,
    description: "UNB-UI-D-003 prototype: charity remittance advice for several grants plus donor grant confirmation, composed only from Folio 0.4.2 primitives.",
    body: `<ef-print-document data-document-kind="grant-remittance" data-requirement="UNB-UI-D-003">
${runningHeader(SPONSOR.brand, "Grant remittance advice", remitRef)}
  <main class="ef-dense">
    <ef-print-section>
${letterhead()}
      <p>${absDate("2026-08-19")}</p>
      <address>
        <span>Lakeshore Food Pantry</span><br>
        <span>Attn: Finance Office</span><br>
        <span>310 Harbor Street</span><br>
        <span>Milwaukee, WI 53202</span>
      </address>
      <h1>Grant remittance advice</h1>
      ${metaList([
        ["Payee", "Lakeshore Food Pantry (EIN 36-1000001)"],
        ["Remittance reference", remitRef],
        ["Payment method", "ACH credit to account ending 4471"],
        ["Payment date", `${absDate("2026-08-19")} (${SPONSOR.tz}; 2026-08-19T14:00:00Z UTC)`],
        ["Total paid", usd(total)],
      ])}
      <p>${esc(SPONSOR.legalName)} (EIN ${esc(SPONSOR.ein)}) is the donor of each grant below. The grants were recommended by advisors of donor advised funds sponsored by ${esc(SPONSOR.legalName)}. Recognition is shown as each advisor chose.</p>
      ${table({
        caption: "Grants included in this payment",
        cols: [{ h: "Grant reference" }, { h: "Recognition" }, { h: "Purpose" }, { h: "Amount", num: true }],
        rows: batch.map(b => [b.ref, esc(b.recognition), esc(b.purpose), usd(b.amount)]),
        foot: ["Total paid", "", "", usd(total)],
      })}
      <ef-print-note role="note"><p>Please acknowledge these grants to ${esc(SPONSOR.legalName)} as the donor. No goods or services may be provided to any advisor in exchange for these grants.</p></ef-print-note>
    </ef-print-section>
    <ef-print-section break-before="page">
${letterhead()}
      <h2>Grant confirmation for the advisor</h2>
      ${metaList([
        ["Advisor", "Elena Rivera, The Rivera Family Fund (fund 100482)"],
        ["Grant reference", "2026-004817"],
        ["Document reference", confirmRef],
        ["Legal donor", `${esc(SPONSOR.legalName)} (EIN ${esc(SPONSOR.ein)})`],
        ["Recipient", "Lakeshore Food Pantry (EIN 36-1000001)"],
        ["Amount", usd(cents(2500))],
        ["Payment method", "ACH credit"],
        ["Payment date", `${absDate("2026-08-19")} (${SPONSOR.tz}; 2026-08-19T14:00:00Z UTC)`],
        ["Purpose", "Emergency relief"],
        ["Recognition", "The Rivera Family (as you chose)"],
      ])}
      <p>${esc(SPONSOR.legalName)} made this grant on your recommendation. The grant is not a contribution by you and is not tax-deductible to you.</p>
    </ef-print-section>
  </main>
${runningFooter({ ref: remitRef, code: "2RNX-6VJH-Q8LS" })}
</ef-print-document>`,
  });
};

// ---------- D-004 annual DAF reporting package ----------
const reportingPackage = () => {
  const ref = "2026-770025";
  const snapshot = sha256("unbound-ledger-snapshot|Example Charitable Foundation|TY2025|locked 2026-02-14");
  const lines = [
    { line: "Line 1(a)", measure: "Number of donor advised funds at year end", pkg: "1,284", ledger: "1,284", diff: "0", src: "Fund register snapshot, classification register v2025.4" },
    { line: "Line 2(a)", measure: "Aggregate value of contributions to", pkg: usd(cents(48210773.18)), ledger: usd(cents(48210773.18)), diff: usd(0), src: "GL 4100 Contributions, TY2025 close" },
    { line: "Line 3(a)", measure: "Aggregate grants from", pkg: usd(cents(39874012.55)), ledger: usd(cents(39874012.55)), diff: usd(0), src: "GL 5100 Grants paid, TY2025 close" },
    { line: "Line 4(a)", measure: "Aggregate value at end of year", pkg: usd(cents(412907551.06)), ledger: usd(cents(412907551.06)), diff: usd(0), src: "GL 1200 Investments + 1010 Cash, 31 Dec 2025" },
  ];
  const classification = [
    ["Donor advised fund", "1,284", "Included in Part I column (a)"],
    ["Field-of-interest fund", "37", "Excluded (column (b), other funds)"],
    ["Designated fund", "12", "Excluded (column (b), other funds)"],
    ["Closed during year, zero balance at year end", "41", "Excluded from year-end count; activity included in lines 2(a) and 3(a)"],
  ];
  return page({
    title: `DAF reporting package ${ref}`,
    description: "UNB-UI-D-004 prototype: annual DAF reporting package supporting Form 990 Schedule D Part I, composed only from Folio 0.4.2 primitives.",
    body: `<ef-print-document data-document-kind="daf-reporting-package" data-requirement="UNB-UI-D-004">
  <ef-print-title-page>
${letterhead()}
    <h1>Annual DAF reporting package</h1>
    <p>Support for Form 990, Schedule D, Part I, column (a)</p>
    ${metaList([
      ["Legal sponsor", `${esc(SPONSOR.legalName)} (EIN ${esc(SPONSOR.ein)})`],
      ["Tax year", `2025 (${absDate("2025-01-01")} to ${absDate("2025-12-31")})`],
      ["Package reference", ref],
      ["Tax year locked", `${absDate("2026-02-14")} (${SPONSOR.tz})`],
    ])}
  </ef-print-title-page>
${runningHeader(SPONSOR.legalName, "Annual DAF reporting package, tax year 2025", ref)}
  <main class="ef-dense">
    <ef-print-section>
      <h2>Year-end figures</h2>
      ${metricGrid([
        { label: "Donor advised funds at year end", value: "1,284", detail: "Line 1(a)", strong: true },
        { label: "Aggregate contributions", value: lines[1].pkg, detail: "Line 2(a)" },
        { label: "Aggregate grants", value: lines[2].pkg, detail: "Line 3(a)" },
        { label: "Aggregate value at year end", value: lines[3].pkg, detail: "Line 4(a)" },
      ])}
      <ef-print-integrity status="complete" role="group" aria-labelledby="pkg-integrity">
        <h3 id="pkg-integrity">Source and reconciliation status</h3>
        <dl>
          <div><dt>Reconciliation</dt><dd>4 of 4 lines agree to ledger</dd></div>
          <div><dt>Classification register</dt><dd>v2025.4 (approved ${absDate("2026-01-20")})</dd></div>
          <div><dt>Ledger close</dt><dd>TY2025 close 2026-0042</dd></div>
          <div><dt>Source snapshot SHA-256</dt><dd><code>${snapshot}</code></dd></div>
        </dl>
        <p data-limitations><strong>Scope.</strong> Figures cover funds sponsored by ${esc(SPONSOR.legalName)} only. Funds administered for other sponsors are reported in their own packages.</p>
      </ef-print-integrity>
    </ef-print-section>
    <ef-print-section>
      <h2>Reconciliation to the ledger</h2>
      ${table({
        caption: "Schedule D Part I column (a) reconciliation",
        cols: [{ h: "Line" }, { h: "Measure" }, { h: "Package", num: true }, { h: "Ledger", num: true }, { h: "Difference", num: true }, { h: "Ledger source" }],
        rows: lines.map(l => [l.line, esc(l.measure), l.pkg, l.ledger, l.diff, esc(l.src)]),
      })}
      ${table({
        caption: "Fund classification at year end (classification register v2025.4)",
        cols: [{ h: "Classification" }, { h: "Funds", num: true }, { h: "Treatment" }],
        rows: classification.map(r => r.map(esc)),
      })}
    </ef-print-section>
    <ef-print-section>
      <h2>Preparation and sign-off</h2>
      <ef-print-keep>
        ${metaList([
          ["Prepared by", "Priya Natarajan, Senior Accountant"],
          ["Prepared on", `${absDate("2026-02-16")} (2026-02-16T21:14:03Z UTC)`],
          ["Reviewed and signed by", "Samuel Adeyemi, Chief Financial Officer"],
          ["Signature", "Signed electronically, signature record 2026-SIG-00318"],
          ["Signed on", `${absDate("2026-02-18")} (2026-02-18T16:02:47Z UTC)`],
        ])}
      </ef-print-keep>
    </ef-print-section>
  </main>
${runningFooter({ ref, code: "6PLC-WM4E-7TZB" })}
</ef-print-document>`,
  });
};

// ---------- D-005 case decision record ----------
const caseRecord = () => {
  const ref = "2026-000731";
  const ev = [
    ["EV-1", "OFAC SDN list entry (fictional)", "Treasury OFAC SDN feed", "2026-11-19T06:00:00Z", "Same day"],
    ["EV-2", "Payee registration record", "Unbound charity registry", "2026-11-19T13:22:41Z", "Same day"],
    ["EV-3", "IRS Pub. 78 / EO BMF lookup", "IRS EO BMF monthly file", "2026-11-12T00:00:00Z", "7 days"],
    ["EV-4", "Bank account ownership attestation", "Payee upload", "2026-11-19T15:03:10Z", "Same day"],
    ["EV-5", "Screening engine match explanation", "Sanctions screening service", "2026-11-19T13:22:45Z", "Same day"],
  ].map(([id, item, src, at, fresh]) => [id, esc(item), esc(src), `${absDate(at.slice(0, 10))} (${at} UTC)`, esc(fresh), `<code>${sha256(id + item)}</code>`]);
  return page({
    title: `Case decision record ${ref}`,
    description: "UNB-UI-D-005 prototype: sanctions case decision record, composed only from Folio 0.4.2 primitives.",
    body: `<ef-print-document data-document-kind="case-decision-record" data-requirement="UNB-UI-D-005">
${runningHeader(SPONSOR.legalName, "Case decision record (internal, audit evidence)", ref)}
  <main class="ef-dense">
    <ef-print-section>
      <h1>Case decision record: sanctions screening match</h1>
      ${metaList([
        ["Case reference", ref],
        ["Raised by rule", "SAN-OFAC-001 version 3: payee name and address match against the OFAC SDN list, score 0.92 or higher"],
        ["Subject", "Grant 2026-004903 to payee Meridian Relief Collective (fictional)"],
        ["Case opened", `${absDate("2026-11-19")} 07:22 ${SPONSOR.tz} (2026-11-19T13:22:45Z UTC)`],
        ["Decision recorded", `${absDate("2026-11-20")} 10:41 ${SPONSOR.tz} (2026-11-20T16:41:09Z UTC)`],
        ["Prepared by", "Analyst Dana Whitfield (user 4471), compliance operations"],
        ["Decided by", "Marcus Lee (user 1093), Chief Compliance Officer"],
      ])}
      <ef-print-callout>
        <p><strong>OFAC report due ${absDate("2026-12-04")}.</strong> Blocked property report due within 10 business days of blocking (blocked ${absDate("2026-11-20")}).</p>
      </ef-print-callout>
    </ef-print-section>
    <ef-print-section>
      <h2>Evidence</h2>
      ${table({
        caption: "Evidence considered, with source and freshness",
        cols: [{ h: "Item" }, { h: "Evidence" }, { h: "Source" }, { h: "Retrieved" }, { h: "Freshness" }, { h: "SHA-256" }],
        rows: ev,
      })}
    </ef-print-section>
    <ef-print-section>
      <h2>Decision</h2>
      <ef-print-finding>
        <dl>
          <div><dt>Observation</dt><dd>Payee name, street address and bank country match SDN entry (fictional) on three of three identifying attributes (EV-1, EV-2, EV-5).</dd></div>
          <div><dt>Decision</dt><dd>Confirmed match. Grant 2026-004903 payment blocked; funds moved to a blocked interest-bearing account.</dd></div>
          <div><dt>Rationale</dt><dd>Attribute agreement exceeds the confirmation threshold; payee attestation (EV-4) names the same beneficial owner. No contrary evidence.</dd></div>
          <div><dt>Follow-up</dt><dd>File blocked property report with OFAC by ${absDate("2026-12-04")}; notify the fund advisor that the grant cannot be paid, without disclosing the screening reason.</dd></div>
        </dl>
      </ef-print-finding>
      <ef-print-note role="note"><p>Separation of duties: the preparer and the decider are different people (user 4471 and user 1093).</p></ef-print-note>
    </ef-print-section>
  </main>
${runningFooter({ ref, code: "8DKS-5RWA-1MPH", donorFacing: false })}
</ef-print-document>`,
  });
};

// ---------- D-006 audit evidence package ----------
const auditPackage = () => {
  const ref = "2026-990004";
  const controls = [
    { id: "CC6.1", title: "Logical access provisioning", n: 9 },
    { id: "CC6.3", title: "Access removal on termination", n: 7 },
    { id: "CC7.2", title: "Security event monitoring", n: 8 },
    { id: "CC8.1", title: "Change management", n: 10 },
    { id: "FIN-03", title: "Grant disbursement dual approval", n: 9 },
  ];
  const rand = prng(2025);
  const evidence = controls.map(c => ({
    ...c,
    rows: Array.from({ length: c.n }, (_, i) => {
      const day = 1 + Math.floor(rand() * 91);
      const d = new Date(Date.UTC(2025, 9, day)).toISOString().slice(0, 10);
      const item = pick(rand, ["Access request ticket", "Approval record", "System log extract", "Pull request review", "Quarterly access review", "Payment approval pair", "Alert triage record"]);
      const id = `${c.id}-E${String(i + 1).padStart(2, "0")}`;
      return [id, esc(item), absDate(d), `<code>${sha256(`${ref}|${id}|${item}|${d}`)}</code>`];
    }),
  }));
  const exceptions = [
    ["EX-01", "CC6.3", "One leaver account disabled 3 business days after termination (policy: 1).", "Account had no activity after termination (log extract CC6.3-E04). Offboarding automation fixed 12 Nov 2025.", "Closed"],
    ["EX-02", "FIN-03", "One grant batch approved by two users in the same role group.", "Batch re-approved by finance controller 9 Dec 2025; role groups separated.", "Closed"],
  ];
  const manifest = sha256(JSON.stringify(evidence.map(e => e.rows)));
  return page({
    title: `Audit evidence package ${ref}`,
    description: "UNB-UI-D-006 prototype: period-bounded audit evidence package, composed only from Folio 0.4.2 primitives.",
    body: `<ef-print-document data-document-kind="audit-evidence-package" data-requirement="UNB-UI-D-006">
  <ef-print-title-page>
${letterhead()}
    <h1>Audit evidence package</h1>
    <p>Period ${absDate("2025-10-01")} to ${absDate("2025-12-31")} (Q4 2025)</p>
    ${metaList([
      ["Package reference", ref],
      ["Prepared for", "External auditor (SOC 2 Type II examination)"],
      ["Generated", `${absDate("2026-01-09")} (2026-01-09T18:30:00Z UTC)`],
      ["Evidence items", String(sum(controls.map(c => c.n)))],
    ])}
    <ef-print-integrity status="complete" role="group" aria-labelledby="manifest-h">
      <h2 id="manifest-h">Package manifest</h2>
      <dl>
        <div><dt>Controls</dt><dd>${controls.length}</dd></div>
        <div><dt>Exceptions</dt><dd>${exceptions.length} (all closed)</dd></div>
        <div><dt>Manifest SHA-256</dt><dd><code>${manifest}</code></dd></div>
      </dl>
    </ef-print-integrity>
  </ef-print-title-page>
${runningHeader(SPONSOR.legalName, "Audit evidence package, Q4 2025", ref)}
  <main class="ef-dense">
    <ef-print-section id="contents">
      <h2>Contents</h2>
      <ef-print-toc>
        <nav aria-label="Package contents">
          <ol>
            <li><a href="#controls">Control list</a></li>
${controls.map(c => `            <li><a href="#control-${c.id}">Control ${c.id}: ${esc(c.title)}</a></li>`).join("\n")}
            <li><a href="#exceptions">Exceptions and resolution</a></li>
          </ol>
        </nav>
      </ef-print-toc>
    </ef-print-section>
    <ef-print-section id="controls">
      <h2>Control list</h2>
      ${table({
        caption: "Controls in scope",
        cols: [{ h: "Control" }, { h: "Description" }, { h: "Evidence items", num: true }],
        rows: controls.map(c => [c.id, esc(c.title), String(c.n)]),
      })}
    </ef-print-section>
${evidence.map(c => `    <ef-print-section id="control-${c.id}">
      <h2>Control ${c.id}: ${esc(c.title)}</h2>
      ${table({
        caption: `Evidence for control ${c.id}`,
        cols: [{ h: "Evidence ID" }, { h: "Item" }, { h: "Dated" }, { h: "SHA-256" }],
        rows: c.rows,
      })}
    </ef-print-section>`).join("\n")}
    <ef-print-section id="exceptions">
      <h2>Exceptions and resolution</h2>
      ${table({
        caption: "Exceptions identified in the period",
        cols: [{ h: "Exception" }, { h: "Control" }, { h: "Description" }, { h: "Resolution" }, { h: "Status" }],
        rows: exceptions.map(r => r.map(esc)),
      })}
    </ef-print-section>
  </main>
${runningFooter({ ref, code: "3VFE-HN8Q-5XYA", donorFacing: false })}
</ef-print-document>`,
  });
};

// ---------- write ----------
const documents = [
  ["D-001-acknowledgment-cash.html", () => acknowledgment({ variant: "cash" })],
  ["D-001-acknowledgment-securities.html", () => acknowledgment({ variant: "securities" })],
  ["D-002-fund-statement.html", fundStatement],
  ["D-003-grant-remittance.html", remittance],
  ["D-004-daf-reporting-package.html", reportingPackage],
  ["D-005-case-decision-record.html", caseRecord],
  ["D-006-audit-evidence-package.html", auditPackage],
];

mkdirSync(outDir, { recursive: true });
documents.forEach(([name, build]) => writeFileSync(join(outDir, name), build()));
console.log(documents.map(([n]) => n).join("\n"));
