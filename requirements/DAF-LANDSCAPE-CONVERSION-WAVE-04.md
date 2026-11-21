# DAF landscape conversion, wave 4: source-level coverage register
Date: 2026-11-21 | Status: inventory converted to candidate requirement obligations, not legally verified

This register inventories **58 events, 54 controls, 65 regulatory assertions and 22 scenarios** from the source CSV files. The source's 50 entries classified as legal are *claims to verify*, not independent legal opinions. None of these rows has passed Unbound's requirements gate. The append-only architecture in wave 3 governs all authoritative records.

## Governing conversion requirements
- **UNB-TRACE-001:** Every source event below SHALL receive an Unbound event contract with immutable event ID, event/effective/recorded times, authorized producer, schema version, causation, provenance, correction/supersession semantics, idempotency, replay safety and side-effect classification. Event rows marked as money-capable require a separately verified sponsor decision and payment/trade gate.
- **UNB-TRACE-002:** Every source control below SHALL be mapped to one or more requirements, applicable regulatory assertions or documented operational risks, control owner, preventive/detective class, objective evidence and executable failure test. The source control catalog is a candidate control baseline, not proof of control operation.
- **UNB-TRACE-003:** Every REG row below SHALL be evaluated for source authority, law/guidance/proposal distinction, current status, jurisdiction, sponsor-versus-provider applicability and qualified reviewer approval. A legal assertion without a reviewed control/evidence mapping SHALL block regulatory readiness.
- **UNB-TRACE-004:** Every source scenario below SHALL be converted into a testable normal, exceptional, adversarial or recovery case with detect, contain, escalate, correct, evidence and residual risk. Correction is always append-only; simulation shall verify prior states remain reconstructible.
- **UNB-TRACE-005:** No source architecture target, financial assumption or legal interpretation is considered validated merely because a CSV row exists. Record evidence maturity and independent reproduction.

## Event inventory (58/58 identified)
Source: [events.csv](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/06-data-and-knowledge/events.csv)

