/// render_discovery.py: render data-driven tables into the cross-industry discovery Markdown reports (same marker
/// convention as Render).
module Unbound.ReqAudit.RenderDiscovery

open System
open System.Text.RegularExpressions
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

let MISSION = "requirements/discovery/2026-11-23-cross-industry"
let PRIORITIES = [ "P0"; "P1"; "P2"; "P3"; "P4" ]
let COVERAGE = [ "covered"; "partial"; "contradictory"; "missing" ]

let GROUP_NAME =
    Map.ofList
        [ "bnk", "Banking & payments"; "inv", "Investment & accounting"; "svc", "Service, claims, healthcare"
          "erp", "ERP, CRM, trust & estate"; "grt", "Grantmaking & regulatory"; "sas", "SaaS, IAM, records, workflow, other"
          "eco", "Operating economics"; "eva", "Event architecture"; "adv", "Adversarial review"; "int", "Integration" ]

let private ws = re @"\s+"

/// cell(text, limit): str as is, list/tuple joined with ", " (str of each item), anything else `str(text or "")`.
let cellL (limit: int) (text: Json) : string =
    let t =
        match text with
        | JStr v -> v
        | JArr xs -> xs |> List.map str |> String.concat ", "
        | x -> str (orElse (JStr "") x)
    let t = strip ((subLit ws " " t).Replace("|", "\\|"))
    if len t <= limit then t else rstrip (take (limit - 1) t) + "…"

let tableL (limit: int) (header: string list) (rows: Json list list) : string =
    [ yield "| " + String.Join(" | ", header) + " |"
      yield "|" + String.Join("|", header |> List.map (fun _ -> "---")) + "|"
      for r in rows do yield "| " + String.Join(" | ", r |> List.map (cellL limit)) + " |" ]
    |> String.concat "\n"

let table = tableL 200

let J = JStr
let N (n: int) = jint n

type Data =
    { A: Json list; G: Json list; R: Json list; F: Json list; C: Json list; S: Json list; D: Json list; X: Json list
      FC: Json list; EV: Json list; RA: Json list; BM: Json list; E: Json list; PC: Json list; O: Json list; SC: Json list
      SP: Json list; DU: Json list; V: Json }

/// GROUP_NAME.get(g, g).
let groupName (g: Json) : Json = match g with JStr k when GROUP_NAME.ContainsKey k -> J GROUP_NAME.[k] | g -> g

let private cnt (recs: Json list) (g: Json) = recs |> List.filter (fun r -> get "_group" r = g) |> List.length

let gCounts (d: Data) =
    table [ "Record set"; "Count" ]
        [ [ J "Capability atlas rows"; N d.A.Length ]; [ J "Findings"; N d.F.Length ]; [ J "New requirements (UNB-XIN-*)"; N d.G.Length ]
          [ J "Refinements of originals (RR-*)"; N d.R.Length ]; [ J "Scenarios"; N d.S.Length ]; [ J "Conflicts"; N d.C.Length ]
          [ J "Open decisions"; N d.D.Length ]; [ J "Rejected recommendations"; N d.X.Length ]; [ J "Failure catalog rows"; N d.FC.Length ]
          [ J "Event authority matrix rows"; N d.EV.Length ]; [ J "Regulatory applicability rows"; N d.RA.Length ]
          [ J "Business-model rows"; N d.BM.Length ]; [ J "Trace edges (generated)"; N d.E.Length ] ]

let gByGroup (d: Data) =
    let keys = (d.A @ d.G) |> List.map (get "_group") |> List.distinct
    table [ "Research pass"; "Atlas"; "Findings"; "New reqs"; "Refinements"; "Scenarios"; "Rejected"; "Failures" ]
        (keys |> List.map (fun g -> [ groupName g; N(cnt d.A g); N(cnt d.F g); N(cnt d.G g); N(cnt d.R g); N(cnt d.S g); N(cnt d.X g); N(cnt d.FC g) ]))

