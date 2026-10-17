## Text-to-speech narration with a bounded priority queue (architecture.md, section 5).
##
## Preferred route on Godot 4.5+ / Redot 26.1+: the native accessibility tree (AccessKit) through
## Control.accessibility_name / accessibility_description, so the player's screen reader speaks.
## This node is the self-voicing fallback and complement, built on DisplayServer.tts_*.
##
## Requirements: enable the project setting [code]audio/general/text_to_speech[/code] (Project Settings >
## Audio > General > Text To Speech) and have at least one OS voice installed. Without them, or on a
## display server without TTS (headless), every call is a safe no-op.
##
## Two independent channels: [constant Channel.MENU] (menus, focus, values, UI announcements) speaks
## only while narration.enabled is on; [constant Channel.DIALOGUE] (unvoiced dialogue and notes, sent
## by [A11yCaptionBus]) speaks only while narration.dialogue is on (see [member require_setting]).
## Turning a channel's setting off stops its current speech and discards its queued speech at once;
## the other channel is untouched. Focus and value-change speech belongs to the OS screen reader
## when one is active: it is skipped when submitted and checked again before each queued item is
## spoken, so nothing is heard twice.
class_name A11yNarrator
extends Node

enum Priority { LOW, NORMAL, HIGH, CRITICAL }
enum Channel { MENU, DIALOGUE }

## Emitted when an utterance is handed to the speech backend.
signal spoken(text: String)

## Queue bound. Lowest-priority items are dropped first; CRITICAL items are never dropped.
@export var max_queue: int = 8
## When an OS screen reader is active and the engine has an accessibility tree, skip self-voicing of
## focus and value changes so the player does not hear everything twice.
@export var prefer_screen_reader: bool = true
## Speak only while the channel's setting is on (narration.enabled for menus, narration.dialogue for
## dialogue). Turn off for a game-specific "always speak" narrator.
@export var require_setting: bool = true
## TTS volume 0..100. Independent from audio buses.
@export_range(0, 100) var volume: int = 100
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null
## Optional custom backend [code]func(text: String, rate: float) -> void[/code] (for example a
## screen-reader bridge or a test fake). When valid, it replaces DisplayServer TTS.
var backend: Callable = Callable()
## Optional [code]func() -> bool[/code]: true while [member backend] is still speaking, so the queue
## waits. Invalid: the backend is treated as never busy.
var backend_is_speaking: Callable = Callable()
## Optional [code]func() -> void[/code] that silences [member backend] immediately.
var backend_stop: Callable = Callable()
## Optional [code]func() -> bool[/code] replacing the OS screen-reader probe (tests, platform bridges).
var screen_reader_probe: Callable = Callable()

var _queue: Array[Dictionary] = []
var _voice: String = ""
var _voice_checked: bool = false
var _utterance_id: int = 0
var _current_priority: int = -1
var _current_channel: int = -1
var _roots: Array[Node] = []
var _connected_store: A11ySettings = null


func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS
	var store: A11ySettings = _settings()
	if store != null:
		_connected_store = store
		store.setting_changed.connect(_on_setting_changed)


func _exit_tree() -> void:
	stop()
	if _connected_store != null and is_instance_valid(_connected_store):
		_connected_store.setting_changed.disconnect(_on_setting_changed)
	_connected_store = null
	if get_tree() != null and get_tree().node_added.is_connected(_on_node_added):
		get_tree().node_added.disconnect(_on_node_added)
	_roots.clear()


func _process(_delta: float) -> void:
	if not _queue.is_empty():
		pump()


func _settings() -> A11ySettings:
	return settings if settings != null else A11ySettings.active()


## True when DisplayServer TTS can speak: project setting on, display server supports TTS, and a
## voice exists.
func is_tts_available() -> bool:
	if not bool(ProjectSettings.get_setting("audio/general/text_to_speech", false)):
		return false
	if not DisplayServer.has_feature(DisplayServer.FEATURE_TEXT_TO_SPEECH):
		return false
	return not _get_voice().is_empty()


## True when narration of [param channel] is switched on for the player (or [member require_setting]
## is off): narration.enabled for menus, narration.dialogue for dialogue.
func is_enabled(channel: Channel = Channel.MENU) -> bool:
	if not require_setting:
		return true
	var store: A11ySettings = _settings()
	var key: String = "narration.dialogue" if channel == Channel.DIALOGUE else "narration.enabled"
	return store != null and store.get_bool(key)


