# Third independent review — SOL

Reviewed 2026-10-17 against `e4f0b56714d333ccb26f70008ac869e6f5cfe025` on `main`.
Paths are relative to `skills/game-accessibility/`, except paths prefixed `repo:`.

This is a static review of the samples and their integration instructions, plus the two
permitted test suites. No Redot, Unity or Unreal editor was launched, nothing was installed,
and no commit or push was made. The review adds only this report to repository content.
“Fixed” below describes the specific round-two defect; it does not certify an engine integration.

## R2 verification

| Finding | Status | Evidence at this HEAD |
| --- | --- | --- |
| R2-M01 — flash classification and budgets | **Partially fixed** | Godot `engines/godot/addons/game_accessibility/flash_guard.gd:68–80,96–104,124–146` implements the OR classification, 3/9 count clamps, saturated-red flag and linear-sRGB red helper. Unreal `engines/unreal/Source/GameAccessibility/Private/A11yFlashGuardSubsystem.cpp:51–67,93–106` implements the classification and count clamps. Unity `engines/unity/Runtime/Core/FlashGuardCore.cs:101–123,140–145` additionally clamps classification thresholds and maps NaN measurements to 1. Default candidate cases are repaired. However Unity's configurable window lengths still defeat the promised fixed-time hard caps: **R3-M01**. Godot/Unreal classification thresholds remain deliberately tunable, as their guides disclose; their clamps apply to counts, not classification thresholds. |
| R2-M02 — Godot subtitle clipping | **Fixed, statically** | `engines/godot/addons/game_accessibility/caption_view.gd:228–258` paginates all text; `:263–303` reflows and advances pages; `:319–324` implements `can_wait`; `:327–348` renders without a maximum-line clipping limit. `repo:tests/godot/run_tests.gd:710–742,745–783` checks ordered page coverage, word preservation, timing, manual advance and resize. These are English fixtures, not the requested translated-line/rendered-layout evidence; I did not rerun them. |
| R2-M03 — less-filtered content fallback | **Fixed** | `engines/godot/addons/game_accessibility/content_variant.gd:84–107` searches only from the requested index upward, then hides variants or selects `neutral_fallback`. `:111–129` exposes coverage gaps and warns. Missing-highest and intermediate cases are present at `repo:tests/godot/run_tests.gd:836–848`; additional coverage tests start at `:872`. |
| R2-M04 — Unity component filename | **Fixed, statically** | `engines/unity/Runtime/Unity/SettingsPanelAccessibility.cs:7–25` contains the matching public MonoBehaviour in its own file; `Narrator.cs:36` now owns only the Narrator component. The attach/save/reload behavior remains unverified, consistently with `engines/unity/GUIDE.md:3–12`. |
| R2-M05 — separate scene/challenge skip gates | **Partially fixed** | Separate APIs and a common deduplicated path exist: Godot `assist_policy.gd:136–170` and `a11y_settings.gd:157–165` under `engines/godot/addons/game_accessibility/`; Unity `engines/unity/Runtime/Core/AssistPolicy.cs:135–170`; Unreal `engines/unreal/Source/GameAccessibility/Private/A11yAssistPolicy.cpp:104–133` and `A11ySettingsSubsystem.cpp:609–618`. `references/architecture.md:106–116` and `references/topics/content-and-phobias.md:135–141` agree. Godot's new scene gate nevertheless enables an absent/pruned key, unlike Unity/Unreal: **R3-m02**. Unity's scene-only registry test is meaningful (`repo:tests/unity_core/CoreChecks.cs:343–348`); Godot tests all four registered-key combinations but no scene-only registry (`repo:tests/godot/run_tests.gd:450–477`). |
| R2-M06 — hidden/disabled Unity reader actions | **Fixed for the reported control-state defect** | `engines/unity/Runtime/Unity/SettingsPanelAccessibility.cs:91–132` guards actions with node activity and Selectable state. `:149–192` updates node visibility, disabled state and value, including transparent CanvasGroups and disabled Canvases. These API names match Unity's [AccessibilityNode reference](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/accessibility/accessibilitynode) and [Disabled documentation](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/accessibility/accessibilitystate/disabled). Real-reader and modal ownership behavior remain unverified; the Core suite does not compile or exercise this component. |
| R2-M07 — Steam incoming chat TTS | **Fixed** | `references/topics/store-claims-and-compliance.md:66` requires R04, incoming messages and canned responses, and explicitly rejects outgoing-only K04 as evidence. `:245–246` adds the corresponding verification check. |
| R2-M08 — router tiers and seated-mode ID | **Fixed** | `references/signal-routing.md:59,97,254,256` corrects S005/S033/S147/S149; `references/genre-recipes.md:221` uses M43. `repo:tests/check_skill.py:175–206,207–228` checks router Advanced/non-Advanced consistency and exact topic-table tiers. It intentionally does not distinguish router B from C and does not check the meaning of G26's ID; those current rows were also checked directly. |
| R2-M09 — independent canonical controls | **Partially fixed** | The canonical registry now has 90 keys; outlines, bob/sway, per-effect motion, scares/warnings and aim parts are separated at `assets/settings-registry.json:39–58,78–79,87–90`. All engine registry copies match. Resolvers exist at Godot `a11y_settings.gd:302–312`, Unity `A11ySettingsStore.cs:161–177`, Unreal `A11ySettingsSubsystem.cpp:375–398` in their respective sample directories. But Godot's zoom consumer and new FOV hookup do not honor the independent FOV setting consistently: **R3-M02**. |
| R2-m01 — timing-assist formula | **Fixed** | `references/topics/combat-aiming-stealth.md:69–72` now gives `real = base × window_scale / time_scale` and applies the multiplier once in simulation time, agreeing with `references/architecture.md:101–105`. |
| R2-m02 — Godot live caption toggles/direction | **Fixed** | `engines/godot/addons/game_accessibility/caption_view.gd:205–222` formats direction independently of names; `:374–397` removes disabled kinds immediately and reflows. `repo:tests/godot/run_tests.gd:786–810` covers visible entries and name/direction combinations. |
| R2-m03 — Unity shake Off during playback | **Fixed** | `engines/unity/Runtime/Core/CameraShakeCore.cs:19–22,32–50` clears trauma on Off and applies current scale at output. `engines/unity/Runtime/Unity/CameraComfort.cs:105–118,138–158` discards shake on disable, resets position and rotation, and uses the current amplitude every frame. Core checks cover the trauma behavior; the actual Transform adapter remains statically reviewed. |
| R2-m04 — stop pending speech by channel and recheck routes | **Fixed for the original toggle/queued-dispatch cases** | Godot `narrator.gd:150–188,399–411`; Unity `NarrationQueue.cs:75–79,169–175,200–208`; Unreal `A11yNarration.cpp:65–119,219–231`, in their respective sample directories, implement category settings, cancellation and dispatch checks. The Unity busy-backend cases pass (`repo:tests/unity_core/CoreChecks.cs:495–537`). The newly introduced handover has an active-utterance defect (**R3-M03**), and Unreal's interrupt path still cuts off the other category (**R3-m03**). |
| R2-m05 — Godot value-change route suppression | **Fixed** | `engines/godot/addons/game_accessibility/narrator.gd:114–118,335–351` sends range, option and toggle changes through the same reader-ownership gate as focus. `repo:tests/godot/run_tests.gd:1179–1201` checks slider/toggle changes with a fake reader probe, not actual OS output. |
| R2-m06 — Unreal global DOF restore ownership | **Fixed, statically** | `engines/unreal/Source/GameAccessibility/Private/A11yDepthOfFieldSubsystem.cpp:38–59,75–109` owns balanced requests and a preserved baseline; `A11yCameraComfortComponent.cpp:42–55,277–293` releases on EndPlay and delegates the fallback. This addresses the old component-local restore value; no Unreal execution is claimed. |
| R2-m07 — MIT notice in the installed skill | **Fixed** | `LICENSE:1–21` is inside the installable folder and matches the repository notice (checker passes). Engine-copy instructions preserve it at `engines/godot/GUIDE.md:33`, `engines/unity/GUIDE.md:42`, `engines/unreal/GUIDE.md:22`. |
| R2-m08 — refund eligibility wording | **Fixed** | `references/core-policy.md:61–65` now requires both 14 days from purchase and under 2 hours played for the standard window, allows review outside it and disclaims guaranteed approval. |
| R2-m09 — unvoiced Unity dialogue null clip | **Fixed** | `engines/unity/GUIDE.md:98–106` selects `authoredReadDuration` before emitting the event and only plays a non-null clip. |

