// Game Accessibility sample: narration routing (native screen reader first, self-voicing TTS as fallback).
// Contract: references/architecture.md section 5.
//
// Unity has NO built-in cross-platform text-to-speech. Self-voicing needs a platform backend you supply
// (INarrationBackend): for example Android TextToSpeech via AndroidJavaObject, iOS/macOS AVSpeechSynthesizer
// or Windows SAPI/WinRT through a native plugin. None is included here.
// On Unity 6000.3+ prefer the native AccessibilityHierarchy (Windows, macOS, Android 8+, iOS): the player's own
// screen reader then speaks focus, names, roles and values. See SettingsPanelAccessibility below.
//
// NOT COMPILED IN THIS REPOSITORY: no Unity Editor was available. See engines/unity/GUIDE.md.

using System;
using System.Collections.Generic;
using UnityEngine;
#if GAME_A11Y_ACCESSIBILITY_MODULE && UNITY_6000_3_OR_NEWER
using System.Collections;
using UnityEngine.UI;
#endif
#if GAME_A11Y_ACCESSIBILITY_MODULE && UNITY_6000_0_OR_NEWER
using UnityEngine.Accessibility;
#endif

namespace GameAccessibility.UnityBridge
{
    public enum NarrationPriority
    {
        /// <summary>Focus speech: replaced by newer focus speech; skipped when a native screen reader handles focus.</summary>
        Focus,
        Normal,
        /// <summary>Never dropped from the queue; interrupts non-critical speech.</summary>
        Critical,
    }

    /// <summary>A text-to-speech engine. Implement one per platform you ship.</summary>
    public interface INarrationBackend
    {
        bool IsAvailable { get; }
        bool IsSpeaking { get; }
        void Speak(string text, bool interrupt);
        void Stop();
        /// <summary>1 = the voice's normal rate. Receives narration.rate (0.5..2.5).</summary>
        void SetRate(float rate);
    }

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
    /// otherwise the self-voicing backend when narration.enabled is on.
    /// </summary>
    public sealed class Narrator : MonoBehaviour
    {
        [SerializeField] private A11yBootstrap bootstrap;
        [SerializeField] private int maxQueue = 8;

        /// <summary>Assign your platform TTS backend at startup. Null = no self-voicing available.</summary>
        public INarrationBackend Backend { get; set; }

        private readonly List<(string text, NarrationPriority priority)> _queue = new List<(string, NarrationPriority)>();
        private NarrationPriority _speakingPriority;
        private A11yBootstrap _boot;
        private IDisposable _rateSub;

        private void OnEnable()
        {
            _boot = A11yBootstrap.Resolve(bootstrap);
            if (_boot == null || _boot.Store == null) return;
            _boot.Captions.NarrationRequested += OnCaptionNarration;
            _rateSub = _boot.Store.Observe("narration.rate", v => Backend?.SetRate(Convert.ToSingle(v)));
        }

        private void OnDisable()
        {
            if (_boot != null && _boot.Captions != null) _boot.Captions.NarrationRequested -= OnCaptionNarration;
            _rateSub?.Dispose();
            _rateSub = null;
            Stop();
        }

        private void OnCaptionNarration(CaptionLine line) =>
            Speak(string.IsNullOrEmpty(line.Speaker) ? line.Text : line.Speaker + ": " + line.Text);

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

        private bool SelfVoicingOn =>
            _boot != null && _boot.Store != null && _boot.Store.GetOr("narration.enabled", false)
            && Backend != null && Backend.IsAvailable;

        /// <summary>Narrates a focus change: "Text size, slider, 150 percent. Scales all text."</summary>
        public void AnnounceFocus(string name, string role, string value = null, string hint = null)
        {
            string text = name + ", " + role;
            if (!string.IsNullOrEmpty(value)) text += ", " + value;
            if (!string.IsNullOrEmpty(hint)) text += ". " + hint;
            Speak(text, NarrationPriority.Focus);
        }

        public void Speak(string text, NarrationPriority priority = NarrationPriority.Normal)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (NativeRouteActive)
            {
                if (priority == NarrationPriority.Focus) return; // the screen reader already reads focused nodes
#if GAME_A11Y_ACCESSIBILITY_MODULE && UNITY_6000_0_OR_NEWER
                AssistiveSupport.notificationDispatcher?.SendAnnouncement(text);
#endif
                return;
            }

            if (!SelfVoicingOn) return;

            if (priority == NarrationPriority.Focus)
            {
                _queue.RemoveAll(e => e.priority == NarrationPriority.Focus); // stale focus speech is useless
                if (Backend.IsSpeaking && _speakingPriority == NarrationPriority.Focus)
                {
                    SpeakNow(text, priority, interrupt: true);
                    return;
                }
            }

            if (priority == NarrationPriority.Critical)
            {
                if (Backend.IsSpeaking && _speakingPriority != NarrationPriority.Critical)
                {
                    SpeakNow(text, priority, interrupt: true);
                    return;
                }
                _queue.Insert(CountLeadingCritical(), (text, priority));
                return;
            }

            if (_queue.Count >= maxQueue)
            {
                int drop = _queue.FindIndex(e => e.priority != NarrationPriority.Critical);
                if (drop < 0) return;
                _queue.RemoveAt(drop);
            }
            _queue.Add((text, priority));
        }

        public void Stop()
        {
            _queue.Clear();
            Backend?.Stop();
        }

