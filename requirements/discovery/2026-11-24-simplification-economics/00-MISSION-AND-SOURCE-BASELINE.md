# 00: Mission and source baseline

**Date:** 2026-11-24. **Branch:** `claude/daf-economics-event-architecture-v1zdti`, restarted from `main` at `5c04c42` after PR #5 merged.

**Mission.** Mount an adversarial examination of the Unbound DAF enterprise operating model and its requirements. The aim is to improve the economics, accessibility, reliability and operating efficiency of a whole DAF organization. The order of work is: design the business first, then refine the software requirements to support it.

**Status.** This is requirements and theory only. No production code was written, and every number is labelled with its basis.

## 1. Sources and revisions

| Source | Revision | Role |
|---|---|---|
| `kemiller2002/unbound` | `main` @ `5c04c42`. This includes PR #4 (cross-industry discovery, decisions and adoption) and PR #5 (release-1 scope and benchmark). | Authoritative requirements. The latest state was located before starting: the branch named in the mission brief had been merged and was superseded by `main`. |
| Baseline originals | `777a70d`, as indexed in `reconciliation/2026-11-23/data/inventory.csv`. Since then, 46 records were adopted into the domain files (additions only). | The original requirement corpus |
| Reconciliation | `requirements/reconciliation/2026-11-23/` | UNB-RCN-*, RR-*-r1, RCF/RCC/RCD/DKS, OB-* |
| Cross-industry discovery | `requirements/discovery/2026-11-23-cross-industry/` (reports 00–18) | UNB-XIN-*, RR-*, the 41 decided-provisional decisions (report 16), the proposed release-1 scope (report 17), the measured benchmark (report 18) |
| `kemiller2002/daf-landscape` | `origin/main` @ `054c1b8` | Read-only evidence. Includes `research/cross-industry-daf-operating-model-findings-2026-11-23.md` (85 lines), which the mission names. |
| Echelon AI-Native Enterprise Design Framework | `763ff8c` | Echelon method and the DAF case study |
| Clarity framework | `200a9eb` (attached read-only for this mission) | Clarity method (report 15) |
| Echelon Diagnostic Framework | `710d54c` (anonymous read) | EDF grammar v0.3 (report 16) |
| AI-Engineering | `1bc519b` | Other Echelon research |

## 2. Review universe (Phase One)

Every requirement record is reviewed exactly once. Gate S1 enforces this.

| Source | Records |
|---|---:|
| Baseline canonical requirement IDs (the 1,386 identifiers of kind `requirement`, less `-Q` restatements and duplicate definition sites) | 1,148 |
| Reconciliation new requirements (UNB-RCN-*) | 97 |
| Reconciliation refinements (RR-*-r1) | 46 |
| Discovery new requirements (UNB-XIN-*) | 150 |
| Discovery refinements | 32 |
| **Total** | **1,473** |

The review was divided into four domain slices:
- finance: 342;
- grants, charities and payments: 370;
- platform, AI, UI and governance: 379;
- enterprise administration and lifecycle: 382.

Each slice was reviewed by its own role-separated agent pass. The remaining phases ran as five further passes, followed by an independent adversarial pass. See [LEDGER.md](LEDGER.md).

**What is not in the review universe.** Controls, scenarios, invariants, tests and decisions are not requirements. They are covered only where a requirement depends on them.

## 3. Conventions

- **Directory.** The mission follows the discovery-directory convention of `requirements/discovery/<date>-<topic>/`. Its data lives in `data/` and its reports are numbered.
- **ID schemes.** The schemes are new; gate G2 verifies they do not collide with any existing family. The validator is `validate-simplification` (`tools/fs/Unbound.ReqAudit`).

  | ID pattern | Record type |
  |---|---|
  | `UNB-SMP-<DOM>-nnn` | New requirement |
  | `SPF-` | Finding |
  | `SPD-` | Open decision |
  | `SPS-` | Scenario |
  | `SPR-` | Rejected approach |
  | `SPW-` | Work-inventory activity |
  | `SPM-` | Metric |
  | `SPG-` | Configuration item |
  | `SPX-` | Customer interaction |
  | `SPC-` | Competitive comparison row |
  | `SPT-` | Research-transfer candidate |
  | `RR-<original>-r<n>` | Refinement of a baseline original |
- **Number labels.**

  | Label | Meaning |
  |---|---|
  | **[S]** | Sourced |
  | **[A]** | Assumption |
  | **[D]** | Derived |
  | **[M]** | Measured |

  The per-account cost targets ($258, $100, $50, $25, $10) are exploratory design targets. They are not costs, forecasts or commitments.
- **Requirement status.** Records keep the status distinctions proposed, accepted, rejected and unresolved. Nothing in this mission edits an original requirement or an adopted section. Where adopting a refinement needs authority, that is recorded as an open `SPD-*` decision.
- **daf-landscape.** It is not modified, and gate D8 checks this. Findings with independent value go to the research-transfer register (report 21).
