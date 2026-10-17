## Accessibility settings store. Register it as the autoload `A11y` (plugin.gd does this).
##
## The single source of truth for every player-facing accessibility option (architecture.md,
## section 1). Gameplay code should not read raw settings: it asks a service such as
## [A11yAssistPolicy], [A11yInputAssist] or [A11yCameraComfort], which read this store.
##
## Contract mapping: get -> [method get_value], set -> [method set_value], changed ->
## [signal setting_changed], save/load -> [method save_settings] / [method load_settings].
## The names avoid clashing with [method Object.get], [method Object.set] and the global [code]load()[/code].
##
## File format ([member storage_path], default [code]user://accessibility.cfg[/code]): a ConfigFile with a
## [code][meta][/code] section holding [code]schema_version[/code], then one section per key prefix. Only values
## the player chose (menu, preset or hand edit) are written. A key that is present in the file counts
## as the player's explicit choice; a missing key uses the game default or the imported OS preference.
class_name A11ySettings
extends Node

## Emitted after a value really changes. Late subscribers read [method get_value] first, then connect.
signal setting_changed(id: String, value: Variant)
## Emitted after [method apply_preset] finished its batch.
signal preset_applied(preset_id: String)
## Emitted after [method undo_last_preset] finished its batch.
signal preset_undone(preset_id: String)
## Emitted once per id by [method A11yAssistPolicy.request_skip] (challenges) and
## [method A11yAssistPolicy.request_scene_skip] (sensitive scenes) through
## [method emit_skip_requested]. Connect the normal completion path of the challenge or scene.
signal skip_requested(challenge_id: String)

const SCHEMA_VERSION: int = 1
const DEFAULT_STORAGE_PATH: String = "user://accessibility.cfg"
const META_SECTION: String = "meta"

## OS preference -> initial values. Applied only to keys the player has not set explicitly.
## Keys missing from a pruned registry are skipped.
const OS_PREF_VALUES: Dictionary = {
	"reduce_motion": {
		"motion.reduce_motion": true,
		"motion.camera_shake": 0.0,
		"motion.head_bob": 0.0,
		"motion.weapon_sway": 0.0,
		"motion.motion_blur": false,
		"motion.reduce_flashing": true,
	},
	"screen_reader_active": {"narration.enabled": true},
	"increase_contrast": {"visual.high_contrast": true},
}

## OS preference -> DisplayServer probe (Godot 4.5+ / Redot 26.1+; each returns -1, 0 or 1).
const OS_PREF_PROBES: Dictionary = {
	"reduce_motion": "accessibility_should_reduce_animation",
	"screen_reader_active": "accessibility_screen_reader_active",
	"increase_contrast": "accessibility_should_increase_contrast",
}

## Volume key -> audio bus name. Buses that do not exist in the project are skipped.
const AUDIO_BUSES: Dictionary = {
	"audio.master": "Master",
	"audio.music": "Music",
	"audio.sfx": "SFX",
	"audio.voice": "Voice",
	"audio.ambience": "Ambience",
}

## Meta key marking the mono down-mix effect this store adds to the Master bus.
const MONO_EFFECT_META: StringName = &"a11y_mono"

static var _active: A11ySettings = null

## Registry JSON. Empty means [code]settings_registry.json[/code] next to this script.
@export_file("*.json") var registry_path: String = ""
## Load the config file and import OS preferences in [method Node._ready].
@export var auto_load: bool = true
## Save automatically shortly after a change (only while inside the tree).
@export var autosave: bool = true
## Seconds to wait after the last change before autosaving.
@export var autosave_delay: float = 0.5
## Apply audio.* keys to AudioServer buses when they change.
@export var apply_audio_buses: bool = true
## Config sections owned by the game (not settings) that load and save with this file, for example
## input bindings. Use [method get_extra] / [method set_extra]. Never use a setting prefix here.
@export var extra_sections: PackedStringArray = PackedStringArray(["bindings"])

