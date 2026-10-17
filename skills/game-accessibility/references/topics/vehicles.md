# Vehicles: driving, flying and piloting

**Load when:** S144 controllable vehicle movement (throttle, steering, brake, pitch, roll, yaw), S055
camera roll, banking or horizon movement from vehicles, S142 XR flying, driving or forced movement.
Related: S024 precise analog magnitude (steering, throttle), S145 collision or physics-induced
movement, S027 force feedback or trigger resistance, S106 a vehicle that can get stuck. One
mandatory chase, boat or turret section counts.
**Skip when:** the player never directly controls a vehicle. A vehicle that only moves on rails in a
cutscene routes to [camera-and-motion](camera-and-motion.md) instead. Record the evidence.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| D19 | Steering assist (auto-steer), with strength | none yet | Off | C | M, V, C |
| D19 | Braking assist (auto-brake before corners and obstacles) | none yet | Off | C | M, V, C |
| D19 | Throttle assist (hold speed, auto-accelerate) | none yet | Off | C | M, C |
| D19 | Automatic gears | none yet | Designer value | C | M, C |
| D19 | Flight stabilisation and auto-level | none yet | Off | C | M, O, C |
| C18 | Racing line or flight path guide | `guidance.trail` (or a project key for the racing line) | `follow` | C | C, V |
| D24 | Rewind, reset to track, collision recovery | none yet | Available | C | M, T |
| D08 | Collision damage and crash failure set separately | none yet | Designer value | C | M, T |
| D14 | Game speed (offline modes) | `assist.game_speed` | 1.0 | C | M, C |
| M01 | Remapping in every vehicle context, menus, hangar, enter/exit | binding file | On | B | M, S |
| M08 | Sensitivity per axis (steer, pitch, roll, yaw, throttle) | none yet | 1.0 | C | M |
| M09 | Inner and outer dead zones per axis | none yet | Designer value | C | M |
| M10 | Response curve per axis, with live preview | none yet | Linear or designer value | C | M |
| M07 | Inversion per axis | `input.invert_y` covers camera only | Off | C | M, O |
| M23 | Digital-only alternative for analog axes | none yet | Available | C | M, V |
| N14 | Trigger resistance and force-feedback strength or Off | none yet | Designer value | C | M, N |
| O12 | Stable horizon: camera roll locked to world, not vehicle | none yet | Off | C | O |
| O06 | Camera view and distance: cockpit, hood, chase | none yet | Designer value | C | O, V |
| O01 | Camera shake from speed, impacts and engines | `motion.camera_shake` | 0.5 (Reduced) | C | O, N |
| O04 | Motion blur | `motion.motion_blur` | true | C | O, V |
| O13 / O14 | Speed FOV kick, speed lines, radial blur | `motion.fov_effects`, `motion.speed_lines` (umbrella `motion.reduce_motion`) | `follow` | C | O, N |
| O05 | Field of view | `motion.fov` | 75 | C | O, V |
| R15 | Distinct audio cues for turns, edges, braking points, opponents | none yet | Off | C (A for full nonvisual driving) | V |
| R16 | Spoken turn-by-turn or bearing callouts | none yet | Off | A | V, C |
| C06 | Practice area and cue preview without failure | none yet | Available | C | C, M, V |

Add missing keys to the project's registry copy with `profile` scope. Device calibration (wheel
range, joystick centre) uses `device` scope. Hold/toggle and remapping basics live in
[input-and-motor](input-and-motor.md).

## How to implement

### 1. Separate driving aids from presentation

Driving aids change what the vehicle does. Comfort options change what the camera shows. Keep them
in different settings and different code paths. A player may want auto-steer and full speed effects,
or manual driving with a locked horizon.

### 2. Independent assists through one vehicle policy

- The vehicle controller asks a policy for each assist at the moment of decision, the same pattern
  as AssistPolicy: `steer_assist()`, `brake_assist()`, `throttle_assist()`, `auto_gears()`,
  `stabilise()`. It never reads raw settings.
