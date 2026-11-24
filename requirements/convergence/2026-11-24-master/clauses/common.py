"""Shared constructor for convergence clauses (pure)."""

CLASSIFICATIONS = ("normative", "capability-index", "research-hypothesis", "design-alternative", "deferred",
                   "experimental-default", "implementation-note", "evidence-backlog")
RELEASE_SCOPES = ("architectural-support", "initial-implementation", "sponsor-activation", "commercial-availability",
                  "regulatory-authorization", "deferred")
DOMAINS = {"FIN": "requirements/FULL-ACCOUNTING-SYSTEM.md",
           "PAY": "requirements/DEFERRED-PAYMENTS-AND-REMAINING-GAPS.md",
           "GRT": "requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md",
           "LEG": "requirements/charities/JURISDICTION-AWARE-REGULATORY-RULES-AND-ELIGIBILITY.md",
           "ENT": "requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md",
           "ADM": "requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md",
           "PLT": "requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md",
           "TEN": "requirements/PARTNER-SEGREGATION-AND-TENANCY.md",
           "GOV": "requirements/governance/SPONSOR-AUTHORITY-AND-AUTONOMOUS-OPERATIONS.md",
           "INV": "requirements/investments/ACCOUNT-STRUCTURES-AND-PORTFOLIO-MANAGEMENT.md",
           "ECO": "requirements/EXPANSION-ROADMAP-AND-GATES.md",
           "EXP": "requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md",
           "AIO": "requirements/ai/AI-NATIVE-INTERACTION-AND-AGENT-COLLABORATION.md"}
OPERATING_MODELS = ("E1", "E2", "E3", "E4", "E5")


def OWN(domain, financial_sor, policy, data, operational, evidence):
    return {"domain_owner": domain, "financial_system_of_record": financial_sor, "policy_owner": policy,
            "data_owner": data, "operational_owner": operational, "evidence_owner": evidence}


def clause(cid, title, statement, acceptance, resolves, amends, domain, scope, gate, owners, evidence,
           classification="normative", models=OPERATING_MODELS, dependencies=(), activation=None):
    assert domain in DOMAINS, domain
    assert scope in RELEASE_SCOPES, scope
    assert classification in CLASSIFICATIONS, classification
    assert "SHALL" in statement or classification != "normative", cid
    assert acceptance, cid
    finding = lambda i: i.startswith(("RCF-", "RCC-", "OB-", "OD-"))
    return {"id": cid, "title": title, "statement": " ".join(statement.split()), "acceptance_criteria": list(acceptance),
            "resolves": list(resolves), "amends": [i for i in amends if not finding(i)],
            "related_findings": [i for i in amends if finding(i)], "domain": domain, "target_file": DOMAINS[domain],
            "release_scope": scope, "activation_gate": gate, "owners": owners, "evidence_batches": list(evidence),
            "classification": classification, "operating_models": list(models), "dependencies": list(dependencies),
            "activation_conditions": activation or (gate if gate else "none beyond release scope")}
