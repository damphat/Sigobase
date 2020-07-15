using System.Collections.Generic;
using Sigobase.Implements;
using Sigobase.Utils;

namespace Sigobase.Database {
    public static class Sigo {
        public static ISigo Create(int lmr) {
            return ImplCreateElement.Create(lmr);
        }

        public static ISigo From(object o) {
            return ImplFrom.From(o);
        }

        public static bool Same(this ISigo sigo, ISigo other) {
            return ImplSame.Same(sigo, other);
        }

        public static bool Equals(this ISigo sigo, ISigo other) {
            return ImplEquals.Equals(sigo, other);
        }

        public static ISigo Merge(this ISigo sigo, ISigo other) {
            return ImplMergeSpec.Merge(sigo, other);
        }

        public static ISigo Merge(params ISigo[] sigos) {
            switch (sigos.Length) {
                case 0: return Create(0);
                case 1: return sigos[0];
                case 2: return Merge(sigos[0], sigos[1]);
                default: {
                    var ret = sigos[0];
                    for (var i = 1; i < sigos.Length; i++) {
                        ret = Merge(ret, sigos[i]);
                    }

                    return ret;
                }
            }
        }

        public static ISigo Get(this ISigo sigo, string path) {
            return ImplGet.Get(sigo, path);
        }

        public static ISigo SetN(this ISigo sigo, IReadOnlyList<string> path, ISigo value, int start) {
            return ImplSet.SetN(sigo, path, value, start);
        }

        public static ISigo Set(this ISigo sigo, string path, ISigo value) {
            return ImplSet.Set(sigo, path, value);
        }

        public static ISigo Create(int lmr, params object[] pvs) {
            return ImplCreate.Create(lmr, pvs);
        }

        public static bool IsFrozen(this ISigo sigo) {
            return Bits.IsFrozen(sigo.Flags);
        }

        public static bool IsLeaf(this ISigo sigo) {
            return Bits.IsLeaf(sigo.Flags);
        }

        public static bool IsTree(this ISigo sigo) {
            return Bits.IsTree(sigo.Flags);
        }
    }
}