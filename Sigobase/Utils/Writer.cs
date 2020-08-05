using System.Collections;
using System.Linq;
using System.Text;
using Sigobase.Database;

namespace Sigobase.Utils {
    public class Writer {
        public static Writer Json = new Writer (
            ignoreFlag: true,
            indent: "",
            keyQuoted: true,
            colon: ":",
            comma: ",",
            trailComma: false,
            quote: '"'
        );

        public static Writer Js = new Writer(Json) {
            KeyQuoted = false,
            Quote = '\''
        };

        public static Writer CSharp = new Writer(
            ignoreFlag: true,
            indent: "",
            keyQuoted: false,
            colon: "=",
            comma: ";",
            trailComma: true,
            quote: '"');

        public static Writer JsonPretty = new Writer(Json) { Indent = "  " };
        public static Writer JsPretty = new Writer(Js) { Indent = "  " };
        public static Writer CSharpPretty = new Writer(CSharp) { Indent = "  " };

        public static Writer Default = new Writer(Js) {
            Comma = ", "
        };
        public static Writer Pretty = new Writer(Default) {
            Indent = "  ",
            Comma = "",
            Colon = ": "
        };

        public bool IgnoreFlag { get; private set; }
        public string Indent { get; private set; }
        public bool KeyQuoted { get; private set; }
        public string Colon { get; private set; }
        public string Comma { get; private set; }
        public bool TrailComma { get; private set; }
        public char Quote { get; private set; }

        private Writer(bool ignoreFlag, string indent, bool keyQuoted, string colon, string comma, bool trailComma,
            char quote) {
            this.IgnoreFlag = ignoreFlag;
            this.Indent = indent;
            this.KeyQuoted = keyQuoted;
            this.Colon = colon;
            this.Comma = comma;
            this.TrailComma = trailComma;
            this.Quote = quote;
        }

        private Writer() : this(
            ignoreFlag: true,
            indent: "",
            keyQuoted: true,
            colon: ":",
            comma: ",",
            trailComma: false,
            quote: '"') { }

        private Writer(Writer writer) : this(
            writer.IgnoreFlag, 
            writer.Indent, 
            writer.KeyQuoted, 
            writer.Colon, 
            writer.Comma, 
            writer.TrailComma, 
            writer.Quote) {
        }

        private void WriteColon(StringBuilder sb) {
            sb.Append(Colon);
        }

        private void WriteSep(StringBuilder sb, int level, int place) {
            if (place == 1 || place == 2 && TrailComma) {
                sb.Append(Comma);
            }

            if (string.IsNullOrEmpty(Indent)) {
                return;
            }

            sb.AppendLine();
            if (place < 2) {
                level++;
            }

            for (var i = 0; i < level; i++) {
                sb.Append(Indent);
            }
        }

        private StringBuilder WriteString(StringBuilder sb, string s) {
            sb.Append(Quote);
            foreach (var c in s) {
                if (c >= ' ') {
                    if (c == Quote || c == '\\') {
                        sb.Append('\\').Append(c);
                    } else {
                        sb.Append(c);
                    }
                } else {
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
                }
            }

            return sb.Append(Quote);
        }

        private void WriteKey(StringBuilder sb, string key) {
            if (!KeyQuoted && Paths.IsIdentifierOrInteger(key)) {
                 sb.Append(key);
            } else {
                 WriteString(sb, key);
            }
        }

        private static bool CanIgnoreFlag(ISigo sigo) {
            var f = sigo.Flags & 7;
            return f == 3 || f == 7 && sigo.Values.Any(v => 4 == (v.Flags & 4));
        }

        // TODO do we need detect circular object?
        public StringBuilder WriteSigo(StringBuilder sb, ISigo sigo, int level) {
            if (sigo.IsLeaf()) {
                return WriteAny(sb, sigo.Data, level);
            }

            sb.Append('{');
            var first = true;
            if (!IgnoreFlag || !CanIgnoreFlag(sigo)) {
                sb.Append(sigo.Flags & 7);
                first = false;
            }

            if (sigo.Count > 0) {
                foreach (var e in sigo) {
                    if (first) {
                        first = false;
                        WriteSep(sb, level, 0);
                    } else {
                        WriteSep(sb, level, 1);
                    }

                    WriteKey(sb, e.Key);
                    WriteColon(sb);
                    WriteSigo(sb, e.Value, level + 1);
                }

                WriteSep(sb, level, 2);
            }

            sb.Append('}');
            return sb;
        }

        // TODO why parentheses?
        private StringBuilder WriteObject(StringBuilder sb, IDictionary dict, int level) {
            sb.Append('(');
            sb.Append('{');
            var first = true;
            if (dict.Count > 0) {
                foreach (DictionaryEntry e in dict) {
                    if (first) {
                        first = false;
                        WriteSep(sb, level, 0);
                    } else {
                        WriteSep(sb, level, 1);
                    }

                    WriteKey(sb, e.Key.ToString());

                    WriteColon(sb);

                    WriteAny(sb, e.Value, level + 1);
                }

                WriteSep(sb, level, 2);
            }

            sb.Append('}');
            sb.Append(')');
            return sb;
        }

        // TODO why parentheses?
        private StringBuilder WriteArray(StringBuilder sb, IEnumerable list, int level) {
            sb.Append('(');
            sb.Append('[');
            if (list is ICollection col && col.Count == 0) {
                var first = true;
                foreach (var e in list) {
                    if (first) {
                        first = false;
                        WriteSep(sb, level, 0);
                    } else {
                        WriteSep(sb, level, 1);
                    }

                    WriteAny(sb, e, level + 1);
                }

                WriteSep(sb, level, 0);
            }

            sb.Append(']');
            sb.Append(')');
            return sb;
        }

        private StringBuilder WriteAny(StringBuilder sb, object o, int level) {
            switch (o) {
                case null: return sb.Append("null");
                case bool b: return sb.Append(b ? "true" : "false");
                case string s: return WriteString(sb, s);
                // FIXME sigo tree inside sigo leaf
                case ISigo sigo: return WriteSigo(sb, sigo, level);
                case IDictionary dict: return WriteObject(sb, dict, level);
                case IEnumerable list: return WriteArray(sb, list, level);
                default: return sb.Append(o);
            }
        }
    }
}