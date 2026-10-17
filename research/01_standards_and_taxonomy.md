# Standards, guidelines and master taxonomy of game accessibility options

Research checked: 17 October 2026. Assignment 01. Scope: genre-neutral, engine-neutral planning and retrofit/audit reference.

## Reading this report

This is a research synthesis and proposed skill taxonomy, not a certification checklist. Recommendations below preserve the owner's rule: options are voluntary, reversible and do not withhold content, achievements, trophies or endings; do not label a save or publicly identify assist use. A settings value is visible to its player, but an “ASSIST ON” gameplay badge is not carried forward from the older local seed implementation.

**Assumptions:** “all distinct options” means an actionable catalogue found across these sources plus explicitly identified applications of their principles. It cannot mean every possible future feature. Similar controls are separate only when players need independent adjustment, such as music versus dialogue volume, or camera bob versus camera shake. Baseline includes checking universal needs; a feature with no corresponding content is recorded as not applicable, not implemented as an empty toggle.

- **Baseline (B):** plan and verify in every game; implement the relevant capability whenever its content exists.
- **Contextual (C):** implement when the listed mechanic, presentation or service exists.
- **Advanced (A):** substantial bespoke work; scope early and never defer a legal duty merely because this taxonomy calls an implementation advanced.
- **Default D:** designer's normal behaviour, with the option off until chosen.
- **Default P:** inherit an available platform preference on first use; player overrides persist.
- **Default S:** safety-oriented default, as stated in the row. This is a proposed skill default, not a claim every cited source mandates it.
- **Default On:** ordinary accessible behaviour is present without requiring discovery of a switch.
- **EXT:** a proposed application of a cited principle; that source does not prescribe this exact individual option.
- Audience abbreviations: V vision; H hearing; M motor/dexterity/pain/fatigue; C cognitive/memory/learning; N neurodivergent/sensory processing; P photosensitivity/migraine; O motion comfort; F phobia/content sensitivity; T mental health/stress; S situational access. These describe possible benefits, not diagnoses or promises of treatment.

Source IDs are clickable citations. G-number IDs are local to this report; GAG does not publish these numbers. X101–X123 are official XAG numbers. APX codes below are also local short names. A cross-reference means topical coverage; it does not assert that every detailed option is expressly required by that source.

Plan used: inventory official guidance → cross-check thresholds and platform status → deduplicate player controls → check source coverage, gaps, defaults and option interactions.

## 1. Standards and guideline crosswalk

### 1.1 Game Accessibility Guidelines (GAG): complete inventory

