using System.Linq;
using Sigobase.Database;
using Xunit;

namespace Sigobase.Generator.Tests {
    public class GenerateOptionTests {
        private string src = "2|0|2|0";

        private static ISigo[] Leafs(params object[] values) {
            return values.Select(Sigo.From).ToArray();
        }

        [Fact]
        public void NoneTest() {
            Utils.Equal(Leafs(2, 0, 2, 0), SigoSchema.Parse(src).Generate(GenerateOptions.None));
        }

        [Fact]
        public void UniqueTest() {
            Utils.Equal(Leafs(2, 0), SigoSchema.Parse(src).Generate(GenerateOptions.Unique));
        }

        [Fact]
        public void SortedTest() {
            Utils.Equal(Leafs(0, 0, 2, 2), SigoSchema.Parse(src).Generate(GenerateOptions.Sorted));
        }

        [Fact]
        public void UniqueSortedTest() {
            Utils.Equal(Leafs(0, 2), SigoSchema.Parse(src).Generate(GenerateOptions.UniqueSorted));
        }
    }
}