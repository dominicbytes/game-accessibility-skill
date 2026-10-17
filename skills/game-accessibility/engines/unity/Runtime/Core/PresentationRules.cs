// Game Accessibility sample: per-class outlines, per-effect motion overrides and scare options.
// Pure C# (netstandard2.1). No UnityEngine references.
//
// Umbrella settings (visual.high_contrast, motion.reduce_motion) are editable patches over per-class/per-effect
// keys. Each override key is "follow" (use the umbrella), "off" or "on" (A11ySettingsStore.ResolveToggle).
// Keys a game did not register behave as "follow"; a missing umbrella means "no change" from the authored game.

using System;

namespace GameAccessibility
{
    /// <summary>Outline class an object is tagged with when it is authored.</summary>
    public enum OutlineClass { Enemies, Allies, Interactables, Hazards }

    /// <summary>Decorative or automatic motion an effect is tagged with when it is authored.</summary>
    public enum MotionEffect { AutoCamera, FovEffects, SpeedLines, Parallax, MenuAnimation }

    public static class PresentationRules
    {
        public const string HighContrastKey = "visual.high_contrast";
        public const string ReduceMotionKey = "motion.reduce_motion";

        public static string OutlineKey(OutlineClass c)
        {
            switch (c)
            {
                case OutlineClass.Enemies: return "visual.outline_enemies";
                case OutlineClass.Allies: return "visual.outline_allies";
                case OutlineClass.Interactables: return "visual.outline_interactables";
                default: return "visual.outline_hazards";
            }
        }

        public static string MotionKey(MotionEffect e)
        {
            switch (e)
            {
                case MotionEffect.AutoCamera: return "motion.auto_camera";
                case MotionEffect.FovEffects: return "motion.fov_effects";
                case MotionEffect.SpeedLines: return "motion.speed_lines";
                case MotionEffect.Parallax: return "motion.parallax";
                default: return "motion.menu_animation";
            }
        }

        /// <summary>Whether objects of class <paramref name="c"/> get the high-contrast outline. "follow" uses visual.high_contrast.</summary>
        public static bool OutlineEnabled(A11ySettingsStore store, OutlineClass c)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            return store.ResolveToggle(OutlineKey(c), store.GetOr(HighContrastKey, false));
        }

        /// <summary>
        /// Whether the effect may play. "follow" uses !motion.reduce_motion. "off" stops it (static menus, instant
        /// transitions, camera stays where the player put it); "on" keeps it even with Reduce motion on.
        /// </summary>
        public static bool MotionEffectEnabled(A11ySettingsStore store, MotionEffect e)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            return store.ResolveToggle(MotionKey(e), !store.GetOr(ReduceMotionKey, false));
        }

        /// <summary>content.jump_scares == "reduced": soften sudden loud stings and camera snaps.</summary>
        public static bool JumpScaresReduced(A11ySettingsStore store) =>
            string.Equals(store?.GetOr("content.jump_scares", "full"), "reduced", StringComparison.Ordinal);

        /// <summary>
        /// content.scare_warnings: show and announce a short, consistent heads-up before scripted scares. Independent
        /// of <see cref="JumpScaresReduced"/>: a player can keep full scares and still be warned.
        /// </summary>
        public static bool ScareWarnings(A11ySettingsStore store) => store != null && store.GetOr("content.scare_warnings", false);
    }
}
