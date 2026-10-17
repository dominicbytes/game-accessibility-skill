# Signal routing (load every time)

A **signal** is something the game actually contains: a mechanic, a modality or a type of content.
The router turns signals into options (IDs from [option-catalog.md](option-catalog.md)) and into
the topic guides to load. Topic guides live in `references/topics/<name>.md`; the Guide column
gives `<name>`. Setting keys for the most common options are in
[settings-registry.json](../assets/settings-registry.json).

Tiers: **B** Baseline (assess in every game), **C** Contextual (add when the signal is present),
**A** Advanced (record with an explicit scope decision; never drop silently). ID ranges such as
`M13–M17` are inclusive. New signals take the next free number after the last row. Never renumber.

## 1. How to use the router

| State | Meaning | Action |
| --- | --- | --- |
| `present` | The developer says the player must do or experience it, or you inspected reachable content that has it. In Build mode, a planned or in-scope feature counts as present for planning. | Load the guide. Plan options, owner and verification. Planned features get their options and hooks in the same milestone as the feature. |
| `suspected` | A name, asset, dependency or half-built feature hints at it. | Trace usage and call sites. If still unclear, ask one targeted question. Keep the route open. |
| `absent` | The developer confirms it, or a complete, scoped inspection shows it is not there. | Skip the guide. Record the scope and evidence in `ACCESSIBILITY.md`. |
| `unknown` | Evidence is missing or conflicting. | Never treat as absent. Continue other work. Ask only when it blocks the next decision. |
| `removed` | Deleted, or cut from scope. | Skip the guide. Record the evidence. Revisit when scope changes. |
| `planned` | Designed or scheduled but not in the current executable. | In Build mode, treat as `present` for planning. Exclude it only from claims about the current executable (audit results, store notes). |

- **One mandatory occurrence counts.** A single timed fishing minigame, one flashing cutscene, one
  vehicle scene or one touch gesture gets its own route, scoped to where it occurs.
- **Genre seeds the questions; signals decide.** Use [genre-recipes.md](genre-recipes.md) for the
  first scan only. A mixed game takes the union of its confirmed signals.
- **Evaluate per mode, scene and mechanic,** then take the union across every supported path:
  campaign, PvP, menus, tutorials, minigames, cinematics, DLC. Record the scope (`chapter_3/escape`).
- **File presence is not reachability.** A bundled engine demo can contain weapons the game never
  uses. A packed video can flash without any matching script name.
- Record evidence (file, scene, design-doc section, developer answer), never guesses about a
  player's disability.

## 2. Baseline: assess in every game

Assessing means checking. A silent game needs no subtitle menu, but you confirm it has no speech.
"Not an action game" never skips safety or input assessment.

| ID | Assess | Options | Guide |
| --- | --- | --- | --- |
| B01 | First launch, discovery, persistence, reset. Settings before logos, music and speech. OS preferences as initial values. Preferences stored outside campaign saves. Reset per category. | B: L01, L02, L04, C27, C30, H01 | screen-reader-and-voice, mods-hardware-and-platforms |
| B02 | Operable input and UI. Logical actions only. Remapping covers menus and minigames. Remap-aware prompts. Stable focus. No pointer-only path. Pause. | B: M01, M41, M42, V32, D27; C: M21, L20 | input-and-motor |
| B03 | Readable, semantic information. Legible default size and contrast. Scaling without clipping. Plain font. Names, roles, values and focus for narration. | B: V01, V03, V11, V12, V14, V16, R01, R10, R11 | text-and-readability, screen-reader-and-voice |
| B04 | Essential state in more than one channel. No colour-only and no sound-only requirement. | B: V25; C: H19, H34 when sound carries meaning | color-and-clarity, audio-and-captions |
| B05 | Safe presentation. No hazardous flashing in any mode, including logos, menus, transitions and video. A warning is not a mitigation. Quiet first audio. Frame cap. | B: P01, N05, N09, N10, N11, H01; C: L19; O01 at "Reduced" where shake exists | photosensitivity, camera-and-motion |
| B06 | Respectful options and truthful documentation. Described difficulty, changeable mid-game. Confirmations. No achievement, save or content penalty. In-game guide and public notes. | B: D01, D02, D27, C02, C29, L16, L17, L18 | difficulty-and-assists, store-claims-and-compliance |

## 3. Signal table

### 3.1 Modalities, essential information and entry points

