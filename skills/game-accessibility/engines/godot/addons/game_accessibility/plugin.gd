## Editor plugin: registers the settings store as the autoload `A11y` when enabled and removes it
## when disabled. The services do not depend on the autoload name; they use A11ySettings.active().
@tool
extends EditorPlugin

const AUTOLOAD_NAME: String = "A11y"


func _enable_plugin() -> void:
	add_autoload_singleton(AUTOLOAD_NAME, _addon_dir().path_join("a11y_settings.gd"))


func _disable_plugin() -> void:
	remove_autoload_singleton(AUTOLOAD_NAME)


func _addon_dir() -> String:
	var script: Script = get_script() as Script
	return script.resource_path.get_base_dir()
