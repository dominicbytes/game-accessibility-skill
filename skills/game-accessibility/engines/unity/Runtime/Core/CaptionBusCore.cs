// Game Accessibility sample: one semantic audio event, many outputs (subtitles, captions, visualiser, narration).
// Pure C# (netstandard2.1). No UnityEngine references.
//
// Contract: references/architecture.md section 4.
// - Captions come from authored events, never from audio levels, so they fire even when audio is muted.
// - Looping sounds carry a MergeKey and are captioned once while active.
// - A hidden speaker is shown as "Unknown voice": captions never reveal more than a hearing player gets.

using System;
using System.Collections.Generic;
using System.Text;

namespace GameAccessibility
{
    public enum CaptionKind { Dialogue, Sound, Music }
    public enum CaptionImportance { Low, Normal, Critical }

    [Flags]
    public enum CaptionRoute
    {
        None = 0,
        Subtitle = 1,
        Caption = 2,
        Visualizer = 4,
        Narration = 8,
        /// <summary>Merged into an already-active event with the same MergeKey; nothing new was emitted.</summary>
        Merged = 16,
    }

    /// <summary>
    /// A semantic audio event. Immutable. (A class rather than a C# 9 record so it compiles on netstandard2.1
    /// without IsExternalInit.)
    /// </summary>
    public sealed class CaptionEvent
    {
        public string Id { get; }
        public CaptionKind Kind { get; }
        /// <summary>Localisation key, for example "dlg.mara.042" or "snd.alarm".</summary>
        public string TextKey { get; }
        public string SpeakerId { get; }
        /// <summary>True when the speaker cannot be seen or identified by a hearing player.</summary>
        public bool SpeakerHidden { get; }
        /// <summary>Horizontal bearing from the listener in degrees: 0 ahead, negative left, positive right, ±180 behind. Null = not positional.</summary>
        public float? BearingDegrees { get; }
        public CaptionImportance Importance { get; }
        /// <summary>How long the line stays up. Tie it to the audio clip length when voiced.</summary>
        public float DurationSeconds { get; }
        public string MergeKey { get; }
        /// <summary>True when the line has recorded voice. Narration reads only unvoiced dialogue.</summary>
        public bool IsVoiced { get; }

        public CaptionEvent(string id, CaptionKind kind, string textKey, float durationSeconds,
            string speakerId = null, bool speakerHidden = false, float? bearingDegrees = null,
            CaptionImportance importance = CaptionImportance.Normal, string mergeKey = null, bool isVoiced = true)
        {
            Id = id;
            Kind = kind;
            TextKey = textKey ?? throw new ArgumentNullException(nameof(textKey));
            DurationSeconds = Math.Max(0.5f, durationSeconds);
            SpeakerId = speakerId;
            SpeakerHidden = speakerHidden;
            BearingDegrees = bearingDegrees;
            Importance = importance;
            MergeKey = mergeKey;
            IsVoiced = isVoiced;
        }
    }

    /// <summary>What a consumer receives: the event plus resolved, display-ready strings.</summary>
    public readonly struct CaptionLine
    {
        public readonly CaptionEvent Event;
        /// <summary>Localised speaker name, "Unknown voice", or null when names are off or there is no speaker.</summary>
        public readonly string Speaker;
        public readonly string Text;
        /// <summary>Localised direction word ("left", "behind"...) or null when direction is off or unknown.</summary>
        public readonly string Direction;

        public CaptionLine(CaptionEvent e, string speaker, string text, string direction)
        {
            Event = e;
            Speaker = speaker;
            Text = text;
            Direction = direction;
        }
    }

    public sealed class CaptionBusCore
    {
        private readonly A11ySettingsStore _store;
        private readonly Dictionary<string, double> _activeMerges = new Dictionary<string, double>(StringComparer.Ordinal);

        public event Action<CaptionLine> SubtitleRequested;
        public event Action<CaptionLine> CaptionRequested;
        public event Action<CaptionLine> VisualizerRequested;
        public event Action<CaptionLine> NarrationRequested;

        /// <summary>Resolves text keys, speaker ids and direction keys to display strings. Default: identity.</summary>
        public Func<string, string> Localize { get; set; } = key => key;

