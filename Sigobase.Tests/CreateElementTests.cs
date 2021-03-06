﻿using System;
using Sigobase.Database;
using Sigobase.Utils;
using Xunit;

namespace Sigobase.Tests {
    public class CreateElementTests {
        [Fact]
        public void AreFrozen() {
            for (var i = 0; i < 8; i++) {
                var sigo = Sigo.Create(i);
                SigoAssert.Equal(Bits.F + i, sigo.Flags);
            }
        }

        [Fact]
        public void AreSingleton() {
            for (var i = 0; i < 8; i++) {
                SigoAssert.Same(Sigo.Create(i), Sigo.Create(i));
            }
        }

        [Fact]
        public void ThrowsIfOutOfRange() {
            SigoAssert.Throws<ArgumentOutOfRangeException>(() => Sigo.Create(-1));
            SigoAssert.Throws<ArgumentOutOfRangeException>(() => Sigo.Create(8));
        }
    }
}