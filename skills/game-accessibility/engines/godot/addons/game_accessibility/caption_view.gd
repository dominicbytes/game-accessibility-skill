## Subtitle and sound-caption display (architecture.md, section 4).
##
## Fills its parent (add it to a HUD CanvasLayer) and draws a panel at the bottom centre. Honours
## subtitles.dialogue, subtitles.sound_captions, subtitles.speaker_names, subtitles.direction,
## subtitles.size, subtitles.background_opacity and text.auto_advance live. Turning a caption kind
## off hides its visible entries at once.
##
## Text is never clipped. Each entry is split into pages of at most [constant LINES_PER_PAGE] lines
## that fit the current width and size, and pages are re-split (keeping the reading position) when
## the view, viewport or subtitle size changes. Pages turn by time: each page stays for its share of
## the event duration, never less than its reading time ([member reading_chars_per_second]) or
## [member min_page_seconds]. An event with [member A11yCaptionEvent.can_wait] set while
## text.auto_advance is off waits for [method advance_page] on every page instead (bind it to the
## game's continue action). Shows at most [member max_entries] entries; events with the same merge
## key replace each other. Timing uses real time, so game speed and muted audio never change it.
class_name A11yCaptionView
extends Control

## Emitted when an entry leaves the view. [param fully_shown] is false when it was hidden early
## (caption kind turned off, [method clear], or pushed out by newer entries).
signal entry_finished(event: A11yCaptionEvent, fully_shown: bool)

## Lines per page (XAG / architecture.md: at most 2 lines).
const LINES_PER_PAGE: int = 2

## Font size at subtitles.size = 1.0.
@export var base_font_size: int = 28
## Entries visible at once (newest kept).
@export var max_entries: int = 2
## Distance from the bottom edge in pixels; keep it clear of button prompts.
@export var bottom_margin: float = 48.0
## Reading speed used for the minimum time of each page, in characters per second.
@export var reading_chars_per_second: float = 15.0
## Minimum real seconds each page stays on screen.
@export var min_page_seconds: float = 1.0
## Bus to listen to. Empty: the first [A11yCaptionBus] in the tree.
@export var bus: A11yCaptionBus
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null

var _panel: PanelContainer
var _box: VBoxContainer
var _style: StyleBoxFlat
## Each entry: {event, pages: PackedStringArray, starts: PackedInt32Array, page: int, left: float}.
## starts[i] is the number of non-space characters before page i (the reading position).
var _entries: Array[Dictionary] = []
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
	get_viewport().size_changed.connect(_on_viewport_resized)
	_restyle()
	_render()


func _exit_tree() -> void:
	if _connected_store != null and is_instance_valid(_connected_store):
		_connected_store.setting_changed.disconnect(_on_setting_changed)
	_connected_store = null
	if get_viewport() != null and get_viewport().size_changed.is_connected(_on_viewport_resized):
		get_viewport().size_changed.disconnect(_on_viewport_resized)


func _notification(what: int) -> void:
	if what == NOTIFICATION_RESIZED:
		reflow()


func _process(delta: float) -> void:
	advance(delta / maxf(Engine.time_scale, 0.0001))


func _settings() -> A11ySettings:
	return settings if settings != null else A11ySettings.active()


## Shows one event if the settings allow it. Safe to call directly without a bus.
func show_event(event: A11yCaptionEvent) -> void:
	if not A11yCaptionBus.should_display(event, _settings()):
		return
	if not event.merge_key.is_empty():
		for entry: Dictionary in _entries:
			var existing: A11yCaptionEvent = entry["event"]
			if existing.merge_key == event.merge_key:
				entry["event"] = event
				_paginate_entry(entry, 0, true)
				_render()
				return
	var added: Dictionary = {"event": event}
	_paginate_entry(added, 0, true)
	_entries.append(added)
	while _entries.size() > maxi(1, max_entries):
		var drop: int = 0
		for i: int in _entries.size():
			var candidate: A11yCaptionEvent = _entries[i]["event"]
			if candidate.importance != A11yCaptionEvent.Importance.CRITICAL:
				drop = i
				break
		var dropped: A11yCaptionEvent = _entries[drop]["event"]
		_entries.remove_at(drop)
		entry_finished.emit(dropped, false)
	_render()


