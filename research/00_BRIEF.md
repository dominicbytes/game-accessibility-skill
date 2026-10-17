# Research brief: genre-neutral game accessibility skill

## Goal
We are building an open-source "agent skill" (a SKILL.md plus reference files, in the Agent Skills format used by Claude Code / Codex / other coding agents). A game developer drops it into their repo, and the coding agents that help build the game automatically plan, implement and verify a broad set of player-facing accessibility options from day one, instead of retrofitting them at the end. It must also support an audit/retrofit mode for games that are already built.

The skill must be **game-genre neutral and engine neutral**. It routes the agent to only the reference material relevant to the game being built (an FPS should never load JRPG guidance). Sample code will be provided for several engines (Godot/Redot GDScript, Unity C#, Unreal C++/Blueprint notes, web/JS; others as research suggests).

## Guiding principle (from the owner)
These are **options the player toggles on as desired**. They alter gameplay smoothly, are totally optional, and exist only for those who want them. Defaults stay the designer's intended experience, except where safety demands a safe default (for example photosensitivity, loud audio). No option may shame the player, lock content, lock achievements/trophies, or mark the save.

## Owner's seed list (not comprehensive; expand it a lot)
Colour-blind modes; subtitles/captions; no flashing lights (epilepsy/photosensitivity); arachnophobia and other phobia modes; dyslexia-friendly text; turning off guiding visuals (yellow paint, glowing trails) for ADHD / visually overstimulated players; HUD off / minimal HUD; extra visual trackers for memory issues; extra quest logs, recaps and optional spoilers/hints for memory issues; customisable difficulty with no penalty to Steam achievements or trophies; click reduction for mobility (hold instead of mash, auto-complete QTEs); invincibility, infinite ammo, infinite stamina for low-stress play; turning off enemy damage; keeping some fail states while turning off others; allowing third-party mods to extend features and hardware compatibility; TTS / screen reader / voice input compatibility.

## Already known locally (do not just repeat; extend)
Our prior jam games already ship: Master/Music/SFX/Ambience volume reachable before music plays, safe first-launch volume, VSync default on, FPS cap default 60, UI/text scale 100/125/150%, screen shake Full/Reduced/Off (default Reduced), reduced motion, no camera rotation, no rapid full-screen flashes, Easy/Normal/Hard with plain-language effects, a visible Assists menu (invincibility, bottomless resources, slower hazards, infinite jumps, skip room after 3 restarts) with an "ASSIST ON" badge, one-click reset, assists never affect ending/grade, non-colour state encoding, visual equivalents for critical audio, no-softlock recovery, auto-pause on focus loss, unambiguous input prompts, remap-aware prompts if remapping ships.

## Rules for you
- Do real web research (search and fetch pages). Cite a source URL for every non-obvious claim and every named game example. Prefer primary sources: official guidelines, developer/publisher accessibility pages, GDC talks, patch notes, GitHub repos, accessibility reviewers (Can I Play That?, Steve Saylor, Ian Hamilton, AbleGamers, DAGERSystem, Game Accessibility Nexus), platform docs, peer-reviewed or standards bodies.
- Flag anything you could not verify as UNVERIFIED instead of guessing. Do not invent games, features, repos, APIs or quotes.
- Write ONLY your one output file (path given in your assignment). Do not edit any other file, do not run git, do not install anything.
- Write in plain English. Use markdown tables and lists. Be exhaustive within your scope; length is fine (aim for depth, roughly 600-2000 lines is acceptable).
- For each accessibility option you describe, where relevant capture: name; what it does; who it helps (vision, hearing, motor, cognitive, neurodivergent/sensory, photosensitivity, phobia, motion sickness, mental health/stress, situational); game context signals that make it relevant (mechanics, genre, presentation features); recommended default; implementation notes (engine-neutral); pitfalls; shipped examples with URLs; priority tier (Baseline = every game; Contextual = when a signal is present; Advanced = larger budgets).
- End your file with a short section "Implications for the skill": concrete rules, checklists or routing logic the skill should encode.
