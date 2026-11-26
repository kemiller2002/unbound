/// Error paths of every validator's gates (pure functions over in-memory records).
module Unbound.ReqAudit.Tests.GateTests

open Xunit
open Unbound.Py.PyJson
open Unbound.ReqAudit
open Unbound.ReqAudit.PyCompat
open Unbound.ReqAudit.Validate

let private o (kvs: (string * Json) list) = JObj kvs

/// dict(base, **extra): extra keys replace base keys in place, new keys are appended.
let private withKeys (baseKvs: (string * Json) list) (extra: (string * Json) list) =
    JObj((baseKvs |> List.map (fun (k, v) -> k, (match List.tryFind (fst >> (=) k) extra with Some(_, x) -> x | None -> v)))
         @ (extra |> List.filter (fun (k, _) -> not (List.exists (fst >> (=) k) baseKvs))))
let private js = JStr
let private arr (xs: string list) = JArr(xs |> List.map JStr)
let private lines (issues: Issue list) = issues |> List.map formatIssue

let private ok = resolver (set [ "UNB-GRT-001"; "UNB-GRT-002" ]) (set [ "RCF-GRT-001" ]) (set [ "NARR-1234abcd" ]) Set.empty

// ---------------------------------------------------------------- validate.py

[<Fact>]
let ``resolver accepts families, qualified ids and trailing dots`` () =
    Assert.True(ok "UNB-GRT-*")
    Assert.True(ok "UNB-GRT-001@other.md")
    Assert.True(ok " UNB-GRT-002. ")
    Assert.False(ok "UNB-FIN-*")
    Assert.False(ok "UNB-GRT-003")

[<Fact>]
let ``G1 reports identifiers defined more than once in insertion order`` () =
    Assert.Equal<string list>(
        [ "ERROR G1   B-001: identifier defined 2 times across reconciliation records"; "ERROR G1   A-001: identifier defined 3 times across reconciliation records" ],
        lines (gDupes [ "x", [ "B-001"; "A-001"; "B-001" ]; "y", [ "A-001"; "A-001" ] ]))

[<Fact>]
let ``G2 flags baseline collisions and non-conforming new ids`` () =
    Assert.Equal<string list>(
        [ "ERROR G2   UNB-GRT-001: new identifier collides with a baseline identifier"
          "ERROR G2   UNB-GRT-001: new identifier does not follow a reconciliation ID scheme" ],
        lines (gCollide (set [ "UNB-GRT-001"; "RCF-GRT-009" ]) (set [ "UNB-GRT-001" ])))

[<Fact>]
let ``G3, G4 and G9 check a refinement against the baseline file`` () =
    let refRec =
        o [ "refinement_id", js "RR-UNB-GRT-009-r1"; "original_id", js "UNB-GRT-009"; "original_path", js "a.md"
            "original_text", js "**Grants** must clear"; "relationship", js "replaces" ]
    let files = function "a.md" -> "- grants must clear sanctions" | _ -> ""
    Assert.Equal<string list>(
        [ "ERROR G3   RR-UNB-GRT-009-r1: original_id UNB-GRT-009 not in baseline inventory"
          "ERROR G9   RR-UNB-GRT-009-r1: relationship replaces invalid for a refinement" ],
        lines (gRefinements files [ refRec ] (set [ "UNB-GRT-001" ])))
    let missing = o [ "refinement_id", js "RR-X-1-r1"; "original_id", js "X-1"; "original_path", js "b.md"; "relationship", js "refines" ]
    Assert.Equal<string list>(
        [ "ERROR G3   RR-X-1-r1: original_id X-1 not in baseline inventory"; "ERROR G4   RR-X-1-r1: original_path b.md not found at 777a70d" ],
        lines (gRefinements files [ missing ] Set.empty))

[<Fact>]
let ``G5 treats null, empty strings, lists and dicts as missing`` () =
    let r = o [ "conflict_id", js "RCC-A-001"; "requirement_a", JNull; "requirement_b", JArr []; "incompatibility", JObj []; "recommended_resolution", jint 0 ]
    Assert.Equal<string list>(
        [ "ERROR G5   RCC-A-001: conflicts: missing or empty field 'requirement_a'"
          "ERROR G5   RCC-A-001: conflicts: missing or empty field 'requirement_b'"
          "ERROR G5   RCC-A-001: conflicts: missing or empty field 'incompatibility'" ],
        lines (gRequired "conflicts" [ r ] "conflict_id"))