        /// <summary>Key resolved through <see cref="Localize"/> for hidden speakers.</summary>
        public string UnknownSpeakerKey { get; set; } = "Unknown voice";

        public CaptionBusCore(A11ySettingsStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>
        /// Routes one event to every consumer the player's settings enable. <paramref name="nowSeconds"/> is
        /// real (unscaled) time, used for merge keys. Returns the routes taken (useful in tests).
        /// </summary>
        public CaptionRoute Emit(CaptionEvent e, double nowSeconds)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (e.MergeKey != null)
            {
                if (_activeMerges.TryGetValue(e.MergeKey, out double until) && until > nowSeconds)
                {
                    _activeMerges[e.MergeKey] = Math.Max(until, nowSeconds + e.DurationSeconds);
                    return CaptionRoute.Merged;
                }
                _activeMerges[e.MergeKey] = nowSeconds + e.DurationSeconds;
            }

            bool showNames = _store.GetOr("subtitles.speaker_names", true);
            bool showDirection = _store.GetOr("subtitles.direction", false);
            string speaker = null;
            if (showNames && (e.SpeakerId != null || e.SpeakerHidden))
                speaker = Localize(e.SpeakerHidden ? UnknownSpeakerKey : e.SpeakerId);
            string direction = showDirection && e.BearingDegrees.HasValue ? Localize(DirectionKey(e.BearingDegrees.Value)) : null;
            var line = new CaptionLine(e, speaker, Localize(e.TextKey), direction);

            CaptionRoute routes = CaptionRoute.None;
            if (e.Kind == CaptionKind.Dialogue)
            {
                if (_store.GetOr("subtitles.dialogue", true)) { SubtitleRequested?.Invoke(line); routes |= CaptionRoute.Subtitle; }
                if (!e.IsVoiced && _store.GetOr("narration.dialogue", false)) { NarrationRequested?.Invoke(line); routes |= CaptionRoute.Narration; }
            }
            else if (_store.GetOr("subtitles.sound_captions", false))
            {
                CaptionRequested?.Invoke(line);
                routes |= CaptionRoute.Caption;
            }

            if (e.BearingDegrees.HasValue && e.Kind != CaptionKind.Music && _store.GetOr("audio.sound_visualizer", false))
            {
                VisualizerRequested?.Invoke(line);
                routes |= CaptionRoute.Visualizer;
            }
            return routes;
        }

        /// <summary>Ends a merge early (for example when a looping alarm stops) so the next start is captioned again.</summary>
        public void EndMerge(string mergeKey)
        {
            if (mergeKey != null) _activeMerges.Remove(mergeKey);
        }

        /// <summary>Direction key for a bearing: "ahead", "behind", "left" or "right". Resolved through Localize.</summary>
        public static string DirectionKey(float bearingDegrees)
        {
            float b = bearingDegrees % 360f;
            if (b > 180f) b -= 360f;
            if (b < -180f) b += 360f;
            float a = Math.Abs(b);
            if (a <= 30f) return "ahead";
            if (a >= 150f) return "behind";
            return b < 0f ? "left" : "right";
        }

        /// <summary>
        /// Splits text into pages of at most <paramref name="maxLines"/> lines of about <paramref name="maxCharsPerLine"/>
        /// characters, breaking at spaces. Long words are kept whole. Default: 2 lines, 40 characters.
        /// </summary>
        public static List<string> Paginate(string text, int maxCharsPerLine = 40, int maxLines = 2)
        {
            var pages = new List<string>();
            if (string.IsNullOrEmpty(text)) return pages;
            var lines = new List<string>();
            var current = new StringBuilder();
            foreach (string word in text.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (current.Length > 0 && current.Length + 1 + word.Length > maxCharsPerLine)
                {
                    lines.Add(current.ToString());
                    current.Clear();
                }
                if (current.Length > 0) current.Append(' ');
                current.Append(word);
            }
            if (current.Length > 0) lines.Add(current.ToString());
            for (int i = 0; i < lines.Count; i += maxLines)
            {
                pages.Add(string.Join("\n", lines.GetRange(i, Math.Min(maxLines, lines.Count - i))));
            }
            return pages;
        }
    }
}