## Removes every entry.
func clear() -> void:
	var removed: Array[Dictionary] = _entries
	_entries = []
	for entry: Dictionary in removed:
		entry_finished.emit(entry["event"], false)
	_render()


## Advances timed pages by real seconds; entries leave after their last page. Pages waiting for the
## player (see [method is_waiting]) do not move.
func advance(seconds: float) -> void:
	var changed: bool = false
	var finished: Array[A11yCaptionEvent] = []
	for entry: Dictionary in _entries:
		if _is_manual(entry):
			continue
		entry["left"] = float(entry["left"]) - seconds
		while float(entry["left"]) <= 0.0 and not entry.has("done"):
			changed = true
			if not _next_page(entry, float(entry["left"])):
				entry["done"] = true
				finished.append(entry["event"])
	if not finished.is_empty():
		_entries = _entries.filter(func(entry: Dictionary) -> bool: return not entry.has("done"))
		for event: A11yCaptionEvent in finished:
			entry_finished.emit(event, true)
	if changed:
		_render()


## Turns the page of [param event] (null: the oldest entry), or removes it after its last page.
## Bind it to the game's continue action. Returns false when there is nothing to advance.
func advance_page(event: A11yCaptionEvent = null) -> bool:
	for i: int in _entries.size():
		var entry: Dictionary = _entries[i]
		if event != null and entry["event"] != event:
			continue
		if not _next_page(entry, 0.0):
			_entries.remove_at(i)
			entry_finished.emit(entry["event"], true)
		_render()
		return true
	return false


## True when an entry is held until the player continues ([method advance_page]).
func is_waiting() -> bool:
	for entry: Dictionary in _entries:
		if _is_manual(entry):
			return true
	return false


## Page counts of the current entries, oldest first (for tests and debugging).
func get_page_counts() -> PackedInt32Array:
	var counts: PackedInt32Array = PackedInt32Array()
	for entry: Dictionary in _entries:
		counts.append((entry["pages"] as PackedStringArray).size())
	return counts


## The page texts currently on screen, one per entry; lines inside a page are separated by "\n".
func get_visible_lines() -> PackedStringArray:
	var lines: PackedStringArray = PackedStringArray()
	if _box == null or not _panel.visible:
		return lines
	for child: Node in _box.get_children():
		if child is Label and not child.is_queued_for_deletion():
			lines.append((child as Label).text)
	return lines


## Text for one event: "Speaker (direction): line", "(direction) line", "[Sound, direction]".
## Speaker names and direction are independent settings.
func format_line(event: A11yCaptionEvent) -> String:
	var store: A11ySettings = _settings()
	var text: String = tr(event.text)
	var show_direction: bool = store != null and store.get_bool("subtitles.direction") and not event.direction.is_empty()
	if event.kind == "dialogue":
		var names: bool = store == null or store.get_bool("subtitles.speaker_names", true)
		var speaker: String = tr(event.speaker) if names else ""
		if not speaker.is_empty():
			if show_direction:
				speaker = "%s (%s)" % [speaker, event.direction]
			return "%s: %s" % [speaker, text]
		if show_direction:
			return "(%s) %s" % [event.direction, text]
		return text
	var caption: String = text.trim_prefix("[").trim_suffix("]")
	if show_direction:
		caption += ", " + event.direction
	return "[%s]" % caption


## Splits [param text] into pages of at most [constant LINES_PER_PAGE] lines that fit
## [param width] pixels at [param font_size]. No word is dropped; a word wider than a line is
## split across lines.
func paginate(text: String, font_size: int, width: float) -> PackedStringArray:
	var pages: PackedStringArray = PackedStringArray()
	var lines: PackedStringArray = wrap_lines(text, font_size, width)
	for i: int in range(0, lines.size(), LINES_PER_PAGE):
		pages.append("\n".join(lines.slice(i, i + LINES_PER_PAGE)))
	return pages


## Greedy word wrap of [param text] to [param width] pixels at [param font_size].
func wrap_lines(text: String, font_size: int, width: float) -> PackedStringArray:
	var font: Font = get_theme_font(&"font", &"Label")
	var lines: PackedStringArray = PackedStringArray()
	var current: String = ""
	for raw_word: String in text.replace("\n", " ").split(" ", false):
		var word: String = raw_word
		var candidate: String = word if current.is_empty() else current + " " + word
		if _width(font, candidate, font_size) <= width:
			current = candidate
			continue
		if not current.is_empty():
			lines.append(current)
		while word.length() > 1 and _width(font, word, font_size) > width:
			var cut: int = word.length() - 1
			while cut > 1 and _width(font, word.substr(0, cut), font_size) > width:
				cut -= 1
			lines.append(word.substr(0, cut))
			word = word.substr(cut)
		current = word
	if not current.is_empty():
		lines.append(current)
	return lines


