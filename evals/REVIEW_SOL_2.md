# Second independent review — SOL

Reviewed 2026-10-17 against `8112bff90052cff839a7e22cf1d3b2f6bf318524` on `main`.
Paths below are relative to `skills/game-accessibility/` unless prefixed `repo:`.
This report is the only file created or changed by this review. No engine was launched,
no software installed, and no commit or push performed.

## Round-1 verification

“Fixed” means the original defect is addressed in this snapshot, not that all related
gameplay has been exercised. Engine findings below are static code traces, not claims
of having reproduced them inside an editor.

| Round 1 | Status | Current evidence |
| --- | --- | --- |
| M01 — batch contrast rows skipped | Fixed | `scripts/contrast_check.py:77–90` recognises `//` comments and rejects an empty batch. In-memory mixed white/white and white/black rows produce FAIL/PASS and exit 1; an empty batch exits 2. |
| M02 — unresolved background alpha passes | Fixed | `scripts/contrast_check.py:46–55` rejects a translucent background without an underlay and rejects a translucent underlay. Both cases raise `ValueError`; opaque black underlay produces 1:1. |
| M03 — sustained flash failure omitted | Fixed | `references/architecture.md:176–184`, `references/topics/photosensitivity.md:39–59,183–191` and `references/verification.md:49–52,80` require both budgets, sustained-sequence tests and captured-output analysis, or `blocked`. Sample threshold defects remain as R2-M01. |
| M04 — forced bundles | Partially fixed | `assets/settings-registry.json:40,51–57,67,70,77` separates dimming, post effects, guidance channels, gore, scene skips and target lock. However outline classes still share one key (`:39`; `references/topics/color-and-clarity.md:28–31` makes separation conditional), bob/sway share a key (`references/topics/camera-and-motion.md:19–20`), and scare reduction/warning remain bundled (`assets/settings-registry.json:68`). See R2-M09. |
| M05 — feature/audit bypass core policy | Fixed | `SKILL.md:28,33–35,42–45` requires steps 1–2 for every invocation, before selecting a mode, and audit returns to steps 4–8. |
| M06 — planned features classified away | Fixed | `SKILL.md:33`, `references/signal-routing.md:17,21–22,366` explicitly include planned/in-scope features in Build planning. Template `assets/templates/ACCESSIBILITY.md:23–25` preserves their separate current-build status. |
| M07 — missing JRPG encounter/auto-battle obligations | Fixed | `references/signal-routing.md:264` adds S157 with D31–D33 and D23; `references/genre-recipes.md:49–55` makes the work explicit; `references/topics/difficulty-and-assists.md:114–131` specifies takeover, progression and reward parity. Registry keys are at `assets/settings-registry.json:89–91`. |
| M08 — missing engine guides | Fixed | All four targets at `SKILL.md:111–114` exist. Each guide states validation limits near its beginning. The relative-link checker passes. |
| M09 — HUD/custom palette values not persisted | Fixed | `assets/settings-registry.json:38,60–61` adds role colours, HUD Off and hidden element IDs. Migration/fallback and persistence expectations are in `references/topics/color-and-clarity.md:55–60` and `references/topics/sensory-load-and-guidance.md:72–81,148`. |
| M10 — time scaling contradicts independent windows | Partially fixed | `references/architecture.md:78–82`, `references/topics/timing-and-speed.md:55–59,78–80`, `references/topics/traversal-and-platforming.md:35–39` and `references/verification.md:94–95` now give the correct combination formula. But `references/topics/combat-aiming-stealth.md:66–67` still says not to stack with game speed. See R2-m01. |
| M11 — router tier/ID drift | Partially fixed | S090/C38, S094/R02, S117/F15 and XR rows are repaired at `references/signal-routing.md:169,173,209,238–242`; `references/topics/vr-and-xr.md:14–18` uses M43–M46. R02 is still Advanced at S147/S149 (`signal-routing.md:254,256`), R17 is Advanced at S033 (`:97`), and G26 still calls seated mode M26 (`references/genre-recipes.md:221`). See R2-M08. |
| m01 — first-launch silence blocks audible activation | Fixed | `references/core-policy.md:79` and `references/architecture.md:212–215` explicitly permit a quiet, repeatable activation prompt at saved/default volume. `references/topics/audio-and-captions.md:74–79` and `screen-reader-and-voice.md:93–100` agree. |
| m02 — installed script path assumption | Fixed | `SKILL.md:37–38` explicitly resolves all paths from the installed skill folder and gives the absolute/expanded invocation form. Later shorthand commands remain understandable under that rule. |
| m03 — genre guidance loaded too broadly | Fixed | `SKILL.md:51–52`, `references/genre-recipes.md:3` and `references/topics/structured-ui-and-strategy.md:15` require reading only matching sections. This addresses the requested section-routing alternative without splitting the files. |
| m04 — local microphone mechanic routes to multiplayer chat | Fixed | `references/signal-routing.md:63` and `references/topics/screen-reader-and-voice.md:130–132` require a local logical-action alternative and reserve K01 for communication signals. |
| m05 — haptic-only essential-audio substitute | Fixed | `references/verification.md:46–48` now tests muted play with vibration off and requires actionable visual/text cues. `references/topics/audio-and-captions.md:144–148` agrees. |
| m06 — original/partial phobia levels tested as full removal | Fixed | `references/verification.md:83` uses level 0 as control and verifies the substitutions each higher level actually promises. |
| m07 — paragraph-spacing units | Fixed | `references/topics/text-and-readability.md:171–172` separates the XAG suggestion from WCAG's paragraph spacing of 2 × font size. |
| m08 — scanner skips gameplay filename substrings | Fixed | `scripts/scan_signals.py:40–45,212–217` now uses exact documentation basename patterns. In-memory scans of both `noticeboard.gd` and `third_party_enemy.gd` scan one file and find S109. |
| m09 — scanner guides/false leads | Fixed | `scripts/scan_signals.py:50–61,125–132,246–261` reads guides from the router, bounds quest matching, distinguishes uppercase `.INK` members, and lists unsupported signals separately. Checked: S014 returns both guides, S035 returns combat guidance; `UiTheme.INK`/`question` do not match, while `foo.ink`/`quest` do. |
| m10 — template unrestricted waiver | Fixed | `assets/templates/ACCESSIBILITY.md:13–19,48–50` now calls the policy binding and records conflicts for resolution. The owner's specifically requested insistence process is separately and explicitly scoped in `references/core-policy.md:51–72`. |
| n01 — stale registry count | Fixed | `repo:README.md:93` says 77; the registry and checker both contain 77 settings. |

