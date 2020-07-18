using Sigobase.Database;
using Sigobase.Implements;
using Xunit;

namespace Sigobase.Tests.MergeTests {
    public class PTests {
        private ISigo a = Sigo.From("a");
        private ISigo aa = Sigo.From("a");
        private ISigo b = Sigo.From("b");
        private ISigo e0 = Sigo.Create(0);

        private static ISigo Merge(ISigo a, ISigo b) {
            var r = ImplMerge.Merge(a, b);
            return r;
        }

        [Fact]
        public void PP_return_a_if_same() {
            Assert.Same(a, Merge(a, a));
            Assert.Same(a, Merge(a, aa));
        }

        [Fact]
        public void PS_return_a_if_b_have_no_m() {
            Assert.Same(a, Merge(a, e0));
            Assert.Same(a, Merge(a, Sigo.Create(5, "x", e0)));
        }

        [Fact]
        public void SP_return_b_if_not_same() {
            Assert.Same(b, Merge(a, b));
            Assert.Same(b, Merge(e0, b));
        }
    }
}