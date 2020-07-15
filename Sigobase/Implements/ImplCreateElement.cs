using System;
using System.Diagnostics;
using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.Implements {
    public static class ImplCreateElement {
        private static readonly ISigo[] Elements;

        static ImplCreateElement() {
            Debug.Assert(Bits.LMR < 8);
            Elements = new ISigo[8];
            for (var i = 0; i < 8; i++) {
                Elements[i] = new SigoTree(i).Freeze();
            }
        }

        public static ISigo Create(int lmr) {
            if (lmr < 0 || lmr > 7) {
                throw new ArgumentOutOfRangeException("lmr", "must in the range of [0..7]");
            }

            return Elements[Bits.Proton(lmr)];
        }
    }
}