Result: **19 fixed, 3 partially fixed, 0 wholly unfixed**.

## New findings

### R2-M01 — FlashGuard samples can bypass both flash budgets for hazardous candidates

- **Location:** `engines/godot/addons/game_accessibility/flash_guard.gd:23–32,59–73,82–85`;
  `engines/unreal/Source/GameAccessibility/Public/A11yFlashGuardSubsystem.h:63–79` and
  `Private/A11yFlashGuardSubsystem.cpp:62–74`; compare `engines/unity/Runtime/Core/FlashGuardCore.cs:65–69,95–113`.
- **Problem:** Godot bypasses all counting below 10% screen area and 0.8 luminance change;
  Unreal does so below 25% screen area and 0.5 change when `bSaturatedRed=false`.
  Thus repeated requests with intensity 0.4 and area 0.09 are unlimited in both samples.
  These are not conservative exclusions: WCAG's general-flash change is 0.1, and its area
  threshold is a fraction of a 10° field, not a quarter of the whole screen. The Unreal
  “heuristic” caveat and mandatory footage analysis do not make these default bypasses a
  reliable implementation of the prevention contract. Godot also exposes unrestricted
  budget exports and has no saturated-red request flag to enforce the guide's rule that
  red requests always count. Its red helper labels encoded sRGB channel ratios as the
  WCAG definition, although that definition uses linearised channels.
