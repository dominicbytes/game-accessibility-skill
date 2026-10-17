# Other engines and runtimes

> **Status:** research-based guidance. Nothing in this file was run on these engines here. The only
> exception is the JavaScript reference below, which was exercised in Node.js with stubbed
> `localStorage`, `matchMedia` and DOM objects, not in a browser with a screen reader. Facts that
> could not be confirmed are marked UNVERIFIED. Re-check every API against the version you ship.

Every section maps the engine onto the [architecture contract](../../references/architecture.md)
with the shared keys from `assets/settings-registry.json`, under
[core-policy.md](../../references/core-policy.md). Each lists what is **built in**, what you **must
build**, **recommended code** with its licence, and **version gates**. Copying third-party code
brings its licence into your project. GPL and LGPL code needs a deliberate decision.

## Web / plain canvas (and WebGL)

- **Built in:** the DOM, ARIA, `matchMedia` for `prefers-reduced-motion` and `prefers-contrast`, the
  optional Web Speech API (`speechSynthesis`), and `localStorage`.
- **Must build:** pixels on a canvas are invisible to screen readers. Mirror menus and meaningful
  state as **native DOM controls** (`<button>`, `<input type="range">`, `<label>`, `<fieldset>`)
  layered over or beside the canvas, and keep keyboard focus in step with the visual focus. Add a
  persistent `aria-live="polite"` region that exists **before** the first announcement, with a bounded
  queue. Use assertive only for urgent updates. Never hide accessible content with `display:none` or
  `aria-hidden="true"`. Don't put `role="application"` on the whole game as a blanket fix.
- **CSS preferences do not reach canvas or WebGL.** A `@media (prefers-reduced-motion)` rule changes
  DOM animation only. Your JS tweens, camera shake, parallax and shaders must read the store. Listen
  for `change` events, because players toggle OS settings mid-session. Treat `prefers-contrast: more`
  as the initial value for `visual.high_contrast`, and keep the in-game toggle as well.
- **Web Speech is optional narration.** It needs feature detection, waiting for voices, and often a
  user gesture before it speaks. It never replaces real DOM semantics.
- **Test** with forced colours (Windows High Contrast) and with at least one browser and reader pair.
  Behaviour differs between pairs.

Minimal reference (original code, MIT; Node-tested with stubs only). It covers the settings store
(all five registry types, validation, clamp, per-key repair, explicit-set tracking), the live-region
narrator and the reduced-motion observer. It has no FlashGuard; implement the rule in the table at
the end:

```js
// SPDX-License-Identifier: MIT
export function createA11y(registry, storageKey = "accessibility") {
  const defs = new Map(registry.settings.map((d) => [d.id, d]));
  const listeners = new Set();
  let saved = {};
  try { saved = JSON.parse(localStorage.getItem(storageKey)) || {}; } catch { saved = {}; }
  const explicit = new Set((saved.explicit || []).filter((id) => defs.has(id)));
  const legal = (d, v) => d.type === "bool" ? typeof v === "boolean" : d.type === "string" ? typeof v === "string"
    : d.type === "enum" ? d.values.includes(v) : typeof v === "number" && Number.isFinite(v);
  const clamp = (d, v) => d.type !== "float" && d.type !== "int" ? v
    : Math.min(d.max, Math.max(d.min, d.type === "int" ? Math.round(v) : v));
  const values = {};  // per-key repair: one bad value falls back to its default, the rest are kept
  for (const [id, d] of defs) values[id] = legal(d, saved.values?.[id]) ? clamp(d, saved.values[id]) : d.default;
  const save = () => localStorage.setItem(storageKey, JSON.stringify({ version: 1, values, explicit: [...explicit] }));
  function set(id, value, fromPlayer = true) {
    const d = defs.get(id);
    if (!d || !legal(d, value) || (!fromPlayer && explicit.has(id))) return false;
    if (fromPlayer) explicit.add(id);
    const v = clamp(d, value), changed = values[id] !== v;
    values[id] = v; save();
    if (changed) listeners.forEach((fn) => fn(id, v));
    return changed;
  }
  // Narrator: one persistent, visually hidden (not display:none) polite live region and a bounded queue.
  const live = document.createElement("div");
  live.setAttribute("role", "status"); live.setAttribute("aria-live", "polite");
  live.style.cssText = "position:absolute;width:1px;height:1px;overflow:hidden;clip-path:inset(50%)";
  document.body.append(live);
  const queue = []; let speaking = false;
  const pump = () => {
    if (!queue.length) { speaking = false; return; }
    speaking = true; live.textContent = "";
    setTimeout(() => { live.textContent = queue.shift(); setTimeout(pump, 1200); }, 50);
  };
  const announce = (text) => { if (queue.length >= 4) queue.shift(); queue.push(text); if (!speaking) pump(); };
  // OS preference: initial value only while the player has not chosen; follows live changes.
  const motion = matchMedia("(prefers-reduced-motion: reduce)");
  const importMotion = () => set("motion.reduce_motion", motion.matches, false);
  importMotion(); motion.addEventListener("change", importMotion);
  return { get: (id) => values[id], set, announce, isExplicit: (id) => explicit.has(id),
    reset: (id) => { explicit.delete(id); const c = set(id, defs.get(id)?.default, false); importMotion(); return c; },
    onChange: (fn) => (listeners.add(fn), () => listeners.delete(fn)) };
}
```

