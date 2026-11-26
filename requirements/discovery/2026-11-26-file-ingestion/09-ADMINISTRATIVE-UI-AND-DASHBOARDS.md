# 09 — Administrative UI and operational dashboard requirements

Status: **proposed**. Requirements UNB-ING-UI-001..003, UNB-ING-OPS-001..003. Presentation follows Forma (zero-runtime
components) and Limen (browser capability); accessibility WCAG 2.2 AA, mobile-first for donors and advisors (CNV-EXP-001).

## 1. Principle

Successful routine processing requires no monitoring and no acknowledgment. People see work only when a decision is
needed (exceptions), when a trend crosses a threshold, or when they ask.

## 2. Surfaces

| Surface | Users | Content | Forma pattern |
|---|---|---|---|
| Upload panel (donor, advisor) | Principals | Accepted formats and limits before upload; template download; progress announced; validated preview; per-row reasons; download of only the rows to fix; confirm | File input + progress + result summary; status (CNV-EXP-003 StatusProjection); receipt (UNB-AVL-002 wording) |
| Ingestion workbench | Sponsor operations, platform operations | Artifact and Run detail: source, channel, profile version, hash, five milestones, inspection evidence, outcome counts, linked exceptions and commands, permitted actions with required authority | Record header + timeline + definition list + actions with disabled-reason |
| Document review | Specialists, reviewers | Raster pages, extraction proposals beside source regions, corroboration, injection flag, accept/correct/reject per field | Split view; field-level state |
| Exception queue | Role-based | One queue for ingestion, reconciliation and document cases: type, severity, age (business calendar), due, owner, suggested cause | Work queue (screen S-008 pattern) |
| Operations dashboard | Sponsor operations, platform | Widgets below | Dashboard widgets (W-* pattern) |

## 3. Dashboard widgets (UNB-ING-OPS-002)

| Widget | Definition | Drill-down |
|---|---|---|
| Expected / missing / late | Expectations by state for today and the last 30 days; lateness distribution | Expectation → source → artifacts |
| Duplicates | Duplicate artifacts and records by source | Pairs |
| Security rejections | Count by rule and source; malware positives highlighted | Artifact inspection evidence |
| Schema changes | Drift holds by source and profile version | Drift report |
| Queue age by stage | Age of oldest and p95 for inspection, parsing, submission, reconciliation | Commands |
| Failed and held records | By reason and source | Records |
| Unreconciled items | By scope and age band | Breaks |
| Pending approvals | Runs awaiting approval or hold decisions, by age | Run |
| Replays | Replays and supersessions this period | Run lineage |
| Source availability | Fetch success rate and last success per source | Fetch attempts |
| Throughput | Records and artifacts processed per day | — |
| Straight-through rate | Records reaching a terminal outcome with no human action ÷ all records; by profile and sponsor | Records with human touches |
| Human interventions | Touches per 1,000 records; minutes recorded per exception type | Exception cases |
| Cost per record / per exception | Platform meters (storage, scanning, AI pages) + recorded effort × loaded rate | Cost lines |
| Sponsor performance | All of the above per sponsor (platform operations only) | Sponsor |

The straight-through rate and cost widgets are how the economics assumptions in daf-landscape get replaced by
measurements; they are not optional reporting.

## 4. Reports

Run report (per artifact, downloadable, formula-safe); monthly ingestion operations report per sponsor; reconciliation
status report at month-end; AI evaluation results per extraction purpose; security inspection summary for the sponsor's
security owner.
