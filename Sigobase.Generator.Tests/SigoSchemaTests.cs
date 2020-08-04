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

        [Fact]
        public void ValueTest() {
            Utils.Equal(Leafs(false), Gen("false"));
            Utils.Equal(Leafs(true), Gen("true"));
            Utils.Equal(Leafs(123), Gen("123"));
            Utils.Equal(Leafs("abc"), Gen("'abc'"));
        }

        [Fact]
        public void ListTest() {
            var expected = Leafs(false, true, 123, "abc");

            Utils.Equal(expected, Gen(" false | true | 123 | 'abc' "));
        }

        [Fact]
        public void Flag_037() {
            var expected = new[] {
                Sigo.Create(0),
                Sigo.Create(3),
                Sigo.Create(7),

            };

            Utils.Equal(expected, Gen("{037}"));
        }

        [Fact]
        public void Flag_default_is_3() {
            var expected = new[] {
                Sigo.Create(3)
            };

            Utils.Equal(expected, Gen("{ }"));
        }

        [Fact]
        public void Flag_question_is_01234567() {
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

            Utils.Equal(expected, Gen("{?}"));
        }

        [Fact]
        public void FieldTest() {
            var expected = new[] {
                Sigo.Create(3, "x", 1),
                Sigo.Create(3, "x", 2),
                Sigo.Create(3, "x", 3)
            };

            Utils.Equal(expected, Gen("{x: 1|2|3}"));
        }

        [Fact]
        public void Field_optional_test() {
            var expected = new[] {
                Sigo.Create(3),
                Sigo.Create(3, "x", 1),
                Sigo.Create(3, "y", 1),
                Sigo.Create(3, "x", 1, "y", 1)
            };

            Utils.Equal(expected, Gen("{x?: 1, y?:1}"));
        }

        [Fact]
        public void Field_auto_test() {
            var expected = new[] {
                Sigo.Create(3, "money", "USD"),
                Sigo.Create(3, "money", "VND")
            };

            Gen("money='USD'|'VND'");

            Utils.Equal(expected, Gen("{money}"));
        }

        [Fact]
        public void Assignment_and_reuse() {
            Gen("money='USD'|'VND'");

            Assert.Equal(2, SigoSchema.Context["money"].Count());

            Utils.Equal(Leafs("bitcoin", "USD", "VND"), Gen("'bitcoin' | money"));
        }

        [Fact]
        public void Assignment_return_empty() {
            Utils.Equal(Leafs(), Gen("money='USD'|'VND'"));
        }

        [Fact]
        public void Multiple_statements() {
            var src = "number=1|2; string='aa'|'bb';  number|string";

            var expected = new[] {
                Sigo.From(1),
                Sigo.From(2),
                Sigo.From("aa"),
                Sigo.From("bb")
            };

            Utils.Equal(expected, Gen(src));
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
        public void OptionalSeparator(string a, string b) {
            var sa = SigoSchema.Parse(a);
            var sb = SigoSchema.Parse(b);
            Assert.Equal(sa.Count(), sb.Count());
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
        public void OptionalSeparatorWithQuestion(string a, string b) {
            var sa = SigoSchema.Parse(a);
            var sb = SigoSchema.Parse(b);
            Assert.Equal(sa.Count(), sb.Count());
        }

        [Theory]
        [InlineData("x=1|2 x", "x=1|2; x")]
        public void OptionalSemiconlon(string a, string b) {
            var sa = SigoSchema.Parse(a);
            var sb = SigoSchema.Parse(b);
            Assert.Equal(sa.Count(), sb.Count());
        }

    }
}