Usage: `const a11y = createA11y(await (await fetch("settings_registry.json")).json());`, then
`a11y.onChange((id, v) => ...)` and `a11y.announce("Text size, slider, 150 percent")`. Add
`prefers-contrast` the same way. Wrap `localStorage` calls if storage can throw (some private modes).

## Phaser

- **Built in:** DOM Element game objects place real HTML over the canvas. They need the DOM container
  enabled in the game config and a parent element.
- **Must build:** everything from the web section: real controls, focus, live region, and motion
  settings read by your tweens and camera (camera shake intensity × `motion.camera_shake`).
- **Recommended code:** Phaser's DOM Element examples (Phaser is MIT), plus the store above.
- **Version gates:** Phaser major version, DOM config, and the canvas scale mode. Check that the
  overlay lines up after a resize.

## PixiJS

- **Built in:** the accessibility system creates HTML shadow elements for display objects marked
  accessible. Options include `accessible`, `accessibleTitle`, `accessibleHint`, `accessibleType`,
  `accessibleText`, `accessibleChildren` and `tabIndex`.
- **Must build:** correct names and roles, real keyboard activation, modal order and dynamic
  updates. The defaults do not make every sprite accessible. Check the shadow element's name and role
  in the target browser with a reader.
- **Version gates:** **v7** imports `@pixi/accessibility`. **v8** documents
  `import 'pixi.js/accessibility'`. Don't mix guides across majors. PixiJS is MIT.

## GameMaker

- **Built in:** keyboard and gamepad input, fonts and text drawing. No cross-platform native
  accessibility tree was found.
- **Must build:** an **action layer** (an action-to-binding struct read by all gameplay code),
  runtime remapping with reset, hold and toggle state that clears on focus loss, a caption event
  queue, a speech adapter boundary, and readable layouts.
- **Recommended code:** `Gannio/GMSTolkTalk` (Marketplace) wraps **Tolk** for Windows screen-reader
  output. The wrapper's licence is UNVERIFIED. Tolk itself is **LGPL-3.0 and unmaintained**, so
  review redistribution. A DLL sends speech; it does not make drawn objects navigable.
- **Version gates:** runtime and extension ABI, 32/64-bit, and a non-Windows fallback.

## RPG Maker MZ

- **Built in:** the JavaScript plugin system and the game data model. The UI is canvas-rendered and
  has no semantics.
- **Must build:** a plugin that hooks message windows, selectable lists, the battle log, state and map
  queries. It mirrors text to a DOM live region (bundled NW.js/Chromium) and adds input alternatives.
  When you patch a method, call the previous implementation. Never ship copied engine files.
- **Recommended code:** `craigbrett17/rpgmaker-mv-access` shows ARIA live messages, wall bumps and an
  interactables menu. It is **MV-specific, not verified on MZ**, and its licence is UNVERIFIED.
  `dengopaiv/QuestersWhitestick` (MPL-2.0) claims MV/MZ support, but its repo showed only a README and
  licence. Source completeness is UNVERIFIED, so treat it as a lead.
- **Version gates:** MZ version, plugin order, and the bundled Chromium. Test the shipped runtime.

## Ren'Py

- **Built in:** self-voicing through OS speech. **`v` toggles self-voicing** and **`Shift+A` opens
  the accessibility menu**. Keyboard traversal works. Screen language supports **`alt`** on
  displayables and buttons, **`group_alt`** for groups, and **`renpy.alt`** for spoken text. Text and
  font preferences are in the shipped accessibility screens.
- **Must build:** `alt` text on every image button and meaningful image, descriptions of non-text
  choices, and narration of gameplay or minigame state. If you replace the default screens, keep the
  accessibility controls and shortcuts working.
- **Recommended code:** `renpy/common/00accessibility.rpy` (MIT-style header for that file; other
  SDK parts have other licences).
- **Version gates:** the bundled Ren'Py version. Speech backends are documented for desktop, so test
  mobile and web exports and each voice language separately.

## Twine (SugarCube vs Harlowe)

