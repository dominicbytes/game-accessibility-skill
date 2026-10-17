#!/usr/bin/env python3
"""Scan a game project for accessibility *signals* (candidate mechanics and content).

Part of the game-accessibility skill. Standard library only; read-only; never modifies files.

Hits are LEADS, not proof. A file called spider_web.png may be an unused asset; a game with
no "flash" identifier may still flash. Confirm every hit by opening it, then record the
signal as present / suspected / absent in ACCESSIBILITY.md (see references/signal-routing.md).

Usage:
    python scan_signals.py <project_dir> [--format markdown|json] [--max-files-per-signal 3]
"""
from __future__ import annotations

import argparse
import json
import os
import re
import sys
from collections import defaultdict

# Directories that are generated, vendored or belong to tools (including this skill).
SKIP_DIRS = {
    ".git", ".hg", ".svn", ".godot", ".import", ".mono", "Library", "Temp", "Obj", "obj",
    "Logs", "UserSettings", "Intermediate", "Binaries", "DerivedDataCache", "Saved",
    "node_modules", "build", "dist", "export", "exports", ".venv", "venv", "__pycache__",
    ".vs", ".vscode", ".idea", ".claude", ".agents", ".cursor", ".codex", ".gemini", ".github",
}

TEXT_EXTS = {
    ".gd", ".gdshader", ".shader", ".tscn", ".tres", ".godot", ".cfg", ".ini", ".cs", ".cpp",
    ".cc", ".c", ".h", ".hpp", ".js", ".mjs", ".ts", ".tsx", ".jsx", ".lua", ".gml", ".yy",
    ".rpy", ".py", ".rs", ".java", ".kt", ".json", ".yaml", ".yml", ".toml", ".md", ".txt",
    ".csv", ".po", ".pot", ".ink", ".yarn", ".twee", ".tw", ".html", ".hlsl", ".glsl", ".usf",
    ".ush", ".prefab", ".unity", ".asset", ".mat", ".controller", ".anim", ".xml", ".lvl",
}

MAX_BYTES = 2_000_000

# Engine sidecar files duplicate their asset's name; licence texts are prose, not game content.
SIDECAR_EXTS = {".import", ".uid", ".meta"}
# Only prose documents are skipped by name; code files are always scanned.
SKIP_FILE_RE = re.compile(
    r"^(licen[cs]e|copying|notice|changelog|authors|credits)(\.(md|txt|rst))?$|(^|-)ofl\.txt$",
    re.IGNORECASE)

ROUTER = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "references", "signal-routing.md")


def load_router_guides() -> dict[str, str]:
    """Guide names per signal, read from signal-routing.md so the scanner never disagrees with it."""
    guides: dict[str, str] = {}
    try:
        with open(ROUTER, encoding="utf-8") as fh:
            for line in fh:
                m = re.match(r"^\|\s*(S\d{3})\b[^|]*\|[^|]*\|[^|]*\|([^|]*)\|", line)
                if m:
                    guides[m.group(1)] = m.group(2).strip()
    except OSError:
        pass
    return guides

