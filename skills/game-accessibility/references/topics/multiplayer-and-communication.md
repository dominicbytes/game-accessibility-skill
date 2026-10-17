# Multiplayer and communication

**Load when:** S123 online voice chat, S124 online text chat, S125 voice callouts needed to
coordinate, S126 lobbies, invites or ready checks, S127 local split screen, S128 shared devices or
controller handoffs, S129 co-op that needs synchronised actions, S130 competitive ranking, S131
spectator, replay or scoreboard views, S132 user content or harassment exposure, S133 disconnect,
AFK or session-loss rules, S108 shared server progression.
**Skip when:** the game is single-player only, with no chat, lobby, shared screen or shared world.
Record that. Offline local play still loads this guide for S127-S129.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| K22 | Accessible path to communication: entry, lobby, invites, chat settings before first conversation | none (design requirement) | On | B | V, H, M, C |
| K01 | Free-form text chat | none yet | Available | C | H, M, S |
| K02 | Voice alongside text | none yet | Available | C | V, C, M |
| K03 | Incoming voice transcription (speech-to-text) | none yet | Off, or platform preference | C | H, S |
| K04 | Outgoing text-to-speech of typed messages | none yet | Off | C | M, C, S |
| R04 | Incoming text-chat narration (local only) | none yet | Off; on if a screen reader is active | C | V, C |
| K05 | Canned phrases | none yet | Available | C | M, C |
| K06 | Symbol communication, labelled and narrated | none yet | Available | C | C, H |
| K07 | Contextual pings and markers | none yet | Available | C | H, M, C |
| K08 | Custom quick-chat phrases | none yet | Off | C | M, C |
| K09 | Real-time text (characters sent as typed) | none yet | Per legal review | C | H |
| K10 | Chat text size and contrast | `text.scale` (extend to chat) | 1.0 | C | V, C |
| K11 | Chat display duration and pinning | none yet | Messages persist | C | C, V, H |
| K12 | Scrollback history | none yet | On | C | C, H, V |
| K13 | Active-speaker identification | none yet | On with voice | C | H, C |
| K14 | Push-to-talk, toggle or open mic | none yet | Privacy-preserving: no automatic open mic | C | M, T, S |
| H08 | Voice-chat volume | none yet | Designer value | C | H, N |
| K15 | Per-player volume and mute | none yet | Available | C | H, N, T |
| K16 | Incoming message and TTS filtering | none yet | Off | C | N, T |
| K17 | Accessible block and report | none yet | On | C | T, M, V |
| K18 | No-voice matchmaking preference | none yet | Off | C | H, T |
| K23 | Agreed rules matchmaking (shared session rules) | none yet | Off | C | M, C, T |
| K19 | Shared control / co-pilot assistance | none yet | Off | C | M, C, T |
| K20 | Per-player settings in local co-op | profile scope for every personal key | On | C | V, H, M, N |
| K21 | Flexible ready-check and lobby deadlines | none yet | Designer value, extendable | C | M, C |

No communication keys exist in the registry yet. Add them to the project's registry copy with
`profile` scope, except device routing such as microphone choice (`device`). Hold and toggle rules for
push-to-talk live in [input-and-motor](input-and-motor.md). Store and legal claims live in
[store-claims-and-compliance](store-claims-and-compliance.md).

## How to implement

### 1. The whole path to the first conversation (K22)

Audit the route, not only the chat box: launch, sign-in, character or server selection, matchmaking,
lobby, invites, chat settings, sending, receiving, mute, block, report, support and documentation.
Every step works with keyboard or controller only, is narrated, scales with `text.scale`, and has no
unexplained timeout. Communication settings (transcription, TTS, mic mode, chat size) are reachable
from the title menu, so a player can set them before anyone speaks. Text fields accept the system
virtual keyboard and dictation (see [mods-hardware-and-platforms](mods-hardware-and-platforms.md)).

### 2. Four different capabilities

| Capability | What it does | Rules |
|---|---|---|
| Speech-to-text (K03) | Shows incoming voice as text | Label it as automatic. Name the speaker. Show when transcription fails or a language is unsupported. |
| Outgoing TTS (K04) | Speaks the player's typed message to others | Broadcast only submitted text, never drafts. Let the player pick a voice. Show that it was sent. |
| Local narration (R04) | Reads incoming text chat to this player only | Route through Narrator at low priority. Critical game announcements interrupt it. One route per message: no double speech with a running screen reader. |
| Real-time text (K09) | Sends characters as they are typed | Not the same as send-button chat or STT captions. Scope it through legal review (section 3). |

Show microphone and transcription state on screen and in narration. Request microphone access only
when the player turns voice on. State whether speech is processed locally or by a service. Never
record silently, and never share a player's accessibility settings with other players. Keep chat in
its own view, not the story subtitle view, but reuse the subtitle size and background settings.

### 3. Legal triage, not legal advice (K09)

