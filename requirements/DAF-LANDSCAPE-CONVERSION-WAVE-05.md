# Wave 5: regulatory-to-control hypothesis mapping and append-only verification gates
Date: 2026-11-21 | Draft, NOT counsel-approved

## Objective and limitation
Convert the 65 source regulatory assertions into an actionable **review queue**, while preserving the difference between legal obligation, voluntary safeguard, proposed rule and non-applicability. Mapping below is a *candidate coverage hypothesis*, not a legal opinion or proof of an operating control. Source: [regulatory-applicability-matrix.csv](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/08-governance-and-compliance/regulatory-applicability-matrix.csv), [controls.csv](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/08-governance-and-compliance/controls.csv).

## Candidate mapping by review tranche
| Source REG range | Candidate controls to investigate | Review focus |
|---|---|---|
| REG-001-006 | CTL-001,003,006,007,011,013,014,021 | DAF distribution eligibility, prohibited benefits, related parties, complex holdings |
| REG-007-012 | CTL-020,021,022,023 | Investment applicability distinctions and donor substantiation |
| REG-013-020 | CTL-011,012,014,020,022,037,038 | Reporting, guidance, eligibility sources and foreign grants |
| REG-021-026 | CTL-009,010,019,037,038,041 | Proposals, donor tax, governance, records |
| REG-027-035 | CTL-023,024,025,037,039,042 | Securities exemptions, disclosures, adviser boundaries |
| REG-036-041 | CTL-012,013,031,037 | Sanctions and AML applicability |
| REG-042-046 | CTL-003,004,032,035,037,044 | Money movement, rails, escheat and fundraising platform |
| REG-047-052 | CTL-031,033,034,035,036,037,051,052,053,054 | Privacy, security and sector-specific applicability |
| REG-053-059 | CTL-037,038,041,042,045,048 | State registrations, audit, charitable fiduciary governance |
| REG-060-065 | CTL-026,027,037,039 | AI and electronic record/signature scope |

**Warning:** A tranche mapping does NOT mean each REG within it is covered by every listed control. Review each assertion separately and record a verified REG→CTL relation, evidence artifact, jurisdiction, applicability and human sign-off. Source legal assertions may be outdated or inaccurate.

## Mandatory review distinctions
- **Sponsor vs infrastructure provider:** sponsorship, fiduciary control, asset custody, money transmission, adviser status, platform solicitation and privacy obligations depend on the actual legal entity and activities. A simulated enterprise must not assert legal licensure or authority.
- **Law vs guidance vs proposal:** REG-016–022, REG-034, REG-056 and REG-063 require explicit status review; proposals and unenacted measures are not binding simply because they appear in the catalog.
- **Negative applicability claims:** REG-007, REG-038, REG-042 and REG-061 must be evaluated with actual entity facts. “Not applicable to a charity” cannot automatically be inherited by a for-profit infrastructure provider.
- **Privacy and immutable history:** REG-026, REG-048–052 and REG-064–065 need an explicit legal design for PII minimization, encrypted vault references, correction notices, retention, export and lawful erasure where required.
- **Time-sensitive legal assertions:** REG-060 and REG-062 require effective-date, geographic scope and current status verification before they are treated as operating obligations.

## Requirements created
- **UNB-REGMAP-001:** Maintain a machine-checkable, one-row-per-REG applicability register with primary citation, accessed date, jurisdiction, affected entity, effective date, classification, accountable reviewer, source evidence and status.
- **UNB-REGMAP-002:** Require a reviewer-approved, many-to-many REG→CTL→test→evidence mapping. Block the compliance gate for applicable binding obligations without an independently verified control and objective evidence.
- **UNB-REGMAP-003:** Explicitly record inapplicable, disputed, guidance-only and proposed obligations with rationale; do not silently omit or misrepresent them as binding.
- **UNB-REGMAP-004:** Re-evaluate applicability on entity-model change (sponsor, infrastructure, partner-of-record), new state or country, asset type, payment rail, vendor, or law effective-date transition.
- **UNB-REGMAP-005:** Preserve every legal interpretation, review, policy approval and superseding correction as immutable events; current compliance status is a projection.
- **UNB-REGMAP-006:** Distinguish required human judgment from deterministic policy execution and probabilistic agent advice for every control; require counsel review of delegated sponsor authority.

