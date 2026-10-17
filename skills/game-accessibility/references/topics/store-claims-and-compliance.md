# Store claims and compliance

**Load when:** B06 truthful documentation (every release), S107 grades, ranks or achievements,
S130 competitive ranking, S123/S124 online voice or text chat (legal triage), or any question
about store tags, certification or accessibility law ([signal-routing.md](../signal-routing.md)).
**Skip when:** a private prototype with no store page, no achievements or scores, and no
communication between players.

This guide covers what you may **say** about the game. It is not legal advice: legal questions go
to the developer's legal owner.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| L16 | In-game accessibility feature guide | — | On | B | All |
| L17 | Public accessibility documentation (store page, README) | — | On | B | All |
| L18 | Accessible support channels | — | On | B | All |
| K22 | Accessible communication setup path | — | On | B | V,H,M,C |
| K09 | Real-time text (if chat may be regulated) | `game.chat.rtt` | Intended; a legal duty may apply | C | H,speech |
| — | Optional "active assists" reminder in HUD | `system.assist_indicator` | Off | C | C |

(Tier: B Baseline, C Contextual, A Advanced. Helps codes as in
[option-catalog.md](../option-catalog.md).) The assist reminder is a player preference only. It
never appears on saves, endings, scores or profiles ([core-policy.md](../core-policy.md) section 2).

## How to implement

### 1. Claim only what was verified

Store metadata **reports** a capability; it does not create one. Build a claim matrix in
`ACCESSIBILITY.md` before filling any store form:

| Claim (store field) | Catalogue IDs | Result (`pass` only) | Platforms | Languages | Modes | Build |
|---|---|---|---|---|---|---|

- A tag is claimable only when **every** required ID is `pass` in the shipped build, for that
  platform, language and mode. `blocked` or `not tested` means no claim. Re-check every patch.
- "Narrated menus" is not "Playable without vision". "Reduced flashing" is not "seizure safe".
- The mappings below are this skill's **minimum evidence**, not the store's rules. Read the
  store's current criteria text before ticking a box.

### 2. Steam: 19 accessibility features

