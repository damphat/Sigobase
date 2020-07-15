using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.Implements {
    public static class ImplGetHashCode {
        /// <summary>
        /// IEqualtyComparer<ISigo>
        /// </summary>
        public static int GetHashCode(ISigo a) {
            unchecked {
                var fa = a.Flags;
                if (Bits.IsLeaf(fa)) {
                    return a.Data != null ? a.Data.GetHashCode() : 0;
                }

                var hash = Bits.Proton(fa) * 11;
                foreach (var k in a.Keys) {
                    var v = a.Get1(k);
                    hash += k.GetHashCode() * 7 + GetHashCode(v) * 3;
                }

                return hash;
            }
        }
    }
}