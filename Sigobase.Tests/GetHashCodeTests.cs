using System.Collections.Generic;
using Sigobase.Database;
using Sigobase.Implements;
using Xunit;

namespace Sigobase.Tests {
    public class GetHashCodeTests {
        private static int Hash(ISigo a) {
            return Sigo.GetHashCode(a);
        }

        [Fact]
        public void If_sigosAreEqual_then_hashsAreEqual() {
            var sigos = new List<ISigo> {
                Sigo.From("a"),
                Sigo.From("a"),
                Sigo.From("b"),
                Sigo.Create(0),
                Sigo.Create(3),
                Sigo.Create(1, "x", "1", "y", "2").Freeze(),
                Sigo.Create(1, "y", "2", "x", "1"),
                Sigo.Create(1, "y", "2", "x", "2"),
                Sigo.Create(1, "diff", "2", "x", "1")
            };
            foreach (var a in sigos) {
                foreach (var b in sigos) {
                    if (a.Equals(b)) {
                        // THIS IS A MUST
                        SigoAssert.Equal(Hash(a), Hash(b));
                    } else {
                        // THIS IS A SHOULD
                        SigoAssert.NotEqual(Hash(a), Hash(b));
                    }
                }
            }
        }

        [Fact]
        public void Overrided_getHashCode() {
            var list = new List<ISigo> {
                Sigo.From("a"),
                Sigo.Create(7),
                Sigo.Create(3, "k", "a")
            };

            foreach (var a in list) {
                SigoAssert.Equal(Sigo.GetHashCode(a), a.GetHashCode());
            }
        }

        [Fact]
        public void Ignore_frozen_bit() {
            SigoAssert.Equal(
                Hash(Sigo.Create(0, "x", 1)),
                Hash(Sigo.Create(0, "x", 1).Freeze())
            );
        }

        [Fact]
        public void Null_return_0() {
            SigoAssert.Equal(0, Hash(null));
        }

        [Fact]
        public void Never_throw_overflow_exception() {
            var n = int.MaxValue / 100;
            for (int i = 0; i < 1000; i++) {
                var s = Sigo.Create(0, "x", n * i, "y", n);
                Hash(s);
            }
        }
    }
}