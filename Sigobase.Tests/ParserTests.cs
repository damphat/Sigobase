using System.Linq;
using Sigobase.Database;
using Sigobase.Generator;
using Sigobase.Utils;
using Xunit;

namespace Sigobase.Tests {
    public class ParserTests {
        [Fact]
        public void LeafTest() {
            Assert.Equal(Sigo.From(false), Sigo.Parse("false"));
            Assert.Equal(Sigo.From(true), Sigo.Parse("true"));
            Assert.Equal(Sigo.From(123), Sigo.Parse("123"));
            Assert.Equal(Sigo.From("abc"), Sigo.Parse("'abc'"));
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
obj1 = {? x?:1, y?:1 } | primitive;
obj2 = {? a?:obj1, b?:obj1};

primitive | obj2
"
            ).Generate().ToList();

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