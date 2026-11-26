/// S-013 Notification preferences (port of tools/screens/S-013.mjs).
module Unbound.Designs.V1.Screens.S013

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let private scope =
    scopeTrail [ "Example Charitable Foundation"; "Harbor Giving donor-advised funds"; "Avery Lindqvist"; "Notifications" ]

/// A switch; `on` checks it and `locked` disables it.
let private sw (id: string) (label: string) (desc: string) (on: bool) (locked: bool) =
    let chk = if on then " checked" else ""
    let dis = if locked then " disabled" else ""
    tag
        "switch"
        $$"""<label class="forma-switch" data-forma-motion-weight="standard">
  <input class="forma-switch__input" type="checkbox" role="switch" id="{{id}}" name="{{id}}" value="on"{{chk}}{{dis}} aria-describedby="{{id}}-description">
  <span class="forma-switch__track" aria-hidden="true"></span>
  <span class="forma-switch__text"><span class="forma-switch__label">{{label}}</span><span class="forma-switch__description" id="{{id}}-description">{{desc}}</span></span>
</label>"""

/// A notification class. channels: (channel, label, description, on, locked).
let private klass (p: string) (key: string) (title: string) (intro: string) (channels: (string * string * string * bool * bool) list) =
    let switches = channels |> List.map (fun (ch, label, desc, on, locked) -> sw $"{p}-{key}-{ch}" label desc on locked) |> String.concat ""
    $$"""<fieldset class="forma-stack"><legend><strong>{{title}}</strong></legend><p>{{intro}}</p>
<div class="forma-form-row">{{switches}}</div></fieldset>"""

let private preferences (p: string) =
    let ack =
        klass p "ack" "Contribution acknowledgments" "Your tax receipt for each contribution." [
            "email", "Email", "Required: tax acknowledgment", true, true
            "app", "In-app", "Required: tax acknowledgment", true, true
        ]
    let sec =
        klass p "sec" "Security alerts" "Sign-ins from a new device and changes to who can access your fund." [
            "email", "Email", "Required: account security", true, true
            "sms", "Text message", "Required: account security", true, true
        ]
    let grant =
        klass p "grant" "Grant updates" "Approved, paid, held or returned grants. These always appear in your activity list." [
            "email", "Email", "Optional", true, false
            "sms", "Text message", "Optional", false, false
        ]
    let news = klass p "news" "Program news" "Occasional news from Harbor Giving." [ ("email", "Email", "Optional", false, false) ]
    let why =
        callout
            $"{p}-why"
            "note"
            "Why some notices cannot be turned off"
            "<p>Example Charitable Foundation must send tax acknowledgments and security alerts. You can still choose optional channels for everything else.</p>"
    $$"""{{scope}}<form class="forma-form" id="{{p}}-f" aria-labelledby="{{p}}-ft"><h3 id="{{p}}-ft">How we contact you</h3>
{{ack}}
{{sec}}
{{grant}}
{{news}}
{{why}}
<div class="forma-actions"><button type="submit">Save preferences</button></div>
<p class="forma-form__outcome" id="{{p}}-saved" role="status" aria-label="Save status">Preferences saved.</p>
</form>"""

let screen: Screen =
    { Id = "S-013"
      File = "S-013-notification-preferences.html"
      Title = "Notification preferences"
      ReqIds = [ "UNB-UI-S-013" ]
      Audience = "donor"
      Brand = true
      Lead = "Switch optional notifications on or off per channel. Required legal and security notices are always on, with the reason."
      States =
        [ state
              "Preferences with required classes locked on"
              "Donor"
              "UNB-UI-S-013 AC-1 (acknowledgment switch on and disabled with 'Required: tax acknowledgment')"
              preferences ]
      Coverage =
        [ cov "UNB-UI-S-013" "AC-1" Yes "Acknowledgment switches are checked and disabled, each described by 'Required: tax acknowledgment'. Disabled switch styling is opacity 0.62 on text; reason is in the description text (FORMA-GAP-11)." ]
      Gaps = [ "FORMA-GAP-06"; "FORMA-GAP-11" ]
      Render = None }