## Config file path. Tests point this at a temporary file.
var storage_path: String = DEFAULT_STORAGE_PATH
## Print a warning when a value is rejected. Tests switch this off.
var warn_on_invalid: bool = true
## Optional per-key migration hook, called for every key of a file whose schema_version is older
## than [constant SCHEMA_VERSION]: [code]func(from_version: int, id: String, value: Variant) -> Dictionary[/code].
## Return [code]{new_id: new_value}[/code] (several entries, or none to drop the key). Values are then
## validated like menu input. Example: [code]v0 "motion.shake_off": true -> {"motion.camera_shake": 0.0}[/code].
var migration_hook: Callable = Callable()
## Keys repaired (reset to their default or clamped) by the last [method load_settings].
var last_repaired: PackedStringArray = PackedStringArray()
## Keys renamed or dropped by migration during the last [method load_settings].
var last_migrated: PackedStringArray = PackedStringArray()

var _defs: Dictionary = {}
var _order: PackedStringArray = PackedStringArray()
var _categories: PackedStringArray = PackedStringArray()
var _presets: Dictionary = {}
var _preset_order: PackedStringArray = PackedStringArray()
var _values: Dictionary = {}
var _explicit: Dictionary = {}
var _os_values: Dictionary = {}
var _undo: Dictionary = {}
var _save_due_msec: int = -1
var _extra: Dictionary = {}
var _skipped: Dictionary = {}


func _init() -> void:
	load_registry()


func _enter_tree() -> void:
	if _active == null or not is_instance_valid(_active):
		_active = self


func _exit_tree() -> void:
	if _active == self:
		_active = null


func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS
	if auto_load:
		load_settings()
		import_os_preferences()
	if apply_audio_buses:
		apply_audio()


func _process(_delta: float) -> void:
	if _save_due_msec >= 0 and Time.get_ticks_msec() >= _save_due_msec:
		save_settings()


func _notification(what: int) -> void:
	if what == NOTIFICATION_WM_CLOSE_REQUEST and _save_due_msec >= 0:
		save_settings()


## The store services use: the one set with [method set_active], else the first store that entered
## the tree (normally the autoload). Returns null when there is none; services then use defaults.
static func active() -> A11ySettings:
	if _active != null and is_instance_valid(_active):
		return _active
	return null


## Points every service at [param store]. Use it in tests or for per-player stores in local co-op.
static func set_active(store: A11ySettings) -> void:
	_active = store


## Emits [signal skip_requested] the first time [param id] is skipped; repeated requests for the
## same id do nothing, so the completion path runs once. Returns true when the signal was emitted.
## Called by [method A11yAssistPolicy.request_skip] and [method A11yAssistPolicy.request_scene_skip].
func emit_skip_requested(id: String) -> bool:
	if _skipped.has(id):
		return false
	_skipped[id] = true
	skip_requested.emit(id)
	return true


## True when [param id] was already skipped (and not cleared since).
func was_skipped(id: String) -> bool:
	return _skipped.has(id)


## Forgets a completed skip so the challenge or scene can be skipped again, for example when the
## player replays it. An empty [param id] forgets every skip.
func clear_skip(id: String = "") -> void:
	if id.is_empty():
		_skipped.clear()
	else:
		_skipped.erase(id)


# --- Registry -------------------------------------------------------------------------------------

## Loads (or reloads) the registry and resets every value to its default.
func load_registry(path: String = "") -> Error:
	if path.is_empty():
		path = registry_path
	if path.is_empty():
		var script: Script = get_script() as Script
		path = script.resource_path.get_base_dir().path_join("settings_registry.json")
	registry_path = path
	var text: String = FileAccess.get_file_as_string(path)
	if text.is_empty():
		push_error("A11ySettings: cannot read registry '%s'. Export *.json files too." % path)
		return ERR_FILE_NOT_FOUND
	var json: JSON = JSON.new()
	if json.parse(text) != OK or not json.data is Dictionary:
		push_error("A11ySettings: registry '%s' is not valid JSON: %s" % [path, json.get_error_message()])
		return ERR_PARSE_ERROR
	var data: Dictionary = json.data
	_defs.clear()
	_order.clear()
	_categories.clear()
	_presets.clear()
	_preset_order.clear()
	_values.clear()
	_explicit.clear()
	_os_values.clear()
	_undo.clear()
	for entry: Variant in data.get("settings", []):
		if not entry is Dictionary or not (entry as Dictionary).has("id"):
			continue
		var def: Dictionary = entry
		var id: String = str(def["id"])
		var default_value: Variant = _validate(def, def.get("default"))
		if default_value == null:
			push_error("A11ySettings: registry default for '%s' is invalid." % id)
			continue
		def["default"] = default_value
		_defs[id] = def
		_order.append(id)
		_values[id] = default_value
		var category: String = str(def.get("category", "general"))
		if not _categories.has(category):
			_categories.append(category)
	for entry: Variant in data.get("presets", []):
		if entry is Dictionary and (entry as Dictionary).has("id"):
			var preset: Dictionary = entry
			_presets[str(preset["id"])] = preset
			_preset_order.append(str(preset["id"]))
	return OK


