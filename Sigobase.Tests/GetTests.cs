using System;
using System.Collections.Generic;
using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class GetTests {
        private ISigo V;
        private ISigo A;
        private ISigo B;
        private ISigo AB;

        private ISigo[] E;

        private List<ISigo> All;

        public GetTests() {
            V = Sigo.From("V");
            A = Sigo.Create(0).Set1("x", Sigo.From(1)).Set1("y", Sigo.From(2));
            B = Sigo.Create(0).Set1("x", Sigo.From(3)).Set1("y", Sigo.From(4));
            AB = Sigo.Create(3, "A", A, "B", B);

            E = new ISigo[8];
            for (var i = 0; i < 8; i++) {
                E[i] = Sigo.Create(i);
            }

            All = new List<ISigo> {A, B, AB, V};
            All.AddRange(E);
        }

        [Fact]
        public void Return_self() {
            var sigos = new[] {V, A, E[0]};
            var paths = new[] {null, "", "/"};

            foreach (var sigo in sigos) {
                foreach (var path in paths) {
                    SigoAssert.Same(sigo, sigo.Get(path));
                }
            }
        }

        [Fact]
        public void Return_using_Get1() {
            var sigos = new[] {A, V, E[0]};
            var paths = new[] {"x", "/x/", "z", "/z/"};

            foreach (var sigo in sigos) {
                foreach (var path in paths) {
                    var key = path.Replace("/", "");
                    SigoAssert.Same(sigo.Get1(key), sigo.Get(path));
                }
            }
        }

        [Fact]
        public void Return_using_Get1_Get1() {
            var sigos = new[] {AB, V, E[0]};
            var paths = new[] {
                "A/x", "A/y", "A/z",
                "B/x", "B/y", "B/z",
                "C/x", "C/y", "C/z"
            };

            foreach (var sigo in sigos) {
                foreach (var path in paths) {
                    var keys = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

                    SigoAssert.Same(
                        sigo.Get1(keys[0]).Get1(keys[1]),
                        sigo.Get(path)
                    );
                }
            }
        }
    }
}