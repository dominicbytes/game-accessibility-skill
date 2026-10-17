# 03 — Genre and mechanic routing for a game accessibility skill

Research date: 17 October 2026. Scope: engine-neutral and genre-neutral routing, not engine sample code.

## How to read this report

The proposed router should select guidance from observable player demands, not from a genre whitelist. Genre is useful for asking initial questions; a confirmed mechanic determines applicability. A game can have several routes, including routes used only in one scene or minigame. Xbox's guidelines explicitly use scoping questions about game elements, which supports this approach. They are design guidance, not a legal compliance certification. [XAG]

Evidence labels used below:

- **Documented feature:** a developer, publisher, platform owner, or feature maintainer documents the feature.
- **Reported barrier:** a cited player, accessibility reviewer, or research participant describes a problem. This is evidence of that person's experience, not a prevalence estimate or proof that everyone with the same disability experiences it.
- **Design inference / recommendation:** my proposed routing, defaults, implementation choices, and tests. These are engineering recommendations derived from the demands of the mechanic; they are not claims that a source tested the proposed implementation.
- **UNVERIFIED:** research did not establish the stated fact. Do not turn an unverified example into a skill requirement or claim of shipped support.

The genre priorities, exclusions, option tiers, and reference-file split are recommendations. “Best examples” means particularly useful documented patterns for this assignment, not a comparative ranking of entire games. A feature list does not establish that a whole game is accessible. CIPT explicitly cautions that its reviews reflect individual perspectives. [REVIEWS]

### Tiers and defaults

| Tier | Meaning for this skill | Routing behavior |
| --- | --- | --- |
| **Baseline (B)** | Evaluate in every game: settings access, usable input/UI, information access, safe presentation, preservation of preferences, and respectful player choice. | Always load a short core checklist. A nonexistent modality requires no dummy option: a silent game does not need a dialogue-volume slider. |
| **Contextual (C)** | Required planning/audit work when a corresponding mechanic, modality, or content signal exists. | Load only the matching reference sections; confirm applicability and implement an appropriate option or accessible design. |
| **Advanced (A)** | More extensive alternatives, such as nonvisual spatial exploration, eye-gaze interaction, or complex shared-control support. | Record the relevant need immediately, then load detailed implementation guidance when requested, budgeted, or necessary to the promised supported experience. |

**Advanced does not mean optional for a player who needs it.** It describes likely implementation scope, not human importance. Screen-reader semantics for ordinary menus should be planned as Baseline architecture; building a complete nonvisual substitute for a complex 3D world is a separate Advanced task. Do not hide foundational UI access in a “large budgets only” route. XAG 106 addresses narration across UI, while XR has additional spatial and interaction needs. [X106] [XAUR]

Unless a safety or usability reason below says otherwise, preserve the developer's intended initial gameplay and make assists opt-in, available before their first barrier and changeable during play. Preserve intentional design without adding arbitrary unlocks. Save preferences independently of story progression. Do not shame players, mark saves, display a compulsory assist badge, disable achievements/trophies, or withhold content. These are **owner requirements from 00_BRIEF.md**, not claims about every cited game. In particular, do not carry forward the older jam convention of a compulsory “ASSIST ON” badge.

### Source key

Reference-style links throughout this file resolve to source URLs here. Official documentation describes particular versions/platforms; retest the target build.

[XAG]: https://learn.microsoft.com/en-us/xbox/accessibility/guidelines
[GAG]: https://gameaccessibilityguidelines.com/full-list/
[X101]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/101
[X103]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/103
[X104]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/104
[X106]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/106
[X107]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/107
[X108]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/108
[X116]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/116
[X117]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/117
[X118]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/118
[X119]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/119
[X120]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/120
[X123]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/123
[REVIEWS]: https://caniplaythat.com/reviewing-guide/
[TLOU2]: https://blog.playstation.com/2020/06/09/the-last-of-us-part-ii-accessibility-features-detailed/
[GEARS]: https://www.gearsofwar.com/en-gb/games/gears-5/accessibility/
[HALO]: https://support.halowaypoint.com/hc/en-us/articles/4408122295444-Accessibility-Features-in-Halo-Infinite
[HALO-FEEDBACK]: https://www.halowaypoint.com/en-us/news/technical-preview-outcomes
[GOW]: https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/
[POP]: https://news.ubisoft.com/en-us/article/5nGZiBSFtcEzFd93QlTotS/prince-of-persia-the-lost-crown-accessibility-spotlight
[ELDEN-REVIEW]: https://caniplaythat.com/2022/02/27/elden-ring-lacking-accessibility-unintentional-barriers-and-land-jellyfish/
[CELESTE]: https://www.celestegame.com/changelog.html
[FOREST]: https://whitethorngames.com/theforestcathedral/accessibility
[FOREST-DEV]: https://www.gamedeveloper.com/design/deep-dive-designing-for-accessibility-in-the-forest-cathedral
[FF-PIXEL]: https://www.square-enix-games.com/en_GB/home/final-fantasy-vi-pixel-remaster-launches-digitally-ps4-nintendo-switch-april-19th
[BG3-REVIEW]: https://caniplaythat.com/2023/08/31/baldurs-gate-3-accessibility-review/
[VALHALLA]: https://news.ubisoft.com/en-us/article/5Rys9UeXZh9bAjnHBZN4pC/assassins-creed-valhalla-bringing-accessibility-to-a-viking-epic
[WOW]: https://news.blizzard.com/en-us/article/24205457/legacy-of-arathor-get-an-assist-with-new-ui-options
[WOW-EXPERIENCE]: https://www.pcgamer.com/games/world-of-warcraft/world-of-warcraft-single-button-rotation-assist-verdict/
[GROUNDED]: https://grounded.obsidian.net/news/grounded/accessibility-features
[NMS]: https://www.nomanssky.com/waypoint-update/
[SOMA]: https://frictionalgames.com/2017-11-what-is-somas-safe-mode/
[DEADSPACE-CONTENT]: https://www.ea.com/games/dead-space/dead-space/content-warning?isLocalized=true
[FORZA]: https://news.xbox.com/en-us/2023/04/27/forza-motorsport-accessibility-features-blind-driving/
[FORZA-FIXES]: https://forza.net/news/forza-motorsport-update-5-fixes
[SQUADRONS]: https://www.ea.com/amp/news/star-wars-squadrons-accessibility
[MADDEN]: https://www.ea.com/able/resources/madden-nfl/madden-nfl-24
[MADDEN-BUG]: https://forums.ea.com/discussions/madden-nfl-24-technical-issues-en/bug-in-menu-narration/8782703
[SF6]: https://www.capcom-support.com/hc/en-us/articles/7122030981660-What-are-the-control-types-in-Street-Fighter-6
[SF6-AUDIO]: https://www.capcom.co.jp/ir/english/news/pdf/e250403.pdf
[HIFI]: https://support.krafton.com/hc/en-us/articles/52371575035801-Hi-Fi-RUSH-Accessibility-Settings
[HIFI-REVIEW]: https://access-ability.uk/2023/02/09/hi-fi-rush-and-audio-visual-rhythm-game-feedback/?amp=1
[BOTANY]: https://whitethorngames.com/botanymanor/accessibility
[FARMER]: https://whitethorngames.com/princessfarmer/accessibility
[FARMER-DEV]: https://whitethorngames.com/whitethorn-games-news/accessibility-features-botany-manor-mythwrecked-princess-farmer-access-ability-showcase-2023
[KING-FEEDBACK]: https://community.king.com/en/candy-crush-saga/discussion/462233/colour-blind-players
[KING-MOTION]: https://community.king.com/en/candy-crush-friends-saga/discussion/313075/new-check-out-the-accessibility-features
[TRIMPS]: https://github.com/Trimps/Trimps.github.io/blob/master/ScreenReader.html
[TRIMPS-ISSUES]: https://github.com/Trimps/Trimps.github.io/issues
[ADR-ISSUE]: https://github.com/doublespeakgames/adarkroom/issues/70
[AOE]: https://www.ageofempires.com/age-iv-accessibility/
[CIV7]: https://support.civilization.com/hc/en-us/articles/38337649895187-Civilization-VII-Patch-Notes-February-5-2025
[CK3-STORE]: https://store.steampowered.com/app/1158310/Crusader_Kings_III/
[CORE-ERROR]: https://cakestorm.itch.io/core-error
[SAP]: https://store.steampowered.com/app/1714040/Super_Auto_Pets/
[RIOT]: https://www.riotgames.com/en/accessibility
[TFT-TRACKER]: https://followthefix.teamfighttactics.leagueoflegends.com/zh-tw/
[CIV-FEEDBACK]: https://www.reddit.com/r/civ/comments/1ffohs0/civ_7_feedback_accessibility_denuvo_hardware/
[DEADCELLS]: https://deadcells.wiki.gg/wiki/Assist_Mode_and_Accessibility
[DEADCELLS-SUPPORT]: https://playdigious.helpshift.com/hc/en/7-dead-cells/faq/104-how-do-i-activate-assist-mode-on-dead-cells/
[BLACKHOLE]: https://github.com/game-a11y/Balatro-BlackHole
[APICO]: https://whitethorngames.com/apico/accessibility
[FS25]: https://www.farming-simulator.com/newsArticle.php?country=us&lang=en&news_id=573
[STARDEW-ACCESS]: https://github.com/stardew-access/stardew-access
[LIS]: https://caniplaythat.com/2021/10/12/life-is-strange-true-colors-accessibility-review/
[LIS2]: https://caniplaythat.com/2020/03/07/teen-friendly-review-series-life-is-strange-2-and-captain-spirit/
[LIS1]: https://caniplaythat.com/2020/01/31/teen-friendly-review-series-life-is-strange-2015/
[RENPY]: https://www.renpy.org/doc/html/self_voicing.html
[JACKBOX]: https://www.jackboxgames.com/blog/accessibility-features-in-the-jackbox-party-pack-10
[JACKBOX8]: https://www.jackboxgames.com/streaming-moderation-accessibility-features-jackbox-party-pack-eight/
[XAUR]: https://www.w3.org/TR/xaur/
[OWL]: https://owlchemylabs.com/faq
[NMS-VR]: https://www.nomanssky.com/fractal-update/?cli_action=1677074777.055
[EDUCATION]: https://edusupport.minecraft.net/hc/en-us/articles/360061503031-Accessibility-Features
[PBS]: https://dc79r36mj3c9w.cloudfront.net/prod/filer_public/pbsnc-redesign-phase-1/about/c85308bf39_July_2022_PBS_DEI_AnnualReport.pdf
[PBS-GUIDE]: https://static.pbslearningmedia.org/media/media_files/31536ca5-64a6-4cb7-9df8-e229b90cd985/87845c36-5a69-4d30-ac39-4c61433e17dc.pdf
[CHOICESCRIPT-REPORT]: https://forum.choiceofgames.com/t/accessibility-issues-in-the-choicescript-game-ui-for-screen-reader-users/174983
[IF-PLAYER]: https://www.reddit.com/r/choiceofgames/comments/144jss5/recommend_me_a_game_for_a_lets_play/
[TEARDOWN]: https://teardowngame.com/faq.html
[MOBILE-STUDY]: https://arxiv.org/abs/2502.09866
[JUSANT]: https://support.dont-nod.com/hc/en-gb/articles/14957643453853-Jusant-Patch-1-04-The-Accessibility-Update-November-2023

## 1. Always evaluate these foundations

These recommendations extend the brief's existing jam settings. They are not a request to repeat the jam checklist as the final skill.

| ID | Baseline obligation | Default and implementation recommendation | Verification / main pitfall |
| --- | --- | --- | --- |
| B01 | Accessible first launch, settings discovery, persistence, and reset. | Offer settings before interactive story or loud output; honor applicable platform preferences with an override; save accessibility preferences outside campaign slots. | Start with no prior save, use each supported input, relaunch, and reset one category. Never erase unrelated preferences. |
| B02 | Operable UI and adaptable input architecture. | Semantic actions, stable focus, remap-aware prompts, readable labels, digital alternatives where feasible; do not require a pointer for keyboard navigation. | Follow a complete start/settings/play/save/exit path. A remapped attack with an unmapped menu confirm still blocks access. |
| B03 | Readable information and semantic UI. | Legible initial size and contrast; scalable text/icons without clipping; plain font option; accessible names, roles, values, focus, and reading order. | Test dense views and localization. A TTS button does not establish complete screen-reader access. |
| B04 | Multiple channels for essential state. | Avoid color-only and sound-only requirements by design. Add configurable equivalent channels where a demand exists. | Mute audio; inspect distinctions without color; test narration independently of appearance. |
| B05 | Safe presentation assessment. | Review all visual output for photosensitivity; avoid hazardous output by default. Provide reduced effects and safe audio access before exposure. | Include boot logos, transitions, stacking effects, video, user content, and menus. An epilepsy warning is not a mitigation. |
| B06 | Respectful options and truthful documentation. | Explain effects, make previews optional and safe, support individual adjustments, preserve achievement/content access and save dignity. | Check save UI, completion logic, restart requirements, and platform-specific documentation. |

Source basis: XAG 101 covers scalable readable text; XAG 103 covers redundant cue channels; XAG 107 covers input alternatives; XAG 118 says to test visual games even without intentional flashes. The core structure above is a proposed implementation policy. [X101] [X103] [X107] [X118]

Baseline is an assessment scope, not a mandate to add unused controls. Apply subtitles to speech, not to a game with no speech; apply camera comfort to moving views, not to static prose. Conversely, “not an action game” is never evidence that safety or input assessment can be skipped.

## 2. Option bundles used by the router

The following is the option catalog for the signal map. An option named in a signal row inherits its beneficiaries, default, implementation notes, pitfalls, and example from its bundle here. Individual alternatives are not automatically enabled together. For example, a hold alternative and a toggle alternative address different input needs.

Beneficiary shorthand: **V** vision; **H** hearing; **M** motor/mobility/fatigue; **C** cognition/learning/memory; **N** neurodivergent/sensory; **P** photosensitivity; **F** phobia; **S** simulation sickness/vestibular; **T** mental health/stress; **U** situational access. These identify possible beneficiaries, not diagnoses or guaranteed outcomes.

### Input, timing, action, and failure

