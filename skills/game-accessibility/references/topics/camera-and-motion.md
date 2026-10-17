# Camera and motion

**Load when:** a moving or first-person camera (S051), camera shake or kick (S052), forced pans,
recentring or scripted camera (S053), zoom, FOV kick, motion blur or speed lines (S054), roll, banking
or horizon tilt (S055), parallax, scrolling backdrops or animated menus (S056), screen-space distortion
or damage overlays (S057), weapon sway (S034), knockback or physics-driven camera motion (S145).
**Skip when:** the whole game is a fixed, non-scrolling view with static menus. A **2D game still
qualifies** when it scrolls, zooms, shakes, uses parallax or animates transitions. Skip the FOV row for
a fixed 2D board.

VR locomotion, snap turning and comfort vignettes (O18–O20) live in [vr-and-xr](vr-and-xr.md).
Vehicle horizon and stabilisation: [vehicles](vehicles.md). Flashing: [photosensitivity](photosensitivity.md).

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| O01 | Camera shake intensity / Off | `motion.camera_shake` | **Reduced (50%)**; 0% exists | C | O, N, P |
| O02 | Camera / head bob intensity / Off | `motion.head_bob` | **Reduced (50%)**; 0% exists | C | O |
| O03 | Weapon sway / breathing motion (visual) | `motion.weapon_sway` | **Reduced (50%)**; 0% exists | C | O |
| O04 | Motion blur / Off | `motion.motion_blur` | On (designer); OS reduce-motion as first value | C | O, V |
| O05 | Field of view | `motion.fov` | 75° vertical; range 60–110 | C | O, V |
| O06 | Camera distance (third person) | — | Designer | C | O, V |
| O07 | Camera speed / look sensitivity | `input.look_sensitivity` | 1.0 (0.25–3.0) | C | O, M |
| O08 | Camera smoothing / lag Off | — | Designer | C | O, M |
| O09 | Automatic recentring Off | `motion.auto_camera` | `follow`; manual recentre always bound | C | O, M |
| O10 | Scripted camera pulls Off | — | Designer | C | O, V |
| O11 | Automatic camera rotation Off | `motion.auto_camera` | `follow`; keep automatic rotation minimal by design | C | O |
| O12 | Stable horizon / reduced roll | — | Designer | C | O |
| O13 | Sprint and aim dynamic FOV Off | `motion.fov_effects` | `follow` | C | O |
| O14 | Speed lines / radial blur Off | `motion.speed_lines` | `follow` | C | O, N |
| O15 | Chromatic aberration Off; vignette, overlays and distortion scale | `motion.chromatic_aberration`; `motion.screen_effects` | On; 100% (designer) | C | O, V |
| O16 | Stable screen-centre dot | — | Off | C | O |
| O17 | Reduce parallax / background scrolling | `motion.parallax` | `follow` | C | O, N |
| O21 | Depth of field Off | `motion.depth_of_field` | On (designer) | C | V, O |
| N05 | Static menu backgrounds | `motion.menu_animation` | `follow` | B | N, C |
| N10 | Menu and UI transition animation Off | `motion.menu_animation` | `follow` | B | N, O |
| — | Reduce motion (all): umbrella for O09, O11, O13, O14, O17, N05, N10 | `motion.reduce_motion` | Off; OS reduce-motion as first value | B | O, N |

Shake, bob and weapon sway default to Reduced because core policy makes them a safe default, unless
the designer explicitly chooses otherwise. The `follow` keys use `motion.reduce_motion` until the
player sets them to `on` or `off`. Full Off always exists. `—` keys are not in the shared registry yet;
add a project key in `ACCESSIBILITY.md` when the signal is present.

## How to implement

### CameraComfort: multiply at the camera owner

- One owner computes the final camera transform and post-effect strengths each frame. Every source
  feeds it; nothing moves the camera behind its back.
- `final_shake = sum(all shake sources) × motion.camera_shake`. **All** sources: hits, explosions,
  landings, recoil, boss stomps, scripted events, **cutscenes**, death, and HUD shake.