# signal id -> (label, guide, regex). Patterns match identifiers, text and file names, case-insensitive.
SIGNALS: dict[str, tuple[str, str, str]] = {
    "S004": ("Colour conveys state", "color-and-clarity",
             r"team_?colou?r|rarity|colou?r_?cod|wire_?colou?r|key_?colou?r|faction_?colou?r"),
    "S006": ("Gameplay-significant audio", "audio-and-captions",
             r"AudioStreamPlayer|AudioSource|PlaySound|play_sfx|audio_play_sound|SoundCue|MetaSound"),
    "S013": ("Timed inputs", "timing-and-speed",
             r"timing_?window|perfect_?(hit|timing|block)|reaction_?time|just_?frame|beat_?map|rhythm"),
    "S014": ("Quick-time events", "input-and-motor",
             r"\bqte\b|quick_?time|qte_|_qte"),
    "S015": ("Button mashing", "input-and-motor",
             r"\bmash|tap_?count|press_?count|clicks_?per|rapid_?press"),
    "S016": ("Sustained holds", "input-and-motor",
             r"hold_?(duration|time|to_)|press_?and_?hold|charge_?(time|attack|shot)"),
    "S019": ("Simultaneous inputs / chords", "input-and-motor",
             r"\bchord|combo_?input|simultaneous_?press|both_?buttons"),
    "S033": ("Precise aiming", "combat-aiming-stealth",
             r"\baim(ing|_assist|_down)?\b|reticle|crosshair|headshot|recoil|bullet_?spread"),
    "S035": ("Ammo / reload management", "difficulty-and-assists",
             r"\bammo\b|magazine_?size|\breload"),
    "S036": ("Parry / dodge / perfect block", "combat-aiming-stealth",
             r"\bparry|perfect_?block|dodge_?window|i_?frames|invuln_?frames"),
    "S040": ("Stamina / oxygen / energy meter", "difficulty-and-assists",
             r"stamina|oxygen|\bbreath\b|energy_?meter|endurance"),
    "S041": ("Damage and death", "difficulty-and-assists",
             r"take_?damage|apply_?damage|TakeDamage|ApplyDamage|on_?hit|hurtbox|\bdie\(\)|game_?over"),
    "S042": ("Grabs / struggles", "input-and-motor",
             r"grabbed|struggle|break_?free|escape_?grab"),
    "S043": ("Jumps, gaps, platforms", "traversal-and-platforming",
             r"\bjump|coyote|ledge|wall_?jump|double_?jump|\bpit\b|fall_?death|kill_?zone"),
    "S044": ("Climbing / ropes / ladders", "traversal-and-platforming",
             r"\bclimb|\bgrip\b|\brope\b|ladder|mantle"),
    "S045": ("Chase / escape / autoscroll", "traversal-and-platforming",
             r"\bchase|pursuit|escape_?sequence|auto_?scroll|death_?wall"),
    "S046": ("Stealth detection", "combat-aiming-stealth",
             r"stealth|detection|suspicion|alert_?level|visibility_?meter|noise_?level|line_?of_?sight"),
    "S051": ("Moving / first-person camera", "camera-and-motion",
             r"Camera3D|CinemachineVirtualCamera|CinemachineCamera|SpringArm|UCameraComponent|first_?person|mouse_?look|look_?sensitivity|head_?bob"),
    "S052": ("Camera shake", "camera-and-motion",
             r"\bshake|trauma|CameraShake"),
    "S054": ("Zoom / FOV kick / blur / DoF", "camera-and-motion",
             r"motion_?blur|MotionBlur|fov_?(kick|punch)|zoom_?punch|speed_?lines|radial_?blur|depth_?of_?field|\bdof_"),
    "S056": ("Parallax / scrolling / animated menus", "camera-and-motion",
             r"parallax|ParallaxBackground|ParallaxLayer|scroll_?speed"),
    "S057": ("Screen distortion overlays", "camera-and-motion",
             r"chromatic|aberration|vignette|distortion|film_?grain"),
    "S058": ("Flashing / strobing / flicker", "photosensitivity",
             r"flash|strobe|flicker|lightning|muzzle|white_?out|\bblink"),
    "S060": ("Particles and stacked effects", "sensory-load-and-guidance",
             r"GPUParticles|CPUParticles|ParticleSystem|Niagara|emitter"),
    "S061": ("Guidance paint / trails / highlights", "sensory-load-and-guidance",
             r"guidance|guide_?paint|yellow_?paint|breadcrumb|waypoint_?trail|highlight_?(ledge|path|interact)|glow_?trail|outline_?interact"),
    "S062": ("HUD / notifications", "sensory-load-and-guidance",
             r"\bhud\b|notification|\btoast|damage_?number|floating_?text"),
    "S063": ("Dark / low-visibility world", "color-and-clarity",
             r"darkness|flashlight|\btorch\b|low_?light|\bfog\b"),
    "S066": ("Spoken dialogue (voice)", "audio-and-captions",
             r"voice_?(line|over|acting|clip)|\bvo_|voiced|lip_?sync|radio_?(line|chatter)"),
    "S068": ("Positional / directional audio", "audio-and-captions",
             r"AudioStreamPlayer3D|AudioStreamPlayer2D|spatial_?(audio|blend)|attenuation|footstep"),
    "S070": ("Loud sudden sounds", "audio-and-captions",
             r"scream|\bsting\b|explosion|jump_?scare|jumpscare"),
    "S073": ("Text-heavy dialogue / UI", "text-and-readability",
             r"dialogu?e|conversation|(?-i:\.(?:ink|yarn))(?:[\x22\x27]|$)|story_?text|codex|lore_?entry"),
    "S074": ("Typewriter / animated text", "text-and-readability",
             r"typewriter|visible_?characters|visible_?ratio|text_?speed|reveal_?speed|char_?delay"),
    "S075": ("Timed choices", "timing-and-speed",
             r"choice_?timer|timed_?choice|dialogu?e_?timer|response_?timer"),
    "S081": ("Quests / objectives", "memory-and-navigation",
             r"\bquests?\b|quest_|objective|\bmissions?\b|journal|task_?list"),
    "S082": ("Long / multi-session story", "memory-and-navigation",
             r"chapter|save_?slot|story_?flag|relationship|affinity"),
    "S084": ("Navigation / maps", "memory-and-navigation",
             r"minimap|world_?map|waypoint|compass|NavigationAgent|NavMesh"),
    "S086": ("Puzzles", "memory-and-navigation",
             r"puzzle|riddle|cipher|keypad|combination_?lock|wiring|sliding_?tile|lever_?sequence"),
    "S087": ("Inventory / equipment", "structured-ui-and-strategy",
             r"inventory|item_?slot|equipment|\bloot"),
    "S088": ("Crafting / recipes", "structured-ui-and-strategy",
             r"\bcraft|recipe|ingredient|workbench"),
    "S097": ("Time limits", "timing-and-speed",
             r"countdown|time_?limit|timer_?expired|time_?left|time_?remaining"),
    "S099": ("Permadeath / run reset", "difficulty-and-assists",
             r"permadeath|ironman|delete_?save|run_?reset|roguelike|roguelite"),
    "S101": ("Resource scarcity", "difficulty-and-assists",
             r"scarce|drop_?rate|\bration|durability|battery|batteries"),
    "S106": ("Possible unrecoverable states", "difficulty-and-assists",
             r"out_?of_?bounds|soft_?lock|unstuck|respawn_?point|checkpoint"),
    "S107": ("Achievements / grades / leaderboards", "store-claims-and-compliance",
             r"achievement|trophy|SteamUserStats|unlock_?achievement|ReportProgress|leaderboard|\bgrade\b|\brank_?(s|ing)?\b"),
    "S109": ("Spiders / arachnids", "content-and-phobias",
             r"spider|arachn|tarantula|broodmother|web_?(nest|sac)"),
    "S110": ("Insects / swarms / larvae", "content-and-phobias",
             r"insect|\bswarm|larva|maggot|cockroach|\broach|\bwasp|\bbees?\b|mosquito|\bgrubs?\b|centipede|\bbugs?_?(enemy|swarm)"),
    "S111": ("Snakes / serpents", "content-and-phobias",
             r"\bsnake|serpent|\bviper|\bcobra|slither"),
    "S112": ("Clowns", "content-and-phobias",
             r"\bclown|jester|harlequin"),
    "S113": ("Needles / medical procedures", "content-and-phobias",
             r"needle|syringe|injection|scalpel|surgery"),
    "S114": ("Blood / gore", "content-and-phobias",
             r"\bblood|\bgore\b|\bgib|dismember|decapitat|\bcorpse|viscera|splatter"),
    "S115": ("Deep water / drowning", "content-and-phobias",
             r"underwater|deep_?water|\bocean|\babyss|\bdrown|\bswim|\bdiving\b"),
    "S116": ("Heights / exposed edges", "content-and-phobias",
             r"vertigo|cliff_?edge|skyscraper|glass_?floor|look_?down"),
    "S117": ("Clustered holes (trypophobia)", "content-and-phobias",
             r"trypo|honeycomb|lotus_?pod|egg_?cluster|hive_?wall"),
    "S118": ("Jump scares", "content-and-phobias",
             r"jump_?scare|jumpscare|scare_?(event|sting|trigger)"),
    "S119": ("Potentially traumatic themes", "content-and-phobias",
             r"suicide|self_?harm|\babuse|torture|addiction"),
    "S123": ("Voice chat", "multiplayer-and-communication",
             r"\bvoip|voice_?chat|VoiceChat|Vivox|microphone|push_?to_?talk"),
    "S124": ("Text chat", "multiplayer-and-communication",
             r"text_?chat|chat_?(box|log|message)|ChatMessage|send_?chat"),
    "S127": ("Local multiplayer / split screen", "multiplayer-and-communication",
             r"split_?screen|local_?multiplayer|couch_?co_?op|player_?two|\bp2_"),
    "S130": ("Competitive / ranked play", "multiplayer-and-communication",
             r"\branked|matchmaking|\bmmr\b|\belo\b"),
    "S134": ("Touch input", "touch-and-mobile",
             r"InputEventScreenTouch|InputEventScreenDrag|touchstart|\bswipe|\bpinch|virtual_?joystick|TouchPhase"),
    "S137": ("VR / XR", "vr-and-xr",
             r"XROrigin|OpenXR|XRCamera|OVRCameraRig|XRController|MotionController|\bxr_"),
    "S144": ("Controllable vehicles", "vehicles",
             r"VehicleBody|vehicle|car_?controller|\bsteering|throttle|aircraft|spaceship|WheeledVehicle|\bkart"),
    "S156": ("Mods / plugins / workshop", "mods-hardware-and-platforms",
             r"mod_?loader|ModLoader|load_?mods?|workshop|BepInEx|HarmonyPatch"),
}

