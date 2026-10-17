# Verification

Load this when implementing, before calling any option done, and before release. A setting existing
in a menu is **not** evidence that it works.

## Result vocabulary

Record every check in `ACCESSIBILITY.md` as one of:

| Result | Meaning |
| --- | --- |
| `pass` | Exercised in a running build. Observed the described behaviour. |
| `fail` | Exercised and wrong. Fix it, or downgrade the claim. |
| `blocked` | Could not run, for example missing engine, SDK, device, screen reader or analyser. **Name the missing prerequisite.** Never report it as a pass. |
| `not tested` | Possible but not done yet. |
| `not applicable` | The signal is confirmed absent. Give the evidence. |

Keep the build or commit, platform, resolution and settings snapshot with each result.

## Evidence levels: what each proves

| Level | Proves | Does not prove |
| --- | --- | --- |
| Static check (schema, grep, scan) | Keys, references and policy greps are consistent | That a player can use it |
| Unit test | A policy returns the right value | That gameplay calls the policy |
| Scenario test in the running game | That route works with those settings | That every route works |
| Captured-output analysis (flash, contrast, text size) | Those frames meet the criteria | That uncaptured content is safe |
| Disabled-player playtest | Those players completed those tasks | That everyone with that disability can |

Aim for the highest level the environment allows. Name the level you reached.

## 1. Always (Baseline checks)

- [ ] **Fresh profile first launch:** delete the settings file and launch. Settings are reachable
      before any logo, music, speech or flashing. The first screen is static. Audio starts quiet.
- [ ] **Operable menus:** complete first launch → settings → start → pause → settings → quit using
      **keyboard only**, then **controller only** (if supported). Focus is always visible. No dead
      ends. Back always works.
- [ ] **Text:** at the minimum supported resolution and maximum text scale, every screen (menus,
      HUD, tooltips, dialogs, subtitles, error popups) is readable with nothing clipped or overlapping.
- [ ] **Contrast:** text and essential icons meet 4.5:1 (large text 3:1). Run
      `python scripts/contrast_check.py <fg> <bg>` on theme colours. Check subtitles over the
      brightest and busiest scenes.
- [ ] **Not colour-only:** take a grayscale screenshot of each colour-coded state. Every distinction
      is still readable from symbol, shape, label or position.
- [ ] **Not sound-only:** play the critical path **muted and with vibration off**. Every essential
      cue has an actionable visual or text equivalent. Haptics are an extra channel, never the only
      one.
- [ ] **Photosensitivity:** record boot, menus, transitions, video and the busiest effects, and
      **analyse** the footage with a pinned tool (see topics/photosensitivity.md). No analyser
      available means `blocked`, not `pass`. Also confirm FlashGuard is the only path to full-screen
      flashes and that a sustained 2 Hz trigger (for example holding fire for 6 seconds) is capped.
- [ ] **Persistence:** change one setting in each category, quit, relaunch, and confirm it is still
      set. Load an *old* story save: accessibility settings do not revert.
- [ ] **Isolation:** change each setting and confirm no other setting or system changes, for example
      VSync not resizing the window, or a preset not erasing a manual value set afterwards.
- [ ] **Live change:** change settings mid-gameplay from pause. They apply without a restart and
      without losing progress.
- [ ] **No-penalty grep:** search achievement, save, ending and score code for `assist`, `difficulty`,
      `settings`, `a11y` and `easy`. No reads of assist state. No assist flags written to saves.
- [ ] **Wording review:** read every option label and description against core-policy section 6.

## 2. Per-signal checks (run the ones whose signals are present)

