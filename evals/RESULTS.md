# Evaluation results

Date: 2026-10-17. Prompts and rubrics are in [SCENARIOS.md](SCENARIOS.md). Baselines (no skill) are
in [BASELINE_RED.md](BASELINE_RED.md).

How the GREEN runs worked:
- Each was a fresh general-purpose subagent, told the skill path and to read `SKILL.md` first.
- No access to `evals/`, `research/` or `tests/`.
- No file writes; text answer only.
- One point per rubric item, half a point for a partial answer.

## Scores

| Scenario | Baseline, Sonnet 5.5, no skill | With skill, Sonnet 5.5 | With skill, Opus 5.5 |
| --- | --- | --- | --- |
| A. FPS survival horror, build (12) | 0.5 | **12** | **12** |
| B. Unity JRPG retrofit (10) | 3.5 | **9.5** | 9.5 |
| C. Cozy puzzle, negative control (8) | not run | not run | 7.5 |
| D. Unreal storm + lockpicking hook (6) | not run | not run | **6** |

The baseline numbers were scored after the fact from the notes in `BASELINE_RED.md`:
- **A:** half a point, for subtitles with speaker names but no sound visualiser.
- **B:** 3.5 points:
  - element icons (1);
  - timing scale plus auto-succeed (1);
  - text without a dyslexia font (0.5);
  - battle speed, journal and recap without encounter options (0.5);
  - a flash threshold without the safety-first structure (0.5).

## What changed with the skill

**A (both models).**
- Accessibility services were built in weeks 1–2.
- Each feature shipped with its hooks:
  - the spider variant came in the same week as the spider;
  - grab-struggle modes, chase skip through the normal completion path and jump-scare modes were planned with those features.
- Assists were split by channel: damage, infinite ammo and batteries, aggression, and separate fail reasons.
- Health was never shown by tint alone, and flashlight flicker went through FlashGuard with the 1 s and 5 s budgets.
- The stealth awareness state was shown on at least three channels.
- Wiring puzzles use symbols.
- The first-launch screen comes before logos and audio.
- Both answers stated the no-penalty rule, drafted `ACCESSIBILITY.md`, and included the timing formula test and honest "blocked" items.

**B (both models).**
- The Hard-boss achievements were reported as a policy conflict for the owner to decide, with redesign options. The baseline had recommended excluding assist users.
- Narration was kept with an explicit scope decision: `AccessibilityHierarchy` on Unity 6.3+, otherwise TTS. It was not dismissed.
- Both answers added encounter rate, auto-battle and battle speed, plus reward-parity checks, and limited Steam claims to verified features.

**C.**
- Recorded combat, timers, speech, camera and phobia signals as absent and added none of their options.
- Planned shape pairing, colour profiles and contrast checks.
- The win sparkle went through FlashGuard and particle density.
- Hints come in steps, and a skipped level is shown exactly like a solved one.

**D.**
- Lightning is a gameplay event separate from its flash, and every stroke goes through FlashGuard.
- Thunder and rain get captions, with their own ambience bus and a reduce-loud-sounds option.
- Lockpicking got a scaled timing window, a slow/auto mode, a visual beat track with an optional haptic channel, a skip through the normal completion path, and a "Lit" state that is not shown by the flash alone.
- It loaded no JRPG, vehicle or VR guides.

## Gaps found and fixes made (REFACTOR)

| Run | Gap | Fix |
| --- | --- | --- |
| C (Opus) | The plan was thorough but long for a game jam (rubric 8: half a point). | `SKILL.md` step 4 now says: "Scale the answer to the project". For jams, prototypes and deadlines under a month, lead with a short prioritised must-ship list and move the rest to "later", but never cut the no-penalty rule or a safety item. The Sonnet B re-run, made after this fix, produced exactly that split. |
| B (Opus) | Presets were not mentioned (rubric 7: half a point). | `retrofit-audit.md` now lists registry presets (editable, with undo) among the cheap late retrofits. The Sonnet B re-run included editable presets. |
| B (Sonnet) | The persistence check was implied, not listed as a verification step (rubric 10: half a point). | None. Persistence is already in the `verification.md` baseline checks, and the answer stated the persistence requirement. |
| A (Opus) | The answer warned that the Godot `CaptionView` might slow down with `Engine.time_scale`. | Not a defect. `caption_view.gd` divides delta by `Engine.time_scale`, and the Unity view uses unscaled time. |

## Not covered

- Baselines for C and D were not run, so those two scores only show how the skill performs, not how much it adds over the baseline.
- No run used a real game folder, so `scan_signals.py` was exercised only in development, on a local jam game. It was not run as part of these scenarios.
- Each scenario was run once per model. The scores show what the skill does, not a statistical result.
