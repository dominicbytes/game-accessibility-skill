# 04 — Engine support, open-source implementations, and reusable code

Research date: 2026-10-17. Assignment: engine capabilities, reusable implementations, algorithms, and modding hooks.

## Scope and evidence rules

- This report is research, not a claim that any listed package has been compiled or tested locally.
- “Built in” means that an engine or platform supplies an API or implementation; the game still needs accessible content, configuration, and testing.
- “Recommended” means a useful starting point subject to the stated version, platform, and licence checks.
- Proposed implementation choices, defaults, priorities, and sample files below are recommendations, not claims about existing engine defaults.
- Baseline means assess in every game; Contextual means implement when the relevant mechanic or presentation exists; Advanced means substantial integration or content work.
- UNVERIFIED means the research did not establish the fact. It does not mean the feature or licence does not exist.
- Repository stars are the counts visible in fetched GitHub pages, not a live API census, quality score, or accessibility certification.
- “Latest observed commit” means the newest dated commit displayed by the fetched branch history. Cached pages may omit newer work.
- A listing date, archive date, release date, and repository-update date are deliberately labelled separately.
- Several GitHub histories and the GitHub REST API were unavailable. Missing dates remain UNVERIFIED; search crawl dates are not substituted.
- Source-available commercial SDKs and engine examples are separated from permissively licensed reusable code.
- The report follows the brief's optional-assist rule: no achievement restrictions, content locks, save marking, or shame labels.
- The brief's historical “ASSIST ON” badge should not become a requirement for the new skill: player-visible disclosure should be optional and useful to that player.

## 1. Decisions that apply across engines

| Capability | What it actually supplies | What it does not establish | Implementation recommendation |
| --- | --- | --- | --- |
| Accessibility tree | Names, roles, values, relationships, actions, focus, and state for assistive technology | Whether gameplay can be understood or completed without sight | Prefer the engine's tree bridge for ordinary UI; add semantic gameplay descriptions. [AccessKit][AK] |
| TTS | Speech synthesis from strings | Screen-reader navigation, braille, OS focus, switch scanning, or accessible controls | Treat as a separate narration backend, with cancellation and player settings. [Godot TTS][G-TTS] |
| Screen-reader output DLL | Sends speech, sometimes braille, through an installed reader | A navigable OS accessibility tree | Useful for self-voicing gameplay and legacy engines; implement navigation separately. [Tolk][TOLK] |
| Input action system | Separates intent from physical buttons | A complete remapping screen, conflict handling, persistence, or alternative interaction modes | Implement remapping, recovery, and prompts together. [Unity bindings][U-INPUT] |
| Font/UI scaling | Changes rendered size or layout metrics | Legibility if controls clip, overlap, shrink automatically, or miss glyphs | Reflow and test the largest scale in every supported language. [CanvasScaler][U-SCALE] |
| Colour simulation | Approximates a colour-vision deficiency for design review | A treatment or universally helpful player correction | Keep simulation in developer tooling; offer semantic palettes to players. [Brettel][P-BRETTEL] |
| Subtitles | Displays authored timed text | Complete captions for non-speech audio or an accessible sound-direction system | Use authored semantic audio events, not only a waveform visualizer. [Unreal dialogue][E-DIALOGUE] |

### Shared option contracts

These are proposed contracts for samples in every engine folder. The cited resources support the underlying methods; defaults and priority assignments are this report's recommendations.

| Option | Helps / context signal | Recommended default / tier | Implementation / pitfall | Concrete reference |
| --- | --- | --- | --- | --- |
| Menu narration | Vision, reading difficulty; any graphical menu | Follow detected assistive technology where supported; explicit opt-in for fallback TTS / Baseline assessment | Expose name, role, value, action, and focus; avoid simultaneous native and custom speech | Unity's LetterSpell sample [U-SAMPLE] |
| Gameplay narration | Vision; meaningful information exists outside ordinary widgets | Off unless player enables it / Contextual, sometimes Advanced | Describe available actions and state changes; offer repeat/query commands instead of voicing each frame | Stardew Access [M-STARDEW] |
| Remappable actions | Motor, situational; any direct control | Designer bindings with complete alternatives / Baseline | Include menus, secondary bindings, cancellation, reset, persistence, and updated prompts | Unity Rebinding UI [U-REBIND] |
| Toggle/hold alternatives | Motor; sustained holds, repeated presses, chords | Designer interaction plus visible alternative / Contextual | Store logical action state; clear on pause, focus loss, death, and disconnect | Xbox input guidance [X-INPUT] |
| Text scale and font choice | Low vision, reading preference, couch/handheld play | Legible designer font; respect supported system preference / Baseline | Separate subtitle size, HUD size, and menu text; do not let auto-fit undo enlargement | Ren'Py accessibility source [R-SOURCE] |
| High-contrast semantic palettes | Colour vision, low vision; teams, threats, puzzles, status colours | Redundant shape/text encoding always; optional palette override / Baseline assessment | Replace semantic tokens, not arbitrary scene pixels; preserve outlines and patterns | Xbox contrast/colour guidance [X-CONTRAST] |
| Reduced motion | Motion sickness, sensory; camera shake, bob, zoom, parallax, animated menus | Respect system preference where available; reduced/off variants / Contextual | Change JS/game tweens as well as CSS; don't hide important state transitions | Browser preference [W-MOTION] |
| Captions and subtitle presentation | Hearing, cognitive, situational; dialogue or informative sound | Captions readily discoverable before relevant content / Contextual | Speaker, timing, background, size, directional information when useful | Lyra subtitle settings [E-LYRA] |
| Visual sound cues | Hearing, situational; sound communicates threats/navigation | Optional, with independent density controls / Contextual | Use event category, position, lifetime, and importance; avoid permanent HUD overload | Proposed implementation; event source can share the caption pipeline |
| Reduced flashes | Photosensitivity; flashes, cuts, patterns, combat effects | Safer authored content in all modes; reduction enabled where appropriate / Contextual safety requirement | Remove hazards at their source and analyze rendered scenarios; an analyser pass is not a safety guarantee | EA IRIS [IRIS] |
| Voice commands | Motor; discrete actions and menu selection | Off, with explicit microphone activation / Advanced | Same action dispatcher as physical input; alternatives, confirmation, latency and recognition-error recovery | Vosk [VOSK] |
| One/two-switch scanning | Motor; serial selection can replace direct pointing | Off; adjustable speed and manual stepping / Contextual or Advanced | Stable focus sequence, pause/resume, activation delay, and back action | Apple Switch Control [SWITCH] |

## 2. Godot 4.x and Redot

### Verified version boundary

Godot 4.5 introduced experimental screen-reader support through AccessKit. The release announcement distinguishes game Control support from incomplete editor accessibility. Do not state that Godot 4.0–4.4 already contained this integration. Desktop support is documented; mobile/web coverage must be checked for the exact engine release and export platform. [4.5 release][G-45], [integration announcement][G-DEV]

Redot's current release policy says Redot 26.1 starts from a hard fork of Godot 4.5.2. Earlier Redot releases have different upstream bases. That establishes a lineage, not proof that a particular installed Redot binary has every accessibility feature enabled. Record `REDOT_BIN --version`, inspect generated API data, and test an exported build. Do not equate a Redot year/quarter number with a Godot version. [Redot policy][REDOT-POLICY]

### Built in / build / reusable code

| Area | Built in | What the game must build | Recommended code / qualification |
| --- | --- | --- | --- |
| Screen-reader UI, Godot 4.5+ | AccessKit-backed standard Controls and accessibility metadata | Accessible labels, relationships, focus order, modal behavior, custom gameplay nodes | Start with standard Controls and the versioned class reference. [G-CONTROL] |
| Narration before/alongside 4.5 | `DisplayServer.tts_*` functions | Enable TTS project setting, choose available voice, queue/cancel, localization, status and recovery | Built-in TTS for opt-in self-voicing; NVDA adapter when reader-specific speech is required. [G-TTS], [G-NVDA] |
| Action remapping | `InputMap` actions, event addition/removal, deadzones | Capture UI, conflict policy, save/load, reset, multiple devices, prompt refresh | Use native `InputMap`, rather than hardcoded keys in gameplay. [G-INPUT] |
| Text/UI scale | Theme font sizes and `default_font_size`, `default_base_scale`; Control containers | Player preference, reflow, minimum targets, long translations, font fallback | Apply shared theme tokens; avoid only scaling a root transform. [G-THEME] |
| Keyboard/controller focus | Focus modes, directional neighbours, next/previous and `grab_focus()` | Predictable initial focus, visible focus, modal restoration, no traps | Use the navigation guide and a tiny test scene. [G-FOCUS] |
| Colour options | Shader and material mechanisms; no universal semantic palette accessibility solution established | Semantic colours, outlines, icons, patterns, optional renderer-compatible filter | `paulloz/godot-colorblindness` for developer simulation; CC0 correction shader only after validation. [G-CVD], [G-CORRECT] |
| Captions | Text/Control/audio building blocks | Caption database, timing, localization, speakers, important sound captions | `FEDE0D/godot-subtitles` as a small starting point, not a complete accessibility suite. [G-SUB] |

### Control properties to check explicitly

The Godot 4.5 Control reference exposes the following. The list is intentionally tied to 4.5; later releases may add different APIs. [G-CONTROL]

- `accessibility_name`: an accessible label for the control.
- `accessibility_description`: supplementary descriptive text.
- `accessibility_labeled_by_nodes`: nodes supplying the label relationship.
- `accessibility_described_by_nodes`: nodes supplying the description relationship.
- `accessibility_controls_nodes`: controls relationship.
- `accessibility_flow_to_nodes`: flow relationship.
- `accessibility_live`: live-region mode for dynamic updates.
- `FOCUS_ACCESSIBILITY`: a focus mode available when the screen reader is active.
- `_accessibility_get_contextual_info()`: virtual contextual information hook.
- `_get_accessibility_container_name(node)`: virtual container naming hook.

Recommended implementation checks:

1. Inspect what the standard widget already exposes before adding a duplicate spoken name.
2. Test button, checkbox, slider, text field, list, dialog, and dynamically replaced content with actual assistive technology.
3. Keep keyboard focus and accessibility focus coherent; do not assume they are interchangeable on every platform.
4. Give icon-only controls meaningful localized names.
5. Describe world objects through an intentional query/narration interface rather than exposing thousands of inaccessible render nodes.
6. Avoid parallel custom TTS when the accessibility bridge is already announcing the same event.

### TTS APIs and limitations

The versioned DisplayServer API includes `tts_get_voices()`, `tts_get_voices_for_language()`, `tts_speak()`, `tts_stop()`, `tts_pause()`, `tts_resume()`, `tts_is_speaking()`, `tts_is_paused()`, and `tts_set_utterance_callback()`. Speech requires project TTS enablement and platform voices; an empty voice list must be handled. This does not provide an OS accessibility tree. [G-TTS]

Recommended narration sample behavior:

- Enumerate voices instead of hardcoding the first English voice.
- Retain player language, speech rate, volume, voice choice, and narration categories.
- Interrupt superseded focus speech; queue important dialogue without replaying obsolete menu focus.
- Stop narration on scene exit where appropriate.
- Keep short status events deduplicated.
- Test missing language voices and speech services unavailable on a target machine.
- Document platform dependencies rather than claiming identical export support everywhere.

### Add-on evaluation

