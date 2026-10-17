# Shipped game accessibility examples

Research assignment 02. Research date: 2026-10-17.

## Scope and evidence rules

This is a catalogue of reference implementations and cautionary examples, not a promise that a game is accessible to every player.

- Years identify the original release or named edition; feature-update dates are stated separately.
- “Ships” includes documented post-launch updates, not necessarily the launch disc.
- Official developer, publisher, and support material is preferred. Specialist reviews supply observed barriers and practical qualifications.
- A review describes its tested platform and date. An old review is not proof of a present-day absence.
- Community wikis and mod pages are identified. A mod is not a built-in feature.
- **UNVERIFIED** means this research did not establish the claim. It does not prove absence.
- Recommended defaults and implementation rules below are proposals for the skill, not the games' actual defaults.
- No general achievement guarantee is inferred from an accessibility menu. The skill must implement the owner's stronger no-penalty policy.
- “God Mode,” “No Fail,” and “Safe Mode” mean different things in different games.
- Audio-first design, turn-based play, and readable text can be useful design examples without being menu options; these are labelled.
- Safety-related reductions should be available before potentially triggering content. None of these examples establishes medical safety.

## Per-game catalogue

### Action adventures, shooters, and open worlds

| Game | Year | Genre | Notable shipped options and qualifications | Source URL |
|---|---:|---|---|---|
| The Last of Us Part I | 2022 | Narrative action / stealth | Cinematic audio descriptions; screen reader; traversal/combat audio cues; high contrast; navigation assistance; puzzle skipping; granular combat/difficulty adjustment. Speech-to-vibration is a DualSense feature with patch scope. Strong reference for combining access paths. | [Official feature list][tlou1] |
| The Last of Us Part II | 2020 | Narrative action / stealth | Editable vision/hearing/motor presets; remapping; hold/toggle alternatives; navigation/traversal assistance; enhanced listen scanning; high contrast; audio cues; puzzle skip. Do not attribute Part I's cinematic descriptions to the original 2020 release. | [Naughty Dog][tlou2] |
| God of War Ragnarök | 2022 | Action adventure | High-contrast HUD; text/icon sizing; navigation assist; auto pickup; traversal assist; repeated-press alternatives; extended puzzle timing; camera comfort. Later PC release documents reduced companion puzzle hints. Navigation assistance alone does not establish independent blind completion. | [Official options][gow]; [2024 PC features][gowhints] |
| Marvel's Spider-Man 2 | 2023 | Open-world superhero action | Game speed at 30%, 50%, or 70%; chase assist; puzzle simplification; wider dodge/parry windows; combat modifiers; high-contrast shaders. Screen reader and cinematic audio descriptions arrived in March 2024 updates. | [Insomniac options][sm2]; [released update][sm2patch] |
| Ratchet & Clank: Rift Apart | 2021 | Action platformer / shooter | Assignable shortcuts activate game-speed reductions and restore normal speed. Developer explains using the feature for combat and traversal rather than tying it to one global difficulty choice. | [SIE developer article][ratchet] |
| Marvel's Spider-Man / Remastered | 2018 / 2020 | Open-world action | Remastered documentation lists QTE auto-completion, puzzle skipping, button-tap-to-hold alternatives, camera assistance, and visual comfort settings. Treat the documented menu as edition-specific rather than assuming complete parity with the 2018 build. | [Insomniac Remastered support][spiderman] |
| Horizon Forbidden West | 2022 | Open-world action RPG | Custom incoming/outgoing damage; Easy Loot; remapping; hold/toggle alternatives; co-pilot; aim/concentration assistance; configurable HUD, guidance, and camera effects. “Story So Far” and optional ability reminders support returning players. Some automatic assists depend on difficulty. | [Guerrilla feature article][horizon] |
| Gears 5 | 2019 | Cover shooter / co-op | Target Lock reduces aiming precision demands; Microsoft discusses improved subtitles and accessibility support. Confirm difficulty/mode availability in the intended build rather than assuming universal multiplayer support. | [Microsoft feature article][gears] |
| Halo Infinite | 2021 | FPS | Subtitle adjustments; menu narration; incoming speech-to-text and outgoing text-to-speech chat; player colours; reduced blur/shake; “Disable Visual Distractions” for frontend menu animation. That last setting does not promise to remove all gameplay effects. | [Halo Support][halo] |
| Sea of Thieves | 2018 | Online co-op sandbox | Single-stick navigation combines camera centring, interaction-prompt locking, and automatic floating. Hub also documents communication/narration settings. Useful for reducing simultaneous movement/camera demands. | [Official hub][sea]; [single-stick release notes][seapatch] |
| Assassin's Creed Valhalla | 2020 | Open-world action RPG | Separate combat/stealth/exploration challenge; guaranteed assassination; aim assist; hold duration; single press/hold/repeated-press QTE choices; some boss-QTE bypasses; captions; shake/gore controls. Menu narration has documented coverage gaps. | [Ubisoft accessibility article][valhalla] |
| Assassin's Creed Mirage | 2023 | Stealth action adventure | Aim assist, difficulty, subtitle presentation, remapping, and guaranteed pickpocketing that removes its QTE. Reference for removing a precision minigame independently of the rest of stealth. | [Ubisoft Help][mirage]; [spotlight, French][miragespot] |
| Cyberpunk 2077 | 2020 | First-person action RPG | Patch 2.1 consolidated accessibility settings and added larger interface text/HUD and other adjustments. Documentation distinguishes cinematic/radio/overhead subtitles, aim-assist categories, camera sway, and centre-screen dot. Do not project 2.1 features onto older console versions. | [CD Projekt Red version-scoped list][cyberpunk] |
| Starfield | 2023 | Space action RPG | Launch review documents Large Font Mode and remaining barriers. Update 1.11.36 added granular gameplay settings and improved surface maps in May 2024. Pair a launch verdict with later patch evidence. | [Launch accessibility review][starfield]; [Bethesda patch][starfieldpatch] |
| Returnal | 2021 | Third-person roguelike shooter | Remapping and UI customization accompany a demanding action loop. Developer discusses dense multisensory low-health feedback; Steve Saylor's launch review finds available help insufficient for his needs. Redundant feedback can also overwhelm. | [Developer UX article][returnal]; [Steve Saylor video][returnalreview] |
| Alan Wake 2 | 2023 | Survival horror / narrative | Anniversary Update 1.2.2 added Gameplay Assist, including immortality, infinite ammunition/flashlight batteries, one-shot kills, and input assistance. Separate hold/toggle choices and automatic/single-tap repeated-input resolution are documented. Retrospective describes scripted-death bugs found during implementation. | [Anniversary update][alan]; [implementation retrospective][alanimpl] |
| Dead Space remake | 2023 | Survival horror | Remapping, menu narration, subtitle customization, content warnings, and hiding selected disturbing scenes. Concealment does not silence the scene's audio. Strong warning/concealment reference with explicit limits. | [EA feature article][deadspace]; [content description][deadspacewarning] |
| Resident Evil 4 remake | 2023 | Survival horror / shooter | Visual/audio/motion-sickness presets; captions; subtitle size/speaker colours; adjustable aim lock; HUD opacity/off. Specialist PC review reports input and window-focus barriers. Built-in yellow-paint removal is **UNVERIFIED**; PC mods are separate. | [Can I Play That?][re4]; [third-party paint mod][re4mod] |
| Star Wars Jedi: Survivor | 2023 | Action adventure / platforming | Slow mode and automatic combat slowdown; QTE auto-success; fall-damage removal; navigation icons/directional audio pings; high contrast; arachnophobia mode. Disabling navigation assistance is not proof of removing painted textures. | [EA accessibility resources][jedi] |
| Star Wars Outlaws | 2024 | Open-world stealth / action | Explorer Mode removes guiding colour from important world elements; configurable markers/HUD; gameplay high contrast; readable typeface; cinematic audio description; precision-input assists. Best verified modern guidance-colour removal example here. | [Ubisoft spotlight][outlaws] |
| Shadow of the Tomb Raider | 2018 | Exploration / action adventure | Independent combat/traversal/puzzle difficulty lets navigation challenge vary separately. Official article verifies separate controls; exact removal of painted traversal marks at a particular setting needs more specific evidence. | [Official hands-on][tombraider] |
| Uncharted 4: A Thief's End | 2016 | Cinematic action adventure | Hold instead of mash and aim assistance followed consultation with a disabled player. Useful older example of removing a progression-blocking interaction without rebuilding the game. | [Reporting on Naughty Dog's explanation][uncharted] |
| DOOM Eternal | 2020 | Fast FPS | Official platform disclosure lists colour alternatives, stick sensitivity, and a consequence-free practice environment. Does not establish a comprehensive no-flash mode or blind-playable campaign. | [PlayStation disclosure][doom] |
| Control | 2019 | Third-person action adventure | Free August 2020 update added separately configurable Assist Mode features. Reference for retrofitting assistance without requiring one monolithic preset. | [Remedy-authored Xbox article][control] |
| Psychonauts 2 | 2021 | 3D platform adventure | Separate No Fall Damage and invincibility; official description qualifies invincibility as preventing damage from most attacks. Useful for separating combat and traversal failure. | [Double Fine support][psychonauts] |

### RPGs, fighting games, platformers, and demanding action