- Each assist blends player input with an AI controller output by a strength value. Player input
  can always override, so the player keeps the decisions they want to make.
- Each assist is its own toggle. A "One Touch" preset may switch several on together, but it shows
  each change and every value stays editable (core-policy section 5).
- "Automatic gears" does not remove simultaneous steering and throttle. Count the inputs that remain
  with each assist on.
- Assists change at the next safe step, mid-race, without a restart.

### 3. Racing line and path guides (C18)

Render the racing line or flight path through GuidanceLayer on its own channel. Braking zones use a
ColorTokens colour **plus** a shape (chevrons, bars), never red/green alone. Offer off, on request
and standard. Checkpoint gates and hazards stay visible when guidance is off; they are not guidance.

### 4. Recovery (D24, D08, S106)

Offer reset to track, flip-back, unstick and a short rewind. Collision damage and crash failure have
their own setting, separate from enemy damage (`assist.damage_taken` does not cover crashes unless
its description says so). Recovery uses the normal checkpoint path and never marks the save or the
result. In ranked or shared races, rewind and assists need explicit session rules: see
[multiplayer-and-communication](multiplayer-and-communication.md).

### 5. Axes and devices (M01, M07-M10, M23, N14)

- Remap every context: menus, garage or hangar, driving, flying, gunner seats, docking, and entering
  or leaving the vehicle. Allow several bindings per action.
- Wheels, joysticks, HOTAS, pedals, adaptive controllers and keyboards all enter through the action
  layer. Never read a device directly in vehicle code.
- Per-axis sensitivity, inner and outer dead zones, curve and inversion, each with a live preview
  that shows input versus output.
- Digital-only steering: holding a button ramps the axis at a set rate. Combined with steering
  assist, a player can drive with a few buttons.
- Force feedback and adaptive trigger resistance have strength and Off. Never require high force to
  accelerate or brake.

### 6. Camera comfort (O01, O04-O06, O12-O14)

- The camera owner (CameraComfort) scales every shake source by `motion.camera_shake`: engine
  rumble, speed shake, collisions, landings and scripted crashes.
- **Horizon lock:** the camera follows vehicle position and heading but keeps world-up. Offer it
  for cockpit and chase views. Banking still shows through the vehicle model and HUD attitude
  indicator.
- Speed effects obey their own keys: FOV kick follows `motion.fov_effects`, speed lines and radial
  blur follow `motion.speed_lines` (both default to the `motion.reduce_motion` umbrella), and blur
  follows `motion.motion_blur`. Keep speed readable through a steady speedometer, engine pitch and captions.
- View choice: cockpit, hood and chase cameras, with camera distance. A view mode that does not
  exist yet is an Advanced scope decision, not a silent promise.

### 7. Nonvisual and low-vision driving (R15, R16, C06)

Full nonvisual driving is Advanced: record a scope decision. If in scope, give each kind of
information its own cue: upcoming turn direction and sharpness, braking point, track edge, opponent
position, collision risk. Each cue has its own volume and can be previewed one at a time in a
glossary and practice area. Expect a learning curve and say so. Build and tune the cues with blind
and low-vision players. Menus, garage and results narrate through Narrator.

### 8. Time and speed

`assist.game_speed` slows simulation time in offline modes. Online, a client cannot slow the race:
the option description says so. Do not stack game speed with a second braking-time multiplier by
accident.

## Numbers that matter

| Value | Source and scope |
|---|---|
| Sensitivity range at least 50% below and 50% above default, per analog control | [XAG 107](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/107). Applies to steering, throttle and flight axes independently. |
| Camera shake default Reduced (0.5), Off available | This skill's safe default ([core-policy](../core-policy.md) section 3), including vehicle shake. |

No sourced comfort numbers for horizon lock or speed effects were found in this skill's research.

## Shipped examples

