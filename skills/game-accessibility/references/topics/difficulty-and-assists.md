# Difficulty and assists

**Load when:** S035 ammo, S039 enemy pressure, S040 stamina or oxygen, S041 several damage
channels, S048 scripted defeats, S086 puzzles that block progress, S099 permadeath, S100 corpse
runs, S101 scarcity or grind, S102 hunger or decay, S104 long sequences, S105 random gates, S106
unrecoverable states, S107 grades, ranks or achievements, S157 random encounters or turn-based
battle commands, or any difficulty setting, fail state or
save system. **Skip when:** the game has no failure, no consumable resource, no achievements and no
blocking challenge. Record that evidence; a cosy or story game often still has one of these.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| D01 | Described difficulty presets with real numbers | `game.difficulty.preset` (editable patch) | Designer preset | B | T, C, M |
| D02 | Change difficulty or any assist at any time | none (core behaviour) | On | B | T, C, M |
| D03 | Independent difficulty per mechanic (combat, puzzle, traversal) | none (one key per mechanic) | Designer | C | M, C, T |
| D04 | Combat damage off (invincibility without soft-lock) | `assist.damage_taken` = 0 | 1.0 | C | M, T |
| D05 | Incoming damage scale | `assist.damage_taken` | 1.0 | C | M, T |
| D06 | Enemy health / outgoing damage scale | `game.assist.damage_dealt` | 1.0 | C | M, T |
| D07 | Safe falls (no fall death) | `assist.no_fall_death` | false | C | M, T, O |
| D08 | Separate fail channels (fall, drown, timer, detection) | `assist.no_fall_death` plus one key per reason | All on | C | T, M, C |
| D09 | Enemy aggression and detection speed | `assist.enemy_aggression` | 1.0 | C | T, M, C |
| D10 | Passive enemies | `assist.enemy_aggression` = 0 | 1.0 | C | T, F |
| D11 | Infinite ammunition | `assist.infinite_ammo` | false | C | M, T, C |
| D12 | Infinite stamina | `assist.infinite_stamina` | false | C | M, T |
| D13 | Lower resource and crafting costs | `game.assist.resource_cost` | 1.0 | C | C, T, M |
| D19 | Steering and braking assists | see [vehicles](vehicles.md) | Off | C | M, C |
| D20, D21 | Jump/landing assist, extra jumps | see [traversal-and-platforming](traversal-and-platforming.md) | Off | C | M, T |
| D22 | Puzzle skip | `assist.skip_available` | true | C | C, T |
| D23 | Room, encounter or chase skip | `assist.skip_available` | true | C | M, T, C |
| D24 | Retry near failure; checkpoint choice | none (design) | Designer | C | M, T |
| D25 | Save anywhere, or reliable suspend and resume | none (core behaviour) | On | C | M, C, S |
| D26 | Autosave history | see [memory-and-navigation](memory-and-navigation.md) | On | C | C, M, T |
| D27 | Pause gameplay and cutscenes | see [timing-and-speed](timing-and-speed.md) | On | B | M, C, S |
| D28 | No-loss death (keep currency, items, time) | `game.assist.death_penalty` | Off | C | T, C, M |
| D30 | Break reminders | `game.system.break_reminder` | Off | C | M, T, S |
| D31 | Random encounter rate, down to off (story battles kept) | `assist.encounter_rate` | 1.0 | C | M, C, T, S |
| D32 | Optional auto-battle; take control at any time | `assist.auto_battle` | false | C | M, C, T, S |
| D33 | Battle animation speed (decision time unaffected) | `assist.battle_speed` | 1.0 | C | M, C, T, S |

## How to implement

### AssistPolicy is the only place gameplay asks

```
policy.damage_to_player(amount, source)  # source: enemy | hazard | fall | self | scripted
policy.should_fail(reason)               # reason: fall | drowned | timer | detected | caught | ...
policy.consume(resource, amount)         # infinite -> true and nothing is spent
policy.enemy_aggression()                # attack frequency and detection speed multiplier
policy.request_skip(challenge_id)        # gated on assist.skip_available; normal success path
policy.request_scene_skip(scene_id)      # sensitive scenes; gated on content.scene_skip only
```

