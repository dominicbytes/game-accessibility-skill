# Option catalogue

The canonical list of **319 player-facing options and support capabilities** in 12 categories.
Every other file uses these IDs. IDs are stable: never renumber, add new IDs at the end of a
category. Research basis: GAG, Xbox Accessibility Guidelines (XAG), AbleGamers APX, AGI, Steam,
WCAG and ITU material, checked 17 October 2026.

**Add an option only when its signal is present.** The "Signal" column is a short hint. The full
routing (which signal selects which options and guides) is in [signal-routing.md](signal-routing.md).
Do not create empty toggles for mechanics the game does not have; record the absence in
`ACCESSIBILITY.md` instead ([core-policy.md](core-policy.md) section 4). Every option obeys the
no-penalty rule in core-policy section 2: no achievement locks, no save marks, no shaming.

## How to read the tables

| Column | Meaning |
| --- | --- |
| **Tier** | **B** Baseline: assess in every game, implement whenever the content exists. **C** Contextual: add when the signal is present (one mandatory occurrence counts). **A** Advanced: large bespoke work; record an explicit scope decision, never drop silently. |
| **Helps** | Who may benefit. **V** vision, **H** hearing, **M** motor/dexterity/pain/fatigue, **C** cognitive/memory/learning, **N** neurodivergent/sensory load, **P** photosensitivity/migraine, **O** motion sickness/comfort, **F** phobia/content sensitivity, **T** stress/mental health, **S** situational, **speech** speech differences, **All** everyone. These describe possible benefit, never a diagnosis. |
| **Signal** | Short relevance hint: the content or mechanic that makes the option necessary. |
| **Default** | **Intended**: the designer's normal behaviour; the option is off or at the designer value until chosen. **Safe:<x>**: a safety default from core-policy section 3. **On**: the accessible behaviour is simply present, no switch needed. **Platform pref**: use the supported OS or platform setting as the initial value until the player sets the key; fall back to Intended when unavailable. |
| **Key** | The [settings-registry.json](../assets/settings-registry.json) key. `game.*` is a suggested project-specific key that is not in the shared registry; rename it to fit the project. `—` means the row is a design rule, capability or document, not a stored setting. |
| **Note** | One-line implementation note or pitfall. |

The letter **O** means motion in both the category IDs (O01-O21) and the Helps codes. Many rows
are proposed applications of a guideline principle, not an exact published requirement. Check the
category's **Sources** line before claiming that a standard requires a specific switch. GAG
labels such as `G75` are local short names for GAG pages; GAG does not publish numbers. XAG
numbers (`X101`-`X123`) are official.

