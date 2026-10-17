# Unreal Engine 5: GameAccessibility module

> **Not compiled here: no Unreal Editor/C++ toolchain on the author's machine. Please report compile
> errors.** API names were checked against Epic's online API reference where marked; everything else
> is labelled UNVERIFIED. Treat this as a reviewed starting point, not tested code.

This is an original C++ runtime module (MIT) that implements the
[architecture contract](../../references/architecture.md) with the shared setting keys. Follow
[core-policy.md](../../references/core-policy.md): options are optional, reversible, available before
the barrier, and never touch achievements or saves.

## 1. Check the engine version first

- Read `Engine/Build/Build.version` (`MajorVersion`, `MinorVersion`) of the installed engine, or the
  project's `EngineAssociation`. The installed headers win over this guide.
- Written against **UE 5.3+**. Version gates: `TObjectPtr`, `FTSTicker` (5.0+);
  `IPlatformInputDeviceMapper` device events (5.1+); Enhanced Input User Settings (5.3+).
- Plugins used by the routes below are Experimental or Beta. Check their status in your version.

## 2. Add the module

1. Copy `engines/unreal/Source/GameAccessibility/` to `<Project>/Source/GameAccessibility/`. Keep the skill's `LICENSE` file (MIT) with the copied code.
2. Copy `engines/unreal/Resources/settings_registry.json` to
   `<Project>/Config/Accessibility/settings_registry.json`. Delete entries the game does not need
   (see `ACCESSIBILITY.md`). `GameAccessibility.Build.cs` stages this file into packaged builds as a
   loose `NonUFS` runtime dependency. `Config/` stages only `.ini` files by default, so this step matters.
3. Add `{ "Name": "GameAccessibility", "Type": "Runtime", "LoadingPhase": "Default" }` to `Modules`
   in the `.uproject`, and add `"GameAccessibility"` to your game module's `PrivateDependencyModuleNames`.
4. Regenerate project files, build, and look for `LogA11y: Accessibility registry: N settings`.

Player file: `Saved/Accessibility.json` (`FPaths::ProjectSavedDir()`). It is versioned, readable and
kept apart from story saves. Writes go to a `.tmp` file that is then moved over the old one. On load,
one bad value is repaired and the rest are kept. Document the file in your README. Consoles and some
mobile targets need the platform save API instead. Replace `Save()`/`Load()` there, and keep a slot
separate from story saves.

Prefer a plugin? Wrap the module in your own `.uplugin` and point `GetRegistryPath()` at the plugin's
`Resources/` folder through `IPluginManager` (add the `Projects` module).

## 3. Service map

| Contract service | Class | Notes |
| --- | --- | --- |
| A11ySettings | `UA11ySettingsSubsystem` (GameInstance) | Typed get/set (bool, float, int, enum, string), `OnSettingChanged` (BlueprintAssignable), explicit-set tracking, `ApplyPreset`/`UndoLastPreset`, `ResetCategory`, OS-pref import stub, `ResolveToggle(Key, bFollowValue)` for `follow`/`off`/`on` override keys |
| AssistPolicy | `UA11yAssistPolicy` (function library) | `DamageToPlayer`, `ShouldFail`, `Consume`, `TimerScale`, `TimeScale`, `EnemyAggression`, `RequestSkip` (challenges, `assist.skip_available`), `RequestSceneSkip` (sensitive scenes, `content.scene_skip`), `ClearSkipRequest`, `AreJumpScaresReduced`, `AreScareWarningsOn`, plus `EncounterRate`, `IsAutoBattleOn`, `BattleAnimationSpeed` |
| InputAssist | `UA11yInputAssistComponent` | Toggle latch, mash modes, QTE modes, `TimingWindow`, aim slowdown / pull / stabilisation strengths (three keys) |
| FlashGuard | `UA11yFlashGuardSubsystem` | A request counts if area ≥ 0.25 **or** intensity ≥ 0.1 **or** saturated red. Counted flashes: no more than 3 in any 1 s and no more than 9 in any 5 s (real time), in every mode, clamped whatever the configuration. Saturated red always desaturated; `IsSaturatedRed` tests linear colour |
| CameraComfort | `UA11yCameraComfortComponent`, `UA11yDepthOfFieldSubsystem` | Shake, head bob and weapon sway (separate keys), FOV, motion blur, chromatic aberration, vignette, depth of field; `IsMotionEffectEnabled` for the per-effect motion overrides |
| CaptionBus | `UA11yCaptionSubsystem` | `FA11yCaptionEvent`, `OnCaption` for UMG; fires even when muted. Your view paginates; it never clips |
| Narrator | `UA11yNarrationSubsystem` + `IA11yNarrationBackend` | Bounded priority queue; menu speech and dialogue reading stop independently when their settings turn off; you bind the TTS backend |
| GuidanceLayer | `UA11yGuidanceComponent` | Per-channel key with `follow` → `guidance.level`; hazard flag; child visibility |
| ContentFilter | `UA11yContentVariantComponent` | Child variants tagged `A11yVariant.<value>`; never falls back below the requested level |
| ColorTokens | `UA11yColorTokens` (DataAsset), `UA11yColorLibrary` | `custom` profile reads `visual.custom_colors`; `ShouldOutline` resolves `visual.outline_*` over `visual.high_contrast`; optional CVD filter helper |

