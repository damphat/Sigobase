using Sigobase.Database;
using System.Collections.Generic;
using Xunit;

namespace Sigobase.Generator.Tests {
    public static class Utils {
        public static readonly IEqualityComparer<ISigo> Comparer = new SigoComparer();

        private class SigoComparer : EqualityComparer<ISigo> {
            public override bool Equals(ISigo x, ISigo y) {
                return Sigo.Equals(x, y);
            }

            public override int GetHashCode(ISigo obj) {
                return obj.GetHashCode();
            }
        }

        public static void Equal(IEnumerable<ISigo> a, IEnumerable<ISigo> b) {
            Assert.Equal(a, b, Comparer);
        }

        public static void NotEqual(IEnumerable<ISigo> a, IEnumerable<ISigo> b) {
            Assert.NotEqual(a, b, Comparer);
        }
    }
}