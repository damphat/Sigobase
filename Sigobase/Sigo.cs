using System.Linq;

namespace Sigobase {
    public static class Sigo {
        public static ISigo From(object o) {
            return new SigoLeaf(o);
        }

        public static ISigo Create(int flags) {
            return new SigoTree(flags);
        }

        public static string ToPath(object path) {
            return path.ToString();
        }

        public static ISigo Create(int flags, params object[] pvs) {
            var i = 0;
            var ret = Create(flags);
            while (i < pvs.Length) {
                var path = ToPath(pvs[i++]);
                var value = From(pvs[i++]);
                ret = ret.Set1(path, value);
            }

            return ret;
        }

        public static ISigo Merge(ISigo a, ISigo b) {
            if(b is SigoLeaf) return b;
            if(a is SigoLeaf) {
                if((b.Flags & 2) == 0) return a;
                return Merge(Create(7), b);
            }
            var r = Create(a.Flags | b.Flags);
            var keys = a.Keys.Union(b.Keys);
            foreach (var key in keys) {
                r = r.Set1(key, Merge(a.Get1(key), b.Get1(key)));
            }

            return r;
        }
    }
}