| ID / tier | Options and purpose | Helps / context | Recommended default | Engine-neutral implementation and pitfall | Documented pattern |
| --- | --- | --- | --- | --- | --- |
| I01 / B architecture, C controls | Full action remapping, duplicate bindings, device choice, sensitivity/deadzones, inversion, vibration/trigger resistance strength. | M, S, U; any active input, analog control, or haptics. | Existing layout; accessible alternative bindings available immediately; haptics optional. | Bind actions rather than physical keys in gameplay and prompts. Include UI, vehicles, menus, minigames, and chord conflicts. Do not bind two indispensable actions to one input accidentally. | Halo documents input/presentation settings; GAG recommends remapping and sensitivity. [HALO] [GAG] |
| I02 / C | Hold-to-toggle, press-to-latch, auto-repeat, mash-to-single-press, mash-to-hold, automatic QTE completion, QTE skip. | M, C, U; sustained pressure, rapid repetition, surprise prompts. | Intended interaction; expose alternatives before it occurs. | Implement an action state machine with cancel/reset on pause and transitions. Replacing mashing only with a long hold still excludes some players. | TLOU II documents hold/toggle and repeated-input alternatives. [TLOU2] |
| I03 / C; A switch scanning | Sequential instead of simultaneous inputs, chord grace, one-stick/one-handed layouts, keyboard-only/mouse-only paths; switch scanning or dwell. | M, U; dual sticks, chords, dragging, continuous cursor control. | Intended layout; alternate paths opt-in. | Ensure every essential action is reachable, including settings. An apparently one-stick game can still require a click on the unused stick. | Botany Manor documents stop-and-look single-stick play. [BOTANY] |
| I04 / C | Timing-window scale, input buffering, adjustable repeat acceptance/debounce, action slowdown, turn-based alternative, pause-to-plan. | M, C, V, U; parries, perfect hits, rhythm, narrow reaction windows. | Intended timings; assist opt-in. | Adjust the acceptance window independently of damage and health. Keep game, audio, animation, and UI clocks coherent. Do not turn latency compensation into unintended double actions. | Prince of Persia documents parry/dodge-window tuning. [POP] |
| I05 / C | Aim magnetism, target snapping/locking/cycling, aim stabilization, recoil/sway reduction, auto-fire, auto-reload, auto-pickup, camera-follow assist. | M, V, C; precision targeting or multitasking. | Intended targeting; assists independently selectable. | Expose strength/range and clear target feedback. Do not force camera movement on players using motion-comfort options. | Gears 5 documents target lock in specified PvE modes/difficulties. [GEARS] |
| I06 / C | Auto-traversal, ledge protection, extra/infinite jumps, dash assistance, slower hazards, float mode, no stamina, traversal/room skip. | M, V, C; jumps, climbing, platform timing. | Intended traversal; assists opt-in. | Preserve movement state and story flags across skips; test fall recovery and no-stamina combinations. Invulnerability alone does not cross a gap. | The Forest Cathedral documents spike removal and Float Mode; Jusant documents simplified climbing and no-stamina options. [FOREST] [JUSANT] |
| I07 / C | Independent enemy/environment/fall/self damage; invulnerability; enemy aggression, speed, health, count; infinite ammo/stamina; resource multipliers. | M, C, T, U; combat or survival pressure. | Intended tuning; granular opt-in. | Separate failure channels and explain each. Infinite health must not leave the player trapped in an endless death pit or an unwinnable scripted sequence. | No Man's Sky's Waypoint framework separates survival, combat, economy, and death consequences. [NMS] |
| I08 / C | Frequent checkpoints, suspend/resume, manual saves, retry near failure, retain inventory/currency, remove corpse run, permadeath opt-out, encounter/segment skip. | M, C, T, U; lost progress or endurance demands. | Intended authored cadence plus robust interruption recovery; assists opt-in. | Save the actual pending turn/scene where feasible. Avoid changing only future runs when a current save is already blocked. | Dead Cells documents Continue Mode; publisher support confirms an Assist Mode menu. [DEADCELLS] [DEADCELLS-SUPPORT] |
| I09 / C | Extend/remove gameplay timers, pause timers in menus, untimed decisions, longer ready/respawn windows, persistent/replayable notifications. | M, C, V, H, T, U; countdowns, disappearing text, timed choices. | Intended gameplay timing; nonessential UI should wait for the player. | Distinguish real-time gameplay clocks, UI expiry, and shared-server clocks. Extending a timer is insufficient if reading narration consumes it. | Jackbox Party Pack 10 documents extended/no timers. [JACKBOX] |
| I10 / C | Practice without failure, repeatable tutorials, move demonstrations, input history, combo simplification/macros, auto-combos, command shortcuts. | M, C, V; complex sequences or skills learned through experimentation. | Tutorial available; practice optional; command simplification opt-in. | Explain exact supported modes and retain manual control. Do not promise a simplified scheme includes every original move. | Street Fighter 6 documents Classic, Modern, and offline-restricted Dynamic controls. [SF6] |

### Presentation, information, and memory

| ID / tier | Options and purpose | Helps / context | Recommended default | Engine-neutral implementation and pitfall | Documented pattern |
| --- | --- | --- | --- | --- | --- |
| V01 / B structure, C customization | UI/text scale, reflow, spacing, plain font, contrast/backplates, cursor/crosshair scale and shape, magnification. | V, C, U; labels, text, aiming markers. | Readable default; player style preference remembered. | Scale tooltips, prompts, inventory, error dialogs and symbols, not just headings. Offer reading preferences without claiming one “dyslexia font” works for everyone. | XAG 101 documents configurable text presentation. [X101] |
| V02 / B encoding, C palette | Symbols, patterns, labels, shape/texture differences, per-team/per-state colors; optional color filters. | V, C; colors encode hazards, ownership, matches, rarity, status. | Redundant non-color encoding on. | Replace meaning at the element level; test overlapping states. A global tint cannot restore a missing symbol or distinguish identical silhouettes. | Halo describes configurable friend/enemy presentation; Princess Farmer documents non-color-only board information. [HALO] [FARMER] |
| V03 / C | High-contrast gameplay, outlines/silhouettes, hazard edges, aim markers, object highlights, selectable highlight strength. | V, C; clutter, dark scenes, fine targets. | Intended appearance; enhanced clarity opt-in. | Keep target identity and occlusion rules coherent. A glowing outline is a visual aid, not nonvisual navigation. | Prince of Persia documents high-contrast gameplay and interactable highlighting. [POP] |
| V04 / C | Screen shake/bob/roll/sway, blur, speed lines, zoom/FOV kicks, camera auto-centering, parallax and background animation controls. | S, N, V, U; moving or decorated views. | Avoid gratuitous motion; conservative reduced setting where compatible; individual Off controls. | Apply in menus, cutscenes, vehicles, transitions and death animations. Camera assist may be desirable to one player and harmful to another. | Halo offers separate effect strengths; XAG 117 discusses additive motion and moving UI. [HALO] [X117] |
| V05 / B review, C reduction | Remove/replace strobing, lightning, muzzle flashes, hit flashes, bright transitions and hazardous patterns; safe effect intensity. | P, N, V; any rendered visuals, especially rapid changes. | Avoid known hazardous output initially; safe reductions active before exposure. | Measure captured sequences and combinations; replace cues with static equivalents. Do not call “reduced flashing” seizure-proof or equate a frequency threshold with complete safety. | Celeste's changelog documents photosensitive-mode fixes, including menu flashing. [CELESTE] [X118] |
| V06 / C | Minimal/custom HUD, guidance paint/trail/highlight Off, on-demand reveal, notification filtering, adjustable hint frequency. | N, C, V, U; excessive overlays or intrusive guidance. | Intended guidance; clear individual controls available. | Separate navigation cues, interaction cues, combat warnings and decorative guidance. Keep an accessible way to restore them. | God of War Ragnarök documents individual HUD visibility controls. [GOW] |
| A01 / B audio access, C mixes | Separate music/dialogue/SFX/ambience/UI/chat, mono, dynamic-range compression/night mix, harsh-sound replacement, volume previews. | H, N, T, U; mixed audio, loud peaks, directional sound. | Conservative startup output; no surprise previews; respect system mute. | Preserve critical cues when muting noncritical effects; label whether a control affects combat cues. Mono needs visual equivalents for lost direction. | APICO documents a separate option to disable buzzing sounds. [APICO] |
| A02 / C | Speech subtitles, speaker labels/direction, caption non-speech events, scalable backplates and duration; lyric text where relevant. | H, C, U; spoken dialogue, sound events, music clues. | Subtitles/captions available before sound; recommend speech subtitles On with override. | Caption meaning, speaker and useful direction without false location. Include gameplay barks, radio and interactive videos. | TLOU II documents story/combat subtitles and direction; XAG 104 treats subtitles and captions distinctly. [TLOU2] [X104] |
| A03 / C; A comprehensive sonification | Directional visual sound indicators, danger captions, optional tactile cues, distinct nonvisual interaction/obstacle cues, audio radar. | H, V, M; positional audio or visually conveyed world state. | Essential redundancy by design; enhanced cue layers opt-in. | Rate-limit clutter; expose range/type/priority. Do not use one vibration pattern for incompatible meanings or require haptics to replace hearing. | Gears 5 documents a navigation ping; XAG 103 supports multiple cue channels. [GEARS] [X103] |
| A04 / B UI semantics, C narration, A spatial access | Screen-reader integration/self-voicing, narration verbosity/rate, read-current-state, audio description, nonvisual world navigation. | V, C, U; UI, text, cinematic or spatial meaning. | Honor applicable system preferences; otherwise offer accessible enablement before menu barriers. | Feed names/values/focus/state rather than screenshots. Keep character voices and narration distinct. Full spatial exploration requires more than menu TTS. | Ren'Py documents focusable self-voicing and alternative text; Forza documents driving-specific audio cues. [RENPY] [FORZA] |
| C01 / C | Current objective, persistent quest/event log, map markers, screenshot/text notes, completed/blocked route tracking, return-session recap. | C, V, U; exploration, quests, multi-session stories. | Basic objective available; additional tracking and spoiler detail opt-in. | Store facts actually learned, accessible text for image notes, and timestamps. Never reveal an unvisited solution unintentionally. | Prince of Persia explicitly addresses remembering blocked paths through map-pinned screenshots. [POP] |
| C02 / C | Layered hints, explicit next step, optional solution/spoiler, puzzle skip, objective route assistance; hint delay and Off. | C, V, T, U; puzzles, obscure goals, spatial navigation. | Intended discovery; hints player-requested unless configured. | Separate “what to do” from “how to get there.” Skips must grant required items and completion events consistently. | TLOU II documents puzzle skip and navigation/traversal assistance. [TLOU2] |
| C03 / C | Inventory search/filter/sort, favorites, recipe pinning, missing-material list, craft-from-storage, batch crafting, auto-equip. | M, C, V; many items, recipes or repeated transactions. | Clear ordinary inventory; convenience assists opt-in. | Expose item names/quantities/status to narration and focus; retain focus after changing a stack. Avoid accidental sale/crafting. | God of War Ragnarök documents auto-equip and automatic pickups. [GOW] |
| C04 / C | Glossary, plain-language rules, expandable explanations, worked examples, forecast/outcome preview, undo where meaningful. | C, V, U; dense statistics, strategy, unusual vocabulary. | Explain before irreversible choice; optional detail layers. | Present underlying numbers and meaning accessibly; distinguish known information from predictions. Undo must not leak hidden opponents' state. | APICO documents on-demand guidebooks and always-expanded tooltips. [APICO] |
| C05 / C | Dialogue backlog/replay, manual text advance, adjustable reveal speed/instant text, skip read text, repeat instruction, chapter summary. | C, V, H, M, U; reading, partial voiceover, story continuity. | Manual advance; no forced text expiry; preserve user autoplay preference. | Keep backlog navigable and stop narration before advancing automatically. Separate skip animation from skipping decisions. | Prince of Persia documents a recent-conversation log; Ren'Py documents reading text through self-voicing. [POP] [RENPY] |

### Content, social play, vehicles, touch, and XR

| ID / tier | Options and purpose | Helps / context | Recommended default | Engine-neutral implementation and pitfall | Documented pattern |
| --- | --- | --- | --- | --- | --- |
| F01 / C | Spoiler-controlled content warnings, scene-specific warnings, text alternatives, sensitive-scene skip, jump-scare reduction, gore/body-detail reduction. | F, T, N, U; disturbing content or abrupt scares. | Offer content preferences before exposure; warnings available; filters player-selected. | Tag cutscenes, audio, text, thumbnails and gameplay separately. A warning is not removal; an invincible enemy can remain frightening. | SOMA documents retained horror atmosphere in Safe Mode; Dead Space documents graphic-content disclosure. [SOMA] [DEADSPACE-CONTENT] |
| F02 / C warning/filter hooks; A extensive variants | Category-specific replacements for spiders/insects/snakes/clowns/needles/blood/deep water/heights/hole clusters; preview-free selection. | F, T; matching assets, descriptions, behaviors or environments. | Category disclosure before exposure; substitutions opt-in; never auto-show trigger previews. | Replace shape, motion, sound, shadows, icons and names as needed; preserve legible hitboxes and mechanics. Do not promise all phobias are covered. | Grounded documents graded spider-appearance changes without changing difficulty. [GROUNDED] |
| S01 / C; A network/service integration | Accessible text/voice/pings, incoming speech-to-text, outgoing text-to-speech, quick phrases, chat history, readable and narratable invite/lobby UI. | H, V, speech disability, M, C; social communication. | No requirement to speak; clear controls and consent for microphone/transcription; preserve platform preferences. | Treat lobby, authentication and communication settings as part of access. Transcription language/accuracy and online dependencies must be explicit. | XAG 119 and 120 cover communication channels and the path to them. [X119] [X120] |
| S02 / C | Per-player profiles, independent input/HUD/mix, teammate outline/name, co-pilot/shared control, reconnect, host-controlled ready/pause. | All, U; co-op, local play, split screen. | Each player owns personal settings; shared rules are explicit. | Never overwrite another profile on controller handoff. Provide nonvoice coordination and readable per-viewport captions. | Jackbox documents player-facing timers and reduced-motion settings; shared-control expansion is a recommendation. [JACKBOX] |
| S03 / C | Mute/block/report, chat modality choice, per-channel notification controls, accessible moderation, private/practice sessions. | T, C, H, speech disability, U; online chat/UGC. | Accessible protective controls available immediately; no automatic public microphone activation. | Ensure reporting is accessible during and after a match; retained evidence should not require replaying harmful media. | Jackbox documents moderation, profanity filtering and room-code controls. [JACKBOX8] |
| D01 / C; A nonvisual driving | Auto-steer/brake/throttle/gearshift, racing line, collision recovery, rewind, flight stabilization, auto-level, axis remapping, horizon reference. | M, V, C, S; driving/flying/vehicle movement. | Intended handling; independent assists opt-in; reduce gratuitous camera effects. | Separate driving aids from cockpit presentation. Test menus and dismount transitions; do not assume every flight game supports every racing aid. | Forza documents One Touch Driving; Squadrons documents remapping and several device types. [FORZA] [SQUADRONS] |
| R01 / C; A alternate interaction systems | Large/movable touch controls, tap-select/tap-place instead of drag, gesture alternatives, orientation choice, switch/voice/keyboard/controller compatibility. | M, V, U; touch, swipes, pinch, tilt. | Generous targets and cancellation; intended layout with reachable alternatives. | Test OS assistive interaction and zoom. Canvas rendering alone may expose no usable semantic controls. | XAG 107 explicitly covers mobile assistive technologies and non-touch alternatives. [X107] |
| R02 / C | Seated mode, height/reach calibration, virtual crouch, distant grab, one-hand action parity, controller alternatives to gestures, recenter. | M, V, U; VR/AR bodily demands. | Accessible calibration before interaction; seated option immediately available. | Test every essential object and menu at calibrated reach. Height scaling alone does not replace bending or two-handed tasks. | Cosmonious High's FAQ documents one-handed mode and height-adjustable surfaces. [OWL] |
| R03 / C | Teleport/snap/smooth movement choice, turn-angle/speed, vignette, fixed horizon, reduced vehicle acceleration, stationary interaction, caption anchoring. | S, V, H, N; XR locomotion and spatial media. | Offer comfort choices before motion; do not prescribe one universal comfortable mode. | Maintain safe recentering and legible captions without forcing repeated neck turns. Never disable platform boundary safety. | XAUR documents multimodal and orientation needs; No Man's Sky documents VR comfort settings. [XAUR] [NMS-VR] |
| E01 / C | Read-aloud/repeat instructions, symbols plus words, untimed learning, adjustable scaffolding, adult/teacher configuration, alternatives to physical execution. | C, V, H, M, children and U; education/early reading. | Clear instructions; no time pressure unless it is the learning target; optional scaffolding. | Keep learning objective separate from input speed. Record the answer, not a child's ability to drag accurately. Avoid equating disability with age or low skill. | Minecraft Education documents Immersive Reader; PBS describes Duck Dash's speed/background/input options. [EDUCATION] [PBS] |

## 3. Genre routes and evidence

Every section inherits B01–B06. “Skip” means no matching signal exists, including optional modes and minigames; it does not mean the option is inherently inappropriate for that genre. Recommendations are deliberately more granular than a single Easy mode.

### G01 — First-person shooter / third-person shooter

