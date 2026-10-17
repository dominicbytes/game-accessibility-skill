# Architecture: integrate from day one

Load this before implementing any option. Every engine sample in `engines/` implements the same
contract with the same setting keys (`assets/settings-registry.json`). Adapt names to the project's
conventions, but keep the boundaries: **gameplay code asks a policy; it never reads raw settings or
physical keys.**

```
hardware / OS prefs / mods
        │
  Input action layer ──► InputAssist (hold↔toggle, mash→hold/single, QTE modes, timing scale)
        │
  A11ySettings store ◄── menus, presets, config file, OS preference import
        │ (typed get / set / changed(key) signal / persist / migrate)
        ├─► AssistPolicy      damage, fail reasons, resources, speed, aggression, timers, skips
        ├─► CaptionBus        semantic audio events → subtitles, captions, visualiser, narration
        ├─► Narrator          TTS + screen-reader bridge, priority queue
        ├─► GuidanceLayer     tagged guidance visuals (paint, trails, outlines, markers)
        ├─► ContentFilter     tagged content variants (spiders, gore, needles...)
        ├─► FlashGuard        every flash/strobe request goes through it
        ├─► CameraComfort     shake/bob/FOV/blur/reduce-motion scaling at the camera owner
        └─► ColorTokens       semantic colours + paired symbols
```

These can be a handful of small scripts in a jam game. Do not add a framework, ECS or
dependency-injection system to get them. Do not create a catch-all mutable global. Each service owns
one concern, and the settings store is the single source of truth.

## 1. A11ySettings: the store

- **Registry, not ad-hoc variables.** Load the keys the game needs from a registry (copy entries from
  `assets/settings-registry.json`). Each entry has `id`, `type`, `default`, range or values, `label`,
  `description`, `scope` and `apply`.
- **API:** `get(id)`, `set(id, value)` (validates and clamps, returns whether it changed),
  `reset(id)`, `reset_category(category)`, `apply_preset(preset_id)` (one batch, undoable),
  `changed(id, value)` signal or event, `save()`, `load()`.
- **Setters change only their own key.** Changing VSync must not resize the window. Changing a preset
  must not erase values the player set by hand afterward. Test isolation (see verification.md).
- **Late subscribers:** a node or actor that spawns later reads the current value on creation, then
  subscribes. Never rely on having seen an earlier change event.
- **Persistence:** a versioned, human-readable file (`user://accessibility.cfg`,
  `Application.persistentDataPath/accessibility.json`, or `Saved/Config/...`). Keep it **outside story
  saves**. Loading an old save must not restore old accessibility settings. Write to a temp file and
  then rename. Repair one invalid value instead of resetting everything. Migrate by meaning when a
  key changes (`v1 shake_off: true` → `v2 motion.camera_shake: 0.0`).
- **OS preferences** are imported as the *initial* value for keys with `platform_pref`
  (reduce_motion, screen_reader_active, increase_contrast, captions, text_scale) **only while the player
  has not set that key explicitly**. Track "explicitly set" per key.
- **Umbrella + per-part keys (the follow resolver).** This is the standard shape for every "all"
  control. The umbrella (`motion.reduce_motion`, `visual.high_contrast`) is a convenience switch.
  Each distinct effect or class has its own `follow` / `off` / `on` key (`motion.auto_camera`,
  `motion.fov_effects`, `motion.speed_lines`, `motion.parallax`, `motion.menu_animation`;
  `visual.outline_enemies`, `visual.outline_allies`, `visual.outline_interactables`,
  `visual.outline_hazards`), default `follow`. Consumers never read the umbrella directly; they ask
  the store:

  ```
  resolve_toggle(key, follow_value) -> bool   # Unity/Unreal: ResolveToggle
    "on"  -> true
    "off" -> false
    "follow", or key not registered -> follow_value   # the umbrella-derived value
  ```

  For a "reduce" umbrella the caller passes the inverted umbrella, for example
  `resolve_toggle("motion.parallax", not reduce_motion)` (parallax shown unless reduced). Outlines
  pass the umbrella as is: `resolve_toggle("visual.outline_hazards", high_contrast)`. Setting the
  umbrella changes only the umbrella key; a part the player set to `on` or `off` keeps its value.
  A game that prunes a part key from its registry still works, because an unregistered key
  follows the umbrella. `guidance.*` channels use the same pattern with levels instead of booleans
  (section 6). Test every part against both umbrella values.
