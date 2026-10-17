## Camera comfort scaling at the camera owner (architecture.md, section 8).
##
## Multiply every shake source (including scripted and cutscene shake) by [method scale_shake], every
## head bob by [method scale_bob], held-item sway and breathing by [method scale_sway] (a separate
## key, motion.weapon_sway), and every overlay (vignette, damage overlay, distortion) by
## [method effects_scale]. Motion blur, chromatic aberration and depth of field are separate
## switches.
##
## motion.reduce_motion is the umbrella; each decorative or automatic motion has its own override
## key ("follow" / "off" / "on", where "on" means the effect is allowed) resolved with
## [method A11ySettings.resolve_toggle] and follow value [code]not reduce_motion[/code]:
## [method allow_auto_camera] (motion.auto_camera), [method allow_fov_effects] (motion.fov_effects),
## [method allow_speed_lines] (motion.speed_lines), [method allow_parallax] (motion.parallax) and
## [method allow_menu_animation] (motion.menu_animation). Zoom pulses are FOV effects:
## [method scale_zoom_pulse] resolves through [method allow_fov_effects]. Set [member camera] to keep
## a Camera3D's FOV in sync with motion.fov.
##
## Dynamic FOV (sprint, aim, boost): write the camera FOV every frame as the player's current base
## FOV plus the authored offset only while [method allow_fov_effects] is true, so turning the effect
## off mid-sprint returns to the base FOV.
class_name A11yCameraComfort
extends Node

## Camera whose vertical FOV follows motion.fov. Optional. Its FOV is also reset to motion.fov when
## FOV effects stop being allowed (motion.fov_effects or motion.reduce_motion changed).
@export var camera: Camera3D
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null

var _connected_store: A11ySettings = null


func _ready() -> void:
	var store: A11ySettings = _settings()
	if store != null:
		_connected_store = store
		store.setting_changed.connect(_on_setting_changed)
	if camera != null:
		apply_fov(camera)
		if camera.attributes is CameraAttributesPractical:
			apply_depth_of_field(camera.attributes as CameraAttributesPractical)


func _exit_tree() -> void:
	if _connected_store != null and is_instance_valid(_connected_store):
		_connected_store.setting_changed.disconnect(_on_setting_changed)
	_connected_store = null


func _settings() -> A11ySettings:
	return settings if settings != null else A11ySettings.active()


func _value(id: String, fallback: float) -> float:
	var store: A11ySettings = _settings()
	return store.get_float(id, fallback) if store != null else fallback


## Shake amplitude after motion.camera_shake (0 removes shake completely).
func scale_shake(amount: float) -> float:
	return amount * _value("motion.camera_shake", 1.0)


## Head-bob amplitude after motion.head_bob.
func scale_bob(amount: float) -> float:
	return amount * _value("motion.head_bob", 1.0)


## Visual sway and breathing motion of held items after motion.weapon_sway (independent of head
## bob). Visual only: aim accuracy and recoil stay as designed (see input.aim_stabilization).
func scale_sway(amount: float) -> float:
	return amount * _value("motion.weapon_sway", 1.0)


## Multiplier for vignette, chromatic aberration, damage overlays and distortion.
func effects_scale() -> float:
	return _value("motion.screen_effects", 1.0)


func motion_blur_enabled() -> bool:
	var store: A11ySettings = _settings()
	return store == null or store.get_bool("motion.motion_blur", true)


func chromatic_aberration_enabled() -> bool:
	var store: A11ySettings = _settings()
	return store == null or store.get_bool("motion.chromatic_aberration", true)


func depth_of_field_enabled() -> bool:
	var store: A11ySettings = _settings()
	return store == null or store.get_bool("motion.depth_of_field", true)


## Turns the authored depth-of-field blur of [param attributes] off while motion.depth_of_field is
## off and restores it when it is on again. The authored flags are kept in metadata.
func apply_depth_of_field(attributes: CameraAttributesPractical) -> void:
	if attributes == null:
		return
	if not attributes.has_meta(&"_a11y_dof"):
		attributes.set_meta(&"_a11y_dof", [attributes.dof_blur_far_enabled, attributes.dof_blur_near_enabled])
	var authored: Array = attributes.get_meta(&"_a11y_dof")
	var allowed: bool = depth_of_field_enabled()
	attributes.dof_blur_far_enabled = allowed and bool(authored[0])
	attributes.dof_blur_near_enabled = allowed and bool(authored[1])


func reduce_motion() -> bool:
	var store: A11ySettings = _settings()
	return store != null and store.get_bool("motion.reduce_motion")


## Whether the motion behind override [param key] is allowed: the key's "on" / "off", or
## [code]not reduce_motion[/code] when it is "follow" or pruned.
func allows_motion(key: String) -> bool:
	var store: A11ySettings = _settings()
	var follow_value: bool = not reduce_motion()
	return store.resolve_toggle(key, follow_value) if store != null else follow_value


## Automatic recentring and camera swings (motion.auto_camera).
func allow_auto_camera() -> bool:
	return allows_motion("motion.auto_camera")


## Field-of-view changes when sprinting, aiming or boosting (motion.fov_effects).
func allow_fov_effects() -> bool:
	return allows_motion("motion.fov_effects")


## Speed lines and radial blur (motion.speed_lines).
func allow_speed_lines() -> bool:
	return allows_motion("motion.speed_lines")


## Parallax and scrolling background layers (motion.parallax).
func allow_parallax() -> bool:
	return allows_motion("motion.parallax")


## Animated menu backgrounds and UI transitions (motion.menu_animation). When false, show static
## menus and instant transitions.
func allow_menu_animation() -> bool:
	return allows_motion("motion.menu_animation")


## Zoom pulse amount; 0 while FOV effects are not allowed (motion.fov_effects, which follows
## motion.reduce_motion unless set to "on" or "off").
func scale_zoom_pulse(amount: float) -> float:
	return amount if allow_fov_effects() else 0.0


## Applies motion.fov (vertical degrees) to [param target].
func apply_fov(target: Camera3D) -> void:
	if target != null:
		target.fov = clampf(_value("motion.fov", target.fov), 1.0, 179.0)


func _on_setting_changed(id: String, _value_changed: Variant) -> void:
	if camera == null:
		return
	if id == "motion.fov":
		apply_fov(camera)
	elif (id == "motion.fov_effects" or id == "motion.reduce_motion") and not allow_fov_effects():
		apply_fov(camera)   # drop an active sprint/aim/boost offset at once
	elif id == "motion.depth_of_field" and camera.attributes is CameraAttributesPractical:
		apply_depth_of_field(camera.attributes as CameraAttributesPractical)
