using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sigobase.Database;
using Xunit;

namespace Sigobase.Generator.Tests {
    public class GenerateOptionTests {
        private string src = "2|0|2|0";

        static ISigo[] Leafs(params object[] values) {
            return values.Select(value => Sigo.From(value)).ToArray();
        }

        [Fact]
        public void NoneTest() {
            Equal(Leafs(2, 0, 2, 0), SigoSchema.Parse(src).Generate(GenerateOptions.None));
        }

        [Fact]
        public void UniqueTest() {
            Equal(Leafs(2, 0), SigoSchema.Parse(src).Generate(GenerateOptions.Unique));
        }

        [Fact]
        public void SortedTest() {
            Equal(Leafs(0, 0, 2, 2), SigoSchema.Parse(src).Generate(GenerateOptions.Sorted));
        }

        [Fact]
        public void UniqueSortedTest() {
            Equal(Leafs(0, 2), SigoSchema.Parse(src).Generate(GenerateOptions.UniqueSorted));
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
