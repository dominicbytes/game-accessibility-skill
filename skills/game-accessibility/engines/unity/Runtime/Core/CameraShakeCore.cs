// Game Accessibility sample: camera-shake trauma with the player's scale applied at final output.
// Pure C# (netstandard2.1). No UnityEngine references. CameraComfort (Unity adapter) drives it.
//
// motion.camera_shake is read every time the amplitude is computed, so lowering it during an active shake
// lowers that shake at once. Setting it to 0 (Off) also discards accumulated trauma, so a shake never resumes
// later even with zero decay, and new shake is ignored while it stays at 0.

using System;

namespace GameAccessibility
{
    public sealed class CameraShakeCore : IDisposable
    {
        public const string ShakeKey = "motion.camera_shake";

        private readonly A11ySettingsStore _store;
        private readonly IDisposable _subscription;

        public CameraShakeCore(A11ySettingsStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _subscription = _store.Observe(ShakeKey, _ => { if (Scale <= 0f) Clear(); });
        }

        /// <summary>Trauma lost per real second. 0 keeps a shake until <see cref="Clear"/>.</summary>
        public float DecayPerSecond { get; set; } = 1.5f;

        /// <summary>Authored trauma 0..1, before the player's scale.</summary>
        public float Trauma { get; private set; }

        /// <summary>motion.camera_shake (0..1). A game that did not register the key keeps authored shake (1).</summary>
        public float Scale
        {
            get
            {
                float s = _store.GetOr(ShakeKey, 1f);
                return float.IsNaN(s) ? 0f : Math.Max(0f, Math.Min(1f, s));
            }
        }

        /// <summary>
        /// Multiplier for the authored maximum offset and roll this frame: trauma squared (a gentle tail) times the
        /// current player scale. 0 means the camera must sit at rest.
        /// </summary>
        public float Amplitude => Trauma * Trauma * Scale;

        public void AddTrauma(float amount)
        {
            if (Scale <= 0f || float.IsNaN(amount)) return; // Off removes shake completely
            Trauma = Math.Min(1f, Trauma + Math.Max(0f, Math.Min(1f, amount)));
        }

        /// <summary>Advances decay by real (unscaled) seconds.</summary>
        public void Tick(float unscaledDeltaSeconds)
        {
            if (unscaledDeltaSeconds > 0f) Trauma = Math.Max(0f, Trauma - Math.Max(0f, DecayPerSecond) * unscaledDeltaSeconds);
        }

        public void Clear() => Trauma = 0f;

        public void Dispose() => _subscription.Dispose();
    }
}
