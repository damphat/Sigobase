using Sigobase.Language;
using Xunit;

namespace Sigobase.Tests {
    public class PathStackTests {
        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("a/b")]
        [InlineData("a/b:")]
        [InlineData("a/b:c")]
        [InlineData("a/b:c/d")]
        [InlineData(":")]
        [InlineData("a::")]
        public void Push(string src) {
            Assert.Equal(src, new PathStack(src).ToString());
        }

        [Theory]
        [InlineData(null, "null")]
        [InlineData("", "")]
        [InlineData("a", "a")]
        [InlineData("a/b", "a/b")]
        [InlineData(true, "true")]
        [InlineData(double.PositiveInfinity, "Infinity")]
        public void AddTest(object key, string expect) {
            var ps = new PathStack();
            ps.Add(key);
            Assert.Equal(expect, ps.ToString());
        }


        [Theory]
        [InlineData(":a::", ":a:")]
        [InlineData(":a:", ":a")]
        [InlineData(":a", "")]
        public void PopTest(string src, string expect) {
            var ps = new PathStack(src);
            Xunit.Assert.Equal(src, ps.ToString());
            ps.Pop();

            Assert.Equal(expect, ps.ToString());
        }

        [Theory]
        [InlineData(":a::", ":a::")]
        [InlineData(":a/b", ":")]
        [InlineData(":a", ":")]
        public void ClearTest(string src, string expect) {
            var ps = new PathStack(src);
            Xunit.Assert.Equal(src, ps.ToString());
            ps.Clear();

            Assert.Equal(expect, ps.ToString());
        }
    }
}
