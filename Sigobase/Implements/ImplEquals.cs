using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.Implements {
    public static class ImplEquals {
        public static bool Equals(ISigo a, ISigo b) {
            if (ReferenceEquals(a, b)) {
                return true;
            }

            if (Bits.IsSame(a.Flags, b.Flags) == false) {
                return false;
            }

            if (Bits.IsLeaf(a.Flags)) {
                return Equals(a.Data, b.Data);
            }

            foreach (var k in a.Keys) {
                if (Equals(a.Get1(k), b.Get1(k)) == false) {
                    return false;
                }
            }

            return true;
        }
    }
}