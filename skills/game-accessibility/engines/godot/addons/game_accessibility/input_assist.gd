## Input assists on top of InputMap actions (architecture.md, section 3).
##
## Add one instance to the player or gameplay scene. Gameplay asks it about actions, never keys:
## [codeblock]
## if input_assist.is_held(&"aim"): ...
## if input_assist.mash_progress(&"struggle", 12) >= 1.0: break_free()
## var window := input_assist.qte("door_kick", 1.2)   # 0.0 = auto success follows
## var parry := input_assist.timing_window(0.15)
## [/codeblock]
class_name A11yInputAssist
extends Node

## Emitted once when a mash/hold/single sequence reaches full progress.
signal mash_completed(action: StringName)
## Emitted after the short readable beat when input.qte_mode is "auto". Run the QTE success branch.
signal qte_auto_succeeded(prompt_id: String)

## Returned by [method qte] in auto mode.
const QTE_AUTO: float = 0.0
## Default presses per second used to derive a hold duration from a press count.
const AUTHORED_PRESS_RATE: float = 5.0

## Actions that follow input.hold_mode. Actions queried through [method is_held] are added too.
@export var hold_actions: PackedStringArray = PackedStringArray()
## Pause before an auto QTE succeeds, so the player can read what happened.
@export var auto_beat_seconds: float = 0.6
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null

var _tracked: Dictionary = {}
var _latched: Dictionary = {}
var _mash: Dictionary = {}
var _connected_store: A11ySettings = null


func _ready() -> void:
	for action: String in hold_actions:
		_track(StringName(action))
	var store: A11ySettings = _settings()
	if store != null:
		_connected_store = store
		store.setting_changed.connect(_on_setting_changed)
	Input.joy_connection_changed.connect(_on_joy_connection_changed)


func _exit_tree() -> void:
	clear_latches()
	if _connected_store != null and is_instance_valid(_connected_store):
		_connected_store.setting_changed.disconnect(_on_setting_changed)
	_connected_store = null
	if Input.joy_connection_changed.is_connected(_on_joy_connection_changed):
		Input.joy_connection_changed.disconnect(_on_joy_connection_changed)


func _notification(what: int) -> void:
	if what == NOTIFICATION_PAUSED or what == NOTIFICATION_APPLICATION_FOCUS_OUT:
		clear_latches()


func _input(event: InputEvent) -> void:
	handle_event(event)


func _process(delta: float) -> void:
	tick(delta)


func _settings() -> A11ySettings:
	return settings if settings != null else A11ySettings.active()


func hold_mode() -> String:
	var store: A11ySettings = _settings()
	return store.get_string("input.hold_mode", "hold") if store != null else "hold"


func mash_mode() -> String:
	var store: A11ySettings = _settings()
	return store.get_string("input.mash_mode", "mash") if store != null else "mash"


func qte_mode() -> String:
	var store: A11ySettings = _settings()
	return store.get_string("input.qte_mode", "normal") if store != null else "normal"


## Feeds one input event. [method Node._input] calls this; tests and replay tools can call it directly.
func handle_event(event: InputEvent) -> void:
	if event.is_echo():
		return
	for action: StringName in _tracked:
		if event.is_action_pressed(action) and hold_mode() == "toggle":
			_latched[action] = not bool(_latched.get(action, false))
	for action: StringName in _mash:
		var state: Dictionary = _mash[action]
		if event.is_action_pressed(action):
			_on_mash_press(action, state)
		elif event.is_action_released(action):
			state["down"] = false


## Whether a hold action is active. In "hold" mode this is the physical hold. In "toggle" mode the
## first press latches and the second releases; the latch clears on pause, focus loss, device
## disconnect, leaving the tree, a hold-mode change and [method clear_latches].
func is_held(action: StringName) -> bool:
	_track(action)
	if hold_mode() == "toggle":
		return bool(_latched.get(action, false))
	return InputMap.has_action(action) and Input.is_action_pressed(action)


## Clears one latch, or all latches when [param action] is empty. Call it on death and scene change.
func clear_latches(action: StringName = &"") -> void:
	if action == &"":
		_latched.clear()
	else:
		_latched.erase(action)


