## Every flash, strobe, lightning strike, hit flash and muzzle flash requests through this node
## (architecture.md, section 8). Put one in the tree (for example in the HUD) and find it with
## [method find], so all sources share one rate limit.
##
## Counting rule (conservative, used in every mode): a request counts toward the budgets when its
## screen area is at least [member area_threshold] (0.25), OR its intensity is at least
## [member intensity_threshold] (0.1, the WCAG general-flash luminance change), OR it is saturated
## red. Only requests that are small AND dim AND not red are left uncounted. These defaults are
## sample values: tune them only with evidence from a photosensitivity analysis tool run on captured
## footage, never to let a busier effect through.
##
## Budgets: at most 3 counted flashes in any 1-second window (WCAG 2.3.1 "three flashes") AND at
## most 9 in any 5-second window, which stops a sustained 2-3 Hz strobe (EA IRIS reports that as an
## extended failure). The exports can only lower these limits: values above 3 / 9 are clamped.
## Requests over either limit return 0.0: skip that flash.
##
## Red: pass [code]saturated_red = true[/code] for a red flash (or use [method request_flash_color]).
## Red always counts and must always be drawn through [method desaturate_red], in every mode.
## With motion.reduce_flashing on, flashes become low-intensity values: draw them as a steady,
## slow fade or an icon pulse, not a strobe. This is one prevention layer, not a safety proof: it
## cannot see flashes baked into video, textures or overlapping shaders. Analyse captured footage
## with a photosensitivity tool before claiming anything.
class_name A11yFlashGuard
extends Node

const GROUP: StringName = &"a11y_flash_guard"
## Hard cap for [member max_flashes_per_second]; higher export values are clamped to it.
const HARD_MAX_PER_SECOND: int = 3
## Hard cap for [member max_flashes_per_5_seconds]; higher export values are clamped to it.
const HARD_MAX_PER_5_SECONDS: int = 9

## Emitted when a flash was blocked by the rate limit.
signal flash_blocked(intensity: float, area: float)

## Counted flashes allowed in any 1-second window. Clamped to 0..[constant HARD_MAX_PER_SECOND].
@export var max_flashes_per_second: int = HARD_MAX_PER_SECOND
## Counted flashes allowed in any 5-second window. Clamped to 0..[constant HARD_MAX_PER_5_SECONDS].
@export var max_flashes_per_5_seconds: int = HARD_MAX_PER_5_SECONDS
## Screen fraction (0..1) at or above which a flash counts, whatever its intensity. Sample value.
@export_range(0.0, 1.0) var area_threshold: float = 0.25
## Intensity (relative luminance change 0..1) at or above which a flash counts, whatever its area.
## 0.1 is the WCAG general-flash change. Sample value.
@export_range(0.0, 1.0) var intensity_threshold: float = 0.1
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


## Effective 1-second budget (the export, clamped to the hard cap).
func per_second_budget() -> int:
	return clampi(max_flashes_per_second, 0, HARD_MAX_PER_SECOND)


## Effective 5-second budget (the export, clamped to the hard cap).
func per_5_seconds_budget() -> int:
	return clampi(max_flashes_per_5_seconds, 0, HARD_MAX_PER_5_SECONDS)


## True when a request counts toward the budgets (see the counting rule above).
func counts(intensity: float, area: float, saturated_red: bool = false) -> bool:
	return saturated_red or area >= area_threshold or intensity >= intensity_threshold


## Asks to show a flash of [param intensity] (0..1, luminance change) covering [param area] (0..1 of
## the screen). Returns the intensity you may use: lowered when reduce_flashing is on, 0.0 when the
## flash is blocked. [param at_time] (seconds) overrides the real-time clock for tests and replays;
## the limit uses real time, not game time. Set [param saturated_red] for red flashes: they always
## count, and their colour must go through [method desaturate_red].
func request_flash(intensity: float, area: float = 1.0, at_time: float = -1.0, saturated_red: bool = false) -> float:
	var now: float = at_time if at_time >= 0.0 else Time.get_ticks_usec() / 1000000.0
	var requested: float = clampf(intensity, 0.0, 1.0)
	if requested <= 0.0:
		return 0.0
	var value: float = minf(requested, reduced_intensity) if is_reduced() else requested
	if value <= 0.0:
		return 0.0
	# Classify on the requested intensity, so lowering it never hides a potentially hazardous flash.
	if not counts(requested, clampf(area, 0.0, 1.0), saturated_red):
		return value
	_times = _times.filter(func(t: float) -> bool: return now - t <= 5.0)
	var last_second: int = _times.filter(func(t: float) -> bool: return now - t <= 1.0).size()
	if last_second >= per_second_budget() or _times.size() >= per_5_seconds_budget():
		flash_blocked.emit(value, area)
		return 0.0
	_times.append(now)
	return value


## Colour form of [method request_flash]: classifies [param color] with [method is_saturated_red],
## and returns the colour to draw, already desaturated, with alpha = the allowed intensity
## (alpha 0 = blocked: skip the flash).
func request_flash_color(color: Color, intensity: float, area: float = 1.0, at_time: float = -1.0) -> Color:
	var allowed: float = request_flash(intensity, area, at_time, is_saturated_red(color))
	var tint: Color = desaturate_red(color)
	return Color(tint.r, tint.g, tint.b, allowed)


## Forgets recent flashes (for example after a scene change).
func reset() -> void:
	_times.clear()


## One sRGB channel (0..1, gamma-encoded) converted to linear light, as in the WCAG
## relative-luminance definition.
static func linear_channel(value: float) -> float:
	var c: float = clampf(value, 0.0, 1.0)
	return c / 12.92 if c <= 0.04045 else pow((c + 0.055) / 1.055, 2.4)


## True for saturated red as defined by WCAG: R / (R + G + B) >= 0.8, where R, G and B are the
## linearised sRGB channels of the relative-luminance definition (not the encoded values).
static func is_saturated_red(color: Color) -> bool:
	var r: float = linear_channel(color.r)
	var total: float = r + linear_channel(color.g) + linear_channel(color.b)
	return total > 0.0 and r / total >= 0.8


## Lowers the saturation of saturated red until it is no longer saturated red, so red flashes are
## not hazardous. Other colours are returned unchanged.
static func desaturate_red(color: Color, max_saturation: float = 0.5) -> Color:
	if not is_saturated_red(color):
		return color
	var saturation: float = minf(color.s, max_saturation)
	var result: Color = Color.from_hsv(color.h, saturation, color.v, color.a)
	while is_saturated_red(result) and saturation > 0.0:
		saturation = maxf(saturation - 0.05, 0.0)
		result = Color.from_hsv(color.h, saturation, color.v, color.a)
	return result