- `final_bob = sum(bob sources) × motion.head_bob`, covering walk, sprint, crouch, swim, climb,
  ladders, vehicles and scripted walking.
- `final_sway = sum(weapon sway, breathing, held-item idle motion) × motion.weapon_sway`. This is
  the visual motion of the held item only; aim accuracy, spread and recoil do not change with it
  (aim stabilisation is `input.aim_stabilization` in
  [combat-aiming-stealth](combat-aiming-stealth.md)). A player can keep head bob and remove sway,
  or the reverse.
- `effect_strength = authored × motion.screen_effects` for vignette, damage overlays, distortion
  and film grain.
- Chromatic aberration follows `motion.chromatic_aberration` and depth of field follows
  `motion.depth_of_field`. Each is its own on/off key, independent of the overlay strength and of
  `motion.motion_blur`: a player can keep vignettes and lose colour fringing, or the reverse.
- **0% means zero** from every emitter. If something must remain, label the option "Reduced" and say
  what remains.
- Shake baked into animation or cutscene tracks must be routed as a camera channel the owner can
  scale. Audit timelines, not only gameplay scripts.
- When shake or an overlay carried information (a hit, low health, danger direction), keep that
  information through another cue: a HUD indicator, a sound with a caption, or rumble.

### reduce_motion (umbrella) and its per-effect keys

- `motion.reduce_motion` is the "Reduce motion (all)" umbrella for **decorative and automatic**
  motion. Each effect class has its own `follow` / `off` / `on` key:

  | Key | Covers | Resolve as |
  |-----|--------|------------|
  | `motion.auto_camera` | Automatic recentring (O09) and camera rotation (O11) | `resolve_toggle("motion.auto_camera", not reduce_motion)` |
  | `motion.fov_effects` | Sprint, aim and boost FOV changes, zoom pulses (O13) | `resolve_toggle("motion.fov_effects", not reduce_motion)` |
  | `motion.speed_lines` | Speed lines and radial blur (O14) | `resolve_toggle("motion.speed_lines", not reduce_motion)` |
  | `motion.parallax` | Parallax and background scrolling (O17) | `resolve_toggle("motion.parallax", not reduce_motion)` |
  | `motion.menu_animation` | Animated menu backgrounds (N05), UI transitions (N10, made instant), wobbling text | `resolve_toggle("motion.menu_animation", not reduce_motion)` |

  `true` means the effect plays. `follow` uses the umbrella; `on` or `off` overrides it, so a player
  can keep parallax with everything else reduced, or remove only speed lines. The resolver and the
  unregistered-key fallback are in [architecture](../architecture.md) section 1.
- The umbrella does not overwrite values the player set by hand. Shake, bob, sway and blur keep
  their own sliders, and a per-effect key set to `on` or `off` keeps its value when the umbrella
  changes. Presets such as "Less visual activity" are editable patches.
