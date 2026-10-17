# Content and phobias

**Load when:** S109 spiders, S110 insects/swarms, S111 snakes, S112 clowns/masks, S113 needles or
medical procedures, S114 blood/gore, S115 deep water, S116 heights, S117 clustered holes, S118 jump
scares, S119 traumatic themes, S120 gambling or addiction content, S121 a settings preview that would
show a trigger, S122 user, mod or procedural content that could bypass filters.
**Skip when:** an inventory of models, sprites, textures, animations, audio, text, cutscenes, UI art
and user content confirms none of these categories. Record the evidence in `ACCESSIBILITY.md`. Names
are not evidence: a character called "Spider" may not look like one.

## Options

| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
| F01 | Spider appearance, graded levels | `content.spiders` | 0 (original) | C | F, T |
| F03 | Creature sound and animation replacement (part of each graded level) | `content.spiders` (levels 1-3) | Original | C | F, N |
| F02 | Other creature substitution: insects/swarms, snakes, clowns | none yet, add one per tag | Original | C (A for full variant sets) | F, T |
| F15 | Clustered-hole texture alternatives | none yet | Original | C | F, N |
| F04 | Deep-water visibility and traversal relief | none yet | Off | C | F, T |
| F05 | Heights and edge presentation | none yet | Off | A | F, O |
| F10 | Needle and medical imagery replacement | none yet | Off | C | F, T |
| F06 | Blood amount | `content.blood` | full | C | F, N |
| F07 | Gore and dismemberment | `content.gore` | full | C | F, T |
| F11 | Jump-scare reduction | `content.jump_scares` (full / reduced) | full | C | F, T, N |
| F12 | Layered content notices | `content.warnings` (notices only) | On | C | F, T |
| F13 | Pre-scene warnings, early enough to act | `content.warnings` (scene notices), `content.scare_warnings` (heads-up before scripted scares) | On / Off | C | F, T |
| F14 | Sensitive scene skip with factual recap | `content.scene_skip` | On (offered) | C | F, T |
| F08 | Nudity filter | none yet | Off | C | F, S |
| F09 | Profanity filter | none yet | Off | C | F, S |

Tags without a registry key need one entry each in the project's registry copy, shaped like
`content.spiders` (integer levels, `profile` scope, live apply, each level described). Owned by other
guides: sudden loud sounds (H14) in [audio-and-captions](audio-and-captions.md); passive enemies
(D10), infinite oxygen and fall protection in [difficulty-and-assists](difficulty-and-assists.md);
flashes inside scares in [photosensitivity](photosensitivity.md).

## How to implement

### 1. Inventory the content, not the file names

For each signal, trace what the player can actually see, hear or read. Check every channel:

- **World:** actors, corpses, ragdolls, shadows, reflections, webs, nests, decals, baked textures,
  posters, statues, mounts, trophies, procedural surfaces.
- **UI:** inventory icons, bestiary or codex pages, quest log, portraits, loading screens, tips,
  achievement art shown in game, map icons, menu backgrounds.
- **Video, audio, text:** intros, cutscenes, recaps, death screens; movement, idle, attack, death
  and off-screen sounds; captions such as `[Spider skittering above]`, narration, item descriptions.

Hogwarts Legacy's arachnophobia patch is the cautionary example: models, some sounds, small-spider
effects and corpses changed, but Field Guide images did not.

### 2. Tag at authoring time and swap presentation only (ContentFilter)

Use the [ContentFilter](../architecture.md) service. Each tagged thing has one logical identity and
several presentation variants:

```
logical_id: enemy.cave_creature      tags: [arachnid]
variants:   original | simplified | replaced   (mesh or sprite, animation set, sound set, icon, portrait)
shared:     hitbox, hurtbox, AI, damage, timing, navigation, rewards, network replication
```

- Gameplay never reads the filter level. Only presentation code asks ContentFilter for a variant.
- The replacement must show the real hittable volume. A tiny icon over a large invisible hitbox is a
  new barrier.
- Removal, passivity or encounter bypass changes gameplay. Offer it as a **separate**, described
  setting. Satisfactory keeps its cat-sprite replacement apart from hostility controls.
- CaptionBus and Narrator follow the variant. A filtered creature captions as `[Creature skittering
  above]`, never with the original name or sound description.

### 3. First render and live swaps

