# 13 — Business-model analysis (mission §5)

This report analyses the ten operating models listed in mission §5. For each model it separates three kinds of capability:

- **Common core.** Every model uses it unchanged.
- **Configured.** Same code, different settings.
- **Model-specific.** A capability only some models need, still built in the same codebase.

The full matrix is in [industry/sas.md](industry/sas.md) §7.

**Headline.** No operating model needs a separate implementation (XIF-SAS-017, XID-SAS-001). Every model is served by some combination of:
- entitlements (UNB-XIN-SAS-001);
- sponsor policy (REQ-X-045);
- brand kit and terminology;
- adapters, the app registry and subscriptions (UNB-XIN-SAS-004, -006);
- view composition;
- the isolation profile.

The isolation profile is a deployment decision taken from the contract, not from the operating model.

## Risks that are specific to a model

| Model | Risk | Requirement or decision |
|---|---|---|
| Hybrid | Safe only if each tenant is one legal sponsor | RCD-004 |
| High-volume, low-balance | Viability cannot be judged without per-tenant metering. Minimum fees reduce charitable assets, which raises a reasonableness question. | UNB-XIN-SAS-009, UNB-XIN-ECO-018, XID-ECO-002 |
| D2C own-brand app; embedded charity-side button | The platform company itself may become a California charitable fundraising platform. The sponsor's exclusion does not pass to its vendor. | XID-SAS-005 (OB-38) |
| Outsourced administration; white-label | Unlawful if operator staff can act outside sponsor authority | UNB-XIN-IAM-002, -005 |
| Advisor distribution; investment-centric | The operator may become an investment adviser or broker-dealer. Advisor fees are debited from charitable assets without anyone checking them. | UNB-XIN-REG-002, UNB-XIN-INV-001, -002 |
| Institutional; white-label | When a partner or program exits with its funds, each fund needs its own sponsor decision | UNB-XIN-SAS-007, UNB-XIN-EVA-009 |
| HNW / family office | Conflicting instructions, entity holders, representatives and succession all become complex | RR-ADM-023-r1, UNB-XIN-TRS-003, -004 |

## Model matrix

<!-- BEGIN GENERATED: models -->
| Model | Distinguishing capabilities | Configuration needed | Unsupported or risky | Confidence |
|---|---|---|---|---|
| D2C | mobile-first self-service, donor CIAM at scale, own-brand charity search/recommendation app | Brand Kit, fee schedule, minimums, notification catalog, step-up thresholds | Platform company may be a CA Type A charitable fundraising platform if it operates an own-brand app (XID-SAS-005), Operator access must be sponsor-approved when vendor runs ops | medium |
| white-label | Brand Kit per program, terminology, program-level entitlements, program description, partner exit | Brand Kit, terminology, entitlements, fee waterfall, SSO per program | Partner exit with funds (UNB-XIN-SAS-007), legal sponsor disclosure must survive branding, CA Type E exposure for vendor | high |
| advisor-distribution | advisor delegation across many funds, custody/CRM integration, advisor fee compensation controls, partner federation | advisor roles, investment options per program, fee policy, app consent | Advisor compensation from fund assets (RCD-021), firm moves = partner exit, partner-held step-up (TVF-136) | medium |
| institutional | isolation profile by contract, SOC 1/2 reports, enterprise SSO, migration from legacy vendor, operator access transparency | isolation profile, SLA tier, entitlements, access review cadence | Profile C only if contract cites it, migration transition windows | medium |
| outsourced-admin | operator-under-sponsor-delegation, SoD across organizations, service catalog/SLAs | delegation of authority to operator roles, work queue routing | Operator acting outside sponsor DoA (TVF-134) — requires UNB-XIN-IAM-002 | high |
| embedded | widgets, partner APIs, webhooks, consent-based donor auth, HostContext | app registration, widget brand, subscription scopes | Host in funds flow (UNB-XIN-OTH-003), CA Type E exposure, webhook misread as settlement | medium |
| investment-centric | per-program investment lineup, multi-custody, NAV/unitization, fee transparency | investment options, custody adapters, fee base | None structural; fee base decisions (RCD-018) | medium |
| high-volume-low-balance | low cost-to-serve, batch payroll intake, automated STP, dormancy handling | minimums, fee floors, dormancy policy, notification throttles | Unknown unit economics without metering (UNB-XIN-SAS-009), human-review rules that do not scale, account-type classification (UNB-RCN-ENT-011) | medium |
| HNW-family-office | complex asset acceptance, family governance/succession, multi-participant roles, operator-access transparency | roles, approval quorum, custody adapters | None structural; Profile C only by contract | low |
| hybrid | multiple programs under one sponsor tenant, per-program entitlements and brand, sponsor-level consolidated books | program entities, per-program config, partner schemas inside sponsor DB | Only risky if tenant = partner (books split across databases, RCF-PLT-003); resolved if tenant = legal sponsor (RCD-004) | medium |
<!-- END GENERATED: models -->
