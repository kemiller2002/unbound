# Cross-industry discovery: customer service (D), insurance claims (E), healthcare administration (F), legal/BPO/fraud cases (M partial) — codes SVC, CLM

Status: proposed, unvalidated. Date: 2026-11-23. Outputs:

| Record type | Count |
|---|---|
| Atlas rows | 60 |
| Findings | 23 (1 critical, 9 high) |
| New requirements | 17 |
| Refinements | 4 (RR-REQ-X-048-r2, RR-ADM-033-r1, RR-ADM-042-r1, RR-CTL-040-r1) |
| Scenarios | 26 |
| Rejections | 15 |
| Failure modes | 20 |
| Decisions | 10 |
| Conflicts | 3 |

## 1. Independent assessments

### D. Call centers

Shared state plus proactive events already remove the largest contact class, "where is my grant" (daf-landscape:research/…/H-donor-services.md:43).

What still transfers:
- assurance by channel, against social engineering (TVF-132);
- recognizing a complaint whatever words the customer uses;
- precise clock definitions (start, pause, stop, calendar);
- tracking of commitments made to customers;
- call-recording law.

What does not transfer is the scale machinery: ACD, wallboards, barge/whisper, dialers, voice biometrics and per-agent handle-time ranking. DAF volume is about 0.3 human contacts per account per year (an assumption, daf-landscape financial-and-staffing-models.md:74), and no sponsor publishes data on why donors contact them.

| Coverage | Capabilities |
|---|---|
| Covered | IVR (REQ-X-025, deferred under RCD-008); escalation; disputes; agent assist; proactive notifications |
| Partial | Messaging; complaint management; SLAs; quality assurance; metrics; workforce management |
| Missing | Contact record; contact/case boundary; recording consent; transcript governance; a governed answer corpus for donors; commitments |

### E. Insurance claims

| Coverage | Capabilities |
|---|---|
| Covered | Intake; eligibility; payment authority; reconstruction of a decision as of its date (TVR-DATA-001, ADV-015); recovery |
| Partial | Information requests; notices giving reasons; fraud investigation unit; appeals |
| Contradictory | Reopening: SM-0 (decisions are immutable) conflicts with UNB-GEX-023 and EXC-18. Resolution: a decision is never reopened, it gets a child record; a work case may reopen within a window. |
| Missing | Leakage audit |

Where the analogy breaks: a claimant has an entitlement, but a donor does not, because the sponsor holds exclusive legal control (§4966(d)(2)). Reconsideration in a DAF is a courtesy, not a statutory right.

Does claims adjudication improve DAF processes?
- **Grant review:** yes. Information-request deadlines, stated reasons, independent reconsideration and leakage sampling all apply.
- **Charity vetting:** yes. Reasons and reconsideration belong on the shared vetting record, which stays separate from the sponsor's grant decision.
- **Contribution acceptance:** yes, for information requests and stated reasons.
- **Exception handling:** marginal. REQ-X-041 already matches.

### F. Healthcare administration (non-clinical)

- **Decision clocks.** The CMS interoperability and prior-authorization rule (CMS-0057-F) sets 72 hours for expedited and 7 calendar days for standard decisions. It also requires specific denial reasons, public metrics from 2026 and a Prior Authorization API by 2027. These serve as the model for decision clocks (RR-ADM-033-r1). They would be voluntary for a sponsor.
- **Eligibility reuse.** Reuse eligibility evidence already held before asking again (AIN-007, plus the check in UNB-XIN-CLM-003).
- **NPPES as an analogy.** A provider identifier is not a credential, just as an entry in the IRS master file does not prove eligibility (UNB-CHAR-016).
- **Representatives.** Personal-representative scope, and refusal of a representative where abuse is suspected (UNB-XIN-CLM-006).
- **Access.** Minimum necessary access and audit trails.

The lesson: verify once, reuse while fresh, ask only for what changed, and measure handoffs.

### M (partial). Legal, BPO and fraud cases

- **Legal:** a privileged access class, a privilege log, and legal holds that bind subjects (UNB-XIN-CLM-007, P2). Full matter management is rejected.
- **BPO:** outsourced staff are a distinct principal type, with QA per vendor and access revoked within 1 hour (UNB-XIN-CLM-008).
- **Fraud:** a restricted investigation case kind (UNB-XIN-CLM-004).

## 2. Contact vs case vs financial transaction vs exception

