# 10 — Executive findings (deliverable 10)

**Date:** 2026-11-23 to 24. **Scope:**
- the cross-industry DAF discovery mission (§§1–17);
- addendum §18, operating economics;
- addendum §19, the event-architecture hypothesis.

**Status:** Proposed. Nothing is legally reviewed, CPA-reviewed, implemented or executed. The requirements gate stays **FAIL** (reconciliation 15). This mission makes the requirement set materially stronger. It does not make it approved.

## 1. Bottom line

Unbound's requirements are **broad but shallow at the mechanics**. Other industries rarely revealed a missing *capability*. Of 368 capabilities mapped, 38 were missing and 10 contradictory (report 02).

What they revealed over and over was a missing *mechanic*. Typical examples:
- a period, payer or precedence rule;
- a reversal window;
- an authority class or sender entity;
- a default.

Without the mechanic, the requirement cannot be tested, or it cannot be operated safely.

The most consequential discoveries are legal and financial-integrity mechanics. Most of them come from banking, payments, accounting and SaaS operations, not from other DAF products.

The adversarial review found the more likely ways the platform fails:
- small accounts cannot be served profitably;
- operational and configuration overhead is too heavy for a staffing floor of 6–8 people.

Neither is a missing feature, and both now have requirements and open decisions.

## 2. The most consequential discoveries

| # | Discovery | Why it matters | Records |
|---|---|---|---|
| 1 | **Nothing constrains who holds title to charitable money.** A for-profit operator holding a float may need money-transmission licensing and become a money services business. FinCEN's payment-processor exemption covers goods and services, and a grant is neither. | Licensing and insolvency exposure. Commingling caused the Flipcause and SDG Impact Fund failures in the corpus. | UNB-XIN-REG-001, UNB-XIN-OTH-003, XRJ-BNK-005 |
| 2 | **The operator's legal status depends on the features it switches on.** Advice makes it an investment adviser. Per-transaction pay makes it a broker-dealer. Hosting solicitations can trigger California fundraising-platform registration. The sponsor's exclusions (§203(b)(4), §3(c)(10)(B)) do not extend to a vendor. | AI portfolio recommendations (UNB-INV-019) could make the operator an investment adviser. | UNB-XIN-REG-002, UNB-XIN-INV-002, RR-UNB-INV-019-r1, XID-SAS-005 |
| 3 | **Posted journals were not authoritative against re-derivation.** The idempotency key included the posting-rule version, and "deterministic rebuild" could regenerate closed years. | An auditor would see two answers. History could be re-posted. | RR-ACC-MDL-007-r1, UNB-XIN-ACC-003, -004, UNB-XIN-EVA-001 |
| 4 | **Binding payment-rail duties fall on the sponsor as Originator, payee and merchant.** These include Nacha 2026 fraud monitoring (in force since June 2026), Reg E payee authorization for recurring debits, card chargeback windows of 120 to 540 days, and Nacha return-rate thresholds. Nothing detects out-of-band bank debits. | Disclosed nonprofit fraud losses (business email compromise, ACH redirection) run from $316k to $566k per event. | UNB-XIN-BNK-001..003, UNB-XIN-PAY-001..005, RR-UNB-PAY-008-r1, RR-TVR-CON-001-r1 |
| 5 | **ASC 958 mechanics the requirements assume but never record.** Variance-power evidence underpins net assets without donor restrictions. Agency funds are liabilities. The grant-recognition point is undecided (OD-19). Schedule D, donor statements and the audit are never required to tie out (one observed $12.2m difference). | Financial statements and the Form 990 cannot be defended. | RR-UNB-ACC-033-r1, UNB-XIN-ACC-001, -002, -006 |
| 6 | **Fees have no period mechanics or payer source.** A donor-paid membership could be acknowledged as a charitable contribution. | Double charges at migration, negative balances, wrong acknowledgments. | UNB-XIN-ERP-001, -002, RR-ADM-028-r1 |
| 7 | **No workflow exists for a violation discovered after payment.** Form 4720, §4958 correction, Schedule L and board reporting are not covered. Every control is preventive. | The first real excise event is handled ad hoc. | UNB-XIN-NPG-008 |
| 8 | **Equivalency determination can now be built.** Rev. Proc. 2017-53 explicitly covers DAF sponsors, and its currency rule (§3.03(1)) is computable. A 2026 temporary regulation (§1.530A-7T) contradicts an earlier requirement. | Foreign grants can route deterministically, and a cheaper intermediary path exists. | UNB-XIN-NPG-001, -002, -007; XIC-NPG-001 |
| 9 | **One case model, but not one case lifecycle.** A shared lifecycle couples SLA clocks, access, legal holds, reopen rules and independence. Investigations had no restricted case kind. | Fraud and sanctions matters were visible in general queues and to the AI assistant. | UNB-XIN-SVC-001, -002, UNB-XIN-CLM-004, RR-REQ-X-048-r2 |
| 10 | **Partner exit with funds was undefined.** That invites an unlawful data-migration shortcut that bypasses §4966 exclusive legal control. | Program switching does happen: Ren iPhi lost 6 of 57 sponsors. | UNB-XIN-SAS-007, UNB-XIN-EVA-009 |
| 11 | **Platform-vendor staff could act outside sponsor authority** in outsourced and white-label models. Break-glass access existed only as findings. | Two operating models would be unlawful as specified. | UNB-XIN-IAM-001..005 |
| 12 | **Small accounts cannot be served profitably as specified** (adversarial review). | The high-volume/low-balance model fails, and nothing caps servicing cost per account. | XIF-ADV-001, UNB-XIN-ADV-001, XID-ADV-002 |

