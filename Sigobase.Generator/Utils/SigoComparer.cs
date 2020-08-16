using System;
using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Utils {
    // Fixed Comparer<object>.Default.Compare(x.Data = "2", y.Data = 1);
    // Fixed method overloading + inheritance
    // child orders master
    internal class SigoComparer : Comparer<ISigo> {
        private static readonly Type[] Types = new Type[] {
            typeof(bool),
            typeof(double),
            typeof(string)
        };

        private static int CompareType(Type tx, Type ty) {
            return Array.IndexOf(Types, tx) - Array.IndexOf(Types, ty);
        }

        private static int CompareObject(object x, object y) {
            if (ReferenceEquals(x, y)) {
                return 0;
            }

            if (x == null) {
                return -1; // null < any
            }

            if (y == null) {
                return 1; // any > null
            }

            var tx = x.GetType();
            var ty = y.GetType();

            if (tx == ty) {
                return Comparer<object>.Default.Compare(x, y);
            }

            return CompareType(tx, ty);
        }

        private int CompareChildrenWithOrder(ISigo x, ISigo y) {
            using (var xe = x.GetEnumerator()) {
                using (var ye = y.GetEnumerator()) {
                    while (xe.MoveNext() && ye.MoveNext()) {
                        var delta = string.CompareOrdinal(xe.Current.Key, ye.Current.Key);
                        if (delta != 0) {
                            return delta;
                        }

                        delta = Compare(xe.Current.Value, ye.Current.Value);
                        if (delta != 0) {
                            return delta;
                        }
                    }

                    return 0;
                }
            }
        }

        public override int Compare(ISigo x, ISigo y) {
            if (ReferenceEquals(x, y)) {
                return 0;
            }

            if (x == null) {
                return -1; // null < any
            }

            if (y == null) {
                return 1; // any > null
            }

            // include neutrons
            if (x.Flags != y.Flags) {
                return x.Flags - y.Flags;
            }

            return x.IsLeaf()
                ? CompareObject(x.Data, y.Data)
                : CompareChildrenWithOrder(x, y);
        }
    }
}