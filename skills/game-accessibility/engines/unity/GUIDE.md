# Unity guide

> **Verification status. Never run in the Unity Editor, in Play Mode or on a device.**
> - `Runtime/Core/*.cs` (plain C#) compiles with Roslyn `csc` (C# 8, warnings as errors) against the
>   official netstandard2.1 reference assembly. It passes 210 behaviour checks on the .NET runtime. To re-run
>   both from the repository root: `bash tests/unity_core/run_unity_core_tests.sh`.
> - `Runtime/Unity/*.cs` (adapters) compiled only against hand-written stubs of the Unity APIs listed in §1,
>   with and without the accessibility-module, 6000.0/6000.3, platform and TMP defines.
>   The API names are checked against Unity's documentation, not against a real Editor. Stubs cannot show
>   Inspector, serialisation or screen-reader behaviour.
> - The repository has no Unity Editor and no .NET SDK. Please report compile errors with your Unity version.

Implements [architecture.md](../../references/architecture.md) under the rules in
[core-policy.md](../../references/core-policy.md). Setting keys come from
[settings-registry.json](../../assets/settings-registry.json).

## 1. Version check (do this first)

Read `ProjectSettings/ProjectVersion.txt` (`m_EditorVersion`). Then check `Packages/manifest.json` for
`com.unity.inputsystem`, `com.unity.textmeshpro` (2022.3) or `com.unity.ugui` 2.x (TMP is inside uGUI 2.0+,
Unity 2023.2 and later), and `com.unity.modules.accessibility`.

| API (namespace `UnityEngine.Accessibility`) | Editor | Platforms (per Unity docs) |
| --- | --- | --- |
| `AccessibilitySettings.fontScale`, `isClosedCaptioningEnabled`, `isBoldTextEnabled`, `*Changed` events | 6000.0+ | Android, iOS only (also in 6000.3) |
| `AssistiveSupport.isScreenReaderEnabled`, `screenReaderStatusChanged` (`Action<bool>`), `notificationDispatcher.SendAnnouncement` | 6000.0+ | 6000.0: Android 8+, iOS. 6000.3: adds Windows and macOS players |
| `AccessibilityHierarchy`, `AccessibilityNode` (`invoked`, `incremented`, `frameGetter`, `role`, `isActive`, `state`), `AccessibilityState.Disabled`, `notificationDispatcher.SendLayoutChanged` | 6000.3+ in this sample | Android 8+, iOS, Windows, macOS; main window only |
| none of the above | 2022.3, 2023.x | Use self-voicing and your own OS-preference plugins |

6000.0 nodes expose `selected` where 6000.3 exposes `invoked`, so the hierarchy example is gated on
`UNITY_6000_3_OR_NEWER`. Reduce-motion, increase-contrast and mono-audio OS preferences have **no Unity API**;
pass them from a native plugin with `Store.ImportPlatformPreference("reduce_motion", true)`. Reduce motion
sets shake, head bob and weapon sway to 0, turns motion blur off, and turns `motion.reduce_motion` and
`motion.reduce_flashing` on. The per-effect overrides keep `follow`, so they follow `motion.reduce_motion`.
Explicit player choices are never overridden.

## 2. Install

1. Copy `engines/unity/Runtime/` to `Packages/com.dominicbytes.game-accessibility/` (an embedded package; Unity
   creates the `.meta` files). Copying into `Assets/GameAccessibility/` also works. **Do not install by git URL**:
   the repo ships no `.meta` files, and Unity ignores assets without them in immutable packages.
   Keep the skill's `LICENSE` file (MIT) with the copied code.
2. Two assemblies: `GameAccessibility.Core` (`noEngineReferences: true`, unit-testable outside Unity) and
   `GameAccessibility.Unity` (adapters). Your gameplay asmdef references both.
3. Defines come from the adapter asmdef's `versionDefines`: `GAME_A11Y_ACCESSIBILITY_MODULE`
   (`com.unity.modules.accessibility`) and `GAME_A11Y_TMP` (`com.unity.textmeshpro` 3+ or `com.unity.ugui` 2+).
   UNVERIFIED: that `versionDefines` resolves built-in modules, and that a missing `Unity.TextMeshPro`
   reference is ignored. If either fails, add the define under Player > Scripting Define Symbols, or remove the reference.
4. Trim the registry: list the keys your signals need in `A11yBootstrap.onlyKeys`. Services fall back to
   "no change" for keys you did not register.

## 3. Boot and persistence

Put `A11yBootstrap` in the first scene. It runs at execution order -1000, survives scene loads, and exposes
`Store`, `Policy`, `InputAssist`, `FlashGuard`, `Captions` and `Colors`. Adapters find it through an
inspector reference or `A11yBootstrap.Active`.

- File: `Application.persistentDataPath/accessibility.json`, outside story saves. Written to `.tmp` then
  replaced. Only explicit player choices are stored, in `profile` and `device` sections with `schema_version`.
  Any key in the file counts as the player's choice. Document this file in your README.
- Order at boot: load file, then import OS preferences. An OS value never overrides an explicit key.
- Renamed a key? Set `Store.Migration` before `Load()` (see the XML example in `A11ySettingsStore`).
- Local co-op: create one `A11ySettingsStore` per player for `profile` keys.

## 4. Wiring into typical systems

Gameplay asks a service. It never reads `Store` directly and never reads physical keys.

```csharp
var a11y = A11yBootstrap.Active;
// Damage and failure channels
health -= a11y.Policy.DamageToPlayer(hit.amount, "enemy");          // "fall" is not scaled
if (fellIntoPit) { if (a11y.Policy.ShouldFail("fall")) Die(); else RespawnAtLastSafePoint(); }
remaining -= Time.deltaTime / a11y.Policy.TimerScale;                // INF stops non-race timers
if (!a11y.Policy.Consume("ammo", 1, ref ammo)) PlayEmptyClick();     // infinite ammo: never spent
attackCooldown = baseCooldown / Mathf.Max(0.05f, a11y.Policy.EnemyAggression); // 0 = never attacks: skip the attack
Time.timeScale = paused ? 0f : a11y.Policy.TimeScale;                // UI and subtitles use unscaled time
a11y.Policy.SkipRequested += id => content[id].CompleteViaNormalSuccessPath(); // once per id, both kinds
a11y.Policy.RequestSkip("chase_03");            // puzzles, chases, challenges: gated on assist.skip_available
a11y.Policy.RequestSceneSkip("scene_hospital"); // content-notice scenes: gated on content.scene_skip only;
                                                // show the scene's text summary first
// Camera: every shake source, including Timeline/cutscenes, goes through CameraComfort
cameraComfort.AddTrauma(0.6f);         // motion.camera_shake is applied at output each frame; Off stops it at once
bobOffset = ComputeBob() * cameraComfort.BobScale;                   // motion.head_bob
swayOffset = ComputeWeaponSway() * cameraComfort.WeaponSwayScale;    // motion.weapon_sway, visual only
if (cameraComfort.IsMotionEffectEnabled(MotionEffect.FovEffects)) fov *= sprintZoom; // follow = !reduce_motion
// Holds, mashing, QTEs, timing windows
bool aiming = a11y.InputAssist.UpdateHeld("aim", aimAction.IsPressed());
var qte = a11y.InputAssist.StartQte(1.2f, canWaitIndefinitely: nothingElseMoves);
// each frame: if (qte.Tick(Time.deltaTime) == QteState.Success) Succeed(); else if (... == Failure) Fail();
parryWindow = a11y.InputAssist.TimingWindow(0.15f);  // simulation seconds = base x timing_window_scale
// count it down with Time.deltaTime. Never also multiply by Policy.TimeScale or use unscaled time:
// in real time it lasts base x window_scale / time_scale (InputAssist.RealSecondsForWindow) on its own.
encounterChance = baseChance * a11y.Policy.EncounterRate;            // 0 = no random battles; story battles stay
battleAnimator.speed = a11y.Policy.BattleSpeed;                       // animations only, never decision time
// Dialogue and sounds -> CaptionBus (works when muted; never derive captions from audio levels)
a11y.Captions.Localize = key => myLocalization.Get(key);
AudioClip clip = line.voiceClip;                       // null for an unvoiced line
float authoredReadDuration = line.readSeconds;         // authored reading time for text-only lines
float duration = clip != null ? clip.length : authoredReadDuration; // never dereference a missing clip
a11y.Captions.Emit(new CaptionEvent(line.id, CaptionKind.Dialogue, line.textKey, duration,
    speakerId: line.speaker, speakerHidden: !speakerVisible, isVoiced: clip != null), Time.unscaledTimeAsDouble);
if (clip != null) voiceSource.PlayOneShot(clip);
// Unvoiced path (clip == null): the subtitle still shows for authoredReadDuration, and with
// narration.dialogue on, Narrator reads it aloud. Nothing plays on the voice bus.
a11y.Captions.Emit(new CaptionEvent("alarm", CaptionKind.Sound, "snd.alarm", 3f,
    bearingDegrees: Vector3.SignedAngle(cam.forward, alarmPos - cam.position, Vector3.up), mergeKey: "alarm"),
    Time.unscaledTimeAsDouble);
// Flashes: every lightning, hit flash and muzzle flash asks first. A request counts when area >= 0.25 OR
// intensity >= 0.1 OR it is saturated red (red always counts). Counted flashes: <= 3 in any 1 s AND <= 9 in
// any 5 s, in every mode (blocks a sustained 2-3 Hz strobe). Budgets, thresholds and the 1 s / 5 s windows can
// only be tightened: shorter windows are ignored, longer ones are honoured.
// Saturated red is also always desaturated. Pass saturatedRed: true for any saturated red effect.
var d = a11y.FlashGuard.RequestFlash(Time.unscaledTimeAsDouble, intensity: 0.9f, screenArea: 1f, saturatedRed: false);
if (d.Output == FlashOutput.Flash) PlayFlash(d.Intensity);
else if (d.Output == FlashOutput.SteadyFade) PlayFade(d.Intensity, d.MinFadeSeconds);
// Suppressed: convey the information another way (sound, icon, text)
```

`Time.unscaledTimeAsDouble` exists from Unity 2020.2 (verify in your version; `(double)Time.unscaledTime`
works everywhere). Clear toggle latches on pause, death, scene change and device loss:
`a11y.InputAssist.ClearLatches()` (the bootstrap already does it on focus loss and app pause).

**Enemy prefab with ContentVariant.** Parent: collider, AI, health, damage, loot. Children `Original`,
`NoHair`, `NoLegs`, `Replaced`: only renderers, animators and audio. Add `ContentVariant` on the parent,
`settingKey = content.spiders`, entries `"0"`..`"3"` in that order (most filtered last). It swaps in
`Awake`, so instantiated enemies never render the original first. Also tag icons, bestiary pages and loading art.

**Guidance decals.** Add `GuidanceChannel` to each painted ledge decal, trail or highlight, choose the channel,
and list its Renderers or CanvasGroups. `Paint`, `Trail` and `InteractHighlight` read `guidance.paint`,
`guidance.trail` and `guidance.highlights`. Their default `follow` uses `guidance.level`, so a player can keep
trails and turn paint off (`GuidanceRules.EffectiveLevel`). `ObjectiveMarker` reads `guidance.objective_marker`.
Tick `hazard` for lethal edges and attack telegraphs: they stay on at every level. Bind your hint action to `GuidanceChannel.RequestHint(5f)`. Intensity uses a
`MaterialPropertyBlock` float (`_GuidanceIntensity` by default). Property blocks make renderers
incompatible with the SRP Batcher, so for many decals use material variants instead.

**Other keys without an adapter.**
- `hud.mode` (`full`, `minimal`, `custom`, `off`) and `hud.hidden_elements` (a comma-separated list of
  element ids, used by `custom`): your HUD reads them through `Store.Observe`.
- `visual.background_dim`: dims backgrounds behind characters, hazards and interactables, never the
  hazards themselves.
- `content.gore` is separate from `content.blood`; use one `ContentVariant` per key.
- `content.scene_skip` gates `Policy.RequestSceneSkip` (`Policy.SceneSkipAvailable`). It is independent of
  `assist.skip_available`, which gates `Policy.RequestSkip`. Both raise the same `SkipRequested` event once per
  id; call `Policy.ForgetSkip(id)` when the content is reset for a replay.
- `input.target_lock` sets `InputAssist.TargetLock`. Aim assists are three separate keys:
  `InputAssist.AimSlowdown` (`input.aim_assist`, slowdown only), `InputAssist.AimPull` (`input.aim_pull`) and
  `InputAssist.AimStabilization` (`input.aim_stabilization`).
- Outlines: `visual.outline_enemies`, `_allies`, `_interactables` and `_hazards` are `follow`/`off`/`on`.
  Ask `PresentationRules.OutlineEnabled(store, OutlineClass.Enemies)`; `follow` uses `visual.high_contrast`.
- Motion: `motion.auto_camera`, `motion.fov_effects`, `motion.speed_lines`, `motion.parallax` and
  `motion.menu_animation` are `follow`/`off`/`on` overrides. `follow` resolves to `!motion.reduce_motion`
  (`PresentationRules.MotionEffectEnabled`, or `CameraComfort.IsMotionEffectEnabled`). `CameraComfort`
  raises `onMotionEffectsChanged` when any of them changes. For other override keys, use
  `Store.ResolveToggle(key, followValue)`: `on` is true, `off` is false, and `follow` or an unregistered key
  returns `followValue`.
- Scares: `content.jump_scares` is `full` or `reduced` (`PresentationRules.JumpScaresReduced`), and
  `content.scare_warnings` (`PresentationRules.ScareWarnings`) is separate. Full scares with warnings is valid.
- `CameraComfort` raises separate events for `motion.motion_blur`, `motion.chromatic_aberration` and
  `motion.depth_of_field`. Wire each to its own post-process override.

**Audio.** `AudioMixerBinding` maps `audio.*` volumes to exposed mixer parameters (`20 * log10(v)`, 0 maps to
-80 dB). Mono output: `var c = AudioSettings.GetConfiguration(); c.speakerMode = AudioSpeakerMode.Mono;
AudioSettings.Reset(c);` resets the audio system and stops playing sources. Apply it from the menu, then
restart music. Start at the registry's quiet master level, and start music only after volume is reachable.

## 5. Input System rebinding

Gameplay reads `InputAction`s. Rebinding (APIs from the Input System manual and `RebindingOperation` reference):

```csharp
InputActionRebindingExtensions.RebindingOperation op;
void Rebind(InputAction action, int bindingIndex, Action done) {
    action.Disable();                                    // re-enabled in Finish
    op = action.PerformInteractiveRebinding().WithTargetBinding(bindingIndex)
        .WithControlsExcluding("<Pointer>/position")
        .WithCancelingThrough("<Keyboard>/escape")
        .OnMatchWaitForAnother(0.1f)
        .OnComplete(o => Finish(action, done)).OnCancel(o => Finish(action, done))
        .Start();
}
void Finish(InputAction a, Action done) { op.Dispose(); op = null; a.Enable(); SaveBindings(); done(); }
void SaveBindings() => File.WriteAllText(path, actions.SaveBindingOverridesAsJson()); // separate from settings
void LoadBindings() { if (File.Exists(path)) actions.LoadBindingOverridesFromJson(File.ReadAllText(path)); }
string Prompt(InputAction a, int i) => a.GetBindingDisplayString(i);  // never hard-code key names
```

- Cancel: `WithCancelingThrough` takes one path. Also show a Cancel button reachable by UI navigation, so
  gamepad and switch users can leave capture.
- Reset: `action.RemoveBindingOverride(i)`, `actions.RemoveAllBindingOverrides()`. Offer per action and all.
- Conflicts: after completion, compare the new `effectivePath` with other bindings in the map. Offer swap
  or keep both. Never silently unbind. Composites (WASD) rebind each part's binding index.
- Include menus: the UI actions used by `InputSystemUIInputModule` (Navigate, Submit, Cancel) are rebindable
  too. Keep one recovery route, such as a reset key documented in the config file, or a timed revert after
  rebinding Submit. Support more than one binding per action.
- Refresh prompts after rebinding and on device change (for example `PlayerInput.onControlsChanged`).
  Check that event name in your package version.
- Start from the package's Rebinding UI sample (Package Manager > Input System > Samples). It is under the
  Unity Companion License, so keep its licence if you copy it.

## 6. Text, layout and UI scale

- Give every text a base size token and set `fontSize = baseSize * text.scale` (`subtitles.size` for subtitles).
  With TMP, set `enableAutoSizing = false`, or raise `fontSizeMin` to the scaled size, so auto-size never
  shrinks text the player enlarged. `CaptionView` does this.
- Reflow, don't clip: use Layout Groups with `ContentSizeFitter`, allow wrapping, and test `text.scale = 2.0`
  in every language and at the smallest supported resolution.
- `CanvasScaler` in Scale With Screen Size handles resolution only. Do not put `text.scale` into it. For
  `visual.ui_scale`, divide `referenceResolution` by the value (larger UI), or use `scaleFactor` in Constant
  Pixel Size mode.
- `text.font`: keep a TMP Font Asset per option with fallback assets for every script you ship.
  `OsBoldText` (mobile) can select a bolder weight.

## 7. Colour

Use `a11y.Colors.Get("team.enemy")` (convert `Rgba` to `Color`) and always show `Colors.Symbol(token)` as a
sprite or glyph. Listen to `Colors.Changed` and recolour materials and UI per element. The `custom` profile
reads `visual.custom_colors` (`team.enemy=#FF00FF;hazard=#00FF00`). Malformed pairs are skipped, and roles
without a colour use the designer colour. A colour picker calls `Colors.SetCustom(token, rgba)`, which writes
the string back so it persists like any other setting. A full-screen
post-process filter is optional, extra at most. Simulation shaders are developer tools, not player options.
The sample palettes are unvalidated starting points; run `scripts/contrast_check.py` on real backgrounds.

## 8. Focus, navigation and narration

- Every screen is fully operable with keyboard or controller. On open, call
  `EventSystem.current.SetSelectedGameObject(firstControl)`. On close, restore the previous selection. Use
  Explicit `Selectable.navigation` where Automatic picks badly, and never leave a dead end.
- Show a visible focus indicator with at least 3:1 contrast, not colour tint alone.
- Narrate focus with a small `ISelectHandler` component: `OnSelect` calls
  `narrator.AnnounceFocus("Text size", "slider", "150 percent", description)`.
- `Narrator` picks **one** route per announcement. When an OS screen reader is active and reported (§1), focus
  speech is left to the reader and announcements go through `SendAnnouncement`. Otherwise it self-voices
  through your `INarrationBackend`. Menu, focus and game announcements follow `narration.enabled`; unvoiced
  dialogue follows `narration.dialogue` on its own. Turning `narration.enabled` off stops the current menu
  speech and discards queued menu speech at once, and dialogue reading carries on. Before each queued
  utterance is dispatched, the route and its setting are checked again. If the screen reader has become
  active, the handover policy is "stop and transfer": self-voicing is stopped before anything is sent to the
  reader, so two voices never overlap. Then the interrupted utterance (re-announced in full, if it is critical
  or dialogue) and the queued critical announcements and dialogue go to the reader in order. Self-voiced menu
  and focus speech is dropped, since the reader speaks focused controls itself. Your backend's `Stop()` must
  silence speech at once. The rules live in Core (`NarrationQueue`), where the checks exercise them with a
  busy fake backend. **Unity has no built-in cross-platform TTS:** supply a backend (Android `TextToSpeech` through
  `AndroidJavaObject`, `AVSpeechSynthesizer` or SAPI through a native plugin). `LogNarrationBackend` is for
  development only. `INarrationBackend`, `NarrationPriority` and `NarrationChannel` are in the
  `GameAccessibility` namespace.
- On 6000.3+, `SettingsPanelAccessibility` (its own file, so you can add it to a panel) exposes a uGUI panel's
  buttons, toggles and sliders as `AccessibilityNode`s (label, role, value, hint, `frameGetter` with a top-left
  origin, as in Unity's a11y-public-sample). Every frame it re-reads each control. A hidden control (inactive,
  disabled, under a CanvasGroup with alpha 0 or under a disabled Canvas) gets `isActive = false`, so the
  reader skips it, and a layout change is sent. A non-interactable control is marked
  `AccessibilityState.Disabled`. Invoke, increment and decrement run only when `IsActive()` and
  `IsInteractable()` pass, as for ordinary uGUI input, so a disabled destructive button cannot fire through the
  reader. It rebuilds when the reader turns on, because Unity clears `activeHierarchy` when the reader turns
  off. Test with TalkBack, VoiceOver, Narrator or NVDA on device. Editor Play Mode is not evidence.
- UAP (Unity Accessibility Plugin) is a legacy fallback: tested up to Unity 2020.1, Apache-2.0 core with
  excluded example art and an LGPL NVDA client. Do not run it alongside the native hierarchy.

## 9. First launch

Boot scene: a static screen with no audio and no logos. If `!File.Exists(boot.SettingsPath)`, show the
accessibility screen first: text size, subtitles, narration toggle, audio levels, reduce motion and flashing,
and the presets list (`Store.Presets`, each showing its `Changes`, with Undo). Then call `boot.Save()`
so it appears once, and keep it re-openable from Settings. Only then load logos and intro. Settings and the
pause menu follow [architecture.md §10](../../references/architecture.md).

## 10. Export and platform notes

- `persistentDataPath` differs per platform. Consoles need the platform save API: replace `Load`/`Save`.
- `File.Replace` is not available everywhere. `Save` falls back to delete and move, and `Load` recovers a
  leftover `.tmp`.
- WebGL stores files in IndexedDB. Whether writes persist without an explicit sync depends on the Unity
  version (UNVERIFIED). Test reload persistence.
- IL2CPP: Core uses no reflection, so code stripping is safe. `Resources/settings_registry.json` is included in
  builds because it is in a `Resources` folder.
- Android OS-preference import compiles only for device builds (`!UNITY_EDITOR`). Test on a device.
- Cinemachine: multiply impulse and noise gain by `CameraComfort.ShakeScale` every frame (not only when the
  impulse starts), so Off stops a running shake. Field names differ between CM2 and CM3, so check your version.
- Not applicable here: Unreal Lyra/CommonUI licence warnings from the Unreal guide.

## 11. Verify

Follow [verification.md](../../references/verification.md). Unity specifics:
1. Change each option mid-level. It applies without a restart, and other keys stay unchanged.
2. Restart and confirm the values persist. Corrupt one value in `accessibility.json` and only that key resets.
3. Apply and undo a preset, then confirm a later hand-set value survives the undo.
4. Mute all audio and confirm captions still appear.
5. Trigger 10 flashes in one second and confirm at most 3 show. Repeat with a small (9% area, 0.4
   intensity) flash and with a small saturated-red flash. Analyse captured footage separately.
6. Rebind, cancel, reset, restart and confirm the prompts update.
7. Operate every settings screen with a controller alone and with keyboard alone.
8. Grep achievement and save code for `Policy`, `Store`, `assist` and `difficulty`: there must be no matches.
9. Start a long shake, set camera shake to 0% and confirm the camera rests at once. Disable and re-enable the
   rig and confirm position and roll reset.
10. With a screen reader, confirm hidden panel items are skipped and a disabled destructive button does nothing.
11. With self-voicing, queue several menu lines, turn menu narration off and confirm speech stops at once,
    while unvoiced dialogue is still read.
