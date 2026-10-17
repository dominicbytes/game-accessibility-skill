# Input and motor

**Load when:** the game takes any player input (remapping is Baseline), and especially S014 QTEs,
S015 mashing, S016 sustained holds, S017 tap-versus-hold on one button, S018 long repetition,
S019 chords, S020 two sticks, S024 precise analog input, S025 motion inputs, S027 stick clicks or
trigger force, S028 repeat or debounce problems, S030 many memorised inputs, S031 prompts, S032
menus operated differently from play, S042 grabs and struggles, S156 custom hardware.
**Skip when:** the project has no interactive input at all. Otherwise skip only the sections whose
signals are confirmed absent.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| M01 | Full remapping: gameplay, menus, minigames, vehicles, pause/back, confirm | `game.input.bindings` | Designer layout | B | M, S |
| M02 | More than one binding per action | `game.input.bindings` | Designer bindings | C | M, S |
| M03 | Split overloaded actions (tap and hold on one button) into separate actions | `game.input.bindings` | Designer | C | M, C |
| M04 | Per-device binding profiles | `game.input.bindings` | Designer | C | M, S |
| M05 | Left-hand and right-hand layout presets | `game.input.layout_preset` | Off | C | M |
| M06 | Swap stick functions | `game.input.swap_sticks` | Off | C | M |
| M07 | Independent X and Y inversion per stick and context | `input.invert_y` | Off | C | M, O |
| M08 | Independent sensitivity per analog control | `input.look_sensitivity` | 1.0 | C | M |
| M09 | Inner and outer deadzones | `game.input.deadzone` | Designer | C | M |
| M10 | Response curve | `game.input.response_curve` | Designer | C | M |
| M11 | Acceleration and smoothing off | `game.input.acceleration` | Designer | C | M, O |
| M12 | Trigger actuation threshold | `game.input.trigger_threshold` | Designer | C | M |
| M13 | Toggle instead of sustained hold | `input.hold_mode` | `hold` | C | M, S |
| M14 | Hold instead of repeated presses | `input.mash_mode` = `hold` | `mash` | C | M |
| M15 | Single press or automatic repeat instead of mashing | `input.mash_mode` = `single` | `mash` | C | M |
| M16 | QTE automatic completion | `input.qte_mode` = `auto` | `normal` | C | M, C, T |
| M17 | Relaxed QTE timing | `input.qte_mode` = `slow` | `normal` | C | M, C |
| M18 | Sequential alternative to chords | `game.input.sticky_modifiers` | Off | C | M |
| M19 | Macros and combo shortcuts | `game.input.macros` | Off | C | M, C |
| M20 | Single-stick play | `game.input.single_stick` | Off | C | M |
| M21 | Keyboard-only play and UI | none (capability) | On when PC | C | M, V |
| M22 | Mouse-only play and UI | none (capability) | Off | C | M |
| M23 | Digital-only alternative to analog input | none (capability) | Off | C | M, V |
| M24–M32 | Touch, gesture and body-tracking input | see [touch-and-mobile](touch-and-mobile.md), [vr-and-xr](vr-and-xr.md) | Designer | C | M |
| M33 | Auto-run / auto-move | `game.input.auto_run` | Off | C | M |
| M34 | Auto-sprint | `game.input.auto_sprint` | Off | C | M |
| M35 | Automatic item pickup | `game.input.auto_pickup` | Off | C | M, C |
| M36 | Automatic vault, climb and interact | `game.input.auto_traverse` | Off | C | M |
| M37 | Menu key-repeat delay and rate | `game.input.menu_repeat` | Designer | C | M |
| M38 | Post-acceptance debounce | `game.input.debounce` | Off | C | M |
| M39 | Longer double-press and hold recognition windows | `input.timing_window_scale` | 1.0 | C | M |
| M40 | Cursor snapping to targets | `game.input.cursor_snap` | Off | C | M, V |
| M41 | Stationary menu targets | none (design rule) | On | B | M, V |
| M42 | Cancel or release-to-activate in menus | none (design rule) | On | B | M |
| L05 | All supported devices reach menus and play | none (capability) | On | C | M, S |
| L06 | Mixed devices at the same time | none (capability) | On where feasible | C | M |
| L07 | Shared control (copilot) | `game.input.copilot` | Off | C | M |
| L10 | Switch scanning | `game.input.switch_scan` | Off | A | M |
| L11 | Gaze and dwell input | `game.input.dwell_time` | Off | A | M |