Numeric thresholds (text size, contrast, subtitles, targets, deadlines, flashing) are at the end:
[Numeric thresholds](#numeric-thresholds).

## V: Visual presentation

Sources: GAG [G14], [G32], [G33], [G35], [G55], [G58], [G59], [G61], [G62], [G75]; XAG [X101], [X102], [X103], [X113]; APX [APX-CT], [APX-DT], [APX-FD], [APX-PI]; [WCAG-SPACING].
Guides: [text-and-readability](topics/text-and-readability.md), [color-and-clarity](topics/color-and-clarity.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| V01 | Menu text scale | B | V,C,S | Any text UI | Platform pref | `text.scale` | Reflow every layout, dialogs included; nothing clips. |
| V02 | HUD text scale | C | V,S | Gameplay readouts | Platform pref | `text.scale` | Scale independently of menus and world text if needed (`game.text.hud_scale`). |
| V03 | Readable font alternative | B | V,C | Decorative typography | On | `text.font` | Plain font with complete glyphs for every shipped language. |
| V04 | Font weight | C | V | Thin glyphs | Intended | `game.text.weight` | Bold keeps counters and spacing open. |
| V05 | Sentence-case text | C | C,V | All-caps paragraphs | On | — | Keep acronyms and localisation intact. |
| V06 | Letter spacing | C | C,V | Dense text | Intended | — | Reflow; too much tracking also hurts reading. |
| V07 | Word spacing | C | C,V | Dense prose | Intended | — | Respect language-specific word segmentation. |
| V08 | Line spacing | C | C,V | Multi-line text | Intended | `text.line_spacing` | Keep selection and hit regions aligned. |
| V09 | Paragraph width | C | C,V | Journals, codex | Intended | — | Adjustable line length without two-axis scrolling. |
| V10 | Text alignment | C | C,V | Centred long passages | Intended | — | Respect text direction; avoid forced justification. |
| V11 | Text foreground colour | B | V,N | Reading | Platform pref | `game.text.color` | Preview against the real background. |
| V12 | Text background colour | B | V,N | Reading | Platform pref | `game.text.background_color` | Store separately from the text colour. |
| V13 | Text panel opacity | C | V,C | Scene visible behind UI | Intended | `text.background_opacity` | Include fully opaque (100%). |
| V14 | Light / dark / high-contrast UI themes | B | V,N | Any UI | Platform pref | `game.ui.theme` | Measure every state; dark alone is not high contrast. |
| V15 | Low-contrast reading theme | C | V,N | Glare-sensitive reading | Intended | `game.ui.theme` | Optional comfort choice; keep the high-contrast theme too. |
| V16 | UI element scale | B | V,M | Icons and widgets | Platform pref | `visual.ui_scale` | Enlarge hit areas as well as art. |
| V17 | HUD repositioning | C | V,M | Restricted visual field | Intended | `game.hud.layout` | Keep focus order and safe areas. |
| V18 | HUD margins / safe area | C | V,S | Overscan, peripheral limits | Intended | `game.hud.safe_area` | Bring essential data inward. |
| V19 | Cursor size | C | V,M | Pointer use | Platform pref | `visual.ui_scale` | Keep the hotspot obvious. |
| V20 | Cursor colour / outline | C | V | Changing backgrounds | Intended | `game.cursor.color` | Contrasting edge; preview bright and dark scenes. |
| V21 | Reticle size / shape / thickness | C | V | Aiming | Intended | `game.reticle.style` | Do not remove spread or recoil information by accident. |
| V22 | Reticle colour | C | V | Aiming | Intended | `game.reticle.color` | Keep separate from the team/enemy palette. |
| V23 | Protan / deutan / tritan palette presets | C | V | Colour carries meaning | Intended | `visual.color_profile` | Supplements V25; never claim a cure. |
| V24 | Custom colours by semantic role | C | V | Teams, rarity, objectives | Intended | `visual.color_profile`, `visual.custom_colors` | Store per role (ColorTokens), not one global tint. |
| V25 | Redundant symbols, labels or patterns | B | V,C | Colour-coded information | On | — | Meaning survives grayscale. Always shown, not a toggle. |
| V26 | Enemy / ally outlines | C | V | Entities blend into scene | Intended | `visual.outline_enemies`, `visual.outline_allies` (umbrella `visual.high_contrast`) | Tune visibility and colour separately. |
| V27 | Interaction highlights | C | V,C | Hard-to-find usable objects | Intended | `visual.outline_interactables` (umbrella `visual.high_contrast`), `guidance.highlights` | Offer persistent and on-request choices. |
| V28 | Platform / edge outlines | C | V | Visual navigation hazards | Intended | `visual.outline_hazards` (umbrella `visual.high_contrast`) | Do not hide landing geometry. |
| V29 | Background dimming / foreground isolation | C | V,C | Visually complex scenes | Intended | `visual.background_dim` | Keep hazards and depth cues; separate from outlines. |
| V30 | World-text readable overlay | C | V,C | Signs or documents drawn in art | Intended | `game.text.world_overlay` | Show equivalent text; avoid tiny baked lettering. |
| V31 | Magnifier / zoom inspection | C | V | Detailed visual material | Intended | `game.visual.magnifier` | Pair with text scale; no forced camera motion. |
| V32 | Persistent focus appearance | B | V,C,M | Navigable menus | On | — | Never shown only by animation or colour. |
| V33 | Focus indicator style / thickness | C | V,N | Default focus hard to see | Intended | `game.ui.focus_style` | Offer a static, high-contrast variant. |
| V34 | Critical indicator centralisation | C | V,C | Time-limited peripheral events | Intended | `game.hud.central_alerts` | Optional copy near gaze; must not block action. |
| V35 | Brightness / gamma | C | V,N | Dark or glaring scenes | Intended | `visual.brightness` | Calibrate in real scenes; not a contrast fix. |
| V36 | HDR peak / paper-white comfort | C | V,N,P | HDR output | Intended (bounded output) | `game.visual.hdr_peak` | Test real displays; make no "safe brightness" claim. |

## H: Audio and hearing (mix, subtitles, captions)

Sources: GAG [G48], [G64], [G68], [G76], [G77], [G78], [G79], [G80], [G81], [G82], [G83], [G84], [G85], [G86], [G90], [G91]; XAG [X104], [X105], [X110]; APX [APX-CC], [APX-2C], [APX-TR].
Guide: [audio-and-captions](topics/audio-and-captions.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| H01 | Master volume / mute | B | H,N,S | Any audio | Safe: quiet initial mix | `audio.master` | Reachable before any sound; keep limiter headroom. |
| H02 | Music volume / mute | C | H,N,C | Soundtrack | Intended | `audio.music` | Keep meaningful musical cues off this bus. |
| H03 | Dialogue volume / mute | C | H,C | Character speech | Intended | `audio.voice` | Include radios and cutscenes. |
| H04 | Critical SFX volume | C | H,V | Gameplay sounds | Intended | `audio.sfx` | Separate useful signals from ambience. |
| H05 | Ambient volume | C | H,N | Background audio | Intended | `audio.ambience` | Weather must not mask warnings. |
| H06 | Accessibility-cue volume | C | V,H | Extra audio navigation cues | Intended | `game.audio.cues` | Own bus, separate from general effects. |
| H07 | Narration volume | C | V,C | Spoken UI | Platform pref | `game.audio.narration` | Never tied to a dialogue bus the player turned down. |
| H08 | Voice chat volume | C | H,N | Live communication | Intended | `game.audio.voice_chat` | Independent of dialogue and TTS. |
| H09 | Mono mix | C | H,S | Stereo or surround cues | Platform pref | `audio.mono` | Sum to both outputs; test phase cancellation. |
| H10 | Stereo / surround output choice | C | V,H | Spatial cues | Platform pref | `game.audio.output_mode` | Keep positional meaning in every output. |
| H11 | Headphone binaural / spatial mix | C | V,H | Spatial world | Intended | `game.audio.headphone_mode` | State that headphones are needed; not a visual-cue substitute. |
| H12 | Dialogue ducking | C | H,C | Sounds mask speech | Intended | `game.audio.dialogue_ducking` | Smooth attack and release; no clipped words. |
| H13 | Screen-reader ducking | C | V,C | Assistive speech | Intended | `game.audio.narration_ducking` | Duck only while speech plays. |
| H14 | Reduced dynamic range | C | H,N,S | Sudden loud/quiet changes | Safe: bounded peaks (option off) | `audio.reduce_loud_sounds` | Compress peaks; keep cue distinctions. |
| H15 | Frequency / tinnitus comfort filter | A | H,N | Aversive tones | Intended | `game.audio.frequency_filter` | Add another cue if filtering masks an event. |
| H16 | Pause / replay important audio | C | H,C | Narrative or audio instructions | On | — | Replay without repeating a hazard. |
| H17 | Dialogue subtitles | B | H,C,S | Any meaningful speech | On (Platform pref; offered at first launch) | `subtitles.dialogue` | Cover boot, tutorial and radio lines. |
| H18 | Secondary / bark subtitles | C | H,C | Background or systemic speech | Intended | `subtitles.dialogue` | Separable from story lines if needed (`game.subtitles.barks`). |
| H19 | Non-speech sound captions | C | H | Meaningful sound events | Intended | `subtitles.sound_captions` | Caption function and origin; prioritise. |
| H20 | Speaker names | C | H,C | Ambiguous speakers | On with subtitles | `subtitles.speaker_names` | Do not reveal a deliberately unknown speaker. |
| H21 | Off-screen speaker direction | C | H | Spatial dialogue | Intended | `subtitles.direction` | Direction updates consistently. |
| H22 | Sound direction / range in captions | C | H | Unseen events | Intended | `subtitles.sound_captions`, `subtitles.direction` | Give actionable distance. |
| H23 | Delivery / tone / source labels | C | H,C | Whisper, radio, meaningful tone | Intended | `game.subtitles.tone_labels` | Include what the line needs; no spoilers. |
| H24 | Subtitle size | B | V,H | Subtitles | Platform pref | `subtitles.size` | Independent of UI scale. |
| H25 | Subtitle font / weight | C | V,C,H | Subtitles | Intended | `game.subtitles.font` | Plain font; check translated scripts. |
| H26 | Subtitle text colour | C | V,H | Subtitles | Platform pref | `game.subtitles.color` | Speaker colour supplements names, never replaces them. |
| H27 | Subtitle background colour | C | V,H | Subtitles | Platform pref | `game.subtitles.background_color` | Include black/white combinations. |
| H28 | Subtitle background opacity | C | V,H | Changing backdrop | Intended | `subtitles.background_opacity` | Include 100%. |
| H29 | Subtitle outline / shadow | C | V,H | Text separation | Intended | `game.subtitles.outline` | An outline alone can fail on busy scenes. |
| H30 | Subtitle position / safe margins | C | H,V | UI collisions | Intended | `game.subtitles.position` | Clamp to the visible area at maximum size. |
| H31 | Caption duration / manual advance | C | C,H,V | Not enough reading time | Intended | `text.auto_advance` | Pause compatible scenes; never desync live threats. |
| H32 | Caption category filters | C | H,N | Many simultaneous captions | Intended | `game.subtitles.categories` | Critical events stay in reduced mode. |
| H33 | Dialogue / caption history | C | H,C | Missed lines | Intended | `game.subtitles.history` | Show speakers and order; no future lines. |
| H34 | Sound visualiser / threat indicators | C | H | Sound-dependent gameplay | Intended | `audio.sound_visualizer` | Cover important events without clutter. |
| H35 | Haptic equivalents for audio events | C | H,V | Event cues | Intended | `game.haptics.audio_cues` | Never the only substitute. |
| H36 | Sign-language interpretation | A | H | Speech-heavy scripted content | Intended | `game.subtitles.sign_language` | Correct local sign language; size and position controls. |
| H37 | Separate speech and subtitle languages | C | H,C,S | Localised dialogue | Platform pref | `game.language.subtitles` | Persist; document language coverage. |

## M: Motor and input

Sources: GAG [G7], [G8], [G9], [G10], [G11], [G12], [G13], [G15], [G16], [G18], [G19], [G20], [G21], [G24], [G25]; XAG [X107]; APX [APX-SC], [APX-IP], [APX-DML], [APX-FC]; store criteria [XTAGS], [STEAM].
Guides: [input-and-motor](topics/input-and-motor.md), [touch-and-mobile](topics/touch-and-mobile.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| M01 | Full action remapping | B | M,S | Any player input | On | `game.input.bindings` | Gameplay, menus, minigames, shortcuts; recovery path if confirm is unbound. |
| M02 | Multiple bindings per action | C | M,S | Mixed setups | Intended | `game.input.bindings` | Keep a working fallback during rebinding. |
| M03 | Separate overloaded actions | C | M,C | One key does several things | Intended | `game.input.bindings` | Let each action bind on its own. |
| M04 | Device-specific binding profiles | C | M,S | Keyboard / controller / touch | Intended | `game.input.bindings` | Switching device never erases settings. |
| M05 | Left / right layouts | C | M | Handedness | Intended | `game.input.layout_preset` | Presets add to full remapping, never replace it. |
| M06 | Swap stick functions | C | M | Asymmetric hand function | Intended | `game.input.swap_sticks` | Update prompts and camera bindings. |
| M07 | Independent X/Y inversion | C | M,O | Analog look | Intended | `input.invert_y` | Per axis, stick and context; never forced pairs. |
| M08 | Independent sensitivity | C | M | Fine/coarse movement limits | Intended | `input.look_sensitivity` | Aim, mouse, trigger and wheel as applicable. |
| M09 | Inner / outer dead zones | C | M | Stick drift, limited travel | Intended | `game.input.deadzone` | Live preview; full output stays reachable. |
| M10 | Response curve | C | M | Uneven usable range | Intended | `game.input.response_curve` | Live preview of the effect. |
| M11 | Acceleration / smoothing | C | M,O | Cursor/camera mismatch | Intended | `game.input.acceleration` | Allow immediate, predictable motion. |
| M12 | Trigger actuation threshold | C | M | Limited force or travel | Intended | `game.input.trigger_threshold` | Avoid accidental activation. |
| M13 | Toggle instead of hold | C | M | Aim, crouch, interact holds | Intended | `input.hold_mode` | Per action; visible state; latch clears on pause, death, focus loss. |
| M14 | Hold instead of repeated taps | C | M | Mashing | Intended | `input.mash_mode` | Also offer single press or auto for players who cannot hold. |
| M15 | Automatic repeated action | C | M | Rapid fire, repetitive work | Intended | `input.mash_mode` | One start/stop action. |
| M16 | QTE auto-complete / skip | C | M,C,T | QTEs, timed sequences | Intended | `input.qte_mode` | Normal success branch; story and rewards kept. |
| M17 | Relaxed QTE timing | C | M,C | Reaction windows | Intended | `input.qte_mode` | Adjust duration and repetition separately. |
| M18 | Sequential alternative to chords | C | M | Simultaneous presses | Intended | `game.input.sticky_modifiers` | Latch modifiers; show the current stage. |
| M19 | Macros / combo shortcuts | C | M,C | Complex sequences | Intended | `game.input.macros` | Bound to game actions; state multiplayer rules. |
| M20 | Single-stick gameplay | C | M | Move plus camera | Intended | `game.input.single_stick` | Include stick-click and D-pad dependencies. |
| M21 | Keyboard-only play and UI | C | M,V | PC release | On (when supported) | — | No mouse-only launcher, map or quit button. |
| M22 | Mouse-only play and UI | C | M | One-pointer setups | Intended | — | Pointer route to every required action. |
| M23 | Digital-only alternative | C | M,V | Analog input required | Intended | — | Digital equivalents for variable actions and looking. |
| M24 | Touch-only play | C | M,S | Touch platform | On (when supported) | — | No mandatory keyboard or controller. |
| M25 | Motion-free control | C | M,O | Gyro, gesture, body input | On (alternative) | — | Motion input stays optional. |
| M26 | Body-tracking calibration / seated use | C | M | Tracked motion | Intended | `game.xr.calibration` | Never assume a standing body. |
| M27 | Touch target size | C | M,V | Touch controls | Platform pref | `game.touch.target_scale` | Enlarge hit areas without overlap. |
| M28 | Touch target position | C | M | Limited reach | Intended | `game.touch.layout` | The layout editor must itself be accessible. |
| M29 | Touch target spacing | C | M | Imprecise touches | Intended | `game.touch.layout` | Neighbours never steal a tap. |
| M30 | Tap alternative to swipe / path gesture | C | M | Precision gestures | Intended | — | Button equivalent for every gesture. |
| M31 | Single-point alternative to pinch | C | M | Multi-touch required | Intended | — | Explicit zoom and rotate controls. |
| M32 | Click-select then click-place | C | M | Drag and drop | Intended | `game.input.click_to_place` | Cancel without committing. |
| M33 | Auto-run / auto-move | C | M | Long travel | Intended | `game.input.auto_run` | Stops on any supported input. |
| M34 | Auto-sprint | C | M | Repeated sprint activation | Intended | `game.input.auto_sprint` | Keep walk and stealth control. |
| M35 | Automatic item pickup | C | M,C | Frequent collection | Intended | `game.input.auto_pickup` | Filter optional pickups. |
| M36 | Automatic vault / climb / interact | C | M | Repeated traversal presses | Intended | `game.input.auto_traverse` | Prevent accidental irreversible routes. |
| M37 | Menu key-repeat delay / rate | C | M | Overshoot or slow input | Intended | `game.input.menu_repeat` | Separate from gameplay action rates. |
| M38 | Post-acceptance debounce | C | M | Tremor, double presses | Intended | `game.input.debounce` | Optional duration; never forced on everyone. |
| M39 | Longer double-press / hold recognition | C | M | Timed input recognition | Intended | `input.timing_window_scale` | Avoid conflicts with tap actions. |
| M40 | Cursor snapping / target magnetism | C | M,V | Precise selection | Intended | `game.input.cursor_snap` | Easy to escape and to disable. |
| M41 | Stationary menu targets | B | M,V | Animated or moving controls | On | — | Targets never move during selection. |
| M42 | Input cancellation / release activation | B | M | Pointer or touch UI | On | — | Allow abort or undo; instant game actions may be exceptions. |
| M43 | XR seated mode | C | M,S | VR/AR requires standing or room-scale movement | Intended; offered before interaction | game.xr_seated | Every required object, menu and recenter reachable while seated. |
| M44 | XR height and reach calibration | C | M,S | VR objects placed at body-relative heights | Intended; calibrate on first launch | game.xr_reach | Rescale or relocate interactables; height scaling alone does not remove bending. |
| M45 | XR virtual crouch and distant grab | C | M | VR floor pickups, low or far objects | Intended | game.xr_virtual_crouch | Button crouch and ray/force grab with the same rewards and timing. |
| M46 | XR one-handed parity and handedness | C | M | VR two-hand tools or off-hand menus | Intended | game.xr_one_handed | Every essential action, including settings, reachable with either single hand. |
| M47 | AR alternative to physical-location gates | A | M,S | Real-world walking, GPS or geofence requirements | Developer decision | — | Stationary or remote route to the same content; record scope. |

## C: Cognitive, memory and learning

Sources: GAG [G28], [G29], [G30], [G31], [G34], [G36], [G38], [G39], [G40], [G41], [G42], [G43], [G44], [G50], [G105]; XAG [X109], [X115], [X116]; APX [APX-TR], [APX-TG], [APX-HH], [APX-UR], [APX-BY].
Guide: [memory-and-navigation](topics/memory-and-navigation.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| C01 | Player-paced text | B | C,H,V | Written prompts | On | `text.reveal`, `text.auto_advance` | Explicit advance; text never silently expires. |
| C02 | Plain-language descriptions | B | C,S | Settings, instructions | On | — | Describe the effect, not a diagnosis. |
| C03 | Keyword emphasis | C | C | Dense objectives | Intended | `game.text.keyword_emphasis` | Never by colour alone. |
| C04 | Interactive tutorials | C | C,M | Unfamiliar mechanics | Intended | — | Teach by doing; assists apply in tutorials too. |
| C05 | Replay tutorials on demand | C | C | Returning after a break | Intended | — | Campaign progress is kept. |
| C06 | Practice without failure | C | C,M,T | Skill-based mechanics | Intended | — | No permanent resource cost. |
| C07 | Control reminder overlay | C | C,M | Many actions | Intended | `game.hud.control_reminders` | Shows current bindings and device. |
| C08 | Current objective tracker | C | C | Goals span time | Intended | `memory.objective_reminder` | Separate primary and secondary tasks. |
| C09 | Completed-objective journal | C | C | Multi-step progress | Intended | — | Record actual completion, not inferred state. |
| C10 | Subtask checklist | C | C | Compound objectives | Intended | `memory.objective_reminder` | Show prerequisites and progress. |
| C11 | Story recap on return | C | C,S | Long narrative | Intended | `memory.recap_on_load` | Summarise only revealed content. |
| C12 | Character / relationship glossary | C | C | Large cast | Intended | — | Unlock entries with the story. |
| C13 | Replay dialogue / instructions | C | C,H | Important transient content | Intended | — | No cost; no forced scene replay. |
| C14 | Contextual hints | C | C,T | Unclear next action | Intended | `memory.hints` | Player can request, reduce or disable. |
| C15 | Graduated hint strength | C | C,T | Puzzles, navigation | Intended | `memory.hints` | Nudge, then clearer hint, then explanation. |
| C16 | Optional solution / spoilers | C | C,T | Persistent puzzle blocker | Intended | `memory.hints` | Player chooses to reveal before it appears. |
| C17 | Waypoint markers | C | C,V | Spatial objectives | Intended | `guidance.objective_marker` | Density and distance control. |
| C18 | Route trail / path guide | C | C,V | Navigation barrier | Intended | `guidance.trail` (overall: `guidance.level`) | Independent of other highlights and HUD. |
| C19 | Collectible / prerequisite counters | C | C | Remembering prerequisites | Intended | — | Make missing requirements explicit. |
| C20 | Map marker filters | C | C,N | Dense map | Intended | `game.map.filters` | Saved categories; accessible list view. |
| C21 | Personal pins / notes | C | C | Exploration memory | Intended | — | Accessible text entry and narration. |
| C22 | Save thumbnails and labels | C | C,V | Several saves | On | — | Image plus text, time and place. Never an assist mark. |
| C23 | Notification duration | B | C,V,H | Transient notices | Platform pref | `notifications.mode` | Extend, pin or dismiss by hand (`game.notifications.duration`). |
| C24 | Persistent instruction reference | C | C | Instructions before a challenge | Intended | — | No memorising across menu changes. |
| C25 | Text plus demonstration plus speech | C | C,V | Complex instructions | Intended | — | Same meaning in every channel offered. |
| C26 | Simplified summaries | C | C | Dense stats or narrative | Intended | — | Detailed view stays available. |
| C27 | Direct Continue / fewer menu steps | B | C,M | Repeated starts | On | — | Accessibility settings still come first. |
| C28 | Undo reversible mistakes | C | C,M | Decisions, inventory | Intended | — | State the limits; keep a safe history. |
| C29 | Destructive-action confirmation | B | C,M | Delete, purchase, overwrite | On | — | Name the item and consequence; not long-hold only. |
| C30 | Settings preview and per-category reset | B | C,V,M | Any settings screen | On | — | Undo without wiping unrelated settings. |
| C31 | Inventory search, sort and filter | C | C,V,M | Many items, slots or containers | Intended | — | Keep focus after sort; narrate name, quantity and state. |
| C32 | Favourites, pins and compare view | C | C,V | Equipment or stat choices | Intended | — | Side-by-side comparison in text, not colour arrows only. |
| C33 | Pinned recipes and missing-material list | C | C | Crafting or recipe trees | Intended | — | Show what is missing and where it was last seen, if known. |
| C34 | Batch and queued actions (craft, buy, build) | C | M,C | Repeated transactions or production chains | Intended | — | "Make 10" / buy-max with confirmation; cancelable queue. |
| C35 | Craft or build from nearby storage | C | M,C | Materials spread across containers | Intended | — | Reduces carrying and menu trips; same costs and results. |
| C36 | Auto-equip and loadouts | C | C,M | Frequent equipment changes or stat optimisation | Intended | — | Opt-in; never equips over a locked or favourite item silently. |
| C37 | Forecasts and outcome previews | C | C | Combat odds, build or trade consequences | Intended | — | Show known facts vs predictions; never leak hidden information. |
| C38 | Focusable, persistent tooltips | B | V,M,C | Tooltips or nested definitions | On | — | Open by button as well as hover; stay open until dismissed; narratable. |
| C39 | Adaptive scaffolding without public labels | C | C,T | Learning or difficulty that adjusts to the player | Intended | — | Never displays or reports a support level to others. |

## N: Neurodivergent and sensory load

Some players need more trackers and some need fewer. Keep these independent; never infer them
from a diagnosis-named preset.

Sources: GAG [G6], [G27], [G47], [G48], [G49]; XAG [X110], [X117]; APX [APX-CC], [APX-PI]; [APPLE-LABELS].
Guide: [sensory-load-and-guidance](topics/sensory-load-and-guidance.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| N01 | Minimal / no HUD | C | N,C | Dense overlays | Intended | `hud.mode` (`minimal`, `off`) | Essential status on demand; captions unaffected. |
| N02 | HUD visibility by category | C | N,V | Cluttered HUD | Intended | `hud.mode` (`custom`), `hud.hidden_elements` | Not one all-or-nothing switch. |
| N03 | Disable guiding paint / glows / trails | C | N | Persistent guidance visuals | Intended | `guidance.paint`, `guidance.trail` (overall: `guidance.level`) | Independent of journal and hazard cues. |
| N04 | Guidance only on request | C | N,C | Visual overload | Intended | `guidance.highlights` (overall: `guidance.level`) | One remappable hint action. |
| N05 | Static menu backgrounds | B | N,C | Decorative motion | Platform pref | `motion.menu_animation` (umbrella `motion.reduce_motion`) | Freeze video and particles; keep meaning. |
| N06 | Reduce decorative detail | C | N,C | Dense non-interactive scenery | Intended | `effects.particle_density` | Keep collision, hazards and landmarks. |
| N07 | Reduce particles / hit effects | C | N,V | Effects obscure play | Intended | `effects.particle_density` | Keep another hit cue. |
| N08 | Reduce notification frequency | C | N,C | Repeated pop-ups | Intended | `notifications.mode` | Keep history and critical alerts. |
| N09 | Disable autoplay previews | B | N,C,S | Moving media in UI | Platform pref | `game.ui.autoplay` | Separate control for autoplay sound. |
| N10 | Disable UI transition animation | B | N,O | Animated UI | Platform pref | `motion.menu_animation` (umbrella `motion.reduce_motion`) | Instant or static; same hierarchy. |
| N11 | Disable decorative blinking / pulses | B | N,P | Attention effects | Safe: static critical cues | `game.ui.decorative_pulses` | Keep state through icon or text. |
| N12 | Vibration intensity / off | C | N,M | Haptic output | Intended | `game.haptics.intensity` | Visual or audio equivalents remain. |
| N13 | Haptics by event category | C | N,H,V | Useful and noisy vibration mixed | Intended | `game.haptics.categories` | Navigation cues independent of ambient rumble. |
| N14 | Adaptive-trigger resistance / off | C | M,N | Force-feedback triggers | Intended | `game.haptics.trigger_resistance` | Never require high force to proceed. |
| N15 | Quiet audio preset | C | N,C,H | Dense soundscape | Intended | — | A preset over `audio.*`; each bus stays adjustable. |

## P: Photosensitivity and flash reduction

Every row is a reduction control **on top of** baseline hazard prevention. A "normal" mode with
known unsafe sequences is never justified by offering a second mode.

Sources: GAG [G27]; XAG [X118]; [WCAG22] flash definition; [ITU] BT.1702-3; [IRIS].
Guide: [photosensitivity](topics/photosensitivity.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| P01 | Global flash-reduced presentation | B | P | Any visual output | Safe: no hazardous flashing in any mode | `motion.reduce_flashing` | Set before logos; never promise "seizure-proof". |
| P02 | Muzzle-flash suppression | C | P | Repeated weapon fire | Intended (within safe bounds) | `motion.reduce_flashing` | Keep firing and reload feedback. |
| P03 | Lightning / strobe replacement | C | P | Weather or venue strobes | Safe: static or subtle alternative | `motion.reduce_flashing` | Check repeated environmental combinations. |
| P04 | Damage / full-screen flash replacement | C | P,V | Hit feedback | Safe: non-flashing cue | `motion.reduce_flashing` | Use a steady edge or status cue. |
| P05 | Explosion / ability flash intensity | C | P | Combat VFX | Safe: bounded output | `motion.reduce_flashing` | Test overlapping players and abilities. |
| P06 | Red-flash desaturation | C | P | Saturated red changes | Safe: avoid risky sequences | `motion.reduce_flashing` | FlashGuard desaturates; test the red criterion, not luminance alone. |
| P07 | Pattern / stripe reduction | C | P | Repetitive high-contrast patterns | Safe: non-provocative design | `game.flash.pattern_reduction` | Replace the texture, not just the brightness. |
| P08 | Glitch / flicker effect disable | C | P,N | Glitch presentation | Safe: static alternative | `motion.reduce_flashing` | Menus and cutscenes too. |
| P09 | Gentle scene transitions | C | P | Bright/dark cuts | Safe: avoid rapid changes | `motion.reduce_flashing` | Test the rendered output sequence. |
| P10 | Suppress flashing player cosmetics | C | P | Player-created effects | Safe: bounded cosmetic effects | `game.flash.player_cosmetics` | Local substitution plus content limits. |

## O: Motion comfort

No single FOV or motion setting works for everyone. Keep controls independent rather than tied
to one reduce-motion preset.

Sources: GAG [G47], [G56], [G57], [G65], [G66]; XAG [X117]; APX [APX-FD].
Guides: [camera-and-motion](topics/camera-and-motion.md), [vr-and-xr](topics/vr-and-xr.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| O01 | Camera shake intensity / off | C | O,N,P | Impact shake | Safe: reduced | `motion.camera_shake` | Must reach zero, scripted scenes included. |
| O02 | Camera / head bob intensity / off | C | O | Walking oscillation | Safe: reduced | `motion.head_bob` | Cover sprint, crouch and scripted movement. |
| O03 | Weapon sway / breathing motion | C | O | First-person equipment | Safe: reduced | `motion.weapon_sway` | Do not change aim precision by accident. |
| O04 | Motion blur / off | C | O,V | Post-processing | Platform pref | `motion.motion_blur` | Per-object and full-screen blur. |
| O05 | Field of view | C | O,V | Perspective camera | Intended (display-appropriate) | `motion.fov` | Label horizontal or vertical and aspect handling. |
| O06 | Camera distance | C | O,V | Third-person view | Intended | `game.camera.distance` | Keep collision and objective visibility. |
| O07 | Camera horizontal / vertical speed | C | O,M | Camera steering | Intended | `input.look_sensitivity` | Separate from movement sensitivity where useful. |
| O08 | Camera smoothing / lag off | C | O,M | Delayed camera response | Intended | `game.camera.smoothing` | Keep response predictable. |
| O09 | Automatic recentring off | C | O,M | Camera recentres itself | Intended | `motion.auto_camera` (umbrella `motion.reduce_motion`) | Manual recentre stays available. |
| O10 | Disable scripted camera pulls | C | O,V | Game takes the camera | Intended | `game.camera.scripted_pulls` | Point the way with another cue. |
| O11 | Disable automatic camera rotation | C | O | Turns without player input | Safe: reduced | `motion.auto_camera` (umbrella `motion.reduce_motion`) | Voluntary turning stays. |
| O12 | Stable horizon / reduced roll | C | O | Vehicle or world tilt | Intended | `game.camera.horizon_lock` | Keep essential orientation information. |
| O13 | Disable sprint / aim dynamic FOV | C | O | Zooming perspective | Intended | `motion.fov_effects` (umbrella `motion.reduce_motion`) | Separate aim magnification if needed. |
| O14 | Disable speed lines / radial blur | C | O,N | Simulated speed effects | Intended | `motion.speed_lines` (umbrella `motion.reduce_motion`) | Show speed another way. |
| O15 | Disable chromatic aberration / distortion | C | O,V | Lens effects | Intended | `motion.chromatic_aberration`, `motion.screen_effects` (distortion) | Damage and cutscenes too. |
| O16 | Stable screen-centre marker | C | O | Moving perspective | Intended | `game.camera.center_dot` | Separate from the weapon reticle. |
| O17 | Reduce parallax / background scrolling | C | O,N | Layered movement | Platform pref | `motion.parallax` (umbrella `motion.reduce_motion`) | Keep foreground navigation movement. |
| O18 | VR teleport or smooth movement | C | O,M | Artificial locomotion | Intended (comfort-oriented) | `game.xr.locomotion` | Same destinations and progression. |
| O19 | VR snap turning / angle | C | O,M | Artificial rotation | Intended (comfort-oriented) | `game.xr.turn_mode` | Smooth turning remains available. |
| O20 | VR comfort vignette | C | O | Artificial movement | Intended | `game.xr.vignette` | Adjustable strength; never hides essential cues. |
| O21 | Depth-of-field off | C | V,O | Focus / defocus effects | Intended | `motion.depth_of_field` | Essential objects stay sharp. |

## F: Phobia and content sensitivity

Replacements beyond spiders and gore are proposed applications of content-customisation
principles. No option promises to remove an individual's fear. Name options by what changes.

Sources: GAG [G51], [G52]; XAG [X123]; APX [APX-MO], [APX-BY]; example [E-GROUNDED].
Guide: [content-and-phobias](topics/content-and-phobias.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| F01 | Spider appearance substitution | C | F | Spider-like entities | Intended; offered before first exposure | `content.spiders` | Icons, trophies, corpses and thumbnails too. |
| F02 | Other creature substitutions | C | F | Snakes, insects, similar | Intended | `game.content.<category>` | Replacement keeps a readable threat role. |
| F03 | Reduce creature sounds / animation | C | F | Replacement still moves or sounds alike | Intended | `content.spiders` | Visuals alone may not remove the barrier. |
| F04 | Deep-water visibility / traversal assist | C | F,T | Underwater dread | Intended | `game.content.deep_water` | State the visibility, breath and enemy changes. |
| F05 | Heights / edge presentation | A | F,O | Vertigo-inducing views | Intended | `game.content.heights` | Alternate route or view where workable. |
| F06 | Blood amount / off | C | F,N | Blood effects | Intended | `content.blood` | Keep hit confirmation. |
| F07 | Gore / dismemberment off | C | F,T | Graphic injury | Intended | `content.gore` | Environment, cinematics and inventory art. |
| F08 | Nudity filter | C | F,S | Sensitive imagery | Intended | `game.content.nudity` | Same progression and scene meaning. |
| F09 | Profanity filter | C | F,S | Spoken or written language | Intended | `game.content.profanity` | Captions and audio agree. |
| F10 | Needle / medical imagery replacement | C | F,T | Medical procedures | Intended | `game.content.needles` | Set before the scene; no exposed preview. |
| F11 | Jump-scare reduction / warning | C | F,T,N | Sudden scares | Intended | `content.jump_scares`, `content.scare_warnings` | Reduction and warning are separate choices; independent of threat difficulty. |
| F12 | Detailed content notices | C | F,T | Sensitive themes | On (before purchase and start) | `content.warnings` | Layer summary and detail so players pick spoiler depth. |
| F13 | Opt-in pre-scene warnings | C | F,T | A specific upcoming trigger | Intended (or chosen at setup) | `content.warnings`, `content.scare_warnings` | Warnings come before the content and are accessible. |
| F14 | Sensitive scene / mission skip | C | F,T | Unavoidable triggering sequence | Intended | `content.scene_skip` | Keep rewards and progress; optional factual recap. |
| F15 | Clustered-hole (trypophobia) texture alternatives | C | F,N | Organic hole patterns, nests, pores, honeycomb surfaces | Intended | game.content_holes | Swap textures, normal maps and decals; check walls and floors, not just creatures. |

## D: Difficulty, assists and stress

Every row keeps achievement, trophy, content and ending parity (core-policy section 2). Presets
may set several values, but each barrier stays independently adjustable.

Sources: GAG [G5], [G25], [G99], [G100], [G102], [G103], [G104], [G105], [G107]; XAG [X108]; APX [APX-HH], [APX-SD], [APX-BY], [APX-SV].
Guides: [difficulty-and-assists](topics/difficulty-and-assists.md), [timing-and-speed](topics/timing-and-speed.md), [combat-aiming-stealth](topics/combat-aiming-stealth.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| D01 | Described difficulty presets | B | T,C,M | Any challenge | Intended | `game.difficulty.preset` | State mechanical effects; presets are editable patches. |
| D02 | Change difficulty without restart | B | T,C,M | Adjustable challenge | On | — | Keep progress and content. |
| D03 | Independent mechanic difficulty | C | M,C,T | Several challenge types | Intended | — | Split combat, puzzles and traversal (keys D04-D33). |
| D04 | Invulnerability | C | M,T | Damage | Intended | `assist.damage_taken` | 0%; define recovery for pits, grabs and scripts. |
| D05 | Incoming damage scale | C | M,T | Combat, hazards | Intended | `assist.damage_taken` | Independent of outgoing damage. |
| D06 | Outgoing damage / enemy health | C | M,T | Sustained combat | Intended | `game.assist.damage_dealt` | Avoid endurance-only tuning. |
| D07 | Separate fall assist | C | M,T | Platforming, pits | Intended | `assist.no_fall_death` | Distinguish damage from respawn position. |
| D08 | Independent fail-state toggles | C | T,M,C | Several failure causes | Intended | `assist.no_fall_death` | Combat, drowning, timers, pits separate (`game.assist.no_<reason>`). |
| D09 | Enemy aggression / detection | C | T,M,C | Stealth, combat | Intended | `assist.enemy_aggression` | Explain the behaviour change. |
| D10 | Passive enemies | C | T,F | Combat blocks exploration | Intended | `assist.enemy_aggression` | 0%; quest and loot triggers still fire. |
| D11 | Infinite ammunition | C | M,T,C | Ammo pressure | Intended | `assist.infinite_ammo` | Cover reload and special ammo. |
| D12 | Infinite stamina | C | M,T | Exertion resource | Intended | `assist.infinite_stamina` | Manual stop stays possible. |
| D13 | Resource / crafting cost reduction | C | C,T,M | Scarcity, grind | Intended | `game.assist.resource_cost` | Reversible; progression intact. |
| D14 | Game speed | C | M,C | Real-time demands | Intended | `assist.game_speed` | Menus, captions and narration unaffected. |
| D15 | On-demand slow motion | C | M,C | Short reaction bursts | Intended | `game.assist.slow_motion` | Remappable; toggle option. |
| D16 | Extended reaction / parry timing | C | M,C | Precise windows | Intended | `input.timing_window_scale` | Show success through several channels. |
| D17 | Aim assist strength | C | M,V | Precision aiming | Intended | `input.aim_assist` (slowdown), `input.aim_pull`, `input.aim_stabilization` | Separate slowdown, pull and stabilisation; range and strength; state multiplayer rules. |
| D18 | Target lock / automatic aim | C | M,V | Persistent tracking | Intended | `input.target_lock` | Player can select and release targets. |
| D19 | Steering / braking assist | C | M,C | Continuous vehicle control | Intended | `game.assist.steering` | Independent assists, not a bundle (see `game.assist.braking`). |
| D20 | Jump / landing assist | C | M,V | Precision traversal | Intended | `game.assist.landing` | Keep the player's destination choice. |
| D21 | Extra / infinite jumps | C | M,T | Repeated traversal failure | Intended | `game.assist.extra_jumps` | Keep recovery from unintended spaces. |
| D22 | Puzzle difficulty / bypass | C | C,T | Puzzle blocker | Intended | `assist.skip_available` | Skip keeps story and rewards. |
| D23 | Room / encounter skip | C | M,T,C | Persistent blocker | Intended | `assist.skip_available` | Always available directly; a post-failure offer is extra. |
| D24 | Progress-preserving retry / checkpoint | C | M,T | Repeated replay | Intended | — | Never redo long completed sections. |
| D25 | Manual save / quick save | C | M,C,S | Sessions need interrupting | On | — | Independent slots; avoid guaranteed-death saves. |
| D26 | Autosave frequency / history | C | C,M,T | Progress risk | On | `game.save.autosave_interval` | Never overwrite every recoverable state. |
| D27 | Pause gameplay and cinematics | B | M,C,H,S | Local play | On | — | Pause simulation and presentation; explain online limits. |
| D28 | No-loss death / reduced penalties | C | T,C,M | Costly failure | Intended | `game.assist.death_penalty` | Separate currency, inventory and time penalties. |
| D29 | Disable countdowns / more turn time | C | C,M,T | Time pressure | Intended | `assist.timers` | Separate gameplay timers from UI timeouts. |
| D30 | Break reminders / session stop | C | M,T,S | Long sessions | Intended | `game.system.break_reminder` | Optional; stopping never loses progress. |
| D31 | Random encounter rate / off (story battles kept) | C | M,C,T,S | Random encounters or turn-based battles | Intended | `assist.encounter_rate` | 0% stops random battles only; required battles, rewards and XP stay reachable. |
| D32 | Optional auto-battle (player can take control any time) | C | M,C,T,S | Random encounters or turn-based battles | Intended | `assist.auto_battle` | One action hands control back instantly; same rewards and flags. |
| D33 | Battle animation speed (decision time unaffected) | C | M,C,T,S | Random encounters or turn-based battles | Intended | `assist.battle_speed` | Speeds animations only; never shortens decision or input windows. |

## K: Communication and multiplayer

Speech-to-text, outgoing typed speech, incoming text narration and real-time text are four
different capabilities. Assess each separately.

Sources: GAG [G37], [G54], [G82], [G87], [G88], [G89], [G101], [G108]; XAG [X116], [X119], [X120]; APX [APX-FT], [APX-PA], [APX-HR]; law [EAA], [EECC].
Guides: [multiplayer-and-communication](topics/multiplayer-and-communication.md), [store-claims-and-compliance](topics/store-claims-and-compliance.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| K01 | Free-form text chat | C | H,speech,M | Player communication | On | — | Participation never needs voice. |
| K02 | Voice alongside text | C | V,C,M | Typing barrier | Intended | — | A route for players unable to type. |
| K03 | Incoming voice transcription | C | H | Voice chat | Platform pref | `game.chat.transcribe_incoming` | Name the speaker; disclose unsupported languages and failures. |
| K04 | Outgoing text-to-speech | C | speech,M | Voice channels | Platform pref | `game.chat.speak_outgoing` | Speak only submitted text, never drafts. |
| K05 | Canned text phrases | C | M,C,speech | Typing burden | Intended | — | Does not replace free-form communication. |
| K06 | Symbol communication | C | C,speech,H | Quick intent | Intended | — | Label symbols and narrate their meaning. |
| K07 | Spatial pings / markers | C | H,speech,C | Team coordination | Intended | — | Text or audio redundancy; adjustable rate limits. |
| K08 | Custom quick-chat phrases | C | M,C,speech | Recurring messages | Intended | — | Accessible editor; safe text handling. |
| K09 | Real-time text | C | H,speech | Possibly regulated chat service | Intended; a legal duty may apply | `game.chat.rtt` | Sends as typed; ordinary send-button chat is different. |
| K10 | Chat text scale / contrast | C | V,C | Chat text | Platform pref | `game.chat.text_scale` | Input, messages, history and transcripts. |
| K11 | Chat display duration / pinning | C | C,V,H | Messages expire | Platform pref | `game.chat.message_duration` | Persistent history prevents missed messages. |
| K12 | Scrollback / message history | C | C,H,V | Fast conversation | On | — | Keep speaker and order; AGI asks for at least 4 messages. |
| K13 | Active-speaker identification | C | H,C | Several live speakers | Intended | `game.chat.speaker_indicator` | Never by voice colour alone. |
| K14 | Push-to-talk / toggle / open mic | C | M,T,S | Microphone activation | Intended (privacy-preserving) | `game.chat.mic_mode` | Remappable; clear mic status. |
| K15 | Per-player volume / mute | C | H,N,T | Loud or abusive players | Intended | — | Accessible list; safety UI never muted. |
| K16 | Incoming message / TTS filtering | C | N,T | Spam or harassment | Intended | `game.chat.filter` | Never disables the player's own communication. |
| K17 | Accessible block / report | C | T,M,V | Online social play | On | — | Keep evidence without forced replay. |
| K18 | No-voice-required matchmaking | C | H,speech,T | Team play | Intended | `game.matchmaking.no_voice` | A social preference; no disability disclosure. |
| K19 | Private / local co-op assistance | C | M,C,T | Shared play possible | Intended | — | Help without replacing the player's agency. |
| K20 | Per-player settings in local co-op | C | V,H,M,N | Shared display or audio | Intended | — | Per-player profiles; document shared-channel conflicts. |
| K21 | Flexible ready-check / lobby deadlines | C | M,C | Timed lobby entry | Intended | — | Available before the ready check starts. |
| K22 | Accessible communication setup | B | V,H,M,C | Any chat service | On | — | Usable before the first conversation. |
| K23 | Agreed rules matchmaking preferences | C | M,C,T | Competitive multiplayer | Intended | `game.matchmaking.ruleset` | Describe shared rules; no assist or diagnosis disclosure; never automatic segregation. |

## L: Platform, hardware, mods and settings infrastructure

Mod support extends access. It never moves responsibility for baseline access to volunteer
modders.

Sources: GAG [G17], [G22], [G23], [G26], [G95], [G96], [G97], [G98], [G109]; XAG [X108], [X112], [X121], [X122]; APX [APX-FC], [APX-LT], [APX-PA]; [APPLE-HIG]; [E-EYEMINE].
Guide: [mods-hardware-and-platforms](topics/mods-hardware-and-platforms.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| L01 | Accessibility setup before first content | B | All | First launch | On | — | No logo, music or cinematic before settings are reachable. |
| L02 | Persist accessibility preferences | B | All | Repeat launches | On | — | Stored outside story saves; never reset on death or load. |
| L03 | Named settings profiles | C | All | Shared device, changing needs | Intended | `game.profile.active` | Never name profiles after a diagnosis. |
| L04 | Read platform preferences | C | V,H,N,M | Supported OS settings | Platform pref | — | Every registry key with `platform_pref`; explicit player value wins. |
| L05 | Multiple supported input devices | C | M,S | Alternative hardware | On | — | Every supported device reaches menus and play. |
| L06 | Simultaneous mixed-device input | C | M | Keyboard plus adaptive controller | Intended | — | Never reject alternate input; prompts do not flicker. |
| L07 | Controller assist / combined controllers | C | M | Shared or split input | Intended | `game.input.copilot` | Test the platform feature; not automatic from controller support. |
| L08 | Windowed / borderless modes | C | M,V | Desktop assistive overlays | Intended | `game.display.window_mode` | Virtual keyboard and screen reader stay usable. |
| L09 | System virtual-keyboard compatibility | C | M,V | Text entry | On | — | Composition, focus, submit and cancel work. |
| L10 | Switch scanning input | A | M | One or few switches | Intended | `game.input.switch_scan` | Adjustable pace, grouping and selection. |
| L11 | Gaze / dwell input | A | M | Pointer alternative | Intended | `game.input.dwell_time` | Adjustable dwell; no selection while reading. |
| L12 | Portrait / landscape choice | C | M,S | Mounted mobile device | Platform pref | `game.display.orientation` | Reflow controls; rotation never required. |
| L13 | External / mobile controllers | C | M | Touch is difficult | Intended | — | Setup screens and menus too, not only gameplay. |
| L14 | Accessibility mod hooks | A | All | Extensible desktop game | Intended | — | Document interfaces and compatibility limits. |
| L15 | Mod / config management and recovery | A | All | Third-party extensions | Intended | — | Easy disable and rollback. |
| L16 | In-game accessibility feature guide | B | All | Discovering options | On | — | Effects, limits and current bindings. |
| L17 | Public accessibility documentation | B | All | Purchase decisions | On | — | Per platform, language and version; keep store claims current. |
| L18 | Accessible support channels | B | All | Help or feedback | On | — | No extra cost; never voice-only. |
| L19 | Stable frame rate / refresh choices | C | O,V,S | Variable presentation | Safe: VSync or frame cap on | `game.display.vsync` | Also `game.display.frame_cap`; not photosensitivity certification. |
| L20 | Pause on focus loss | C | M,S,T | Desktop interruptions | On (registry default) | `system.pause_on_focus_loss` | Safe resume; explain multiplayer limits. |
| L21 | Adult/teacher configuration that never locks a child's own access settings | C | All | Parent, teacher or admin settings exist | On | — | Admin limits content or time; personal access settings stay editable. |

## R: Screen reader, TTS, audio access and voice input

Menu narration is Baseline. Narrated menus are not "playable without sight"; full nonvisual
gameplay is a separate, larger claim.

Sources: GAG [G46], [G53], [G67], [G69], [G70], [G72], [G73], [G74], [G92], [G93], [G94]; XAG [X106], [X111], [X113], [X114]; [ANDROID], [APPLE-HIG].
Guide: [screen-reader-and-voice](topics/screen-reader-and-voice.md).

| ID | Option | Tier | Helps | Signal | Default | Key | Note |
|---|---|---|---|---|---|---|---|
| R01 | Menu narration / screen-reader support | B | V,C | Any text UI | Platform pref | `narration.enabled` | All menus, dialogs and setup; no mouse-only path. |
| R02 | Read HUD / status on demand | C | V,C | Changing player state | Intended | `game.narration.status_action` | Remappable, prioritised request. |
| R03 | Automatic important-status narration | C | V | Time-critical state | Intended | `game.narration.auto_status` | Higher priority than optional readouts. |
| R04 | Incoming text-chat narration | C | V | Text communication | Platform pref | `game.chat.narrate_incoming` | Local reading differs from broadcasting TTS. |
| R05 | Narration speed | C | V,C,H | Spoken interface | Platform pref | `narration.rate` | Interruptible; persists. |
| R06 | Narration voice / pitch | C | H,V | Voice intelligibility | Platform pref | `game.narration.voice` | Test localised voices. |
| R07 | Narration verbosity | C | V,C | Many role or state details | Intended | `game.narration.verbosity` | Never drop essential state changes. |
| R08 | Repeat / cancel narration | C | V,C | Missed or queued speech | On | — | Immediate on every supported input. |
| R09 | Narration language | C | V,C | Multilingual game | Platform pref | `game.narration.language` | Announce language changes; pronunciation data. |
| R10 | Descriptions for meaningful images | B | V | Icons, charts, diagrams | On | — | Describe purpose, not a filename. |
| R11 | Spoken context and focus changes | B | V,C | Screen transitions | On | `narration.enabled` | Announce changes; stop stale speech. |
| R12 | Text / document reading view | C | V,C | Signs, books, notes | Intended | `narration.dialogue` | Structured navigation and readable transcript. |
| R13 | Cinematic audio description | A | V,C | Visual storytelling | Intended | `game.narration.audio_description` | Localise; never mask dialogue. |
| R14 | Extended audio description | A | V,C | Too few dialogue gaps | Intended | `game.narration.audio_description` | Pause compatible scenes while describing. |
| R15 | Distinct gameplay audio cues | C | V | Visual-only events | Intended | — | Teachable, independently balanced cues. |
| R16 | Spoken turn-by-turn navigation | A | V,C | Spatial navigation | Intended | `game.narration.navigation` | Reroute and report blocked paths. |
| R17 | Compass-aligned movement / orientation | C | V,M | Spatial aiming | Intended | `game.input.compass_snap` | Remappable snap; confirm direction. |
| R18 | On-demand sonar / audio scene scan | A | V | Finding objects | Intended | `game.narration.scan_action` | Distinct categories without speech overload. |
| R19 | OS voice-control compatibility | C | M | Voice used for UI navigation | On (compatible semantics) | — | Native labels and actions; speech never required. |
| R20 | Short voice-command vocabulary | C | M,speech | Optional voice recognition | Intended | `game.input.voice_commands` | Short words; other inputs remain. |
| R21 | Dictation for text fields | C | M,C | Typing is difficult | Intended | — | Review and edit before sending. |
| R22 | Nonverbal sound activation | A | M,speech | Word recognition unusable | Intended | `game.input.sound_threshold` | Calibrate the threshold; keep a switch alternative. |

## Numeric thresholds

Numbers keep their source and scope. Rendered pixels, CSS px, Android dp, Apple points,
millimetres and visual angle are different units. Engine font-size fields often do not equal
visible glyph height: measure the final output. These are different criteria, not
interchangeable measurements.

### Text, contrast and targets

| Source and scope | Number | Note |
|---|---|---|
| [X101] console text | Default at least 26 px at 1080p, 52 px at 4K | Visible height, ascender to descender. |
| [X101] PC / VR text | At least 18 px at 1080p, 36 px at 4K | Not interchangeable with AGI's PC criterion. |
| [X101] scaling | At least 200% of the minimum default | Content and operation preserved. |
| [X101] long text | At most 80 characters per line (40 CJK) | Blocks longer than two lines. |
| [AGI] Clear Text | Default 17 / 26 / 52 px at 720p / 1080p / 4K | PC and console. |
| [AGI] Large Text | Available 25 / 38 / 76 px at 720p / 1080p / 4K | Default or selectable. |
| [AGI] Large & Clear Subtitles | Default 32 px at 1080p; PC scaling to at least 46 px | Other resolutions scale equivalently. |
| [X102] contrast | 4.5:1 standard text and important elements; 3:1 large; 7:1 high-contrast mode | AGI and Microsoft's copied text print "4:5:1"; [XTAGS] test steps use 4.5:1. |
| [WCAG-CONTRAST] SC 1.4.3 / 1.4.6 | AA 4.5:1 (3:1 large); AAA 7:1 (4.5:1 large) | Large = 18 pt regular or 14 pt bold. |
| [WCAG-NONTEXT] SC 1.4.11 | 3:1 | Control parts, states and meaningful graphics. |
| [WCAG-SPACING] SC 1.4.12 | Line 1.5×, paragraph 2×, letters 0.12×, words 0.16× font size | Content must tolerate these overrides; not a forced default. |
| [WCAG-TARGET] SC 2.5.8 / 2.5.5 | 24 × 24 CSS px (AA); 44 × 44 CSS px (AAA) | Exceptions apply. |
| [ANDROID-DESIGN] touch | 48 × 48 dp | Touchable area; icon can be smaller. |
| [X107] touch | Phone 15 mm; tablet 24 mm | 59 / 118 / 236 px and 94 / 189 / 378 px at 100 / 200 / 400 DPI. |
| [X107] analog sensitivity | At least 50% below and 50% above default | Each relevant analog control. |
| [G24] debounce | 0.5 s | A suggested option, never a forced delay. |

Contrast uses `(L_lighter + 0.05) / (L_darker + 0.05)` on linearised sRGB relative luminance.
Do not round 4.499 up to 4.5. Check the worst background behind moving text. Sources:
[WCAG-CONTRAST], [X102].

### Subtitles and timing

| Source | Number | Note |
|---|---|---|
| [X104] | At most about 40 characters per line; normally at most 2 lines | 3 lines only in exceptional cases. |
| [X104] | Size scalable to at least 200%; background opacity 0-100% | Use X101 minimums. |
| [X104] | Repeat speaker name after a change or a 1-2 minute pause | A sole narrator is an exception. |
| [BBC] | 160-180 words per minute | Broadcast editorial target; slower may be needed. |
| [HAMILTON-SUB] | About 38 characters per line; about 0.25 s between subtitles | 2015 game practice advice, not a platform rule. |
| [X116] UI deadlines | Adjustable to at least 10× default, or disable | Applies to UI limits outside core mechanics. |
| [X116] deadline extension | Warning at least 20 s ahead; simple extend action; at least 10 extensions | Alternative to adjustable limits. |
| [X105] audio pause | Events shorter than 3 s need no pause control | Live multiplayer is exempt from pausing the session. |
| [X106] recurring spoken state | 7-10 s is an acceptable interval | Do not interrupt more important speech. |
| [AGI] chat history | At least the last 4 messages available | Displayed, scrollable or expandable. |

### Flashing and patterns

| Source | Rule |
|---|---|
| [WCAG22] SC 2.3.1 (A) | Pass if no more than 3 general and no more than 3 red flashes in **any** 1-second period (sliding window), or the flashing stays below the thresholds. |
| [WCAG22] area | Combined flashing area at most 0.006 steradians within any 10° visual field (about 25% of that field at typical viewing distance). This is **not** 25% of the screen. |
| [WCAG22] general flash | A pair of opposing luminance changes of at least 10% of maximum relative luminance, where the darker state is below 0.80. |
| [WCAG22] red flash | Opposing transitions involving saturated red: R/(R+G+B) ≥ 0.8 and a chromaticity difference > 0.2 in CIE 1976 UCS. |
| [WCAG-THREE] SC 2.3.2 (AAA) | No more than 3 flashes in any second, with no area or intensity exception. |
| [X118] older red formula | R/(R+G+B) ≥ 0.8 and a change > 20 in `(R−G−B) × 320`. Do not label this as the WCAG 2.2 definition. |
| [ITU] BT.1702-3 (Nov 2023) | Flash if luminance difference ≥ 20 cd/m² where the darker state is below 160 cd/m²; for HDR above 160 cd/m², Michelson contrast > 1/17. Saturated red can harm regardless of luminance. |
| [ITU] prohibited sequence | Flashing area > 25% of the displayed screen **and** more than 3 flashes in any second. Sequences over 5 s may stay risky even within limits. |
| [ITU] patterns | More than 5 light/dark stripe pairs, plus area > 40% if stationary or > 25% if moving, oscillating, flashing or reversing. |
| [IRIS] (EA, BSD-3-Clause) | Flags more than 3 flashes/s in any second; 2-3 flashes/s for 5 consecutive seconds as an extended failure; harmful patterns lasting ≥ 0.5 s. Not a certificate. |

Skill rule (stricter than a single pass): the game has **no hazardous flashing in any mode**;
FlashGuard allows at most 3 large or bright flashes in any 1-second window, always. A warning
screen is not a mitigation. "Passes Harding" needs a named tool, version, profile and tested
footage. See [photosensitivity](topics/photosensitivity.md).

## Source links

GAG pages ([full list](https://gameaccessibilityguidelines.com/full-list/)):

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
[G61]: https://gameaccessibilityguidelines.com/provide-a-choice-of-cursor-crosshair-colours-designs/
[G62]: https://gameaccessibilityguidelines.com/provide-an-option-to-adjust-contrast/
[G64]: https://gameaccessibilityguidelines.com/use-surround-sound/
[G65]: https://gameaccessibilityguidelines.com/avoid-or-provide-option-to-disable-any-difference-between-controller-movement-and-camera-movement/
[G66]: https://gameaccessibilityguidelines.com/if-the-game-uses-field-of-view-3d-engine-only-allow-a-means-for-it-to-be-adjusted/
[G67]: https://gameaccessibilityguidelines.com/provide-an-audio-description-track/
[G68]: https://gameaccessibilityguidelines.com/simulate-binaural-recording/
[G69]: https://gameaccessibilityguidelines.com/use-distinct-sound-music-design-for-all-objects-and-events/
[G70]: https://gameaccessibilityguidelines.com/ensure-screenreader-support-including-menus-installers/
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
[G107]: https://gameaccessibilityguidelines.com/allow-difficulty-level-to-be-altered-during-gameplay-either-through-settings-or-adaptive-difficulty/
[G108]: https://gameaccessibilityguidelines.com/realtime-text-speech-transcription/
[G109]: https://gameaccessibilityguidelines.com/allow-settings-to-be-saved-to-different-profiles-at-either-game-or-platform-level/

Xbox Accessibility Guidelines ([overview](https://learn.microsoft.com/en-us/xbox/accessibility/guidelines), v3.2):

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

AbleGamers APX ([overview](https://accessible.games/accessible-player-experiences/)):

[APX-FC]: https://accessible.games/accessible-player-experiences/access-patterns/flexible-controllers/
[APX-SC]: https://accessible.games/accessible-player-experiences/access-patterns/same-controls-but-different/
[APX-IP]: https://accessible.games/accessible-player-experiences/access-patterns/improved-precision/
[APX-DML]: https://accessible.games/accessible-player-experiences/access-patterns/do-more-with-less/
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

Stores, platforms, standards, law and examples:

[AGI]: https://accessiblegames.com/wp-content/uploads/2025/03/Accessible-Games-Initiative-Tags-and-Criteria-March-2025.pdf
[XTAGS]: https://learn.microsoft.com/en-us/xbox/accessibility/accessibility-feature-tags
[STEAM]: https://partner.steamgames.com/doc/accessibility_features
[APPLE-HIG]: https://developer.apple.com/design/human-interface-guidelines/accessibility
[APPLE-LABELS]: https://developer.apple.com/help/app-store-connect/manage-app-accessibility/overview-of-accessibility-nutrition-labels/
[ANDROID]: https://developer.android.com/guide/topics/ui/accessibility/principles
[ANDROID-DESIGN]: https://developer.android.com/design/ui/mobile/guides/foundations/accessibility
[WCAG-CONTRAST]: https://www.w3.org/WAI/WCAG22/Understanding/contrast-minimum.html
[WCAG-NONTEXT]: https://www.w3.org/WAI/WCAG22/Understanding/non-text-contrast.html
[WCAG-SPACING]: https://www.w3.org/WAI/WCAG22/Understanding/text-spacing.html
[WCAG-TARGET]: https://www.w3.org/WAI/WCAG22/Understanding/target-size-minimum.html
[WCAG-THREE]: https://www.w3.org/WAI/WCAG21/Understanding/three-flashes
[WCAG22]: https://www.w3.org/TR/WCAG22/#dfn-general-flash-and-red-flash-thresholds
[BBC]: https://www.bbc.co.uk/accessibility/forproducts/guides/subtitles/
[HAMILTON-SUB]: https://ian-hamilton.com/how-to-do-subtitles-well-basics-and-good-practices/
[ITU]: https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.1702-3-202311-I%21%21PDF-E.pdf
[IRIS]: https://github.com/electronicarts/IRIS
[EAA]: https://eur-lex.europa.eu/eli/dir/2019/882/oj/eng
[EECC]: https://eur-lex.europa.eu/eli/dir/2018/1972/oj/eng
[E-GROUNDED]: https://grounded.obsidian.net/news/grounded/accessibility-features
[E-EYEMINE]: https://github.com/SpecialEffect/EyeMine
