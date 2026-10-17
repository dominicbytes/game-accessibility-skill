// Game Accessibility sample: narration routing (native screen reader first, self-voicing TTS as fallback).
// Contract: references/architecture.md section 5. The routing and queue rules live in Core (NarrationQueue).
//
// Unity has NO built-in cross-platform text-to-speech. Self-voicing needs a platform backend you supply
// (INarrationBackend, in Core): for example Android TextToSpeech via AndroidJavaObject, iOS/macOS
// AVSpeechSynthesizer or Windows SAPI/WinRT through a native plugin. None is included here.
// On Unity 6000.3+ prefer the native AccessibilityHierarchy (Windows, macOS, Android 8+, iOS): the player's own
// screen reader then speaks focus, names, roles and values. See SettingsPanelAccessibility.cs.
//
// NOT COMPILED IN THIS REPOSITORY: no Unity Editor was available. See engines/unity/GUIDE.md.

using System;
using UnityEngine;
#if GAME_A11Y_ACCESSIBILITY_MODULE && UNITY_6000_0_OR_NEWER
using UnityEngine.Accessibility;
#endif

namespace GameAccessibility.UnityBridge
{
    /// <summary>Development backend: logs instead of speaking. Never ship it as "narration".</summary>
    public sealed class LogNarrationBackend : INarrationBackend
    {
        public bool IsAvailable => true;
        public bool IsSpeaking => false;
        public void Speak(string text, bool interrupt) => Debug.Log("[Narration] " + text);
        public void Stop() { }
        public void SetRate(float rate) { }
    }

    /// <summary>
    /// One narration entry point for menus and gameplay. Picks one route per announcement so speech never doubles:
    /// native screen reader (announcements via the notification dispatcher) when it is running and Unity exposes it;
    /// otherwise the self-voicing backend when the utterance's setting is on (narration.enabled for menus and
    /// announcements, narration.dialogue for unvoiced dialogue). Turning either setting off stops its speech at once.
    /// </summary>
    public sealed class Narrator : MonoBehaviour
    {
        [SerializeField] private A11yBootstrap bootstrap;
        [SerializeField] private int maxQueue = 8;

        private INarrationBackend _backend;
        private NarrationQueue _queue;
        private A11yBootstrap _boot;
        private IDisposable _rateSub;

        /// <summary>Assign your platform TTS backend at startup. Null = no self-voicing available.</summary>
        public INarrationBackend Backend
        {
            get => _backend;
            set
            {
                if (_backend != null && _backend != value) _queue?.Stop();
                _backend = value;
                if (_queue != null) _queue.Backend = value;
                if (value != null && _boot != null && _boot.Store != null)
                    value.SetRate(_boot.Store.GetOr("narration.rate", 1f));
            }
        }

        private void OnEnable()
        {
            _boot = A11yBootstrap.Resolve(bootstrap);
            if (_boot == null || _boot.Store == null) return;
            _queue = new NarrationQueue(_boot.Store, maxQueue)
            {
                Backend = _backend,
                NativeRouteActive = () => NativeRouteActive,
                Announce = SendNativeAnnouncement,
            };
            _boot.Captions.NarrationRequested += OnCaptionNarration;
            _rateSub = _boot.Store.Observe("narration.rate", v => _backend?.SetRate(Convert.ToSingle(v)));
        }

        private void OnDisable()
        {
            if (_boot != null && _boot.Captions != null) _boot.Captions.NarrationRequested -= OnCaptionNarration;
            _rateSub?.Dispose();
            _rateSub = null;
            if (_queue != null)
            {
                _queue.Stop();
                _queue.Dispose();
                _queue = null;
            }
        }

        private void OnCaptionNarration(CaptionLine line) =>
            Speak(string.IsNullOrEmpty(line.Speaker) ? line.Text : line.Speaker + ": " + line.Text,
                NarrationPriority.Normal, NarrationChannel.Dialogue);

        /// <summary>True when an OS screen reader is running and this build can talk to it natively.</summary>
        public bool NativeRouteActive
        {
            get
            {
#if GAME_A11Y_ACCESSIBILITY_MODULE && !UNITY_EDITOR && ((UNITY_6000_0_OR_NEWER && (UNITY_ANDROID || UNITY_IOS)) || (UNITY_6000_3_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)))
                return _boot != null && _boot.OsScreenReaderActive;
#else
                return false;
#endif
            }
        }

        private static void SendNativeAnnouncement(string text)
        {
#if GAME_A11Y_ACCESSIBILITY_MODULE && UNITY_6000_0_OR_NEWER
            AssistiveSupport.notificationDispatcher?.SendAnnouncement(text);
#endif
        }

        /// <summary>Narrates a focus change: "Text size, slider, 150 percent. Scales all text."</summary>
        public void AnnounceFocus(string name, string role, string value = null, string hint = null)
        {
            string text = name + ", " + role;
            if (!string.IsNullOrEmpty(value)) text += ", " + value;
            if (!string.IsNullOrEmpty(hint)) text += ". " + hint;
            Speak(text, NarrationPriority.Focus);
        }

        /// <summary>
        /// Speaks <paramref name="text"/> through one route. Menu-channel speech (the default) follows
        /// narration.enabled; pass <see cref="NarrationChannel.Dialogue"/> for unvoiced lines read aloud.
        /// </summary>
        public void Speak(string text, NarrationPriority priority = NarrationPriority.Normal,
            NarrationChannel channel = NarrationChannel.Menu) => _queue?.Speak(text, priority, channel);

        /// <summary>Stops all self-voicing and empties the queue.</summary>
        public void Stop() => _queue?.Stop();

        // Re-checks route ownership and each channel's setting before dispatching queued speech.
        private void Update() => _queue?.Tick();
    }
}
