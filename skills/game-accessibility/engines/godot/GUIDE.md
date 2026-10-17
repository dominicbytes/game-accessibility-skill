# Godot 4 / Redot: wiring the accessibility services

For coding agents. The sample is `addons/game_accessibility/`, the contract is
[architecture.md](../../references/architecture.md), and the rules are
[core-policy.md](../../references/core-policy.md).

**Verified on:** Redot 26.3-stable (`26.3.stable.official.85e520394`, Godot 4.5 based), Windows,
headless: tests pass **30/30**, with every default GDScript warning (plus untyped declarations)
promoted to an error. Command, run from the repository root:
`REDOT_BIN=/path/to/redot tests/run_godot_tests.sh`. It copies the addon, runs `--import`, then
`--headless --audio-driver Dummy --script res://run_tests.gd --quit-after 600` under `timeout 180`.
`colorblind_sim.gdshader` was rendered on Forward+ (Vulkan) and Compatibility (OpenGL 3.3) and matched
the Machado matrices within 0.0025 per channel. **Not verified:** real TTS speech, screen-reader
(AccessKit) output, the editor plugin toggle, exported builds and plain Godot 4.3/4.4. The code
avoids 4.4+ syntax and guards every 4.5-only API.

## 1. Check the engine first

```sh
"$REDOT_BIN" --version      # or "$GODOT_BIN" --version
```

- Godot 4.5+ and Redot 26.1+ ship AccessKit: `Control.accessibility_name`,
  `accessibility_description`, `accessibility_live`, and `DisplayServer.accessibility_*` OS-preference
  probes. Older engines have none of these; the addon detects that with `"accessibility_name" in
  control` and `DisplayServer.has_method(...)`. Do not trust a version number alone: on Redot, check
  the generated API (`--dump-extension-api`) or run the tests.
- TTS (`DisplayServer.tts_*`) exists since 4.0 but only works when the project setting
  `audio/general/text_to_speech` is on and the OS has a voice. Headless runs have no TTS.

## 2. Install

1. Copy `addons/game_accessibility/` into the project. Enable the plugin (adds autoload `A11y`).
2. Enable `audio/general/text_to_speech` if the game has menus (menu narration is Baseline).
3. Prune `settings_registry.json` to the signals in the project's `ACCESSIBILITY.md`. Keep ids.
4. Create audio buses named `Master`, `Music`, `SFX`, `Voice`, `Ambience` for the volume keys.
   Missing buses are skipped. Mono adds an `AudioEffectStereoEnhance` (pan pullout 0) to `Master`.
5. CI: run `--headless --import` once before `--script` runs, or `class_name` types are unknown.

## 3. Service map

| Contract (architecture.md) | Godot sample | Typical owner |
| --- | --- | --- |
| A11ySettings store | `a11y_settings.gd`, autoload `A11y`, `A11ySettings.active()` | autoload |
| AssistPolicy | `A11yAssistPolicy` static functions | gameplay code |
| InputAssist | `A11yInputAssist` node | player scene |
| CaptionBus + views | `A11yCaptionBus`, `A11yCaptionEvent`, `A11yCaptionView` | HUD `CanvasLayer` |
| Narrator | `A11yNarrator` | menus, HUD |
| GuidanceLayer | `A11yGuidanceLayer` + groups | level root |
| ContentFilter | `A11yContentVariant` per tagged entity | enemy/prop scenes |
| FlashGuard | `A11yFlashGuard` (one per game, found by group) | HUD |
| CameraComfort | `A11yCameraComfort` | camera rig |
| ColorTokens | `A11yColorTokens` resource (`.tres`) | UI theme, materials |
| Menus | `A11ySettingsMenu` (generated from the registry) | title, pause, first launch |

API names that differ from the contract: `get_value`/`set_value` (Object already has `get`/`set`),
`save_settings`/`load_settings`, and `setting_changed(id, value)`. `set_value` validates, clamps,
changes only its own key, returns whether it changed and marks the key as the player's choice.
Late-spawned nodes read the current value first, then connect to `setting_changed`.

## 4. Wiring into typical systems

```gdscript
# Player damage: one policy call at the moment of decision, per channel.
func take_hit(amount: float, source: String) -> void:   # "enemy", "hazard", "fall", "scripted"
	health -= A11yAssistPolicy.damage_to_player(amount, source)

func fell_into_pit() -> void:
	if A11yAssistPolicy.should_fail("fall"):
		die()
	else:
		respawn_at_last_safe_spot()   # never a soft-lock

func fire() -> void:
	if A11yAssistPolicy.consume("ammo", 1, self):   # spends self.ammo unless infinite
		spawn_bullet()
	else:
		play_empty_click()

# Countdown that is not a race:
var limit := base_limit * A11yAssistPolicy.timer_scale()   # INF when timers are off
```

