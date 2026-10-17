# VR and XR

**Load when:** S137 VR (XR rig, headset and controller actions), S136 AR camera or physical-location
requirements, S138 standing or room-scale movement, S139 tasks needing both hands, S140 large or
precise gestures, S141 continuous locomotion or turning, S142 flying, driving or forced movement in
XR, S143 spatial captions or menus. XR is a platform, not a genre: every underlying gameplay guide
(combat, timing, puzzles, vehicles) still applies.
**Skip when:** no supported build runs on a headset or AR device. Record the target platforms.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| M43 | Seated mode | `game.xr_seated` | Offered before first interaction | C | M, S |
| M44 | Height and reach calibration | `game.xr_reach` | Calibrate on first launch | C | M, S |
| M45 | Virtual crouch, raise-floor and distant grab / reach extension | `game.xr_virtual_crouch` | Available | C | M |
| M05 | Handedness: swap dominant hand, menus on either hand | none yet | Designer value | C | M |
| M46 | One-handed parity for two-handed tasks | `game.xr_one_handed` | Available | C | M |
| M13 | Toggle grip instead of holding grip | `input.hold_mode` | `hold` | C | M |
| M25 | Button alternatives to gestures, poses and throws | none yet | Available | C | M, O |
| O18 | Teleport, snap or smooth locomotion choice | none yet | Choice offered before first motion | C | O, M |
| O19 | Snap or smooth turning; turn angle and speed | none yet | Choice offered before first turn | C | O |
| O20 | Comfort vignette strength, including Off | none yet | Designer value | C | O |
| O12 | Fixed horizon; reduced roll in forced movement | none yet | Off | C | O |
| O09 | Manual recenter always available; auto-recenter optional | `motion.auto_camera` (umbrella `motion.reduce_motion`) stops auto-recenter; manual recenter is an action | Manual on | C | O, M |
| H30 | Caption anchoring: head-locked or world-locked, distance | none yet | Designer value | C | H, V, O |
| H24 | Caption size in the headset | `subtitles.size` | 1.0 | B | V, H |
| H21 | Direction indicator for speakers outside the view | `subtitles.direction` | false | C | H |
| V01 | Menu and HUD text scale in the headset | `text.scale` | 1.0 | B | V, C |
| N12 | Controller haptics intensity and Off | none yet | Designer value | C | N, M |
| M47 | AR: stationary or remote alternative to physical-location gates | none yet | Developer decision | A | M, S |

Add missing keys to the project's registry copy. Height, reach and boundary calibration are `device`
scope; comfort and handedness are `profile` scope. Shared motion rules live in
[camera-and-motion](camera-and-motion.md); vehicle aids in [vehicles](vehicles.md).

## How to implement

### 1. Calibrate before anything moves

The first headset screen is static, readable and operable with controller buttons only. It offers:
seated or standing, height calibration, reach calibration, dominant hand, locomotion and turn
choices, vignette, and caption placement. Each choice previews while stationary. All of it stays
reachable later from the pause menu, and recalibration never needs standing up or a gesture.

### 2. Seated play and reach (M43, M44, M45)

- Every required object, menu and recalibration action must be reachable from the calibrated seated
  position without standing, bending to the floor or reaching behind.
- Seated mode (M43) is chosen before the first interaction. Height and reach calibration (M44)
  rescales or relocates interactables; height scaling alone does not remove bending.
- Virtual crouch (M45, a button that lowers the viewpoint) and raise-floor cover low objects.
- Distant grab (M45, point and pull) covers floor items, high shelves and objects out of reach.
- Height scaling alone does not replace bending or two-handed tasks. Test each task, not the
  average reach.
- With snap turn available, seated players never need to turn their chair to face a target.

### 3. One hand, either hand (M05, M13, one-handed parity)

- Swap the dominant hand, and put menus and the pause button on either hand.
- Grip can toggle instead of being held, through `InputAssist.is_held()` with `input.hold_mode`.
  The latch clears on pause, scene change, controller loss and death.
- Every two-handed task (hold a jar, open its lid) gets a one-handed path: auto-hold, a dock that
  holds an object, or doing steps one after another.
- Settings must open and work with one hand.

### 4. Gestures and motion (M25)

Throws, overhead reaches, poses, swings and drawing shapes all have button alternatives. Offer
gesture-range calibration and tolerance. Gesture timing uses `InputAssist.timing_window()`. Motion
controls stay available for players who want them.

### 5. Locomotion and turning (O18, O19, O20, O12)

- Offer teleport, snap and smooth movement as independent choices. Do not prescribe one comfortable
  mode for everyone.
- Every mode reaches every destination. Teleport must also handle ladders, gaps, lifts and vehicles,
  or those places need another route.
- Offer several snap-turn angles and a smooth-turn speed. The turn method is separate from the
  movement method.
- The comfort vignette has a strength slider, including Off. It must not hide essential cues.
- Forced movement (S142: vehicles, rides, conveyors): offer reduced acceleration, a fixed horizon, a
  cockpit or frame of reference, and a stationary alternative or skip where the content allows.
