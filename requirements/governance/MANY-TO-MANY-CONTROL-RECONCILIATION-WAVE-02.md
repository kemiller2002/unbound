# Detailed many-to-many control reconciliation, wave 2
Date: 2026-11-21. **Candidate mapping only; no implementation or independent verification.**

## Scope
Read 15 actual requirement documents from the connected Unbound GitHub repository. Extracted 494 unique requirement IDs and produced 1893 candidate many-to-many links to the 46 specific controls (maximum four candidates per requirement). 0 requirements had no candidate. 2 detailed controls received no candidate from this inspected subset: UNB-SCTRL-029, UNB-SCTRL-030.

## Method and limitations
Mappings were derived by inspecting each source requirement line, matching relevant concepts, and adding domain-default candidates. This is a *triage mapping*, not a verified control-objective match. Some links may be false positives and some missing links may be material. Every CSV row explicitly says 'Proposed, not validated'. The source excerpt is retained for human review. Multiple controls per requirement are expected and desirable, but they do not prove full coverage.

## Most important review questions
1. Does the control actually prevent or detect the specific risk in the requirement?
2. Does the event prove the control operated, or merely record that an action was attempted?
3. Is the evidence generated independently enough to support the conclusion?
4. Can the negative test show enforcement and preserve evidence of denial?
5. Are the owner and reviewer independent and assigned, not merely role placeholders?
6. Does this requirement also require a DAF fiduciary, legal, tax or accounting assertion beyond SOC 2?
7. What happens when the evidence is absent, stale, contradictory or restricted?

## Research regulatory assertions
Initial GitHub code searches for literal REG-001 and CTL-001 in kemiller2002/daf-landscape returned no matching files. **No claim of direct REG/CTL reconciliation is made.** The earlier wave summary indicates 65 regulatory assertions and 54 controls, but their source records must be located and inspected before exact crosswalks can be committed. Do not fabricate citations, control texts or legal conclusions.

## Priority remediation backlog
- Validate sponsor approval, AI authority and separation-of-duties mappings first.
- Validate charity identity, legal eligibility, sanctions and first-party source evidence.
- Validate accounting, NAV, custody, independent reconciliation and deferred payment controls.
- Add real evidence producers, reviewers, retention, monitoring and positive/negative tests.
- Search and inspect remaining requirements and regulatory research files; deduplicate by meaning.
- Confirm SOC 2 criteria with independent CPA and DAF-specific legal rules with counsel.

## Assurance status
494 requirements inspected; 1893 proposed links; **0 independently validated mappings**, **0 executed negative tests**, **0 operating control evidence verified**, **0 auditor-reviewed controls**. A mapping is not proof of compliance.