Gameplay never reads raw settings. Changing an assist mid-encounter applies at the next safe step
and never needs a restart ([architecture](../architecture.md) section 2).

### Presets and changing difficulty (D01–D03)

- A preset is an editable patch over individual values. Its description lists the actual numbers:
  "Enemies deal 50% damage. Timing windows are twice as long." Never "for players who only want the
  story".
- The pause menu changes difficulty and every assist at any time, mid-save, keeping progress.
  Applying a preset never overwrites a value the player set by hand afterwards.
- Expose each barrier on its own: combat, puzzles, traversal, stealth, timing. One "Easy" preset is
  not independent options.

### Damage, invincibility and fail channels (D04, D05, D07, D08)

- `assist.damage_taken` scales enemy and hazard damage. Falls follow `assist.no_fall_death`. Combat
  damage Off with falls On must be a supported, tested combination.
- Say exactly what each option covers: "Combat damage: Off. Falling and drowning are unchanged."
- **Invincibility must not soft-lock.** A health floor is not enough. Define recovery for pits,
  drowning, grabs, stun-lock, repeated knockdown and death-animation loops. Scripted defeats (S048)
  continue the script instead of waiting for a death that never comes.
- Represent each failure as a typed reason routed through `should_fail`. A disabled failure picks a
  real continuation: checkpoint recovery, last safe position, refill, or completed interaction.

### Enemy pressure (D06, D09, D10)

- `enemy_aggression` multiplies attack frequency and detection speed. At 0, enemies never attack.
  Quest, loot and phase triggers that depend on enemy attacks still fire.
- Enemy health is separate from incoming damage. Avoid endurance-only tuning that makes fights long
  and tedious. Combat and stealth detail: [combat-aiming-stealth](combat-aiming-stealth.md).

### Resources (D11–D13)

- **Intercept spending.** `consume()` returns true without spending. Never add infinity to a counter.
- Decide and document reload semantics: infinite reserve with normal reloads, or a bottomless
  magazine. Keep "empty" events working where a script or cue needs them.
- Quest items and currencies stay separate from consumables. Cost reduction is a multiplier applied
  at spend time, not a change to item data, so turning it off is clean.

### Skips (D22, D23)

- `request_skip` is for challenges. Sensitive-scene skips use `request_scene_skip`, gated on
  `content.scene_skip` and never on `assist.skip_available`
  ([content-and-phobias](content-and-phobias.md)); both reach the same completion path.
- `request_skip` grants the same items, flags, achievements, checkpoints and next-scene state as
  winning. It is idempotent: a skip and a success in the same frame cannot double rewards.
- Restore camera, input, AI, collision, music and objective state. Save and reload right after.
- The skip lives in the pause menu, available before the first attempt. An offer after N failures is
  an optional **extra**. It uses neutral copy, can be dismissed for the session or for good, and
  never replaces direct access.
- An achievement that requires a specific act ("collect 50 feathers") needs an accessible way to do
  that act. Do not add an "award everything" button: Xbox XR-057 forbids unlocking achievements from
  a menu without the gameplay, while allowing options that make the gameplay easier.

### Retry, checkpoints, saves and loss (D24, D25, D28)

- Offer retry close to the failure and a choice of recent checkpoints. A fix that only changes
  future runs does not help a save that is already blocked.
- Save anywhere, or a reliable suspend/resume that says what it keeps. A rest point several minutes
  away does not help someone who must stop now. Never write a save in an unwinnable state.
- No-loss death: separate switches for currency, items, time and corpse runs. Permadeath opt-out
  writes no marker on the save.

### Random encounters and battle commands (D31–D33)

Route these only when S157 is present: random battles, or battles fought through turn commands.
Each is its own key; a "fast battles" preset may set several, but the player can undo each.

