---
name: game-accessibility
description: Use when designing, building, extending, reviewing or releasing any video game or game prototype, in any engine or genre. This includes adding or changing mechanics, controls, combat, QTEs, puzzles, timers, cameras, visual effects, audio or dialogue, menus and settings, HUD, difficulty, enemies, creatures or gore, multiplayer chat, or save systems. Also use when anyone mentions accessibility, a11y, assist modes, colour-blind modes, subtitles or captions, flashing or epilepsy, motion sickness, remapping, difficulty options, phobia modes, dyslexia, screen readers or TTS, and when retrofitting accessibility into an existing game.
license: MIT
metadata:
  version: "1.0.0"
  repository: "https://github.com/dominicbytes/game-accessibility-skill"
---

# Game accessibility

## Overview

Build player-toggled accessibility options **into every game system as it is written**, and
retrofit them into games that forgot. Options are **optional, reversible at any time, available
before the barrier, and never penalised**. Defaults stay the designer's intended experience,
except for safety.

Core principle: **route by what the game actually contains.** A mechanic or content *signal*
(QTEs, spiders, spoken dialogue, colour-coded state, camera shake) selects options and guides.
Load only the guides for signals that are present. An FPS never loads JRPG guidance. A JRPG with
a timed fishing minigame does load the timing guidance for that minigame.

**Violating the letter of these rules is violating the spirit of these rules.**

## Modes

**Every invocation starts with workflow steps 1 and 2** (read core policy, open `ACCESSIBILITY.md`).
Then:

| Mode | When | Then |
| --- | --- | --- |
| **Build** | New game, new milestone plan, or any new feature or content | Steps 3-8 for the whole design. **Planned and in-scope features count as present**: plan their options now and ship them in the same milestone as the feature. |
| **Feature hook** | Adding or changing one mechanic, screen, effect, sound, enemy or level | Steps 3-8 for the signals this change adds or alters (the delta) |
| **Audit / retrofit** | Existing game, pre-release check, "add accessibility", reviewer feedback | [references/retrofit-audit.md](references/retrofit-audit.md), then steps 4-8 |

Paths below are relative to **this skill's folder** (the folder containing this `SKILL.md`), not to
the game. Run scripts as `python "<skill folder>/scripts/<name>.py" "<game folder>"`.

## Workflow

1. **Read [references/core-policy.md](references/core-policy.md).** It is short and binding.
2. **Open or create `ACCESSIBILITY.md`** at the game's root from
   [assets/templates/ACCESSIBILITY.md](assets/templates/ACCESSIBILITY.md). It records signals,
   options, owners, verification and scope decisions across sessions.