[<Fact>]
let ``G6 prints the criterion repr truncated to 60 characters`` () =
    let r = o [ "id", js "G-1"; "acceptance_criteria", JArr [ o [ "id", js "AC-1"; "given", js ""; "when", js "the long trigger happens"; "then", js "x" ]; js "free text" ] ]
    Assert.Equal<string list>(
        [ "ERROR G6   G-1: acceptance criterion {'id': 'AC-1', 'given': '', 'when': 'the long trigger happen lacks given/when/then"
          "ERROR G6   G-1: acceptance criterion 'free text' lacks given/when/then" ],
        lines (gAcceptance [ r ] "id"))

[<Fact>]
let ``G7 requires gap-analysis origin and a registered finding`` () =
    let g = o [ "id", js "UNB-RCN-GRT-001"; "origin", js "analysis"; "gap_finding", js "RCF-GRT-404" ]
    Assert.Equal<string list>(
        [ "ERROR G7   UNB-RCN-GRT-001: origin must be gap-analysis"; "ERROR G7   UNB-RCN-GRT-001: gap_finding RCF-GRT-404 not in findings register" ],
        lines (gGaps [ g ] (set [ "RCF-GRT-001" ])))

[<Fact>]
let ``G8 ignores non-identifiers and reports unresolved references`` () =
    Assert.Equal<string list>(
        [ "ERROR G8   S-1: links: unresolved reference 'UNB-FIN-001'" ],
        lines (gRefs "links" "S-1" [ "UNB-GRT-001"; "free text"; " "; "UNB-FIN-001" ] ok))

[<Fact>]
let ``G9 checks edge vocabulary and endpoints`` () =
    let e = o [ "source", js "UNB-GRT-001"; "relationship", js "blocks"; "target", js "UNB-FIN-001"; "provenance", js "p.csv" ]
    Assert.Equal<string list>(
        [ "ERROR G9   UNB-GRT-001->UNB-FIN-001: relationship 'blocks' not in vocabulary"
          "ERROR G9   UNB-GRT-001->UNB-FIN-001: unresolved endpoint (p.csv)" ],
        lines (gEdges [ e ] ok))

[<Fact>]
let ``G10 lists unknown cited ids once each in sorted order`` () =
    Assert.Equal<string list>(
        [ "ERROR G10  r.md: cites unknown identifier RCD-002"; "ERROR G10  r.md: cites unknown identifier RCF-GRT-002" ],
        lines (gReports [ "r.md", "RCF-GRT-002 and RCD-002, RCF-GRT-001, RCD-002" ] ok))

[<Fact>]
let ``G11 warns and G12 checks component snapshots`` () =
    Assert.Equal<string list>([ "WARN  G11  RR-A-1-r1: no tested_by scenario" ], lines (gTested [ "RR-A-1-r1" ] []))
    let u = o [ "id", js "UNB-UI-S-001"; "forma_components", arr [ "Card"; "Ghost" ]; "folio_elements", arr [ "Nope" ]; "capability_gaps", arr [ "GAP-9" ] ]
    Assert.Equal<string list>(
        [ "ERROR G12  UNB-UI-S-001: Forma component 'Ghost' not in catalog snapshot"
          "ERROR G12  UNB-UI-S-001: Folio element 'Nope' not in element snapshot"
          "ERROR G12  UNB-UI-S-001: capability gap 'GAP-9' not registered" ],
        lines (gUi [ u ] (set [ "Card" ]) Set.empty Set.empty))

[<Fact>]
let ``summary counts errors and warnings by gate in first-seen order`` () =
    let issues = [ err "G9" "a" "m"; issue "WARN" "G11" "b" "m"; err "G1" "c" "m"; err "G9" "d" "m" ]
    Assert.Equal(
        """{"records": {"x": 2}, "trace_edges": 5, "errors": 3, "warnings": 1, "errors_by_gate": {"G9": 2, "G1": 1}}""",
        dumps (summaryJson [ "x", 2 ] [ "trace_edges", jint 5 ] issues))

// ---------------------------------------------------------------- validate_discovery.py

module VD = ValidateDiscovery

[<Fact>]
let ``D1 enumerations print the value repr and the sorted allowed list`` () =
    Assert.Equal<string list>(
        [ "ERROR D1   XCAP-A-1: coverage='sometimes' not in ['contradictory', 'covered', 'missing', 'partial']"
          "ERROR D1   ?: coverage=None not in ['contradictory', 'covered', 'missing', 'partial']" ],
        lines (VD.gEnum "D1" [ o [ "id", js "XCAP-A-1"; "coverage", js "sometimes" ]; o [ "coverage", JNull ] ] "id" "coverage" VD.COVERAGE))

