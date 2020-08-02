using System;
using System.Collections.Generic;
using System.Linq;
using Sigobase.Database;
using Xunit;

namespace Sigobase.Generator.Tests
{
    public class SigoSchemaTests
    {
        public static IList<ISigo> Gen(string src) {
            return SigoSchema.Parse(src).Generate().ToList();
        }

        public static IList<ISigo> Leafs(params object[] values) {
            return values.Select(Sigo.From).ToList();
        }


        [Fact]
        public void ValueTest()
        {
            Equal(Leafs(false), Gen("false"));
            Equal(Leafs(true), Gen("true"));
            Equal(Leafs(123), Gen("123"));
            Equal(Leafs("abc"), Gen("'abc'"));
        }

        [Fact]
        public void ListTest() {
            var expected = Leafs(false, true, 123, "abc");

            Equal(expected, Gen(" false | true | 123 | 'abc' "));
        }

        [Fact]
        public void Flag_default_is_3() {
            var expected = new[] {
                Sigo.Create(3)
            };

            Equal(expected, Gen("{ }"));
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
                Sigo.Create(7),
            };

            Equal(expected, Gen("{?}"));
        }

        [Fact]
        public void FieldTest() {
            var expected = new[] {
                Sigo.Create(3, "x", 1),
                Sigo.Create(3, "x", 2),
                Sigo.Create(3, "x", 3)
            };

            Equal(expected, Gen("{x: 1|2|3}"));
        }

        [Fact]
        public void Field_optional_test() {
            var expected = new[] {
                Sigo.Create(3),
                Sigo.Create(3, "x", 1),
                Sigo.Create(3, "y", 1),
                Sigo.Create(3, "x", 1, "y", 1)
            };

            Equal(expected, Gen("{x?: 1, y?:1}"));
        }


        [Fact]
        public void Field_auto_test() {
            var expected = new[] {
                Sigo.Create(3, "money", "USD"),
                Sigo.Create(3, "money", "VND"),
            };

            Gen("money='USD'|'VND'");
            
            Equal(expected, Gen("{money}"));
        }


        [Fact]
        public void Assignment_and_reuse() {
            Gen("money='USD'|'VND'");

            Assert.Equal(2, SigoSchema.Context["money"].Count());

            Equal(Leafs("bitcoin", "USD", "VND"), Gen("'bitcoin' | money"));
        }

        [Fact]
        public void Assignment_return_empty() {
            Equal(Leafs(), Gen("money='USD'|'VND'"));
        }

        [Fact]
        public void Multiple_statements() {
            var src = "number=1|2; string='aa'|'bb';  number|string";

            var expected = new[] {
                Sigo.From(1),
                Sigo.From(2),
                Sigo.From("aa"),
                Sigo.From("bb"),
            };

            Equal(expected, Gen(src));
        }

        public static void Equal(IEnumerable<ISigo> a, IEnumerable<ISigo> b) {
            var la = a.ToList();
            var lb = b.ToList();
            Assert.Equal(la.Count, lb.Count);
            for (int i = 0; i < la.Count; i++) {
                Assert.Equal(la[i], lb[i]);
            }
        }

        public static void NotEqual(IEnumerable<ISigo> a, IEnumerable<ISigo> b) {
            var la = a.ToList();
            var lb = b.ToList();
            if (la.Count != lb.Count) return;

            for (int i = 0; i < la.Count; i++) {
                if (!Equals(la[i], lb[i])) return;
            }

            Assert.True(false);
        }

        [Fact]
        public void xUnit_compare_enumerable() {
            var a1 = Sigo.From(1);
            var a2 = Sigo.From(1);
            var b = Sigo.From(2);

            Assert.Equal(a1, a2);
            Assert.NotEqual(a1, b);

            Equal(new[] { a1 }, new[] { a2 });
            NotEqual(new[] { a1 }, new[] { b });


            Equal(new[] { a1 }, new[] { a2 });
            NotEqual(new[] { a1 }, new[] { b });


        }
    }
}