- **Most relevant:** I01–I05, I07–I09, V02–V05, A01–A03; C01/C02 for campaign navigation; S01–S03 for online play. A04 nonvisual targeting/navigation is Advanced beyond UI narration.
- **Reported barrier:** Halo's team reports players found holding forward to sprint difficult or painful, and reports colorblind-player disadvantage from earlier red/blue distinctions. These are feedback findings, not a claim that all shooter players need auto-run. [HALO-FEEDBACK]
- **Strong shipped patterns:** Gears 5's target lock; Halo Infinite's configurable team/reticle presentation and separate visual-effect settings. Gears' lock is documented for particular PvE difficulties/modes, so do not promise unrestricted multiplayer auto-aim. [GEARS] [HALO]
- **Skip when absent:** platforming jump assists in a ground-only shooter; inventory automation without inventory; dialogue backlog without dialogue; phobia substitutions without matching content.
- **Design inference:** headshots, recoil, reload timing, simultaneous movement/aiming and off-screen threats deserve separate checks. A first-person view also triggers comfort even if shooting is trivial.
- **Verify:** clear a mandatory fight, traverse a doorway/stair, reload and change weapons with one-hand input; repeat muted and with effects reduced; test campaign and PvP separately.

### G02 — Action-adventure / hack-and-slash / soulslike

- **Most relevant:** I02/I04/I05/I07/I08/I10; V03–V05; A02/A03; C01/C02. Include no-stamina, independent damage channels and retry/skip options when those systems exist.
- **Reported barrier:** CIPT's Elden Ring account describes inaccessible directional information, missing subtitle customization, quest-memory demands and reliance on outside explanations. Treat this as the reviewer's documented launch-period experience. [ELDEN-REVIEW]
- **Strong shipped patterns:** God of War Ragnarök documents miniboss checkpoints, navigation and presentation aids; Prince of Persia: The Lost Crown documents independent combat parameters. Neither is evidence that all “soulslike” barriers disappear. [GOW] [POP]
- **Skip when absent:** firearm ammo assists in melee-only combat, random-encounter controls in seamless combat, jump assists when no jump exists.
- **Design inference:** “soulslike” must not veto pause, parry-window adjustment, corpse-run relief or enemy-damage Off. Preserve intended defaults while making separate assists available without save/content penalties.
- **Verify:** boss phase transitions, environmental hazards, grabs, stamina exhaustion, recovery after death and required precision traversal under combined assists.

### G03 — 2D / 3D platformer

- **Most relevant:** I01–I04/I06/I08/I09; V03–V05; A03; optional traversal descriptions and spatial audio under A04.
- **Reported barrier:** The Forest Cathedral's developer case study identifies analog-only platforming, rapid presses, reaction demands and journal legibility during its accessibility review process. [FOREST-DEV]
- **Strong shipped patterns:** The Forest Cathedral's Float Mode and spike removal; Celeste's documented Dash Assist and screen-shake controls. Celeste also documents an assist file-slot marker: do not copy that marker into this skill because the brief forbids marking saves. [FOREST] [CELESTE]
- **Skip when absent:** aiming and recoil in a jump-only game; map-backtracking guidance in a linear room sequence; camera rotation controls in a fixed 2D view.
- **Design inference:** configurable coyote time/jump buffering, extra jumps, slower hazards and ledge guards solve different demands. In 3D, add depth/distance cues and camera control checks.
- **Verify:** moving platforms, ladders, air dashes, wall jumps, one-off chase sequences and puzzle switches; no invisible required timing remains after traversal assist.

### G04 — Metroidvania

- **Most relevant:** platformer/action routes plus C01/C02/C05; readable multi-level maps, custom pins and accessible blocked-route records.
- **Documented barrier:** Ubisoft explicitly describes remembering/visualizing blocked paths and missing powers as a genre barrier. This is a developer research/design account, not a quantified player survey. [POP]
- **Strong shipped pattern:** Prince of Persia: The Lost Crown offers screenshot map pins, optional guided routes and platforming skips. The route-memory lesson is more distinctive than merely lowering combat difficulty. [POP]
- **Skip when absent:** firearm precision, crafting automation, online chat and turn timers; do not load JRPG command-menu guidance because the game has stats.
- **Design inference:** mark why a route is blocked in text as well as imagery, allow “show newly reachable places,” and provide recap after acquiring a traversal ability. Keep spoiler depth player-controlled.
- **Verify:** return after a long absence, identify a previously blocked route using a new ability, and complete progression after a skipped traversal challenge.

### G05 — JRPG / turn-based RPG / tactics

- **Most relevant:** V01/V02, A02/A04, C01–C05, I03/I08/I09. Add I04 only for active-time gauges, timed hits, parries or real-time exploration hazards.
- **Reported barrier:** CIPT's Baldur's Gate 3 review finds turn-based combat removes reaction demands for its reviewer but identifies confusing/visually complex maps. Do not infer that being turn-based makes dense UI or navigation accessible. [BG3-REVIEW]
- **Strong shipped patterns:** FINAL FANTASY Pixel Remaster's documented auto-battle, encounter disabling and experience multipliers; Baldur's Gate 3's reviewed subtitle presentation. The cited Pixel Remaster announcement concerns PS4/Switch; verify other releases independently. [FF-PIXEL] [BG3-REVIEW]
- **Skip when absent:** aim stabilization, vehicle steering, jump buffering and real-time combat slowdown. Preserve a route for a rare timed minigame instead of ignoring it.
- **Design inference:** offer stable focus, nested explanations, searchable ability lists, attack-area/turn-order narration, decision review and confirmation before ending a turn. Separate animation speed from decision time.
- **Verify:** inspect a unit, compare abilities, choose a grid target, read predicted consequences, cancel, commit and resume mid-battle using each supported input.

### G06 — Western / open-world RPG

- **Most relevant:** C01–C05, V01/V03/V06, A02/A04, I08; add action/shooter/stealth/vehicle routes from actual play.
- **Reported barrier:** Elden Ring's cited reviewer asks for an optional on-screen quest log rather than physical notes in an unpausable world; BG3's cited reviewer identifies map legibility problems. These support memory and route checks, not a universal mandatory waypoint. [ELDEN-REVIEW] [BG3-REVIEW]
- **Strong shipped pattern:** Assassin's Creed Valhalla separates fighting, stealth and exploration tuning, with a guaranteed-assassination option. Its controls article also discusses alternatives to free-cursor menu navigation. [VALHALLA]
- **Skip when absent:** parry helpers in a dialogue-led RPG, jump assists in click-to-move exploration, chat transcription in wholly offline play.
- **Design inference:** recap current commitments, locations, characters and last actions; offer adjustable guidance, including Off. Keep image maps and manuscript text accessible through alternate representations.
- **Verify:** return to an old save, identify a quest's next action and location, manage a full inventory, then complete any required noncombat minigame.

### G07 — MMO

- **Most relevant:** I01–I05/I09/I10; V01–V06; A01–A04; C01/C03/C04; S01–S03. Distinguish personal presentation from encounter/server rules.
- **Reported barrier:** Blizzard's director, quoted in PC Gamer's report, acknowledges feedback that repeatedly pressing the Single-Button Assistant can be fatiguing. This illustrates that fewer distinct buttons does not mean fewer total actions. [WOW-EXPERIENCE]
- **Strong shipped pattern:** World of Warcraft documents Assisted Highlight and a Single-Button Assistant selecting recommended damage abilities; the official account includes an added global cooldown. It is not a documented full automation of healing, movement or every raid mechanic. [WOW]
- **Skip when absent:** precision aim in tab-target combat; crafting controls in an MMO without crafting; jumping aids without platform traversal. Never skip chat because PvE is the main activity.
- **Design inference:** independent allied/enemy effect density, persistent encounter cues, action-queue alternatives, accessible group finding and role/status labels matter. Do not require unauthorized combat mods to close foundational gaps.
- **Verify:** log in, select a character, join a group, communicate without speech, read a combat warning and manage loot; test simultaneous VFX and chat rather than an empty training room.

### G08 — Survival / crafting

- **Most relevant:** I02/I05/I07–I09, C01/C03/C04, V03, A03, F02 if creatures/content apply; multiplayer routes if shared worlds exist.
- **Documented barrier framing:** XAG 108 explicitly treats depletion speed, resource availability and reminders as separate adjustable survival demands. This is guideline analysis rather than a genre-wide player survey. [X108]
- **Strong shipped patterns:** No Man's Sky Waypoint exposes separate survival/resource/combat/death settings; Grounded documents graded arachnophobia presentation. Those solve different barriers and should be independently selectable. [NMS] [GROUNDED]
- **Skip when absent:** starvation sliders in a crafting toy with no needs meters; spider replacements without spiders; shooter targeting in noncombat crafting.
- **Design inference:** queue/batch crafting, pin recipes, show accessible missing-material lists, reduce collection presses, pause offline time costs, and allow scarce resources without mandatory enemy damage if the player chooses.
- **Verify:** craft a required item from scattered stores, survive an interruption and a depleted-meter state, recover after death, and progress with enemies or hazards disabled.

### G09 — Horror, including jump scares, gore and phobia content

- **Most relevant:** F01/F02, V03–V05, A01–A03, I07–I09, C02; actual camera, stealth, aiming and puzzle routes still apply.
- **Reported barrier:** Frictional describes players wanting SOMA's narrative but being unable to continue because of monsters; community response to a mod informed Safe Mode. [SOMA]
- **Strong shipped patterns:** SOMA changes monster behavior to remove the death threat while retaining horror atmosphere. Grounded demonstrates spider-shape substitution. Dead Space's official page documents graphic-content disclosure; **the exact completeness of its hide-content coverage is UNVERIFIED in this research**, so do not call it a universal gore removal mode. [SOMA] [GROUNDED] [DEADSPACE-CONTENT]
- **Skip when absent:** every unrelated phobia control; combat invulnerability in a purely observational story. “Horror” alone does not tell which categories occur.
- **Design inference:** separate jump-scare visual intensity, sharp audio stings, pursuit threat, gore, disturbing themes and individual phobias. Offer warnings without forced thumbnails or surprise previews.
- **Verify:** inspect the whole content path: startup, settings thumbnails, loading, narration, sounds, enemy death, reflections and DLC; confirm a replacement does not retain a triggering animation or sound.

### G10 — Stealth

- **Most relevant:** I02/I04/I05/I07/I09, V02/V03, A03, C01/C02; detection speed, detection state and chase recovery deserve explicit settings.
- **Reported barrier:** the Elden Ring reviewer describes not realizing an enemy had detected them and being unable to determine relevant sound direction. Although not a pure stealth game, this is direct evidence for the detection mechanic route. [ELDEN-REVIEW]
- **Strong shipped patterns:** TLOU II documents invisible-while-prone options; Valhalla documents guaranteed assassination. These address concealment and execution respectively; neither substitutes for knowing a guard's alert state. [TLOU2] [VALHALLA]
- **Skip when absent:** weapon recoil for noncombat stealth, loot batch crafting without inventory, permanent invisibility when merely clearer feedback is requested.
- **Design inference:** offer multimodal suspicion/detection stages, optionally slower buildup, grace after discovery, no-fail stealth or skip for mandatory stealth sections; keep concealment rules explainable.
- **Verify:** distinguish unaware/suspicious/searching/detected states without audio or color alone; recover after failed concealment; test scripted unavoidable discovery.

### G11 — Racing / driving

- **Most relevant:** D01, I01/I03/I08/I09, V01/V02/V04/V05, A01/A03/A04; tactile force strength and independent steering/throttle/brake matter.
- **Documented player involvement:** Forza's developers describe testing with players with mobility and stamina disabilities and collaborating with blind/low-vision players on driving cues. [FORZA]
- **Strong shipped pattern:** Forza Motorsport documents supplemental blind-driving cues and independently combined One Touch Driving assists. Its article warns of a learning curve; this is not a promise of instant nonvisual driving. [FORZA]
- **Skip when absent:** jump/platforming assistance, dialogue backlog, hunger/crafting, spatial first-person walking navigation.
- **Design inference:** optional chase/cockpit view, stable horizon, steering response, auto-shifting, turn cues and recovery/rewind need separate controls. “Automatic gears” does not remove simultaneous steering and throttle.
- **Verify:** start a race, navigate pit entry, recover from a collision and complete a lap. Check cue previews as well as runtime: official patch notes record fixes to blind-driving preview and pit cues. [FORZA-FIXES]

### G12 — Flight / space simulation

- **Most relevant:** D01, I01/I03/I05, V01–V05, A01–A04, C01/C04. Add R02/R03 for VR; long-session save routes for persistent simulation.
- **Barrier analysis:** simultaneous pitch/yaw/roll/throttle, small instruments, target acquisition and horizon motion are mechanic-based design inferences. **A representative flight-sim-specific disability player study was not verified here.**
- **Strong shipped pattern:** STAR WARS: Squadrons documents remapping across menus/hangar/flight, keyboard/mouse/joystick/HOTAS support, off-screen indicators, ship outlines and communication alternatives. This is a combat-flight example, not evidence for every civilian-sim feature. [SQUADRONS]
- **Skip when absent:** ground jump assistance, cockpit comfort if using a static tactical view, combat auto-targeting in civilian flight.
- **Design inference:** axis curves/deadzones, auto-level/stability, autopilot for selected tasks, simplified systems management, instrument magnification and objective bearing readout can reduce distinct demands. Keep authentic simulation available.
- **Verify:** remap flight and menus, acquire a waypoint, recover orientation, dock/land and suspend; test stalls, rapid rotations and vehicle-induced camera motion with comfort options.

### G13 — Sports

- **Most relevant:** I01–I05/I09/I10, V01–V05, A01/A03/A04, C04; S01–S03 for online/team play; R02 where bodily tracking is used.
- **Reported barrier:** a Madden NFL 24 forum report says receiver button callouts stopped functioning during passing plays. This is a historical report for a specific platform/build, not proof of a current unresolved defect. [MADDEN-BUG]
- **Strong shipped pattern:** EA documents Madden NFL 24 menu narration, open-receiver button callouts and defensive assists. The guide is explicitly scoped to its English-US experience; subtitles are described only for particular cutscenes. [MADDEN]
- **Skip when absent:** opponent navigation sonification for a static management screen, stamina toggles where no stamina system exists, story recaps for a standalone match.
- **Design inference:** simplify play selection, kick/swing timing and complex gestures; add stable ball/player indicators, play previews and drills without failure. Keep team colors separable from uniforms and backgrounds.
- **Verify:** follow the ball, identify possession and players, select a play, make a timed action and understand referee/whistle decisions without relying on one channel.

### G14 — Fighting games

- **Most relevant:** I01–I04/I09/I10, V02–V05, A01/A03/A04; narrated move lists, practice and readable frame/command data; online routes if present.
- **Barrier analysis:** command complexity, simultaneous presses, rapid combos, visual spacing and high/low attacks are mechanic-based demands; input reduction and information access should be separate routes. GAG specifically recommends alternatives to simultaneous actions and complex controls. [GAG]
- **Strong shipped patterns:** Street Fighter 6 documents simplified Modern inputs and Dynamic controls with offline restrictions. Capcom also documents improved audio accessibility; the press release alone does not establish full menu narration or total blind access. [SF6] [SF6-AUDIO]
- **Skip when absent:** world quest logs in a match-only fighter, resource gathering, free-camera aim controls in a fixed arena.
- **Design inference:** sequential/chord alternatives, adjustable practice speed, auto-combos, replayable demonstrations and readable nonvisual spacing/state cues are distinct features. Do not assume a simpler scheme has identical move availability.
- **Verify:** menu-to-match access, character selection, action canceling, side swaps, opponent distance, attack level and training feedback; test accessibility in every supported online/offline mode.

### G15 — Rhythm / music games

- **Most relevant:** I02–I04/I08–I10, V02/V04/V05, A01–A03; latency calibration, visual/haptic beat, alternative lanes and no-fail completion.
- **Reported barrier:** Laura Kate Dale describes benefiting from Hi-Fi Rush's beat reinforcement while reporting other players found the visual density distracting. She also distinguishes single-button action selection from relief from beat timing. [HIFI-REVIEW]
- **Strong shipped pattern:** Hi-Fi RUSH documents Auto-Action and Single Key rhythm options. Use it as an example of reducing action complexity, not as proof that all timing demands disappear. [HIFI]
- **Skip when absent:** quest markers, shooting magnetism, survival systems. Do not load VR bodily guidance for a flat-screen keyboard rhythm game.
- **Design inference:** separate timing tolerance, song speed, visual travel speed, calibration, number of lanes, miss penalties and background motion. A simple stationary metronome may help where animated whole-world cues overwhelm.
- **Verify:** play muted, vary audio/display latency, miss deliberately without losing access to the song/story, and test quiet/simple visual cues. Keep flashes safe even when beat feedback is enabled.

