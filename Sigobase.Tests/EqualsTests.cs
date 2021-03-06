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

            diffs.Add(Sigo.Create(3, "x", 1));
            diffs.Add(Sigo.Create(3, "x", 2));
            diffs.Add(Sigo.Create(3, "y", 1));
            diffs.Add(Sigo.Create(3, "y", 2));

            foreach (var a in diffs) {
                foreach (var b in diffs) {
                    if (!a.Same(b)) {
                        SigoAssert.NotEqual(a, b);
                    }
                }
            }
        }

        [Fact]
        public void Leafs_compare_theirData() {
            SigoAssert.Equal(Sigo.From(10000), Sigo.From(10000));
            SigoAssert.Equal(Sigo.From(true), Sigo.From(true));
            SigoAssert.Equal(Sigo.From("abc"), Sigo.From("abc"));
        }

        [Fact]
        public void Trees_compare_theirProton() {
            var a = Sigo.Create(3, "k", "v");
            var b = Sigo.Create(0, "k", "v");

            SigoAssert.NotEqual(a, b);
        }

        [Fact]
        public void Trees_compare_theirChildren() {
            var a = Sigo.Create(0, "k", "v+");
            var b = Sigo.Create(0, "k", "v+");
            var c = Sigo.Create(0, "k", "v-");

            SigoAssert.Equal(a, b);
            SigoAssert.NotEqual(b, c);
        }

        [Fact]
        public void Trees_dontCompare_theirNeutron() {
            var a = Sigo.Create(3, "k", "v");
            var b = Sigo.Create(3, "k", "v").Freeze();
            SigoAssert.NotEqual(a.Flags, b.Flags);

            SigoAssert.Equal(a, b);
        }

        [Fact]
        public void Trees_dontCompare_theirChildOder() {
            var a = Sigo.Create(3, "k1", "v1", "k2", "v2");
            var b = Sigo.Create(3, "k2", "v2", "k1", "v1");
            SigoAssert.NotEqual(a.Keys.First(), b.Keys.First());

            SigoAssert.Equal(a, b);
        }

        [Fact]
        public void Trees_compare_deeply() {
            var a1 = Sigo.Create(0, "a/b/c/d", "v+");
            var a2 = Sigo.Create(0, "a/b/c/d", "v+");
            var b1 = Sigo.Create(0, "a/b/c/d", "v-");

            SigoAssert.Equal(a1, a2);
            SigoAssert.NotEqual(a1, b1);
        }

        [Fact]
        public void Overrided_equal_object() {
            var list = new List<ISigo> {
                Sigo.From("a"),
                Sigo.From("a"),
                Sigo.From("b"),
                Sigo.Create(3, "k", "a"),
                Sigo.Create(3, "k", "a"),
                Sigo.Create(3, "k", "b")
            };

            foreach (var a in list) {
                foreach (var b in list) {
                    SigoAssert.Equal(Sigo.Equals(a, b), a.Equals((object)b));
                }
            }
        }
    }
}