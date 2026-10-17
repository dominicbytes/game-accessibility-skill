# 05 — Player and community voices, and the omissions developers must prevent

Research date: 17 October 2026.
Scope: genre-neutral, engine-neutral research for the accessibility agent skill described in `00_BRIEF.md`.
This is a research report, not a claim that any particular game is universally accessible.

## 1. How to read the evidence

The recurring request is agency: let players change the particular barrier that stops them playing, without requiring them to justify their disability or surrender the parts of the game they enjoy. AbleGamers' Accessible Player Experiences framework explicitly distinguishes access to controls/information from flexibility in challenge. [A01]

The brief's policy governs all recommendations here: assists are optional; preserve the designer's intended gameplay defaults except where safety requires safer behavior; do not lock content, achievements, trophies, endings, or saves because someone uses an accessibility option. This is the owner's requirement, not a claim that every cited organization or game follows it.

Evidence labels used below:

- **Player account:** the named person's experience; it does not establish prevalence, a diagnosis, or a universal preference.
- **Advocate guidance:** recommendations published by accessibility specialists or disabled reviewers.
- **Documented feature:** a developer, publisher, or identified reviewer describes the feature in a specified release context. This research did not run the game.
- **Skill recommendation:** this report's implementation or verification proposal, informed by the linked evidence; it is not a claim about the internals of a cited game.
- **UNVERIFIED:** a claimed example, current behavior, attribution, or effectiveness could not be established. Do not silently promote it into implementation guidance as fact.

### Limits that matter

1. These sources are a purposive sample, not a representative survey of disabled players.
2. Forum contributors self-select; upvotes do not measure need or importance.
3. Ian Hamilton explicitly warns that survey and social-thread data cannot establish a canonical priority ranking. His synthesis nevertheless identifies recurring concerns such as text, remapping, captions, motion, and adjustable challenge. [A02]
4. The ranked 30 omissions in section 12 are therefore an engineering prevention order, not a measured frequency table.
5. Reviews describe the version and platform reviewed. A 2023 launch review is not evidence that the same omissions remain in a 2026 build.
6. A feature announcement is evidence of the published design, not proof of independent completion by every player.
7. A single inaccessible shop, minigame, configuration dialog, or mandatory cutscene can invalidate otherwise extensive support. SightlessKombat's Valhalla review supplies a concrete example of narration and navigation gaps within a game with substantial accessibility work. [P06]
8. Recommendations about phobias and stress concern player control and informed choice. They are not medical advice or promises to treat a condition.

### Priority and default vocabulary

| Label | Meaning in this report |
| --- | --- |
| Baseline | Every game must assess and verify this principle; a speech-free game need not invent subtitles, but must establish that no speech is present. |
| Contextual | Required investigation when the matching content or mechanic exists: combat, timers, spiders, a movable 3D camera, and so on. |
| Advanced | Larger production effort, such as comprehensive cinematic audio description or bespoke blind traversal assistance. It must be scoped honestly, not automatically discarded. |
| Intended | Preserve the designer's specified default. If none is specified, make that an explicit design decision before implementation. |
| Offered | Available before the relevant exposure or first required input, and changeable later. |
| Safe | Avoid known hazardous content and sudden loud playback; a warning or an opt-out after exposure does not satisfy this. |

## 2. What players and advocates actually say

### 2.1 Neurodivergent players are not one preference group

Kylie Noble describes being autistic with ADHD and finding both predictable routine and novelty in games. Their account includes comfort in repeated activities as well as enjoyment of exploration and narrative. This challenges a simplistic assumption that neurodivergent accessibility means making every game quiet, slow, or uneventful. [P01]

Cherry Thompson, discussing their own overlapping motor and cognitive barriers, explains that extreme contrast can itself make reading difficult. They prefer some dark interfaces with softer contrast and bolder text. This is a web-accessibility interview, so its transfer to game settings is an inference: provide adjustable presentation instead of insisting that one high-contrast theme helps everyone. [P02]

Laura Kate Dale's autistic-player primer asks for more time and clearer context around choices, sensory controls, alternatives to voice communication, and the ability to pause. It also proposes optional explanations of nonverbal subtext and idioms. Some of these are proposals, not widely shipped features; Dale explicitly says autism is a spectrum and the advice does not apply to every autistic gamer. [P03]

| Attributed account | What it establishes | Skill consequence, inferred from the account |
| --- | --- | --- |
| r/ADHD poster electricghost92 describes side activities becoming chores; another participant, nadroj37, describes returning months later without remembering the plot. [R01] | A player can enjoy a game yet lose the thread or momentum. | Offer a focused objective view and a return summary; do not require external note-taking. |
| In the same thread, themadscientist420 describes enjoying exploration more without a map full of question marks. [R01] | Additional markers are not always helpful. | Make marker categories optional and provide on-demand help. |
| r/autism participant moolie-sheep identifies timed events and uncertainty about what to do as overwhelming; TheQuietType84 describes difficulty with a spinning 3D viewpoint. The opening post is deleted; these surviving comments are the evidence. [R02] | Stress, cognitive uncertainty, and camera discomfort can overlap. | Investigate timers, instructions, and camera separately. |
| r/disabledgamers contributor aBirdwithNoName identifies easy mode, avoiding nauseating graphics, and remappable controls as personally important. [R03] | An individual may require several categories of accommodation together. | Verify combinations of assists; do not force exclusive disability presets. |
| In an r/gaming yellow-paint thread, banditkeith describes their partner becoming stuck when navigation cues disappear; kiladre asks for a toggle and color/transparency control because they dislike the yellow presentation. [R04] | The same cue can be essential to one person and unwanted by another. | Preserve reliable guidance when enabled and allow independent removal/customization. |
| Core Keeper player toriplumrose asks for trypophobia options because grub walls/floors make areas difficult to look at. Other commenters' proposed detours do not answer the request for a visual adjustment. [R05] | Avoiding a route is different from making its presentation tolerable. | Review textures and scenery, not just enemy models. |
| A player reports custom health colors resetting after updates in Marvel Rivals, making their default health bar difficult to distinguish. [R06] | Settings persistence can be an access requirement. | Treat update migration as accessibility regression coverage. Current bug status is UNVERIFIED. |

### 2.2 Motor and sensory testimony is about effort, not just possibility

Antonio Martinez's Can I Play That? motor guide describes holds and repeated inputs as tiring or impossible for some players, and calls out hardcoded menu keys despite gameplay remapping. This is an account of access and fatigue, not a request to trivialize game systems. [P04]

In his PlayStation Access controller review, Martinez explains why even configurable hardware does not solve every game's control requirements for him: moving between buttons, operating two sticks, and changing profiles can remain barriers. His experience is explicitly personal. The lesson is to reduce required actions and simultaneous inputs as well as support hardware. [P05]

Steve Saylor's own 2023 Starfield review identifies several distinct barriers: limited subtitle customization, missing menu narration, unreliable guidance, and inconsistent menu navigation. Treat this as a launch-era review of specified builds, not a statement about the current game. A large-font option did not make the rest of the interaction accessible to him. [P07]

SightlessKombat's Valhalla review values the combat and its audio cues while documenting unread menus and optional areas requiring sighted help. Enjoying a game and finding it inaccessible are compatible experiences. Do not use enjoyment, completion with assistance, or exceptional player skill as evidence that support is complete. [P06]

Brandon Cole's contribution to Forza Motorsport is documented by Turn 10: blind driving assistance was developed with repeated feedback from Cole and other blind/low-vision players. The design provides information needed to drive, rather than simply assuming that automation alone is the desired experience. [G01]

### 2.3 Shame and dismissal are barriers too

A ResetEra thread about treatment of disabled users objects to dismissing access needs and using unusually successful disabled players as arguments against accommodations. It is a discussion of community behavior, not a verified census of the forum. [R07]

The brief's no-penalty requirement is stricter than some historical guidance: the older AbleGamers Includification document suggests disabling achievements as a response to concern about easier settings. That historical recommendation conflicts with this project's policy and must not be copied into the skill. Prefer the current APX barrier-focused framing and the owner's explicit requirement. [A01] [A03]

## 3. YouTube talks, channels, and what was accessible

Search covered the requested advocates, GDC, Game Maker's Toolkit, AbleGamers, and relevant written counterparts. Direct YouTube extraction often returned only page chrome or a fetch error. No reliable YouTube caption transcript was retrieved in this run. Titles and indexed descriptions below are verified to the extent indicated; no unseen spoken quote or timestamp is attributed to them.

An author-published written version is identified as such, not mislabeled a downloaded transcript.