Every component reads the current value in `BeginPlay`, then subscribes. Gameplay code asks the policy
and never reads raw keys.

## 4. Wiring snippets

**Damage and falls**

```cpp
float AMyCharacter::TakeDamage(float Damage, FDamageEvent const& DamageEvent, AController* EventInstigator, AActor* DamageCauser)
{
    const bool bHazard = DamageCauser && DamageCauser->ActorHasTag(TEXT("Hazard"));
    const FName Source = bHazard ? FName(TEXT("hazard")) : FName(TEXT("enemy"));
    return Super::TakeDamage(UA11yAssistPolicy::DamageToPlayer(this, Damage, Source), DamageEvent, EventInstigator, DamageCauser);
}
// Pit / kill volume:
if (UA11yAssistPolicy::ShouldFail(this, TEXT("fall"))) { Die(); } else { TeleportToLastSafeSpot(); }
// Ammo:
if (!UA11yAssistPolicy::Consume(this, TEXT("ammo"), 1.f, Ammo)) { PlayDryFire(); }
```

Blueprint: call **Damage To Player** before **Apply Damage**, and branch on **Should Fail** in the pit
volume. A grab, drowning or scripted defeat needs its own recovery path, so invincibility cannot
soft-lock.

**Timers, game speed, skips.** A countdown subtracts `DeltaSeconds / UA11yAssistPolicy::TimerScale(this)`
and stops when `AreTimersDisabled`. For game speed, `UGameplayStatics::SetGlobalTimeDilation(this,
UA11yAssistPolicy::TimeScale(this))` dilates world time. Verify that menus, UI animation, subtitle
timing and audio are not slowed in your setup (UNVERIFIED in general).

**Skips: two options, one completion path.** A puzzle, chase or challenge calls
`UA11yAssistPolicy::RequestSkip(this, ChallengeId)`, gated only by `assist.skip_available`. A sensitive
scene listed in the content notices calls `UA11yAssistPolicy::RequestSceneSkip(this, SceneId)`, gated
only by `content.scene_skip`. It works with challenge skips turned off or with `assist.skip_available`
absent from the registry. Both broadcast the same `OnChallengeSkipRequested` on
`UA11ySettingsSubsystem`. Bind it on the owner and call the same `CompleteChallenge()` that winning
calls (same flags, items, checkpoint, achievements; a scene adds its optional factual recap). The path is
idempotent: a repeat for the same Id, through either function, returns true without a second broadcast.
Call `UA11yAssistPolicy::ClearSkipRequest(this, Id)` when the challenge or scene can be played again.
Test all four combinations of the two keys, and a registry containing only `content.scene_skip`.

**Scares.** `content.jump_scares` is `full`/`reduced` (`AreJumpScaresReduced`); `content.scare_warnings`
is a separate bool (`AreScareWarningsOn`). Any combination is valid, including full scares with
warnings. Flashes inside a scare still go through FlashGuard.

