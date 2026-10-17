## One semantic audio event (architecture.md, section 4). Author these instead of bare audio files:
## the same event drives subtitles, sound captions, the sound visualiser and narration, and it fires
## even when audio is muted.
class_name A11yCaptionEvent
extends Resource

enum Importance { LOW, NORMAL, CRITICAL }

## Stable id, for logs and localisation.
@export var id: String = ""
## dialogue = spoken line; sound = important non-speech sound; music = music cue.
@export_enum("dialogue", "sound", "music") var kind: String = "dialogue"
## Text or translation key. Sound captions describe the sound: "Alarm blaring".
@export_multiline var text: String = ""
## Speaker name or key. Use "Unknown voice" for a speaker the player cannot identify yet.
@export var speaker: String = ""
## Whether [member source_position] is meaningful.
@export var has_source_position: bool = false
## World position of the source (3D; for 2D use Vector3(x, y, 0) and set [member direction]).
@export var source_position: Vector3 = Vector3.ZERO
## Direction word ("left", "right", "behind", "above", "below", "ahead"). Filled by the bus from
## [member source_position] when empty and a listener is set.
@export var direction: String = ""
@export var importance: Importance = Importance.NORMAL
## Seconds on screen (real time, not affected by game speed).
@export var duration: float = 3.0
## Events with the same non-empty key replace each other instead of stacking (looping sounds).
@export var merge_key: String = ""
## False for unvoiced lines: narration.dialogue reads them aloud.
@export var voiced: bool = true
## True when the scene can hold this line until the player continues (dialogue boxes, paused
## conversations). With text.auto_advance off, [A11yCaptionView] then turns its pages only on
## [method A11yCaptionView.advance_page] instead of by time.
@export var can_wait: bool = false


## Convenience constructor for a dialogue line.
static func dialogue(speaker_name: String, line: String, seconds: float = 3.0, is_voiced: bool = true) -> A11yCaptionEvent:
	var event: A11yCaptionEvent = A11yCaptionEvent.new()
	event.kind = "dialogue"
	event.speaker = speaker_name
	event.text = line
	event.duration = seconds
	event.voiced = is_voiced
	return event


## Convenience constructor for a sound caption.
static func sound(description: String, key: String = "", seconds: float = 2.5) -> A11yCaptionEvent:
	var event: A11yCaptionEvent = A11yCaptionEvent.new()
	event.kind = "sound"
	event.text = description
	event.merge_key = key
	event.duration = seconds
	return event
