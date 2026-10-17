// Game Accessibility sample: tag a guidance visual (painted ledge, trail, highlight, objective marker) with its channel.
// Contract: references/architecture.md section 6. Hazard warnings are NOT guidance: tick "hazard" and they stay on.
//
// NOT COMPILED IN THIS REPOSITORY: no Unity Editor was available. See engines/unity/GUIDE.md.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAccessibility.UnityBridge
{
    /// <summary>
    /// Applies the channel's effective guidance level to the listed Renderers and CanvasGroups. Paint, Trail and
    /// InteractHighlight read guidance.paint / guidance.trail / guidance.highlights, and "follow" (the default) uses
    /// guidance.level (see <see cref="GuidanceRules"/>). ObjectiveMarker reads guidance.objective_marker.
    /// off = hidden, on_request = hidden until <see cref="RequestHint"/>, subtle = ~40%, standard = authored,
    /// boosted = brighter plus the optional outline object. Paint baked into albedo textures cannot be toggled;
    /// author it as a decal, overlay or material parameter.
    /// </summary>
    public sealed class GuidanceChannel : MonoBehaviour
    {
        [SerializeField] private A11yBootstrap bootstrap;
        [SerializeField] private GuidanceKind channel = GuidanceKind.Paint;

        [Tooltip("Lethal edge, attack telegraph or required prompt. Always shown as authored, whatever the guidance level.")]
        [SerializeField] private bool hazard;

        [SerializeField] private Renderer[] renderers = Array.Empty<Renderer>();
        [SerializeField] private CanvasGroup[] canvasGroups = Array.Empty<CanvasGroup>();

        [Tooltip("Optional float shader property for intensity (1 = authored). Leave empty to only toggle visibility.")]
        [SerializeField] private string intensityProperty = "_GuidanceIntensity";

        [Tooltip("Optional child (outline, glow) enabled only at 'boosted'.")]
        [SerializeField] private GameObject boostedExtras;

        [SerializeField] private float subtleIntensity = 0.4f;
        [SerializeField] private float boostedIntensity = 1.5f;

        private static readonly List<GuidanceChannel> s_All = new List<GuidanceChannel>();
        private static float s_HintUntil = -1f;

        private A11ySettingsStore _store;
        private IDisposable[] _subs = Array.Empty<IDisposable>();
        private MaterialPropertyBlock _block;
        private int _propertyId;
        private string _level = "standard";
        private bool _hintVisible;

        /// <summary>Shows on_request guidance for <paramref name="seconds"/>. Bind it to your "hint" action.</summary>
        public static void RequestHint(float seconds = 5f)
        {
            s_HintUntil = Time.unscaledTime + Mathf.Max(0.5f, seconds);
            foreach (var g in s_All) g.Apply();
        }

        private void OnEnable()
        {
            s_All.Add(this);
            _block ??= new MaterialPropertyBlock();
            _propertyId = string.IsNullOrEmpty(intensityProperty) ? 0 : Shader.PropertyToID(intensityProperty);
            var boot = A11yBootstrap.Resolve(bootstrap);
            _store = boot != null ? boot.Store : null;
            if (_store == null)
            {
                Apply();
                return;
            }
            // Observe both the channel key and the overall level: either can change the effective level.
            Action<object> recompute = _ => { _level = GuidanceRules.EffectiveLevel(_store, channel); Apply(); };
            _subs = new[]
            {
                _store.Observe(GuidanceRules.ChannelKey(channel), recompute),
                _store.Observe(GuidanceRules.OverallKey, recompute),
            };
        }

        private void OnDisable()
        {
            s_All.Remove(this);
            foreach (var s in _subs) s.Dispose();
            _subs = Array.Empty<IDisposable>();
        }

        private void Update()
        {
            // Hide again when an on_request hint expires.
            if (_hintVisible && Time.unscaledTime >= s_HintUntil) Apply();
        }

        private void Apply()
        {
            bool boosted = false;
            float intensity = hazard
                ? 1f // hazards are not guidance: always as authored
                : GuidanceRules.Intensity(_level, Time.unscaledTime < s_HintUntil, out boosted, subtleIntensity, boostedIntensity);

            bool visible = intensity > 0f;
            _hintVisible = !hazard && _level == "on_request" && visible;
            foreach (var r in renderers)
            {
                if (r == null) continue;
                r.enabled = visible;
                if (_propertyId != 0 && visible)
                {
                    r.GetPropertyBlock(_block);
                    _block.SetFloat(_propertyId, intensity);
                    r.SetPropertyBlock(_block);
                }
            }
            foreach (var g in canvasGroups)
            {
                if (g == null) continue;
                g.alpha = Mathf.Clamp01(intensity);
            }
            if (boostedExtras != null) boostedExtras.SetActive(visible && boosted);
        }
    }
}