let SECTIONS =
    [ [ "finops"; "tdabc"; "management accounting"; "lean"; "six sigma"; "process analytics"; "rpa"; "pricing"; "benchmark" ], "§18 Operating economics analogues"
      [ "event"; "architecture"; "data platform"; "model risk" ], "§19 Event architecture analogues"
      [ "payment"; "merchant"; "nacha" ], "I Payments, merchant processing, billing"
      [ "subscription"; "telecom"; "utility billing" ], "M Subscription billing"
      [ "bank" ], "A Banking and financial services"
      [ "invest"; "wealth"; "custody"; "brokerage"; "fund admin"; "transfer agency"; "market data"; "endowment" ], "B Investment and wealth / M custody, fund admin"
      [ "accounting"; "ledger"; "financial reporting" ], "C Accounting, GL, fund accounting"
      [ "call cent"; "contact cent"; "customer service"; "service management" ], "D Call centers and customer service"
      [ "insurance"; "claims" ], "E Insurance and claims"
      [ "healthcare" ], "F Healthcare administration"
      [ "legal case"; "bpo" ], "M Legal case mgmt, BPO, fraud investigation"
      [ "erp"; "enterprise resource"; "shared services"; "performance management" ], "G Enterprise resource planning"
      [ "crm"; "relationship" ], "H CRM, sales, relationship management"
      [ "nonprofit"; "grantmaking"; "cross-border"; "charity data" ], "J Nonprofit administration and grantmaking"
      [ "trust"; "estate"; "family" ], "K Trust, estate, family office"
      [ "saas"; "api platform"; "multi-tenant" ], "L Enterprise SaaS and platform operations"
      [ "identity" ], "M Identity and access management"
      [ "records" ], "M Records management"
      [ "workflow" ], "M Enterprise workflow automation"
      [ "regulatory"; "government"; "tax administration" ], "§9 / M Regulatory, tax and benefits administration" ]

/// section(industry): `(industry or "").lower()` matched against SECTIONS.
let section (industry: Json) : string =
    let low = lower (asStr (orElse (J "") industry))
    SECTIONS |> List.tryFind (fun (keys, _) -> keys |> List.exists low.Contains) |> Option.map snd |> Option.defaultValue "M Other industries"

let private counterJ (xs: Json list list) = counter (xs |> List.map JArr)
let private c2 (c: (Json * int) list) (a: Json) (b: Json) = count c (JArr [ a; b ])

let gCoverageByPass (d: Data) =
    let keys = d.A |> List.map (get "_group") |> List.distinct
    let c = counterJ (d.A |> List.map (fun a -> [ get "_group" a; get "coverage" a ]))
    table ("Research pass" :: COVERAGE @ [ "Total" ])
        (keys |> List.map (fun k -> groupName k :: (COVERAGE |> List.map (fun v -> N(c2 c k (J v)))) @ [ N(COVERAGE |> List.sumBy (fun v -> c2 c k (J v))) ]))

let gCoverageMatrix (d: Data) =
    let sec a = J(section (getOr "industry" (J "") a))
    let inds = d.A |> List.map sec |> List.distinct |> sortJson
    let c = counterJ (d.A |> List.map (fun a -> [ sec a; get "coverage" a ]))
    let rows = inds |> List.map (fun i -> i :: (COVERAGE |> List.map (fun k -> N(c2 c i (J k)))) @ [ N(COVERAGE |> List.sumBy (fun k -> c2 c i (J k))) ])
    let tot = J "**Total**" :: (COVERAGE |> List.map (fun k -> N(inds |> List.sumBy (fun i -> c2 c i (J k))))) @ [ N d.A.Length ]
    tableL 80 ("Industry" :: COVERAGE @ [ "Total" ]) (rows @ [ tot ])

let gNecessity (d: Data) =
    let c = counterJ (d.A |> List.map (fun a -> [ get "necessity" a; get "coverage" a ]))
    let ns = d.A |> List.map (get "necessity") |> List.distinct |> sortJson
    table ("Necessity" :: COVERAGE) (ns |> List.map (fun n -> n :: (COVERAGE |> List.map (fun k -> N(c2 c n (J k))))))

let private countTable (header: string list) (xs: Json list) =
    table header (byCountDesc (counter xs) |> List.map (fun (k, n) -> [ k; N n ]))

