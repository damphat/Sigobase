using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.Implements {
    public static class ImplSame {
        /// <summary>
        /// 
        /// </summary>
        public static bool Same(ISigo a, ISigo b) {
            if (ReferenceEquals(a, b)) {
                return true;
            }

            if (Bits.IsLeaf(a.Flags) && Bits.IsLeaf(b.Flags)) {
                return Equals(a.Data, b.Data);
            }

            return false;
        }
    }
}