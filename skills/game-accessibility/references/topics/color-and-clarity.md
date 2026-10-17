# Colour and clarity

**Load when:** colour conveys state (S004: teams, rarity, keys and locks, damage types, good/bad);
state shown only by shape, position or animation (S005); dark, foggy or low-contrast worlds (S063);
stacked effects that hide targets (S060); heatmaps and data overlays (S094); local players told apart
by colour (S127); grid or target overlays (S147); card suits and statuses (S149); stealth states
(S046). **Skip when:** never skip V25. Every game with visuals is checked in grayscale. Skip palettes
(V23, V24) only when no colour carries meaning, and V35/V36 when there are no dark scenes or HDR output.

Text size, fonts and themes: [text-and-readability](text-and-readability.md). Flash and pattern safety
of any high-contrast mode: [photosensitivity](photosensitivity.md). Guidance paint and trails:
[sensory-load-and-guidance](sensory-load-and-guidance.md).

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| V25 | Redundant symbols, labels or patterns for every colour meaning | — (always on) | On | B | V, C |
| V23 | Protan / deutan / tritan palette presets | `visual.color_profile` | `none` | C | V |
| V24 | Custom colour per semantic role | `visual.color_profile` = `custom` + `visual.custom_colors` | Designer colours, `""` | C | V |
| V26 | Enemy / ally outlines | `visual.outline_enemies`, `visual.outline_allies` (umbrella `visual.high_contrast`) | `follow`; umbrella Off, OS increase-contrast as its first value | C | V |
| V27 | Interaction highlights | `visual.outline_interactables` (umbrella `visual.high_contrast`), `guidance.highlights` | `follow` / `follow` | C | V, C |
| V28 | Traversable platform and edge outlines | `visual.outline_hazards` (umbrella `visual.high_contrast`) | `follow` | C | V |
| V29 | Background dimming / foreground isolation | `visual.background_dim` (0–0.75) | 0.0 | C | V, C |
| V35 | Brightness / gamma | `visual.brightness` | 1.0 | C | V, N |
| V36 | HDR peak and paper-white comfort | — | Bounded output | C | V, N, P |

`visual.high_contrast` is the umbrella for outlines only (V26–V28). Each outline class has its own
`follow` / `off` / `on` key: `visual.outline_enemies`, `visual.outline_allies`,
`visual.outline_interactables` and `visual.outline_hazards` (hazards, ledges and platform edges).
`follow` uses the umbrella; `on` or `off` overrides it, so a player can outline enemies but not
interactables, or keep hazard edges with everything else off. Add a class key whenever the game
has that class; never ship only the umbrella. Background dimming is its own key,
`visual.background_dim`, so a player can have outlines without a darker world, or the reverse.

## How to implement

### Semantic ColorTokens with paired symbols (V25)

- Every meaningful colour is a **token** (`team.ally`, `team.enemy`, `hazard`, `heal`, `poison`,
  `pickup.rare`, `wire.a`, `route`). Gameplay and UI scripts never hold literal colours for meaning.
- Each token has a **paired symbol that is always shown**: shape, icon, letter, number, texture,
  outline style or position. Meaning must survive grayscale. Example: ally = blue circle, enemy = orange
  diamond, neutral = grey square.
- Cover every place the meaning appears: world materials, particles, minimap, map, HUD bars, damage
  numbers, chat names, tooltips, scoreboards, tutorial art and icons.
- Patterns used as symbols must be calm: no fine high-contrast stripes, and nothing that scrolls or
  oscillates (see [photosensitivity](photosensitivity.md)).
- Review step: grep gameplay and UI code for literal colour constructors used for state. Each one is
  either decoration or a missing token.

### Palette presets and custom colours (V23, V24)

- `visual.color_profile` swaps the **token palette per element**. It never tints the whole frame and
  never changes skin, lighting or scenery that carries no meaning.
- Start presets from the Okabe–Ito set (table below), then check every token pair against the real
  backgrounds it appears on. A palette is a starting point, not a guarantee.