- Import the OS reduce-motion preference as the **first value** for `motion.reduce_motion`, shake,
  bob, sway and blur, only while the player has not set them. The per-effect keys stay `follow`
  and pick it up through the umbrella. Sources: CSS
  [`prefers-reduced-motion`](https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/At-rules/@media/prefers-reduced-motion)
  (mirror it in canvas and WebGL code too), macOS
  [`accessibilityDisplayShouldReduceMotion`](https://developer.apple.com/documentation/appkit/nsworkspace/accessibilitydisplayshouldreducemotion),
  iOS [`isReduceMotionEnabled`](https://developer.apple.com/documentation/uikit/uiaccessibility/isreducemotionenabled),
  Windows [`UISettings`](https://learn.microsoft.com/en-us/uwp/api/windows.ui.viewmanagement.uisettings)
  animation preference. Android's
  [`areAnimatorsEnabled()`](https://developer.android.com/reference/android/animation/ValueAnimator#areAnimatorsEnabled())
  is only a conservative hint. Check engine support for each.
- The settings screen obeys these values immediately, including its own background.

### Field of view (O05)

- `motion.fov` stores **vertical** degrees. Show the label as vertical and display the horizontal
  equivalent for the current aspect ratio. If the engine uses horizontal FOV, convert; never mix.
- Keep vertical FOV fixed across aspect ratios (horizontal grows on wider screens) and test ultrawide
  and 4:3.
- Wider is not better for everyone; distortion grows at the edges. Preview in the real scene before
  entering the world. Choose a default suited to the viewing distance (TV vs desk).
- Aiming magnification is separate from the comfort FOV and from dynamic sprint FOV (O13).

### Blur and screen effects (O04, O15, O21)

- `motion.motion_blur = false` removes camera **and** per-object blur. Lowering a graphics preset is
  not a substitute.
- `motion.chromatic_aberration = false` removes colour fringing from gameplay, damage effects and
  cutscenes.
- `motion.depth_of_field = false` keeps readable interactables and text sharp, including in photo
  mode and cutscenes, or the description states the exclusion.
- A damage vignette is not a comfort vignette. Removing it must not remove the low-health information.

### Automatic camera movement (O08–O11, O13, O14)

- Automatic recentring (O09) and auto-rotation (O11) stop when `motion.auto_camera` resolves to
  off (by default, when `motion.reduce_motion` is on); a manual recentre action stays bound.
- Scripted camera pulls toward points of interest (O10) become a marker, a sound cue or a prompt to
  look. Never take the camera without a reduced alternative.
- **Avoid forced or continuous rotation.** In a recorded playthrough of the Redot jam entry *Last
  Ride*, continuous view rotation gave the player a headache and ended the session (Dominic Bytes,
  "Not Meant to Last, part 3", [YouTube, 00:25:03-00:28:14](https://www.youtube.com/watch?v=KotObSElUb0);
  one player's account). Keep turning under player control.
- Camera assists that help motor access can move the view more. Offer them separately from reduced
  motion and test both on together; manual input must win without a tug-of-war.
- Smoothing or lag (O08) can be turned off so the view responds predictably.

### Horizon, distance, sensitivity, centre dot (O06, O07, O12, O16)

- Roll and banking (O12) can be locked to a stable horizon. Keep orientation information elsewhere
  (an instrument, a HUD horizon).
- Camera distance (O06) keeps collision and objectives visible at every value.
- Sensitivity (O07) offers real range: at least 50% below and 50% above default per axis, separate
  from movement speed. Hidden acceleration is its own setting.
- A centre dot (O16) has size, opacity and contrast settings and is separate from the weapon reticle.
  State exclusions such as cutscenes.

### 2D games

- Scrolling speed, screen shake, zoom punches, parallax layers, screen-wipe transitions and animated
  backgrounds all route through CameraComfort or the per-effect keys above (`motion.parallax`,
  `motion.menu_animation`).
- Under reduced parallax, background layers lock or move with the foreground; the gameplay layer still
  scrolls so navigation is unaffected.

### Cutscenes

- Cinematic cameras use the same multipliers. If a cutscene cannot reduce shake, it is skippable and
  pausable, and the description says so.

## Numbers that matter

| Source | Number | Scope |
|--------|--------|-------|
| [XAG 107](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/107) | Sensitivity at least 50% below and 50% above default | Each relevant analog control |
| Core policy and registry | Shake and bob default 50% ("Reduced"); 0% is complete Off | Safe default |
| Registry `motion.fov` | 60–110° vertical, default 75° | Label convention; convert if the engine is horizontal |

[XAG 117](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/117)
covers FOV, secondary camera motion, blur and automatic camera movement. It gives no single comfortable
configuration; neither does any other source.

## Shipped examples

- Marvel's Spider-Man 2: camera shake, motion blur, depth of field, chromatic aberration and
  full-screen effect toggles.
  [Insomniac](https://support.insomniac.games/hc/en-us/articles/46730041467027-What-Accessibility-options-does-Marvel-s-Spider-Man-2-feature).
  A low-vision reviewer kept one effect that helped him notice low health
  ([Game Accessibility Nexus](https://www.gameaccessibilitynexus.com/blog/2023/10/16/marvels-spider-man-2-low-vision-review/)):
  independent toggles beat one "all effects off" switch.
- The Last of Us Part I: persistent centre dot with stated exclusions, and camera distance.
  [PlayStation Blog](https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/).
- Cyberpunk 2077 (patch 2.1): camera sway and centre-screen dot settings.
  [CD Projekt Red](https://www.cyberpunk.net/en/news/49591/update-2-1-accessibility-features).
- Halo Infinite: blur and shake reduction; "Disable Visual Distractions" for front-end menu
  animation, which does not promise to remove gameplay effects.
  [Halo Support](https://support.halowaypoint.com/hc/en-us/articles/4408122295444-Accessibility-Features-in-Halo-Infinite).
- Dead Space (2023): option to disable the floating menu effect.
  [EA](https://www.ea.com/ea-play/news/accessibility-and-player-choice-in-dead-space).
- Rocket League: camera shake, FOV and distance settings.
  [Epic help](https://www.epicgames.com/help/c-37599050/c-7261959845275/a14443106?lang=en-US).
- God of War Ragnarök: camera comfort options and a centre-screen dot.
  [PlayStation](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/).
- Horizon Forbidden West: auto-camera support added in a later patch
  ([community patch thread](https://www.reddit.com/r/horizon/comments/12qo4vr/horizon_forbidden_west_patch_121/));
  a motor aid that may not suit motion-sensitive players.

## Verify

Run the same route with designer defaults, then with shake 0, bob 0, sway 0, blur off, effects 0,
chromatic aberration off, depth of field off and `motion.reduce_motion` on:

- [ ] First-launch logos, menu, character creation and loading. Menu backgrounds and transitions are
      static or instant under reduced motion.
- [ ] Walk, sprint, turn, aim, recoil and take damage. No shake, bob or sway remains at 0%; hit and
      low-health information is still visible another way.
- [ ] Climb, fall, moving platforms, vehicles and swimming where present.
- [ ] Scripted camera transitions, cutscenes, boss attacks, death and respawn. No residual shake at 0%.
- [ ] Change camera values mid-play from the pause menu; they apply live without restart.
- [ ] For each camera or effect source found in code and timelines, record whether the setting
      reaches it. Unreached sources are failures or documented exclusions.
- [ ] FOV label states vertical or horizontal; ultrawide and 4:3 behave as described.
- [ ] Fresh profile with OS reduce-motion on: first values import; a value set by hand later is not
      overwritten by the OS on next launch.
- [ ] Camera assist and reduced motion on together: manual input controls the view.
- [ ] Screen effects 0 with chromatic aberration and depth of field on, then the reverse: each key
      changes only its own effect.
- [ ] Head bob 0 with weapon sway 100%, then the reverse: each changes only its own motion, and aim
      accuracy is the same at every sway value.
- [ ] Per-effect overrides: with `motion.reduce_motion` on, set one per-effect key (for example
      `motion.parallax`) to `on`; only that effect returns. With the umbrella off, set one key to
      `off`; only that effect stops. Toggle the umbrella afterward: overridden keys keep their
      values, `follow` keys change. Repeat for each per-effect key the game ships.
- [ ] Settings persist across restart and are not stored in story saves.

An unaffected tester's comfort does not prove the configuration works for motion-sensitive players.

## Common mistakes

- Shake slider that misses cutscene or animation-baked shake → route every source through the owner.
- "Off" that leaves residual shake → 0% is zero, or rename it "Reduced".
- Bob setting that covers walking only → include sprint, crouch, swim, ladders and vehicles.
- FOV slider with no axis label → say vertical or horizontal and show the other.
- Treating a 2D game as exempt → scrolling, parallax, zoom and shake still count.
- Animated settings screen that ignores reduced motion → the settings UI obeys its own settings.
- Reduced motion overwriting the player's own shake value → it covers decorative motion only.
- One "reduce motion" switch for every decorative effect → keep the umbrella, but give each effect
  class its own `follow` / `off` / `on` key.
- Weapon sway on the head-bob slider → `motion.weapon_sway` is its own key.
- Forced continuous camera rotation → keep turning under player control.
- Removing damage overlays with no replacement cue → keep the information in another channel.
