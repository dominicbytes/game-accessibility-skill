// Game Accessibility sample: semantic colour tokens with a paired symbol, swapped per visual.color_profile.
// Pure C# (netstandard2.1). No UnityEngine references (the Unity side converts Rgba to UnityEngine.Color).
//
// Contract: references/architecture.md section 9.
// - Scripts use tokens ("team.enemy"), never literal colours.
// - Every token has a symbol id that is ALWAYS shown, in every profile.
// - The profile swaps the palette per element. A full-screen filter is optional extra at most.
// - The "custom" profile reads the player's colours from visual.custom_colors ("role=#RRGGBB;role=#RRGGBB").
// The sample palettes are starting points, not validated sets. Check contrast with scripts/contrast_check.py
// on real backgrounds and test with players who use each profile.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace GameAccessibility
{
    /// <summary>Colour as linear-agnostic 0..1 floats (sRGB-encoded values when built from hex).</summary>
    public readonly struct Rgba : IEquatable<Rgba>
    {
        public readonly float R, G, B, A;

        public Rgba(float r, float g, float b, float a = 1f) { R = r; G = g; B = b; A = a; }

        /// <summary>Parses "#RRGGBB" or "#RRGGBBAA" (the # is optional).</summary>
        public static Rgba FromHex(string hex)
        {
            if (hex == null) throw new ArgumentNullException(nameof(hex));
            string h = hex.StartsWith("#", StringComparison.Ordinal) ? hex.Substring(1) : hex;
            if (h.Length != 6 && h.Length != 8) throw new FormatException($"Bad colour '{hex}'.");
            float Channel(int i) => int.Parse(h.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture) / 255f;
            return new Rgba(Channel(0), Channel(2), Channel(4), h.Length == 8 ? Channel(6) : 1f);
        }

        public bool Equals(Rgba o) => R == o.R && G == o.G && B == o.B && A == o.A;
        public override bool Equals(object obj) => obj is Rgba o && Equals(o);
        public override int GetHashCode() => (R, G, B, A).GetHashCode();
        public override string ToString() => $"Rgba({R:0.###}, {G:0.###}, {B:0.###}, {A:0.###})";

        /// <summary>"#RRGGBB" (alpha dropped), as stored in visual.custom_colors.</summary>
        public string ToHex()
        {
            int Byte(float v) => (int)Math.Round(Math.Max(0f, Math.Min(1f, v)) * 255f);
            return "#" + Byte(R).ToString("X2", CultureInfo.InvariantCulture) + Byte(G).ToString("X2", CultureInfo.InvariantCulture)
                + Byte(B).ToString("X2", CultureInfo.InvariantCulture);
        }
    }

    public sealed class ColorTokens : IDisposable
    {
        /// <summary>Profile ids, matching visual.color_profile values.</summary>
        public static readonly string[] Profiles = { "none", "protan", "deutan", "tritan", "custom" };

        private sealed class Token
        {
            public string Symbol;
            public readonly Dictionary<string, Rgba> ByProfile = new Dictionary<string, Rgba>(StringComparer.Ordinal);
        }

        private readonly A11ySettingsStore _store;
        private readonly Dictionary<string, Token> _tokens = new Dictionary<string, Token>(StringComparer.Ordinal);
        private readonly Dictionary<string, Rgba> _custom = new Dictionary<string, Rgba>(StringComparer.Ordinal);
        private readonly IDisposable _subscription;
        private readonly IDisposable _customSubscription;

        public const string CustomColorsKey = "visual.custom_colors";

        /// <summary>Raised when the active profile or a custom colour changes. Re-read every token you display.</summary>
        public event Action Changed;

        public string ActiveProfile { get; private set; } = "none";

        public ColorTokens(A11ySettingsStore store, bool withSamplePalette = true)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            if (withSamplePalette) DefineSamplePalette();
            _subscription = _store.Observe("visual.color_profile", v =>
            {
                ActiveProfile = v as string ?? "none";
                Changed?.Invoke();
            });
            _customSubscription = _store.Observe(CustomColorsKey, v =>
            {
                _custom.Clear();
                foreach (var kv in ParseCustomColors(v as string)) _custom[kv.Key] = kv.Value;
                if (ActiveProfile == "custom") Changed?.Invoke();
            });
        }

        public void Dispose()
        {
            _subscription.Dispose();
            _customSubscription.Dispose();
        }

        /// <summary>
        /// Parses "role=#RRGGBB;role=#RRGGBB". Whitespace is trimmed; malformed pairs are skipped so one typo in the
        /// config file never discards the other colours.
        /// </summary>
        public static Dictionary<string, Rgba> ParseCustomColors(string value)
        {
            var result = new Dictionary<string, Rgba>(StringComparer.Ordinal);
            if (string.IsNullOrWhiteSpace(value)) return result;
            foreach (string pair in value.Split(';'))
            {
                int eq = pair.IndexOf('=');
                if (eq <= 0) continue;
                string role = pair.Substring(0, eq).Trim();
                string hex = pair.Substring(eq + 1).Trim();
                if (role.Length == 0) continue;
                try { result[role] = Rgba.FromHex(hex); }
                catch (FormatException) { }
            }
            return result;
        }

        /// <summary>Formats colours for visual.custom_colors, sorted by role.</summary>
        public static string FormatCustomColors(IDictionary<string, Rgba> colours)
        {
            var roles = new List<string>(colours.Keys);
            roles.Sort(StringComparer.Ordinal);
            var parts = new List<string>(roles.Count);
            foreach (string role in roles) parts.Add(role + "=" + colours[role].ToHex());
            return string.Join(";", parts);
        }

        /// <summary>Defines or replaces a token. Profiles left null fall back to <paramref name="none"/>.</summary>
        public void Define(string token, string symbol, Rgba none, Rgba? protan = null, Rgba? deutan = null, Rgba? tritan = null)
        {
            if (string.IsNullOrEmpty(symbol)) throw new ArgumentException("Every token needs a symbol.", nameof(symbol));
            var t = new Token { Symbol = symbol };
            t.ByProfile["none"] = none;
            if (protan.HasValue) t.ByProfile["protan"] = protan.Value;
            if (deutan.HasValue) t.ByProfile["deutan"] = deutan.Value;
            if (tritan.HasValue) t.ByProfile["tritan"] = tritan.Value;
            _tokens[token] = t;
        }

        /// <summary>
        /// Player-chosen colour for the "custom" profile. Written to visual.custom_colors (an explicit player choice,
        /// persisted with the other settings). Without that key the colour is kept for this session only.
        /// </summary>
        public void SetCustom(string token, Rgba colour)
        {
            Find(token);
            var next = new Dictionary<string, Rgba>(_custom, StringComparer.Ordinal) { [token] = colour };
            if (_store.Has(CustomColorsKey))
            {
                _store.Set(CustomColorsKey, FormatCustomColors(next)); // the observer reparses and raises Changed
                return;
            }
            _custom[token] = colour;
            if (ActiveProfile == "custom") Changed?.Invoke();
        }

        public bool Has(string token) => _tokens.ContainsKey(token);

        /// <summary>Colour for <paramref name="token"/> in the active profile (falls back to "none").</summary>
        public Rgba Get(string token)
        {
            var t = Find(token);
            if (ActiveProfile == "custom") return _custom.TryGetValue(token, out Rgba mine) ? mine : t.ByProfile["none"];
            return t.ByProfile.TryGetValue(ActiveProfile, out Rgba c) ? c : t.ByProfile["none"];
        }

        /// <summary>Symbol id that must be shown with the colour (map it to a sprite, glyph or pattern).</summary>
        public string Symbol(string token) => Find(token).Symbol;

        public IEnumerable<string> TokenIds => _tokens.Keys;

        private Token Find(string token) =>
            _tokens.TryGetValue(token, out var t) ? t : throw new KeyNotFoundException($"Unknown colour token '{token}'.");

        /// <summary>
        /// Sample palette. "none" is a typical designer palette. protan/deutan use blue/orange/yellow contrasts from
        /// the Okabe-Ito set; tritan avoids blue/yellow pairs. Replace with your game's tokens and verify.
        /// </summary>
        private void DefineSamplePalette()
        {
            Rgba H(string hex) => Rgba.FromHex(hex);
            Define("team.ally", "circle", H("#43A047"), H("#0072B2"), H("#0072B2"), H("#009E73"));
            Define("team.enemy", "diamond", H("#E53935"), H("#D55E00"), H("#D55E00"), H("#D55E00"));
            Define("hazard", "warning_triangle", H("#FFB300"), H("#F0E442"), H("#F0E442"), H("#CC79A7"));
            Define("pickup.rare", "star", H("#8E24AA"), H("#CC79A7"), H("#CC79A7"), H("#FFFFFF"));
            Define("objective", "chevron", H("#1E88E5"), H("#56B4E9"), H("#56B4E9"), H("#E69F00"));
            Define("status.good", "check", H("#2E7D32"), H("#0072B2"), H("#0072B2"), H("#009E73"));
            Define("status.bad", "cross", H("#C62828"), H("#D55E00"), H("#D55E00"), H("#D55E00"));
        }
    }
}
