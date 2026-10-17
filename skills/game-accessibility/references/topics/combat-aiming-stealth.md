# Combat, aiming and stealth

**Load when:** S033 precise aiming, S034 sway or recoil, S035 reload and ammo handling, S036 parry
or dodge, S037 off-screen threats, S038 combos or ability rotations, S039 many enemies or
projectiles, S042 grabs, S046 stealth detection, S047 takedown timing, S048 scripted defeats, S049
companion micromanagement, S050 an action minigame in a slower game. S040 stamina and S041 damage
channels route to [difficulty-and-assists](difficulty-and-assists.md); S043–S045 to
[traversal-and-platforming](traversal-and-platforming.md).
**Skip when:** there is no aiming, real-time combat or detection. Turn-based command menus without
aiming belong in [structured-ui-and-strategy](structured-ui-and-strategy.md).

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| D17 | Aim assist: slowdown near targets, gentle pull, stabilised reticle, reduced recoil and sway | `input.aim_assist` (slowdown), `input.aim_pull`, `input.aim_stabilization` | 0.0 each | C | M, V |
| D18 | Target lock, snap and target cycling with easy release | `input.target_lock` | false | C | M, V |
| M15 | Auto-fire and auto-repeat attacks; auto-reload when safe | `input.mash_mode` or a per-weapon auto-fire key | Off | C | M |
| D16 | Wider parry, dodge and takedown windows | `input.timing_window_scale` | 1.0 | C | M, C |
| H34 | Off-screen threat and sound direction indicators | `audio.sound_visualizer` | false | C | H, V |
| V26 | Enemy and ally outlines | `visual.outline_enemies`, `visual.outline_allies` (umbrella `visual.high_contrast`) | `follow` | C | V |
| M19 | Combo simplification, auto-combo, ability shortcuts | `game.input.macros` | Off | C | M, C |
| D06 | Enemy health and outgoing damage | `game.assist.damage_dealt` | 1.0 | C | M, T |
| D09 | Enemy aggression, simultaneous attackers, projectile speed, detection speed | `assist.enemy_aggression` | 1.0 | C | T, M, C |
| D10 | Passive enemies | `assist.enemy_aggression` = 0 | 1.0 | C | T, F |
| V25 | Detection state shown by shape, label and sound, never one channel | none (design rule) | On | B | V, H, C, N |
| D08 | Stealth failure off: detection starts a recoverable search, not a fail | game-specific, read via `should_fail("detected")` | Fail | C | T, C |
| D23 | Skip a mandatory stealth, chase or combat section | `assist.skip_available` | true | C | M, T, C |

Grab and struggle inputs use `input.mash_mode` and `input.qte_mode` from
[input-and-motor](input-and-motor.md). Damage scaling and invincibility live in
[difficulty-and-assists](difficulty-and-assists.md). Reticle size and colour (V21, V22) live in
[color-and-clarity](color-and-clarity.md).

## How to implement

### Aim assistance (D17, D18)

Split assistance into parts the player can feel and set separately. Each part has its own key and
its own 0–1 strength, with 0 meaning off; no part switches on because another slider passed a
threshold. A player can use pull with no slowdown, stabilisation alone, or lock-on with no pull.

| Part | What it does | Key |
|------|--------------|-----|
| Slowdown (friction) | Reticle moves slower over a target | `input.aim_assist` (0–1) |
| Pull (magnetism) | Reticle drifts toward a nearby target | `input.aim_pull` (0–1) |
| Stabilisation | Smooths small input jitter and reduces gameplay recoil and sway | `input.aim_stabilization` (0–1) |
| Lock-on | Holds a target until released; next/previous target; release on any deliberate look input | `input.target_lock` (on/off) |

The "Reduced physical effort" preset sets several of these together as an editable patch; each key
stays adjustable afterward.

- Apply to **every mandatory target**: weak points, switches, puzzle targets, thrown items. Help that
  only works on enemies leaves a tiny puzzle target blocking progress.
- Keep target choice. Lock-on picks the nearest threat, but cycling and release are always there.
- Cosmetic weapon sway (O03) is a comfort setting in [camera-and-motion](camera-and-motion.md).
  Gameplay recoil and sway are aim behaviour. Keep the two separate.
- Lock-on that turns the camera can conflict with reduced motion. Offer lock-on without camera
  turning, using an on-screen target marker instead.

### Auto-fire and reloads (M15, S035)

