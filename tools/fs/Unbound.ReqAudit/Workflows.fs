/// workflows.py: validate workflow state-machine models (checks W1-W11) and derive the UI surface inventory.
module Unbound.ReqAudit.Workflows

open System
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

let RECON = "requirements/reconciliation/2026-11-23"
let AUTH = [ "A0"; "A1"; "A2"; "A3"; "A4"; "A5" ]
let LOZENGE = set [ "ok"; "attention"; "unknown"; "blocked"; "neutral" ]
let ACTORS = set [ "human"; "deterministic"; "ai"; "external"; "timer" ]
let CEILING = Map.ofList [ "ai", "A2"; "deterministic", "A3"; "timer", "A3"; "external", "A3"; "human", "A4" ]

/// effect tag -> guards that must be present on the transition
let GUARDS =
    Map.ofList
        [ "external-money-out", set [ "eligibility-current"; "sanctions-clear"; "hold-watermark-current"; "destination-verified"; "funded" ]
          "reserve-capacity", set [ "capacity-in-transaction" ]
          "grant-approval", set [ "eligibility-current"; "sanctions-clear"; "segregation-of-duties" ]
          "credit-grantable", set [ "contribution-final" ]
          "issue-acknowledgment", set [ "completed-gift-determined"; "sponsor-accepted" ] ]

type Issue = { Level: string; Check: string; Subject: string; Message: string }

let private err check subject message = { Level = "ERROR"; Check = check; Subject = subject; Message = message }

/// {s["id"]: s for s in w["states"]}: first position, last value.
let statesOf (w: Json) : (string * Json) list =
    let pairs = iter (at "states" w) |> List.map (fun st -> s "id" st, st)
    let last = pairs |> List.fold (fun (m: Map<string, Json>) (k, v) -> m.Add(k, v)) Map.empty
    pairs |> List.map fst |> List.distinct |> List.map (fun k -> k, last.[k])

let private froms (t: Json) = match at "from" t with JArr xs -> xs |> List.map asStr | f -> [ asStr f ]

let private failureTargets (t: Json) =
    iter (getOr "failure" (JArr []) t) |> List.map (get "to_state") |> List.filter truthy |> List.map asStr

/// Adjacency in insertion order: state -> successors (with repeats).
let graph (w: Json) : (string * string list) list =
    let edges = iter (at "transitions" w) |> List.collect (fun t -> froms t |> List.map (fun f -> f, s "to" t :: failureTargets t))
    edges |> List.map fst |> List.distinct |> List.map (fun f -> f, edges |> List.filter (fst >> (=) f) |> List.collect snd)

let private succ (g: (string * string list) list) (n: string) = match List.tryFind (fst >> (=) n) g with Some(_, v) -> v | None -> []

let reach (g: (string * string list) list) (start: string) : Set<string> =
    let rec go (seen: Set<string>) (queue: string list) =
        match queue with
        | [] -> seen
        | q :: rest ->
            let fresh = succ g q |> List.distinct |> List.filter (fun n -> not (seen.Contains n))
            go (Set.union seen (Set.ofList fresh)) (rest @ fresh)
    go (Set.singleton start) [ start ]

let checkStructure (w: Json) : Issue list =
    let wid = s "workflow_id" w
    let S = statesOf w
    let has st = S |> List.exists (fst >> (=) st)
    let tids = iter (at "transitions" w) |> List.map (s "id")
    let dupes = counter tids |> List.filter (fun (_, n) -> n > 1) |> List.map fst |> sortStrings
    let unknown =
        iter (at "transitions" w)
        |> List.collect (fun t ->
            (froms t @ [ s "to" t ] @ failureTargets t) |> List.filter (fun st -> not (has st)) |> List.map (fun st -> err "W1" (s "id" t) $"unknown state '{st}'"))
    let head = (dupes |> List.map (fun t -> err "W1" wid $"duplicate transition id {t}")) @ unknown
    let inits = S |> List.filter (fun (_, v) -> pyEq (at "kind" v) (JStr "initial"))
    if inits.Length <> 1 then head @ [ err "W2" wid $"expected exactly one initial state, found {inits.Length}" ]
    else
        let g = graph w
        let r = reach g (fst inits.Head)
        let kind (v: Json) = at "kind" v
        let isTerminal (v: Json) = pyEq (kind v) (JStr "terminal") || pyEq (kind v) (JStr "correction")
        let terminals = S |> List.filter (snd >> isTerminal) |> List.map fst |> Set.ofList
        let unreachable = S |> List.filter (fun (st, _) -> not (r.Contains st)) |> List.map (fun (st, _) -> err "W2" $"{wid}:{st}" "state unreachable from initial")
        let perState =
            S
            |> List.collect (fun (st, v) ->
                if isTerminal v then
                    let bad =
                        succ g st
                        |> List.filter (fun n -> match List.tryFind (fst >> (=) n) S with Some(_, nv) -> not (pyEq (get "kind" nv) (JStr "correction")) | None -> true)
                    if bad.IsEmpty then [] else [ err "W4" $"{wid}:{st}" $"terminal state transitions to non-correction state(s) {reprList bad}" ]
                elif (succ g st).IsEmpty then [ err "W3" $"{wid}:{st}" "non-terminal state has no outgoing transition (dead end)" ]
                elif (Set.intersect (reach g st) terminals).IsEmpty then [ err "W3" $"{wid}:{st}" "no path to any terminal state (trap)" ]
                else [])
        head @ unreachable @ perState

