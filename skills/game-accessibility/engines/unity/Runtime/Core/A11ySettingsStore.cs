// Game Accessibility sample: the settings store. Single source of truth for accessibility settings.
// Pure C# (netstandard2.1). No UnityEngine references. Main-thread use only.
//
// Contract: references/architecture.md section 1.
// - Set() validates and clamps, changes only its own key and returns whether the value changed.
// - Every Set() by the player marks the key "explicit". OS preferences never override explicit keys.
// - Presets apply as one batch and can be undone.
// - The file stores only explicit choices, split into "profile" and "device" sections, with schema_version.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace GameAccessibility
{
    /// <summary>Registry-backed accessibility settings with validation, presets, OS-preference import and persistence.</summary>
    public sealed class A11ySettingsStore
    {
        /// <summary>
        /// Called by <see cref="Deserialize"/> when the file's schema_version is lower than <see cref="SchemaVersion"/>.
        /// Edit <c>root</c> in place: rename keys and convert values by meaning. Example (v0 to v1):
        /// <code>
        /// store.Migration = (from, root) => {
        ///     if (from &lt; 1 &amp;&amp; root.TryGetValue("shake_off", out var v) &amp;&amp; v is bool off) {
        ///         if (!root.TryGetValue("profile", out var p) || !(p is Dictionary&lt;string, object&gt; profile))
        ///             root["profile"] = profile = new Dictionary&lt;string, object&gt;();
        ///         if (off) profile["motion.camera_shake"] = 0.0;
        ///         root.Remove("shake_off");
        ///     }
        /// };
        /// </code>
        /// </summary>
        public delegate void MigrationHandler(int fromVersion, IDictionary<string, object> root);

        /// <summary>Raised once per changed key, after the value is stored. Value is bool, int, double or string.</summary>
        public event Action<string, object> SettingChanged;

        /// <summary>Raised after a preset or an undo finished applying (all SettingChanged events already raised).</summary>
        public event Action<string> PresetApplied;

        /// <summary>Optional migration hook. See <see cref="MigrationHandler"/>.</summary>
        public MigrationHandler Migration { get; set; }

        /// <summary>Version written to and expected from the settings file. Taken from the registry's schema_version.</summary>
        public int SchemaVersion { get; }

        /// <summary>True when values changed since the last <see cref="Serialize"/>. The Unity bootstrap uses this to save.</summary>
        public bool IsDirty { get; private set; }

        /// <summary>Problems found by the last <see cref="Deserialize"/> (repaired keys, unknown keys, newer file).</summary>
        public IReadOnlyList<string> LoadWarnings => _warnings;

        public IReadOnlyList<SettingDefinition> Definitions => _order;
        public IReadOnlyList<PresetDefinition> Presets => _presetOrder;
        public bool CanUndoPreset => _undo != null;

        private readonly Dictionary<string, SettingDefinition> _defs = new Dictionary<string, SettingDefinition>(StringComparer.Ordinal);
        private readonly List<SettingDefinition> _order = new List<SettingDefinition>();
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>(StringComparer.Ordinal);
        private readonly HashSet<string> _explicit = new HashSet<string>(StringComparer.Ordinal);
        private readonly Dictionary<string, object> _osInitial = new Dictionary<string, object>(StringComparer.Ordinal);
        private readonly Dictionary<string, object> _unknownFromFile = new Dictionary<string, object>(StringComparer.Ordinal);
        private readonly Dictionary<string, PresetDefinition> _presets = new Dictionary<string, PresetDefinition>(StringComparer.Ordinal);
        private readonly List<PresetDefinition> _presetOrder = new List<PresetDefinition>();
        private readonly List<string> _warnings = new List<string>();
        private PresetUndo _undo;

        private sealed class PresetUndo
        {
            public string PresetId;
            public readonly List<(string id, object before, bool wasExplicit, object applied)> Entries =
                new List<(string, object, bool, object)>();
        }

        // ------------------------------------------------------------------ construction

        public A11ySettingsStore(IEnumerable<SettingDefinition> definitions, IEnumerable<PresetDefinition> presets = null,
            int schemaVersion = 1)
        {
            if (definitions == null) throw new ArgumentNullException(nameof(definitions));
            SchemaVersion = schemaVersion;
            foreach (var d in definitions)
            {
                if (_defs.ContainsKey(d.Id)) throw new ArgumentException($"Duplicate setting id '{d.Id}'.");
                _defs.Add(d.Id, d);
                _order.Add(d);
                _values[d.Id] = d.Default;
            }
            if (presets == null) return;
            foreach (var p in presets)
            {
                // Keep only preset entries for keys this game registered, with legal values.
                var kept = new Dictionary<string, object>(StringComparer.Ordinal);
                foreach (var kv in p.Changes)
                {
                    if (_defs.TryGetValue(kv.Key, out var def) && def.TryNormalize(kv.Value, out object v)) kept[kv.Key] = v;
                }
                if (kept.Count == 0) continue;
                var filtered = new PresetDefinition(p.Id, p.Label, kept);
                _presets[p.Id] = filtered;
                _presetOrder.Add(filtered);
            }
        }

        /// <summary>
        /// Builds a store from registry JSON text. Pass <paramref name="onlyKeys"/> to register only the keys
        /// this game's signals require; null registers every key in the file.
        /// </summary>
        public static A11ySettingsStore FromRegistryJson(string json, ICollection<string> onlyKeys = null)
        {
            if (!(MiniJson.Parse(json) is Dictionary<string, object> root))
                throw new FormatException("Registry root must be a JSON object.");
            int version = root.TryGetValue("schema_version", out object sv) && sv is double dv ? (int)dv : 1;
            var defs = new List<SettingDefinition>();
            if (root.TryGetValue("settings", out object rawSettings) && rawSettings is List<object> settings)
            {
                foreach (object s in settings)
                {
                    if (!(s is Dictionary<string, object> so)) continue;
                    var def = SettingDefinition.FromJson(so);
                    if (onlyKeys == null || onlyKeys.Contains(def.Id)) defs.Add(def);
                }
            }
            var presets = new List<PresetDefinition>();
            if (root.TryGetValue("presets", out object rawPresets) && rawPresets is List<object> plist)
            {
                foreach (object p in plist)
                {
                    if (p is Dictionary<string, object> po) presets.Add(PresetDefinition.FromJson(po));
                }
            }
            return new A11ySettingsStore(defs, presets, version);
        }

        // ------------------------------------------------------------------ reading

        public bool Has(string id) => id != null && _defs.ContainsKey(id);

        public SettingDefinition GetDefinition(string id) =>
            _defs.TryGetValue(id, out var d) ? d : throw new KeyNotFoundException($"Unknown accessibility setting '{id}'.");

        /// <summary>Current value as stored (bool, int, double or string). Throws for unknown keys.</summary>
        public object GetRaw(string id)
        {
            if (!_values.TryGetValue(id, out object v)) throw new KeyNotFoundException($"Unknown accessibility setting '{id}'.");
            return v;
        }

        /// <summary>
        /// Typed read. Numeric settings convert to float, double or int. Enum settings read as string.
        /// Throws <see cref="KeyNotFoundException"/> for keys this game did not register.
        /// </summary>
        public T Get<T>(string id) => ConvertTo<T>(GetRaw(id));

        /// <summary>Typed read that returns <paramref name="fallback"/> when the game did not register the key.</summary>
        public T GetOr<T>(string id, T fallback) =>
            id != null && _values.TryGetValue(id, out object v) ? ConvertTo<T>(v) : fallback;

        public bool IsExplicit(string id) => _explicit.Contains(id);

        /// <summary>
        /// Resolves a per-effect override key with values "follow", "off" and "on" (for example visual.outline_enemies
        /// or motion.parallax). "on" returns true and "off" returns false. "follow", any other value, or a key the game
        /// did not register returns <paramref name="followValue"/>: the umbrella setting the override patches.
        /// A bool key returns its own value.
        /// </summary>
        public bool ResolveToggle(string key, bool followValue)
        {
            if (key == null || !_values.TryGetValue(key, out object v)) return followValue;
            if (v is bool b) return b;
            if (v is string s)
            {
                if (string.Equals(s, "on", StringComparison.Ordinal)) return true;
                if (string.Equals(s, "off", StringComparison.Ordinal)) return false;
            }
            return followValue;
        }

        private static T ConvertTo<T>(object v)
        {
            if (v is T t) return t;
            return (T)Convert.ChangeType(v, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Invokes <paramref name="onValue"/> with the current value now, then on every change of <paramref name="id"/>.
        /// Use this from objects that spawn late. Dispose the result in OnDisable/OnDestroy.
        /// Returns a no-op handle when the key is not registered.
        /// </summary>
        public IDisposable Observe(string id, Action<object> onValue)
        {
            if (onValue == null) throw new ArgumentNullException(nameof(onValue));
            if (!Has(id)) return new Subscription(null);
            void Handler(string changed, object value)
            {
                if (string.Equals(changed, id, StringComparison.Ordinal)) onValue(value);
            }
            SettingChanged += Handler;
            onValue(_values[id]);
            return new Subscription(() => SettingChanged -= Handler);
        }

        private sealed class Subscription : IDisposable
        {
            private Action _dispose;
            public Subscription(Action dispose) { _dispose = dispose; }
            public void Dispose() { _dispose?.Invoke(); _dispose = null; }
        }

        // ------------------------------------------------------------------ writing

        /// <summary>
        /// Player-initiated change. Validates and clamps, marks the key explicit, raises SettingChanged if
        /// the value changed. Returns false when the value is invalid or unchanged. Throws for unknown keys.
        /// </summary>
        public bool Set(string id, object value)
        {
            var def = GetDefinition(id);
            if (!def.TryNormalize(value, out object normalised)) return false;
            _explicit.Add(id);
            IsDirty = true; // explicit flag itself is persisted
            return Store(id, normalised, raise: true);
        }

        /// <summary>Returns <paramref name="id"/> to its initial value (OS preference if imported, else default) and clears the explicit flag.</summary>
        public bool Reset(string id)
        {
            GetDefinition(id);
            bool wasExplicit = _explicit.Remove(id);
            if (wasExplicit) IsDirty = true;
            return Store(id, InitialValue(id), raise: true);
        }

        /// <summary>Resets every key in <paramref name="category"/> (for example "motion"). Returns the number of changed values.</summary>
        public int ResetCategory(string category)
        {
            int changed = 0;
            foreach (var d in _order)
            {
                if (string.Equals(d.Category, category, StringComparison.Ordinal) && Reset(d.Id)) changed++;
            }
            return changed;
        }

        public int ResetAll()
        {
            int changed = 0;
            foreach (var d in _order)
            {
                if (Reset(d.Id)) changed++;
            }
            _undo = null;
            return changed;
        }

        private object InitialValue(string id) =>
            _osInitial.TryGetValue(id, out object os) ? os : _defs[id].Default;

        private bool Store(string id, object value, bool raise)
        {
            object old = _values[id];
            if (Equals(old, value)) return false;
            _values[id] = value;
            IsDirty = true;
            if (raise) SettingChanged?.Invoke(id, value);
            return true;
        }

        // ------------------------------------------------------------------ presets

        public bool TryGetPreset(string presetId, out PresetDefinition preset) => _presets.TryGetValue(presetId, out preset);

        /// <summary>
        /// Applies every change of the preset as one batch: all values are stored first, then SettingChanged is
        /// raised per changed key, then PresetApplied. Values become explicit choices. Undo with <see cref="UndoLastPreset"/>.
        /// </summary>
        public bool ApplyPreset(string presetId)
        {
            if (!_presets.TryGetValue(presetId, out var preset)) return false;
            var undo = new PresetUndo { PresetId = presetId };
            var changed = new List<string>();
            foreach (var kv in preset.Changes)
            {
                undo.Entries.Add((kv.Key, _values[kv.Key], _explicit.Contains(kv.Key), kv.Value));
                _explicit.Add(kv.Key);
                if (Store(kv.Key, kv.Value, raise: false)) changed.Add(kv.Key);
            }
            IsDirty = true;
            _undo = undo;
            foreach (string id in changed) SettingChanged?.Invoke(id, _values[id]);
            PresetApplied?.Invoke(presetId);
            return true;
        }

        /// <summary>
        /// Reverts the last preset. Keys the player changed by hand after applying it are left alone.
        /// Returns false when there is nothing to undo.
        /// </summary>
        public bool UndoLastPreset()
        {
            if (_undo == null) return false;
            var undo = _undo;
            _undo = null;
            var changed = new List<string>();
            foreach (var e in undo.Entries)
            {
                if (!Equals(_values[e.id], e.applied)) continue; // player changed it since: keep their choice
                if (e.wasExplicit) _explicit.Add(e.id);
                else _explicit.Remove(e.id);
                if (Store(e.id, e.before, raise: false)) changed.Add(e.id);
            }
            IsDirty = true;
            foreach (string id in changed) SettingChanged?.Invoke(id, _values[id]);
            PresetApplied?.Invoke("undo:" + undo.PresetId);
            return true;
        }

        // ------------------------------------------------------------------ OS preferences

        /// <summary>
        /// Records an OS accessibility preference as the initial value of every key whose <c>platform_pref</c>
        /// matches, and applies it to keys the player has not set explicitly. Call at boot and whenever the OS
        /// reports a change. Known prefs: reduce_motion, screen_reader_active, increase_contrast,
        /// captions_enabled, text_scale, caption_scale, mono_audio. Returns the number of changed values.
        /// </summary>
        public int ImportPlatformPreference(string pref, object osValue)
        {
            int changed = 0;
            foreach (var d in _order)
            {
                if (!string.Equals(d.PlatformPref, pref, StringComparison.Ordinal)) continue;
                object mapped = MapPlatformPreference(d, pref, osValue);
                if (mapped != null && d.TryNormalize(mapped, out object normalised)) _osInitial[d.Id] = normalised;
                else _osInitial.Remove(d.Id);
                if (_explicit.Contains(d.Id)) continue; // an explicit player choice always wins
                if (Store(d.Id, InitialValue(d.Id), raise: true)) changed++;
            }
            return changed;
        }

        /// <summary>
        /// Maps an OS preference to a setting value, or null for "no preference: use the designer default".
        /// A boolean OS preference that is off never overrides the designer default (subtitles stay on).
        /// </summary>
        public static object MapPlatformPreference(SettingDefinition def, string pref, object osValue)
        {
            switch (pref)
            {
                case "text_scale":
                case "caption_scale":
                    return osValue is IConvertible c && !(osValue is bool) && !(osValue is string)
                        ? c.ToDouble(CultureInfo.InvariantCulture)
                        : (object)null;
                case "reduce_motion":
                    if (!(osValue is bool reduce) || !reduce) return null;
                    if (def.Type == SettingType.Float) return def.Min ?? 0.0;    // shake, bob, weapon sway: off
                    // Bools flip away from their default: effects that default on (motion blur) turn off;
                    // reductions that default off (reduce_motion, reduce_flashing) turn on.
                    if (def.Type == SettingType.Bool && def.Default is bool d) return !d;
                    // "follow"/"off"/"on" overrides (motion.parallax, motion.auto_camera...) carry no platform_pref:
                    // they keep "follow" and so follow motion.reduce_motion, which this preference sets.
                    return null;
                default:
                    // screen_reader_active, increase_contrast, captions_enabled, mono_audio
                    if (osValue is bool on && on && def.Type == SettingType.Bool) return true;
                    return null;
            }
        }

        // ------------------------------------------------------------------ persistence

        /// <summary>
        /// Writes only explicit choices, so the file is a readable list of what the player changed.
        /// Shape: { "schema_version": N, "profile": { key: value }, "device": { key: value } }.
        /// Unknown keys read from a newer or modded file are written back unchanged.
        /// </summary>
        public string Serialize()
        {
            var profile = new Dictionary<string, object>(StringComparer.Ordinal);
            var device = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var kv in _unknownFromFile) profile[kv.Key] = kv.Value;
            foreach (var d in _order)
            {
                if (!_explicit.Contains(d.Id)) continue;
                (d.IsDeviceScope ? device : profile)[d.Id] = _values[d.Id];
            }
            var root = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["schema_version"] = SchemaVersion,
                ["profile"] = profile,
                ["device"] = device,
            };
            IsDirty = false;
            return MiniJson.Serialize(root);
        }

        /// <summary>
        /// Loads a settings file. Never throws for bad content: a corrupt file keeps the defaults, and an invalid
        /// value repairs only that key. Runs <see cref="Migration"/> for older files. Any key present in the file
        /// counts as an explicit choice (hand edits by players and tools are respected).
        /// Raises SettingChanged for values that differ from the current ones. Returns false if the file was unreadable.
        /// </summary>
        public bool Deserialize(string json)
        {
            _warnings.Clear();
            _unknownFromFile.Clear();
            if (string.IsNullOrWhiteSpace(json)) return false;
            if (!MiniJson.TryParse(json, out object parsed, out string error) || !(parsed is Dictionary<string, object> root))
            {
                _warnings.Add("Settings file unreadable; defaults kept. " + (error ?? "Root is not an object."));
                return false;
            }

            int fileVersion = root.TryGetValue("schema_version", out object sv) && sv is double dv ? (int)dv : 0;
            if (fileVersion < SchemaVersion && Migration != null)
            {
                try { Migration(fileVersion, root); }
                catch (Exception e) { _warnings.Add("Migration failed: " + e.Message); }
            }
            else if (fileVersion > SchemaVersion)
            {
                _warnings.Add($"Settings file version {fileVersion} is newer than {SchemaVersion}; known keys loaded.");
            }

            var incoming = new Dictionary<string, object>(StringComparer.Ordinal);
            CollectSection(root, "profile", incoming);
            CollectSection(root, "device", incoming);

            var changed = new List<string>();
            foreach (var kv in incoming)
            {
                if (!_defs.TryGetValue(kv.Key, out var def))
                {
                    _unknownFromFile[kv.Key] = kv.Value;
                    _warnings.Add($"Unknown key '{kv.Key}' kept in file but ignored.");
                    continue;
                }
                if (!def.TryNormalize(kv.Value, out object v))
                {
                    _warnings.Add($"Invalid value for '{kv.Key}'; using its initial value.");
                    continue; // per-key repair: the rest of the file still loads
                }
                _explicit.Add(kv.Key);
                if (Store(kv.Key, v, raise: false)) changed.Add(kv.Key);
            }
            foreach (string id in changed) SettingChanged?.Invoke(id, _values[id]);
            IsDirty = _warnings.Count > 0; // rewrite a repaired file on next save
            return true;
        }

        private static void CollectSection(Dictionary<string, object> root, string name, Dictionary<string, object> into)
        {
            if (!root.TryGetValue(name, out object raw) || !(raw is Dictionary<string, object> section)) return;
            foreach (var kv in section) into[kv.Key] = kv.Value;
        }
    }
}
