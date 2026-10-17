// Game Accessibility sample: every full-screen flash, strobe, lightning, hit flash and muzzle flash asks here first.
// Pure C# (netstandard2.1). No UnityEngine references.
//
// Contract: references/architecture.md section 8.
// - A request COUNTS when area >= 0.25 OR intensity >= 0.1 OR it is saturated red (red always counts).
// - In EVERY mode, counted flashes: at most 3 in any 1-second window AND at most 9 in any 5-second window
//   (the second limit blocks a sustained 2-3 Hz strobe). Extra requests are suppressed. Both budgets, both
//   window lengths and both thresholds are hard-clamped: configuration can make the guard stricter, never
//   looser. Windows shorter than 1 s / 5 s are ignored (1 s / 5 s are used); longer windows are honoured.
// - With motion.reduce_flashing on: flashes become steady, low-contrast fades with capped intensity.
// - Saturated red flashes are desaturated in every mode.
// This limits what the game requests. It cannot measure the rendered frame: analyse captured footage with a
// photosensitivity tool (for example EA IRIS) before claiming anything about flashing.

using System;
using System.Collections.Generic;

namespace GameAccessibility
{
    public enum FlashOutput
    {
        /// <summary>Play the flash as authored, using <see cref="FlashDecision.Intensity"/>.</summary>
        Flash,
        /// <summary>Replace the flash with a steady, slow fade (or an icon pulse) at <see cref="FlashDecision.Intensity"/>.</summary>
        SteadyFade,
        /// <summary>Do not show it. Convey any information it carried another way (sound, icon, text).</summary>
        Suppressed,
    }

    public readonly struct FlashDecision
    {
        public readonly FlashOutput Output;
        /// <summary>0..1 peak intensity to use.</summary>
        public readonly float Intensity;
        /// <summary>True when the effect must be shown without saturated red.</summary>
        public readonly bool Desaturate;
        /// <summary>Minimum seconds for a fade in and out (0 for authored flashes).</summary>
        public readonly float MinFadeSeconds;

        public FlashDecision(FlashOutput output, float intensity, bool desaturate, float minFadeSeconds)
        {
            Output = output;
            Intensity = intensity;
            Desaturate = desaturate;
            MinFadeSeconds = minFadeSeconds;
        }
    }

    public sealed class FlashGuardCore
    {
        private readonly A11ySettingsStore _store;
        private readonly Queue<double> _recent = new Queue<double>();

        /// <summary>Counted flashes allowed in any 1-second window. Values above 3 are ignored.</summary>
        public int MaxFlashesPerWindow { get; set; } = 3;

        /// <summary>
        /// Length of the short window. Values below 1 s (or NaN) are ignored and 1 s is used: a shorter window would
        /// let more than 3 counted flashes into one second. A longer window is stricter and is honoured.
        /// </summary>
        public double WindowSeconds { get; set; } = 1.0;

        /// <summary>Counted flashes allowed in any 5-second window. Values above 9 are ignored.</summary>
        public int MaxFlashesPerLongWindow { get; set; } = 9;

        /// <summary>
        /// Length of the long window. Values below 5 s (or NaN, or below the effective short window) are ignored:
        /// a shorter window would let more than 9 counted flashes into five seconds. A longer window is honoured.
        /// </summary>
        public double LongWindowSeconds { get; set; } = 5.0;

        /// <summary>
        /// Screen fraction (0..1) at or above which a flash counts as "large". Values above 0.25 are ignored
        /// (0.25 is used): lower it if footage analysis shows smaller flashes are hazardous, never raise it.
        /// Area alone is not an exclusion: a small flash still counts when it is bright or saturated red.
        /// </summary>
        public float LargeAreaThreshold { get; set; } = 0.25f;

        /// <summary>
        /// Relative-luminance change (0..1) at or above which a flash counts as "bright". 0.1 follows the 10% change
        /// in the WCAG 2.x "general flash" definition. Values above 0.1 are ignored (0.1 is used).
        /// </summary>
        public float BrightIntensityThreshold { get; set; } = 0.1f;

        /// <summary>Hard limits that configuration cannot loosen.</summary>
        public const int MaxShortBudget = 3, MaxLongBudget = 9;
        public const float MaxAreaThreshold = 0.25f, MaxIntensityThreshold = 0.1f;
        public const double MinShortWindowSeconds = 1.0, MinLongWindowSeconds = 5.0;