EXISTING_A11Y = (
    r"subtitle|caption|colou?r_?blind|daltoni|accessib|a11y|remap|rebind|text_?size|font_?scale|"
    r"reduce_?motion|invert_?y|screen_?reader|text_?to_?speech|\btts_|assist_?mode|difficulty"
)


def iter_files(root: str):
    for dirpath, dirnames, filenames in os.walk(root):
        dirnames[:] = [d for d in dirnames if d not in SKIP_DIRS and not d.startswith(".")]
        for name in filenames:
            yield os.path.join(dirpath, name)


def scan(root: str, max_files: int):
    compiled = {sid: re.compile(pat, re.IGNORECASE) for sid, (_, _, pat) in SIGNALS.items()}
    existing_re = re.compile(EXISTING_A11Y, re.IGNORECASE)
    hits: dict[str, dict] = defaultdict(lambda: {"count": 0, "files": {}, "terms": defaultdict(int)})
    existing: dict[str, int] = defaultdict(int)
    scanned = 0
    for path in iter_files(root):
        rel = os.path.relpath(path, root).replace(os.sep, "/")
        base = os.path.basename(path)
        ext = os.path.splitext(base)[1].lower()
        if ext in SIDECAR_EXTS or SKIP_FILE_RE.search(base):
            continue
        # File names count for every file (asset names such as spider_idle.png).
        for sid, rx in compiled.items():
            m = rx.search(base)
            if m:
                h = hits[sid]
                h["count"] += 1
                h["terms"][m.group(0).lower()] += 1
                h["files"].setdefault(rel, "filename")
        if ext not in TEXT_EXTS:
            continue
        try:
            if os.path.getsize(path) > MAX_BYTES:
                continue
            with open(path, "r", encoding="utf-8", errors="ignore") as fh:
                lines = fh.readlines()
        except OSError:
            continue
        scanned += 1
        for lineno, line in enumerate(lines, 1):
            if existing_re.search(line):
                existing[rel] += 1
            for sid, rx in compiled.items():
                m = rx.search(line)
                if m:
                    h = hits[sid]
                    h["count"] += 1
                    h["terms"][m.group(0).lower()] += 1
                    h["files"].setdefault(rel, f"line {lineno}")
    router = load_router_guides()
    result = []
    for sid, (label, guide, _) in SIGNALS.items():
        guide = router.get(sid, guide)
        if sid not in hits:
            continue
        h = hits[sid]
        files = list(h["files"].items())[:max_files]
        terms = sorted(h["terms"].items(), key=lambda kv: -kv[1])[:5]
        result.append({
            "signal": sid, "label": label, "guide": guide, "hits": h["count"],
            "examples": [{"file": f, "where": w} for f, w in files],
            "terms": [t for t, _ in terms],
        })
    no_hits = [{"signal": sid, "label": SIGNALS[sid][0]} for sid in SIGNALS if sid not in hits]
    not_scannable = sorted(set(router) - set(SIGNALS))
    existing_files = sorted(existing.items(), key=lambda kv: -kv[1])[:10]
    return {"root": os.path.abspath(root), "text_files_scanned": scanned, "candidates": result,
            "no_hits": no_hits, "not_scannable": not_scannable, "existing_accessibility_mentions": [
                {"file": f, "lines": n} for f, n in existing_files]}