| Creator / video or collection | URL | Material actually available | Use and limit |
| --- | --- | --- | --- |
| Mark Brown / Game Maker's Toolkit, Designing for Disability collection | [Video Game Accessibility playlist][V01] | Search results and platform documentation linking the series. | Four complementary domains; the series is educational analysis, not one disabled person's testimony. |
| GMTK, Making Games Better for the Deaf and Hard of Hearing | [YouTube][V02] | Verified title and indexed description. | Hearing episode identified; detailed recommendations here are supported independently by CIPT/XAG sources. |
| GMTK, Making Games Better for Gamers with Colourblindness & Low Vision | [YouTube][V03] | Verified title and indexed description. | Vision episode identified; no verbatim speech used. |
| GMTK, Making Games Better for Players with Motor Disabilities | [YouTube][V04] | Verified title and indexed description. | Motor episode identified; no transcript-derived claims. |
| GMTK, Improving Games for Those with Cognitive Disabilities | [YouTube][V05] | Indexed title; creator's Patreon listing also uses an older Making Games Better title. | Cognitive episode identified. Chapters and spoken detail UNVERIFIED. |
| Ian Hamilton, Accessibility on a Shoestring | [YouTube][V06]; [GDC Vault][V07] | GDC description and session abstract; 2021 session, video uploaded in 2022. | Supports the relevance of accessibility to small indie budgets; individual techniques checked against written guidance. |
| Ian Hamilton, Accessibility: Lessons Learned from Designing for Gamers with Disabilities | [GDC Vault][V08] | Speaker and session abstract. | Pitfalls and successful approaches are the stated subject; no unavailable full-talk content quoted. |
| Cherry Thompson, A Fraught Love Letter To The Games Industry From A Disabled Gamer | [YouTube][V09] | Indexed title and IGDA GASIG attribution. | Lived-experience talk identified; specific testimony in this report comes from Thompson's accessible interview transcript. |
| Steve Saylor, Starfield – Accessibility Review (Xbox Series X) | [YouTube][V10]; [Saylor's written review][P07] | Video metadata; detailed first-person written review. | Written review supports findings; video speech not separately transcribed. |
| Brandon Cole, Gaming Without Sight: We Wanna Play Too | [Cole's announcement][V11] | Cole's post identifies and embeds the talk. | Direct YouTube watch ID and full transcript UNVERIFIED; no fabricated URL. |
| SightlessKombat, blind gameplay demonstrations | [Killer Instinct demonstration][V12]; [review][P06] | IGDA's documented video link; detailed written Valhalla review. | Demonstration identified; specific blind-completion claims must be tested per game. |
| Laura Kate Dale, Autistic Player Accessibility in Video Games | [YouTube][V13]; [written version][P03] | Author-published script/article and embedded video ID. | Primary autistic-player perspective; speculative proposals clearly distinguished. |
| Laura Kate Dale, Generic Photosensitivity Warnings Protect Liability, Not Users | [YouTube][V14]; [written version][P08] | Full author-published article and embed. | Specific warnings and player interviews; no claim that any generic warning makes exposure safe. |
| Motion Twin, Dead Cells VLOG – Breaking Barriers update | [YouTube][V15] | Indexed description from the developer. | Describes tailored adjustments; no unavailable transcript claimed. |
| AbleGamers | [APX][A01]; [player impact account][P09] | Current written framework and a player-support account. | YouTube channel searched, but no specific talk transcript is relied on. |

### Social-media and forum provenance

- Ian Hamilton's prioritization article links two source Twitter threads, discusses selection bias, and summarizes recurring requests. The article was read; individual replies were not independently reconstructed. [A02]
- Saylor's launch-era X post is reproduced by GameSpot. It is an indirect social citation; use Saylor's own written review for the detailed critique. [P07] [P10]
- Dale's warning article attributes input to Tara Voelker, Harri, and Charlotte Callister and mentions a Bluesky post as its stimulus. Those interviews are available through Dale's article; the original Bluesky post was not independently verified. [P08]
- Mastodon, Bluesky, and X searches did not yield a comparably strong directly readable primary game-settings thread for every topic. Those gaps remain UNVERIFIED; this report does not invent posts, identities, or consensus.
- Reddit pages were fetched where possible. Deleted opening posts are not reconstructed from replies. Pseudonyms identify only public statements; no disability is inferred from membership in a subreddit.
- DAGERSystem's archive moved to Can I Play That?, according to its founder's announcement. Search the archived material there rather than assume the original domain remains a reliable live resource. [A04]

## 4. Neurodivergent and sensory options

The following are skill recommendations. The cited evidence establishes the barrier or published example; implementation and QA details are proposals for the skill, not claims that the example implements every detail.

### S01 — Guidance intensity, paint, trails, and markers

- **Does / helps:** independently show, reduce, recolor, or hide route paint, breadcrumb trails, interactable highlights, and waypoint markers; helps cognitive navigation, memory, low vision, and players who want fewer overlays.
- **Signal / tier / default:** navigation cues or dense exploration; Contextual; Intended cue level, with Offered controls before navigation is required. A reduced-clutter preset should not silently remove a separate navigation preference.
- **Implementation:** distinguish destination markers from climbability and interaction cues. Offer on-demand reveal as well as continuous guidance. Keep enabled cues semantically reliable.
- **Pitfall / verification:** do not equate ADHD with wanting paint off. Test reaching a destination with guidance On, Off, and On-demand; test routes after objectives change. Evidence for both preference directions: [R04]. Optional guided exploration example: Prince of Persia: The Lost Crown. [G02]

### S02 — Minimal HUD and per-element visibility

- **Does / helps:** lets players hide selected HUD components while retaining chosen essentials; helps sensory overload, attention, vision, and immersion preferences.
- **Signal / tier / default:** HUD or map overlays; Contextual; Intended. Offer named components, not only Full/None.
- **Implementation:** separate combat status, objectives, tutorial hints, damage numbers, notifications, and decorative elements. Add an accessible route back to the menu when the HUD is hidden.
- **Pitfall / verification:** HUD Off must not be the only way to remove clutter, nor suppress captions automatically. CIPT recommends selective map/UI information; Horizon Forbidden West documents HUD customization. [P11] [G03]

### S03 — Particle density and background detail

- **Does / helps:** reduces decorative particles, confetti, weather overlays, and background activity; helps attention and sensory comfort.
- **Signal / tier / default:** particles or noninteractive animation near action/UI; Contextual; Intended within safety constraints; offer Reduced/Off where feasible.
- **Implementation:** keep gameplay hitboxes and hazard information stable. Replace a removed attack effect with a simple readable cue rather than making it invisible.
- **Pitfall / verification:** a low graphics preset need not remove distracting effects, and must not be the only access route. Verify worst-case simultaneous effects, not only a quiet menu. The background-distraction barrier is explicitly covered by GAG. [A05]

### S04 — Notification density and persistence

- **Does / helps:** limits popups, reward banners, repeated tips, and animation; helps attention, cognitive load, and sensory needs.
- **Signal / tier / default:** achievements, loot feeds, quests, chat, live-service announcements; Contextual; Intended; offer Quiet and manual review.
- **Implementation:** classify critical alerts separately from marketing and celebrations. Provide a history for useful information; permit longer reading time without multiplying visual interruptions.
- **Pitfall / verification:** turning notifications off must not delete the only record of a new objective. Test several alerts arriving together and during dialogue. XAG distinguishes control over auto-updating content from forced presentation. [A06]

### S05 — Audio hierarchy and background-music reduction

- **Does / helps:** separate music, ambience, effects, dialogue, voice chat, narration, and accessibility cues where present; helps HoH, auditory processing, attention, and sensory comfort.
- **Signal / tier / default:** any audio; Baseline assessment; safe initial master level, Intended mix, independent mutes Offered before playback.
- **Implementation:** keep narration and critical navigation cues out of the music bus. Provide clear labels and optional speech ducking. Preserve individual choices when a preset changes.
- **Pitfall / verification:** Music 0 must silence menu music too; lowering explosions must not also remove an essential audio cue. Test every bus independently. [A07]

### S06 — Sudden sound, tinnitus, and sound-specific controls

- **Does / helps:** reduces sharp transients or selected distressing/repetitive sounds; helps sensory sensitivity, tinnitus-related preferences, and auditory comfort.
- **Signal / tier / default:** ringing, shrieks, explosions, heartbeat loops, repetitive vocalizations; Contextual; Intended with safe loudness; offer specific reductions rather than claim one universal sensory mode.
- **Implementation:** document whether a switch mutes, replaces, or attenuates sounds. Use a neutral alternative for information-bearing sounds. Offer a compressed dynamic-range mix as a separate choice.
- **Pitfall / verification:** no surprise full-volume audio preview. Horizon Forbidden West documents tinnitus removal and additional sound-category controls on PS5; platform scope matters. [G03]

### S07 — Sensory warnings with optional automatic pause

- **Does / helps:** warns before supported intense events and optionally pauses before them; helps sensory planning and stress management.
- **Signal / tier / default:** scripted intense scenes; Contextual; warnings Offered before the first event; never use them as a substitute for removing known hazardous flashing.
- **Implementation:** describe the event in words, offer reduce/skip/continue where supported, and preserve enough story context after a skip.
- **Pitfall / verification:** a warning after the flash is not a warning. Dale describes warnings and optional pause in Life is Strange: Double Exposure; her consulting involvement is disclosed. [P03] Safety requirement: [A08]

### S08 — Contrast, font, and reading comfort choices

- **Does / helps:** adjustable text size, weight, spacing where feasible, readable alternative fonts, and light/dark/mid contrast; helps low vision, dyslexia, cognitive processing, and sensory comfort.
- **Signal / tier / default:** text or UI; Baseline; readable Intended default with immediate previews. Do not substitute a tiny stylized default merely because a settings page exists.
- **Implementation:** preview real paragraphs and numbers; support layout reflow and the actual localization fonts. A dyslexia-associated font is an option, not a promised treatment.
- **Pitfall / verification:** maximum contrast and one branded font are not universal solutions. Thompson's account explains competing contrast needs; CIPT cautions against a sole dyslexia font. [P02] [P11] Shipped readable-font example: Pentiment. [G04]

### S09 — Nonverbal context and tone clarification

- **Does / helps:** optional tone labels, clearer dialogue-choice wording, or descriptions of visible subtext; may help players who find ambiguous social cues difficult to interpret.
- **Signal / tier / default:** narrative choices and meaningful nonverbal acting; Advanced; Off/optional, without requiring a diagnosis.
- **Implementation:** preserve intended ambiguity and avoid declaring a character's hidden truth. Explain delivery where known: e.g. sarcastic, uncertain, joking. Localize with narrative review.
- **Pitfall / verification:** do not automatically label all autistic users as needing it or add spoilers. Dale proposes broader nonverbal/idiom support; this is an exploratory recommendation, not a claim of established universal effectiveness. [P03]

### Sensory combination checks

These are proposed acceptance checks derived from the barriers above:

- Enable minimal HUD, captions, and navigation guidance together; each chosen information channel remains available.
- Turn decorative particles off during a hazard-heavy encounter; attack timing and danger boundaries remain legible.
- Mute music and ambience while keeping dialogue, screen reader, and navigation cues audible.
- Disable notification animations while retaining a readable, replayable notification history.
- Compare normal and reduced effects in representative bright, dark, busy, and transitional scenes.
- Restore one category without resetting unrelated categories or the whole profile.

## 5. Cognitive and memory support

### C01 — Return-to-game summary / “Where was I?”

- **Does / helps:** summarizes the current situation, last meaningful action, active objective, and available next step; helps memory, attention, fatigue, and returning players.
- **Signal / tier / default:** persistent campaigns or tasks across sessions; Contextual; an on-demand summary available, with optional reminder on load.
- **Implementation:** compose from recorded game state, not speculative generated narrative. Include a route to relevant controls and the journal. Limit to facts already revealed in that save.
- **Pitfall / verification:** do not summarize an entire franchise when the player needs one immediate objective. GAG recommends progress summaries; r/ADHD testimony describes the restart-or-forget dilemma. [A09] [R01]

### C02 — Quest journal, checklist, and objective reminders

- **Does / helps:** records explicit objectives, completed steps, and next known actions; helps memory, executive function, and interrupted play.
- **Signal / tier / default:** objectives that persist beyond a single screen; Contextual; recording On, intrusive reminders optional.
- **Implementation:** make goals concrete and distinguish main/optional/expired tasks. Provide sorting and focus on one objective. Reference current remapped controls.
- **Pitfall / verification:** a poetic conversation excerpt is not necessarily an actionable task. Test loading an old save and locating the next task without external help. CIPT's cognitive guide makes the instruction/journal distinction. [P11]

### C03 — Waypoint, map, and route assistance

- **Does / helps:** provides optional destination, direction, route, or map guidance; helps spatial memory and navigation, including blind navigation when appropriately implemented.
- **Signal / tier / default:** navigable spaces or backtracking; Contextual; Intended, Offered independently from difficulty.
- **Implementation:** disclose whether the cue points toward an objective, the next reachable passage, or a collectible. Recalculate after traversal and objective changes; support accessible map labels.
- **Pitfall / verification:** an arrow through an impassable wall is not a route. Saylor reports disappearing/no-route guidance in launch-era Starfield; use it as a test case for reliability. [P07]

### C04 — Player map notes and visual memories

- **Does / helps:** lets the player pin a screenshot or note to a location; helps remembering blocked routes and deferred tasks.
- **Signal / tier / default:** exploration with revisiting or ability gates; Contextual; tool available, use optional.
- **Implementation:** attach notes to stable locations and offer text labels or narration-compatible descriptions. Permit removal and editing without consuming scarce gameplay resources solely for access.
- **Pitfall / verification:** a screenshot-only memory aid may exclude blind players. Prince of Persia: The Lost Crown documents map-pinned screenshots and optional guided mode; the accessible-text extension is this report's recommendation. [G02]

### C05 — Character, terminology, and relationship glossary

- **Does / helps:** supplies context-sensitive, spoiler-limited definitions for people, places, factions, terms, and relationships; helps memory and reading comprehension.
- **Signal / tier / default:** dense lore, unusual terminology, or large casts; Contextual; accessible on demand.
- **Implementation:** version entries by story knowledge; explain a newly mentioned person without revealing later events. Make glossary navigation usable with keyboard, controller, and narration where supported.
- **Pitfall / verification:** a large unsearchable encyclopedia can increase the burden. Final Fantasy XVI's published Active Time Lore design offers tailored prompts while paused, including during cutscenes. [G05]

### C06 — Tutorial, instruction, and dialogue replay

- **Does / helps:** permits revisiting controls, instructions, briefings, tutorial exercises, and past dialogue; helps attention, memory, reading speed, and interrupted play.
- **Signal / tier / default:** any taught mechanic; Baseline record of instructions; full cinematic/exercise replay Contextual or Advanced; on-demand.
- **Implementation:** store actual instructions and relevant conversation history. Let practice replay run without overwriting campaign progress.
- **Pitfall / verification:** replaying a tutorial should use the current bindings. GAG explicitly links replay to memory and missed information, including ADHD-related attention gaps. [A10]

### C07 — Hints with graduated spoilers

- **Does / helps:** offers a nudge, more explicit guidance, and optionally a solution or skip; helps cognitive problem solving and players blocked on an isolated task.
- **Signal / tier / default:** puzzles, investigation, navigation uncertainty; Contextual; on-demand, automatic hints separately configurable.
- **Implementation:** clearly identify the reveal level before showing it; track which hints have already been requested. Separate reminder of known facts from revealing new solution information.
- **Pitfall / verification:** do not blurt out the answer while a player is thinking. Control's published Game Help approach starts with hints and becomes more explicit through later steps. That example is PS5-specific and originally tied to PlayStation Plus. [G06]

### C08 — Text progression, choice timers, and reading time

- **Does / helps:** manual text advancement, longer or disabled choice timers, text display all at once, and a review log; helps reading, processing, motor, attention, and anxiety-related needs.
- **Signal / tier / default:** text that disappears, timed dialogue or UI; Contextual; Intended gameplay timer, with untimed or extended alternatives Offered beforehand; no unnecessary UI timeout.
- **Implementation:** separate hiding the countdown from extending the deadline. Pause deadlines while reading settings or supported accessibility text.
- **Pitfall / verification:** hiding a timer does not remove failure pressure. XAG 116's formal scope excludes core gameplay timers; the broader gameplay recommendation here follows CIPT and the brief. Life is Strange: Double Exposure documents longer and removed choice timers. [A11] [P11] [G07]

### C09 — Autosave frequency and recovery history

- **Does / helps:** saves progress without requiring the player to remember; reduces replay after fatigue, missed cues, or sudden interruption.
- **Signal / tier / default:** persistent progress; Contextual; On, with sensible scene/event saves and a tested interval rather than one universal number.
- **Implementation:** retain multiple recoverable saves, protect an older valid state, and indicate when a save has completed. Make frequency adjustable if saving itself causes interruptions.
- **Pitfall / verification:** one autosave overwritten during a softlock is not recovery. Test failure immediately before and after a checkpoint. GAG recommends both manual and automatic saves. [A12]

### C10 — Save anywhere or equivalent suspend-and-resume

- **Does / helps:** allows stopping promptly without losing meaningful progress; helps fatigue, pain, narcolepsy-related interruption, and situational needs.
- **Signal / tier / default:** sessions longer than a trivial encounter; Contextual; available, without requiring a difficulty downgrade.
- **Implementation:** where exact-state save is impractical, provide a reliable suspend/resume contract and clearly explain what is retained. Keep manual recovery distinct from an expendable autosave slot.
- **Pitfall / verification:** a rest point several minutes away is not equivalent for someone who must stop now. GAG includes player accounts about fibromyalgia and narcolepsy and the barrier created by checkpoint-only saving. [A13]

### C11 — Pause anywhere, including cutscenes

- **Does / helps:** gives time to process, rest, use assistive equipment, or manage overload; helps motor, cognitive, sensory, stress, and situational needs.
- **Signal / tier / default:** real-time activity, cinematics, dialogue; Baseline for single-player/local play; a clear direct pause action, with optional focus-loss pause.
- **Implementation:** freeze gameplay clocks, damaging simulation, and relevant media together. Separate Pause from Skip. Keep a paused image/audio from continuing an intense exposure when a neutral pause overlay is possible.
- **Pitfall / verification:** menu open while damage continues is not pause. For live multiplayer, state the limit honestly and design a safe step-away mechanism; do not promise global pause without synchronization. XAG 108 recommends pausing single-player/local experiences; FFXVI supplies a cutscene-pause example. [A14] [G05]

### C12 — Practice without failure and small-step refreshers

- **Does / helps:** offers focused rehearsal and a compact reminder of an infrequently used action; helps memory, motor learning, and stress.
- **Signal / tier / default:** complex controls or special mechanics introduced far apart; Contextual; optional.
- **Implementation:** practice the exact mechanic with adjustable pace and current controls. Resume the original state afterward; do not make a full campaign restart the refresher mechanism.
- **Pitfall / verification:** training that assumes the same inaccessible input does not solve the barrier. AbleGamers APX includes Training Ground and Total Recall patterns. [A01]

### Cognitive verification scenarios proposed for the skill

1. Return to a mid-campaign save after an imagined month away: identify who/where/why and the next action using only in-game tools.
2. Miss one conversation and one notification: recover the objective and any required code or instruction.
3. Remap an action after its tutorial: every replayed instruction and prompt reflects the new binding.
4. Ask for the first hint: the final solution is not shown before it is requested.
5. Pause during a cutscene and during a timer: each resumes in a coherent state, without hidden progress or unavoidable failure.
6. Stop during a long encounter: the documented save/suspend promise actually holds.
7. Load an older save: codex entries and recap contain no facts from a later save or another playthrough.

## 6. Mental health, stress, and separate fail states

### What shipped examples establish

Thomas Grip's explanation of SOMA's Safe Mode says creatures were given redesigned behaviors rather than merely having attacks disabled. It also explicitly says the game remains horror: atmosphere and themes can still be frightening. A no-death mode must not be advertised as removing all disturbing content. The post describes choosing the mode when starting a new game; that is a limitation relative to this skill's mid-game-change recommendation. [G08]

Double Fine lists No Fall Damage, Invincibility against most attacks, and Narrative Combat as separate Psychonauts 2 features. This is evidence for separating failure sources. It is not proof that its Invincibility setting blocks every possible failure. [G09]

Control's developer documents independent combat modifications, immortality, and one-hit kills, and explicitly says assists do not affect trophies or achievements. This is a useful no-penalty example; its original advice to try without assists first is not wording this skill should automatically adopt. [G06] [G10]

### T01 — Low-stress preset

- **Does / helps:** bundles player-selected reductions in damage, repetition, timers, or threatening pursuit; helps players seeking lower-pressure play, including some with stress, fatigue, cognitive, or motor needs.
- **Signal / tier / default:** failure-heavy or tense gameplay; Contextual; Off/Intended, offered alongside other presets without stigma.
- **Implementation:** show all changes before applying; preserve per-option edits. Let the player keep challenging puzzles or platforming while reducing combat pressure.
- **Pitfall / verification:** do not call a bundle universally anxiety-safe. Check its complete path through the game, including bosses and unexpected chases. Granular difficulty is supported by XAG 108; APX distinguishes types of challenge. [A14] [A01]

### T02 — Combat damage off / invincibility

- **Does / helps:** removes incoming combat damage or specified lethal effects; helps motor, fatigue, and low-stress play.
- **Signal / tier / default:** combat damage; Contextual; Off/Intended.
- **Implementation:** name exactly what is blocked: attacks, status effects, environmental hazards, self-damage, drowning, or all of these. Prevent repeated knockdown or death-animation loops from becoming new barriers.
- **Pitfall / verification:** health that cannot fall below one can still leave the player stun-locked or unable to advance. Verify every damage type and defeat trigger separately. Example of deliberately bounded wording: Psychonauts 2's “most attacks.” [G09]

### T03 — Independent fall, drowning, and environmental death

- **Does / helps:** separately controls falling death, oxygen depletion, hazard damage, and combat damage; helps players tailor the challenge they want to retain.
- **Signal / tier / default:** more than one failure source; Contextual; each Intended, independent toggles Offered.
- **Implementation:** model assist effects at the failure-source level, not with one catch-all difficulty flag. For example, combat damage Off plus falling death On must be a supported combination where those mechanics exist.
- **Pitfall / verification:** turning off health loss should not accidentally make a required death-triggered transition impossible. Test each pair of settings. Psychonauts 2 documents separate fall/combat assists; the inverse combination and full matrix are skill recommendations, not verified claims about that game. [G09]

### T04 — Bottomless resources and reduced attrition

- **Does / helps:** optional unlimited ammunition, stamina, oxygen, consumables, or reduced repair/resource loss; helps fatigue, cognitive bookkeeping, motor, and stress needs.
- **Signal / tier / default:** management of depleting resources; Contextual; Off/Intended, separated by resource.
- **Implementation:** preserve pickup, crafting, and quest transitions that normally depend on resource changes. Explain whether spending is prevented or replenished.
- **Pitfall / verification:** infinite resources must not prevent a required empty-meter event or make the UI appear broken. Celeste documents stamina assistance; Control documents energy-regeneration adjustments. Exact coverage outside these examples is a recommendation to scope per game. [G11] [G10]

### T05 — Chase-sequence assist or skip

- **Does / helps:** widens catch-up windows, reduces pursuer pressure, or bypasses an isolated chase; helps motor timing, navigation processing, fatigue, and stress.
- **Signal / tier / default:** mandatory pursuit/escape; Contextual; Off/Intended.
- **Implementation:** distinguish easier chase mechanics from skipping the encounter. Restore a stable post-chase state and retain narrative information when skipping.
- **Pitfall / verification:** lower damage does not help if a scripted pursuer instantly fails the sequence. Marvel's Spider-Man 2 documents Chase Assist; a complete chase skip is not claimed for that example. [G12]

### T06 — Jump-scare reduction and horror-intensity controls

- **Does / helps:** reduces supported sudden audiovisual shocks, warns before them, or provides scene skips; helps players wishing to regulate fear or stress.
- **Signal / tier / default:** horror, surprise stingers, abrupt imagery, fake crashes, sudden pursuers; Contextual; Intended, except hazardous effects and unsafe loudness must be addressed by default.
- **Implementation:** inventory triggers separately from enemy lethality. A reduced setting may need changed audio, timing, camera, and scripted scenes, not just less damage.
- **Pitfall / verification:** do not label the game scare-free if the atmosphere remains. SOMA's developer makes that distinction; the proposed independent intensity controls extend the principle. [G08]

### T07 — Blood, gore, and disturbing-scene controls

- **Does / helps:** hides/replaces selected graphic visuals or permits skipping supported scenes; helps content preferences, phobias, sensory comfort, and stress.
- **Signal / tier / default:** graphic content; Contextual; Intended with accessible advance notice and controls before exposure.
- **Implementation:** specify whether this affects combat decals, dismemberment, cutscenes, death screens, item art, or only selected scenes. Preserve essential information through an alternative channel.
- **Pitfall / verification:** hiding a visual while leaving its audio may be insufficient. EA explicitly describes that limitation in Dead Space's scene-concealment feature. Do not describe it as a global gore removal mode. [G13]

### T08 — Retry, loss, and recovery rules

- **Does / helps:** optional shorter retries, preserved resources, more checkpoints, or a bypass after repeated failure; helps fatigue, motor, memory, and stress.
- **Signal / tier / default:** repeated attempts with substantial replay cost; Contextual; Intended, with optional recovery changes.
- **Implementation:** allow enabling before the first failure; a gentle offer after retries can supplement discovery. Let players dismiss suggestions permanently or for the session.
- **Pitfall / verification:** “fail three times to earn access” can itself block someone. Dead Cells' developer announcement describes continuation/multiple-life assistance and achievement availability. [G14]

### Stress-support acceptance checks

- Combat damage Off + falling death On behaves exactly as described.
- Timers Off does not leave a hidden timeout in scripted events.
- A skip grants the same progression state and access to the following story as ordinary completion.
- No-death mode does not require the player to witness an unchanged death animation repeatedly.
- A player can change an assist while blocked, without restarting the campaign.
- A suggested assist uses neutral language and can be ignored without repeated prompts.
- No ending, achievement call, grade, save card, or content gate checks whether assistance was used.

## 7. Phobias, content warnings, and replacement techniques

### What players ask for, and what a switch must actually change

Grounded's published accessibility page describes increasingly abstract spider appearance while keeping gameplay and difficulty unchanged. Its list includes an unchanged state plus five reductions: do not confuse five reduction steps with only five total selectable states. Xbox's research write-up also considers textures, eyes, and audio as contributors to reactions. [G15] [G16]

The official Satisfactory wiki describes replacing Stingers with cat sprites. That is a visual substitution, not proof that the underlying movement or encounter stops frightening everyone. [G17]

Guerrilla's Patch 1.21 announcement introduces thalassophobia support and specifies that the patch was for PS5. Descriptions of the mode report improved underwater visibility and unlimited breath; players in the thalassophobia community also say those changes do not resolve their particular fear of deep water. Respect that disagreement instead of promising a cure. [G18] [R08]

In r/Games' phobia discussion, players describe discomfort with injection animations in BioShock. These are player accounts about the game, not evidence that a needle-hiding feature exists. [R09]

### Phobia/content matrix

All rows below are **Contextual**: route them only if an inventory of the game's assets, sounds, environments, scripts, or marketing detects the relevant content. Default is **Intended presentation with optional modification offered before exposure**, except separately unsafe audiovisual effects must use safe defaults. Beneficiaries are players avoiding that content, including some with phobias, sensory needs, or stress-related needs.

| Content signal / option | Proposed engine-neutral implementation | Failure to avoid | Evidence and shipped-example status |
| --- | --- | --- | --- |
| Spiders / Arachnophobia reduction | Replace or abstract silhouette, limbs, eyes, fangs, material, sounds, and selected animations; make levels explicit. | Removing legs but retaining a close-up spider face or characteristic sound; forgetting corpses and inventory icons. | Grounded's visual abstraction documented. [G15] [G16] |
| Spider-like creatures / Alternate model or sprite | Replace rendered presentation consistently while retaining comprehensible hazards and interactions. | A replacement that is itself frightening, unreadable, or still moves in the same disturbing way. | Satisfactory's cat-sprite substitution documented by its official wiki. [G17] |
| Clustered holes / Trypophobia texture reduction | Alternate normal maps, textures, decals, organic surfaces, and UI close-ups; optionally use plain surfaces. | Checking only monsters while walls and floors retain the pattern. | Core Keeper player request is directly documented; a shipped comprehensive option for that game is UNVERIFIED. [R05] |
| Needles / Injection presentation | Hide or replace injection close-ups; offer alternate healing animation and audio; retain resource/state feedback. | Leaving first-person arm/eye imagery in a mandatory opening or consumable animation. | BioShock discomfort is a player report; a shipped needle mode there is UNVERIFIED. [R09] |
| Blood / Blood reduction | Separate blood color/intensity, splatter, wounds, decals, and UI pulses where relevant. | Assuming recoloring alone addresses blood imagery or fainting-related concerns. | General content-reduction guidance supports investigating this; universal effectiveness is UNVERIFIED. [A15] |
| Gore / Graphic scene concealment | Substitute affected geometry or use clearly documented concealment/skip for supported scenes. | Claiming global coverage when a setting handles only selected scripted scenes. | Dead Space documents selected scene warnings/concealment, including retained audio. [G13] |
| Deep water / Underwater visibility and breath | Offer clear water, visible boundaries, oxygen relief, route assistance, or an alternate route/skip when scoped. | Treating fear of drowning and fear of vast underwater spaces as identical. | Horizon Forbidden West patch and community response; a complete water-removal option is UNVERIFIED. [G18] [R08] |
| Heights / Height exposure reduction | Consider less vertiginous camera framing, opaque platforms, reduced edge effects, or bypass of isolated height challenges. | Calling a camera change sufficient for all players; retaining a compulsory falling cinematic. | Route from the game's content inventory and XAG mental-health/content guidance; specific shipped height mode not verified here. [A15] |
| Snakes / Alternate creature presentation | Replace mesh, animation, sounds, portraits, and cinematic appearances; allow a non-snake substitute that preserves cues. | Replacing a model while retaining hissing or snake-like locomotion without explanation. | Contextual proposal; named shipped comprehensive snake mode UNVERIFIED. [A15] |
| Insects / Insect or swarm reduction | Replace close-ups and swarm presentation; preserve direction, threat, count, and timing information. | Removing enemy readability or retaining swarming audio as the only cue. | Grounded illustrates content-specific analysis, not a verified all-insect mode. [G16] |
| Clowns / Alternate character appearance | Offer alternate appearance, scene warning, or supported bypass; include masks, portraits, and posters in the inventory. | Assuming costume replacement covers voice, laugh, or narrative content. | Contextual proposal; named shipped comprehensive clown mode UNVERIFIED. [A15] |

### Content-warning requirements proposed for the skill

1. Use an accessible text category list before purchase where practical and before first exposure in-game.
2. Let the player choose coarse warnings or more specific detail; do not assume everyone prioritizes spoiler avoidance over safety or comfort.
3. Offer scene-level warnings early enough to pause, change a setting, or leave before the content starts.
4. Describe the available action truthfully: reduce, replace, conceal, skip, or leave unchanged.
5. Do not auto-play a preview containing the very content someone is trying to avoid.
6. For replacement previews, start at the most abstract alternative and require an explicit request before showing the original.
7. Include audio, subtitles, narration, inventory, cutscenes, reflections, shadows, loading screens, and marketing in the content inventory.
8. Document coverage boundaries; “reduces spider appearance” is more honest than “safe for arachnophobia.”
9. Preserve progression, rewards, and the next stable save state after a supported skip.
10. Test with people who have the relevant access need; do not validate solely with an unaffected developer looking at an alternate asset.

Dale's warning article argues for specific, useful descriptions instead of boilerplate, and includes players requesting timing/location information and toggles. XAG 123 similarly covers advance information and options to avoid, remove, or bypass sensitive content. The preview and coverage inventory above are this report's practical extensions. [P08] [A15]

## 8. Motor access: effort, timing, devices, and mappings

### M01 — Hold, toggle, and tap alternatives

- **Does / helps:** supplies alternatives for aim, sprint, crouch, lock-on, menus, interaction, and other sustained actions; helps reduced strength, fatigue, dexterity, one-handed play, and situational use.
- **Signal / tier / default:** a sustained input exists; Contextual; Intended with alternatives Offered before the action.
- **Implementation:** choose per action. Define cancel behavior and what happens during pause, death, focus loss, and device disconnect.
- **Pitfall / verification:** replacing mash with a long hold excludes players who cannot hold. Test tap-to-start/tap-to-stop as well as hold. Martinez's guide covers the barrier; The Last of Us Part I documents per-action alternatives. [P04] [G19]

### M02 — Mash avoidance and repeat actions

- **Does / helps:** reduces repeated presses through hold, toggle-repeat, single activation, or automation; helps fatigue, pain, and limited motor control.
- **Signal / tier / default:** repeated attack, crafting, harvesting, or escape input; Contextual; Intended with alternatives.
- **Implementation:** retain action cadence and cancellation; provide a clear indicator when repeat is active. Include noncombat repeated interactions.
- **Pitfall / verification:** menus and resource collection can be more repetitive than combat. Dead Cells documents hold-to-repeat support; GAG recommends avoiding repeated input barriers. [G14] [A16]

### M03 — QTE auto-complete and timing assistance

- **Does / helps:** removes required QTE presses or offers fewer inputs, slower prompts, longer windows, and no rapid alternation; helps motor, cognitive, vision, and stress needs.
- **Signal / tier / default:** mandatory timed prompts; Contextual; Intended, independently selectable assist.
- **Implementation:** cover every QTE type, including boss finales, escape grabs, dialogue, minigames, and cinematics. Keep story branching behavior explicit.
- **Pitfall / verification:** one unhandled “cinematic” QTE can block the ending. Spider-Man 2 documents QTE autocomplete; GAG discusses alternatives to precise timing. [G12] [A17]

### M04 — Full remapping across keyboard, mouse, and controller

- **Does / helps:** lets players bind game and menu actions to reachable controls; helps motor, one-handed, alternative hardware, and established input preferences.
- **Signal / tier / default:** all interactive games; Baseline; Intended bindings plus remapping.
- **Implementation:** include mouse buttons, secondary bindings where useful, stick-function swap, vehicles, minigames, UI, pause, and confirmation. Support resolving conflicts without trapping the player.
- **Pitfall / verification:** system remapping alone does not update game prompts or necessarily provide action-level separation. Test inaccessible default keys from launch to exit. XAG 107 and CIPT provide detailed support. [A18] [P04]

### M05 — Remap-aware prompts and input vocabulary

- **Does / helps:** updates tutorials, HUD, controller diagrams, help text, and button glyphs after remapping; helps memory and motor access.
- **Signal / tier / default:** remapping or multiple devices; Baseline when supported; always accurate.
- **Implementation:** format prompts from action bindings rather than hardcoded button names. Offer a fixed preferred glyph set if automatic device switching flickers.
- **Pitfall / verification:** test nested menus and rare late-game tutorials, not just the opening tutorial. XAG input guidance specifically requires updated labels throughout hints and tutorials. [A18]

### M06 — One-handed and reduced-input schemes

- **Does / helps:** lowers simultaneous control demand through camera assistance, auto-run, toggled modes, shortcuts, and simpler interaction schemes; helps one-handed play, switch users, and fatigue.
- **Signal / tier / default:** two-stick or many-button simultaneous input; Contextual; optional scheme, with left/right variants where meaningful.
- **Implementation:** check required input count over the whole game, not only movement. Avoid forcing a second inaccessible control to activate the one-handed scheme.
- **Pitfall / verification:** “one-handed” is not a universal hardware configuration. TLOU Part I documents left/right-only schemes; Martinez explains real constraints with adaptive hardware. [G19] [P05]

### M07 — Aim assist strength, snap, and target selection

- **Does / helps:** widens targeting tolerance, slows reticle motion near targets, snaps or locks to targets, and optionally selects the next target; helps motor precision and some low-vision needs.
- **Signal / tier / default:** aiming; Contextual; Intended with adjustable strength and Off.
- **Implementation:** separate aim friction, lock duration, target switching, and snap where mechanics justify it. Apply to mandatory targets, not only ordinary enemies.
- **Pitfall / verification:** help against enemies does not solve an inaccessible tiny puzzle target. Dead Space documents friction/magnetism controls; Control documents Aim Snap. [G13] [G10]

### M08 — Larger timing windows and input buffering

- **Does / helps:** increases parry/dodge or interaction windows and accepts an input slightly before an action becomes valid; helps motor timing and processing needs.
- **Signal / tier / default:** precision timing; Contextual; Intended, adjustable without changing unrelated enemy behavior.
- **Implementation:** describe actual timing changes. Treat rhythm, platforming, combat, and menu confirmation independently where needed.
- **Pitfall / verification:** do not silently couple extra timing to reduced story rewards. Spider-Man 2 documents dodge/parry timing changes; GAG supports alternatives to precise timing. [G12] [A17]

### M09 — Game-speed adjustment

- **Does / helps:** changes simulation pace to allow more time to perceive, decide, and act; helps motor, cognitive processing, and fatigue.
- **Signal / tier / default:** real-time gameplay; Contextual; Intended 100%, optional reduction; convenient shortcut if rapid switching helps.
- **Implementation:** audit physics, animations, timers, input buffers, camera, audio, and scripted events. State whether menus, cutscenes, and online play are affected.
- **Pitfall / verification:** lowering FPS is not slowing simulation. Forza Horizon 5's published speed feature is offline-specific; Ratchet & Clank: Rift Apart documents speed shortcuts. [G20] [G21]

### M10 — Sensitivity, deadzones, inversion, and stable targeting

- **Does / helps:** adjusts required travel and precision; helps tremor, reduced range of motion, alternative pointing devices, and fatigue.
- **Signal / tier / default:** analog movement, pointer, touch, or aiming; Contextual; Intended with wide, useful ranges and X/Y choices.
- **Implementation:** preview without a failure state. Permit keyboard/controller adjustment without requiring the mouse under adjustment.
- **Pitfall / verification:** a narrow slider range or forced acceleration can defeat the option. Martinez's motor guide explicitly includes sensitivity and inversion. [P04]

### M11 — Adaptive controllers, mixed input, and assistive software

- **Does / helps:** preserves normal input compatibility and avoids unnecessary restrictions on devices, overlays, and alternative input paths; helps motor and situational access.
- **Signal / tier / default:** controller support, configurable input, desktop builds; Baseline compatibility assessment; supported devices available without an assist-only gate.
- **Implementation:** support standard platform input paths, mixed devices where feasible, digital alternatives to gestures/analog triggers, and navigation that does not demand a particular physical device.
- **Pitfall / verification:** an adaptive controller is not evidence that two sticks or mandatory chords are accessible. CIPT notes that adaptive devices often present as standard controllers; real setup constraints remain individual. [P04] [P05]

### M12 — Copilot / shared control

- **Does / helps:** allows two devices or people to contribute controls to one player; helps assisted play, skill sharing, and motor access.
- **Signal / tier / default:** a feasible shared-input platform path or local cooperation; Contextual/Advanced; Off/optional.
- **Implementation:** say whether the game has its own copilot feature or relies on a platform feature. Define device disconnect, active prompts, and conflicting-input handling.
- **Pitfall / verification:** assistance from another person should not be required to get through an otherwise advertised independent setup. Horizon Forbidden West documents a second-controller copilot system. [G03]

### Motor verification scenarios proposed for the skill

1. Complete launch, settings, gameplay, saving, and exit without the default pause key or default confirm button.
2. Play using only one supported input device, including all menus and minigames.
3. Trigger a held/toggled action, pause, disconnect, and resume; no unwanted permanent action remains.
4. Reach the final encounter with QTE assistance; every QTE type honors the setting.
5. Play a representative extended session with reduced repetitive input; inspect total effort rather than one successful press.
6. Change game speed during a moving-platform or timer sequence; no collision, audio, or progression regression appears.
7. Try simultaneous mouse/controller input if supported; do not let glyph switching interrupt control.

## 9. Difficulty, achievements, and assist-menu language

### The debate, with precise examples

| Example / position | Verified claim | What the skill should learn |
| --- | --- | --- |
| Control | Developer says assists do not affect trophies/achievements. [G06] | State parity explicitly and test the unlock paths. |
| Dead Cells, Breaking Barriers | Developer announcement says achievements are not disabled for these options. [G14] | A demanding game can expose granular adjustments without withdrawing achievements. |
| Steelrising, 2022 publisher/developer feature article | Says some difficulty-related achievements become unavailable if Assist Mode has been activated at least once in a playthrough. [G22] | This conflicts with the brief, including after turning an assist back off. Do not copy it. |
| Steelrising criticism | Digital Trends' review calls the trophy restriction punitive and interprets it as implying a preferred “right” way to play. [P12] | This is identifiable published criticism, not a claim of unanimous community backlash. |
| Steelrising forum discussion | A thread contains contradictory claims about all versus some achievements and patch behavior. [R10] | Use the published, dated wording above; current exact platform behavior remains UNVERIFIED without runtime/patch confirmation. |
| Minecraft Creative versus difficulty | XAG 108 distinguishes Creative's changed experience and achievement restrictions from Survival difficulty options. [A14] | Meeting a guideline's difficulty criteria does not establish this skill's no-penalty policy. A separate creative mode is not automatically an accessible campaign substitute. |
| Older Includification | Historical advice allows achievement restrictions in response to easier settings. [A03] | Source authority does not override the brief; version and policy differences must be exposed. |

No source here proves that all disabled players want every game to be easy. APX and XAG 108 instead support adjustable barriers so players can pursue the experience and degree of challenge they want. [A01] [A14]

### Celeste: framing to borrow, details not to copy blindly

The Celeste wiki reproduces the revised message's phrase “modify the game’s rules to fit your specific needs.” It also records that earlier wording recommended a first attempt without assists. An indexed extract of the official changelog confirms that the description changed and credits @halfcoordinated and @Kathy_E_J; direct changelog fetching failed, so the reproduced wording itself remains sourced through the wiki. [G11] [G24]

The same wiki documents an assist indicator on the save card. That is a reason not to reproduce the entire implementation as a template: this brief explicitly prohibits marking the save. The useful lesson is neutral, agency-focused wording, with each feature reviewed against the current project's policy. [G11]

### Wording rules proposed for this skill

| Avoid | Prefer | Why |
| --- | --- | --- |
| “Cheats”, “Wimp”, “Baby”, “For people who cannot play properly” | “Assists”, “Gameplay adjustments”, or a precise feature name | Describes function without ranking the player. |
| “We strongly recommend trying without this first” | “Change these settings at any time.” | Does not make players attempt a known barrier to justify using help. |
| “Easy: for people who only care about the story” | “Reduced incoming damage and longer timing windows.” | Describes actual changes without guessing motivation. |
| “Invincibility” when only enemy attacks are blocked | “Combat damage: Off. Falling and drowning are unchanged.” | Makes combinations and limits understandable. |
| “This ruins the intended experience” | “Adjust the game to suit your preferred experience.” | Avoids shaming a supported configuration. |
| “You used assists” on a save card or ending | Setting state within the settings UI, when needed | The brief prohibits save marking and punitive public labels. |
| “Achievements may be unavailable” when they are not | “All achievements remain available.” | Answers a real concern accurately; must be backed by verification. |
| “Accessibility mode” with an opaque bundle | A named preset with a preview of every change | Players can assess fit and retain individual preferences. |

Suggested original menu introduction:

> Adjust controls, information, presentation, and gameplay to suit you. You can change these settings at any time. All story content, endings, and achievements remain available.

Use that final sentence only after implementing and verifying it. It is a product promise, not decorative copy.

### Achievement implementation and audit requirements

- No unlock condition reads an accessibility/assist-used flag.
- No permanent “ever enabled” disqualification is written into the save.
- No achievement requires choosing an inaccessible input method when an equivalent supported method exists.
- If challenge-specific achievements would exclude supported assist combinations, redesign the achievement conditions during planning; do not postpone the conflict until QA.
- Test ordinary progression, bosses, collectibles, chapter skips, and endings with relevant assists enabled.
- Test turning assists On and Off mid-save, then earning an achievement afterward.
- Verify platform-specific unlock reporting; a local success toast alone is not proof of a platform trophy.
- Publish the no-penalty statement in settings and accessibility documentation.
- If a retrofit has unavoidable platform or legacy limitations, document the exact unresolved conflict; do not claim compliance.

## 10. Motion sickness and visual comfort

XAG 117 identifies controllable field of view, secondary camera motion, blur, and automatic camera movement as relevant design areas. It does not establish one universally comfortable configuration. [A06]

Victor Branco's low-vision Spider-Man 2 review illustrates an important interaction: he turns some effects off, while retaining an effect that helps him identify low health. A blanket removal of all feedback is not equivalent to a good comfort configuration. His criticism of the initial setup also shows that merely offering settings before gameplay is insufficient when the setup text itself is inaccessible. [P13]

### Motion-option matrix

Common beneficiaries: motion sensitivity, some low-vision users, sensory comfort, and players who find automatic visual movement distracting. Every game should assess motion; individual 3D camera options are **Contextual**, not mandatory scaffolding for a static board game. Defaults below follow the brief: preserve specified intended presentation, provide safe effects, and make comfort choices available before exposure. If the designer has not specified a default, ask them to decide or explicitly document a conservative reduced-motion choice.

| Option | What it changes / signal | Recommended default | Implementation and verification proposal | Pitfall / documented example |
| --- | --- | --- | --- | --- |
| Field-of-view slider | Visible angle in a movable 3D view. | Intended; adjustable before entering the world. | Label range clearly, including angle convention when relevant; preview the actual camera and test all supported aspect ratios. | A very wide view is not universally better. XAG 117 shows both wide-angle distortion and narrow-view tradeoffs. [A06] |
| Head bob / weapon sway | Decorative viewpoint or held-object oscillation. | Intended or Reduced if unspecified; allow Off. | Separate viewpoint bob from animation that conveys an action. Verify walking, running, crouching, swimming, and vehicles. | A switch covering only walking is incomplete. XAG 117 documents head-bob and sway considerations. [A06] |
| Camera shake | Impact, explosion, recoil, or scripted viewpoint displacement. | Reduced if unspecified; full Off must exist when shake exists. | Audit shake from every source, including UI, cutscenes, special attacks, and death. Replace lost information with a stable cue. | “0” that leaves scripted shaking is false coverage. Spider-Man 2 lists a shake toggle. [G12] |
| Motion blur | Blur from camera/object movement. | Intended; allow Off before play. | Distinguish blur types if separately implemented. Verify menu/background blur is not accidentally coupled. | Lowering effect quality is not guaranteed to remove blur. Published Insomniac settings explicitly control blur. [G12] |
| Depth of field | Focus-based blurring. | Intended; Off available where used. | Preserve readable interactables and important text at every supported setting. Include photo-like UI and cinematics in scope decisions. | “Accessibility blur Off” that excludes cutscenes needs a clear label. Insomniac documents the option. [G12] |
| Chromatic aberration | Color separation/out-of-focus edge effect. | Intended; Off available where used. | Apply to every relevant rendering path, including full-screen status effects. Check that disabling it changes no gameplay cue. | Do not call it a proven motion-sickness treatment; it is a comfort/clarity choice. Spider-Man 2 documents the switch; Branco describes personally disabling it. [G12] [P13] |
| Persistent center dot / crosshair | Stable visual reference even when not aiming. | Optional/Off unless chosen in a preset. | Adjust size, contrast, and opacity; state exclusions such as cinematics. Keep it separately configurable from weapon reticles. | Do not promise universal relief. TLOU Part I documents a persistent dot and its exclusions. [G19] |
| Camera centering and follow | Automatic rotation toward movement, action, or a target. | Intended; allow Off and/or constrained axes. | Offer speed/delay when appropriate; test manual input overriding follow without a tug-of-war. | Automatic camera help can aid motor play while discomforting another player. XAG 117 discusses this tradeoff; HFW documents auto-camera support in its later patch. [A06] [G18] |
| Camera sensitivity / smoothing | Speed, acceleration, inertia, and response. | Intended; independent useful controls. | Test very slow and very fast values, aim/vehicle modes, and changes mid-session. Avoid hiding automatic acceleration behind “sensitivity.” | Different devices and player needs require real ranges, not token adjustment. Motor guidance supports sensitivity control. [P04] |
| Camera distance / viewpoint choice | Near/far third-person camera, or existing first/third-person views. | Intended; optional where the game supports the view. | Test occlusion and navigation. A new view mode is Advanced if it requires substantial new animation/design. | Do not promise an unimplemented third-person mode. TLOU Part I documents camera distance; XAG discusses viewpoint choices. [G19] [A06] |
| Vignette and full-screen effects | Peripheral darkening, speed effects, damage pulses, and screen overlays. | Intended within safety limits; separately offered reductions. | Distinguish comfort vignette that narrows visible motion from a damage vignette. Provide critical damage information another way. | Vignetting can obscure necessary peripheral information. Spider-Man 2 documents removing damage/full-screen effects; a universal comfort-vignette benefit is UNVERIFIED. [G12] |
| UI parallax and animated menus | Menu floating, background drift, animated panels, auto-scroll. | Static/reduced if unspecified; player control over motion. | Configuration screens must honor comfort choices immediately. Support stopping movement rather than merely slowing it. | The settings screen itself can be a barrier. Dead Space documents disabling its floating menu effect. [G13] |

### Proposed motion verification route

Run the same representative route under Intended and reduced-motion settings:

1. First-launch logos, menu, character creation, and loading.
2. Walking, sprinting, turning, aiming, recoil, and damage.
3. Climbing, falling, moving platforms, vehicles, and swimming where present.
4. Scripted camera transitions, cutscenes, boss attacks, death, and respawn.
5. Returning to settings and changing camera values while already in play.

Record each automatic camera/effect source and whether the control reaches it. An unaffected tester's comfort is not proof that the configuration works for motion-sensitive players.

## 11. Blind/low-vision and Deaf/HoH needs developers commonly flatten

### 11.1 Blindness and low vision require different combinations

Christy Smith's CIPT guide explains that visual ability varies across many dimensions and recommends a range of presentation and nonvisual options. The skill should never infer a person's readable text size, contrast preference, or ability to navigate from a single label such as legally blind. [P14]

| Option | Helps / context / tier | Default | Implementation proposal and pitfall | Evidence / named example |
| --- | --- | --- | --- | --- |
| Screen-reader-compatible menus | Blindness, low vision, reading barriers; any UI; Baseline semantic/focus design, Contextual platform integration. | Respect platform preference where available and offer an audible activation path on first launch. | Expose label, value, role, state, focus, and instructions. A speaking title screen alone is not menu support. | XAG 106 covers holistic narration and platform/game-specific paths. [A19] |
| Narration of changing state | Blindness; health, inventory, rewards, timers, shops; Contextual. | User-configured verbosity, with critical events not lost. | Announce relevant changes without re-reading the whole screen. Allow interruption and repeat; avoid stale queues. | SightlessKombat identifies non-speaking rewards/shop systems as blockers. [P06] |
| Narration speed, volume, and verbosity | Blind/low-vision and cognitive needs; any speech UI; Contextual. | Respect selected preference; audible before other loud sounds. | Keep narration independent from effects/music and expose a sample. Avoid narration being masked by its own menu music. | Forza Motorsport documents voice settings and verbosity controls. [G01] |
| Audio description of cinematics | Blind/low-vision access to visual narrative; cinematics; Advanced. | Offered before the opening cinematic. | Describe meaningful silent actions without drowning dialogue. Keep description separate from UI narration; localize honestly. | XAG 111; TLOU Part I documents cinematic descriptions. [A20] [G19] |
| Navigation and object audio cues | Blind/low-vision gameplay; exploration/action; Contextual to Advanced. | Optional, with a learnable cue set. | Communicate direction, distance, interaction, or threat consistently; preserve player agency. | Forza Motorsport's driving-information design and cue previews. [G01] |
| Audio-cue glossary and rehearsal | Blind/low-vision and memory needs; added cue language; Contextual. | Available on demand. | Name and replay cues one at a time; offer practice in context. Do not require memorizing an unexplained soundscape. | TLOU Part I documents a cue glossary. [G19] |
| High-contrast objects and outlines | Low vision; cluttered worlds, target identification; Contextual. | Off/Intended with per-class choices where useful. | Separate interactables, hazards, allies, and enemies; preserve meaningful patterns without relying only on color. | Insomniac lists per-class contrast options; CIPT discusses outlines and contrast. [G12] [P14] |
| Magnification and readable text | Low vision; text, maps, inventory; Baseline readability, Contextual magnifier. | Readable default with scale changes available immediately. | Reflow text rather than clipping it. Keep focused content visible when enlarged. Test handheld and sofa distances. | CIPT low-vision guidance and XAG text display. [P14] [A21] |
| UI position and icon/prompt scaling | Low vision, restricted field of view, motor targeting; HUD/maps; Contextual. | Intended; allow adjustment. | Include icons, labels, tooltips, and diagrams; avoid scaling only body text. | Spider-Man 2 review documents separate icon/prompt sizing and the limits of initial setup. [P13] |
| Color-independent information | Color-vision differences, low vision; any color-coded state; Baseline. | Meaning available without color by design. | Add shapes, labels, patterns, placement, or appropriate additional channels. Offer semantic color choices when needed. | CIPT color-blind guide and XAG 103. [P15] [A22] |

**Independence acceptance criterion:** with an appropriate test participant and their chosen tools, verify launch, configuration, navigation, interaction, failure/recovery, inventory, shops, rewards, saving, and completion. A developer playing with the monitor off is a useful smoke check, not a substitute for that testing.

**Support claims must be precise:** distinguish narrated menus, partially narrated gameplay, fully described cinematics, and independent nonvisual play. Do not advertise full blind accessibility solely because one of those exists. Saylor and SightlessKombat provide concrete examples of why those distinctions matter. [P07] [P06]

### 11.2 Deaf and hard-of-hearing access extends beyond dialogue subtitles

CIPT's Deaf/HoH guide asks for immersion and equivalent access to information, including speakers, nonspeech cues, stealth/danger feedback, and communication. Some of the same choices help auditory-processing and cognitive needs. [P16]

| Option | Helps / context / tier | Default | Implementation proposal and pitfall | Evidence / example |
| --- | --- | --- | --- | --- |
| Subtitles for all speech | Deaf/HoH, auditory processing, situational sound-off play; dialogue; Baseline assessment. | Honor platform preference. If no preference/default exists, propose On and document the designer's choice; always offer before first speech. | Include cinematics, gameplay chatter, radios, tutorials, and voiced settings. Match final recorded audio. | CIPT recommends On; the brief's default policy still governs. [P16] |
| Captions for meaningful nonspeech audio | Deaf/HoH; sound-coded danger, puzzle, or narrative; Contextual. | Offered before the first relevant cue. | Describe the event and relevant direction/distance; avoid an indiscriminate transcript of every ambient noise. | XAG 104 distinguishes captions from dialogue subtitles. [A23] |
| Subtitle size, background, and colors | Deaf/HoH, low vision, reading; subtitles; Baseline when speech exists. | Readable size/contrast; customizable before opening cinematic. | Size text and speaker tags; provide background opacity and color choices with a live sample. Avoid pale text disappearing over snow. | XAG 104; Hamilton's subtitle guidance. [A23] [A24] |
| Speaker identity and direction | Deaf/HoH; multiple/offscreen voices; Contextual. | Names or equivalent identification available, direction when relevant. | Use name labels without prematurely revealing a disguised/unknown speaker. Do not use color alone for identity. | XAG 104 identifies speaker and spatial information requirements. [A23] |
| Subtitle timing and history | Deaf/HoH, cognitive/reading; timed text; Contextual. | Match audible content; offer readable pacing/history. | Avoid dialogue overlap wiping previous text; allow instant text display instead of compulsory typewriter animation. | Hamilton discusses subtitle coverage/presentation; CIPT covers text display and comprehension. [A24] [P16] |
| Sound visualization | Deaf/HoH; approaching enemies, gunfire, stealth detection, directional puzzles; Contextual. | Optional visual equivalent, with adjustable prominence. | Preserve timing, direction, and urgency; avoid revealing more than the intended audio unless expressly designed. Test with audio muted. | CIPT Deaf/HoH guide. [P16] |
| Mono audio and balance | Unilateral hearing differences, single speaker/headphone; spatial audio; Contextual. | Intended stereo; Mono optional and clearly labeled. | Fold information into mono without cancelling essential cues; supplement direction visually or haptically when needed. | HFW documents forced mono; GAG has a mono/stereo guideline. [G03] [A25] |
| Haptic equivalents | Deaf/HoH and multisensory preferences; supported hardware; Contextual. | Optional with intensity/Off. | Treat as an additional channel; do not require holding a vibrating controller for otherwise accessible play. | CIPT warns that vibration can also be uncomfortable. [P16] |
| Text chat, pings, and speech conversion | Deaf/HoH, speech differences, non-speaking players; communication; Contextual. | Multiple accessible routes; no requirement to disclose why voice is unused. | Make text readable and persistent; route TTS/STT separately from UI narration. Verify with supported platform tooling. | Forza Motorsport documents communication TTS/STT; APX supports alternate information channels. [G01] [A01] |
| Sign-language interpretation | Sign-language users; scripted narrative; Advanced. | Optional, with clear language choice and layout. | ASL and BSL are different language deliverables, not one universal signing switch. Captioning is still needed. | Forza Horizon 5's sign-language work is documented by Microsoft. [G23] |

### Combined sensory checks proposed for the skill

- Play muted: every essential audio-dependent instruction and event has an understandable alternative.
- Play with screen reader: its speech remains intelligible over music, cinematic dialogue, and added cues.
- Use largest captions plus minimal HUD: no action prompt or subtitle is clipped or obscured.
- Enable directional captions without a known speaker name: identity remains intentionally anonymous while direction is conveyed.
- Use Mono and no vibration: directional tasks remain playable through an appropriate additional channel.
- Preserve caption/narration preferences after returning from a cinematic, loading a save, changing devices, and applying a preset.

## 12. Thirty recurring accessibility mistakes and omissions

This is a synthesized prevention ranking. The sources support the barriers; no representative dataset establishes that these are literally the 30 most frequent mistakes among indie or jam developers. Priority combines harm, complete exclusion, cross-game relevance, and the cost of retrofitting. A contextual omission moves higher when it blocks the game's core mechanic.

| Rank | Mistake / omission | Player consequence | Skill prevention and observable check | Supporting evidence |
| --- | --- | --- | --- | --- |
| 1 | Accessibility setup appears after the first flash, loud logo, speech, or required input. | The player encounters the barrier before the remedy. | Test a fresh profile from process launch; load safety and access settings before exposure. | [A08] [A24] [P13] |
| 2 | Known hazardous flashing/patterns are shipped behind a generic warning. | A warning does not remove the exposure. | Review the final rendered output, remove failures, and document remaining known triggers specifically. | [A08] [P08] |
| 3 | Tiny text or scaling that clips content. | Menus, instructions, or objectives cannot be read. | Test supported resolutions, localization, viewing contexts, and maximum scale. | [A21] [P14] |
| 4 | Incomplete remapping with hardcoded menu/minigame controls. | A player reaches a screen they cannot leave or use. | Complete launch-to-exit using remapped actions, including pause and confirmations. | [A18] [P04] |
| 5 | Subtitles exist but are small, low contrast, incomplete, or late. | Story and instructions are missed. | Audit all speech and bright/busy backgrounds; test the opening cinematic. | [A24] [A23] |
| 6 | Color alone distinguishes state, teams, hazards, or puzzles. | Necessary distinctions disappear. | Inspect semantic states without color discrimination; add redundant encoding. | [P15] [A22] |
| 7 | Only an opaque “Easy” preset addresses access needs. | The inaccessible mechanic may remain unchanged. | Inventory barriers and expose relevant independent parameters. | [A14] [A01] |
| 8 | Mandatory mashing, long holds, simultaneous chords, or QTEs. | Pain, fatigue, or an absolute progression block. | Test tap/toggle/automatic alternatives through every required sequence. | [P04] [A16] [A17] |
| 9 | Camera shake/bob/blur cannot be fully disabled. | Some players become uncomfortable or unable to continue. | Trace all effect sources, including scripted events; verify Off visually. | [A06] [R03] |
| 10 | Essential audio lacks captions or visual equivalents. | Danger, objectives, or puzzle information is absent with sound off. | Complete relevant tasks muted and identify each missing cue. | [P16] [A23] |
| 11 | Menus speak but inventory, shops, rewards, or setup do not. | Blind players require assistance at mandatory steps. | Test every mandatory interface with supported narration. | [P06] [A19] |
| 12 | Accessibility settings reset on restart, update, or preset application. | Players repeatedly rebuild their access configuration. | Test persistence and migration; never overwrite individual values silently. | [A26] [R06] |
| 13 | Settings are locked during play or changing them requires a restart/new save. | A newly discovered barrier becomes a campaign restart. | Change an option at the point of difficulty and continue coherently. | [A14] [G15] |
| 14 | Long gaps between saves; no immediate suspend. | Short-session players lose progress or cannot stop safely. | Verify the save/resume promise at demanding and transitional moments. | [A12] [A13] |
| 15 | Pause is missing from combat, dialogue, or cutscenes. | Rest, processing, and interruptions cause unavoidable loss. | Verify simulation, media, and deadlines freeze together where pause is supported. | [A14] [P03] |
| 16 | No recap, objective reminder, or replayable instruction. | Returning or distracted players must restart or seek external help. | Recover the task after a long absence and a missed instruction. | [A09] [A10] [P11] |
| 17 | Navigation cues are absent, misleading, intermittent, or mandatory for everyone. | One player gets lost; another receives unwanted clutter. | Test reliable On, Off, and on-demand paths with meaningful labels. | [R04] [P07] [G02] |
| 18 | Gameplay or choice timers cannot be extended, removed, or paused. | Processing and motor differences become automatic failure. | Identify actual timer logic; distinguish hiding the display from altering the deadline. | [P11] [G07] [A11] |
| 19 | Decorative clutter, particles, and notification spam have no controls. | Essential information becomes difficult to isolate. | Test a busy encounter with reduced decorative output and useful history retained. | [A05] [A06] [R01] |
| 20 | One master volume, or music mute that also mutes useful cues. | Players cannot balance information and sensory load. | Verify independent buses and narration/cue audibility. | [A07] [G03] |
| 21 | Assists lock trophies/content or mark the save. | Access carries an explicit penalty or stigma. | Audit unlock predicates and persistent flags; test assisted progression. | [G06] [G22] [G11] and brief policy |
| 22 | Assist wording shames players or requires repeated failure first. | Players avoid an option that could make play possible. | Review copy and allow proactive selection; suggestions must be dismissible. | [A14] [G11] |
| 23 | A phobia mode changes one model but misses textures, sound, or cutscenes. | The advertised accommodation fails at the next appearance. | Maintain a content inventory and verify replacements across all presentation channels. | [G16] [R05] [R09] |
| 24 | Vague content warnings, or spoilers are prioritized over informed consent. | The player cannot judge whether to proceed. | Use layered specificity and offer controls before the relevant scene. | [P08] [A15] |
| 25 | No safe preview or explanation of what a setting changes. | Players must discover effects by exposing themselves. | Preview benign representative content and describe exclusions; no autoplay trigger demonstration. | [A23] [G01] [P08] |
| 26 | One disability preset overwrites another or cannot be customized. | Overlapping access needs cannot be accommodated. | Preview the union of changes; keep independent overrides and undo. | [P02] [G19] |
| 27 | Adaptive hardware is treated as a substitute for accessible game design. | Simultaneous actions and camera control still block play. | Test actual required inputs and reduced-input schemes with relevant hardware/users. | [P05] [P04] |
| 28 | New DLC, updates, or final encounters are excluded from accessibility QA. | A previously playable experience gains new blockers. | Include all new interfaces and content in regression coverage. | [P06] [R06] |
| 29 | Feature names promise more coverage than exists. | Players cannot make an informed purchase or configuration choice. | Distinguish menu narration, partial gameplay support, and independent completion; document limits. | [P06] [P07] [G08] |
| 30 | Accessibility is postponed and no disabled players are involved. | The team discovers design-level barriers after key decisions harden. | Scope with player feedback early and verify representative tasks throughout production. | [V07] [G02] [A01] |

### Why these matter particularly in indies and jams

The evidence does not measure jam-specific omission rates. The practical risk is scope: a tiny game may consist almost entirely of the one mechanic a player cannot use. Conversely, a small project can avoid a barrier at design time without building an elaborate menu. Hamilton's Shoestring session explicitly addresses small indie budgets, and Ubisoft's Lost Crown interview describes early consideration and shared team responsibility. [V07] [G02]

Recommended jam approach:

- Inspect the actual loop before choosing features: one-button, text-only, puzzle, rhythm, platforming, horror, and camera-driven games have different barriers.
- First prevent hazardous startup and unreadable/inoperable configuration.
- Keep essential state readable without color or sound alone.
- Build the minimal relevant alternative to the main input/timing barrier.
- Add saving/pausing proportional to session length and interruption needs.
- Prefer a few verified options to a large menu of disconnected toggles.
- Keep unsupported access claims out of the release page; publish exact known limits and a feedback route.
- Do not assume a jam label makes warnings, controls, or no-penalty promises unimportant.

## 13. Accessibility options that are themselves badly implemented

Some rows are documented cases; others are explicit audit failure patterns derived from guidelines. A pattern is not attributed to a named game unless a source establishes it.

| Failure pattern / documented case | Why the checkbox is insufficient | Required skill response |
| --- | --- | --- |
| Full-screen color-blind filter as the only solution | It may change every color without distinguishing the important semantic elements; it may also damage other readable contrasts. | First remove color-only dependencies; then offer meaningful per-role choices and verify with color-blind players. Do not categorically ban a filter that an individual finds useful. [P15] [A22] |
| A developer simulation filter exposed as a player fix | Simulating reduced color perception is a development diagnostic, not automatically compensation for it. | Distinguish simulation from correction; evaluate task performance, not the filter's label. XAG 103 describes simulation as an internal checking tool. [A22] |
| A high-contrast option creates a new patterned-light hazard | An accessibility feature can introduce another access barrier. | Evaluate accessibility rendering paths for photosensitivity too. The January 2024 Tekken 8 discussion documented expert objections to striped filters; do not claim current versions still behave identically. [P17] [A08] |
| Subtitles have no background, or only a weak shadow | Text can disappear over bright or complex scenes. | Adjustable background opacity/color plus actual in-game preview; test bright backgrounds. [A24] [A23] |
| “Large text” enlarges only a subset | A mandatory unread screen still blocks use. | Inventory title, menus, HUD, prompts, maps, tutorials, and error dialogs; test the smallest remaining text. Saylor's review shows why a large-font feature does not establish whole-game access. [P07] [A21] |
| Settings exist before the tutorial but the settings UI is unreadable | The player cannot configure the accommodation independently. | Make the first configuration itself navigable, readable, and narrated where supported. Branco documents a launch setup he had to configure using recorded video and external magnification. [P13] |
| Settings unlock only after the tutorial | The player must clear the barrier before removing it. | Expose controls before first required action and before opening narrative/media. Hamilton explicitly discusses opening-cinematic subtitle coverage. [A24] |
| Settings cannot change mid-game | A limitation discovered later forces loss of progress. | Apply live where feasible; explain genuinely deferred changes. GAG and XAG difficulty guidance support mid-game adjustment. [A14] [A27] |
| Update resets access settings | A working configuration becomes unusable unexpectedly. | Test migration; retain values and notify clearly about unavoidable schema changes. A player reports a color-setting reset; no claim of a confirmed universal bug. [R06] [A26] |
| “No flashes” misses intro logos, cutscenes, or damage effects | The promise fails outside the tested gameplay path. | Audit final rendering from launch onward; avoid absolute seizure-safety claims. [A08] [P08] |
| Hold-to-replace-mash is the only alternative | It moves effort from rapid input to sustained effort. | Offer brief tap/toggle/automation options when that barrier exists. [P04] [A16] |
| Color modes do not reach gameplay | A menu palette change can leave team colors or a puzzle unchanged. | Enumerate every color-coded mechanic and artifact and verify the actual distinction. [P15] |
| “Screen reader” covers only some menus | Mandatory upgrades, rewards, and shops can remain unavailable. | Track narration coverage as a matrix; never label partial support as independent full access. [P06] [A19] |
| Added audio cues compete with music and each other | More information can become an unintelligible soundscape. | Independent cue levels, priority/interrupt rules, and cue rehearsal. Forza documents cue previews and individual adjustment. [G01] [A07] |
| A warning/preview automatically shows the triggering content | Configuration becomes the exposure. | Start with text and an abstract alternative; require explicit preview activation. This is a skill design rule inferred from the requirement for advance choice, not a claim about a specific shipped preview. [A15] [P08] |
| “Arachnophobia safe” retains a threatening sound, animation, or close-up | A visual-only change may not meet the player's need. | State coverage exactly and test with affected players; optionally provide more levels or a bypass. Grounded's research considers multiple contributing attributes. [G16] |
| A bundle disables the help someone still needs | Minimal HUD may remove navigation; a vision preset may override a comfortable theme. | Make presets editable, with an explicit change summary and undo. Conflicting presentation preferences are documented by Thompson. [P02] |
| An “Off” option actually means Reduced | The UI promise is false even if the change helps some users. | Verify zero effect from every emitter/source; use Reduced if residual behavior is intended and explain it. [A06] |
| No-death mode leaves chase failure or horror unchanged without saying so | Players may buy or continue based on an incorrect interpretation. | Describe precisely which failure/stress sources change. SOMA's official explanation explicitly retains horror themes and atmosphere. [G08] |
| All assists are stored in the save and resetting settings brands the file forever | Enabling once has a permanent consequence. | Separate persistent user preferences from gameplay save data where appropriate; never add disqualification markers under this brief. Steelrising's published “activated at least once” achievement rule is the counterexample. [G22] |

### Settings-system acceptance criteria proposed for the skill

1. **Discoverable:** an accessible first-launch entry and a persistent route from normal settings.
2. **Available in time:** safety and information choices take effect before any relevant logo, media, tutorial, or interaction.
3. **Understandable:** description states effect, scope, exclusions, and whether restart is required.
4. **Previewable:** benign, user-triggered preview or immediate reversible change.
5. **Composable:** overlapping presets/options work together; no hidden mutual exclusion.
6. **Persistent:** restart, load, chapter change, device reconnect, update, and cloud/profile behavior are checked as applicable.
7. **Reversible:** change back without restarting a campaign, marking a save, or disabling achievements.
8. **Accurate:** UI value, rendered behavior, input logic, and saved preference agree.
9. **Tested:** verify the actual barrier with relevant players and representative tasks, not the existence of a checkbox.
10. **Documented:** list known gaps honestly on the release page and in the audit result.

## 14. Source register and verification notes

URLs below are the cited sources. Page fetches were attempted during this research; where direct fetches were blocked but indexed content was available, that limitation is recorded. No claim depends on an invented quotation, game, or feature.

### Advocate guidance and standards

| ID | Author / source | Material used / qualification |
| --- | --- | --- |
| A01 | [AbleGamers / Accessible Games — Accessible Player Experiences][A01] | Full framework read; barrier-oriented access/challenge distinction and pattern names. |
| A02 | [Ian Hamilton — Prioritising Accessibility Considerations, 2020][A02] | Full article read; survey/social-source caveats and recurring concerns. |
| A03 | [AbleGamers — Includification, historical PDF][A03] | Indexed historical text; achievement restriction explicitly treated as incompatible with this brief. |
| A04 | [Josh Straub / Can I Play That? — The Future of Dagersystem, 2023][A04] | Archive transfer announcement read; used only for archive provenance. |
| A05 | [Game Accessibility Guidelines — Hide background movement][A05] | Guideline read; attention/distraction barrier. |
| A06 | [Microsoft XAG 117 — Visual distractions and motion settings][A06] | Guideline content read despite generic authorization banner. |
| A07 | [Microsoft XAG 105 — Audio accessibility][A07] | Separate channel controls and masking of important audio. |
| A08 | [Microsoft XAG 118 — Photosensitivity][A08] | Prefers removing hazardous content over relying on warnings; includes unintentional triggers. |
| A09 | [GAG — Summaries of progress][A09] | Memory/returning-player guidance; player quote attribution appears on page. |
| A10 | [GAG — Narrative and instruction replay][A10] | Memory/attention rationale and examples. |
| A11 | [Microsoft XAG 116 — Time limits][A11] | Read; formal UI scope explicitly distinguished from core gameplay timers. |
| A12 | [GAG — Autosave][A12] | Guideline read. |
| A13 | [GAG — Manual save][A13] | Guideline and attributed fibromyalgia/narcolepsy accounts read. |
| A14 | [Microsoft XAG 108 — Game difficulty options][A14] | Difficulty dimensions, pausing, saves, neutral wording; its scope is not identical to the owner's policy. |
| A15 | [Microsoft XAG 123 — Mental health best practices][A15] | Readable regional version; content inventory, warning/customization/bypass guidance. |
| A16 | [GAG — Avoid repeated inputs][A16] | Guideline read, including player accounts; no specific game's behavior inferred from it. |
| A17 | [GAG — Alternatives to precise timing][A17] | Guideline read; alternatives, pause, and skips. |
| A18 | [Microsoft XAG 107 — Input][A18] | Indexed guideline text and remapping requirements. |
| A19 | [Microsoft XAG 106 — Screen narration][A19] | Guideline read; meaningful roles, values, state, and focus, not just spoken labels. |
| A20 | [Microsoft XAG 111 — Audio descriptions][A20] | Guideline read; visual information in cinematics. |
| A21 | [Microsoft XAG 101 — Text display][A21] | Indexed guidance on text scaling and readable alternatives. |
| A22 | [Microsoft XAG 103 — Additional channels for visual/audio cues][A22] | Indexed primary text distinguishes internal color simulation from accessible information. |
| A23 | [Microsoft XAG 104 — Subtitles and captions][A23] | Guideline read; background, size, speaker, preview, and coverage. |
| A24 | [Ian Hamilton / Game Developer — How to do subtitles well, 2015][A24] | Article read; presentation, coverage, first-use access. Historical platform details are not assumed current. |
| A25 | [GAG — Stereo/mono toggle][A25] | Guideline read, including attributed unilateral-hearing account; implementation checks here are synthesis. |
| A26 | [GAG — Settings saved/remembered][A26] | Guideline read; burden of repeating configuration. |
| A27 | [GAG — Change difficulty during gameplay][A27] | Guideline read; independently corroborated by XAG 108. |

### Player, reviewer, and interview sources

| ID | Author / source | Material used / qualification |
| --- | --- | --- |
| P01 | [Kylie Noble / CIPT — Contradictions of Being Autistic with ADHD, 2021][P01] | Full first-person article read. |
| P02 | [Cherry Thompson / A11y Rules — Cognitive impairments interview][P02] | Published transcript read; web-to-game application clearly labeled inference. |
| P03 | [Laura Kate Dale / Access-Ability — Autistic Player Accessibility, 2025][P03] | Author-published written video script read; proposals distinguished from shipped features. |
| P04 | [Antonio Martinez / CIPT — Motor / Physical Accessibility Guide][P04] | Full guide read; originally 2019, page notes 2026 update. |
| P05 | [Antonio I. Martinez / Game Accessibility Nexus — Access controller review, 2024][P05] | Full first-person hardware review read; experience is individual. |
| P06 | [SightlessKombat — God of War Ragnarök: Valhalla review][P06] | Full review read through web fetch; dated 2023, updated 2025. |
| P07 | [Steve Saylor / GamesRadar — Starfield accessibility verdict, 2023][P07] | Full first-person review read; historical launch context retained. |
| P08 | [Laura Kate Dale — Generic Photosensitivity Warnings, 2024][P08] | Full article/script read, including attributed player/advocate input. |
| P09 | [AbleGamers — Anthony's player-impact account][P09] | Indexed account of struggling with a setup and receiving peer support; not efficacy research. |
| P10 | [GameSpot — Saylor's Starfield review and X post, 2023][P10] | Indexed article reproduces social statement; indirect social evidence. |
| P11 | [Ruth Cassidy / CIPT — Cognitive Accessibility Guide][P11] | Full guide read; originally 2019, page notes 2026 update. |
| P12 | [Digital Trends — Steelrising review, 2022][P12] | Relevant criticism available in indexed extract; direct fetch blocked with 403. |
| P13 | [Victor Branco / Game Accessibility Nexus — Spider-Man 2 low-vision review][P13] | Full relevant review sections read; initial feature availability described historically. |
| P14 | [Christy Smith / CIPT — Blind and Low-Vision Guide][P14] | Full guide read; no demographic/medical statistics reproduced. |
| P15 | [Douglas Pennant / CIPT — Color-Blindness Accessibility Guide][P15] | Full guide read; redundancy, semantic colors, player evaluation. |
| P16 | [CIPT — Deaf and Hard of Hearing Guide][P16] | Full guide read; subtitles, cue visualization, communication. |
| P17 | [Ars Technica — Tekken 8 filter concerns, January 2024][P17] | Indexed reporting of expert/player reactions; not a claim about current filter behavior or proven injury incidence. |

### Game/developer examples

| ID | Source | What is verified / remaining boundary |
| --- | --- | --- |
| G01 | [Turn 10 / Xbox Wire — Forza Motorsport accessibility, 2023][G01] | Primary design description including Cole's feedback, cues, narration, and communication. Announcement, not this research's runtime verification. |
| G02 | [Ubisoft — The Lost Crown accessibility spotlight][G02] | Interview read; early development, guided exploration, map-pinned screenshots, granular challenge. |
| G03 | [Guerrilla / PlayStation Blog — HFW accessibility, 2022][G03] | Primary feature description; platform distinctions retained. |
| G04 | [Obsidian — Pentiment][G04] | Official accessibility section read; readable fonts and writing-effect changes. Narration explicitly excludes PlayStation/Switch in that page; no universal narration claim made here. |
| G05 | [PlayStation Blog — FFXVI hands-on, February 2023][G05] | Primary pre-release design description of Active Time Lore and pause. Not a runtime audit of the shipped build. |
| G06 | [Remedy / Control — Game Help and Assist Mode deep dive, 2021][G06] | Full article read; hints and explicit achievement parity. |
| G07 | [Laura Kate Dale — Life is Strange: Double Exposure tour, 2024][G07] | Written tour read; preview-build scope and consulting conflict disclosed. |
| G08 | [Thomas Grip / Frictional — What is SOMA's Safe Mode?, 2017][G08] | Full developer explanation read; persistent horror content and new-game selection scope retained. |
| G09 | [Double Fine — Psychonauts 2 accessibility support][G09] | Full feature page read; separate fall/combat/narrative assists. |
| G10 | [Control — August 2020 update notes][G10] | Primary notes read/indexed; parameters and Aim Snap. |
| G11 | [Celeste Wiki — Assist Mode][G11] | Full wiki read; reproduced menu text and save indicator. Primary indexed changelog corroborates description/indicator changes separately under G24. |
| G12 | [Insomniac — Spider-Man 2 accessibility options][G12] | Official list read; article mixes launch and planned additions, so no assumption that every listed future item shipped at launch. |
| G13 | [EA / Motive — Accessibility and Player Choice in Dead Space][G13] | Primary article read; precise distinction between content warnings, concealment, and retained audio. |
| G14 | [Motion Twin / Evil Empire — Dead Cells Steam announcement archive][G14] | Developer archive containing Breaking Barriers and explicit achievement-parity wording; individual news page returned only a page shell. Historical update scope. |
| G15 | [Obsidian — Grounded accessibility features][G15] | Full page read; abstraction states, settings from loading/in-game, applied changes. |
| G16 | [Obsidian / Xbox Wire — Grounded arachnophobia research, 2020][G16] | Indexed primary article; direct alternate fetch encountered 403. |
| G17 | [Official Satisfactory Wiki — Settings][G17] | Indexed official-wiki description of cat sprites; not developer source-code verification. |
| G18 | [Guerrilla — HFW Patch 1.21 announcement on Reddit][G18] | Announcement fetched; PS5 scope and new features. Detailed relief behavior also reported in community comparison. |
| G19 | [Naughty Dog / PlayStation Blog — TLOU Part I feature list, 2022][G19] | Primary list read; examples cited only within documented scope. |
| G20 | [Xbox Wire — Forza Horizon 5 features, 2021][G20] | Indexed primary description explicitly limits game-speed setting to offline play. |
| G21 | [Sony Interactive Entertainment — Rift Apart accessibility, 2021][G21] | Indexed primary description of speed shortcuts. |
| G22 | [Spiders / Xbox Wire — Steelrising Assist Mode, 2022][G22] | Primary article read, including “activated at least once” restriction. Current exact platform behavior remains UNVERIFIED. |
| G23 | [Microsoft — Forza Horizon 5 sign-language support][G23] | Primary implementation account; ASL/BSL scope. |
| G24 | [Maddy Makes Games / Extremely OK Games — Celeste changelog][G24] | Relevant official indexed extract read; direct page fetch failed. Description and save-indicator changes corroborated, not a current runtime audit. |

### Community threads

| ID | Thread | Material used / qualification |
| --- | --- | --- |
| R01 | [r/ADHD — Side quests and completion becoming a chore][R01] | Opening post and identified replies fetched. Personal experience, not diagnostic evidence. |
| R02 | [r/autism — Sensory overload from video games][R02] | Surviving comments fetched; opening post deleted, not reconstructed. |
| R03 | [r/disabledgamers — What matters most to you?][R03] | Identified replies fetched; no population ranking inferred. |
| R04 | [r/gaming — Should yellow paint be toggleable?][R04] | Opposing preferences and partner-navigation account fetched; no diagnosis inferred. |
| R05 | [r/CoreKeeperGame — Trypophobia][R05] | Post and replies fetched; current mod availability not established. |
| R06 | [r/rivals — Accessibility settings resetting after updates][R06] | Player report fetched; no independently reproduced bug or current fix status. |
| R07 | [ResetEra — Treatment of users with disabilities/accessibility issues][R07] | Relevant indexed opening-post text; direct fetch blocked with 403. |
| R08 | [r/thalassophobia — HFW relief-mode discussion][R08] | Indexed discussion including explicitly unmet needs; not a clinical evaluation. |
| R09 | [r/Games — Phobias triggered by games][R09] | Thread fetched; injection-related accounts are user reports. |
| R10 | [TrueAchievements — Steelrising Assist Mode discussion][R10] | Indexed contradictory player claims; used to identify uncertainty, not settle current behavior. |

### Link definitions

[A01]: https://accessible.games/accessible-player-experiences/
[A02]: https://ian-hamilton.com/prioritising-accessibility-considerations/
[A03]: https://accessible.games/wp-content/uploads/2018/11/AbleGamers_Includification.pdf
[A04]: https://caniplaythat.com/2023/10/02/the-future-of-dagersystem/
[A05]: https://gameaccessibilityguidelines.com/provide-an-option-to-turn-off-hide-background-movement/
[A06]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/117
[A07]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/105
[A08]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/118
[A09]: https://gameaccessibilityguidelines.com/if-using-a-long-overarching-narrative-provide-summaries-of-progress/
[A10]: https://gameaccessibilityguidelines.com/allow-all-narrative-and-instructions-to-be-replayed/
[A11]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/116
[A12]: https://gameaccessibilityguidelines.com/provide-an-autosave-feature/
[A13]: https://gameaccessibilityguidelines.com/provide-a-manual-save-feature/
[A14]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/108
[A15]: https://learn.microsoft.com/en-za/gaming/accessibility/xbox-accessibility-guidelines/123
[A16]: https://gameaccessibilityguidelines.com/avoid-repeated-inputs-button-mashing-quick-time-events/
[A17]: https://gameaccessibilityguidelines.com/do-not-make-precise-timing-essential-to-gameplay-offer-alternatives-actions-that-can-be-carried-out-while-paused-or-a-skip-mechanism/
[A18]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/107
[A19]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/106
[A20]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/111
[A21]: https://learn.microsoft.com/gaming/accessibility/xbox-accessibility-guidelines/101
[A22]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/103
[A23]: https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/104
[A24]: https://www.gamedeveloper.com/audio/how-to-do-subtitles-well-basics-and-good-practices
[A25]: https://gameaccessibilityguidelines.com/provide-a-stereo-mono-toggle/
[A26]: https://gameaccessibilityguidelines.com/ensure-that-all-settings-are-saved-remembered/
[A27]: https://gameaccessibilityguidelines.com/allow-difficulty-level-to-be-altered-during-gameplay-either-through-settings-or-adaptive-difficulty/
[P01]: https://caniplaythat.com/2021/07/06/in-video-games-contradictions-of-being-autistic-with-adhd-make-sense/
[P02]: https://a11yrules.com/podcast/cherry-thompson-talks-about-cognitive-impairments-and-web-accessibility/
[P03]: https://access-ability.uk/2025/03/07/autistic-player-accessibility-in-video-games/
[P04]: https://caniplaythat.com/2019/07/04/basic-accessibility-options-for-mobility/
[P05]: https://www.gameaccessibilitynexus.com/blog/2024/04/24/playstation-access-controller-review/
[P06]: https://www.sightlesskombat.com/god-of-war-ragnarok-valhalla-accessibility-review/
[P07]: https://www.gamesradar.com/starfield-accessibility-verdict/
[P08]: https://access-ability.uk/2024/11/29/generic-photosensitivity-warnings-protect-liability-not-users/
[P09]: https://www.ablegamers.org/ablegamers-impact-anthony/
[P10]: https://www.gamespot.com/articles/psa-starfields-accessibility-options-are-very-limited/1100-6517402/
[P11]: https://caniplaythat.com/2019/09/12/basic-accessibility-options-for-cognitive-accessibility/
[P12]: https://www.digitaltrends.com/gaming/steelrising-review-ps5/
[P13]: https://www.gameaccessibilitynexus.com/blog/2023/10/16/marvels-spider-man-2-low-vision-review/
[P14]: https://caniplaythat.com/2020/01/18/basic-accessibility-options-for-blind-and-low-vision-players/
[P15]: https://caniplaythat.com/2020/01/29/color-blindness-accessibility-guide/
[P16]: https://caniplaythat.com/2019/07/11/basic-accessibility-options-for-deaf-hoh-players/
[P17]: https://arstechnica.com/gaming/2024/01/tekken-8s-colorblind-mode-is-causing-migraines-vertigo-and-debate/
[G01]: https://news.xbox.com/en-us/2023/04/27/forza-motorsport-accessibility-features-blind-driving/
[G02]: https://news.ubisoft.com/en-gb/article/eQyaL5awqH4AXyovZUG58/prince-of-persia-the-lost-crown-accessibility-spotlight
[G03]: https://blog.playstation.com/2022/02/10/accessibility-features-in-horizon-forbidden-west/
[G04]: https://pentiment.obsidian.net/
[G05]: https://blog.playstation.com/2023/02/28/hands-on-with-final-fantasy-xvi-new-gameplay-details/
[G06]: https://legacy.controlgame.com/es/game-help-on-playstation-5-and-assist-mode-in-control-a-deep-dive/
[G07]: https://access-ability.uk/2024/10/24/touring-accessibility-in-life-is-strange-double-exposure/
[G08]: https://frictionalgames.com/2017-11-what-is-somas-safe-mode/
[G09]: https://www.doublefine.com/games/support/psychonauts-2/accessibility?platform=all
[G10]: https://legacy.controlgame.com/de/control-august-update-notes/
[G11]: https://celeste.ink/wiki/Assist_Mode
[G12]: https://support.insomniac.games/hc/en-us/articles/46730041467027-What-Accessibility-options-does-Marvel-s-Spider-Man-2-feature
[G13]: https://www.ea.com/ea-play/news/accessibility-and-player-choice-in-dead-space
[G14]: https://store.steampowered.com/news/posts/?appgroupname=Dead+Cells&appids=588650&enddate=1658241276&feed=steam_community_announcements
[G15]: https://grounded.obsidian.net/news/grounded/accessibility-features
[G16]: https://news.xbox.com/en-us/2020/07/27/grounded-arachnophobia-mode-details/
[G17]: https://satisfactory.wiki.gg/wiki/Settings
[G18]: https://www.reddit.com/r/horizon/comments/12qo4vr/horizon_forbidden_west_patch_121/
[G19]: https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/
[G20]: https://news.xbox.com/en-us/2021/11/04/gaming-for-everyone-the-accessibility-features-of-forza-horizon-5/
[G21]: https://sonyinteractive.com/en/news/blog/inside-the-accessibility-features-of-ratchet-clank-rift-apart/
[G22]: https://news.xbox.com/en-us/2022/09/08/steelrisings-assist-mode-feature/
[G23]: https://news.microsoft.com/source/features/work-life/forza-horizon-5-introduces-sign-language-support-throughout-in-game-scenes/
[G24]: https://www.celestegame.com/changelog.html
[R01]: https://www.reddit.com/r/ADHD/comments/tj260i/gamers_with_adhd_do_you_end_up_getting_fixated_on/
[R02]: https://www.reddit.com/r/autism/comments/10tto7q/sensory_overload_from_video_games/
[R03]: https://www.reddit.com/r/disabledgamers/comments/11dgh6a/what_matters_most_to_you_as_a_disabled_gamer/
[R04]: https://www.reddit.com/r/gaming/comments/1cjxrjb/how_would_you_feel_if_the_yellow_paint_issue_was/
[R05]: https://www.reddit.com/r/CoreKeeperGame/comments/1mpip0z/trypophobia/
[R06]: https://www.reddit.com/r/rivals/comments/1vmixsr/accessibility_settings_resetting_every_single/
[R07]: https://www.resetera.com/threads/we-need-to-have-a-talk-about-how-era-users-interact-with-users-with-disabilities-and-accessibility-issues.260895/
[R08]: https://www.reddit.com/r/thalassophobia/comments/12uski1/
[R09]: https://www.reddit.com/r/Games/comments/fxs96l/what_phobias_have_games_tripped_for_you_or_how/
[R10]: https://www.trueachievements.com/forum/viewthread.aspx?tid=1408659
[V01]: https://www.youtube.com/playlist?list=PLc38fcMFcV_vvWOhMDriBlVocTZ8mKQzR
[V02]: https://www.youtube.com/watch?v=4NGe4dzlukc
[V03]: https://www.youtube.com/watch?v=xrqdU4cZaLw
[V04]: https://www.youtube.com/watch?v=Ufe0i26DGiA
[V05]: https://www.youtube.com/watch?v=ObhvacfIOg0
[V06]: https://www.youtube.com/watch?v=PiCsvZZh5-k
[V07]: https://www.gdcvault.com/play/1027451/Independent-Games-Summit-Accessibility-on
[V08]: https://www.gdcvault.com/play/1020377/Accessibility-Lessons-Learned-from-Designing
[V09]: https://www.youtube.com/watch?v=-bGEMsrYA6g
[V10]: https://www.youtube.com/watch?v=poeoiuN2GZ4
[V11]: https://www.brandoncole.net/?p=243
[V12]: https://www.youtube.com/watch?v=THbVXGulDUE
[V13]: https://www.youtube.com/watch?v=c-a2bEuAETs
[V14]: https://www.youtube.com/watch?v=2mTKwNLIZuU
[V15]: https://www.youtube.com/watch?v=joDnuQaGXsg

## Implications for the skill

These are proposed enforceable rules, not additional prevalence claims.

### Wording, defaults, and presets

- Describe barriers and effects in plain language; do not require a diagnostic category to find an option.
- Never shame, label a save, lock an achievement/trophy, withhold an ending, or alter a grade because assistance was used.
- Do not blindly preserve the locally known “ASSIST ON” badge as a save/end-screen marker; use setting state only where necessary to operate the interface.
- Keep intended gameplay defaults except where safety requires safer behavior. Record the decision explicitly; an imported preset must not silently become the default.
- Apply saved safety/access choices before presentation. Remove known hazardous flashing rather than rely on a warning.
- Provide optional task-oriented presets: Less visual activity, More navigation support, More time to respond, Reduced physical effort, Readable text, and Lower combat pressure. Present them as editable starting points, not universal disability solutions.
- Preview the exact setting changes, preserve independent overrides, offer Undo, and allow category-level reset without erasing the rest of the profile.
- Explain an option's exclusions. Combat damage Off is not the same promise as no death; selected scene concealment is not global gore removal.

### First-launch accessibility prompt

1. Load stored preferences and available platform access preferences before splash media and sound.
2. Use a stable, readable, non-flashing screen with an audible access-entry path where narration is supported.
3. Offer a short route to text/captions, narration, audio, motion/sensory, and input setup, plus Continue. Do not demand medical disclosure.
4. Allow access to all settings before the tutorial or opening cinematic. The prompt itself must work with the supported controls and assistive tools.
5. Do not expose original phobia imagery, intense flashes, or loud sounds in automatic previews. Use benign representative samples and explicit activation.
6. Keep a persistent route back to these settings during play; do not make the prompt the only discovery opportunity.
7. Verify from a fresh profile, then a returning profile, then a migrated profile after an update.

### Ranked omissions the skill must prevent

Use the full section 12 table for evidence and acceptance checks. This compact order is for audit output; elevate a contextual blocker to the top when it prevents the core experience.

1. Access settings offered after first exposure or required action.
2. Hazardous flashes/patterns left behind generic warnings.
3. Unreadable or clipped text.
4. Incomplete remapping and hardcoded controls.
5. Incomplete or unreadable subtitles.
6. Color-only essential information.
7. One broad difficulty preset instead of relevant independent controls.
8. Compulsory mash, hold, chord, or QTE barriers.
9. Uncontrollable camera and motion effects.
10. Essential sound with no equivalent information channel.
11. Partial narration presented as complete UI access.
12. Lost or overwritten settings.
13. Mid-game settings changes blocked.
14. Insufficient save/suspend support.
15. Missing pause in required single-player activity or cinematics.
16. Missing recap, reminders, and instruction replay.
17. Unreliable guidance or compulsory clutter.
18. Unadjustable time pressure.
19. Uncontrollable decorative clutter and notifications.
20. Audio channels that cannot be balanced independently.
21. Achievement/content/save penalties for assistance.
22. Shaming wording and failure-gated access to assists.
23. Incomplete phobia/content replacements.
24. Uninformative or late content warnings.
25. No safe preview or accurate effect description.
26. Presets that cannot combine or preserve overrides.
27. Hardware support substituted for accessible interaction design.
28. Accessibility regressions in updates, DLC, or late content.
29. Unsupported accessibility claims and hidden coverage gaps.
30. Accessibility postponed until release without disabled-player involvement.

### Routing and completion rules

- Inspect mechanics and content first, then load only applicable references. No FPS camera guidance for a static text game; no arachnophobia asset work when no spider-like content exists.
- Always assess first-launch access, readable/operable UI, safety, information channels, persistence, and the no-penalty policy.
- Route timers/real-time reactions to cognitive, motor, and stress checks together; route navigation to memory, vision, and sensory-clutter checks together.
- Route overlapping needs to combinational verification, not mutually exclusive presets.
- In retrofit mode, distinguish Verified, Partially supported, Absent, Not applicable, and UNVERIFIED; record the exact task, version, platform, and evidence.
- Require functional checks of each claimed effect and its save/restart/update behavior. Do not mark a feature complete merely because its menu option exists.
- Keep player testimony attributed, preserve disagreement, and never convert forum anecdotes into diagnostic defaults or prevalence statistics.


