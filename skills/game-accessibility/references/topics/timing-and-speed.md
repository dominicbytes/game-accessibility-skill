# Timing and speed

**Load when:** S013 timed inputs, S036 parry/dodge windows, S039 fast multi-enemy pressure, S045
chases or autoscroll, S072 rhythm, S075 timed dialogue or choices, S097 time limits, S098 UI
timeouts, S104 long uninterrupted sequences, S108 shared server clocks, S133 idle kicks, S146
real-time strategy, S148 wave countdowns, S152 day cycles and timed minigames. Pause (D27) is
Baseline for any real-time single-player or local game.
**Skip when:** every decision and action waits for the player, nothing expires, and no simulation
runs while the player reads. A turn-based label alone is not proof: check minigames, lobbies and
expiring UI.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| D14 | Game speed (simulation clocks only) | `assist.game_speed` | 1.0 | C | M, C |
| D15 | Slow motion on demand (bound action, optional auto-trigger) | `game.assist.slow_motion` | Off | C | M, C |
| D16 | Wider timing windows and input buffer (parry, dodge, perfect hit, rhythm note) | `input.timing_window_scale` | 1.0 | C | M, C, V, H |
| D29 | Gameplay countdowns and timed choices: extended or off | `assist.timers` | `normal` | C | C, M, T |
| C23 | UI deadlines and transient text wait for the player | `notifications.mode` (log), see [memory-and-navigation](memory-and-navigation.md) | Wait | B | C, V, H, M |
| D27 | Pause anywhere, including cutscenes and QTEs | none (core behaviour) | On | B | M, C, H, S, T |
| L20 | Pause when the window loses focus | `system.pause_on_focus_loss` | true | C | M, S, T |

Related options live elsewhere: relaxed QTE timing (M17) and recognition windows (M39) in
[input-and-motor](input-and-motor.md); coyote time and jump buffers (D20) in
[traversal-and-platforming](traversal-and-platforming.md); caption duration (H31) in
[audio-and-captions](audio-and-captions.md).

## How to implement

### Start with a clock inventory

List every clock before touching speed or timers. Record it in `ACCESSIBILITY.md`.

| Clock | Contains | Changed by |
|-------|----------|------------|
| Simulation | Movement, physics, AI, projectiles, hazards, cooldowns, spawn timers, gameplay countdowns | `assist.game_speed`, slow motion |
| Presentation / media | Voice, music, video, subtitle cues tied to audio | Never by game speed |
| UI | Menus, focus feedback, previews, settings, pause | Never |
| External | Network deadlines, matchmaking, platform services | Never by a local setting |

Search the project for timers, tweens, animation players, `delta` math, physics steps and
`time`/`sleep` calls. Tag each with its clock. A gameplay timer driven by wall-clock time ignores
game speed and breaks pause.

### Game speed (D14)

```
dt_sim = dt_real * policy.time_scale()   # simulation only
```

- Scale simulation delta, not the frame rate. Lowering FPS is not slowing the game.
- Menus, pause, settings, narration and subtitle timing tied to audio run at normal speed.
- Physics: scale the simulation step, then retest collision at the slowest setting.
- **Two independent choices, one formula.** Measure timing windows in simulation time and apply
  `input.timing_window_scale` once. Real duration = base × window_scale / time_scale. A player may
  combine both deliberately (the "More time" preset does), and gets both. The bug is applying
  either multiplier twice: scaling the window by `time_scale` as well, measuring it in real time
  and then compensating, or passing an already-scaled window back through `timing_window()`.
  See [architecture](../architecture.md) section 2.
- Apply live, mid-encounter, at the next simulation step. No restart.
- The registry range is 0.5 to 1.0. Spider-Man 2 ships 30%, 50% and 70%, so widen the range if the
  game's reaction demands need it.

### Slow motion on demand (D15)

- A remappable action. Offer toggle as well as hold; a hold-only slow-mo is a new motor barrier.
- Optional automatic trigger when a threat appears, as a separate choice.
- Uses the same `time_scale()` path. It clears on pause, death and scene change.

### Timing windows and input buffering (D16)

```
window = InputAssist.timing_window(base_seconds)   # base x input.timing_window_scale, simulation time
# real seconds = base x window_scale / time_scale
```

