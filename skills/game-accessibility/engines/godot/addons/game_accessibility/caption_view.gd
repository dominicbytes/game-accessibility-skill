## Subtitle and sound-caption display (architecture.md, section 4).
##
## Fills its parent (add it to a HUD CanvasLayer) and draws a panel at the bottom centre. Honours
## subtitles.dialogue, subtitles.sound_captions, subtitles.speaker_names, subtitles.direction,
## subtitles.size and subtitles.background_opacity live. Shows at most [member max_entries]
## entries of at most 2 lines each. Events with the same merge key replace each other. Timing uses
## real time, so game speed and muted audio never change it.
class_name A11yCaptionView
extends Control

## Font size at subtitles.size = 1.0.
@export var base_font_size: int = 28
## Entries visible at once (newest kept). Each entry wraps to at most 2 lines.
@export var max_entries: int = 2
## Distance from the bottom edge in pixels; keep it clear of button prompts.
@export var bottom_margin: float = 48.0
## Bus to listen to. Empty: the first [A11yCaptionBus] in the tree.
@export var bus: A11yCaptionBus
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null

var _panel: PanelContainer
var _box: VBoxContainer
var _style: StyleBoxFlat
var _entries: Array[Dictionary] = []
var _clock: float = 0.0
var _connected_store: A11ySettings = null


func _ready() -> void:
	mouse_filter = Control.MOUSE_FILTER_IGNORE
	set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	_style = StyleBoxFlat.new()
	_style.set_content_margin_all(12.0)
	_style.set_corner_radius_all(6)
	_panel = PanelContainer.new()
	_panel.name = "CaptionPanel"
	_panel.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_panel.add_theme_stylebox_override("panel", _style)
	_panel.set_anchors_preset(Control.PRESET_CENTER_BOTTOM)
	_panel.grow_horizontal = Control.GROW_DIRECTION_BOTH
	_panel.grow_vertical = Control.GROW_DIRECTION_BEGIN
	add_child(_panel)
	_box = VBoxContainer.new()
	_box.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_panel.add_child(_box)
	var store: A11ySettings = _settings()
	if store != null:
		_connected_store = store
		store.setting_changed.connect(_on_setting_changed)
	if bus == null:
		bus = A11yCaptionBus.find(self)
	if bus != null:
		bus.subtitle_requested.connect(show_event)
		bus.caption_requested.connect(show_event)
	_restyle()
	_render()


func _exit_tree() -> void:
	if _connected_store != null and is_instance_valid(_connected_store):
		_connected_store.setting_changed.disconnect(_on_setting_changed)
	_connected_store = null


func _process(delta: float) -> void:
	advance(delta / maxf(Engine.time_scale, 0.0001))


func _settings() -> A11ySettings:
	return settings if settings != null else A11ySettings.active()


## Shows one event if the settings allow it. Safe to call directly without a bus.
func show_event(event: A11yCaptionEvent) -> void:
	if not A11yCaptionBus.should_display(event, _settings()):
		return
	var expires: float = _clock + maxf(event.duration, 0.5)
	if not event.merge_key.is_empty():
		for entry: Dictionary in _entries:
			var existing: A11yCaptionEvent = entry["event"]
			if existing.merge_key == event.merge_key:
				entry["event"] = event
				entry["expires"] = expires
				_render()
				return
	_entries.append({"event": event, "expires": expires})
	while _entries.size() > maxi(1, max_entries):
		var drop: int = 0
		for i: int in _entries.size():
			var candidate: A11yCaptionEvent = _entries[i]["event"]
			if candidate.importance != A11yCaptionEvent.Importance.CRITICAL:
				drop = i
				break
		_entries.remove_at(drop)
	_render()


## Removes every entry.
func clear() -> void:
	_entries.clear()
	_render()


## Advances the caption clock by real seconds and removes expired entries.
func advance(seconds: float) -> void:
	_clock += seconds
	var before: int = _entries.size()
	_entries = _entries.filter(func(entry: Dictionary) -> bool: return float(entry["expires"]) > _clock)
	if _entries.size() != before:
		_render()


## The formatted lines currently on screen (for tests and debugging).
func get_visible_lines() -> PackedStringArray:
	var lines: PackedStringArray = PackedStringArray()
	if _box == null or not _panel.visible:
		return lines
	for child: Node in _box.get_children():
		if child is Label and not child.is_queued_for_deletion():
			lines.append((child as Label).text)
	return lines


## Text for one event: "Speaker: line", "[Sound, direction]".
func format_line(event: A11yCaptionEvent) -> String:
	var store: A11ySettings = _settings()
	var text: String = tr(event.text)
	var show_direction: bool = store != null and store.get_bool("subtitles.direction") and not event.direction.is_empty()
	if event.kind == "dialogue":
		var speaker: String = tr(event.speaker)
		if show_direction:
			speaker = ("%s (%s)" % [speaker, event.direction]) if not speaker.is_empty() else "(%s)" % event.direction
		var names: bool = store == null or store.get_bool("subtitles.speaker_names", true)
		if names and not speaker.is_empty():
			return "%s: %s" % [speaker, text]
		return text
	var caption: String = text.trim_prefix("[").trim_suffix("]")
	if show_direction:
		caption += ", " + event.direction
	return "[%s]" % caption


func _render() -> void:
	if _box == null:
		return
	for child: Node in _box.get_children():
		_box.remove_child(child)
		child.queue_free()
	var font_size: int = _font_size()
	for entry: Dictionary in _entries:
		var label: Label = Label.new()
		label.auto_translate_mode = Node.AUTO_TRANSLATE_MODE_DISABLED
		label.text = format_line(entry["event"])
		label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
		label.max_lines_visible = 2
		label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
		label.custom_minimum_size.x = _line_width(font_size)
		label.add_theme_font_size_override("font_size", font_size)
		label.mouse_filter = Control.MOUSE_FILTER_IGNORE
		_box.add_child(label)
	_panel.visible = not _entries.is_empty()
	_panel.offset_bottom = -bottom_margin
	_panel.reset_size()


func _restyle() -> void:
	var store: A11ySettings = _settings()
	var opacity: float = store.get_float("subtitles.background_opacity", 0.75) if store != null else 0.75
	_style.bg_color = Color(0.0, 0.0, 0.0, opacity)


func _font_size() -> int:
	var store: A11ySettings = _settings()
	var scale_factor: float = store.get_float("subtitles.size", 1.0) if store != null else 1.0
	return int(round(base_font_size * scale_factor))


# About 40 characters per line, never wider than 90% of the view.
func _line_width(font_size: int) -> float:
	var width: float = font_size * 22.0
	var available: float = size.x if size.x > 0.0 else get_viewport_rect().size.x
	return minf(width, available * 0.9)


func _on_setting_changed(id: String, _value: Variant) -> void:
	if id.begins_with("subtitles.") or id == "text.scale":
		_restyle()
		_render()
