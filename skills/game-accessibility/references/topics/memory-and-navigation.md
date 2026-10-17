# Memory and navigation

**Load when:** S081 quests or objectives, S082 long or multi-session story, S083 ability-gated
backtracking, S084 navigation or exploration, S085 screenshot or image journals, S086 puzzles, S091
irreversible or destructive actions, S095 large cast or relationships, S012 compulsory tutorials,
S030 many inputs to remember, S066 dialogue worth reviewing. Saves (C22, D26) and destructive-action
confirmation (C29) apply to almost every game. **Skip when:** a single-session game with no
objectives, no story, no saves and no puzzles. Record that evidence.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| C01 | Player-paced text | `text.auto_advance` | false | B | C, H, V |
| C02 | Plain-language descriptions and instructions | none (writing rule) | On | B | C, S |
| C03 | Optional keyword emphasis | `game.text.keyword_emphasis` | Off | C | C |
| C04 | Interactive tutorials | none (design) | On | C | C, M |
| C05 | Replay tutorials on demand, with current bindings | none (menu entry) | Available | C | C |
| C06 | Practice without failure | none (menu entry) | Available | C | C, M, T |
| C07 | Control reminder overlay, from live bindings | `game.hud.control_reminders` | Off | C | C, M |
| C08 | Current objective, with a reminder action | `memory.objective_reminder` | `on_request` | C | C |
| C09 | Completed-objective journal | none (journal) | On | C | C |
| C10 | Subtask checklist with prerequisites | `memory.objective_reminder` | `on_request` | C | C |
| C11 | Recap on load: facts only, no spoilers | `memory.recap_on_load` | false | C | C, S |
| C12 | Glossary and codex by story knowledge | none (menu entry) | Available | C | C |
| C13 | Dialogue and instruction replay | none (log) | Available | C | C, H |
| C14 | Contextual hints | `memory.hints` | `on_request` | C | C, T |
| C15 | Graduated hints: nudge, hint, solution | `memory.hints` | `on_request` | C | C, T |
| C16 | Optional solution with spoiler control | `memory.hints` | `on_request` | C | C, T |
| C17 | Waypoint marker, independent of difficulty | `guidance.objective_marker` | `standard` | C | C, V |
| C18 | Route trail, independent of other highlights | `guidance.trail` | `follow` (uses `guidance.level`) | C | C, V |
| C19 | Collectible and prerequisite counters | none (journal) | On | C | C |
| C20 | Map marker filters with a list view | `game.map.filters` | All shown | C | C, N |
| C21 | Personal map pins and notes, with text | none (map tool) | Available | C | C |
| C22 | Save labels: text, time, place, objective | none (save UI) | On | C | C, V |
| C23 | Notification duration, pin and dismiss | `notifications.mode` (log) | Designer | B | C, V, H |
| C24 | Persistent instruction reference before a challenge | none (UI) | On | C | C |
| C25 | Instructions as text, demonstration and speech | none (design) | Designer | C | C, V |
| C26 | Simplified summaries with full detail available | none (design) | Off | C | C |
| C27 | Direct Continue from the title | none (menu) | On | B | C, M |
| C28 | Undo reversible mistakes | none (design) | Where meaningful | C | C, M |
| C29 | Destructive-action confirmation | none (design) | On | B | C, M |
| C30 | Settings preview and per-category reset | none (settings UI) | On | B | C, V, M |
| D26 | Autosave history; never overwrite the only good save | `game.save.autosave_interval` | On | C | C, M, T |

Text reveal speed and dialogue reading: [text-and-readability](text-and-readability.md). Puzzle and
encounter skips (D22, D23) and save-anywhere (D25): [difficulty-and-assists](difficulty-and-assists.md).
Guidance paint presentation: [sensory-load-and-guidance](sensory-load-and-guidance.md).

## How to implement

### Objectives, journal and checklist (C08–C10, C19)