| Signal | Look for | Options | Guide |
| --- | --- | --- | --- |
| S001 Any visual output | Rendered menu, logo, cinematic, board, world, loading screen | B: P01, V25; C: V01, V02 | photosensitivity, text-and-readability |
| S002 Settings before gameplay | Boot flow, first-launch screen, preferences file | B: L01, L02, L04, C30, R01 | screen-reader-and-voice, mods-hardware-and-platforms |
| S003 Text or symbols carry meaning | Labels, icons, numbers, cards, world signs | B: V01, V03, V16, R10; C: V02, V13 | text-and-readability |
| S004 Colour conveys state | Team colours, rarity, key/lock pairs, damage types, good/bad | B: V25; C: V23, V24 (filters extra only) | color-and-clarity |
| S005 Shape, position or animation alone conveys state | Poses, board patterns, gesture clues | B: V25, R10; C: H34, R15, R17; A: R16, R18 | color-and-clarity, screen-reader-and-voice |
| S006 Gameplay-significant audio | Alerts, timer ticks, enemy cues, whistles, music clues | B: H01; C: H02, H04, H05, H19, H22, H34, H35 | audio-and-captions |
| S007 No audio at all | Silent novel, board or puzzle | Skip H02–H37 (confirm no speech in any mode); C: R01, R12 if text | screen-reader-and-voice |
| S008 Haptic information or force | Rumble calls, adaptive triggers, force-feedback wheel | C: N12, N13, N14; visual or audio equivalent | input-and-motor |
| S009 Speech, breath, pitch or mic input | Sing, shout, blow, voice spells, speech recognition | C: M23 (local button, menu or text alternative through logical actions), R20 (optional voice vocabulary); A: R22 (nonverbal activation scope decision). K01 only with a communication signal (S123, S124) | screen-reader-and-voice, input-and-motor |
| S010 Cinematic or nonverbal storytelling | Facial cues, silent action, montage | C: H19, C13, D27; A: R13, R14 | audio-and-captions, screen-reader-and-voice |
| S011 Account, login, launcher or lobby gate | Authentication, character creation, server browser | B: R01, R11, M21, C23; C: K22 | multiplayer-and-communication, screen-reader-and-voice |
| S012 Compulsory tutorial or onboarding | Interactive trial, demonstration, mandatory setup | B: L01; C: C04, C05, C07, C24, C25 | memory-and-navigation |

### 3.2 Input effort, precision and timing

| Signal | Look for | Options | Guide |
| --- | --- | --- | --- |
| S013 Timed inputs | Success window, perfect hit, reaction prompt, combo cancel | C: D14, D15, D16, M39, D29, C06 | timing-and-speed |
| S014 Quick-time events | Prompt during animation, branch on a quick press | C: M16, M17, D27 | input-and-motor, timing-and-speed |
| S015 Button mashing | Press counters, clicks-per-second scoring | C: M14, M15, M16 (hold alone is not enough) | input-and-motor |
| S016 Sustained holds | Aim, sprint, guard, charge, hold-to-interact | C: M13 per action, M15 | input-and-motor |
| S017 Short vs long press on one button | Tap interacts, hold opens a secondary action | C: M03, M39 | input-and-motor |
| S018 Repetition over long sessions | Harvesting, mining, buying, crafting loops | C: M15, M35, M36, D13, D25 | input-and-motor, structured-ui-and-strategy |
| S019 Chords | Modifier plus button, multi-key combos, hold-and-click | C: M01, M03, M18; A: L10 | input-and-motor |
| S020 Two analog sticks at once | Move plus aim or look, dual-stick play | C: M06, M20, D17 | input-and-motor |
| S021 Fine cursor or touch targets | Tiny hitboxes, inventory slots, moving targets | C: V16, V19, M27, M29, M40 | input-and-motor |
| S022 Dragging | Card placement, sliders, carrying, drag-select | C: M21, M32, M42 | input-and-motor |
| S023 Swipe, pinch, multitouch or path tracing | Gesture recognisers, lock-pick patterns | C: M30, M31; A: alternate puzzle form | touch-and-mobile |
| S024 Precise analog magnitude | Steering, variable jumps, bow draw, radial menus | C: M07–M11, O07, M23, D16 | input-and-motor |
| S025 Fast rotations or motion inputs | Quarter-circle specials, stick spinning | C: M01, M18, M19 | input-and-motor |
| S026 Controller motion, tilt or shake | Gyro-only aim, shake to escape, tilt maze | C: M25, M14 | input-and-motor |
| S027 Stick clicks, trigger resistance, strong force feedback | Sprint on stick click, adaptive triggers | C: M01, M12, M34, N12, N14 | input-and-motor |
| S028 Accidental repeats or rejected slow input | Repeat delay, double-click, debounce | C: M37, M38, M39 | input-and-motor |
| S029 Keyboard text entry | Names, chat, commands, quantities | B: labelled field (R01); C: L09, R21 | input-and-motor |
| S030 Many memorised inputs | Large action bars, shortcut sets, command lists | C: C07, C04, M03, M19; A: L10 | input-and-motor |
| S031 Input prompts and glyphs | Button icons, key names, help overlays | B: M01 (prompts follow bindings); C: C07, V16 | input-and-motor |
| S032 Menus operated differently from gameplay | Pointer-only inventory in a keyboard game | B: M01, M41; C: M21 | input-and-motor, screen-reader-and-voice |

### 3.3 Combat, traversal and detection

