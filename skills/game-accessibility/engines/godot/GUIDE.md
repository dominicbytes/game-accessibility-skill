# Godot 4 / Redot: wiring the accessibility services

For coding agents. The sample is `addons/game_accessibility/`, the contract is
[architecture.md](../../references/architecture.md), and the rules are
[core-policy.md](../../references/core-policy.md).

**Verified on:** Redot 26.3 (`26.3.rc.3.custom_build.ccb92435b`, Godot 4.5 based), Windows,
headless: tests pass **43/43**, also with every default GDScript warning (plus untyped declarations)
promoted to an error. Earlier rounds also passed on `26.3.stable.official.85e520394`. Command, run from the repository root:
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

1. Copy `addons/game_accessibility/` into the project. Enable the plugin (adds autoload `A11y`). Keep the skill's `LICENSE` file (MIT) with the copied code.
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
	camera.v_offset = comfort.scale_bob(bob_curve(walk_time))          # motion.head_bob
	weapon.position = rest + sway_offset(walk_time) * comfort.scale_sway(1.0)  # motion.weapon_sway, visual only
	vignette.material.set_shader_parameter("strength", base_vignette * comfort.effects_scale())
	# motion.fov_effects: write the FOV every frame from the player's current base FOV, so turning
	# the effect off mid-sprint returns to the base FOV. Zoom pulses: comfort.scale_zoom_pulse(amount).
	var base_fov: float = A11y.get_float("motion.fov", 75.0)
	var fov_kick: float = sprint_fov_kick if sprinting else 0.0
	camera.fov = base_fov + (fov_kick if comfort.allow_fov_effects() else 0.0)
	speed_lines.visible = comfort.allow_speed_lines() and speed > 20.0  # motion.speed_lines
```

`motion.reduce_motion` is the umbrella. Each automatic or decorative motion has its own override
(`follow` / `off` / `on`; `on` means the effect is allowed), resolved with
`follow_value = not reduce_motion`: `allow_auto_camera()` (`motion.auto_camera`),
`allow_fov_effects()`, `allow_speed_lines()`, `allow_parallax()` and `allow_menu_animation()`.
Zoom pulses are FOV effects: `scale_zoom_pulse()` resolves through `allow_fov_effects()`, so an
explicit `motion.fov_effects` `on` or `off` wins over the umbrella. When FOV effects stop being
allowed, the comfort node's `camera` is reset to `motion.fov` at once.

Any `follow`/`off`/`on` key resolves through the store:
`A11y.resolve_toggle(key, follow_value)` returns true for `on`, false for `off`, and
`follow_value` for `follow` or a key pruned from the registry. There is no outline component in
this sample; an outline shader or material resolves its class like this:

```gdscript
# visual.high_contrast is the umbrella; each outline class can override it.
func outline_on(outline_class: String) -> bool:   # "enemies", "allies", "interactables", "hazards"
	return A11y.resolve_toggle("visual.outline_" + outline_class, A11y.get_bool("visual.high_contrast"))
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

`A11yCaptionView` never clips text. It splits each line into pages of at most 2 lines that fit the
current width and `subtitles.size`, and re-splits them (keeping the reading position) when the view,
viewport or size changes. Pages turn by time: each page gets its share of `duration`, never less
than its reading time (`reading_chars_per_second`, default 15) or `min_page_seconds`. A line that
the scene can hold (`event.can_wait = true`, for example a dialogue box) waits for the player while
`text.auto_advance` is off: call `view.advance_page()` from the continue action, and check
`view.is_waiting()` before moving the conversation on. `entry_finished(event, fully_shown)` fires
when an entry leaves. Turning `subtitles.dialogue` or `subtitles.sound_captions` off hides active
entries of that kind at once. Direction is independent of speaker names: `Mara (left): ...`, or
`(left) ...` with names off.

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
enum value (`full`, `reduced`, `off`). A missing level uses the next MORE filtered variant, never a
less filtered one. If nothing at or above the requested level exists, the presentation is hidden
(or the explicitly authored `neutral_fallback` node is shown) and a warning lists the missing
levels; the `coverage_missing` signal reports the same. Run `missing_levels()` on every variant
node in a scene test and author the gaps before shipping.

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
	var flash := guard.request_flash_color(flash_color, 0.9, 1.0)   # colour, intensity, screen fraction
	if flash.a <= 0.0:
		return                                   # over 3 per 1 s or 9 per 5 s: skip it
	var fade := 0.6 if guard.is_reduced() else 0.08  # reduced: slow, low-contrast fade
	flash_rect.color = flash                     # red already desaturated, alpha = allowed intensity
	create_tween().tween_property(flash_rect, "color:a", 0.0, fade)
