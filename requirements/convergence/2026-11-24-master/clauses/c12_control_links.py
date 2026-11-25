"""Internal-control classification of convergence clauses (ADV-T-011, ADV-R2-033).

Each entry marks a clause as an internal control and links it to the authoritative control register
(requirements/governance/MASTER-ENTERPRISE-CONTROL-MATRIX.csv). Fields:
  controls          UNB-CTRL ids (capability x {authority and segregation, evidence integrity, independent review})
  operator          who performs the control: Unbound | sponsor | administrator | rail partner | custodian | shared
  report            SOC1 | SOC2 | SOC1+SOC2 | none (expected relevance of a service-organization report)
  automated         True when performed by the platform without a person at run time
  cuec              True when the sponsor must operate a complementary user entity control for the control to work
"""

CONTROL_LINKS = {
    "CNV-FIN-001": {"controls": ["UNB-CTRL-025", "UNB-CTRL-043"], "operator": "Unbound", "report": "SOC1", "automated": True, "cuec": True},
    "CNV-FIN-003": {"controls": ["UNB-CTRL-052", "UNB-CTRL-053"], "operator": "shared", "report": "SOC1", "automated": True, "cuec": True},
    "CNV-FIN-004": {"controls": ["UNB-CTRL-046", "UNB-CTRL-047", "UNB-CTRL-048"], "operator": "shared", "report": "SOC1", "automated": False, "cuec": True},
    "CNV-FIN-005": {"controls": ["UNB-CTRL-037", "UNB-CTRL-038", "UNB-CTRL-039"], "operator": "shared", "report": "SOC1", "automated": True, "cuec": True},
    "CNV-FIN-011": {"controls": ["UNB-CTRL-043", "UNB-CTRL-044"], "operator": "sponsor", "report": "SOC1", "automated": False, "cuec": True},
    "CNV-PAY-001": {"controls": ["UNB-CTRL-049"], "operator": "Unbound", "report": "SOC1", "automated": True, "cuec": True},
    "CNV-PAY-002": {"controls": ["UNB-CTRL-031", "UNB-CTRL-032", "UNB-CTRL-033", "UNB-CTRL-049"], "operator": "shared", "report": "SOC1+SOC2", "automated": False, "cuec": True},
    "CNV-PAY-004": {"controls": ["UNB-CTRL-049", "UNB-CTRL-051"], "operator": "sponsor", "report": "SOC1", "automated": False, "cuec": True},
    "CNV-PAY-005": {"controls": ["UNB-CTRL-053"], "operator": "Unbound", "report": "SOC1", "automated": True, "cuec": False},
    "CNV-PAY-007": {"controls": ["UNB-CTRL-046", "UNB-CTRL-049"], "operator": "shared", "report": "SOC1", "automated": True, "cuec": True},
    "CNV-GRT-001": {"controls": ["UNB-CTRL-025", "UNB-CTRL-026", "UNB-CTRL-027"], "operator": "shared", "report": "SOC1", "automated": True, "cuec": True},
    "CNV-GRT-003": {"controls": ["UNB-CTRL-016", "UNB-CTRL-017"], "operator": "Unbound", "report": "SOC1", "automated": True, "cuec": False},
    "CNV-GRT-005": {"controls": ["UNB-CTRL-028", "UNB-CTRL-030"], "operator": "sponsor", "report": "none", "automated": False, "cuec": True},
    "CNV-GRT-006": {"controls": ["UNB-CTRL-016", "UNB-CTRL-018", "UNB-CTRL-028"], "operator": "shared", "report": "SOC1+SOC2", "automated": True, "cuec": True},
    "CNV-GRT-009": {"controls": ["UNB-CTRL-025", "UNB-CTRL-028"], "operator": "Unbound", "report": "SOC1", "automated": True, "cuec": False},
    "CNV-GRT-011": {"controls": ["UNB-CTRL-027", "UNB-CTRL-030"], "operator": "sponsor", "report": "SOC1", "automated": False, "cuec": True},
    "CNV-GOV-003": {"controls": ["UNB-CTRL-001", "UNB-CTRL-003", "UNB-CTRL-046"], "operator": "shared", "report": "SOC1", "automated": True, "cuec": True},
    "CNV-GOV-004": {"controls": ["UNB-CTRL-001", "UNB-CTRL-004"], "operator": "sponsor", "report": "SOC1", "automated": False, "cuec": True},
    "CNV-GOV-005": {"controls": ["UNB-CTRL-070", "UNB-CTRL-071", "UNB-CTRL-072"], "operator": "shared", "report": "SOC1+SOC2", "automated": False, "cuec": True},
    "CNV-GOV-010": {"controls": ["UNB-CTRL-001", "UNB-CTRL-064", "UNB-CTRL-067"], "operator": "Unbound", "report": "SOC1+SOC2", "automated": True, "cuec": False},
    "CNV-ARC-002": {"controls": ["UNB-CTRL-004", "UNB-CTRL-007", "UNB-CTRL-008"], "operator": "Unbound", "report": "SOC2", "automated": True, "cuec": True},
    "CNV-ARC-003": {"controls": ["UNB-CTRL-049", "UNB-CTRL-050"], "operator": "Unbound", "report": "SOC1", "automated": True, "cuec": False},
    "CNV-TEN-001": {"controls": ["UNB-CTRL-007", "UNB-CTRL-008", "UNB-CTRL-009"], "operator": "Unbound", "report": "SOC2", "automated": True, "cuec": False},
    "CNV-PLT-003": {"controls": ["UNB-CTRL-004", "UNB-CTRL-019"], "operator": "Unbound", "report": "SOC2", "automated": True, "cuec": False},
    "CNV-PLT-004": {"controls": ["UNB-CTRL-074", "UNB-CTRL-086"], "operator": "Unbound", "report": "SOC2", "automated": True, "cuec": True},
    "CNV-PLT-007": {"controls": ["UNB-CTRL-076", "UNB-CTRL-077", "UNB-CTRL-049"], "operator": "Unbound", "report": "SOC1+SOC2", "automated": False, "cuec": True},
    "CNV-PLT-009": {"controls": ["UNB-CTRL-074"], "operator": "Unbound", "report": "SOC1+SOC2", "automated": True, "cuec": False},
    "CNV-PLT-010": {"controls": ["UNB-CTRL-070", "UNB-CTRL-071", "UNB-CTRL-073"], "operator": "Unbound", "report": "SOC1+SOC2", "automated": True, "cuec": False},
    "CNV-PLT-011": {"controls": ["UNB-CTRL-019", "UNB-CTRL-020", "UNB-CTRL-031"], "operator": "Unbound", "report": "SOC2", "automated": True, "cuec": False},
    "CNV-PLT-012": {"controls": ["UNB-CTRL-082", "UNB-CTRL-049"], "operator": "shared", "report": "SOC1", "automated": False, "cuec": True},
    "CNV-AIO-001": {"controls": ["UNB-CTRL-064", "UNB-CTRL-065"], "operator": "Unbound", "report": "SOC2", "automated": True, "cuec": False},
    "CNV-AIO-002": {"controls": ["UNB-CTRL-064", "UNB-CTRL-066"], "operator": "Unbound", "report": "SOC2", "automated": True, "cuec": False},
    "CNV-ENT-001": {"controls": ["UNB-CTRL-022", "UNB-CTRL-023"], "operator": "sponsor", "report": "SOC1", "automated": False, "cuec": True},
    "CNV-ENT-002": {"controls": ["UNB-CTRL-007", "UNB-CTRL-079"], "operator": "shared", "report": "SOC2", "automated": False, "cuec": True},
}
