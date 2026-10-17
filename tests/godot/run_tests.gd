## Headless tests for the game-accessibility Godot/Redot sample.
## Run through tests/run_godot_tests.sh (copies the addon, imports, runs this script).
## Prints PASS/FAIL per test and a summary; exits with code 1 when any test fails.
extends SceneTree

const TMP_DIR: String = "user://a11y_tests"
const ADDON: String = "res://addons/game_accessibility"
const ACTION: StringName = &"a11y_test_action"


class AmmoHolder:
	var ammo: int = 3


var _passed: int = 0
var _failed: int = 0
var _errors: PackedStringArray = PackedStringArray()
var _store: A11ySettings = null
var _cleanup: Array[Node] = []
var _tmp_index: int = 0


func _initialize() -> void:
	_run_all()


func _run_all() -> void:
	# The root enters the tree after _initialize(); wait so added nodes get _ready() as in a game.
	await process_frame
	DirAccess.make_dir_recursive_absolute(TMP_DIR)
	var script: Script = get_script() as Script
	var names: PackedStringArray = PackedStringArray()
	for method: Dictionary in script.get_script_method_list():
		var method_name: String = method["name"]
		if method_name.begins_with("test_") and not names.has(method_name):
			names.append(method_name)
	for test_name: String in names:
		_errors = PackedStringArray()
		_setup()
		await call(test_name)
		_teardown()
		if _errors.is_empty():
			_passed += 1
			print("PASS  %s" % test_name)
		else:
			_failed += 1
			print("FAIL  %s: %s" % [test_name, "; ".join(_errors)])
	_remove_tmp()
	await process_frame
	print("")
	print("Summary: %d passed, %d failed, %d total" % [_passed, _failed, _passed + _failed])
	quit(1 if _failed > 0 else 0)


# --- Helpers --------------------------------------------------------------------------------------

func _setup() -> void:
	_store = _new_store()
	A11ySettings.set_active(_store)
	if not InputMap.has_action(ACTION):
		InputMap.add_action(ACTION)


func _teardown() -> void:
	for node: Node in _cleanup:
		if is_instance_valid(node):
			node.free()
	_cleanup.clear()
	A11ySettings.set_active(null)
	if is_instance_valid(_store):
		_store.free()
	_store = null
	if InputMap.has_action(ACTION):
		InputMap.erase_action(ACTION)


func _new_store(path: String = "") -> A11ySettings:
	var store: A11ySettings = A11ySettings.new()
	store.warn_on_invalid = false
	store.autosave = false
	store.auto_load = false
	if path.is_empty():
		_tmp_index += 1
		path = "%s/settings_%d.cfg" % [TMP_DIR, _tmp_index]
	store.storage_path = path
	return store


func _add(node: Node) -> Node:
	root.add_child(node)
	_cleanup.append(node)
	return node


func _check(condition: bool, message: String) -> void:
	if not condition:
		_errors.append(message)


func _eq(actual: Variant, expected: Variant, message: String) -> void:
	var same: bool = actual == expected
	if (actual is float or actual is int) and (expected is float or expected is int):
		same = is_equal_approx(float(actual), float(expected)) or (is_inf(float(actual)) and is_inf(float(expected)))
	if typeof(actual) != typeof(expected) and not ((actual is float or actual is int) and (expected is float or expected is int)):
		same = false
	if not same:
		_errors.append("%s (expected %s, got %s)" % [message, var_to_str(expected), var_to_str(actual)])


func _action_event(pressed: bool) -> InputEventAction:
	var event: InputEventAction = InputEventAction.new()
	event.action = ACTION
	event.pressed = pressed
	return event


func _write_cfg(path: String, version: int, values: Dictionary) -> void:
	var cfg: ConfigFile = ConfigFile.new()
	cfg.set_value("meta", "schema_version", version)
	for id: String in values:
		cfg.set_value(id.get_slice(".", 0), id.substr(id.find(".") + 1), values[id])
	cfg.save(path)


func _remove_tmp() -> void:
	var dir: DirAccess = DirAccess.open(TMP_DIR)
	if dir == null:
		return
	for file: String in dir.get_files():
		dir.remove(file)
	DirAccess.remove_absolute(TMP_DIR)


# --- Settings store -------------------------------------------------------------------------------

func test_registry_loads() -> void:
	var data: Variant = JSON.parse_string(FileAccess.get_file_as_string(ADDON + "/settings_registry.json"))
	_check(data is Dictionary, "registry JSON parses")
	var settings: Array = (data as Dictionary).get("settings", [])
	_eq(_store.get_ids().size(), settings.size(), "every registry key loaded")
	_eq(_store.get_presets().size(), (data as Dictionary).get("presets", []).size(), "every preset loaded")
	_check(_store.get_categories().has("motion"), "categories include motion")
	for id: String in _store.get_ids():
		var def: Dictionary = _store.get_definition(id)
		_check(not str(def.get("label", "")).is_empty(), "%s has a label" % id)
		_check(not str(def.get("description", "")).is_empty(), "%s has a description" % id)


func test_defaults() -> void:
	for id: String in _store.get_ids():
		_check(A11ySettings._same(_store.get_value(id), _store.get_default(id)), "%s starts at its default" % id)
		_check(not _store.is_explicit(id), "%s starts not explicit" % id)
	_eq(_store.get_float("text.scale"), 1.0, "text.scale default")
	_eq(_store.get_bool("subtitles.dialogue"), true, "subtitles on by default")
	_eq(_store.get_float("motion.camera_shake"), 0.5, "camera shake reduced by default")
	_eq(_store.get_float("audio.master"), 0.7, "quiet master volume")
	_eq(_store.get_int("content.spiders"), 0, "content.spiders is an int")
	_eq(_store.get_float("missing.key", 0.25), 0.25, "pruned key returns fallback")


