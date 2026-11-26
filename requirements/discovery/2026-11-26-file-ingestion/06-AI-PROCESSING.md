# 06 — AI processing requirements

Status: **proposed**. Requirements UNB-ING-AI-001..006; governing clauses CNV-AIO-001, CNV-AIO-002, UNB-XIN-SVC-009.

## 1. Where AI provides measurable value — and where it does not

| Use | Value | Execution mode | Measured? |
|---|---|---|---|
| Profile authoring for a new institution (format classification, schema discovery, mapping proposals, sample-based tests) | High: replaces most analyst time in onboarding | AI-assisted configuration; execution deterministic after review | No — estimate (economics model: onboarding days) |
| Migration mapping and anomaly detection | High: predecessor schemas are undocumented | AI-assisted configuration | No |
| Evidence documents (appraisals, death certificates, trust certifications, determination letters for exceptions, bank letters, fiscal-sponsorship agreements) | Medium–high: pre-filled review with citations | Recurring AI proposals, always corroborated, human decides | No public benchmark (FIE-C-048); evaluation sets required (UNB-ING-AI-005) |
| Correspondence and scanned-mail classification and routing | High at volume | Recurring AI classification; routing only | Vendor accuracy claims only |
| Exception explanation and reconciliation investigation | Medium: suggested causes and candidates | Suggestion; human or deterministic rule decides | No |
| Bank, custodian, IRS, OFAC, bulk grant, migration *data* files | **None** | Deterministic parsing | — |
| Checks (lockbox) | Low: the bank's lockbox service already keys amounts; AI only reads remittance letters | Proposals | Prebuilt check model exists ($10/1,000 pages, FIE-C-042) |

The rule of thumb the matrix applies: **AI where the input is unstructured and a human decision follows anyway; never in
the path that turns structured records into financial effects.**

**Release-1 scope** (revised after review R2-17): AI extraction is off by default and absent from the appraisal profile;
the release-1 AI value is configuration-time assistance for partner onboarding and migration mapping on synthetic or
contractually permitted samples. Extraction proposals for death certificates, correspondence and determination-letter
exceptions are available only to sponsors that enable them under an approved provider policy, and each purpose carries a
kill criterion: it is switched off if measured reviewer minutes saved fall below a published threshold (UNB-ING-AI-005).

## 2. Controls

- **Untrusted content.** One document, a fixed task, no tools, no retrieval, no other records, strict output schema;
  extraction runs on the same raster the reviewer sees; the injection flag comes from deterministic detectors
  (instruction patterns, hidden or off-page text, raster/text disagreement) as well as the model and is never the only
  control (UNB-ING-AI-001; OWASP LLM01, FIE-C-049; EchoLeak, FIE-C-050).
- **Minimisation.** Fields not needed for the purpose (e.g. SSNs) are masked before disclosure, and a disclosure log
  records what was sent (UNB-ING-AI-007).
- **Proposals, not facts.** Every value carries provenance (artifact hash, page, region, model and prompt version) and
  must be corroborated by a source independent of the submitter (IRS data, routing checksum plus account validation,
  bank or custodian observations). Comparing two documents from the same donor is a labelled consistency check, not
  corroboration. Pre-fill is gated on measured precision as well as recall; confidence scores never establish financial
  correctness (UNB-ING-AI-002).
- **No authority.** AI cannot authorize transactions, override deterministic controls, change profiles or policies, or
  treat document text as instructions (CNV-AIO-001). Correspondence requests that would change accounts or payment
  destinations become proposed actions an authenticated principal confirms (UNB-ING-AI-006).
- **Disclosure.** Documents go only to providers under an approved policy (no-training and retention terms, in-region or
  zero-data-retention, subprocessors listed), enabled per sponsor; with no policy, AI is off and deterministic paths are
  unaffected (UNB-ING-AI-003; decision FIL-OD-04).
- **Measurement.** Each extraction purpose has a labelled synthetic evaluation set, published precision/recall and a
  threshold for pre-filling versus leaving blank; a model or prompt change that lowers recall is blocked
  (UNB-ING-AI-005).

## 3. Confidence thresholds, review and escalation

| Corroboration result | Presentation | Human action |
|---|---|---|
| Corroborated by an authoritative source | Pre-filled, marked "corroborated" with the source | Confirm (one action) |
| Not corroborable (no independent source for the field) | Pre-filled only if field precision and recall meet the purpose's published thresholds; otherwise blank | Confirm or enter |
| Contradicted by an authoritative source | Shown with the contradiction; cannot be accepted without an override reason | Decide; escalates to specialist |
| Injection signal present | Shown with region highlighted | Review whole document; security owner notified on repeat |

## 4. Cost

Per-page prices measured 2026-11-26: OCR $1.50 per 1,000 pages on AWS and Azure; prebuilt models (US check, bank
statement) $10; custom extraction $30 (FIE-C-041/042). LLM extraction is estimated at roughly $0.50–$10 per 1,000 pages
depending on model size (FIE-C-045, inferred, to be verified). At the volumes in the economics model, AI extraction is
a rounding error next to reviewer time; its value must be measured in reviewer minutes saved, not provider cost.