- One press starts auto-fire, one press stops it. It respects fire rate, heat and ammo.
- Auto-reload triggers when the magazine is empty and no other action is in progress.
- Infinite ammo is `assist.infinite_ammo` in [difficulty-and-assists](difficulty-and-assists.md).

### Parry, dodge and perfect block (D16, S036)

- Use `InputAssist.timing_window(base)` at the check. Apply the window multiplier once, in
  simulation time; deliberate game slowdown (`assist.game_speed`) also lengthens real time. The
  window lasts `real = base × window_scale / time_scale` seconds. Never scale it again by
  `time_scale` or measure it in real time (see [timing-and-speed](timing-and-speed.md)).
- Telegraph every attack through at least two channels: a shape or animation cue, a sound sent
  through `CaptionBus`, and optional haptics. A flash-only telegraph also needs
  [photosensitivity](photosensitivity.md) review.
- A practice space that costs nothing helps players learn the timing.

### Off-screen threats (H34, S037)

- Emit a `CaptionEvent` with `source_position` for each attack that starts outside the view.
  The visualiser shows direction; captions say what it is ("[Arrow, behind]").
- Let players set indicator size and density. Rate-limit repeats with a merge key.
- Do not reveal more than a hearing player would get, unless the designer chooses to.

### Combos and ability rotations (M19, S038)

- Auto-combo: repeated single presses or one hold advance the chain at the authored cadence.
- Shortcuts and macros map one input to a bounded sequence of game actions.
- Document which moves a simplified scheme cannot perform. Prefer a menu option over access tied
  to equipment slots or characters.
- Fewer buttons does not mean fewer presses. A one-button rotation that needs constant pressing is
  still a repetition barrier; pair it with `input.mash_mode`.

### Enemy pressure (D06, D09, D10, S039)

- Read multipliers at decision time: attack frequency, maximum simultaneous attackers, projectile
  speed, spawn count. Changes apply at the next decision, mid-fight.
- Passive enemies keep their quest and loot triggers.
- Bosses: decide what passive means for each phase and write it in the option description.

### Grabs and scripted defeats (S042, S048)

- Grabs use `mash_progress` or `qte`. With combat damage Off, a grab still releases after a
  defined time.
- A scripted defeat continues the script whatever the assist state. See
  [difficulty-and-assists](difficulty-and-assists.md).

### Stealth (S046, S047)

- **Four states, each multimodal:** unaware, suspicious, searching and detected. Each gets a distinct
  icon shape, a short text label, a distinct sound sent through `CaptionBus`, and optional haptics.
  Never colour only, never sound only. Show which enemy and which direction.
- Detection speed follows `policy.enemy_aggression()`. Add a grace period after discovery.
- **Chase recovery:** breaking line of sight returns enemies to searching after a timeout that scales
  with the same multiplier. Mandatory stealth sections need a fail-off option and a skip.
- Takedowns: the execution window uses `timing_window()`. A guaranteed takedown is a separate
  choice from detection control.
- Concealment help (for example staying hidden while prone) does not replace knowing a guard's state.

### Companions (S049)

Offer simpler orders and companion survivability. A companion's death that fails the mission routes
through `should_fail`.

### PvP and shared simulation

- Personal presentation and input always work in every mode: remapping, captions, colours,
  indicators, reticle, camera comfort.
- **Do not silently weaken the shared simulation.** Aim assist strength, game speed, damage and
  timing windows change outcomes for other players. Ask the developer to choose: PvE and private
  matches only, an agreed lobby rule open to everyone, or a published ruleset. Describe rules, never
  who needs them. Never announce an individual's assists.
- Record the decision in `ACCESSIBILITY.md` and show the limit beside the option.

## Shipped examples

