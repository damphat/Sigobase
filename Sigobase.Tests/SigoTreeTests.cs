using Xunit;

namespace Sigobase.Tests {
    public class SigoTreeTests {
        private readonly ISigo s;
        private readonly ISigo v1 = new SigoLeaf("v1");
        private readonly ISigo v2 = new SigoLeaf("v2");

        public SigoTreeTests() {
            s = new SigoTree(3);
            s = s.Set1("k1", v1);
        }

        [Fact()]
        public void SigoTreeTest() {
            Assert.Equal(256 + 7, s.Flags);
            Assert.Equal(new[] {"k1"} ,s.Keys);
            Assert.Null(s.Data);
        }

        [Fact()]
        public void Get1Test_return_the_child() {
            Assert.StrictEqual(v1, s.Get1("k1"));
            Assert.StrictEqual(Sigo.Create(3), s.Get1("k2"));
        }

        [Fact()]
        public void Get1Test_return_default() {
            Assert.StrictEqual(Sigo.Create(3), s.Get1("k2"));
        }

        [Fact()]
        public void Set1Test_delete() {
            ISigo s1 = s.Set1("k1", Sigo.Create(3));

            Assert.StrictEqual(Sigo.Create(7), s1);
        }

        [Fact()]
        public void Set1Test_change() {
            ISigo s1 = s.Set1("k1", v2);

            Assert.StrictEqual(v2, s1.Get1("k1"));
        }

        [Fact()]
        public void Set1Test_add() {
            ISigo s1 = s.Set1("k2", v2);

            Assert.StrictEqual(v1, s1.Get1("k1"));
            Assert.StrictEqual(v2, s1.Get1("k2"));
        }

        [Fact()]
        public void ToStringTest() {
            Assert.Equal("{7,k1:v1}", s.ToString());
        }
    }
}