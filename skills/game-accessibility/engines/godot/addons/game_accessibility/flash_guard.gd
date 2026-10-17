## Every flash, strobe, lightning strike, hit flash and muzzle flash requests through this node
## (architecture.md, section 8). Put one in the tree (for example in the HUD) and find it with
## [method find], so all sources share one rate limit.
##
## Always, in every mode, for large or bright flashes: at most [member max_flashes_per_second] in any
## 1-second window (WCAG 2.3.1 "three flashes") AND at most [member max_flashes_per_5_seconds] in any
## 5-second window, which stops a sustained 2-3 Hz strobe (EA IRIS reports that as an extended
## failure). Requests over either limit return 0.0: skip that flash. Pass flash colours through
## [method desaturate_red] in every mode.
## With motion.reduce_flashing on, flashes become low-intensity values: draw them as a steady,
## slow fade or an icon pulse, not a strobe. This is one prevention layer, not a safety proof: it
## cannot see flashes baked into video, textures or overlapping shaders. Analyse captured footage
## with a photosensitivity tool before claiming anything.
class_name A11yFlashGuard
extends Node

const GROUP: StringName = &"a11y_flash_guard"

## Emitted when a flash was blocked by the rate limit.
signal flash_blocked(intensity: float, area: float)

## Large or bright flashes allowed in any 1-second window.
@export var max_flashes_per_second: int = 3
## Large or bright flashes allowed in any 5-second window.
@export var max_flashes_per_5_seconds: int = 9
## Screen fraction (0..1) at or above which a flash counts. WCAG's general-flash area is roughly
## a quarter of a 10-degree field, about 10% of a typical screen.
@export_range(0.0, 1.0) var large_area: float = 0.1
## Intensity (relative luminance change 0..1) at or above which even a small flash counts.
@export_range(0.0, 1.0) var bright_intensity: float = 0.8
## Maximum intensity returned while motion.reduce_flashing is on.
@export_range(0.0, 1.0) var reduced_intensity: float = 0.2
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null

var _times: Array[float] = []


func _enter_tree() -> void:
	add_to_group(GROUP)


## First guard in the tree of [param from], or null.
static func find(from: Node) -> A11yFlashGuard:
	if from == null or not from.is_inside_tree():
		return null
	return from.get_tree().get_first_node_in_group(GROUP) as A11yFlashGuard


func is_reduced() -> bool:
	var store: A11ySettings = settings if settings != null else A11ySettings.active()
	return store != null and store.get_bool("motion.reduce_flashing")


## Asks to show a flash of [param intensity] (0..1, luminance change) covering [param area] (0..1 of
## the screen). Returns the intensity you may use: lowered when reduce_flashing is on, 0.0 when the
## flash is blocked. [param at_time] (seconds) overrides the real-time clock for tests and replays;
## the limit uses real time, not game time.
func request_flash(intensity: float, area: float = 1.0, at_time: float = -1.0) -> float:
	var now: float = at_time if at_time >= 0.0 else Time.get_ticks_usec() / 1000000.0
	var value: float = clampf(intensity, 0.0, 1.0)
	if is_reduced():
		value = minf(value, reduced_intensity)
	if value <= 0.0:
		return 0.0
	if area < large_area and value < bright_intensity:
		return value
	_times = _times.filter(func(t: float) -> bool: return now - t <= 5.0)
	var last_second: int = _times.filter(func(t: float) -> bool: return now - t <= 1.0).size()
	if last_second >= max_flashes_per_second or _times.size() >= max_flashes_per_5_seconds:
		flash_blocked.emit(value, area)
		return 0.0
	_times.append(now)
	return value


## Forgets recent flashes (for example after a scene change).
func reset() -> void:
	_times.clear()


## True for saturated red as defined by WCAG: R / (R + G + B) >= 0.8 (sRGB values).
static func is_saturated_red(color: Color) -> bool:
	var total: float = color.r + color.g + color.b
	return total > 0.0 and color.r / total >= 0.8


## Lowers the saturation of saturated red so red flashes are not hazardous. Other colours unchanged.
static func desaturate_red(color: Color, max_saturation: float = 0.5) -> Color:
	if not is_saturated_red(color):
		return color
	return Color.from_hsv(color.h, minf(color.s, max_saturation), color.v, color.a)
