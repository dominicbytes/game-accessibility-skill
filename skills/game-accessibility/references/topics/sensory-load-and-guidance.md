# Sensory load and guidance

**Load when:** S061 guidance paint, glowing trails or interaction highlights; S062 crowded HUD,
floating numbers or notification pop-ups; S060 stacked particles and hit effects; S056 animated menu
backgrounds; S008 haptics or adaptive triggers; S071 repetitive or distressing sounds; S070 loud
dense audio. **Skip when:** the game has no guidance visuals, no HUD beyond essentials, no
decorative effects, no notifications and no haptics. Decorative blinking (N11) and static menus
(N05) are still checked in every game.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| — | Overall guidance level (sets every channel left on `follow`) | `guidance.level` | `standard` | C | N, C, V |
| N03 | Guidance paint off, per channel | `guidance.paint`, `guidance.trail` = `off` | `follow` | C | N, C |
| N04 | Guidance only on request (hint action), per channel | `guidance.paint` / `guidance.trail` / `guidance.highlights` = `on_request` | `follow` | C | N, C |
| V27 | Interaction highlights: off, subtle or boosted | `guidance.highlights` | `follow` | C | V, C, N |
| N01 | Minimal HUD or HUD off | `hud.mode` = `minimal` / `off` | `full` | C | N, C |
| N02 | HUD visibility per element | `hud.mode` = `custom` + `hud.hidden_elements` | `full`, `""` | C | N, V |
| N06 | Reduce decorative world detail | `effects.particle_density` | 1.0 | C | N, C |
| N07 | Particle and hit-effect density | `effects.particle_density` | 1.0 | C | N, V |
| N08 | Fewer notifications, full log kept | `notifications.mode` | `all` | C | N, C |
| N05 | Static menu backgrounds | `motion.menu_animation` (umbrella `motion.reduce_motion`) | `follow` | B | N, C |
| N09 | No autoplay previews (separate sound control) | `game.ui.autoplay` | Designer | B | N, C, S |
| N10 | No menu transition animation | `motion.menu_animation` (umbrella `motion.reduce_motion`) | `follow` | B | N, O |
| N11 | No decorative blinking or pulsing; critical cues static | `game.ui.decorative_pulses` | Static critical cues | B | N, P |
| N12 | Vibration intensity and Off | `game.haptics.intensity` | Designer | C | N, M |
| N13 | Haptics by event category | `game.haptics.categories` | Designer | C | N, H, V |
| N14 | Adaptive-trigger resistance and Off | `game.haptics.trigger_resistance` | Designer | C | M, N |
| N15 | Quiet audio preset (editable) | preset over audio keys | Off | C | N, C, H |

Waypoints and route trails as memory aids (C17, C18) live in
[memory-and-navigation](memory-and-navigation.md). Motion effects live in
[camera-and-motion](camera-and-motion.md); flashing in [photosensitivity](photosensitivity.md).

## How to implement

### GuidanceLayer channels (N03, N04, V27)

- Tag every guidance visual when it is authored: paint, trail, interaction highlight, objective
  marker. Use a group, tag, layer or component per channel ([architecture](../architecture.md)
  section 6).
- Each channel has its own key: `guidance.paint` (painted ledges and routes), `guidance.trail`
  (trails, breadcrumbs, companion pointers) and `guidance.highlights` (glow or outline on usable
  objects). Their default `follow` uses the overall `guidance.level`. The effective level of a
  channel is its own key unless that is `follow`. A player can turn paint off and keep trails, or
  keep highlights boosted with paint subtle. There is no single switch for all guidance; the
  overall level is a convenience that the channel keys override.
- Levels map to visibility: `off` hidden; `on_request` hidden until the remappable hint action,
  then shown for a few seconds; `subtle` about 40% intensity; `standard` as authored; `boosted`
  brighter, thicker, outlined.
- Objective markers have their own key (`guidance.objective_marker`), owned by
  [memory-and-navigation](memory-and-navigation.md).
- **Hazard cues are never guidance.** Lethal edges, attack telegraphs and interaction prompts use a
  separate hazard channel that always shows. If guidance Off removes the only sign of a danger, the
  design needs another cue.
- Baked paint in textures cannot be toggled. Author it as a decal, overlay or material parameter.
  Check distant LODs, minimap icons and particles.
- Give guidance its own colour token so players can change its hue and opacity as well as its
  strength.
- Guidance Off never turns off the objective journal or recap. Those are memory aids.

**Both sides of the yellow-paint debate are real.** In an r/gaming thread, one player describes their
partner getting stuck when navigation cues disappear; another asks for a toggle and
colour/transparency control because they dislike the yellow presentation. Final Fantasy VII
Rebirth's director, as reported by PC Gamer, knows many players dislike yellow ledge paint but sees a
need for it. Do not equate any diagnosis with wanting paint off. Keep the designer's default, offer
Off, On request and Boosted, and treat no choice as a measure of skill.

