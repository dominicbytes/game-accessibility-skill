## Gameplay assists in one place (architecture.md, section 2).
##
## Gameplay systems call these static functions at the moment of decision. They read the active
## [A11ySettings] store and fall back to designer values when there is no store or the key was
## pruned from the registry. Achievement code must never call this class: achievements read events.
class_name A11yAssistPolicy
extends RefCounted

## Damage sources that follow assist.no_fall_death instead of assist.damage_taken.
const FALL_SOURCES: PackedStringArray = ["fall", "pit", "out_of_bounds"]
## Damage sources that no assist changes (self-inflicted and scripted story damage).
const UNSCALED_SOURCES: PackedStringArray = ["self", "scripted"]
## Resource name -> infinite-resource key. Currencies and quest items are never infinite.
const INFINITE_KEYS: Dictionary = {
	"ammo": "assist.infinite_ammo",
	"stamina": "assist.infinite_stamina",
}


static func _store() -> A11ySettings:
	return A11ySettings.active()


## Damage the player actually takes. Separate channels: "enemy", "hazard" and any unknown source are
## scaled by assist.damage_taken (0 blocks them); falls follow assist.no_fall_death; "self" and
## "scripted" are unchanged. With damage blocked, pits, drowning and grabs still need a recovery
## path in your game (return to the last safe spot, release the grab), never a soft-lock.
static func damage_to_player(amount: float, source: String = "enemy") -> float:
	var store: A11ySettings = _store()
	if store == null or amount <= 0.0:
		return amount
	if FALL_SOURCES.has(source):
		return 0.0 if store.get_bool("assist.no_fall_death") else amount
	if UNSCALED_SOURCES.has(source):
		return amount
	return amount * store.get_float("assist.damage_taken", 1.0)


## Whether a failure state should end the attempt. "fall"/"pit"/"out_of_bounds" follow
## assist.no_fall_death (false = return the player to the last safe spot); "timer" is false when
## assist.timers is "off". Every other reason fails as designed.
static func should_fail(reason: String) -> bool:
	var store: A11ySettings = _store()
	if store == null:
		return true
	if FALL_SOURCES.has(reason):
		return not store.get_bool("assist.no_fall_death")
	if reason == "timer":
		return store.get_string("assist.timers", "normal") != "off"
	return true


## True when [param resource] ("ammo", "stamina") is currently infinite.
static func is_infinite(resource: String) -> bool:
	var store: A11ySettings = _store()
	if store == null or not INFINITE_KEYS.has(resource):
		return false
	return store.get_bool(INFINITE_KEYS[resource])


## Spends [param amount] from [code]holder[property][/code] (an Object or a Dictionary; property
## defaults to the resource name). Returns true when the action may go ahead. With the matching
## infinite assist on, returns true and spends nothing, so "empty" effects never trigger falsely and
## the counter is never set to INF. Returns false (and spends nothing) when not enough is left.
static func consume(resource: String, amount: float, holder: Variant, property: String = "") -> bool:
	if is_infinite(resource):
		return true
	var prop: String = property if not property.is_empty() else resource
	var current: Variant = null
	if holder is Dictionary:
		current = (holder as Dictionary).get(prop)
	elif holder is Object:
		current = (holder as Object).get(prop)
	if not (current is int or current is float):
		push_error("A11yAssistPolicy.consume: '%s' is not a number on %s." % [prop, str(holder)])
		return false
	if float(current) < amount:
		return false
	var next: Variant = float(current) - amount
	if current is int:
		next = int(current) - ceili(amount)
	if holder is Dictionary:
		(holder as Dictionary)[prop] = next
	else:
		(holder as Object).set(prop, next)
	return true


## Multiplier for countdowns that are not races: multiply the starting duration by it.
## normal = 1, extended = 2, off = INF (also check should_fail("timer")).
static func timer_scale() -> float:
	var store: A11ySettings = _store()
	var mode: String = store.get_string("assist.timers", "normal") if store != null else "normal"
	match mode:
		"extended":
			return 2.0
		"off":
			return INF
	return 1.0


## Game speed for simulation clocks only (assist.game_speed). Do not apply it to menus, subtitle
## timing, narration or network clocks. Timing windows are measured in simulation time and scaled
## once by input.timing_window_scale (see [method A11yInputAssist.timing_window]); never multiply a
## window by this value as well.
static func time_scale() -> float:
	var store: A11ySettings = _store()
	return store.get_float("assist.game_speed", 1.0) if store != null else 1.0


## Multiplier for attack frequency and detection speed. 0 means enemies never attack.
static func enemy_aggression() -> float:
	var store: A11ySettings = _store()
	return store.get_float("assist.enemy_aggression", 1.0) if store != null else 1.0


## Multiplier for random-encounter chance. 0 turns random battles off; story battles still happen.
static func encounter_rate() -> float:
	var store: A11ySettings = _store()
	return store.get_float("assist.encounter_rate", 1.0) if store != null else 1.0


## True when party members should choose actions on their own. The player can take control at any
## time, so read this at each decision, not once per battle.
static func auto_battle() -> bool:
	var store: A11ySettings = _store()
	return store != null and store.get_bool("assist.auto_battle")


## Speed multiplier for battle animations only. Decision time (menus, timers) is unaffected.
static func battle_speed() -> float:
	var store: A11ySettings = _store()
	return store.get_float("assist.battle_speed", 1.0) if store != null else 1.0


## Whether scenes listed in the content notices may be skipped (with a short text summary).
## Follows content.scene_skip only; independent of the challenge skip. False when content.scene_skip
## is not in the registry (pruned): the registry default applies only to a registered key.
static func scene_skip_offered() -> bool:
	var store: A11ySettings = _store()
	return store != null and store.get_bool("content.scene_skip", false)


## Whether the pause menu should offer "Skip challenge". Follows assist.skip_available only. False
## when assist.skip_available is not in the registry (pruned).
static func skip_offered() -> bool:
	var store: A11ySettings = _store()
	return store != null and store.get_bool("assist.skip_available", false)


## Requests a challenge skip (puzzle, chase, boss), gated on assist.skip_available. Emits
## [signal A11ySettings.skip_requested] once per id; the challenge completes through its normal
## success path (same rewards, flags, achievements and checkpoint). A repeated request for an id that
## was already skipped returns true without emitting again. Returns false when challenge skips are
## not offered or there is no store.
static func request_skip(challenge_id: String) -> bool:
	if not skip_offered():
		return false
	_store().emit_skip_requested(challenge_id)
	return true


## Requests a sensitive-scene skip (a scene listed in the content notices), gated on
## content.scene_skip, never on assist.skip_available. Reaches the same
## [signal A11ySettings.skip_requested] signal and completion path as [method request_skip], once
## per id. Show the scene's short text summary before continuing. Returns false when scene skips are
## not offered or there is no store.
static func request_scene_skip(scene_id: String) -> bool:
	if not scene_skip_offered():
		return false
	_store().emit_skip_requested(scene_id)
	return true
