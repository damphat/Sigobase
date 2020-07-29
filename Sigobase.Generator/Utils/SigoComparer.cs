using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Utils {
    class SigoComparer : Comparer<ISigo> {
        public override int Compare(ISigo x, ISigo y) {
            if (y.IsLeaf()) {
                if (x.IsLeaf()) {
                    return Comparer<object>.Default.Compare(x.Data, y.Data);
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

                    using var xe = x.GetEnumerator();
                    using var ye = y.GetEnumerator();
                    while (xe.MoveNext() && ye.MoveNext()) {
                        delta = Comparer<string>.Default.Compare(xe.Current.Key, ye.Current.Key);
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