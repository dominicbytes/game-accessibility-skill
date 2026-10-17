# Baseline (RED): agent behaviour WITHOUT the skill

Date: 2026-10-17. Model: Claude Sonnet 5.5 general-purpose subagent, no files, text only.

## Scenario A: proactive build (first-person survival horror, Godot)
Prompt signals: giant spiders, infected crew, keypad + wiring puzzles, scarce batteries/ammo,
jump scares, chase sequences, QTE struggle when grabbed, voiced radio dialogue, 6-week milestone.

Included unprompted: subtitle data shape (size, background, speaker), head-bob toggle, camera
shake and flash-reduction levels, text size, full rebinding, hold-vs-toggle, FOV, sensitivity,
invert Y, brightness/gamma.

Missed (failures the skill must fix):
1. **Content signals ignored.** Giant spiders -> no arachnophobia mode. Jump scares -> no
   intensity/warning option. Chases -> no chase skip. Gore-capable infected -> no gore toggle.
2. **Mechanic signals deferred without accessibility hooks.** QTE struggle pushed to M2 with a
   `begin_grab` hook but no auto-complete / hold-instead-of-mash / timing-window plan.
   Wiring puzzle (classically colour-coded) -> no colour-independent encoding plan.
3. **No difficulty or assists at all** in a scarcity game: no invincibility, damage scale,
   infinite batteries/ammo, enemy perception scale, no "assists never affect achievements" rule.
4. **Accessibility is a settings tab built in weeks 4-5**, not a service the gameplay systems
   query from week 1. The "Accessibility" bullet is the 4th sub-list of a settings item.
5. **Introduced new barriers without noticing:** health shown only by vignette + desaturation
   (visual-only, colour-ish); low-battery flashlight *flicker* (photosensitivity risk, no
   off switch); stealth spider "relies on hearing" (no sound visualisation / awareness indicator
   for deaf/HoH players).
6. No cognitive/memory aids (objective reminder, journal, recap on load), no hint system for puzzles.
7. No first-launch safety (settings before first flash/loud scare), no screen reader / TTS for
   menus, no dyslexia-friendly font, no colour-blind option, no minimal-HUD / crosshair options.
8. No verification of options (tests cover save round trip, inventory, AI only).

## Scenario B: retrofit audit (Unity turn-based JRPG, 3 weeks to Steam launch)
Prompt signals: 40h text-heavy story, colour-coded elements, timed button presses,
world map, side quests, Hard-difficulty boss achievements.

Included unprompted (good): icons + labels for elements, colour-blind palette, timing-window
scale + auto-succeed, text size + box contrast, remapping, text speed / auto-advance / backlog,
journal + recap, volume buses, flash threshold, battle speed, easy mode, Steam accessibility tags.

Missed / wrong (failures the skill must fix):
1. **Recommended penalising assists:** "Exclude the assisted modes from the Hard boss
   achievements" (item 2), then contradicted itself in item 8. No firm no-penalty rule.
2. **Dismissed screen reader / TTS** ("expensive, skip") for a 40-hour text-only game, with no
   cheaper middle ground offered (TTS of dialogue/menus via platform TTS).
3. No dyslexia-friendly font option, no line spacing, no text-background option beyond the box.
4. No encounter-rate / auto-battle / skip-battle options; no mash/hold alternatives check.
5. No presets, no first-launch accessibility prompt, no "options reachable from title" rule
   beyond one line, no "assist on" indicator, no reversibility rule.
6. No automated verification plan (colour-blind simulation screenshots, scenario completion with
   each assist, settings persistence).

## Patterns to counter in the skill
- Agents treat accessibility as a *settings menu feature*, not as hooks owned by each gameplay system.
- Agents do not map *content and mechanic signals* to options (spiders -> arachnophobia, QTE -> auto/hold,
  chase -> skip, colour puzzle -> symbols, hearing stealth -> sound visualisation).
- Agents defer or cut cheap high-value items ("expensive", "M2", "nice to have").
- Agents waver on achievements; need a hard rule.
- Agents create new barriers while designing (visual-only health, flicker) without noticing.
- Agents do not verify options.
