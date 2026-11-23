# Enterprise Document Classification and Retention Matrix
Date: 2026-11-23. Requirements only, no implementation or legal review.

## Governing rules
All financial documents and substantiating evidence have a **nonwaivable seven-year minimum**. Longer law, regulation, sponsor policy, contract or legal hold prevails. Seven years is not a universal statutory conclusion. Classify records by substance, not file format, channel or repository. A financial email, administrative note, call transcript or attachment inherits the financial floor. Multiple classes apply the longest obligation. Expiration means eligible for controlled review, **never automatic purge**. TBD classes cannot be deleted without an approved policy and legal/privacy review.

## Classification matrix
| ID | Class and examples | Minimum |
|---|---|---|
| FIN-01 | Fund statements and corrections | 7 years minimum |
| FIN-02 | Contributions and tax acknowledgments | 7 years minimum |
| FIN-03 | Grant decisions, correspondence and distributions | 7 years minimum |
| FIN-04 | Checks, ACH, wire, hand-delivery and payment evidence | 7 years minimum |
| FIN-05 | Ledger, valuation, custody, investment and reconciliation | 7 years minimum |
| FIN-06 | Fees, invoices and financial adjustments | 7 years minimum |
| FIN-07 | Financial audit workpapers and supporting attachments | 7 years minimum |
| FIN-08 | Financially material emails, notes, SMS and calls | 7 years minimum |
| GOV-01 | Board minutes, governance and delegation | TBD legal/compliance |
| GOV-02 | Fund/account agreements and succession | TBD; financial evidence 7-year floor |
| LEG-01 | Legal matters, disputes and holds | TBD; holds override |
| CMP-01 | Charity vetting, sanctions and investigations | TBD; financial evidence 7-year floor |
| COM-01 | Routine operational correspondence | TBD legal/compliance |
| COM-02 | Transactional notices and delivery evidence | TBD; financial evidence 7-year floor |
| COM-03 | Marketing, consent and unsubscribe records | TBD privacy/legal |
| ADM-01 | Administrative notes and attachments | TBD; financial evidence 7-year floor |
| TEL-01 | IVR logs, call metadata and recordings | TBD; financial evidence 7-year floor |
| SEC-01 | Access, security and export audit logs | TBD security/legal |
| TEC-01 | Telemetry, diagnostics and traces | TBD security/privacy |
| SYS-01 | Template, branding and disclosure revisions | Retain while linked financial artifacts require |
| SYS-02 | Artifact hashes, manifests and provenance | Retain while linked financial artifacts require |
| HR-01 | Personnel and contractor records | TBD HR/legal |
| VND-01 | Vendor contracts, processor and billing evidence | TBD; financial billing 7-year floor |
| TMP-01 | Unissued drafts, previews and temporary uploads | TBD short-lived policy; preserve evidence/holds |
| BAK-01 | Backups and replicas | Separate approved backup policy; archive floor unaffected |

## Policy record and retention computation
Each class must define stable ID, inclusion/exclusion rules, source component, responsible owner, sponsor and jurisdiction, confidentiality, authorized viewers, approved minimum, trigger date, legal basis, hold propagation, artifact immutability, archival format, disposition approvals and versioned effective dates. Calculate the longest applicable requirement, using a counsel-approved per-class trigger (e.g. issue year, accounting close, last correction, matter resolution), never one universal file creation timestamp. Preserve prior calculation and policy history. Reclassification must be audited and cannot shorten established financial retention without independent documented legal authority.

## Classification workflow
Automatically classify known financial workflow artifacts and source evidence. Attachments, emails, notes, calls, correspondence, provider records and generated PDFs may have multiple classifications. Linkage to financial transactions creates durable evidentiary obligations even if a link is later removed. Deterministic rules should cover straightforward cases; AI may suggest classes for ambiguous records but may never authorize deletion or shorter retention. Unknown classifications require conservative preservation and review. Keep financial source records and issued documents separately traceable.

## Holds, archival and disposition
Legal/compliance holds may cover sponsors, funds, people, cases, periods and classes, including existing and future matching records. Holds override expiration and sponsor offboarding. Release requires authorized independent review. After retention eligibility, inspect overlapping classes, unresolved disputes, holds, legal/privacy requirements, linked provenance, backups and third-party copies. Use documented two-person approval for high-risk financial disposition, with deletion evidence and retained minimal tombstones when lawful. No ordinary administrator may bypass holds or the seven-year floor. Fund closure, donor death, advisor changes and sponsor termination do not erase history; access revocation affects visibility, not preservation.

## Architecture and UI
The shared Document Artifact Registry retains exact issued bytes, hashes and provenance. Communications, IVR, Administrative Notes, Fund Accounting, Grants, Contributions, Compliance and security components attach classifications through typed in-process contracts within the component monolith. Folio/Typst and brand/template assets needed to interpret financial documents must survive as long as dependent artifacts. Document Center displays authorized history; Retention Console shows class, trigger, policy version, hold, eligible-review date, archive health, integrity and approved disposition. Protect derived search indexes, thumbnails, exports, AI summaries and backups from retention or privacy-policy bypass.

## Adversarial scenarios, proposed and unexecuted
- ADV-355: Financial grant email mislabeled marketing; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-356: Financial note lacks FIN classification; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-357: Sponsor sets financial retention to 3 years; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-358: Multiple classes conflict; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-359: Attachment loses financial lineage; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-360: Hold added immediately before disposal; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-361: Hold propagates to future revisions; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-362: AI suggests short retention; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-363: TBD class sent for deletion; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-364: Clock starts at draft rather than issue; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-365: Corrected original prematurely destroyed; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-366: Historic template asset removed; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-367: Backup prematurely purges archive; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-368: Sponsor offboarding erases metadata; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-369: Access revocation treated as destruction; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-370: Privacy request conflicts with legal preservation; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-371: Policy migration shortens expiry; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-372: Two-person approval bypassed; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-373: Cross-sponsor record misclassified; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-374: Unresolved litigation record disposed; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-375: Search index survives lawful deletion; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-376: IVR transcript documents payment decision; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-377: Tax acknowledgment uses wrong trigger; expected: preserve applicable retention, enforce authorization and audit exception.
- ADV-378: Hold released without authority; expected: preserve applicable retention, enforce authorization and audit exception.

## Unresolved decisions
Counsel/compliance must validate per-class periods, jurisdictional triggers, nonprofit/tax/AML/privacy/recording requirements, permanent governance archives, privacy deletion rights, provider contractual obligations, backup retention, and legal hold handling. Do not treat the proposed TBD periods as permission to delete.