- **Encounter rate (D31).** `assist.encounter_rate` scales random-encounter checks; 0 stops random
  battles only. Story, boss and scripted battles still trigger. Required rewards, items and XP
  stay reachable: if levels depend on grinding, pair it with grind relief (D13) or a scripted
  route that does not need random fights.
- **Auto-battle (D32).** `assist.auto_battle` lets party members pick sensible actions. One
  remappable action hands control back instantly, including mid-turn at the next command prompt.
  It uses the same battle rules, rewards and flags as manual play.
- **Battle speed (D33).** `assist.battle_speed` speeds animations, text pops and transitions only.
  It never shortens a decision window, an active-time gauge or a timed hit; those follow D29 and
  `input.timing_window_scale` ([timing-and-speed](timing-and-speed.md)).
- **Battle skip (D23)** uses the normal win path, as in the Skips section.
- Verify rewards, story flags and achievements through each path: manual win, auto-battle win,
  skipped battle and encounters off.
- FINAL FANTASY Pixel Remaster documents auto-battle, encounter disabling and experience
  multipliers for its PS4/Switch release; verify other releases independently.
  [Square Enix](https://www.square-enix-games.com/en_GB/home/final-fantasy-vi-pixel-remaster-launches-digitally-ps4-nintendo-switch-april-19th)

### Break reminders (D30)

Off by default. The player sets the interval. A reminder never forces a stop and never costs
progress; suspend must work at that moment.

### Achievements, endings and saves (core-policy section 2)

- Achievements evaluate **events** ("boss_defeated", "chapter_completed"), never settings. Grep the
  achievement and ending code for `assist`, `difficulty` and `settings`.
- No `everUsedAssist` flag, no badge on save cards, endings, grades or scores.
- **Counterexample, do not copy:** Steelrising's published Assist Mode makes some difficulty-related
  achievements unavailable once Assist Mode has been activated at least once in a playthrough.
- **Parity examples:** Control's developer says assists do not affect trophies or achievements.
  Dead Cells' developer says its Breaking Barriers options do not disable achievements.
- An existing project with difficulty-gated achievements is a conflict: report it to the owner and
  propose redesigns. Do not silently implement a penalty or rewrite published definitions. Separate
  leaderboards are an owner decision, not a default.
- **If the developer asks for the penalty** ("disable trophies when assist mode is on"), push back
  first. Use the script in core-policy section 2, "When the developer asks for a penalty": it
  makes players feel bad, they stop playing, and the worst case is a refund inside Steam's 2-hour
  and 14-day window.
- `system.assist_indicator` is an opt-in HUD reminder of active assists. It is never shown on saves,
  endings or results.

### Assist menu wording

| Avoid | Use |
|-------|-----|
| Cheats, baby mode, story-only mode | Assists, gameplay adjustments, or the feature's name |
| "Try without this first" | "Change these settings at any time." |
| "Easy: for players who only want the story" | "Enemies deal 50% damage. Timing windows are twice as long." |
| "Invincibility" when only enemy attacks are blocked | "Combat damage: Off. Falling and drowning are unchanged." |
| "Achievements may be unavailable" | "All achievements remain available." (only after verifying it) |

Use the intro *"You can change these settings at any time. All story content, endings and
achievements remain available."* only after the parity checks pass.

### Low-stress preset

Ship the registry's "Lower combat pressure" preset (`assist.damage_taken` 0.5,
`assist.enemy_aggression` 0.5, `assist.infinite_ammo` on) as an editable starting point. Show every
change before applying it. The player can keep puzzles or platforming hard while reducing combat.
Never call it "anxiety-safe": a game without death can still be frightening.

## Shipped examples