**Camera shake.** Replace `PlayerCameraManager->StartCameraShake(Class, Scale)` with
`CameraComfort->PlayCameraShake(Class, Scale)`. Every tick, the component also rescales the other
active shakes on the camera manager: it reads `UCameraModifier_CameraShake::GetActiveCameraShakes` and
writes `UCameraShakeBase::ShakeScale`. That covers `UCameraShakeSourceComponent`,
`PlayWorldCameraShake` and, if they run through the camera manager, Sequencer shakes. Check a cutscene
shake at 0%. List head-bob shake classes in `HeadBobShakeClasses` (`motion.head_bob`) and weapon
sway or breathing classes in `WeaponSwayShakeClasses` (`motion.weapon_sway`). They are separate keys.
Procedural bob multiplies by `GetHeadBobScale()`, procedural sway by `GetWeaponSwayScale()`. Sway is
visual only, so never change aim accuracy with it.

**Post-process and FOV.** Author motion blur, chromatic aberration (`SceneFringeIntensity`) and
vignette on the **camera's** post-process settings. The component captures those authored values and
overrides each from its own key: `motion.motion_blur`, `motion.chromatic_aberration`,
`motion.screen_effects` (vignette) and `motion.depth_of_field`. Depth of field uses
`DepthOfFieldEnabled` when you define `A11Y_UE_HAS_DOF_ENABLED=1` (only listed in newer API docs).
Otherwise it falls back to the global `r.DepthOfFieldQuality` cvar, owned by
`UA11yDepthOfFieldSubsystem` (GameInstance, so it survives level changes). Each component holds at
most one "off" request. The subsystem captures the authored quality at the first request and
restores it when the last request is released, so Off → level change → On restores the authored
quality, and overlapping components stay balanced. Route your graphics menu's DOF quality through
`SetAuthoredQuality`. Damage overlays and distortion in
UMG multiply by `GetScreenEffectsScale()`. `motion.fov` is vertical degrees. UE's `FieldOfView` is
horizontal under the default `AspectRatio_MaintainXFOV`, so the component converts with the viewport
aspect. Call `ApplyNow()` after a resize.

**Motion overrides and outlines (`follow` keys).** Some keys are `follow`/`off`/`on` overrides of an
umbrella. `UA11ySettingsSubsystem::ResolveToggle(Key, bFollowValue)` returns true for `on`, false for
`off`, and `bFollowValue` for `follow` or a key the game removed from its registry.

```cpp
// Motion: motion.auto_camera, motion.fov_effects, motion.speed_lines, motion.parallax, motion.menu_animation.
// "follow" resolves to !motion.reduce_motion.
if (UA11yCameraComfortComponent::IsMotionEffectEnabled(this, TEXT("motion.fov_effects"))) { StartSprintFovPulse(); }
// Outlines: visual.outline_enemies / _allies / _interactables / _hazards. "follow" uses visual.high_contrast.
EnemyMesh->SetRenderCustomDepth(UA11yColorLibrary::ShouldOutline(this, TEXT("visual.outline_enemies")));
```

Re-evaluate when `OnSettingChanged` reports the override key **or** its umbrella key.

**QTE and mashing (Enhanced Input).**

```cpp
EIC->BindAction(AimAction, ETriggerEvent::Started,   this, &AMyCharacter::AimPressed);   // -> InputAssist->HandleHoldInput(TEXT("aim"), true)
EIC->BindAction(AimAction, ETriggerEvent::Completed, this, &AMyCharacter::AimReleased);  // -> HandleHoldInput(TEXT("aim"), false)
InputAssist->OnHeldChanged.AddDynamic(this, &AMyCharacter::OnLogicalHeld);               // set bIsAiming here

InputAssist->BeginQte(TEXT("door_kick"), 1.2f, /*bNothingElseMoves*/ true);
// Tick: switch (InputAssist->TickQte(TEXT("door_kick"), DeltaSeconds, bPressedPromptThisFrame))
//   Succeeded -> the normal success branch;  Failed -> the authored retry.
const float ParryWindow = InputAssist->TimingWindow(0.15f);   // simulation seconds
```