| Record | Owned by | Moves money? |
|---|---|---|
| Contact | Contact type | Never |
| Case | Shared case kernel plus a profile for its kind | Never. It only references transactions. |
| Financial transaction | Its domain aggregate | Yes, through gates. It is never modelled as a case. |
| Exception | A case of kind *exception*, created only by rules (a)–(d) | No. Recovery commands go to the domain. |

A contact becomes a case only when it is:
- unresolved;
- creates a commitment;
- a complaint, reconsideration or dispute; or
- a fraud report.

Separate systems for these records are rejected (XRJ-SVC-008), because they split the audit trail and contradict REQ-X-057. The answer is distinct types inside one system (UNB-XIN-SVC-001).

RR-REQ-X-048-r1 allowed a work item only for four system causes, so complaints, reconsiderations and unanswerable questions had nowhere to go (XIC-SVC-001). RR-REQ-X-048-r2 adds a fifth cause: a request promoted from a contact. Such a work item cannot close until the customer has been told the outcome.

## 3. Unified work management: argued both ways

**For:**
- one My Work list, one routing engine, one SLA engine and one audit trail;
- nothing falls between systems;
- REQ-X-057 and RR-REQ-X-057-r1 already require it.

**Against:**
- different case kinds have different closure authorities;
- the design drifts toward a generic workflow engine;
- logic specific to one kind leaks into the shared layer.

**Where a single shared lifecycle becomes harmful (XIF-SVC-001, XIC-SVC-002 against the single lifecycle in RCF-PLT-025):**

| Shared element | Harm |
|---|---|
| SLA clocks | An investigation inherits a customer clock and is pushed to close. |
| Permissions | Unified search exposes investigations and privileged notes. |
| Legal holds and retention | One retention class either over-retains every service conversation or purges matters under hold. |
| Reopen rules | Reopening rewrites decisions, which SM-0 forbids. |
| Independence | Separation of duties between reconsideration and investigation is broken. |
| Metrics | Averaging across kinds hides problems. |

