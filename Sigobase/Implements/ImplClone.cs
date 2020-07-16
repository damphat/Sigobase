using Sigobase.Database;

namespace Sigobase.Implements {
    public static class ImplClone {
        public static ISigo Clone(ISigo a) {
            if (a.IsFrozen()) return a;
            var ret = Sigo.Create(a.Flags & 7);
            foreach (var e in a) {
                ret = ret.Set1(e.Key, Clone(e.Value));
            }

            return ret;
        }
    }
}