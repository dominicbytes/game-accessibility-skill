# Structured UI and strategy

Inventories, crafting, cards and deckbuilders, tactics grids, RTS, 4X, grand strategy, tower
defence, auto-battlers, city builders, management, farming and idle games.

**Load when:** S087 inventory or equipment, S088 crafting or recipes, S089 stat comparison, S090
nested tooltips or codex, S091 irreversible or destructive actions, S092 hidden rules, S093
multi-stage plans or queues, S094 resource overlays or charts, S078 rapidly changing state, S146
RTS orders under continuous simulation, S147 turn or grid targeting, S148 wave or shop countdowns,
S149 card or deck state, S150 construction and placement, S152 farming day cycles, S154 idle
production or prestige, S155 numeracy-heavy rules, S157 turn-based battle commands.
**Skip when:** none of these views exist. "Turn-based" or "auto-battle" is not a reason to skip:
automated combat still has shops, placement, tooltips and timers.

Read only the subsections for this game's systems.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| C31, C32 | Search, sort, filter and favourites for items, recipes, cards, units | none yet | Available | C | C, M, V |
| C26 | Compare view and plain summaries, detail on request | none yet | Available | C | C, V |
| C33, C19 | Pinned recipes, missing materials and prerequisite counters | none yet | On | C | C |
| C34, M15 | Batch and repeat: craft many, buy max, queue builds, early automation | none yet | Available | C | M, C |
| C35 | Craft or build from nearby storage | none yet | Available | C | M, C |
| C36 | Auto-equip and loadouts (opt-in) | none yet | Off | C | C, M |
| M35 | Automatic pickup and collection | none yet | Off | C | M |
| M32 | Select, then place (no drag) | none yet | Available | C | M |
| M21 | Keyboard or controller only, with discrete grid navigation | none yet | On | C | M, V |
| M40 | Cursor snapping to cells and targets | none yet | Off | C | M, V |
| D27 | Pause anywhere; issue and queue orders while paused | none (design requirement) | On in single-player | B | M, C |
| D14 | Simulation speed | `assist.game_speed` | 1.0 | C | M, C |
| D29 | Extend or remove turn, wave and shop timers | `assist.timers` | `normal` | C | C, M, T |
| C37 | Forecasts and outcome previews | none yet | Available | C | C |
| C28 | Undo reversible moves | none yet | Available where rules allow | C | C, M |
| C29 | Confirm destructive actions: sell, scrap, prestige, end turn | none (design requirement) | On | B | C, M |
| C38 | Focusable, persistent tooltips; no hover-only information | none (design requirement) | On | B | V, M, C |
| R10 | Text descriptions for icons, cards, charts and overlays | none (design requirement) | On | B | V |
| R02 | Read current state on demand (resources, selection, tile) | none yet | Off | C | V, C |
| R07 | Narration verbosity | none yet | Designer value | C | V, C |
| V25 | Symbols or labels paired with every colour (rarity, owner, overlay) | ColorTokens | On | B | V, C |
| V24 | Custom colours per semantic role | `visual.color_profile` = `custom`, `visual.custom_colors` | `none` | C | V |
| N08 | Notification categories; everything kept in an event log | `notifications.mode` | `all` | C | N, C |
| D13 | Lower resource or crafting costs | none yet | Off | C | C, T, M |
| D25 | Save or suspend mid-run, mid-turn | none (design requirement) | On | C | M, C, S |

Add missing keys to the project's registry copy with `profile` scope. Related guides:
[text-and-readability](text-and-readability.md) for scale and reflow,
[screen-reader-and-voice](screen-reader-and-voice.md) for narration plumbing,
[timing-and-speed](timing-and-speed.md) for clocks, [touch-and-mobile](touch-and-mobile.md) for
touch boards.

## How to implement

### 1. Data first, pixels second

Every structured view renders a model: a list of entries with name, quantity, properties, state and
available actions. The same model drives the visual grid, search, sort, comparison, tooltips and
narration. If a fact exists only in an icon position or a colour, nothing else can reach it. Expose
the model to the engine accessibility tree or the Narrator (see [architecture](../architecture.md)).

### 2. Inventory and crafting (S087-S089, S093; C31-C36)

- Search by name, sort by type, value or recency, filter by category, and pin favourites.
- Compare a hovered or focused item with the equipped one. Show differences as text and signs
  ("+4 armour, -2 speed"), not colour alone.