- Forza Motorsport: Blind Driving Assists with configurable track cues and cue previews; One Touch
  Driving combines independently selectable assists; screen narration; communication TTS/STT.
  Turn 10 built the blind driving assists with repeated feedback from Brandon Cole and other blind
  and low-vision players, and warns of a learning curve.
  [Xbox Wire](https://news.xbox.com/en-us/2023/04/27/forza-motorsport-accessibility-features-blind-driving/),
  [Can I Play That? preview](https://caniplaythat.com/2023/09/20/forza-motorsport-accessibility-preview/).
  Later patch notes fixed cue previews and pit cues, so test previews and pit lanes too.
  [Update 5 fixes](https://forza.net/news/forza-motorsport-update-5-fixes).
- STAR WARS: Squadrons: remapping across menus, hangar and flight; keyboard and mouse, joystick and
  HOTAS; off-screen indicators; ship outlines; communication alternatives.
  [EA](https://www.ea.com/amp/news/star-wars-squadrons-accessibility). Combat flight, not civil sims.
- Microsoft Flight Simulator (2020): assistance presets and individual assists, cockpit interaction
  alternatives, AI help; the page notes incomplete narration and imperfect AI.
  [Official](https://www.flightsimulator.com/accessibility/). 2024 has separate documentation:
  [MSFS 2024](https://www.flightsimulator.com/accessibility-msfs-2024/).
- Battlefield 2042: remapping across on-foot, vehicle, gunner and pilot contexts.
  [EA](https://www.ea.com/able/resources/battlefield-2042/pc).
- Rocket League: camera shake, FOV and distance settings; Free Play training with game speed.
  [Camera guide](https://www.epicgames.com/help/c-37599050/c-7261959845275/a14443106?lang=en-US).
  Training speed is not competitive slow motion.
- Forza Horizon 5: the game-speed setting is documented as offline only.
  [Xbox Wire](https://news.xbox.com/en-us/2021/11/04/gaming-for-everyone-the-accessibility-features-of-forza-horizon-5/).
- Farming Simulator 25: GPS steering and AI workers for field vehicles.
  [Giants](https://www.farming-simulator.com/newsArticle.php?country=us&lang=en&news_id=573).

## Verify

- [ ] Racing: start a race, enter and leave the pit, recover from a collision and complete a lap,
      once with each assist alone and once with all assists on.
- [ ] Flight: remap flight and menu controls, reach a waypoint, recover from a spin or stall, land
      or dock, then suspend and resume.
- [ ] Each assist changes only its own behaviour. Player input can override it at any time.
- [ ] Digital-only steering and throttle complete the same route.
- [ ] Remapping covers menus, garage or hangar, every seat, and entering and leaving vehicles.
      Prompts show the current bindings.
- [ ] Per-axis sensitivity, dead zone, curve and inversion each change only their own axis, with a
      working preview.
- [ ] Shake at 0 removes engine, speed, collision and scripted crash shake in every camera view.
- [ ] Horizon lock keeps world-up during banking, barrel rolls and crashes, in cockpit and chase.
- [ ] With speed effects reduced, speed is still readable from the HUD and audio.
- [ ] If nonvisual cues ship: preview each cue, then complete a lap or approach using cues only,
      including pit entry and recovery.
- [ ] Changing an assist mid-race applies without restart and persists after relaunch.
- [ ] Results screens, saves and achievements are identical with assists on and off.
- [ ] Ranked or shared modes have a recorded session-rule decision for each gameplay assist.

## Common mistakes

- One "Easy driving" toggle that bundles steering, braking and gears → independent assists, with
  an optional editable preset.
- Assuming automatic gears solves the motor barrier → steering and throttle still overlap; offer
  steering and throttle assists too.
- Remapping driving but not menus, hangar or vehicle exit → remap every context.
- Shake option that misses engine rumble or crash cutscenes → route all shake through
  CameraComfort.
- Racing line that is red/green only → add shapes through ColorTokens.
- Hiding checkpoints when guidance is off → hazards and gates are not guidance.
- Promising "playable without sight" after adding a few beeps → full nonvisual driving is an
  Advanced scope decision; test full laps with blind players before claiming it.
- Offline game speed silently ignored online → say where it does not apply.
- Rewind or reset that marks the result or blocks achievements → forbidden (core-policy section 2).
