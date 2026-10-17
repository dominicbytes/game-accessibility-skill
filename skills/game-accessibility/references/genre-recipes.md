# Genre recipes (first scan only)

Read only the recipes for this game's genres.

**Genre only seeds the first scan and the first questions. Confirmed signals decide what loads.**
A mixed game takes the union of its confirmed signals: a JRPG with timed fishing loads the timing
guide for the fishing scene. "Usually skip" never means "never": one mandatory occurrence counts
(see [signal-routing.md](signal-routing.md)). Do not add options for mechanics a game lacks.

Each recipe lists likely signals (S IDs), the guides to load (files in `references/topics/`),
what to skip unless present, the items most often missed (option IDs from
[option-catalog.md](option-catalog.md)), reference games and one verification line. Reference
games show one documented feature at the cited scope. They do not prove the whole game is
accessible. Copy behaviour, never penalties: no achievement locks, save marks or assist badges
([core-policy.md](core-policy.md)).

## G01 First-person and third-person shooter
- **Likely signals:** S016, S020, S027, S033–S037, S039, S041, S051, S052, S054, S058, S060, S068; online adds S123–S126, S130.
- **Load:** input-and-motor, combat-aiming-stealth, camera-and-motion, photosensitivity, audio-and-captions, color-and-clarity, difficulty-and-assists; multiplayer-and-communication when online.
- **Usually skip unless present:** traversal-and-platforming (no gaps), structured-ui-and-strategy (no inventory), memory-and-navigation (arena only), content-and-phobias.
- **Must-not-miss:** hold-to-sprint and hold-to-aim as toggles or auto-sprint (M13, M34); team and enemy identity beyond red/blue (V24, V25); shake, bob and FOV in any first-person view (O01, O02, O05); off-screen and sound-only threats shown visually (H34).
- **Reference games:** Halo Infinite separate effect strengths, team colours and chat conversion ([Halo Support](https://support.halowaypoint.com/hc/en-us/articles/4408122295444-Accessibility-Features-in-Halo-Infinite)); Halo technical-preview feedback on painful hold-to-sprint and red/blue colours ([343](https://www.halowaypoint.com/en-us/news/technical-preview-outcomes)); Gears 5 Target Lock, documented for certain PvE modes only ([Gears](https://www.gearsofwar.com/en-gb/games/gears-5/accessibility/)).
- **Verify:** clear a mandatory fight and reload one-handed, muted, with effects reduced; test campaign and PvP rules separately.

## G02 Action-adventure, hack-and-slash, soulslike
- **Likely signals:** S013, S014, S016, S036, S038–S041, S048, S081, S084, S100; S043 if there is jumping.
- **Load:** input-and-motor, timing-and-speed, combat-aiming-stealth, difficulty-and-assists, camera-and-motion, photosensitivity, audio-and-captions, memory-and-navigation.
- **Usually skip unless present:** ammo options (D11) in melee-only combat; traversal-and-platforming without jumps; turn-based UI.
- **Must-not-miss:** "soulslike" never vetoes pause, parry-window scale, corpse-run relief or combat damage Off (D27, D16, D28, D04); separate damage channels (D05, D07); quest memory without outside notes (C08, C09); sound direction shown visually (H22, H34).
- **Reference games:** God of War Ragnarök miniboss checkpoints, navigation and presentation aids ([PlayStation](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/)); Prince of Persia: The Lost Crown independent combat parameters ([Ubisoft](https://news.ubisoft.com/en-us/article/5nGZiBSFtcEzFd93QlTotS/prince-of-persia-the-lost-crown-accessibility-spotlight)); Elden Ring reviewer's launch-period barriers: direction, subtitles, quest memory ([Can I Play That?](https://caniplaythat.com/2022/02/27/elden-ring-lacking-accessibility-unintentional-barriers-and-land-jellyfish/)).
- **Verify:** boss phase changes, grabs, stamina exhaustion, death recovery and required traversal with assists combined.

## G03 2D and 3D platformer
- **Likely signals:** S013, S016, S041, S043–S045, S052, S056, S058; 3D adds S051, S053.
- **Load:** traversal-and-platforming, input-and-motor, timing-and-speed, difficulty-and-assists, camera-and-motion, photosensitivity.
- **Usually skip unless present:** combat-aiming-stealth in a jump-only game; memory-and-navigation for linear rooms; camera rotation in fixed 2D.
- **Must-not-miss:** fall failure separate from combat damage (D07, D04); analog-only movement and rapid presses (M23, M14); chase and autoscroll sections (D14, D23); 2D still has shake, parallax and flashes (O01, O17, P01).
- **Reference games:** The Forest Cathedral Float Mode and spike removal ([Whitethorn](https://whitethorngames.com/theforestcathedral/accessibility)); Celeste Dash Assist, grab toggle and shake controls; do **not** copy its assist marker on save files, which core-policy forbids ([changelog](https://www.celestegame.com/changelog.html)); Psychonauts 2 separate No Fall Damage and invincibility ([Double Fine](https://www.doublefine.com/games/support/psychonauts-2/accessibility?platform=all)).
- **Verify:** moving platforms, wall jumps and a one-off chase with assists on; no hidden timing check remains.

## G04 Metroidvania
- **Likely signals:** platformer and action signals plus S081, S083–S085; S099 for roguelite runs.
- **Load:** the G02 and G03 guides plus memory-and-navigation.
- **Usually skip unless present:** firearm aiming, crafting UI, multiplayer-and-communication. Stats alone do not load turn-based UI guidance.
- **Must-not-miss:** blocked-route notes in text, not only images (C21, R10); hints for newly reachable places after a new ability (C14, C19); traversal skip that keeps progression flags (D23); a filterable, readable map (C20).
- **Reference games:** Prince of Persia: The Lost Crown Memory Shards pin screenshots to the map, plus Guided Mode and platforming skips ([Microsoft](https://developer.microsoft.com/en-us/games/articles/2025/03/prince-of-persia-the-lost-crown-accessibility/)); Dead Cells continues, auto-hit and separate trap damage ([Update 29](https://dead-cells.com/patchnotes/29)).
- **Verify:** return after a long break and use a new ability on a previously blocked route; progress after a skipped traversal.

## G05 JRPG, turn-based RPG, tactics
- **Likely signals:** S003, S004, S066, S073, S081, S082, S087, S089, S090, S092, S147, S157; check S013 (timed hits, active-time gauges) and S050 (minigames).
- **Load:** text-and-readability, screen-reader-and-voice, structured-ui-and-strategy, memory-and-navigation, audio-and-captions, difficulty-and-assists; timing-and-speed only for timed hits, gauges or minigames.
- **Usually skip unless present:** combat-aiming-stealth, vehicles, traversal-and-platforming.
- **Must-not-miss:** turn-based is not automatically accessible. With random encounters or battle commands (S157): random encounter rate down to off, story battles kept (D31); optional auto-battle the player can take over at any time (D32); battle animation speed that never shortens decision time (D33); battle skip that keeps rewards, XP and story flags (D23). Also: dense UI and maps (V01, R07, C26); more turn time where decisions are timed (D29); grind relief (D13); confirmation before ending a turn (C29).
- **Reference games:** FINAL FANTASY Pixel Remaster auto-battle, encounter toggle and XP multipliers, PS4/Switch announcement scope ([Square Enix](https://www.square-enix-games.com/en_GB/home/final-fantasy-vi-pixel-remaster-launches-digitally-ps4-nintendo-switch-april-19th)); Baldur's Gate 3 reviewer: turn-based combat helped, maps confused ([Can I Play That?](https://caniplaythat.com/2023/08/31/baldurs-gate-3-accessibility-review/)); Dragon Quest XI S recap on return ([Nintendo Wire](https://nintendowire.com/reviews/dragon-quest-xi-s-echoes-of-an-elusive-age-definitive-edition/)).
- **Verify:** inspect a unit, compare abilities, target a cell, read the forecast, cancel, commit and resume mid-battle with each input. With encounters off, auto-battle on and a battle skipped, story battles still trigger and rewards, flags and achievements match a normal win.

## G06 Western and open-world RPG
- **Likely signals:** S061, S062, S064, S066, S067, S073, S081–S084, S087, S089; add combat, stealth and vehicle signals from actual play.
- **Load:** memory-and-navigation, text-and-readability, audio-and-captions, sensory-load-and-guidance, structured-ui-and-strategy, screen-reader-and-voice; plus combat or vehicle guides from mechanics.
- **Usually skip unless present:** parry helpers in a dialogue-led RPG; jump assists in click-to-move; chat in offline play.
- **Must-not-miss:** on-screen quest log and recap instead of paper notes (C08, C09, C11); guidance with Off and On request (N03, N04); separate fight, stealth and exploration tuning (D03); readable world text (V30).
- **Reference games:** Assassin's Creed Valhalla separate challenge per activity and guaranteed assassination ([Ubisoft](https://news.ubisoft.com/en-us/article/5Rys9UeXZh9bAjnHBZN4pC/assassins-creed-valhalla-bringing-accessibility-to-a-viking-epic)); Horizon Forbidden West "Story So Far" and reminders ([PlayStation Blog](https://blog.playstation.com/2022/02/10/accessibility-features-in-horizon-forbidden-west/)).
- **Verify:** load an old save, name the next action and place, manage a full inventory, finish a required noncombat minigame.

## G07 MMO
- **Likely signals:** S011, S016, S019, S030, S038, S039, S060, S062, S081, S087, S108, S123–S126, S133.
- **Load:** input-and-motor, combat-aiming-stealth, sensory-load-and-guidance, multiplayer-and-communication, text-and-readability, screen-reader-and-voice, structured-ui-and-strategy, memory-and-navigation.
- **Usually skip unless present:** precision aim in tab-target combat; crafting UI without crafting; jump aids. Never skip chat because PvE dominates.
- **Must-not-miss:** fewer buttons is not fewer presses (M15, M19); ally versus enemy effect density (N07); group play without voice (K05, K07, K18); server clocks limit personal slowdown, so design shared rules (K21, K23).
- **Reference games:** World of Warcraft Assisted Highlight and Single-Button Assistant, with an added global cooldown ([Blizzard](https://news.blizzard.com/en-us/article/24205457/legacy-of-arathor-get-an-assist-with-new-ui-options)); fatigue feedback on its repeated presses ([PC Gamer](https://www.pcgamer.com/games/world-of-warcraft/world-of-warcraft-single-button-rotation-assist-verdict/)).
- **Verify:** log in, join a group, communicate without speech, read a warning in a busy fight with chat open.

## G08 Survival and crafting
- **Likely signals:** S018, S039–S041, S087, S088, S093, S101–S103; S109–S111 and S115 when that content exists.
- **Load:** difficulty-and-assists, structured-ui-and-strategy, input-and-motor, memory-and-navigation; content-and-phobias for creatures; multiplayer-and-communication for shared worlds.
- **Usually skip unless present:** hunger sliders without needs meters; spider options without spiders; shooter targeting in noncombat crafting.
- **Must-not-miss:** depletion, scarcity and reminders as separate settings (D08, D13, C08); batch crafting and missing-material lists (M15, C19); hostility separate from appearance filters (D10, F01); pause and offline time costs (D27, D28).
- **Reference games:** No Man's Sky Waypoint separate survival, combat, economy and death settings ([Hello Games](https://www.nomanssky.com/waypoint-update/)); Grounded graded spider appearance with unchanged difficulty ([Obsidian](https://grounded.obsidian.net/news/grounded/accessibility-features)); Subnautica Freedom and Creative modes, per community wiki ([wiki](https://dev.subnautica.wiki/wiki/Game_Modes)).
- **Verify:** craft from scattered storage, survive an interruption and an empty meter, progress with hostiles off.

## G09 Horror (jump scares, gore, phobia content)
- **Likely signals:** S045, S046, S048, S052, S058, S063, S070, S114, S118, S119; S109–S117 only as present.
- **Load:** content-and-phobias, audio-and-captions, photosensitivity, camera-and-motion, color-and-clarity, difficulty-and-assists; stealth or puzzle guides from mechanics.
- **Usually skip unless present:** unrelated phobia categories ("horror" does not say which occur); combat invulnerability in a purely observational story.
- **Must-not-miss:** separate jump scares, loud stings, pursuit threat, gore and themes (F11, H14, D10, F07, F12); warnings before exposure without auto-previews (F13; signal S121); replacements cover sounds, animation, corpses and thumbnails (F03); dark scenes readable (V35).
- **Reference games:** SOMA Safe Mode stops attacks but keeps atmosphere ([Frictional](https://frictionalgames.com/2017-11-what-is-somas-safe-mode/)); Dead Space content-warning page; completeness of its hide-content coverage is UNVERIFIED ([EA](https://www.ea.com/games/dead-space/dead-space/content-warning?isLocalized=true)); Outer Wilds: Echoes of the Eye Reduced Frights, exact changes unspecified ([PlayStation Blog](https://blog.playstation.com/2021/07/29/more-to-explore-outer-wilds-echoes-of-the-eye-expansion-lands-september-28/)).
- **Verify:** walk boot, settings thumbnails, loading screens, narration, enemy deaths and DLC with each filter on.

## G10 Stealth
- **Likely signals:** S016, S037, S045–S048, S068, S084.
- **Load:** combat-aiming-stealth, audio-and-captions, color-and-clarity, difficulty-and-assists, timing-and-speed.
- **Usually skip unless present:** recoil options in noncombat stealth; inventory tooling without inventory.
- **Must-not-miss:** detection states readable without sound or colour (H34, V25); detection speed separate from takedown timing (D09, D16); recovery after discovery and a mandatory-stealth skip (D23, D24); chase sections (S045).
- **Reference games:** The Last of Us Part II invisible-while-prone and enhanced listen mode ([PlayStation Blog](https://blog.playstation.com/2020/06/09/the-last-of-us-part-ii-accessibility-features-detailed/)); Assassin's Creed Mirage guaranteed pickpocketing removes its QTE ([Ubisoft Help](https://www.ubisoft.com/en-us/help/assassins-creed-mirage/gameplay/article/accessibility-features-in-assassins-creed-mirage/000105449)).
- **Verify:** tell unaware, suspicious, searching and detected apart muted and in greyscale; recover after failed concealment.

## G11 Racing and driving
- **Likely signals:** S008, S024, S027, S054, S055, S068, S097, S106, S144.
- **Load:** vehicles, input-and-motor, camera-and-motion, audio-and-captions, photosensitivity; difficulty-and-assists for rewind and recovery.
- **Usually skip unless present:** traversal-and-platforming, story memory, crafting.
- **Must-not-miss:** steering, braking, throttle and gears as independent aids (D19); force-feedback strength and Off (N12, N14); stable horizon and chase camera (O12); turn and track cues in more than one channel (H34).
- **Reference games:** Forza Motorsport Blind Driving Assists and One Touch Driving, with a stated learning curve ([Xbox Wire](https://news.xbox.com/en-us/2023/04/27/forza-motorsport-accessibility-features-blind-driving/)); later fixes to blind-driving previews and pit cues ([Turn 10](https://forza.net/news/forza-motorsport-update-5-fixes)).
- **Verify:** start a race, enter the pit, recover from a collision and finish a lap; test cue previews too.

## G12 Flight and space simulation
- **Likely signals:** S019, S024, S055, S073, S104, S144; S033 for combat flight; S142 for VR.
- **Load:** vehicles, input-and-motor, camera-and-motion, text-and-readability, audio-and-captions; combat-aiming-stealth for combat; vr-and-xr for VR.
- **Usually skip unless present:** jump assists; cockpit comfort for a static tactical view; auto-targeting in civilian flight.
- **Must-not-miss:** axis curves and deadzones (M09, M10); auto-level, stability and roll reduction (D19, O12); remapping covers menus and hangar, not only flight (M01); instrument magnification (V31).
- **Reference games:** STAR WARS: Squadrons remapping across menus, hangar and flight, HOTAS, outlines, off-screen indicators ([EA](https://www.ea.com/amp/news/star-wars-squadrons-accessibility)); Microsoft Flight Simulator assistance presets, with stated narration limits ([MSFS](https://www.flightsimulator.com/accessibility/)). A flight-sim player study is UNVERIFIED.
- **Verify:** remap flight and menus, reach a waypoint, recover orientation, land or dock, then suspend.

## G13 Sports
- **Likely signals:** S004, S013, S020, S024, S037, S127, S131; online adds S123–S126, S130.
- **Load:** input-and-motor, timing-and-speed, color-and-clarity, audio-and-captions, screen-reader-and-voice; multiplayer-and-communication when online.
- **Usually skip unless present:** stamina toggles without stamina; story recaps for single matches.
- **Must-not-miss:** team identity beyond kit colour (V24, V25); timed swing, kick or shot alternatives (D16); ball and possession readable through more than one channel (H34, R03); practice without failure (C06).
- **Reference games:** Madden NFL 24 menu narration, open-receiver callouts and defensive assists; English-US scope, subtitles only in some cutscenes ([EA](https://www.ea.com/able/resources/madden-nfl/madden-nfl-24)); EA Sports FC 24 pass and shot assists, auto switching ([EA](https://www.ea.com/able/resources/ea-sports-fc/fc-24?isLocalized=true)).
- **Verify:** follow the ball, identify possession, call a play and make a timed action without relying on one sense.

## G14 Fighting
- **Likely signals:** S013, S019, S025, S030, S036, S038, S068, S130.
- **Load:** input-and-motor, timing-and-speed, combat-aiming-stealth, audio-and-captions, screen-reader-and-voice; multiplayer-and-communication when online.
- **Usually skip unless present:** quest logs, resource gathering, free-camera aim.
- **Must-not-miss:** simplified inputs that state which moves remain (M18, M19); spacing and attack height readable or audible (H34, R15); narrated move list and training (R01, C06); menus that reach the fight unaided.
- **Reference games:** Street Fighter 6 Classic, Modern and offline-only Dynamic controls ([Capcom](https://www.capcom-support.com/hc/en-us/articles/7122030981660-What-are-the-control-types-in-Street-Fighter-6)); its launch sound accessibility and missing menu narration ([Can I Play That?](https://caniplaythat.com/2023/05/31/street-fighter-6-accessibility-hits-and-misses/)); Mortal Kombat 1 first-launch screen-reader setup ([WB Games](https://mortalkombatgamessupport.wbgames.com/hc/en-us/articles/19435787154963-Mortal-Kombat-1-Accessibility-Features-A11Y)).
- **Verify:** menu to match, side switch, distance and attack height recognisable, in every online and offline mode.

## G15 Rhythm and music
- **Likely signals:** S013, S015, S016, S056, S058, S060, S072, S104.
- **Load:** timing-and-speed, input-and-motor, photosensitivity, camera-and-motion, sensory-load-and-guidance, audio-and-captions; vr-and-xr only for VR.
- **Usually skip unless present:** quest markers, aim magnetism, survival systems.
- **Must-not-miss:** latency calibration and timing tolerance separate from song speed (D16, D14); no-fail completion keeping story and rewards (D08); beat feedback without hazardous flashes (P01, P05); visual or haptic beat (H34, H35).
- **Reference games:** Hi-Fi RUSH Auto-Action and Single Key, which still expect beat timing ([KRAFTON](https://support.krafton.com/hc/en-us/articles/52371575035801-Hi-Fi-RUSH-Accessibility-Settings)); reviewer on helpful beat cues and others' visual overload ([Access-Ability](https://access-ability.uk/2023/02/09/hi-fi-rush-and-audio-visual-rhythm-game-feedback/?amp=1)); Beat Saber study cautions on one-handed play ([CHI 2025](https://johannwentzel.ca/projects/motionblocks/files/MotionBlocks__CHI_2025.pdf)).
- **Verify:** play muted, vary audio and display latency, miss on purpose without losing the song or story.

## G16 Puzzle (including timed and colour-matching)
- **Likely signals:** S004, S005, S021, S022, S069, S086, S091; S097 if timed.
- **Load:** memory-and-navigation, color-and-clarity, text-and-readability, screen-reader-and-voice; timing-and-speed only for timed puzzles.
- **Usually skip unless present:** damage options, camera-and-motion for static boards, combat-aiming-stealth.
- **Must-not-miss:** every colour clue also a symbol or label (V25); hints from nudge to solution, and a skip that grants rewards (C15, C16, D22); undo and partial reset (C28); sound clues with a text or visual equivalent (H19).
- **Reference games:** Botany Manor untimed puzzles and plain-text clue overlay ([Whitethorn](https://whitethorngames.com/botanymanor/accessibility)); Princess Farmer non-colour-only board information ([Whitethorn](https://whitethorngames.com/princessfarmer/accessibility)); Life is Strange reviewer on hidden-object and precise focusing barriers ([Can I Play That?](https://caniplaythat.com/2020/01/31/teen-friendly-review-series-life-is-strange-2015/)).
- **Verify:** solve every mandatory puzzle in greyscale, muted, with discrete input.

## G17 Match-3, casual, mobile, touch
- **Likely signals:** S004, S018, S021–S023, S058, S060, S097, S134, S135.
- **Load:** touch-and-mobile, input-and-motor, color-and-clarity, photosensitivity, text-and-readability, screen-reader-and-voice.
- **Usually skip unless present:** camera and FOV for static boards; traversal; story backlog without story.
- **Must-not-miss:** tap-select then tap-target instead of swipe (M30, M32); every special piece distinct without colour (V25); one-handed reach (M05, M28); reduced board shake and cascades (O01, N07).
- **Reference games:** Princess Farmer multiple play styles, one-stick play, same rewards ([Whitethorn](https://whitethorngames.com/princessfarmer/accessibility)); Candy Crush Friends Saga reduced scrolling and shake ([King](https://community.king.com/en/candy-crush-friends-saga/discussion/313075/new-check-out-the-accessibility-features)); a Candy Crush Saga player report of blue/purple confusion ([King community](https://community.king.com/en/candy-crush-saga/discussion/462233/colour-blind-players)).
- **Verify:** OS screen reader and switch navigation, board narration, interrupted sessions, every special piece.

## G18 Idle, incremental, clicker
- **Likely signals:** S018, S062, S078, S080, S091, S103, S154.
- **Load:** structured-ui-and-strategy, input-and-motor, screen-reader-and-voice, text-and-readability.
- **Usually skip unless present:** timing-and-speed (unless events impose real deadlines), combat-aiming-stealth, traversal, camera-and-motion.
- **Must-not-miss:** automation from the first repetitive task, never after hours of clicking (M15); buy-many and buy-max; quiet narrated summaries without speech floods (R02, R07); prestige confirmation (C29); offline rewards that do not expire (D28).
- **Reference games:** Trimps ScreenReader.html variant, with open issues ([GitHub](https://github.com/Trimps/Trimps.github.io/blob/master/ScreenReader.html)); A Dark Room browser issue on unlabelled elements and verbosity ([GitHub](https://github.com/doublespeakgames/adarkroom/issues/70)).
- **Verify:** reach the first automation without rapid clicking, reload after time away, avoid an accidental prestige.

## G19 Strategy: RTS, 4X, grand strategy, tower defense, auto-battler
- **Likely signals:** S004, S062, S078, S090–S092, S094, S146 (RTS), S147 (grids), S148 (waves, shop phases).
- **Load:** structured-ui-and-strategy, text-and-readability, screen-reader-and-voice, color-and-clarity, sensory-load-and-guidance; timing-and-speed for RTS, waves and timed shops.
- **Usually skip unless present:** aiming, platforming, rhythm; speech captions in text-only strategy. Automatic combat never removes the need for accessible state.
- **Must-not-miss:** pause-to-command and slower simulation in RTS (D27, D14); manual next wave and untimed shop phases (D29); placement without dragging (M32); menu narration is not HUD narration (R02, R03).
- **Reference games:** Age of Empires IV Story Mode, contrast and text size; its narration explicitly excludes the HUD ([Age of Empires](https://www.ageofempires.com/age-iv-accessibility/)); Super Auto Pets untimed asynchronous Arena mode ([Steam](https://store.steampowered.com/app/1714040/Super_Auto_Pets/)); core_error jam game tile symbols and a reported settings-persistence gap ([itch.io](https://cakestorm.itch.io/core-error)). Best-in-class rankings are UNVERIFIED.
- **Verify:** receive an event, find it, issue and cancel a command, read consequences, resume a complex save.

## G20 Cards, deckbuilders, roguelikes, roguelites
- **Likely signals:** S022, S090–S092, S099, S149; S157 only if it has turn-based battles; combat signals from the actual mechanics.
- **Load:** structured-ui-and-strategy, text-and-readability, screen-reader-and-voice, color-and-clarity, difficulty-and-assists. "Roguelike" says nothing about combat: add shooter, platformer or tactics guides from mechanics.
- **Usually skip unless present:** aim assists in turn-based cards; timing for untimed deck editors; permadeath relief when loss is a short rematch.
- **Must-not-miss:** full card properties narrated (R07); select-and-place instead of drag (M32); suspend mid-run (D25); continues and checkpoints with no save labels (D24).
- **Reference games:** Dead Cells continues and auto-hit, achievements kept; its leaderboard exclusion is an owner decision under core-policy, not a default ([wiki](https://deadcells.wiki.gg/wiki/Assist_Mode_and_Accessibility)); Balatro Black Hole is a community mod with documented gaps, not native support ([GitHub](https://github.com/game-a11y/Balatro-BlackHole)).
- **Verify:** compare nested card effects, choose a target, reorder without dragging, resume a run.

## G21 Simulation, management, city builder, farming
- **Likely signals:** S018, S062, S087, S088, S093, S094, S102, S103, S150, S152; S144 for driven vehicles.
- **Load:** structured-ui-and-strategy, timing-and-speed, input-and-motor, text-and-readability, sensory-load-and-guidance, difficulty-and-assists; vehicles if driven.
- **Usually skip unless present:** combat damage in a peaceful builder; harvest timing in an untimed sim.
- **Must-not-miss:** "cozy" still has fishing, day deadlines and collection loops (D29, D16, M15); build while paused and keyboard grid placement (D27, M32); overlays readable without colour (V25); repetition automation (M15).
- **Reference games:** APICO automation, always-expanded tooltips, focus mode and buzzing toggle ([Whitethorn](https://whitethorngames.com/apico/accessibility)); Farming Simulator 25 GPS steering and AI workers ([GIANTS](https://www.farming-simulator.com/newsArticle.php?country=us&lang=en&news_id=573)); Stardew Access is a desktop community mod, not native support ([GitHub](https://github.com/stardew-access/stardew-access)).
- **Verify:** place, rotate and demolish; read an overlay without colour; finish any fishing or cooking minigame.

## G22 Visual novel, narrative, adventure, point-and-click
- **Likely signals:** S014, S021, S066, S073–S077, S082, S095, S119.
- **Load:** text-and-readability, screen-reader-and-voice, audio-and-captions, memory-and-navigation; timing-and-speed for timed choices or QTEs; content-and-phobias as content requires.
- **Usually skip unless present:** damage options, traversal, camera inversion in static scenes. Load action guides only for embedded minigames.
- **Must-not-miss:** timed choices can be untimed, because silence otherwise counts as a choice (D29); backlog and manual advance (H33, C01); hotspots reachable without precise clicks (M21, M40); QTE skip (M16).
- **Reference games:** Life is Strange 2 reviewer: timed dialogue read slow selection as silence ([Can I Play That?](https://caniplaythat.com/2020/03/07/teen-friendly-review-series-life-is-strange-2-and-captain-spirit/)); Life is Strange: True Colors QTE skipping ([Can I Play That?](https://caniplaythat.com/2021/10/12/life-is-strange-true-colors-accessibility-review/)); Ren'Py self-voicing is an engine capability that needs integration, not proof of access ([Ren'Py](https://www.renpy.org/doc/html/self_voicing.html)).
- **Verify:** read choices before committing, re-read earlier lines, reach every hotspot by keyboard.

## G23 Walking simulator
- **Likely signals:** S051–S053, S064, S066, S081, S084, S119.
- **Load:** camera-and-motion, memory-and-navigation, text-and-readability, audio-and-captions, input-and-motor.
- **Usually skip unless present:** health, ammo, combat and traversal-and-platforming.
- **Must-not-miss:** camera comfort despite the slow pace (O02, O05, O10); auto-walk (M33); route help on request (C14, C18); notes as readable text (V30).
- **Reference games:** Botany Manor single-stick stop-and-look, FOV and smoothing, blink transitions ([Whitethorn](https://whitethorngames.com/botanymanor/accessibility)); Life is Strange reviewer on disorientation without route guidance ([Can I Play That?](https://caniplaythat.com/2020/01/31/teen-friendly-review-series-life-is-strange-2015/)).
- **Verify:** walk the longest required route, find the next destination, resume after an interruption.

## G24 Party, couch co-op, local multiplayer
- **Likely signals:** S004, S009, S013, S015, S097, S127–S129; S134 for phone controllers.
- **Load:** multiplayer-and-communication, input-and-motor, timing-and-speed, color-and-clarity, text-and-readability, audio-and-captions; touch-and-mobile for phone controllers.
- **Usually skip unless present:** speech transcription in offline, speech-free play; aim assist in trivia.
- **Must-not-miss:** per-player settings that survive controller swaps (K20, L03); extended or no timers (D29); team identity by shape and name (V25); alternatives to drawing or physical gestures.
- **Reference games:** Jackbox Party Pack 10 subtitles, motion settings, extended or no timers, Draw For Me in Tee K.O. 2 ([Jackbox](https://www.jackboxgames.com/blog/accessibility-features-in-the-jackbox-party-pack-10)); Jackbox Party Pack 8 longer timers for stream delay and moderation tools ([Jackbox](https://www.jackboxgames.com/streaming-moderation-accessibility-features-jackbox-party-pack-eight/)).
- **Verify:** players with different settings join, swap controllers, finish a round, disconnect and rejoin.

## G25 Competitive online, esports, voice chat
- **Likely signals:** S123–S126, S130–S133, plus the core loop's signals (often G01, G14 or G19).
- **Load:** multiplayer-and-communication, store-claims-and-compliance, plus the guides for the real mechanics.
- **Usually skip unless present:** story recaps, crafting, platforming; shooter aim for a card game.
- **Must-not-miss:** accessible account, lobby and invite path (K22, R01); play without voice (K05, K07, K18); usable mute, block and report (K17); gameplay assists need stated shared rules, never silent exclusion (K23).
- **Reference games:** Halo Infinite speech-to-text and text-to-speech chat, team outlines ([Halo Support](https://support.halowaypoint.com/hc/en-us/articles/4408122295444-Accessibility-Features-in-Halo-Infinite)); Apex Legends contextual pings ([EA](https://www.ea.com/able/resources/apex-legends/pc)); Street Fighter 6 control types with mode restrictions ([Capcom](https://www.capcom-support.com/hc/en-us/articles/7122030981660-What-are-the-control-types-in-Street-Fighter-6)).
- **Verify:** invite, join, communicate without voice, mute and report, reconnect, read the scoreboard.

## G26 VR and AR
- **Likely signals:** S051, S136 (AR), S137–S143; plus the underlying gameplay signals. XR is not a genre.
- **Load:** vr-and-xr, camera-and-motion, input-and-motor, audio-and-captions; plus guides for the real gameplay.
- **Usually skip unless present:** room-scale settings in non-XR builds; vehicle assists without vehicles.
- **Must-not-miss:** seated mode that reaches every object and menu (M43); one-handed parity (M05); teleport, snap or smooth choice before the first move (O18, O19, O20); captions that follow the view (H30). Never disable guardian or boundary safety.
- **Reference games:** Cosmonious High one-handed mode and height adjustment ([Owlchemy FAQ](https://owlchemylabs.com/faq)); No Man's Sky VR comfort settings; verify headset coverage ([Hello Games](https://www.nomanssky.com/fractal-update/?cli_action=1677074777.055)); W3C XAUR needs, a guideline note, not a certification ([W3C](https://www.w3.org/TR/xaur/)).
- **Verify:** calibrate, then use menus and every required object seated, one-handed, at low and high heights.

## G27 Kids and educational
- **Likely signals:** S003, S012, S022, S073, S097, S134, S153.
- **Load:** kids-and-education, text-and-readability, screen-reader-and-voice, timing-and-speed, input-and-motor; touch-and-mobile for touch.
- **Usually skip unless present:** combat, shooting and traversal assists. Never skip narration because players are "old enough to read".
- **Must-not-miss:** read-aloud instructions with symbols plus words (C25, R12); speed kept apart from the learning goal (D29); replayable demonstrations (C05); scaffolding without public labels.
- **Reference games:** Minecraft Education Immersive Reader and UI narration, with stated controller limits ([Minecraft Education](https://edusupport.minecraft.net/hc/en-us/articles/360061503031-Accessibility-Features)); PBS Duck Dash speed, background and eye/head-tracking options ([PBS report](https://dc79r36mj3c9w.cloudfront.net/prod/filer_public/pbsnc-redesign-phase-1/about/c85308bf39_July_2022_PBS_DEI_AnnualReport.pdf)); PBS guide on differing needs among autistic children ([PBS guide](https://static.pbslearningmedia.org/media/media_files/31536ca5-64a6-4cb7-9df8-e229b90cd985/87845c36-5a69-4d30-ac39-4c61433e17dc.pdf)).
- **Verify:** a child can hear, repeat and act on an instruction with no reading deadline.

## G28 Text-heavy, text-based, interactive fiction
- **Likely signals:** S029, S073, S075, S076, S082, S096, S119.
- **Load:** text-and-readability, screen-reader-and-voice, memory-and-navigation; content-and-phobias for described content.
- **Usually skip unless present:** camera, aiming, traversal, mesh or texture replacement.
- **Must-not-miss:** labelled input fields and semantic structure (R01, R11); narration not cut at paragraph breaks; parser synonyms, history and spelling tolerance (C24, C26); warnings for words, not only images (F12).
- **Reference games:** ChoiceScript forum report of unlabelled fields and interrupted narration ([Choice of Games forum](https://forum.choiceofgames.com/t/accessibility-issues-in-the-choicescript-game-ui-for-screen-reader-users/174983)); Ren'Py self-voicing as an implementation reference ([Ren'Py](https://www.renpy.org/doc/html/self_voicing.html)).
- **Verify:** screen-reader navigation after scene changes; finish without forced advance; save and load.

## G29 Physics sandbox
- **Likely signals:** S022, S024, S052, S058, S060, S106, S150, S151.
- **Load:** structured-ui-and-strategy, input-and-motor, photosensitivity, sensory-load-and-guidance, camera-and-motion; touch-and-mobile or vr-and-xr per platform.
- **Usually skip unless present:** permadeath, combat, hunger, quest recaps. Do not assume a sandbox has no timed missions.
- **Must-not-miss:** pause, step and slow simulation (D27, D14); undo and snapshots (C28, D26); numeric or snapped transforms instead of dragging (M23, M32); effect budgets for chaotic explosions (N07, P05).
- **Reference games:** Teardown Creative Mode and saved creations; its FAQ says console mouse and keyboard are not planned ([Teardown FAQ](https://teardowngame.com/faq.html)). A disability study of this category is UNVERIFIED.
- **Verify:** build, rotate, move, save and restore an object with discrete actions; recover from an unstable simulation.

## Combining recipes

Start from every recipe that matches part of the game, then keep only confirmed signals.

| Game | Recipes to merge | Note |
| --- | --- | --- |
| Turn-based JRPG with a rhythm attack and fishing | G05 + G15 + G21 | Timing guides apply only to the rhythm and fishing scenes |
| Farming sim with combat dungeons and spiders | G21 + G02 + G09 | Add content-and-phobias only for the confirmed spider-like enemies |
| Horror walking sim with a chase | G23 + G09 + G03 | One chase loads traversal and timing for that scene |
| Roguelike deckbuilder with real-time dodging | G20 + G02 | "Roguelike" adds nothing by itself; the dodging adds timing |
| Mobile party game with phone controllers | G24 + G17 | Each phone is a player profile with its own settings |