[<Fact>]
let ``D4 rejects AI observations in ledgers or synchronous paths`` () =
    let e = o [ "id", js "XEV-001"; "authority", js "observation-ai"; "source_of_truth_store", js "General Ledger"; "consistency", js "synchronous" ]
    Assert.Equal<string list>(
        [ "ERROR D4   XEV-001: no owning module"
          "ERROR D4   XEV-001: AI observation declared as stored in an authoritative ledger"
          "ERROR D4   XEV-001: AI observation declared synchronous-consistent" ],
        lines (VD.gEvents [ e ]))
    Assert.Equal<string list>(
        [ "ERROR D4   XEV-002: authority 'guess' not enumerated" ],
        lines (VD.gEvents [ o [ "id", js "XEV-002"; "authority", js "guess"; "owning_module", js "m" ] ]))

[<Fact>]
let ``D7 is an error and D8 reports the clone state`` () =
    Assert.Equal<string list>([ "ERROR D7   UNB-XIN-A-1: no tested_by scenario" ], lines (VD.gTestedStrict [ "UNB-XIN-A-1" ] []))
    Assert.Equal<string list>([ "WARN  D8   daf-landscape: clone not found beside repository; unchanged-state not checked" ], lines (VD.gLandscape None))
    Assert.Equal<string list>([ "ERROR D8   daf-landscape: working tree has changes" ], lines (VD.gLandscape (Some false)))
    Assert.Empty(VD.gLandscape (Some true))

[<Fact>]
let ``D10 outcomes must match an existing decision and carry their fields`` () =
    let decs = [ o [ "decision_id", js "XID-A-001"; "status", js "decided" ] ]
    let outcomes =
        [ o [ "decision_id", js "XID-A-001"; "status", js "open"; "decision", js "d"; "rationale", js "r"; "authority", arr [ "x" ]; "confirmation_required", JBool false; "confidence", js "low" ]
          o [ "decision_id", js "XID-A-404"; "decision", js "d" ] ]
    Assert.Equal<string list>(
        [ "ERROR D10  XID-A-001: decisions.csv status 'decided' disagrees with outcome 'open'"
          "ERROR D10  XID-A-404: outcome for an unknown decision"
          "ERROR D10  XID-A-404: missing or empty field 'rationale'"
          "ERROR D10  XID-A-404: missing or empty field 'authority'"
          "ERROR D10  XID-A-404: missing or empty field 'confirmation_required'"
          "ERROR D10  XID-A-404: missing or empty field 'confidence'" ],
        lines (VD.gOutcomes outcomes decs))

[<Fact>]
let ``reference lists accept lists or free text`` () =
    let r = o [ "id", js "XFC-A-001"; "covered_by", js "UNB-GRT-001, UNB-FIN-009"; "new_ids", arr [ "UNB-FIN-002" ] ]
    Assert.Equal<string list>(
        [ "ERROR D6   XFC-A-001: covered_by: unresolved reference 'UNB-FIN-009'"; "ERROR D6   XFC-A-001: new_ids: unresolved reference 'UNB-FIN-002'" ],
        lines (VD.gRefsList "D6" [ r ] "id" [ "covered_by"; "new_ids" ] ok))

[<Fact>]
let ``G4 for a CSV-defined original compares against the rebuilt row`` () =
    let r = o [ "refinement_id", js "RR-UNB-GRT-001-r1"; "original_id", js "UNB-GRT-001"; "original_path", js "t.csv"; "original_line", jint 2
                "original_text", js "UNB-GRT-001 | Grants clear"; "relationship", js "refines" ]
    let files = function "t.csv" -> "id,text\nUNB-GRT-001,Grants clear\n" | _ -> ""
    Assert.Empty(VD.gRefinementsCsvAware files [ r ] (set [ "UNB-GRT-001" ]))
    Assert.Equal<string list>(
        [ "ERROR G4   RR-UNB-GRT-001-r1: original_text not found verbatim (first 160 normalized chars) in original file" ],
        lines (gRefinements files [ r ] (set [ "UNB-GRT-001" ])))

// ---------------------------------------------------------------- validate_simplification.py

module VS = ValidateSimplification

let private regRow id disp extra =
    withKeys
        [ "id", js id; "disposition", js disp; "original_purpose", js "p"; "reasoning", js "r"; "confidence", js "c"
          "human_work", o [ "level", js "low" ]; "customer_friction", o [ "level", js "none" ]; "configuration_burden", o [ "level", js "high" ] ]
        extra

