# Photosensitivity

**Load when:** always. Any visual output (S001) gets the Baseline assessment. Also flashing, strobing
or rapid exposure changes (S058), patterned textures or rapid cuts (S059), stacked effects and bloom
(S060), pulsing damage overlays (S057), video or generated visual content (S065), beat-synced visual
feedback (S072), user, mod or procedural content (S122, S132). **Skip when:** never. A game with no
intentional flashing still gets a startup and transition capture; "we have no strobes" is not
evidence.

Colour filters and high-contrast modes are checked here too:
[color-and-clarity](color-and-clarity.md). Store wording:
[store-claims-and-compliance](store-claims-and-compliance.md).

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| P01 | No hazardous flashing in any mode, plus a global "reduce flashing" | `motion.reduce_flashing` | **Safe: no hazards in any mode**; reduction off; OS reduce-motion as first value | B | P |
| P02 | Muzzle-flash suppression | `motion.reduce_flashing` | Within safe bounds | C | P |
| P03 | Lightning and strobe replacement | `motion.reduce_flashing` | Safe; static or subtle alternative | C | P |
| P04 | Damage and full-screen flash replacement | `motion.reduce_flashing` | Safe; steady edge or status cue | C | P, V |
| P05 | Explosion and ability flash intensity | `motion.reduce_flashing` | Safe; bounded output | C | P |
| P06 | Saturated-red flash desaturation | — (FlashGuard, always) | On in every mode | C | P |
| P07 | Pattern and stripe reduction | — (asset rule) | Non-provocative patterns by design | C | P |
| P08 | Glitch and flicker effect disable | `motion.reduce_flashing` | Safe; static alternative | C | P, N |
| P09 | Gentle scene transitions | `motion.reduce_flashing` | No rapid bright/dark cuts | C | P |
| P10 | Suppress flashing player-made cosmetics | — | Bounded by FlashGuard | C | P |

Every row sits **on top of** hazard prevention. A "normal" mode with known hazardous sequences is
never justified by offering a reduced mode.

## How to implement

### FlashGuard: every flash goes through one gate

- All full-screen flashes, strobes, lightning, hit flashes, muzzle flashes, explosion bursts, UI
  blinks, glitch effects and transitions call `FlashGuard.request_flash(intensity, area)`. No effect
  writes screen brightness directly.
- **Counting rule (every engine sample uses it):** a request counts toward the budget if it covers
  **≥ 25% of the screen**, **or** changes relative luminance by **≥ 0.1**, **or** is **saturated
  red**. A request whose area or luminance change is unknown counts. Only a request known to be
  below all three is exempt.
- **Rule in every mode:** counted flashes are limited to **3 in any 1-second window and 9 in any
  5-second window**. These are hard caps: configuration may lower them but never raise them. Use
  sliding windows over real time, not whole-second buckets. A flash is a pair of opposing changes
  (bright then dark).
