using System.Collections.Generic;
using System.Text;
using Sigobase.Database;

namespace Sigobase.Generator.Schemas {
    class ObjSchema : Schema {
        public class FieldInfo {
            public FieldInfo(Schema schema, bool optional) {
                this.schema = schema;
                this.optional = optional;
            }

            public Schema schema;
            public bool optional;
        }

        public List<ISigo> Flags;
        public Dictionary<string, FieldInfo> Fields = new Dictionary<string, FieldInfo>();

        public void Add(string key, Schema schema, in bool optional) {
            Fields.Add(key, new FieldInfo(schema, optional));
        }

        public override IEnumerable<ISigo> Values() {
            var keys = new List<string>();
            var values = new List<List<ISigo>>();

            foreach (var field in Fields) {
                keys.Add(field.Key);
                var ret = new List<ISigo>();
                if (field.Value.optional) {
                    ret.Add(null);
                }

                ret.AddRange(field.Value.schema.Values());
                values.Add(ret);
            }

            var n = 1;
            foreach (var value in values) {
                n *= value.Count;
            }

            foreach (var e in Flags) {
                for (int i = 0; i < n; i++) {
                    var t = i;
                    var o = e;
                    for (var k = 0; k < keys.Count; k++) {
                        var key = keys[k];
                        var count = values[k].Count;
                        var value = values[k][t % count];
                        if (value != null) {
                            o = o.Set1(key, value);
                        }

                        t /= count;
                    }

                    yield return o;
                }
            }
        }

        public override int Count() {
            var ret = Flags.Count;
            foreach (var field in Fields.Values) {
                if (field.optional) {
                    ret *= field.schema.Count() + 1;
                } else {
                    ret *= field.schema.Count();
                }
            }

            return ret;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var e in Flags) {
                sb.Append((char) ((e.Flags & 7) + '0'));
            }

            var f = sb.ToString();
            sb.Clear();
            var first = true;
            sb.Append('{');
            if (f != "01234567") {
                sb.Append(f);
                first = false;
            }

            foreach (var field in Fields) {
                if (first) {
                    first = false;
                } else {
                    sb.Append(',');
                }

                sb.Append(field.Key);
                if (field.Value.optional) sb.Append('?');
                sb.Append(':');
                sb.Append(field.Value.schema);
            }

            sb.Append('}');
            return sb.ToString();
        }
    }
}