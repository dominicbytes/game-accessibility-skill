// Game Accessibility sample: binds audio settings to exposed AudioMixer parameters.
// Volumes are linear 0..1 in the settings and converted to decibels here (20 * log10(v)).
//
// NOT COMPILED IN THIS REPOSITORY: no Unity Editor was available. See engines/unity/GUIDE.md.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameAccessibility.UnityBridge
{
    public sealed class AudioMixerBinding : MonoBehaviour
    {
        public enum Mode
        {
            /// <summary>Float setting 0..1 to decibels on an exposed volume parameter.</summary>
            LinearToDecibels,
            /// <summary>Bool setting to <see cref="Binding.onValue"/>/<see cref="Binding.offValue"/> (for example a compressor threshold or wet mix).</summary>
            BoolToValue,
        }

        [Serializable]
        public struct Binding
        {
            [Tooltip("Registry key, for example audio.music or audio.reduce_loud_sounds.")]
            public string settingKey;
            [Tooltip("Exposed parameter name in the AudioMixer (right-click a parameter > Expose).")]
            public string exposedParameter;
            public Mode mode;
            public float onValue;
            public float offValue;
        }

        public const float SilenceDb = -80f;

        [SerializeField] private A11yBootstrap bootstrap;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private Binding[] bindings =
        {
            new Binding { settingKey = "audio.master", exposedParameter = "MasterVolume", mode = Mode.LinearToDecibels },
            new Binding { settingKey = "audio.music", exposedParameter = "MusicVolume", mode = Mode.LinearToDecibels },
            new Binding { settingKey = "audio.sfx", exposedParameter = "SfxVolume", mode = Mode.LinearToDecibels },
            new Binding { settingKey = "audio.voice", exposedParameter = "VoiceVolume", mode = Mode.LinearToDecibels },
            new Binding { settingKey = "audio.ambience", exposedParameter = "AmbienceVolume", mode = Mode.LinearToDecibels },
        };

        private IDisposable[] _subs = Array.Empty<IDisposable>();

        /// <summary>Linear 0..1 to decibels. 0 maps to <see cref="SilenceDb"/> instead of -infinity.</summary>
        public static float LinearToDb(float linear) =>
            linear <= 0.0001f ? SilenceDb : Mathf.Max(SilenceDb, 20f * Mathf.Log10(linear));

        private bool _started;

        // Start, not Awake: AudioMixer.SetFloat calls made in Awake are widely reported to be ignored.
        private void Start()
        {
            _started = true;
            Bind();
        }

        // Re-bind after a disable/enable cycle. The first enable waits for Start.
        private void OnEnable()
        {
            if (_started) Bind();
        }

        private void Bind()
        {
            var boot = A11yBootstrap.Resolve(bootstrap);
            var store = boot != null ? boot.Store : null;
            if (store == null || mixer == null) return;
            var subs = new List<IDisposable>();
            foreach (var b in bindings)
            {
                if (!store.Has(b.settingKey) || string.IsNullOrEmpty(b.exposedParameter)) continue;
                Binding binding = b;
                subs.Add(store.Observe(binding.settingKey, v => Apply(binding, v)));
            }
            _subs = subs.ToArray();
        }

        private void OnDisable()
        {
            foreach (var s in _subs) s.Dispose();
            _subs = Array.Empty<IDisposable>();
        }

        private void Apply(Binding b, object value)
        {
            float target = b.mode == Mode.LinearToDecibels
                ? LinearToDb(Convert.ToSingle(value))
                : (value is bool on && on ? b.onValue : b.offValue);
            if (!mixer.SetFloat(b.exposedParameter, target))
                Debug.LogWarning($"[A11y] AudioMixer parameter '{b.exposedParameter}' is not exposed.", this);
        }
    }
}
