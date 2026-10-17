## Applies guidance.level to tagged guidance visuals (architecture.md, section 6).
##
## Tag each guidance visual with its channel group:
## [constant GROUP_PAINT] (painted ledges, climbable marks) -> guidance.paint,
## [constant GROUP_TRAIL] (glowing trails, breadcrumbs) -> guidance.trail,
## [constant GROUP_HIGHLIGHTS] (interaction outlines and highlights) -> guidance.highlights,
## [constant GROUP_GUIDANCE] (any other guidance) -> guidance.level,
## [constant GROUP_OBJECTIVE] (objective markers) -> guidance.objective_marker.
## A channel key set to "follow" (the default) uses guidance.level, so a player can keep trails and
## turn paint off. Hazard warnings, attack telegraphs and interaction prompts use
## [constant GROUP_HAZARD]: this layer never hides or dims them, even if they are also in a
## guidance group.
##
## Levels: off = hidden; on_request = hidden until [method show_on_request]; subtle = 40% intensity;
## standard = as authored; boosted = 150% (brighter; add an outline in your shader).
## Intensity reaches each node as: a [code]set_guidance_intensity(value: float)[/code] method if it has one,
## otherwise the alpha (and brightness when boosted) of [code]modulate[/code] on CanvasItems and Decals,
## or [code]transparency[/code] on GeometryInstance3D. The value is also stored in the node's
## [code]guidance_intensity[/code] meta.
class_name A11yGuidanceLayer
extends Node

const GROUP_GUIDANCE: StringName = &"a11y_guidance"
const GROUP_PAINT: StringName = &"a11y_guidance_paint"
const GROUP_TRAIL: StringName = &"a11y_guidance_trail"
const GROUP_HIGHLIGHTS: StringName = &"a11y_guidance_highlights"
## Channel group -> setting key. A value of "follow" defers to guidance.level.
const CHANNEL_KEYS: Dictionary = {
	&"a11y_guidance_paint": "guidance.paint",
	&"a11y_guidance_trail": "guidance.trail",
	&"a11y_guidance_highlights": "guidance.highlights",
	&"a11y_guidance": "guidance.level",
	&"a11y_objective_marker": "guidance.objective_marker",
}
const GROUP_OBJECTIVE: StringName = &"a11y_objective_marker"
const GROUP_HAZARD: StringName = &"a11y_hazard"
const LEVEL_INTENSITY: Dictionary = {
	"off": 0.0,
	"on_request": 1.0,
	"subtle": 0.4,
	"standard": 1.0,
	"boosted": 1.5,
}
const BASE_MODULATE_META: StringName = &"_a11y_base_modulate"

## How long [method show_on_request] reveals on-request guidance.
@export var request_seconds: float = 4.0
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null

var _request_left: float = 0.0
var _connected_store: A11ySettings = null


func _ready() -> void:
	var store: A11ySettings = _settings()
	if store != null:
		_connected_store = store
		store.setting_changed.connect(_on_setting_changed)
	get_tree().node_added.connect(_on_node_added)
	apply()


func _exit_tree() -> void:
	if _connected_store != null and is_instance_valid(_connected_store):
		_connected_store.setting_changed.disconnect(_on_setting_changed)
	_connected_store = null
	if get_tree().node_added.is_connected(_on_node_added):
		get_tree().node_added.disconnect(_on_node_added)


func _process(delta: float) -> void:
	tick(delta)


func _settings() -> A11ySettings:
	return settings if settings != null else A11ySettings.active()


## Re-applies the current levels to every tagged node in the tree.
func apply() -> void:
	if not is_inside_tree():
		return
	for group: StringName in CHANNEL_KEYS:
		for node: Node in get_tree().get_nodes_in_group(group):
			apply_node(node)


## Shows on-request guidance for [param seconds] (default [member request_seconds]). Bind it to your
## hint action.
func show_on_request(seconds: float = -1.0) -> void:
	_request_left = seconds if seconds > 0.0 else request_seconds
	apply()


## Counts down an on-request reveal. [method Node._process] calls this.
func tick(delta: float) -> void:
	if _request_left <= 0.0:
		return
	_request_left -= delta
	if _request_left <= 0.0:
		_request_left = 0.0
		apply()


## Applies the right level to one node. Hazard nodes are left untouched.
func apply_node(node: Node) -> void:
	if node == null or node.is_in_group(GROUP_HAZARD):
		return
	var level: String = level_for_node(node)
	var shown: bool = level != "off" and (level != "on_request" or _request_left > 0.0)
	var intensity: float = float(LEVEL_INTENSITY.get(level, 1.0)) if shown else 0.0
	set_intensity(node, shown, intensity)


## Effective level of a channel key: its own value unless that is "follow" (or the key was pruned),
## in which case guidance.level.
func effective_level(key: String) -> String:
	var store: A11ySettings = _settings()
	if store == null:
		return "standard"
	var overall: String = store.get_string("guidance.level", "standard")
	var own: String = store.get_string(key, "follow")
	return overall if own == "follow" else own


## Effective level for a node from its first channel group.
func level_for_node(node: Node) -> String:
	for group: StringName in CHANNEL_KEYS:
		if node.is_in_group(group):
			return effective_level(CHANNEL_KEYS[group])
	return effective_level("guidance.level")


## Sets visibility and intensity on one node.
static func set_intensity(node: Node, shown: bool, intensity: float) -> void:
	if "visible" in node:
		node.set("visible", shown)
	node.set_meta(&"guidance_intensity", intensity)
	if node.has_method("set_guidance_intensity"):
		node.call("set_guidance_intensity", intensity)
	elif node is CanvasItem or node is Decal:
		if not node.has_meta(BASE_MODULATE_META):
			node.set_meta(BASE_MODULATE_META, node.get("modulate"))
		var base: Color = node.get_meta(BASE_MODULATE_META)
		var brightness: float = maxf(intensity, 1.0)
		node.set("modulate", Color(base.r * brightness, base.g * brightness, base.b * brightness, base.a * minf(intensity, 1.0)))
	elif node is GeometryInstance3D:
		(node as GeometryInstance3D).transparency = 1.0 - clampf(intensity, 0.0, 1.0)


func _on_setting_changed(id: String, _value: Variant) -> void:
	if id.begins_with("guidance."):
		apply()


func _on_node_added(node: Node) -> void:
	_apply_late.call_deferred(node)


# Groups added in a node's own _ready() are visible only after node_added, hence the deferred call.
func _apply_late(node: Variant) -> void:
	if is_instance_valid(node) and (node as Node).is_inside_tree():
		var tagged: Node = node
		for group: StringName in CHANNEL_KEYS:
			if tagged.is_in_group(group):
				apply_node(tagged)
				return
