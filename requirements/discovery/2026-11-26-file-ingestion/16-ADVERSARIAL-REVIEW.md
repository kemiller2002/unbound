# 16 — Independent adversarial review

Three reviewers worked independently and read-only. Each was given the package, the governing baseline and the
relevant code, but not the authors' conclusions, and was asked to break the design:

| Pass | Dimensions | Material / moderate / minor |
|---|---|---|
| R1 | Financial integrity, security, tenant isolation | 10 / 13 / 8 |
| R2 | Architecture consistency, operational simplicity, developer experience, requirements completeness, AI safety | 9 / 15 / 7 |
| R3 | Economics, straight-through-processing realism, regulatory applicability, evidence quality (daf-landscape model and evidence) | 6 / 9 / 5 |

All 82 findings were dispositioned; source: [`data/review-findings.csv`](data/review-findings.csv) (gate I8 requires a
disposition and a recorded change or reason for each). Requirement changes are recorded on each requirement as a
`revisions` entry naming the finding; lifecycle, profiles, catalogs, tests, decisions and the daf-landscape model were
changed in the same revision.

## Challenge questions the mission asked reviewers to test

| Question | Conclusion after review |
|---|---|
| Is every proposed file type necessary? | No: OFAC narrowed to one representation, camt.052/054 made conditional, identity metadata moved out of ingestion (R2-21) |
| Does centralized ingestion introduce excessive abstraction? | The single custody path and "same commands as the interface" are sound (R1, R2); applying Runs and profiles to evidence documents was excessive and was removed (R2-16) |
| Is database storage sufficient? Is object storage justified? | Object storage is justified by managed scanning and storage-enforced retention and legal hold; restore time is an unmeasured supporting argument (R2-31) |
| Does AI add value to each workflow? | Not measured; release-1 AI limited to onboarding and migration mapping; extraction off by default with kill criteria (R2-17) |
| Do profiles become an uncontrolled programming environment? | No — the risk found was the opposite (too rigid); accepted as a trade-off (R2-08); catalogs made verifiable (R2-14) |
| Are STP targets realistic? | 99% has no adjacent-industry precedent; planning targets 75–85% → 90% → 95% stretch (R3-02) |
| Does the design create unnecessary approvals? | Calibration and shadow requirements were too broad and were narrowed (R1-19, R2-08); one approval path was missing and was restored (R1-01) |
| Can controls be bypassed? | Yes, in ten material ways in the first version (R1-01..10); each closed and covered by a test case |
| Is it economically viable for small sponsors? | Only on shared multi-tenant infrastructure; standalone ingestion takes ≈45% of 20 bps at 1,000 funds under modal assumptions (R3-04) |

## Recorded disagreements

- **R2-01 (sandbox scope).** The reviewer proposed isolating only native-codec work (PDF rasterisation, image
  decoding), since managed F# parsers carry less risk. Kept: one sandbox path for all parsing, because two paths cost
  more to build and test than the per-artifact sandbox costs to run. The owner decides the isolation primitive
  (FIL-OD-13).
- **R2-16 (generality).** Accepted for evidence documents and sponsor specialisations; rejected for expectations, which
  CNV-PAY-007's feed-freshness rule needs from the first bank feed.
- **R3-02 (99% STP).** Kept as a sensitivity point, relabelled a prevention scenario without precedent.
- **R3-06 (small-sponsor delivery profile).** A monthly-feed scenario for small sponsors was not modelled; recorded as an
  evidence gap.
- **R1-19 (first-run hold).** The payroll profile keeps its first-run hold until calibration exists for the source.

## Remaining risks no further desk research can close

Measured investigation rates and handling minutes; contract-only bank, lockbox and custodian specifications; the cloud
choice and sandbox primitive; accuracy of document AI on DAF documents; restore-time benchmark with and without document
bytes. Each is an owner decision, an external authorization or an empirical measurement listed in
[14](14-QUESTIONS-AND-OWNER-DECISIONS.md) and the daf-landscape evidence gaps.

## Findings and dispositions

<!-- BEGIN GENERATED: review -->
| Severity | Disposition | Findings |
|---|---|---|
| material | accepted | 22 |
| material | partially-accepted | 3 |
| minor | accepted | 20 |
| moderate | accepted | 36 |
| moderate | partially-accepted | 1 |