| Project | Licence | Date / stars observed | Evaluation |
| --- | --- | --- | --- |
| [Godot NVDA Integration][G-NVDA] | MIT wrapper; NVDA controller-client licence is separate | Asset Library listing 2024-02-03, v1.0.0 / repo 9 stars; latest commit UNVERIFIED | Godot 4.2-era Windows speech bridge with built-in TTS fallback. GDExtension binary/export compatibility requires verification. Does not replace AccessKit. |
| [Godot Asset Library NVDA entry][G-NVDA-ASSET] | MIT listing | 2024-02-03 / stars not applicable | Confirms target 4.2 and package provenance; inspect the bundled native components separately. |
| [paulloz/godot-colorblindness][G-CVD] | MIT | Asset Library v1.6, 2026-05-12, target 4.6 / 126 stars; latest commit UNVERIFIED | Useful editor simulation. Do not silently install its current version into Godot 4.3 or older Redot. |
| [lightsoutgames/godot-accessibility][G-OLD] | MIT, verified in [source licence][G-OLD-LIC] | Latest commit UNVERIFIED / 86 stars | Historical UI screen reader with Godot 3.2 instructions; not a verified Godot 4 solution. Study architecture, do not recommend installation into 4.x. |
| [FEDE0D/godot-subtitles][G-SUB] | MIT | Latest commit UNVERIFIED / 12 stars | Reusable subtitle implementation; inspect target engine version, timing, layout and localization before adoption. |
| [DTM correction shader][G-CORRECT] | CC0 stated in shader header | Latest commit and stars UNVERIFIED | Real code, but benefit and mathematical accuracy are not established by the word “correction.” Preserve license header and validate output. |

## 3. Unity

### The important distinctions

`com.unity.modules.accessibility` is an engine built-in module, not a generic third-party package that brings the newest APIs to any older editor. A module page exists for Unity 2023.2, while the full hierarchy/setting guidance examined here is versioned to Unity 6. Treat module presence, API introduction, and platform implementation as separate checks. Exact first patch availability of every member in 2023.2 is UNVERIFIED. [U-MODULE-23], [U-MODULE]

The Unity 6.0 accessibility hierarchy documentation describes mobile accessibility. Unity 6.3 explicitly lists Android 8/API 26+, iOS, Windows, and macOS for `AccessibilityHierarchy`. Its platform support must not be copied into Unity 6.0 instructions. The earliest desktop-support patch was not independently established here; use a verified 6000.3 release and compile/test rather than relying on an alpha announcement. [U-HIER-60], [U-HIER-63]

`AccessibilitySettings` is a separate platform contract: even the 6000.3 page limits it to Android and iOS. It exposes `fontScale`, `isBoldTextEnabled`, `isClosedCaptioningEnabled`, and change events. Desktop hierarchy availability does not imply desktop implementation of these system-preference APIs. [U-SETTINGS]

### Built in / build / reusable code

| Area | Built in | What the game must build | Recommended code / qualification |
| --- | --- | --- | --- |
| Native screen-reader integration | `AccessibilityHierarchy`, `AccessibilityNode`, `AssistiveSupport`, notification dispatcher on documented platforms | Semantic nodes, roles, actions, screen-space frames, activation, updates, lifecycle | Unity's `a11y-public-sample` / LetterSpell; Unity Companion License. [U-SAMPLE], [U-SAMPLE-LIC] |
| Platform preferences | Read-only `AccessibilitySettings` values and events on supported mobile platforms | Font/layout adaptation, captions initial preference, runtime updates and player override | Use the setting APIs without assuming the OS modifies Unity UI automatically. [U-SETTINGS] |
| Legacy or unsupported narration | No universal all-platform native bridge established | Narration/UI navigation adapter and platform-specific export validation | UAP if its self-voicing model and maintenance risk are acceptable. [U-UAP] |
| Rebinding | Input System rebinding operations, binding display strings, JSON overrides | Accessible capture flow, exclusions, composites, conflicts, reset, save/load | Official Input System `Assets/Samples/RebindingUI`; match package version. [U-REBIND], [U-INPUT] |
| Readability | TMP text size/font settings; CanvasScaler, layout components | Text scale separate from resolution scale, fallback glyphs, reflow, subtitles | Explicit size tokens and minimum sizes; prevent auto-sizing from shrinking user-enlarged text. [U-TMP], [U-SCALE] |
| Colour-vision modes | Rendering pipelines and shaders; `VisionUtility` assists palette design | Semantic UI/world colours and redundant encoding; pipeline-specific optional effects | MIT colourblind simulation projects; do not mistake simulation for assistance. [U-CVD] |
| Subtitles/dialogue | Text rendering and audio/Timeline infrastructure | Timed, localized subtitle/caption system; speaker and sound descriptions | Yarn Spinner Unity or ink narrative data, with custom presentation and audio-event captions. [YARN-U], [INK] |
| Accessibility guidance | Official learning course | Incorporate design review and user testing in the actual game | Unity's Practical Game Accessibility course. [U-GUIDE] |

### AccessibilityHierarchy integration details

The hierarchy is independent of the visual UI hierarchy and can describe custom or non-UI elements. Set `AssistiveSupport.activeHierarchy`; activate again when the reader is enabled, and notify relevant layout/screen changes. Node frames and hierarchy order affect navigation. The 6000.3 documentation only promises accessibility for the application's main window. [U-HIER-63]

Recommended sample coverage:

- A uGUI settings panel with button, toggle, slider, explanatory text, and modal.
- Accessible node lifecycle when panels are created, pooled, hidden, and destroyed.
- A custom gameplay node with a meaningful role/action rather than a raw image label.
- System-reader enable/disable handling while the application runs.
- Screen-coordinate updates after resize and text scaling.
- Narration adapter selection that prevents UAP and native hierarchy speech from talking together.
- Player testing on the actual supported screen reader/platform pair; Editor Play Mode alone is insufficient.

### UAP: useful source, material limitations

UAP's repository states testing from Unity 5.6 to 2020.1 and support for Windows, Android, iOS, Mac, and WebGL with limitations. It implements screen-reader-like interaction; its FAQ explicitly says it does not expose uGUI/NGUI as a native accessibility tree. Its documented TalkBack and iOS direct-touch interactions need real-device validation. This is not evidence of automatic Unity 6 compatibility. [U-UAP]

The core code is Apache-2.0, but example art is excluded and cannot simply be shipped under that licence. Included NVDA controller libraries have a separate LGPL licence. Do not repeat the README's simplified “cannot be modified” interpretation as a legal rule; use the actual controller-client licence. [U-UAP], [NVDA-LIC]

### Rebinding sample review requirements

The official sample supplies a concrete UI starting point and uses the Input System API. It does not remove the need for a game-specific conflict policy. [U-REBIND], [U-INPUT]

Recommended tests:

1. Rebind a keyboard action and verify the on-screen prompt changes immediately.
2. Rebind each part of a composite without requiring the original keyboard layout.
3. Cancel with a reachable independent control.
4. Disconnect during capture, reconnect, and recover.
5. Save overrides, restart, and verify restoration.
6. Restore one binding or all defaults without stranding the player.
7. Keep UI navigation available if gameplay bindings become unusable.
8. Allow supported assistive devices rather than filtering to one familiar gamepad model.

### Unity code catalogue

| Repository | Licence | Maintenance observation / stars | Decision |
| --- | --- | --- | --- |
| [Unity-Technologies/a11y-public-sample][U-SAMPLE] | Unity Companion License for Unity-dependent projects | Latest commit UNVERIFIED / 25 stars | Official API architecture reference; not engine-neutral MIT sample code. |
| [mikrima/UnityAccessibilityPlugin][U-UAP] | Apache-2.0 core; excluded examples; LGPL native dependency | Latest observed main commit 2022-02-15 [history][U-UAP-HIST] / 87 stars | Legacy fallback, not default for modern supported native platforms. |
| [Unity-Technologies/InputSystem][U-REBIND] | Unity Companion License for Unity-dependent projects, verified in [licence file][U-INPUT-LIC] | Latest observed develop commit 2026-08-12 [history][U-INPUT-HIST] / stars UNVERIFIED | Use installed package sample and preserve its licence. |
| [mariosubspace/colorblind-unity][U-CVD] | MIT | Latest commit UNVERIFIED / 9 stars | Simulation shader example; adapt to render pipeline and compare with a reference model. |
| [Chman/ColorBlindr][U-COLORBLINDR] | UNVERIFIED from fetched licence text | Archived 2021-11-07; latest commit and stars UNVERIFIED | Historical simulator, Unity 5+ in README; not a verified URP/HDRP drop-in. |
| [YarnSpinnerTool/YarnSpinner-Unity][YARN-U] | MIT in fetched repository; check version/product | Latest commit UNVERIFIED / 839 stars | Dialogue presentation foundation, not complete closed captions. |
| [inkle/ink][INK] | MIT | Latest commit UNVERIFIED / 5.0k stars | Engine-neutral narrative runtime; presentation, timing and captions remain game work. |

## 4. Unreal Engine 5

### Built in / build / reusable code

| Area | Built in | What the game must build | Recommended implementation |
| --- | --- | --- | --- |
| Colour vision | Widget Blueprint Library `SetColorVisionDeficiencyType` | Player UI, persistence, preview, semantic colours and redundant cues | Use verified installed signature; separate player correction from developer simulation. [E-CVD] |
| Native screen readers | Experimental Slate/UMG accessibility; Windows third-party readers and iOS VoiceOver in examined docs | Metadata, custom widget implementation, focus, navigation, packaging and testing | Enable `Accessibility.Enable=1`; configure accessible behavior and summaries. [E-NATIVE] |
| Self-voicing UI | Text To Speech, Screen Reader, Slate Screen Reader plugins | Plugin configuration, narrated content, interaction and backend selection | Follow blind accessibility overview; distinguish plugin narration from native reader integration. [E-BLIND] |
| Subtitles | Dialogue/SoundWave-associated subtitles and rendering infrastructure | Complete caption content, style customization, timing, speaker identification | Use dialogue pipeline for authored speech; extend for informative non-speech audio. [E-DIALOGUE] |
| Remapping | Enhanced Input and player-mappable key profiles/user settings | Accessible settings UI, context registration, conflicts, persistence, reset | UE 5.3+ `UEnhancedInputUserSettings`; do not copy obsolete early-UE5 tutorials blindly. [E-53], [E-INPUT] |
| Menu interaction | CommonUI navigation, input routing, controller data and common actions | Correct focus restoration, accessible labels, logical action prompts | Reuse CommonUI where it already fits architecture; it is not a screen-reader solution by itself. [E-COMMON] |
| Example settings | Lyra options, subtitle styling, colour-vision strength, controls/audio | Extract relevant patterns and test against the game's own architecture | Lyra is a sample under Epic terms, not a generic permissively licensed toolkit. [E-LYRA], [E-LYRA-LIC] |
| Photosensitivity analysis | No general built-in guarantee | Capture scenarios, analyze, report and fix authored effects | BSD-3-Clause EA IRIS UE plugin, with dependency/version review. [IRIS-UE] |

### Colour-vision API traps

The fetched Blueprint reference documents deficiency type, severity, correction enablement, and visualization of correction under the deficiency. It describes severity as 0–1, whereas Lyra's UI documents a 0–10 strength range. Do not pass a menu value straight through without checking the installed C++ implementation and normalizing deliberately. [E-CVD], [E-LYRA]

Recommended sample should contain:

- A player-facing “None” mode and independent correction strength.
- A preview using actual gameplay signals, not only colour swatches.
- A developer-only simulation toggle clearly labelled as such.
- Semantic team/threat/loot colour tokens that do not depend on the full-window correction.
- Renderer/HDR/UI tests to establish which outputs the correction affects.

### Screen readers and TTS are different routes

The native screen-reader guide marks support Experimental, documents Windows and iOS, lists common UMG widgets, and exposes customization for bespoke Slate widgets. Do not turn that into a promise of native screen-reader support across every UE5 export target. [E-NATIVE]

The blind accessibility overview describes engine TTS, Screen Reader, and Slate Screen Reader plugins. These offer a self-voicing route and UI integration; verify their backends on each platform. A documented historical issue showed TTS working Standalone while failing in Play in Editor. Packaged/Standalone testing is necessary, and the old issue is not proof the current version still fails. [E-BLIND], [E-TTS-ISSUE]