- **Scope:** `profile` keys follow the player. `device` keys (master volume, brightness, mono output)
  stay with the machine. Local co-op needs per-player profiles for personal settings.
- **Config file is public API.** Document it in the README so players, modders and assistive tools
  can edit it. Validate it the same way the menus do.

## 2. AssistPolicy: gameplay modifiers in one place

Gameplay systems call the policy at the moment of decision:

```
policy.damage_to_player(amount, source)  -> float     # source: "enemy" | "hazard" | "fall" | "self" | ...
policy.should_fail(reason)               -> bool      # reason: "fall" | "timer" | "detected" | "drowned" | ...
policy.consume(resource, amount)         -> bool      # resource: "ammo" | "stamina" | ...; infinite → true, not spent
policy.timer_scale()                     -> float|INF # applies to countdowns that are not races
policy.time_scale()                      -> float     # game speed for simulation clocks only
policy.enemy_aggression()                -> float     # attack frequency / detection speed multiplier
policy.request_skip(challenge_id)        -> bool      # gated on assist.skip_available
policy.request_scene_skip(scene_id)      -> bool      # gated on content.scene_skip
```

Rules:
- **Separate failure channels.** `damage_taken = 0` blocks enemy and hazard damage, but falls follow
  `no_fall_death`. The player can keep the failure states they enjoy.
- **Invincibility must not soft-lock.** Pits, drowning, grabs, scripted defeats and stun-locks need
  a defined recovery: return to the last safe position, release the grab, or continue the script.
- **Infinite resources intercept spending.** Do not add `INF` to a counter. Keep quest items and
  currencies separate. Effects that need an "empty" event (reload animation, low-stamina cue) still
  work.
- **Game speed changes simulation time only.** It does not change menus, subtitle timing tied to
  audio, narration or network clocks.
- **Timing windows and game speed are independent choices that combine predictably.** Measure
  windows in simulation time and apply `input.timing_window_scale` once. The window then lasts
  `base × window_scale / time_scale` seconds of real time. A player who picks both (the "More time"
  preset does) gets both. The bug to avoid is applying either multiplier twice, for example by also
  scaling the window by `time_scale` or measuring it in real time.
- **Two skip requests, two gates, one completion path.** `request_skip(challenge_id)` skips a
  challenge (puzzle, boss, chase) and is allowed only while `assist.skip_available` is on.
  `request_scene_skip(scene_id)` skips a scene listed in the content notices and is allowed only
  while `content.scene_skip` is on. Neither checks the other key: a player can skip a sensitive
  scene with challenge skips off, and a story game whose registry has only `content.scene_skip`
  still skips scenes. Both return whether the request was accepted and, when accepted, raise the
  same skip-completed event that the owner handles through its normal success path. Test all four
  key combinations.
- **Skips use the normal completion path.** Grant the same items, flags, achievements, checkpoints
  and next-scene state as winning. The transition must be idempotent, save and reload correctly
  right after, and restore camera, input, music and objective state.
- **Achievements read events, never assist state** (core-policy section 2). Grep the achievement
  code for any reference to `assist`, `difficulty` or `settings` during review.
- Changing an assist mid-encounter applies at the next safe step. It never needs a restart.

## 3. InputAssist and the action layer

- Gameplay reads actions (`jump`, `aim`, `interact`), never keys. Prompts are generated from the
  **current binding** of the action and update when it is remapped or the device changes.
- Remapping covers **gameplay, menus, minigames, vehicles and pause/back**, supports more than one
  binding per action, handles conflicts and offers reset to default. A reachable recovery route
  must remain even if the player unbinds "confirm" (a default reset key in the config file, or a
  timed revert).
- `InputAssist` wraps actions:
  - `is_held(action)`: with `hold_mode = toggle`, the first press latches and the second releases.
    The latch clears on pause, death, scene change, focus loss and device disconnect.
  - `mash_progress(action, required_presses)`: in `mash` mode it counts presses. In `hold` mode,
    holding fills progress at the authored rate. In `single` mode one press completes it.
  - `qte(prompt)`: in `normal` mode it uses the authored window. In `slow` mode the window is scaled
    by at least 3 times, or waits indefinitely when nothing else moves. In `auto` mode it succeeds
    through the normal success branch after a short readable beat.
  - `timing_window(base_seconds)`: returns `base × input.timing_window_scale` in simulation time
    for parry, dodge, rhythm and perfect-hit windows (see the formula in section 2).
