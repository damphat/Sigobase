using System.Globalization;
using System.Linq;
using Sigobase.Database;
using Sigobase.Generator;
using Sigobase.Utils;
using Xunit;

namespace Sigobase.Tests {
    public class ParserTests {
        [Theory]
        [InlineData(" 1")]
        [InlineData("+1")]
        [InlineData("-1")]
        [InlineData(" Infinity")]
        [InlineData("+Infinity")]
        [InlineData("-Infinity")]
        [InlineData(" NaN")]
        [InlineData("+NaN")]
        [InlineData("-NaN")]
        public void NumberTest(string src) {
            var num = double.Parse(src, CultureInfo.InvariantCulture);

            Assert.Equal(Sigo.From(num), Sigo.Parse(src));
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void BoolTest(string src, bool value) {
            Assert.Equal(Sigo.From(value), Sigo.Parse(src));
        }

        [Theory]
        [InlineData("'abc'", "abc")]
        [InlineData("\"abc\"", "abc")]
        public void StringTest(string src, string value) {
            Assert.Equal(Sigo.From(value), Sigo.Parse(src));
        }

        [Fact]
        public void PathTest() {
            var expected = Sigo.Create(3, "name", Sigo.Create(3, "first", 1, "last", 2));
            Assert.Equal(expected, Sigo.Parse("{name/first:1, name/last:2}"));
        }

        [Fact]
        public void ObjectTest() {
            var anything = SigoSchema.Parse(@"
                // paste this to Sigobase.Generator.REPL
                primitive = true | 1 | 'a';
                obj1 = primitive | {? x?:1, y?:1 };
                obj2 = primitive | {? a?: obj1, b?: obj1};

                // return obj2
                obj2
            "
            ).Generate(GenerateOptions.None).ToList();

            foreach (var a in anything) {
                Assert.Equal(a, Sigo.Parse(a.ToString(Writer.Default)));
                Assert.Equal(a, Sigo.Parse(a.ToString(Writer.Pretty)));
                Assert.Equal(a, Sigo.Parse(a.ToString(Writer.Json)));
                Assert.Equal(a, Sigo.Parse(a.ToString(Writer.JsonPretty)));
                Assert.Equal(a, Sigo.Parse(a.ToString(Writer.Js)));
                Assert.Equal(a, Sigo.Parse(a.ToString(Writer.JsPretty)));
            }
        }
    }
}