let checkTransitions (w: Json) (resolves: string -> bool) : Issue list =
    iter (at "transitions" w)
    |> List.collect (fun t ->
        let tid = s "id" t
        let a, auth = get "actor_type" t, get "authority" t
        let aStr = match a with JStr v -> Some v | _ -> None
        let reqs = getOr "requirement_ids" (JArr []) t
        let effects = iter (getOr "effects" (JArr []) t) |> List.map asStr |> Set.ofList
        let guards = iter (getOr "guards" (JArr []) t) |> List.map asStr |> Set.ofList
        [ if not (match aStr with Some v -> ACTORS.Contains v | None -> false) then yield err "W5" tid $"actor_type '{str a}' invalid"
          match auth with
          | JStr v when List.contains v AUTH ->
              if v = "A5" then yield err "W6" tid "A5 (prohibited) cannot be a transition"
              else
                  match aStr |> Option.bind CEILING.TryFind with
                  | Some ceiling when List.findIndex ((=) v) AUTH > List.findIndex ((=) ceiling) AUTH ->
                      yield err "W6" tid $"{aStr.Value} actor exceeds its ceiling {ceiling} with {v}"
                  | _ -> ()
          | _ -> yield err "W5" tid $"authority '{str auth}' invalid"
          if not (truthy reqs) then yield err "W5" tid "no requirement_ids"
          for r in iter reqs |> List.map asStr do
              if not (resolves r) then yield err "W5" tid $"requirement '{r}' does not resolve"
          // Python iterates a set here (hash order); guarded effects are reported in sorted order
          for e in sortStrings (Set.intersect effects (GUARDS |> Map.toList |> List.map fst |> Set.ofList)) do
              let missing = Set.difference GUARDS.[e] guards
              if not missing.IsEmpty then yield err "W7" tid $"effect '{e}' missing guards {reprList (sortStrings missing)}"
          let timeoutOutcome =
              iter (getOr "failure" (JArr []) t)
              |> List.exists (fun x ->
                  let c = lower (asStr (getOr "condition" (JStr "") x))
                  c.StartsWith("unknown", StringComparison.Ordinal) || c.StartsWith("timeout", StringComparison.Ordinal))
          if (aStr = Some "external" || effects.Contains "external-call") && not timeoutOutcome then
              yield err "W8" tid "external interaction without an Unknown/timeout outcome"
          if aStr = Some "human" && not (truthy (get "ui_surface" t)) then yield err "W10" tid "human transition without ui_surface" ])

let checkLabels (ws: Json list) : Issue list =
    let entries =
        ws |> List.collect (fun w -> iter (at "states" w) |> List.map (fun st -> s "workflow_id" w, st))
    let missing =
        entries
        |> List.filter (fun (_, st) -> not (truthy (get "user_label" st)) || not (match get "lozenge" st with JStr l -> LOZENGE.Contains l | _ -> false))
        |> List.map (fun (wid, st) -> err "W9" $"""{wid}:{s "id" st}""" "missing user_label or invalid lozenge")
    let ids = entries |> List.map (fun (_, st) -> s "id" st) |> List.distinct
    let conflicts =
        ids
        |> List.choose (fun sid ->
            let labels = entries |> List.filter (fun (_, st) -> s "id" st = sid) |> List.map (fun (_, st) -> JArr [ get "user_label" st; get "lozenge" st ]) |> List.distinct
            if labels.Length > 1 then
                let tuples = sortJson labels |> List.map (fun l -> match l with JArr xs -> "(" + (xs |> List.map repr |> String.concat ", ") + ")" | _ -> "")
                Some(err "W9" sid $"""same state id with different labels [{String.concat ", " tuples}]""")
            else None)
    missing @ conflicts

let checkScenarios (ws: Json list) (scenarioIds: Set<string>) : Issue list =
    ws
    |> List.collect (fun w ->
        iter (at "transitions" w)
        |> List.collect (fun t ->
            let sc = getOr "scenario_ids" (JArr []) t
            (iter sc |> List.filter (fun x -> match x with JStr v -> not (scenarioIds.Contains v) | _ -> true)
             |> List.map (fun x -> err "W11" (s "id" t) $"scenario '{str x}' not found"))
            @ (if not (truthy sc) && (truthy (get "guards" t) || truthy (get "failure" t)) then
                   [ { Level = "WARN"; Check = "W11"; Subject = s "id" t; Message = "guarded or failure-bearing transition has no scenario" } ]
               else [])))

type Surface = { Surface: string; Workflows: Set<string>; Transitions: string list; Roles: Set<string>; MaxAuthority: string; Release: Set<string> }

