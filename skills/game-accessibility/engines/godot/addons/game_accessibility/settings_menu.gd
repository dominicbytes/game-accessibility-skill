## Accessibility settings screen generated from the registry (architecture.md, section 10).
##
## One section per category with a CheckButton, HSlider or OptionButton per key and a one-line
## description underneath; a presets row with Undo; a reset button per category; Back. Fully
## operable with keyboard or controller (one vertical focus chain, wraps around), narrated through
## an [A11yNarrator] when narration.enabled is on, accessibility_name/description set on Godot 4.5+,
## and scaled by text.scale. It runs while the tree is paused, so the same scene serves the title
## screen, the first-launch screen and the pause menu.
## [codeblock]
## var menu := A11ySettingsMenu.new()
## add_child(menu)            # in a CanvasLayer
## menu.close_requested.connect(menu.queue_free)
## menu.open()
## [/codeblock]
class_name A11ySettingsMenu
extends Control

## Emitted by Back or ui_cancel. Settings are already saved.
signal close_requested

const CATEGORY_LABELS: Dictionary = {
	"text": "Text",
	"subtitles": "Subtitles and captions",
	"audio": "Audio",
	"visual": "Visuals",
	"motion": "Motion and flashing",
	"guidance": "Guidance",
	"hud": "HUD and effects",
	"content": "Content",
	"input": "Controls",
	"assist": "Gameplay assists",
	"narration": "Narration",
	"memory": "Reminders and hints",
	"system": "System",
}
const VALUE_LABELS: Dictionary = {
	"none": "Off",
	"protan": "Red-weak (protan)",
	"deutan": "Green-weak (deutan)",
	"tritan": "Blue-weak (tritan)",
	"on_request": "On request",
	"dyslexia_friendly": "Dyslexia-friendly",
}

## Font size at text.scale = 1.0. Headers are 1.4x.
@export var base_font_size: int = 22
@export var title_text: String = "Accessibility"
## Keep this truthful (core-policy section 6).
@export var intro_text: String = "Change these settings at any time."
## Narrator for menu speech. Null: the menu creates its own child narrator.
@export var narrator: A11yNarrator
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null

var _controls: Dictionary = {}
var _value_labels: Dictionary = {}
var _focus_chain: Array[Control] = []
var _undo_button: Button
var _theme: Theme
var _updating: bool = false
var _content: Control
var _connected_store: A11ySettings = null


func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS
	set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	_theme = Theme.new()
	_theme.set_type_variation(&"A11yHeader", &"Label")
	theme = _theme
	if narrator == null:
		narrator = A11yNarrator.new()
		narrator.name = "Narrator"
		add_child(narrator)
	var store: A11ySettings = _settings()
	if store != null:
		_connected_store = store
		store.setting_changed.connect(_on_setting_changed)
		store.preset_applied.connect(_on_preset_state_changed)
		store.preset_undone.connect(_on_preset_state_changed)
	build()


func _exit_tree() -> void:
	if _connected_store != null and is_instance_valid(_connected_store):
		_connected_store.setting_changed.disconnect(_on_setting_changed)
		_connected_store.preset_applied.disconnect(_on_preset_state_changed)
		_connected_store.preset_undone.disconnect(_on_preset_state_changed)
	_connected_store = null


func _unhandled_input(event: InputEvent) -> void:
	if is_visible_in_tree() and event.is_action_pressed(&"ui_cancel"):
		get_viewport().set_input_as_handled()
		_close()


func _settings() -> A11ySettings:
	return settings if settings != null else A11ySettings.active()


## Shows the menu, focuses the first control and announces the title.
func open() -> void:
	show()
	focus_first()
	narrator.speak(title_text, A11yNarrator.Priority.HIGH, true, "focus")


func focus_first() -> void:
	if not _focus_chain.is_empty():
		_focus_chain[0].grab_focus()


## The interactive control for setting [param id], or null.
func get_control_for(id: String) -> Control:
	return _controls.get(id) as Control


## Every focusable control in navigation order.
func get_focus_chain() -> Array[Control]:
	return _focus_chain.duplicate()


