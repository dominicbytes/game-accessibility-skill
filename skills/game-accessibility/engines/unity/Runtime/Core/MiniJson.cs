// Game Accessibility sample: small JSON reader/writer for the settings registry and settings file.
// Pure C# (netstandard2.1). No UnityEngine references.
// Parsed shapes: objects -> Dictionary<string, object>, arrays -> List<object>, numbers -> double,
// strings -> string, true/false -> bool, null -> null.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GameAccessibility
{
    /// <summary>Minimal JSON parser and pretty-printing serializer. Not a general-purpose library.</summary>
    public static class MiniJson
    {
        /// <summary>Maximum nesting depth accepted by <see cref="Parse"/>. Protects against hostile files.</summary>
        public const int MaxDepth = 64;

        /// <summary>Parses JSON text. Throws <see cref="FormatException"/> on invalid input.</summary>
        public static object Parse(string json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            var parser = new Parser(json);
            object value = parser.ParseValue(0);
            parser.SkipWhitespace();
            if (!parser.AtEnd) throw parser.Error("Unexpected trailing characters");
            return value;
        }

        /// <summary>Parses JSON text without throwing. Returns false and an error message on failure.</summary>
        public static bool TryParse(string json, out object value, out string error)
        {
            try
            {
                value = Parse(json);
                error = null;
                return true;
            }
            catch (FormatException e)
            {
                value = null;
                error = e.Message;
                return false;
            }
        }

        /// <summary>Serializes dictionaries, lists, strings, bools, numbers and null as indented JSON.</summary>
        public static string Serialize(object value, bool pretty = true)
        {
            var sb = new StringBuilder();
            Write(sb, value, pretty, 0);
            return sb.ToString();
        }

        // ------------------------------------------------------------------ parser

        private sealed class Parser
        {
            private readonly string _s;
            private int _i;

            public Parser(string s) { _s = s; }

            public bool AtEnd => _i >= _s.Length;

            public FormatException Error(string message) =>
                new FormatException($"{message} at character {_i}.");

            public void SkipWhitespace()
            {
                while (_i < _s.Length)
                {
                    char c = _s[_i];
                    if (c == ' ' || c == '\t' || c == '\n' || c == '\r') _i++;
                    else break;
                }
            }

            public object ParseValue(int depth)
            {
                if (depth > MaxDepth) throw Error("JSON nested too deeply");
                SkipWhitespace();
                if (AtEnd) throw Error("Unexpected end of input");
                char c = _s[_i];
                switch (c)
                {
                    case '{': return ParseObject(depth);
                    case '[': return ParseArray(depth);
                    case '"': return ParseString();
                    case 't': Expect("true"); return true;
                    case 'f': Expect("false"); return false;
                    case 'n': Expect("null"); return null;
                    default:
                        if (c == '-' || (c >= '0' && c <= '9')) return ParseNumber();
                        throw Error($"Unexpected character '{c}'");
                }
            }

            private void Expect(string word)
            {
                if (string.CompareOrdinal(_s, _i, word, 0, word.Length) != 0) throw Error($"Expected '{word}'");
                _i += word.Length;
            }

            private Dictionary<string, object> ParseObject(int depth)
            {
                var result = new Dictionary<string, object>(StringComparer.Ordinal);
                _i++; // {
                SkipWhitespace();
                if (!AtEnd && _s[_i] == '}') { _i++; return result; }
                while (true)
                {
                    SkipWhitespace();
                    if (AtEnd || _s[_i] != '"') throw Error("Expected object key");
                    string key = ParseString();
                    SkipWhitespace();
                    if (AtEnd || _s[_i] != ':') throw Error("Expected ':'");
                    _i++;
                    result[key] = ParseValue(depth + 1); // last duplicate wins
                    SkipWhitespace();
                    if (AtEnd) throw Error("Unterminated object");
                    char c = _s[_i++];
                    if (c == '}') return result;
                    if (c != ',') throw Error("Expected ',' or '}'");
                }
            }

            private List<object> ParseArray(int depth)
            {
                var result = new List<object>();
                _i++; // [
                SkipWhitespace();
                if (!AtEnd && _s[_i] == ']') { _i++; return result; }
                while (true)
                {
                    result.Add(ParseValue(depth + 1));
                    SkipWhitespace();
                    if (AtEnd) throw Error("Unterminated array");
                    char c = _s[_i++];
                    if (c == ']') return result;
                    if (c != ',') throw Error("Expected ',' or ']'");
                }
            }

            private string ParseString()
            {
                _i++; // opening quote
                var sb = new StringBuilder();
                while (true)
                {
                    if (AtEnd) throw Error("Unterminated string");
                    char c = _s[_i++];
                    if (c == '"') return sb.ToString();
                    if (c < 0x20) throw Error("Control character in string");
                    if (c != '\\') { sb.Append(c); continue; }
                    if (AtEnd) throw Error("Unterminated escape");
                    char e = _s[_i++];
                    switch (e)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            if (_i + 4 > _s.Length) throw Error("Truncated \\u escape");
                            if (!ushort.TryParse(_s.Substring(_i, 4), NumberStyles.HexNumber,
                                    CultureInfo.InvariantCulture, out ushort code))
                                throw Error("Invalid \\u escape");
                            sb.Append((char)code); // surrogate pairs arrive as two escapes and stay paired
                            _i += 4;
                            break;
                        default: throw Error($"Invalid escape '\\{e}'");
                    }
                }
            }

            private double ParseNumber()
            {
                int start = _i;
                if (_s[_i] == '-') _i++;
                while (_i < _s.Length)
                {
                    char c = _s[_i];
                    if ((c >= '0' && c <= '9') || c == '.' || c == 'e' || c == 'E' || c == '+' || c == '-') _i++;
                    else break;
                }
                string token = _s.Substring(start, _i - start);
                if (!double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double d)
                    || double.IsNaN(d) || double.IsInfinity(d))
                    throw Error($"Invalid number '{token}'");
                return d;
            }
        }

        // ------------------------------------------------------------------ writer

        private static void Write(StringBuilder sb, object value, bool pretty, int indent)
        {
            switch (value)
            {
                case null: sb.Append("null"); return;
                case string s: WriteString(sb, s); return;
                case bool b: sb.Append(b ? "true" : "false"); return;
                case double d: WriteDouble(sb, d); return;
                case float f: WriteDouble(sb, f); return;
                case int i: sb.Append(i.ToString(CultureInfo.InvariantCulture)); return;
                case long l: sb.Append(l.ToString(CultureInfo.InvariantCulture)); return;
                case IDictionary dict: WriteObject(sb, dict, pretty, indent); return;
                case IEnumerable list: WriteArray(sb, list, pretty, indent); return;
                case IConvertible conv:
                    WriteDouble(sb, conv.ToDouble(CultureInfo.InvariantCulture));
                    return;
                default:
                    WriteString(sb, value.ToString());
                    return;
            }
        }

        private static void WriteDouble(StringBuilder sb, double d)
        {
            // JSON has no NaN/Infinity. The settings store never holds them; write null defensively.
            if (double.IsNaN(d) || double.IsInfinity(d)) { sb.Append("null"); return; }
            sb.Append(d.ToString("R", CultureInfo.InvariantCulture));
        }

        private static void WriteObject(StringBuilder sb, IDictionary dict, bool pretty, int indent)
        {
            if (dict.Count == 0) { sb.Append("{}"); return; }
            // Sort keys so the file diffs cleanly and is easy to read by hand.
            var keys = new List<string>();
            foreach (object k in dict.Keys) keys.Add(Convert.ToString(k, CultureInfo.InvariantCulture));
            keys.Sort(StringComparer.Ordinal);
            sb.Append('{');
            for (int n = 0; n < keys.Count; n++)
            {
                if (n > 0) sb.Append(',');
                NewLine(sb, pretty, indent + 1);
                WriteString(sb, keys[n]);
                sb.Append(pretty ? ": " : ":");
                Write(sb, dict[keys[n]], pretty, indent + 1);
            }
            NewLine(sb, pretty, indent);
            sb.Append('}');
        }

        private static void WriteArray(StringBuilder sb, IEnumerable list, bool pretty, int indent)
        {
            sb.Append('[');
            bool first = true;
            foreach (object item in list)
            {
                if (!first) sb.Append(',');
                first = false;
                NewLine(sb, pretty, indent + 1);
                Write(sb, item, pretty, indent + 1);
            }
            if (!first) NewLine(sb, pretty, indent);
            sb.Append(']');
        }

        private static void NewLine(StringBuilder sb, bool pretty, int indent)
        {
            if (!pretty) return;
            sb.Append('\n');
            sb.Append(' ', indent * 2);
        }

        private static void WriteString(StringBuilder sb, string s)
        {
            sb.Append('"');
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    default:
                        if (c < 0x20) sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        else sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
        }
    }
}
