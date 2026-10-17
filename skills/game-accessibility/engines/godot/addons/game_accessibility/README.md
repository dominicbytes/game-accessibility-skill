# Game Accessibility addon (Godot 4.3+ / Redot)

Player-toggled accessibility options built on one settings store. Typed GDScript only, no
dependencies. Full integration guide for coding agents: `engines/godot/GUIDE.md` in the
game-accessibility skill.

## Install

1. Copy `addons/game_accessibility/` into your project's `addons/`.
2. Project > Project Settings > Plugins: enable **Game Accessibility**. This registers the autoload
   `A11y` (`a11y_settings.gd`). The other scripts are `class_name` services (`A11yAssistPolicy`,
   `A11yInputAssist`, `A11yCaptionBus`, `A11yCaptionView`, `A11yNarrator`, `A11yGuidanceLayer`,
   `A11yContentVariant`, `A11yFlashGuard`, `A11yCameraComfort`, `A11yColorTokens`,
   `A11ySettingsMenu`). They find the store through `A11ySettings.active()`, so they never depend
   on the autoload name.
3. For narration, enable Project Settings > Audio > General > **Text To Speech**
   (`audio/general/text_to_speech`).

## Prune the registry

`settings_registry.json` is a copy of the skill's canonical registry. Delete every setting whose
signal your game does not have (no ammo, no `assist.infinite_ammo`; no spiders, no
`content.spiders`). Preset entries for removed keys are skipped automatically. Services fall
back to the designer default when a key is pruned. Keep the ids unchanged.

## Behaviour worth knowing

- **Follow keys.** Keys with the values `follow` / `off` / `on` (outline classes
  `visual.outline_*`, motion overrides `motion.auto_camera`, `motion.fov_effects`,
  `motion.speed_lines`, `motion.parallax`, `motion.menu_animation`) resolve with
  `A11y.resolve_toggle(key, follow_value)`: `on` is true, `off` is false, and `follow` or a pruned
  key returns `follow_value`. Outlines follow `visual.high_contrast`; motion overrides follow
  `not motion.reduce_motion` (`A11yCameraComfort.allow_*()` does this for you).
- **Camera.** `scale_shake()`, `scale_bob()` (`motion.head_bob`) and `scale_sway()`
  (`motion.weapon_sway`) are separate scalars.
- **Flashes.** A request counts toward the budgets when `area >= 0.25` OR `intensity >= 0.1` OR it
  is saturated red (`saturated_red` argument, or `request_flash_color()`). The thresholds are
  conservative sample values to tune only with a photosensitivity analyser. The budgets are capped
  at 3 per 1 s and 9 per 5 s whatever the exports say. Red always counts and is always desaturated;
  saturated red uses linearised sRGB channels as in WCAG.
- **Captions.** Never clipped: long text is split into pages of at most 2 lines, re-split when the
  size or viewport changes, and timed by reading speed, or advanced with `advance_page()` for lines
  marked `can_wait` while `text.auto_advance` is off. Caption toggles apply to visible lines at once.
- **Content variants.** Never fall back to a less filtered level. Without a variant at or above the
  requested level, the presentation is hidden (or `neutral_fallback` is shown) and a warning lists
  the missing levels.
- **Skips.** `A11yAssistPolicy.request_skip(challenge_id)` follows `assist.skip_available`;
  `request_scene_skip(scene_id)` follows `content.scene_skip`. Both emit `A11y.skip_requested` once
  per id (`A11y.clear_skip(id)` re-arms it). A skip whose own key is pruned is never offered.
- **Narration.** Menu speech follows `narration.enabled` and stops at once (current and queued)
  when it is turned off; dialogue reading (`speak_dialogue()`) follows `narration.dialogue`
  independently. Focus and value changes stay silent while a screen reader is active.
- **Aim and scares.** `input.aim_assist` is slowdown only; `input.aim_pull` and
  `input.aim_stabilization` are separate. `content.jump_scares` is `full`/`reduced`;
  `content.scare_warnings` is a separate bool.

Tests: `tests/run_godot_tests.sh` in the skill repository runs 43 headless tests.

## Config file (public API)

`user://accessibility.cfg`, a ConfigFile:

```ini
[meta]
schema_version=1

[text]
scale=1.5

[motion]
camera_shake=0.0

[bindings]
jump="..."
```

Only values the player chose are written; a missing key means "game default or system
preference". Values are validated like the menu: an invalid value is reset on its own and an
out-of-range number is clamped. `[bindings]` (see `extra_sections`) belongs to the game.

`colorblind_sim.gdshader` is a developer simulation tool, not a player option.