| Signal | Look for | Options | Guide |
| --- | --- | --- | --- |
| S033 Precise aiming | Raycasts, projectiles, headshots, small targets | C: D17, D18, V21, V22, V26, R17 | combat-aiming-stealth |
| S034 Sway, recoil or reticle movement | Recoil curves, aim sway, breath hold | C: O03, D17, V21 | combat-aiming-stealth |
| S035 Reload, swap or ammo management | Ammo counters, timed reload, empty-gun failure | C: D11, M15, M35, D16 | combat-aiming-stealth |
| S036 Parry, dodge or perfect block | Frame windows, telegraph-and-counter | C: D16, M39, D15, C06, H34, D05 | combat-aiming-stealth, timing-and-speed |
| S037 Off-screen threats | Attacks from behind, rear pursuit | C: H34, H22, V26, V34 | combat-aiming-stealth |
| S038 Melee combos or ability rotations | Combo graphs, action priorities | C: M19, M15, M03, C06 | combat-aiming-stealth |
| S039 Many simultaneous enemies or projectiles | Waves, bullet patterns, crowds | C: D05, D06, D09, D10, D14, N07, V26 | difficulty-and-assists |
| S040 Stamina, energy or oxygen | Depletion meter gates actions | C: D12, D08; second-channel warning | difficulty-and-assists |
| S041 Damage from several channels | Enemy, fall, hazard, self, friendly fire | C: D04, D05, D07, D08, D24 | difficulty-and-assists |
| S042 Grabs, struggles, escape prompts | Mash-to-break-free, pull meters | C: M14, M15, M16, D16 | input-and-motor |
| S043 Jumps, gaps, platforms | Landing checks, moving platforms, pits | C: D20, D21, D07, D08, V28, D14; A: R16 | traversal-and-platforming |
| S044 Climbing, ropes, ledges | Alternating grips, swinging, rappels | C: M36, M13, D12, D20 | traversal-and-platforming |
| S045 Chase, escape or autoscroll | Pursuit failure, moving death wall, rail event | C: D14, D29, D22, D23, D24 | traversal-and-platforming, timing-and-speed |
| S046 Stealth detection | Visibility, noise or suspicion meters, guard states | C: H34, V25, D09, D10, C14 | combat-aiming-stealth |
| S047 Stealth execution timing | Takedown or pickpocket window | C: D16, D18, M16 (separate from detection) | combat-aiming-stealth |
| S048 Forced failure, capture or death | Script advances on loss, unkillable boss | C: D04 (scripts stay assist-aware), D22, D23, D24 | difficulty-and-assists |
| S049 Companion or team AI management | Orders, companion survival duties | C: D27, D04, C26 | difficulty-and-assists |
| S050 Action minigame inside a slow game | Fishing, hacking, chase, sports challenge | Route its real demand (S013–S016, S022); C: D22, D23 | timing-and-speed |

### 3.4 Camera, motion, visual density and safety

| Signal | Look for | Options | Guide |
| --- | --- | --- | --- |
| S051 Moving or first-person camera | Look axes, camera rig, first-person controller | C: O02, O05, O07, O08, O16, M07, M08, M20 | camera-and-motion |
| S052 Camera shake or kick | Impulses, trauma values, hit or landing shake | C: O01 (default Reduced; Off reaches zero) | camera-and-motion |
| S053 Forced pan, recentre or cutscene camera | Camera rails, auto-look, scripted rotation | C: O09, O10, O11 | camera-and-motion |
| S054 Zoom, FOV kick, motion blur, speed lines | Sprint zoom, radial blur, tunnel effects | C: O04, O13, O14 | camera-and-motion |
| S055 Roll, banking or horizon motion | Vehicles, leaning, waves, head tilt | C: O12, D19 | camera-and-motion, vehicles |
| S056 Parallax, scrolling or animated menus | Layered skies, particle menus, moving UI | B: N05, N10; C: O17, V13 | camera-and-motion |
| S057 Screen distortion or damage overlays | Chromatic aberration, blur-on-hit, grain, vignette | C: O15, O21, P04 | camera-and-motion |
| S058 Flashing, strobing, rapid exposure | Lightning, muzzle flash, hit flash, light shows | B: P01; C: P02–P06, P08, P09 | photosensitivity |
| S059 Patterned textures or rapid cuts | Stripes, grids, checkerboards, high-contrast wipes | B: P01; C: P07, P09 | photosensitivity |
| S060 Stacking spells, explosions, hit effects | Particle emitters, bloom, additive shaders | C: N06, N07, P05, V26 | sensory-load-and-guidance, photosensitivity |
| S061 Guidance paint, highlights, trails | Coloured ledges, footprint trails, scan highlights | C: N03, N04, V27, C18 | sensory-load-and-guidance |
| S062 Crowded HUD or notifications | Floating numbers, toasts, big trackers | C: N01, N02, N08, C23, C09 | sensory-load-and-guidance |
| S063 Dark, low-contrast or foggy world | Darkness mechanics, silhouettes, distant targets | C: V26–V29, V35, V36 | color-and-clarity |
| S064 Rendered world text or handwriting | Notes, inscriptions, signs, diegetic screens | C: V30, V31, R12 | text-and-readability |
| S065 Video, user or generated visuals | Movie files, uploads, mods, procedural effects | B: P01; C: P10, F12; A: R13 | photosensitivity, content-and-phobias |

### 3.5 Audio, language, reading and nonvisual information