## Append-only database acceptance specification
- **UNB-IMM-001:** An authorized append transaction creates an immutable authoritative event and durable sequence position. All existing authoritative event bytes remain unchanged; reject UPDATE, DELETE, TRUNCATE and destructive schema migration through normal operational roles.
- **UNB-IMM-002:** A correction must specify target event/record, reason, actor, effective time, recorded time, evidence and correction kind. Reject an orphan correction and preserve the original in all audit queries.
- **UNB-IMM-003:** Ledger corrections use append-only balanced reversal plus replacement or adjustment. No destructive balance reset.
- **UNB-IMM-004:** Historical queries support (a) actual effective date and (b) what was known by recorded date. A backdated correction does not retroactively alter an earlier as-known view.
- **UNB-IMM-005:** Deterministic projection rebuild from an immutable snapshot/checkpoint and subsequent events produces the same state as full replay. Snapshot is a cache, never authority.
- **UNB-IMM-006:** Replays do not issue external effects. Durable effect receipts and external idempotency keys prevent double grants, payments, trades, notices and filings.
- **UNB-IMM-007:** Detect missing, tampered, reordered and duplicated records; demonstrate independent checkpoint verification and restore without accepting a silently truncated history.
- **UNB-IMM-008:** A privacy or retention request is handled through an approved policy preserving legally required evidence while honoring applicable rights. Do not claim cryptographic erasure works if plaintext exists in backups, logs or exports.
- **UNB-IMM-009:** Cross-tenant authorization is enforced for event append, projection, historical replay, export and correction. Test attempted cross-tenant corrections and provenance leakage.
- **UNB-IMM-010:** Require load tests for 10× grant surge, historical replay and projection rebuild at defined scales; measure storage, integrity verification and correction propagation costs before selecting storage technology.

## Executable test backlog (specification, not test results)
| ID | Given / action | Required assertion |
|---|---|---|
| IMM-T01 | Application attempts to mutate a committed journal row | Database denies; original digest unchanged |
| IMM-T02 | Correct wrong contribution effective date | Both original and correction remain; bitemporal queries differ correctly |
| IMM-T03 | Reverse erroneous grant commitment | Reversal balances and retains sponsor decision evidence |
| IMM-T04 | Replay payment-settled events twice | No duplicate external payment, exactly one current settled projection |
| IMM-T05 | Duplicate or out-of-order custodian events | Idempotent, explicitly ordered per subject, or held for reconciliation |
| IMM-T06 | Alter or truncate historical event store | Integrity verification detects mismatch and halts trusted projection |
| IMM-T07 | Restore backup missing recent events | Checkpoint/watermark mismatch detected; reconciliation before resume |
| IMM-T08 | Agent proposes an unauthorized correction | Tool authorization rejects; append-only denial evidence preserved |
| IMM-T09 | Cross-sponsor principal requests event history | Denied, including replay and exports |
| IMM-T10 | Privacy request targets personal data referenced by events | Reviewed response; no false claim of deletion; necessary evidence retained |
| IMM-T11 | Policy version changes during in-flight grant | Decision tied to correct immutable policy version |
| IMM-T12 | 10× surge plus downstream payment outage | Bounded queue, no lost events, no duplicated effects, eventual reconciliation |

## Validation gates and honest status
- 65/65 REG identifiers inventoried in Wave 4; **0/65 independently revalidated for this wave**.
- 54/54 controls inventoried; **0/54 proven operational in Unbound**.
- 58/58 event names inventoried; **0/58 implemented or replay-tested in Unbound**.
- 22/22 scenario names inventoried; **0/22 executed in Unbound**.
- 12 append-only test specifications above; **0 executed**.
- Conditor installation, actual current EF capability plan and sponsor/provider counsel review are still pending.

Next: build an actual one-row-per-REG register with specific control candidates, and a control-to-scenario-to-test traceability table; verify citations with primary sources before claiming legal readiness. No application code before the requirements gate.
