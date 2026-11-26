# 07 — Storage architecture evaluation

Status: **proposed** (ADR-ING-002; owner decision FIL-OD-02). Requirements UNB-ING-STO-001..005.

## 1. The question

Where do inbound file bytes live, given CNV-ARC-001 ("a second durable protective store only if objectives cannot
otherwise be met"), UNB-XIN-EVA-013 ("no second authoritative store without an ADR with measurements") and UNB-AVL-011
("must not buffer work outside the database")? Several older texts already assume an object store (REQ-X-034,
UNB-TEN-009, UNB-XIN-SAS-009). The reconciliation inventory classed this as the one unresolved conflict.

## 2. Options compared

Prices measured from the AWS Price List API and Azure Retail Prices API on 2026-11-26 unless marked inferred
(daf-landscape evidence FIE-C-019..031).

| Criterion | A. PostgreSQL bytea | B. Managed object storage (bytes) + PostgreSQL (everything else) | C. Hybrid threshold (small in PG, large in objects) | D. Managed file storage (NFS/SMB) |
|---|---|---|---|---|
| Infrastructure added | None | One managed bucket/container pair per environment | Same as B | A file share |
| Transactional consistency | Native | Slot-first protocol + reconciler (§3) | Both mechanisms | Same as B, weaker checksums |
| Size limit | 1 GB per value (FIE-C-019) | ≥ 5 TB per object | Mixed | Large |
| Storage cost | $0.115/GB-month, ~$0.23 with HA, plus backup/WAL churn (FIE-C-023/024) | $0.0208–$0.023/GB-month (FIE-C-020/021) | Mixed | Higher than objects; capacity provisioned |
| Backup size and restore time | Grows with every document; PITR replays every byte through WAL | Database stays metadata-sized; objects versioned separately | Partially grows | Separate backup regime |
| Malware scanning | Self-hosted engine required | Native on-upload scanning on AWS and Azure with fail-closed access primitives (FIE-C-001..009) | Two scanning paths | Self-hosted or agent |
| Retention and legal hold | Application code only | Write-once retention and legal hold enforced by storage, Cohasset-assessed (FIE-C-027..030) | Two regimes | Snapshots, not WORM per object |
| Tenant segregation | Schema/role + RLS (existing) | Tenant key prefix + IAM conditions + application authorization | Both | Directory ACLs |
| Encryption | Database encryption | Provider-managed keys per environment; per-tenant keys possible (crypto-erasure) | Both | Share-level |
| Large-file processing | Poor (memory, TOAST) | Streaming | Mixed | Streaming |
| Portability | High | High (S3-compatible APIs are common; abstraction is a small adapter) | Lower (two paths) | Low |
| Developer experience | Simple locally | Local emulator or file-system adapter in tests; one code path | Two code paths to test | Mount management |
| Operational burden | DB growth, vacuum, longer restores | Lifecycle rules, inventory reconciliation | Highest | Capacity management |

**Recommendation: B.** It is the simplest architecture that satisfies all requirements, because two of them —
malware inspection without a custom engine, and storage-enforced retention with legal hold — are only met natively by
object storage. Keeping document bytes out of PostgreSQL's WAL and backups should also help restore objectives, but
that is a supporting argument that has not been measured (review R2-31); at the modelled volumes size and cost alone
would not decide it. The bytes are non-authoritative and hash-verified (duplicate detection is scoped to tenant,
purpose and source, never across tenants): every
fact about them (identity, hash, state, verdicts, retention class, holds, extracted values, audit) is an authoritative
PostgreSQL row, and the durable command queue carries references, never bytes (unbound#15). C was rejected because it
doubles the scanning, retention, export and recovery logic to save one round trip on small files.

**What was not added:** no new service, no message broker, no cache, no file-share, no custom scanner. The inspector is
a role of the existing binary.

## 3. Consistency protocol (slot-first)

1. **Slot.** Commit an Artifact row in *Receiving* (tenant, profile, generated UUIDv7 key, size limits). If PostgreSQL is
   down, stop here: nothing is issued (UNB-ING-BND-005).
2. **Bytes.** Upload to the quarantine container through a short-lived, write-only URL bound to the exact key, a size
   range and the required SHA-256; server-side streams for pull adapters.
3. **Commit.** On storage confirmation, one transaction moves the Artifact to *Quarantined* with hash and size and
   enqueues the inspection command.
4. **Release.** After a committed clean verdict bound to the object version and hash (the supervisor polls the scan
   result; no event broker is needed), copy to the records container and record the version id. Retention is applied
   after a short correction window (proposed 7 days) in which a misfiled upload can be deleted under dual control, in a
   retention mode that court-, regulator- or privacy-mandated deletion can lift under dual control (review R2-12).
   Uploads are resumable multipart with per-part checksums; the server verifies the whole-file SHA-256 on commit,
   because not every cloud can bind a checksum to the upload URL (review R2-23).

| Failure | Detection | Resolution |
|---|---|---|
| Upload interrupted | Slot older than its lifetime | Artifact *Abandoned*; partial object deleted |
| Object without row (crash between storage write and commit) | Daily inventory comparison | Quarantine object expires by lifecycle rule; reported |
| Row without object (lost write, operator error) | Inventory comparison; hash check on every read | Artifact *Revoked*; security exception |
| Database restored to an earlier point | Post-restore reconciliation before ingestion resumes | Later objects reported as orphans (kept while under retention); no Run may reference a missing object |
| Records copy fails after verdict | Command retry (idempotent by key) | Retries; exception after expectation |

Nothing enters the records container before its row exists, so locked orphans cannot occur (FIE-C research report §2.3).

## 4. Sizing and cost

Using the daf-landscape model's volume drivers (mode case, `scripts/fie_economics.py`):

| Funds | Files per year | GB per year | GB retained (10 years, CNV-PLT-004) | Object storage per year | Same bytes in HA PostgreSQL (≈10×) |
|---|---|---|---|---|---|
| 1,000 | ≈6,500 | ≈11 | ≈115 | ≈$32 | ≈$320 |
| 10,000 | ≈14,000 | ≈20 | ≈205 | ≈$57 | ≈$570 |
| 100,000 | ≈87,000 | ≈110 | ≈1,100 | ≈$305 | ≈$3,000 |
| 1,000,000 | ≈820,000 | ≈1,000 | ≈10,000 | ≈$2,800 | ≈$28,000 |

Scanning adds $0.09–$0.15 per GB plus $0.215 per 1,000 objects on AWS (≈$30 a year at 100,000 funds). These costs are
small at every scale; the decision rests on capability (scanning, write-once retention, legal hold) and on keeping the
database's backup and restore objectives achievable, not on storage price. The volumes are model outputs from assumed
drivers, not measurements.
