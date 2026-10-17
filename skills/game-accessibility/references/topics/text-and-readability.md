# Text and readability

**Load when:** any text or meaningful symbol (S003), menus that need focus (S002), text-heavy UI (S073),
animated or typewriter text (S074), world text or handwriting (S064), several locales (S079), icons
without labels (S080), small cursor or touch targets (S021), input prompts (S031), aiming reticles
(S033), image-based journals or maps (S085). **Skip when:** never skip completely. A game with any menu
gets the Baseline rows. Skip only the rows whose signal is confirmed absent: reticle rows without
aiming, cursor rows without a pointer, world-text rows without diegetic text.

Subtitle presentation lives in [audio-and-captions](audio-and-captions.md). Contrast ratios and colour
live in [color-and-clarity](color-and-clarity.md). Spoken menus live in
[screen-reader-and-voice](screen-reader-and-voice.md).

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| V01 | Menu text scale | `text.scale` | 100%; OS text size as first value | B | V, C, S |
| V02 | HUD text scale | `text.scale` | 100% | C | V, S |
| V03 | Readable font alternative (plain, dyslexia-friendly) | `text.font` | `default` (must itself be readable) | B | V, C |
| V04 | Font weight | — | Designer weight | C | V |
| V05 | Sentence case instead of all-caps paragraphs | — | On (authored that way) | C | C, V |
| V06 | Letter spacing | — | Designer value | C | C, V |
| V07 | Word spacing | — | Designer value | C | C, V |
| V08 | Line spacing | `text.line_spacing` | 1.0 | C | C, V |
| V09 | Paragraph width (line length) | — | 80 characters or fewer | C | C, V |
| V10 | Text alignment | — | Start-aligned, not justified | C | C, V |
| V11 | Text foreground colour | — | Designer; OS contrast as first value | B | V, N |
| V12 | Text background colour | — | Designer; OS contrast as first value | B | V, N |
| V13 | Text panel opacity | `text.background_opacity` | 75% (100% available) | C | V, C |
| V14 | Light / dark / high-contrast UI theme | — | Designer theme | B | V, N |
| V15 | Low-contrast reading theme | — | Off | C | V, N |
| V16 | UI element scale (icons, buttons, prompts) | `visual.ui_scale` | 100% | B | V, M |
| V17 | HUD element repositioning | — | Designer layout | C | V, M |
| V18 | HUD margins / safe area | — | Platform safe area | C | V, S |
| V19 | Cursor size | `visual.ui_scale` | 100% | C | V, M |
| V20 | Cursor colour / outline | — | Designer | C | V |
| V21 | Reticle size, shape, thickness | — | Designer | C | V |
| V22 | Reticle colour | — | Designer | C | V |
| V30 | World-text readable overlay | — | On when diegetic text exists | C | V, C |
| V31 | Magnifier / zoom inspection | — | Available, off until used | C | V |
| V32 | Persistent visible focus | — (always on) | On | B | V, C, M |
| V33 | Focus indicator style / thickness | — | Designer style | C | V, N |
| V34 | Critical indicator near the centre | — | Off | C | V, C |
| C01 | Player-paced and instant text | `text.reveal`, `text.auto_advance` | `normal`; auto-advance off | B | C, H, V |

A `—` key means the shared registry has no key yet. Name a project key in `ACCESSIBILITY.md` and keep
it in the same settings store. Do not invent a second store.

## How to implement

All text options are read from `A11ySettings` and applied live through `changed(key)`. Late-spawned UI
reads the current value on creation, then subscribes.

### Size and scaling (V01, V02, V16, V19)

- Put sizes in **theme tokens** (body, small, heading, prompt, HUD). Multiply the tokens by
  `text.scale`. Multiply icon, button, glyph and cursor sizes by `visual.ui_scale`. Do not scale a
  root transform: it blurs text and moves hit areas away from what is drawn.
- **Reflow, never clip.** Containers grow, wrap or scroll. Tooltips, error dialogs, confirmation
  boxes, input prompts, inventory descriptions and the settings screen itself all scale.
- **Never auto-fit text downward.** Auto-shrink silently undoes the player's choice. If text cannot
  fit, wrap or scroll.