| Signal | Look for | Options | Guide |
| --- | --- | --- | --- |
| S066 Spoken dialogue | Voice assets, conversation events, cutscene speech | B: H17 (default On); C: C01, H03, H18, H20, H24–H31, H33, H37 | audio-and-captions |
| S067 Overlapping, radio or off-screen speech | Combat barks, party chatter, radio | C: H17, H18, H12, H21, H22, H23, H33 | audio-and-captions |
| S068 Positional audio cues | Footsteps, panned threats, sonar, turn calls | C: H09, H10, H19, H21, H22, H34; A: R18 | audio-and-captions |
| S069 Sound-only puzzle or music clue | Melody order, tune matching, echo clue | C: H19, H34, C14, C16 | audio-and-captions, memory-and-navigation |
| S070 Loud sudden sounds, wide dynamic range | Stings, explosions, quiet-to-loud mix | B: H01; C: H14, F11; A: H15 | audio-and-captions |
| S071 Distressing repetitive audio | Buzzing, chewing, breathing, alarms, heartbeat | C: H05, N15, F03; A: H15 | audio-and-captions |
| S072 Music sets the gameplay rhythm | Beat maps, song position, rhythm judgement | C: D16, M39, H34, H35, O17 | timing-and-speed |
| S073 Text-heavy UI | Tables, tooltips, rulebooks, many stats | C: V06–V10, C01, H31, C26, R07, R12 | text-and-readability |
| S074 Animated or typewriter text | Timed reveal, jittering or bouncing words | B: C01 (instant reveal); C: N11 | text-and-readability |
| S075 Timed dialogue or choices | Response clock, silence as a default choice | C: D29, C01, H31 | timing-and-speed |
| S076 Partial or no voiceover | Unvoiced lines, voice samples only | C: R12, R05, C01 | screen-reader-and-voice |
| S077 Important visual story events | Silent facial cue, map reveal, image clue | C: C13, R10; A: R13, R14 | screen-reader-and-voice |
| S078 Rapid or frequently changing state | Resource counters, combat log, prices | C: R02, R03, R07, C09 | screen-reader-and-voice |
| S079 Multiple languages | Translation tables, font fallbacks, voice packs | B: V03; C: H37, R09 | text-and-readability |
| S080 Icons without text | Status glyphs, toolbars, radial menus | B: R10, V25; C: C26 | text-and-readability |

### 3.6 Objectives, memory, inventory and understanding

| Signal | Look for | Options | Guide |
| --- | --- | --- | --- |
| S081 Quests or objectives | Quest data, objective manager, mission steps | C: C08, C09, C10, C17, C14 | memory-and-navigation |
| S082 Long or multi-session story | Chapters, story flags, long save timeline | C: C11, C12, C13, H33, D25 | memory-and-navigation |
| S083 Ability-gated backtracking | Locked doors, required powers, revisits | C: C17, C19, C20, C21 | memory-and-navigation |
| S084 Navigation or exploration | Large maps, multi-floor spaces | C: C14–C18, C20; A: R16, R18 | memory-and-navigation |
| S085 Screenshot or image journal | Map photos, clue pictures, illustrated codex | C: C21, R10, V31 | memory-and-navigation |
| S086 Puzzles | Puzzle state, clue objects, mandatory reasoning | C: C14, C15, C16, D22, C28 | memory-and-navigation |
| S087 Inventory or equipment | Slots, containers, rarity, stacks | C: V25, M35, C28, R07, C31, C32, C35, C36 | structured-ui-and-strategy |
| S088 Crafting or recipes | Recipe data, workbench, material checks | C: M15, D13, C10, C19, C31, C33, C34, C35 | structured-ui-and-strategy |
| S089 Equipment or stat optimisation | Comparisons, builds, skill trees | C: C26, C28, R07 | structured-ui-and-strategy |
| S090 Nested tooltips or encyclopedia | Hover chains, codex references | B: C38; C: C24, C26, M21, R11 | structured-ui-and-strategy |
| S091 Irreversible or destructive actions | Sell, drop, delete, end turn, prestige | B: C29; C: C28, D26 | structured-ui-and-strategy |
| S092 Hidden rules or complex dependencies | Damage formulas, logistics, status interactions | B: C02; C: C05, C06, C12, C26 | structured-ui-and-strategy |
| S093 Multi-stage plans or jobs | Craft queues, build chains, tactical orders | C: C10, C28, M15 | structured-ui-and-strategy |
| S094 Data overlays | Heatmaps, economy charts, production graphs | C: V24, V25, C26, R02 (narrated aggregates) | structured-ui-and-strategy |
| S095 Relationship or character memory | Cast lists, affinity, aliases | C: C11, C12, H33 | memory-and-navigation |
| S096 Parser or typed commands | Command grammar, required spelling | C: L09, R21, C24, C26; synonyms, history | text-and-readability, input-and-motor |

### 3.7 Failure, time, scarcity and persistent worlds

| Signal | Look for | Options | Guide |
| --- | --- | --- | --- |
| S097 Gameplay time limits | Mission countdown, oxygen deadline, end of day | C: D29, D14, D08 | timing-and-speed |
| S098 Non-essential UI timeouts | Toast expiry, respawn choice, disappearing tips | B: C23; C: K11 | timing-and-speed |
| S099 Permadeath or run reset | Save deletion, ironman, restart from zero | C: D24, D25, D28 | difficulty-and-assists |
| S100 Corpse run or currency loss | Dropped loot, death penalties | C: D28, D24, D05 | difficulty-and-assists |
| S101 Resource scarcity or grind | Scarce ammo, food or fuel, rare drops | C: D11, D13, M15 | difficulty-and-assists |
| S102 Hunger, thirst, decay, maintenance | Needs meters, spoilage, durability | C: D08, D13, C08, D27 | difficulty-and-assists |
| S103 Real-world clock or offline progress | Daily events, crop decay, idle reward caps | C: D25, D28, D29, C11 | timing-and-speed |
| S104 Long uninterrupted sequences | Long boss, raid, song, cutscene, race | B: D27; C: D24, D25, L20, C11 | difficulty-and-assists |
| S105 Random gates or repeated failed rolls | Rare spawns, failed crafts, mutation chance | C: D13, D24, C26 | difficulty-and-assists |
| S106 Possibly unrecoverable states | Lost key, out of bounds, stuck vehicle | B: assess; C: D23, D24, D26 | difficulty-and-assists |
| S107 Grades, ranks or achievements | Timer ranks, perfect chains, trophies | B: policy audit (no assist penalties) | store-claims-and-compliance |
| S108 Shared server or global progression | Persistent world, server-owned clock, economy | C: K21, K23, D24 | multiplayer-and-communication |