- Build objectives from game state, never from free text. Each has a concrete next action ("Bring
  the key to Mara in the mill"), not a poetic line of dialogue.
- Separate main, optional and expired tasks. Let the player focus one objective.
- The journal records actual completion. Checklists show prerequisites and counts ("3 of 5 gears").
- `memory.objective_reminder`: `off`, `on_request` (a remappable action shows the goal) or `periodic`.
- Objective text that names controls uses live bindings.

### Recap on load (C11)

- Compose the recap from recorded facts in **this save**: current goal, last meaningful action,
  where the player is, who matters right now, and a link to the controls and journal.
- Never generate narrative and never reveal anything the save has not reached.
- Show it as a dismissible screen that waits for the player. A recap that vanishes when loading
  finishes is useless. Keep it available from the pause menu as well.

### Glossary and codex (C12)

- Version entries by story knowledge: a newly met character gets the facts revealed so far.
- Searchable, and usable with keyboard, controller and narration.
- Final Fantasy XVI's Active Time Lore shows entries relevant to the current scene, including while
  a cutscene is paused. Prefer that to a huge unsearchable encyclopedia.

### Replay and reference (C05, C07, C13, C24, C25)

- A dialogue log with speaker names. Opening it never advances the conversation or discards a
  pending choice.
- Tutorial replay runs in a sandbox, keeps campaign progress and uses current bindings.
- Instructions for a challenge stay readable during it. Show important instructions as text plus a
  demonstration, and voice them or let `Narrator` read them.
- The control reminder overlay reads live bindings for the active device.

### Graduated hints (C14–C16)

```
hint_level(puzzle_id) -> 0 none | 1 nudge | 2 clear hint | 3 solution
```

- `memory.hints`: `off`, `on_request` or `auto`. Auto waits for a designer-set idle time and only
  offers a nudge; it never shows the solution unasked.
- Label the level before revealing it: "Show a nudge?" then "Show the solution?". Track which levels
  the player has seen.
- Keep "reminder of what you already know" separate from "new solution information".
- A skip is the step after the solution: [difficulty-and-assists](difficulty-and-assists.md).

### Waypoints and route trail (C17, C18, C20)

- Navigation help is independent of difficulty and of guidance paint. A player can turn paint off
  (`guidance.paint`) and keep the waypoint and the trail (`guidance.trail`), or the reverse.
- Say what a marker points at: the objective, the next reachable passage or a collectible.
- Route along walkable paths and recalculate after traversal and objective changes. An arrow through
  a wall is not a route.
- Marker filters by category, saved between sessions, plus a list view usable with narration.

### Map pins and notes (C21, S083, S085)

- A pin always has a text label. A screenshot is optional extra, never the only record: blind
  players cannot use an image-only note.
- Pins are editable and removable and cost no scarce resource.
- Record why a route is blocked in text ("Needs double jump"). After a new ability, offer to show
  newly reachable pins.

### Saves (C22, C27, D26)

- Save labels pair any thumbnail with text: chapter, location, current objective, play time and real
  date. **No assist badge** on the card (core policy).
- Keep a rotating history of autosaves plus manual slots. Never overwrite the only recoverable
  state. Do not autosave while falling, dying, mid-chase or in a soft-lock; validate the state first.
- Show when a save has finished. Continue loads the latest good save in one step, after accessibility
  settings are reachable.
- Save anywhere or suspend: [difficulty-and-assists](difficulty-and-assists.md).

### Notifications (C23)

Let players extend, pin or dismiss transient messages. Everything goes to the log described in
[sensory-load-and-guidance](sensory-load-and-guidance.md).

### Destructive actions and undo (C28, C29)

- Confirmation names the item and the consequence: "Sell Iron Sword for 40 gold? You cannot buy it
  back." Default focus is on Cancel. Never require a long hold as the only way to confirm.
- Undo where meaningful: buy-back, re-pick the last choice in a menu, restore a dropped item. State
  the limits plainly.

### Summaries and plain language (C02, C03, C26)

Write instructions in short sentences. Keyword emphasis uses weight or an icon as well as colour.
Summaries sit on top of the full detail and never replace it.

## Shipped examples

- Dragon Quest XI S: story recap when returning after a break.
  [Nintendo Wire](https://nintendowire.com/reviews/dragon-quest-xi-s-echoes-of-an-elusive-age-definitive-edition/)
- Horizon Forbidden West: "Story So Far" and optional ability reminders.
  [PlayStation Blog](https://blog.playstation.com/2022/02/10/accessibility-features-in-horizon-forbidden-west/)
- Final Fantasy XVI: Active Time Lore, contextual entries during scenes and while paused.
  [Square Enix](https://na.finalfantasy.com/news/2277),
  [hands-on](https://blog.playstation.com/2023/02/28/hands-on-with-final-fantasy-xvi-new-gameplay-details/)
  (pre-release description).
- Control: Game Help starts with hints and grows more explicit. PS5-specific and originally tied to
  PlayStation Plus. [Remedy](https://legacy.controlgame.com/es/game-help-on-playstation-5-and-assist-mode-in-control-a-deep-dive/)
- Chicory: A Colorful Tale: in-world hints the player chooses to request.
  [Can I Play That?](https://caniplaythat.com/2021/06/10/chicory-a-colorful-tale-can-i-play-that-accessibility-review/)
- Prince of Persia: The Lost Crown: Memory Shards pin screenshots to map positions. Image-only; the
  text label above is this skill's addition.
  [Microsoft](https://developer.microsoft.com/en-us/games/articles/2025/03/prince-of-persia-the-lost-crown-accessibility/)
- Baldur's Gate 3: conversation history without leaving the conversation.
  [Larian forum](https://forums.larian.com/ubbthreads.php?Number=873064&ubb=showflat)
- Persona 5 Royal: dialogue log and optional auto-advance; the same review notes caption gaps.
  [Can I Play That?](https://caniplaythat.com/2020/04/02/deaf-hoh-review-persona-5-royal/)
- Starfield (launch, 2023): Steve Saylor reports unreliable guidance. A test case, not the current
  build. [GamesRadar](https://www.gamesradar.com/starfield-accessibility-verdict/)

## Verify

Scenarios adapted from community and advocate evidence:

- [ ] Load a mid-campaign save as if after a month away. Using only in-game tools, identify who,
      where, why and the next action.
- [ ] Miss a conversation and a notification. Recover the objective and any code or instruction.
- [ ] Remap an action after its tutorial. Replayed instructions, reminders and objective text show
      the new binding.
- [ ] Ask for the first hint. The solution does not appear until requested.
- [ ] Pause during a cutscene and during a timer. Both resume coherently.
- [ ] Stop during a long encounter. The documented save or suspend promise holds.
- [ ] Load an older save. Recap and codex contain no facts from later saves or other playthroughs.

Also:

- [ ] Guidance paint Off + waypoint On, and paint On + waypoint Off, both work.
- [ ] The waypoint routes around walls and updates after the objective changes.
- [ ] A map pin has a text label and is read by narration.
- [ ] Force a soft-lock right after a checkpoint. An older autosave still restores a good state.
- [ ] Save cards show no assist marker. Destructive actions name the item and default to Cancel.
- [ ] `memory.*` settings persist after restart and are not restored from an old story save.

## Common mistakes

- Journal repeats a vague line of dialogue → write a concrete next action from game state.
- Recap on the loading screen disappears → make it wait for the player.
- Recap or codex spoils later events → filter by what this save has revealed.
- First hint is the answer → step from nudge to hint to solution, labelled.
- Waypoints only on Easy → navigation help is independent of difficulty.
- Screenshot-only map notes → always add a text label.
- One autosave slot overwritten during a soft-lock → keep a rotating history.