### G16 — Puzzle, including timed and color-matching puzzles

- **Most relevant:** V01–V03, A03/A04, C01/C02/C04, I03/I09; I04 only when execution timing exists; I08 for long/irreversible puzzle states.
- **Reported barrier:** CIPT's Life is Strange review describes hidden-object search and precise picture-focusing as visual barriers, despite the overall narrative/puzzle framing. [LIS1]
- **Strong shipped patterns:** Botany Manor documents untimed puzzles, a plain-text clue overlay and camera comfort options. Princess Farmer documents at-your-own-pace puzzle play and non-color-only board information. [BOTANY] [FARMER]
- **Skip when absent:** invulnerability and ammo in a purely logical puzzle, FOV in a static board, timing assists when decisions and execution are already untimed.
- **Design inference:** support symbols/labels as well as color; offer hint levels, undo, reset-part-of-puzzle, alternate sensory clues and optional solution/skip. Preserve the puzzle question in an accessible representation.
- **Verify:** solve every mandatory puzzle with color removed and audio muted, using discrete input; identify which puzzles still depend on inaccessible image details or spatial precision.

### G17 — Match-3 / casual / mobile / touch

- **Most relevant:** R01, I02/I03/I08/I09, V01/V02/V04/V05, C02/C04, A04 for semantic boards. “Mobile” is a platform and “casual” a market label, not a reason to stop at easy difficulty.
- **Reported barrier:** a self-identified colorblind Candy Crush player reports problems distinguishing blue/purple shades. Another user's grayscale suggestion in that thread is not evidence of a complete fix. [KING-FEEDBACK]
- **Developer experience:** Princess Farmer's publisher describes a designer's hand injury informing the one-handed design. This supports inspecting repeated action and reach rather than assuming a casual game has low motor demands. [FARMER-DEV]
- **Strong shipped patterns:** Princess Farmer documents multiple play styles, one-stick interactions and the same story/rewards across styles; Candy Crush Friends Saga's official community announcement documents reduced scrolling/shake. Do not transfer features between the two Candy Crush titles. [FARMER] [KING-MOTION]
- **Skip when absent:** camera/FOV for static boards; jump assists without movement; story backlog without story; VR-only controls on phones.
- **Design inference:** tap-select/tap-destination, large stationary targets, non-color special-piece states, one-handed reach, pause and persistent instructions matter more than a blanket color filter.
- **Verify:** OS screen-reader/switch navigation, board coordinates and state narration, interrupted sessions, target resizing and every special piece rather than only base pieces.

### G18 — Idle / incremental / clicker

- **Most relevant:** I02/I03/I08, V01/V02/V04, A04, C03/C04; I09 if event or prestige timers impose actual deadlines.
- **Reported barrier:** a blind contributor to A Dark Room's repository describes unlabeled clickable elements, dynamic-content verbosity and map access issues. This is a browser-version issue discussion; do not generalize iOS behavior from it. [ADR-ISSUE]
- **Strong shipped pattern:** Trimps maintains a ScreenReader.html variant with focusable tooltip access and alternative info-button behavior. The repository also contains accessibility issue reports, so this is a concrete implementation example, not a verified bug-free benchmark. [TRIMPS] [TRIMPS-ISSUES]
- **Skip when absent:** twitch slowdown, parry timing, jump assistance and free-camera comfort. Do not add immortality if there is no failure/damage system.
- **Design inference:** hold/toggle automation from the first repetitive task, buy-many/buy-max, quiet state summaries, nonexpiring offline rewards, save export and prestige confirmation. Do not require hours of clicking to unlock an accessibility automation.
- **Verify:** reach the first automation without rapid clicking, understand growth rates, reload after time away, stop noisy announcements and avoid accidental prestige/reset.

### G19 — Strategy: RTS / 4X / grand strategy / tower defense / auto-battler

- **Shared priorities:** V01/V02, A03/A04, C01/C03/C04, I03/I08/I09; readable tooltips, queued commands, forecasts, map alternatives and stable focus.
- **Reported barrier:** a Civilization player describing limited vision reports that even enlarged UI remained difficult to read. This is an individual account, not an audit of every Civilization version. [CIV-FEEDBACK]
- **Strong shipped pattern:** Age of Empires IV documents story difficulty, contrast, text size, remapping and camera-pan speed. Its official guide explicitly says its narration feature does not narrate the in-game HUD: menu TTS must not be routed as complete strategy-state access. [AOE]
- **RTS inference:** multi-front events and actions-per-minute trigger pause-to-command, slower simulation, queues, selection helpers and optional automation. Only apply shared time changes in a compatible session design.
- **4X inference and shipped pattern:** decision comparisons, long turns, diplomatic history and end-turn confirmation matter; real-time slowdown is irrelevant if every decision already waits indefinitely. Civilization VII's official February 2025 patch notes document added text-to-speech narration; the note does not establish complete keyboard, console or gameplay narration coverage. [CIV7]
- **Grand-strategy inference and shipped pattern:** independently pause on chosen events, notification categories, readable nested tooltips, chronological logs and explicit rule dependencies matter. Crusader Kings III's publisher-controlled Steam listing identifies adjustable text size, mouse-only play, saving at any time and play at the player's pace. These are publisher declarations to verify, not an independent audit. [CK3-STORE]
- **Tower-defense inference and shipped pattern:** adjustable wave pace, manually start next wave, construction while paused, accessible grid placement and readable range coverage matter. The published jam hybrid core_error documents a colorblind mode adding tile symbols. Its comments include a player request to retain the setting across menu/game transitions, acknowledged by the developer: a useful small-project example of both redundant encoding and persistence failure. [CORE-ERROR]
- **Auto-battler inference and shipped patterns:** buying/positioning deadlines, drag placement, shop refresh repetition and rapidly changing units still trigger timing/input/UI routes even if combat is automatic. Super Auto Pets documents an asynchronous Arena mode without timers, contrasted with timed synchronous Versus. Riot lists color alternatives and play without button holds for Teamfight Tactics. [SAP] [RIOT]
- **Auto-battler reported/acknowledged barriers:** TFT's official issue tracker lists rarity-color distinction, tooltip readability and health-bar clarity work as pending. These are acknowledged issues and planned work, not shipped fixes or evidence that the feature tags eliminate every barrier. [TFT-TRACKER]
- **Evidence limit:** the examples establish specific patterns, not a comparative best-in-class rating for these subgenres. Such a ranking remains **UNVERIFIED**.
- **Skip when absent:** aiming, physical platforming, rhythm timing, spoken subtitles in text-only strategy; never skip accessible state because combat is automated.
- **Verify:** receive an event, find its location, understand alternatives, issue/cancel a command, inspect consequences and resume a complex save without losing focus or information.

### G20 — Card / deckbuilder / roguelike / roguelite

- **Shared priorities:** V01/V02, A04, C03/C04, I03/I08/I09. **“Roguelike” does not select a combat system**: add shooter, platformer, tactics, rhythm or strategy routes from the mechanics.
- **Reported barrier evidence:** the Balatro Black Hole mod README lists inaccessible settings and tab-output limitations. This is maintainer documentation of gaps, not a claim of native Balatro narration. [BLACKHOLE]
- **Strong shipped patterns:** Dead Cells documents configurable continues and auto-hit; its official wiki distinguishes leaderboard effects from preserved achievements/progression. The skill must preserve the owner's stronger no-penalty policy. Black Hole is a **community mod example**, not a first-party shipped feature. [DEADCELLS] [BLACKHOLE]
- **Skip when absent:** aim assists in turn-based cards, motor slowdown in an untimed deck editor, permanent-death relief in a game whose only loss is a short rematch.
- **Design inference:** narrate complete card properties and changes; allow select-and-place instead of dragging; show synergies and accessible discard/deck history; suspend mid-run and offer retry/checkpoints without save labels.
- **Verify:** compare cards with nested effects, choose a target, reorder without dragging, survive focus changes and resume a run. Test unknown information without leaking future draws.

### G21 — City builder / management / simulation / farming

- **Most relevant:** C01/C03/C04, V01/V02/V04/V06, I02/I03/I08/I09, A04; D01 for manually operated vehicles and I04 for fishing or timed minigames.
- **Documented need pattern:** APICO exposes mechanism automation, expanded tooltips, focus mode and removal of some random waiting. These reveal independent repetition, clutter and waiting demands within a cozy simulation. [APICO]
- **Strong shipped patterns:** APICO's relevant options; Farming Simulator 25's documented GPS steering/AI workers. Stardew Access provides narrated tiles, tracking and grid movement as a **desktop community mod**, not native console accessibility. [APICO] [FS25] [STARDEW-ACCESS]
- **Skip when absent:** combat damage in a peaceful builder, harvest timing helpers in an untimed simulation, driving aids without controllable vehicles.
- **Design inference:** pause while building, grid snap and keyboard placement, read overlay data as lists, queue repeated jobs, flexible day lengths and adjustable survival/economic pressure.
- **Verify:** place/rotate/demolish a structure, understand an overlay without color alone, process a busy event queue, leave/rejoin and complete any fishing/cooking minigame.

### G22 — Visual novel / narrative / adventure / point-and-click

- **Most relevant:** V01/V04/V05, A01/A02/A04, C01/C02/C05, I02/I03/I08/I09, F01/F02 according to content.
- **Reported barrier:** Life is Strange 2's review describes timed dialogue interpreting slow selection as a choice not to speak. The barrier alters story agency, not merely completion speed. [LIS2]
- **Strong shipped patterns:** Life is Strange: True Colors' reviewed QTE skipping and toggles; Princess Farmer's documented text-motion controls and dialogue presentation. Ren'Py's self-voicing is an **engine capability** requiring project integration/testing, not proof that all Ren'Py releases are accessible. [LIS] [FARMER] [RENPY]
- **Skip when absent:** combat damage and platforming helpers; camera inversion in a static novel. Add action routes for embedded minigames rather than blocking the whole story.
- **Design inference:** no forced reading deadlines, backlog, read-aloud, transcript, skip-read, image/relationship descriptions and forgiving hotspot navigation; allow manual text advance alongside optional autoplay.
- **Verify:** read choices before committing, re-read earlier lines, navigate all hotspots without precision clicking, pause a cutscene and understand nonverbal story events.

### G23 — Walking simulator

- **Most relevant:** V01/V03–V05, I01/I03, A02–A04, C01/C02/C05; F01 where themes are disturbing; I08 for interruption recovery.
- **Reported barrier evidence:** the Life is Strange review describes disorientation and being told a path was wrong without being shown the correct route. This is a transferable exploration barrier; it is not a claim that the game is exclusively a walking simulator. [LIS1]
- **Strong shipped pattern:** Botany Manor documents single-stick stop-and-look, plain-text clues, FOV/smoothing controls and blink-style camera transitions. It is a puzzle-exploration comparator, not proof of an entire genre's access. [BOTANY]
- **Skip when absent:** health, ammo, combat slowdown, jump and parry assists. A quiet game still needs camera, text and navigation assessment.
- **Design inference:** auto-walk/toggle sprint, route assistance, interactable scanning, readable environmental notes, nonvisual landmarks and optional scene summaries.
- **Verify:** traverse the longest required route, inspect an object that normally forces a camera animation, locate the next destination, and resume after a story interruption.

### G24 — Party / couch co-op / local multiplayer

- **Most relevant:** S02, I01–I04/I09, R01 for phone controllers, V01/V02/V04/V05, A01/A02; S01/S03 if network/stream/UGC exists.
- **Documented barrier response:** Jackbox explicitly recommends longer timers for remote stream delay and documents games that advance only after submissions. Its support is per game, not guaranteed across every pack. [JACKBOX8]
- **Strong shipped pattern:** Jackbox Party Pack 10 documents subtitles, motion-sensitivity, extended/no timers and Tee K.O. 2's Draw For Me alternative. Drawing substitution does not imply every drawing mechanic in the pack is accessible. [JACKBOX]
- **Skip when absent:** online speech transcription in wholly offline, speech-free local play; aim assist for a text trivia game.
- **Design inference:** independent player profiles, readable split-screen UI, team identity through shapes/names, ready-before-start, pause/rejoin, flexible participation and no physically mandatory gestures.
- **Verify:** players with different settings join and swap controllers, read their own prompts, finish a round without rapid input/speech, disconnect/rejoin and retain preferences.

### G25 — Competitive online multiplayer / esports / voice chat

- **Most relevant:** S01–S03; B01–B06; relevant input, clarity and comfort routes. Add I07–I09 in practice/private/compatible rulesets, not as client-side changes to shared simulation.
- **Documented need basis:** XAG 119/120 treat accessible communication and all prerequisite UI as a connected experience. Reaching a match or an invite can be the access barrier even when gameplay controls are usable. [X119] [X120]
- **Strong shipped patterns:** Halo Infinite documents text/speech conversion settings and adjustable team outlines; Street Fighter 6 demonstrates that different control types can have different supported modes. Neither source establishes a universal esports policy. [HALO] [SF6]
- **Skip when absent:** story recaps, crafting and platforming; do not load shooter aiming for a competitive card game.
- **Design inference:** keep personal text, audio, remapping and comfort universally usable; define rules explicitly for timing, target assistance and hidden information. Offer equivalent accessible play routes without content or achievement penalties.
- **Verify:** accessible account/lobby/invites, no-voice communication, translated/narrated chat, mute/block/report, reconnect, controller alternatives and spectator/scoreboard readability.
- **Unresolved requirement:** if competitive architecture cannot support a requested gameplay assist fairly, explain the specific limitation and ask the developer to choose an architectural/session solution. Do not silently weaken the brief's no-exclusion rule or claim an offline mode solves all online access.

### G26 — VR / AR

- **Most relevant:** R01–R03, I01–I04, V01/V04/V05, A01–A04, S01/S03 if social. Add every underlying gameplay route; XR is not itself a combat or puzzle genre.
- **Evidence basis:** W3C XAUR documents multimodal input/output, orientation, object querying and customization needs. It is a Working Group Note, not a certification that one comfort setting fits everyone. [XAUR]
- **Strong shipped patterns:** Cosmonious High documents one-handed mode, smaller-human/height adjustments and subtitles. No Man's Sky's Fractal documentation includes VR comfort/controller settings; verify exact headset/build coverage. [OWL] [NMS-VR]
- **Skip when absent:** room-scale locomotion settings in non-XR games; vehicle assists without vehicles; binocular targeting when only a static interface exists.
- **Design inference:** seated reach, handedness, virtual crouch, grab distance, trigger/grip alternatives, snap/teleport choices, caption anchoring, head-motion limits and safe reorientation. AR location walking triggers a separate mobility/physical-location question.
- **Verify:** calibration, menus and every required object from seated and one-handed configurations; test low/high height, no crouching/reaching behind, captions off-axis and locomotion transitions. Do not disable guardian/boundary systems.

### G27 — Educational / kids games

- **Most relevant:** E01, I02/I03/I09, V01/V02/V04/V05, A01–A04, C04/C05 and R01 if touch. Add gameplay routes independent of the educational label.
- **Evidence basis:** PBS's Cyberchase accessibility guide explicitly notes varied needs among autistic children; one child's helpful configuration can make another child's experience harder. [PBS-GUIDE]
- **Strong shipped patterns:** PBS documents Duck Dash background, speed and difficulty options plus eye/head-tracking functionality. Minecraft Education documents Immersive Reader and UI narration, while explicitly identifying some controller limitations. [PBS] [EDUCATION]
- **Skip when absent:** combat, shooting or traversal assists that do not occur; never skip narration because the target audience is “old enough to read.”
- **Design inference:** offer spoken/symbol instructions, replayable demonstrations, untimed practice and adaptive scaffolding without public labels. Keep an assessed learning goal distinct from motor speed or literacy unless those are explicitly the assessment target.
- **Verify:** a child can understand, repeat and act on an instruction without a reading deadline; adult configuration does not prevent the player changing needed personal settings.

### G28 — Text-heavy / text-based / interactive fiction

