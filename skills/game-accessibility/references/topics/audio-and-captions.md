# Audio and captions

**Load when:** any audio. Gameplay-significant sound (S006), spoken dialogue (S066), overlapping, radio
or off-screen speech (S067), positional cues (S068), sound-only puzzles (S069), loud sudden sounds
(S070), distressing repetitive sounds (S071), music-driven rhythm (S072), cinematic storytelling
(S010), several languages (S079), haptic information (S008). **Skip when:** the game has no audio
(S007) confirmed by inspection. Record the absence in `ACCESSIBILITY.md`. Skip only the speech rows
when there is no speech, never the master volume and safe-start checks when any sound exists.

Voice chat and transcription: [multiplayer-and-communication](multiplayer-and-communication.md).
Spoken UI and audio description: [screen-reader-and-voice](screen-reader-and-voice.md). Jump-scare
stings: [content-and-phobias](content-and-phobias.md).

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| H01 | Master volume / mute | `audio.master` | 0.7 with limiter headroom; quiet first launch | B | H, N, S |
| H02 | Music volume | `audio.music` | 0.6 | C | H, N, C |
| H03 | Dialogue volume | `audio.voice` | 1.0 | C | H, C |
| H04 | Effects volume | `audio.sfx` | 0.8 | C | H, V |
| H05 | Ambience volume | `audio.ambience` | 0.7 | C | H, N |
| H06 | Accessibility-cue volume | — | Designer | C | V, H |
| H07 | Narration volume | — | Designer; audible over music | C | V, C |
| H08 | Voice chat volume | — | Designer | C | H, N |
| H09 | Mono audio | `audio.mono` | Off; OS mono as first value | C | H, S |
| H10 | Stereo / surround output choice | — | Platform | C | V, H |
| H11 | Headphone spatial mix | — | Off | C | V, H |
| H12 | Dialogue ducking | — | Designer | C | H, C |
| H13 | Screen-reader ducking | — | On while narration speaks | C | V, C |
| H14 | Reduce sudden loud sounds | `audio.reduce_loud_sounds` | Off (peaks bounded anyway) | C | H, N, S |
| H15 | Sound-specific comfort (tinnitus-like tones, repetitive sounds) | — | Off | A | H, N |
| H16 | Pause and replay important audio | — | On | C | H, C |
| H17 | Dialogue subtitles | `subtitles.dialogue` | **On**; offered on first launch | B | H, C, S |
| H18 | Barks and background speech subtitles | `subtitles.dialogue` | On with H17 | C | H, C |
| H19 | Non-speech sound captions | `subtitles.sound_captions` | Off; OS captions as first value | C | H |
| H20 | Speaker names | `subtitles.speaker_names` | On | C | H, C |
| H21 | Off-screen speaker direction | `subtitles.direction` | Off | C | H |
| H22 | Sound direction and range in captions | `subtitles.direction` | Off | C | H |
| H23 | Delivery, tone and source labels (whisper, radio) | — | Off | C | H, C |
| H24 | Subtitle size | `subtitles.size` | 1.0; OS caption scale as first value | B | V, H |
| H25 | Subtitle font / weight | — | Plain font | C | V, C, H |
| H26 | Subtitle text colour | — | Designer; OS caption style as first value | C | V, H |
| H27 | Subtitle background colour | — | Designer; includes black | C | V, H |
| H28 | Subtitle background opacity | `subtitles.background_opacity` | 75% (0–100%) | C | V, H |
| H29 | Subtitle outline / shadow | — | Designer | C | V, H |
| H30 | Subtitle position / safe margins | — | Bottom, inside safe area | C | H, V |
| H31 | Caption duration / manual advance | `text.auto_advance` | Off (wait for player) | C | C, H, V |
| H32 | Caption category filters | — | All categories | C | H, N |
| H33 | Dialogue and caption history | — | Available | C | H, C |
| H34 | Sound visualiser / directional indicators | `audio.sound_visualizer` | Off | C | H |
| H35 | Haptic equivalents for audio events | — | Off | C | H, V |
| H36 | Sign-language interpretation (per language) | — | Off | A | H |
| H37 | Separate speech and subtitle languages | — | Follows system language | C | H, C, S |

`—` keys are not in the shared registry yet. Add a project key in `ACCESSIBILITY.md` when the signal
is present.

## How to implement

### Buses and mix (H01–H08, H12, H13)

- One bus per category: **master, music, voice (dialogue), sfx, ambience, ui, narration, accessibility
  cues, voice chat**. Each slider maps to one bus. Narration never shares the dialogue bus; voice chat
  never shares dialogue or narration.
- **No essential information on the music bus.** Alarm stings, timer ticks and enemy cues go on sfx or
  accessibility cues, so Music 0 never removes information. Music 0 also silences menu music.
- Duck music and ambience under dialogue (H12) and under narration (H13), with smooth attack and
  release. Never clip the first syllable.
- `audio.master` is a **device** setting. The others follow the player profile.

### Safe first launch (H01)