- Control (August 2020 update): separately configurable Assist Mode, immortality, one-hit kills;
  developer states trophies and achievements are unaffected.
  [Update notes](https://legacy.controlgame.com/de/control-august-update-notes/),
  [deep dive](https://legacy.controlgame.com/es/game-help-on-playstation-5-and-assist-mode-in-control-a-deep-dive/)
- Dead Cells (Update 29, 2022): continues, auto-hit, separate enemy health, enemy damage and trap
  damage. [Patch notes](https://dead-cells.com/patchnotes/29),
  [achievement statement](https://store.steampowered.com/news/posts/?appids=588650&enddate=1663602205&feed=steam_community_announcements)
- Psychonauts 2: No Fall Damage separate from invincibility, which covers "most attacks".
  [Double Fine](https://www.doublefine.com/games/support/psychonauts-2/accessibility?platform=all)
- TUNIC: separate No Fail Mode and No Stamina Restrictions.
  [Xbox podcast](https://news.xbox.com/en-us/podcast/gdc-and-sarah-bond/?ver=3.7.1)
- Alan Wake 2 (Anniversary Update): immortality, infinite ammo and batteries, one-shot kills; the
  retrospective describes scripted-death bugs found while building them.
  [Retrospective](https://www.alanwake.com/story/gameplay-assist-alan-wake-2/)
- Assassin's Creed Valhalla: separate combat, stealth and exploration difficulty.
  [Ubisoft](https://news.ubisoft.com/en-us/article/5Rys9UeXZh9bAjnHBZN4pC/assassins-creed-valhalla-bringing-accessibility-to-a-viking-epic)
- No Man's Sky (Waypoint): separate survival, combat, economy and death settings.
  [Hello Games](https://www.nomanssky.com/waypoint-update/)
- Hades: God Mode adds damage resistance that grows after deaths. Resistance is not invulnerability.
  [Supergiant FAQ](https://www.supergiantgames.com/blog/hades-faq/)
- SOMA Safe Mode: creatures behave differently, the game stays horror; chosen at new game, which is
  narrower than this skill's mid-game rule.
  [Frictional](https://frictionalgames.com/2017-11-what-is-somas-safe-mode/)
- Steelrising: counterexample above.
  [Xbox Wire](https://news.xbox.com/en-us/2022/09/08/steelrisings-assist-mode-feature/);
  criticism in [Digital Trends](https://www.digitaltrends.com/gaming/steelrising-review-ps5/).
  Current platform behaviour is UNVERIFIED.

## Verify

- [ ] Combat damage Off + falls On: enemies cannot kill; pits still do, exactly as described.
- [ ] Invincibility + pit recovery + checkpoint reload: no soft-lock, no endless death loop; grabs
      release; every scripted defeat continues.
- [ ] Each typed fail reason: disabled ones recover, retained ones still fail.
- [ ] Infinite ammo and stamina: actions still run, no underflow, scripted "empty" events fire.
- [ ] Every skip: rewards, doors, journal, checkpoint and next scene are coherent; save and reload
      immediately after; trigger skip and success together, rewards granted once.
- [ ] Skip is reachable from the pause menu before the first attempt. The failure offer can be
      dismissed for good.
- [ ] Change difficulty and assists mid-encounter and mid-save. No restart, no progress loss.
- [ ] Encounters off: every story battle still triggers and required items and XP stay reachable.
- [ ] Auto-battle on, then take control mid-battle with one action; battle speed at maximum leaves
      decision and timed-hit windows unchanged. Rewards, flags and achievements match a manual win.
- [ ] Earn the same achievement with assists on and off, and after toggling them mid-save. Use
      platform test facilities, not just a local toast.
- [ ] Grep: no achievement, ending, grade or save code reads assist or difficulty state.
- [ ] Save cards, endings and results show no assist marker.
- [ ] Assist settings persist across restart and are not restored from an old story save.

## Common mistakes

- One "Story mode" as the whole assist plan → expose each barrier separately.
- Health floor called invincibility → handle pits, grabs, drowning and scripted defeats.
- Infinite ammo by setting ammo to 9999 → intercept spending in `consume()`.
- Skip that teleports past the boss → run the normal completion path, then save and reload.
- Skip offered only after three failures → put it in the pause menu too.
- Achievement code checks the difficulty flag → evaluate events only.
- Copying Celeste's save-card marker or Steelrising's achievement rule → both break core policy.