- `custom` lets the player pick a colour per semantic role, with a live preview of real gameplay
  objects (enemy, ally, pickup, hazard) on a real scene. A swatch row alone is not a preview.
- Custom choices persist in `visual.custom_colors` as `role=#RRGGBB` pairs separated by semicolons,
  for example `team.ally=#0072B2;team.enemy=#E69F00`. Roles missing from the string use the
  designer colour. On load, ignore unknown roles and invalid values one at a time instead of
  discarding the whole string, so the choice survives a revised build.
- Label menu entries by what they do and show the preview, for example "Red–green set (protan)". No
  "cure" or "fix" wording. Symbols stay on in every profile.

### Full-screen filters: optional extra only

- A whole-screen daltonisation or correction filter is **at most an extra preference**. It changes
  everything, can lower text contrast, and cannot recover meaning that was never encoded.
- "Daltonize" is not one validated algorithm. If you ship one, name the algorithm and parameters,
  preview it, and keep it independent of the per-element palette.
- Unreal's `SetColorVisionDeficiencyType` takes severity 0–1 while Lyra's menu shows 0–10. Normalise
  deliberately; do not pass the menu value straight through
  ([Epic API](https://dev.epicgames.com/documentation/en-us/unreal-engine/BlueprintAPI/Widget/Accessibility/SetColorVisionDeficiencyType)).
- Cautionary note: in January 2024, Tekken 8's colour-blind filters drew expert and player objections
  about striped patterns causing discomfort
  ([Ars Technica](https://arstechnica.com/gaming/2024/01/tekken-8s-colorblind-mode-is-causing-migraines-vertigo-and-debate/)).
  Do not assume current builds behave the same. The lesson is general: test every accessibility
  rendering path for flashes and patterns too.

### Developer simulation (not a player option)

- Simulation approximates how distinctions may be lost. It is a **QA tool**, never a player setting
  and never proof of access.
- Models: Brettel, Viénot & Mollon 1997 ([DOI](https://doi.org/10.1364/JOSAA.14.002647)); Viénot,
  Brettel & Mollon 1999 for fast red–green
  ([DOI](https://doi.org/10.1002/(SICI)1520-6378(199908)24:4%3C243::AID-COL5%3E3.0.CO;2-3)); Machado,
  Oliveira & Fernandes 2009, severity-dependent ([PubMed](https://pubmed.ncbi.nlm.nih.gov/19834201/)).
  Use Brettel, not Viénot, for tritan.
- Reference code: [libDaltonLens](https://github.com/DaltonLens/libDaltonLens) (Unlicense, C) and
  [DaltonLens-Python](https://github.com/DaltonLens/DaltonLens-Python) (MIT) as an image oracle;
  [explanation](https://daltonlens.org/understanding-cvd-simulation/). Godot editor simulation:
  [paulloz/godot-colorblindness](https://github.com/paulloz/godot-colorblindness) (MIT; current
  release targets 4.6, so check compatibility with your engine version first).
- Shader correctness: matrices apply to **linear** RGB. Decode sRGB exactly once, keep alpha
  separate, watch row/column-major order, and test greys, primaries, black and white.
- Web builds can use Chrome DevTools or Puppeteer `emulateVisionDeficiency`
  ([Chromium](https://developer.chrome.com/docs/chromium/cvd)). It does not inspect native renders.

### Contrast (text, icons, UI states)

- Measure with the WCAG formula: `(L_lighter + 0.05) / (L_darker + 0.05)`, where L is relative
  luminance from **linearised** sRGB channels. Do not average encoded RGB. Do not round 4.499 up.
- Check pairs with the bundled script:

  ```
  python scripts/contrast_check.py "#RRGGBB" "#RRGGBB"
  ```

  Run it for every token pair the theme defines (text on panel, icon on HUD bar, focus ring on button,
  disabled label on panel), in default, palette and high-contrast modes.
- For translucent panels and world-space text, measure the **composited** pixels over the least
  favourable local background (snow, sky, explosions), not the screen average.
- High contrast is a measured target, not a theme name. Some players prefer softer contrast: keep that
  as a separate choice (V15 in [text-and-readability](text-and-readability.md)).

### High-contrast outlines and background dimming (V26–V29)

- Implement as a render feature driven by tags: `enemy`, `ally`, `interactive`, `hazard`,
  `traversable`. Outline colours come from ColorTokens and keep their paired symbols.
- Each tag reads its class key through the follow resolver (architecture section 1):
  `resolve_toggle("visual.outline_enemies", high_contrast)`, and the same for allies,
  interactables and hazards (`hazard` and `traversable` tags both use `visual.outline_hazards`).
  Turning the umbrella on or off changes only classes still set to `follow`. Nothing here dims
  anything.
- Background dimming (V29, `visual.background_dim`) desaturates or darkens non-tagged scenery by
  the chosen amount. Hazards, landing geometry, depth cues and projectiles stay at full clarity.
- Outlines must not hide landing surfaces (V28) or replace recoil and telegraph animation.
- High contrast can raise overall brightness and create stripes. Test it together with
  `motion.reduce_flashing` and in the brightest scenes.
- Import the OS "increase contrast" preference as the first value of `visual.high_contrast` only
  until the player sets it.

### Dark scenes, brightness and HDR (V35, V36)

- Calibrate `visual.brightness` on a **real dark scene** with a reference object that should be
  barely visible. Brightness changes do not replace contrast or outlines.
- Darkness used as a mechanic still needs another route to essential information: a light source,
  an outline, a sound with a caption, or a HUD indicator.
- HDR: offer peak brightness and paper-white controls. Do not claim any brightness level is safe for
  everyone. Validate SDR and HDR outputs separately.

## Numbers that matter

| Source | Number | Scope |
|--------|--------|-------|
| [XAG 102](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/102) | **4.5:1** | Standard text and important visual elements |
| XAG 102 | **3:1** | Large elements and inactive text (WCAG exempts inactive controls) |
| XAG 102 | **7:1** | UI elements in a high-contrast mode |
| XAG 102 | Large text = 52 px at 1080p (console); 36 px at 1080p (PC/VR) | Defines when 3:1 applies |
| [WCAG 1.4.3](https://www.w3.org/WAI/WCAG22/Understanding/contrast-minimum.html) | 4.5:1 text; 3:1 large text (18 pt, or 14 pt bold) | Web AA |
| WCAG 1.4.6 | 7:1 text; 4.5:1 large text | Web AAA |
| [WCAG 1.4.11](https://www.w3.org/WAI/WCAG22/Understanding/non-text-contrast.html) | 3:1 | Control boundaries, states and meaningful graphics |

The March 2025 AGI PDF prints "4:5:1". Treat that as a typo for 4.5:1, which Microsoft's own test
steps and XAG 102 use.

Okabe–Ito categorical palette ([source](https://jfly.uni-koeln.de/color/)). Keep symbols and labels
too; no palette is safe for every vision difference or every background.

| Name | Hex | Name | Hex |
|------|-----|------|-----|
| Black | `#000000` | Blue | `#0072B2` |
| Orange | `#E69F00` | Vermilion | `#D55E00` |
| Sky blue | `#56B4E9` | Reddish purple | `#CC79A7` |
| Bluish green | `#009E73` | Yellow | `#F0E442` |

## Shipped examples

- Battlefield 2042: team, enemy and HUD colour choices plus minimap opacity.
  [EA](https://www.ea.com/able/resources/battlefield-2042/pc).
- Destiny 2: targeted-reticle colour options.
  [Bungie](https://www.bungie.net/7/en/News/Article/destiny-2-accessibility-set). Dated seasonal notes;
  not a current menu map.
- Life is Strange: True Colors: colour-vision options for its emotion auras.
  [Xbox Wire](https://news.xbox.com/en-us/2021/09/17/making-empathy-accessible-life-is-strange-true-colors/).
- The Last of Us Part II: high-contrast display mode.
  [Naughty Dog](https://www.naughtydog.com/blog/the_last_of_us_part_ii_accessibility_features_detailed).
- Marvel's Spider-Man 2: high-contrast shaders with per-class options.
  [Insomniac](https://support.insomniac.games/hc/en-us/articles/46730041467027-What-Accessibility-options-does-Marvel-s-Spider-Man-2-feature).
- Eagle Island: background dimming and outlines, used as the
  [XAG 102](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/102)
  illustration. Contrast still needs checking per background and state.
- Age of Empires IV (PC): documentation points players to Windows colour filters.
  [Age of Empires](https://www.ageofempires.com/age-iv-accessibility/). An OS filter is not in-game
  semantic colour support.
- Wordle: an orange/blue high-contrast option, yet a browser bug shows forced-colour mode can still
  fail ([Mozilla bug](https://bugzilla.mozilla.org/show_bug.cgi?id=1996950)).
- Marvel Rivals: a player reports custom health colours resetting after updates
  ([Reddit](https://www.reddit.com/r/rivals/comments/1vmixsr/accessibility_settings_resetting_every_single/)).
  Current bug status UNVERIFIED; the lesson is that persistence is part of colour access.
- VALORANT: colour alternatives are disclosed ([Riot](https://www.riotgames.com/en/accessibility));
  flash reduction is UNVERIFIED. Colour support is not evidence of other visual safety.

## Verify

- [ ] Capture the same fixed scenes (HUD, combat, map, puzzle, menus) in colour, grayscale and
      protan, deutan and tritan simulation. In every image, each meaningful state is still told apart
      by symbol, label, pattern or position. Record the tool and algorithm used.
- [ ] Repeat the captures with each `visual.color_profile` value. The new palette reaches world
      materials, particles, minimap, map, HUD, scoreboard and icons.
- [ ] Run `python scripts/contrast_check.py` for every theme token pair. Text meets 4.5:1, large text
      and non-text UI meet 3:1, high-contrast mode meets 7:1.
- [ ] Measure composited captions and tooltips over the brightest and busiest scenes.
- [ ] Turn on `visual.high_contrast` in a busy encounter: enemies, allies, interactables and hazards
      are outlined and the background is not dimmed. Then set `visual.background_dim` with outlines
      off: scenery darkens and hazards stay visible. Test both together.
- [ ] With the umbrella on, set one class key to `off` (for example `visual.outline_interactables`):
      only that class loses its outline. With the umbrella off, set one class to `on`: only that
      class is outlined. Toggle the umbrella afterward: the overridden class keeps its value. Repeat
      for each class key the game ships, and after restart.
- [ ] Set custom colours, restart, and load an old save: `visual.custom_colors` is restored and
      every role uses the chosen colour.
- [ ] Combine high contrast with `motion.reduce_flashing`: no new stripes, strobes or bright pulses.
- [ ] Brightness calibration screen works on a real dark scene; dark-mechanic information has a
      non-visual or outlined route.
- [ ] Palette, high contrast and brightness persist across restart and update, and are not stored in
      story saves.
- [ ] No simulation shader is exposed as a player option.

## Common mistakes

- A colour-blind filter as the whole answer → symbols first, per-element palette second, filter last.
- Shipping a developer simulation shader as a "colour-blind mode" → simulation is QA only.
- Palette changes the menu but not particles, minimap or puzzle pieces → enumerate every token use.
- Red/green for good/bad with no icon → add a symbol or label that survives grayscale.
- Contrast checked against a flat menu → check the least favourable real background.
- Averaging RGB or rounding 4.49 up to 4.5 → use the WCAG luminance formula exactly.
- "High contrast" named but not measured → measure 7:1 for the mode's UI.
- Outlines that hide landing edges or telegraphs → keep geometry and animation readable underneath.
- Diagnosis-only menu labels with no preview → describe the effect and show real objects.
