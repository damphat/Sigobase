using System;
using System.Collections;
using System.Collections.Generic;
using Sigobase.Database;
using Sigobase.Utils;
using Xunit;

namespace Sigobase.Tests {
    public class Get1Tests {

        [Fact]
        public void Throws_if_null_empty_or_has_slash() {
            List<ISigo> sigos = new List<ISigo> {
                Sigo.From("v"),
                Sigo.Create(0),
                Sigo.Create(1),
                Sigo.Create(0).Set1("k", Sigo.From("v")),
                Sigo.Create(1).Set1("k", Sigo.From("v"))
            };

            foreach (var sigo in sigos) {
                Assert.ThrowsAny<Exception>(() => sigo.Get1(null));
                Assert.ThrowsAny<Exception>(() => sigo.Get1(""));
                Assert.ThrowsAny<Exception>(() => sigo.Get1("/"));
                Assert.ThrowsAny<Exception>(() => sigo.Get1("x/"));
            }

        }

        [Fact]
        public void Return_default() {
            List<ISigo> sigos = new List<ISigo> {
                Sigo.From("v"),
                Sigo.Create(0),
                Sigo.Create(1),
                Sigo.Create(0).Set1("k", Sigo.From("v")),
                Sigo.Create(1).Set1("k", Sigo.From("v"))
            };
            foreach (var sigo in sigos) {
                if ((sigo.Flags & Bits.R) == Bits.R) {
                    Assert.Equal(Sigo.Create(3), sigo.Get1("nooo"));
                } else {
                    Assert.Equal(Sigo.Create(0), sigo.Get1("nooo"));
                }
            }
        }

        [Fact]
        public void Return_the_child() {
            var sigo = Sigo.Create(0).Set1("k", Sigo.From("v"));

            Assert.Equal("v", sigo.Get1("k").Data);
        }

    }
}