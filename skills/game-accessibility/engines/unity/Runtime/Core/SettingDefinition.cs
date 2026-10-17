// Game Accessibility sample: one registry entry and one preset, parsed from settings_registry.json.
// Pure C# (netstandard2.1). No UnityEngine references.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace GameAccessibility
{
    /// <summary>Value type of a registry setting. Stored values are bool, int, double or string.</summary>
    public enum SettingType { Bool, Int, Float, Enum, String }

    /// <summary>One entry from <c>settings-registry.json</c>. Immutable after load.</summary>
    public sealed class SettingDefinition
    {
        public string Id { get; }
        public string Category { get; }
        public SettingType Type { get; }
        /// <summary>Default value, already normalised: bool, int, double or string.</summary>
        public object Default { get; }
        public double? Min { get; }
        public double? Max { get; }
        /// <summary>UI step for sliders. Not enforced by <see cref="TryNormalize"/>.</summary>
        public double? Step { get; }
        public IReadOnlyList<string> Values { get; }
        public string Label { get; }
        public string Description { get; }
        /// <summary>"profile" follows the player; "device" stays with the machine.</summary>
        public string Scope { get; }
        public string Apply { get; }
        /// <summary>OS preference this key may take its initial value from, or null.</summary>
        public string PlatformPref { get; }

        public bool IsDeviceScope => string.Equals(Scope, "device", StringComparison.Ordinal);

        private SettingDefinition(string id, string category, SettingType type, double? min, double? max,
            double? step, IReadOnlyList<string> values, string label, string description, string scope,
            string apply, string platformPref, object rawDefault)
        {
            Id = id;
            Category = category;
            Type = type;
            Min = min;
            Max = max;
            Step = step;
            Values = values;
            Label = label;
            Description = description;
            Scope = scope;
            Apply = apply;
            PlatformPref = platformPref;
            if (!TryNormalize(rawDefault, out object normalised))
                throw new FormatException($"Registry setting '{id}' has an illegal default.");
            Default = normalised;
        }

        /// <summary>
        /// Validates <paramref name="raw"/> for this setting and clamps numbers into range.
        /// Accepts any numeric CLR type for int/float. Rejects NaN, infinity, wrong types and unknown enum values.
        /// </summary>
        public bool TryNormalize(object raw, out object value)
        {
            value = null;
            switch (Type)
            {
                case SettingType.Bool:
                    if (raw is bool b) { value = b; return true; }
                    return false;

                case SettingType.Int:
                    if (!TryGetNumber(raw, out double di)) return false;
                    di = Clamp(Math.Round(di, MidpointRounding.AwayFromZero));
                    if (di < int.MinValue || di > int.MaxValue) return false;
                    value = (int)di;
                    return true;

                case SettingType.Float:
                    if (!TryGetNumber(raw, out double df)) return false;
                    value = Clamp(df);
                    return true;

                case SettingType.Enum:
                    if (raw is string e && Values != null)
                    {
                        for (int i = 0; i < Values.Count; i++)
                        {
                            if (string.Equals(Values[i], e, StringComparison.Ordinal)) { value = e; return true; }
                        }
                    }
                    return false;

                case SettingType.String:
                    if (raw is string s) { value = s; return true; }
                    return false;
            }
            return false;
        }

        private double Clamp(double v)
        {
            if (Min.HasValue && v < Min.Value) v = Min.Value;
            if (Max.HasValue && v > Max.Value) v = Max.Value;
            return v;
        }

        private static bool TryGetNumber(object raw, out double d)
        {
            d = 0;
            switch (raw)
            {
                case null:
                case bool _:
                case string _:
                    return false;
                case double x: d = x; break;
                case float x: d = x; break;
                case int x: d = x; break;
                case long x: d = x; break;
                case IConvertible c:
                    try { d = c.ToDouble(CultureInfo.InvariantCulture); }
                    catch (Exception) { return false; }
                    break;
                default:
                    return false;
            }
            return !double.IsNaN(d) && !double.IsInfinity(d);
        }

        /// <summary>Builds a definition from a parsed registry object. Throws <see cref="FormatException"/> if invalid.</summary>
        public static SettingDefinition FromJson(IDictionary<string, object> o)
        {
            string id = Str(o, "id") ?? throw new FormatException("Registry setting without 'id'.");
            SettingType type;
            switch (Str(o, "type"))
            {
                case "bool": type = SettingType.Bool; break;
                case "int": type = SettingType.Int; break;
                case "float": type = SettingType.Float; break;
                case "enum": type = SettingType.Enum; break;
                case "string": type = SettingType.String; break;
                default: throw new FormatException($"Registry setting '{id}' has unknown type.");
            }

            List<string> values = null;
            if (o.TryGetValue("values", out object rawValues) && rawValues is List<object> list)
            {
                values = new List<string>(list.Count);
                foreach (object v in list)
                {
                    if (v is string s) values.Add(s);
                }
            }

            if (!o.TryGetValue("default", out object def))
                throw new FormatException($"Registry setting '{id}' has no default.");

            return new SettingDefinition(
                id, Str(o, "category") ?? "", type, Num(o, "min"), Num(o, "max"), Num(o, "step"),
                values, Str(o, "label") ?? id, Str(o, "description") ?? "", Str(o, "scope") ?? "profile",
                Str(o, "apply") ?? "live", Str(o, "platform_pref"), def);
        }

        internal static string Str(IDictionary<string, object> o, string key) =>
            o.TryGetValue(key, out object v) ? v as string : null;

        private static double? Num(IDictionary<string, object> o, string key) =>
            o.TryGetValue(key, out object v) && v is double d ? d : (double?)null;

        public override string ToString() => $"{Id} ({Type})";
    }

    /// <summary>
    /// An editable patch of setting values ("Readable text", "More time to respond"...). Applying it is
    /// one undoable batch. Presets are named by what they do, never by diagnosis.
    /// </summary>
    public sealed class PresetDefinition
    {
        public string Id { get; }
        public string Label { get; }
        /// <summary>Exactly what the preset changes. Show this list to the player before applying.</summary>
        public IReadOnlyDictionary<string, object> Changes { get; }

        public PresetDefinition(string id, string label, IReadOnlyDictionary<string, object> changes)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Label = label ?? id;
            Changes = changes ?? new Dictionary<string, object>();
        }

        public static PresetDefinition FromJson(IDictionary<string, object> o)
        {
            string id = SettingDefinition.Str(o, "id") ?? throw new FormatException("Preset without 'id'.");
            var changes = new Dictionary<string, object>(StringComparer.Ordinal);
            if (o.TryGetValue("changes", out object raw) && raw is Dictionary<string, object> dict)
            {
                foreach (var kv in dict) changes[kv.Key] = kv.Value;
            }
            return new PresetDefinition(id, SettingDefinition.Str(o, "label"), changes);
        }
    }
}