## Re-splits every entry for the current width and size, keeping each entry's reading position.
## Called on resize, viewport changes and subtitle setting changes.
func reflow() -> void:
	if _box == null:
		return
	for entry: Dictionary in _entries:
		var starts: PackedInt32Array = entry["starts"]
		var reading_pos: int = starts[int(entry["page"])] if not starts.is_empty() else 0
		_paginate_entry(entry, reading_pos, false)
	_render()


# [param reading_pos]: non-space characters already read; [param restart]: start the page timer anew.
func _paginate_entry(entry: Dictionary, reading_pos: int, restart: bool) -> void:
	var font_size: int = _font_size()
	var event: A11yCaptionEvent = entry["event"]
	var pages: PackedStringArray = paginate(format_line(event), font_size, _line_width(font_size) * 0.98)
	if pages.is_empty():
		pages.append("")
	var starts: PackedInt32Array = PackedInt32Array()
	var count: int = 0
	for page: String in pages:
		starts.append(count)
		count += page.replace(" ", "").replace("\n", "").length()
	var index: int = 0
	for i: int in starts.size():
		if starts[i] <= reading_pos:
			index = i
	entry["pages"] = pages
	entry["starts"] = starts
	entry["page"] = index
	if restart or not entry.has("left"):
		entry["left"] = _page_seconds(entry, index)
	else:
		entry["left"] = maxf(float(entry["left"]), min_page_seconds)


func _next_page(entry: Dictionary, carry: float) -> bool:
	var next: int = int(entry["page"]) + 1
	if next >= (entry["pages"] as PackedStringArray).size():
		return false
	entry["page"] = next
	entry["left"] = carry + _page_seconds(entry, next)
	return true


func _page_seconds(entry: Dictionary, index: int) -> float:
	var pages: PackedStringArray = entry["pages"]
	var event: A11yCaptionEvent = entry["event"]
	var total: int = 0
	for page: String in pages:
		total += page.length()
	var chars: int = pages[index].length()
	var share: float = event.duration * float(chars) / float(maxi(total, 1))
	var reading: float = float(chars) / maxf(reading_chars_per_second, 0.1)
	return maxf(maxf(share, reading), min_page_seconds)


func _is_manual(entry: Dictionary) -> bool:
	var event: A11yCaptionEvent = entry["event"]
	if not event.can_wait:
		return false
	var store: A11ySettings = _settings()
	return store != null and not store.get_bool("text.auto_advance", false)


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
		label.text = (entry["pages"] as PackedStringArray)[int(entry["page"])]
		# Pages already fit; wrapping stays on as a safety net and no line limit is set, so nothing
		# is ever clipped.
		label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
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


# About 40 characters per line, never wider than 90% of the view (minus the panel margins).
func _line_width(font_size: int) -> float:
	var width: float = font_size * 22.0
	var available: float = size.x if size.x > 0.0 else get_viewport_rect().size.x
	return maxf(minf(width, available * 0.9 - 24.0), float(font_size))


static func _width(font: Font, text: String, font_size: int) -> float:
	return font.get_string_size(text, HORIZONTAL_ALIGNMENT_LEFT, -1, font_size).x


func _hide_disabled_kinds() -> void:
	var store: A11ySettings = _settings()
	var hidden: Array[A11yCaptionEvent] = []
	var kept: Array[Dictionary] = []
	for entry: Dictionary in _entries:
		if A11yCaptionBus.should_display(entry["event"], store):
			kept.append(entry)
		else:
			hidden.append(entry["event"])
	_entries = kept
	for event: A11yCaptionEvent in hidden:
		entry_finished.emit(event, false)


func _on_viewport_resized() -> void:
	reflow()


func _on_setting_changed(id: String, _value: Variant) -> void:
	if id == "subtitles.dialogue" or id == "subtitles.sound_captions":
		_hide_disabled_kinds()
	if id.begins_with("subtitles.") or id == "text.scale":
		_restyle()
		reflow()
