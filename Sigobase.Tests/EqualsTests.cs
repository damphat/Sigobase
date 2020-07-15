﻿using System.Collections.Generic;
using System.Linq;
using Sigobase.Database;
using Xunit;

namespace Sigobase.Tests {
    public class EqualsTests {
        [Fact]
        public void Compare_aListOf_differenceValues() {
            var diffs = new List<ISigo> {
                Sigo.From(true),
                Sigo.From(false),
                Sigo.From(1),
                Sigo.From(2),
                Sigo.From("a"),
                Sigo.From("b")
            };

            for (var i = 0; i < 8; i++) {
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
        public void Leafs_compare_data() {
            Assert.Equal(Sigo.From(10000), Sigo.From(10000));
            Assert.Equal(Sigo.From(true), Sigo.From(true));
            Assert.Equal(Sigo.From("abc"), Sigo.From("abc"));
        }

        [Fact]
        public void Trees_compare_protonBits() {
            var a = Sigo.Create(3, "k", "v");
            var b = Sigo.Create(0, "k", "v");

            Assert.NotEqual(a, b);
        }

        [Fact]
        public void Trees_compare_childEquality() {
            var a = Sigo.Create(0, "k", "v+");
            var b = Sigo.Create(0, "k", "v+");
            var c = Sigo.Create(0, "k", "v-");

            Assert.Equal(a, b);
            Assert.NotEqual(b, c);
        }


        [Fact]
        public void Trees_doNotCompare_neutronBits() {
            var a = Sigo.Create(3, "k", "v");
            var b = Sigo.Create(3, "k", "v").Freeze();
            Assert.NotEqual(a.Flags, b.Flags);

            Assert.Equal(a, b);
        }

        [Fact]
        public void Trees_doNotCompare_childOder() {
            var a = Sigo.Create(3, "k1", "v1", "k2", "v2");
            var b = Sigo.Create(3, "k2", "v2", "k1", "v1");
            Assert.NotEqual(a.Keys.First(), b.Keys.First());

            Assert.Equal(a, b);
        }


        [Fact]
        public void Trees_compare_deeply() {
            var a = Sigo.Create(0, "a/b/c/d", "v+");
            var b = Sigo.Create(0, "a/b/c/d", "v+");
            var c = Sigo.Create(0, "a/b/c/d", "v-");

            Assert.Equal(a, b);
            Assert.NotEqual(b, c);
        }
    }
}