## Starts (or restarts) a repeated-press sequence. [param hold_seconds] is the authored hold time for
## "hold" mode; 0 derives it from the press count at [constant AUTHORED_PRESS_RATE].
func start_mash(action: StringName, required_presses: int, hold_seconds: float = 0.0) -> void:
	var seconds: float = hold_seconds if hold_seconds > 0.0 else maxf(0.5, required_presses / AUTHORED_PRESS_RATE)
	_mash[action] = {
		"required": maxi(1, required_presses),
		"hold_seconds": seconds,
		"presses": 0,
		"progress": 0.0,
		"down": InputMap.has_action(action) and Input.is_action_pressed(action),
		"done": false,
	}


## Progress 0..1 of a repeated-press sequence, starting it on first call when
## [param required_presses] > 0. "mash": counts presses. "hold": holding fills at the authored rate.
## "single": one press completes it.
func mash_progress(action: StringName, required_presses: int = 0) -> float:
	if not _mash.has(action):
		if required_presses <= 0:
			return 0.0
		start_mash(action, required_presses)
	return float(_mash[action]["progress"])


func is_mash_complete(action: StringName) -> bool:
	return _mash.has(action) and bool(_mash[action]["done"])


func stop_mash(action: StringName) -> void:
	_mash.erase(action)


## Advances hold-mode progress. [method Node._process] calls this with the frame delta.
func tick(delta: float) -> void:
	for action: StringName in _mash:
		var state: Dictionary = _mash[action]
		if state["done"] or not state["down"] or mash_mode() != "hold":
			continue
		_set_progress(action, state, float(state["progress"]) + delta / float(state["hold_seconds"]))


## Response window for a quick-time event. "normal": [param base_window]. "slow": at least 3x the
## window, or INF when [param wait_if_static] (nothing else moves while the prompt waits). "auto":
## returns [constant QTE_AUTO] and emits [signal qte_auto_succeeded] after [member auto_beat_seconds].
func qte(prompt_id: String, base_window: float, wait_if_static: bool = false) -> float:
	match qte_mode():
		"auto":
			_schedule_auto_success(prompt_id)
			return QTE_AUTO
		"slow":
			if wait_if_static:
				return INF
			var store: A11ySettings = _settings()
			var scale: float = store.get_float("input.timing_window_scale", 1.0) if store != null else 1.0
			return base_window * maxf(3.0, scale)
	return base_window


## Parry, dodge, rhythm and perfect-hit windows in SIMULATION seconds:
## [param base_seconds] x input.timing_window_scale. Compare it with simulation time (the scaled
## delta your gameplay already uses). Never multiply it by the game speed as well: a slowed game
## already stretches it to base x window_scale / time_scale real seconds.
func timing_window(base_seconds: float) -> float:
	var store: A11ySettings = _settings()
	return base_seconds * (store.get_float("input.timing_window_scale", 1.0) if store != null else 1.0)


## The same window in real seconds: base x window_scale / time_scale. For displays or tests only;
## gameplay should measure [method timing_window] in simulation time instead.
func timing_window_real(base_seconds: float) -> float:
	var store: A11ySettings = _settings()
	var speed: float = store.get_float("assist.game_speed", 1.0) if store != null else 1.0
	return timing_window(base_seconds) / maxf(speed, 0.0001)


func _track(action: StringName) -> void:
	if not _tracked.has(action) and InputMap.has_action(action):
		_tracked[action] = true


func _on_mash_press(action: StringName, state: Dictionary) -> void:
	if state["done"]:
		return
	match mash_mode():
		"single":
			_set_progress(action, state, 1.0)
		"hold":
			state["down"] = true
		_:
			state["presses"] = int(state["presses"]) + 1
			_set_progress(action, state, float(state["presses"]) / float(state["required"]))


func _set_progress(action: StringName, state: Dictionary, value: float) -> void:
	state["progress"] = clampf(value, 0.0, 1.0)
	if float(state["progress"]) >= 1.0 and not state["done"]:
		state["done"] = true
		mash_completed.emit(action)


func _schedule_auto_success(prompt_id: String) -> void:
	if auto_beat_seconds > 0.0 and is_inside_tree():
		get_tree().create_timer(auto_beat_seconds, false).timeout.connect(_emit_auto_success.bind(prompt_id))
	else:
		_emit_auto_success.call_deferred(prompt_id)


func _emit_auto_success(prompt_id: String) -> void:
	qte_auto_succeeded.emit(prompt_id)


func _on_setting_changed(id: String, _value: Variant) -> void:
	if id == "input.hold_mode":
		clear_latches()


func _on_joy_connection_changed(_device: int, connected: bool) -> void:
	if not connected:
		clear_latches()
