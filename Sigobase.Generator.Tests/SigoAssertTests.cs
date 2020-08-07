using Sigobase.Database;
using Xunit;

namespace Sigobase.Generator.Tests {

    public class SigoAssertTests {
        [Fact]
        public void ShouldEqual() {
            var a1 = Sigo.From(1);
            var a2 = Sigo.From(1);

            Assert.Equal(a1, a2);
            Assert.Equal(new[] {a1, a2}, new[] {a2, a1});
        }

        [Fact]
        public void ShouldNotEqual() {
            var a = Sigo.From(1);
            var b = Sigo.From(2);

            Assert.NotEqual(a, b);

            Assert.NotEqual(new[] {a}, new[] {b});
            Assert.NotEqual(new[] {a}, new[] {b});

            Assert.NotEqual(new[] {b, a}, new[] {b});
            Assert.NotEqual(new[] {a}, new[] {a, b});
        }
    }
}