### HUD (N01, N02)

- `hud.mode`: `full`, `minimal`, `custom` or `off`. `custom` hides the element ids listed in
  `hud.hidden_elements` (comma-separated, for example `minimap,damage_numbers,quest_tracker`).
  Name elements by stable id: health, resources, objective tracker, minimap, damage numbers,
  tutorial hints, notifications, decorative frames. Ignore unknown ids on load so an old list
  survives a revised build; keep new elements visible until the player hides them.
- `off` hides the HUD. Captions and subtitles are separate keys and stay as the player set them.
  Pause access and an on-demand status action stay available.
- Minimal, custom and off never hide subtitles, captions, pause and settings access, or hazard
  warnings.
- Essential status stays available on demand: a "show HUD" action, a status screen or narration.
- Damage numbers and loot pop-ups are separate from health and status.

### Effects density (N06, N07)

- Multiply decorative emitter counts by `effects.particle_density` at spawn. Tag effects as
  decorative or informative.
- When an informative effect is reduced, replace it with a simple readable cue. Hitboxes and
  hazard areas stay unchanged and visible.
- Test the worst case: many enemies, allies and abilities at once, not a quiet room.
- More feedback is not always better. Dense low-health feedback can overwhelm as well as inform.

### Notifications (N08)

- `notifications.mode`: `all`, `important` (objectives, threats, saves) or `quiet` (critical alerts
  only). Celebrations, achievements toasts and marketing are never "important".
- Every notification goes to a readable, replayable log. Quiet never deletes the only record of a
  new objective.
- Queue alerts that arrive together or during dialogue instead of stacking them. Durations and
  pinning: [memory-and-navigation](memory-and-navigation.md).

### Menus, autoplay and decorative blinking (N05, N09, N10, N11)

- When `motion.menu_animation` resolves to off (`off`, or `follow` with `motion.reduce_motion` on;
  resolver in [architecture](../architecture.md) section 1), menu video and particles freeze and
  transitions become instant without losing menu meaning. A player can stop menu animation and
  keep in-game parallax, or the reverse.
- Autoplay trailers and previews have their own off switch and their own sound switch.
- Replace decorative blinking and pulsing with a static icon or label. Critical cues stay static by
  default. Large or bright flashes go through `FlashGuard`.

### Haptics (N12–N14)

- Intensity slider with a true Off, reachable before the first strong vibration.
- Categories: information cues (navigation, threat direction, timing), impacts and ambient rumble,
  UI feedback. Information cues stay independent of ambient rumble.
- Vibration is never the only channel for information.
- Adaptive-trigger resistance can be lowered or turned off. Never require high force to progress.

### Quiet audio preset (N15)

An editable preset: lower `audio.music` and `audio.ambience`, turn on `audio.reduce_loud_sounds`,
keep `audio.voice` and important cues. Show every change before applying. Each bus stays separately
adjustable. Music at 0 also silences menu music. Isolated distressing sounds (buzzing, alarms)
get their own volume or replacement. Mix details: [audio-and-captions](audio-and-captions.md).

### Not one preference group

- Kylie Noble, autistic with ADHD, describes enjoying both predictable routine and novelty in games.
  Neurodivergent access is not "make everything quiet and slow".
- Cherry Thompson explains that extreme contrast can make reading harder for them; they prefer some
  softer dark interfaces with bolder text. This is a web interview, so applying it to games is an
  inference: offer adjustable presentation, not one high-contrast theme.
- Laura Kate Dale asks for more time, clearer context, sensory controls and pause, and says autism is
  a spectrum: the advice does not apply to every autistic player.
- In an r/ADHD thread, one player enjoys exploring more without a map full of markers, while another
  describes losing the plot after months away. Fewer markers and more recaps can both be right.

So: name presets by effect ("Less visual activity"), keep every control independent, and never ask
for a diagnosis.

### Combination checks

- Minimal HUD + captions + guidance On: every chosen information channel stays.
- Particles at minimum in a hazard-heavy fight: attack timing and danger areas stay readable.
- Music and ambience muted: dialogue, narration and navigation cues stay audible.
- Notification animations off: the log still holds every message.
- Guidance Off + objective journal On; paint Off + trail On; guidance Boosted + high-contrast
  outlines + reduced effects.
- HUD Off + captions On; custom HUD list kept after restart and after loading an old save.
- Reset one category without resetting the others.

## Shipped examples