Example: a 0.2 s parry window with `input.timing_window_scale` 2.0 and `assist.game_speed` 0.5
lasts 0.4 s of simulation time and 0.8 s of real time. With game speed 1.0 it lasts 0.4 s real;
with window scale 1.0 and game speed 0.5 it lasts 0.4 s real.

- Use it for parry, dodge, perfect block, reload, takedown, rhythm and perfect-hit checks.
  Compare timestamps on the same clock as the window.
- Input buffer: accept an input slightly before it becomes valid and execute it when it does.
  Scale the buffer with the same setting. Make sure buffering cannot create double actions.
- Change only the acceptance window. Damage, enemy health and rewards stay as they are.
- Show success through more than one channel: visual, sound and optional haptic.

### Gameplay countdowns and timed choices (D29)

```
remaining -= dt_sim / policy.timer_scale()   # normal 1, extended 2, off INF (never expires)
```

- `off` removes countdowns that are not a race. Where time *is* the challenge (a race, a song), say so
  in the option description and point to game speed and other assists.
- Hiding the countdown display is not the same as extending it. Never leave a hidden timeout in a
  scripted event when timers are off.
- Freeze countdowns while settings, a codex, a tutorial card or narration is open. Reading must not
  consume the deadline.
- **Timed choices:** with timers off, the choice waits. Never treat slow selection as a choice. In
  Life is Strange 2, the reviewer describes a timeout read as a decision not to speak; that changes
  story agency, not just pace. Let narration finish reading every option before any clock starts.
- Keep **gameplay timers** and **UI deadlines** apart. XAG 116 covers UI and session deadlines and
  expressly excludes core gameplay timers. Gameplay timer options here are this skill's
  recommendation, not an XAG requirement.
- UI deadlines (respawn choice, ready check, tutorial card, toast): wait for the player by default.
  If a limit must exist, meet XAG 116 (numbers below).

### Pause anywhere (D27, L20)

- Single-player and local play: pause works in gameplay, dialogue, cutscenes, QTEs, minigames and
  timed sequences.
- **Freeze simulation and media together:** damage, AI, gameplay timers, voice, video, music sync and
  subtitles. A menu that opens while damage continues is not a pause.
- Pause and Skip are separate actions. Show a neutral pause overlay rather than a frozen intense
  frame where possible.
- Clear input latches on pause (see [input-and-motor](input-and-motor.md)).
- `system.pause_on_focus_loss` defaults on and stays with the device.
- Online play: say plainly that the match cannot pause. Offer a safe step-away instead: accessible
  idle warnings, a longer AFK window and a clean rejoin.

### Turn-based and pause-to-plan alternatives

- Real-time strategy: issue and queue orders while paused, with optional pause on chosen events.
- Tower defence: a manual "next wave" and building while paused.
- Party and asynchronous play: extended or no timers, or a mode that advances only when everyone
  has submitted.
- Separate animation speed from decision time.

### Rhythm (S072)

- Calibrate audio and visual latency separately with a tap test before applying any window scale.
  Store the offsets with the device, not the profile.
- Separate timing tolerance, song speed, note travel speed and lane count.
- Offer a steady visual or haptic metronome. Beat reinforcement helps some players and distracts
  others, so make it adjustable, and keep its flashes within
  [photosensitivity](photosensitivity.md) limits.
- A missed song must not block the story. Offer no-fail completion.
- Auto-action or single-key modes reduce button complexity but can still require beat timing. Do not
  describe them as removing timing.

### Network limits (S108, S130)

A client cannot change the authoritative match clock. Game speed, slow motion and timer changes need
an offline, private or agreed session rule. Show "Not available in online matches" next to the
option rather than ignoring it silently. Ask the developer which rule set to build; see
[combat-aiming-stealth](combat-aiming-stealth.md) for shared-simulation rules.

## Numbers that matter

| Number | Source and scope |
|--------|------------------|
| UI time limit adjustable to at least 10x default, or disabled, or extendable | [XAG 116](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/116). UI limits only, not core gameplay. |
| Extension warning at least 20 s before expiry, a simple extend action, at least 10 extensions | [XAG 116](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/116). Same scope. |
| Audio events shorter than 3 s need no pause control; live multiplayer need not pause the session | [XAG 105](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/105) |
| `assist.timers = extended` doubles countdowns; `input.timing_window_scale` 1x to 3x; `assist.game_speed` 0.5 to 1.0 | This skill's registry defaults. Tune per game. |