- Never replace mashing with a long hold as the *only* alternative. Some players cannot hold.

## 4. CaptionBus: one event, many outputs

Author **semantic audio events**, not just audio files:

```
CaptionEvent { id, kind: dialogue|sound|music, text_key, speaker_id?, source_position?,
               importance: low|normal|critical, duration_s | audio_player, merge_key? }
```

- Dialogue events always carry text. Important non-speech sounds carry a caption key
  (`[Alarm blaring, left]`, `[Spider skittering above]`).
- The bus emits to: the subtitle view (dialogue, if `subtitles.dialogue`), the caption view (sounds,
  if `subtitles.sound_captions`), the sound visualiser (direction indicator, if
  `audio.sound_visualizer`) and the Narrator (if `narration.dialogue` is on and the line is unvoiced).
- **Captions fire from the event even when audio is muted.** Never derive them from audio levels.
- Looping sounds are captioned once with a merge key, not every loop.
- Subtitle style: at most 2 lines and about 40 characters per line. Speaker name before the line.
  Background opacity and size come from settings. Placed inside the safe area, never over prompts.
- Captions do not reveal more than a hearing player would get. A hidden speaker is "Unknown voice".

## 5. Narrator: TTS and screen readers

- Prefer the engine's **native accessibility tree** where it exists, for example Godot 4.5+ and
  Redot 26.1+ through AccessKit `accessibility_name` and `accessibility_description`, or Unity 6.3+
  `AccessibilityHierarchy`. Then the player's own screen reader does the talking.
- Use built-in **TTS self-voicing** as the fallback or complement:
  `speak(text, priority, interrupt)`, `stop()` and `set_rate()`. Use a bounded queue. New focus speech
  interrupts stale focus speech. Critical announcements are never dropped.
- Narrate focus changes in every menu: name, role, value and hint. Examples: "Text size, slider, 150
  percent" and "Subtitles, on, checkbox".
- Do not double-speak when an OS screen reader is active and self-voicing is on. Pick one route per
  announcement.
- Menu narration is Baseline. Narrating gameplay state (health on demand, objective on demand) is
  Contextual. Full nonvisual play is Advanced and needs its own scope decision.

## 6. GuidanceLayer: paint, trails, highlights

- Every guidance visual is tagged with a **channel** when it is authored: `paint`,
  `trail`, `interact_highlight`, `objective_marker`. Use an engine group,
  tag, layer or component.
- `guidance.level` is the overall level. Each channel has its own key (`guidance.paint`,
  `guidance.trail`, `guidance.highlights`) whose default `follow` uses the overall level, so a
  player can keep trails and turn paint off. The effective level for a channel is its own key
  unless that is `follow`. Levels map to visibility and intensity:
  - `off`: hidden.
  - `on_request`: hidden until the hint action is pressed, then shown for a few seconds.
  - `subtle`: about 40% intensity.
  - `standard`: as authored.
  - `boosted`: brighter, thicker and with an added outline.
- **Hazard warnings are not guidance.** Never hide the only cue for a lethal edge, an attack telegraph
  or an interaction prompt. Give hazards their own channel that stays on.
- Baked paint in textures cannot be toggled. Author it as a decal, overlay or material parameter.

## 7. ContentFilter: phobia and sensitive content

- Tag content at authoring time with categories from signal-routing.md (`arachnid`, `insect`,
  `snake`, `needle`, `gore`, `deep_water`, `heights`, `holes`, `clown`, ...).
- Each tagged thing has **presentation variants**: mesh or sprite, animation set, sound set, icon and
  portrait. The logical entity, hitbox, AI, damage and rewards are shared and unchanged.
- Swap before first render. Swap live when the setting changes, with no frame of the original.
  Cover corpses, icons, bestiary entries, loading screens, trophies, achievement art and reflections.
- Settings previews start from the **most filtered** variant. The original is shown only on an
  explicit request.

## 8. FlashGuard and CameraComfort

