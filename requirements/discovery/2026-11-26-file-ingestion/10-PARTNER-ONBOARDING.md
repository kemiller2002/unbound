# 10 — Partner onboarding requirements

Status: **proposed**. Requirements UNB-ING-ONB-001..002, UNB-ING-RCN-003, UNB-ING-AI-004, UNB-ING-PRF-002.

## 1. Pipeline

| Step | Output (recorded evidence) | AI assistance | Control |
|---|---|---|---|
| 1. Receive samples and documentation | Evidence artifacts in an integration workspace (FIW-PLT-01) | — | Same inspection as any artifact; synthetic or contractually permitted samples only |
| 2. Inspect and classify formats | Format, encoding, record types, control-total presence | Format classification | Engineer confirms |
| 3. Match existing profiles | Candidate profiles with match score per field | Schema matching | — |
| 4. Propose mappings | Draft profile version (catalog transforms only) | Mapping proposals with sample evidence | Field-by-field review |
| 5. Validate against samples | Per-sample run report | Explain mismatches | Zero unexplained rejects |
| 6. Synthetic and historical scenarios | Corpus runs incl. edge cases generated from the partner's spec | Test-case generation | Corpus passes |
| 7. Approvals | Profile approval (engineering under CNV-PLT-010 for new catalog entries); sponsor activation request | — | Two approvers for control-relevant changes |
| 8. Calibration | Live processing at zero tolerance with 100% human review | — | At least one month-end reconciled (ProvisionalCalibration, CNV-FIN-004); a mandatory bank feed is never left in a non-producing shadow, because CNV-PAY-007 needs a verified balance |
| 9. Reconcile | Shadow results reconciled to an independent control | Break explanations | No unexplained breaks |
| 10. Activate | Profile *active*; expectation calendar live | — | Activation refused if any step lacks evidence |
| 11. Monitor | Drift holds, lateness, error share | Drift explanation | Exceptions only |

## 2. Marginal cost of another institution

From the daf-landscape onboarding model (assumptions, not measurements; anchored on the 8–12 week elapsed time of
traditional EDI partner onboarding, FIE-B-021):

| Case | Engineer-days (low / mode / high) | Cost at loaded rates |
|---|---|---|
| Bespoke integration (no shared infrastructure) | 15 / 30 / 50 | ≈ $9k / $25k / $71k |
| New platform profile for an unseen format | 8 / 15 / 25 | ≈ $5k / $12k / $36k |
| Institution matching an existing profile | 1 / 3 / 6 | ≈ $0.6k / $2.5k / $8.6k |

Shadow processing adds 10–40 business days of elapsed time but little effort. The economics point is the third row:
once the canonical bank-transaction model and the first BAI2 or camt.053 profile exist, the second bank using the same
standard is a configuration exercise. Custodian feeds are proprietary, so each new custodian is a "new profile" case.

## 3. What onboarding must not become

No bespoke code paths per partner, no partner-specific business rules in adapters, no activation without a shadow
month-end, and no AI in the recurring path after activation.