| Game | Year | Genre | Notable shipped options and qualifications | Source URL |
|---|---:|---|---|---|
| Celeste | 2018 | Precision platformer | Assist Mode includes reduced speed and invincibility; changelog records Dash Assist, grab-as-toggle, directional bindings, Photosensitive Mode fixes, and revised Assist Mode wording. Reference for individual movement assists and respectful explanation. | [Developer changelog][celeste]; [developer interview][celesteinterview] |
| Hades | 2020, 1.0 | Action roguelike | God Mode increases damage resilience and strengthens after deaths. It is not literal invulnerability. FAQ frames it as accommodating different challenge preferences. | [Supergiant FAQ][hades] |
| Hades II | 2025, 1.0 | Action roguelike | God Mode returns; review also documents auto-fire/auto-sprint, aim assist, dead zones, subtitles, vibration, and effect adjustments. Official FAQ confirms September 2025 release; distinguish 2024 Early Access. | [Release FAQ][hades2]; [tested accessibility section][hades2review] |
| Diablo IV | 2023 | Loot-driven action RPG | Remapping; skill/action-wheel hold/toggle choices; loot audio cues; item highlighting; built-in/third-party screen-reader support. Later work added compass and audio navigation. Inventory-heavy action reference. | [Blizzard launch features][diablo]; [later navigation][diablonav] |
| Street Fighter 6 | 2023 | Fighting | Sound accessibility communicates distance, attacks, and health; Modern/Dynamic controls simplify input. Launch coverage warns missing menu/tutorial narration and mode restrictions prevent independence. Accessible combat is not whole-game blind access. | [Can I Play That?][sf6]; [Capcom developer account][sf6capcom] |
| Mortal Kombat 1 | 2023 | Fighting | First-launch screen-reader setup; descriptive audio for story, Fatalities, and Fatal Blows; adjustable fight cues; Invasions navigation sounds; Test Your Might mash assistance. Useful comparison for access outside fights. | [WB Games support][mk1] |
| Baldur's Gate 3 | 2023 | Party turn-based RPG | In-conversation history supports recovering a lost train of thought. Patch 5 added Custom Mode. Turn-based combat reduces time pressure by design; it does not prove every interface/exploration/reaction is accessible. | [Larian community update][bg3]; [Patch 5][bg3patch] |
| Final Fantasy XVI | 2023 | Action JRPG | Timely Accessories support one-button combos, auto evasion, and auto potions. Active Time Lore supplies context during cutscenes. Imitate behaviours and contextual lore while evaluating access tied to equipment. | [Square Enix explanation][ff16]; [Active Time Lore][ff16lore] |
| Dragon Quest XI S | 2019 | Turn-based JRPG | A story recap when returning is specifically praised after a reviewer's break. Strong session re-entry reference. Turn-based structure is not a substitute for narration or text scaling. | [Nintendo Wire review][dq11] |
| Persona 5 Royal | 2019 Japan / 2020 West | Turn-based JRPG / social sim | Dialogue log and optional auto-advance support recovery/reading pace. Review finds inconsistent subtitles, no size increase, and omitted background speech. Copy log/pacing, not caption gaps. | [Deaf/HoH review][persona] |
| Dead Cells | 2018; assist update 2022 | Action roguelite / metroidvania | Breaking Barriers added biome continues, auto-hit, independent enemy health/damage and trap damage, easier parry, text/HUD and outline options. Developer explicitly says options do not disable achievements. | [Official Update 29][deadcells]; [achievement policy][deadcellsach] |
| TUNIC | 2022 | Isometric action / puzzles | Separate No Fail Mode and No Stamina Restrictions reduce combat barriers. These alone do not explain knowledge puzzles or provide accessible hints. | [Xbox discussion of settings][tunic] |
| Elden Ring | 2022 | Action RPG / soulslike | Remapping, camera settings, subtitles, HUD, and tutorials exist; specialist coverage identifies incomplete remapping paths and limited presentation settings. Summons/build choices do not substitute for configurable access. | [Menu deep dive][elden]; [review][eldenreview] |
| Sekiro: Shadows Die Twice | 2019 | Timing-intensive action | Visible awareness indicators supplement stealth. Reviewer found fixed subtitle presentation, absent speaker labels, and physical barriers. Their theory about particular audio timing cues was uncertain and must not become a verified mechanics claim. | [Deaf accessibility review][sekiro] |
| Hollow Knight | 2017 | Action metroidvania | Listed features include remapping, control-map display, readable text, and audio levels. Not a verified example of built-in invincibility/difficulty sliders. Mods must be recorded separately. | [FGD data via Nintendo Life][hollow] |
| Super Mario Bros. Wonder | 2023 | 2D platformer | Yoshi/Nabbit avoid enemy damage but do not use power-ups like other characters. Assistance is character-bound. Later support describes Switch 2 edition Assist Mode; do not backport it to the original. | [Original Nintendo page][mario]; [edition-sensitive support][mariosupport] |
| Kirby and the Forgotten Land | 2022 | 3D platformer | Spring-Breeze Mode alongside Wild Mode; developers describe switching in Waddle Dee Town/world map. Friendly difficulty presentation, with switching locations to explain. | [Nintendo developer interview][kirby] |
| Hi-Fi RUSH | 2023 | Rhythm action | Current support documents Auto-Action and single-key rhythm controls. Auto-Action still expects beat-timed attack input; some techniques are unavailable. Not fully automatic play or removal of all rhythm demands. | [KRAFTON guide][hifi] |
| Prince of Persia: The Lost Crown | 2024 | Action metroidvania | Guided Mode, high contrast, combat tuning, and Memory Shards pinning screenshots to map positions. Excellent for remembering obstacles and returning with newly acquired abilities. | [Developer interview][prince]; [Ubisoft video][princevideo] |
| Chicory: A Colorful Tale | 2021 | Painting adventure / puzzles | Boss skipping, reaction-speed adjustment, and in-world hints praised by reviewer. Indie reference separating reaction demands from exploration/creativity. | [Can I Play That?][chicory] |

### Additional strategy and edition-specific examples

| Game | Year | Genre | Notable shipped options and qualifications | Source URL |
|---|---:|---|---|---|
| Age of Empires IV | 2021 PC / 2023 Xbox | Real-time strategy | First-launch/main-menu narration; campaign Story Mode; colour access. PC and Xbox have separate documentation; the PC page points to Windows colour filters. Do not infer all battlefield information is narrated. | [PC features][aoe]; [Xbox features][aoexbox] |
| The Last of Us Part II Remastered | 2024 | Narrative action / stealth | Official support verifies cinematic audio descriptions. This closes an important edition distinction: the original Part II row must not silently inherit a remaster feature. | [Naughty Dog support][tlou2ad] |

### Racing, flight, sports, and rhythm

| Game | Year | Genre | Notable shipped options and qualifications | Source URL |
|---|---:|---|---|---|
| Forza Motorsport | 2023 | Circuit-racing simulation | Blind Driving Assists supply configurable track cues and cue previews; One Touch Driving reduces inputs; screen narration; dynamic cinematic description responds to weather/time. Combine track access, menu access, and cue training. | [Turn 10 article][forzam]; [independent preview][forzamreview] |
| Forza Horizon 5 | 2021 | Open-world arcade racing | March 2022 update added picture-in-picture ASL/BSL for cinematics, with position/background settings. Later notes confirm every then-existing Horizon Story chapter covered. Name languages and content scope explicitly. | [Series 5 patch][forzah]; [2023 coverage][forzahstories] |
| Microsoft Flight Simulator | 2020 | Flight simulation | Assistance presets/individual assists; remapping; text size; subtitles; cockpit interaction alternatives; AI help. Official page notes incomplete narration and imperfect AI. | [MSFS 2020 documentation][msfs] |
| Microsoft Flight Simulator 2024 | 2024 | Flight simulation | Separate documentation covers system-voice UI narration, assistance, and visual navigation aids. Do not assume 2020's coverage/language limits persist unchanged. | [MSFS 2024 documentation][msfs24] |
| Rocket League | 2015 | Vehicle sports | Camera shake/FOV/distance and Ball Cam feedback; Free Play training controls and game speed. Training speed does not establish competitive slow motion. | [Camera guide][rocket]; [Free Play options][rockettraining] |
| EA Sports FC 24 | 2023 | Football / soccer | Menu narration/navigation remapping; chat conversion; colour options; pass/shot/cross assists; auto player switching; difficulty/sliders. Remapping has scheme restrictions. | [EA resources][fc] |
| Madden NFL 24 | 2023 | American football | Narrated settings/play calls; assistive vibration; open-receiver button narration; defensive assists. Guide limits subtitles to Superstar cutscenes: do not claim fully captioned play. | [EA resources][madden] |
| NBA 2K24 | 2023 | Basketball | Real Player % alternative to user timing; shot feedback/meter configuration. Community-tested MyNBA sliders tune challenge. Evidence does not establish every online/MyCareer mode's availability. | [Specialist settings catalogue][nba]; [MyNBA sliders][nbaslid] |
| Tetris Effect / Connected | 2018 / 2020 | Falling-block puzzle | PC particle amount/size options; Zone time mechanic. Review praises mute play but notes distraction and limited UI/playfield sizing. Zone is a core mechanic, not a global access slow-mode toggle. | [Official PC options][tetris]; [DAGERSystem archive review][tetrisreview] |
| Beat Saber | 2019, 1.0; EA 2018 | VR rhythm | No Fail/practice support; one-saber content reduces limb demands. Primary 2025 research reports one-handed participants facing content/difficulty restrictions and discomfort in two-hand No Fail. One-saber playlists are not universal one-handed support. | [Developer storefront][beatsaber]; [research study][beatstudy]; [community mode reference][beatwiki] |

### Multiplayer communication and competitive readability

| Game | Year | Genre | Notable shipped options and qualifications | Source URL |
|---|---:|---|---|---|
| Fortnite | 2017 | Battle royale / action | Visualize Sound Effects supplies radial indicators for selected sounds. Epic says “some sounds”; not a full transcript or proof of identical coverage across Fortnite experiences. | [Epic support][fortnite] |
| Apex Legends | 2019 | Squad battle royale | Contextual pings communicate without speech; opacity, subtitle size, colour options, chat conversion. EA identifies benefits for speech/hearing/cognitive access. | [Settings][apex]; [EA ping-access description][apexping] |
| Overwatch 2 | 2022 | Team hero FPS | Season 3 expanded subtitle size, speaker portrait/name, text/background colours, and preview. Separate voice-chat transcription for reports is moderation, not a verified player-facing live-caption setting. | [Season 3][overwatch]; [moderation scope][owtranscription] |
| VALORANT | 2020 | Tactical FPS | Riot discloses colour alternatives; developer explains clarity across graphics-quality levels. General flash reduction/blind-playable combat **UNVERIFIED**; visual clarity alone proves neither. | [Riot disclosure][valorant]; [clarity article][valorantclarity] |
| Call of Duty: Modern Warfare III | 2023 | Military FPS | Low Motor Strain preset; controller/dead-zone tuning; blur reduction; tagged/previewable settings; campaign high contrast. Do not imply unrestricted multiplayer parity. | [Official explanation][codmw3] |
| Call of Duty: Black Ops 6 | 2024 | FPS / Zombies | Arachnophobia toggle changes spider-like Zombies enemies without changing gameplay. Appearance filtering preserves threat mechanics. | [Official accessibility updates][codbo6] |
| Battlefield 2042 | 2021 | Large-scale FPS | Menu narration; subtitles; team/enemy/HUD colours; minimap opacity; remapping across on-foot/vehicle/gunner/pilot contexts. | [EA PC resources][battlefield] |
| Destiny 2 | 2017 | Shared-world FPS / RPG | Full Auto Firing reduces repeated presses; targeted-reticle colours; further auto-melee/subtitle-colour changes documented. Dated “next season” statements are not an exact current menu map. | [Bungie changes][destiny] |