- Resolve the variant **before first render**. Load the setting before the level streams.
- When the setting changes mid-game, swap every tagged instance in the same frame. Preload the
  variants for tagged content in the loaded area so no original frame appears while an asset streams.
  If a variant is not ready, keep that node hidden until it is.
- Invalidate cached thumbnails, bestiary renders and paused video frames.
- Apply the same rules to newly spawned actors, reloaded saves, deaths and respawns.

### 4. Graded levels (F01, F03)

`content.spiders` uses 0 = original, 1 = no hair or fangs, 2 = no legs and a simplified body,
3 = a non-spider shape and sounds. Four levels are a starting point, not a required count: Grounded
ships the original plus five reduction steps. Each level must say what it changes. Visuals alone may not be enough: Xbox's
Grounded write-up treats texture, eyes and audio as separate contributors. Replace the crawling gait
and skitter sounds at the levels that promise it. Reuse the same graded pattern for other tags.

### 5. Category notes

- **Insects and swarms (S110):** reduce close-ups and swarm density in the presentation layer. Keep
  threat count, direction and timing readable through outlines, captions or a counter. Buzzing is a
  separate sound control (APICO ships one).
- **Snakes (S111):** shape, slithering motion and hissing are three separate triggers. Replace all
  three, or say which remain.
- **Clowns, masks, faces (S112):** include masks, posters, portraits and laughter audio. A costume
  swap does not cover voice or story content. Pair it with a warning or a text alternative.
- **Needles (S113):** find injection close-ups in openings, healing items and cutscenes. Offer an
  alternate use animation and sound, or a camera cut. Keep the health or state feedback.
- **Blood and gore (S114):** two independent keys. `content.blood` (full, reduced, off) covers
  blood splatter, pools, wound decals and blood particles. `content.gore` (full, reduced, off)
  covers dismemberment, exposed injuries and graphic corpse detail. A player can keep blood and
  remove gore, or the reverse. Tag assets with the right category: corpses, death screens,
  cinematics and inventory art often carry both. Keep hit confirmation through another cue.
  Recolouring blood alone is not "off".
- **Deep water (S115):** oxygen relief is not fear relief. Fear of drowning and fear of vast dark
  water are different. Offer clearer water, visible floor and boundaries, less depth fog, route help,
  and a skip or alternate route where scoped. List which you ship.
- **Heights (S116):** opaque railings, less vertiginous framing, no forced look-down, skips for
  isolated height sections. Fall protection (`assist.no_fall_death`) is a separate option.
- **Clustered holes (S117):** check walls, floors, nests and procedural textures, not only enemies.
  Provide alternate textures, normal maps or plain surfaces. A detour route does not answer a request
  to make an area viewable.
- **Jump scares (S118):** two independent keys. `content.jump_scares` = full or reduced sets the
  scare's intensity: reduced softens the sting, the camera snap and sudden spawns.
  `content.scare_warnings` (on/off) adds a short, consistent, shown-and-announced heads-up before
  each scripted scare. Any combination works: a player can keep authored full-strength scares and
  still get the warning, or reduce scares without warnings. Both are independent of enemy
  lethality: an invincible player can still be startled, and a scare-reduced game can still be
  lethal. Any flash in a scare still goes through FlashGuard.

### 6. Warnings, previews and skips (F12-F14, S119-S121)

- **Layered notices.** Layer 1: a category list on the store page and on first launch, before any
  content. Layer 2: specific detail (which scene, how long, what is shown) only on request, so the
  player controls spoilers. Do not assume everyone ranks spoilers above comfort.
- **Pre-scene warnings** appear early enough to pause, change a filter or skip. They are narrated,
  readable, untimed and not graphic themselves. `content.warnings` controls the notices only.
- **Skip access is separate.** `content.scene_skip` decides whether a listed scene offers a skip.
  Turning warnings off never removes the skip, and a player who wants the warning but never the
  skip prompt can have that. With warnings off, the skip stays reachable from the pause menu.
- **Previews start most-filtered.** The settings screen shows the most abstract variant first and a
  muted sample. The original appears only after an explicit "Show original" action. Never autoplay a
  trigger to demonstrate its removal.
- **Skip** calls `AssistPolicy.request_scene_skip(scene_id)` (`RequestSceneSkip` in Unity and
  Unreal), which is gated on `content.scene_skip` only. It is not `request_skip(challenge_id)`:
  that one is for challenges and is gated on `assist.skip_available`, so a player who turns
  challenge skips off keeps sensitive-scene skips, and a story game whose registry has only
  `content.scene_skip` still works. Both requests share the normal completion path: same flags,
  items, checkpoint and achievements, and an optional factual text recap. Saving and reloading
  right after the skip must work. See [architecture](../architecture.md) section 2.