- The 5-second limit matters. Two or three flashes every second for five seconds stays under the
  1-second cap, yet EA IRIS treats 2–3 flashes per second for five consecutive seconds as an
  extended failure ([IRIS](https://github.com/electronicarts/IRIS)). The 5-second budget stops a
  sustained 2–3 Hz strobe.
- The 25%-of-screen and 0.1 thresholds are **conservative sample defaults**, not a WCAG
  equivalence. Tune them only with evidence from footage analysis of the real effects. WCAG's
  general-flash area is 25% of any 10° visual field, which it estimates as a 341 × 256 px
  rectangle on a 1024 × 768 display: roughly 22,000 px, under 3% of that screen and far smaller
  than 25% of the screen. A flash below the sample's area threshold can still be hazardous, so the
  luminance and red tests apply on their own, unknown area always counts, and captured footage
  remains the real check. The budget is **global**: two emitters that are each fine can combine
  into a hazard.
- Over budget: drop the request, or merge it into a held steady state. Never queue flashes to play
  later.
- **Saturated red:** FlashGuard desaturates saturated-red flashes (one state with R/(R+G+B) ≥ 0.8,
  computed on linearised channel values as in WCAG's relative-luminance definition, not on encoded
  sRGB values) in every mode, and always counts them against the budget whatever their area.
- `motion.reduce_flashing` on: convert flashes to **steady, low-contrast fades**, a static edge tint,
  or a small icon change. Remove decorative flicker entirely. Keep the information: a hit still shows,
  a shot still reads, through a non-flashing cue.
- The flag is read live. Late-spawned emitters ask FlashGuard; they do not cache the value.
- **FlashGuard is one prevention layer, not proof of safety.** It cannot see flashes baked into
  video, textures, animated materials or overlapping shaders, and it does not judge spatial
  patterns. Photosensitivity verification requires analysing captured footage (Analysis workflow
  below). If no analyser can run, the result is `blocked`, never `pass`.

### Inventory every source

Check each, in normal and reduced modes:

- Boot and publisher logos, legal screens, menus, loading screens and their transitions.
- Pre-rendered cutscenes and trailers in game. Video cannot pass through FlashGuard: analyse it, then
  re-edit or ship a reduced-flash variant, and keep it skippable.
- Lightning, fire, sparks, muzzle flashes, explosions, ability VFX, hit flashes, low-health pulses.
- Glitch, scanline and flicker post effects; rhythm and beat-synced lights.
- Scene cuts between bright and dark shots; camera shake combined with exposure changes; auto-exposure
  and bloom; HDR output.
- Blinking UI: cursors, warnings, notification badges, "press start".

### Patterns and stripes (P07)

- Avoid regular high-contrast stripes, grids and checkerboards over large areas, especially when they
  move, oscillate, flash or reverse contrast. Replace the texture; dimming the screen is not a fix.
- This includes accessibility rendering: high-contrast modes and colour filters. Tekken 8's
  colour-blind filters drew January 2024 objections over striped patterns
  ([Ars Technica](https://arstechnica.com/gaming/2024/01/tekken-8s-colorblind-mode-is-causing-migraines-vertigo-and-debate/));
  do not assume current builds behave the same.

### User-generated and procedural content (P10)

- Route cosmetics, emotes, sprays and modded effects through FlashGuard and clamp their frequency and
  luminance. Offer local substitution of other players' flashing cosmetics.
- State coverage honestly. A filter cannot promise that all user content is safe.

### Analysis workflow

1. List effect producers and write capture scenarios: startup, menus, each transition, worst-case
   combat combinations, repeated player-triggered actions, every cutscene.
2. Capture the **final composited output** at supported resolutions and frame rates. Record build,
   settings, resolution, frame rate, tone mapping and SDR/HDR.
3. Capture normal and `reduce_flashing` modes.
4. Analyse with a pinned tool and configuration:
   - [EA IRIS](https://github.com/electronicarts/IRIS): BSD-3-Clause; flags luminance flashes,
     saturated-red flashes and spatial patterns; console example writes CSV results. An
     [Unreal plugin](https://github.com/electronicarts/IRIS-Unreal-Plugin) exists. **Not a
     certification.** Inspect the example's real interface for your pinned version; do not invent CLI
     flags or exit codes.
   - [HardingFPA](https://www.hardingfpa.com/hardingfpa-for-games/): commercial. "Passes Harding"
     needs the tool version, profile, capture conditions and footage named.
   - [PEAT](https://trace.umd.edu/photosensitive-epilepsy-analysis-tool-peat-user-guide/): its guide
     says it was not designed for commercial video or game analysis. Do not use it as the game check.
5. "Tool did not run" is **blocked**, never pass. Investigate every flagged range, fix the source
   effect, recapture the same scenario and its neighbours.
6. Keep captures, reports and metadata. One recording covers only that recording.

HDR caveat: an SDR capture analysed successfully does not validate HDR output. Confirm the tool's
supported colour pipeline before claiming HDR coverage.

**Never show photosensitive testers known hazardous footage** to confirm a finding. Fix and analyse
first. Testers must be able to avoid exposure.

### Warnings and wording

- A warning screen is extra information, **not mitigation**. A warning shown after the flash is not a
  warning. Generic warnings mainly protect liability
  ([Laura Kate Dale](https://access-ability.uk/2024/11/29/generic-photosensitivity-warnings-protect-liability-not-users/)).
- Specific advance notices are useful for content that remains intense (for example "bright
  lightning in chapter 3"), with options to reduce or skip.
- Option text: "Reduce flashing: replaces flashes with steady effects. The game avoids hazardous
  flashing even when this is off." Public text: "Reduces flashing effects. Analysed with <tool,
  version>." Never "seizure safe", "epilepsy safe" or "seizure free".

## Numbers that matter

WCAG 2.2 SC 2.3.1, Level A ([definition](https://www.w3.org/TR/WCAG22/#dfn-general-flash-and-red-flash-thresholds),
[understanding](https://www.w3.org/WAI/WCAG22/Understanding/three-flashes-or-below-threshold.html)):
content passes if it never has more than three flashes in any one-second period, **or** the flashing
stays below the general and red flash thresholds.

| WCAG 2.2 term | Exact value |
|---------------|-------------|
| Frequency | No more than 3 general and/or 3 red flashes in any 1 s |
| Area | Combined flashing area ≤ 0.006 steradians within any 10° visual field (about 25% of that field at typical distance) — **not** 25% of the whole screen |
| General flash | Opposing changes of ≥ 10% of max relative luminance, where the darker state is below 0.80 |
| Red flash | One state has R/(R+G+B) ≥ 0.8 and chromaticity difference > 0.2 in CIE 1976 UCS |
| Fine pattern exception | Balanced elements smaller than 0.1° per side |
| Desktop illustration | 341 × 256 px in a 1024 × 768 screen ≈ 10° at desktop distance; not a TV, phone or headset rule |

SC 2.3.2 (AAA) drops the area exception: no more than three flashes per second at all.
[XAG 118](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/118)
still documents an older red formula; do not label it as WCAG 2.2's.

ITU-R BT.1702-3, November 2023 ([PDF](https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.1702-3-202311-I%21%21PDF-E.pdf)):

| Criterion | Value |
|-----------|-------|
| Flash, darker state < 160 cd/m² (SDR/HDR) | Difference ≥ 20 cd/m² |
| Flash, darker state ≥ 160 cd/m² (HDR only) | Michelson contrast > 1/17 |
| Red | Saturated-red transitions can be harmful regardless of luminance |
| Prohibited sequence | Area > 25% of the screen **and** > 3 flashes in any second |
| Leading-edge separation accepted | ≥ 360 ms at 50 Hz; ≥ 334 ms at 60 Hz |
| Prolonged sequences | > 5 s may remain risky even within the limits |
| Patterns | > 5 light/dark stripe pairs, and area > 40% if stationary or > 25% if moving, oscillating, flashing or reversing |

EA IRIS documented rules: > 3 flashes per second in any second fails; 2–3 flashes per second for five
consecutive seconds is an extended failure; harmful patterns lasting ≥ 0.5 s are flagged.

## Shipped examples

- Celeste: the changelog records Photosensitive Mode fixes.
  [changelog](https://www.celestegame.com/changelog.html). Evidence of the mode's maintenance, not of
  medical safety.
- Stardew Valley: a "Show Flash Effects" option ([wiki, community source](https://wiki.stardewvalley.net/Options)).
  A toggle, not a safety guarantee.
- Life is Strange: True Colors: optional brightness and volume event warnings.
  [Xbox Wire](https://news.xbox.com/en-us/2021/09/17/making-empathy-accessible-life-is-strange-true-colors/).
  Warning is not reduction.
- Life is Strange: Double Exposure: sensory warnings with optional pause before intense events, per a
  consultant who disclosed involvement.
  [Access-Ability](https://access-ability.uk/2024/10/24/touring-accessibility-in-life-is-strange-double-exposure/).
- Tekken 8: cautionary, above.
- VALORANT: flash reduction UNVERIFIED; colour alternatives are not evidence of flash safety.
  [Riot](https://www.riotgames.com/en/accessibility).

## Verify

- [ ] Grep effect, shader, tween and UI code for direct brightness, modulate or exposure writes that
      bypass FlashGuard. Each one is routed or justified.
- [ ] Unit test FlashGuard: 4 large requests within 1 s (sliding, across a second boundary) yield at
      most 3 flashes; a sustained 2–3 Hz sequence of large requests for 6 s yields at most 9 flashes
      in any 5 s; repeated small-area requests with a luminance change of 0.4 are also capped;
      repeated small saturated-red requests come out desaturated and are capped; requests with
      unknown area count; configured budgets above 3/9 are clamped; reduce mode yields no
      alternating changes. A passing unit test does not replace footage analysis.
- [ ] Fresh profile: capture from process launch through logos, menus and first gameplay. Analyse with
      the pinned tool. Record tool, version, configuration and capture metadata. No analyser run
      means this check is `blocked`; inspecting footage by eye is not a pass.
- [ ] Capture and analyse worst-case combat stacking, repeated player-triggered effects and every
      cutscene, in normal and reduce modes. Flagged ranges are fixed and recaptured.
- [ ] With `motion.reduce_flashing` on, hits, shots and alerts are still understandable.
- [ ] High-contrast and colour-profile modes are captured and analysed too.
- [ ] HDR is either analysed in its own pipeline or recorded as not tested.
- [ ] The reduce option is reachable on the first screen, before any logo, and persists.
- [ ] No release text claims "seizure safe" or similar.

## Common mistakes

- A warning screen instead of fixing the effect → remove hazards in every mode; warnings are extra.
- Testing only gameplay → logos, menus, transitions and cutscenes flash too.
- Per-emitter limits → one global budget; combined effects count.
- Counting flashes per whole second → use a sliding 1-second window.
- Only a 1-second cap → also cap 9 flashes in any 5 s, so a sustained 2–3 Hz strobe cannot pass.
- FlashGuard unit test passes, so the game is called safe → analyse captured footage or report `blocked`.
- 25% of the whole screen as the WCAG area rule → WCAG uses a 10° field; ITU uses screen area.
- Reducing flashes by darkening the whole frame → change the effect at its source.
- Treating an IRIS pass as certification → it is evidence for that capture only.
- Re-testing on photosensitive players to "confirm" → never expose them to known hazards.