3. **Detect signals.** Read the design doc and conversation, and inspect the project. Run
   `scripts/scan_signals.py` on the game folder to get candidate signals: **leads, not proof**.
   Classify each signal as present, suspected, absent or unknown, with evidence. Ask the developer
   short, task-based questions only about ambiguous signals that affect the next decision. Never ask
   about anyone's disabilities. Signal list: [references/signal-routing.md](references/signal-routing.md).
   Genre starting points: [references/genre-recipes.md](references/genre-recipes.md) (read only
   the sections for this game's genres).
4. **Select options.** Baseline options apply to every game. Add the Contextual options each present
   signal triggers. Record Advanced needs with an explicit scope decision; do not drop them silently.
   Use the IDs and keys from [references/option-catalog.md](references/option-catalog.md) and
   [assets/settings-registry.json](assets/settings-registry.json).

   **Scale the answer to the project.** For a game jam, prototype or deadline under a month, lead
   with a short prioritised "must ship" list (safety, Baseline and the options its signals require,
   each with an effort estimate) and put everything else under "later". Cut detail, never the
   no-penalty rule or a safety item.

5. **Load only the matching topic guides** (table below) and the **one engine guide** for this
   project.
6. **Implement through the architecture** in [references/architecture.md](references/architecture.md):
   - a settings store;
   - AssistPolicy;
   - InputAssist;
   - CaptionBus, Narrator, GuidanceLayer, ContentFilter, FlashGuard and ColorTokens.

   Gameplay asks a policy. It never reads raw settings or physical keys. Each feature ships **with**
   its hooks, not in a later milestone.
7. **Verify behaviour** with [references/verification.md](references/verification.md). Run the game.
   Exercise each option, its persistence and its combinations. Record pass, fail, blocked or not
   tested in `ACCESSIBILITY.md`.
8. **Report honestly.** List what was verified, what was blocked (and why) and the known gaps.
   Store-page claims must match verified behaviour:
   [references/topics/store-claims-and-compliance.md](references/topics/store-claims-and-compliance.md).

## Topic guides: load only when the signal is present

| Signal family (examples) | Guide |
| --- | --- |
| Any text, UI, fonts, world text, readability, dyslexia | [text-and-readability](references/topics/text-and-readability.md) |
| Colour carries meaning; dark or cluttered scenes; outlines | [color-and-clarity](references/topics/color-and-clarity.md) |
| Any audio, speech, sound cues, music | [audio-and-captions](references/topics/audio-and-captions.md) |
| Menus (always); narration, TTS, voice input, unvoiced text | [screen-reader-and-voice](references/topics/screen-reader-and-voice.md) |
| Any direct input: holds, mashing, QTEs, chords, sticks, sensitivity | [input-and-motor](references/topics/input-and-motor.md) |
| Timed inputs, countdowns, reaction windows, real-time pace, pause | [timing-and-speed](references/topics/timing-and-speed.md) |
| Damage, death, resources, fail states, skips, saves, difficulty | [difficulty-and-assists](references/topics/difficulty-and-assists.md) |
| Aiming, melee, parry, enemies, stealth detection | [combat-aiming-stealth](references/topics/combat-aiming-stealth.md) |
| Jumps, platforms, climbing, chases, falls | [traversal-and-platforming](references/topics/traversal-and-platforming.md) |
| Moving camera, shake, bob, blur, FOV, parallax, animated UI | [camera-and-motion](references/topics/camera-and-motion.md) |
| **Any visuals (always)**; flashes, strobes, lightning, patterns | [photosensitivity](references/topics/photosensitivity.md) |
| Guidance paint, trails, busy HUD, particles, notifications | [sensory-load-and-guidance](references/topics/sensory-load-and-guidance.md) |
| Objectives, long story, exploration, puzzles, codex, saves | [memory-and-navigation](references/topics/memory-and-navigation.md) |
| Spiders, insects, snakes, needles, gore, deep water, heights, holes, clowns, jump scares, themes | [content-and-phobias](references/topics/content-and-phobias.md) |
| Online or local multiplayer, chat, pings, lobbies | [multiplayer-and-communication](references/topics/multiplayer-and-communication.md) |
| Driving, flying, piloting | [vehicles](references/topics/vehicles.md) |
| Touch, mobile, gestures, orientation | [touch-and-mobile](references/topics/touch-and-mobile.md) |
| VR, AR, motion controls, room-scale | [vr-and-xr](references/topics/vr-and-xr.md) |
| Inventory, crafting, cards, tactics, strategy, management, idle | [structured-ui-and-strategy](references/topics/structured-ui-and-strategy.md) |
| Children, education, early readers | [kids-and-education](references/topics/kids-and-education.md) |
| Config files, mods, adaptive hardware, OS settings, platforms | [mods-hardware-and-platforms](references/topics/mods-hardware-and-platforms.md) |
| Store pages, achievements policy, legal or chat regulation, release | [store-claims-and-compliance](references/topics/store-claims-and-compliance.md) |

## Engine guides: load exactly one

| Engine | Guide and sample code |
| --- | --- |
| Godot 4.x / Redot | [engines/godot/GUIDE.md](engines/godot/GUIDE.md) |
| Unity | [engines/unity/GUIDE.md](engines/unity/GUIDE.md) |
| Unreal Engine 5 | [engines/unreal/GUIDE.md](engines/unreal/GUIDE.md) |
| Web/JS, GameMaker, Ren'Py, Bevy, RPG Maker, Twine, LÖVE, Defold, Construct, Roblox, MonoGame, custom | [engines/other/GUIDE.md](engines/other/GUIDE.md) |

Check the installed engine version before using any API. A newer engine's documentation is not
proof that an API exists in the project's version. When copying sample code into a project, keep
the MIT notice: copy [LICENSE](LICENSE) next to the copied folder or into its third-party notices.

## Red flags: stop and correct course

| Thought | Reality |
| --- | --- |
| "Accessibility settings go in a later milestone" | Hooks ship with the feature. A QTE system without its auto path is unfinished. |
| "Screen reader support is too expensive; skip it" | Menu narration is Baseline. Use the engine's accessibility tree or TTS. Only full nonvisual *gameplay* is Advanced, and it gets a scope decision. |
| "Exclude assist users from the Hard achievements" | Forbidden. Achievements read events, never settings. Report existing conflicts to the owner. |
| The developer asks to turn off achievements or trophies when an option is on | Push back before coding. Explain that it makes players feel bad, makes them stop playing, and can end in a Steam refund. Offer alternatives. See core-policy section 2, "When the developer asks for a penalty". |
| "There's an Easy mode, so assists are covered" | One preset is not independent options. Expose the specific barrier: damage, timing, fall death, resources, speed. |
| "A colour-blind filter covers colour" | Pair every colour with a symbol, shape or label first. Palettes are per-element. Filters are extra at most. |
| "It's just a cosmetic flicker or tint" | You may be creating a barrier: a flash hazard, colour-only state or sound-only cue. Check signals for every new feature. |
| "Spiders, QTEs or chases will get options later" | The signal is present now. Plan the option now and record it in ACCESSIBILITY.md. |
| "Add infinite ammo and FOV, just in case" | Do not add options for mechanics that don't exist. Record the absence instead. |
| "The setting exists, so it works" | Verify behaviour, persistence and combinations in a running build. |
| "I can't run the game, so mark it done" | Mark it `blocked`, name the missing prerequisite, and say what remains untested. |
| "Show an ASSIST badge on the save" | Forbidden. An optional, player-enabled HUD reminder is the only allowed indicator. |

## Done means

- `ACCESSIBILITY.md` lists every signal with its status and every option with its owner and
  verification result.
- Settings are reachable before the first logo, music or speech, and from title and pause.
- Options persist, change live, are isolated from each other, and survive loading an old save.
- Each assist was exercised through real content. Achievements and endings are unaffected.
- Remaining gaps and blocked checks are stated plainly to the developer.
