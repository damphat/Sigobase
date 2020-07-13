using System.Collections.Generic;

namespace Sigobase {
    public class SigoLeaf : ISigo {
        public int Flags => 31;

        public SigoLeaf(object data) {
            Data = data;
        }

        public ISigo Get1(string key) {
            return Sigo.Create(3);
        }

        public ISigo Set1(string key, ISigo value) {
            switch (value.Flags & (-256 + 8 + 7)) {
                case 3: return this;
                case 7: return Sigo.Create(7);
                default: return Sigo.Create(7).Set1(key, value);
            }
        }

        public IEnumerable<string> Keys {
            get { yield break; }
        }

        public object Data { get; }

        public override string ToString() {
            return (Data ?? "null").ToString();
        }

        public override bool Equals(object obj) {
            return Equals(obj as SigoLeaf);
        }

        public bool Equals(SigoLeaf other) {
            return other != null && Equals(Data, other.Data);
        }

        public ISigo Freeze() {
            return this;
        }

        public override int GetHashCode() {
            return Data != null ? Data.GetHashCode() : 0;
        }
    }
}