# Mods, hardware and platforms

**Load when:** B01 first launch and persistence (every game), S002 settings before gameplay,
S156 mods or custom hardware supported, S029 keyboard text entry, S128 shared device or
controller handoff, or the game runs in a desktop window, supports several input devices or reads
OS accessibility settings ([signal-routing.md](../signal-routing.md)). **Skip when:** never skip
the Baseline rows (L01, L02, L16-L18). Skip the mod section when no mod support is planned.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| L01 | Accessibility setup before first content | — | On | B | All |
| L02 | Persist accessibility preferences | — | On | B | All |
| L03 | Named settings profiles | `game.profile.active` | Intended | C | All |
| L04 | Read platform preferences | every key with `platform_pref` | Platform pref | C | V,H,N,M |
| L05 | Multiple supported input devices | — | On | C | M,S |
| L06 | Simultaneous mixed-device input | — | Intended | C | M |
| L07 | Controller assist / combined controllers (copilot) | `game.input.copilot` | Intended | C | M |
| L08 | Windowed / borderless modes | `game.display.window_mode` | Intended | C | M,V |
| L09 | System virtual-keyboard compatibility | — | On | C | M,V |
| L10 | Switch scanning input | `game.input.switch_scan` | Intended | A | M |
| L11 | Gaze / dwell input | `game.input.dwell_time` | Intended | A | M |
| L12 | Portrait / landscape choice | `game.display.orientation` | Platform pref | C | M,S |
| L13 | External / mobile controllers | — | Intended | C | M |
| L14 | Accessibility mod hooks | — | Intended | A | All |
| L15 | Mod / config management and recovery | — | Intended | A | All |
| L16 | In-game accessibility feature guide | — | On | B | All |
| L17 | Public accessibility documentation | — | On | B | All |
| L18 | Accessible support channels | — | On | B | All |
| L19 | Stable frame rate / refresh choices | `game.display.vsync` | Safe: VSync or frame cap on | C | O,V,S |
| L20 | Pause on focus loss | `system.pause_on_focus_loss` | On | C | M,S,T |

(Tier: B = Baseline, C = Contextual, A = Advanced. Helps: V vision, H hearing, M motor,
C cognitive/memory, N neurodivergent/sensory, P photosensitivity, F phobia, O motion sickness,
T stress/mental health, S situational.)

L01-L02 are built in [architecture.md](../architecture.md) sections 1 and 10. L16-L18 public
claims are in [store-claims-and-compliance](store-claims-and-compliance.md). Advanced rows
(L10, L11, L14, L15) need an explicit scope decision in `ACCESSIBILITY.md`.

## How to implement

### 1. One action layer for every device (L05-L07, L13)

