// Game Accessibility sample: per-channel guidance levels with the "follow" rule.
// Pure C# (netstandard2.1). No UnityEngine references.
//
// Contract: references/architecture.md section 6. guidance.level is the overall level. guidance.paint,
// guidance.trail and guidance.highlights default to "follow" (use the overall level); any other value
// overrides it for that channel only. Objective markers use guidance.objective_marker.
// Hazard warnings are not guidance and never go through these rules.

using System;

namespace GameAccessibility
{
    /// <summary>Guidance channel a visual is tagged with when it is authored.</summary>
    public enum GuidanceKind { Paint, Trail, InteractHighlight, ObjectiveMarker }

    public static class GuidanceRules
    {
        public const string Follow = "follow";
        public const string OverallKey = "guidance.level";

        /// <summary>The channel's own setting key.</summary>
        public static string ChannelKey(GuidanceKind kind)
        {
            switch (kind)
            {
                case GuidanceKind.Paint: return "guidance.paint";
                case GuidanceKind.Trail: return "guidance.trail";
                case GuidanceKind.InteractHighlight: return "guidance.highlights";
                default: return "guidance.objective_marker";
            }
        }

        /// <summary>
        /// Effective level for a channel: off, on_request, subtle, standard or boosted. Its own key wins unless it
        /// is "follow" or not registered, in which case guidance.level applies.
        /// </summary>
        public static string EffectiveLevel(A11ySettingsStore store, GuidanceKind kind)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (kind == GuidanceKind.ObjectiveMarker) return store.GetOr("guidance.objective_marker", "standard");
            string own = store.GetOr(ChannelKey(kind), Follow);
            return string.Equals(own, Follow, StringComparison.Ordinal) ? store.GetOr(OverallKey, "standard") : own;
        }

        /// <summary>
        /// Intensity for a level: 0 hidden, ~0.4 subtle, 1 standard, &gt;1 boosted (plus outline).
        /// <paramref name="hintActive"/> shows on_request guidance while the hint is up.
        /// </summary>
        public static float Intensity(string level, bool hintActive, out bool boosted,
            float subtle = 0.4f, float boostedIntensity = 1.5f)
        {
            boosted = false;
            switch (level)
            {
                case "off": return 0f;
                case "on_request": return hintActive ? 1f : 0f;
                case "subtle": return subtle;
                case "boosted": boosted = true; return boostedIntensity;
                default: return 1f; // standard
            }
        }
    }
}
