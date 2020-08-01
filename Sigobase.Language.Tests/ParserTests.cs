using Sigobase.Database;
using Xunit;

namespace Sigobase.Language.Tests {
    public class ParserTests {
        [Fact]
        public void ObjectTest() {
            var result = new Parser("{a: {b:1, c:1}}").Parse();
            var expected = Sigo.Create(3, "a", Sigo.Create(3, "b", 1, "c", 1));

            Assert.Equal(expected, result);
        }
    }
}