| Event | Domain | Producer | Money-capable | Required Unbound treatment |
|---|---|---|---|---|
| `PartyRegistered` | party | C1.1 Party service | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `IdentityVerified` | party | C1.1 Party service (from IdP result) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `RelationshipDeclared` | party | C1.1 Party service | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `RelationshipInferred` | party | Relationship agent (flag-only) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `AccountOpened` | account | C1.1 Account service | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `AgreementAccepted` | account | C1.1 Account service | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `PrivilegeGranted` | account | C1.2 Authorization service | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `PrivilegeRevoked` | account | C1.2 Authorization service | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `DonorInstructionChanged` | account | C1.2 / C1.1 | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `ContributionIntentDeclared` | contribution | C2.1 Intake | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `ContributionReceived` | contribution | C2.1 (from custodian/bank feed) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `ContributionSettled` | contribution | C2.1 Intake | no (internal unitization only) | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `AcknowledgmentIssued` | contribution | C2.1 Intake | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `NoncashAssetDisposed` | contribution | C2.3/C2.4 | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `InvestmentRequested` | investment | C1.2-authorized donor/advisor channel | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `TradeInstructed` | investment | C2.4 Rebalancer/Mandate checker | yes (securities) | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `PositionChanged` | investment | C2.3 (from custodian) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `PolicyBandBreached` | investment | C2.4 compliance monitor | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `NavStruck` | accounting | C4.1 Valuation | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `RecommendationSubmitted` | grant | C3.2 Intake | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `RecipientResolved` | grant | C3.1 Registry | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `EligibilityEvaluated` | grant | C3.1 Eligibility | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `ScreeningCompleted` | grant | C5.2 Screening | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `GrantFlagged` | grant | Flag agents or rules | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `GrantApproved` | grant | C3.3 Decisioning (policy engine or human approver) | yes (via payment gate) | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `GrantDeclined` | grant | C3.3 | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `GrantCanceled` | grant | C3.3 (donor request before release) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `PaymentInstructed` | payment | C3.4 Payment gate | yes | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `PaymentSettled` | payment | C3.4 (from rail) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `PaymentReturned` | payment | C3.4 (from rail) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `PaymentProfileChanged` | payment | C3.1/C3.4 (recipient-initiated) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `CheckAged` | payment | C3.4 aging monitor | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `CharityEligibilityChanged` | registry | C3.1 (from IRS/OFAC diff) | no (can block) | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `SanctionsListUpdated` | registry | C5.2 (from OFAC) | no (can block) | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `ReconciliationBreakDetected` | accounting | C4.2 | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `ReconciliationBreakResolved` | accounting | C4.2 (rule) or Controller (H) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `FeeAccrued` | accounting | C4.3 Fee engine | no (internal) | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `PolicyPublished` | policy | C5.1 Policy registry | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `ComplianceObligationChanged` | policy | GC / obligation register | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `AgentPermissionChanged` | agent | C6.4 Change board | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `AgentRunCompleted` | agent | Agent runtime | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `DocumentationRequested` | service | C1.4 (donor or advisor request) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `DocumentationDelivered` | service | C4.4 | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `RegulatoryDeadlineApproaching` | compliance | C4.4 calendar | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `ExceptionCaseOpened` | operations | C5.3 | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `ExceptionCaseResolved` | operations | Specialist (H) | depends on decision | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `DonorDeceased` | account | C1.1 (documented notice) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `AccountInactivityThresholdReached` | account | C1.4 engagement monitor | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `TransferOutRequested` | account | C1.2-authorized donor | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `CustodianUnavailable` | operations | C5.5 vendor monitor | no (reduces) | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `PaymentGateDenied` | payment | C3.4 Payment gate | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `DonorConfirmationRequested` | grant | C3.2 Intake (agent-originated request above confirm_above) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `DonorConfirmationReceived` | grant | C3.2 Intake | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `ScheduledGrantDue` | grant | C3.2 scheduler (recurring grant instruction) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `KillSwitchActivated` | operations | Authorized person (CTO/CRCO/COO) | no (stops) | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `JournalPosted` | accounting | C4.1 Ledger (posting rules) | no | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `CharityEligibilityHoldApplied` | grant | C3.1 monitor | no (blocks) | Append immutable occurrence; verify authorization, provenance, correction and replay |
| `PostPaymentReviewOpened` | grant | C3.1 monitor | no | Append immutable occurrence; verify authorization, provenance, correction and replay |

## Control inventory (54/54 identified)
Source: [controls.csv](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/08-governance-and-compliance/controls.csv)