- Boot screen plays no logo, jingle, music or content audio. Load settings and apply volumes first.
  The one exception is a quiet, repeatable spoken-menu prompt when no screen reader is detected
  ([architecture](../architecture.md) section 10).
- Master starts quiet with limiter headroom. Music starts **after** the volume controls are reachable
  and fades in.
- Volume previews play only when the player asks. Never fire a loud sample on each slider tick.

### Mono, output and spatial (H09–H11)

- Mono folds both channels into each output. Test that panned cues do not cancel (phase) when summed.
- Mono removes direction, so pair it with `audio.sound_visualizer` or `subtitles.direction`. The
  registry description says so; keep it true.
- Import the OS mono setting as the first value while the player has not set it.

### Loudness and sound-specific comfort (H14, H15)

- `audio.reduce_loud_sounds` adds a compressor/limiter that lowers peaks (explosions, stings,
  screams). Keep cue distinctions audible after compression.
- H15 replaces, attenuates or removes one named sound category (high ringing, heartbeat loops,
  buzzing). The description says which. Information-bearing sounds get a neutral replacement, not
  silence.

### Subtitles vs captions (H17–H23)

- **Subtitles** transcribe speech. **Captions** also describe meaningful non-speech sound and music:
  `[Alarm blaring, left]`, `[Footsteps behind you]`, `[Tense music]`.
- Author **semantic events** and send them through `CaptionBus`, not audio files:
  `CaptionEvent { id, kind, text_key, speaker_id, source_position, importance, duration_s | audio_player, merge_key }`.
- Captions fire **from the event, even when the mixer is muted**. Never derive them from audio level.
- Cover every speech source: cutscenes, systemic barks, radios, tutorials, the boot sequence, voiced
  settings. A line with no text is a bug.
- Caption meaningful sounds only, by priority. Loops get one caption with a `merge_key`, plus start
  and stop. Do not caption every footstep at equal priority.
- **Speaker names** (H20) go before the line. Repeat the name after a speaker change or after a 1–2
  minute pause. A hidden speaker is "Unknown voice" until the story reveals them. Speaker colour is
  extra; the name is the identity.
- **Direction** (H21, H22): a word or arrow for off-screen sources ("left", "behind"), updated
  consistently. Give no more than a hearing player gets: no exact distances through walls.

### Presentation (H24–H30)

- At most 2 lines and about 40 characters per line; 3 lines only by exception. Break lines at
  meaningful points; re-break when size or language changes.
- `subtitles.size` scales independently of `text.scale`, to at least 200%.
- `subtitles.background_opacity` from 0% to 100%. An outline or shadow alone fails on snow and fire.
- Text and background colours are player choices with a white-on-black option. Preview on a bright,
  busy scene that the player starts by pressing a button.
- Place inside the safe area and never over input prompts, QTE prompts or other captions. Reserve
  space for the largest size.

### Timing, categories and history (H16, H31–H33)

- Tie dialogue cues to actual playback, including pause, skip and seek. Reactive captions get a
  readable minimum display time in UI time, not game time (game speed does not shorten them).
- Where a scene can wait, `text.auto_advance = false` holds the line until the player continues.
- Category filters (H32) can hide ambient captions, but **critical** captions (threats, objectives)
  stay available in every reduced mode.
- History (H33) lists past lines with speakers in order. Opening it never advances the scene,
  discards a pending choice or shows future lines.
- Pausable or replayable important audio (H16): replay without repeating a hazard.

### Sound visualiser (H34)

- Driven by the same `CaptionBus` events: category, `source_position` and importance give an icon at
  the screen edge pointing to the source. Clamp to the safe area. Offer a stationary variant.
- **Not a waveform or amplitude meter.** Audio-derived direction cannot know game semantics; the game
  already knows where the source is.
- Separate "sound occurred" from "threat nearby". Merge repeats. Offer size and density controls so
  it does not become clutter.

### Haptics (H35)

- Haptics are an **extra** channel with intensity and Off. Never the only substitute for sound, and
  never required (some players find vibration uncomfortable or use devices without it). Every
  essential sound has a visual or text equivalent first; haptics are added on top.

### Sign language (H36, Advanced)

- Record a scope decision. Each sign language is its own deliverable: **ASL is not BSL**. Name the
  languages and the content covered (for example "story cinematics only").
- Picture-in-picture with size, position and background controls. Captions are still needed.

### Languages (H37)

- Store speech language and subtitle language separately. Document which voice and subtitle languages
  cover which content, and ship fonts with full glyph coverage.

## Numbers that matter