```gdscript
# Camera owner: every shake source, including cutscenes, goes through the scale.
@onready var comfort: A11yCameraComfort = $Comfort   # its `camera` export keeps FOV in sync
func add_shake(strength: float) -> void:
	trauma = minf(trauma + comfort.scale_shake(strength), 1.0)
func _process(delta: float) -> void:
	camera.v_offset = comfort.scale_bob(bob_curve(walk_time))
	vignette.material.set_shader_parameter("strength", base_vignette * comfort.effects_scale())
```

```gdscript
# Quick-time event and timing windows.
@onready var input_assist: A11yInputAssist = $InputAssist
func start_door_qte() -> void:
	var window := input_assist.qte("door_kick", 1.2)
	if window == A11yInputAssist.QTE_AUTO:
		await input_assist.qte_auto_succeeded
		door_kicked()                 # the normal success branch
		return
	run_prompt(window)                # INF = wait until pressed
func parry_window() -> float:
	return input_assist.timing_window(0.15)   # simulation seconds
func _physics_process(_delta: float) -> void:
	aiming = input_assist.is_held(&"aim")     # hold or toggle, per input.hold_mode
	if struggling and input_assist.mash_progress(&"struggle", 12) >= 1.0:
		break_free()
```

Timing: measure windows in simulation time (the scaled `delta` gameplay already uses) and apply
`input.timing_window_scale` once. A window then lasts `base × window_scale / time_scale` real
seconds (`timing_window_real()`). Never also multiply it by the game speed.

```gdscript
# Dialogue -> captions. Text comes from the event, so it works with audio muted.
func say(speaker: String, line_key: String, voice: AudioStream) -> void:
	A11yCaptionBus.find(self).post(A11yCaptionEvent.dialogue(speaker, line_key, voice.get_length()))
	voice_player.stream = voice
	voice_player.play()

func alarm_started() -> void:   # looping sound: one caption, refreshed by merge_key
	var event := A11yCaptionEvent.sound("Alarm blaring", "alarm", 4.0)
	event.has_source_position = true
	event.source_position = alarm.global_position   # bus.listener turns this into "left"/"behind"
	A11yCaptionBus.find(self).post(event)
```

Put one `A11yCaptionBus` and one `A11yCaptionView` in the HUD `CanvasLayer` (bus first). Use
"Unknown voice" for speakers the player cannot identify yet.

```gdscript
# Enemy spawn with a content variant (scene tree):
# Spider (CharacterBody3D, AI, hitbox, damage)   <- unchanged by the filter
#   └ Looks (A11yContentVariant: setting_id="content.spiders", tag="arachnid")
#        ├ 0  original mesh, animations, sounds
#        ├ 1  no hair or fangs
#        ├ 2  no legs
#        └ 3  non-spider shape and sounds
var spider := SpiderScene.instantiate()
add_child(spider)   # Looks swaps in _ready, before the first frame; live changes swap instantly
```

`content.blood` and `content.gore` are separate keys: use one variant node per key. Name children by
enum value (`full`, `reduced`, `off`). A missing level falls back to the more filtered one.

```gdscript
# Guidance decals: tag by channel in the scene or in code. Hazards get their own group.
ledge_paint_decal.add_to_group(A11yGuidanceLayer.GROUP_PAINT)      # guidance.paint
breadcrumb_trail.add_to_group(A11yGuidanceLayer.GROUP_TRAIL)       # guidance.trail
pickup_outline.add_to_group(A11yGuidanceLayer.GROUP_HIGHLIGHTS)    # guidance.highlights
lava_edge_warning.add_to_group(A11yGuidanceLayer.GROUP_HAZARD)     # never hidden
func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed(&"hint"):
		guidance_layer.show_on_request()
```

Channel keys default to `follow` (use `guidance.level`). Nodes with a `set_guidance_intensity(x)`
method receive the intensity (0, 0.4, 1, 1.5) so a shader can thicken outlines when boosted.

