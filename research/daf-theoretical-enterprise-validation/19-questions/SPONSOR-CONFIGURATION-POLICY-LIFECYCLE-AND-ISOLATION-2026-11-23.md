# Sponsor configuration, policy lifecycle and isolation
Status: proposed requirements, not implemented. 2026-11-23.

## Principle
One product, one component monolith, versioned sponsor-specific configuration. White label affects language, branding, experience and permitted policy choices, not statutory meaning or platform safety. A sponsor cannot shorten the seven-year financial retention floor, bypass charity global holds, weaken payment destination verification or disable accounting integrity.

## Policy layering
Platform hard controls > binding law and protective holds > sponsor governing instruments and approved policy > fund restrictions > channel/user preferences. Where obligations conflict, fail safely and route to authorized review rather than using a simplistic numeric precedence. Use typed policy composition with explicit deny, allow, requires evidence and unresolved conflict. Sponsor configuration must be declarative, schema-validated and reviewable, not executable tenant code.

## Configuration catalog
Brand kit, legal entity/disclosures, terminology (“grant” aliases), sponsor-specific prohibited-benefit affirmations, donation and grant minimums, recurring contribution and grant schedules, fee schedules/manual processing, investment programs, charity restrictions, fund types, delegated approval thresholds, document templates, email templates, delivery methods, accessibility, locale, timezone, accounting calendar, statement deadlines, reporting permissions, embedded widgets and APIs. Distinguish mandatory global controls from adjustable choices.

## Lifecycle
Draft -> Validated -> Simulated -> SponsorApproved -> PlatformSafetyApproved(where required) -> Scheduled -> Active -> Superseded/RolledBack. Record effective date, activation timestamp, approval provenance, policy hash, affected fund cohorts and migration behavior. Transactions and issued documents pin policy versions. Never retroactively rewrite as-issued decisions; explicit remediation/correction if required.

## Migration and conflict testing
Simulate change against open grants, future grants, recurring $10/pay-period contributions, active holds, payment instructions, fee accruals, pending statements and fund closure. Flag grandfathering versus prospective rules. Changes to terminology must preserve legal disclaimers and accessibility labels. Support sponsor sandbox and preview with mock data. A sponsor termination triggers governed export and retention, not destruction.

## UI and validation
Sponsor Policy Studio with change comparison, risk classification, impacted workflows, simulation results, approvals, rollout schedule, rollback and audit. Role-based controls for sponsor administrators and platform compliance. Test illegal shorter retention, fee backdating, conflicting fund restrictions, grant synonym changing affirmation, future recurring grants after policy update, active workflow during rollback, template change after issuance, tenant crossover and partial deployment.
