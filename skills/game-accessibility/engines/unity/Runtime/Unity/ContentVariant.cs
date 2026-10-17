// Game Accessibility sample: presentation variants for tagged content (spiders, gore, needles...).
// Contract: references/architecture.md section 7. Only presentation changes: put hitboxes, AI, damage and rewards
// on the parent, and only meshes/sprites/animators/audio in the variant children.
//
// NOT COMPILED IN THIS REPOSITORY: no Unity Editor was available. See engines/unity/GUIDE.md.

using System;
using System.Globalization;
using UnityEngine;

namespace GameAccessibility.UnityBridge
{
    /// <summary>
    /// Activates exactly one child variant for the current value of a content setting. Swaps in Awake (before
    /// the first rendered frame, also for prefabs instantiated mid-game) and synchronously on change, so the
    /// original never shows for a frame. If no variant matches, the LAST entry (most filtered) is used.
    /// </summary>
    [DefaultExecutionOrder(-900)]
    public sealed class ContentVariant : MonoBehaviour
    {
        [Serializable]
        public struct Variant
        {
            [Tooltip("Setting value as text: \"0\"..\"3\" for content.spiders, \"full\"/\"reduced\"/\"off\" for content.blood.")]
            public string value;
            public GameObject root;
        }

        [SerializeField] private A11yBootstrap bootstrap;

        [Tooltip("Registry key, for example content.spiders or content.blood.")]
        [SerializeField] private string settingKey = "content.spiders";

        [Tooltip("Order from original (first) to most filtered (last).")]
        [SerializeField] private Variant[] variants = Array.Empty<Variant>();

        private IDisposable _sub;

        public string CurrentValue { get; private set; }

        private void Awake()
        {
            // Hide everything except the most filtered variant until the setting is read: if anything goes wrong
            // the player sees the filtered version, never the original.
            Show(variants.Length - 1);
        }

        private void OnEnable()
        {
            var boot = A11yBootstrap.Resolve(bootstrap);
            var store = boot != null ? boot.Store : null;
            if (store == null || !store.Has(settingKey)) return;
            _sub = store.Observe(settingKey, v =>
            {
                CurrentValue = Convert.ToString(v, CultureInfo.InvariantCulture);
                Show(IndexOf(CurrentValue));
            });
        }

        private void OnDisable()
        {
            _sub?.Dispose();
            _sub = null;
        }

        private int IndexOf(string value)
        {
            for (int i = 0; i < variants.Length; i++)
            {
                if (string.Equals(variants[i].value, value, StringComparison.Ordinal)) return i;
            }
            return variants.Length - 1;
        }

        private void Show(int index)
        {
            // Deactivate first, then activate, so two variants are never visible together.
            for (int i = 0; i < variants.Length; i++)
            {
                if (i != index && variants[i].root != null) variants[i].root.SetActive(false);
            }
            if (index >= 0 && index < variants.Length && variants[index].root != null) variants[index].root.SetActive(true);
        }
    }
}