### Enhanced Input / CommonUI / Lyra

UE 5.3 added Enhanced Input User Settings and deprecated older player-mappable config patterns. Current settings/profile APIs support mapping and saving, but require the game to expose a coherent remapping workflow. Keep `UGameUserSettings` graphics/game preferences distinct from Enhanced Input user settings. [E-53], [E-INPUT]

CommonUI routes input through its viewport/input stack and supplies navigation actions and controller assets. Recommended tests include back/accept reachability, focus restoration after modal closure, and avoiding simultaneous gameplay activation beneath a menu. [E-COMMON], [E-COMMON-TECH]

Lyra documents subtitle appearance options, colour-blind modes/strength, audio controls and key bindings. Reuse its settings architecture selectively; do not import an entire shooter framework merely to get a slider. Verify exact class and content names in the matching Lyra release. [E-LYRA]

Unreal source access and Epic sample availability are not OSI-style open-source licensing. Keep any snippets copied from engine/sample sources subject to their actual Epic terms; do not relicense them as MIT in a cross-engine skill. [Unreal EULA][E-LYRA-LIC]

## 5. Other engines and runtimes

“No verified built-in bridge” below means none was established by this research, not proof of absence in all versions.

### GameMaker

| Built in | Must build | Recommended code |
| --- | --- | --- |
| Keyboard/gamepad input and thresholds; text/font rendering primitives | An action layer, runtime remapping menu, readable layout, caption pipeline, narration and semantic navigation | `Gannio/GMSTolkTalk`, a real Windows Tolk-based extension; inspect wrapper and dependency licences before use. [GM-INPUT], [GM-TOLK] |
| No cross-platform native accessibility tree verified | Windows speech bridge or web DOM adapter, plus reachable menus and gameplay descriptions | Do not promise that a DLL makes visual objects accessible. [GM-TOLK] |

The marketplace describes speech/braille through several reader APIs and a fallback speech backend. The GitHub source endpoint timed out; wrapper licence, latest update and stars remain UNVERIFIED. Tolk itself is LGPL-3.0 and unmaintained, so redistribution review is still required. [GM-TOLK], [TOLK]

Suggested GML samples: action-to-input lookup, secondary binding capture, hold/toggle action handling, caption event display, and a small narration adapter boundary. These are proposed files, not supplied by the extension.

### RPG Maker MZ

| Built in | Must build | Recommended code |
| --- | --- | --- |
| JavaScript plugin mechanism and game data/model | Accessible menu/dialogue/battle messages, map navigation, input alternatives, focus and narration | Examine source plugins, not only installer binaries. MV and MZ compatibility must be checked separately. [RPG-MV], [RPG-WHITE] |
| Canvas-rendered UI is not automatically semantic HTML | Mirror relevant text/state to accessible DOM or reader output; expose map interactions intentionally | `craigbrett17/rpgmaker-mv-access` demonstrates ARIA live messages, wall bumps and an interactables menu. It is MV-specific, not verified MZ-compatible. [RPG-MV] |

`dengopaiv/QuestersWhitestick` claims MV/MZ support and MPL-2.0. However, the fetched repository lists only README and LICENSE, with implementation distributed through releases. Source completeness is UNVERIFIED: this is a lead for testing, not yet recommended as auditable reusable MZ code. Latest commit and stars UNVERIFIED. [RPG-WHITE]

`rpgmaker-mv-access` has 5 visible stars; exact licence and latest commit are UNVERIFIED here. Its ARIA technique depends on the bundled Chromium/NW.js version. Read its actual source and test the runtime shipped with the game. [RPG-MV]

Proposed MZ samples should hook message windows, selectable lists, battle log/state and map queries through supported plugin mechanisms; do not ship copied proprietary engine files. Avoid overwriting another plugin's method without preserving the previous implementation. This is an integration recommendation, not a verified universal plugin API contract.

### Ren'Py

| Built in | Must build | Recommended code |
| --- | --- | --- |
| Self-voicing, keyboard traversal and OS speech; `v` toggles self-voicing; `Shift+A` opens accessibility menu | Accessible custom screens, alt text, meaningful narration of image/gameplay information | Start with shipped accessibility screens and documented self-voicing. [R-VOICE], [R-SOURCE] |
| Displayable `alt`, `extra_alt`, `group_alt` and screen-language support | Names for image buttons, concise groups, descriptions of non-text choices | Use public screen properties and `renpy.alt` rather than scraping rendered text. [R-SCREENS], [R-VOICE] |
| Accessibility preferences in supplied screen implementation | Ensure custom screen replacements retain controls and shortcuts | Engine source contains font/text and self-voicing preference UI; inspect the release bundled with the game. [R-SOURCE] |

Ren'Py relies on platform speech services; self-voicing is not evidence of identical mobile/web speech support. The examined self-voicing documentation describes desktop speech backends. Test every distribution target and voice language. [R-VOICE]

Reusable source: [renpy/renpy][R-REPO], especially `renpy/common/00accessibility.rpy`. That file has an MIT-style header; the complete distribution includes components under other licences. Latest commit and stars UNVERIFIED here. Do not apply one file's licence to the entire SDK. [R-SOURCE], [R-LIC]

### Twine

| Built in | Must build | Recommended code |
| --- | --- | --- |
| HTML output and story-format-specific semantics; SugarCube documents `ariaClick` keyboard activation support | Semantic passages, focus after passage changes, image descriptions, motion options, non-timed alternatives | Prefer native links/buttons and explicit headings; use story-format mechanisms before custom click handlers. [TW-SUGAR] |
| Browser accessibility infrastructure | Test selected story format and custom CSS/JS, including disappearing/cycling links | Twine editor accessibility statements do not certify every exported story. [TW-A11Y] |

SugarCube and Harlowe are different runtimes. The exact Harlowe release's current accessibility behavior was not verified; do not make a universal “Twine is screen-reader accessible” claim. A published Twine answer also emphasizes story-format choice and author markup. [TW-QA]