func has_setting(id: String) -> bool:
	return _defs.has(id)


## Registry entry for [param id] (id, type, default, min/max/step or values, label, description...).
func get_definition(id: String) -> Dictionary:
	return _defs.get(id, {})


## All setting ids in registry order.
func get_ids() -> PackedStringArray:
	return _order.duplicate()


## Categories in registry order.
func get_categories() -> PackedStringArray:
	return _categories.duplicate()


func get_ids_in_category(category: String) -> PackedStringArray:
	var ids: PackedStringArray = PackedStringArray()
	for id: String in _order:
		if str(_defs[id].get("category", "general")) == category:
			ids.append(id)
	return ids


func get_default(id: String) -> Variant:
	return _defs[id]["default"] if _defs.has(id) else null


## True when the player set this key (menu, preset or config file), so OS preferences leave it alone.
func is_explicit(id: String) -> bool:
	return _explicit.has(id)


# --- Get / set ------------------------------------------------------------------------------------

## Current value, or null (with a warning) for an unknown id.
func get_value(id: String) -> Variant:
	if not _values.has(id):
		if warn_on_invalid:
			push_warning("A11ySettings: unknown setting '%s'." % id)
		return null
	return _values[id]


## Typed getters return [param fallback] silently when a pruned registry has no such key.
func get_bool(id: String, fallback: bool = false) -> bool:
	var v: Variant = _values.get(id)
	return v if v is bool else fallback


func get_float(id: String, fallback: float = 0.0) -> float:
	var v: Variant = _values.get(id)
	return float(v) if (v is float or v is int) else fallback


func get_int(id: String, fallback: int = 0) -> int:
	var v: Variant = _values.get(id)
	return int(v) if (v is float or v is int) else fallback


func get_string(id: String, fallback: String = "") -> String:
	var v: Variant = _values.get(id)
	return str(v) if v is String else fallback


## Resolves a per-class or per-effect override key with the values "follow" / "off" / "on":
## "on" -> true, "off" -> false, "follow" (or a key missing from a pruned registry) -> [param follow_value].
## Examples: [code]resolve_toggle("visual.outline_enemies", get_bool("visual.high_contrast"))[/code],
## [code]resolve_toggle("motion.parallax", not get_bool("motion.reduce_motion"))[/code].
func resolve_toggle(key: String, follow_value: bool) -> bool:
	match get_string(key, "follow"):
		"on":
			return true
		"off":
			return false
	return follow_value


## Splits a string key into trimmed, non-empty items, for example hud.hidden_elements
## ("minimap, damage_numbers") -> ["minimap", "damage_numbers"].
func get_list(id: String, separator: String = ",") -> PackedStringArray:
	var items: PackedStringArray = PackedStringArray()
	for item: String in get_string(id).split(separator, false):
		if not item.strip_edges().is_empty():
			items.append(item.strip_edges())
	return items


## Validates and clamps [param value], stores it and emits [signal setting_changed] when it changed.
## Only this key changes. Returns true when the stored value changed. Wrong types, unknown enum
## values and unknown ids are rejected (returns false, value unchanged).
## [param explicit] marks the key as the player's choice (default); pass false for system imports.
func set_value(id: String, value: Variant, explicit: bool = true) -> bool:
	if not _defs.has(id):
		if warn_on_invalid:
			push_warning("A11ySettings: unknown setting '%s'." % id)
		return false
	var valid: Variant = _validate(_defs[id], value)
	if valid == null:
		if warn_on_invalid:
			push_warning("A11ySettings: rejected value %s for '%s'." % [var_to_str(value), id])
		return false
	if explicit:
		_explicit[id] = true
	return _store(id, valid, true)


