# Architecture, verification, platform policy, and Agent Skills packaging

Research assignment 06. Research date: 2026-10-17.

## Scope and evidence rules

This report follows `00_BRIEF.md`. It covers engine-neutral integration, verification, public platform policy, and distributing the eventual accessibility skill. It does not implement a game or create the final skill.

- **Verified fact** means a linked primary source documents the statement.
- **Recommended design** means this report's engineering synthesis. Proposed schemas, service names, defaults, tests, and repository paths are recommendations, not claims about a shipped game's internal code.
- **UNVERIFIED** means the public research did not establish the claim. In particular, absence of a published certification rule does not establish that no private rule exists.
- **Baseline** means an expectation for every game, with genuinely inapplicable cases recorded. **Contextual** means required when the corresponding mechanic or presentation exists. **Advanced** means a broader implementation that may need more resources.
- Needs overlap. Vision, hearing, motor, cognitive, neurodivergent/sensory, photosensitivity, phobia, motion sickness, stress, and situational use are routing categories, not diagnoses the game should request.
- Defaults below preserve the designer's intended gameplay. Safety-oriented presentation starts conservatively. Players choose assists; the game must not infer a disability or force a preset.

The brief's policy is stronger than a generic accessibility recommendation: no achievement/trophy penalties, content locks, save marks, or shaming for accessibility choices. Do not carry the older local jam-game `ASSIST ON` badge into this skill as an automatic requirement.

## Part A — Implementation architecture

### A1. Start with capabilities and ownership

Microsoft's Xbox Accessibility Guidelines are design and testing best practices; they are not interchangeable with Xbox certification requirements. Their numbered topics provide useful routing categories, including input, difficulty, captions, narration, objectives, motion, and photosensitivity. [Xbox Accessibility Guidelines](https://learn.microsoft.com/en-us/xbox/accessibility/guidelines)

**Recommended design:** inventory the existing game before adding services. Identify who owns damage, resource spending, clocks, input actions, progression, UI focus, audio events, localization, and save files. Extend existing owners instead of introducing a second implementation for accessible play.

| Integration boundary | One owner should expose | Why this boundary matters |
| --- | --- | --- |
| Settings | Typed values, validation, per-key subscriptions, persistence | The title screen, pause screen, config file, and platform preferences must agree |
| Gameplay policies | Damage, resource, timing, targeting, and failure decisions | An assist must affect every relevant mechanic consistently |
| Progression | Encounter/puzzle completion and recovery transitions | Skipping must not leave the world half-completed |
| Presentation | Content variants, guidance channels, motion/flash policies | Assets spawned later must respect current settings |
| Captions | Semantic audio events, localization keys, timing, speaker metadata | Captions should survive mute, pause, localization, and slow motion |
| Accessible UI | Focus, roles, labels, values, actions, announcements | Screen readers and voice control need meaning, not screenshots |
| Input | Logical actions and remap-aware prompts | Adaptive devices should enter the same action pipeline |

These can be a few modules in a small game. They do not require a dependency-injection framework, event-bus singleton, ECS, or custom editor plugin.

### A2. A central typed settings store

