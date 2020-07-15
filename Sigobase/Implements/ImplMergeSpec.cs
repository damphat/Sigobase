using System.Linq;
using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.Implements {
    public static class ImplMergeSpec {
        private static ISigo Select(ISigo r, ISigo a, ISigo b) {
            if (a.IsFrozen() && Sigo.Equals(r, a)) {
                return a;
            }

            if (b.IsFrozen() && Sigo.Equals(r, b)) {
                return b;
            }

            return r;
        }

        /// <summary>
        /// try to return part of a if a is frozen
        /// try to return part of b if b is frozen
        /// return new sigo
        /// </summary>
        public static ISigo Merge(ISigo a, ISigo b) {
            ISigo r;
            if (b is SigoLeaf) {
                r = b;
                return Select(r, a, b);
            }

            if (a is SigoLeaf) {
                if ((b.Flags & Bits.M) == 0) {
                    r = a;

                    return Select(r, a, b);
                }

                r = Merge(Sigo.Create(Bits.LMR), b);
                return Select(r, a, b);
            }

            r = Sigo.Create(Bits.Proton(a.Flags | b.Flags));
            var keys = a.Keys.Union(b.Keys);
            foreach (var key in keys) {
                r = r.Set1(key, Merge(a.Get1(key), b.Get1(key)));
            }

            return Select(r, a, b);
        }
    }
}