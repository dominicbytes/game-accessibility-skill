# Traversal and platforming

**Load when:** S043 jumps, gaps or platforms; S044 climbing, ropes or ledge hanging; S045 chases,
escapes or forced autoscroll. Also load for a single mandatory jump section in an otherwise
different game. **Skip when:** movement has no gaps, falls, climbing, pursuit or moving hazards
(for example grid movement on a static board).

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| D20 | Jump forgiveness: coyote time and jump buffer scale | `game.assist.landing` (not `input.timing_window_scale`) | 1.0 | C | M, C |
| D20 | Ledge guard and automatic ledge grab | `game.assist.landing` | Off | C | M, V |
| D20 | Float or glide mode | `game.assist.landing` | Off | C | M |
| D21 | Extra or infinite jumps | `game.assist.extra_jumps` | Off | C | M, T |
| D07 | Safe falls: return to the last safe spot | `assist.no_fall_death` | false | C | M, T, O |
| M13 | Climbing grip as toggle | `input.hold_mode` | `hold` | C | M |
| D12 | Climbing without stamina | `assist.infinite_stamina` | false | C | M, T |
| M36 | Automatic climb, vault and ledge transitions | `game.input.auto_traverse` | Off | C | M |
| D14 | Slower hazards, moving platforms and autoscroll | `assist.game_speed` (whole game) or a hazard-only key | 1.0 | C | M, C |
| D23 | Chase assist or skip; platforming section skip | `assist.skip_available` | true | C | M, T, C |
| V28 | Platform and edge outlines, landing marker | `visual.outline_hazards` (umbrella `visual.high_contrast`) | `follow` | C | V, C |
| R15 | Distinct audio for edges, landing and ledges | none (design) | Designer | C | V |

Combat damage and invincibility: [difficulty-and-assists](difficulty-and-assists.md). Camera
control, shake and FOV: [camera-and-motion](camera-and-motion.md). Fear of heights (F05):
[content-and-phobias](content-and-phobias.md).

## How to implement

### Jump forgiveness (D20)

- **Coyote time:** the player can still jump for a short time after walking off a ledge.
- **Jump buffer:** a jump pressed just before landing fires on landing.
- Scale both with one assist multiplier, measured in simulation time and applied once. Real
  duration = base × multiplier / time_scale, so a player who also lowers game speed gets both
  effects, by choice. The bug is applying either multiplier twice (for example, also scaling by
  `time_scale`). Keep it separate from `input.timing_window_scale`; a player may want wider parry
  windows but authored jumps, or the reverse.
- Base values are a design choice. No published standard sets them; tune and record them in
  `ACCESSIBILITY.md`.
- Dash and air-control assists follow the same pattern: wider tolerance, same destination choice.

### Ledge guard, auto-grab and float (D20)

- Ledge guard stops the player at an edge unless they jump or give a deliberate drop input. Keep a
  way to drop on purpose.
- Auto-grab catches ledges within reach during a fall.
- Float mode slows descent or lets the player hover. Check that it cannot reach unintended areas,
  or that those areas have a way back.

### Extra and infinite jumps (D21)

- Infinite jumps can take the player out of bounds or past triggers. Add an "unstuck" action that
  returns to the last safe spot, and make sure skipped triggers still fire.
- Show the jump count only if it matters.

### Safe falls (D07)

```
if policy.should_fail("fall") == false:
    respawn_at(last_safe_position)   # no death, no lost progress
```

- Record a safe position only when the player stands on stable, non-moving, non-hazard ground
  outside a chase or timed section.
- Fall **damage** and fall **death** are different. State which one the option removes.
- `assist.no_fall_death` is independent of `assist.damage_taken`. Combat damage Off with falls On,
  and the reverse, both work.
- After respawn, restore moving platforms and hazards so the player does not land in danger.

### Climbing (S044)

- Grip uses `InputAssist.is_held("grip")`, so `input.hold_mode = toggle` turns a held grip into a
  latch. The latch clears on pause, fall and scene change.
- Climbing stamina calls `policy.consume("stamina", amount)`. Infinite stamina never drains.
- Automatic attachment and single-input climbing reduce alternating presses.
- Test infinite stamina + toggle sprint + traversal assists together.

### Hazards, moving platforms, autoscroll and chases (S045)

- Slower hazards: scale hazard, platform and autoscroll motion by a hazard-time multiplier, or rely
  on `assist.game_speed` for the whole simulation. Say which in the option text.
- Chase assist: widen the catch-up distance, scale pursuer speed, and replace scripted instant
  failure with `should_fail("caught")`.
- Chase and platforming skip use `policy.request_skip(section_id)`. Place the player at the exit
  with the abilities, flags and objective state they would have earned.