## Shipped examples

- Marvel's Spider-Man 2: game speed at 30%, 50% or 70%; wider dodge and parry windows.
  [Insomniac](https://support.insomniac.games/hc/en-us/articles/46730041467027-What-Accessibility-options-does-Marvel-s-Spider-Man-2-feature)
- Ratchet & Clank: Rift Apart: assignable shortcuts switch game speed on and off.
  [SIE](https://sonyinteractive.com/en/news/blog/inside-the-accessibility-features-of-ratchet-clank-rift-apart/)
- Star Wars Jedi: Survivor: slow mode plus automatic combat slowdown.
  [EA](https://www.ea.com/able/resources/star-wars/star-wars-jedi-survivor)
- Forza Horizon 5: game speed limited to offline play. A clear example of stating a network limit.
  [Xbox Wire](https://news.xbox.com/en-us/2021/11/04/gaming-for-everyone-the-accessibility-features-of-forza-horizon-5/)
- God of War Ragnarök: extended puzzle timing, independent from combat settings.
  [PlayStation](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/)
- Dead Cells (Update 29): easier parry among separate assists.
  [Patch notes](https://dead-cells.com/patchnotes/29)
- Life is Strange 2: counterexample. Timed dialogue read slow selection as silence.
  [Can I Play That?](https://caniplaythat.com/2020/03/07/teen-friendly-review-series-life-is-strange-2-and-captain-spirit/)
- Life is Strange: Double Exposure: longer and removed choice timers. Preview-build tour by a
  disclosed consultant. [Access-Ability](https://access-ability.uk/2024/10/24/touring-accessibility-in-life-is-strange-double-exposure/)
- The Jackbox Party Pack 10: extended and no-timer settings, per game.
  [Jackbox](https://www.jackboxgames.com/blog/accessibility-features-in-the-jackbox-party-pack-10)
- Final Fantasy XVI: pause during cutscenes. Pre-release hands-on description.
  [PlayStation Blog](https://blog.playstation.com/2023/02/28/hands-on-with-final-fantasy-xvi-new-gameplay-details/)
- Hi-Fi RUSH: Auto-Action and single-key rhythm controls; still expects beat-timed attacks.
  [KRAFTON](https://support.krafton.com/hc/en-us/articles/52371575035801-Hi-Fi-RUSH-Accessibility-Settings)

## Verify

- [ ] The clock inventory lists every timer, and each one uses the documented clock.
- [ ] At the slowest game speed: menus, pause, narration and subtitles run at normal speed; captions
      stay in sync with audio; no collision or physics breaks on moving platforms.
- [ ] Change game speed during a timed sequence. No progression or audio regression.
- [ ] A boundary input that fails at 1x succeeds at the configured window scale, and only once.
- [ ] Window scale 2.0 + game speed 0.5: a 0.2 s window lasts 0.8 s of real time, not 1.6 s or
      0.4 s. Each multiplier is applied exactly once. Subtitles stay at normal speed.
- [ ] `assist.timers = off`: every non-race countdown, including scripted ones, never expires.
- [ ] Timed choices wait with timers off. Narration reads all options before any clock starts.
- [ ] Every UI deadline waits, or meets the XAG 116 numbers.
- [ ] Pause during a cutscene, a QTE and a countdown. Media, damage and timers freeze; each resumes
      coherently with no hidden progress.
- [ ] Alt-tab pauses when `system.pause_on_focus_loss` is on. The value persists per device.
- [ ] Online: speed and timer options show their limit instead of failing silently.
- [ ] Settings persist after restart and after loading an old save.

## Common mistakes

- Slowing the game by capping FPS → scale simulation delta.
- Game speed slows subtitles and narration → keep presentation and UI clocks unscaled.
- Hiding the timer and calling it "timers off" → change the deadline, not the display.
- A timed choice defaults to silence → make choices wait when timers are off.
- Citing XAG 116 for a race timer → XAG 116 covers UI deadlines; gameplay timers are this
  skill's recommendation.
- "Pause" opens a menu while enemies still attack → freeze simulation and media together.
- Cutscenes cannot pause → pause every presentation layer the player can be in.