let gAutomation (d: Data) = countTable [ "Automation class"; "Capabilities" ] (d.A |> List.map (get "automation"))

let gAtlas (d: Data) =
    tableL 140 [ "ID"; "Industry"; "Capability"; "DAF equiv."; "Necessity"; "Coverage"; "Existing IDs"; "Automation"; "Analogy breaks down" ]
        (d.A |> List.map (fun a ->
            [ at "id" a; get "industry" a; get "capability" a; get "daf_equivalent" a; get "necessity" a; get "coverage" a
              getOr "existing_ids" (JArr []) a; get "automation" a; get "analogy_breaks_down" a ]))

let gGapsAttention (d: Data) =
    let rows = d.A |> List.filter (fun a -> pyEq (get "coverage" a) (J "missing") || pyEq (get "coverage" a) (J "contradictory"))
    tableL 160 [ "ID"; "Industry"; "Capability"; "Coverage"; "Necessity"; "Business problem" ]
        (rows |> List.map (fun a -> [ at "id" a; get "industry" a; get "capability" a; get "coverage" a; get "necessity" a; get "business_problem" a ]))

let gPriorityCounts (d: Data) =
    let keys = d.G |> List.map (get "_group") |> List.distinct
    let c = counterJ (d.G |> List.map (fun g -> [ get "_group" g; get "priority" g ]))
    let rows = keys |> List.map (fun k -> groupName k :: (PRIORITIES |> List.map (fun p -> N(c2 c k (J p)))) @ [ N(PRIORITIES |> List.sumBy (fun p -> c2 c k (J p))) ])
    let tot = J "**Total**" :: (PRIORITIES |> List.map (fun p -> N(keys |> List.sumBy (fun k -> c2 c k (J p))))) @ [ N d.G.Length ]
    table ("Research pass" :: PRIORITIES @ [ "Total" ]) (rows @ [ tot ])

let private priOrder (p: Json) = match p with JStr v -> (match List.tryFindIndex ((=) v) PRIORITIES with Some i -> i | None -> 9) | _ -> 9

let private byPriorityThenId (key: string) (xs: Json list) =
    xs |> List.sortWith (fun a b -> match compare (priOrder (get "priority" a)) (priOrder (get "priority" b)) with 0 -> cmpJson (at key a) (at key b) | c -> c)

let gRegister (d: Data) =
    let pri (g: Json) =
        let p = get "priority" g
        let asProposed = getOr "priority_as_proposed" p g
        J(asStr p + (if not (pyEq asProposed p) then $""" (was {s "priority_as_proposed" g})""" else ""))
    tableL 260 [ "ID"; "P"; "Parent capability"; "Statement"; "Applicability"; "Discovery source"; "Priority justification"; "Adversarial disposition"; "Links" ]
        (byPriorityThenId "id" d.G
         |> List.map (fun g ->
             [ at "id" g; pri g; get "parent_capability" g; get "statement" g; get "applicability" g; get "discovery_source" g
               orElse (get "priority_justification" g) (get "priority_revision" g); getOr "adversarial_disposition" (J "") g; getOr "links" (JArr []) g ]))

let gPriorityShift (d: Data) =
    let c = counterJ (d.G |> List.map (fun g -> [ getOr "priority_as_proposed" (get "priority" g) g; get "priority" g ]))
    table ("As proposed by research pass" :: PRIORITIES @ [ "Total" ])
        (PRIORITIES |> List.map (fun p -> J p :: (PRIORITIES |> List.map (fun q -> N(c2 c (J p) (J q)))) @ [ N(PRIORITIES |> List.sumBy (fun q -> c2 c (J p) (J q))) ]))

let gRefinements (d: Data) =
    tableL 260 [ "Refinement"; "Original"; "Relationship"; "Original path:line"; "Reason for change"; "Decision status" ]
        (d.R |> List.map (fun r ->
            [ at "refinement_id" r; at "original_id" r; get "relationship" r; J $"""{str (get "original_path" r)}:{str (get "original_line" r)}"""
              get "reason_for_change" r; get "decision_status" r ]))

