## Semantic colour tokens with paired symbols (architecture.md, section 9).
##
## Scripts and shaders ask for a token ("team.enemy"), never a literal colour. visual.color_profile
## picks the palette per element; every token also has a symbol that is always shown, so colour is
## never the only cue. Save a copy as a .tres to author your own palettes and symbols.
##
## Default palettes use Okabe-Ito colours. Checked with the Machado 2009 severity-1.0 simulation
## (minimum OKLab distance between any two tokens under the simulated deficiency, higher is better):
## protan 0.096 -> 0.169 with the protan palette, deutan 0.131 -> 0.147, tritan 0.085 -> 0.110.
## Check contrast against your real backgrounds (scripts/contrast_check.py) and test with players.
class_name A11yColorTokens
extends Resource

const DEFAULT_PALETTES: Dictionary = {
	"none": {
		"team.ally": Color("0072b2"),
		"team.enemy": Color("d55e00"),
		"hazard": Color("e69f00"),
		"pickup.rare": Color("cc79a7"),
		"objective": Color("f0e442"),
		"interact": Color("ffffff"),
	},
	"protan": {
		"team.ally": Color("56b4e9"),
		"team.enemy": Color("e69f00"),
		"hazard": Color("f0e442"),
		"pickup.rare": Color("0072b2"),
		"objective": Color("d55e00"),
		"interact": Color("ffffff"),
	},
	"deutan": {
		"team.ally": Color("56b4e9"),
		"team.enemy": Color("e69f00"),
		"hazard": Color("009e73"),
		"pickup.rare": Color("f0e442"),
		"objective": Color("0072b2"),
		"interact": Color("ffffff"),
	},
	"tritan": {
		"team.ally": Color("56b4e9"),
		"team.enemy": Color("e69f00"),
		"hazard": Color("009e73"),
		"pickup.rare": Color("f0e442"),
		"objective": Color("d55e00"),
		"interact": Color("ffffff"),
	},
}

## ASCII placeholders that render in any font. Replace them with icons or shapes in your UI.
const DEFAULT_SYMBOLS: Dictionary = {
	"team.ally": "+",
	"team.enemy": "X",
	"hazard": "!",
	"pickup.rare": "*",
	"objective": ">",
	"interact": "o",
}

## Profile name -> {token: Color}. The "custom" profile reads visual.custom_colors
## ("role=#RRGGBB;role=#RRGGBB") from the store, falling back per token to the "none" palette.
@export var palettes: Dictionary = DEFAULT_PALETTES.duplicate(true)
## Token -> symbol string (or an icon name your UI maps to a texture).
@export var symbols: Dictionary = DEFAULT_SYMBOLS.duplicate(true)
## Returned for unknown tokens.
@export var fallback_color: Color = Color(0.6, 0.6, 0.6)
## Store override; null uses [method A11ySettings.active].
var settings: A11ySettings = null


## Active profile from visual.color_profile ("none" without a store).
func current_profile() -> String:
	var store: A11ySettings = _store()
	return store.get_string("visual.color_profile", "none") if store != null else "none"


## Colour for [param token] in [param profile] (default: the player's profile). Tokens missing from
## a profile fall back to the "none" palette, then to [member fallback_color].
func get_color(token: String, profile: String = "") -> Color:
	var chosen: String = profile if not profile.is_empty() else current_profile()
	var palette: Dictionary = palettes.get(chosen, {})
	if chosen == "custom":
		palette = custom_colors()
	if palette.has(token):
		return palette[token]
	var base: Dictionary = palettes.get("none", {})
	return base.get(token, fallback_color)


## Symbol paired with [param token]; always show it next to the colour.
func get_symbol(token: String) -> String:
	return str(symbols.get(token, ""))


## Tokens defined in the "none" palette.
func get_tokens() -> PackedStringArray:
	var out: PackedStringArray = PackedStringArray()
	for token: Variant in palettes.get("none", {}):
		out.append(str(token))
	return out


## The player's custom colours from visual.custom_colors, as {token: Color}.
func custom_colors() -> Dictionary:
	var store: A11ySettings = _store()
	return parse_custom_colors(store.get_string("visual.custom_colors")) if store != null else {}


## Stores a player-chosen colour in visual.custom_colors (other roles keep their colours).
## Returns false when there is no store or the key was pruned.
func set_custom_color(token: String, color: Color) -> bool:
	var store: A11ySettings = _store()
	if store == null or not store.has_setting("visual.custom_colors"):
		return false
	var colors: Dictionary = custom_colors()
	colors[token] = color
	store.set_value("visual.custom_colors", format_custom_colors(colors))
	emit_changed()
	return true


## Parses "role=#RRGGBB;role=#RRGGBB". Malformed pairs are skipped; the rest are kept.
static func parse_custom_colors(text: String) -> Dictionary:
	var out: Dictionary = {}
	for pair: String in text.split(";", false):
		var parts: PackedStringArray = pair.split("=", false, 1)
		if parts.size() != 2:
			continue
		var role: String = parts[0].strip_edges()
		var code: String = parts[1].strip_edges()
		if role.is_empty() or not Color.html_is_valid(code):
			continue
		out[role] = Color.html(code)
	return out


## Formats {token: Color} as "role=#RRGGBB;..." (sorted by role, so the file diff stays stable).
static func format_custom_colors(colors: Dictionary) -> String:
	var roles: Array = colors.keys()
	roles.sort()
	var pairs: PackedStringArray = PackedStringArray()
	for role: Variant in roles:
		pairs.append("%s=#%s" % [str(role), (colors[role] as Color).to_html(false)])
	return ";".join(pairs)


func _store() -> A11ySettings:
	return settings if settings != null else A11ySettings.active()