## 3. Addendum answers

**§18: Can reducing operating cost be a measurable, continuous process?**
- **Yes, for variable cost that can be attributed** (attribution grades A and B). It runs as a closed loop:
  - measure, attribute, detect, hypothesize, experiment;
  - verify independently, then ratchet the target;
  - with counter-metric floors that halt any initiative degrading controls, compliance or service.
- **No, for structural cost.** The staffing floor imposed by separation of duties, overhead and the December specialist pool change only through annual decisions.
- **Measurement comes first.** Every economics parameter today is an assumption.
- **Safeguards built in:**
  - cost attribution is a projection that never posts to the ledger;
  - every figure carries a confidence grade;
  - a monthly tie-out to the general ledger makes any unallocated residual explicit;
  - per-worker cost views are prohibited.
- **Scope was cut back.** The adversarial review found the 22-requirement economics stack over-engineered against its own release-1 scope of six metrics plus a monthly pack. Two requirements are recommended for rejection, and the scope is an owner decision (XID-ADV-011).
- Report: [11](11-OPERATING-ECONOMICS.md).

**§19: Is the event-driven hypothesis correct?** **Partially supported.**
- **Holds:** traceable events coordinate everything *downstream* of authoritative commits, at volumes far below where a message broker is needed.
- **Fails:**
  - **Synchronous steps.** Steps that carry invariants must be synchronous:
    - reservation;
    - hold vs release;
    - payee change;
    - sanctions block;
    - period close;
    - authority epoch;
    - conflicting instructions.
  - **"Relationship management" misstates the law.** The sponsor controls the assets, and CRM data must never authorize money.
  - **External truth arrives as reconciled batches,** not as events.
- **Chosen architecture:** a hybrid authoritative-ledger architecture.
  - One PostgreSQL database per legal sponsor.
  - The journal is the only financial log.
  - Event coordination runs through an outbox.
  - Process managers run in the database.
  - No broker, workflow engine or event store.
- **Falsification criteria** are explicit. The benchmark has since run ([18](18-ARCHITECTURE-BENCHMARK.md)): criteria 1, 2 and 5 hold at 5–10% of measured capacity, provided fund-row batches are chunked.
- Report: [12](12-EVENT-ARCHITECTURE-DECISION.md).

## 4. Most consequential contradictions

| Contradiction | Records | Status |
|---|---|---|
| Single EventStore in the persistence design vs erasure and journal authority | XIC-EVA-001, XIC-EVA-005 | Resolution recommended; architecture owner |
| RR-REQ-X-048-r1 (work items only for system causes) vs complaints and reconsiderations | XIC-SVC-001 | Superseded by RR-REQ-X-048-r2 |
| Opposite co-advisor authority defaults from two research passes | XIC-ADV-004 | Decision XID-ADV-004 |
| Journal-only rule vs sponsors whose book of record is external | XIC-ADV-001 | UNB-XIN-ADV-002; XID-ADV-003 |
| §1.530A-7T vs the natural-person class | XIC-NPG-001 | Separate class, off by default |
| No refunds after acceptance vs involuntary card chargebacks | XIC-BNK-001 | Chargeback modelled as involuntary loss, not a refund |
| Two event envelopes; two authority vocabularies | XIC-INT-001, XIC-INT-002 | Merged |

## 5. Architectural implications