func can_speak(channel: Channel = Channel.MENU) -> bool:
	return is_enabled(channel) and (backend.is_valid() or is_tts_available())


## True when the OS screen reader owns focus and value speech for [param control] (null: any control
## of this engine build): [member prefer_screen_reader] is on, the engine exposes an accessibility
## tree and a screen reader is active. Self-voicing then stays silent for that speech.
func reader_owns_route(control: Control = null) -> bool:
	if not prefer_screen_reader:
		return false
	var native: bool = ("accessibility_name" in control) if control != null else ClassDB.class_has_method(&"Control", &"set_accessibility_name")
	return native and _reader_active()


## True when an OS screen reader is running (Godot 4.5+ / Redot 26.1+). False when unknown.
static func screen_reader_active() -> bool:
	if not DisplayServer.has_method("accessibility_screen_reader_active"):
		return false
	var result: Variant = DisplayServer.call("accessibility_screen_reader_active")
	return result is int and result == 1


## Queues [param text] on [param channel]. [param interrupt] stops current speech of the same channel
## (unless it is CRITICAL and this is not) and drops that channel's queued non-critical items. Items with the
## same non-empty [param tag] replace each other, so stale focus speech never plays. Returns false
## when the channel cannot speak.
func speak(text: String, priority: Priority = Priority.NORMAL, interrupt: bool = false, tag: String = "", channel: Channel = Channel.MENU) -> bool:
	return _submit(text, priority, interrupt, tag, channel, false)


## Reads an unvoiced dialogue line or note (narration.dialogue), independent of menu narration.
func speak_dialogue(text: String, priority: Priority = Priority.NORMAL) -> bool:
	return _submit(text, priority, false, "", Channel.DIALOGUE, false)


## Stops speech and clears the queue.
func stop() -> void:
	_queue.clear()
	_current_priority = -1
	_current_channel = -1
	_stop_backend()


## Stops the current utterance if it belongs to [param channel] and discards that channel's queued
## items. Other channels keep speaking.
func stop_channel(channel: Channel) -> void:
	for i: int in range(_queue.size() - 1, -1, -1):
		if int(_queue[i]["channel"]) == channel:
			_queue.remove_at(i)
	if _current_channel == channel:
		_current_channel = -1
		_current_priority = -1
		_stop_backend()


## Dispatches queued speech when the backend is free. [method Node._process] calls it.
func pump() -> void:
	if backend.is_valid():
		while not _queue.is_empty():
			if backend_is_speaking.is_valid() and bool(backend_is_speaking.call()):
				return
			var item: Dictionary = _queue.pop_front()
			if not _still_wanted(item):
				continue
			_current_priority = int(item["priority"])
			_current_channel = int(item["channel"])
			backend.call(str(item["text"]), _rate())
			spoken.emit(str(item["text"]))
		return
	if not is_tts_available():
		return
	while not _queue.is_empty():
		if DisplayServer.tts_is_speaking():
			return
		var next: Dictionary = _queue.pop_front()
		if not _still_wanted(next):
			continue
		_utterance_id += 1
		_current_priority = int(next["priority"])
		_current_channel = int(next["channel"])
		DisplayServer.tts_speak(str(next["text"]), _voice, volume, 1.0, _rate(), _utterance_id, false)
		spoken.emit(str(next["text"]))
		return


## Texts waiting in the queue (for debugging and tests).
func pending() -> PackedStringArray:
	var out: PackedStringArray = PackedStringArray()
	for item: Dictionary in _queue:
		out.append(str(item["text"]))
	return out


## Forgets the cached voice, for example after the locale changed.
func refresh_voice() -> void:
	_voice_checked = false
	_voice = ""


## Speaks "name, role, value, hint" for a focused control, interrupting stale focus speech.
## Silent when the screen reader owns the route ([method reader_owns_route]).
func narrate_focus(control: Control) -> bool:
	if control == null or reader_owns_route(control):
		return false
	return _submit(describe(control), Priority.NORMAL, true, "focus", Channel.MENU, "accessibility_name" in control)


