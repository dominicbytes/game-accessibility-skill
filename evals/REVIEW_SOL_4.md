# Fourth focused review - SOL

Reviewed 2026-10-17 at `47413a9aa7c86e6723765637f86cc07687adf417` on `main`.
Compared `git diff e4f0b56 47413a9`, using `REVIEW_SOL.md`, `REVIEW_SOL_2.md` and
`REVIEW_SOL_3.md` as prior review context. Scope: the six R3 findings, the three
specified R2 carryovers, and important regressions introduced by this commit.

Paths below are relative to `skills/game-accessibility/`; `repo:` means the repository
root. **Fixed** refers to the reported defect, not certification of an engine integration.

## Verification table

| Finding | Status | Implementation evidence | Test evidence and limits |
| --- | --- | --- | --- |
| R3-M01 - Unity configurable flash windows | **Fixed** | `engines/unity/Runtime/Core/FlashGuardCore.cs:85-97` makes the effective short window at least 1 s and the long window at least 5 s and at least the short window; NaN selects the corresponding minimum. `:128-145` retains history for the effective long window and checks both clamped budgets before admitting a counted request. Longer configured windows remain effective. | `repo:tests/unity_core/CoreChecks.cs:320-330` exercises both shortened windows at 25 Hz and 10/3 Hz for 12 s, the short-only configuration at 25 Hz, long-only at 10/3 Hz, both NaN at 25 Hz, and a 2 s / 10 s configuration. `:344-366` checks half-open rolling 1 s and 5 s intervals beginning at each accepted flash; any violating interval has an accepted first flash, so these alignments suffice. `:333-339` uses repeated dim, non-red NaN-area requests, including both budgets and reduced-flashing mode. These assertions now depend on unknown area being counted. All pass. |
| R3-M02 - Godot independent zoom/FOV control | **Fixed** | `engines/godot/addons/game_accessibility/camera_comfort.gd:115-128,147-150` resolves zoom pulses through the FOV override and umbrella resolver. `:159-167` restores base FOV when either relevant setting disallows effects. `engines/godot/GUIDE.md:93-107` writes the current base FOV plus only an allowed kick every frame and describes explicit overrides correctly. | `repo:tests/godot/run_tests.gd:1070-1089` enumerates all six umbrella/override combinations and checks both the boolean gate and pulse output. `:1092-1120` asserts immediate restoration before the next simulated frame, continued restoration afterward, a changed base FOV, umbrella suppression with `follow`, and explicit `on` under the umbrella. The test invokes the actual settings callback; its frame callable reproduces the guide expression. Inspected statically; not rerun. |
| R3-M03 - Unity active-utterance handover | **Fixed** | `engines/unity/Runtime/Core/NarrationQueue.cs:192-218` saves the active entry, stops a busy backend before announcing anything, transfers a still-required active entry first, then transfers queued critical/dialogue entries in queue order while dropping ordinary menu/focus entries. `:122-127` performs the same handover when `Speak()` observes the new route before a tick. `:47-48` explicitly requires synchronous silencing by `Stop()`. | `repo:tests/unity_core/CoreChecks.cs:516-525,529-594` uses a genuinely busy fake backend, records native output in order, and checks backend speaking state at each native dispatch. Cases cover an active critical instruction, active dialogue, a new announcement arriving before the next tick, one-time transfer, menu dropping, and an already-finished utterance not being repeated. `:648-657` also verifies preservation of the original active critical `alert`. All pass. This proves dispatch ordering at the interface boundary; no real native reader was exercised. |
| R3-m01 - Godot weapon-sway example type mismatch | **Fixed** | `engines/godot/GUIDE.md:91` multiplies the vector returned by `sway_offset()` by the scalar `comfort.scale_sway(1.0)` before adding it to the rest position. The scalar helper remains consistent at `engines/godot/addons/game_accessibility/camera_comfort.gd:69-72`. | Verified the vector/scalar expression statically. The markdown example is not compiled or executed by the added Godot tests. |
| R3-m02 - Godot pruned skip keys | **Fixed** | `engines/godot/addons/game_accessibility/assist_policy.gd:136-148` uses false for each missing-key fallback. `:156-171` refuses each unavailable route before reaching the shared completion signal. Registered keys retain their registry defaults through `a11y_settings.gd:281-284` in the same directory. | `repo:tests/godot/run_tests.gd:480-527` writes and loads genuinely pruned registries for scene-only, challenge-only and neither-key cases, verifies key presence, checks both offer/request results, and compares the exact completion list and recorded skip state. Thus it detects unavailable routes emitting events. Inspected statically; not rerun. |
| R3-m03 - Unreal cross-category interruption | **Fixed** | `engines/unreal/Source/GameAccessibility/Private/A11yNarration.cpp:155-169` now requires `CurrentCategory == Category` before stopping an ordinary active utterance. `:202-234` leaves a busy dialogue backend running and dispatches queued menu speech after it becomes idle. Current category is assigned at `:230-232`. `engines/unreal/Source/GameAccessibility/Public/A11yNarration.h:90-97` documents the repaired behavior. | Static trace: dialogue active -> interrupting menu request removes only stale queued menu speech -> current-category guard prevents stop -> busy tick waits -> idle tick dispatches menu speech. No new executable Unreal test; no Unreal compilation or editor run is claimed or requested. |
| R2-M01 - flash classification and budgets, remaining R3-M01 gap | **Fixed** | The unchanged repairs accepted in round 3 remain at Godot `engines/godot/addons/game_accessibility/flash_guard.gd:68-80,96-104,124-146` and Unreal `engines/unreal/Source/GameAccessibility/Private/A11yFlashGuardSubsystem.cpp:51-67,93-106`. Unity's classification, NaN-measurement handling and count clamps remain at `engines/unity/Runtime/Core/FlashGuardCore.cs:122-145,162-167`; its outstanding window bypass is closed at `:85-97,128-142`. | The new rolling-window and corrected NaN-area cases cited for R3-M01 pass. Original small-area/moderate-intensity, saturated-red and loosened-budget cases remain at `repo:tests/unity_core/CoreChecks.cs:305-318` and also pass. This closes the specific carryover; it does not replace rendered-footage analysis or change the previously disclosed Godot/Unreal threshold tunability. |
| R2-M05 - independent scene/challenge skip gates, remaining R3-m02 gap | **Fixed** | `engines/godot/addons/game_accessibility/assist_policy.gd:136-171` now gives each route its own key, false when absent, before the common completion path. The previously accepted Unity/Unreal separation is unchanged by this commit. | Existing registered-key cross-combinations and deduplication remain at `repo:tests/godot/run_tests.gd:450-477`; new pruned-registry checks at `:480-527` cover the remaining gap. Unity's independent-key and scene-only checks remain at `repo:tests/unity_core/CoreChecks.cs:370-393` and pass. Godot coverage inspected statically. |
| R2-M09 - independent canonical controls, remaining R3-M02 gap | **Fixed** | Previously accepted registry separation remains at `assets/settings-registry.json:39-58,78-79,87-90`. The outstanding Godot consumer now uses the independent FOV resolver at `engines/godot/addons/game_accessibility/camera_comfort.gd:115-128,147-165`; `engines/godot/GUIDE.md:91-107` shows scalar sway scaling and a reversible per-frame FOV effect. | The six combinations and active-effect reversal at `repo:tests/godot/run_tests.gd:1070-1120` directly exercise the remaining consumer defect. Inspected statically. The registry was not changed by this commit. |

