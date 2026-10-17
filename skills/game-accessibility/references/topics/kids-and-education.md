# Kids and education

**Load when:** the game targets children or early readers, is used in classrooms, or has S153
educational assessment (scores meant to show learning or mastery). Also load for S012 compulsory
tutorials when the audience includes young or early readers.
**Skip when:** the developer confirms the audience is adults and nothing is scored as learning.
Gameplay guides still apply in either case: a kids' platformer also loads
[traversal-and-platforming](traversal-and-platforming.md). Never skip narration because players are
"old enough to read".

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| R12 | Read all text aloud: instructions, questions, story, item text | `narration.dialogue` | false | C | V, C |
| R01 | Menu narration | `narration.enabled` | false, or on with the OS screen reader | B | V, C |
| C13 | Repeat an instruction on demand, any time, no cost | none yet | Available | C | C, H |
| C25 | Instructions as text, speech and demonstration together | none (design requirement) | On | C | C, V |
| V25 | Symbols plus words: every icon labelled, every word paired with a picture where possible | none (design requirement) | On | B | C, V |
| C02 | Plain-language instructions and settings | none (design requirement) | On | B | C |
| C01 | Text waits for the player | `text.auto_advance`, `text.reveal` | false, `normal` | B | C |
| H17 | Subtitles for voiced instructions | `subtitles.dialogue` | true | B | H, C |
| V01 / V03 | Text size and readable font | `text.scale`, `text.font` | 1.0, `default` | B | V, C |
| D29 | Untimed play unless speed is the learning goal | `assist.timers` | `normal` | C | C, M, T |
| C06 | Practice without failure or score | none yet | Available | C | C, M, T |
| C14 / C15 | Graduated hints | `memory.hints` | `on_request` | C | C, T |
| D14 | Game speed | `assist.game_speed` | 1.0 | C | M, C |
| M32 | Tap or click to select, then place, instead of dragging | none yet | Available | C | M |
| M14 / M15 | Hold or single press instead of repeated presses | `input.mash_mode` | `mash` | C | M |
| N06 | Calmer background and less decorative detail | `effects.particle_density` (extend to backgrounds) | 1.0 | C | N, C |
| L03 | Separate profiles per child on shared devices | none yet | One per player | C | V, H, M, C |
| L21 | Adult or teacher configuration that never locks a child's personal access settings | none yet | Personal settings stay editable | C | V, H, M, C |
| C39 | Adaptive scaffolding with no public labels | none yet | Off or designer value | C | C, T |
| L11 | Gaze and dwell input compatibility | none yet | Off | A | M |

Add missing keys to the project's registry copy with `profile` scope. Related guides:
[text-and-readability](text-and-readability.md), [screen-reader-and-voice](screen-reader-and-voice.md),
[timing-and-speed](timing-and-speed.md), [touch-and-mobile](touch-and-mobile.md).

## How to implement

### 1. Separate the learning goal from access demands (S153)

Write down what each activity assesses, for example "adds numbers to 20". Then remove every other
demand from the score:

- Score the **answer**, not reading speed, drag precision, reaction time or rapid tapping, unless
  that skill is the stated target.
- If speed is the learning goal (for example reading fluency), say so in the activity description
  and still offer untimed practice.
- A slipped drag or a mis-tap is not a wrong answer. Use select-then-confirm, so an input error can
  be undone before it counts.
- Reports to adults describe learning outcomes. They never treat accessibility settings as
  performance data, and never infer a disability from them.

### 2. Instructions through several channels (R12, C13, C25, V25, C02)

- Every instruction exists as speech (recorded voice or TTS through Narrator), as text, as symbols
  and, for actions, as a short demonstration.
- A "repeat" button (a remappable action) replays the current instruction at any time, with no
  timer, cost or score change. Instructions stay on screen until the child continues.
- Pair icons with words and key words with pictures. Use short sentences and concrete words.
  Explain idioms or leave them out.
- Voiced instructions also show subtitles (`subtitles.dialogue`).

### 3. Time and pace (D29, C01, D14, C06, C14/C15)

- No countdowns unless time is the learning target. `assist.timers` extends or removes the rest.
- Text never auto-advances by default. `text.reveal` offers instant text.
- A practice mode with no failure and no score lets a child repeat an activity freely.
- Hints come in steps (`memory.hints`): a nudge, a clearer hint, then a worked example.

### 4. Physical demands (M32, M14/M15, L11)

Replace drags with select-then-place, mashing with a single press or hold, and small targets with
large, well-spaced ones. All input goes through the action layer, so eye-gaze, head-tracking and
switch tools work. Gaze and switch scanning are Advanced scope decisions; record them.

### 5. Adult and teacher configuration

- Adults can set curriculum, level, session length and content options, by class or by child.
- Class or parent presets are **starting patches** (core-policy section 5). They never lock or
  override a child's personal access settings: text size, narration, subtitles, colours, motion,
  input and hints stay changeable by the child, from the child's own screen.
