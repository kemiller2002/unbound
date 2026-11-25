# 04 — Decisions and open questions

## ADR-AVL-001 — Adopt the INIT-012 hybrid execution architecture for Unbound

* **Status:** Proposed (requires owner ratification; does not amend CNV clauses)
* **Context:** CNV-ARC-001 and ADR-EVA-001 already select a modular monolith on managed PostgreSQL with outbox,
  in-database state machines and a timer table, with brokers only on evidence. unbound#12 asks for durable
  asynchronous intake "as normal architecture, not maintenance-only mode".
* **Decision:** Unbound applies [ADR-EAR-001…009](https://github.com/kemiller2002/echelon-organization-administration/blob/claude/echelon-availability-architecture-ezhitq/application-governance/availability-architecture/17-ARCHITECTURE-DECISIONS.md): synchronous execution by default
  (queries, immediate controls, short local commands); the durable command path for the classes in the operation
  matrix; intake and worker as roles of one build (consistent with CNV-ARC-001's separately scaled worker roles);
  the Execution module incubated as a separate project (UNB-AVL-019); honest refusal during database outage
  (UNB-AVL-011); no cache in release 1 (UNB-AVL-018).
* **Why not durable intake for every write:** acknowledgments, controls and review decisions must take effect
  immediately; queuing them adds status polling without reducing any failure mode (INIT-012 adversarial review C-1/C-2).
* **Consequences:** unbound#12's "durable asynchronous intake as normal architecture" is satisfied for the operations
  where it adds value, not universally — this is a deliberate narrowing for owner review.

## Threat-model deltas specific to Unbound

The portfolio threat model ([10](https://github.com/kemiller2002/echelon-organization-administration/blob/claude/echelon-availability-architecture-ezhitq/application-governance/availability-architecture/10-THREAT-MODEL.md)) applies. Unbound-specific additions:
hold-version fencing at dispatch (CNV-ARC-003) is the control for T2/T17 on payments; the federated queue projection
contains only routing metadata (CNV-TEN-001) for T3/T9; message publication abuse (T12) is additionally constrained by
communications law classification (CNV-PLT-001) for any notice that is also emailed.

## Open questions (Unbound-specific)

| ID | Question | Owner |
|---|---|---|
| U-10 | Queued commands when a new mandatory notice is published: hold or reject, per operation | Product / compliance |
| U-11 | May an acknowledgment ever be legal assent | Legal |
| U-12 | Retention period for terminal command payloads | Sponsor compliance owner (CNV-PLT-004) |
| AVL-Q1 | Is a December change freeze part of the CNV-SVC-002 readiness gate | Owner |
| AVL-Q2 | Candidate SLO values (processing delay by class) | Owner after benchmark |

U-numbers are shared with the portfolio register ([19](https://github.com/kemiller2002/echelon-organization-administration/blob/claude/echelon-availability-architecture-ezhitq/application-governance/availability-architecture/19-UNRESOLVED-DECISIONS.md)).
