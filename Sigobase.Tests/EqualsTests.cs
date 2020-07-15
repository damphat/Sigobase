using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class EqualsTests {
        [Fact]
        public void Difference() {
            var diffs = new List<ISigo> {
                Sigo.From(true),
                Sigo.From(false),
                Sigo.From(1),
                Sigo.From(2),
                Sigo.From("a"),
                Sigo.From("b"),
            };

            for (int i = 0; i < 8; i++) {
                diffs.Add(Sigo.Create(i));
            }

            diffs.Add(Sigo.Create(0, "x", 1));
            diffs.Add(Sigo.Create(0, "x", 2));
            diffs.Add(Sigo.Create(0, "y", 1));
            diffs.Add(Sigo.Create(0, "y", 2));
            diffs.Add(Sigo.Create(1, "x", 1));
            diffs.Add(Sigo.Create(1, "x", 2));
            diffs.Add(Sigo.Create(1, "y", 1));
            diffs.Add(Sigo.Create(1, "y", 2));

            foreach (var a in diffs) {
                foreach (var b in diffs) {
                    if (!a.Same(b)) {
                        Assert.NotEqual(a, b);
                    }
                }
            }
        }

        [Fact]
        public void Leafs() {
            Assert.Equal(Sigo.From(10000), Sigo.From(10000));
            Assert.Equal(Sigo.From(true), Sigo.From(true));
            Assert.Equal(Sigo.From("abc"), Sigo.From("abc"));
        }

        [Fact]
        public void Trees() {
            var s1 = Sigo.Create(3,
                "a/x", 1,
                "a/y", 2,
                "b", 3
            );

            var s2 = Sigo.Create(3,
                "b", 3,
                "a/y", 2,
                "a/x", 1
            );

            Assert.Equal(s1, s2);

        }
    }
}