**Timing formula.** `TimingWindow(base) = base × input.timing_window_scale`, measured in **simulation
time** (dilated `DeltaSeconds`; pass the same delta to `TickQte`). In real time it lasts
`base × window_scale / time_scale`, so the "More time" preset gets both effects once each. Never
multiply by the time scale as well, and never time the window with an undilated clock.

Call `ClearAllLatches()` on pause, death and level change. Focus loss and device disconnect clear the
latch automatically. Mash: `BeginMash`, then `RegisterMashPress` on each press and `TickMashHold` every
frame. The "single" mode means a long hold is never the only alternative.

**Dialogue and subtitles.** UE's built-in subtitles on `USoundWave`/`UDialogueWave` cover **voiced
speech**. Map `subtitles.dialogue` to `UGameplayStatics::SetSubtitlesEnabled(bool)`. For UMG
styling, `FSubtitleManager::OnSetSubtitleText()` exists, but Epic's reference marks it "HACK". The
**Subtitles and Closed Captions** plugin is listed as Beta in the 5.8 docs. Its first version and API
are UNVERIFIED here. `UA11yCaptionSubsystem` adds **non-speech captions**, direction for the sound
visualiser, and unvoiced lines for narration:

```cpp
FA11yCaptionEvent E; E.Kind = EA11yCaptionKind::Sound; E.Text = LOCTEXT("Alarm", "[Alarm blaring]");
E.bHasSourceLocation = true; E.SourceLocation = GetActorLocation(); E.MergeKey = TEXT("alarm_loop");
GetGameInstance()->GetSubsystem<UA11yCaptionSubsystem>()->PostCaption(E);   // even if SFX volume is 0
```

The UMG view binds `OnCaption` and uses `Routing.TextScale`/`BackgroundOpacity`. Show at most 2 lines
of about 40 characters per page, inside the safe zone. **Never clip or truncate the text** (no
`Clipping` or max-line limit that hides words). Split the full line into pages, show them in order for
the event's duration (or advance on input), and re-paginate when the viewport, `text.scale` or
`subtitles.size` changes. Test a long translated line at 200% subtitle size on the smallest supported
viewport, and check that every word is eventually shown.

**Enemy variants.** Put a `UA11yContentVariantComponent` (key `content.spiders`) under the spider's
root, with child meshes tagged `A11yVariant.0` to `A11yVariant.3`. Use separate components for
`content.blood` and `content.gore`. They are independent keys. Keep collision, hitbox and AI on the
actor. Swap sounds and portraits in `OnVariantChanged`, and cover icons and bestiary entries too. If
the requested variant is missing, the component shows the nearest **more** filtered one and never falls
back below the requested level. If none exists it hides every variant and reports an empty
`ActiveVariant`, so author full coverage, or a neutral stand-in tagged with the highest level.

**Guidance.** Parent paint decals under a `UA11yGuidanceComponent` with `Channel = guidance.paint`,
trails under one with `guidance.trail`, and highlights under one with `guidance.highlights`. Each
channel key defaults to `follow`, which uses `guidance.level`. A player can keep trails and turn
paint off. Lethal-edge markers and attack telegraphs go under a separate one with
`bIsHazardCue = true`. The hint action calls `UA11yGuidanceComponent::RequestHint(this, 5.f)`. Mesh
materials need a `GuidanceIntensity` scalar parameter. Decals and Niagara bind `OnGuidanceChanged`.

**Flashes.**

```cpp
const FA11yFlashResult R = GetGameInstance()->GetSubsystem<UA11yFlashGuardSubsystem>()->RequestFlash(0.8f, 0.6f, false);
if (R.Style != EA11yFlashStyle::Suppressed) { StartFlash(R.Intensity, FMath::Max(R.MinFadeSeconds, AuthoredFade), R.bDesaturate); }
```

Lightning, muzzle flashes, hit flashes and UI transitions all go through it. **Counting rule:** a
request counts toward the budgets if `ScreenArea >= 0.25` **or** `Intensity >= 0.1` **or**
`bSaturatedRed`. A small flash with moderate brightness (area 0.09, intensity 0.4) still counts. Pass
area 1 when you do not know it. The thresholds (`LargeAreaFraction`, `BrightIntensityThreshold`) are
conservative sample defaults. Tune them only with evidence from captured-output analysis. The budgets
are clamped to at most 3 per 1 s and 9 per 5 s whatever you configure. Saturated red is always
desaturated. Classify it with `UA11yFlashGuardSubsystem::IsSaturatedRed(FLinearColor)`, which tests
R/(R+G+B) ≥ 0.8 on **linear** channels. FlashGuard cannot see flashes baked into video, textures or
overlapping shaders, so analyse captured footage as well, as a separate gate (section 8).