```gdscript
# Every flash goes through the one FlashGuard.
func lightning() -> void:
	var guard := A11yFlashGuard.find(self)
	var strength := guard.request_flash(0.9, 1.0)   # intensity, screen fraction
	if strength <= 0.0:
		return                                   # over 3 per 1 s or 9 per 5 s: skip it
	var tint := A11yFlashGuard.desaturate_red(flash_color)
	var fade := 0.6 if guard.is_reduced() else 0.08  # reduced: slow, low-contrast fade
	flash_rect.color = Color(tint, strength)
	create_tween().tween_property(flash_rect, "color:a", 0.0, fade)
```

FlashGuard cannot see flashes baked into video or textures. Analyse captured footage (see
[photosensitivity.md](../../references/topics/photosensitivity.md)); without an analyser the check
is `blocked`.

Colours: scripts ask `tokens.get_color("team.enemy")` and always show `tokens.get_symbol(...)`.
The `custom` profile reads `visual.custom_colors` (`role=#RRGGBB;...`). Other keys a game reads
directly: `hud.mode` / `store.get_list("hud.hidden_elements")`, `visual.background_dim`,
`input.target_lock`, `A11yAssistPolicy.encounter_rate()`, `auto_battle()`, `battle_speed()`.

## 5. First launch, title and pause

Boot order (architecture.md section 10):

1. `boot.tscn`: static screen, no audio. The `A11y` autoload has already loaded and migrated the
   file and imported OS preferences (only for keys the player never set).
2. If `user://accessibility.cfg` did not exist, show the first-launch screen: an
   `A11ySettingsMenu` (or a short screen with text size, subtitles, narration, volume, reduce
   motion/flashing and presets). Keyboard, controller and TTS operable; skippable.
3. Logos and intro respect the settings; music fades in after volume is reachable.
4. Title: "Accessibility" always visible. Pause: Resume, Settings, Assists, Skip challenge (if
   `A11yAssistPolicy.skip_offered()`), Restart checkpoint, Quit.

```gdscript
func open_accessibility() -> void:   # same code on title and pause
	var menu := A11ySettingsMenu.new()
	$MenuLayer.add_child(menu)       # CanvasLayer; the menu processes while paused
	menu.close_requested.connect(menu.queue_free)
	menu.open()                       # focuses the first control and announces the title
```

Skips: connect `A11y.skip_requested` to the challenge's normal success path (same rewards, flags,
achievements and checkpoint). Achievement code never reads assist state.

## 6. Remapping with InputMap

- Gameplay reads actions only. Rebind with `InputMap.action_erase_events(action)` /
  `InputMap.action_erase_event(action, event)` and `InputMap.action_add_event(action, event)`.
  Allow more than one event per action and cover menus (`ui_*`), minigames and pause.
- Save bindings in the same file: `A11y.capture_bindings(actions)` stores key, mouse, joypad button
  and axis events as plain data in `[bindings]` (ConfigFile cannot store InputEvent objects), and
  `A11y.restore_bindings()` re-applies them after `load_settings()`.
- Build prompts from `InputMap.action_get_events(action)` every time they are shown, so they follow
  remaps and device changes.
- Keep a recovery route: `InputMap.load_from_project_settings()` restores the defaults; bind it to a
  fixed key or a timed revert after rebinding confirm.

## 7. Text size, fonts and the accessibility tree

- Scale text through the theme, not a root transform:
  `A11ySettingsMenu.apply_text_scale_to_theme(game_theme, 22, A11y.get_float("text.scale", 1.0))`
  sets `default_font_size`. Remove hard-coded `theme_override_font_sizes`, use containers and
  `autowrap_mode` so layouts reflow, and re-run it from `setting_changed`.
- `text.font`: keep one Font resource per value (`default`, `readable`, `dyslexia_friendly`) and
  assign it to `theme.default_font`. Ship only fonts whose licence allows it.
- Godot 4.5+/Redot 26.1+: give icon-only controls `accessibility_name`; put the one-line effect in
  `accessibility_description`. `A11yNarrator.attach_to_tree(root)` fills empty names and narrates
  focus ("Subtitles, switch, on"). When a screen reader is active it skips self-voicing focus so
  nothing is spoken twice.

## 8. Export

- Check the exported build reads `settings_registry.json`. If the log says "cannot read registry",
  add `*.json` to Export > Resources > "Filters to export non-resource files".
- TTS depends on OS voices: Windows SAPI, macOS AVSpeech, Linux speech-dispatcher (must be
  installed), Android/iOS/Web vary. Handle an empty voice list; the narrator then stays silent.
- AccessKit coverage differs per platform and engine build. Test the exported build with NVDA or
  Narrator (Windows) and VoiceOver (macOS) before claiming screen-reader support.
- Keep `colorblind_sim.gdshader` out of release builds; it is a developer tool.
