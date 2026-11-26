/// Data shapes of a screen prototype module (the default export of each tools/screens/*.mjs).
module Unbound.Designs.V1.Model

/// A design state: Body receives the state's unique id prefix (for example "s001-1").
type State = { Title: string; Persona: string; Demonstrates: string; Body: string -> string }

/// satisfied_by_markup: true, false or "needs-application-behaviour".
type Satisfied =
    | Yes
    | No
    | NeedsApplicationBehaviour

type Coverage = { RequirementId: string; AcceptanceId: string; Satisfied: Satisfied; Note: string }

/// A state after rendering, as handed to the page shell or a custom renderer.
type RenderedState = { Id: string; Title: string; Html: string }

type Screen =
    { Id: string
      File: string
      Title: string
      ReqIds: string list
      /// "donor" | "staff" | "charity"; unused when Render is set.
      Audience: string
      Brand: bool
      Lead: string
      States: State list
      Coverage: Coverage list
      Gaps: string list
      /// A custom page renderer replacing the shared shell (W-001).
      Render: (RenderedState list -> string) option }

let state title persona demonstrates body : State = { Title = title; Persona = persona; Demonstrates = demonstrates; Body = body }

let cov requirementId acceptanceId satisfied note : Coverage =
    { RequirementId = requirementId; AcceptanceId = acceptanceId; Satisfied = satisfied; Note = note }