### 3.8 Phobia and sensitive content

Routing categories, not diagnoses. Inspect shape, motion, sound, icons, text and thumbnails, not
just file names. Coverage beyond Grounded's spider changes is **UNVERIFIED** as a shipped pattern.

| Signal | Look for | Options | Guide |
| --- | --- | --- | --- |
| S109 Spiders or spider-like creatures | Legs, silhouettes, webs, crawling audio, icons | C: F01, F03, F12, F13 | content-and-phobias |
| S110 Insects, swarms, larvae | Swarms, eggs, buzzing, infestations | C: F02, F03, F12, H05 | content-and-phobias |
| S111 Snakes or serpentine motion | Coils, slithering, hissing reveals | C: F02, F03, F12 | content-and-phobias |
| S112 Clowns, masks, faces | Make-up, masks, facial close-ups | C: F02, F12, F13, F14 | content-and-phobias |
| S113 Needles or medical procedures | Close-up tools, injections, narrated procedures | C: F10, F13, F14 | content-and-phobias |
| S114 Blood, gore, dismemberment | Decals, gibs, corpses, injury text or audio | C: F06, F07, F12, F14 | content-and-phobias |
| S115 Deep water, drowning, dark water | Mandatory dives, oxygen, submerged threats | C: F04, F13, F14, D08; A: alternate route | content-and-phobias |
| S116 Heights, falls, voids | Exposed ledges, glass floors, forced look-down | C: F13, F14, O10, D07; A: F05 | content-and-phobias |
| S117 Clustered holes (trypophobia-like) | Organic hole patterns, nests, pores | C: F12, F13, F15 texture alternatives; A: fully authored replacement surfaces (UNVERIFIED as shipped) | content-and-phobias |
| S118 Jump scares or abrupt pursuit | Scare triggers, loud stings, sudden spawns | C: F11, F13, H14, O01, P04, D10 | content-and-phobias |
| S119 Traumatic themes | Assault, self-harm, abuse, death, discrimination | C: F12, F13, F14 | content-and-phobias |
| S120 Gambling, addiction or other distressing content | Casino play, simulated betting | C: F12, F14, N08 | content-and-phobias |
| S121 A settings preview shows the trigger | Spider preview, gore clip, alarm sample | B: text-first choice, preview only on request; C: F01–F03 | content-and-phobias |
| S122 User, mod or procedural content bypasses filters | Spawnable mod enemies, generated patterns | C: F12, P10; A: L14 content tags | content-and-phobias |

### 3.9 Multiplayer and social

| Signal | Look for | Options | Guide |
| --- | --- | --- | --- |
| S123 Online voice chat | VOIP, proximity voice, team channels | C: K03, K04, K05, K07, K14, K15, K17, H08 | multiplayer-and-communication |
| S124 Online text chat | Chat panel, channel tabs | C: K01, K10, K11, K12, K16, R04 | multiplayer-and-communication |
| S125 Coordination needs callouts | Raid roles, secret info, team tactics | C: K05–K08, K18 | multiplayer-and-communication |
| S126 Lobby, invites, ready checks | Party creation, match accept countdown | B: K22; C: K21 | multiplayer-and-communication |
| S127 Local players or split screen | Several input users or viewports | C: K20, L03, V25, H24 | multiplayer-and-communication |
| S128 Shared device or controller handoff | Pass-and-play, hotplug, profile swap | B: L02; C: L03, K20 | multiplayer-and-communication |
| S129 Co-op synchronised actions | Two switches at once, co-op QTE | C: M18, K19, K21 | multiplayer-and-communication |
| S130 Competitive ranking | Ladders, tournaments, leaderboards | B: no assist labels or penalties; C: K23, C06 | multiplayer-and-communication, store-claims-and-compliance |
| S131 Spectator, replay or scoreboard | Esports HUD, replays, results | C: V01, V25, H17, R01 | multiplayer-and-communication |
| S132 UGC or harassment exposure | Drawings, uploads, emotes, proximity voice | C: K15, K16, K17, F12, P10 | multiplayer-and-communication |
| S133 Disconnect, AFK kick, session loss | Idle kick, reconnect, server save | C: K21, D25, L20 | multiplayer-and-communication |

### 3.10 Touch, XR, vehicles and the body