## Resets one key to its baseline (imported OS preference, else registry default).
func reset(id: String) -> bool:
	if not _defs.has(id):
		return false
	_explicit.erase(id)
	_mark_dirty()
	return _store(id, _baseline(id), true)


## Resets every key of [param category]. Returns the number of keys that changed.
func reset_category(category: String) -> int:
	var changed: int = 0
	for id: String in get_ids_in_category(category):
		if reset(id):
			changed += 1
	return changed


func reset_all() -> int:
	var changed: int = 0
	for category: String in _categories:
		changed += reset_category(category)
	return changed


# --- Presets --------------------------------------------------------------------------------------

## Presets as registry dictionaries: {id, label, changes}.
func get_presets() -> Array[Dictionary]:
	var out: Array[Dictionary] = []
	for preset_id: String in _preset_order:
		out.append(_presets[preset_id])
	return out


## Applies a preset as one batch (all values stored before any signal fires) and remembers it for
## [method undo_last_preset]. Preset values count as the player's choice. Keys missing from a pruned
## registry are skipped. Returns false for an unknown preset.
func apply_preset(preset_id: String) -> bool:
	if not _presets.has(preset_id):
		return false
	var changes: Dictionary = _presets[preset_id].get("changes", {})
	var before: Dictionary = {}
	var after: Dictionary = {}
	var changed: PackedStringArray = PackedStringArray()
	for id: String in changes:
		if not _defs.has(id):
			continue
		var valid: Variant = _validate(_defs[id], changes[id])
		if valid == null:
			continue
		before[id] = [_values[id], _explicit.has(id)]
		after[id] = valid
		_explicit[id] = true
		if _store(id, valid, false):
			changed.append(id)
	_undo = {"preset": preset_id, "before": before, "after": after}
	_mark_dirty()
	_emit_batch(changed)
	preset_applied.emit(preset_id)
	return true


func can_undo_preset() -> bool:
	return not _undo.is_empty()


## Restores the values the last preset replaced. A key the player changed by hand after the preset
## keeps the player's value. Returns false when there is nothing to undo.
func undo_last_preset() -> bool:
	if _undo.is_empty():
		return false
	var before: Dictionary = _undo["before"]
	var after: Dictionary = _undo["after"]
	var preset_id: String = _undo["preset"]
	_undo = {}
	var changed: PackedStringArray = PackedStringArray()
	for id: String in before:
		if not _same(_values[id], after[id]):
			continue
		var previous: Array = before[id]
		if previous[1]:
			_explicit[id] = true
		else:
			_explicit.erase(id)
		if _store(id, previous[0], false):
			changed.append(id)
	_mark_dirty()
	_emit_batch(changed)
	preset_undone.emit(preset_id)
	return true


# --- OS preferences -------------------------------------------------------------------------------

## Imports OS accessibility preferences as initial values for keys the player has not set.
## [param prefs] maps "reduce_motion" / "screen_reader_active" / "increase_contrast" to bool; when
## empty the DisplayServer is queried (Godot 4.5+ / Redot 26.1+, skipped on older engines).
## Returns the ids that changed.
func import_os_preferences(prefs: Dictionary = {}) -> PackedStringArray:
	if prefs.is_empty():
		prefs = read_os_preferences()
	var changed: PackedStringArray = PackedStringArray()
	for pref: String in OS_PREF_VALUES:
		if not prefs.get(pref, false):
			continue
		var mapping: Dictionary = OS_PREF_VALUES[pref]
		for id: String in mapping:
			if not _defs.has(id):
				continue
			var valid: Variant = _validate(_defs[id], mapping[id])
			_os_values[id] = valid
			if _explicit.has(id):
				continue
			if _store(id, valid, true):
				changed.append(id)
	return changed


## Reads OS accessibility preferences that this engine build exposes. Missing APIs are skipped.
static func read_os_preferences() -> Dictionary:
	var prefs: Dictionary = {}
	for pref: String in OS_PREF_PROBES:
		var method: String = OS_PREF_PROBES[pref]
		if DisplayServer.has_method(method):
			var result: Variant = DisplayServer.call(method)
			prefs[pref] = (result is bool and result) or (result is int and result == 1)
	return prefs


