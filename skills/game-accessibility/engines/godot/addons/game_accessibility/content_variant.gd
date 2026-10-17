## Swaps the presentation of sensitive content (architecture.md, section 7).
##
## Put this node inside an enemy or prop and give it one child per level of [member setting_id],
## named by the value ("0", "1", "2", "3" for content.spiders; "full", "reduced", "off" for
## content.blood), or list them in [member variants] by level index. Only the matching variant is
## visible and processing; the others are hidden, process-disabled and their audio players paused.
## Hitboxes, AI, damage and rewards live outside this node and are never touched.
##
## The swap happens in _ready (before the first frame is drawn) and synchronously when the setting
## changes, so no frame of the original is shown. A missing variant falls back to the nearest MORE
## filtered one, never to a less filtered one. When no variant at or above the requested level
## exists, the presentation is hidden (or [member neutral_fallback] is shown, if authored) and a
## warning lists the missing levels; author those variants before shipping. [method missing_levels]
## reports gaps for a validation pass.
class_name A11yContentVariant
extends Node

## Emitted when no variant at or above [param requested] exists. [param missing] lists the
## unauthored levels from the requested one upward; [param used_fallback] is true when
## [member neutral_fallback] is shown, false when the presentation is hidden.
signal coverage_missing(setting_id: String, requested: String, missing: PackedStringArray, used_fallback: bool)

## Setting that selects the level (int or enum key).
@export var setting_id: String = "content.spiders"
## Content tag from signal-routing (arachnid, gore, needle...). Joins group "a11y_content_<tag>".
@export var tag: String = "arachnid"
## Optional explicit variants by level index. Empty: children named by level value.
@export var variants: Array[NodePath] = []
## Optional explicitly authored neutral presentation (for example a crate or a silhouette that does
## not show the filtered content), used only when no variant at or above the requested level exists.
## Empty: the presentation stays hidden instead.
@export var neutral_fallback: NodePath = NodePath()
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null
## Print a warning when coverage is missing. Tests switch this off and use [signal coverage_missing].
var warn_on_missing: bool = true

var _current: Node = null
var _connected_store: A11ySettings = null


func _ready() -> void:
	if not tag.is_empty():
		add_to_group(StringName("a11y_content_" + tag))
	var store: A11ySettings = _settings()
	if store != null:
		_connected_store = store
		store.setting_changed.connect(_on_setting_changed)
	apply()


func _exit_tree() -> void:
	if _connected_store != null and is_instance_valid(_connected_store):
		_connected_store.setting_changed.disconnect(_on_setting_changed)
	_connected_store = null


func _settings() -> A11ySettings:
	return settings if settings != null else A11ySettings.active()


## The variant node currently shown.
func current_variant() -> Node:
	return _current


## Level names in order from original (index 0) to most filtered.
func level_names() -> PackedStringArray:
	var store: A11ySettings = _settings()
	var names: PackedStringArray = PackedStringArray()
	if store == null or not store.has_setting(setting_id):
		return names
	var def: Dictionary = store.get_definition(setting_id)
	if str(def.get("type", "")) == "enum":
		for value: Variant in def.get("values", []):
			names.append(str(value))
	else:
		for level: int in range(int(def.get("min", 0)), int(def.get("max", 0)) + 1):
			names.append(str(level))
	return names


## Shows the variant for the current setting value.
func apply() -> void:
	var names: PackedStringArray = level_names()
	var nodes: Array[Node] = []
	for i: int in names.size():
		nodes.append(_variant_at(i, names[i]))
	var wanted: int = 0
	var store: A11ySettings = _settings()
	if store != null and store.has_setting(setting_id):
		wanted = maxi(0, names.find(str(store.get_value(setting_id))))
	var target: Node = null
	for i: int in range(wanted, nodes.size()):
		if nodes[i] != null:
			target = nodes[i]
			break
	var fallback: Node = get_node_or_null(neutral_fallback) if not neutral_fallback.is_empty() else null
	if target == null and not names.is_empty():
		target = fallback
		_report_missing(names, nodes, wanted, fallback != null)
	for node: Node in nodes:
		if node != null:
			_set_active(node, node == target)
	if fallback != null and not nodes.has(fallback):
		_set_active(fallback, fallback == target)
	_current = target


## Levels (by name) that have no variant node. Use it in a validation pass or a scene test.
func missing_levels() -> PackedStringArray:
	var names: PackedStringArray = level_names()
	var missing: PackedStringArray = PackedStringArray()
	for i: int in names.size():
		if _variant_at(i, names[i]) == null:
			missing.append(names[i])
	return missing


func _report_missing(names: PackedStringArray, nodes: Array[Node], wanted: int, used_fallback: bool) -> void:
	var missing: PackedStringArray = PackedStringArray()
	for i: int in range(wanted, names.size()):
		if nodes[i] == null:
			missing.append(names[i])
	if warn_on_missing:
		push_warning("A11yContentVariant '%s': no variant at or above %s=%s (missing levels: %s); %s." % [
			str(get_path()) if is_inside_tree() else String(name), setting_id, names[wanted],
			", ".join(missing), "showing the neutral fallback" if used_fallback else "presentation hidden"])
	coverage_missing.emit(setting_id, names[wanted], missing, used_fallback)


func _variant_at(index: int, level_name: String) -> Node:
	if not variants.is_empty():
		return get_node_or_null(variants[index]) if index < variants.size() and not variants[index].is_empty() else null
	return get_node_or_null(NodePath(level_name))


static func _set_active(node: Node, active: bool) -> void:
	if "visible" in node:
		node.set("visible", active)
	node.process_mode = Node.PROCESS_MODE_INHERIT if active else Node.PROCESS_MODE_DISABLED
	_pause_audio(node, not active)


static func _pause_audio(node: Node, paused: bool) -> void:
	if node is AudioStreamPlayer or node is AudioStreamPlayer2D or node is AudioStreamPlayer3D:
		node.set("stream_paused", paused)
	for child: Node in node.get_children():
		_pause_audio(child, paused)


func _on_setting_changed(id: String, _value: Variant) -> void:
	if id == setting_id:
		apply()
