## Swaps the presentation of sensitive content (architecture.md, section 7).
##
## Put this node inside an enemy or prop and give it one child per level of [member setting_id],
## named by the value ("0", "1", "2", "3" for content.spiders; "full", "reduced", "off" for
## content.blood), or list them in [member variants] by level index. Only the matching variant is
## visible and processing; the others are hidden, process-disabled and their audio players paused.
## Hitboxes, AI, damage and rewards live outside this node and are never touched.
##
## The swap happens in _ready (before the first frame is drawn) and synchronously when the setting
## changes, so no frame of the original is shown. A missing variant falls back to the nearest more
## filtered one.
class_name A11yContentVariant
extends Node

## Setting that selects the level (int or enum key).
@export var setting_id: String = "content.spiders"
## Content tag from signal-routing (arachnid, gore, needle...). Joins group "a11y_content_<tag>".
@export var tag: String = "arachnid"
## Optional explicit variants by level index. Empty: children named by level value.
@export var variants: Array[NodePath] = []
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null

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
	if target == null:
		for i: int in range(mini(wanted, nodes.size() - 1), -1, -1):
			if nodes[i] != null:
				target = nodes[i]
				break
	for node: Node in nodes:
		if node != null:
			_set_active(node, node == target)
	_current = target


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
