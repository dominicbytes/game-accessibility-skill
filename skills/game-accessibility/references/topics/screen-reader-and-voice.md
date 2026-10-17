# Screen readers, narration and voice

**Load when:** always for menus (S002, S003). Also icons without text (S080), partial or no voiceover
(S076), text-heavy UI (S073), rapidly changing state (S078), important visual story events (S077),
cinematic storytelling (S010), state shown only by shape or position (S005), world text (S064),
speech or microphone input (S009), text entry (S029), typed commands (S096). **Skip when:** never for
menus. Skip the gameplay and voice-input rows only when their signals are confirmed absent.

Incoming chat narration (R04) and speech-to-text chat: [multiplayer-and-communication](multiplayer-and-communication.md).
Narration volume and ducking: [audio-and-captions](audio-and-captions.md). Text that narration reads:
[text-and-readability](text-and-readability.md).

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| R01 | Menu narration / screen-reader support | `narration.enabled` | Off; on if a screen reader is active | B | V, C |
| R11 | Spoken screen changes and focus changes | `narration.enabled` | On with narration | B | V, C |
| R10 | Accessible names for meaningful images and icons | — (always on) | On | B | V |
| R05 | Narration speed | `narration.rate` | 1.0 (0.5–2.5) | C | V, C, H |
| R06 | Narration voice / pitch | — | System voice for the language | C | H, V |
| R07 | Narration verbosity | — | Standard | C | V, C |
| R08 | Repeat / stop narration commands | — | Bound when narration is on | C | V, C |
| R09 | Narration language | — | Game language | C | V, C |
| R12 | Read unvoiced dialogue and documents aloud | `narration.dialogue` | Off | C | V, C |
| R02 | Read status on demand (health, objective, position) | — | Off until bound | C | V, C |
| R03 | Automatic important status narration | — | Off | C | V |
| R04 | Incoming text-chat narration | — | Platform preference | C | V |
| R15 | Distinct gameplay audio cues for visual-only events | — | Designer | C | V |
| R17 | Compass-aligned orientation / snap turn | — | Off | C | V, M |
| R19 | OS voice-control compatibility (labels match visible text) | — (always on) | On | C | M |
| R20 | Short in-game voice commands | — | Off | C | M |
| R21 | Dictation for text fields | — | Off | C | M, C |
| R13 | Cinematic audio description | — | Off | A | V, C |
| R14 | Extended audio description (pauses scene) | — | Off | A | V, C |
| R16 | Spoken turn-by-turn navigation | — | Off | A | V, C |
| R18 | On-demand audio scan / sonar | — | Off | A | V |
| R22 | Non-verbal sound activation | — | Off | A | M |

Advanced rows (R13, R14, R16, R18, R22) get an explicit scope decision in `ACCESSIBILITY.md`.
Menu semantics and narration are Baseline, never Advanced.

## How to implement

### Choose the route per platform

Check the installed engine before using any API. Run the engine's `--version`. Documentation for a
newer engine is not proof that the API exists here.