# --- Persistence ----------------------------------------------------------------------------------

## Value stored in a game-owned section (see [member extra_sections]), e.g. input bindings.
func get_extra(section: String, key: String, fallback: Variant = null) -> Variant:
	return (_extra.get(section, {}) as Dictionary).get(key, fallback)


## Stores a value in a game-owned section; it is written by the next save (autosave included).
func set_extra(section: String, key: String, value: Variant) -> void:
	if not extra_sections.has(section):
		push_warning("A11ySettings: '%s' is not in extra_sections; it will not be saved." % section)
		return
	if not _extra.has(section):
		_extra[section] = {}
	(_extra[section] as Dictionary)[key] = value
	_mark_dirty()


## Removes a game-owned section, or one key of it.
func clear_extra(section: String, key: String = "") -> void:
	if key.is_empty():
		_extra.erase(section)
	elif _extra.has(section):
		(_extra[section] as Dictionary).erase(key)
	_mark_dirty()


## Saves the current InputMap events of [param actions] into the "bindings" section as plain data
## (ConfigFile cannot store InputEvent objects). Call it after the player remaps, then save.
func capture_bindings(actions: PackedStringArray) -> void:
	for action: String in actions:
		if not InputMap.has_action(action):
			continue
		var data: Array = []
		for event: InputEvent in InputMap.action_get_events(action):
			var entry: Dictionary = event_to_data(event)
			if not entry.is_empty():
				data.append(entry)
		set_extra("bindings", action, data)


## Applies saved bindings to InputMap (call after [method load_settings]). Unknown actions and
## unreadable entries are skipped; an action whose saved list is empty keeps its defaults.
## Returns the actions that were restored.
func restore_bindings() -> PackedStringArray:
	var restored: PackedStringArray = PackedStringArray()
	var section: Dictionary = _extra.get("bindings", {})
	for action: String in section:
		var data: Variant = section[action]
		if not InputMap.has_action(action) or not data is Array:
			continue
		var events: Array[InputEvent] = []
		for entry: Variant in data:
			if entry is Dictionary:
				var event: InputEvent = event_from_data(entry)
				if event != null:
					events.append(event)
		if events.is_empty():
			continue
		InputMap.action_erase_events(action)
		for event: InputEvent in events:
			InputMap.action_add_event(action, event)
		restored.append(action)
	return restored


## Plain-data form of a key, mouse button, joypad button or joypad axis event ({} for others).
static func event_to_data(event: InputEvent) -> Dictionary:
	if event is InputEventKey:
		var key: InputEventKey = event
		return {"type": "key", "physical_keycode": int(key.physical_keycode), "keycode": int(key.keycode),
			"shift": key.shift_pressed, "ctrl": key.ctrl_pressed, "alt": key.alt_pressed}
	if event is InputEventMouseButton:
		return {"type": "mouse_button", "button_index": int((event as InputEventMouseButton).button_index)}
	if event is InputEventJoypadButton:
		return {"type": "joy_button", "button_index": int((event as InputEventJoypadButton).button_index)}
	if event is InputEventJoypadMotion:
		var motion: InputEventJoypadMotion = event
		return {"type": "joy_motion", "axis": int(motion.axis), "axis_value": signf(motion.axis_value)}
	return {}


## Rebuilds an event from [method event_to_data] output, or null when the data is not valid.
static func event_from_data(data: Dictionary) -> InputEvent:
	match str(data.get("type", "")):
		"key":
			var key: InputEventKey = InputEventKey.new()
			key.physical_keycode = int(data.get("physical_keycode", 0)) as Key
			key.keycode = int(data.get("keycode", 0)) as Key
			key.shift_pressed = bool(data.get("shift", false))
			key.ctrl_pressed = bool(data.get("ctrl", false))
			key.alt_pressed = bool(data.get("alt", false))
			return key if (key.physical_keycode != KEY_NONE or key.keycode != KEY_NONE) else null
		"mouse_button":
			var mouse: InputEventMouseButton = InputEventMouseButton.new()
			mouse.button_index = int(data.get("button_index", 0)) as MouseButton
			return mouse if mouse.button_index != MOUSE_BUTTON_NONE else null
		"joy_button":
			var button: InputEventJoypadButton = InputEventJoypadButton.new()
			button.button_index = int(data.get("button_index", -1)) as JoyButton
			return button if int(button.button_index) >= 0 else null
		"joy_motion":
			var motion: InputEventJoypadMotion = InputEventJoypadMotion.new()
			motion.axis = int(data.get("axis", -1)) as JoyAxis
			motion.axis_value = float(data.get("axis_value", 1.0))
			return motion if int(motion.axis) >= 0 else null
	return null