## Connects focus and value changes of every focusable Control under [param root], now and for
## Controls added later. Also fills accessibility_name (Godot 4.5+) where it is empty and the
## control has no visible text of its own.
func attach_to_tree(root: Node) -> void:
	if root == null:
		return
	if not _roots.has(root):
		_roots.append(root)
	_attach_recursive(root)
	if is_inside_tree() and not get_tree().node_added.is_connected(_on_node_added):
		get_tree().node_added.connect(_on_node_added)


## "Name, role, value, hint" text for [param control].
static func describe(control: Control) -> String:
	var parts: PackedStringArray = PackedStringArray()
	for part: String in [control_name(control), control_role(control), control_value(control), control_hint(control)]:
		if not part.is_empty():
			parts.append(part)
	return ", ".join(parts)


static func control_name(control: Control) -> String:
	if control.has_meta(&"a11y_name"):
		return str(control.get_meta(&"a11y_name"))
	if "accessibility_name" in control and not str(control.get("accessibility_name")).is_empty():
		return str(control.get("accessibility_name"))
	if control is OptionButton:
		return control.tooltip_text if not control.tooltip_text.is_empty() else String(control.name)
	if "text" in control and not str(control.get("text")).is_empty():
		return str(control.get("text"))
	if not control.tooltip_text.is_empty():
		return control.tooltip_text
	return String(control.name)


static func control_role(control: Control) -> String:
	if control is CheckButton:
		return "switch"
	if control is CheckBox:
		return "checkbox"
	if control is OptionButton:
		return "list"
	if control is LinkButton:
		return "link"
	if control is Button:
		return "toggle button" if (control as Button).toggle_mode else "button"
	if control is Slider:
		return "slider"
	if control is SpinBox:
		return "spin box"
	if control is ProgressBar:
		return "progress bar"
	if control is LineEdit or control is TextEdit:
		return "text field"
	if control is TabBar:
		return "tab"
	if control is ItemList or control is Tree:
		return "list"
	return ""


static func control_value(control: Control) -> String:
	if control.has_meta(&"a11y_value_text"):
		return str(control.get_meta(&"a11y_value_text"))
	if control is OptionButton:
		var option: OptionButton = control
		return option.get_item_text(option.selected) if option.selected >= 0 else ""
	if control is BaseButton and (control as BaseButton).toggle_mode:
		var on: bool = (control as BaseButton).button_pressed
		if control is CheckButton or control is CheckBox:
			return "on" if on else "off"
		return "pressed" if on else "not pressed"
	if control is Range:
		return str((control as Range).value)
	if control is LineEdit:
		return (control as LineEdit).text
	return ""


static func control_hint(control: Control) -> String:
	if control.has_meta(&"a11y_hint"):
		return str(control.get_meta(&"a11y_hint"))
	if "accessibility_description" in control:
		return str(control.get("accessibility_description"))
	return ""


func _attach_recursive(node: Node) -> void:
	_attach(node)
	for child: Node in node.get_children():
		_attach_recursive(child)


func _attach(node: Node) -> void:
	if not node is Control or node.has_meta(&"_a11y_narrated"):
		return
	var control: Control = node
	if control.focus_mode == Control.FOCUS_NONE:
		return
	control.set_meta(&"_a11y_narrated", true)
	control.focus_entered.connect(narrate_focus.bind(control))
	if control is Range:
		(control as Range).value_changed.connect(_on_range_value.bind(control))
	elif control is OptionButton:
		(control as OptionButton).item_selected.connect(_on_option_value.bind(control))
	elif control is BaseButton:
		(control as BaseButton).toggled.connect(_on_toggle_value.bind(control))
	if "accessibility_name" in control and str(control.get("accessibility_name")).is_empty():
		var has_text: bool = "text" in control and not str(control.get("text")).is_empty()
		if not has_text:
			control.set("accessibility_name", control_name(control))


func _on_node_added(node: Node) -> void:
	for root: Node in _roots:
		if is_instance_valid(root) and root.is_ancestor_of(node):
			_attach(node)
			return


# Value changes use the same route-ownership rule as focus, so a screen reader is never doubled.
func _speak_value(control: Control) -> void:
	if not control.has_focus() or reader_owns_route(control):
		return
	_submit(control_value(control), Priority.NORMAL, true, "focus", Channel.MENU, "accessibility_name" in control)


