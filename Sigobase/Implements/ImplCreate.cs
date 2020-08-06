using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.Implements {
    public static class ImplCreate {
        public static ISigo Create(int lmr, params object[] pvs) {
            var i = 0;
            var ret = Sigo.Create(lmr);
            while (i < pvs.Length) {
                var path = Paths.ToPath(pvs[i++]);
                var value = Sigo.From(pvs[i++]);
                if (path == null) {
                    return value;
                }

                if (path is string key) {
                    if (key == "") {
                        return value;
                    }

                    ret = ret.Set1(key, value);
                } else if (path is string[] keys) {
                    ret = ret.SetN(keys, value, 0);
                }
            }

            return ret;
        }
    }
}