### Survival, phobia filters, cozy play, and audio-first games

| Game | Year | Genre | Notable shipped options and qualifications | Source URL |
|---|---:|---|---|---|
| Grounded | 2022, 1.0; EA 2020 | Survival crafting | Graduated Arachnophobia Safe Mode reduces spider-like appearance without changing difficulty. Also narration/communication settings. Appearance reduction is not reduced aggression. | [Obsidian documentation][grounded] |
| Hogwarts Legacy | 2023 | Open-world action RPG | May 2023 arachnophobia patch changes models/sounds, removes small-spider effects, hides corpses; Field Guide images remain unchanged. Explicitly documented coverage gap. | [Official May 4 patch, Spanish][hogwarts] |
| Satisfactory | 2024, 1.0; EA 2019 | Factory / exploration | Arachnophobia cat-image replacement; separate hostility controls. Developer-stream archive describes full arachnid removal as another setting. Appearance/passivity/removal differ. | [Official community wiki][satisfactory]; [developer transcript archive][satisfactoryremove] |
| Phasmophobia | 2020, Early Access | Co-op investigation horror | Text alternatives for Spirit Box/Ouija questions reduce mandatory speech. Requested arachnophobia mode **UNVERIFIED** in official updates/community settings checked. Do not substitute Lethal Company's option. | [Kinetic Tempest notes][phasmo]; [console text fallback][phasmopatch]; [community inventory][phasmooptions] |
| Lethal Company | 2023, Early Access | Co-op extraction horror | Community v45 record documents spider-model replacement with “spider” text. Weaker evidence than official patch notes; distinct from the unverified Phasmophobia request. | [Community version record][lethal] |
| World of Warcraft: The War Within | 2024 expansion | MMORPG | Arachnophobia mode; developer discussion describes crab replacements. Quest-icon differentiation also improved. Does not establish coverage of every legacy/promotional spider image. | [Official UI article][wow]; [developer chat][wowcrabs] |
| Subnautica | 2018, 1.0 | Underwater survival | Freedom removes food/water management; Creative removes further survival/resource pressures. Not a sea-creature filter. Mobile publisher's access options require separate platform attribution. | [Community game modes][subnautica]; [mobile publisher help][subnauticamobile] |
| Stardew Valley | 2016 | Farming / life sim | UI scale/zoom, Show Flash Effects, audio channels. Stardew Access supplies screen-reader support as a mod. “Cozy” does not remove time, fishing, or combat barriers. | [Game wiki][stardew]; [accessibility mod][stardewmod] |
| Minecraft | 2011, 1.0 | Sandbox / survival | Official hub documents narration, text, input, and subtitles across titles/editions. Verify edition-specific support; Java subtitles are not proof of Bedrock parity. | [Official accessibility hub][minecraft] |
| Animal Crossing: New Horizons | 2020 | Social / life sim | Large dialogue bubbles and low-pressure sequential interactions help by design; blind/low-vision barriers remain. Not a verified comprehensive TTS/accessibility-menu example. | [General review][animal]; [vision review][animalvision] |
| Pokémon Sword / Shield | 2019 | Turn-based RPG | Casual Controls ease single-Joy-Con/one-handed operation. Do not generalize the feature to every Pokémon title. | [Controls guide][pokemon]; [blind-player reporting][pokemonreview] |
| A Short Hike | 2019 | Exploration / platformer | Developer-pinned FAQ explains reducing/removing pixelation through Pixel Size. Reference for optional art treatment when readability/comfort requires it. | [Developer FAQ][shorthike] |
| Night in the Woods | 2017 | Narrative exploration | Text dialogue; controller remapping; listed high-contrast text. TTS and rhythm-minigame bypass **UNVERIFIED**. | [PCGamingWiki][night]; [accessibility listing][nightreview] |
| The Vale: Shadow of the Crown | 2021 | Audio-first action adventure | Nonvisual navigation/combat are core design. Valuable spatial-audio reference, not evidence of Deaf access. | [Developer storefront][vale] |
| A Blind Legend | 2015 mobile / 2016 PC | Audio-first action | Publisher describes blind/low-vision access via binaural audio and mobile touch interaction. Controls are platform-specific; equivalent visual/caption route not established. | [Publisher App Store description][blindlegend] |
| Unpacking | 2021 | Object-placement narrative | Allow Items Anywhere removes placement-rule pressure; zoom supports inspection. Placement freedom does not automatically identify objects. | [Developer explanation][unpackingdev]; [Apple developer profile][unpacking] |
| Wordle | 2021 | Daily web word puzzle | Dark theme and orange/blue high contrast. Browser issue shows forced-colour compatibility can still fail. Comprehensive current NYT screen-reader support **UNVERIFIED**. | [Feature history, secondary][wordle]; [Mozilla issue][wordlecontrast] |
| Wordle Global | First-release year **UNVERIFIED** | Wordle-like web puzzle | Own statement claims keyboard, screen-reader, and high-contrast support. Requires interaction testing; not independent proof of live-announcement quality. | [Project statement][wordleglobal] |
| Life is Strange: True Colors | 2021 | Narrative adventure | Colour-vision options for auras; demanding-input skips; optional brightness/volume warnings. Useful colour-driven narrative reference. | [Developer-authored Xbox article][lis] |
| It Takes Two | 2021 | Required two-player platformer | Review documents contrast/brightness and chat conversion. Does not remove partner requirement or varied real-time mechanics. Verify each partner/minigame separately. | [Review access section][ittakes] |
| SOMA | 2015; Safe Mode 2017 | Narrative horror | Safe Mode retains creatures/atmosphere while preventing attacks/death; developer changes creature behaviour rather than disabling only damage. | [Frictional explanation][soma] |
| Outer Wilds: Echoes of the Eye | 2021 expansion | Exploration mystery / horror | Reduced Frights is verified. Announcement does not specify every change, so do not promise elimination of darkness/startles/fear. | [Developer announcement][outerwilds] |
| Pentiment | 2022 | Historical narrative RPG | Easy-read font removes some decorative fonts/writing effects; scaling, high contrast, TTS. Reference for an optional readable treatment preserving default art. | [Official site][pentiment]; [Xbox article][pentimentxbox] |
| Final Fantasy VII Rebirth | 2024 | Action JRPG | Guidance-verification case: built-in paint-removal toggle **UNVERIFIED**; 2025 PC mod verified. Debate/mods do not establish an official patch. | [Official updates][ff7patch]; [mod author][ff7mod]; [director interview reporting][ff7debate] |

## Focused findings

### Yellow paint and guidance removal

Guidance can mean a texture, outline, route line, objective icon, hint voice, or scan.
A switch for one is not evidence that all can be removed.

| Example | Verified | Do not infer |
|---|---|---|
| [Star Wars Outlaws][outlaws] | Explorer Mode removes guiding colour on important elements. | Every HUD marker/hint also disappears. |
| [Jedi: Survivor][jedi] | Navigation icons/audio pings are configurable. | Painted environmental materials disappear. |
| [Horizon Forbidden West][horizon] | Minimal/marked navigation and climbing annotations are configurable. | All authored climbing paint disappears. |
| [Ragnarök PC][gowhints] | 2024 option reduces companion puzzle hints. | All hints or traversal paint are removed. |
| [Shadow of the Tomb Raider][tombraider] | Independent traversal/puzzle/combat controls. | Exact paint removal from this broad source alone. |
| [Resident Evil 4][re4] | HUD controls and a separate [paint mod][re4mod]. | Shipped paint toggle: **UNVERIFIED**. |
| [FFVII Rebirth][ff7patch] | Public debate and a [PC mod][ff7mod]. | Official paint-removal patch: **UNVERIFIED**. |
| [Alan Wake 2][alan] | Post-launch gameplay assists. | Yellow-paint removal: **UNVERIFIED**. |

No causal claim is established that a studio added a setting because of the 2023–2024 debate.
Evidence establishes dated features/public discussion, not internal decision processes.
Recommended design: separate world colour, outlines, breadcrumbs, markers, and unsolicited hints.
Offer “off” where feasible and retain on-demand help.
Different players may want more or less guidance; neither choice measures intelligence or competence.

### Phobia filters

| Reference | Established scope | Caution |
|---|---|---|
| [Grounded][grounded] | Graduated spider appearance reduction. | Difficulty/aggression remain. |
| [Hogwarts Legacy][hogwarts] | Models, some sounds, small-spider effects, corpses. | Field Guide pictures remain; collision retained. |
| [Jedi: Survivor][jedi] | Creature changed to be less arachnid-like. | Narrower than every spider-related image. |
| [Satisfactory][satisfactory] | Cat replacement; separate hostility/removal. | Replacement can still startle; test with affected players. |
| [WoW][wow] | Arachnophobia creature changes. | Audit legacy/new content separately. |
| [Black Ops 6][codbo6] | Appearance changes without gameplay changes. | Movement/threat may still trigger fear. |
| [Lethal Company][lethal] | Community record: spider becomes text. | Does not establish associated sound removal. |
| [Phasmophobia][phasmooptions] | Requested mode **UNVERIFIED**. | Speech alternative solves another barrier. |