Result: **15 fixed, 3 partially fixed, 0 wholly unfixed**. Remaining issues are described
below without repeating the original round-two findings as new findings.

## New important findings

### R3-M01 — Unity's “hard” flash caps can be bypassed by shortening its windows

- **Location:** `engines/unity/Runtime/Core/FlashGuardCore.cs:53–59,107–123`;
  `repo:tests/unity_core/CoreChecks.cs:301–322`.
- **Problem:** The fix clamps the two counts but still uses freely writable `WindowSeconds`
  and `LongWindowSeconds`. With `WindowSeconds = 0.1` and the default long window, requests
  at 0, 0.04, 0.08, 0.12 and 0.16 seconds are all allowed: five counted flashes in 0.16 s.
  Conversely, setting only `LongWindowSeconds = 1` prunes history after one second and
  permits a sustained sequence exceeding nine flashes in five seconds. This contradicts
  the fixed-time invariant at `references/architecture.md:211–214` and the new “whatever
  configuration” claim. The tests loosen thresholds/counts, but leave both durations at
  their defaults. Separately, the NaN-area assertion at `CoreChecks.cs:322` uses intensity
  1 and a fresh timestamp, so it would pass even without the NaN-area handling.
- **Concrete fix:** Always enforce independent immutable 1 s / 5 s safety windows; retain
  optional configurable limits only as additional restrictions, or clamp their effective
  durations to at least 1 s / 5 s. Retain history for the effective long window. Add
  shortened-window tests that check every rolling 1 s and 5 s interval. For the NaN-area
  regression, use repeated dim, non-red requests with unknown area and assert suppression.