| Signal | Look for | Options | Guide |
| --- | --- | --- | --- |
| S134 Touch input | Touch handlers, virtual sticks, canvas gestures | C: M24, M27, M28, M29, M30, L13, R19 | touch-and-mobile |
| S135 Fixed orientation or reach | Edge controls, two-thumb layout, landscape lock | C: L12, M05, M28 | touch-and-mobile |
| S136 AR camera or location gate | Camera permission, GPS, geofence, walking | C: M43; A: M47 stationary or remote route | vr-and-xr |
| S137 VR | XR origin, headset, tracked controllers | C: O18, O19, O20, M43, M44, H30 | vr-and-xr |
| S138 Standing or room-scale required | Floor pickups, bending, walk-to-target | C: M43, M44, M45; A: alternate task | vr-and-xr |
| S139 Both hands required | Two-hand tools, off-hand menus | C: M46, M05, M18 | vr-and-xr |
| S140 Large or precise gestures | Throws, overhead reach, dance poses | C: M25, M26, M44, M45, D16 | vr-and-xr |
| S141 XR continuous locomotion or turning | Smooth move, smooth turn, acceleration | C: O18, O19, O20 | vr-and-xr |
| S142 XR flying, driving, forced movement | Cockpits, rollercoasters, conveyors | C: O12, O20, D19 | vr-and-xr, vehicles |
| S143 Spatial captions or menus | Head- or world-locked panels | C: H21, H30, V01; A: R16 | vr-and-xr |
| S144 Controllable vehicle | Throttle, steer, brake, roll axes, cockpit | C: D19, O12, M07–M10, M01; A: R16 | vehicles |
| S145 Collision or physics-driven camera motion | Knockback, boat waves, ragdoll view | C: O01, O11, D08, D19 | camera-and-motion, vehicles |

### 3.11 Slow genres with hidden active demands

| Signal | Look for | Options | Guide |
| --- | --- | --- | --- |
| S146 Orders under a running simulation (RTS) | Economy ticks while you act, multi-front alerts | C: D14, D15, D27, M15, R03 | structured-ui-and-strategy, timing-and-speed |
| S147 Turn or grid targeting | Hex or square board, legal-target overlay | C: M21, M32, C26, V25, R02 (state and coordinates on demand) | structured-ui-and-strategy |
| S148 Wave or preparation countdowns | Next-wave timer, auto-battler shop phase | C: D29, M32 | structured-ui-and-strategy |
| S149 Card or deck state | Hand, deck, discard, nested effects | C: V01, V25, M32, C28, R07, R02 | structured-ui-and-strategy |
| S150 Construction, placement, gizmos | Snap grids, transform handles | C: M23, M32, C28, D27, V16 | structured-ui-and-strategy |
| S151 Player-manipulated physics | Rigid bodies, force tools, unstable stacks | C: D14, D27, C28, D26, P05 | structured-ui-and-strategy |
| S152 Day cycles, fishing, cooking | End-of-day collapse, fishing tension, timed recipes | C: D29, M15, D16, D25 | timing-and-speed |
| S153 Educational assessment | Scores meant to measure learning | C: C04, C25, C26, D29 | kids-and-education |
| S154 Idle production or prestige | Per-second counters, automation unlocks, resets | C: M15, C29, R02, D25 | structured-ui-and-strategy |
| S155 Numeracy or rule-reading demands | Schedules, money, odds, rates | B: C02; C: C26, R07 | structured-ui-and-strategy |
| S156 Mods or custom hardware supported | Plugin loader, remap layer, content packs | B: M01; C: L05, L06; A: L14, L15 | mods-hardware-and-platforms |
| S157 Has random encounters or turn-based battle commands | Encounter tables, step counters, battle scenes, turn queues, `encounter`, `battle`, `turn_order` | C: D31, D32, D33, D23, D14 | difficulty-and-assists, structured-ui-and-strategy |

## 4. Exclusions and collisions

- **Turn-based** suppresses real-time timing only when no real-time exploration, minigame, active
  time gauge, timed decision, lobby deadline or expiring UI remains.
- **2D** does not suppress camera comfort. Scrolling, parallax, zoom, shake and transitions count.
- **Silent** skips speech captions, not narration or alternatives to visual state.
- **Cozy** does not suppress failure, fatigue, time or resource routes. Inspect harvesting, fishing,
  daily deadlines and collection loops.
- **Minimal HUD** must coexist with memory and navigation aids. Store each independently.
- **Camera assist** may move the view more; **reduced motion** may need it off. Offer both separately.
- **High contrast** can add glare or patterns. Test it with the photosensitivity checks.
- **Sound visualiser** helps hearing access but adds clutter. Expose priority, size and density.
- **Auto-action** reduces command variety but can keep timing and repetition (WoW Single-Button
  Assistant, Hi-Fi RUSH Auto-Action). Route those signals separately.
- **Infinite resources** is not invulnerability. **Combat damage Off** is not fall damage Off.
- **Warning, substitution, reduced scare and skip** are different capabilities. Record which exist.
- **Menu narration** is not screen-reader-playable gameplay. Track world, tooltips, dialogs and
  dynamic updates separately.
- **A mod** shows a useful pattern. It never replaces native access or proves platform support.

## 5. Detection

Run `python scripts/scan_signals.py <project_dir>` to generate **candidate** signals from file
names, identifiers and assets. Candidates are leads, never proof. Trace each to reachable content.