- Never move the headset camera without head motion for effects such as impact shake. Put that
  feedback in the hands, HUD or haptics instead.

### 6. Recentering and safety (O09)

- A remappable recenter action is always available. Automatic recentering is optional.
- **Never disable, hide or override the platform guardian or boundary system.** No option, mode or
  assist may ask the player to leave the play area.
- Do not require turning around in the real room. Snap turn and recenter cover full rotation.

### 7. Captions and spatial UI (H30, H24, H21, V01, S143)

- Offer head-locked captions (follow the view at a fixed distance, with gentle smoothing) and
  world-locked captions (near the speaker). Distance and size are adjustable.
- For a speaker outside the view, show an arrow or word pointing to them (`subtitles.direction`).
  Never force repeated neck turns to read.
- Captions use the same CaptionBus events as flat-screen play. Style rules (2 lines, speaker name,
  background) still apply. See [audio-and-captions](audio-and-captions.md).
- Menus can be summoned in front of the player and follow them. Text scales with `text.scale`.

### 8. Nonvisual and low-vision access

Menu narration is Baseline. Querying objects by voice or button ("what is in front of me?") is a
documented XR need in W3C XAUR, but full nonvisual spatial play is Advanced: record a scope
decision. See [screen-reader-and-voice](screen-reader-and-voice.md).

### 9. AR and physical location (S136)

Camera permission, GPS, geofences and walking to real places are access gates. A stationary or
remote alternative (M47) is Advanced and changes the design: ask the developer what must stay
location-based and record the decision before implementing anything. Provide input fallbacks for
camera-aiming and gesture tasks.

## Numbers that matter

| Value | Source and scope |
|---|---|
| Text at least 18 px at 1080p (PC/VR) | [XAG 101](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/101). A rendered-pixel rule; headset legibility also depends on distance and lens. Test in the headset. |

No sourced numeric comfort thresholds (turn angles, vignette strength, caption distance, locomotion
speed) were found in this skill's research. Offer choices; do not present any value as "the safe
one".

## Shipped examples

- Cosmonious High: one-handed mode, height adjustments including a smaller-human option, and
  subtitles. [Owlchemy Labs FAQ](https://owlchemylabs.com/faq).
- No Man's Sky: the Fractal update documents VR comfort and controller settings.
  [Hello Games](https://www.nomanssky.com/fractal-update/?cli_action=1677074777.055). Verify exact
  headset and build coverage.
- Beat Saber: No Fail, practice and one-saber content. A CHI 2025 study reports one-handed
  participants still met content and difficulty restrictions and discomfort in two-hand No Fail.
  [Steam](https://store.steampowered.com/app/620980/Beat_Saber/),
  [study](https://johannwentzel.ca/projects/motionblocks/files/MotionBlocks__CHI_2025.pdf).
  One-saber playlists are not general one-handed support.
- W3C XR Accessibility User Requirements describe multimodal input and output, orientation, object
  querying and customisation needs. [XAUR](https://www.w3.org/TR/xaur/). A Working Group Note, not
  a certification.
- GAG: avoid VR simulation-sickness triggers.
  [GAG](https://gameaccessibilityguidelines.com/avoid-vr-simulation-sickness-triggers/). Body
  tracking must not assume one body type.
  [GAG](https://gameaccessibilityguidelines.com/do-not-rely-on-motion-tracking-of-specific-body-types/).

## Verify

- [ ] From a fresh profile, complete calibration, settings and the first scene seated, using
      controller buttons only.
- [ ] Reach every required object and menu seated, at the lowest and highest calibrated heights,
      without bending, standing or reaching behind.
- [ ] Complete every two-handed task with one hand, for each dominant-hand setting.
- [ ] Every gesture, throw and pose has a working button alternative.
- [ ] Teleport, snap and smooth movement each reach every destination, including ladders, gaps and
      vehicle entry.
- [ ] Turn method, turn angle and vignette change independently and apply without restart.
- [ ] Recenter works at any time, including during menus and forced movement.
- [ ] The guardian or boundary is never disabled or hidden by any option.
- [ ] Captions stay readable head-locked and world-locked; off-view speakers show a direction cue.
- [ ] Forced-movement sections offer the documented comfort alternatives.
- [ ] Settings persist across sessions; height calibration stays with the device.
- [ ] AR location gates have a recorded developer decision.
- [ ] Achievements and progress are identical in every comfort and seated configuration.

## Common mistakes

- Comfort settings only reachable after the first smooth-movement scene → calibrate first.
- "Seated mode" that only lowers the camera → test every task's reach seated.
- Teleport that cannot cross gaps or climb ladders → every mode reaches every destination.
- Tying turn method to movement method → separate settings.
- Caption panels pinned to the speaker behind the player → head-locked option and direction cue.
- Two-handed puzzles with no one-handed path → auto-hold, dock or sequential steps.
- Disabling the guardian to fit a large room-scale level → never; redesign the space.
- Camera shake applied to the headset view → feedback through hands, HUD or haptics.
- Assuming flat-screen guidance covers XR → load the gameplay guides too.