| Source | Number | Scope |
|--------|--------|-------|
| [XAG 104](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/104) | **≤ 40 characters per line; ≤ 2 lines** (3 by exception) | Authored subtitles; reflow for translation and scale |
| XAG 104 | Size scalable to **≥ 200%**; background opacity **0–100%** | Subtitle presentation |
| XAG 104 | Re-identify the speaker after a change or a **1–2 minute** pause | Sole unnamed narrator exempt |
| [AGI Large & Clear Subtitles](https://accessiblegames.com/wp-content/uploads/2025/03/Accessible-Games-Initiative-Tags-and-Criteria-March-2025.pdf) | Default **32 px at 1080p**; PC scaling to **≥ 46 px** | Store tag criterion |
| [BBC subtitle guidelines](https://www.bbc.co.uk/accessibility/forproducts/guides/subtitles/) | 160–180 words per minute | Broadcast editorial target; games may need slower |
| [Ian Hamilton](https://ian-hamilton.com/how-to-do-subtitles-well-basics-and-good-practices/) | About 38 characters per line; about 0.25 s between subtitles | 2015 practical guidance |
| [XAG 105](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/105) | Audio events shorter than 3 s need no pause control | Live multiplayer exempt from pause |

## Shipped examples

- The Last of Us Part I: subtitle speaker names and direction, cue volume categories, and
  speech-to-vibration on DualSense (device-specific).
  [PlayStation Blog](https://blog.playstation.com/2022/08/26/the-last-of-us-part-i-full-list-of-accessibility-features/).
  2022 PlayStation feature list, not a PC parity claim.
- Minecraft (Java): sound captions with off-screen direction arrows, used as the XAG 104 example.
  [XAG 104](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/104).
  Java captions are not proof of Bedrock parity.
- Fortnite: Visualize Sound Effects shows radial indicators for selected sounds.
  [Epic](https://www.epicgames.com/help/c-34254770/c-33726977/a25071786?lang=en-US). Epic says "some
  sounds"; not full coverage.
- Overwatch 2: Season 3 added subtitle size, speaker portrait and name, text and background colours,
  and a preview. [Blizzard](https://news.blizzard.com/en-us/article/23912175/overwatch-2-season-3-starts-february-7-new-antarctic-peninsula-control-map-one-punch-man-collab-loverwatch-dating-sim-and-so-much-more).
- Horizon Forbidden West: tinnitus-sound removal, extra sound categories and forced mono on PS5.
  [PlayStation Blog](https://blog.playstation.com/2022/02/10/accessibility-features-in-horizon-forbidden-west/).
  PS5 scope.
- God of War Ragnarök: detects the PlayStation mono-audio setting.
  [PlayStation](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/).
- Forza Horizon 5: picture-in-picture ASL and BSL for cinematics, with position and background
  settings. [Forza](https://forza.net/news/forza-horizon-5-series-5-update). Story scenes only, not all
  interactive play.
- Cautionary: Persona 5 Royal reviewer found background speech uncaptioned and no size option
  ([CIPT](https://caniplaythat.com/2020/04/02/deaf-hoh-review-persona-5-royal/)); Sekiro reviewer
  found fixed presentation and no speaker labels
  ([CIPT](https://caniplaythat.com/2019/03/31/deaf-game-review-sekiro-shadows-die-twice/)); Madden NFL
  24 subtitles cover only Superstar cutscenes ([EA](https://www.ea.com/able/resources/madden-nfl/madden-nfl-24?isLocalized=true)).

## Verify

- [ ] Fresh profile: boot plays nothing but the optional quiet spoken-menu prompt; volume and
      subtitle settings are reachable before the first logo, music or speech; music fades in
      afterwards.
- [ ] Move each bus slider to 0 in turn. Only that category goes silent. Music 0 silences menu music
      and removes no gameplay cue. Narration stays audible with music at maximum.
- [ ] **Muted play:** mute at the mixer (not the caption source) **and turn vibration off**. From
      first launch, play through critical dialogue, alarms, hazards, directional enemies, timer cues,
      off-screen events and one cinematic. Every essential sound has a visible caption, indicator or
      text equivalent; haptics do not count. Record misses.
- [ ] Every speech line, including barks, radio and tutorials, produces a subtitle. Count a sample of
      voice assets against subtitle events.
- [ ] Largest `subtitles.size` with `hud.mode = minimal`: nothing is clipped; no prompt is covered.
- [ ] Background at 100% stays readable over the brightest scene; at 0% the option truly removes it.
- [ ] Speaker name appears and re-appears after a speaker change; a hidden speaker reads "Unknown
      voice" while direction still shows.
- [ ] Mono on: panned cues stay audible; direction shows visually when the visualiser is on.
- [ ] Looping sounds caption once; overlapping critical captions are not dropped.
- [ ] History shows past lines in order without advancing the scene.
- [ ] Settings persist after restart, preset changes and loading an old save.

## Common mistakes

- Captions computed from audio levels → emit from semantic events, so muted play still works.
- A single "subtitles" toggle covering only cutscenes → cover barks, radio, tutorials and boot.
- Important cues on the music bus → keep information on sfx or cue buses.
- Shadow-only subtitles → background with 0–100% opacity and a live preview.
- Speaker identity by colour only → names first; colour is extra.
- Waveform "sound radar" → event-driven indicators through `CaptionBus`.
- Haptics as the only substitute for sound → always add a visual channel.
- "Sign language" without naming which → name ASL, BSL or others, and the scenes covered.
- Loud preview on every slider tick → player-triggered preview only.