Pipeline: hardware events → action layer and remapping → InputAssist → game commands
([architecture.md](../architecture.md) section 3). Adaptive controllers, Steam Input, remapping
software, voice tools, switch interfaces and test automation all enter through the same actions
([XAG 107](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/107),
[Steam Input for developers](https://partner.steamgames.com/doc/features/steam_controller/getting_started_for_devs)).

- **Mixed sources:** for a digital action, combine held states without duplicate press events,
  and release only when every source releases. For analog conflicts, pick a clear rule (strongest
  magnitude or last active source); never silently sum two sticks.
- **Never reject emulated input** just because it is emulated, never hard-code a controller
  brand, never require two devices at once.
- **Prompts come from the current binding**, and update when the device or binding changes. Do
  not let prompts flicker when two devices are active (L06).
- **Hot-plug:** connecting or losing a device keeps focus, keeps bindings and clears held latches.
- **Remapping recovery:** a reset path outside the game (config key or launch flag) for a binding
  set that became unusable.
- Do not block Steam Input, AutoHotkey-style remappers or assistive overlays as a blanket policy.
  Universal compatibility with every anti-cheat is **UNVERIFIED**; for competitive games, agree
  and publish the supported paths with the security owner, and keep native remapping.

| Technology | What exists | What the game must do | Caveat |
|---|---|---|---|
| [Steam Input](https://partner.steamgames.com/doc/features/steam_controller) | Action-based mappings, action origins and glyphs, [legacy keyboard/mouse/gamepad emulation](https://partner.steamgames.com/doc/features/steam_controller/legacy_mode) | Logical actions; query real origins for prompts; test custom configurations | Steamworks SDK terms apply |
| [SDL Gamepad](https://wiki.libsdl.org/SDL3/CategoryGamepad) | Standard controller mapping, hot-plug | Respect mappings, axes and multiple sources | SDL2 `GameController` became SDL3 `Gamepad` |
| [Xbox Adaptive Controller](https://learn.microsoft.com/en-us/shows/one-dev-minute/what-is-the-xbox-adaptive-controller) | Adaptive hardware that presents as a controller | No mandatory chords, fixed timings or unremappable analog gestures | No special "disabled player" path should be needed |
| PlayStation Access controller | Configurable adaptive controller | Same as above; reduce required actions and simultaneous inputs | A [player review](https://www.gameaccessibilitynexus.com/blog/2024/04/24/playstation-access-controller-review/) notes two sticks and profile switching can stay barriers. Official spec not in this research: UNVERIFIED |
| [Xbox Copilot](https://support.xbox.com/en-US/help/account-profile/accessibility/copilot) | Platform feature merging two controllers | Never assume one person or one device sends every action | Current PC/console limits UNVERIFIED |
| In-game copilot (L07) | Game-level shared control, e.g. [Horizon Forbidden West](https://blog.playstation.com/2022/02/10/accessibility-features-in-horizon-forbidden-west/) | Define disconnect, prompts and conflicting input; say whether it is yours or the platform's | Another person's help must never be required for an advertised solo setup |
| [Apple Switch Control](https://support.apple.com/en-ie/119835) (L10) | Item, manual and auto scanning | Expose navigable UI items, or build a deliberate in-game scan mode | Canvas pixels are not accessible items; test the exported UI |
| [Tobii Game Integration](https://developer.tobii.com/pc-gaming/downloads/) (L11) | Gaze and head input SDK | Optional gaze targeting, adjustable dwell, explicit activation, calibration and recovery, physical-input alternative | Proprietary licence. The [terms](https://developer.tobii.com/vr/sdla/) separate interactive use from recording or analysing gaze: never add gaze analytics |

**Switch scanning (L10):** an explicit ordered set of selectable items, adjustable interval,
pause, back and optional manual stepping. Remove or extend time limits rather than making a
switch user race a scan cycle. **Dwell (L11):** configurable delay, visible progress, cancel when
the gaze leaves, no repeat activation, no selection while reading.

### 2. The config file is a public API (L02, L15)

Players, modders and assistive tools edit it ([core-policy.md](../core-policy.md) section 8).

- Human-readable, versioned (`schema_version`), stable keys from
  [settings-registry.json](../../assets/settings-registry.json). Bindings live in the same or a
  sibling file.
- Same validation as the menus. Repair one invalid value; never reset the whole file.
- Write to a temp file, then rename. Keep it **outside story saves**.
- Migrate by meaning when a key changes. Record which keys were set explicitly (needed for L04).
- Document in the README: path per platform, every key, its range, which keys apply live and
  which need a reload, and the reset/recovery route.
- Keep personal accessibility config separate from mod code and from server-authoritative rules.
- Profiles (L03) are named by the player, never after a diagnosis. Local co-op needs per-player
  profiles for personal settings; device keys (master volume, brightness, mono) stay per machine.

### 3. OS accessibility preferences (L04)

Each inheritable key offers `System` and `Custom`. **Unknown is not false.** Read at startup and on
change notifications. Never overwrite an explicit player choice after an OS event.

**Precedence:** game fallback (safe default) → system value, when the choice is `System` →
explicit player value → temporary preview. Do not stack two text multipliers. Show the resolved
value and where it came from in settings.

| Target | Public signals | Boundary |
|---|---|---|
| Windows | [`UISettings`](https://learn.microsoft.com/en-us/uwp/api/windows.ui.viewmanagement.uisettings) (animations, text scale); [`AccessibilitySettings`](https://learn.microsoft.com/en-us/uwp/api/windows.ui.viewmanagement.accessibilitysettings) (high contrast); [`ClosedCaptionProperties`](https://learn.microsoft.com/en-us/uwp/api/windows.media.closedcaptioning.closedcaptionproperties) (caption style) | Custom game UI must apply them itself. Caption style is not transcription. |
| macOS | [`NSWorkspace`](https://developer.apple.com/documentation/appkit/nsworkspace/accessibilitydisplayshouldreducemotion) reduce motion, increase contrast, change notifications | A general macOS game text-size API: UNVERIFIED. |
| iOS / iPadOS | [`UIAccessibility.isReduceMotionEnabled`](https://developer.apple.com/documentation/uikit/uiaccessibility/isreducemotionenabled), [`isDarkerSystemColorsEnabled`](https://developer.apple.com/documentation/uikit/uiaccessibility/isdarkersystemcolorsenabled), [content-size categories](https://developer.apple.com/documentation/uikit/uicontentsizecategory) | Dynamic Type is a text preference, not a scale for every HUD dimension. |
| Apple captions | Media Accessibility [captioning characteristics](https://developer.apple.com/documentation/mediaaccessibility/macaptionappearancecopypreferredcaptioningmediacharacteristics(_:)), [relative character size](https://developer.apple.com/documentation/mediaaccessibility/macaptionappearancegetrelativecharactersize(_:_:)) | Check availability per deployment target. |
| Android | [`CaptioningManager`](https://developer.android.com/reference/android/view/accessibility/CaptioningManager) (enabled, locale, scale, style); [`AccessibilityManager`](https://developer.android.com/reference/android/view/accessibility/AccessibilityManager) (`isHighContrastTextEnabled()` API 36+, `getRecommendedTimeoutMillis()` API 29+); [`ValueAnimator.areAnimatorsEnabled()`](https://developer.android.com/reference/android/animation/ValueAnimator#areAnimatorsEnabled()) | Animator state is only a conservative motion hint. Do not branch general UI on `AccessibilityManager.isEnabled()`. |
| Xbox / GDK | [`XAccessibility`](https://learn.microsoft.com/en-us/gaming/gdk/docs/reference/system/xaccessibility/xaccessibility_members): `XClosedCaptionGetProperties`, `XHighContrastGetMode`, speech-to-text overlay | Verify targets in the licensed SDK. No public reduce-motion or text-size API confirmed. |
| PlayStation | [System settings](https://www.playstation.com/en-gb/support/hardware/ps5-accessibility-settings/): screen reader, captions, controller | A public game API for these: UNVERIFIED. Use authorised SDK docs. |
| Web | [`prefers-reduced-motion`](https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/At-rules/@media/prefers-reduced-motion) | Mirror it in canvas/WebGL code, not only CSS. |

Engine wrappers may or may not expose these. Check the installed engine version (see the engine
guide) before calling any of them.

### 4. Desktop and device behaviour (L08, L09, L12, L19, L20)

- **Pause on focus loss (L20):** in single-player, pause simulation and presentation when the
  window loses focus, and resume to the pause menu, not straight into action. Clear held toggles
  and latches ([architecture.md](../architecture.md) section 3). Online play cannot pause the
  session: say so in the option description. Never steal focus back.
- **Windowed / borderless (L08):** players use on-screen keyboards, magnifiers and other overlays
  beside the game ([GAG](https://gameaccessibilityguidelines.com/if-producing-a-pc-game-support-windowed-mode-for-compatibility-with-overlaid-virtual-keyboards/)).
  The UI reflows at any window size. Do not hold an exclusive mouse capture in menus or pause.
  Changing window mode changes only that setting.
- **Virtual keyboard (L09):** use the engine or platform text-input path so system and on-screen
  keyboards, IME composition and dictation work. Focus, submit and cancel behave correctly.
- **Orientation (L12):** support both where the device may be mounted; rotation is never required
  to proceed.
- **Frame pacing (L19):** VSync or a frame cap is on by default (core-policy section 3). A stable
  frame rate is not photosensitivity certification.

### 5. Mod hooks (L14, L15)

Mods extend access; they never replace native Baseline access. Recommended hooks, inferred from
the gap between supported mod systems and fragile patch workflows:

1. Read-only, stable access to player, world, objective, inventory, menu and interaction state.
2. Semantic IDs and localised strings, not only textures or text drawn into meshes.
3. Events for focus, selection, state changes, audio cues and world interactions.
4. A supported action-dispatch interface so alternate devices invoke the same commands.
5. UI extension points and navigation control with documented ownership and lifecycle.
6. Spatial queries and interaction lists that need no screen-coordinate OCR.
7. Settings registration, persistence and reset, with no save stigma.
8. A versioned API and schema with a compatibility policy for accessibility extensions.
9. Accessibility mods considered when changing anti-cheat, packaging or mod loading.
10. Safe failure when an extension is missing, and a route to settings and recovery without it.

Also: content-variant tags for filter mods ([architecture.md](../architecture.md) section 7),
caption events from CaptionBus, and an easy disable and rollback for any mod (L15). Never require
an inaccessible mod installation to unlock essential play.

## Numbers that matter

- Android: `isHighContrastTextEnabled()` is public from API 36; `getRecommendedTimeoutMillis()`
  from API 29 ([AccessibilityManager](https://developer.android.com/reference/android/view/accessibility/AccessibilityManager)).
  Gate by API level.

## Shipped examples

Accessibility mods show what hooks enable. They also show why mods never replace native access.

- [Stardew Access](https://github.com/stardew-access/stardew-access): menu and dialogue narration,
  tile reading, object tracking, keyboard-only play, built on SMAPI. GPL-3.0 (the current repo).
- [Say the Spire](https://github.com/bradjrenshaw/say-the-spire): screen-reader support for Slay the
  Spire; needs [ModTheSpire and BaseMod](https://bradjrenshaw.github.io/say-the-spire/devs/compilation.html).
  Licence UNVERIFIED.
- [Hearthstone Access](https://www.hearthstoneaccess.com/): keyboard and screen-reader play through
  an approved patch workflow, not a public API. [Patch day](https://hearthstoneaccess.github.io/under-the-hood-patch-day.html)
  shows that game updates can force rebuilding patches.
- [Factorio Access](https://github.com/Factorio-Access/FactorioAccess): narrated interaction,
  navigation and tutorial through Lua mod integration plus a speech launcher. Its README warns of
  multiplayer limits and unsupported Space Age content.
- [Minecraft Access](https://github.com/minecraft-access/minecraft-access): narration and nonvisual
  gameplay information on Fabric/NeoForge, with its own [add-on API](https://docs.mcaccess.org/api/client/javadoc/index.html).
  GPL-3.0; the Minecraft, loader and mod versions must match.
- [EyeMine](https://github.com/SpecialEffect/EyeMine): open-source gaze access for supported
  Minecraft setups. Not a universal mod API.
- God of War Ragnarök detects the PlayStation mono-audio system setting.
  [PlayStation](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/). One
  integration, not a general SDK promise.

## Verify

- [ ] Rebind every gameplay and menu action to a non-default control; actions and all visible and
      narrated prompts update; bindings persist after restart.
- [ ] Unbind "confirm": the documented recovery route restores a usable layout.
- [ ] Keyboard plus controller at once: no duplicate presses, no prompt flicker, holds release only
      when every source releases.
- [ ] Unplug and replug a controller in a menu and mid-action: focus stays, latches clear, bindings
      stay.
- [ ] Steam Input (or other emulation) in the shipping executable drives menus and gameplay.
- [ ] Adaptive controller claims are tested with the real device and recorded with its
      configuration. A synthetic key event is not proof.
- [ ] Edit the config file by hand: valid edits apply; one invalid value is repaired and the rest
      kept; the README documents every key.
- [ ] With no explicit choice, toggle the OS reduce-motion or caption setting: the game follows.
      After the player sets the key, the OS change no longer overrides it.
- [ ] Alt-tab in single-player: the game pauses to a menu and held toggles are released.
- [ ] Windowed mode: an on-screen keyboard and screen reader work beside the game; the UI reflows.
- [ ] Text fields accept the system or on-screen keyboard and IME composition.
- [ ] With every mod disabled, Baseline options still work; a missing mod fails safely.

## Common mistakes

- Gameplay reads physical keys → read logical actions only.
- Settings stored inside the story save → separate file; loading a save never restores old settings.
- OS preference applied on every launch → only while the player has not set that key.
- Treating "screen reader running" as a reason to change the whole UI → expose accessible actions
  through the shared path.
- "Mods will fix it" → ship native Baseline access; hooks are extra.
- Blocking remappers or overlays without a reason → state the reason or allow them.
- Pausing on focus loss in online play and desyncing → describe the online limit instead.
- Exclusive fullscreen only → offer windowed or borderless for overlays.
- Gaze telemetry added to eye-tracking support → interactive use only; check the Tobii licence.
- Calling the PlayStation Access controller or Xbox Adaptive Controller "supported" because
  controllers work → test chords, timings and two-stick demands with the real device.
