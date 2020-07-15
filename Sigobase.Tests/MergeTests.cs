using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class MergeTests {

        [Fact]
        public void Associative_property() {
            var a = Sigo.Create(1,
                "a/x", "ax",
                "a/y", "ay",
                "b", "b"
            );

            var b = Sigo.Create(0, "a/z", "az");
            var c = Sigo.Create(0, "b", "b+");

            var abc1 = Sigo.Merge(Sigo.Merge(a, b), c);
            var abc2 = Sigo.Merge(a, Sigo.Merge(b, c));
            var expect = Sigo.Create(3,
                "a/x", "ax",
                "a/y", "ay",
                "a/z", "az",
                "b", "b+"
            );

            Assert.Equal(abc1, abc2);
            Assert.Equal(expect, abc2);
        }
    }
}