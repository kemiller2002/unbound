/// A screen and its states, as each screen module exports them (port of the `export default` objects).
module Unbound.Designs.Screen

/// What a state renders: an application page (framed by Ui.appPage) or a complete document of its own.
type Content =
    | App of Ui.Page
    | Html of string

type State =
    { Id: string
      Title: string
      Persona: string
      Scenarios: string list
      /// None renders as "app" in pages.json.
      Kind: string option
      Content: Content }

type Screen = { Id: string; Slug: string; Title: string; Requirement: string; States: State list }

/// A state rendered as an application page.
let app (id: string) (title: string) (persona: string) (scenarios: string list) (page: Ui.Page) : State =
    { Id = id; Title = title; Persona = persona; Scenarios = scenarios; Kind = None; Content = App page }