| Route | What it gives | Limits |
|-------|---------------|--------|
| **Engine accessibility tree** (preferred) | The player's own screen reader reads names, roles, values and focus | Must be verified per engine version and export platform |
| — Godot 4.5+ / Redot 26.1+ | AccessKit-backed Controls: `accessibility_name`, `accessibility_description`, `accessibility_live`, labelled-by and described-by relations ([Godot 4.5 Control](https://docs.godotengine.org/en/4.5/classes/class_control.html), [release note](https://godotengine.org/article/godot-4-5-making-dreams-accessible/)) | Experimental. Desktop documented; check mobile and web per release. Redot 26.1 forks Godot 4.5.2 ([policy](https://docs.redotengine.org/en/About/release_policy)): lineage, not proof for your binary |
| — Unity 6.3 | `AccessibilityHierarchy` on Android 8+, iOS, Windows, macOS ([6000.3 docs](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/accessibility/accessibilityhierarchy)) | Unity 6.0 docs cover mobile only. `AccessibilitySettings` is Android and iOS only. Main window only |
| — Unreal Engine 5 | Experimental Slate/UMG screen-reader support, `Accessibility.Enable=1`; Windows readers and iOS VoiceOver documented ([Epic](https://dev.epicgames.com/documentation/en-us/unreal-engine/supporting-screen-readers-in-unreal-engine)) | Experimental; custom Slate widgets need work; test packaged builds |
| **TTS self-voicing** | Game speaks through system voices: Godot `DisplayServer.tts_*`, platform speech APIs | No OS focus, braille or reader navigation. Handle an empty voice list |
| **Screen-reader output library** | Sends strings to the running reader (speech, sometimes braille) | No accessibility tree; you build navigation |

Screen-reader output libraries (check licence and maintenance before vendoring):

| Library | Licence | Status |
|---------|---------|--------|
| [Tolk](https://github.com/dkager/tolk) | LGPL-3.0 | Windows; README says not currently developed |
| [Prism](https://github.com/ethindp/prism) | MPL-2.0 | Active in 2026; check backend and native build needs |
| [UniversalSpeech](https://github.com/qtnc/UniversalSpeech) | MIT | Windows |
| [SRAL](https://github.com/m1maker/SRAL) | MIT | Archived 2026-08-05 |
| [NVDA controller client](https://github.com/nvaccess/nvda/tree/master/extras/controllerClient) | LGPL-2.1 or later | NVDA on Windows only |

### Focus narration contract

- Every focused control announces **name, role, value, hint**, plus disabled or selected state:
  "Text size, slider, 150 percent", "Subtitles, checkbox, on", "Delete save, button, cannot be undone".
- Use localised strings and the **current binding names**. Never read an internal setting ID.
- Icon-only controls get a localised accessible name (R10). Images that carry meaning get a short
  purpose description, not a filename.
- Debounce slider changes; announce the settled value.
- Announce screen and dialog changes (R11). Dialogs trap focus and return it on close. Hidden menus
  behind a dialog are never read.
- Ordinary input and accessibility actions call the **same command handler**.

### Narrator queue

- `speak(text, priority, interrupt)`, `stop()`, `set_rate()`. A bounded queue with categories (focus,
  status, dialogue, critical).
- New focus speech interrupts stale focus speech. Critical announcements are never dropped.
- **No double speech.** Pick one route per announcement. If the OS reader is reading the engine tree,
  self-voice only what the tree does not expose (gameplay status), or nothing.
- Never wait for speech to finish before accepting input.
- Repeat and stop commands (R08) work from every supported input device.
- `narration.rate`, voice (R06), language (R09) and verbosity (R07) persist. Verbosity never removes
  essential state changes.

### First launch: an audible way in

- `narration.enabled` imports "screen reader active" as its first value. Do not turn on fallback TTS
  merely because no reader was detected.
- The first-launch screen is narratable before anything else. Give an **audible activation path**:
  a short, quiet, repeatable spoken or tonal prompt saying which button turns narration on, with its
  volume controllable. Do not make the player see small text to enable speech.
- The whole settings screen, presets list and confirmation dialogs are narrated.

### Status on demand and automatic status (R02, R03)

- Bind actions such as "read health", "read objective", "read position". Answer from game state, not
  screen pixels.
- Automatic announcements cover only time-critical changes, with priority above optional readouts.
  Recurring spoken state every 7–10 s is acceptable
  ([XAG 106](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/106));
  never interrupt more important speech.

### Dialogue and documents (R12)

- With `narration.dialogue` on, unvoiced lines, notes, signs and codex entries are read once, in full,
  from the same text the overlay shows. Voiced lines are not double-read.

### Audio description and nonvisual gameplay (R13–R18)

- Audio description is authored, timed and localised, mixed on its own bus, and never masks dialogue.
  It is separate from UI narration. Extended description pauses the scene when there is no gap.
- Gameplay cues (R15) need a learnable, previewable set: objective, obstacle, edge, hazard, interact.
  Provide a cue glossary to replay each one.
- Full nonvisual play is a separate Advanced scope. Plan it early if promised; never infer it from
  menu narration.

### Voice input and OS voice control (R19–R22)

- OS voice control (R19) works when visible labels and accessible names match. Use the platform's
  input-label hooks where available ([Apple](https://developer.apple.com/documentation/swiftui/view/accessibilityinputlabels(_:)-9q3yf),
  [Android](https://support.google.com/accessibility/android/answer/7158690)).
- Microphone mechanics (S009: singing, shouting, blowing, voice spells) dispatch a logical action.
  Give each one a local button, menu or text alternative (M23) that triggers the same action and
  result. Multiplayer text chat (K01) belongs to communication signals, not to this mechanic.
- In-game recognition (R20) is optional and **never required**. Options:
  [Vosk](https://github.com/alphacep/vosk-api) (Apache-2.0; check model licences),
  [whisper.cpp](https://github.com/ggml-org/whisper.cpp) (MIT; check model weights), or platform
  speech APIs. Results map to an **allow-list of logical actions** through the normal action layer.
- Show listening state; request the microphone only when the feature is used; offer stop and cancel;
  confirm destructive actions; debounce repeats; say whether processing is local or remote.
- Prefer short words from a small vocabulary ("yes", "open"), and never execute free transcripts.
- Dictation (R21) lets the player review and edit before sending. Non-verbal activation (R22) needs a
  calibrated threshold and a switch alternative.

### Claims

Distinguish **narrated menus**, **partly narrated gameplay**, **described cinematics** and
**independent nonvisual play**. Record which one was verified. "Narrated menus" never becomes
"playable without sight". See [store-claims-and-compliance](store-claims-and-compliance.md).

## Numbers that matter

| Source | Number | Scope |
|--------|--------|-------|
| [XAG 106](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/106) | 7–10 s | Acceptable interval for recurring spoken state |
| Registry `narration.rate` | 0.5–2.5 × | Player range; persist the choice |

## Shipped examples

- Diablo IV: built-in screen reader plus third-party reader support at launch; later added compass and
  audio navigation. [Blizzard launch](https://news.blizzard.com/en-gb/article/23954932/combatting-demons-with-accessibility-in-diablo-iv),
  [later](https://news.blizzard.com/en-us/article/24139219/a-crucible-for-allaccessibility-features-in-vessel-of-hatred).
- Mortal Kombat 1: first-launch screen-reader setup and descriptive audio for story and finishers.
  [WB Games](https://mortalkombatgamessupport.wbgames.com/hc/en-us/articles/19435787154963-Mortal-Kombat-1-Accessibility-Features-A11Y).
- Age of Empires IV: first-launch and main-menu narration.
  [Age of Empires](https://www.ageofempires.com/age-iv-accessibility/). Not evidence that battlefield
  information is narrated.
- Forza Motorsport: Blind Driving Assists with cue previews, screen narration with voice and verbosity
  settings, built with blind players. [Xbox Wire](https://news.xbox.com/en-us/2023/04/27/forza-motorsport-accessibility-features-blind-driving/).
- The Last of Us Part I: cinematic audio description; Part II Remastered also verified.
  [PlayStation Blog](https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/),
  [Naughty Dog](https://feedback.naughtydog.com/hc/en-us/articles/22879676199188-Does-The-Last-of-Us-Part-II-Remastered-have-Audio-Descriptions).
  The 2020 Part II release does not inherit the remaster's description.
- Marvel's Spider-Man 2: screen reader and audio description arrived in a March 2024 update.
  [PlayStation Blog](https://blog.playstation.com/2024/02/21/marvels-spider-man-2-update-adds-new-game-plus-and-new-suits-on-march-7).
- Phasmophobia: text alternatives for Spirit Box and Ouija questions, so voice is not required.
  [Kinetic Games](https://kineticgames.co.uk/news/phasmophobia-tempest).
- Ren'Py: engine self-voicing mode. [docs](https://www.renpy.org/doc/html/self_voicing.html).
- Cautionary: Street Fighter 6 launch coverage reports missing menu and tutorial narration despite
  strong fight audio ([CIPT](https://caniplaythat.com/2023/05/31/street-fighter-6-accessibility-hits-and-misses/));
  God of War Ragnarök: Valhalla review documents unread menus and areas needing sighted help
  ([SightlessKombat](https://www.sightlesskombat.com/god-of-war-ragnarok-valhalla-accessibility-review/)).
- Stardew Access is a community **mod** (GPL-3.0), not native support.
  [GitHub](https://github.com/stardew-access/stardew-access).

## Verify

- [ ] Fresh profile with the target screen reader running (NVDA, Narrator, VoiceOver, TalkBack as
      shipped): the first screen is announced; settings, presets and dialogs are reachable and read.
- [ ] Fresh profile without a reader: the audible activation prompt plays quietly and turns
      narration on; volume and repeat work.
- [ ] Tab or d-pad through every menu: each control reads name, role, value and hint; focus order is
      logical; dialogs trap and return focus; no internal IDs are spoken.
- [ ] Rebind a key: the next spoken hint uses the new binding.
- [ ] Hold a slider key: one settled value is announced, not a flood.
- [ ] With an OS reader and self-voicing both on, nothing is spoken twice.
- [ ] Every mandatory screen (inventory, shop, rewards, save, confirmation, game over) is narrated.
      Record coverage as a matrix.
- [ ] Status-on-demand actions answer correctly mid-gameplay; critical alerts interrupt stale speech.
- [ ] Voice commands: only allow-listed actions run; destructive ones confirm; the game is fully
      playable with the microphone off.
- [ ] Test the **packaged** build on each target platform, not only the editor.
- [ ] Narration settings persist across restart and are not stored in saves.

## Common mistakes

- A speaking title screen called "screen reader support" → cover every mandatory screen.
- TTS that reads a string on hover only → narrate focus changes for keyboard and controller.
- Engine tree plus self-voicing reading the same thing → one route per announcement.
- Narration that blocks input until speech ends → speech is asynchronous.
- Turning on TTS because no reader was found → offer an audible opt-in instead.
- Reading setting IDs or raw keycodes → localised names and current bindings.
- Voice input that executes free transcripts → allow-list plus confirmation; never required.
- "Playable without sight" after adding menu narration → claim only what was verified.
