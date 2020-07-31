using System.Collections;
using System.Text;
using Sigobase.Database;

namespace Sigobase.Implements {
    // TODO options (indent, flagImplicit, flagFrozen, keyPath, keyQuote, stringQuote, comma)
    public static class ImplToString {
        private static StringBuilder WriteLineIndent(StringBuilder sb, int indent, int indentLevel) {
            sb.AppendLine();
            return sb.Append(' ', indentLevel * indent);
        }

        private static StringBuilder WriteColon(StringBuilder sb, int indent) {
            sb.Append(':');
            if (indent > 0) {
                sb.Append(' ');
            }

            return sb;
        }

        private static StringBuilder WriteComma(StringBuilder sb, int indent) {
            if (indent == 0) {
                sb.Append(',');
            }

            return sb;
        }

        private static StringBuilder WriteString(StringBuilder sb, string s) {
            sb.Append('"');
            foreach (var c in s)
                if (c >= ' ')
                    switch (c) {
                        case '\"':
                            sb.Append(@"\""");
                            break;
                        case '\\':
                            sb.Append(@"\\");
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                else
                    switch (c) {
                        case '\b':
                            sb.Append(@"\b");
                            break;
                        case '\f':
                            sb.Append(@"\f");
                            break;
                        case '\r':
                            sb.Append(@"\r");
                            break;
                        case '\n':
                            sb.Append(@"\n");
                            break;
                        case '\t':
                            sb.Append(@"\t");
                            break;
                        default: {
                            char Hex(int n) {
                                return n < 10 ? (char) ('0' + n) : (char) ('a' + (n - 10));
                            }

                            sb.Append(@"\u00");
                            sb.Append(Hex(c / 16));
                            sb.Append(Hex(c % 16));
                            break;
                        }
                    }

            return sb.Append('"');
        }

        // TODO move this to Utils and write some tests for this
        private static bool IsIdentifierOrInteger(string key) {
            if (string.IsNullOrEmpty(key)) return false;
            var c = key[0];

            if (c == '0') {
                return key.Length == 1;
            } else if (c >= '1' && c <= '9') {
                for (var i = 1; i < key.Length; i++) {
                    c = key[i];
                    if (c < '0' || c > '9') {
                        return false;
                    }
                }

                return true;
            } else if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_') {
                for (var i = 1; i < key.Length; i++) {
                    c = key[i];
                    if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '_' && (c < '0' || c > '9')) {
                        return false;
                    }
                }

                return true;
            } else {
                return false;
            }
        }

        private static StringBuilder WriteKey(StringBuilder sb, string key) {
            if (IsIdentifierOrInteger(key)) {
                return sb.Append(key);
            } else {
                return WriteString(sb, key);
            }
        }

        // TODO do we need detect circular object?
        private static StringBuilder WriteSigo(StringBuilder sb, ISigo sigo, int indent, int indentLevel) {
            if (sigo.IsLeaf()) {
                return WriteAny(sb, sigo.Data, indent, indentLevel);
            }

            if (sigo.Count == 0) indent = 0;
            sb.Append('{');
#if TESTMODE
            if (0 == (sigo.Flags & 16)) sb.Append('-');
#endif
            sb.Append(sigo.Flags & 7);

            foreach (var e in sigo) {
                WriteComma(sb, indent);
                if (indent > 0) WriteLineIndent(sb, indent, indentLevel + 1);

                WriteKey(sb, e.Key);

                WriteColon(sb, indent);
                WriteSigo(sb, e.Value, indent, indentLevel + 1);
            }

            if (indent > 0) WriteLineIndent(sb, indent, indentLevel);
            sb.Append('}');
            return sb;
        }

        // TODO why parentheses?
        private static StringBuilder WriteObject(StringBuilder sb, IDictionary dict, int indent, int indentLevel) {
            if (dict.Count == 0) indent = 0;
            sb.Append('(');
            sb.Append('{');
            var first = true;

            foreach (DictionaryEntry e in dict) {
                if (first) first = false;
                else WriteComma(sb, indent);
                if (indent > 0) WriteLineIndent(sb, indent, indentLevel + 1);

                WriteKey(sb, e.Key.ToString());

                WriteColon(sb, indent);

                WriteAny(sb, e.Value, indent, indentLevel + 1);
            }

            if (indent > 0) WriteLineIndent(sb, indent, indentLevel);
            sb.Append('}');
            sb.Append(')');
            return sb;
        }

        // TODO why parentheses?
        private static StringBuilder WriteArray(StringBuilder sb, IEnumerable list, int indent, int indentLevel) {
            if (list is ICollection col && col.Count == 0) indent = 0;
            sb.Append('(');
            sb.Append('[');
            var first = true;
            foreach (var e in list) {
                if (first) first = false;
                else WriteComma(sb, indent);

                if (indent > 0)
                    WriteLineIndent(sb, indent, indentLevel + 1);

                WriteAny(sb, e, indent, indentLevel + 1);
            }

            if (indent > 0) WriteLineIndent(sb, indent, indentLevel);
            sb.Append(']');
            sb.Append(')');
            return sb;
        }

        private static StringBuilder WriteAny(StringBuilder sb, object o, int indent, int indentLevel) {
            switch (o) {
                case null: return sb.Append("null");
                case bool b: return sb.Append(b ? "true" : "false");
                case string s: return WriteString(sb, s);
                // FIXME sigo tree inside sigo leaf
                case ISigo sigo: return WriteSigo(sb, sigo, indent, indentLevel);
                case IDictionary dict: return WriteObject(sb, dict, indent, indentLevel);
                case IEnumerable list: return WriteArray(sb, list, indent, indentLevel);
                default: return sb.Append(o);
            }
        }

        public static string ToString(ISigo sigo, int indent) {
            return WriteSigo(new StringBuilder(), sigo, indent, 0).ToString();
        }
    }
}