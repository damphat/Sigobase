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

        public static ISigo Clone(this ISigo sigo) {
            return ImplClone.Clone(sigo);
        }

        public static bool Same(this ISigo sigo, ISigo other) {
            return ImplSame.Same(sigo, other);
        }

        public static bool Equals(ISigo sigo, ISigo other) {
            return ImplEquals.Equals(sigo, other);
        }

        public static int GetHashCode(ISigo sigo) {
            return ImplGetHashCode.GetHashCode(sigo);
        }

        public static ISigo Merge(this ISigo sigo, ISigo other) {
            return ImplMergeSpec.Merge(sigo, other);
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

        public static ISigo Set(this ISigo sigo, string path, object value) {
            return ImplSet.Set(sigo, path, From(value));
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

        public static string ToString(this ISigo sigo, int indent) {
            return ImplToString.ToString(sigo, indent);
        }
    }
}