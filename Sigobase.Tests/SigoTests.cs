using Xunit;

namespace Sigobase.Tests {
    public class SigoTests {

        [Fact]
        public void MergeTest() {
            ISigo user = Sigo.Create(3, "name", "phat");
            ISigo addAge = Sigo.Create(0, "age", 30);

            ISigo ret = Sigo.Merge(user, addAge);

            Assert.StrictEqual(Sigo.Create(3, "name", "phat", "age", 30), ret);
        }
    }
}