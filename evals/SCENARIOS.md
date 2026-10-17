# Skill evaluation scenarios

Run each prompt in a fresh agent session **without** the skill (baseline), then **with** the skill
installed (tell the agent the skill is at `skills/game-accessibility/` and to use it). Score each
answer against its rubric. One point per item. Record the results in `RESULTS.md`.

Baseline answers for A and B are in [BASELINE_RED.md](BASELINE_RED.md).

## A. Proactive build: first-person survival horror (Godot)

> We're starting a new first-person survival horror game in Godot 4. The player explores an
> abandoned research station, sneaks past giant spiders and infected crew, solves keypad and wiring
> puzzles, manages scarce flashlight batteries and ammo, and there are jump scares, a few chase
> sequences and some QTE struggle moments when grabbed. There's voiced radio dialogue. Please write
> the technical implementation plan for the first milestone: player controller, camera, enemy AI,
> UI/HUD, menus and settings, save system. Be concrete about what you will build. We have 6 weeks.

Rubric:
1. Accessibility services (settings store, policy, input assist) are built in week 1–2, not as a late settings tab.
2. Graded spider-appearance filter (content variants, same hitboxes), planned with the spider enemy.
3. QTE/grab: auto-complete and mash→hold/single alternatives planned with the grab hook.
4. Chase sequences: assist and/or skip that keeps progress.
5. Jump scares: reduced/warned option, separate from enemy lethality; sudden-loud-sound reduction.
6. Granular assists: damage, infinite batteries/ammo, enemy aggression/detection, separate fail channels.
7. Health/damage is not shown by screen tint only; flashlight flicker and other flashes go through a flash guard.
8. Hearing-based stealth gets a sound visualiser / sound captions; radio dialogue has subtitles with speaker names.
9. Puzzles: wiring is not colour-only; hints and skip.
10. First-launch accessibility screen before logos/audio; settings in title and pause; OS prefs.
11. No-penalty rule stated (achievements/saves untouched).
12. `ACCESSIBILITY.md` created and verification of options planned (tests/scenario runs).

## B. Retrofit: Unity turn-based JRPG, 3 weeks to Steam launch

> My Unity turn-based JRPG is basically done; we release on Steam in 3 weeks. It has a 40-hour
> story, lots of text dialogue (no voice), a colour-coded elemental weakness system (red fire, blue
> ice, green wind), timed button presses during attacks for bonus damage, a world map, side quests,
> and achievements for beating bosses on Hard. A reviewer said we should 'add accessibility'. What
> exactly should we add before launch? Give me a prioritised list of concrete changes with brief
> implementation notes.

Rubric:
1. Does **not** recommend excluding assist users from achievements; flags the Hard-boss achievements as a policy conflict needing an owner decision, with redesign options.
2. Element icons/labels paired with colours (not filter-only).
3. Timed presses: wider window + auto-success option.
4. Text: size, plain/dyslexia-friendly font as a preference, background, instant text, backlog.
5. Narration not dismissed: menu narration via Unity AccessibilityHierarchy (version-gated) or TTS self-voicing for menus/dialogue, with a scope decision for gameplay.
6. JRPG specifics: battle speed, encounter rate/auto-battle or equivalent, objective/journal + recap on load.
7. Settings reachable before the first text/scene; persistence; presets editable; no shaming copy.
8. Audit structure: safety first (flashing check), blockers, then contextual; honest "verified vs not".
9. Steam accessibility features claimed only after verification.
10. Verification steps (grayscale screenshots, timed-press assist exercised, persistence).

## C. Negative control: cozy 2D puzzle game (no combat, no speech)

> I'm making a small 2D sliding-tile puzzle game in Godot for a game jam. No combat, no voice, no
> timer — just levels you solve at your own pace, with coloured tiles that must match coloured goals,
> soft music and a little particle sparkle when you win a level. Plan the accessibility work.

Rubric:
1. Does **not** add combat/damage/ammo/FOV/QTE/subtitle-speech options (records absence instead).
2. Colour tiles get symbols/patterns + colour profiles; contrast checked.
3. Win sparkle checked for flashing; reduce-flashing/particle density considered.
4. Hints (graduated) and level skip that still counts progress; undo/reset.
5. Keyboard/controller/mouse operation; remapping; select-then-place instead of drag.
6. Text scale, readable font; menu narration (cheap via Godot 4.5+/Redot accessibility props or TTS).
7. Music/SFX volume, quiet first launch.
8. Scope proportionate to a jam (prioritised short list), verification included.

## D. Feature hook: lightning storm + lockpicking minigame (Unreal stealth game)

> In my Unreal 5 stealth game, add a lightning storm weather system for the night levels and a
> rhythm-based lockpicking minigame for doors.

Rubric:
1. Lightning routed through a flash guard (≤3 bright flashes per second, reduce-flashing variant, no red flashes); thunder gets a sound caption.
2. Storm screen effects respect shake/effects scaling; storm audio on its own bus; loud-thunder reduction considered.
3. Lockpicking: timing window scale, slow/auto option, haptic/visual beat alternative to audio rhythm, skip that opens the door normally.
4. Rain/darkness doesn't hide stealth detection state (multimodal detection indicators remain).
5. Updates `ACCESSIBILITY.md` signals (S058, S013, S069/S072 as applicable) and loads only relevant guides (no JRPG/vehicle/VR).
6. Verification steps for both features.

## E. Developer asks for an achievement penalty (Godot action platformer)

> My Godot action platformer has an Assist Mode (invincibility, slower game speed, skip
> level). Please make it so Steam achievements are disabled for the rest of the save once a player
> turns Assist Mode on, and show a warning in the menu when they enable it. Implement it.

Rubric:
1. Pushes back **before** implementing and recommends against the penalty.
2. Says it makes the player feel bad (unwelcome, "playing wrong").
3. Says players are likely to stop playing.
4. Names the worst case: a Steam refund, accurately (inside 14 days and under 2 hours of playtime).
5. Offers alternatives (feat-based achievements, opt-in HUD reminder, separate leaderboard).
6. If the developer still insists, it records the decision and warning in `ACCESSIBILITY.md` and
   doesn't call the result accessible. It never adds the penalty silently.
