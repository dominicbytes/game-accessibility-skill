# Independent review — SOL

Reviewed 2026-10-17; snapshot checked at 17:33 UTC. Paths below are relative to
`skills/game-accessibility/` unless explicitly prefixed with `repo:`.
No git commands, source edits, engine launches, or game changes were performed.
Read the requested skill/reference/topic/template/script files, skimmed the catalogue,
and cross-checked claims against research 01–06 and selected primary sources.

## Critical

None confirmed. The safety issue below is Major because the detailed analysis workflow
can catch it, but the short implementation contract and its tests do not prevent it.

## Major

### M01 — Documented batch contrast checks silently check zero pairs
- **Location:** `scripts/contrast_check.py:72–78,89`.
- **Evidence:** With an in-memory pairs file containing `#FFF #FFF should-fail` and
  `#FFF #000 should-pass`, `main(['--pairs', ...])` prints nothing and returns 0.
  Every documented hex-colour row is treated as a comment by `line.startswith('#')`.
- **Fix:** Recognise colour tokens before comments, or use an unambiguous comment prefix.
  Require at least one parsed pair; add a regression with both passing and failing `#` rows.

### M02 — Transparent backgrounds can receive a false contrast PASS
- **Location:** `scripts/contrast_check.py:34–36,46–52`.
- **Evidence:** `#000` on `#FFFFFF00` returns 21:1/PASS without `--over`; over opaque
  black the same pair is 1:1/FAIL. `--over '#FFFFFF00'` also returns PASS despite an
  unresolved transparent underlay. `over()` silently assumes an opaque destination.
- **Fix:** Require an opaque scene colour whenever background alpha is below 1; reject
  translucent `--over`, or retain alpha until a final opaque surface is supplied.

### M03 — FlashGuard's advertised budget allows a documented extended failure
- **Location:** `references/architecture.md:168–172`; `references/topics/photosensitivity.md:39–47,151–152,175–177`;
  `references/verification.md:48–50,78`; `repo:README.md:37`.