- If an adult control must restrict something (for example chat), say what it restricts. It must
  not hide accessibility settings.
- Each child on a shared device has a profile (L03). Profile names never describe a disability.

### 6. Adaptive scaffolding without labels

If the game adapts difficulty or adds scaffolds, do it privately. No "easy group" badges, no
leaderboards that reveal scaffold level, and no visible marks on saves, certificates or rewards
(core-policy section 2). Children compare screens: a child should not be able to tell from another
child's screen which supports are on.

### 7. Children differ from each other

Do not build one "autism mode" or "dyslexia mode". The PBS Cyberchase guide notes that what helps
one autistic child can make the game harder for another. Offer independent options (background
detail, speed, sound, narration), and name presets by what they do.

### 8. Privacy and microphones: a legal question

Read-along recording, speech recognition, voice chat and any data about children raise legal
questions, such as COPPA in the United States and similar laws elsewhere. **This guide does not give
legal advice, and this skill's research did not review child-privacy law (UNVERIFIED).** Flag the
question to the project's legal owner and record the answer in `ACCESSIBILITY.md`. Until then, keep
the microphone off by default, show its state clearly, and do not record or upload audio. Speech
input is always optional and has a button or tap alternative.

## Numbers that matter

| Value | Source and scope |
|---|---|
| Caption rate appropriate to the target age group | [GAG](https://gameaccessibilityguidelines.com/ensure-that-subtitles-captions-are-cut-down-to-and-presented-at-an-appropriate-words-per-minute-for-the-target-age-group/). No single number given. |
| 160-180 words per minute | [BBC subtitle guidelines](https://www.bbc.co.uk/accessibility/forproducts/guides/subtitles/). An editorial target for general audiences; young readers may need slower pacing or player-paced text. |
| UI time limits adjustable to at least 10x, or removable | [XAG 116](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/116). UI limits, not core gameplay. |
| Body text at least 26 px (console) or 18 px (PC) at 1080p | [XAG 101](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/101). Measure rendered glyph height. |

## Shipped examples

- PBS Cyberchase accessibility guide: notes that autistic children's needs vary and one child's
  helpful setting can make another's experience harder.
  [PBS LearningMedia guide](https://static.pbslearningmedia.org/media/media_files/31536ca5-64a6-4cb7-9df8-e229b90cd985/87845c36-5a69-4d30-ac39-4c61433e17dc.pdf).
- PBS Duck Dash: background, speed and difficulty options, plus eye- and head-tracking
  functionality, as described in PBS's July 2022 report.
  [PBS report](https://dc79r36mj3c9w.cloudfront.net/prod/filer_public/pbsnc-redesign-phase-1/about/c85308bf39_July_2022_PBS_DEI_AnnualReport.pdf).
  A report summary, not a feature audit.
- Minecraft Education: Immersive Reader and UI narration. The same page identifies some controller
  limitations. [Minecraft Education support](https://edusupport.minecraft.net/hc/en-us/articles/360061503031-Accessibility-Features).
  Check edition and platform before copying a feature.

## Verify

- [ ] A child who cannot yet read can start the game, hear and repeat the first instruction, and
      complete the first activity without a reading deadline.
- [ ] Every instruction has speech, text and a symbol or demonstration; "repeat" works everywhere,
      including mid-activity and in menus.
- [ ] With narration on, every menu, question and answer choice is read.
- [ ] With timers off, every activity can be completed; speed-target activities say so and offer
      untimed practice.
- [ ] A mis-drag or mis-tap can be undone before it is scored.
- [ ] Scores and adult reports change only with answers, not with which access settings are on.
- [ ] Apply a class or parent preset, then change text size, narration and subtitles from the
      child's own screen. The changes stick and the preset does not overwrite them on relaunch.
- [ ] Two child profiles on one device keep separate settings.
- [ ] No badge, colour or label anywhere shows which child uses scaffolds or assists.
- [ ] Microphone is off by default, its state is visible, and every speech feature has a non-speech
      alternative. The legal owner's answer on child data is recorded, or the item is `blocked`.

## Common mistakes

- Timing the answer when the lesson is not about speed → score answers; untimed practice always.
- Text-only instructions for early readers → speech, symbols and demonstration too.
- Instructions that vanish after one play → a repeat button that always works.
- Drag-to-answer where a slipped finger counts as wrong → select, then confirm.
- Teacher presets that lock text size or narration → presets are patches; personal settings stay
  editable by the child.
- "Easy group" labels, coloured badges or leaderboards that expose scaffolds → adapt privately.
- One "special needs mode" → independent options named by effect.
- Treating kids as needing less accessibility than adults → children need the same Baseline.
- Turning on a microphone or recording voice without a recorded legal decision → mic off, flag the
  question.
