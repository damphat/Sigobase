using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests.MergeTests {
    public class OperatorPropertiesTests {
        [Fact]
        public void Associative() {
            var a = Sigo.Create(1,
                "a/x", "ax",
                "a/y", "ay",
                "b", "b"
            );

            // add "/a/z": az
            var b = Sigo.Create(0, "a/z", "az");

            // change "/b": "b+"
            var c = Sigo.Create(0, "b", "b+");

            var abc1 = Sigo.Merge(Sigo.Merge(a, b), c);
            var abc2 = Sigo.Merge(a, Sigo.Merge(b, c));
            var expect = Sigo.Create(3,
                "a/x", "ax",
                "a/y", "ay",
                "a/z", "az",
                "b", "b+"
            );

            SigoAssert.Equal(abc1, abc2);
            SigoAssert.Equal(expect, abc2);
        }
    }
}