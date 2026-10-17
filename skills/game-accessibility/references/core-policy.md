# Core policy (load every time)

These rules apply to every game, genre and engine. They are the contract the rest of the skill
implements. When a rule conflicts with a habit, a tutorial, an older guideline or a shipped game you
are imitating, the rule wins.

## 1. What an accessibility option is

An option is a **player-chosen, reversible adjustment** that removes a specific barrier. It:

- is **off or at the designer's intended value by default**. The exception is safety (section 3).
- can be turned on, adjusted and turned off **at any time**: from the first-launch screen, the title
  menu and the pause menu, mid-level and mid-save, without restarting the campaign.
- is **available before the first moment the barrier appears**. Subtitles exist before the first
  line of speech. The flash option exists before the first flash. The QTE assist exists before the
  first QTE.
- changes **one barrier**. "Combat damage off" does not also remove fall death. "Minimal HUD" does
  not also remove captions. Players combine options. They are never forced into a bundle.
- **behaves exactly as its description says**. "Off" means zero. If something residual remains, call
  it "Reduced" and say what remains.

## 2. No-penalty rule (non-negotiable)

Using any option, at any time, for any length of time, must never:

- block, disable or change achievements, trophies, unlocks, collectibles, endings, credits or
  story content;
- mark, badge, rename or recolour a save slot, ending screen, grade, score, profile or stats.
  No `assisted` flag in save data. No "ever used assist" history;
- lower a grade, rank or reward, or be announced to other players;
- require failing first. An offer after repeated failure is fine as an *extra* discovery route, but
  the option is always available directly, and the offer can be dismissed for good;
- use shaming copy: "cheat", "baby mode", "not the intended experience", "we recommend trying
  without this first".

Achievements are evaluated from **what happened** (boss defeated, chapter completed), never from
which settings were active. A skip counts as completing the chapter. An objective that requires a
specific act ("collect 50 feathers") still needs a supported way to do that act, so design an
accessible route rather than an "award everything" button. Platform rule: Xbox XR-057 forbids unlocking
achievements from a menu option without the corresponding gameplay, and it explicitly allows options
that reduce the difficulty of that gameplay.

A visible "assist active" indicator is allowed **only as an opt-in player preference** shown in the
settings or HUD. It is never on save cards, endings or results.

Separate leaderboards for assisted play are an owner decision, not a default. If a project already
has difficulty-gated achievements or ranked rules that conflict with this policy, **report the
conflict to the developer** and propose redesigns. Do not silently implement a penalty, and do not
silently rewrite published achievement definitions.

### When the developer asks for a penalty

If the developer asks to disable, hide or lock achievements or trophies (or mark saves) when an
accessibility or assist option is on, **push back and recommend against it before writing any
code**. Tell them plainly:

- **It makes the player feel bad.** The player turned the option on because they needed it. Losing
  achievements for that tells them they are playing the game wrong, or are not welcome in it.
- **It makes players stop playing.** A player who sees achievements switch off, or has to choose
  between the option they need and the rewards everyone else gets, is likely to quit instead.
