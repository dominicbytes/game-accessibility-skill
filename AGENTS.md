# AGENTS.md: working on this repository

This repo **is** the `game-accessibility` skill. Agents editing it follow these rules. For using the
skill in a game, see [README.md](README.md).

- The installable skill is `skills/game-accessibility/`. Everything it needs must live inside that
  folder. Never link from it to `research/`, `evals/` or `tests/`.
- Follow [CONTRIBUTING.md](CONTRIBUTING.md): cite sources, keep stable IDs and setting keys, keep
  `SKILL.md` under 250 lines, use the topic template, and don't duplicate guidance across guides.
- Never weaken [`core-policy.md`](skills/game-accessibility/references/core-policy.md). Options stay
  optional, reversible and available before the barrier. No achievement locks, save marks or
  shaming copy.
- When you change `assets/settings-registry.json`, copy it to every engine sample
  (`engines/godot/addons/game_accessibility/settings_registry.json`,
  `engines/unity/Runtime/Resources/settings_registry.json`,
  `engines/unreal/Resources/settings_registry.json`).
- Before finishing, run:
  - `python tests/check_skill.py` (must exit 0);
  - `tests/run_godot_tests.sh` if you touched the Godot sample (needs `REDOT_BIN` or `GODOT_BIN`).
    Use bounded, muted runs only (`--headless --audio-driver Dummy`, a timeout, never `-d`).
- Report what you could not verify (for example Unity or Unreal compilation) instead of claiming it
  works.