func test_set_clamp_and_validation() -> void:
	_check(_store.set_value("text.scale", 5.0), "out-of-range float accepted")
	_eq(_store.get_float("text.scale"), 2.0, "float clamped to max")
	_check(not _store.set_value("text.scale", "huge"), "string rejected for float")
	_eq(_store.get_float("text.scale"), 2.0, "rejected value leaves key unchanged")
	_check(not _store.set_value("text.scale", NAN), "NaN rejected")
	_check(_store.set_value("input.qte_mode", "auto"), "valid enum accepted")
	_check(not _store.set_value("input.qte_mode", "turbo"), "unknown enum value rejected")
	_eq(_store.get_string("input.qte_mode"), "auto", "enum kept after rejection")
	_check(_store.set_value("content.spiders", 2.6), "float accepted for int key")
	_eq(_store.get_value("content.spiders"), 3, "int key rounded")
	_check(not _store.set_value("subtitles.dialogue", 1), "int rejected for bool")
	_check(not _store.set_value("no.such_key", true), "unknown id rejected")
	_check(_store.set_value("hud.hidden_elements", "minimap, damage_numbers"), "string accepted")
	_check(not _store.set_value("hud.hidden_elements", 3), "number rejected for string")
	_eq(_store.get_list("hud.hidden_elements"), PackedStringArray(["minimap", "damage_numbers"]), "string list helper")
	_check(_store.set_value("motion.fov", 10.0), "fov accepted")
	_eq(_store.get_float("motion.fov"), 60.0, "fov clamped to min")


func test_per_key_isolation() -> void:
	var before: Dictionary = {}
	for id: String in _store.get_ids():
		before[id] = _store.get_value(id)
	_store.set_value("motion.camera_shake", 0.0)
	for id: String in _store.get_ids():
		if id != "motion.camera_shake":
			_check(A11ySettings._same(_store.get_value(id), before[id]), "%s untouched by camera_shake" % id)
	_check(_store.is_explicit("motion.camera_shake"), "touched key explicit")
	_check(not _store.is_explicit("motion.head_bob"), "neighbour key not explicit")


func test_changed_signal_only_on_change() -> void:
	var got: Array = []
	_store.setting_changed.connect(func(id: String, value: Variant) -> void: got.append([id, value]))
	_check(not _store.set_value("audio.music", 0.6), "setting the same value returns false")
	_eq(got.size(), 0, "no signal for unchanged value")
	_check(_store.set_value("audio.music", 0.3), "new value returns true")
	_eq(got.size(), 1, "one signal for one change")
	if got.size() == 1:
		_eq(got[0][0], "audio.music", "signal id")
		_eq(got[0][1], 0.3, "signal value")
	_store.set_value("audio.music", 0.3)
	_store.set_value("audio.music", "loud")
	_eq(got.size(), 1, "no signal for repeat or rejected value")


func test_persistence_round_trip() -> void:
	_store.set_value("text.scale", 1.5)
	_store.set_value("input.qte_mode", "slow")
	_store.set_value("subtitles.speaker_names", false)
	_store.set_value("content.spiders", 2)
	var key: InputEventKey = InputEventKey.new()
	key.physical_keycode = KEY_SPACE
	InputMap.action_add_event(ACTION, key)
	var pad: InputEventJoypadButton = InputEventJoypadButton.new()
	pad.button_index = JOY_BUTTON_A
	InputMap.action_add_event(ACTION, pad)
	_store.capture_bindings(PackedStringArray([String(ACTION)]))
	_eq(_store.save_settings(), OK, "save returns OK")
	_check(FileAccess.file_exists(_store.storage_path), "config file written")
	_check(not FileAccess.file_exists(_store.storage_path + ".tmp"), "temp file renamed away")
	var other: A11ySettings = _new_store(_store.storage_path)
	_eq(other.load_settings(), OK, "load returns OK")
	_eq(other.get_float("text.scale"), 1.5, "float restored")
	_eq(other.get_string("input.qte_mode"), "slow", "enum restored")
	_eq(other.get_bool("subtitles.speaker_names", true), false, "bool restored")
	_eq(other.get_value("content.spiders"), 2, "int restored")
	_check(other.is_explicit("text.scale"), "explicit flag restored")
	_check(not other.is_explicit("audio.music"), "untouched key not explicit after load")
	_eq(other.get_float("audio.music"), 0.6, "untouched key at default")
	var cfg: ConfigFile = ConfigFile.new()
	cfg.load(_store.storage_path)
	_eq(cfg.get_value("meta", "schema_version", -1), A11ySettings.SCHEMA_VERSION, "schema version written")
	InputMap.action_erase_events(ACTION)
	_eq(other.restore_bindings(), PackedStringArray([String(ACTION)]), "bindings restored from the same file")
	var events: Array[InputEvent] = InputMap.action_get_events(ACTION)
	_eq(events.size(), 2, "both bindings restored")
	if events.size() == 2:
		_check(events[0] is InputEventKey and (events[0] as InputEventKey).physical_keycode == KEY_SPACE, "key binding restored")
		_check(events[1] is InputEventJoypadButton and (events[1] as InputEventJoypadButton).button_index == JOY_BUTTON_A, "pad binding restored")
	_check(not other.has_setting("bindings.jump"), "bindings are not settings")
	_eq(cfg.get_value("text", "scale", 0.0), 1.5, "human-readable [text] scale entry")
	other.free()


func test_invalid_value_repaired_alone() -> void:
	_write_cfg(_store.storage_path, A11ySettings.SCHEMA_VERSION, {
		"text.scale": "huge",
		"audio.music": 0.3,
		"motion.camera_shake": 7.0,
		"input.qte_mode": "warp",
		"subtitles.size": 1.5,
	})
	_eq(_store.load_settings(), OK, "load OK")
	_eq(_store.get_float("text.scale"), 1.0, "invalid value back to default")
	_eq(_store.get_float("audio.music"), 0.3, "valid neighbour kept")
	_eq(_store.get_float("subtitles.size"), 1.5, "second valid neighbour kept")
	_eq(_store.get_float("motion.camera_shake"), 1.0, "out-of-range value clamped")
	_eq(_store.get_string("input.qte_mode"), "normal", "invalid enum back to default")
	_check(_store.last_repaired.has("text.scale"), "text.scale reported repaired")
	_check(_store.last_repaired.has("input.qte_mode"), "qte_mode reported repaired")
	_check(_store.last_repaired.has("motion.camera_shake"), "camera_shake reported repaired")
	_check(not _store.last_repaired.has("audio.music"), "valid key not reported")
	_check(not _store.is_explicit("text.scale"), "repaired-to-default key not explicit")


