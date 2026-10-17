# Touch and mobile

**Load when:** S134 touch input (touch handlers, virtual sticks, canvas gestures), S135 a required
phone orientation or reach (edge controls, two-thumb layouts, landscape lock). Related: S021 small
touch targets, S022 dragging, S023 swipe, pinch, multi-touch or path tracing, S026 tilt or shake,
S029 text entry. Also load for phone-as-controller party games.
**Skip when:** no supported platform takes touch input and no mode uses a phone. Record the target
platforms as evidence. A desktop keyboard test never proves mobile access.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| M24 | Touch-only play: no required keyboard or controller | none (design requirement) | On when touch is supported | C | M, S |
| M27 | Touch target size, including hit area | `visual.ui_scale` (must scale hit areas) | At or above platform minimum | C | M, V |
| M29 | Touch target spacing | none yet | At or above platform minimum | C | M |
| M28 | Movable, resizable virtual controls | none yet | Designer layout | C | M |
| M05 | Left-hand and right-hand layouts, one-handed reach | none yet | Designer layout | C | M |
| M30 | Tap alternative to swipe and path gestures | none yet | Available | C | M |
| M31 | Single-finger alternative to pinch and multi-touch | none yet | Available | C | M |
| M32 | Tap to select, tap to place, instead of drag | none yet | Available | C | M |
| M39 | Longer long-press and double-tap recognition | `input.timing_window_scale` | 1.0 | C | M |
| M38 | Ignore accidental repeat taps (debounce) | none yet | Off | C | M |
| M41 | Stationary targets while aiming a touch | none (design requirement) | On | B | M, V |
| M42 | Cancel by sliding off; activate on release | none (design requirement) | On | B | M |
| M25 | Alternative to tilt, shake and gyro | none yet | Available | C | M, O |
| L12 | Portrait or landscape choice | none yet | Follow device | C | M, S |
| V01 | Text scale, seeded from OS text size | `text.scale` | 1.0, or the OS value | B | V, C, S |
| R01 | TalkBack and VoiceOver compatibility for menus | `narration.enabled` | Follows OS screen reader | B (menus), A (full gameplay) | V, C |
| R19 | OS voice-control compatibility | none (needs semantics) | On where semantics exist | C | M |
| L10 | Switch Access / Switch Control scanning | none yet | Off | A | M |
| L13 | External controller and keyboard, including setup screens | none yet | On when supported | C | M |
| L20 | Safe suspend on calls, notifications and backgrounding | `system.pause_on_focus_loss` | true | C | M, S, T |
| N12 | Vibration intensity and Off | none yet | Designer value (mild) | C | N, M |

Add missing keys to the project's registry copy. Touch layouts are `profile` scope per orientation;
vibration strength is `device` scope. Remapping and hold/toggle basics live in
[input-and-motor](input-and-motor.md). Narration details live in
[screen-reader-and-voice](screen-reader-and-voice.md).

## How to implement

### 1. Targets and spacing (M27, M29, M41, M42)

- Size the **hit area**, not the icon. A small glyph can sit inside a large invisible touch region.
- `visual.ui_scale` scales hit areas with the art. After scaling, targets must not overlap: reflow
  or scroll instead.
- Keep targets still while a finger approaches them. No bouncing buttons in menus.
- Activate on release. Sliding off a control cancels it. Destructive actions confirm.
- Keep essential controls out of OS gesture zones (home indicator, back swipe, notification pull)
  and inside the safe area.

### 2. Movable controls and reach (M28, M05)

- Offer a layout editor: move, resize and set opacity of each virtual control, with reset. Store a
  layout per orientation.
- The editor itself must not need dragging. Select a control, then tap a destination or use nudge
  and size buttons. Changes preview live and can be undone.
- Provide left-hand and right-hand presets and a one-thumb layout that puts every required action
  within one thumb's reach. Check settings, pause and back too, not only gameplay buttons.

### 3. Gestures and dragging (M30-M32, M39, M38, M25)

- Every gesture function also exists as a button or a discrete step. Swipe to match becomes tap
  piece, tap neighbour. Pinch zoom becomes + and - buttons. Rotate becomes rotate buttons. Path
  tracing becomes tap points in order.
