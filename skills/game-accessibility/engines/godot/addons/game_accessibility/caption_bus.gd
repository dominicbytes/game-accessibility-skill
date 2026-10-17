## Routes semantic audio events to every text, visual and speech output (architecture.md, section 4).
##
## Not an autoload: put one in your HUD or game scene. It joins the group [constant GROUP] so any
## system can find it with [method find]. Captions come from events, never from audio levels, so
## they work with audio muted.
## [codeblock]
## A11yCaptionBus.find(self).post(A11yCaptionEvent.dialogue("Mara", "Keep your head down."))
## [/codeblock]
class_name A11yCaptionBus
extends Node

const GROUP: StringName = &"a11y_caption_bus"

## Every posted event, before filtering (logs, dialogue history).
signal event_posted(event: A11yCaptionEvent)
## Dialogue to show as a subtitle (subtitles.dialogue on).
signal subtitle_requested(event: A11yCaptionEvent)
## Sound or music caption to show (subtitles.sound_captions on).
signal caption_requested(event: A11yCaptionEvent)
## Positioned sound for an on-screen direction indicator (audio.sound_visualizer on).
signal visualizer_requested(event: A11yCaptionEvent)

## Camera or player used to turn [member A11yCaptionEvent.source_position] into a direction word.
@export var listener: Node3D
## Reads unvoiced dialogue aloud when narration.dialogue is on.
@export var narrator: A11yNarrator
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null


func _enter_tree() -> void:
	add_to_group(GROUP)


## First bus in the tree of [param from], or null.
static func find(from: Node) -> A11yCaptionBus:
	if from == null or not from.is_inside_tree():
		return null
	return from.get_tree().get_first_node_in_group(GROUP) as A11yCaptionBus


## Whether [param event] should be displayed as text under the current settings.
static func should_display(event: A11yCaptionEvent, store: A11ySettings) -> bool:
	if event == null or event.text.is_empty():
		return false
	if event.kind == "dialogue":
		return store == null or store.get_bool("subtitles.dialogue", true)
	return store != null and store.get_bool("subtitles.sound_captions")


## Posts one event to every output whose setting is on.
func post(event: A11yCaptionEvent) -> void:
	if event == null:
		return
	var store: A11ySettings = settings if settings != null else A11ySettings.active()
	var routed: A11yCaptionEvent = event
	if event.has_source_position and event.direction.is_empty() and listener != null and listener.is_inside_tree():
		routed = event.duplicate() as A11yCaptionEvent
		routed.direction = direction_word(listener.global_transform, event.source_position)
	event_posted.emit(routed)
	if should_display(routed, store):
		if routed.kind == "dialogue":
			subtitle_requested.emit(routed)
		else:
			caption_requested.emit(routed)
	var positioned: bool = routed.has_source_position or not routed.direction.is_empty()
	if positioned and store != null and store.get_bool("audio.sound_visualizer"):
		visualizer_requested.emit(routed)
	if routed.kind == "dialogue" and not routed.voiced and narrator != null and store != null and store.get_bool("narration.dialogue"):
		var line: String = tr(routed.text)
		if not routed.speaker.is_empty():
			line = "%s: %s" % [tr(routed.speaker), line]
		var priority: A11yNarrator.Priority = A11yNarrator.Priority.HIGH if routed.importance == A11yCaptionEvent.Importance.CRITICAL else A11yNarrator.Priority.NORMAL
		narrator.speak_dialogue(line, priority)


## Direction of [param world_position] relative to a listener transform (forward is -Z).
static func direction_word(listener_transform: Transform3D, world_position: Vector3) -> String:
	var local: Vector3 = listener_transform.affine_inverse() * world_position
	var flat: float = Vector2(local.x, local.z).length()
	if local.y > flat:
		return "above"
	if -local.y > flat:
		return "below"
	var angle: float = rad_to_deg(atan2(local.x, -local.z))
	if absf(angle) <= 30.0:
		return "ahead"
	if absf(angle) >= 150.0:
		return "behind"
	return "right" if angle > 0.0 else "left"


## 2D direction from [param from] to [param to] in screen orientation (y grows downward).
static func direction_word_2d(from: Vector2, to: Vector2) -> String:
	var delta: Vector2 = to - from
	if absf(delta.x) >= absf(delta.y):
		return "right" if delta.x > 0.0 else "left"
	return "below" if delta.y > 0.0 else "above"
