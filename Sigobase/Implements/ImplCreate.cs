using Sigobase.Database;

namespace Sigobase.Implements {
    public static class ImplCreate {
        private static string ToPath(object path) {
            return path?.ToString();
        }

        public static ISigo Create(int lmr, params object[] pvs) {
            var i = 0;
            var ret = Sigo.Create(lmr);
            while (i < pvs.Length) {
                var path = ToPath(pvs[i++]);
                var value = Sigo.From(pvs[i++]);
                ret = ret.Set(path, value);
            }

            return ret;
        }
    }
}