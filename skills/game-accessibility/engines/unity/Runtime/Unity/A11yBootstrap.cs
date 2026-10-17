// Game Accessibility sample: Unity owner of the settings store and the core services.
// Put one in the first (boot) scene. It loads the registry, loads/saves the settings file, imports OS
// preferences, and exposes the services. It does not hold game state.
//
// NOT COMPILED IN THIS REPOSITORY: no Unity Editor was available. See engines/unity/GUIDE.md.

using System;
using System.IO;
using UnityEngine;
#if GAME_A11Y_ACCESSIBILITY_MODULE && UNITY_6000_0_OR_NEWER
using UnityEngine.Accessibility;
#endif

namespace GameAccessibility.UnityBridge
{
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public sealed class A11yBootstrap : MonoBehaviour
    {
        public const string FileName = "accessibility.json";

        [Tooltip("Registry JSON. Leave empty to load Resources/settings_registry.")]
        [SerializeField] private TextAsset registry;

        [Tooltip("Register only these keys (copy the ones your game's signals need). Empty = every key.")]
        [SerializeField] private string[] onlyKeys = Array.Empty<string>();

        [Tooltip("Keep this object across scene loads.")]
        [SerializeField] private bool dontDestroyOnLoad = true;

        [Tooltip("Seconds between automatic saves after a change.")]
        [SerializeField] private float saveDelaySeconds = 0.5f;

        /// <summary>The bootstrap that finished Awake most recently. Read-only access point, not a settings bag.</summary>
        public static A11yBootstrap Active { get; private set; }

        public A11ySettingsStore Store { get; private set; }
        public AssistPolicy Policy { get; private set; }
        public InputAssistCore InputAssist { get; private set; }
        public FlashGuardCore FlashGuard { get; private set; }
        public CaptionBusCore Captions { get; private set; }
        public ColorTokens Colors { get; private set; }

        /// <summary>True when the OS reports bold text (mobile, Unity 6+). No registry key; UI may use a bolder weight.</summary>
        public bool OsBoldText { get; private set; }

        /// <summary>True when the OS screen reader is on (where Unity reports it). Narrator uses it to pick a route.</summary>
        public bool OsScreenReaderActive { get; private set; }

        /// <summary>Raised when <see cref="OsScreenReaderActive"/> changes.</summary>
        public event Action<bool> ScreenReaderStatusChanged;

        public string SettingsPath => Path.Combine(Application.persistentDataPath, FileName);

        private float _saveAt = -1f;

        private void Awake()
        {
            if (Active != null && Active != this)
            {
                Debug.LogWarning("[A11y] A second A11yBootstrap was found and disabled.", this);
                enabled = false;
                return;
            }
            Active = this;
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

            TextAsset asset = registry != null ? registry : Resources.Load<TextAsset>("settings_registry");
            if (asset == null)
            {
                Debug.LogError("[A11y] No settings registry found. Assign one or keep Resources/settings_registry.json.", this);
                return;
            }

            Store = A11ySettingsStore.FromRegistryJson(asset.text,
                onlyKeys != null && onlyKeys.Length > 0 ? onlyKeys : null);
            // Migration hook example: Store.Migration = (from, root) => { ... };  (see A11ySettingsStore.MigrationHandler)
            Load();
            ImportOsPreferences();

            Policy = new AssistPolicy(Store);
            InputAssist = new InputAssistCore(Store);
            FlashGuard = new FlashGuardCore(Store);
            Captions = new CaptionBusCore(Store);
            Colors = new ColorTokens(Store);

            Store.SettingChanged += OnSettingChanged;
        }

        private void OnDestroy()
        {
            if (Active != this) return;
            if (Store != null)
            {
                Store.SettingChanged -= OnSettingChanged;
                if (Store.IsDirty) Save();
            }
            InputAssist?.Dispose();
            Colors?.Dispose();
            UnsubscribeOsEvents();
            Active = null;
        }

        private void OnSettingChanged(string id, object value) => _saveAt = Time.unscaledTime + saveDelaySeconds;

        private void Update()
        {
            if (_saveAt >= 0f && Time.unscaledTime >= _saveAt)
            {
                _saveAt = -1f;
                Save();
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused && Store != null && Store.IsDirty) Save();
            if (paused) InputAssist?.ClearLatches();
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused) InputAssist?.ClearLatches();
        }

        private void OnApplicationQuit()
        {
            if (Store != null && Store.IsDirty) Save();
        }

        // ------------------------------------------------------------------ persistence