[<Fact>]
let ``S1 to S3 check coverage, vocabularies and removal evidence`` () =
    let reg =
        [ regRow "A-1" "maybe" [ "convention_origin", arr [ "none"; "habit" ] ]
          regRow "A-1" "recommend-removal" [ "human_work", JNull ]
          regRow "Z-9" "retain" [] ]
    Assert.Equal<string list>(
        [ "ERROR S1   B-2: requirement not reviewed in the register"
          "ERROR S1   Z-9: register row for an identifier outside the review universe"
          "ERROR S1   A-1: reviewed 2 times"
          "ERROR S2   A-1: disposition 'maybe'"
          "ERROR S2   A-1: convention_origin ['habit']"
          "ERROR S2   A-1: human_work.level None"
          "ERROR S3   A-1: removal recommended without elimination_consequence"
          "ERROR S3   A-1: removal recommended without evidence_to_change" ],
        lines (VS.gRegister reg (set [ "A-1"; "B-2" ])))

[<Fact>]
let ``S7 follows consolidation chains`` () =
    let reg =
        [ regRow "C-1" "consolidate" [ "consolidate_with", arr [ "C-2" ] ]
          regRow "C-2" "consolidate" [ "consolidate_with", arr [ "C-1" ] ]
          regRow "C-3" "consolidate" [ "consolidate_with", arr [ "C-3" ] ]
          regRow "C-4" "consolidate" [ "consolidate_with", arr [ "R-1" ] ]
          regRow "C-5" "consolidate" [ "consolidate_with", arr [ "D-1" ] ]
          regRow "C-6" "consolidate" [ "consolidate_with", arr [ "OUTSIDE-1" ] ]
          regRow "R-1" "recommend-removal" []
          regRow "D-1" "defer" [] ]
    Assert.Equal<string list>(
        [ "ERROR S7   C-1: consolidation chain does not reach a retained carrier: cycle C-1 -> C-2 -> C-1"
          "ERROR S7   C-2: consolidation chain does not reach a retained carrier: cycle C-2 -> C-1 -> C-2"
          "ERROR S7   C-3: consolidation chain does not reach a retained carrier: C-3 (no target)"
          "ERROR S7   C-4: consolidation chain does not reach a retained carrier: R-1 (recommend-removal)"
          "WARN  S7   C-5: obligation is carried by a deferred requirement (D-1); it is deferred with that capability" ],
        lines (VS.gChains reg))

[<Fact>]
let ``S4 requires decision fields for kept metrics and a reason for removed ones`` () =
    Assert.Equal<string list>(
        [ "ERROR S4   SPM-001: missing decision_supported"; "ERROR S4   SPM-001: missing expected_action"; "ERROR S4   SPM-001: missing authoritative_source" ],
        lines (VS.gMetrics [ o [ "id", js "SPM-001"; "keep", js "yes"; "definition", js "d" ] ]))
    Assert.Empty(VS.gMetrics [ o [ "id", js "SPM-002"; "keep", js "no: vanity metric" ] ])

// ---------------------------------------------------------------- workflows.py

let private state id kind = o [ "id", js id; "kind", js kind; "user_label", js id; "lozenge", js "ok" ]