| Control | Description | Owner | Evidence and test required |
|---|---|---|---|
| CTL-001 | Request/instruction separation: only policy engine or authorized sponsor human can emit GrantApproved TradeInstructed PaymentInstructed | CTO | authorization denials log; invariant test results; architecture test: donor/advisor/agent principals cannot invoke instruction commands |
| CTL-002 | Every asset movement has a sponsor decision record | Controller | daily movement-to-decision reconciliation report; inject movement without decision in test env; expect critical break |
| CTL-003 | Payment gate: payment released only if grant Approved + payee profile verified + not in cooling-off + within limits + funds available | Controller | gate decision record per payment; property tests over gate inputs; negative scenario tests |
| CTL-004 | Payee bank-account change cooling-off with out-of-band verification and notice to prior contact | Controller | PaymentProfileChanged verification evidence; simulated hijack scenario; verify hold |
| CTL-005 | Dual control for payments above threshold and for all exception-path payments | CFO | two distinct approval records; attempt same-person double approval in test; expect deny |
| CTL-006 | Segregation: payee creator cannot approve payment to that payee; policy author cannot publish own policy; requester cannot decide own case | CRCO | SoD conflict report (expect empty); quarterly SoD matrix test + continuous conflict query |
| CTL-007 | Related-party approval block: approver cannot approve grant/case involving a party they are related to | CRCO | denial records; seeded related-party test cases |
| CTL-008 | Flag-only AI: model outputs may add flags but cannot clear flags or change authoritative state | CTO (agent owners) + CRCO | AgentRun tool-call logs; flag lifecycle (cleared_by never an agent); attempt clear via agent tool in test; red-team prompts |
| CTL-009 | Policy publish gate: back-test against historical corpus + independent test + H approval + author≠approver | General Counsel | PolicyPublished with back-test report and approvals; publish attempt without approval in test; diff review sampling |
| CTL-010 | Monotonic safety rule: any parameter change that loosens a control (higher auto-approve threshold; lower fuzzy-match sensitivity; longer freshness SLO) classified material and requires Change Board + CRCO | COO (Change Board) | change classification record; attempt loosening via ordinary change path; expect reclassification |
| CTL-011 | Eligibility data freshness SLO: stale BMF/Pub78/revocation data blocks D-path approvals and forces refer | Head of Grants Compliance | freshness check on each EligibilityEvaluated; age data past SLO in test; expect refer |
| CTL-012 | Sanctions list version pinning and full rescreen on list update | CRCO (sanctions officer) | ScreeningCompleted with list version; rescreen run report; seeded SDN names; list-update replay test |
| CTL-013 | Potential sanctions true match cleared only by sanctions officer | CRCO | approval record with evidence viewed; attempt clearance by other role; expect deny |
| CTL-014 | Continuous eligibility monitoring of payee universe; in-flight grants re-evaluated on CharityEligibilityChanged | Head of Grants Compliance | diff evidence + re-evaluation records; replay a revocation diff; expect holds |
| CTL-015 | Three-way daily reconciliation: custodian vs position mirror vs ledger; bank vs treasury ledger; rail settlements vs payment instructions | Controller | ReconciliationResult with aging; inject breaks of each class; verify detection and aging |
| CTL-016 | Break aging SLO and escalation; write-off above threshold requires second approver | Controller | break aging report; write-off approvals; aged break simulation |
| CTL-017 | NAV tolerance checks (price source agreement; day-over-day move vs benchmark) hold NAV on breach | Controller | NAV calc record with checks; price shock fixtures |
| CTL-018 | Ledger invariants: debits=credits; Σ(account units × NAV)=pool NAV; cash in transit = unsettled instructions | Controller | invariant check results; property tests; mutation tests on posting rules |
| CTL-019 | Append-only ledger; corrections only by reversing entries | Controller | hash chain verification; attempt update/delete in test; verify hash chain |
| CTL-020 | Acknowledgment content schema test incl. exclusive legal control statement and quid-pro-quo disclosure | Controller | generated document hash + validation result; golden-file and negative content tests |
| CTL-021 | Complex asset acceptance requires specialist decision and committee above threshold; excess business holdings test | CFO | acceptance decision + dossier; sampled file review |
| CTL-022 | Form 8283/8282 obligation tracking from noncash events | CFO | obligation records + filing receipts; disposal-event fixtures |
| CTL-023 | Pre-trade mandate and IPS band checks; liquidity floor check | CIO (outsourced) / Investment Committee | pre-trade check records; band-breach fixtures |
| CTL-024 | Rebalancer parameter changes require Investment Committee approval record | Investment Committee | approval linked to parameter version; attempt engineer change; expect deny |
| CTL-025 | Advisor fee cap enforcement (policy cap) and fee reasonableness report | Controller | FeeAccrued with cap check; over-cap fixture |
| CTL-026 | Agent evaluation gate: no agent version or permission expansion deployed without passing frozen eval set + red-team set + CRCO model-risk sign-off | CTO (eval lead) + CRCO | eval report linked to AgentPermissionChanged; attempt deploy with failing eval in CI |
| CTL-027 | Agent runtime monitoring: flag-rate drift; schema-failure rate; cost and latency anomalies; circuit breaker trips to human queue | Agent owner | telemetry + breaker events; inject drift in twin; verify breaker |
| CTL-028 | Kill switches: per-agent disable and enterprise payments pause; human-operated and recorded | COO + CISO | switch activation records; quarterly game day |
| CTL-029 | Money-movement fan-out caps per account / recipient / enterprise window; excess held | CRCO | cap breach holds; burst simulation in twin |
| CTL-030 | Causation-depth limit on event chains without deterministic or human checkpoint | CTO | quarantine records; synthetic loop test |
| CTL-031 | Privileged access just-in-time with approval; no standing production write or payment-signing credentials held by humans | CISO | access grant records; session logs; access review; attempt standing grant |
| CTL-032 | Payment signing keys in HSM/KMS; signing only callable by payment gate service identity | CISO | KMS audit logs; attempt sign from other identity |
| CTL-033 | Phishing-resistant MFA for workforce; risk-based step-up for donors on payout-relevant changes | CISO | auth logs; ATO simulation |
| CTL-034 | API credential scoping per partner; short-lived tokens; anomaly detection on partner traffic; automated revocation | CTO | token issuance and revocation logs; compromised-key scenario |
| CTL-035 | Vendor exit-readiness: quarterly full export of all data into open formats and restore into cold standby; contract data-rights clauses | COO | restore test report; restore drill |
| CTL-036 | DR restore tests against RPO/RTO targets | CTO | restore test evidence; timed restore |
| CTL-037 | Obligation→control coverage test: every current-law obligation in register maps to ≥1 operating control with evidence | CRCO | coverage report (expect 100%); remove mapping in test; expect failure |
| CTL-038 | Regulatory deadline calendar with T-60/T-30/T-7 escalation | CFO | RegulatoryDeadlineApproaching tasks; clock-shift test |
| CTL-039 | Grounded-answer check for service agent: every factual claim cites state field or approved clause; tax-advice classifier routes to referral | Head of Client and Partner Operations | AgentRun citations; QA sample + adversarial questions |
| CTL-040 | Service QA sampling and complaint taxonomy | Head of Client and Partner Operations | QA scores; inter-rater checks |
| CTL-041 | Conflict-of-interest register from relationship graph plus annual attestation; recusal enforcement in board tooling | Governance Committee | register + recusal records; seeded conflict test |
| CTL-042 | Related-party transaction approval by disinterested directors with documented comparables | Audit & Risk Committee | minutes + comparables; internal audit review |
| CTL-043 | Recipient attribution: remittance with grant ID and donor-chosen info delivered with each payment | Controller | remittance delivery record; remittance completeness report |
| CTL-044 | Uncashed check aging and escheat workflow per state rules | Controller | aging and filing records; aged-check fixtures |
| CTL-045 | Discretionary hold policy: holds require board-approved policy basis and recorded decision; periodic review of all active holds | CEO / Board | hold decision + review records; internal audit sample |
| CTL-046 | Reviewer seeded-error program: known-bad test cases injected into the exception queue to measure reviewer detection and counter automation complacency | CRCO | seeded-case detection rates per reviewer and per case type; trend review; detection below target triggers retraining or queue redesign |
| CTL-047 | Expiry dates on governed policy parameters (thresholds caps model cut-offs) with an age report in the board pack; expired parameters revert to the more conservative prior value | General Counsel / Board | parameter register with approval date and expiry; board-pack age report; internal audit verifies no parameter past expiry |
| CTL-048 | Monthly controller variance attestation: controller reviews ledger-to-custodian-to-bank variances and fee and NAV movements against expectations and attests | Controller / CFO | signed attestation with variance pack hash; external auditor walk-through |
| CTL-049 | Sampled human review of unflagged anomaly clusters (e.g. unusual grant patterns account behavior or payee changes that no rule flagged) | CRCO | sample selections and reviewer findings; findings rate tracked; new rules derived from findings via change control |
| CTL-050 | Cross-provider check on eligibility data ingestion: IRS bulk-file loads reconciled against an independent source sample and prior-load diff limits before publish | Head of Grants Compliance | load reconciliation report; diff-limit breaches; fault-injection test with truncated file |
| CTL-051 | Partner MFA trust: a partner's federated MFA counts only with a contractual MFA attestation and accepted amr values (RFC 8176); otherwise local step-up | CISO | federation config + per-session assurance record; conformance tests 1-3 (identity-and-partner-federation.md §6) |
| CTL-052 | SCIM deprovisioning: partner leavers lose access immediately; deprovisioning latency monitored against the federation agreement | Head of Partnerships | SCIM event log; latency report; conformance test 10; monthly latency report |
| CTL-053 | Sender-constrained tokens for partner machine clients (mTLS RFC 8705 or DPoP RFC 9449); org-scoped data; least-privilege scopes | CTO | token binding evidence; scope grants; conformance test 12; quarterly scope review |
| CTL-054 | Recovery-factor changes require fresh phishing-resistant authentication; help-desk recovery triggers cooling-off and prior-channel notice | Head of Client and Partner Operations | recovery event log; notices; conformance test 6; sampled recovery audit |

