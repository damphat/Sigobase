using System.Linq;
using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.Implements {
    public static class ImplMerge {
        /// <summary>
        /// The most simple implementation of merge.
        /// This do not reuse any trees from a and b
        /// This do reuse leafs, from b is prefer.
        /// </summary>
        public static ISigo Merge(ISigo a, ISigo b) {
            if (b is SigoLeaf) {
                return b;
            }

            if (a is SigoLeaf) {
                if ((b.Flags & Bits.M) == 0) {
                    return a;
                }

                return Merge(Sigo.Create(Bits.LMR), b);
            }

            var r = Sigo.Create(a.Flags | b.Flags);
            var keys = a.Keys.Union(b.Keys);
            foreach (var key in keys) {
                r = r.Set1(key, Merge(a.Get1(key), b.Get1(key)));
            }

            return r;
        }
    }
}