func test_schema_migration_hook() -> void:
	_write_cfg(_store.storage_path, 0, {"motion.shake_off": true, "audio.music": 0.4})
	_store.migration_hook = func(from_version: int, id: String, value: Variant) -> Dictionary:
		if from_version == 0 and id == "motion.shake_off":
			return {"motion.camera_shake": 0.0 if value else 0.5}
		return {id: value}
	_eq(_store.load_settings(), OK, "load OK")
	_eq(_store.get_float("motion.camera_shake"), 0.0, "old key migrated by meaning")
	_check(_store.is_explicit("motion.camera_shake"), "migrated key keeps the player's choice")
	_eq(_store.get_float("audio.music"), 0.4, "unchanged key carried over")
	_check(_store.last_migrated.has("motion.shake_off"), "migration reported")
	_store.save_settings()
	var cfg: ConfigFile = ConfigFile.new()
	cfg.load(_store.storage_path)
	_eq(cfg.get_value("meta", "schema_version", -1), A11ySettings.SCHEMA_VERSION, "saved with current schema")
	_check(not cfg.has_section_key("motion", "shake_off"), "old key not written back")


func test_os_import_keeps_explicit_choice() -> void:
	_store.set_value("motion.camera_shake", 0.75)
	var changed: PackedStringArray = _store.import_os_preferences({"reduce_motion": true, "screen_reader_active": true})
	_eq(_store.get_float("motion.camera_shake"), 0.75, "explicit camera_shake kept")
	_eq(_store.get_float("motion.head_bob"), 0.0, "head_bob imported")
	_eq(_store.get_bool("motion.reduce_motion"), true, "reduce_motion imported")
	_eq(_store.get_bool("narration.enabled"), true, "screen reader turns narration on")
	_eq(_store.get_bool("visual.high_contrast"), false, "absent preference changes nothing")
	_check(not changed.has("motion.camera_shake"), "explicit key not reported changed")
	_check(not _store.is_explicit("motion.head_bob"), "imported value is not explicit")
	_store.set_value("motion.head_bob", 0.5)
	_store.reset("motion.head_bob")
	_eq(_store.get_float("motion.head_bob"), 0.0, "reset returns to the OS-imported baseline")
	var empty_import: PackedStringArray = _store.import_os_preferences({"reduce_motion": false})
	_eq(empty_import.size(), 0, "false preference imports nothing")
	var probe: Dictionary = A11ySettings.read_os_preferences()
	_check(probe is Dictionary, "OS probe runs without error")


func test_preset_apply_and_undo() -> void:
	_store.set_value("assist.timers", "off")
	var seen_batch: Array = []
	_store.setting_changed.connect(func(_id: String, _value: Variant) -> void:
		seen_batch.append(_store.get_string("input.qte_mode") == "slow" and is_equal_approx(_store.get_float("assist.game_speed"), 0.75)))
	_check(_store.apply_preset("more_time"), "preset applied")
	_eq(_store.get_float("assist.game_speed"), 0.75, "game speed from preset")
	_eq(_store.get_float("input.timing_window_scale"), 2.0, "timing windows from preset")
	_eq(_store.get_string("assist.timers"), "extended", "timers from preset")
	_check(not seen_batch.is_empty() and not seen_batch.has(false), "signals fire after the whole batch is stored")
	_check(_store.can_undo_preset(), "undo available")
	_store.set_value("assist.game_speed", 0.9)
	_check(_store.undo_last_preset(), "undo works")
	_eq(_store.get_float("assist.game_speed"), 0.9, "hand-set value survives undo")
	_eq(_store.get_float("input.timing_window_scale"), 1.0, "preset value undone")
	_eq(_store.get_string("input.qte_mode"), "normal", "qte mode undone")
	_eq(_store.get_string("assist.timers"), "off", "earlier explicit value restored")
	_check(_store.is_explicit("assist.timers"), "earlier explicit flag restored")
	_check(not _store.is_explicit("input.qte_mode"), "default key not explicit after undo")
	_check(not _store.can_undo_preset(), "nothing left to undo")
	_check(not _store.apply_preset("no_such_preset"), "unknown preset rejected")


func test_reset_category() -> void:
	_store.set_value("text.scale", 1.5)
	_store.set_value("text.font", "readable")
	_store.set_value("audio.music", 0.2)
	_check(_store.reset_category("text") >= 2, "reset reports changed keys")
	_eq(_store.get_float("text.scale"), 1.0, "text.scale reset")
	_eq(_store.get_string("text.font"), "default", "text.font reset")
	_check(not _store.is_explicit("text.scale"), "reset key not explicit")
	_eq(_store.get_float("audio.music"), 0.2, "other category untouched")


func test_autoload_flow_load_and_autosave() -> void:
	_write_cfg(_store.storage_path, A11ySettings.SCHEMA_VERSION, {"text.scale": 1.25})
	A11ySettings.set_active(null)
	var store: A11ySettings = _new_store(_store.storage_path)
	store.auto_load = true
	store.autosave = true
	store.autosave_delay = 0.0
	_add(store)
	_check(A11ySettings.active() == store, "store in the tree becomes the active store")
	_eq(store.get_float("text.scale"), 1.25, "_ready loads the config file")
	store.set_value("audio.music", 0.25)
	await process_frame
	await process_frame
	var cfg: ConfigFile = ConfigFile.new()
	cfg.load(store.storage_path)
	_eq(cfg.get_value("audio", "music", -1.0), 0.25, "change autosaved")
	_eq(cfg.get_value("text", "scale", -1.0), 1.25, "loaded value kept on autosave")