| Signal family | Check |
| --- | --- |
| Damage / death (S041) | `damage_taken = 0`: enemy and hazard hits do no harm, falls still follow `no_fall_death`, scripted defeats still progress, no stun-lock. |
| Falls / pits (S043) | `no_fall_death`: falling returns to the last safe spot. Progress and items kept. |
| Resources (S035, S040, S101) | Infinite ammo or stamina: actions still play their animations, no underflow, quest items still consumed, turning it off mid-way leaves a valid count. |
| QTE (S014) | `auto` completes **every** QTE type, including boss finales and cinematics, through the success branch. `slow` gives a visibly longer window. |
| Mashing / grabs (S015, S042) | `hold` and `single` modes each complete the longest mash sequence. |
| Holds (S016) | `toggle`: one press starts and one ends. Pause, death and focus loss clear the latch. |
| Timing (S013, S036) | A boundary input that fails at 1.0× succeeds at the configured scale. |
| Game speed (S013, S039) | At 0.5×, physics, animation, projectiles and AI stay in sync. Subtitles and menus run at normal speed. |
| Timers (S097, S075) | `extended` doubles non-race countdowns. `off` removes them. Hidden script timers obey too. |
| Skips (S086, S045, S048, S118, S119) | Skip from pause: same rewards, flags, achievement events and next-scene state as success. Save and reload immediately afterwards and the state is coherent. Challenge skips (`request_skip`) follow `assist.skip_available` only; sensitive-scene skips (`request_scene_skip`) follow `content.scene_skip` only. Check all four combinations. |
| Speech (S066, S067) | Every voiced line, including barks, radio and cutscenes, has a subtitle with a speaker name, at most 2 lines. Size and background settings apply. |
| Sound cues (S006, S068) | Sound captions and the visualiser fire with audio muted and show direction. Repeated loops merge. |
| Camera (S051, S052, S057) | Shake 0 means none from any source, including cutscenes. Bob 0 means none. Weapon sway 0 means none, and aim accuracy does not change. Effects 0 removes vignette, aberration and overlays. FOV applies. |
| Flashing (S058) | Spam the flashiest action for 6+ seconds. At most 3 counted flashes in any second and 9 in any 5 seconds; a request counts if it covers ≥ 25% of the screen, changes luminance by ≥ 0.1, is saturated red, or has unknown area. Small, moderately bright repeated flashes are capped too. `reduce_flashing` makes them steady. Captured footage analysed. |
| Guidance (S061) | `off` hides paint, trails and highlights but never hazard cues. `on_request` reveals on demand. `boosted` is clearly stronger. |
| HUD (S062) | `minimal` keeps the chosen essentials, and pause and settings stay reachable. |
| Content filter (S109–S118) | Level 0 is the original (the control). Each higher level shows exactly the substitutions its description promises, on every channel it names (model, animation, sound, icon, corpse, portrait), before and after swap, death, reload and cutscene. A level that promises full replacement shows no original asset. The preview starts at the most filtered level. |
| Objectives / story (S081, S082) | Load a mid-game save "after a month away": the recap and objective tell you where to go and why, without spoilers. |
| Puzzles (S086) | Hints come in order (nudge, then hint, then solution), each only on request. Solving with colour removed and audio muted is possible. |
| Remapping | Rebind every action, including menu confirm and back, to non-defaults. All prompts update. Restart: the bindings persist. Reset works. |
| Narration | With narration on, navigate every menu. Each focus change speaks name, role and value. No double speech with a screen reader. Narration that cannot run reports as unavailable without crashing. |
| Multiplayer (S123–S133) | Join a game, communicate with no voice, read chat history, mute and report, all with keyboard or controller only. |

## 3. Combinations (run whichever apply)

- Minimal HUD + captions + objective reminder: each chosen channel still appears.
- Guidance off + objective marker on + hints on request.
- Game speed 0.5 + timing window 2× + subtitles: a 0.2 s authored window lasts 0.2 × 2 / 0.5 =
  0.8 s of real time (not 1.6 s, not 0.4 s), and captions stay in sync with audio.
- Damage 0 + falls on (or the reverse): each failure channel independent.
- Infinite stamina + toggle sprint + traversal assist.
- Max text + max subtitles + longest remapped prompt at minimum resolution.
- Reduce motion + high contrast + reduce flashing (contrast mode creates no new strobing).
- Umbrella overrides: `motion.reduce_motion` on with one per-effect key `on`, and off with one key
  `off`; `visual.high_contrast` on with one outline class `off`, and off with one class `on`. Only
  the overridden part differs, and it keeps its value when the umbrella is toggled again.
- Full jump scares + scare warnings on: the warning appears and the scare is unchanged.
- Aim pull on + slowdown 0 + lock-on off: pull works alone.
- Content filter level 3 + newly spawned enemies + replayed cutscene.

## 4. Automating in the engine

- **Unit tests** for the store: defaults, clamping, isolation, persistence round trip, single-value
  repair, migration, OS-pref precedence, preset apply and undo. The Godot sample's
  `tests/godot/run_tests.gd` is a template.
- **Scenario tests:** a headless or scripted run per assist, using a fixed seed. Assert *outcomes*:
  the player is alive, the room was completed, the achievement event fired once. Bound every run with
  a timeout. Mute test runs (for example `--audio-driver Dummy`).
- **Screenshots:** capture key screens at minimum resolution and maximum text scale, plus a
  grayscale copy, for review.
- **Flash capture:** record gameplay video at fixed output settings and analyse it with EA IRIS or
  an equivalent pinned tool. A tool that did not run is `blocked`.

## 5. People

Automated checks cannot tell you whether something is usable. Before release, try to include
disabled playtesters, for example through [AbleGamers Player Panels](https://www.ablegamers.org/player-panels/)
or community groups. Pay them. Let them use their own devices and settings. Never expose
photosensitive or phobic testers to known triggers. Record tasks, blockers and fixes, not just
opinions.

## 6. Release gate

- [ ] Every option in `ACCESSIBILITY.md` is `pass` or honestly `blocked` or `not applicable`.
- [ ] Baseline checks pass on each shipped platform (exported build, not just the editor).
- [ ] No-penalty grep is clean. Achievements tested with assists on.
- [ ] Public accessibility notes match verified behaviour and list the known gaps.