func _on_range_value(_value: float, control: Control) -> void:
	_speak_value(control)


func _on_option_value(_index: int, control: Control) -> void:
	_speak_value(control)


func _on_toggle_value(_pressed: bool, control: Control) -> void:
	_speak_value(control)


# [param reader_route]: the speech is also available to the OS screen reader through the
# accessibility tree, so it is dropped at dispatch if a reader became active meanwhile.
func _submit(text: String, priority: Priority, interrupt: bool, tag: String, channel: Channel, reader_route: bool) -> bool:
	var clean: String = text.strip_edges()
	if clean.is_empty() or not can_speak(channel):
		return false
	if interrupt:
		_interrupt(priority, channel)
	if not tag.is_empty():
		for i: int in range(_queue.size() - 1, -1, -1):
			if _queue[i]["tag"] == tag and int(_queue[i]["priority"]) < Priority.CRITICAL:
				_queue.remove_at(i)
	_enqueue({"text": clean, "priority": int(priority), "tag": tag, "channel": int(channel), "reader_route": reader_route})
	pump()
	return true


# Within [param channel] only: stops the current utterance (unless it is CRITICAL and the new item
# is not) and drops queued non-critical items. Speech of the other channel is left alone.
func _interrupt(priority: int, channel: int) -> void:
	if _current_channel == channel and _current_priority == Priority.CRITICAL and priority < Priority.CRITICAL:
		return
	for i: int in range(_queue.size() - 1, -1, -1):
		if int(_queue[i]["priority"]) < Priority.CRITICAL and int(_queue[i]["channel"]) == channel:
			_queue.remove_at(i)
	if _current_channel == channel:
		_current_priority = -1
		_current_channel = -1
		_stop_backend()


func _stop_backend() -> void:
	if backend.is_valid():
		if backend_stop.is_valid():
			backend_stop.call()
	elif is_tts_available():
		DisplayServer.tts_stop()


func _reader_active() -> bool:
	if screen_reader_probe.is_valid():
		return bool(screen_reader_probe.call())
	return screen_reader_active()


# Re-checks the channel setting and route ownership just before speaking, so speech queued before
# a setting or screen-reader change never plays.
func _still_wanted(item: Dictionary) -> bool:
	if not is_enabled(int(item["channel"]) as Channel):
		return false
	return not (bool(item["reader_route"]) and prefer_screen_reader and _reader_active())


func _on_setting_changed(id: String, _value: Variant) -> void:
	if id == "narration.enabled" and not is_enabled(Channel.MENU):
		stop_channel(Channel.MENU)
	elif id == "narration.dialogue" and not is_enabled(Channel.DIALOGUE):
		stop_channel(Channel.DIALOGUE)


func _enqueue(item: Dictionary) -> void:
	if _queue.size() >= max_queue:
		var lowest: int = -1
		for i: int in _queue.size():
			var p: int = int(_queue[i]["priority"])
			if p < Priority.CRITICAL and (lowest < 0 or p < int(_queue[lowest]["priority"])):
				lowest = i
		if lowest >= 0 and int(_queue[lowest]["priority"]) <= int(item["priority"]):
			_queue.remove_at(lowest)
		elif int(item["priority"]) < Priority.CRITICAL:
			return
	var index: int = _queue.size()
	for i: int in _queue.size():
		if int(_queue[i]["priority"]) < int(item["priority"]):
			index = i
			break
	_queue.insert(index, item)


func _rate() -> float:
	var store: A11ySettings = _settings()
	return clampf(store.get_float("narration.rate", 1.0) if store != null else 1.0, 0.1, 10.0)


func _get_voice() -> String:
	if _voice_checked:
		return _voice
	_voice_checked = true
	var language: String = TranslationServer.get_locale().get_slice("_", 0)
	var voices: PackedStringArray = DisplayServer.tts_get_voices_for_language(language)
	if not voices.is_empty():
		_voice = voices[0]
	else:
		var all_voices: Array[Dictionary] = DisplayServer.tts_get_voices()
		if not all_voices.is_empty():
			_voice = str(all_voices[0].get("id", ""))
	return _voice
