import { tag, callout, scopeTrail } from "../lib/markup.mjs";

const scope = scopeTrail(["Example Charitable Foundation", "Harbor Giving donor-advised funds", "Avery Lindqvist", "Notifications"]);

const sw = (id, label, desc, { on = false, locked = false } = {}) =>
  tag(
    "switch",
    `<label class="forma-switch" data-forma-motion-weight="standard">
  <input class="forma-switch__input" type="checkbox" role="switch" id="${id}" name="${id}" value="on"${on ? " checked" : ""}${locked ? " disabled" : ""} aria-describedby="${id}-description">
  <span class="forma-switch__track" aria-hidden="true"></span>
  <span class="forma-switch__text"><span class="forma-switch__label">${label}</span><span class="forma-switch__description" id="${id}-description">${desc}</span></span>
</label>`
  );

const klass = (p, key, title, intro, channels) => `<fieldset class="forma-stack"><legend><strong>${title}</strong></legend><p>${intro}</p>
<div class="forma-form-row">${channels.map(([ch, label, desc, opts]) => sw(`${p}-${key}-${ch}`, label, desc, opts)).join("")}</div></fieldset>`;

export default {
  id: "S-013",
  file: "S-013-notification-preferences.html",
  title: "Notification preferences",
  reqIds: ["UNB-UI-S-013"],
  audience: "donor",
  brand: true,
  lead: "Switch optional notifications on or off per channel. Required legal and security notices are always on, with the reason.",
  states: [
    {
      title: "Preferences with required classes locked on",
      persona: "Donor",
      demonstrates: "UNB-UI-S-013 AC-1 (acknowledgment switch on and disabled with 'Required: tax acknowledgment')",
      body: (p) => `${scope}<form class="forma-form" id="${p}-f" aria-labelledby="${p}-ft"><h3 id="${p}-ft">How we contact you</h3>
${klass(p, "ack", "Contribution acknowledgments", "Your tax receipt for each contribution.", [
  ["email", "Email", "Required: tax acknowledgment", { on: true, locked: true }],
  ["app", "In-app", "Required: tax acknowledgment", { on: true, locked: true }],
])}
${klass(p, "sec", "Security alerts", "Sign-ins from a new device and changes to who can access your fund.", [
  ["email", "Email", "Required: account security", { on: true, locked: true }],
  ["sms", "Text message", "Required: account security", { on: true, locked: true }],
])}
${klass(p, "grant", "Grant updates", "Approved, paid, held or returned grants. These always appear in your activity list.", [
  ["email", "Email", "Optional", { on: true }],
  ["sms", "Text message", "Optional", { on: false }],
])}
${klass(p, "news", "Program news", "Occasional news from Harbor Giving.", [["email", "Email", "Optional", { on: false }]])}
${callout(`${p}-why`, "note", "Why some notices cannot be turned off", "<p>Example Charitable Foundation must send tax acknowledgments and security alerts. You can still choose optional channels for everything else.</p>")}
<div class="forma-actions"><button type="submit">Save preferences</button></div>
<p class="forma-form__outcome" id="${p}-saved" role="status" aria-label="Save status">Preferences saved.</p>
</form>`,
    },
  ],
  coverage: [
    { requirement_id: "UNB-UI-S-013", acceptance_id: "AC-1", satisfied_by_markup: true, note: "Acknowledgment switches are checked and disabled, each described by 'Required: tax acknowledgment'. Disabled switch styling is opacity 0.62 on text; reason is in the description text (FORMA-GAP-11)." },
  ],
  gaps: ["FORMA-GAP-06", "FORMA-GAP-11"],
};