let surfaceInventory (ws: Json list) : Map<string, Surface> =
    let blank sfc = { Surface = sfc; Workflows = Set.empty; Transitions = []; Roles = Set.empty; MaxAuthority = "A0"; Release = Set.empty }
    let upd (inv: Map<string, Surface>) sfc f = inv.Add(sfc, f (inv.TryFind sfc |> Option.defaultValue (blank sfc)))
    ws
    |> List.fold
        (fun inv w ->
            let wid = s "workflow_id" w
            let release = str (getOr "release" (JStr "R1") w)
            let inv =
                iter (at "transitions" w)
                |> List.fold
                    (fun inv t ->
                        let sv = get "ui_surface" t
                        if not (truthy sv) then inv
                        else
                            upd inv (asStr sv) (fun e ->
                                let authority = s "authority" t
                                { e with
                                    Workflows = e.Workflows.Add wid
                                    Transitions = e.Transitions @ [ s "id" t ]
                                    Roles = e.Roles.Add(match getOr "actor_role" (JStr "") t with JNull -> "" | r -> str r)
                                    Release = e.Release.Add release
                                    MaxAuthority =
                                        if List.findIndex ((=) authority) AUTH > List.findIndex ((=) e.MaxAuthority) AUTH then authority else e.MaxAuthority }))
                    inv
            iter (getOr "observer_surfaces" (JArr []) w)
            |> List.fold (fun inv sv -> upd inv (asStr sv) (fun e -> { e with Workflows = e.Workflows.Add wid; Release = e.Release.Add release })) inv)
        Map.empty

let main (argv: string list) : int =
    let root = resolve argv.[0]
    let data = combine root (RECON + "/data")
    let d n = combine data n
    let ws = glob (combine root (RECON + "/workflows")) "WFM-*.json" |> List.filter IO.File.Exists |> List.map Io.loadJson
    let inv = Validate.column "identifier" (Io.loadCsv (d "inventory.csv"))
    let newIds =
        Set.ofList (
            (Io.loadJsonl (d "refinements.jsonl") |> List.map (s "refinement_id"))
            @ (Io.loadJsonl (d "gap-requirements.jsonl") |> List.map (s "id"))
            @ (Io.loadJsonl (d "ui-requirements.jsonl") |> List.map (s "id"))
            @ (Io.loadCsv (d "decisions.csv") |> List.map (s "decision_id")))
    let extra = Validate.column "new_id" (Io.loadCsv (d "renamed-ids.csv"))
    let ok = Validate.resolver inv (Set.union newIds extra) Set.empty Set.empty
    let scen =
        Set.union (Validate.column "scenario_id" (Io.loadCsv (d "scenarios.csv")))
            (Io.loadCsv (d "inventory.csv") |> List.filter (fun r -> s "kind" r = "scenario") |> List.map (s "identifier") |> Set.ofList)
    let issues = (ws |> List.collect (fun w -> checkStructure w @ checkTransitions w ok)) @ checkLabels ws @ checkScenarios ws scen
    let errors = issues |> List.filter (fun i -> i.Level = "ERROR")
    issues |> List.iter (fun i -> Io.print $"{ljust 5 i.Level} {ljust 4 i.Check} {i.Subject}: {i.Message}")
    let surfaces = surfaceInventory ws
    let rows =
        surfaces
        |> Map.toList
        |> List.map fst
        |> sortStrings
        |> List.map (fun sfc ->
            let e = surfaces.[sfc]
            [ sfc
              (if sfc.StartsWith("UNB-UI-", StringComparison.Ordinal) then "existing" else "new")
              String.Join(";", sortStrings e.Release)
              String.Join(";", sortStrings e.Workflows)
              string e.Transitions.Length
              String.Join(";", e.Roles |> Set.filter (fun r -> r <> "") |> sortStrings)
              e.MaxAuthority ])
    writeText (d "surface-inventory.csv")
        (PyCsv.format PyCsv.QuoteAll ([ "surface"; "kind"; "release"; "workflows"; "transition_count"; "roles"; "max_authority" ] :: rows))
    let surfaceNames = surfaces |> Map.toList |> List.map fst
    let summary =
        JObj
            [ "workflows", jint ws.Length
              "states", jint (ws |> List.sumBy (fun w -> jlen (at "states" w)))
              "transitions", jint (ws |> List.sumBy (fun w -> jlen (at "transitions" w)))
              "errors", jint errors.Length
              "warnings", jint (issues.Length - errors.Length)
              "errors_by_check", JObj(counter (errors |> List.map (fun i -> i.Check)) |> List.sortWith (fun (a, _) (b, _) -> cmpStr a b) |> List.map (fun (k, n) -> k, jint n))
              "surfaces", jint surfaceNames.Length
              "new_surfaces", jint (surfaceNames |> List.filter (fun x -> not (x.StartsWith("UNB-UI-", StringComparison.Ordinal))) |> List.length) ]
    Io.writeJsonReport (d "workflow-validation.json") summary
    Io.print (Io.dumps2 summary)
    if errors.IsEmpty then 0 else 1