A dedicated shipped **trypophobia filter was not verified**.
Search results included warnings, requests, and guidelines, not adequate implemented-toggle evidence.
Do not turn those into a success example.
For clustered holes/nests/lesions/repeated organic patterns, route to asset review.
[Microsoft trigger guidance][phobiaguideline] identifies phobia categories; guidance is not proof of a shipped filter.

Other verified intensity models include [Dead Space warnings/concealment][deadspace], [SOMA Safe Mode][soma], and [Outer Wilds Reduced Frights][outerwilds].
They address different barriers and are not interchangeable phobia modes.

### Reduced visual noise and minimal HUD

- [Halo Infinite][halo]: menu animation suppression; blur/shake reduction.
- [Resident Evil 4][re4]: HUD opacity/off and damage-display presentation.
- [Horizon Forbidden West][horizon]: selected HUD information and reduced camera effects.
- [Star Wars Outlaws][outlaws]: guidance placement, reminders, guiding-colour removal.
- [Battlefield 2042][battlefield]: HUD colours, hit indicators, minimap opacity.
- [Tetris Effect PC][tetris]: particle amount/size; verify console parity separately.
- [A Short Hike][shorthike]: remove pixelation instead of merely lowering graphics quality.
- [Pentiment][pentiment]: easy-read treatment removes some writing effects.
- [Stardew Valley][stardew]: flash-effects toggle, not a medical safety guarantee.

Recommended design: independent decoration, telemetry, warnings, captions, and assist layers.
When hiding a layer, retain an accessible way to retrieve essential information.

### Memory and cognitive support

