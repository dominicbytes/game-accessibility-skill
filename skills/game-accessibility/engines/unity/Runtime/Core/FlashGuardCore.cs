// Game Accessibility sample: every full-screen flash, strobe, lightning, hit flash and muzzle flash asks here first.
// Pure C# (netstandard2.1). No UnityEngine references.
//
// Contract: references/architecture.md section 8.
// - In EVERY mode, for large or bright flashes: at most 3 in any 1-second window AND at most 9 in any
//   5-second window (the second limit blocks a sustained 2-3 Hz strobe). Extra requests are suppressed.
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

        /// <summary>Large/bright flashes allowed in any 1-second window. Values above 3 are ignored.</summary>
        public int MaxFlashesPerWindow { get; set; } = 3;
        public double WindowSeconds { get; set; } = 1.0;

        /// <summary>Large/bright flashes allowed in any 5-second window. Values above 9 are ignored.</summary>
        public int MaxFlashesPerLongWindow { get; set; } = 9;
        public double LongWindowSeconds { get; set; } = 5.0;

        /// <summary>
        /// Screen fraction (0..1) at or above which a flash counts as "large". 0.25 is a conservative sample
        /// threshold, not a standard value; tune it with an analysis tool on your real content.
        /// </summary>
        public float LargeAreaThreshold { get; set; } = 0.25f;

        /// <summary>
        /// Relative-luminance change (0..1) at or above which a flash counts as "bright". 0.1 follows the 10% change
        /// in the WCAG 2.x "general flash" definition.
        /// </summary>
        public float BrightIntensityThreshold { get; set; } = 0.1f;

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
        /// <param name="saturatedRed">True when the effect is a saturated red.</param>
        public FlashDecision RequestFlash(double nowSeconds, float intensity, float screenArea, bool saturatedRed = false)
        {
            intensity = Math.Max(0f, Math.Min(1f, intensity));
            bool reduce = ReduceFlashing;
            bool largeOrBright = screenArea >= LargeAreaThreshold || intensity >= BrightIntensityThreshold;

            // _recent holds allowed large/bright flashes from the last long window, oldest first.
            double longWindow = Math.Max(LongWindowSeconds, WindowSeconds);
            while (_recent.Count > 0 && nowSeconds - _recent.Peek() >= longWindow) _recent.Dequeue();

            // Both budgets apply in every mode, before any conversion.
            if (largeOrBright)
            {
                int inShort = 0;
                foreach (double t in _recent)
                {
                    if (nowSeconds - t < WindowSeconds) inShort++;
                }
                bool overShort = inShort >= Math.Min(3, Math.Max(0, MaxFlashesPerWindow));
                bool overLong = _recent.Count >= Math.Min(9, Math.Max(0, MaxFlashesPerLongWindow));
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

        /// <summary>Forget history (for example on scene load). Never needed for safety.</summary>
        public void Clear() => _recent.Clear();
    }
}