`game.*` keys are suggested project-specific names from the [option catalogue](../option-catalog.md);
they are not in the shared registry. Store bindings in the documented, human-readable config file
([core-policy](../core-policy.md) section 8), never in story saves.

## How to implement

### Action layer and remapping (M01–M06, M41, M42, L05, L06)

- Gameplay asks the action layer and `InputAssist` for actions (`jump`, `interact`). It never reads
  physical keys. Adaptive controllers, Steam Input, remapping tools, voice tools and test bots all
  enter through the same actions ([architecture](../architecture.md) section 3).
- The remap screen covers **every context**: gameplay, menus, pause, confirm/back, inventory,
  vehicles, minigames, tutorials, photo mode, text-entry dialogs and rare late-game screens. A
  hard-coded menu key blocks players who remapped everything else
  ([Martinez, CIPT](https://caniplaythat.com/2019/07/04/basic-accessibility-options-for-mobility/)).
- Conflicts: show the clash and let the player swap, duplicate or cancel. Never silently unbind.
- Recovery: if the player unbinds confirm, back or pause, keep a reset route. Use a reset key read
  from the config file, or a timed "Keep these bindings?" revert.
- **Prompts must read live bindings.** Generate every glyph, tutorial line, loading tip, control
  reminder and narrated hint from the action's current binding at display time. Refresh on remap and
  on device change. Stale prompts come from strings in tutorial data, glyphs baked into textures,
  and prompts cached when a scene loads. If automatic device switching makes glyphs flicker, offer a
  fixed glyph set.
- Split overloaded actions (S017) so tap-interact and hold-for-menu can live on different inputs.
- Mixed devices (L06): merge held digital states and release only when every source releases. For
  analog conflicts, pick one rule (strongest magnitude or last active device). Never add the two.
- Menus: targets do not move while the player aims at them (M41). Activation happens on release and
  can be cancelled by moving off (M42).

### Holds, toggles, mashing and repetition (M13–M15, M33–M36)

```
InputAssist.is_held(action)                       # hold_mode = toggle: first press latches, second releases
InputAssist.mash_progress(action, presses_needed) # mash | hold (fills at authored rate) | single (one press)
```

- Choose per action where it matters. Sprint and aim may need different modes, so add per-action
  overrides on top of the global `input.hold_mode`.
- **Latch rules:** a toggled action clears on pause, death, scene change, cutscene start, focus loss
  and device disconnect. Show the latched state visibly and say it through narration when it is on.
- **Never offer a long hold as the only alternative to mashing.** Some players cannot hold. The
  `single` mode and QTE `auto` must reach every repeated-press site.
- Repetition outside combat counts: harvesting, crafting, buying, opening containers, menu
  scrolling. Route all of it through `mash_progress` or an auto-repeat action (M15): one press
  starts, one press stops, at the authored cadence.
- Grabs and struggles (S042) use `mash_progress` or `qte`, never their own polling loop.
- Auto-run, auto-sprint, auto-pickup and auto-interact stop on any movement or cancel input. Filter
  auto-pickup so it never fills the inventory with junk or takes quest-critical choices. Auto-vault
  must not commit the player to an irreversible drop.

### QTEs (M16, M17)

```
InputAssist.qte(prompt) # normal: authored window
                        # slow: window x3 or more, or wait until pressed when nothing else moves
                        # auto: short readable beat, then the normal success branch
```

- Cover **every QTE type**: gameplay prompts, boss finishers, cinematic QTEs, escape grabs,
  dialogue prompts, co-op prompts and minigames. One unhandled cinematic QTE can block the ending.
- `auto` runs the same success transition as a perfect input: animation, rewards, flags, music.
  Remove the failure timeout too. A hidden timer that can still fire is a race bug.
- If a QTE failure branch is authored story, document what `auto` chooses.
- Audit: search for every timer, prompt widget and input poll in cutscene and sequence code. Any QTE
  that does not call `InputAssist.qte` is unfinished.

### Chords, one-handed and single-device paths (M18–M23)

- Sequential chords: the first input latches the modifier, the HUD shows the current stage, and the
  next input completes the chord. Allow a grace window instead of exact simultaneity.
- One-handed layouts: count every required input over the whole game, including stick clicks,
  D-pad, triggers and menu shortcuts. Turning the scheme on must not need the hand it replaces.
- Single-stick (S020): pair automatic camera centring or a move/look mode switch with interaction
  lock-on. Remapping alone does not remove simultaneous stick use. Check the camera-assist conflict
  with reduced motion in [camera-and-motion](camera-and-motion.md).
- Keyboard-only, mouse-only and digital-only: every required action, including launcher, map,
  inventory and quit. Give analog actions digital steps or hold-to-ramp equivalents.
- Macros: bounded to game actions. State whether they are allowed in competitive modes.

### Sensitivity, deadzones and curves (M07–M12)

- Each analog control gets its own sensitivity and inversion: look, aim, mouse, triggers, vehicles.
- Ranges must be wide. XAG 107 asks for at least 50% below and 50% above default per analog control.
- Show a live preview of usable range with no failure state. Let players adjust with keyboard or
  controller, not only with the device being adjusted.
- Never hide forced acceleration behind "sensitivity". Expose smoothing and acceleration off.
- Haptics and adaptive-trigger resistance belong to
  [sensory-load-and-guidance](sensory-load-and-guidance.md).

### Debounce, repeat and recognition windows (M37–M39)

- Debounce is a player option, off by default. Never impose a global delay on everyone.
- Menu key-repeat delay and rate are separate from in-game action rates.
- Double-press and hold recognition windows scale with `input.timing_window_scale`. Check they do not
  steal single-tap actions.

### Adaptive hardware, copilot and Steam Input (L05–L07)

- **Hardware is not a substitute for design.** An adaptive controller still cannot hold two sticks
  and a chord at once
  ([Martinez, controller review](https://www.gameaccessibilitynexus.com/blog/2024/04/24/playstation-access-controller-review/)).
  Reduce simultaneous and sustained demands in the game.
- Copilot: state whether the game has its own shared-control mode or relies on a platform feature.
  Define disconnect, active prompts and conflicting input. Never require a second person for
  setup. Exact current Xbox Copilot limits are UNVERIFIED
  ([Xbox support](https://support.xbox.com/en-US/help/account-profile/accessibility/copilot)).
- Steam Input: read the configured action origins for glyphs
  ([Steamworks](https://partner.steamgames.com/doc/features/steam_controller/getting_started_for_devs)).
  Do not reject emulated input or block third-party remappers without a stated reason.

### Switch scanning and dwell (L10, L11, Advanced)

Record a scope decision in `ACCESSIBILITY.md`. If in scope: an ordered set of selectable actions,
adjustable scan interval, pause, back and manual stepping. For dwell: adjustable delay, visible
progress, cancel on leave and no repeat activation. Remove or extend time limits rather than make a
scan cycle race a timer. OS switch control needs real UI items, not canvas pixels
([Apple Switch Control](https://support.apple.com/en-ie/119835)).

## Numbers that matter

| Number | Source and scope |
|--------|------------------|
| At least 50% below and above default sensitivity, per analog control | [XAG 107](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/107) |
| 0.5 s post-acceptance cooldown, as an option | [GAG](https://gameaccessibilityguidelines.com/include-a-cool-down-period-post-acceptance-delay-of-0-5-seconds-between-inputs/). Not a mandatory delay for all players. |
| QTE `slow` window at least 3x the authored window | This skill's contract, [architecture](../architecture.md) section 3. |

## Shipped examples

- Uncharted 4: hold instead of mash, added after consulting a disabled player.
  [GameSpot](https://www.gamespot.com/articles/uncharted-4s-accessibility-options-inspired-by-inp/1100-6439983/)
- Alan Wake 2 (Anniversary Update 1.2.2): separate hold/toggle choices and automatic or single-tap
  repeated input. [Remedy](https://www.alanwake.com/story/the-anniversary-update-explained/),
  [retrospective](https://www.alanwake.com/story/gameplay-assist-alan-wake-2/)
- Marvel's Spider-Man Remastered: QTE auto-completion and tap-to-hold alternatives. Edition-specific.
  [Insomniac](https://support.insomniac.games/hc/en-us/articles/46670280641299-What-Accessibility-options-does-Marvel-s-Spider-Man-Remastered-have)
- Assassin's Creed Valhalla: single press, hold or repeated-press QTE choices and hold duration;
  only some boss QTEs bypassed. [Ubisoft](https://news.ubisoft.com/en-us/article/5Rys9UeXZh9bAjnHBZN4pC/assassins-creed-valhalla-bringing-accessibility-to-a-viking-epic)
- Mortal Kombat 1: Test Your Might mash assistance.
  [WB Games](https://mortalkombatgamessupport.wbgames.com/hc/en-us/articles/19435787154963-Mortal-Kombat-1-Accessibility-Features-A11Y)
- Sea of Thieves: single-stick navigation with camera centring and interaction-prompt locking.
  [Release notes 2.0.18](https://www.seaofthieves.com/release-notes/2.0.18)
- The Last of Us Part I: left-only and right-only control schemes. PlayStation 2022 list.
  [PlayStation Blog](https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/)
- Battlefield 2042: remapping across on-foot, vehicle, gunner and pilot contexts.
  [EA](https://www.ea.com/able/resources/battlefield-2042/pc)
- Call of Duty: Modern Warfare III: Low Motor Strain preset and dead-zone tuning.
  [Blizzard](https://news.blizzard.com/en-gb/article/24043597/explore-the-accessibility-options-that-launched-with-mwiii)
- Horizon Forbidden West: second-controller copilot. PS5/PS4 article.
  [PlayStation Blog](https://blog.playstation.com/2022/02/10/accessibility-features-in-horizon-forbidden-west/)
- Celeste: grab as toggle and directional bindings in the changelog.
  [Changelog](https://www.celestegame.com/changelog.html)

## Verify

- [ ] Launch, configure, play, save and quit without the default confirm or pause key.
- [ ] Remap every gameplay and menu action. Executed actions **and** every prompt, tutorial,
      reminder and narrated hint show the new binding, including a late-game tutorial.
- [ ] Bindings persist after restart and after loading an old save. Reset works per device.
- [ ] Unbind confirm: the recovery route still restores a usable layout.
- [ ] Toggle sprint/aim, then pause, die, change scene, alt-tab and unplug the device. Nothing stays
      latched.
- [ ] Every mash site, including struggles and noncombat repetition, completes with `hold` and with
      `single`.
- [ ] Reach the ending with `input.qte_mode = auto`. Every QTE type, including cinematic and boss,
      succeeds; no failure timer fires.
- [ ] `slow` QTEs give at least 3x the authored window.
- [ ] Play with one supported device only, including menus and minigames.
- [ ] Mouse plus controller together: no glyph flicker interrupts control.
- [ ] Sensitivity reaches at least 0.5x and 1.5x default on every analog control.
- [ ] Combination: toggle hold + remapped bindings + QTE auto + controller disconnect.

## Common mistakes

- Gameplay remaps, menus and minigames don't → remap every context, including pause and confirm.
- Tutorial text says "Press E" after remapping → build prompts from live bindings.
- Mash replaced only by a long hold → also offer `single` and QTE `auto`.
- Toggle latch survives pause or death → clear latches on every listed event.
- QTE auto handles gameplay QTEs only → audit cinematic, boss, dialogue and co-op prompts.
- "Supports the Xbox Adaptive Controller" used as the motor plan → reduce simultaneous and
  sustained inputs in the design.
- Remapping blocked to stop third-party tools → let them in through the action layer.