func test_audio_buses_and_mono() -> void:
	_store.set_value("audio.master", 0.5)
	_eq(AudioServer.get_bus_volume_db(0), linear_to_db(0.5), "master bus volume follows audio.master")
	_store.set_value("audio.master", 0.0)
	_check(AudioServer.is_bus_mute(0), "volume 0 mutes the bus")
	_store.set_value("audio.master", 0.7)
	_check(not AudioServer.is_bus_mute(0), "volume above 0 unmutes")
	_store.set_value("audio.mono", true)
	var index: int = A11ySettings.find_mono_effect(0)
	_check(index >= 0, "mono effect added to Master")
	if index >= 0:
		_check(AudioServer.is_bus_effect_enabled(0, index), "mono effect enabled")
		_store.set_value("audio.mono", false)
		_check(not AudioServer.is_bus_effect_enabled(0, index), "mono effect disabled")
		var effect: AudioEffectStereoEnhance = AudioServer.get_bus_effect(0, index) as AudioEffectStereoEnhance
		_check(effect != null and is_zero_approx(effect.pan_pullout), "mono uses pan_pullout 0")


# --- Assist policy --------------------------------------------------------------------------------

func test_assist_damage_channels() -> void:
	_store.set_value("assist.damage_taken", 0.0)
	_eq(A11yAssistPolicy.damage_to_player(10.0, "enemy"), 0.0, "enemy damage blocked")
	_eq(A11yAssistPolicy.damage_to_player(10.0, "hazard"), 0.0, "hazard damage blocked")
	_eq(A11yAssistPolicy.damage_to_player(10.0, "fall"), 10.0, "fall damage not blocked by damage_taken")
	_check(A11yAssistPolicy.should_fail("fall"), "pits still fail without no_fall_death")
	_store.set_value("assist.no_fall_death", true)
	_eq(A11yAssistPolicy.damage_to_player(10.0, "fall"), 0.0, "no_fall_death blocks fall damage")
	_check(not A11yAssistPolicy.should_fail("fall"), "no_fall_death prevents fall failure")
	_eq(A11yAssistPolicy.damage_to_player(5.0, "scripted"), 5.0, "scripted damage unchanged")
	_store.set_value("assist.damage_taken", 0.5)
	_eq(A11yAssistPolicy.damage_to_player(10.0, "enemy"), 5.0, "damage scaled")
	_check(A11yAssistPolicy.should_fail("detected"), "other failure reasons unchanged")


func test_infinite_ammo_consume() -> void:
	var pool: Dictionary = {"ammo": 2, "stamina": 1.0}
	_check(A11yAssistPolicy.consume("ammo", 1, pool), "normal spend allowed")
	_eq(pool["ammo"], 1, "ammo spent")
	_store.set_value("assist.infinite_ammo", true)
	_check(A11yAssistPolicy.consume("ammo", 1, pool), "infinite spend allowed")
	_eq(pool["ammo"], 1, "infinite ammo not spent")
	_check(A11yAssistPolicy.consume("ammo", 5, pool), "infinite allows more than available")
	_check(A11yAssistPolicy.consume("stamina", 0.5, pool), "stamina still spends")
	_eq(pool["stamina"], 0.5, "stamina is separate from ammo")
	_store.set_value("assist.infinite_ammo", false)
	_check(A11yAssistPolicy.consume("ammo", 1, pool), "last round")
	_check(not A11yAssistPolicy.consume("ammo", 1, pool), "empty refuses")
	_eq(pool["ammo"], 0, "never negative, never INF")
	var holder: AmmoHolder = AmmoHolder.new()
	_check(A11yAssistPolicy.consume("ammo", 2, holder), "object holder works")
	_eq(holder.ammo, 1, "object property spent")


func test_timers_speed_aggression_skip() -> void:
	_eq(A11yAssistPolicy.timer_scale(), 1.0, "normal timers")
	_store.set_value("assist.timers", "extended")
	_eq(A11yAssistPolicy.timer_scale(), 2.0, "extended doubles")
	_store.set_value("assist.timers", "off")
	_check(is_inf(A11yAssistPolicy.timer_scale()), "timers off -> INF")
	_check(not A11yAssistPolicy.should_fail("timer"), "timers off never fail")
	_store.set_value("assist.game_speed", 0.7)
	_eq(A11yAssistPolicy.time_scale(), 0.7, "time scale")
	_store.set_value("assist.enemy_aggression", 0.25)
	_eq(A11yAssistPolicy.enemy_aggression(), 0.25, "aggression")
	_eq(A11yAssistPolicy.encounter_rate(), 1.0, "encounter rate default")
	_store.set_value("assist.encounter_rate", 0.0)
	_eq(A11yAssistPolicy.encounter_rate(), 0.0, "random battles off")
	_check(not A11yAssistPolicy.auto_battle(), "auto battle off by default")
	_store.set_value("assist.auto_battle", true)
	_check(A11yAssistPolicy.auto_battle(), "auto battle on")
	_store.set_value("assist.battle_speed", 2.0)
	_eq(A11yAssistPolicy.battle_speed(), 2.0, "battle speed")
	_check(A11yAssistPolicy.scene_skip_offered(), "content scene skip offered by default")
	var skipped: Array = []
	_store.skip_requested.connect(func(challenge: String) -> void: skipped.append(challenge))
	_check(A11yAssistPolicy.request_skip("bridge_puzzle"), "skip requested")
	_eq(skipped, ["bridge_puzzle"], "skip signal carries the challenge id")
	_store.set_value("assist.skip_available", false)
	_check(not A11yAssistPolicy.request_skip("bridge_puzzle"), "skip refused when not offered")