- **Most relevant:** V01/V04, A04, C01/C04/C05, I03/I08/I09; optional speech input with keyboard alternatives; F01 for text-described content.
- **Reported barriers:** a blind ChoiceScript forum contributor reports unlabeled input fields and narration interrupted at paragraph breaks. Text on screen does not imply screen-reader access. [CHOICESCRIPT-REPORT]
- **Shipped experience example:** a blind player reports completing a Choice of the Dragon playthrough and praises the text-based catalog's access. This is an individual experience, not a technical audit of all versions. Ren'Py provides an independently documented self-voicing implementation reference. [IF-PLAYER] [RENPY]
- **Skip when absent:** camera comfort in static prose, aiming, jump assistance, gore mesh replacements. Keep content warnings for disturbing words/descriptions even without images.
- **Design inference:** semantic paragraphs/choices/headings, command history and synonyms for parsers, spelling tolerance, autocomplete, accessible status summaries, plain-language mode and deliberate destructive-choice confirmation.
- **Verify:** screen-reader navigation after page/scene changes, complete text without forced advance, labeled text entry, keyboard choices, save/load and a long backlog without speech flooding.

### G29 — Physics sandbox

- **Most relevant:** I01–I05/I08/I09, V01–V05, A01/A03/A04, C04; R01 for touch and R02/R03 for VR. Add failure/resource/vehicle routes only where real mechanics demand them.
- **Barrier analysis:** continuous dragging, multi-axis rotation, tiny gizmos, fast unstable bodies and chaotic visual/audio output are mechanic-based inferences. **A dedicated disability player study of this full category was UNVERIFIED here.**
- **Documented shipped comparator:** Teardown documents Creative Mode and saving creations. Its FAQ also says console mouse/keyboard is not planned; creative freedom is not evidence of full input accessibility. [TEARDOWN]
- **Skip when absent:** permadeath, combat, hunger and quest recaps in a free build toy; do not assume “sandbox” means there are no timed campaign missions.
- **Design inference:** pause/step simulation, selectable slower speed, undo/redo, save snapshots, snap-to-grid, numeric transforms, select-and-place, stabilized grab, searchable spawn lists and effect budgets.
- **Verify:** build/rotate/move/save/restore an object using discrete actions, recover from an unstable simulation and clear debris without requiring precise mouse dragging.

## 4. Genre-independent mechanic / feature signal map

This is the main proposed routing specification. It is a design synthesis, not a claim that all listed options have been studied or shipped together. Bundle definitions in section 2 supply the purpose, beneficiaries, defaults, engineering cautions and sourced examples. All rows inherit Baseline assessment; **C** and **A** below label the additional work triggered by the signal.

Evaluate signals at **mode/scene/mechanic level**, then take their union for the game's supported paths. A mechanic used only once can block the entire game. Confirmed absence permits skipping a route; unknown status does not.

Distinguish timer evidence precisely: XAG 116's time-limit guidance concerns UI/session interactions and expressly excludes core gameplay countdowns. Gameplay timer options in this report are separate design recommendations supported by the documented game examples, not a claim that XAG 116 mandates untimed racing or rhythm play. [X116]

### 4.1 Modalities, essential information and entry points

| Signal | Evidence to look for | Options and tier triggered |
| --- | --- | --- |
| M001 — Has any visual output | Rendered menu, logo, cinematic, board, world, loading screen. | **B:** B05 photosensitivity assessment; **C:** V05 flash/pattern replacement if relevant, V01/V02 for displayed information. |
| M002 — Has settings before gameplay | Startup/menu flow and save of preferences. | **B:** B01/B02/B06 accessible entry, persistence and safe reset; **C:** accessible enablement for A04 narration. |
| M003 — Has text or symbols conveying meaning | Labels, icons, cards, numbers, subtitles, world signs. | **B:** B03 readable/semantic information; **C:** V01 scaling/reflow and A04 narration; labels for otherwise ambiguous symbols. |
| M004 — Uses color to convey state | Team colors, rarity, key/lock pairs, damage types, good/bad status. | **B:** B04/V02 non-color encoding; **C:** per-element palette customization; optional filter, never filter alone. |
| M005 — Uses only shape, spatial position or animation for essential state | Character poses, board patterns, gesture clues. | **B:** B04 alternative channels; **C:** A03 explicit cue/state descriptions; **A:** A04 nonvisual representation of spatial relations. |
| M006 — Has gameplay-significant audio | Alert, timer tick, enemy cue, whistle, music puzzle. | **B:** B04; **C:** A02 captions, A03 visible/directional or tactile equivalent, A01 mix controls. |
| M007 — Has no audio | Silent novel, board or puzzle. | **B:** retain UI/safety assessment; **skip** A01/A02 speech settings unless another mode supplies audio; **C:** A04 self-voicing if text/visual state exists. |
| M008 — Has haptic information or force/resistance | Vibration calls, adaptive trigger, force-feedback wheel. | **C:** I01 intensity/Off, A03 equivalent visible/audio cue; ensure haptic Off preserves essential information. |
| M009 — Requires speech, breath, pitch or microphone input | Sing, shout, blow, voice spell, speech recognition. | **C:** I03/R01 digital alternatives, adjustable threshold/calibration, S01 text route; **A:** robust optional voice command integration. |
| M010 — Has cinematic/nonverbal storytelling | Facial expressions, silent action, montage, illustrated information. | **C:** A02 event captions and C05 replay; **A:** A04 audio description; I08 checkpoint/skip consistency. |
| M011 — Has account/login/launcher/lobby prerequisite | Authentication, character creation, server browser. | **B:** B02/B03 complete path; **C:** S01 accessible narration/input/error/timeout handling when social features depend on it. |
| M012 — Has compulsory tutorials or onboarding | Interactive trial, animated demonstration, mandatory setup. | **C:** I10 replay/practice/skip, C05 persistent instructions, E01 alternative instruction channels; **B:** no inaccessible prerequisite to settings. |

### 4.2 Input effort, precision and timing

| Signal | Evidence to look for | Options and tier triggered |
| --- | --- | --- |
| M013 — Has timed inputs | Success window, perfect-hit timing, reaction prompt, combo cancel. | **C:** I04 wider windows/buffering/slowdown; I09 remove deadline; I10 practice; optional auto-success/skip through I02/I08. |
| M014 — Has QTEs | Prompt appears during animation, branch on quick key response. | **C:** I02 automatic completion, single press, hold or skip; I04 longer window; I09 pause while choosing. |
| M015 — Has button mashing/click-per-second scoring | Repeated press counts or rapid polling. | **C:** I02 single press/hold/toggle auto-repeat; I04 acceptance adjustment; retain a pressure-free alternative. |
| M016 — Has sustained button holds | Aim, sprint, guard, charge, interaction progress. | **C:** I02 toggle/latch/automatic completion; independent choices per action. |
| M017 — Distinguishes short and long press on one button | Tap interacts, hold opens secondary action. | **C:** I01 separate bindings, I02 selectable hold duration or discrete command; make prompts reflect actual bindings. |
| M018 — Has repeated actions over long sessions | Harvesting, mining, buying, attacking, rolling, crafting. | **C:** I02 auto-repeat and C03 batching; I08 safe pause/suspend; automation must be reachable before the repetition barrier. |
| M019 — Requires simultaneous buttons/chords | Modifier plus face button, multi-key combo, hold-and-click. | **C:** I03 sequential/latching alternatives and chord grace; I01 direct action bindings; **A:** switch scanning layout if needed. |
| M020 — Requires two analog sticks | Move plus aim/look, dual-stick paddles. | **C:** I03 one-stick mode, I05 camera/aim assist, mode-switch between moving and looking; do not assume remapping alone removes simultaneity. |
| M021 — Has fine cursor/touch targets | Tiny hitboxes, inventory slots, moving targets. | **C:** V01 larger cursor/targets, I03 focus navigation/snap/select lists, R01 stationary generously spaced targets. |
| M022 — Has dragging | Card placement, sliders, object carrying, drag-select. | **C:** I03/R01 select-then-place, discrete increment controls, keyboard/group selection, cancelable operation. |
| M023 — Requires swipe/pinch/multitouch/path tracing | Touch gesture recognition, lockpick pattern. | **C:** R01 buttons/discrete alternatives, selectable gesture tolerance; **A:** alternate puzzle representation where gesture is central. |
| M024 — Uses analog magnitude/direction precisely | Steering, variable jumps, bow draw, radial selection. | **C:** I01 curves/deadzones/inversion, I03 digital increments, I04 timing alternatives; D01 if vehicle axes. |
| M025 — Has fast rotations/complex motion inputs | Quarter-circle special move, repeated stick rotation. | **C:** I10 command shortcuts/macros, I03 sequential commands, I01 independent action bindings. |
| M026 — Requires controller motion/tilt/shake | Shake-to-escape, gyro-only aim, tilt maze. | **C:** I03/R01 digital alternative and sensitivity; I02 nonrepetitive escape action; keep optional gyro support. |
| M027 — Uses stick clicks, trigger resistance or strong force feedback | Sprint-on-click, adaptive trigger, steering force. | **C:** I01 rebind and strength/Off; I02 sprint toggle; prompt/menu parity. |
| M028 — Repeats accidental inputs or rejects slow inputs | Repeat delay, double-click, debounce, short release interval. | **C:** I04 configurable repeat/acceptance/debounce; I01 separate double-click action; retain responsiveness for users who want it. |
| M029 — Requires keyboard text entry | Names, chat, commands, numeric quantity. | **B:** labeled editable field; **C:** I03 virtual keyboard/dictation compatibility, R01 touch alternatives, C04 parser synonyms/history. |
| M030 — Requires many memorized inputs | Large action bar, build shortcuts, fighter command list. | **C:** I10 practice/prompts/simpler schemes, C04 searchable reference, I03 reduced-control path; **A:** contextual scanning or shared control. |
| M031 — Has input prompts/tutorial glyphs | Button images, key names, help overlays. | **B:** B02 remap-aware readable prompts; **C:** V01 glyph scale, C05 persistent reference; handle mixed simultaneous device use without flicker. |
| M032 — Has menus operated differently from gameplay | Pointer-only inventory in keyboard game, trigger-only settings. | **B:** B02 full path with supported inputs; **C:** I01/I03 alternate menu navigation. |

### 4.3 Combat, traversal and detection

| Signal | Evidence to look for | Options and tier triggered |
| --- | --- | --- |
| M033 — Has precise aiming | Raycast/projectile targeting, headshots, tiny target. | **C:** I05 aim snap/lock/stabilization/auto-fire, V01 reticle options; **A:** A04 target querying/nonvisual acquisition. |
| M034 — Has weapon sway/recoil/reticle movement | Recoil curve, aim sway, breath control. | **C:** I05 reduced recoil/sway and stabilization; V04 visual sway Off; separate actual aim behavior from cosmetic camera motion. |
| M035 — Has reload/swap/ammo micromanagement | Ammo counters, timed reload, empty-gun failure. | **C:** I05 auto-reload/swap, I07 infinite ammo, I04 forgiving reload timing, C03 inventory shortcuts. |
| M036 — Has parry/dodge/perfect block | Frame-window checks, telegraph and counter. | **C:** I04 enlarged window, I10 practice, I05 targeting aid, A03 redundant telegraph; I07 independent damage relief. |
| M037 — Has off-screen threats | Attacks from outside view, rear pursuit. | **C:** A03 directional visual/audio cues, V03 clarity, I05 camera-follow options; allow independent cue intensity. |
| M038 — Has melee combos/complex ability rotations | Chain state machine, action priorities, combo graphs. | **C:** I02 hold/auto-repeat, I10 auto-combo/simplification, I03 reduced action set; avoid moving complexity into new undocumented modes. |
| M039 — Has multiple simultaneous enemy/projectile demands | Waves, bullet patterns, crowded encounters. | **C:** I07 enemy count/speed/aggression/damage, I04 slowdown, V03 clarity and V04/V05 reduced effects. |
| M040 — Has consumable stamina/energy/oxygen | Depletion/recovery meter gating action. | **C:** I07 independent unlimited/recovery adjustment, C01 reminders, A03 warning equivalents; I06 no-stamina traversal. |
| M041 — Has damage from different channels | Enemy, fall, collision, hazard, self damage, friendly fire. | **C:** I07 independent sliders/toggles per channel, I08 safe recovery. Enemy damage Off must not silently turn off desired platform hazards. |
| M042 — Has grabs/struggles/escape prompts | Mash-to-break-out, pull meter. | **C:** I02 single press/toggle/auto-escape, I04 wider timing; do not replace one inaccessible hold with another. |
| M043 — Has jumps/gaps/platforms | Landing checks, jump velocity, moving platforms. | **C:** I06 buffer/coyote/extra jumps/ledge guard/hazard speed/skip, A03 landing/edge cues; **A:** A04 nonvisual traversal representation. |
| M044 — Has climbing/ropes/ledge hanging | Alternating grips, stamina, swinging, rappel. | **C:** I06 automatic hold/attachment/no stamina, I03 single-control interaction, I02 grip toggle, V04 rope/camera comfort. |
| M045 — Has chase/escape or forced autoscroll | Pursuit failure, moving death wall, rail event. | **C:** I04 speed scale, I09 time extension, I06 traversal help, I08 section skip/checkpoint; V04 motion options. |
| M046 — Has stealth detection | Visibility/noise/suspicion meters, guard states. | **C:** A03 multimodal detection states, V02 shape/label coding, I07 slower/no detection or aggression, C02 route hints. |
| M047 — Has stealth execution timing | Takedown window, stealth-kill threshold. | **C:** I04 longer/removed execution window, I05 auto-orient, I07 guaranteed takedown if chosen; do not conflate with detection control. |
| M048 — Has forced failure/capture/death | Script advances on losing; boss cannot be killed yet. | **C:** I07 assist-aware scripted transition and I08 skip/recovery; invulnerability must still allow progression. |
| M049 — Has companion/pet/team AI micromanagement | Orders, targeting, survival responsibilities. | **C:** I10 simpler orders, I07 companion survivability, C04 tactical pause/forecast, S02 shared control if applicable. |
| M050 — Has mandatory action minigame in otherwise slow game | Fishing, hacking, chase, sports challenge. | **C:** route actual I02–I10 demand and C02 skip/alternate task; retain story/rewards. Never inherit an exclusion from the parent genre. |

### 4.4 Cameras, motion, visual density and safety

| Signal | Evidence to look for | Options and tier triggered |
| --- | --- | --- |
| M051 — Has moving/first-person camera | Player camera transforms, look axes, first-person controller. | **C:** V04 FOV/rotation speed/smoothing/inversion/bob Off; I03 one-stick alternative; optional fixed reference reticle. |
| M052 — Has camera shake/kick | Camera impulse, trauma value, hit/landing shake. | **C:** V04 intensity/Off globally and per context; V05 if rapid exposure changes accompany it. |
| M053 — Has forced pan/recenter/cutscene camera | Camera rails, auto-look, scripted rotation. | **C:** V04 manual/optional recenter, static/blink transition, cinematic motion reduction; I08 cutscene skip with information alternative. |
| M054 — Has zoom/FOV kick/motion blur/speed lines | Sprint zoom, radial blur, tunnel effects. | **C:** V04 separate Off/intensity for each; retain meaningful speed information through stable cues. |
| M055 — Has camera roll/banking/horizon movement | Vehicles, leaning, waves, head tilt. | **C:** V04 roll/bank Off, D01 stable horizon/camera mode, R03 XR comfort when applicable. |
| M056 — Has parallax, scrolling backdrop or animated menu | Layered sky, particle menu, moving UI background. | **C:** V04 static/reduced background, V06 minimal presentation, V01 opaque text backgrounds. |
| M057 — Has screen-space distortion/damage overlays | Chromatic aberration, blur-on-hit, grain, vignette. | **C:** V04 intensity/Off and V03 alternate readable threat cue; V05 review pulsing damage effects. |
| M058 — Has flashing/strobing/rapid exposure | Lightning, muzzle flash, hit flashes, rhythmic lights. | **B:** B05 safety verification; **C:** V05 replace/remove/intensity before exposure, A03 static alternative cue. |
| M059 — Has patterned textures or rapid scene cuts | Grids, stripes, checkerboards, high-contrast wipes. | **B:** B05 review; **C:** V05 safer textures/transitions. Intentional “no strobe” is insufficient evidence of safety. |
| M060 — Has layered spells/explosions/stacking hit effects | Particle emitters, bloom, additive shaders. | **C:** V03 clarity, V04 density controls, V05 combined-output review; separate allied/enemy/essential effects. |
| M061 — Has guidance paint/highlights/trails | Colored ledges, glowing footprint trail, focus scan. | **C:** V06 Off/on-demand/intensity/frequency and C02 selectable guidance; V03 stronger accessibility highlighting independently available. |
| M062 — Has crowded HUD/notifications/numbers | Floating damage, achievements toasts, large quest trackers. | **C:** V06 per-element toggles, C01 persistent log replacing transient spam, V01 readable scaling and A04 prioritized narration. |
| M063 — Has dark/low-contrast/fogged world | Darkness mechanics, silhouettes, distant targets. | **C:** V03 contrast/outlines/brightness adjustments, A03 alternative cues; F02 visibility relief for deep-water scenes when requested. |
| M064 — Has rendered world text/handwriting | Notes, inscriptions, tiny signage, diegetic screens. | **C:** V01 plain-text overlay/zoom, A04 reading; C05 replay/save the text. Preserve localization and reading order. |
| M065 — Has video or generated/user visual content | Cutscene files, uploaded images, mods, procedural effects. | **B:** B05 coverage; **C:** V05 separate safe/skip path, F01 warnings and A04 description where applicable. Flag unreviewed content honestly. |

