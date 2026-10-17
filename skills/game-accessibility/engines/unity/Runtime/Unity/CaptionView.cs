// Game Accessibility sample: subtitle and sound-caption presenter for uGUI.
// Uses UnityEngine.UI.Text by default. When TextMesh Pro is present (GAME_A11Y_TMP, set by the asmdef
// versionDefines), the fields are TMP_Text instead. Contract: references/architecture.md section 4.
//
// NOT COMPILED IN THIS REPOSITORY: no Unity Editor was available. See engines/unity/GUIDE.md.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if GAME_A11Y_TMP
using TMPro;
using LabelType = TMPro.TMP_Text;
#else
using LabelType = UnityEngine.UI.Text;
#endif

namespace GameAccessibility.UnityBridge
{
    /// <summary>
    /// Shows dialogue (max 2 lines of ~40 characters per page, speaker first) and sound captions in separate
    /// labels. Size and background opacity follow the settings. Place the RectTransforms inside the safe area and
    /// away from input prompts. Timing uses unscaled time so game speed does not change reading time.
    /// </summary>
    public sealed class CaptionView : MonoBehaviour
    {
        [SerializeField] private A11yBootstrap bootstrap;

        [Header("Dialogue")]
        [SerializeField] private LabelType subtitleLabel;
        [SerializeField] private Image subtitleBackground;

        [Header("Sound captions")]
        [SerializeField] private LabelType captionLabel;
        [SerializeField] private Image captionBackground;

        [Tooltip("Font size at subtitles.size = 1. Choose a size that is readable at your minimum supported resolution.")]
        [SerializeField] private float baseFontSize = 36f;
        [SerializeField] private int maxCharsPerLine = 40;
        [SerializeField] private float minSecondsPerPage = 1.5f;

        private readonly Queue<(string text, float seconds)> _subtitlePages = new Queue<(string, float)>();
        private readonly Queue<(string text, float seconds)> _captionPages = new Queue<(string, float)>();
        private float _subtitleUntil, _captionUntil;
        private CaptionBusCore _bus;
        private IDisposable[] _subs = Array.Empty<IDisposable>();

        private void OnEnable()
        {
            var boot = A11yBootstrap.Resolve(bootstrap);
            if (boot == null || boot.Store == null) return;
            _bus = boot.Captions;
            _bus.SubtitleRequested += OnSubtitle;
            _bus.CaptionRequested += OnCaption;
            var store = boot.Store;
            _subs = new[]
            {
                store.Observe("subtitles.size", v => ApplySize(Convert.ToSingle(v))),
                store.Observe("subtitles.background_opacity", v => ApplyOpacity(Convert.ToSingle(v))),
                store.Observe("subtitles.dialogue", v => { if (!(bool)v) ClearSubtitles(); }),
                store.Observe("subtitles.sound_captions", v => { if (!(bool)v) ClearCaptions(); }),
            };
            ClearSubtitles();
            ClearCaptions();
        }

        private void OnDisable()
        {
            if (_bus != null)
            {
                _bus.SubtitleRequested -= OnSubtitle;
                _bus.CaptionRequested -= OnCaption;
                _bus = null;
            }
            foreach (var s in _subs) s.Dispose();
            _subs = Array.Empty<IDisposable>();
        }

        private void OnSubtitle(CaptionLine line)
        {
            // A new line replaces the old one: never stack stale dialogue.
            _subtitlePages.Clear();
            string prefix = string.IsNullOrEmpty(line.Speaker) ? "" : line.Speaker + ": ";
            string suffix = string.IsNullOrEmpty(line.Direction) ? "" : " (" + line.Direction + ")";
            Enqueue(_subtitlePages, prefix + line.Text + suffix, line.Event.DurationSeconds);
            _subtitleUntil = 0f;
        }

        private void OnCaption(CaptionLine line)
        {
            string text = string.IsNullOrEmpty(line.Direction) ? line.Text : line.Text + " (" + line.Direction + ")";
            Enqueue(_captionPages, text, line.Event.DurationSeconds);
            if (_captionPages.Count > 3) _captionPages.Dequeue(); // bounded; captions are also in any log you keep
        }

        private void Enqueue(Queue<(string, float)> queue, string text, float totalSeconds)
        {
            var pages = CaptionBusCore.Paginate(text, maxCharsPerLine, 2);
            if (pages.Count == 0) return;
            float perPage = Mathf.Max(minSecondsPerPage, totalSeconds / pages.Count);
            foreach (string p in pages) queue.Enqueue((p, perPage));
        }

        private void Update()
        {
            float now = Time.unscaledTime;
            Advance(_subtitlePages, ref _subtitleUntil, now, subtitleLabel, subtitleBackground);
            Advance(_captionPages, ref _captionUntil, now, captionLabel, captionBackground);
        }

        private static void Advance(Queue<(string text, float seconds)> queue, ref float until, float now,
            LabelType label, Image background)
        {
            if (now < until) return;
            if (queue.Count == 0)
            {
                SetVisible(label, background, false);
                return;
            }
            var page = queue.Dequeue();
            if (label != null) label.text = page.text;
            SetVisible(label, background, true);
            until = now + page.seconds;
        }

        private static void SetVisible(LabelType label, Image background, bool visible)
        {
            if (label != null && label.enabled != visible) label.enabled = visible;
            if (background != null && background.enabled != visible) background.enabled = visible;
        }

        private void ApplySize(float scale)
        {
            float size = baseFontSize * Mathf.Max(1f, scale);
#if GAME_A11Y_TMP
            // Never let auto-size shrink text the player enlarged.
            if (subtitleLabel != null) { subtitleLabel.enableAutoSizing = false; subtitleLabel.fontSize = size; }
            if (captionLabel != null) { captionLabel.enableAutoSizing = false; captionLabel.fontSize = size; }
#else
            if (subtitleLabel != null) { subtitleLabel.resizeTextForBestFit = false; subtitleLabel.fontSize = Mathf.RoundToInt(size); }
            if (captionLabel != null) { captionLabel.resizeTextForBestFit = false; captionLabel.fontSize = Mathf.RoundToInt(size); }
#endif
        }

        private void ApplyOpacity(float alpha)
        {
            foreach (var bg in new[] { subtitleBackground, captionBackground })
            {
                if (bg == null) continue;
                Color c = bg.color;
                c.a = Mathf.Clamp01(alpha);
                bg.color = c;
            }
        }

        private void ClearSubtitles()
        {
            _subtitlePages.Clear();
            _subtitleUntil = 0f;
            SetVisible(subtitleLabel, subtitleBackground, false);
        }

        private void ClearCaptions()
        {
            _captionPages.Clear();
            _captionUntil = 0f;
            SetVisible(captionLabel, captionBackground, false);
        }
    }
}