```

Counting rule: a request counts toward the budgets when `area >= 0.25` OR `intensity >= 0.1` OR it
is saturated red (`request_flash(intensity, area, at_time, saturated_red)`, or let
`request_flash_color()` classify the colour). Only small, dim, non-red requests go uncounted. The
thresholds (`area_threshold`, `intensity_threshold`) are conservative sample values: change them
only with evidence from a photosensitivity analyser. The budgets are hard-capped at 3 per 1 s and
9 per 5 s whatever the exports say; the exports can only lower them. Saturated red follows WCAG on
linearised sRGB channels (`R / (R + G + B) >= 0.8`) and always goes through `desaturate_red()`.

FlashGuard cannot see flashes baked into video or textures. Analyse captured footage (see
[photosensitivity.md](../../references/topics/photosensitivity.md)); without an analyser the check
is `blocked`.

Colours: scripts ask `tokens.get_color("team.enemy")` and always show `tokens.get_symbol(...)`.
The `custom` profile reads `visual.custom_colors` (`role=#RRGGBB;...`). Other keys a game reads
directly: `hud.mode` / `store.get_list("hud.hidden_elements")`, `visual.background_dim`,
`input.target_lock`, `A11yAssistPolicy.encounter_rate()`, `auto_battle()`, `battle_speed()`.

- Aim: three separate floats, none bundled. `input.aim_assist` is slowdown only (slow the aim
  while it is over a target), `input.aim_pull` pulls toward a nearby target, and
  `input.aim_stabilization` smooths small unintended movement and reduces recoil and sway while
  aiming. Read each where its effect is applied.
- Scares: `content.jump_scares` is `full` or `reduced` (softer stings and camera snaps).
  `content.scare_warnings` (bool) is separate: show and announce a consistent heads-up before a
  scripted scare at either intensity.

## 5. First launch, title and pause

Boot order (architecture.md section 10):

1. `boot.tscn`: static screen, no audio. The `A11y` autoload has already loaded and migrated the
   file and imported OS preferences (only for keys the player never set).
2. If `user://accessibility.cfg` did not exist, show the first-launch screen: an
   `A11ySettingsMenu` (or a short screen with text size, subtitles, narration, volume, reduce
   motion/flashing and presets). Keyboard, controller and TTS operable; skippable.
3. Logos and intro respect the settings; music fades in after volume is reachable.
4. Title: "Accessibility" always visible. Pause: Resume, Settings, Assists, Skip challenge (if
   `A11yAssistPolicy.skip_offered()`), Restart checkpoint, Quit. Before a scene listed in the
   content notices: Skip scene (if `A11yAssistPolicy.scene_skip_offered()`).

```gdscript
func open_accessibility() -> void:   # same code on title and pause
	var menu := A11ySettingsMenu.new()
	$MenuLayer.add_child(menu)       # CanvasLayer; the menu processes while paused
	menu.close_requested.connect(menu.queue_free)
	menu.open()                       # focuses the first control and announces the title
```

Skips: two separate requests, each gated on its own key, reaching the same completion signal.
`A11yAssistPolicy.request_skip(challenge_id)` (puzzles, chases, bosses) follows
`assist.skip_available`; `A11yAssistPolicy.request_scene_skip(scene_id)` (sensitive scenes; then
show the scene's text summary) follows `content.scene_skip` only. Connect `A11y.skip_requested` to
the normal success path (same rewards, flags, achievements and checkpoint). It fires once per id;
repeated requests do nothing. A skip whose own key is pruned from the registry is never offered
(the registry default applies only to a registered key). Call `A11y.clear_skip(id)` when the
player replays that challenge or scene. Achievement code never reads assist state.

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
  focus and value changes ("Subtitles, switch, on"). When a screen reader is active it skips
  self-voicing of both, and checks again before speaking anything already queued, so nothing is
  spoken twice.
- The narrator has two independent channels. Menu speech (`speak()`, focus, values) follows
  `narration.enabled`; unvoiced dialogue (`speak_dialogue()`, used by `A11yCaptionBus`) follows
  `narration.dialogue`. Turning `narration.enabled` off stops current menu speech and discards
  queued menu speech at once; dialogue reading carries on. A custom `backend` can add
  `backend_is_speaking` and `backend_stop` so it queues and stops like OS TTS.

## 8. Export

- Check the exported build reads `settings_registry.json`. If the log says "cannot read registry",
  add `*.json` to Export > Resources > "Filters to export non-resource files".
- TTS depends on OS voices: Windows SAPI, macOS AVSpeech, Linux speech-dispatcher (must be
  installed), Android/iOS/Web vary. Handle an empty voice list; the narrator then stays silent.
- AccessKit coverage differs per platform and engine build. Test the exported build with NVDA or
  Narrator (Windows) and VoiceOver (macOS) before claiming screen-reader support.
- Keep `colorblind_sim.gdshader` out of release builds; it is a developer tool.