## Writes explicit values (and [member extra_sections]) to [member storage_path] through a
## temporary file and a rename.
func save_settings() -> Error:
	_save_due_msec = -1
	var cfg: ConfigFile = ConfigFile.new()
	cfg.set_value(META_SECTION, "schema_version", SCHEMA_VERSION)
	for section: String in _extra:
		var entries: Dictionary = _extra[section]
		for key: String in entries:
			cfg.set_value(section, key, entries[key])
	for id: String in _order:
		if _explicit.has(id):
			cfg.set_value(_section_of(id), _key_of(id), _values[id])
	DirAccess.make_dir_recursive_absolute(storage_path.get_base_dir())
	var tmp_path: String = storage_path + ".tmp"
	var err: Error = cfg.save(tmp_path)
	if err != OK:
		push_error("A11ySettings: cannot write '%s' (%s)." % [tmp_path, error_string(err)])
		return err
	err = DirAccess.rename_absolute(tmp_path, storage_path)
	if err != OK:
		push_error("A11ySettings: cannot replace '%s' (%s)." % [storage_path, error_string(err)])
	return err


## Loads [member storage_path]. Migrates older schema versions key by key, repairs each invalid
## value on its own (that key returns to its baseline; out-of-range numbers are clamped) and keeps
## every other key. Keys absent from the file return to their baseline. Returns
## ERR_FILE_NOT_FOUND on first launch (defaults stay).
func load_settings() -> Error:
	last_repaired = PackedStringArray()
	last_migrated = PackedStringArray()
	if not FileAccess.file_exists(storage_path):
		return ERR_FILE_NOT_FOUND
	var cfg: ConfigFile = ConfigFile.new()
	var err: Error = cfg.load(storage_path)
	if err != OK:
		push_warning("A11ySettings: '%s' is unreadable (%s); keeping defaults and a .bad copy." % [storage_path, error_string(err)])
		DirAccess.copy_absolute(storage_path, storage_path + ".bad")
		return err
	var version: int = int(cfg.get_value(META_SECTION, "schema_version", 0))
	var raw: Dictionary = {}
	_extra.clear()
	for section: String in cfg.get_sections():
		if extra_sections.has(section):
			_extra[section] = {}
			for key: String in cfg.get_section_keys(section):
				(_extra[section] as Dictionary)[key] = cfg.get_value(section, key)
			continue
		if section == META_SECTION:
			continue
		for key: String in cfg.get_section_keys(section):
			raw[section + "." + key] = cfg.get_value(section, key)
	if version < SCHEMA_VERSION:
		raw = _migrate(version, raw)
	var changed: PackedStringArray = PackedStringArray()
	for id: String in _order:
		var value: Variant = _baseline(id)
		_explicit.erase(id)
		if raw.has(id):
			var valid: Variant = _validate(_defs[id], raw[id])
			if valid == null:
				last_repaired.append(id)
			else:
				if not _same_loose(valid, raw[id]):
					last_repaired.append(id)
				value = valid
				_explicit[id] = true
		if _store(id, value, false):
			changed.append(id)
	_save_due_msec = -1
	_emit_batch(changed)
	return OK


# --- Audio ----------------------------------------------------------------------------------------

## Applies every audio.* key to the matching AudioServer bus (see [constant AUDIO_BUSES]) and the
## mono down-mix. Volume 0 mutes the bus.
func apply_audio() -> void:
	for id: String in AUDIO_BUSES:
		if _defs.has(id):
			_apply_bus_volume(id)
	if _defs.has("audio.mono"):
		_apply_mono(get_bool("audio.mono"))


func _apply_bus_volume(id: String) -> void:
	var bus: int = AudioServer.get_bus_index(AUDIO_BUSES[id])
	if bus < 0:
		return
	var linear: float = get_float(id, 1.0)
	AudioServer.set_bus_mute(bus, linear <= 0.0)
	AudioServer.set_bus_volume_db(bus, linear_to_db(maxf(linear, 0.0001)))