let private tr id from to' extra =
    withKeys [ "id", js id; "from", js from; "to", js to'; "actor_type", js "deterministic"; "authority", js "A1"; "requirement_ids", arr [ "UNB-GRT-001" ] ] extra

let private wlines (issues: Workflows.Issue list) = issues |> List.map (fun i -> $"{i.Level} {i.Check} {i.Subject}: {i.Message}")

[<Fact>]
let ``W1 to W4 check structure`` () =
    let w =
        o [ "workflow_id", js "WFM-T"
            "states", JArr [ state "s0" "initial"; state "s1" "normal"; state "trap" "normal"; state "dead" "normal"; state "end" "terminal"; state "lost" "normal" ]
            "transitions", JArr [ tr "T1" "s0" "s1" []; tr "T1" "s1" "end" []; tr "T2" "s0" "trap" []; tr "T3" "trap" "trap" []; tr "T4" "end" "s1" []
                                  tr "T5" "s0" "dead" []; tr "T6" "s0" "nowhere" [] ] ]
    Assert.Equal<string list>(
        [ "ERROR W1 WFM-T: duplicate transition id T1"
          "ERROR W1 T6: unknown state 'nowhere'"
          "ERROR W2 WFM-T:lost: state unreachable from initial"
          "ERROR W3 WFM-T:trap: no path to any terminal state (trap)"
          "ERROR W3 WFM-T:dead: non-terminal state has no outgoing transition (dead end)"
          "ERROR W4 WFM-T:end: terminal state transitions to non-correction state(s) ['s1']"
          "ERROR W3 WFM-T:lost: non-terminal state has no outgoing transition (dead end)" ],
        wlines (Workflows.checkStructure w))
    let twoInits = o [ "workflow_id", js "WFM-U"; "states", JArr [ state "a" "initial"; state "b" "initial" ]; "transitions", JArr [] ]
    Assert.Equal<string list>([ "ERROR W2 WFM-U: expected exactly one initial state, found 2" ], wlines (Workflows.checkStructure twoInits))

[<Fact>]
let ``W5 to W10 check each transition`` () =
    let w =
        o [ "workflow_id", js "WFM-T"
            "transitions",
            JArr [ tr "T1" "a" "b" [ "actor_type", js "ai"; "authority", js "A3" ]
                   tr "T2" "a" "b" [ "actor_type", js "robot"; "authority", js "A5"; "requirement_ids", arr [ "UNB-ZZZ-001" ] ]
                   tr "T3" "a" "b" [ "actor_type", js "external"; "requirement_ids", JArr []; "effects", arr [ "grant-approval" ]; "guards", arr [ "sanctions-clear" ] ]
                   tr "T4" "a" "b" [ "actor_type", js "human"; "authority", js "A4"; "failure", JArr [ o [ "condition", js "Timeout after 5s" ] ] ] ] ]
    Assert.Equal<string list>(
        [ "ERROR W6 T1: ai actor exceeds its ceiling A2 with A3"
          "ERROR W5 T2: actor_type 'robot' invalid"
          "ERROR W6 T2: A5 (prohibited) cannot be a transition"
          "ERROR W5 T2: requirement 'UNB-ZZZ-001' does not resolve"
          "ERROR W5 T3: no requirement_ids"
          "ERROR W7 T3: effect 'grant-approval' missing guards ['eligibility-current', 'segregation-of-duties']"
          "ERROR W8 T3: external interaction without an Unknown/timeout outcome"
          "ERROR W10 T4: human transition without ui_surface" ],
        wlines (Workflows.checkTransitions w ok))

[<Fact>]
let ``W9 and W11 check labels and scenarios`` () =
    let w1 = o [ "workflow_id", js "W1"; "states", JArr [ o [ "id", js "s"; "user_label", js "Open"; "lozenge", js "ok" ]; o [ "id", js "t"; "lozenge", js "red" ] ] ]
    let w2 = o [ "workflow_id", js "W2"; "states", JArr [ o [ "id", js "s"; "user_label", js "Opened"; "lozenge", js "ok" ] ] ]
    Assert.Equal<string list>(
        [ "ERROR W9 W1:t: missing user_label or invalid lozenge"; "ERROR W9 s: same state id with different labels [('Open', 'ok'), ('Opened', 'ok')]" ],
        wlines (Workflows.checkLabels [ w1; w2 ]))
    let w3 = o [ "transitions", JArr [ tr "T1" "a" "b" [ "scenario_ids", arr [ "SIM-404" ] ]; tr "T2" "a" "b" [ "guards", arr [ "funded" ] ] ] ]
    Assert.Equal<string list>(
        [ "ERROR W11 T1: scenario 'SIM-404' not found"; "WARN W11 T2: guarded or failure-bearing transition has no scenario" ],
        wlines (Workflows.checkScenarios [ w3 ] (set [ "SIM-001" ])))

// ---------------------------------------------------------------- ia.py

[<Fact>]
let ``ia reports surfaces missing from the map`` () =
    let inv = [ o [ "surface", js "UNB-UI-S-001"; "workflows", js "WFM-01"; "max_authority", js "A2"; "transition_count", js "3" ]; o [ "surface", js "NEW:Unmapped" ] ]
    let rows, missing, extra = Ia.plan inv
    Assert.Equal<string list>([ "NEW:Unmapped" ], missing)
    Assert.Contains("UNB-UI-S-002", extra)
    Assert.Equal<string list>([ "UNB-UI-S-001"; "WS-01"; "Giving home and fund"; "Giving home"; "Donor, advisor, successor"; "phone-complete"; "WFM-01"; "A2"; "3" ], rows.Head)
