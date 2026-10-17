# Game Accessibility Skill

An **agent skill** that makes AI coding agents build player-toggled accessibility options into any
game, in any genre and any engine, **from the first line of code**. Install it in a game repository
and the agents working there will:

- detect what the game actually contains: QTEs, spiders, spoken dialogue, colour-coded puzzles,
  camera shake, timers, chat;
- add the matching options **while** they build each feature, not as a late-stage retrofit;
- load only the guidance that applies. A shooter never reads JRPG advice, and a JRPG with a timed
  fishing minigame still gets timing assists for that minigame;
- verify the options in a running build and keep an `ACCESSIBILITY.md` contract up to date;
- audit and retrofit games where accessibility was forgotten.

Every option is **optional, reversible at any time, available before the barrier it removes, and
never penalised**:

- no achievement locks;
- no save badges;
- no shaming copy.

Defaults stay the designer's intended experience, except for safety (no hazardous flashing, quiet
first launch, settings before logos and speech).

## What's covered

319 catalogued options across 12 areas, routed by 157 mechanic and content signals, with 29 genre
recipes. Examples:

| Area | Examples |
| --- | --- |
| Vision and text | Text and UI scale, readable and dyslexia-friendly fonts, contrast, symbol and colour pairs, colour profiles, outlines, magnifier |
| Hearing | Subtitles with speaker names and direction, sound captions, sound visualiser, separate volume buses, mono, loud-sound reduction |
| Motor | Full remapping, hold to toggle, mash to hold or single press, QTE auto-complete, sequential chords, one-handed play, separate aim slowdown, pull, stabilisation and lock-on, sensitivity |
| Cognitive and memory | Objective reminders, recaps, journals, graduated hints and spoilers, instruction replay, save anywhere, undo |
| Sensory load and ADHD | Guidance paint and trails off / on request / boosted, minimal HUD, particle density, notification modes |
| Photosensitivity | Flash rate guard (at most 3 per second and 9 per 5 seconds in every mode), reduce flashing, red-flash and pattern checks, IRIS analysis workflow |
| Motion comfort | Camera shake, head bob, weapon sway, FOV, motion blur, screen effects, reduce motion (honours the OS setting) with per-effect overrides |
| Phobias and content | Graded spider appearance, insects, snakes, needles, gore, deep water, heights, trypophobia, jump scares, content warnings, scene skips |
| Difficulty and stress | Separate damage, fall and timer fail states, invincibility, infinite resources, enemy aggression, game speed, skips that keep rewards |
| Narration | Engine screen-reader trees (AccessKit, Unity AccessibilityHierarchy, Slate), TTS self-voicing, focus narration, voice input |
| Multiplayer | Text chat, speech-to-text and TTS, pings, accessible lobbies, block and report, per-player profiles |
| Openness | Logical action layer, documented config file, mod hooks, adaptive controllers, OS preference import |

## Install