- Recipes list missing materials with counts. Crafting can draw from storage and craft in batches.
- Queues show progress and can be reordered or cancelled.
- After a stack changes, focus stays on the same slot or the nearest valid one. Selling and
  scrapping confirm with the item name and count.
- Auto-pickup and auto-equip are optional. Filter what auto-pickup collects so it does not flood the
  inventory.

### 3. Placement, grids and targeting (S147, S150)

- Select a building, unit or card, then choose a destination. Cancel at any point. No held drag.
- A keyboard or controller cursor moves cell by cell. Rotate with buttons. Placement shows a
  preview and validity before commit.
- Cycle through legal targets with a button instead of pointing at them.
- Narrate position as coordinates plus meaning: "Row 3, column 5. Empty. In range of 2 towers."
- Numeric transforms and snapping replace precise gizmo dragging.

### 4. Time pressure in real-time strategy (S146, S148)

- Single-player: pause at any time and give, queue and cancel orders while paused. Simulation speed
  (`assist.game_speed`) slows only simulation time.
- Tower defence: start the next wave manually. Build while paused.
- Auto-battlers and shop phases: extend or remove preparation timers with `assist.timers` in
  single-player or asynchronous modes. Synchronous online timers need session rules; see
  [multiplayer-and-communication](multiplayer-and-communication.md).
- 4X and turn-based: slowing real time is irrelevant when decisions already wait. Focus on end-turn
  confirmation, idle-unit warnings and readable turn summaries.
- Turn-based battle commands (S157): the command menu follows the same data model, focus and
  narration rules as other lists. Encounter rate, auto-battle and battle speed (D31-D33) are in
  [difficulty-and-assists](difficulty-and-assists.md).

### 5. Tooltips and rules (S090, S092, S155; C38)

- Tooltips open by focus as well as hover, stay until dismissed, and never time out.
- Nested tooltips and glossary terms are focusable links with a Back action. Offer an "always
  expanded" mode.
- Tooltips follow `text.scale` and narrate in reading order.
- Explain rules in plain language with the exact numbers available on request. Mark predictions as
  predictions.

### 6. Forecasts, undo and confirmation (S091)

- Show outcome previews (attack odds, resulting resources) before commit.
- Undo moves where the rules allow it. Undo must not reveal hidden information such as an
  opponent's hand or the next draw.
- Destructive actions name the item and the consequence. Prestige confirmation lists what resets and
  what stays. A long hold is never the only way to confirm.

### 7. Cards and decks (S149)

Each card can be read in full: name, cost, type, rules text, current modifiers, in a fixed order.
Zoom any card. Hand, deck counts and discard history are browsable. Reorder by select-and-place.
Never leak future draws through narration or sort order.

### 8. Notifications and event logs (S062, S078)

Group notifications by category (combat, economy, diplomacy, construction). Each category can be
shown, hidden or set to pause the game. Everything goes into a chronological event log that can be
read, narrated and used to jump to the event's location. `notifications.mode` limits pop-ups but
never drops log entries.

### 9. Narrated state (R02, R07, R10)

Give an on-demand summary action: "Gold 120, plus 5 per turn. Three idle workers. Wave 4 in 20
seconds." Fast counters announce on request or at thresholds, not on every tick. Verbosity is
adjustable, but state changes the player must act on are never dropped. Charts, overlays and heat
maps have a list or table view with values.

### 10. Idle and incremental games (S154)

Make automation available from the first repetitive task, not after hours of clicking. Offer buy
many and buy max. Offline rewards wait to be claimed. Summaries are quiet and on request. Label
every clickable element, and keep dynamic text updates from flooding a screen reader.

## Numbers that matter

| Value | Source and scope |
|---|---|
| Recurring spoken state every 7-10 s is acceptable | [XAG 106](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/106). Not a required cadence; never interrupt more important speech. |
| UI time limits adjustable to at least 10x, or removable | [XAG 116](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/116). UI limits, not core gameplay timers. |
| 4.5:1 contrast for card and tooltip text and essential icons | [XAG 102](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/102). |
| At most 80 characters per line for long text | [XAG 101](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/101), for blocks over two lines. |

## Shipped examples

