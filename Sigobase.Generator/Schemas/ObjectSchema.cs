using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sigobase.Database;
using Sigobase.Generator.Utils;

namespace Sigobase.Generator.Schemas {
    internal class ObjectSchema : SigoSchema {
        public class FieldInfo {
            public FieldInfo(SigoSchema schema, bool optional) {
                this.schema = schema;
                this.optional = optional;
            }

            public SigoSchema schema;
            public bool optional;
        }

        public List<ISigo> Flags;
        public Dictionary<string, FieldInfo> Fields = new Dictionary<string, FieldInfo>();

        public void Add(string key, SigoSchema schema, in bool optional) {
            Fields.Add(key, new FieldInfo(schema, optional));
        }

        private IEnumerable<ISigo> GenerateInternal(GenerateOptions filter) {
            var keys = new List<string>();
            var values = new List<List<ISigo>>();

            foreach (var field in Fields) {
                keys.Add(field.Key);
                var ret = new List<ISigo>();
                if (field.Value.optional) {
                    ret.Add(null);
                }

                ret.AddRange(field.Value.schema.Generate(filter));
                values.Add(ret);
            }

            var n = 1;
            foreach (var value in values) {
                n *= value.Count;
            }

            foreach (var e in Flags) {
                for (var i = 0; i < n; i++) {
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

        public override IEnumerable<ISigo> Generate(GenerateOptions options) {
            switch (options) {
                default: return GenerateInternal(options);
                case GenerateOptions.Unique: {
                    var ret = new SigoHashSet(GenerateInternal(options));
                    return ret.ToList();
                }
                case GenerateOptions.Sorted: {
                    var ret = new List<ISigo>(GenerateInternal(options));
                    ret.Sort(new SigoComparer());
                    return ret;
                }
                case GenerateOptions.Unique | GenerateOptions.Sorted: {
                    var ret = new SigoSortedSet(GenerateInternal(options));
                    return ret.ToList();
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
            } else {
                sb.Append('?');
                first = false;
            }

            foreach (var field in Fields) {
                if (first) {
                    first = false;
                } else {
                    sb.Append(", ");
                }

                sb.Append(field.Key);
                if (field.Value.optional) {
                    sb.Append('?');
                }

                sb.Append(':');
                sb.Append(field.Value.schema);
            }

            sb.Append('}');
            return sb.ToString();
        }
    }
}