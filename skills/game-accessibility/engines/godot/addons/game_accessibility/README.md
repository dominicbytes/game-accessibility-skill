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