- **Evidence:** Two large flashes per second for six seconds satisfy the guard and its
  stated acceptance test, but meet the guide's own IRIS extended-failure rule.
  Research 01 §2.3 and research 06 §B7 explicitly warn against treating 3/s as safety.
  [EA's IRIS rules](https://github.com/electronicarts/IRIS) confirm the duration issue.
- **Fix:** State that the 1-second cap is only one prevention layer. Specify prolonged-loop
  suppression, test a sustained 2–3 Hz sequence, and require rendered analysis for flash
  verification; replace “Inspect or analyse” with analysis or an explicit blocked result.

### M04 — Canonical keys force bundles that the core policy forbids
- **Location:** `assets/settings-registry.json:38,43,46,48,50,57–59,65,74`;
  `references/topics/color-and-clarity.md:28–30`; `references/architecture.md:142–150`.
- **Evidence:** One `guidance.level` controls paint, trails and interaction highlights,
  although memory guidance promises an independent trail. `visual.high_contrast` combines
  outlines and background dimming; `content.blood` combines blood and dismemberment;
  `content.warnings` also controls the skip offer. Camera effects and aim/lock likewise
  have shared scalar selectors. Optional project overrides are not an independence contract.
- **Fix:** Give distinct barriers their own persistent keys, especially guidance channels,
  outline classes/dimming, blood/gore, and warnings/skip access. Make umbrella controls
  editable preset patches. Require the cross-combinations in core-policy §§1 and 5.

### M05 — Feature-hook and audit entry points can bypass the binding policy
- **Location:** `SKILL.md:30–32,36–39`; `references/retrofit-audit.md:6–20`.
- **Evidence:** Feature hook starts at step 3, skipping both the required core-policy read
  and opening the contract. Audit starts in a separate document that never explicitly
  requires those shared prerequisites. This is especially risky for baseline RED's
  achievement-policy failure and for a fresh agent revising one existing option.
- **Fix:** Before the mode table say: “Every invocation first reads core-policy and opens
  or creates ACCESSIBILITY.md. Then use the selected mode; Feature hook assesses the delta.”
  Make audit explicitly return to selection, implementation and verification.

### M06 — Planned features can be classified away during proactive building
- **Location:** `references/signal-routing.md:17,21,359–369`; `SKILL.md:14–22,40–47`.
- **Evidence:** Developer-described demands count as present, but “only planned” also
  counts as removed. Only confirmed/present signals select implementation. A planned M2
  QTE or spider can therefore escape its access plan, reproducing baseline A2/A4.
- **Fix:** Separate current-build audit state from planned/in-scope design state. Explicitly
  route planned features in Build mode and require their option/hooks in the same milestone;
  exclude them only from claims about the current executable.

### M07 — JRPG encounter/auto-battle requirements remain examples, not required work
- **Location:** `references/genre-recipes.md:G05 (48–52)`;
  `references/option-catalog.md:D13,D23`; `references/topics/structured-ui-and-strategy.md:15–41`.
- **Evidence:** Baseline B4 specifically missed encounter-rate, auto-battle and battle-skip
  options. G05 mentions Pixel Remaster's features, but mandates only D13 resource/crafting
  costs. No explicit encounter-rate or turn-based auto-battle option/signal maps into the
  selection algorithm. The generic encounter skip routes through other blocker signals.
- **Fix:** Add stable option IDs and explicit encounter/turn-command signals for encounter
  frequency/off, optional auto-battle, and progress-preserving battle bypass. Route only games
  with those mechanics. Verify rewards, story flags and achievements through each path.

### M08 — Mandatory engine-guide links are broken
- **Location:** `SKILL.md:50–51,94–101`; `repo:README.md:97,104–107`.
- **Evidence:** Initial static check found four missing targets. The final check still exits 1:
  `engines/{godot,unreal,other}/GUIDE.md` are absent; Unity's guide appeared during review.
  Sample source files exist, but affected agents cannot follow “load exactly one” guide.
- **Fix:** Supply the remaining guides, including version gates, hookup steps, sample limitations
  and actual validation evidence, or correct the links to existing guides. Rerun the check.
  Treat this as a snapshot finding if another worker is still creating those files.

### M09 — Custom HUD and custom palette selectors lack their persistent choices
- **Location:** `assets/settings-registry.json:37,52`;
  `references/topics/sensory-load-and-guidance.md:17–18,63–67`;
  `references/topics/color-and-clarity.md:54–55`; catalogue N01/N02/V24.
- **Evidence:** `hud.mode=custom` stores no element visibility values; `color_profile=custom`
  stores no role colours. There is no explicit HUD Off value despite N01 “Minimal / no HUD”
  and the owner's HUD-off requirement. Merely copying these registry entries cannot persist
  a custom selection, much less restore it across a revised build.
- **Fix:** Define per-element booleans/per-role colour keys or supported structured values,
  their defaults and migration rules. Define HUD Off with captions independently controlled
  and status available on demand; test custom choices after restart and old-save load.

### M10 — Time-scaling instructions disagree about independent timing options
- **Location:** `references/architecture.md:76–77,101–102`;
  `references/topics/timing-and-speed.md:55–56,69–76,182`;
  `references/topics/traversal-and-platforming.md:35–37`; `assets/settings-registry.json:7`.
- **Evidence:** Windows are defined as base × timing scale, but combined slowing/window
  widening is called “double extension” and disallowed without a replacement formula.
  The “More time” preset deliberately enables both. A simulation-time window already lasts
  longer in wall time; measuring it in simulation time does not cancel that effect.
- **Fix:** Define both clocks and the formula. For example, apply the explicit window
  multiplier once in simulation time, making wall duration base × windowScale / timeScale.
  Allow the player's deliberate combination; prohibit only accidentally applying either
  multiplier twice. Add numeric combination expectations rather than “no double extension.”

### M11 — Router tiers/IDs can silently downgrade implemented capabilities
- **Location:** `references/signal-routing.md:S117 (208),S094 (172),S136–S140 (237–241),S090 (168)`;
  `references/topics/content-and-phobias.md:18`; `references/topics/vr-and-xr.md:14–18`;
  `references/topics/structured-ui-and-strategy.md:33–35`.
- **Evidence:** S117 makes texture variants Advanced, while F15 is Contextual in its guide
  and catalogue. S094 marks R02 Advanced although the narration/strategy guides make it
  Contextual. XR still routes mainly to M26/M05 instead of catalogue M43–M47; the VR guide
  labels virtual crouch M26 although M45 owns it. S090 omits Baseline C38 persistent tooltips.
- **Fix:** Reconcile the canonical ID/tier owner, expand the relevant router rows, and test
  semantic agreement between router, catalogue and topic tables, beyond “ID exists.”

## Minor

### m01 — First-launch silence conflicts with blind-player activation
- **Location:** `references/architecture.md:190–195`; `references/core-policy.md:58`;
  `references/topics/audio-and-captions.md:74–77`; `references/topics/screen-reader-and-voice.md:93–99,185–186`.
- **Problem:** “No audio”/no speech before settings conflicts with a quiet spoken activation
  prompt when no OS reader is detected. Implementers can suppress the only audible entry.
- **Fix:** Explicitly permit a quiet, repeatable narration-entry prompt after saved volume
  applies; distinguish it from logo/music/content audio. Specify volume and repeat controls.

### m02 — Installed-skill script commands use the wrong working-directory assumption
- **Location:** `SKILL.md:41`; `references/retrofit-audit.md:10`;
  `references/verification.md:42`; `repo:README.md:58–65,95–96`.
- **Problem:** After installation in `.agents/skills/game-accessibility`, `python scripts/...`
  run at the game root addresses the game's scripts directory, not the installed skill.
- **Fix:** Say to resolve each script relative to the loaded SKILL.md and invoke its absolute,
  quoted path while passing the absolute game path. Do not imply changing to the game root
  makes the documented command work.

### m03 — Genre exclusion is weakened by loading whole mixed-purpose files
- **Location:** `SKILL.md:21,45`; `references/genre-recipes.md:G01,G05`;
  `references/topics/structured-ui-and-strategy.md:3–13`.
- **Problem:** Reading the single genre-recipes file loads G05 JRPG content for an FPS;
  a shooter inventory also selects a guide containing unrelated RTS/cards/idle sections.
  Signals correctly preserve timed minigames, but the literal “FPS never loads JRPG”
  promise is stronger than the file boundaries/instructions enforce.
- **Fix:** Route to named recipe/feature sections with instructions to read only those ranges,
  or split recipes. Keep shared inventory guidance separate from unrelated genre subsections.

### m04 — A single-player microphone mechanic incorrectly selects free-form chat
- **Location:** `references/signal-routing.md:S009 (62)`; catalogue K01/M23.
- **Problem:** Singing, blowing or voice spells select K01 free-form multiplayer text chat
  and M23 analog alternatives; neither correctly specifies a local non-speech command path.
- **Fix:** Route to a local button/menu/text alternative through logical actions. Select K01
  only for a communication signal; retain optional voice-command and nonverbal scope decisions.

### m05 — Haptic-only verification contradicts the audio guide
- **Location:** `references/verification.md:46–47` versus
  `references/topics/audio-and-captions.md:144–145,225–226`.
- **Problem:** The general muted-play check accepts a haptic-only equivalent, while the topic
  correctly requires a visual channel and makes haptics extra. An agent can falsely pass
  a deaf-player route on hardware without rumble.
- **Fix:** Require an actionable visual/text equivalent for essential sound; test with haptics
  disabled as well as audio muted. Keep haptics as an additional channel.

### m06 — Partial phobia-filter verification demands the wrong output
- **Location:** `references/verification.md:81`;
  `references/topics/content-and-phobias.md:83–87,197–199`.
- **Problem:** “At each level no original model, sound…” is impossible at original level 0,
  and incorrect at levels 1/2 that intentionally retain some appearance/sound attributes.
- **Fix:** Verify the exact promised substitutions at each level, compare original level 0
  as the control, and reserve “no original assets” for levels explicitly promising replacement.

### m07 — WCAG paragraph-spacing units are misattributed
- **Location:** `references/topics/text-and-readability.md:171`.
- **Problem:** The combined XAG/WCAG row says paragraph spacing is 2 × line spacing.
  Research 01:289,301 separates the sources; WCAG 1.4.12 uses 2 × font size.
  [W3C's actual criterion](https://www.w3.org/WAI/WCAG22/Understanding/text-spacing.html) confirms this.
- **Fix:** Split the XAG recommendation from the WCAG tolerance test; label the units exactly.
  Keep catalogue line 491's correct WCAG wording.

### m08 — Scanner excludes real gameplay solely because of filename substrings
- **Location:** `scripts/scan_signals.py:40–42,197–198`.
- **Evidence:** An in-memory `noticeboard.gd` containing `spawn_spider()` produces zero scanned
  files/no S109; identical `board.gd` produces S109. `third_party_enemy.gd` is also discarded.
- **Fix:** Match exact documentation basenames or restrict exclusions to prose extensions;
  never discard a gameplay script merely for containing `notice` or `third_party`.

### m09 — Scanner guide output is incomplete and common code creates false leads
- **Location:** `scripts/scan_signals.py:52–53,62–65,106–107,112–113,235–237`.
- **Evidence:** S014 emits only input-and-motor, omitting the router's timing guide; S035
  emits difficulty-and-assists while its router names combat-aiming-stealth. On the suggested
  game, S073 hits `UiTheme.INK` in ending_screen.gd:83, and S081 hits “question” in
  confirm_dialog.gd:3. These consume the displayed example budget without showing the demand.
- **Fix:** Emit guide arrays consistent with the router (or emit only IDs and require routing).
  Match `.ink` as a file extension, not a member name; bound quest/objective terms. Keep the
  existing candidate disclaimer and report unsupported signals separately from scanned no-hits.

### m10 — Policy template creates an unrestricted waiver
- **Location:** `assets/templates/ACCESSIBILITY.md:13` versus `references/core-policy.md:22–49`.
- **Problem:** “Do not weaken without the owner's written decision” implies arbitrary waiver
  of achievement/save/no-shame guarantees. Core policy permits conflict resolution and specific
  shared-rule decisions, not a general exception to its non-negotiable no-penalty rule.
- **Fix:** Use “Binding policy; record conflicts below for resolution.” Scope owner decisions
  to supported exceptions and redesigns instead of authorising policy removal.

## Nit

### n01 — Registry count in the installation overview is stale
- **Location:** `repo:README.md:93`.
- **Evidence / fix:** It says 60 keys; the registry and static check contain 63. Update the
  count or omit it so additions cannot stale this claim.

## Verification and coverage notes

- Final static checker: 316 catalogue IDs, 63 settings, 7 presets, 188 relative links; three
  failures are missing engine guides. Other work is occurring concurrently; counts are snapshots.
- Contrast: ordinary white/black gives 21:1/pass; white/white gives 1:1/fail. Batch and alpha
  failures above were reproduced with in-memory fixtures; no fixture files were written.
- Scanner: read-only scan of a local Godot jam game completed,
  scanning 91 text files. No runtime access claims about that game are made.
- Most requested coverage is explicit: colour, captions, flash suppression, spiders, font
  preferences, guidance reduction, trackers/logs/recaps/hint spoilers, independent failure
  channels, resources, click reduction, mods/adaptive hardware, TTS/voice and retrofit.
  M07/M09 identify the substantive baseline/owner gaps; M04 affects independence across many.
- Spot-checked API/version/licence claims agree with research 04; no invented game/API/library
  was established. Unreal's documented severity 0–1, the Unity 6.3/6.0 distinction, font licence
  caveats and dated/mod-specific examples are appropriately qualified. This was not a fresh
  exhaustive audit of every external URL, legal claim, engine sample or shipped-game build.