let gRefinementDetail (d: Data) =
    let one (r: Json) =
        let ac =
            iter (getOr "acceptance_criteria" (JArr []) r)
            |> List.map (fun a -> $"""  - **{str (getOr "id" (J "AC") a)}** Given {str (get "given" a)}; when {str (get "when" a)}; then {str (get "then" a)}.""")
            |> String.concat "\n"
        $"""### {s "refinement_id" r} ({str (get "relationship" r)} {s "original_id" r})

"""
        + $"""- **Original** (`{str (get "original_path" r)}:{str (get "original_line" r)}` @ `{str (get "original_revision" r)}`): {cellL 600 (get "original_text" r)}
"""
        + $"""- **Refined statement:** {cellL 1200 (get "statement" r)}
"""
        + $"""- **Reason:** {cellL 500 (get "reason_for_change" r)}
"""
        + $"""- **Decision status:** {str (get "decision_status" r)} · **Confidence:** {str (get "confidence" r)} · **Findings:** {cellL 200 (getOr "related_findings" (JArr []) r)}
"""
        + $"""- **Acceptance criteria:**
{ac}
"""
    d.R |> List.map one |> String.concat "\n"

let private domainOf (f: Json) = J((s "finding_id" f).Split('-').[1])

let gFindingsSeverity (d: Data) =
    let keys = d.F |> List.map domainOf |> List.distinct
    let sev = [ "critical"; "high"; "medium"; "low" ]
    let c = counterJ (d.F |> List.map (fun f -> [ domainOf f; at "severity" f ]))
    let rows = keys |> List.map (fun k -> k :: (sev |> List.map (fun sv -> N(c2 c k (J sv)))) @ [ N(sev |> List.sumBy (fun sv -> c2 c k (J sv))) ])
    table ("Domain" :: sev @ [ "Total" ]) (rows @ [ J "**Total**" :: (sev |> List.map (fun sv -> N(keys |> List.sumBy (fun k -> c2 c k (J sv))))) @ [ N d.F.Length ] ])

let gFindingsSevere (d: Data) =
    let rows = d.F |> List.filter (fun f -> List.contains (s "severity" f) [ "critical"; "high" ])
    tableL 220 [ "Finding"; "Severity"; "Title"; "Affected"; "Proposed resolution" ]
        (rows |> List.map (fun f -> [ at "finding_id" f; at "severity" f; at "title" f; at "affected_ids" f; at "proposed_resolution" f ]))

let gConflicts (d: Data) =
    tableL 240 [ "Conflict"; "A"; "B"; "Incompatibility"; "Recommended resolution"; "Human authority" ]
        (d.C |> List.map (fun c ->
            [ at "conflict_id" c; at "requirement_a" c; at "requirement_b" c; at "incompatibility" c; at "recommended_resolution" c; get "requires_human_authority" c ]))

let gDuplicates (d: Data) =
    let rows = d.F |> List.filter (fun f -> (lower (s "title" f)).StartsWith("duplicate", StringComparison.Ordinal))
    tableL 320 [ "Finding"; "Canonical, then overlapping"; "Topic"; "Resolution" ]
        (rows |> List.map (fun f ->
            let t = s "title" f
            let topic = match t.IndexOf ':' with -1 -> t | i -> t.Substring(i + 1)
            [ at "finding_id" f; at "affected_ids" f; J topic; at "proposed_resolution" f ]))

let gRegulatory (d: Data) =
    tableL 160 [ "ID"; "Topic"; "Binding"; "Authority"; "Citation"; "Applies to (entity)"; "Activity"; "Models"; "Confidence"; "Counsel question" ]
        (d.RA |> List.map (fun r ->
            [ at "id" r; get "topic" r; get "binding" r; get "authority_type" r; get "citation" r; get "applies_to_entity" r
              get "applies_to_activity" r; get "business_models" r; get "confidence" r; get "counsel_question" r ]))