The EU Accessibility Act applies from 28 June 2025 to listed services. Where game chat is in scope,
Annex I requires real-time text in addition to voice. The European Electronic Communications Code
excludes some minor, purely ancillary chat under a narrow test. Neither "all game chat is covered"
nor "game chat is always exempt" is supportable. In the US, CVAA rules cover relevant advanced
communications; the FCC game-software waiver ended on 31 December 2018. Inventory voice, text,
video, free-form versus canned, and who operates chat, then hand the questions in
[store-claims-and-compliance](store-claims-and-compliance.md) to the project's legal owner. Do not
decide coverage yourself.

### 4. Pings, phrases and symbols (K05-K08)

Each ping carries an icon, a text label, a sound and a narration string ("Enemy spotted, north
east"). Pings work with any bound input, not only a mouse wheel. Rate limits are adjustable.
Canned phrases and pings help, but they **do not replace free-form communication**. Keep text chat
available. A custom-phrase editor must itself be accessible.

### 5. Readable chat (K10-K13)

- Chat input, messages, history and transcripts all follow `text.scale` and a background-opacity
  setting. Meet 4.5:1 contrast for chat text.
- Messages persist until the player dismisses them, or stay in a scrollback of at least the last 4
  messages (AGI). Show speaker name and order. Do not identify speakers by colour alone.
- Active speaker: show a name plus an icon near the speaker list or portrait, never colour only.

### 6. Microphone, volume and filters (K14, H08, K15, K16)

- Offer push-to-talk, toggle-to-talk and open mic. The default never opens a public microphone by
  itself. Push-to-talk is remappable, and toggle replaces holding it.
- Voice-chat volume is separate from dialogue and narration. Per-player volume and mute sit in an
  accessible player list that does not hide block or report.
- Incoming filters (profanity, spam, TTS of incoming text) never mute the player's own outgoing
  communication.

### 7. Block and report (K17)

Block and report are reachable during and after a match, by keyboard, controller and narration,
with no voice-only step. Evidence is captured without forcing the reporter to replay harmful audio
or video. Reports state what happens next.

### 8. Shared simulation rules (K18, K23, S130)

Split settings into two kinds:

- **Personal presentation and input: always allowed, in every mode.** Text size, captions, chat
  options, narration, colour profiles, content filters, camera comfort, remapping, hold/toggle. They
  change only this player's view and controls. Nobody else is told.
- **Gameplay-affecting assists in shared sessions** (aim assist, game speed, timing windows, damage,
  extra information about opponents) **need explicit session rules.** A client cannot change the
  server's clock or another player's damage. Ask the developer to choose: allowed for everyone, a
  visible lobby rule set by the host, private or practice modes, or separate rule-based playlists open
  to every player. **Escalate; do not decide.** Record the choice in `ACCESSIBILITY.md`.

Never disclose disability. Matchmaking preferences describe rules ("No voice required", "Aim assist
allowed"), not players. No assist badge is shown to other players (core-policy section 2). When a
setting is unavailable in a mode, say so in its description; do not silently ignore it. Separate
leaderboards are an owner decision, never a default.

### 9. Local co-op and shared devices (K19, K20, S127-S129)

Each local player has their own profile: bindings, text size, captions, narration and assists.
Controller handoff never overwrites another player's settings. Captions and prompts render in each
viewport. Where a channel is shared (one speaker, one global clock), document the conflict and how
it resolves. For synchronised co-op actions, offer a sequential window, a ready-up, or a bot or
co-pilot that can take one role. Shared control combines two devices for one player.

### 10. Deadlines, reconnect and spectators (K21, S126, S131, S133)

Ready checks, lobby countdowns, respawn choices and AFK kicks are UI deadlines: allow extension or an
accessible warning before they expire. Reconnect returns the player to the session with settings
intact. Scoreboards and spectator views use names and shapes, not colour alone, and are narratable.

## Numbers that matter

| Value | Source and scope |
|---|---|
| At least the past 4 messages accessible | [AGI chat tag criteria](https://accessiblegames.com/wp-content/uploads/2025/03/Accessible-Games-Initiative-Tags-and-Criteria-March-2025.pdf): displayed, scrollable or expandable, including transcription views. |
| Up to at least 10x default; at least 20 s warning; at least 10 extensions | [XAG 116](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/116): UI time limits outside core mechanics, such as lobby and ready timers. |
| Live multiplayer exempt from pausing | [XAG 105](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/105). Settings must still open without pausing the match. |
| 4.5:1 text contrast | [XAG 102](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/102), applied to chat text and transcripts. |
| 28 June 2025 | [EAA](https://eur-lex.europa.eu/eli/dir/2019/882/oj/eng) application date; real-time text where in scope. |
| 31 December 2018 | End of the FCC game-software waiver ([FCC order](https://docs.fcc.gov/public/attachments/DA-17-1243A1.pdf)); see [47 CFR Part 14](https://www.ecfr.gov/current/title-47/chapter-I/subchapter-A/part-14). |

Guidance: [XAG 119](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/119)
(communication channels) and [XAG 120](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/120)
(the whole path to communication). Both are best practice, not law.

## Shipped examples

- Halo Infinite: incoming speech-to-text and outgoing text-to-speech chat, menu narration.
  [Halo Support](https://support.halowaypoint.com/hc/en-us/articles/4408122295444-Accessibility-Features-in-Halo-Infinite).
- Apex Legends: contextual pings for communication without speech; chat conversion.
  [EA settings](https://www.ea.com/able/resources/apex-legends/pc), [EA impact report](https://media.contentapi.ea.com/content/dam/eacom/common/ea-impact-report-22.pdf).
- Forza Motorsport: communication TTS and STT alongside narration. [Xbox Wire](https://news.xbox.com/en-us/2023/04/27/forza-motorsport-accessibility-features-blind-driving/).
- EA Sports FC 24: chat conversion with menu narration. [EA](https://www.ea.com/able/resources/ea-sports-fc/fc-24?isLocalized=true).
- Overwatch 2: voice transcription is used for moderation reports, not as player-facing captions.
  [Blizzard](https://overwatch.blizzard.com/en-us/news/23857517/defense-matrix-activated-fortifying-gameplay-integrity-and-positivity-in-overwatch-2/).
  Do not count moderation transcripts as K03.
- Nintendo Switch 2 GameChat: system speech/text conversion with language-dependent support.
  [Nintendo](https://www.nintendo.com/en-gb/Support/Troubleshooting/Guide-to-Accessibility-Features-on-Nintendo-Switch-2-2843775.html).
  Not evidence that a game's own voice chat inherits it.
- Phasmophobia: text alternatives to speaking Spirit Box and Ouija questions.
  [Kinetic Games](https://kineticgames.co.uk/news/phasmophobia-tempest).
- Jackbox Party Pack 10: extended or no timers. [Jackbox](https://www.jackboxgames.com/blog/accessibility-features-in-the-jackbox-party-pack-10).
  Pack 8: moderation, profanity filter, room-code controls, longer timers for stream delay.
  [Jackbox](https://www.jackboxgames.com/streaming-moderation-accessibility-features-jackbox-party-pack-eight/).
  Support varies per game in each pack.
- Street Fighter 6: control types with different supported modes; Dynamic is offline-restricted.
  [Capcom](https://www.capcom-support.com/hc/en-us/articles/7122030981660-What-are-the-control-types-in-Street-Fighter-6).
  An example of stating per-mode rules openly.
- Horizon Forbidden West: second-controller co-pilot. [PlayStation Blog](https://blog.playstation.com/2022/02/10/accessibility-features-in-horizon-forbidden-west/).
- It Takes Two: chat conversion, but the partner requirement and real-time mechanics remain.
  [TechRadar](https://www.techradar.com/gaming/it-takes-two-review).

## Verify

- [ ] From a fresh profile, reach the lobby, invite a friend and send a message using only keyboard,
      only controller, and with narration on. Every step speaks and nothing times out unannounced.
- [ ] Communication settings can be changed before the first conversation and during a match
      without pausing the session.
- [ ] Play a match without speaking and without hearing: text, pings and transcription carry the
      required coordination.
- [ ] Transcription labels the speaker, shows a failure state, and lists unsupported languages.
- [ ] Outgoing TTS sends only submitted text. Drafts are never heard.
- [ ] Incoming chat narration does not double-speak with a running screen reader and is interrupted
      by critical game announcements.
- [ ] At least the last 4 messages can be reviewed at maximum `text.scale`, with speaker names.
- [ ] Mic state is visible and narrated. No setting opens a public mic automatically.
- [ ] Mute, block and report work mid-match and post-match with each supported input.
- [ ] No other player can see which accessibility settings anyone uses, in lobby, scoreboard, replay
      or profile.
- [ ] Each gameplay-affecting assist has a recorded session-rule decision. Its description states
      where it is unavailable.
- [ ] Local co-op: two players with different settings swap controllers; neither profile changes.
- [ ] Disconnect and rejoin: settings and chat history survive.
- [ ] If chat may be in legal scope, the legal owner's answer is recorded, or the item is `blocked`.

## Common mistakes

- Treating canned pings as full communication → keep free-form text and a speech route.
- Calling STT captions "real-time text" → RTT sends characters as typed. Route it to legal review.
- Accessible chat box behind an inaccessible lobby or sign-in → audit the whole path (K22).
- Open mic by default, or transcription enabled without telling the player → privacy-preserving
  defaults and visible state.
- Showing an "assisted" icon on the scoreboard → never announce settings to other players.
- Silently disabling a player's assist in ranked → state the session rule in the option text.
- Deciding competitive assist policy alone → escalate to the developer with concrete options.
- Asking players why they don't use voice → matchmaking preferences describe rules, not people.
- One profile for the whole couch → per-player profiles; handoff never overwrites.
- Block or report only reachable with a mouse, or only after the match → reachable everywhere.