## 5. Remapping: Enhanced Input User Settings (5.3+)

- Turn on **Enable User Settings** (`UEnhancedInputDeveloperSettings::bEnableUserSettings`). Otherwise
  `GetUserSettings()` on `UEnhancedInputLocalPlayerSubsystem` returns null.
- Mark mappings as player-mappable. Call `RegisterInputMappingContext(IMC)` for every context,
  including menus, vehicles and minigames.
- `MapPlayerKey(const FMapPlayerKeyArgs&, FGameplayTagContainer& FailureReason)`, `UnMapPlayerKey`,
  `ResetKeyProfileToDefault` (its parameter type has changed between versions, so check your header),
  `SaveSettings()`. Check the `FMapPlayerKeyArgs` fields in your version.
- Don't copy early-UE5 `PlayerMappableInputConfig` tutorials: those APIs are deprecated. Keep
  `UGameUserSettings` (graphics) separate from input settings and from `Accessibility.json`.
- Generate prompts from the current binding. Support secondary bindings, conflicts and reset. Keep a
  recovery path if "confirm" is unbound (for example a reset key in the config file, or a timed
  revert).

## 6. Menus: CommonUI, text scale, colour

- **CommonUI** routes input and back/accept. On each `UCommonActivatableWidget`, override
  `NativeGetDesiredFocusTarget()` (or `BP_GetDesiredFocusTarget`) so focus lands on a real control, and
  `NativeOnHandleBackAction()` for back. Test that focus comes back after a modal closes, and that
  gameplay does not fire under the menu (`GetDesiredInputConfig`). CommonUI is not a screen reader.
- **Text scale.** Use font size tokens. Make one text style or `UTextBlock` subclass that sets
  `FSlateFontInfo.Size = BaseSize * text.scale` and listens for `OnSettingChanged`. Give subtitles
  their own scale. The project **DPI Scaling curve** (Project Settings > User Interface) adapts to
  resolution. It is not a player text-size option, and Scale Boxes can shrink enlarged text back.
  Use wrapping, Size Box minimums and scroll boxes so 200% text reflows without clipping.