### Landing, edge and depth cues

- Lethal edges, spikes and pits use the **hazard** channel, not guidance. Turning guidance off never
  hides them (see [sensory-load-and-guidance](sensory-load-and-guidance.md)).
- `visual.outline_hazards` (following the `visual.high_contrast` umbrella unless set) adds platform
  and edge outlines through `ColorTokens`, with a shape or pattern as well as colour.
- 3D depth: a drop shadow or landing marker directly below the player, and an optional jump arc
  preview. These show where the player will land without changing the jump.
- Audio: a distinct sound for nearing an edge, for a ledge in reach and for a safe landing. Send
  each through `CaptionBus` so captions and the sound visualiser show them too.

### Framing the menu (Celeste lesson)

Celeste's revised Assist Mode text invites players to "modify the game's rules to fit your specific
needs" (as reproduced by the Celeste wiki). Earlier text recommended a first attempt without assists;
that line was removed. Borrow the revised framing. **Do not copy its assist marker on the save
card**: core policy forbids marking saves.

## Shipped examples

- Celeste: Assist Mode with game speed, invincibility, infinite stamina and Dash Assist; revised
  wording. [Changelog](https://www.celestegame.com/changelog.html),
  [wiki text](https://celeste.ink/wiki/Assist_Mode)
- The Forest Cathedral: Float Mode and spike removal; the developer's case study names analog-only
  platforming and rapid presses as barriers found in review.
  [Whitethorn](https://whitethorngames.com/theforestcathedral/accessibility),
  [Game Developer](https://www.gamedeveloper.com/design/deep-dive-designing-for-accessibility-in-the-forest-cathedral)
- Jusant (patch 1.04, November 2023): simplified climbing and no-stamina options.
  [Don't Nod](https://support.dont-nod.com/hc/en-gb/articles/14957643453853-Jusant-Patch-1-04-The-Accessibility-Update-November-2023)
- Psychonauts 2: No Fall Damage, separate from invincibility.
  [Double Fine](https://www.doublefine.com/games/support/psychonauts-2/accessibility?platform=all)
- Prince of Persia: The Lost Crown: platforming skips and optional guided routes.
  [Ubisoft](https://news.ubisoft.com/en-us/article/5nGZiBSFtcEzFd93QlTotS/prince-of-persia-the-lost-crown-accessibility-spotlight)
- Star Wars Jedi: Survivor: fall-damage removal.
  [EA](https://www.ea.com/able/resources/star-wars/star-wars-jedi-survivor)
- Marvel's Spider-Man 2: Chase Assist. A full chase skip is not claimed.
  [Insomniac](https://support.insomniac.games/hc/en-us/articles/46730041467027-What-Accessibility-options-does-Marvel-s-Spider-Man-2-feature)
- Super Mario Bros. Wonder: damage-free characters exist but lack power-ups; assistance is tied to
  character choice. A trade-off not to copy.
  [Nintendo](https://www.nintendo.com/us/store/products/super-mario-bros-wonder-switch/)

## Verify

- [ ] Moving platforms, ladders, air dashes, wall jumps, chases and puzzle switches all complete
      with each traversal assist. No hidden timing check remains.
- [ ] With coyote and buffer scale at maximum, a late jump and an early jump both succeed, once.
- [ ] Change game speed during a moving-platform section. No collision errors.
- [ ] Coyote multiplier 2.0 + game speed 0.5: a 0.1 s coyote time lasts 0.4 s of real time; each
      multiplier is applied once.
- [ ] Safe falls: fall into every pit type. The player returns to a safe spot, never inside a
      hazard, a closing door or a chase, and loses no progress.
- [ ] Invincibility + pit recovery + checkpoint reload: no death loop.
- [ ] Combat damage Off + safe falls Off: pits still kill, as described.
- [ ] Infinite jumps: try to leave the level. The unstuck action recovers, and triggers still fire.
- [ ] Grip toggle + infinite stamina: finish the longest climb; pause and fall clear the latch.
- [ ] Skip a chase and a platforming section, then save and reload. Abilities and objective match
      a normal clear.
- [ ] Guidance off: every lethal edge still shows its hazard cue.
- [ ] Settings persist after restart and after loading an old save.

## Common mistakes

- Invincibility used as the traversal assist → it does not cross a gap; add jump and fall assists.
- Safe spot recorded mid-air or on a moving platform → record only stable, safe ground.
- One timing slider for jumps and parries → keep jump forgiveness separate.
- Ledge cue removed with guidance paint → hazards have their own channel.
- Chase skip drops the player before the ability unlock → grant the full completion state.
- Copying a save-card assist marker → never mark the save.