# --- Input assist ---------------------------------------------------------------------------------

func test_input_assist_toggle_latch() -> void:
	var assist: A11yInputAssist = _add(A11yInputAssist.new()) as A11yInputAssist
	_check(not assist.is_held(ACTION), "hold mode: not held without input")
	_store.set_value("input.hold_mode", "toggle")
	assist.handle_event(_action_event(true))
	_check(assist.is_held(ACTION), "first press latches")
	assist.handle_event(_action_event(false))
	_check(assist.is_held(ACTION), "release keeps the latch")
	assist.handle_event(_action_event(true))
	_check(not assist.is_held(ACTION), "second press releases")
	assist.handle_event(_action_event(true))
	assist.notification(Node.NOTIFICATION_APPLICATION_FOCUS_OUT)
	_check(not assist.is_held(ACTION), "focus loss clears the latch")
	assist.handle_event(_action_event(true))
	assist.notification(Node.NOTIFICATION_PAUSED)
	_check(not assist.is_held(ACTION), "pause clears the latch")
	assist.handle_event(_action_event(true))
	assist.clear_latches()
	_check(not assist.is_held(ACTION), "explicit clear")
	assist.handle_event(_action_event(true))
	_store.set_value("input.hold_mode", "hold")
	_store.set_value("input.hold_mode", "toggle")
	_check(not assist.is_held(ACTION), "mode change clears the latch")
	_eq(assist.timing_window(0.2), 0.2, "timing window default")
	_store.set_value("input.timing_window_scale", 2.5)
	_eq(assist.timing_window(0.2), 0.5, "timing window scaled once, in simulation time")
	_store.set_value("assist.game_speed", 0.5)
	_eq(assist.timing_window(0.2), 0.5, "game speed never multiplies the simulation window")
	_eq(assist.timing_window_real(0.2), 1.0, "real window = base x window_scale / time_scale")


func test_input_assist_mash_modes() -> void:
	var assist: A11yInputAssist = _add(A11yInputAssist.new()) as A11yInputAssist
	var completed: Array = []
	assist.mash_completed.connect(func(action: StringName) -> void: completed.append(action))
	_eq(assist.mash_progress(ACTION, 3), 0.0, "mash starts at 0")
	for i: int in 3:
		assist.handle_event(_action_event(true))
		assist.handle_event(_action_event(false))
	_eq(assist.mash_progress(ACTION), 1.0, "mash: three presses complete")
	_eq(completed.size(), 1, "completion emitted once")
	_store.set_value("input.mash_mode", "hold")
	assist.start_mash(ACTION, 10, 1.0)
	assist.handle_event(_action_event(true))
	assist.tick(0.5)
	_eq(assist.mash_progress(ACTION), 0.5, "hold: half filled after half the authored time")
	assist.handle_event(_action_event(false))
	assist.tick(1.0)
	_eq(assist.mash_progress(ACTION), 0.5, "hold: releasing pauses progress")
	assist.handle_event(_action_event(true))
	assist.tick(0.6)
	_check(assist.is_mash_complete(ACTION), "hold: completes")
	_store.set_value("input.mash_mode", "single")
	assist.start_mash(ACTION, 20)
	assist.handle_event(_action_event(true))
	_check(assist.is_mash_complete(ACTION), "single: one press completes")
	_eq(completed.size(), 3, "each sequence completed once")


func test_input_assist_qte_modes() -> void:
	var assist: A11yInputAssist = _add(A11yInputAssist.new()) as A11yInputAssist
	assist.auto_beat_seconds = 0.0
	_eq(assist.qte("door", 1.2), 1.2, "normal window")
	_store.set_value("input.qte_mode", "slow")
	_eq(assist.qte("door", 1.2), 3.6, "slow window at least 3x")
	_check(is_inf(assist.qte("door", 1.2, true)), "slow waits when nothing else moves")
	_store.set_value("input.qte_mode", "auto")
	var succeeded: Array = []
	assist.qte_auto_succeeded.connect(func(prompt: String) -> void: succeeded.append(prompt))
	_eq(assist.qte("door", 1.2), A11yInputAssist.QTE_AUTO, "auto returns QTE_AUTO")
	await process_frame
	_eq(succeeded, ["door"], "auto success emitted through the signal")


# --- Flash guard ----------------------------------------------------------------------------------

func test_flash_guard_rate_limit() -> void:
	var guard: A11yFlashGuard = _add(A11yFlashGuard.new()) as A11yFlashGuard
	_eq(guard.request_flash(1.0, 1.0, 10.0), 1.0, "flash 1 allowed")
	_eq(guard.request_flash(1.0, 1.0, 10.2), 1.0, "flash 2 allowed")
	_eq(guard.request_flash(1.0, 1.0, 10.4), 1.0, "flash 3 allowed")
	_eq(guard.request_flash(1.0, 1.0, 10.6), 0.0, "4th flash within 1 s blocked")
	_eq(guard.request_flash(0.3, 0.02, 10.7), 0.3, "small dim flash not counted")
	_eq(guard.request_flash(1.0, 1.0, 11.5), 1.0, "allowed again once the window passed")
	guard.reset()
	_store.set_value("motion.reduce_flashing", true)
	var reduced: float = guard.request_flash(1.0, 1.0, 20.0)
	_check(reduced > 0.0 and reduced <= 0.2, "reduce_flashing lowers intensity (got %s)" % reduced)
	guard.request_flash(1.0, 1.0, 20.1)
	guard.request_flash(1.0, 1.0, 20.2)
	_eq(guard.request_flash(1.0, 1.0, 20.3), 0.0, "rate limit also applies with reduce_flashing")
	_check(A11yFlashGuard.is_saturated_red(Color(1, 0, 0)), "pure red is saturated red")
	var softened: Color = A11yFlashGuard.desaturate_red(Color(1, 0, 0))
	_check(not A11yFlashGuard.is_saturated_red(softened), "desaturated red is no longer saturated red")
	_eq(A11yFlashGuard.desaturate_red(Color(0, 0, 1)), Color(0, 0, 1), "other colours unchanged")
	_check(A11yFlashGuard.find(guard) == guard, "guard found through its group")


