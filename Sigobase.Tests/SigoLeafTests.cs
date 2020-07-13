using Xunit;

namespace Sigobase.Tests {
    public class SigoLeafTests {
        private readonly ISigo s = new SigoLeaf("s");

        [Fact()]
        public void SigoLeafTest() {
            Assert.Equal(16 + 8 + 7, s.Flags);
            Assert.Empty(s.Keys);
            Assert.Equal("s", s.Data);
        }

        [Fact()]
        public void Get1Test() {
            Assert.StrictEqual(new SigoTree(3), s.Get1("k"));
        }

        [Fact()]
        public void Set1Test_return_self() {
            ISigo s1 = s.Set1("k", new SigoTree(3));

            Assert.Same(s, s1);
        }

        [Fact()]
        public void Set1Test_return_empty_7() {
            ISigo s1 = s.Set1("k", new SigoTree(7));

            Assert.StrictEqual(new SigoTree(7), s1);
        }

        [Fact()]
        public void Set1Test_return_tree() {
            ISigo v = new SigoLeaf("v");
            ISigo s1 = s.Set1("k", v);

            Assert.Same(v, s1.Get1("k"));
        }

        [Fact()]
        public void ToStringTest() {
            Assert.Equal("s", s.ToString());
        }
    }
}