- Drag-and-drop becomes tap to select, tap to place, with a visible selection and a cancel.
- Long-press and double-tap timing scale with `input.timing_window_scale` through
  `InputAssist.timing_window()`. Long-press is never the only route; offer a separate button.
- Optional debounce ignores a second tap within a player-set interval. Never force it on everyone.
- Tilt, shake and gyro are optional. Provide on-screen or button alternatives with sensitivity.

### 4. Orientation (L12)

Support both orientations where the layout allows, or at least never require rotating the device to
continue. Reflow UI and controls for each. Offer an in-game orientation lock so a mounted device
does not flip.

### 5. OS assistive technology (R01, R19, L10)

- **A canvas exposes nothing by default.** Engines draw pixels. TalkBack, VoiceOver, Switch Access,
  Switch Control and Voice Control see no buttons unless the engine publishes an accessibility tree
  for them. Check the installed engine version and export platform before promising support: Godot
  4.5's AccessKit support is documented for desktop, with mobile coverage to be checked per release,
  and Unity's `AccessibilityHierarchy` platform list differs between 6.0 and 6.3. Test on real devices.
- **TTS is not screen-reader support.** Self-voicing through Android `TextToSpeech` or Apple
  `AVSpeechSynthesizer` creates speech, not TalkBack or VoiceOver semantics.
- Expose name, role, value and state for each control. Gesture-only functions get named actions so
  a screen-reader user can trigger them with the standard double-tap.
- Switch scanning needs a navigable, ordered set of items. If the engine cannot expose them, a
  deliberate in-game scan mode is an Advanced scope decision.
- Do not change general UI behaviour just because an accessibility service is running. Android
  advises against branching on `AccessibilityManager.isEnabled()`.
- Menu narration is Baseline. Narrated board state (coordinates, piece states) is Contextual; see
  [structured-ui-and-strategy](structured-ui-and-strategy.md). Full nonvisual play is Advanced.

### 6. Interruptions (L20)

Calls, notifications, alarms, low-battery dialogs, app switching and screen lock all trigger a safe
suspend:

- pause simulation and timers, save state, and stop audio;
- release every held touch and clear latched toggles (`InputAssist` latch rules);
- resume to a pause screen, never straight into action;
- time does not run while backgrounded, unless the design says so and the option text explains it.

Live online sessions cannot pause. Say what happens to the player's character while away.

### 7. Text and platform preferences (V01)

Seed `text.scale` from the OS text size (Dynamic Type on iOS, font scale on Android) only until the
player sets it. Android 14 scales fonts non-linearly, so do not multiply by the font scale twice.
Larger text must reflow, not clip.

### 8. Colour and feedback on small screens

Special pieces, matches and timers use ColorTokens with a shape or symbol, not hue alone. Haptics are
an extra channel, never the only one. See [color-and-clarity](color-and-clarity.md).

## Numbers that matter