Remembering settings avoids making players repeat potentially difficult configuration. This is explicitly recommended by Game Accessibility Guidelines. [Remember settings](https://gameaccessibilityguidelines.com/ensure-that-all-settings-are-saved-remembered/)

**Recommended design:** maintain one registry of definitions and a separate store of player choices. A definition describes an option; a choice stores a value. Resolved effective values may also depend on an available platform preference.

| Definition field | Purpose and example |
| --- | --- |
| `id` | Stable machine key, such as `captions.dialogue.enabled`; never a translated label |
| `type` | Boolean, bounded number, enum, color, binding, or structured value |
| `default` | Explicit intended/safe fallback, distinct from an imported OS preference |
| `range` / `enumValues` | Legal serialized values and meaningful UI increments |
| `labelKey`, `descriptionKey` | Localization keys; description states the practical effect |
| `scope` | Device, player profile, session; save scope only for actual save-owned rules |
| `applyPolicy` | Immediate, next safe simulation boundary, or documented next encounter |
| `platformPreference` | Optional mapping to a supported host preference |
| `contextSignals` | Why the option applies: timed input, resource depletion, spoken dialogue, etc. |
| `consumers` | Owners responsible for applying it, useful for audits and tests |
| `verificationIds` | Named tests and manual scenarios, not a claim that tests already exist |
| `introducedVersion` | Helps author explicit migrations and release notes |

Do not overfill a runtime schema with research prose. Needs, evidence, tiers, and examples can live in a separate authoring manifest that generates documentation and audit checklists.

The runtime API should have narrow operations:

```text
getTyped(optionId) -> resolved value
getChoice(optionId) -> { mode: explicit | system, value? }
setChoice(optionId, choice) -> validation result
subscribe(optionId, callback) -> unsubscribe handle
applyPatch(validatedChoices) -> one coherent update
resetCategory(categoryId) -> reversible category reset
```

#### Change notifications

Recommended event payload:

```text
SettingChanged {
  id,
  previousEffectiveValue,
  effectiveValue,
  source: player | preset | platform | migration | reset,
  revision
}
```

- Publish after validation and committing a coherent batch, not while half a preset has applied.
- Notify a consumer only when its effective value changes. Store a choice change even if the effective value happens to match.
- A new scene/component reads the current value on attachment, then subscribes; it cannot rely on having witnessed earlier events.
- Dispose subscriptions when a screen/actor is removed.
- Marshal platform callbacks to the game's safe UI/simulation boundary.
- Cache a stable modifier snapshot per simulation step when mid-step changes would be inconsistent.
- Avoid polling every setting from every actor each frame.
- Avoid passing mutable settings dictionaries into arbitrary gameplay code.

#### Persistence and migration

The following is a proposed durability strategy, not a platform mandate:

1. Save a small versioned document with stable IDs and explicit choices.
2. Keep personal accessibility preferences outside ordinary story-save rollback.
3. Validate types and bounds before they reach renderer, mixer, physics, or input code.
4. Write through a temporary file and the platform's appropriate replacement/commit mechanism.
5. Keep the last valid document until the new document is safely committed.
6. Debounce slider writes; flush at a suitable menu/lifecycle boundary.
7. Apply sequential, deterministic migrations, such as v1 → v2 → v3.
8. Preserve existing explicit choices when adding options; introduce only the new default.
9. For a renamed or inverted option, migrate its meaning, not merely its key.
10. If a value is invalid, repair that value rather than resetting the entire player's configuration.
11. Define what happens when an older build reads a newer document. Avoid destructive rewriting of unknown fields.
12. Test upgrades from real released versions, not just the previous development snapshot.

Example migration: `disableCameraShake: true` becomes `camera.shake = off`; `false` maps to the previous build's actual intensity, not an assumed new default.

Cloud synchronization needs a defined conflict policy. A recent per-key choice can be merged when trusted revisions exist; otherwise offer a readable local/cloud choice. Do not silently substitute another machine's display mode, input device ID, or speaker setup.

#### Scope and isolation

| Scope | Recommended contents | Important boundary |
| --- | --- | --- |
| Device | Display mode, local audio-device routing, physical-device calibration | Do not blindly roam hardware-specific values |
| Player profile | Text/caption preferences, narration, content filters, bindings, personal assists | Default home for accessibility preferences; no disability label |
| Story save | Actual world state and existing game-rule choices | Loading an old save must not undo current readability or input access |
| Session | Temporary experiment, local co-op participant override | Clearly distinguish temporary from remembered changes |
| Shared game/server | Authoritative multiplayer rules | Do not apply one person's visual filter or narration to every player |

The brief forbids marking a save for using assists. Accordingly, keep assist-use history, permanent `assisted` flags, save-slot badges, and achievement-disqualification bits out of saves. A profile may remember the player's current assist values without branding a playthrough.

For local co-op, separate per-player input/narration/subtitles where practical. A shared screen or global clock needs an explicit shared design; do not claim independent per-player game speed when the simulation cannot provide it.

### A3. Presets plus individual options

**Verified example:** The Last of Us Part I documents vision, hearing, and motor accessibility presets, custom difficulty, and trophy eligibility independent of difficulty. God of War Ragnarök also documents accessibility presets that apply multiple individual settings. These are evidence for the player-facing pattern, not for either game's internal architecture. [The Last of Us Part I](https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/), [God of War Ragnarök](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/)

**Recommended design:** a preset is a versioned patch of existing options. It is not a separate game mode or an invisible permanent constraint.

| Proposed preset | Optional patch contents | Helps / relevant signal | Default and tier |
| --- | --- | --- | --- |
| Low vision | Larger text, strong backgrounds, distinguishable interactables, narration entry point | Vision; visually dense presentation or reading | Not selected; individual readable defaults remain Baseline |
| Deaf / hard of hearing | Dialogue subtitles, sound captions, speaker/direction cues, visual equivalents | Hearing and muted play; meaningful audio | Not selected; caption infrastructure Contextual to audio |
| Motor | Hold/toggle alternatives, generous timings, auto-QTE, aiming/traversal aids where relevant | Motor, fatigue, temporary injury; complex or rapid input | Not selected; context-specific assists Contextual |
| Low stress | Reduced damage, reversible failure penalties, optional invincibility/resources | Stress, cognitive, motor, situational; punitive failure or combat | Not selected; Contextual |
| Reduced sensory | Less shake, blur, optional motion, nonessential visual/audio clutter | Sensory/neurodivergent, motion sickness; intense presentation | Conservative safety defaults independent of preset; Contextual |

These names are optional entry points, not claims that all people in a category want the same values. Avoid forcing a player to identify a diagnosis. Also expose functional categories such as Text, Sound, Controls, Guidance, and Gameplay.

- Show exactly which values a preset will change.
- Applying it should be one coherent patch with an Undo action.
- An individual edit remains possible afterward; display `Custom` if a summary is needed.
- Combining presets must not silently turn off a previously selected aid.
- A preset update should not overwrite established user choices on the next launch.
- Reset a category without also deleting bindings, display settings, or personal presets.
- Keep a usable route to every option outside the preset chooser.
- Do not promise that a preset makes the entire game playable for a disability group without relevant testing.

### A4. First launch, reachability, previews, and live application

SpecialEffect recommends settings access before and during play, with opportunities to preview or test settings. Its DevKit describes The Last of Us Part II's initial setup as an example. [SpecialEffect: Information, sections 5.5–5.6](https://specialeffectdevkit.info/gameplay/5_information/)

**Recommended boot sequence:**

```text
Start minimal accessible shell
  -> load/validate/migrate settings
  -> read supported platform preferences
  -> initialize quiet mixer and conservative presentation
  -> accessible language/narration/text/input entry
  -> optional first-launch accessibility choices
  -> title menu
  -> game content
```

- Apply preferences before studio-logo videos, startup music, legal screens, shader effects, and tutorials.
- If an externally imposed startup screen cannot be changed, record the exact limitation and consult the relevant platform agreement; do not invent an exemption.
- Use a static, readable background for initial setup.
- Do not require hearing speech to enable subtitles or seeing small text to enlarge text.
- Provide an audible, repeatable route to narration where the platform does not already narrate the shell; make its volume controllable.
- Preserve normal keyboard/controller navigation and a clear way to continue.
- Make settings reachable from title and pause using the same screen and data model.
- Do not require completing the tutorial, dying repeatedly, or reaching a particular checkpoint to reveal an assist.
- In modes that cannot pause, preserve a practical settings route and explain live-play behavior accurately.

#### Preview design

| Setting family | Useful preview | Pitfall to avoid |
| --- | --- | --- |
| Text and captions | Sample dialogue, speaker label, sound cue, multiline text | A single short English line hides wrapping defects |
| Contrast and colors | Representative HUD and gameplay objects | Testing against only a flat menu background |
| Controls | Action test area, current input state, remap-aware prompts | Requiring the inaccessible action to leave the test |
| Audio | Player-triggered sample for dialogue/SFX/music/narration | Automatically playing a loud sound after every slider tick |
| Motion and guidance | Safe static or subdued representative sample | Showing the strongest effect before consent |
| Phobia filter | Neutral filtered sample first; original only by explicit request | Previewing an unfiltered spider to demonstrate spider removal |
| Gameplay assists | Small reversible practice encounter | Consuming story resources or altering the real save during testing |

Apply accessibility changes without restart wherever technically possible. For simulation-sensitive settings, apply at the next safe step. If a specific feature requires a scene reload, disclose that fact and keep current access available until the transition; do not present a nonfunctional immediate toggle.

### A5. Platform preference inheritance: what public APIs actually support

**Recommended model:** each inheritable option has `System` and `Custom` choices, with a documented fallback when a system value is unavailable. Unknown is not false. Read at startup and handle supported change notifications; never overwrite an explicit game choice after an OS event.

The following APIs are verified public entry points. Engine support, minimum OS versions, packaging, permissions, and console SDK availability must be checked in the actual target build.

| Target | Publicly documented signals | Integration boundary and uncertainty |
| --- | --- | --- |
| Windows | `Windows.UI.ViewManagement.UISettings`: animation preference and text scaling; `AccessibilitySettings`: high contrast; `Windows.Media.ClosedCaptioning.ClosedCaptionProperties`: caption appearance | Custom-rendered game UI must translate these preferences itself. Caption appearance is not automatic dialogue transcription. [UISettings](https://learn.microsoft.com/en-us/uwp/api/windows.ui.viewmanagement.uisettings), [AccessibilitySettings](https://learn.microsoft.com/en-us/uwp/api/windows.ui.viewmanagement.accessibilitysettings), [ClosedCaptionProperties](https://learn.microsoft.com/en-us/uwp/api/windows.media.closedcaptioning.closedcaptionproperties) |
| macOS | `NSWorkspace.accessibilityDisplayShouldReduceMotion`, `accessibilityDisplayShouldIncreaseContrast`, related display-option notifications | Translate to game presentation; do not assume a renderer automatically inherits AppKit behavior. A general macOS game-text-size inheritance API was **UNVERIFIED** here. [NSWorkspace reduced motion and related properties](https://developer.apple.com/documentation/appkit/nsworkspace/accessibilitydisplayshouldreducemotion) |
| iOS / iPadOS | `UIAccessibility.isReduceMotionEnabled`, `isDarkerSystemColorsEnabled`; preferred content-size categories and change notification | Dynamic Type gives a text-size preference, not an instruction to scale every HUD dimension identically. [Reduce Motion](https://developer.apple.com/documentation/uikit/uiaccessibility/isreducemotionenabled), [Darker system colors](https://developer.apple.com/documentation/uikit/uiaccessibility/isdarkersystemcolorsenabled), [Content-size categories](https://developer.apple.com/documentation/uikit/uicontentsizecategory) |
| Apple caption settings | Media Accessibility exposes preferred captioning characteristics, `MACaptionAppearanceGetRelativeCharacterSize`, and caption-style preference behavior | Verify each function's availability for macOS/iOS deployment targets. Preserve system override semantics where applicable. [Caption characteristics](https://developer.apple.com/documentation/mediaaccessibility/macaptionappearancecopypreferredcaptioningmediacharacteristics(_:)), [Relative character size](https://developer.apple.com/documentation/mediaaccessibility/macaptionappearancegetrelativecharactersize(_:_:)), [Preference behavior](https://developer.apple.com/documentation/mediaaccessibility/macaptionappearancebehavior) |
| Android captions | `CaptioningManager`: enabled state, locale, caption font scale, style, and change listeners | These are caption preferences; the game still authors, times, and renders its own caption events. [CaptioningManager](https://developer.android.com/reference/android/view/accessibility/CaptioningManager) |
| Android motion | `ValueAnimator.areAnimatorsEnabled()` | This reports animator availability, which can reflect more than an accessibility choice; it is not a universal native-game Reduce Motion API. Treat as a conservative animation hint and retain explicit game controls. [ValueAnimator](https://developer.android.com/reference/android/animation/ValueAnimator#areAnimatorsEnabled()) |
| Android contrast and time | `AccessibilityManager.isHighContrastTextEnabled()` is public from API 36; `getRecommendedTimeoutMillis()` from API 29 | Gate by API level. Do not assume older Android exposes this public high-contrast getter. UI timeout preference is not a mandate to change every gameplay timer. [AccessibilityManager](https://developer.android.com/reference/android/view/accessibility/AccessibilityManager) |
| Android text size | Platform text uses `sp`; Android 14 supports nonlinear font scaling | Do not implement text conversion as a blanket multiplication by `fontScale`/`scaledDensity`. Use supported conversion/layout behavior or an explicit verified engine mapping. [Android 14 font scaling guidance](https://developer.android.com/about/versions/14/features) |
| Xbox / Microsoft GDK | Public `XAccessibility` index lists `XClosedCaptionGetProperties`, `XHighContrastGetMode`, and speech-to-text overlay functions | Verify each function's supported targets in the licensed/current SDK. These do not establish public APIs for every reduce-motion or text-size preference. [XAccessibility API index](https://learn.microsoft.com/en-us/gaming/gdk/docs/reference/system/xaccessibility/xaccessibility_members) |
| PlayStation | Public support documents system screen reader, caption styling, controller settings, and chat transcription | A public general-purpose game API for inheriting every requested preference was **UNVERIFIED**. Console UI support does not prove arbitrary game menus are narrated. Consult authorized SDK docs. [PS5 accessibility settings](https://www.playstation.com/en-gb/support/hardware/ps5-accessibility-settings/), [PlayStation accessibility](https://www.playstation.com/en-us/accessibility/) |
| Web games | CSS `prefers-reduced-motion` exposes a motion preference | Mirror it in canvas/WebGL presentation code as well as CSS. Other preferences need separately verified support and fallback. [MDN reduced motion](https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/At-rules/@media/prefers-reduced-motion) |

Do not infer broad accessibility needs from a running accessibility service. Android specifically advises against branching general UI behavior on `AccessibilityManager.isEnabled()`; expose accessible actions through the shared UI path. [AccessibilityManager guidance](https://developer.android.com/reference/android/view/accessibility/AccessibilityManager#isEnabled())

**Recommended precedence:** safe/game fallback → supported system value when the choice is `System` → explicit profile value → intentional temporary preview. Do not stack two independent font multipliers accidentally. Show the resolved value and its source in the settings UI where useful.

**Verified shipped inheritance example:** God of War Ragnarök's accessibility page documents detection of PlayStation mono-audio settings. This demonstrates one specific supported integration, not a general SDK promise. [God of War Ragnarök audio settings](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/)

### A6. Gameplay modifier service

XAG 108 recommends configurable difficulty and ways to adjust individual gameplay barriers. This is useful design guidance, not a requirement that every genre implement combat assists. [XAG 108: Game difficulty options](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/108)

**Recommended design:** keep raw option reads out of individual enemy scripts. Give existing gameplay owners a small policy interface. A typed snapshot resolved from settings is enough for many games.

```text
DamagePolicy.resolve(event) -> { healthLoss, stagger, knockback, recovery }
ResourcePolicy.canSpend(resourceId, amount) -> bool
ResourcePolicy.spend(resourceId, amount) -> result
TimingPolicy.window(actionId, authoredWindow) -> effectiveWindow
InteractionPolicy.mode(actionId) -> press | hold | toggle | automatic
FailurePolicy.resolve(reason, checkpoint) -> fail | recover | continue
ProgressionService.completeOrSkip(encounterId, requestedOutcome)
```

These names are illustrative. Keep one authority per rule. A damage policy should not also own menu focus, caption queues, or cloud saves.

#### Modifier catalogue

Each row is a proposed integration contract. The linked example verifies only the named player-facing feature; none of these sources proves the proposed code structure.

| Option | What it does; who it helps; relevance | Recommended default / tier | Implementation and pitfalls | Verified example |
| --- | --- | --- | --- | --- |
| Incoming damage multiplier | Reduces health loss; motor, cognitive, stress; combat/hazards | `1.0`; Contextual | Resolve at damage authority; distinguish enemy, fall, environmental, and scripted damage; avoid healing through a negative multiplier | Custom player/enemy difficulty parameters in [The Last of Us Part I](https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/) |
| Invincibility | Prevents specified defeat outcomes; motor, stress; lethal gameplay | Off; Contextual | Define knockback, status, drowning, pits, and scripted loss explicitly; zero HP damage alone can still softlock | [Psychonauts 2](https://www.doublefine.com/games/support/psychonauts-2/accessibility?platform=all) documents invincibility against most attacks |
| No fall damage | Removes a specific failure cost; motor, motion/stress; falls | Off; Contextual | Separate falls from other damage; recover from voids to a valid position | [Psychonauts 2](https://www.doublefine.com/games/support/psychonauts-2/accessibility?platform=all) |
| Infinite stamina / ammo / other resource | Removes repeated depletion; motor, cognitive, stress; relevant resource loop | Off separately per resource; Contextual | Intercept spending instead of adding infinity to a numeric field; keep unique quest-item and currency rules distinct | Infinite breath is documented in [The Last of Us Part I](https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/); generalized resource variants here are proposals |
| Game speed | More time to perceive and respond; motor, cognitive; real-time play | `1.0`; Contextual | Declare affected clocks; keep menus/narration responsive; test physics, audio, projectiles, and cutscenes | Slow-mode shortcuts in [Star Wars Jedi: Survivor](https://www.ea.com/able/resources/star-wars/star-wars-jedi-survivor) |
| Wider timing windows | Extends parry, rhythm, puzzle, or reaction windows; motor, cognitive, hearing/vision depending cues | Authored window; Contextual | Scope by mechanic; compare timestamps in the correct clock; avoid double-extension with slow motion | Puzzle timing in [God of War Ragnarök](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/) |
| Auto-QTE | Completes a required reaction interaction; motor, cognitive; QTE present | Off; Contextual | Route through normal success transition, including animation/progression; no missed timeout side effect | QTE Autocomplete in [Marvel's Spider-Man 2](https://support.insomniac.games/hc/en-us/articles/46730041467027-What-Accessibility-options-does-Marvel-s-Spider-Man-2-feature) |
| Hold instead of mash | One held input replaces repeated presses; motor, fatigue; repeated input | Designer behavior plus equally visible alternative; Contextual | Accumulate logical hold duration, cancel safely, and avoid requiring an unusually long hold | [Uncharted 4 accessibility settings](https://gameaccessibilityguidelines.com/uncharted-4-accessibility-settings/) |
| Toggle instead of hold | Removes sustained pressure; motor/fatigue; aiming, crouch, sprint | Designer behavior with configurable alternative; Contextual | Reset stuck state on menu entry/disconnect; do not require a second inaccessible chord | Toggle aiming in [Uncharted 4](https://gameaccessibilityguidelines.com/uncharted-4-accessibility-settings/) |
| Aim assistance | Reduces precision demand; motor, vision; aiming mechanics | Existing design default; Contextual | Separate sensitivity, slowdown, lock-on, target switching, and puzzle assist; avoid removing player control unexpectedly | [God of War Ragnarök](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/) |
| Skip puzzle / encounter | Advances past a blocker; cognitive, motor, stress, phobia depending content | Available, invoked by player; Contextual | Use explicit completion/recovery transitions and preserve world consistency; never require repeated failure first | Skip-puzzle option in [The Last of Us Part I](https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/); arbitrary encounter skip is a proposal |
| Granular fail states | Disable selected health, fall, timer, stealth, ally, or resource failures | Existing rules; separate toggles when relevant; Contextual | Use a typed failure reason; retain desired failure types and ensure a recoverable world state | Separate invincibility and no-fall-damage options in [Psychonauts 2](https://www.doublefine.com/games/support/psychonauts-2/accessibility?platform=all) |

The resource list is not a requirement to add ammunition or stamina to a game that lacks them. Likewise, a turn-based game may need response-time removal rather than global slow motion.

#### Clocks and time dilation

Recommended clock inventory:

- **Simulation time:** movement, combat, physics-facing updates, authored hazards.
- **Presentation/media time:** audio playback, video, subtitle cue synchronization.
- **UI time:** menu animation, focus feedback, player-controlled previews.
- **External time:** network deadlines and platform service operations.

Define which clock every timing aid changes. If audio continues at ordinary speed, captions should follow the audio rather than a slowed gameplay timer. If an encounter timer is deliberately extended, do not also extend it again through an accidental second multiplier.

In networked play, an individual client cannot safely change the authoritative match clock. Design negotiated modes/server rules where relevant and document unsupported combinations. Do not silently ignore the selected setting.

#### Failure and skip transitions

Recommended implementation sequence:

1. Represent failure causes explicitly, such as `enemy_damage`, `fall`, `timer_expired`, `ally_defeat`, and `detection`.
2. Route each through the corresponding owner and policy.
3. For disabled failures, choose a real continuation: checkpoint recovery, replenishment, alternate route, or completed interaction.
4. Keep transitions idempotent so simultaneous failure/success events cannot duplicate rewards.
5. Restore camera, input, AI, collision, music, and active objective state after recovery or skip.
6. Update prerequisites through the normal progression system.
7. Preserve content rewards/ending access according to the brief.
8. Test saving and reloading immediately after the transition.

### A7. Achievements, trophies, leaderboards, and public platform policy

#### What is actually public

| Platform / document | Verified public statement | What must not be inferred |
| --- | --- | --- |
| Steamworks Stats and Achievements | Developers define application-specific achievement conditions and unlock them through the supplied achievement/stat APIs. The reviewed public documentation does not impose an accessibility-based disqualification rule. [Steamworks achievements](https://partner.steamgames.com/doc/features/achievements) | This is not proof that every Steam agreement, game-specific anti-cheat system, or future policy permits every implementation |
| Xbox XR-057 | Achievements require corresponding game actions, gameplay, or experiences; direct unlocks through purchases, cheat codes, or menu/console options without such activity are disallowed. The rule explicitly permits these options to reduce the gameplay difficulty needed for achievements. [XR-057](https://learn.microsoft.com/en-us/gaming/gdk/docs/store/policies/xr/xr057) | It does not require difficulty-gated achievements or require accessibility penalties; nor does it permit an “unlock all achievements” accessibility button |
| Xbox XAGs | Published accessibility best practices and evaluation guidance. [XAG overview](https://learn.microsoft.com/en-us/xbox/accessibility/guidelines) | Do not label every XAG recommendation a mandatory XR certification test |
| PlayStation | The Last of Us Part I's official accessibility article states that trophies are not limited by difficulty. Public PS5 accessibility tags include difficulty-related features. [Game article](https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/), [Store accessibility tags](https://blog.playstation.com/2023/04/03/accessibility-tags-roll-out-this-week-on-playstation-store-on-the-ps5-console/) | A universal TRC rule about difficulty-gated trophies/accessibility is **UNVERIFIED**. One game's policy is not the platform certification manual |
| Nintendo | The public developer portal explains registration and the publishing process. [Nintendo process](https://developer.nintendo.com/the-process) | A public lotcheck rule requiring or prohibiting accessibility-related achievement/difficulty restrictions was **UNVERIFIED**. Do not use leaked/confidential manuals as sources |

**Policy conclusion:** the reviewed public sources do not establish a platform requirement to punish accessibility use. Xbox's public achievement requirement expressly allows reducing required gameplay difficulty. For private console certification details, the shipping studio must check its authorized current documentation; this report cannot certify compliance.

#### Preserve eligibility by construction

**Recommended project rules derived from the brief:**

- Progression emits semantic milestones such as `chapter_completed` or `boss_resolved`.
- Achievement evaluation consumes those milestones and objective evidence.
- It does not consume assist settings, preset names, or a historical `everUsedAssist` flag.
- Enabling, disabling, or changing an assist never clears progress or earned rewards.
- Do not send disability/preset information to platform stats or public rich presence.
- Do not prefix a save slot, result, ending, grade, or achievement with an assist badge.
- Keep rewards, narrative content, and ordinary progression available with assists.
- Test unlock behavior using platform test/development facilities, not production achievement resets.

Keeping eligibility does not mean automatically awarding an objective that was never performed. For example, a collectible objective still needs a supported way to obtain its collectibles. Design an accessible equivalent or reconsider the objective; do not add a blanket award button that conflicts with XR-057.

For skip features, document the intended objective semantics: “complete the chapter” can include its supported skip transition; “perform action X” needs actual or explicitly designed equivalent gameplay. Design this before publishing achievement definitions.

Existing difficulty-only achievements create a retrofit conflict with the owner's no-penalty policy. Surface it as product/platform work: redesign conditions or add accessible equivalent paths under authorized publishing procedures. Do not silently change public descriptions or assume an agent can rewrite deployed trophy definitions.

#### Leaderboards

Steamworks documents multiple named leaderboards and score sorting/update behavior, so separate rule-set boards are technically possible. That is not an accessibility policy recommendation. [Steamworks leaderboards](https://partner.steamgames.com/doc/features/leaderboards)

Recommended default for this brief: do not disqualify, suppress, or label scores merely because an accessibility option is enabled. Presentation preferences, captions, bindings, screen readers, and content presentation swaps should never select a different board.

A separate leaderboard category can be a reasonable competitive product design when it represents an openly chosen ruleset available to everybody, such as different event rules, and preserves equal rewards and content. It must not infer disability or permanently tag a save. However, **mandatory assist-driven segregation would be an exception to this brief**, not something this research authorizes. The eventual skill should report that conflict instead of implementing it automatically.

For ranked multiplayer, distinguish presentation/input access from server-authorized gameplay rules. Keep third-party competition rules, platform policy, and the owner's product policy separate; do not claim all three are identical.

### A8. Content filters and phobia modes

**Verified evidence:** XAG 123 recommends advance content information and options to customize or bypass potentially distressing content. Grounded is its documented example of a visual-only arachnophobia mode. Obsidian patch notes also add support to spider-head mounts and a Broodmother trophy, demonstrating why coverage must include secondary depictions. [XAG 123](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/123), [Spider-head mounts](https://www.obsidian.net/news/grounded/shroom-and-doom-update-out-now), [Broodmother trophy](https://grounded.obsidian.net/news/get-ready-as-the-bugs-strike-back)

**Recommended design:** tag content at authoring time and resolve a presentation variant before first render/playback.

| Layer | Suggested metadata | Coverage requirement |
| --- | --- | --- |
| Actor/model | `contentTags`, stable logical identity, variant key | Living actor, corpse, ragdoll, shadow, reflection, silhouette |
| World art | Tagged props, decals, textures, baked depictions | Nests, posters, statues, trophies, environmental storytelling |
| Audio | Semantic event and alternate event IDs | Movement, idle, attack, death, ambient and off-screen cues |
| UI | Icon/card/thumbnail variant | Inventory, bestiary, loading screen, quest log, achievements shown in game |
| Video | Tagged segment or alternate render | Intro movies, cutscenes, recaps, thumbnails |
| Text | Neutral description or alternate wording when appropriate | Avoid exposing unwanted details merely to explain the filter |

Proposed asset table:

```text
logicalId: enemy.cave_creature
tags: [arachnid_like]
variants:
  original: { mesh: ..., audioSet: ..., icon: ... }
  simplified: { mesh: ..., audioSet: ..., icon: ... }
gameplayDefinition: enemy.cave_creature.rules
collisionDefinition: enemy.cave_creature.collision
```

The rules/collision references stay outside the presentation variant. For a visual/audio-only mode, preserve hitboxes, hurtboxes, interaction distance, timing, navigation, rewards, and replication. Ensure the replacement visibly communicates the actual hittable volume; identical invisible collision with a misleading tiny icon is still poor access.

If a player explicitly chooses removal or encounter bypass, gameplay necessarily changes. Give that a separate clearly described setting; do not claim it has identical hitboxes or difficulty.

- **Phobia visual simplification:** Contextual to relevant content; off by default after a neutral pre-content explanation, with remembered choice.
- **Audio substitution:** Contextual when characteristic sounds are triggers; link to the filter or expose separately; do not leave the original sound in the filtered variant.
- **Sensitive-content bypass:** Contextual; player-invoked and available before and during the sequence; preserve progression.
- **Extensible authored tag catalogue:** Advanced for large games/mod APIs; do not invent a list of every possible phobia for a tiny prototype.

Who it helps: phobia, stress, sensory needs, and situational preferences. The necessary testing is whether the intended player finds the replacement usable; a developer's belief that a cartoon version looks harmless is not sufficient.

When changing the setting while actors are already present, replace their presentation safely and invalidate cached UI/video previews. Never briefly render the original while streaming the alternate asset.

### A9. Guidance layers: less stimulation and more support

XAG 109 recommends accessible current objectives, progress tracking, and ways to revisit narrative context. God of War Ragnarök documents navigation assistance and HUD-related options. These support offering guidance, but do not prove a particular engine tagging implementation. [XAG 109](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/109), [God of War Ragnarök](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/)

**Recommended design:** author guidance as identifiable presentation channels rather than baking it irreversibly into textures.

```text
guidance.nonessential.paint
guidance.nonessential.trail
guidance.nonessential.outline
guidance.nonessential.waypoint
guidance.objective.tracker
guidance.memory.recap
guidance.hint.on_request
```

One `guidance` group makes bulk dim/off/boost possible; subchannels preserve individual control. A single all-or-nothing visibility flag is usually too coarse.

| Option | Behavior / who / signal | Default and tier | Pitfalls |
| --- | --- | --- | --- |
| Guidance intensity | Off, reduced, authored, boosted; sensory/attention or low vision; paint/trails/glows | Authored, with motion safety limits; Contextual | Baked paint cannot disappear with a UI toggle; test decals, materials, particles, minimap and distant LODs |
| Minimal HUD | Hide nonessential HUD categories; sensory/cognitive; busy HUD | Authored; Contextual | Preserve required decisions through chosen channels; never hide pause/settings access |
| Persistent objective tracker | Keep current action and completed steps visible; memory/cognitive; multi-step goals | Designer default with opt-in persistence; Contextual | Avoid flicker, needless animation, and transient-only task updates |
| Recap / task history | Revisit story and recent actions; memory, attention, interrupted play | Available on demand; Contextual | Do not accidentally spoil unencountered content |
| Extra hints / explicit next step | Graduated hint to direct solution; cognitive, stress; ambiguous puzzles or navigation | On request; Contextual | Let players control unsolicited hints and spoilers independently |

The combined dim/off/boost architecture and exact setting set are **proposals**. A verified shipped example of that exact unified group was **UNVERIFIED** here.

Keep hazard communication distinct from optional guidance. If reduced guidance removes the only indication of a lethal edge or an interaction target, the design needs another accessible cue. Where a player deliberately requests stronger suppression, explain the effect without moral language and preserve reversal.

### A10. Subtitle and caption data pipeline

XAG 104 distinguishes dialogue subtitles from captions for meaningful non-speech sounds and provides guidance on speaker identification and presentation. WebVTT defines timed cues, cue identifiers, voice annotations, regions, and styling; TTML provides another timed-text exchange format. Neither alone defines a game's complete reactive caption system. [XAG 104](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/104), [WebVTT](https://www.w3.org/TR/webvtt1/), [TTML2](https://www.w3.org/TR/ttml2/)

**Recommended authoring pipeline:**

```text
Dialogue/audio source + semantic sound-event catalogue
  -> stable cue IDs + speaker IDs + localization keys
  -> localized text and locale-specific timing review
  -> validation/build import
  -> runtime semantic cue stream
  -> caption layout + optional transcript/narration consumers
```

Use the team's existing dialogue/localization tools. CSV, JSON, engine resources, WebVTT, and TTML are possible interchange choices; do not create a new toolchain solely to satisfy a preferred file extension.

Suggested cue fields:

```text
Cue {
  id,
  kind: dialogue | sound | music,
  textKey,
  speakerId?,
  speakerDisplayKey?,
  sourceEntityId?,
  startOffsetMs,
  endOffsetMs?,
  timingSource: media | gameplay_event,
  priority,
  repeatPolicy,
  directionPolicy,
  localeOverrides?
}
```

- Stable speaker IDs must not be translated; displayed names should be localized.
- Keep identity separate from presentation. An unseen speaker may be “Unknown voice” until the story permits identification.
- Author meaningful sound events, such as a nearby alarm or an opening door, rather than captioning every audio sample filename.
- Trigger captions from the semantic event even when the mixer is muted. Do not listen to final audio amplitude to decide whether a caption exists.
- Give looping sounds start/update/stop semantics and a repeat policy; do not spam the same caption each audio tick.
- Provide visual direction/distance only when relevant and avoid leaking information that hearing players would not receive.
- Couple prerecorded dialogue cues to actual playback position, including pause, seek, skip, buffering, and selected playback rate.
- For reactive sounds, define readable minimum display and grouping rules in UI time.
- Permit overlap where multiple speakers/sounds matter; establish priority without silently discarding critical warnings.
- Treat caption text/background size, position, speaker labels, and sound captions as independently adjustable where useful.
- Keep captions inside the safe area and resolve collisions with important prompts/HUD.
- Test font coverage, right-to-left layout, line breaking, text expansion, and localized audio durations.
- Keep text editable in localization files; do not bake important subtitles into video frames.
- If live speech recognition supplements captions, label the distinction and test errors/latency; it does not replace authored critical cues.

Default recommendation: dialogue subtitles on when speech conveys information; meaningful sound captions available and easy to enable, with a preview. This is a project recommendation, not a universal platform requirement. Tier: Contextual to speech/audio; the common settings and localization infrastructure are Baseline when such content exists.

**Verified example:** The Last of Us Part I documents subtitle names and direction settings. Do not infer its authoring format from that public feature list. [Official accessibility list](https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/)

### A11. Screen readers, TTS, UI focus, and voice input

XAG 106 covers menu and UI narration. Windows UI Automation requires appropriate providers for custom controls. AccessKit supplies a shared accessibility schema and platform adapters, but its documented coverage is not a guarantee that any particular engine/game automatically supports screen readers. [XAG 106](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/106), [UI Automation providers](https://learn.microsoft.com/en-us/windows/win32/winauto/uiauto-providersoverview), [AccessKit](https://github.com/AccessKit/accesskit)

**Recommended semantic UI adapter:**

```text
AccessibleNode {
  stableId, role, localizedName, localizedDescription,
  value, minimum?, maximum?, selected?, disabled?,
  bounds, parentId, children, supportedActions
}

Events: focusChanged, valueChanged, selectionChanged,
        dialogOpened, errorRaised, statusUpdated
Actions: invoke, increment, decrement, setValue,
         select, scroll, focus, dismiss
```

Both ordinary input and accessibility actions should invoke the same command handler. Do not implement a second “accessible button” behavior that drifts from the visible one.

#### Focus narration contract

- Narrate the control's name, role, current value, disabled state, and relevant help.
- Use current localization and remapped actions; never read an internal setting ID aloud.
- Keep a deterministic focus order and return focus sensibly after closing a dialog.
- Ensure every meaningful control is reachable without pointer movement.
- Expose modal boundaries and errors; do not read a hidden background menu over the active dialog.
- Debounce repeated slider announcements; announce a useful final value.
- Interrupt obsolete focus speech when focus changes, while preserving important warnings.
- Permit repeat/help, rate, voice, and narration-volume control where supported.
- Avoid double narration when both an OS screen reader and built-in speech are active.
- Make narration available before a visual-only opt-in barrier.
- Test the actual target screen reader with the built game, not only the engine editor.

Built-in TTS is a useful fallback, but it does not by itself provide screen-reader navigation, braille output, accessible text editing, or voice activation of controls. The skill should distinguish these capabilities in its audit report.

| Capability | Recommended default / tier | Who / signal | Verification focus |
| --- | --- | --- | --- |
| Semantic menu accessibility | Present by design; Baseline target for menus | Blind/low-vision, motor and cognitive; menus | Roles/names/values/actions, focus order, actual AT operation |
| Built-in menu narration | Follow supported system preference or accessible opt-in; implementation depth Advanced | Vision/reading; no reliable native bridge | Speech coverage, startup access, no overlap or dead ends |
| UI status/error narration | Enabled with narration; Contextual | Vision/cognitive; dynamic status | Important changes announced without excessive interruption |
| Voice-operable UI labels | Labels present by design; Baseline for supported UI integration | Motor; voice-control use | Visible/semantic names align; actions execute the same command |
| Native in-game command recognition | Optional; Advanced | Motor; repeated commands | Privacy, recognition errors, language support, equivalent nonvoice path |

Apple documents accessibility input labels for Voice Control and Full Keyboard Access. Android provides semantic content labels consumed by assistive technology. These are verified hooks; neither establishes automatic support for an unlabeled custom 3D menu. [Apple input labels](https://developer.apple.com/documentation/swiftui/view/accessibilityinputlabels(_:)-9q3yf), [Android content labels](https://support.google.com/accessibility/android/answer/7158690)

For optional native voice commands, route recognition results to logical actions. Make listening state clear, request microphone access only when used, provide stop/cancel and a nonvoice equivalent, and document whether processing is local or remote. These are proposed product requirements, not a claim that every platform offers identical speech APIs.

### A12. Mod and adaptive-hardware openness

Steam Input supports controller configuration and legacy emulation of mouse, keyboard, and gamepad inputs. Its developer guidance describes obtaining actual assigned action origins for appropriate on-screen glyphs. [Steam Input](https://partner.steamgames.com/doc/features/steam_controller), [Legacy bindings](https://partner.steamgames.com/doc/features/steam_controller/legacy_mode), [Developer integration](https://partner.steamgames.com/doc/features/steam_controller/getting_started_for_devs)

**Recommended architecture:** hardware events → remapping/action layer → interaction policy → game commands. Voice recognition, adaptive devices, keyboard macros, and test automation should enter through supported actions where feasible.

- Expose documented settings files using the same schema and validation as menus.
- Give configuration a schema version and readable stable keys.
- Provide a reset/recovery path outside the game for accidentally unusable bindings.
- Support multiple bindings per action, axis inversion, sensitivity/dead zones, and separate menu/gameplay contexts where relevant.
- Preserve an accessible confirm/back/pause route during remapping; offer a tested reset.
- Avoid hardcoding controller brand, requiring two simultaneous devices, or rejecting emulated input solely because it is emulated.
- Support input-device changes without losing focus or invalidating bindings.
- For mods, expose versioned settings/actions/content-variant hooks rather than encouraging patching scattered private fields.
- Keep personal accessibility config distinct from executable mod code and online authoritative rules.
- Document which configuration changes require reload and which apply live.
- Let screen readers and overlays coexist with supported rendering modes; test focus and audio behavior.
- Do not disable Steam Input, AutoHotkey-style remappers, or assistive overlays merely as an unexplained blanket policy.

These openness points are project recommendations. **UNVERIFIED:** universal compatibility or blanket permission for AutoHotkey, overlays, and mods under every anti-cheat/platform/game policy. Do not promise that a third-party tool cannot trigger a game's anti-cheat. For competitive/networked releases, agree and publish supported pathways with the relevant platform/security owners, while keeping accessible native remapping available.

Hardware support is Contextual to supported inputs; a stable logical action layer and truthful documentation are Baseline. A public mod API is Advanced and should not become a prerequisite for basic accessibility.

## Part B — Verification a coding agent can perform

### B1. What a passing automated check means

Game Accessibility Guidelines recommends involving disabled players early because generic checklists do not reveal every game-specific barrier. W3C similarly describes user evaluation as complementary to technical evaluation. Web-specific standards should not be presented as complete native-game certification. [Game playtesting guidance](https://gameaccessibilityguidelines.com/include-some-people-with-impairments-amongst-play-testing-participants/), [W3C: involving users in evaluation](https://www.w3.org/WAI/test-evaluate/involving-users/)

**Recommended evidence levels:**

| Evidence level | What it can establish | What it does not establish |
| --- | --- | --- |
| Static/schema check | Values, metadata, references, and declared coverage are internally consistent | A player can find or use the feature |
| Unit test | A local policy has the expected behavior for specified inputs | All encounters invoke that policy |
| Integration/scenario test | A particular built scenario works with specified settings/devices | Every route through the game works |
| Captured output analysis | Measured frames/text/audio in that capture meet selected criteria | Every uncaptured effect, display, or player experience is safe/accessible |
| Disabled-player playtest | Actual participants can use the tested tasks under documented conditions | Universal accessibility across every disability and platform |
| Platform submission result | The submitted build meets the tested submission requirements | A blanket accessibility guarantee |

Report `pass`, `fail`, `not applicable`, `blocked`, and `not tested` separately. A missing analyzer or missing console SDK is not a pass. Keep evidence tied to build ID, settings revision, scene, locale, resolution, display pipeline, device, and tool version.

All tests below are proposed acceptance tests for the future skill. This research assignment has not run a game or validated an implementation.

### B2. Static and schema checks

Recommended Baseline checks:

- Unique stable option IDs and valid type/range/default combinations.
- Every exposed option has a localized label and practical effect description.
- Every option has an owner that actually applies it.
- Presets reference real options and legal values.
- Every relevant option has at least one verification scenario.
- Serialized version and migration path are explicit.
- No save/achievement schema fields such as `assistDisqualified` or permanent assisted-run markers are introduced.
- No achievement evaluator references assist settings through a hidden shared helper.
- All UI text required for navigation is present in supported fonts/localization tables.
- Every required dialogue/audio cue has a caption mapping or an explicit documented nonessential reason.
- Content-filter variants cover all tagged assets, including icons and cinematics.
- Logical actions used by prompts and remapping exist.
- Menu focus graphs contain no unreachable required controls or trapped modal loops.

Static scans find suspicious patterns, not proof of behavior. For example, an achievement module can call a function that hides an assist check; inspection and runtime scenarios remain necessary.

### B3. Settings unit and integration tests

| Test | Setup and assertion | Risk caught |
| --- | --- | --- |
| Round trip | Set every supported type, save, create a fresh store, reload, compare choices and effective values | UI appears changed but persistence loses data |
| Defaults | Load a new profile with no file; assert intended defaults and safety-oriented presentation | Missing keys resolve to zero/false accidentally |
| Per-key isolation | Change captions; assert damage, bindings, volume, and other unrelated values remain unchanged | Shared indices or category reset errors |
| Profile isolation | Configure two player profiles, switch repeatedly and relaunch | One player's preferences overwrite another's |
| Story-save independence | Change text/input, load an earlier story save | Story rollback restores inaccessible settings |
| Device isolation | Load roamed profile on a second simulated hardware configuration | Invalid device/display identifiers copied across machines |
| Migration semantics | Load fixtures from each shipped version; verify renamed, split, and inverted settings | Syntactic migration changes player intent |
| Migration idempotence | Reload migrated output; assert no further changes | Repeated migrations drift values |
| Interrupted write | Simulate failure at temporary write/commit boundaries | Valid existing preferences lost after interruption |
| Partial corruption | Corrupt one numeric/enum field | Whole accessibility profile resets |
| Future version | Open newer document with older reader under its documented policy | Downgrade silently deletes newer preferences |
| Event isolation | Set one value; observe only corresponding consumers and one coherent revision | Global redraw storms and unrelated gameplay changes |
| No-op update | Set same effective value; confirm no duplicate consumer effects | Repeated announcements and expensive rebuilds |
| Late subscriber | Spawn a UI/actor after setting an assist | Newly spawned content ignores settings until next toggle |
| Preset patch | Apply and undo a preset; verify exact changed values | Preset secretly overwrites unrelated preferences |
| Preset update | Upgrade content containing a revised preset | Launch overwrites personal adjustments |
| Platform inheritance | Change mocked system value in System mode and Custom mode | OS events override explicit game choices |
| Unsupported platform | Return unavailable for a preference | Unknown treated as a request to disable access |
| Preview cancellation | Adjust then cancel an experiment | Preview unexpectedly persists or leaves stale consumers |
| Reset scope | Reset one category, relaunch | Reset also deletes other categories or profile identity |

Use representative boundary values and invalid values, including non-finite floats where the format/parser can produce them. Tests should check meaningful behavior rather than duplicate the registry implementation line for line.

### B4. Scenario tests that prove an assist reaches gameplay

**Recommended two-layer plan:** a small deterministic fixture for each mechanic plus completion coverage for the actual game. A fixture validates the policy; a route through real content proves integration.

For a small prototype, complete the whole game with each applicable assist separately, then with key combinations. For a large/open-ended game, automate representative main-story/progression routes and every assist-specific blocker, add release-scale completion testing, and report the exact coverage. A sampled route is not “the entire game passed.”

| Scenario | Required assertion beyond “no crash” |
| --- | --- |
| Damage multiplier | All listed damage sources use the configured factor; unchanged sources match the option's description |
| Invincibility | Enemy attacks cannot defeat the player; status effects, falls, grabs, and scripted sequences follow documented policy |
| Infinite resource | Resource-using actions still execute; no underflow, NaN, permanent busy state, or broken quest-item consumption |
| Slow motion | Combat, physics, AI, projectiles, animation events, audio, and captions remain synchronized according to the clock policy |
| Wider timing window | Boundary input that failed under normal timing succeeds within the configured extension |
| Auto-QTE | Success/progression occurs without the QTE input; failure timeout cannot race it |
| Hold instead of mash | A single uninterrupted hold performs the action; repeat events are unnecessary |
| Toggle hold action | One press starts, one press ends; menu/disconnect does not leave an action stuck |
| Aim assistance | Target acquisition/switching works through supported input without breaking puzzle targeting |
| Skip puzzle | Prerequisites, rewards, doors, quest journal, checkpoint, and subsequent scene are coherent |
| Skip encounter | Enemies, music, camera, combat locks, and objective state cleanly transition; no stranded NPC |
| Granular failure | Disabled failure recovers; each intentionally retained failure still works |
| Guidance off | Optional paint/trails/glows/waypoints disappear; pause/settings and necessary alternative information remain usable |
| Extra guidance | Trackers update across task completion, reload, death, and returning after a long break |
| Phobia variant | No original model/audio/icon appears before, during, or after a swap, death, inventory display, or reload |
| Captioned muted run | Critical audio events remain understandable with all game audio muted |
| Narrated menu run | New game, settings, save/load, error handling, pause, and exit work without visual menu information |
| Achievement parity | The same supported objective completion generates the same achievement request with relevant assists on/off |

Run assist transitions too: enable midway through damage, disable after infinite-resource use, change speed during a timed interaction, switch filters with live actors, and open settings while a prompt is active.

Recommended interaction combinations:

- Slow motion + extended timing + subtitles.
- Invincibility + pit recovery + checkpoint reload.
- Infinite stamina + toggle sprint + traversal assistance.
- Maximum text + maximum captions + remapped long prompts.
- Reduced sensory + high contrast + guidance boosted.
- Guidance off + explicit memory tracker on.
- Screen reader + controller disconnect/reconnect + modal error.
- Filter enabled + newly streamed content + replayed cinematic.

Pairwise coverage is useful for scale but must not replace known hazardous combinations. Run explicit safety-critical and progression-critical combinations even if a pairwise generator omitted them.

Use fixed seeds, stable fixtures, and bounded test timeouts. Store action traces and relevant state transitions. Never mistake a bot teleporting to the ending for proof that accessible controls can complete the route.

### B5. Muted play and meaningful audio equivalence

Recommended Contextual verification for meaningful audio:

1. Disable all game audio at the mixer, not the caption event source.
2. Start from first launch and complete settings without relying on sound.
3. Trigger critical dialogue, alarms, hazards, directional enemies, timer cues, and off-screen events.
4. Assert a corresponding accessible channel exists where required.
5. Verify cue identity, speaker/source, timing, priority, readability, and direction.
6. Inspect overlapping sound/dialogue cases and repeated events.
7. Confirm cinematics and gameplay both participate.
8. Ask deaf/HoH playtesters to complete representative tasks.

Automation can assert that a cue was emitted and rendered; it cannot establish that its wording conveys enough information during stressful gameplay. Do not treat automatically generated `[sound]` labels as equivalent communication.

### B6. Grayscale and colour-vision simulations

Chrome documents rendering simulations and the programmatic Puppeteer `emulateVisionDeficiency` API. Microsoft Edge explicitly describes these tools as approximations. These are verified browser capabilities, not tools that directly inspect arbitrary native-engine rendering. [Chromium simulation](https://developer.chrome.com/docs/chromium/cvd), [Edge simulation limitations](https://learn.microsoft.com/en-us/microsoft-edge/devtools/accessibility/emulate-vision-deficiencies)

**Recommended screenshot workflow:**

- Capture representative actual gameplay/UI with fixed scene state and final compositing.
- Inspect ordinary color, grayscale, and appropriate protanopia/deuteranopia/tritanopia simulations.
- For native captures, use a separately validated image-processing tool or shader; identify its algorithm/version rather than inventing an equivalent browser capability.
- Check enemy/friendly identity, map paths, rarity, resource states, target highlights, puzzle states, charts, prompts, and enabled/disabled controls.
- Assert secondary encoding exists: shape, icon, label, texture, placement, or outline.
- Test the player's configurable palette in the same states.
- Review filtered screenshots with the original scene context and actual task.

A screenshot looking distinct to an agent does not prove a player can distinguish it quickly enough. No single global “colour-blind filter” replaces semantic non-colour encoding and relevant user testing.

### B7. Automated photosensitivity analysis

EA publishes IRIS to identify potential photosensitivity risks in video footage. Its repository documents per-frame luminance, red-flash, and pattern results, plus an example application that reads videos from `TestVideos` and writes analysis to `Results`. EA also publishes an Unreal Engine plugin. [IRIS repository](https://github.com/electronicarts/IRIS), [Unreal plugin](https://github.com/electronicarts/IRIS-Unreal-Plugin)

XAG 118 addresses photosensitivity in games. W3C's web guidance covers flash frequency, area, luminance, and saturated-red thresholds, and notes limitations involving viewing distance and magnification. Do not translate it into the false rule “three flashes per second is always safe.” [XAG 118](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/118), [WCAG 2.3.1 explanation](https://www.w3.org/WAI/WCAG22/Understanding/three-flashes-or-below-threshold.html)

**Recommended Contextual release/CI workflow:**

1. Identify effect producers: muzzle flashes, lightning, damage overlays, explosions, transitions, high-contrast scrolling patterns, UI alerts, and startup videos.
2. Include a Baseline startup capture even if the designer believes gameplay contains no flashing.
3. Capture the final composite at representative supported output settings and frame rates, recording capture metadata.
4. Capture normal and reduced-flash modes, worst-case effect combinations, repeated loops, and rapid player-triggered actions.
5. Use the supported analyzer build according to its actual README and license; pin tool/version/threshold configuration.
6. Parse results into a report with timestamps and effect/scene IDs.
7. Fail on analyzer errors and investigate every flagged region; “tool did not run” must fail verification coverage.
8. Fix the effect source or provide an effective safe presentation; rerun the identical capture and nearby scenarios.
9. Recheck compositing interactions. Two individually acceptable effects may coincide.
10. Retain the original capture, report, build ID, and resolution/tone-mapping details.

Do not invent a universal `iris --input --json` command; the repository's example application and build interfaces must be inspected for the pinned version. A generic wrapper can be added later once it has a reproducible integration.

**Limitations:** analysis of one recording covers that recording. Dropped/duplicated frames, changed playback rate, lossy capture, tone mapping, HDR conversion, UI scaling, random effects, and future content can change what is measured. Confirm the selected tool's supported color/output pipeline before claiming HDR coverage. An SDR capture analyzed successfully is not evidence that HDR output was validated.

Do not expose photosensitive playtesters to known dangerous sequences to confirm a machine finding. Fix and analyze content first; participants must be able to avoid exposure. A warning screen and a toggle reached after the effect are insufficient mitigations.

### B8. Contrast checks

XAG 102 specifies at least 4.5:1 for standard important text/visual elements, 3:1 for large elements, 3:1 for inactive text, and 7:1 for its high-contrast-mode guidance. It defines platform-dependent large-text sizes and calls for testing the lowest-contrast background region. These are XAG targets, not a blanket claim about every legal standard. [XAG 102](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/102)

**Recommended checks:**

- Validate theme token foreground/background pairs where the colors are known.
- Calculate contrast in the correct color representation; do not use simple RGB channel differences.
- For translucent caption panels, measure the final composited result over representative bright/dark/moving scenes.
- Inspect focus rings, selected items, warnings, placeholder text, disabled labels, glyphs, and important HUD states.
- Check the least favorable local background, not the average color of the screen.
- Distinguish source font color from antialiased edge pixels when designing automated measurement.
- Avoid using the name “high contrast” as proof that the numeric target is met.
- For world-object visibility, also assess silhouettes, occlusion, scale, texture, and background clutter; a single text ratio is not a complete 3D visibility metric.

EA's Fonttik is an open text-size/contrast analysis tool and can support captured-image/video checking. Its OCR and measured output need review; missing detection of a small or stylized label must not count as that label passing. [Fonttik](https://github.com/electronicarts/fonttik)

### B9. Text-size and layout checks at minimum resolution

XAG 101 measures rendered body height, not merely the engine font-size field. It lists minimum defaults of 26 px at 1080p for console and 18 px at 1080p for PC/VR, and recommends scaling to 200% without losing content or functionality; mobile guidance is DPI-dependent. [XAG 101](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/101)

**Recommended acceptance matrix:** minimum supported viewport, common desktop/TV size, handheld/mobile targets, different aspect ratios, supported safe areas, and both minimum/default and maximum text/caption scales.

For each:

- Capture the actual presented output, including any render-scale/upscale pipeline.
- Check all screens, tooltips, dynamic notifications, input glyphs, inventory descriptions, error dialogs, and caption overlays.
- Compare rendered glyph metrics/screenshots to the chosen platform target; a 26-point asset setting is not proof of 26 rendered pixels.
- Confirm readable wrapping and scrolling instead of clipping, ellipsis of critical meaning, or overlap.
- Verify controls remain reachable when content expands.
- Test pseudo-localized long strings, actual supported languages, CJK coverage, and right-to-left layout where applicable.
- Include the longest current remapped input prompt and dynamic numeric values.
- Check text over both busy and plain backgrounds.
- Test at realistic viewing distances with relevant players.

Declare the minimum resolution explicitly; do not quietly test only 4K. If no supported minimum is specified, the skill should identify that missing project constraint before asserting layout compliance.

### B10. Input remapping and adaptive-device checks

XAG 107 provides input-accessibility guidance, including alternatives to demanding input patterns. Steam Input's origin APIs support prompts that reflect the configured action mapping. [XAG 107](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/107), [Steam Input integration](https://partner.steamgames.com/doc/features/steam_controller/getting_started_for_devs)

Recommended automated and device-assisted tests:

- Rebind every supported gameplay and menu action to a nondefault control.
- Confirm both the executed action and all visible/narrated prompts update.
- Restart and verify bindings persist under the correct profile.
- Detect conflicts and verify the chosen swap/duplicate/unbind behavior.
- Test axis inversion, dead zones, sensitivity bounds, and digital alternatives where supported.
- Test remapping while hold/toggle/mash alternatives are active.
- Confirm gamepad, keyboard, pointer, and supported mixed-device use.
- Connect/disconnect/reconnect a device in menus and gameplay.
- Confirm focus remains recoverable and held actions reset appropriately.
- Verify actual Steam Input/emulated input paths in the shipping executable.
- Test the supported adaptive controller configuration with a player who uses it.
- Confirm no required menu action is excluded from the remapping surface without a usable alternative.

A synthetic key event test alone does not validate an adaptive controller, switch interface, or console accessory. Record the real device and configuration when making compatibility claims.

### B11. Manual testing with disabled players

AbleGamers Player Panels connects disabled players with game/user-research opportunities. Its public information describes research participation and compensation information; developers should confirm current service arrangements directly. [AbleGamers Player Panels](https://www.ablegamers.org/player-panels/)

Additional recruitment routes include local/online disability communities, gaming groups, and educational institutions, as suggested by Game Accessibility Guidelines. Use community-approved recruitment channels rather than unsolicited individual outreach. [Recruitment guidance](https://gameaccessibilityguidelines.com/include-some-people-with-impairments-amongst-play-testing-participants/)

**Recommended recruitment brief:**

- Specify game, platform, tasks, approximate session length, compensation, access needs, and location/remote options.
- Recruit for relevant barriers and assistive technology experience, not a single representative for “all disabilities.”
- Include intersecting needs, different skill levels, and players with fatigue/fluctuating access needs.
- Provide accessible consent, instructions, communication, and payment methods.
- Support breaks, shorter sessions, alternate response formats, and a no-penalty stop.
- Ask only information needed for the study; handle recordings and disability information with agreed privacy protections.
- Let participants use their usual devices where possible.
- Pilot the study and the remote/build setup before a paid session.
- Never make a photosensitivity/phobia study depend on exposure to known harmful content.

#### Practical session checklist

- [ ] Can the participant start the build and configure access before demanding content?
- [ ] Can they discover a useful option without being told its exact menu location?
- [ ] Do option names and descriptions match actual behavior?
- [ ] Can they preview safely, undo, reset selectively, and resume?
- [ ] Can they complete core tasks using the settings they choose?
- [ ] Are all important audio/visual cues available in a usable alternative channel?
- [ ] Do reading, attention, memory, and motor demands remain manageable over time?
- [ ] Does the game remain understandable with reduced sensory/guidance settings?
- [ ] Do failure recovery and skips preserve agency and continuity?
- [ ] Do save/load/relaunch and profile changes preserve access?
- [ ] Do supported assistive technologies work beyond the first screen?
- [ ] Is there any shaming, unwanted disclosure, reward penalty, or inaccessible content gate?
- [ ] Can the participant explain where they were blocked, what they expected, and what would help?

Record concrete barriers and task outcomes, not just satisfaction scores. Include settings, devices, task, point of failure, workaround, severity, evidence consent, and retest outcome. A single participant's success or preference should not be generalized to everyone with the same diagnosis.

### B12. Agent verification output and release gate

Proposed evidence record:

```text
CheckResult {
  checkId,
  optionIds,
  buildId,
  platform,
  locale,
  viewport,
  outputPipeline,
  deviceConfiguration,
  settingsSnapshot,
  method,
  result: pass | fail | blocked | not_tested | not_applicable,
  evidencePaths,
  limitations,
  followUpOwner
}
```

Recommended release gate:

1. All selected options have real consumers and truthful descriptions.
2. Persistence/migration/isolation tests pass.
3. Applicable progression and assist scenarios pass in the built game.
4. Startup/content captures have completed the selected safety analyses.
5. Text, contrast, captions, remapping, and focus checks cover declared targets.
6. Achievement/reward/content parity is verified.
7. Disabled-player findings are resolved or honestly documented with scope and owner.
8. Store accessibility claims match verified shipping behavior.

Steam's accessibility feature documentation and Xbox accessibility metadata both describe feature criteria. Do not infer a store tag from a setting's name or from an untested plan. [Steam accessibility features](https://partner.steamgames.com/doc/accessibility_features), [Xbox accessibility metadata](https://learn.microsoft.com/en-us/gaming/game-publishing/concepts/metadata-accessibility)

## Part C — Agent Skills format and cross-agent distribution

### C1. Standard, client behavior, and installer are different layers

Agent Skills is an open format originally developed by Anthropic. A skill packages instructions and optional resources in a directory; compatible clients discover and activate it on demand. The standard does not make all clients use the same installation directory, tool permissions, or invocation UI. [Agent Skills overview](https://agentskills.io/home)

Keep these layers separate:

1. **Portable content:** standard `SKILL.md`, references, scripts, assets.
2. **Discovery/invocation:** each host's directories, commands, consent, and settings.
3. **Distribution:** `npx skills`, manual copying, or optional host plugin packages.
4. **Project policy:** a small instruction-file pointer telling the agent when the project expects the skill to be used.

The following findings reflect the public documentation retrieved on the research date. Discovery paths and host extensions can change; the eventual README should state tested client versions rather than promise universal future compatibility.

### C2. Required frontmatter and exact portable limits

The standard requires YAML frontmatter followed by Markdown. Its field requirements are summarized below. [Agent Skills specification](https://agentskills.io/specification)

| Field | Required? | Portable constraint |
| --- | --- | --- |
| `name` | Yes | 1–64 characters; lowercase letters/numbers/hyphens; no leading/trailing or consecutive hyphens; match containing directory name |
| `description` | Yes | 1–1,024 characters; describes capability and when to use it |
| `license` | No | License name or bundled license-file reference |
| `compatibility` | No | 1–500 characters if supplied; actual environment requirements |
| `metadata` | No | Mapping of string keys to string values |
| `allowed-tools` | No | Experimental space-separated tool names; support varies |

Use ASCII lowercase/digits/hyphens for the broadest predictable naming compatibility. A valid directory name here would be `game-accessibility`.

Example **proposed** portable frontmatter, with a project-specific description rather than copied vendor wording:

```yaml
---
name: game-accessibility
description: >-
  Plans, implements, audits, and verifies optional game accessibility settings.
  Use when creating or changing game mechanics, controls, menus, audiovisual
  presentation, difficulty, captions, content filters, or accessibility support,
  including retrofit work. Routes by actual mechanics and engine; does not
  impose unrelated genre features or penalize assists.
metadata:
  author: "REPLACE_WITH_MAINTAINER"
  version: "0.1.0"
---
```

Add a real `license` after the owner chooses and includes one. Do not publish a placeholder license or claim third-party code is covered by the skill's license.

### C3. Directory conventions and progressive disclosure

The format's core is one skill folder with `SKILL.md`; conventional optional folders are `scripts/`, `references/`, and `assets/`. Discovery loads name/description, activation loads the main instructions, and execution reads resources as needed. [Agent Skills overview](https://agentskills.io/home)

| Location | Suitable content in this project | Loading expectation |
| --- | --- | --- |
| `SKILL.md` | Non-negotiable policy, inspection workflow, routing table, verification gate | Read in full when activated |
| `references/` | Engine-neutral details, feature families, platform caveats, engine adapters | Read only when the task's signals match |
| `scripts/` | Deterministic schema/content checks and validated analyzer adapters | Execute when needed; do not load all source into context |
| `assets/` | Settings schema, audit templates, check-result templates, sample fixtures | Read/copy only for the current operation |
| `agents/openai.yaml` | Optional Codex-specific appearance/policy metadata | Host-specific; not required by the open standard |

Keep `SKILL.md` under 500 lines and its instructions under roughly 5,000 tokens; these are recommendations, not a schema-enforced body-size ceiling. Use relative file links and avoid reference chains. The standard's reference validator is `skills-ref validate ./my-skill`. [Agent Skills specification](https://agentskills.io/specification)

Anthropic's best-practices guide also recommends a main body under 500 lines, specific third-person descriptions, concise instructions, and direct supporting-file references. Its frontmatter guidance adds no XML tags in name/description and cautions against reserved naming involving Anthropic/Claude. These are additional Anthropic guidance, not new universal Agent Skills fields. [Anthropic authoring best practices](https://platform.claude.com/docs/en/agents-and-tools/agent-skills/best-practices)

**Recommended size for this skill:** about 150–250 carefully edited lines in the main file. This is this report's target, not an official host limit. Keep the large research reports outside the installed main instructions.

#### Useful routing instructions

| Observed project signal | Read |
| --- | --- |
| New game or system | Core architecture and settings schema; relevant engine adapter |
| Existing game audit | Retrofit workflow, evidence template, then only implicated feature references |
| Spoken or meaningful audio | Captions/audio pipeline |
| Timed/repeated/precise input | Motor/input and timing modifiers |
| Combat/resource/failure barriers | Gameplay policy and progression recovery |
| Flicker, intense effects, moving camera | Photosensitivity/motion and capture analysis |
| Relevant feared/distressing content | Content tags, filter coverage, and safe previews |
| Waypoints/glows/paint or memory-heavy tasks | Guidance suppression and additional memory support |
| Menu/custom-rendered UI | Semantic UI, focus, narration, voice-operable controls |
| Platform inheritance or submission | Only target-platform references and current official docs |
| Export/release | Verification evidence and truthful store-feature criteria |

Never route solely by a genre label. A puzzle game can have combat, a shooter can have a reading-heavy inventory, and a turn-based game can still contain timed menus. The assignment's “FPS should never load JRPG guidance” should become “do not load mechanics that are absent,” not “refuse relevant guidance because the label differs.”

### C4. How descriptions trigger loading

OpenAI documents explicit skill invocation and implicit matching against the description, and advises front-loading key use cases because long catalogues can shorten descriptions. `SKILL.md` must include both `name` and `description`. [OpenAI skill documentation](https://developers.openai.com/codex/skills/)

**Recommended description design:** say what the skill does, name practical triggers, mention new-build and retrofit modes, and include its no-penalty scope. Do not put the only trigger instructions in the body: the agent may not read that body until after choosing the skill.

**Recommended activation tests:**

- Should trigger: “Add an accessibility menu to this game.”
- Should trigger: “Add a parry mechanic and QTE to the existing player controller.”
- Should trigger: “Audit whether captions and controls make the intro playable.”
- Should trigger with narrow routing: “Replace spider visuals when the content filter is enabled.”
- Should not load unrelated gameplay references: “Fix a typo in the game's legal notices.”
- Should not trigger as a general website accessibility skill: “Audit this marketing site's HTML form.”

Automatic matching is a host/model decision, not a guaranteed background watcher. A short project policy pointer can require the workflow for relevant changes. Do not promise that merely installing a skill audits every future commit.

### C5. Current host compatibility

| Host | Verified project discovery locations relevant here | Invocation / important distinction | Primary source |
| --- | --- | --- | --- |
| Claude Code | `.claude/skills/<name>/SKILL.md`; personal `~/.claude/skills/` | `/skill-name`; context-based automatic use; host accepts optional metadata beyond the standard | [Claude Code skills](https://code.claude.com/docs/en/skills) |
| OpenAI Codex | `.agents/skills/` from working directory through repository ancestors; personal `~/.agents/skills/` | `$skill-name` or skill selector; implicit description matching | [Codex skills](https://developers.openai.com/codex/skills/) |
| Cursor | `.agents/skills/`, `.cursor/skills/`; documented compatibility locations include `.claude/skills/` and `.codex/skills/` | `/` discovery; context-based activation; project placement travels with repository | [Cursor skills](https://cursor.com/docs/skills) |
| GitHub Copilot | `.github/skills/`, `.claude/skills/`, `.agents/skills/`; personal `~/.copilot/skills/` or `~/.agents/skills/` | Supported across documented Copilot agent surfaces; instruction support still varies by surface | [Copilot skills](https://docs.github.com/en/copilot/concepts/agents/about-agent-skills) |
| Gemini CLI | `.gemini/skills/` or `.agents/skills/`; personal counterparts | Description discovery, `activate_skill`, and documented user consent; same-tier `.agents` alias takes precedence | [Gemini CLI skills](https://geminicli.com/docs/cli/skills/) |

#### Claude Code: portable standard versus current host permissiveness

Current Claude Code docs say frontmatter fields are optional, with `description` recommended and a directory-derived name fallback. That is host behavior, not permission to omit the standard's required fields. Ship both `name` and `description`.

Host extensions include invocation control, tool controls, subagent/context behavior, and dynamic content features. Keep those out of the portable core unless a separate adapter genuinely needs them. Standard-only packaging/upload paths can reject extra fields that Claude Code itself accepts. [Claude Code frontmatter and cross-product field guidance](https://code.claude.com/docs/en/skills)

#### Codex metadata

Codex supports symlinked skill folders. Optional `agents/openai.yaml` can describe appearance, dependencies, and implicit-invocation policy. Do not copy a policy disabling implicit invocation if the desired workflow is automatic matching. These are Codex integration features, not cross-agent guarantees. [Codex metadata and discovery](https://developers.openai.com/codex/skills/)

#### Cursor packaging

Directly discovered project skill directories and Cursor's in-product repository/plugin installation flow are distinct. Current Cursor docs describe a plugin marketplace package for its UI repository import. The plain local `SKILL.md` directory remains usable; do not claim a bare repo automatically appears in that UI marketplace. [Cursor skills and repository installation](https://cursor.com/docs/skills)

#### Gemini consent

Gemini's documented activation flow asks for consent before adding a skill's body and resource access. A README or `AGENTS.md` pointer must not promise to bypass that host control. [Gemini activation lifecycle](https://geminicli.com/docs/cli/skills/)

### C6. One repository for all five agents

**Recommended distribution:** one canonical `skills/game-accessibility/` directory. Let installers place it in supported discovery locations. Do not maintain five divergent copies of the actual guidance.

For a project using all five hosts, `.agents/skills/` can serve the four clients documented above as accepting it, with an appropriate Claude Code installation under `.claude/skills/`. Use installer-managed links or copies; test Windows link behavior and avoid exposing duplicate same-name versions in one client's catalogue.

#### Project instruction pointers

A short `AGENTS.md` policy can tell agents when to use the installed skill and preserve the project's no-penalty rule. This is project policy, not part of the Agent Skills format.

Proposed pointer text:

```markdown
For changes to game mechanics, controls, menus, audiovisual presentation,
or difficulty, use the installed game-accessibility skill. Inspect the
project first and load only references relevant to its actual mechanics
and engine. Accessibility options are optional and must not penalize
achievements, rewards, content access, or save files. Report verification
evidence and anything not tested.
```

Use these documented host mechanisms rather than assuming `AGENTS.md` loads identically everywhere:

| Host | Pointer strategy | Source |
| --- | --- | --- |
| Codex | Root/nested project `AGENTS.md`; skill itself in its documented discovery path | [Codex project instruction guide](https://developers.openai.com/codex/guides/agents-md/) |
| Claude Code | `CLAUDE.md` with `@AGENTS.md` import provides a documented compatibility route; current versions can also read `AGENTS.md` directly under documented conditions | [Claude memory/import guidance](https://code.claude.com/docs/en/memory) |
| Cursor | `AGENTS.md` project instructions, with skill discovered separately | [Cursor rules and AGENTS.md](https://cursor.com/docs/rules) |
| Copilot | `AGENTS.md` where supported; `.github/copilot-instructions.md` as a small project-wide pointer for relevant supported surfaces | [Instruction support matrix](https://docs.github.com/en/copilot/reference/custom-instructions-support) |
| Gemini CLI | Small `GEMINI.md` pointer/import, or configure `context.fileName` to include `AGENTS.md` | [Gemini context files](https://geminicli.com/docs/cli/gemini-md/) |

Claude's documented import syntax can use a line such as `@AGENTS.md`. Its current memory docs describe direct `AGENTS.md` reading from v2.1.277, normally when no applicable `CLAUDE.md`/`CLAUDE.local.md` takes precedence; unavailable sessions still need the import. Gemini documents configurable context filenames; do not silently rewrite a user's global configuration just to install this skill. [Claude imports and AGENTS.md support](https://code.claude.com/docs/en/memory), [Gemini filename configuration](https://geminicli.com/docs/cli/gemini-md/)

Keep pointers short. Do not import the entire reference library into every conversation; that defeats progressive disclosure. A pointer file also does not replace installation/discovery of a skill.

### C7. Multi-skill repositories, `npx skills`, and skills.sh

Vercel's installer supports repository/URL/local sources, selecting named skills and agents, project/global scope, and link/copy installation. Its discovery includes root `SKILL.md`, `skills/`, curated/system containers, and agent-specific skill roots. A shallower skill can shadow nested ones. [vercel-labs/skills](https://github.com/vercel-labs/skills)

**Recommended structure:** even if the repository initially publishes one skill, use `skills/game-accessibility/SKILL.md`. Add sibling skill directories only when they have genuinely independent triggers and workflows. Feature-specific references do not need to become separate skills.

Do not put a catalog-level `SKILL.md` at the root and then expect every installer to discover nested skills identically. Keep repository README/contribution guidance at the top and installable content in explicit skill folders.

skills.sh is a discovery directory for the ecosystem; `npx skills` is the installation CLI. Neither is the Agent Skills specification itself, and directory popularity is not an accessibility-quality certification. [Vercel ecosystem announcement](https://vercel.com/changelog/introducing-skills-the-open-agent-skills-ecosystem)

The installer can execute through `npx`, which means users must review the tool and downloaded skill/scripts. A manual directory-copy route is valuable for users who do not want that installation dependency. Explain what files are added and how updates are reviewed.

### C8. Public repositories worth studying

These are widely visible first-party/community examples, not endorsements that every contained skill fits this project. Popularity numbers change, so no star-count ranking is claimed.

| Repository | Verified structure / useful lesson | Caution |
| --- | --- | --- |
| [anthropics/skills](https://github.com/anthropics/skills) | Separate skill folders under `skills/`, a template, and optional plugin packaging; inspect [mcp-builder](https://github.com/anthropics/skills/tree/main/skills/mcp-builder) for task instructions plus supporting material | The README distinguishes open-source examples from source-available document skills; check each license before copying |
| [openai/skills](https://github.com/openai/skills) | Official Codex skill catalogue; curated/system organization and the [skill-creator](https://github.com/openai/skills/tree/main/skills/.system/skill-creator) illustrate separation of main instructions and optional resources | OpenAI-specific integration metadata should not be mistaken for the portable standard |
| [vercel-labs/agent-skills](https://github.com/vercel-labs/agent-skills) | Multi-skill `skills/` layout, installation guidance, scripts, and root `AGENTS.md`/`CLAUDE.md` | Framework-specific guidance should remain in routed references in this engine-neutral project |
| [github/awesome-copilot](https://github.com/github/awesome-copilot) | A broad catalogue that visibly distinguishes skills from agents/instructions and other customization types | An agent definition or instructions file is not automatically a portable skill |
| [vercel-labs/skills](https://github.com/vercel-labs/skills) | Installer behavior and supported-agent mapping are a useful distribution reference | It is chiefly the installer/tool repository, not a substitute for the content specification |

Borrow structural ideas rather than copying large bodies of vendor-specific instructions. Keep original source attribution and license notices for any reused material.

### C9. Test the skill itself

The Agent Skills authoring guidance recommends learning from real execution, checking traces, using conditional references, and keeping procedures focused. [Agent Skills creator best practices](https://agentskills.io/skill-creation/best-practices)

**Recommended skill acceptance checks:**

- Validate YAML and standard name/description limits.
- Confirm the folder basename matches `name`.
- Check every relative link from the installed skill root.
- Check the installed bundle includes scripts, schemas, fixtures, and referenced engine notes.
- Ensure no required reference points outside the installable folder into the research workspace.
- Count main-file lines and tokens; preserve a compact routing workflow.
- Test explicit invocation in all five intended hosts.
- Test relevant implicit triggers and plausible false positives.
- Inspect traces to confirm an FPS-like task does not load unrelated turn-based mechanics.
- Test mixed-mechanic examples so routing does not miss relevant accessibility needs.
- Exercise both greenfield and retrofit modes.
- Test a small game with no combat/resources/QTEs: the skill must not add them.
- Test a custom engine with no prewritten adapter: the agent should inspect capabilities and adapt, not switch engines.
- Test missing analyzer/SDK/AT dependencies: the report must say blocked/not tested.
- Verify no generated assist badge, achievement gate, or save mark appears.
- Test manual-copy and installer routes on Windows as well as a Unix-like environment.
- Confirm host permission/consent flows remain intact.
- Verify optional scripts are deterministic, bounded, and do not download/install packages without the project's authorized workflow.

An instruction-only first release can still be useful. Add scripts only for repeatable checks that have been exercised on real projects; do not distribute speculative multi-engine automation as proven verification.

## Remaining public-evidence limitations

- **UNVERIFIED:** universal PlayStation TRC and Nintendo lotcheck rules about difficulty-based achievements, accessibility penalties, or the complete requested system-preference APIs.
- **UNVERIFIED:** one general public API on every listed platform for Reduce Motion, contrast, text size, captions, and screen narration. The capability table deliberately lists narrower verified functions.
- **UNVERIFIED:** the internal settings-store/modifier/content-tag architecture of the cited commercial games.
- **UNVERIFIED:** a shipped game implementing the exact proposed unified guidance dim/off/boost group.
- **UNVERIFIED:** blanket AutoHotkey/mod/overlay compatibility under all anti-cheat rules.
- No installation command, analyzer, game scenario, or cross-agent compatibility test was executed for this research. The report supplies verified documentation and proposed verification, not completed certification.

## Implications for the skill

### Recommended repository layout

This is a proposed layout. Only the main skill directory is installed; research and repository maintenance material stay outside it.

```text
README.md
LICENSE
AGENTS.md
CLAUDE.md                         # short shared-policy pointer
GEMINI.md                         # short shared-policy pointer
.github/copilot-instructions.md   # short pointer, if maintained
research/                         # evidence, not automatically loaded
tests/skill-evals/                # activation/routing and output fixtures
skills/
  game-accessibility/
    SKILL.md                      # compact workflow and direct routing links
    references/
      architecture.md
      retrofit.md
      verification.md
      captions-audio.md
      input-motor.md
      gameplay-modifiers.md
      guidance-memory.md
      sensory-safety.md
      content-filters.md
      semantic-ui.md
      platform-policies.md
      engines/
        godot-redot.md
        unity.md
        unreal.md
        web.md
    assets/
      settings-definition.schema.json
      settings-profile.example.json
      audit-report-template.md
      verification-result.schema.json
    scripts/                      # only validated, optional checks
    agents/openai.yaml            # optional host metadata
```

Directly link each routed reference from `SKILL.md`, including engine-specific files; avoid a chain of “read this index to find the next index.” Keep samples within the installed directory and adapt them to the actual engine/version.

### Recommended settings-schema shape

```text
Definitions {
  schemaVersion,
  options: [{
    id, type, default, rangeOrEnum,
    labelKey, descriptionKey, scope, applyPolicy,
    platformPreference?, contextSignals,
    consumers, verificationIds
  }],
  presets: [{ id, version, labelKey, changes }]
}

ProfileChoices {
  schemaVersion,
  choices: { stableOptionId: { mode: explicit | system, value? } }
}
```

Keep definition metadata, player choices, resolved values, and story state distinct. No diagnosis field, permanent assist-use flag, disqualification flag, or save badge. Use the existing project's owners and persistence primitives rather than creating a framework by default.

### Verification checklist to encode

- [ ] Inspect mechanics, presentation, engine version, existing settings/input/save architecture.
- [ ] Route by actual signals; mark absent mechanics not applicable.
- [ ] Make first launch, title, pause, previews, and reset usable.
- [ ] Validate persistence, migration, scope isolation, and supported OS inheritance.
- [ ] Route gameplay decisions through their owners; verify assists and critical combinations.
- [ ] Complete applicable real progression routes, including recovery and skips.
- [ ] Preserve achievement/reward/content parity and unmarked saves.
- [ ] Check captions in muted play, text at minimum resolution, contrast, remapped prompts, and semantic focus.
- [ ] Analyze startup and relevant gameplay captures for photosensitivity with a verified tool/pipeline.
- [ ] Check filtered assets/audio across gameplay, UI, cinematics, and newly loaded content.
- [ ] Include disabled playtesters early and retest concrete findings.
- [ ] Report evidence, scope, and blocked/not-tested items; make no universal safety/accessibility claim.

### README installation instructions to give

Replace `OWNER/REPO` with the actual published repository. The following command forms follow the installer's documented source/skill/agent selectors; these are instructions for the future README, not commands executed during research. [Installer options](https://github.com/vercel-labs/skills)

```sh
# Inspect the repository's available skills.
npx skills add OWNER/REPO --list

# Install the one skill into this game project for the selected agents.
npx skills add OWNER/REPO --skill game-accessibility --agent claude-code codex cursor github-copilot gemini-cli

# If links are unsuitable, use the documented copy mode.
npx skills add OWNER/REPO --skill game-accessibility --agent claude-code codex cursor github-copilot gemini-cli --copy

# Inspect installed project skills.
npx skills list
```

State that `npx` requires an available Node.js/npm toolchain and repository access; Git-based source retrieval also needs its supported Git setup. Offer a manual alternative: copy the entire `skills/game-accessibility/` folder to `.agents/skills/game-accessibility/` for the documented compatible hosts and to `.claude/skills/game-accessibility/` for Claude Code. Preserve all bundled relative paths.

For global installation, explain the installer's `--global` option and the selected hosts' actual user directories. Prefer project installation for a team-shared, reviewed version. Do not imply global user skills automatically travel to every remote/cloud agent.

After installation, show how to confirm discovery in the user's host and explicitly request the skill once. For example, Claude Code uses `/game-accessibility`; Codex supports `$game-accessibility`. Give other clients' current documented selection flow instead of promising identical slash commands everywhere.

Tell users to add a short project policy pointer if they want relevant game changes to invoke the workflow consistently. Explain that installation adds guidance, not an accessibility runtime, engine plugin, legal certification, or automatic safety scan. Document dependencies only for optional checks actually supplied, update/review policy, supported/tested client versions, and how to report missing or incorrect guidance.