- Gears 5: Target Lock in documented PvE modes and difficulties. Not a multiplayer promise.
  [Gears of War](https://www.gearsofwar.com/en-gb/games/gears-5/accessibility/)
- Control: Aim Snap among separate Assist Mode options.
  [Update notes](https://legacy.controlgame.com/de/control-august-update-notes/)
- Dead Space (2023): aim friction and magnetism controls.
  [EA](https://www.ea.com/ea-play/news/accessibility-and-player-choice-in-dead-space)
- Marvel's Spider-Man 2: wider dodge and parry windows.
  [Insomniac](https://support.insomniac.games/hc/en-us/articles/46730041467027-What-Accessibility-options-does-Marvel-s-Spider-Man-2-feature)
- Dead Cells (Update 29): auto-hit and easier parry. [Patch notes](https://dead-cells.com/patchnotes/29)
- Destiny 2: Full Auto Firing. Dated announcement, not a current menu map.
  [Bungie](https://www.bungie.net/7/en/News/Article/destiny-2-accessibility-set)
- Final Fantasy XVI: one-button combos and auto evasion, tied to equipment slots.
  [Square Enix](https://www.square-enix-games.com/en_US/news/final-fantasy-xvi-everything-you-need-to-know)
- Street Fighter 6: Modern and Dynamic controls; Dynamic is offline-only.
  [Capcom](https://www.capcom-support.com/hc/en-us/articles/7122030981660-What-are-the-control-types-in-Street-Fighter-6)
- World of Warcraft: Single-Button Assistant; feedback that repeated pressing is tiring.
  [Blizzard](https://news.blizzard.com/en-us/article/24205457/legacy-of-arathor-get-an-assist-with-new-ui-options),
  [PC Gamer](https://www.pcgamer.com/games/world-of-warcraft/world-of-warcraft-single-button-rotation-assist-verdict/)
- Fortnite: Visualize Sound Effects for "some sounds".
  [Epic](https://www.epicgames.com/help/c-34254770/c-33726977/a25071786?lang=en-US)
- Assassin's Creed Valhalla: separate stealth difficulty and guaranteed assassination.
  [Ubisoft](https://news.ubisoft.com/en-us/article/5Rys9UeXZh9bAjnHBZN4pC/assassins-creed-valhalla-bringing-accessibility-to-a-viking-epic)
- Assassin's Creed Mirage: guaranteed pickpocketing removes its QTE.
  [Ubisoft Help](https://www.ubisoft.com/en-us/help/assassins-creed-mirage/gameplay/article/accessibility-features-in-assassins-creed-mirage/000105449)
- The Last of Us Part II: stay hidden while prone. Concealment, not alert-state information.
  [PlayStation Blog](https://blog.playstation.com/2020/06/09/the-last-of-us-part-ii-accessibility-features-detailed/)
- Elden Ring: a launch-period reviewer did not realise enemies had detected them and could not tell
  sound direction. [Can I Play That?](https://caniplaythat.com/2022/02/27/elden-ring-lacking-accessibility-unintentional-barriers-and-land-jellyfish/)

## Verify

- [ ] Clear a mandatory fight with slowdown, pull and stabilisation at 1.0 using one-handed input.
      Lock-on cycles and releases. Puzzle and weak-point targets accept assisted aim.
- [ ] Set each of `input.aim_assist`, `input.aim_pull`, `input.aim_stabilization` and
      `input.target_lock` on alone with the others at 0 or off: each works alone and the others
      stay inactive. Then try pull with stabilisation, and lock-on with slowdown.
- [ ] Reduced motion + lock-on: the camera does not swing; a target marker shows the lock.
- [ ] Boundary parry input that fails at 1x succeeds at the configured window scale.
- [ ] Play muted: every off-screen attack and every detection state change is visible.
- [ ] Turn colour off (grayscale capture): all four stealth states remain distinct.
- [ ] Get detected in a mandatory stealth section with stealth failure off; the search ends and the
      section completes. Skip the section, then save and reload.
- [ ] Aggression 0: no attacks; quest, loot and boss-phase triggers still fire.
- [ ] Grab with combat damage Off: the grab releases.
- [ ] Change aim assist and aggression mid-fight; the change applies at the next decision.
- [ ] PvP or online: each simulation-affecting option shows its documented limit.
- [ ] Achievements earned with assists match those earned without.

## Common mistakes

- Aim assist that only helps against enemies → include every required target.
- Lock-on with no release or cycling → keep target choice with the player.
- Lock-on only at maximum aim-assist strength → give it its own key (`input.target_lock`).
- Pull or stabilisation that switches on when the slowdown slider passes a threshold → each part
  has its own key (`input.aim_pull`, `input.aim_stabilization`).
- Stealth state shown only as a colour ring or a sting → add shape, label and sound.
- Combat damage Off, but detection still fails the mission → route detection through
  `should_fail` with its own option.
- Single-button combo still needs rapid pressing → pair it with hold or auto-repeat.
- Client-side aim assist quietly enabled in ranked play → ask the developer and state the rule.