func test_flash_guard_blocks_sustained_strobe() -> void:
	var guard: A11yFlashGuard = _add(A11yFlashGuard.new()) as A11yFlashGuard
	var allowed: Array[float] = []
	for i: int in 12:
		var t: float = 100.0 + i * 0.5
		if guard.request_flash(1.0, 1.0, t) > 0.0:
			allowed.append(t)
	_check(allowed.size() < 12, "2 flashes per second for 6 s is capped (allowed %d of 12)" % allowed.size())
	for t: float in allowed:
		var in_one: int = allowed.filter(func(other: float) -> bool: return other >= t - 1.0 and other <= t).size()
		var in_five: int = allowed.filter(func(other: float) -> bool: return other >= t - 5.0 and other <= t).size()
		_check(in_one <= 3, "at most 3 in the second ending at %s" % t)
		_check(in_five <= 9, "at most 9 in the 5 s ending at %s (got %d)" % [t, in_five])
	_check(not allowed.has(104.5), "10th flash inside 5 s blocked")
	_store.set_value("motion.reduce_flashing", true)
	guard.reset()
	var reduced_allowed: int = 0
	for i: int in 12:
		if guard.request_flash(1.0, 1.0, 200.0 + i * 0.5) > 0.0:
			reduced_allowed += 1
	_check(reduced_allowed <= 10, "5 s limit also applies with reduce_flashing")


# --- Captions -------------------------------------------------------------------------------------

func test_caption_view_muted_and_speaker_names() -> void:
	_store.set_value("audio.master", 0.0)
	var bus: A11yCaptionBus = _add(A11yCaptionBus.new()) as A11yCaptionBus
	var view: A11yCaptionView = _add(A11yCaptionView.new()) as A11yCaptionView
	_check(AudioServer.is_bus_mute(0), "audio muted for this test")
	bus.post(A11yCaptionEvent.dialogue("Mara", "Keep your head down."))
	_eq(view.get_visible_lines(), PackedStringArray(["Mara: Keep your head down."]), "subtitle shown with audio muted")
	_store.set_value("subtitles.speaker_names", false)
	_eq(view.get_visible_lines(), PackedStringArray(["Keep your head down."]), "speaker names off")
	view.clear()
	bus.post(A11yCaptionEvent.sound("Alarm blaring", "alarm"))
	_eq(view.get_visible_lines().size(), 0, "sound captions off by default")
	_store.set_value("subtitles.sound_captions", true)
	_store.set_value("subtitles.direction", true)
	var alarm: A11yCaptionEvent = A11yCaptionEvent.sound("Alarm blaring", "alarm")
	alarm.direction = "left"
	bus.post(alarm)
	bus.post(alarm)
	_eq(view.get_visible_lines(), PackedStringArray(["[Alarm blaring, left]"]), "looping sound merged, direction word shown")
	bus.post(A11yCaptionEvent.dialogue("Mara", "One."))
	bus.post(A11yCaptionEvent.dialogue("Mara", "Two."))
	_eq(view.get_visible_lines().size(), 2, "at most two entries")
	view.advance(10.0)
	_eq(view.get_visible_lines().size(), 0, "entries expire")
	_store.set_value("subtitles.dialogue", false)
	bus.post(A11yCaptionEvent.dialogue("Mara", "Hidden."))
	_eq(view.get_visible_lines().size(), 0, "subtitles off hides dialogue")
	_eq(A11yCaptionBus.direction_word(Transform3D.IDENTITY, Vector3(-5, 0, 0)), "left", "direction word left")
	_eq(A11yCaptionBus.direction_word(Transform3D.IDENTITY, Vector3(0, 0, 5)), "behind", "direction word behind")


# --- Content and guidance -------------------------------------------------------------------------

func test_content_variant_swaps_levels() -> void:
	var spider: Node2D = _add(Node2D.new()) as Node2D
	var hitbox: Node2D = Node2D.new()
	hitbox.name = "Hitbox"
	spider.add_child(hitbox)
	var variant: A11yContentVariant = A11yContentVariant.new()
	for level: String in ["0", "1", "2", "3"]:
		var look: Node2D = Node2D.new()
		look.name = level
		variant.add_child(look)
	spider.add_child(variant)
	var looks: Dictionary = {}
	for level: String in ["0", "1", "2", "3"]:
		looks[level] = variant.get_node(level)
	_check((looks["0"] as Node2D).visible and not (looks["2"] as Node2D).visible, "original shown at level 0")
	_store.set_value("content.spiders", 2)
	_check((looks["2"] as Node2D).visible, "level 2 shown immediately")
	_check(not (looks["0"] as Node2D).visible and not (looks["1"] as Node2D).visible and not (looks["3"] as Node2D).visible, "only one variant visible")
	_eq((looks["0"] as Node).process_mode, Node.PROCESS_MODE_DISABLED, "hidden variant stops processing")
	_check(hitbox.visible and hitbox.process_mode == Node.PROCESS_MODE_INHERIT, "hitbox outside the variant untouched")
	(looks["3"] as Node).free()
	_store.set_value("content.spiders", 3)
	_check(variant.current_variant() == looks["2"], "missing level falls back to the nearest available")
	var gore: A11yContentVariant = A11yContentVariant.new()
	gore.setting_id = "content.blood"
	gore.tag = "gore"
	for level: String in ["full", "off"]:
		var look: Node2D = Node2D.new()
		look.name = level
		gore.add_child(look)
	_add(gore)
	_store.set_value("content.blood", "reduced")
	_eq(String(gore.current_variant().name), "off", "missing enum level falls back to the more filtered variant")


