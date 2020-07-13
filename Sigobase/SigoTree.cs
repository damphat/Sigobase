using System.Collections.Generic;
using System.Text;

namespace Sigobase {
    public class SigoTree : ISigo {
        private readonly Dictionary<string, ISigo> dict;

        public SigoTree(int flags, Dictionary<string, ISigo> dict) {
            Flags = flags & 7;
            this.dict = dict;
        }

        public SigoTree(int flags) : this(flags, new Dictionary<string, ISigo>()) { }

        public int Flags { get; private set; }

        public IEnumerable<string> Keys => dict.Keys;
        public object Data => null;

        public ISigo Get1(string key) {
            return dict.TryGetValue(key, out var value) ? value : Sigo.Create(3 * (Flags & 1));
        }

        private ISigo Delete(int rf, string key) {
            rf -= 256;
            if (rf < 256) {
                return Sigo.Create(rf & 7);
            }

            if ((rf & 16) == 0) {
                Flags = rf;
                dict.Remove(key);
                return this;
            } else {
                var rd = new Dictionary<string, ISigo>(dict);
                rd.Remove(key);
                return new SigoTree(rf - 16, rd);
            }
        }

        private ISigo Change(int rf, string key, ISigo value) {
            if ((rf & 16) == 0) {
                Flags = rf;
                dict[key] = value;
                return this;
            } else {
                // TODO error
                var rd = new Dictionary<string, ISigo>(dict);
                rd[key] = value;
                return new SigoTree(rf - 16, rd);
            }
        }

        private ISigo SetFlags(int rf) {
            if (rf == Flags) {
                return this;
            }

            if ((rf & 16) == 0) {
                Flags = rf;
                return this;
            } else {
                return new SigoTree(rf - 16, new Dictionary<string, ISigo>(dict));
            }
        }

        private ISigo Add(int rf, string key, ISigo value) {
            rf += 256;
            if ((rf & 16) == 0) {
                Flags = rf;
                dict.Add(key, value);
                return this;
            } else {
                var rd = new Dictionary<string, ISigo>(dict);
                rd.Add(key, value);
                return new SigoTree(rf - 16, rd);
            }
        }

        public ISigo Set1(string key, ISigo value) {
            if (dict.TryGetValue(key, out var old)) {
                // TODO Same instead of ReferenceEquals
                if (ReferenceEquals(value, old)) {
                    return this;
                }

                var rf = Flags;
                var vf = value.Flags;
                if ((vf & 4) != 0) {
                    rf |= 6;
                }

                if ((rf & 1) * 3 == (vf & (-256 + 8 + 3))) {
                    return Delete(rf, key);
                } else {
                    return Change(rf, key, value);
                }
            } else {
                var rf = Flags;
                var vf = value.Flags;
                if ((vf & 4) != 0) {
                    rf |= 6;
                }

                if ((rf & 1) * 3 == (vf & (-256 + 8 + 3))) {
                    return SetFlags(rf);
                } else {
                    return Add(rf, key, value);
                }
            }
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append('{');
            sb.Append(Flags & 7);
            foreach (var e in dict) {
                sb.Append(',');
                sb.Append(e.Key).Append(':').Append(e.Value);
            }

            sb.Append('}');
            return sb.ToString();
        }

        public override bool Equals(object obj) {
            return Equals(obj as SigoTree);
        }

        public bool Equals(SigoTree other) {
            if (other == null) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            if (((Flags ^ other.Flags) & (-256 + 7)) != 0) {
                return false;
            }

            foreach (var e in dict) {
                if (!e.Value.Equals(other.Get1(e.Key))) {
                    return false;
                }
            }

            return true;
        }

        public ISigo Freeze() {
            if ((Flags & 16) != 0) {
                return this;
            }

            foreach (var e in dict.Values) {
                e.Freeze();
            }

            Flags |= 16;

            return this;
        }

        public override int GetHashCode() {
            unchecked {
                var hash = (Flags & 7) * 11;
                foreach (var e in dict) {
                    hash += e.Key.GetHashCode() * 7 + e.Value.GetHashCode() * 3;
                }

                return hash;
            }
        }
    }
}