The skill lives in [`skills/game-accessibility/`](skills/game-accessibility/). It uses the open
[Agent Skills](https://agentskills.io) format: a `SKILL.md` plus references, assets, scripts and
engine samples.

**With the `skills` installer** (Node.js required; review what it installs):

```sh
npx skills add dominicbytes/game-accessibility-skill --skill game-accessibility \
  --agent claude-code codex cursor github-copilot gemini-cli
```

**Manually:** copy the whole `skills/game-accessibility/` folder into your game repository:

| Agent | Folder |
| --- | --- |
| Claude Code | `.claude/skills/game-accessibility/` |
| Codex, Cursor, GitHub Copilot, Gemini CLI | `.agents/skills/game-accessibility/` |

Keep the folder intact: `SKILL.md` links to its files by relative path, and the folder carries its
own copy of the MIT [`LICENSE`](skills/game-accessibility/LICENSE).

**Copying an engine sample** (for example `engines/godot/addons/game_accessibility/` into a Godot
project): keep the copyright and permission notice with it. Copy
`skills/game-accessibility/LICENSE` into the copied folder, or add its text to your project's
third-party notices. An SPDX tag or the `license` field alone is not the notice text.

**Recommended:** add a short pointer to your project's `AGENTS.md` (or `CLAUDE.md`), so agents use
the skill for every relevant change, not only when asked:

```markdown
## Accessibility
For any change to game mechanics, controls, menus, audio, visuals, camera, difficulty, content or
saves, use the `game-accessibility` skill. Keep `ACCESSIBILITY.md` current. Accessibility options
are optional and must never penalise achievements, content, endings or saves.
```

Then ask once to confirm it loads. In Claude Code use `/game-accessibility`. In Codex use
`$game-accessibility`. For a finished game, say *"audit this game's accessibility"*.

## How it works

```
SKILL.md                         modes, workflow, routing tables, red flags (short)
references/core-policy.md        binding rules: optional, reversible, no penalty, safe defaults, wording
references/signal-routing.md     S001-S157 signals → options → guide (the router)
references/genre-recipes.md      29 genre starting points (genre only seeds the scan)
references/option-catalog.md     319 options with IDs, tiers, defaults, setting keys, thresholds
references/architecture.md       settings store, AssistPolicy, InputAssist, CaptionBus, Narrator,
                                 GuidanceLayer, ContentFilter, FlashGuard, CameraComfort, ColorTokens
references/topics/*.md           22 topic guides, loaded only when their signal is present
references/verification.md       checks per signal, combinations, release gate
references/retrofit-audit.md     audit workflow and report format
assets/settings-registry.json    90 canonical setting keys + 7 presets, shared by every engine
assets/templates/ACCESSIBILITY.md the per-game contract agents maintain
scripts/scan_signals.py          finds candidate signals in a project (stdlib Python)
scripts/contrast_check.py        WCAG contrast ratios for theme colours
engines/godot|unity|unreal|other sample code and integration guides
```

## Engine samples

| Engine | Status |
| --- | --- |
| **Godot 4.3+ / Redot** | Drop-in addon (settings store, policies, auto-generated accessible settings menu, captions, TTS narrator, flash guard, content variants, guidance layer, colour tokens). **43/43 tests pass headless on Redot 26.3** (earlier rounds also on 26.3-stable) (`bash tests/run_godot_tests.sh`, with `REDOT_BIN` set). TTS speech, AccessKit screen-reader output and exported builds were not exercised. |
| **Unity 2022.3+ / 6** | Plain C# core (netstandard2.1) plus Unity adapters. The engine-free core is compiled with Roslyn `csc` and exercised by `bash tests/unity_core/run_unity_core_tests.sh`. The Unity adapters were only compiled against stubs and have **never run in the Unity Editor**, so please report problems. |
| **Unreal Engine 5** | Runtime module (subsystems and components). **Not compiled here**, because there was no Unreal or C++ toolchain. Please report compile errors. |
| Others | A guide for web/canvas, Phaser, PixiJS, GameMaker, RPG Maker MZ, Ren'Py, Twine, Bevy, MonoGame/FNA, LÖVE, Defold, Construct, Roblox and Minecraft modding, plus a minimal JS reference. |

## Research and evidence

[`research/`](research/) holds the source reports behind the skill, with links to primary
sources:
- the Game Accessibility Guidelines, Xbox Accessibility Guidelines, AbleGamers APX, WCAG, ITU-R BT.1702;
- platform documentation;
- developer accessibility pages;
- reviewers such as Can I Play That?, Steve Saylor, SightlessKombat and Laura Kate Dale;
- player communities;
- open-source code.

The reports were compiled by AI research agents (6.1 Sol) and synthesized by Opus 5.5.
Unverified items are labelled `UNVERIFIED`. The installed skill does not load these reports. They
exist so that claims can be checked and updated.

[`evals/`](evals/) records how agents behaved **without** the skill (missed arachnophobia mode for
giant spiders, recommended locking achievements for assist users, dismissed screen readers as "too
expensive"), the scenarios used to test it, and the [results with the skill](evals/RESULTS.md).

## Not legal advice, not certification

The skill helps teams build and verify accessibility options. It does not certify compliance with
any law (CVAA, the European Accessibility Act and others) or platform requirement, and it does not
guarantee seizure safety. It tells agents to escalate those questions to the people responsible.
Test with disabled players: [AbleGamers Player Panels](https://www.ablegamers.org/player-panels/) is
one route.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Corrections backed by sources, new shipped examples, engine
samples and test reports from real projects are all welcome. Run `python tests/check_skill.py`
before opening a pull request.

## Licence

[MIT](LICENSE). The installable folder includes the same notice at
[`skills/game-accessibility/LICENSE`](skills/game-accessibility/LICENSE); keep it when you copy the
skill or any engine sample. Third-party fonts, tools and libraries mentioned in the guides keep their own licences.

## About Dominic Bytes

Greetings! I am Dominic Bytes, the synth walker. I hail from the distant future. Where brains occupy robot bodies, time travel is a trip to the corner store, and the neon glow of our attire is powered by the light of our souls. Join me on a 1.21 gigawatt powered journey of chill vibes with gaming, anime, movies, and more!

- [Website](https://dominicbytes.carrd.co/)
- [X](https://x.com/DominicBytes)
- [Twitch](https://www.twitch.tv/dominicbytes)
- [YouTube](http://www.youtube.com/@DominicBytes)
- [Kick](https://kick.com/dominicbytes)