| Need | Reference | Transferable pattern |
|---|---|---|
| Return after a break | [Dragon Quest XI S][dq11] | Brief story recap at re-entry. |
| Recover story/procedure | [Horizon Forbidden West][horizon] | Story So Far, reminders, informative loading screens. |
| Understand current people/places | [Final Fantasy XVI][ff16lore] | Contextual lore during scenes. |
| Recover a conversational thread | [Baldur's Gate 3][bg3] | History without leaving the conversation. |
| Re-read at own pace | [Persona 5 Royal][persona] | Log and optional auto-advance. |
| Remember an obstacle | [The Lost Crown][prince] | Screenshot linked to map position. |
| Get help on demand | [Chicory][chicory] | In-world hints with player-controlled access. |
| Learn nonvisual information | [Forza Motorsport][forzam] | Cue preview before use. |

These examples do not establish clinical efficacy.
Recommend separate recaps, objectives, history, lore lookup, and graded hints.
A journal repeating a vague objective does not replace a recap or concrete next action.

## Per-option index: best reference implementations

These identify a behaviour worth studying, not an endorsement of every feature or restriction in that game.
Baseline means every project should consider the capability; Contextual means a named mechanic/content signal activates it.
Advanced means substantially higher production or validation cost.

| Option / pattern | Best-practice games or useful comparisons | Tier and main qualification |
|---|---|---|
| Editable access presets | [The Last of Us Part II][tlou2], [Ragnarök][gow] | Baseline menu pattern; individual values remain editable. |
| Discoverable first-launch setup | [Mortal Kombat 1][mk1], [Age of Empires IV][aoe] | Baseline; setup itself must be usable. |
| Screen-reader/menu narration | [Diablo IV][diablo], [Mortal Kombat 1][mk1] | Baseline foundations; full custom narration may be Advanced. |
| Nonvisual gameplay cues | [The Last of Us Part II][tlou2], [Forza Motorsport][forzam] | Contextual/Advanced; include training and information coverage. |
| Cinematic audio description | [Part I][tlou1], [Part II Remastered][tlou2ad], [Spider-Man 2][sm2patch] | Advanced; identify languages and cinematics covered. |
| Dynamic cinematic description | [Forza Motorsport][forzam] | Advanced; dynamic scene state needs matching description. |
| Spoken dialogue through haptics | [The Last of Us Part I][tlou1] | Advanced, device-specific; supplementary channel. |
| Sign-language interpretation | [Forza Horizon 5][forzah] | Advanced; ASL and BSL are distinct languages. |
| Subtitle size/background/speaker | [Ragnarök][gow], [Dead Space][deadspace] | Contextual to speech; accessible default presentation. |
| Non-dialogue captions | [Resident Evil 4][re4], [Jedi: Survivor][jedi] | Contextual to informative sound. |
| Directional captions/indicators | [Jedi: Survivor][jedi], [Fortnite][fortnite] | Contextual; avoid asserting every sound is represented. |
| Live chat speech/text alternatives | [Apex Legends][apex], [Halo Infinite][halo] | Contextual to online communication. |
| Speech-free interaction with game entities | [Phasmophobia][phasmo] | Contextual to microphone mechanics. |
| Contextual nonverbal team ping | [Apex Legends][apexping] | Contextual to team coordination. |
| Cue-volume categories | [The Last of Us Part I][tlou1], [Mortal Kombat 1][mk1] | Contextual; protect informative cues from masking. |
| Audio cue glossary/preview | [Forza Motorsport][forzam], [Part I][tlou1] | Contextual; teach before testing. |
| One-stick movement/camera help | [Sea of Thieves][sea] | Contextual to dual-stick navigation. |
| One-handed control layout | [Pokémon Sword/Shield][pokemon] | Contextual; cover exploration as well as menus. |
| Full context-aware remapping | [Battlefield 2042][battlefield], [Horizon Forbidden West][horizon] | Baseline; inspect menu and vehicle exceptions. |
| Toggle instead of sustained hold | [Alan Wake 2][alanimpl], [Diablo IV][diablo] | Contextual to sustained actions. |
| Hold instead of rapid mash | [Uncharted 4][uncharted], [Valhalla][valhalla] | Contextual; long holds need their own alternatives. |
| Single-press/automatic QTE completion | [Spider-Man Remastered][spiderman], [Jedi: Survivor][jedi] | Contextual; preserve progression and story. |
| Auto-fire/repeated attack | [Destiny 2][destiny], [Hades II][hades2review] | Contextual; respect action cadence/resources. |
| Auto pickup/traversal | [Ragnarök][gow], [The Last of Us Part II][tlou2] | Contextual; remove repetitive low-value input. |
| Aim lock / aim assistance | [Gears 5][gears], [Spider-Man 2][sm2] | Contextual; mode scope must be explicit. |
| Simplified combat controls | [Street Fighter 6][sf6], [Final Fantasy XVI][ff16] | Contextual; review restrictions/costs before copying. |
| Game-speed adjustment | [Rift Apart][ratchet], [Spider-Man 2][sm2] | Contextual; global time interactions need validation. |
| Combat-only slow mode | [Jedi: Survivor][jedi] | Contextual; allow manual and automatic activation. |
| Wider timing/parry windows | [Spider-Man 2][sm2], [Dead Cells][deadcells] | Contextual to short reaction windows. |
| Extended puzzle timing | [Ragnarök][gow] | Contextual; independent from enemy damage. |
| Puzzle skip/simplification | [Spider-Man Remastered][spiderman], [Part I][tlou1] | Contextual; preserve result and reward state. |
| Boss-fight skip | [Chicory][chicory] | Contextual; avoid removing required story rewards. |
| Invulnerability/no fail | [TUNIC][tunic], [Psychonauts 2][psychonauts] | Contextual; define exceptions in plain language. |
| Damage resilience that grows | [Hades][hades], [Hades II][hades2review] | Contextual; resistance is not invulnerability. |
| Infinite stamina | [TUNIC][tunic] | Contextual to resource-gated mobility/combat. |
| Infinite ammunition / flashlight batteries | [Alan Wake 2][alan] | Contextual; test scripted resource checks. |
| One-shot kills | [Alan Wake 2][alan] | Contextual; test bosses and phase transitions. |
| Separate enemy health/damage/trap damage | [Dead Cells][deadcells] | Contextual; independent sliders preserve preferred challenge. |
| Independent challenge domains | [Valhalla][valhalla], [Tomb Raider][tombraider] | Contextual; combat, exploration, puzzles need not share one setting. |
| No fall damage | [Psychonauts 2][psychonauts], [Jedi: Survivor][jedi] | Contextual; distinguish damage from reset/death. |
| Checkpoint/biome continues | [Dead Cells][deadcells] | Contextual; retain local progress without achievement loss. |
| Friendly-creature / nonlethal horror mode | [SOMA][soma], [Satisfactory][satisfactory] | Contextual; intensity and failure are separate. |
| Survival-pressure removal | [Subnautica][subnautica] | Contextual; distinguish hunger/thirst, oxygen, damage, resources. |
| Free placement / correctness bypass | [Unpacking][unpackingdev] | Contextual to object-placement puzzles. |
| Readable font alternate | [Pentiment][pentiment], [Outlaws][outlaws] | Baseline typography foundations; optional art replacement. |
| UI/HUD/text scaling | [Age of Empires IV][aoe], [Cyberpunk 2077][cyberpunk] | Baseline; measure actual screen coverage. |
| Semantic colour alternatives | [Battlefield 2042][battlefield], [Destiny 2][destiny] | Baseline non-colour encoding; Contextual customization. |
| High-contrast world rendering | [Part II][tlou2], [Jedi: Survivor][jedi] | Contextual/Advanced; categories/background need independent control. |
| Adjustable camera motion/blur | [Ragnarök][gow], [Halo Infinite][halo] | Contextual to camera/effects motion. |
| Centre-screen reference dot | [Cyberpunk 2077][cyberpunk], [Ragnarök][gow] | Contextual to moving cameras; user choice. |
| Menu animation reduction | [Halo Infinite][halo], [Pentiment][pentiment] | Contextual to animated backgrounds/text. |
| Particle/noise reduction | [Tetris Effect PC][tetris] | Contextual; preserve gameplay information. |
| Remove pixelation treatment | [A Short Hike][shorthike] | Contextual to readability/comfort. |
| HUD opacity / minimal HUD | [Resident Evil 4][re4], [Horizon][horizon] | Contextual; make essential status retrievable. |
| Guiding-colour removal | [Star Wars Outlaws][outlaws] | Contextual; distinguish texture/material changes from icons. |
| Guidance icons/audio on demand | [Jedi: Survivor][jedi], [Part II][tlou2] | Contextual to spatial navigation. |
| Reduced unsolicited puzzle hints | [Ragnarök PC][gowhints] | Contextual; does not imply every hint is suppressed. |
| Arachnophobia appearance slider | [Grounded][grounded] | Contextual to spider imagery/movement. |
| Spider audio/secondary-asset filtering | [Hogwarts Legacy][hogwarts] | Contextual; document known unfiltered assets. |
| Content warning vs concealment | [Dead Space][deadspace] | Contextual; two controls, not one. |
| Reduced fright/intensity | [Outer Wilds expansion][outerwilds] | Contextual; name what remains. |
| Brightness/volume event warnings | [Life is Strange: True Colors][lis] | Contextual; warning is not reduction. |
| Story recap at return | [Dragon Quest XI S][dq11], [Horizon][horizon] | Contextual to long/serialized narratives. |
| Contextual lore lookup | [Final Fantasy XVI][ff16lore] | Contextual to dense named people/places. |
| Conversation log | [Baldur's Gate 3][bg3], [Persona 5 Royal][persona] | Contextual to dialogue/choices. |
| Player-controlled text advance | [Persona 5 Royal][persona] | Contextual to authored dialogue text. |
| Map screenshot memory | [The Lost Crown][prince] | Contextual to revisiting obstacles. |
| Graded/in-world hints | [Chicory][chicory] | Contextual to puzzles/exploration. |
| Safe practice and rehearsal | [DOOM Eternal][doom], [Rocket League Free Play][rockettraining] | Contextual to complex skills. |
| Per-task driving/flight assistance | [Forza Motorsport][forzam], [MSFS][msfs] | Contextual; motor assistance is not necessarily auto-win. |
| Remove shot-timing requirement | [NBA 2K24][nba] | Contextual; verify mode restrictions. |
| No achievement penalty for assistance | [Dead Cells developer policy][deadcellsach] | Baseline product rule for this skill; do not infer for other games. |

## Option records for the skill writer

The following are engine-neutral recommendations inferred from the examples.
They deliberately extend beyond a list of shipped menu labels.
Defaults follow the owner's rule: designer-intended play remains default, with conservative safety presentation.
A reference indicates inspiration, not a claim that the game implements every recommendation in the record.

### 1. Presets and discoverability
- Helps: vision, hearing, motor, cognitive, situational access.
- Signal/tier: every game; Baseline.
- Default: concise first-launch access entry; no gameplay assists silently activated.
- Implement: editable presets, plain-language effects, previews, persistence, reset of individual settings.
- Pitfall: presets overwrite unrelated preferences or require inaccessible setup.
- Reference: [The Last of Us Part II][tlou2].

### 2. Menu narration and screen-reader compatibility
- Helps: blind/low-vision players and players who benefit from spoken text.
- Signal/tier: all menus need accessible structure; full custom speech coverage can be Advanced.
- Default: respect a reliable platform preference, otherwise offer activation before setup.
- Implement: expose focus, role, value, state, and errors; interrupt stale speech appropriately.
- Pitfall: title menus work but inventory, tutorial, multiplayer lobby, or confirmation is silent.
- References: [Diablo IV][diablo]; [Street Fighter 6's documented gap][sf6].

### 3. Captions and sound visualization
- Helps: Deaf/HoH, muted play, noisy environments, cognitive processing.
- Signal/tier: meaningful speech/sound; Contextual.
- Default: readable speech subtitles enabled; configurable non-dialogue/directional information.
- Implement: semantic events with source identity/direction; prioritize urgent events without flooding.
- Pitfall: dialogue-only subtitles omit warnings, nonvisual threats, or objective information.
- References: [Fortnite][fortnite], [Jedi: Survivor][jedi].

### 4. Audio description
- Helps: blind/low-vision, attention, cognitive and situational access.
- Signal/tier: meaningful visual storytelling; Advanced.
- Default: optional, easy to activate before the first relevant scene.
- Implement: authored timed descriptions, language coverage, separate mix, replay access.
- Pitfall: description collides with speech; menus/gameplay remain inaccessible.
- References: [Part I][tlou1], [Part II Remastered][tlou2ad].

### 5. Sign-language cinematics
- Helps: signing Deaf/HoH players; comprehension in a preferred language.
- Signal/tier: substantial cinematic dialogue; Advanced.
- Default: optional, language and placement chosen by player.
- Implement: budget translation/performances; readable inset, adjustable background/position.
- Pitfall: treat ASL/BSL as interchangeable or claim all gameplay is interpreted.
- Reference: [Forza Horizon 5][forzah].

### 6. Speech-free communication and interaction
- Helps: speech, hearing, motor, anxiety/stress, noisy/shared-space play.
- Signal/tier: microphone requirements or team coordination; Contextual.
- Default: text/pings available alongside voice.
- Implement: contextual vocabulary, remap-aware inputs, visible acknowledgement and rate control.
- Pitfall: voice remains mandatory for a single puzzle, tutorial, or device.
- References: [Apex pings][apexping], [Phasmophobia text interactions][phasmo].

### 7. Remapping and one-handed layouts
- Helps: motor, limb difference, pain/fatigue, alternative hardware.
- Signal/tier: every input system; Baseline, with Contextual one-handed presets.
- Default: intended layout; alternatives selectable before play.
- Implement: remap all contexts, regenerate prompts, validate conflicts, allow sequential alternatives.
- Pitfall: gameplay remaps but inventory, vehicles, minigames, or escape actions do not.
- References: [Battlefield 2042][battlefield], [Pokémon Casual Controls][pokemon].

### 8. Hold, toggle, single press, and mash
- Helps: motor, pain/fatigue, variable reaction speed.
- Signal/tier: sustained or repeated inputs; Contextual.
- Default: intended interaction; configurable independently per action.
- Implement: separate sprint/aim/heal options; automatic release on pause, death, focus loss.
- Pitfall: replacing mash with an exhausting hold, or ambiguous press-length-dependent behaviour.
- Reference: [Alan Wake 2 developer retrospective][alanimpl].

### 9. Automatic QTE and low-value action completion
- Helps: motor, vision, cognitive, low-stress/situational play.
- Signal/tier: timed prompts or repetitive traversal/collection; Contextual.
- Default: manual, with visible selectable automatic alternatives.
- Implement: enter the normal success state; grant required items and preserve narrative timing.
- Pitfall: remove the visual prompt but leave an invisible failure timer or progression check.
- References: [Spider-Man Remastered][spiderman], [Jedi: Survivor][jedi].

### 10. Game speed and timing windows
- Helps: motor, visual processing, cognitive and stress-related access.
- Signal/tier: fast action, projectiles, short parry/puzzle windows; Contextual.
- Default: intended speed; separate manual shortcut and automatic combat options.
- Implement: use one authoritative time model; test camera/audio, input buffers, physics, and scripts.
- Pitfall: accessibility timers stay fast while enemies slow, or narration becomes unintelligible.
- References: [Rift Apart][ratchet], [Ragnarök puzzle timing][gow].

### 11. Aim and simplified combat
- Helps: motor, low vision, fatigue, reaction time.
- Signal/tier: precision aiming or complex move sequences; Contextual.
- Default: intended targeting; tune strength and target switching separately.
- Implement: distinguish snap/lock/tracking; preserve intended resource use and target choice.
- Pitfall: assistance available only in one mode, or a simplified layout removes needed actions.
- References: [Gears 5][gears], [Street Fighter 6][sf6].

### 12. Custom challenge domains
- Helps: motor, cognitive, vision, stress and situational constraints.
- Signal/tier: several independently demanding systems; Contextual.
- Default: designer preset; individual combat/puzzle/traversal values editable.
- Implement: plain descriptions of damage, health, timing, detection, and navigation changes.
- Pitfall: “easy” reduces enemy health but leaves the player's actual barrier unchanged.
- References: [Valhalla][valhalla], [Tomb Raider][tombraider].

### 13. Damage, invulnerability, immortality, and resources
- Helps: motor, stress, fatigue, practice and narrative access.
- Signal/tier: failure through damage/resource exhaustion; Contextual.
- Default: intended failure rules; optional individual assists.
- Implement: explicitly define health floor, damage reactions, fall resets, ammunition, stamina.
- Pitfall: scripts depend on death or an empty resource and softlock with assists active.
- References: [TUNIC][tunic], [Alan Wake 2 implementation][alanimpl].

### 14. Checkpoint continues and content skips
- Helps: fatigue, memory, motor, stress, limited session length.
- Signal/tier: long runs, repeated bosses, demanding isolated sequences; Contextual.
- Default: normal checkpoints; optional retries/skips available without humiliation.
- Implement: restore consistent inventory/quest state and all mandatory rewards.
- Pitfall: a skipped boss never awards its key, or a continue invalidates achievements.
- References: [Dead Cells][deadcells], [Chicory][chicory].

### 15. Independent failure types
- Helps: motor, motion sensitivity, stress and fear.
- Signal/tier: combat, falls, hunger, oxygen, and hazards coexist; Contextual.
- Default: intended rules; explicit independent switches.
- Implement: name exactly which failures remain; allow recovery from out-of-bounds states.
- Pitfall: “no fall damage” is presented as “cannot fall/die,” or Creative is called a phobia filter.
- References: [Psychonauts 2][psychonauts], [Subnautica][subnautica].

### 16. Readable typography and scale
- Helps: low vision, reading differences, cognitive, small/distant screens.
- Signal/tier: text/interface in any game; Baseline.
- Default: readable size/contrast; decorative effects should have an alternate treatment.
- Implement: scale and reflow real content, separate subtitle/UI choices, test localization.
- Pitfall: larger fonts clip names or icons stay too small; claim one font treats dyslexia.
- References: [Pentiment][pentiment], [Cyberpunk 2077][cyberpunk].

### 17. Colour alternatives and high contrast
- Helps: colour-vision deficiency, low vision, visual processing.
- Signal/tier: state encoded by colour; Baseline non-colour encoding, Contextual world contrast.
- Default: legible palette with shape/text redundancy; contrast overlay optional.
- Implement: semantic categories with adjustable foreground/background and identifiable icons.
- Pitfall: global tint damages readability or a colour change never reaches particles/minimap.
- References: [Destiny 2][destiny], [Jedi: Survivor][jedi].

### 18. Camera and visual comfort
- Helps: motion sickness, vestibular conditions, migraine/sensory sensitivity.
- Signal/tier: moving camera, shake, blur, sway, simulated lens effects; Contextual.
- Default: conservative camera/effect presentation and easy first-launch reduction.
- Implement: separate shake, sway, blur, FOV, sensitivity, reference-dot choices.
- Pitfall: gameplay reduction fails during cinematics, vehicles, damage, or scripted camera moves.
- References: [Ragnarök][gow], [Cyberpunk 2077][cyberpunk].

### 19. Visual-noise reduction and HUD control
- Helps: neurodivergent/sensory, visual processing, attention, low vision.
- Signal/tier: dense particles, low-health overlays, floating numbers, animated menus; Contextual.
- Default: clear essential information; conservative decorative motion.
- Implement: separate essential cues, decoration, reminders, telemetry; offer previews.
- Pitfall: minimal HUD hides access cues/captions or reduced particles conceal a hazard.
- References: [Halo Infinite][halo], [Tetris Effect PC][tetris], [Returnal caution][returnal].

### 20. Player-controlled guidance
- Helps: navigation/memory needs and players overwhelmed by extra visual cues.
- Signal/tier: guidance paint, outlines, route trails, quest markers, unsolicited hints; Contextual.
- Default: designer guidance; independently allow less, more, and on-demand help.
- Implement: explicit semantic categories; texture/material variants when removal requires them.
- Pitfall: claim an icon toggle removes baked paint or tie all guidance to combat difficulty.
- Reference: [Star Wars Outlaws][outlaws].

### 21. Phobia filters
- Helps: phobia, panic/stress, sensory avoidance.
- Signal/tier: named triggering creatures/materials/animation/sounds; Contextual.
- Default: offer before exposure with a non-triggering description, not an unsolicited preview.
- Implement: audit models, motion, audio, icons, books, corpses, particles, menus, trailers.
- Pitfall: a new update introduces unfiltered variants; “replacement” retains feared motion.
- References: [Grounded][grounded], [Hogwarts Legacy coverage gap][hogwarts].

### 22. Content warnings, concealment, and intensity
- Helps: trauma-related triggers, fear, stress, sensory sensitivity.
- Signal/tier: intense imagery, sudden brightness/volume, disturbing scenes; Contextual.
- Default: accessible advance notice; optional granular concealment/reduction.
- Implement: distinguish warnings, visual hiding, audio reduction, skipping, and nonlethal modes.
- Pitfall: blur hides gore but leaves distressing audio, or a warning is too late.
- References: [Dead Space][deadspace], [Life is Strange: True Colors][lis].

### 23. Recaps, objectives, and session re-entry
- Helps: memory, attention, cognitive access, intermittent play.
- Signal/tier: long narratives, many quests, long breaks; Contextual.
- Default: concise optional recap offered on return, never forced spoilers.
- Implement: current goal, last meaningful actions, named people, controls refresher, replayable history.
- Pitfall: loading completes before a recap can be read, or a journal reveals future information.
- References: [Dragon Quest XI S][dq11], [Horizon Forbidden West][horizon].

### 24. Dialogue history, lore, and text pacing
- Helps: memory, reading differences, language learning, hearing/attention.
- Signal/tier: dialogue choices and dense world information; Contextual.
- Default: player-controlled reading; optional auto-advance; easy log access.
- Implement: speaker-labelled history, contextual lore, spoiler-aware entries, resumable scenes.
- Pitfall: opening a log advances the conversation or discards the pending choice.
- References: [Baldur's Gate 3][bg3], [Final Fantasy XVI][ff16lore], [Persona 5 Royal][persona].

### 25. Spatial memory and hints
- Helps: memory, executive function, navigation, intermittent play.
- Signal/tier: revisiting locked routes or tracking many puzzle states; Contextual.
- Default: voluntary notes/hints, with progressively clearer help.
- Implement: player screenshot/description pinned to world location, editable labels and next step.
- Pitfall: screenshots are the only representation, excluding blind players.
- References: [The Lost Crown][prince], [Chicory][chicory].

### 26. Blind navigation and cue literacy
- Helps: blind/low vision, reduced visual attention.
- Signal/tier: spatial traversal or racing; Contextual, often Advanced for full independence.
- Default: optional spatial cues with a tutorial and controllable mix.
- Implement: separate objective, obstacle, boundary, hazard, and interaction cues; reachable recovery.
- Pitfall: cues work in a demo route but fail on menus, branches, retries, or changing maps.
- References: [Forza Motorsport][forzam], [The Last of Us Part II][tlou2].

### 27. Audio-first games
- Helps: blind players when nonvisual access is complete.
- Signal/tier: intentionally audio-led design; Contextual architecture decision.
- Default: coherent nonvisual navigation; offer other channels where feasible.
- Implement: make direction, distance, timing, menus, and error recovery perceivable without sight.
- Pitfall: market the result as universally accessible while Deaf players have no path.
- References: [The Vale][vale], [A Blind Legend][blindlegend].

### 28. Sports, flight, and task assistance
- Helps: motor, processing, fatigue, learning and situational access.
- Signal/tier: simultaneous steering/aiming/timing or complex cockpit tasks; Contextual.
- Default: intended controls; granular auto/assisted/manual alternatives.
- Implement: separate action selection, timing, direction, and camera assistance by task.
- Pitfall: assistance works for one aircraft/team/mode but is advertised across all content.
- References: [Forza Motorsport][forzam], [MSFS][msfs], [NBA 2K24][nba].

### 29. Practice and calibration
- Helps: motor, cognitive, timing, learning, alternative-device users.
- Signal/tier: complex or precise mechanics; Contextual.
- Default: easy-to-find practice with no progress cost.
- Implement: repeatable situations, cue previews, controllable speed and adjustable feedback.
- Pitfall: tutorials are time-limited, unreplayable, or introduce mechanics only through audio.
- References: [Rocket League Free Play][rockettraining], [DOOM Eternal][doom].

### 30. Free placement and low-stress modes
- Helps: stress, motor, cognition, creative/situational play.
- Signal/tier: correctness enforcement or resource management blocks the desired experience; Contextual.
- Default: intended rules; voluntary bypass of the particular pressure.
- Implement: preserve authored story availability and completion state; allow later reversion.
- Pitfall: “relaxed” removes all interesting systems when one was the problem.
- References: [Unpacking][unpackingdev], [SOMA][soma].

### 31. Achievement and content parity
- Helps: everyone who uses assistance; prevents coercive trade-offs.
- Signal/tier: achievements, endings, rewards, saves; Baseline.
- Default: assists never shame, lock content, lock achievements, or mark the save.
- Implement: unlock logic reads completed outcomes, not the presence of accessibility settings.
- Pitfall: copy a shipped game's equipment/mode/character restrictions as if mandatory.
- Reference: [Dead Cells explicitly preserves achievements][deadcellsach]; parity elsewhere requires its own evidence.

### 32. Flash reduction and photosensitivity controls
- Helps: photosensitivity, migraine/sensory sensitivity, distraction.
- Signal/tier: flashes, lightning, strobing, rapid full-screen changes; Contextual safety requirement.
- Default: conservative effects before setup; optional reductions reachable before exposure.
- Implement: inventory flashing sources, reduce intensity/frequency/area, preserve critical information through other cues, and validate the resulting scenes.
- Pitfall: treat an advance warning or one lightning toggle as proof the whole game is medically safe.
- References: [Stardew Valley's documented effects toggle][stardew], [Celeste's Photosensitive Mode maintenance][celeste], [True Colors warnings][lis].

### 33. Third-party accessibility extensions
- Helps: blind/low-vision, motor, alternative-device and assistive-technology users.
- Signal/tier: extensible PC/web platforms; Contextual, with Baseline input compatibility foundations.
- Default: retain built-in access; extensions are optional additions.
- Implement: stable semantic state/input interfaces where practical; document supported versions and installation/access requirements.
- Pitfall: claim a mod ships with the game or require inaccessible mod installation to unlock essential play.
- Reference: [Stardew Access][stardewmod] illustrates extension work rather than native support.

## Evidence quality and unresolved questions

- This was desk research, not a hands-on audit of all listed builds.
- Current official pages can be updated in place. Record platform/build/date again before implementing exact menu parity.
- [Sea of Thieves' hub][sea] was search-indexed but direct fetch returned HTTP 403. Its official [single-stick patch notes][seapatch] independently corroborate that example.
- [Celeste's official changelog][celeste] and developer interview were used rather than treating an inaccessible community wiki as fetched evidence.
- [Dead Cells' patch page][deadcells] was search-indexed; developer Steam announcements supplement the extracted patch record.
- [NYT Wordle access][wordlecontrast] needs direct assistive-technology testing; the NYT domain was robots-blocked during research.
- [Phasmophobia arachnophobia][phasmooptions]: **UNVERIFIED**; verified text interaction is a separate finding.
- Trypophobia filter in a shipped game: **UNVERIFIED**; no success example should be invented.
- [Resident Evil 4 paint removal][re4mod]: verified as a mod; official toggle **UNVERIFIED**.
- [FFVII Rebirth paint removal][ff7mod]: verified as a mod; official toggle **UNVERIFIED**.
- [Alan Wake 2 paint removal][alan]: **UNVERIFIED**; Gameplay Assist is verified.
- [Jedi: Survivor paint removal][jedi]: **UNVERIFIED**; navigation icon/ping controls are verified.
- [Tomb Raider paint removal][tombraider]: separate difficulty verified; exact painted-mark behaviour not established by the cited official article.
- [VALORANT flash reduction][valorant]: **UNVERIFIED** here; do not transform colour alternatives into that claim.
- [Overwatch reporting transcripts][owtranscription]: verified moderation feature; not player-facing live captions.
- [Beat Saber][beatstudy], [NBA 2K24][nba], and [It Takes Two][ittakes] need especially careful mode/content coverage testing.
- Lethal Company, Subnautica, Stardew Valley, Hollow Knight, and Night in the Woods use explicitly identified community/secondary evidence for some details.
- Achievement parity is explicitly sourced for [Dead Cells][deadcellsach]; it remains a mandatory design requirement for the new skill regardless of other games' policies.

## Source URLs

Reference definitions below resolve the links used in every table and record.
Sources include official pages, patch notes, developer interviews, specialist access reviews, a primary research paper, and labelled community documentation.

[tlou1]: https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/
[tlou2]: https://www.naughtydog.com/blog/the_last_of_us_part_ii_accessibility_features_detailed
[gow]: https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/
[gowhints]: https://blog.playstation.com/2024/09/18/god-of-war-ragnarok-brings-a-host-of-new-features-to-pc-available-tomorrow/
[sm2]: https://support.insomniac.games/hc/en-us/articles/46730041467027-What-Accessibility-options-does-Marvel-s-Spider-Man-2-feature
[sm2patch]: https://blog.playstation.com/2024/02/21/marvels-spider-man-2-update-adds-new-game-plus-and-new-suits-on-march-7
[ratchet]: https://sonyinteractive.com/en/news/blog/inside-the-accessibility-features-of-ratchet-clank-rift-apart/
[spiderman]: https://support.insomniac.games/hc/en-us/articles/46670280641299-What-Accessibility-options-does-Marvel-s-Spider-Man-Remastered-have
[horizon]: https://blog.playstation.com/2022/02/10/accessibility-features-in-horizon-forbidden-west/
[gears]: https://news.microsoft.com/en-my/2019/12/13/gears-5-sets-a-higher-bar-for-accessibility-with-features-that-mean-more-people-can-play-the-game/
[halo]: https://support.halowaypoint.com/hc/en-us/articles/4408122295444-Accessibility-Features-in-Halo-Infinite
[sea]: https://www.seaofthieves.com/accessibility
[seapatch]: https://www.seaofthieves.com/release-notes/2.0.18
[valhalla]: https://news.ubisoft.com/en-us/article/5Rys9UeXZh9bAjnHBZN4pC/assassins-creed-valhalla-bringing-accessibility-to-a-viking-epic
[mirage]: https://www.ubisoft.com/en-us/help/assassins-creed-mirage/gameplay/article/accessibility-features-in-assassins-creed-mirage/000105449
[miragespot]: https://news.ubisoft.com/fr-fr/article/4gDuMRKW25Z4eOLVSZBrvx/assassins-creed-mirage-coup-de-projecteur-sur-laccessibilit
[cyberpunk]: https://www.cyberpunk.net/en/news/49591/update-2-1-accessibility-features
[starfield]: https://www.gamesradar.com/starfield-accessibility-verdict/
[starfieldpatch]: https://bethesda.net/en-US/news/starfield-update-1-11-36-may-15-2024
[returnal]: https://blog.playstation.com/2021/05/11/unpacking-returnals-ux-design-gameplay-first-ui-retro-futuristic-tech-and-accessibility/
[returnalreview]: https://www.youtube.com/watch?v=f3qJXxIPe94
[alan]: https://www.alanwake.com/story/the-anniversary-update-explained/
[alanimpl]: https://www.alanwake.com/story/gameplay-assist-alan-wake-2/
[deadspace]: https://www.ea.com/seed/news/accessibility-and-player-choice-in-dead-space
[deadspacewarning]: https://www.ea.com/games/dead-space/dead-space/content-warning?isLocalized=true
[re4]: https://caniplaythat.com/2023/05/12/resident-evil-4-remake-accessibility-review/
[re4mod]: https://www.nexusmods.com/residentevil42023/mods/73
[jedi]: https://www.ea.com/able/resources/star-wars/star-wars-jedi-survivor
[outlaws]: https://news.ubisoft.com/en-gb/article/4J5XWevEccm1LBSduyij2H/star-wars-outlaws-accessibility-spotlight
[tombraider]: https://blog.playstation.com/2018/08/10/hands-on-with-shadow-of-the-tomb-raider/
[uncharted]: https://www.gamespot.com/articles/uncharted-4s-accessibility-options-inspired-by-inp/1100-6439983/
[doom]: https://www.playstation.com/en-us/games/doom-eternal/
[control]: https://news.xbox.com/en-us/2020/08/27/stay-in-the-light-in-awe-the-new-expansion-for-control-out-today/?ver=3.7.1
[psychonauts]: https://www.doublefine.com/games/support/psychonauts-2/accessibility?platform=all
[celeste]: https://www.celestegame.com/changelog.html
[celesteinterview]: https://filmstories.co.uk/features/the-new-normal-the-indie-developers-changing-our-conceptions-of-difficulty/
[hades]: https://www.supergiantgames.com/blog/hades-faq/
[hades2]: https://www.supergiantgames.com/blog/hades2-faq/
[hades2review]: https://www.techradar.com/gaming/hades-2-review
[diablo]: https://news.blizzard.com/en-gb/article/23954932/combatting-demons-with-accessibility-in-diablo-iv
[diablonav]: https://news.blizzard.com/en-us/article/24139219/a-crucible-for-allaccessibility-features-in-vessel-of-hatred
[sf6]: https://caniplaythat.com/2023/05/31/street-fighter-6-accessibility-hits-and-misses/
[sf6capcom]: https://www.capcom.co.jp/ir/english/data/pdf/annual/2023/annual_2023_teach.pdf
[mk1]: https://mortalkombatgamessupport.wbgames.com/hc/en-us/articles/19435787154963-Mortal-Kombat-1-Accessibility-Features-A11Y
[bg3]: https://forums.larian.com/ubbthreads.php?Number=873064&ubb=showflat
[bg3patch]: https://baldursgate3.game/news/patch-5-now-live_99
[ff16]: https://www.square-enix-games.com/en_US/news/final-fantasy-xvi-everything-you-need-to-know
[ff16lore]: https://na.finalfantasy.com/news/2277
[dq11]: https://nintendowire.com/reviews/dragon-quest-xi-s-echoes-of-an-elusive-age-definitive-edition/
[persona]: https://caniplaythat.com/2020/04/02/deaf-hoh-review-persona-5-royal/
[deadcells]: https://dead-cells.com/patchnotes/29
[deadcellsach]: https://store.steampowered.com/news/posts/?appids=588650&enddate=1663602205&feed=steam_community_announcements
[tunic]: https://news.xbox.com/en-us/podcast/gdc-and-sarah-bond/?ver=3.7.1
[elden]: https://caniplaythat.com/2022/02/28/elden-ring-accessibility-menu-deep-dive/
[eldenreview]: https://caniplaythat.com/2022/04/18/elden-ring-review/
[sekiro]: https://caniplaythat.com/2019/03/31/deaf-game-review-sekiro-shadows-die-twice/
[hollow]: https://www.nintendolife.com/games/switch-eshop/hollow_knight
[mario]: https://www.nintendo.com/us/store/products/super-mario-bros-wonder-switch/
[mariosupport]: https://en-americas-support.nintendo.com/app/answers/detail/a_id/63383/~/general-tips-for-playing-super-mario-bros.-wonder
[kirby]: https://www.nintendo.com/us/whatsnew/ask-the-developer-vol-4-kirby-and-the-forgotten-land-part-4/
[hifi]: https://support.krafton.com/hc/en-us/articles/52371575035801-Hi-Fi-RUSH-Accessibility-Settings
[prince]: https://developer.microsoft.com/en-us/games/articles/2025/03/prince-of-persia-the-lost-crown-accessibility/
[princevideo]: https://www.youtube.com/watch?v=v0pyVhDnG6o
[chicory]: https://caniplaythat.com/2021/06/10/chicory-a-colorful-tale-can-i-play-that-accessibility-review/
[aoe]: https://www.ageofempires.com/age-iv-accessibility/
[aoexbox]: https://www.ageofempires.com/age-iv-on-xbox-accessibility-options/
[tlou2ad]: https://feedback.naughtydog.com/hc/en-us/articles/22879676199188-Does-The-Last-of-Us-Part-II-Remastered-have-Audio-Descriptions
[forzam]: https://news.xbox.com/en-us/2023/04/27/forza-motorsport-accessibility-features-blind-driving/
[forzamreview]: https://caniplaythat.com/2023/09/20/forza-motorsport-accessibility-preview/
[forzah]: https://forza.net/news/forza-horizon-5-series-5-update
[forzahstories]: https://forza.net/news/forza-horizon-5-american-automotive
[msfs]: https://www.flightsimulator.com/accessibility/
[msfs24]: https://www.flightsimulator.com/accessibility-msfs-2024/
[rocket]: https://www.epicgames.com/help/c-37599050/c-7261959845275/a14443106?lang=en-US
[rockettraining]: https://www.epicgames.com/help/en-US/rocket-league-c5719357623323/le-gameplay-c7262179951387/how-do-i-change-bindings-for-free-play-ball-control-in-rocket-league-a5720124262939?lang=en-US
[fc]: https://www.ea.com/able/resources/ea-sports-fc/fc-24?isLocalized=true
[madden]: https://www.ea.com/able/resources/madden-nfl/madden-nfl-24?isLocalized=true
[nba]: https://nba2kw.com/nba-2k24-controller-settings-explained-recommendations
[nbaslid]: https://www.operationsports.com/mike-lowes-nba-2k24-sliders/
[tetris]: https://www.tetriseffect.game/2019/07/23/award-winning-tetris-effect-now-available-on-epic-games-store-with-optional-support-for-oculus-rift-and-htc-vive/
[tetrisreview]: https://caniplaythat.com/2020/12/02/tetris-effect-connected-accessibility-review/
[beatsaber]: https://store.steampowered.com/app/620980/Beat_Saber/
[beatstudy]: https://johannwentzel.ca/projects/motionblocks/files/MotionBlocks__CHI_2025.pdf
[beatwiki]: https://beatsaber.fandom.com/wiki/Campaign
[fortnite]: https://www.epicgames.com/help/c-34254770/c-33726977/a25071786?lang=en-US
[apex]: https://www.ea.com/able/resources/apex-legends/pc
[apexping]: https://media.contentapi.ea.com/content/dam/eacom/common/ea-impact-report-22.pdf
[overwatch]: https://news.blizzard.com/en-us/article/23912175/overwatch-2-season-3-starts-february-7-new-antarctic-peninsula-control-map-one-punch-man-collab-loverwatch-dating-sim-and-so-much-more
[owtranscription]: https://overwatch.blizzard.com/en-us/news/23857517/defense-matrix-activated-fortifying-gameplay-integrity-and-positivity-in-overwatch-2/
[valorant]: https://www.riotgames.com/en/accessibility
[valorantclarity]: https://www.riotgames.com/en/news/valorant-shaders-and-gameplay-clarity
[codmw3]: https://news.blizzard.com/en-gb/article/24043597/explore-the-accessibility-options-that-launched-with-mwiii
[codbo6]: https://news.blizzard.com/en-gb/article/24151767/call-of-duty-black-ops-6-accessibility-updates-and-new-features
[battlefield]: https://www.ea.com/able/resources/battlefield-2042/pc
[destiny]: https://www.bungie.net/7/en/News/Article/destiny-2-accessibility-set
[grounded]: https://grounded.obsidian.net/news/grounded/accessibility-features
[hogwarts]: https://portkeygamessupport.wbgames.com/hc/es-419/articles/16304407055251-Notas-del-parche-de-Hogwarts-Legacy-para-PlayStation-5-Xbox-Series-X-S-y-PC-4-5-2023
[satisfactory]: https://satisfactory.wiki.gg/wiki/Settings
[satisfactoryremove]: https://archive.satisfactory.video/transcriptions/yt-T2lOiHtygVM%2C595.2613333333334%2C657.7904666666667/
[phasmo]: https://kineticgames.co.uk/news/phasmophobia-tempest
[phasmopatch]: https://www.kineticgames.co.uk/news/phasmophobia-voice-recognition-update
[phasmooptions]: https://phasmophobia.fandom.com/wiki/Options
[lethal]: https://lethal-company.fandom.com/wiki/Version_45
[wow]: https://news.blizzard.com/en-us/article/24117139/user-interface-and-quest-updates-in-the-war-withintm
[wowcrabs]: https://news.blizzard.com/en-us/article/24061009/wowcast-developer-chat-go-into-the-war-within
[subnautica]: https://dev.subnautica.wiki/wiki/Game_Modes
[subnauticamobile]: https://playdigious.helpshift.com/hc/en/32-subnautica/faq/523-what-accessibility-options-are-available/
[stardew]: https://wiki.stardewvalley.net/Options
[stardewmod]: https://www.nexusmods.com/stardewvalley/mods/16205
[minecraft]: https://www.minecraft.net/en-us/accessibility
[animal]: https://caniplaythat.com/2020/07/08/animal-crossing-new-horizons-accessibility-review/
[animalvision]: https://caniplaythat.com/2020/03/26/blind-low-vision-review-animal-crossing-new-horizons/
[pokemon]: https://www.gamerguides.com/pokemon-sword-and-shield/guide/intro-and-gameplay/getting-started/controls
[pokemonreview]: https://www.nintendolife.com/features/how-accessible-are-zelda-pokemon-and-animal-crossing-for-blind-gamers
[shorthike]: https://steamcommunity.com/app/1055540/discussions/0/1639792569850442961/
[night]: https://www.pcgamingwiki.com/wiki/Night_in_the_Woods
[nightreview]: https://www.nintendolife.com/reviews/switch-eshop/night_in_the_woods
[vale]: https://store.steampowered.com/app/989790/The_Vale_Shadow_of_the_Crown/
[blindlegend]: https://apps.apple.com/us/app/a-blind-legend/id973483154
[unpackingdev]: https://steamcommunity.com/app/1135690/discussions/0/3874842132562443643/?l=italian
[unpacking]: https://apps.apple.com/us/iphone/story/id1743255567
[wordle]: https://en.wikipedia.org/wiki/Wordle
[wordlecontrast]: https://bugzilla.mozilla.org/show_bug.cgi?id=1996950
[wordleglobal]: https://wordle.global/en/accessibility
[lis]: https://news.xbox.com/en-us/2021/09/17/making-empathy-accessible-life-is-strange-true-colors/
[ittakes]: https://www.techradar.com/gaming/it-takes-two-review
[soma]: https://frictionalgames.com/2017-11-what-is-somas-safe-mode/
[outerwilds]: https://blog.playstation.com/2021/07/29/more-to-explore-outer-wilds-echoes-of-the-eye-expansion-lands-september-28/
[pentiment]: https://pentiment.obsidian.net/
[pentimentxbox]: https://news.xbox.com/en-us/2022/10/13/xbox-accessibility-showcase-2022/
[ff7patch]: https://www.square-enix-games.com/en_US/documents/notice-regarding-updates-final-fantasy-vii-rebirth
[ff7mod]: https://www.nexusmods.com/finalfantasy7rebirth/mods/92
[ff7debate]: https://www.pcgamer.com/games/rpg/ff7-rebirth-director-knows-a-whole-heap-of-people-hate-yellow-paint-on-ledges-but-reckons-there-is-definitely-a-need-for-that-kind-of-thing/
[phobiaguideline]: https://learn.microsoft.com/en-za/gaming/accessibility/xbox-accessibility-guidelines/123

## Implications for the skill

1. Route by mechanics first, genre second. An FPS with crafting needs inventory/text access; a JRPG with action minigames needs timing/input assistance.
2. Load a small baseline reference plus only the contextual records triggered by the actual game. Keep the large catalogue out of routine implementation context.
3. For each setting, record the barrier, effect, default, independent controls, persistence, known exceptions, and an executable verification scenario.
4. Keep assistance optional and reversible. Preserve the intended default except conservative safety presentation; never shame, mark saves, remove content, or disable achievements.
5. Prefer menu options over access tied to an equipment slot, character, difficulty, unlock, or partner. Shipped examples show these trade-offs; the owner's policy does not require copying them.
6. Separate presentation filtering, information assistance, motor automation, and failure rules. Do not make one switch unexpectedly change all four.
7. Require a first-launch-to-completion path, including setup, menus, tutorials, minigames, failures, rewards, and return after a break.
8. Add specific regression cases for assists that bypass death, timing, resources, damage, or puzzle correctness. Include scripted encounters and save/reload.
9. Audit sensory reductions and phobia filters across assets, animation, sound, UI, books/codices, corpses, cutscenes, and added content. Describe gaps honestly.
10. Treat mods as extensions, not built-in shipping evidence. Preserve compatible input/assistive-tool integration where practical.
11. Every borrowed game example needs exact title, edition/platform, feature/update date, supporting URL, and uncertainty label.
12. Treat achievement parity as a tested contract. [Dead Cells][deadcellsach] supplies a clear reference; a menu label in another game supplies no guarantee.

| Actual game context | Reference implementations to imitate | Essential verification |
|---|---|---|
| Narrative action / stealth | [Part II's combined access paths][tlou2]; [Part I audio description][tlou1]; [Spider-Man QTE/puzzle alternatives][spiderman] | Complete required story, inventory, traversal, and scripted sequences with relevant assists. |
| Fast FPS / team action | [Halo sensory controls][halo]; [Apex pings][apexping]; [Battlefield input contexts][battlefield]; [Destiny repeated-fire relief][destiny] | Sound-off information coverage, low-strain inputs, HUD legibility, clear mode scope. |
| Action roguelike / metroidvania | [Dead Cells granular assistance][deadcells]; [Hades resilience][hades]; [Lost Crown memory][prince] | Repeated failure, run recovery, map return, unlocks, achievement parity. |
| Precision / 3D platformer | [Celeste adjustable assistance][celeste]; [Rift Apart speed shortcut][ratchet]; [Psychonauts independent fall/combat help][psychonauts] | Timing, camera, falls, recovery, scripted traversal, alternative input layouts. |
| Fighting | [SF6 combat audio][sf6]; [MK1 menus/navigation/description][mk1] | Reach fights independently; distinguish hit types/distance; test tutorial and story inputs. |
| Turn-based / story-heavy RPG | [BG3 conversation history][bg3]; [FFXVI contextual lore][ff16lore]; [DQXI return recap][dq11] | Recover after a break, review choices, avoid spoiler leaks, retain readable UI. |
| RTS / strategy | [Age IV scalable UI, narration, Story Mode][aoe] | Setup, battlefield information, camera movement, command contexts; narration coverage must be explicit. |
| Racing / flight | [Forza Motorsport][forzam]; [MSFS task assistance][msfs]; [FH5 sign languages][forzah] | Complete a route, menus, recovery, cue learning, different vehicles and weather. |
| Sports | [FC task assists][fc]; [Madden narrated decisions][madden]; [NBA timing alternatives][nba] | Match/roster/play-call screens and actual action; distinguish offline/ranked availability. |
| Survival / crafting / factory | [Grounded filter][grounded]; [Satisfactory separate hostility][satisfactory]; [Subnautica pressure modes][subnautica] | Filter coverage, resource checks, hostile encounters, crafting text, shared-session behaviour. |
| Horror | [Dead Space warning/concealment][deadspace]; [SOMA behaviour changes][soma]; [Alan Wake scripted-failure lessons][alanimpl] | Warning before exposure, audio/visual coverage, nonlethal progression, reversible choices. |
| Rhythm / VR | [Hi-Fi input simplification][hifi]; [Beat Saber research cautions][beatstudy]; [Tetris particle tuning][tetris] | Calibrated timing, seated/one-handed content coverage, sensory reduction, practice. |
| Cozy / creative / puzzle | [Unpacking rule bypass][unpackingdev]; [A Short Hike art-treatment option][shorthike]; [Chicory hints/skips][chicory] | Remove only selected pressure; preserve story, identification, completion, and rewards. |
| Browser word game | [Wordle contrast][wordle]; [Wordle Global declared keyboard/reader support][wordleglobal] | Keyboard focus, non-colour result meaning, announced guesses/errors, forced colours. |
| Audio-first adventure | [The Vale][vale]; [A Blind Legend][blindlegend] | Nonvisual setup/navigation/combat/recovery, plus an explicit account of hearing-related exclusions. |

The skill should imitate a reference's useful behaviour, improve documented gaps, and verify the player's complete route through the actual game.

