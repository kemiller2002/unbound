# Workflow model schema (WFM)

Each release-1 workflow is one JSON file `WFM-nn-<slug>.json`, validated by `workflows` (`tools/fs/Unbound.ReqAudit`) (checks W1–W11).

```json
{
  "workflow_id": "WFM-07",
  "name": "Grant approval, reservation and funding (sponsor side)",
  "release": "R1",
  "outcome": "A lawful, sponsor-authorized grant funded exactly once",
  "aggregate": "Grant",
  "sources": ["RR-REQ-X-002-r1", "UNB-RCN-FIN-008", "TX-08"],
  "observer_surfaces": ["UNB-UI-S-004"],
  "states": [
    {"id": "GrantRecommended", "kind": "initial", "user_label": "Recommended", "lozenge": "attention", "description": "..."},
    {"id": "Approved", "kind": "intermediate", "user_label": "Approved", "lozenge": "ok", "description": "..."},
    {"id": "Paid", "kind": "terminal", "user_label": "Paid", "lozenge": "ok", "description": "..."},
    {"id": "GrantCorrection", "kind": "correction", "user_label": "Corrected", "lozenge": "neutral", "description": "linked correction child; parent unchanged"}
  ],
  "transitions": [
    {
      "id": "WFM-07-T03",
      "from": "EligibilityConfirmed",            // or a list of states
      "to": "Approved",
      "trigger": "command",                       // command | event | timer
      "name": "ApproveGrant",
      "actor_type": "deterministic",              // human | deterministic | ai | external | timer
      "actor_role": "Sponsor policy (board-delegated) or Grants approver",
      "authority": "A3",                          // A0..A4 (A5 prohibited, never a transition)
      "guards": ["eligibility-current", "sanctions-clear", "segregation-of-duties", "capacity-in-transaction"],
      "effects": ["grant-approval", "reserve-capacity", "ledger"],
      "idempotency_key": "(grantId, policyVersion)",
      "failure": [{"condition": "PolicyVersionChanged", "to_state": "EligibilityConfirmed"},
                  {"condition": "InsufficientCapacity", "to_state": "FundingShortfall"}],
      "requirement_ids": ["RR-REQ-X-002-r1", "UNB-RCN-FIN-009"],
      "scenario_ids": ["DKS-XD-005", "DKS-XD-013"],
      "ui_surface": "UNB-UI-S-009",               // required when actor_type = human; "NEW:<Surface name>" if no UI requirement exists yet
      "notes": "Until RCD-005 delegation is approved, outcome is ApproveRecommended then a human A4 batch approval."
    }
  ],
  "invariants": ["No payment attempt exists for a grant not Funded"]
}
```

(Comments above are illustrative; files must be strict JSON.)

## Rules

- **States.** `kind` is `initial` (exactly one), `intermediate`, `terminal` or `correction`.
  - Terminal states may transition only to correction states (linked correction children).
  - Every state has a `user_label` and a `lozenge` (`ok`, `attention`, `unknown`, `blocked` or `neutral`), following UNB-UI-G-001.
  - The same state id must carry the same label and lozenge in every workflow.
- **Authority ceilings.** `ai` ≤ A2, `deterministic`/`timer`/`external` ≤ A3, A4 only for `human`.
- **Mandatory guards by effect:**

  | Effect | Required guards |
  |---|---|
  | `external-money-out` | `eligibility-current`, `sanctions-clear`, `hold-watermark-current`, `destination-verified`, `funded` |
  | `reserve-capacity` | `capacity-in-transaction` |
  | `grant-approval` | `eligibility-current`, `sanctions-clear`, `segregation-of-duties` |
  | `credit-grantable` | `contribution-final` |
  | `issue-acknowledgment` | `completed-gift-determined`, `sponsor-accepted` |

- **External interactions.** Any transition whose `actor_type` is `external`, or whose `effects` include `external-call`, MUST include a failure entry whose `condition` starts with `Unknown` or `Timeout`.
- **UI surfaces.** Every `human` transition names `ui_surface`: an existing `UNB-UI-S-*`/`W-*` id if that screen covers it, otherwise `NEW:<surface name>`. Use the same NEW name consistently so the inventory groups correctly. `observer_surfaces` lists screens that display the workflow without acting on it.
- **Traceability.** `requirement_ids` must resolve: baseline ids, `RR-*`, `UNB-RCN-*`, `UNB-UI-*`, `RCD-*`, renamed ids. Cite `TX-nn` rows of the transition matrix in `sources` only; they are not requirement ids.
- **Scenarios.** `scenario_ids` must exist (`DKS-*`, `ADV-*`, `SIM-*`, `TVS-*`). An empty list is allowed but warned on guarded or failure-bearing transitions.