### 4.5 Audio, language, reading and nonvisual information

| Signal | Evidence to look for | Options and tier triggered |
| --- | --- | --- |
| M066 — Has spoken dialogue | Voice assets, conversation events, cutscene speech. | **C:** A02 complete speech subtitles, speaker identification, backplate/size controls; C05 backlog/replay. |
| M067 — Has overlapping/radio/off-screen speech | Combat barks, party chatter, radio channel. | **C:** A02 priority/speaker/direction, A01 voice clarity/mix, C05 replay; do not silently omit gameplay instructions. |
| M068 — Has positional audio cues | Footsteps, panned threat, sonar, race turns. | **C:** A03 direction/distance/type indicators, A01 mono/mix; **A:** calibrated nonvisual spatial querying. |
| M069 — Has sound-only puzzle/music clue | Melody ordering, tune matching, echo clue. | **C:** A03 symbolic/text/haptic equivalent and C02 optional solution; preserve relationships rather than merely labeling “music.” |
| M070 — Has loud sudden sounds/high dynamic range | Jump stings, explosions, quiet-to-loud mix. | **C:** A01 peak reduction/night mix/per-category volume and F01 sudden-scare controls; safe startup access under B05. |
| M071 — Has potentially distressing repetitive audio | Buzzing, chewing, breathing, alarm, heartbeat. | **C:** A01 isolated replacement/volume/Off, A03 replacement essential cue; F01 disclosure if appropriate. |
| M072 — Has music defining gameplay rhythm | Beat maps, song position, rhythm judgment. | **C:** I04 latency/timing control, A03 visual/haptic metronome, V04 reduced background motion, V05 safe beat feedback. |
| M073 — Has text-heavy UI | Tables, tooltips, rulebooks, many stats. | **C:** V01 reflow/spacing, C04 glossary/expanded explanations, A04 navigation/reading; **A:** accessible structured world/stat summary. |
| M074 — Has animated/typewriter text | Timed character reveal, jittering dialogue, bouncing words. | **C:** C05 instant/reveal speed/manual advance, V04 text animation Off; narration should not restart per character. |
| M075 — Has timed dialogue/choices | Response clock, default silence, timed branching. | **C:** I09 untimed/extended choice; C05 fully read and re-read; A04 narration completion before expiration. |
| M076 — Has partial/no voiceover | Unvoiced dialogue, voice samples, optional narration. | **C:** A04 self-voicing/screen-reader integration, C05 manual reading speed; do not confuse voice acting with UI narration. |
| M077 — Has important visual story events | Silent facial cue, map reveal, image clue, gesture. | **C:** C05 transcript/replay and textual description; **A:** A04 audio description/spatial alternative. |
| M078 — Has rapid or frequently changing state | Resource counters, combat log, stock values. | **C:** A04 on-demand summaries/verbosity controls, C01 event log; V04 update frequency/pause. Avoid continuously interrupting speech. |
| M079 — Has multiple languages/locales | Translation tables, font fallbacks, downloadable voices. | **B:** B03 scalable readable localization; **C:** A02/A04 language coverage, localized semantics/captions, separate text/voice selection. |
| M080 — Has icons without text | Status glyphs, toolbar, card symbols, radial menu. | **B:** B03 accessible names; **C:** V01 labels/tooltips, C04 symbol legend, A04 announced state and action. |

### 4.6 Objectives, memory, inventory and understanding

| Signal | Evidence to look for | Options and tier triggered |
| --- | --- | --- |
| M081 — Has quests/objectives | Quest data, objective manager, mission steps. | **C:** C01 persistent current/previous objectives, optional reminders, C02 route/hints and V06 tracker visibility. |
| M082 — Has long/multi-session story | Chapters, relationship flags, large save timeline. | **C:** C01 return recap, C05 story/choice log, I08 suspend/manual saves; spoiler level player-controlled. |
| M083 — Has ability-gated backtracking | Locked doors, required powers, revisited rooms. | **C:** C01 map pins/blocked-route text/screenshot notes, C02 newly accessible route hints; A04 accessible map representation. |
| M084 — Has navigation/exploration | Large maps, objective locations, multi-floor space. | **C:** C01 orientation/objective records, C02 optional route/compass; **A:** A04 nonvisual landmarks/audio navigation. |
| M085 — Has screenshot/image-based journal | Map photos, clue pictures, illustrated codex. | **C:** C01 textual labels/search, A04 descriptions; V01 image zoom/plain-text facts. Images alone cannot be the memory aid for blind play. |
| M086 — Has puzzles | Puzzle state/solver, clue objects, mandatory reasoning. | **C:** C02 graduated hints/solution/skip, C04 explanation/undo, A03/A04 sensory alternatives; add I04/I09 only if timed. |
| M087 — Has inventory/equipment | Item arrays, slots, containers, rarity. | **C:** C03 sort/filter/search/favorites/compare, V01/V02 readable states, A04 quantity/property narration, I03 discrete movement. |
| M088 — Has crafting/recipes | Recipe database, workbench, material checks. | **C:** C03 pinned recipes/missing materials/batch/storage access, I02 reduced repetition, C04 clear dependencies. |
| M089 — Has equipment/stat optimization | Item comparison, build presets, skill trees. | **C:** C03 auto-equip/loadouts, C04 meaning/forecast/undo, V01 readable comparisons and A04 structured narration. |
| M090 — Has nested tooltips/encyclopedia | Hover-to-open chains, codex references. | **C:** C04 persistent focusable tooltips, I03 button alternatives, A04 reading order; avoid pointer-only hover puzzles. |
| M091 — Has irreversible choices/destructive actions | Sell/drop/delete, end turn, prestige, permadeath start. | **B:** B06 clear confirmation/cancel; **C:** I08 backup/undo and C04 consequence review where compatible. |
| M092 — Has hidden rules/complex dependency systems | Damage formulas, logistics, unusual status interactions. | **C:** C04 plain-language explanations/glossary/forecast, I10 replayable tutorial/practice, C01 history. |
| M093 — Has multi-stage plans/jobs | Craft queue, construction chain, tactical orders. | **C:** C03 queue/batch, C04 editable plan/preview, C01 progress and required-next-step reminders. |
| M094 — Has resource/time-management overlays | Heatmap, economic chart, production graph. | **C:** V02 patterns/labels, V01 scale, C04 underlying tabular values; **A:** A04 narrated aggregate/spatial data. |
| M095 — Has relationship/character memory demands | Cast list, affinity, aliases, recurring plot roles. | **C:** C01 character/relationship recap, C05 dialogue log, A04 descriptions; distinguish known facts from spoilers. |
| M096 — Has parser/typed commands | Command grammar, required spelling, text adventure prompt. | **C:** I03 keyboard/dictation compatibility, C04 synonyms/autocomplete/history/plain errors; A04 properly labeled input and output. |

### 4.7 Failure, time, resource scarcity and persistent worlds

| Signal | Evidence to look for | Options and tier triggered |
| --- | --- | --- |
| M097 — Has time limits | Mission countdown, oxygen deadline, end-of-day timer. | **C:** I09 extend/Off/pause/practice; I07 adjust pressure source; separate essential mechanics from UI expiry. |
| M098 — Has nonessential UI timeouts | Toast expiry, respawn choice, login keepalive, disappearing tutorial. | **C:** I09 persistent/replayable information and extendable expiry; **B:** accessible explanation and warning. |
| M099 — Has permadeath/run reset | Save deletion, restart-from-zero, ironman. | **C:** I08 opt-out/checkpoint/continue/suspend; no public save marker or achievement/content punishment under brief. |
| M100 — Has corpse run/dropped loot/currency loss | Recoverable corpse, death inventory drop. | **C:** I08 retain/auto-recover resources, checkpoint choice, I07 damage tuning; keep failure channels independently configurable. |
| M101 — Has resource scarcity/grind | Ammo/food/fuel scarcity, rare drops, high repetition. | **C:** I07 resource abundance/infinite specific resource, C03 batching, I02 automation; explicit independent costs. |
| M102 — Has hunger/thirst/decay/maintenance | Timers drain needs, spoilage, equipment wear. | **C:** I07 slow/stop selected depletion or decay, C01 reminders, I09 pause in menus/offline behavior choice. |
| M103 — Has real-world clock/offline progress | Daily event, crops decay, idle reward cap. | **C:** I08 safe suspend, C01 catch-up summary, I09 generous claiming/return windows, I07 no-offline-loss option. |
| M104 — Has long uninterrupted sequences | Long raid, song, boss, cutscene, marathon race. | **C:** I08 suspend/checkpoints/resume, I09 pause where session allows, C05 recap; avoid endurance gating. |
| M105 — Has random gates/repeated failed rolls | Rare spawn, mutation chance, failed crafting. | **C:** I07 adjustable yield/chance or guarantee, C04 transparent outcomes, I08 retry; preserve intended probabilities by default. |
| M106 — Has potentially unrecoverable states | Lost key, out of bounds, stuck vehicle, impossible puzzle. | **B:** assess recoverability; **C:** I08 reset/unstick/rewind/skip with valid state restoration. |
| M107 — Has speed/grade/achievement systems | Timer rank, perfect-chain reward, platform trophies. | **B:** B06 no assist-shaming or achievement/content penalties; **C:** independent display/leaderboard policy; do not import exclusion rules blindly. |
| M108 — Has asynchronous server/global progression | Economy, persistent shared world, multiplayer clock. | **C:** S02 shared-rule design, I08 reconnect/suspend alternatives, I09 ready/catch-up windows; personal slowdown cannot alter the server unilaterally. |

### 4.8 Phobia and sensitive-content signals

These are **routing categories**, not diagnoses, treatment recommendations, or a list of clinically equivalent conditions. Content needs can involve images, words, audio, motion and anticipation. XAG 123 explicitly includes phobias and potentially upsetting content; this report proposes additional specific content tags. Shipped substitutes for every category below were **not** verified. [X123]

| Signal | What the agent must inspect beyond a filename | Options and tier triggered |
| --- | --- | --- |
| M109 — Spiders/arachnid-like enemies | Legs, silhouette, movement, webs, crawling audio, icons. | **C:** F01 disclosure, F02 graded visual/audio/animation replacement and safe preview; **A:** complete asset variants. Grounded is verified for graded appearance changes. [GROUNDED] |
| M110 — Insects/swarms/larvae | Swarms, crawling, eggs, buzzing, infestation environments. | **C:** F01 category warning, F02 replacement/less swarm detail, A01 buzzing control; **A:** alternate animation/environment variants. |
| M111 — Snakes/serpentine motion | Coiled shapes, slithering, hissing, surprise reveals. | **C:** F01/F02 separate shape/motion/sound substitutions; **A:** authored replacement creatures. |
| M112 — Clowns/masks/faces | Makeup, masks, facial close-ups, carnival cutscenes. | **C:** F01 warnings/text-only scene alternative; F02 neutral designs; **A:** full story/asset variants. |
| M113 — Needles/injections/medical procedures | Close-up tools, insertion animation, narrated procedure. | **C:** F01 advance warning, camera cut/skip/text summary, F02 object replacement when compatible. |
| M114 — Blood/gore/dismemberment | Decals, particles, model damage, corpses, UI, text/audio. | **C:** F01 gore/blood/detail controls and scene skips; F02 non-gore replacements; **A:** reauthored sequences preserving story/action state. |
| M115 — Deep water/drowning/underwater darkness | Mandatory immersion, limited visibility, oxygen, submerged threats. | **C:** F01 warning/skip/summary, F02 visibility/horizon relief, I07 infinite oxygen; **A:** alternate traversal. Oxygen alone may leave the feared content intact. |
| M116 — Heights/falling/void exposure | Exposed ledges, glass floors, forced look-down, falling sequences. | **C:** F01 disclosure/skip, F02 visual enclosure/less exposure, V04 no forced tilt; I06 fall protection separately. |
| M117 — Clustered holes/trypophobia-like imagery | Organic hole patterns, nests, pores, procedural textures. | **C:** F01 content disclosure and F02 texture/pattern alternatives; **A:** authored replacement surfaces. Specific shipped mode: **UNVERIFIED**. |
| M118 — Jump scares/abrupt pursuit | Scare event graph, loud sting, sudden camera cut/spawn. | **C:** F01 reduced abruptness/advance cue/skip, A01 sting volume, V04 camera reduction, V05 flash replacement, I07 threat Off separately. |
| M119 — Potentially traumatic narrative themes | Assault, self-harm, abuse, death, discrimination; dialogue and text. | **C:** F01 layered spoiler-controlled warnings, summaries/skip, C05 accessible content index; **A:** alternative authored scenes. |
| M120 — Gambling/addiction-related or distressing content | Casino interactions, simulated betting, addiction narratives. | **C:** F01 disclosure and selected scene/mechanic alternatives, V06 reduce intrusive cues where applicable. Do not infer diagnosis from player choice. |
| M121 — Settings preview itself contains trigger | Spider preview, gore video, alarm sample. | **B:** B05/B06 do not surprise-expose; **C:** F01/F02 text-first selection and explicit preview action; A01 muted/controlled samples. |
| M122 — User/mod/procedural content can bypass filters | Spawnable mod enemies, generated patterns, custom rooms. | **C:** F01 truthful coverage disclosure, F02 tag/replace hooks and moderation; **A:** reviewable extensible content contract. No blanket “safe” claim. |

### 4.9 Multiplayer and social features