- **Worst case, they refund it.** A penalty can lead to refund requests. Steam's standard refund
  covers a request made [within 14 days of purchase **and** with under 2 hours of
  playtime](https://store.steampowered.com/steam_refunds/), and many players meet assist options in
  the settings menu or early play, which is often inside that window. Requests outside those terms
  can still be reviewed, but nothing guarantees approval either way. The same players are also the
  ones who write reviews and accessibility round-ups.
- **It is not needed.** Achievements that read events are already earned by playing. Control and
  Dead Cells keep all achievements with assists on (see the difficulty guide).

Then offer alternatives that keep a challenge: achievements for specific feats any player can
attempt, an opt-in "assist active" HUD reminder, or a separate challenge leaderboard if the owner
wants one. If the developer still insists after hearing this, it is their game: record the decision
and the warning given under "Conflicts with policy" in `ACCESSIBILITY.md`. Never describe the result as
accessible, and never add the penalty silently or without the warning.

## 3. Safe defaults (the only exceptions to "designer default")

| Area | Safe default | Why |
| --- | --- | --- |
| Photosensitivity | Content contains **no hazardous flashing in any mode**, including logos, menus, transitions and video. A warning screen is not a mitigation. "Reduce flashing" removes even non-hazardous flashes. | Seizure risk exists before any menu can be opened. |
| First-launch audio | Quiet master level. Music starts **after** volume is reachable and fades in. The only sound allowed before settings is one quiet, repeatable prompt offering spoken menus. | Surprise loudness hurts and startles; blind players still need an audible way in. |
| Camera shake / head bob / weapon sway | Moderate ("Reduced") unless the designer explicitly chooses otherwise. Full Off always exists. | Common motion-sickness trigger. |
| Settings access | Accessible settings are reachable **before** logos, intro video, music, tutorial and first speech. | The remedy must come before the barrier. |
| OS preferences | Honour supported system settings (reduce motion, screen reader active, increase contrast, caption style, text size) as the *initial* value. An explicit player choice always overrides them. | The player already told the device what they need. |
| Subtitles | Dialogue subtitles **On** by default, unless the designer decides otherwise, and offered on first launch either way. | Large share of players use them. Cheap. |
| Frame pacing | VSync or a frame cap on by default. Never uncapped by default. | Runaway GPU, heat and noise. |

## 4. Scope rules: what to add and what not to add

- **Baseline** areas are assessed in every game: first-launch access and persistence, readable and
  operable UI, remapping, information through more than one sense (not colour-only, not sound-only),
  photosensitivity, audio controls, pause, no-penalty policy and truthful documentation. Assessing
  means checking. A silent game needs no subtitle menu, but you must confirm it has no speech.
- **Contextual** options are added when a **signal** is present (see
  [signal-routing.md](signal-routing.md)). One mandatory occurrence counts. A single fishing
  minigame with timed input triggers the timing options for that minigame.
- **Advanced** options (full screen-reader gameplay, audio description, sign language, switch
  scanning) are **recorded with an explicit scope decision**. They are never silently dropped as
  "too expensive". Foundational menu semantics are Baseline, not Advanced.
- **Do not add options for mechanics that do not exist.** No infinite ammo without ammo. No
  arachnophobia mode without spider-like content. No FOV slider for a fixed 2D board. Record the
  absence and the evidence in `ACCESSIBILITY.md`.
- **Do not create new barriers while designing a feature.** Check every new feature against the
  signal list before writing it. Examples: health shown only by screen tint, a flickering light,
  a stealth enemy that is only audible, a colour-coded wiring puzzle. Fix the design or add the
  option at the same time as the feature.

## 5. Independence, presets and combinations

- Every option is an **independent setting**. Presets (for example "Less visual activity", "More
  time to respond", "Readable text", "Reduced physical effort", "Lower combat pressure", "More
  navigation help") are **editable patches**. They show exactly what they change, can be undone,
  and never overwrite a value the player set by hand on a later launch.
- **Umbrella controls never remove separate control.** An "all" switch such as "Reduce motion
  (all)" or "High-contrast outlines (all)" is an editable patch over its parts. Each part keeps its
  own persistent key whose default `follow` uses the umbrella and whose `on` or `off` overrides it,
  so the player can always set each effect or class separately (see
  [architecture.md](architecture.md) section 1). Never ship only the umbrella for distinct effects.
- Name presets by **what they do**, not by diagnosis. Never ask a player to disclose a disability.
- Options that pull in opposite directions coexist: guidance off **and** objective journal on;
  minimal HUD **and** captions on; reduced motion **and** camera assist. Test those combinations.

## 6. Wording

| Avoid | Use |
| --- | --- |
| Cheats, wimp/baby/story-only mode | Assists, gameplay adjustments, or the exact feature name |
| "Try without this first" | "Change these settings at any time." |
| "Easy: for players who only want the story" | "Enemies deal 50% damage. Timing windows are twice as long." |
| "Invincibility" when only enemy attacks are blocked | "Combat damage: Off. Falling and drowning are unchanged." |
| "Arachnophobia safe" | "Spider appearance: Simplified. Legs, eyes and skittering sounds replaced." |
| "Epilepsy safe" / "seizure free" | "Reduces flashing effects. The game was analysed with <tool, version>." |

Suggested menu intro, to be used **only after it has been verified**: *"Adjust controls, information,
presentation and gameplay to suit you. You can change these settings at any time. All story
content, endings and achievements remain available."*

## 7. Truthful claims

- Store pages, menus and documentation claim only what has been verified in the shipped build.
  "Narrated menus" is not "playable without sight". "Reduced flashing" is not "seizure safe".
- Every option description states its effect, scope and exclusions, and whether it needs a
  restart (avoid restarts).
- Known gaps are listed honestly in `ACCESSIBILITY.md` and in the public accessibility notes.

## 8. Openness: mods, hardware and assistive tools

- Gameplay reads **logical actions**, never physical keys. Every input device, including adaptive
  controllers, Steam Input, remapping software, voice tools and switch interfaces, enters through
  the same action layer.
- Settings and bindings live in a **documented, human-readable config file** that uses the same
  validation as the menus. It has a recovery path if a binding becomes unusable.
- Do not block third-party remappers, overlays or screen readers without a stated reason.
- If the game supports mods, expose stable hooks: settings registration, action dispatch, readable
  game and UI state for narration mods, and content-variant tags for filter mods.
  Mods extend access. They never replace native baseline access.

## 9. Process rules for agents

1. Keep `ACCESSIBILITY.md` (the per-project contract, from `assets/templates/ACCESSIBILITY.md`)
   current. Every mechanic, content type or screen you add updates its signal list and option table
   in the same change.
2. Build accessibility hooks **with** the feature, not in a later milestone. A QTE system ships
   with its auto-complete path. A camera ships with its shake scale. A dialogue system ships with
   its subtitle events.
3. Verify behaviour, not existence. A checkbox is not evidence. See [verification.md](verification.md).
4. Report results as `pass`, `fail`, `blocked`, `not tested` or `not applicable`. A missing tool is
   `blocked`, never `pass`.