## Regulatory inventory (65/65 identified)
Source: [regulatory-applicability-matrix.csv](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/08-governance-and-compliance/regulatory-applicability-matrix.csv)

| Regulatory assertion | Domain | Source classification | Source status | Sponsor / provider | Verification |
|---|---|---|---|---|---|
| REG-001 | Federal tax (DAF) | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-002 | Federal tax (DAF) | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-003 | Federal tax (DAF) | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-004 | Federal tax (DAF) | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-005 | Federal tax (DAF) | legal | current_law | yes / conditional | Pending independent legal applicability and REG→CTL mapping |
| REG-006 | Federal tax (DAF) | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-007 | Federal tax (DAF) | risk_control | current_law | no / no | Pending independent legal applicability and REG→CTL mapping |
| REG-008 | Federal tax (DAF) | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-009 | Federal tax (substantiation) | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-010 | Federal tax (substantiation) | legal | current_law | conditional / no | Pending independent legal applicability and REG→CTL mapping |
| REG-011 | Federal tax (substantiation) | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-012 | Federal tax (substantiation) | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-013 | Federal tax (reporting) | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-014 | Federal tax (reporting) | legal | current_law | conditional / no | Pending independent legal applicability and REG→CTL mapping |
| REG-015 | Federal tax (status) | legal | current_law | yes / no | Pending independent legal applicability and REG→CTL mapping |
| REG-016 | Federal tax (guidance) | legal | guidance | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-017 | Federal tax (guidance) | risk_control | guidance | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-018 | Federal tax (guidance) | risk_control | proposed | conditional / no | Pending independent legal applicability and REG→CTL mapping |
| REG-019 | Federal tax (guidance) | legal | guidance | yes / yes | Pending independent legal applicability and REG→CTL mapping |
| REG-020 | Federal tax (guidance) | legal | guidance | conditional / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-021 | Federal tax (proposed) | risk_control | proposed | no (until final) / no (until final) | Pending independent legal applicability and REG→CTL mapping |
| REG-022 | Federal tax (proposed) | risk_control | proposed | no (watch) / no (watch) | Pending independent legal applicability and REG→CTL mapping |
| REG-023 | Federal tax (donor) | convention | current_law | no (donor-side law) / no | Pending independent legal applicability and REG→CTL mapping |
| REG-024 | Federal tax (disclosure) | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-025 | Governance | convention | current_law | conditional / no | Pending independent legal applicability and REG→CTL mapping |
| REG-026 | Records | legal | current_law | yes / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-027 | Securities | legal | current_law | yes / no | Pending independent legal applicability and REG→CTL mapping |
| REG-028 | Securities | legal | current_law | conditional / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-029 | Securities | legal | current_law | yes / no | Pending independent legal applicability and REG→CTL mapping |
| REG-030 | Securities | legal | current_law | yes / conditional | Pending independent legal applicability and REG→CTL mapping |
| REG-031 | Securities | legal | current_law | yes / conditional | Pending independent legal applicability and REG→CTL mapping |
| REG-032 | Securities | legal | current_law | yes / no | Pending independent legal applicability and REG→CTL mapping |
| REG-033 | Securities | legal | current_law | no / conditional | Pending independent legal applicability and REG→CTL mapping |
| REG-034 | Securities (proposed) | risk_control | proposed | no (until final) / no (until final) | Pending independent legal applicability and REG→CTL mapping |
| REG-035 | Securities | legal | current_law | no (if within exclusion) / conditional | Pending independent legal applicability and REG→CTL mapping |
| REG-036 | Sanctions | legal | current_law | yes / yes | Pending independent legal applicability and REG→CTL mapping |
| REG-037 | Sanctions | legal | current_law | yes / conditional | Pending independent legal applicability and REG→CTL mapping |
| REG-038 | AML | risk_control | current_law | no / conditional | Pending independent legal applicability and REG→CTL mapping |
| REG-039 | AML | convention | guidance | yes (voluntary) / no | Pending independent legal applicability and REG→CTL mapping |
| REG-040 | AML | convention | guidance | no / no | Pending independent legal applicability and REG→CTL mapping |
| REG-041 | Sanctions | legal | current_law | yes / yes | Pending independent legal applicability and REG→CTL mapping |
| REG-042 | Money movement | legal | current_law | no (inference) / see REG-043 | Pending independent legal applicability and REG→CTL mapping |
| REG-043 | Money movement | legal | current_law | no / conditional | Pending independent legal applicability and REG→CTL mapping |
| REG-044 | Money movement | legal | current_law | via contract / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-045 | Money movement | legal | current_law | conditional / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-046 | State charity / platforms | legal | current_law | conditional / conditional | Pending independent legal applicability and REG→CTL mapping |
| REG-047 | Privacy | risk_control | current_law | no (generally) / conditional | Pending independent legal applicability and REG→CTL mapping |
| REG-048 | Privacy | legal | current_law | conditional / yes (as processor) / conditional (as controller) | Pending independent legal applicability and REG→CTL mapping |
| REG-049 | Privacy | legal | current_law | yes / yes (notify the controller/sponsor by contract and statute) | Pending independent legal applicability and REG→CTL mapping |
| REG-050 | Privacy | legal | current_law | via contract / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-051 | Privacy | legal | current_law | no (unless affiliated adviser/BD) / via contract | Pending independent legal applicability and REG→CTL mapping |
| REG-052 | Privacy | legal | current_law | no / conditional | Pending independent legal applicability and REG→CTL mapping |
| REG-053 | State charity law | legal | current_law | yes / conditional (if acting as fundraising counsel/platform) | Pending independent legal applicability and REG→CTL mapping |
| REG-054 | State charity law | legal | current_law | conditional / no | Pending independent legal applicability and REG→CTL mapping |
| REG-055 | State charity law | legal | current_law | yes / no | Pending independent legal applicability and REG→CTL mapping |
| REG-056 | State charity law | convention | not_enacted | no / no | Pending independent legal applicability and REG→CTL mapping |
| REG-057 | State charity law | legal | current_law | conditional / no | Pending independent legal applicability and REG→CTL mapping |
| REG-058 | Governance | legal | current_law | yes / no | Pending independent legal applicability and REG→CTL mapping |
| REG-059 | Governance | convention | current_law | conditional / no | Pending independent legal applicability and REG→CTL mapping |
| REG-060 | AI | legal | current_law | conditional (unlikely) / conditional (as developer) | Pending independent legal applicability and REG→CTL mapping |
| REG-061 | AI | legal | current_law | no / no | Pending independent legal applicability and REG→CTL mapping |
| REG-062 | AI | legal | current_law | no / no (unless EU nexus) | Pending independent legal applicability and REG→CTL mapping |
| REG-063 | AI | convention | not_enacted | no / no | Pending independent legal applicability and REG→CTL mapping |
| REG-064 | E-records | legal | current_law | yes / yes | Pending independent legal applicability and REG→CTL mapping |
| REG-065 | E-records | legal | current_law | yes / yes | Pending independent legal applicability and REG→CTL mapping |