## (Re)builds the whole screen from the registry.
func build() -> void:
	if _content != null:
		remove_child(_content)
		_content.queue_free()
	_controls.clear()
	_value_labels.clear()
	_focus_chain.clear()
	var store: A11ySettings = _settings()

	var background: Panel = Panel.new()
	background.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	var scroll: ScrollContainer = ScrollContainer.new()
	scroll.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	scroll.follow_focus = true
	scroll.horizontal_scroll_mode = ScrollContainer.SCROLL_MODE_DISABLED
	background.add_child(scroll)
	var margin: MarginContainer = MarginContainer.new()
	margin.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	for side: String in ["left", "right", "top", "bottom"]:
		margin.add_theme_constant_override("margin_" + side, 24)
	scroll.add_child(margin)
	var list: VBoxContainer = VBoxContainer.new()
	list.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	list.add_theme_constant_override("separation", 10)
	margin.add_child(list)
	_content = background
	add_child(background)

	list.add_child(_header(title_text))
	list.add_child(_wrapped_label(intro_text))
	if store == null:
		list.add_child(_wrapped_label("No accessibility settings store found. Enable the Game Accessibility plugin."))
		_link_focus()
		return

	var presets: Array[Dictionary] = store.get_presets()
	if not presets.is_empty():
		list.add_child(_header("Presets"))
		list.add_child(_wrapped_label("Each preset changes the settings listed in its description. You can adjust any of them afterwards."))
		var row: HFlowContainer = HFlowContainer.new()
		for preset: Dictionary in presets:
			var button: Button = _button(str(preset.get("label", preset["id"])))
			var summary: String = _preset_summary(store, preset)
			button.tooltip_text = summary
			button.set_meta(&"a11y_hint", summary)
			button.pressed.connect(_on_preset_pressed.bind(str(preset["id"])))
			row.add_child(button)
		_undo_button = _button("Undo preset")
		_undo_button.disabled = not store.can_undo_preset()
		_undo_button.pressed.connect(_on_undo_pressed)
		row.add_child(_undo_button)
		list.add_child(row)

	for category: String in store.get_categories():
		var title: String = str(CATEGORY_LABELS.get(category, category.capitalize()))
		list.add_child(_header(title))
		for id: String in store.get_ids_in_category(category):
			list.add_child(_setting_row(store, id))
		var reset: Button = _button("Reset %s" % title)
		reset.set_meta(&"a11y_hint", "Returns every %s setting to its default." % title)
		reset.pressed.connect(_on_reset_pressed.bind(category))
		list.add_child(reset)

	var back: Button = _button("Back")
	back.pressed.connect(_close)
	list.add_child(back)
	_link_focus()
	_apply_text_scale()
	narrator.attach_to_tree(self)


## Text for a value, as shown and narrated ("150%", "75 degrees", "On request").
func format_value(id: String, value: Variant) -> String:
	var store: A11ySettings = _settings()
	var def: Dictionary = store.get_definition(id) if store != null else {}
	match str(def.get("type", "")):
		"bool":
			return "On" if value else "Off"
		"enum":
			return humanize(str(value))
		"int":
			return str(int(value))
		"string":
			return str(value) if not str(value).is_empty() else "empty"
		"float":
			if float(def.get("max", 1.0)) <= 3.0:
				return "%d%%" % roundi(float(value) * 100.0)
			if id == "motion.fov":
				return "%d degrees" % roundi(float(value))
			return str(snappedf(float(value), 0.01))
	return str(value)


static func humanize(value: String) -> String:
	if VALUE_LABELS.has(value):
		return VALUE_LABELS[value]
	return value.replace("_", " ").capitalize()


## Sets the theme's default font size from text.scale. Use the same idea for your game's root theme.
static func apply_text_scale_to_theme(target: Theme, base_size: int, scale_factor: float) -> void:
	target.default_font_size = roundi(base_size * scale_factor)


func _setting_row(store: A11ySettings, id: String) -> Control:
	var def: Dictionary = store.get_definition(id)
	var label_text: String = str(def.get("label", id))
	var description: String = str(def.get("description", ""))
	var row: VBoxContainer = VBoxContainer.new()
	var line: HBoxContainer = HBoxContainer.new()
	line.add_theme_constant_override("separation", 16)
	row.add_child(line)
	var control: Control
	match str(def.get("type", "")):
		"bool":
			var check: CheckButton = CheckButton.new()
			check.text = label_text
			check.size_flags_horizontal = Control.SIZE_EXPAND_FILL
			check.toggled.connect(_on_bool_toggled.bind(id))
			control = check
		"float", "int":
			line.add_child(_name_label(label_text))
			var slider: HSlider = HSlider.new()
			slider.min_value = float(def.get("min", 0.0))
			slider.max_value = float(def.get("max", 1.0))
			slider.step = float(def.get("step", 0.0 if str(def["type"]) == "float" else 1.0))
			slider.custom_minimum_size = Vector2(240.0, 32.0)
			slider.size_flags_vertical = Control.SIZE_SHRINK_CENTER
			slider.value_changed.connect(_on_range_changed.bind(id))
			control = slider
		"enum":
			line.add_child(_name_label(label_text))
			var option: OptionButton = OptionButton.new()
			for value: Variant in def.get("values", []):
				option.add_item(humanize(str(value)))
			option.item_selected.connect(_on_option_selected.bind(id))
			control = option
		"string":
			# Free text such as hud.hidden_elements or visual.custom_colors. A game should replace
			# these with friendlier pickers (checkbox list, colour picker) that write the same string.
			line.add_child(_name_label(label_text))
			var edit: LineEdit = LineEdit.new()
			edit.custom_minimum_size.x = 280.0
			edit.text_submitted.connect(_on_text_submitted.bind(edit, id))
			edit.focus_exited.connect(_on_text_focus_exited.bind(edit, id))
			control = edit
		_:
			control = _name_label("%s (unsupported type)" % label_text)
	line.add_child(control)
	if control is Range:
		var value_label: Label = Label.new()
		value_label.custom_minimum_size.x = 110.0
		line.add_child(value_label)
		_value_labels[id] = value_label
	if control.focus_mode != Control.FOCUS_NONE or control is BaseButton or control is Range or control is LineEdit:
		control.focus_mode = Control.FOCUS_ALL
		control.set_meta(&"a11y_name", label_text)
		control.set_meta(&"a11y_hint", description)
		if "accessibility_name" in control:
			control.set("accessibility_name", label_text)
			control.set("accessibility_description", description)
		_controls[id] = control
		_focus_chain.append(control)
	row.add_child(_wrapped_label(description))
	_sync_control(id)
	return row