### R3-M02 — Godot's zoom helper and FOV hookup bypass the new independent motion option

- **Location:** `engines/godot/addons/game_accessibility/camera_comfort.gd:120–122,141–143`;
  `engines/godot/GUIDE.md:93–102`; compare
  `references/topics/camera-and-motion.md:79–87`.
- **Problem:** The topic explicitly assigns zoom pulses to `motion.fov_effects`, but
  `scale_zoom_pulse()` still reads only `motion.reduce_motion`. Thus an explicit FOV-effects
  Off does not stop those pulses when the umbrella is off; an explicit On cannot keep
  them when the umbrella is on. The new guide repeats that contradictory umbrella-only
  behavior. Its dynamic-FOV example also only writes the FOV while effects are allowed:
  turning Off during a sprint leaves the camera at the last enlarged FOV rather than
  restoring the current base FOV. The settings-change callback in `camera_comfort.gd:152–158`
  does not restore it on `motion.fov_effects` changes either.
- **Concrete fix:** Resolve zoom pulses through `allow_fov_effects()`. Update the example to
  write `current_base_fov + (allowed ? authored_offset : 0)` every frame, using the player's
  current base FOV, and remove the umbrella-only instruction. Add both umbrella/override
  cross-combinations for `scale_zoom_pulse`, plus an On → Off check during an active FOV effect.

### R3-M03 — Unity's native-reader handover loses active critical speech and overlaps dialogue

- **Location:** `engines/unity/Runtime/Core/NarrationQueue.cs:156–165,179–197`;
  `repo:tests/unity_core/CoreChecks.cs:519–537`.
- **Problem:** Handover treats the active utterance differently solely by channel. If a
  critical menu announcement is playing, enabling the native route calls `Backend.Stop()`
  and forgets it; only queued entries are forwarded. A long critical instruction can be
  cut off before its meaningful portion is heard, despite the “critical announcements
  are never dropped” contract. If dialogue is currently playing instead, handover leaves
  the backend speaking and immediately forwards queued dialogue/critical items to the
  native reader, causing simultaneous voices. The current test actually stops an active
  critical `alert`, but never asserts that it survives handover; it also never switches
  routes during active dialogue.
- **Concrete fix:** Define one transition policy for active and queued speech: preserve
  critical information and serialize output across backends. For example, stop active
  self-voicing and transfer its still-required critical/dialogue entry before queued
  entries, or finish it before dispatching native announcements. Do not send new native
  announcements over active fallback dialogue. Add busy-backend tests for active critical
  and active dialogue transitions, checking preserved content and no overlapping dispatch.

## Other important defects

### R3-m01 — New Godot weapon-sway example cannot type-check

- **Location:** `engines/godot/GUIDE.md:91`;
  `engines/godot/addons/game_accessibility/camera_comfort.gd:63–66`.
- **Problem:** The example assigns `weapon.position = rest + comfort.scale_sway(...)`,
  but `scale_sway` accepts and returns a float. A position/rest is a Vector2 or Vector3.
  Passing a vector offset is an invalid argument; passing a scalar instead leaves an
  invalid vector-plus-float expression. The new integration example cannot be used as shown.
