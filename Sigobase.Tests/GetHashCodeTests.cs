using System.Collections.Generic;
using Sigobase.Database;
using Sigobase.Implements;
using Xunit;

namespace Sigobase.Tests {
    public class GetHashCodeTests {
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
                        Assert.Equal(ImplGetHashCode.GetHashCode(a), ImplGetHashCode.GetHashCode(b));
                    } else {
                        // THIS IS A SHOULD
                        Assert.NotEqual(ImplGetHashCode.GetHashCode(a), ImplGetHashCode.GetHashCode(b));
                    }
                }
            }
        }

        [Fact]
        public void BeCarefulOf_arithmeticOverflow() {
            var s1 = Sigo.Create(3, "k1", int.MaxValue, "k2", int.MaxValue);
            var s2 = Sigo.Create(3, "k2", int.MaxValue, "k1", int.MaxValue);
            Assert.Equal(ImplGetHashCode.GetHashCode(s1), ImplGetHashCode.GetHashCode(s2));
        }
    }
}