The [GAG full list](https://gameaccessibilityguidelines.com/full-list/) groups guidance by Motor, Cognitive, Vision, Hearing, Speech and General, with Basic, Intermediate and Advanced implementation levels. These describe implementation effort and reach, not the skill's priority tiers. The inventory includes design/process guidance because reducing everything to settings would lose essential coverage.

Some guidelines appear in several categories. The “Also listed” notes preserve those placements. G19 and G71 are separate published URLs expressing closely related digital-input guidance; they lead to one family of player options rather than double-counting controls.

| Local ID / source | Short description | Original category / level |
|---|---|---|
| [G5] | Adjust game speed | Motor / Basic |
| [G6] | Adjust or disable haptics | Motor / Basic |
| [G7] | Large separated interaction targets | Motor / Basic |
| [G8] | Control sensitivity | Motor / Basic |
| [G9] | One input method across UI and play | Motor / Basic |
| [G10] | Simpler controls | Motor / Basic |
| [G11] | Control remapping | Motor / Basic |
| [G12] | Body-independent motion tracking | Motor / Intermediate |
| [G13] | Input macros | Motor / Intermediate |
| [G14] | Resize interface | Motor / Intermediate |
| [G15] | Reposition interface | Motor / Intermediate |
| [G16] | Alternatives to sustained holds | Motor / Intermediate |
| [G17] | Windowed mode for assistive overlays | Motor / Intermediate |
| [G18] | Alternatives to rapid repeated presses | Motor / Intermediate |
| [G19] | Digital alternatives to complex inputs | Motor / Intermediate |
| [G20] | Alternatives to simultaneous inputs | Motor / Intermediate |
| [G21] | Stationary interaction targets | Motor / Intermediate |
| [G22] | Multiple input devices | Motor / Intermediate |
| [G23] | Switch and gaze control schemes | Motor / Advanced |
| [G24] | Optional post-acceptance delay | Motor / Advanced |
| [G25] | Timing alternatives, pause or skip | Motor / Advanced |
| [G26] | Portrait and landscape play | Motor / Advanced |
| [G27] | Avoid flicker and repetitive patterns | Cognitive / Basic |
| [G28] | Player-paced text | Cognitive / Basic |
| [G29] | Interactive tutorials | Cognitive / Basic |
| [G30] | Plain language | Cognitive / Basic |
| [G31] | Direct route to starting play | Cognitive / Basic |
| [G32] | Readable text formatting | Cognitive / Basic |
| [G33] | Readable default text size | Cognitive / Basic |
| [G34] | Optional keyword emphasis | Cognitive / Intermediate |
| [G35] | Configurable text colours and contrast | Cognitive / Intermediate |
| [G36] | Save thumbnails | Cognitive / Intermediate |
| [G37] | Voice alongside text chat | Cognitive / Intermediate |
| [G38] | Multimodal instructions | Cognitive / Intermediate |
| [G39] | Story progress summaries | Cognitive / Intermediate |
| [G40] | Clear narrative structure | Cognitive / Intermediate |
| [G41] | Practice without failure | Cognitive / Intermediate |
| [G42] | Control reminders | Cognitive / Intermediate |
| [G43] | Objective reminders | Cognitive / Intermediate |
| [G44] | Contextual assistance | Cognitive / Intermediate |
| [G45] | Recognisable interactive objects | Cognitive / Intermediate |
| [G46] | Distinct important event sounds | Cognitive / Intermediate |
| [G47] | Disable background motion | Cognitive / Intermediate |
| [G48] | Separate audio channels | Cognitive / Intermediate |
| [G49] | Hide decorative scene elements | Cognitive / Advanced |
| [G50] | Replay story and instructions | Cognitive / Advanced |
| [G51] | Avoid sudden surprise events | Cognitive / Advanced |
| [G52] | Disable gore | Cognitive / Advanced |
| [G53] | Recorded text narration | Cognitive / Advanced |
| [G54] | Symbol chat | Cognitive / Advanced |
| [G55] | UI contrast | Vision / Basic |
| [G56] | VR comfort | Vision / Basic |
| [G57] | Viewing-distance-aware default FOV | Vision / Basic |
| [G58] | Colour-independent information | Vision / Basic |
| [G59] | Keep temporary information near gaze | Vision / Intermediate |
| [G60] | Screen-reader-compatible documentation | Vision / Intermediate |
| [G61] | Custom cursor and reticle | Vision / Intermediate |
| [G62] | Adjust contrast | Vision / Intermediate |
| [G63] | Mobile screen reader access | Vision / Intermediate |
| [G64] | Surround audio | Vision / Intermediate |
| [G65] | Remove input/camera mismatch | Vision / Intermediate |
| [G66] | Adjustable FOV | Vision / Intermediate |
| [G67] | Audio description | Vision / Advanced |
| [G68] | Binaural audio | Vision / Advanced |
| [G69] | Distinct sounds for all events | Vision / Advanced |
| [G70] | Screen reader through installation and menus | Vision / Advanced |
| [G71] | Digital alternatives for nonvisual play | Vision / Advanced |
| [G72] | Compass orientation | Vision / Advanced |
| [G73] | Spoken navigation | Vision / Advanced |
| [G74] | Audio scanning map | Vision / Advanced |
| [G75] | Adjustable text size | Vision / Advanced |
| [G76] | Legible captions | Hearing / Basic |
| [G77] | Alternatives to essential sounds | Hearing / Basic |
| [G78] | Caption essential speech | Hearing / Basic |
| [G79] | Mono audio | Hearing / Intermediate |
| [G80] | Visualise supplementary sound information | Hearing / Intermediate |
| [G81] | Customise caption presentation | Hearing / Intermediate |
| [G82] | Visible speaker identity | Hearing / Intermediate |
| [G83] | Caption important non-speech sound | Hearing / Intermediate |
| [G84] | Captions before first audio | Hearing / Intermediate |
| [G85] | Caption secondary speech | Hearing / Intermediate |
| [G86] | Reduce background audio under speech | Hearing / Intermediate |
| [G87] | Visual multiplayer communication | Hearing / Intermediate |
| [G88] | Text alongside voice chat | Hearing / Intermediate |
| [G89] | Matchmaking without voice requirement | Hearing / Intermediate |
| [G90] | Sign language | Hearing / Advanced |
| [G91] | Age-appropriate caption reading speed | Hearing / Advanced |
| [G92] | Speech input optional | Speech / Basic |
| [G93] | Short-vocabulary speech commands | Speech / Intermediate |
| [G94] | Nonverbal sound input threshold | Speech / Advanced |
| [G95] | Request accessibility feedback | General / Basic |
| [G96] | Persist settings | General / Basic |
| [G97] | In-game feature documentation | General / Basic |
| [G98] | Public feature documentation | General / Basic |
| [G99] | Difficulty presets | General / Basic |
| [G100] | Granular gameplay variables | General / Intermediate |
| [G101] | Optional multiplayer rules preferences | General / Intermediate |
| [G102] | Autosave | General / Intermediate |
| [G103] | Manual save | General / Intermediate |
| [G104] | Gameplay assists | General / Intermediate |
| [G105] | Bypass secondary barriers | General / Intermediate |
| [G106] | Disabled participants in playtesting | General / Intermediate |
| [G107] | Change difficulty during play | General / Intermediate |
| [G108] | Live communication transcription | General / Advanced |
| [G109] | Multiple settings profiles | General / Advanced |
| [G110] | Representative disabled playtest coverage | General / Advanced |

Also listed in the live full list:

- G05 and G06: Cognitive / Intermediate as well as Motor / Basic.
- G07: Vision / Basic as well as Motor / Basic.
- G32 and G33: Vision / Basic as well as Cognitive / Basic.
- G14: Vision / Intermediate as well as Motor / Intermediate.
- G45 and G46: Vision / Intermediate as well as Cognitive / Intermediate.
- G47: Vision / Intermediate as well as Cognitive / Intermediate.
- G48: Hearing / Basic; Cognitive and Vision / Intermediate.
- G53: Vision and Cognitive / Advanced.
- G54: Hearing, Speech and Cognitive / Advanced.
- G87, G88 and G89: Speech / Intermediate as well as Hearing / Intermediate.

**Interpretation tradeoff:** G101 describes matchmaking preferences around potentially advantageous assists. Do not turn that into an assist badge, diagnosis disclosure or punitive segregation. Any multiplayer rules separation should describe rules and apply to everyone choosing them. This is an explicit reconciliation with the owner's stronger no-shaming rule, not a change to what the GAG page says.

### 1.2 Xbox Accessibility Guidelines: every published number

The [XAG landing page](https://learn.microsoft.com/en-us/xbox/accessibility/guidelines) identifies version 3.2, dated 8 June 2023, and lists 101–123. Individual pages have newer maintenance dates. XAGs are best practices, not a legal compliance certificate. The following paraphrases capture each guideline's implementation scope; follow the linked full guideline for its tests and exceptions.

| Official number / source | What implementation should provide | Taxonomy routes |
|---|---|---|
| [X101] | Legible default text, scaling, readable font choices and configurable text presentation. | V; captions; chat; narration UI |
| [X102] | Adequate foreground/background contrast, high-contrast presentation and alternatives to colour-only meaning. | V; HUD; caption backgrounds |
| [X103] | Critical information through another sensory channel; redundant colour coding. | V; H; haptics; audio cues |
| [X104] | Speech subtitles, meaningful sound captions, speaker/direction identification and configurable readable presentation; signing and transcripts. | H; V; communication |
| [X105] | Independent audio channels, spatial and mono output, pausable audio and assistive-audio ducking. | H; V; sensory load |
| [X106] | Narrate core UI and state changes; expose names, roles, values and alternatives; controllable speech and logical focus. | Screen reader/TTS |
| [X107] | Remap all controls; avoid demanding input patterns; support digital alternatives, adjustable sensitivity and accessible mobile controls. | M; hardware; mobile |
| [X108] | Descriptive difficulty choices, granular assists, changeable difficulty, saves and pause; a very low challenge route. | Difficulty; T; M |
| [X109] | Review objectives and progress; clear subtasks, journals, markers and reminders; instruction and tutorial support. | C; navigation |
| [X110] | Disable or adjust haptics; avoid relying on vibration as the sole information channel. | H; M; N |
| [X111] | Localised cinematic audio descriptions; extended descriptions when needed; accessible transcripts as an additional route. | V; C |
| [X112] | Accessible first-launch configuration, consistent navigation and operation with supported alternative input methods. | UI; M; screen reader |
| [X113] | Clear, continuously visible focus; keep focus inside active modal interactions and away from invisible elements. | UI; V; C |
| [X114] | Explain UI context and control purposes; announce unexpected transitions and external destinations; preserve assistive-technology parity. | UI; C; screen reader |
| [X115] | Explain and identify errors; provide review, confirmation, correction or undo for destructive actions. | C; M; saves; purchases |
| [X116] | Avoid unnecessary UI deadlines; let players extend/disable limits and control temporary information duration. | C; M; chat |
| [X117] | Control moving/blinking UI and autoplay; disable secondary camera effects and adjust FOV/camera movement. | N; O |
| [X118] | Prevent harmful luminance/red flashing and spatial patterns; analyse and mitigate hazards. | P |
| [X119] | Incoming speech transcription, outgoing typed speech, incoming text narration and accessible non-text communication. | Communication; TTS |
| [X120] | Accessible entire communication journey: entry, setup, notifications, settings, lobbies and relevant support paths. | Communication/multiplayer |
| [X121] | Discoverable, localised, accessible game-specific feature documentation, including in-game help where present. | Documentation; onboarding |
| [X122] | Accessible support at no extra cost through multiple communication methods. | Support |
| [X123] | Specific content information/warnings, content customisation and skips; mental-health resources and respectful representation. | F; T; content |

### 1.3 AbleGamers Accessible Player Experiences (APX)

APX is a design vocabulary for finding barriers, not a finish-line checklist. It separates access to the action/feedback loop from the ability to overcome its challenges. **All 22 patterns are verified: 12 Access and 10 Challenge.** The public navigation omits “Do More With Less,” but that live pattern is linked from related-pattern sections. Sources: [APX overview](https://accessible.games/accessible-player-experiences/), [Do More With Less][APX-DML], [pattern-use guidance](https://accessible.games/accessible-player-experiences/design-patterns/).

The short codes below are local indexing aids. Each links to the relevant pattern.

| Code / pattern | Design problem translated into player choices |
|---|---|
| [APX-FC: Flexible Controllers][APX-FC] | Use alternative physical or software input devices. |
| [APX-SC: Same Controls But Different][APX-SC] | Reassign controls to a usable arrangement. |
| [APX-IP: Improved Precision][APX-IP] | Adjust targeting and movement precision. |
| [APX-DML: Do More With Less][APX-DML] | Reduce the number of actions needed in a sequence. |
| [APX-FT: Flexible Text Entry][APX-FT] | Choose workable ways to enter text. |
| [APX-2C: Second Channel][APX-2C] | Receive information through another sensory modality. |
| [APX-CC: Clear Channels][APX-CC] | Separate competing signals and reduce masking. |
| [APX-DT: Distinguish This From That][APX-DT] | Tell important elements and states apart. |
| [APX-FD: Flexible Displays][APX-FD] | Adapt presentation to different display needs. |
| [APX-CT: Clear Text][APX-CT] | Configure readable text. |
| [APX-PI: Personal Interface][APX-PI] | Adapt interface layout and presentation. |
| [APX-LT: Leave It There][APX-LT] | Retain presentation/control settings between sessions. |
| [APX-MO: Moderation In All Things][APX-MO] | Avoid strong emotional content and moderate emotional challenge. |
| [APX-UR: Undo Redo][APX-UR] | Recover from input or decision mistakes. |
| [APX-PA: Play Alongside][APX-PA] | Share play with another person. |
| [APX-HR: House Rules][APX-HR] | Choose skill/preferences matching for play with others. |
| [APX-HH: Helping Hand][APX-HH] | Let the game assist an action. |
| [APX-SD: Slow It Down][APX-SD] | Reduce the pace or quantity of demands. |
| [APX-BY: Bypass][APX-BY] | Move past a blocking challenge. |
| [APX-TG: Training Ground][APX-TG] | Practise in a supportive environment. |
| [APX-TR: Total Recall][APX-TR] | Review information instead of relying on memory. |
| [APX-SV: Save Early, Save Often][APX-SV] | Preserve progress and support returning to play. |

### 1.4 Storefront metadata and platform guidance

**Metadata reports a capability; it does not create one or prove complete accessibility.** Publish tested coverage by platform, language, game mode and version. “Narrated menus” must never be presented as “playable without vision.” Different storefronts use different names and criteria.

#### Accessible Games Initiative (AGI) and Xbox

The [official AGI criteria][AGI] contain 24 unique tags. The communication tag appears under two headings but counts once. Local AGI IDs below are not official numeric identifiers.

| Local IDs | Published tags |
|---|---|
| AGI-01–05 | Multiple Volume Controls; Mono Sound; Stereo Sound; Surround Sound; Narrated Menus |
| AGI-06–07 | Difficulty Levels; Save Anytime |
| AGI-08–10 | Basic Input Remapping; Full Input Remapping; Stick Inversion |
| AGI-11–12 | Playable without Button Holds; Playable without Rapid Button Presses |
| AGI-13–16 | Playable with Keyboard Only; Playable with Mouse Only; Playable with Buttons Only; Playable with Touch Only |
| AGI-17–18 | Playable without Motion Controls; Playable without Touch Controls |
| AGI-19 | Chat Speech-to-Text & Text-to-Speech |
| AGI-20–24 | Clear Text; Large Text; Large & Clear Subtitles; Color Alternatives; Camera Comfort |

[Xbox's current documentation][XTAGS] adopts AGI equivalents and retains six additional tags: **Additional accessibility information, Accessibility on launch, On-demand tutorials, Pausable, Adjustable input sensitivity, Single stick gameplay**. The page identifies version 2.0.1, 14 March 2025. Do not use the older 2021 “20 tags” launch announcement as the current schema. Xbox announced AGI availability across its digital experiences in [July 2025](https://news.xbox.com/en-us/2025/07/09/accessible-games-initiative-tags-xbox-interview/).

Important differences: full remapping includes analog actions; presets alone are insufficient. Save Anytime means returning to the saved situation, not merely a nearby checkpoint. See the tag's precise scope before claiming it. The numeric table below preserves differences between XAG text recommendations and AGI text criteria.

#### Steam: shipped and verified

[Steamworks' live documentation][STEAM] describes a developer wizard, store-page display and search filters. This is shipped functionality, not merely a proposal or ordinary user-assigned genre tags. The verified feature list contains 19 entries:

| Group | Steam's published feature names |
|---|---|
| Gameplay | Adjustable Difficulty; Save Anytime |
| Audio | Custom Volume Controls; Narrated Game Menus; Stereo Sound; Surround Sound |
| Visual | Adjustable Text Size; Subtitle Options; Color Alternatives; Contrast Controls; Camera Comfort; Playable without Vision |
| Input | Keyboard Only Option; Mouse Only Option; Touch Only Option; Playable without Quick Time Events; Playable at Your Own Pace; Chat Text-to-speech; Chat Speech-to-text |

Steam's list is not identical to AGI: for example, it has separate chat direction fields and a whole-game nonvisual claim. **Do not invent Steam fields** for mono audio, phobia modes, remapping or flashing reductions; describe these in feature documentation if a dedicated field is absent.

#### Sony / PlayStation

Sony's [official announcement][PS-TAGS] verifies over 50 tags in six groups: Visual, Audio, Subtitles/Captions, Controls, Gameplay and Online Communication. Published examples include clear/large text, colour alternatives, directional cues, mono audio, screen reader, caption sizing, remapping, stick sensitivity, alternatives to holds/mashing/motion, puzzle skips, QTE simplification, speed controls, transcription and pings. PS4 and PS5 versions can expose different tags in the PS5 store.

A complete current public Sony tag schema with acceptance criteria was **UNVERIFIED**. Do not fabricate its missing names or assume Xbox numeric criteria apply. [SpecialEffect's observed store guide](https://gameaccess.info/ps5-accessibility-tags-playstation-store/) additionally records simultaneous-input alternatives, stick inversion, no vibration/adaptive-trigger effect, practice, reminders, pausing and manual saving. This is an observed subset, not an official exhaustive specification.

Sony separately documents [system accessibility](https://www.playstation.com/en-us/accessibility/): screen-reader and controller adjustments, assistant-controller support and chat transcription. System functions and in-game functions require separate verification.

#### Nintendo

The current [Switch 2 accessibility guide][NINTENDO] verifies system TTS, GameChat speech/text conversion, HOME/System Settings text and contrast changes, zoom, display colour changes, controller remapping and mono audio. TTS voice, speed and volume are adjustable. Speech/text language coverage varies; check the supported language table rather than assuming parity.

These are **system-scoped capabilities**, not proof arbitrary games expose semantic UI to TTS. The [GameChat guide](https://en-americas-support.nintendo.com/app/answers/detail/a_id/68390) distinguishes Switch 2 GameChat from original Switch chat. A Nintendo-wide, public exhaustive game accessibility guideline/tag set was **UNVERIFIED**.

#### Apple and Google mobile

Apple's [HIG][APPLE-HIG] and [game UI guidance](https://developer.apple.com/documentation/Metal/adapting-your-game-interface-for-smaller-screens) apply to games: adapt text/layout, support platform assistive technologies and multiple inputs, and test on actual supported displays. Native platform guidance is useful even when an engine draws a custom interface; the engine must still supply the semantics.

Apple's [Accessibility Nutrition Labels][APPLE-LABELS] list nine capabilities: VoiceOver, Voice Control, Larger Text, Dark Interface, Differentiate Without Color Alone, Sufficient Contrast, Reduced Motion, Captions and Audio Descriptions. Labels describe common-task support for each platform; ad experiences can affect the claim. Larger Text means **200% or more**. The live page describes an initially voluntary rollout and possible future submission requirements; this report does not assert an unverified mandatory deadline.

[Android accessibility principles][ANDROID] cover TalkBack, Switch Access, meaningful labels, alternative actions for complex gestures and multimodal media. [Android mobile design guidance][ANDROID-DESIGN] supplies contrast and touch-target recommendations. A complete Google Play game-specific equivalent to all 19 Steam fields was **UNVERIFIED**.

## 2. Numeric thresholds and how to interpret them

Numbers below retain their source and scope. A rendered game pixel, CSS pixel, Android dp, Apple point, millimetre and visual angle are different units. Engine font-size fields frequently do not equal the visible glyph height. Measure final output under the intended viewing conditions.

### 2.1 Text, contrast and input

| Source / scope | Verified number | Application / qualification |
|---|---|---|
| [X101], console text | At least 26 px at 1080p; 52 px at 4K | Default visible body height, ascender to descender. |
| [X101], PC/VR text | At least 18 px at 1080p; 36 px at 4K | XAG's recommendation; not interchangeable with AGI's PC criterion. |
| [X101], mobile/streaming | 18 / 36 / 72 px at 100 / 200 / 400 DPI | Scale linearly with DPI. |
| [X101], scaling | At least 200% of minimum default text size | Preserve content and operation, including glyphs/icons. |
| [X101], long text | At most 80 characters/glyphs, or 40 CJK, excluding spaces | Applies to blocks over two lines at normal size, or configurable equivalent. |
| [X101], spacing | Line spacing 1.5; paragraph spacing at least twice line spacing; letter spacing 0.12 × font size; word spacing 0.16 × font size | Source-specific suggested minimums when configurability is absent. |
| [X102], standard informative elements | At least 4.5:1 | Text and important standard-size visual elements. |
| [X102], large elements / inactive text | At least 3:1 | XAG includes inactive-element text; WCAG exempts inactive controls. |
| [X102], high-contrast mode | At least 7:1 for UI elements | XAG high-contrast goal, not WCAG AA's general minimum. |
| [X102], definition of large console text | 52 px at 1080p; 104 px at 4K | PC/VR: 36/72 px; mobile: 36/72/144 px at 100/200/400 DPI. |
| [AGI] Clear Text | Default height 17 / 26 / 52 px at 720p / 1080p / 4K | PC and console; use current published scope/exemptions. |
| [AGI] Large Text | Available height 25 / 38 / 76 px at 720p / 1080p / 4K | Can be default or selectable. |
| [AGI] Large & Clear Subtitles | Default 32 px at 1080p; PC scaling to at least 46 px | Other resolutions scale equivalently. Source prints “32p”; context and Microsoft tests identify pixel height. |
| [WCAG-CONTRAST], SC 1.4.3 AA | 4.5:1 ordinary text; 3:1 large text | Large = at least 18 pt regular or 14 pt bold; approximately 24 / 18.67 CSS px. |
| [WCAG-CONTRAST], SC 1.4.6 AAA | 7:1 ordinary; 4.5:1 large | Distinct from AA; logos, incidental text and inactive controls have exceptions. |
| [WCAG-NONTEXT], SC 1.4.11 AA | 3:1 | Needed identifying parts of controls/states and meaningful graphics against adjacent colours; exceptions apply. |
| [WCAG-RESIZE], SC 1.4.4 AA | 200% | Text resizing without content/function loss; captions and images of text have specific exceptions. |
| [WCAG-SPACING], SC 1.4.12 AA | Line 1.5 × font size; following paragraph 2 ×; letters 0.12 ×; words 0.16 × | Content must tolerate user override to these values. This is not a rule to force every default to them. |
| [WCAG-TARGET], SC 2.5.8 AA | 24 × 24 CSS px | Or the precise 24 CSS px diameter spacing-circle test; equivalent, inline, user-agent and essential exceptions. |
| [WCAG-TARGET], link to SC 2.5.5 AAA | 44 × 44 CSS px | Enhanced target size; has exceptions. |
| [ANDROID-DESIGN] touch targets | 48 × 48 dp recommended minimum | Includes touchable area; visual icon may be smaller. |
| [X107], analog sensitivity | At least 50% below and 50% above default | Each relevant analog control independently. |
| [X107], phone touch target | 15 × 15 mm or diameter 15 mm | 59 / 118 / 236 px at 100 / 200 / 400 DPI. |
| [X107], tablet touch target | 24 × 24 mm or diameter 24 mm | 94 / 189 / 378 px at 100 / 200 / 400 DPI. |
| [AGI], communication history | At least the past 4 messages accessible | Displayed, scrollable or expandable; includes integrated text/transcription view. |
| [G24], acceptance cooldown | 0.5 s | A suggested bounce-prevention option, not a mandatory delay on all gameplay. |
| [X116], adjustable UI deadlines | Up to at least 10 × default | Alternatively disable, request longer/no limit before it starts, or use supported extension behaviour. |
| [X116], deadline extension | At least 20 s warning; simple extension action; at least 10 extensions | Applies to UI limits outside core mechanics. |
| [X105], pause audio exception | Events shorter than 3 s need no pause control | Live multiplayer is also exempt from pausing the session. |
| [X106], recurring spoken state | 7–10 s is an acceptable interval | Avoid interrupting more important narration; not a universal countdown cadence. |

**Contrast calculation:** WCAG uses `(Llighter + 0.05) / (Ldarker + 0.05)`. Relative luminance uses linearised sRGB channels, not an average of encoded RGB. Do not round 4.499 up to 4.5. Check the least-contrasting background under moving text, disabled labels, focused controls and HDR presentation. Sources: [WCAG contrast explanation][WCAG-CONTRAST], [X102].

**Source discrepancy:** the March 2025 AGI PDF and Microsoft's copied requirements text print “4:5:1”; this is not a meaningful contrast ratio. Microsoft's own [Clear Text test steps][XTAGS] use 4.5:1, as does [X102]. Record the apparent editorial defect rather than reproducing a new threshold.

### 2.2 Subtitle and caption authoring

| Source | Verified numeric guidance | Scope |
|---|---|---|
| [X104] | Avoid lines over 40 characters; normally no more than 2 lines; 3 only exceptionally | Author meaningful line breaks; reflow translated and scaled text. |
| [X104] | Size scalable to at least 200%; background opacity 0–100% | Use X101 default minimums, preferably larger for temporary captions. |
| [X104] | Repeat identity after speaker change or a significant 1–2 minute pause | A sole unidentified narrator is an explicit exception. |
| [BBC] | Recommended 160–180 WPM, approximately 0.33–0.375 s/word | Editorial target, not universal game reading capacity. Slower pacing may be needed. |
| [BBC] | Broadcast: 37 characters/line, potentially fewer with colour coding | Teletext constraint; not a universal modern subtitle limit. |
| [BBC] | Online width: 68% for 16:9; 90% for 4:3, 1:1 and 9:16 | Proportional-font line length depends on rendered width. |
| [BBC] | Normally 2 lines for landscape/square; 3 for vertical video | Do not obscure relevant picture content. |
| [BBC] authoring size | Computed line height 7–8% of active video height for 16:9/4:3/1:1; 3.9–4.5% for 9:16 | Font fits within that line height; not glyph-height or a universal game font requirement. |
| [HAMILTON-SUB] | About 38 characters/line; around 0.25 s between successive subtitles | 2015 game-focused practical recommendations, not a platform requirement. |
| [HAMILTON-SUB] | Reports Channel 4 HD 46 px and SD 26 px | Historical broadcast comparison, not a newly verified universal game minimum. |

Authoring checks beyond numbers:

- Make speech captions complete across cinematics, systemic dialogue, enemy barks, radios and tutorials. Label offscreen direction, speaker identity and meaningful delivery where needed. [G78], [G85], [G82], [G80].
- Caption meaningful non-speech events and music information, not every incidental sound at identical priority. Supply enough event/direction detail to act. [G83], [X103].
- Separate dialogue, environmental captions and presentation controls; provide a readable opaque background option, live preview and collision-free placement. [G81], [G76].
- Synchronise with audio and narrative reveal; avoid exposing a surprise early. Pausing, reviewing or manually advancing text can address game-specific reading demands. [G28], [G50], [G91].
- Caption live players through a separately evaluated transcription system; scripted captions should be human-reviewed and localised. This is an implementation recommendation connecting [X104] and [X119], not a claim that transcription is error-free.
- Sign languages are different languages, not a single universal hand-sign track. Provide appropriate localisation. [G90].

The BBC page could not be opened by the main web fetcher because of access restrictions, but its actual primary text was retrieved through the alternate web content fetch tool. The numbers above were checked against that text; no third-party mirror is relied upon.

### 2.3 Photosensitivity: preserve the exact standard and edition

**WCAG 2.2 SC 2.3.1 (Level A):** content passes if it never exceeds three flashes in any one-second period, **or** flashing remains below the general/red flash thresholds. The normative definition below must be evaluated over sliding intervals, not just integer-second bins. The sliding-interval test is an implementation consequence of “any” one-second period. Sources: [WCAG-FLASH], [WCAG22].

| WCAG 2.2 definition | Exact value |
|---|---|
| Frequency route | No more than 3 general flashes and/or no more than 3 red flashes in any 1 s. |
| Area route | Concurrent flashing area at most 0.006 steradians within any 10° visual field; equivalent to 25% of that field at typical viewing distance. |
| General flash | Opposing luminance transitions with change at least 10% of maximum relative luminance 1.0, and darker state below 0.80. |
| Red flash | Opposing transitions involving saturated red. Working definition in Note 3: one state has R/(R+G+B) ≥ 0.8 and chromaticity-state difference > 0.2 in CIE 1976 UCS. |
| Fine-pattern exception | Balanced pattern elements smaller than 0.1° per side at typical viewing distance. |
| Desktop measurement illustration | 341 × 256 px region within 1024 × 768; an estimate of 10° under the specified conventional desktop conditions. |

Source for every row: [WCAG22] glossary “general flash and red flash thresholds.” This is **not 25% of the entire screen**, and the pixel illustration is not a universal headset/TV/mobile test. A flash contains two opposing transitions. SC 2.3.2 AAA removes the small-area/low-intensity exception: do not exceed three flashes per second. [WCAG-THREE].

**Older red formula:** [X118] still documents saturation R/(R+G+B) ≥ 0.8 and a change greater than 20 in `(R−G−B) × 320`. Do not mislabel this as WCAG 2.2's updated UCS definition. X118 also describes approximate screen-area triggers near 20%; those approximations do not replace WCAG's visual-angle rule.

**ITU-R BT.1702-3, November 2023:** [in-force edition](https://www.itu.int/rec/R-REC-BT.1702/). The [recommendation][ITU] distinguishes flash rules from informative pattern-measurement guidance.

| Criterion / scope | Threshold or qualification |
|---|---|
| Darker luminance <160 cd/m², SDR/HDR | Flash difference ≥20 cd/m². |
| Darker luminance ≥160 cd/m², HDR only | Michelson contrast >1/17; (Lhigh−Llow)/(Lhigh+Llow). |
| Red | Saturated-red transitions can be harmful regardless of luminance. |
| Prohibited flash sequence | Concurrent area >25% of displayed screen AND >3 flashes in any second. |
| Accepted leading-edge separation | ≥360 ms at 50 Hz; ≥334 ms at 60 Hz. |
| Prolonged sequence caution | >5 s may remain risky despite meeting preceding flash limits. |
| Attachment 1 pattern identification | >5 light/dark stripe pairs, plus area >40% if stationary, or >25% if changing direction, oscillating, flashing or reversing contrast. |
| Pattern qualification | Same luminance-difference test as flashes; smoothly flowing one-direction patterns are exempt in this guidance. |
| Annex 2 peak-white measurement assumptions | SDR 200 cd/m²; HLG 1,000 cd/m²; PQ 10,000 cd/m². These are analysis assumptions, not player comfort targets. |

**Harding test:** HardingFPA is a commercial analysis tool family; [the vendor supplies a games product](https://www.hardingfpa.com/hardingfpa-for-games/). “Passes Harding” needs a named tool/version, analysis profile, capture conditions and tested footage. It is not a synonym for passing one WCAG frequency check, or proof all possible gameplay is safe.

**EA IRIS:** [EA's open-source BSD-3-Clause repository][IRIS] provides a cross-platform analyser for luminance flashes, saturated-red flashes and spatial patterns. Its documented rules flag **>3 flashes/s in any second**, **2–3 flashes/s through five consecutive seconds** as extended failure, and harmful patterns persisting **≥0.5 s**. The README explicitly disclaims certification or guaranteed legal/regulatory compliance. Record library/configuration versions and use its reports as evidence, not a safety certificate.

**PEAT:** [Trace's primary user guide][PEAT] describes a free analysis resource but explicitly says it was **not designed for commercial video/game analysis**. “Free download” is not proof of open-source licensing. Do not recommend PEAT as a modern, supported game compliance substitute for IRIS/Harding.

Proposed safety policy: remove avoidable hazardous flashing in every mode, including boot logos, menus, transitions, damage, scene cuts, lightning, weapons and user content. Offer further flash/pattern suppression before exposure. Analyse combinations and repeated events, not only isolated assets; include SDR/HDR, frame-rate and display variants. Do not ask a photosensitive tester to watch known hazardous footage. Warnings are additional information, not mitigation. These are conservative skill rules informed by [G27], [X118] and the analysis limitations above.

### 2.4 Colour palettes and simulation

[Okabe and Ito's primary Color Universal Design material][OKABE] puts redundant coding before palette selection. The commonly used eight-colour set is:

| Name | sRGB hex |
|---|---|
| Black | #000000 |
| Orange | #E69F00 |
| Sky blue | #56B4E9 |
| Bluish green | #009E73 |
| Yellow | #F0E442 |
| Blue | #0072B2 |
| Vermilion | #D55E00 |
| Reddish purple | #CC79A7 |

This is a useful categorical starting palette, not a guarantee every pair has adequate text contrast or works at every size/background. Keep labels, shapes, textures and position as redundant cues. Do not call a palette universally “safe” for all vision differences.

Simulation is a development check, not a player treatment or proof of access. Evaluate protan, deutan and tritan conditions, grayscale, real backgrounds, simultaneous effects and low contrast. Offer per-semantic-colour editing and previews instead of only whole-screen filters. Related guidance: [G58], [G61], [G62], [X103]; [Steam's colour recommendations][STEAM]. Player testing remains necessary.

## 3. Legal and regulatory context for communication

This section is scope triage for the skill; it does not decide a project's legal compliance. Laws concern accessible outcomes and service journeys, not simply whether settings switches exist.

### 3.1 United States: CVAA / FCC advanced communications

CVAA duties apply to covered advanced communications services/equipment, including relevant game communications, subject to statutory achievability and applicable exceptions. [47 CFR Part 14][CFR14] defines accessibility, usability, compatibility and recordkeeping requirements. A lack of budget is not an automatic exemption. The live eCFR title was current through 15 October 2026 when retrieved.

The FCC's [2017 game-software order](https://docs.fcc.gov/public/attachments/DA-17-1243A1.pdf) extended the game-software waiver only through **31 December 2018**, without renewal. Covered software introduced before 1 January 2019 retained that waiver while sold; substantial changes and later products need their own assessment. Do not describe this as every game's entire gameplay becoming subject to a single federal accessibility standard on 1 January 2019. The FCC had already [rejected a blanket exclusion for game-related communications](https://docs.fcc.gov/public/attachments/DA-12-1645A1.pdf).

For game teams, audit the route to communication: launch, authentication, character/server selection when required, matchmaking, lobby, chat settings, sending/receiving, mute/block/report, support and documentation. [X120] provides an implementation reference for that end-to-end journey, but is not itself the law.

Practical consequences: support communication without vision, hearing, speech or demanding motor actions as applicable; give incoming voice a text route, typed communication a speech route and text a local narrated route. Do not equate fixed canned pings with full free-form communication equivalence. These are design consequences of the regulatory performance goals and [X119], not a claim that CVAA mandates one particular vendor API.

### 3.2 European Union: EAA and communications classification

[Directive (EU) 2019/882][EAA] applies from **28 June 2025** to listed products/services. It is not a blanket rule that every video-game mechanic must meet WCAG. Relevant categories can include e-commerce and electronic communications; Annex I IV(a) requires **real-time text in addition to voice**, and **total conversation when video is provided**. These are not simply batch transcripts after a conversation.

Determine scope under each relevant national implementation, including service microenterprise exemptions, transitional provisions and permitted fundamental-alteration/disproportionate-burden assessments. Article 4(5) exempts microenterprises providing services; product rules differ.

Important qualification: [the European Electronic Communications Code][EECC], recital 17 and Article 2(5), excludes a communication facility that is minor, purely ancillary and intrinsically linked to another service under a narrow test. Its recital explicitly discusses possible online-game channels. **Neither “all game chat is covered” nor “game chat is always exempt” is a supportable blanket conclusion.** Assess the facility's actual utility, use and separability and the national interpretation. This classification needs qualified legal review.

Skill consequence: inventory voice, text, video, free-form versus canned communication, real-time behaviour and account/shop flows. Route potentially covered chat to real-time text and interoperable multimodal communication review. STT captions alone do not necessarily implement real-time text, which transmits text as it is entered; adding ordinary send-button chat alone may also be insufficient.

### 3.3 United Kingdom

The [Equality Act service-provider guidance][UK-EQUALITY] describes an anticipatory reasonable-adjustment duty. This is broader than a checklist of UI options. Northern Ireland has a separate legal framework. The [public-sector websites/apps regulations guidance][UK-PUBLIC] is specifically scoped to public-sector bodies and exceptions; do not automatically apply it to every private game studio.

For in-game communications, provide an accessible route to participation and support, evaluate relevant services and foreseeable disadvantage, and document adjustments and testing. EU EAA is not automatically a general UK domestic game law; EU-market services may separately need EAA analysis.

### 3.4 Checks the skill should request from the project's legal owner

- Which countries, storefronts and consumer services are in scope?
- Who operates chat, account creation, payments and moderation: game developer, platform or another service provider?
- Is voice/text/video free-form, ephemeral, persistent or essential to participating?
- Are lobby entry and required character/server selection accessible?
- Are live transcription, typed speech and local text narration supported in each shipped language?
- Is real-time text required, and is the implementation actually RTT rather than ordinary chat?
- What documented exemptions, transition rules, records and complaint/support duties apply?
- Which claims are recommendations, store eligibility criteria, contractual platform duties or law?

## 4. Master taxonomy of player-facing options

The following tables contain **300 player-facing options and support capabilities across 12 categories**. Defaults, integration details and pitfalls are **synthesis recommendations**, not quotations or assertions that every cited standard prescribes the exact switch. Coverage identifies the nearest published guideline/pattern; **EXT** identifies a more specific proposed application. No shipped implementation is implied unless the example register in section 5 explicitly identifies it.

The audience and trigger share one column so a skill can route by game content, without asking players to disclose a diagnosis. “D” is not a fixed mandate: preserve an existing player preference and provide safety exceptions. A menu that exposes many controls should also offer optional, reversible presets and search/categories.

### 4.1 Visual presentation

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| V01 | Menu text scale | B | V,C,S; text UI | P | Reflow, including dialogs; no clipping. | [G75], [X101] |
| V02 | HUD text scale | C | V,S; gameplay readouts | P | Scale independently from menus and world. | [G14], [APX-PI] |
| V03 | Readable font alternative | B | V,C; decorative typography | On | Offer plain type with complete language glyphs. | [G32], [APX-CT] |
| V04 | Font weight adjustment | C | V; thin glyphs | D | Retain counters and spacing at bold weights. | [X101], [APX-CT] |
| V05 | Sentence-case text | C | C,V; full-cap paragraphs | On | Preserve acronyms and localisation. | [G32], [X101] |
| V06 | Letter spacing | C | C,V; dense text | D | Reflow; excessive tracking can also hinder reading. | [APX-CT], [X101] |
| V07 | Word spacing | C | C,V; dense prose | D | Preserve language-specific segmentation. | [APX-CT], [X101] |
| V08 | Line spacing | C | C,V; multi-line text | D | Keep selection/hit regions aligned. | [G32], [WCAG-SPACING] |
| V09 | Paragraph width | C | C,V; journals or codex | D | Adjustable measure, without two-axis reading scroll. | [X101], [APX-CT] |
| V10 | Text alignment | C | C,V; centred long passages | D | Support language direction; avoid forced justification. | [G32], [X101] |
| V11 | Text foreground colour | B | V,N; reading | P | Preview against actual background. | [G35] |
| V12 | Text background colour | B | V,N; reading | P | Store separately from text colour. | [G35], [G55] |
| V13 | Text panel opacity | C | V,C; scene behind UI | D | Include fully opaque option. | [G62], [APX-FD] |
| V14 | Light/dark/high-contrast UI themes | B | V,N; UI | P | Measure every state; dark alone is not high contrast. | [G55], [X102] |
| V15 | Low-contrast reading theme | C | V,N; glare-sensitive reading | D | Optional comfort choice; retain accessible high contrast. | [G35] |
| V16 | UI element scale | B | V,M; icons and widgets | P | Increase hit area as well as art. | [G14], [G7] |
| V17 | HUD element repositioning | C | V,M; restricted visual field | D | Preserve focus order and safe areas. | [G15], [APX-PI] |
| V18 | HUD margins / safe area | C | V,S; overscan or peripheral limits | D | Bring essential data inward. | [G59], [APX-PI] |
| V19 | Cursor size | C | V,M; pointer use | P | Keep hotspot obvious. | [G61] |
| V20 | Cursor colour / outline | C | V; changing backgrounds | D | Use contrasting edge; preview bright/dark scenes. | [G61] |
| V21 | Reticle size / shape / thickness | C | V; aiming | D | Avoid replacing recoil information accidentally. | [G61], [APX-DT] |
| V22 | Reticle colour | C | V; aiming | D | Separate from team/enemy palette. | [G61] |
| V23 | Protan/deutan/tritan palette presets | C | V; semantic colour distinctions | D | Presets supplement redundant cues; no cure claim. | [G58], [X103] |
| V24 | Custom colours by semantic role | C | V; teams, rarity, objectives | D | Store roles rather than one global tint. | [G58], [APX-DT] |
| V25 | Redundant symbols, labels or patterns | B | V,C; colour-coded information | On | Meaning must survive grayscale. | [G58], [APX-DT] |
| V26 | Enemy/ally outlines | C | V; entities blend into scene | D | Separately tune visibility and colour. | [G62], [APX-DT] |
| V27 | Interaction highlights | C | V,C; hard-to-find usable objects | D | Offer persistent and on-demand choices. | [G45] |
| V28 | Traversable platform/edge outlines | C | V; visual navigation hazards | D | Do not obscure landing geometry. | [G62], [APX-DT] |
| V29 | Background dimming / foreground isolation | C | V,C; visually complex scene | D | Retain essential hazards and depth cues. | [G62], [G49] |
| V30 | World-text readable overlay | C | V,C; signs or documents as art | D | Present equivalent text; EXT to avoid tiny baked lettering. | [G33], [X106] |
| V31 | Magnifier / zoom inspection | C | V; detailed visual material | D | Pair with scalable text; avoid compulsory camera motion. | [APX-FD]; EXT |
| V32 | Persistent focus appearance | B | V,C,M; navigable menus | On | Never depend only on animation or colour. | [X113] |
| V33 | Focus indicator style / thickness | C | V,N; default focus difficult | D | Allow a static, high-contrast variant. | [X113], [APX-PI]; EXT |
| V34 | Critical indicator centralisation | C | V,C; time-limited peripheral events | D | Optional duplicate near gaze; avoid blocking action. | [G59] |
| V35 | Brightness / gamma adjustment | C | V,N; dark or glaring scenes | D | EXT; calibrate in realistic scenes; does not replace contrast. | [G62], [APX-FD] |
| V36 | HDR peak/paper-white comfort controls | C | V,N,P; HDR output | D bounded output | EXT; test actual display; no universal safe brightness claim. | [APX-FD], [ITU] |

### 4.2 Audio and hearing: mix, subtitles and captions

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| H01 | Master volume / mute | B | H,N,S; audio output | S: quiet initial mix | Available before audio; retain limiter headroom. | [G48] |
| H02 | Music volume / mute | C | H,N,C; soundtrack | D | Separate from meaningful musical cues. | [G48], [APX-CC] |
| H03 | Dialogue volume / mute | C | H,C; character speech | D | Include radios and cutscenes consistently. | [G48], [X105] |
| H04 | Critical SFX volume | C | H,V; gameplay sounds | D | Separate useful signals from ambience. | [G48], [APX-CC] |
| H05 | Ambient sound volume | C | H,N; background audio | D | Do not mute essential warnings with weather. | [G48] |
| H06 | Accessibility-cue volume | C | V,H; extra audio navigation | D | Separate from general effects; EXT channel split. | [APX-CC], [AGI] |
| H07 | Narration volume | C | V,C; spoken UI | P | Never silently share an unusably low dialogue bus. | [X105] |
| H08 | Voice chat volume | C | H,N; live communication | D | Independent of dialogue and TTS. | [G48], [X105] |
| H09 | Mono mix | C | H,S; stereo/surround cues | P | Combine content into both outputs; test cancellation. | [G79] |
| H10 | Stereo / surround output choice | C | V,H; spatial cues | P | Preserve positional semantics in supported outputs. | [G64], [X105] |
| H11 | Headphone binaural/spatial mix | C | V,H; spatial world | D | Document headphones; not equivalent to visual cues. | [G68] |
| H12 | Dialogue ducking | C | H,C; masking sounds | D | Smooth attack/release; no clipped words. | [G86] |
| H13 | Screen-reader ducking | C | V,C; assistive speech | D | Reduce competing audio only while needed. | [X105] |
| H14 | Reduced dynamic-range mix | C | H,N,S; sharp loud/quiet changes | S: bounded peaks | Optional compression; preserve signal distinctions. | [APX-CC]; EXT |
| H15 | Sound-frequency / tinnitus comfort filter | A | H,N; aversive tones | D | EXT; provide alternate cues when filtering masks events. | [APX-CC], [G46] |
| H16 | Pause/replay important audio | C | H,C; narrative/audio instructions | On | Allow replay without repeating a hazard. | [G50], [X105] |
| H17 | Dialogue subtitles | B | H,C,S; any meaningful speech | P or setup before speech | Include boot and tutorial content. | [G78], [G84] |
| H18 | Secondary/bark subtitles | C | H,C; background/systemic speech | D | Distinct switch from story-only captions. | [G85] |
| H19 | Non-speech sound captions | C | H; meaningful events | D | Caption function and origin; prioritise important events. | [G83], [X104] |
| H20 | Speaker names | C | H,C; ambiguous speakers | On with captions | Do not reveal an intentionally unknown identity. | [G82] |
| H21 | Offscreen speaker direction | C | H; spatial dialogue | D | Relative direction updates consistently. | [G80], [X104] |
| H22 | Captioned sound direction / range | C | H; unseen events | D | Communicate actionable distance qualitatively or clearly. | [G80], [APX-2C] |
| H23 | Delivery/tone/source labels | C | H,C; whisper, radio, meaningful tone | D | Include cues needed for interpretation; avoid spoilers. | [G83], [G82]; EXT |
| H24 | Subtitle text size | B | V,H; subtitles | P | Independent from global UI scale. | [G81], [G76] |
| H25 | Subtitle font / weight | C | V,C,H; captions | D | Plain font available; verify translated scripts. | [G81] |
| H26 | Subtitle foreground colour | C | V,H; captions | P | Speaker colour remains supplementary to names. | [G81], [G35] |
| H27 | Subtitle background colour | C | V,H; captions | P | Include black/white readable combination. | [G81] |
| H28 | Subtitle background opacity | C | V,H; changing backdrop | D | Include 100%; preserve gameplay visibility via layout. | [G81] |
| H29 | Subtitle outline / shadow | C | V,H; text separation | D | Outline alone may fail over detailed scenes. | [G76], [G81]; EXT |
| H30 | Subtitle position / safe margins | C | H,V; UI collisions | D | Clamp within visible space; reserve maximum-scale area. | [G81], [APX-PI] |
| H31 | Caption duration / manual advancement | C | C,H,V; insufficient reading time | D | Pause compatible scenes; do not desynchronise live threats. | [G91], [G28], [X116] |
| H32 | Caption category filters | C | H,N; many simultaneous captions | D | Critical events remain available even in reduced mode. | [G81], [G83] |
| H33 | Dialogue/caption history | C | H,C; missed lines | D | Mark chronology and speakers; avoid future lines. | [G50], [APX-TR] |
| H34 | Sound visualiser / threat indicators | C | H; sound-dependent gameplay | D | Cover important events without constant visual overload. | [G77], [G80] |
| H35 | Haptic equivalents for audio events | C | H,V; event cues | D | Never make vibration the only substitute. | [APX-2C], [X110] |
| H36 | Localised sign-language interpretation | A | H; speech-heavy scripted content | D | Correct sign language; interpreter size/position controls. | [G90] |
| H37 | Separate speech and subtitle languages | C | H,C,S; localised dialogue | P | EXT; language choices persist and coverage is documented. | [G81], [X121] |

### 4.3 Motor and input

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| M01 | Full action remapping | B | M,S; player inputs | On | Cover gameplay, menus and shortcuts; avoid reserved keys. | [G11], [APX-SC] |
| M02 | Multiple bindings per action | C | M,S; mixed setups | D | EXT; retain fallback during rebinding. | [G11], [APX-SC] |
| M03 | Separate overloaded actions | C | M,C; one key means several things | D | Allow actions to become independent bindings. | [APX-SC], [AGI]; EXT |
| M04 | Device-specific binding profiles | C | M,S; keyboard/controller/touch | D | Switching device must not erase settings. | [G109], [G11] |
| M05 | Left/right control layouts | C | M; handedness constraints | D | Presets supplement full remapping. | [G10], [G11] |
| M06 | Swap stick functions | C | M; asymmetric hand function | D | Update prompts and camera bindings. | [G11], [APX-SC] |
| M07 | Independent X/Y inversion | C | M,O; analog control | D | Per stick and context; no forced paired inversion. | [G11], [X107] |
| M08 | Independent sensitivity | C | M; fine/coarse movement limits | D | Include aim, mouse, trigger and wheel as applicable. | [G8] |
| M09 | Inner/outer dead zones | C | M; drift or limited travel | D | EXT; preview usable range; prevent loss of full output. | [APX-IP], [G8] |
| M10 | Input response curve | C | M; uneven usable range | D | EXT; communicate effect with live preview. | [APX-IP], [G8] |
| M11 | Acceleration/smoothing control | C | M,O; cursor/camera mismatch | D | EXT; allow immediate predictable motion. | [G65], [APX-IP] |
| M12 | Trigger actuation threshold | C | M; limited force/travel | D | EXT; avoid accidental activation. | [G8], [APX-IP] |
| M13 | Toggle instead of sustained hold | C | M; aim, crouch, interact | D | Per action; state visibly and audibly identifiable. | [G16] |
| M14 | Hold instead of repeated taps | C | M; mashing | D | Also provide automatic/skip for players unable to hold. | [G18] |
| M15 | Automatic repeated action | C | M; rapid firing or work | D | EXT; one start/stop action rather than fatigue. | [G18], [APX-DML] |
| M16 | QTE automatic completion / skip | C | M,C,T; timed sequences | D | Preserve story and rewards. | [G18], [G25] |
| M17 | Relaxed QTE timing | C | M,C; reaction barrier | D | Separately adjust duration and repetition count. | [G25], [APX-SD] |
| M18 | Sequential alternative to chords | C | M; simultaneous presses | D | Latch modifiers; communicate current stage. | [G20] |
| M19 | Input macros / combo shortcuts | C | M,C; complex sequences | D | Bound macros to game actions; disclose multiplayer rules. | [G13], [APX-DML] |
| M20 | Single-stick gameplay | C | M; movement plus camera | D | Include stick-click and D-pad dependencies. | [G10], [XTAGS] |
| M21 | Keyboard-only gameplay and UI | C | M,V; PC release | On when supported | No mouse-only launcher, map or quit button. | [G19], [X107] |
| M22 | Mouse-only gameplay and UI | C | M; one-pointer setup | D | Supply pointer access to every required action. | [APX-FC], [STEAM] |
| M23 | Digital-only alternative | C | M,V; analog input required | D | Provide equivalents for variable actions and looking. | [G19], [G71] |
| M24 | Touch-only play | C | M,S; touch platform | On when supported | No mandatory attached keyboard/controller. | [G9], [STEAM] |
| M25 | Motion-free control | C | M,O; gyro/gesture/body input | On alternative | Preserve motion input as an optional useful method. | [G19], [G12] |
| M26 | Body-tracking calibration / seated use | C | M; tracked motion | D | Calibrate available movement; do not assume standing body. | [G12] |
| M27 | Touch target size | C | M,V; touchscreen controls | P | Enlarge hit areas without overlap. | [G7], [X107] |
| M28 | Touch target position | C | M; limited reach | D | Layout editor itself must support accessible input. | [G15] |
| M29 | Touch target spacing | C | M; imprecise touches | D | Avoid neighbours stealing activation. | [G7] |
| M30 | Tap alternative to swipe / path gesture | C | M; precision gestures | D | Equivalent buttons for all gesture functions. | [G20], [X107] |
| M31 | Single-point alternative to pinch/multitouch | C | M; multiple contacts required | D | Offer explicit zoom and rotate controls. | [G20] |
| M32 | Click-select then click-place | C | M; drag/drop | D | Cancel without committing; no held drag dependency. | [G20], [APX-IP] |
| M33 | Auto-run / auto-move | C | M; long travel | D | EXT; easy stop on every supported input. | [G16], [APX-HH] |
| M34 | Auto-sprint | C | M; repeated sprint activation | D | EXT; retain stealth/walk control. | [G10], [G16] |
| M35 | Automatic item pickup | C | M,C; frequent collection | D | Filter optional pickups to avoid inventory harm. | [G10], [APX-DML]; EXT |
| M36 | Automatic vault/climb/interactions | C | M; repeated traversal presses | D | EXT; prevent accidental irreversible routes. | [G10], [APX-DML] |
| M37 | Adjustable menu key-repeat delay/rate | C | M; overshoot or slow input | D | EXT; separate from game action rates. | [G24], [APX-IP] |
| M38 | Post-acceptance debounce | C | M; tremor/double presses | D | Optional duration; never impose on everyone. | [G24] |
| M39 | Longer double-press/hold recognition windows | C | M; timed input recognition | D | EXT; avoid conflicts with single-tap actions. | [G25], [APX-IP] |
| M40 | Cursor snapping / target magnetism | C | M,V; precise selection | D | EXT; allow easy escape and disable. | [APX-IP] |
| M41 | Stationary menu targets | B | M,V; animated moving controls | On | Motion must not shift a target during acquisition. | [G21] |
| M42 | Input cancellation / release activation | B | M; pointer/touch UI | On | Permit abort/undo; immediate game actions may need exceptions. | [X107], [APX-UR] |

### 4.4 Cognitive, memory and learning

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| C01 | Player-paced text | B | C,H,V; written prompts | On | Explicit advance; do not silently expire. | [G28] |
| C02 | Plain-language descriptions | B | C,S; settings/instructions | On | Explain actual effect, not diagnosis labels. | [G30] |
| C03 | Optional keyword emphasis | C | C; dense objectives | D | Do not communicate meaning solely by colour. | [G34] |
| C04 | Interactive tutorials | C | C,M; unfamiliar mechanics | D | Teach actions through doing; allow assist settings. | [G29] |
| C05 | Replay tutorials on demand | C | C; returning after absence | D | Preserve campaign progress. | [G50], [XTAGS] |
| C06 | Practice sandbox without failure | C | C,M,T; skill-based mechanics | D | Separate practising from permanent resource expenditure. | [G41], [APX-TG] |
| C07 | Control reminder overlay | C | C,M; many actions | D | Reflect current bindings and device. | [G42] |
| C08 | Current objective tracker | C | C; goals span time | D | Clearly separate primary/secondary tasks. | [G43] |
| C09 | Completed-objective journal | C | C; multi-step progression | D | Record actual completion, not inferred narrative state. | [X109], [APX-TR] |
| C10 | Explicit subtask checklist | C | C; compound objectives | D | EXT; mark prerequisites and progress. | [G43], [X109] |
| C11 | Story recap on return | C | C,S; long narrative | D | Summarise only revealed content. | [G39] |
| C12 | Character/relationship glossary | C | C; large narrative cast | D | EXT; reveal entries with the story. | [G39], [APX-TR] |
| C13 | Replay dialogue / instructions | C | C,H; important transient content | D | No cost or forced scene replay. | [G50] |
| C14 | Contextual hints | C | C,T; uncertain next action | D | Player can request, reduce or disable. | [G44] |
| C15 | Graduated hint strength | C | C,T; puzzles or navigation | D | EXT; escalate from nudge to explanation. | [G44], [APX-HH] |
| C16 | Optional solution / spoilers | C | C,T; persistent puzzle blocker | D | EXT; player chooses reveal before content appears. | [G105], [APX-BY] |
| C17 | Waypoint markers | C | C,V; spatial objectives | D | Offer density and distance control. | [G43], [X109] |
| C18 | Route trail / path guide | C | C,V; navigation barrier | D | Independent of other highlights and HUD. | [X109], [APX-HH] |
| C19 | Collectible/prerequisite progress counters | C | C; remembering prerequisites | D | EXT; make missing requirements explicit. | [X109], [APX-TR] |
| C20 | Map marker filters | C | C,N; dense map | D | EXT; saved categories; accessible list alternative. | [G49], [APX-PI] |
| C21 | Personal pins / notes | C | C; exploration memory | D | EXT; accessible text entry and narration. | [APX-TR] |
| C22 | Save thumbnails and descriptive labels | C | C,V; several saves | On | Pair image with text, time and location. | [G36] |
| C23 | Notification duration | B | C,V,H; transient notices | P | Extend or pin; support manual dismissal. | [X116], [APX-PI] |
| C24 | Persistent instruction reference | C | C; instructions before a challenge | D | Avoid requiring memorisation across menu changes. | [G50], [APX-TR] |
| C25 | Text plus demonstration plus speech | C | C,V; complex instructions | D | Meaning equivalent across offered channels. | [G38] |
| C26 | Simplified information summaries | C | C; dense stats or narrative | D | EXT; detailed view remains available. | [G40], [APX-TR] |
| C27 | Direct Continue / reduced menu steps | B | C,M; repeated starts | On | Keep accessibility configuration reachable first. | [G31] |
| C28 | Undo reversible mistakes | C | C,M; decisions or inventory | D | Clearly state limits; preserve safe data history. | [X115], [APX-UR] |
| C29 | Destructive-action confirmation | B | C,M; deletion/purchase/overwrite | On | Name item and consequence; do not require long hold only. | [X115] |
| C30 | Settings preview and per-category reset | B | C,V,M; configuration | On | EXT; undo changes without wiping unrelated preferences. | [X115], [APX-PI] |

### 4.5 Neurodivergent / sensory load

Controls in this group can pull in opposite directions. Some players need extra trackers; others need fewer. Let players tune them independently, without inferring preferences from an ADHD, autism or dyslexia preset.

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| N01 | Minimal / no HUD | C | N,C; dense overlays | D | Preserve access to essential status on demand. | [G49], [APX-PI] |
| N02 | HUD element visibility by category | C | N,V; clutter | D | EXT; avoid one destructive all-or-nothing switch. | [G49], [APX-PI] |
| N03 | Disable guiding paint/glows/trails | C | N; persistent guidance | D | Independent from objective journal and usable-object cues. | [X109], [G49] |
| N04 | Guidance only on request | C | N,C; visual overload | D | EXT; one remappable request action. | [G44], [APX-PI] |
| N05 | Static menu backgrounds | B | N,C; decorative motion | P | Freeze video/particles without losing menu meaning. | [G47], [X117] |
| N06 | Reduce decorative gameplay detail | C | N,C; dense noninteractive scene | D | Preserve collision, hazards and navigational landmarks. | [G49] |
| N07 | Reduce particles / hit-effects density | C | N,V; effects obscure play | D | EXT; keep an alternative hit cue. | [G49], [APX-CC] |
| N08 | Reduce notification frequency | C | N,C; repeated popups | D | EXT; keep history and critical alerts. | [APX-CC], [X117] |
| N09 | Disable autoplay previews | B | N,C,S; moving media UI | P | Include separate autoplay sound control. | [X117] |
| N10 | Disable menu/UI transition animation | B | N,O; animated UI | P | Instant/static alternative retains hierarchy. | [X117], [APPLE-LABELS] |
| N11 | Disable decorative blinking / pulses | B | N,P; attention effects | S: static critical cues | Retain state using icon/text. | [G27], [X117] |
| N12 | Vibration intensity / off | C | N,M; haptic output | S: mild | Preserve visual/audio equivalents. | [G6], [X110] |
| N13 | Haptics by event category | C | N,H,V; useful and noisy vibration mixed | D | EXT; navigation cues independent of ambient rumble. | [APX-CC], [X110] |
| N14 | Adaptive-trigger resistance / off | C | M,N; force-feedback triggers | D | EXT; do not require high force to proceed. | [APX-FC], [PS-TAGS] |
| N15 | Simplified / quiet audio preset | C | N,C,H; dense soundscape | D | EXT; user can then change each bus separately. | [G48], [APX-CC] |

### 4.6 Photosensitivity and flash reduction

Every row is a contextual reduction control on top of baseline hazard prevention. A “normal” mode containing known unsafe sequences is not justified by providing a second mode.

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| P01 | Global flash-reduced presentation | B | P; any visual output | S: avoid hazardous flashes | Configure before logos; do not promise seizure-proof play. | [G27], [X118] |
| P02 | Muzzle-flash suppression | C | P; repeated weapons | D within safe bounds | EXT; retain firing/reload feedback. | [X118], [APX-2C] |
| P03 | Lightning/strobe replacement | C | P; weather or venue strobes | S: static/subtle alternative | EXT; consider repeated environmental combinations. | [G27], [ITU] |
| P04 | Damage/fullscreen flash replacement | C | P,V; hit feedback | S: nonflashing cue | EXT; use steady edge/status cue instead. | [G27], [X103] |
| P05 | Explosion/ability flash intensity | C | P; combat VFX | S: bounded output | EXT; test overlapping players and abilities. | [X118], [IRIS] |
| P06 | Red-flash desaturation | C | P; saturated-red changes | S: avoid risky sequences | EXT; test current red criterion, not luminance alone. | [X118], [WCAG22] |
| P07 | Pattern/stripe reduction | C | P; repetitive high-contrast patterns | S: nonprovocative design | EXT; replace texture, not merely screen brightness. | [G27], [ITU] |
| P08 | Glitch/flicker effect disable | C | P,N; glitch presentation | S: static alternative | EXT; menus and cutscenes included. | [G27] |
| P09 | Gentle scene transitions | C | P; alternating bright/dark cuts | S: avoid rapid changes | EXT; test the output sequence. | [ITU], [G27] |
| P10 | Suppress flashing player-generated cosmetics | C | P; user-created effects | S: bounded cosmetic effects | EXT; local substitution and safety limits on content. | [X118], [IRIS] |

### 4.7 Motion comfort

No single FOV or motion setting works for everyone. Preserve independent controls rather than binding everything permanently to one reduced-motion preset.

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| O01 | Camera shake intensity / off | C | O,N,P; impact shake | S: reduced | Must reach zero, including scripted scenes. | [G65], [X117] |
| O02 | Camera/head bob intensity / off | C | O; walking oscillation | S: reduced/off | Cover sprint, crouch and scripted movement. | [G65] |
| O03 | Weapon sway / breathing motion | C | O; first-person equipment | D | EXT; do not alter aim precision inadvertently. | [G65], [X117] |
| O04 | Motion blur / off | C | O,V; postprocessing | P | Include per-object and full-screen blur. | [G65], [X117] |
| O05 | FOV adjustment | C | O,V; perspective camera | D appropriate to display | Label horizontal/vertical FOV and aspect handling. | [G57], [G66] |
| O06 | Camera distance | C | O,V; third-person view | D | EXT; preserve collision and objective visibility. | [X117], [APX-FD] |
| O07 | Camera horizontal/vertical speed | C | O,M; camera steering | D | Distinct from movement sensitivity where appropriate. | [G8], [X117] |
| O08 | Camera smoothing / lag disable | C | O,M; delayed camera response | D | Keep response predictable. | [G65] |
| O09 | Automatic recentering disable | C | O,M; camera recentres | D | Manual recenter action remains available. | [G65], [X117] |
| O10 | Disable scripted camera pulls | C | O,V; game takes camera control | D | Replace important direction with another cue. | [X117] |
| O11 | Disable automatic camera rotation | C | O; turns without player action | S: reduced | EXT; retain voluntary turn controls. | [G65], [G56] |
| O12 | Stable horizon / reduced roll | C | O; vehicle or world tilting | D | EXT; avoid hiding essential orientation data. | [G56], [G65] |
| O13 | Disable sprint/aim dynamic FOV | C | O; changing perspective zoom | D | EXT; separate aiming magnification if needed. | [G66], [G65] |
| O14 | Disable speed lines / radial blur | C | O,N; simulated speed effects | D | EXT; speed remains available in other channels. | [X117], [APX-CC] |
| O15 | Disable chromatic aberration / distortion | C | O,V; lens effects | D | EXT; remove from damage and cutscenes too. | [G65], [APX-FD] |
| O16 | Stable screen-centre marker | C | O; moving perspective | D | EXT; separate from weapon reticle. | [G56], [APX-FD] |
| O17 | Reduce parallax / background scrolling | C | O,N; layered movement | P | Preserve navigational foreground movement. | [G47], [X117] |
| O18 | VR teleport versus smooth movement | C | O,M; immersive locomotion | D comfort-oriented | EXT; equivalent destinations and progression. | [G56] |
| O19 | VR snap turning / angle choice | C | O,M; immersive rotation | D comfort-oriented | EXT; optional smooth turn remains available. | [G56] |
| O20 | VR comfort vignette | C | O; artificial movement | D | EXT; adjust strength; avoid concealing essential cues. | [G56] |
| O21 | Depth-of-field disable | C | V,O; focus/defocus effects | D | EXT; essential objects remain sharp when disabled. | [APX-FD], [X117] |

### 4.8 Phobia and content sensitivity

Specific replacements beyond spider/gore examples are **EXT proposals** applying content customisation principles. No standard promises elimination of an individual's fear. A name such as “arachnophobia mode” must describe what actually changes, including sounds, movement and associated UI.

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| F01 | Spider appearance substitution | C | F; spider-like entities | D; announce before exposure | Include icons, trophies, corpses and thumbnails. | [X123], [E-GROUNDED] |
| F02 | Other creature phobia substitutions | C | F; snakes, insects or similar content | D | EXT; alternative must retain readable threat role. | [X123], [APX-MO] |
| F03 | Reduce creature sounds/animation triggers | C | F; replacement still moves/sounds alike | D | EXT; visuals alone may not address the barrier. | [X123], [APX-CC] |
| F04 | Deep-water visibility / traversal assist | C | F,T; underwater dread | D | EXT; specify visibility, breath and enemy changes. | [X123], [APX-HH] |
| F05 | Heights/edge presentation adjustment | A | F,O; vertigo-inducing views | D | EXT; offer alternate route or view where workable. | [X123], [APX-BY] |
| F06 | Blood quantity / off | C | F,N; blood effects | D | Preserve hit confirmation with other cues. | [G52] |
| F07 | Gore/dismemberment disable | C | F,T; graphic injury | D | Cover environment, cinematics and inventory art. | [G52], [X123] |
| F08 | Nudity filter | C | F,S; sensitive imagery | D | EXT; equivalent progression and scene meaning. | [X123] |
| F09 | Profanity filter | C | F,S; spoken/written language | D | EXT; captions and audio agree without losing meaning. | [X123] |
| F10 | Needle/medical imagery replacement | C | F,T; procedures | D | EXT; specific pre-scene setting without exposed preview. | [X123] |
| F11 | Jump-scare reduction / warning | C | F,T,N; sudden surprises | D | EXT; independent of steady threat difficulty. | [G51], [X123] |
| F12 | Detailed content notices | C | F,T; sensitive themes | On before purchase/start | Layer summary/detail to let players choose spoiler depth. | [X123] |
| F13 | Opt-in granular pre-scene warnings | C | F,T; specific upcoming trigger | D or chosen at setup | Warnings must precede the content and be accessible. | [X123] |
| F14 | Sensitive scene/mission skip | C | F,T; unavoidable triggering sequence | D | Preserve rewards/progress; optional factual recap. | [X123], [APX-BY] |

### 4.9 Difficulty, assists and stress

All rows retain achievement/trophy/content parity under this brief. A preset may configure several values, but players can adjust independent barriers. Never make a difficulty description claim that one choice is the only legitimate experience.

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| D01 | Described difficulty presets | B | T,C,M; challenge progression | D | State actual mechanical effects plainly. | [G99], [X108] |
| D02 | Change difficulty without restart | B | T,C,M; adjustable challenge | On | Preserve completed progress and content. | [G107] |
| D03 | Independent mechanic difficulty | C | M,C,T; several challenge types | D | Separate combat, puzzles and traversal. | [G100] |
| D04 | Invulnerability | C | M,T; damage barrier | D | Preserve necessary death/reset recovery routes. | [G100], [APX-HH] |
| D05 | Incoming damage scale | C | M,T; combat/hazards | D | EXT; independent from outgoing damage. | [G100] |
| D06 | Outgoing damage / enemy health scale | C | M,T; sustained combat | D | EXT; avoid tedious endurance-only adjustment. | [G100] |
| D07 | Separate fall-damage/fall-failure assist | C | M,T; platforming | D | EXT; distinguish damage from respawn position. | [G100], [E-PSYCHONAUTS] |
| D08 | Independent fail-state toggles | C | T,M,C; multiple failure causes | D | EXT; combat, drowning, timers and pits separately. | [G100], [APX-HH] |
| D09 | Enemy aggression / detection scale | C | T,M,C; stealth/combat | D | EXT; explain changes to enemy behaviour. | [G100] |
| D10 | Passive/no-attack enemies | C | T,F; exploration blocked by combat | D | EXT; preserve quest and loot triggers. | [G100], [APX-HH] |
| D11 | Infinite ammunition | C | M,T,C; ammunition pressure | D | EXT; cover reload and special ammunition semantics. | [G100], [APX-HH] |
| D12 | Infinite stamina | C | M,T; exertion resource | D | EXT; preserve intentional manual stop controls. | [G100], [APX-HH] |
| D13 | Resource/crafting cost reduction | C | C,T,M; scarcity/grind | D | EXT; reversible, without corrupting progression. | [G100] |
| D14 | Game/world speed adjustment | C | M,C; real-time demands | D | Keep UI and captions usable at all speeds. | [G5], [APX-SD] |
| D15 | On-demand slow motion | C | M,C; short reaction bursts | D | EXT; remappable activation; optional toggle. | [G25], [APX-SD] |
| D16 | Extended reaction/parry timing | C | M,C; precise windows | D | EXT; communicate success using several channels. | [G25], [APX-HH] |
| D17 | Aim assistance strength | C | M,V; precision aiming | D | Offer range/strength; explain multiplayer rules. | [G104], [APX-IP] |
| D18 | Target lock / automatic aiming | C | M,V; persistent tracking | D | EXT; allow target selection and release. | [G104], [APX-HH] |
| D19 | Steering / braking assistance | C | M,C; continuous vehicle control | D | Independent assists rather than mandatory bundle. | [G104] |
| D20 | Jump/landing assistance | C | M,V; precision traversal | D | EXT; preserve destination choice. | [G104], [APX-HH] |
| D21 | Extra/infinite jumps | C | M,T; repeated traversal failure | D | EXT; retain escape/recovery from unintended spaces. | [G100], [APX-HR] |
| D22 | Puzzle difficulty / bypass | C | C,T; puzzle blocker | D | Preserve story and rewards on skip. | [G105], [APX-BY] |
| D23 | Room/encounter skip | C | M,T,C; persistent blocker | D | EXT; available by request, not only after humiliation. | [G105], [APX-BY] |
| D24 | Progress-preserving retry/checkpoint choice | C | M,T; repeated replay | D | EXT; avoid redoing already completed long sections. | [G102], [APX-SV] |
| D25 | Manual save / quick save | C | M,C,S; sessions need interruption | On | Independent manual slots; avoid guaranteed-death saves. | [G103] |
| D26 | Autosave frequency / retained history | C | C,M,T; progress risk | On | Do not overwrite all recoverable states. | [G102] |
| D27 | Pause gameplay and cinematics | B | M,C,H,S; local game | On | Explain online limits; pause presentation and simulation. | [X108], [XTAGS] |
| D28 | No-loss death / reduced penalties | C | T,C,M; costly failure | D | EXT; separate currency, inventory and time penalties. | [G100], [APX-HH] |
| D29 | Disable countdown / increase turn time | C | C,M,T; time pressure | D | Distinguish gameplay timer from UI timeout. | [G25], [APX-SD] |
| D30 | Break reminders / session-stop support | C | M,T,S; long sessions | D | EXT; optional reminder; no progress loss for stopping. | [APX-SV], [APX-SD] |

### 4.10 Communication and multiplayer

Speech-to-text, outgoing typed speech, incoming text narration and real-time text are four different capabilities. Assess their scope, languages, latency, privacy and service availability separately.

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| K01 | Free-form text chat | C | H,speech,M; player communication | Available | No voice-only requirement for participating. | [G88], [X120] |
| K02 | Voice alongside text | C | V,C,M; typing barrier | D | Offer another route for players unable to type. | [G37] |
| K03 | Incoming voice transcription | C | H; voice chat | P | Identify speaker; disclose unsupported languages/failure. | [G108], [X119] |
| K04 | Outgoing text-to-speech | C | speech,M; voice channels | P | Broadcast only submitted text, not private drafts. | [G108], [X119] |
| K05 | Canned text phrases | C | M,C,speech; typing burden | D | Does not replace free-form equivalent communication. | [G54], [APX-FT] |
| K06 | Symbol communication | C | C,speech,H; quick intent | D | Label symbols and narrate meaning. | [G54] |
| K07 | Spatial pings / markers | C | H,speech,C; team coordination | D | Redundant text/audio; rate limits adjustable for comfort. | [G87], [APX-2C] |
| K08 | Custom quick-chat phrases | C | M,C,speech; recurring messages | D | EXT; accessible editor and safe handling of text. | [APX-FT], [G54] |
| K09 | Real-time text communication | C | H,speech; potentially covered service | D / applicable duty | EXT; transmit while composing; ordinary chat is different. | [EAA], [EECC] |
| K10 | Text chat scale / contrast | C | V,C; chat text | P | Apply to input, messages, history and transcription. | [X101], [X119] |
| K11 | Chat display duration / pinning | C | C,V,H; messages expire | P | Persistent history prevents missed information. | [X116], [APX-PI] |
| K12 | Scrollback / message history | C | C,H,V; conversation pace | On | Keep accessible speaker attribution and chronology. | [X119], [APX-TR] |
| K13 | Visible active-speaker identification | C | H,C; several live speakers | D | Do not use voice colour alone. | [G82], [X120]; EXT |
| K14 | Push-to-talk versus toggle/open mic | C | M,T,S; microphone activation | D privacy-preserving | EXT; remappable; clear mic status. | [G16], [APX-FT] |
| K15 | Per-player volume / mute | C | H,N,T; loud or abusive players | D | EXT; accessible list without muting safety UI. | [APX-CC], [X120] |
| K16 | Incoming message/TTS filtering | C | N,T; spam or harassment | D | EXT; controls do not disable own communication. | [X120], [APX-MO] |
| K17 | Block/report using accessible UI | C | T,M,V; online social play | On | EXT; preserve evidence without forced replay. | [X120], [X115] |
| K18 | No-voice-required matchmaking preference | C | H,speech,T; team play | D | State social preference; avoid disclosing disability. | [G89] |
| K19 | Private/local cooperative assistance | C | M,C,T; shared play possible | D | EXT; allow help without replacing player agency. | [APX-PA] |
| K20 | Per-player settings in local co-op | C | V,H,M,N; shared display/audio | D | EXT; document unavoidable shared-channel conflicts. | [G109], [APX-PA] |
| K21 | Flexible ready-check / lobby deadlines | C | M,C; timed lobby entry | D | EXT; available before ready check; respect session constraints. | [X116], [X120] |
| K22 | Accessible communication setup path | B | V,H,M,C; any chat service | On | Entry and settings must be usable before first conversation. | [X120], [X112] |
| K23 | Agreed skill/rules matchmaking preferences | C | M,C,T; competitive multiplayer | D | Describe shared rules; no assist/diagnosis disclosure. | [G101], [APX-HR] |

### 4.11 Platform, hardware, mods and accessibility settings

These include player-facing support capabilities, not solely toggles. Mod support is an extension strategy; it does not transfer responsibility for foundational access to volunteer modders.

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| L01 | Accessibility setup before first content | B | All; first launch | On | No inaccessible logo/cinematic first. | [X112], [G84] |
| L02 | Persist accessibility preferences | B | All; repeat launches | On | Save separately from campaign progress; avoid reset on death. | [G96], [APX-LT] |
| L03 | Named settings profiles | C | All; shared device / changing needs | D | Do not name profiles by diagnosis by default. | [G109] |
| L04 | Read platform preferences | C | V,H,N,M; supported system settings | P | Apply on first use; player override takes priority. | [X102], [X104], [APPLE-HIG] |
| L05 | Multiple supported input devices | C | M,S; alternative hardware | On | All supported devices reach menus and play. | [G22], [APX-FC] |
| L06 | Simultaneous mixed-device input | C | M; keyboard plus adaptive controller | D | EXT; do not reject alternate input or flicker prompts. | [G22], [APX-FC] |
| L07 | Controller-assist / combined-controller support | C | M; shared or split input | D | Test platform feature; not automatic from controller support. | [APX-PA], [APX-FC] |
| L08 | Windowed/borderless modes | C | M,V; desktop assistive overlays | D | Virtual keyboard and screen reader must remain operable. | [G17] |
| L09 | System virtual-keyboard compatibility | C | M,V; text entry | On | Accept composition, focus and submit/cancel correctly. | [APX-FT], [G17] |
| L10 | Switch scanning input | A | M; one/few switches | D | Accessible scanning pace, grouping and selection. | [G23], [ANDROID] |
| L11 | Gaze/dwell input | A | M; pointer alternative | D | Adjustable dwell; avoid unintended selection during reading. | [G23], [APX-IP] |
| L12 | Portrait/landscape choice | C | M,S; mounted mobile device | P | Reflow controls; rotation not required to proceed. | [G26] |
| L13 | Compatible external/mobile controllers | C | M; touch difficult | D | Cover setup screens and UI, not gameplay alone. | [G22], [X107] |
| L14 | Accessibility mod loading / extension hooks | A | All; extensible desktop game | D | EXT; document supported interfaces and compatibility limits. | [APX-FC], [E-EYEMINE] |
| L15 | Mod/configuration management and recovery | A | All; third-party extensions | D | EXT; easy disable/rollback; no mandatory unsafe workaround. | [E-EYEMINE], [APX-UR] |
| L16 | Accessible feature guide in game | B | All; discovering options | On | Include effects, limits and current remapped controls. | [G97], [X121] |
| L17 | Public accessible feature documentation | B | All; deciding whether to buy | On | Per title/platform/language; keep store claims current. | [G98], [G60], [X121] |
| L18 | Multiple accessible support channels | B | All; help or feedback | On | No extra charge or voice-only barrier. | [G95], [X122] |
| L19 | Stable frame-rate / refresh choices | C | O,V,S; variable presentation | D stable target | EXT; FPS/VSync are not photosensitivity certification. | [G56], [APX-FD] |
| L20 | Focus-loss pause / resume control | C | M,S,T; desktop interruption | D | EXT; safe resumption and multiplayer limitations explained. | [X108], [APX-SV] |

### 4.12 Screen reader, TTS, audio access and voice input

A game may use platform semantics, a game-specific narration layer, or both. The player's experience must avoid duplicated speech and unreachable custom controls. Complete nonvisual gameplay is a separate, larger claim from reading the settings menu.

| ID | Option / behaviour | Tier | Helps; relevance signal | Default | Implementation / pitfall | Coverage |
|---|---|---|---|---|---|---|
| R01 | Menu narration / screen-reader support | B | V,C; text-based UI | P / accessible setup | All menus, dialogs and setup; avoid mouse-only path. | [G70], [X106] |
| R02 | Read current HUD/status on demand | C | V,C; changing player state | D | EXT; remappable, prioritised status request. | [G69], [APX-2C] |
| R03 | Automatic important status narration | C | V; time-critical state | D | Distinct priority from optional readouts. | [X106], [APX-CC] |
| R04 | Incoming text-chat narration | C | V; text communication | P | Local reading differs from broadcasting outgoing TTS. | [X119] |
| R05 | Narration speed | C | V,C,H; spoken interface | P | Allow interruption and persistence. | [X106], [APX-CT] |
| R06 | Narration voice / pitch | C | H,V; voice intelligibility | P | Test supported localised voices. | [X106] |
| R07 | Narration verbosity / enumeration | C | V,C; many role/state details | D | Never remove essential state changes with verbosity. | [X106], [APX-CC] |
| R08 | Repeat / cancel narration commands | C | V,C; missed or queued speech | On | Immediate response using every supported input. | [X106] |
| R09 | Narration language selection | C | V,C; multilingual game | P | Expose language changes; correct pronunciation metadata. | [X106], [ANDROID] |
| R10 | Accessible descriptions for meaningful images | B | V; icons, charts or diagrams | On | Purpose/function, not merely a filename. | [X103], [X106] |
| R11 | Spoken UI context and focus changes | B | V,C; screen transitions | On | Announce scene/menu changes; stop stale speech. | [X114], [X113] |
| R12 | Text/document reading view | C | V,C; world signs or books | D | EXT; structured navigation and readable transcript. | [G53], [APX-CT] |
| R13 | Cinematic audio description | A | V,C; visual storytelling | D | Localise and avoid masking dialogue. | [G67], [X111] |
| R14 | Extended audio description | A | V,C; insufficient dialogue gaps | D | Pause compatible scenes while describing. | [X111] |
| R15 | Distinct gameplay audio cues | C | V; visual-only events | D | Cues teachable and independently balanced. | [G46], [G69] |
| R16 | Spoken turn-by-turn navigation | A | V,C; spatial navigation | D | Reroute and report blocked paths reliably. | [G73] |
| R17 | Compass-aligned movement/orientation | C | V,M; spatial aiming | D | Remappable snap orientation; confirm direction. | [G72] |
| R18 | On-demand sonar / audio scene scan | A | V; object-finding/navigation | D | Keep categories distinguishable without speech overload. | [G74] |
| R19 | OS voice-control compatibility | C | M; voice used for UI navigation | On compatible semantics | Native labels/actions; speech never mandatory. | [APPLE-HIG], [ANDROID], [G92] |
| R20 | Short/custom voice command vocabulary | C | M,speech; optional recognition | D | Short terms and alternate inputs; do not assume clear speech. | [G93] |
| R21 | Dictation for text fields | C | M,C; typing difficult | D | Accessible editing/review before sending. | [APX-FT], [X107] |
| R22 | Nonverbal sound activation | A | M,speech; word recognition unusable | D | Calibrate threshold; retain switch alternative. | [G94] |

## 5. Verified shipped examples and evidence limits

These examples illustrate particular controls, not an endorsement that every part of the game is accessible. A standard's example screenshot is evidence for the feature highlighted there, not all incidental content.

| Example / primary source | Verified feature(s) relevant to taxonomy | Limit |
|---|---|---|
| [The Last of Us Part I][E-TLOU] | Remapping, automatic pickup, navigation assistance, high-contrast display, motion settings, subtitles with names/direction, and audio-description options. | Feature availability/settings differ by release; this is the 2022 PlayStation feature list, not a PC parity claim. |
| [Grounded][E-GROUNDED] | Arachnophobia Safe Mode changes spider appearance; accessibility setup also exposes text/audio-related settings. | Altered appearance does not establish that every phobia trigger is removed. |
| [Psychonauts 2][E-PSYCHONAUTS] | Separate no-fall-damage, invincibility and Narrative Combat; remapping, font legibility and camera-shake controls. | The official page says invincibility prevents damage from most attacks; do not paraphrase that as every possible fail state disappearing. |
| [Forza Horizon 5][E-FORZA] | ASL/BSL interpretation for supported in-game scenes/cinematics. | Sign support is localised and scoped; not all interactive play is interpreted. |
| [Minecraft Java captions, XAG example][X104] | Sound-event captions with offscreen direction arrows. | The XAG distinguishes this specific example from complete nonvisual access. |
| [Gears 5 input example][X107] | An alternative to repeated button taps. | Hold alternatives still require an additional route for people unable to sustain a press. |
| [The Long Dark input example][G16] | Accessible interactions replace sustained interaction holds with presses. | Verify current version and affected actions before using as an implementation specification. |
| [Eagle Island contrast example][X102] | Backdrop dimming and outlines illustrate foreground separation. | Contrast checks still need every relevant background/state. |
| [Switch 2 GameChat][NINTENDO] | System chat speech-to-text and typed speech with language-dependent support. | Not evidence that third-party in-game voice chat inherits these functions. |
| [SpecialEffect EyeMine][E-EYEMINE] | Open-source gaze-access tools for supported Minecraft setups. | Not a universal mod API, supported version guarantee or reason to omit native access. |

No claim about another game's achievement/trophy behaviour is needed to justify this skill's policy. Parity is the owner's explicit requirement; test it in the game being built.

## 6. Gaps, conflicts and audit boundaries

- **APX navigation omission:** all 22 patterns were verified, including Do More With Less through related-pattern links. Crawling only the main navigation would miss it.
- **Sony:** public categories and named examples verified; exhaustive current tag vocabulary and numeric acceptance criteria UNVERIFIED.
- **Nintendo/Google:** useful official platform guidance verified; a comprehensive public game-tag scheme equivalent to Steam/Xbox UNVERIFIED.
- **WCAG scope:** authoritative for web content, with transferable interface principles. Do not issue a whole-game “WCAG compliant” badge on the strength of text contrast checks.
- **Broadcast versus interactive safety:** ITU/Harding footage tests cannot enumerate every live camera movement, random combat combination or player-created pattern. The audit must specify tested conditions.
- **Legacy/current thresholds:** XAG's default text sizes, current AGI tag thresholds, historical subtitle advice and WCAG CSS sizing are different criteria, not interchangeable measurements.
- **Content preferences:** sensitivity is individual. Broader snake/needle/water replacements here are proposed implementations of content-control principles, not claims of a published universal phobia standard.
- **No diagnosis-based locking:** profiles may offer convenient presets, but controls remain independent and the player is not asked to prove need.
- **No one “dyslexia font”:** this report recommends selectable readable typography and spacing; it makes no unverified claim that a named font cures dyslexia.
- **Online rules:** local accessibility presentation and input adaptations should work online. If gameplay-altering rules need an agreed session configuration, describe the rules without identifying who needs them.
- **Privacy:** display chat microphone/transcription state and explain processing/failure. Do not silently record or expose accessibility preferences to other players. This is a proposed product rule, not a quotation of one statute.
- **Retrofit evidence:** “exists in the settings menu” is insufficient. Test the actual behaviour, first launch, reload, multiplayer, cinematics, localisation and combinations.

## 7. Source reference definitions

All web sources below were consulted for this assignment on 17 October 2026. Stable public URLs are used instead of tool-internal citation IDs. GAG numbers and APX codes are local; XAG numbers are official. Live sources may change after this research date.
[G5]: https://gameaccessibilityguidelines.com/include-an-option-to-adjust-the-game-speed/
[G6]: https://gameaccessibilityguidelines.com/include-toggle-slider-for-any-haptics/
[G7]: https://gameaccessibilityguidelines.com/ensure-interactive-elements-virtual-controls-are-large-and-well-spaced-particularly-on-small-or-touch-screens/
[G8]: https://gameaccessibilityguidelines.com/include-an-option-to-adjust-the-sensitivity-of-controls/
[G9]: https://gameaccessibilityguidelines.com/ensure-that-all-areas-of-the-user-interface-can-be-accessed-using-the-same-input-method-as-the-gameplay/
[G10]: https://gameaccessibilityguidelines.com/ensure-controls-are-as-simple-as-possible-or-provide-a-simpler-alternative/
[G11]: https://gameaccessibilityguidelines.com/allow-controls-to-be-remapped-reconfigured/
[G12]: https://gameaccessibilityguidelines.com/do-not-rely-on-motion-tracking-of-specific-body-types/
[G13]: https://gameaccessibilityguidelines.com/provide-a-macro-system/
[G14]: https://gameaccessibilityguidelines.com/allow-interfaces-to-be-resized/
[G15]: https://gameaccessibilityguidelines.com/allow-interfaces-to-be-rearranged/
[G16]: https://gameaccessibilityguidelines.com/avoid-provide-alternatives-to-requiring-buttons-to-be-held-down/
[G17]: https://gameaccessibilityguidelines.com/if-producing-a-pc-game-support-windowed-mode-for-compatibility-with-overlaid-virtual-keyboards/
[G18]: https://gameaccessibilityguidelines.com/avoid-repeated-inputs-button-mashing-quick-time-events/
[G19]: https://gameaccessibilityguidelines.com/ensure-that-all-key-actions-can-be-carried-out-by-digital-controls-pad-keys-presses-with-more-complex-input-eg-analogue-speech-gesture-not-required-and-included-only-as-supplementary-al/
[G20]: https://gameaccessibilityguidelines.com/ensure-that-multiple-simultaneous-actions-eg-click-drag-or-swipe-are-not-required-and-included-only-as-a-supplementary-alternative-input-method/
[G21]: https://gameaccessibilityguidelines.com/make-interactive-elements-that-require-accuracy-eg-cursor-touch-controlled-menu-options-stationary/
[G22]: https://gameaccessibilityguidelines.com/support-more-than-one-input-device/
[G23]: https://gameaccessibilityguidelines.com/provide-very-simple-control-schemes-that-are-compatible-with-assistive-technology-devices-such-as-switch-or-eye-tracking/
[G24]: https://gameaccessibilityguidelines.com/include-a-cool-down-period-post-acceptance-delay-of-0-5-seconds-between-inputs/
[G25]: https://gameaccessibilityguidelines.com/do-not-make-precise-timing-essential-to-gameplay-offer-alternatives-actions-that-can-be-carried-out-while-paused-or-a-skip-mechanism/
[G26]: https://gameaccessibilityguidelines.com/allow-play-in-both-landscape-and-portrait/
[G27]: https://gameaccessibilityguidelines.com/avoid-flickering-images-and-repetitive-patterns/
[G28]: https://gameaccessibilityguidelines.com/allow-players-to-progress-through-text-prompts-at-their-own-pace/
[G29]: https://gameaccessibilityguidelines.com/include-interactive-tutorials/
[G30]: https://gameaccessibilityguidelines.com/use-simple-clear-language/
[G31]: https://gameaccessibilityguidelines.com/allow-the-game-to-be-started-without-the-need-to-navigate-through-multiple-levels-of-menus/
[G32]: https://gameaccessibilityguidelines.com/use-simple-clear-text-formatting/
[G33]: https://gameaccessibilityguidelines.com/use-an-easily-readable-default-font-size/
[G34]: https://gameaccessibilityguidelines.com/highlight-important-words/
[G35]: https://gameaccessibilityguidelines.com/provide-a-choice-of-text-colour-low-high-contrast-choice-as-a-minimum/
[G36]: https://gameaccessibilityguidelines.com/provide-gameplay-thumbnails-with-game-saves/
[G37]: https://gameaccessibilityguidelines.com/support-voice-chat-as-well-as-text-for-multiplayer-games/
[G38]: https://gameaccessibilityguidelines.com/ensure-no-essential-information-especially-instructions-is-conveyed-by-text-alone-reinforce-with-visuals-and-or-speech/
[G39]: https://gameaccessibilityguidelines.com/if-using-a-long-overarching-narrative-provide-summaries-of-progress/
[G40]: https://gameaccessibilityguidelines.com/employ-a-simple-clear-narrative-structure/
[G41]: https://gameaccessibilityguidelines.com/include-a-means-of-practicing-without-failure-such-as-a-practice-level-or-sandbox-mode/
[G42]: https://gameaccessibilityguidelines.com/indicate-allow-reminder-of-controls-during-gameplay/
[G43]: https://gameaccessibilityguidelines.com/indicate-allow-reminder-of-current-objectives-during-gameplay/
[G44]: https://gameaccessibilityguidelines.com/include-contextual-in-game-helpguidancetips/
[G45]: https://gameaccessibilityguidelines.com/give-a-clear-indication-that-interactive-elements-are-interactive/
[G46]: https://gameaccessibilityguidelines.com/ensure-sound-music-choices-for-each-key-objects-events-are-distinct-from-each-other/
[G47]: https://gameaccessibilityguidelines.com/provide-an-option-to-turn-off-hide-background-movement/
[G48]: https://gameaccessibilityguidelines.com/provide-separate-volume-controls-or-mutes-for-effects-speech-and-background-music/
[G49]: https://gameaccessibilityguidelines.com/provide-an-option-to-turn-off-hide-all-non-interactive-elements/
[G50]: https://gameaccessibilityguidelines.com/allow-all-narrative-and-instructions-to-be-replayed/
[G51]: https://gameaccessibilityguidelines.com/avoid-any-sudden-unexpected-movement-or-events/
[G52]: https://gameaccessibilityguidelines.com/provide-an-option-to-disable-blood-and-gore/
[G53]: https://gameaccessibilityguidelines.com/provide-pre-recorded-voiceovers-for-all-text-including-menus-and-installers/
[G54]: https://gameaccessibilityguidelines.com/use-symbol-based-chat-smileys-etc/
[G55]: https://gameaccessibilityguidelines.com/provide-high-contrast-between-text-ui-and-background/
[G56]: https://gameaccessibilityguidelines.com/avoid-vr-simulation-sickness-triggers/
[G57]: https://gameaccessibilityguidelines.com/if-the-game-uses-field-of-view-3d-engine-only-set-an-appropriate-default-for-the-expected-viewing-environment/
[G58]: https://gameaccessibilityguidelines.com/ensure-no-essential-information-is-conveyed-by-a-fixed-colour-alone/
[G59]: https://gameaccessibilityguidelines.com/avoid-placing-essential-temporary-information-outside-the-players-eye-line/
[G60]: https://gameaccessibilityguidelines.com/ensure-manual-website-are-provided-in-a-screenreader-friendly-format/
[G61]: https://gameaccessibilityguidelines.com/provide-a-choice-of-cursor-crosshair-colours-designs/
[G62]: https://gameaccessibilityguidelines.com/provide-an-option-to-adjust-contrast/
[G63]: https://gameaccessibilityguidelines.com/ensure-screenreader-support-for-mobile-devices/
[G64]: https://gameaccessibilityguidelines.com/use-surround-sound/
[G65]: https://gameaccessibilityguidelines.com/avoid-or-provide-option-to-disable-any-difference-between-controller-movement-and-camera-movement/
[G66]: https://gameaccessibilityguidelines.com/if-the-game-uses-field-of-view-3d-engine-only-allow-a-means-for-it-to-be-adjusted/
[G67]: https://gameaccessibilityguidelines.com/provide-an-audio-description-track/
[G68]: https://gameaccessibilityguidelines.com/simulate-binaural-recording/
[G69]: https://gameaccessibilityguidelines.com/use-distinct-sound-music-design-for-all-objects-and-events/
[G70]: https://gameaccessibilityguidelines.com/ensure-screenreader-support-including-menus-installers/
[G71]: https://gameaccessibilityguidelines.com/ensure-that-all-key-actions-can-be-carried-out-by-digital-controls-pads-keys-presses-with-more-complex-input-eg-analogue-gesture-not-required-and-included-only-as-supplementary-alternati/
[G72]: https://gameaccessibilityguidelines.com/allow-easy-orientation-to-movement-along-compass-points/
[G73]: https://gameaccessibilityguidelines.com/provide-a-voiced-gps/
[G74]: https://gameaccessibilityguidelines.com/provide-a-pingable-sonar-style-audio-map/
[G75]: https://gameaccessibilityguidelines.com/allow-the-font-size-to-be-adjusted/
[G76]: https://gameaccessibilityguidelines.com/if-any-subtitles-captions-are-used-present-them-in-a-clear-easy-to-read-way/
[G77]: https://gameaccessibilityguidelines.com/ensure-no-essential-information-is-conveyed-by-sounds-alone/
[G78]: https://gameaccessibilityguidelines.com/provide-subtitles-for-all-important-speech/
[G79]: https://gameaccessibilityguidelines.com/provide-a-stereo-mono-toggle/
[G80]: https://gameaccessibilityguidelines.com/ensure-that-all-important-supplementary-information-eg-the-direction-you-are-being-shot-from-conveyed-by-audio-is-replicated-in-text-visuals/
[G81]: https://gameaccessibilityguidelines.com/allow-subtitle-caption-presentation-to-be-customised/
[G82]: https://gameaccessibilityguidelines.com/provide-a-visual-indication-of-who-is-currently-speaking/
[G83]: https://gameaccessibilityguidelines.com/provide-captions-or-visuals-for-significant-background-sounds/
[G84]: https://gameaccessibilityguidelines.com/ensure-subtitles-captions-are-or-can-be-turned-on-before-any-sound-is-played/
[G85]: https://gameaccessibilityguidelines.com/provide-subtitles-for-supplementary-speech/
[G86]: https://gameaccessibilityguidelines.com/keep-background-noise-to-minimum-during-speech/
[G87]: https://gameaccessibilityguidelines.com/provide-visual-means-of-communicating-in-multiplayer/
[G88]: https://gameaccessibilityguidelines.com/support-text-chat-as-well-as-voice-for-multiplayer/
[G89]: https://gameaccessibilityguidelines.com/allow-a-preference-to-be-set-for-playing-online-multiplayer-with-players-who-will-only-play-with-are-willing-to-play-without-voice-chat/
[G90]: https://gameaccessibilityguidelines.com/provide-signing/
[G91]: https://gameaccessibilityguidelines.com/ensure-that-subtitles-captions-are-cut-down-to-and-presented-at-an-appropriate-words-per-minute-for-the-target-age-group/
[G92]: https://gameaccessibilityguidelines.com/ensure-that-speech-input-is-not-required-and-included-only-as-a-supplementary-alternative-input-method/
[G93]: https://gameaccessibilityguidelines.com/base-speech-recognition-on-individual-words-from-a-small-vocabulary-eg-yes-no-open-instead-of-long-phrases-or-multi-syllable-words/
[G94]: https://gameaccessibilityguidelines.com/base-speech-recognition-on-hitting-a-volume-threshold-eg-50-instead-of-words/
[G95]: https://gameaccessibilityguidelines.com/solicit-accessibility-feedback/
[G96]: https://gameaccessibilityguidelines.com/ensure-that-all-settings-are-saved-remembered/
[G97]: https://gameaccessibilityguidelines.com/provide-details-of-accessibility-features-in-game/
[G98]: https://gameaccessibilityguidelines.com/provide-details-of-accessibility-features-on-packaging-and-or-website/
[G99]: https://gameaccessibilityguidelines.com/offer-a-wide-choice-of-difficulty-levels/
[G100]: https://gameaccessibilityguidelines.com/allow-gameplay-to-be-fine-tuned-by-exposing-as-many-variables-as-possible/
[G101]: https://gameaccessibilityguidelines.com/allow-a-preference-to-be-set-for-playing-online-multiplayer-with-without-others-who-are-using-accessibility-features-that-could-give-a-competitive-advantage/
[G102]: https://gameaccessibilityguidelines.com/provide-an-autosave-feature/
[G103]: https://gameaccessibilityguidelines.com/provide-a-manual-save-feature/
[G104]: https://gameaccessibilityguidelines.com/include-assist-modes-such-as-auto-aim-and-assisted-steering/
[G105]: https://gameaccessibilityguidelines.com/offer-a-means-to-bypass-gameplay-elements-that-arent-part-of-the-core-mechanic-via-settings-or-in-game-skip-option/
[G106]: https://gameaccessibilityguidelines.com/include-some-people-with-impairments-amongst-play-testing-participants/
[G107]: https://gameaccessibilityguidelines.com/allow-difficulty-level-to-be-altered-during-gameplay-either-through-settings-or-adaptive-difficulty/
[G108]: https://gameaccessibilityguidelines.com/realtime-text-speech-transcription/
[G109]: https://gameaccessibilityguidelines.com/allow-settings-to-be-saved-to-different-profiles-at-either-game-or-platform-level/
[G110]: https://gameaccessibilityguidelines.com/include-every-relevant-category-of-impairment-motor-cognitive-etc-amongst-play-testing-participants-in-representative-numbers-based-on-age-demographic-of-target-audience/

[X101]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/101
[X102]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/102
[X103]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/103
[X104]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/104
[X105]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/105
[X106]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/106
[X107]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/107
[X108]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/108
[X109]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/109
[X110]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/110
[X111]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/111
[X112]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/112
[X113]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/113
[X114]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/114
[X115]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/115
[X116]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/116
[X117]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/117
[X118]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/118
[X119]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/119
[X120]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/120
[X121]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/121
[X122]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/122
[X123]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/123

[APX-FC]: https://accessible.games/accessible-player-experiences/access-patterns/flexible-controllers/
[APX-SC]: https://accessible.games/accessible-player-experiences/access-patterns/same-controls-but-different/
[APX-IP]: https://accessible.games/accessible-player-experiences/access-patterns/improved-precision/
[APX-FT]: https://accessible.games/accessible-player-experiences/access-patterns/flexible-text-entry/
[APX-2C]: https://accessible.games/accessible-player-experiences/access-patterns/second-channel/
[APX-CC]: https://accessible.games/accessible-player-experiences/access-patterns/clear-channels/
[APX-DT]: https://accessible.games/accessible-player-experiences/access-patterns/distinguish-this-from-that/
[APX-FD]: https://accessible.games/accessible-player-experiences/access-patterns/flexible-displays/
[APX-CT]: https://accessible.games/accessible-player-experiences/access-patterns/clear-text/
[APX-PI]: https://accessible.games/accessible-player-experiences/access-patterns/personal-interface/
[APX-LT]: https://accessible.games/accessible-player-experiences/access-patterns/leave-it-there/
[APX-MO]: https://accessible.games/accessible-player-experiences/challenge-patterns/moderation-in-all-things/
[APX-UR]: https://accessible.games/accessible-player-experiences/challenge-patterns/undo-redo/
[APX-PA]: https://accessible.games/accessible-player-experiences/challenge-patterns/play-alongside/
[APX-HR]: https://accessible.games/accessible-player-experiences/challenge-patterns/house-rules/
[APX-HH]: https://accessible.games/accessible-player-experiences/challenge-patterns/helping-hand/
[APX-SD]: https://accessible.games/accessible-player-experiences/challenge-patterns/slow-it-down/
[APX-BY]: https://accessible.games/accessible-player-experiences/challenge-patterns/bypass/
[APX-TG]: https://accessible.games/accessible-player-experiences/challenge-patterns/training-ground/
[APX-TR]: https://accessible.games/accessible-player-experiences/challenge-patterns/total-recall/
[APX-SV]: https://accessible.games/accessible-player-experiences/challenge-patterns/save-early-save-often/
[APX-DML]: https://accessible.games/accessible-player-experiences/access-patterns/do-more-with-less/

[AGI]: https://accessiblegames.com/wp-content/uploads/2025/03/Accessible-Games-Initiative-Tags-and-Criteria-March-2025.pdf
[XTAGS]: https://learn.microsoft.com/en-us/xbox/accessibility/accessibility-feature-tags
[STEAM]: https://partner.steamgames.com/doc/accessibility_features
[PS-TAGS]: https://blog.playstation.com/2023/04/03/accessibility-tags-roll-out-this-week-on-playstation-store-on-the-ps5-console/
[NINTENDO]: https://www.nintendo.com/en-gb/Support/Troubleshooting/Guide-to-Accessibility-Features-on-Nintendo-Switch-2-2843775.html
[APPLE-HIG]: https://developer.apple.com/design/human-interface-guidelines/accessibility
[APPLE-LABELS]: https://developer.apple.com/help/app-store-connect/manage-app-accessibility/overview-of-accessibility-nutrition-labels/
[ANDROID]: https://developer.android.com/guide/topics/ui/accessibility/principles
[ANDROID-DESIGN]: https://developer.android.com/design/ui/mobile/guides/foundations/accessibility
[WCAG-CONTRAST]: https://www.w3.org/WAI/WCAG22/Understanding/contrast-minimum.html
[WCAG-NONTEXT]: https://www.w3.org/WAI/WCAG22/Understanding/non-text-contrast.html
[WCAG-RESIZE]: https://www.w3.org/WAI/WCAG22/Understanding/resize-text.html
[WCAG-SPACING]: https://www.w3.org/WAI/WCAG22/Understanding/text-spacing.html
[WCAG-TARGET]: https://www.w3.org/WAI/WCAG22/Understanding/target-size-minimum.html
[WCAG-FLASH]: https://www.w3.org/WAI/WCAG22/Understanding/three-flashes-or-below-threshold.html
[WCAG-THREE]: https://www.w3.org/WAI/WCAG21/Understanding/three-flashes
[WCAG22]: https://www.w3.org/TR/WCAG22/#dfn-general-flash-and-red-flash-thresholds
[BBC]: https://www.bbc.co.uk/accessibility/forproducts/guides/subtitles/
[HAMILTON-SUB]: https://ian-hamilton.com/how-to-do-subtitles-well-basics-and-good-practices/
[ITU]: https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.1702-3-202311-I%21%21PDF-E.pdf
[IRIS]: https://github.com/electronicarts/IRIS
[PEAT]: https://trace.umd.edu/photosensitive-epilepsy-analysis-tool-peat-user-guide/
[OKABE]: https://jfly.uni-koeln.de/color/
[CFR14]: https://www.ecfr.gov/current/title-47/chapter-I/subchapter-A/part-14
[EAA]: https://eur-lex.europa.eu/eli/dir/2019/882/oj/eng
[EECC]: https://eur-lex.europa.eu/eli/dir/2018/1972/oj/eng
[UK-EQUALITY]: https://www.gov.uk/government/publications/equality-act-guidance/disability-quick-start-guide-for-service-providers-html
[UK-PUBLIC]: https://www.gov.uk/guidance/accessibility-requirements-for-public-sector-websites-and-apps
[E-TLOU]: https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/
[E-GROUNDED]: https://grounded.obsidian.net/news/grounded/accessibility-features
[E-PSYCHONAUTS]: https://www.doublefine.com/games/support/psychonauts-2/accessibility?platform=all
[E-FORZA]: https://forza.net/news/forza-horizon-5-series-5-update
[E-EYEMINE]: https://github.com/SpecialEffect/EyeMine

## Implications for the skill

1. **Start with mechanic signals.** Inventory text, audio, perspective cameras, motion, timing, precision input, narrative, phobia content, resource/fail systems, communication, supported hardware and target markets. Load only applicable category/engine references.
2. **Keep priorities separate from source tiers.** Record Baseline/Contextual/Advanced, original GAG level, XAG IDs, APX patterns and store criteria independently. A legal duty cannot be downgraded by a budget tier.
3. **Use the catalogue as capability choices.** Select controls because of observed barriers; do not generate hundreds of empty settings. Preserve the designer's ordinary defaults except safety and existing platform/player preferences.
4. **Provide accessible configuration first.** No hazardous flash, loud intro, unreadable setup or inaccessible account flow before players can configure what they need.
5. **Make settings independent and reversible.** Save them outside campaign state; provide previews, per-category reset, optional presets and an explanation of actual effects.
6. **Enforce parity.** Assists never lock achievements, trophies, content, endings or grades; never mark a save or publicly badge a player. Verify this in code and a runtime unlock/progression check.
7. **Keep conflicting needs available.** Trackers/highlights can be added, hidden or requested on demand. High contrast, low-contrast comfort, haptics and audio cues need separate choices.
8. **Do not swap one motor barrier for another.** For mashing, consider hold, toggle, automation and skip. Test remapping through menus, tutorials, overlays and supported devices.
9. **Budget nonvisual access early.** Semantic UI, focus and narration are foundational. Navigation audio, cinematic description and whole-game blind play require separate scope and tests.
10. **Treat numeric standards as versioned data.** Store source, edition, unit, scope, exceptions and measurement method. Distinguish WCAG visual-angle flashes from ITU screen-area tests and XAG approximations.
11. **Test safety without exposure.** Scan captured output before sensitive playtesting; retain analysis reports for worst-case repeated/combined effects. A warning or passing one recording is insufficient.
12. **Audit communication end to end.** Separate incoming STT, outgoing TTS, local text narration and RTT. Route jurisdiction/service classification to the legal owner; test every supported language and failure state.
13. **Make platform claims only after verification.** Map exact storefront field names to tested outcomes; never infer complete blind play from menu narration or captions from a text-only store badge.
14. **Use disabled-player feedback throughout.** Include relevant impairment groups, assistive devices and situational needs in planning and testing; pay attention to combinations, not only one isolated preset.
15. **Support both build and retrofit modes.** New work gets scoped acceptance criteria before architecture. Retrofit work inventories existing behaviour, reproduces barriers, changes the smallest affected paths and retests progression.
16. **Require evidence per selected option.** Record trigger, player outcome, default, implementation location, test steps, tested mode/language/device, source IDs and known limits. Mark unavailable or unverified evidence explicitly.