def to_markdown(report: dict) -> str:
    out = [f"# Accessibility signal scan: `{report['root']}`", "",
           f"Text files scanned: {report['text_files_scanned']}. "
           "**These are candidates.** Open each example, confirm the signal is reachable in the "
           "shipped game, then record it as present/suspected/absent in ACCESSIBILITY.md.", "",
           "Baseline areas (first launch, text, remapping, colour/sound redundancy, "
           "photosensitivity, audio controls, pause, no-penalty) are assessed for every game "
           "regardless of this scan.", "",
           "| Signal | Candidate | Hits | Top terms | Examples | Guide |",
           "| --- | --- | ---: | --- | --- | --- |"]
    for c in sorted(report["candidates"], key=lambda c: c["signal"]):
        ex = "<br>".join(f"`{e['file']}` ({e['where']})" for e in c["examples"])
        out.append(f"| {c['signal']} | {c['label']} | {c['hits']} | {', '.join(c['terms'])} | {ex} | "
                   f"{c['guide']} |")
    out += ["", "## No hits (absence of a match is not proof of absence)", ""]
    out += [", ".join(f"{n['signal']} {n['label']}" for n in report["no_hits"]) or "None", ""]
    out += ["## Signals this script cannot detect (assess them from the design and by playing)", ""]
    out += [", ".join(report.get("not_scannable", [])) or "None", ""]
    out += ["## Existing accessibility-related code (verify that it works)", ""]
    if report["existing_accessibility_mentions"]:
        out += [f"- `{e['file']}`: {e['lines']} matching lines" for e in report["existing_accessibility_mentions"]]
    else:
        out.append("- None found.")
    out.append("")
    return "\n".join(out)


def main(argv: list[str]) -> int:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("project_dir")
    ap.add_argument("--format", choices=["markdown", "json"], default="markdown")
    ap.add_argument("--max-files-per-signal", type=int, default=3)
    args = ap.parse_args(argv)
    if not os.path.isdir(args.project_dir):
        print(f"error: not a directory: {args.project_dir}", file=sys.stderr)
        return 2
    report = scan(args.project_dir, args.max_files_per_signal)
    if args.format == "json":
        print(json.dumps(report, indent=2))
    else:
        sys.stdout.reconfigure(encoding="utf-8")
        print(to_markdown(report))
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
