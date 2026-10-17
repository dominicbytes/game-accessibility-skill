# Accessibility contract: <GAME NAME>

> Maintained by the agents and developers working on this game, using the `game-accessibility`
> skill. Update it **in the same change** as any new mechanic, content, screen, audio or camera
> feature. It is the source of truth for which options exist, why, and how they were verified.

- **Engine / version:** <e.g. Redot 26.3-stable, verified with --version on YYYY-MM-DD>
- **Platforms / inputs:** <Windows, Web; keyboard+mouse, gamepad; touch? VR?>
- **Minimum supported resolution:** <e.g. 1280x720>
- **Languages:** <en>
- **Last full audit:** <date, agent/person>

## Policy (binding; record conflicts below for the owner to resolve)

- All options are optional, reversible at any time, and available before the barrier they address.
- No achievement, trophy, unlock, ending, grade or save is changed or marked by any option.
- Every option is set independently. An "all" control (reduce motion, outlines) is an editable
  patch; each part it covers keeps its own setting.
- Safe defaults: no hazardous flashing in any mode; quiet first-launch audio; settings before logos,
  music and speech; reduced camera shake; OS accessibility preferences respected.
- Wording describes effects, never shames. Claims match verified behaviour.

## Signals

Status: `present`, `planned` (in scope but not built yet: plan its options now and ship them with the
feature), `suspected`, `absent`, `unknown`. Evidence is a file, scene, design-doc
section or the developer's answer.

| Signal | Status | Scope (where in game) | Evidence | Options triggered |
| --- | --- | --- | --- | --- |
| S001 visual output | present | whole game | always | P01, V01, V25 |
| S066 spoken dialogue | <...> | <...> | <...> | H17, H20, H24, H28 |
| <add rows> | | | | |

## Options

Status: `planned`, `implemented`, `verified`, `blocked`, `not applicable` (with reason).

| Catalog ID | Setting key | Default | Owner (script / system) | Status | Verification |
| --- | --- | --- | --- | --- | --- |
| V01 | text.scale | 1.0 | ui/theme.gd | planned | text at 200% on 1280x720, no clipping |
| <add rows> | | | | | |

## Advanced needs and scope decisions

| Need | Decision (now / later / out of scope) | Reason | Owner sign-off |
| --- | --- | --- | --- |
| Full nonvisual play | <...> | <...> | <...> |

## Conflicts with policy (must be resolved by the owner)

<e.g. "Achievement 'Untouchable' requires Hard difficulty", with proposed redesign>

## Verification log

| Date | Build | Check | Result (pass/fail/blocked/not tested) | Evidence |
| --- | --- | --- | --- | --- |

## Known gaps (also listed in public accessibility notes)

- <gap, impact, plan>