| Inspect | Reveals | Cannot prove alone |
| --- | --- | --- |
| Pitch, design doc, feature list | Camera, combat, puzzles, platforms, saves, progression | What actually ships; content coverage |
| Level and mission scripts, encounter graphs | QTEs, chases, stealth, timers, vehicles, forced loss | That every path was inspected |
| Input map and action definitions | Holds, chords, stick clicks, gyro, touch, XR actions | Runtime remapping or assistive-tool use |
| Gameplay scripts and state machines | Windows, mash counters, damage, drains, jump buffers | That a setting reaches every subsystem |
| Scenes and prefabs | Camera rigs, focus, text widgets, drag handlers, XR reach | Runtime readability and comfort |
| Shaders, particles, animations | Flash emission, exposure pulses, distortion, sway | Photosensitivity safety (needs rendered checks) |
| Dialogue and localisation files | Speech, unvoiced text, timed branches, speakers, tags | Caption quality, timing, re-readability |
| Item, recipe, quest and save schemas | Inventory density, economy, run loss, destructive acts | Usable UI or valid recovery |
| Audio events and mixer | Voice, positional cues, stings, harsh loops, buses | Caption completeness, sound-only progression |
| Asset names, tags, import metadata | Spiders, snakes, needles, gore, water, heights, holes | Appearance, motion, reachability |
| Network, chat, session code | Voice, text, pings, AFK, reconnect, server clocks | Service availability, transcription quality |
| Platform and export config | Mobile, VR/AR, console targets | Feature parity across platforms |
| Existing accessibility settings and tests | Implemented toggles and saved config | That they work end to end |

| Candidate route | Search terms | Follow up |
| --- | --- | --- |
| Timed input | `qte`, `quick_time`, `perfect`, `parry`, `window`, `deadline`, `reaction` | Gameplay timing or just a cooldown? What happens on failure? |
| Repetition, holding | `mash`, `tap_count`, `clicks`, `repeat`, `held`, `is_pressed`, `charge` | Is rapid pressing required? Is there already a single-press path? |
| Aiming | `aim`, `reticle`, `raycast`, `headshot`, `recoil`, `spread`, `lock_on` | Essential, optional or AI-only? Which modes allow assists? |
| Traversal | `jump`, `ledge`, `dash`, `grip`, `rope`, `climb`, `coyote`, `platform` | Mandatory gaps or timing? Needed to finish the tutorial? |
| Camera, motion | `camera`, `shake`, `trauma`, `bob`, `sway`, `roll`, `fov`, `blur`, `zoom`, `parallax` | Trace gameplay, cutscenes, vehicles and menus |
| Flashing | `flash`, `strobe`, `flicker`, `lightning`, `exposure`, `emission`, `blink`, sine pulses | Inspect rendered output. No match proves nothing |
| Colour state | `team_color`, `rarity`, `palette`, red/green compares, tinted status icons | Can shape, label or sound identify every state? |
| Narrative, memory | `quest`, `objective`, `journal`, `codex`, `recap`, `dialogue`, `speaker`, `chapter` | Does learned information persist across sessions? |
| Random battles, turn commands | `encounter`, `encounter_rate`, `step_count`, `battle`, `turn_order`, `command_menu` | Random or scripted battles? Which battles are story-required? Can a battle be left on auto? |
| Inventory, crafting | `recipe`, `ingredient`, `stack`, `slot`, `craft`, `container`, `equipment` | Count repeated operations; check focus and reading order |
| Failure, scarcity | `permadeath`, `ironman`, `delete_save`, `death`, `drop_loot`, `hunger`, `oxygen`, `durability` | Separate damage channels; assist-aware scripted failure |
| Stealth | `alert`, `suspicion`, `detection`, `visibility`, `noise`, `search_state` | Which senses convey each state? Recovery after discovery? |
| Phobia content | `spider`, `arachnid`, `insect`, `larva`, `swarm`, `snake`, `serpent`, `clown`, `needle`, `blood`, `gore`, `ocean`, `abyss`, `nest`, `pores` | Trace art, animation, sound and text without opening trigger previews. A character named Spider need not be one |
| Multiplayer | `voice`, `voip`, `chat`, `ping`, `invite`, `lobby`, `ready`, `afk`, `disconnect` | No-voice paths, moderation, server-owned timing |
| Touch, XR, AR | `touch`, `swipe`, `pinch`, `gyro`, `xr`, `vr`, `headset`, `grip`, `teleport`, `gps`, `geofence` | Target platforms, input fallback, reach, location gates |

## 6. Conversation

Ask about **tasks the player performs**, never about anyone's medical history or disabilities.
For a new project, use this short interview. For a built project, summarise the evidence first and
ask only for the unresolved fact.

1. What does the player do repeatedly, and which actions must be fast or precise?
2. Which camera or view, input devices and platforms are supported?
3. What information is delivered through colour, sound, text, spatial layout or animation?
4. What causes failure, lost progress, urgency or resource pressure?
5. What must the player remember across scenes or sessions?
6. Which multiplayer rules or physical, VR or AR requirements are fixed, and which can change?
7. Which sensitive subjects, creatures, imagery or abrupt effects can occur?