## Mono uses an AudioEffectStereoEnhance with pan_pullout 0 at the end of the Master bus, which mixes
## left and right into the same signal. Stereo only: surround layouts need their own down-mix.
func _apply_mono(enabled: bool) -> void:
	var bus: int = AudioServer.get_bus_index("Master")
	if bus < 0:
		return
	var index: int = find_mono_effect(bus)
	if index < 0:
		if not enabled:
			return
		var effect: AudioEffectStereoEnhance = AudioEffectStereoEnhance.new()
		effect.pan_pullout = 0.0
		effect.set_meta(MONO_EFFECT_META, true)
		AudioServer.add_bus_effect(bus, effect)
		index = AudioServer.get_bus_effect_count(bus) - 1
	AudioServer.set_bus_effect_enabled(bus, index, enabled)


## Index of the mono effect on [param bus], or -1.
static func find_mono_effect(bus: int) -> int:
	for i: int in AudioServer.get_bus_effect_count(bus):
		if AudioServer.get_bus_effect(bus, i).has_meta(MONO_EFFECT_META):
			return i
	return -1


# --- Internals ------------------------------------------------------------------------------------

func _store(id: String, value: Variant, emit: bool) -> bool:
	if _values.has(id) and _same(_values[id], value):
		return false
	_values[id] = value
	_mark_dirty()
	if apply_audio_buses and (AUDIO_BUSES.has(id) or id == "audio.mono"):
		if id == "audio.mono":
			_apply_mono(bool(value))
		else:
			_apply_bus_volume(id)
	if emit:
		setting_changed.emit(id, value)
	return true


func _emit_batch(ids: PackedStringArray) -> void:
	for id: String in ids:
		setting_changed.emit(id, _values[id])


func _mark_dirty() -> void:
	if autosave and is_inside_tree():
		_save_due_msec = Time.get_ticks_msec() + int(autosave_delay * 1000.0)


func _baseline(id: String) -> Variant:
	return _os_values[id] if _os_values.has(id) else _defs[id]["default"]


func _migrate(from_version: int, raw: Dictionary) -> Dictionary:
	var out: Dictionary = {}
	for old_id: String in raw:
		var mapped: Dictionary = {old_id: raw[old_id]}
		if migration_hook.is_valid():
			var result: Variant = migration_hook.call(from_version, old_id, raw[old_id])
			mapped = result if result is Dictionary else {}
		if not mapped.has(old_id):
			last_migrated.append(old_id)
		for new_id: Variant in mapped:
			out[str(new_id)] = mapped[new_id]
	return out


## Returns the validated value, or null when [param value] is not acceptable for [param def].
static func _validate(def: Dictionary, value: Variant) -> Variant:
	match str(def.get("type", "")):
		"bool":
			return value if value is bool else null
		"float":
			if not (value is float or value is int):
				return null
			var f: float = float(value)
			if is_nan(f) or is_inf(f):
				return null
			return clampf(f, float(def.get("min", f)), float(def.get("max", f)))
		"int":
			if not (value is float or value is int):
				return null
			var g: float = float(value)
			if is_nan(g) or is_inf(g):
				return null
			return clampi(roundi(g), int(def.get("min", roundi(g))), int(def.get("max", roundi(g))))
		"enum":
			if not (value is String or value is StringName):
				return null
			var s: String = String(value)
			var allowed: Array = def.get("values", [])
			if allowed.has(s):
				return s
			return null
		"string":
			if value is String or value is StringName:
				return String(value)
			return null
	return null


static func _same(a: Variant, b: Variant) -> bool:
	if a is float and b is float:
		return is_equal_approx(a, b)
	return typeof(a) == typeof(b) and a == b


static func _same_loose(a: Variant, b: Variant) -> bool:
	if (a is float or a is int) and (b is float or b is int):
		return is_equal_approx(float(a), float(b))
	return _same(a, b)


static func _section_of(id: String) -> String:
	return id.get_slice(".", 0) if id.contains(".") else "settings"


static func _key_of(id: String) -> String:
	return id.substr(id.find(".") + 1) if id.contains(".") else id