        /// <summary>Loads the settings file if present. Recovers from an interrupted save (temp file left behind).</summary>
        public void Load()
        {
            string path = SettingsPath;
            string tmp = path + ".tmp";
            try
            {
                if (!File.Exists(path) && File.Exists(tmp)) File.Move(tmp, path);
                if (!File.Exists(path)) return;
                Store.Deserialize(File.ReadAllText(path));
                foreach (string w in Store.LoadWarnings) Debug.LogWarning("[A11y] " + w);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[A11y] Could not read settings; defaults kept. " + e.Message);
            }
        }

        /// <summary>Writes the settings file: temp file first, then replace, so a crash never leaves a half-written file.</summary>
        public void Save()
        {
            if (Store == null) return;
            string path = SettingsPath;
            string tmp = path + ".tmp";
            try
            {
                File.WriteAllText(tmp, Store.Serialize());
                if (File.Exists(path))
                {
                    try
                    {
                        File.Replace(tmp, path, null);
                    }
                    catch (Exception)
                    {
                        // File.Replace is not available on every platform. Load() recovers the .tmp if we stop here.
                        File.Delete(path);
                        File.Move(tmp, path);
                    }
                }
                else
                {
                    File.Move(tmp, path);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[A11y] Could not save settings: " + e.Message);
            }
        }

        // ------------------------------------------------------------------ OS preferences

        /// <summary>
        /// Imports the OS preferences Unity exposes on this platform. Other prefs (reduce_motion, increase_contrast,
        /// mono_audio, caption_scale) have no Unity API; feed them from a native plugin with
        /// <c>Store.ImportPlatformPreference(pref, value)</c>.
        /// </summary>
        public void ImportOsPreferences()
        {
#if GAME_A11Y_ACCESSIBILITY_MODULE && UNITY_6000_0_OR_NEWER && (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            // UnityEngine.Accessibility.AccessibilitySettings: documented for Android and iOS only.
            Store.ImportPlatformPreference("text_scale", AccessibilitySettings.fontScale);
            Store.ImportPlatformPreference("captions_enabled", AccessibilitySettings.isClosedCaptioningEnabled);
            OsBoldText = AccessibilitySettings.isBoldTextEnabled;
            AccessibilitySettings.fontScaleChanged += OnOsFontScaleChanged;
            AccessibilitySettings.closedCaptioningStatusChanged += OnOsCaptionsChanged;
            AccessibilitySettings.boldTextStatusChanged += OnOsBoldTextChanged;
#endif
#if GAME_A11Y_ACCESSIBILITY_MODULE && !UNITY_EDITOR && ((UNITY_6000_0_OR_NEWER && (UNITY_ANDROID || UNITY_IOS)) || (UNITY_6000_3_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)))
            // AssistiveSupport: Android 8+/iOS from 6000.0; Windows/macOS players listed from 6000.3.
            OsScreenReaderActive = AssistiveSupport.isScreenReaderEnabled;
            Store.ImportPlatformPreference("screen_reader_active", OsScreenReaderActive);
            AssistiveSupport.screenReaderStatusChanged += OnOsScreenReaderChanged;
#endif
        }

        private void UnsubscribeOsEvents()
        {
#if GAME_A11Y_ACCESSIBILITY_MODULE && UNITY_6000_0_OR_NEWER && (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            AccessibilitySettings.fontScaleChanged -= OnOsFontScaleChanged;
            AccessibilitySettings.closedCaptioningStatusChanged -= OnOsCaptionsChanged;
            AccessibilitySettings.boldTextStatusChanged -= OnOsBoldTextChanged;
#endif
#if GAME_A11Y_ACCESSIBILITY_MODULE && !UNITY_EDITOR && ((UNITY_6000_0_OR_NEWER && (UNITY_ANDROID || UNITY_IOS)) || (UNITY_6000_3_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)))
            AssistiveSupport.screenReaderStatusChanged -= OnOsScreenReaderChanged;
#endif
        }

        // Handlers are always compiled so the event wiring above stays simple; they are tiny.
        private void OnOsFontScaleChanged(float scale) => Store?.ImportPlatformPreference("text_scale", scale);
        private void OnOsCaptionsChanged(bool on) => Store?.ImportPlatformPreference("captions_enabled", on);
        private void OnOsBoldTextChanged(bool on) => OsBoldText = on;

        private void OnOsScreenReaderChanged(bool on)
        {
            OsScreenReaderActive = on;
            Store?.ImportPlatformPreference("screen_reader_active", on);
            ScreenReaderStatusChanged?.Invoke(on);
        }

        /// <summary>Finds the store for a component: an explicit reference first, then <see cref="Active"/>.</summary>
        public static A11yBootstrap Resolve(A11yBootstrap explicitReference)
        {
            if (explicitReference != null) return explicitReference;
            if (Active == null)
                Debug.LogError("[A11y] No active A11yBootstrap. Add one to the boot scene before other accessibility components.");
            return Active;
        }
    }
}