| Value | Source and scope |
|---|---|
| 48 x 48 dp minimum touch target, including touchable area | [Android accessibility design](https://developer.android.com/design/ui/mobile/guides/foundations/accessibility). Visual icon may be smaller. |
| 15 x 15 mm (phone): 59 / 118 / 236 px at 100 / 200 / 400 DPI | [XAG 107](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/107), phone touch targets. |
| 24 x 24 mm (tablet): 94 / 189 / 378 px at 100 / 200 / 400 DPI | [XAG 107](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/107), tablet touch targets. |
| 24 x 24 CSS px (AA); 44 x 44 CSS px (AAA) | [WCAG 2.2 SC 2.5.8 and 2.5.5](https://www.w3.org/WAI/WCAG22/Understanding/target-size-minimum.html). Web units with exceptions. |
| Text 18 / 36 / 72 px at 100 / 200 / 400 DPI | [XAG 101](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/101), mobile default text height. |
| Larger Text means 200% or more | [Apple Accessibility Nutrition Labels](https://developer.apple.com/help/app-store-connect/manage-app-accessibility/overview-of-accessibility-nutrition-labels/). |
| 0.5 s post-acceptance cooldown | [GAG](https://gameaccessibilityguidelines.com/include-a-cool-down-period-post-acceptance-delay-of-0-5-seconds-between-inputs/). Optional, never forced on everyone. |

Dp, mm, CSS px and rendered pixels are different units. Measure on the real device.

## Shipped examples

- Princess Farmer: several play styles, one-stick interactions, the same story and rewards across
  styles, and board information not carried by colour alone.
  [Whitethorn](https://whitethorngames.com/princessfarmer/accessibility). The publisher describes a
  designer's hand injury informing the one-handed design.
  [Whitethorn news](https://whitethorngames.com/whitethorn-games-news/accessibility-features-botany-manor-mythwrecked-princess-farmer-access-ability-showcase-2023).
  Shows that "casual" games still carry motor load.
- Candy Crush Saga: a self-identified colour-blind player reports trouble telling blue and purple
  pieces apart. [King community](https://community.king.com/en/candy-crush-saga/discussion/462233/colour-blind-players).
  A player report, not an audit. Another user's grayscale suggestion is not a fix.
- Candy Crush Friends Saga: official announcement of reduced scrolling and screen shake.
  [King community](https://community.king.com/en/candy-crush-friends-saga/discussion/313075/new-check-out-the-accessibility-features).
  Do not transfer features between the two Candy Crush titles.
- A Blind Legend: audio-first game using binaural audio and touch on mobile.
  [App Store](https://apps.apple.com/us/app/a-blind-legend/id973483154). An equivalent visual or
  caption route is not established.
- Research: interviews with 32 experienced blind and low-vision mobile players in China describe
  barriers across games with different accessibility levels.
  [arXiv 2502.09866](https://arxiv.org/abs/2502.09866). One country's participants do not represent
  every mobile player or genre.
- Platform guidance: [Android accessibility principles](https://developer.android.com/guide/topics/ui/accessibility/principles)
  cover TalkBack, Switch Access, labels and alternatives to complex gestures. Guidance, not a
  shipped game example.

## Verify

- [ ] Complete first launch, settings, a full level, pause and quit with touch only.
- [ ] Measure the smallest required hit area on the smallest supported phone at default and
      maximum `visual.ui_scale`. No overlaps.
- [ ] Every gesture has a working tap or button alternative, including in tutorials and minigames.
- [ ] Every drag interaction works with tap-select, tap-place and can be cancelled.
- [ ] The layout editor works without dragging; layouts persist per orientation and reset works.
- [ ] Play one-handed with each handed layout, including settings, pause and back.
- [ ] Rotate the device mid-game: layout reflows and nothing is lost. The orientation lock holds.
- [ ] Receive a call or notification, background the app, lock the screen: the game suspends,
      held touches release, timers stop, and it resumes to a pause screen.
- [ ] With TalkBack and VoiceOver on, real devices announce every menu control with name, role and
      value. If not, record the gap as `fail` or `blocked`; do not claim support.
- [ ] Switch Access or Switch Control can reach menus, or the gap is recorded with a scope decision.
- [ ] OS text size at its largest seeds `text.scale`; the player can override it, and text reflows.
- [ ] Every special piece and state is distinguishable in grayscale.
- [ ] A connected controller or keyboard reaches setup screens as well as gameplay.

## Common mistakes

- Small icon with a small hit area → enlarge the touch region even if art stays small.
- UI scale that grows art but not hit areas → scale both.
- Layout editor that only works by dragging → tap-select and nudge buttons.
- Swipe, pinch or long-press as the only route → add a button or discrete step.
- Assuming TalkBack or VoiceOver "just works" on a canvas game → expose semantics or record the gap.
- Calling TTS self-voicing "screen-reader support" → they are different capabilities.
- Landscape lock with no way to play mounted in portrait → support both, or at least never require
  rotating to continue.
- Resuming from a phone call straight into a moving level → resume paused.
- A "colour-blind filter" while special pieces differ only by hue → add shapes or symbols.
- Copying a feature from one title in a series to another → verify per title and platform.
