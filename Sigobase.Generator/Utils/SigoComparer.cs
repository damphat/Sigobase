using System;
using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Utils {
    // Fixed Comparer<object>.Default.Compare(x.Data = "2", y.Data = 1);
    class SigoComparer : Comparer<ISigo> {
        private static readonly Type[] types = new Type[] {
            typeof(bool), 
            typeof(double), 
            typeof(string)
        };

        private static int CompareType(Type tx, Type ty) {
            return Array.IndexOf(types, tx) - Array.IndexOf(types, ty);
        }

        private static int CompareObject(object x, object y) {
            if (ReferenceEquals(x, y)) return 0;
            if (y == null) return 1; // everything > null

            var tx = x.GetType();
            var ty = y.GetType();

            if (tx == ty) return Comparer<object>.Default.Compare(x, y);
            return CompareType(tx, ty);

        }

        public override int Compare(ISigo x, ISigo y) {
            if (y.IsLeaf()) {
                if (x.IsLeaf()) {
                    return CompareObject(x.Data, y.Data);
                } else {
                    return -1;
                }
            } else {
                if (x.IsLeaf()) {
                    return 1;
                } else {
                    var delta = x.Flags - y.Flags;
                    if (delta != 0) {
                        return delta;
                    }

                    using IEnumerator<KeyValuePair<string, ISigo>> xe = x.GetEnumerator();
                    using IEnumerator<KeyValuePair<string, ISigo>> ye = y.GetEnumerator();
                    while (xe.MoveNext() && ye.MoveNext()) {
                        delta = string.CompareOrdinal(xe.Current.Key, ye.Current.Key);
                        if (delta != 0) return delta;
                        delta = Compare(xe.Current.Value, ye.Current.Value);
                        if (delta != 0) return delta;
                    }

                    return 0;
                }
            }
        }
    }
}