- **Colour.** Semantic `UA11yColorTokens` plus an always-visible symbol per token is the real solution.
  With `visual.color_profile = custom`, `ResolveTokenColor` reads `visual.custom_colors`
  (`team.ally=#3366FF;hazard=#FFAA00`, keyed by token name). Malformed entries fall back to the
  palette.
  `UA11yColorLibrary::ApplyOptionalCvdFilter` wraps
  `UWidgetBlueprintLibrary::SetColorVisionDeficiencyType(Type, Severity, CorrectDeficiency,
  ShowCorrectionWithDeficiency)` (signature checked in Epic's reference) as an optional extra. **Severity
  trap:** the Blueprint reference says 0-1, but Lyra's menu shows a 0-10 strength. Read the clamp in
  your engine's `WidgetBlueprintLibrary.cpp` and set `EngineSeverityMax` to match. Never ship
  "show correction with deficiency", the developer simulation, as a player option.

## 7. Screen readers and TTS (two different routes)

- **Native Slate/UMG accessibility** (Experimental): add `Accessibility.Enable=1` to
  `Config/DefaultEngine.ini` (`[ConsoleVariables]`). Epic documents support for Windows (NVDA, JAWS)
  and iOS (VoiceOver). In each widget's Accessibility section, tick **Override Accessible Defaults**,
  then set **Accessible Behavior** (Auto, Summary, Custom, Tool Tip, Not Accessible) and the custom text.
  Documented widgets: Text Block, Editable Text Box, Slider, Button, Checkbox. A custom Slate widget
  overrides `SWidget::CreateAccessibleWidget()` (see `FSlateAccessibleButton`). Don't promise reader
  support on other platforms.
- **Self-voicing plugins:** Text To Speech, Screen Reader (a framework), and Slate Screen Reader.
  Epic says the TTS plugin is English-only. Implement `IA11yNarrationBackend` in your game module
  with `UTextToSpeechEngineSubsystem`: `AddDefaultChannel`, `ActivateChannel`, `SpeakOnChannel`,
  `StopSpeakingOnChannel`, `IsSpeakingOnChannel`, `SetRateOnChannel` (clamped 0-1). The full TODO is in
  `A11yNarration.h`. Call `SetNativeScreenReaderActive(true)` when the native route speaks focus, so
  nothing is spoken twice. This also drops self-voiced focus speech that is already queued or playing.
- **Turning narration off stops it.** The queue subscribes to `OnSettingChanged`. Turning
  `narration.enabled` off stops the current menu/focus utterance and discards queued menu speech.
  Dialogue reading (`narration.dialogue`) is independent: it keeps going, and turning it off stops only
  dialogue. Interrupting menu speech only replaces menu speech: a focus change during a dialogue line
  waits for that line to finish, then speaks. Every queued item is re-checked (setting and route) just
  before dispatch.
- **Test packaged/Standalone builds on each target.** TTS has behaved differently in Play In Editor
  (UE-161327), and the export target decides the backend.

## 8. Photosensitivity capture: EA IRIS Unreal plugin

[IRIS-Unreal-Plugin](https://github.com/electronicarts/IRIS-Unreal-Plugin) (BSD-3-Clause) analyses
captured frames at runtime for luminance flashes, red flashes and patterns. EA's repository documents Windows
support and a PixelCapture dependency. Check it against your UE minor version. Capture
scripted scenarios with `motion.reduce_flashing` on and off. EA disclaims it as a guarantee: report
"analysed with IRIS <version>", never "seizure safe".

## 9. Lyra: reference only

Lyra shows subtitle styling, colour-blind mode and strength, audio and key-binding options. It is
under Epic's licence terms, not MIT. Read it for patterns. **Do not copy Lyra or engine source into
this MIT module or your MIT code**, and do not import the shooter framework to get one slider.

## 10. Verify (see the architecture contract)

- [ ] Packaged build: `Accessibility.json` survives a restart. A hand-broken value is repaired alone.
      A preset undo keeps a later hand edit.
- [ ] Cutscene shake, world shake and head bob are all motionless at 0%. FOV converts correctly at 16:9
      and 21:9.
- [ ] Captions appear with SFX volume at 0. A looping alarm is captioned once.
- [ ] A strobe sequence never produces more than 3 counted flashes in 1 s or more than 9 in 5 s. A
      steady 2.5 Hz strobe is cut off. Reduce flashing turns it into soft fades. Repeated small
      moderate flashes (area 0.09, intensity 0.4) and small saturated-red flashes are limited too.
      Setting the budgets to 10 and 30 still enforces 3 and 9.
- [ ] Depth of field Off → level change → On restores the authored quality. Two camera components
      overlapping keep DOF off until both have released it.
- [ ] Head bob 0 leaves weapon sway as set, and the reverse. `motion.reduce_motion` on with
      `motion.fov_effects = on` keeps FOV effects. Outlines follow `visual.high_contrast` unless a
      class is set to `on` or `off`.
- [ ] A scene skip works with `assist.skip_available` off, and a challenge skip works with
      `content.scene_skip` off. A double request completes the challenge once.
- [ ] Turning menu narration off mid-utterance stops it and drops the queue; dialogue reading
      continues. Moving menu focus while a dialogue line is read lets the line finish, then speaks focus.
- [ ] A long translated subtitle at 200% size is paginated; no word is clipped.
- [ ] The toggle latch clears on pause, alt-tab and controller unplug. QTE auto mode uses the success
      branch.
- [ ] Spider variant 3 is visible on the first frame, and the hitbox is unchanged.
- [ ] `guidance.paint = off` with `guidance.trail = follow` hides paint and keeps trails.
- [ ] Remap, reset and back stay reachable at 200% text. CommonUI restores focus after closing a modal.
- [ ] The achievement code does not reference anything in this module.
