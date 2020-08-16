using System.Collections.Generic;
using System.Linq;
using Sigobase.Database;
using Xunit;

namespace Sigobase.Generator.Tests {
    public class SigoSchemaTests {
        private static IList<ISigo> Gen(string src) {
            return SigoSchema.Parse(src).Generate().ToList();
        }

        private static IList<ISigo> Leafs(params object[] values) {
            return values.Select(Sigo.From).ToList();
        }

        [Theory]
        [InlineData("{1;}", "{1}")]
        [InlineData("{1,}", "{1}")]
        [InlineData("{1,x:1,}", "{1, x:1}")]
        [InlineData("{1;x:1;}", "{1, x:1}")]
        [InlineData("{1 x:1 }", "{1, x:1}")]
        [InlineData("{x:1;y:1;}", "{x:1, y:1}")]
        [InlineData("{x:1,y:1,}", "{x:1, y:1}")]
        [InlineData("{x:1 y:1 }", "{x:1, y:1}")]
        public void Object_separator_field(string a, string b) {
            var sa = SigoSchema.Parse(a);
            var sb = SigoSchema.Parse(b);
            SigoAssert.Equal(sa.Count(), sb.Count());
        }

        [Theory]
        [InlineData("{?;}", "{?}")]
        [InlineData("{?,}", "{?}")]
        [InlineData("{?,x:1,}", "{?, x:1}")]
        [InlineData("{?;x:1;}", "{?, x:1}")]
        [InlineData("{? x:1 }", "{?, x:1}")]
        [InlineData("x=1; {x?,}", "x=1; {x?}")]
        [InlineData("x=1; {x?;}", "x=1; {x?}")]
        [InlineData("x=1; {x? }", "x=1; {x?}")]
        public void Object_separator_flag(string a, string b) {
            var sa = SigoSchema.Parse(a);
            var sb = SigoSchema.Parse(b);
            SigoAssert.Equal(sa.Count(), sb.Count());
        }

        [Theory]
        [InlineData("x=1 x")]
        [InlineData("x=1;x")]
        [InlineData("x=1;x;")]
        [InlineData("x=2; x=1; x; x")]
        public void Statements_separator(string a) {
            SigoAssert.Equal(Leafs(1), Gen(a));
        }

        [Fact]
        public void List_() {
            var expected = Leafs(false, true, 123, "abc");

            SigoAssert.Equal(expected, Gen(" false | true | 123 | 'abc' "));
        }

        [Fact]
        public void Object_field_() {
            var expected = new[] {
                Sigo.Create(3, "x", 1),
                Sigo.Create(3, "x", 2),
                Sigo.Create(3, "x", 3)
            };

            SigoAssert.Equal(expected, Gen("{x: 1|2|3}"));
        }

        [Fact]
        public void Object_field_auto() {
            var expected = new[] {
                Sigo.Create(3, "money", "USD"),
                Sigo.Create(3, "money", "VND")
            };

            Gen("money='USD'|'VND'");

            SigoAssert.Equal(expected, Gen("{money}"));
        }

        [Fact]
        public void Object_field_optional() {
            var expected = new[] {
                Sigo.Create(3),
                Sigo.Create(3, "x", 1),
                Sigo.Create(3, "y", 1),
                Sigo.Create(3, "x", 1, "y", 1)
            };

            SigoAssert.Equal(expected, Gen("{x?: 1, y?:1}"));
        }

        [Fact]
        public void Object_flag_() {
            var expected = new[] {
                Sigo.Create(0),
                Sigo.Create(3),
                Sigo.Create(7)
            };

            SigoAssert.Equal(expected, Gen("{037}"));
        }

        [Fact]
        public void Object_flag_any() {
            var expected = new[] {
                Sigo.Create(0),
                Sigo.Create(1),
                Sigo.Create(2),
                Sigo.Create(3),
                Sigo.Create(4),
                Sigo.Create(5),
                Sigo.Create(6),
                Sigo.Create(7)
            };

            SigoAssert.Equal(expected, Gen("{?}"));
        }

        [Fact]
        public void Object_flag_default() {
            var expected = new[] {
                Sigo.Create(3)
            };

            SigoAssert.Equal(expected, Gen("{ }"));
        }

        [Fact]
        public void Statements() {
            var src = "number=1|2; string='aa'|'bb'; 'ok'; number|string";

            var expected = new[] {
                Sigo.From(1),
                Sigo.From(2),
                Sigo.From("aa"),
                Sigo.From("bb")
            };

            SigoAssert.Equal(expected, Gen(src));
        }

        [Fact]
        public void Value_() {
            SigoAssert.Equal(Leafs(false), Gen("false"));
            SigoAssert.Equal(Leafs(true), Gen("true"));
            SigoAssert.Equal(Leafs(123), Gen("123"));
            SigoAssert.Equal(Leafs("abc"), Gen("'abc'"));
        }

        [Fact]
        public void Var_get_() {
            Gen("money='USD'|'VND'");

            SigoAssert.Equal(Leafs("bitcoin", "USD", "VND"), Gen("'bitcoin' | money"));
        }

        [Fact]
        public void Var_set_() {
            Gen("money='USD'|'VND'");

            SigoAssert.Equal(2, SigoSchema.Context["money"].Count());
        }

        [Fact]
        public void Var_set_return_empty() {
            SigoAssert.Equal(Leafs(), Gen("money='USD'|'VND'"));
        }
    }
}