        /// <summary>Short window actually enforced: at least 1 s.</summary>
        public double EffectiveWindowSeconds =>
            double.IsNaN(WindowSeconds) ? MinShortWindowSeconds : Math.Max(MinShortWindowSeconds, WindowSeconds);

        /// <summary>Long window actually enforced: at least 5 s and never shorter than the short window.</summary>
        public double EffectiveLongWindowSeconds =>
            Math.Max(EffectiveWindowSeconds,
                double.IsNaN(LongWindowSeconds) ? MinLongWindowSeconds : Math.Max(MinLongWindowSeconds, LongWindowSeconds));

        /// <summary>Peak intensity cap used when motion.reduce_flashing is on. Keep it below the bright threshold.</summary>
        public float ReducedIntensityCap { get; set; } = 0.08f;

        /// <summary>Minimum fade length when reduce_flashing converts a flash.</summary>
        public float ReducedFadeSeconds { get; set; } = 0.5f;

        public FlashGuardCore(A11ySettingsStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public bool ReduceFlashing => _store.GetOr("motion.reduce_flashing", false);

        /// <summary>
        /// Asks permission for one flash.
        /// </summary>
        /// <param name="nowSeconds">Monotonic real time (unscaled; flashes are seen in real time even in slow motion).</param>
        /// <param name="intensity">0..1 peak luminance change the effect would cause.</param>
        /// <param name="screenArea">0..1 fraction of the screen the effect covers.</param>
        /// <param name="saturatedRed">True when the effect is a saturated red. Saturated red always counts against both
        /// budgets, whatever its area or intensity.</param>
        public FlashDecision RequestFlash(double nowSeconds, float intensity, float screenArea, bool saturatedRed = false)
        {
            // Unknown or invalid measurements are treated as worst case.
            intensity = float.IsNaN(intensity) ? 1f : Math.Max(0f, Math.Min(1f, intensity));
            screenArea = float.IsNaN(screenArea) ? 1f : Math.Max(0f, Math.Min(1f, screenArea));
            bool reduce = ReduceFlashing;
            bool counts = Counts(intensity, screenArea, saturatedRed);

            // _recent holds allowed counted flashes from the last effective long window, oldest first.
            double shortWindow = EffectiveWindowSeconds;
            double longWindow = EffectiveLongWindowSeconds;
            while (_recent.Count > 0 && nowSeconds - _recent.Peek() >= longWindow) _recent.Dequeue();

            // Both budgets apply in every mode, before any conversion.
            if (counts)
            {
                int inShort = 0;
                foreach (double t in _recent)
                {
                    if (nowSeconds - t < shortWindow) inShort++;
                }
                bool overShort = inShort >= Math.Min(MaxShortBudget, Math.Max(0, MaxFlashesPerWindow));
                bool overLong = _recent.Count >= Math.Min(MaxLongBudget, Math.Max(0, MaxFlashesPerLongWindow));
                if (overShort || overLong)
                    return new FlashDecision(FlashOutput.Suppressed, 0f, saturatedRed, 0f);
                _recent.Enqueue(nowSeconds);
            }

            if (reduce)
            {
                // Steady, low-contrast replacement, capped below the bright threshold.
                float capped = Math.Min(intensity, ReducedIntensityCap);
                return new FlashDecision(FlashOutput.SteadyFade, capped, saturatedRed, ReducedFadeSeconds);
            }

            return new FlashDecision(FlashOutput.Flash, intensity, saturatedRed, 0f);
        }

        /// <summary>
        /// Whether a request counts against the budgets: area &gt;= 0.25 OR intensity &gt;= 0.1 OR saturated red.
        /// Configured thresholds can only lower these limits.
        /// </summary>
        public bool Counts(float intensity, float screenArea, bool saturatedRed)
        {
            if (saturatedRed) return true;
            float area = Math.Min(MaxAreaThreshold, Math.Max(0f, LargeAreaThreshold));
            float bright = Math.Min(MaxIntensityThreshold, Math.Max(0f, BrightIntensityThreshold));
            return screenArea >= area || intensity >= bright;
        }

        /// <summary>Forget history (for example on scene load). Never needed for safety.</summary>
        public void Clear() => _recent.Clear();
    }
}