        private int CountLeadingCritical()
        {
            int i = 0;
            while (i < _queue.Count && _queue[i].priority == NarrationPriority.Critical) i++;
            return i;
        }

        private void SpeakNow(string text, NarrationPriority priority, bool interrupt)
        {
            _speakingPriority = priority;
            Backend.Speak(text, interrupt);
        }

        private void Update()
        {
            if (_queue.Count == 0 || Backend == null || Backend.IsSpeaking) return;
            var next = _queue[0];
            _queue.RemoveAt(0);
            SpeakNow(next.text, next.priority, interrupt: false);
        }
    }

#if GAME_A11Y_ACCESSIBILITY_MODULE && UNITY_6000_3_OR_NEWER
    /// <summary>
    /// Example: exposes a uGUI settings panel to the OS screen reader through Unity 6000.3+ AccessibilityHierarchy.
    /// Buttons, toggles and sliders become nodes with label, role, value and hint. Only the main window is exposed.
    /// Test on each target platform with its real screen reader; Play Mode in the Editor is not enough.
    /// </summary>
    public sealed class SettingsPanelAccessibility : MonoBehaviour
    {
        [Serializable]
        public struct Item
        {
            public Selectable control;
            public string label;
            [Tooltip("One-line effect description, for example the registry description.")]
            public string hint;
            [Tooltip("Slider step for increment/decrement. 0 = 10% of the range.")]
            public float step;
        }

        [SerializeField] private Item[] items = Array.Empty<Item>();

        private AccessibilityHierarchy _hierarchy;
        private readonly List<(Item item, AccessibilityNode node)> _nodes = new List<(Item, AccessibilityNode)>();

        private void OnEnable()
        {
            AssistiveSupport.screenReaderStatusChanged += OnScreenReaderStatusChanged;
            StartCoroutine(BuildAtEndOfFrame());
        }

        private void OnDisable()
        {
            AssistiveSupport.screenReaderStatusChanged -= OnScreenReaderStatusChanged;
            if (_hierarchy != null && AssistiveSupport.activeHierarchy == _hierarchy) AssistiveSupport.activeHierarchy = null;
            _nodes.Clear();
            _hierarchy = null;
        }

        // Unity clears activeHierarchy when the reader turns off; it must be set again when it turns on.
        private void OnScreenReaderStatusChanged(bool on)
        {
            if (on && isActiveAndEnabled) StartCoroutine(BuildAtEndOfFrame());
        }

        private IEnumerator BuildAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame(); // layout must be final before frames are read
            Build();
        }

        private void Build()
        {
            _hierarchy = new AccessibilityHierarchy();
            _nodes.Clear();
            foreach (var item in items)
            {
                if (item.control == null) continue;
                AccessibilityNode node = _hierarchy.AddNode(item.label);
                node.hint = item.hint;
                var rect = (RectTransform)item.control.transform;
                node.frameGetter = () => ScreenFrame(rect);
                Configure(item, node);
                _nodes.Add((item, node));
            }
            // Takes effect only while the screen reader is on.
            AssistiveSupport.activeHierarchy = _hierarchy;
        }

        private static void Configure(Item item, AccessibilityNode node)
        {
            switch (item.control)
            {
                case Toggle toggle:
                    node.role = AccessibilityRole.Toggle;
                    node.value = toggle.isOn ? "On" : "Off";
                    node.invoked += () => { toggle.isOn = !toggle.isOn; node.value = toggle.isOn ? "On" : "Off"; return true; };
                    break;
                case Slider slider:
                    node.role = AccessibilityRole.Slider;
                    node.value = FormatSlider(slider);
                    float step = item.step > 0f ? item.step : (slider.maxValue - slider.minValue) * 0.1f;
                    node.incremented += () => { slider.value += step; node.value = FormatSlider(slider); };
                    node.decremented += () => { slider.value -= step; node.value = FormatSlider(slider); };
                    break;
                case Button button:
                    node.role = AccessibilityRole.Button;
                    node.invoked += () => { button.onClick.Invoke(); return true; };
                    break;
                default:
                    node.role = AccessibilityRole.None;
                    break;
            }
        }

        private void LateUpdate()
        {
            // Keep values in sync when they change by other input (keyboard, controller, presets).
            foreach (var (item, node) in _nodes)
            {
                string value = item.control is Toggle t ? (t.isOn ? "On" : "Off")
                    : item.control is Slider s ? FormatSlider(s) : null;
                if (value != null && value != node.value) node.value = value;
            }
        }

        private static string FormatSlider(Slider s)
        {
            float range = s.maxValue - s.minValue;
            float pct = range > 0f ? (s.value - s.minValue) / range * 100f : 0f;
            return Mathf.RoundToInt(pct) + " percent";
        }

        /// <summary>Screen rect with a top-left origin, as computed by Unity's a11y-public-sample (LetterSpell).</summary>
        private static Rect ScreenFrame(RectTransform rt)
        {
            if (rt == null) return Rect.zero;
            var canvas = rt.GetComponentInParent<Canvas>();
            Camera cam = canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            var corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            foreach (var c in corners)
            {
                Vector2 p = RectTransformUtility.WorldToScreenPoint(cam, c);
                minX = Mathf.Min(minX, p.x); maxX = Mathf.Max(maxX, p.x);
                minY = Mathf.Min(minY, p.y); maxY = Mathf.Max(maxY, p.y);
            }
            return new Rect(minX, Screen.height - maxY, maxX - minX, maxY - minY);
        }
    }
#endif
}