let gFailures (d: Data) =
    tableL 150 [ "ID"; "Capability"; "Failure mode"; "Scenario"; "Detection"; "Recovery"; "Escalation"; "Covered by / new" ]
        (d.FC |> List.map (fun f ->
            [ at "id" f; get "capability" f; get "failure_mode" f; get "scenario" f; get "detection" f; get "recovery" f; get "escalation" f
              JArr(iter (getOr "covered_by" (JArr []) f) @ iter (getOr "new_ids" (JArr []) f)) ]))

let gFailureModes (d: Data) =
    countTable [ "Failure mode"; "Catalogued cases" ]
        (d.FC |> List.map (fun f -> J(lower (strip ((str (getOr "failure_mode" (J "?") f)).Split(';').[0])))))

let gRejected (d: Data) =
    tableL 170 [ "ID"; "Idea"; "Source industry"; "Argument for"; "Argument against"; "Reason rejected"; "Revisit if" ]
        (d.X |> List.map (fun r ->
            [ at "id" r; get "idea" r; get "source_industry" r; get "argue_for" r; get "argue_against" r; get "reason_rejected" r; get "revisit_if" r ]))

let gEvents (d: Data) =
    tableL 110 [ "ID"; "Event"; "Capability"; "Owner"; "Authority"; "Store"; "Consistency"; "Idempotency key"; "Correction" ]
        (d.EV |> List.map (fun e ->
            [ at "id" e; get "event" e; get "business_capability" e; get "owning_module" e; get "authority" e
              get "source_of_truth_store" e; get "consistency" e; get "idempotency_key" e; get "correction_pattern" e ]))

let gEventSummary (d: Data) =
    let auth = [ "authoritative-financial"; "authoritative-nonfinancial"; "derived"; "informational"; "observation-ai" ]
    let caps = d.EV |> List.map (get "business_capability") |> List.distinct
    let c = counterJ (d.EV |> List.map (fun e -> [ get "business_capability" e; get "authority" e ]))
    let sc = counterJ (d.EV |> List.map (fun e -> [ get "business_capability" e; get "consistency" e ]))
    table ("Capability" :: auth @ [ "synchronous"; "eventual" ])
        (caps |> List.map (fun k -> k :: (auth |> List.map (fun a -> N(c2 c k (J a)))) @ [ N(c2 sc k (J "synchronous")); N(c2 sc k (J "eventual")) ]))

let gModels (d: Data) =
    tableL 260 [ "Model"; "Distinguishing capabilities"; "Configuration needed"; "Unsupported or risky"; "Confidence" ]
        (d.BM |> List.map (fun m -> [ get "model" m; get "distinguishing_capabilities" m; get "configuration_needed" m; get "unsupported_or_risky" m; get "confidence" m ]))

let gDecisions (d: Data) =
    tableL 220 [ "Decision"; "Type"; "Question"; "Owner"; "Recommendation"; "Blocks impl."; "Status" ]
        (d.D |> List.map (fun x ->
            [ at "decision_id" x; get "type" x; at "question" x; at "decision_owner" x; at "recommendation" x; get "blocks_implementation" x; at "status" x ]))

let gDecisionTypes (d: Data) = countTable [ "Type"; "Open decisions" ] (d.D |> List.map (get "type"))

let gScenarios (d: Data) =
    tableL 180 [ "Scenario"; "Title"; "Requirements"; "Stimulus"; "Expected states" ]
        (d.S |> List.map (fun sc -> [ at "scenario_id" sc; at "title" sc; at "requirement_ids" sc; at "stimulus" sc; at "expected_states" sc ]))

let private startsWith (prefix: string) (j: Json) = (asStr j).StartsWith(prefix, StringComparison.Ordinal)

let gEconomics (d: Data) =
    let rows = d.G |> List.filter (fun g -> startsWith "UNB-XIN-ECO-" (at "id" g))
    let acs (g: Json) =
        iter (getOr "acceptance_criteria" (JArr []) g)
        |> List.map (fun a -> $"""[{str (get "id" a)}] Given {str (get "given" a)}; when {str (get "when" a)}; then {str (get "then" a)}.""")
        |> String.concat " "
    tableL 600 [ "ID"; "P"; "Requirement"; "Acceptance criteria"; "UI / reporting implications" ]
        (rows |> List.map (fun g ->
            [ at "id" g; get "priority" g; get "statement" g; J(acs g)
              J $"""{str (getOr "ui_implications" (J "") g)} {str (getOr "reporting_implications" (J "") g)}""" ]))

