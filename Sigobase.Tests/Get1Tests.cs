using System;
using System.Collections.Generic;
using Sigobase.Database;
using Sigobase.Utils;
using Xunit;

namespace Sigobase.Tests {
    public class Get1Tests {
        [Fact]
        public void Throws_if_null_empty_or_has_slash() {
            // TODO this behaviour take some cpu cost. Add UnsafeGet1()?
            var sigos = new List<ISigo> {
                Sigo.From("v"),
                Sigo.Create(0),
                Sigo.Create(1),
                Sigo.Create(0).Set1("k", Sigo.From("v")),
                Sigo.Create(1).Set1("k", Sigo.From("v"))
            };

            foreach (var sigo in sigos) {
                SigoAssert.ThrowsAny<Exception>(() => sigo.Get1(null));
                SigoAssert.ThrowsAny<Exception>(() => sigo.Get1(""));
                SigoAssert.ThrowsAny<Exception>(() => sigo.Get1("/"));
                SigoAssert.ThrowsAny<Exception>(() => sigo.Get1("x/"));
            }
        }

        [Fact]
        public void Return_default() {
            var sigos = new List<ISigo> {
                Sigo.From("v"),
                Sigo.Create(0),
                Sigo.Create(1),
                Sigo.Create(0).Set1("k", Sigo.From("v")),
                Sigo.Create(1).Set1("k", Sigo.From("v"))
            };
            foreach (var sigo in sigos) {
                if ((sigo.Flags & Bits.R) == Bits.R) {
                    SigoAssert.Equal(Sigo.Create(3), sigo.Get1("nonExists"));
                } else {
                    SigoAssert.Equal(Sigo.Create(0), sigo.Get1("nonExists"));
                }
            }
        }

        [Fact]
        public void Return_the_child() {
            var sigo = Sigo.Create(0).Set1("k", Sigo.From("v"));

            SigoAssert.Equal("v", sigo.Get1("k").Data);
        }
    }
}