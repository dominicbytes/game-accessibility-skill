// Game Accessibility sample: exposes a uGUI settings panel to the OS screen reader (Unity 6000.3+).
// Contract: references/architecture.md section 5. Kept in its own file so Unity can attach it as a component
// (a MonoBehaviour must live in a file with the same name).
//
// NOT COMPILED IN THIS REPOSITORY: no Unity Editor was available. See engines/unity/GUIDE.md.

#if GAME_A11Y_ACCESSIBILITY_MODULE && UNITY_6000_3_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UI;

namespace GameAccessibility.UnityBridge
{
    /// <summary>
    /// Exposes a uGUI settings panel to the OS screen reader through Unity 6000.3+ AccessibilityHierarchy.
    /// Buttons, toggles and sliders become nodes with label, role, value and hint. Only the main window is exposed.
    /// Hidden controls (inactive, disabled or under a transparent CanvasGroup or disabled Canvas) are deactivated
    /// so the reader skips them; non-interactable controls are marked Disabled and their actions do nothing,
    /// using the same IsActive()/IsInteractable() checks as ordinary uGUI input. State is re-read every frame.
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

        private static readonly List<CanvasGroup> s_Groups = new List<CanvasGroup>();
        private static readonly List<Canvas> s_Canvases = new List<Canvas>();

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
            SyncNodes(notify: false);
            // Takes effect only while the screen reader is on.
            AssistiveSupport.activeHierarchy = _hierarchy;
        }

        /// <summary>The same gate uGUI applies to pointer, keyboard and controller input.</summary>
        private static bool CanOperate(Selectable s) => s != null && s.IsActive() && s.IsInteractable();

        private static void Configure(Item item, AccessibilityNode node)
        {
            switch (item.control)
            {
                case Toggle toggle:
                    node.role = AccessibilityRole.Toggle;
                    node.value = toggle.isOn ? "On" : "Off";
                    node.invoked += () =>
                    {
                        if (!node.isActive || !CanOperate(toggle)) return false;
                        toggle.isOn = !toggle.isOn;
                        node.value = toggle.isOn ? "On" : "Off";
                        return true;
                    };
                    break;
                case Slider slider:
                    node.role = AccessibilityRole.Slider;
                    node.value = FormatSlider(slider);
                    float step = item.step > 0f ? item.step : (slider.maxValue - slider.minValue) * 0.1f;
                    node.incremented += () =>
                    {
                        if (!node.isActive || !CanOperate(slider)) return;
                        slider.value += step;
                        node.value = FormatSlider(slider);
                    };
                    node.decremented += () =>
                    {
                        if (!node.isActive || !CanOperate(slider)) return;
                        slider.value -= step;
                        node.value = FormatSlider(slider);
                    };
                    break;
                case Button button:
                    node.role = AccessibilityRole.Button;
                    // A disabled or hidden button never runs its command, even through the reader.
                    node.invoked += () =>
                    {
                        if (!node.isActive || !CanOperate(button)) return false;
                        button.onClick.Invoke();
                        return true;
                    };
                    break;
                default:
                    node.role = AccessibilityRole.None;
                    break;
            }
        }

        private void LateUpdate()
        {
            if (_hierarchy != null) SyncNodes(notify: true);
        }

        // Keeps visibility, disabled state and values in sync when the UI changes by other input (keyboard,
        // controller, presets, menus opening and closing).
        private void SyncNodes(bool notify)
        {
            bool layoutChanged = false;
            foreach (var (item, node) in _nodes)
            {
                Selectable control = item.control;
                bool visible = IsVisible(control);
                if (node.isActive != visible)
                {
                    node.isActive = visible;
                    layoutChanged = true;
                }

                AccessibilityState state = visible && !control.IsInteractable() ? AccessibilityState.Disabled : AccessibilityState.None;
                if (node.state != state) node.state = state;

                if (!visible) continue;
                string value = control is Toggle t ? (t.isOn ? "On" : "Off")
                    : control is Slider s ? FormatSlider(s) : null;
                if (value != null && value != node.value) node.value = value;
            }
            if (notify && layoutChanged) AssistiveSupport.notificationDispatcher?.SendLayoutChanged(null);
        }

        /// <summary>
        /// Shown to a sighted player: active and enabled, no transparent CanvasGroup above it (until one that
        /// ignores parent groups), and no disabled Canvas above it.
        /// </summary>
        private static bool IsVisible(Selectable control)
        {
            if (control == null || !control.IsActive()) return false;
            control.GetComponentsInParent(false, s_Groups);
            foreach (var g in s_Groups)
            {
                if (!g.enabled) continue;
                if (g.alpha <= 0f) return false;
                if (g.ignoreParentGroups) break;
            }
            control.GetComponentsInParent(false, s_Canvases);
            foreach (var c in s_Canvases)
            {
                if (!c.enabled) return false;
            }
            return true;
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
}
#endif