- **Built in:** HTML output. SugarCube documents keyboard activation for its `ariaClick` helper.
  SugarCube 2 is BSD-2-Clause.
- **Must build:** headings per passage, focus moved to the new passage after navigation (SugarCube
  passage events, such as `:passagedisplay`, UNVERIFIED for your version), image descriptions, motion
  options, and non-timed alternatives to timed or disappearing links. SugarCube's `Setting` API
  (`Setting.addToggle`, `Setting.addList`, `Setting.addRange`) can hold the registry keys. Check the
  names in your release.
- **Harlowe is a different runtime.** Its current accessibility behaviour and licence are UNVERIFIED
  here. Never claim "Twine games are screen-reader accessible": test the exported story.

## Bevy

- **Built in:** `bevy_a11y` coordinates with **AccessKit** (`AccessibilityNode`,
  `AccessibilityRequested`). Bevy UI widgets can expose nodes. The crate docs say it supplies
  primitives; it does not make the whole interface accessible on its own.
- **Must build:** AccessKit roles, labels and actions for custom widgets, a logical action layer, the
  caption event system and a TTS adapter.
- **Recommended code:** Bevy is MIT/Apache-2.0. Start from the examples of the **same** Bevy release.
- **Version gates:** pin Bevy and AccessKit as a pair (observed: `bevy_a11y` 0.17.2 → `accesskit
  ^0.21`). Bevy 0.15 removed the AccessKit re-export, so add `accesskit` directly. Role and type
  names change between releases, so never paste a 0.12 example into a newer project.

## MonoGame / FNA

- **Built in:** rendering, fonts, input, audio. No native accessibility tree was found in core.
- **Must build:** an action layer, a retained UI or semantic overlay, focus, narration, subtitles and
  text reflow.
- **Recommended code:** Windows reader output through a reviewed C# binding to **Tolk** (LGPL-3.0,
  unmaintained) or **Prism** (MPL-2.0, active; check the native build needs). AccessKit integration is
  possible but takes engineering; it is not a ready package.
- **Licences and gates:** MonoGame and FNA are both Ms-PL with third-party exceptions. Their native
  dependencies differ, so check DLL bitness per platform.

## LÖVE (LOVE2D)

- **Built in:** fonts, graphics, input. No screen-reader bridge in the core API.
- **Must build:** an action map, keyboard menus, caption events and a narration adapter. Create fonts
  once per size token and cache them. Never call `love.graphics.newFont` every frame.
- **Recommended code:** `JRJurman/love2d-a11y-template` sends `tts:`-prefixed output to a browser
  live region for web builds. The licence changed from ISC to CC0 on 2026-01-31, so pin the revision.
  `JRJurman/love2d-native-sr-example` uses **SRAL** (MIT, **archived** August 2026). Its own licence
  is UNVERIFIED, so do not vendor it yet.
- **Version gates:** LÖVE 11.x vs 12.x API differences, LuaJIT FFI, and the target OS.

## Defold

- **Built in:** input bindings with action IDs, GUI nodes, text layout, `sys.get_save_file` with
  `sys.save`/`sys.load` for the settings file, and `window.set_listener` focus events (clear toggle
  latches on focus loss).
- **Must build:** a runtime remapping adapter over action IDs, scalable GUI layouts, captions, and
  narration through a native extension.
- **Recommended code:** `Brun0oO/defold-tts-druid` (MIT). Its 2021 announcement covered iOS and macOS
  only, so check the current platforms and the extension ABI.
- **Version gates:** Defold version, native extension build server, and font resources per size.

## Construct 3

- **Built in:** the **Speech Synthesis** object (support check, voice and rate). The **Text** object's
  **"Read aloud"** property forwards important text changes to screen readers. The Local Storage
  plugin can persist settings.
- **Must build:** narration preferences and queue policy, menu navigation and labels, and logical
  actions. Use "Read aloud" only on a dedicated announcement text, never on a fast-changing score.
- **Gates:** speech needs browser support and often a user gesture, so it cannot be assumed at
  startup. Projects in worker mode cannot touch the DOM directly. Confirm the context before using web
  APIs. No vetted open-source Construct accessibility add-on was found; prefer the built-ins.

## Roblox

- **Built in:** `GuiService.PreferredTextSize`, `GuiService.PreferredTransparency` and
  `GuiService.ReducedMotionEnabled`. Observe changes with `GetPropertyChangedSignal`. TextService has a
  size-offset helper (`GetTextSizeOffsetAsync`). It yields and can fail, so wrap it in `pcall`.
- **Must build:** apply these to custom UI and motion yourself. Use reflowing, large controls and
  controller focus. Bind actions with `ContextActionService`. Store player settings server-side.