Result: **6/6 R3 findings fixed; 3/3 specified R2 carryovers fixed.**

## New findings

None. No release-blocking or clearly important regression was identified in
`git diff e4f0b56 47413a9`. No R4-M## or R4-m## findings are raised.

## Checks and coverage limits

- `python tests/check_skill.py`: **exit 0**, **0 failures, 0 warnings**. Checked 145
  SKILL.md lines, 207 links, 90 settings, 7 presets, 319 catalogue IDs and 1,112 tier labels.
- `bash tests/unity_core/run_unity_core_tests.sh`: **exit 0**, **210 passed, 0 failed**.
  Core compiled against netstandard2.1 with C# 8 and warnings as errors; checks ran on
  Microsoft.NETCore.App 10.0.4. The first invocation lacked Git Bash utility paths and
  failed before compilation; the successful invocation explicitly added `/usr/bin:/bin`
  to Bash's PATH. It used the existing cached reference assembly and installed compiler;
  no download or installation was performed. The permitted runner regenerated its ignored
  `.build` outputs.
- The new flash fixtures cover the configurations/rates listed in the table, not a full
  Cartesian product. The longer-window fixture checks the combined result and effective
  long duration; it does not independently isolate the longer short window or exercise
  a short window longer than the configured long window. Those branches were verified
  from the effective-window expressions. These are coverage limits, not observed defects.
- The Godot runner discovers methods prefixed `test_`
  (`repo:tests/godot/run_tests.gd:31-40`), so the three new test functions are included by
  its discovery mechanism. The claimed **43/43 on Redot 26.3 was not rerun**, as instructed.
- No Redot, Unity or Unreal process was launched. No Unity Editor, device, actual TTS,
  native screen-reader or Unreal compilation validation was performed. The Unity and
  Unreal verification labels remain explicit. No installation, commit or push was made.
- The only repository content added by this review is `evals/REVIEW_SOL_4.md`.

## Release verdict

**ready** for public release within this focused review's scope, with the existing
sample verification labels retained. The six R3 defects and three linked R2 carryovers
are closed; no new important regression was found. This verdict does not certify the
unrun engine integrations or real assistive-technology output.