**Coverage caution:** 50 legal-classified, 8 risk-control and 7 convention rows. Some are guidance, proposed or not enacted. Source labels do not resolve legal applicability; do not count all 65 as binding law.

## Scenario inventory (22/22 identified)
Source: [scenarios.csv](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/11-scenarios-and-validation/scenarios.csv)

| Scenario | Name | Referenced controls | Required test disposition |
|---|---|---|---|
| SC-01 | Routine cash contribution and domestic grant | CTL-001, CTL-003, CTL-011, CTL-012, CTL-015, CTL-020, CTL-043 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-02 | Appreciated publicly traded securities contribution | CTL-015, CTL-017, CTL-020, CTL-023 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-03 | Complex privately held asset (closely held stock / LP interest / real estate) | CTL-021, CTL-022, CTL-020 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-04 | Donor recommends ineligible recipient (revoked / private non-operating foundation / individual) | CTL-011, CTL-014 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-05 | Suspicious / potentially fraudulent transaction (account takeover then grant to new recipient) | CTL-029, CTL-033, CTL-003, CTL-004 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-06 | Grant involving potential private or prohibited benefit (tickets / tuition / pledge) | CTL-008, CTL-007, CTL-006 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-07 | Large volume of simultaneous recommendations (year-end or disaster-relief spike 10x) | CTL-029, CTL-030 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-08 | Market disruption requiring liquidity management | CTL-023, CTL-024 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-09 | Custodian outage | CTL-015, CTL-035, CTL-036 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-10 | Incorrect AI recommendation (agent mis-resolves recipient or misses a flag) | CTL-008, CTL-026, CTL-027 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-11 | Compromised partner API credential | CTL-034, CTL-001, CTL-028 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-12 | Attempted unauthorized payment (insider or direct call to payment path) | CTL-003, CTL-006, CTL-031, CTL-032, CTL-005 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-13 | Regulatory inquiry (IRS exam or state AG letter) | CTL-037, CTL-019 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-14 | External financial statement audit | CTL-015, CTL-017, CTL-018, CTL-019 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-15 | Donor dispute (grant declined or delayed; fee disagreement) | CTL-039, CTL-040 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-16 | Financial reconciliation discrepancy (custodian position ≠ ledger) | CTL-015, CTL-016, CTL-017, CTL-018 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-17 | Key technology provider failure (core platform vendor or model provider exits) | CTL-035, CTL-036, CTL-027 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-18 | Board-level governance conflict (director related to vendor or major donor) | CTL-041, CTL-042 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-19 | Prompt injection via charity website text into discovery or benefit-risk agent | CTL-008, CTL-026, CTL-027 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-20 | IRS data feed stale or unavailable | CTL-011, CTL-014 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-21 | Mass eligibility / discretionary hold controversy (SPLC-style 2026 holds; multistate AG letter) | CTL-045, CTL-009 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |
| SC-22 | Payee bank-account hijack (BEC impersonating nonprofit) | CTL-004, CTL-003 | Not executed in Unbound; create reproducible fixture, assert immutable history and correction |

## Reconciliation findings and gates
1. All 22 scenarios reference control IDs that exist in the 54-control source inventory.
2. Four scenario event references are **descriptive phrases**, not exact catalog event types: `PaymentInstructed (denied)`, `AgentRunCompleted (absent)`, `CustodianUnavailable analog`, `EligibilityEvaluated (refer)`. Normalize them as scenario assertions (e.g. denial means no PaymentInstructed) rather than inventing authoritative event types.
3. Source CTL-019 covers append-only **ledger** only. Unbound's stricter UNB-DATA-001 applies append-only to **all authoritative business records**. Extend controls to deny UPDATE/DELETE across decisions, policies, identities' authoritative event histories, approvals, grants, positions and evidence.
4. A complete reviewed REG→CTL legal mapping is still missing. Do not report 100% legal-control coverage.
5. No live partner replay, real financial transactions, legal counsel sign-off or Conditor execution was performed by this inventory conversion.
6. Next gate: create row-level tests and regulatory mappings, reconcile source-control gaps, and execute synthetic append-only tamper/replay/correction/adversarial tests. Do not implement the application before requirements approval.
