using System;
using System.Collections;
using System.Collections.Generic;
using Sigobase.Database;
using Sigobase.Utils;
using Xunit;

namespace Sigobase.Tests {
    public class CreateElementTests {
        [Fact]
        public void Return_8_frozen() {
            for (int i = 0; i < 8; i++) {
                var sigo = Sigo.Create(i);
                Assert.Equal(Bits.F + i, sigo.Flags);
            }
        }

        [Fact]
        public void Is_singleton() {
            for (int i = 0; i < 8; i++) {
                Assert.Same(Sigo.Create(i), Sigo.Create(i));
            }
        }

        [Fact]
        public void Throws_exceptions() {
            Assert.Throws<ArgumentOutOfRangeException>(() => Sigo.Create(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Sigo.Create(8));
        }
    }
}