let gEvaReqs (d: Data) =
    tableL 500 [ "ID"; "P"; "Requirement"; "Links" ]
        (d.G |> List.filter (fun g -> startsWith "UNB-XIN-EVA-" (at "id" g))
         |> List.map (fun g -> [ at "id" g; get "priority" g; get "statement" g; getOr "links" (JArr []) g ]))

let gPriorityChallenges (d: Data) =
    tableL 260 [ "Requirement"; "Current"; "Proposed"; "Reason" ]
        (d.PC |> List.map (fun p -> [ get "id" p; get "current_priority" p; get "proposed_priority" p; get "reason" p ]))

let gAdvFindings (d: Data) =
    tableL 260 [ "Finding"; "Severity"; "Title"; "Affected"; "Proposed resolution" ]
        (d.F |> List.filter (fun f -> startsWith "XIF-ADV-" (at "finding_id" f))
         |> List.map (fun f -> [ at "finding_id" f; at "severity" f; at "title" f; at "affected_ids" f; at "proposed_resolution" f ]))

let gAdvConflicts (d: Data) =
    tableL 300 [ "Conflict"; "A"; "B"; "Incompatibility"; "Recommended resolution" ]
        (d.C |> List.filter (fun c -> startsWith "XIC-ADV-" (at "conflict_id" c))
         |> List.map (fun c -> [ at "conflict_id" c; at "requirement_a" c; at "requirement_b" c; at "incompatibility" c; at "recommended_resolution" c ]))

let gAdvDecisions (d: Data) =
    tableL 300 [ "Decision"; "Question"; "Recommendation" ]
        (d.D |> List.filter (fun x -> startsWith "XID-ADV-" (at "decision_id" x)) |> List.map (fun x -> [ at "decision_id" x; at "question" x; at "recommendation" x ]))

let gAdvRequirements (d: Data) =
    tableL 600 [ "ID"; "P"; "Statement"; "Business problem" ]
        (d.G |> List.filter (fun g -> startsWith "UNB-XIN-ADV-" (at "id" g)) |> List.map (fun g -> [ at "id" g; get "priority" g; get "statement" g; get "business_problem" g ]))

/// {x["decision_id"]: x for x in d["D"]}.get(id, {})
let private decisionIndex (d: Data) =
    let m = d.D |> List.fold (fun (m: Map<string, Json>) x -> m.Add(s "decision_id" x, x)) Map.empty
    fun (id: Json) -> match id with JStr k when m.ContainsKey k -> m.[k] | _ -> JObj []

let gOutcomeSummary (d: Data) =
    let q = decisionIndex d
    let c = counterJ (d.O |> List.map (fun o -> [ get "type" (q (at "decision_id" o)); get "confidence" o ]))
    let keyParts (k: Json) = match k with JArr [ a; b ] -> a, b | _ -> JNull, JNull
    let types = c |> List.map (fst >> keyParts >> fst) |> List.distinct |> sortJson
    let confs = c |> List.map (fst >> keyParts >> snd) |> List.distinct |> sortJson
    table ("Type" :: (confs |> List.map str) @ [ "Total" ])
        (types |> List.map (fun t -> t :: (confs |> List.map (fun k -> N(c2 c t k))) @ [ N(confs |> List.sumBy (fun k -> c2 c t k)) ]))