| Finding | Severity | Issue | Disposition | Resulting change or reason |
|---|---|---|---|---|
| R1-01 | material | Hold release skipped approval and requester confirmation | accepted | Held -> ReleaseHold -> Validated; exits re-evaluated in order (hold rules, approval, submission mode); TC-LCY-001 |
| R1-02 | material | Revocation did not stop derived runs | accepted | Run state Suspended reachable from every pre-submission state; guard 'artifact Released' on entering Submitting; TC-LCY-002 |
| R1-03 | material | Principal-scoped, time-limited idempotency allowed replay double-posting | accepted | FIN-009: principal-independent business key, original requester authority, retention of record identity, KeyConflict instead of stuck runs |
| R1-04 | material | Key match with different payload treated as Duplicate | accepted | FIN-004: payload hash, KeyConflict outcome and case, intra-file collision Invalid, reference fallback to file id and sequence; TC-FIN-003 |
| R1-05 | material | Tolerance and donor-level auto-matching could credit the wrong fund | accepted | RCN-002: fund credits only on exact reference + amount + sender; tolerance only for confirming existing commands; DAF-002 matches intents only; TC-RCN-001/002 |
| R1-06 | material | Gift dates could come from file fields or execution time | accepted | FIN-006: dates from instrument evidence by observation id only; TC-FIN-004 |
| R1-07 | material | Security override could release unscanned bytes | accepted | Override transition removed; false-positive reinstatement requires a second engine's clean verdict on the exact hash; TC-SEC-014 |
| R1-08 | material | Inspector held database and storage credentials across tenants | accepted | SEC-004: supervisor plus per-artifact sandbox without network, database or cloud credentials; supervisor recomputes totals; TC-SEC-015 |
| R1-09 | material | Adapter parsed multi-tenant files before inspection | accepted | SEC-016: platform-scope source artifact inspected whole; derived per-tenant artifacts with lineage; TC-SEC-016 |
| R1-10 | material | Expectation satisfied by release, not accepted run | accepted | Expectation states Delivered and Suspended; satisfied only by a completed run; watermark (OPS-001); TC-LCY-003 |
| R1-11 | moderate | Record outcomes had no transitions | accepted | Explicit Record machine with transitions and the FIN-003 invariant at Completed |
| R1-12 | moderate | Runs could get stuck | accepted | DependencyTimeout, Cancel, DriftDetected and InspectionFault transitions; intake refusals become record outcomes |
| R1-13 | moderate | Completed with unknown outcomes | accepted | Run state CompletedWithUnknowns |
| R1-14 | moderate | System could supersede a held run; race with auto-submit | accepted | Held runs superseded by humans only; supersession serialised with submission on the source ordering key |
| R1-15 | moderate | Closed reconciliation could not absorb restatements | accepted | Closed scopes immutable; later changes open a linked current-period item (documented in the Reconciliation machine) |
| R1-16 | moderate | Run-level approval and affirmation must be per-command evidence | accepted | FIN-010 and FIN-011: per-command AffirmationEvidence bound to content hashes; batch carries no authority |
| R1-17 | moderate | Payroll profile gaps | accepted | Profile v2: hold-run on unknown fields, funds-received bound to the referenced bank observation, enrollment rule, suspense for remainder, correction rows |
| R1-18 | moderate | Pause drained runs announced as bad | accepted | OPS-003: pause stops every pre-submission transition |
| R1-19 | moderate | Calibration too broad and shadow-only | accepted | RCN-003 and PRF-002: live zero-tolerance calibration for new sources and semantic changes only; tightening-only versions need the corpus; single approver for recipients. The first-run hold remains in the payroll profile until calibration exists for the source |
| R1-20 | moderate | Artifact dedup scope unsafe | accepted | STO-001: dedup scoped to tenant, purpose and source; 'content-addressed' replaced by 'hash-verified'; same date/sequence with different hash is a conflict |
| R1-21 | moderate | Hidden text layer could mislead extraction | accepted | AI-001: extraction on the reviewer's raster; raster/text disagreement flags injection deterministically; TC-AI-002 |
| R1-22 | moderate | Revocation retention conflicted with the 10-year floor | accepted | RET-001: retention follows the highest derived class; never-released bytes only follow the 1-year rule |
| R1-23 | moderate | Presigned URL is not per-request authorization | accepted | SEC-014: proxy or <=60 s version-bound URLs with storage-level deny on revocation; acceptance at T+2 minutes |
| R1-24 | minor | Lifecycle missing transitions required by requirements | accepted | SourceAuthenticationFailed, IntegrityFailed, ReleaseInvalidated added; active-content contradiction resolved |
| R1-25 | minor | Formula neutralisation could corrupt negative numbers | accepted | SEC-009: text cells only; TC-SEC-019 |
| R1-26 | minor | Orphans after restore could not be adopted | accepted | STO-005: dual-controlled adopt-orphan command |
| R1-27 | minor | Webhooks should carry identifiers only | accepted | DLV-003 revised |
| R1-28 | minor | SAML metadata as configuration enables takeover | accepted | Identity metadata removed from the ingestion XML allowlist; FIW-PLT-02 owned by the identity component with out-of-band fingerprint verification |
| R1-29 | minor | Real documents could enter AI evaluation sets | accepted | AI-005 and UI-003: synthetic evaluation sets only |
| R1-30 | minor | Profiles and catalogs not control-relevant | accepted | PRF-002: platform profiles, catalogs and mappings are control-relevant under CNV-PLT-010 |
| R1-31 | minor | Package consistency (missing 16, API batch definition) | accepted | This document created; BND-001 states every API record batch is an artifact |
| R2-01 | material | Inspector is a second deployment unit with cross-tenant reach | partially-accepted | Accepted: honest wording, no database credentials, per-task tenant-scoped storage access, owner decision FIL-OD-13. Not accepted: isolating only native-codec work — one sandbox path for all parsing is simpler than two and its per-artifact cost is small; recorded as a disagreement |
| R2-02 | material | Multi-file sources | accepted | BND-010 artifact sets with manifest, completeness and set hash; lockbox image packages; TC-BND-007, TC-DAF-001 |
| R2-03 | material | All-or-nothing only at validation | accepted | FIN-011 atomic batch command; TC-FIN-002 |
| R2-04 | material | Queue fairness for ingestion (#15) | accepted | BND-007; TC-BND-005 |
| R2-05 | material | Network restrictions bypassed by presigned URLs (#16) | accepted | SEC-017; TC-SEC-018 |
| R2-06 | material | Authority of machine-submitted batches (#17) | accepted | BND-008 on-behalf-of principal; profile field on_behalf_of_principal; gate I5; TC-BND-006 |
| R2-07 | material | PGP-encrypted partner files had no decryption home | accepted | DLV-002: supervisor decrypts with secret-store key before inspection; TC-DLV-002 |
| R2-08 | material | Profiles too rigid with month-end shadow for every version | accepted | RCN-003 and PRF-002 narrowed; sponsor tuning without shadow; partner quirks still need a catalog release (accepted trade-off, ADR-ING-003) |
| R2-09 | material | Recall-gated pre-fill; non-independent corroboration | accepted | AI-002 precision gating, independent sources only, consistency checks labelled; catalog labels appraisal-vs-8283 as consistency |
| R2-10 | moderate | No local developer path | accepted | BND-009; TC-TST-001 |
| R2-11 | moderate | Verdict path implied event infrastructure | accepted | SEC-005: poll verdict bound to object version and hash; TC-SEC-017 |
| R2-12 | moderate | Write-once compliance mode blocks misfiled-upload deletion and crypto-erasure | accepted | STO-003 correction window and liftable retention mode; STO-004 per-tenant key decision before first write |
| R2-13 | moderate | Lifecycle inconsistencies | accepted | Resolved with R1-07, R1-11, R1-12 and drift transition |
| R2-14 | moderate | Catalogs were open strings | accepted | data/catalogs.json; gate I5 checks rules, corroborations, purposes, formats, operations and record keys; business rules removed from profile catalogs (handler validate mode) |
| R2-15 | moderate | Profile data inconsistencies | accepted | Profiles v2 (BMF, payroll, migration, appraisal) and schema conditionals |
| R2-16 | moderate | Every class forced through Run and profile machinery | partially-accepted | Accepted: evidence documents attach without a Run (BND-001); sponsor specialisations (extends) deferred until a second sponsor uses a profile. Not accepted: deferring expectations — CNV-PAY-007 freshness needs them from the first bank feed |
| R2-17 | moderate | AI value asserted, not measured | accepted | Appraisal profile without AI; release-1 AI scope is onboarding and migration assistance; kill criterion in AI-005; extraction off by default |
| R2-18 | moderate | Self-referential injection controls; routing as action | accepted | AI-001 deterministic detectors; AI-006 deterministic routing and linking; AI-007 minimisation and disclosure log; TC-AI-007 |
| R2-19 | moderate | New requirements that should be refinements | accepted | FIN-005 refines ADM-032; FIN-008 refines ACC-MDL-002; LCY-003 refines UNB-GAP-019; refinements of UNB-AVL parents gate on adoption of the availability mission |
| R2-20 | moderate | Test cases under profiles that reject their input; P0 coverage | accepted | Three evidence profiles added; cases repointed; gate I7 now covers every P0 requirement except review-only ones; 36 cases added |
| R2-21 | moderate | Format scope larger than needed | accepted | One OFAC representation; camt.052/054 conditional; identity metadata out of ingestion |
| R2-22 | moderate | Framework ownership unassigned | accepted | Ownership table in 12; UI-004 offline honesty (Limen) |
| R2-23 | moderate | Mobile and WASM upload underspecified | accepted | STO-002 resumable multipart with per-part checksums; UI-004 optional WASM pre-validation; FIL-OD-01 notes checksum binding differences |
| R2-24 | moderate | Material decisions routed to the agent | accepted | FIL-OD-05 and FIL-OD-13 reopened to the owner (QE-001581..001585) |
| R2-25 | minor | Missing 16; unresolved AR-06 reference | accepted | This document; references corrected to R2-08 |
| R2-26 | minor | EFI references depend on an unmerged pull request | accepted | README states the dependency on the organization-administration pull request |
| R2-27 | minor | Headline counts not derivable | accepted | primary_classification column; counts generated in 13 |
| R2-28 | minor | P0 inflation | accepted | AI controls P0 only where AI is enabled; several requirements re-tiered to P1 |
| R2-29 | minor | Garbled FIN-010 acceptance | accepted | Rewritten |
| R2-30 | minor | Parser library licensing unassessed | accepted | parser_library_note column in format decisions (licences to confirm) |
| R2-31 | minor | Restore-time argument unmeasured | accepted | 07 and ADR-ING-002 rest on scanning and write-once retention; restore time stated as supporting and unmeasured |
| R3-01 | material | Free cost cliff above 1 - investigation share | accepted | Model v2: explicit prevention scenario with cost; routine STP excludes investigations; 05 says 'assumes' |
| R3-02 | material | 99% STP unsupported by evidence | partially-accepted | Accepted: planning targets 75-85% / 90% / 95% stretch, dashboard STP reported, 99% labelled a prevention scenario. Kept 99% in the grid as a sensitivity point |
| R3-03 | material | Balance confounded with cost case | accepted | Balance reported separately ($50k/$150k/$300k) in every row; FIL-OD-10 corrected (QE-001583) |
| R3-04 | material | Small-sponsor fixed and indivisible costs omitted | accepted | Run cost, amortised onboarding and a standalone staffing floor; pooled vs standalone reported |
| R3-05 | material | Document rows silently zero | accepted | Documents count files plus per-fund records; data rows without record drivers count files |
| R3-06 | material | Volume drivers unlabelled; grant-driven rows mis-set | partially-accepted | Accepted: volume_basis and minutes_basis columns; grant-driven rows scale with grants per fund. Not done: a monthly-feed small-sponsor delivery scenario (listed as an evidence gap) |
| R3-07 | moderate | File exceptions used the record STP rate | accepted | file_exception_share parameter |
| R3-08 | moderate | Uniform investigation share; parameter inconsistencies | accepted | Class-specific shares aligned with reconciliation-break rate; productive hours 1600/1600/1400; minutes relabelled assumed |
| R3-09 | moderate | Possible double counting with staffing models | accepted | Model and findings state ingestion is a slice of operating cost |
| R3-10 | moderate | Advisor-managed and migration volumes | accepted | Advisor-managed funds drive custody rows (monthly reconciliation unit); migration excluded from the annual model |
| R3-11 | moderate | Onboarding parameters contradicted their anchor | accepted | Bespoke 15/30/50 days; vendor-sourced anchor noted |
| R3-12 | moderate | 'Platform' cost was only metered cloud | accepted | Renamed metered_cloud_usd; run cost per sponsor added |
| R3-13 | moderate | Measured vs assumed lost downstream | accepted | input_basis column; retention relabelled decision; downstream wording corrected |
| R3-14 | moderate | Unverifiable or mis-cited evidence rows | accepted | FIE-B-013 marked unverified; FIE-B-012, FIE-B-066, FIE-A-016, FIE-B-043 notes corrected |
| R3-15 | moderate | Nacha 2026 overstated; no ingestion requirement | accepted | Research wording corrected; DAF-013 monitoring evidence |
| R3-16 | minor | Form 8283/8282 thresholds imprecise | accepted | DAF-010 thresholds by asset class |
| R3-17 | minor | SEC 17a-4 applicability | accepted | No change needed; 07 cites Cohasset as control strength, not applicability |
| R3-18 | minor | OFAC SLS | accepted | No change needed; 'where provided' kept for hashes |
| R3-19 | minor | State registries | accepted | No change to requirements; California registry date to be re-read before build (gap G12) |
| R3-20 | minor | Metric hygiene | accepted | Cost per human unit includes file exceptions; manual baseline labelled a reference, not current practice |
<!-- END GENERATED: review -->