- **Concealment is not silence.** If a scene is hidden behind a card or blur, either mute or
  replace its audio too, or say clearly that audio continues.

### 7. Profanity and nudity (F08, F09)

Use alternate lines or bleeps, and keep subtitles, captions and audio in agreement. Nudity filters
use ContentFilter clothing variants. Scene meaning and progression stay the same.

### 8. User content, mods and procedural generation (S122)

- Mods declare content tags in their manifest. Untagged content counts as unfiltered.
- Expose the tag and variant hooks as a documented mod API (core-policy section 8).
- Say exactly what is covered: "Filters apply to official content. Mods and player-made levels may
  not be filtered."
- Check procedural generators for hole clusters, gore and creature shapes, not only authored assets.

### 9. Honest naming

| Avoid | Use |
|---|---|
| "Arachnophobia safe" | "Spider appearance: Simplified. Legs, eyes and skittering sounds replaced. Attacks and hitboxes unchanged." |
| "No gore" when only cutscenes change | "Hides gore in listed cutscenes. Combat blood is set separately." |
| "Thalassophobia mode" | "Clearer water and visible sea floor. Breathing is unchanged." |
| "Horror off" | "Jump scares: Reduced. Sudden sounds and camera snaps are softened. Enemies still attack." |

## Shipped examples