func test_guidance_off_hides_guidance_not_hazard() -> void:
	var paint: Node2D = _add(Node2D.new()) as Node2D
	paint.add_to_group(A11yGuidanceLayer.GROUP_GUIDANCE)
	var hazard_paint: Node2D = _add(Node2D.new()) as Node2D
	hazard_paint.add_to_group(A11yGuidanceLayer.GROUP_GUIDANCE)
	hazard_paint.add_to_group(A11yGuidanceLayer.GROUP_HAZARD)
	var telegraph: Node2D = _add(Node2D.new()) as Node2D
	telegraph.add_to_group(A11yGuidanceLayer.GROUP_HAZARD)
	var layer: A11yGuidanceLayer = _add(A11yGuidanceLayer.new()) as A11yGuidanceLayer
	_check(paint.visible, "standard: guidance visible")
	_store.set_value("guidance.level", "off")
	_check(not paint.visible, "off hides guidance")
	_check(hazard_paint.visible and telegraph.visible, "off never hides hazards")
	_store.set_value("guidance.level", "subtle")
	_check(paint.visible and is_equal_approx(paint.modulate.a, 0.4), "subtle is 40% intensity")
	_store.set_value("guidance.level", "boosted")
	_check(paint.modulate.r > 1.0 and is_equal_approx(paint.modulate.a, 1.0), "boosted is brighter")
	_store.set_value("guidance.level", "on_request")
	_check(not paint.visible, "on_request hidden until asked")
	layer.show_on_request(2.0)
	_check(paint.visible, "shown on request")
	layer.tick(2.5)
	_check(not paint.visible, "hidden again after the reveal")
	_check(hazard_paint.visible and is_equal_approx(hazard_paint.modulate.a, 1.0), "hazard never dimmed")


func test_guidance_channels_follow_rule() -> void:
	var paint: Node2D = _add(Node2D.new()) as Node2D
	paint.add_to_group(A11yGuidanceLayer.GROUP_PAINT)
	var trail: Node2D = _add(Node2D.new()) as Node2D
	trail.add_to_group(A11yGuidanceLayer.GROUP_TRAIL)
	var highlight: Node2D = _add(Node2D.new()) as Node2D
	highlight.add_to_group(A11yGuidanceLayer.GROUP_HIGHLIGHTS)
	var hazard: Node2D = _add(Node2D.new()) as Node2D
	hazard.add_to_group(A11yGuidanceLayer.GROUP_PAINT)
	hazard.add_to_group(A11yGuidanceLayer.GROUP_HAZARD)
	var layer: A11yGuidanceLayer = _add(A11yGuidanceLayer.new()) as A11yGuidanceLayer
	_eq(layer.effective_level("guidance.paint"), "standard", "follow uses guidance.level")
	_store.set_value("guidance.level", "subtle")
	_check(is_equal_approx(trail.modulate.a, 0.4), "following channel takes the overall level")
	_store.set_value("guidance.paint", "off")
	_check(not paint.visible, "paint off")
	_check(trail.visible and highlight.visible, "trail and highlights keep following")
	_check(hazard.visible, "hazard in the paint channel never hidden")
	_store.set_value("guidance.level", "off")
	_store.set_value("guidance.trail", "boosted")
	_check(trail.visible and trail.modulate.r > 1.0, "own key overrides overall off")
	_check(not highlight.visible, "following channel hidden by overall off")
	_store.set_value("guidance.trail", "follow")
	_check(not trail.visible, "back to follow uses the overall level again")
	_check(hazard.visible, "hazard still visible")


# --- Camera, colours ------------------------------------------------------------------------------

func test_camera_comfort() -> void:
	var camera: Camera3D = _add(Camera3D.new()) as Camera3D
	var comfort: A11yCameraComfort = A11yCameraComfort.new()
	comfort.camera = camera
	_add(comfort)
	_eq(camera.fov, 75.0, "fov applied on ready")
	_eq(comfort.scale_shake(10.0), 5.0, "shake reduced by default")
	_store.set_value("motion.camera_shake", 0.0)
	_eq(comfort.scale_shake(10.0), 0.0, "shake off is zero")
	_store.set_value("motion.fov", 90.0)
	_eq(camera.fov, 90.0, "fov follows the setting live")
	_check(comfort.allow_auto_camera(), "auto camera allowed by default")
	_store.set_value("motion.reduce_motion", true)
	_check(not comfort.allow_auto_camera(), "reduce motion stops automatic camera swings")
	_eq(comfort.scale_zoom_pulse(0.3), 0.0, "reduce motion removes zoom pulses")
	var attributes: CameraAttributesPractical = CameraAttributesPractical.new()
	attributes.dof_blur_far_enabled = true
	camera.attributes = attributes
	_check(comfort.chromatic_aberration_enabled() and comfort.depth_of_field_enabled(), "effects on by default")
	_store.set_value("motion.depth_of_field", false)
	comfort.apply_depth_of_field(attributes)
	_check(not attributes.dof_blur_far_enabled, "depth of field off")
	_store.set_value("motion.depth_of_field", true)
	_check(attributes.dof_blur_far_enabled and not attributes.dof_blur_near_enabled, "authored depth of field restored live")
	_store.set_value("motion.chromatic_aberration", false)
	_check(not comfort.chromatic_aberration_enabled(), "chromatic aberration switch")


