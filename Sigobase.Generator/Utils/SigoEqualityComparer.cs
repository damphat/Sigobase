using System.Collections.Generic;
using System.Diagnostics;
using Sigobase.Database;

namespace Sigobase.Generator.Utils {
    internal class SigoEqualityComparer : EqualityComparer<ISigo> {
        public override bool Equals(ISigo x, ISigo y) {
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            if (ReferenceEquals(x, y)) {
                return true;
            }

            if (x.Flags != y.Flags) {
                return false;
            }

            if (x.IsLeaf()) {
                return Equals(x.Data, y.Data);
            }

            foreach (var (k, v) in x) {
                if (!Equals(v, y.Get1(k))) {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode(ISigo obj) {
            if (obj.IsLeaf()) {
                return obj.Data.GetHashCode();
            }

            var hash = obj.Flags;
            foreach (var (k, v) in obj) {
                hash = unchecked(hash + k.GetHashCode() * 11 + GetHashCode(v));
            }

            return hash;
        }
    }
}