- **Build:**
  - one F# component monolith;
  - one PostgreSQL database per legal sponsor, plus a platform registry database for shared charity facts and holds;
  - typed stores per authority class, with separate database privileges, including an AI-only schema;
  - outbox and in-database process managers;
  - sealed financial snapshots for reporting;
  - one tenant and correlation envelope.
- **Do not build:**
  - a message broker;
  - a workflow engine;
  - an event-sourced financial core;
  - an ERP (accounts payable, procurement, HR, budgeting);
  - an order management system;
  - a contact-centre platform;
  - customer IAM. Buy a managed identity provider.
- **New fail-closed single point of failure:** the platform hold registry. It needs its own availability objective.

## 6. Remaining uncertainties and the evidence needed to resolve them

**Update 2026-11-24:** all 41 legal and accounting questions now have provisional decisions, made at the owner's direction ([16](16-LEGAL-AND-ACCOUNTING-DECISIONS.md)). The rows below remain uncertain until the confirmations listed in report 16 are obtained. Five decisions keep their features switched off until an opinion is on file.

| Uncertainty | Evidence needed | Records |
|---|---|---|
| Operator money-transmission, investment-adviser and broker-dealer status for each operating model | Counsel opinion per feature set | XID-REG-001, XID-REG-002, XID-INV-004 |
| California AB 488 platform status for D2C and embedded models | Counsel | XID-SAS-005 |
| Grant recognition point; variance power; agency treatment | External CPA | XID-INV-001, -002, XID-ADV-006 |
| Whether high-volume/low-balance is viable | Measured cost to serve; fee-floor reasonableness | XID-ADV-002, XID-ECO-002 |
| PostgreSQL headroom at year-end peak; hot-row contention | Benchmark (ADR falsification criteria 1 and 5) | UNB-XIN-EVA-013 |
| Bank and custodian file formats that P0 controls depend on | Bank-partner and custodian specifications | XIF-ADV-020, XID-BNK-007 |
| Every cost and volume parameter | Operating measurement | economic-model.md; report 11 |
| Call recording and AI transcription in all-party-consent states | Counsel, state by state | XID-SVC-002 |
| Unclaimed-property treatment of uncashed grant checks | Counsel; state statutes | XID-BNK-003 |
| Whether agreements are needed on routine grants | Counsel confirmation | XID-NPG-006 |

## 7. Completion against mission §15

| Quality gate | Status |
|---|---|
| Every industry assessed | **Met.** A–L independently; M via brokerage/custody, fund administration, legal/BPO/fraud, tax and benefits administration, regtech, IAM, records, workflow automation, logistics, subscription billing, 529, HSA, payroll, 401(k) conversions, escrow, crowdfunding, lottery and endowments ([industry/](industry/)) |
| Existing requirements inventoried | **Met** by reuse of reconciliation 01, mapped onto the capability taxonomy (report 00) |
| Cross-industry capabilities mapped | **Met:** 368 atlas rows |
| Significant gaps identified | **Met:** 150 new requirements (after review) |
| Contradictions documented | **Met:** 38 conflicts |
| Requirements refined | **Met:** 31 refinements, each linked to its original and quoted verbatim |
| New requirements carry discovery provenance | **Met** (gate D2) |
| Legal applicability assessed | **Met, with limits:** 20 new XRA rows plus re-checks of OB rows; no counsel review |
| Automation alternatives challenged | **Met:** every atlas row records an automation class and its risks; every requirement records the simplest valid alternative |
| Failure modes investigated | **Met:** 148 catalog rows |
| UI and API implications documented | **Met** (fields enforced by gate D2) |
| Tests and acceptance criteria specified | **Met:** Given/When/Then on every record; every new item tested by a scenario (gate D7). **None executed.** |
| Duplicate requirements reconciled | **Met:** 9 duplicate sets (XIF-INT-001..009) and 3 vocabulary conflicts |
| Findings saved in the correct repository | **Met:** `unbound` only; `daf-landscape` unchanged (gate D8) |
| Repository validation checks pass | **Met:** `validate_discovery.py` 0 errors; `validate.py` (reconciliation) unchanged at 0 errors |
| Final adversarial review | **Met**, and revisions applied (report 14) |

**Remaining work is explicitly deferred, not hidden.** It needs evidence or authority the agents do not have:
- counsel review;
- CPA review;
- operating measurement;
- the PostgreSQL benchmark;
- bank and custodian specifications;
- owner decisions (86 open);
- adoption of the remaining records into the domain files (RCD-001). The 46 records that apply the legal and accounting decisions were adopted on 2026-11-24.
