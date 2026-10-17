// Game Accessibility sample: gameplay modifiers in one place.
// Pure C# (netstandard2.1). No UnityEngine references.
//
// Contract: references/architecture.md section 2. Gameplay asks the policy at the moment of decision and
// never reads raw settings. Missing keys (a game that did not register them) fall back to "no change".
// Achievement and save code must never read this class: they record what happened, not which options were on.

using System;
using System.Collections.Generic;

namespace GameAccessibility
{
    public sealed class AssistPolicy
    {
        private readonly A11ySettingsStore _store;

        /// <summary>Raised by <see cref="RequestSkip"/>. The handler must complete the challenge through its normal success path.</summary>
        public event Action<string> SkipRequested;

        /// <summary>
        /// Damage sources NOT scaled by assist.damage_taken. Falls follow assist.no_fall_death instead.
        /// Add game-specific sources (for example "scripted") here.
        /// </summary>
        public HashSet<string> SourcesExemptFromDamageScale { get; } =
            new HashSet<string>(StringComparer.Ordinal) { "fall" };

        /// <summary>Resource name to the bool setting that makes it infinite. Currencies and quest items never belong here.</summary>
        public Dictionary<string, string> InfiniteResourceKeys { get; } =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ammo"] = "assist.infinite_ammo",
                ["stamina"] = "assist.infinite_stamina",
            };

        public AssistPolicy(A11ySettingsStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>
        /// Damage to apply to the player. <paramref name="source"/>: "enemy", "hazard", "fall", "self"...
        /// Healing (negative or zero amounts) passes through unchanged. A result of 0 must not soft-lock:
        /// grabs, drowning and scripted defeats need their own recovery path.
        /// </summary>
        public float DamageToPlayer(float amount, string source)
        {
            if (amount <= 0f) return amount;
            if (source != null && SourcesExemptFromDamageScale.Contains(source)) return amount;
            float scale = _store.GetOr("assist.damage_taken", 1f);
            return amount * Math.Max(0f, scale);
        }

        /// <summary>
        /// Whether a failure state should happen. "fall": false when safe falls are on (return the player to the
        /// last safe position instead). "timer": false when time limits are off. Other reasons: true.
        /// </summary>
        public bool ShouldFail(string reason)
        {
            switch (reason)
            {
                case "fall": return !_store.GetOr("assist.no_fall_death", false);
                case "timer": return !string.Equals(_store.GetOr("assist.timers", "normal"), "off", StringComparison.Ordinal);
                default: return true;
            }
        }

        /// <summary>True when spending <paramref name="resource"/> is intercepted by an infinite-resource assist.</summary>
        public bool IsInfinite(string resource) =>
            resource != null && InfiniteResourceKeys.TryGetValue(resource, out string key) && _store.GetOr(key, false);

        /// <summary>
        /// Attempts to spend <paramref name="amount"/> from <paramref name="pool"/>. Returns true when the action may
        /// proceed. With an infinite assist it returns true and leaves the pool untouched (never adds infinity to a
        /// counter). Otherwise it spends and returns true, or returns false when the pool is too small.
        /// </summary>
        public bool Consume(string resource, float amount, ref float pool)
        {
            if (IsInfinite(resource)) return true;
            if (pool < amount) return false;
            pool -= amount;
            return true;
        }

        /// <inheritdoc cref="Consume(string, float, ref float)"/>
        public bool Consume(string resource, int amount, ref int pool)
        {
            if (IsInfinite(resource)) return true;
            if (pool < amount) return false;
            pool -= amount;
            return true;
        }

        /// <summary>
        /// Multiplier for countdowns that are not races: 1 (normal), 2 (extended) or +infinity (off).
        /// Use <c>remaining -= dt / TimerScale</c>; dividing by infinity stops the countdown without special cases.
        /// </summary>
        public float TimerScale
        {
            get
            {
                switch (_store.GetOr("assist.timers", "normal"))
                {
                    case "extended": return 2f;
                    case "off": return float.PositiveInfinity;
                    default: return 1f;
                }
            }
        }

        /// <summary>
        /// Game speed for simulation clocks only. Do not apply it to menus, subtitles tied to audio, narration or
        /// network clocks. Timing windows are measured in simulation time and scaled once by
        /// input.timing_window_scale (<see cref="InputAssistCore.TimingWindow"/>); never multiply them by this as well.
        /// </summary>
        public float TimeScale => Clamp(_store.GetOr("assist.game_speed", 1f), 0.1f, 1f);

        /// <summary>Multiplier for attack frequency and detection speed. 0 means enemies never attack.</summary>
        public float EnemyAggression => Clamp(_store.GetOr("assist.enemy_aggression", 1f), 0f, 1f);

        /// <summary>Multiplier for random-encounter chance. 0 turns random battles off; story battles still happen.</summary>
        public float EncounterRate => Clamp(_store.GetOr("assist.encounter_rate", 1f), 0f, 1f);

        /// <summary>Party members choose actions themselves; the player can take control at any time.</summary>
        public bool AutoBattle => _store.GetOr("assist.auto_battle", false);

        /// <summary>Battle animation speed (1..4). Applies to animations only, never to decision time.</summary>
        public float BattleSpeed => Clamp(_store.GetOr("assist.battle_speed", 1f), 1f, 4f);

        /// <summary>Whether the pause menu should offer a skip for the current challenge.</summary>
        public bool SkipAvailable => _store.GetOr("assist.skip_available", false);

        /// <summary>Whether scenes listed in the content notices offer a skip with a short text summary.</summary>
        public bool SceneSkipAvailable => _store.GetOr("content.scene_skip", false);

        /// <summary>
        /// Requests a skip. Returns false when skips are not offered. Handlers must grant the same items, flags,
        /// checkpoints and achievement events as winning, and the transition must be idempotent.
        /// </summary>
        public bool RequestSkip(string challengeId)
        {
            if (!SkipAvailable || string.IsNullOrEmpty(challengeId)) return false;
            SkipRequested?.Invoke(challengeId);
            return true;
        }

        private static float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v);
    }
}
