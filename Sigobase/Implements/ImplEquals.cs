using System.Linq;
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

            return Bits.IsLeaf(a.Flags)
                ? Equals(a.Data, b.Data)
                : a.All(e => Equals(e.Value, b.Get1(e.Key)));
        }
    }
}