- **Don't compound transparency:** keep each element's **original** value and set
  `frame.BackgroundTransparency = original * GuiService.PreferredTransparency`. Never multiply an
  already-modified value.
- **Licence:** a proprietary platform. Public Creator Store assets are not MIT by default. No general
  native screen-reader API for custom experience UI was verified.

## Minecraft modding

- **Built in:** Java and Bedrock have their own accessibility settings, including Java's narrator.
  Edition and release matter.
- **Must build:** narration for custom widgets and screens (method names differ between Mojang and
  Yarn mappings and by version, UNVERIFIED), spatial queries for new mechanics, and readable mod
  screens.
- **Recommended code:** `minecraft-access/minecraft-access` (**GPL-3.0**; Fabric and NeoForge builds
  documented) exposes an add-on API (`MinecraftAccessAddon`). Integrating with it, or copying its code,
  brings GPL obligations.
- **Version gates:** match Minecraft version, loader (Fabric vs NeoForge), mappings, Java runtime and
  mod version. A Fabric mod does not run on NeoForge unchanged.

## Custom engine: implement the contract with these primitives

| Contract piece | Minimal primitive |
| --- | --- |
| A11ySettings | A dictionary loaded from the registry JSON, with typed validate and clamp, a per-key change callback, explicit-set flags, and a versioned file written via temp + rename. Add a follow resolver for `follow`/`off`/`on` override keys: `resolve_toggle(key, follow_value)` returns true for `on`, false for `off`, and `follow_value` for `follow` or a key the game removed. Outline classes (`visual.outline_*`) follow `visual.high_contrast`. The motion overrides (`motion.auto_camera`, `motion.fov_effects`, `motion.speed_lines`, `motion.parallax`, `motion.menu_animation`) follow `not motion.reduce_motion`. Re-evaluate when either the override or its umbrella changes |
| InputAssist | An action table (action → bindings), with hold/toggle latch state cleared on pause, focus loss and disconnect. Timing windows are `base × input.timing_window_scale` in simulation time (real time: `/ time_scale`). Never apply either factor twice |
| AssistPolicy | Pure functions over the store, called at the moment of decision. Skips are two functions with one completion path: `request_skip(challenge_id)` is gated only by `assist.skip_available`; `request_scene_skip(scene_id)` is gated only by `content.scene_skip` and works when the challenge key is off or absent. Both reach the same completion event, so rewards, flags and checkpoints match a win. A repeat for the same id never completes twice |
| CaptionBus | An event struct `{id, kind, text, speaker, position, importance, duration, merge_key}` with a list of subscribers. The view paginates and never clips: at most 2 lines per page, every word of the event shown in order (timed or advanced on input), re-paginated when the viewport, text scale or subtitle size changes |
| Narrator | An OS screen-reader bridge where available (**AccessKit**, MIT/Apache-2.0), plus a TTS fallback with a bounded queue. Tag each utterance with its setting (menu or dialogue). Turning one off stops its current speech and drops its queued items; re-check the setting and route owner before each dispatch |
| FlashGuard | A ring buffer of real-time timestamps. A request counts if area ≥ 0.25 **or** luminance change ≥ 0.1 **or** saturated red (R/(R+G+B) ≥ 0.8 on linear channels); these are conservative defaults, tuned only with captured-output analysis. Allow no more than 3 counted flashes in any 1 s **and** no more than 9 in any 5 s, in every mode; clamp configured budgets to those maxima. Always desaturate saturated red. Still analyse captured footage as a separate gate |
| CameraComfort / Guidance / ColorTokens | Multipliers at the camera owner, with separate `motion.head_bob` and `motion.weapon_sway`. A global render override (such as depth-of-field quality) has one persistent owner that keeps the authored value and counts requests. Guidance channel keys (`guidance.paint`, `guidance.trail`, `guidance.highlights`) where `follow` means use `guidance.level`. A token → colour + symbol table, where `custom` parses `visual.custom_colors` (`role=#RRGGBB;...`) |
| ContentFilter | Tags or groups on variant nodes. Show the requested level, else the nearest **more** filtered authored variant. Never fall back below the requested level: with no sufficient variant, keep the content hidden (or show an authored neutral stand-in) and report the missing coverage |

Speech backends: `Windows.Media.SpeechSynthesis` or SAPI (Windows), `AVSpeechSynthesizer` (Apple),
Android `TextToSpeech`, and browser `speechSynthesis`. Reader bridges: **Prism** (MPL-2.0),
**UniversalSpeech** (MIT), **Tolk** (LGPL-3.0, unmaintained), **NVDA controller client**
(LGPL-2.1+). TTS is not a screen reader. Pick one route per announcement. Photosensitivity: analyse
captured footage with **EA IRIS** (BSD-3-Clause). It is not a certification.
