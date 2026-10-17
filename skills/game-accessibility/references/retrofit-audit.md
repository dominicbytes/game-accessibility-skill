# Audit and retrofit an existing game

Use this when a game is already built, close to release, or a reviewer said "add accessibility".
The goal is a prioritised, honest plan, followed by fixes that reuse the game's existing systems.

## 1. Inventory (read before changing anything)

1. Record the engine and exact version (run the editor binary with `--version` or read the project
   file). List platforms, input devices, languages and the minimum resolution.
2. Run `python scripts/scan_signals.py <project_dir> --format markdown > a11y_scan.md`. Treat the
   hits as **candidates**. Open each one and confirm whether it is reachable in the shipped game.
3. Read the design docs, the level list, the input map, and the menu and settings code. Find:
   - **where settings are stored** (file, format, whether they are separate from saves);
   - **who owns** damage, death, resources, timers, camera, audio buses, dialogue, achievements, saves
     and UI focus;
   - **existing options** (difficulty, subtitles, volume, remapping) and whether they actually work.
4. Play or run the critical path if possible: first launch, the first 10 minutes, one boss or hard
   section, settings, save and load, and the ending.
5. Create or update `ACCESSIBILITY.md` from the template. Mark every signal present, absent or
   unknown, with evidence.

## 2. Existing options: verify, don't trust

For each existing option, run its row from [verification.md](verification.md). Common retrofit
findings:

- Settings reachable only after the intro, tutorial or first cutscene.
- Settings that reset on relaunch or update, or are stored inside story saves.
- Subtitles exist but miss barks, radio and cutscenes. No background. Tiny.
- "Easy" changes enemy health only. Timing, QTE, falls and resources are unchanged.
- Camera shake slider ignores scripted or cutscene shake.
- Remapping covers gameplay but not menus, or prompts show default keys.
- Colour-blind filter is full-screen while the puzzle still uses red/green only.
- Achievements or saves check difficulty or assists (**policy conflict**: report it).

## 3. Prioritise

Score each gap by **blocking × reach × cost**. Fix in this order:

1. **Safety:** hazardous flashing anywhere, including boot, transitions and video. Loud first
   launch. Settings that appear after the first exposure.
2. **Total blockers in the core loop:** mandatory mashing, QTEs, holds or chords with no
   alternative; colour-only or sound-only critical information; unreadable text at minimum
   resolution; menus that need a mouse; no subtitles on speech.
3. **Policy conflicts:** achievement locks, save marks, shaming copy. These need an owner decision
   (see core-policy section 2).
4. **Persistence and live change:** settings must save, apply live and stay out of saves.
5. **Contextual options** triggered by present signals, ordered by how much of the game each
   signal covers.
6. **Advanced needs:** record a scope decision with the owner.

Cheap, high-value retrofits that usually fit even late in development:

- First-launch accessibility screen and settings moved to the title and pause menus.
- Subtitle background, size, speaker names, and turning subtitles on by default.
- Camera shake, bob and screen-effect multipliers at the camera owner.
- A FlashGuard wrapper around existing flash calls.
- Hold-to-toggle and mash-to-hold in the input layer.
- QTE auto mode in the QTE system.
- Damage, falls and infinite-resource checks at the existing damage and resource owners.
- Skip from pause for puzzles and chases.
- Symbols added next to colour-coded states.
- Objective reminder and recap from the existing quest data.
- Menu narration using the engine's accessibility properties or TTS.
- Presets from the registry ("Readable text", "More time"…) on the first-launch screen. Each
  preset only sets independent options that stay editable, with a one-step undo.

## 4. Retrofit without rewriting

- **Wrap, don't replace.** Put the AssistPolicy call inside the existing `take_damage()`, add
  `InputAssist.is_held()` where `Input.is_action_pressed()` is used for sustained actions, and route
  existing `camera.shake()` calls through the comfort scale. Keep the game's architecture and naming.
- **One owner per rule.** If damage is applied in five places, first route them through one function,
  then add the policy there.
- **Settings store:** if the game already has a settings system, extend it with the registry keys
  and the rules from [architecture.md](architecture.md) (validation, isolation, separate file,
  OS-pref import, migration). Don't add a second store.
- **Content you can't re-author:** for baked guidance paint, phobia assets or burned-in subtitles,
  record the gap honestly. Then offer the closest real alternative (an overlay toggle, an encounter
  skip, a caption track) and plan the proper fix.
- **Achievements already published:** do not change definitions silently. Propose redesigns to the
  owner, such as removing the difficulty condition or adding an equivalent path, and follow platform
  procedures.

## 5. Audit report format

Give the developer this report, and keep it in `ACCESSIBILITY.md`:

```markdown
## Accessibility audit: <game> <version/commit> <date>

Engine/platforms: ...   Method: static scan + code review + play of <routes> (+ tools run)
Not tested / blocked: <what and why>

### Safety and policy (fix before release)
| # | Finding | Evidence (file/scene/time) | Impact | Fix | Effort |

### Blockers in the core loop
| # | Finding | Evidence | Who is blocked (barrier, not diagnosis) | Fix | Effort |

### Contextual improvements (by signal)
| Signal | Missing option(s) (IDs) | Fix | Effort |

### Existing options: verification results
| Option | Result (pass/fail/blocked/not tested) | Notes |

### Advanced needs: decisions required
| Need | Options | Recommendation |

### Proposed public accessibility notes (only verified items)
```

Rate effort as S (hours), M (a day or two) or L (a week or more). Never inflate claims, and never
mark something `pass` that was not exercised.