- **Concrete fix:** Show vector scaling explicitly, for example
  `weapon.position = rest + sway_offset(walk_time) * comfort.scale_sway(1.0)`.
  Keep the small scalar helper; no new abstraction is needed.

### R3-m02 — Godot's new scene-skip gate enables a key absent from the registry

- **Location:** `engines/godot/addons/game_accessibility/assist_policy.gd:136–146,166–170`;
  `engines/godot/addons/game_accessibility/a11y_settings.gd:281–284`;
  `repo:tests/godot/run_tests.gd:450–477`.
- **Problem:** `scene_skip_offered()` passes `true` as the missing-key fallback. A project
  that registers only challenge skipping therefore also accepts `request_scene_skip()`
  and emits the completion event, although it never registered sensitive-scene skipping.
  The converse scene-only registry also incorrectly offers challenge skips via the
  existing `skip_offered()` fallback. Unity and Unreal return false for absent keys.
  This defeats the stated pruned-registry/no-change contract and creates different
  behavior behind the newly shared API. The Godot regression tests use the full registry.
- **Concrete fix:** Return false when either gate's own key is unregistered; the registry's
  true default still applies when the key exists. Add scene-only, challenge-only and
  neither-key fixtures, asserting that an unavailable route emits no completion event.

### R3-m03 — Unreal menu interruption discards an in-progress dialogue line

- **Location:** `engines/unreal/Source/GameAccessibility/Private/A11yNarration.cpp:155–167`;
  `engines/unreal/Source/GameAccessibility/Public/A11yNarration.h:89–91`.
- **Problem:** The category-aware interrupt path filters queued items by `Category`, but
  its current-utterance branch never checks `CurrentCategory == Category`. While a normal
  unvoiced dialogue line is playing, a default `Speak()` call for menu focus stops that
  dialogue, which has already left the queue and is never resumed. The new category
  bookkeeping is therefore applied to queued speech but not active speech. Merely
  focusing a control can lose the remainder of an independently enabled read-aloud line.
- **Concrete fix:** Restrict ordinary interruption to the same category, as Godot's
  `_interrupt()` does (`engines/godot/addons/game_accessibility/narrator.gd:373–382`),
  or explicitly preserve/requeue cross-category speech. Verify the trace with a busy
  backend: dialogue playing → menu focus submitted → dialogue finishes → menu speaks.
  This is a static implementation repair, not a request to install an editor.

## Checks and limits

- `python tests/check_skill.py`: **exit 0**, 0 failures, 0 warnings; 145 SKILL.md lines,
  207 relative links, 90 settings, 7 presets, 319 catalogue IDs and 1,112 tier labels checked.
  This checks local file links, not live external-link availability or rendered layouts.
- `bash tests/unity_core/run_unity_core_tests.sh`: **exit 0**, **186 passed, 0 failed**.
  Core compiled against netstandard2.1 with C# 8/warnings as errors; checks ran on
  Microsoft.NETCore.App 10.0.4. Git Bash's utility directories needed to be added explicitly
  to its PATH after the initial invocation could not find `dirname`, `tr` and `head`.
  The successful run used the existing cached reference assembly and installed compiler;
  it performed no download or installation. Its ignored build outputs are not release edits.
- Reviewed the round-two commit's sample changes, registry copies, routing/tier changes,
  policy/topic changes, engine integration examples and relevant tests. Verified the new
  Unity accessibility-node APIs against the primary documentation linked in the table.
- Godot's claimed **40/40** result was **not rerun**, as instructed. Its new test source was
  inspected. Unity adapters were not compiled against real Unity assemblies, and Unreal
  was not compiled. Those known validation limits are honestly labelled in the guides
  and are not, by themselves, additional release findings.
- No real TTS/screen-reader output, footage analysis or rendered caption layout was tested.
  New findings above are concrete static traces, not editor reproductions. No production
  fix or test file was changed by this review.

## Release verdict

**Not ready.** Most round-two defects are repaired, but the remaining flash-cap escape,
independent FOV behavior and speech-handover defects contradict core sample contracts.
Resolve **R3-M01–R3-M03** and the three listed integration defects **R3-m01–R3-m03**,
then rerun the permitted checks and the focused regressions in the owner's available
environment. Keep the existing Unity/Unreal validation labels; no editor installation is
required by this review.