- Star Wars Outlaws: Explorer Mode removes guiding colour from important world elements; markers and
  HUD configurable. [Ubisoft](https://news.ubisoft.com/en-gb/article/4J5XWevEccm1LBSduyij2H/star-wars-outlaws-accessibility-spotlight)
- Final Fantasy VII Rebirth: public yellow-paint debate. An official paint toggle is UNVERIFIED;
  a PC mod exists. [PC Gamer](https://www.pcgamer.com/games/rpg/ff7-rebirth-director-knows-a-whole-heap-of-people-hate-yellow-paint-on-ledges-but-reckons-there-is-definitely-a-need-for-that-kind-of-thing/)
- Resident Evil 4 (2023): HUD opacity and off. A built-in paint toggle is UNVERIFIED; the removal
  is a mod. [Can I Play That?](https://caniplaythat.com/2023/05/12/resident-evil-4-remake-accessibility-review/)
- Star Wars Jedi: Survivor: configurable navigation icons and audio pings; not proof that painted
  materials disappear. [EA](https://www.ea.com/able/resources/star-wars/star-wars-jedi-survivor)
- God of War Ragnarök: individual HUD visibility; the 2024 PC release reduces companion puzzle hints.
  [PlayStation](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/),
  [PC features](https://blog.playstation.com/2024/09/18/god-of-war-ragnarok-brings-a-host-of-new-features-to-pc-available-tomorrow/)
- Horizon Forbidden West: configurable HUD, guidance and camera effects.
  [PlayStation Blog](https://blog.playstation.com/2022/02/10/accessibility-features-in-horizon-forbidden-west/)
- Halo Infinite: "Disable Visual Distractions" for front-end menu animation only.
  [Halo Support](https://support.halowaypoint.com/hc/en-us/articles/4408122295444-Accessibility-Features-in-Halo-Infinite)
- Tetris Effect (PC): particle amount and size options; console parity not verified.
  [Tetris Effect](https://www.tetriseffect.game/2019/07/23/award-winning-tetris-effect-now-available-on-epic-games-store-with-optional-support-for-oculus-rift-and-htc-vive/)
- APICO: a separate option to disable buzzing sounds.
  [Whitethorn](https://whitethorngames.com/apico/accessibility)
- Returnal: caution. Dense multisensory low-health feedback; a launch reviewer found the help
  insufficient for his needs. [Developer article](https://blog.playstation.com/2021/05/11/unpacking-returnals-ux-design-gameplay-first-ui-retro-futuristic-tech-and-accessibility/),
  [Steve Saylor](https://www.youtube.com/watch?v=f3qJXxIPe94)

Voices cited above: [r/gaming yellow paint](https://www.reddit.com/r/gaming/comments/1cjxrjb/how_would_you_feel_if_the_yellow_paint_issue_was/),
[Kylie Noble](https://caniplaythat.com/2021/07/06/in-video-games-contradictions-of-being-autistic-with-adhd-make-sense/),
[Cherry Thompson](https://a11yrules.com/podcast/cherry-thompson-talks-about-cognitive-impairments-and-web-accessibility/),
[Laura Kate Dale](https://access-ability.uk/2025/03/07/autistic-player-accessibility-in-video-games/),
[r/ADHD](https://www.reddit.com/r/ADHD/comments/tj260i/gamers_with_adhd_do_you_end_up_getting_fixated_on/).

## Verify

- [ ] Reach the next destination with guidance `off`, `on_request`, `subtle`, `standard` and
      `boosted`. Re-test after the objective changes.
- [ ] Set paint `off`, trail `standard`, highlights `boosted`: each channel follows its own key;
      changing `guidance.level` changes only channels left on `follow`.
- [ ] Guidance Off: no paint, trail or glow remains (including decals, LODs and minimap), and every
      hazard cue and interaction prompt still shows.
- [ ] On request: the hint action is remappable and narrated; guidance hides again afterwards.
- [ ] Minimal, custom and off HUD never hide subtitles, captions, pause access or hazard warnings.
      HUD Off still offers status on demand. Custom hidden elements persist after restart.
- [ ] Particle density at minimum in the busiest encounter: every attack stays readable.
- [ ] Quiet notifications during dialogue: no pop-up, and the log has every entry.
- [ ] Menus with reduced motion: static background, instant transitions, no autoplay.
- [ ] Haptics Off: no information is lost.
- [ ] Every combination check above passes. Settings persist after restart and old-save load.

## Common mistakes

- One "HUD off" switch that also hides captions → per-element control; captions are separate.
- One guidance switch for paint, trails and highlights → per-channel keys with `follow`.
- Guidance Off hides the lethal-edge cue → give hazards their own channel.
- Paint baked into albedo textures → author it as a toggleable decal or material parameter.
- An "ADHD mode" that removes all markers → effect-named, independent controls.
- Reduced particles hide an enemy attack → replace it with a simple readable cue.
- Quiet notifications delete the new objective → keep everything in the log.