Steamworks has a shipped developer wizard, store-page display and search filters with 19 features
([Steamworks accessibility features](https://partner.steamgames.com/doc/accessibility_features)).
Do not invent Steam fields for mono audio, phobia modes, remapping or flash reduction; describe
those in your accessibility notes instead.

| Steam feature | Group | Must be verified true |
|---|---|---|
| Adjustable Difficulty | Gameplay | D01 or the relevant D03-D33 assists, changeable during play (D02) |
| Save Anytime | Gameplay | D25: save and return to the same situation |
| Custom Volume Controls | Audio | H01-H05 for every bus the game has (H07, H08 if present) |
| Narrated Game Menus | Audio | R01, R11 across **all** menus, setup and dialogs |
| Stereo Sound / Surround Sound | Audio | H10, positional meaning kept; surround tested on a real surround setup |
| Adjustable Text Size | Visual | V01, V02 (and H24 if subtitles exist) |
| Subtitle Options | Visual | H17 plus presentation options such as H24, H28 |
| Color Alternatives | Visual | V25 always; V23 or V24 |
| Contrast Controls | Visual | V14 and/or V26-V29 |
| Camera Comfort | Visual | The present motion signals' O-rows (O01, O02, O04, O05 ...) |
| Playable without Vision | Visual | Whole game finishable without sight: R01-R18 as needed, tested by blind players. Menu narration alone never qualifies. |
| Keyboard Only / Mouse Only / Touch Only Option | Input | M21 / M22 / M24 |
| Playable without Quick Time Events | Input | M16 for **every** QTE, or the game has none |
| Playable at Your Own Pace | Input | No required action under time pressure, or every one is removable: D27, D29, M16/M17, D14, C01. Read Steam's definition. |
| Chat Text-to-speech | Input | **R04**: incoming text chat narrated to the player in real time, including canned or quick-chat responses ("Good job!"), as Valve defines it. Verify with received messages and canned responses at the chosen speech volume. K04 (speaking the player's own outgoing text) is a separate capability and does **not** satisfy this field. |
| Chat Speech-to-text | Input | K03: incoming voice chat transcribed in real time, shown with text chat |

### 3. Accessible Games Initiative (AGI) and Xbox

The [AGI criteria (March 2025)](https://accessiblegames.com/wp-content/uploads/2025/03/Accessible-Games-Initiative-Tags-and-Criteria-March-2025.pdf)
contain **24 tags** (the communication tag appears twice but counts once).
[Xbox's tag documentation](https://learn.microsoft.com/en-us/xbox/accessibility/accessibility-feature-tags)
(v2.0.1, 14 March 2025) uses the AGI tags plus **6 Xbox-only tags**, across its storefronts since
[July 2025](https://news.xbox.com/en-us/2025/07/09/accessible-games-initiative-tags-xbox-interview/).
Do not use the older 2021 "20 tags" list. Two traps: Full Input Remapping includes analog actions and presets alone are not
enough; Save Anytime means returning to the saved situation, not a nearby checkpoint.

| Tag | Must be verified true |
|---|---|
| Multiple Volume Controls | H01-H05 for each bus present |
| Mono Sound | H09 |
| Stereo Sound / Surround Sound | H10 for the claimed output |
| Narrated Menus | R01, R11 across all menus |
| Difficulty Levels | D01 (with D02 recommended) |
| Save Anytime | D25, exact situation restored |
| Basic Input Remapping | M01 to at least the scope AGI defines for Basic |
| Full Input Remapping | M01 for every action including analog (M06); M02 helps |
| Stick Inversion | M07 |
| Playable without Button Holds | M13 for every hold, or none exist |
| Playable without Rapid Button Presses | M14 or M15 for every mash, or none exist |
| Playable with Keyboard Only / Mouse Only | M21 / M22 |
| Playable with Buttons Only / Touch Only | M23 / M24 |
| Playable without Motion Controls | M25 |
| Playable without Touch Controls | L05, L13: every required action works without touch |
| Chat Speech-to-Text & Text-to-Speech | K03 and K04; K12 (AGI: at least the last 4 messages) |
| Clear Text | V01, V03 and default size thresholds in [option-catalog.md](../option-catalog.md#numeric-thresholds) |
| Large Text | V01 reaching the AGI Large Text sizes |
| Large & Clear Subtitles | H17, H24, H28 at AGI subtitle sizes |
| Color Alternatives | V25; V23 or V24 |
| Camera Comfort | O-rows for the motion signals present |
| *Xbox:* Additional accessibility information / Accessibility on launch / On-demand tutorials | L17 / L01 / C05 |
| *Xbox:* Pausable / Adjustable input sensitivity / Single stick gameplay | D27 / M08 / M20 |

### 4. PlayStation

Sony's [announcement](https://blog.playstation.com/2023/04/03/accessibility-tags-roll-out-this-week-on-playstation-store-on-the-ps5-console/)
confirms **over 50 tags in six groups** (Visual, Audio, Subtitles/Captions, Controls, Gameplay,
Online Communication), with examples such as large text, mono audio, screen reader, remapping,
alternatives to holds and mashing, puzzle skips, QTE simplification and transcription. PS4 and
PS5 versions can show different tags. A complete public tag list with criteria is **UNVERIFIED**:
use only names from authorised Sony documentation, and never apply Xbox numbers to Sony tags. The
[SpecialEffect observed list](https://gameaccess.info/ps5-accessibility-tags-playstation-store/) is
a subset, not the specification.

### 5. Apple Accessibility Nutrition Labels

[Apple's labels](https://developer.apple.com/help/app-store-connect/manage-app-accessibility/overview-of-accessibility-nutrition-labels/)
list nine capabilities. A label claims support for the app's **common tasks** on that platform, and
ad experiences can affect the claim. The rollout was described as voluntary at first; a mandatory
deadline is **UNVERIFIED**.

| Label | Must be verified true |
|---|---|
| VoiceOver | R01, R11, R10 through the platform accessibility tree for common tasks |
| Voice Control | R19 |
| Larger Text | V01 reaching **200% or more** |
| Dark Interface | V14 |
| Differentiate Without Color Alone | V25 |
| Sufficient Contrast | V14 plus contrast thresholds |
| Reduced Motion | N05, N10 and present O-rows, honouring the system setting (L04) |
| Captions | H17, H19 |
| Audio Descriptions | R13 |

### 6. Nintendo and Google Play

- The [Switch 2 guide](https://www.nintendo.com/en-gb/Support/Troubleshooting/Guide-to-Accessibility-Features-on-Nintendo-Switch-2-2843775.html)
  confirms **system** features: TTS, GameChat speech/text conversion, text and contrast, zoom,
  colour changes, remapping and mono audio. They do not prove a game's menus work with system
  TTS: never list them as game features. A public Nintendo game tag scheme is **UNVERIFIED**.
- A Google Play equivalent of the Steam fields is **UNVERIFIED**. Use the description and notes.

### 7. Achievements, trophies and leaderboards

Public platform facts:

| Platform | What is public | Do not infer |
|---|---|---|
| Steam | Developers define achievement conditions and unlock them through the stats/achievements API. No accessibility-based disqualification rule in the public docs. [Steamworks achievements](https://partner.steamgames.com/doc/features/achievements) | That every agreement or anti-cheat permits every implementation |
| Xbox | [XR-057](https://learn.microsoft.com/en-us/gaming/gdk/docs/store/policies/xr/xr057): achievements need corresponding gameplay; unlocking through purchases, cheat codes or menu options without that gameplay is disallowed. It **explicitly allows** options that reduce the difficulty of that gameplay. | That penalties are required, or that an "unlock all" button is allowed |
| PlayStation | A universal rule on difficulty-gated trophies is **UNVERIFIED**. One game's policy is not the TRC. | Rules from leaked or confidential manuals |
| Nintendo | A public lotcheck rule on accessibility and achievements is **UNVERIFIED**. [Nintendo process](https://developer.nintendo.com/the-process) | Anything not in authorised docs |

The skill's rule is **stricter** than any platform:

- Progression emits events (`chapter_completed`, `boss_defeated`). Achievements read events,
  never assist settings, preset names or an "ever used assist" flag.
- Turning an assist on or off never clears progress or earned rewards.
- Never send assist or preset data to platform stats, rich presence or public profiles.
- A skip counts as completing the chapter. An act-specific objective ("collect 50 feathers")
  needs a supported way to do the act, not an award button (XR-057).
- If the project already has difficulty-gated achievements, **report the conflict** to the
  developer with redesign options. Never rewrite published achievement definitions silently.
- Test unlocks with the platform's development tools; a local toast is not a platform unlock.

**Leaderboards.** Steam supports several named boards
([Steamworks leaderboards](https://partner.steamgames.com/doc/features/leaderboards)), so separate
boards are possible, but that is not a policy recommendation. Default: never disqualify, hide or
label a score because an option is on. Presentation, captions, bindings, screen readers and
content filters never change the board. A separate board for an openly chosen ruleset that anyone
can pick is an owner decision. Mandatory assist segregation contradicts core policy: report it,
do not build it.

### 8. Legal triage for communication features

Not legal advice. Record the facts below, then ask the developer's legal owner the questions.

| Area | Verified fact | Source |
|---|---|---|
| US, CVAA | Duties cover advanced communications services, including relevant game chat, subject to achievability and exceptions. The FCC game waiver ended 31 December 2018. | [47 CFR Part 14](https://www.ecfr.gov/current/title-47/chapter-I/subchapter-A/part-14), [FCC 2017 order](https://docs.fcc.gov/public/attachments/DA-17-1243A1.pdf), [FCC 2012 order](https://docs.fcc.gov/public/attachments/DA-12-1645A1.pdf) |
| EU, EAA | Applies from 28 June 2025 to listed products and services. Annex I asks for real-time text in addition to voice, and total conversation where video is provided. Article 4(5) exempts microenterprises providing services. | [Directive (EU) 2019/882](https://eur-lex.europa.eu/eli/dir/2019/882/oj/eng) |
| EU, EECC | Recital 17 and Article 2(5) exclude a minor, purely ancillary facility intrinsically linked to another service, and discuss online games. Neither "all game chat is covered" nor "game chat is always exempt" is supportable. | [Directive (EU) 2018/1972](https://eur-lex.europa.eu/eli/dir/2018/1972/oj/eng) |
| UK | Equality Act service providers have an anticipatory reasonable-adjustment duty. Public-sector app rules apply to public bodies, not every studio. Northern Ireland is separate. EAA is not UK law. | [Equality Act guidance](https://www.gov.uk/government/publications/equality-act-guidance/disability-quick-start-guide-for-service-providers-html), [Public sector rules](https://www.gov.uk/guidance/accessibility-requirements-for-public-sector-websites-and-apps) |

Questions to escalate:

1. Which countries, storefronts and consumer services are in scope?
2. Who operates chat, accounts, payments and moderation: the developer, the platform or a vendor?
3. Is voice, text or video free-form, ephemeral, persistent, or needed to take part?
4. Are lobby entry and any required character or server selection accessible (K22)?
5. Are incoming transcription (K03), typed speech (K04) and local narration (R04) supported in
   each shipped language?
6. Is real-time text required, and is the build real RTT (K09) rather than send-button chat?
7. Which exemptions, transition rules, records and complaint or support duties apply?
8. Which claims are recommendations, store criteria, platform contract terms or law?

### 9. Public accessibility notes template

Publish on the store page, website or README (L17). Keep it in sync with the claim matrix.

```markdown
## Accessibility notes: <Game> <version>, <platform(s)>
Tested build: <version/date>. Languages tested: <list>.

You can change every setting at any time, from the first screen, title menu and pause menu.
All story content, endings and achievements remain available with any setting.

### Features (example lines: list only what passed verification)
- Text: size 100-200%, readable font, solid text backgrounds.
- Subtitles: on by default; speaker names; sound captions with direction; size and background.
- Controls: full remapping (keyboard, mouse, controller); hold-to-toggle; QTE: Slow or Auto.
- Gameplay: damage taken 0-100%; safe falls; skip any puzzle or chase with full rewards.
- Motion: camera shake 0-100% (default 50%); head bob; motion blur; FOV 60-110.
- Flashing: no hazardous flashing in any mode; "Reduce flashing" removes the rest. Analysed
  with <tool, version>.

### Limits and known gaps
- <e.g. Menus are narrated; gameplay is not fully playable without sight.>
- <e.g. Subtitles in English and French only. Voice chat has no transcription yet.>

Contact: <accessible support channels, not voice-only (L18)>
```

## Numbers that matter

Apple Larger Text is 200% or more; AGI chat history is at least 4 messages. Text and subtitle
sizes, and the AGI "4:5:1" misprint (read 4.5:1): [option-catalog.md](../option-catalog.md#numeric-thresholds).

## Shipped examples

- The Last of Us Part I: trophies are not limited by difficulty. [PlayStation Blog](https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/). One game, not a platform rule.
- Control: assists do not affect trophies or achievements. [Remedy](https://legacy.controlgame.com/es/game-help-on-playstation-5-and-assist-mode-in-control-a-deep-dive/).
- Dead Cells (Breaking Barriers): achievements stay enabled with the new options. [Steam announcement](https://store.steampowered.com/news/posts/?appgroupname=Dead+Cells&appids=588650&enddate=1658241276&feed=steam_community_announcements).
- Do not copy: Steelrising made some difficulty achievements unavailable once Assist Mode was used
  in a playthrough ([Xbox Wire](https://news.xbox.com/en-us/2022/09/08/steelrisings-assist-mode-feature/);
  criticised by [Digital Trends](https://www.digitaltrends.com/gaming/steelrising-review-ps5/)). Patched behaviour UNVERIFIED.
- Do not copy: Celeste's Assist Mode marks the save card ([Celeste wiki](https://celeste.ink/wiki/Assist_Mode)).
  Borrow its agency-focused wording, not the marker.

## Verify

- [ ] Every ticked store field has a claim-matrix row whose IDs all `pass` in the shipped build,
      split by platform, language and mode.
- [ ] "Playable without Vision" (if claimed) was checked by a full playthrough without sight.
- [ ] Steam "Chat Text-to-speech" (if claimed): received text messages and canned responses are
      read aloud to the player (R04). Outgoing typed-message speech (K04) alone does not qualify.
- [ ] Achievement, trophy and leaderboard code has no reads of `assist`, `difficulty`,
      `settings` or `preset` (grep).
- [ ] Assists on, then off, mid-save: the next achievement still unlocks in the platform dev tools.
- [ ] Saves hold no assist history; save cards, endings and results show no badge; scores with
      options on reach the normal board.
- [ ] Legal questions for each communication feature were sent, answer or "pending" recorded.
- [ ] Public notes list known gaps; the achievements sentence in the menu intro is verified.

## Common mistakes

- Ticking "Narrated Game Menus" because the main menu talks → every menu, dialog and setup screen.
- Claiming "Playable without QTEs" with one auto-QTE missing → check every QTE, or no claim.
- Ticking Steam "Chat Text-to-speech" for outgoing TTS (K04) → the field means incoming chat
  narration (R04), including canned responses.
- "Epilepsy safe" or "seizure free" → "Reduces flashing. Analysed with <tool, version>."
- Copying Celeste's save marker or Steelrising's achievement lock → achievements read events only.
- An "unlock all achievements" accessibility button → design an accessible route to the act (XR-057).
- Giving a legal answer about CVAA or EAA → record facts and escalate the questions.
- Store page never updated after a patch removed a feature → re-run the claim matrix every release.