| Signal | Evidence to look for | Options and tier triggered |
| --- | --- | --- |
| M123 — Has online voice chat | VOIP, proximity voice, team channel. | **C:** S01 incoming STT/text alternatives/quick phrases, A01 voice mix, S03 accessible mute/block/report; **A:** service integration if necessary. |
| M124 — Has online text chat | Chat panel, channel tabs, scrolling messages. | **C:** S01 incoming narration/outgoing TTS/readable history, V01 size/backplate, I09 duration, I03 accessible entry. |
| M125 — Requires voice/social callouts to coordinate | Raid roles, secret information, team tactics. | **C:** S01 semantic pings/quick chat/text/voice parity; S03 no-voice participation; C01 persistent shared objective/state. |
| M126 — Has lobby/invites/ready checks | Party creation, countdown, match acceptance. | **C:** S01/S02 accessible focus/narration/labels/ready time; I09 response window and reconnect. |
| M127 — Has local players/split screen | Multiple input users/viewports. | **C:** S02 per-player settings/input, V01 per-viewport scale, A02 readable captions, V02 team identity independent of color. |
| M128 — Has shared-device/controller handoffs | Pass-and-play, controller hotplug, profile swap. | **C:** S02 explicit identity/ownership and persistence; B01 safe profile reset; never overwrite another player's preferences. |
| M129 — Has co-op requiring synchronized actions | Two switches at once, cooperative QTE, partner gates. | **C:** I03 sequential window or bot/co-pilot option, I09 ready/pause, S02 shared-control design; **A:** detailed alternative shared-action system. |
| M130 — Has competitive ranking | PvP ladder, tournament, leaderboard validation. | **C:** S02 explicit shared rules, I10 practice/private training; B06 no penalties/labels for personal accessibility; architect assist-compatible options deliberately. |
| M131 — Has spectator/replay/scoreboard | Esports HUD, replays, match results. | **C:** V01/V02 readable identity/state, A02/A04 caption/narration, I03 navigation; do not omit these from access coverage. |
| M132 — Has UGC/social harassment exposure | Drawing/text uploads, emotes, proximity voice. | **C:** S03 accessible moderation/mute/report, F01 content disclosure, S01 communication modality control. |
| M133 — Has disconnect/AFK punishment/session loss | Idle kick, connection recovery, server save. | **C:** I09 accessible warning/extension where possible, I08 recovery/rejoin, S02 explain shared-session limits and alternatives. |

### 4.10 Touch, XR, vehicle and body demands

| Signal | Evidence to look for | Options and tier triggered |
| --- | --- | --- |
| M134 — Has touch input | Touch handlers, virtual sticks, canvas gestures. | **C:** R01 target scale/layout, tap alternatives, supported non-touch inputs and OS assistive interaction; V01 readable mobile UI. |
| M135 — Requires specific phone orientation/reach | Edge controls, two-thumb layout, landscape lock. | **C:** R01 handed layouts/movable controls/orientation choice, I03 one-hand path; avoid overlap with OS gestures. |
| M136 — Has AR camera/location requirements | Camera permission, GPS/geofence, real-world walking. | **C:** R02 alternative reach/interaction, R01 input fallback; **A:** equivalent stationary/remote route where physical location is an access gate. Confirm design requirements before implementing. |
| M137 — Has VR | XR origin/rig, headset/controller actions. | **C:** R02/R03 comfort/calibration/input/caption anchoring; all underlying gameplay routes remain active. |
| M138 — Requires standing/room-scale movement | Walk-to-target, bending, floor pickup. | **C:** R02 seated mode/virtual crouch/reach/teleport; **A:** alternate task interaction preserving function. |
| M139 — Requires both hands | Two-hand tool, simultaneous grip, other-hand menu. | **C:** R02 one-handed equivalent and handedness; I03 sequential/latching commands; include settings access. |
| M140 — Requires large/precise gestures | Throw arc, overhead reach, dance pose. | **C:** R02 range/gesture calibration, R01/I03 button alternatives; I04 timing tolerance; **A:** alternate task execution. |
| M141 — Has XR continuous locomotion/turning | Smooth move, snap turn, artificial acceleration. | **C:** R03 teleport/snap/smooth choices, speed/vignette/recenter; V04 remove extra camera motion. |
| M142 — Has XR flying/driving/forced movement | Cockpit, rollercoaster, spaceship, conveyor. | **C:** R03 comfort/stationary alternative, D01 horizon/stabilization, V04 reduced motion; **A:** alternate traversal if needed. |
| M143 — Has spatial captions/menus | Head-locked/world-locked panels, captions behind viewer. | **C:** R03 anchoring/distance/size/direction, V01 readability, A02 speaker cues; **A:** A04 spatial semantic access. |
| M144 — Has controllable vehicle movement | Throttle/steer/brake/roll input, cockpit. | **C:** D01 independent aids/axis adjustments/recovery, V04 stable view, I01 device remapping; **A:** A04 nonvisual navigation. |
| M145 — Has collision/physics-induced movement | Knockback, boat waves, ragdoll view, vehicle impact. | **C:** V04 camera decoupling/reduction, I07 collision-damage choice, D01 recovery/stabilization; keep actual motion information understandable. |

### 4.11 Slow genres with hidden active demands and specialist representations

| Signal | Evidence to look for | Options and tier triggered |
| --- | --- | --- |
| M146 — Has RTS actions under continuous simulation | Orders while economy/enemies run, multi-front alerts. | **C:** I04 speed/pause-to-command, C03 queues/automation, C04 command previews, A03 prioritized events; S02 shared-rule limits. |
| M147 — Has turn/grid targeting | Hex/square board, active unit, legal target overlay. | **C:** I03 discrete navigation, C04 range/path/outcome review, V02 patterns; **A:** A04 narrated coordinates/neighbors/line of sight. |
| M148 — Has wave or preparation countdowns | Tower-defense next wave, auto-battler shop phase. | **C:** I09 manual next wave/extended or untimed preparation, I03 non-drag placement, C03 batch commands. |
| M149 — Has card/deck state | Hand/deck/discard, nested card effects, deck reorder. | **C:** V01 readable full card, V02 suits/status, C03 filters, C04 effect history, I03 select/place; **A:** A04 structured board/state querying. |
| M150 — Has construction/placement/gizmos | Snap grid, transform handles, rotation axes. | **C:** I03 discrete/numeric transforms, C04 preview/undo, V01 large handles, I09 pause while placing. |
| M151 — Has physics simulation player can manipulate | Rigid bodies, constraints, force tools, unstable structures. | **C:** I04 pause/step/slow simulation, I03 stabilized grab/discrete transform, I08 snapshots/undo, V04/V05 effect limits. |
| M152 — Has farming day cycles/fishing/cooking | End-of-day collapse, fishing tension, timed recipe. | **C:** I09 day/time tuning, I02 automation, I04 minigame tolerance, I08 save anywhere/retry; inspect each activity independently. |
| M153 — Has educational assessment | Scoring intended to represent learning or mastery. | **C:** E01 separate learning from motor/reading/time demands, I09 untimed practice, C04 explanations; avoid falsely scoring an input barrier as failed knowledge. |
| M154 — Has idle production/prestige | Per-second counters, automation unlock, reset multiplier. | **C:** I02 early automation, C03 buy-many/max, A04 quiet summaries, I08 saves and C04 prestige confirmation. |
| M155 — Has time/date/rule numeracy demands | Schedules, money, probability, resource rates. | **C:** C04 plain-language/exact-number toggle and worked examples, V01 formatting, A04 structured reading; no assumed universal “simple mode.” |
| M156 — Supports or intends mods/custom hardware | Plugin loader, remapping layer, content packs. | **B:** preserve accessible input abstractions; **C:** I01 documented action/config hooks, F02 content tags, A04 semantic extension hooks; **A:** stable reviewed extension API. |

### 4.12 Important exclusions and collisions

- A **turn-based** label suppresses real-time timing routes only when no real-time exploration, minigame, active-time gauge, timed decision, lobby deadline or expiring UI remains.
- A **2D** label does not suppress camera comfort: scrolling, parallax, zoom, shake and rapid transitions still qualify.
- A **silent** game skips speech captioning, not screen narration or alternatives to visual state.
- A **cozy** label does not suppress failure, fatigue, time or resource routes; inspect harvesting, fishing, daily deadlines and collection loops.
- A **minimal HUD** preference must coexist with optional memory/navigation aids. Store each independently; offer on-demand reveal and accessible restoration.
- A **camera assist** may move the view more; a **motion reduction** preference may need it disabled or decoupled. Ask about the actual conflict and offer separate controls.
- A **high-contrast mode** can increase uncomfortable brightness or patterns; test it together with V05 rather than treating contrast as universally beneficial.
- A **visual sound indicator** can improve hearing access while increasing clutter; expose event priorities, size and density without removing essential information.
- An **auto-action** can reduce command variety but preserve timing/repetition demands. The WoW and Hi-Fi RUSH examples illustrate why those are separate signals. [WOW] [HIFI]
- An **infinite-resource** assist should not imply invulnerability, and **enemy damage Off** should not imply fall damage Off. The player may deliberately keep some failure states.
- A **content warning**, **substitution**, **reduced scare** and **skip** are different capabilities. Record exactly which is implemented and which modalities remain.
- A **menu narrator** is not equivalent to a screen-reader-compatible game. Record gameplay objects, tooltips, dialog fields, tab changes and dynamic updates separately.
- A **mod** is evidence of a useful extension pattern, not justification for omitting native access or claiming support on platforms where the mod cannot run.

## 5. Detecting signals from a project

This is a proposed inspection method for an AI coding agent. It does not require a particular engine, package manager or file extension. Run it in planning mode against design materials and in retrofit mode against actual supported content.

### 5.1 Evidence hierarchy and signal states

| State | Meaning | Router action |
| --- | --- | --- |
| Confirmed present | Developer describes an intended player demand, or inspected reachable implementation/content establishes it. | Load matching contextual guide and plan implementation/verification. |
| Suspected | A name, asset, dependency or partial implementation suggests it. | Inspect usage and ask one targeted question if still ambiguous; keep the route pending. |
| Confirmed absent | Explicit developer confirmation or sufficiently complete scoped inspection establishes absence. | Skip the contextual guide, recording scope and reason. |
| Unknown | Evidence is missing or conflicting. | Do not call the feature absent. Continue other work; ask only questions affecting current decisions. |
| Removed / planned | Feature is deleted, behind an unreleased flag, or described only in future plans. | Separate current-build audit from planned implementation. Revisit when feature scope changes. |

Source-file presence is not reachability. A demo can load an optional spider encounter that the main campaign does not; a bundled engine example can contain weapons that the game never uses. Conversely, a packed movie can contain flashing content absent from source script searches.

Confidence is a judgment with an explanation, not a fabricated statistical probability. Store the evidence location, relevant mode/scene, last checked build/date, counterevidence and the developer's answer. Do not infer the player's disability from settings or conversations.

### 5.2 What to inspect

| Evidence surface | Useful signals | What it cannot establish by itself |
| --- | --- | --- |
| Pitch / design doc / feature list | Camera, combat, puzzles, target platforms, body demands, progression, save structure. | The actually shipped path, input alternatives or complete content coverage. |
| Level/mission scripts and encounter graphs | QTE, chase, stealth, timer, mandatory vehicle, forced loss, boss phases. | Whether all paths/assets have been inspected, or whether assist combinations are safe. |
| Input map / action definitions | Hold/toggle, chords, device-specific action sets, stick clicks, gyro, touch and XR. | Whether controls can be fully remapped or used through assistive technology at runtime. |
| Gameplay scripts / state machines | Success windows, mash counters, damage/death, resource drain, inventory, jump buffers. | Whether exposed settings actually affect every relevant subsystem. |
| Scenes/prefabs/entity definitions | Camera rigs, UI focus, text widgets, drag handlers, interactable targets, XR positions. | Runtime readability, reachability, supported reading order or whole-session comfort. |
| Shader/material/particle/animation files | Flash emission, exposure pulses, distortion, hit flashes, swaying camera/UI. | Photosensitivity safety: output frequency/area/contrast must be assessed in rendered combinations. |
| Dialogue/localization/narrative files | Spoken/unvoiced text, timed branches, objectives, content tags, speaker identity. | Complete caption quality, actual timing or whether the player can re-read. |
| Item/recipe/quest/save schemas | Dense inventory, economy, multi-session memory, destructive actions, run loss. | Usable UI, valid recovery or equal achievement/content behavior. |
| Audio events/mixer/routing | Voice, positional cues, stings, harsh repetitive sounds, ambience and music. | Caption completeness, audible intelligibility or absence of sound-only progression. |
| Asset names/tags/import metadata | Spiders, snakes, needles, gore, underwater, heights, hole-pattern textures. | What the asset looks/sounds like, how it moves, or whether it is reachable. |
| Network/chat/session code | Voice/text, quick chat, AFK expiry, reconnect, shared time, matchmaking. | Service availability, transcription accuracy, applicable session rules or external anti-cheat compatibility. |
| Platform/build/export configuration | Mobile/touch, VR/AR, console/device coverage, alternate rendering paths. | Feature parity between platforms or actual OS-assistive-technology integration. |
| Existing accessibility settings/tests | Implemented toggles, persistent config, documented supported scenarios. | End-to-end success: an unused setting or menu checkbox is not proof of functioning access. |

### 5.3 Search heuristics: candidate generation, never proof

Use repository-aware text/file search, then trace definitions and call sites. Match identifiers, serialized properties, translated labels and developer vocabulary. These are search suggestions, not required API names.

| Candidate route | Example search terms / structures | Follow-up inspection |
| --- | --- | --- |
| Timed input | `qte`, `quick_time`, `perfect`, `parry`, `window`, `deadline`, `reaction`, success comparison to elapsed time. | Is this gameplay time, input timing, animation time or only a cooldown? What happens on failure? |
| Repetition / holding | `mash`, `tap_count`, `clicks`, `repeat`, `held`, `is_pressed`, `charge`, input-per-frame loops. | Is rapid pressing actually required? Is there already a single-press or latched path? |
| Precision aiming | `aim`, `reticle`, `raycast`, `headshot`, `recoil`, `spread`, `lock_on`. | Is aiming essential, optional or AI-only? Which modes allow assistance? |
| Traversal | `jump`, `ledge`, `dash`, `grip`, `rope`, `climb`, `coyote`, `platform`. | Are there mandatory gaps/timing checks? Are they required to reach settings or tutorial completion? |
| Motion / camera | `camera`, `shake`, `trauma`, `bob`, `sway`, `roll`, `fov`, `blur`, `zoom`, `parallax`. | Trace gameplay, cutscenes, vehicles and menus; distinguish cosmetic transforms from required view control. |
| Flashing | `flash`, `strobe`, `flicker`, `lightning`, `exposure`, `emission`, `blink`, shader time/sine pulses. | Inspect rendered output, texture patterns and overlapping emitters; absence of these words proves nothing. |
| Color state | `team_color`, `rarity`, `palette`, red/green comparisons, colored materials/status icons. | Can shape, label, texture or sound independently identify every meaningful state? |
| Narrative / memory | `quest`, `objective`, `journal`, `codex`, `recap`, `dialogue`, `speaker`, `chapter`. | Does learned information persist? Does the game assume remembering names/routes from weeks earlier? |
| Inventory / crafting | `recipe`, `ingredient`, `stack`, `slot`, `craft`, `container`, `equipment`. | Count repeated player operations, inspect focus/reading order, compare states and material access rules. |
| Failure / scarcity | `permadeath`, `ironman`, `delete_save`, `death`, `drop_loot`, `hunger`, `oxygen`, `durability`. | Separate damage channels and resource loops; check assist-aware scripted failure. |
| Stealth | `alert`, `suspicion`, `detection`, `visibility`, `noise`, `search_state`. | Which sensory channels convey each state? Can the player recover after discovery? |
| Phobia content | `spider`, `arachnid`, `insect`, `larva`, `swarm`, `snake`, `serpent`, `clown`, `needle`, `blood`, `gore`, `ocean`, `abyss`, `nest`, `pores`. | Trace referenced art, animations, sound and text; use safe preview practices. A character called Spider need not depict one. |
| Multiplayer | `voice`, `voip`, `chat`, `ping`, `invite`, `lobby`, `ready`, `afk`, `disconnect`. | Inspect menus before communication, no-voice alternatives, moderation, and server-owned timing. |
| Touch / XR / AR | `touch`, `swipe`, `pinch`, `gyro`, `xr`, `vr`, `headset`, `grip`, `teleport`, `gps`, `geofence`. | Check target platforms and actual input fallback, calibrated reach and physical-location gates. |

### 5.4 Detection by conversation

Ask about **tasks the player must perform**, not diagnostic categories. “Does the player ever have to hold two buttons and move a stick at the same time?” is more actionable than “Is this accessible to disabled users?”

For a new project, begin with a compact developer interview:

1. What does the player do repeatedly, and which actions must be fast or precise?
2. Which camera/view, input devices and platforms are supported?
3. What information is delivered through color, sound, text, spatial layout or animation?
4. What causes failure, lost progress, urgency or resource pressure?
5. What must the player remember across scenes/sessions?
6. Which multiplayer rules or physical/VR/AR requirements are fixed, and which can be customized?
7. Which sensitive subjects, creatures, imagery or abrupt effects can occur?