- Hit areas grow with the art (V16). A larger icon with the old touch area is not an improvement.
- Keep subtitle size separate (`subtitles.size`). Players often want large subtitles and normal HUD.
- Import the OS text-size preference only while the player has not set `text.scale` by hand. Unknown
  is not false.
- Measure **rendered glyph height** in a screenshot at the target resolution. An engine font-size
  field is not the visible height.

### Fonts (V03, V04, V05)

- The **default font must already be readable**. A decorative title font is fine for titles; body
  text, numbers and prompts use a plain face. A settings page does not excuse a tiny stylised default.
- `text.font` values: `default`, `readable` (a high-legibility sans such as Atkinson Hyperlegible),
  `dyslexia_friendly` (for example OpenDyslexic or Lexend). Apply the choice to menus, HUD, dialogue,
  journals and subtitles.
- **Describe fonts honestly.** Wery and Diliberto (2017) found no speed or accuracy gain from
  OpenDyslexic for pupils with dyslexia
  ([PMC5629233](https://pmc.ncbi.nlm.nih.gov/articles/PMC5629233/)). Kuster et al. (2018) found no
  reading benefit from Dyslexie, a different font
  ([PMC5934461](https://pmc.ncbi.nlm.nih.gov/articles/PMC5934461/)). Offer these fonts as a **reading
  preference**. Never write "fixes dyslexia" or "dyslexia mode".
- Bold weight (V04) keeps open counters and spacing. Check `I l 1`, `O 0`, `rn m`, punctuation,
  currency, decimal separators and controller glyph names in every font.
- Author paragraphs in sentence case (V05). All-caps is fine for one- or two-word labels.

| Font | Licence to check | Honest claim |
|------|------------------|--------------|
| OpenDyslexic | SIL OFL 1.1 per [author site](https://opendyslexic.org/) and [repo](https://github.com/antijingoist/opendyslexic). Older downloads used other terms: check the licence shipped with your exact file. | Optional preference, not a treatment |
| Atkinson Hyperlegible (Google Fonts build) | OFL 1.1 ([OFL.txt](https://github.com/googlefonts/atkinson-hyperlegible/blob/main/OFL.txt)). The original 2020 Braille Institute download has its own [Atkinson Hyperlegible Font License](https://mirrors.ibiblio.org/CTAN/fonts/atkinson/doc/Atkinson-Hyperlegible-Font-License-2020-1104.pdf). Newer Next/Mono families need their own review. | Designed for distinguishable letterforms for low vision |
| Lexend | OFL 1.1 ([OFL.txt](https://github.com/googlefonts/lexend/blob/main/OFL.txt)); repo archived 2026-04-19, font still usable | Another reading preference; universal benefit not established |
| Noto families | Commonly OFL; confirm per family ([notofonts](https://github.com/notofonts)) | Script coverage and fallback |

"Installed on Windows" is not permission to redistribute a font.

### Spacing, line length and alignment (V06–V10)

- Layouts must survive the WCAG 1.4.12 overrides (table below) without overlap or clipping. That is a
  tolerance test, not a rule to make every default that loose.
- Keep long-text blocks (journals, codex, tutorials) to 80 characters per line, or 40 for CJK.
- Start-align body text (left for LTR, right for RTL). Avoid justified text and centred paragraphs.
- When line spacing grows, move hit regions and selection highlights with the lines.

### Text panels and themes (V11–V15)

- `text.background_opacity` controls the panel behind dialogue, tooltips and prompts. 100% must be
  reachable. Outline or shadow alone fails over busy scenes.
- Store text colour and background colour separately. Preview them over a real bright scene and a real
  dark scene, not a flat menu.
- A high-contrast theme (V14) and a softer reading theme (V15) can both be wanted. Some players find
  maximum contrast hard to read
  ([Cherry Thompson interview](https://a11yrules.com/podcast/cherry-thompson-talks-about-cognitive-impairments-and-web-accessibility/)).
  Ratios and checks: [color-and-clarity](color-and-clarity.md).

### Text reveal and pacing (C01)

- `text.reveal`: `instant` shows the whole line at once; `fast` and `normal` animate. The first press
  during a reveal completes the line. The next press advances. Never skip an unread line.
- `text.auto_advance` is off by default. Dialogue waits for the player. Text never silently expires.
- Jitter, wobble and bouncing letters stop when `motion.menu_animation` resolves to off (`off`, or
  `follow` with `motion.reduce_motion` on).
- Narration reads the complete line once. It never restarts per revealed character.

### HUD layout (V17, V18, V34)

- Keep essential HUD inside the platform safe area. Offer a margin slider when players use TVs with
  overscan or have a restricted field of view.
- Repositioning keeps focus order and never hides captions or prompts.
- V34 duplicates a time-limited peripheral warning (low health, incoming attack) near screen centre.
  It must not block the action. It is optional.

### Cursor and reticle (V19–V22)

- Cursor size follows `visual.ui_scale`. Add a contrasting outline so it stays visible on bright and
  dark scenes. Keep the hotspot obvious at every size.
- Reticle shape, size, thickness and colour are separate settings. Reticle colour must not reuse the
  team or enemy tokens. A reticle change must not remove recoil or spread information.

### World text and magnifier (V30, V31)

- Every sign, note, screen and inscription has a **localisation key**, not baked pixels. Interacting
  (or focusing) opens a plain-text overlay at the player's text size and font. The overlay is the
  source for narration (`narration.dialogue`).
- Add the read text to a log or codex so it can be re-read later.
- A magnifier or inspect zoom (V31) helps for maps, diagrams and item art. It must not force camera
  motion and must work with keyboard or controller.

### Focus (V32, V33)

- Focus is always visible on every focusable control. It does not rely on colour alone or on
  animation alone: use a thick outline, a shape change or a marker as well.
- Offer a static, high-contrast focus style (V33) for players who find pulsing focus hard to track.
- Each screen opens with a sensible initial focus. Dialogs trap focus until closed and return it to
  the control that opened them. Focus never lands on hidden or disabled-and-invisible elements.

### Localisation

- Ship font fallbacks that cover **every glyph in every shipped language**, including button names
  and player-entered names. Missing-glyph boxes are a readability bug.
- Test with pseudo-localised long strings, real translations, CJK and RTL where shipped. Longest remapped prompt plus largest scale is the worst case.

## Numbers that matter

| Source | Number | Scope |
|--------|--------|-------|
| [XAG 101](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/101) | Body text at least **26 px at 1080p** (52 px at 4K) on console | Rendered height, ascender to descender |
| XAG 101 | At least **18 px at 1080p** (36 px at 4K) on PC/VR | Not interchangeable with AGI's PC criterion |
| XAG 101 | Mobile: 18 / 36 / 72 px at 100 / 200 / 400 DPI | Scale linearly with DPI |
| XAG 101 | Scale to at least **200%** of the minimum default | No loss of content or function |
| XAG 101 | At most **80 characters** per line (40 CJK), excluding spaces | Blocks longer than two lines |
| XAG 101 (suggested spacing) | Line spacing 1.5; paragraph spacing at least **2 × line spacing**; letters 0.12 × font size; words 0.16 × font size | Suggested minimums when spacing is not configurable |
| [WCAG 1.4.12](https://www.w3.org/WAI/WCAG22/Understanding/text-spacing.html) (tolerance test) | Line height 1.5 × font size; paragraph spacing **2 × font size**; letters 0.12 × font size; words 0.16 × font size | Layout must tolerate these overrides without loss; not a required default |
| [AGI Clear Text](https://accessiblegames.com/wp-content/uploads/2025/03/Accessible-Games-Initiative-Tags-and-Criteria-March-2025.pdf) | Default height **17 / 26 / 52 px** at 720p / 1080p / 4K | PC and console store tag |
| AGI Large Text | Available height **25 / 38 / 76 px** at 720p / 1080p / 4K | Default or selectable |
| [WCAG 1.4.4](https://www.w3.org/WAI/WCAG22/Understanding/resize-text.html) | Resize to 200% without loss | Web criterion; principle transfers |
| [WCAG 2.5.8](https://www.w3.org/WAI/WCAG22/Understanding/target-size-minimum.html) | Pointer targets 24 × 24 CSS px minimum | Web criterion; useful floor for cursor UI |

Pixels at 1080p, CSS pixels, Android dp and engine font units are different units. Measure the final
output. Contrast numbers are in [color-and-clarity](color-and-clarity.md).

## Shipped examples

- Pentiment: an easy-read font removes some decorative fonts and writing effects, plus text scaling,
  high contrast and TTS. [official site](https://pentiment.obsidian.net/). Good model for an optional
  readable treatment that keeps the default art.
- Star Wars Outlaws: a readable typeface option among its accessibility settings.
  [Ubisoft spotlight](https://news.ubisoft.com/en-gb/article/4J5XWevEccm1LBSduyij2H/star-wars-outlaws-accessibility-spotlight).
- Cyberpunk 2077: patch 2.1 added larger interface text and HUD.
  [CD Projekt Red](https://www.cyberpunk.net/en/news/49591/update-2-1-accessibility-features). Version
  2.1 only; do not project onto older console builds.
- God of War Ragnarök: text and icon sizing.
  [PlayStation](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/).
- Marvel's Spider-Man 2: a low-vision review praises separate icon and prompt sizing but reports that
  the initial setup screen itself was hard to read.
  [Game Accessibility Nexus](https://www.gameaccessibilitynexus.com/blog/2023/10/16/marvels-spider-man-2-low-vision-review/).
- Starfield (launch, 2023): a large-font mode existed, but the reviewer still hit other barriers.
  [GamesRadar review](https://www.gamesradar.com/starfield-accessibility-verdict/). Cautionary: one
  font option is not whole-game readability. Launch-era build only.
- Persona 5 Royal: dialogue log and optional auto-advance, but the reviewer found no subtitle size
  increase. [Can I Play That?](https://caniplaythat.com/2020/04/02/deaf-hoh-review-persona-5-royal/).
- A Short Hike: a Pixel Size option reduces or removes pixelation.
  [developer FAQ](https://steamcommunity.com/app/1055540/discussions/0/1639792569850442961/).

## Verify

- [ ] Measure rendered body text at the minimum supported resolution. Record px at 1080p and compare
      with the platform target.
- [ ] At maximum `text.scale` and `visual.ui_scale`, visit every screen: title, settings, pause, HUD,
      inventory, journal, tooltips, error and confirmation dialogs, credits. Nothing clips, overlaps or
      leaves the safe area. Back and reset stay reachable.
- [ ] Repeat with the longest translation or pseudo-localised strings and the longest remapped prompt.
- [ ] Every shipped language renders without missing glyphs in every font choice.
- [ ] Switch `text.font` live in a menu and in dialogue. All text surfaces change; nothing re-lays out
      off-screen.
- [ ] Apply the WCAG 1.4.12 spacing overrides; text stays readable and selectable.
- [ ] `text.background_opacity` at 100% gives a solid panel over the brightest and busiest scene.
- [ ] `text.reveal = instant` shows full lines; a press during reveal completes, a second press
      advances; no line is skipped. Auto-advance off waits indefinitely.
- [ ] Every diegetic text opens a readable overlay and appears in the log.
- [ ] Focus is visible on every control in grayscale and with animations off; dialogs return focus.
- [ ] Change values, restart, load an old save: values persist and are not taken from the save.
- [ ] OS text size imports on a fresh profile and does not override a value set by hand.

## Common mistakes

- Scaling the root viewport or canvas → scale theme tokens and reflow containers.
- Auto-fit text that shrinks back after the player enlarges it → wrap or scroll instead.
- "Large text" that covers menus but not HUD, tooltips or prompts → inventory every text surface.
- Body text in a stylised display font → keep display fonts for titles; body text is plain.
- "Dyslexia mode" or "fixes dyslexia" copy → "Font: dyslexia-friendly (reading preference)".
- Bundling a font file without checking its exact licence → read the licence shipped with that file.
- Signs and notes baked into textures → localisation keys plus a readable overlay.
- Focus shown only by a colour change or a pulse → add a thick outline or marker; offer a static style.
- Typewriter text that cannot be completed or that expires → instant option, press-to-complete, manual
  advance.
- Testing only at 4K on a large monitor → declare and test the minimum resolution and a TV distance.