- All full-screen flashes, strobes, lightning, hit flashes and muzzle flashes request through
  `FlashGuard.request_flash(intensity, area)` (plus a saturated-red flag where the engine sample
  takes one). **Counting rule, used by every sample:** a request counts toward the budget if it
  covers **at least 25% of the screen**, **or** changes relative luminance by **at least 0.1**, **or**
  is **saturated red**. When the area or luminance change is unknown, count it. Counted requests are
  limited, always and in every mode, to **no more than 3 in any 1-second window and no more than 9
  in any 5-second window**; these budgets are hard caps that configuration can lower but never
  raise. The second limit stops a sustained 2-3 Hz strobe, which EA IRIS treats as an extended
  failure. With `motion.reduce_flashing` on, it converts flashes into steady, low-contrast fades or
  icon pulses. Saturated red flashes are desaturated in every mode.
- The 25%-of-screen and 0.1 thresholds are **conservative sample defaults**, to be tuned against
  footage analysis, not a WCAG equivalence. WCAG's general-flash area is 25% of any 10° visual
  field. WCAG estimates that field as a 341 × 256 px rectangle on a 1024 × 768 display, so the
  limit is roughly 22,000 px, under 3% of that screen and far smaller than a quarter of it. A small
  flash can still fail; that is why unknown area counts and why captured footage is the real check
  (topics/photosensitivity.md).
- FlashGuard is **one prevention layer, not a safety proof**. It cannot see flashes baked into
  video, textures or overlapping shaders. Verify by analysing captured footage (see
  topics/photosensitivity.md). If no analyser can run, the photosensitivity check is `blocked`.
- The camera owner multiplies every shake source by `motion.camera_shake`, every bob by
  `motion.head_bob`, held-item sway by `motion.weapon_sway`, and every overlay by
  `motion.screen_effects`, and honours `motion.motion_blur`, `motion.chromatic_aberration` and
  `motion.depth_of_field` separately. This includes scripted and cutscene shake.
  `motion.reduce_motion` is the umbrella for automatic camera movement, dynamic FOV, speed lines,
  parallax and menu animation; each is resolved through its own key with `resolve_toggle`
  (section 1).

## 9. ColorTokens

- UI and gameplay colours are **semantic tokens** (`team.ally`, `team.enemy`, `hazard`, `pickup.rare`,
  `wire.a`), never literal colours in scripts. Each token has a paired **symbol or pattern**
  (shape, icon, letter or texture) that is always shown.
- `visual.color_profile` swaps the token palette per element. `custom` reads the player's own colours
  from `visual.custom_colors` (`role=#RRGGBB;...`). A full-screen daltonisation filter is
  optional and extra at most. Simulation shaders are developer tools, not player options.
- Text and UI contrast: 4.5:1 minimum for body text and essential icons, 3:1 for large text, 7:1 for
  high-contrast mode. Check with `scripts/contrast_check.py`.

## 10. Menus and first launch

```
boot (static screen, no logo/music/content audio) → load + migrate settings → import OS prefs
→ first-launch accessibility screen (once; skippable; re-openable from Settings):
    text size + font · subtitles · narration toggle (keyboard/controller/TTS operable)
    · audio levels · reduce motion/flashing · presets list
→ logos / intro (respecting the above) → title (Settings · Accessibility always visible)
```

- No OS screen reader detected? The first-launch screen may play **one quiet, repeatable spoken
  prompt** ("Press N for spoken menus") at the saved or quiet default volume. It is the only sound
  allowed before settings, it never turns narration on by itself, and it is distinct from logo,
  music or content audio.
- The settings UI follows its own settings: it is narrated, scales with text size, is fully
  operable with keyboard or controller only, and has visible focus.
- Every option shows a one-line effect description and a **safe preview**: sample subtitle on a
  bright background, a subdued motion sample, the filtered creature first.
- Reset per category. Undo for presets.
- HUD: `hud.mode` = `off` hides the HUD but keeps captions (separate keys), the pause route and an
  on-demand status action. `custom` hides the element ids listed in `hud.hidden_elements`.
- Pause menu: Resume · Settings (same screen) · Assists · Skip challenge (if the signal is present)
  · Restart checkpoint · Quit to title.

## 11. When the engine has no sample here

Implement the same contract with the engine's primitives: a dictionary-backed store, an event or
callback per key, the engine's input action system, its TTS or platform speech API, tags or groups
for guidance and content. Verify the engine version's API before using it. See `engines/other/`.