- Grounded: graded spider appearance from original through five reductions, difficulty unchanged,
  settable from loading screens and in game. [Obsidian](https://grounded.obsidian.net/news/grounded/accessibility-features).
  Research write-up on textures, eyes and audio: [Xbox Wire](https://news.xbox.com/en-us/2020/07/27/grounded-arachnophobia-mode-details/).
  Later patches extended it to [spider-head mounts](https://www.obsidian.net/news/grounded/shroom-and-doom-update-out-now)
  and a [Broodmother trophy](https://grounded.obsidian.net/news/get-ready-as-the-bugs-strike-back),
  which shows why secondary depictions need coverage. Appearance changes do not reduce aggression.
- Satisfactory: replaces Stingers with cat sprites; hostility is a separate control.
  [Official wiki](https://satisfactory.wiki.gg/wiki/Settings). A replacement can still startle.
- Hogwarts Legacy: May 2023 patch changed models, some sounds, small-spider effects and corpses;
  Field Guide pictures stayed the same. [Patch notes, Spanish](https://portkeygamessupport.wbgames.com/hc/es-419/articles/16304407055251-Notas-del-parche-de-Hogwarts-Legacy-para-PlayStation-5-Xbox-Series-X-S-y-PC-4-5-2023).
- Call of Duty: Black Ops 6: arachnophobia toggle changes spider-like Zombies enemies without
  changing gameplay. [Blizzard news](https://news.blizzard.com/en-gb/article/24151767/call-of-duty-black-ops-6-accessibility-updates-and-new-features).
  Movement and threat may still trigger fear.
- Horizon Forbidden West: Patch 1.21 (PS5) added thalassophobia support, reported as clearer
  underwater visibility and unlimited breath. [Announcement](https://www.reddit.com/r/horizon/comments/12qo4vr/horizon_forbidden_west_patch_121/).
  Players in r/thalassophobia said it did not resolve their fear of deep water.
  [Discussion](https://www.reddit.com/r/thalassophobia/comments/12uski1/). Respect that disagreement.
- Dead Space (2023): content warnings plus hiding selected disturbing scenes; the scene audio still
  plays. [EA](https://www.ea.com/ea-play/news/accessibility-and-player-choice-in-dead-space),
  [content description](https://www.ea.com/games/dead-space/dead-space/content-warning?isLocalized=true).
  Not a global gore filter.
- SOMA: Safe Mode stops creatures from killing the player but keeps the horror atmosphere.
  [Frictional](https://frictionalgames.com/2017-11-what-is-somas-safe-mode/). Lethality and fear
  are separate.
- Outer Wilds: Echoes of the Eye: Reduced Frights. [PlayStation Blog](https://blog.playstation.com/2021/07/29/more-to-explore-outer-wilds-echoes-of-the-eye-expansion-lands-september-28/).
  The announcement does not list every change.
- Core Keeper: a player asked for trypophobia options because grub walls and floors were hard to
  look at. [Reddit](https://www.reddit.com/r/CoreKeeperGame/comments/1mpip0z/trypophobia/). A
  request, not a shipped feature. No shipped trypophobia filter was verified in this skill's research.
- BioShock: players report discomfort with injection animations.
  [r/Games](https://www.reddit.com/r/Games/comments/fxs96l/what_phobias_have_games_tripped_for_you_or_how/).
  Player reports only; no needle option is claimed.
- Jackbox Party Pack 8: profanity filtering and moderation tools.
  [Jackbox](https://www.jackboxgames.com/streaming-moderation-accessibility-features-jackbox-party-pack-eight/).
- XAG 123 recommends advance content information and ways to customise or bypass distressing
  content. [Microsoft](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/123).

## Verify

- [ ] Every tagged category has an inventory entry covering actors, corpses, world art, UI, video,
      audio and text, with known gaps listed.
- [ ] Set each filter level before loading a level, with level 0 (original) as the control: each
      level shows exactly the substitutions it promises on every channel it names, anywhere on the
      content path, including death, inventory, codex, loading screens and reflections. Only a
      level that promises full replacement must show no original model, sound, icon or portrait.
- [ ] Change the level live with tagged actors on screen: the swap happens with no original frame,
      and cached thumbnails update.
- [ ] With the filter on, hitboxes, attack timing, AI, damage and rewards match the original
      (compare a recorded encounter or automated test).
- [ ] Captions and narration name the replacement, not the original.
- [ ] The settings preview opens on the most filtered variant, with audio muted, and shows the
      original only after an explicit request.
- [ ] Content notices appear before first exposure, are narrated and untimed, and detail is opt-in.
- [ ] A scene skip grants the same flags, items, checkpoint and achievement events as finishing the
      scene. Save and reload straight after it.
- [ ] Warnings off + scene skip on, and warnings on + scene skip off: each behaves as described.
- [ ] `content.scene_skip` × `assist.skip_available`, all four combinations: a sensitive-scene skip
      is accepted exactly when `content.scene_skip` is on, a challenge skip exactly when
      `assist.skip_available` is on. Repeat with a registry that contains only `content.scene_skip`.
- [ ] Blood off + gore full, and blood full + gore off: each category changes alone, including
      corpses, cinematics and inventory art.
- [ ] Jump scares: all four combinations of `content.jump_scares` (full, reduced) and
      `content.scare_warnings` (off, on) behave as described, including full scares with warnings,
      with enemy damage both on and off. FlashGuard still limits any flash.
- [ ] Filter settings persist across restart, load an old save without reverting, and do not appear
      on save cards or endings.
- [ ] Mod or user content: untagged content is disclosed as unfiltered in the option description.
- [ ] Testing with affected players is planned or recorded. Do not expose testers to unreviewed
      triggers; a developer judging a cartoon replacement "harmless" is not evidence.

## Common mistakes

- Swapping the model but keeping the skitter, hiss or crawling gait → replace motion and sound at
  the levels that promise it, or say what remains.
- Filtering the live creature but not corpses, icons, trophies or loading art → audit the whole
  inventory in section 1.
- Previewing the original spider to show what the filter removes → start most-filtered; the
  original only on explicit request.
- Calling a filter "arachnophobia safe" or "thalassophobia mode" → describe the exact change and
  its exclusions.
- Treating infinite oxygen as a deep-water fear option → it fixes a resource barrier, not the
  imagery. Offer visibility relief or a skip, and name each.
- Tying jump-scare reduction to difficulty or enemy damage → keep `content.jump_scares` separate from
  AssistPolicy.
- Giving the scare warning only to players who also reduce scares → `content.scare_warnings` is its
  own key.
- Routing sensitive-scene skips through the challenge skip → use `request_scene_skip`, gated on
  `content.scene_skip`.
- Making the filter change hitboxes or remove enemies silently → presentation only; removal is a
  separate, described setting.
- Concealing a gore scene behind a black card while its audio plays → mute or replace the audio, or
  state that it continues.
- A warning shown after the content starts, or one that describes the content graphically → show
  it before exposure, in neutral words, with detail on request.
- Assuming mods and procedural levels inherit filters → expose tag hooks and disclose coverage.
- Adding spider, snake or clown options to a game with no such content → record the absence instead
  (core-policy section 4).