Code candidates: [SugarCube 2 repository][TW-REPO], BSD-2-Clause, 270 stars, and [Harlowe's official source repository][TW-HARLOWE], linked from the [Twine homepage][TW-HOME]. Harlowe's source host blocked automated access; its current licence and source completeness are UNVERIFIED here. Latest commits for both and Harlowe's popularity count were not established. Use original browser-native samples where vendoring terms remain unchecked.

### Phaser, PixiJS, and web canvas games

| Runtime | Built in | Must build | Recommended code |
| --- | --- | --- | --- |
| Phaser | DOM Element game objects can align actual HTML over the canvas | Accessible menu structure, real controls, focus, semantic gameplay state and announcements | Phaser DOM Element examples; enable DOM container and game parent correctly. [WEB-PHASER] |
| PixiJS | Accessibility package/system creates HTML overlays for accessible display objects | Appropriate roles, labels, actual keyboard activation, modal order and dynamic updates | Match Pixi major version: v7 `@pixi/accessibility`; current v8 documentation uses `pixi.js/accessibility`. [WEB-PIXI], [WEB-PIXI7] |
| Plain canvas/WebGL | Browser DOM, ARIA and CSS media preferences | DOM representation for meaningful UI; semantic gameplay event bridge | Small native DOM controls plus ARIA status region; no requirement for a framework. [W-LIVE] |

Pixi options include `accessible`, `accessibleChildren`, `accessibleTitle`, `accessibleHint`, `accessibleType`, `accessibleText`, and `tabIndex`. The documented defaults do not make every sprite accessible. The shadow element's real name/role must be inspected with the target browser and reader. [WEB-PIXI-OPTIONS]

Recommended web sample contract:

- Use `<button>`, `<input>`, `<label>`, `<fieldset>` and real links when they express the interaction.
- A persistent, initially present `aria-live="polite"` region receives short status changes.
- Reserve assertive announcements for genuinely urgent updates; prevent queue floods.
- Do not hide accessible content with `display:none` or `aria-hidden="true"` if it must remain available to assistive technology.
- Keep visual focus synchronized with the corresponding DOM control and restore it after dialogs.
- Avoid `role="application"` as a blanket fix for a game; keyboard/reader interaction must be tested deliberately.
- Use `matchMedia('(prefers-reduced-motion: reduce)')` and listen for changes in JS-driven animation.
- Use `prefers-contrast` as a preference signal, with an explicit in-game contrast setting.
- Include forced-colour testing where the UI uses native DOM.
- CSS preferences do not automatically alter WebGL shaders, canvas motion, or game camera movement.
- Browser speech synthesis is optional narration, not a replacement for semantics and reader navigation.

The ARIA and media-query mechanisms above are documented by MDN. Detailed behavior varies by browser/assistive-technology pair. [W-LIVE], [W-MOTION], [W-CONTRAST]

### Bevy

| Built in | Must build | Recommended code |
| --- | --- | --- |
| `bevy_a11y` primitives, `AccessibilityPlugin`, `AccessibilityNode`, AccessKit coordination with ECS/window/UI | Semantics and supported actions for custom controls/gameplay; ensure the whole UI/window bridge is active | Pin Bevy and compatible AccessKit versions; start from matching engine examples. [B-A11Y] |
| Accessibility request/update resources | Integrate third-party UI providers without assuming every ECS entity is accessible | `AccessibilityRequested` and update-management resources; verify APIs in pinned crate. [B-REQUEST] |

The current crate docs explicitly warn that `bevy_a11y` supplies coordination primitives rather than powering the whole accessible interface by itself. Bevy 0.15 removed the AccessKit re-export, and role/type names have changed across releases. Do not paste a 0.12 example into a newer Bevy project unchanged. [B-A11Y], [B-MIGRATION]

Code: [bevyengine/bevy][B-REPO], dual MIT/Apache-2.0, 49.1k visible stars; pinned `bevy_a11y` 0.17.2 docs show an AccessKit dependency of `^0.21`. The fetched release list includes 0.18.1 dated 2026-03-04, while current API docs were newer. These are version observations, not a latest-engine-release claim. Latest main commit UNVERIFIED. [B-PINNED]

### MonoGame and FNA

| Built in | Must build | Recommended code |
| --- | --- | --- |
| Rendering, fonts, input, audio and controller primitives | Accessible retained UI or semantic overlay, action mapping, focus, narration, subtitles, scaling/reflow | Windows reader output via a reviewed C# binding to Tolk/Prism or a platform adapter; explicit gameplay state descriptions. [MG-REPO], [TOLK], [PRISM] |
| No general native accessibility tree verified in the examined core documentation | Expose the actual UI through a compatible platform/native window adapter if required | AccessKit is a candidate requiring integration engineering; not a verified turnkey MonoGame/FNA package. [AK] |

MonoGame and FNA are compatible frameworks with documented differences; do not assume identical native dependencies or platform behavior. [FNA-DIFF]

Core sources: [MonoGame/MonoGame][MG-REPO], Microsoft Public License with exceptions and separately licensed dependencies, 14.5k stars; and [FNA-XNA/FNA][FNA-REPO], Microsoft Public License with named third-party exceptions, 3.1k stars. Inspect pinned component licences and native dependencies before copying. Latest commits UNVERIFIED. Do not add a large UI framework solely on an unverified accessibility claim.

### LÖVE / LOVE2D

| Built in | Must build | Recommended code |
| --- | --- | --- |
| Fonts, rendering and input primitives | Logical actions, keyboard menus, narration, caption events and layout | Cache font objects and choose size tokens; do not construct fonts every draw call. [LOVE-FONT] |
| Native screen-reader bridge not established in core APIs | Native speech/reader FFI or web DOM transport | `JRJurman/love2d-native-sr-example` demonstrates SRAL; `love2d-a11y-template` demonstrates browser live output. [LOVE-NATIVE], [LOVE-WEB] |

The web template's author describes `tts:`-prefixed output routed into browser reader announcements, with a working jam-game example linked from the post. That is a semantic output transport, not a full browser accessibility tree for game controls. [LOVE-POST]

The web template changed from ISC to CC0 on 2026-01-31 in observed history; pin the licence corresponding to the revision used. The native example showed 0 stars and no verified licence in the fetched page; do not vendor it until clarified. Its SRAL dependency is MIT but was archived in August 2026. [LOVE-HIST], [LOVE-NATIVE], [SRAL]

### Defold

| Built in | Must build | Recommended code |
| --- | --- | --- |
| Input bindings/action IDs, GUI nodes, text layout and font resources | Runtime remapping/action adapter, scalable layouts, captions and narration semantics | Use existing input/GUI structures and add only required accessibility events. [D-INPUT], [D-TEXT] |
| Native extensions infrastructure | Platform reader/TTS integration and export verification | `Brun0oO/defold-tts-druid`: MIT, 4 stars, latest commit UNVERIFIED. [D-TTS] |

The author's 2021 announcement implemented iOS and OSX and sought help for Android/Windows. That historical statement does not establish current cross-platform coverage. Check the current native source, build targets, manifest and installed Defold extension ABI. [D-POST], [D-TTS]

### Construct 3

| Built in | Must build | Recommended code |
| --- | --- | --- |
| Speech Synthesis object with support checks and voice/rate controls | Player narration preferences, queue policy, meaningful state events and fallback | Prefer built-in event-sheet actions; no third-party library is required for basic TTS. [C-TTS] |
| Text “Read aloud” option forwards important text appearance/changes to screen readers | Select important text, avoid rapidly changing score spam, implement menu navigation and labels | Use a dedicated announcement text object where appropriate. [C-TEXT] |
| Browser/DOM integration routes | Accessible HTML controls and gameplay representation for canvas content | Reuse engine-neutral web patterns; verify worker/export context before calling DOM APIs. [C-TEXT], [W-LIVE] |

Construct documents browser/platform speech support and user-activation restrictions. TTS on startup cannot be assumed. No verified open-source all-purpose Construct accessibility add-on was found; prefer documented built-ins plus minimal original event sheets. [C-TTS]

### Roblox

| Built in | Must build | Recommended code |
| --- | --- | --- |
| `GuiService.PreferredTextSize`, `PreferredTransparency`, `ReducedMotionEnabled` and change observation | Apply preferences to custom UI and motion; design sufficiently large, reflowing controls | Official accessibility guidance and service code samples. [RB-GUI], [RB-A11Y] |
| TextService size-offset helper and normal GUI navigation machinery | Responsive layout, controller focus, optional in-experience controls | Check `GetTextSizeOffsetAsync` behavior; it may yield/fail and requires careful layout integration. [RB-TEXT] |

The preference properties are signals to experience code, not automatic fixes for all custom animations or manually sized UI. Roblox documents multiplying an element's original background transparency by `PreferredTransparency`, preserving the baseline value. Repeatedly multiplying the already-modified result would compound the effect. [RB-GUI]

Roblox is a proprietary platform. Public Luau source and example availability do not make every CoreScript or Creator Store asset reusable under MIT. A verified general native screen-reader API for every custom experience UI was not established here: UNVERIFIED. Do not infer it from the presence of player accessibility preferences. [RB-A11Y]

### Minecraft modding

| Built in | Must build | Recommended code |
| --- | --- | --- |
| Java/Bedrock accessibility settings exist, but edition and release must be distinguished | Game-specific narration gaps, spatial queries, readable custom mod screens, accessible new mechanics | Use the official edition-specific accessibility page and current Minecraft Access docs. [MC-OFFICIAL], [MC-ACCESS] |
| Java community mod loaders permit rich additions | Version/loader-specific hooks, mixins/events, compatibility and reader integration | `minecraft-access/minecraft-access`: GPL-3.0; Fabric and NeoForge distribution documented; 95 visible stars. [MC-ACCESS], [MC-DOWNLOAD] |

Do not promise that a Fabric accessibility mod runs on NeoForge unchanged or supports a future Minecraft version. Match Minecraft, loader, mappings, Java runtime, dependencies and mod version. Latest repository commit was not verified. Current project documentation recommends Fabric and describes a more complex NeoForge route. [MC-DOWNLOAD], [MC-SETUP]

Minecraft Access exposes an add-on API requiring a `MinecraftAccessAddon`, an example of accessibility software itself exposing extension points. Treat the API and its package names as versioned contracts; the fetched API heading's version must not be mistaken for the game's version. [MC-API]

## 6. Cross-engine speech and assistive-technology libraries

### Reader output and accessibility infrastructure

| Project / API | Function and platforms | Licence | Date / stars / evaluation |
| --- | --- | --- | --- |
| [Tolk][TOLK] | Windows reader speech/braille abstraction with SAPI fallback | LGPL-3.0 library; bindings have separate terms | Latest observed master commit 2024-06-08 [history][TOLK-HIST]; stars UNVERIFIED; README says not currently developed. Useful legacy bridge, not default maintenance-safe choice. |
| [Prism, ethindp/prism][PRISM] | Reader and TTS abstraction with several backends and language/Godot bindings | MPL-2.0; inspect NOTICE and backend dependencies | Latest observed master commit 2026-08-27 [history][PRISM-HIST] / 58 stars. Promising current candidate; verify target backend and C++23/native build requirements. |
| [UniversalSpeech][UNIVERSAL] | Windows speech-reader abstraction with native and COM interfaces | MIT as stated in repository | Latest observed master commit 2025-04-04 [history][UNIVERSAL-HIST] / 48 stars. Review DLL architecture and avoid unnecessary COM registration when native API suffices. |
| [SRAL][SRAL] | Cross-platform speech/braille abstraction | MIT | Archived 2026-08-05 / 21 stars; latest commit UNVERIFIED. Maintainer cites architectural problems; useful reference with explicit maintenance risk. |
| [AccessKit][AK] | Semantic tree and action abstraction for custom UI toolkits; platform adapters | MIT OR Apache-2.0, plus Chromium-derived BSD notices | Latest commit UNVERIFIED / 1.5k stars; check individual adapter version/platform. Prefer existing engine integration over a second tree. |
| [AccessKit C bindings][AK-C] | Native-language integration surface | Main README says MIT OR Apache-2.0; fetched organization metadata differed, so confirm exact package licence | Organization listed update 2026-07-20 / 31 stars; not a verified last-commit timestamp. Avoid choosing licence by GitHub language badge. |
| [NVDA controller client][NVDA-API] | Sends speech/braille to running NVDA; Windows | LGPL-2.1 or later in examined licence | Latest component commit/stars not separately verified. Bitness and actual controller-client version matter; not the same licence as every NVDA source file. [NVDA-LIC] |
| [Windows SAPI][SAPI] | Native Windows speech synthesis, typically COM `ISpVoice` | Microsoft platform SDK/runtime terms; not an OSS library recommendation | Version support tied to Windows/SDK; no repository stars applicable. Does not expose an accessibility tree. |

### Platform speech synthesis

| Platform API | Implementation notes | Important limitation |
| --- | --- | --- |
| [`Windows.Media.SpeechSynthesis`][WIN-TTS] | Produces speech streams from installed voices; integrate playback and cancellation explicitly | Does not make a custom game UI visible to Narrator; WinRT packaging/API-contract availability needs verification. |
| [`AVSpeechSynthesizer`][APPLE-TTS] | Create `AVSpeechUtterance`, select voice/rate, speak and observe completion | VoiceOver navigation and application TTS are separate features; test audio session behavior and installed language voices. |
| [Android `TextToSpeech`][ANDROID-TTS] | Wait for initialization, enumerate/check language/voice, use utterance progress, stop/shutdown at correct lifecycle points | Different services/voices may have different offline availability; TTS does not create TalkBack semantics. |
| [Browser Speech Synthesis][WEB-SPEECH] | Detect support, wait for voice availability, provide user activation and opt-in narration | Browser voice list and autoplay restrictions vary; native DOM reader output remains a distinct path. |

Recommended adapter interface, as a design rather than an existing library promise:

```text
is_available()
speak(text, category, priority, interrupt_policy, locale)
stop(category_or_all)
set_voice_and_rate(player_preferences)
on_started / on_finished / on_error
```

- Keep localization outside platform-specific backends.
- Do not enable fallback TTS merely because no screen reader was detected.
- Reader detection may fail or change while the game runs.
- Allow the player to choose output behavior explicitly.
- Use one route per announcement unless a player intentionally requests multiple outputs.
- Keep speech queue size bounded and discard stale navigation messages.
- Never make speech completion a prerequisite for responsive input.

### Speech recognition / voice input

| Code / API | Licence / maintenance observation | Useful for | Constraints to document |
| --- | --- | --- | --- |
| [Vosk API][VOSK] | Apache-2.0 code; model licences checked separately; latest commit UNVERIFIED / 15.2k stars | Offline command vocabularies and streaming recognition | Language/model availability, memory, microphone permission, confidence/error recovery and player speech differences |
| [whisper.cpp][WHISPER] | MIT code; verify model weights/source terms; latest commit UNVERIFIED / 54.4k stars | Local transcription and experimental voice-command interpretation | Latency/device budget, unintended text on noise, action confirmation; do not use unrestricted transcripts as executable game commands |
| [Android SpeechRecognizer][ANDROID-ASR] | Platform API, not an OSS library | Platform-supported speech input | On-device capability is not universal; check availability and API-level methods rather than assuming offline recognition |
| [Apple Speech framework][APPLE-ASR] | Platform API | Recognition with language/availability checks | Check authorization, on-device support and network requirements for chosen recognizer |
| [Windows speech recognition][WIN-ASR] | Platform API | Constrained commands or platform recognition | API/app-model and locale support; preserve full physical-input alternatives |

Recommended voice sample behavior: explicit microphone control, action allow-list, visible recognition state, cancellation, cooldown/debounce, confirmation for destructive actions, and an accessible error message. Recognition should be an optional alternate input path, never a requirement to play.

## 7. Controllers, switches, eye tracking, and copilot input

### Device support is necessary but not sufficient

| Technology | What exists | What the game must do | Licence / version concern |
| --- | --- | --- | --- |
| [Steam Input][STEAM] | Action-based mappings, action origins/glyphs and controller configuration | Provide logical actions, query actual bindings for prompts, test custom configurations | Steamworks SDK terms; not open-source merely because sample code is visible |
| [SDL GameController / Gamepad][SDL] | Standardized controller mapping and device APIs | Respect mappings, hotplug, axes/deadzones, multiple device sources and alternatives | SDL is zlib-licensed; SDL2 `GameController` names became SDL3 `Gamepad` names. [SDL-MIG], [SDL-LIC] |
| [Xbox Adaptive Controller][XAC] | Adaptive hardware that supplies controller input | Avoid mandatory simultaneous buttons, fixed timings and unremappable analog gestures | No special “disabled player” device path should be required; test actual supported input behavior |
| [Xbox Copilot][COPILOT] | Platform facility combining control assistance | Avoid assumptions that one person or one physical device must supply every action | Official page requires JS and detail could not be fetched; precise current PC/console limitations UNVERIFIED |
| [Apple Switch Control][SWITCH] | Item/manual/auto scanning and switch actions | Expose an accessible navigable UI, or implement a separate deliberate game scan mode | Canvas pixels alone do not become accessible items; test actual exported UI |
| [Tobii Game Integration API][TOBII] | Gaze/head input, integration samples and C#/C++ interfaces | Optional gaze targeting, generous dwell controls, explicit activation, calibration/recovery and physical-input alternatives | Proprietary SDK licensing; platform/hardware and analytical-use restrictions must be checked |

### Recommended control algorithms

- Action mapping: resolve all physical devices to an action name before gameplay logic.
- Multiple sources: for digital actions, combine held states without duplicate press events; release only when all relevant sources release.
- Analog conflicts: define a clear merge policy such as strongest magnitude or last active source; do not silently sum competing sticks.
- Hold/toggle: model action intent rather than synthesizing indefinite key events.
- Dwell activation: configurable delay, progress feedback, cancel-on-leave and repeat prevention.
- One-switch scan: explicit ordered selectable set, adjustable interval, pause, back, and optional manual stepping.
- Debounce: player-adjustable where meaningful; do not discard intentional rapid input globally.
- Deadzones: expose inner deadzone and sensitivity separately where the mechanics warrant them.
- Timed interaction: remove/extend time limits rather than forcing switch users to race a scan cycle.
- Copilot: make in-game shared control an explicit option, and keep it separate from platform Copilot branding.

These are proposed engine-neutral algorithms. Their presence in a library is not established unless cited above. Motor-accessibility design should follow the game-specific action model and controller testing. [X-INPUT]

Tobii's developer terms distinguish interactive use from recording/transmitting gaze for analysis. Do not add gaze analytics to an accessibility sample or assume the downloadable SDK is MIT-licensed. Older UE4/EyeX pages are historical references, not proof of current UE5 compatibility. [TOBII], [TOBII-LIC]

## 8. Colour-vision algorithms, shaders, and semantic palettes

### Simulation versus assistance

Simulation asks approximately how a stimulus may lose distinctions under a selected deficiency model. It is useful for reviewing palettes. It does not restore missing cone information and should not be advertised as a player correction. [P-BRETTEL], [P-MACHADO]

Daltonization redistributes information into other colour dimensions. Results depend on content, deficiency type, severity and individual preference. Treat it as an optional transform with a preview, not a guaranteed accessibility mode. [DALTONIZE], [DL-DESKTOP]

Recommended order of implementation:

1. Assign colours to named semantics: enemy, ally, interaction, poison, healing, team, rarity, route and puzzle state.
2. Pair each critical distinction with shape, text, icon, outline, pattern or spatial grouping.
3. Allow per-semantic replacement and sensible presets, without forcing the player to know a medical category.
4. Review contrast on actual backgrounds and under every supported rendering mode.
5. Use developer simulation to find collisions; involve players with relevant colour-vision differences.
6. Add a global correction filter only as an additional preference if it helps this game's content.

Why per-element changes are usually better: the game knows which objects must remain distinguishable and can change those signals while retaining readable text and lighting. A global transform also changes skin, scenery, UI, contrast and authored effects; it cannot recover semantics that the image never encoded. This is an engineering inference from the simulation model and semantic-colour guidance, not a claim that one palette works for everyone. [P-BRETTEL], [X-CONTRAST]

### Papers and reusable implementations

| Algorithm | Reference | What to reuse / limitation |
| --- | --- | --- |
| Brettel, Viénot & Mollon, 1997 | *Computerized simulation of color appearance for dichromats*, JOSA A 14(10), 2647–2655; DOI `10.1364/JOSAA.14.002647` [P-BRETTEL] | Piecewise projection in LMS space; requires two projection choices and a separating plane, not one universal RGB matrix |
| Viénot, Brettel & Mollon, 1999 | *Digital video colourmaps for checking the legibility of displays by dichromats*, Color Research & Application 24(4), 243–252 [P-VIENOT] | Efficient red/green dichromacy approximation; document monitor/colour-space assumptions |
| Machado, Oliveira & Fernandes, 2009 | *A physiologically-based model for simulation of color vision deficiency*, IEEE TVCG 15(6); DOI `10.1109/TVCG.2009.113` [P-MACHADO] | Severity-dependent matrices; use reference tables and correct linear colour handling |
| Daltonization / error redistribution | Concrete Python implementation [DALTONIZE] | “Daltonize” is not one uniquely defined scientifically validated correction; specify the algorithm and parameters |
| DaltonLens C | [libDaltonLens][DL-C], Unlicense | Small portable reference with generated values and tests; Brettel/Viénot, including SVG variants |
| DaltonLens Python | [DaltonLens-Python][DL-PY], MIT | CPU reference conversions and simulations; useful image-regression oracle |

### Published/reference matrices and application convention

Below are the severity-1.0 Machado simulation matrices reproduced from the MIT-licensed reference table in DaltonLens-Python. They map a column vector of **linear RGB** to simulated linear RGB. They are not player correction matrices. Other severities should use the reference tables, not assume interpolating identity to the full matrix reproduces the physiological model. [matrix source][DL-MATRICES]

```text
Protanopia:
  0.152286   1.052583  -0.204868
  0.114503   0.786281   0.099216
 -0.003882  -0.048116   1.051998

Deuteranopia:
  0.367322   0.860646  -0.227968
  0.280085   0.672501   0.047413
 -0.011820   0.042940   0.968881

Tritanopia:
  1.255528  -0.076749  -0.178779
 -0.078411   0.930809   0.147602
  0.004733   0.691367   0.303900
```

There is no source-independent single “Brettel matrix”: coefficients depend on RGB primaries, white point, LMS fundamentals and selected projection. The following complete parameter sets come from libDaltonLens, using linear sRGB, Smith–Pokorny 1975 LMS fundamentals and RGB white as the neutral point. Select matrix A when `dot(rgb, n) >= 0`, otherwise B. These are reference implementation coefficients, not literal RGB tables independent of the papers' assumptions. [DL-C-SOURCE]

```text
Brettel protan:
A = [ 0.14980  1.19548 -0.34528
      0.10764  0.84864  0.04372
      0.00384 -0.00540  1.00156 ]
B = [ 0.14570  1.16172 -0.30742
      0.10816  0.85291  0.03892
      0.00386 -0.00524  1.00139 ]
n = [ 0.00048  0.00393 -0.00441 ]

Brettel deutan:
A = [ 0.36477  0.86381 -0.22858
      0.26294  0.64245  0.09462
     -0.02006  0.02728  0.99278 ]
B = [ 0.37298  0.88166 -0.25464
      0.25954  0.63506  0.10540
     -0.01980  0.02784  0.99196 ]
n = [-0.00281 -0.00611  0.00892 ]

Brettel tritan:
A = [ 1.01277  0.13548 -0.14826
     -0.01243  0.86812  0.14431
      0.07589  0.80500  0.11911 ]
B = [ 0.93678  0.18979 -0.12657
      0.06154  0.81526  0.12320
     -0.37562  1.12767  0.24796 ]
n = [ 0.03901 -0.02788 -0.01113 ]

Vienot protan:
    [ 0.11238  0.88762  0.00000
      0.11238  0.88762  0.00000
      0.00401 -0.00401  1.00000 ]
Vienot deutan:
    [ 0.29275  0.70725  0.00000
      0.29275  0.70725  0.00000
     -0.02234  0.02234  1.00000 ]
```

Viénot's simplified tritan approximation is explicitly discouraged by this implementation; use Brettel for that case. Both algorithms are simulations. [DL-C-SOURCE]

For LMS projection, the conceptual operation is:

```text
linear RGB -> LMS
choose the model's projection plane
replace missing-cone response according to that plane
LMS -> linear RGB -> gamut handling -> output encoding
```

A correction sample may compute a simulation error and redistribute it, but its redistribution weights are a design choice requiring evaluation. This report does not provide a universal “correction matrix”; none was verified. [DALTONIZE]

### Shader correctness checklist

- Record whether the source texture is encoded sRGB or already linear.
- Decode sRGB exactly once; engine render targets may already be linear.
- Preserve alpha independently from the colour transform.
- Verify row/column-major conventions; GLSL matrix constructors can transpose a copied table unintentionally.
- Test neutral greys, primary swatches, black/white, alpha, and reference image differences.
- Apply clamp/gamut handling deliberately; document information lost at gamut boundaries.
- Compare gamma and linear rendering configurations.
- Check HDR, tone mapping, bloom and UI compositing order.
- Do not report passing a simulation screenshot as proof that colour-only puzzles are accessible.

The colour-space and projection requirements are explained by the model implementations and derivation. The test cases are proposed verification rules. [DL-MATRICES], [DL-MATH]

### Code choices and maintenance

| Project | Licence | Date / stars | Evaluation |
| --- | --- | --- | --- |
| [DaltonLens/libDaltonLens][DL-C] | Unlicense | Organization update 2021-12-03 / 20 stars on directly fetched repository; latest commit UNVERIFIED | Best small cross-engine reference among evaluated candidates; distinguish simulation from correction |
| [DaltonLens/DaltonLens-Python][DL-PY] | MIT | Latest observed master commit 2021-12-03 [history][DL-HIST]; organization update 2021-12-17 / 43 stars | Old code can still implement a stable algorithm; regression/reference verification matters more than star count |
| [mariosubspace/colorblind-unity][U-CVD] | MIT | Date UNVERIFIED / 9 stars | Unity simulation code; render-pipeline integration remains work |
| [paulloz/godot-colorblindness][G-CVD] | MIT | Asset release 2026-05-12 / 126 stars; latest commit UNVERIFIED | Godot editor simulation; select compatible release |
| [DTM colour correction shader][G-CORRECT] | CC0 in source header | Date/stars UNVERIFIED | Compact Godot code; numerical/player benefit requires independent checks |
| [simonwittber correction gist][U-CVD-GIST] | UNVERIFIED | Date/stars UNVERIFIED | Real Post Processing Stack code, but do not vendor without licence permission or assume URP/HDRP support |

## 9. Readability fonts and the evidence

### Font choices with licence caveats

| Font | Source / licence | Appropriate claim | Packaging concern |
| --- | --- | --- | --- |
| OpenDyslexic | Current author site and repository say SIL Open Font License 1.1. [FONT-OD], [FONT-OD-REPO] | Optional player preference, not an evidence-based dyslexia treatment | Older downloads used different terms; inspect the exact font binary's accompanying licence |
| Atkinson Hyperlegible, Google Fonts distribution | OFL-1.1 verified in repository. [FONT-AH-OFL] | Designed for distinguishable glyphs and low-vision readability | Do not assume every newer Braille Institute family/download has identical terms |
| Atkinson Hyperlegible, original 2020 distribution | Separate Atkinson Hyperlegible Font License exists. [FONT-AH-OLD] | Check the package used | New Next/Mono offerings and download EULAs require their own licence review |
| Lexend | SIL OFL-1.1 in Google Fonts repository. [FONT-LEXEND], [FONT-LEXEND-OFL] | Another reading preference; research/marketing does not establish universal benefit | Repository archived 2026-04-19; font usability does not expire because source repo is archived |
| Noto families | [Google Noto fonts][FONT-NOTO], commonly OFL; confirm exact family | Script coverage/fallback candidate | Include relevant weights and glyphs without claiming all Noto packages have identical files |
| Other system sans-serif choices | Platform font availability and redistribution rights vary | Let players choose supported readable alternatives | “Installed on Windows” is not redistribution permission |

Metadata: current OpenDyslexic, Atkinson and Lexend last commits/star counts were not verified. The archived status of the fetched Atkinson Google Fonts repository and Lexend repository is visible, but archive date is not automatically the last font-update date. [FONT-AH], [FONT-LEXEND]

### What studies actually show

Wery & Diliberto studied elementary pupils with dyslexia using OpenDyslexic, Arial and Times New Roman on letter naming, word reading and nonsense-word reading. The study did not find improved speed or accuracy with OpenDyslexic. Its small single-subject design does not establish every person's preference or every game-reading scenario. [Wery & Diliberto, 2017][FONT-STUDY-OD]

Kuster and colleagues found no reading benefit from Dyslexie in children with and without dyslexia in their experiments. Dyslexie is a different font from OpenDyslexic; don't conflate the products. Preference is not evidence of improved objective performance. [Kuster et al., 2018][FONT-STUDY-DY]

The Lexend repository describes reading-fluency studies, but independent evidence establishing universal benefit for dyslexic game players was not verified here. Atkinson's design goal is distinguishable letterforms for low vision; it should not be represented as a proven dyslexia intervention. [FONT-LEXEND], [FONT-AH-SITE]

Recommended skill language: “Font choice and spacing preferences” rather than “fix dyslexia.” Offer a readable default, optional alternate fonts, size, line spacing and reduced text animation; preserve the player's choice and test comprehension/comfort with users.

Recommended samples should test:

- `I/l/1`, `O/0`, punctuation, currency, decimal separators and button glyphs.
- Font fallback for every shipped script, including non-Latin languages.
- Reflow and scrolling at maximum scale; no auto-fit shrinkage.
- Text spacing without breaking scripted labels, tables, dialogue bubbles or subtitles.
- Instant text/reduced typewriter motion and manual dialogue advance where relevant.
- Saved preferences applied before the first substantive text screen.

## 10. Photosensitivity analysis and CI

### Tools

| Tool | Licence / availability | What it analyzes | Evaluation |
| --- | --- | --- | --- |
| [EA IRIS][IRIS] | BSD-3-Clause; 235 visible stars | Luminance flashes, saturated red flashes, spatial patterns | Strong open-source starting point; C++ library and console example, CSV results. Latest observed main commit 2025-01-14 [history][IRIS-HIST]. Not a guarantee or compliance certificate. |
| [EA IRIS Unreal plugin][IRIS-UE] | BSD-3-Clause; 90 visible stars | Asynchronous runtime frame capture/analysis and incident recording in UE5 | Documented Windows support and PixelCapture dependency; latest commit date UNVERIFIED. Verify UE minor version and capture coverage. |
| [HardingFPA][HARDING] | Commercial/proprietary; not open-source | Flash/pattern analysis and reporting | Useful specialist option; do not describe it as a free GitHub dependency or proof all interactive paths are safe. |
| [Trace PEAT][PEAT] | Free tool/user guide; open-source licence was not verified | Recorded content analysis | Legacy format/workflow constraints should be checked against modern capture; free availability is not an OSS licence. |
| [FlashDetect][FLASHDETECT] | Licence/latest commit/stars UNVERIFIED | Python adaptive-threshold/contrast research implementation | Candidate for exploratory CI only after source/licence review and validation; not a substitute for a validated analyser. |
| [EPI-LENS][EPILENS] | Licence/latest commit/stars UNVERIFIED | Browser video analysis prototype | Real code lead; do not adopt its simplified metrics as a safety standard without evaluation. |

EA explicitly disclaims IRIS as a guarantee or certification of content safety/compliance. Preserve that qualification wherever the skill generates reports. The evaluated tools can disagree; a 2025 tool-conformance study is relevant when choosing and validating a pipeline. [IRIS], [PSE-STUDY]

### Proposed CI wrapper around IRIS

1. Capture scripted gameplay scenarios, menu transitions, effects showcases and cutscenes at known output parameters.
2. Include normal/default rendering and reduced-flash variants, plus relevant HDR/brightness/settings permutations.
3. Analyze the captured frames/video with a pinned IRIS build and configuration.
4. Parse the resulting CSV into a machine-readable issue artifact with scenario ID, frame/time range and diagnostic category.
5. Fail the internal review gate on newly detected problems; use human triage for tool errors and intentional baseline changes.
6. Store short incident clips for authorized review without automatically replaying dangerous footage to people.
7. Fix the authored effect/material/camera/cut; recapture the same scenario.
8. Keep a content-coverage list. Passing sampled video cannot establish safety for every camera angle or random event combination.

The console example and CSV output make a wrapper plausible, but a maintained ready-made cross-engine GitHub Action was not verified. Do not invent an `iris --certify` CLI or exit-code contract; inspect the example's actual command-line interface before writing automation. [IRIS]

### Recommended prevention rules

- Separate decorative flashes from essential hit/threat feedback.
- Give important events stable icon/text/audio equivalents.
- Reduce effect luminance/area/repetition at source rather than applying an opaque end-of-frame filter.
- Review rapid alternating cuts and regular high-contrast patterns, not only particles.
- Treat reduced motion, reduced flashes, brightness and colour simulation as distinct settings.
- Do not use a photosensitivity warning as a substitute for removing avoidable hazards.
- Never label an unvalidated script “epilepsy safe.”

These are implementation recommendations. For exact quantitative criteria use the selected published standard/tool specification, not a guessed flashes-per-second heuristic. [IRIS], [PEAT]

## 11. Subtitles, captions, and audio-to-visual implementations

### Reusable sources

| Implementation | Reuse | Licence / date / stars | Missing work |
| --- | --- | --- | --- |
| [Godot Subtitles][G-SUB] | Timed subtitle structure and presentation | MIT; latest commit UNVERIFIED; 12 stars | Validate engine version, localization, speaker labels, layout and sound captions |
| [Yarn Spinner Unity][YARN-U] | Localized dialogue lines, presenters, line delivery and options | MIT in fetched repository; latest commit UNVERIFIED / 839 stars | Caption important non-dialogue audio; check exact Yarn product/version licence |
| [ink][INK] | Narrative line/choice data and tagging | MIT; latest commit UNVERIFIED / 5.0k stars | Timing, speech linkage, subtitle renderer, audio descriptions and settings |
| [UE dialogue/subtitles][E-DIALOGUE] | Engine-supported dialogue/subtitle association | Epic engine terms, not permissive cross-engine library | Customization, complete captions and non-dialogue event coverage |
| [Ren'Py accessibility screens][R-SOURCE] | Text preferences, speech preferences, menu layout | File-level MIT-style licence; date/stars UNVERIFIED | Custom game screens and image/gameplay descriptions |
| [SoundDirectionVisualizer][SOUND-OVERLAY] | Example of a Windows sound-direction overlay | MIT per repository README; latest commit/stars UNVERIFIED | Waveform-derived direction cannot identify game semantics as reliably as authored events |

Yarn's public materials now discuss a distinct Yarn Spinner Public License for some products. The fetched core and Unity repositories still identify MIT; do not extend that to every future Yarn add-on or paid edition. Pin the exact component and inspect its licence. [YARN-U], [YARN-CORE], [YARN-LIC]

### Proposed engine-neutral event format

```text
AccessibleAudioEvent:
  stable_event_id
  localized_caption_key
  speaker_or_source_label
  semantic_category
  world_position_or_screen_relation
  importance
  starts_at / ends_at_or_duration
  repeat_or_merge_key
```

This is a recommended schema, not an existing industry-standard middleware format.

- A single authored event can drive subtitles, sound-effect captions, a visual indicator and optional narration.
- Keep presentation categories independently toggleable.
- Use bearing/direction only if the player's context makes it meaningful.
- Clamp offscreen indicators to safe viewport bounds and offer a stationary alternative.
- Merge repeated ambient sounds to prevent text/indicator flooding.
- Distinguish “sound occurred” from actionable “threat nearby.”
- Respect timing/pause behavior for local gameplay; networked contexts need their own timing policy.
- Do not derive authoritative threat locations from stereo amplitude alone.
- Provide an adjustable background and font size, plus readable speaker/source labels.
- Include captions for important sounds that lack dialogue audio files.

The overlay repository estimates direction from played sound; the recommended game integration instead uses source events already known to the game. This is an engineering inference, not a claim that a particular game uses this schema. [SOUND-OVERLAY]

## 12. Accessibility mods: what enabled them

These examples illustrate implemented access, not a guarantee that every mode/version is fully playable. Licences apply to the mod's own code and dependencies, not the proprietary base game.

| Game / project | Implemented access described by project | Hooks / mechanism that enabled it | Maintenance / licence lesson |
| --- | --- | --- | --- |
| [Hearthstone Access][M-HEARTH] | Keyboard/screen-reader play across supported menus and game modes | Approved patch workflow, game decompilation and diff-based rebuilding; this is not a stable public accessibility API | Updates can require reapplying/rebuilding patches; never publish proprietary decompiled game code. Full reusable source licence UNVERIFIED. [M-HEARTH-TOOLS], [M-HEARTH-HOOD] |
| [Stardew Access][M-STARDEW] | Menu/dialogue narration, tile reading, nearby-object tracking and keyboard-only interaction | SMAPI-based mod integration and patches into game/menu data; releases mention SMAPI input helpers | Current fetched repo is GPL-3.0 with separately licensed speech dependencies; do not rely on an old MIT mirror. 100 stars; latest commit UNVERIFIED. [M-STARDEW-RELEASE] |
| [Say the Spire][M-SPIRE] | Screen-reader support for Slay the Spire | ModTheSpire and BaseMod are explicitly required; these enable patches/hooks into Java game interactions | Windows/Linux described; 21 stars; no licence established from fetched root, latest commit UNVERIFIED. Do not confuse with another project named Speak the Spire. [M-SPIRE-DEV] |
| [Factorio Access][M-FACTORIO] | Narrated interaction, navigation and tutorial | Lua mod integration with game state plus a launcher/speech transport | Current README warns about multiplayer limitations and unsupported Space Age content. Licence and latest commit UNVERIFIED; 35 stars. [M-FACTORIO] |
| [Minecraft Access][MC-ACCESS] | Narration and additional nonvisual gameplay information | Fabric/NeoForge mod integration and access to entity/block/UI state; versioned mod/add-on API | GPL-3.0, 95 stars; exact loader/version must match; implementation details should be inspected per target version. [MC-API] |
| Skyrim Access / Skyrim reader mods | Primary implementation/source was not verified in this research | UNVERIFIED; do not claim Papyrus/SKSE/UI hooks for a specific mod without its source | Leave as an explicit research gap instead of repeating a community list as proof |

### Design lessons for a game's mod API

Recommended hooks for base-game developers:

1. Read-only stable access to player, world, objective, inventory, menu and interaction state.
2. Semantic IDs and localized strings, not only pre-rendered textures or inaccessible text meshes.
3. Events for focus, selection, state changes, audio cues and world interactions.
4. A supported action-dispatch interface so alternate devices can invoke the same commands.
5. UI extension points and navigation control with documented ownership/lifecycle.
6. Spatial queries and interaction lists that do not require screen-coordinate OCR.
7. Settings registration, persistence and reset with no save stigma.
8. Versioned API/schema and a compatibility policy for accessibility extensions.
9. Accessibility-critical mods considered when updating anti-cheat, packaging or mod loading.
10. Safe failure when an extension is missing, and a way to reach settings/recovery without it.

These are recommendations inferred from the contrast between supported mod systems and fragile patch workflows. Hearthstone Access is especially useful evidence that community ingenuity can create access while patch maintenance remains costly. Do not present “mods can fix it” as a substitute for an accessible base game. [M-HEARTH-HOOD], [M-SPIRE-DEV], [M-STARDEW]

## 13. Reusable algorithms the skill should reference

| Algorithm / pattern | Inputs -> outputs | Why reusable | Reference / validation |
| --- | --- | --- | --- |
| Semantic action mapping | Device events -> logical action states | Alternate hardware, remapping, voice, switches and shared input use the same game commands | Unity Input System sample; SDL mapping docs. [U-INPUT], [SDL] |
| Binding capture transaction | Capture start/candidate/cancel -> accepted or rolled-back binding | Prevents unusable menus and accidental replacement | Official Rebinding UI; add game-specific conflicts and recovery. [U-REBIND] |
| Narration queue | Semantic messages + priority/category -> speech stream | Avoids stale focus speech and announcement floods | Tolk asynchronous output and platform TTS queues are building blocks. [TOLK], [APPLE-TTS] |
| Accessible tree diff | Widget state -> minimal semantic tree/state/action changes | Works across native/DOM bridges | AccessKit and Unity hierarchy notification model. [AK], [U-HIER-63] |
| User-scale layout | Text/theme metrics + container constraints -> reflowed UI | Engine-independent contract | Godot Theme and Unity CanvasScaler; manual layout verification remains necessary. [G-THEME], [U-SCALE] |
| Timed caption scheduler | Localized event records + clock -> caption blocks | Separates audio content from UI rendering | Godot Subtitles and UE dialogue pipeline. [G-SUB], [E-DIALOGUE] |
| Event-to-indicator projection | Audio source/category + camera -> stationary or directional cue | Adds hearing access without waveform inference | Proposed algorithm; compare limitations of sound overlay. [SOUND-OVERLAY] |
| CVD simulation | Linear colour + model/severity -> simulated colour | Developer palette review and regression images | Brettel/Viénot/Machado; DaltonLens CPU oracle. [DL-C], [DL-PY] |
| Semantic palette replacement | Meaning -> player-selected visual token | Preserves useful distinctions while allowing art direction | Proposed architecture grounded in redundant-colour guidance. [X-CONTRAST] |
| Flash/pattern analysis | Captured frames + thresholds -> diagnostic intervals | Engine-neutral CI review | IRIS; do not claim certification. [IRIS] |
| Dwell/scan selection | Focus candidates + elapsed time + activation -> action | Switch/gaze selection without precise mouse movement | Apple Switch Control model; proposed game implementation. [SWITCH] |
| Voice command recognizer | Audio -> allowed discrete action proposal | Alternative motor input | Vosk/whisper.cpp with confirmation and ordinary input fallback. [VOSK], [WHISPER] |

## 14. Proposed engine-folder sample inventory and version gates

The following are files the eventual skill should contain, not files created during this research. Keep implementations small and standalone; route to the selected engine only.

| Folder | Minimum sample code / notes | Required version/platform gate |
| --- | --- | --- |
| `engines/godot/` | Typed GDScript settings persistence; InputMap capture/reset/prompts; Theme scale/reflow scene; caption-event presenter; TTS queue; AccessKit Control metadata example; semantic palette resource; developer CVD shader | Godot 4.5+ for AccessKit fields; exact TTS platform/project settings; renderer/shader syntax; installed editor and exported target |
| `engines/redot/` | Compatibility notes and capability probe that routes to matching GDScript samples | Actual `REDOT_BIN --version` and generated API data; don't infer features only from upstream lineage |
| `engines/unity/` | C# native hierarchy/settings panel; Input System sample integration; TMP size/reflow; caption presenter; narration adapter; semantic material/UI colours | Editor version and built-in module; 6000.0 mobile versus 6000.3 desktop hierarchy; mobile-only settings; Input System/TMP/render-pipeline package versions |
| `engines/unreal/` | Original C++/Blueprint recipe for settings, colour option normalization, UMG metadata, TTS route choice, Enhanced Input capture/profile save, CommonUI focus restoration, captions | Installed engine/plugin headers; 5.3 user-settings migration; native reader platform; TTS packaged target; Lyra licence/version if copied |
| `engines/web/` | Native HTML options panel, live-region queue, semantic canvas bridge, media-query observers, remapping/persistence, caption scheduler, semantic CSS palette | Browser/reader pair; media preference support; user activation; no blanket Web Speech availability assumption |
| `engines/phaser/` | DOM element overlay and event bridge using web samples | Phaser major version, DOM configuration, canvas scaling and parent/container |
| `engines/pixijs/` | Accessibility system setup, semantic object/action sample and resize/focus handling | v7 package versus v8 import path; exact properties and shadow DOM behavior |
| `engines/gamemaker/` | GML action mapping/remap UI, caption events, settings persistence and speech adapter boundary | Runtime/extension ABI and target architecture; Tolk wrapper licence and Windows-only assumptions |
| `engines/rpg-maker-mz/` | Original plugin hooks for messages/selectable windows, map query/narration and persistent preferences | MZ version; plugin order; Chromium/NW.js; no untested MV plugin assumption |
| `engines/renpy/` | Short `.rpy` example retaining Shift+A/v, custom button alt/group text, descriptive narration and preference persistence | Bundled Ren'Py version, custom screens, OS voices and export platform |
| `engines/twine/` | Separate SugarCube/Harlowe recipes; focus and semantic passages; motion/contrast preferences | Story-format version and generated HTML tested with reader; no editor-to-story compliance inference |
| `engines/bevy/` | Small Rust custom-control accessibility example, action dispatcher and caption-event system | Pin Bevy/AccessKit pair and backend features; re-export/role/action changes |
| `engines/monogame-fna/` | C# action layer, narration abstraction, caption scheduler, responsive UI contract | MonoGame versus FNA dependencies; .NET/native DLL bitness; window/accessibility bridge |
| `engines/love/` | Lua action map, cached fonts, reader adapter and captions; web output transport separately | LuaJIT/native FFI and target OS; SRAL archive risk; web-template revision licence |
| `engines/defold/` | Lua input/GUI scale/captions/settings; small native-extension adapter notes | Defold extension build/version/platforms and font resources |
| `engines/construct/` | Event-sheet recipes for Read aloud, TTS availability, captions, logical actions and preferences | Construct release/export/browser; worker versus DOM context; speech activation |
| `engines/roblox/` | Luau preference listeners preserving original values; reflow and reduced-motion examples; caption events | Current service properties/client runtime; async size method; no inferred native-reader guarantee |
| `engines/minecraft/` | Guide for accessible mod screens, narration events, input alternatives and optional Minecraft Access add-on integration | Edition, game version, loader, mappings, Java, accessibility mod/add-on API; GPL boundaries |

### Sample acceptance criteria

1. A player reaches and changes accessibility preferences using supported non-pointer input.
2. A player can hear/read the current option name, value, effect, and reset action where narration is supported.
3. Changing an option updates the live game without unnecessary reload or loss of progress.
4. Preferences survive restart and apply before the relevant first-launch content.
5. Rebinding and largest text scale do not make reset/back unreachable.
6. Critical colour distinctions remain meaningful with default and selected palettes.
7. Captions/indicators cover semantic events, not merely dialogue assets.
8. Speech fallback failure leaves the game responsive and understandable through other supported paths.
9. Options do not modify achievements, content eligibility, ending, grading, or save identity.
10. Feature claims name tested engine version, export platform, hardware and assistive technology.

## 15. Source index

All links below were either fetched/searched during research or are explicitly labelled UNVERIFIED leads in the body. Primary implementation/documentation links are preferred. A source URL is not a licence grant.

[AK]: https://github.com/AccessKit/accesskit
[AK-C]: https://github.com/AccessKit/accesskit-c
[G-45]: https://godotengine.org/article/godot-4-5-making-dreams-accessible/
[G-DEV]: https://godotengine.org/article/dev-snapshot-godot-4-5-dev-3/
[G-CONTROL]: https://docs.godotengine.org/en/4.5/classes/class_control.html
[G-TTS]: https://docs.godotengine.org/en/4.4/classes/class_displayserver.html
[G-INPUT]: https://docs.godotengine.org/en/4.4/classes/class_inputmap.html
[G-THEME]: https://docs.godotengine.org/en/4.4/classes/class_theme.html
[G-FOCUS]: https://docs.godotengine.org/en/4.5/tutorials/ui/gui_navigation.html
[G-NVDA]: https://github.com/nightblade9/godot-nvda-integration
[G-NVDA-ASSET]: https://godotengine.org/asset-library/asset/2519
[G-CVD]: https://github.com/paulloz/godot-colorblindness
[G-OLD]: https://github.com/lightsoutgames/godot-accessibility
[G-OLD-LIC]: https://raw.githubusercontent.com/lightsoutgames/godot-accessibility/master/LICENSE
[G-SUB]: https://github.com/FEDE0D/godot-subtitles
[G-CORRECT]: https://github.com/dondenik/DTM/blob/main/colourblind.gdshader
[REDOT-POLICY]: https://docs.redotengine.org/en/About/release_policy
[U-MODULE-23]: https://docs.unity3d.com/2023.2/Documentation/Manual/com.unity.modules.accessibility.html
[U-MODULE]: https://docs.unity3d.com/6000.0/Documentation/Manual/com.unity.modules.accessibility.html
[U-HIER-60]: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Accessibility.AccessibilityHierarchy.html
[U-HIER-63]: https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/accessibility/accessibilityhierarchy
[U-SETTINGS]: https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/accessibility/accessibilitysettings
[U-SAMPLE]: https://github.com/Unity-Technologies/a11y-public-sample
[U-SAMPLE-LIC]: https://github.com/Unity-Technologies/a11y-public-sample/blob/main/LICENSE.md
[U-UAP]: https://github.com/mikrima/UnityAccessibilityPlugin
[U-UAP-HIST]: https://github.com/mikrima/UnityAccessibilityPlugin/commits/main/
[U-REBIND]: https://github.com/Unity-Technologies/InputSystem/tree/develop/Assets/Samples/RebindingUI
[U-INPUT]: https://docs.unity.cn/Packages/com.unity.inputsystem%401.0/manual/ActionBindings.html
[U-INPUT-HIST]: https://github.com/Unity-Technologies/InputSystem/commits/develop/
[U-INPUT-LIC]: https://github.com/Unity-Technologies/InputSystem/blob/develop/LICENSE.md
[U-SCALE]: https://docs.unity3d.com/ja/2022.3/Manual/script-CanvasScaler.html
[U-TMP]: https://docs.unity.cn/Packages/com.unity.textmeshpro%404.0/api/TMPro.TMP_Text.html
[U-CVD]: https://github.com/mariosubspace/colorblind-unity
[U-COLORBLINDR]: https://github.com/Chman/ColorBlindr
[U-CVD-GIST]: https://gist.github.com/simonwittber/acfac14329312ecbae7e28f6ba9a1c5c
[U-GUIDE]: https://learn.unity.com/course/625fdcaaedbc2a43147471e0
[YARN-U]: https://github.com/YarnSpinnerTool/YarnSpinner-Unity
[YARN-CORE]: https://github.com/YarnSpinnerTool/YarnSpinner
[YARN-LIC]: https://yarnspinner.dev/yspl
[INK]: https://github.com/inkle/ink
[E-CVD]: https://dev.epicgames.com/documentation/en-us/unreal-engine/BlueprintAPI/Widget/Accessibility/SetColorVisionDeficiencyType
[E-NATIVE]: https://dev.epicgames.com/documentation/en-us/unreal-engine/supporting-screen-readers-in-unreal-engine
[E-BLIND]: https://dev.epicgames.com/documentation/unreal-engine/blind-accessibility-features-overview-in-unreal-engine?lang=en-US
[E-TTS-ISSUE]: https://issues.unrealengine.com/issue/UE-161327
[E-DIALOGUE]: https://dev.epicgames.com/documentation/unreal-engine/using-dialogue-voices-and-waves-in-unreal-engine?application_version=5.4
[E-53]: https://dev.epicgames.com/documentation/en-us/unreal-engine/unreal-engine-5.3-release-notes?application_version=5.3
[E-INPUT]: https://dev.epicgames.com/documentation/unreal-engine/API/Plugins/EnhancedInput/UEnhancedInputUserSettings?lang=en-US
[E-COMMON]: https://dev.epicgames.com/documentation/en-us/unreal-engine/common-ui-quickstart-guide-for-unreal-engine
[E-COMMON-TECH]: https://dev.epicgames.com/documentation/en-us/unreal-engine/commonui-input-technical-guide-for-unreal-engine
[E-LYRA]: https://dev.epicgames.com/documentation/en-us/unreal-engine/tour-of-lyra-in-unreal-engine
[E-LYRA-LIC]: https://www.unrealengine.com/en-US/eula/unreal
[GM-INPUT]: https://manual.gamemaker.io/lts/en/GameMaker_Language/GML_Reference/Game_Input/GamePad_Input/gamepad_button_check.htm
[GM-TOLK]: https://marketplace.gamemaker.io/assets/11486/gmstolktalk-blind-accessibility-features
[RPG-MV]: https://github.com/craigbrett17/rpgmaker-mv-access
[RPG-WHITE]: https://github.com/dengopaiv/QuestersWhitestick
[R-VOICE]: https://www.renpy.org/doc/html/self_voicing.html
[R-SOURCE]: https://github.com/renpy/renpy/blob/master/renpy/common/00accessibility.rpy
[R-SCREENS]: https://renpy.org/doc/html/screens.html
[R-REPO]: https://github.com/renpy/renpy
[R-LIC]: https://www.renpy.org/doc/html/license.html
[TW-SUGAR]: https://www.motoslave.net/sugarcube/2/docs/
[TW-A11Y]: https://twinery.org/accessibility-and-compliance/
[TW-QA]: https://twinery.org/archive/questions/63778/are-twine-games-generally-accessible-via-screenreader.html
[TW-REPO]: https://github.com/tmedwards/sugarcube-2
[TW-HARLOWE]: https://foss.heptapod.net/games/harlowe/
[TW-HOME]: https://www.twinery.org/
[WEB-PHASER]: https://docs.phaser.io/phaser/concepts/gameobjects/dom-element
[WEB-PIXI]: https://pixijs.download/dev/docs/accessibility.html
[WEB-PIXI7]: https://pixijs.com/8.x/guides/migrations/v7
[WEB-PIXI-OPTIONS]: https://pixijs.download/v8.22.0/docs/accessibility.AccessibleOptions.html
[W-LIVE]: https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-live
[W-MOTION]: https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/At-rules/@media/prefers-reduced-motion
[W-CONTRAST]: https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/At-rules/@media/prefers-contrast
[B-A11Y]: https://docs.rs/bevy/latest/bevy/a11y/index.html
[B-REQUEST]: https://docs.rs/bevy_a11y/latest/bevy_a11y/struct.AccessibilityRequested.html
[B-MIGRATION]: https://bevy.org/learn/migration-guides/0-14-to-0-15/
[B-PINNED]: https://docs.rs/crate/bevy_a11y/0.17.2
[B-REPO]: https://github.com/bevyengine/bevy
[MG-REPO]: https://github.com/MonoGame/MonoGame
[FNA-REPO]: https://github.com/FNA-XNA/FNA
[FNA-DIFF]: https://github.com/FNA-XNA/docs/blob/main/docs/appendix/Appendix-D%3A-MonoGame.md
[LOVE-FONT]: https://love2d.org/wiki/love.graphics.setFont
[LOVE-NATIVE]: https://github.com/JRJurman/love2d-native-sr-example
[LOVE-WEB]: https://github.com/JRJurman/love2d-a11y-template
[LOVE-HIST]: https://github.com/JRJurman/love2d-a11y-template/commits/main/
[LOVE-POST]: https://love2d.org/forums/viewtopic.php?t=96374
[D-INPUT]: https://defold.com/manuals/input/
[D-TEXT]: https://defold.com/manuals/gui-text/
[D-TTS]: https://github.com/Brun0oO/defold-tts-druid
[D-POST]: https://forum.defold.com/t/offline-text-to-speech-native-extension/68335
[C-TTS]: https://www.construct.net/en/make-games/manuals/construct-3/plugin-reference/speech-synthesis
[C-TEXT]: https://www.construct.net/en/make-games/manuals/construct-3/plugin-reference/text
[RB-GUI]: https://create.roblox.com/docs/reference/engine/classes/GuiService
[RB-TEXT]: https://create.roblox.com/docs/reference/engine/classes/TextService
[RB-A11Y]: https://create.roblox.com/docs/production/publishing/accessibility
[MC-OFFICIAL]: https://www.minecraft.net/en-us/accessibility
[MC-ACCESS]: https://github.com/minecraft-access/minecraft-access
[MC-DOWNLOAD]: https://mcaccess.org/downloads/
[MC-SETUP]: https://docs.mcaccess.org/setup/basic/
[MC-API]: https://docs.mcaccess.org/api/client/javadoc/index.html
[TOLK]: https://github.com/dkager/tolk
[TOLK-HIST]: https://github.com/dkager/tolk/commits/master/
[PRISM]: https://github.com/ethindp/prism
[PRISM-HIST]: https://github.com/ethindp/prism/commits/master/
[UNIVERSAL]: https://github.com/qtnc/UniversalSpeech
[UNIVERSAL-HIST]: https://github.com/qtnc/UniversalSpeech/commits/master/
[SRAL]: https://github.com/m1maker/SRAL
[NVDA-API]: https://github.com/nvaccess/nvda/tree/master/extras/controllerClient
[NVDA-LIC]: https://github.com/nvaccess/nvda/blob/master/extras/controllerClient/license.txt
[SAPI]: https://learn.microsoft.com/en-us/previous-versions/windows/desktop/ms723627(v=vs.85)
[WIN-TTS]: https://learn.microsoft.com/en-us/uwp/api/windows.media.speechsynthesis?view=winrt-26100
[APPLE-TTS]: https://developer.apple.com/documentation/avfoundation/speech-synthesis
[ANDROID-TTS]: https://developer.android.com/reference/android/speech/tts/TextToSpeech
[WEB-SPEECH]: https://developer.mozilla.org/en-US/docs/Web/API/SpeechSynthesis
[VOSK]: https://github.com/alphacep/vosk-api
[WHISPER]: https://github.com/ggml-org/whisper.cpp
[ANDROID-ASR]: https://developer.android.com/reference/android/speech/SpeechRecognizer
[APPLE-ASR]: https://developer.apple.com/documentation/speech
[WIN-ASR]: https://learn.microsoft.com/en-us/uwp/api/windows.media.speechrecognition
[STEAM]: https://partner.steamgames.com/doc/features/steam_controller/getting_started_for_devs
[SDL]: https://wiki.libsdl.org/SDL3/CategoryGamepad
[SDL-MIG]: https://wiki.libsdl.org/SDL3/README-migration
[SDL-LIC]: https://github.com/libsdl-org/SDL/blob/main/LICENSE.txt
[XAC]: https://learn.microsoft.com/en-us/shows/one-dev-minute/what-is-the-xbox-adaptive-controller
[COPILOT]: https://support.xbox.com/en-US/help/account-profile/accessibility/copilot
[SWITCH]: https://support.apple.com/en-ie/119835
[TOBII]: https://developer.tobii.com/pc-gaming/downloads/
[TOBII-LIC]: https://developer.tobii.com/vr/sdla/
[X-INPUT]: https://learn.microsoft.com/en-us/gaming/accessibility/xbox-accessibility-guidelines/107
[X-CONTRAST]: https://learn.microsoft.com/en-us/gaming/accessibility/xbox-accessibility-guidelines/102
[P-BRETTEL]: https://doi.org/10.1364/JOSAA.14.002647
[P-VIENOT]: https://doi.org/10.1002/(SICI)1520-6378(199908)24:4%3C243::AID-COL5%3E3.0.CO;2-3
[P-MACHADO]: https://pubmed.ncbi.nlm.nih.gov/19834201/
[DALTONIZE]: https://github.com/joergdietrich/daltonize/blob/main/daltonize/daltonize.py
[DL-DESKTOP]: https://github.com/DaltonLens/DaltonLens
[DL-C]: https://github.com/DaltonLens/libDaltonLens
[DL-C-SOURCE]: https://raw.githubusercontent.com/DaltonLens/libDaltonLens/master/libDaltonLens.c
[DL-PY]: https://github.com/DaltonLens/DaltonLens-Python
[DL-HIST]: https://github.com/DaltonLens/DaltonLens-Python/commits/master/
[DL-MATRICES]: https://github.com/DaltonLens/DaltonLens-Python/blob/master/daltonlens/simulate.py
[DL-MATH]: https://daltonlens.org/understanding-cvd-simulation/
[FONT-OD]: https://opendyslexic.org/
[FONT-OD-REPO]: https://github.com/antijingoist/opendyslexic
[FONT-AH]: https://github.com/googlefonts/atkinson-hyperlegible
[FONT-AH-OFL]: https://github.com/googlefonts/atkinson-hyperlegible/blob/main/OFL.txt
[FONT-AH-OLD]: https://mirrors.ibiblio.org/CTAN/fonts/atkinson/doc/Atkinson-Hyperlegible-Font-License-2020-1104.pdf
[FONT-AH-SITE]: https://www.brailleinstitute.org/freefont/
[FONT-LEXEND]: https://github.com/googlefonts/lexend
[FONT-LEXEND-OFL]: https://github.com/googlefonts/lexend/blob/main/OFL.txt
[FONT-NOTO]: https://github.com/notofonts
[FONT-STUDY-OD]: https://pmc.ncbi.nlm.nih.gov/articles/PMC5629233/
[FONT-STUDY-DY]: https://pmc.ncbi.nlm.nih.gov/articles/PMC5934461/
[IRIS]: https://github.com/electronicarts/IRIS
[IRIS-HIST]: https://github.com/electronicarts/IRIS/commits/main/
[IRIS-UE]: https://github.com/electronicarts/IRIS-Unreal-Plugin
[HARDING]: https://www.hardingfpa.com/technical-support/how-to-interpret-hardingfpa-results/
[PEAT]: https://trace.umd.edu/photosensitive-epilepsy-analysis-tool-peat-user-guide/
[FLASHDETECT]: https://github.com/LoopGlitch26/FlashDetect
[EPILENS]: https://github.com/Pi-0r-Tau/EPI-LENS
[PSE-STUDY]: https://pmc.ncbi.nlm.nih.gov/articles/PMC12249941/
[SOUND-OVERLAY]: https://github.com/SamiKamara/SoundDirectionVisualizer
[M-HEARTH]: https://www.hearthstoneaccess.com/
[M-HEARTH-TOOLS]: https://github.com/HearthstoneAccess/DevTools
[M-HEARTH-HOOD]: https://hearthstoneaccess.github.io/under-the-hood-patch-day.html
[M-STARDEW]: https://github.com/stardew-access/stardew-access
[M-STARDEW-RELEASE]: https://github.com/stardew-access/stardew-access/releases
[M-SPIRE]: https://github.com/bradjrenshaw/say-the-spire
[M-SPIRE-DEV]: https://bradjrenshaw.github.io/say-the-spire/devs/compilation.html
[M-FACTORIO]: https://github.com/Factorio-Access/FactorioAccess

## Implications for the skill

- Detect engine, exact version, UI framework, renderer, export platforms and input stack before loading an engine folder.
- Follow the sample inventory/version gates in section 14; engine name alone is insufficient routing information.
- Keep native accessibility trees, reader-output libraries, and fallback TTS as distinct capabilities.
- Prefer built-in semantic accessibility where supported; add narration for gameplay information it cannot infer.
- Require an explicit unsupported-platform fallback and record what was actually tested.
- Generate original small examples; vendor third-party code only with a verified revision, licence and dependency notices.
- Recheck licence changes, archive status and missing metadata before dependency selection; never infer rights from public GitHub visibility.
- Route colour simulation to developer QA and semantic palette changes to player settings; optional correction is additional.
- Offer font choice honestly; do not claim special fonts treat dyslexia.
- Build captions and visual sound indicators from shared semantic audio events.
- Use IRIS or validated specialist analysis as a review aid; never generate a safety-certification claim from sampled captures.
- Keep remapping, persistence, reset, accessible prompts and hold/toggle alternatives in one verification checklist.
- Preserve optional-assist policy with no achievement/content/save penalty or mandatory badge.
- Include modding routes when available, but do not offload baseline access onto future community patches.
- Audit mode should inventory actual capabilities first, reproduce barriers, patch the narrowest layer, and test the exported game with relevant users and assistive technology.
- Every generated claim should name its tested version/platform or remain UNVERIFIED.
