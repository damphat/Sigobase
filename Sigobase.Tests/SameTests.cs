using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class SameTests {
        [Fact]
        public void Trees_only_compare_using_referenceEquals() {
            var tree1 = Sigo.Create(3).Set1("k", Sigo.From("v"));
            var tree2 = Sigo.Create(3).Set1("k", Sigo.From("v"));

            Assert.True(Sigo.Same(tree1, tree1));

            Assert.False(Sigo.Same(tree1, tree2));
        }

        [Fact]
        public void Leafs_compare_data_using_equals() {
            var v1 = Sigo.From("v");
            var v2 = Sigo.From("v");
            var x1 = Sigo.From("x");

            Assert.True(Sigo.Same(v1, v1));
            Assert.True(Sigo.Same(v1, v2));

            Assert.False(Sigo.Same(v1, x1));
        }
    }
}