- **Concrete fix:** Conservatively count every potentially flashing request when actual
  rendered area/luminance is unknown; otherwise use a validated classification that does
  not exclude these cases. Clamp Godot's hard budgets. Pass red classification into the
  counting decision and compute it in the correct colour space. Add small-area/moderate-
  luminance and small-red repeated-request cases, retaining footage analysis as a separate gate.
  [W3C threshold and relative-luminance definitions](https://www.w3.org/WAI/WCAG22/Understanding/three-flashes-or-below-threshold.html).

### R2-M02 — Godot subtitles silently discard text after two rendered lines

- **Location:** `engines/godot/addons/game_accessibility/caption_view.gd:144–161,179–183`.
- **Problem:** The complete event text is assigned to a Label with `max_lines_visible = 2`.
  There is no page state, scrolling or advance path. A long line, translation, or enlarged
  subtitle that wraps to three or more lines loses the remainder for the event's entire
  lifetime. The two-line presentation limit is being enforced by clipping information.
- **Concrete fix:** Paginate the complete text into readable cues, reflow pages on size or
  viewport changes, and preserve every word with playback/manual-advance semantics.
  Verify a long translated line at 200% subtitle size and the minimum supported viewport;
  assert that all text is eventually presented, not merely that at most two lines are visible.

### R2-M03 — Godot missing phobia variants can reveal the original

- **Location:** `engines/godot/addons/game_accessibility/content_variant.gd:70–91`;
  `references/topics/content-and-phobias.md:74–77`.
- **Problem:** When no variant at or above the requested level exists, the fallback loop
  searches downward. With only child `0` authored and `content.spiders=3`, it selects and
  displays child `0`. With `3` missing but `2` present, it silently shows a less filtered
  version than the description promises. This contradicts both the documented “nearest
  more filtered” fallback and the instruction to keep unavailable content hidden.
- **Concrete fix:** Never fall back below the requested filtration level. Validate variant
  coverage and keep the presentation hidden or use an explicitly authored neutral fallback
  until a sufficient variant exists. Report missing coverage. Test missing highest and
  intermediate variants as well as complete variant sets.

### R2-M04 — Unity's native settings accessibility component cannot be attached as packaged

- **Location:** `engines/unity/Runtime/Unity/Narrator.cs:60,191–210`;
  `engines/unity/GUIDE.md:202–205`.
- **Problem:** `Narrator.cs` defines both `Narrator : MonoBehaviour` and
  `SettingsPanelAccessibility : MonoBehaviour`. Unity selects the component matching the
  filename when multiple classes are in a script. There is no
  `SettingsPanelAccessibility.cs` or runtime construction hook, so the supplied native
  accessibility panel cannot be configured as its own component in the normal Inspector
  workflow. Compiling against API stubs cannot detect this import/serialization problem.
- **Concrete fix:** Move `SettingsPanelAccessibility` into its own matching file with the
  same version guards/imports. Verify it can be added to a panel, its `items` can be
  assigned, and the component survives scene save/reload in Unity 6.3.
  [Unity 6.3 script naming rules](https://docs.unity.com/en-us/engine/6000.3/manual/scripting/get-started/naming-scripts).

### R2-M05 — Sensitive-scene skip is gated by the unrelated challenge-skip option

- **Location:** `references/topics/content-and-phobias.md:125–134`;
  `engines/godot/addons/game_accessibility/assist_policy.gd:136–155`;
  `engines/unity/Runtime/Core/AssistPolicy.cs:129–142`;
  `engines/unreal/Source/GameAccessibility/Private/A11yAssistPolicy.cpp:104–117`.
- **Problem:** The guide says `content.scene_skip` is independent, then instructs callers
  to use `request_skip(scene_id)`. All three sample request functions instead gate on
  `assist.skip_available`. A player can enable sensitive-scene skipping but disable puzzle
  skipping and have the advertised scene skip rejected. A story-only game that prunes
  the general challenge key also cannot use this route in Unity/Unreal.
- **Concrete fix:** Distinguish sensitive-scene requests from challenge requests, validating
  each against its own key before reaching the shared completion event/path. Test all
  four combinations and a registry containing only the sensitive-scene key.

### R2-M06 — Unity accessibility actions expose hidden/disabled controls and bypass their state

- **Location:** `engines/unity/Runtime/Unity/Narrator.cs:241–277,285–293`.
- **Problem:** The panel builds nodes for every non-null configured control, regardless of
  active/visible/interactable state. Its invocation callbacks directly toggle values or
  call `button.onClick.Invoke()`, without checking the control's state. A disabled button
  can therefore execute through the screen-reader action, and hidden controls remain in
  the reading/action tree. Only values are synchronised afterward. This violates the
  guide's “same command handler” and hidden-menu guarantees and can affect destructive actions.
- **Concrete fix:** Track node visibility and disabled state as the UI changes, omit or
  deactivate hidden nodes, and guard actions with the same active/interactable checks as
  ordinary UI input. Preserve correct modal ownership. Test a hidden panel item and a
  disabled destructive button with the real reader.
  [Unity AccessibilityNode.isActive](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Accessibility.AccessibilityNode-isActive.html).

### R2-M07 — Steam Chat Text-to-speech claim maps to the wrong direction

- **Location:** `references/topics/store-claims-and-compliance.md:66`;
  compare `references/topics/multiplayer-and-communication.md:19–20,61–64`.
- **Problem:** The claim matrix accepts K04 (outgoing typed-message speech), making R04
  incoming narration something to “check whether” the field covers. Valve explicitly
  describes this tag as having text chat narrated to the player, including canned
  responses. An outgoing-only implementation can pass the supplied matrix while a blind
  player still cannot read incoming chat.
- **Concrete fix:** Require R04 for the Steam tag and verify incoming messages/canned
  responses and their speech volume. Keep K04 as a distinct outgoing communication
  capability; it is not sufficient evidence for this Steam field.
  [Valve's Chat Text-to-speech definition](https://partner.steamgames.com/doc/accessibility_features).

### R2-M08 — Router still permits silent scope cuts to Contextual narration

- **Location:** `references/signal-routing.md:97,254,256`;
  `references/option-catalog.md:451,466`;
  `references/topics/structured-ui-and-strategy.md:39,127–132`;
  `references/genre-recipes.md:221`.
- **Problem:** S147 and S149 label on-demand state/coordinate reading R02 Advanced,
  although the catalogue/topic make it Contextual. S033 similarly labels R17 Advanced
  instead of Contextual. Under `SKILL.md:53–54`, an agent can defer these selected
  capabilities as scope decisions. The XR recipe also still uses M26 for seated mode,
  although M43 owns it. These are residual round-1 M11 defects, not merely unknown IDs.
- **Concrete fix:** Use catalogue tiers consistently, reserve Advanced for the separately
  defined full nonvisual gameplay work, and change G26 seated mode to M43. Add semantic
  tier/meaning checks for these rows; the existing checker only checks ID existence/range.

### R2-M09 — Remaining canonical controls still contradict the independence guarantee

- **Location:** `assets/settings-registry.json:39,45,48,68`;
  `references/topics/color-and-clarity.md:28–31`;
  `references/topics/camera-and-motion.md:19–20,26–34,67–70`;
  `references/topics/combat-aiming-stealth.md:43–47`;
  `references/core-policy.md:17–18,108–114`.
- **Problem:** The main round-1 bundles were split, but enemy/ally/interactable/edge outlines
  still share one boolean; bob and weapon sway share one scalar; `motion.reduce_motion`
  remains the only key for multiple unrelated automatic/decorative motions; and jump
  scares offer `full`, `reduced`, `warned`, with `warned` also applying reduction. A player
  cannot keep authored scares and request advance warning through that selector. Aim
  friction/pull/stabilisation are likewise tied to thresholds on one slider unless a
  project optionally adds keys. Saying “if players need” separate keys leaves the same
  escape hatch identified in M04, against the unconditional independence contract.
- **Concrete fix:** Give these distinct choices persistent per-effect/per-class keys or
  supported overrides, and make umbrella controls editable patches over them. In
  particular separate warning from scare intensity. Require cross-combination checks
  rather than leaving separation to an optional later request. Synchronise all registry copies.

### R2-m01 — Combat guide retains the old prohibition on combining time assists

- **Location:** `references/topics/combat-aiming-stealth.md:66–67` versus
  `references/architecture.md:78–82` and `references/topics/timing-and-speed.md:78–80`.
- **Problem:** “Do not stack with game speed” still tells the combat implementer to reject
  or cancel the player's intentional combination. The linked guide now explicitly allows it.
- **Concrete fix:** Replace this with “Apply the window multiplier once in simulation time;
  deliberate game slowdown also lengthens real time,” including the shared formula.

### R2-m02 — Godot caption toggles do not apply independently to visible lines

- **Location:** `engines/godot/addons/game_accessibility/caption_view.gd:126–137,144–154,186–189`.
- **Problem:** Turning subtitles or sound captions off only restyles/renders stored entries;
  rendering never filters them against the new settings, so existing text stays until
  expiry. Also, dialogue direction is appended to the speaker string and returned only
  when speaker names are on. `subtitles.direction=true` with names off loses direction.
- **Concrete fix:** Filter/hide existing entries immediately on category changes, and format
  direction independently of speaker-name visibility. Test both combinations on active cues.

### R2-m03 — Unity shake Off does not remove an existing shake

- **Location:** `engines/unity/Runtime/Unity/CameraComfort.cs:81–85,95–124`.
- **Problem:** The scale is applied only when trauma is added. After trauma is accumulated,
  setting `motion.camera_shake=0` leaves the existing offsets running until decay ends;
  zero decay makes this indefinite. `OnDisable` resets position but leaves rotation.
- **Concrete fix:** Apply the current scale at final output or clear trauma/offsets when
  Off is selected; reset rotation on disable too. Test Off during an active shake and
  a disable/enable cycle, not just requests made while already Off.

### R2-m04 — Pending self-voicing continues after menu narration is disabled

- **Location:** `engines/godot/addons/game_accessibility/narrator.gd:95–106,314–328`;
  `engines/unity/Runtime/Unity/Narrator.cs:118–131,182–187`;
  `engines/unreal/Source/GameAccessibility/Private/A11yNarration.cpp:57–68,136–149`.
- **Problem:** The menu setting is checked when speech is submitted, but none of these
  dispatch loops rechecks that setting or subscribes to stop pending/current menu speech.
  Queue a long focus/menu utterance plus another, turn menu narration off, and the
  already-accepted speech continues. Unity also keeps dispatching queued TTS if the
  native route activates while the queue is pending.
- **Concrete fix:** Track speech categories/routes, stop and discard menu/focus speech when
  its setting is disabled, and re-evaluate route ownership before dispatch. Keep the
  independent dialogue-reading option independent. Test toggles and route changes with
  a busy fake backend, then verify actual output when an editor/device is available.

### R2-m05 — Godot's value-change narration ignores the native-reader suppression rule

- **Location:** `engines/godot/addons/game_accessibility/narrator.gd:133–138,262–276`.
- **Problem:** Focus changes check `prefer_screen_reader` and `screen_reader_active()`;
  slider, option and toggle value changes call `speak()` directly. With both routes
  available and narration enabled, native value announcements can be duplicated by TTS.
- **Concrete fix:** Use the same route-ownership check for value changes as for focus,
  and verify changing each control with an active OS reader produces one announcement.

### R2-m06 — Unreal's global depth-of-field fallback loses its restore value across scenes

- **Location:** `engines/unreal/Source/GameAccessibility/Private/A11yCameraComfortComponent.cpp:42–49,248–276`.
- **Problem:** The default fallback writes global `r.DepthOfFieldQuality=0`, but remembers
  the previous quality on the individual component. EndPlay never restores/transfers it.
  Start with quality 2, disable DOF, change scenes, then enable it: the new component
  captured 0 as its baseline and restores 0. The option cannot restore the authored effect.
- **Concrete fix:** Give the global override a persistent single owner with a preserved
  baseline, or implement balanced ownership/restoration across component lifetimes.
  Test Off → scene transition → On, and overlapping camera components.

### R2-m07 — The installable skill omits the MIT notice

- **Location:** `repo:LICENSE:3–13`, `repo:README.md:58–65`;
  `SKILL.md:4`; `engines/unity/Runtime/package.json:7`.
- **Problem:** The only full licence/copyright notice is outside the installed folder.
  Copying just the documented skill directory, or just an engine sample as instructed,
  omits that notice. Naming MIT in metadata or an SPDX tag is not the notice text the
  repository licence asks redistributors to preserve.
- **Concrete fix:** Include the licence/copyright notice inside the installable skill and
  ensure engine-copy instructions/artifacts retain it. No uncredited third-party code
  reuse was established by this review; do not infer that merely from reference links.

### R2-m08 — Refund-window explanation conflates early play with a recent purchase

- **Location:** `references/core-policy.md:61–64`.
- **Problem:** “In the settings menu or the first hours of play” is followed by “that is
  inside the refund window.” A first play can occur months after purchase, and “first
  hours” can exceed the under-two-hour condition. The linked policy gives both conditions;
  the explanatory inference is too categorical.
- **Concrete fix:** Keep the owner's pushback, but say a penalty *can* cause a refund and
  that ordinary Steam eligibility is within 14 days of purchase **and** under 2 hours
  played; requests outside those conditions can still be reviewed. Do not imply
  accessibility complaints guarantee approval.
  [Valve refund terms](https://store.steampowered.com/steam_refunds/).

### R2-m09 — Unity's dialogue hookup dereferences a missing voice clip

- **Location:** `engines/unity/GUIDE.md:87–88`.
- **Problem:** The example passes `clip.length` before testing `clip != null` for
  `isVoiced`. Using the shown hookup for an unvoiced line throws instead of emitting
  its subtitle/narration event, precisely the fallback the flag is intended to support.
- **Concrete fix:** Resolve duration conditionally (`clip != null ? clip.length : authoredReadDuration`)
  before constructing the event, and show the unvoiced path explicitly.

No R2-n findings: no style-only changes are requested.

## Checks and limits

- `python tests/check_skill.py`: exit 0; 0 failures, 0 warnings; 319 catalogue IDs,
  77 settings, 7 presets, 200 relative links checked.
- `python skills/game-accessibility/scripts/scan_signals.py skills/game-accessibility/engines`:
  exit 0; 64 text files scanned. This checks scanner operation on sample files, not
  accessibility of a real game.
- `python skills/game-accessibility/scripts/contrast_check.py --help`: exit 0.
- Additional Python checks used only in-memory/mocked fixtures for the original contrast
  and scanner regressions. One initial scanner fixture mock also intercepted the router
  read and failed; rerunning with the real router data isolated passed the intended cases.
- Reviewed the entry point, policy, catalogue/router/recipes, topic and engine guidance,
  registry copies, template, scripts and engine samples. Cross-checked selected threshold,
  Steam, Xbox, installation and Unity API/import claims against primary documentation.
  External links and third-party provenance were not exhaustively audited. No tracked
  personal filesystem paths or credentials were identified; ignored local build/cache
  files are not release contents.
- Unity/Unreal compilation, native reader/TTS output, rendered layouts, captured flashing,
  and existing Godot test-result claims were **not revalidated at runtime** in this round.
  Those limits do not negate the deterministic code paths described above.

## Release verdict

**Not ready.** Resolve R2-M01–R2-M09 before public release, then address the listed minor
defects and run focused regression checks. The instruction/routing repairs and Python
checks are substantial improvements, but the packaged samples still violate safety,
content-filter, caption, independent-option and screen-reader contracts, and one store
claim has the wrong acceptance criterion. Preserve the honest Unity/Unreal validation
labels until real editor/build testing is available.