| Ambiguity | Ask | Do meanwhile |
| --- | --- | --- |
| "Turn-based", but a timer is in code | "Does this timer ever limit a decision or response, or is it only animation or cooldown?" | Plan readable UI and narration |
| "No combat", but fishing or hacking exists | "Can any mandatory minigame fail because input is too late, fast or imprecise?" | List every minigame and its dependencies |
| Camera-shake variable found | "Can it run in cutscenes, menus or vehicles, and should one setting cover all?" | Trace camera modifiers |
| `spider` asset found | "Is this a reachable spider-like creature with matching sound, animation or icons, or just a name?" | Trace references without opening previews |
| Water level found | "Is going underwater mandatory? Which relief fits: clearer water, no oxygen pressure, a skip?" | Inspect oxygen and progression separately |
| "Easy" difficulty exists | "Which variables change, and can damage, timing, navigation and resources be set separately?" | Read difficulty data; list unexposed demands |
| "Accessible font" listed | "Can players set font, size and spacing everywhere, including tooltips and translations?" | Audit layouts and font fallback |
| "One-button" mode promised | "Does it remove repetition and timing too, including menus and movement?" | Count actions in a full flow |
| Server owns multiplayer time | "Which modes can allow longer decisions, pause or assists, and what shared rules apply elsewhere?" | Build personal text, comfort and input settings |
| Mods offered as the solution | "Which platforms and actions must work natively? Which hooks stay stable for assistive tools?" | Document input and semantic boundaries |
| Guidance paint is divisive | "Should navigation, interaction highlights, combat warnings and hints be set separately?" | Find the separate render channels |
| A narration setting exists | "Does it cover gameplay state and all controls, or only menus? Which screen readers and languages?" | Build a narration coverage list |
| VR mode is "seated" | "Can every task, menu and recalibration be done without standing, bending, reaching behind or two hands?" | Inspect reach bounds |
| Learning game has a scored timer | "Is speed the learning goal, or does the timer only add pressure?" | Separate scoring dimensions |
| Save restrictions are intentional | "What must stay for the default challenge, and which opt-in suspend or checkpoint path fits the no-penalty rule?" | Trace save and progression integrity |

Record unresolved choices as limitations with a scope decision. Never invent a feature or quietly
mark an unknown signal as irrelevant.

## 7. Selection

1. Load [core-policy.md](core-policy.md), this router and the Baseline rows (section 2).
2. Extract intended mechanics from the conversation and design; inspect the project when it exists.
3. Seed candidate signals from genre and platform labels. Candidates are not confirmations.
4. Inspect every reachable activity, mandatory and optional: menus, minigames, cinematics, DLC.
5. Classify each signal with a state, evidence and scope. In Build mode, planned features count.
6. Expand confirmed signals into option IDs and the guides in the Guide column.
7. Deduplicate guides. Keep every option an independent setting with its own owner.
8. Ask targeted questions only for ambiguous signals that affect the next decision.
9. Implement Baseline and confirmed Contextual work. Give each Advanced need a scope decision.
10. Verify the whole player task with the options on, not just that the setting exists.
11. Re-scan when mechanics or content change, and revisit earlier exclusions.

Example `ACCESSIBILITY.md` row:
`| S014 QTEs | present | chapter_3/escape | scenes/escape.tscn, qte.gd | M16, M17, D27 |`

| Distinction | Keep separate |
| --- | --- |
| Campaign vs PvP vs private or practice | Personal presentation and input everywhere; simulation-changing assists need explicit session rules |
| Local player vs host or server | Per-player preferences stored apart from shared world settings |
| Native vs mod vs engine capability | Record provenance. A mod or engine API is not a shipped native feature |
| Desktop vs mobile vs console vs XR | Load only the real target's guides; verify per platform, never assume parity |
| Existing save vs new game | The assist must unblock the current save |
| Base game vs DLC, minigame, UGC | Scan each; state filter and coverage limits truthfully |
| Menu access vs gameplay access | Track navigation, world information, action and story understanding separately |
| Input simplification vs timing vs fatigue | Verify each; one-button timing can still need rapid sustained input |

| Project | Load now | Do not load without new evidence |
| --- | --- | --- |
| Silent, static, untimed card puzzle; keyboard and mouse | Baseline; text-and-readability, color-and-clarity, structured-ui-and-strategy, memory-and-navigation | combat-aiming-stealth, audio-and-captions speech work, vehicles, vr-and-xr |
| 2D metroidvania, dash combat, voiced story | Baseline; input-and-motor, timing-and-speed, combat-aiming-stealth, traversal-and-platforming, memory-and-navigation, audio-and-captions, camera-and-motion, photosensitivity | structured-ui-and-strategy turn UI, vehicles, multiplayer-and-communication |
| Turn-based JRPG with a rhythm attack and fishing | Baseline; structured-ui-and-strategy, screen-reader-and-voice, memory-and-navigation, timing-and-speed (rhythm and fishing scope), difficulty-and-assists | combat-aiming-stealth aiming, vehicles |
| Cozy phone farming game, timed days, dragging | Baseline; touch-and-mobile, input-and-motor, text-and-readability, structured-ui-and-strategy, timing-and-speed, difficulty-and-assists | combat damage options, vr-and-xr |
| Competitive FPS with private bot matches and voice | Baseline; input-and-motor, combat-aiming-stealth, color-and-clarity, camera-and-motion, photosensitivity, audio-and-captions, multiplayer-and-communication | structured-ui-and-strategy crafting, dialogue-choice timing |
| VR city builder | Baseline; structured-ui-and-strategy, vr-and-xr, screen-reader-and-voice, difficulty-and-assists (saves) | combat-aiming-stealth, traversal-and-platforming |
| Text adventure with disturbing descriptions | Baseline; text-and-readability, screen-reader-and-voice, memory-and-navigation, content-and-phobias (warnings, skips) | Mesh or texture replacement, camera-and-motion |
