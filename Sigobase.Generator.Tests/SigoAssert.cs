using System;
using System.Collections.Generic;
using Sigobase.Database;

namespace Sigobase.Generator.Tests {
    // an attempt to hide Xunit.Assert
    public static class Assert {}

    public static class SigoAssert {
        public static readonly IEqualityComparer<ISigo> Comparer = new SigoComparer();

        public static void Equal<T>(T a, T b) {
            if (a is ISigo sa && b is ISigo sb) {
                Xunit.Assert.Equal(sa, sb, Comparer);
            } else if (a is IEnumerable<ISigo> la && b is IEnumerable<ISigo> lb) {
                Xunit.Assert.Equal(la, lb, Comparer);
            } else {
                Xunit.Assert.Equal(a, b);
            }
        }

        public static void NotEqual<T>(T a, T b) {
            if (a is ISigo sa && b is ISigo sb) {
                Xunit.Assert.NotEqual(sa, sb, Comparer);
            } else if (a is IEnumerable<ISigo> la && b is IEnumerable<ISigo> lb) {
                Xunit.Assert.NotEqual(la, lb, Comparer);
            } else {
                Xunit.Assert.NotEqual(a, b);
            }
        }

        public static void Null(object obj) {
            Xunit.Assert.Null(obj);
        }

        public static void NotNull(object obj) {
            Xunit.Assert.NotNull(obj);
        }

        public static void Same(object expected, object actual) {
            Xunit.Assert.Same(expected, actual);
        }

        public static void NotSame(object expected, object actual) {
            Xunit.Assert.NotSame(expected, actual);
        }

        public static void Throws<T>(Action action) where T : Exception {
            Xunit.Assert.Throws<T>(action);
        }

        public static void ThrowsAny<T>(Action action) where T : Exception {
            Xunit.Assert.ThrowsAny<T>(action);
        }

        public static void True(bool b) {
            Xunit.Assert.True(b);
        }

        public static void False(bool b) {
            Xunit.Assert.False(b);
        }

        private class SigoComparer : EqualityComparer<ISigo> {
            public override bool Equals(ISigo x, ISigo y) {
                return Sigo.Equals(x, y);
            }

            public override int GetHashCode(ISigo obj) {
                return obj.GetHashCode();
            }
        }
    }
}