func test_color_tokens() -> void:
	var tokens: A11yColorTokens = A11yColorTokens.new()
	_eq(tokens.get_color("team.enemy"), Color("d55e00"), "none profile colour")
	_store.set_value("visual.color_profile", "deutan")
	_check(tokens.get_color("team.enemy") != Color("d55e00"), "profile swaps the palette")
	_eq(tokens.get_symbol("team.enemy"), "X", "symbol paired with token")
	_eq(tokens.get_color("unknown.token"), tokens.fallback_color, "unknown token falls back")
	for profile: String in ["none", "protan", "deutan", "tritan"]:
		for token: String in tokens.get_tokens():
			_check((tokens.palettes[profile] as Dictionary).has(token), "%s defines %s" % [profile, token])
			_check(not tokens.get_symbol(token).is_empty(), "%s has a symbol" % token)
	_store.set_value("visual.color_profile", "custom")
	_store.set_value("visual.custom_colors", "team.enemy=#ff00ff; hazard=#ffff00;broken;objective=#zzzzzz")
	_eq(tokens.get_color("team.enemy"), Color("ff00ff"), "custom colour read from visual.custom_colors")
	_eq(tokens.get_color("hazard"), Color("ffff00"), "second custom colour")
	_eq(tokens.get_color("objective"), Color("f0e442"), "malformed pair skipped, falls back per token")
	_eq(tokens.get_color("team.ally"), Color("0072b2"), "custom falls back per token")
	_check(tokens.set_custom_color("team.ally", Color(0, 1, 0)), "set_custom_color writes the store")
	_eq(_store.get_string("visual.custom_colors"), "hazard=#ffff00;team.ally=#00ff00;team.enemy=#ff00ff", "custom colours stored as role=#RRGGBB pairs")


# --- Menu and narration ---------------------------------------------------------------------------

func test_settings_menu_covers_every_key() -> void:
	var menu: A11ySettingsMenu = _add(A11ySettingsMenu.new()) as A11ySettingsMenu
	for id: String in _store.get_ids():
		var control: Control = menu.get_control_for(id)
		_check(control != null, "%s has a control" % id)
		if control != null:
			_eq(control.focus_mode, Control.FOCUS_ALL, "%s is focusable" % id)
			if "accessibility_name" in control:
				_eq(str(control.get("accessibility_name")), str(_store.get_definition(id)["label"]), "%s accessibility_name" % id)
	var chain: Array[Control] = menu.get_focus_chain()
	_check(chain.size() > _store.get_ids().size(), "chain includes presets, resets and Back")
	for control: Control in chain:
		_check(control.focus_mode == Control.FOCUS_ALL, "%s focusable" % control.name)
		_check(control.get_node_or_null(control.focus_neighbor_bottom) != null, "%s has a next focus target" % control.name)
		_check(control.get_node_or_null(control.focus_neighbor_top) != null, "%s has a previous focus target" % control.name)
	menu.open()
	_check(root.gui_get_focus_owner() == chain[0], "open() focuses the first control")
	var scale_slider: HSlider = menu.get_control_for("text.scale") as HSlider
	scale_slider.value = 1.5
	_eq(_store.get_float("text.scale"), 1.5, "slider writes the store")
	_eq(menu.theme.default_font_size, 33, "menu text scales with text.scale")
	_eq(str(scale_slider.get_meta(&"a11y_value_text")), "150%", "value text for narration")
	_store.set_value("subtitles.dialogue", false)
	_check(not (menu.get_control_for("subtitles.dialogue") as CheckButton).button_pressed, "store change updates the control")
	var qte: OptionButton = menu.get_control_for("input.qte_mode") as OptionButton
	qte.select(2)
	qte.item_selected.emit(2)
	_eq(_store.get_string("input.qte_mode"), "auto", "option button writes the store")
	var hidden: LineEdit = menu.get_control_for("hud.hidden_elements") as LineEdit
	_check(hidden != null, "string key uses a LineEdit")
	if hidden != null:
		hidden.text = "minimap"
		hidden.text_submitted.emit("minimap")
		_eq(_store.get_string("hud.hidden_elements"), "minimap", "line edit writes the store")
	_store.apply_preset("readable_text")
	_eq((menu.get_control_for("text.scale") as HSlider).value, 1.5, "preset reflected in the menu")
	var undo_enabled: bool = false
	for control: Control in chain:
		if control is Button and (control as Button).text == "Undo preset":
			undo_enabled = not (control as Button).disabled
	_check(undo_enabled, "undo button enabled after a preset")


func test_narrator_safe_without_tts() -> void:
	var narrator: A11yNarrator = _add(A11yNarrator.new()) as A11yNarrator
	_store.set_value("narration.enabled", true)
	var button: Button = _add(Button.new()) as Button
	button.text = "Play"
	if not narrator.is_tts_available():
		_check(not narrator.speak("Hello"), "speak is a no-op without TTS")
		_check(not narrator.narrate_focus(button), "focus narration is a no-op without TTS")
	narrator.stop()
	narrator.attach_to_tree(button)
	button.grab_focus()
	_eq(narrator.pending().size(), 0, "nothing queued without a backend")
	var spoken: Array = []
	narrator.backend = func(text: String, _rate: float) -> void: spoken.append(text)
	var check: CheckButton = _add(CheckButton.new()) as CheckButton
	check.text = "Subtitles"
	check.button_pressed = true
	_check(narrator.narrate_focus(check), "backend speaks")
	_eq(spoken, ["Subtitles, switch, on"], "name, role, value")
	var slider: HSlider = _add(HSlider.new()) as HSlider
	slider.set_meta(&"a11y_name", "Text size")
	slider.set_meta(&"a11y_value_text", "150%")
	narrator.narrate_focus(slider)
	_eq(spoken.back(), "Text size, slider, 150%", "slider description")
	_store.set_value("narration.enabled", false)
	_check(not narrator.speak("Hidden"), "narration off stays silent")


func test_addon_files_load() -> void:
	var plugin: Script = load(ADDON + "/plugin.gd") as Script
	_check(plugin != null and plugin.can_instantiate(), "plugin.gd compiles")
	var shader: Shader = load(ADDON + "/colorblind_sim.gdshader") as Shader
	_check(shader != null and shader.code.contains("0.152286"), "simulation shader loads with Machado matrices")
	var cfg: ConfigFile = ConfigFile.new()
	_eq(cfg.load(ADDON + "/plugin.cfg"), OK, "plugin.cfg parses")
	_eq(cfg.get_value("plugin", "script", ""), "plugin.gd", "plugin.cfg points at plugin.gd")