let gOutcomes (d: Data) =
    let q = decisionIndex d
    let one (o: Json) =
        let x = q (at "decision_id" o)
        let bullets k = iter (getOr k (JArr []) o) |> List.map (fun a -> $"  - {str a}") |> String.concat "\n"
        $"""### {s "decision_id" o} ({str (getOr "type" (J "?") x)})

"""
        + $"""**Question.** {cellL 600 (get "question" x)}

"""
        + $"""**Decision.** {s "decision" o}

"""
        + $"""**Rationale.** {s "rationale" o}

"""
        + $"""- **Confidence:** {str (get "confidence" o)} · **Status:** {str (get "status" o)} · **Confirmation required:** {str (get "confirmation_required" o)}
"""
        + $"""- **Authority:**
{bullets "authority"}
"""
        + $"""- **Requirement consequences:**
{bullets "requirement_consequences"}
"""
    let order (o: Json) = [ getOr "type" (J "") (q (at "decision_id" o)); at "decision_id" o ]
    d.O |> List.sortWith (fun a b -> cmpTuple (order a) (order b)) |> List.map one |> String.concat "\n"

/// {**{g["id"]: g}, **{r["refinement_id"]: r}}: insertion order, later values win.
let private recordIndex (d: Data) : (string * Json) list =
    let pairs = (d.G |> List.map (fun g -> s "id" g, g)) @ (d.R |> List.map (fun r -> s "refinement_id" r, r))
    let last = pairs |> List.fold (fun (m: Map<string, Json>) (k, v) -> m.Add(k, v)) Map.empty
    pairs |> List.map fst |> List.distinct |> List.map (fun k -> k, last.[k])

let gApplied (d: Data) =
    let rows =
        recordIndex d
        |> List.sortWith (fun (a, _) (b, _) -> cmpStr a b)
        |> List.filter (fun (_, r) -> truthy (get "decisions_applied" r))
        |> List.map (fun (rid, r) -> [ J rid; J(iter (getOr "decisions_applied" (JArr []) r) |> List.map asStr |> String.concat ", "); get "statement" r ])
    tableL 900 [ "Requirement"; "Decisions applied"; "Statement as revised" ] rows

let gScopeSummary (d: Data) =
    let scopes = [ "R1"; "R1-conditional"; "Deferred"; "Rejected-recommended" ]
    let c = counterJ (d.SC |> List.map (fun x -> [ orElse (J "refinement") (at "priority" x); at "scope" x ]))
    let pris = c |> List.map (fun (k, _) -> match k with JArr(p :: _) -> p | _ -> JNull) |> List.distinct |> sortJson
    let rows = pris |> List.map (fun p -> p :: (scopes |> List.map (fun sc -> N(c2 c p (J sc)))) @ [ N(scopes |> List.sumBy (fun sc -> c2 c p (J sc))) ])
    let tot = J "**Total**" :: (scopes |> List.map (fun sc -> N(pris |> List.sumBy (fun p -> c2 c p (J sc))))) @ [ N d.SC.Length ]
    table ("Priority" :: scopes @ [ "Total" ]) (rows @ [ tot ])

let gScopeOut (d: Data) =
    table [ "Requirement"; "Priority"; "Scope"; "Reason" ]
        (d.SC |> List.filter (fun x -> not (pyEq (at "scope" x) (J "R1"))) |> List.map (fun x -> [ at "id" x; at "priority" x; at "scope" x; at "reason" x ]))

let gScopeIn (d: Data) =
    let stmt = recordIndex d |> Map.ofList
    let rows = d.SC |> List.filter (fun x -> pyEq (at "scope" x) (J "R1")) |> byPriorityThenId "id"
    tableL 180 [ "Requirement"; "Priority"; "Statement" ]
        (rows |> List.map (fun x ->
            let r = match stmt.TryFind(s "id" x) with Some r -> r | None -> raise (PyError $"""KeyError: {repr (at "id" x)}""")
            [ at "id" x; at "priority" x; get "statement" r ]))

let gScopeProposals (d: Data) =
    let q = decisionIndex d
    tableL 700 [ "Decision"; "Question"; "Proposed answer"; "Effect" ]
        (d.SP |> List.map (fun p -> [ at "decision_id" p; get "question" (q (at "decision_id" p)); at "proposed_answer" p; at "effect" p ]))

let gTrace (d: Data) = countTable [ "Relationship"; "Edges" ] (d.E |> List.map (at "relationship"))

