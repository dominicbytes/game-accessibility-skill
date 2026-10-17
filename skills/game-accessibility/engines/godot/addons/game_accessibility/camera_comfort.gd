## Camera comfort scaling at the camera owner (architecture.md, section 8).
##
## Multiply every shake source (including scripted and cutscene shake) by [method scale_shake], every
## bob by [method scale_bob] and every overlay (vignette, damage overlay, distortion) by
## [method effects_scale]. Motion blur, chromatic aberration and depth of field are separate
## switches. Gate automatic camera
## swings and zoom pulses with [method allow_auto_camera] / [method scale_zoom_pulse].
## Set [member camera] to keep a Camera3D's FOV in sync with motion.fov.
class_name A11yCameraComfort
extends Node

## Camera whose vertical FOV follows motion.fov. Optional.
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


## False while reduce motion is on: skip automatic camera swings, parallax and animated menus.
func allow_auto_camera() -> bool:
	return not reduce_motion()


## Zoom pulse amount; 0 while reduce motion is on.
func scale_zoom_pulse(amount: float) -> float:
	return 0.0 if reduce_motion() else amount


## Applies motion.fov (vertical degrees) to [param target].
func apply_fov(target: Camera3D) -> void:
	if target != null:
		target.fov = clampf(_value("motion.fov", target.fov), 1.0, 179.0)


func _on_setting_changed(id: String, _value_changed: Variant) -> void:
	if camera == null:
		return
	if id == "motion.fov":
		apply_fov(camera)
	elif id == "motion.depth_of_field" and camera.attributes is CameraAttributesPractical:
		apply_depth_of_field(camera.attributes as CameraAttributesPractical)
