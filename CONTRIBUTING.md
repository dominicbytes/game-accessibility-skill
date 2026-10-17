# Contributing

Thanks for improving the skill. Players depend on the advice being correct, so changes follow a
few strict rules.

## Ground rules

1. **Evidence or label.** Every non-obvious claim, threshold and shipped-game example links a
   primary source (guideline, developer page, patch notes, reviewer, repository). If you cannot
   verify something, write `UNVERIFIED` rather than guessing. Never invent games, features, APIs,
   repos, numbers or quotes.
2. **Policy first.** Nothing may contradict
   [`skills/game-accessibility/references/core-policy.md`](skills/game-accessibility/references/core-policy.md):
   - options are optional, reversible, available before the barrier and unpenalised;
   - no achievement locks, save marks or shaming;
   - safe defaults only for safety.
3. **Stable IDs.** Option IDs (`V01`, `H17`, `M14`, ...) come from
   [`option-catalog.md`](skills/game-accessibility/references/option-catalog.md). Signal IDs
   (`S001`-`S157`) come from [`signal-routing.md`](skills/game-accessibility/references/signal-routing.md).
   Setting keys (`text.scale`, `input.qte_mode`, ...) come from
   [`settings-registry.json`](skills/game-accessibility/assets/settings-registry.json). Add new IDs at
   the end. Never renumber.
4. **Progressive disclosure.** `SKILL.md` stays short (under 250 lines). Detail lives in the one
   topic guide that its signal routes to. Do not duplicate guidance across guides; link instead.
   References are linked directly from `SKILL.md`, one level deep.
5. **Engine code is verified or labelled.** Samples state the engine versions they were run on. Code
   that could not be compiled or run here says so at the top of its guide.
6. **Plain English.** Short sentences, active voice, no hype. Describe effects, not diagnoses.

## Topic guide template

Each file in `skills/game-accessibility/references/topics/` follows this shape. Target length is
120-260 lines.

```markdown
# <Topic>

**Load when:** <signals, e.g. S013 timed inputs, S014 QTEs>. **Skip when:** <confirmed absence>.

## Options
| ID | Option | Setting key | Default | Tier | Helps |
|----|--------|-------------|---------|------|-------|
(Tier: B = Baseline, C = Contextual, A = Advanced. Helps: V vision, H hearing, M motor,
 C cognitive/memory, N neurodivergent/sensory, P photosensitivity, F phobia, O motion sickness,
 T stress/mental health, S situational.)

## How to implement
Engine-neutral steps that use the architecture services (A11ySettings, AssistPolicy, InputAssist,
CaptionBus, Narrator, GuidanceLayer, ContentFilter, FlashGuard, CameraComfort, ColorTokens).
One subsection per option group, with the pitfalls.

## Numbers that matter
(Thresholds with their source and scope. Omit the section if none apply.)

## Shipped examples
- Game: what it does. [source](url). Note limits and platform scope.

## Verify
- [ ] Concrete, observable checks, including combinations and persistence.

## Common mistakes
- Mistake → correction.
```

## Tests

- `python tests/check_skill.py` validates the frontmatter, links, IDs, registry keys and SKILL.md
  length.
- `tests/run_godot_tests.sh` runs the Godot/Redot sample tests headless. Set `REDOT_BIN` or
  `GODOT_BIN` first.
- Scenario evals live in `evals/`. Run the prompts with and without the skill installed and
  compare the results with the rubric.