**Recommendation (UNB-XIN-SVC-002, XID-SVC-001):**
- One shared case kernel: ID, kind, subjects, owner, routing, timeline, evidence, commitments, audit.
- Each kind has a profile defined in code (F# types). The profile sets the states and closure rule, clocks, access class, retention and hold binding, outcome set, independence rule, auto-close and reopen window.
- About five profiles at release 1.
- Every metric is reported per kind.

## 4. Workflow redesigns: conventional → existing proposal → AI-native

| Workflow | Conventional | Existing proposal | AI-native redesign |
|---|---|---|---|
| Grant status | Donor calls; staff look it up | Proactive events plus IVR | Proactive notice with a reason code and a portal timeline. AI answers by citing the state field and the policy clause; a human is available on request. Commitments are tracked. |
| Reconsideration | Written appeal to an independent fiduciary | A case with an owner and a response time | Notice with a reason code and a filing window. An independent reviewer takes a fresh look under current policy, against a decision clock. The original decision record is unchanged (SM-0). |
| Evidence collection | Emailed checklists and repeated requests | WaitingForEvidence state | Typed information request, checked first against evidence already held. The decision clock pauses while waiting. Reminders are sent, and expiry has defined handling. |
| Complaint | Only complaints labelled as such are logged | WF-09 | AI flags dissatisfaction and staff confirm it. Default clocks of 2 and 15 business days. Complaints from a regulator go to counsel. Closure needs a response and a root cause, which feed REQ-X-042. |

## 5. Automation boundaries

| Mode | Activities |
|---|---|
| Deterministic | Promotion of a contact to a case; clocks; routing; assurance gating; mapping to reason codes |
| AI, flag only | Detecting complaints and sentiment |
| AI with a citation check (every claim must cite a state field or policy clause; otherwise hand to a human) | Informational answers |
| AI, labelled and non-authoritative | Summaries (UNB-XIN-SVC-009) |
| AI pre-score, human decides | Quality assurance |
| Human only | Reconsideration; investigation; protective suspension of a representative; complaint closure |

**Phone disclosure.** The phone channel needs a matrix of what may be disclosed at each assurance level (UNB-XIN-SVC-004, TVF-132). Knowledge-based questions alone unlock only public information.

**Call recording (UNB-XIN-SVC-003, P0 if recording is ever enabled; XID-SVC-002).** Federal law requires one party's consent (18 USC 2511(2)(d)). Florida (§934.03(2)(d)) requires all parties' consent, and about 9–12 states are listed as all-party; counts vary by source. The caller's location is unknowable, so one uniform rule is needed. The simplest valid alternative is no recording at release 1.

## 6. Applicability by sponsor model

| Requirements | Sponsor models |
|---|---|
| UNB-XIN-SVC-001, 002, 004, 005; UNB-XIN-CLM-001..004 | All ten |
| Live-channel capacity (UNB-XIN-SVC-008) | D2C, white-label, outsourced-admin, high-volume-low-balance |
| Sticky routing to the relationship owner | White-label, advisor-distribution, institutional, investment-centric, HNW, hybrid |
| Outsourced (BPO) principals (UNB-XIN-CLM-008) | Outsourced-admin, white-label, embedded |

## 7. Cost drivers for the economics model

Parameters from sources (each labelled an assumption or scenario in its source):

| Parameter | Value | Source |
|---|---|---|
| Human contacts per account per year | 0.3 | daf-landscape financial-and-staffing-models.md:74 |
| Minutes per exception | 45 | same file, :71 |
| Loaded labour rate | $38.87–$86 per hour | TVC-020 |
| AI inference cost | $0.05 per service interaction; $0.90 per exception dossier | unbound economic-model.md:94 |
| Heavy review | $0.45 inference vs about $83 of reviewer time | daf-landscape DAF-INDUSTRY-REPORT-2026-11.md:225 |
| December concentration | Vanguard about 22% of CY2024 grants by count; Daffy about 43% of 2025 contributions | daf-landscape direction-donor-behaviour-crises.md:29 |

Illustrative unit costs at $1.00 per minute (handle times and occupancy are assumptions):

| Unit | Illustrative cost |
|---|---|
| Live phone contact | (8 + 2 min) ÷ 0.6 occupancy ≈ $16.7, plus telephony |
| Callback | ≈ $11.8 |
| Asynchronous message | ≈ $7 |
| Contact resolved by AI | < $0.50, plus fixed corpus governance |
| Manual exception | ≈ $46 |
| Human approval | ≈ $13 |
| Reconsideration | ≈ 2–3 approvals |
| Complaint from a regulator | Dominated by counsel time |
| Resolved case | Driven mainly by handoffs |

Erlang-C sizing (10-minute handle time; 80% of calls answered within 60 seconds; computed):

| Period | Calls per hour | Agents | Occupancy |
|---|---|---|---|
| Normal | 15 | 5 | 50% |
| December (3× load) | 45 | 11 | 68% |
| 30–31 December | 90 | 19 | 79% |

Small agent pools carry a cost penalty. The lever is moving live demand to callbacks or asynchronous messages.

Measurement rules (RR-ADM-042-r1):
- Handle time is measured per contact reason. It is never used unadjusted to rank individuals.
- A contact counts as deflected only if there is no human contact on the same reason within 7 days.
- Contacts are counted per 1,000 accounts, by reason.
- Handoffs are counted per case.
- Human touches are counted per outcome.

At 100,000 accounts, the contact rate moves total cost by only about 5.5% (financial-and-staffing-models.md:110). Beyond proactive notices, deflection is justified by customer experience and complaint risk, not labour savings.

## 8. Evidence not obtained

- California Penal Code §632 and Justia pages returned 403. The list of all-party consent states rests on search snippets, and counsel must verify it state by state.
- Whether AI transcription vendors create interception exposure.
- Any benchmark for channel cost, handle time or CSAT.
- That NPPES/NPI does not verify credentials (general knowledge only).
- Elder-exploitation hold laws.
- Whether TCPA applies to charities.
- Sponsor contact volumes, contact reasons and December arrival patterns (none published).
- Echelon framework case studies were not consulted.
- The defaults (90-day filing window, 30-day reconsideration target, 2- and 15-business-day complaint clocks) are proposals that need owner decisions (XID-SVC-003, XID-CLM-001).
- UNB-XIN-CLM-006 (protective suspension of a representative) has low confidence until counsel decides it (XID-CLM-002).

## Sources read (accessed 2026-11-23)

- CMS-0057-F fact sheet
- 29 CFR 2560.503-1 (Cornell LII)
- 45 CFR 164.502 (Cornell LII)
- 18 USC 2511
- Florida Statutes §934.03
- CFPB complaint-process page
- NAIC Model 900 (PDF; §4 extracted)