For a built project, turn findings into targeted questions rather than asking the entire interview again. Summarize the evidence first and ask only for the unresolved fact.

| Ambiguity | Question the agent should ask | Safe independent work while waiting |
| --- | --- | --- |
| Genre says “turn-based,” timer appears in code | “Does this timer ever limit a player's decision or response, or is it only an animation/cooldown?” | Inspect UI and turn sequencing; plan readable information access. |
| “No combat” but fishing or hacking exists | “Can any mandatory minigame fail because an input is too late, too fast or imprecise?” | Inventory all minigames and progression dependencies. |
| Camera-shake variable found | “Can this effect run in cutscenes, menus or vehicles, and should the same reduction preference cover all of them?” | Trace camera modifiers and effect ownership. |
| `spider` asset found | “Is this a reachable spider-like creature or only a name/unused asset, and does it have matching sound, animation or UI imagery?” | Trace asset references without showing trigger previews. |
| Water level found | “Is going underwater mandatory, and is the intended optional relief clearer water, no oxygen pressure, a route skip, or a combination?” | Inspect oxygen and progression state separately. |
| “Easy” difficulty exists | “Which exact variables change, and can damage, timing, navigation and resources be adjusted independently?” | Inspect difficulty data and identify unexposed demands. |
| “Accessible font” listed | “Can players choose plain text, size and spacing throughout the UI, including tooltips and localized text?” | Audit layouts and font fallback. |
| “One-button” mode promised | “Does one button remove rapid repetition and timing as well as command complexity, including menus and movement?” | Count required actions in a complete flow. |
| Multiplayer time is server-owned | “Which modes can support longer decisions, pause or assists, and what equivalent shared rules should we design for other sessions?” | Implement personal readability/comfort/input settings. |
| Mod support offered as the solution | “Which platforms and actions must work natively, and which extension interfaces can remain stable for assistive tools?” | Document input/semantic boundaries and platform differences. |
| Guiding paint divisive | “Should navigation, interactable highlights, combat warnings and hint frequency each be independently configurable?” | Identify separate render/logic channels. |
| Narration setting exists | “Does narration cover gameplay state and all controls, or only some menus; which screen readers/platforms/languages are actually supported?” | Build a coverage inventory and inspect accessible semantics. |
| VR mode is “seated” | “Can every required task, menu and recalibration action be completed without standing, bending, reaching behind or using both hands?” | Inspect reach and interaction bounds. |
| Learning game has scored timer | “Is speed itself the learning objective, or is the timer only creating pressure while measuring something else?” | Separate scoring dimensions and practice flow. |
| Save restrictions intentional | “What must be preserved for the default challenge, and what opt-in suspend/checkpoint/recovery path will satisfy the brief without save or achievement penalties?” | Trace save and progression integrity. |

Never ask the developer to disclose their or their players' medical history. Ask about interaction and content requirements. For unresolved choices, record the limitation and chosen scope; do not fabricate a feature or silently classify unknown as irrelevant.

## 6. Applying the router in planning and retrofit work

### 6.1 Minimal signal record

An agent can maintain this information in its task notes or the project's chosen planning format. This is a suggested data shape, not a request to add a new framework or telemetry system.

```json
{
  "signal": "M014",
  "state": "confirmed_present",
  "scope": "campaign/chapter_3/escape",
  "evidence": "escape sequence requires repeated prompt responses",
  "source_location": "design doc section or inspected scene/script reference",
  "player_demand": "rapid response plus repeated presses",
  "option_bundles": ["I02", "I04", "I09"],
  "tier": "Contextual",
  "references_to_load": ["input-and-effort.md", "timing-and-failure.md"],
  "status": "planned",
  "verification": "finish the sequence using the single-press alternative"
}
```

Store an actual evidence reference in real use. Avoid storing speculative disability labels about players. Add platform/build, confidence explanation, prerequisites and open questions only when they help the task.

### 6.2 Selection algorithm

1. Load the short Baseline checklist and route index.
2. Extract intended mechanics from the conversation/design; inspect actual project structure when available.
3. Seed candidate signals from genre/platform labels, without treating candidates as confirmed.
4. Inspect each reachable mandatory and optional activity, including menus/minigames/cinematics.
5. Classify present/suspected/absent/unknown, with evidence and scope.
6. Expand confirmed signals into option bundles and matching reference sections.
7. Deduplicate overlapping guidance; preserve independent player settings and implementation ownership.
8. Ask targeted questions for ambiguous signals affecting the next implementation decision.
9. Implement Baseline and confirmed Contextual work; make Advanced needs visible with a concrete scope decision.
10. Verify the complete player task under the selected alternatives, not just that the setting exists.
11. Re-scan changed mechanics/content and revisit prior exclusions when the scope changes.

### 6.3 Mode and platform distinctions the agent must preserve

| Distinction | Required routing behavior |
| --- | --- |
| Campaign vs PvP vs private/practice | Personal presentation/input always assessed; simulation-affecting settings need deliberate session rules. |
| Local player vs host/server setting | Store per-player preferences separately from shared world parameters; explain ownership clearly. |
| Native game vs mod vs engine capability | Record provenance. Never promote a mod or engine API into a verified native shipped feature. |
| Desktop vs mobile vs console vs XR | Load only the adapter for the actual target; verify accessibility features per platform instead of inferring parity. |
| Existing save vs new game | Verify the assist can unblock the current save; provide reversible transitions where feasible. |
| Base game vs DLC/minigame/UGC | Scan each route; document content/filter limits and unsupported extensions truthfully. |
| Menu access vs gameplay access | Track separate coverage for navigation, world information, action execution and narrative understanding. |
| Input simplification vs timing vs fatigue | Verify each separately; one-button timing can still require rapid sustained action. |

### 6.4 Verification scenarios selected by signals

These are recommended tests, not a claim that static inspection proves accessibility. Full confidence requires representative players and the assistive technologies actually promised. GAG recommends including disabled participants in playtesting; the Forest Cathedral case study documents a practical playtesting process. [GAG] [FOREST-DEV]

- **First-launch path:** settings before loud/story output; enable required reading/input preference; start and quit without an inaccessible pointer-only or gesture-only step.
- **Timing/effort path:** complete the longest repeated/timed action using single press/toggle, wider timing or skip; cancel and resume without stuck action state.
- **Information path:** understand required state without color alone, then without sound; separately test the promised nonvisual route using actual narration/assistive technology.
- **Memory path:** reload a complex old save, identify the current goal, next action, known route and required item without external notes.
- **Comfort/safety path:** check camera/effect options across cutscenes and vehicles; review rendered sequences and stacked effects. Do not deliberately expose a photosensitive or phobic tester to an unreviewed trigger.
- **Failure path:** combine invulnerability, resource relief, checkpoints and skips; prove scripted defeats, puzzle rewards and progression remain valid.
- **Multiplayer path:** join/rejoin, communicate without speaking/hearing, configure personal settings and use moderation; verify shared-rule decisions.
- **Touch/XR path:** test the actual supported OS tools/devices, seated/one-handed reach and menu access. A successful desktop keyboard test does not prove mobile or headset support.
- **Policy path:** inspect achievements, save UI, content unlocks, endings and completion logic for assist-dependent penalties or labels forbidden by the brief.

### 6.5 Routing acceptance examples

| Project description | Load now | Do not load unless later evidence changes scope |
| --- | --- | --- |
| Silent static untimed card puzzle, keyboard/mouse | Baseline; UI/semantics; color-state; discrete card manipulation; hint/undo; saves. | Shooter aim/recoil, speech captions, vehicles, XR, MMO combat. |
| 2D metroidvania with dash combat and voiced story | Baseline; input/effort; combat/timing; traversal; map/memory; captions; camera/effect safety. | JRPG turn-order UI, vehicle simulation, social chat if absent. |
| Turn-based JRPG with rhythm attack and fishing | Baseline; RPG/structured UI; narration/text; memory; the rhythm/timed-minigame sections; recovery. | Shooter-specific recoil or vehicle guides without evidence. |
| Cozy phone farming game with timed days and dragging | Baseline; touch/input; readable UI; crafting; day pressure; repetition; saves and mobile semantics. | Combat invulnerability if no combat, FPS guidance, room-scale XR. |
| Competitive FPS with private bot matches and voice | Baseline; input/aim; visual clarity; camera/effects; audio/captions; social/competitive rules; bot/private assists. | JRPG dialogue-choice routing and crafting if absent. |
| VR city builder | Baseline; strategy/placement/UI; XR reach/comfort; narration of map/stat information; saves. | Gun targeting or platform jumping without such mechanics. |
| Text adventure with disturbing descriptions | Baseline; text/semantics; parser; memory; content warnings/alternatives; save/recovery. | Mesh-replacement implementation when no images/models exist. |

## 7. Evidence limits and unresolved research

- This is source research, not an independent hands-on audit of the cited game builds. Shipped features are described at the cited source's scope/date. Current implementation quality remains a runtime verification task.
- Direct player/reviewer accounts are included where found. Several genres have guideline-based barrier analysis or developer research rather than a dedicated disability player study; those are labeled. Do not turn them into claims about the frequency of reported barriers.
- Specific patterns are documented for 4X, grand strategy, tower defense and auto-battlers, including small published games. A comparative best-in-class ranking for those subgenres or physics sandboxes was not established and is **UNVERIFIED**. Publisher feature declarations and an issue tracker are not independent hands-on audits.
- First-party complete narration for Balatro was not established. Black Hole is a documented community mod with limitations. Do not cite it as native support. [BLACKHOLE]
- Universal phobia-mode coverage for insects, snakes, clowns, needles, heights and clustered holes was not established. Route the content signal and propose specific alternatives; cite Grounded only for its documented spider presentation changes. [GROUNDED]
- The exact scope of Dead Space's optional hidden-content coverage was not established from the fetched official page. Do not promise that it removes every gore image, jump scare, theme or sound. [DEADSPACE-CONTENT]
- The Candy Crush Saga settings-support URL surfaced in search but returned 404 on fetch. This report relies on the separately cited community reports/announcement, and does not assert comprehensive Saga accessibility based on that unavailable page.
- A blind/low-vision mobile-player study interviewed 32 experienced players in China and describes differences across accessibility levels. It supports taking mobile barriers seriously, but does not establish that one country's participants represent every mobile player or genre. [MOBILE-STUDY]
- No source justifies guaranteeing comfort, seizure safety, phobia relief, dyslexia improvement or access for every player from a setting's name alone. Use precise descriptions, documented coverage, rendered checks and user testing.

## Implications for the skill

### Routing table and recommended reference-file split

Keep the entry-point SKILL.md short: core policy, signal triage, route index, implementation/verification loop. Do not make every invocation ingest this research report or all genre descriptions.

| Recommended reference file | Load when | Content boundaries |
| --- | --- | --- |
| `references/core.md` | Every invocation. | B01–B06, tier/default policy, first-launch path, persistence, no penalty/save marking, truthful claims. |
| `references/routing.md` | Every invocation; short index only. | Compact signal → guide map, evidence states, question selection and mode/platform scope. |
| `references/input-and-effort.md` | I01–I03/I10, touch/XR input demands. | Remapping, holds/mashing/chords, one-hand paths, switch/dwell and complete UI input paths. |
| `references/timing-and-failure.md` | I04/I07–I09, time/resource/failure signals. | Gameplay/UI clock distinction, granular assists, suspend/checkpoints/permadeath/skip and progression integrity. |
| `references/aiming-combat-stealth.md` | I05, combat/detection/targeting signals. | Aim/lock/recoil/telegraphs/detection/complex combat; exclude turn-based command UI unless it also aims. |
| `references/traversal.md` | I06, jumps/climbing/chases. | Jump buffers/extra jumps, ledges, grips, hazards and traversal skips. |
| `references/text-ui-and-narration.md` | Any meaningful text/UI; detailed sections on demand. | V01/A04 UI semantics, scale/reflow, focus, tooltips, narration; distinguish world access. |
| `references/color-and-world-clarity.md` | V02/V03 and spatial-state signals. | Redundant encoding, palettes, outlines, dark/cluttered worlds, accessible board/state representations. |
| `references/camera-and-sensory-effects.md` | V04/V06 and moving/dense UI/world. | Motion, camera, HUD, guidance on/off, quiet presentation and preference conflicts. |
| `references/photosensitivity.md` | Baseline compact safety check; full guide if visual output/effects need review. | Rendered analysis, all content paths, safer substitutes; no warning-only pass. |
| `references/audio-captions-and-cues.md` | A01–A03, speech/audio/music clues. | Mixes, mono, subtitles, captions, directional equivalents, cue density. |
| `references/memory-navigation-and-puzzles.md` | C01/C02/C05; objectives, exploration, reading, puzzles. | Recaps, maps/pins, logs, optional spoilers/hints/skip and minimal-guidance controls. |
| `references/inventory-and-structured-systems.md` | C03/C04, cards, tactics, management, crafting. | Search/sort/batch, forecasts/rules, discrete placement and structured/nonvisual state. No unrelated FPS content. |
| `references/content-and-phobias.md` | F01/F02, matching content/UGC. | Category tags, warning/substitution/skip differences, preview safety and explicit coverage. |
| `references/social-and-multiplayer.md` | S01–S03, any chat/co-op/competitive feature. | Accessible communication path, profiles, moderation, shared rules and reconnect. |
| `references/vehicles.md` | D01/vehicle movement. | Separate control aids, horizon/camera comfort and driving/flight information. |
| `references/touch-and-mobile.md` | R01/mobile/touch. | Reach/targets/gestures, OS assistive access, orientation and interruption behavior. |
| `references/xr-and-ar.md` | R02/R03/XR or physical-location requirements. | Seated/one-hand/reach, calibration, movement comfort, caption placement, physical alternatives. |
| `references/education-and-early-reading.md` | E01/educational or early-reader requirements. | Separate learning goals from motor/reading demands; repeatable instructions and individual scaffolding. |
| `references/verification.md` | Implementation or retrofit verification. | Select tests by signal; representative player/assistive-tech coverage; build/platform evidence. |
| `references/genre-recipes.md` | Optional initial triage only. | One short recipe per G01–G29, pointing to mechanic guides; not a second duplicated option encyclopedia. |
| `references/engines/<detected-engine>.md` | Only when implementing in that engine. | Verified adapter code for settings/input/UI/captions; never preload Unity/Unreal/web into a Redot task or vice versa. |

### Decision rules to encode

1. **Core first, then mechanic union.** Genre seeds questions; positive signals select guides. Mixed games get the union of their actual demands.
2. **Positive evidence loads; confirmed scoped absence skips; uncertainty asks.** File names and genre labels alone do not prove presence or absence.
3. **One mandatory exception counts.** A single timed minigame, vehicle scene, flashing cinematic or touch gesture receives its own route.
4. **Separate presentation, information and action demands.** Reduced motion, narrated state and easier targeting do different jobs; none substitutes automatically for the others.
5. **Separate personal preferences from shared simulation.** Preserve accessible presentation/input while deliberately designing multiplayer timing/assists. Surface unresolved conflicts instead of inventing blanket exclusions.
6. **Offer independent options with intended gameplay defaults.** Apply safe presentation before exposure; avoid forcing one disability preset, involuntary previews, shaming, save marks or achievement/content penalties.
7. **Load Advanced guidance when relevant and scoped.** Do not discard an essential access need merely because implementation is expensive; distinguish foundational UI semantics from extensive world alternatives.
8. **Reuse guidance, not assumptions.** Card/tactics/strategy interfaces can share structured-state guidance; an FPS should not load JRPG-specific advice unless it actually contains those mechanics.
9. **Retain provenance and limitations.** Native feature, mod, engine capability, reviewer experience and proposed design must remain distinct in plans and user-facing claims.
10. **Verify an achievable player task.** The acceptance condition is completing and understanding the relevant task with the selected alternatives, including first launch, interruptions and combined settings—not the existence of a checkbox.