- Age of Empires IV: first-launch and menu narration, Story Mode, text size, contrast, camera-pan
  speed. The official guide says narration does not cover the in-game HUD.
  [PC](https://www.ageofempires.com/age-iv-accessibility/), [Xbox](https://www.ageofempires.com/age-iv-on-xbox-accessibility-options/).
  Menu TTS is not strategy-state access.
- Civilization VII: February 2025 patch added text-to-speech narration.
  [Patch notes](https://support.civilization.com/hc/en-us/articles/38337649895187-Civilization-VII-Patch-Notes-February-5-2025).
  Coverage of keyboard, console and gameplay narration is not established.
- Crusader Kings III: the publisher's Steam listing declares adjustable text size, mouse-only play,
  saving at any time and play at the player's pace.
  [Steam](https://store.steampowered.com/app/1158310/Crusader_Kings_III/). Declarations, not an audit.
- core_error (jam game): colour-blind mode adds tile symbols. A player asked for the setting to
  persist between menu and game, and the developer acknowledged it.
  [itch.io](https://cakestorm.itch.io/core-error). Redundant encoding and a persistence bug in one.
- Super Auto Pets: asynchronous Arena mode without timers, beside timed Versus.
  [Steam](https://store.steampowered.com/app/1714040/Super_Auto_Pets/).
- Teamfight Tactics: Riot lists colour alternatives and play without button holds.
  [Riot](https://www.riotgames.com/en/accessibility). Its tracker lists rarity colours, tooltip
  readability and health bars as pending. [TFT tracker](https://followthefix.teamfighttactics.leagueoflegends.com/zh-tw/).
- Trimps: a ScreenReader.html variant with focusable tooltips.
  [GitHub](https://github.com/Trimps/Trimps.github.io/blob/master/ScreenReader.html). Open issues
  exist, so it is not a bug-free benchmark.
- A Dark Room: a blind contributor describes unlabelled clickable elements, verbose dynamic content
  and map access problems in the browser version. [Issue #70](https://github.com/doublespeakgames/adarkroom/issues/70).
- Balatro: Black Hole is a **community mod** adding narration, with documented gaps.
  [GitHub](https://github.com/game-a11y/Balatro-BlackHole). Not native support.
- Stardew Valley: Stardew Access is a **desktop community mod** for narrated tiles, tracking and grid
  movement. [GitHub](https://github.com/stardew-access/stardew-access). Not native or console support.
- APICO: automation, always-expanded tooltips, on-demand guidebooks, focus mode, less random waiting,
  and a separate option to turn off buzzing. [Whitethorn](https://whitethorngames.com/apico/accessibility).
- God of War Ragnarök: auto-equip and automatic pickups.
  [PlayStation](https://www.playstation.com/en-us/games/god-of-war-ragnarok/accessibility/).

## Verify

- [ ] Craft a required item using materials in several storage places, by keyboard or controller
      only, then by narration only.
- [ ] Place, rotate, move and demolish a structure without dragging, and cancel mid-placement.
- [ ] Receive an event, find its location from the log, review options, issue an order, cancel it.
- [ ] Every tooltip opens by focus, stays open, and its nested links work with Back.
- [ ] Compare two items or cards at maximum `text.scale` with no clipping and differences as text.
- [ ] Understand every overlay and rarity in grayscale.
- [ ] Pause and issue queued orders in single-player; timers extend or turn off with `assist.timers`.
- [ ] The on-demand summary reports the current state; fast counters do not flood narration.
- [ ] Undo never reveals hidden information; prestige and selling confirm with consequences.
- [ ] Idle games: reach the first automation without rapid clicking; reload after time away.
- [ ] Save mid-turn or mid-run, reload, and keep focus, selection and log.
- [ ] Settings such as colour profile persist between menu and game (the core_error lesson).

## Common mistakes

- Information only on hover → focusable, persistent tooltips.
- Drag-only placement or card play → select, then place.
- Rarity, ownership or overlays shown by colour only → symbols, labels and value lists.
- Menu narration advertised as full strategy access → narrate state or record the gap.
- Notifications that vanish with no log → keep a chronological, narratable log.
- Narrating every resource tick → summaries on request and thresholds.
- Automation unlocked only after hours of clicking → available from the first repetitive task.
- Prestige or sell with no confirmation → name the consequence and confirm.
- Undo that reveals the next card → keep hidden state hidden.
- Treating a community mod as native support → native access first; credit mods as extensions.