func _button(text: String) -> Button:
	var button: Button = Button.new()
	button.text = text
	button.focus_mode = Control.FOCUS_ALL
	_focus_chain.append(button)
	return button


func _header(text: String) -> Label:
	var label: Label = Label.new()
	label.text = text
	label.theme_type_variation = &"A11yHeader"
	return label


func _name_label(text: String) -> Label:
	var label: Label = Label.new()
	label.text = text
	label.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	return label


func _wrapped_label(text: String) -> Label:
	var label: Label = Label.new()
	label.text = text
	label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	label.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	return label


func _preset_summary(store: A11ySettings, preset: Dictionary) -> String:
	var parts: PackedStringArray = PackedStringArray()
	var changes: Dictionary = preset.get("changes", {})
	for id: String in changes:
		if store.has_setting(id):
			parts.append("%s: %s" % [str(store.get_definition(id).get("label", id)), format_value(id, changes[id])])
	return "Sets " + ", ".join(parts) + "."


func _link_focus() -> void:
	var count: int = _focus_chain.size()
	for i: int in count:
		var control: Control = _focus_chain[i]
		var previous: Control = _focus_chain[(i - 1 + count) % count]
		var next: Control = _focus_chain[(i + 1) % count]
		control.focus_neighbor_top = control.get_path_to(previous)
		control.focus_neighbor_bottom = control.get_path_to(next)
		control.focus_previous = control.get_path_to(previous)
		control.focus_next = control.get_path_to(next)


func _sync_control(id: String) -> void:
	var store: A11ySettings = _settings()
	var control: Control = _controls.get(id) as Control
	if store == null or control == null:
		return
	var value: Variant = store.get_value(id)
	_updating = true
	if control is LineEdit:
		if not control.has_focus():
			(control as LineEdit).text = str(value)
	elif control is BaseButton and not (control is OptionButton):
		(control as BaseButton).set_pressed_no_signal(bool(value))
	elif control is Range:
		(control as Range).set_value_no_signal(float(value))
	elif control is OptionButton:
		var values: Array = store.get_definition(id).get("values", [])
		(control as OptionButton).select(values.find(str(value)))
	_updating = false
	var text: String = format_value(id, value)
	control.set_meta(&"a11y_value_text", text)
	if _value_labels.has(id):
		(_value_labels[id] as Label).text = text


func _apply_text_scale() -> void:
	var store: A11ySettings = _settings()
	var factor: float = store.get_float("text.scale", 1.0) if store != null else 1.0
	apply_text_scale_to_theme(_theme, base_font_size, factor)
	_theme.set_font_size(&"font_size", &"A11yHeader", roundi(base_font_size * 1.4 * factor))


func _close() -> void:
	var store: A11ySettings = _settings()
	if store != null:
		store.save_settings()
	close_requested.emit()


func _on_bool_toggled(pressed: bool, id: String) -> void:
	if not _updating:
		_settings().set_value(id, pressed)
		_sync_control(id)


func _on_range_changed(value: float, id: String) -> void:
	if not _updating:
		_settings().set_value(id, value)
		_sync_control(id)


func _on_option_selected(index: int, id: String) -> void:
	if not _updating:
		var values: Array = _settings().get_definition(id).get("values", [])
		if index >= 0 and index < values.size():
			_settings().set_value(id, values[index])
		_sync_control(id)


func _on_text_submitted(text: String, edit: LineEdit, id: String) -> void:
	_commit_text(text, edit, id)


func _on_text_focus_exited(edit: LineEdit, id: String) -> void:
	_commit_text(edit.text, edit, id)


func _commit_text(text: String, edit: LineEdit, id: String) -> void:
	if _updating:
		return
	_settings().set_value(id, text.strip_edges())
	edit.text = _settings().get_string(id)
	_sync_control(id)


func _on_preset_pressed(preset_id: String) -> void:
	_settings().apply_preset(preset_id)


func _on_undo_pressed() -> void:
	_settings().undo_last_preset()


func _on_reset_pressed(category: String) -> void:
	_settings().reset_category(category)


func _on_preset_state_changed(_preset_id: String) -> void:
	if _undo_button != null:
		_undo_button.disabled = not _settings().can_undo_preset()


func _on_setting_changed(id: String, _value: Variant) -> void:
	_sync_control(id)
	if id == "text.scale":
		_apply_text_scale()