let gValidation (d: Data) =
    let v = d.V
    let records = match getOr "records" (JObj []) v with JObj kvs -> kvs | _ -> []
    table [ "Measure"; "Value" ]
        ([ [ J "Errors"; get "errors" v ]; [ J "Warnings"; get "warnings" v ]; [ J "Trace edges"; get "trace_edges" v ] ]
         @ (records |> List.map (fun (k, n) -> [ J $"Records: {k}"; n ])))

let GENERATORS : Map<string, Data -> string> =
    Map.ofList
        [ "counts", gCounts; "by-group", gByGroup; "coverage-by-pass", gCoverageByPass; "coverage-matrix", gCoverageMatrix
          "necessity", gNecessity; "automation", gAutomation; "atlas", gAtlas; "gaps-attention", gGapsAttention
          "priority-counts", gPriorityCounts; "register", gRegister; "priority-shift", gPriorityShift; "refinements", gRefinements
          "refinement-detail", gRefinementDetail; "findings-severity", gFindingsSeverity; "findings-severe", gFindingsSevere
          "conflicts", gConflicts; "duplicates", gDuplicates; "regulatory", gRegulatory; "failures", gFailures
          "failure-modes", gFailureModes; "rejected", gRejected; "events", gEvents; "event-summary", gEventSummary
          "models", gModels; "decisions", gDecisions; "decision-types", gDecisionTypes; "scenarios", gScenarios
          "economics", gEconomics; "eva-reqs", gEvaReqs; "priority-challenges", gPriorityChallenges
          "adv-findings", gAdvFindings; "adv-conflicts", gAdvConflicts; "adv-decisions", gAdvDecisions
          "adv-requirements", gAdvRequirements; "outcome-summary", gOutcomeSummary; "outcomes", gOutcomes; "applied", gApplied
          "scope-summary", gScopeSummary; "scope-out", gScopeOut; "scope-in", gScopeIn; "scope-proposals", gScopeProposals
          "trace", gTrace; "validation", gValidation ]

let BLOCK = rx RegexOptions.Singleline @"(<!-- BEGIN GENERATED: ([a-z0-9-]+) -->\n)(.*?)(<!-- END GENERATED: \2 -->)"

let render (text: string) (d: Data) : string = subFn BLOCK (fun m -> grp 1 m + GENERATORS.[grp 2 m] d + "\n" + grp 4 m) text

let load (data: string) : Data =
    let p n = combine data n
    let jl n = if IO.File.Exists(p n) then loadJsonlText (readText (p n)) else []
    let cv n = if IO.File.Exists(p n) then dictRows (readText (p n)) else []
    { A = jl "capability-atlas.jsonl"; G = jl "gap-requirements.jsonl"; R = jl "refinements.jsonl"; F = cv "findings.csv"
      C = cv "conflicts.csv"; S = cv "scenarios.csv"; D = cv "decisions.csv"; X = jl "rejected.jsonl"; FC = jl "failure-catalog.jsonl"
      EV = jl "event-matrix.jsonl"; RA = jl "regulatory-applicability.jsonl"; BM = jl "business-models.jsonl"; E = cv "trace-edges.csv"
      PC = jl "priority-challenges.jsonl"; O = jl "decision-outcomes.jsonl"; SC = jl "release1-scope.jsonl"; SP = jl "scope-proposals.jsonl"
      DU = cv "duplicates.csv"; V = (if IO.File.Exists(p "validation-report.json") then Io.loadJson (p "validation-report.json") else JObj []) }

let main (argv: string list) : int =
    let root = argv.[0]
    let mission = combine root (if argv.Length > 1 then argv.[1] else MISSION)
    let d = load (combine mission "data")
    let docs = glob mission "*.md" |> List.filter IO.File.Exists
    let unknown =
        docs |> List.collect (fun p -> finditer BLOCK (readText p) |> List.map (grp 2)) |> List.filter (fun n -> not (GENERATORS.ContainsKey n))
        |> List.distinct |> sortStrings
    if not unknown.IsEmpty then
        Io.print ("unknown generated blocks: " + reprList unknown)
        1
    else
        for p in docs do
            let old = readText p
            let updated = render old d
            if updated <> old then
                writeText p updated
                Io.print $"rendered {name p}"
        0
