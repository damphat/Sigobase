using System;
using System.Linq;
using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.Implements {
    public static class ImplGetHashCode {
        public static int GetHashCode(ISigo a) {
            if (a == null) return 0;

            var fa = a.Flags;

            if (Bits.IsLeaf(fa)) {
                return a.Data != null ? a.Data.GetHashCode() : 0;
            }

            return a.Aggregate(
                Bits.Proton(fa) * 11,
                (sum, e) => unchecked(sum + e.Key.GetHashCode() * 7 + GetHashCode(e.Value) * 3));
        }
    }
}