# Enterprise decision rights and delegated authority
Status: proposed requirements, not implemented or legally certified. 2026-11-23.

## Principle
Every material decision has an explicit accountable authority, permitted delegate, scope, expiry, separation-of-duties rule, evidence threshold and audit record. Sponsor boards retain sponsor-specific legal authority; platform operators cannot silently substitute for sponsor officers. AI has no independent legal, accounting, compliance, payment or financial authority. A case closure is never a domain approval.

## Decision matrix
| Decision | Source authority | Delegation and guardrails |
|---|---|---|
| Routine grant eligibility and approval | Sponsor board-approved policy | Deterministic approval within valid scope; hard holds override |
| Exceptional grant and prohibited-benefit determination | Authorized sponsor/compliance officers | Independent review; no universal override |
| Charity eligibility and 180-day refresh | Platform compliance policy plus sponsor restrictions | Deterministic evidence checks, independent sanctions escalation |
| Payment release and destination control | Sponsor-approved payment authority and financial core | Dual control where mandated, atomic reservation, provider Unknown blocks retries |
| Contribution acceptance and reversals | Sponsor finance and governing policy | Deterministic routine cases; independent disputed cases |
| NAV, valuation and financial recognition | Finance/controller under approved accounting policy | CPA/CFO review where required |
| Period close and restatement | Sponsor finance authority | Separately approved from statement issuance |
| Statement issuance and correction | Sponsor-approved statement policy | Automated after verified close only under valid delegation |
| Fees, waivers and refunds | Sponsor-authorized pricing/finance | Guardrails, independent exceptions, immutable posting evidence |
| Legal hold, retention shortening and disposition | Legal/compliance authority | Nonwaivable seven-year financial floor; no sponsor override |
| User permissions, succession and account closure | Sponsor-admin authority subject to platform security | Proof of identity and segregation |
| Sponsor policy changes | Sponsor policy owner plus platform validation | Prospective versioning, simulation, effective dates |
| Incident protective freeze | Authorized platform security/compliance | Protective-only; no new payment authority |
| External provider credential rotation | Platform security/integration owner | Dual authorization for sensitive operations |

## Typed authority record
DecisionType, Scope(sponsor/fund/amount/recipient/channel), AuthoritySource, DelegationId, EffectiveFrom, ExpiresAt, PolicyVersion, EvidenceThreshold, RequiredApprovers, ConflictsOfInterest, ProtectiveHolds, DecisionDigest, AuditRef. Check authority at decision and execution; revocation or policy change must invalidate stale authorizations. Do not let role titles alone grant unlimited rights.

## Challenge
Avoid universal super-admin and permanent override. Emergency authority is protective-only. Cross-sponsor staff can support multiple tenants without cross-tenant authorization leakage. Automated approval requires positive evidence, not absence of a detected exception. Approval authority and technical access are distinct. Appeals and corrections preserve original decisions.

## UI and validation
Authority Matrix console: per sponsor and decision, active delegations, expirations, evidence, conflicting duties, changes and audit. Show “why permitted/blocked.” Test expired delegation, revoked approver, self-approval, cross-sponsor access, AI override, concurrent hold, emergency freeze abuse, amount threshold splitting and stale policy.
